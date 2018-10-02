using System.Collections.Generic;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Income;

namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Screen caller mode for screen ICS_033 screen
    /// </summary>
    public enum ICS033_ScreenCallerMode
    {
        GetByBillingTarget = 1,
        GetByInvoice,
        GetByBillingCode
    }

    /// <summary>
    /// Parameter of ICS033 screen
    /// </summary>
    public class ICS033_ScreenParameter : ScreenParameter
    {
        public string BillingOfficeCode { set; get; }
        public string BillingOfficeName { set; get; }
        public string BillingClientNameEN { set; get; }
        public string BillingClientNameLC { set; get; }
        public string BillingClientAddressEN { set; get; }
        public string BillingClientAddressLC { set; get; }
        public string BillingClientTelNo { set; get; }
        public string ContactPersonName { set; get; }
        public string UnpaidAmountString { set; get; }
        
        public ICS033_ScreenCallerMode Mode { set; get; }
        
        public string BillingTargetCode { set; get; }
        public string InvoiceNo { set; get; }
        public int? InvoiceOCC { set; get; }
        public string BillingCode { set; get; }

        public List<doGetUnpaidDetailDebtSummary> UnpaidDetailDebtSummary { set; get; }
    }
}
