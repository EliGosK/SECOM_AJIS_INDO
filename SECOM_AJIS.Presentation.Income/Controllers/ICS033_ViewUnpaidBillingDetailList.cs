//*********************************
// Create by: Waroon H.
// Create date: 04/Apr/2012
// Update date: 04/Apr/2012
//*********************************

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Presentation.Income.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Linq;
using SECOM_AJIS.Presentation.Common;

namespace SECOM_AJIS.Presentation.Income.Controllers
{
    public partial class IncomeController : BaseController
    {
        #region Authority
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS033_Authority(ICS033_ScreenParameter inputparam)
        {

            ObjectResultData res = new ObjectResultData();
            ICS033_ScreenParameter param = GetScreenObject<ICS033_ScreenParameter>();

            param = inputparam;
            return InitialScreenEnvironment<ICS033_ScreenParameter>("ICS033", param, res);

        }
        #endregion

        #region View
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS033")]
        public ActionResult ICS033()
        {
            ICS033_ScreenParameter param = GetScreenObject<ICS033_ScreenParameter>();
            if (param != null)
            {
                #region Billing detail
                ViewBag.BillingOfficeCode = param.BillingOfficeCode;
                ViewBag.BillingOfficeName = param.BillingOfficeName;
                ViewBag.BillingClientNameEN = param.BillingClientNameEN;
                ViewBag.BillingClientNameLC = param.BillingClientNameLC;
                ViewBag.BillingClientAddressEN = param.BillingClientAddressEN;
                ViewBag.BillingClientAddressLC = param.BillingClientAddressLC;
                ViewBag.BillingClientTelNo = param.BillingClientTelNo;
                ViewBag.ContactPersonName = param.ContactPersonName;
                

                ViewBag.Mode = param.Mode;

                ViewBag.BillingTargetCode = param.BillingTargetCode;
                ViewBag.InvoiceNo = param.InvoiceNo;
                ViewBag.InvoiceOCC = param.InvoiceOCC;
                ViewBag.BillingCode = param.BillingCode;
                #endregion

                #region Unpaid billing detail list
                #region Get data
                IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                if (param.Mode == ICS033_ScreenCallerMode.GetByBillingTarget)
                {
                    string billingTargetCodeLongFormat = new CommonUtil().ConvertBillingTargetCode(param.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    param.UnpaidDetailDebtSummary = incomeHandler.GetUnpaidDetailDebtSummaryByBillingTargetList(billingTargetCodeLongFormat);
                }
                else if (param.Mode == ICS033_ScreenCallerMode.GetByInvoice)
                {
                    param.UnpaidDetailDebtSummary = incomeHandler.GetUnpaidDetailDebtSummaryByInvoiceList(param.InvoiceNo, param.InvoiceOCC);
                }
                else if (param.Mode == ICS033_ScreenCallerMode.GetByBillingCode)
                {
                    param.UnpaidDetailDebtSummary = incomeHandler.GetUnpaidDetailDebtSummaryByBillingCodeList(param.BillingCode);
                }
                #endregion

                #region Display Yes/No format
                string yesDisplay = string.Empty;
                string noDisplay = string.Empty;
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                    {
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_FLAG_DISPLAY,
                            ValueCode = "%"
                        }
                    };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> lst = hand.GetMiscTypeCodeList(miscs);

                foreach (doMiscTypeCode l in lst)
                {
                    if (l.ValueCode == FlagDisplay.C_FLAG_DISPLAY_NO)
                    {
                        yesDisplay = l.ValueDisplay;
                    }
                    if (l.ValueCode == FlagDisplay.C_FLAG_DISPLAY_YES)
                    {
                        noDisplay = l.ValueDisplay;
                    }
                }

                foreach (doGetUnpaidDetailDebtSummary item in param.UnpaidDetailDebtSummary)
                {
                    if (item.DebtTracingRegistered == 1)
                    {
                        item.DebtTracingRegisteredGridFormat = noDisplay;
                    }
                    else
                    {
                        item.DebtTracingRegisteredGridFormat = yesDisplay;
                    }
                }
                #endregion
                #endregion

                #region Calculate total unpaid
                decimal unpaidAmount = 0;
                decimal unpaidAmountUs = 0;
                if (param.UnpaidDetailDebtSummary != null)
                {
                    unpaidAmount = param.UnpaidDetailDebtSummary.Sum(d => d.BillingAmount).GetValueOrDefault();
                    unpaidAmountUs = param.UnpaidDetailDebtSummary.Sum(d => d.BillingAmountUsd).GetValueOrDefault();
                }
                ViewBag.UnpaidAmountString = unpaidAmount.ToString("N2");
                ViewBag.unpaidAmountUsString = unpaidAmountUs.ToString("N2");
                #endregion

                // add by Jirawat Jannet @ 2016-10-17
                #region Currency Type

                ViewBag.CurrencyTypeLocalName = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_LOCAL);
                ViewBag.CurrencyTypeUsName = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_US);

                #endregion
            }
            return View();
        }
        #endregion

        #region Actions
        /// <summary>
        /// Generate xml for initial view unpaid billing detail list grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS033_InitialViewUnpaidBillingDetailListGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS033_ViewUnpaidBillingDetailList", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Generate xml for invoice list of specific billing target code of screen session
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS033_GetViewUnpaidBillingDetailListGrid()
        {
            List<doGetUnpaidDetailDebtSummary> doUnpaidDetailDebtSummary = new List<doGetUnpaidDetailDebtSummary>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICS033_ScreenParameter param = GetScreenObject<ICS033_ScreenParameter>();
                if (param != null && param.UnpaidDetailDebtSummary != null)
                {
                    doUnpaidDetailDebtSummary = param.UnpaidDetailDebtSummary;
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<doGetUnpaidDetailDebtSummary>(doUnpaidDetailDebtSummary, "Income\\ICS033_ViewUnpaidBillingDetailList", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);
        }
        #endregion   
    }
}