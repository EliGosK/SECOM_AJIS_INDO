
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
        /// Check permission for access screen BLS040
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS040_Authority(BLS040_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            if (chandler.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_BASIC, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            if (param.CallerScreenID != ScreenID.C_SCREEN_ID_VIEW_BILLING_BASIC_INFORMATION)
            {
                param.ContractProjectCodeShort = "";
                param.BillingOCC = "";
            }
            else
            {
                param.ContractProjectCodeShort = param.ContractCode;
            }

            return InitialScreenEnvironment<BLS040_ScreenParameter>("BLS040", param, res);

        }

        /// <summary>
        /// Method for return view of screen BLS040
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS040")]
        public ActionResult BLS040()
        {

            BLS040_ScreenParameter param = GetScreenObject<BLS040_ScreenParameter>();

            // Narupon W.

            if (param != null)
            {
                ViewBag.ContractProjectCode = param.ContractProjectCodeShort;
                ViewBag.BillingOCC = param.BillingOCC;
            }

            ViewBag.HasRegisterCreditCardPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_BASIC, FunctionID.C_FUNC_ID_REGISTER_CREDIT_CARD) ? "1" : "0";
            ViewBag.HasRegisterAutoTransferPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_BASIC, FunctionID.C_FUNC_ID_REGISTER_AUTO_TRANSFER) ? "1" : "0";



            return View();
        }


        /// <summary>
        /// Initial grid of screen BLS040 (billing type grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS040_InitialGridBillingType()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS040_BillingType", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Retrieve billing basic data
        /// </summary>
        /// <param name="ContractProjectCodeShort"></param>
        /// <param name="BillingOCC"></param>
        /// <returns></returns>
        public ActionResult BLS040_RetrieveData(string ContractProjectCodeShort, string BillingOCC)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil commUtil = new CommonUtil();
            ICommonContractHandler commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            string lang = CommonUtil.GetCurrentLanguage();
            ValidatorUtil validator = new ValidatorUtil();
            try
            {

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                BLS040_ScreenParameter sParam = GetScreenObject<BLS040_ScreenParameter>();

                sParam.doBillingTypeDetailList = new List<doBillingTypeDetailList>();

                if (String.IsNullOrEmpty(ContractProjectCodeShort))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "ContractCodeProjectCode",
                                         "lblBillingCode",
                                         "ContractCodeProjectCode");
                }

                if (String.IsNullOrEmpty(BillingOCC))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "BillingOCC",
                                         "lblBillingCode",
                                         "BillingOCC");
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    return Json(res);
                }

                sParam.ContractProjectCodeShort = ContractProjectCodeShort;
                sParam.BillingOCC = BillingOCC;

                // === Contract code & Project code ===
                string strProjectCodeLong = commUtil.ConvertProjectCode(ContractProjectCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
                string strContractCodeLong = commUtil.ConvertContractCode(ContractProjectCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);

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


                if (isContractCode == false && isProjectCode == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
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



                // === Billing Basic ===
                sParam.doBillingBasic = new doTbt_BillingBasic();
                List<doTbt_BillingBasic> lstdoBillingBasic = billingHandler.GetBillingBasic(sParam.ContractProjectCodeLong, BillingOCC, null, null, null, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                CommonUtil.MappingObjectLanguage<doTbt_BillingBasic>(lstdoBillingBasic);

                if (lstdoBillingBasic.Count > 0)
                {
                    sParam.doBillingBasic = new doTbt_BillingBasic();
                    sParam.doBillingBasic = lstdoBillingBasic[0];

                    sParam.BillingClientCode = lstdoBillingBasic[0].BillingClientCode;
                    sParam.BillingTargetRunningNo = lstdoBillingBasic[0].BillingTargetNo;
                }
                else
                {
                    sParam.doBillingBasic = null;

                    sParam.BillingClientCode = string.Empty;
                    sParam.BillingTargetRunningNo = string.Empty;
                }


                if (sParam.doBillingBasic == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6041,
                                         null,
                                         new string[] { "ContractCodeProjectCode", "BillingOCC" });
                    return Json(res);
                }


                // === Billing Target ===

                sParam.doBillingTarget = null;
                List<dtTbt_BillingTargetForView> lstBilling = billingHandler.GetTbt_BillingTargetForView(sParam.doBillingBasic.BillingTargetCode, MiscType.C_CUST_TYPE);

                if (lstBilling.Count > 0)
                {
                    var existsBillingOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == lstBilling[0].BillingOfficeCode);
                    if (existsBillingOffice.Count() <= 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0063,
                                            null,
                                             new string[] { "ContractCodeProjectCode", "BillingOCC" });
                        return Json(res);
                    }

                    sParam.doBillingTarget = lstBilling[0];
                    sParam.BillingClientCodeShort = commUtil.ConvertBillingClientCode(sParam.doBillingTarget.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }


                // === Billng Type Detail === 

                sParam.doBillingTypeDetailList = billingHandler.GetBillingTypeDetailList(sParam.ContractProjectCodeLong, sParam.BillingOCC);
                CommonUtil.MappingObjectLanguage<doBillingTypeDetailList>(sParam.doBillingTypeDetailList);

                sParam.doBillingTypeDetailListPrevious = new List<doBillingTypeDetailList>();

                if (sParam.doBillingTypeDetailList != null)
                {
                    foreach (doBillingTypeDetailList billingType in sParam.doBillingTypeDetailList)
                    {
                        sParam.doBillingTypeDetailListPrevious.Add(billingType);
                    }
                }


                // === Monthly Billing History ===

                sParam.doTbt_MonthlyBillingHistoryList = new List<doTbt_MonthlyBillingHistoryList>();
                sParam.doTbt_MonthlyBillingHistoryList = billingHandler.GetBillingHistoryList(sParam.ContractProjectCodeLong, sParam.BillingOCC, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);

                // === Auto Transfer ===
                sParam.doTbt_AutoTransferBankAccount = new tbt_AutoTransferBankAccount();
                List<tbt_AutoTransferBankAccount> lst = new List<tbt_AutoTransferBankAccount>();
                lst = billingHandler.GetTbt_AutoTransferBankAccount(sParam.ContractProjectCodeLong, sParam.BillingOCC);

                var autoTransferForview = billingHandler.GetAutoTransferBankAccountForView(sParam.ContractProjectCodeLong, sParam.BillingOCC);

                if (lst.Count > 0 && autoTransferForview.Count > 0)
                {
                    // Edit by Narupon W 28/05/2012
                    string strAccount = string.Empty;
                    if (string.IsNullOrEmpty(lst[0].AccountNo) == false)
                    {
                        lst[0].AccountNo = lst[0].AccountNo.Replace("-", "");

                        // XXX-X-XXXXX-X  => (10 digits)
                        if (lst[0].AccountNo.Length == 10)
                        {
                            strAccount = string.Format("{0}-{1}-{2}-{3}", lst[0].AccountNo.Substring(0, 3), lst[0].AccountNo.Substring(3, 1), lst[0].AccountNo.Substring(4, 5), lst[0].AccountNo.Substring(9, 1));
                        }
                        else
                        {
                            strAccount = lst[0].AccountNo;
                        }
                    }

                    // **
                    lst[0].AccountNo = strAccount;
                    autoTransferForview[0].AccountNo = strAccount;

                    sParam.doTbt_AutoTransferBankAccount = lst[0];
                    sParam.dtAutoTransferForView = autoTransferForview[0];
                }
                else
                {
                    sParam.doTbt_AutoTransferBankAccount = null;
                    sParam.dtAutoTransferForView = null;
                }


                // === Credit card ===

                sParam.doTbt_CreditCard = new tbt_CreditCard();
                List<tbt_CreditCard> lstcredit = new List<tbt_CreditCard>();
                lstcredit = billingHandler.GetTbt_CreditCard(sParam.ContractProjectCodeLong, sParam.BillingOCC);
                var creditCard_Forview = billingHandler.GetCreditCardForView(sParam.ContractProjectCodeLong, sParam.BillingOCC);

                if (lstcredit.Count > 0 && creditCard_Forview.Count > 0)
                {
                    // lstcredit[0]

                    // Edit by Narupon W 28/05/2012
                    string strCreditCardNo = string.Empty;
                    if (string.IsNullOrEmpty(lstcredit[0].CreditCardNo) == false)
                    {
                        lstcredit[0].CreditCardNo = lstcredit[0].CreditCardNo.Replace("-", "");

                        if (CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_BASIC, FunctionID.C_FUNC_ID_REGISTER_CREDIT_CARD))
                        {
                            // XXXX-XXXX-XXXX-XXXX  => (16 digits)
                            if (lstcredit[0].CreditCardNo.Length == 16)
                            {
                                strCreditCardNo = string.Format("{0}-{1}-{2}-{3}", lstcredit[0].CreditCardNo.Substring(0, 4), lstcredit[0].CreditCardNo.Substring(4, 4), lstcredit[0].CreditCardNo.Substring(8, 4), lstcredit[0].CreditCardNo.Substring(12, 4));
                            }
                            else
                            {
                                strCreditCardNo = lstcredit[0].CreditCardNo;
                            }
                        }
                        else
                        {
                            // XXXX-XXXX-XXXX-XXXX  => (16 digits)
                            if (lstcredit[0].CreditCardNo.Length == 16)
                            {
                                strCreditCardNo = string.Format("****-****-****-{0}", lstcredit[0].CreditCardNo.Substring(12, 4));
                            }
                            else
                            {
                                strCreditCardNo = "****-****-****-1234";
                            }
                        }


                    }

                    // **
                    lstcredit[0].CreditCardNo = strCreditCardNo;
                    creditCard_Forview[0].CreditCardNo = strCreditCardNo;

                    sParam.doTbt_CreditCard = lstcredit[0];
                    sParam.dtCreditCardForView = creditCard_Forview[0];
                }
                else
                {
                    sParam.doTbt_CreditCard = null;
                    sParam.dtCreditCardForView = null;
                }


                if (CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_BASIC, FunctionID.C_FUNC_ID_CAREFUL_SPECIAL))
                {
                    sParam.EnableCarefulSpecial = true;
                }

                if (CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_BASIC, FunctionID.C_FUNC_ID_REGISTER_AUTO_TRANSFER))
                {
                    sParam.EnableAutoTransfer = true;
                }

                if (CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_BASIC, FunctionID.C_FUNC_ID_REGISTER_CREDIT_CARD))
                {
                    sParam.EnableCreditCard = true;
                }

                if (sParam.ContractProjectCodeLong != null && sParam.ContractProjectCodeLong.Length > 0)
                {
                    string Q_Code = sParam.ContractProjectCodeLong.Substring(0, 1).ToUpper();
                    if (Q_Code == BillingServiceTypeCode.C_BILLING_SERVICE_TYPE_CODE_Q.ToUpper())
                    {
                        sParam.IsQCodeContract = true;
                    }
                    else
                    {
                        sParam.IsQCodeContract = false;
                    }
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
        /// Get data to BillingDetail grid
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult BLS040_GetBillingDetailListData(List<doBillingTypeDetailList> doBillingDetail)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res.ResultData = CommonUtil.ConvertToXml<doBillingTypeDetailList>(doBillingDetail, "Billing\\BLS040_BillingType", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }


        /// <summary>
        /// Validate input data before save to database (BLS040)
        /// </summary>
        /// <param name="validData"></param>
        /// <param name="screenBillingBasic"></param>
        /// <param name="changingDateList"></param>
        /// <param name="screenAutoTransferBankAccount"></param>
        /// <param name="screenCreditCard"></param>
        /// <returns></returns>
        public ActionResult BLS040_ValidateBeforeRegister(BLS040_ValidateData validData, DataEntity.Billing.tbt_BillingBasic screenBillingBasic, BLS040_ChangingDateList changingDateList, tbt_AutoTransferBankAccount screenAutoTransferBankAccount, tbt_CreditCard screenCreditCard)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            CommonUtil comUtil = new CommonUtil();
            res.ResultData = true;

            try
            {
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);

                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_BASIC, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);

                    return Json(res);
                }

                BLS040_ScreenParameter sParam = GetScreenObject<BLS040_ScreenParameter>();

                ICommonContractHandler handlerCommonContract = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;


                if (String.IsNullOrEmpty(validData.ContractProjectCodeShort))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "ContractCodeProjectCode",
                                         "lblBillingCode",
                                         "ContractCodeProjectCode");
                }

                if (String.IsNullOrEmpty(validData.BillingOCC))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                          "BillingOCC",
                                         "lblBillingOCC",
                                         "BillingOCC");
                }

                if (String.IsNullOrEmpty(validData.BillingClientCode))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "BillingClientCode",
                                         "lblBillingTargetCode",
                                         "BillingClientCode");
                }

                if (String.IsNullOrEmpty(validData.BillingTargetNo) || sParam.doBillingTarget == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "BillingTargetNo",
                                         "lblBillingTargetCode",
                                         "BillingTargetNo");
                }


                // **** Check required filed monthly billing history date ***
                if (sParam.doTbt_MonthlyBillingHistoryList == null)
                {
                    sParam.doTbt_MonthlyBillingHistoryList = new List<doTbt_MonthlyBillingHistoryList>();
                }


                // Data from screen
                var screenBillingHistoryDate = new List<DateTime?>();
                screenBillingHistoryDate.Add(changingDateList.BillingStartDate0);
                screenBillingHistoryDate.Add(changingDateList.BillingStartDate1);
                screenBillingHistoryDate.Add(changingDateList.BillingStartDate2);
                screenBillingHistoryDate.Add(changingDateList.BillingStartDate3);
                screenBillingHistoryDate.Add(changingDateList.BillingStartDate4);
                screenBillingHistoryDate.Add(changingDateList.BillingStartDate5);

                for (int i = 0; i < sParam.doTbt_MonthlyBillingHistoryList.Count; i++)
                {
                    if (i < 6)
                    {
                        if (sParam.doTbt_MonthlyBillingHistoryList[i].BillingStartDate.HasValue && (screenBillingHistoryDate[i].HasValue == false))
                        {
                            // zz
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                          "BillingStartDate" + i.ToString(),
                                         "lblDatebeforechanging" + i.ToString(),
                                          "BillingStartDate" + i.ToString());

                        }
                    }
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    res.ResultData = false;
                    return Json(res);
                }


                // === Billing Basic validate ===

                List<doTbt_BillingBasic> billingBasicList = billingHandler.GetBillingBasic(sParam.ContractProjectCodeLong, null, sParam.BillingClientCode + "-" + sParam.BillingTargetRunningNo, null, null, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);

                if (billingBasicList.Count > 0)
                {
                    if (!(billingBasicList[0].ContractCode == sParam.ContractProjectCodeLong && billingBasicList[0].BillingOCC == sParam.BillingOCC))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS040",
                                             MessageUtil.MODULE_BILLING,
                                             MessageUtil.MessageList.MSG6003,
                                             new string[] { comUtil.ConvertBillingCode(billingBasicList[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT) + "-" + billingBasicList[0].BillingOCC },
                                             new string[] { "BillingClientCode", "BillingTargetNo" });

                        res.ResultData = false;
                        return Json(res);
                    }
                }


                // === Billing Type Detail ===
                if (sParam.doBillingTypeDetailList == null || sParam.doBillingTypeDetailList.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6005,
                                         null,
                                         null);
                }

                // === Auto Transfer ===
                if (screenBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER && CommonUtil.IsNullOrEmpty(screenAutoTransferBankAccount.AccountNo))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6006,
                                         null,
                                         new string[] { "PaymentMethod" });
                }


                // === Credit Card ===
                if (screenBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER && CommonUtil.IsNullOrEmpty(screenCreditCard.CreditCardNo))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6007,
                                         null,
                                         new string[] { "PaymentMethod" });
                }


                // == Check Billing Detail if Payment method = Auto Tranfer or Credit Card
                var billngDetail_AutoTransfer_CreditCard = handlerBilling.GetBillingDetailAutoTransferList(sParam.ContractProjectCodeLong, sParam.BillingOCC);

                if (billngDetail_AutoTransfer_CreditCard.Count > 0)
                {
                    if (sParam.doBillingBasic.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER && screenBillingBasic.PaymentMethod != PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER)
                    {
                        // == change to other PaymentMethod
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS040",
                                        MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6075,
                                        null,
                                        new string[] { "PaymentMethod" });
                    }
                    else if (sParam.doBillingBasic.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER && screenBillingBasic.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER)
                    {
                        // == Not change PaymentMethod but detail is change!
                        if (sParam.doTbt_AutoTransferBankAccount.AccountName != screenAutoTransferBankAccount.AccountName ||
                            sParam.doTbt_AutoTransferBankAccount.AccountNo != screenAutoTransferBankAccount.AccountNo ||
                            sParam.doTbt_AutoTransferBankAccount.AccountType != screenAutoTransferBankAccount.AccountType ||
                            sParam.doTbt_AutoTransferBankAccount.AutoTransferDate != screenAutoTransferBankAccount.AutoTransferDate ||
                            sParam.doTbt_AutoTransferBankAccount.BankBranchCode != screenAutoTransferBankAccount.BankBranchCode ||
                            sParam.doTbt_AutoTransferBankAccount.BankCode != screenAutoTransferBankAccount.BankCode
                            )
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS040",
                                        MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6076,
                                        null,
                                        new string[] { "PaymentMethod" });
                        }
                    }
                    else if (sParam.doBillingBasic.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD && screenBillingBasic.PaymentMethod != PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD)
                    {
                        // == change to other PaymentMethod
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS040",
                                        MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6077,
                                        null,
                                        new string[] { "PaymentMethod" });

                    }
                    else if (sParam.doBillingBasic.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD && screenBillingBasic.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD)
                    {
                        // == Not change PaymentMethod but detail is change!
                        if (
                            sParam.doTbt_CreditCard.CardName != screenCreditCard.CardName ||
                            sParam.doTbt_CreditCard.CreditCardCompanyCode != screenCreditCard.CreditCardCompanyCode ||
                            sParam.doTbt_CreditCard.CreditCardNo != screenCreditCard.CreditCardNo ||
                            sParam.doTbt_CreditCard.CreditCardType != screenCreditCard.CreditCardType ||
                            sParam.doTbt_CreditCard.ExpMonth != screenCreditCard.ExpMonth ||
                            sParam.doTbt_CreditCard.ExpYear != screenCreditCard.ExpYear
                            )
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS040",
                                        MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6078,
                                        null,
                                        new string[] { "PaymentMethod" });
                        }
                    }
                }


                // Need Approve No.
                if (sParam.doBillingBasic.BillingCycle != screenBillingBasic.BillingCycle)
                {
                    if (screenBillingBasic.BillingCycle <= 2 && CommonUtil.IsNullOrEmpty(screenBillingBasic.ApproveNo) && (sParam.IsQCodeContract == false))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                            "BLS040",
                                            MessageUtil.MODULE_BILLING,
                                            MessageUtil.MessageList.MSG6009,
                                            null,
                                            new string[] { "ApproveNo" }); //new string[] { "BillingCycle" }); //Modify by Jutarat A. on 14062013
                    }

                }

                // input.AdjustEndDate must later than current.LastBillingDate
                if (screenBillingBasic.AdjustEndDate <= sParam.doBillingBasic.LastBillingDate)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS040",
                                        MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6010,
                                        null,
                                        new string[] { "AdjustEndDate" });
                }

                if (screenBillingBasic.ChangeDate > DateTime.Now.Date)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS040",
                                        MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6011,
                                        null,
                                        new string[] { "ChangeDate" });
                }


                // Billing History : Check Billing start date 
                if (changingDateList.BillingStartDate0 < changingDateList.BillingStartDate1
                    || changingDateList.BillingStartDate1 < changingDateList.BillingStartDate2
                    || changingDateList.BillingStartDate2 < changingDateList.BillingStartDate3
                    || changingDateList.BillingStartDate3 < changingDateList.BillingStartDate4
                    || changingDateList.BillingStartDate4 < changingDateList.BillingStartDate5
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS040",
                                        MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6012,
                                        null,
                                        null);
                }
                if (sParam.doTbt_MonthlyBillingHistoryList.Count > 6)
                {
                    if (changingDateList.BillingStartDate5 < sParam.doTbt_MonthlyBillingHistoryList[6].BillingStartDate.Value)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                            "BLS040",
                                            MessageUtil.MODULE_BILLING,
                                            MessageUtil.MessageList.MSG6012,
                                            null,
                                            null);
                    }
                }

                // Billing History : Check first of BillingStartDate(s) with StartOperationDate
                // Edit by Patchree T. 25/07/2012
                //if (changingDateList.BillingStartDate0 < sParam.doBillingBasic.StartOperationDate)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                //                        "BLS040",
                //                        MessageUtil.MODULE_BILLING,
                //                        MessageUtil.MessageList.MSG6013,
                //                        null,
                //                        null);
                //}
                for (int i = 0; i < sParam.doTbt_MonthlyBillingHistoryList.Count; i++)
                {
                    if (i < 6)
                    {
                        if (i == sParam.doTbt_MonthlyBillingHistoryList.Count - 1) // First Monthly Billing History because list of Monthly Billing History order by Max --> Min
                        {
                            // Billing History : Check first of BillingStartDate(s) with StartOperationDate
                            if (screenBillingHistoryDate[i].Value < sParam.doBillingBasic.StartOperationDate.Value)
                            {
                                // zz
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                    "BLS040",
                                                    MessageUtil.MODULE_BILLING,
                                                    MessageUtil.MessageList.MSG6013,
                                                    null,
                                                    null);
                            }
                            //There is at least one billing OCC which 'Start date of billing period' of the first billing history is the same date as 
                            //New start service date in Contract module, in billing basic of all contract fee billing (including billing OCC 
                            //which billed in the past and now no longer bill)
                            if (screenBillingHistoryDate[i].Value.Date != sParam.doBillingBasic.StartOperationDate.Value)
                            {
                                // === Get All Monthly Billing History ===
                                List<doTbt_MonthlyBillingHistoryList> doTbt_MonthlyBillingHistoryList = new List<doTbt_MonthlyBillingHistoryList>();
                                doTbt_MonthlyBillingHistoryList = billingHandler.GetBillingHistoryList(sParam.ContractProjectCodeLong, null, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                                doTbt_MonthlyBillingHistoryList = (from t in doTbt_MonthlyBillingHistoryList
                                                                   where t.HistoryNo == 1 && t.BillingOCC != sParam.BillingOCC && t.BillingStartDate == sParam.doBillingBasic.StartOperationDate.Value
                                                                   select t).ToList<doTbt_MonthlyBillingHistoryList>();
                                if (doTbt_MonthlyBillingHistoryList.Count == 0)
                                {
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                        "BLS040",
                                                        MessageUtil.MODULE_BILLING,
                                                        MessageUtil.MessageList.MSG6089,
                                                        null,
                                                        null);
                                }

                            }
                        }
                    }

                }

                if (res.IsError)
                {
                    res.ResultData = false;
                    return Json(res);
                }


            }
            catch (Exception ex)
            {
                res.ResultData = false;
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }


            return Json(res);
        }

        /// <summary>
        ///  Register input data to database (BLS040)
        /// </summary>
        /// <param name="validData"></param>
        /// <param name="screenBillingBasic"></param>
        /// <param name="changingDateList"></param>
        /// <param name="screenAutoTransferBankAccount"></param>
        /// <param name="screenCreditCard"></param>
        /// <returns></returns>
        public ActionResult BLS040_RegisterData(BLS040_ValidateData validData, DataEntity.Billing.tbt_BillingBasic screenBillingBasic
            , BLS040_ChangingDateList changingDateList, tbt_AutoTransferBankAccount screenAutoTransferBankAccount
            , tbt_CreditCard screenCreditCard, BLS040_MonthlyBillingAmountList monthlyBillingAmountList) //Modify by Jutarat A. on 25042013 (Add monthlyBillingAmountList)
        {

            ObjectResultData res = new ObjectResultData();
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ICommonContractHandler commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            IBillingTempHandler billingTempHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

            CommonUtil comUtil = new CommonUtil();
            try
            {
                if (handlerCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_BASIC, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                // === Billing Basic validate === 

                BLS040_ScreenParameter sParam = GetScreenObject<BLS040_ScreenParameter>();

                



                List<doTbt_BillingBasic> billingBasicList = billingHandler.GetBillingBasic(sParam.ContractProjectCodeLong, null, sParam.BillingClientCode + "-" + sParam.BillingTargetRunningNo, null, null, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);

                if (billingBasicList.Count > 0)
                {
                    if (!(billingBasicList[0].ContractCode == sParam.ContractProjectCodeLong && billingBasicList[0].BillingOCC == sParam.BillingOCC))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS040",
                                             MessageUtil.MODULE_BILLING,
                                             MessageUtil.MessageList.MSG6003,
                                             new string[] { comUtil.ConvertBillingCode(billingBasicList[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT) + "-" + billingBasicList[0].BillingOCC },
                                             null);

                        return Json(res);
                    }

                }

                // ***********************************************
                using (TransactionScope scope = new TransactionScope())
                {

                //=== Billing Basic ===
                screenBillingBasic.ContractCode = sParam.ContractProjectCodeLong;
                    screenBillingBasic.BillingTargetCode = sParam.BillingClientCode + "-" + sParam.BillingTargetRunningNo; // sParam.doBillingBasic.BillingClientCode + "-" + sParam.doBillingBasic.BillingTargetNo;
                    screenBillingBasic.PreviousBillingTargetCode = sParam.doBillingBasic.BillingTargetCode;
                    screenBillingBasic.StartOperationDate = sParam.doBillingBasic.StartOperationDate;

                    screenBillingBasic.MonthlyBillingAmount = sParam.doBillingBasic.MonthlyBillingAmount;
                screenBillingBasic.MonthlyBillingAmountUsd = sParam.doBillingBasic.MonthlyBillingAmountUsd;
                screenBillingBasic.MonthlyBillingAmountCurrencyType = sParam.doBillingBasic.MonthlyBillingAmountCurrencyType;

                screenBillingBasic.MonthlyFeeBeforeStop = sParam.doBillingBasic.MonthlyFeeBeforeStop;
                    screenBillingBasic.DocReceiving = sParam.doBillingBasic.DocReceiving;
                    screenBillingBasic.AdjustType = sParam.doBillingBasic.AdjustType;
                    screenBillingBasic.AdjustBillingPeriodAmount = sParam.doBillingBasic.AdjustBillingPeriodAmount;
                    screenBillingBasic.AdjustBillingPeriodStartDate = sParam.doBillingBasic.AdjustBillingPeriodStartDate;
                    screenBillingBasic.AdjustBillingPeriodEndDate = sParam.doBillingBasic.AdjustBillingPeriodEndDate;
                    screenBillingBasic.CreateBy = sParam.doBillingBasic.CreateBy;
                    screenBillingBasic.CreateDate = sParam.doBillingBasic.CreateDate;
                    screenBillingBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    screenBillingBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;


                    // Narupon
                    if (sParam.doBillingBasic.DebtTracingOfficeCode != null && screenBillingBasic.DebtTracingOfficeCode == null)
                    {
                        screenBillingBasic.DebtTracingOfficeCode = sParam.doBillingBasic.DebtTracingOfficeCode;
                    }

                    // UPDATE !!
                    billingHandler.UpdateTbt_BillingBasic(screenBillingBasic);

                    var billingTempList = billingTempHandler.GetTbt_BillingTemp(sParam.ContractProjectCodeLong, null);
                    if (billingTempList != null && billingTempList.Count > 0)
                    {
                        foreach (var billingTemp in billingTempList)
                        {
                            if (billingTemp.BillingOCC == sParam.BillingOCC)
                            {
                                billingTemp.BillingClientCode = sParam.BillingClientCode;
                                billingTemp.BillingTargetCode = screenBillingBasic.BillingTargetCode;
                                billingTemp.BillingTargetRunningNo = sParam.BillingTargetRunningNo;
                                billingTemp.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                billingTemp.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                billingTempHandler.UpdateBillingTempByKey(billingTemp);
                            }
                        }
                    }

                    //=== Auto Transfer ===
                    screenAutoTransferBankAccount.ContractCode = sParam.ContractProjectCodeLong;
                    screenAutoTransferBankAccount.BillingOCC = sParam.doBillingBasic.BillingOCC;
                    screenCreditCard.ContractCode = sParam.ContractProjectCodeLong;
                    screenCreditCard.BillingOCC = sParam.doBillingBasic.BillingOCC;

                    if (CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_BASIC, FunctionID.C_FUNC_ID_REGISTER_AUTO_TRANSFER))
                    {
                        if (screenBillingBasic.PaymentMethod != sParam.doBillingBasic.PaymentMethod && sParam.doBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                        {
                            billingHandler.DeleteTbt_AutoTransferBankAccount(sParam.ContractProjectCodeLong, sParam.BillingOCC);

                        }
                        else if (screenBillingBasic.PaymentMethod != sParam.doBillingBasic.PaymentMethod && screenBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                        {
                            // Edit by Narupon W. 28/05/2012
                            string accountNo = string.IsNullOrEmpty(screenAutoTransferBankAccount.AccountNo) ? string.Empty : screenAutoTransferBankAccount.AccountNo.Replace("-", "");
                            screenAutoTransferBankAccount.AccountNo = accountNo;
                            screenAutoTransferBankAccount.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            screenAutoTransferBankAccount.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            screenAutoTransferBankAccount.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            screenAutoTransferBankAccount.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                            List<tbt_AutoTransferBankAccount> doTbt_AutoTransferBankAccountDataList = new List<tbt_AutoTransferBankAccount>();
                            doTbt_AutoTransferBankAccountDataList.Add(screenAutoTransferBankAccount);

                            billingHandler.InsertTbt_AutoTransferBankAccountData(doTbt_AutoTransferBankAccountDataList);
                        }
                        else if (screenBillingBasic.PaymentMethod == sParam.doBillingBasic.PaymentMethod && screenBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                        {
                            // Edit by Narupon W. 28/05/2012
                            string accountNo = string.IsNullOrEmpty(screenAutoTransferBankAccount.AccountNo) ? string.Empty : screenAutoTransferBankAccount.AccountNo.Replace("-", "");
                            screenAutoTransferBankAccount.AccountNo = accountNo;

                            if (sParam.doTbt_AutoTransferBankAccount != null) //Add by Jutarat A. on 21032013
                            {
                                screenAutoTransferBankAccount.CreateBy = sParam.doTbt_AutoTransferBankAccount.CreateBy;
                                screenAutoTransferBankAccount.CreateDate = sParam.doTbt_AutoTransferBankAccount.CreateDate;
                            }

                            screenAutoTransferBankAccount.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            screenAutoTransferBankAccount.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                            List<tbt_AutoTransferBankAccount> doTbt_AutoTransferBankAccountDataList = new List<tbt_AutoTransferBankAccount>();
                            doTbt_AutoTransferBankAccountDataList.Add(screenAutoTransferBankAccount);
                            billingHandler.UpdateTbt_AutoTransferBankAccount(doTbt_AutoTransferBankAccountDataList);
                        }
                    }




                    //=== Credit card ===
                    if (CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_BILL_BASIC, FunctionID.C_FUNC_ID_REGISTER_CREDIT_CARD))
                    {
                        if (screenBillingBasic.PaymentMethod != sParam.doBillingBasic.PaymentMethod && sParam.doBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                        {
                            billingHandler.DeleteTbt_CreditCard(sParam.ContractProjectCodeLong, sParam.BillingOCC);

                        }
                        else if (screenBillingBasic.PaymentMethod != sParam.doBillingBasic.PaymentMethod && screenBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                        {

                            // Edit by Narupon W. 28/05/2012
                            string strCreditCardNo = string.IsNullOrEmpty(screenCreditCard.CreditCardNo) ? string.Empty : screenCreditCard.CreditCardNo.Replace("-", "");
                            screenCreditCard.CreditCardNo = strCreditCardNo;
                            screenCreditCard.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            screenCreditCard.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            screenCreditCard.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            screenCreditCard.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                            List<tbt_CreditCard> doTbt_CreditCardList = new List<tbt_CreditCard>();
                            doTbt_CreditCardList.Add(screenCreditCard);
                            billingHandler.InsertTbt_CreditCard(doTbt_CreditCardList);
                        }
                        else if (screenBillingBasic.PaymentMethod == sParam.doBillingBasic.PaymentMethod && screenBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                        {
                            // Edit by Narupon W. 28/05/2012
                            string strCreditCardNo = string.IsNullOrEmpty(screenCreditCard.CreditCardNo) ? string.Empty : screenCreditCard.CreditCardNo.Replace("-", "");
                            screenCreditCard.CreditCardNo = strCreditCardNo;
                            screenCreditCard.CreateBy = sParam.doTbt_CreditCard.CreateBy;
                            screenCreditCard.CreateDate = sParam.doTbt_CreditCard.CreateDate;
                            screenCreditCard.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            screenCreditCard.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;


                            List<tbt_CreditCard> doTbt_CreditCardList = new List<tbt_CreditCard>();
                            doTbt_CreditCardList.Add(screenCreditCard);
                            billingHandler.UpdateTbt_CreditCard(doTbt_CreditCardList);
                        }
                    }



                    //=== Billing type detail ===
                    if (sParam.doBillingTypeDetailList != null)
                    {

                        if (sParam.doBillingTypeDetailList == null)
                        {
                            sParam.doBillingTypeDetailList = new List<doBillingTypeDetailList>();
                        }

                        // == Add new ==
                        var billingTypeDetail_addnew = sParam.doBillingTypeDetailList.Where(p => !(sParam.doBillingTypeDetailListPrevious.Select(m => m.BillingTypeCode).Contains(p.BillingTypeCode))).ToList<doBillingTypeDetailList>();

                        foreach (var item in billingTypeDetail_addnew)
                        {
                            tbt_BillingTypeDetail TempTbt_BillingTypeDetail;

                            if (item.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE
                                || item.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE
                                || item.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN
                                || item.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL)
                            {
                                TempTbt_BillingTypeDetail = new tbt_BillingTypeDetail()
                                {
                                    ContractCode = sParam.ContractProjectCodeLong,
                                    BillingOCC = sParam.BillingOCC,
                                    BillingTypeCode = item.BillingTypeCode,
                                    InvoiceDescriptionEN = item.InvoiceDescriptionEN,
                                    InvoiceDescriptionLC = item.InvoiceDescriptionLC,
                                    IssueInvoiceFlag = true,
                                    ProductCode = null,
                                    ProductTypeCode = null,
                                    BillingTypeGroup = item.BillingTypeGroup,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                                };
                            }
                            else
                            {
                                TempTbt_BillingTypeDetail = new tbt_BillingTypeDetail()
                                {
                                    ContractCode = sParam.ContractProjectCodeLong,
                                    BillingOCC = sParam.BillingOCC,
                                    BillingTypeCode = item.BillingTypeCode,
                                    InvoiceDescriptionEN = item.IsSpecificInvoiceDescription.GetValueOrDefault(false) ? item.InvoiceDescriptionEN : null,
                                    InvoiceDescriptionLC = item.IsSpecificInvoiceDescription.GetValueOrDefault(false) ? item.InvoiceDescriptionLC : null,
                                    IssueInvoiceFlag = true,
                                    ProductCode = null,
                                    ProductTypeCode = null,
                                    BillingTypeGroup = item.BillingTypeGroup,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                                };
                            }

                            

                            billingHandler.CreateBillingTypeDetail(TempTbt_BillingTypeDetail); //----*
                        }

                        // == Update ==
                        var billingTypeDetail_update = sParam.doBillingTypeDetailList.Where(p => sParam.doBillingTypeDetailListPrevious.Select(m => m.BillingTypeCode).Contains(p.BillingTypeCode)).ToList<doBillingTypeDetailList>();

                        List<tbt_BillingTypeDetail> billingTypeDetail_UpdateList = new List<tbt_BillingTypeDetail>();

                        foreach (var item in billingTypeDetail_update)
                        {
                            tbt_BillingTypeDetail TempTbt_BillingTypeDetail = new tbt_BillingTypeDetail()
                                {
                                    ContractCode = sParam.ContractProjectCodeLong,
                                    BillingOCC = sParam.BillingOCC,
                                    BillingTypeCode = item.BillingTypeCode,
                                    InvoiceDescriptionEN = item.IsSpecificInvoiceDescription.GetValueOrDefault(false) ? item.InvoiceDescriptionEN : null,
                                    InvoiceDescriptionLC = item.IsSpecificInvoiceDescription.GetValueOrDefault(false) ? item.InvoiceDescriptionLC : null,
                                    IssueInvoiceFlag = true,
                                    ProductCode = null,
                                    ProductTypeCode = null,
                                    BillingTypeGroup = item.BillingTypeGroup,
                                    CreateBy = item.CreateBy,
                                    CreateDate = item.CreateDate,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                                };


                            billingTypeDetail_UpdateList.Add(TempTbt_BillingTypeDetail);
                        }

                        billingHandler.UpdateTbt_BillingTypeDetail(billingTypeDetail_UpdateList); //----*


                        // == Delete ==
                        var billingTypeDetail_delete = sParam.doBillingTypeDetailListPrevious.Where(p => !(sParam.doBillingTypeDetailList.Select(m => m.BillingTypeCode).Contains(p.BillingTypeCode))).ToList<doBillingTypeDetailList>();

                        foreach (var item in billingTypeDetail_delete)
                        {
                            billingHandler.DeleteTbt_BillingTypeDetail(sParam.ContractProjectCodeLong, sParam.BillingOCC, item.BillingTypeCode);
                        }



                    }


                    // === Monthly Billing History ===

                    if (sParam.doTbt_MonthlyBillingHistoryList == null)
                    {
                        sParam.doTbt_MonthlyBillingHistoryList = new List<doTbt_MonthlyBillingHistoryList>();
                    }

                    // Data from screen
                    var screenBillingHistoryDate = new List<DateTime?>();
                    screenBillingHistoryDate.Add(changingDateList.BillingStartDate0);
                    screenBillingHistoryDate.Add(changingDateList.BillingStartDate1);
                    screenBillingHistoryDate.Add(changingDateList.BillingStartDate2);
                    screenBillingHistoryDate.Add(changingDateList.BillingStartDate3);
                    screenBillingHistoryDate.Add(changingDateList.BillingStartDate4);
                    screenBillingHistoryDate.Add(changingDateList.BillingStartDate5);

                    //Add by Jutarat A. on 25042013
                    var screenBillingHistoryAmount = new List<decimal?>();
                    screenBillingHistoryAmount.Add(monthlyBillingAmountList.MonthlyBillingAmount0);
                    screenBillingHistoryAmount.Add(monthlyBillingAmountList.MonthlyBillingAmount1);
                    screenBillingHistoryAmount.Add(monthlyBillingAmountList.MonthlyBillingAmount2);
                    screenBillingHistoryAmount.Add(monthlyBillingAmountList.MonthlyBillingAmount3);
                    screenBillingHistoryAmount.Add(monthlyBillingAmountList.MonthlyBillingAmount4);
                    screenBillingHistoryAmount.Add(monthlyBillingAmountList.MonthlyBillingAmount5);
                //End Add

                // Add by Jirawat Jannet on 2016-08-18

                List<string> currencyCodes = new List<string>();
                currencyCodes.Add(monthlyBillingAmountList.MonthlyBillingAmount0MulC);
                currencyCodes.Add(monthlyBillingAmountList.MonthlyBillingAmount1MulC);
                currencyCodes.Add(monthlyBillingAmountList.MonthlyBillingAmount2MulC);
                currencyCodes.Add(monthlyBillingAmountList.MonthlyBillingAmount3MulC);
                currencyCodes.Add(monthlyBillingAmountList.MonthlyBillingAmount4MulC);
                currencyCodes.Add(monthlyBillingAmountList.MonthlyBillingAmount5MulC);

                // End Add

                //changingDateList
                List<tbt_MonthlyBillingHistory> billingHistoryList = new List<tbt_MonthlyBillingHistory>();

                    // Transfer value
                    for (int i = 0; i < sParam.doTbt_MonthlyBillingHistoryList.Count; i++)
                    {
                        if (i < screenBillingHistoryDate.Count)
                        {
                            sParam.doTbt_MonthlyBillingHistoryList[i].BillingStartDate = screenBillingHistoryDate[i];
                        }

                        //Add by Jutarat A. on 25042013
                        if (i < screenBillingHistoryAmount.Count)
                        {
                            sParam.doTbt_MonthlyBillingHistoryList[i].MonthlyBillingAmount = screenBillingHistoryAmount[i];
                        }
                    //End Add

                    // ADd by Jirawat Jannet on 2017-08-18
                    if (i < currencyCodes.Count)
                    {
                        sParam.doTbt_MonthlyBillingHistoryList[i].MonthlyBillingAmountCurrencyType = currencyCodes[i];
                    }
                    // End Add 

                        // Update value of BillingEndDate(n-1) =  BillingStartDate(n).AddDays(-1)
                        if (i > 0)
                        {
                            if (sParam.doTbt_MonthlyBillingHistoryList[i - 1].BillingStartDate.HasValue)
                            {
                                var newEndDate = Convert.ToDateTime(sParam.doTbt_MonthlyBillingHistoryList[i - 1].BillingStartDate);
                                newEndDate = newEndDate.AddDays(-1);

                                if (sParam.doTbt_MonthlyBillingHistoryList[i].BillingEndDate != null)
                                {
                                    if (sParam.doTbt_MonthlyBillingHistoryList[i].BillingStartDate > newEndDate)
                                    {
                                        sParam.doTbt_MonthlyBillingHistoryList[i].BillingEndDate = sParam.doTbt_MonthlyBillingHistoryList[i].BillingStartDate;
                                    }
                                    else
                                    {
                                        sParam.doTbt_MonthlyBillingHistoryList[i].BillingEndDate = newEndDate;
                                    }
                                }

                            }
                        }
                    }

                    // Prepare !!
                    foreach (var item in sParam.doTbt_MonthlyBillingHistoryList)
                    {
                    var billingHistory = new tbt_MonthlyBillingHistory()
                    {
                        ContractCode = item.ContractCode,
                        BillingOCC = item.BillingOCC,
                        HistoryNo = item.HistoryNo,
                        MonthlyBillingAmount = item.MonthlyBillingAmountCurrencyType == "1" ? item.MonthlyBillingAmount : null,
                        BillingStartDate = item.BillingStartDate, // --
                        BillingEndDate = item.BillingEndDate, // --
                        CreateDate = item.CreateDate,
                        CreateBy = item.CreateBy,
                        UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                        MonthlyBillingAmountCurrencyType = item.MonthlyBillingAmountCurrencyType,
                        MonthlyBillingAmountUsd = item.MonthlyBillingAmountCurrencyType == "2" ? item.MonthlyBillingAmount : null
                    };

                        billingHistoryList.Add(billingHistory);
                    }


                    // Update
                    billingHandler.UpdateTbt_MonthlyBillingHistoryData(billingHistoryList);


                scope.Complete(); //*****************************************************
                res.ResultData = true;
                } //*************************************************************************

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get billing type list
        /// </summary>
        /// <param name="ProductTypeCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BLS040_GetBillingType(string ProductTypeCode)
        {

            ObjectResultData res = new ObjectResultData();
            try
            {
                BLS040_ScreenParameter sParam = GetScreenObject<BLS040_ScreenParameter>();
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


                //// tt
                //List<tbm_BillingType> BillingTypeList = billingMasterHandler.GetBillingTypeOneTimeListData(strBillingServiceTypeCode);

                // GetBillingTypeList 
                List<tbm_BillingType> BillingTypeList = billingMasterHandler.GetBillingTypeList(strBillingServiceTypeCode);


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
        public ActionResult BLS040_AddBillingType(string BillingTypeCode, string InvoiceDescriptionEN, string InvoiceDescriptionLC)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                if (string.IsNullOrEmpty(BillingTypeCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6090);
                    return Json(res);
                }

                BLS040_ScreenParameter sParam = GetScreenObject<BLS040_ScreenParameter>();

                IBillingMasterHandler billingMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                List<tbm_BillingType> BillingTypeList = billingMasterHandler.GetBillingTypeList(sParam.strBillingServiceTypeCode);
                CommonUtil.MappingObjectLanguage<tbm_BillingType>(BillingTypeList);

                List<tbm_BillingType> BillingTypeList2 = (from t in BillingTypeList
                                                          where t.BillingTypeCode == BillingTypeCode
                                                          select t).ToList<tbm_BillingType>();

                List<doBillingTypeDetailList> BillingTypeDetailExistList = (from t in sParam.doBillingTypeDetailList
                                                                            where t.BillingTypeCode == BillingTypeCode
                                                                            select t).ToList<doBillingTypeDetailList>();

                if (BillingTypeDetailExistList.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6004,
                                         new string[] { BillingTypeList2[0].BillingTypeCodeName },
                                         null);
                    return Json(res);
                }

                doBillingTypeDetailList BillingTypeDetail = new doBillingTypeDetailList();

                ISaleContractHandler handlerSaleContract = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                var saleContractBasic_list = handlerSaleContract.GetSaleContractBasicForView(sParam.ContractProjectCodeLong);

                foreach (tbm_BillingType BillType in BillingTypeList2)
                {
                    //Used user defined invoice desciption
                    if (string.IsNullOrEmpty(InvoiceDescriptionEN) == false || string.IsNullOrEmpty(InvoiceDescriptionLC) == false)
                    {

                        BillType.BillingTypeNameEN = InvoiceDescriptionEN;
                        BillType.BillingTypeNameLC = InvoiceDescriptionLC;
                        BillingTypeDetail.InvoiceDescriptionEN = InvoiceDescriptionEN;
                        BillingTypeDetail.InvoiceDescriptionLC = InvoiceDescriptionLC;
                        BillingTypeDetail.IsSpecificInvoiceDescription = true;
                    }
                    else
                    {
                        if (BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE
                            || BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE
                            || BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN
                            || BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL)
                        {
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
                            BillingTypeDetail.InvoiceDescriptionEN = null;
                            BillingTypeDetail.InvoiceDescriptionLC = null;

                        }

                        BillingTypeDetail.IsSpecificInvoiceDescription = false;

                    }

                    BillingTypeDetail.BillingTypeCode = BillType.BillingTypeCode;
                    BillingTypeDetail.BillingTypeGroup = BillType.BillingTypeCode;
                    sParam.doBillingTypeDetailList.Add(BillingTypeDetail);
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
        /// Update billing type data
        /// </summary>
        /// <param name="BillingTypeCode"></param>
        /// <param name="InvoiceDescriptionEN"></param>
        /// <param name="InvoiceDescriptionLC"></param>
        /// <returns></returns>
        public ActionResult BLS040_UpdateBillingType(string BillingTypeCode, string InvoiceDescriptionEN, string InvoiceDescriptionLC)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                BLS040_ScreenParameter sParam = GetScreenObject<BLS040_ScreenParameter>();


                foreach (doBillingTypeDetailList BillType in sParam.doBillingTypeDetailList)
                {
                    if (BillType.BillingTypeCode == BillingTypeCode)
                    {
                        //Used user defined invoice desciption (always on edit mode)
                        BillType.InvoiceDescriptionEN = InvoiceDescriptionEN;
                        BillType.InvoiceDescriptionLC = InvoiceDescriptionLC;
                        BillType.IsSpecificInvoiceDescription = true;
                    }
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

        /// <summary>
        /// Remove billing type from selected list
        /// </summary>
        /// <param name="BillingTypeCode"></param>
        /// <param name="BillingFlag"></param>
        /// <returns></returns>
        public ActionResult BLS040_RemoveBillingTypeClick(string BillingTypeCode, string BillingFlag)
        {

            ObjectResultData res = new ObjectResultData();
            try
            {
                BLS040_ScreenParameter sParam = GetScreenObject<BLS040_ScreenParameter>();

                if (sParam.doBillingTypeDetailList == null)
                {
                    sParam.doBillingTypeDetailList = new List<doBillingTypeDetailList>();
                }

                string strBillingTypeGrop = string.Empty;
                var list = (from p in sParam.doBillingTypeDetailList where p.BillingTypeCode == BillingTypeCode select p.BillingTypeGroup).ToList<string>();
                if (list.Count > 0)
                {
                    strBillingTypeGrop = list[0];
                }



                // === Validate ===

                if ((BillingFlag == StopBillingFlag.C_BILLING_FLAG_CONTRACT_FEE || (BillingFlag == StopBillingFlag.C_BILLING_FLAG_STOP && sParam.doBillingBasic.StartOperationDate.HasValue == false)) && strBillingTypeGrop == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.ResultData = false;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6008,
                                         new string[] { "lblBillingType" },
                                         null);
                }
                else
                {
                    sParam.doBillingTypeDetailList.RemoveAll(t => t.BillingTypeCode == BillingTypeCode);
                    res.ResultData = true;
                }

                return Json(res);
            }
            catch (Exception ex)
            {

                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Get billing target data by new billing client code , new billing target running no
        /// </summary>
        /// <param name="BillingClientCode"></param>
        /// <param name="BillingTargetRunningNo"></param>
        /// <returns></returns>
        public ActionResult BLS040_GetBillingTypeTargetNew(string BillingClientCode, string BillingTargetRunningNo)
        {

            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            ValidatorUtil validator = new ValidatorUtil();
            try
            {

                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                BLS040_ScreenParameter sParam = GetScreenObject<BLS040_ScreenParameter>();

                // === Mandatory check ===

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (String.IsNullOrEmpty(BillingClientCode))
                {

                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "BillingClientCode",
                                         "lblBillingTargetCode",
                                         "BillingClientCode");
                }

                if (String.IsNullOrEmpty(BillingTargetRunningNo))
                {

                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "BillingTargetNo",
                                         "lblBillingTargetCode",
                                         "BillingTargetNo");
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    return Json(res);
                }


                // === Billing Target ===
                sParam.BillingClientCode = "";
                sParam.BillingTargetRunningNo = "";
                sParam.doBillingTarget = null;

                string strBillingClientCodeLong = comUtil.ConvertBillingClientCode(BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                List<dtTbt_BillingTargetForView> lst = billingHandler.GetTbt_BillingTargetForView(strBillingClientCodeLong + "-" + BillingTargetRunningNo, MiscType.C_CUST_TYPE);
                CommonUtil.MappingObjectLanguage<dtTbt_BillingTargetForView>(lst);

                if (lst.Count > 0)
                {
                    sParam.BillingClientCode = strBillingClientCodeLong;
                    sParam.BillingTargetRunningNo = BillingTargetRunningNo;
                    sParam.doBillingTarget = lst[0];
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS040",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6040,
                                         null,
                                         new string[] { "BillingClientCode", "BillingTargetNo" });
                    return Json(res);
                }

                res.ResultData = sParam.doBillingTarget;

                return Json(res);
            }
            catch (Exception ex)
            {

                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Clear billing target
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS040_ClearBillingTarget()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                BLS040_ScreenParameter sParam = GetScreenObject<BLS040_ScreenParameter>();
                sParam.doBillingTarget = null;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
    }
}
