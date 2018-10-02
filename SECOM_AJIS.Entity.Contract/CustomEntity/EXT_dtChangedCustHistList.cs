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
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(dtChangedCustHistList_MetaData))]
    public partial class dtChangedCustHistList
    {
        public string ChangeNameReasonTypeName { get; set; }
        public string ContractSignerTypeName { get; set; }
        
        public string CSCustName {
            get {
                return "(1) " + CSCustNameEN + "<br/>(2) " + CSCustNameLC;
            }
        }

        public string RCCustName {
            get {
                return "(1) " + RCCustNameEN + "<br/>(2) " + RCCustNameLC;
            }
        }

        public string SiteName {
            get {
                    return "(1) " + SiteNameEN + "<br/>(2) " + SiteNameLC;
            }
        }

        public string ChangeNameReasonTypeDisplay {
            get
            {
                //return ChangeNameReasonType + " : " + ChangeNameReasonTypeNameEN;
                return ChangeNameReasonTypeName;
            }
        }

        //Add by Jutarat A. on 19092012
        public string CSCustCodeName
        {
            get
            {
                return CommonUtil.TextLineFormat(CSCustCode, CSCustNameEN, CSCustNameLC);
            }
        }

        public string RCCustCodeName
        {
            get
            {
                return CommonUtil.TextLineFormat(RCCustCode, RCCustNameEN, RCCustNameLC);
            }
        }

        public string SiteCodeName
        {
            get
            {
                return CommonUtil.TextLineFormat(SiteCode, SiteNameEN, SiteNameLC);
            }
        }
        //End Add
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class dtChangedCustHistList_MetaData
    {
        [LanguageMapping]
        public string ChangeNameReasonTypeName { get; set; }
        [LanguageMapping]
        public string ContractSignerTypeName { get; set; }
    }
}