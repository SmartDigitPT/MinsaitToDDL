using System.Xml.Serialization;

namespace MinsaitToDDL.Lib.Models.Minsait.Common
{
    public class Price
    {

        [XmlElement(ElementName = "NetPrice")]
        public double NetPrice { get; set; }

        [XmlElement(ElementName = "GrossPrice")]
        public double GrossPrice { get; set; }

        [XmlElement(ElementName = "PriceBasisQuantity")]
        public double PriceBasisQuantity { get; set; }
    }
}