using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Search criteria for searching payment transaction information.
    /// </summary>
    public partial class doPaymentSearchCriteria
    {
        //ICS080
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public int? SECOMAccountID { get; set; }
        public string PaymentTransNo { get; set; }
        public string Payer { get; set; }
        public DateTime? PaymentDateFrom { get; set; }
        public DateTime? PaymentDateTo { get; set; }
        public decimal? MatchableBalanceFrom { get; set; }
        public decimal? MatchableBalanceTo { get; set; }
        public string InvoiceNo { get; set; }
        public string ReceiptNo { get; set; }
        public string SendingBank { get; set; }
        public string MatchRGroupName { get; set; }
        public bool? MyPayment { get; set; }
        public string EmpNo { get; set; }
        public string MatchableBalancCurrencyType { get; set; }
    }
}


