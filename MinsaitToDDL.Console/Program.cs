using Newtonsoft.Json;
using MinsaitToDDL.Lib;
using MinsaitToDDL.Lib.Models;
using MinsaitToDDL.Lib.Validation;

namespace MinsaitToDDL.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var minsaitParser = new MinsaitParser();
            //var reverseParser = new MinsaitReverseParser();

            var serializeOptions = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            while (true)
            {
                System.Console.Clear();
                System.Console.WriteLine("=== Minsait / DDL Conversion Console ===");
                System.Console.WriteLine("Select an option:");
                System.Console.WriteLine("1) Parse Minsait INVOICE XML -> DDL");
                System.Console.WriteLine("2) Parse Minsait ORDER XML -> DDL");
                System.Console.WriteLine("3) Parse DDL (from INVOICE sample) -> XML + XSD validation");
                System.Console.WriteLine("4) Parse DDL (from ORDER sample) -> XML + XSD validation");
                //System.Console.WriteLine("5) Parse DDL JSON string -> XML + XSD validation");
                System.Console.WriteLine("6) Parse REAL Minsait ORDER sample -> DDL -> XML + XSD");
                //System.Console.WriteLine("7) Load XML from file -> DDL -> XML + XSD");
                System.Console.WriteLine("0) Exit");
                System.Console.Write("> ");


                var choice = System.Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ProcessMinsaitXml(
                            minsaitParser,
                            Properties.Resources.MinsaitInvoiceSample,
                            serializeOptions);
                        break;

                    case "2":
                        ProcessMinsaitXml(
                            minsaitParser,
                            Properties.Resources.MinsaitOrderSample,
                            serializeOptions);
                        break;

                    case "3":
                        ProcessDdlFromMinsaitSample(
                            minsaitParser,
                            //reverseParser,
                            Properties.Resources.MinsaitInvoiceSample,
                            serializeOptions, Lib.Enums.Enums.DocumentType.INVOICE);
                        break;

                    case "4":
                        ProcessDdlFromMinsaitSample(
                            minsaitParser,
                            //reverseParser,
                            Properties.Resources.MinsaitOrderSample,
                            serializeOptions, Lib.Enums.Enums.DocumentType.ORDER);
                        break;

                    //case "5":
                    //    //ProcessDdlFromString(reverseParser);
                    //    ProcessDdlFromString();
                    //    break;

                    case "6":
                        ProcessDdlFromMinsaitSample(
                            minsaitParser,
                            //reverseParser,
                            Properties.Resources.MinsaitOrderSampleReal,
                            serializeOptions, Lib.Enums.Enums.DocumentType.ORDER);
                        break;

                    case "0":
                        System.Console.WriteLine("Exiting...");
                        return;

                    default:
                        System.Console.WriteLine("Invalid option. Press Enter to try again...");
                        System.Console.ReadLine();
                        break;
                }
            }
        }

        static void ProcessMinsaitXml(
            MinsaitParser minsaitParser,
            string xmlSample,
            JsonSerializerSettings serializeOptions)
        {
            System.Console.WriteLine("\n=== Parsing Minsait XML -> DDL ===");

            var itemTransaction = minsaitParser.Parse(xmlSample, Lib.Enums.Enums.DocumentType.INVOICE, Properties.Resources.MinsaitInvoiveXsd);

            var json = JsonConvert.SerializeObject(
                itemTransaction,
                Formatting.Indented,
                serializeOptions);

            System.Console.WriteLine("\n--- DDL JSON Output ---");
            System.Console.WriteLine(json);

            System.Console.WriteLine("\nPress Enter to continue...");
            System.Console.ReadLine();
        }

        static void ProcessDdlFromMinsaitSample(
            MinsaitParser minsaitParser,
            //MinsaitReverseParser reverseParser,
            string xmlSample,
            JsonSerializerSettings serializeOptions,
            Lib.Enums.Enums.DocumentType documentType)
        {
            System.Console.WriteLine("\n=== Parsing Minsait XML -> DDL -> XML ===");

            var itemTransaction = minsaitParser.Parse(xmlSample, Lib.Enums.Enums.DocumentType.INVOICE, Properties.Resources.MinsaitInvoiveXsd);

            var ddlJson = JsonConvert.SerializeObject(
                itemTransaction,
                Formatting.Indented,
                serializeOptions);

            System.Console.WriteLine("\n--- Generated DDL JSON ---");
            System.Console.WriteLine(ddlJson);

            var ddlObject = JsonConvert.DeserializeObject<ItemTransaction>(ddlJson);

            //GenerateXmlAndValidate(reverseParser, ddlObject);
            GenerateXmlAndValidate(minsaitParser, ddlObject, documentType);
        }

        //static void ProcessDdlFromString(
        //    //MinsaitReverseParser reverseParser
        //)
        //{
        //    System.Console.WriteLine("\n=== Parsing DDL JSON String -> XML ===");

        //    var ddlJsonString = Properties.Resources.dllSampleString2;

        //    System.Console.WriteLine("\n--- Loaded DDL JSON ---");
        //    System.Console.WriteLine(ddlJsonString);

        //    var ddlObject = JsonConvert.DeserializeObject<ItemTransaction>(ddlJsonString);

        //    //GenerateXmlAndValidate(reverseParser, ddlObject);
        //    GenerateXmlAndValidate(ddlObject);
        //}

        static void GenerateXmlAndValidate(
            MinsaitParser minsaitParser,
            ItemTransaction ddlObject,
            Lib.Enums.Enums.DocumentType documentType)
        {
            System.Console.WriteLine("\n--- Mapping DDL -> Minsait XML ---");

            var xml = minsaitParser.MapToXml(ddlObject, documentType);

            // Detectar tipo de documento
            var kind = MinsaitSchemaResolver.Detect(xml);

            byte[] xsdBytes;

            switch (kind)
            {
                case MinsaitSchemaResolver.DocumentKind.Invoice:
                    xsdBytes = Properties.Resources.MinsaitInvoiveXsd;
                    break;

                case MinsaitSchemaResolver.DocumentKind.Order:
                    xsdBytes = Properties.Resources.OrderIeDocs;
                    break;

                default:
                    throw new InvalidOperationException("Unsupported document type");
            }

            // Validar
            var errors = XmlSchemaValidator.Validate(xml, xsdBytes);

            if (errors.Count == 0)
            {
                System.Console.WriteLine("\n XML is VALID against XSD");
            }
            else
            {
                System.Console.WriteLine("\n XML is INVALID:");
                foreach (var err in errors)
                    System.Console.WriteLine(" - " + err);
            }

            System.Console.WriteLine("\n--- Minsait XML Output ---");
            System.Console.WriteLine(xml);

            System.Console.WriteLine("\nPress Enter to continue...");
            System.Console.ReadLine();
        }

        //static void ProcessXmlFromFile(
        //    MinsaitParser minsaitParser,
        //    //MinsaitReverseParser reverseParser,
        //    JsonSerializerSettings serializeOptions)
        //{
        //    System.Console.Write("Enter XML file path: ");
        //    var path = System.Console.ReadLine();

        //    if (!File.Exists(path))
        //    {
        //        System.Console.WriteLine("File not found.");
        //        System.Console.ReadLine();
        //        return;
        //    }

        //    var xml = File.ReadAllText(path);

        //    var itemTransaction = minsaitParser.Parse(xml);

        //    var ddlJson = JsonConvert.SerializeObject(
        //        itemTransaction,
        //        Formatting.Indented,
        //        serializeOptions);

        //    System.Console.WriteLine("\n--- DDL JSON ---");
        //    System.Console.WriteLine(ddlJson);

        //    var ddlObject = JsonConvert.DeserializeObject<ItemTransaction>(ddlJson);

        //    //GenerateXmlAndValidate(reverseParser, ddlObject);
        //    GenerateXmlAndValidate(ddlObject);
        //}
    }
}