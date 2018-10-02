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

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS200
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS200_Authority(CMS200_ScreenParameter param) // IN parameter: string strContractCode, string strServiceTypeCode
        {
            
            ObjectResultData res = new ObjectResultData();

            // Check permission
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_CONTRACT_BILLING, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            // Check parameter is OK ?
            if (CommonUtil.IsNullOrEmpty(param.strContractCode) == false && CommonUtil.IsNullOrEmpty(param.strServiceTypeCode) == false)
            {
                    param.ContractCode = param.strContractCode;
                    param.ServiceTypeCode = param.strServiceTypeCode;
            }
            else
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                return Json(res);
            }

            // Check exist data
            try
            {
                CommonUtil c = new CommonUtil();
                string ContractCode = c.ConvertContractCode(param.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);


                IRentralContractHandler handlerRC = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler handlerSC = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                List<dtTbt_RentalContractBasicForView> listRC = new List<dtTbt_RentalContractBasicForView>();
                List<dtTbt_SaleBasicForView> listSC = new List<dtTbt_SaleBasicForView>();

                if (param.strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    listRC = handlerRC.GetTbt_RentalContractBasicForView(ContractCode);
                }
                else if (param.strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    listSC = handlerSC.GetTbt_SaleBasicForView(ContractCode, null, true);
                }

                if (listRC.Count == 0 && listSC.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }

            }
            catch
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                return Json(res);
            }



            return InitialScreenEnvironment<CMS200_ScreenParameter>("CMS200", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS200
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS200")]
        public ActionResult CMS200()
        {
            string strContractCode = "" ;
            string strServiceTypeCode = "";

            try
            {
                CMS200_ScreenParameter param = GetScreenObject<CMS200_ScreenParameter>();
                strContractCode = param.ContractCode;
                strServiceTypeCode = param.ServiceTypeCode;
            }
            catch
            {
            }

            // Keep service type code
            ViewBag.ServiceTypeCode = strServiceTypeCode;

            // Keep contract code  (short)
            ViewBag._ContractCode = strContractCode;
            ViewBag._OCC = string.Empty;
            ViewBag._ContractTargetCode = string.Empty;
            ViewBag._RealCustomerCode = string.Empty;
            ViewBag._PurchaserCustCode = string.Empty;
            ViewBag._SiteCode = string.Empty;



            CommonUtil c = new CommonUtil();
            strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            try
            {
                IRentralContractHandler handlerRC = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler handlerSC = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    List<dtTbt_RentalContractBasicForView> listRC =  handlerRC.GetTbt_RentalContractBasicForView(strContractCode );

                    if (listRC.Count > 0)
                    {
                        // convert code to short format
                        listRC[0].ContractCode = c.ConvertContractCode(listRC[0].ContractCode ,CommonUtil.CONVERT_TYPE.TO_SHORT);
                        listRC[0].ContractTargetCustCode = c.ConvertCustCode(listRC[0].ContractTargetCustCode ,CommonUtil.CONVERT_TYPE.TO_SHORT);
                        listRC[0].RealCustomerCustCode = c.ConvertCustCode(listRC[0].RealCustomerCustCode ,CommonUtil.CONVERT_TYPE.TO_SHORT);
                        listRC[0].SiteCode = c.ConvertSiteCode(listRC[0].SiteCode ,CommonUtil.CONVERT_TYPE.TO_SHORT);

                        
                        // gorbal javascript variable
                        ViewBag._OCC = listRC[0].LastOCC;
                        ViewBag._ContractTargetCode = listRC[0].ContractTargetCustCode ;
                        ViewBag._RealCustomerCode = listRC[0].RealCustomerCustCode;
                        ViewBag._SiteCode = listRC[0].SiteCode;



                        ViewBag.txtContractCode  = listRC[0].ContractCode  ;
                        ViewBag.txtUserCode  = listRC[0].UserCode  ;
                        ViewBag.lnkCustomerCodeC  = listRC[0].ContractTargetCustCode  ;
                        ViewBag.lnkCustomerCodeR  = listRC[0].RealCustomerCustCode  ;
                        ViewBag.lnkSiteCode  = listRC[0].SiteCode  ;
                        ViewBag.txtContractNameEng  = listRC[0].CustFullNameEN_Cust  ;
                        ViewBag.txtContractAddrEng  = listRC[0].AddressFullEN_Cust  ;
                        ViewBag.txtSiteNameEng  = listRC[0].SiteNameEN_Site  ;
                        ViewBag.txtSiteAddrEng  = listRC[0].AddressFullEN_Site  ;
                        ViewBag.txtContractNameLocal  = listRC[0].CustFullNameLC_Cust  ;
                        ViewBag.txtContractAddrLocal  = listRC[0].AddressFullLC_Cust  ;
                        ViewBag.txtSiteNameLocal  = listRC[0].SiteNameLC_Site  ;
                        ViewBag.txtSiteAddrLocal  = listRC[0].AddressFullLC_Site  ;
                        //ViewBag.txtContactPoint  = listRC[0].ContactPoint  ;
                        ViewBag.txtContactPoint = CommonUtil.IsNullOrEmpty(listRC[0].ContactPoint) == true ? "-" : listRC[0].ContactPoint;

                        ViewBag.txtRentalAttachImportanceFlag = listRC[0].SpecialCareFlag;

                        ViewBag.ProductTypeCode = listRC[0].ProductTypeCode; 
                    }
                   
                }
                else if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    List<dtTbt_SaleBasicForView> listSC =  handlerSC.GetTbt_SaleBasicForView(strContractCode, null, true);

                    if (listSC.Count > 0) // SCB : Sale Contract Basic
                    {
                        // convert code to short format
                        listSC[0].ContractCode = c.ConvertContractCode(listSC[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        listSC[0].CustCode_PurCust = c.ConvertCustCode(listSC[0].CustCode_PurCust, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        listSC[0].CustCode_RealCust = c.ConvertCustCode(listSC[0].CustCode_RealCust, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        listSC[0].SiteCode = c.ConvertSiteCode(listSC[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        // gorbal javascript variable
                        ViewBag._OCC = listSC[0].OCC;
                        ViewBag._PurchaserCustCode = listSC[0].CustCode_PurCust;
                        ViewBag._RealCustomerCode = listSC[0].CustCode_RealCust;
                        ViewBag._SiteCode = listSC[0].SiteCode;


                       
                        ViewBag.txtSCBContractCode  = listSC[0].ContractCode  ;
                        ViewBag.lnkSCBPurchaserC = listSC[0].CustCode_PurCust;
                        ViewBag.lnkSCBCustomerCodeR  = listSC[0].CustCode_RealCust  ;
                        ViewBag.lnkSCBSiteCode  = listSC[0].SiteCode  ;
                        ViewBag.txtSCBPurchaserNameEng  = listSC[0].PurCust_CustFullNameEN  ;
                        ViewBag.txtSCBPurchaserAddrEng  = listSC[0].AddressFullEN_PurCust  ;
                        ViewBag.txtSCBSiteNameEng  = listSC[0].site_SiteNameEN  ;
                        ViewBag.txtSCBSiteAddrEng  = listSC[0].AddressFullEN_site  ;
                        ViewBag.txtSCBPurchaserNameLocal  = listSC[0].PurCust_CustFullNameLC  ;
                        ViewBag.txtSCBPurchaserAddrLocal  = listSC[0].AddressFullLC_PurCust  ;
                        ViewBag.txtSCBSiteNameLocal  = listSC[0].site_SiteNameLC  ;
                        ViewBag.txtSCBSiteAddrLocal = listSC[0].AddressFullLC_site;
                        //ViewBag.txtSCBContactPoint  = listSC[0].ContactPoint  ;
                        ViewBag.txtSCBContactPoint = CommonUtil.IsNullOrEmpty(listSC[0].ContactPoint) == true ? "-" : listSC[0].ContactPoint;

                        ViewBag.txtSaleAttachImportanceFlag = listSC[0].SpecialCareFlag;

                        ViewBag.ProductTypeCode = listSC[0].ProductTypeCode; 
                    }
                }

              

                return View();
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res); 
            }

        }

        /// <summary>
        /// Initial grid of screen CMS200
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS200_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS200"));
        }

        /// <summary>
        /// Get billing temporary list by condition
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS200_GetBillingTempList(doContractInfoCondition cond)
        {
            CommonUtil c = new CommonUtil();

            List<View_dtTbt_BillingTempListForView> nlst = new List<View_dtTbt_BillingTempListForView>();

            ObjectResultData res = new ObjectResultData();
            
            try
            {
                cond.ContractCode = c.ConvertContractCode(cond.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                ICommonContractHandler handler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtTbt_BillingTempListForView> list = handler.GetTbt_BillingTempListForView(cond.ContractCode, null); 

                //Add Currency
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        list[i].BillingAmt = list[i].BillingAmtUsd;
                    }
                    list[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                // clone object to View
                foreach (dtTbt_BillingTempListForView l in list)
                {
                    nlst.Add(CommonUtil.CloneObject<dtTbt_BillingTempListForView, View_dtTbt_BillingTempListForView>(l));
                }


                // select language 
                nlst = CommonUtil.ConvertObjectbyLanguage<View_dtTbt_BillingTempListForView, View_dtTbt_BillingTempListForView>(nlst, "BillingOffice");

                // get misc display value 
                ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<string> lsFieldNames = new List<string>();
                lsFieldNames.Add(MiscType.C_BILLING_TIMING);
                lsFieldNames.Add(MiscType.C_CONTRACT_BILLING_TYPE);
                lsFieldNames.Add(MiscType.C_PAYMENT_METHOD);

                List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

                string strDisplayValue = string.Empty;
                foreach (var item in nlst)
                {
                    strDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_BILLING_TIMING,
                                                        item.BillingTiming)
                                                        ;
                    item.BillingTiming_EX = strDisplayValue; // CommonUtil.TextCodeName(item.BillingTiming, strDisplayValue);

                    strDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_CONTRACT_BILLING_TYPE,
                                                        item.BillingType);
                    item.BillingType_EX = strDisplayValue; // CommonUtil.TextCodeName(item.BillingType, strDisplayValue);

                    strDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_PAYMENT_METHOD,
                                                        item.PayMethod);
                    item.Paymethod_EX = strDisplayValue; // CommonUtil.TextCodeName(item.PayMethod, strDisplayValue);
                    
                }

                // Sorting , order by BillingOCC
                nlst = (from p in nlst orderby p.BillingOCC ascending select p).ToList<View_dtTbt_BillingTempListForView>();


            }
            catch (Exception ex)
            {
                nlst = new List<View_dtTbt_BillingTempListForView>();
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtTbt_BillingTempListForView>(nlst, "Common\\CMS200");
            return Json(res);
        }
    }
}
