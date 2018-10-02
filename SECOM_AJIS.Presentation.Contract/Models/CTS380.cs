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
    public class CTS380_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string pRequestNo { get; set; }
        public Dictionary<int, string> newAttachLst { get; set; }
        public Dictionary<int, string> delAttachLst { get; set; }

        public bool blnIncidentIsHeadOfficeFlag { get; set; } //Add by jutarat A. on 20082012
        public CTS380_ScreenData ScreenData { get; set; } //Add by Jutarat A. on 24082012
        public string MailTo { get; set; } //Add by jutarat A. on 07092012
    }

    /// <summary>
    /// Person in charge grid item for display
    /// </summary>
    public class CTS380_PICDat
    {
        public string EmpNo { get; set; }
        public string EmployeeName { get; set; }
        public string OfficeCode { get; set; }
        public string OfficeName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string ARRoleCode { get; set; }
        public string ARRoleName { get; set; }
        public bool CanDelete { get; set; }
        public bool SendMail { get; set; }
        public bool CanEditSendMail { get; set; }
    }

    /// <summary>
    /// AR data container memory
    /// </summary>
    public class CTS380_ScreenData
    {
        public CTS380_ARInfo ARInfo { get; set; }
        public CTS380_SaleContract SaleContract { get; set; }
        public CTS380_RentalContract RentalContract { get; set; }
        public CTS380_ARDetail ARDetail { get; set; }
        public List<CTS380_PICDat> ARRole { get; set; }

        public bool CanEdit { get; set; }
        public bool CanSpecialViewEdit { get; set; }
        public bool CanModPIC { get; set; } //Add by Jutarat A. on 24082012

        //Add by Jutarat A. on 05102012
        public bool IsSpecialView { get; set; }
        public bool IsEditable { get; set; }
        public doARPermission ARPermissionData { get; set; }
        //End Add

        //Add by Jutarat A. on 26042013
        public bool IsEnableAlarm { get; set; }
        public bool IsEnableSale { get; set; }
        //End Add
    }

    /// <summary>
    /// AR status for display on screen
    /// </summary>
    public class CTS380_ARStatusReturn
    {
        public string ARStatus { get; set; }
        public bool IsEnable { get; set; }
    }

    /// <summary>
    /// AR detail data for display
    /// </summary>
    public class CTS380_ARDetail
    {
        public string AuditDetailHistory { get; set; }
        public string AuditDetailHistoryWithUpdate { get; set; }

        public string ARType { get; set; }
        public string ARTitle { get; set; }
        public string ARSubTitle { get; set; }
        public bool ImportantFlag { get; set; }
        public string ARPurpose { get; set; }
        public string DueDateDeadlineType { get; set; }
        public string DueDate_Date { get; set; }
        public string DueDate_Time { get; set; }
        public string Deadline_Date { get; set; }
        public string Deadline_Until { get; set; }

        public string ContractQuotationFeeCurrencyType { get; set; }
        public decimal ContractQuotationFee { get; set; }
        public string ContractARFeeCurrencyType { get; set; }
        public decimal ContractARFee { get; set; }
        public string DepositQuotationFeeCurrencyType { get; set; }
        public decimal DepositQuotationFee { get; set; }
        public string DepositARFeeCurrencyType { get; set; }
        public decimal DepositARFee { get; set; }
        public string InstallQuotationFeeCurrencyType { get; set; }
        public decimal InstallQuotationFee { get; set; }
        public string InstallARFeeCurrencyType { get; set; }
        public decimal InstallARFee { get; set; }

        public string ProductPriceQuotationFeeCurrencyType { get; set; }
        public decimal ProductPriceQuotationFee { get; set; }
        public string ProductPriceARFeeCurrencyType { get; set; }
        public decimal ProductPriceARFee { get; set; }
        public string InstallFeeQuotationFeeCurrencyType { get; set; }
        public decimal InstallFeeQuotationFee { get; set; }
        public string InstallFeeARFeeCurrencyType { get; set; }
        public decimal InstallFeeARFee { get; set; }

        public string ARStatus { get; set; } //Add by Jutarat A. on 23082012
    }

    /// <summary>
    /// Sale contract data for display
    /// </summary>
    public class CTS380_SaleContract
    {
        public string ContractCode { get; set; }
        public string ProcessMgmtStatus { get; set; }
        public string ProductName { get; set; }
        public string ExpectedCompleteInstallDate { get; set; }
        public string MaintainContractCode { get; set; }
        public string CompleteInstallDate { get; set; }
        public string SaleType { get; set; }
        public string ExpectedCustomerAcceptedDate { get; set; }
        public string OperationOffice { get; set; }
        public string CustomerAcceptedDate { get; set; }
        public string LastChangeType { get; set; }
    }

    /// <summary>
    /// Rental contract data for display
    /// </summary>
    public class CTS380_RentalContract
    {
        public string ContractCode { get; set; }
        public string StartDealDate { get; set; }
        public string MonthlyContractFee { get; set; }
        public string ContractDurationFrom { get; set; }
        public string ContractDurationTo { get; set; }
        public string FirstOperationDate { get; set; }
        public string ContractDurationMonth { get; set; }
        public string LastChangeType { get; set; }
        public string AutoRenew { get; set; }
        public string LastOperationDate { get; set; }
        public string OldContractCode { get; set; }
        public string SecurityType { get; set; }
        public string OperationOffice { get; set; }
        public string ProductName { get; set; }
        public string MonthlyContractFeeCurrencyType { get; set; }
        public string MonthlyContractFeeUsd { get; set; }

    }

    /// <summary>
    /// AR data for display
    /// </summary>
    public class CTS380_ARInfo
    {
        public string ARRelevantType { get; set; }
        public string RequestNo { get; set; }
        public string ApproveNo { get; set; }
        public string CustomerCode { get; set; }
        public string Status { get; set; }
        public string CustomerNameEN { get; set; }
        public string CustomerNameLC { get; set; }
        public string SiteCode { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteNameLC { get; set; }
        public string SiteAddressEN { get; set; }
        public string ContractCode { get; set; }
        public string QuotationCode { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string UserCode { get; set; }
        public string Requester { get; set; }
    }

    /// <summary>
    /// AR data for edit 
    /// </summary>
    public class CTS380_EntryData
    {
        [NotNullOrEmpty(ControlName = "InteractionType", Controller = "Contract", Screen = "CTS380", Parameter = "lblInteractionType")]
        public string InteractionType { get; set; }
        [NotNullOrEmpty(ControlName = "IncidentStatusAfterUpdate", Controller = "Contract", Screen = "CTS380", Parameter = "lblStatusAfterUpdate")]
        public string StatusAfterUpdate { get; set; }
        public string AuditDetail { get; set; }

        //Comment by Jutarat A. on 03092012
        //public string DueDate_DeadlineType { get; set; }
        //public DateTime? DueDate_Date { get; set; }
        //public TimeSpan? DueDate_Time { get; set; }
        //public DateTime? Deadline_Date { get; set; }
        //public string Deadline_Until { get; set; }
        //End Comment

        public List<CTS380_PICDat> ARRole { get; set; }
        public List<CTS380_HistoryLog> HistoryList { get; set; }
        public List<CTS380_HistoryLog> OriginList { get; set; }

        //Add by Jutarat A. on 03042013
        public decimal ContractFee_Quotation { get; set; }
        public decimal ContractFee_AR { get; set; }
        public decimal Deposit_Quotation { get; set; }
        public decimal Deposit_AR { get; set; }
        public decimal Installation_Quotation { get; set; }
        public decimal Installation_AR { get; set; }

        public decimal ProductPrice_Quotation { get; set; }
        public decimal ProductPrice_AR { get; set; }
        public decimal InstallFee_Quotation { get; set; }
        public decimal InstallFee_AR { get; set; }
        //End Add
    }

    /// <summary>
    /// History data
    /// </summary>
    public class CTS380_HistoryLog
    {
        public string EmpNo { get; set; }
        public string Action { get; set; }
        public string FunctionType
        {
            get
            {
                if (Action == "1")
                    return Common.Util.ConstantValue.FunctionID.C_FUNC_ID_ADD;
                else if (Action == "2")
                    return Common.Util.ConstantValue.FunctionID.C_FUNC_ID_EDIT;
                else if (Action == "3")
                    return Common.Util.ConstantValue.FunctionID.C_FUNC_ID_DEL;
                else
                    return String.Empty;
            }
        }
    }
}
