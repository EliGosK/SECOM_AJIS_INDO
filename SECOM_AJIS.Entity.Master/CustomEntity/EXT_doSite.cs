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
    /// Do Of site
    /// </summary>
    public partial class doSite
    {
        public string SiteCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        [LanguageMapping]
        public string BuildingUsageName
        {
            get;
            set;
        }

        public Boolean? ValidateSiteData
        {
            get;
            set;
        }

        public string SiteNameENAndLC
        {
            get { return "(1) " + this.SiteNameEN + "<br>(2) " + this.SiteNameLC; }
        }

        public string AddressFullENAndLC
        {
            get { return "(1) " + this.AddressFullEN + "<br>(2) " + this.AddressFullLC; }
        }

        public string ToJson
        {
            get
            {
                return CommonUtil.CreateJsonString(this);
            }
        }
    }

    [MetadataType(typeof(ValidateSite_MetaData))]
    public class ValidateSite : doSite
    {
    }
}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    /// <summary>
    /// Do of validate site
    /// </summary>
    public class ValidateSite_MetaData
    {
        [NotNullOrEmpty]
        public string SiteNameEN { get; set; }
        //[NotNullOrEmpty]
        //public string SiteNameLC { get; set; }
        [NotNullOrEmpty]
        [CodeHasValue("BuildingUsageName")]
        public string BuildingUsageCode { get; set; }
        [NotNullOrEmpty]
        public string AddressEN { get; set; }
        [NotNullOrEmpty]
        public string SubDistrictEN { get; set; }
        [NotNullOrEmpty]
        public string ProvinceNameEN { get; set; }
        [NotNullOrEmpty]
        public string DistrictNameEN { get; set; }
        //[NotNullOrEmpty]
        //public string AddressLC { get; set; }
        //[NotNullOrEmpty]
        //public string SubDistrictLC { get; set; }
        //[NotNullOrEmpty]
        //public string ProvinceNameLC { get; set; }
        //[NotNullOrEmpty]
        //public string DistrictNameLC { get; set; }

    }
}