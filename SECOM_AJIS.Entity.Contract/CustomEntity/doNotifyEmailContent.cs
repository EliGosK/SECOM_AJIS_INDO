using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doNotifyEmailContent
    {
        public virtual string ContractCode { get; set; }
        public virtual string ContractTargetNameEN { get; set; }
        public virtual string ContractTargetNameLC { get; set; }
        public virtual string SiteNameEN { get; set; }
        public virtual string SiteNameLC { get; set; }
        public virtual DateTime? ChangeDateOfContractFee { get; set; }
        public virtual decimal? ContractFeeBeforeChange { get; set; }
        public virtual decimal? ContractFeeAfterChange { get; set; }
        public virtual DateTime? ReturnToOriginalFeeDate { get; set; }
        public virtual string OperationOffice { get; set; }
        public virtual string RegisterChangeEmpNameEN { get; set; }
        public virtual string RegisterChangeEmpNameLC { get; set; }
        public virtual string BillingOffice { get; set; }
        public virtual string Sender { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Content { get; set; }
    }
}

