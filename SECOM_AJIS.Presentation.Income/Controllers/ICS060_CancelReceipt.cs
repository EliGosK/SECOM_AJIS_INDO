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
        public ActionResult ICS060_Authority(ICS060_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                // Check permission
                if (!ICS060_IsAllowOperate(res))
                    return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<ICS060_ScreenParameter>("ICS060", param, res);
        }
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool ICS060_IsAllowOperate(ObjectResultData res)
        {
            //Check Permission 
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_CANCEL_RECEIPT, FunctionID.C_FUNC_ID_OPERATE) == false)
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
        [Initialize("ICS060")]
        public ActionResult ICS060()
        {
            ICS060_ScreenParameter param = GetScreenObject<ICS060_ScreenParameter>();
            return View();
        }
        #endregion

        #region Actions
        /// <summary>
        /// Retrieve receipt information of specific receipt no.
        /// </summary>
        /// <param name="param">receipt no.</param>
        /// <returns></returns>
        public ActionResult ICS060_GetReceipt(ICS060_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            doReceipt doReceipt;
            try
            {
                //Validate receipt business
                doReceipt = ICS060_ValidateReceiptBusiness(param, res);
                if (res.IsError || doReceipt == null)
                    return Json(res);

                //Pass, Set doReceipt
                ICS060_ReceiptInformation result = new ICS060_ReceiptInformation();
                result.doReceipt = doReceipt;

                //Set cancel method
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler; // Add By Sommai P., Oct 31, 2013

                bool isIssuedTaxInvoice = billingHandler.CheckInvoiceIssuedTaxInvoice(doReceipt.InvoiceNo, doReceipt.InvoiceOCC);
                bool isCancelTaxInvoiceOption = incomeHandler.CheckCancelTaxInvoiceOption(doReceipt.InvoiceNo, doReceipt.InvoiceOCC); // Add By Sommai P., Oct 31, 2013
                string filterValueCode = "%";
                if (isIssuedTaxInvoice == false || isCancelTaxInvoiceOption == false)  // Modify By Sommai P., Oct 31, 2013
                    filterValueCode = CancelReceiptTarget.C_CANCEL_RECEIPT_ONLY;
                
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_CANCEL_RECEIPT_TARGET,
                        ValueCode = filterValueCode
                    }
                };
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> lst = hand.GetMiscTypeCodeList(miscs);
                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(lst, "ValueDisplay", "ValueCode");
                result.CancelMethodComboBoxModel = cboModel;
                res.ResultData = result;
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
        /// Event when click confirm cancel button, this function will cancel the issued receipt information of specific receipt no. to the system.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS060_cmdConfirmCancel(ICS060_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            doReceipt doReceipt;
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS060_IsAllowOperate(res))
                    return Json(res);


                //Validate receipt business
                doReceipt = ICS060_ValidateReceiptBusiness(param, res);
                if (res.IsError || doReceipt == null)
                    return Json(res);


                //Connect db
                using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                        //Cancel receipt
                        IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                        
                        bool isSuccess = incomeHandler.CancelReceipt(doReceipt);
                        if (!isSuccess)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7031, null);

                        IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                        //Get Invoice
                        doInvoice doInvoice = billingHandler.GetInvoice(doReceipt.InvoiceNo);
                        if (doInvoice == null)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7031, null);
                        
                        //string invoiceType = billingHandler.GetInvoiceType(doInvoice.BillingTypeCode);
                        //if ((invoiceType == InvoiceType.C_INVOICE_TYPE_DEPOSIT || invoiceType == InvoiceType.C_INVOICE_TYPE_SERVICE)
                        //    && (doInvoice.IssueReceiptTiming != IssueRecieptTime.C_ISSUE_REC_TIME_SAME_INV
                        //        || (doInvoice.IssueReceiptTiming == IssueRecieptTime.C_ISSUE_REC_TIME_SAME_INV
                        //             && doInvoice.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_MESSENGER_TRANSFER)))
                        bool isIssuedTaxInvoice = billingHandler.CheckInvoiceIssuedTaxInvoice(doReceipt.InvoiceNo, doReceipt.InvoiceOCC);
                        if (isIssuedTaxInvoice == true)
                        {
                            if (param.CancelMethod == CancelReceiptTarget.C_CANCEL_TAX_INVOICE_RECEIPT)
                            {
                                //Cancel tax invoice (Update cancel flag only)
                                isSuccess = billingHandler.CancelTaxInvoice(doReceipt.TaxInvoiceNo);
                                if (!isSuccess)
                                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7032, null);
                            }
                            else
                            {
                                //Clear receipt no.
                                isSuccess = billingHandler.UpdateReceiptNo(doReceipt.InvoiceNo, doReceipt.InvoiceOCC, null);
                                if (!isSuccess)
                                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7032, null);
                            }
                        }


                        //Clear receipt no, invoice no of deposit fee
                        if (doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT
                            && (doReceipt.AdvanceReceiptStatus == AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_NOT
                                || doReceipt.AdvanceReceiptStatus == AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_PAID))
                        {
                            isSuccess = billingHandler.UpdateReceiptNoDepositFeeCancelReceipt(doInvoice.InvoiceNo, doReceipt.ReceiptNo
                            ,CommonUtil.dsTransData.dtUserData.EmpNo
                            ,CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                            if (!isSuccess)
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7032, null);
                        }
                        

                        //Show attention in case of paid receipt
                        if (doReceipt.AdvanceReceiptStatus == SECOM_AJIS.Common.Util.ConstantValue.AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_NOT
                            || doReceipt.AdvanceReceiptStatus == SECOM_AJIS.Common.Util.ConstantValue.AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_PAID)
                        {
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                            res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7030);
                        }

                    //Success
                    scope.Complete();
                    res.ResultData = "1";
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
        #endregion

        #region Methods
        /// <summary>
        /// Validate business for receipt information of specific receipt no.
        /// </summary>
        /// <param name="param">receipt no.</param>
        /// <param name="res"></param>
        /// <returns></returns>
        private doReceipt ICS060_ValidateReceiptBusiness(ICS060_ScreenParameter param, ObjectResultData res)
        {
            ValidatorUtil validator = new ValidatorUtil();

            //Validate model
            ValidatorUtil.BuildErrorMessage(res, this, new object[] { param });
            if (res.IsError)
                return null;


            //Get receipt Data
            var incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            doReceipt doReceipt = incomeHandler.GetReceiptIncludeCancel(param.ReceiptNo);

            // add by jirawat jannet @ 2016-10-20
            #region Set currency

            if (doReceipt.ReceiptAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) doReceipt.ReceiptAmount = doReceipt.ReceiptAmountUsd;
            if (doReceipt.VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) doReceipt.VATAmount = doReceipt.VatAmountUsd;
            if (doReceipt.WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) doReceipt.WHTAmount = doReceipt.WHTAmountUsd;

            #endregion

            if (doReceipt == null)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS060"
                    , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                    , "ReceiptNo", "lblReceiptNo", "ReceiptNo");
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                return null;
            }

            //Validate business
            if (doReceipt.CancelFlag == true)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS060"
                    , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7029
                    , "");
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                return null;
            }

            //This receipt no. has been registered 
            if (doReceipt.AdvanceReceiptStatus == AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_REGISTERED)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS060"
                    , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7109
                    , "");
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                return null;
            }

            //Success
            return doReceipt;
        }  
        #endregion
    }
}