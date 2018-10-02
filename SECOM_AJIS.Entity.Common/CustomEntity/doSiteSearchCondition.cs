using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SECOM_AJIS.DataEntity.Common
{
    [Serializable]
    public class doSiteSearchCondition
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustStatus { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string BuildingUsageCode { get; set; }
        public string Address { get; set; }
        public string Alley { get; set; }
        public string Road { get; set; }
        public string SubDistrict { get; set; }
        public string ProvinceCode { get; set; }
        public string DistrictCode { get; set; }
        public string ZipCode { get; set; }
        public string chkNewCustomer { get; set; }
        public string chkExistingCustomer { get; set; }
        public bool isSearchByRealCust { get; set; }

        public int Counter { set; get; }
    }
}
