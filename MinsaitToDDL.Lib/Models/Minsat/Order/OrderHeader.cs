using MinsaitToDDL.Lib.Models.Minsait.Common;
using MinsaitToDDL.Lib.Models.Minsait.Order;
using System;
using System.Xml.Serialization;

namespace MinsaitToDDL.Lib.Models.Minsait.Order
{
    public class OrderHeader
    {
        [XmlElement("OrderNumber")]
        public string OrderNumber { get; set; }

        [XmlElement("OrderDate")]
        public DateTime OrderDate { get; set; }

        [XmlElement("DocType")]
        public string DocType { get; set; } = "221";

        [XmlElement("OrderType")]
        public string OrderType { get; set; } = "9";

        [XmlElement("OrderCurrency")]
        public string OrderCurrency { get; set; } = "EUR";

        [XmlElement("OtherOrderDates")]
        public OtherOrderDates OtherOrderDates { get; set; }

        [XmlElement("BuyerInformation")]
        public Common.Party BuyerInformation { get; set; }

        [XmlElement("SellerInformation")]
        public Common.Party SellerInformation { get; set; }

        [XmlElement("DeliveryPlaceInformation")]
        public Common.Party DeliveryPlaceInformation { get; set; }

        [XmlElement("BillToPartyInformation")]
        public Common.Party BillToPartyInformation { get; set; }

        [XmlElement("PaymentInstructions")]
        public PaymentInstructions PaymentInstructions { get; set; }

        [XmlElement(ElementName = "HeaderTaxes")]
        public HeaderTaxes HeaderTaxes { get; set; }
    }
}