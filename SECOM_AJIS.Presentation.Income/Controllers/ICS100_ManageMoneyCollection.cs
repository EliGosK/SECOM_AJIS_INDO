
//*********************************
// Create by: Waroon H.
// Create date: 29/Mar/2012
// Update date: 29/Mar/2012
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
        public ActionResult ICS100_Authority(ICS100_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            // System Suspend
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            if (handlerCommon.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            // Check User Permission
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_MONEY_COLLECTION, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            return InitialScreenEnvironment<ICS100_ScreenParameter>("ICS100", param, res);

        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS100")]
        public ActionResult ICS100()
        {
            ICS100_ScreenParameter param = GetScreenObject<ICS100_ScreenParameter>();
            return View();
        }

        /// <summary>
        /// Generate xml for initial manage money collection grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS100_InitialManageMoneyCollectionGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS100_ManageMoneyCollection", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Retrieve receipt information of specific screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria</param>
        /// <returns></returns>
        public ActionResult ICS100_RetrieveData(ICS100_RegisterData data)
        {

            ICS100_ScreenParameter param = GetScreenObject<ICS100_ScreenParameter>();
            ICS100_RegisterData RegisterData = new ICS100_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();

            try
            {

                IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                doReceipt _doReceipt = new doReceipt();
                List<tbt_MoneyCollectionInfo> _dotbt_MoneyCollectionInfoList = new List<tbt_MoneyCollectionInfo>();

                // Check System Suspend
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ICS100_ScreenParameter sParam = GetScreenObject<ICS100_ScreenParameter>();

                if (data == null)
                {
 
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                     "ICS100",
                                     MessageUtil.MODULE_COMMON,
                                     MessageUtil.MessageList.MSG0007,
                                     "txtReceiptNo", "lblReceiptNo", "txtReceiptNo");
                }
                if (data.Header == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                     "ICS100",
                                     MessageUtil.MODULE_COMMON,
                                     MessageUtil.MessageList.MSG0007,
                                     "txtReceiptNo", "lblReceiptNo", "txtReceiptNo");
                }
                
                /////////////////////////////////////////////////////////////////////
                if (String.IsNullOrEmpty(data.Header.txtReceiptNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                     "ICS100",
                                     MessageUtil.MODULE_COMMON,
                                     MessageUtil.MessageList.MSG0007,
                                     "txtReceiptNo", "lblReceiptNo", "txtReceiptNo");

                    
                }
 
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    return Json(res);
                }
                //--------------------------------------------------------------

                param._dotbt_MoneyCollectionInfo = null;
                param.doReceipt = null;

                _doReceipt = incomeHandler.GetReceipt(data.Header.txtReceiptNo);
                if (_doReceipt == null)
                {
                    //MSG7003
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                     "ICS100",
                                     MessageUtil.MODULE_INCOME,
                                     MessageUtil.MessageList.MSG7003,
                                     new string[] { "lblReceiptNo" },
                                     new string[] { "txtReceiptNo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                else
                {
                    if (_doReceipt.AdvanceReceiptStatus != AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_ISSUED)
                    {
                        //MSG7028
                        res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                     "ICS100",
                                     MessageUtil.MODULE_INCOME,
                                     MessageUtil.MessageList.MSG7028,
                                     new string[] { data.Header.txtReceiptNo },
                                     new string[] { "txtReceiptNo" });
                        return Json(res);
                    }
                    _dotbt_MoneyCollectionInfoList = incomeHandler.GetTbt_MoneyCollectionInfo(data.Header.txtReceiptNo);
                }
                if (_dotbt_MoneyCollectionInfoList != null)
                {
                    if (_dotbt_MoneyCollectionInfoList.Count > 0)
                    {
                        param._dotbt_MoneyCollectionInfo = _dotbt_MoneyCollectionInfoList[0];
                    }
                }
                
                param.doReceipt = _doReceipt;

                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = sParam; }
                else
                { res.ResultData = null; }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Check inputted information before add to grid list
        /// </summary>
        /// <param name="data">input data from screen</param>
        /// <returns></returns>

        public ActionResult ICS100_CheckAddDataToGrid(ICS100_CheckAddData data)
        {
 
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                if (data.txtCollectionArea == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                     "ICS100",
                                     MessageUtil.MODULE_COMMON,
                                     MessageUtil.MessageList.MSG0007,
                                     "cboCollectionArea", "lblCollectionArea", "cboCollectionArea");
                }
                if (data.dtpExpectedCollectDate == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                     "ICS100",
                                     MessageUtil.MODULE_COMMON,
                                     MessageUtil.MessageList.MSG0007,
                                     "dtpExpectedCollectDate", "lblExpectedCollectDate", "dtpExpectedCollectDate");
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    return Json(res);
                }
                //--------------------------------------------------
                if (data.bolCheckDuplicate)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME,
                                     "ICS100",
                                     MessageUtil.MODULE_INCOME,
                                     MessageUtil.MessageList.MSG7002,
                                     new string[] {data.txtReceiptNo} ,
                                     new string[] {"txtReceiptNo"});
 
                    if (res.IsError)
                    {
                        return Json(res);
                    }
                }

                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = "1"; }
                else
                { res.ResultData = null; }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Event when click register button 
        /// </summary>
        /// <param name="data">screen input information</param>
        /// <returns></returns>
        public ActionResult ICS100_Register(ICS100_RegisterData data)
        {

            ICS100_ScreenParameter param = GetScreenObject<ICS100_ScreenParameter>();
            ICS100_RegisterData RegisterData = new ICS100_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ObjectResultData resByIssue = new ObjectResultData();
            ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check User Permission
                // revise ScreenID. search value by "<SECOM_AJIS.Common.Util.ConstantValue.ScreenID>"

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_MONEY_COLLECTION, FunctionID.C_FUNC_ID_OPERATE) )
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                //    return Json(res);
                //}

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
        /// validate input data confirm and register data into database
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS100_Confirm()
        {
            ICS100_ScreenParameter param = GetScreenObject<ICS100_ScreenParameter>();
            ICS100_RegisterData RegisterData = new ICS100_RegisterData();
            CommonUtil comUtil = new CommonUtil();
            // reuse param that send on Register Click
            if (param != null)
            {
                RegisterData = param.RegisterData;
            }

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ObjectResultData resByIssue = new ObjectResultData();
            ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                    #region Tbt_MoneyCollectionInfo
                    for (int i = 0; i < RegisterData.Detail1.Count; i++)
                    {
                        decimal? ra = (decimal?)Convert.ToDecimal(RegisterData.Detail1[i].txtReceiptAmount);
                        string raCurrencyType = RegisterData.Detail1[i].txtReceiptAmountCurrencyType;
                        tbt_MoneyCollectionInfo _dotbt_MoneyCollectionInfo = new tbt_MoneyCollectionInfo()
                        {
                            ReceiptNo = RegisterData.Detail1[i].txtReceiptNo,
                            ReceiptDate = RegisterData.Detail1[i].dtpReceiptDate,
                            BillingTargetCode = RegisterData.Detail1[i].txtBillingTargetCode,
                            CollectionArea = RegisterData.Detail1[i].txtCollectionArea,
                            ExpectedCollectDate = (DateTime)RegisterData.Detail1[i].dtpExpectedCollectDate,
                            Memo = RegisterData.Detail1[i].txtMemo,

                            // add by jirawat jannet on 2016-10-28
                            ReceiptAmount = raCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL ? ra : null,
                            ReceiptAmountCurrencyType = RegisterData.Detail1[i].txtReceiptAmountCurrencyType,
                            ReceiptAmountUsd = raCurrencyType == CurrencyUtil.C_CURRENCY_US ? ra : null
                        };
                        // Comment by Jirawat Jannet on 2016-10-28
                        //_dotbt_MoneyCollectionInfo.ReceiptNo = RegisterData.Detail1[i].txtReceiptNo;
                        //_dotbt_MoneyCollectionInfo.ReceiptDate = RegisterData.Detail1[i].dtpReceiptDate;
                        //_dotbt_MoneyCollectionInfo.BillingTargetCode = RegisterData.Detail1[i].txtBillingTargetCode;
                        //_dotbt_MoneyCollectionInfo.ReceiptAmount = (decimal?)Convert.ToDecimal(RegisterData.Detail1[i].txtReceiptAmount);
                        //_dotbt_MoneyCollectionInfo.CollectionArea = RegisterData.Detail1[i].txtCollectionArea;
                        //_dotbt_MoneyCollectionInfo.ExpectedCollectDate = (DateTime)RegisterData.Detail1[i].dtpExpectedCollectDate;
                        //_dotbt_MoneyCollectionInfo.Memo = RegisterData.Detail1[i].txtMemo;

                        if (incomeHandler.CreateTbt_MoneyCollectionInfo(_dotbt_MoneyCollectionInfo) == 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7122, null);
                        }
                    }
                    #endregion

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    // Fail rollback all
                    scope.Dispose();
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(ex);
                    return Json(res);
                }
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
    }
}
