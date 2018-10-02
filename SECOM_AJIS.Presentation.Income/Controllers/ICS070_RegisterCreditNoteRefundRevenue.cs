
//*********************************
// Create by: Waroon H.
// Create date: 04/Apr/2012
// Update date: 04/Apr/2012
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
using SECOM_AJIS.DataEntity.Billing;
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
        public ActionResult ICS070_Authority(ICS070_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();

            // System Suspend
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            if (handlerCommon.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_OPERATE))
            {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_CANCEL))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
            }

            return InitialScreenEnvironment<ICS070_ScreenParameter>("ICS070", param, res);

        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS070")]
        public ActionResult ICS070()
        {
            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();

            return View();
        }

        /// <summary>
        /// Generate xml for initial cancel credit note grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS070_InitialCancelCreditNoteGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS070_CancelCreditNoteGrid", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
 
        /// <summary>
        /// Check User screen operate authority
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult ICS070_CheckOperate()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                // Check User Permission
                string strResultData = "1";

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_OPERATE))
                {
                    strResultData = "0";
                }
 
                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = strResultData; }
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
        /// Check User screen cancel authority
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult ICS070_CheckCancelCreditNote()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                // Check User Permission
                string strResultData = "1";
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_CANCEL))
                {
                    strResultData = "0";
                }
 
                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = strResultData; }
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
        /// Retrieve cancel creditnote information of specific screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria</param>
        /// <returns></returns>
        public ActionResult ICS070_CancelCreditNoteRetrieveData(ICS070_RegisterData data)
        {
 
            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                // Common Check Sequence

                doGetCreditNote _doGetCreditNote = new doGetCreditNote();

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (String.IsNullOrEmpty(data.Header.txtCancelCreditNoteNo))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         new string[] { "lblCancelCreditNoteNo" },
                                         new string[] { "txtCancelCreditNoteNo" });

                    return Json(res);
                }

                _doGetCreditNote = iincomeHandler.GetCreditNote(data.Header.txtCancelCreditNoteNo);
                if (_doGetCreditNote != null)
                {
                    param.doGetCreditNote = _doGetCreditNote;
                }
                else
                { 
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7044,
                                         new string[] { data.Header.txtCancelCreditNoteNo },
                                         new string[] { "txtCancelCreditNoteNo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
 
                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
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
        /// Retrieve billing information of search criteria information
        /// </summary>
        /// <param name="data">Search criteria of button Except DepositFee</param>
        /// <returns></returns>
        public ActionResult ICS070_ExceptDepositFeeRetrieveBillingInformation(ICS070_RegisterData data)
        {
            string conModeRadio1optRegisterRefundCreditNoteExceptDepositFee = "1";
            string conModeRadio1optRegisterDecreasedCreditNote = "2";
            string conModeRadio1optRegisterRefundCreditNoteDepositFee = "3";
            string conModeRadio1optRevenueDepositFee = "4";
            string conModeRadio1optCancelCreditNote = "5";

            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            //ObjectResultData resByIssue = new ObjectResultData();
            //ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<doGetTaxInvoiceForIC> _doGetTaxInvoiceForICList = new List<doGetTaxInvoiceForIC>();
                List<doGetBillingDetailOfInvoice> _doGetBillingDetailOfInvoice = new List<doGetBillingDetailOfInvoice>();

                string strBillingCode = string.Empty;
                string strNextBillingCode = string.Empty;
                bool bolMultipleBillingCode = false;

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                ////Validate Details
                if (String.IsNullOrEmpty(data.Header.txtExceptDepositFeeTaxInvoiceNoForCreditNote))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         new string[] { "lblExceptDepositFeeTaxInvoiceNoForCreditNote" },
                                         new string[] { "txtExceptDepositFeeTaxInvoiceNoForCreditNote" });

                    return Json(res);
                }

                // Add By Sommai P., Nov 7, 2013
                // 4.2	Check tax invoice no. can register credit note
                bool blnCanRegisterCreditNote = iincomeHandler.CheckCanRegisterCreditNote(data.Header.txtExceptDepositFeeTaxInvoiceNoForCreditNote);
                if (blnCanRegisterCreditNote == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7123,
                                         new string[] { "lblExceptDepositFeeTaxInvoiceNoForCreditNote" },
                                         new string[] { "txtExceptDepositFeeTaxInvoiceNoForCreditNote" });

                    return Json(res);
                }

                // End Add

                _doGetTaxInvoiceForICList = iBillingHandler.GetTaxInvoiceForIC(data.Header.txtExceptDepositFeeTaxInvoiceNoForCreditNote);

                if (_doGetTaxInvoiceForICList != null && _doGetTaxInvoiceForICList.Count > 0)
                {
                    #region Validate business by each mode
                    //Validate only Except deposit
                    if (data.Header.rdoProcessType == conModeRadio1optRegisterRefundCreditNoteExceptDepositFee)
                    { 
                        //Except deposit
                        if (_doGetTaxInvoiceForICList[0].BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                        {
                            //MSG7072
                            res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_INCOME,
                                             MessageUtil.MessageList.MSG7072,
                                             new string[] { "lblExceptDepositFeeTaxInvoiceNoForCreditNote" },
                                             new string[] { "txtExceptDepositFeeTaxInvoiceNoForCreditNote" });

                            return Json(res);
                        }
                    }
                    #endregion

                    #region Pass validation, do process
                    // CheckTaxInvoiceMultipleBillingCode
                    _doGetBillingDetailOfInvoice = iBillingHandler.GetBillingDetailOfInvoiceList(
                        _doGetTaxInvoiceForICList[0].InvoiceNo,
                        _doGetTaxInvoiceForICList[0].InvoiceOCC);

                    if (_doGetBillingDetailOfInvoice != null)
                    {
                        //Comment by budd, no need 
                        //if (_doGetBillingDetailOfInvoice.Count > 1)
                        //{
                        //    //MSG7053
                        //    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                        //                     "ICS070",
                        //                     MessageUtil.MODULE_INCOME,
                        //                     MessageUtil.MessageList.MSG7053,
                        //                     new string[] { "lblDepositFeeTaxInvoiceNoForCreditNote" },
                        //                     new string[] { "txtDepositFeeTaxInvoiceNoForCreditNote" });

                        //    return Json(res);
                        //}

                        if (_doGetBillingDetailOfInvoice.Count > 0)
                        {
                            param.doGetBillingDetailOfInvoice = _doGetBillingDetailOfInvoice[0];
                            strBillingCode = _doGetBillingDetailOfInvoice[0].ContractCodeShort
                                + '-' + _doGetBillingDetailOfInvoice[0].BillingOCC;

                            for (int i = 1; i < _doGetBillingDetailOfInvoice.Count; i++)
                            {
                                if (strBillingCode != _doGetBillingDetailOfInvoice[i].ContractCodeShort
                                + '-' + _doGetBillingDetailOfInvoice[i].BillingOCC)
                                {
                                    bolMultipleBillingCode = true;
                                }
                            }
                        }
                        else
                        {
                            param.doGetBillingDetailOfInvoice = null;
                        }
                    }

                    if (bolMultipleBillingCode == false)
                    {
                        param.strBillingCode = strBillingCode;
                    }

                    param.doGetTaxInvoiceForIC = _doGetTaxInvoiceForICList[0];
                    #endregion
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7045,
                                         new string[] { "lblExceptDepositFeeTaxInvoiceNoForCreditNote" },
                                         new string[] { "txtExceptDepositFeeTaxInvoiceNoForCreditNote" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
 

                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
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
        /// Retrieve billing information of search criteria information
        /// </summary>
        /// <param name="data">Search criteria of button DepositFee</param>
        /// <returns></returns>
        public ActionResult ICS070_DepositFeeRetrieveBillingInformation(ICS070_RegisterData data)
        {
            string conModeRadio1optRegisterRefundCreditNoteExceptDepositFee = "1";
            string conModeRadio1optRegisterDecreasedCreditNote = "2";
            string conModeRadio1optRegisterRefundCreditNoteDepositFee = "3";
            string conModeRadio1optRevenueDepositFee = "4";
            string conModeRadio1optCancelCreditNote = "5";

            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            //ObjectResultData resByIssue = new ObjectResultData();
            //ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<doGetTaxInvoiceForIC> _doGetTaxInvoiceForICList = new List<doGetTaxInvoiceForIC>();
                List<tbt_BillingDetail> _doTbt_BillingDetail = new List<tbt_BillingDetail>();
                List<doGetBillingDetailOfInvoice> _doGetBillingDetailOfInvoice = new List<doGetBillingDetailOfInvoice>();

                string strBillingCode = string.Empty;
                string strNextBillingCode = string.Empty;
                bool bolMultipleBillingCode = false;

                decimal? decBalanceDeposit = 0;
                string decBalanceDepositCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                ////Validate Details
                if (String.IsNullOrEmpty(data.Header.txtDepositFeeTaxInvoiceNoForCreditNote))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         new string[] { "lblDepositFeeTaxInvoiceNoForCreditNote" },
                                         new string[] { "txtDepositFeeTaxInvoiceNoForCreditNote" });

                    return Json(res);
                }

                // Add By Sommai P., Nov 7, 2013
                // 10.2	Check tax invoice no. can register credit note
                bool blnCanRegisterCreditNote = iincomeHandler.CheckCanRegisterCreditNote(data.Header.txtDepositFeeTaxInvoiceNoForCreditNote);
                if (blnCanRegisterCreditNote == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7123,
                                         new string[] { "lblDepositFeeTaxInvoiceNoForCreditNote" },
                                         new string[] { "txtDepositFeeTaxInvoiceNoForCreditNote" });

                    return Json(res);
                }

                _doGetTaxInvoiceForICList = iBillingHandler.GetTaxInvoiceForIC(
                    data.Header.txtDepositFeeTaxInvoiceNoForCreditNote);

                if (_doGetTaxInvoiceForICList != null)
                {
                    if (_doGetTaxInvoiceForICList.Count > 0)
                    {

                        if (_doGetTaxInvoiceForICList[0].BillingTypeCode != BillingType.C_BILLING_TYPE_DEPOSIT)
                        {
                            //MSG7072
                            res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_INCOME,
                                             MessageUtil.MessageList.MSG7073,
                                             new string[] { "lblDepositFeeTaxInvoiceNoForCreditNote" },
                                             new string[] { "txtDepositFeeTaxInvoiceNoForCreditNote" });

                            return Json(res);
                        }
                        else
                        {
                            decBalanceDeposit = 0;
                            decBalanceDepositCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                            _doTbt_BillingDetail = iBillingHandler.GetTbt_BillingDetailOfInvoice(
                                _doGetTaxInvoiceForICList[0].InvoiceNo,
                                _doGetTaxInvoiceForICList[0].InvoiceOCC);
                            if (_doTbt_BillingDetail != null)
                            {
                                if (_doTbt_BillingDetail.Count > 1)
                                {
                                    //MSG7053
                                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                     "ICS070",
                                                     MessageUtil.MODULE_INCOME,
                                                     MessageUtil.MessageList.MSG7053,
                                                     new string[] { "lblDepositFeeTaxInvoiceNoForCreditNote" },
                                                     new string[] { "txtDepositFeeTaxInvoiceNoForCreditNote" });

                                    return Json(res);
                                }
                                if (_doTbt_BillingDetail.Count > 0)
                                {
                                    // Comment by Jirawat Jannet @ 2016-10-26
                                    //decBalanceDeposit = iBillingHandler.GetBalanceDepositValByBillingCode(
                                    //    _doTbt_BillingDetail[0].ContractCode,
                                    //    _doTbt_BillingDetail[0].BillingOCC);

                                    // Add by Jirawat Jannet @ 2016-10-26
                                    var decBalanceDepositDatas = iBillingHandler.GetBalanceDepositByBillingCode(
                                        _doTbt_BillingDetail[0].ContractCode,
                                        _doTbt_BillingDetail[0].BillingOCC);
                                    if (decBalanceDepositDatas != null && decBalanceDepositDatas.Count > 0)
                                    {
                                        decBalanceDeposit = decBalanceDepositDatas[0].BalanceDeposit;
                                        decBalanceDepositCurrencyType = decBalanceDepositDatas[0].BalanceDepositCurrencyType;
                                    }
                                    // end add  @ 2016-10-26

                                }
                            }

                            param.decBalanceDeposit = decBalanceDeposit;
                            param.decBalanceDepositCurrencyType = decBalanceDepositCurrencyType;
                            param.doGetTaxInvoiceForIC = _doGetTaxInvoiceForICList[0];
                        }

                        // CheckTaxInvoiceMultipleBillingCode
                        _doGetBillingDetailOfInvoice = iBillingHandler.GetBillingDetailOfInvoiceList(
                            _doGetTaxInvoiceForICList[0].InvoiceNo,
                            _doGetTaxInvoiceForICList[0].InvoiceOCC);

                        if (_doGetBillingDetailOfInvoice != null)
                        {
                            if (_doGetBillingDetailOfInvoice.Count > 1)
                            {
                                //MSG7053
                                res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                 "ICS070",
                                                 MessageUtil.MODULE_INCOME,
                                                 MessageUtil.MessageList.MSG7053,
                                                 new string[] { "lblDepositFeeTaxInvoiceNoForCreditNote" },
                                                 new string[] { "txtDepositFeeTaxInvoiceNoForCreditNote" });

                                return Json(res);
                            }

                            if (_doGetBillingDetailOfInvoice.Count > 0)
                            {
                                strBillingCode = _doGetBillingDetailOfInvoice[0].ContractCodeShort
                                    + '-' + _doGetBillingDetailOfInvoice[0].BillingOCC;

                                for (int i = 1; i < _doGetBillingDetailOfInvoice.Count; i++)
                                {
                                    if (strBillingCode != _doGetBillingDetailOfInvoice[i].ContractCodeShort
                                    + '-' + _doGetBillingDetailOfInvoice[i].BillingOCC)
                                    {
                                        bolMultipleBillingCode = true;
                                    }
                                }
                            }

                        }

                        if (bolMultipleBillingCode == false)
                        {
                            param.strBillingCode = strBillingCode;
                        }
                        else
                        {
                            //MSG7053
                            res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_INCOME,
                                             MessageUtil.MessageList.MSG7053,
                                             new string[] { "lblDepositFeeTaxInvoiceNoForCreditNote" },
                                             new string[] { "txtDepositFeeTaxInvoiceNoForCreditNote" });

                            return Json(res);
                        }
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7045,
                                         new string[] { "lblDepositFeeTaxInvoiceNoForCreditNote" },
                                         new string[] { "txtDepositFeeTaxInvoiceNoForCreditNote" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7045,
                                         new string[] { "lblDepositFeeTaxInvoiceNoForCreditNote" },
                                         new string[] { "txtDepositFeeTaxInvoiceNoForCreditNote" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }


                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
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
        /// Retrieve billing code info information list of ExceptDepositFee screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria</param>
        /// <returns></returns>
        public ActionResult ICS070_ExceptDepositFeeRetrieve(ICS070_RegisterData data)
        {
            string conModeRadio1optRegisterRefundCreditNoteExceptDepositFee = "1";
            string conModeRadio1optRegisterDecreasedCreditNote = "2";
            string conModeRadio1optRegisterRefundCreditNoteDepositFee = "3";
            string conModeRadio1optRevenueDepositFee = "4";
            string conModeRadio1optCancelCreditNote = "5";

            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<doGetBillingCodeInfo> _doGetBillingCodeInfoList = new List<doGetBillingCodeInfo>();

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                ////Validate Details
                ValidatorUtil validator = new ValidatorUtil();
                if (String.IsNullOrEmpty(data.Header.txtExceptDepositFeeBillingCode) || data.Header.txtExceptDepositFeeBillingCode.Equals("-"))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                      , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                      , "txtExceptDepositFeeBillingCode1", "lblExceptDepositFeeBillingCode", "txtExceptDepositFeeBillingCode1");

                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                     , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                     , "txtExceptDepositFeeBillingCode2", "lblExceptDepositFeeBillingCode", "txtExceptDepositFeeBillingCode2");

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    return Json(res);
                }

                _doGetBillingCodeInfoList = iBillingHandler.GetBillingCodeInfo(
                    comUtil.ConvertContractCode(data.Header.txtExceptDepositFeeBillingCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                if (_doGetBillingCodeInfoList != null)
                {
                    if (_doGetBillingCodeInfoList.Count > 0)
                    {
                        param.doGetBillingCodeInfo = _doGetBillingCodeInfoList[0];
                    }
                    else
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                          , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                          , "txtExceptDepositFeeBillingCode1", "lblExceptDepositFeeBillingCode", "txtExceptDepositFeeBillingCode1");

                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                         , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                         , "txtExceptDepositFeeBillingCode2", "lblExceptDepositFeeBillingCode", "txtExceptDepositFeeBillingCode2");

                        ValidatorUtil.BuildErrorMessage(res, validator, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }
                else
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                         , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                         , "txtExceptDepositFeeBillingCode1", "lblExceptDepositFeeBillingCode", "txtExceptDepositFeeBillingCode1");

                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                     , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                     , "txtExceptDepositFeeBillingCode2", "lblExceptDepositFeeBillingCode", "txtExceptDepositFeeBillingCode2");

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }


                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
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
        /// Retrieve billing code info information list of DepositFee screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria</param>
        /// <returns></returns>
        public ActionResult ICS070_DepositFeeRetrieve(ICS070_RegisterData data)
        {
            string conModeRadio1optRegisterRefundCreditNoteExceptDepositFee = "1";
            string conModeRadio1optRegisterDecreasedCreditNote = "2";
            string conModeRadio1optRegisterRefundCreditNoteDepositFee = "3";
            string conModeRadio1optRevenueDepositFee = "4";
            string conModeRadio1optCancelCreditNote = "5";

            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            //ObjectResultData resByIssue = new ObjectResultData();
            //ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<doGetBillingCodeInfo> _doGetBillingCodeInfoList = new List<doGetBillingCodeInfo>();

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                ////Validate Details
                ValidatorUtil validator = new ValidatorUtil();
                if (String.IsNullOrEmpty(data.Header.txtDepositFeeBillingCode) || data.Header.txtDepositFeeBillingCode.Equals("-"))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                      , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                      , "txtDepositFeeBillingCode1", "lblDepositFeeBillingCode", "txtDepositFeeBillingCode1");

                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                     , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                     , "txtDepositFeeBillingCode2", "lblDepositFeeBillingCode", "txtDepositFeeBillingCode2");

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    return Json(res);
                }
                 
                _doGetBillingCodeInfoList = iBillingHandler.GetBillingCodeInfo(
                    comUtil.ConvertContractCode(data.Header.txtDepositFeeBillingCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                if (_doGetBillingCodeInfoList != null)
                {
                    if (_doGetBillingCodeInfoList.Count > 0)
                    {
                        param.doGetBillingCodeInfo = _doGetBillingCodeInfoList[0];
                    }
                    else
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                          , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                          , "txtDepositFeeBillingCode1", "lblDepositFeeBillingCode", "txtDepositFeeBillingCode1");

                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                         , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                         , "txtDepositFeeBillingCode2", "lblDepositFeeBillingCode", "txtDepositFeeBillingCode2");

                        ValidatorUtil.BuildErrorMessage(res, validator, null);

                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }
                else
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                          , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                          , "txtDepositFeeBillingCode1", "lblDepositFeeBillingCode", "txtDepositFeeBillingCode1");

                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                     , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                     , "txtDepositFeeBillingCode2", "lblDepositFeeBillingCode", "txtDepositFeeBillingCode2");

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }


                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
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
        /// Retrieve billing code info information list of Revenue screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria</param>
        /// <returns></returns>
        public ActionResult ICS070_RevenueRetrieve(ICS070_RegisterData data)
        {
            string conModeRadio1optRegisterRefundCreditNoteExceptDepositFee = "1";
            string conModeRadio1optRegisterDecreasedCreditNote = "2";
            string conModeRadio1optRegisterRefundCreditNoteDepositFee = "3";
            string conModeRadio1optRevenueDepositFee = "4";
            string conModeRadio1optCancelCreditNote = "5";

            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            //ObjectResultData resByIssue = new ObjectResultData();
            //ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<doGetBillingCodeInfo> _doGetBillingCodeInfoList = new List<doGetBillingCodeInfo>();

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                ////Validate Details
                ValidatorUtil validator = new ValidatorUtil();
                if (String.IsNullOrEmpty(data.Header.txtRevenueBillingCode) || data.Header.txtRevenueBillingCode.Equals("-"))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                      , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                      , "txtRevenueBillingCode1", "lblRevenueBillingCode", "txtRevenueBillingCode1");

                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                     , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                     , "txtRevenueBillingCode2", "lblRevenueBillingCode", "txtRevenueBillingCode2");

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    return Json(res);
                }

                _doGetBillingCodeInfoList = iBillingHandler.GetBillingCodeInfo(
                    comUtil.ConvertContractCode(data.Header.txtRevenueBillingCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                if (_doGetBillingCodeInfoList != null)
                {
                    if (_doGetBillingCodeInfoList.Count > 0)
                    {
                        param.doGetBillingCodeInfo = _doGetBillingCodeInfoList[0];
                    }
                    else
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                         , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                         , "txtRevenueBillingCode1", "lblRevenueBillingCode", "txtRevenueBillingCode1");

                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                         , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                         , "txtRevenueBillingCode2", "lblRevenueBillingCode", "txtRevenueBillingCode2");

                        ValidatorUtil.BuildErrorMessage(res, validator, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }
                else
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                         , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                         , "txtRevenueBillingCode1", "lblRevenueBillingCode", "txtRevenueBillingCode1");

                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                     , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                     , "txtRevenueBillingCode2", "lblRevenueBillingCode", "txtRevenueBillingCode2");

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }


                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
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
        /// validate check when add except deposit fee data to selected list
        /// </summary>
        /// <param name="data">input data</param>
        /// <returns></returns>
        public ActionResult ICS070_ExceptDepositFeeAdd(ICS070_RegisterData data)
        {
            string conModeRadio1optRegisterRefundCreditNoteExceptDepositFee = "1";
            string conModeRadio1optRegisterDecreasedCreditNote = "2";
            string conModeRadio1optRegisterRefundCreditNoteDepositFee = "3";
            string conModeRadio1optRevenueDepositFee = "4";
            string conModeRadio1optCancelCreditNote = "5";

            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();

            //Reset warning flag
            if (param.WarningMessage == null)
            {
                param.WarningMessage = new List<string>();
            }
            else
            {
                param.WarningMessage.Clear();
            }
            

            //ObjectResultData resByIssue = new ObjectResultData();
            //ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<doGetRegContent> _doGetRegContentList = new List<doGetRegContent>();


                List<doGetBillingCodeInfo> _doGetBillingCodeInfoList = new List<doGetBillingCodeInfo>();

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                //Validate Details

                if (String.IsNullOrEmpty(data.Input1.strExceptDepositFeeTaxInvoiceNoForCreditNote))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "txtExceptDepositFeeTaxInvoiceNoForCreditNote",
                                         "lblExceptDepositFeeTaxInvoiceNoForCreditNote",
                                         "txtExceptDepositFeeTaxInvoiceNoForCreditNote");

                    //return Json(res);
                }

                if (String.IsNullOrEmpty(data.Input1.dtpExceptDepositFeeTaxInvoiceDate.ToString()))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "dtpExceptDepositFeeTaxInvoiceDate",
                                         "lblExceptDepositFeeTaxInvoiceDate",
                                         "dtpExceptDepositFeeTaxInvoiceDate");

                    //return Json(res);
                }

                if (String.IsNullOrEmpty(data.Input1.strExceptDepositFeeCreditNoteAmountIncludeVat))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "txtExceptDepositFeeCreditNoteAmountIncludeVat",
                                         "lblExceptDepositFeeCreditNoteAmountIncludeVAT",
                                         "txtExceptDepositFeeCreditNoteAmountIncludeVat");

                    //return Json(res);
                }

                if (String.IsNullOrEmpty(data.Input1.strExceptDepositFeeApproveNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "txtExceptDepositFeeApproveNo",
                                         "lblExceptDepositFeeApproveNo",
                                         "txtExceptDepositFeeApproveNo");

                    //return Json(res);
                }

                //Add by Jutarat A. on 23012014 (Move)
                if (String.IsNullOrEmpty(data.Input1.dtpExceptDepositFeeCreditNoteDate.ToString()))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                            "ICS070",
                                            MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0007,
                                            "dtpExceptDepositFeeCreditNoteDate",
                                            "lblExceptDepositFeeCreditNoteDate",
                                            "dtpExceptDepositFeeCreditNoteDate");
                }
                //End Add

                if (data.Input1.chkExceptDepositFeeNotRetrieve)
                {

                    if (String.IsNullOrEmpty(data.Input1.strExceptDepositFeeTaxInvoiceAmountIncludeVat))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                             "txtExceptDepositFeeTaxInvoiceAmountIncludeVat" ,
                                             "lblExceptDepositFeeTaxInvoiceAmountIncludeVAT",
                                             "txtExceptDepositFeeTaxInvoiceAmountIncludeVat" );

                        //return Json(res);
                    }

                    if (String.IsNullOrEmpty(data.Input1.strExceptDepositFeeTaxInvoiceAmount))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                             "txtExceptDepositFeeTaxInvoiceAmount",
                                             "lblExceptDepositFeeTaxInvoiceAmount",
                                             "txtExceptDepositFeeTaxInvoiceAmount");

                        //return Json(res);
                    }

                    if (String.IsNullOrEmpty(data.Input1.strExceptDepositFeeTaxInvoiceBillingType))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                             "cboExceptDepositFeeTaxInvoiceBillingType",
                                             "lblExceptDepositFeeTaxInvoiceBillingType",
                                             "cboExceptDepositFeeTaxInvoiceBillingType");

                        //return Json(res);
                    }

                    //Comment by Jutarat A. on 23012014 (Move)
                    /*// Add By Sommai P., Nov 7, 2013
                    if (String.IsNullOrEmpty(data.Input1.dtpExceptDepositFeeCreditNoteDate.ToString()))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                "ICS070",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "dtpExceptDepositFeeCreditNoteDate",
                                                "lblExceptDepositFeeCreditNoteDate",
                                                "dtpExceptDepositFeeCreditNoteDate");
                    }
                    // End Add*/
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    return Json(res);
                }

                if (Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmountIncludeVat) <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7047,
                                         new string[] { "lblExceptDepositFeeCreditNoteAmountIncludeVAT" },
                                         new string[] { "txtExceptDepositFeeCreditNoteAmountIncludeVat" });

                    return Json(res);
                }

                if (data.Header.rdoProcessType == conModeRadio1optRegisterRefundCreditNoteExceptDepositFee)
                {
                    if (Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmountIncludeVat)
                        > Convert.ToDecimal(data.Input1.strExceptDepositFeeTaxInvoiceAmountIncludeVat))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_INCOME,
                                             MessageUtil.MessageList.MSG7048,
                                             new string[] { "lblExceptDepositFeeCreditNoteAmountIncludeVAT" },
                                             new string[] { "txtExceptDepositFeeCreditNoteAmountIncludeVat" });

                        return Json(res);
                    }
                }

                if (data.Header.rdoProcessType == conModeRadio1optRegisterDecreasedCreditNote)
                {
                    if (Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmountIncludeVat)
                        > (Convert.ToDecimal(data.Input1.strExceptDepositFeeTaxInvoiceAmountIncludeVat)
                        - Convert.ToDecimal(data.Input1.strExceptDepositFeeAccumulatedPaymentAmount)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_INCOME,
                                             MessageUtil.MessageList.MSG7049,
                                             new string[] { "lblExceptDepositFeeCreditNoteAmountIncludeVAT" },
                                             new string[] { "txtExceptDepositFeeCreditNoteAmountIncludeVat" });

                        return Json(res);
                    }
                }

                if (Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmount)
                        >= Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmountIncludeVat))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7050,
                                         new string[] { "lblExceptDepositFeeCreditNoteAmount" },
                                         new string[] { "txtExceptDepositFeeCreditNoteAmount" });

                    return Json(res);
                }


                #region Check VAT uncharge flag
                #region Getting billing code info
                doGetBillingCodeInfo doBillingCodeInfo = param.doGetBillingCodeInfo;
                if (doBillingCodeInfo == null)
                {
                    //case: Not reteieve
                    doBillingCodeInfo = iBillingHandler.GetBillingCodeInfo(
                        param.doGetBillingDetailOfInvoice.ContractCode + "-" + param.doGetBillingDetailOfInvoice.BillingOCC).FirstOrDefault();

                    if (doBillingCodeInfo == null)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                                  , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                                  , "txtExceptDepositFeeBillingCode1", "lblExceptDepositFeeBillingCode", "txtExceptDepositFeeBillingCode1");

                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                                 , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                                 , "txtExceptDepositFeeBillingCode2", "lblExceptDepositFeeBillingCode", "txtExceptDepositFeeBillingCode2");

                        ValidatorUtil.BuildErrorMessage(res, validator, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }
                #endregion

                #region Getting billing basic
                tbt_BillingBasic doTbt_BillingBasic = iBillingHandler.GetTbt_BillingBasic(doBillingCodeInfo.ContractCode, doBillingCodeInfo.BillingOCC).FirstOrDefault();
                if (doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == true
                    && Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmount) != 0)
                {
                    //Uncharge flag = true , and input va need Confirm with warning message MSG7110
                    param.WarningMessage.Add("MSG7110");
                }
                #endregion
                #endregion



                decimal decCalVAT = 0;  
                decimal decVATRate = 0;

                if (!(data.Input1.chkExceptDepositFeeNotRetrieve) && param.doGetTaxInvoiceForIC != null)
                {
                    if ((doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == true
                            && Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmount) > 0)
                        || doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == false)
                    {
                        #region Need check diff more than ±10
                        decimal vatRate = 0;
                        if (doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == true)
                        {
                            vatRate = iBillingHandler.GetVATMaster(
                                data.Input1.strExceptDepositFeeTaxInvoiceBillingType
                                , CommonUtil.dsTransData.dtOperationData.ProcessDateTime).GetValueOrDefault();
                        }
                        else
                        {
                            vatRate = param.doGetTaxInvoiceForIC.VatRate.GetValueOrDefault();
                        }

                        //decCalVAT = (Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmountIncludeVat)
                        //       - Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmount)) * vatRate;

                        decCalVAT = (vatRate * Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmountIncludeVat)) / (1 + vatRate);
                        decCalVAT = iBillingHandler.RoundUp(decCalVAT, 2);      //Round from 2nd decimal point
                        
                        if ((Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmount) - decCalVAT >= 10)
                            || (Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmount) - decCalVAT <= -10))
                        {
                            //Confirm with warning message MSG7052 calculated more than ±10. Continue?
                            param.WarningMessage.Add("MSG7052");
                        }
                        #endregion
                    }
                }
                else
                {
                    if ((doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == true
                            && Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmount) > 0)
                        || doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == false)
                    {
                        #region Need check diff more than ±10
                        //doTax _doTax = iBillingHandler.GetTaxChargedData(
                        //    param.doGetBillingCodeInfo.ContractCode
                        //    , param.doGetBillingCodeInfo.BillingOCC
                        //    , data.Input1.strExceptDepositFeeTaxInvoiceBillingType
                        //    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                        //decVATRate = (decimal)_doTax.VATRate;

                        decVATRate = iBillingHandler.GetVATMaster(
                                    data.Input1.strExceptDepositFeeTaxInvoiceBillingType
                                    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime).GetValueOrDefault();

                        //decCalVAT = (Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmountIncludeVat)
                        //    - Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmount))
                        //    * decVATRate;

                        decCalVAT = (decVATRate * Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmountIncludeVat)) / (1 + decVATRate);
                        decCalVAT = iBillingHandler.RoundUp(decCalVAT, 2);      //Round from 2nd decimal point

                        if ((Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmount) - decCalVAT >= 10)
                            || (Convert.ToDecimal(data.Input1.strExceptDepositFeeCreditNoteAmount) - decCalVAT <= -10))
                        {
                            //Confirm with warning message MSG7052 calculated more than ±10. Continue?
                            param.WarningMessage.Add("MSG7052");
                        }
                        #endregion
                    }
                }



                if (data.Input1.dtpExceptDepositFeeCreditNoteDate != null)
                {
                    if (data.Input1.dtpExceptDepositFeeCreditNoteDate.Value.CompareTo(System.DateTime.Now) >= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_INCOME,
                                             MessageUtil.MessageList.MSG7051,
                                             new string[] { "lblExceptDepositFeeCreditNoteDate" },
                                             new string[] { "dtpExceptDepositFeeCreditNoteDate" });

                        return Json(res);
                    }
                }

                _doGetRegContentList = iincomeHandler.GetRegContent("1");

                if (_doGetRegContentList != null)
                {
                    if (_doGetRegContentList.Count > 0)
                    {
                        param.strRegContent = _doGetRegContentList[0].ValueDisplay;
                    }
                }

                //Modified by Budd
                //if (!(data.Input2.chkDepositFeeNotRetrieve))
                if (!(data.Input1.chkExceptDepositFeeNotRetrieve))
                {
                    if (param.doGetTaxInvoiceForIC != null)
                    {
                        param.strBillingTargetCode = param.doGetTaxInvoiceForIC.BillingTargetCode;
                        param.strInvoiceNo = param.doGetTaxInvoiceForIC.InvoiceNo;
                        param.strInvoiceOCC = param.doGetTaxInvoiceForIC.InvoiceOCC.ToString();
                        param.bolNotRetrieveFlag = false;
                    }
                }
                else
                {
                    if (param.doGetBillingCodeInfo != null)   //Modified by Budd, doGetTaxInvoiceForIC --> doGetBillingCodeInfo
                    {
                        param.strBillingTargetCode = param.doGetBillingCodeInfo.BillingTargetCode;
                        param.strInvoiceNo = null;
                        param.strInvoiceOCC = "0";
                        param.bolNotRetrieveFlag = true;
                    }
                }

                if (data.Header.rdoProcessType == conModeRadio1optRegisterRefundCreditNoteExceptDepositFee)
                {
                    //C_CN_TYPE_REFUND_NOT_DEPOSIT
                    param.strCreditNoteType = CreditNoteType.C_CN_TYPE_REFUND_NOT_DEPOSIT;
                }
                else
                {
                    //C_CN_TYPE_DECREASE
                    param.strCreditNoteType = CreditNoteType.C_CN_TYPE_DECREASE;
                }

                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
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
        /// validate check when add deposit fee data to selected list
        /// </summary>
        /// <param name="data">input data</param>
        /// <returns></returns>
        public ActionResult ICS070_DepositFeeAdd(ICS070_RegisterData data)
        {
            string conModeRadio1optRegisterRefundCreditNoteExceptDepositFee = "1";
            string conModeRadio1optRegisterDecreasedCreditNote = "2";
            string conModeRadio1optRegisterRefundCreditNoteDepositFee = "3";
            string conModeRadio1optRevenueDepositFee = "4";
            string conModeRadio1optCancelCreditNote = "5";

            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();

            //Reset warning flag
            if (param.WarningMessage == null)
            {
                param.WarningMessage = new List<string>();
            }
            else
            {
                param.WarningMessage.Clear();
            }

            //ObjectResultData resByIssue = new ObjectResultData();
            //ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<doGetRegContent> _doGetRegContentList = new List<doGetRegContent>();


                List<doGetBillingCodeInfo> _doGetBillingCodeInfoList = new List<doGetBillingCodeInfo>();

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                //Validate Details

                if (String.IsNullOrEmpty(data.Input2.strDepositFeeTaxInvoiceNoForCreditNote))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "txtDepositFeeTaxInvoiceNoForCreditNote",
                                         "lblDepositFeeTaxInvoiceNoForCreditNote",
                                         "txtDepositFeeTaxInvoiceNoForCreditNote");

                    //return Json(res);
                }

                if (String.IsNullOrEmpty(data.Input2.dtpDepositFeeTaxInvoiceDate.ToString()))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "dtpDepositFeeTaxInvoiceDate",
                                         "lblDepositFeeTaxInvoiceDate",
                                         "dtpDepositFeeTaxInvoiceDate");

                    //return Json(res);
                }

                if (String.IsNullOrEmpty(data.Input2.strDepositFeeCreditNoteAmountIncludeVat))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "txtDepositFeeCreditNoteAmountIncludeVat",
                                         "lblDepositFeeCreditNoteAmountIncludeVAT",
                                         "txtDepositFeeCreditNoteAmountIncludeVat");

                    //return Json(res);
                }
                // add by Jirawat Jannet on 2016-10-28
                else
                {
                    if (data.Input2.strDepositFeeCreditNoteAmount.ConvertTo<decimal>(false, -1) != -1)
                    {
                        if (data.Input2.strDepositFeeCreditNoteAmountIncludeVatCurrencyType != data.Input2.strDepositFeeCreditNoteAmountCurrencyType)
                        {
                            //validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                            //             "ICS070",
                            //             MessageUtil.MODULE_COMMON,
                            //             MessageUtil.MessageList.MSG0165,
                            //             "txtDepositFeeCreditNoteAmountIncludeVatCurrencyType",
                            //             "lblDepositFeeCreditNoteAmountIncludeVAT",
                            //             "txtDepositFeeCreditNoteAmountIncludeVatCurrencyType");
                            validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                        "ICS070",
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0165,
                                        "txtDepositFeeCreditNoteAmountCurrencyType",
                                        "lblDepositFeeCreditNoteAmount",
                                        "txtDepositFeeCreditNoteAmountCurrencyType");
                        }
                    }
                }

                if (String.IsNullOrEmpty(data.Input2.strDepositFeeApproveNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "txtDepositFeeApproveNo",
                                         "lblDepositFeeApproveNo",
                                         "txtDepositFeeApproveNo");

                    //return Json(res);
                }

                //Add by Jutarat A. on 23012014 (Move)
                if (String.IsNullOrEmpty(data.Input2.dtpDepositFeeCreditNoteDate.ToString()))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                            "ICS070",
                                            MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0007,
                                            "dtpDepositFeeCreditNoteDate",
                                            "lblDepositFeeCreditNoteDate",
                                            "dtpDepositFeeCreditNoteDate");
                }
                //End Add

                if (data.Input2.chkDepositFeeNotRetrieve)
                {

                    if (String.IsNullOrEmpty(data.Input2.strDepositFeeTaxInvoiceAmountIncludeVat))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                              "txtDepositFeeTaxInvoiceAmountIncludeVat",
                                              "lblDepositFeeTaxInvoiceAmountIncludeVAT",
                                              "txtDepositFeeTaxInvoiceAmountIncludeVat");

                        //return Json(res);
                    }

                    if (String.IsNullOrEmpty(data.Input2.strDepositFeeTaxInvoiceAmount))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                             "txtDepositFeeTaxInvoiceAmount",
                                             "lblDepositFeeTaxInvoiceAmount",
                                             "txtDepositFeeTaxInvoiceAmount");

                        //return Json(res);
                    }

                    //Comment by Jutarat A. on 23012014 (Move)
                    /*// Add By Sommai P., Nov 7, 2013
                    if (String.IsNullOrEmpty(data.Input2.dtpDepositFeeCreditNoteDate.ToString()))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                "ICS070",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "dtpDepositFeeCreditNoteDate",
                                                "lblDepositFeeCreditNoteDate",
                                                "dtpDepositFeeCreditNoteDate");
                    }
                    // End Add*/

                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    return Json(res);
                }

                if (Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmountIncludeVat) <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7054,
                                         new string[] { "lblDepositFeeCreditNoteAmountIncludeVAT" },
                                         new string[] { "txtDepositFeeCreditNoteAmountIncludeVat" });

                    return Json(res);
                }

                

                //Edit by Patcharee T. (BalanceOfDepositFee is not include VAT)
                //if (Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmountIncludeVat)
                //        > Convert.ToDecimal(data.Input2.strDepositFeeBalanceOfDepositFee))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                //                         "ICS070",
                //                         MessageUtil.MODULE_INCOME,
                //                         MessageUtil.MessageList.MSG7055,
                //                         new string[] { "lblDepositFeeCreditNoteAmountIncludeVAT" },
                //                         new string[] { "txtDepositFeeCreditNoteAmountIncludeVat" });

                //    return Json(res);
                //}

                //Edit by Patcharee T. (Refund Deposit Fee is not deduct by WHT)
                //if (!(data.Input2.chkDepositFeeNotRetrieve))
                //{
                //    if (Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmountIncludeVat)
                //        > Convert.ToDecimal(data.Input2.strDepositFeeAccumulatedPaymentAmount))    //Modified by budd  >=  -->    >
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                //                             "ICS070",
                //                             MessageUtil.MODULE_INCOME,
                //                             MessageUtil.MessageList.MSG7056,
                //                             new string[] { "lblDepositFeeAccumulatedPaymentAmount" },
                //                             new string[] { "txtDepositFeeAccumulatedPaymentAmount" });

                //        return Json(res);
                //    }
                //}

                if (Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmountIncludeVat)
                        <= Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmount))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7057,
                                         new string[] { "lblDepositFeeCreditNoteAmount" },
                                         new string[] { "txtDepositFeeCreditNoteAmount" });

                    return Json(res);
                }

                #region Check VAT uncharge flag
                #region Getting billing code info
                doGetBillingCodeInfo doBillingCodeInfo = param.doGetBillingCodeInfo;
                if (doBillingCodeInfo == null)
                { 
                    //case: Not reteieve
                    doBillingCodeInfo = iBillingHandler.GetBillingCodeInfo(
                        comUtil.ConvertContractCode(data.Header.txtDepositFeeBillingCode
                        , CommonUtil.CONVERT_TYPE.TO_LONG)).FirstOrDefault();

                    if (doBillingCodeInfo == null)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                                  , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                                  , "txtDepositFeeBillingCode1", "lblDepositFeeBillingCode", "txtDepositFeeBillingCode1");

                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                                 , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                                 , "txtDepositFeeBillingCode2", "lblDepositFeeBillingCode", "txtDepositFeeBillingCode2");

                        ValidatorUtil.BuildErrorMessage(res, validator, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }
                #endregion
                
                #region Getting billing basic
                tbt_BillingBasic doTbt_BillingBasic = iBillingHandler.GetTbt_BillingBasic(doBillingCodeInfo.ContractCode, doBillingCodeInfo.BillingOCC).FirstOrDefault();
                if (doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == true
                    && Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmount) != 0)
                { 
                    //Uncharge flag = true , and input va need Confirm with warning message MSG7110
                    param.WarningMessage.Add("MSG7110");
                }
                #endregion
                #endregion


                decimal decCalVAT = 0;
                decimal decVATRate = 0;

                if (!(data.Input2.chkDepositFeeNotRetrieve) && param.doGetTaxInvoiceForIC != null)
                {
                    if ((doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == true
                            && Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmount) > 0)
                        || doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == false)
                    {
                        #region Need check diff more than ±10
                        decimal vatRate = 0;
                        if (doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == true)
                        {
                            vatRate = iBillingHandler.GetVATMaster(
                                BillingType.C_BILLING_TYPE_DEPOSIT
                                , CommonUtil.dsTransData.dtOperationData.ProcessDateTime).GetValueOrDefault();
                        }
                        else
                        {
                            vatRate = param.doGetTaxInvoiceForIC.VatRate.GetValueOrDefault();
                        }

                        //decCalVAT = (Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmountIncludeVat)
                        //    - Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmount)) * vatRate;

                        decCalVAT = (vatRate * Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmountIncludeVat)) / (1 + vatRate);
                        decCalVAT = iBillingHandler.RoundUp(decCalVAT, 2);      //Round from 2nd decimal point

                        if ((Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmount) - decCalVAT >= 10)
                            || (Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmount) - decCalVAT <= -10))
                        {
                            //Confirm with warning message MSG7052 calculated more than ±10. Continue?
                            param.WarningMessage.Add("MSG7052");
                        }
                        #endregion
                    }
                }
                else
                {
                    if ((doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == true
                            && Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmount) > 0)
                        || doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == false)
                    {
                        #region Need check diff more than ±10
                        //doTax _doTax = iBillingHandler.GetTaxChargedData(
                        //    param.doGetBillingCodeInfo.ContractCode
                        //    , param.doGetBillingCodeInfo.BillingOCC
                        //    , BillingType.C_BILLING_TYPE_DEPOSIT
                        //    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        decVATRate = iBillingHandler.GetVATMaster(
                                BillingType.C_BILLING_TYPE_DEPOSIT
                                , CommonUtil.dsTransData.dtOperationData.ProcessDateTime).GetValueOrDefault();

                        //decCalVAT = (Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmountIncludeVat)
                        //    - Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmount)) * decVATRate;

                        decCalVAT = (decVATRate * Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmountIncludeVat)) / (1 + decVATRate);
                        decCalVAT = iBillingHandler.RoundUp(decCalVAT, 2);      //Round from 2nd decimal point

                        if ((Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmount) - decCalVAT >= 10)
                            || (Convert.ToDecimal(data.Input2.strDepositFeeCreditNoteAmount) - decCalVAT <= -10))
                        {
                            //Confirm with warning message MSG7052 calculated more than ±10. Continue?
                            param.WarningMessage.Add("MSG7052");
                        }
                        #endregion
                    }
                }


                if (data.Input2.dtpDepositFeeCreditNoteDate != null)
                {
                    if (data.Input2.dtpDepositFeeCreditNoteDate.Value.CompareTo(System.DateTime.Now) >= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_INCOME,
                                             MessageUtil.MessageList.MSG7051,
                                             new string[] { "lblDepositFeeCreditNoteDate" },
                                             new string[] { "dtpDepositFeeCreditNoteDate" });

                        return Json(res);
                    }
                }

                _doGetRegContentList = iincomeHandler.GetRegContent("2");

                if (_doGetRegContentList != null)
                {
                    if (_doGetRegContentList.Count > 0)
                    {
                        param.strRegContent = _doGetRegContentList[0].ValueDisplay;
                    }
                }
               
                //Modified by Budd
                //if (!(data.Input1.chkExceptDepositFeeNotRetrieve))
                if (!(data.Input2.chkDepositFeeNotRetrieve))
                {
                    if (param.doGetTaxInvoiceForIC != null)
                    {
                        param.strBillingTargetCode = param.doGetTaxInvoiceForIC.BillingTargetCode;
                        param.strInvoiceNo = param.doGetTaxInvoiceForIC.InvoiceNo;
                        param.strInvoiceOCC = param.doGetTaxInvoiceForIC.InvoiceOCC.ToString();
                        param.bolNotRetrieveFlag = false;
                    }
                }
                else
                {
                    if (param.doGetBillingCodeInfo != null)     //Modified by Budd    doGetTaxInvoiceForIC --> doGetBillingCodeInfo
                    {
                        param.strBillingTargetCode = param.doGetBillingCodeInfo.BillingTargetCode;
                        param.strInvoiceNo = null;
                        param.strInvoiceOCC = "0";
                        param.bolNotRetrieveFlag = true;
                    }
                }

                if (data.Header.rdoProcessType == conModeRadio1optRegisterRefundCreditNoteExceptDepositFee)
                {
                    //C_CN_TYPE_REFUND_NOT_DEPOSIT
                    param.strCreditNoteType = CreditNoteType.C_CN_TYPE_REFUND_NOT_DEPOSIT;
                }
                else
                {
                    //C_CN_TYPE_DECREASE
                    param.strCreditNoteType = CreditNoteType.C_CN_TYPE_DECREASE;
                }

                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
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
        /// validate check when add revenue data to selected list
        /// </summary>
        /// <param name="data">input data</param>
        /// <returns></returns>
        public ActionResult ICS070_RevenueAdd(ICS070_RegisterData data)
        {
            string conModeRadio1optRegisterRefundCreditNoteExceptDepositFee = "1";
            string conModeRadio1optRegisterDecreasedCreditNote = "2";
            string conModeRadio1optRegisterRefundCreditNoteDepositFee = "3";
            string conModeRadio1optRevenueDepositFee = "4";
            string conModeRadio1optCancelCreditNote = "5";

            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();

            //Reset warning flag
            if (param.WarningMessage == null)
            {
                param.WarningMessage = new List<string>();
            }
            else
            {
                param.WarningMessage.Clear();
            }

            //ObjectResultData resByIssue = new ObjectResultData();
            //ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<doGetRegContent> _doGetRegContentList = new List<doGetRegContent>();


                List<doGetBillingCodeInfo> _doGetBillingCodeInfoList = new List<doGetBillingCodeInfo>();

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                //Validate Details

                if (String.IsNullOrEmpty(data.Input3.strRevenueRevenueDepositFee))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                          "txtRevenueRevenueDepositFee",
                                          "lblRevenueRevenueDepositFee",
                                          "txtRevenueRevenueDepositFee");

                    //return Json(res);
                }
                //if (String.IsNullOrEmpty(data.Input3.strRevenueRevenueVatAmount))
                //{
                //    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                //                         "ICS070",
                //                         MessageUtil.MODULE_COMMON,
                //                         MessageUtil.MessageList.MSG0007,
                //                         "txtRevenueRevenueVatAmount",
                //                         "lblRevenueRevenueVatAmount",
                //                         "txtRevenueRevenueVatAmount");

                //    //return Json(res);
                //}

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    return Json(res);
                }

                if (Convert.ToDecimal(data.Input3.strRevenueRevenueDepositFee) <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7058,
                                         new string[] { "lblRevenueRevenueDepositFee" },
                                         new string[] { "txtRevenueRevenueDepositFee" });

                    return Json(res);
                }
                if (!(String.IsNullOrEmpty(data.Input3.strRevenueRevenueVatAmount)))
                {
                    if (Convert.ToDecimal(data.Input3.strRevenueRevenueDepositFee)
                        <= Convert.ToDecimal(data.Input3.strRevenueRevenueVatAmount))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                             "ICS070",
                                             MessageUtil.MODULE_INCOME,
                                             MessageUtil.MessageList.MSG7059,
                                             new string[] { "lblRevenueRevenueVatAmount" },
                                             new string[] { "txtRevenueRevenueVatAmount" });

                        return Json(res);
                    }
                }
                //if (Convert.ToDecimal(data.Input3.strRevenueRevenueDepositFee)
                //    > Convert.ToDecimal(data.Input3.strRevenueBalanceOfDepositFee))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                //                         "ICS070",
                //                         MessageUtil.MODULE_INCOME,
                //                         MessageUtil.MessageList.MSG7091,
                //                         new string[] { "lblRevenueRevenueDepositFee" },
                //                         new string[] { "txtRevenueRevenueDepositFee" });

                //    return Json(res);
                //}


                #region Check VAT uncharge flag
                #region Getting billing code info
                //Always retrieve billing code info
                doGetBillingCodeInfo doBillingCodeInfo = param.doGetBillingCodeInfo;
                if (doBillingCodeInfo == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                              , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                              , "txtRevenueBillingCode1", "lblRevenueBillingCode", "txtRevenueBillingCode1");

                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS070"
                     , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7046
                     , "txtRevenueBillingCode2", "lblRevenueBillingCode", "txtRevenueBillingCode2");

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                #endregion

                #region Getting billing basic
                tbt_BillingBasic doTbt_BillingBasic = iBillingHandler.GetTbt_BillingBasic(doBillingCodeInfo.ContractCode, doBillingCodeInfo.BillingOCC).FirstOrDefault();
                if (doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == true
                    && Convert.ToDecimal(data.Input3.strRevenueRevenueVatAmount) != 0)
                {
                    //Uncharge flag = true , and input va need Confirm with warning message MSG7110
                    param.WarningMessage.Add("MSG7110");
                }
                #endregion
                #endregion


                decimal decCalVAT = 0;
                decimal decVATRate = 0;


                if ((doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == true
                            && Convert.ToDecimal(data.Input3.strRevenueRevenueVatAmount) > 0)
                        || doTbt_BillingBasic.VATUnchargedFlag.GetValueOrDefault() == false)
                {
                    #region Need check diff more than ±10
                    //doTax _doTax = iBillingHandler.GetTaxChargedData(
                    //    param.doGetBillingCodeInfo.ContractCode
                    //    , param.doGetBillingCodeInfo.BillingOCC
                    //    , BillingType.C_BILLING_TYPE_DEPOSIT
                    //    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                    //decVATRate = (decimal)_doTax.VATRate;

                    decVATRate = iBillingHandler.GetVATMaster(
                        BillingType.C_BILLING_TYPE_DEPOSIT
                        , CommonUtil.dsTransData.dtOperationData.ProcessDateTime).GetValueOrDefault();

                    //decCalVAT = (Convert.ToDecimal(data.Input3.strRevenueRevenueDepositFee)
                    //    - Convert.ToDecimal(data.Input3.strRevenueRevenueVatAmount)) * decVATRate;

                    decCalVAT = (decVATRate * Convert.ToDecimal(data.Input3.strRevenueRevenueDepositFee)) / (1 + decVATRate);
                    decCalVAT = iBillingHandler.RoundUp(decCalVAT, 2);      //Round from 2nd decimal point

                    if ((Convert.ToDecimal(data.Input3.strRevenueRevenueVatAmount) - decCalVAT >= 10)
                        || (Convert.ToDecimal(data.Input3.strRevenueRevenueVatAmount) - decCalVAT <= -10))
                    {
                        //Confirm with warning message MSG7052 calculated more than ±10. Continue?
                        param.WarningMessage.Add("MSG7052");
                    }
                    #endregion
                }
                
                
                _doGetRegContentList = iincomeHandler.GetRegContent("3");

                if (_doGetRegContentList != null)
                {
                    if (_doGetRegContentList.Count > 0)
                    {
                        param.strRegContent = _doGetRegContentList[0].ValueDisplay;
                    }
                }

                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
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
        public ActionResult ICS070_Register(ICS070_RegisterData data)
        {
            string conModeRadio1optRegisterRefundCreditNoteExceptDepositFee = "1";
            string conModeRadio1optRegisterDecreasedCreditNote = "2";
            string conModeRadio1optRegisterRefundCreditNoteDepositFee = "3";
            string conModeRadio1optRevenueDepositFee = "4";
            string conModeRadio1optCancelCreditNote = "5";

            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            //ObjectResultData resByIssue = new ObjectResultData();
            //ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                List<doGetTaxInvoiceForIC> _doGetTaxInvoiceForICList = new List<doGetTaxInvoiceForIC>();
                List<doGetBillingCodeInfo> _doGetBillingCodeInfoList = new List<doGetBillingCodeInfo>();
                decimal? decTempBalanceDeposit = 0;
                string decTempBalanceDepositCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                List<doGetBillingDetailOfInvoice> _doGetBillingDetailOfInvoiceList = new List<doGetBillingDetailOfInvoice>();

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

                if (data.Detail1 == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7071,
                                         new string[] { },
                                         new string[] { });

                    return Json(res);
                }


                if (data.Detail1.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7071,
                                         new string[] { },
                                         new string[] { });

                    return Json(res);
                }
                ////Validate Details

                // loop in registration list
                for (int i = 0; i < data.Detail1.Count; i++)
                {
                    if (data.Header.rdoProcessType == conModeRadio1optRegisterRefundCreditNoteExceptDepositFee) // 19.3.1.1
                    {

                        if (data.Detail1[i].strNotRetrieveFlag == "0")
                        {
                            _doGetTaxInvoiceForICList = iBillingHandler.GetTaxInvoiceForIC(
                                data.Detail1[i].strTaxInvoiceNo);

                            if (_doGetTaxInvoiceForICList != null)
                            {
                                if (_doGetTaxInvoiceForICList.Count > 0)
                                {
                                    if (Convert.ToDecimal(data.Detail1[i].strCreditAmountIncVAT) >
                                        (_doGetTaxInvoiceForICList[0].InvoiceAmount
                                         + _doGetTaxInvoiceForICList[0].VatAmount))
                                    {
                                        //MSG7048
                                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7048,
                                         new string[] { },
                                         new string[] { });
                                    }

                                    // Add By Sommai P., Nov 7, 2013
                                    if (Convert.ToDecimal(data.Detail1[i].strCreditAmountIncVAT) >
                                        (_doGetTaxInvoiceForICList[0].InvoiceAmount
                                         + _doGetTaxInvoiceForICList[0].VatAmount
                                         - iincomeHandler.GetTotalCreditAmtIncVAT(data.Detail1[i].strTaxInvoiceNo)))
                                    {
                                        //MSG7124
                                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7124,
                                         new string[] { },
                                         new string[] { });
                                    }
                                    // End Add
                                }
                            }

                        }

                    }
                    if (data.Header.rdoProcessType == conModeRadio1optRegisterDecreasedCreditNote) // 19.3.1.2
                    {
                        if (data.Detail1[i].strNotRetrieveFlag == "0")
                        {
                            _doGetTaxInvoiceForICList = iBillingHandler.GetTaxInvoiceForIC(
                                    data.Detail1[i].strTaxInvoiceNo);

                            if (_doGetTaxInvoiceForICList != null)
                            {
                                if (_doGetTaxInvoiceForICList.Count > 0)
                                {
                                    if (Convert.ToDecimal(data.Detail1[i].strCreditAmountIncVAT) >
                                        ((_doGetTaxInvoiceForICList[0].InvoiceAmount
                                         + _doGetTaxInvoiceForICList[0].VatAmount))
                                        - _doGetTaxInvoiceForICList[0].PaidAmountIncVat)
                                    {
                                        //MSG7049
                                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7049,
                                         new string[] { },
                                         new string[] { });
                                    }

                                    // Add By Sommai P., Nov 7, 2013
                                    if (Convert.ToDecimal(data.Detail1[i].strCreditAmountIncVAT) >
                                        (_doGetTaxInvoiceForICList[0].InvoiceAmount
                                         + _doGetTaxInvoiceForICList[0].VatAmount
                                         - iincomeHandler.GetTotalCreditAmtIncVAT(data.Detail1[i].strTaxInvoiceNo)))
                                    {
                                        //MSG7124
                                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7124,
                                         new string[] { },
                                         new string[] { });
                                    }
                                    // End Add
                                }
                            }
                        }
                    }
                    if (data.Header.rdoProcessType == conModeRadio1optRegisterRefundCreditNoteDepositFee) // 19.3.1.3
                    {
                        if (data.Detail1[i].strNotRetrieveFlag == "1")
                        {

                            _doGetBillingCodeInfoList = iBillingHandler.GetBillingCodeInfo(
                                    data.Detail1[i].strBillingCode);

                            if (_doGetBillingCodeInfoList != null)
                            {
                                if (_doGetBillingCodeInfoList.Count > 0)
                                {
                                    //if (Convert.ToDecimal(data.Detail1[i].strCreditAmountIncVAT) >
                                    //     _doGetBillingCodeInfoList[0].BalanceDeposit)
                                    //{
                                    //    //MSG7056
                                    //    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                    //     "ICS070",
                                    //     MessageUtil.MODULE_INCOME,
                                    //     MessageUtil.MessageList.MSG7056,
                                    //     new string[] { },
                                    //     new string[] { });
                                    //}

                                    
                                }
                            }
                        }
                        else
                        {
                            _doGetTaxInvoiceForICList = iBillingHandler.GetTaxInvoiceForIC(
                                        data.Detail1[i].strTaxInvoiceNo);

                            if (_doGetTaxInvoiceForICList != null)
                            {
                                if (_doGetTaxInvoiceForICList.Count > 0)
                                {
                                    //Edit by Patcharee T. (06-Mar-2013) 
                                    //if (Convert.ToDecimal(data.Detail1[i].strCreditAmountIncVAT) >
                                    //    _doGetTaxInvoiceForICList[0].PaidAmountIncVat)
                                    //{
                                    //    //MSG7056
                                    //    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                    //     "ICS070",
                                    //     MessageUtil.MODULE_INCOME,
                                    //     MessageUtil.MessageList.MSG7056,
                                    //     new string[] { },
                                    //     new string[] { });
                                    //}

                                    _doGetBillingDetailOfInvoiceList = iBillingHandler.GetBillingDetailOfInvoiceList(
                                        _doGetTaxInvoiceForICList[0].InvoiceNo,
                                        _doGetTaxInvoiceForICList[0].InvoiceOCC);

                                    // Comment by Jirawat Jannet @ 2016-10-26
                                    //decTempBalanceDeposit = iBillingHandler.GetBalanceDepositValByBillingCode(
                                    //    _doGetBillingDetailOfInvoiceList[0].ContractCode,
                                    //    _doGetBillingDetailOfInvoiceList[0].BillingOCC);

                                    // Add by Jirawat Jannet @ 2016-10-26
                                    var decTempBalanceDepositDatas = iBillingHandler.GetBalanceDepositByBillingCode(
                                        _doGetBillingDetailOfInvoiceList[0].ContractCode,
                                        _doGetBillingDetailOfInvoiceList[0].BillingOCC);
                                    if (decTempBalanceDepositDatas != null && decTempBalanceDepositDatas.Count > 0)
                                    {
                                        decTempBalanceDeposit = decTempBalanceDepositDatas[0].BalanceDeposit;
                                        decTempBalanceDepositCurrencyType = decTempBalanceDepositDatas[0].BalanceDepositCurrencyType;
                                    }
                                    // end add  @ 2016-10-26

                                    if (decTempBalanceDeposit == null)
                                    {
                                        decTempBalanceDeposit = 0;
                                    }

                                    //if (Convert.ToDecimal(data.Detail1[i].strCreditAmountIncVAT) >
                                    //    decTempBalanceDeposit)
                                    //{
                                    //    //MSG7056
                                    //    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                    //     "ICS070",
                                    //     MessageUtil.MODULE_INCOME,
                                    //     MessageUtil.MessageList.MSG7056,
                                    //     new string[] { },
                                    //     new string[] { });
                                    //}

                                    // Add By Sommai P., Nov 7, 2013
                                    if (Convert.ToDecimal(data.Detail1[i].strCreditAmountIncVAT) >
                                        (_doGetTaxInvoiceForICList[0].InvoiceAmount
                                         + _doGetTaxInvoiceForICList[0].VatAmount
                                         - iincomeHandler.GetTotalCreditAmtIncVAT(data.Detail1[i].strTaxInvoiceNo)))
                                    {
                                        //MSG7124
                                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                         "ICS070",
                                         MessageUtil.MODULE_INCOME,
                                         MessageUtil.MessageList.MSG7124,
                                         new string[] { },
                                         new string[] { });
                                    }
                                    // End Add
                                }
                            }
                        }
                    }
                    if (data.Header.rdoProcessType == conModeRadio1optRevenueDepositFee)
                    {
                        _doGetBillingCodeInfoList = iBillingHandler.GetBillingCodeInfo(
                                    data.Detail1[i].strBillingCode);

                        if (_doGetBillingCodeInfoList != null)
                        {
                            if (_doGetBillingCodeInfoList.Count > 0)
                            {
                                //if (Convert.ToDecimal(data.Detail1[i].strRevenueAmountIncVAT) >
                                //        _doGetBillingCodeInfoList[0].BalanceDeposit)
                                //{
                                //    //MSG7058
                                //    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                //        "ICS070",
                                //        MessageUtil.MODULE_INCOME,
                                //        MessageUtil.MessageList.MSG7058,
                                //        new string[] { },
                                //        new string[] { });
                                //}
                            }
                        }
                    }

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
        /// validate input data confirm and register data into database in cancel mode
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS070_ConfirmDelete()
        {
 
            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            // Comment scope by jirawat jannet on 2016-11-03
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    // Common Check Sequence

                    // System Suspend
                    ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                    IBillingHandler ibillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                    List<DataEntity.Income.tbt_CreditNote> _doTbt_CreditNoteList = new List<DataEntity.Income.tbt_CreditNote>();
                    tbt_Payment _dotbt_Payment = new tbt_Payment();

                    if (handlerCommon.IsSystemSuspending())
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                        return Json(res);
                    }

                    // Check User Permission

                    //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    //    return Json(res);
                    //}

                    if (!(iincomeHandler.CheckCreditNoteCanCancel(param.doGetCreditNote.CreditNoteNo)))   // Modified by budd,  param.doGetCreditNote.TaxInvoiceNo --> param.doGetCreditNote.CreditNoteNo
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                 "ICS070",
                                                 MessageUtil.MODULE_INCOME,
                                                 MessageUtil.MessageList.MSG7060,
                                                 new string[] { param.doGetCreditNote.TaxInvoiceNo },
                                                 new string[] { "txtCancelCreditNoteNo" });
                        return Json(res);
                    }

                    // Add By Sommai P., Nov 11, 2013
                    bool bIsStatusReturn = iincomeHandler.CheckDepositStatusReturn(param.doGetCreditNote.CreditNoteNo);

                    if (bIsStatusReturn == true)
                    {
                        DataEntity.Billing.tbt_BillingBasic _doBillingBasic = ibillingHandler.GetBillingBasicByCreditNoteNo(param.doGetCreditNote.CreditNoteNo);

                        bool bSuccess = iincomeHandler.CancelCreditNote(_doBillingBasic, param.doGetCreditNote);
                        if (!bSuccess)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                        "ICS070",
                                                        MessageUtil.MODULE_INCOME,
                                                        MessageUtil.MessageList.MSG7063,
                                                        new string[] { param.doGetCreditNote.TaxInvoiceNo },
                                                        new string[] { "txtCancelCreditNoteNo" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }
                    }
                    // End Add
                    _doTbt_CreditNoteList = iincomeHandler.GetTbt_CreditNote(param.doGetCreditNote.CreditNoteNo);
                    if (_doTbt_CreditNoteList != null)
                    {
                        if (_doTbt_CreditNoteList.Count > 0)
                        {

                            _doTbt_CreditNoteList[0].CancelFlag = true;

                            if (iincomeHandler.UpdateTbt_CreditNote(_doTbt_CreditNoteList[0]) <= 0)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                    "ICS070",
                                                    MessageUtil.MODULE_INCOME,
                                                    MessageUtil.MessageList.MSG7063,
                                                    new string[] { param.doGetCreditNote.TaxInvoiceNo },
                                                    new string[] { "txtCancelCreditNoteNo" });
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                return Json(res);
                            };

                            _dotbt_Payment = iincomeHandler.GetPayment(_doTbt_CreditNoteList[0].PaymentTransNo);
                            if (iincomeHandler.DeletePaymentTransaction(_dotbt_Payment) == false)   // Added by budd,   == false
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                        "ICS070",
                                                        MessageUtil.MODULE_INCOME,
                                                        MessageUtil.MessageList.MSG7063,
                                                        new string[] { param.doGetCreditNote.TaxInvoiceNo },
                                                        new string[] { "txtCancelCreditNoteNo" });
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                return Json(res);
                            }

                        }
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                "ICS070",
                                                MessageUtil.MODULE_INCOME,
                                                MessageUtil.MessageList.MSG7063,
                                                new string[] { param.doGetCreditNote.TaxInvoiceNo },
                                                new string[] { "txtCancelCreditNoteNo" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
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
                finally
                {
                    if (res.MessageList == null || res.MessageList.Count == 0)
                    {
                        scope.Complete();
                    }
                    else
                    {
                        scope.Dispose();
                    }
                }
            }
            return Json(res);
        }

        /// <summary>
        /// validate input data confirm and register data into database
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS070_Confirm()
        {
            string conModeRadio1optRegisterRefundCreditNoteExceptDepositFee = "1";
            string conModeRadio1optRegisterDecreasedCreditNote = "2";
            string conModeRadio1optRegisterRefundCreditNoteDepositFee = "3";
            string conModeRadio1optRevenueDepositFee = "4";
            string conModeRadio1optCancelCreditNote = "5";

            ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
            ICS070_RegisterData RegisterData = new ICS070_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            //ObjectResultData resByIssue = new ObjectResultData();
            //ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                IIncomeDocumentHandler incomeDocHandler = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler; //Add by Jutarat A. on 02102013

                tbt_Payment _dotbt_Payment = new tbt_Payment();
                List<tbt_Payment> _dotbt_PaymentList = new List<tbt_Payment>();
                List<tbt_Payment> _dotbt_PaymentListReturn = new List<tbt_Payment>();
                List<dtTbt_BillingTargetForView> _dodtTbt_BillingTargetForViewList = new List<dtTbt_BillingTargetForView>();
                DataEntity.Income.tbt_CreditNote _dotbt_CreditNote = new DataEntity.Income.tbt_CreditNote();
                DataEntity.Income.tbt_CreditNote _dotbt_CreditNoteReturn = new DataEntity.Income.tbt_CreditNote();
                tbt_Invoice _dotbt_Invoice = new tbt_Invoice();

                tbt_Revenue _dotbt_Revenue = new tbt_Revenue();
                tbt_Revenue _dotbt_RevenueReturn = new tbt_Revenue();
                string strTempLongContractCode = string.Empty;
                string strTempContractCode = string.Empty;
                string strTempBillingOCC = string.Empty;
                List<DataEntity.Billing.tbt_Depositfee> _dotbt_DepositFeeList = new List<DataEntity.Billing.tbt_Depositfee>();
                DataEntity.Billing.tbt_Depositfee _dotbt_DepositFee = new DataEntity.Billing.tbt_Depositfee();

                //string C_CN_TYPE_DECREASE = "3";
                //string C_CN_TYPE_REFUND_DEPOSIT = "2";
                //string C_INC_PAYMENT_CN_PROCESS = "3";

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check User Permission

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_CREDIT_NOTE, FunctionID.C_FUNC_ID_DEL))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}


                //Get income misc word
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> incomeMiscWord = hand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_INC_MISC_WORD,
                        ValueCode = "%"
                    }
                });


                using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                    // loop in registration list
                    for (int i = 0; i < param.RegisterData.Detail1.Count; i++)
                    {
                        #region Prepare
                        // comment by jirawat janne @ 2016-10-20
                        //if (param.RegisterData.Detail1[i].strAmountIncludingVat != null ||
                        //   param.RegisterData.Detail1[i].strAmountIncludingVat != "")
                        //{
                        //    param.RegisterData.Detail1[i].strAmountIncludingVat = Convert.ToDecimal(param.RegisterData.Detail1[i].strAmountIncludingVat).ToString();
                        //}
                        //else
                        //{
                        //    param.RegisterData.Detail1[i].strAmountIncludingVat = "0";
                        //}

                        //if (param.RegisterData.Detail1[i].strVatAmount != null ||
                        //   param.RegisterData.Detail1[i].strVatAmount != "")
                        //{
                        //    param.RegisterData.Detail1[i].strVatAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strVatAmount).ToString();
                        //}
                        //else
                        //{
                        //    param.RegisterData.Detail1[i].strVatAmount = "0";
                        //}
                        // add by jirawat janne @ 2016-10-20
                        if (!string.IsNullOrEmpty(param.RegisterData.Detail1[i].strAmountIncludingVat))
                        {
                            param.RegisterData.Detail1[i].strAmountIncludingVat = Convert.ToDecimal(param.RegisterData.Detail1[i].strAmountIncludingVat).ToString();
                        }
                        else
                        {
                            param.RegisterData.Detail1[i].strAmountIncludingVat = "0";
                        }

                        if (!string.IsNullOrEmpty(param.RegisterData.Detail1[i].strVatAmount))
                        {
                            param.RegisterData.Detail1[i].strVatAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strVatAmount).ToString();
                        }
                        else
                        {
                            param.RegisterData.Detail1[i].strVatAmount = "0";
                        }
                        #endregion

                        if (!(param.RegisterData.Header.rdoProcessType == conModeRadio1optRevenueDepositFee))
                        {
                            #region For first 3 case (Except revenue deposit fee
                            #region Payment
                            _dotbt_Payment = new tbt_Payment();

                            if (param.RegisterData.Detail1[i].strCreditNoteType == "1"
                                || param.RegisterData.Detail1[i].strCreditNoteType == "2")
                            {
                                _dotbt_Payment.PaymentType = PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND;
                            }
                            if (param.RegisterData.Detail1[i].strCreditNoteType == "3")
                            {
                                _dotbt_Payment.PaymentType = PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED;
                            }
                            _dotbt_Payment.PaymentDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            // Comment by Jirawat Jannet on 2016-10-28
                            //_dotbt_Payment.PaymentAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditAmountIncVAT);

                            // add by jirawat jannet @ 2016-10-20 รอตรวจสอบ
                            _dotbt_Payment.PaymentAmountCurrencyType = param.RegisterData.Detail1[i].strAmountIncludingVatCurrencyType;
                            if (param.RegisterData.Detail1[i].strAmountIncludingVatCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                _dotbt_Payment.PaymentAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditAmountIncVAT);
                                _dotbt_Payment.PaymentAmountUsd = null;
                            }
                            else
                            {
                                _dotbt_Payment.PaymentAmount = null;
                                _dotbt_Payment.PaymentAmountUsd = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditAmountIncVAT);
                            }

                            _dotbt_Payment.SECOMAccountID = null;

                            _dodtTbt_BillingTargetForViewList = iBillingHandler.GetTbt_BillingTargetForView(
                                param.RegisterData.Detail1[i].strBillingTargetCode
                                , MiscType.C_CUST_TYPE);

                            if (_dodtTbt_BillingTargetForViewList != null && _dodtTbt_BillingTargetForViewList.Count > 0)
                            {
                                if (_dodtTbt_BillingTargetForViewList[0].DocLanguage == DocLanguage.C_DOC_LANGUAGE_EN)
                                {
                                    //EN doc language -> EN desc
                                    _dotbt_Payment.Payer = param.RegisterData.Detail1[i].strBillingClientNameEN;
                                }
                                else
                                {
                                    //TH doc language -> LC desc
                                    _dotbt_Payment.Payer = param.RegisterData.Detail1[i].strBillingClientName;
                                }
                            }

                            _dotbt_Payment.PayerBankAccNo = null;
                            _dotbt_Payment.SendingBankCode = null;
                            _dotbt_Payment.SendingBranchCode = null;
                            _dotbt_Payment.TelNo = null;

                            string taxInvoiceWord = string.Empty;
                            string issueReasonWord = string.Empty;

                            if (_dodtTbt_BillingTargetForViewList != null && _dodtTbt_BillingTargetForViewList.Count > 0
                                && _dodtTbt_BillingTargetForViewList[0].DocLanguage == DocLanguage.C_DOC_LANGUAGE_TH)
                            {
                                //TH doc language -> LC desc
                                taxInvoiceWord = incomeMiscWord.Where(d => d.ValueCode == IncomeMiscWord.C_INC_MISC_WORD_TAX_INVOICE).First().ValueDisplayLC;
                                issueReasonWord = incomeMiscWord.Where(d => d.ValueCode == IncomeMiscWord.C_INC_MISC_WORD_ISSUE_REASON).First().ValueDisplayLC;
                            }
                            else
                            {
                                //All else -> EN desc
                                taxInvoiceWord = incomeMiscWord.Where(d => d.ValueCode == IncomeMiscWord.C_INC_MISC_WORD_TAX_INVOICE).First().ValueDisplayEN;
                                issueReasonWord = incomeMiscWord.Where(d => d.ValueCode == IncomeMiscWord.C_INC_MISC_WORD_ISSUE_REASON).First().ValueDisplayEN;
                            }
                            _dotbt_Payment.Memo = string.Format("{0} : {1}, {2} : {3}", taxInvoiceWord, param.RegisterData.Detail1[i].strTaxInvoiceNo
                                                                                    , issueReasonWord, param.RegisterData.Detail1[i].strIssueReason);

                            _dotbt_Payment.SystemMethod = PaymentSystemMethod.C_INC_PAYMENT_CN_PROCESS;
                            _dotbt_Payment.RefAdvanceReceiptNo = null;
                            _dotbt_Payment.RefAdvanceReceiptAmount = null;
                            _dotbt_Payment.RefAdvanceReceiptAmountCurrencyType = null; // add by jirawat jannet on 2016-10-28
                            _dotbt_Payment.RefAdvanceReceiptAmountUsd = null; // add by jirawat jannet on 2016-10-28
                            //_dotbt_Payment.RefInvoiceNo = param.RegisterData.Detail1[i].strInvoiceNo;
                            //if (param.RegisterData.Detail1[i].strInvoiceOCC == null)
                            //{
                            //    param.RegisterData.Detail1[i].strInvoiceOCC = "0";
                            //}
                            //_dotbt_Payment.RefInvoiceOCC = Convert.ToInt32(param.RegisterData.Detail1[i].strInvoiceOCC);
                            _dotbt_Payment.RefInvoiceNo = null;
                            _dotbt_Payment.RefInvoiceOCC = null;

                            // Comment by Jirawat Jannet @ 2016-10-21
                            //_dotbt_Payment.MatchableBalance = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditAmountIncVAT);

                            // Add by Jirawat Jannet @ 2016-10-21 รอตรวจสอบ
                            _dotbt_Payment.MatchableBalanceCurrencyType = param.RegisterData.Detail1[i].strAmountIncludingVatCurrencyType;
                            if (param.RegisterData.Detail1[i].strAmountIncludingVatCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                _dotbt_Payment.MatchableBalance = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditAmountIncVAT);
                                _dotbt_Payment.MatchableBalanceUsd = 0;
                            }
                            else
                            {
                                _dotbt_Payment.MatchableBalance = 0;
                                _dotbt_Payment.MatchableBalanceUsd = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditAmountIncVAT);
                            }

                            _dotbt_Payment.BankFeeRegisteredFlag = false;
                            _dotbt_Payment.OtherIncomeRegisteredFlag = false;
                            _dotbt_Payment.OtherExpenseRegisteredFlag = false;
                            _dotbt_Payment.DeleteFlag = false;

                            _dotbt_PaymentList = new List<tbt_Payment>();
                            _dotbt_PaymentList.Add(_dotbt_Payment);
                            _dotbt_PaymentListReturn = iincomeHandler.RegisterPayment(_dotbt_PaymentList);
                            #endregion

                            #region Credit note
                            _dotbt_CreditNote = new DataEntity.Income.tbt_CreditNote();
                            if (_dotbt_PaymentListReturn != null)
                            {
                                if (_dotbt_PaymentListReturn.Count > 0)
                                {
                                    _dotbt_CreditNote.PaymentTransNo = _dotbt_PaymentListReturn[0].PaymentTransNo;
                                }
                            }

                            //_dotbt_CreditNote.CreditNoteNo = param.RegisterData.Detail1[i].strCreditNoteNo;
                            _dotbt_CreditNote.TaxInvoiceNo = param.RegisterData.Detail1[i].strTaxInvoiceNo;

                            _dotbt_CreditNote.BillingCode = comUtil.ConvertBillingCode(param.RegisterData.Detail1[i].strBillingCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                            _dotbt_CreditNote.BillingTargetCode = param.RegisterData.Detail1[i].strBillingTargetCode;

                            //Modified by budd 
                            //_dotbt_CreditNote.CreditNoteDate = (DateTime)param.RegisterData.Detail1[i].dtpCreditNoteDate;
                            _dotbt_CreditNote.CreditNoteDate = param.RegisterData.Detail1[i].dtpCreditNoteDate;

                            // Comment by jirawat jannet on 2016-10-28
                            //_dotbt_CreditNote.CreditAmountIncVAT = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditAmountIncVAT);
                            // Add by Jirawat Jannet @ 2016-10-28 //strDepositFeeCreditNoteAmountCurrencyType
                            _dotbt_CreditNote.CreditAmountIncVATCurrencyType = param.RegisterData.Detail1[i].strVatAmountCurrencyType;
                            if (param.RegisterData.Detail1[i].strVatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                _dotbt_CreditNote.CreditAmountIncVAT = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditAmountIncVAT);
                                _dotbt_CreditNote.CreditAmountIncVATUsd = 0;
                            }
                            else
                            {
                                _dotbt_CreditNote.CreditAmountIncVAT = 0;
                                _dotbt_CreditNote.CreditAmountIncVATUsd = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditAmountIncVAT);
                            }

                            // comment by jirawat jannet on 2016-10-28
                            //_dotbt_CreditNote.CreditVATAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditVATAmount);
                            // add by jirawat jannet on 2016-10-28
                            _dotbt_CreditNote.CreditVATAmountCurrencyType = param.RegisterData.Detail1[i].strVatAmountCurrencyType;
                            if (param.RegisterData.Detail1[i].strVatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                _dotbt_CreditNote.CreditVATAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditVATAmount);
                                _dotbt_CreditNote.CreditVATAmountUsd = 0;
                            }
                            else
                            {
                                _dotbt_CreditNote.CreditVATAmount = 0;
                                _dotbt_CreditNote.CreditVATAmountUsd = Convert.ToDecimal(param.RegisterData.Detail1[i].strCreditVATAmount);
                            }


                            _dotbt_CreditNote.BillingTypeCode = param.RegisterData.Detail1[i].strBillingTypeCode;
                            _dotbt_CreditNote.ApproveNo = param.RegisterData.Detail1[i].strApproveNo;
                            _dotbt_CreditNote.IssueReason = param.RegisterData.Detail1[i].strIssueReason;

                            // Comment by jirawat jannet o 2016-10-28
                            //_dotbt_CreditNote.TaxInvoiceAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strTaxInvoiceAmount);
                            // add by jirawat jannet on 2016-10-28
                            _dotbt_CreditNote.TaxInvoiceAmountCurrencyType = param.RegisterData.Detail1[i].strVatAmountCurrencyType;
                            if (param.RegisterData.Detail1[i].strVatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                _dotbt_CreditNote.TaxInvoiceAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strTaxInvoiceAmount);
                                _dotbt_CreditNote.TaxInvoiceAmountUsd = 0;
                            }
                            else
                            {
                                _dotbt_CreditNote.TaxInvoiceAmount = 0;
                                _dotbt_CreditNote.TaxInvoiceAmountUsd = Convert.ToDecimal(param.RegisterData.Detail1[i].strTaxInvoiceAmount);
                            }

                            // Comment by jirawat jannet o 2016-10-28
                            //_dotbt_CreditNote.TaxInvoiceVATAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strTaxInvoiceVATAmount);
                            // add by jirawat jannet on 2016-10-28
                            _dotbt_CreditNote.TaxInvoiceVATAmountCurrencyType = param.RegisterData.Detail1[i].strVatAmountCurrencyType;
                            if (param.RegisterData.Detail1[i].strVatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                _dotbt_CreditNote.TaxInvoiceVATAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strTaxInvoiceVATAmount);
                                _dotbt_CreditNote.TaxInvoiceVATAmountUsd = 0;
                            }
                            else
                            {
                                _dotbt_CreditNote.TaxInvoiceVATAmount = 0;
                                _dotbt_CreditNote.TaxInvoiceVATAmountUsd = Convert.ToDecimal(param.RegisterData.Detail1[i].strTaxInvoiceVATAmount);
                            }


                            _dotbt_CreditNote.TaxInvoiceDate = param.RegisterData.Detail1[i].dtpTaxInvoiceDate;
                            //_dotbt_CreditNote.PaymentTransNo = param.RegisterData.Detail1[i].strPaymentTransNo;
                            _dotbt_CreditNote.CancelFlag = null;
                            _dotbt_CreditNote.CreditNoteType = param.RegisterData.Detail1[i].strCreditNoteType;

                            _dotbt_CreditNoteReturn = iincomeHandler.RegisterCreditNote(_dotbt_CreditNote);
                            if (_dotbt_CreditNoteReturn == null)
                            {
                                scope.Dispose();
                                res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                     "ICS070",
                                                     MessageUtil.MODULE_INCOME,
                                                     MessageUtil.MessageList.MSG7064,
                                                     new string[] { },
                                                     new string[] { });
                                return Json(res);
                            }
                            #endregion

                            // push new running number back to client
                            param.RegisterData.Detail1[i].strCreditNoteNo = _dotbt_CreditNoteReturn.CreditNoteNo;

                            // AND doCreditNote.NotRetrieveFlag=FALSE 
                            if (_dotbt_CreditNote.CreditNoteType == CreditNoteType.C_CN_TYPE_DECREASE
                                && param.RegisterData.Detail1[i].strNotRetrieveFlag == "0")
                            {
                                //Finding invoice no. and invoice info
                                List<tbt_TaxInvoice> doTaxInvoice = iBillingHandler.GetTbt_TaxInvoice(_dotbt_CreditNote.TaxInvoiceNo);
                                _dotbt_Invoice = iBillingHandler.GetTbt_InvoiceData(doTaxInvoice[0].InvoiceNo, doTaxInvoice[0].InvoiceOCC);

                                #region Not use, use MatchPaymentInvoice instead
                                //iincomeHandler.MatchPaymentInvoiceAuto(_dotbt_Payment, _dotbt_Invoice);
                                #endregion

                                #region MatchPaymentInvoice
                                #region Prepare match payment info
                                string matchStatus = string.Empty;
                                if ((_dotbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL // add by Jirawat Jannet on 2016-10-28 
                                    && (_dotbt_Invoice.PaidAmountIncVat ?? 0) == 0 //&& (_dotbt_Invoice.RegisteredWHTAmount ?? 0) == 0 //Comment by Jutarat A. on 20122013
                                    && _dotbt_Payment.PaymentAmount == (_dotbt_Invoice.InvoiceAmount ?? 0) + (_dotbt_Invoice.VatAmount ?? 0))//- (_dotbt_Invoice.WHTAmount ?? 0)) //Comment by Jutarat A. on 20122013
                                    || (_dotbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US // add by Jirawat Jannet on 2016-10-28 
                                        && (_dotbt_Invoice.PaidAmountIncVatUsd ?? 0) == 0 // add by Jirawat Jannet on 2016-10-28 
                                        && _dotbt_Payment.PaymentAmountUsd == (_dotbt_Invoice.InvoiceAmountUsd ?? 0) + (_dotbt_Invoice.VatAmountUsd ?? 0)))  // add by Jirawat Jannet on 2016-10-28 
                                {
                                    //Match full
                                    matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL;
                                }
                                else if ((_dotbt_Invoice.PaidAmountIncVatCurrencyType ==CurrencyUtil.C_CURRENCY_LOCAL // add by jirawat jannet on 2016-10-28
                                           && (_dotbt_Invoice.PaidAmountIncVat ?? 0) > 0
                                           && _dotbt_Payment.PaymentAmount == (_dotbt_Invoice.InvoiceAmount ?? 0) + (_dotbt_Invoice.VatAmount ?? 0) - (_dotbt_Invoice.PaidAmountIncVat ?? 0))//- (_dotbt_Invoice.RegisteredWHTAmount ?? 0)) //Comment by Jutarat A. on 20122013
                                           || (_dotbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US // add by jirawat jannet on 2016-10-28
                                           && (_dotbt_Invoice.PaidAmountIncVatUsd ?? 0) > 0 // add by jirawat jannet on 2016-10-28
                                           && _dotbt_Payment.PaymentAmountUsd == (_dotbt_Invoice.InvoiceAmountUsd ?? 0) + (_dotbt_Invoice.VatAmountUsd ?? 0) - (_dotbt_Invoice.PaidAmountIncVatUsd ?? 0)))  // add by jirawat jannet on 2016-10-28
                                {
                                    //Match to full (CN only)
                                    matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL_TO_FULL;
                                }
                                else if ((_dotbt_Payment.PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL // add by jirawat jannet on 2016-10-28
                                            && _dotbt_Payment.PaymentAmount < (_dotbt_Invoice.InvoiceAmount ?? 0) + (_dotbt_Invoice.VatAmount ?? 0) - (_dotbt_Invoice.PaidAmountIncVat ?? 0)) //- (_dotbt_Invoice.RegisteredWHTAmount ?? 0)) //Comment by Jutarat A. on 20122013
                                            || (_dotbt_Payment.PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_US // add by jirawat jannet on 2016-10-28
                                            && _dotbt_Payment.PaymentAmountUsd < (_dotbt_Invoice.InvoiceAmountUsd ?? 0) + (_dotbt_Invoice.VatAmountUsd ?? 0) - (_dotbt_Invoice.PaidAmountIncVatUsd ?? 0)))  // add by jirawat jannet on 2016-10-28
                                {
                                    //Partially match (CN only)
                                    matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL;
                                }
                                else
                                {
                                    //Not found case
                                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7079, null);
                                }

                                doMatchPaymentDetail matchDetail = new doMatchPaymentDetail()
                                {
                                    MatchID = null,
                                    InvoiceNo = _dotbt_Invoice.InvoiceNo,
                                    InvoiceOCC = _dotbt_Invoice.InvoiceOCC,

                                    MatchAmountExcWHT = _dotbt_Payment.PaymentAmount.ConvertTo<decimal>(false, 0),
                                    MatchAmountExcWHTUsd = _dotbt_Payment.PaymentAmountUsd.ConvertTo<decimal>(false, 0), // add by jirawat jannet @ 2016-10-21 
                                    MatchAmountExcWHTCurrencyType = _dotbt_Payment.PaymentAmountCurrencyType, // add by jirawat jannet @ 2016-10-21 

                                    MatchAmountIncWHT = _dotbt_Payment.PaymentAmount.ConvertTo<decimal>(false, 0),
                                    MatchAmountIncWHTUsd = _dotbt_Payment.PaymentAmountUsd.ConvertTo<decimal>(false, 0), // add by jirawat jannet @ 2016-10-21 
                                    MatchAmountIncWHTCurrencyType = _dotbt_Payment.PaymentAmountCurrencyType, // add by jirawat jannet @ 2016-10-21 

                                    WHTAmount = 0,
                                    WHTAmountUsd = 0, // add by jirawat jannet @ 2016-10-21
                                    WHTAmountCurrencyType = null, // add by jirawat jannet @ 2016-10-21 

                                    MatchStatus = matchStatus,
                                    CancelFlag = false,
                                    CancelApproveNo = null,
                                    CorrectionReason = null,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                                };
                                doMatchPaymentHeader matchHeader = new doMatchPaymentHeader()
                                {
                                    MatchID = null,
                                    MatchDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    PaymentTransNo = _dotbt_Payment.PaymentTransNo,

                                    TotalMatchAmount = _dotbt_Payment.PaymentAmount.ConvertTo<decimal>(false, 0),
                                    TotalMatchAmountUsd = _dotbt_Payment.PaymentAmountUsd.ConvertTo<decimal>(false, 0), // add by jirawat jannet @ 2016-10-21 
                                    TotalMatchAmountCurrencyType = _dotbt_Payment.PaymentAmountCurrencyType, // add by jirawat jannet @ 2016-10-21 

                                    BankFeeAmount = null,
                                    BankFeeAmountUsd = null, // add by jirawat jannet @ 2016-10-21 
                                    BankFeeAmountCurrencyType = null, // add by jirawat jannet @ 2016-10-21 

                                    SpecialProcessFlag = false,
                                    ApproveNo = null,

                                    OtherExpenseAmount = null,
                                    OtherExpenseAmountUsd = null, // add by jirawat jannet @ 2016-10-21
                                    OtherExpenseAmountCurrencyType = null, // add by jirawat jannet @ 2016-10-21 

                                    OtherIncomeAmount = null, 
                                    OtherIncomeAmountUsd = null, // add by jirawat jannet @ 2016-10-21
                                    OtherIncomeAmountCurrencyType = null, // add by jirawat jannet @ 2016-10-21 

                                    CancelFlag = false,
                                    CancelApproveNo = null,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                                };
                                matchHeader.MatchPaymentDetail = new List<doMatchPaymentDetail>();
                                matchHeader.MatchPaymentDetail.Add(matchDetail);
                                #endregion
                                bool isSuccess = iincomeHandler.MatchPaymentInvoices(matchHeader, _dotbt_Payment);
                                if (isSuccess == false)
                                {
                                    scope.Dispose();
                                    res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7120);
                                    return Json(res);
                                }
                                #endregion
                            }
                            #endregion

                            //Add by Jutarat A. on 02102013
                            //string strCreditNoteFilePath = incomeDocHandler.GenerateICR020FilePath(_dotbt_CreditNoteReturn.CreditNoteNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                            //param.RegisterData.Detail1[i].FilePath = strCreditNoteFilePath;
                            //End Add
                        }
                        else
                        {
                            #region Revenue deposit fee
                            _dotbt_Revenue = new tbt_Revenue();

                            //_dotbt_Revenue.RevenueNo = param.RegisterData.Detail1[i].strRevenueNo;

                            _dotbt_Revenue.BillingCode = comUtil.ConvertBillingCode(param.RegisterData.Detail1[i].strBillingCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                            //_dotbt_Revenue.IssueDate = param.RegisterData.Detail1[i].dtpIssueDate;;
                            _dotbt_Revenue.IssueDate = System.DateTime.Now.Date;


                            // Start: add by jirawat jannet on 2016-11-01
                            _dotbt_Revenue.RevenueAmountIncVATCurrencyType = param.RegisterData.Detail1[i].strRevenueAmountIncVATCurrencyType;
                            if (param.RegisterData.Detail1[i].strRevenueAmountIncVATCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                _dotbt_Revenue.RevenueAmountIncVAT = Convert.ToDecimal(param.RegisterData.Detail1[i].strRevenueAmountIncVAT);
                                _dotbt_Revenue.RevenueAmountIncVATUsd = 0;
                            }
                            else
                            {
                                _dotbt_Revenue.RevenueAmountIncVAT = 0;
                                _dotbt_Revenue.RevenueAmountIncVATUsd = Convert.ToDecimal(param.RegisterData.Detail1[i].strRevenueAmountIncVAT);
                            }

                            _dotbt_Revenue.RevenueVATAmountCurrencyType = param.RegisterData.Detail1[i].strRevenueVATAmountCurrencyType;
                            if (_dotbt_Revenue.RevenueVATAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                _dotbt_Revenue.RevenueVATAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strRevenueVATAmount);
                                _dotbt_Revenue.RevenueVATAmountUsd = null;
                            }
                            else
                            {
                                _dotbt_Revenue.RevenueVATAmount = null;
                                _dotbt_Revenue.RevenueVATAmountUsd = Convert.ToDecimal(param.RegisterData.Detail1[i].strRevenueVATAmount);
                            }
                            // End: add by jirawat jannet on 2016-11-01

                            // Comment by jirawat jannet on 2016-11-01
                            //_dotbt_Revenue.RevenueAmountIncVAT = Convert.ToDecimal(param.RegisterData.Detail1[i].strRevenueAmountIncVAT);
                            //_dotbt_Revenue.RevenueVATAmount = Convert.ToDecimal(param.RegisterData.Detail1[i].strRevenueVATAmount);

                            _dotbt_RevenueReturn = iincomeHandler.RegisterRevenue(_dotbt_Revenue);
                            if (_dotbt_RevenueReturn == null)
                            {
                                scope.Dispose();
                                res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                     "ICS070",
                                                     MessageUtil.MODULE_INCOME,
                                                     MessageUtil.MessageList.MSG7065,
                                                     new string[] { },
                                                     new string[] { });
                                return Json(res);
                            }

                            // push new running number back to client
                            param.RegisterData.Detail1[i].strRevenueNo = _dotbt_RevenueReturn.RevenueNo;

                            #region old
                            //// strBillingCode format <code 12>-<occ 2> '123456789012-12'
                            //strTempContractCode = param.RegisterData.Detail1[i].strBillingCode.Substring(0, 12);
                            //strTempBillingOCC = param.RegisterData.Detail1[i].strBillingCode.Substring(12, 2);

                            //strTempLongContractCode = comUtil.ConvertContractCode(strTempContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                            //_dotbt_DepositFeeList = iBillingHandler.GetLatestDepositFee(
                            //    strTempContractCode
                            //    , strTempBillingOCC);

                            //if (_dotbt_DepositFeeList != null)
                            //{
                            //    if (_dotbt_DepositFeeList.Count > 0)
                            //    {
                            //        _dotbt_DepositFee = new DataEntity.Billing.tbt_DepositFee();

                            //        _dotbt_DepositFee.ContractCode = strTempContractCode;
                            //        _dotbt_DepositFee.BillingOCC = strTempBillingOCC;
                            //        _dotbt_DepositFee.DepositFeeNo = _dotbt_DepositFeeList[0].DepositFeeNo + 1;
                            //        _dotbt_DepositFee.ProcessDate = System.DateTime.Now.Date;
                            //        _dotbt_DepositFee.DepositStatus = DepositStatus.C_DEPOSIT_STATUS_REVENUE;
                            //        _dotbt_DepositFee.ProcessAmount = _dotbt_RevenueReturn.RevenueAmountIncVAT;
                            //        _dotbt_DepositFee.InvoiceNo = _dotbt_DepositFeeList[0].InvoiceNo;
                            //        _dotbt_DepositFee.ReceiptNo = _dotbt_DepositFeeList[0].ReceiptNo;
                            //        _dotbt_DepositFee.CreditNoteNo = _dotbt_DepositFeeList[0].CreditNoteNo;
                            //        _dotbt_DepositFee.RevenueNo = _dotbt_RevenueReturn.RevenueNo;
                            //        _dotbt_DepositFee.SlideBillingCode = _dotbt_RevenueReturn.BillingCode;

                            //        var inserted_depositFee = iBillingHandler.CreateTbt_Depositfee(_dotbt_DepositFee);
                            //        if (inserted_depositFee.Count <= 0)
                            //        {
                            //            //MSG7065
                            //            res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                            //                     "ICS070",
                            //                     MessageUtil.MODULE_INCOME,
                            //                     MessageUtil.MessageList.MSG7065,
                            //                     new string[] { },
                            //                     new string[] { });

                            //            return Json(res);
                            //        }

                            //    }
                            //}
                            #endregion

                            bool isSuccess = ICS070_RevenueDepositFee(param.RegisterData.Detail1[i].strBillingCode, _dotbt_RevenueReturn);
                            if (!isSuccess)
                            {
                                scope.Dispose();
                                res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                                        "ICS070",
                                                        MessageUtil.MODULE_INCOME,
                                                        MessageUtil.MessageList.MSG7065,
                                                        new string[] { },
                                                        new string[] { });
                                return Json(res);
                            }
                            #endregion
                        }
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    //Fail, rollback all record, all transaction
                    scope.Dispose();
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(ex);
                    return Json(res);
                }
                }

                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = param.RegisterData; }
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

        //Add by Jutarat A. on 02102013
        public ActionResult ICS070_CreditNoteReport(string strFilePath)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                if (String.IsNullOrEmpty(strFilePath) == false)
                {
                    IDocumentHandler handlerDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                    Stream reportStream = handlerDoc.GetDocumentReportFileStream(strFilePath);
                    return File(reportStream, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        //End Add

        /// <summary>
        /// Reset session variable
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS070_Reset()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            try
            {
                ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
                param.doGetCreditNote = null;
                param.doGetTaxInvoiceForIC = null;
                param.strBillingCode = string.Empty;
                param.doBillingDetailList = null;
                param.decBalanceDeposit = null;

                param.doGetBillingCodeInfo = null;
                param.doGetBillingDetailOfInvoice = null;

                param.strBillingTargetCode = string.Empty;
                param.strInvoiceNo = string.Empty;
                param.strInvoiceOCC = string.Empty;
                param.bolNotRetrieveFlag = false;
                param.strCreditNoteType = string.Empty;
                param.strRegContent = string.Empty;
                
                param.WarningMessage = null;
                param.RegisterData = null;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Cancel adding deposit fee
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS070_DepositFeeCancel()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            try
            {
                ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
                param.doGetCreditNote = null;
                param.doGetTaxInvoiceForIC = null;
                param.strBillingCode = string.Empty;
                param.doBillingDetailList = null;
                param.decBalanceDeposit = null;

                param.doGetBillingCodeInfo = null;
                param.doGetBillingDetailOfInvoice = null;

                param.strBillingTargetCode = string.Empty;
                param.strInvoiceNo = string.Empty;
                param.strInvoiceOCC = string.Empty;
                param.bolNotRetrieveFlag = false;
                param.strCreditNoteType = string.Empty;
                param.strRegContent = string.Empty;

                param.WarningMessage = null;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Cancel adding except deposit fee
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS070_ExceptDepositFeeCancel()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            try
            {
                ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
                param.doGetCreditNote = null;
                param.doGetTaxInvoiceForIC = null;
                param.strBillingCode = string.Empty;
                param.doBillingDetailList = null;
                param.decBalanceDeposit = null;

                param.doGetBillingCodeInfo = null;
                param.doGetBillingDetailOfInvoice = null;

                param.strBillingTargetCode = string.Empty;
                param.strInvoiceNo = string.Empty;
                param.strInvoiceOCC = string.Empty;
                param.bolNotRetrieveFlag = false;
                param.strCreditNoteType = string.Empty;
                param.strRegContent = string.Empty;

                param.WarningMessage = null;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Cancel adding revenue
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS070_RevenueCancel()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            try
            {
                ICS070_ScreenParameter param = GetScreenObject<ICS070_ScreenParameter>();
                param.doGetCreditNote = null;
                param.doGetTaxInvoiceForIC = null;
                param.strBillingCode = string.Empty;
                param.doBillingDetailList = null;
                param.decBalanceDeposit = null;

                param.doGetBillingCodeInfo = null;
                param.doGetBillingDetailOfInvoice = null;

                param.strBillingTargetCode = string.Empty;
                param.strInvoiceNo = string.Empty;
                param.strInvoiceOCC = string.Empty;
                param.bolNotRetrieveFlag = false;
                param.strCreditNoteType = string.Empty;
                param.strRegContent = string.Empty;

                param.WarningMessage = null;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Update balance deposit of billing basic data into database
        /// </summary>
        /// <param name="billingCode">input billing code</param>
        /// <param name="dotbt_RevenueReturn">select tbt_RevenueReturn</param>
        /// <returns></returns>
        public bool ICS070_RevenueDepositFee(string billingCode, tbt_Revenue dotbt_RevenueReturn)
        {
            if (string.IsNullOrEmpty(billingCode))
                return false;

            //Convert to long format
            billingCode = new CommonUtil().ConvertBillingCode(billingCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            string contactCode = string.Empty;
            string billingOCC = string.Empty;
            if (string.IsNullOrEmpty(billingCode) == false)
            {
                contactCode = billingCode.Substring(0, billingCode.IndexOf('-'));
                billingOCC = billingCode.Substring(billingCode.IndexOf('-') + 1);
            }

            //Update deposit fee amount and insert deposit fee table
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            decimal? balanceDepositAfterUpdate = null;
            string balanceDepositAfterUpdateCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
            decimal? balanceDepositUsdAfterUpdate = null;

            //bool isUpdate = billingHandler.UpdateBalanceDepositOfBillingBasic(contactCode, billingOCC
            //, dotbt_RevenueReturn.RevenueAmountIncVAT * (-1), out balanceDepositAfterUpdate);  // (-1) -> Minus value

            // Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
            decimal adjustAmount = (dotbt_RevenueReturn.RevenueAmountIncVATVal - (dotbt_RevenueReturn.RevenueVATAmountVal ?? 0)) * (-1);
            decimal adjustAmountUsd = (dotbt_RevenueReturn.RevenueAmountIncVATVal - (dotbt_RevenueReturn.RevenueVATAmountVal ?? 0)) * (-1); // add by Jirawat Jannet @2016-11-02

            if (dotbt_RevenueReturn.RevenueAmountIncVATCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) adjustAmountUsd = 0;
            else adjustAmount = 0;


            bool isUpdate = billingHandler.UpdateBalanceDepositOfBillingBasic(contactCode, billingOCC
                , adjustAmount, adjustAmountUsd, dotbt_RevenueReturn.RevenueVATAmountCurrencyType, out balanceDepositAfterUpdate, out balanceDepositUsdAfterUpdate, out balanceDepositAfterUpdateCurrencyType);  // (-1) -> Minus value
            
            if (isUpdate)
            {
                //result of handler
                //return billingHandler.InsertDepositFeeRevenue(contactCode, billingOCC
                //    , dotbt_RevenueReturn.RevenueAmountIncVAT
                //    , balanceDepositAfterUpdate
                //    , dotbt_RevenueReturn.RevenueNo);

                // Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                // edit by jirawat jannet on 2016-11-02
                decimal revenueAmount = ((dotbt_RevenueReturn.RevenueAmountIncVAT ?? 0) - (dotbt_RevenueReturn.RevenueVATAmount ?? 0));
                decimal revenueAmountUsd = (dotbt_RevenueReturn.RevenueAmountIncVATUsd - (dotbt_RevenueReturn.RevenueVATAmountUsd ?? 0));
                return billingHandler.InsertDepositFeeRevenue(contactCode, billingOCC
                    , revenueAmount
                    , revenueAmountUsd
                    , dotbt_RevenueReturn.RevenueAmountIncVATCurrencyType
                    , (balanceDepositAfterUpdate ?? 0)
                    , (balanceDepositUsdAfterUpdate ?? 0)
                    , dotbt_RevenueReturn.RevenueAmountIncVATCurrencyType
                    , dotbt_RevenueReturn.RevenueNo); 
            }

            //fail
            return false;
        }
    }
}
