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
    [MetadataType(typeof(doSendBillingData_MetaData))]
    public class doSendBillingData
    {
        public virtual string ContractCode { get; set; }
        public virtual DateTime? CancelDate { get; set; }
        public virtual DateTime? ResumeDate { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? StopDate { get; set; }
        public virtual string OCC { get; set; }
        public virtual decimal? Fee { get; set; }
        public virtual bool? CompleteInstallFlag { get; set; }
        public virtual string BillingOCC { get; set; }
        public virtual decimal? ResultBaseFee { get; set; }
    }

    [MetadataType(typeof(doSendBillingData_CheckContractAndCancelDate_MetaData))]
    public class doSendBillingDataCheckContractCancelDate : doSendBillingData
    {

    }

    [MetadataType(typeof(doSendBillingData_CheckContractAndResumeDate_MetaData))]
    public class doSendBillingDataCheckContractResumeDate : doSendBillingData
    {

    }

    [MetadataType(typeof(doSendBillingData_CheckContractAndStartDate_MetaData))]
    public class doSendBillingDataCheckContractStartDate : doSendBillingData
    {

    }

    [MetadataType(typeof(doSendBillingData_CheckContractAndStopDate_MetaData))]
    public class doSendBillingDataCheckContractStopDate : doSendBillingData
    {

    }

    [MetadataType(typeof(doSendBillingData_CheckContractAndOCC_MetaData))]
    public class doSendBillingDataCheckContractOCC : doSendBillingData
    {

    }

    [MetadataType(typeof(doSendBillingData_CheckContractAndFee_MetaData))]
    public class doSendBillingDataCheckContractFee : doSendBillingData
    {

    }

    [MetadataType(typeof(doSendBillingData_CheckContractAndFeeAndOCC_MetaData))]
    public class doSendBillingDataCheckContractFeeOCC : doSendBillingData
    {

    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{

    public class doSendBillingData_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
    }

    public class doSendBillingData_CheckContractAndCancelDate_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public DateTime? CancelDate { get; set; }

        [NotNullOrEmpty]
        public bool? CompleteInstallFlag { get; set; }
    }

    public class doSendBillingData_CheckContractAndResumeDate_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public DateTime? ResumeDate { get; set; }
    }

    public class doSendBillingData_CheckContractAndStartDate_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public DateTime? StartDate { get; set; }
    }

    public class doSendBillingData_CheckContractAndStopDate_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public DateTime? StopDate { get; set; }
    }

    public class doSendBillingData_CheckContractAndOCC_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public string OCC { get; set; }
    }

    public class doSendBillingData_CheckContractAndFee_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public string Fee { get; set; }
    }

    public class doSendBillingData_CheckContractAndFeeAndOCC_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public string BillingOCC { get; set; }

        [NotNullOrEmpty]
        public decimal? ResultBaseFee { get; set; }
    }
}

