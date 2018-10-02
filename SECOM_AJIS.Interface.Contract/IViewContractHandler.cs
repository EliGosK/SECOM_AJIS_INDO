using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Contract 
{
    public interface IViewContractHandler
    {
        /// <summary>
        /// Get customer list for view customer group from rental contract (contract target)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtCustomerListGrp> GetCustomerListForViewCustGrp_CT_Rental(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get customer list for view customer group from sale contract (Purchaser)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtCustomerListGrp> GetCustomerListForViewCustGrp_CT_Sale(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get customer list for view customer group from rental contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtCustomerListGrp> GetCustomerListForViewCustGrp_R_Rental(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get customer list for view customer group from sale contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtCustomerListGrp> GetCustomerListForViewCustGrp_R_Sale(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get contract list for view customer group from rental contract (contract target)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtContractListGrp> GetContractListForViewCustGrp_CT_Rental(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get contract list for view customer group from sale contract (Purchaser)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtContractListGrp> GetContractListForViewCustGrp_CT_Sale(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get contract list for view customer group from rental contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtContractListGrp> GetContractListForViewCustGrp_R_Rental(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get contract list for view customer group from sale contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtContractListGrp> GetContractListForViewCustGrp_R_Sale(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get site list for view customer group from rental contract (contract target)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtsiteListGrp> GetSiteListForViewCustGrp_CT_Rental(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get site list for view customer group from sale contract (Purchaser)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtsiteListGrp> GetSiteListForViewCustGrp_CT_Sale(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get site list for view customer group from rental contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtsiteListGrp> GetSiteListForViewCustGrp_R_Rental(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get site list for view customer group from rental contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        List<dtsiteListGrp> GetSiteListForViewCustGrp_R_Sale(string strGroupCode, string strCONTRACT_PREFIX);

        /// <summary>
        /// Get site list for view customer group
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCustRoleType"></param>
        /// <param name="strProductTypeCode"></param>
        /// <returns></returns>
        List<dtsiteListGrp> GetSiteListForViewCustGrp(string strGroupCode, string strCustRoleType, string strProductTypeCode);

        /// <summary>
        /// Get customer list for view customer group
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCustRoleType"></param>
        /// <param name="strProductTypeCode"></param>
        /// <returns></returns>
        List<dtCustomerListGrp> GetCustomerListForViewCustGrp(string strGroupCode, string strCustRoleType, string strProductTypeCode);
        //List<dtGroupSummary> GetGroupSummaryForViewCustGrp(string strGroupCode);

        /// <summary>
        /// Get group summary for view customer group data
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <returns></returns>
        List<dtGroupSummaryForShow> GetGroupSummaryForViewCustGrpData(string strGroupCode);

        /// <summary>
        /// Get contract data for search
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtContractData> GetContractDataForSearch(doContractSearchCondition cond);

        /// <summary>
        /// Getting branch name data for case live search
        /// </summary>
        /// <param name="pchvLiveSearch"></param>
        /// <returns></returns>
        List<dtContractBranchName> GetContractBranchName(string pchvLiveSearch);

        /// <summary>
        /// Get related contract list
        /// </summary>
        /// <param name="pchrRelationType"></param>
        /// <param name="pchvstrContractCode"></param>
        /// <param name="pchrOCC"></param>
        /// <returns></returns>
        List<dtRelatedContract> GetRelatedContractList(string pchrRelationType, string pchvstrContractCode, string pchrOCC);

        /// <summary>
        /// Get contract same site list
        /// </summary>
        /// <param name="pSiteCode"></param>
        /// <param name="pContractCode"></param>
        /// <returns></returns>
        List<dtContractsSameSite> GetContractsSameSiteList(string pSiteCode, string pContractCode = null);

        /// <summary>
        /// Get contract document data list
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <param name="pContractOfficeCode_List"></param>
        /// <param name="pOperationOfficeCode_List"></param>
        /// <returns></returns>
        List<dtContractDocument> GetContractDocDataListForView(string pContractCode, string pOCC, string pContractOfficeCode_List, string pOperationOfficeCode_List);

        /// <summary>
        /// Get rental history digest list
        /// </summary>
        /// <param name="pchvContractCode"></param>
        /// <param name="dtSelChangeType"></param>
        /// <param name="dtSelIncidentARtype"></param>
        /// <returns></returns>
        List<dtRentalHistoryDigest> GetRentalHistoryDigestList(string pchvContractCode, List<dtChangeType> dtSelChangeType, List<dtIncidentARType> dtSelIncidentARtype);

        /// <summary>
        /// Get sale historu digest list
        /// </summary>
        /// <param name="pchvContractCode"></param>
        /// <param name="dtSelChangeType"></param>
        /// <param name="dtSelIncidentARtype"></param>
        /// <returns></returns>
        List<dtSaleHistoryDigest> GetSaleHistoryDigestList(string pchvContractCode, List<dtChangeType> dtSelChangeType, List<dtIncidentARType> dtSelIncidentARtype);

        /// <summary>
        /// Get site list for customer info
        /// </summary>
        /// <param name="pchvCustomerCode"></param>
        /// <param name="pchrCustomerRole"></param>
        /// <param name="pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET"></param>
        /// <param name="pchrC_CUST_ROLE_TYPE_REAL_CUST"></param>
        /// <param name="pchrC_CUST_ROLE_TYPE_PURCHASER"></param>
        /// <param name="pbitC_FLAG_ON"></param>
        /// <param name="pchrC_SERVICE_TYPE_RENTAL"></param>
        /// <param name="pchrC_RENTAL_CHANGE_TYPE_END_CONTRACT"></param>
        /// <param name="pchrC_RENTAL_CHANGE_TYPE_CANCEL"></param>
        /// <param name="pchrC_SERVICE_TYPE_SALE"></param>
        /// <param name="pchrC_CONTRACT_STATUS_CANCEL"></param>
        /// <param name="pchrC_CONTRACT_STATUS_END"></param>
        /// <param name="pchrC_CONTRACT_STATUS_BEF_START"></param>
        /// <param name="c_CONTRACT_STATUS_FIXED_CANCEL"></param>
        /// <returns></returns>
        List<dtSiteList> GetSiteListForCustInfo(string pchvCustomerCode, string pchrCustomerRole, string pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET, string pchrC_CUST_ROLE_TYPE_REAL_CUST, string pchrC_CUST_ROLE_TYPE_PURCHASER, Nullable<bool> pbitC_FLAG_ON, string pchrC_SERVICE_TYPE_RENTAL, string pchrC_RENTAL_CHANGE_TYPE_END_CONTRACT, string pchrC_RENTAL_CHANGE_TYPE_CANCEL, string pchrC_SERVICE_TYPE_SALE, string pchrC_CONTRACT_STATUS_CANCEL, string pchrC_CONTRACT_STATUS_END, string pchrC_CONTRACT_STATUS_BEF_START, string c_CONTRACT_STATUS_FIXED_CANCEL);

        /// <summary>
        /// Get customer list for search info
        /// </summary>
        /// <param name="pchrCustomerCode"></param>
        /// <param name="pchrRoleTypeContractTarget"></param>
        /// <param name="pchrRoleTypePurchaser"></param>
        /// <param name="pchrRoleTypeRealCustomer"></param>
        /// <param name="pchrGroupCode"></param>
        /// <param name="pchrnCustomerName"></param>
        /// <param name="pchrnGroupName"></param>
        /// <param name="pchrCustomerStatus"></param>
        /// <param name="pchrCustomerTypeCode"></param>
        /// <param name="pchrCompanyTypeCode"></param>
        /// <param name="pchrnIDNo"></param>
        /// <param name="pchrRegionCode"></param>
        /// <param name="pchrBusinessTypeCode"></param>
        /// <param name="pchrnCust_Address"></param>
        /// <param name="pchrnCust_Alley"></param>
        /// <param name="pchrnCust_Road"></param>
        /// <param name="pchrnCust_SubDistrict"></param>
        /// <param name="pchrCust_ProvinceCode"></param>
        /// <param name="pchrCust_DistrictCode"></param>
        /// <param name="pchrCust_ZipCode"></param>
        /// <param name="pchrnCust_PhoneNo"></param>
        /// <param name="pchrnCust_FaxNo"></param>
        /// <param name="pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET"></param>
        /// <param name="pchrC_CUST_ROLE_TYPE_REAL_CUST"></param>
        /// <param name="pchrC_CUST_ROLE_TYPE_PURCHASER"></param>
        /// <param name="pbitC_FLAG_ON"></param>
        /// <param name="pchrC_SERVICE_TYPE_RENTAL"></param>
        /// <param name="pchrC_SERVICE_TYPE_SALE"></param>
        /// <param name="pchrC_CONTRACT_STATUS_BEF_START"></param>
        /// <param name="c_CUST_TYPE_JURISTIC"></param>
        /// <returns></returns>
        List<dtCustomerList> GetCustomerListForSearchInfo(string pchrCustomerCode, string pchrRoleTypeContractTarget, string pchrRoleTypePurchaser, string pchrRoleTypeRealCustomer, string pchrGroupCode, string pchrnCustomerName, string pchrnGroupName, string pchrCustomerStatus, string pchrCustomerTypeCode, string pchrCompanyTypeCode, string pchrnIDNo, string pchrRegionCode, string pchrBusinessTypeCode, string pchrnCust_Address, string pchrnCust_Alley, string pchrnCust_Road, string pchrnCust_SubDistrict, string pchrCust_ProvinceCode, string pchrCust_DistrictCode, string pchrCust_ZipCode, string pchrnCust_PhoneNo, string pchrnCust_FaxNo, string pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET, string pchrC_CUST_ROLE_TYPE_REAL_CUST, string pchrC_CUST_ROLE_TYPE_PURCHASER, Nullable<bool> pbitC_FLAG_ON, string pchrC_SERVICE_TYPE_RENTAL, string pchrC_SERVICE_TYPE_SALE, string pchrC_CONTRACT_STATUS_BEF_START, string c_CUST_TYPE_JURISTIC);

        /// <summary>
        /// Get contract list for seatch info
        /// </summary>
        /// <param name="pchrRoleTypeContractTarget"></param>
        /// <param name="pchrRoleTypePurchaser"></param>
        /// <param name="pchrRoleTypeRealCustomer"></param>
        /// <param name="pchrServiceTypeCode"></param>
        /// <param name="pchrCustomerCode"></param>
        /// <param name="pchrGroupCode"></param>
        /// <param name="pchrSiteCode"></param>
        /// <param name="pchvContractCode"></param>
        /// <param name="pchvUserCode"></param>
        /// <param name="pchvPlanCode"></param>
        /// <param name="pchvProjectCode"></param>
        /// <param name="pchrnCustomerName"></param>
        /// <param name="pchrnBranchName"></param>
        /// <param name="pchrnGroupName"></param>
        /// <param name="pchrCustomerStatus"></param>
        /// <param name="pchrCustomerTypeCode"></param>
        /// <param name="pchrCompanyTypeCode"></param>
        /// <param name="pchrnIDNo"></param>
        /// <param name="pchrRegionCode"></param>
        /// <param name="pchrBusinessTypeCode"></param>
        /// <param name="pchrnCust_Address"></param>
        /// <param name="pchrnCust_Alley"></param>
        /// <param name="pchrnCust_Road"></param>
        /// <param name="pchrnCust_SubDistrict"></param>
        /// <param name="pchrCust_ProvinceCode"></param>
        /// <param name="pchrCust_DistrictCode"></param>
        /// <param name="pchrCust_ZipCode"></param>
        /// <param name="pchrnCust_PhoneNo"></param>
        /// <param name="pchrnCust_FaxNo"></param>
        /// <param name="pchrnSiteName"></param>
        /// <param name="pchrnSite_Address"></param>
        /// <param name="pchrnSite_Alley"></param>
        /// <param name="pchrnSite_Road"></param>
        /// <param name="pchrnSite_SubDistrict"></param>
        /// <param name="pchrSite_ProvinceCode"></param>
        /// <param name="pchrSite_DistrictCode"></param>
        /// <param name="pchrSite_ZipCode"></param>
        /// <param name="pchrnSite_PhoneNo"></param>
        /// <param name="pdtmOperationDate_From"></param>
        /// <param name="pdtmOperationDate_To"></param>
        /// <param name="pdtmCustAcceptDate_From"></param>
        /// <param name="pdtmCustAcceptDate_To"></param>
        /// <param name="pdtmInstallationCompleteDate_From"></param>
        /// <param name="pdtmInstallationCompleteDate_To"></param>
        /// <param name="pchvContractOfficeCode"></param>
        /// <param name="pchvdsTransDataOfficeCode"></param>
        /// <param name="pchvOperationOfficeCode"></param>
        /// <param name="pchvSalesmanEmpNo1"></param>
        /// <param name="pchvSalesmanEmpName1"></param>
        /// <param name="pchrProductCode"></param>
        /// <param name="pchrChangeTypeCode"></param>
        /// <param name="pchrProcessManageStatusCode"></param>
        /// <param name="pchrStartTypeCode"></param>
        /// <param name="pchvC_RENTAL_CHANGE_TYPE"></param>
        /// <param name="pchvC_SALE_CHANGE_TYPE"></param>
        /// <param name="pchvC_SALE_PROC_MANAGE_STATUS"></param>
        /// <param name="pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET"></param>
        /// <param name="pchrC_CUST_ROLE_TYPE_REAL_CUST"></param>
        /// <param name="pchrC_CUST_ROLE_TYPE_PURCHASER"></param>
        /// <param name="pbitC_FLAG_ON"></param>
        /// <param name="pchrC_SERVICE_TYPE_RENTAL"></param>
        /// <param name="pchrC_SERVICE_TYPE_SALE"></param>
        /// <param name="pchrC_CONTRACT_STATUS_BEF_START"></param>
        /// <param name="pchrC_CONTRACT_STATUS_CANCEL"></param>
        /// <param name="pchrC_CONTRACT_STATUS_END"></param>
        /// <param name="c_SALE_CHANGE_TYPE_NEW_SALE"></param>
        /// <param name="c_CUST_TYPE_JURISTIC"></param>
        /// <param name="cancelDateFrom"></param>
        /// <param name="cancelDateTo"></param>
        /// <param name="stopDateFrom"></param>
        /// <param name="stopDateTo"></param>
        /// <returns></returns>
        List<dtContractList> GetContractListForSearchInfo(string pchrRoleTypeContractTarget, string pchrRoleTypePurchaser, string pchrRoleTypeRealCustomer, string pchrServiceTypeCode, string pchrCustomerCode, string pchrGroupCode, string pchrSiteCode, string pchvContractCode, string pchvUserCode, string pchvPlanCode, string pchvProjectCode, string pchrnCustomerName, string pchrnBranchName, string pchrnGroupName, string pchrCustomerStatus, string pchrCustomerTypeCode, string pchrCompanyTypeCode, string pchrnIDNo, string pchrRegionCode, string pchrBusinessTypeCode, string pchrnCust_Address, string pchrnCust_Alley, string pchrnCust_Road, string pchrnCust_SubDistrict, string pchrCust_ProvinceCode, string pchrCust_DistrictCode, string pchrCust_ZipCode, string pchrnCust_PhoneNo, string pchrnCust_FaxNo, string pchrnSiteName, string pchrnSite_Address, string pchrnSite_Alley, string pchrnSite_Road, string pchrnSite_SubDistrict, string pchrSite_ProvinceCode, string pchrSite_DistrictCode, string pchrSite_ZipCode, string pchrnSite_PhoneNo, Nullable<System.DateTime> pdtmOperationDate_From, Nullable<System.DateTime> pdtmOperationDate_To, Nullable<System.DateTime> pdtmCustAcceptDate_From, Nullable<System.DateTime> pdtmCustAcceptDate_To, Nullable<System.DateTime> pdtmInstallationCompleteDate_From, Nullable<System.DateTime> pdtmInstallationCompleteDate_To, string pchvContractOfficeCode, string pchvdsTransDataOfficeCode, string pchvOperationOfficeCode, string pchvSalesmanEmpNo1, string pchvSalesmanEmpName1, string pchrProductCode, string pchrChangeTypeCode, string pchrProcessManageStatusCode, string pchrStartTypeCode, string pchvC_RENTAL_CHANGE_TYPE, string pchvC_SALE_CHANGE_TYPE, string pchvC_SALE_PROC_MANAGE_STATUS, string pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET, string pchrC_CUST_ROLE_TYPE_REAL_CUST, string pchrC_CUST_ROLE_TYPE_PURCHASER, Nullable<bool> pbitC_FLAG_ON, string pchrC_SERVICE_TYPE_RENTAL, string pchrC_SERVICE_TYPE_SALE, string pchrC_CONTRACT_STATUS_BEF_START, string pchrC_CONTRACT_STATUS_CANCEL, string pchrC_CONTRACT_STATUS_END, string c_SALE_CHANGE_TYPE_NEW_SALE, string c_CUST_TYPE_JURISTIC, Nullable<System.DateTime> stopDateFrom, Nullable<System.DateTime> stopDateTo, Nullable<System.DateTime> cancelDateFrom, Nullable<System.DateTime> cancelDateTo);

        /// <summary>
        /// Get group list for search customer group
        /// </summary>
        /// <param name="pchvGroupCode"></param>
        /// <param name="pchrnGroupName"></param>
        /// <param name="pchvOfficeCode"></param>
        /// <param name="pchvEmpNo"></param>
        /// <param name="pintNumOfCustFrom"></param>
        /// <param name="pintNumOfCustTo"></param>
        /// <param name="pintNumOfSiteFrom"></param>
        /// <param name="pintNumOfSiteTo"></param>
        /// <param name="c_CONTRACT_STATUS_BEF_START"></param>
        /// <param name="c_CONTRACT_STATUS_CANCEL"></param>
        /// <param name="c_CONTRACT_STATUS_AFTER_START"></param>
        /// <param name="c_FLAG_ON"></param>
        /// <returns></returns>
        List<dtGroupList> GetGroupListForSearchCustGrp(string pchvGroupCode, string pchrnGroupName, string pchvOfficeCode, string pchvEmpNo, Nullable<int> pintNumOfCustFrom, Nullable<int> pintNumOfCustTo, Nullable<int> pintNumOfSiteFrom, Nullable<int> pintNumOfSiteTo, string c_CONTRACT_STATUS_BEF_START, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_AFTER_START, Nullable<bool> c_FLAG_ON);

        /// <summary>
        /// Get changed customer history list
        /// </summary>
        /// <param name="pchvContractCode"></param>
        /// <param name="pchrOCC"></param>
        /// <param name="pchrCSCustCode"></param>
        /// <param name="pchrRCCustCode"></param>
        /// <param name="pchrSiteCode"></param>
        /// <param name="pchvC_CONTRACT_SIGNER_TYPE"></param>
        /// <param name="pchvC_CHANGE_NAME_REASON_TYPE"></param>
        /// <returns></returns>
        List<dtChangedCustHistList> GetChangedCustHistList(string pchvContractCode, string pchrOCC, string pchrCSCustCode, string pchrRCCustCode, string pchrSiteCode, string pchvC_CONTRACT_SIGNER_TYPE, string pchvC_CHANGE_NAME_REASON_TYPE);

        /// <summary>
        /// Get changed customer history list with change name reason type
        /// </summary>
        /// <param name="pchvContractCode"></param>
        /// <param name="pintSequenceNo"></param>
        /// <param name="pchvC_CONTRACT_SIGNER_TYPE"></param>
        /// <param name="pchvC_CUST_STATUS"></param>
        /// <param name="pchvC_CUST_TYPE"></param>
        /// <param name="pchvC_FINANCIAL_MARKET_TYPE"></param>
        /// <param name="pchvC_CHANGE_NAME_REASON_TYPE"></param>
        /// <returns></returns>
        List<dtChangedCustHistDetail> GetChangedCustHistDetail(string pchvContractCode, Nullable<int> pintSequenceNo, string pchvC_CONTRACT_SIGNER_TYPE, string pchvC_CUST_STATUS, string pchvC_CUST_TYPE, string pchvC_FINANCIAL_MARKET_TYPE, string pchvC_CHANGE_NAME_REASON_TYPE);

        /// <summary>
        /// Get changed customer history list by condition
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtChangedCustHistList2> GetChangedCustHistList2(doContractInfoCondition cond);

        /// <summary>
        /// Get maintenance checkup result list
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pMATargetContractCode"></param>
        /// <param name="pProductCode"></param>
        /// <returns></returns>
        List<dtMaintCheckUpResultList> GetMaintCheckUpResultList(string pContractCode, string pMATargetContractCode, string pProductCode);

        /// <summary>
        /// Get contract list for view site
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <returns></returns>
        List<dtContractsSameSite> GetContractsListForViewSite(string SiteCode);

        //List<dtContractTargetInfoByRelated> GetContractTargetInfoByRelated(string pRelatedContractCode, string pRelationType, string pRelatedOCC, string pC_PROD_TYPE_SALE, string pC_PROD_TYPE_AL, string pC_PROD_TYPE_ONLINE, string pC_PROD_TYPE_RENTAL_SALE, string pRelatedProductTypeCode, string pC_RELATION_TYPE_MA, string pC_RELATION_TYPE_SALE);
        //List<dtMaintContractTargetInfoByRelated> GetMaintContractTargetInfoByRelated(string pRelatedContractCode, string pC_MA_TARGET_PROD_TYPE, string pC_MA_TYPE, string pC_MA_FEE_TYPE, string pC_RELATION_TYPE_MA, string pRelatedOCC, string pC_PROD_TYPE_SALE, string pC_PROD_TYPE_AL, string pC_PROD_TYPE_ONLINE, string pC_PROD_TYPE_RENTAL_SALE, string pRelatedProductTypeCode);

        /// <summary>
        /// Get contract target info by related
        /// </summary>
        /// <param name="pRelatedContractCode"></param>
        /// <param name="pRelationType"></param>
        /// <param name="pRelatedOCC"></param>
        /// <param name="pRelatedProductTypeCode"></param>
        /// <returns></returns>
        List<dtContractTargetInfoByRelated> GetContractTargetInfoByRelated(string pRelatedContractCode, string pRelationType, string pRelatedOCC, string pRelatedProductTypeCode);

        /// <summary>
        /// Get maintenance target info by related
        /// </summary>
        /// <param name="pRelatedContractCode"></param>
        /// <param name="pRelatedOCC"></param>
        /// <param name="pRelatedProductTypeCode"></param>
        /// <returns></returns>
        List<dtMaintContractTargetInfoByRelated> GetMaintContractTargetInfoByRelated(string pRelatedContractCode, string pRelatedOCC, string pRelatedProductTypeCode);

        /// <summary>
        /// Get sale instrument detail list for view
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <param name="pInstrumentTypeCode"></param>
        /// <returns></returns>
        List<dtSaleInstruDetailListForView> GetSaleInstruDetailListForView(string pContractCode, string pOCC, string pInstrumentTypeCode = null);

        /// <summary>
        /// Get contract signer type
        /// </summary>
        /// <param name="c_CONTRACT_SIGNER_TYPE"></param>
        /// <param name="contractCode"></param>
        /// <param name="oCC"></param>
        /// <returns></returns>
        List<dtContractSignerType> GetContractSignerType(string c_CONTRACT_SIGNER_TYPE, string contractCode, string oCC);

        /// <summary>
        /// Getting site information
        /// </summary>
        /// <param name="strSiteCode"></param>
        /// <returns></returns>
        dsSiteInfo GetSiteInfo(string strSiteCode);

        /// <summary>
        /// Getting project information
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        dsProjectInfo GetProjectInfo(string strProjectCode);

        /// <summary>
        /// Get sale contract data to show on flow menu screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        List<doSaleContractDataForFlowMenu> GetSaleContractDataForFlowMenu(string contractCode);

        /// <summary>
        /// Get rental contract data to show on flow menu screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        List<doRentalContractDataForFlowMenu> GetRentalContractDataForFlowMenu(string contractCode);
        /// <summary>
        /// Get Contract / Project information
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<doGetContractProjectInfo> GetContractProjectInfo(string strContractCode);
    }
}
