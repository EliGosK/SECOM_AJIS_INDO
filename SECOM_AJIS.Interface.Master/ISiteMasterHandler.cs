using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Common;


namespace SECOM_AJIS.DataEntity.Master
{
    public interface ISiteMasterHandler
    {
        List<doGetTbm_Site> GetTbm_Site(string pchrCustCode);
        List<tbm_Site> InsertSite(string siteCode, string custCode, string siteNo, string siteNameEN, string siteNameLC, string sECOMContactPerson, string personInCharge, string phoneNo, string buildingUsageCode, string addressEN, string alleyEN, string roadEN, string subDistrictEN, string addressFullEN, string addressLC, string alleyLC, string roadLC, string subDistrictLC, string addressFullLC, string districtCode, string provinceCode, string zipCode, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy);
        
        List<dtSiteData> GetSiteDataForSearch(doSiteSearchCondition cond);
        List<CheckUpdateDate> CheckUpdateDate(string siteCode);
        List<tbm_Site> UpdateSite(string siteCode, string custCode, string siteNo, string siteNameEN, string siteNameLC, string sECOMContactPerson, string personInCharge, string phoneNo, string buildingUsageCode, string addressEN, string alleyEN, string roadEN, string subDistrictEN, string addressFullEN, string addressLC, string alleyLC, string roadLC, string subDistrictLC, string addressFullLC, string districtCode, string provinceCode, string zipCode, Nullable<System.DateTime> updateDate, string updateBy);
        //int DeleteSite(string pchnvSiteCode);
        bool CheckExistSiteData(string siteCode, string custCode);

        /// <summary>
        /// Get site and mapping language.
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        List<doSite> GetSite(string siteCode, string custCode);

        /// <summary>
        /// Validate site data.<br />
        /// - Set building usage.<br />
        /// - Set province.<br />
        /// - Set district.<br />
        /// - Set address full name.
        /// </summary>
        /// <param name="doSite"></param>
        /// <returns></returns>
        void ValidateSiteData(doSite doSite);

        bool CheckDuplicateSiteData(doSite doSite);

        /// <summary>
        /// Insert site and write transaction log.
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        List<tbm_Site> InsertSite(doSite site);

        /// <summary>
        /// Update site data.<br />
        /// - Check require field.<br />
        /// - Check update date.<br />
        /// - Update site.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        List<tbm_Site> UpdateSite(doSite site);

        /// <summary>
        /// Delete site data.<br />
        /// - Check require field.<br />
        /// - Check update date.<br />
        /// - Delete site.<br />
        /// - Write transaction log.
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        List<tbm_Site> DeleteSite(doSite site);
    }
}
