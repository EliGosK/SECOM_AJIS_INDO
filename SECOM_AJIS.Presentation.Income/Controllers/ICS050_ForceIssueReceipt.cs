using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Income.Models;
using System.Transactions;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Income;
using System.IO;
using SECOM_AJIS.DataEntity.Common;

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
        public ActionResult ICS050_Authority(ICS050_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                // Check permission
                if (!ICS050_IsAllowOperate(res))
                    return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<ICS050_ScreenParameter>("ICS050", param, res);
        }
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool ICS050_IsAllowOperate(ObjectResultData res)
        {
            //Check Permission 
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_FORCE_ISSUE_RECEIPT, FunctionID.C_FUNC_ID_OPERATE) == false)
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
        [Initialize("ICS050")]
        public ActionResult ICS050()
        {
            ICS050_ScreenParameter param = GetScreenObject<ICS050_ScreenParameter>();
            return View();
        }
        #endregion

        #region Actions
        /// <summary>
        /// Retrieve invoice information of specific invoice no.
        /// </summary>
        /// <param name="param">invoice no.</param>
        /// <returns></returns>
        public ActionResult ICS050_GetInvoice(ICS050_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            doInvoice doInvoice;
            try
            {
                ValidatorUtil validator = new ValidatorUtil();

                // Check required field.
                if (CommonUtil.IsNullOrEmpty(param) || string.IsNullOrEmpty(param.InvoiceNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS050"
                       , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                       , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    return Json(res);
                }

                //Get Invoice Data
                var billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                doInvoice = billingHandler.GetInvoice(param.InvoiceNo);
                if (doInvoice == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS050"
                         , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                         , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Validate business
                ICS050_ValidateInvoiceBusiness(doInvoice, validator);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);

                //Pass
                res.ResultData = doInvoice;
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
        /// Event when click register button 
        /// </summary>
        /// <param name="param">invoice information</param>
        /// <returns></returns>
        public ActionResult ICS050_cmdRegister(ICS050_ScreenParameter param)
        {
            ICS050_ScreenParameter session = GetScreenObject<ICS050_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            doInvoice doInvoice;

            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS050_IsAllowOperate(res))
                    return Json(res);

                ValidatorUtil validator = new ValidatorUtil();

                //Validate Model
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { param });
                if (res.IsError)
                    return Json(res);


                //Get Invoice Data
                var billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                doInvoice = billingHandler.GetInvoice(param.InvoiceNo);
                if (doInvoice == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS050"
                        , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                        , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Validate business
                ICS050_ValidateInvoiceBusiness(doInvoice, validator);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);



                //Result flag  1 = Success
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
        /// Event when click confirm button, this function will force issued taxinvoice, receipt information of specific invoice information to the system.
        /// </summary>
        /// <param name="param">invoice information</param>
        /// <returns></returns>
        public ActionResult ICS050_cmdConfirm(ICS050_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            doInvoice doInvoice;

            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS050_IsAllowOperate(res))
                    return Json(res);

                ValidatorUtil validator = new ValidatorUtil();


                //Validate Model
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { param });
                if (res.IsError)
                    return Json(res);


                //Get Invoice Data
                var billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                doInvoice = billingHandler.GetInvoice(param.InvoiceNo);
                if (doInvoice == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS050"
                        , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                        , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Validate business
                ICS050_ValidateInvoiceBusiness(doInvoice, validator);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);


                //Issued TaxInvoice , Receipt
                using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                    //bool isIssuedTaxInvoice = billingHandler.CheckTaxInvoiceIssued(doInvoice);
                    bool isIssuedTaxInvoice = billingHandler.CheckInvoiceIssuedTaxInvoice(doInvoice.InvoiceNo, doInvoice.InvoiceOCC);
                    string pdfFilePath;

                    if (isIssuedTaxInvoice == false)
                    {
                        //Issue tax invoice, receipt
                        pdfFilePath = ICS050_IssueTaxInvoiceReceipt(doInvoice, param.IssueInvoiceDate.Value, validator);
                    }
                    else
                    {
                        //Issue only receipt
                        pdfFilePath = ICS050_IssueReceipt(doInvoice, param.IssueInvoiceDate.Value, validator);
                    }

                    if (pdfFilePath != null)
                    {
                        //Success 
                        scope.Complete();

                        // Commeny by Jirawat Jannet @2016-10-17
                        ICS050_ScreenParameter screenSession = GetScreenObject<ICS050_ScreenParameter>();
                        screenSession.PDFFilePath = pdfFilePath;
                        res.ResultData = "1";
                        return Json(res);
                        //res.ResultData = "1"; // add by Jirawat Jannet @ 2016-10-17
                        //return Json(res);
                    }

                    //All incompleted cases
                    scope.Dispose();
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
            }
            return Json(res);
        }
        
        /// <summary>
        /// Generate taxinvoice/receipt pdf report 
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS050_DisplayReport()
        {
            ICS050_ScreenParameter screenSession = GetScreenObject<ICS050_ScreenParameter>();
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
        /// Force issue taxinvoice/receipt pdf report of specific invoice information
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="issueInvoiceDate">issue invoice date</param>
        /// <param name="validator"></param>
        /// <returns></returns>
        public string ICS050_IssueTaxInvoiceReceipt(doInvoice doInvoice, DateTime issueInvoiceDate, ValidatorUtil validator)
        {
            ///Issue tax invoice
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            tbt_TaxInvoice doTaxInvoice = billingHandler.ForceIssueTaxInvoice(doInvoice, issueInvoiceDate);

            if (doTaxInvoice != null)
            {
                //Issue receipt
                IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                tbt_Receipt doReceipt = incomeHandler.ForceIssueReceipt(doInvoice, issueInvoiceDate);

                if (doReceipt != null)
                {
                    //Generate pdf on server
                    IIncomeDocumentHandler incomeDocumentHandler = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;
                    //string pdfFilePath = string.Empty;
                    // comment by Jirawta Jannet @ 2016-10-17
                    string pdfFilePath = incomeDocumentHandler.GenerateBLR020_ICR010FilePath(
                        doTaxInvoice.TaxInvoiceNo
                        , doReceipt.ReceiptNo
                        , CommonUtil.dsTransData.dtUserData.EmpNo
                        , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);


                    if (doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT
                        && doReceipt.AdvanceReceiptStatus == AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_NOT)
                    {
                        bool isSuccess = billingHandler.UpdateReceiptNoDepositFee(doInvoice.InvoiceNo, doReceipt.ReceiptNo
                            , CommonUtil.dsTransData.dtUserData.EmpNo
                            , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        if (!isSuccess)
                        {
                            //Cannot issue receipt
                            validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS050"
                                , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7010
                                , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                            return null;
                        }
                    }

                    //Success
                    return pdfFilePath;
                }
                else
                {
                    //Cannot issue receipt
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS050"
                        , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7010
                        , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                    return null;
                }
            }
            else
            {
                //Cannot issue tax invoice
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS050"
                        , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7011
                        , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                return null;
            }
        }
        /// <summary>
        /// Force issue receipt pdf report of specific invoice information
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="issueInvoiceDate">issue invoice date</param>
        /// <param name="validator"></param>
        /// <returns></returns>
        public string ICS050_IssueReceipt(doInvoice doInvoice, DateTime issueInvoiceDate, ValidatorUtil validator)
        {
            //Issue receipt
            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            tbt_Receipt doReceipt = incomeHandler.ForceIssueReceipt(doInvoice, issueInvoiceDate);
            if (doReceipt != null)
            {
                IIncomeDocumentHandler incomeDocumentHandler = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;
                //ICR010 Receipt report
                //string pdfFilePath = string.Empty;
                // Comment by Jirawat Jannet @ 2016-10-17
                string pdfFilePath = incomeDocumentHandler.GenerateICR010FilePath(doReceipt.ReceiptNo
                    , CommonUtil.dsTransData.dtUserData.EmpNo
                    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                if (doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT
                    && doReceipt.AdvanceReceiptStatus == AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_NOT)
                {
                    IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                    bool isSuccess = billingHandler.UpdateReceiptNoDepositFee(doInvoice.InvoiceNo, doReceipt.ReceiptNo
                        , CommonUtil.dsTransData.dtUserData.EmpNo
                        , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                    if (!isSuccess)
                    {
                        //Cannot issue receipt
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS050"
                            , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7010
                            , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                        return null;
                    }
                }

                //Success
                return pdfFilePath;
            }
            else
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS050"
                        , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7010
                        , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                return null;
            }
        }

        /// <summary>
        /// Validate business of specific invoice information
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="validator"></param>
        private void ICS050_ValidateInvoiceBusiness(doInvoice doInvoice, ValidatorUtil validator)
        {
            if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_CANCEL
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_FAIL)
            {
                //Payment status not in (08,18,28,38)
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7089
                    , "InvoiceNo", doInvoice.InvoiceNo);
            }
            else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_BILLING_EXEMPTION)
            {
                //Payment status not in (19)
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7090
                    , "InvoiceNo", doInvoice.InvoiceNo);
            }
            else
            {
                IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                bool isIssued = incomeHandler.CheckInvoiceIssuedReceipt(doInvoice.InvoiceNo, doInvoice.InvoiceOCC);
                if (isIssued)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7009
                        , "InvoiceNo", doInvoice.InvoiceNo);
                }
            }
        }  
        #endregion
    }
}