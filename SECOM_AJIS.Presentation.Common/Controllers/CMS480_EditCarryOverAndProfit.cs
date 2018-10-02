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
using System.Transactions;
using SECOM_AJIS.Presentation.Common.Helpers;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of CMS480
        /// </summary>
        /// <param name="param">Screen parameter</param>
        /// <returns></returns>
        public ActionResult CMS480_Authority(CMS480_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // - Check permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_CARRY_OVER_AND_PROFIT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }                                             
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS480_ScreenParameter>("CMS480", param, res);
        }

        /// <summary>
        /// Initialize screen CMS480
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS480")]
        public ActionResult CMS480()
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil cm = new CommonUtil();
            try
            {
                CMS480_ScreenParameter param = GetScreenObject<CMS480_ScreenParameter>();

                // Prepare for show section                   
                ViewBag.txtCallerScreenId = param.CallerScreenID;

                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }


        /// <summary>
        /// Get data for initialize search result grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for initialize search result grid.</returns>
        public ActionResult CMS480_InitialSearchGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS480_CarryOverProfit", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Search carry over and profit data.
        /// </summary>
        /// <param name="param">DO of searchig parameter.</param>
        /// <returns>Return ActionResult of JSON data for carry over and profit grid.</returns>
        public ActionResult CMS480_SearchManageCarryOverProfit(doGetManageCarryOverProfitCriteria param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {

                if (ModelState.IsValid == false)
                {
                    res.ResultData = false;
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }
                else
                {
                    CommonUtil com = new CommonUtil();
                    ICommonHandler service = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    var lst = service.GetManageCarryOverProfitForEdit(param.ReportYear, param.ReportMonth, param.ProductType, com.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.BillingOCC);

                    //res.ResultData = CommonUtil.ConvertToXml<doResultManageCarryOverProfitForEdit>(lst, "Common\\CMS480_CarryOverProfit", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                    res.ResultData = CMS480_SearchManageCarryOverProfit_CreateResult(true, lst);
                }

                //List<string> requiredField = new List<string>();

                //if (string.IsNullOrEmpty(param.ProductType))
                //    requiredField.Add("Product Type");

                //if (string.IsNullOrEmpty(param.ReportMonth))
                //    requiredField.Add("Month");

                //if (string.IsNullOrEmpty(param.ReportYear))
                //    requiredField.Add("Year");


                //if (requiredField.Count == 0)
                //{
                //    CommonUtil com = new CommonUtil();
                //    ICommonHandler service = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //    var lst = service.GetManageCarryOverProfitForEdit(param.ReportYear, param.ReportMonth, param.ProductType, com.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.BillingOCC);

                //    //res.ResultData = CommonUtil.ConvertToXml<doResultManageCarryOverProfitForEdit>(lst, "Common\\CMS480_CarryOverProfit", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                //    res.ResultData = CMS480_SearchManageCarryOverProfit_CreateResult(true, lst);
                //}
                //else
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { string.Join(", ", requiredField) });
                //    res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(false);
                //    return Json(res);
                //}
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                res.ResultData = CMS480_SearchManageCarryOverProfit_CreateResult(false, null);
                //res.ResultData = new
                //{
                //    Xml = "",
                //    ResultData = CMS480_SearchManageCarryOverProfit_CreateResult(false, null)
                //};
            }

            return Json(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS480_ValidateManageCarryOverProfit(List<CMS480_ManageCarryOverProfit_Param> param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (param == null && param.Count == 0)
                    throw new ArgumentNullException("param", "\"Manage carry over profit\" data can't be empty");

                foreach (CMS480_ManageCarryOverProfit_Param item in param)
                {
                    string reportMonth = !string.IsNullOrEmpty(item.ReportMonthYear) ? item.ReportMonthYear.Split('/')[0] : null;
                    string reportYear = !string.IsNullOrEmpty(item.ReportMonthYear) ? item.ReportMonthYear.Split('/')[1] : null;
                    string contractCode = !string.IsNullOrEmpty(item.BillingCode) ? item.BillingCode.Split('-')[0] : null;
                    string billingOCC = !string.IsNullOrEmpty(item.BillingCode) ? item.BillingCode.Split('-')[1] : null;


                    if (item.ReceiveAmount == null)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                        res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(false);
                        return Json(res);
                    }

                    if (item.IncomeRentalFee == null)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                        res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(false);
                        return Json(res);
                    }

                    if (item.AccumulatedReceiveAmount == null)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                        res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(false);
                        return Json(res);
                    }

                    if (item.AccumulatedUnpaid == null)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                        res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(false);
                        return Json(res);
                    }

                    if (item.IncomeVat == null)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                        res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(false);
                        return Json(res);
                    }

                    if (item.UnpaidPeriod == null)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                        res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(false);
                        return Json(res);
                    }

                    //if (item.IncomeDate == null)
                    //{
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    //    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    //    res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(false);
                    //    return Json(res);
                    //}
                }

                if (res.MessageList != null)
                {
                    res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(false);
                }
                else
                {
                    res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(true);
                }

                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(false);
                return Json(res);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS480_UpdateManageCarryOverProfit(List<CMS480_ManageCarryOverProfit_Param> param, string productTypeCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                if (param == null && param.Count == 0)
                    throw new ArgumentNullException("param", "\"Manage carry over profit\" data can't be empty");

                CommonUtil com = new CommonUtil();

                using (TransactionScope trans = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    foreach (CMS480_ManageCarryOverProfit_Param item in param)
                    {
                        string reportMonth = !string.IsNullOrEmpty(item.ReportMonthYear) ? item.ReportMonthYear.Split('/')[0] : null;
                        string reportYear = !string.IsNullOrEmpty(item.ReportMonthYear) ? item.ReportMonthYear.Split('/')[1] : null;
                        string contractCode = !string.IsNullOrEmpty(item.BillingCode) ? item.BillingCode.Split('-')[0] : null;
                        string billingOCC = !string.IsNullOrEmpty(item.BillingCode) ? item.BillingCode.Split('-')[1] : null;

                        if(item.ReceiveAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            item.ReceiveAmountUsd = item.ReceiveAmount;
                            item.ReceiveAmount = null;
                        }
                        if (item.IncomeRentalFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            //item.IncomeRentalFeeUsd = item.ReceiveAmount;
                            item.IncomeRentalFeeUsd = item.IncomeRentalFee;
                            item.IncomeRentalFee = null;
                        }
                        if (item.AccumulatedReceiveAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            // item.AccumulatedReceiveAmountUsd = item.ReceiveAmount;
                            item.AccumulatedReceiveAmountUsd = item.AccumulatedReceiveAmount;
                            item.AccumulatedReceiveAmount = null;
                        }
                        if (item.AccumulatedUnpaidCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            // item.AccumulatedUnpaidUsd = item.ReceiveAmount;
                            item.AccumulatedUnpaidUsd = item.AccumulatedUnpaid;
                            item.AccumulatedUnpaid = null;
                        }
                        if (item.IncomeVatCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                           // item.IncomeVatUsd = item.ReceiveAmount;
                            item.IncomeVatUsd = item.IncomeVat;
                            item.IncomeVat = null;
                        }

                        ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        srvCommon.UpdateTbtManageCarryOverProfit(reportYear, reportMonth, com.ConvertContractCode(contractCode, CommonUtil.CONVERT_TYPE.TO_LONG), billingOCC,
                            item.ReceiveAmount, item.ReceiveAmountUsd, item.ReceiveAmountCurrencyType,
                            item.IncomeRentalFee,item.IncomeRentalFeeUsd, item.IncomeRentalFeeCurrencyType,
                            item.AccumulatedReceiveAmount, item.AccumulatedReceiveAmountUsd, item.AccumulatedReceiveAmountCurrencyType,
                            item.AccumulatedUnpaid,item.AccumulatedUnpaidUsd, item.AccumulatedUnpaidCurrencyType,
                            item.IncomeVat, item.IncomeVatUsd, item.IncomeVatCurrencyType, item.UnpaidPeriod, item.IncomeDate, CommonUtil.dsTransData.dtUserData.EmpNo);

                    }

                    trans.Complete();
                    this.Dispose();
                }

                // - Generate CSV Files -
                string paramReportMonth = param[0].ReportMonthYear.Split('/')[0];
                string paramReportYear = param[0].ReportMonthYear.Split('/')[1];

                IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                documentHandler.GenerateCMR020FilePath(paramReportYear, paramReportMonth, productTypeCode, ProcessID.C_PROCESS_ID_GEN_ACCOUNT_CARRY_OVER_AND_PROFIT, DateTime.Now, false);

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(true);

                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                res.ResultData = this.CMS480_UpdateManageCarryOverProfit_CreateResult(false);
                return Json(res);
            }

        }

        public ActionResult CMS480_GetMessage()
        {
            ObjectResultData res = new ObjectResultData();
            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0052, CommonValue.MAX_GRID_ROWS.ToString("N0"));
            return Json(res);
        }

        private object CMS480_SearchManageCarryOverProfit_CreateResult(bool bIsSuccess, List<doResultManageCarryOverProfitForEdit> lstResultData)
        {
            return new
            {
                IsSuccess = bIsSuccess,
                ResultData = lstResultData
            };
        }
        private object CMS480_UpdateManageCarryOverProfit_CreateResult(bool bIsSuccess)
        {
            return new
            {
                IsSuccess = bIsSuccess
            };
        }

        public ActionResult CMS480_GenerateCurrencyNumericTextBox(string id, string currency, string textboxValue, bool enable)
        {
            string html = "";

            if (enable)
            {
                html = TextBoxHelper.NumericTextBoxWithMultipleCurrency(null, id, textboxValue, currency, new { style = "width: 140px;" }).ToString();
            }
            else
            {
                html = TextBoxHelper.NumericTextBoxWithMultipleCurrency(null, id, textboxValue, currency, new { style = "width: 140px;", @readonly = true }).ToString();
            }
            return Json(html);
        }
    }
}
