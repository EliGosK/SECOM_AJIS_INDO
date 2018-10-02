using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Income.Models;
using System.Transactions;
using System.Globalization;
using SECOM_AJIS.DataEntity.Billing;
using System.Web;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Xml;

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
        public ActionResult ICS020_Authority(ICS020_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                // Check permission
                if (!ICS020_IsAllowOperate(res))
                    return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return InitialScreenEnvironment<ICS020_ScreenParameter>("ICS020", param, res);
        }
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool ICS020_IsAllowOperate(ObjectResultData res)
        {
            if (res == null)
            {
                res = new ObjectResultData();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            }
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_IMPORT_PAYMENT_DATA, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return false;
            }
            return true;
        }
        #endregion

        #region Views
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS020")]
        public ActionResult ICS020()
        {
            return View();
        }
        /// <summary>
        /// Generate attach file section
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult ICS020_Upload(string k = "")
        {
            ViewBag.sKey = k;
            return View("ICS020_Upload");
        }
        #endregion

        #region Actions
        /// <summary>
        /// Generate xml of payment list of selected auto transfer type
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS020_InitialAutoTransferGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS020_autotransfer", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// Generate xml of payment list of selected bank transfer type
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS020_InitialBankTransferGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS020_banktransfer", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Generate SECOM bank/branch comboitem list upon selected import type
        /// </summary>
        /// <param name="SelectProcess">import type (auto transfer or bank transfer)</param>
        /// <returns></returns>
        public ActionResult ICS020_GetBankBranch(ICS020_ImportPaymentProcess SelectProcess)
        {
            try
            {
                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<doSECOMAccount> doSECOMAccount = null;
                if (SelectProcess == ICS020_ImportPaymentProcess.AutoTransfer)
                {
                    //return secom's bank (auto only)
                    doSECOMAccount = masterHandler.GetSECOMAccountAutoTransfer();
                }
                else if (SelectProcess == ICS020_ImportPaymentProcess.BankTransfer)
                {
                    //return all secom's bank
                    doSECOMAccount = masterHandler.GetSECOMAccountBankTransfer();
                }
                else
                {
                    //return blank combobox, do nothing
                    doSECOMAccount = new List<doSECOMAccount>();
                }

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doSECOMAccount>(doSECOMAccount, "Text", "SecomAccountID");
                return Json(cboModel);
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Submit selected payment data file (CSV format)
        /// </summary>
        /// <param name="fileSelect"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public ActionResult ICS020_AttachFile(HttpPostedFileBase fileSelect, string sKey)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();
            ICS020_ScreenParameter session = ICS020_GetScreenData(sKey);
            try
            {
                if (fileSelect == null || string.IsNullOrEmpty(fileSelect.FileName) ||
                    string.IsNullOrEmpty(Path.GetFileName(fileSelect.FileName)) || fileSelect.ContentLength == 0)
                {
                    MessageModel msgModel = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7011, null);
                    ViewBag.AttachResult = "Error";
                    ViewBag.MsgCode = msgModel.Code;
                    ViewBag.Message = msgModel.Message;
                }
                else if (!System.IO.Path.GetExtension(fileSelect.FileName).ToUpper().Equals(".CSV"))
                {
                    //MSG7075 File type is invalid. Please import csv file.
                    MessageModel msgModel = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7075, null);
                    ViewBag.AttachResult = "Error";
                    ViewBag.MsgCode = msgModel.Code;
                    ViewBag.Message = msgModel.Message;
                }
                else
                {
                    //Save file 
                    ICommonHandler commonHandle = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    session.CsvFilePath = PathUtil.GetPathValue(PathUtil.PathName.PaymentDataFile);
                    session.CsvFileName = string.Format("{0}_{1}{2}", System.IO.Path.GetFileNameWithoutExtension(fileSelect.FileName)
                        , DateTime.Now.ToString("yyyyMMdd_HHmmss")
                        , System.IO.Path.GetExtension(fileSelect.FileName));

                    if (Directory.Exists(session.CsvFilePath) == false)
                        Directory.CreateDirectory(session.CsvFilePath);

                    string savePathFile = Path.Combine(session.CsvFilePath, session.CsvFileName);
                    fileSelect.SaveAs(savePathFile);
                    ViewBag.AttachResult = "Success";
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                ViewBag.AttachResult = "Error";
                ViewBag.MsgCode = res.Message.Code;
                ViewBag.Message = res.Message.Message;
            }

            //ICS020_SetScreenData(session, sKey);
            ViewBag.sKey = sKey;
            ViewBag.CsvFileName = fileSelect.FileName;
            return View("ICS020_Upload");
        }

        /// <summary>
        /// Submit import content information
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS020_SubmitShowContentToGrid(ICS020_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            List<tbt_tmpImportContent> doTbt_TmpImportContent = new List<tbt_tmpImportContent>();

            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS020_IsAllowOperate(res))
                    return Json(res);

                #region Validate require field
                ValidatorUtil validator = new ValidatorUtil();
                if (CommonUtil.IsNullOrEmpty(param) || param.SECOMAccountID.HasValue == false)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS020"
                      , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                      , "SECOMAccountID", "lblBankBranch", "SECOMAccountID");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    return Json(res);
                }
                ICS020_ScreenParameter session = ICS020_GetScreenData();
                if (CommonUtil.IsNullOrEmpty(session) || session.CsvFileName == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7011);
                    return Json(res);
                }
                #endregion

                #region Load csv data file
                Guid importID = Guid.NewGuid();         //Genereate Import ID
                IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                string paymentType = param.SelectProcess.Value == ICS020_ImportPaymentProcess.AutoTransfer ? PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER : PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER;
                string csvFilePath = Path.Combine(session.CsvFilePath, session.CsvFileName);

                //Validate structure imported file
                IMasterHandler iMasterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                string bankCode = iMasterHandler.GetTbm_SecomBankAccount(param.SECOMAccountID.Value).First().BankCode;
                int loadResult = incomeHandler.LoadPaymentDataFile(importID, csvFilePath, param.SECOMAccountID.Value, paymentType, bankCode, param.CurrencyType);
                ICS020_DisplayErrorMessageImportShowPayment(validator, loadResult);

                if (param.SelectProcess.Value == ICS020_ImportPaymentProcess.AutoTransfer)
                {
                    //Validate invoice no. whole file
                    List<doValidateWholeFile> doValidateWholeFileRemaining = incomeHandler.ValidateWholeFile(importID);
                    if (doValidateWholeFileRemaining != null && doValidateWholeFileRemaining.Count > 0)
                    {
                        //Found remaining invoice whole file
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7114, null);
                    }
                }

                //Validate business
                doTbt_TmpImportContent = ICS020_ValidateBusiness(param.SelectProcess.Value, param.SECOMAccountID.Value, importID, validator, param.CurrencyType);
                #endregion

                //Success
                #region Set session
                session.ImportID = (Guid?)importID;
                session.SelectProcess = param.SelectProcess;
                session.SECOMAccountID = param.SECOMAccountID;
                session.ContentData = doTbt_TmpImportContent;
                session.CurrencyType = param.CurrencyType;
                ICS020_SetScreenData(session);
                res.ResultData = "1";
                #endregion
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Event when clilck import button, this function will import and register the payment of selected payment file to the system.
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS020_CmdImport()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            List<tbt_tmpImportContent> doTbt_TmpImportContent = new List<tbt_tmpImportContent>();
            ICS020_ScreenParameter session = ICS020_GetScreenData();

            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS020_IsAllowOperate(res))
                    return Json(res);

                #region Validate required field
                ValidatorUtil validator = new ValidatorUtil();
                if (session == null || !session.ImportID.HasValue)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7074);
                    return Json(res);
                }
                #endregion

                #region Set session & Validate business
                doTbt_TmpImportContent = ICS020_ValidateBusiness(session.SelectProcess.Value, session.SECOMAccountID.Value, session.ImportID.Value, validator, session.CurrencyType);
                session.ContentData = doTbt_TmpImportContent;
                ICS020_SetScreenData(session);

                if (doTbt_TmpImportContent == null || doTbt_TmpImportContent.Count == 0)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7074, null);

                if (doTbt_TmpImportContent.Where(d => string.IsNullOrEmpty(d.ImportErrorReason) == false).Count() > 0)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7105, null);
                #endregion

                #region Import
                Guid importID = session.ImportID.Value;
                IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                if (session.SelectProcess.Value == ICS020_ImportPaymentProcess.AutoTransfer)
                {
                    #region Auto Transfer
                    //Validate business
                    bool isAlreadyImported = incomeHandler.CheckAutoTransferFileImported(importID, session.SECOMAccountID.Value);
                    if (isAlreadyImported)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7012);
                        return Json(res);
                    }

                    using (TransactionScope scope = new TransactionScope())
                    {
                    tbt_tmpImportContent currentItem = null;
                        try
                        {
                            #region Insert Tbt_PaymentImportFile
                            DateTime paymentDate = doTbt_TmpImportContent.First().PaymentDate.Value;
                            tbt_PaymentImportFile importFile = new tbt_PaymentImportFile()
                            {
                                ImportID = importID,
                                PaymentType = PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER,
                                SECOMAccountID = session.SECOMAccountID.Value,
                                PaymentDate = paymentDate,
                                FilePath = session.CsvFilePath,
                                FileName = session.CsvFileName,
                                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                            };
                            List<tbt_PaymentImportFile> importFilelist = new List<tbt_PaymentImportFile>();
                            importFilelist.Add(importFile);
                            incomeHandler.InsertTbt_PaymentImportFile(importFilelist);
                            #endregion

                            #region Tbt_TmpImportContent
                            foreach (var item in doTbt_TmpImportContent)
                            {
                                currentItem = item;
                                if (item.AutoTransferResult == AutoTransferResult.C_AUTO_TRANSFER_RESULT_OK)
                                {
                                    #region Transfer Status: OK
                                    tbt_Payment doPayment = ICS020_RegisterPayment(PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER, session.SECOMAccountID, item);
                                    if (doPayment != null)
                                    {
                                        bool isMatched = ICS020_MatchPaymentAutoTransfer(doPayment, item.RefInvoiceNo,item);
                                        if (!isMatched)
                                        {
                                            ICS020_ShowImportErrorReason(MessageUtil.MessageList.MSG7024, item);
                                            break;
                                        }
                                        billingHandler.UpdateAutoTransferAccountLastResult(item.RefInvoiceNo, AutoTransferResult.C_AUTO_TRANSFER_RESULT_OK);
                                    }
                                    else
                                    {
                                        ICS020_ShowImportErrorReason(MessageUtil.MessageList.MSG7023, item);
                                    }
                                    #endregion
                                }
                                else if (item.AutoTransferResult == AutoTransferResult.C_AUTO_TRANSFER_RESULT_FAIL)
                                {
                                    #region Transfer Status: Fail
                                    tbt_Invoice doTbt_Invoice = billingHandler.GetTbt_InvoiceData(item.RefInvoiceNo, null);
                                    List<tbt_BillingDetail> doTbt_BillingDetails = billingHandler.GetTbt_BillingDetailOfInvoice(doTbt_Invoice.InvoiceNo, doTbt_Invoice.InvoiceOCC);
                                    bool isUpdated = billingHandler.UpdateInvoicePaymentStatus(doTbt_Invoice, doTbt_BillingDetails, PaymentStatus.C_PAYMENT_STATUS_CANCEL);
                                    if (!isUpdated)
                                    {
                                        res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7026);
                                        return Json(res);
                                    }
                                    doInvoice doInvoice = billingHandler.GetInvoice(item.RefInvoiceNo);
                                    //bool isTaxInvoiceIssued = billingHandler.CheckTaxInvoiceIssued(doInvoice);
                                    bool isTaxInvoiceIssued = billingHandler.CheckInvoiceIssuedTaxInvoice(doInvoice.InvoiceNo, doInvoice.InvoiceOCC);
                                    bool isGenInvoice = ICS020_GenerateInvoiceNewOCC(doTbt_Invoice, doTbt_BillingDetails, isTaxInvoiceIssued);
                                    if (!isGenInvoice)
                                    {
                                        res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7027, new string[] { doInvoice.InvoiceNo });
                                        return Json(res);
                                    }
                                    billingHandler.UpdateAutoTransferAccountLastResult(item.RefInvoiceNo, AutoTransferResult.C_AUTO_TRANSFER_RESULT_FAIL);
                                    #endregion
                                }
                            }
                        #endregion

                        //Success
                        scope.Complete();
                    }
                    catch (Exception ex)
                        {
                            //Fail, rollback all transaction
                            currentItem.ImportErrorReason = ex.Message;
                        scope.Dispose();
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7104, null);
                        }
                    }
                    #endregion
                }
                else if (session.SelectProcess.Value == ICS020_ImportPaymentProcess.BankTransfer)
                {
                    #region Bank Transfer
                    using (TransactionScope scope = new TransactionScope())
                    {
                    tbt_tmpImportContent currentItem = null;
                        try
                        {
                            #region Tbt_PaymentImportFile
                            DateTime paymentDate = doTbt_TmpImportContent.First().PaymentDate.Value;
                            tbt_PaymentImportFile importFile = new tbt_PaymentImportFile()
                            {
                                ImportID = importID,
                                PaymentType = PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER,
                                SECOMAccountID = session.SECOMAccountID.Value,
                                PaymentDate = null,
                                FilePath = session.CsvFilePath,
                                FileName = session.CsvFileName,
                                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                            };

                            List<tbt_PaymentImportFile> importFilelist = new List<tbt_PaymentImportFile>();
                            importFilelist.Add(importFile);
                            incomeHandler.InsertTbt_PaymentImportFile(importFilelist);
                            #endregion

                            #region Tbt_TmpImportContent
                            foreach (var item in doTbt_TmpImportContent)
                            {
                                tbt_Payment doPayment = ICS020_RegisterPayment(PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER, session.SECOMAccountID, item);
                                if (doPayment != null)
                                {
                                    doInvoice doInvoice = billingHandler.GetInvoice(item.RefInvoiceNo);
                                    tbt_Invoice doTbtInvoice = CommonUtil.CloneObject<doInvoice, tbt_Invoice>(doInvoice);
                                    if (doInvoice != null && (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                                                            || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN))
                                    {
                                        ICS020_ShowImportErrorReason(MessageUtil.MessageList.MSG7025, item);
                                    }
                                    else if (doInvoice != null && doInvoice.InvoiceAmountCurrencyType == item.PaymentAmountCurrencyType
                                            && (
                                            (doInvoice.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && ((item.PaymentAmount ?? 0) + (item.WHTAmount ?? 0)) == ((doInvoice.InvoiceAmount ?? 0) + (doInvoice.VatAmount ?? 0)))
                                            || (doInvoice.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US && ((item.PaymentAmountUsd ?? 0) + (item.WHTAmountUsd ?? 0)) == ((doInvoice.InvoiceAmountUsd ?? 0) + (doInvoice.VatAmountUsd ?? 0)))))
                                    {
                                        bool isAddMatch = ICS020_MatchPaymentBankTransfer(doPayment, doTbtInvoice, item);
                                        if (!isAddMatch)
                                        {
                                            ICS020_ShowImportErrorReason(MessageUtil.MessageList.MSG7024, item);
                                        }
                                    }
                                    else
                                    {
                                        ICS020_ShowImportErrorReason(MessageUtil.MessageList.MSG7025, item);
                                    }
                                }
                                else
                                {
                                    ICS020_ShowImportErrorReason(MessageUtil.MessageList.MSG7023, item);
                                }
                            }
                        #endregion

                        //Success
                        scope.Complete();
                    }
                    catch (Exception ex)
                        {
                            //Fail, rollback all transaction
                            currentItem.ImportErrorReason = ex.Message;
                        scope.Dispose();
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7104, null);
                        }
                    }
                #endregion
                }
                #endregion

                //Success
                res.ResultData = "1";
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Generate xml of payment list of selected payment data file (CSV Format)
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS020_ShowContentToGrid()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            ICS020_ScreenParameter session = ICS020_GetScreenData();
            List<tbt_tmpImportContent> doTbt_TmpImportContent = new List<tbt_tmpImportContent>();
            try
            {
                if (session != null || session.ContentData != null)
                    doTbt_TmpImportContent = session.ContentData;
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            //Return result
            string xmlTemplate = string.Empty;
            if (session.SelectProcess.Value == ICS020_ImportPaymentProcess.AutoTransfer)
            {
                xmlTemplate = "Income\\ICS020_autotransfer";
            }
            else if (session.SelectProcess.Value == ICS020_ImportPaymentProcess.BankTransfer)
            {
                xmlTemplate = "Income\\ICS020_banktransfer";
            }
            res.ResultData = CommonUtil.ConvertToXml<tbt_tmpImportContent>(doTbt_TmpImportContent, xmlTemplate, CommonUtil.GRID_EMPTY_TYPE.INSERT);
            return Json(res);
        }

        /// <summary>
        /// Event when click reset button, this function will reset the screen to be the same screen as opening the screen.
        /// </summary>
        /// <param name="SelectProcess"></param>
        /// <returns></returns>
        public ActionResult ICS020_CmdReset(ICS020_ImportPaymentProcess SelectProcess)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            try
            {
                ICS020_ClearScreenData();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
            string xmlTemplate = string.Empty;
            if (SelectProcess == ICS020_ImportPaymentProcess.AutoTransfer)
            {
                xmlTemplate = "Income\\ICS020_autotransfer";
            }
            else if (SelectProcess == ICS020_ImportPaymentProcess.AutoTransfer)
            {
                xmlTemplate = "Income\\ICS020_banktransfer";
            }
            res.ResultData = CommonUtil.ConvertToXml<tbt_tmpImportContent>(new List<tbt_tmpImportContent>(), xmlTemplate, CommonUtil.GRID_EMPTY_TYPE.INSERT);
            return Json(res);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Retrieve inputted screen session
        /// </summary>
        /// <param name="key">session key (optional)</param>
        /// <returns></returns>
        private ICS020_ScreenParameter ICS020_GetScreenData(string key = null)
        {
            //Specific key when calling by iframe
            return GetScreenObject<ICS020_ScreenParameter>(key);
        }
        /// <summary>
        /// Set inputted screen session to the system
        /// </summary>
        /// <param name="param"></param>
        /// <param name="key"></param>
        private void ICS020_SetScreenData(ScreenParameter param, string key = null)
        {
            //UpdateScreenObject(param, key);
        }
        /// <summary>
        /// Clear inputted screen session 
        /// </summary>
        /// <param name="key"></param>
        private void ICS020_ClearScreenData(string key = null)
        {
            ICS020_ScreenParameter session = ICS020_GetScreenData(key);
            session.SelectProcess = null;
            session.SECOMAccountID = null;
            session.ImportID = null;
            session.CsvFileName = null;
            session.ContentData = new List<tbt_tmpImportContent>();
            //ICS020_SetScreenData(session, key);
        }

        /// <summary>
        /// Register payment information of specific payment content data.
        /// </summary>
        /// <param name="paymentType">payment type</param>
        /// <param name="SECOMAccountID">SECOM account id</param>
        /// <param name="doTmpImport">payment content data</param>
        /// <returns></returns>
        public tbt_Payment ICS020_RegisterPayment(string paymentType, int? SECOMAccountID, tbt_tmpImportContent doTmpImport)
        {
            tbt_Payment doPayment = new tbt_Payment()
            {
                PaymentTransNo = string.Empty,
                PaymentType = paymentType,
                PaymentDate = doTmpImport.PaymentDate.Value,
                SECOMAccountID = SECOMAccountID,
                Payer = doTmpImport.Payer,
                PayerBankAccNo = doTmpImport.PayerBankAccNo,
                SendingBankCode = doTmpImport.SendingBankCode,
                SendingBranchCode = doTmpImport.SendingBranchCode,
                TelNo = doTmpImport.TelNo,
                Memo = doTmpImport.RefInvoiceNo,
                SystemMethod = PaymentSystemMethod.C_INC_PAYMENT_IMPORT,
                RefInvoiceNo = doTmpImport.RefInvoiceNo,
                MatchableBalance = doTmpImport.PaymentAmount ?? 0,
                MatchableBalanceUsd = doTmpImport.PaymentAmountUsd ?? 0,
                BankFeeRegisteredFlag = false,
                OtherIncomeRegisteredFlag = false,
                OtherExpenseRegisteredFlag = false,
                DeleteFlag = false,

                PaymentAmountCurrencyType = doTmpImport.PaymentAmountCurrencyType,
                PaymentAmount = doTmpImport.PaymentAmount,
                PaymentAmountUsd = doTmpImport.PaymentAmountUsd,

                MatchableBalanceCurrencyType = doTmpImport.PaymentAmountCurrencyType
            };
            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            List<tbt_Payment> paymentList = new List<tbt_Payment>();
            paymentList.Add(doPayment);
            List<tbt_Payment> result = incomeHandler.RegisterPayment(paymentList);
            if (result != null && result.Count > 0)
                return result[0];
            return null;
        }
        /// <summary>
        /// Generate payment matching information of specific payment information and invoice no. 
        /// </summary>
        /// <param name="doPayment">payment information</param>
        /// <param name="invoiceNo">invoice no.</param>
        /// <returns></returns>
        public bool ICS020_MatchPaymentAutoTransfer(tbt_Payment doTbt_Payment, string invoiceNo, tbt_tmpImportContent doTbt_TmpImportContent)
        {
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            doInvoice doInvoice = billingHandler.GetInvoice(invoiceNo);

            #region Prepare match payment info
            doMatchPaymentDetail matchDetail = new doMatchPaymentDetail()
            {
                MatchID = null,
                InvoiceNo = doInvoice.InvoiceNo,
                InvoiceOCC = doInvoice.InvoiceOCC,
                MatchAmountExcWHT = doTbt_TmpImportContent.PaymentAmount.GetValueOrDefault(0),
                MatchAmountIncWHT = doTbt_TmpImportContent.PaymentAmount.GetValueOrDefault(0) + doTbt_TmpImportContent.WHTAmount.GetValueOrDefault(0),
                MatchAmountExcWHTUsd = doTbt_TmpImportContent.PaymentAmountUsd.GetValueOrDefault(0),
                MatchAmountIncWHTUsd = doTbt_TmpImportContent.PaymentAmountUsd.GetValueOrDefault(0) + doTbt_TmpImportContent.WHTAmountUsd.GetValueOrDefault(0),
                WHTAmount = doTbt_TmpImportContent.WHTAmount,
                MatchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL,
                CancelFlag = false,
                CancelApproveNo = null,
                CorrectionReason = null,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,

                WHTAmountUsd = doTbt_TmpImportContent.WHTAmountUsd,

                WHTAmountCurrencyType = doTbt_TmpImportContent.PaymentAmountCurrencyType,
                MatchAmountExcWHTCurrencyType = doTbt_TmpImportContent.PaymentAmountCurrencyType,
                MatchAmountIncWHTCurrencyType = doTbt_TmpImportContent.PaymentAmountCurrencyType
            };
            doMatchPaymentHeader matchHeader = new doMatchPaymentHeader()
            {
                MatchID = null,
                MatchDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                PaymentTransNo = doTbt_Payment.PaymentTransNo,
                TotalMatchAmount = doTbt_Payment.PaymentAmount,
                BankFeeAmount = null,
                SpecialProcessFlag = false,
                ApproveNo = null,
                OtherExpenseAmount = null,
                OtherIncomeAmount = null,
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

            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            bool isSuccess = incomeHandler.MatchPaymentInvoices(matchHeader, doTbt_Payment);
            return isSuccess;
        }
        /// <summary>
        /// Genereate new invoice occc of specific invoice information.
        /// </summary>
        /// <param name="doTbt_Invoice">invoice information</param>
        /// <param name="doTbt_BillingDetails">billing detail informatioin</param>
        /// <param name="isTaxInvoiceIssued">is already issued tax invoice</param>
        /// <returns></returns>
        public bool ICS020_GenerateInvoiceNewOCC(tbt_Invoice doTbt_Invoice, List<tbt_BillingDetail> doTbt_BillingDetails, bool isTaxInvoiceIssued)
        {
            if (doTbt_Invoice == null)
                return false;

            //Dummy concept to support ef,
            List<tbt_BillingDetail> newBillingDetails = CommonUtil.ClonsObjectList<tbt_BillingDetail, tbt_BillingDetail>(doTbt_BillingDetails);
            if (newBillingDetails != null)
            {
                foreach (var item in newBillingDetails)
                {
                    item.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK;
                    item.BillingDetailNo = 0;
                    //item.InvoiceOCC = null; //31-May-2013 Edit by Patcharee T. Send old Inv OCC to ManageInvoiceByCommand for check Tax invoice
                    //item.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime; //01-Apr-2013 Edit by Patcharee T. Use old issue invoice date
                    item.PaymentMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                    item.AutoTransferDate = null;
                    item.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    item.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    item.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    item.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }
            }

            //Dummy concept to support ef,
            tbt_Invoice newdoTbt_Invoice = CommonUtil.CloneObject<tbt_Invoice, tbt_Invoice>(doTbt_Invoice);
            newdoTbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK;
            //newdoTbt_Invoice.InvoiceOCC = 0; //31-May-2013 Edit by Patcharee T. Send old Inv OCC to ManageInvoiceByCommand for check Tax invoice
            //newdoTbt_Invoice.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime; //01-Apr-2013 Edit by Patcharee T. Use old issue invoice date
            newdoTbt_Invoice.AutoTransferDate = null;
            newdoTbt_Invoice.PaymentMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
            newdoTbt_Invoice.FirstIssueInvFlag = false;
            newdoTbt_Invoice.FirstIssueInvDate = null;
            newdoTbt_Invoice.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            newdoTbt_Invoice.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            //Generate new invoice
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            tbt_Invoice resultInvoice = billingHandler.ManageInvoiceByCommand(newdoTbt_Invoice, newBillingDetails, isTaxInvoiceIssued);
            return resultInvoice != null;
        }
        /// <summary>
        /// Genereate payment matching information of specific payment information and invoice information.
        /// </summary>
        /// <param name="doPayment">payment information</param>
        /// <param name="doInvoice">invoice information</param>
        /// <returns></returns>
        public bool ICS020_MatchPaymentBankTransfer(tbt_Payment doTbt_Payment, tbt_Invoice doTbt_Invoice, tbt_tmpImportContent doTbt_TmpImportContent)
        {
            if (doTbt_Payment == null || doTbt_Invoice == null)
                return false;

            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            doInvoice doInvoice = billingHandler.GetInvoice(doTbt_Invoice.InvoiceNo);

            #region Prepare match payment info
            doMatchPaymentDetail matchDetail = new doMatchPaymentDetail()
            {
                MatchID = null,
                InvoiceNo = doInvoice.InvoiceNo,
                InvoiceOCC = doInvoice.InvoiceOCC,
                MatchAmountExcWHT = doTbt_TmpImportContent.PaymentAmount.GetValueOrDefault(0),
                MatchAmountIncWHT = doTbt_TmpImportContent.PaymentAmount.GetValueOrDefault(0) + doTbt_TmpImportContent.WHTAmount.GetValueOrDefault(0),
                MatchAmountExcWHTUsd = doTbt_TmpImportContent.PaymentAmountUsd.GetValueOrDefault(0),
                MatchAmountIncWHTUsd = doTbt_TmpImportContent.PaymentAmountUsd.GetValueOrDefault(0) + doTbt_TmpImportContent.WHTAmountUsd.GetValueOrDefault(0),
                MatchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL,
                CancelFlag = false,
                CancelApproveNo = null,
                CorrectionReason = null,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,

                WHTAmount = doTbt_TmpImportContent.WHTAmount,
                WHTAmountUsd = doTbt_TmpImportContent.WHTAmountUsd,
                WHTAmountCurrencyType = doTbt_TmpImportContent.WHTAmountCurrencyType,

                MatchAmountExcWHTCurrencyType = doTbt_TmpImportContent.PaymentAmountCurrencyType,
                MatchAmountIncWHTCurrencyType = doTbt_TmpImportContent.PaymentAmountCurrencyType
            };
            doMatchPaymentHeader matchHeader = new doMatchPaymentHeader()
            {
                MatchID = null,
                MatchDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                PaymentTransNo = doTbt_Payment.PaymentTransNo,
                BankFeeAmount = null,
                SpecialProcessFlag = false,
                ApproveNo = null,
                OtherExpenseAmount = null,
                OtherIncomeAmount = null,
                CancelFlag = false,
                CancelApproveNo = null,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,

                TotalMatchAmount = doTbt_Payment.PaymentAmount,
                TotalMatchAmountUsd = doTbt_Payment.PaymentAmountUsd,
                TotalMatchAmountCurrencyType = doTbt_Payment.PaymentAmountCurrencyType,

                BankFeeAmountUsd = null,
                BankFeeAmountCurrencyType = null,
                OtherExpenseAmountUsd = null,
                OtherExpenseAmountCurrencyType = null,
                OtherIncomeAmountUsd = null,
                OtherIncomeAmountCurrencyType = null
            };
            matchHeader.MatchPaymentDetail = new List<doMatchPaymentDetail>();
            matchHeader.MatchPaymentDetail.Add(matchDetail);
            #endregion

            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            bool isSuccess = incomeHandler.MatchPaymentInvoices(matchHeader,doTbt_Payment);
            return isSuccess;
        }

        /// <summary>
        /// Validate business of inputted payment information
        /// </summary>
        /// <param name="selectProcess">import type (auto transfer or bank transfer)</param>
        /// <param name="secomAccountID">SECOM account id</param>
        /// <param name="importID">import id</param>
        /// <param name="validator"></param>
        /// <returns></returns>
        private List<tbt_tmpImportContent> ICS020_ValidateBusiness(ICS020_ImportPaymentProcess selectProcess, int secomAccountID, Guid importID, ValidatorUtil validator, string curencyType)
        {
            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler; // add by jirawat jannet @ 2016-10-07

            List<tbt_tmpImportContent> result = null;
            if (selectProcess == ICS020_ImportPaymentProcess.AutoTransfer)
            {
                //Auto Transfer
                bool isAlreadyImported = incomeHandler.CheckAutoTransferFileImported(importID, secomAccountID);
                if (isAlreadyImported)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7012, null);
                }
                else
                {
                    incomeHandler.ValidateAutoTransferContent(importID, secomAccountID);
                    result = incomeHandler.GetTbt_tmpImportContent(importID);

                    #region Display doTbt_TmpImportContent in grid
                    if (result != null)
                    {
                        string msg7076 = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7076, null).Message;
                        string msg7013 = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7013, null).Message;
                        string msg7113 = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7113, null).Message;
                        string msg7118 = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7118, null).Message;

                        foreach (var item in result)
                        {
                            if (item.PaymentDate.Value > DateTime.Now.Date)
                            {
                                item.ImportErrorReason = msg7076;
                            }
                            else if (item.ValidationResult == ValidationImportResult.C_PAYMENT_IMPORT_NO_ERROR)
                            {
                                item.ImportErrorReason = null;
                            }
                            else if (item.ValidationResult == ValidationImportResult.C_PAYMENT_IMPORT_ERROR_INVOICE_AMOUNT_UNMATCH)
                            {
                                item.ImportErrorReason = msg7013;
                            }
                            else if (item.ValidationResult == ValidationImportResult.C_PAYMENT_IMPORT_ERROR_PAY_DATE_UNMATCH)
                            {
                                item.ImportErrorReason = msg7113;
                            }
                            else if (item.ValidationResult == ValidationImportResult.C_PAYMENT_IMPORT_ERROR_BANK_UNMATCH)
                            {
                                item.ImportErrorReason = msg7118;
                            }    
                        }
                    }
                    #endregion
                }
            }
            else if (selectProcess == ICS020_ImportPaymentProcess.BankTransfer)
            {
                //Bank Transfer
                incomeHandler.ValidateBankTransferContent(importID);
                result = incomeHandler.GetTbt_tmpImportContent(importID);

                #region Display doTbt_TmpImportContent in grid
                if (result != null)
                {
                    string msg7076 = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7076, null).Message;
                    string msg7014 = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7014, null).Message;
                    string msg7015 = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7015, null).Message;
                    string msg7016 = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7016, null).Message;
                    string msg7112 = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7112, null).Message;
                    foreach (var item in result)
                    {
                        if (item.PaymentDate.Value > DateTime.Now.Date)
                        {
                            item.ImportErrorReason = msg7076;
                        }
                        else if (item.ValidationResult == ValidationImportResult.C_PAYMENT_IMPORT_NO_ERROR)
                        {
                            item.ImportErrorReason = null;
                        }
                        else if (item.ValidationResult == ValidationImportResult.C_PAYMENT_IMPORT_ERROR_INVALID_INVOICE)
                        {
                            item.ImportErrorReason = msg7014;
                        }
                        else if (item.ValidationResult == ValidationImportResult.C_PAYMENT_IMPORT_ERROR_IMPORTED_INVOICE)
                        {
                            item.ImportErrorReason = msg7015;
                        }
                        else if (item.ValidationResult == ValidationImportResult.C_PAYMENT_IMPORT_ERROR_PAID_INVOICE)
                        {
                            item.ImportErrorReason = msg7016;
                        }
                        else if (item.ValidationResult == ValidationImportResult.C_PAYMENT_IMPORT_ERROR_INCORRECT_STATUS)
                        {
                            item.ImportErrorReason = msg7112;
                        }
                    }
                }
                #endregion
            }

            // Add by Jirawat Jannet @ 2016-10-04
            #region Validate invoice currency type

            //string MSG7128 = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7128, null).Message;

            //foreach (var item in result)
            //{
            //    doInvoice doInvoice = billingHandler.GetInvoice(item.RefInvoiceNo);

            //    if (curencyType != doInvoice.InvoiceAmountCurrencyType)
            //    {
            //        item.ImportErrorReason = MSG7128;
            //    }
            //}

            #endregion

            // Add by Jirawat Jannet @ 2016-10-04
            #region Set currency type 

            foreach (var item in result)
            {
                item.PaymentAmountCurrencyType = curencyType;
                item.WHTAmountCurrencyType = curencyType;
            }

            #endregion


            return result;
        }
        /// <summary>
        /// Mapping loading result code to error message.
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="loadResult">loading result code</param>
        private void ICS020_DisplayErrorMessageImportShowPayment(ValidatorUtil validator, int loadResult)
        {
            MessageUtil.MessageList? msgID = null;
            switch (loadResult)
            {
                case -1:
                    msgID = MessageUtil.MessageList.MSG7018;
                    break;
                case -2:
                    msgID = MessageUtil.MessageList.MSG7019;
                    break;
                case -3:
                    msgID = MessageUtil.MessageList.MSG7020;
                    break;
                case -4:
                    msgID = MessageUtil.MessageList.MSG7021;
                    break;
                //case -5:
                //    msgID = MessageUtil.MessageList.MSG7022;
                //    break;
                case -6:
                    msgID = MessageUtil.MessageList.MSG7117;
                    break;
            }
            if (msgID.HasValue)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, msgID.Value, null);
        }
        /// <summary>
        /// Set import error reason to specific payment content data.
        /// </summary>
        /// <param name="msgID"></param>
        /// <param name="doTmpImport"></param>
        public void ICS020_ShowImportErrorReason(MessageUtil.MessageList msgID, tbt_tmpImportContent doTmpImport)
        {
            string errorMsg = MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, msgID, null).Message;
            doTmpImport.ImportErrorReason = errorMsg;
        }
        #endregion

    }   
}