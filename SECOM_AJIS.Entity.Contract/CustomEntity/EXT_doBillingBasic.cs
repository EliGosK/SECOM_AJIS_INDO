﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(doBillingBasic_MetaData))]
    public partial class doBillingTempBasic
    {
        
    }

    [MetadataType(typeof(doBillingBasic_ChangeFee_MetaData))]
    public class doBillingBasicChangeFee : doBillingTempBasic
    {

    }

    [MetadataType(typeof(doBillingBasic_ChangeName_MetaData))]
    public class doBillingBasicChangeName : doBillingTempBasic
    {

    }

    [MetadataType(typeof(doBillingBasic_CheckBillingOCC_MetaData))]
    public class doBillingBasicCheckBillingOCC : doBillingTempBasic
    {

    }

    [MetadataType(typeof(doBillingBasic_CheckBillingTargetCode_MetaData))]
    public class doBillingBasicCheckBillingTargetCode : doBillingTempBasic
    {

    }

    [MetadataType(typeof(doBillingBasic_CheckBillingClientOfficeCode_MetaData))]
    public class doBillingBasicCheckBillingClientOfficeCode : doBillingTempBasic
    {

    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{

    public class doBillingBasic_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
    }

    public class doBillingBasic_ChangeFee_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        //[NotNullOrEmpty]
        //public virtual string PayMethod { get; set; }

        [NotNullOrEmpty]
        public virtual string ContractBillingType { get; set; }

        [NotNullOrEmpty]
        public virtual Nullable<decimal> BillingAmount { get; set; }

        [NotNullOrEmpty]
        public virtual Nullable<System.DateTime> ChangeFeeDate { get; set; }
    }

    public class doBillingBasic_ChangeName_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public string BillingOCC { get; set; }
    }

    public class doBillingBasic_CheckBillingOCC_MetaData
    {
        [NotNullOrEmpty]
        public string BillingOCC { get; set; }
    }

    public class doBillingBasic_CheckBillingTargetCode_MetaData
    {
        [NotNullOrEmpty]
        public string BillingTargetCode { get; set; }
    }

    public class doBillingBasic_CheckBillingClientOfficeCode_MetaData
    {
        [NotNullOrEmpty]
        public string BillingClientCode { get; set; }

        [NotNullOrEmpty]
        public string BillingOfficeCode { get; set; }
    }
}