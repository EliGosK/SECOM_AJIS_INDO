//------------------------------------------------------------------------------
// <auto-generated>
//    このコードはテンプレートから生成されました。
//
//    このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//    このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SECOM_AJIS.DataEntity.Accounting
{
    using System;
    
    public partial class dtACC012_Sheet3
    {
        public Nullable<long> RowNumber { get; set; }
        public string FullNameEN { get; set; }
        public string BillingTargetCode { get; set; }
        public string ContractCode { get; set; }
        public string BillingType { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> IssueInvDate { get; set; }
        public Nullable<System.DateTime> BillingStartDate { get; set; }
        public Nullable<System.DateTime> BillingEndDate { get; set; }
        public string BillingAmountCurrencyType { get; set; }
        public Nullable<decimal> BillingAmount { get; set; }
        public Nullable<decimal> Vat { get; set; }
        public string TaxInvoiceNo { get; set; }
        public Nullable<System.DateTime> TaxInvoiceDate { get; set; }
        public Nullable<System.DateTime> FirstSecurityStartDate { get; set; }
    }
}
