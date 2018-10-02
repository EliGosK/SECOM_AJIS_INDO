using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter for CMS480
    /// </summary>
    public class CMS480_ScreenParameter : ScreenParameter
    {
        public string ReportMonth { get; set; }
        public string ReportYear { get; set; }
    }


    public class CMS480_ManageCarryOverProfit_Param
    {
        public string ReportMonthYear { get; set; }
        public string BillingCode { get; set; }
        public Nullable<decimal> ReceiveAmount { get; set; }
        public Nullable<decimal> ReceiveAmountUsd { get; set; }
        public string ReceiveAmountCurrencyType { get; set; }
        public Nullable<decimal> IncomeRentalFee { get; set; }
        public Nullable<decimal> IncomeRentalFeeUsd { get; set; }
        public string IncomeRentalFeeCurrencyType { get; set; }
        public Nullable<decimal> AccumulatedReceiveAmount { get; set; }
        public Nullable<decimal> AccumulatedReceiveAmountUsd { get; set; }
        public string AccumulatedReceiveAmountCurrencyType { get; set; }
        public Nullable<decimal> AccumulatedUnpaid { get; set; }
        public Nullable<decimal> AccumulatedUnpaidUsd { get; set; }
        public string AccumulatedUnpaidCurrencyType { get; set; }
        public Nullable<decimal> IncomeVat { get; set; }
        public Nullable<decimal> IncomeVatUsd { get; set; }
        public string IncomeVatCurrencyType { get; set; }
        public Nullable<decimal> UnpaidPeriod { get; set; }
        public Nullable<System.DateTime> IncomeDate { get; set; }

    }

}
