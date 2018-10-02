
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
    [MetadataType(typeof(dtTbt_ProjectSystemDetailForView_MetaData))]
    public partial class dtTbt_ProjectSystemDetailForView
    {
        public string ProductName { get; set; }
        public string ProductCodeName { get { return this.ProductCode + ": " + this.ProductName; } }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class dtTbt_ProjectSystemDetailForView_MetaData
    {
        [LanguageMapping]
        public string ProductName { get; set; }
    }
}