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
    public partial class doResultReturnInstrument
    {
        [LanguageMapping]
        public string AreaName { get; set; }

        public string InstrumentAreaCodeName
        {
            get
            {
                string res = "-";
                if (!CommonUtil.IsNullOrEmpty(this.DestinationAreaCode))
                {
                    res = CommonUtil.TextCodeName(this.DestinationAreaCode, this.AreaName);
                }
                return res;
            }
        }

        public int? ContractRemoveQty
        {
            get
            {
                int contract= 0;
                int remove =  this.RemoveQty == null ? 0 : Convert.ToInt32(this.RemoveQty);
                int unremove = this.UnRemovableQty == null ? 0 : Convert.ToInt32(this.UnRemovableQty);
                contract = remove + unremove;

                if (this.RemoveQty == null && this.UnRemovableQty == null)
                    return null;
                else
                    return contract;
            }
        }
    }
}
