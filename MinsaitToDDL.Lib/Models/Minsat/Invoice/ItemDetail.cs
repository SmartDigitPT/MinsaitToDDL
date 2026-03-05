using MinsaitToDDL.Lib.Models.Minsait.Common;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MinsaitToDDL.Lib.Models.Minsait.Invoice
{
    public class ItemDetail
    {

        [XmlElement(ElementName = "LineItemNum")]
        public int LineItemNum { get; set; }

        [XmlElement(ElementName = "StandardPartNumber")]
        public string StandardPartNumber { get; set; }

        [XmlElement(ElementName = "BuyerPartNumber")]
        public string BuyerPartNumber { get; set; }

        [XmlElement(ElementName = "SellerPartNumber")]
        public string SellerPartNumber { get; set; }

        [XmlElement(ElementName = "ItemDescription")]
        public string ItemDescription { get; set; }

        [XmlElement(ElementName = "Quantity")]
        public Quantity Quantity { get; set; }

        [XmlElement(ElementName = "PackSize")]
        public PackSize PackSize { get; set; }

        [XmlElement(ElementName = "Price")]
        public Price Price { get; set; }

        [XmlElement(ElementName = "MonetaryAmount")]
        public double MonetaryAmount { get; set; }

        [XmlElement(ElementName = "Package")]
        public Package Package { get; set; }

        [XmlElement(ElementName = "LineAllowancesOrCharges")]
        public object LineAllowancesOrCharges { get; set; }

        [XmlElement(ElementName = "LineTaxes")]
        public object LineTaxes { get; set; }

        [XmlElement(ElementName = "LineFreeTexts")]
        public object LineFreeTexts { get; set; }
    }
}