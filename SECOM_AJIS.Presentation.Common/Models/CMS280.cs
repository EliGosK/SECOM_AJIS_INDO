using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Do contain Site Information and Contract with same site.
    /// </summary>
    public class dsSiteInfoForView
    {
        public List<dtSiteData> dtSiteData { get; set; }
        public List<dtContractsSameSite> dtContractsSameSite { get; set; }
    }

    /// <summary>
    /// Parameter for screen CMS280.
    /// </summary>
    public class CMS280_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string strSiteCode { get; set; }
        [KeepSession]
        public dsSiteInfoForView dsSiteIfoForView { get; set; }
    }
}
