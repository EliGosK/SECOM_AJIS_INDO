using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;

using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;

namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(doInventorySlipDetailList_Meta))]
    public partial class doInventorySlipDetailList
    {
        public string StockInTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.StockInFlag, this.StockInName);

            }
        }
        public string StockInName { get; set; }
        public string AreaName { get; set; }
        public string AreaCodeName
        {
            get
            {
                string res = "-";
                if (!CommonUtil.IsNullOrEmpty(this.SourceAreaCode))
                {
                    res = CommonUtil.TextCodeName(this.SourceAreaCode, this.AreaName);
                }
                return res;
            }
        }
        public string RegisterAssetName { get; set; }
    }
}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{

    public partial class doInventorySlipDetailList_Meta
    {

        [LanguageMapping]
        public string StockInName { get; set; }

        [LanguageMapping]
        public string AreaName { get; set; }

        [LanguageMapping]
        public string RegisterAssetName { get; set; }   

    }
}