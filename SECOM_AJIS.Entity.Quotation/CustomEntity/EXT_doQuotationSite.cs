using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public partial class doQuotationSite
    {
        public string SiteNo { get; set; }

        public string SiteCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
    }
}
