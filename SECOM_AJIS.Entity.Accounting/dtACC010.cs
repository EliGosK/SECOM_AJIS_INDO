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
    
    public partial class dtACC010
    {
        public Nullable<long> RowNumber { get; set; }
        public string CustFullNameEN { get; set; }
        public string ContractCode { get; set; }
        public string SiteNameEN { get; set; }
        public string PlanCode { get; set; }
        public string OfficeNameEN { get; set; }
        public Nullable<System.DateTime> CustAcceptanceDate { get; set; }
        public string ProductNameEN { get; set; }
        public string SalesmanName { get; set; }
        public string OrderProductPriceCurrencyType { get; set; }
        public Nullable<decimal> OrderProductPrice { get; set; }
        public Nullable<decimal> InstrumentCost { get; set; }
        public string OrderInstallFeeCurrencyType { get; set; }
        public Nullable<decimal> OrderInstallFee { get; set; }
        public string PayToSubcontractorCurrencyType { get; set; }
        public Nullable<decimal> PayToSubcontractor { get; set; }
        public string InstrumentCostCurrencyType { get; set; }
    }
}