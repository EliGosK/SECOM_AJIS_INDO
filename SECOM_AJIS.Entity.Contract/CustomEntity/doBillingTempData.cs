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
    [MetadataType(typeof(doBillingTempData_MetaData))]
    public class doBillingTempData
    {
        public virtual string ContractCode { get; set; }
        public virtual string OCC { get; set; }
        public virtual int SequenceNo { get; set; }
        public virtual string BillingType { get; set; }
        public virtual Nullable<decimal> BillingAmt { get; set; }
        public virtual string PayMethod { get; set; }
        public virtual string BillingClientCode { get; set; }
        public virtual string BillingOfficeCode { get; set; }
        public virtual string BillingOCC { get; set; }
        public virtual string BillingTargetCode { get; set; }
    }

    [MetadataType(typeof(doBillingTempData_CheckContractAndOCC_MetaData))]
    public class doBillingTempDataCheckContractOCC : doBillingTempData
    {

    }

    [MetadataType(typeof(doBillingTempData_CheckContractAndSequence_MetaData))]
    public class doBillingTempDataCheckContractSequence : doBillingTempData
    {

    }

    [MetadataType(typeof(doBillingTempData_Insert_MetaData))]
    public class doBillingTempData_Insert : doBillingTempData
    {

    }

    [MetadataType(typeof(doBillingTempData_Update_MetaData))]
    public class doBillingTempData_Update : tbt_BillingTemp
    {

    }

    [MetadataType(typeof(doBillingTempData_UpdateBillingClientAndOffice_MetaData))]
    public class doBillingTempData_UpdateBillingClientAndOffice : tbt_BillingTemp
    {

    }

    [MetadataType(typeof(doBillingTempData_ClientOfficeCode_MetaData))]
    public class doBillingTempData_ClientOfficeCode : tbt_BillingTemp
    {

    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{

    public class doBillingTempData_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
    }

    public class doBillingTempData_CheckContractAndOCC_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public string OCC { get; set; }
    }

    public class doBillingTempData_CheckContractAndSequence_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public string OCC { get; set; }
        [NotNullOrEmpty]
        public int SequenceNo { get; set; }
    }

    public class doBillingTempData_Insert_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public string OCC { get; set; }
        [NotNullOrEmpty]
        public string BillingType { get; set; }

        /*[NotNullOrEmpty]
        public Nullable<decimal> BillingAmt { get; set; }*/ //Comment by Jutarat A. on 13112013
        
        // Comment by Natthavat S.
        // 26/05/2012
        //[NotNullOrEmpty]
        //public string PayMethod { get; set; }
    }

    public class doBillingTempData_Update_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public string OCC { get; set; }
        [NotNullOrEmpty]
        public string SequenceNo { get; set; }
        [NotNullOrEmpty]
        public string BillingType { get; set; }

        /*[NotNullOrEmpty]
        public Nullable<decimal> BillingAmt { get; set; }*/ //Comment by Jutarat A. on 13112013

        // Comment by Natthavat S.
        // 26/05/2012
        //[NotNullOrEmpty]
        //public string PayMethod { get; set; }
    }

    public class doBillingTempData_ClientOfficeCode_MetaData
    {
        [NotNullOrEmpty]
        public string BillingClientCode { get; set; }
        [NotNullOrEmpty]
        public string BillingOfficeCode { get; set; }
    }

    public class doBillingTempData_UpdateBillingClientAndOffice_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public string BillingClientCode { get; set; }
        [NotNullOrEmpty]
        public string BillingOfficeCode { get; set; }
        [NotNullOrEmpty]
        public string BillingOCC { get; set; }
        [NotNullOrEmpty]
        public string BillingTargetCode { get; set; }
    }
}

