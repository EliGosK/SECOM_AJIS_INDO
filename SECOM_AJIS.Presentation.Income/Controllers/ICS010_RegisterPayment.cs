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
using SECOM_AJIS.Presentation.Common.Helpers;

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
        public ActionResult ICS010_Authority(ICS010_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                // Check permission
                if (!ICS010_IsAllowOperate(res))
                    return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<ICS010_ScreenParameter>("ICS010", param, res);
        }
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool ICS010_IsAllowOperate(ObjectResultData res)
        {
            //Check Permission 
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_PAYMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return false;
            }

            return true;
        }
        /// <summary>
        /// Check the status of system
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool IsSuspend(ObjectResultData res)
        {
            if (res == null)
            {
                res = new ObjectResultData();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            }

            //Is suspend ?
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            if (handlerCommon.IsSystemSuspending())
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return true;
            }

            return false;
        }
        #endregion

        #region Views
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS010")]
        public ActionResult ICS010()
        {
            ICS010_ScreenParameter param = GetScreenObject<ICS010_ScreenParameter>();
            return View();
        }
        #endregion

        #region Actions
        /// <summary>
        /// Generate xml for payment grid in register mode 
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS010_InitialPaymentGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS010", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// Generate xml for payment grid in confirm mode
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS010_InitialPaymentConfirmGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS010_Confirm", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Generate bank's branch comboitem list upon selected bank code 
        /// </summary>
        /// <param name="bankCode">bank code</param>
        /// <returns></returns>
        public ActionResult ICS010_GetBankBranch(string bankCode)
        {
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_BankBranch> list = handler.GetTbm_BankBranch(bankCode);

                foreach (var item in list)
                {
                    // Check user aplication launguage
                    if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        item.BankBranchName = item.BankBranchNameEN;
                    }
                    else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                    {
                        item.BankBranchName = item.BankBranchNameEN;
                    }
                    else
                    {
                        item.BankBranchName = item.BankBranchNameLC;
                    }
                }

                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    list = list.OrderBy(p => p.BankBranchName, StringComparer.Create(culture, false)).ToList();
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);     // Note BankName have only EN , LC then JP set to EN
                    list = list.OrderBy(p => p.BankBranchName, StringComparer.Create(culture, false)).ToList();
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                    list = list.OrderBy(p => p.BankBranchName, StringComparer.Create(culture, false)).ToList();
                }

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<tbm_BankBranch>(list, "BankBranchName", "BankBranchCode");

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
        /// Generate SECOM bank/branch comboitem list upon selected payment type
        /// </summary>
        /// <param name="paymentType">bayment type</param>
        /// <returns></returns>
        public ActionResult ICS010_GetSECOMAccount(string paymentType)
        {
            try
            {
                List<doSECOMAccount> doSECOMAccount = ICS010_GetSECOMAccountByPaymentType(paymentType);

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doSECOMAccount>(doSECOMAccount, "Text", "SecomAccountID");
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
        /// Generate receipt information of specified receipt no. 
        /// </summary>
        /// <param name="param">receipt no.</param>
        /// <returns></returns>
        public ActionResult ICS010_GetReceipt(ICS010_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            doReceipt resultData;
            try
            {
                ValidatorUtil validator = new ValidatorUtil();

                // Check required field.
                if (CommonUtil.IsNullOrEmpty(param) || string.IsNullOrEmpty(param.ReceiptNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                   , "RefAdvanceReceiptNo", "lblReceiptNo", "RefAdvanceReceiptNo");
                }
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);

                // Check duplicate ReceiptNo 
                ICS010_ScreenParameter screenObject = GetScreenObject<ICS010_ScreenParameter>();
                if (screenObject.PaymentList != null)
                {
                    foreach (var paymentData in screenObject.PaymentList)
                    {
                        if (!CommonUtil.IsNullOrEmpty(paymentData.RefAdvanceReceiptNo)
                            && param.ReceiptNo.Trim().Equals(paymentData.RefAdvanceReceiptNo.Trim()))
                        {
                            validator.AddErrorMessage( MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7002
                               , "RefAdvanceReceiptNo", param.ReceiptNo);
                            ValidatorUtil.BuildErrorMessage(res, validator, null);
                            if (res.IsError)
                                return Json(res);
                        }
                    }
                }

                //Get Receipt Data
                var handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                resultData = handler.GetReceipt(param.ReceiptNo);
                
                if (resultData == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7106
                        , "RefAdvanceReceiptNo", param.ReceiptNo);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    if (res.IsError)
                        return Json(res);
                }

                //Validate business
                ICS010_ValidateReceiptBusiness(resultData, validator);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);

                //Pass
                res.ResultData = resultData;
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
        /// Genereate xml of payment list for payment grid in register mode
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS010_GetPaymentList()
        {
            List<tbt_Payment> list = new List<tbt_Payment>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICS010_ScreenParameter param = GetScreenObject<ICS010_ScreenParameter>();
                list = param.PaymentList;
                if (list == null)
                    list = new List<tbt_Payment>();
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<tbt_Payment>(list, "Income\\ICS010", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            return Json(res);
        }
        /// <summary>
        /// Genereate xml of payment list for payment grid in confirm mode
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS010_GetPaymentListForConfirm()
        {
            List<tbt_Payment> list = new List<tbt_Payment>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            try
            {
                ICS010_ScreenParameter param = GetScreenObject<ICS010_ScreenParameter>();
                list = param.PaymentList;
                if (list == null)
                    list = new List<tbt_Payment>();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<tbt_Payment>(list, "Income\\ICS010_Confirm", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            return Json(res);
        }

        /// <summary>
        /// Add inputted payment information to payment list
        /// </summary>
        /// <param name="param">inputted payment information</param>
        /// <returns></returns>
        public ActionResult ICS010_AddPayment(ICS010_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS010_IsAllowOperate(res))
                    return Json(res);

                // Add by Jirawat Jannet @ 2016-10-19
                #region Init data by currency type 

                param.Payment.MatchableBalanceCurrencyType = param.Payment.PaymentAmountCurrencyType;
                if (param.Payment.MatchableBalanceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    param.Payment.MatchableBalanceUsd = param.Payment.PaymentAmount.ConvertTo<decimal>(false, 0);
                    param.Payment.MatchableBalance = 0;
                }
                else
                {
                    param.Payment.MatchableBalance = param.Payment.PaymentAmount.ConvertTo<decimal>(false, 0);
                    param.Payment.MatchableBalanceUsd = 0;
                }

                if (param.Payment.PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    param.Payment.PaymentAmountUsd = param.Payment.PaymentAmount;
                    param.Payment.PaymentAmount = null;
                }

                if (param.Payment.RefAdvanceReceiptAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    param.Payment.RefAdvanceReceiptAmountUsd = param.Payment.RefAdvanceReceiptAmount;
                    param.Payment.RefAdvanceReceiptAmount = null;
                }

                #endregion

                ICS010_ValidatePaymentBusiness(validator, param.Payment);

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);


                #region Add by Jirawat jannet change gridview data with currency @ 2016-09-28

                //foreach (var d in param.PaymentList)
                //{
                //    //var html1 = 
                //    //d.InvoiceReceiptGrid = 
                //}

                #endregion


                //Pass, Add to PaymentList
                ICS010_ScreenParameter screenSession = GetScreenObject<ICS010_ScreenParameter>();
                if (screenSession.PaymentList == null)
                    screenSession.PaymentList = new List<tbt_Payment>();

                ICS010_FillBankInfo(param.Payment);
                ICS010_FillPaymentType(param.Payment);
                param.Payment.PaymentDate = (DateTime)param.Payment.PaymentDateNull;
                param.Payment.MatchableBalance = param.Payment.PaymentAmount.ConvertTo<decimal>(false, 0);
                param.Payment.SystemMethod = PaymentSystemMethod.C_INC_PAYMENT_REGISTER;
                param.Payment.BankFeeRegisteredFlag = false;
                param.Payment.OtherIncomeRegisteredFlag = false;
                param.Payment.OtherExpenseRegisteredFlag = false;
                param.Payment.DeleteFlag = false;

                

                screenSession.PaymentList.Add(param.Payment);

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
        /// Remove payment information of specific row index of payment grid
        /// </summary>
        /// <param name="rowIndex">row index of payment grid</param>
        /// <returns></returns>
        public ActionResult ICS010_RemovePaymetList(int rowIndex)
        {
            List<tbt_Payment> list = new List<tbt_Payment>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICS010_ScreenParameter screenObject = GetScreenObject<ICS010_ScreenParameter>();
                list = screenObject.PaymentList;
                list.RemoveAt(rowIndex);

                //ResultFlag = Success;
                res.ResultData = "1";
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
            return Json(res);
        }

        /// <summary>
        /// Event when click register button 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS010_cmdRegister(ICS010_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS010_IsAllowOperate(res))
                    return Json(res);

                ICS010_ScreenParameter screenObject = GetScreenObject<ICS010_ScreenParameter>();

                //Validate only Existing Payment List
                ValidatorUtil validator = new ValidatorUtil();
                ICS010_ValidateIsExistingPaymentList(validator);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);

                // Validate business
                List<tbt_Payment> paymentList = screenObject.PaymentList;
                foreach (var item in paymentList)
                    ICS010_ValidatePaymentBusiness(validator, item);

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                //Have Error Message
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
        /// Event when click confirm button, this function will register the information of payment detail list to the system. The payment transaction no. will be generated for each registered payment information and displayed in result screen.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS010_cmdConfirm(ICS010_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;

            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS010_IsAllowOperate(res))
                    return Json(res);

                ICS010_ScreenParameter screenObject = GetScreenObject<ICS010_ScreenParameter>();

                //Validate only Existing Payment List
                ValidatorUtil validator = new ValidatorUtil();
                ICS010_ValidateIsExistingPaymentList(validator);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);

                // Validate business
                List<tbt_Payment> paymentList = screenObject.PaymentList;
                foreach (var item in paymentList)
                    ICS010_ValidatePaymentBusiness(validator, item);

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                //Have Error Message
                if (res.IsError)
                    return Json(res);

                //Save Data to db
                var handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                List<tbt_Payment> payments = new List<tbt_Payment>();
                foreach (var item in paymentList)
                {
                    payments.Add(item);
                }
                //foreach (var item in paymentList)
                //{
                //    item.MatchableBalanceCurrencyType = item.PaymentAmountCurrencyType;
                //    if (item.MatchableBalanceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                //    {
                //        item.MatchableBalanceUsd = item.PaymentAmount.ConvertTo<decimal>(false, 0);
                //        item.MatchableBalance = 0;
                //    }
                //    else
                //    {
                //        item.MatchableBalance = item.PaymentAmount.ConvertTo<decimal>(false, 0);
                //        item.MatchableBalanceUsd = 0;
                //    }

                //    if (item.PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                //    {
                //        item.PaymentAmountUsd = item.PaymentAmount;
                //        item.PaymentAmount = null;
                //    }

                //    if (item.RefAdvanceReceiptAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                //    {
                //        item.RefAdvanceReceiptAmountUsd = item.RefAdvanceReceiptAmount;
                //        item.RefAdvanceReceiptAmount = null;
                //    }

                //    payments.Add(item);
                //}

                //Save
                List<tbt_Payment> result = new List<tbt_Payment>();
                 using (TransactionScope scope = new TransactionScope())
                 {
                try
                {
                        result = handler.RegisterPayment(payments);
                        if (result != null && result.Count == payments.Count)
                        {
                        //Success
                        scope.Complete();

                        //Result flag 1 = success
                        res.ResultData = "1";
                            return Json(res);
                        }
                    }
                    catch (Exception ex)
                    {
                    //all Fail
                    scope.Dispose();
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7066);
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
        /// <summary>
        /// Clear payment list
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS010_cmdClearPayment()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICS010_ScreenParameter screenObject = GetScreenObject<ICS010_ScreenParameter>();
                if (screenObject != null && screenObject.PaymentList != null)
                {
                    screenObject.PaymentList.Clear();
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<tbt_Payment>(null, "Income\\ICS010", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            return Json(res);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Validate business of receipt information
        /// </summary>
        /// <param name="receipt"></param>
        /// <param name="validator"></param>
        private void ICS010_ValidateReceiptBusiness(doReceipt receipt, ValidatorUtil validator)
        {
            if (receipt == null)
                return;
            if (validator == null)
                validator = new ValidatorUtil();

            if (receipt.AdvanceReceiptStatus.Equals(AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_NOT))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7004
                    , "RefAdvanceReceiptNo", new string[] { receipt.ReceiptNo });
            }
            else if (receipt.AdvanceReceiptStatus.Equals(AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_REGISTERED)
                || receipt.AdvanceReceiptStatus.Equals(AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_PAID))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7005
                    , "RefAdvanceReceiptNo", new string[] { receipt.ReceiptNo });
            }
        }
        /// <summary>
        /// Validate payment list
        /// </summary>
        /// <param name="validator"></param>
        private void ICS010_ValidateIsExistingPaymentList(ValidatorUtil validator)
        {
            ICS010_ScreenParameter session = GetScreenObject<ICS010_ScreenParameter>();
            if (CommonUtil.IsNullOrEmpty(session) || CommonUtil.IsNullOrEmpty(session.PaymentList)
                || session.PaymentList.Count == 0)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                     , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7071
                     , "Payment");
            }
        }
        /// <summary>
        /// Validate business of payment information 
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="payment"></param>
        private void ICS010_ValidatePaymentBusiness(ValidatorUtil validator, tbt_Payment payment)
        {
            ICS010_ScreenParameter screenObject = GetScreenObject<ICS010_ScreenParameter>();
            bool isHaveErrorRefAdvanceReceiptNo = false;

            // Check required field by Payment Type
            if (CommonUtil.IsNullOrEmpty(payment) || CommonUtil.IsNullOrEmpty(payment.PaymentType))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                   , "PaymentType", "lblPaymentType", "PaymentType");
            }

            //Validate validate require field
            if (CommonUtil.IsNullOrEmpty(payment.SECOMAccountID))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                   , "SECOMAccountID", "lblSECOMBank", "SECOMAccountID");
            }
            if (payment.PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL 
                && (CommonUtil.IsNullOrEmpty(payment.PaymentAmount) || payment.PaymentAmount == 0))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                  , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                  , "PaymentAmount", "lblPaymentAmount", "PaymentAmount");
            }
            else if (payment.PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_US
                && (CommonUtil.IsNullOrEmpty(payment.PaymentAmountUsd) || payment.PaymentAmountUsd == 0))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                  , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                  , "PaymentAmount", "lblPaymentAmount", "PaymentAmount");
            }
            if (!payment.PaymentDateNull.HasValue)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                  , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                  , "PaymentDateNull", "lblPaymentDate", "PaymentDateNull");
            }
            else if (payment.PaymentDateNull.Value.Date > DateTime.Now.Date)
            {
                //Payment date must not be future date.
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                                , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7076
                                , "PaymentDateNull", "lblPaymentDate", "PaymentDateNull");
            }

            //Validate only promissory note
            if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE || payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
            {
                if (CommonUtil.IsNullOrEmpty(payment.DocNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                       , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                       , "PromissoryNoteNo", "lblPromissoryNoteNo", "PromissoryNoteNo");
                }
                if (payment.DocDate.HasValue == false)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                       , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                       , "PromissoryNoteDate", "lblPromissoryNoteDate", "PromissoryNoteDate");
                }

                if (CommonUtil.IsNullOrEmpty(payment.SendingBankCode))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                       , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                       , "SendingBankCode", "lblSendingBank", "SendingBankCode");
                }
                if (CommonUtil.IsNullOrEmpty(payment.SendingBranchCode))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                       , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                       , "SendingBranchCode", "lblSendingBranch", "SendingBranchCode");
                }
            }


            if (!string.IsNullOrEmpty(payment.PayerBankAccNo) && payment.PayerBankAccNo.Length != 20)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                 , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7077
                 , "");
            }

            //Comment by Jutarat A. on 01022013
            //if (!CommonUtil.IsNullOrEmpty(payment.PaymentType) && !(payment.PaymentType.Equals(PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER)
            //    || payment.PaymentType.Equals(PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)))
            //{
            //    // Ext validate require field for not bank transfer
            //    if (CommonUtil.IsNullOrEmpty(payment.RefAdvanceReceiptNo))
            //    {
            //        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
            //          , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
            //          , "RefAdvanceReceiptNo", "lblReceiptNo", "RefAdvanceReceiptNo");
            //        isHaveErrorRefAdvanceReceiptNo = true;
            //    }

            //    if (CommonUtil.IsNullOrEmpty(payment.RefAdvanceReceiptAmount))
            //    {
            //        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
            //          , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
            //          , "RefAdvanceReceiptAmount", "lblReceiptAmount", "RefAdvanceReceiptAmount");
            //        isHaveErrorRefAdvanceReceiptNo = true;
            //    }
            //    if (CommonUtil.IsNullOrEmpty(payment.RefInvoiceNo))
            //    {
            //        validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
            //          , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
            //          , "RefInvoiceNo", "lblInvoiceNO", "RefInvoiceNo");
            //    }

            //    if (!isHaveErrorRefAdvanceReceiptNo)
            //    {
            //        var handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            //        var receiptData = handler.GetReceipt(payment.RefAdvanceReceiptNo);
            //        if (receiptData == null)
            //        {
            //            validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7106
            //              , "RefAdvanceReceiptNo", payment.RefAdvanceReceiptNo);
            //            isHaveErrorRefAdvanceReceiptNo = true;
            //        }

            //        if (!isHaveErrorRefAdvanceReceiptNo)
            //            ICS010_ValidateReceiptBusiness(receiptData, validator);
            //    }
            //}

            //Add by Jutarat A. on 01022013
            if (CommonUtil.IsNullOrEmpty(payment.RefAdvanceReceiptAmount) == false)
            {
                var handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var receiptData = handler.GetReceipt(payment.RefAdvanceReceiptNo);
                if (receiptData == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7106
                      , "RefAdvanceReceiptNo", payment.RefAdvanceReceiptNo);
                    isHaveErrorRefAdvanceReceiptNo = true;
                }

                if (!isHaveErrorRefAdvanceReceiptNo)
                    ICS010_ValidateReceiptBusiness(receiptData, validator);
            }
            if (CommonUtil.IsNullOrEmpty(payment.MatchRGroupName))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS010"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                   , "GroupNameInput", "lblGroupname", "GroupNameInput");
                  
            }
            //End Add
        }

        /// <summary>
        /// Get SECOM bank account by specific payment type
        /// </summary>
        /// <param name="paymentType"></param>
        /// <returns></returns>
        private List<doSECOMAccount> ICS010_GetSECOMAccountByPaymentType(string paymentType)
        { 
            IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            List<doSECOMAccount> doSECOMAccount = null;

            if (paymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
            {
                //return auto transfer bank
                doSECOMAccount = masterHandler.GetSECOMAccountAutoTransfer();
            }
            else if (paymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE
                || paymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
            { 
                //return dummy bank
                doSECOMAccount = masterHandler.GetSECOMAccountDummyTransfer();
            }
            else if (string.IsNullOrEmpty(paymentType))
            {
                //no bank 
                doSECOMAccount = new List<doSECOMAccount>();
            }
            else    
            {
                //return bank transfer bank
                doSECOMAccount = masterHandler.GetSECOMAccountBankTransfer();
            }
            return doSECOMAccount;
        }

        /// <summary>
        /// Mapping language for SECOM bank information, sending bank information of specific payment information 
        /// </summary>
        /// <param name="payment"></param>
        private void ICS010_FillBankInfo(tbt_Payment payment)
        {
            List<tbt_Payment> paymentList = new List<tbt_Payment>();
            paymentList.Add(payment);
            ICS010_FillBankInfo(paymentList);
        }
        /// <summary>
        /// Mapping language for SECOM bank information, sending bank information of payment List
        /// </summary>
        /// <param name="payments"></param>
        private void ICS010_FillBankInfo(List<tbt_Payment> payments)
        {
            var masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            var incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

            List<doSECOMAccount> secomAccount = masterHandler.GetSECOMAccount();
            List<tbm_Bank> tbmBank = masterHandler.GetTbm_Bank();
            CommonUtil.MappingObjectLanguage<tbm_Bank>(tbmBank);

            foreach (var payment in payments)
            {
                int? secombankID = payment.SECOMAccountID;
                if (secombankID.HasValue == true)
                {
                    doSECOMAccount secomBankinfo = secomAccount.Where(b => b.SecomAccountID == secombankID).FirstOrDefault();
                    tbm_Bank sendingBank = tbmBank.Where(d => d.BankCode == payment.SendingBankCode).FirstOrDefault();
                    tbm_BankBranch sendingBranch = masterHandler.GetTbm_BankBranch(payment.SendingBankCode).Where(d => d.BankBranchCode == payment.SendingBranchCode).FirstOrDefault();

                    #region Mapping Language
                    if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        if (secomBankinfo != null)
                        {
                            payment.SECOMBankNameEN = secomBankinfo.BankNameEN;
                            payment.SECOMBranchNameEN = secomBankinfo.BankBranchNameEN;
                        }
                        if (sendingBank != null)
                        {
                            payment.SendingBankNameEN = sendingBank.BankNameEN;
                        }
                        if (sendingBranch != null)
                        {
                            payment.SendingBranchNameEN = sendingBranch.BankBranchNameEN;
                        }
                    }
                    else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                    {
                        if (secomBankinfo != null)
                        {
                            //payment.SECOMBankNameEN <--- secomBankinfo.BankNameJP
                            payment.SECOMBankNameEN = secomBankinfo.BankNameJP;
                            payment.SECOMBranchNameEN = secomBankinfo.BankBranchNameJP;
                        }
                        if (sendingBank != null)
                        {
                            payment.SendingBankNameEN = sendingBank.BankNameEN;
                        }
                        if (sendingBranch != null)
                        {
                            payment.SendingBranchNameEN = sendingBranch.BankBranchNameEN;
                        }
                    }
                    else
                    {
                        if (secomBankinfo != null)
                        {
                            payment.SECOMBankNameLC = secomBankinfo.BankNameLC;
                            payment.SECOMBranchNameLC = secomBankinfo.BankBranchNameLC;
                        }
                        if (sendingBank != null)
                        {
                            payment.SendingBankNameLC = sendingBank.BankNameLC;
                        }
                        if (sendingBranch != null)
                        {
                            payment.SendingBranchNameLC = sendingBranch.BankBranchNameLC;
                        }
                    }
                    #endregion

                    payment.SECOMBankFullName = secomBankinfo.Text;
                    payment.SendingBankFullName = payment.SendingBankName + (string.IsNullOrEmpty(payment.SendingBranchName) ? "" : " /" + payment.SendingBranchName);
                }
            }
        }

        /// <summary>
        /// Mapping description for payment type of specific payment information
        /// </summary>
        /// <param name="doPayment">payment information</param>
        private void ICS010_FillPaymentType(tbt_Payment doPayment)
        {
            if (doPayment != null && !(string.IsNullOrEmpty(doPayment.PaymentType)))
            {
                List<string> listFieldName = new List<string>();
                listFieldName.Add(MiscType.C_PAYMENT_TYPE);
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> listMisc = commonHandler.GetMiscTypeCodeListByFieldName(listFieldName).Where(d => d.ValueCode == doPayment.PaymentType).ToList(); // This result has language mapping already
                if (listMisc != null && listMisc.Count > 0)
                {
                    doPayment.PaymentTypeDisplay = listMisc[0].ValueDisplay;
                }
            }
        }
        /// <summary>
        /// Mapping description for payment type of payment information List
        /// </summary>
        /// <param name="doPayments">payment information list</param>
        private void ICS010_FillPaymentType(List<tbt_Payment> doPayments)
        {
            if (doPayments != null)
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<string> listFieldName = new List<string>();
                listFieldName.Add(MiscType.C_PAYMENT_TYPE);
                List<doMiscTypeCode> listMisc = commonHandler.GetMiscTypeCodeListByFieldName(listFieldName);

                foreach (var doPayment in doPayments)
                {
                    if (!string.IsNullOrEmpty(doPayment.PaymentType))
                    {
                        List<doMiscTypeCode> miscs = listMisc.Where(d => d.ValueCode == doPayment.PaymentType).ToList(); // This result has language mapping already
                        if (listMisc != null && listMisc.Count > 0)
                            doPayment.PaymentTypeDisplay = miscs[0].ValueDisplay;
                    }
                }
            }
        }
        #endregion  
    }
}