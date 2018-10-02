using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.ActionFilters;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Common.Models;
using SECOM_AJIS.DataEntity.Billing;
using System.IO;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of CMS450
        /// </summary>
        /// <param name="param">Screen parameter</param>
        /// <returns></returns>
        public ActionResult CMS450_Authority(CMS450_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

                // Check permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_BILLING_DETAIL, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                // is parameter OK ?
                if (CommonUtil.IsNullOrEmpty(param.BillingTargetCode)
                    && CommonUtil.IsNullOrEmpty(param.ContractCode)
                    && CommonUtil.IsNullOrEmpty(param.BillingOCC)
                    && CommonUtil.IsNullOrEmpty(param.InvoiceNo))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0156);
                    return Json(res);
                }

                // data is exist ?

                CommonUtil cm = new CommonUtil();
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                IViewBillingHandler handlerViewBilling = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;

                List<dtTbt_BillingTargetForView> listBillingTarget = new List<dtTbt_BillingTargetForView>();
                if (!CommonUtil.IsNullOrEmpty(param.BillingTargetCode))
                {
                    listBillingTarget = handlerBilling.GetTbt_BillingTargetForView(cm.ConvertBillingTargetCode(param.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG), MiscType.C_CUST_TYPE);

                    if (listBillingTarget.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                        return Json(res);
                    }
                }
                else if (!CommonUtil.IsNullOrEmpty(param.BillingOCC) && !CommonUtil.IsNullOrEmpty(param.ContractCode))
                {
                    //GetBillingDetailOfBillingCode
                    string strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    List<dtTbt_BillingBasicForView> listBillingBasicForView = handlerBilling.GetTbt_BillingBasicForView(strContractCode, param.BillingOCC);
                    if (listBillingBasicForView.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                        return Json(res);
                    }
                }
                else if (!CommonUtil.IsNullOrEmpty(param.InvoiceNo))
                {
                    List<dtViewBillingDetailListOfLastInvoiceOCC> listBillingDetailListOfLastInvoiceOCC = handlerViewBilling.GetViewBillingDetailListOfLastInvoiceOCC(param.InvoiceNo, null, null, null, null);
                    if (listBillingDetailListOfLastInvoiceOCC.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                        return Json(res);
                    }
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0156);
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS450_ScreenParameter>("CMS450", param, res);
        }

        /// <summary>
        /// Initialize screen of CMS450
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS450")]        
        public ActionResult CMS450()
        {
            ObjectResultData res = new ObjectResultData();
            CMS450_ScreenParameter param = GetScreenObject<CMS450_ScreenParameter>();
            CommonUtil cm = new CommonUtil();

            // Prepare for show section
            ViewBag._ContractCode = param.ContractCode;
            ViewBag._BillingOCC = param.BillingOCC;
            ViewBag._BillingTargetCode = param.BillingTargetCode;
            ViewBag._InvoiceNo = param.InvoiceNo;

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                IViewBillingHandler handlerViewBilling = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;

                if (!CommonUtil.IsNullOrEmpty(param.BillingTargetCode))
                {

                    List<dtTbt_BillingTargetForView> listBillingTarget = handlerBilling.GetTbt_BillingTargetForView(cm.ConvertBillingTargetCode(param.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG), MiscType.C_CUST_TYPE);
                    if (listBillingTarget.Count > 0)
                    {
                        ViewBag.txtBillingTargetCode = listBillingTarget[0].BillingTargetCode_Short;
                        ViewBag.txtBillingClientNameEnglish = listBillingTarget[0].FullNameEN;
                        ViewBag.txtBillingClientAddressEnglish = listBillingTarget[0].AddressEN;
                        ViewBag.txtBillingClientNameLocal = listBillingTarget[0].FullNameLC;
                        ViewBag.txtBillingClientAddressLocal = listBillingTarget[0].AddressLC;
                    }

                }
                else if (!CommonUtil.IsNullOrEmpty(param.BillingOCC) && !CommonUtil.IsNullOrEmpty(param.ContractCode))
                {
                    //GetBillingDetailOfBillingCode
                    string strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    List<dtTbt_BillingBasicForView> listBillingBasicForView = handlerBilling.GetTbt_BillingBasicForView(strContractCode, param.BillingOCC);
                    if (listBillingBasicForView.Count > 0)
                    {
                        ViewBag.txtBillingCode = cm.ConvertBillingCode(listBillingBasicForView[0].BillingCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        ViewBag.txtBillingOffice = listBillingBasicForView[0].OfficeName;
                        ViewBag.txtBillingTargetCode = cm.ConvertBillingTargetCode(listBillingBasicForView[0].BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        ViewBag.txtBillingClientNameEnglish = listBillingBasicForView[0].FullNameEN;
                        ViewBag.txtBillingClientAddressEnglish = listBillingBasicForView[0].AddressEN;
                        ViewBag.txtBillingClientNameLocal = listBillingBasicForView[0].FullNameLC;
                        ViewBag.txtBillingClientAddressLocal = listBillingBasicForView[0].AddressLC;
                    }
                }
                else if (!CommonUtil.IsNullOrEmpty(param.InvoiceNo))
                {
                    List<dtViewBillingDetailListOfLastInvoiceOCC> listBillingDetailListOfLastInvoiceOCC = handlerViewBilling.GetViewBillingDetailListOfLastInvoiceOCC(param.InvoiceNo, null, null, null, null);
                    if (listBillingDetailListOfLastInvoiceOCC.Count > 0)
                    {
                        ViewBag.txtInvoiceNo = listBillingDetailListOfLastInvoiceOCC[0].InvoiceNo;
                        ViewBag.txtBillingTargetCode = cm.ConvertBillingTargetCode(listBillingDetailListOfLastInvoiceOCC[0].BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        ViewBag.txtBillingClientNameEnglish = listBillingDetailListOfLastInvoiceOCC[0].FullNameEN;
                        ViewBag.txtBillingClientAddressEnglish = listBillingDetailListOfLastInvoiceOCC[0].AddressEN;
                        ViewBag.txtBillingClientNameLocal = listBillingDetailListOfLastInvoiceOCC[0].FullNameLC;
                        ViewBag.txtBillingClientAddressLocal = listBillingDetailListOfLastInvoiceOCC[0].AddressLC;
                    }
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return View();
        }

        /// <summary>
        /// Get billing detail list
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS450_ViewBillingDetailList()
        {
            ObjectResultData res = new ObjectResultData();
            List<dtViewBillingDetailList> list = new List<dtViewBillingDetailList>();
            
            try
            {
                CMS450_ScreenParameter param = GetScreenObject<CMS450_ScreenParameter>();
                CommonUtil cm = new CommonUtil();
                string strContractCode = cm.ConvertContractCode(param.ContractCode,CommonUtil.CONVERT_TYPE.TO_LONG);
                IViewBillingHandler handlerViewBilling = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                if (!CommonUtil.IsNullOrEmpty(param.BillingTargetCode))
                {
                    list = handlerViewBilling.GetViewBillingDetailListByTargetCode(cm.ConvertBillingTargetCode(param.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                }
                else if (!CommonUtil.IsNullOrEmpty(param.BillingOCC) && !CommonUtil.IsNullOrEmpty(param.ContractCode))
                {
                    list = handlerViewBilling.GetViewBillingDetailList(strContractCode, param.BillingOCC);
                }
                else if (!CommonUtil.IsNullOrEmpty(param.InvoiceNo))
                {
                    List<dtViewBillingDetailListOfLastInvoiceOCC> listBillingDetail = handlerViewBilling.GetViewBillingDetailListOfLastInvoiceOCC(param.InvoiceNo, null, null, null, null);
                    for (int i = 0; i < listBillingDetail.Count(); i++)
                    {
                        listBillingDetail[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }
                    if (listBillingDetail.Count > 0)
                    {                         
                        List<dtViewBillingDetailList> lst = CommonUtil.ClonsObjectList<dtViewBillingDetailListOfLastInvoiceOCC,dtViewBillingDetailList>(listBillingDetail);
                        res.ResultData = CommonUtil.ConvertToXml<dtViewBillingDetailList>(lst, "Common\\CMS450_ViewBillingDetailInformation", CommonUtil.GRID_EMPTY_TYPE.VIEW);
                        return Json(res);
                    }                                                        
                }
                for (int i = 0; i < list.Count(); i++)
                {
                    list[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            // test 
            //list.RemoveRange(0, 670);
            res.ResultData = CommonUtil.ConvertToXml<dtViewBillingDetailList>(list, "Common\\CMS450_ViewBillingDetailInformation", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(res);
        }


        /// <summary>
        /// Check exist file before download file
        /// </summary>
        /// <param name="strDocumentNo"></param>
        /// <param name="documentOCC"></param>
        /// <param name="strDocumentCode"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult CMS450_CheckExistFile(string reportId, string docNo)
        {
            IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            try
            {
                List<dtDocumentData> list = handler.GetDocumentDataListByDocumentCode(docNo, reportId, null);
                if (list != null && list.Count > 0)
                {
                    string path = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, list[0].FilePath);// ReportUtil.GetGeneratedReportPath(fileName);

                    if (System.IO.File.Exists(path) == true)
                    {
                        return Json(1);
                    }
                    else
                    {
                        return Json(0);
                    }
                }
                else
                {
                    return Json(0);
                }

            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Download document (PDF)
        /// </summary>
        /// <param name="inventorySlipNo"></param>
        /// <returns></returns>
        public ActionResult CMS450_DownloadDocument(string reportId, string docNo)
        {
            ObjectResultData res = new ObjectResultData();
            IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            try
            {
                List<dtDocumentData> list = handler.GetDocumentDataListByDocumentCode(docNo, reportId, null);
                if (list != null && list.Count > 0)
                {
                    Stream reportStream = handler.GetDocumentReportFileStream(list[0].FilePath);
                    return File(reportStream, "application/octet-stream", Path.GetFileName(list[0].FilePath));
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

       
    }
}
