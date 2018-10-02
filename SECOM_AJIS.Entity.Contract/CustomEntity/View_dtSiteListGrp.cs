using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class View_dtSiteListGrp : dtsiteListGrp
    {
        public string SiteNameEN_LC
        {
            get { return "(1)" + " " + this.SiteNameEN + "<br/>" + "(2)" + " " + this.SiteNameLC; }
        }
        public string FullAddressEN_LC
        {
            get { return "(1)" + " " + this.AddressFullEN + "<br/>" + "(2)" + " " + this.AddressFullLC; }
        }
    }
}
