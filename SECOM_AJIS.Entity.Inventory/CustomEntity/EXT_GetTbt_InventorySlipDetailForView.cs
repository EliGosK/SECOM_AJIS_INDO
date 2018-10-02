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
    public partial class doTbt_InventorySlipDetailForView
    {
        [LanguageMapping]
        public string SourceAreaName { get; set; }

        [LanguageMapping]
        public string DestinationAreaName { get; set; }

        public string InstrumentAreaCodeName
        {
            get
            {
                string res = "-";
                if (!CommonUtil.IsNullOrEmpty(this.DestinationAreaCode))
                {
                    res = CommonUtil.TextCodeName(this.DestinationAreaCode, this.DestinationAreaName);
                }
                return res;
            }
        }
    }
}
