
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
    public enum ActionFlag { Insert = 1, Delete = 2, Edit = 3 }
    [MetadataType(typeof(dtTbt_ProjectStockoutBranchIntrumentDetailForView_MetaData))]
    public partial class dtTbt_ProjectStockoutBranchIntrumentDetailForView
    {
        public ActionFlag ActionFlag { get; set; }

    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class dtTbt_ProjectStockoutBranchIntrumentDetailForView_MetaData
    {

    }
}