using AutoMapper;
using MinsaitToDDL.Lib.Interfaces;
using MinsaitToDDL.Lib.Models;
using MinsaitToDDL.Lib.Models.Minsait.Order;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MinsaitToDDL.Lib.Parsers
{
    public class MinsaitOrderParser : IMinsaitDocumentParser
    {
        public bool CanParse(XElement root)
        {
            // Ajusta se o root tiver outro nome
            return root.Name.LocalName == "Order";
        }

        public ItemTransaction Parse(string xml)
        {
            var serializer = new XmlSerializer(typeof(Order));
            Order document;

            using (var reader = new StringReader(xml))
            {
                document = (Order)serializer.Deserialize(reader);
            }

            var mapper = CreateMapper();
            return mapper.Map<ItemTransaction>(document);
        }

        public string ParseFromDdl(ItemTransaction transaction)
        {
            var mapper = CreateMapper();
            var document = mapper.Map<Order>(transaction);

            var serializer = new XmlSerializer(typeof(Order));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, document);
                return writer.ToString();
            }
        }

        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Order → ItemTransaction
                cfg.CreateMap<Order, ItemTransaction>()
                    .ForMember(d => d.CreateDate,
                        o => o.MapFrom(s => s.OrderHeader.OrderDate))
                    .ForMember(d => d.ActualDeliveryDate,
                        o => o.MapFrom(s => s.OrderHeader.OtherOrderDates != null ? s.OrderHeader.OtherOrderDates.DeliveryDate : (DateTime?)null))
                    .ForMember(d => d.ISignableTransactionTransactionID,
                        o => o.MapFrom(s => s.OrderHeader.OrderNumber))
                    .ForMember(d => d.TotalGrossAmount,
                        o => o.MapFrom(s => s.OrderSummary.OrderTotals.NetValue))
                    .ForMember(d => d.TotalAmount,
                        o => o.MapFrom(s => s.OrderSummary.OrderTotals.GrossValue))
                    .ForMember(d => d.Party,
                        o => o.MapFrom(s => MapParty(s.OrderHeader.BuyerInformation)))
                    .ForMember(d => d.PartyGLN,
                    o => o.MapFrom(s => s.OrderHeader != null && s.OrderHeader.BuyerInformation != null
                        ? s.OrderHeader.BuyerInformation.EANCode
                        : null))
                    .ForMember(d => d.SupplierParty,
                        o => o.MapFrom(s => MapParty(s.OrderHeader.SellerInformation)))
                    //.ForMember(d => d.LoadPlaceAddress,
                    //    o => o.MapFrom(s => MapParty(s.OrderHeader.DeliveryPlaceInformation)))
                    .ForMember(d => d.Details,
                        o => o.MapFrom(s => MapOrderLines(
                            s.OrderDetail != null ? s.OrderDetail.ItemDetails : null)))
                    .ForMember(d => d.Taxes,
                        o => o.MapFrom(s => s.OrderHeader != null
                            && s.OrderHeader.HeaderTaxes != null
                            && s.OrderHeader.HeaderTaxes.HeaderTaxesHeader != null
                            ? s.OrderHeader.HeaderTaxes.HeaderTaxesHeader.ConvertAll(
                                h => new TaxValue { TaxRate = h.TaxPercent })
                            : null))
                    .ForMember(d => d.Payment,
                        o => o.MapFrom(s => MapPayment(s.OrderHeader)))
                    .ForAllOtherMembers(o => o.Ignore());

                // ItemTransaction → Order (already present)
                cfg.CreateMap<ItemTransaction, Order>()
                    .ForPath(d => d.OrderHeader.OrderDate,
                        o => o.MapFrom(s => s.CreateDate))
                    .ForPath(d => d.OrderHeader.OtherOrderDates.DeliveryDate,
                        o => o.MapFrom(s => s.ActualDeliveryDate))
                    .ForPath(d => d.OrderHeader.OtherOrderDates.LastAcceptableDeliveryDate,
                        o => o.MapFrom(s => s.ActualDeliveryDate))
                    .ForPath(d => d.OrderHeader.DocType,
                        o => o.MapFrom(_ => "221"))
                    .ForPath(d => d.OrderHeader.OrderType,
                        o => o.MapFrom(_ => "9"))
                    .ForPath(d => d.OrderHeader.OrderCurrency,
                        o => o.MapFrom(_ => "EUR"))
                    .ForPath(d => d.OrderHeader.PaymentInstructions.PaymentTerm,
                        o => o.MapFrom(s => ((int)s.Payment.PaymentDays).ToString()))
                    .ForPath(d => d.OrderHeader.OrderNumber,
                        o => o.MapFrom(s => s.ISignableTransactionTransactionID))
                    .ForPath(d => d.OrderSummary.NumberOfLines,
                        o => o.MapFrom(s => s.Details.Count))
                    .ForPath(d => d.OrderSummary.OrderTotals.NetValue,
                        o => o.MapFrom(s => s.TotalGrossAmount))
                    .ForPath(d => d.OrderSummary.OrderTotals.GrossValue,
                        o => o.MapFrom(s => s.TotalAmount))
                    .ForPath(d => d.OrderHeader.BuyerInformation,
                        o => o.MapFrom(s => MapPartyReverse(s.Party, s.PartyGLN)))
                    .ForPath(d => d.OrderHeader.SellerInformation,
                        o => o.MapFrom(s => MapPartyReverse(s.SupplierParty, s.LoadPlaceAddress.GLN)))
                    .ForPath(d => d.OrderHeader.DeliveryPlaceInformation,
                        o => o.MapFrom(s => MapPartyReverse(s.SupplierParty, s.PartyGLN)))
                    .ForPath(d => d.OrderHeader.BillToPartyInformation,
                        o => o.MapFrom(s => MapPartyReverse(s.SupplierParty, s.PartyGLN)))
                    .ForPath(d => d.OrderHeader.HeaderTaxes,
                        o => o.MapFrom(s => MapOrderHeaderTaxesReverse(s.Taxes)))
                    .ForPath(d => d.OrderDetail.ItemDetails,
                        o => o.MapFrom(s => MapOrderLinesReverse(s.Details)));
            });

            return config.CreateMapper();
        }

        #region "Forward"

        private static Party MapParty(Models.Minsait.Common.Party party)
        {
            if (party == null) return null;

            return new Party
            {
                GLN = party.EANCode,
                // Add other mappings if needed
            };
        }

        private static List<Detail> MapOrderLines(IEnumerable<ItemDetail> items)
        {
            var list = new List<Detail>();
            if (items == null) return list;

            foreach (var i in items)
            {
                var detail = new Detail
                {
                    LineItemID = i.LineItemNum,
                    ItemID = i.StandardPartNumber,
                    //BuyerItemID = i.BuyerPartNumber,
                    SupplierItemID = i.SellerPartNumber,
                    Description = i.ItemDescriptions?.Description,
                    Quantity = (double?)i.Quantity?.QuantityValue,
                    UnitPrice = i.Price?.NetPrice,
                    TotalNetAmount = i.MonetaryAmount
                };

                list.Add(detail);
            }

            return list;
        }

        private static Payment MapPayment(OrderHeader header)
        {
            if (header == null || header.PaymentInstructions == null)
                return new Payment { PaymentDays = 0 };

            int days = 0;
            int.TryParse(header.PaymentInstructions.PaymentTerm, out days);

            return new Payment
            {
                PaymentDays = days
            };
        }

        #endregion

        #region "Reverse"

        private static Models.Minsait.Common.Party MapPartyReverse(Party party, string partyGLN)
        {
            //if (party == null) return null;

            return new Models.Minsait.Common.Party
            {
                EANCode = partyGLN,
                // InternalCode = party.PartyID,
                // Department = party.Department
            };
        }

        private static List<ItemDetail> MapOrderLinesReverse(IEnumerable<Detail> details)
        {
            var list = new List<ItemDetail>();
            if (details == null) return list;

            foreach (var d in details)
            {
                list.Add(new ItemDetail
                {
                    LineItemNum = (int)(d.LineItemID != null ? d.LineItemID.Value : 0),
                    StandardPartNumber = d.ItemID,
                    BuyerPartNumber = d.ItemID,
                    SellerPartNumber = d.Description,

                    ItemDescriptions = d.Description != null
                        ? new Models.Minsait.Common.ItemDescriptions
                        {
                            Description = d.Description
                        }
                        : null,
                    Quantity = d.Quantity != null
                        ? new Models.Minsait.Common.Quantity
                        {
                            QuantityValue = (decimal)d.Quantity
                        }
                        : null,
                    Price = d.UnitPrice != null
                        ? new Models.Minsait.Common.Price
                        {
                            NetPrice = (d.UnitPrice != null ? d.UnitPrice.Value : 0),
                            GrossPrice = (d.TaxIncludedPrice != null ? d.TaxIncludedPrice.Value : 0),
                            PriceBasisQuantity = (d.Quantity != null ? d.Quantity.Value : 0),
                        }
                        : null,
                    MonetaryAmount = (d.TotalGrossAmount != null ? d.TotalGrossAmount.Value : 0),
                });
            }

            return list;
        }

        private static Models.Minsait.Common.HeaderTaxes MapOrderHeaderTaxesReverse(IEnumerable<TaxValue> taxes)
        {
            var headerTaxes = new Models.Minsait.Common.HeaderTaxes
            {
                HeaderTaxesHeader = new List<Models.Minsait.Common.HeaderTaxesHeader>()
            };

            if (taxes == null) return headerTaxes;

            foreach (var t in taxes)
            {
                headerTaxes.HeaderTaxesHeader.Add(new Models.Minsait.Common.HeaderTaxesHeader
                {
                    TaxPercent = t.TaxRate
                });
            }
            return headerTaxes;

        }

        #endregion
    }
}