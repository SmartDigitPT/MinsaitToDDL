using MinsaitToDDL.Lib.Interfaces;
using MinsaitToDDL.Lib.Models;
using MinsaitToDDL.Lib.Parsers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;

namespace MinsaitToDDL.Lib
{
    public class MinsaitParser
    {
        private readonly List<IMinsaitDocumentParser> _parsers;

        public MinsaitParser()
        {
            _parsers = new List<IMinsaitDocumentParser>
            {
                new MinsaitInvoiceParser(),
                new MinsaitOrderParser(),
                // new MinsaitDesadvParser()
            };
        }

        public ItemTransaction Parse(string xml, Enums.Enums.DocumentType documentType, byte[] xsdBytes)
        {
            var doc = XDocument.Parse(xml);

            ValidateXmlAgainstXsd(doc, xsdBytes);

            var root = doc.Root;

            switch (documentType)
            {
                case Enums.Enums.DocumentType.INVOICE:
                    var invoiceParser = _parsers.OfType<MinsaitInvoiceParser>().FirstOrDefault();
                    if (invoiceParser != null)
                    {
                        return invoiceParser.Parse(xml);
                    }
                    break;
                case Enums.Enums.DocumentType.ORDER:
                    var orderParser = _parsers.OfType<MinsaitOrderParser>().FirstOrDefault();
                    if (orderParser != null)
                    {
                        return orderParser.Parse(xml);
                    }
                    break;
                // Adicione outros casos conforme necessário
                default:
                    throw new InvalidOperationException("Unsupported document type: " + documentType);
            }

            var parser = _parsers.FirstOrDefault(p => p.CanParse(root));

            if (parser == null)
                throw new InvalidOperationException(
                    "Unsupported Minsait document type: " + root.Name.LocalName);

            return parser.Parse(xml);
        }

        public string MapToXml(ItemTransaction transaction, Enums.Enums.DocumentType documentType)
        {
            switch (documentType)
            {
                case Enums.Enums.DocumentType.INVOICE:
                    var invoiceParser = _parsers.OfType<MinsaitInvoiceParser>().FirstOrDefault();
                    if (invoiceParser != null)
                    {
                        return invoiceParser.ParseFromDdl(transaction);
                    }
                    break;
                case Enums.Enums.DocumentType.ORDER:
                    var orderParser = _parsers.OfType<MinsaitOrderParser>().FirstOrDefault();
                    if (orderParser != null)
                    {
                        return orderParser.ParseFromDdl(transaction);
                    }
                    break;
                // Adicione outros casos conforme necessário
                default:
                    throw new InvalidOperationException("Unsupported document type: " + documentType);
            }

            return null;
        }
            
        public Tuple<bool, string, string> MapToXmlFromJson(string json, Enums.Enums.DocumentType documentType, byte[] xsdBytes)
        {
            var xml = string.Empty;

            try
            {
                var transaction = JsonConvert.DeserializeObject<ItemTransaction>(json);
                if (transaction == null)
                    throw new ArgumentException("Invalid JSON for ItemTransaction.", nameof(json));

                xml = MapToXml(transaction, documentType);

                if (xsdBytes != null)
                    ValidateXmlAgainstXsd(XDocument.Parse(xml), xsdBytes);
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, string.Empty, ex.Message);
            }

            return Tuple.Create(true, xml, string.Empty);
        }

        private static void ValidateXmlAgainstXsd(XDocument doc, byte[] xsdBytes)
        {
            using (var ms = new System.IO.MemoryStream(xsdBytes))
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
    }
}