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
        public ActionResult ICS040_Authority(ICS040_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                // Check permission
                if (!ICS040_IsAllowOperate(res))
                    return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<ICS040_ScreenParameter>("ICS040", param, res);
        }
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool ICS040_IsAllowOperate(ObjectResultData res)
        {
            //Check Permission 
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_BILLING_EXEMPTION, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
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
        [Initialize("ICS040")]
        public ActionResult ICS040()
        {
            ICS040_ScreenParameter param = GetScreenObject<ICS040_ScreenParameter>();
            return View();
        }
        #endregion

        #region Actions
        /// <summary>
        /// Generate xml for invoice grid in register mode 
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS040_InitialInvoiceGrid()
        {
            return Json(CommonUtil.ConvertToXml<doInvoice>(null, "Income\\ICS040", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// Generate xml for invoice grid in confirm mode
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS040_InitialInvoiceConfirmGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS040_Confirm", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Add inputted invoice information to invoice list
        /// </summary>
        /// <param name="param">invoice information</param>
        /// <returns></returns>
        public ActionResult ICS040_AddInvoice(ICS040_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS040_IsAllowOperate(res))
                    return Json(res);

                //Check required field.
                if (CommonUtil.IsNullOrEmpty(param) || string.IsNullOrEmpty(param.InvoiceNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS040"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                   , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                }
                if (CommonUtil.IsNullOrEmpty(param) || string.IsNullOrEmpty(param.ApproveNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS040"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                   , "ApproveNo", "lblApproveNo", "ApproveNo");
                }
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);


                //Get Invoice Data
                doInvoice doInvoice = ICS040_GetInvoice(param.InvoiceNo, res);
                if (res.IsError)
                    return Json(res);

                //Validate  business
                ICS040_ValidateInvoiceBusiness(doInvoice, res);
                if (res.IsError)
                    return Json(res);


                //Check duplicated 
                ICS040_ScreenParameter screenObject = GetScreenObject<ICS040_ScreenParameter>();
                if (screenObject.InvoiceList != null)
                {
                    foreach (var item in screenObject.InvoiceList)
                    {
                        if (!CommonUtil.IsNullOrEmpty(item.InvoiceNo)
                            && param.InvoiceNo.Trim().Equals(item.InvoiceNo.Trim()))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7070
                               , "lblInovoiceNo", item.InvoiceNo);

                            ValidatorUtil.BuildErrorMessage(res, validator, null);
                            if (res.IsError)
                                return Json(res);
                        }
                    }
                }

                //Set exempt approve no
                doInvoice.ExemptApproveNo = param.ApproveNo;

                ICS040_ScreenParameter screenSession = GetScreenObject<ICS040_ScreenParameter>();
                if (screenSession.InvoiceList == null)
                    screenSession.InvoiceList = new List<doInvoice>();
                screenSession.InvoiceList.Add(doInvoice);

                res.ResultData = CommonUtil.ConvertToXml<doInvoice>(screenSession.InvoiceList, "Income\\ICS040", CommonUtil.GRID_EMPTY_TYPE.INSERT);
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
        /// Remove invoice information of specific row index of invoice grid
        /// </summary>
        /// <param name="rowIndex">row index of invoic grid</param>
        /// <returns></returns>
        public ActionResult ICS040_RemoveInvoice(int rowIndex)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICS040_ScreenParameter screenObject = GetScreenObject<ICS040_ScreenParameter>();
                List<doInvoice> list = screenObject.InvoiceList;
                list.RemoveAt(rowIndex);

                res.ResultData = CommonUtil.ConvertToXml<doInvoice>(list, "Income\\ICS040", CommonUtil.GRID_EMPTY_TYPE.INSERT);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Genereate xml of invoice list for invoice grid in register mode
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS040_GetInvoice(ICS040_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            doInvoice doInvoice;
            try
            {
                //Check required field.
                ValidatorUtil validator = new ValidatorUtil();
                if (CommonUtil.IsNullOrEmpty(param) || string.IsNullOrEmpty(param.InvoiceNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS040"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007
                   , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
                }
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return Json(res);


                //Get Invoice Data
                doInvoice = ICS040_GetInvoice(param.InvoiceNo, res);
                if (res.IsError)
                    return Json(res);

                //Validate  business
                ICS040_ValidateInvoiceBusiness(doInvoice, res);
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
        /// Genereate xml of invoice list for payment grid in confirm mode
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS040_GetInvoiceForConfirm()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            List<doInvoice> list = new List<doInvoice>();
            try
            {
                ICS040_ScreenParameter param = GetScreenObject<ICS040_ScreenParameter>();
                if (param != null && param.InvoiceList != null)
                    list = param.InvoiceList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<doInvoice>(list, "Income\\ICS040_Confirm", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            return Json(res);
        }

        /// <summary>
        /// Event when click register button 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS040_cmdRegister(ICS040_ScreenParameter param)
        {
            ICS040_ScreenParameter session = GetScreenObject<ICS040_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS040_IsAllowOperate(res))
                    return Json(res);

                //Validate only Existing invoice List
                ValidatorUtil validator = new ValidatorUtil();
                if (CommonUtil.IsNullOrEmpty(session)
                    || CommonUtil.IsNullOrEmpty(session.InvoiceList)
                    || session.InvoiceList.Count == 0)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS040"
                         , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7071
                         , "Invoice");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    return Json(res);
                }

                //Validate  business, (Already confirmed with sa, need validate at this point)
                ICS040_ScreenParameter screenObject = GetScreenObject<ICS040_ScreenParameter>();
                List<doInvoice> doInvoiceDb = new List<doInvoice>();
                foreach (var item in screenObject.InvoiceList)
                {
                    //Get Invoice Data
                    doInvoice doInvoice = ICS040_GetInvoice(item.InvoiceNo, res);
                    if (res.IsError)
                        return Json(res);

                    //Validate  business
                    ICS040_ValidateInvoiceBusiness(doInvoice, res);
                    if (res.IsError)
                        return Json(res);

                    //Refresh 
                    doInvoice.ExemptApproveNo = item.ExemptApproveNo;
                    doInvoiceDb.Add(doInvoice);
                }
                //Refresh
                screenObject.InvoiceList = doInvoiceDb;


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
        /// Event when click confirm button, this function will register the information of invoice detail list to the system.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS040_cmdConfirm(ICS040_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;

            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS040_IsAllowOperate(res))
                    return Json(res);

                //Validate  business, (Already confirmed with sa, need validate at this point)
                ICS040_ScreenParameter screenObject = GetScreenObject<ICS040_ScreenParameter>();
                List<doInvoice> doInvoiceDb = new List<doInvoice>();
                foreach (var item in screenObject.InvoiceList)
                {
                    //Get Invoice Data
                    doInvoice doInvoice = ICS040_GetInvoice(item.InvoiceNo, res);
                    if (res.IsError)
                        return Json(res);

                    //Validate  business
                    ICS040_ValidateInvoiceBusiness(doInvoice, res);
                    if (res.IsError)
                        return Json(res);

                    //Refresh 
                    doInvoice.ExemptApproveNo = item.ExemptApproveNo;
                    doInvoiceDb.Add(doInvoice);
                }
                //Refresh
                screenObject.InvoiceList = doInvoiceDb;


                //Save Data to db
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                        foreach (doInvoice item in screenObject.InvoiceList)
                        {
                            bool isSuccess = billingHandler.RegisterInvoiceExemption(item);
                            if (!isSuccess)
                            {
                            //all Fail
                            scope.Dispose();
                            res.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7087);
                                return Json(res);
                            }
                        }

                        //Clear session
                        screenObject.InvoiceList = new List<doInvoice>();

                    //Success
                    scope.Complete();
                    res.ResultData = "1";
                        return Json(res);
                    }
                    catch (Exception ex)
                    {
                    //all Fail
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
        /// Event when click reset button, this function will reset the screen to be the same screen as opening the screen.
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS040_cmdReset()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICS040_ScreenParameter screenObject = GetScreenObject<ICS040_ScreenParameter>();
                if (screenObject != null && screenObject.InvoiceList != null)
                {
                    screenObject.InvoiceList.Clear();
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<doInvoice>(null, "Income\\ICS040", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            return Json(res);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Retrieve invoice information of specific invoice no.
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="res"></param>
        /// <returns></returns>
        private doInvoice ICS040_GetInvoice(string invoiceNo, ObjectResultData res)
        {
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            ValidatorUtil validator = new ValidatorUtil();
            doInvoice doInvoice = billingHandler.GetInvoice(invoiceNo);
            if (doInvoice == null)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS040"
                    , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7003
                    , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                return null;
            }
            return doInvoice;
        }
        /// <summary>
        /// Validate business of specific invoice information
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="res"></param>
        private void ICS040_ValidateInvoiceBusiness(doInvoice doInvoice, ObjectResultData res)
        {
            if (doInvoice == null)
                return;

            ValidatorUtil validator = new ValidatorUtil();
            if ((doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK) == false)
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7007
                    , "InvoiceNo", doInvoice.InvoiceNo);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
            }
            //else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_BILLING_EXEMPTION)
            //{
            //    validator.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS040"
            //        , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7088
            //        , "InvoiceNo", "lblInvoiceNo", "InvoiceNo");
            //    ValidatorUtil.BuildErrorMessage(res, validator, null);
            //}
        }
        #endregion
    }
}