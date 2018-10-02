using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(doGetQuotationDataCondition_Q_MetaData))]
    public class doGetQuotationDataCondition 
    {
        public virtual string QuotationTargetCode {get;set;}
        public virtual string Alphabet { get; set; }
        public virtual string ServiceTypeCode { get; set; }
        public virtual string TargetCodeTypeCode { get; set; }
        public virtual bool? ContractFlag { get; set; }
        public virtual string ProductTypeCode { get; set; }
    }

    [MetadataType(typeof(doGetQuotationDataCondition_QS_MetaData))]
    public class doQuotationDataForContractCondition : doGetQuotationDataCondition
    {

    }
    [MetadataType(typeof(doGetQuotationDataCondition_QAP_MetaData))]
    public class doQuotationDataForInstrumentCondition : doGetQuotationDataCondition 
    {
        
    }
    [MetadataType(typeof(doGetQuotationDataCondition_QA_MetaData))]
    public class doQuotationDataForOperationTypeCondition : doGetQuotationDataCondition 
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_QA_MetaData))]
    public class doQuotationDataForFacilityCondition : doGetQuotationDataCondition 
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_QA_MetaData))]
    public class doQuotationDataForBeatGuardCondition : doGetQuotationDataCondition 
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_QA_MetaData))]
    public class doQuotationDataForSentryGuardCondition : doGetQuotationDataCondition 
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_QA_MetaData))]
    public class doQuotationDataForQuotationBasicData : doGetQuotationDataCondition
    {

    }


    [MetadataType(typeof(doGetQuotationDataCondition_Q_MetaData))]
    public class doQuotationDataForGetTbt_TargetCondition : doGetQuotationDataCondition 
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_Q_MetaData))]
    public class doQuotationDataForGetTbt_CustomerCondition : doGetQuotationDataCondition
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_Q_MetaData))]
    public class doQuotationDataForGetTbt_SiteCondition : doGetQuotationDataCondition
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_QA_MetaData))]
    public class doQuotationDataForGetTbt_InstDetailsCondition : doGetQuotationDataCondition
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_QA_MetaData))]
    public class doQuotationDataForGetTbt_OperationTypeCondition : doGetQuotationDataCondition
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_QA_MetaData))]
    public class doQuotationDataForGetTbt_FacilityDetailsCondition : doGetQuotationDataCondition
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_QA_MetaData))]
    public class doQuotationDataForGetTbt_BeatGuardDetailsCondition : doGetQuotationDataCondition
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_QA_MetaData))]
    public class doQuotationDataForGetTbt_SentryGuardDetailsCondition : doGetQuotationDataCondition
    {
    }
    [MetadataType(typeof(doGetQuotationDataCondition_QA_MetaData))]
    public class doQuotationDataForGetTbt_MaintLinkageCondition : doGetQuotationDataCondition
    {
    }
}
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class doGetQuotationDataCondition_Q_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
    }
    public class doGetQuotationDataCondition_QAP_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty]
        public string Alphabet { get; set; }
        [NotNullOrEmpty]
        public string ProductTypeCode { get; set; }
    }
    public class doGetQuotationDataCondition_QA_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty]
        public string Alphabet { get; set; }
    }
    public class doGetQuotationDataCondition_QS_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty]
        public string ServiceTypeCode { get; set; }
    }
    
}
