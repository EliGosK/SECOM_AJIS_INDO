using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter of CMS420
    /// </summary>
    public class CMS420_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public string ContractCode { set; get; }

        [KeepSession]
        public string BillingOCC { set; get; }

        [KeepSession]
        public string ServiceTypeCode{ set; get; }
    }
    /// <summary>
    /// Do Of view billing basic
    /// </summary>
    public class dtViewBillingBasic_ForView
    {
        public string BillingCode { set; get; }
        public string BillingOffice { set; get; }
        public string DebtTracingOffice { set; get; }
        public string BillingTargetCode { set; get; }
        public string PreviousBillingTargetCode { set; get; }
        public string CustomerType { set; get; }
        public string BillingClientNameEN { set; get; }
        public string BillingClientBranchNameEN { set; get; }
        public string BillingClientAddressEN { set; get; }
        public string BillingClientNameLC { set; get; }
        public string BillingClientBranchNameLC { set; get; }
        public string BillingClientAddressLC { set; get; }
        public string MonthlyBillingAmount { set; get; }
        public string PaymentMethod { set; get; }
        public string BillingCycle { set; get; }
        public string CreditTerm { set; get; }
        public string CalculationDailyFee { set; get; }
        public string LastBillingDate { set; get; }
        public string ManagementCodeForSortDetails { set; get; }
        public string AdjustEndingDateOfBillingPeriod { set; get; }
        public string BillingFlag { set; get; }
        public bool VATUnchargedBillingTarget { set; get; }
        public string BalanceOfDepositFee { set; get; }
        public string MonthlyFeeBeforeStop { set; get; }
        public bool ResultBasedMaintenanceBillingFlag { set; get; }
        public string LastPaymentConditionChangingDate { set; get; }
        public string RegisteringDateOfLastChanging { set; get; }
        public string ApproveNo { set; get; }
        public string DocumentReceiving { set; get; }

        public string AdjustmentType { set; get; }
        public string AdjustBillingAmount { set; get; }

        public string AdjustBillingPeriodStartDate { set; get; }
        public string AdjustBillingPeriodEndDate { set; get; }

        public string IDNo { set; get; } //Add by Jutarat A. on 12122013
    }

    /// <summary>
    /// Do of table monthly billing history list for view
    /// </summary>
    public class doTbt_MonthlyBillingHistoryList_ForView
    {
        public string LastMonthlyBillingAmount { set; get; }
        public string LastDate { set; get; }
        public string BillingAmountBeforeChanging1 { set; get; }
        public string DateBeforeChanging1 { set; get; }
        public string BillingAmountBeforeChanging2 { set; get; }
        public string DateBeforeChanging2 { set; get; }
        public string BillingAmountBeforeChanging3 { set; get; }
        public string DateBeforeChanging3 { set; get; }
        public string BillingAmountBeforeChanging4 { set; get; }
        public string DateBeforeChanging4 { set; get; }
        public string BillingAmountBeforeChanging5 { set; get; }
        public string DateBeforeChanging5 { set; get; }
    }
}
