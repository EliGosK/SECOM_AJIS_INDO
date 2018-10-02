using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IAutoCompleteHandler
    {
        /// <summary>
        /// Get customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustAddress> GetCustAddress(string pchvnstrAutoComplete);

        /// <summary>
        /// Get Englist customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustAddressEN> GetCustAddressEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustAddressLC> GetCustAddressLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get alley in customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustAlley> GetCustAlley(string pchvnstrAutoComplete);

        /// <summary>
        /// Get English alley in customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustAlleyEN> GetCustAlleyEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local alley in customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustAlleyLC> GetCustAlleyLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get road in customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustRoad> GetCustRoad(string pchvnstrAutoComplete);

        /// <summary>
        /// Get English road in customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustRoadEN> GetCustRoadEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local road in customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustRoadLC> GetCustRoadLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get sub district in customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustSubDistrict> GetCustSubDistrict(string pchvnstrAutoComplete);

        /// <summary>
        /// Get English sub district in customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustSubDistrictEN> GetCustSubDistrictEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local sub district in customer's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doCustSubDistrictLC> GetCustSubDistrictLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get group name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doGroupNameDataList> GetGroupNameDataList(string pchvnstrAutoComplete);

        /// <summary>
        /// Get English billing's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doBillingNameEN> GetBillingNameEn(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local billing's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doGetBillingNameLC> GetBillingNameLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get English billing branch's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doBillingBranchNameEN> GetBillingBranchNameEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local billing branch's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doBillingBranchNameLC> GetBillingBranchNameLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get English billing address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doBillingAddressNameEN> GetBillingAddressNameEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local billing address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doBillingAddressNameLC> GetBillingAddressNameLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get product's name for auto complete textbox.
        /// </summary>
        /// <param name="pLiveSearch"></param>
        /// <returns></returns>
        List<dtProductName> GetProductName(string pLiveSearch);

        /// <summary>
        /// Get customer's name for auto complete textbox.
        /// </summary>
        /// <param name="pLiveSearch"></param>
        /// <returns></returns>
        List<dtCustName> GetCustName(string pLiveSearch);

        /// <summary>
        /// Get English customer's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtCustNameEN> GetCustNameEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local customer's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtCustNameLC> GetCustNameLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get site's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteName> GetSiteName(string pchvnstrAutoComplete);

        /// <summary>
        /// Get site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteAddress> GetSiteAddress(string pchvnstrAutoComplete);

        /// <summary>
        /// Get alley in site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteAlley> GetSiteAlley(string pchvnstrAutoComplete);

        /// <summary>
        /// Get road in site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteRoad> GetSiteRoad(string pchvnstrAutoComplete);

        /// <summary>
        /// Get sub district in site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteSubDistrict> GetSiteSubDistrict(string pchvnstrAutoComplete);

        /// <summary>
        /// Get billing client's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtBillingClientName> GetBillingClientName(string pchvnstrAutoComplete);

        /// <summary>
        /// Get billing client's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtBillingClientAddress> GetBillingClientAddress(string pchvnstrAutoComplete);

        /// <summary>
        /// Get English site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteAddressEN> GetSiteAddressEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteAddressLC> GetSiteAddressLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get English alley in site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteAlleyEN> GetSiteAlleyEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local alley in site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteAlleyLC> GetSiteAlleyLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get English site's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteNameEN> GetSiteNameEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local site's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteNameLC> GetSiteNameLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get English road in site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteRoadEN> GetSiteRoadEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local road in site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteRoadLC> GetSiteRoadLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get English sub district in site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteSubDistrictEN> GetSiteSubDistrictEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Get local sub district in site's address for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtSiteSubDistrictLC> GetSiteSubDistrictLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Get instrument maker for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtGetInstrumentMaker> GetInstrumentMaker(string pchnvStrAuto);

        /// <summary>
        /// Get instrument's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doInstrumentName> GetInstrumentName(string pchvInstrumentName);

        /// <summary>
        /// Get employee's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<dtEmployeeName> GetEmployeeName(string pchrnEmpFirstNameEN);

        /// <summary>
        /// Get permission group's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<string> GetPermissionGroupName(string empFirstNameEN);

        /// <summary>
        /// Get employee's code for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<string> GetEmployeeCode(string empNo);

        /// <summary>
        /// Get supplier's name for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<string> GetSupplierName(string strLiveSearch);

        /// <summary>
        /// Get Instrument's code for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<string> GetInstrumentCode(string InstrumentCode); //Add by Jutarat A. on 24032014

        /// <summary>
        /// Get Instrument's code for auto complete textbox.
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<string> GetInstrumentCodeAll(string InstrumentCode, string ExpansionTypeCode);      
  
        #region Master II
        /// <summary>
        /// Getting shelf name for case live search ()
        /// </summary>
        /// <param name="strLiveSearch"></param>
        /// <returns></returns>
        List<doShelfName> GetShelfName(string strLiveSearch);

        /// <summary>
        /// Getting subcontractor address for case auto-complete (sp_MA_GetSubcontractorAddress)
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doSubcontractorAddress> GetSubcontractorAddress(string pchvnstrAutoComplete);

        /// <summary>
        /// Getting subcontractor address for case auto-complete by english language (sp_MA_GetSubcontractorAddressEN)
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doSubcontractorAddressEN> GetSubcontractorAddressEN(string pchvnstrAutoComplete);
        
        /// <summary>
        /// Getting subcontractor address for case auto-complete by thai language (sp_MA_GetSubcontractorAddressLC)
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doSubcontractorAddressLC> GetSubcontractorAddressLC(string pchvnstrAutoComplete);

        /// <summary>
        /// Getting subcontractor name for auto-complete (sp_MA_GetSubcontractorName)
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doSubcontractorName> GetSubcontractorName(string pchvnstrAutoComplete);

        /// <summary>
        /// Getting subcontractor name for auto-complete by english language (sp_MA_GetSubcontractorNameEN)
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doSubcontractorNameEN> GetSubcontractorNameEN(string pchvnstrAutoComplete);

        /// <summary>
        /// Getting subcontractor name for auto-complete by thai language (sp_MA_GetSubcontractorNameLC)
        /// </summary>
        /// <param name="pchvnstrAutoComplete"></param>
        /// <returns></returns>
        List<doSubcontractorNameLC> GetSubcontractorNameLC(string pchvnstrAutoComplete);

        #endregion
        
    }
}
