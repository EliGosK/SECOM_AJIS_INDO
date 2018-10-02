
//*********************************
// Create by: Waroon H.
// Create date: 21/Feb/2012
// Update date: 21/Feb/2012
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
using SECOM_AJIS.Presentation.Billing.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Billing;
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

namespace SECOM_AJIS.Presentation.Billing.Controllers
{
    public partial class BillingController : BaseController
    {

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS060_Authority(BLS060_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();

            // System Suspend
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            if (handlerCommon.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_BILLING, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            return InitialScreenEnvironment<BLS060_ScreenParameter>("BLS060", param, res);

        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS060")]
        public ActionResult BLS060()
        {

            return View();
        }

        /// <summary>
        /// Generate xml for initial by billing detail section grid
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS060_InitialByBillingDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS060_ByBillingDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Generate xml for initial by invoice section grid
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS060_InitialByInvoiceGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS060_ByInvoice", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Event when click register button 
        /// </summary>
        /// <param name="data">screen input information</param>
        /// <returns></returns>
        public ActionResult BLS060_Register(BLS060_RegisterData data)
        {

            string conModeRadio2rdoByBillingDetails = "1";
            string conModeRadio2rdoByInvoice = "2";

            BLS060_ScreenParameter param = GetScreenObject<BLS060_ScreenParameter>();
            BLS060_RegisterData RegisterData = new BLS060_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ObjectResultData resByIssue = new ObjectResultData();
            ObjectResultData resByInvoice = new ObjectResultData();

            try
            {
                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                //Validate Details
                if (data.Header.rdoProcessUnit == conModeRadio2rdoByBillingDetails)
                {
                    resByIssue = ValidateByBillingDetails(data);
                    if (resByIssue.MessageList != null)
                    {
                        if (resByIssue.MessageList.Count > 0)
                        {
                            return Json(resByIssue);
                        }
                    }
                }
                if (data.Header.rdoProcessUnit == conModeRadio2rdoByInvoice)
                {
                    resByInvoice = ValidateByInvoice(data);
                    if (resByInvoice.MessageList != null)
                    {
                        if (resByInvoice.MessageList.Count > 0)
                        {
                            return Json(resByInvoice);
                        }
                    }
                }

                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }
                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = param.RegisterData; }
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
        /// Validate business in by billing details screen mode
        /// </summary>
        /// <param name="RegisterData">input data from screen</param>
        /// <returns></returns>
        public ObjectResultData ValidateByBillingDetails(BLS060_RegisterData RegisterData)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            string conModeRadio1rdoSwitchToAutoTransferCreditCard = "1";
            string conModeRadio1rdoStopAutoTransferCreditCard = "2";
            string conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate = "3";

            string rdoProcessType = RegisterData.Header.rdoProcessType;
            string rdoProcessUnit = RegisterData.Header.rdoProcessUnit;

            string txtContractCode = "";
            string txtBillingOCC = "";
            string txtRunningNo = "";
            DateTime? dtpIssueDate = null;
            bool chkRealTimeIssue = false;

            bool bolHaveDataMoreThanOneLine = false;

            try
            {
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                BLS060_ScreenParameter param = GetScreenObject<BLS060_ScreenParameter>();
                CommonUtil comUtil = new CommonUtil();

                List<tbt_BillingDetail> _doCheckdoBillingDetail = new List<tbt_BillingDetail>();
                List<tbt_BillingBasic> _doCheckdoBillingBasic = new List<tbt_BillingBasic>();
                List<doGetUnpaidInvoiceData> _doCheckdoGetUnpaidInvoiceData = new List<doGetUnpaidInvoiceData>();
                // Do Business 

                for (int i = 0; i < RegisterData.Detail1.Count; i++)
                {

                    if (!(String.IsNullOrEmpty(RegisterData.Detail1[i].txtContractCode) && String.IsNullOrEmpty(RegisterData.Detail1[i].txtBillingOCC) && String.IsNullOrEmpty(RegisterData.Detail1[i].txtRunningNo)))
                    {
                        bolHaveDataMoreThanOneLine = true;

                        txtContractCode = RegisterData.Detail1[i].txtContractCode;
                        txtBillingOCC = RegisterData.Detail1[i].txtBillingOCC;
                        txtRunningNo = RegisterData.Detail1[i].txtRunningNo;
                        dtpIssueDate = RegisterData.Detail1[i].dtpIssueDate;
                        chkRealTimeIssue = RegisterData.Detail1[i].chkRealTimeIssue;

                        if (String.IsNullOrEmpty(txtContractCode))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS060",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail1[i].txtContractCodeID,
                                                 "lblHeader2ContractCodeErrorMsg",
                                                 RegisterData.Detail1[i].txtContractCodeID);
                        }

                        if (String.IsNullOrEmpty(txtBillingOCC))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS060",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail1[i].txtBillingOCCID,
                                                 "lblHeader2BillingOCCErrorMsg",
                                                 RegisterData.Detail1[i].txtBillingOCCID);
                        }

                        if (String.IsNullOrEmpty(txtRunningNo))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS060",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail1[i].txtRunningNoID,
                                                 "lblHeader2RunningNoErrorMsg",
                                                 RegisterData.Detail1[i].txtRunningNoID);
                        }
                        if (rdoProcessType == conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate)
                        {
                            if (String.IsNullOrEmpty(dtpIssueDate.ToString()))
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                     "BLS060",
                                                     MessageUtil.MODULE_COMMON,
                                                     MessageUtil.MessageList.MSG0007,
                                                      RegisterData.Detail1[i].dtpIssueDateID,
                                                      "lblHeader2IssuedateAutoTransferDateErrorMsg",
                                                      RegisterData.Detail1[i].dtpIssueDateID);
                            }
                        }

                        ValidatorUtil.BuildErrorMessage(res, validator, null);
                        if (res.IsError)
                        {
                            return res;
                        }
                        //------------------------------------------------------------

                        txtContractCode = comUtil.ConvertBillingCode(txtContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                        // Business 7.2.2.1
                        _doCheckdoBillingDetail = billingHandler.GetTbt_BillingDetailData(
                                    txtContractCode
                                    , txtBillingOCC
                                    , Convert.ToInt32(txtRunningNo));

                        if (_doCheckdoBillingDetail != null)
                        {
                            if (_doCheckdoBillingDetail.Count == 0)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                     "BLS060",
                                                     MessageUtil.MODULE_BILLING,
                                                     MessageUtil.MessageList.MSG6042,
                                                     new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                     new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID});
                                return res;
                            }
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                     "BLS060",
                                                     MessageUtil.MODULE_BILLING,
                                                     MessageUtil.MessageList.MSG6042,
                                                     new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                     new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                            return res;
                        }


                        // Check Business 7.2.2.2
                        _doCheckdoBillingBasic = billingHandler.GetTbt_BillingBasic(
                                        txtContractCode
                                        , txtBillingOCC);

                        if (_doCheckdoBillingBasic != null)
                        {
                            if (_doCheckdoBillingBasic.Count > 0)
                            {
                                var lstBillingTarget = billingHandler.GetTbt_BillingTargetForView(_doCheckdoBillingBasic[0].BillingTargetCode, MiscType.C_CUST_TYPE);
                                if (lstBillingTarget.Count > 0)
                                {
                                    var existsBillingOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == lstBillingTarget[0].BillingOfficeCode);
                                    if (existsBillingOffice.Count() <= 0)
                                    {
                                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                        res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                                            MessageUtil.MessageList.MSG0063,
                                                            null,
                                                            new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                        return res;
                                    }
                                }
                                if (_doCheckdoBillingBasic[0].CarefulFlag == true)
                                {
                                    //MSG6056
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                         "BLS060",
                                                         MessageUtil.MODULE_BILLING,
                                                         MessageUtil.MessageList.MSG6056,
                                                         new string[] { RegisterData.Detail1[i].txtContractCode + "-" + RegisterData.Detail1[i].txtBillingOCC },
                                                         new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID});
                                    return res;
                                }
                            }
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS060",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6042,
                                                 new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                 new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                            return res;
                        }



                        // Check Business 7.2.2.3
                        if (rdoProcessType == conModeRadio1rdoSwitchToAutoTransferCreditCard)
                        {

                            string strBankCode = null;
                            List<tbt_AutoTransferBankAccount> _dotbt_AutoTransferBankAccountList = new List<tbt_AutoTransferBankAccount>();
                            List<tbt_CreditCard> _dotbt_CreditCardList = new List<tbt_CreditCard>();
                            tbt_ExportAutoTransfer _dotbt_ExportAutoTransfer = new tbt_ExportAutoTransfer();
                            if (_doCheckdoBillingBasic != null)
                            {
                                if (_doCheckdoBillingBasic.Count > 0)
                                {
                                    if (_doCheckdoBillingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                                    {
                                        _dotbt_AutoTransferBankAccountList = billingHandler.GetTbt_AutoTransferBankAccount(
                                            txtContractCode
                                             , txtBillingOCC);

                                        if (_dotbt_AutoTransferBankAccountList != null && _dotbt_AutoTransferBankAccountList.Count > 0)
                                        {
                                            strBankCode = _dotbt_AutoTransferBankAccountList[0].BankCode;
                                        }
                                        else
                                        {
                                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                     "BLS060",
                                                     MessageUtil.MODULE_BILLING,
                                                     MessageUtil.MessageList.MSG6006,
                                                     new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                     new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                            return res;
                                        }

                                    }
                                    else if (_doCheckdoBillingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                    {
                                        _dotbt_CreditCardList = billingHandler.GetTbt_CreditCard(
                                            txtContractCode
                                             , txtBillingOCC);

                                        if (_dotbt_CreditCardList != null && _dotbt_CreditCardList.Count > 0)
                                        {
                                            strBankCode = _dotbt_CreditCardList[0].CreditCardCompanyCode;
                                        }
                                        else
                                        {
                                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                     "BLS060",
                                                     MessageUtil.MODULE_BILLING,
                                                     MessageUtil.MessageList.MSG6007,
                                                     new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                     new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                            return res;
                                        }
                                    }
                                    else if (_doCheckdoBillingBasic[0].PaymentMethod != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER &&
                                        _doCheckdoBillingBasic[0].PaymentMethod != PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                    {
                                        //MSG6044 
                                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS060",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6044,
                                                 new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                 new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                        return res;
                                    }
                                }
                            }
                            if (!(string.IsNullOrEmpty(dtpIssueDate.ToString())))
                            {
                                if (dtpIssueDate < System.DateTime.Now.AddDays(-1))
                                {
                                    // MSG6025 
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                        "BLS060",
                                                        MessageUtil.MODULE_BILLING,
                                                        MessageUtil.MessageList.MSG6025,
                                                        new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                        new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                    return res;
                                }
                            }

                            if (strBankCode != null && dtpIssueDate != null)
                            {

                                List<SECOM_AJIS.DataEntity.Master.tbm_AutoTransferScheduleList> _dotbm_AutoTransferScheduleList = new List<DataEntity.Master.tbm_AutoTransferScheduleList>();
                                string strTempDateNo = string.Empty;
                                strTempDateNo = dtpIssueDate.Value.Day.ToString();
                                strTempDateNo = strTempDateNo.PadLeft(2, '0');

                                _dotbm_AutoTransferScheduleList = masterHandler.GetTbm_AutoTransferScheduleList(
                                    strBankCode, strTempDateNo);

                                if (_dotbm_AutoTransferScheduleList == null)
                                {
                                    // MSG6026 
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             "BLS060",
                                                             MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6026,
                                                             new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                             new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                    return res;
                                }
                                else
                                {
                                    if (_dotbm_AutoTransferScheduleList.Count == 0)
                                    {
                                        // MSG6026 
                                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             "BLS060",
                                                             MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6026,
                                                             new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                             new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                        return res;
                                    }
                                }

                                _dotbt_ExportAutoTransfer = billingHandler.GetExportAutoTransfer
                                    (strBankCode, Convert.ToDateTime(dtpIssueDate));

                                if (_dotbt_ExportAutoTransfer != null)
                                {
                                    // MSG6028 
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                 "BLS060",
                                                                 MessageUtil.MODULE_BILLING,
                                                                 MessageUtil.MessageList.MSG6028,
                                                                 new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                                 new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                    return res;
                                }

                            }
                            // bls060-17
                            else if (dtpIssueDate == null)
                            {

                                dtpIssueDate = billingHandler.GetNextAutoTransferDate(
                                    _doCheckdoBillingBasic[0].ContractCode,
                                    _doCheckdoBillingBasic[0].BillingOCC,
                                    _doCheckdoBillingBasic[0].PaymentMethod);

                                RegisterData.Detail1[i].dtpIssueDate = dtpIssueDate;
                                param.RegisterData = RegisterData;
                            }

                            if (_doCheckdoBillingDetail != null)
                            {
                                if (_doCheckdoBillingDetail.Count > 0)
                                {
                                    //                                 
                                    if (_doCheckdoBillingDetail[0].PaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT)
                                    {
                                        _doCheckdoGetUnpaidInvoiceData = billingHandler.GetUnpaidInvoiceDataList(_doCheckdoBillingDetail[0].InvoiceNo);

                                        if (_doCheckdoGetUnpaidInvoiceData != null)
                                        {
                                            if (_doCheckdoGetUnpaidInvoiceData.Count > 0)
                                            {

                                                List<doGetBillingDetailOfInvoice> _doCheck = billingHandler.GetBillingDetailOfInvoiceList(
                                                _doCheckdoGetUnpaidInvoiceData[0].InvoiceNo,
                                                _doCheckdoGetUnpaidInvoiceData[0].InvoiceOCC);

                                                if (_doCheck.Count > 1) //Invoice have billing detail > 1
                                                {
                                                    //MSG6045 
                                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             "BLS060",
                                                             MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6045,
                                                             new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                             new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                                    return res;
                                                }

                                            }
                                        }
                                    }
                                    else if (_doCheckdoBillingDetail[0].PaymentStatus !=
                                        PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT)
                                    {
                                        //MSG6046 
                                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS060",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6046,
                                                 new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                 new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                        return res;
                                    }
                                }
                            }

                        }
                        // Check Business 7.2.2.4
                        if (rdoProcessType == conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate)
                        {
                            string strBankCode = null;
                            List<tbt_AutoTransferBankAccount> _dotbt_AutoTransferBankAccountList = new List<tbt_AutoTransferBankAccount>();
                            List<tbt_CreditCard> _dotbt_CreditCardList = new List<tbt_CreditCard>();
                            tbt_ExportAutoTransfer _dotbt_ExportAutoTransfer = new tbt_ExportAutoTransfer();
                            if (_doCheckdoBillingBasic != null)
                            {
                                if (_doCheckdoBillingBasic.Count > 0)
                                {
                                    if (_doCheckdoBillingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                                    {
                                        _dotbt_AutoTransferBankAccountList = billingHandler.GetTbt_AutoTransferBankAccount(
                                            txtContractCode
                                             , txtBillingOCC);

                                        if (_dotbt_AutoTransferBankAccountList != null)
                                        {
                                            if (_dotbt_AutoTransferBankAccountList.Count > 0)
                                            {
                                                strBankCode = _dotbt_AutoTransferBankAccountList[0].BankCode;
                                            }
                                        }

                                    }
                                    else if (_doCheckdoBillingBasic[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                    {
                                        _dotbt_CreditCardList = billingHandler.GetTbt_CreditCard(
                                            txtContractCode
                                             , txtBillingOCC);

                                        if (_dotbt_CreditCardList != null)
                                        {
                                            if (_dotbt_CreditCardList.Count > 0)
                                            {
                                                strBankCode = _dotbt_CreditCardList[0].CreditCardCompanyCode;
                                            }
                                        }
                                    }
                                }
                            }
                            if (!(string.IsNullOrEmpty(dtpIssueDate.ToString())))
                            {
                                if (dtpIssueDate < System.DateTime.Now.AddDays(-1))
                                {
                                    // MSG6025 
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                        "BLS060",
                                                        MessageUtil.MODULE_BILLING,
                                                        MessageUtil.MessageList.MSG6025,
                                                        new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                        new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                    return res;
                                }
                            }

                            if (strBankCode != null && dtpIssueDate != null &&
                                (_doCheckdoBillingDetail[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER || _doCheckdoBillingDetail[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER))
                            {

                                List<SECOM_AJIS.DataEntity.Master.tbm_AutoTransferScheduleList> _dotbm_AutoTransferScheduleList = new List<DataEntity.Master.tbm_AutoTransferScheduleList>();
                                string strTempDateNo = string.Empty;
                                strTempDateNo = dtpIssueDate.Value.Day.ToString();
                                strTempDateNo = strTempDateNo.PadLeft(2, '0');

                                _dotbm_AutoTransferScheduleList = masterHandler.GetTbm_AutoTransferScheduleList(
                                    strBankCode, strTempDateNo);

                                if (_dotbm_AutoTransferScheduleList == null)
                                {
                                    // MSG6026 
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             "BLS060",
                                                             MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6026,
                                                             new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                             new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                    return res;
                                }
                                else
                                {
                                    if (_dotbm_AutoTransferScheduleList.Count == 0)
                                    {
                                        // MSG6026 
                                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             "BLS060",
                                                             MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6026,
                                                             new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                             new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                        return res;
                                    }
                                }

                                _dotbt_ExportAutoTransfer = billingHandler.GetExportAutoTransfer
                                    (strBankCode, Convert.ToDateTime(dtpIssueDate));

                                if (_dotbt_ExportAutoTransfer != null)
                                {
                                    // MSG6028 
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                 "BLS060",
                                                                 MessageUtil.MODULE_BILLING,
                                                                 MessageUtil.MessageList.MSG6028,
                                                                 new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                                 new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                    return res;
                                }

                            }

                            if (_doCheckdoBillingDetail != null)
                            {
                                if (_doCheckdoBillingDetail[0].PaymentStatus != PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT
                                    && _doCheckdoBillingDetail[0].PaymentStatus != PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT)
                                {
                                    //MSG6049 
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                "BLS060",
                                                MessageUtil.MODULE_BILLING,
                                                MessageUtil.MessageList.MSG6049,
                                                new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                            RegisterData.Detail1[i].txtBillingOCCID, 
                                                            RegisterData.Detail1[i].txtRunningNoID,
                                                        RegisterData.Detail1[i].dtpIssueDateID });
                                    return res;
                                }
                            }
                        }
                        // Check Business 7.2.2.5
                        if (rdoProcessType == conModeRadio1rdoStopAutoTransferCreditCard)
                        {
                            if (_doCheckdoBillingDetail != null)
                            {
                                if (_doCheckdoBillingDetail.Count > 0)
                                {
                                    //                                 
                                    if (_doCheckdoBillingDetail[0].PaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT)
                                    {

                                        _doCheckdoGetUnpaidInvoiceData = billingHandler.GetUnpaidInvoiceDataList(_doCheckdoBillingDetail[0].InvoiceNo);

                                        if (_doCheckdoGetUnpaidInvoiceData != null)
                                        {
                                            if (_doCheckdoGetUnpaidInvoiceData.Count > 0)
                                            {

                                                List<doGetBillingDetailOfInvoice> _doCheck = billingHandler.GetBillingDetailOfInvoiceList(
                                                    _doCheckdoGetUnpaidInvoiceData[0].InvoiceNo,
                                                    _doCheckdoGetUnpaidInvoiceData[0].InvoiceOCC);

                                                if (_doCheck.Count > 1)
                                                {
                                                    //MSG6047  
                                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             "BLS060",
                                                             MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6047,
                                                             new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                             new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                                    return res;
                                                }
                                            }
                                        }
                                    }
                                    else if (_doCheckdoBillingDetail[0].PaymentStatus !=
                                        PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT)
                                    {
                                        //MSG6048  
                                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS060",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6048,
                                                 new string[] { "lblHeader2ContractCodeErrorMsg" },
                                                 new string[] { RegisterData.Detail1[i].txtContractCodeID, 
                                                             RegisterData.Detail1[i].txtBillingOCCID, 
                                                             RegisterData.Detail1[i].txtRunningNoID,
                                                         RegisterData.Detail1[i].dtpIssueDateID });
                                        return res;
                                    }
                                }
                            }
                        }
                    }
                }
                if (!(bolHaveDataMoreThanOneLine))
                {

                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS060",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6091,
                                         new string[] { "lblHeader2ContractCodeErrorMsg" },
                                         new string[] { RegisterData.Detail1[0].txtContractCodeID, 
                                                             RegisterData.Detail1[0].txtBillingOCCID, 
                                                             RegisterData.Detail1[0].txtRunningNoID,
                                                         RegisterData.Detail1[0].dtpIssueDateID  });
                    return res;
                }

                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = "1"; }
                else
                { res.ResultData = "0"; }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.ResultData = "0";
                res.AddErrorMessage(ex);
            }

            return res;

        }

        /// <summary>
        /// Validate business in by invoice screen mode
        /// </summary>
        /// <param name="RegisterData">input data from screen</param>
        /// <returns></returns>
        public ObjectResultData ValidateByInvoice(BLS060_RegisterData RegisterData)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();

            string conModeRadio1rdoSwitchToAutoTransferCreditCard = "1";
            string conModeRadio1rdoStopAutoTransferCreditCard = "2";
            string conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate = "3";

            string rdoProcessType = RegisterData.Header.rdoProcessType;
            string rdoProcessUnit = RegisterData.Header.rdoProcessUnit;

            string txtInvoiceNo = "";
            DateTime? dtpAutoTransferDate = null;
            bool chkRealTimeIssue = false;

            bool bolHaveDataMoreThanOneLine = false;

            try
            {
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                BLS060_ScreenParameter param = GetScreenObject<BLS060_ScreenParameter>();

                tbt_ExportAutoTransfer _dotbt_ExportAutoTransfer = new tbt_ExportAutoTransfer();
                List<tbt_BillingDetail> _doCheckdoBillingDetail = new List<tbt_BillingDetail>();
                List<tbt_BillingBasic> _doCheckdoBillingBasic = new List<tbt_BillingBasic>();
                tbt_Invoice _doCheckdoGetInvoiceData = new tbt_Invoice();
                List<doGetBillingDetailOfInvoice> _doCheckdoGetBillingDetailOfInvoice = new List<doGetBillingDetailOfInvoice>();
                CommonUtil comUtil = new CommonUtil();
                // Do Business 

                for (int i = 0; i < RegisterData.Detail2.Count; i++)
                {

                    if (!(String.IsNullOrEmpty(RegisterData.Detail2[i].txtInvoiceNo)))
                    {
                        bolHaveDataMoreThanOneLine = true;

                        txtInvoiceNo = RegisterData.Detail2[i].txtInvoiceNo;
                        dtpAutoTransferDate = RegisterData.Detail2[i].dtpAutoTransferDate;
                        chkRealTimeIssue = RegisterData.Detail2[i].chkRealTimeIssue;

                        if (String.IsNullOrEmpty(txtInvoiceNo))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS060",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].txtInvoiceNoID,
                                                 "lblHeader3InvoiceNoErrorMsg",
                                                 RegisterData.Detail2[i].txtInvoiceNoID);
                        }
                        //if (rdoProcessType == conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate)
                        //{
                        //    if (String.IsNullOrEmpty(dtpAutoTransferDate.ToString()))
                        //    {
                        //        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                        //                             "BLS060",
                        //                             MessageUtil.MODULE_COMMON,
                        //                             MessageUtil.MessageList.MSG0007,
                        //                             RegisterData.Detail2[i].dtpAutoTransferDateID,
                        //                             "lblHeader3AutoTransferDate",
                        //                             RegisterData.Detail2[i].dtpAutoTransferDateID );
                        //    }
                        //}
                        ValidatorUtil.BuildErrorMessage(res, validator, null);
                        if (res.IsError)
                        {
                            return res;
                        }

                        string strPaymentStatus =
                            "," + PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                            + "," + PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                            + "," + PaymentStatus.C_PAYMENT_STATUS_COUNTER_BAL
                            + "," + PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                            + "," + PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                            + "," + PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                            + ",";
                        //7.3.2.1	Check invoice no. must exist in billing DB
                        _doCheckdoGetInvoiceData = billingHandler.GetTbt_InvoiceData(txtInvoiceNo, null);

                        if (_doCheckdoGetInvoiceData != null)
                        {
                            _doCheckdoGetBillingDetailOfInvoice = billingHandler.GetBillingDetailOfInvoiceList(
                                _doCheckdoGetInvoiceData.InvoiceNo
                                , _doCheckdoGetInvoiceData.InvoiceOCC);
                        }
                        else
                        {
                            //MSG6043 
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                     "BLS060",
                                     MessageUtil.MODULE_BILLING,
                                     MessageUtil.MessageList.MSG6043,
                                     new string[] { "lblHeader3InvoiceNoErrorMsg" },
                                     new string[] { RegisterData.Detail2[i].txtInvoiceNoID, 
                                             RegisterData.Detail2[i].dtpAutoTransferDateID });
                            return res;
                        }

                        //7.3.2.2	Loop in doBillingDetailList of doInvoice
                        if (_doCheckdoGetBillingDetailOfInvoice != null)
                        {
                            foreach (doGetBillingDetailOfInvoice _doGetBillingDetailOfInvoice in _doCheckdoGetBillingDetailOfInvoice)
                            {

                                _doCheckdoBillingBasic = billingHandler.GetTbt_BillingBasic(
                                    _doGetBillingDetailOfInvoice.ContractCode
                                    , _doGetBillingDetailOfInvoice.BillingOCC);

                                if (_doCheckdoBillingBasic != null)
                                {
                                    if (_doCheckdoBillingBasic.Count > 0)
                                    {
                                        var lstBillingTarget = billingHandler.GetTbt_BillingTargetForView(_doCheckdoBillingBasic[0].BillingTargetCode, MiscType.C_CUST_TYPE);
                                        if (lstBillingTarget.Count > 0)
                                        {
                                            var existsBillingOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == lstBillingTarget[0].BillingOfficeCode);
                                            if (existsBillingOffice.Count() <= 0)
                                            {
                                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                                res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                                                    MessageUtil.MessageList.MSG0063,
                                                                    null,
                                                                    new string[] { RegisterData.Detail2[i].txtInvoiceNoID });
                                                return res;
                                            }
                                        }

                                        if (_doCheckdoBillingBasic[0].CarefulFlag == true)
                                        {
                                            //MSG6056
                                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                 "BLS060",
                                                                 MessageUtil.MODULE_BILLING,
                                                                 MessageUtil.MessageList.MSG6056,
                                                                 new string[] { comUtil.ConvertBillingCode(_doCheckdoBillingBasic[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT) + "-" + _doCheckdoBillingBasic[0].BillingOCC },
                                                                 new string[] { RegisterData.Detail2[i].txtInvoiceNoID });
                                            return res;
                                        }

                                        if (rdoProcessType == conModeRadio1rdoSwitchToAutoTransferCreditCard)
                                        {
                                            if (_doCheckdoBillingBasic[0].PaymentMethod != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                                && _doCheckdoBillingBasic[0].PaymentMethod != PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                            {
                                                //MSG6044
                                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                     "BLS060",
                                                                     MessageUtil.MODULE_BILLING,
                                                                     MessageUtil.MessageList.MSG6044,
                                                                     new string[] { "lblHeader3InvoiceNoErrorMsg" },
                                                                     new string[] { RegisterData.Detail2[i].txtInvoiceNoID });
                                                return res;
                                            }
                                            else
                                            {
                                                _doGetBillingDetailOfInvoice.PaymentMethod = _doCheckdoBillingBasic[0].PaymentMethod;
                                            }
                                        }

                                    }
                                }


                            }
                        }

                        if (_doCheckdoGetInvoiceData != null)
                        {
                            //7.3.2.3	If [Process type] == “Switch to auto transfer/credit card” 
                            if (rdoProcessType == conModeRadio1rdoSwitchToAutoTransferCreditCard)
                            {
                                if (_doCheckdoGetInvoiceData.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT)
                                {
                                    //MSG6051
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                 "BLS060",
                                                                 MessageUtil.MODULE_BILLING,
                                                                 MessageUtil.MessageList.MSG6051,
                                                                 new string[] { "lblHeader3InvoiceNoErrorMsg" },
                                                                new string[] { RegisterData.Detail2[i].txtInvoiceNoID });
                                    return res;
                                }

                                if (_doCheckdoGetBillingDetailOfInvoice != null)
                                {
                                    foreach (doGetBillingDetailOfInvoice _doGetBillingDetailOfInvoice in _doCheckdoGetBillingDetailOfInvoice)
                                    {
                                        //string strBankCode = billingHandler.CheckInvoiceSameAccountData(
                                        //    _doGetBillingDetailOfInvoice.InvoiceNo
                                        //    , _doGetBillingDetailOfInvoice.InvoiceOCC
                                        //    , _doGetBillingDetailOfInvoice.PaymentMethod);

                                        string strBankCode = billingHandler.CheckInvoiceSameAccount(_doGetBillingDetailOfInvoice.InvoiceNo
                                                                                        , _doGetBillingDetailOfInvoice.InvoiceOCC
                                                                                        , _doGetBillingDetailOfInvoice.PaymentMethod);
                                        if (strBankCode == null)
                                        {
                                            //MSG6050 
                                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                         "BLS060",
                                                                         MessageUtil.MODULE_BILLING,
                                                                         MessageUtil.MessageList.MSG6050,
                                                                         new string[] { "lblHeader3InvoiceNoErrorMsg" },
                                                                            new string[] { RegisterData.Detail2[i].txtInvoiceNoID });
                                            return res;
                                        }

                                        if (strBankCode != null && dtpAutoTransferDate != null)
                                        {

                                            if (!(string.IsNullOrEmpty(dtpAutoTransferDate.ToString())))
                                            {
                                                if (dtpAutoTransferDate < System.DateTime.Now.AddDays(-1))
                                                {
                                                    // MSG6025
                                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                        "BLS060",
                                                                        MessageUtil.MODULE_BILLING,
                                                                        MessageUtil.MessageList.MSG6025,
                                                                        new string[] { "lblHeader3AutoTransferDateErrorMsg" },
                                                                        new string[] { RegisterData.Detail2[i].dtpAutoTransferDateID });
                                                    return res;
                                                }
                                            }


                                            List<SECOM_AJIS.DataEntity.Master.tbm_AutoTransferScheduleList> _dotbm_AutoTransferScheduleList = new List<DataEntity.Master.tbm_AutoTransferScheduleList>();
                                            string strTempDateNo = string.Empty;
                                            strTempDateNo = dtpAutoTransferDate.Value.Day.ToString();
                                            strTempDateNo = strTempDateNo.PadLeft(2, '0');

                                            _dotbm_AutoTransferScheduleList = masterHandler.GetTbm_AutoTransferScheduleList(
                                                strBankCode, strTempDateNo);

                                            if (_dotbm_AutoTransferScheduleList == null)
                                            {
                                                // MSG6026  
                                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                         "BLS060",
                                                                         MessageUtil.MODULE_BILLING,
                                                                         MessageUtil.MessageList.MSG6026,
                                                                         new string[] { "lblHeader3InvoiceNoErrorMsg" },
                                                                        new string[] { RegisterData.Detail2[i].txtInvoiceNoID , 
                                                                        RegisterData.Detail2[i].dtpAutoTransferDateID });
                                                return res;
                                            }
                                            else
                                            {
                                                if (_dotbm_AutoTransferScheduleList.Count == 0)
                                                {
                                                    // MSG6026  
                                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                             "BLS060",
                                                                             MessageUtil.MODULE_BILLING,
                                                                             MessageUtil.MessageList.MSG6026,
                                                                             new string[] { "lblHeader3InvoiceNoErrorMsg" },
                                                                            new string[] { RegisterData.Detail2[i].txtInvoiceNoID , 
                                                                        RegisterData.Detail2[i].dtpAutoTransferDateID });
                                                    return res;
                                                }
                                            }

                                            _dotbt_ExportAutoTransfer = billingHandler.GetExportAutoTransfer
                                                (strBankCode, Convert.ToDateTime(dtpAutoTransferDate));

                                            if (_dotbt_ExportAutoTransfer != null)
                                            {
                                                // MSG6028 
                                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                             "BLS060",
                                                                             MessageUtil.MODULE_BILLING,
                                                                             MessageUtil.MessageList.MSG6028,
                                                                             new string[] { "lblHeader3InvoiceNoErrorMsg" },
                                                                            new string[] { RegisterData.Detail2[i].txtInvoiceNoID , 
                                                                        RegisterData.Detail2[i].dtpAutoTransferDateID });
                                                return res;
                                            }

                                        }
                                        // bls060-10
                                        else if (dtpAutoTransferDate == null)
                                        {
                                            dtpAutoTransferDate = billingHandler.GetNextAutoTransferDate(
                                             _doGetBillingDetailOfInvoice.ContractCode,
                                             _doGetBillingDetailOfInvoice.BillingOCC,
                                             _doGetBillingDetailOfInvoice.PaymentMethod);

                                            RegisterData.Detail2[i].dtpAutoTransferDate = dtpAutoTransferDate;
                                            param.RegisterData = RegisterData;
                                        }
                                    }
                                }
                            }

                            //7.3.2.4	Else If [Process type] == “Stop auto transfer/credit card” Then
                            if (rdoProcessType == conModeRadio1rdoStopAutoTransferCreditCard)
                            {
                                if (_doCheckdoGetBillingDetailOfInvoice != null)
                                {
                                    foreach (doGetBillingDetailOfInvoice _doGetBillingDetailOfInvoice in _doCheckdoGetBillingDetailOfInvoice)
                                    {
                                        if (_doGetBillingDetailOfInvoice.PaymentStatus != PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT)
                                        {
                                            //MSG6052  
                                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                         "BLS060",
                                                                         MessageUtil.MODULE_BILLING,
                                                                         MessageUtil.MessageList.MSG6052,
                                                                         new string[] { "lblHeader3InvoiceNoErrorMsg" },
                                                                        new string[] { RegisterData.Detail2[i].txtInvoiceNoID });
                                            return res;
                                        }

                                    }
                                }
                            }

                        }

                        //if (RegisterData.Detail2[i].chkRealTimeIssue)
                        //RegisterData.Detail2[i].dtpAutoTransferDate = DateTime.Now;
                        //param.RegisterData = RegisterData;
                    }

                }
                if (!(bolHaveDataMoreThanOneLine))
                {

                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS060",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6091,
                                         new string[] { "lblHeader3InvoiceNoErrorMsg" },
                                         new string[] { RegisterData.Detail2[0].txtInvoiceNoID });
                    return res;

                }

                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = "1"; }
                else
                { res.ResultData = "0"; }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.ResultData = "0";
                res.AddErrorMessage(ex);
            }

            return res;

        }

        /// <summary>
        /// validate input data confirm and register data into database by mode
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS060_Confirm()
        {
            string conModeRadio1rdoSwitchToAutoTransferCreditCard = "1";
            string conModeRadio1rdoStopAutoTransferCreditCard = "2";
            string conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate = "3";

            string conModeRadio2rdoByBillingDetails = "1";
            string conModeRadio2rdoByInvoice = "2";

            BLS060_ScreenParameter param = GetScreenObject<BLS060_ScreenParameter>();
            BLS060_RegisterData RegisterData = new BLS060_RegisterData();
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
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }


                //Validate Details
                if (RegisterData.Header.rdoProcessUnit == conModeRadio2rdoByBillingDetails)
                {
                    resByIssue = BLS060_ByBillingDetails(RegisterData);
                    if (resByIssue.MessageList != null)
                    {
                        if (resByIssue.MessageList.Count > 0)
                        {
                            return Json(resByIssue);
                        }
                    }
                }

                if (RegisterData.Header.rdoProcessUnit == conModeRadio2rdoByInvoice)
                {
                    resByInvoice = BLS060_ByInvoice(RegisterData);
                    if (resByInvoice.MessageList != null)
                    {
                        if (resByInvoice.MessageList.Count > 0)
                        {
                            return Json(resByInvoice);
                        }
                    }
                }

                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                { res.ResultData = param.RegisterData; }
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
        /// register data into database mode by billing details
        /// </summary>
        /// <returns></returns>
        public ObjectResultData BLS060_ByBillingDetails(BLS060_RegisterData RegisterData)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            string conModeRadio1rdoSwitchToAutoTransferCreditCard = "1";
            string conModeRadio1rdoStopAutoTransferCreditCard = "2";
            string conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate = "3";

            string rdoProcessType = RegisterData.Header.rdoProcessType;
            string rdoProcessUnit = RegisterData.Header.rdoProcessUnit;

            string txtContractCode = "";
            string txtBillingOCC = "";
            string txtRunningNo = "";
            DateTime? dtpIssueDate = null;
            bool chkRealTimeIssue = false;

            using (TransactionScope scope = new TransactionScope())
            {

            try
            {
                    IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                    ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    BLS060_ScreenParameter param = GetScreenObject<BLS060_ScreenParameter>();
                    CommonUtil comUtil = new CommonUtil();
                    List<string> lstFilePath = new List<string>();


                    // Do Business 

                    for (int i = 0; i < RegisterData.Detail1.Count; i++)
                    {
                        List<tbt_BillingDetail> _doCheckdoBillingDetail = new List<tbt_BillingDetail>();
                        List<tbt_BillingBasic> _doCheckdoBillingBasic = new List<tbt_BillingBasic>();
                        List<doGetUnpaidInvoiceData> _doCheckdoGetUnpaidInvoiceData = new List<doGetUnpaidInvoiceData>();
                        // delete
                        tbt_Invoice _doDeletetbt_Invoice = new tbt_Invoice();
                        List<tbt_BillingDetail> _doDeletetbt_BillingDetailList = new List<tbt_BillingDetail>();

                        tbt_Invoice _doUpdatetbt_Invoice = new tbt_Invoice();

                        List<tbt_BillingDetail> _doUpdatetbt_BillingDetailList = new List<tbt_BillingDetail>();
                        List<tbt_BillingDetail> _doAfterUpdatetbt_BillingDetailList = new List<tbt_BillingDetail>();


                        if (!(String.IsNullOrEmpty(RegisterData.Detail1[i].txtContractCode)))
                        {
                            txtContractCode = RegisterData.Detail1[i].txtContractCode;
                            txtBillingOCC = RegisterData.Detail1[i].txtBillingOCC;
                            txtRunningNo = RegisterData.Detail1[i].txtRunningNo;
                            dtpIssueDate = RegisterData.Detail1[i].dtpIssueDate;
                            chkRealTimeIssue = RegisterData.Detail1[i].chkRealTimeIssue;

                            txtContractCode = comUtil.ConvertBillingCode(txtContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                            // get billing details and billing basic
                            // all data pass check already paid and create invoice then all data is no invoice
                            _doCheckdoBillingDetail = billingHandler.GetTbt_BillingDetailData(
                                           txtContractCode
                                           , txtBillingOCC
                                           , Convert.ToInt32(txtRunningNo));
                            if (rdoProcessType == conModeRadio1rdoSwitchToAutoTransferCreditCard || rdoProcessType == conModeRadio1rdoStopAutoTransferCreditCard)
                            {
                                if (_doCheckdoBillingDetail != null)
                                {
                                    if (_doCheckdoBillingDetail.Count != 0)
                                    {
                                        // case cancel invoice before do any thing
                                        if (!(String.IsNullOrEmpty(_doCheckdoBillingDetail[0].InvoiceNo)))
                                        {
                                            _doDeletetbt_Invoice = new tbt_Invoice();
                                            _doDeletetbt_Invoice.InvoiceNo = _doCheckdoBillingDetail[0].InvoiceNo;
                                            _doDeletetbt_Invoice.InvoiceOCC = (int)_doCheckdoBillingDetail[0].InvoiceOCC;

                                            _doDeletetbt_BillingDetailList = new List<tbt_BillingDetail>();
                                            _doDeletetbt_BillingDetailList.Add(_doCheckdoBillingDetail[0]);

                                            if (!(billingHandler.UpdateInvoicePaymentStatus(
                                                _doDeletetbt_Invoice,
                                                _doDeletetbt_BillingDetailList,
                                                PaymentStatus.C_PAYMENT_STATUS_CANCEL)))
                                            {
                                                // update fail case
                                            };
                                        }
                                        else
                                        {
                                            _doCheckdoBillingDetail[0].PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CANCEL;
                                            if (billingHandler.Updatetbt_BillingDetail(_doCheckdoBillingDetail[0]) != 1)
                                            {
                                                // update fail case
                                            }
                                        }

                                        _doCheckdoBillingBasic = billingHandler.GetTbt_BillingBasic(
                                                    txtContractCode
                                                    , txtBillingOCC);
                                    }

                                }
                            }

                            if (rdoProcessType == conModeRadio1rdoSwitchToAutoTransferCreditCard)
                            {
                                foreach (tbt_BillingDetail _doCheckdoBillingDetailData in _doCheckdoBillingDetail)
                                {
                                    tbt_BillingDetail _doUpdateTemptbt_BillingDetail = new tbt_BillingDetail();
                                    _doUpdateTemptbt_BillingDetail.ContractCode = _doCheckdoBillingDetailData.ContractCode;
                                    _doUpdateTemptbt_BillingDetail.BillingOCC = _doCheckdoBillingDetailData.BillingOCC;
                                    _doUpdateTemptbt_BillingDetail.BillingDetailNo = 0;
                                    _doUpdateTemptbt_BillingDetail.InvoiceNo = null;
                                    _doUpdateTemptbt_BillingDetail.InvoiceOCC = null;
                                    _doUpdateTemptbt_BillingDetail.IssueInvDate = dtpIssueDate;
                                    if (dtpIssueDate.Value.AddDays(-30) < CommonUtil.dsTransData.dtOperationData.ProcessDateTime)
                                    {
                                        _doUpdateTemptbt_BillingDetail.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    }
                                    else
                                    {
                                        _doUpdateTemptbt_BillingDetail.IssueInvDate = dtpIssueDate.Value.AddDays(-30);
                                    }
                                    _doUpdateTemptbt_BillingDetail.IssueInvFlag = _doCheckdoBillingDetailData.IssueInvFlag;
                                    _doUpdateTemptbt_BillingDetail.BillingTypeCode = _doCheckdoBillingDetailData.BillingTypeCode;
                                    _doUpdateTemptbt_BillingDetail.BillingAmount = _doCheckdoBillingDetailData.BillingAmount;
                                    _doUpdateTemptbt_BillingDetail.AdjustBillingAmount = _doCheckdoBillingDetailData.AdjustBillingAmount;
                                    _doUpdateTemptbt_BillingDetail.AdjustBillingAmountUsd = _doCheckdoBillingDetailData.AdjustBillingAmountUsd;
                                    _doUpdateTemptbt_BillingDetail.AdjustBillingAmountCurrencyType = _doCheckdoBillingDetailData.AdjustBillingAmountCurrencyType;
                                    _doUpdateTemptbt_BillingDetail.BillingStartDate = _doCheckdoBillingDetailData.BillingStartDate;
                                    _doUpdateTemptbt_BillingDetail.BillingEndDate = _doCheckdoBillingDetailData.BillingEndDate;
                                    _doUpdateTemptbt_BillingDetail.PaymentMethod = _doCheckdoBillingBasic[0].PaymentMethod;
                                    _doUpdateTemptbt_BillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT;
                                    _doUpdateTemptbt_BillingDetail.AutoTransferDate = dtpIssueDate;
                                    _doUpdateTemptbt_BillingDetail.FirstFeeFlag = _doCheckdoBillingDetailData.FirstFeeFlag;
                                    _doUpdateTemptbt_BillingDetail.DelayedMonth = _doCheckdoBillingDetailData.DelayedMonth;
                                    _doUpdateTemptbt_BillingDetail.StartOperationDate = _doCheckdoBillingBasic[0].StartOperationDate;
                                    _doUpdateTemptbt_BillingDetail.CreateDate = _doCheckdoBillingDetailData.CreateDate;
                                    _doUpdateTemptbt_BillingDetail.CreateBy = _doCheckdoBillingDetailData.CreateBy;
                                    _doUpdateTemptbt_BillingDetail.UpdateDate = _doCheckdoBillingDetailData.UpdateDate;
                                    _doUpdateTemptbt_BillingDetail.UpdateBy = _doCheckdoBillingDetailData.UpdateBy;
                                    _doUpdateTemptbt_BillingDetail.BillingAmountUsd = _doCheckdoBillingDetailData.BillingAmountUsd;
                                    _doUpdateTemptbt_BillingDetail.BillingAmountCurrencyType = _doCheckdoBillingDetailData.BillingAmountCurrencyType;

                                    //Modify by Jutarat A. on 07052013
                                    //_doUpdateTemptbt_BillingDetail = billingHandler.ManageBillingDetail(
                                    //    _doUpdateTemptbt_BillingDetail);
                                    //_doAfterUpdatetbt_BillingDetailList.Add(_doUpdateTemptbt_BillingDetail);
                                    _doUpdatetbt_BillingDetailList.Add(_doUpdateTemptbt_BillingDetail);
                                    //End Modify
                                }
                            }

                            if (rdoProcessType == conModeRadio1rdoStopAutoTransferCreditCard)
                            {
                                foreach (tbt_BillingDetail _doCheckdoBillingDetailData in _doCheckdoBillingDetail)
                                {
                                    tbt_BillingDetail _doUpdateTemptbt_BillingDetail = new tbt_BillingDetail();
                                    _doUpdateTemptbt_BillingDetail.ContractCode = _doCheckdoBillingDetailData.ContractCode;
                                    _doUpdateTemptbt_BillingDetail.BillingOCC = _doCheckdoBillingDetailData.BillingOCC;
                                    _doUpdateTemptbt_BillingDetail.BillingDetailNo = 0;
                                    _doUpdateTemptbt_BillingDetail.InvoiceNo = null;
                                    _doUpdateTemptbt_BillingDetail.InvoiceOCC = null;
                                    _doUpdateTemptbt_BillingDetail.IssueInvDate = System.DateTime.Now;
                                    _doUpdateTemptbt_BillingDetail.IssueInvFlag = _doCheckdoBillingDetailData.IssueInvFlag;
                                    _doUpdateTemptbt_BillingDetail.BillingTypeCode = _doCheckdoBillingDetailData.BillingTypeCode;
                                    _doUpdateTemptbt_BillingDetail.BillingAmount = _doCheckdoBillingDetailData.BillingAmount;
                                    _doUpdateTemptbt_BillingDetail.AdjustBillingAmount = _doCheckdoBillingDetailData.AdjustBillingAmount;
                                    _doUpdateTemptbt_BillingDetail.AdjustBillingAmountUsd = _doCheckdoBillingDetailData.AdjustBillingAmountUsd;
                                    _doUpdateTemptbt_BillingDetail.AdjustBillingAmountCurrencyType = _doCheckdoBillingDetailData.AdjustBillingAmountCurrencyType;
                                    _doUpdateTemptbt_BillingDetail.BillingStartDate = _doCheckdoBillingDetailData.BillingStartDate;
                                    _doUpdateTemptbt_BillingDetail.BillingEndDate = _doCheckdoBillingDetailData.BillingEndDate;
                                    _doUpdateTemptbt_BillingDetail.PaymentMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                                    _doUpdateTemptbt_BillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT;
                                    _doUpdateTemptbt_BillingDetail.AutoTransferDate = null;
                                    _doUpdateTemptbt_BillingDetail.FirstFeeFlag = _doCheckdoBillingDetailData.FirstFeeFlag;
                                    _doUpdateTemptbt_BillingDetail.DelayedMonth = _doCheckdoBillingDetailData.DelayedMonth;
                                    _doUpdateTemptbt_BillingDetail.StartOperationDate = _doCheckdoBillingBasic[0].StartOperationDate;
                                    _doUpdateTemptbt_BillingDetail.CreateDate = _doCheckdoBillingDetailData.CreateDate;
                                    _doUpdateTemptbt_BillingDetail.CreateBy = _doCheckdoBillingDetailData.CreateBy;
                                    _doUpdateTemptbt_BillingDetail.UpdateDate = _doCheckdoBillingDetailData.UpdateDate;
                                    _doUpdateTemptbt_BillingDetail.UpdateBy = _doCheckdoBillingDetailData.UpdateBy;
                                    _doUpdateTemptbt_BillingDetail.ContractOCC = _doCheckdoBillingDetailData.ContractOCC;
                                    _doUpdateTemptbt_BillingDetail.ForceIssueFlag = _doCheckdoBillingDetailData.ForceIssueFlag;
                                    _doUpdateTemptbt_BillingDetail.BillingAmountUsd = _doCheckdoBillingDetailData.BillingAmountUsd;
                                    _doUpdateTemptbt_BillingDetail.BillingAmountCurrencyType = _doCheckdoBillingDetailData.BillingAmountCurrencyType;
                                    //Modify by Jutarat A. on 07052013
                                    //_doUpdateTemptbt_BillingDetail = billingHandler.ManageBillingDetail(
                                    //    _doUpdateTemptbt_BillingDetail);
                                    //_doAfterUpdatetbt_BillingDetailList.Add(_doUpdateTemptbt_BillingDetail);
                                    _doUpdatetbt_BillingDetailList.Add(_doUpdateTemptbt_BillingDetail);
                                    //End Modify
                                }
                            }

                            //Add by Jutarat A. on 07052013
                            if (_doUpdatetbt_BillingDetailList != null && _doUpdatetbt_BillingDetailList.Count > 0)
                                _doAfterUpdatetbt_BillingDetailList.AddRange(billingHandler.ManageBillingDetail(_doUpdatetbt_BillingDetailList));
                            //End Add

                            if (rdoProcessType == conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate)
                            {
                                List<tbt_BillingDetail> doUpdateCheckdoBillingDetailList = new List<tbt_BillingDetail>(); //Add by Jutarat A. on 07052013
                                foreach (tbt_BillingDetail _doCheckdoBillingDetailData in _doCheckdoBillingDetail)
                                {
                                    //_doCheckdoBillingDetailData.ContractCode //same value as load 
                                    //_doCheckdoBillingDetailData.BillingOCC //same value as load 
                                    //_doCheckdoBillingDetailData.BillingDetailNo //same value as load 
                                    //_doCheckdoBillingDetailData.InvoiceNo //same value as load 
                                    //_doCheckdoBillingDetailData.InvoiceOCC //same value as load 


                                    //_doCheckdoBillingDetailData.IssueInvFlag //same value as load 
                                    //_doCheckdoBillingDetailData.BillingTypeCode //same value as load 
                                    //_doCheckdoBillingDetailData.BillingAmount  //same value as load 
                                    //_doCheckdoBillingDetailData.AdjustBillingAmount //same value as load 
                                    //_doCheckdoBillingDetailData.BillingStartDate //same value as load 
                                    //_doCheckdoBillingDetailData.BillingEndDate //same value as load 
                                    //_doCheckdoBillingDetailData.PaymentMethod //same value as load 
                                    //_doCheckdoBillingDetailData.PaymentStatus //same value as load 

                                    if (_doCheckdoBillingDetailData.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                        || _doCheckdoBillingDetailData.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                    {
                                        if (dtpIssueDate.Value.AddDays(-30) < CommonUtil.dsTransData.dtOperationData.ProcessDateTime)
                                        {
                                            _doCheckdoBillingDetailData.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                        }
                                        else
                                        {
                                            _doCheckdoBillingDetailData.IssueInvDate = dtpIssueDate.Value.AddDays(-30);
                                        }

                                        _doCheckdoBillingDetailData.AutoTransferDate = dtpIssueDate;
                                    }
                                    else
                                    {
                                        _doCheckdoBillingDetailData.IssueInvDate = dtpIssueDate;
                                        _doCheckdoBillingDetailData.AutoTransferDate = null;
                                    }

                                    //_doCheckdoBillingDetailData.FirstFeeFlag //same value as load 
                                    //_doCheckdoBillingDetailData.DelayedMonth //same value as load 
                                    //_doCheckdoBillingDetailData.CreateDate //same value as load 
                                    //_doCheckdoBillingDetailData.CreateBy //same value as load 
                                    //_doCheckdoBillingDetailData.UpdateDate //same value as load 
                                    //_doCheckdoBillingDetailData.UpdateBy //same value as load 

                                    //Modify by Jutarat A. on 07052013
                                    //if (billingHandler.Updatetbt_BillingDetail(_doCheckdoBillingDetailData) == 0)
                                    //{
                                    //    // error msg here
                                    //};
                                    //// add for create invoice if check real time
                                    //_doAfterUpdatetbt_BillingDetailList.Add(_doCheckdoBillingDetailData);
                                    doUpdateCheckdoBillingDetailList.Add(_doCheckdoBillingDetailData);
                                    //End Modify
                                }

                                //Add by Jutarat A. on 07052013
                                if (doUpdateCheckdoBillingDetailList != null && doUpdateCheckdoBillingDetailList.Count > 0)
                                    _doAfterUpdatetbt_BillingDetailList.AddRange(billingHandler.Updatetbt_BillingDetail(doUpdateCheckdoBillingDetailList));
                                //End Add

                            }
                            List<tbt_BillingBasic> _Realtbt_BillingBasicList = billingHandler.GetTbt_BillingBasic(
                                comUtil.ConvertBillingCode(RegisterData.Detail1[i].txtContractCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                RegisterData.Detail1[i].txtBillingOCC);

                            List<tbt_BillingDetail> _Realtbt_BillingDetailList = billingHandler.GetTbt_BillingDetailData(
                                comUtil.ConvertBillingCode(RegisterData.Detail1[i].txtContractCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                RegisterData.Detail1[i].txtBillingOCC,
                                Convert.ToInt32(RegisterData.Detail1[i].txtRunningNo));

                            string strRealBillingTargetCode = string.Empty;
                            string strRealBillingTypeCode = string.Empty;
                            string strRealPaymentMethod = string.Empty;

                            if (_Realtbt_BillingBasicList != null)
                            {
                                if (_Realtbt_BillingBasicList.Count > 0)
                                {
                                    strRealBillingTargetCode = _Realtbt_BillingBasicList[0].BillingTargetCode;
                                    strRealBillingTypeCode = _Realtbt_BillingDetailList[0].BillingTypeCode;
                                    strRealPaymentMethod = _Realtbt_BillingBasicList[0].PaymentMethod;
                                }
                            }

                            if (RegisterData.Detail1[i].chkRealTimeIssue)
                            {
                                _doUpdatetbt_Invoice = new tbt_Invoice();
                                if (rdoProcessType == conModeRadio1rdoSwitchToAutoTransferCreditCard)
                                {

                                    _doUpdatetbt_Invoice.InvoiceNo = _doCheckdoBillingDetail[0].InvoiceNo;
                                    //_doUpdatetbt_Invoice.InvoiceOCC = null;
                                    _doUpdatetbt_Invoice.IssueInvDate = System.DateTime.Now;
                                    _doUpdatetbt_Invoice.AutoTransferDate = dtpIssueDate;
                                    _doUpdatetbt_Invoice.BillingTargetCode = strRealBillingTargetCode;
                                    _doUpdatetbt_Invoice.BillingTypeCode = strRealBillingTypeCode;
                                    _doUpdatetbt_Invoice.InvoiceAmount = null;
                                    _doUpdatetbt_Invoice.PaidAmountIncVat = null;
                                    _doUpdatetbt_Invoice.VatRate = null;
                                    _doUpdatetbt_Invoice.VatAmount = null;
                                    _doUpdatetbt_Invoice.WHTRate = null;
                                    _doUpdatetbt_Invoice.WHTAmount = null;
                                    _doUpdatetbt_Invoice.RegisteredWHTAmount = null;
                                    _doUpdatetbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;
                                    _doUpdatetbt_Invoice.IssueInvFlag = true;

                                    _doUpdatetbt_Invoice.FirstIssueInvDate = System.DateTime.Now;
                                    _doUpdatetbt_Invoice.FirstIssueInvFlag = true;
                                    _doUpdatetbt_Invoice.PaymentMethod = strRealPaymentMethod;
                                    _doUpdatetbt_Invoice.CorrectReason = null;
                                    _doUpdatetbt_Invoice.RefOldInvoiceNo = null;
                                    _doUpdatetbt_Invoice.VatAmountUsd = null;
                                    _doUpdatetbt_Invoice.VatAmountCurrencyType = null;
                                    _doUpdatetbt_Invoice.WHTAmountUsd = null;
                                    _doUpdatetbt_Invoice.WHTAmountCurrencyType = null;
                                    _doUpdatetbt_Invoice.InvoiceAmountUsd = null;
                                    _doUpdatetbt_Invoice.InvoiceAmountCurrencyType = null;
                            }
                                else if (rdoProcessType == conModeRadio1rdoStopAutoTransferCreditCard)
                                {
                                    _doUpdatetbt_Invoice.InvoiceNo = _doCheckdoBillingDetail[0].InvoiceNo;
                                    //_doUpdatetbt_Invoice.InvoiceOCC = null;
                                    _doUpdatetbt_Invoice.IssueInvDate = System.DateTime.Now;
                                    _doUpdatetbt_Invoice.AutoTransferDate = null;
                                    _doUpdatetbt_Invoice.BillingTargetCode = strRealBillingTargetCode;
                                    _doUpdatetbt_Invoice.BillingTypeCode = strRealBillingTypeCode;
                                    _doUpdatetbt_Invoice.InvoiceAmount = null;
                                    _doUpdatetbt_Invoice.PaidAmountIncVat = null;
                                    _doUpdatetbt_Invoice.VatRate = null;
                                    _doUpdatetbt_Invoice.VatAmount = null;
                                    _doUpdatetbt_Invoice.WHTRate = null;
                                    _doUpdatetbt_Invoice.WHTAmount = null;
                                    _doUpdatetbt_Invoice.RegisteredWHTAmount = null;
                                    _doUpdatetbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                                    _doUpdatetbt_Invoice.IssueInvFlag = true;

                                    _doUpdatetbt_Invoice.FirstIssueInvDate = System.DateTime.Now;
                                    _doUpdatetbt_Invoice.FirstIssueInvFlag = true;
                                    _doUpdatetbt_Invoice.PaymentMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                                    _doUpdatetbt_Invoice.CorrectReason = null;
                                    _doUpdatetbt_Invoice.RefOldInvoiceNo = null;

                                    _doUpdatetbt_Invoice.VatAmountUsd = null;
                                    _doUpdatetbt_Invoice.VatAmountCurrencyType = null;
                                    _doUpdatetbt_Invoice.WHTAmountUsd = null;
                                    _doUpdatetbt_Invoice.WHTAmountCurrencyType = null;
                                    _doUpdatetbt_Invoice.InvoiceAmountUsd = null;
                                    _doUpdatetbt_Invoice.InvoiceAmountCurrencyType = null;
                            }
                                else //rdoProcessType == conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate
                                {
                                    _doUpdatetbt_Invoice.InvoiceNo = _doCheckdoBillingDetail[0].InvoiceNo;
                                    //_doUpdatetbt_Invoice.InvoiceOCC = null;
                                    _doUpdatetbt_Invoice.IssueInvDate = System.DateTime.Now;
                                    if (_doCheckdoBillingDetail[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                        || _doCheckdoBillingDetail[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                    {
                                        _doUpdatetbt_Invoice.AutoTransferDate = dtpIssueDate;
                                        _doUpdatetbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;
                                    }
                                    else
                                    {
                                        _doUpdatetbt_Invoice.AutoTransferDate = null;
                                        _doUpdatetbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                                    }
                                    _doUpdatetbt_Invoice.BillingTargetCode = strRealBillingTargetCode;
                                    _doUpdatetbt_Invoice.BillingTypeCode = _doCheckdoBillingDetail[0].BillingTypeCode;
                                    _doUpdatetbt_Invoice.InvoiceAmount = null;
                                    _doUpdatetbt_Invoice.PaidAmountIncVat = null;
                                    _doUpdatetbt_Invoice.VatRate = null;
                                    _doUpdatetbt_Invoice.VatAmount = null;
                                    _doUpdatetbt_Invoice.WHTRate = null;
                                    _doUpdatetbt_Invoice.WHTAmount = null;
                                    _doUpdatetbt_Invoice.RegisteredWHTAmount = null;
                                    _doUpdatetbt_Invoice.IssueInvFlag = true;
                                    _doUpdatetbt_Invoice.FirstIssueInvDate = System.DateTime.Now;
                                    _doUpdatetbt_Invoice.FirstIssueInvFlag = true;
                                    _doUpdatetbt_Invoice.PaymentMethod = _doCheckdoBillingDetail[0].PaymentMethod;
                                    _doUpdatetbt_Invoice.CorrectReason = null;
                                    _doUpdatetbt_Invoice.RefOldInvoiceNo = null;
                                    _doUpdatetbt_Invoice.VatAmountUsd = null;
                                    _doUpdatetbt_Invoice.VatAmountCurrencyType = null;
                                    _doUpdatetbt_Invoice.WHTAmountUsd = null;
                                    _doUpdatetbt_Invoice.WHTAmountCurrencyType = null;
                                    _doUpdatetbt_Invoice.InvoiceAmountUsd = null;
                                    _doUpdatetbt_Invoice.InvoiceAmountCurrencyType = null;
                            }

                                _doUpdatetbt_Invoice = billingHandler.ManageInvoiceByCommand(
                                    _doUpdatetbt_Invoice
                                    , _doAfterUpdatetbt_BillingDetailList
                                    , true
                                    , false);

                                //RegisterData.strFilePath = _doUpdatetbt_Invoice.FilePath;
                                if (_doUpdatetbt_Invoice.FilePath != null)
                                {
                                    lstFilePath.Add(_doUpdatetbt_Invoice.FilePath);
                                }
                            }
                            //else if (!(String.IsNullOrEmpty(_doCheckdoBillingDetail[0].InvoiceNo)))
                            //{
                            //    _doUpdatetbt_Invoice = new tbt_Invoice();
                            //    if (rdoProcessType == conModeRadio1rdoSwitchToAutoTransferCreditCard)
                            //    {

                            //        _doUpdatetbt_Invoice.InvoiceNo = _doCheckdoBillingDetail[0].InvoiceNo;
                            //        //_doUpdatetbt_Invoice.InvoiceOCC = null;
                            //        _doUpdatetbt_Invoice.IssueInvDate = System.DateTime.Now;
                            //        _doUpdatetbt_Invoice.AutoTransferDate = dtpIssueDate;
                            //        _doUpdatetbt_Invoice.BillingTargetCode = strRealBillingTargetCode;
                            //        _doUpdatetbt_Invoice.BillingTypeCode = strRealBillingTypeCode;
                            //        _doUpdatetbt_Invoice.InvoiceAmount = null;
                            //        _doUpdatetbt_Invoice.PaidAmountIncVat = null;
                            //        _doUpdatetbt_Invoice.VatRate = null;
                            //        _doUpdatetbt_Invoice.VatAmount = null;
                            //        _doUpdatetbt_Invoice.WHTRate = null;
                            //        _doUpdatetbt_Invoice.WHTAmount = null;
                            //        _doUpdatetbt_Invoice.RegisteredWHTAmount = null;
                            //        _doUpdatetbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;
                            //        _doUpdatetbt_Invoice.IssueInvFlag = false;
                            //        _doUpdatetbt_Invoice.FirstIssueInvDate = null;
                            //        _doUpdatetbt_Invoice.FirstIssueInvFlag = false;
                            //        _doUpdatetbt_Invoice.PaymentMethod = strRealPaymentMethod;
                            //        _doUpdatetbt_Invoice.CorrectReason = null;
                            //        _doUpdatetbt_Invoice.RefOldInvoiceNo = null;
                            //    }
                            //    else if (rdoProcessType == conModeRadio1rdoStopAutoTransferCreditCard)
                            //    {
                            //        _doUpdatetbt_Invoice.InvoiceNo = _doCheckdoBillingDetail[0].InvoiceNo;
                            //        //_doUpdatetbt_Invoice.InvoiceOCC = null;
                            //        _doUpdatetbt_Invoice.IssueInvDate = System.DateTime.Now;
                            //        _doUpdatetbt_Invoice.AutoTransferDate = null;
                            //        _doUpdatetbt_Invoice.BillingTargetCode = strRealBillingTargetCode;
                            //        _doUpdatetbt_Invoice.BillingTypeCode = strRealBillingTypeCode;
                            //        _doUpdatetbt_Invoice.InvoiceAmount = null;
                            //        _doUpdatetbt_Invoice.PaidAmountIncVat = null;
                            //        _doUpdatetbt_Invoice.VatRate = null;
                            //        _doUpdatetbt_Invoice.VatAmount = null;
                            //        _doUpdatetbt_Invoice.WHTRate = null;
                            //        _doUpdatetbt_Invoice.WHTAmount = null;
                            //        _doUpdatetbt_Invoice.RegisteredWHTAmount = null;
                            //        _doUpdatetbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                            //        _doUpdatetbt_Invoice.IssueInvFlag = false;
                            //        _doUpdatetbt_Invoice.FirstIssueInvDate = null;
                            //        _doUpdatetbt_Invoice.FirstIssueInvFlag = false;
                            //        _doUpdatetbt_Invoice.PaymentMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                            //        _doUpdatetbt_Invoice.CorrectReason = null;
                            //        _doUpdatetbt_Invoice.RefOldInvoiceNo = null;
                            //    }

                            //    _doUpdatetbt_Invoice = billingHandler.ManageInvoiceByCommand(
                            //        _doUpdatetbt_Invoice
                            //        , _doAfterUpdatetbt_BillingDetailList
                            //        , false
                            //        , false);
                            //}
                        }
                    }

                    string mergeOutputFilename = string.Empty;
                    string encryptOutputFileName = string.Empty;

                    if (lstFilePath.Count > 0)
                    {
                        mergeOutputFilename = PathUtil.GetTempFileName(".pdf");
                        encryptOutputFileName = PathUtil.GetTempFileName(".pdf");
                        bool isSuccess = ReportUtil.MergePDF(lstFilePath.ToArray(), mergeOutputFilename, true, encryptOutputFileName, null);
                    }

                    RegisterData.strFilePath = encryptOutputFileName;

                    param.RegisterData = RegisterData;

                // == COMMIT TRANSACTION ==
                scope.Complete();

                if (res.MessageList == null || res.MessageList.Count == 0)
                    { res.ResultData = "1"; }
                    else
                    { res.ResultData = "0"; }
                }
                catch (Exception ex)
                {
                // == ROLLBANK TRANSACTION ==
                scope.Dispose();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.ResultData = "0";
                    res.AddErrorMessage(ex);
                }
            }
            return res;

        }

        /// <summary>
        /// register data into database mode by invoice
        /// </summary>
        /// <returns></returns>
        public ObjectResultData BLS060_ByInvoice(BLS060_RegisterData RegisterData)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            string conModeRadio1rdoSwitchToAutoTransferCreditCard = "1";
            string conModeRadio1rdoStopAutoTransferCreditCard = "2";
            string conModeRadio1rdoChangeExpectedIssueDateAutoTransferDate = "3";

            string rdoProcessType = RegisterData.Header.rdoProcessType;
            string rdoProcessUnit = RegisterData.Header.rdoProcessUnit;

            string txtInvoiceNo = "";
            DateTime? dtpAutoTransferDate = null;
            bool chkRealTimeIssue = false;

            using (TransactionScope scope = new TransactionScope())
            {
            try
            {
                    IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                    ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    BLS060_ScreenParameter param = GetScreenObject<BLS060_ScreenParameter>();

                    for (int i = 0; i < RegisterData.Detail2.Count; i++)
                    {
                        List<tbt_BillingDetail> _doCheckdoBillingDetail = new List<tbt_BillingDetail>();
                        List<tbt_BillingBasic> _doCheckdoBillingBasic = new List<tbt_BillingBasic>();
                        List<doGetUnpaidInvoiceData> _doCheckdoGetUnpaidInvoiceData = new List<doGetUnpaidInvoiceData>();

                        List<doGetBillingDetailOfInvoice> _doCheckdoGetBillingDetailOfInvoice = new List<doGetBillingDetailOfInvoice>();

                        List<tbt_BillingDetail> _doUpdatetbt_BillingDetailList = new List<tbt_BillingDetail>();
                        List<tbt_BillingDetail> _doAfterUpdatetbt_BillingDetailList = new List<tbt_BillingDetail>();
                        List<tbt_BillingDetail> doManagetbt_BillingDetailList = new List<tbt_BillingDetail>(); //Add by Jutarat A. on 07052013

                        // Do Business 
                        tbt_Invoice _doUpdatetbt_Invoice = new tbt_Invoice();

                        if (!(String.IsNullOrEmpty(RegisterData.Detail2[i].txtInvoiceNo)))
                        {
                            txtInvoiceNo = RegisterData.Detail2[i].txtInvoiceNo;
                            dtpAutoTransferDate = RegisterData.Detail2[i].dtpAutoTransferDate;
                            chkRealTimeIssue = RegisterData.Detail2[i].chkRealTimeIssue;

                            //if (RegisterData.Detail2[i].chkRealTimeIssue)

                            //get current invoice for cancel

                            _doCheckdoGetUnpaidInvoiceData = billingHandler.GetUnpaidInvoiceDataList(txtInvoiceNo);

                            if (_doCheckdoGetUnpaidInvoiceData != null && _doCheckdoGetUnpaidInvoiceData.Count > 0)
                            {
                                _doUpdatetbt_Invoice.InvoiceNo = _doCheckdoGetUnpaidInvoiceData[0].InvoiceNo;
                                _doUpdatetbt_Invoice.InvoiceOCC = _doCheckdoGetUnpaidInvoiceData[0].InvoiceOCC;
                                _doUpdatetbt_Invoice.IssueInvDate = _doCheckdoGetUnpaidInvoiceData[0].IssueInvDate;
                                //_doUpdatetbt_Invoice.AutoTransferDate = _doCheckdoGetUnpaidInvoiceData[0].AutoTransferDate;
                                _doUpdatetbt_Invoice.BillingTargetCode = _doCheckdoGetUnpaidInvoiceData[0].BillingTargetCode;
                                //_doUpdatetbt_Invoice.BillingTypeCode = _doCheckdoGetUnpaidInvoiceData[0].BillingTypeCode;
                                _doUpdatetbt_Invoice.InvoiceAmount = _doCheckdoGetUnpaidInvoiceData[0].InvoiceAmount;
                                _doUpdatetbt_Invoice.PaidAmountIncVat = _doCheckdoGetUnpaidInvoiceData[0].PaidAmountIncVat;
                                _doUpdatetbt_Invoice.VatRate = _doCheckdoGetUnpaidInvoiceData[0].VatRate;
                                _doUpdatetbt_Invoice.VatAmount = _doCheckdoGetUnpaidInvoiceData[0].VatAmount;
                                _doUpdatetbt_Invoice.WHTRate = _doCheckdoGetUnpaidInvoiceData[0].WHTRate;
                                _doUpdatetbt_Invoice.WHTAmount = _doCheckdoGetUnpaidInvoiceData[0].WHTAmount;
                                _doUpdatetbt_Invoice.RegisteredWHTAmount = _doCheckdoGetUnpaidInvoiceData[0].RegisteredWHTAmount;
                                _doUpdatetbt_Invoice.InvoicePaymentStatus = _doCheckdoGetUnpaidInvoiceData[0].InvoicePaymentStatus;
                               // _doUpdatetbt_Invoice.IssueInvFlag = _doCheckdoGetUnpaidInvoiceData[0].IssueInvFlag;
                                _doUpdatetbt_Invoice.VatAmountUsd = _doCheckdoGetUnpaidInvoiceData[0].VatAmountUsd;
                                _doUpdatetbt_Invoice.VatAmountCurrencyType = _doCheckdoGetUnpaidInvoiceData[0].VatAmountCurrencyType;
                                _doUpdatetbt_Invoice.WHTAmountUsd = _doCheckdoGetUnpaidInvoiceData[0].WHTAmountUsd;
                                _doUpdatetbt_Invoice.WHTAmountCurrencyType = _doCheckdoGetUnpaidInvoiceData[0].WHTAmountCurrencyType;

                                _doUpdatetbt_Invoice.FirstIssueInvDate = _doCheckdoGetUnpaidInvoiceData[0].FirstIssueInvDate;
                                _doUpdatetbt_Invoice.FirstIssueInvFlag = _doCheckdoGetUnpaidInvoiceData[0].FirstIssueInvFlag;
                                _doUpdatetbt_Invoice.PaymentMethod = _doCheckdoGetUnpaidInvoiceData[0].PaymentMethod;
                                _doUpdatetbt_Invoice.CorrectReason = _doCheckdoGetUnpaidInvoiceData[0].CorrectReason;
                                _doUpdatetbt_Invoice.RefOldInvoiceNo = _doCheckdoGetUnpaidInvoiceData[0].RefOldInvoiceNo;
                                _doUpdatetbt_Invoice.InvoiceAmountUsd = _doCheckdoGetUnpaidInvoiceData[0].InvoiceAmountUsd;
                                _doUpdatetbt_Invoice.InvoiceAmountCurrencyType = _doCheckdoGetUnpaidInvoiceData[0].InvoiceAmountCurrencyType;
                            // get details of current invoice

                            _doCheckdoGetBillingDetailOfInvoice = billingHandler.GetBillingDetailOfInvoiceList(
                                    _doCheckdoGetUnpaidInvoiceData[0].InvoiceNo
                                    , _doCheckdoGetUnpaidInvoiceData[0].InvoiceOCC);

                                if (_doCheckdoGetBillingDetailOfInvoice != null && _doCheckdoGetBillingDetailOfInvoice.Count > 0)
                                {

                                    foreach (doGetBillingDetailOfInvoice _doCheckdoGetBillingDetailOfInvoiceData in _doCheckdoGetBillingDetailOfInvoice)
                                    {
                                        _doCheckdoBillingBasic = billingHandler.GetTbt_BillingBasic(
                                                    _doCheckdoGetBillingDetailOfInvoiceData.ContractCode
                                                    , _doCheckdoGetBillingDetailOfInvoiceData.BillingOCC);
                                        tbt_BillingDetail _doUpdateTemptbt_BillingDetail = new tbt_BillingDetail();
                                        _doUpdateTemptbt_BillingDetail.ContractCode = _doCheckdoGetBillingDetailOfInvoiceData.ContractCode;
                                        _doUpdateTemptbt_BillingDetail.BillingOCC = _doCheckdoGetBillingDetailOfInvoiceData.BillingOCC;
                                        _doUpdateTemptbt_BillingDetail.BillingDetailNo = _doCheckdoGetBillingDetailOfInvoiceData.BillingDetailNo;
                                        _doUpdateTemptbt_BillingDetail.InvoiceNo = _doCheckdoGetBillingDetailOfInvoiceData.InvoiceNo;
                                        _doUpdateTemptbt_BillingDetail.InvoiceOCC = _doCheckdoGetBillingDetailOfInvoiceData.InvoiceOCC;
                                        _doUpdateTemptbt_BillingDetail.IssueInvDate = _doCheckdoGetBillingDetailOfInvoiceData.IssueInvDate;
                                        _doUpdateTemptbt_BillingDetail.IssueInvFlag = _doCheckdoGetBillingDetailOfInvoiceData.IssueInvFlag;
                                        _doUpdateTemptbt_BillingDetail.BillingTypeCode = _doCheckdoGetBillingDetailOfInvoiceData.BillingTypeCode;
                                        _doUpdateTemptbt_BillingDetail.BillingAmount = _doCheckdoGetBillingDetailOfInvoiceData.BillingAmount;
                                        _doUpdateTemptbt_BillingDetail.AdjustBillingAmount = _doCheckdoGetBillingDetailOfInvoiceData.AdjustBillingAmount;
                                        _doUpdateTemptbt_BillingDetail.AdjustBillingAmountUsd = _doCheckdoGetBillingDetailOfInvoiceData.AdjustBillingAmountUsd;
                                        _doUpdateTemptbt_BillingDetail.AdjustBillingAmountCurrencyType = _doCheckdoGetBillingDetailOfInvoiceData.AdjustBillingAmountCurrencyType;
                                        _doUpdateTemptbt_BillingDetail.BillingStartDate = _doCheckdoGetBillingDetailOfInvoiceData.BillingStartDate;
                                        _doUpdateTemptbt_BillingDetail.BillingEndDate = _doCheckdoGetBillingDetailOfInvoiceData.BillingEndDate;
                                        _doUpdateTemptbt_BillingDetail.PaymentMethod = _doCheckdoGetBillingDetailOfInvoiceData.PaymentMethod;
                                        _doUpdateTemptbt_BillingDetail.PaymentStatus = _doCheckdoGetBillingDetailOfInvoiceData.PaymentStatus;
                                        _doUpdateTemptbt_BillingDetail.AutoTransferDate = _doCheckdoGetBillingDetailOfInvoiceData.AutoTransferDate;
                                        _doUpdateTemptbt_BillingDetail.FirstFeeFlag = _doCheckdoGetBillingDetailOfInvoiceData.FirstFeeFlag;
                                        _doUpdateTemptbt_BillingDetail.DelayedMonth = _doCheckdoGetBillingDetailOfInvoiceData.DelayedMonth;
                                        _doUpdateTemptbt_BillingDetail.StartOperationDate = _doCheckdoBillingBasic[0].StartOperationDate;
                                        //_doUpdateTemptbt_BillingDetail.CreateDate = _doCheckdoGetBillingDetailOfInvoiceData.CreateDate;
                                        //_doUpdateTemptbt_BillingDetail.CreateBy = _doCheckdoGetBillingDetailOfInvoiceData.CreateBy;
                                        //_doUpdateTemptbt_BillingDetail.UpdateDate = _doCheckdoGetBillingDetailOfInvoiceData.UpdateDate;
                                        //_doUpdateTemptbt_BillingDetail.UpdateBy = _doCheckdoGetBillingDetailOfInvoiceData.UpdateBy;
                                        _doUpdateTemptbt_BillingDetail.ContractOCC = _doCheckdoGetBillingDetailOfInvoiceData.ContractOCC;
                                        _doUpdateTemptbt_BillingDetail.ForceIssueFlag = _doCheckdoGetBillingDetailOfInvoiceData.ForceIssueFlag;
                                        _doUpdateTemptbt_BillingDetail.BillingAmountUsd = _doCheckdoGetBillingDetailOfInvoiceData.BillingAmountUsd;
                                        _doUpdateTemptbt_BillingDetail.BillingAmountCurrencyType = _doCheckdoGetBillingDetailOfInvoiceData.BillingAmountCurrencyType;
                                        _doUpdatetbt_BillingDetailList.Add(_doUpdateTemptbt_BillingDetail);
                                    }
                                }
                            }
                            // Update Invoice and Billing Deatils to Cancel
                            if (billingHandler.UpdateInvoicePaymentStatus(
                                _doUpdatetbt_Invoice
                                , _doUpdatetbt_BillingDetailList
                                , PaymentStatus.C_PAYMENT_STATUS_CANCEL))
                            {
                                // error msg here
                            }
                            // create new Billing Details
                            foreach (tbt_BillingDetail _doUpdatetbt_BillingDetail in _doUpdatetbt_BillingDetailList)
                            {
                                _doCheckdoBillingBasic = billingHandler.GetTbt_BillingBasic(
                                                            _doUpdatetbt_BillingDetail.ContractCode
                                                            , _doUpdatetbt_BillingDetail.BillingOCC);

                                if (rdoProcessType == conModeRadio1rdoSwitchToAutoTransferCreditCard)
                                {
                                    //_doUpdatetbt_BillingDetail.ContractCode //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.BillingOCC //same value as load for cancel

                                    //_doUpdatetbt_BillingDetail.BillingDetailNo = null;
                                    _doUpdatetbt_BillingDetail.InvoiceNo = null;
                                    _doUpdatetbt_BillingDetail.InvoiceOCC = null;
                                    _doUpdatetbt_BillingDetail.IssueInvDate = System.DateTime.Now;
                                    //_doUpdatetbt_BillingDetail.IssueInvFlag	//same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.BillingTypeCode //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.BillingAmount //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.AdjustBillingAmount //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.BillingStartDate //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.BillingEndDate //same value as load for cancel
                                    _doUpdatetbt_BillingDetail.PaymentMethod = _doCheckdoBillingBasic[0].PaymentMethod;
                                    _doUpdatetbt_BillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT;
                                    _doUpdatetbt_BillingDetail.AutoTransferDate = dtpAutoTransferDate;
                                    //_doUpdatetbt_BillingDetail.FirstFeeFlag  //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.DelayedMonth //same value as load for cancel
                                    _doUpdatetbt_BillingDetail.StartOperationDate = _doCheckdoBillingBasic[0].StartOperationDate;

                                }
                                else if (rdoProcessType == conModeRadio1rdoStopAutoTransferCreditCard)
                                {

                                    //_doUpdatetbt_BillingDetail.ContractCode //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.BillingOCC //same value as load for cancel

                                    //_doUpdatetbt_BillingDetail.BillingDetailNo = null;
                                    _doUpdatetbt_BillingDetail.InvoiceNo = null;
                                    _doUpdatetbt_BillingDetail.InvoiceOCC = null;
                                    _doUpdatetbt_BillingDetail.IssueInvDate = System.DateTime.Now;
                                    //_doUpdatetbt_BillingDetail.IssueInvFlag	//same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.BillingTypeCode //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.BillingAmount //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.AdjustBillingAmount //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.BillingStartDate //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.BillingEndDate //same value as load for cancel
                                    _doUpdatetbt_BillingDetail.PaymentMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                                    _doUpdatetbt_BillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT;
                                    _doUpdatetbt_BillingDetail.AutoTransferDate = null;
                                    //_doUpdatetbt_BillingDetail.FirstFeeFlag  //same value as load for cancel
                                    //_doUpdatetbt_BillingDetail.DelayedMonth //same value as load for cancel
                                    _doUpdatetbt_BillingDetail.StartOperationDate = _doCheckdoBillingBasic[0].StartOperationDate;

                                }

                                //Modify by Jutarat A. on 07052013
                                //// Create New Billing Details
                                //_doAfterUpdatetbt_BillingDetailList.Add(
                                //    billingHandler.ManageBillingDetail(_doUpdatetbt_BillingDetail)
                                //    );
                                doManagetbt_BillingDetailList.Add(_doUpdatetbt_BillingDetail);
                                //End Modify
                            }

                            //Add by Jutarat A. on 07052013
                            if (doManagetbt_BillingDetailList != null && doManagetbt_BillingDetailList.Count > 0)
                                _doAfterUpdatetbt_BillingDetailList.AddRange(billingHandler.ManageBillingDetail(doManagetbt_BillingDetailList));
                            //End Add

                            if (RegisterData.Detail2[i].chkRealTimeIssue)
                            {

                                if (rdoProcessType == conModeRadio1rdoSwitchToAutoTransferCreditCard)
                                {
                                    //_doUpdatetbt_Invoice.InvoiceNo //same value as load for cancel
                                    //_doUpdatetbt_Invoice.InvoiceOCC //same value as load for cancel
                                    _doUpdatetbt_Invoice.IssueInvDate = System.DateTime.Now;
                                    _doUpdatetbt_Invoice.AutoTransferDate = dtpAutoTransferDate;
                                    _doUpdatetbt_Invoice.BillingTargetCode = _doCheckdoGetUnpaidInvoiceData[0].BillingTargetCode;
                                    _doUpdatetbt_Invoice.BillingTypeCode = _doUpdatetbt_BillingDetailList[0].BillingTypeCode;
                                    _doUpdatetbt_Invoice.InvoiceAmount = null;
                                    _doUpdatetbt_Invoice.PaidAmountIncVat = null;
                                    _doUpdatetbt_Invoice.VatRate = null;
                                    _doUpdatetbt_Invoice.VatAmount = null;
                                    _doUpdatetbt_Invoice.WHTRate = null;
                                    _doUpdatetbt_Invoice.WHTAmount = null;
                                    _doUpdatetbt_Invoice.RegisteredWHTAmount = null;
                                    _doUpdatetbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;
                                    _doUpdatetbt_Invoice.IssueInvFlag = true;
                                    _doUpdatetbt_Invoice.FirstIssueInvDate = System.DateTime.Now;
                                    _doUpdatetbt_Invoice.FirstIssueInvFlag = true;
                                    _doUpdatetbt_Invoice.PaymentMethod = _doCheckdoBillingBasic[0].PaymentMethod;
                                    _doUpdatetbt_Invoice.CorrectReason = null;
                                    _doUpdatetbt_Invoice.VatAmountUsd = null;
                                    _doUpdatetbt_Invoice.VatAmountCurrencyType = null;
                                    _doUpdatetbt_Invoice.WHTAmountUsd = null;
                                    _doUpdatetbt_Invoice.WHTAmountCurrencyType = null;
                                    _doUpdatetbt_Invoice.InvoiceAmountUsd = null;
                                    _doUpdatetbt_Invoice.InvoiceAmountCurrencyType = null;
                                //_doUpdatetbt_Invoice.RefOldInvoiceNo //same value as load for cancel
                            }
                                else if (rdoProcessType == conModeRadio1rdoStopAutoTransferCreditCard)
                                {
                                    //_doUpdatetbt_Invoice.InvoiceNo //same value as load for cancel
                                    //_doUpdatetbt_Invoice.InvoiceOCC //same value as load for cancel
                                    _doUpdatetbt_Invoice.IssueInvDate = System.DateTime.Now; ;
                                    _doUpdatetbt_Invoice.AutoTransferDate = null;
                                    //_doUpdatetbt_Invoice.BillingTargetCode //same value as load for cancel
                                    _doUpdatetbt_Invoice.BillingTypeCode = _doUpdatetbt_BillingDetailList[0].BillingTypeCode;
                                    _doUpdatetbt_Invoice.InvoiceAmount = null;
                                    _doUpdatetbt_Invoice.PaidAmountIncVat = null;
                                    _doUpdatetbt_Invoice.VatRate = null;
                                    _doUpdatetbt_Invoice.VatAmount = null;
                                    _doUpdatetbt_Invoice.WHTRate = null;
                                    _doUpdatetbt_Invoice.WHTAmount = null;
                                    _doUpdatetbt_Invoice.RegisteredWHTAmount = null;
                                    _doUpdatetbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                                    _doUpdatetbt_Invoice.IssueInvFlag = true;
                                    _doUpdatetbt_Invoice.FirstIssueInvDate = System.DateTime.Now;
                                    _doUpdatetbt_Invoice.FirstIssueInvFlag = true;
                                    _doUpdatetbt_Invoice.PaymentMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                                    _doUpdatetbt_Invoice.CorrectReason = null;
                                    _doUpdatetbt_Invoice.VatAmountUsd = null;
                                    _doUpdatetbt_Invoice.VatAmountCurrencyType = null;
                                    _doUpdatetbt_Invoice.WHTAmountUsd = null;
                                    _doUpdatetbt_Invoice.WHTAmountCurrencyType = null;
                                    _doUpdatetbt_Invoice.InvoiceAmountUsd = null;
                                    _doUpdatetbt_Invoice.InvoiceAmountCurrencyType = null;
                                //_doUpdatetbt_Invoice.RefOldInvoiceNo //same value as load for cancel
                            }

                                _doUpdatetbt_Invoice = billingHandler.ManageInvoiceByCommand(
                                    _doUpdatetbt_Invoice
                                    , _doAfterUpdatetbt_BillingDetailList
                                    , true
                                    , false);

                                //RegisterData.Detail2[0].strInvoiceno = tempdotbt_Invoice.InvoiceNo;
                                //RegisterData.Detail2[0].strInvoiceOCC = tempdotbt_Invoice.InvoiceOCC.ToString();
                                RegisterData.strFilePath = _doUpdatetbt_Invoice.FilePath;
                                param.RegisterData = RegisterData;
                            }
                            //else
                            //{
                            //    if (rdoProcessType == conModeRadio1rdoSwitchToAutoTransferCreditCard)
                            //    {
                            //        //_doUpdatetbt_Invoice.InvoiceNo //same value as load for cancel
                            //        //_doUpdatetbt_Invoice.InvoiceOCC //same value as load for cancel
                            //        _doUpdatetbt_Invoice.IssueInvDate = System.DateTime.Now;
                            //        _doUpdatetbt_Invoice.AutoTransferDate = dtpAutoTransferDate;
                            //        //_doUpdatetbt_Invoice.BillingTargetCode //same value as load for cancel
                            //        _doUpdatetbt_Invoice.BillingTypeCode = _doUpdatetbt_BillingDetailList[0].BillingTypeCode;
                            //        _doUpdatetbt_Invoice.InvoiceAmount = null;
                            //        _doUpdatetbt_Invoice.PaidAmountIncVat = null;
                            //        _doUpdatetbt_Invoice.VatRate = null;
                            //        _doUpdatetbt_Invoice.VatAmount = null;
                            //        _doUpdatetbt_Invoice.WHTRate = null;
                            //        _doUpdatetbt_Invoice.WHTAmount = null;
                            //        _doUpdatetbt_Invoice.RegisteredWHTAmount = null;
                            //        _doUpdatetbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;
                            //        _doUpdatetbt_Invoice.IssueInvFlag = false;
                            //        _doUpdatetbt_Invoice.FirstIssueInvDate = null;
                            //        _doUpdatetbt_Invoice.FirstIssueInvFlag = false;
                            //        _doUpdatetbt_Invoice.PaymentMethod = _doCheckdoBillingBasic[0].PaymentMethod;
                            //        _doUpdatetbt_Invoice.CorrectReason = null;
                            //        //_doUpdatetbt_Invoice.RefOldInvoiceNo //same value as load for cancel
                            //    }
                            //    else if (rdoProcessType == conModeRadio1rdoStopAutoTransferCreditCard)
                            //    {
                            //        //_doUpdatetbt_Invoice.InvoiceNo //same value as load for cancel
                            //        //_doUpdatetbt_Invoice.InvoiceOCC //same value as load for cancel
                            //        _doUpdatetbt_Invoice.IssueInvDate = System.DateTime.Now;
                            //        _doUpdatetbt_Invoice.AutoTransferDate = null;
                            //        //_doUpdatetbt_Invoice.BillingTargetCode //same value as load for cancel
                            //        _doUpdatetbt_Invoice.BillingTypeCode = _doUpdatetbt_BillingDetailList[0].BillingTypeCode;
                            //        _doUpdatetbt_Invoice.InvoiceAmount = null;
                            //        _doUpdatetbt_Invoice.PaidAmountIncVat = null;
                            //        _doUpdatetbt_Invoice.VatRate = null;
                            //        _doUpdatetbt_Invoice.VatAmount = null;
                            //        _doUpdatetbt_Invoice.WHTRate = null;
                            //        _doUpdatetbt_Invoice.WHTAmount = null;
                            //        _doUpdatetbt_Invoice.RegisteredWHTAmount = null;
                            //        _doUpdatetbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                            //        _doUpdatetbt_Invoice.IssueInvFlag = false;
                            //        _doUpdatetbt_Invoice.FirstIssueInvDate = null;
                            //        _doUpdatetbt_Invoice.FirstIssueInvFlag = false;
                            //        _doUpdatetbt_Invoice.PaymentMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                            //        _doUpdatetbt_Invoice.CorrectReason = null;
                            //        //_doUpdatetbt_Invoice.RefOldInvoiceNo //same value as load for cancel
                            //    }

                            //    _doUpdatetbt_Invoice = billingHandler.ManageInvoiceByCommand(
                            //        _doUpdatetbt_Invoice
                            //        , _doAfterUpdatetbt_BillingDetailList
                            //        , false
                            //        , false);

                            //}
                            //if (!(String.IsNullOrEmpty(RegisterData.Detail2[i].txtInvoiceNo)))

                        }

                        //for (int i = 0; i < RegisterData.Detail2.Count; i++)

                    }

                // == COMMIT TRANSACTION ==
                scope.Complete();

                if (res.MessageList == null || res.MessageList.Count == 0)
                    { res.ResultData = "1"; }
                    else
                    { res.ResultData = "0"; }
                }
                catch (Exception ex)
                {
                // == ROLLBANK TRANSACTION ==
                scope.Dispose();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.ResultData = "0";
                    res.AddErrorMessage(ex);
                }
            }

            return res;
        }

        /// <summary>
        /// Mothod for download document (PDF) and write history to download log
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult BLS060_GetInvoiceReport(string fileName)
        {

            try
            {
                // doDocumentDownloadLog
                //doDocumentDownloadLog cond = new doDocumentDownloadLog();
                //cond.DocumentNo = strDocumentNo;
                //cond.DocumentCode = strDocumentCode;
                //cond.DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //cond.DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                //cond.DocumentOCC = documentOCC;

                //ILogHandler handlerLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                //int isOK = handlerLog.WriteDocumentDownloadLog(cond);

                Stream reportStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                return File(reportStream, "application/pdf");

            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }
    }

}
