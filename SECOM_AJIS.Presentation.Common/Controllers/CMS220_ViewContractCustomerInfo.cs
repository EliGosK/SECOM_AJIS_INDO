//*********************************
// Create by: Narupon W.
// Create date: /Jun/2011
// Update date: /Jun/2011
//*********************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;

using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Common.Models;
namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS220
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS220_Authority(CMS220_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS220_ScreenParameter>("CMS220", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS220
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS220")]
        public ActionResult CMS220()
        {

            CMS220_ScreenParameter cond = new CMS220_ScreenParameter();

            try
            {
                cond = GetScreenObject<CMS220_ScreenParameter>();

            }
            catch
            {
            }

            // Conver to long format
            CommonUtil c = new CommonUtil();
            cond.ContractCode = c.ConvertContractCode(cond.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            cond.ContractTargetCode = c.ConvertCustCode(cond.ContractTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            cond.PurchaserCustCode = c.ConvertCustCode(cond.PurchaserCustCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            cond.RealCustomerCode = c.ConvertCustCode(cond.RealCustomerCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            cond.SiteCode = c.ConvertSiteCode(cond.SiteCode, CommonUtil.CONVERT_TYPE.TO_LONG);


            // Keep data to ViewBag
            ViewBag.isShowContractTarget = false;
            ViewBag.isShowPurchaser = false;
            ViewBag.isShowRealCustomer = false;
            ViewBag.isShowSite = false;


            string CustomerCode = string.Empty;
            string OCC = string.Empty;
            string ContractCode = string.Empty;


            // Assign value of CSCustCode , RCCustCode
            if (!CommonUtil.IsNullOrEmpty(cond.ContractTargetCode))
            {
                cond.CSCustCode = cond.ContractTargetCode;

            }
            else if (!CommonUtil.IsNullOrEmpty(cond.PurchaserCustCode))
            {
                cond.CSCustCode = cond.PurchaserCustCode;

            }
            else if (!CommonUtil.IsNullOrEmpty(cond.RealCustomerCode))
            {
                cond.RCCustCode = cond.RealCustomerCode;

            }


            // View mode
            if (cond.Mode == "Contract")
            {
                ViewBag.isShowContractTarget = true;

                ContractCode = cond.ContractCode;
                OCC = cond.OCC;
                CustomerCode = cond.ContractTargetCode;

            }
            else if (cond.Mode == "Purchaser")
            {
                ViewBag.isShowPurchaser = true;

                ContractCode = cond.ContractCode;
                OCC = cond.OCC;
                CustomerCode = cond.PurchaserCustCode;
            }
            else if (cond.Mode == "Customer")
            {
                ViewBag.isShowRealCustomer = true;

                ContractCode = "NotSpecify";
                OCC = "NotSpecify";
                CustomerCode = cond.RealCustomerCode;
            }
            else if (cond.Mode == "Site")
            {
                ViewBag.isShowSite = true;
            }



            try
            {
                ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<string> lsFieldNames = new List<string>();
                lsFieldNames.Add(MiscType.C_CONTRACT_SIGNER_TYPE);
                lsFieldNames.Add(MiscType.C_CUST_STATUS);
                lsFieldNames.Add(MiscType.C_CUST_TYPE);
                lsFieldNames.Add(MiscType.C_FINANCIAL_MARKET_TYPE);

                List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);


                //IViewContractHandler handler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                //List<dtChangedCustHistList2> list = handler.GetChangedCustHistList2(cond);

                // *******

                ICustomerMasterHandler handlerCM = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                IViewContractHandler handlerVC = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                ISiteMasterHandler handlerSM = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;



                List<doCustomer> listCustomer = new List<doCustomer>();
                List<doSite> listSite = new List<doSite>();
                List<dtContractSignerType> listSigner = new List<dtContractSignerType>();

                listCustomer = handlerCM.GetCustomer(CustomerCode);
                listSigner = handlerVC.GetContractSignerType(MiscType.C_CONTRACT_SIGNER_TYPE, ContractCode, OCC);
                listSite = handlerSM.GetSite(cond.SiteCode, null);

                // select language  
                listCustomer = CommonUtil.ConvertObjectbyLanguage<doCustomer, doCustomer>(listCustomer,
                                            "BusinessTypeName",
                                            "Nationality",
                                            "CustStatusName",
                                            "CustTypeName",
                                            "FinancialMaketTypeName"
                                            );


                // select language
                listSite = CommonUtil.ConvertObjectbyLanguage<doSite, doSite>(listSite, "BuildingUsageName");

                // select language
                listSigner = CommonUtil.ConvertObjectbyLanguage<dtContractSignerType, dtContractSignerType>(listSigner, "ContractSignerTypeName");



                // Convert code to short format
                foreach (doCustomer item in listCustomer)
                {
                    item.CustCode = c.ConvertCustCode(item.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                }

                foreach (var item in listSite)
                {
                    item.SiteCode = c.ConvertSiteCode(item.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }


                if (listCustomer.Count > 0)
                {

                    if (!CommonUtil.IsNullOrEmpty(cond.ContractTargetCode))
                    {
                        ViewBag.txtCTCustomerCode = listCustomer[0].CustCode;

                        if (listSigner.Count > 0)
                            ViewBag.txtCTContractSignerType = CommonUtil.TextCodeName(listSigner[0].ContractSignerTypeCode, listSigner[0].ContractSignerTypeName);


                        ViewBag.txtCTCustomerStatus = CommonUtil.TextCodeName(listCustomer[0].CustStatus, listCustomer[0].CustStatusName);

                        ViewBag.txtCTCustomerType = CommonUtil.TextCodeName(listCustomer[0].CustTypeCode, listCustomer[0].CustTypeName);
                        ViewBag.txtCTName_English = listCustomer[0].CustFullNameEN;
                        ViewBag.txtCTAddress_English = listCustomer[0].AddressFullEN;
                        ViewBag.txtCTName_Local = listCustomer[0].CustFullNameLC;
                        ViewBag.txtCTAddress_Local = listCustomer[0].AddressFullLC;
                        ViewBag.txtCTRepresentativePersonName = listCustomer[0].RepPersonName;
                        ViewBag.txtCTContactPersonName = listCustomer[0].ContactPersonName;
                        ViewBag.txtCTNationality = listCustomer[0].Nationality;
                        ViewBag.txtCTTelephoneNo = listCustomer[0].PhoneNo;
                        ViewBag.txtCTFinancialMarketType = CommonUtil.TextCodeName(listCustomer[0].FinancialMarketTypeCode, listCustomer[0].FinancialMaketTypeName);
                        ViewBag.txtCTBusinessType = listCustomer[0].BusinessTypeName;
                        ViewBag.txtCTIDTax = listCustomer[0].IDNo;
                        ViewBag.txtCTSECOMContactPerson = listCustomer[0].SECOMContactPerson;
                        ViewBag.txtCTURL = listCustomer[0].URL;

                        // ** Old version
                        //ViewBag.txtCTBranchName_English = listCustomer[0].BranchNameEN;
                        //ViewBag.txtCTBranchAddress_English = listCustomer[0].BranchAddressEN;
                        //ViewBag.txtCTBranchName_Local = listCustomer[0].BranchNameLC;
                        //ViewBag.txtCTBranchAddress_Local = listCustomer[0].BranchAddressLC;

                    }

                    if (!CommonUtil.IsNullOrEmpty(cond.PurchaserCustCode))
                    {
                        ViewBag.txtPUCustomerCode = listCustomer[0].CustCode;

                        if (listSigner.Count > 0)
                            ViewBag.txtPUContractSignerType = CommonUtil.TextCodeName(listSigner[0].ContractSignerTypeCode, listSigner[0].ContractSignerTypeName);

                        ViewBag.txtPUCustomerStatus = CommonUtil.TextCodeName(listCustomer[0].CustStatus, listCustomer[0].CustStatusName);


                        ViewBag.txtPUCustomerType = CommonUtil.TextCodeName(listCustomer[0].CustTypeCode, listCustomer[0].CustTypeName);
                        ViewBag.txtPUName_English = listCustomer[0].CustFullNameEN;
                        ViewBag.txtPUAddress_English = listCustomer[0].AddressFullEN;
                        ViewBag.txtPUName_Local = listCustomer[0].CustFullNameLC;
                        ViewBag.txtPUAddress_Local = listCustomer[0].AddressFullLC;
                        ViewBag.txtPURepresentativePersonName = listCustomer[0].RepPersonName;
                        ViewBag.txtPUContactPersonName = listCustomer[0].ContactPersonName;
                        ViewBag.txtPUNationality = listCustomer[0].Nationality;
                        ViewBag.txtPUTelephoneNo = listCustomer[0].PhoneNo;

                        ViewBag.txtPUFinancialMarketType = CommonUtil.TextCodeName(listCustomer[0].FinancialMarketTypeCode, listCustomer[0].FinancialMaketTypeName);
                        ViewBag.txtPUBusinessType = listCustomer[0].BusinessTypeName;
                        ViewBag.txtPUIDTax = listCustomer[0].IDNo;
                        ViewBag.txtPUSECOMContactPerson = listCustomer[0].SECOMContactPerson;
                        ViewBag.txtPUURL = listCustomer[0].URL;

                        // ** Old version
                        //ViewBag.txtPUBranchName_English = listCustomer[0].BranchNameEN;
                        //ViewBag.txtPUBranchAddress_English = listCustomer[0].BranchAddressEN;
                        //ViewBag.txtPUBranchName_Local = listCustomer[0].BranchNameLC;
                        //ViewBag.txtPUBranchAddress_Local = listCustomer[0].BranchAddressLC;
                    }


                    if (!CommonUtil.IsNullOrEmpty(cond.RealCustomerCode))
                    {
                        ViewBag.txtRCCustomerCode = listCustomer[0].CustCode;

                        ViewBag.txtRCCustomerStatus = CommonUtil.TextCodeName(listCustomer[0].CustStatus, listCustomer[0].CustStatusName);
                        ViewBag.txtRCCustomerType = CommonUtil.TextCodeName(listCustomer[0].CustTypeCode, listCustomer[0].CustTypeName);
                        ViewBag.txtRCName_English = listCustomer[0].CustFullNameEN;
                        ViewBag.txtRCAddress_English = listCustomer[0].AddressFullEN;
                        ViewBag.txtRCName_Local = listCustomer[0].CustFullNameLC;
                        ViewBag.txtRCAddress_Local = listCustomer[0].AddressFullLC;
                        ViewBag.txtRCRepresentativePersonName = listCustomer[0].RepPersonName;
                        ViewBag.txtRCContactPersonName = listCustomer[0].ContactPersonName;
                        ViewBag.txtRCNationality = listCustomer[0].Nationality;
                        ViewBag.txtRCTelephoneNo = listCustomer[0].PhoneNo;

                        ViewBag.txtRCFinancialMarketType = CommonUtil.TextCodeName(listCustomer[0].FinancialMarketTypeCode, listCustomer[0].FinancialMaketTypeName);
                        ViewBag.txtRCBusinessType = listCustomer[0].BusinessTypeName ;
                        ViewBag.txtRCIDTax = listCustomer[0].IDNo;
                        ViewBag.txtRCSECOMContactPerson = listCustomer[0].SECOMContactPerson;
                        ViewBag.txtRCURL = listCustomer[0].URL;

                        //** old version
                        //ViewBag.txtRCBranchName_English = listCustomer[0].BranchNameEN;
                        //ViewBag.txtRCBranchAddress_English = listCustomer[0].BranchAddressEN;
                        //ViewBag.txtRCBranchName_Local = listCustomer[0].BranchNameLC;
                        //ViewBag.txtRCBranchAddress_Local = listCustomer[0].BranchAddressLC;

                    }



                }
                if (listSite.Count > 0)
                {
                    if (!CommonUtil.IsNullOrEmpty(cond.SiteCode))
                    {
                        ViewBag.txtSiteCode = listSite[0].SiteCode;
                        ViewBag.txtSiteName_English = listSite[0].SiteNameEN;
                        ViewBag.txtSiteAddress_English = listSite[0].AddressFullEN;
                        ViewBag.txtSiteName_Local = listSite[0].SiteNameLC;
                        ViewBag.txtSiteAddress_Local = listSite[0].AddressFullLC;
                        ViewBag.txtSiteTelephoneNo = listSite[0].PhoneNo;
                        ViewBag.txtSiteUsage = listSite[0].BuildingUsageName;

                        ViewBag.txtAttachImportanceFlag = listSite[0].SpecialCareFlag;
                    }
                }



            }
            catch (Exception ex)
            {

                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }


            return View();
        }

        /// <summary>
        /// Initial grid of screen CMS220
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS220_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS220"));
        }

        /// <summary>
        /// Get customer group data by customer code
        /// </summary>
        /// <param name="strCustCode"></param>
        /// <returns></returns>
        public ActionResult CMS220_GetCustomerGroup(string strCustCode)
        {
            CommonUtil c = new CommonUtil();

            List<dtCustomerGroupForView> list = new List<dtCustomerGroupForView>();

            ObjectResultData res = new ObjectResultData();

            try
            {
                strCustCode = c.ConvertCustCode(strCustCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                ICustomerMasterHandler handler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;


                if (!CommonUtil.IsNullOrEmpty(strCustCode))
                {
                    list = handler.GetCustomerGroup(null, strCustCode);
                }


                //// convert code to short format
                //foreach (var item in list)
                //{
                //    item.GroupCode = c.ConvertGroupCode(item.GroupCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //}

                //return Json(CommonUtil.ConvertToXml<dtCustomerGroupForView>(list, "Common\\CMS220"));

            }
            catch (Exception ex)
            {
                list = new List<dtCustomerGroupForView>();
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtCustomerGroupForView>(list, "Common\\CMS220");
            return Json(res);
        }
    }
}
