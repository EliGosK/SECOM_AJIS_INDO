using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Common
{
    public class doDocumentDataCondition
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_COMMON,
                     Screen = "CMS030",
                     Parameter = "lblDocumentType",
                     ControlName = "DocumentType")]
        public string DocumentType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_COMMON,
                     Screen = "CMS030",
                     Parameter = "lblDocumentName",
                     ControlName = "DocumentCode")]
        public string DocumentCode { get; set; }
        public DateTime? GenerateDateFrom { get; set; }
        public DateTime? GenerateDateTo { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public DateTime? dtMonth
        {
            get
            {
                if (this.Month != null)
                {
                    return (new DateTime(2000, (int)this.Month, 1, 0, 0, 0));
                }
                else
                {
                    return null; 
                }
                  
            }
        }

        public DateTime? dtYear
        {
            get
            {
                if (this.Year != null)
                {
                    return (new DateTime((int)this.Year, 1, 1, 0, 0, 0));
                }
                else
                {
                    return null;
                }

            }
        }
       

        public string ContractOfficeCode { get; set; }
        public string OperationOfficeCode { get; set; }
        public string BillingOfficeCode { get; set; }
        public string IssueOfficeCode { get; set; }
        public string DocumentNo { get; set; }
        public string QuotationTargetCode { get; set; }
        public string Alphabet { get; set; }
        public string ProjectCode { get; set; }
        public string ContractCode { get; set; }
        public string OCC { get; set; }
        public string BillingTargetCode { get; set; }
        public string InstrumentCode { get; set; }
        public string OfficeCodeList { get; set; }
        public string LocationCode { get; set; }

        public int Counter { get; set; }
        public int Mode { get; set; }
    }
}
