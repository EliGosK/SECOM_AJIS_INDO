using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory.MetaData;

namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(dtSearchInstrumentListResult_Meta))]
    public partial class dtSearchInstrumentListResult
    {
        public string LocationCode { get; set; }
        public string AreaName { get; set; }
        public string AreaCodeName { get { return CommonUtil.TextCodeName(this.AreaCode, this.AreaName); } }        
        public int? FixedReturnQty { get; set; }
        public int? FixedStockQty { get; set; }
        public string ToJson { get { return CommonUtil.CreateJsonString(this); } }
    }

}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class dtSearchInstrumentListResult_Meta
    {
        [LanguageMapping]
        public string AreaName { get; set; }
    }

}