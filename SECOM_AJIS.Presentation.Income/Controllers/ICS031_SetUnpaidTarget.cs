
//*********************************
// Create by: Waroon H.
// Create date: 29/Mar/2012
// Update date: 29/Mar/2012
//*********************************



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Transactions;
using SECOM_AJIS.Presentation.Income.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using System.ComponentModel;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using SECOM_AJIS.Presentation.Common;
using SECOM_AJIS.Presentation.Common.Helpers;

namespace SECOM_AJIS.Presentation.Income.Controllers
{
    public partial class IncomeController : BaseController
    {
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS031_Authority(ICS031_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();
            // System Suspend
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            if (handlerCommon.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            // Permission
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_SET_UNPAID_TARGET, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            return InitialScreenEnvironment<ICS031_ScreenParameter>("ICS031", param, res);

        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS031")]
        public ActionResult ICS031()
        {
            ViewBag.currencyLocal = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_LOCAL);
            ViewBag.currencyUs = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_US);

            ICS031_ScreenParameter param = GetScreenObject<ICS031_ScreenParameter>();
            return View();

        }

        /// <summary>
        /// Generate xml for initial set unpaid target grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS031_InitialSetUnpaidTargetGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS031_SetUnpaidTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Retrieve dept target information list of specific screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria</param>
        /// <returns></returns>
        public ActionResult ICS031_SearchData(ICS031_RegisterData data)
        {

            ICS031_ScreenParameter param = GetScreenObject<ICS031_ScreenParameter>();
            ICS031_RegisterData RegisterData = new ICS031_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<doGetDebtTarget> _doGetDebtTargetList = new List<doGetDebtTarget>();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ICS031_ScreenParameter sParam = GetScreenObject<ICS031_ScreenParameter>();

                _doGetDebtTargetList = iincomeHandler.GetDebtTarget();


                // add by Jirawat Jannet @ 2016-10-13
                #region Initial detail input datas for  bindinh to data grid

                string currencyLocal = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_LOCAL);
                string currencyUs = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_US);

                List<ICS031_DetailData> _detailInputDatas = new List<ICS031_DetailData>();
                int no = 1;
                if (_doGetDebtTargetList != null)
                {
                    foreach (var item in _doGetDebtTargetList)
                    {
                        // local currency
                        _detailInputDatas.Add(new ICS031_DetailData()
                        {
                            No = no,
                            BillingOfficeCode = item.BillingOfficeCode,
                            BillingOfficeName = item.OfficeName,
                            AllUnpaidTargetAmount = item.AmountAll,
                            AllUnpaidTargetBillingDetail = item.DetailAll,
                            UnpaidOverTargetAmount = item.Amount2Month,
                            UnpaidOverBillingDetail = item.Detail2Month,
                            CurrencyType = CurrencyUtil.C_CURRENCY_LOCAL,
                            CurrencyTypeName = currencyLocal
                        });
                        // us currency
                        _detailInputDatas.Add(new ICS031_DetailData()
                        {
                            No = no,
                            BillingOfficeCode = item.BillingOfficeCode,
                            BillingOfficeName = item.OfficeName,
                            AllUnpaidTargetAmount = item.AmountAllUsd,
                            AllUnpaidTargetBillingDetail = item.DetailAllUsd,
                            UnpaidOverTargetAmount = item.Amount2MonthUsd,
                            UnpaidOverBillingDetail = item.Detail2MonthUsd,
                            CurrencyType = CurrencyUtil.C_CURRENCY_US,
                            CurrencyTypeName = currencyUs
                        });
                        no++;
                    }
                }

                #endregion

                param.RegisterData = data;
                param.doGetDebtTargetList = _doGetDebtTargetList;
                param.detailInputDatas = _detailInputDatas;

                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = param; }
                else
                { res.ResultData = null; }

                return Json(res);

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Event when click register button 
        /// </summary>
        /// <param name="data">screen input information</param>
        /// <returns></returns>
        public ActionResult ICS031_Register(ICS031_RegisterData data)
        {

            ICS031_ScreenParameter param = GetScreenObject<ICS031_ScreenParameter>();
            ICS031_RegisterData RegisterData = new ICS031_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ObjectResultData resByIssue = new ObjectResultData();
            ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = "1"; }
                else
                { res.ResultData = "0"; }

                return Json(res);

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// validate input data confirm and register data into database
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS031_Confirm()
        {
            ICS031_ScreenParameter param = GetScreenObject<ICS031_ScreenParameter>();
            ICS031_RegisterData RegisterData = new ICS031_RegisterData();
            CommonUtil comUtil = new CommonUtil();
            // reuse param that send on Register Click
            if (param != null)
            {
                RegisterData = param.RegisterData;
            }

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ObjectResultData resByIssue = new ObjectResultData();
            ObjectResultData resByInvoice = new ObjectResultData();

            tbt_DebtTarget doTbt_DebtTarget = new tbt_DebtTarget();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }


                 using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                    #region tbt_DebtTarget
                    // Comment by Jirawat Jannet @ 2016-10-13
                    //for (int i = 0; i < RegisterData.Detail1.Count; i++)
                    //{
                    //    doTbt_DebtTarget = new tbt_DebtTarget();
                    //    doTbt_DebtTarget.BillingOfficeCode = RegisterData.Detail1[i].txtBillingOfficeCode;

                    //    doTbt_DebtTarget.AmountAll = (decimal?)Convert.ToDecimal(RegisterData.Detail1[i].txtAmountAll);
                    //    doTbt_DebtTarget.DetailAll = (int?)Convert.ToInt32(RegisterData.Detail1[i].txtDetailAll);
                    //    doTbt_DebtTarget.Amount2Month = (decimal?)Convert.ToDecimal(RegisterData.Detail1[i].txtAmount2Month);
                    //    doTbt_DebtTarget.Detail2Month = (int?)Convert.ToInt32(RegisterData.Detail1[i].txtDetail2Month);
                    //    if (iincomeHandler.UpdateTbt_DebtTarget(doTbt_DebtTarget) == 0)
                    //    {
                    //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7121, null);
                    //    }
                    //}
                    #endregion

                    // add by Jirawat jannet @ 2016-10-13
                    #region tbt_DebtTarget edit by Jirawat Jannet

                    string currencyLocal = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_LOCAL);
                    string currencyUs = MiscellaneousTypeCommon.getCurrencyName(CurrencyUtil.C_CURRENCY_US);

                    foreach (var g in RegisterData.Detail1.GroupBy(m => m.txtBillingOfficeCode).Select(m => m.Key))
                    {
                        var local = RegisterData.Detail1.Where(m => m.txtBillingOfficeCode == g && m.txtCurrency == currencyLocal).First();
                        var us = RegisterData.Detail1.Where(m => m.txtBillingOfficeCode == g && m.txtCurrency == currencyUs).First();

                        doTbt_DebtTarget = new tbt_DebtTarget();
                        doTbt_DebtTarget.BillingOfficeCode = local.txtBillingOfficeCode;

                        doTbt_DebtTarget.AmountAll = (decimal?)Convert.ToDecimal(local.txtAmountAll);
                        doTbt_DebtTarget.DetailAll = (int?)Convert.ToInt32(local.txtDetailAll);
                        doTbt_DebtTarget.Amount2Month = (decimal?)Convert.ToDecimal(local.txtAmount2Month);
                        doTbt_DebtTarget.Detail2Month = (int?)Convert.ToInt32(local.txtDetail2Month);

                        doTbt_DebtTarget.AmountAllUsd = (decimal?)Convert.ToDecimal(us.txtAmountAll);
                        doTbt_DebtTarget.DetailAllUsd = (int?)Convert.ToInt32(us.txtDetailAll);
                        doTbt_DebtTarget.Amount2MonthUsd = (decimal?)Convert.ToDecimal(us.txtAmount2Month);
                        doTbt_DebtTarget.Detail2MonthUsd = (int?)Convert.ToInt32(us.txtDetail2Month);

                        if (iincomeHandler.UpdateTbt_DebtTarget(doTbt_DebtTarget) == 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7121, null);
                        }
                    }

                    #endregion



                    scope.Complete();
                }
                catch (Exception ex)
                {
                    // Fail rollback all record
                    scope.Dispose();
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(ex);
                    return Json(res);
                }
                }


                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                {
                    res.ResultData = "1";
                }
                else
                {
                    res.ResultData = "0";
                }
                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        public ActionResult ICS031_GenerateCurrencyNumericTextBox(string id, string currency)
        {
            string html = TextBoxHelper.NumericTextBoxWithMultipleCurrency(null, id, 0, currency, new { style = "width: 140px;" }).ToString();
            return Json(html);
        }
    }
}