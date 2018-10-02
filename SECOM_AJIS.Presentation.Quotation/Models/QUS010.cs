using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Presentation.Quotation.Models.MetaData;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Quotation.Models
{
    /// <summary>
    /// Do for getting quotation data
    /// </summary>
    [MetadataType(typeof(doSearchQuotationListCondition_MetaData))]
    public class QUS010_SearchQuotation : doSearchQuotationListCondition
    {
        public string ViewMode { get; set; }
    }

    /// <summary>
    /// DO for initial screen
    /// </summary>
    public class QUS010_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string strServiceTypeCode { get; set; }
        [KeepSession]
        public string strTargetCodeTypeCode { get; set; }
        [KeepSession]
        public string strQuotationTargetCode { get; set; }
        [KeepSession]
        public string strContractTransferStatus { get; set; }
        [KeepSession]
        public string ViewMode { get; set; }
    }
    /// <summary>
    /// Do for validating condition if call from CTS010 or CTS020
    /// </summary>
    [MetadataType(typeof(doQUS010Condition_FN_Q99_MetaData))]
    public class doQUS010Condition_FN_Q99 : QUS010_ScreenParameter { }
    /// <summary>
    /// Do for validating condition if call from CTS051, CTS052 or CTS062
    /// </summary>
    [MetadataType(typeof(doQUS010Condition_CP_Q12_MetaData))]
    public class doQUS010Condition_CPQ12 : QUS010_ScreenParameter { }
}
namespace SECOM_AJIS.Presentation.Quotation.Models.MetaData
{
    /// <summary>
    /// Metadata for doSearchQuotationListCondition DO
    /// </summary>
    public class doSearchQuotationListCondition_MetaData
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string QuotationTargetCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        [CodeNullOtherNull("QuotationTargetCode",
            Module = MessageUtil.MODULE_QUOTATION,
            MessageCode = MessageUtil.MessageList.MSG2002,
            ControlName = "QuotationTargetCode",
            Screen = "QUS010",
            Parameter = "lblQuotationCode")]
        public string Alphabet { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string ProductTypeCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string LockStatus { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string QuotationOfficeCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string OperationOfficeCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string ContractTargetCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string ContractTargetName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string ContractTargetAddr { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string SiteCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string SiteName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string SiteAddr { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string EmpNo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string EmpName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public DateTime? QuotationDateFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public DateTime? QuotationDateTo { get; set; }
        public String ServiceTypeCode { get; set; }
        public String TargetCodeTypeCode { get; set; }
    }
    /// <summary>
    /// Metadata for doQUS010Condition_FN_Q99 DO
    /// </summary>
    public class doQUS010Condition_FN_Q99_MetaData
    {
        [NotNullOrEmpty]
        public string strServiceTypeCode { get; set; }
        [NotNullOrEmpty]
        public string strTargetCodeTypeCode { get; set; }
    }
    /// <summary>
    /// Metadata for doQUS010Condition_CP_Q12 DO
    /// </summary>
    public class doQUS010Condition_CP_Q12_MetaData
    {
        [NotNullOrEmpty]
        public string strServiceTypeCode { get; set; }
        [NotNullOrEmpty]
        public string strTargetCodeTypeCode { get; set; }
        [NotNullOrEmpty]
        public string strQuotationTargetCode { get; set; }
    }


}
