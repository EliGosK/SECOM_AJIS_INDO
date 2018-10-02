using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    
    public partial class dtCheckingDetailList
    {   
        [LanguageMapping]
        public string AreaName { get; set; }
        public string AreaCodeName { get { return CommonUtil.TextCodeName(this.AreaCode, this.AreaName); } }

        public int Page { get; set; }
        public string key { get { return string.Format("{0}-{1}-{2}", this.InstrumentCode, this.AreaCode, this.ShelfNo); } }
        public string txtCheckingQtyID { set; get; }
        public int? DefaultCheckingQty { get; set; }
        public string ToJson { get { return CommonUtil.CreateJsonString(this); } }
    }

  

}
