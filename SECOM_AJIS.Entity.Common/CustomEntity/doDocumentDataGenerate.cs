using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Common
{
    [Serializable]
    public class doDocumentDataGenerate
    {
        //Add by budd to support csv report
        public enum ReportType
        {
            Pdf,
            Csv
        }

        public doDocumentDataGenerate()
        {
            OtherKey = new OtherKeyData();
            this.DocumentReportType = doDocumentDataGenerate.ReportType.Pdf;
        }
        [NotNullOrEmpty]
        public string DocumentNo { get; set; }
        public string DocumentOCC { get; set; }
        [NotNullOrEmpty]
        public string DocumentCode { get; set; }
        [NotNullOrEmpty]
        public object DocumentData { get; set; }
        public OtherKeyData OtherKey { get; set; }
        [NotNullOrEmpty]
        public string EmpNo { get; set; }
        [NotNullOrEmpty]
        public DateTime? ProcessDateTime { get; set; }

        public List<ReportParameterObject> MainReportParam { set; get; }
        public List<ReportParameterObject> SubReportParam { set; get; }
        public List<ReportParameterObject> SubReportDataSource { set; get; }

        // Akat K. : use when document template and document code not the same
        public string DocumentTemplateCode { get; set; }
        public string GetDocumentTemplateCode
        {
            get
            {
                if (DocumentTemplateCode == null)
                {
                    return DocumentCode;
                }
                else
                {
                    return DocumentTemplateCode;
                }
            }
        }

        public string GeneratedReportName { get { return string.Format(@"{0}\{1}{2}_{3}-{4}.pdf",DateTime.Now.ToString("yyyyMM"), DateTime.Now.ToString("yyyyMM"), this.DocumentCode, this.DocumentNo, this.DocumentOCC); } }

        public ReportType DocumentReportType { get; set; }
        public string GeneratedCsvReportName { get; set; }
    }

    [Serializable]
    public class OtherKeyData
    {
        public string ContractCode { get; set; }
        public string ContractOCC { get; set; }
        public string QuotationTargetCode { get; set; }
        public string QuotationAlphabet { get; set; }
        public string Alphabet { get; set; }
        public string ContractOffice { get; set; }
        public string OperationOffice { get; set; }
        public string InstallationSlipIssueOffice { get; set; }
        public string ProjectCode { get; set; }
        public string InventorySlipIssueOffice { get; set; }
        public DateTime MonthYear { get; set; }
        public string InstrumentCode { get; set; }
        public string BillingCode { get; set; }
        public string BillingOffice { get; set; }
        public string BillingTargetCode { get; set; }
        public string LocationCode { get; set; }

        // additional
        public int ManagementNo { set; get; }
        public int MinManagementNo { set; get; }
        public int MaxManagementNo { set; get; }
    }

    public class ReportParameterObject
    {
        public string ParameterName { get; set; }
        public object Value { get; set; }
        public string SubReportName { get; set; }
    }
}
