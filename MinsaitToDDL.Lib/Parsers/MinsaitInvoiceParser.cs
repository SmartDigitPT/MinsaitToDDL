using AutoMapper;
using MinsaitToDDL.Lib.Interfaces;
using MinsaitToDDL.Lib.Models;
using MinsaitToDDL.Lib.Models.Minsait.Invoice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MinsaitToDDL.Lib.Parsers
{
    public class MinsaitInvoiceParser : IMinsaitDocumentParser
    {
        public bool CanParse(XElement root)
        {
            return root.Name.LocalName == "Invoice";
        }

        public ItemTransaction Parse(string xml)
        {
            var serializer = new XmlSerializer(typeof(Invoice));
            Invoice document;

            using (var reader = new StringReader(xml))
            {
                document = (Invoice)serializer.Deserialize(reader);
            }

            var mapper = CreateMapper();
            return mapper.Map<ItemTransaction>(document);
        }

        public string ParseFromDdl(ItemTransaction transaction)
        {
            var mapper = CreateMapper();
            var document = mapper.Map<Invoice>(transaction);

            var serializer = new XmlSerializer(typeof(Invoice));

            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false), // sem BOM
                Indent = true
            };

            using (var stream = new MemoryStream())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, document);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Invoice → ItemTransaction
                cfg.CreateMap<Invoice, ItemTransaction>()
                    .ForMember(d => d.CreateDate,
                        o => o.MapFrom(s => s.InvoiceHeader.InvoiceDate))
                    .ForMember(d => d.ActualDeliveryDate,
                        o => o.MapFrom(s => s.InvoiceHeader.OtherInvoiceDates != null ? s.InvoiceHeader.OtherInvoiceDates.DeliveryDate : (DateTime?)null))
                    .ForMember(d => d.ISignableTransactionTransactionID,
                        o => o.MapFrom(s => s.InvoiceHeader.InvoiceNumber))
                    .ForMember(d => d.TotalGrossAmount,
                        o => o.MapFrom(s => s.InvoiceSummary.InvoiceTotals.NetValue))
                    .ForMember(d => d.TotalAmount,
                        o => o.MapFrom(s => s.InvoiceSummary.InvoiceTotals.GrossValue))
                    .ForMember(d => d.Party,
                        o => o.MapFrom(s => MapParty(s.InvoiceHeader.BuyerInformation)))
                    .ForMember(d => d.PartyGLN,
                        o => o.MapFrom(s => s.InvoiceHeader != null && s.InvoiceHeader.BuyerInformation != null
                            ? s.InvoiceHeader.BuyerInformation.EANCode
                            : null))
                    .ForMember(d => d.BillToPartyFederalTaxID,
                        o => o.MapFrom(s => s.InvoiceHeader != null && s.InvoiceHeader.BuyerInformation != null
                            ? s.InvoiceHeader.BuyerInformation.NIF
                            : null))
                    .ForMember(d => d.SupplierParty,
                        o => o.MapFrom(s => MapParty(s.InvoiceHeader.SellerInformation)))
                    //.ForMember(d => d.LoadPlaceAddress,
                    //    o => o.MapFrom(s => MapParty(s.InvoiceHeader.DeliveryPlaceInformation)))
                    .ForMember(d => d.Details,
                        o => o.MapFrom(s => MapInvoiceLines(
                            s.InvoiceDetail != null ? s.InvoiceDetail.ItemDetails : null)))
                    //.ForMember(d => d.Taxes,
                    //    o => o.MapFrom(s => s.InvoiceHeader != null
                    //        && s.InvoiceHeader.HeaderTaxes != null
                    //        && s.InvoiceHeader.HeaderTaxes.HeaderTaxesHeader != null
                    //        ? s.InvoiceHeader.HeaderTaxes.HeaderTaxesHeader.ConvertAll(
                    //            h => new TaxValue { TaxRate = h.TaxPercent })
                    //        : null))
                    //.ForMember(d => d.Payment,
                    //    o => o.MapFrom(s => MapPayment(s.InvoiceHeader)))
                    .ForAllOtherMembers(o => o.Ignore());

                // ItemTransaction → Invoice (already present)
                cfg.CreateMap<ItemTransaction, Invoice>()
                    .ForPath(d => d.InvoiceHeader.InvoiceDate,
                        o => o.MapFrom(s => s.CreateDate))
                    .ForPath(d => d.InvoiceHeader.OtherInvoiceDates.DeliveryDate,
                        o => o.MapFrom(s => s.ActualDeliveryDate))
                    //.ForPath(d => d.InvoiceHeader.OtherInvoiceDates.LastAcceptableDeliveryDate,
                    //    o => o.MapFrom(s => s.ActualDeliveryDate))
                    //.ForPath(d => d.InvoiceHeader.DocType,
                    //    o => o.MapFrom(_ => "221"))
                    .ForPath(d => d.InvoiceHeader.TypeOfDocument,
                        o => o.MapFrom(_ => "380"))
                    .ForPath(d => d.InvoiceHeader.InvoiceType,
                        o => o.MapFrom(_ => "9"))
                    .ForPath(d => d.InvoiceHeader.InvoiceCurrency,
                        o => o.MapFrom(_ => "EUR"))
                    //.ForPath(d => d.InvoiceHeader.PaymentInstructions.PaymentTerm,
                    //    o => o.MapFrom(s => ((int)s.Payment.PaymentDays).ToString()))
                    .ForPath(d => d.InvoiceHeader.InvoiceNumber,
                        o => o.MapFrom(s => s.ISignableTransactionTransactionID))
                    .ForPath(d => d.InvoiceSummary.NumberOfLines,
                        o => o.MapFrom(s => s.Details.Count))
                    .ForPath(d => d.InvoiceSummary.InvoiceTotals.NetValue,
                        o => o.MapFrom(s => s.TotalGrossAmount))
                    .ForPath(d => d.InvoiceSummary.InvoiceTotals.GrossValue,
                        o => o.MapFrom(s => s.TotalAmount))
                    .ForPath(d => d.InvoiceHeader.BuyerInformation,
                        o => o.MapFrom(s => MapPartyReverse(s.Party, s.PartyGLN, s.BillToPartyFederalTaxID)))
                    .ForPath(d => d.InvoiceHeader.SellerInformation,
                        o => o.MapFrom(s => MapPartyReverse(s.SupplierParty, s.LoadPlaceAddress.GLN, s.PartyFederalTaxID)))
                    //.ForPath(d => d.InvoiceHeader.DeliveryPlaceInformation,
                    //    o => o.MapFrom(s => MapPartyReverse(s.SupplierParty, s.PartyGLN)))
                    //.ForPath(d => d.InvoiceHeader.BillToPartyInformation,
                    //    o => o.MapFrom(s => MapPartyReverse(s.SupplierParty, s.PartyGLN)))
                    //.ForPath(d => d.InvoiceHeader.HeaderTaxes,
                    //    o => o.MapFrom(s => MapInvoiceHeaderTaxesReverse(s.Taxes)))
                    .ForPath(d => d.InvoiceDetail.ItemDetails,
                        o => o.MapFrom(s => MapInvoiceLinesReverse(s.Details)))
                    .ForAllOtherMembers(o => o.Ignore());
            });

            return config.CreateMapper();
        }

        public ItemTransaction Parse(string xml, byte[] xsdBytes)
        {
            ValidateXmlAgainstXsd(xml, xsdBytes);
            return Parse(xml);
        }

        private static void ValidateXmlAgainstXsd(string xml, byte[] xsdBytes)
        {
            var doc = XDocument.Parse(xml);
            using (var ms = new MemoryStream(xsdBytes))
            using (var reader = System.Xml.XmlReader.Create(ms))
            {
                var schemas = new XmlSchemaSet();
                schemas.Add(null, reader);
                string validationErrors = string.Empty;
                doc.Validate(schemas, (o, e) => {
                    validationErrors += e.Message + "\n";
                });
                if (!string.IsNullOrEmpty(validationErrors))
                {
                    throw new InvalidOperationException("XML validation against XSD failed: " + validationErrors);
                }
            }
        }

        #region "Forward"

        private static Party MapParty(Models.Minsait.Common.Party party)
        {
            if (party == null) return null;

            return new Party
            {
                GLN = party.EANCode,
                FederalTaxID = party.NIF,
                // Add other mappings if needed
            };
        }

        private static List<Detail> MapInvoiceLines(IEnumerable<ItemDetail> items)
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
                    Description = i.ItemDescription,
                    Quantity = (double?)i.Quantity?.QuantityValue,
                    UnitPrice = i.Price?.NetPrice,
                    TotalNetAmount = i.MonetaryAmount
                };

                list.Add(detail);
            }

            return list;
        }

        //private static Payment MapPayment(InvoiceHeader header)
        //{
        //    if (header == null || header.PaymentInstructions == null)
        //        return new Payment { PaymentDays = 0 };

        //    int days = 0;
        //    int.TryParse(header.PaymentInstructions.PaymentTerm, out days);

        //    return new Payment
        //    {
        //        PaymentDays = days
        //    };
        //}

        #endregion

        #region "Reverse"

        private static Models.Minsait.Common.Party MapPartyReverse(Party party, string partyGLN, string federalTaxID)
        {
            //if (party == null) return null;

            return new Models.Minsait.Common.Party
            {
                EANCode = partyGLN,
                NIF = federalTaxID,
                // InternalCode = party.PartyID,
                // Department = party.Department
            };
        }

        private static List<ItemDetail> MapInvoiceLinesReverse(IEnumerable<Detail> details)
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

                    ItemDescription = d.Description,
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
                            //PriceBasisQuantity = (d.Quantity != null ? d.Quantity.Value : 0),
                        }
                        : null,
                    MonetaryAmount = (d.TotalGrossAmount != null ? d.TotalGrossAmount.Value : 0),
                });
            }

            return list;
        }

        private static Models.Minsait.Common.HeaderTaxes MapInvoiceHeaderTaxesReverse(IEnumerable<TaxValue> taxes)
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