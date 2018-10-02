using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Quotation.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Quotation.Models.CustomAttribute;


namespace SECOM_AJIS.DataEntity.Quotation
{
    //[MetadataType(typeof(doUpdateQuotationData_MetaData))]
    //public partial class doUpdateQuotationData_Validate : doUpdateQuotationData
    //{

    //}

}

namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class doUpdateQuotationData_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }

        [DraftLastUpdateDateCorrect]
        public DateTime LastUpdateDate { get; set; }
        [ApproveOrChangeCorrect]
        public string ContractCode { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Quotation.Models.CustomAttribute
{
    public class DraftLastUpdateDateCorrectAttribute : AValidatorAttribute
    {
        public DraftLastUpdateDateCorrectAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            doUpdateQuotationData qt = validationContext.ObjectInstance as doUpdateQuotationData;
            if (qt.ActionTypeCode == ActionType.C_ACTION_TYPE_DRAFT)
            {
                if (CommonUtil.IsNullOrEmpty(value) == true)
                {
                    return base.IsValid(value, validationContext);
                }
            }
            return null;
        }
    }

    public class ApproveOrChangeCorrectAttribute : AValidatorAttribute
    {
        public ApproveOrChangeCorrectAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            doUpdateQuotationData qt = validationContext.ObjectInstance as doUpdateQuotationData;
            if (qt.ActionTypeCode == ActionType.C_ACTION_TYPE_APPROVE || qt.ActionTypeCode == ActionType.C_ACTION_TYPE_CHANGE)
            {
                if (CommonUtil.IsNullOrEmpty(value))
                {
                    return base.IsValid(value, validationContext);
                }
            }
            return null;
        }
    }
}