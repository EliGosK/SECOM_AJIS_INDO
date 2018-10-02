
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

namespace SECOM_AJIS.Presentation.Income.Controllers
{
    public partial class IncomeController : BaseController
    {
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS032_Authority(ICS032_ScreenParameter data)
        {

            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

            ICS032_ScreenParameter param = GetScreenObject<ICS032_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();

            try
            {
                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_DEBT_TRACING_INFO, FunctionID.C_FUNC_ID_OPERATE))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                // receive data from ICS030
                param = data;

                if (data.doGetUnpaidInvoiceDebtSummaryByBillingTargetList == null)
                {
                    if (data.strInvoiceNo != null)
                    {
                        data.doGetUnpaidInvoiceDebtSummaryByBillingTargetList = iincomeHandler.GetUnpaidInvoiceDebtSummaryByInvoiceNo(data.strInvoiceNo);
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<ICS032_ScreenParameter>("ICS032", param, res);
        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS032")]
        public ActionResult ICS032()
        {

            ICS032_ScreenParameter param = GetScreenObject<ICS032_ScreenParameter>();
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();

            if (param != null)
            {

                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_FLAG_DISPLAY,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);

                foreach (doMiscTypeCode l in lst)
                {
                    if (l.ValueCode == FlagDisplay.C_FLAG_DISPLAY_NO)
                    {
                        param.conNo = l.ValueDisplay;
                    }
                    if (l.ValueCode == FlagDisplay.C_FLAG_DISPLAY_YES)
                    {
                        param.conYes = l.ValueDisplay;
                    }
                }
            }

            return View();

        }

        /// <summary>
        /// Generate xml for initial money collection management information grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS032_InitialMoneyCollectionManagementInformationGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS032_MoneyCollectionManagementInformation", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Retrieve unpaid details debt summary information list of specific screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria</param>
        /// <returns></returns>
        public ActionResult ICS032_SearchData(ICS032_RegisterData data)
        {
             

            ICS032_ScreenParameter param = GetScreenObject<ICS032_ScreenParameter>();
            ICS032_RegisterData RegisterData = new ICS032_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            string strMemoBillingTargetCode = string.Empty;
            string strMemoInvoiceNo = string.Empty;
            int intMemo = 0;
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

                // Check User Permission
 
                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MONEY_COLLECTION_MANAGEMENT_INFO, FunctionID.C_FUNC_ID_OPERATE) )
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}


                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

                //Add by budd, support multi-language
                IOfficeMasterHandler masterHandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                List<tbm_Office> doTbm_Office =  masterHandler.GetTbm_Office(param.strBillingOfficeCode);

                if (doTbm_Office != null && doTbm_Office.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage<tbm_Office>(doTbm_Office);
                    param.strBillingOfficeName = doTbm_Office[0].OfficeName;
                }
               


                // load GetBillingCodeDeptSummary
                if (param.doGetUnpaidDetailDebtSummaryByBillingCodeList != null)
                {
                    if (param.doGetUnpaidDetailDebtSummaryByBillingCodeList.Count != 0)
                    {
                       param.doGetBillingCodeDebtSummaryList = iincomeHandler.GetBillingCodeDebtSummaryList(
                            param.doGetUnpaidDetailDebtSummaryByBillingCodeList[0].BillingCode);
                    }
                }
                // load GetDebtTracingMemo
                if (param.doBillingTargetDebtSummaryList != null)
                {
                    if (param.doBillingTargetDebtSummaryList.Count != 0)
                    {
                        strMemoBillingTargetCode = param.doBillingTargetDebtSummaryList[0].BillingTargetCode;
                    }
                }

                if (param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList != null)
                {
                    if (param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList.Count != 0)
                    {
                        strMemoInvoiceNo = param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].InvoiceNo;
                        intMemo = param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].InvoiceOCC;
                    }
                }


                param.doGetDebtTracingMemoList = iincomeHandler.GetDebtTracingMemoList(
                    strMemoBillingTargetCode,
                    strMemoInvoiceNo,
                    intMemo );
 
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
        /// Retrieve debt tracing memo history list
        /// </summary>
        /// <param name="data">Search criteria</param>
        /// <returns></returns>
        public ActionResult ICS032_SearchMEMO(ICS032_RegisterData data)
        {

            ICS032_ScreenParameter param = GetScreenObject<ICS032_ScreenParameter>();
            ICS032_RegisterData RegisterData = new ICS032_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            string strMemoBillingTargetCode = string.Empty;
            string strMemoInvoiceNo = string.Empty;
            int intMemo = 0;
            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
 
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

                // load GetDebtTracingMemo
                if (param.doBillingTargetDebtSummaryList != null)
                {
                    if (param.doBillingTargetDebtSummaryList.Count != 0)
                    {
                        strMemoBillingTargetCode = param.doBillingTargetDebtSummaryList[0].BillingTargetCode;
                    }
                }

                if (param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList != null)
                {
                    if (param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList.Count != 0)
                    {
                        strMemoInvoiceNo = param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].InvoiceNo;
                        intMemo = param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].InvoiceOCC;
                    }
                }


                param.doGetDebtTracingMemoList = iincomeHandler.GetDebtTracingMemoList(
                    strMemoBillingTargetCode,
                    strMemoInvoiceNo,
                    intMemo);

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
        public ActionResult ICS032_Register(ICS032_RegisterData data)
        {
 
            string conModeRadio1rdo1Invoice = "1";
            string conModeRadio1rdo1BillingTarget = "2";

            ICS032_ScreenParameter param = GetScreenObject<ICS032_ScreenParameter>();
            ICS032_RegisterData RegisterData = new ICS032_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
 
            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                // null value = select ALL
                
                if (data.Header.cboTracingResault == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS032",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "cboTracingResault", "lblTracingResault", "cboTracingResault");

                    //return Json(res);
                }

                if (String.IsNullOrEmpty(data.Header.dtpLastContractDate.ToString()))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS032",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "dtpLastContractDate", "lblLastContractDate", "dtpLastContractDate");

                    //return Json(res);
                }

                //if (String.IsNullOrEmpty(data.Header.dtpExpectedPaymentdate.ToString()))
                //{
                //    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                //                         "ICS032",
                //                         MessageUtil.MODULE_COMMON,
                //                         MessageUtil.MessageList.MSG0007,
                //                         "dtpExpectedPaymentdate", "lblExpectedPaymentdate", "dtpExpectedPaymentdate");

                //    //return Json(res);
                //}

                //if (data.Header.cboPaymentMethods == null)
                //{
                //    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                //                         "ICS032",
                //                         MessageUtil.MODULE_COMMON,
                //                         MessageUtil.MessageList.MSG0007,
                //                         "cboPaymentMethods", "lblPaymentMethods", "cboPaymentMethods");

                //    //return Json(res);
                //}
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
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
        public ActionResult ICS032_Confirm()
        {
            string conModeRadio1rdo1Invoice = "1";
            string conModeRadio1rdo1BillingTarget = "2";

            ICS032_ScreenParameter param = GetScreenObject<ICS032_ScreenParameter>();
            ICS032_RegisterData RegisterData = new ICS032_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                
                tbt_InvoiceDebtTracing _dotbt_InvoiceDebtTracing = new tbt_InvoiceDebtTracing();
                tbt_BillingTargetDebtTracing _dotbt_BillingTargetDebtTracing = new tbt_BillingTargetDebtTracing();

                //Already checked at ICS032_Register()
                //if (handlerCommon.IsSystemSuspending())
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                //    return Json(res);
                //}

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL) )
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                string strBillingTargetCode = string.Empty;
                string strInvoiceNo = string.Empty;
                int intInvoiceOCC = 0;

                if (param.doBillingTargetDebtSummaryList != null)
                {
                    if (param.doBillingTargetDebtSummaryList.Count != 0)
                    {
                        strBillingTargetCode = param.doBillingTargetDebtSummaryList[0].BillingTargetCode;
                    }
                }

                if (param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList != null)
                {
                    if (param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList.Count != 0)
                    {
                        strInvoiceNo = param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].InvoiceNo;
                        intInvoiceOCC = param.doGetUnpaidInvoiceDebtSummaryByBillingTargetList[0].InvoiceOCC;
                    }
                }

                using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                        if (strInvoiceNo != string.Empty)
                        {
                            if (param.RegisterData.Header.rdoProcessType == conModeRadio1rdo1Invoice)
                            {
                                #region Tbt_InvoiceDebtTracing
                                _dotbt_InvoiceDebtTracing = new tbt_InvoiceDebtTracing();
                                _dotbt_InvoiceDebtTracing.InvoiceNo = strInvoiceNo;
                                _dotbt_InvoiceDebtTracing.InvoiceOCC = intInvoiceOCC;
                                _dotbt_InvoiceDebtTracing.TracingResult = param.RegisterData.Header.cboTracingResault;
                                _dotbt_InvoiceDebtTracing.LastContactDate = param.RegisterData.Header.dtpLastContractDate;
                                _dotbt_InvoiceDebtTracing.ExpectPaymentDate = param.RegisterData.Header.dtpExpectedPaymentdate;
                                _dotbt_InvoiceDebtTracing.PaymentMethod = param.RegisterData.Header.cboPaymentMethods;
                                _dotbt_InvoiceDebtTracing.Memo = param.RegisterData.Header.txtaMemo;

                                if (iincomeHandler.InsertTbt_InvoiceDebtTracing(_dotbt_InvoiceDebtTracing) <= 0)
                                {
                                    //res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                    //             "ICS032",
                                    //             MessageUtil.MODULE_INCOME,
                                    //             MessageUtil.MessageList.MSG7006,
                                    //             new string[] { },
                                    //             new string[] { });
                                    //return Json(res);

                                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7006, null);
                                }
                                #endregion
                            }
                        }
                        if (strBillingTargetCode != string.Empty)
                        {
                            if (param.RegisterData.Header.rdoProcessType == conModeRadio1rdo1BillingTarget)
                            {
                                #region Tbt_BillingTargetDebtTracing
                                _dotbt_BillingTargetDebtTracing = new tbt_BillingTargetDebtTracing();
                                _dotbt_BillingTargetDebtTracing.BillingTargetCode = strBillingTargetCode;
                                _dotbt_BillingTargetDebtTracing.TracingResult = param.RegisterData.Header.cboTracingResault;
                                _dotbt_BillingTargetDebtTracing.LastContactDate = param.RegisterData.Header.dtpLastContractDate;
                                _dotbt_BillingTargetDebtTracing.ExpectPaymentDate = param.RegisterData.Header.dtpExpectedPaymentdate;
                                _dotbt_BillingTargetDebtTracing.PaymentMethod = param.RegisterData.Header.cboPaymentMethods;
                                _dotbt_BillingTargetDebtTracing.Memo = param.RegisterData.Header.txtaMemo;

                                if (iincomeHandler.InsertTbt_BillingTargetDebtTracing(_dotbt_BillingTargetDebtTracing) <= 0)
                                {
                                    //res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                    //             "ICS032",
                                    //             MessageUtil.MODULE_INCOME,
                                    //             MessageUtil.MessageList.MSG7006,
                                    //             new string[] { },
                                    //             new string[] { });
                                    //return Json(res);
                                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7006, null);
                                }
                                #endregion
                            }
                        }
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
                { res.ResultData = "1"; }
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
        /// Retrieve unpaid detail debt summary information for ICS033 popup
        /// </summary>
        /// <param name="data">Search criteria for ICS033 poppup</param>
        /// <returns></returns>
        //public ActionResult ICS033_SearchData(ICS033_ScreenParameter data)
        //{

        //    ICS032_ScreenParameter param = GetScreenObject<ICS032_ScreenParameter>();
        //    ObjectResultData res = new ObjectResultData();
        //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

        //    List<doGetUnpaidDetailDebtSummary> _doGetUnpaidDetailDebtSummary = new List<doGetUnpaidDetailDebtSummary>();

        //    string strBillingTargetCode = string.Empty;

        //    data.conYes = param.conYes;
        //    data.conNo = param.conNo;


        //    try
        //    {
        //        // Common Check Sequence

        //        // System Suspend
        //        ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
        //        CommonUtil comUtil = new CommonUtil();

        //        if (handlerCommon.IsSystemSuspending())
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
        //            return Json(res);
        //        }

        //        // Check User Permission

        //        //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MONEY_COLLECTION_MANAGEMENT_INFO, FunctionID.C_FUNC_ID_OPERATE) )
        //        //{
        //        //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
        //        //    return Json(res);
        //        //}


        //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
        //        if (data.strMode == "1")
        //        {
        //            strBillingTargetCode = comUtil.ConvertBillingTargetCode(data.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

        //            //GetUnpaidDetailDebtSummaryByBillingTarget
        //            _doGetUnpaidDetailDebtSummary = iincomeHandler.GetUnpaidDetailDebtSummaryByBillingTargetList(
        //                strBillingTargetCode);
        //        }

        //        if (data.strMode == "2")
        //        {
        //            //GetUnpaidDetailDebtSummaryByInvoice
        //            _doGetUnpaidDetailDebtSummary = iincomeHandler.GetUnpaidDetailDebtSummaryByInvoiceList(
        //                data.InvoiceNo,
        //                Convert.ToInt32(data.InvoiceOCC));
        //        }

        //        if (data.strMode == "3")
        //        {
        //            //GetUnpaidDetailDebtSummaryByBillingCode
        //            _doGetUnpaidDetailDebtSummary = iincomeHandler.GetUnpaidDetailDebtSummaryByBillingCodeList(
        //                data.BillingCode);
        //        }

        //        data.doBillingTargetDebtSummary = _doGetUnpaidDetailDebtSummary;

        //        // return "1" to js is every thing OK
        //        if (res.MessageList == null || res.MessageList.Count == 0)
        //        { res.ResultData = data; }
        //        else
        //        { res.ResultData = null; }

        //        return Json(res);

        //    }
        //    catch (Exception ex)
        //    {
        //        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}


    }
}
