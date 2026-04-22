using System;
using System.Xml.Serialization;

namespace MinsaitToDDL.Lib.Models.Minsait.Invoice
{
    public class InvoiceHeader
    {
        [XmlElement("InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        [XmlElement("TypeOfDocument")]
        public string TypeOfDocument { get; set; } = "380";

        [XmlElement("InvoiceType")]
        public string InvoiceType { get; set; } = "9";

        [XmlElement("InvoiceDate")]
        public DateTime InvoiceDate { get; set; }

        //[XmlElement("DocType")]
        //public string DocType { get; set; } = "221";

        [XmlElement("InvoiceCurrency")]
        public string InvoiceCurrency { get; set; } = "EUR";

        [XmlElement("OtherInvoiceDates")]
        public OtherInvoiceDates OtherInvoiceDates { get; set; }

        //[XmlElement("PaymentInstructions")]
        //public PaymentInstructions PaymentInstructions { get; set; }

        //[XmlElement(ElementName = "HeaderTaxes")]
        //public HeaderTaxes HeaderTaxes { get; set; }

        [XmlElement("BuyerInformation")]
        public Common.Party BuyerInformation { get; set; }

        [XmlElement("SellerInformation")]
        public Common.Party SellerInformation { get; set; }

        [XmlElement("BillToPartyInformation")]
        public Common.Party BillToPartyInformation { get; set; }

        [XmlElement("DeliveryPlaceInformation")]
        public Common.Party DeliveryPlaceInformation { get; set; }
    }
}