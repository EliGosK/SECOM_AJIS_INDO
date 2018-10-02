
//*********************************
// Create by: Teerapong
// Create date: 2/Nov/2011
// Update date: 2/Nov/2011
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
        /// Check permission for access screen BLS030
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS030_Authority(BLS030_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            CommonUtil cm = new CommonUtil();

            if (chandler.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGIST_BILL_BASIC, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            if (param.CallerScreenID != ScreenID.C_SCREEN_ID_REGIST_BILL_TARGET)
            {
                param.ContractProjectCodeShort = "";
                param.BillingClientCode = "";
                param.BillingTargetRunningNo = "";
            }



            return InitialScreenEnvironment<BLS030_ScreenParameter>("BLS030", param, res);

        }

        /// <summary>
        /// Method for return view of screen BLS030
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS030")]
        public ActionResult BLS030()
        {

            BLS030_ScreenParameter param = GetScreenObject<BLS030_ScreenParameter>();
            CommonUtil cm = new CommonUtil();

            // Narupon W.
            if (param != null)
            {

                ViewBag.ContractProjectCode = param.ContractProjectCodeShort;
                ViewBag.BillingClientCode = param.BillingClientCode;
                ViewBag.BillingTargetRunningNo = param.BillingTargetRunningNo;
            }

            if (CheckUserPermission(ScreenID.C_SCREEN_ID_REGIST_BILL_BASIC, FunctionID.C_FUNC_ID_REGISTER_AUTO_TRANSFER))
            {
                ViewBag.HaveAutoTransfer = true;
            }
            else
            {
                ViewBag.HaveAutoTransfer = false;
            }
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_REGIST_BILL_BASIC, FunctionID.C_FUNC_ID_REGISTER_CREDIT_CARD))
            {
                ViewBag.HaveCreditCard = true;
            }
            else
            {
                ViewBag.HaveCreditCard = false;
            }
            return View();
        }


        /// <summary>
        /// Initial grid of screen BLS030 (billing type grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS030_InitialGridBillingType()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS030_BillingType", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Retrieve billing basic data
        /// </summary>
        /// <param name="ContractProjectCodeShort"></param>
        /// <param name="BillingClientCode"></param>
        /// <param name="BillingTargetRunningNo"></param>
        /// <returns></returns>
        public ActionResult BLS030_RetrieveData(string ContractProjectCodeShort, string BillingClientCode, string BillingTargetRunningNo)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            ICommonContractHandler commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            string lang = CommonUtil.GetCurrentLanguage();

            ValidatorUtil validator = new ValidatorUtil();

            try
            {

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                BLS030_ScreenParameter sParam = GetScreenObject<BLS030_ScreenParameter>();

                sParam.doBillingTypeList = new List<tbt_BillingTypeDetail>();

                if (String.IsNullOrEmpty(ContractProjectCodeShort))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "ContractCodeProjectCode",
                                         "lblContractProjectCode",
                                         "ContractCodeProjectCode");


                }

                if (String.IsNullOrEmpty(BillingClientCode))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "BillingTargetCode",
                                         "lblBillingTargetCode",
                                         "BillingTargetCode");
                }

                if (String.IsNullOrEmpty(BillingTargetRunningNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "BillingTargetRunningNo",
                                         "lblBillingTargetCode",
                                         "BillingTargetRunningNo");
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    return Json(res);
                }


                string strProjectCodeLong = comUtil.ConvertProjectCode(ContractProjectCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
                string strContractCodeLong = comUtil.ConvertContractCode(ContractProjectCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);

                List<doServiceProductTypeCode> lstContractServiceProductType = commonContractHandler.GetServiceProductTypeCode(strContractCodeLong);
                List<doServiceProductTypeCode> lstProjectServiceProductType = commonContractHandler.GetServiceProductTypeCode(strProjectCodeLong);


                bool isContractCode = false;
                bool isProjectCode = false;

                if (lstContractServiceProductType.Count > 0 && lstProjectServiceProductType.Count > 0)
                {
                    isContractCode = (string.IsNullOrEmpty(lstContractServiceProductType[0].ServiceTypeCode) == false);
                    if (isContractCode == false)
                    {
                        isProjectCode = (string.IsNullOrEmpty(lstProjectServiceProductType[0].ServiceTypeCode) == false);
                    }
                }

                // --- (1)
                if (isContractCode == false && isProjectCode == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS030",
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0011,
                                        new string[] { "lblContractProjectCode" },
                                        new string[] { "ContractCodeProjectCode" });

                    return Json(res);
                }
                else
                {
                    if (isContractCode)
                    {
                        sParam.ContractProjectCodeLong = strContractCodeLong;
                        sParam.ServiceTypeCode = lstContractServiceProductType[0].ServiceTypeCode;
                        sParam.ProductTypeCode = lstContractServiceProductType[0].ProductTypeCode;
                    }
                    else
                    {
                        sParam.ContractProjectCodeLong = strProjectCodeLong;
                        sParam.ServiceTypeCode = lstProjectServiceProductType[0].ServiceTypeCode;
                        sParam.ProductTypeCode = lstProjectServiceProductType[0].ProductTypeCode;
                    }
                }



                //--- (2)
                string strBillingClientCodeLong = comUtil.ConvertBillingClientCode(BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                var lst = billingHandler.GetTbt_BillingTargetForView(strBillingClientCodeLong + "-" + BillingTargetRunningNo, MiscType.C_CUST_TYPE);

                if (lst.Count == 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6040,
                                         null,
                                         new string[] { "BillingTargetCode", "BillingTargetRunningNo" });
                    return Json(res);
                }
                else
                {
                    var existsBillingOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == lst[0].BillingOfficeCode);
                    if (existsBillingOffice.Count() <= 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, 
                                            MessageUtil.MessageList.MSG0063, 
                                            null,
                                             new string[] { "BillingTargetCode", "BillingTargetRunningNo" });
                        return Json(res);
                    }
                    sParam.doBillingTarget = lst[0];
                    sParam.BillingClientCode = strBillingClientCodeLong;
                    sParam.BillingTargetRunningNo = BillingTargetRunningNo;
                }

                doTbt_BillingBasic doBillingBasic = new doTbt_BillingBasic();
                List<doTbt_BillingBasic> lstdoBillingBasic = billingHandler.GetBillingBasic(sParam.ContractProjectCodeLong,
                                                                                            null,
                                                                                            strBillingClientCodeLong + "-" + BillingTargetRunningNo,
                                                                                            null, null, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (lstdoBillingBasic.Count > 0)
                {
                    doBillingBasic = lstdoBillingBasic[0];
                }
                else
                {
                    doBillingBasic = null;
                }

                if (doBillingBasic != null)
                {
                    string tmpContractProjectCodeLong = ContractProjectCodeShort + "-" + doBillingBasic.BillingOCC;

                    // doBillingBasic.ContractCode + "-" + doBillingBasic.BillingOCC

                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6003,
                                         new string[] { tmpContractProjectCodeLong },
                                         new string[] { "ContractCodeProjectCode", "BillingTargetCode", "BillingTargetRunningNo" });
                    return Json(res);
                }

                res.ResultData = sParam;

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate input data before save to database (BLS030)
        /// </summary>
        /// <param name="validData"></param>
        /// <param name="ScreenData"></param>
        /// <param name="doTbt_AutoTransferBankAccount"></param>
        /// <param name="doTbt_CreditCard"></param>
        /// <returns></returns>
        public ActionResult BLS030_ValidateBeforeRegister(BLS030_ValidateData validData, BLS030_ScreenParameter ScreenData, tbt_AutoTransferBankAccount doTbt_AutoTransferBankAccount, tbt_CreditCard doTbt_CreditCard)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            CommonUtil comUtil = new CommonUtil();
            res.ResultData = true;

            try
            {

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.ResultData = false;
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGIST_BILL_BASIC, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    res.ResultData = false;
                    return Json(res);
                }

                BLS030_ScreenParameter sParam = GetScreenObject<BLS030_ScreenParameter>();

                sParam.AccountName = ScreenData.AccountName;
                sParam.CardName = ScreenData.CardName;
                sParam.PaymentMethod = ScreenData.PaymentMethod;
                sParam.SortingType = ScreenData.SortingType;


                //===================== Assign autotransfer data =======================
                sParam.doAutoTransferBankAccount = new BLS030_doAutoTransferBankAccount();
                sParam.doAutoTransferBankAccount.BankCode = doTbt_AutoTransferBankAccount.BankCode;
                sParam.doAutoTransferBankAccount.BankBranchCode = doTbt_AutoTransferBankAccount.BankBranchCode;
                sParam.doAutoTransferBankAccount.AccountNo = doTbt_AutoTransferBankAccount.AccountNo;
                sParam.doAutoTransferBankAccount.AccountName = doTbt_AutoTransferBankAccount.AccountName;
                sParam.doAutoTransferBankAccount.AccountType = doTbt_AutoTransferBankAccount.AccountType;
                sParam.doAutoTransferBankAccount.AutoTransferDate = doTbt_AutoTransferBankAccount.AutoTransferDate;
                //======================================================================
                //===================== Assign Credit Card data ========================
                sParam.doCreditCard = new BLS030_doCreditCard();
                sParam.doCreditCard.CreditCardCompanyCode = doTbt_CreditCard.CreditCardCompanyCode;
                sParam.doCreditCard.CreditCardType = doTbt_CreditCard.CreditCardType;
                sParam.doCreditCard.CreditCardNo = doTbt_CreditCard.CreditCardNo;
                sParam.doCreditCard.CardName = doTbt_CreditCard.CardName;
                sParam.doCreditCard.ExpMonth = doTbt_CreditCard.ExpMonth;
                sParam.doCreditCard.ExpYear = doTbt_CreditCard.ExpYear;
                //======================================================================


                ICommonContractHandler commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                    {
                        res.ResultData = false;
                        return Json(res);
                    }
                }

                //================ Validate Business ======================

                // Billing Type List
                if (sParam.doBillingTypeList == null || sParam.doBillingTypeList.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6005,
                                         null,
                                         null);
                    res.ResultData = false;
                }

                if (sParam.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER && CommonUtil.IsNullOrEmpty(sParam.doAutoTransferBankAccount.AccountNo))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6006,
                                         null,
                                         new string[] { "PaymentMethod" });
                    res.ResultData = false;
                }


                if (sParam.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER && CommonUtil.IsNullOrEmpty(sParam.doCreditCard.CreditCardNo))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6007,
                                         null,
                                         new string[] { "PaymentMethod" });
                    res.ResultData = false;
                }


                List<doServiceProductTypeCode> lstContractServiceProductType = commonContractHandler.GetServiceProductTypeCode(sParam.ContractProjectCodeLong);
                List<doServiceProductTypeCode> lstProjectServiceProductType = commonContractHandler.GetServiceProductTypeCode(sParam.ContractProjectCodeLong);


                bool isContractCode = false;
                bool isProjectCode = false;

                if (lstContractServiceProductType.Count > 0 && lstProjectServiceProductType.Count > 0)
                {
                    isContractCode = (string.IsNullOrEmpty(lstContractServiceProductType[0].ServiceTypeCode) == false);
                    if (isContractCode == false)
                    {
                        isProjectCode = (string.IsNullOrEmpty(lstProjectServiceProductType[0].ServiceTypeCode) == false);
                    }
                }


                // ----- (1)
                if (isContractCode == false && isProjectCode == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0011,
                                         new string[] { "lblContractProjectCode" },
                                         new string[] { "ContractCodeProjectCode" });
                    res.ResultData = false;
                }


                // ----- (2)
                var billingTargetForView = billingHandler.GetTbt_BillingTargetForView(sParam.BillingClientCode + "-" + sParam.BillingTargetRunningNo, MiscType.C_CUST_TYPE);

                if (billingTargetForView.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6040,
                                         null,
                                         new string[] { "BillingTargetCode", "BillingTargetRunningNo" });
                    res.ResultData = false;
                }


                // ----- (1) + (2)
                var lstdoBillingBasic = billingHandler.GetBillingBasic(sParam.ContractProjectCodeLong, null, sParam.BillingClientCode + "-" + sParam.BillingTargetRunningNo, null, null, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (lstdoBillingBasic.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6003,
                                         new string[] { comUtil.ConvertBillingCode(lstdoBillingBasic[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT) + "-" + lstdoBillingBasic[0].BillingOCC },
                                         new string[] { "ContractCodeProjectCode", "BillingTargetCode", "BillingTargetRunningNo" });
                    res.ResultData = false;
                }



            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.ResultData = false;
                res.AddErrorMessage(ex);
            }


            return Json(res);
        }

        /// <summary>
        /// Register input data to database (BLS030)
        /// </summary>
        /// <param name="validData"></param>
        /// <returns></returns>
        public ActionResult BLS030_RegisterData(BLS030_ValidateData validData)
        {

            ObjectResultData res = new ObjectResultData();
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ICommonContractHandler commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

            CommonUtil comUtil = new CommonUtil();
            try
            {
                if (handlerCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REGIST_BILL_BASIC, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }



                BLS030_ScreenParameter sParam = GetScreenObject<BLS030_ScreenParameter>();

                // === Business validate ===

                List<doServiceProductTypeCode> lstContractServiceProductType = commonContractHandler.GetServiceProductTypeCode(sParam.ContractProjectCodeLong);
                List<doServiceProductTypeCode> lstProjectServiceProductType = commonContractHandler.GetServiceProductTypeCode(sParam.ContractProjectCodeLong);


                bool isContractCode = false;
                bool isProjectCode = false;

                if (lstContractServiceProductType.Count > 0 && lstProjectServiceProductType.Count > 0)
                {
                    isContractCode = (string.IsNullOrEmpty(lstContractServiceProductType[0].ServiceTypeCode) == false);
                    if (isContractCode == false)
                    {
                        isProjectCode = (string.IsNullOrEmpty(lstProjectServiceProductType[0].ServiceTypeCode) == false);
                    }
                }

                //------ (1)
                if (isContractCode == false && isProjectCode == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0011,
                                         new string[] { "lblContractProjectCode" },
                                         new string[] { "ContractCodeProjectCode" });
                    return Json(res);

                }

                //------ (2)
                var lst = billingHandler.GetTbt_BillingTargetForView(sParam.BillingClientCode + "-" + sParam.BillingTargetRunningNo, MiscType.C_CUST_TYPE);

                if (lst.Count == 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6040,
                                         null,
                                         null);
                    return Json(res);
                }


                //------ (1) + (2)
                var lstdoBillingBasic = billingHandler.GetBillingBasic(sParam.ContractProjectCodeLong, null, sParam.BillingClientCode + "-" + sParam.BillingTargetRunningNo, null, null, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);

                if (lstdoBillingBasic.Count > 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6003,
                                         new string[] { comUtil.ConvertBillingCode(lstdoBillingBasic[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT) + "-" + lstdoBillingBasic[0].BillingOCC },
                                         null);
                    return Json(res);
                }


                using (TransactionScope scope = new TransactionScope())
                {

                //=== Billing Basic ===
                DataEntity.Billing.tbt_BillingBasic doBillingBasic2 = new DataEntity.Billing.tbt_BillingBasic()
                    {
                        ContractCode = sParam.ContractProjectCodeLong,
                        BillingTargetCode = sParam.BillingClientCode + "-" + sParam.BillingTargetRunningNo,
                        DebtTracingOfficeCode = sParam.doBillingTarget.BillingOfficeCode,
                        PaymentMethod = sParam.PaymentMethod,
                        SortingType = sParam.SortingType,
                        StopBillingFlag = true,
                        VATUnchargedFlag = false,
                        BillingCycle = BillingCycle.C_BILLING_CYCLE_DEFAULT_ONETIME,
                        CreditTerm = CreditTerm.C_CREDIT_TERM_DEFAULT,
                        CalDailyFeeStatus = CalculationDailyFeeType.C_CALC_DAILY_FEE_TYPE_CALENDAR,
                        ResultBasedMaintenanceFlag = false,

                        // Narupon
                        PreviousBillingTargetCode = sParam.BillingClientCode + "-" + sParam.BillingTargetRunningNo
                    };

                    // CREATE !!
                    string strBillingOCC = billingHandler.CreateBillingBasic(doBillingBasic2);


                    if (sParam.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                    {
                        //=== Auto Transfer ===

                        string strAccountNo = string.Empty;
                        strAccountNo = string.IsNullOrEmpty(sParam.doAutoTransferBankAccount.AccountNo) ? string.Empty : sParam.doAutoTransferBankAccount.AccountNo.Replace("-", "");

                        tbt_AutoTransferBankAccount doTbt_AutoTransferBankAccount = new tbt_AutoTransferBankAccount()
                        {
                            ContractCode = sParam.ContractProjectCodeLong,
                            BillingOCC = strBillingOCC,
                            BankCode = sParam.doAutoTransferBankAccount.BankCode,
                            BankBranchCode = sParam.doAutoTransferBankAccount.BankBranchCode,
                            AccountNo = strAccountNo,  // Edit Narupon W. 29/05/2012
                            AccountName = sParam.doAutoTransferBankAccount.AccountName,
                            AccountType = sParam.doAutoTransferBankAccount.AccountType,
                            AutoTransferDate = sParam.doAutoTransferBankAccount.AutoTransferDate
                        };

                        List<tbt_AutoTransferBankAccount> doTbt_AutoTransferBankAccountList = new List<tbt_AutoTransferBankAccount>();
                        doTbt_AutoTransferBankAccountList.Add(doTbt_AutoTransferBankAccount);
                        billingHandler.InsertTbt_AutoTransferBankAccountData(doTbt_AutoTransferBankAccountList);

                    }
                    else if (sParam.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                    {

                        //=== Credit Card ===

                        string strCreditCardNo = string.IsNullOrEmpty(sParam.doCreditCard.CreditCardNo) ? string.Empty : sParam.doCreditCard.CreditCardNo.Replace("-", "");

                        tbt_CreditCard doTbt_CreditCard = new tbt_CreditCard()
                        {
                            ContractCode = sParam.ContractProjectCodeLong,
                            BillingOCC = strBillingOCC,
                            CreditCardCompanyCode = sParam.doCreditCard.CreditCardCompanyCode,
                            CreditCardType = sParam.doCreditCard.CreditCardType,
                            CreditCardNo = strCreditCardNo, // Edit by Narupon W. 28/05/2012
                            CardName = sParam.doCreditCard.CardName,
                            ExpMonth = sParam.doCreditCard.ExpMonth,
                            ExpYear = sParam.doCreditCard.ExpYear
                        };

                        List<tbt_CreditCard> doTbt_CreditCardList = new List<tbt_CreditCard>();
                        doTbt_CreditCardList.Add(doTbt_CreditCard);
                        billingHandler.InsertTbt_CreditCard(doTbt_CreditCardList);

                    }

                    // === Billing Type Detail ===
                    if (sParam.doBillingTypeList != null)
                    {
                        foreach (tbt_BillingTypeDetail doBillingType in sParam.doBillingTypeList)
                        {
                            doBillingType.ContractCode = sParam.ContractProjectCodeLong;
                            doBillingType.BillingOCC = strBillingOCC;
                            doBillingType.IssueInvoiceFlag = true;
                            doBillingType.ProductCode = null;
                            doBillingType.ProductTypeCode = sParam.ProductTypeCode;

                            if (doBillingType.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE
                                || doBillingType.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE
                                || doBillingType.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN
                                || doBillingType.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL)
                            {
                                // tt
                                doBillingType.InvoiceDescriptionEN = doBillingType.InvoiceDescriptionEN;
                                doBillingType.InvoiceDescriptionLC = doBillingType.InvoiceDescriptionLC;
                            }
                            else
                            {
                                // tt
                                doBillingType.InvoiceDescriptionEN = null;
                                doBillingType.InvoiceDescriptionLC = null;
                            }

                            billingHandler.CreateBillingTypeDetail(doBillingType);
                        }
                    }
                scope.Complete();
                res.ResultData = true;
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
        /// Get billing type by product type code
        /// </summary>
        /// <param name="ProductTypeCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BLS030_GetBillingType(string ProductTypeCode)
        {

            ObjectResultData res = new ObjectResultData();
            try
            {
                BLS030_ScreenParameter sParam = GetScreenObject<BLS030_ScreenParameter>();
                string strBillingServiceTypeCode = "";
                if (sParam.ProductTypeCode == ProductType.C_PROD_TYPE_AL || sParam.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                {
                    strBillingServiceTypeCode = BillingServiceTypeCode.C_BILLING_SERVICE_TYPE_CODE_N;
                }
                else if (sParam.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                {
                    strBillingServiceTypeCode = BillingServiceTypeCode.C_BILLING_SERVICE_TYPE_CODE_MA;
                }
                else if (sParam.ProductTypeCode == ProductType.C_PROD_TYPE_BE || sParam.ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                {
                    strBillingServiceTypeCode = BillingServiceTypeCode.C_BILLING_SERVICE_TYPE_CODE_SG;
                }
                else if (sParam.ProductTypeCode == ProductType.C_PROD_TYPE_SALE)
                {
                    strBillingServiceTypeCode = BillingServiceTypeCode.C_BILLING_SERVICE_TYPE_CODE_Q;
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    strBillingServiceTypeCode = null;
                }

                sParam.strBillingServiceTypeCode = strBillingServiceTypeCode;
                IBillingMasterHandler billingMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                List<tbm_BillingType> BillingTypeList = billingMasterHandler.GetBillingTypeOneTimeListData(strBillingServiceTypeCode);
                CommonUtil.MappingObjectLanguage<tbm_BillingType>(BillingTypeList);
                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<tbm_BillingType>(BillingTypeList, "BillingTypeCodeName", "BillingTypeCode");


                return Json(cboModel);
            }
            catch (Exception ex)
            {

                res.AddErrorMessage(ex);
                return Json(res);
            }
            return Json(res);
        }

        /// <summary>
        /// Add billing type to selected list
        /// </summary>
        /// <param name="BillingTypeCode"></param>
        /// <returns></returns>
        public ActionResult BLS030_AddBillingType(string BillingTypeCode)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            if (string.IsNullOrEmpty(BillingTypeCode))
            {
                res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6090);
                return Json(res);
            }

            try
            {
                BLS030_ScreenParameter sParam = GetScreenObject<BLS030_ScreenParameter>();

                IBillingMasterHandler billingMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                List<tbm_BillingType> BillingTypeList = billingMasterHandler.GetBillingTypeOneTimeListData(sParam.strBillingServiceTypeCode);
                CommonUtil.MappingObjectLanguage<tbm_BillingType>(BillingTypeList);

                List<tbm_BillingType> BillingTypeList2 = (from t in BillingTypeList
                                                          where t.BillingTypeCode == BillingTypeCode
                                                          select t).ToList<tbm_BillingType>();

                if (sParam.doBillingTypeList == null)
                {
                    sParam.doBillingTypeList = new List<tbt_BillingTypeDetail>();
                }

                List<tbt_BillingTypeDetail> BillingTypeDetailExistList = (from t in sParam.doBillingTypeList
                                                                          where t.BillingTypeCode == BillingTypeCode
                                                                          select t).ToList<tbt_BillingTypeDetail>();

                if (BillingTypeDetailExistList.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS030",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6004,
                                         new string[] { BillingTypeList2[0].BillingTypeCodeName },
                                         null);
                    return Json(res);
                }

                tbt_BillingTypeDetail BillingTypeDetail = new tbt_BillingTypeDetail();

                ISaleContractHandler handlerSaleContract = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                var saleContractBasic_list = handlerSaleContract.GetSaleContractBasicForView(sParam.ContractProjectCodeLong);

                foreach (tbm_BillingType BillType in BillingTypeList2)
                {
                    BillingTypeDetail.BillingTypeCode = BillType.BillingTypeCode;

                    if (BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE
                    || BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE
                    || BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN
                    || BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL)
                    {
                        // ==
                        if (saleContractBasic_list.Count > 0)
                        {
                            if (BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE)
                            {
                                BillingTypeDetail.InvoiceDescriptionEN = BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE_PREFIX_EN + " " + saleContractBasic_list[0].ProductNameEN;
                                BillingTypeDetail.InvoiceDescriptionLC = BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE_PREFIX_LC + " " + saleContractBasic_list[0].ProductNameLC;

                                BillType.BillingTypeNameEN = BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE_PREFIX_EN + " " + saleContractBasic_list[0].ProductNameEN;
                                BillType.BillingTypeNameLC = BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE_PREFIX_LC + " " + saleContractBasic_list[0].ProductNameLC;

                            }

                            else if (BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE)
                            {
                                BillingTypeDetail.InvoiceDescriptionEN = BillingType.C_BILLING_TYPE_SALE_PRICE_PREFIX_EN + " " + saleContractBasic_list[0].ProductNameEN;
                                BillingTypeDetail.InvoiceDescriptionLC = BillingType.C_BILLING_TYPE_SALE_PRICE_PREFIX_LC + " " + saleContractBasic_list[0].ProductNameLC;

                                BillType.BillingTypeNameEN = BillingType.C_BILLING_TYPE_SALE_PRICE_PREFIX_EN + " " + saleContractBasic_list[0].ProductNameEN;
                                BillType.BillingTypeNameLC = BillingType.C_BILLING_TYPE_SALE_PRICE_PREFIX_LC + " " + saleContractBasic_list[0].ProductNameLC;
                            }
                            else if (BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN)
                            {
                                BillingTypeDetail.InvoiceDescriptionEN = BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN_SUBFIX_EN + " " + saleContractBasic_list[0].ProductNameEN;
                                BillingTypeDetail.InvoiceDescriptionLC = BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN_SUBFIX_LC + " " + saleContractBasic_list[0].ProductNameLC;

                                BillType.BillingTypeNameEN = BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN_SUBFIX_EN + " " + saleContractBasic_list[0].ProductNameEN;
                                BillType.BillingTypeNameLC = BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN_SUBFIX_LC + " " + saleContractBasic_list[0].ProductNameLC;
                            }

                            else if (BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL)
                            {
                                BillingTypeDetail.InvoiceDescriptionEN = BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL_SUBFIX_EN + " " + saleContractBasic_list[0].ProductNameEN;
                                BillingTypeDetail.InvoiceDescriptionLC = BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL_SUBFIX_LC + " " + saleContractBasic_list[0].ProductNameLC;

                                BillType.BillingTypeNameEN = BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL_SUBFIX_EN + " " + saleContractBasic_list[0].ProductNameEN;
                                BillType.BillingTypeNameLC = BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL_SUBFIX_LC + " " + saleContractBasic_list[0].ProductNameLC;
                            }
                        }



                    }
                    else
                    {
                        BillingTypeDetail.InvoiceDescriptionEN = BillType.BillingTypeNameEN;
                        BillingTypeDetail.InvoiceDescriptionLC = BillType.BillingTypeNameLC;
                    }


                    BillingTypeDetail.BillingTypeGroup = BillType.BillingTypeCode;
                    sParam.doBillingTypeList.Add(BillingTypeDetail);
                }


                res.ResultData = BillingTypeList2;

                return Json(res);
            }
            catch (Exception ex)
            {

                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Remove billing type from selected list
        /// </summary>
        /// <param name="BillingTypeCode"></param>
        /// <returns></returns>
        public ActionResult BLS030_RemoveBillingTypeClick(string BillingTypeCode)
        {

            ObjectResultData res = new ObjectResultData();
            try
            {
                BLS030_ScreenParameter sParam = GetScreenObject<BLS030_ScreenParameter>();

                List<tbt_BillingTypeDetail> BillingTypeDetailList = (from t in sParam.doBillingTypeList
                                                                     where t.BillingTypeCode == BillingTypeCode
                                                                     select t).ToList<tbt_BillingTypeDetail>();


                foreach (tbt_BillingTypeDetail BillType in BillingTypeDetailList)
                {
                    sParam.doBillingTypeList.Remove(BillType);
                }


                res.ResultData = true;

                return Json(res);
            }
            catch (Exception ex)
            {

                res.AddErrorMessage(ex);
                return Json(res);
            }

        }


    }
}
