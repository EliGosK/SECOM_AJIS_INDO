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
        public ActionResult ICS080_Authority(ICS080_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // No need check suspend for view mode
                if (param.ScreenMode != ICS080_SCREEN_MODE.View)
                {
                    if (IsSuspend(res))
                        return Json(res);
                }

                if (!ICS080_IsAllowOperate(param.ScreenMode, res))
                    return Json(res);

                ICS080_ScreenParameter screenData = GetScreenObject<ICS080_ScreenParameter>();
                screenData = param;

                if (param.ScreenMode == ICS080_SCREEN_MODE.Match || param.ScreenMode == ICS080_SCREEN_MODE.Delete)
                {
                    if (IsSuspend(res))
                        return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<ICS080_ScreenParameter>("ICS080", param, res);
        }
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool ICS080_IsAllowOperate(ICS080_SCREEN_MODE screenMode, ObjectResultData res)
        {
            //Check Permission 
            bool isHavePermission = false;
            if (screenMode == ICS080_SCREEN_MODE.Match)
                isHavePermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_PAYMENT_INFO, FunctionID.C_FUNC_ID_OPERATE);
            else if (screenMode == ICS080_SCREEN_MODE.View)
                isHavePermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_PAYMENT_INFO, FunctionID.C_FUNC_ID_VIEW);
            else if (screenMode == ICS080_SCREEN_MODE.Delete)
                isHavePermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_PAYMENT_INFO, FunctionID.C_FUNC_ID_DEL);
            if (!isHavePermission)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return false;
            }

            //Pass
            return true;
        }
        #endregion

        #region View
        /// <summary>
        /// Genereate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS080")]
        public ActionResult ICS080()
        {
            ICS080_ScreenParameter session = GetScreenObject<ICS080_ScreenParameter>();
            ViewBag.ScreenMode = session.ScreenMode;

            //ics080
            var ics080 = CommonUtil.dsTransData.dtMenuNameList.Where(d => d.ObjectID == "ICS080").FirstOrDefault();
            CommonUtil.MappingObjectLanguage(ics080);
            ViewBag.ICS080ScreenName = ics080.ObjectName;
            //ics081
            var ics081 = CommonUtil.dsTransData.dtMenuNameList.Where(d => d.ObjectID == "ICS081").FirstOrDefault();
            CommonUtil.MappingObjectLanguage(ics081);
            ViewBag.ICS081ScreenName = ics081.ObjectName;
            //ics084
            var ics084 = CommonUtil.dsTransData.dtMenuNameList.Where(d => d.ObjectID == "ICS084").FirstOrDefault();
            CommonUtil.MappingObjectLanguage(ics084);
            ViewBag.ICS084ScreenName = ics084.ObjectName;

            return View();
        }
        #endregion

        #region Actions
        /// <summary>
        /// Generate xml for initial search result grid 
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS080_InitialSearchGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS080_Search", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Genereate xml for initial match result grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS080_InitialMatchGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS080_Match", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Generate SECOM bank/branch comboitem list upon selected payment type
        /// </summary>
        /// <param name="paymentType">payment type</param>
        /// <returns></returns>
        public ActionResult ICS080_GetSECOMAccount(string paymentType)
        {
            try
            {
                //Display secom bank same as ics010
                List<doSECOMAccount> doSECOMAccount = ICS010_GetSECOMAccountByPaymentType(paymentType);

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doSECOMAccount>(doSECOMAccount, "Text", "SecomAccountID", true, CommonUtil.eFirstElementType.All);
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
        /// Retrieve payment information list of specific screen mode and search criteria information
        /// </summary>
        /// <param name="param">screen mode, search criteria information</param>
        /// <returns></returns>
        public ActionResult ICS080_SearchPaymentData(ICS080_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                ICS080_ScreenParameter scrParam = GetScreenObject<ICS080_ScreenParameter>(); //Add by Jutarat A. on 06082013

                //Validate search model
                if (param.ScreenMode == ICS080_SCREEN_MODE.View || param.ScreenMode == ICS080_SCREEN_MODE.Delete)
                {
                    //Validate only screen mode: view, delete, At least 1 condition
                    ValidatorUtil.BuildErrorMessage(res, this, new object[] { param.PaymentSearchCriteria });
                    if (res.IsError)
                        return Json(res);
                }
                if (res.IsError)
                    return Json(res);

                //Validate search business
                ValidatorUtil validator = new ValidatorUtil();
                ICS080_ValidateSearchPaymentCondition(validator, param.PaymentSearchCriteria);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);

                //Add by Jutarat A. on 06082013
                if (param.ScreenMode == ICS080_SCREEN_MODE.Delete)
                {
                    if (param.PaymentSearchCriteria != null && param.PaymentSearchCriteria.PaymentType == null)
                    {
                        if (String.IsNullOrEmpty(scrParam.AllPaymentTypeExceptCreditNoteDecreased) == true)
                        {
                            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>() {
                                new doMiscTypeCode() {
                                    FieldName = MiscType.C_PAYMENT_TYPE,
                                    ValueCode = "%"
                                }
                            };

                            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                            List<doMiscTypeCode> lst = hand.GetMiscTypeCodeList(miscs).Where(d => d.ValueCode != PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED).ToList();

                            if (lst != null && lst.Count > 0)
                            {
                                List<string> paymentTypeList = new List<string>();
                                foreach (doMiscTypeCode data in lst)
                                {
                                    paymentTypeList.Add(data.ValueCode);
                                }

                                scrParam.AllPaymentTypeExceptCreditNoteDecreased = CommonUtil.CreateCSVString(paymentTypeList);
                            }
                        }

                        param.PaymentSearchCriteria.PaymentType = scrParam.AllPaymentTypeExceptCreditNoteDecreased;
                    }
                }
                //End Add

                param.PaymentSearchCriteria.EmpNo = ((param.PaymentSearchCriteria.MyPayment ?? false) ? CommonUtil.dsTransData.dtUserData.EmpNo : null);

                //Get Data
                var handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                List<doPayment> paymentList = handler.SearchPayment((doPaymentSearchCriteria)param.PaymentSearchCriteria);

                //Business filter
                if (param.ScreenMode == ICS080_SCREEN_MODE.Match || param.ScreenMode == ICS080_SCREEN_MODE.Delete)
                {
                    //Filter: Not show Deleted data
                    if (paymentList != null && paymentList.Count > 0)
                        paymentList = paymentList.Where(d => d.DeleteFlag == false).ToList();
                }

                //Validate Result
                //ICS080_ValidateSearchPaymentResult(res, paymentList);
                //if (res.IsError)
                //    return Json(res);

                //Pass, Return result
                res.ResultData = CommonUtil.ConvertToXml<doPayment>(paymentList, "Income\\ICS080_Search", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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

        //Match Mode
        /// <summary>
        /// Retrieve next screen of specific payment transaction no. in match mode
        /// </summary>
        /// <param name="paymentTranNo">payment transaction no.</param>
        /// <returns></returns>
        public ActionResult ICS080_MatchPaymentNextStep(string paymentTranNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                if (!ICS080_IsAllowOperate(ICS080_SCREEN_MODE.Match, res))
                    return Json(res);

                //Confirm with P.Karnt 28 Mar 2012
                //IF RefInvoice มีค่า AND MatchableAmount = PaymentAmount --> 84 
                //ELSE -->81

                IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                tbt_Payment payment = handler.GetPayment(paymentTranNo);
                if (payment != null)
                {
                    if (!string.IsNullOrEmpty(payment.RefInvoiceNo)
                        && ((payment.MatchableBalance == payment.PaymentAmount 
                                        && payment.PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                             || (payment.MatchableBalanceUsd == payment.PaymentAmountUsd 
                                        && payment.PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)))
                    {
                        res.ResultData = "ICS084";
                    }
                    else
                    {
                        res.ResultData = "ICS081";
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
        /// Retrieve matchable balance amount of specific payment transaction no. in match mode
        /// </summary>
        /// <param name="paymentTranNo">payment transaction no.</param>
        /// <returns></returns>
        public ActionResult ICS080_GetMatchableBalance(string paymentTranNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                tbt_Payment payment = handler.GetPayment(paymentTranNo);
                if (payment != null)
                {
                    res.ResultData = payment.MatchableBalance;
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        
        
        //View Mode
        /// <summary>
        /// Retrieve payment information of payment transaction no. in view mode
        /// </summary>
        /// <param name="paymentTranNo">payment transaction no.</param>
        /// <returns></returns>
        public ActionResult ICS080_DisplayPaymentMatchingResult(string paymentTranNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                if (!ICS080_IsAllowOperate(ICS080_SCREEN_MODE.View, res))
                    return Json(res);

                //Send back to client
                ICS080_PaymentMatchingResult result = ICS080_GetPaymentMatchingResult(paymentTranNo);
                res.ResultData = result;
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Retrieve payment matching result information of specific payment trasaction no. in view mode
        /// </summary>
        /// <param name="paymentTranNo">payment transaction no.</param>
        /// <returns></returns>
        public ActionResult ICS080_SearchPaymentMatching(string paymentTranNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (!ICS080_IsAllowOperate(ICS080_SCREEN_MODE.View, res))
                    return Json(res);

                //Get doPayment
                IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                tbt_Payment doPayment = incomeHandler.GetPayment(paymentTranNo);

                //Get PaymentMatchingResult
                List<doPaymentMatchingResult> doPaymentMatchingResult = new List<doPaymentMatchingResult>();
                List<doPaymentMatchingResultDetail> doDecoratedMatchingResult = new List<doPaymentMatchingResultDetail>();
                doPaymentMatchingResult = incomeHandler.GetPaymentMatchingResult(paymentTranNo);
                doDecoratedMatchingResult = ICS080_DecorateDisplayPaymentMatchingResult(doPayment, doPaymentMatchingResult);
                res.ResultData = CommonUtil.ConvertToXml<doPaymentMatchingResultDetail>(doDecoratedMatchingResult, "Income\\ICS080_Match", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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

        public ActionResult ICS080_Encash(doPaymentEncashParam param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                //Check encash permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_PAYMENT_INFO, FunctionID.C_FUNC_ID_ENCASH) == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                ICS080_PaymentMatchingResult paymentInfo = ICS080_GetPaymentMatchingResult(param.PaymentTransNo);
                if (paymentInfo.IsEncashable)
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        try
                        {
                            

                            //Add By Sommai P., Oct 29, 2013
                            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                            bool bUpdateSuccess = incomeHandler.UpdatePaymentEncashFlag(param);
                            if (bUpdateSuccess)
                            {

                            // End Add

                                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                                List<tbt_Invoice> doTbt_InvoiceList = new List<tbt_Invoice>(); //Add by Jutarat A. on 26032014
                                
                                foreach (tbt_Invoice item in paymentInfo.doTbt_Invoice)
                                {
                                    if (item.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                                        && item.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN)
                                    {
                                        byte encashedflag = incomeHandler.CheckAllPaymentEncashed(item.InvoiceNo, item.InvoiceOCC);
                                        string expectedInvoicePaymentStatus = null;
                                        if (encashedflag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_DEFAULT)
                                        {
                                            if (paymentInfo.doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE)
                                                expectedInvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED;
                                            else if (paymentInfo.doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
                                                expectedInvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED;
                                        }
                                        else if (encashedflag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_ENCASHED)
                                        {
                                            if (paymentInfo.doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE)
                                                expectedInvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED;
                                            else if (paymentInfo.doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
                                                expectedInvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED;
                                        }
                                        else if (encashedflag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_RETURNED)
                                        {
                                            if (paymentInfo.doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE)
                                                expectedInvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED_RETURN;
                                            else if (paymentInfo.doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
                                                expectedInvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED_RETURN;
                                        }

                                        if (expectedInvoicePaymentStatus != null && item.InvoicePaymentStatus != expectedInvoicePaymentStatus)
                                        {
                                            item.InvoicePaymentStatus = expectedInvoicePaymentStatus;
                                            item.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                            item.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                            tbt_Invoice doTbt_Invoice = CommonUtil.CloneObject<tbt_Invoice, tbt_Invoice>(item);
                                            doTbt_InvoiceList.Add(doTbt_Invoice);
                                        }
                                    }
                                }

                                if (doTbt_InvoiceList != null && doTbt_InvoiceList.Count > 0) //Add by Jutarat A. on 26032014
                                {

                                    //paymentInfo.doTbt_Invoice = billingHandler.UpdateTbt_Invoice(CommonUtil.ConvertToXml_Store<tbt_Invoice>(paymentInfo.doTbt_Invoice));
                                    paymentInfo.doTbt_Invoice = billingHandler.UpdateTbt_Invoice(CommonUtil.ConvertToXml_Store<tbt_Invoice>(doTbt_InvoiceList)); //Modify by Jutarat A. on 26032014
                                    if (paymentInfo.doTbt_Invoice == null || paymentInfo.doTbt_Invoice.Count == 0)
                                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7115, null);

                                    foreach (var invoice in doTbt_InvoiceList)
                                    {
                                        var doBillingDetail = billingHandler.GetTbt_BillingDetailOfInvoice(invoice.InvoiceNo, invoice.InvoiceOCC);
                                        foreach (var detail in doBillingDetail)
                                        {
                                            detail.PaymentStatus = invoice.InvoicePaymentStatus;
                                            detail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                            detail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                            billingHandler.Updatetbt_BillingDetail(detail);
                                        }
                                    }
                                }

                                //Success
                                scope.Complete();

                                //Result flag 1 = success
                                res.ResultData = "1";
                                return Json(res);
                            // Add By Sommai P., Oct 29, 2013
                            }
                            else
                            {
                                scope.Dispose();
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG7115);
                                return Json(res);
                            }
                            // End Add
                        }
                        catch (Exception ex)
                        {
                            //all fail
                            scope.Dispose();
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            res.AddErrorMessage(ex);
                            return Json(res);
                        }
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
        /// Delete payment information of specific payment transaction no. in delete mode
        /// </summary>
        /// <param name="paymentTranNo">payment transactio no.</param>
        /// <returns></returns>
        public ActionResult ICS080_DeletePayment(string paymentTranNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS080_IsAllowOperate(ICS080_SCREEN_MODE.Delete, res))
                    return Json(res);

                IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                    tbt_Payment doPayment = incomeHandler.GetPayment(paymentTranNo);
                    bool isSuccess = false;
                    bool isDelete = (doPayment != null) && incomeHandler.DeletePaymentTransaction(doPayment);
                    if (isDelete)
                    {
                        if (!string.IsNullOrEmpty(doPayment.RefAdvanceReceiptNo))
                        {
                            isSuccess = incomeHandler.UpdateAdvanceReceiptDeletePayment(doPayment.RefAdvanceReceiptNo);
                            if (!isSuccess)
                            {
                                //Fail 
                                scope.Dispose();
                                res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7033);
                                return Json(res);
                            }
                        }

                        if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND)
                        {
                            doRefundInfo doRefund = billingHandler.GetRefundInfo(doPayment.PaymentTransNo);
                            doInvoice doInvoiceData = billingHandler.GetInvoice(doRefund.InvoiceNo);
                            if (doRefund.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                            {
                                decimal? balanceDepositAfterUpdate = null;
                                decimal? balanceDepositUsdAfterUpdate = null;
                                string balanceDepositAfterUpdateCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                                //isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC, doPayment.PaymentAmount * (-1), out balanceDepositAfterUpdate);

                                // Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                                // Modify By Sommai P., Oct 29, 2013
                                //isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC, (doPayment.PaymentAmount / (1 + doInvoiceData.VatRate ?? 0)) * (-1), out balanceDepositAfterUpdate);

                                // add by jirawat jannet @ 2016-10-25
                                decimal? adjustAmount = null;
                                decimal? adjustAmountUsd = null;
                                if (doRefund.CreditAmountIncVATCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                                {
                                    adjustAmount = -(doRefund.CreditAmountIncVAT - doRefund.CreditVATAmount.ConvertTo<decimal>(false, 0));
                                }
                                else
                                {
                                    adjustAmountUsd = -(doRefund.CreditAmountIncVATUsd - doRefund.CreditVATAmountUsd.ConvertTo<decimal>(false, 0));
                                }

                                isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC
                                , adjustAmount, adjustAmountUsd, doRefund.CreditAmountIncVATCurrencyType, out balanceDepositAfterUpdate, out balanceDepositUsdAfterUpdate, out balanceDepositAfterUpdateCurrencyType); // edit by jirawat jannet @ 2016-10-25
                                // End Modify

                                if (!isSuccess)
                                {
                                    //Fail 
                                    scope.Dispose();
                                    res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7033);
                                    return Json(res);
                                }

                                if ((balanceDepositAfterUpdate < 0 && balanceDepositAfterUpdateCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                                    && (balanceDepositUsdAfterUpdate < 0 && balanceDepositAfterUpdateCurrencyType == CurrencyUtil.C_CURRENCY_US))
                                {
                                    //Fail 
                                    scope.Dispose();
                                    res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7111);
                                    return Json(res);
                                }

                                //isSuccess = billingHandler.InsertDepositFeeReturn(doRefund, doPayment.PaymentAmount, balanceDepositAfterUpdate);

                                // Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                                // Modify By Sommai P., Oct 29, 2013
                                //isSuccess = billingHandler.InsertDepositFeeReturn(doRefund, (doPayment.PaymentAmount / (1 + doInvoiceData.VatRate ?? 0)), balanceDepositAfterUpdate);

                                // add by Jirawat Jannet
                                decimal? returnAmount = null;
                                decimal? returnAmountUsd = null;
                                if (doRefund.CreditAmountIncVATCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                                    returnAmount = doRefund.CreditAmountIncVAT - doRefund.CreditVATAmount;
                                else
                                    returnAmountUsd = doRefund.CreditAmountIncVATUsd - doRefund.CreditVATAmountUsd;
                                isSuccess = billingHandler.InsertDepositFeeReturn(doRefund, returnAmount, returnAmountUsd, doRefund.CreditAmountIncVATCurrencyType
                                                                , balanceDepositAfterUpdate, balanceDepositUsdAfterUpdate, doRefund.CreditAmountIncVATCurrencyType);
                                // End add

                                // End Modify
                                if (!isSuccess)
                                {
                                    //Fail 
                                    scope.Dispose();
                                    res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7033);
                                    return Json(res);
                                }
                            }
                        }

                        //Success
                        scope.Complete();

                        //Result flag 1 = success
                        res.ResultData = "1";
                        return Json(res);
                    }
                    else
                    {
                        //Unable Delete, 
                        scope.Dispose();
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7033);
                        return Json(res);
                    }
                }
                catch (Exception ex)
                {
                    //all fail
                    scope.Dispose();
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(ex);
                    return Json(res);
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
        /// Validate business for payment search criteria
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="criteria">search criteria </param>
        private void ICS080_ValidateSearchPaymentCondition(ValidatorUtil validator, ICS080_PaymentSearchCriteria criteria)
        {
            if (criteria.PaymentDateFrom.HasValue && criteria.PaymentDateTo.HasValue
                && criteria.PaymentDateFrom > criteria.PaymentDateTo)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS080"
                    , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7034
                    , "PaymentDateFrom", "lblPaymentDate", "PaymentDateFrom");
            }
            if (criteria.MatchableBalanceFrom.HasValue && criteria.MatchableBalanceTo.HasValue
                && criteria.MatchableBalanceFrom > criteria.MatchableBalanceTo)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS080"
                   , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7034
                   , "MatchableBalanceFrom", "lblMatchableBalance", "MatchableBalanceFrom");
            }
        }
        /// <summary>
        /// Decorate payment matching result list to display on the screen 
        /// </summary>
        /// <param name="doPayment">payment information</param>
        /// <param name="doPaymentMatchingResult">payment matching information</param>
        /// <returns></returns>
        private List<doPaymentMatchingResultDetail> ICS080_DecorateDisplayPaymentMatchingResult(tbt_Payment doPayment, List<doPaymentMatchingResult> doPaymentMatchingResult)
        {
            List<doPaymentMatchingResultDetail> decoreated = new List<doPaymentMatchingResultDetail>();
            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

            //Get partial constant
            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_FLAG_DISPLAY,
                        ValueCode = FlagDisplay.C_FLAG_DISPLAY_YES
                    }
                };
            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            doMiscTypeCode miscDisplayFlagYes = hand.GetMiscTypeCodeList(miscs).FirstOrDefault();
            
            string displayPartialFlagYes = miscDisplayFlagYes.ValueDisplay;
            string displayPartialFlagNo = "-";
            string redColor = "red";

            #region Display delete payment transaction
            if (doPayment != null)
            {
                if ((doPayment.DeleteFlag ?? false) == true)
                {
                    doPaymentMatchingDesc doMatchingDesc = incomeHandler.GetPaymentMatchingDesc(PaymentMatchingDesc.C_PAYMENT_MATCHING_DESC_DELETE);
                    doPaymentMatchingResultDetail deletedPayment = new doPaymentMatchingResultDetail()
                    {
                        GridMatchDate = doPayment.UpdateDate.Value,
                        InvoiceNo = "-",
                        ReceiptNo = "-",
                        BillingTargetCode = "-",
                        GridBillingClientNameOrCustomValue = doMatchingDesc.Description,   //Custom Value
                        MatchAmountIncWHT = doPayment.PaymentAmount.ConvertTo<decimal>(false, 0),
                        GridPartialPayment = displayPartialFlagNo,
                        MatchingDetailSection = (int)ICS080_MATCHING_DETAIL_SECTION.Payment,
                        MatchingDetailSorting = (int)ICS080_MATCHING_DETAIL_SORTING.PaymentAmount,
                        IsMatchAmountIncWHTShowDash = true
                    };
                    decoreated.Add(deletedPayment);
                }
            }
            #endregion

            #region Display payment matching result
            if (doPaymentMatchingResult != null)
            {
                //Ensure   by MatchDate desc, MatchID desc 
                //doPaymentMatchingResult = (from data in doPaymentMatchingResult
                //                           orderby data.MatchDate descending, data.MatchID descending
                //                           select data).ToList<doPaymentMatchingResult>();


                foreach (doPaymentMatchingResult header in doPaymentMatchingResult)
                {
                    if (header.PaymentMatchingResultDetail != null)
                    {
                        foreach (doPaymentMatchingResultDetail detail in header.PaymentMatchingResultDetail)
                        {
                            #region Detail
                            if ((detail.CancelFlag ?? false) == false)
                            {
                                #region CancelFlag == false
                                //Add detail
                                doPaymentMatchingResultDetail matchingDetail = new doPaymentMatchingResultDetail()
                                {
                                    GridMatchDate = header.MatchDate,
                                    InvoiceNo = detail.InvoiceNo,
                                    ReceiptNo = CommonUtil.IsNullOrEmpty(detail.ReceiptNo) ? "-" : detail.ReceiptNo,
                                    BillingTargetCode = detail.BillingTargetCode,
                                    GridBillingClientNameOrCustomValue = detail.BillingClientName,
                                    MatchAmountIncWHT = detail.MatchAmountIncWHT,
                                    GridPartialPayment = detail.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL ? displayPartialFlagYes : displayPartialFlagNo,
                                    MatchingDetailSection = (int)ICS080_MATCHING_DETAIL_SECTION.MatchingResult,
                                    MatchingDetailSorting = (int)ICS080_MATCHING_DETAIL_SORTING.MatchAmount,
                                    MatchAmountIncWHTCurrencyType = detail.MatchAmountIncWHTCurrencyType, // add by Jirawat Jannet @ 2016-10-25
                                    MatchAmountIncWHTUsd = detail.MatchAmountIncWHTUsd, // add by Jirawat Jannet @ 2016-10-25,
                                    IsMatchAmountIncWHTShowDash = false
                                };
                                decoreated.Add(matchingDetail);

                                //Add WHTAmount
                                // edit if condition by Jirawat Jannet @ 2016-10-25
                                if (detail.WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && ((detail.WHTAmount ?? 0) > 0)
                                    || detail.WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US && ((detail.WHTAmountUsd ?? 0) > 0))
                                {
                                    doPaymentMatchingDesc doMatchingDesc = incomeHandler.GetPaymentMatchingDesc(PaymentMatchingDesc.C_PAYMENT_MATCHING_DESC_WHT);
                                    doPaymentMatchingResultDetail matchingWhtAmount = new doPaymentMatchingResultDetail()
                                    {
                                        GridMatchDate = header.MatchDate,
                                        InvoiceNo = detail.InvoiceNo,
                                        ReceiptNo = CommonUtil.IsNullOrEmpty(detail.ReceiptNo) ? "-" : detail.ReceiptNo,
                                        BillingTargetCode = detail.BillingTargetCode,
                                        GridBillingClientNameOrCustomValue = doMatchingDesc.Description,        //Custom Value
                                        MatchAmountIncWHT = (detail.WHTAmount ?? 0) * (-1),            //Minus value  // update by jirawat jannet on 2016-11-16
                                        GridPartialPayment = displayPartialFlagNo,
                                        MatchingDetailSection = (int)ICS080_MATCHING_DETAIL_SECTION.MatchingResult,
                                        MatchingDetailSorting = (int)ICS080_MATCHING_DETAIL_SORTING.WHTAmount,
                                        MatchAmountIncWHTCurrencyType = detail.MatchAmountIncWHTCurrencyType, // add by Jirawat Jannet @ 2016-10-25
                                        MatchAmountIncWHTUsd = (detail.WHTAmountUsd ?? 0) * (-1), // add by Jirawat Jannet @ 2016-10-25 // update by jirawat jannet on 2016-11-16
                                        IsMatchAmountIncWHTShowDash = true
                                    };
                                    decoreated.Add(matchingWhtAmount);
                                }
                                #endregion
                            }
                            else
                            {
                                #region CancelFlag == true
                                // Add detail
                                doPaymentMatchingDesc doMatchingDesc = incomeHandler.GetPaymentMatchingDesc(PaymentMatchingDesc.C_PAYMENT_MATCHING_DESC_CANCEL);
                                doPaymentMatchingResultDetail matchingDeletedDetail = new doPaymentMatchingResultDetail()
                                {
                                    GridMatchDate = detail.UpdateDate.Value,
                                    InvoiceNo = detail.InvoiceNo,
                                    ReceiptNo = CommonUtil.IsNullOrEmpty(detail.ReceiptNo) ? "-" : detail.ReceiptNo,
                                    BillingTargetCode = detail.BillingTargetCode,
                                    GridBillingClientNameOrCustomValue = doMatchingDesc.Description,        //Custom Value
                                    MatchAmountIncWHT = detail.MatchAmountIncWHT * (-1),            //Minus value
                                    GridPartialPayment = detail.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL ? displayPartialFlagYes : displayPartialFlagNo,
                                    GirdBusinessDecorateFlag = redColor,
                                    MatchingDetailSection = (int)ICS080_MATCHING_DETAIL_SECTION.MatchingResult,
                                    MatchingDetailSorting = (int)ICS080_MATCHING_DETAIL_SORTING.CancelMatchAmount,
                                    MatchAmountIncWHTUsd = detail.MatchAmountIncWHTUsd * (-1), // add by Jirawat Jannet @ 2016-10-25
                                    MatchAmountExcWHTCurrencyType = detail.MatchAmountExcWHTCurrencyType, // add by Jirawat Jannet @ 2016-10-25
                                    IsMatchAmountIncWHTShowDash = true
                                };
                                decoreated.Add(matchingDeletedDetail);

                                //Add WHTAmount
                                // edit if condition by Jirawat Jannet @ 2016-10-25
                                if (detail.WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && ((detail.WHTAmount ?? 0) > 0)
                                    || detail.WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US && ((detail.WHTAmountUsd ?? 0) > 0))
                                {
                                    doMatchingDesc = incomeHandler.GetPaymentMatchingDesc(PaymentMatchingDesc.C_PAYMENT_MATCHING_DESC_CANCEL_WHT);
                                    doPaymentMatchingResultDetail matchingDeletedWhtAmount = new doPaymentMatchingResultDetail()
                                    {
                                        GridMatchDate = detail.UpdateDate.Value,
                                        InvoiceNo = detail.InvoiceNo,
                                        ReceiptNo = CommonUtil.IsNullOrEmpty(detail.ReceiptNo) ? "-" : detail.ReceiptNo,
                                        BillingTargetCode = detail.BillingTargetCode,
                                        GridBillingClientNameOrCustomValue = doMatchingDesc.Description,        //Custom Value
                                        MatchAmountIncWHT = detail.WHTAmount.Value,
                                        GridPartialPayment = displayPartialFlagNo,
                                        GirdBusinessDecorateFlag = redColor,
                                        MatchingDetailSection = (int)ICS080_MATCHING_DETAIL_SECTION.MatchingResult,
                                        MatchingDetailSorting = (int)ICS080_MATCHING_DETAIL_SORTING.CancelWHTAmount,
                                        MatchAmountIncWHTUsd = detail.WHTAmountUsd.Value, // add by Jirawat Jannet @ 2016-10-25
                                        MatchAmountExcWHTCurrencyType = detail.MatchAmountExcWHTCurrencyType ,// add by Jirawat Jannet @ 2016-10-25
                                        IsMatchAmountIncWHTShowDash = false
                                    };
                                    decoreated.Add(matchingDeletedWhtAmount);
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }

                    #region Header
                    if ((header.CancelFlag ?? false) == false)
                    {
                        #region CancelFlag == false
                        #region BankFee
                        // edit if condition by Jirawat Jannet @ 2016-10-25
                        if (header.BankFeeAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && ((header.BankFeeAmount ?? 0) > 0)
                            || header.BankFeeAmountCurrencyType == CurrencyUtil.C_CURRENCY_US && ((header.BankFeeAmountUsd ?? 0) > 0))
                        {
                            doPaymentMatchingDesc doMatchingDesc = incomeHandler.GetPaymentMatchingDesc(PaymentMatchingDesc.C_PAYMENT_MATCHING_DESC_BANK_FEE);
                            doPaymentMatchingResultDetail matchingBankFee = new doPaymentMatchingResultDetail()
                            {
                                GridMatchDate = header.MatchDate,
                                InvoiceNo = "-",
                                ReceiptNo = "-",
                                BillingTargetCode = "-",
                                GridBillingClientNameOrCustomValue = doMatchingDesc.Description,        //Custom Value
                                //MatchAmountIncWHT = header.BankFeeAmount.Value * (-1),            //Minus value comment by Jirawat Jannet @ 2016-10-25
                                MatchAmountIncWHT = (header.BankFeeAmount ?? 0) * (-1),            //Minus value add by jirawat jannet @ 2016-10-25
                                GridPartialPayment = displayPartialFlagNo,
                                MatchingDetailSection = (int)ICS080_MATCHING_DETAIL_SECTION.MatchingResult,
                                MatchingDetailSorting = (int)ICS080_MATCHING_DETAIL_SORTING.BankFee,
                                MatchAmountIncWHTUsd = (header.BankFeeAmountUsd ?? 0) * (-1), // add by jirawat jannet @ 2016-10-25
                                MatchAmountIncWHTCurrencyType = header.BankFeeAmountCurrencyType, // add by jirawat jannet @ 2016-10-25
                                IsMatchAmountIncWHTShowDash = true
                            };
                            decoreated.Add(matchingBankFee);
                        }
                        #endregion

                        #region OtherExpense
                        //if ((header.OtherExpenseAmount ?? 0) > 0)
                        if (header.OtherExpenseAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL &&((header.OtherExpenseAmount ?? 0) > 0)
                            || header.OtherExpenseAmountCurrencyType == CurrencyUtil.C_CURRENCY_US && ((header.OtherExpenseAmountUsd ?? 0) > 0))
                        {
                            doPaymentMatchingDesc doMatchingDesc = incomeHandler.GetPaymentMatchingDesc(PaymentMatchingDesc.C_PAYMENT_MATCHING_DESC_OTH_EXP);

                            doPaymentMatchingResultDetail matchingOtherExpense = new doPaymentMatchingResultDetail()
                            {
                                GridMatchDate = header.MatchDate,
                                InvoiceNo = "-",
                                ReceiptNo = "-",
                                BillingTargetCode = "-",
                                GridBillingClientNameOrCustomValue = doMatchingDesc.Description,        //Custom Value
                                //MatchAmountIncWHT = header.OtherExpenseAmount.Value * (-1),            //Minus value comment by Jirawat Jannet @ 2016-10-25
                                MatchAmountIncWHT = (header.OtherExpenseAmount ?? 0) * (-1),            //Minus value add by jirawat jannet @ 2016-10-25
                                GridPartialPayment = displayPartialFlagNo,
                                MatchingDetailSection = (int)ICS080_MATCHING_DETAIL_SECTION.MatchingResult,
                                MatchingDetailSorting = (int)ICS080_MATCHING_DETAIL_SORTING.OtherExpense,
                                MatchAmountIncWHTUsd = (header.OtherExpenseAmountUsd ?? 0) * (-1), // add by jirawat jannet @ 2016-10-25
                                MatchAmountIncWHTCurrencyType = header.OtherExpenseAmountCurrencyType, // add by jirawat jannet @ 2016-10-25
                                IsMatchAmountIncWHTShowDash = true
                            };
                            decoreated.Add(matchingOtherExpense);
                        }
                        #endregion

                        #region OtherIncome
                        if (header.OtherIncomeAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && ((header.OtherIncomeAmount ?? 0) > 0)
                            || header.OtherIncomeAmountCurrencyType == CurrencyUtil.C_CURRENCY_US && ((header.OtherIncomeAmountUsd ?? 0) > 0))
                        {
                            doPaymentMatchingDesc doMatchingDesc = incomeHandler.GetPaymentMatchingDesc(PaymentMatchingDesc.C_PAYMENT_MATCHING_DESC_OTH_INC);
                            doPaymentMatchingResultDetail matchingOtherIncome = new doPaymentMatchingResultDetail()
                            {
                                GridMatchDate = header.MatchDate,
                                InvoiceNo = "-",
                                ReceiptNo = "-",
                                BillingTargetCode = "-",
                                GridBillingClientNameOrCustomValue = doMatchingDesc.Description,        //Custom Value
                                //MatchAmountIncWHT = header.OtherIncomeAmount.Value, // comment by Jirawat Jannet @ 2016-10-25
                                MatchAmountIncWHT = header.OtherIncomeAmount ?? 0, // add by Jirawat Jannet @ 2016-10-25
                                GridPartialPayment = displayPartialFlagNo,
                                MatchingDetailSection = (int)ICS080_MATCHING_DETAIL_SECTION.MatchingResult,
                                MatchingDetailSorting = (int)ICS080_MATCHING_DETAIL_SORTING.OtherIncome,
                                MatchAmountIncWHTUsd = header.OtherIncomeAmountUsd ?? 0, // add by Jirawat Jannet @ 2016-10-25
                                MatchAmountIncWHTCurrencyType = header.OtherIncomeAmountCurrencyType, // add by Jirawat Jannet @ 2016-10-25
                                IsMatchAmountIncWHTShowDash = false
                            };
                            decoreated.Add(matchingOtherIncome);
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        #region CancelFlag == true
                        #region BankFee
                        // add and if condition by Jirawat Jannet @ 2016-10-25
                        //if ((header.BankFeeAmount ?? 0) > 0)
                        if (header.BankFeeAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && ((header.BankFeeAmount ?? 0) > 0)
                          || header.BankFeeAmountCurrencyType == CurrencyUtil.C_CURRENCY_US && ((header.BankFeeAmountUsd ?? 0) > 0))
                        {
                            doPaymentMatchingDesc doMatchingDesc = incomeHandler.GetPaymentMatchingDesc(PaymentMatchingDesc.C_PAYMENT_MATCHING_DESC_CANCEL_BANK_FEE);
                            doPaymentMatchingResultDetail matchingBankFee = new doPaymentMatchingResultDetail()
                            {
                                GridMatchDate = header.UpdateDate.Value,
                                InvoiceNo = "-",
                                ReceiptNo = "-",
                                BillingTargetCode = "-",
                                GridBillingClientNameOrCustomValue = doMatchingDesc.Description,        //Custom Value
                                //MatchAmountIncWHT = header.BankFeeAmount.Value, // comment by Jirawat Jannet @ 2016-10-25
                                MatchAmountIncWHT = header.BankFeeAmount ?? 0, // add by Jirawat Jannet @ 2016-10-25
                                GridPartialPayment = displayPartialFlagNo,
                                GirdBusinessDecorateFlag = redColor,
                                MatchingDetailSection = (int)ICS080_MATCHING_DETAIL_SECTION.MatchingResult,
                                MatchingDetailSorting = (int)ICS080_MATCHING_DETAIL_SORTING.CancelBankFee,
                                MatchAmountIncWHTCurrencyType = header.BankFeeAmountCurrencyType, // add by Jirawat Jannet @ 2016-10-25
                                MatchAmountIncWHTUsd = header.BankFeeAmountUsd ?? 0, // add by Jirawat Jannet @ 2016-10-25
                                IsMatchAmountIncWHTShowDash = false // add by Jirawat Jannet @ 2016-10-25
                            };
                            decoreated.Add(matchingBankFee);
                        }
                        #endregion

                        #region OtherExpense
                        // add and if condition by Jirawat Jannet @ 2016-10-25
                        //if ((header.OtherExpenseAmount ?? 0) > 0)
                        if (header.OtherExpenseAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && ((header.OtherExpenseAmount ?? 0) > 0)
                            || header.OtherExpenseAmountCurrencyType == CurrencyUtil.C_CURRENCY_US && ((header.OtherExpenseAmountUsd ?? 0) > 0))
                        {
                            doPaymentMatchingDesc doMatchingDesc = incomeHandler.GetPaymentMatchingDesc(PaymentMatchingDesc.C_PAYMENT_MATCHING_DESC_CANCEL_OTH_EXP);
                            doPaymentMatchingResultDetail matchingOtherExpense = new doPaymentMatchingResultDetail()
                            {
                                GridMatchDate = header.UpdateDate.Value,
                                InvoiceNo = "-",
                                ReceiptNo = "-",
                                BillingTargetCode = "-",
                                GridBillingClientNameOrCustomValue = doMatchingDesc.Description,        //Custom Value
                                //MatchAmountIncWHT = header.OtherExpenseAmount.Value, // comment by Jirawat Jannet @ 2016-10-25
                                MatchAmountIncWHT = header.OtherExpenseAmount ?? 0, // add by Jirawat Jannet @ 2016-10-25
                                GridPartialPayment = displayPartialFlagNo,
                                GirdBusinessDecorateFlag = redColor,
                                MatchingDetailSection = (int)ICS080_MATCHING_DETAIL_SECTION.MatchingResult,
                                MatchingDetailSorting = (int)ICS080_MATCHING_DETAIL_SORTING.CancelOtherExpense,
                                MatchAmountIncWHTUsd = header.OtherExpenseAmountUsd ?? 0, // add by jirawat jannet @ 2016-10-25
                                MatchAmountIncWHTCurrencyType = header.OtherExpenseAmountCurrencyType, // add by jirawat jannet @ 2016-10-25
                                IsMatchAmountIncWHTShowDash = false
                            };
                            decoreated.Add(matchingOtherExpense);
                        }
                        #endregion

                        #region OtherIncome
                        // add and if condition by Jirawat Jannet @ 2016-10-25
                        //if ((header.OtherIncomeAmount ?? 0) > 0)
                        if (header.OtherIncomeAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && ((header.OtherIncomeAmount ?? 0) > 0)
                            || header.OtherIncomeAmountCurrencyType == CurrencyUtil.C_CURRENCY_US && ((header.OtherIncomeAmountUsd ?? 0) > 0))
                        {
                            doPaymentMatchingDesc doMatchingDesc = incomeHandler.GetPaymentMatchingDesc(PaymentMatchingDesc.C_PAYMENT_MATCHING_DESC_CANCEL_OTH_INC);
                            doPaymentMatchingResultDetail matchingOtherIncome = new doPaymentMatchingResultDetail()
                            {
                                GridMatchDate = header.UpdateDate.Value,
                                InvoiceNo = "-",
                                ReceiptNo = "-",
                                BillingTargetCode = "-",
                                GridBillingClientNameOrCustomValue = doMatchingDesc.Description,        //Custom Value
                                //MatchAmountIncWHT = header.OtherIncomeAmount.Value * (-1),            //Minus value comment by Jirawat Jannet @ 2016-10-25
                                MatchAmountIncWHT = (header.OtherIncomeAmount ?? 0) * (-1),            // add by Jirawat Jannet @ 2016-10-25
                                GridPartialPayment = displayPartialFlagNo,
                                GirdBusinessDecorateFlag = redColor,
                                MatchingDetailSection = (int)ICS080_MATCHING_DETAIL_SECTION.MatchingResult,
                                MatchingDetailSorting = (int)ICS080_MATCHING_DETAIL_SORTING.CancelOtherIncome,
                                MatchAmountIncWHTUsd = (header.OtherIncomeAmountUsd ?? 0) * (-1), // add by Jirawat Jannet @ 2016-10-25
                                MatchAmountIncWHTCurrencyType = header.OtherIncomeAmountCurrencyType, // add by Jirawat Jannet @ 2016-10-25
                                IsMatchAmountIncWHTShowDash = true // add by Jirawat Jannet @ 2016-10-25
                            };
                            decoreated.Add(matchingOtherIncome);
                        }
                        #endregion
                        #endregion
                    }
                    #endregion
                }
            }
            #endregion


            //Sorting information as below 
            //1. MatchDate Desc
            //2. InvoiceNo Asc ('-' at last)
            //3. MatchingDetailSection Asc (ICS080_MATCHING_DETAIL_SECTION)
            //4. MatchingDetailSorting Asc (ICS080_MATCHING_DETAIL_SORTING)
            decoreated = (from data in decoreated
                          orderby data.GridMatchDate descending
                                , ((data.InvoiceNo.Equals("-") == false) ? data.InvoiceNo : "ZZZZZZZZZZZZ") ascending
                                , data.MatchingDetailSection ascending
                            select data).ToList<doPaymentMatchingResultDetail>();
            return decoreated;
        }
        

        private ICS080_PaymentMatchingResult ICS080_GetPaymentMatchingResult(string paymentTranNo)
        {
            ICS080_PaymentMatchingResult result = new ICS080_PaymentMatchingResult() { 
                doTbt_Invoice = new List<tbt_Invoice>(),
                IsPermissionEncash = false,
                IsEncashable = false,
                IsEncashed = false
            };

            //Get payment information
            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            doPayment doPayment = incomeHandler.SearchPayment(new doPaymentSearchCriteria() { PaymentTransNo = paymentTranNo }).First();
            result.doPayment = doPayment;

            //Get encash information
            result.IsPermissionEncash = CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_PAYMENT_INFO, FunctionID.C_FUNC_ID_ENCASH);
            if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE
                    || doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
            {
                result.doTbt_Invoice = incomeHandler.GetInvoicePaymentMatchingList(doPayment.PaymentTransNo);
                if (result.doTbt_Invoice.Count > 0)
                {
                    result.IsEncashable = (
                        doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE 
                        || doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED
                    );

                    result.IsEncashed = doPayment.EncashedFlag.HasValue && doPayment.EncashedFlag.Value == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_ENCASHED;
                }
            }
            return result;
        }
        #endregion
    }
}