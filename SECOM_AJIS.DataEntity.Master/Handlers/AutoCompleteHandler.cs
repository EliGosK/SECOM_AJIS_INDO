using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.DataEntity.Master
{
    public class AutoCompleteHandler : BizMADataEntities, IAutoCompleteHandler
    {
        
        /// <summary>
        /// Get province data by language
        /// </summary>
        /// <returns></returns>
        public List<tbm_Province> GetTbm_ProvinceByLanguage()
        {
            try
            {
                List<tbm_Province> lst = CommonUtil.ConvertObjectbyLanguage<tbm_Province, tbm_Province>(
                    this.GetTbm_Province(),
                    "ProvinceName");

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public override List<doCustAddress> GetCustAddress(string pchvnstrAutoComplete)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pchvnstrAutoComplete);
        //        return base.GetCustAddress(pchvnstrAutoComplete);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public override List<doGroupNameDataList> GetGroupNameDataList(string pchvnstrAutoComplete)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pchvnstrAutoComplete);
        //        return base.GetGroupNameDataList(pchvnstrAutoComplete);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public override List<dtCustName> GetCustName(string pLiveSearch)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pLiveSearch);
        //        return base.GetCustName(pLiveSearch);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public override List<doCustAlley> GetCustAlley(string pchvnstrAutoComplete)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pchvnstrAutoComplete);
        //        return base.GetCustAlley(pchvnstrAutoComplete);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public override List<doCustRoad> GetCustRoad(string pchvnstrAutoComplete)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pchvnstrAutoComplete);
        //        return base.GetCustRoad(pchvnstrAutoComplete);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public override List<doCustSubDistrict> GetCustSubDistrict(string pchvnstrAutoComplete)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pchvnstrAutoComplete);
        //        return base.GetCustSubDistrict(pchvnstrAutoComplete);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public override List<dtSiteName> GetSiteName(string pchvnstrAutoComplete)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pchvnstrAutoComplete);
        //        return base.GetSiteName(pchvnstrAutoComplete);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public override List<dtSiteAddress> GetSiteAddress(string pchvnstrAutoComplete)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pchvnstrAutoComplete);
        //        return base.GetSiteAddress(pchvnstrAutoComplete);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public override List<dtSiteAlley> GetSiteAlley(string pchvnstrAutoComplete)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pchvnstrAutoComplete);
        //        return base.GetSiteAlley(pchvnstrAutoComplete);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}



        //public override List<dtSiteSubDistrict> GetSiteSubDistrict(string pchvnstrAutoComplete)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pchvnstrAutoComplete);
        //        return base.GetSiteSubDistrict(pchvnstrAutoComplete);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public override List<dtBillingClientName> GetBillingClientName(string pchvnstrAutoComplete)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pchvnstrAutoComplete);
        //        return base.GetBillingClientName(pchvnstrAutoComplete);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //public override List<dtBillingClientAddress> GetBillingClientAddress(string pchvnstrAutoComplete)
        //{
        //    try
        //    {
        //        ApplicationErrorException.CheckMandatoryField(pchvnstrAutoComplete);
        //        return base.GetBillingClientAddress(pchvnstrAutoComplete);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
    }
}
