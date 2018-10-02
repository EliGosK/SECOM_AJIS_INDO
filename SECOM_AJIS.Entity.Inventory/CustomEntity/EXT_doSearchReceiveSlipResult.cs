using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class doSearchReceiveSlipResult
    {
        public string ContractCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set
            {
                CommonUtil c = new CommonUtil();
                this.ContractCode = c.ConvertContractCode(value, CommonUtil.CONVERT_TYPE.TO_LONG);
            }
        }

        [LanguageMapping]
        public string SubContractorName { get; set; }
    }
}