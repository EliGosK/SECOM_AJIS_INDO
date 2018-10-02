using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    public partial class tbm_Site
    {
    }
    [MetadataType(typeof(tbm_SiteCondition_MetaData))]
    public class tbm_SiteCondition : tbm_Site
    {
    }
}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    public class tbm_SiteCondition_MetaData
    {
        [NotNullOrEmpty]
        public string SiteCode { get; set; }
        [NotNullOrEmpty]
        public string CustCode { get; set; }
        [NotNullOrEmpty]
        public string SiteNameEN { get; set; }
        //[NotNullOrEmpty]
        //public string SiteNameLC { get; set; }
        [NotNullOrEmpty]
        public string BuildingUsageCode { get; set; }
        [NotNullOrEmpty]
        public string AddressEN { get; set; }
        //[NotNullOrEmpty]
        //public string RoadEN { get; set; }
        [NotNullOrEmpty]
        public string SubDistrictEN { get; set; }
        //[NotNullOrEmpty]
        //public string AddressLC { get; set; }
        //[NotNullOrEmpty]
        //public string RoadLC { get; set; }
        //[NotNullOrEmpty]
        //public string SubDistrictLC { get; set; }
        [NotNullOrEmpty]
        public string ProvinceCode { get; set; }
        [NotNullOrEmpty]
        public string DistrictCode { get; set; }
        [NotNullOrEmpty]
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
