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
using SECOM_AJIS.Presentation.Common.Helpers;
using SECOM_AJIS.Presentation.Common.Service;

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
        public ActionResult ICS084_Authority(ICS084_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS084_IsAllowOperate(res))
                    return Json(res);

                ICS084_ScreenData = param;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<ICS084_ScreenParameter>("ICS084", param, res);
        }
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool ICS084_IsAllowOperate(ObjectResultData res)
        {
            //Check Permission 
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_MATCH_PAYMENT_BY_INVOICE, FunctionID.C_FUNC_ID_OPERATE) == false)
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
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS084()
        {
            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            doPayment payment = new doPayment();

            // Set Payment Info
            if (!CommonUtil.IsNullOrEmpty(ICS084_ScreenData.PaymentTransNo))
            {
                payment = incomeHandler.SearchPayment(new doPaymentSearchCriteria() { PaymentTransNo = ICS084_ScreenData.PaymentTransNo }).FirstOrDefault();
                ICS084_ScreenData.doPayment = payment;
                if (payment != null)
                {
                    ViewBag.PaymentTransNo = payment.PaymentTransNo;

                    ViewBag.MatchableBalanceCurrencyType = payment.MatchableBalanceCurrencyType;
                    if (payment.MatchableBalanceCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        ViewBag.MatchableBalance = payment.MatchableBalance.ToString("#,##0.00");
                    else if (payment.MatchableBalanceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                        ViewBag.MatchableBalance = payment.MatchableBalanceUsd.ToString("#,##0.00");
                    else
                        ViewBag.MatchableBalance = "0.00";


                    ViewBag.FirstPaymentAmountCurrencyType = payment.PaymentAmountCurrencyType;
                    if (payment.PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        ViewBag.FirstPaymentAmount = payment.PaymentAmount?.ToString("#,##0.00");
                    else if(payment.PaymentAmountCurrencyType  == CurrencyUtil.C_CURRENCY_US)
                        ViewBag.FirstPaymentAmount = payment.PaymentAmountUsd?.ToString("#,##0.00");
                    else
                        ViewBag.FirstPaymentAmount = "0.00";


                    ViewBag.SECOMBankBranch = payment.SECOMBankFullName;
                    ViewBag.PaymentDate = payment.PaymentDate.ToString("dd-MMM-yyyy");
                    ViewBag.PayerName = payment.Payer;
                    ViewBag.Memo = payment.Memo;

                    ViewBag.BankFeeRegisteredFlag = payment.BankFeeRegisteredFlag.HasValue ? payment.BankFeeRegisteredFlag.Value : false;
                    ViewBag.OtherIncomeRegisteredFlag = payment.OtherIncomeRegisteredFlag.HasValue ? payment.OtherIncomeRegisteredFlag.Value : false;
                    ViewBag.OtherExpenseRegisteredFlag = payment.OtherExpenseRegisteredFlag.HasValue ? payment.OtherExpenseRegisteredFlag.Value : false;
                }

                //Add by Jutarat A. on 13062013
                tbt_Payment paymentData = incomeHandler.GetPayment(ICS084_ScreenData.PaymentTransNo);
                ICS084_ScreenData.PaymentData = paymentData;
                ViewBag.WHTFlag = paymentData.WHTFlag ?? true;
                //End Add
            }

            //ViewBag is used for displaying information on screen
            ViewBag.ScreenCaller = ICS084_ScreenData.ScreenCaller;
            ViewBag.BillingTargetCodeLongFormat = string.Empty;
            ViewBag.BillingTargetCode = string.Empty;
            ViewBag.InvoiceNo = string.Empty;
            ViewBag.BillingClientNameEN = string.Empty;
            ViewBag.BillingClientNameLC = string.Empty;


            if (ICS084_ScreenData != null && !string.IsNullOrEmpty(ICS084_ScreenData.BillingTargetCode))
            {
                //Call by ICS081
                ViewBag.BillingTargetCodeLongFormat = ICS084_ScreenData.BillingTargetCode;
                ViewBag.BillingTargetCode = new CommonUtil().ConvertBillingTargetCode(ICS084_ScreenData.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                ViewBag.InvoiceNo = payment.RefInvoiceNo;
                List<doUnpaidBillingTarget> unpaidBilling = incomeHandler.GetUnpaidBillingTargetByCode(ICS084_ScreenData.BillingTargetCode);
                if (unpaidBilling != null && unpaidBilling.Count > 0)
                {
                    ViewBag.BillingClientNameEN = unpaidBilling[0].BillingClientNameEN;
                    ViewBag.BillingClientNameLC = unpaidBilling[0].BillingClientNameLC;
                }
            }
            else if (ICS084_ScreenData != null && !string.IsNullOrEmpty(ICS084_ScreenData.InvoiceNo))
            {
                //Call by ICS080
                List<doUnpaidBillingTarget> unpaidBilling = incomeHandler.GetUnpaidBillingTargetByInvoiceNo(ICS084_ScreenData.InvoiceNo);
                if (unpaidBilling != null && unpaidBilling.Count > 0)
                {
                    ICS084_ScreenData.BillingTargetCode = unpaidBilling[0].BillingTargetCode;
                    ViewBag.BillingTargetCodeLongFormat = unpaidBilling[0].BillingTargetCode;
                    ViewBag.BillingTargetCode = new CommonUtil().ConvertBillingTargetCode(unpaidBilling[0].BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.InvoiceNo = ICS084_ScreenData.InvoiceNo;
                    ViewBag.BillingClientNameEN = unpaidBilling[0].BillingClientNameEN;
                    ViewBag.BillingClientNameLC = unpaidBilling[0].BillingClientNameLC;
                }
            }
            return View("_ICS084");
        }
        #endregion

        #region Actions

        #region Get / Set Screen

        /// <summary>
        /// Set inputted screen session to the system
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS084_SetScreenData(ICS084_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            try
            {
                ICS084_ScreenData = param;

                //ResultFlag = Success;
                res.ResultData = "1";
                return Json(true);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Generate xml of unpaid invoice list of specific invoice no., billing target code
        /// </summary>
        /// <param name="param">invoice no., billing target code</param>
        /// <returns></returns>
        public ActionResult ICS084_GetInvoiceGrid(ICS084_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                List<doUnpaidInvoice> unpaidInvoice = GetUnpaidInvoice(param.ScreenCaller, param.InvoiceNo, param.BillingTargetCode);
                //foreach(var item in unpaidInvoice)
                //{
                //    item.IncomeCurrencyType = param.FirstPaymentAmountCurrencyType;
                //}
                unpaidInvoice.RemoveAll(m => m.BillingAmountIncTaxShow == "-");
                ICS084_ScreenData.UnpaidInvoice = unpaidInvoice;
                res.ResultData = CommonUtil.ConvertToXml<doUnpaidInvoice>(unpaidInvoice, "Income\\ICS084", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        #endregion

        #region Generate Control

        // add by jirawat jannet @ 2016-10-27
        public ActionResult GenerateCurrencyNumericTextBox(string id, string value, string currency)
        {
            string html = TextBoxHelper.NumericTextBoxWithMultipleCurrency(null, id, value, currency, new { style = "width: 140px;" }).ToString();
            return Json(html);
        }
        public ActionResult GenerateCurrencyNumericTextBoxControl()
        {
            string html = TextBoxHelper.NumericTextBoxWithMultipleCurrency(null, "{{id}}", "{{value}}", "{{currency}}", new { style = "width: 140px;" }).ToString();
            return Json(html);
        }

        #endregion

        #region Register

        /// <summary>
        /// Event when click register button
        /// </summary>
        /// <param name="param">payment matching information</param>
        /// <returns></returns>
        public ActionResult ICS084_cmdRegister(ICS084_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS010_IsAllowOperate(res))
                    return Json(res);

                //Validate only Existing Payment List
                ValidatorUtil validator = new ValidatorUtil();
                decimal balanceAfterProcessing = 0;
                string balanceAfterProcessingCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                List<string> confirmMessageID = new List<string>();
                ICS084_ValidateAdjustInvoiceData(validator, param, true, out balanceAfterProcessing, out confirmMessageID, out balanceAfterProcessingCurrencyType);


                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);



                ICS084_ScreenData.BalanceAfterProcessing = balanceAfterProcessing;
                ICS084_ScreenData.BalanceAfterProcessingCurrencyType = balanceAfterProcessingCurrencyType;
                ICS084_AdjustMatchPaymentRegister result = new ICS084_AdjustMatchPaymentRegister()
                {
                    //Result flag  1 = Success
                    ResultFlag = "1",
                    BalanceAfterProcessing = balanceAfterProcessing,
                    ConfirmMessageID = confirmMessageID,
                    BalanceAfterProcessingCurrencyType = balanceAfterProcessingCurrencyType
                };
                res.ResultData = result;
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        #endregion

        #region Confirm pack function

        /// <summary>
        /// Event when click confirm button, this function will register payment matching information to the system.
        /// </summary>
        /// <param name="param">payment matching information</param>
        /// <returns></returns>
        public ActionResult ICS084_cmdConfirm(ICS084_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS010_IsAllowOperate(res))
                    return Json(res);

                //Validate only Existing Payment List
                ValidatorUtil validator = new ValidatorUtil();
                decimal balanceAfterProcessing = 0;
                string balanceAfterProcessingCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                List<string> confirmMessageID = new List<string>();
                ICS084_ValidateAdjustInvoiceData(validator, param, false, out balanceAfterProcessing, out confirmMessageID, out balanceAfterProcessingCurrencyType);

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);

                List<doMatchPaymentDetail> matchDetails = GenMatchPaymentDetailDatas(param);
                doMatchPaymentHeader matchHeader = GenMatchPaymentHeaderData(param, matchDetails);

                IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                tbt_Payment payment = ICS084_ScreenData.PaymentData;
                if (payment == null)
                    payment = handler.GetPayment(ICS084_ScreenData.PaymentTransNo);

                using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                    bool isSuccess = handler.MatchPaymentInvoices(matchHeader, payment, param.FirstPaymentAmountCurrencyType);
                    if (isSuccess)
                    {
                        scope.Complete();
                        //Result flag  1 = Success
                        res.ResultData = "1";
                        return Json(res);
                    }
                    else
                    {
                        scope.Dispose();
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7066);
                        return Json(res);
                    }
                }
                catch (Exception ex)
                {
                    //all Fail
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

        // Add by Jirawat Jannet on 2016-12-08
        private doMatchPaymentHeader GenMatchPaymentHeaderData(ICS084_ScreenParameter param, List<doMatchPaymentDetail> matchDetails)
        {
            double errorCode = 0;
            decimal totalMatchAmount = 0;
            string totalMatchAmountCurrencyType = string.Empty;
            foreach (var item in param.MatchInvoiceData)
            {
                decimal KeyInMatchAmountIncWHT = item.KeyInMatchAmountIncWHT.ConvertCurrencyPrice(item.KeyInMatchAmountIncWHTCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                decimal KeyInWHTAmount = item.KeyInWHTAmount.ConvertCurrencyPrice(item.KeyInWHTAmountCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                totalMatchAmount += KeyInMatchAmountIncWHT - KeyInWHTAmount;
                totalMatchAmountCurrencyType = item.KeyInMatchAmountIncWHTCurrencyType;
            }

            decimal? TotalMatchAmount = null, TotalMatchAmountUsd = null
                , OtherExpenseAmount = null, OtherExpenseAmountUsd = null
                , OtherIncomeAmount = null, OtherIncomeAmountUsd = null
                , BankFeeAmount = null, BankFeeAmountUsd = null;

            if (totalMatchAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                TotalMatchAmount = totalMatchAmount;
            else if (totalMatchAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                TotalMatchAmountUsd = totalMatchAmount;

            if (param.OtherExpenseCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                OtherExpenseAmount = param.OtherExpense.ConvertCurrencyPrice(param.OtherExpenseCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode);
            else if (param.OtherExpenseCurrencyType == CurrencyUtil.C_CURRENCY_US)
                OtherExpenseAmountUsd = param.OtherExpense.ConvertCurrencyPrice(param.OtherExpenseCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode);

            if (param.OtherIncomeCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                OtherIncomeAmount = param.OtherIncome.ConvertCurrencyPrice(param.OtherIncomeCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode);
            else if (param.OtherIncomeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                OtherIncomeAmountUsd = param.OtherIncome.ConvertCurrencyPrice(param.OtherIncomeCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode);

            if (param.BankFeeCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                BankFeeAmount = param.BankFee.ConvertCurrencyPrice(param.BankFeeCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode);
            else if (param.BankFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                BankFeeAmountUsd = param.BankFee.ConvertCurrencyPrice(param.BankFeeCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode);

            return new doMatchPaymentHeader()
            {
                MatchID = null,
                MatchDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                PaymentTransNo = ICS084_ScreenData.PaymentTransNo,

                TotalMatchAmount = TotalMatchAmount.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, totalMatchAmountCurrencyType, DateTime.Now, ref errorCode),
                TotalMatchAmountUsd = TotalMatchAmountUsd.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, totalMatchAmountCurrencyType, DateTime.Now, ref errorCode),
                TotalMatchAmountCurrencyType = totalMatchAmountCurrencyType,

                BankFeeAmount = BankFeeAmount.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, param.BankFeeCurrencyType, DateTime.Now, ref errorCode),
                BankFeeAmountUsd = BankFeeAmountUsd.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, param.BankFeeCurrencyType, DateTime.Now, ref errorCode),
                BankFeeAmountCurrencyType = param.BankFeeCurrencyType,

                SpecialProcessFlag = param.SpecialProcess,
                ApproveNo = param.SpecialProcess ? param.ApproveNo : null,

                OtherExpenseAmount = OtherExpenseAmount.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, param.OtherExpenseCurrencyType, DateTime.Now, ref errorCode),
                OtherExpenseAmountUsd = OtherExpenseAmountUsd.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, param.OtherExpenseCurrencyType, DateTime.Now, ref errorCode),
                OtherExpenseAmountCurrencyType = param.OtherExpenseCurrencyType,

                OtherIncomeAmount = OtherIncomeAmount.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, param.OtherIncomeCurrencyType, DateTime.Now, ref errorCode),
                OtherIncomeAmountUsd = OtherIncomeAmountUsd.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, param.OtherIncomeCurrencyType, DateTime.Now, ref errorCode),
                OtherIncomeAmountCurrencyType = param.OtherIncomeCurrencyType,

                CancelFlag = false,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                MatchPaymentDetail = matchDetails,

                ExchangeGain = param.ExchangeGain,
                ExchangeGainCurrencyType = param.ExchangeGainCurrencyType,

                ExchangeLoss = param.ExchangeLoss,
                ExchangeLossCurrencyType = param.ExchangeLossCurrencyType
            };
        }

        // Add by Jirawat Jannet on 2016-12-08
        private List<doMatchPaymentDetail> GenMatchPaymentDetailDatas(ICS084_ScreenParameter param)
        {
            double errorCode = 0;
            List<doMatchPaymentDetail> matchDetails = new List<doMatchPaymentDetail>();
            foreach (var item in param.MatchInvoiceData)
            {
                doUnpaidInvoice unpaidInvoice = ICS084_ScreenData.UnpaidInvoice.Where(d => d.InvoiceNo == item.InvoiceNo).First();
                decimal matchAmountIncWht = item.KeyInMatchAmountIncWHT.ConvertCurrencyPrice(item.KeyInMatchAmountIncWHTCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                decimal whtAmount = item.KeyInWHTAmount.ConvertCurrencyPrice(item.KeyInWHTAmountCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;

                decimal UnpaidAmount = unpaidInvoice.UnpaidAmountVal.ConvertCurrencyPrice(unpaidInvoice.UnpaidAmountCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode);

                string matchStatus = string.Empty;

                if (matchAmountIncWht < UnpaidAmount)
                {
                    matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL;
                }
                else if (unpaidInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID)
                {
                    matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL_TO_FULL;
                }
                else
                {
                    matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL;
                }

                decimal? MatchAmountExcWHT = null, MatchAmountExcWHTUsd = null, MatchAmountIncWHT = null
                    , MatchAmountIncWHTUsd = null, WHTAmount = null, WHTAmountUsd = null;

                if (item.KeyInMatchAmountIncWHTCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    MatchAmountExcWHT = matchAmountIncWht - whtAmount;
                    MatchAmountIncWHT = matchAmountIncWht;
                }
                else if (item.KeyInMatchAmountIncWHTCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    MatchAmountExcWHTUsd = matchAmountIncWht - whtAmount;
                    MatchAmountIncWHTUsd = matchAmountIncWht;
                }

                if (item.KeyInWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    WHTAmount = whtAmount;
                else if (item.KeyInWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    WHTAmountUsd = whtAmount;

                matchDetails.Add(new doMatchPaymentDetail()
                {
                    MatchID = null,
                    InvoiceNo = item.InvoiceNo,
                    InvoiceOCC = unpaidInvoice.InvoiceOCC,
                    MatchAmountExcWHT = MatchAmountExcWHT.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, item.KeyInMatchAmountIncWHTCurrencyType, DateTime.Now, ref errorCode, 0).Value,
                    MatchAmountExcWHTUsd = MatchAmountExcWHTUsd.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, item.KeyInMatchAmountIncWHTCurrencyType, DateTime.Now, ref errorCode, 0).Value,
                    MatchAmountExcWHTCurrencyType = item.KeyInMatchAmountIncWHTCurrencyType,
                    MatchAmountIncWHT = MatchAmountIncWHT.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, item.KeyInMatchAmountIncWHTCurrencyType, DateTime.Now, ref errorCode, 0).Value,
                    MatchAmountIncWHTUsd = MatchAmountIncWHTUsd.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, item.KeyInMatchAmountIncWHTCurrencyType, DateTime.Now, ref errorCode, 0).Value,
                    MatchAmountIncWHTCurrencyType = item.KeyInMatchAmountIncWHTCurrencyType, 
                    WHTAmount = WHTAmount.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, item.KeyInMatchAmountIncWHTCurrencyType, DateTime.Now, ref errorCode, 0).Value,
                    WHTAmountUsd = WHTAmountUsd.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, item.KeyInMatchAmountIncWHTCurrencyType, DateTime.Now, ref errorCode, 0).Value,
                    WHTAmountCurrencyType = item.KeyInWHTAmountCurrencyType,
                    MatchStatus = matchStatus,
                    CancelFlag = false,
                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime

                });
            }
            return matchDetails;
        }

        #endregion


        #endregion

        #region Methods
        /// <summary>
        /// Screen session
        /// </summary>
        private ICS084_ScreenParameter ICS084_ScreenData
        {
            get
            {
                return GetScreenObject<ICS084_ScreenParameter>("ics084");
            }
            set
            {
                UpdateScreenObject(value,"ics084");
            }
        }
        /// <summary>
        /// Retrieve unpaid invoice list of specific caller screen, invoice no., billing target code
        /// </summary>
        /// <param name="caller">caller screen</param>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="billingTargetCode">billing target code</param>
        /// <returns></returns>
        private List<doUnpaidInvoice> GetUnpaidInvoice(ICS084_ScreenCaller caller, string invoiceNo, string billingTargetCode)
        {
            IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            List<doUnpaidInvoice> doUnpaidInvoice = new List<doUnpaidInvoice>();

            // Set Unpaid Invoice by Screen Caller
            if (caller == ICS084_ScreenCaller.ICS080)
            {
                doUnpaidInvoice = handler.GetUnpaidInvoice(invoiceNo);
            }
            else if (caller == ICS084_ScreenCaller.ICS081)
            {
                doUnpaidInvoice = handler.GetUnpaidInvoiceByBillingTarget(billingTargetCode);
            }

            //Validate invoice payment status is able to match payment
            foreach(var item in doUnpaidInvoice)
            {
                if (ICS084_ScreenData != null && ICS084_ScreenData.doPayment != null  //Add by Jutarat A. on 28022013
                    && ICS084_ScreenData.doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
                {
                    if (item.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT)
                    {
                        item.IsToMatchableProcess = true;
                    }
                    else
                    {
                        //including item.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                        item.IsToMatchableProcess = false;
                    }
                }
                else
                {
                    //Other Payment type
                    if (item.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                        || item.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT)
                    {
                        item.IsToMatchableProcess = false;
                    }
                    else
                    {
                        item.IsToMatchableProcess = true;
                    }
                }
            }

            return doUnpaidInvoice;
        }
        /// <summary>
        /// Validate adjusted unpaid invoice list
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="param">payment matching information</param>
        /// <param name="isRegister">is occur when click register button</param>
        /// <param name="balanceAfterProcessing">balance amount after processing</param>
        /// <param name="confirmMessageID">warning message list</param>
        private void ICS084_ValidateAdjustInvoiceData(ValidatorUtil validator, ICS084_ScreenParameter param
            , bool isRegister, out decimal balanceAfterProcessing, out List<string> confirmMessageID, out string balanceAfterProcessingCurrencyType)
        {
            double errorCode = 0;
            balanceAfterProcessing = 0;
            confirmMessageID = new List<string>();

            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            doPayment doPayment = ICS084_ScreenData.doPayment;

            if (param.MatchInvoiceData != null && param.MatchInvoiceData.Count > 0)
            {
                balanceAfterProcessingCurrencyType = param.MatchInvoiceData[0].KeyInMatchAmountIncWHTCurrencyType;
            }
            else
            {
                balanceAfterProcessingCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
            }


            #region Not support current spect: Convery currency price
            // ADD BY JIRAWAT JANNET ON 216-11-21
            //if (param.MatchInvoiceData != null && param.MatchInvoiceData.Count > 0)
            //{
            //    foreach (var d in param.MatchInvoiceData)
            //    {
            //        if (d.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US
            //            && (d.KeyInMatchAmountIncWHTCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL
            //                || d.KeyInWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL))
            //        {
            //            validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
            //               , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7131
            //               , "");
            //            return;
            //        }
            //    }
            //}
            #endregion

            #region CHECKED special process
            if (param.SpecialProcess)
            {
                //required approve no
                if (CommonUtil.IsNullOrEmpty(param.ApproveNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                       , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7102
                       , "ApproveNo", "lblApproveNo", "ApproveNo");
                    return;
                }

                //require income or expense
                if ((param.OtherExpense ?? 0) == 0 && (param.OtherIncome ?? 0) == 0)
                {
                    //Not input
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                       , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7102
                       , "OtherExpense", "lblOtherExpense", "OtherExpense");
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                       , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7102
                       , "OtherIncome", "lblOtherIncome", "OtherIncome");
                    return;
                }
                else
                {
                    if ((doPayment.OtherExpenseRegisteredFlag ?? false) == false && (param.OtherExpense ?? 0) == 0
                        && ((doPayment.OtherIncomeRegisteredFlag ?? false) == true || (param.OtherIncome ?? 0) == 0))
                    {
                        //Unregistered Expense
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                           , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7102
                           , "OtherExpense", "lblOtherExpense", "OtherExpense");
                        return;
                    }
                    else if ((doPayment.OtherIncomeRegisteredFlag ?? false) == false && (param.OtherIncome ?? 0) == 0
                        && ((doPayment.OtherExpenseRegisteredFlag ?? false) == true || (param.OtherExpense ?? 0) == 0))
                    {
                        //Unregistered Income
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                           , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7102
                           , "OtherIncome", "lblOtherIncome", "OtherIncome");
                        return;
                    }
                }
            }
            #endregion

            #region Match invoice data
            //At least one row
            if (CommonUtil.IsNullOrEmpty(param) || CommonUtil.IsNullOrEmpty(param.MatchInvoiceData) || param.MatchInvoiceData.Count == 0)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                   , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7035
                   , "");
                return;
            }

            foreach (var item in param.MatchInvoiceData)
            {
                //KeyIn all row
                if ((item.KeyInMatchAmountIncWHT ?? 0) == 0)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                       , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                       , item.KeyInMatchAmountIncWHT_ID ?? "", "gridHeaderMatchPaymentAmount", item.KeyInMatchAmountIncWHT_ID ?? "");
                    return;
                }
            }
            #endregion

            #region Validate business
             
            doPayment doPaymentNew = incomeHandler.SearchPayment(new doPaymentSearchCriteria() { PaymentTransNo = ICS084_ScreenData.PaymentTransNo }).First();

            #region Already registered

            //Already registered bankfee
            if ((doPaymentNew.BankFeeRegisteredFlag ?? false) == true && (param.BankFee ?? 0) > 0)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                   , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7092
                   , "");
                return;
            }

            //Already registered otherincome
            if ((doPaymentNew.OtherIncomeRegisteredFlag ?? false) == true && (param.OtherIncome ?? 0) > 0)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                   , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7094
                   , "");
                return;
            }

            //Already registered otherincome
            if ((doPaymentNew.OtherExpenseRegisteredFlag ?? false) == true && (param.OtherExpense ?? 0) > 0)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                   , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7093
                   , "");
                return;
            }

            #endregion

            #region Check balance after processing not minus : Old

            // //Check balance after processing not minus
            //decimal sumOfMatchPaymentAmount = 0;
            //decimal sumOfTotalWht = 0;
            //foreach (var item in param.MatchInvoiceData)
            //{
            //    sumOfMatchPaymentAmount += (item.KeyInMatchAmountIncWHT ?? 0);
            //    sumOfTotalWht += (item.KeyInWHTAmount ?? 0);
            //}
            ////Formular Balance After processing: 
            ////balanceAfterProcessing = payment.MatchableBalance - (sumOfMatchPaymentAmount - sumOfTotalWht)  + BankFee + OtherExpense - OtherIncome
            //if (doPaymentNew.MatchableBalanceCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
            //{
            //    balanceAfterProcessing = doPaymentNew.MatchableBalance -
            //            (sumOfMatchPaymentAmount - sumOfTotalWht) 
            //            + (param.BankFee ?? 0) + (param.OtherExpense ?? 0) - (param.OtherIncome ?? 0)
            //            + (param.ExchangeLoss ?? 0) - (param.ExchangeGain ?? 0);
            //}
            //else
            //{
            //    balanceAfterProcessing = doPaymentNew.MatchableBalanceUsd -
            //            (sumOfMatchPaymentAmount - sumOfTotalWht) 
            //            + (param.BankFee ?? 0) + (param.OtherExpense ?? 0) - (param.OtherIncome ?? 0)
            //            + (param.ExchangeLoss ?? 0) - (param.ExchangeGain ?? 0);
            //}

            //if (balanceAfterProcessing < 0)
            //{
            //    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
            //           , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7036
            //           , "");
            //    return;
            //}

            #endregion

            // add by jirawat jannet on 2016-12-08
            #region Check balance after processing not minus

            //Check balance after processing not minus
            decimal sumOfMatchPaymentAmount = 0;
            decimal sumOfTotalWht = 0;
            foreach (var item in param.MatchInvoiceData)
            {
                sumOfMatchPaymentAmount += item.KeyInMatchAmountIncWHT.ConvertCurrencyPrice(item.KeyInMatchAmountIncWHTCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                sumOfTotalWht += item.KeyInWHTAmount.ConvertCurrencyPrice(item.KeyInWHTAmountCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
            }
            //Formular Balance After processing: 
            decimal MatchableBalance = 0;
            if (doPaymentNew.MatchableBalanceCurrencyType == CurrencyType.C_CURRENCY_TYPE_USD)
                MatchableBalance = doPaymentNew.MatchableBalanceUsd.ConvertCurrencyPrice(doPaymentNew.MatchableBalanceCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode);
            else MatchableBalance = doPaymentNew.MatchableBalance.ConvertCurrencyPrice(doPaymentNew.MatchableBalanceCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode);

            balanceAfterProcessing = MatchableBalance -
                        (sumOfMatchPaymentAmount - sumOfTotalWht)
                        + param.BankFee.ConvertCurrencyPrice(param.BankFeeCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                        + param.OtherExpense.ConvertCurrencyPrice(param.OtherExpenseCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                        - param.OtherIncome.ConvertCurrencyPrice(param.OtherIncomeCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                        + param.ExchangeLoss.ConvertCurrencyPrice(param.ExchangeLossCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                        - param.ExchangeGain.ConvertCurrencyPrice(param.ExchangeGainCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;

            balanceAfterProcessing = balanceAfterProcessing.ConvertCurrencyPrice(param.FirstPaymentAmountCurrencyType, balanceAfterProcessingCurrencyType, DateTime.Now, ref errorCode);

            if (balanceAfterProcessing < 0)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                       , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7036
                       , "");
                return;
            }

            #endregion


            //Check match payment amount not greater than unpaid amount, check invoice is not same CN
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            SECOM_AJIS.DataEntity.Billing.doRefundInfo doRefund = new doRefundInfo();
            if (doPaymentNew.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND)
            {
                doRefund = billingHandler.GetRefundInfo(doPaymentNew.PaymentTransNo);
            }

            foreach (var item in param.MatchInvoiceData)
            {
                doInvoice doInvoice = billingHandler.GetInvoice(item.InvoiceNo);

                #region Check invoice is not same CN’s invoice
                if (doPaymentNew.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND)
                {
                    if (item.InvoiceNo == doRefund.InvoiceNo)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7084
                            , "", new string[] { item.InvoiceNo });
                        return;
                    }
                }

                // Comment by Jirawat Jannet on 2016-12-14
                // มี convert แล้ว
                //if (param.ExchangeGain != null || param.ExchangeLoss != null)
                //{
                //    if (doPaymentNew.PaymentAmountCurrencyType != CurrencyUtil.C_CURRENCY_LOCAL
                //        && item.KeyInWHTAmountCurrencyType != CurrencyUtil.C_CURRENCY_US)
                //    {
                //        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7129
                //            , "", new string[] { doPaymentNew.PaymentAmountCurrencyType, item.KeyInWHTAmountCurrencyType });
                //        return;
                //    }
                //}
                #endregion

                #region Unpaid amount : old code
                //decimal unpaidAmount = (doInvoice.InvoiceAmount ?? 0) + (doInvoice.VatAmount ?? 0)
                //                        - (doInvoice.PaidAmountIncVat ?? 0) - (doInvoice.RegisteredWHTAmount ?? 0);

                ////Check match payment amount not greater thant unpaid amount
                //if ((item.KeyInMatchAmountIncWHT ?? 0) > unpaidAmount)
                //{
                //    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                //       , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7037
                //       , item.KeyInMatchAmountIncWHT_ID ?? "", "gridHeaderMatchPaymentAmount", item.KeyInMatchAmountIncWHT_ID ?? "");
                //    return;
                //}

                //if ((item.KeyInMatchAmountIncWHT ?? 0) != unpaidAmount)
                //{
                //    //Auto transfer
                //    if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
                //    {
                //        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                //          , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7069
                //          , item.KeyInMatchAmountIncWHT_ID ?? "", "gridHeaderMatchPaymentAmount", item.KeyInMatchAmountIncWHT_ID ?? "");
                //        return;
                //    }

                //    //Edit By Patcharee T. (Refer to CRC 6.13 Payment type cheque split 3 Pcs for 1 Invoice )
                //    //if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE
                //    //    || doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
                //    //{
                //    //    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                //    //      , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7119
                //    //      , item.KeyInMatchAmountIncWHT_ID ?? "", "gridHeaderMatchPaymentAmount", item.KeyInMatchAmountIncWHT_ID ?? "");
                //    //    return;
                //    //}
                //    //Edit By Patcharee T. (Refer to CRC 6.13 Payment type cheque split 3 Pcs for 1 Invoice )
                //}
                #endregion
                // add by jirawat jannet on 2016-12-08
                #region Unpaid amount

                decimal InvoiceAmount = (doInvoice.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US ?
                                            doInvoice.InvoiceAmountUsd.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_US, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                                            : doInvoice.InvoiceAmount.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_LOCAL, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value);
                decimal VatAmount = (doInvoice.VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_US ?
                                             doInvoice.VatAmountUsd.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_US, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                                             : doInvoice.VatAmount.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_LOCAL, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value);
                decimal PaidAmountIncVat = (doInvoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US ?
                                            doInvoice.PaidAmountIncVatUsd.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_US, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                                            : doInvoice.PaidAmountIncVat.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_LOCAL, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value);
                decimal RegisteredWHTAmount = (doInvoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US ?
                                            doInvoice.RegisteredWHTAmountUsd.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_US, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                                            : doInvoice.RegisteredWHTAmount.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_LOCAL, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value);


                decimal unpaidAmount = InvoiceAmount + VatAmount - PaidAmountIncVat - RegisteredWHTAmount;

                //Check match payment amount not greater thant unpaid amount
                if (item.KeyInMatchAmountIncWHT.ConvertCurrencyPrice(item.KeyInMatchAmountIncWHTCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0) > unpaidAmount)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                       , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7037
                       , item.KeyInMatchAmountIncWHT_ID ?? "", "gridHeaderMatchPaymentAmount", item.KeyInMatchAmountIncWHT_ID ?? "");
                    return;
                }

                if (item.KeyInMatchAmountIncWHT.ConvertCurrencyPrice(item.KeyInMatchAmountIncWHTCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0) != unpaidAmount)
                {
                    //Auto transfer
                    if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                          , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7069
                          , item.KeyInMatchAmountIncWHT_ID ?? "", "gridHeaderMatchPaymentAmount", item.KeyInMatchAmountIncWHT_ID ?? "");
                        return;
                    }
                }

                #endregion
            }

            // add by jirawat jannet on 2016-11-01
            #region Validate new control on 2016-11-01

            if (param.ExchangeGain != null && param.ExchangeLoss != null)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                       , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7130
                       , "ExchangeLoss", "lbExchangeLoss", "ExchangeLoss");
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS084"
                       , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7130
                       , "ExchangeGain", "lbExchangeGain", "ExchangeGain");
                return;
            }

            #endregion

            #endregion

            if (isRegister)
            {
                // Comment by Jirawat Jannet
                #region Warning Message : old code
                //if (balanceAfterProcessing > 0)
                //{
                //    //show confirm message to user
                //    confirmMessageID.Add("MSG7038");
                //}

                ////Validate each row for warning message
                //bool isAlreadyAdd7039 = false;
                //bool isAlreadyAdd7040 = false;
                //foreach (var item in param.MatchInvoiceData)
                //{
                //    doInvoice doInvoice = billingHandler.GetInvoice(item.InvoiceNo);



                //    if (!isAlreadyAdd7039 && item.KeyInMatchAmountIncWHT.Value 
                //            < (doInvoice.InvoiceAmount ?? 0) 
                //                + (doInvoice.VatAmount ?? 0)
                //                - (doInvoice.PaidAmountIncVat ?? 0) 
                //                - (doInvoice.RegisteredWHTAmount ?? 0))
                //    {
                //        //show confirm message to user
                //        confirmMessageID.Add("MSG7039");
                //        isAlreadyAdd7039 = true;
                //    }
                //    if (!isAlreadyAdd7040 && (doInvoice.WHTAmount ?? 0) > 0 && (item.KeyInWHTAmount ?? 0) == 0)
                //    {
                //        //show confirm message to user
                //        confirmMessageID.Add("MSG7040");
                //        isAlreadyAdd7040 = true;
                //    }

                //    if (isAlreadyAdd7039 && isAlreadyAdd7040)
                //        break;  //No need more process
                //}
                #endregion

                #region Warning Message
                if (balanceAfterProcessing > 0)
                {
                    //show confirm message to user
                    confirmMessageID.Add("MSG7038");
                }

                //Validate each row for warning message
                bool isAlreadyAdd7039 = false;
                bool isAlreadyAdd7040 = false;
                foreach (var item in param.MatchInvoiceData)
                {
                    doInvoice doInvoice = billingHandler.GetInvoice(item.InvoiceNo);

                    decimal KeyInWHTAmount = item.KeyInWHTAmount.ConvertCurrencyPrice(item.KeyInWHTAmountCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                    decimal KeyInMatchAmountIncWHT = item.KeyInMatchAmountIncWHT.ConvertCurrencyPrice(item.KeyInMatchAmountIncWHTCurrencyType, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                    decimal InvoiceAmount = (doInvoice.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US ?
                                                doInvoice.InvoiceAmountUsd.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_US, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                                                : doInvoice.InvoiceAmount.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_LOCAL, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value);
                    decimal VatAmount = (doInvoice.VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_US ?
                                                 doInvoice.VatAmountUsd.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_US, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                                                 : doInvoice.VatAmount.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_LOCAL, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value);
                    decimal PaidAmountIncVat = (doInvoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US ?
                                                doInvoice.PaidAmountIncVatUsd.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_US, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                                                : doInvoice.PaidAmountIncVat.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_LOCAL, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value);
                    decimal RegisteredWHTAmount = (doInvoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US ?
                                                doInvoice.RegisteredWHTAmountUsd.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_US, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                                                : doInvoice.RegisteredWHTAmount.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_LOCAL, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value);
                    decimal WHTAmount = (doInvoice.WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US ?
                                                doInvoice.WHTAmountUsd.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_US, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value
                                                : doInvoice.WHTAmount.ConvertCurrencyPrice(CurrencyUtil.C_CURRENCY_LOCAL, param.FirstPaymentAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value);

                    if (!isAlreadyAdd7039 && KeyInMatchAmountIncWHT
                            < (InvoiceAmount + VatAmount - PaidAmountIncVat - RegisteredWHTAmount))
                    {
                        //show confirm message to user
                        confirmMessageID.Add("MSG7039");
                        isAlreadyAdd7039 = true;
                    }
                    if (!isAlreadyAdd7040 && WHTAmount > 0 && KeyInWHTAmount == 0)
                    {
                        //show confirm message to user
                        confirmMessageID.Add("MSG7040");
                        isAlreadyAdd7040 = true;
                    }

                    if (isAlreadyAdd7039 && isAlreadyAdd7040)
                        break;  //No need more process
                }
                #endregion
            }
        }
        #endregion
    }
}


// Comment เพื่อทำ spect ใหม่ ที่มีการแปลงสกุลเงิน
#region Old confirm function : Comment by Jirawat Jannet on 2016-12-08

//public ActionResult ICS084_cmdConfirm(ICS084_ScreenParameter param)
//{
//    ObjectResultData res = new ObjectResultData();
//    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
//    try
//    {
//        if (IsSuspend(res))
//            return Json(res);

//        if (!ICS010_IsAllowOperate(res))
//            return Json(res);

//        //Validate only Existing Payment List
//        ValidatorUtil validator = new ValidatorUtil();
//        decimal balanceAfterProcessing = 0;
//        string balanceAfterProcessingCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
//        List<string> confirmMessageID = new List<string>();
//        ICS084_ValidateAdjustInvoiceData(validator, param, false, out balanceAfterProcessing, out confirmMessageID, out balanceAfterProcessingCurrencyType);

//        ValidatorUtil.BuildErrorMessage(res, validator, null);
//        if (res.IsError)
//            return Json(res);

//        #region Prepare Detail data
//        List<doMatchPaymentDetail> matchDetails = new List<doMatchPaymentDetail>();
//        foreach (var item in param.MatchInvoiceData)
//        {
//            doUnpaidInvoice unpaidInvoice = ICS084_ScreenData.UnpaidInvoice.Where(d => d.InvoiceNo == item.InvoiceNo).First();
//            decimal matchAmountIncWht = (item.KeyInMatchAmountIncWHT.HasValue ? item.KeyInMatchAmountIncWHT.Value : 0);
//            decimal whtAmount = (item.KeyInWHTAmount.HasValue ? item.KeyInWHTAmount.Value : 0);
//            string matchStatus = string.Empty;
//            if (matchAmountIncWht < unpaidInvoice.UnpaidAmount)
//            {
//                matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL;
//            }
//            else if (unpaidInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID)
//            {
//                matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL_TO_FULL;
//            }
//            else
//            {
//                matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL;
//            }

//            // begin add by jirawat jannet on 2016-11-16
//            decimal? MatchAmountExcWHT = null, MatchAmountExcWHTUsd = null, MatchAmountIncWHT = null
//                , MatchAmountIncWHTUsd = null, WHTAmount = null, WHTAmountUsd = null;

//            if (item.KeyInMatchAmountIncWHTCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
//            {
//                MatchAmountExcWHT = matchAmountIncWht - whtAmount;
//                MatchAmountIncWHT = matchAmountIncWht;
//            }
//            else if (item.KeyInMatchAmountIncWHTCurrencyType == CurrencyUtil.C_CURRENCY_US)
//            {
//                MatchAmountExcWHTUsd = matchAmountIncWht - whtAmount;
//                MatchAmountIncWHTUsd = matchAmountIncWht;
//            }

//            if (item.KeyInWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
//                WHTAmount = whtAmount;
//            else if (item.KeyInWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
//                WHTAmountUsd = whtAmount;
//            // end add 

//            matchDetails.Add(new doMatchPaymentDetail()
//            {
//                MatchID = null,
//                InvoiceNo = item.InvoiceNo,
//                InvoiceOCC = unpaidInvoice.InvoiceOCC,
//                MatchAmountExcWHT = MatchAmountExcWHT ?? 0, //matchAmountIncWht - whtAmount, // edit by jirawat jannet on 2016-11-16
//                MatchAmountExcWHTUsd = MatchAmountExcWHTUsd ?? 0, // add by jirawat jannet on 2016-11-16
//                MatchAmountExcWHTCurrencyType = item.KeyInMatchAmountIncWHTCurrencyType,
//                MatchAmountIncWHT = MatchAmountIncWHT ?? 0, // add by jirawat jannet on 2016-11-16
//                MatchAmountIncWHTUsd = MatchAmountIncWHTUsd ?? 0, // add by jirawat jannet on 2016-11-16
//                MatchAmountIncWHTCurrencyType = item.KeyInMatchAmountIncWHTCurrencyType, // add by jirawat jannet on 2016-11-16
//                WHTAmount = WHTAmount,// add by jirawat jannet on 2016-11-16
//                WHTAmountUsd = WHTAmountUsd,// add by jirawat jannet on 2016-11-16
//                WHTAmountCurrencyType = item.KeyInWHTAmountCurrencyType,// add by jirawat jannet on 2016-11-16
//                MatchStatus = matchStatus,
//                CancelFlag = false,
//                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
//                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
//                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
//                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime

//            });
//        }
//        #endregion

//        #region Prepare Header data
//        decimal totalMatchAmount = 0;
//        string totalMatchAmountCurrencyType = string.Empty; // add by jirawat jannet on 2016-11-16
//        foreach (var item in param.MatchInvoiceData)
//        {
//            totalMatchAmount += (item.KeyInMatchAmountIncWHT ?? 0) - (item.KeyInWHTAmount ?? 0);
//            totalMatchAmountCurrencyType = item.KeyInMatchAmountIncWHTCurrencyType; // add by jirawat jannet on 2016-11-16
//        }

//        // begin add by jirawat jannet o 2016-11-16
//        decimal? TotalMatchAmount = null, TotalMatchAmountUsd = null
//            , OtherExpenseAmount = null, OtherExpenseAmountUsd = null
//            , OtherIncomeAmount = null, OtherIncomeAmountUsd = null
//            , BankFeeAmount = null, BankFeeAmountUsd = null;

//        if (totalMatchAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
//            TotalMatchAmount = totalMatchAmount;
//        else if (totalMatchAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
//            TotalMatchAmountUsd = totalMatchAmount;

//        if (param.OtherExpenseCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
//            OtherExpenseAmount = param.OtherExpense;
//        else if (param.OtherExpenseCurrencyType == CurrencyUtil.C_CURRENCY_US)
//            OtherExpenseAmountUsd = param.OtherExpense;

//        if (param.OtherIncomeCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
//            OtherIncomeAmount = param.OtherIncome;
//        else if (param.OtherIncomeCurrencyType == CurrencyUtil.C_CURRENCY_US)
//            OtherIncomeAmountUsd = param.OtherIncome;

//        if (param.BankFeeCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
//            BankFeeAmount = param.BankFee;
//        else if (param.BankFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
//            BankFeeAmountUsd = param.BankFee;
//        // end add

//        doMatchPaymentHeader matchHeader = new doMatchPaymentHeader()
//        {
//            MatchID = null,
//            MatchDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
//            PaymentTransNo = ICS084_ScreenData.PaymentTransNo,

//            TotalMatchAmount = TotalMatchAmount, // add by jirawat jannet on 2016-11-16
//            TotalMatchAmountUsd = TotalMatchAmountUsd, // add by jirawat jannet on 2016-11-16
//            TotalMatchAmountCurrencyType = totalMatchAmountCurrencyType,

//            BankFeeAmount = BankFeeAmount, //param.BankFee, // edit by jirawat janet on 2016-11-16
//            BankFeeAmountUsd = BankFeeAmountUsd, // add by jirawat jannet on 2016-11-16
//            BankFeeAmountCurrencyType = param.BankFeeCurrencyType,

//            SpecialProcessFlag = param.SpecialProcess,
//            ApproveNo = param.SpecialProcess ? param.ApproveNo : null,

//            OtherExpenseAmount = OtherExpenseAmount, //param.OtherExpense,  // edit by jirawat jannet on 2016-11-16
//            OtherExpenseAmountUsd = OtherExpenseAmountUsd, // add by jirawat jannet on 2016-11-16
//            OtherExpenseAmountCurrencyType = param.OtherExpenseCurrencyType,

//            OtherIncomeAmount = OtherIncomeAmount, // param.OtherIncome, // edit by jirawat jannet on 2016-11-16
//            OtherIncomeAmountUsd = OtherIncomeAmountUsd, // add by jirawat jannet on 2016-11-16
//            OtherIncomeAmountCurrencyType = param.OtherIncomeCurrencyType,

//            CancelFlag = false,
//            CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
//            CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
//            UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
//            UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
//            MatchPaymentDetail = matchDetails,

//            ExchangeGain = param.ExchangeGain,
//            ExchangeGainCurrencyType = param.ExchangeGainCurrencyType,

//            ExchangeLoss = param.ExchangeLoss,
//            ExchangeLossCurrencyType = param.ExchangeLossCurrencyType
//        };
//        #endregion

//        IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

//        //Modify by Jutarat A. on 13062013
//        //tbt_Payment payment = handler.GetPayment(ICS084_ScreenData.PaymentTransNo);
//        tbt_Payment payment = ICS084_ScreenData.PaymentData;
//        if (payment == null)
//            payment = handler.GetPayment(ICS084_ScreenData.PaymentTransNo);
//        //End Modify

//         using (TransactionScope scope = new TransactionScope())
//          {
//        try
//        {
//            bool isSuccess = handler.MatchPaymentInvoices(matchHeader, payment);
//            if (isSuccess)
//            {
//                scope.Complete();
//                //Result flag  1 = Success
//                res.ResultData = "1";
//                return Json(res);
//            }
//            else
//            {
//                scope.Dispose();
//                res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7066);
//                return Json(res);
//            }
//        }
//        catch (Exception ex)
//        {
//            //all Fail
//            scope.Dispose();
//            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
//            res.AddErrorMessage(ex);
//            return Json(res);
//        }
//        }
//    }
//    catch (Exception ex)
//    {
//        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
//        res.AddErrorMessage(ex);
//    }
//    return Json(res);
//}

#endregion