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
    public partial class doRentalSecurityBasicInformation
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
    
        public string ChangeType
        {
            get;
            set;
        }
    
        public string ImplementFlag
        {
            get;
            set;
        }
    
        public string QuotationTargetCode
        {
            get;
            set;
        }
    
        public string QuotationAlphabet
        {
            get;
            set;
        }
    
        public string SecurityTypeCode
        {
            get;
            set;
        }
    
        public string ProductCode
        {
            get;
            set;
        }
    
        public string PhoneLineTypeCode1
        {
            get;
            set;
        }
    
        public string PhoneLineOwnerTypeCode1
        {
            get;
            set;
        }
    
        public string PhoneNo1
        {
            get;
            set;
        }
    
        public string PhoneLineTypeCode2
        {
            get;
            set;
        }
    
        public string PhoneLineOwnerTypeCode2
        {
            get;
            set;
        }
    
        public string PhoneNo2
        {
            get;
            set;
        }
    
        public string PhoneLineTypeCode3
        {
            get;
            set;
        }
    
        public string PhoneLineOwneTypeCode3
        {
            get;
            set;
        }
    
        public string PhoneNo3
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ExpectedOperationDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ContractStartDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ContractEndDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> CalContractEndDate
        {
            get;
            set;
        }
    
        public Nullable<int> ContractDurationMonth
        {
            get;
            set;
        }
    
        public Nullable<int> AutoRenewMonth
        {
            get;
            set;
        }
    
        public Nullable<int> MaintenanceCycle
        {
            get;
            set;
        }
    
        public Nullable<bool> FireMonitorFlag
        {
            get;
            set;
        }
    
        public Nullable<bool> CrimePreventFlag
        {
            get;
            set;
        }
    
        public Nullable<bool> EmergencyReportFlag
        {
            get;
            set;
        }
    
        public Nullable<bool> FacilityMonitorFlag
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
    
        public string SalesSupporterEmpNo
        {
            get;
            set;
        }
    
        public string BuildingTypeCode
        {
            get;
            set;
        }
    
        public Nullable<bool> NewBldMgmtFlag
        {
            get;
            set;
        }
    
        public Nullable<decimal> NewBldMgmtCost
        {
            get;
            set;
        }
    
        public string MainStructureTypeCode
        {
            get;
            set;
        }
    
        public Nullable<int> NumOfBuilding
        {
            get;
            set;
        }
    
        public Nullable<int> NumOfFloor
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
    
        public Nullable<bool> DivideContractFeeBillingFlag
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ChangeImplementDate
        {
            get;
            set;
        }
    
        public Nullable<decimal> NormalContractFee
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderContractFee
        {
            get;
            set;
        }
    
        public string OrderContractFeePayMethod
        {
            get;
            set;
        }
    
        public Nullable<decimal> ContractFeeOnStop
        {
            get;
            set;
        }
    
        public Nullable<decimal> NormalAdditionalDepositFee
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderAdditionalDepositFee
        {
            get;
            set;
        }
    
        public string DepositFeeBillingTiming
        {
            get;
            set;
        }
    
        public string PlanCode
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
    
        public string AlmightyProgramEmpNo
        {
            get;
            set;
        }
    
        public Nullable<int> CounterNo
        {
            get;
            set;
        }
    
        public string ChangeReasonType
        {
            get;
            set;
        }
    
        public string ChangeNameReasonType
        {
            get;
            set;
        }
    
        public string StopCancelReasonType
        {
            get;
            set;
        }
    
        public string UninstallType
        {
            get;
            set;
        }
    
        public Nullable<bool> ContractDocPrintFlag
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ExpectedInstallationCompleteDate
        {
            get;
            set;
        }
    
        public Nullable<bool> InstallationCompleteFlag
        {
            get;
            set;
        }
    
        public string InstallationSlipNo
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> InstallationCompleteDate
        {
            get;
            set;
        }
    
        public string InstallationTypeCode
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
    
        public Nullable<decimal> CalIndex
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ExpectedResumeDate
        {
            get;
            set;
        }
    
        public Nullable<int> FacilityPassYear
        {
            get;
            set;
        }
    
        public Nullable<int> FacilityPassMonth
        {
            get;
            set;
        }
    
        public Nullable<decimal> InsuranceCoverageAmount
        {
            get;
            set;
        }
    
        public string InsuranceTypeCode
        {
            get;
            set;
        }
    
        public Nullable<decimal> MonthlyInsuranceFee
        {
            get;
            set;
        }
    
        public Nullable<decimal> NormalInstallFee
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderInstallFee
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderInstallFee_ApproveContract
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderInstallFee_CompleteInstall
        {
            get;
            set;
        }
    
        public Nullable<decimal> OrderInstallFee_StartService
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
    
        public string DispatchTypeCode
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ReturnToOriginalFeeDate
        {
            get;
            set;
        }
    
        public Nullable<decimal> TotalSlideAmt
        {
            get;
            set;
        }
    
        public Nullable<decimal> TotalReturnAmt
        {
            get;
            set;
        }
    
        public Nullable<decimal> TotalBillingAmt
        {
            get;
            set;
        }
    
        public Nullable<bool> TotalAmtAfterCounterBalanceFlag
        {
            get;
            set;
        }
    
        public Nullable<decimal> TotalAmtAfterCounterBalance
        {
            get;
            set;
        }
    
        public Nullable<bool> ReturnCancelContractFeeFlag
        {
            get;
            set;
        }
    
        public Nullable<decimal> AdditionalFee1
        {
            get;
            set;
        }
    
        public Nullable<decimal> AdditionalFee2
        {
            get;
            set;
        }
    
        public Nullable<decimal> AdditionalFee3
        {
            get;
            set;
        }
    
        public string AdditionalApproveNo1
        {
            get;
            set;
        }
    
        public string AdditionalApproveNo2
        {
            get;
            set;
        }
    
        public string AdditionalApproveNo3
        {
            get;
            set;
        }
    
        public Nullable<decimal> MaintenanceFee1
        {
            get;
            set;
        }
    
        public Nullable<decimal> MaintenanceFee2
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> CompleteChangeOperationDate
        {
            get;
            set;
        }
    
        public string CompleteChangeOperationEmpNo
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
    
        public string FacilityMemo
        {
            get;
            set;
        }
    
        public Nullable<int> DepreciationPeriodContract
        {
            get;
            set;
        }
    
        public Nullable<int> DepreciationPeriodRevenue
        {
            get;
            set;
        }
    
        public string DocumentCode
        {
            get;
            set;
        }
    
        public string DocAuditResult
        {
            get;
            set;
        }
    
        public string SecurityMemo
        {
            get;
            set;
        }
    
        public System.DateTime CreateDate
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

        #endregion

    }
}
