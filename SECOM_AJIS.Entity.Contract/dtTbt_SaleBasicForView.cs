//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtTbt_SaleBasicForView
    {
        #region Primitive Properties
    
        public string ContractCode
        {
            get;
            set;
        }
    
        public string OCC
        {
            get;
            set;
        }
    
        public Nullable<bool> LatestOCCFlag
        {
            get;
            set;
        }
    
        public string ChangeType
        {
            get;
            set;
        }
    
        public string SaleProcessManageStatus
        {
            get;
            set;
        }
    
        public string SalesType
        {
            get;
            set;
        }
    
        public Nullable<int> CounterNo
        {
            get;
            set;
        }
    
        public string ProductTypeCode
        {
            get;
            set;
        }
    
        public string ProductCode
        {
            get;
            set;
        }
    
        public string PurchaserCustCode
        {
            get;
            set;
        }
    
        public string PurchaserSignerTypeCode
        {
            get;
            set;
        }
    
        public string RealCustomerCustCode
        {
            get;
            set;
        }
    
        public string BranchNameEN
        {
            get;
            set;
        }
    
        public string BranchNameLC
        {
            get;
            set;
        }
    
        public string BranchAddressEN
        {
            get;
            set;
        }
    
        public string BranchAddressLC
        {
            get;
            set;
        }
    
        public string ContactPoint
        {
            get;
            set;
        }
    
        public string SiteCode
        {
            get;
            set;
        }
    
        public Nullable<bool> InstallationCompleteFlag
        {
            get;
            set;
        }
    
        public Nullable<bool> MaintenanceContractFlag
        {
            get;
            set;
        }
    
        public string SecurityTypeCode
        {
            get;
            set;
        }
    
        public string PrefixCode
        {
            get;
            set;
        }
    
        public string ServiceTypeCode
        {
            get;
            set;
        }
    
        public string ProjectCode
        {
            get;
            set;
        }
    
        public string QuotationOfficeCode
        {
            get;
            set;
        }
    
        public string ContractOfficeCode
        {
            get;
            set;
        }
    
        public string OperationOfficeCode
        {
            get;
            set;
        }
    
        public string SalesOfficeCode
        {
            get;
            set;
        }
    
        public string SalesmanEmpNo1
        {
            get;
            set;
        }
    
        public string SalesmanEmpNo2
        {
            get;
            set;
        }
    
        public string SalesmanEmpNo3
        {
            get;
            set;
        }
    
        public string SalesmanEmpNo4
        {
            get;
            set;
        }
    
        public string SalesmanEmpNo5
        {
            get;
            set;
        }
    
        public string SalesmanEmpNo6
        {
            get;
            set;
        }
    
        public string SalesmanEmpNo7
        {
            get;
            set;
        }
    
        public string SalesmanEmpNo8
        {
            get;
            set;
        }
    
        public string SalesmanEmpNo9
        {
            get;
            set;
        }
    
        public string SalesmanEmpNo10
        {
            get;
            set;
        }
    
        public string QuotationStaffEmpNo
        {
            get;
            set;
        }
    
        public string DistributedInstallTypeCode
        {
            get;
            set;
        }
    
        public string DistributedOriginCode
        {
            get;
            set;
        }
    
        public Nullable<bool> ConnectionFlag
        {
            get;
            set;
        }
    
        public string ConnectTargetCode
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> FirstContractDate
        {
            get;
            set;
        }
    
        public string FirstContractEmpNo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> LastChangeProcessDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ExpectedInstallCompleteDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ExpectedCustAcceptanceDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> InstrumentStockOutDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> SubcontractInstallCompleteDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> InstallCompleteDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> CustAcceptanceDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DeliveryDocReceiveDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DataCorrectionProcessDate
        {
            get;
            set;
        }
    
        public string DataCorrectionProcessEmpNo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> WarranteeFrom
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> WarranteeTo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> StartMaintenanceDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> EndMaintenanceDate
        {
            get;
            set;
        }
    
        public Nullable<decimal> NormalProductPrice
        {
            get;
            set;
        }
    
        public Nullable<decimal> NormalInstallFee
        {
            get;
            set;
        }
    
        public Nullable<decimal> NormalSalePrice
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderProductPrice
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderInstallFee
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderSalePrice
        {
            get;
            set;
        }
    
        public Nullable<decimal> BillingAmt_ApproveContract
        {
            get;
            set;
        }
    
        public Nullable<decimal> BillingAmt_PartialFee
        {
            get;
            set;
        }
    
        public Nullable<decimal> BillingAmt_Acceptance
        {
            get;
            set;
        }
    
        public Nullable<decimal> SaleAdjAmt
        {
            get;
            set;
        }
    
        public string QuotationTargetCode
        {
            get;
            set;
        }
    
        public string Alphabet
        {
            get;
            set;
        }
    
        public string NegotiationStaffEmpNo1
        {
            get;
            set;
        }
    
        public string NegotiationStaffEmpNo2
        {
            get;
            set;
        }
    
        public string ApproveNo1
        {
            get;
            set;
        }
    
        public string ApproveNo2
        {
            get;
            set;
        }
    
        public string ApproveNo3
        {
            get;
            set;
        }
    
        public string ApproveNo4
        {
            get;
            set;
        }
    
        public string ApproveNo5
        {
            get;
            set;
        }
    
        public string IEInchargeEmpNo
        {
            get;
            set;
        }
    
        public string InstallationSlipNo
        {
            get;
            set;
        }
    
        public string InstallationTypeCode
        {
            get;
            set;
        }
    
        public Nullable<decimal> InstallFeePaidBySECOM
        {
            get;
            set;
        }
    
        public Nullable<decimal> InstallFeeRevenueBySECOM
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ChangeNameProcessDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ContractConditionProcessDate
        {
            get;
            set;
        }
    
        public string ContractConditionProcessEmpNo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> AlmightyProgramprocessDate
        {
            get;
            set;
        }
    
        public string AlmightyProgramEmpNo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DocReceiveDate
        {
            get;
            set;
        }
    
        public string DocAuditResult
        {
            get;
            set;
        }
    
        public string DocumentCode
        {
            get;
            set;
        }
    
        public Nullable<decimal> BidGuaranteeAmount1
        {
            get;
            set;
        }
    
        public Nullable<decimal> BidGuaranteeAmount2
        {
            get;
            set;
        }
    
        public string AcquisitionTypeCode
        {
            get;
            set;
        }
    
        public string IntroducerCode
        {
            get;
            set;
        }
    
        public string MotivationTypeCode
        {
            get;
            set;
        }
    
        public string InstallFeeType
        {
            get;
            set;
        }
    
        public string ChangeNameReasonType
        {
            get;
            set;
        }
    
        public string PlanCode
        {
            get;
            set;
        }
    
        public Nullable<bool> SpecialInstallationFlag
        {
            get;
            set;
        }
    
        public string PlannerEmpNo
        {
            get;
            set;
        }
    
        public string PlanCheckerEmpNo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> PlanCheckDate
        {
            get;
            set;
        }
    
        public string PlanApproverEmpNo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> PlanApproveDate
        {
            get;
            set;
        }
    
        public Nullable<decimal> SiteBuildingArea
        {
            get;
            set;
        }
    
        public Nullable<decimal> SecurityAreaFrom
        {
            get;
            set;
        }
    
        public Nullable<decimal> SecurityAreaTo
        {
            get;
            set;
        }
    
        public string MainStructureTypeCode
        {
            get;
            set;
        }
    
        public string BuildingTypeCode
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> CreateDate
        {
            get;
            set;
        }
    
        public string CreateBy
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> UpdateDate
        {
            get;
            set;
        }
    
        public string UpdateBy
        {
            get;
            set;
        }
    
        public Nullable<decimal> NewBldMgmtCost
        {
            get;
            set;
        }
    
        public Nullable<bool> NewBldMgmtFlag
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> BidGuaranteeReturnDate1
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> BidGuaranteeReturnDate2
        {
            get;
            set;
        }
    
        public string CustCode_PurCust
        {
            get;
            set;
        }
    
        public string CustStatus_PurCust
        {
            get;
            set;
        }
    
        public Nullable<bool> ImportantFlag_PurCust
        {
            get;
            set;
        }
    
        public string PurCust_CustName
        {
            get;
            set;
        }
    
        public string PurCust_CustNameEN
        {
            get;
            set;
        }
    
        public string PurCust_CustNameLC
        {
            get;
            set;
        }
    
        public string PurCust_CustFullName
        {
            get;
            set;
        }
    
        public string PurCust_CustFullNameEN
        {
            get;
            set;
        }
    
        public string PurCust_CustFullNameLC
        {
            get;
            set;
        }
    
        public string RepPersonName_PurCust
        {
            get;
            set;
        }
    
        public string ContactPersonName_PurCust
        {
            get;
            set;
        }
    
        public string SECOMContactPerson_PurCust
        {
            get;
            set;
        }
    
        public string CustTypeCode_PurCust
        {
            get;
            set;
        }
    
        public string CompanyTypeCode_PurCust
        {
            get;
            set;
        }
    
        public string FinancialMarketTypeCode_PurCust
        {
            get;
            set;
        }
    
        public string BusinessTypeCode_PurCust
        {
            get;
            set;
        }
    
        public string PhoneNo_PurCust
        {
            get;
            set;
        }
    
        public string FaxNo_PurCust
        {
            get;
            set;
        }
    
        public string IDNo_PurCust
        {
            get;
            set;
        }
    
        public Nullable<bool> DummyIDFlag_PurCust
        {
            get;
            set;
        }
    
        public string RegionCode_PurCust
        {
            get;
            set;
        }
    
        public string URL_PurCust
        {
            get;
            set;
        }
    
        public string Memo_PurCust
        {
            get;
            set;
        }
    
        public string AddressEN_PurCust
        {
            get;
            set;
        }
    
        public string AlleyEN_PurCust
        {
            get;
            set;
        }
    
        public string RoadEN_PurCust
        {
            get;
            set;
        }
    
        public string SubDistrictEN_PurCust
        {
            get;
            set;
        }
    
        public string AddressFullEN_PurCust
        {
            get;
            set;
        }
    
        public string AddressLC_PurCust
        {
            get;
            set;
        }
    
        public string AlleyLC_PurCust
        {
            get;
            set;
        }
    
        public string RoadLC_PurCust
        {
            get;
            set;
        }
    
        public string SubDistrictLC_PurCust
        {
            get;
            set;
        }
    
        public string AddressFullLC_PurCust
        {
            get;
            set;
        }
    
        public string DistrictCode_PurCust
        {
            get;
            set;
        }
    
        public string ProvinceCode_PurCust
        {
            get;
            set;
        }
    
        public string ZipCode_PurCust
        {
            get;
            set;
        }
    
        public Nullable<bool> DeleteFlag_PurCust
        {
            get;
            set;
        }
    
        public string CustCode_RealCust
        {
            get;
            set;
        }
    
        public string CustStatus_RealCust
        {
            get;
            set;
        }
    
        public Nullable<bool> ImportantFlag_RealCust
        {
            get;
            set;
        }
    
        public string RealCust_CustName
        {
            get;
            set;
        }
    
        public string RealCust_CustNameEN
        {
            get;
            set;
        }
    
        public string RealCust_CustNameLC
        {
            get;
            set;
        }
    
        public string RealCust_CustFullName
        {
            get;
            set;
        }
    
        public string RealCust_CustFullNameEN
        {
            get;
            set;
        }
    
        public string RealCust_CustFullNameLC
        {
            get;
            set;
        }
    
        public string RepPersonName_RealCust
        {
            get;
            set;
        }
    
        public string ContactPersonName_RealCust
        {
            get;
            set;
        }
    
        public string SECOMContactPerson_RealCust
        {
            get;
            set;
        }
    
        public string CustTypeCode_RealCust
        {
            get;
            set;
        }
    
        public string CompanyTypeCode_RealCust
        {
            get;
            set;
        }
    
        public string FinancialMarketTypeCode_RealCust
        {
            get;
            set;
        }
    
        public string BusinessTypeCode_RealCust
        {
            get;
            set;
        }
    
        public string PhoneNo_RealCust
        {
            get;
            set;
        }
    
        public string FaxNo_RealCust
        {
            get;
            set;
        }
    
        public string IDNo_RealCust
        {
            get;
            set;
        }
    
        public Nullable<bool> DummyIDFlag_RealCust
        {
            get;
            set;
        }
    
        public string RegionCode_RealCust
        {
            get;
            set;
        }
    
        public string URL_RealCust
        {
            get;
            set;
        }
    
        public string Memo_RealCust
        {
            get;
            set;
        }
    
        public string AddressEN_RealCust
        {
            get;
            set;
        }
    
        public string AlleyEN_RealCust
        {
            get;
            set;
        }
    
        public string RoadEN_RealCust
        {
            get;
            set;
        }
    
        public string SubDistrictEN_RealCust
        {
            get;
            set;
        }
    
        public string AddressFullEN_RealCust
        {
            get;
            set;
        }
    
        public string AddressLC_RealCust
        {
            get;
            set;
        }
    
        public string AlleyLC_RealCust
        {
            get;
            set;
        }
    
        public string RoadLC_RealCust
        {
            get;
            set;
        }
    
        public string SubDistrictLC_RealCust
        {
            get;
            set;
        }
    
        public string AddressFullLC_RealCust
        {
            get;
            set;
        }
    
        public string DistrictCode_RealCust
        {
            get;
            set;
        }
    
        public string ProvinceCode_RealCust
        {
            get;
            set;
        }
    
        public string ZipCode_RealCust
        {
            get;
            set;
        }
    
        public Nullable<bool> DeleteFlag_RealCust
        {
            get;
            set;
        }
    
        public string SiteCode_site
        {
            get;
            set;
        }
    
        public string CustCode_site
        {
            get;
            set;
        }
    
        public string SiteNo_site
        {
            get;
            set;
        }
    
        public string site_SiteName
        {
            get;
            set;
        }
    
        public string site_SiteNameEN
        {
            get;
            set;
        }
    
        public string site_SiteNameLC
        {
            get;
            set;
        }
    
        public string SECOMContactPerson_site
        {
            get;
            set;
        }
    
        public string PersonInCharge_site
        {
            get;
            set;
        }
    
        public string PhoneNo_site
        {
            get;
            set;
        }
    
        public string BuildingUsageCode_site
        {
            get;
            set;
        }
    
        public string AddressEN_site
        {
            get;
            set;
        }
    
        public string AlleyEN_site
        {
            get;
            set;
        }
    
        public string RoadEN_site
        {
            get;
            set;
        }
    
        public string SubDistrictEN_site
        {
            get;
            set;
        }
    
        public string AddressFullEN_site
        {
            get;
            set;
        }
    
        public string AddressLC_site
        {
            get;
            set;
        }
    
        public string AlleyLC_site
        {
            get;
            set;
        }
    
        public string RoadLC_site
        {
            get;
            set;
        }
    
        public string SubDistrictLC_site
        {
            get;
            set;
        }
    
        public string AddressFullLC_site
        {
            get;
            set;
        }
    
        public string DistrictCode_site
        {
            get;
            set;
        }
    
        public string ProvinceCode_site
        {
            get;
            set;
        }
    
        public string ZipCode_site
        {
            get;
            set;
        }
    
        public string EmpNo_QuoEmp
        {
            get;
            set;
        }
    
        public string QuoEmp_EmpFirstName
        {
            get;
            set;
        }
    
        public string QuoEmp_EmpFirstNameEN
        {
            get;
            set;
        }
    
        public string QuoEmp_EmpFirstNameLC
        {
            get;
            set;
        }
    
        public string QuoEmp_EmpLastName
        {
            get;
            set;
        }
    
        public string QuoEmp_EmpLastNameEN
        {
            get;
            set;
        }
    
        public string QuoEmp_EmpLastNameLC
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> StartDate_QuoEmp
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> EndDate_QuoEmp
        {
            get;
            set;
        }
    
        public string Class_QuoEmp
        {
            get;
            set;
        }
    
        public string Gender_QuoEmp
        {
            get;
            set;
        }
    
        public string EmailAddress_QuoEmp
        {
            get;
            set;
        }
    
        public string PhoneNo_QuoEmp
        {
            get;
            set;
        }
    
        public Nullable<bool> IncidentNotificationFlag_QuoEmp
        {
            get;
            set;
        }
    
        public Nullable<bool> DeleteFlag_QuoEmp
        {
            get;
            set;
        }
    
        public string DocumentCode_DocTemp
        {
            get;
            set;
        }
    
        public string DocumentType_DocTemp
        {
            get;
            set;
        }
    
        public string DocTemp_DocumentName
        {
            get;
            set;
        }
    
        public string DocTemp_DocumentNameEN
        {
            get;
            set;
        }
    
        public string DocTemp_DocumentNameLC
        {
            get;
            set;
        }
    
        public string DocTemp_DocumentNameJP
        {
            get;
            set;
        }
    
        public string DocTemp_DocumentNoName
        {
            get;
            set;
        }
    
        public string DocTemp_DocumentNoNameEN
        {
            get;
            set;
        }
    
        public string DocTemp_DocumentNoNameLC
        {
            get;
            set;
        }
    
        public string DocTemp_DocumentNoNameJP
        {
            get;
            set;
        }
    
        public string FilePath_DocTemp
        {
            get;
            set;
        }
    
        public Nullable<int> ModuleID_DocTemp
        {
            get;
            set;
        }
    
        public string EmpNo_PlanEmp
        {
            get;
            set;
        }
    
        public string PlanEmp_EmpFirstName
        {
            get;
            set;
        }
    
        public string PlanEmp_EmpFirstNameEN
        {
            get;
            set;
        }
    
        public string PlanEmp_EmpFirstNameLC
        {
            get;
            set;
        }
    
        public string PlanEmp_EmpLastName
        {
            get;
            set;
        }
    
        public string PlanEmp_EmpLastNameEN
        {
            get;
            set;
        }
    
        public string PlanEmp_EmpLastNameLC
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> StartDate_PlanEmp
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> EndDate_PlanEmp
        {
            get;
            set;
        }
    
        public string Class_PlanEmp
        {
            get;
            set;
        }
    
        public string Gender_PlanEmp
        {
            get;
            set;
        }
    
        public string EmailAddress_PlanEmp
        {
            get;
            set;
        }
    
        public string PhoneNo_PlanEmp
        {
            get;
            set;
        }
    
        public Nullable<bool> IncidentNotificationFlag_PlanEmp
        {
            get;
            set;
        }
    
        public Nullable<bool> DeleteFlag_PlanEmp
        {
            get;
            set;
        }
    
        public string EmpNo_planAppEmp
        {
            get;
            set;
        }
    
        public string planAppEmp_EmpFirstName
        {
            get;
            set;
        }
    
        public string planAppEmp_EmpFirstNameEN
        {
            get;
            set;
        }
    
        public string planAppEmp_EmpFirstNameLC
        {
            get;
            set;
        }
    
        public string planAppEmp_EmpLastName
        {
            get;
            set;
        }
    
        public string planAppEmp_EmpLastNameEN
        {
            get;
            set;
        }
    
        public string planAppEmp_EmpLastNameLC
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> StartDate_planAppEmp
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> EndDate_planAppEmp
        {
            get;
            set;
        }
    
        public string Class_planAppEmp
        {
            get;
            set;
        }
    
        public string Gender_planAppEmp
        {
            get;
            set;
        }
    
        public string EmailAddress_planAppEmp
        {
            get;
            set;
        }
    
        public string PhoneNo_planAppEmp
        {
            get;
            set;
        }
    
        public Nullable<bool> IncidentNotificationFlag_planAppEmp
        {
            get;
            set;
        }
    
        public Nullable<bool> DeleteFlag_planAppEmp
        {
            get;
            set;
        }
    
        public string EmpNo_PlanChkrEmp
        {
            get;
            set;
        }
    
        public string PlanChkrEmp_EmpFirstName
        {
            get;
            set;
        }
    
        public string PlanChkrEmp_EmpFirstNameEN
        {
            get;
            set;
        }
    
        public string PlanChkrEmp_EmpFirstNameLC
        {
            get;
            set;
        }
    
        public string PlanChkrEmp_EmpLastName
        {
            get;
            set;
        }
    
        public string PlanChkrEmp_EmpLastNameEN
        {
            get;
            set;
        }
    
        public string PlanChkrEmp_EmpLastNameLC
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> StartDate_PlanChkrEmp
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> EndDate_PlanChkrEmp
        {
            get;
            set;
        }
    
        public string Class_PlanChkrEmp
        {
            get;
            set;
        }
    
        public string Gender_PlanChkrEmp
        {
            get;
            set;
        }
    
        public string EmailAddress_PlanChkrEmp
        {
            get;
            set;
        }
    
        public string PhoneNo_PlanChkrEmp
        {
            get;
            set;
        }
    
        public Nullable<bool> IncidentNotificationFlag_PlanChkrEmp
        {
            get;
            set;
        }
    
        public Nullable<bool> DeleteFlag_PlanChkrEmp
        {
            get;
            set;
        }
    
        public string ProductName
        {
            get;
            set;
        }
    
        public string ProductNameEN
        {
            get;
            set;
        }
    
        public string ProductNameJP
        {
            get;
            set;
        }
    
        public string ProductNameLC
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> SalesStartDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> SalesEndDate
        {
            get;
            set;
        }
    
        public Nullable<int> LifeCycle
        {
            get;
            set;
        }
    
        public string EmpNo_SalesMan1
        {
            get;
            set;
        }
    
        public string SalesMan1_EmpFirstName
        {
            get;
            set;
        }
    
        public string SalesMan1_EmpFirstNameEN
        {
            get;
            set;
        }
    
        public string SalesMan1_EmpFirstNameLC
        {
            get;
            set;
        }
    
        public string SalesMan1_EmpLastName
        {
            get;
            set;
        }
    
        public string SalesMan1_EmpLastNameEN
        {
            get;
            set;
        }
    
        public string SalesMan1_EmpLastNameLC
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> StartDate_SalesMan1
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> EndDate_SalesMan1
        {
            get;
            set;
        }
    
        public string Class_SalesMan1
        {
            get;
            set;
        }
    
        public string Gender_SalesMan1
        {
            get;
            set;
        }
    
        public string EmailAddress_SalesMan1
        {
            get;
            set;
        }
    
        public string PhoneNo_SalesMan1
        {
            get;
            set;
        }
    
        public string Quo_OfficeName
        {
            get;
            set;
        }
    
        public string Quo_OfficeNameEN
        {
            get;
            set;
        }
    
        public string Quo_OfficeNameJP
        {
            get;
            set;
        }
    
        public string Quo_OfficeNameLC
        {
            get;
            set;
        }
    
        public string OfficeCode_Con
        {
            get;
            set;
        }
    
        public string Con_OfficeName
        {
            get;
            set;
        }
    
        public string Con_OfficeNameEN
        {
            get;
            set;
        }
    
        public string Con_OfficeNameJP
        {
            get;
            set;
        }
    
        public string Con_OfficeNameLC
        {
            get;
            set;
        }
    
        public string Op_OfficeName
        {
            get;
            set;
        }
    
        public string Op_OfficeNameEN
        {
            get;
            set;
        }
    
        public string Op_OfficeNameJP
        {
            get;
            set;
        }
    
        public string Op_OfficeNameLC
        {
            get;
            set;
        }
    
        public string Sale_OfficeName
        {
            get;
            set;
        }
    
        public string Sale_OfficeNameEN
        {
            get;
            set;
        }
    
        public string Sale_OfficeNameJP
        {
            get;
            set;
        }
    
        public string Sale_OfficeNameLC
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> InstallCompleteProcessDate
        {
            get;
            set;
        }
    
        public string InstallationCompleteEmpNo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ChangeImplementDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> NewAddInstallCompleteProcessDate
        {
            get;
            set;
        }
    
        public string NewAddInstallCompleteEmpNo
        {
            get;
            set;
        }
    
        public string BICContractCode
        {
            get;
            set;
        }
    
        public Nullable<bool> SpecialCareFlag
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderProductPriceUsd
        {
            get;
            set;
        }
    
        public string OrderProductPriceCurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> NormalProductPriceUsd
        {
            get;
            set;
        }
    
        public string NormalProductPriceCurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderInstallFeeUsd
        {
            get;
            set;
        }
    
        public string OrderInstallFeeCurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> NormalInstallFeeUsd
        {
            get;
            set;
        }
    
        public string NormalInstallFeeCurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> BidGuaranteeAmount1Usd
        {
            get;
            set;
        }
    
        public string BidGuaranteeAmount1CurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> BidGuaranteeAmount2Usd
        {
            get;
            set;
        }
    
        public string BidGuaranteeAmount2CurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> NewBldMgmtCostUsd
        {
            get;
            set;
        }
    
        public string NewBldMgmtCostCurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> InstallFeePaidBySECOMUsd
        {
            get;
            set;
        }
    
        public string InstallFeePaidBySECOMCurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> InstallFeeRevenueBySECOMUsd
        {
            get;
            set;
        }
    
        public string InstallFeeRevenueBySECOMCurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderSalePriceUsd
        {
            get;
            set;
        }
    
        public string OrderSalePriceCurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> NormalSalePriceUsd
        {
            get;
            set;
        }
    
        public string NormalSalePriceCurrencyType
        {
            get;
            set;
        }
    
        public string QuotationNo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> PaymentDateIncentive
        {
            get;
            set;
        }

        #endregion

    }
}
