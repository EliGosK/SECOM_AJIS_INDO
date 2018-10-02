using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of screen
    /// </summary>
    public class CTS350_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string strARRelevantType { get; set; }

        [KeepSession]
        public string strARRelevantCode { get; set; }

        public bool blnIncidentIsHeadOfficeFlag { get; set; } //Add by jutarat A. on 20082012
        public string MailTo { get; set; } //Add by jutarat A. on 07092012
    }

    //public class CTS350_ParameterData
    //{
    //    public string strARRelevantType { get; set; }
    //    public string strARRelevantCode { get; set; }
    //}

    /// <summary>
    /// Person in charge data to display on screen
    /// </summary>
    public class CTS350_PersonInCharge
    {
        public string OfficeCode { get; set; }
        public string OfficeName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string ARRoleCode { get; set; }
        public string ARRoleName { get; set; }
        public string EmpNo { get; set; }
        public string EmployeeName { get; set; }
        public bool SendMail { get; set; }
        public bool CanRemove { get; set; }
        public bool CanEditSendMail { get; set; }
    }

    /// <summary>
    /// Condition object to retrieve customer, site, quotation, project, or contract data
    /// </summary>
    public class CTS350_RetrieveCondition
    {
        public string ARRelevantType { get; set; }
        public string CustomerCode { get; set; }
        public string SiteCode { get; set; }
        public string QuotationCode { get; set; }
        public string ProjectCode { get; set; }
        public string ContractCode { get; set; }
    }

    /// <summary>
    /// AR purpose data to display on screen
    /// </summary>
    public class CTS350_ARPurposeResult
    {
        public string ARPurpose { get; set; }
        public bool EnableGrid1 { get; set; }
        public bool EnableGrid2 { get; set; }
    }

    /// <summary>
    /// Contract data item selected from screen
    /// </summary>
    public class CTS350_TargetContract
    {
        public string ContractCode { get; set; }
        public string SiteCode { get; set; }
        public string UserCode { get; set; }
        public string ContractType { get; set; }
        public string ServiceType { get; set; }
    }

    /// <summary>
    /// All AR data keep in memory
    /// </summary>
    public class CTS350_ARData
    {
        public CTS350_ARData_Customer CustomerData { get; set; }
        public CTS350_ARData_Site SiteData { get; set; }
        public CTS350_ARData_Quotation QuotationData { get; set; }
        public CTS350_ARData_Project ProjectData { get; set; }
        public CTS350_ARData_Contract ContractData { get; set; }
        public CTS350_PersonInCharge PersonInCharge { get; set; }
    }

    /// <summary>
    /// Customer data
    /// </summary>
    public class CTS350_ARData_Customer
    {
        public string CustCode { get; set; }
        public string CustFullNameEN { get; set; }
        public string AddressFullEN { get; set; }
        public string CustFullNameLC { get; set; }
        public string AddressFullLC { get; set; }
        public string Nationality { get; set; }
        public string PhoneNo { get; set; }
        public string IDNo { get; set; }
        public string BusinessTypeName { get; set; }
        public string URL { get; set; }
    }

    /// <summary>
    /// Site data
    /// </summary>
    public class CTS350_ARData_Site
    {
        public string SiteCode { get; set; }
        public string SiteNameEN { get; set; }
        public string AddressFullEN { get; set; }
        public string SiteNameLC { get; set; }
        public string AddressFullLC { get; set; }
        public string PhoneNo { get; set; }
        public string Usage { get; set; }
    }

    /// <summary>
    /// Quotation data
    /// </summary>
    public class CTS350_ARData_Quotation
    {
        public string QuotationCode { get; set; }
        public string ContractCode { get; set; }
        public string ContractTargetEN { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteAddressEN { get; set; }
        public string ContractTargetLC { get; set; }
        public string SiteNameLC { get; set; }
        public string SiteAddressLC { get; set; }
        public string OperationOffice { get; set; }
        public string QuotationOffice { get; set; }
    }

    /// <summary>
    /// Project data
    /// </summary>
    public class CTS350_ARData_Project
    {
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectAddress { get; set; }
        public string ProjectPurchaseNameEN { get; set; }
        public string ProjectPurchaseNameLC { get; set; }
    }

    /// <summary>
    /// Contract data
    /// </summary>
    public class CTS350_ARData_Contract
    {
        public string ContractName { get; set; }
        public string ContractCode { get; set; }
        public string ShortContractCode { get; set; }
        public string SiteName { get; set; }
        public string SiteCode { get; set; }
        public string UserCode { get; set; }
        public string ServiceType { get; set; }
    }

    /// <summary>
    /// Register AR object data from screen
    /// </summary>
    public class CTS350_ARRegisForm
    {
        public string ARRelevantType { get; set; }

        [NotNullOrEmpty(ControlName = "ARType", Screen = "CTS350", Controller = "Contract", Parameter = "lblARType")]
        public string ARTypeCode { get; set; }
        [NotNullOrEmpty(ControlName = "ARTitle", Screen = "CTS350", Controller = "Contract", Parameter = "lblARTitle")]
        public string ARTitle { get; set; }
        [NotNullOrEmpty(ControlName = "ARSubTitle", Screen = "CTS350", Controller = "Contract", Parameter = "lblARSubTitle")]
        public string ARSubTitle { get; set; }
        [NotNullOrEmpty(ControlName = "ARPurpose", Screen = "CTS350", Controller = "Contract", Parameter = "lblARPurpose")]
        public string ARPurpose { get; set; }
        public bool IsImportance { get; set; }

        //Comment by Jutarat A. on 03092012
        //public string DueDate_Deadline_Type { get; set; }
        //public DateTime? DueDate_Date { get; set; }
        //public TimeSpan? DueDate_Time { get; set; }
        //public DateTime? Deadline_Date { get; set; }
        //public string Deadline_Until { get; set; }
        //End Comment

        public List<CTS350_ARData_Contract> ContractData { get; set; }
        public List<CTS350_PersonInCharge> PersonInChargeData { get; set; }
        public CTS350_RetrieveCondition Condition { get; set; }

        public string CustomerRelateType { get; set; }
        public string SiteRelateType { get; set; }

        public string ContractFee_QuotationCurrencyType { get; set; }
        public decimal ContractFee_Quotation { get; set; }
        public string ContractFee_ARCurrencyType { get; set; }
        public decimal ContractFee_AR { get; set; }
        public string Deposit_QuotationCurrencyType { get; set; }
        public decimal Deposit_Quotation { get; set; }
        public string Deposit_ARCurrencyType { get; set; }
        public decimal Deposit_AR { get; set; }
        public string Installation_QuotationCurrencyType { get; set; }
        public decimal Installation_Quotation { get; set; }
        public string Installation_ARCurrencyType { get; set; }
        public decimal Installation_AR { get; set; }

        public string ProductPrice_QuotationCurrencyType { get; set; }
        public decimal ProductPrice_Quotation { get; set; }
        public string ProductPrice_ARCurrencyType { get; set; }
        public decimal ProductPrice_AR { get; set; }
        public string InstallFee_QuotationCurrencyType { get; set; }
        public decimal InstallFee_Quotation { get; set; }
        public string InstallFee_ARCurrencyType { get; set; }
        public decimal InstallFee_AR { get; set; }
    }

    /// <summary>
    /// Register new AR result
    /// </summary>
    public class CTS350_RegisResult
    {
        //public int? ARID { get; set; }
        public string RegisStatus { get; set; }
        public string RequestNo { get; set; }
    }
}
