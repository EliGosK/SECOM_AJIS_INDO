using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
   public class View_tbm_Site:doGetTbm_Site
    {
        public string FullNameEN_LC
        {
            get { return "(1)" + this.SiteNameEN + "<br/>" + "(2)" + this.SiteNameLC; }
        }
        public string AddressFullEN_LC
        {
            get { return "(1)" + this.AddressFullEN + "<br/>" + "(2)" + this.AddressFullLC; }
        }

        public string ToJson
        {
            get
            {
                return SECOM_AJIS.Common.Util.CommonUtil.CreateJsonString(this);
            }
        }
    }
}
