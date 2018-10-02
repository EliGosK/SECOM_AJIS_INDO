using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doDraftRentalContractCondition
    {
        public string QuotationTargetCode { get; set; }
        public string QuotationTargetCodeLong
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertQuotationTargetCode(this.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            }
        }

        public string Alphabet { get; set; }
    }
}
