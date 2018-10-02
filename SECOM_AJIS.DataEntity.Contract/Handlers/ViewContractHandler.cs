using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class ViewContractHandler : BizCTDataEntities, IViewContractHandler
    {
        /// <summary>
        /// Get contract data for search
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<dtContractData> GetContractDataForSearch(doContractSearchCondition cond)
        {
            try
            {
                return base.GetContractDataForSearch(cond.CustomerCode,
                                                      cond.CustomerName,
                                                      cond.BranchName,
                                                      cond.Address,
                                                      cond.Alley,
                                                      cond.Road,
                                                      cond.SubDistrict,
                                                      cond.ProvinceCode,
                                                      cond.DistrictCode,
                                                      cond.ZipCode,
                                                      SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE,
                                                      SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL,
                                                      SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Get rental history digest list
        /// </summary>
        /// <param name="pchvContractCode"></param>
        /// <param name="dtSelChangeType"></param>
        /// <param name="dtSelIncidentARtype"></param>
        /// <returns></returns>
        public List<dtRentalHistoryDigest> GetRentalHistoryDigestList(string pchvContractCode, List<dtChangeType> dtSelChangeType, List<dtIncidentARType> dtSelIncidentARtype)
        {
            string xmlChangType = SECOM_AJIS.Common.Util.CommonUtil.ConvertToXml_Store<dtChangeType>(dtSelChangeType);
            string xmlIncidentARType = SECOM_AJIS.Common.Util.CommonUtil.ConvertToXml_Store<dtIncidentARType>(dtSelIncidentARtype);
            return base.GetRentalHistoryDigestList(pchvContractCode, xmlChangType, xmlIncidentARType, MiscType.C_RENTAL_CHANGE_TYPE, MiscType.C_INCIDENT_TYPE, MiscType.C_AR_TYPE, MiscType.C_DOC_AUDIT_RESULT, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
        }

        /// <summary>
        /// Get sale historu digest list
        /// </summary>
        /// <param name="pchvContractCode"></param>
        /// <param name="dtSelChangeType"></param>
        /// <param name="dtSelIncidentARtype"></param>
        /// <returns></returns>
        public List<dtSaleHistoryDigest> GetSaleHistoryDigestList(string pchvContractCode, List<dtChangeType> dtSelChangeType, List<dtIncidentARType> dtSelIncidentARtype)
        {
            return base.GetSaleHistoryDigestList(pchvContractCode, SECOM_AJIS.Common.Util.CommonUtil.ConvertToXml_Store<dtChangeType>(dtSelChangeType), SECOM_AJIS.Common.Util.CommonUtil.ConvertToXml_Store<dtIncidentARType>(dtSelIncidentARtype), MiscType.C_SALE_CHANGE_TYPE, MiscType.C_INCIDENT_TYPE, MiscType.C_AR_TYPE, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
        }

        /// <summary>
        /// Get changed customer history list by condition
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<dtChangedCustHistList2> GetChangedCustHistList2(doContractInfoCondition cond)
        {
            return base.GetChangedCustHistList2(cond.ContractCode, cond.OCC, cond.CSCustCode, cond.RCCustCode, cond.SiteCode);
        }

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
        /// <returns></returns>
        //public List<dtContractList> GetContractListForSearchInfo(string pchrCustomerRole, string pchrServiceTypeCode, string pchrCustomerCode, string pchrGroupCode, string pchrSiteCode, string pchvContractCode, string pchvUserCode, string pchvPlanCode, string pchvProjectCode, string pchrnCustomerName, string pchrnBranchName, string pchrnGroupName, string pchrCustomerStatus, string pchrCustomerTypeCode, string pchrCompanyTypeCode, string pchrnIDNo, string pchrRegionCode, string pchrBusinessTypeCode, string pchrnCust_Address, string pchrnCust_Alley, string pchrnCust_Road, string pchrnCust_SubDistrict, string pchrCust_ProvinceCode, string pchrCust_DistrictCode, string pchrCust_ZipCode, string pchrnCust_PhoneNo, string pchrnCust_FaxNo, string pchrnSiteName, string pchrnSite_Address, string pchrnSite_Alley, string pchrnSite_Road, string pchrnSite_SubDistrict, string pchrSite_ProvinceCode, string pchrSite_DistrictCode, string pchrSite_ZipCode, string pchrnSite_PhoneNo, DateTime? pdtmOperationDate_From, DateTime? pdtmOperationDate_To, DateTime? pdtmCustAcceptDate_From, DateTime? pdtmCustAcceptDate_To, DateTime? pdtmInstallationCompleteDate_From, DateTime? pdtmInstallationCompleteDate_To, string pchvContractOfficeCode, string pchvdsTransDataOfficeCode, string pchvOperationOfficeCode, string pchvSalesmanEmpNo1, string pchvSalesmanEmpName1, string pchrProductCode, string pchrChangeTypeCode, string pchrProcessManageStatusCode, string pchrStartTypeCode, string pchvC_RENTAL_CHANGE_TYPE, string pchvC_SALE_CHANGE_TYPE, string pchvC_SALE_PROC_MANAGE_STATUS, string pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET, string pchrC_CUST_ROLE_TYPE_REAL_CUST, string pchrC_CUST_ROLE_TYPE_PURCHASER, bool? pbitC_FLAG_ON, string pchrC_SERVICE_TYPE_RENTAL, string pchrC_SERVICE_TYPE_SALE)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Get contract list for view site
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <returns></returns>
        public List<dtContractsSameSite> GetContractsListForViewSite(string SiteCode)
        {
            return base.GetContractsListForViewSite(SiteCode, MiscType.C_RENTAL_CHANGE_TYPE, MiscType.C_SALE_CHANGE_TYPE, ServiceType.C_SERVICE_TYPE_RENTAL, SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
        }

        public List<dtRelatedContract> GetRelatedContractList(string pchrRelationType, string pchvstrContractCode, string pchrOCC)
        {
            return base.GetRelatedContractList(pchrRelationType, pchvstrContractCode, pchrOCC, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
        }

        /// <summary>
        /// Get contract same site list
        /// </summary>
        /// <param name="pSiteCode"></param>
        /// <param name="pContractCode"></param>
        /// <returns></returns>
        public List<dtContractsSameSite> GetContractsSameSiteList(string SiteCode, string ContractCode)
        {
            //Check mandatory data SiteCode
            doContractSite contractSite = new doContractSite();
            contractSite.SiteCode = SiteCode;
            ApplicationErrorException.CheckMandatoryField(contractSite);

            return base.GetContractsSameSiteList(SiteCode, MiscType.C_RENTAL_CHANGE_TYPE, MiscType.C_SALE_CHANGE_TYPE, ServiceType.C_SERVICE_TYPE_RENTAL, ContractCode);
        }

        /// <summary>
        /// Get customer list for view customer group from rental contract (contract target)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtCustomerListGrp> GetCustomerListForViewCustGrp_CT_Rental(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetCustomerListForViewCustGrp_CT_Rental(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get customer list for view customer group from sale contract (Purchaser)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtCustomerListGrp> GetCustomerListForViewCustGrp_CT_Sale(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetCustomerListForViewCustGrp_CT_Sale(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get customer list for view customer group from rental contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtCustomerListGrp> GetCustomerListForViewCustGrp_R_Rental(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetCustomerListForViewCustGrp_R_Rental(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get customer list for view customer group from sale contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtCustomerListGrp> GetCustomerListForViewCustGrp_R_Sale(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetCustomerListForViewCustGrp_R_Sale(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get contract list for view customer group from rental contract (contract target)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtContractListGrp> GetContractListForViewCustGrp_CT_Rental(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetContractListForViewCustGrp_CT_Rental(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get contract list for view customer group from sale contract (Purchaser)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtContractListGrp> GetContractListForViewCustGrp_CT_Sale(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetContractListForViewCustGrp_CT_Sale(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get contract list for view customer group from rental contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtContractListGrp> GetContractListForViewCustGrp_R_Rental(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetContractListForViewCustGrp_R_Rental(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get contract list for view customer group from sale contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtContractListGrp> GetContractListForViewCustGrp_R_Sale(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetContractListForViewCustGrp_R_Sale(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get site list for view customer group from rental contract (contract target)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtsiteListGrp> GetSiteListForViewCustGrp_CT_Rental(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetSiteListForViewCustGrp_CT_Rental(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get site list for view customer group from sale contract (Purchaser)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtsiteListGrp> GetSiteListForViewCustGrp_CT_Sale(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetSiteListForViewCustGrp_CT_Sale(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get site list for view customer group from rental contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtsiteListGrp> GetSiteListForViewCustGrp_R_Rental(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetSiteListForViewCustGrp_R_Rental(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get site list for view customer group from rental contract (Real customer)
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <param name="strCONTRACT_PREFIX"></param>
        /// <returns></returns>
        public List<dtsiteListGrp> GetSiteListForViewCustGrp_R_Sale(string strGroupCode, string strCONTRACT_PREFIX)
        {
            try
            {
                return base.GetSiteListForViewCustGrp_R_Sale(
                    strGroupCode, strCONTRACT_PREFIX, ContractStatus.C_CONTRACT_STATUS_CANCEL
                    , ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get group summary for view customer group data
        /// </summary>
        /// <param name="strGroupCode"></param>
        /// <returns></returns>
        public List<dtGroupSummaryForShow> GetGroupSummaryForViewCustGrpData(string strGroupCode)
        {
            List<dtGroupSummaryForShow> result = new List<dtGroupSummaryForShow>();
            try
            {
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();
                List<dtGroupSummary> lst = base.GetGroupSummaryForViewCustGrp(strGroupCode, ContractStatus.C_CONTRACT_STATUS_CANCEL, ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                //Add Currency
                for (int i = 0; i < lst.Count(); i++)
                {
                    lst[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                if (lst.Count > 0)
                {
                    //set data to match grid
                    dtGroupSummaryForShow custGrp = new dtGroupSummaryForShow();
                    dtGroupSummaryForShow siteGrp = new dtGroupSummaryForShow();
                    dtGroupSummaryForShow contractGrp = new dtGroupSummaryForShow();
                    dtGroupSummaryForShow amountGrp = new dtGroupSummaryForShow();
                    dtGroupSummaryForShow contractAmt = new dtGroupSummaryForShow();

                    //Contract Prefix 'N' => Alarm
                    dtGroupSummary dtGroupAlarm = lst.Find(grp => grp.ContractPrefix == ContractPrefix.C_CONTRACT_PREFIX_ALARM);
                    if (!CommonUtil.IsNullOrEmpty(dtGroupAlarm))
                    {
                        custGrp.ContractAlarm = ConvertFromIntToStringFormat(dtGroupAlarm.CountCustCode_CT);
                        siteGrp.ContractAlarm = ConvertFromIntToStringFormat(dtGroupAlarm.CountSiteCode_CT);
                        contractGrp.ContractAlarm = ConvertFromIntToStringFormat(dtGroupAlarm.CountContractCode_CT);
                        amountGrp.ContractAlarm = dtGroupAlarm.TextTransferSumContractAmount_CT_LOCAL + "<br>" + dtGroupAlarm.TextTransferSumContractAmount_CT_US;
                        //amountGrp.ContractAlarm = ConvertFromDecimalToStringFormat(dtGroupAlarm.SumContractAmount_CT);
                        custGrp.CustomerAlarm = ConvertFromIntToStringFormat(dtGroupAlarm.CountCustCode_R);
                        siteGrp.CustomerAlarm = ConvertFromIntToStringFormat(dtGroupAlarm.CountSiteCode_R);
                        contractGrp.CustomerAlarm = ConvertFromIntToStringFormat(dtGroupAlarm.CountContractCode_R);
                        amountGrp.CustomerAlarm = dtGroupAlarm.TextTransferSumContractAmount_R_LOCAL + "<br>" + dtGroupAlarm.TextTransferSumContractAmount_R_US;
                        //amountGrp.CustomerAlarm = ConvertFromDecimalToStringFormat(dtGroupAlarm.SumContractAmount_R);
                    }

                    //Contract Prefix 'MA' => Maintainence
                    dtGroupSummary dtGroupMaint = lst.Find(grp => grp.ContractPrefix == ContractPrefix.C_CONTRACT_PREFIX_MAINTAINENCE);
                    if (!CommonUtil.IsNullOrEmpty(dtGroupMaint))
                    {
                        custGrp.ContractMaintenance = ConvertFromIntToStringFormat(dtGroupMaint.CountCustCode_CT);
                        siteGrp.ContractMaintenance = ConvertFromIntToStringFormat(dtGroupMaint.CountSiteCode_CT);
                        contractGrp.ContractMaintenance = ConvertFromIntToStringFormat(dtGroupMaint.CountContractCode_CT);
                        amountGrp.ContractMaintenance = dtGroupMaint.TextTransferSumContractAmount_CT_LOCAL + "<br>" + dtGroupMaint.TextTransferSumContractAmount_CT_US;
                        //amountGrp.ContractMaintenance = ConvertFromDecimalToStringFormat(dtGroupMaint.SumContractAmount_CT);
                        custGrp.CustomerMaintenance = ConvertFromIntToStringFormat(dtGroupMaint.CountCustCode_R);
                        siteGrp.CustomerMaintenance = ConvertFromIntToStringFormat(dtGroupMaint.CountSiteCode_R);
                        contractGrp.CustomerMaintenance = ConvertFromIntToStringFormat(dtGroupMaint.CountContractCode_R);
                        amountGrp.CustomerMaintenance = dtGroupMaint.TextTransferSumContractAmount_R_LOCAL + "<br>" + dtGroupMaint.TextTransferSumContractAmount_R_US;
                        //amountGrp.CustomerMaintenance = ConvertFromDecimalToStringFormat(dtGroupMaint.SumContractAmount_R);
                    }

                    //Contract Prefix 'SG' => Maintainence
                    dtGroupSummary dtGroupGuard = lst.Find(grp => grp.ContractPrefix == ContractPrefix.C_CONTRACT_PREFIX_GUARD);
                    if (!CommonUtil.IsNullOrEmpty(dtGroupGuard))
                    {
                        custGrp.ContractGuard = ConvertFromIntToStringFormat(dtGroupGuard.CountCustCode_CT);
                        siteGrp.ContractGuard = ConvertFromIntToStringFormat(dtGroupGuard.CountSiteCode_CT);
                        contractGrp.ContractGuard = ConvertFromIntToStringFormat(dtGroupGuard.CountContractCode_CT);
                        amountGrp.ContractGuard = dtGroupGuard.TextTransferSumContractAmount_CT_LOCAL + "<br>" + dtGroupGuard.TextTransferSumContractAmount_CT_US;
                        //amountGrp.ContractGuard = ConvertFromDecimalToStringFormat(dtGroupGuard.SumContractAmount_CT);
                        custGrp.CustomerGuard = ConvertFromIntToStringFormat(dtGroupGuard.CountCustCode_R);
                        siteGrp.CustomerGuard = ConvertFromIntToStringFormat(dtGroupGuard.CountSiteCode_R);
                        contractGrp.CustomerGuard = ConvertFromIntToStringFormat(dtGroupGuard.CountContractCode_R);
                        amountGrp.CustomerGuard = dtGroupGuard.TextTransferSumContractAmount_R_LOCAL + "<br>" + dtGroupGuard.TextTransferSumContractAmount_R_US;
                        //amountGrp.CustomerGuard = ConvertFromDecimalToStringFormat(dtGroupGuard.SumContractAmount_R);
                    }

                    //Contract Prefix 'Q' => Sale
                    dtGroupSummary dtGroupSale = lst.Find(grp => grp.ContractPrefix == ContractPrefix.C_CONTRACT_PREFIX_SALE);
                    if (!CommonUtil.IsNullOrEmpty(dtGroupSale))
                    {
                        //custGrp.ContractSale = (dtGroupSale.CountCustCode_CT.HasValue) ? dtGroupSale.CountCustCode_CT.Value.ToString("#,###") : "-";
                        //siteGrp.ContractSale = (dtGroupSale.CountSiteCode_CT.HasValue) ? dtGroupSale.CountSiteCode_CT.Value.ToString("#,###") : "-";
                        //contractGrp.ContractSale = (dtGroupSale.CountContractCode_CT.HasValue) ? dtGroupSale.CountContractCode_CT.Value.ToString("#,###") : "-";
                        //amountGrp.ContractSale = (dtGroupSale.SumContractAmount_CT.HasValue) ? dtGroupSale.SumContractAmount_CT.Value.ToString("#,###,0000") : "-";
                        //custGrp.CustomerSale = (dtGroupSale.CountCustCode_R.HasValue) ? dtGroupSale.CountCustCode_R.Value.ToString("#,###") : "-";
                        //siteGrp.CustomerSale = (dtGroupSale.CountSiteCode_R.HasValue) ? dtGroupSale.CountSiteCode_R.Value.ToString("#,###") : "-";
                        //contractGrp.CustomerSale = (dtGroupSale.CountContractCode_R.HasValue) ? dtGroupSale.CountContractCode_R.Value.ToString("#,###") : "-";
                        //amountGrp.CustomerSale = (dtGroupSale.SumContractAmount_R.HasValue) ? dtGroupSale.SumContractAmount_R.Value.ToString("#,###,0000") : "-";
                        custGrp.ContractSale = ConvertFromIntToStringFormat(dtGroupSale.CountCustCode_CT);
                        siteGrp.ContractSale = ConvertFromIntToStringFormat(dtGroupSale.CountSiteCode_CT);
                        contractGrp.ContractSale = ConvertFromIntToStringFormat(dtGroupSale.CountContractCode_CT);
                        amountGrp.ContractSale = dtGroupSale.TextTransferSumContractAmount_CT_LOCAL + "<br>" + dtGroupSale.TextTransferSumContractAmount_CT_US;
                        //amountGrp.ContractSale = ConvertFromDecimalToStringFormat(dtGroupSale.SumContractAmount_CT);
                        contractAmt.ContractSale = dtGroupSale.FinalTextContractAmount_CT;
                        custGrp.CustomerSale = ConvertFromIntToStringFormat(dtGroupSale.CountCustCode_R);
                        siteGrp.CustomerSale = ConvertFromIntToStringFormat(dtGroupSale.CountSiteCode_R);
                        contractGrp.CustomerSale = ConvertFromIntToStringFormat(dtGroupSale.CountContractCode_R);
                        amountGrp.CustomerSale = dtGroupSale.TextTransferSumContractAmount_R_LOCAL + "<br>" + dtGroupSale.TextTransferSumContractAmount_R_US;
                        //amountGrp.CustomerSale = ConvertFromDecimalToStringFormat(dtGroupSale.SumContractAmount_R);
                        contractAmt.CustomerSale = dtGroupSale.FinalTextContractAmount_R;
                    }

                    custGrp.RowHeader = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "colNumberOfCustomer");
                    custGrp.RowPrefix = "customer";
                    siteGrp.RowHeader = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "colNumberOfSite");
                    siteGrp.RowPrefix = "site";
                    contractGrp.RowHeader = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "colNumberOfContract");
                    contractGrp.RowPrefix = "contract";
                    amountGrp.RowHeader = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "colAmountOfContract");
                    amountGrp.RowPrefix = "amount";
                    result.Add(custGrp);
                    result.Add(siteGrp);
                    result.Add(contractGrp);
                    result.Add(amountGrp);
                    result.Add(contractAmt);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Convert int to string
        /// </summary>
        /// <param name="inputInt"></param>
        /// <returns></returns>
        private string ConvertFromIntToStringFormat(int? inputInt)
        {
            string result = "-";
            if (inputInt.HasValue && inputInt.Value > 0)
            {
                //result = inputInt.Value.ToString("#,###");
                result = inputInt.Value.ToString("N0");
            }

            return result;
        }

        /// <summary>
        /// Convert decimal to string
        /// </summary>
        /// <param name="inputDec"></param>
        /// <returns></returns>
        private string ConvertFromDecimalToStringFormat(decimal? inputDec)
        {
            string result = "-";
            if (inputDec.HasValue && inputDec.Value > 0)
            {
                //result = inputDec.Value.ToString("#,###,0000");
                result = inputDec.Value.ToString("N0");
            }

            return result;
        }

        /// <summary>
        /// Getting site information
        /// </summary>
        /// <param name="strSiteCode"></param>
        /// <returns></returns>
        public dsSiteInfo GetSiteInfo(string strSiteCode)
        {
            IMasterHandler masterhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            dsSiteInfo res = new dsSiteInfo();

            var siteLst = masterhandler.GetTbm_Site(strSiteCode);
            if (siteLst.Count == 1)
            {
                var siteusageLst = masterhandler.GetSiteBuildingUsage(siteLst[0].BuildingUsageCode);
                res.doGetTbm_Site = siteLst;
                res.tbm_BuildingUsage = siteusageLst;
            }

            return res;
        }

        /// <summary>
        /// Getting project information
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public dsProjectInfo GetProjectInfo(string strProjectCode)
        {
            ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            IProjectHandler projecthandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            dsProjectInfo res = new dsProjectInfo();

            var projRes = projecthandler.GetTbt_Project(strProjectCode);

            if (projRes.Count == 1)
            {
                var projCust = projecthandler.GetTbt_ProjectPurchaserCustomerForView(strProjectCode);
                res.dtProjectPurcheser = new dtProjectPurcheser();

                if (projCust.Count == 1)
                {
                    if (string.IsNullOrEmpty(projCust[0].CustCode))
                    {
                        res.dtProjectPurcheser.CustCode = "";
                        res.dtProjectPurcheser.CustFullNameEN = projCust[0].CustFullNameEN;
                        res.dtProjectPurcheser.CustFullNameLC = projCust[0].CustFullNameLC;
                    }
                    else
                    {
                        var custRes = custhandler.GetCustomer(projCust[0].CustCode);
                        if (custRes.Count == 1)
                        {
                            res.dtProjectPurcheser.CustCode = projCust[0].CustCode;
                            res.dtProjectPurcheser.CustFullNameEN = custRes[0].CustFullNameEN;
                            res.dtProjectPurcheser.CustFullNameLC = custRes[0].CustFullNameLC;
                        }
                    }
                }

                res.tbt_Project = projRes;
            }

            return res;
        }

        /// <summary>
        /// Get contract target info by related
        /// </summary>
        /// <param name="pRelatedContractCode"></param>
        /// <param name="pRelationType"></param>
        /// <param name="pRelatedOCC"></param>
        /// <param name="pRelatedProductTypeCode"></param>
        /// <returns></returns>
        public List<dtContractTargetInfoByRelated> GetContractTargetInfoByRelated(string pRelatedContractCode, string pRelationType, string pRelatedOCC, string pRelatedProductTypeCode)
        {
            try
            {
                return base.GetContractTargetInfoByRelated(pRelatedContractCode,
                                                            pRelationType,
                                                            pRelatedOCC, 
                                                            ProductType.C_PROD_TYPE_SALE, 
                                                            ProductType.C_PROD_TYPE_AL, 
                                                            ProductType.C_PROD_TYPE_ONLINE,
                                                            ProductType.C_PROD_TYPE_RENTAL_SALE,
                                                            pRelatedProductTypeCode,
                                                            RelationType.C_RELATION_TYPE_MA, 
                                                            RelationType.C_RELATION_TYPE_SALE);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get maintenance target info by related
        /// </summary>
        /// <param name="pRelatedContractCode"></param>
        /// <param name="pRelatedOCC"></param>
        /// <param name="pRelatedProductTypeCode"></param>
        /// <returns></returns>
        public List<dtMaintContractTargetInfoByRelated> GetMaintContractTargetInfoByRelated(string pRelatedContractCode, string pRelatedOCC, string pRelatedProductTypeCode)
        {
            try
            {
                return base.GetMaintContractTargetInfoByRelated(pRelatedContractCode, 
                                                                ProductType.C_PROD_TYPE_MA, 
                                                                MiscType.C_MA_TYPE, 
                                                                MiscType.C_MA_FEE_TYPE, 
                                                                RelationType.C_RELATION_TYPE_MA, 
                                                                pRelatedOCC,
                                                                ProductType.C_PROD_TYPE_SALE, 
                                                                ProductType.C_PROD_TYPE_AL, 
                                                                ProductType.C_PROD_TYPE_ONLINE,
                                                                ProductType.C_PROD_TYPE_RENTAL_SALE,
                                                                pRelatedProductTypeCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<dtContractList> GetContractListForSearchInfo(string pchrRoleTypeContractTarget, string pchrRoleTypePurchaser, string pchrRoleTypeRealCustomer, string pchrServiceTypeCode, string pchrCustomerCode, string pchrGroupCode, string pchrSiteCode, string pchvContractCode, string pchvUserCode, string pchvPlanCode, string pchvProjectCode, string pchrnCustomerName, string pchrnBranchName, string pchrnGroupName, string pchrCustomerStatus, string pchrCustomerTypeCode, string pchrCompanyTypeCode, string pchrnIDNo, string pchrRegionCode, string pchrBusinessTypeCode, string pchrnCust_Address, string pchrnCust_Alley, string pchrnCust_Road, string pchrnCust_SubDistrict, string pchrCust_ProvinceCode, string pchrCust_DistrictCode, string pchrCust_ZipCode, string pchrnCust_PhoneNo, string pchrnCust_FaxNo, string pchrnSiteName, string pchrnSite_Address, string pchrnSite_Alley, string pchrnSite_Road, string pchrnSite_SubDistrict, string pchrSite_ProvinceCode, string pchrSite_DistrictCode, string pchrSite_ZipCode, string pchrnSite_PhoneNo, Nullable<System.DateTime> pdtmOperationDate_From, Nullable<System.DateTime> pdtmOperationDate_To, Nullable<System.DateTime> pdtmCustAcceptDate_From, Nullable<System.DateTime> pdtmCustAcceptDate_To, Nullable<System.DateTime> pdtmInstallationCompleteDate_From, Nullable<System.DateTime> pdtmInstallationCompleteDate_To, string pchvContractOfficeCode, string pchvdsTransDataOfficeCode, string pchvOperationOfficeCode, string pchvSalesmanEmpNo1, string pchvSalesmanEmpName1, string pchrProductCode, string pchrChangeTypeCode, string pchrProcessManageStatusCode, string pchrStartTypeCode, string pchvC_RENTAL_CHANGE_TYPE, string pchvC_SALE_CHANGE_TYPE, string pchvC_SALE_PROC_MANAGE_STATUS, string pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET, string pchrC_CUST_ROLE_TYPE_REAL_CUST, string pchrC_CUST_ROLE_TYPE_PURCHASER, Nullable<bool> pbitC_FLAG_ON, string pchrC_SERVICE_TYPE_RENTAL, string pchrC_SERVICE_TYPE_SALE, string pchrC_CONTRACT_STATUS_BEF_START, string pchrC_CONTRACT_STATUS_CANCEL, string pchrC_CONTRACT_STATUS_END, string c_SALE_CHANGE_TYPE_NEW_SALE, string c_CUST_TYPE_JURISTIC, Nullable<System.DateTime> stopDateFrom, Nullable<System.DateTime> stopDateTo, Nullable<System.DateTime> cancelDateFrom, Nullable<System.DateTime> cancelDateTo)
        {
            return base.GetContractListForSearchInfo(pchrRoleTypeContractTarget, pchrRoleTypePurchaser, pchrRoleTypeRealCustomer, pchrServiceTypeCode, pchrCustomerCode, pchrGroupCode
                                                    , pchrSiteCode, pchvContractCode, pchvUserCode, pchvPlanCode, pchvProjectCode, pchrnCustomerName, pchrnBranchName, pchrnGroupName
                                                    , pchrCustomerStatus, pchrCustomerTypeCode, pchrCompanyTypeCode, pchrnIDNo, pchrRegionCode, pchrBusinessTypeCode, pchrnCust_Address
                                                    , pchrnCust_Alley, pchrnCust_Road, pchrnCust_SubDistrict, pchrCust_ProvinceCode, pchrCust_DistrictCode, pchrCust_ZipCode, pchrnCust_PhoneNo
                                                    , pchrnCust_FaxNo, pchrnSiteName, pchrnSite_Address, pchrnSite_Alley, pchrnSite_Road, pchrnSite_SubDistrict, pchrSite_ProvinceCode, pchrSite_DistrictCode
                                                    , pchrSite_ZipCode, pchrnSite_PhoneNo, pdtmOperationDate_From, pdtmOperationDate_To, pdtmCustAcceptDate_From, pdtmCustAcceptDate_To, pdtmInstallationCompleteDate_From
                                                    , pdtmInstallationCompleteDate_To, pchvContractOfficeCode, pchvdsTransDataOfficeCode, pchvOperationOfficeCode, pchvSalesmanEmpNo1, pchvSalesmanEmpName1
                                                    , pchrProductCode, pchrChangeTypeCode, pchrProcessManageStatusCode, pchrStartTypeCode, pchvC_RENTAL_CHANGE_TYPE, pchvC_SALE_CHANGE_TYPE, pchvC_SALE_PROC_MANAGE_STATUS
                                                    , pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET, pchrC_CUST_ROLE_TYPE_REAL_CUST, pchrC_CUST_ROLE_TYPE_PURCHASER, pbitC_FLAG_ON, pchrC_SERVICE_TYPE_RENTAL, pchrC_SERVICE_TYPE_SALE
                                                    , pchrC_CONTRACT_STATUS_BEF_START, pchrC_CONTRACT_STATUS_CANCEL, pchrC_CONTRACT_STATUS_END, c_SALE_CHANGE_TYPE_NEW_SALE, c_CUST_TYPE_JURISTIC, stopDateFrom, stopDateTo
                                                    , cancelDateFrom, cancelDateTo, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
        }
    }
}
