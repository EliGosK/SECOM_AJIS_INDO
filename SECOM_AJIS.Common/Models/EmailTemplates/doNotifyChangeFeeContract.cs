using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models.EmailTemplates
{
    /// <summary>
    /// DO for notify change fee contract
    /// </summary>
    public class doNotifyChangeFeeContract : ATemplateObject
    {
        public virtual string ContractCode { get; set; }
        public virtual string ContractTargetNameEN { get; set; }
        public virtual string ContractTargetNameLC { get; set; }
        public virtual string SiteNameEN { get; set; }
        public virtual string SiteNameLC { get; set; }
        public virtual string ChangeDateOfContractFee { get; set; }
        public virtual string ContractFeeBeforeChange { get; set; }
        public virtual string ContractFeeAfterChange { get; set; }
        public virtual string ReturnToOriginalFeeDate { get; set; }
        public virtual string OperationOfficeEN { get; set; }
        public virtual string OperationOfficeLC { get; set; }
        public virtual string RegisterChangeEmpNameEN { get; set; }
        public virtual string RegisterChangeEmpNameLC { get; set; }
        public virtual string BillingOfficeEN { get; set; }
        public virtual string BillingOfficeLC { get; set; }
        public virtual string Sender { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Content { get; set; }
    }
}
