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
using System.IO;

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
        public ActionResult ICS090_Authority(ICS090_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                // Check permission
                if (!ICS090_IsAllowOperate(res))
                    return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<ICS090_ScreenParameter>("ICS090", param, res);
        }
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool ICS090_IsAllowOperate(ObjectResultData res)
        {
            //Check Permission 
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_CANCEL_PAYMENT_MATCHING, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return false;
            }

            return true;
        }
        #endregion

        #region View
        /// <summary>
        /// Genereate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS090")]
        public ActionResult ICS090()
        {
            ICS090_ScreenParameter param = GetScreenObject<ICS090_ScreenParameter>();
            return View();
        }
        #endregion

        #region Actions
        /// <summary>
        /// Retrieve invoice information of specfic invoice no. and inputted infoice information
        /// </summary>
        /// <param name="param">invoice no. and inputted invoice information</param>
        /// <returns></returns>
        public ActionResult ICS090_GetInvoice(ICS090_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ICS090_InvoiceInfo invoiceInfo = new ICS090_InvoiceInfo();
            try
            {
                //Validate invoice business
                invoiceInfo = ICS090_ValidateInvoiceBusiness(param, res, false);
                if (res.IsError || invoiceInfo == null)
                    return Json(res);

                //Pass
                res.ResultData = invoiceInfo;
                return Json(res);
            }
            catch (Exception ex)
            {
                res = new ObjectResultData();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Generate correction reason comboitem list upon payment status of specific invoice no.
        /// </summary>
        /// <param name="paymentStatus">payment status</param>
        /// <returns></returns>
        public ActionResult ICS090_GetCorrectionReason(string paymentStatus)
        {
            try
            {
                //Get misc type
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_CORRECTION_REASON,
                        ValueCode = "%"
                    }
                };
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> correctionList = new List<doMiscTypeCode>();
                if (paymentStatus == PaymentStatus.C_PAYMENT_STATUS_BANK_PAID
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_AUTO_PAID
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_CASH_PAID
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_CHEQUE_PAID
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_CASHIER_PAID
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_AUTO_FAIL_BANK_PAID
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL_BANK_PAID
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_FAIL_BANK_PAID
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND
                        || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID)
                {
                    correctionList = commonHandler.GetMiscTypeCodeList(miscs).Where(d => d.ValueCode == CorrectionReason.C_CORRECTION_REASON_MISTAKE).ToList();
                }
                else if (paymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED
                         || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED)
                {
                    //All, Except encash
                    correctionList = commonHandler.GetMiscTypeCodeList(miscs).Where(d=> d.ValueCode !=CorrectionReason.C_CORRECTION_REASON_ENCASH_MISTAKE).ToList();
                }
                else if (paymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED
                         || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED)
                {
                    //Encash only
                    correctionList = commonHandler.GetMiscTypeCodeList(miscs).Where(d => d.ValueCode == CorrectionReason.C_CORRECTION_REASON_ENCASH_MISTAKE).ToList();
                }

                //Set correction reason combobox
                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(correctionList, "ValueDisplay", "ValueCode");

                //Success, return result
                return Json(cboModel);
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Retrieve next payment status description of specific current payment status and correction reason 
        /// </summary>
        /// <param name="paymentStatus">current payment status</param>
        /// <param name="correctionReason">correction reason</param>
        /// <returns></returns>
        public ActionResult ICS090_GetNextPaymentStatus(string paymentStatus, string correctionReason)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                string nextPaymentStatus = ICS090_GetCancelPaymentNextStatus(paymentStatus, correctionReason);
                res.ResultData = ICS090_GetPaymentStatusDesctription(nextPaymentStatus);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Event when click confirm cancel button, this function will cancel the payment matching information of specific invoice no. and inputted cancelling information to the system.
        /// </summary>
        /// <param name="param">invoice no. and inputted cancelling information</param>
        /// <returns></returns>
        public ActionResult ICS090_cmdConfirmCancel(ICS090_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS090_IsAllowOperate(res))
                    return Json(res);


                //Validate invoice business
                ICS090_InvoiceInfo invoiceInfo = ICS090_ValidateInvoiceBusiness(param, res, true);
                if (res.IsError || invoiceInfo == null || invoiceInfo.doInvoice == null)
                    return Json(res);


                //Connect db
                doInvoice doInvoice = invoiceInfo.doInvoice;
                using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                        IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                        //Comment by Jutarat A. on 25122013
                        /*if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED
                            || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED)
                        {
                            #region Encash

                            var incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                            bool isSuccess = incomeHandler.CancelPaymentMatching(doInvoice, param.CorrectionReason, param.ApproveNo);
                            if (!isSuccess)
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7116, null);

                            //Success, return result
                            scope.Complete();
                            res.ResultData = "1";
                            return Json(res);
                            #endregion
                        }
                        else
                        {*/
                        //End Comment
                            #region Other case 
                            //Cancel payment matching
                            bool isTaxInvoiceIssued = billingHandler.CheckInvoiceIssuedTaxInvoice(doInvoice.InvoiceNo, doInvoice.InvoiceOCC);

                            var incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                            bool isSuccess = incomeHandler.CancelPaymentMatching(doInvoice, param.CorrectionReason, param.ApproveNo);
                            if (!isSuccess)
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7042, null);


                            //Create new invoice (with pdf)
                            tbt_Invoice doTbt_Invoice = ICS090_CreateNewInvoice(doInvoice, param.CorrectionReason, isTaxInvoiceIssued);
                            if (doTbt_Invoice == null)
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7043, null);


                    //Success, return result
                    scope.Complete();
                    if ((doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                                    || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK))
                            {
                                //Success, Display PDF and information
                                ICS090_ScreenParameter screenSession = GetScreenObject<ICS090_ScreenParameter>();
                                screenSession.PDFFilePath = doTbt_Invoice.FilePath;
                                res.ResultData = "PDF";
                                return Json(res);
                            }
                            else
                            {
                                //Success, Display information popup only
                                res.ResultData = "1";
                                return Json(res);
                            }
                            #endregion
                        //}
                    }
                    catch (Exception ex)
                    {
                    scope.Dispose();
                    throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        
        /// <summary>
        /// Display invoice/tax invoice pdf report
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS090_DisplayReport()
        {
            ICS090_ScreenParameter screenSession = GetScreenObject<ICS090_ScreenParameter>();
            if (screenSession != null && !string.IsNullOrEmpty(screenSession.PDFFilePath))
            {
                IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                Stream filestream = handlerDocument.GetDocumentReportFileStream(screenSession.PDFFilePath);
                return File(filestream, "application/pdf");
            }
            else
            {
                //Do nothing
                ObjectResultData res = new ObjectResultData();
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                return Json(res);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create new invoice with specific invoice informaion and cancelling information
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="correctionReason">correction reason</param>
        /// <param name="isTaxInvoiceIssued">is already issued tax invoice</param>
        /// <returns></returns>
        public tbt_Invoice ICS090_CreateNewInvoice(doInvoice doInvoice, string correctionReason, bool isTaxInvoiceIssued)
        {
            var billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            tbt_Invoice doTbt_Invoice = CommonUtil.CloneObject<doInvoice, tbt_Invoice>(doInvoice);
            List<tbt_BillingDetail> tbt_BillingDetails = doInvoice.Tbt_BillingDetails;

            if (doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_BANK_PAID
                || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_CASH_PAID
                || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_CHEQUE_PAID
                || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_CASHIER_PAID
                || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_AUTO_FAIL_BANK_PAID
                || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL_BANK_PAID
                || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_FAIL_BANK_PAID
                || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN
                || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND
                || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID)
            {
                #region PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                //doTbt_Invoice.InvoiceOCC = 0; //01-Apr-2013 Edit by Patcharee T. Use old tax invoice and reciept
                doTbt_Invoice.IssueInvFlag = false;
                //doTbt_Invoice.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;  //01-Apr-2013 Edit by Patcharee T. Use old issue invoice date

                // edit by Jirawat Jannet on 2016-10-27
                if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    doTbt_Invoice.PaidAmountIncVat = 0;
                else if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doTbt_Invoice.PaidAmountIncVatUsd = 0;

                if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    doTbt_Invoice.RegisteredWHTAmount = 0;
                else if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doTbt_Invoice.RegisteredWHTAmountUsd = 0;
                // end edit

                doTbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                doTbt_Invoice.FirstIssueInvFlag = false;
                doTbt_Invoice.FirstIssueInvDate = null;

                //Dummy concept to support ef,
                List<tbt_BillingDetail> newBillingDetails = CommonUtil.ClonsObjectList<tbt_BillingDetail, tbt_BillingDetail>(tbt_BillingDetails);
                foreach (var item in newBillingDetails)
                {
                    item.BillingDetailNo = 0;
                    //item.InvoiceOCC = null;
                    //item.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;  //01-Apr-2013 Edit by Patcharee T. Use old issue invoice date
                    item.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                }
                return billingHandler.ManageInvoiceByCommand(doTbt_Invoice, newBillingDetails, isTaxInvoiceIssued, false, false);
                #endregion
            }
            else if (doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_AUTO_PAID)
            {
                #region PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                //doTbt_Invoice.InvoiceOCC = 0; //01-Apr-2013 Edit by Patcharee T. Use old tax invoice and reciept
                doTbt_Invoice.IssueInvFlag = false;
                //doTbt_Invoice.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;  //01-Apr-2013 Edit by Patcharee T. Use old issue invoice date

                if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    doTbt_Invoice.PaidAmountIncVat = 0;
                else if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doTbt_Invoice.PaidAmountIncVatUsd = 0;

                if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    doTbt_Invoice.RegisteredWHTAmount = 0;
                else if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doTbt_Invoice.RegisteredWHTAmountUsd = 0;

                doTbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT;
                doTbt_Invoice.FirstIssueInvFlag = false;
                doTbt_Invoice.FirstIssueInvDate = null;

                //Dummy concept to support ef,
                List<tbt_BillingDetail> newBillingDetails = CommonUtil.ClonsObjectList<tbt_BillingDetail, tbt_BillingDetail>(tbt_BillingDetails);
                foreach (var item in newBillingDetails)
                {
                    item.BillingDetailNo = 0;
                    //item.InvoiceOCC = null;
                    //item.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;  //01-Apr-2013 Edit by Patcharee T. Use old issue invoice date
                    item.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT;
                }
                return billingHandler.ManageInvoiceByCommand(doTbt_Invoice, newBillingDetails, isTaxInvoiceIssued, false, false);
                #endregion
            }
            else if (doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED
                    || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED) //Add by Jutarat A. on 25122013
            {
                if (correctionReason == CorrectionReason.C_CORRECTION_REASON_MISTAKE
                    || correctionReason == CorrectionReason.C_CORRECTION_REASON_ENCASH_MISTAKE) //Add by Jutarat A. on 25122013
                {
                    #region PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                    //doTbt_Invoice.InvoiceOCC = 0; //01-Apr-2013 Edit by Patcharee T. Use old tax invoice and reciept
                    doTbt_Invoice.IssueInvFlag = false;
                    //doTbt_Invoice.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime; //01-Apr-2013 Edit by Patcharee T. Use old issue invoice date

                    // Edit by Jirawat Jannet on 201610-27
                    if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        doTbt_Invoice.PaidAmountIncVat = 0;
                    else if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US)
                        doTbt_Invoice.PaidAmountIncVatUsd = 0;

                    if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        doTbt_Invoice.RegisteredWHTAmount = 0;
                    else if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                        doTbt_Invoice.RegisteredWHTAmountUsd = 0;
                    // End edit

                    doTbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                    doTbt_Invoice.FirstIssueInvFlag = false;
                    doTbt_Invoice.FirstIssueInvDate = null;

                    //Dummy concept to support ef,
                    List<tbt_BillingDetail> newBillingDetails = CommonUtil.ClonsObjectList<tbt_BillingDetail, tbt_BillingDetail>(tbt_BillingDetails);
                    foreach (var item in newBillingDetails)
                    {
                        item.BillingDetailNo = 0;
                        //item.InvoiceOCC = null;
                        //item.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;  //Edit by Patcharee T. Use old issue invoice date
                        item.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                    }
                    return billingHandler.ManageInvoiceByCommand(doTbt_Invoice, newBillingDetails, isTaxInvoiceIssued, false, false);
                    #endregion
                }
                else if (correctionReason == CorrectionReason.C_CORRECTION_REASON_DISHONOR)
                {
                    #region PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                    doTbt_Invoice.RefOldInvoiceNo = doTbt_Invoice.InvoiceNo;
                    doTbt_Invoice.InvoiceNo = null;
                    doTbt_Invoice.InvoiceOCC = 0;
                    doTbt_Invoice.IssueInvFlag = true;
                    doTbt_Invoice.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                    // Edit by Jirawat Jannet on 2016-10-27
                    if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        doTbt_Invoice.PaidAmountIncVat = 0;
                    else if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US)
                        doTbt_Invoice.PaidAmountIncVatUsd = 0;

                    if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        doTbt_Invoice.RegisteredWHTAmount = 0;
                    else if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                        doTbt_Invoice.RegisteredWHTAmountUsd = 0;
                    // Edit by Jirawat Jannet

                    doTbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK;
                    //Show PDF on screen immediately
                    doTbt_Invoice.FirstIssueInvFlag = true;
                    doTbt_Invoice.FirstIssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                    //Dummy concept to support ef,
                    List<tbt_BillingDetail> newBillingDetails = CommonUtil.ClonsObjectList<tbt_BillingDetail, tbt_BillingDetail>(tbt_BillingDetails);
                    foreach (var item in newBillingDetails)
                    {
                        item.InvoiceNo = null;
                        item.InvoiceOCC = null;
                        item.BillingDetailNo = 0;
                        item.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        item.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK;
                    }
                    return billingHandler.ManageInvoiceByCommand(doTbt_Invoice, newBillingDetails, isTaxInvoiceIssued);
                    #endregion
                }
            }
            else if (doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED
                    || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED) //Add by Jutarat A. on 25122013
            {
                if (correctionReason == CorrectionReason.C_CORRECTION_REASON_MISTAKE
                    || correctionReason == CorrectionReason.C_CORRECTION_REASON_ENCASH_MISTAKE) //Add by Jutarat A. on 25122013
                {
                    #region PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                    //doTbt_Invoice.InvoiceOCC = 0; //01-Apr-2013 Edit by Patcharee T. Use old tax invoice and reciept
                    doTbt_Invoice.IssueInvFlag = false;
                    //doTbt_Invoice.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;  //01-Apr-2013 Edit by Patcharee T. Use old issue invoice date

                    // Edit by Jirawat Jannet on 2016-10-27
                    if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        doTbt_Invoice.PaidAmountIncVat = 0;
                    else if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US)
                        doTbt_Invoice.PaidAmountIncVatUsd = 0;

                    if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        doTbt_Invoice.RegisteredWHTAmount = 0;
                    else if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                        doTbt_Invoice.RegisteredWHTAmountUsd = 0;
                    // End edit

                    doTbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                    doTbt_Invoice.FirstIssueInvFlag = false;
                    doTbt_Invoice.FirstIssueInvDate = null;

                    //Dummy concept to support ef,
                    List<tbt_BillingDetail> newBillingDetails = CommonUtil.ClonsObjectList<tbt_BillingDetail, tbt_BillingDetail>(tbt_BillingDetails);
                    foreach (var item in newBillingDetails)
                    {
                        item.BillingDetailNo = 0;
                        //item.InvoiceOCC = null;
                        //item.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;  //01-Apr-2013 Edit by Patcharee T. Use old issue invoice date
                        item.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                    }
                    return billingHandler.ManageInvoiceByCommand(doTbt_Invoice, newBillingDetails, isTaxInvoiceIssued, false, false);
                    #endregion
                }
                else if (correctionReason == CorrectionReason.C_CORRECTION_REASON_DISHONOR)
                {
                    #region PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                    doTbt_Invoice.RefOldInvoiceNo = doTbt_Invoice.InvoiceNo;
                    doTbt_Invoice.InvoiceNo = null;
                    doTbt_Invoice.InvoiceOCC = 0;
                    doTbt_Invoice.IssueInvFlag = true;
                    doTbt_Invoice.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                    // Edit by Jirawat Jannet on 2016-10-27
                    if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        doTbt_Invoice.PaidAmountIncVat = 0;
                    else if (doTbt_Invoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US)
                        doTbt_Invoice.PaidAmountIncVatUsd = 0;

                    if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        doTbt_Invoice.RegisteredWHTAmount = 0;
                    else if (doTbt_Invoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                        doTbt_Invoice.RegisteredWHTAmountUsd = 0;
                    // End edit


                    doTbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK;
                    //Show PDF on screen immediately
                    doTbt_Invoice.FirstIssueInvFlag = true;
                    doTbt_Invoice.FirstIssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                    //Dummy concept to support ef,
                    List<tbt_BillingDetail> newBillingDetails = CommonUtil.ClonsObjectList<tbt_BillingDetail, tbt_BillingDetail>(tbt_BillingDetails);
                    foreach (var item in newBillingDetails)
                    {
                        item.InvoiceNo = null;
                        item.InvoiceOCC = null;
                        item.BillingDetailNo = 0;
                        item.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        item.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK;
                    }
                    return billingHandler.ManageInvoiceByCommand(doTbt_Invoice, newBillingDetails, isTaxInvoiceIssued);
                    #endregion
                }
            }

            return null;
        }
        /// <summary>
        /// Retrieve next payment status code of specific current payment status and correction reason
        /// </summary>
        /// <param name="paymentStatus">current payment status</param>
        /// <param name="correctionReason">correction reason</param>
        /// <returns></returns>
        public string ICS090_GetCancelPaymentNextStatus(string paymentStatus, string correctionReason)
        {
            if (paymentStatus == PaymentStatus.C_PAYMENT_STATUS_BANK_PAID
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_AUTO_PAID
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_CASH_PAID
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_CHEQUE_PAID
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_CASHIER_PAID
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_AUTO_FAIL_BANK_PAID
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL_BANK_PAID
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_FAIL_BANK_PAID
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND
                  || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID)
            {
                if (correctionReason == CorrectionReason.C_CORRECTION_REASON_MISTAKE
                    || correctionReason == CorrectionReason.C_CORRECTION_REASON_DISHONOR)
                {
                    return PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED;
                }
            }
            else if (paymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED
                    || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED) //Add by Jutarat A. on 25122013
            {
                if (correctionReason == CorrectionReason.C_CORRECTION_REASON_MISTAKE
                    || correctionReason == CorrectionReason.C_CORRECTION_REASON_ENCASH_MISTAKE) //Add by Jutarat A. on 25122013
                {
                    return PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED;
                }
                else if (correctionReason == CorrectionReason.C_CORRECTION_REASON_DISHONOR)
                {
                    return PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL;
                }
            }
            else if (paymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED
                    || paymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED) //Add by Jutarat A. on 25122013
            {
                if (correctionReason == CorrectionReason.C_CORRECTION_REASON_MISTAKE
                    || correctionReason == CorrectionReason.C_CORRECTION_REASON_ENCASH_MISTAKE) //Add by Jutarat A. on 25122013
                {
                    return PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED;
                }
                else if (correctionReason == CorrectionReason.C_CORRECTION_REASON_DISHONOR)
                {
                    return PaymentStatus.C_PAYMENT_STATUS_POST_FAIL;
                }
            }
            //Comment by Jutarat A. on 25122013
            /*else if (paymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED)
            {
                return PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED;
            }
            else if (paymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED)
            {
                return PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED;
            }*/
            //End Comment

            return null;
        }
        /// <summary>
        /// Retrieve payment status description of specific payment status code
        /// </summary>
        /// <param name="invoicePaymentStatus">payment status code</param>
        /// <returns></returns>
        private string ICS090_GetPaymentStatusDesctription(string invoicePaymentStatus)
        {
            var incomeHander = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            doMiscTypeCode doPaymentStatus = incomeHander.GetpaymentStatusDesc(invoicePaymentStatus);
            return doPaymentStatus == null ? string.Empty : doPaymentStatus.ValueCodeDisplay;
        }
        
        /// <summary>
        /// Validate business for specific invoice no. and cancelling information
        /// </summary>
        /// <param name="param">invoice no., cancelling information</param>
        /// <param name="res"></param>
        /// <param name="isConfirmCancel">validate when click confirm cancel button</param>
        /// <returns></returns>
        private ICS090_InvoiceInfo ICS090_ValidateInvoiceBusiness(ICS090_ScreenParameter param, ObjectResultData res, bool isConfirmCancel)
        {
            ValidatorUtil validator = new ValidatorUtil();

            if (CommonUtil.IsNullOrEmpty(param.InvoiceNo))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS090"
               , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
               , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                return null;
            }

            if (isConfirmCancel == true)
            {
                if (CommonUtil.IsNullOrEmpty(param.CorrectionReason))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS090"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                   , "CorrectionReason", "lblCorrectionReason", "CorrectionReason");
                }
                if (CommonUtil.IsNullOrEmpty(param.ApproveNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS090"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                   , "ApproveNo", "lblApproveNo", "ApproveNo");
                }
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return null;
            }



            //Get invoice Data
            var billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            doInvoice doInvoice = billingHandler.GetInvoice(param.InvoiceNo);
            if (doInvoice == null)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS050"
                        , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                        , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                return null;
            }

            //Validate business
            if ((doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_BANK_PAID
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_AUTO_PAID
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_CASH_PAID
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_CHEQUE_PAID
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_CASHIER_PAID
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_AUTO_FAIL_BANK_PAID
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL_BANK_PAID
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_FAIL_BANK_PAID
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID) == false)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS090"
                    , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7095
                    , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                return null;
            }

            //Edit by Patcharee T. 22/02/2013 
            //CRC 6.12 The income type are cheque or other may be matching have mistake and issue receipt before matching invoice 
            //therefore not need to cancel receipt if select correction reason is matching mistake.

            //Other case than encash
            //if (doInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED
            //    && doInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED)
            //{
            //    bool isIssued = billingHandler.CheckExistReceiptForInvoice(doInvoice.InvoiceNo, doInvoice.InvoiceOCC);
            //    if (isIssued == true)
            //    {
            //        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS090"
            //            , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7041
            //            , "");
            //        ValidatorUtil.BuildErrorMessage(res, validator, null);
            //        return null;
            //    }
            //}

            //Edit by Patcharee T. 22/02/2013 
            //CRC 6.12 The income type are cheque or other may be matching have mistake and issue receipt before matching invoice 
            //therefore not need to cancel receipt if select correction reason is matching mistake.

            ICS090_InvoiceInfo invoiceInfo = new ICS090_InvoiceInfo();
            invoiceInfo.doInvoice = doInvoice;

            //Update Current payment status
            invoiceInfo.PaymentStatus = ICS090_GetPaymentStatusDesctription(invoiceInfo.doInvoice.InvoicePaymentStatus);

            //Update Next payment status, Require correctionReason, No need assign
            invoiceInfo.NextPaymentStatus = string.Empty;


            //Success
            return invoiceInfo;
        }
        #endregion
    }
}