using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models.EmailTemplates
{
    /// <summary>
    /// DO for change fee email
    /// </summary>
    public class doChangeFeeEmailObject : ATemplateObject
    {
        public string ContractCode { get; set; }
        public string ContractTargetNameEN { get; set; }
        public string ContractTargetNameLC { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteNameLC { get; set; }
        public string ChangeDateOfContractFee { get; set; }
        public string ContractFeeBeforeChange { get; set; }
        public string ContractFeeAfterChange { get; set; }
        public string ReturnToOriginalFeeDate { get; set; }
        public string OperationOffice { get; set; }
        public string RegisterChangeEmpNameEN { get; set; }
        public string RegisterChangeEmpNameLC { get; set; }
        public string BillingOffice { get; set; }
        public string Sender { get; set; }
    }
}
