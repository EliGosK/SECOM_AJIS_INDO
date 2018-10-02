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
    public partial class tbt_ContractDocument
    {
        CommonUtil comUtil = new CommonUtil();

        public string ContractCodeShort
        {
            get
            {
                return comUtil.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public virtual string QuotationTargetCodeShort
        {
            get
            {
                return comUtil.ConvertQuotationTargetCode(QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public virtual string DocNoShort
        {
            get
            {
                return comUtil.ConvertContractCode(DocNo, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }



    }
}
