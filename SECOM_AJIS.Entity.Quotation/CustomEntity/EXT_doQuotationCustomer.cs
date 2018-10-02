using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public partial class doQuotationCustomer
    {
        public string CustCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertCustCode(this.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
    }
}
