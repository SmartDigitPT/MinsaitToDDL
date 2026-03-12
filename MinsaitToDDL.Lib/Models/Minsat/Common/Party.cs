using System.Xml.Serialization;

namespace MinsaitToDDL.Lib.Models.Minsait.Common
{
    public class Party
    {
        [XmlElement("EANCode")]
        public string EANCode { get; set; }

        [XmlElement("Department")]
        public string Department { get; set; }

        [XmlElement("InternalCode")]
        public string InternalCode { get; set; }

        [XmlElement("NIF")]
        public string NIF { get; set; }

        //[XmlElement("Name")]
        //public string Name { get; set; }

        //[XmlElement("Street")]
        //public string Street { get; set; }

        //[XmlElement("PostalCode")]
        //public string PostalCode { get; set; }

        //[XmlElement("City")]
        //public string City { get; set; }

        //[XmlElement("Country")]
        //public string Country { get; set; }
    }
}