using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class RPTMACheckupSlipDo
    {
        public string CheckupNoShow
        {
            get
            {
                string txt = null;
                if (this.CheckupNo != null)
                {
                    txt = this.CheckupNo.ToString();
                    txt = txt.Substring(0, 2) + txt.Substring(2).PadLeft(5, '0');
                }

                return txt;
            }
        }
        public string ContractCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

    }
}
