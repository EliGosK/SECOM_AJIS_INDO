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
    [MetadataType(typeof(dtSummaryAR_MetaData))]
    public partial class dtSummaryAR
    {
        public string OfficeName { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class dtSummaryAR_MetaData
    {
        [LanguageMapping]
        public string OfficeName { get; set; }
    }
}
