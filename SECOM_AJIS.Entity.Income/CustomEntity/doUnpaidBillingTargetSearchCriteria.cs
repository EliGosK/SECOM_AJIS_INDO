using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Search criteria for searching billing target unpaid information.
    /// </summary>
    public partial class doUnpaidBillingTargetSearchCriteria
    {
        public string BillingClientName { get; set; }
        public string InvoiceAmountCurrencyType { get; set; }
        public decimal? InvoiceAmountFrom { get; set; }
        public decimal? InvoiceAmountTo { get; set; }
        public DateTime? IssueInvoiceDateFrom { get; set; }
        public DateTime? IssueInvoiceDateTo { get; set; }
        public bool? HaveCreditNoteIssued { get; set; }
        public string BillingDetailAmountCurrencyType { get; set; }
        public decimal? BillingDetailAmountFrom { get; set; }
        public decimal? BillingDetailAmountTo { get; set; }
        public int? BillingCycle { get; set; }
        public int? LastPaymentDayFrom { get; set; }
        public int? LastPaymentDayTo { get; set; }
        public DateTime? ExpectedPaymentDateFrom { get; set; }
        public DateTime? ExpectedPaymentDateTo { get; set; }

        //Ext
        public bool? BillingType_ContractFee { get; set; }
        public bool? BillingType_InstallationFee { get; set; }
        public bool? BillingType_DepositFee { get; set; }
        public bool? BillingType_SalePrice { get; set; }
        public bool? BillingType_OtherFee { get; set; }

        public bool? PaymentMethod_BankTransfer { get; set; }
        public bool? PaymentMethod_Messenger { get; set; }
        public bool? PaymentMethod_AutoTransfer { get; set; }
        public bool? PaymentMethod_CreditCard { get; set; }
    }

}
