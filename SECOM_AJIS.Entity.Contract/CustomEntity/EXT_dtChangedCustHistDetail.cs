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
    [MetadataType(typeof(dtChangedCustHistDetail_MetaData))]
    public partial class dtChangedCustHistDetail
    {
        public string ContractSignerTypeName { get; set; }
        public string CSCustStatusName { get; set; }
        public string CSCustTypeName { get; set; }
        public string CSFinancialMaketTypeName { get; set; }
        public string CSNationality { get; set; }
        public string CSBusinessTypeName { get; set; }
        public string RCCustStatusName { get; set; }
        public string RCCustTypeName { get; set; }
        public string RCFinancialMaketTypeName { get; set; }
        public string RCNationality { get; set; }
        public string RCBusinessTypeName { get; set; }
        public string SiteName { get; set; }
        public string SiteBuildingUsageName { get; set; }

        public string ContractSignerTypeDisplay {
            get {
                if ((ContractSignerTypeCode == null || ContractSignerTypeCode.Trim() == "")
                        && (ContractSignerTypeName == null || ContractSignerTypeName.Trim() == ""))
                    return null;
                else
                    return ContractSignerTypeCode + " : " + ContractSignerTypeName;
            }
        }

        public string CSCustStatusDisplay {
            get {
                if ((CSCustStatus == null || CSCustStatus.Trim() == "")
                    && (CSCustStatusName == null || CSCustStatusName.Trim() == ""))
                    return null;
                else
                    return CSCustStatus + " : " + CSCustStatusName;
            }
        }

        public string CSCustTypeDisplay {
            get {
                if ((CSCustTypeCode == null || CSCustTypeCode.Trim() == "")
                    && (CSCustTypeName == null || CSCustTypeName.Trim() == ""))
                    return null;
                else
                    return CSCustTypeCode + " : " + CSCustTypeName;
            }
        }

        public string CSFinancialMarketTypeDisplay {
            get {
                if ((CSFinancialMarketTypeCode == null || CSFinancialMarketTypeCode.Trim() == "")
                    && (CSFinancialMaketTypeName == null || CSFinancialMaketTypeName.Trim() == ""))
                    return null;
                else
                    return CSFinancialMarketTypeCode + " : " + CSFinancialMaketTypeName;
            }
        }

        public string CSRegionDisplay {
            get {
                if ((CSRegionCode == null || CSRegionCode.Trim() == "") && (CSNationality == null || CSNationality.Trim() == ""))
                    return null;
                else
                    return CSRegionCode + " : " + CSNationality;
            }
        }

        public string CSBusinessTypeDisplay {
            get {
                if ((CSBusinessTypeCode == null || CSBusinessTypeCode.Trim() == "") && (CSBusinessTypeName == null || CSBusinessTypeName.Trim() == ""))
                    return null;
                else
                    return CSBusinessTypeCode + " : " + CSBusinessTypeName;
            }
        }

        public string RCCustStatusDisplay {
            get {
                if ((RCCustStatus == null || RCCustStatus.Trim() == "") && (RCCustStatusName == null || RCCustStatusName.Trim() == ""))
                    return null;
                else
                    return RCCustStatus + " : " + RCCustStatusName;
            }
        }

        public string RCCustTypeDisplay {
            get {
                if ((RCCustTypeCode == null || RCCustTypeCode.Trim() == "") && (RCCustTypeName == null || RCCustTypeName.Trim() == ""))
                    return null;
                else
                    return RCCustTypeCode + " : " + RCCustTypeName;
            }
        }

        public string RCFinancialMarketTypeDisplay {
            get {
                if ((RCFinancialMarketTypeCode == null || RCFinancialMarketTypeCode.Trim() == "")
                    && (RCFinancialMaketTypeName == null || RCFinancialMaketTypeName.Trim() == ""))
                    return null;
                else
                    return RCFinancialMarketTypeCode + " : " + RCFinancialMaketTypeName;
            }
        }

        public string RCRegionDisplay {
            get {
                if ((RCRegionCode == null || RCRegionCode.Trim() == "") && (RCNationality == null || RCNationality.Trim() == ""))
                    return null;
                else
                    return RCRegionCode + " : " + RCNationality;
            }
        }

        public string RCBusinessTypeDisplay {
            get {
                if ((RCBusinessTypeCode == null || RCBusinessTypeCode.Trim() == "") && (RCBusinessTypeName == null || RCBusinessTypeName.Trim() == ""))
                    return null;
                else
                    return RCBusinessTypeCode + " : " + RCBusinessTypeName;
            }
        }

        public string SiteBuildingUsageDisplay {
            get {
                if ((SiteBuildingUsageCode == null || SiteBuildingUsageCode.Trim() == "") && (SiteBuildingUsageName == null || SiteBuildingUsageName.Trim() == ""))
                    return null;
                else
                    return SiteBuildingUsageCode + " : " + SiteBuildingUsageName;
            }
        }
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class dtChangedCustHistDetail_MetaData
    {
        [LanguageMapping]
        public string ContractSignerTypeName { get; set; }
        [LanguageMapping]
        public string CSCustStatusName { get; set; }
        [LanguageMapping]
        public string CSCustTypeName { get; set; }
        [LanguageMapping]
        public string CSFinancialMaketTypeName { get; set; }
        [LanguageMapping]
        public string CSNationality { get; set; }
        [LanguageMapping]
        public string CSBusinessTypeName { get; set; }
        [LanguageMapping]
        public string RCCustStatusName { get; set; }
        [LanguageMapping]
        public string RCCustTypeName { get; set; }
        [LanguageMapping]
        public string RCFinancialMaketTypeName { get; set; }
        [LanguageMapping]
        public string RCNationality { get; set; }
        [LanguageMapping]
        public string RCBusinessTypeName { get; set; }
        [LanguageMapping]
        public string SiteName { get; set; }
        [LanguageMapping]
        public string SiteBuildingUsageName { get; set; }
    }
}
