using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtsiteListGrp
    {
        CommonUtil c = new CommonUtil();
        public string SiteCodeShort
        {
            get { return c.ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT); }
        }

        public string SiteNameForShow
        {
            get { return "(1) " + this.SiteNameEN + "<br>(2) " + this.SiteNameLC; }
        }

        public string AddressFullForShow
        {
            get { return "(1) " + this.AddressFullEN + "<br>(2) " + this.AddressFullLC; }
        }
    }
}



