using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MinsaitToDDL.Lib.Models.Minsait.Invoice
{
    public class OtherInvoiceDates
    {
        [XmlElement("DeliveryDate")]
        public DateTime? DeliveryDate { get; set; }

        [XmlElement("InvoiceDueDate")]
        public DateTime? InvoiceDueDate { get; set; }

        [XmlElement("InvoiceDueDays")]
        public int? InvoiceDueDays { get; set; }

        public bool ShouldSerializeInvoiceDueDate()
        {
            return InvoiceDueDate.HasValue;
        }

        public bool ShouldSerializeInvoiceDueDays()
        {
            return InvoiceDueDays.HasValue;
        }
    }
}