using System;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    public class View_tbm_Product : tbm_Product
    {
        [LanguageMapping]
        public string ProductName { get; set; }
        public string ValueDisplay { get; set; }
        public string ProductCodeName
        {
            get
            {
                return SECOM_AJIS.Common.Util.CommonUtil.TextCodeName(
                    this.ProductCode,
                    this.ProductName);
            }
        }
    }
}
