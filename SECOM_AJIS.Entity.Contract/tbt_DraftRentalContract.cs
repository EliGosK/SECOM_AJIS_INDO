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
    public partial class tbt_DraftRentalContract
    {
        #region Primitive Properties
    
        public virtual string QuotationTargetCode
        {
            get;
            set;
        }
    
        public virtual string Alphabet
        {
            get;
            set;
        }
    
        public virtual string ProductTypeCode
        {
            get;
            set;
        }
    
        public virtual string ContractTargetCustCode
        {
            get;
            set;
        }
    
        public virtual string ContractTargetSignerTypeCode
        {
            get;
            set;
        }
    
        public virtual string RealCustomerCustCode
        {
            get;
            set;
        }
    
        public virtual string BranchNameEN
        {
            get;
            set;
        }
    
        public virtual string BranchNameLC
        {
            get;
            set;
        }
    
        public virtual string BranchAddressEN
        {
            get;
            set;
        }
    
        public virtual string BranchAddressLC
        {
            get;
            set;
        }
    
        public virtual string ContactPoint
        {
            get;
            set;
        }
    
        public virtual string SiteCode
        {
            get;
            set;
        }
    
        public virtual string DraftRentalContractStatus
        {
            get;
            set;
        }
    
        public virtual string ContractCode
        {
            get;
            set;
        }
    
        public virtual string SecurityTypeCode
        {
            get;
            set;
        }
    
        public virtual string ProductCode
        {
            get;
            set;
        }
    
        public virtual string ProjectCode
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> NormalContractFee
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> NormalInstallFee
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> NormalDepositFee
        {
            get;
            set;
        }
    
        public virtual string QuotationOfficeCode
        {
            get;
            set;
        }
    
        public virtual string OperationOfficeCode
        {
            get;
            set;
        }
    
        public virtual string ContractOfficeCode
        {
            get;
            set;
        }
    
        public virtual string MainStructureTypeCode
        {
            get;
            set;
        }
    
        public virtual Nullable<int> NumOfBuilding
        {
            get;
            set;
        }
    
        public virtual Nullable<int> NumOfFloor
        {
            get;
            set;
        }
    
        public virtual Nullable<int> TotalFloorArea
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> SiteBuildingArea
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> SecurityAreaFrom
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> SecurityAreaTo
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> InsuranceCoverageAmount
        {
            get;
            set;
        }
    
        public virtual string InsuranceTypeCode
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> MonthlyInsuranceFee
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> ExpectedStartServiceDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> ExpectedInstallCompleteDate
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderContractFee
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderInstallFee
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderDepositFee
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderInstallFee_ApproveContract
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderInstallFee_CompleteInstall
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderInstallFee_StartService
        {
            get;
            set;
        }
    
        public virtual string BillingTimingDepositFee
        {
            get;
            set;
        }
    
        public virtual string OldContractCode
        {
            get;
            set;
        }
    
        public virtual string CounterBalanceOriginContractCode
        {
            get;
            set;
        }
    
        public virtual string IrregulationContractDurationFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<int> ContractDurationMonth
        {
            get;
            set;
        }
    
        public virtual Nullable<int> AutoRenewMonth
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> ContractEndDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CalContractEndDate
        {
            get;
            set;
        }
    
        public virtual Nullable<int> BillingCycle
        {
            get;
            set;
        }
    
        public virtual string PayMethod
        {
            get;
            set;
        }
    
        public virtual Nullable<int> CreditTerm
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> DivideContractFeeBillingFlag
        {
            get;
            set;
        }
    
        public virtual string CalDailyFeeStatus
        {
            get;
            set;
        }
    
        public virtual string SalesmanEmpNo1
        {
            get;
            set;
        }
    
        public virtual string SalesmanEmpNo2
        {
            get;
            set;
        }
    
        public virtual string SalesSupporterEmpNo
        {
            get;
            set;
        }
    
        public virtual string ApproveNo1
        {
            get;
            set;
        }
    
        public virtual string ApproveNo2
        {
            get;
            set;
        }
    
        public virtual string ApproveNo3
        {
            get;
            set;
        }
    
        public virtual string ApproveNo4
        {
            get;
            set;
        }
    
        public virtual string ApproveNo5
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> FireMonitorFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> CrimePreventFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> EmergencyReportFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> FacilityMonitorFlag
        {
            get;
            set;
        }
    
        public virtual string PhoneLineTypeCode1
        {
            get;
            set;
        }
    
        public virtual string PhoneLineOwnerTypeCode1
        {
            get;
            set;
        }
    
        public virtual string PhoneLineTypeCode2
        {
            get;
            set;
        }
    
        public virtual string PhoneLineOwnerTypeCode2
        {
            get;
            set;
        }
    
        public virtual string PhoneLineTypeCode3
        {
            get;
            set;
        }
    
        public virtual string PhoneLineOwnerTypeCode3
        {
            get;
            set;
        }
    
        public virtual Nullable<int> FacilityPassYear
        {
            get;
            set;
        }
    
        public virtual Nullable<int> FacilityPassMonth
        {
            get;
            set;
        }
    
        public virtual string BICContractCode
        {
            get;
            set;
        }
    
        public virtual string DispatchTypeCode
        {
            get;
            set;
        }
    
        public virtual string AcquisitionTypeCode
        {
            get;
            set;
        }
    
        public virtual string IntroducerCode
        {
            get;
            set;
        }
    
        public virtual string MotivationTypeCode
        {
            get;
            set;
        }
    
        public virtual string BuildingTypeCode
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> NewBldMgmtFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> NewBldMgmtCost
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> AdditionalFee1
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> AdditionalFee2
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> AdditionalFee3
        {
            get;
            set;
        }
    
        public virtual string AdditionalApproveNo1
        {
            get;
            set;
        }
    
        public virtual string AdditionalApproveNo2
        {
            get;
            set;
        }
    
        public virtual string AdditionalApproveNo3
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> MaintenanceFee1
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> MaintenanceFee2
        {
            get;
            set;
        }
    
        public virtual Nullable<int> MaintenanceCycle
        {
            get;
            set;
        }
    
        public virtual string Memo
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> ApproveContractDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> RegisterDate
        {
            get;
            set;
        }
    
        public virtual string PlanCode
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> SpecialInstallationFlag
        {
            get;
            set;
        }
    
        public virtual string PlannerEmpNo
        {
            get;
            set;
        }
    
        public virtual string PlanCheckerEmpNo
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> PlanCheckDate
        {
            get;
            set;
        }
    
        public virtual string PlanApproverEmpNo
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> PlanApproveDate
        {
            get;
            set;
        }
    
        public virtual string FacilityMemo
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CreateDate
        {
            get;
            set;
        }
    
        public virtual string CreateBy
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> UpdateDate
        {
            get;
            set;
        }
    
        public virtual string UpdateBy
        {
            get;
            set;
        }
    
        public virtual string QuotationStaffEmpNo
        {
            get;
            set;
        }
    
        public virtual string ContractTargetMemo
        {
            get;
            set;
        }
    
        public virtual string RealCustomerMemo
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> NormalContractFeeUsd
        {
            get;
            set;
        }
    
        public virtual string NormalContractFeeCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> NormalInstallFeeUsd
        {
            get;
            set;
        }
    
        public virtual string NormalInstallFeeCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> NormalDepositFeeUsd
        {
            get;
            set;
        }
    
        public virtual string NormalDepositFeeCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> InsuranceCoverageAmountUsd
        {
            get;
            set;
        }
    
        public virtual string InsuranceCoverageAmountCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> MonthlyInsuranceFeeUsd
        {
            get;
            set;
        }
    
        public virtual string MonthlyInsuranceFeeCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderContractFeeUsd
        {
            get;
            set;
        }
    
        public virtual string OrderContractFeeCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderInstallFeeUsd
        {
            get;
            set;
        }
    
        public virtual string OrderInstallFeeCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderDepositFeeUsd
        {
            get;
            set;
        }
    
        public virtual string OrderDepositFeeCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderInstallFee_ApproveContractUsd
        {
            get;
            set;
        }
    
        public virtual string OrderInstallFee_ApproveContractCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderInstallFee_CompleteInstallUsd
        {
            get;
            set;
        }
    
        public virtual string OrderInstallFee_CompleteInstallCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OrderInstallFee_StartServiceUsd
        {
            get;
            set;
        }
    
        public virtual string OrderInstallFee_StartServiceCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> NewBldMgmtCostUsd
        {
            get;
            set;
        }
    
        public virtual string NewBldMgmtCostCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> AdditionalFee1Usd
        {
            get;
            set;
        }
    
        public virtual string AdditionalFee1CurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> AdditionalFee2Usd
        {
            get;
            set;
        }
    
        public virtual string AdditionalFee2CurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> AdditionalFee3Usd
        {
            get;
            set;
        }
    
        public virtual string AdditionalFee3CurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> MaintenanceFee1Usd
        {
            get;
            set;
        }
    
        public virtual string MaintenanceFee1CurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> MaintenanceFee2Usd
        {
            get;
            set;
        }
    
        public virtual string MaintenanceFee2CurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}
