

using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Billing;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.ActionFilters;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of CMS400
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS400_Authority(CMS400_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // for test 
                //param.CallerScreenID = "CMS410";
                //param.BillingTargetCode = "0000009-001";
                //param.isEnableBtnShowInvoiceList = true;

                //Check permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_BILLING_INFORMATION, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                IViewBillingHandler handlerViewBilling = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                CommonUtil cm = new CommonUtil();
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                string strBillingTargetCode = cm.ConvertBillingTargetCode(param.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                if (param.CallerScreenID == ScreenID.C_SCREEN_ID_VIEW_BILLING_TARGET_INFORMATION)
                {
                    //check require field
                    if (CommonUtil.IsNullOrEmpty(param.BillingTargetCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                        return Json(res);
                    }

                    //data exist
                    List<dtTbt_BillingTargetForView> lst = handlerBilling.GetTbt_BillingTargetForView(strBillingTargetCode, MiscType.C_CUST_TYPE);
                    if (lst.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                        return Json(res);
                    }
                    else
                    {
                        param.dtBillingTargetForView = lst[0];
                    }
                    if (param.isEnableBtnShowInvoiceList == false)
                    {
                        List<dtViewBillingBasicList> lstBasic = handlerViewBilling.GetViewBillingBasicList(null, strBillingTargetCode, null, null, null, null);
                        for (int i = 0; i < lstBasic.Count(); i++)
                        {
                            lstBasic[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                        }
                        //if (lstBasic.Count <= 0)
                        //{
                        //    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                        //    //return Json(res);
                        //}
                        //else
                        //{
                        param.doBasicList = lstBasic;
                        //}
                    }
                    else
                    {
                        List<dtViewBillingInvoiceListOfLastInvoiceOcc> lstInvoice = handlerViewBilling.GetViewBillingInvoiceListOfLastInvoiceOcc(null, strBillingTargetCode, null, null, null, null);
                        //if (lstInvoice.Count <= 0)
                        //{
                        //    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                        //    //return Json(res);
                        //}
                        //else
                        //{
                            param.dtViewBillingInvoiceListOfLastInvoiceOccList = lstInvoice;
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS400_ScreenParameter>("CMS400", param, res);
        }

        /// <summary>
        /// Initialize screen of CMS400
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS400")]
        public ActionResult CMS400()
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil cm = new CommonUtil();
            try
            {
                CMS400_ScreenParameter param = GetScreenObject<CMS400_ScreenParameter>();

                // Prepare for show section                   
                ViewBag.txtCallerScreenId = param.CallerScreenID;
                ViewBag.txtShowInvoiceList = param.isEnableBtnShowInvoiceList;
                if (param.dtBillingTargetForView != null)
                {
                    ViewBag.txtBillingTargetCode2 = param.dtBillingTargetForView.BillingTargetCode_Short;
                    ViewBag.txtBillingClientNameEnglish = param.dtBillingTargetForView.FullNameEN;
                    ViewBag.txtBillingClientAddressEnglish = param.dtBillingTargetForView.AddressEN;
                    ViewBag.txtBillingClientNameLocal = param.dtBillingTargetForView.FullNameLC;
                    ViewBag.txtBillingClientAddressLocal = param.dtBillingTargetForView.AddressLC;
                }
                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial BillingTarget Grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS400_InitialBillingTargetGrid()
        {
            //return Json(CommonUtil.ConvertToXml<dtTbt_BillingTargetForView>(null, "Common\\CMS400_BillingTarget",CommonUtil.GRID_EMPTY_TYPE.SEARCH));
            return Json(CommonUtil.ConvertToXml<doBillingTargetList>(null, "Common\\CMS400_BillingTarget", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial Billing Basic Grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS400_InitialBillingBasicGrid()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS400_ScreenParameter param = GetScreenObject<CMS400_ScreenParameter>();
                return Json(CommonUtil.ConvertToXml<dtViewBillingBasicList>(param.doBasicList, "Common\\CMS400_BillingBasic", CommonUtil.GRID_EMPTY_TYPE.VIEW));
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Initial Uncancel Invoice Grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS400_InitialUncancelInvoiceGrid()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS400_ScreenParameter param = GetScreenObject<CMS400_ScreenParameter>();
                return Json(CommonUtil.ConvertToXml<dtViewBillingInvoiceListOfLastInvoiceOcc>(param.dtViewBillingInvoiceListOfLastInvoiceOccList, "Common\\CMS400_UncancelInvoice", CommonUtil.GRID_EMPTY_TYPE.VIEW));
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Search Data
        /// </summary>
        /// <param name="strBillingClientCode"></param>
        /// <param name="strBillingTargetNo"></param>
        /// <param name="strBillingClientName"></param>
        /// <param name="strBillingClientAddress"></param>
        /// <param name="strBillingClientCode2"></param>
        /// <param name="Flag"></param>
        /// <param name="strInvoiceNo"></param>
        /// <returns></returns>
        public ActionResult CMS400_SearchDataToGrid(string strBillingClientCode, string strBillingTargetNo,
                            string strBillingClientName, string strBillingClientAddress, string strBillingClientCode2, int Flag, string strInvoiceNo, string strTaxIDNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // Check validate control


                CMS400_ScreenParameter param = GetScreenObject<CMS400_ScreenParameter>();
                CommonUtil cm = new CommonUtil();
                string strBillingTargetCode = null;
                IViewBillingHandler handlerViewBilling = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;

                if (!CommonUtil.IsNullOrEmpty(strBillingClientCode2) || !CommonUtil.IsNullOrEmpty(strBillingTargetNo))
                {
                    strBillingTargetCode = cm.ConvertBillingClientCode(strBillingClientCode2, CommonUtil.CONVERT_TYPE.TO_LONG) + "-" + strBillingTargetNo;
                }
                else
                {
                    strBillingTargetCode = null;
                }
                if (CommonUtil.IsNullOrEmpty(strBillingClientCode))
                {
                    strBillingClientCode = null;
                }
                else
                {
                    strBillingClientCode = cm.ConvertBillingClientCode(strBillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                }
                if (CommonUtil.IsNullOrEmpty(strBillingClientName))
                {
                    strBillingClientName = null;
                }
                if (CommonUtil.IsNullOrEmpty(strBillingClientAddress))
                {
                    strBillingClientAddress = null;
                }
                if (CommonUtil.IsNullOrEmpty(strInvoiceNo))
                {
                    strInvoiceNo = null;
                }
                if (CommonUtil.IsNullOrEmpty(strTaxIDNo))
                {
                    strTaxIDNo = null;
                }
                if (Flag == 0)
                {
                    List<doBillingTargetList> lst = handlerViewBilling.GetViewBillingTargetList(
                                               strBillingClientCode,
                                               strBillingTargetCode,
                                               strBillingClientName,
                                               strBillingClientAddress,
                                               strInvoiceNo,
                                               strTaxIDNo
                                               );
                    res.ResultData = CommonUtil.ConvertToXml<doBillingTargetList>(lst, "Common\\CMS400_BillingTarget", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                }
                else if (Flag == 1)
                {
                    List<dtViewBillingBasicList> lst = handlerViewBilling.GetViewBillingBasicList(
                                               strBillingClientCode,
                                               strBillingTargetCode,
                                               strBillingClientName,
                                               strBillingClientAddress,
                                               strInvoiceNo,
                                               strTaxIDNo
                                               );
                    res.ResultData = CommonUtil.ConvertToXml<dtViewBillingBasicList>(lst, "Common\\CMS400_BillingBasic", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                }
                else if (Flag == 2)
                {
                    if (CommonUtil.IsNullOrEmpty(strInvoiceNo))
                    {
                        strInvoiceNo = null;
                    }
                    List<dtViewBillingInvoiceListOfLastInvoiceOcc> lst = handlerViewBilling.GetViewBillingInvoiceListOfLastInvoiceOcc(
                                              strBillingClientCode,
                                              strBillingTargetCode,
                                              strBillingClientName,
                                              strBillingClientAddress,
                                              strInvoiceNo,
                                               strTaxIDNo
                                              );
                    //lst[0].NoOfBillingDetail = 2500;
                    res.ResultData = CommonUtil.ConvertToXml<dtViewBillingInvoiceListOfLastInvoiceOcc>(lst, "Common\\CMS400_UncancelInvoice", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get View Billin gBasic 
        /// </summary>
        /// <param name="strBillingTargetCode"></param>
        /// <returns></returns>
        public ActionResult CMS400_GetViewBillingBasicForGrid(string strBillingTargetCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CommonUtil cm = new CommonUtil();
                IViewBillingHandler handlerViewBilling = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                if (!CommonUtil.IsNullOrEmpty(strBillingTargetCode))
                {
                    strBillingTargetCode = cm.ConvertBillingTargetCode(strBillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                }

                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtViewBillingBasicList> lst = handlerViewBilling.GetViewBillingBasicList(
                                            null,
                                            strBillingTargetCode,
                                            null,
                                            null,
                                            null,
                                            null
                                            );

                for(int i=0; i<lst.Count(); i++)
                {
                    lst[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                res.ResultData = CommonUtil.ConvertToXml<dtViewBillingBasicList>(lst, "Common\\CMS400_BillingBasic", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
            return Json(res);
        }

        /// <summary>
        /// Validate input screen of CMS400
        /// </summary>
        /// <param name="strBillingClientCode"></param>
        /// <param name="strBillingTargetNo"></param>
        /// <param name="strBillingClientName"></param>
        /// <param name="strBillingClientAddress"></param>
        /// <param name="strBillingClientCode2"></param>
        /// <param name="strInvoiceNo"></param>
        /// <returns></returns>
        public ActionResult CMS400_ValidateControl(string strBillingClientCode, string strBillingTargetNo,
                            string strBillingClientName, string strBillingClientAddress, string strBillingClientCode2, string strInvoiceNo, string strTaxIDNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (CommonUtil.IsNullOrEmpty(strBillingClientCode) && CommonUtil.IsNullOrEmpty(strBillingTargetNo)
                    && CommonUtil.IsNullOrEmpty(strBillingClientName) && CommonUtil.IsNullOrEmpty(strBillingClientAddress)
                    && CommonUtil.IsNullOrEmpty(strBillingClientCode2) && CommonUtil.IsNullOrEmpty(strInvoiceNo)
                    && CommonUtil.IsNullOrEmpty(strTaxIDNo)
                    )
                {

                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                     ScreenID.C_SCREEN_ID_SEARCH_BILLING_INFORMATION,
                                     MessageUtil.MODULE_COMMON,
                                     MessageUtil.MessageList.MSG0006);
                    res.ResultData = false;                   
                }
                else
                {
                    res.ResultData = true;
                }
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }        
        }
    }
}
