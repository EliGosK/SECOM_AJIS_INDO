using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do of site data
    /// </summary>
    public partial class dtSiteData
    {
        public string CustCode_Short { get { return new CommonUtil().ConvertCustCode(this.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT); } }
        public string SiteCode_Short { get { return new CommonUtil().ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT); } }

        // additional
        [LanguageMapping]
        public string CustStatusName { get; set; }
        [LanguageMapping]
        public string BuildingUsageName { get; set; }

        public string CustStatusCodeName { get { return CommonUtil.TextCodeName(this.CustStatus ,this.CustStatusName); } }
        public string BuildingUsageCodeName { get { return CommonUtil.TextCodeName(this.BuildingUsageCode, this.BuildingUsageName); } }
    }


}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{

}