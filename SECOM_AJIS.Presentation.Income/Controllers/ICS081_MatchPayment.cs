using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Income.Models;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Common;
using System.Linq;
using SECOM_AJIS.DataEntity.Master;

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
        public ActionResult ICS081_Authority(ICS081_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS081_IsAllowOperate(res))
                    return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return InitialScreenEnvironment<ICS081_ScreenParameter>("ICS081", param, res);
        }
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool ICS081_IsAllowOperate(ObjectResultData res)
        {
            //Check Permission 
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_MATCH_PAYMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
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
        public ActionResult ICS081()
        {
            if (!CommonUtil.IsNullOrEmpty(ICS081_ScreenData.PaymentTransNo))
            {
                IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                doPayment payment = handler.SearchPayment(new doPaymentSearchCriteria() { PaymentTransNo = ICS081_ScreenData.PaymentTransNo }).FirstOrDefault();
                if (payment != null)
                {
                    ViewBag.PaymentTransNo = payment.PaymentTransNo;
                    //ViewBag.MatchableBalance = payment.MatchableBalance.ToString("#,##0.00"); // Comment by Jirawat Jannet @ 2016-10-25
                    // add by Jirawat Jannet @ 2016-10-25
                    if (payment.MatchableBalanceCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        ViewBag.MatchableBalance = payment.MatchableBalance.ToString("#,##0.00");
                    else
                        ViewBag.MatchableBalance = payment.MatchableBalanceUsd.ToString("#,##0.00");
                    ViewBag.MatchableBalanceCurrencyType = payment.MatchableBalanceCurrencyType;

                    //ViewBag.FirstPaymentAmount = payment.PaymentAmount?.ToString("#,##0.00"); // Comment by Jirawat Jannet @ 2016-10-25
                    // add by Jirawat Jannet @ 2016-10-25
                    if (payment.PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        ViewBag.FirstPaymentAmount = payment.PaymentAmount?.ToString("#,##0.00");
                    else
                        ViewBag.FirstPaymentAmount = payment.PaymentAmountUsd?.ToString("#,##0.00");
                    ViewBag.FirstPaymentAmountCurrencyType = payment.PaymentAmountCurrencyType;


                    ViewBag.SECOMBankBranch = payment.SECOMBankFullName;
                    ViewBag.PaymentDate = payment.PaymentDate.ToString("dd-MMM-yyyy");
                    ViewBag.PayerName = payment.Payer;
                    ViewBag.Memo = payment.Memo;
                }
            }
            return View("_ICS081");
        }
        #endregion

        #region Actions
        /// <summary>
        /// Generate xml for initial billing detail grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS081_InitialResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS081", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Set specific payment transaction no. to screen session
        /// </summary>
        /// <param name="paymentTranNo">payment transaction no.</param>
        /// <returns></returns>
        public ActionResult ICS081_SetPaymentTransNo(string paymentTranNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                if (CommonUtil.IsNullOrEmpty(ICS081_ScreenData))
                {
                    ICS081_ScreenData = new ICS081_ScreenParameter();
                }
                ICS081_ScreenData.PaymentTransNo = paymentTranNo;

                //ResultFlag = Success;
                res.ResultData = "1";
                return Json(true);
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
        /// Retrieve billing detail list of specific billing target code
        /// </summary>
        /// <param name="billingTargetCode">billing target code</param>
        /// <returns></returns>
        public ActionResult ICS081_SearchBillingbyBillingTargetCode(string billingTargetCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();

            try
            {
                //Validate business
                if (CommonUtil.IsNullOrEmpty(billingTargetCode) || billingTargetCode.Equals("-"))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                      , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                      , "BillingTargetCode1", "lblBillingTargetCode", "BillingTargetCode1");

                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                      , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                      , "BillingTargetCode2", "lblBillingTargetCode", "BillingTargetCode2");

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    return Json(res);
                }

                //Convert short id format to long id format
                CommonUtil c = new CommonUtil();
                string longFormat = c.ConvertBillingTargetCode(billingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);


                //Get billing target
                var billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                var billingTarget = billingHandler.GetTbt_BillingTarget(longFormat, null, null);
                if (billingTarget == null || billingTarget.Count == 0)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                   , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                   , "BillingTargetCode1", "lblBillingTargetCode", "BillingTargetCode1");

                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                      , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                      , "BillingTargetCode2", "lblBillingTargetCode", "BillingTargetCode2");

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Get unpaidbilling, return result
                var incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                List<doGetUnpaidBillingTargetByCodeWithExchange> unpaidList = incomeHandler.GetUnpaidBillingTargetByCodeWithExchange(longFormat);
                ICS081_ValidateDisplayBillingTargetSearchResult(res, unpaidList, true);
                res.ResultData = CommonUtil.ConvertToXml<doGetUnpaidBillingTargetByCodeWithExchange>(unpaidList, "Income\\ICS081", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Retrieve billing detail list of specific customer code.
        /// </summary>
        /// <param name="custCode">Customer Code</param>
        /// <returns></returns>
        public ActionResult ICS081_SearchBillingbyCustomerCode(string customerCode) //Add by Jutarat A. on 09042013
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                //Validate business
                if (CommonUtil.IsNullOrEmpty(customerCode))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                      , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                      , "ics081CutomerCode", "lblCustomerCode", "ics081CutomerCode");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    return Json(res);
                }

                //Get invoice
                CommonUtil c = new CommonUtil();
                string longCustomerCode = c.ConvertCustCode(customerCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                var masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                var doCustomer = masterHandler.GetTbm_Customer(longCustomerCode);
                if (doCustomer == null || doCustomer.Count == 0)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                          , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                          , "ics081CutomerCode", "lblCustomerCode", "ics081CutomerCode");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Get unpaidbilling, retun result
                var incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                List<doGetUnpaidBillingTargetByCodeWithExchange> unpaidList = incomeHandler.GetUnpaidBillingTargetByCustomerCodeWithExchange(longCustomerCode);
                ICS081_ValidateDisplayBillingTargetSearchResult(res, unpaidList, true);
                res.ResultData = CommonUtil.ConvertToXml<doGetUnpaidBillingTargetByCodeWithExchange>(unpaidList, "Income\\ICS081", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Retrieve billing detail list of specific invoice no.
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <returns></returns>
        public ActionResult ICS081_SearchBillingbyInvoiceNo(string invoiceNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                //Validate business
                if (CommonUtil.IsNullOrEmpty(invoiceNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                      , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                      , "ics081InvoiceNo", "lblInvoiceNo", "ics081InvoiceNo");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    return Json(res);
                }

                //Get invoice
                var billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                var doInvoice = billingHandler.GetInvoice(invoiceNo);
                if (doInvoice == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                          , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                          , "ics081InvoiceNo", "lblInvoiceNo", "ics081InvoiceNo");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Get unpaidbilling, retun result
                var incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                List<doGetUnpaidBillingTargetByCodeWithExchange> unpaidList = incomeHandler.GetUnpaidBillingTargetByInvoiceNoWithExchange(invoiceNo);
                ICS081_ValidateDisplayBillingTargetSearchResult(res, unpaidList, true);
                res.ResultData = CommonUtil.ConvertToXml<doGetUnpaidBillingTargetByCodeWithExchange>(unpaidList, "Income\\ICS081", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Retrieve billing detail list of specific billing code
        /// </summary>
        /// <param name="billingCode">billing code</param>
        /// <returns></returns>
        public ActionResult ICS081_SearchBillingbyBillingCode(string billingCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                //Validate business
                if (CommonUtil.IsNullOrEmpty(billingCode) || billingCode.Equals("-"))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                      , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                      , "BillingCode1", "lblBillingCode", "BillingCode1");

                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                     , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                     , "BillingCode2", "lblBillingCode", "BillingCode2");

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    return Json(res);
                }

                //Convert short id format to long id format
                CommonUtil c = new CommonUtil();
                string longFormat = c.ConvertBillingCode(billingCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                //Get billing basic by long format
                var billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                string contactCode = longFormat.Substring(0, longFormat.IndexOf('-'));
                string billingOCC = longFormat.Substring(longFormat.IndexOf('-') + 1);
                var billingBasic = billingHandler.GetTbt_BillingBasic(contactCode, billingOCC);
                if (billingBasic == null || billingBasic.Count == 0)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                          , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                          , "BillingCode1", "lblBillingCode", "BillingCode1");

                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                          , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                          , "BillingCode2", "lblBillingCode", "BillingCode2");

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Get unpaidbilling, return result
                var handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                List<doGetUnpaidBillingTargetByCodeWithExchange> unpaidList = handler.GetUnpaidBillingTargetByBillingCodeWithExchange(longFormat);
                ICS081_ValidateDisplayBillingTargetSearchResult(res, unpaidList, true);
                res.ResultData = CommonUtil.ConvertToXml<doGetUnpaidBillingTargetByCodeWithExchange>(unpaidList, "Income\\ICS081", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Retrieve billing detail list of specific receipt no.
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        public ActionResult ICS081_SearchBillingbyReceiptNo(string receiptNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                //Validate business
                if (CommonUtil.IsNullOrEmpty(receiptNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                      , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                      , "ics081ReceiptNo", "lblReceiptNo", "ics081ReceiptNo");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    return Json(res);
                }

                var handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                //Get receipt
                doReceipt doReceipt = handler.GetReceipt(receiptNo);
                if (doReceipt == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                      , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                      , "ics081ReceiptNo", "lblReceiptNo", "ics081ReceiptNo");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Get unpaid, return result
                List<doGetUnpaidBillingTargetByCodeWithExchange> unpaidList = handler.GetUnpaidBillingTargetByReceiptNoWithExchange(receiptNo);
                ICS081_ValidateDisplayBillingTargetSearchResult(res, unpaidList, true);
                res.ResultData = CommonUtil.ConvertToXml<doGetUnpaidBillingTargetByCodeWithExchange>(unpaidList, "Income\\ICS081", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Retrieve billing detail list of inputted advance search criteria.
        /// </summary>
        /// <param name="doSearch">advance search criteria information</param>
        /// <returns></returns>
        public ActionResult ICS081_SearchBillingByCriteria(ICS081_UnpaidBillingTargetSearchCriteria doSearch)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                //Validate Model
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { doSearch });
                if (res.IsError)
                    return Json(res);

                //Validate Business
                ValidatorUtil validator = new ValidatorUtil();
                ICS081_ValidateSearchBillingByCriteria(validator, doSearch);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);

                //Get Data
                IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                List<doGetUnpaidBillingTargetByCodeWithExchange> unpaidList = handler.SearchUnpaidBillingTargetWithExchange(doSearch);
                ICS081_ValidateDisplayBillingTargetSearchResult(res, unpaidList, true);
                res.ResultData = CommonUtil.ConvertToXml<doGetUnpaidBillingTargetByCodeWithExchange>(unpaidList, "Income\\ICS081", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Retrieve next screen of specific payment transaction no. and invoice no.
        /// </summary>
        /// <param name="PaymentTransNo">payment transaction no.</param>
        /// <param name="InvoiceNo">invoice no.</param>
        /// <returns></returns>
        public ActionResult ICS081_MatchPaymentNextStep(string PaymentTransNo, string InvoiceNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (IsSuspend(res))
                    return Json(res);

                res.ResultData = "ICS084";
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
        /// Screen session
        /// </summary>
        private ICS081_ScreenParameter ICS081_ScreenData
        {
            get
            {
                return GetScreenObject<ICS081_ScreenParameter>("ics081");
            }
            set
            {
                UpdateScreenObject(value,"ics081");
            }
        }
        /// <summary>
        /// Validate business for billing search result
        /// </summary>
        /// <param name="res"></param>
        /// <param name="unpaidBillingTargets">billing search result</param>
        /// <param name="isSearchByCode">is search by specific code</param>
        private void ICS081_ValidateDisplayBillingTargetSearchResult(ObjectResultData res, List<doGetUnpaidBillingTargetByCodeWithExchange> unpaidBillingTargets, bool isSearchByCode)
        {
            if (unpaidBillingTargets == null || unpaidBillingTargets.Count == 0)
            {
                if (isSearchByCode)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7101);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return;
                }
                //else {} Do nothing for search criteria method
            }
            //else if (unpaidBillingTargets.Count > 1000)
            //{
            //    //Don't show result, but show message
            //    unpaidBillingTargets.Clear();
            //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0052, new string[] { "1000" });
            //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            //    return;
            //}
        }
        /// <summary>
        /// Validate business for advance search criteria
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="doSearch">advance search criteria</param>
        private void ICS081_ValidateSearchBillingByCriteria(ValidatorUtil validator, doUnpaidBillingTargetSearchCriteria doSearch)
        {
            if (doSearch.IssueInvoiceDateFrom.HasValue && doSearch.IssueInvoiceDateTo.HasValue
                && doSearch.IssueInvoiceDateFrom > doSearch.IssueInvoiceDateTo)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                    , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7034
                    , "IssueInvoiceDateFrom", "lblIssueInvoiceDate", "IssueInvoiceDateFrom");
            }
            if (doSearch.ExpectedPaymentDateFrom.HasValue && doSearch.ExpectedPaymentDateTo.HasValue
                && doSearch.ExpectedPaymentDateFrom > doSearch.ExpectedPaymentDateTo)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                    , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7034
                    , "ExpectedPaymentDateFrom", "lblExpectedPaymentDate", "ExpectedPaymentDateFrom");
            }
            if (doSearch.InvoiceAmountFrom.HasValue && doSearch.InvoiceAmountTo.HasValue
                && doSearch.InvoiceAmountFrom > doSearch.InvoiceAmountTo)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                    , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7034
                    , "InvoiceAmountFrom", "lblInvoiceAmount", "InvoiceAmountFrom");
            }
            if (doSearch.BillingDetailAmountFrom.HasValue && doSearch.BillingDetailAmountTo.HasValue
                && doSearch.BillingDetailAmountFrom > doSearch.BillingDetailAmountTo)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                    , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7034
                    , "BillingDetailAmountFrom", "lblBillingDetailAmount", "BillingDetailAmountFrom");
            }
            if (doSearch.LastPaymentDayFrom.HasValue && doSearch.LastPaymentDayTo.HasValue
                && doSearch.LastPaymentDayFrom > doSearch.LastPaymentDayTo)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS081"
                    , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7034
                    , "LastPaymentDayFrom", "lblLastPaymentDate", "LastPaymentDayFrom");
            }
        }
        #endregion
    }
}