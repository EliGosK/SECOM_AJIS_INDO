using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Default value container
    /// </summary>
    public class CTS150_DefaultValue
    {
        public string C_ACQUISITION_TYPE_CUST { get; set; }
        public string C_ACQUISITION_TYPE_INSIDE_COMPANY { get; set; }
        public string C_ACQUISITION_TYPE_SUBCONTRACTOR { get; set; }

        public string C_DISTRIBUTED_TYPE_NOT_DISTRIBUTED { get; set; }
        public string C_DISTRIBUTED_TYPE_ORIGIN { get; set; }

        public string ContractCode { get; set; }
    }

    /// <summary>
    /// Condition object contain key for retrieve data
    /// </summary>
    public class CTS150_Condition
    {
        public string strContractCode { get; set; }
        public string strOccurrenceCode { get; set; }
    }

    /// <summary>
    /// Contract data object for display on screen
    /// </summary>
    public class CTS150_ContractDetailResult
    {
        public bool HasMA { get; set; }
        public int ViewMode { get; set; }

        public string PurchaseCode { get; set; }
        public string EndUserCode { get; set; }
        public string SiteCode { get; set; }
        public bool IsImportant { get; set; }
        public string PurchaseNameEN { get; set; }
        public string PurchaseNameLC { get; set; }
        public string PurchaseAddressEN { get; set; }
        public string PurchaseAddressLC { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteNameLC { get; set; }
        public string SiteAddressEN { get; set; }
        public string SiteAddressLC { get; set; }
        public string ChangeType { get; set; }

        public string ProductName { get; set; }
        public string ApproveContractDate { get; set; }
        public string ContractOffice { get; set; }
        public string SaleType { get; set; }
        public string OperationOffice { get; set; }
        public string PlanCode { get; set; }
        public string SalesOffice { get; set; }
        public string QuotationCode { get; set; }

        public string QuotationNo { get; set; }

        public string ProjectCode { get; set; }
        public string DocAuditResult { get; set; }
        public string DistributedType { get; set; }
        public string DocRecieveDate { get; set; }
        public string DistributedOriginCode { get; set; }
        public string OnlineContractCode { get; set; }

        public string ProductBillingAmountCurrencyType { get; set; }
        public string ProductBillingAmount { get; set; }

        public string ProductNormalAmountCurrencyType { get; set; }
        public string ProductNormalAmount { get; set; }

        public string InstallBillingAmountCurrencyType { get; set; }
        public string InstallBillingAmount { get; set; }

        public string InstallNormalAmountCurrencyType { get; set; }
        public string InstallNormalAmount { get; set; }

        public string BidGuaranteeAmount1CurrencyType { get; set; }
        public string BidGuaranteeAmount1 { get; set; }

        public string BidGuaranteeReturnDate1 { get; set; }

        public string BidGuaranteeAmount2CurrencyType { get; set; }
        public string BidGuaranteeAmount2 { get; set; }

        public string BidGuaranteeReturnDate2 { get; set; }
        public string ContractApproveNo1 { get; set; }
        public string ContractApproveNo2 { get; set; }
        public string ContractApproveNo3 { get; set; }
        public string ContractApproveNo4 { get; set; }
        public string ContractApproveNo5 { get; set; }
        public string PurchaseReasonType { get; set; }
        public string AcquisitionType { get; set; }
        public string IntroducerCode { get; set; }
        public string RawQuotationCode { get; set; }
        public string RawAlphabet { get; set; }

        public string SalesmanCode1 { get; set; }
        public string SalesmanCode2 { get; set; }
        public string SalesmanCode3 { get; set; }
        public string SalesmanCode4 { get; set; }
        public string SalesmanCode5 { get; set; }
        public string SalesmanCode6 { get; set; }
        public string SalesmanCode7 { get; set; }
        public string SalesmanCode8 { get; set; }
        public string SalesmanCode9 { get; set; }
        public string SalesmanCode10 { get; set; }

        public string SalesmanName1 { get; set; }
        public string SalesmanName2 { get; set; }
        public string SalesmanName3 { get; set; }
        public string SalesmanName4 { get; set; }
        public string SalesmanName5 { get; set; }
        public string SalesmanName6 { get; set; }
        public string SalesmanName7 { get; set; }
        public string SalesmanName8 { get; set; }
        public string SalesmanName9 { get; set; }
        public string SalesmanName10 { get; set; }

        public string ProcessManagementStatus { get; set; }
        public string InstallSlipNo { get; set; }
        public string ExpectedCompleteInstallDate { get; set; }
        public string NewOldBuilding { get; set; }
        public string ExpectedCustomerAccept { get; set; }
        public bool NewBuildMgmtType { get; set; }
        public string InstrumentStockOutDate { get; set; }
        public string NewBuildingMgmtCost { get; set; }
        public string SubcontractCompleteInstallDate { get; set; }
        public string CompleteInstallDate { get; set; }
        public string CustomerAcceptDate { get; set; }
        public string DeliveryDocRecieveDate { get; set; }
        public string IEInChargeCode { get; set; }
        public string IEInChargeName { get; set; }

        public string MaintenanceContractCode { get; set; }
        public string MaintenanceTargetProduct { get; set; }
        public string MaintenanceType { get; set; }
        public string MaintenanceCycle { get; set; }
        public string MaintenanceStartYear { get; set; }
        public string MaintenanceStartMonth { get; set; }
        public string MaintenanceFeeType { get; set; }
        public string MaintenanceMemo { get; set; }
        public string ContractDuration { get; set; }
        public string EndContractDate { get; set; }
        public string AutoRenew { get; set; }
        public string StartMaintenanceDate { get; set; }
        public string EndMaintenanceDate { get; set; }

        public string InstallationType { get; set; }

        public string NormalInstallFeeCurrencyType { get; set; }
        public string NormalInstallFee { get; set; }
        public string OrderInstallFeeCurrencyType { get; set; }
        public string OrderInstallFee { get; set; }

        public string SecomPaymentCurrencyType { get; set; }
        public string SecomPayment { get; set; }
        public string SecomRevenueCurrencyType { get; set; }
        public string SecomRevenue { get; set; }

        public string ChangeApproveNo1 { get; set; }
        public string NegotiationStaffCode { get; set; }
        public string NegotiationStaffName { get; set; }
        public string ChangeApproveNo2 { get; set; }
        public string CompleteRegistrantCode { get; set; }
        public string CompleteRegistrantName { get; set; }

        public string WaranteePeriodFrom { get; set; }
        public string WaranteePeriodTo { get; set; }

        public string ChangeTypeCode { get; set; }

        public string PaymentDateIncentive { get; set; }
    }

    /// <summary>
    /// Subcontractor data object for display on grid
    /// </summary>
    public class CTS150_SubContractorResult
    {
        public string SubcontractorCode { get; set; }
        public string SubContractorNameEN { get; set; }
        public string SubContractorNameLC { get; set; }
    }

    /// <summary>
    /// Instrument data object for display on grid
    /// </summary>
    public class CTS150_InstrumentResult
    {
        public string InstrumentName { get; set; }
        public string InstrumentCode { get; set; }
        public string MakerName { get; set; }
        public string QtyAdded { get; set; }
        public string QtyRemoved { get; set; }
        public string QtyTotal { get; set; }
    }

    /// <summary>
    /// Instrument code object for search, retrieve, and add instrument
    /// </summary>
    public class CTS150_SearchByCode
    {
        public string Code { get; set; }
    }

    /// <summary>
    /// Search result object for display
    /// </summary>
    public class CTS150_SearchResultByCode
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Maker { get; set; }
    }

    /// <summary>
    /// Parameter of screen
    /// </summary>
    public class CTS150_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public string strContractCode { get; set; }
    }

    /// <summary>
    /// Object from screen for register maintain contract
    /// </summary>
    public class CTS150_CQ31Change
    {
        public String ContractCode { get; set; }
        public String OCCCode { get; set; }

        public String ContractOfficeCode { get; set; }
        public String OperationOfficeCode { get; set; }
        public String SalesOfficeCode { get; set; }
        public String SaleType { get; set; }
        public String DocAuditResult { get; set; }
        public DateTime? DocRecieveDate { get; set; }
        public String DistributedType { get; set; }
        public String DistributedOriginCode { get; set; }
        public String OnlineContractCode { get; set; }

        public string BillingProductPriceCurrencyType { get; set; }
        public Decimal? BillingProductPrice { get; set; }

        public string NormalProductPriceCurrencyType { get; set; }
        public Decimal? NormalProductPrice { get; set; }

        public string BillingInstallFeeCurrencyType { get; set; }
        public Decimal? BillingInstallFee { get; set; }

        public string NormalInstallFeeCurrencyType { get; set; }
        public Decimal? NormalInstallFee { get; set; }

        public string BidGuaranteeAmount1CurrencyType { get; set; }
        public Decimal? BidGuaranteeAmount1 { get; set; }

        public string BidGuaranteeAmount2CurrencyType { get; set; }
        public Decimal? BidGuaranteeAmount2 { get; set; }

        public DateTime? BidGuaranteeReturnDate1 { get; set; }
        public DateTime? BidGuaranteeReturnDate2 { get; set; }
        public String PurchaseReasonType { get; set; }
        public String AcquisitionType { get; set; }
        public String IntroducerCode { get; set; }
        public String ContractApproveNo1 { get; set; }
        public String ContractApproveNo2 { get; set; }
        public String ContractApproveNo3 { get; set; }
        public String ContractApproveNo4 { get; set; }
        public String ContractApproveNo5 { get; set; }
        public String[] SalesmanCode { get; set; }

        public String ProcessMgmtStatus { get; set; }
        public DateTime? ExpectedCompleteInstallDate { get; set; }
        public DateTime? ExpectedCustomerAcceptDate { get; set; }
        public DateTime? InstrumentStockOutDate { get; set; }
        public DateTime? SubcontractCompleteInstallDate { get; set; }
        public DateTime? CompleteInstallDate { get; set; }
        public DateTime? CustomerAcceptDate { get; set; }
        public DateTime? DeliveryDocRecieveDate { get; set; }
        public String NewOldBuilding { get; set; }
        public String NewBuildingMgmtType { get; set; }
        public Decimal? NewBuildingMgmtCost { get; set; }
        public String IEInCargeCode { get; set; }

        public string ChangeNormalInstallFeeCurrencyType { get; set; }
        public Decimal? ChangeNormalInstallFee { get; set; }

        public string OrderInstallFeeCurrencyType { get; set; }
        public Decimal? OrderInstallFee { get; set; }

        public DateTime? ChangeCompleteInstallDate { get; set; }
        public String ChangeApproveNo1 { get; set; }
        public String ChangeApproveNo2 { get; set; }
        public String NegotiationStaffCode { get; set; }
        public String CompleteRegistrantCode { get; set; }

        public DateTime? WaranteePeriodFrom { get; set; }
        public DateTime? WaranteePeriodTo { get; set; }
        public CTS150_InstrumentDetail[] InstrumentDetail { get; set; }
        public string ChangeType { get; set; }

        public string SecomPaymentCurrencyType { get; set; }
        public decimal? SecomPayment { get; set; }
        public string SecomRevenueCurrencyType { get; set; }
        public decimal? SecomRevenue { get; set; }

        public DateTime? PaymentDateIncentive { get; set; }

        public string QuotationNo { get; set; }
    }

    /// <summary>
    /// Instrument object for register maintain contract
    /// </summary>
    public class CTS150_InstrumentDetail
    {
        public String Code { get; set; }
        public int Total { get; set; }
    }

    /// <summary>
    /// Result object from validate business
    /// </summary>
    public class CTS150_ValidateResult
    {
        public bool IsValid { get; set; }
        public String InvalidMessage { get; set; }
        public String[] InvalidControl { get; set; }
    }
}
