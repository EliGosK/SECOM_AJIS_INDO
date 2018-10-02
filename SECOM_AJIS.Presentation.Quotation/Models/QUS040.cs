using System;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Presentation.Quotation.Models.MetaData;
using SECOM_AJIS.Common.Models;
namespace SECOM_AJIS.Presentation.Quotation.Models
{
    /// <summary>
    /// DO for initial screen
    /// </summary>
    public class QUS040_ScreenParameter:ScreenParameter
    {
        
    }
    /// <summary>
    /// DO for getting quotation target data
    /// </summary>
    [MetadataType(typeof(doSearchQuotationTarget_MetaData))]
    public class QUS040_SearchQuotationTarget : doSearchQuotationTargetListCondition
    {
    }
}
namespace SECOM_AJIS.Presentation.Quotation.Models.MetaData
{ 
    /// <summary>
    /// Metadata for doSearchQuotationTarget DO
    /// </summary>
    public class doSearchQuotationTarget_MetaData
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string QuotationTargetCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string ProductTypeCode { get; set; }
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
        public string StaffCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string StaffName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public DateTime QuotationDateFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public DateTime QuotationDateTo { get; set; }
    }
}
