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
    
    public partial class dtAccountingDocumentList
    {
        public string DocumentNo { get; set; }
        public string DocumentCode { get; set; }
        public System.DateTime TargetPeriodFrom { get; set; }
        public System.DateTime TargetPeriodTo { get; set; }
        public string GenerateHQCode { get; set; }
        public Nullable<int> ReportMonth { get; set; }
        public Nullable<int> ReportYear { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public string DocumentTimingType { get; set; }
        public Nullable<System.DateTime> GenerateDate { get; set; }
    }
}