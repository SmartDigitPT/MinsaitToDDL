using Newtonsoft.Json;

namespace MinsaitToDDL.Lib.Models
{
    /// <summary>
    /// Represents a party.
    /// </summary>
    public class Party
    {
        [JsonProperty("CustomerID")]
        public double? CustomerID;

        [JsonProperty("PartyID")]
        public double? PartyID;

        [JsonProperty("CustomerKey")]
        public string CustomerKey;

        [JsonProperty("OrganizationName")]
        public string OrganizationName;

        [JsonProperty("AddressLine1")]
        public string AddressLine1;

        [JsonProperty("AddressLine2")]
        public string AddressLine2;

        [JsonProperty("PostalCode")]
        public string PostalCode;

        [JsonProperty("CountryID")]
        public string CountryID;

        [JsonProperty("ProvinceID")]
        public string ProvinceID;

        [JsonProperty("LocalityID")]
        public double? LocalityID;

        [JsonProperty("PersonalIdNumber")]
        public string PersonalIdNumber;

        [JsonProperty("BirthDate")]
        public System.DateTime? BirthDate;

        [JsonProperty("WebAddress")]
        public string WebAddress;

        [JsonProperty("Telephone1")]
        public string Telephone1;

        [JsonProperty("Telephone2")]
        public string Telephone2;

        [JsonProperty("Telephone3")]
        public string Telephone3;

        [JsonProperty("Telephone4")]
        public string Telephone4;

        [JsonProperty("MobileTelephone1")]
        public string MobileTelephone1;

        [JsonProperty("MobileTelephone2")]
        public string MobileTelephone2;

        [JsonProperty("Fax")]
        public string Fax;

        [JsonProperty("Email")]
        public string Email;

        [JsonProperty("CustomerPicturePath")]
        public string CustomerPicturePath;

        [JsonProperty("SalesmanId")]
        public double? SalesmanId;

        [JsonProperty("SalesmanName")]
        public string SalesmanName;

        [JsonProperty("CardID")]
        public string CardID;

        [JsonProperty("ZoneID")]
        public double? ZoneID;

        [JsonProperty("ZoneName")]
        public string ZoneName;

        [JsonProperty("CurrencyID")]
        public string CurrencyID;

        [JsonProperty("PaymentID")]
        public double? PaymentID;

        [JsonProperty("PaymentName")]
        public string PaymentName;

        [JsonProperty("CarrierID")]
        public double? CarrierID;

        [JsonProperty("CarrierName")]
        public string CarrierName;

        [JsonProperty("CashDiscountTermID")]
        public string CashDiscountTermID;

        [JsonProperty("CashDiscountTermName")]
        public object CashDiscountTermName;

        [JsonProperty("TenderID")]
        public double? TenderID;

        [JsonProperty("PriceLineID")]
        public double? PriceLineID;

        [JsonProperty("CustomerLevel")]
        public double? CustomerLevel;

        [JsonProperty("CustomerGroupDescription")]
        public string CustomerGroupDescription;

        [JsonProperty("EntityFiscalStatusID")]
        public double? EntityFiscalStatusID;

        [JsonProperty("AplyRetentionTax")]
        public bool? AplyRetentionTax;

        [JsonProperty("RetentionTax")]
        public double? RetentionTax;

        [JsonProperty("Comments")]
        public string Comments;

        [JsonProperty("Locked")]
        public bool? Locked;

        [JsonProperty("ActiveParty")]
        public bool? ActiveParty;

        [JsonProperty("LimitType")]
        public double? LimitType;

        [JsonProperty("LimitPurchaseDays")]
        public double? LimitPurchaseDays;

        [JsonProperty("LimitPurchaseValue")]
        public double? LimitPurchaseValue;

        [JsonProperty("LimitPurchaseCurrencyID")]
        public string LimitPurchaseCurrencyID;

        [JsonProperty("LimitPurchaseExchange")]
        public double? LimitPurchaseExchange;

        [JsonProperty("LimitPurchaseFactor")]
        public double? LimitPurchaseFactor;

        [JsonProperty("DirectDiscount")]
        public double? DirectDiscount;

        [JsonProperty("GlobalDiscount")]
        public double? GlobalDiscount;

        [JsonProperty("TemporaryID")]
        public string TemporaryID;

        [JsonProperty("IntranetUsername")]
        public object IntranetUsername;

        [JsonProperty("IntranetPassword")]
        public object IntranetPassword;

        [JsonProperty("IntranetGroup")]
        public object IntranetGroup;

        [JsonProperty("IntranetActive")]
        public bool? IntranetActive;

        [JsonProperty("IntranetEMailDaily")]
        public bool? IntranetEMailDaily;

        [JsonProperty("IntranetEMailWeekly")]
        public bool? IntranetEMailWeekly;

        [JsonProperty("IntranetLastVisit")]
        public System.DateTime? IntranetLastVisit;

        [JsonProperty("CreateDate")]
        public System.DateTime? CreateDate;

        [JsonProperty("LastUpdated")]
        public System.DateTime? LastUpdated;

        [JsonProperty("LastTransSerial")]
        public string LastTransSerial;

        [JsonProperty("LastTransDocument")]
        public string LastTransDocument;

        [JsonProperty("LastTransDocNumber")]
        public double? LastTransDocNumber;

        [JsonProperty("LedgerAccountBalance")]
        public double? LedgerAccountBalance;

        [JsonProperty("DefaultTransSerial")]
        public string DefaultTransSerial;

        [JsonProperty("DefaultTransDocument")]
        public string DefaultTransDocument;

        [JsonProperty("DefaultTransDocNumber")]
        public double? DefaultTransDocNumber;

        [JsonProperty("FrequentShopperPoints")]
        public double? FrequentShopperPoints;

        [JsonProperty("PartyInfo")]
        public PartyInfo PartyInfo;

        [JsonProperty("ContractList")]
        public System.Collections.Generic.List<object> ContractList;

        [JsonProperty("ImputeList")]
        public System.Collections.Generic.List<object> ImputeList;

        [JsonProperty("BankRemittanceAccount")]
        public object BankRemittanceAccount;

        [JsonProperty("RemittanceBankID")]
        public double? RemittanceBankID;

        [JsonProperty("RemittanceAccountID")]
        public double? RemittanceAccountID;

        [JsonProperty("SendXMLMailAfterSave")]
        public bool? SendXMLMailAfterSave;

        [JsonProperty("UseIntraStat")]
        public bool? UseIntraStat;

        [JsonProperty("IntraStatPortID")]
        public string IntraStatPortID;

        [JsonProperty("AccountingGeneralLedgerID")]
        public string AccountingGeneralLedgerID;

        [JsonProperty("RelationshipID")]
        public double? RelationshipID;

        [JsonProperty("CustomerDefaultTransSerial")]
        public string CustomerDefaultTransSerial;

        [JsonProperty("CustomerPrintCopies")]
        public double? CustomerPrintCopies;

        [JsonProperty("TaxablePersonType")]
        public double? TaxablePersonType;

        [JsonProperty("CashAccountingSchemeSignIn")]
        public System.DateTime? CashAccountingSchemeSignIn;

        [JsonProperty("CashAccountingSchemeSignOut")]
        public System.DateTime? CashAccountingSchemeSignOut;

        [JsonProperty("DefaultBillToCustomerID")]
        public double? DefaultBillToCustomerID;

        [JsonProperty("IssuingOutput")]
        public double? IssuingOutput;

        [JsonProperty("PlantProtectionActivityAuth")]
        public string PlantProtectionActivityAuth;

        [JsonProperty("EinvoicingEmail")]
        public string EinvoicingEmail;

        [JsonProperty("GDPRConsent")]
        public bool? GDPRConsent;

        [JsonProperty("GDPRConsentDate")]
        public System.DateTime? GDPRConsentDate;

        [JsonProperty("GDPRSystemUserGUID")]
        public string GDPRSystemUserGUID;

        [JsonProperty("GDPRForgetDate")]
        public System.DateTime? GDPRForgetDate;

        [JsonProperty("GDPRForgetUserGUID")]
        public string GDPRForgetUserGUID;

        [JsonProperty("GDPRRequestDate")]
        public System.DateTime? GDPRRequestDate;

        [JsonProperty("GDPRRequestEmail")]
        public string GDPRRequestEmail;

        [JsonProperty("GDPRRequestUserGUID")]
        public string GDPRRequestUserGUID;

        [JsonProperty("GDPRStateDate")]
        public System.DateTime? GDPRStateDate;

        [JsonProperty("GDPRStateUserGUID")]
        public string GDPRStateUserGUID;

        [JsonProperty("GDPRState")]
        public double? GDPRState;

        [JsonProperty("IsPublicAdministration")]
        public bool? IsPublicAdministration;

        [JsonProperty("PricingID")]
        public double? PricingID;

        [JsonProperty("CustomerSupplierID")]
        public string CustomerSupplierID;

        [JsonProperty("GLN")]
        public string GLN;

        [JsonProperty("Delivery")]
        public string Delivery;

        [JsonProperty("ExtendedProperties")]
        public System.Collections.Generic.List<object> ExtendedProperties;

        [JsonProperty("Properties")]
        public System.Collections.Generic.List<object> Properties;

        [JsonProperty("ItemContractList")]
        public System.Collections.Generic.List<object> ItemContractList;

        [JsonProperty("EInvoiceChannel")]
        public string EInvoiceChannel;

        [JsonProperty("WithholdingTaxType")]
        public string WithholdingTaxType;

        [JsonProperty("WhatsappConsent")]
        public bool? WhatsappConsent;

        [JsonProperty("PACustomerID")]
        public string PACustomerID;

        [JsonProperty("XCHOnboarded")]
        public bool? XCHOnboarded;

        [JsonProperty("XCHConsent")]
        public bool? XCHConsent;

        [JsonProperty("FederalTaxID")]
        public string FederalTaxID;

        [JsonProperty("FiscalStatusID")]
        public double? FiscalStatusID;

        [JsonProperty("PartyType")]
        public double? PartyType;

        [JsonProperty("Telephone")]
        public string Telephone;

        [JsonProperty("AccountBalance")]
        public double? AccountBalance;

        [JsonProperty("AccountCurrencyID")]
        public string AccountCurrencyID;

        [JsonProperty("$Type")]
        public string Type;

        [JsonProperty("CapSoc")]
        public string CapSoc;

        [JsonProperty("NRCC")]
        public string NRCC;
    }
}
