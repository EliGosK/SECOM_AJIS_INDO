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
    [MetadataType(typeof(doBillingDetail_MetaData))]
    public partial class doBillingTempDetail
    {
       
    }
    
    [MetadataType(typeof(doBillingDetail_SaleCompleteInstall_MetaData))]
    public class doBillingDetailSaleCompleteInstall : doBillingTempDetail
    {

    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class doBillingDetail_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
    }


    public class doBillingDetail_SaleCompleteInstall_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public virtual string BillingOCC { get; set; }

        [NotNullOrEmpty]
        public virtual string ContractBillingType { get; set; }

        [NotNullOrEmpty]
        public virtual Nullable<decimal> BillingAmount { get; set; }
    }
}