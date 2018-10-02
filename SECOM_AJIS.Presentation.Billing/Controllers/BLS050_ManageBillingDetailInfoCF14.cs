
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
using SECOM_AJIS.Presentation.Common.Helpers;
using SECOM_AJIS.Presentation.Common;

namespace SECOM_AJIS.Presentation.Billing.Controllers
{
    public partial class BillingController : BaseController
    {
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS050_Authority(BLS050_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();

            // System Suspend
            //ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            //if (handlerCommon.IsSystemSuspending())
            //{
            //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
            //    return Json(res);
            //}

            //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DETAIL, FunctionID.C_FUNC_ID_OPERATE))
            //{
            //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
            //    return Json(res);
            //}

            return InitialScreenEnvironment<BLS050_ScreenParameter>("BLS050", param, res);

        }
        /// <summary>
        /// Check User have Delete permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS050_ChkDelPermission(BLS050_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();

            try
            {
                res.ResultData = "1";

                // for debug
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DETAIL, FunctionID.C_FUNC_ID_CANCEL_BILLING_DETAIL))
                {
                    // show in javascript
                    //res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6023);
                    res.ResultData = "0";
                }
                // end for debug
                return Json(res);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }
        /// <summary>
        /// Check User have Operate permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS050_ChkOperatePermission(BLS050_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();

            try
            {
                res.ResultData = "1";

                //(FunctionID = C_FUNC_ID_OPERATE)
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DETAIL, FunctionID.C_FUNC_ID_OPERATE))
                {
                    // show in javascript
                    //res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6087);
                    res.ResultData = "0";
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
        /// Check User have Operate or Operate and Special Create permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS050_ChkOperateOrSpCreatePermission(BLS050_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();

            try
            {
                res.ResultData = "0";

                if (CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DETAIL, FunctionID.C_FUNC_ID_OPERATE) ||
                CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DETAIL, FunctionID.C_FUNC_ID_SPECIAL_CREATE))
                {
                    // show in javascript
                    //res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6087);
                    res.ResultData = "1";
                    return Json(res);
                }

                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DETAIL, FunctionID.C_FUNC_ID_SPECIAL_CREATE))
                //{
                //    // show in javascript
                //    //res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6087);
                //    res.ResultData = "0";
                //    return Json(res);
                //}

                return Json(res);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS050")]
        public ActionResult BLS050()
        {
            //BillingTypeGroup
            ViewBag.C_BILLING_TYPE_GROUP_DEPOSIT = BillingTypeGroup.C_BILLING_TYPE_GROUP_DEPOSIT;
            ViewBag.C_BILLING_TYPE_GROUP_CONTINUES = BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES;
            ViewBag.C_BILLING_TYPE_GROUP_SALE = BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE;
            ViewBag.C_BILLING_TYPE_GROUP_INSTALL = BillingTypeGroup.C_BILLING_TYPE_GROUP_INSTALL;
            ViewBag.C_BILLING_TYPE_GROUP_DIFF_AMOUNT = BillingTypeGroup.C_BILLING_TYPE_GROUP_DIFF_AMOUNT;
            ViewBag.C_BILLING_TYPE_GROUP_OTHER = BillingTypeGroup.C_BILLING_TYPE_GROUP_OTHER;

            //IssueInv
            ViewBag.C_ISSUE_INV_NORMAL = IssueInv.C_ISSUE_INV_NORMAL;
            ViewBag.C_ISSUE_INV_REALTIME = IssueInv.C_ISSUE_INV_REALTIME;
            ViewBag.C_ISSUE_INV_NOT_ISSUE = IssueInv.C_ISSUE_INV_NOT_ISSUE;

            //PaymentMethodType
            ViewBag.C_PAYMENT_METHOD_AUTO_TRANFER = PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER;
            ViewBag.C_PAYMENT_METHOD_BANK_TRANSFER = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
            ViewBag.C_PAYMENT_METHOD_CREDIT_CARD = PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD;
            ViewBag.C_PAYMENT_METHOD_MESSENGER = PaymentMethodType.C_PAYMENT_METHOD_MESSENGER;

            return View();
        }

        /// <summary>
        /// Generate xml for initial cancel billing detail grid
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS050_InitialCancelBillingDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS050_CancelBillingDetail", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Generate xml for initial issue billing detail grid
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS050_InitialIssueBillingDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS050_IssueBillingDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        public ActionResult BLS050_GenerateCurrencyNumericTextBox(string id,string value, string currency)
        {
            string html = TextBoxHelper.NumericTextBoxWithMultipleCurrency(null, id, value, currency, new { style = "width: 140px;" }).ToString();
            return Json(html);
        }

        /// <summary>
        /// Generate billing type comboitem list 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult BLS050_GetComboBoxBillingType(string id)
        {

            BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();
            CommonUtil comUtil = new CommonUtil();
            List<doBillingTypeDetailList> lst = new List<doBillingTypeDetailList>();

            try
            {
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                lst = billingHandler.GetBillingTypeDetailList(
                    comUtil.ConvertBillingCode(sParam.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                    , sParam.BillingOCC);
                CommonUtil.MappingObjectLanguage<doBillingTypeDetailList>(lst);
                ComboBoxModel cboModel = new ComboBoxModel();

                //if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN ||
                //    CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                //{

                //    if (lst != null)
                //    {
                //        cboModel.SetList<doBillingTypeDetailList>(lst, "InvoiceDescriptionEN", "BillingTypeCode", true);
                //    }
                //    else
                //    {
                //        doBillingTypeDetailList dummy = new doBillingTypeDetailList();
                //        dummy.BillingTypeCode = "";
                //        dummy.InvoiceDescriptionEN = "Not Found Data";
                //        lst.Add(dummy);
                //        cboModel.SetList<doBillingTypeDetailList>(lst, "InvoiceDescriptionEN", "BillingTypeCode", true);
                //    }
                //}
                //else
                //{
                //    if (lst != null)
                //    {
                //        cboModel.SetList<doBillingTypeDetailList>(lst, "BillingTypeBLS050", "BillingTypeCode", true);
                //    }
                //    else
                //    {
                //        doBillingTypeDetailList dummy = new doBillingTypeDetailList();
                //        dummy.BillingTypeCode = "";
                //        dummy.InvoiceDescriptionLC = "Not Found Data";
                //        lst.Add(dummy);
                //        cboModel.SetList<doBillingTypeDetailList>(lst, "BillingTypeBLS050", "BillingTypeCode", true);
                //    }
                //}
                if (lst != null)
                {
                    cboModel.SetList<doBillingTypeDetailList>(lst, "BillingTypeBLS050", "BillingTypeCode", true);
                }
                else
                {
                    doBillingTypeDetailList dummy = new doBillingTypeDetailList();
                    dummy.BillingTypeCode = "";
                    dummy.InvoiceDescriptionLC = "Not Found Data";
                    lst.Add(dummy);
                    cboModel.SetList<doBillingTypeDetailList>(lst, "BillingTypeBLS050", "BillingTypeCode", true);
                }

                return Json(cboModel);

            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }
        /// <summary>
        /// Generate issue invoice comboitem list 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult BLS050_GetComboBoxIssueInvoice(string id)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();

            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_ISSUE_INV,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(lst, "ValueDisplay", "ValueCode", false);
                return Json(cboModel);
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Generate payment method comboitem list
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult BLS050_GetComboBoxPaymentMethod(string id)
        {
            BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();

            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            List<doMiscTypeCode> Templst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_PAYMENT_METHOD,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                Templst = hand.GetMiscTypeCodeList(miscs);

                if (sParam._doBLS050GetBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER)
                {
                    //-	C_PAYMENT_METHOD_BANK_TRANSFER
                    //-	C_PAYMENT_METHOD_MESSENGER
                    for (int i = 0; i < Templst.Count; i++)
                    {
                        if (Templst[i].ValueCode == PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER ||
                            Templst[i].ValueCode == PaymentMethod.C_PAYMENT_METHOD_MESSENGER_TRANSFER)
                        {
                            lst.Add(Templst[i]);
                        }
                    }
                }
                if (sParam._doBLS050GetBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_MESSENGER_TRANSFER)
                {
                    //-	C_PAYMENT_METHOD_BANK_TRANSFER
                    //-	C_PAYMENT_METHOD_MESSENGER
                    for (int i = 0; i < Templst.Count; i++)
                    {
                        if (Templst[i].ValueCode == PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER ||
                            Templst[i].ValueCode == PaymentMethod.C_PAYMENT_METHOD_MESSENGER_TRANSFER)
                        {
                            lst.Add(Templst[i]);
                        }
                    }
                }
                if (sParam._doBLS050GetBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                {
                    //-	C_PAYMENT_METHOD_BANK_TRANSFER
                    //-	C_PAYMENT_METHOD_MESSENGER
                    //-	C_PAYMENT_METHOD_AUTO_TRANSFER
                    for (int i = 0; i < Templst.Count; i++)
                    {
                        if (Templst[i].ValueCode == PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER ||
                            Templst[i].ValueCode == PaymentMethod.C_PAYMENT_METHOD_MESSENGER_TRANSFER ||
                            Templst[i].ValueCode == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                        {
                            lst.Add(Templst[i]);
                        }
                    }
                }
                if (sParam._doBLS050GetBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                {
                    //-	C_PAYMENT_METHOD_BANK_TRANSFER
                    //-	C_PAYMENT_METHOD_MESSENGER
                    //-	C_PAYMENT_METHOD_CREDIT_CARD
                    for (int i = 0; i < Templst.Count; i++)
                    {
                        if (Templst[i].ValueCode == PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER ||
                            Templst[i].ValueCode == PaymentMethod.C_PAYMENT_METHOD_MESSENGER_TRANSFER ||
                            Templst[i].ValueCode == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                        {
                            lst.Add(Templst[i]);
                        }
                    }

                }

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(lst, "ValueDisplay", "ValueCode", true, CommonUtil.eFirstElementType.Short);
                return Json(cboModel);
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Generate billing detils invoice format comboitem list
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult BLS050_GetComboBoxBillingDetilsInvoiceFormat(string id)
        {
            BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();

            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            List<doMiscTypeCode> Templst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_BILLING_INV_FORMAT,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                Templst = hand.GetMiscTypeCodeList(miscs);

                if (sParam._doBLS050GetTbt_BillingTargetForView.InvFormatType == InvFormatType.C_INV_FORMAT_SPECIFIC)
                {
                    //-	C_BILLING_INV_FORMAT_SPECIFIC 
                    for (int i = 0; i < Templst.Count; i++)
                    {
                        if (Templst[i].ValueCode == BillingInvFormatType.C_BILLING_INV_FORMAT_SPECIFIC)
                        {
                            lst.Add(Templst[i]);
                        }
                    }
                }
                else
                {
                    //-	C_BILLING_INV_FORMAT_INV
                    //-	C_BILLING_INV_FORMAT_INV_TAXINV

                    for (int i = 0; i < Templst.Count; i++)
                    {
                        if (Templst[i].ValueCode == BillingInvFormatType.C_BILLING_INV_FORMAT_INV ||
                            Templst[i].ValueCode == BillingInvFormatType.C_BILLING_INV_FORMAT_INV_TAXINV)
                        {
                            lst.Add(Templst[i]);
                        }
                    }
                }

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(lst, "ValueDisplay", "ValueCode", false);
                return Json(cboModel);
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }


        /// <summary>
        /// Generate billing detils invoice format comboitem list
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult BLS050_GetComboBoxContractOCC(string id)
        {
            BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();

            ComboBoxModel cboModel = new ComboBoxModel();
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                string contractCode = new CommonUtil().ConvertContractCode(sParam.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                List<ComboObjectModel> lstItems = new List<ComboObjectModel>();
                IRentralContractHandler rentalHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<tbt_RentalSecurityBasic> dtRentalSecurity = rentalHandler.GetTbt_RentalSecurityBasic(contractCode, null);
                if (dtRentalSecurity != null && dtRentalSecurity.Count > 0)
                {
                    lstItems = dtRentalSecurity.Select(d => new ComboObjectModel() { Code = d.OCC, DisplayName = d.OCC }).ToList();
                }
                else
                {
                    ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                    List<tbt_SaleBasic> dtSaleBasic = saleHandler.GetTbt_SaleBasic(contractCode, null, null);
                    if (dtSaleBasic != null && dtSaleBasic.Count > 0)
                    {
                        lstItems = dtSaleBasic.Select(d => new ComboObjectModel() { Code = d.OCC, DisplayName = d.OCC }).ToList();
                    }
                }

                if (lstItems.Count > 0 && lstItems.Any(d => !string.IsNullOrEmpty(d.Code) && d.Code.StartsWith("9")))
                {
                    int? minOCC = (
                        from d in lstItems
                        where !string.IsNullOrEmpty(d.Code) && d.Code.StartsWith("9")
                        let occ = int.Parse(d.Code)
                        select occ
                    ).Min();
                    if (minOCC != null && minOCC > 9000)
                    {
                        minOCC = minOCC - 10;
                        lstItems.Add(new ComboObjectModel() { Code = minOCC.Value.ToString("0000"), DisplayName = minOCC.Value.ToString("0000") });
                    }
                }

                if (!lstItems.Any(d => d.Code == "9990"))
                {
                    lstItems.Add(new ComboObjectModel() { Code = "9990", DisplayName = "9990" });
                }

                lstItems = (
                    from d in lstItems
                    let sortobj = new
                     {
                         ComboObject = d,
                         SortIndex = (!string.IsNullOrEmpty(d.Code) && d.Code.StartsWith("9") ? -1 : 1)
                     }
                     orderby sortobj.SortIndex descending, sortobj.ComboObject.Code descending
                     select sortobj.ComboObject
                 ).ToList();

                cboModel.SetList<ComboObjectModel>(lstItems, "DisplayName", "Code", true, CommonUtil.eFirstElementType.Short);

                return Json(cboModel);
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }


        /// <summary>
        /// Calculate billing amount by period and billing type
        /// </summary>
        /// <param name="dtpFrom">Business Date From</param>
        /// <param name="dtpTo">Business Date To</param>
        /// <param name="BillingTypeCode">Billing Type Code</param>
        /// <returns>Billing Amount</returns>
        public ActionResult BLS050_CalculateBillingAmount(DateTime? dtpFrom, DateTime? dtpTo, string BillingTypeCode)
        {

            try
            {
                //string strBillingAmount;
                decimal decBillingAmount;

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();
                CommonUtil comUtil = new CommonUtil();

                if (BLS050_fnGetBillingTypeGroup(BillingTypeCode) == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                {
                    decBillingAmount = billingHandler.CalculateBillingAmount(
                        sParam._doBLS050GetBillingBasic.ContractCode,
                        sParam._doBLS050GetBillingBasic.BillingOCC,
                        sParam._doBLS050GetBillingBasic.CalDailyFeeStatus,
                        dtpFrom,
                        dtpTo);
                    //strBillingAmount = decBillingAmount.ToString();
                    return Json(decBillingAmount);
                }
                else
                {
                    return Json(0);
                }
            }
            catch (Exception ex)
            {
                //ObjectResultData res = new ObjectResultData();
                //res.AddErrorMessage(ex);
                //return Json(res);
                return Json(0);
            }
        }

        /// <summary>
        /// Retrieve billing information list of specific screen mode and search criteria information
        /// </summary>
        /// <param name="ContractCode">Search criteria</param>
        /// <param name="BillingOCC">Search criteria</param>
        /// <returns></returns>
        public ActionResult BLS050_RetrieveData(string ContractCode, string BillingOCC)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            ICommonContractHandler commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            string lang = CommonUtil.GetCurrentLanguage();

            doBLS050GetBillingBasic _doBLS050GetBillingBasic = new doBLS050GetBillingBasic();
            List<doBLS050GetBillingDetailForCancelList> _doBLS050GetBillingDetailForCancelList = new List<doBLS050GetBillingDetailForCancelList>();
            doBLS050GetTbt_BillingTargetForView _doBLS050GetTbt_BillingTargetForView = new doBLS050GetTbt_BillingTargetForView();

            try
            {
                // Is System Suspend
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidatorUtil validator = new ValidatorUtil();

                BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();

                if (String.IsNullOrEmpty(ContractCode))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS050",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                          "BillingCode",
                                          "lblBillingCode",
                                          "BillingCode");
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS050",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                          "BillingOCC",
                                          "lblBillingCode",
                                          "BillingOCC");
                }
                else
                {
                    if (String.IsNullOrEmpty(BillingOCC))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                              "BillingCode",
                                              "lblBillingCode",
                                              "BillingCode");
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                              "BillingOCC",
                                              "lblBillingCode",
                                              "BillingOCC");
                    }
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    return Json(res);
                }

                string billingCodeShortFullFormat = string.Format("{0}-{1}", ContractCode, BillingOCC);

                _doBLS050GetBillingBasic = billingHandler.BLS050_GetBillingBasic(comUtil.ConvertBillingCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                                                                                        , BillingOCC);

                // add by jirawat jannet
                if (string.IsNullOrEmpty(_doBLS050GetBillingBasic.AdjustBillingPeriodAmountCurrencyType))
                    _doBLS050GetBillingBasic.AdjustBillingPeriodAmountCurrencyType = "01";// _doBLS050GetBillingBasic.MonthlyBillingAmountCurrency; 


                if (_doBLS050GetBillingBasic == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_BILLING,
                                             MessageUtil.MessageList.MSG6041,
                                              "BillingCode",
                                              "lblBillingCode",
                                              "BillingCode");
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS050",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6041,
                                          "BillingOCC",
                                          "lblBillingCode",
                                          "BillingOCC");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    if (res.IsError)
                    {
                        return Json(res);
                    }

                }
                if (_doBLS050GetBillingBasic.ContractCode == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_BILLING,
                                             MessageUtil.MessageList.MSG6041,
                                              "BillingCode",
                                              "lblBillingCode",
                                              "BillingCode");
                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS050",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6041,
                                          "BillingOCC",
                                          "lblBillingCode",
                                          "BillingOCC");
                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    if (res.IsError)
                    {
                        return Json(res);
                    }
                }

                List<dtTbt_BillingTargetForView> lstBillingTarget = billingHandler.GetTbt_BillingTargetForView(_doBLS050GetBillingBasic.BillingTargetCode, MiscType.C_CUST_TYPE);
                if (lstBillingTarget.Count > 0)
                {
                    var existsBillingOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == lstBillingTarget[0].BillingOfficeCode);
                    if (existsBillingOffice.Count() <= 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0063,
                                            null,
                                             new string[] { "BillingCode", "BillingOCC" });
                        return Json(res);
                    }
                }

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (_doBLS050GetBillingBasic != null)
                {
                    if (_doBLS050GetBillingBasic.CarefulFlag == true)
                    {
                        //MSG6056
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS050",
                                         MessageUtil.MODULE_BILLING,
                                         MessageUtil.MessageList.MSG6056,
                                          "BillingCode",
                                          billingCodeShortFullFormat,
                                          "BillingCode");

                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS050",
                                        MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6056,
                                        "BillingOCC",
                                        billingCodeShortFullFormat,
                                        "BillingOCC");

                        ValidatorUtil.BuildErrorMessage(res, validator, null);
                        if (res.IsError)
                        {
                            return Json(res);
                        }
                        //res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                        //                        "BLS050",
                        //                        MessageUtil.MODULE_BILLING,
                        //                        MessageUtil.MessageList.MSG6022,
                        //                        new string[] { _doBLS050GetBillingBasic.ContractCode + "-" + _doBLS050GetBillingBasic.BillingOCC },
                        //                        new string[] { "lblBillingCode", "BillingCode" });

                        //return Json(res);
                    }
                    // under this is not CarefulFlag == true case
                    sParam._doBLS050GetBillingBasic = _doBLS050GetBillingBasic;

                    _doBLS050GetTbt_BillingTargetForView = billingHandler.BLS050_GetTbt_BillingTargetForView(_doBLS050GetBillingBasic.BillingTargetCode);
                    sParam._doBLS050GetTbt_BillingTargetForView = _doBLS050GetTbt_BillingTargetForView;


                    if (_doBLS050GetBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                    {
                        sParam.dotbt_AutoTransferBankAccount = billingHandler.GetTbt_AutoTransferBankAccount(
                            _doBLS050GetBillingBasic.ContractCode
                            , _doBLS050GetBillingBasic.BillingOCC);
                    }
                    else if (_doBLS050GetBillingBasic.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                    {
                        sParam.doTbt_CreditCard = billingHandler.GetTbt_CreditCard(
                            _doBLS050GetBillingBasic.ContractCode
                            , _doBLS050GetBillingBasic.BillingOCC);

                    }
                }

                _doBLS050GetBillingDetailForCancelList = billingHandler.BLS050_GetBillingDetailForCancelList(
                   comUtil.ConvertBillingCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                   , BillingOCC);

                int id = 1;
                foreach (var d in _doBLS050GetBillingDetailForCancelList)
                {
                    if (string.IsNullOrEmpty(d.BillingAmountCurrencyType))
                    {
                        d.BillingAmountCurrencyType = string.Empty;
                    }
                    else
                    {
                        d.BillingAmountCurrencyType = MiscellaneousTypeCommon.getCurrencyName(d.BillingAmountCurrencyType) + " " + d.BillingAmount.Value.ToString("#,##0.00");
                        //d.BillingAmountCurrency = TextBoxHelper.NumericTextBoxWithMultipleCurrency(null, "BillingCurrency" + id.ToString()
                        //    , d.BillingAmount, d.BillingAmountCurrency
                        //    , new { style = @"border: 0px; width: 130px; background-color: transparent;", @readonly = "readonly" }
                        //    , TextBoxHelper.MULTIPLE_CURRENCY_TYPE.LABEL).ToString();
                    }
                }

                if (_doBLS050GetBillingDetailForCancelList != null)
                {
                    if (_doBLS050GetBillingDetailForCancelList.Count > 0)
                    {
                        sParam._doBLS050GetBillingDetailForCancelList = _doBLS050GetBillingDetailForCancelList;
                    }
                    else
                    {
                        sParam._doBLS050GetBillingDetailForCancelList = new List<doBLS050GetBillingDetailForCancelList>();
                    }
                }
                else
                {
                    sParam._doBLS050GetBillingDetailForCancelList = new List<doBLS050GetBillingDetailForCancelList>();
                }

                sParam.ContractCode = ContractCode;
                sParam.BillingOCC = BillingOCC;

                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<dtTbt_RentalContractBasicForView> dtRentalContract = handler.GetTbt_RentalContractBasicForView(_doBLS050GetBillingBasic.ContractCode);
                if (dtRentalContract.Count > 0)
                {
                    sParam.LastOCC = dtRentalContract[0].LastOCC;
                    ViewBag.LastOCC = dtRentalContract[0].LastOCC;
                }
                else
                {
                    ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                    List<tbt_SaleBasic> dtSaleBasic = saleHandler.GetTbt_SaleBasic(_doBLS050GetBillingBasic.ContractCode, null, true);
                    if (dtSaleBasic != null && dtSaleBasic.Count > 0)
                    {
                        sParam.LastOCC = dtSaleBasic[0].OCC;
                        ViewBag.LastOCC = dtSaleBasic[0].OCC;
                    }
                }

                sParam.C_ISSUE_INV_NORMAL = IssueInv.C_ISSUE_INV_NORMAL;
                res.ResultData = sParam;

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
        public ActionResult BLS050_Register(BLS050_RegisterData data)
        {
            string conModeRadio1rdoReCreateBillingDetail = "1";
            string conModeRadio1rdoCancelBillingDetail = "2";
            string conModeRadio1rdoForceCreateBillingDetail = "3";
            string conModeRadio1rdoRegisterAdjustOnNextPeriodAmount = "4";

            BLS050_ScreenParameter param = GetScreenObject<BLS050_ScreenParameter>();
            BLS050_RegisterData RegisterData = new BLS050_RegisterData();
            CommonUtil comUtil = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ObjectResultData resRec = new ObjectResultData();
            ObjectResultData resDel = new ObjectResultData();
            ObjectResultData resIns = new ObjectResultData();
            ObjectResultData resAdj = new ObjectResultData();

            List<BLS050_WarningMessage> confirmWarningMessageID = new List<BLS050_WarningMessage>();

            try
            {
                // Is suspend ?
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }

                // input shot but all database use long code
                // convert it's here before use
                data.Header.strContractCode = comUtil.ConvertBillingCode(data.Header.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                // Check Validate Business 
                if (data.Header.rdoProcessTypeSpe == conModeRadio1rdoReCreateBillingDetail)
                {
                    #region Re-create billing detail
                    //re-create section
                    resRec = ValidateReCreateBillingDetail(data);
                    if (resRec.MessageList != null)
                    {
                        if (resRec.MessageList.Count > 0)
                        {
                            return Json(resRec);
                        }
                    }

                    //cancel  section
                    resDel = ValidateCancelBillingDetail(data, true, confirmWarningMessageID);
                    if (resDel.MessageList != null)
                    {
                        if (resDel.MessageList.Count > 0)
                        {
                            return Json(resDel);
                        }
                    }

                    //Force create section
                    resIns = ValidateForceCreateBillingDetail(data, true, confirmWarningMessageID);
                    if (resIns.MessageList != null)
                    {
                        if (resIns.MessageList.Count > 0)
                        {
                            return Json(resIns);
                        }
                    }

                    //Next period section
                    resAdj = ValidateAdjustOnNextPeriod(data);
                    if (resAdj.MessageList != null)
                    {
                        if (resAdj.MessageList.Count > 0)
                        {
                            return Json(resAdj);
                        }
                    }
                    #endregion
                }
                else if (data.Header.rdoProcessTypeSpe == conModeRadio1rdoCancelBillingDetail)
                {
                    #region Cancel billing detail
                    resDel = ValidateCancelBillingDetail(data, false, confirmWarningMessageID);
                    if (resDel.MessageList != null)
                    {
                        if (resDel.MessageList.Count > 0)
                        {
                            return Json(resDel);
                        }
                    }
                    #endregion
                }
                else if (data.Header.rdoProcessTypeSpe == conModeRadio1rdoForceCreateBillingDetail)
                {
                    #region Force create billing detail
                    resIns = ValidateForceCreateBillingDetail(data, false, confirmWarningMessageID);
                    if (resIns.MessageList != null)
                    {
                        if (resIns.MessageList.Count > 0)
                        {
                            return Json(resIns);
                        }
                    }
                    #endregion
                }
                else if (data.Header.rdoProcessTypeSpe == conModeRadio1rdoRegisterAdjustOnNextPeriodAmount)
                {
                    #region Next period amount
                    resAdj = ValidateAdjustOnNextPeriod(data);
                    if (resAdj.MessageList != null)
                    {
                        if (resAdj.MessageList.Count > 0)
                        {
                            return Json(resAdj);
                        }
                    }
                    #endregion
                }

                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }


                //Pass (may be with warning message)
                res.ResultData = new BLS050_RegisterResult()
                {
                    ResultFlag = "1",
                    ConfirmMessageID = confirmWarningMessageID,
                };
                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        class BusinessBillingTypeCheck
        {
            public string strBillingType { get; set; }
            public bool bolAlreadyIssue { get; set; }
            // Add by Patcharee T. for case set first fee flag 07-Jun-2013
            public bool? isFirstFee { get; set; }
        }

        /// <summary>
        /// Validate business in recreate billing details screen mode
        /// </summary>
        /// <param name="RegisterData">input data from screen</param>
        /// <returns></returns>
        public ObjectResultData ValidateReCreateBillingDetail(BLS050_RegisterData RegisterData)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            bool bolCheckDatePeriod = false;
            bool bolCheckSameBillingType = false;
            bool bolCheckCancelMoreThanZeroRow = false;
            bool bolCheckIssueMoreThanZeroRow = false;

            string strCheckSameBillingType = string.Empty;

            string strContractCode = RegisterData.Header.strContractCode;
            string strBillingOCC = RegisterData.Header.strBillingOCC;
            string rdoProcessTypeSpe = RegisterData.Header.rdoProcessTypeSpe;

            string strBillingtype = "";
            string strContractOCC = "";
            DateTime? dtpFrom = null;
            DateTime? dtpTo = null;
            decimal intBillingamount = 0;
            string strIssueinvoice = "";
            string strPaymentmethod = "";
            string strBillingdetailinvoiceformat = "";
            DateTime? dtpExpectedissueautotransferdate = null;

            try
            {
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                ICommonContractHandler comContractHadler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler; //Add by Jutarat A. on 29072013

                BusinessStartEndDate bolDateRangeProblem = new BusinessStartEndDate();
                BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();
                CommonUtil comUtil = new CommonUtil();
                ValidatorUtil validator = new ValidatorUtil();

                // load all billing detail with no condition
                List<doGetBillingDetailContinues> _doChecktbt_BillingDetailList = billingHandler.GetBillingDetailContinuesList(
                    sParam._doBLS050GetBillingBasic.ContractCode,
                    sParam._doBLS050GetBillingBasic.BillingOCC,
                    string.Empty);

                doGetBillingDetailContinues _doTempDeletetbt_BillingDetail = new doGetBillingDetailContinues();
                List<BusinessStartEndDate> DateRangesForDataList = new List<BusinessStartEndDate>();
                int iRowCancelBillingDetail = RegisterData.Detail1 != null ? RegisterData.Detail1.Count : 0;
                int iRowIssueBillingDetail = RegisterData.Detail2 != null ? RegisterData.Detail2.Count : 0;

                for (int i = 0; i < iRowCancelBillingDetail; i++)
                {

                    if (RegisterData.Detail1[i].bolDel)
                    {
                        bolCheckCancelMoreThanZeroRow = true;
                    }
                }
                if (!(bolCheckCancelMoreThanZeroRow))
                {
                    //check send param for screen
                    //if 0 show
                    //MSG6054
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6054,
                                                 new string[] { },
                                                 new string[] { });
                    return res;
                }

                //Add by Jutarat A. on 29072013
                bool isDiffFirstFeeFlag = false;
                bool? prevFirstFeeFlag = null;
                int iCount = 0;
                //End Add

                List<BusinessBillingTypeCheck> BusinessBillingTypeCheckList = new List<BusinessBillingTypeCheck>();
                bool bolAddFlag = true;
                for (int i = 0; i < iRowCancelBillingDetail; i++)
                {
                    if (RegisterData.Detail1[i].bolDel)
                    {
                        bolAddFlag = true;
                        foreach (BusinessBillingTypeCheck _BusinessBillingTypeCheck in BusinessBillingTypeCheckList)
                        {
                            if (_BusinessBillingTypeCheck.strBillingType == RegisterData.Detail1[i].strBillingtypeCode)
                            {
                                bolAddFlag = false;
                            }
                        }
                        if (bolAddFlag)
                        {
                            BusinessBillingTypeCheckList.Add(new BusinessBillingTypeCheck
                             {
                                 strBillingType = RegisterData.Detail1[i].strBillingtypeCode,
                                 bolAlreadyIssue = false
                             });

                        }

                        //Add by Jutarat A. on 29072013
                        if (iCount > 0)
                        {
                            if (prevFirstFeeFlag != RegisterData.Detail1[i].FirstFeeFlag)
                                isDiffFirstFeeFlag = true;
                        }

                        prevFirstFeeFlag = RegisterData.Detail1[i].FirstFeeFlag;
                        iCount++;
                        //End Add
                    }
                }

                for (int i = 0; i < iRowIssueBillingDetail; i++)
                {
                    strBillingtype = RegisterData.Detail2[i].strBillingtype;
                    strContractOCC = RegisterData.Detail2[i].ContractOCC;
                    dtpFrom = RegisterData.Detail2[i].dtpFrom;
                    dtpTo = RegisterData.Detail2[i].dtpTo;
                    intBillingamount = Convert.ToDecimal(RegisterData.Detail2[i].intBillingamount);
                    strIssueinvoice = RegisterData.Detail2[i].strIssueinvoice;
                    strPaymentmethod = RegisterData.Detail2[i].strPaymentmethod;
                    strBillingdetailinvoiceformat = RegisterData.Detail2[i].strBillingdetailinvoiceformat;
                    dtpExpectedissueautotransferdate = RegisterData.Detail2[i].dtpExpectedissueautotransferdate;

                    if (!(String.IsNullOrEmpty(strBillingtype)))
                    {
                        string strBillingTypeGroup = BLS050_fnGetBillingTypeGroup(strBillingtype);
                        if (string.IsNullOrEmpty(strContractOCC))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].cboContractOCCId,
                                                 "lblHeaderContractOCC",
                                                 RegisterData.Detail2[i].cboContractOCCId);
                        }
                        if (String.IsNullOrEmpty(dtpFrom.ToString()))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].dtpFromID,
                                                 "lblHeader3FromErrorMsg",
                                                 RegisterData.Detail2[i].dtpFromID);
                        }
                        if (!(String.IsNullOrEmpty(strBillingtype)))
                        {
                            if (strBillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES || strBillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_DIFF_AMOUNT)
                            {
                                if (String.IsNullOrEmpty(dtpTo.ToString()))
                                {
                                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                         "BLS050",
                                                         MessageUtil.MODULE_COMMON,
                                                         MessageUtil.MessageList.MSG0007,
                                                         RegisterData.Detail2[i].dtpToID,
                                                         "lblHeader3ToErrorMsg",
                                                         RegisterData.Detail2[i].dtpToID);
                                }
                            }
                        }
                        if (String.IsNullOrEmpty(intBillingamount.ToString()) || intBillingamount.Equals(0))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].intBillingamountID,
                                                 "lblHeader3BillingamountErrorMsg",
                                                 RegisterData.Detail2[i].intBillingamountID);
                        }
                        if (String.IsNullOrEmpty(strPaymentmethod))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].strPaymentmethodID,
                                                 "lblHeader3PaymentmethodErrorMsg",
                                                 RegisterData.Detail2[i].strPaymentmethodID);
                        }
                        if (String.IsNullOrEmpty(dtpExpectedissueautotransferdate.ToString()))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].dtpExpectedissueautotransferdateID,
                                                 "lblHeader3ExpectedissueautotransferdateErrorMsg",
                                                 RegisterData.Detail2[i].dtpExpectedissueautotransferdateID);
                        }

                        ValidatorUtil.BuildErrorMessage(res, validator, null);
                        if (res.IsError)
                        {
                            return res;
                        }

                        bolCheckIssueMoreThanZeroRow = true;
                        foreach (BusinessBillingTypeCheck _BusinessBillingTypeCheck in BusinessBillingTypeCheckList)
                        {
                            if (_BusinessBillingTypeCheck.strBillingType == strBillingtype)
                            {
                                _BusinessBillingTypeCheck.bolAlreadyIssue = true;
                            }
                            else if (_BusinessBillingTypeCheck.strBillingType != BillingType.C_BILLING_TYPE_CARD
                                        && strBillingtype != BillingType.C_BILLING_TYPE_CARD
                                        && BLS050_fnGetBillingTypeGroup(_BusinessBillingTypeCheck.strBillingType) == BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE
                                        && BLS050_fnGetBillingTypeGroup(strBillingtype) == BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE)
                            {
                                _BusinessBillingTypeCheck.bolAlreadyIssue = true;
                            }
                        }
                    }
                }

                if (!(bolCheckIssueMoreThanZeroRow))
                {
                    //check send param for screen
                    //if 0 show
                    //MSG6055
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6055,
                                                 new string[] { },
                                                 new string[] { });
                    return res;
                }

                bolCheckSameBillingType = false;

                foreach (BusinessBillingTypeCheck _BusinessBillingTypeCheck in BusinessBillingTypeCheckList)
                {
                    if (_BusinessBillingTypeCheck.bolAlreadyIssue == false)
                    {
                        bolCheckSameBillingType = true;
                    }
                }

                //MSG6024
                if (bolCheckSameBillingType)
                {
                    // MSG6024
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                     "BLS050",
                                     MessageUtil.MODULE_BILLING,
                                     MessageUtil.MessageList.MSG6024,
                                     new string[] { },
                                     new string[] { });
                    return res;
                }

                //Add by Jutarat A. on 29072013
                List<doServiceProductTypeCode> doServiceProductTypeCodeList = comContractHadler.GetServiceProductTypeCode(strContractCode);
                if (doServiceProductTypeCodeList != null && doServiceProductTypeCodeList.Count > 0)
                {
                    if (doServiceProductTypeCodeList[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        if (isDiffFirstFeeFlag == true)
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING, "BLS050", MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6092, null, null);
                    }
                }
                //End Add

                for (int i = 0; i < iRowCancelBillingDetail; i++)
                {
                    if (RegisterData.Detail1[i].bolDel && _doChecktbt_BillingDetailList != null)
                    {
                        // compare _doChecktbt_BillingDetailList and remove check del out
                        _doTempDeletetbt_BillingDetail = new doGetBillingDetailContinues();
                        foreach (doGetBillingDetailContinues _dotbt_BillingDetail in _doChecktbt_BillingDetailList)
                        {
                            if (_dotbt_BillingDetail.BillingDetailNo.ToString() == RegisterData.Detail1[i].strRunningno)
                            {
                                _doTempDeletetbt_BillingDetail = _dotbt_BillingDetail;
                            }
                        }
                        _doChecktbt_BillingDetailList.Remove(_doTempDeletetbt_BillingDetail);

                        if (BLS050_fnGetBillingTypeGroup(_doTempDeletetbt_BillingDetail.BillingTypeCode)
                            == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                        {
                            bolCheckDatePeriod = true;
                        }

                    }
                }

                // after del item
                if (_doChecktbt_BillingDetailList != null)
                {

                    foreach (doGetBillingDetailContinues _dotbt_BillingDetail in _doChecktbt_BillingDetailList)
                    {
                        if (_dotbt_BillingDetail.BillingStartDate == null)
                        {
                            _dotbt_BillingDetail.BillingStartDate = _dotbt_BillingDetail.BillingEndDate;
                        }
                        if (_dotbt_BillingDetail.BillingEndDate == null)
                        {
                            _dotbt_BillingDetail.BillingEndDate = _dotbt_BillingDetail.BillingStartDate;
                        }

                        DateRangesForDataList.Add(new BusinessStartEndDate
                        {
                            Start = (DateTime)_dotbt_BillingDetail.BillingStartDate,
                            End = (DateTime)_dotbt_BillingDetail.BillingEndDate
                        });
                    }
                }
                // add issue data
                List<BusinessStartEndDate> RegisterDataDateRangesList = new List<BusinessStartEndDate>();
                for (int i = 0; i < RegisterData.Detail2.Count; i++)
                {
                    strBillingtype = RegisterData.Detail2[i].strBillingtype;
                    dtpFrom = RegisterData.Detail2[i].dtpFrom;
                    dtpTo = RegisterData.Detail2[i].dtpTo;
                    intBillingamount = Convert.ToDecimal(RegisterData.Detail2[i].intBillingamount);
                    strIssueinvoice = RegisterData.Detail2[i].strIssueinvoice;
                    strPaymentmethod = RegisterData.Detail2[i].strPaymentmethod;
                    strBillingdetailinvoiceformat = RegisterData.Detail2[i].strBillingdetailinvoiceformat;
                    dtpExpectedissueautotransferdate = RegisterData.Detail2[i].dtpExpectedissueautotransferdate;

                    if (!(
                            (String.IsNullOrEmpty(strBillingtype)) ||
                            (String.IsNullOrEmpty(dtpFrom.ToString())) ||
                            (String.IsNullOrEmpty(dtpTo.ToString()))
                        ))
                    {
                        if (BLS050_fnGetBillingTypeGroup(strBillingtype) == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                        {
                            bolCheckDatePeriod = true;

                            DateRangesForDataList.Add(new BusinessStartEndDate
                            {
                                Start = (DateTime)RegisterData.Detail2[i].dtpFrom,
                                End = (DateTime)RegisterData.Detail2[i].dtpTo
                            });

                            RegisterDataDateRangesList.Add(new BusinessStartEndDate
                            {
                                Start = (DateTime)RegisterData.Detail2[i].dtpFrom,
                                End = (DateTime)RegisterData.Detail2[i].dtpTo
                            });
                        }
                    }
                }

                bolDateRangeProblem = BLS050_fnCheckBusinessDate(DateRangesForDataList, RegisterDataDateRangesList);
                //// case check continue bill and cancel from last date of bill back to begin

                //BillingStartDate Max -> Min
                //MSG6053
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DETAIL, FunctionID.C_FUNC_ID_SPECIAL_CREATE))
                {
                    if (bolDateRangeProblem != null &&
                        bolDateRangeProblem.ProblemCase != 0 &&
                        bolCheckDatePeriod)
                    {

                        // 1 duplicate period
                        // 2 overlap period
                        // 9 not continue
                        if (bolDateRangeProblem.ProblemCase == 1)
                        {
                            // MSG6088
                            //Billing period ({0}-{1}) must not be duplicate with exiting billing detail's billing period.
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_BILLING,
                                             MessageUtil.MessageList.MSG6088,
                                             new string[] { CommonUtil.TextDate(bolDateRangeProblem.Start), CommonUtil.TextDate(bolDateRangeProblem.End) },
                                             new string[] { });
                            return res;
                        }
                        else if (bolDateRangeProblem.ProblemCase == 2)
                        {
                            // MSG6032
                            //Billing period ({0}-{1}) must not be overlapped with exiting billing detail's billing period.
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_BILLING,
                                             MessageUtil.MessageList.MSG6032,
                                             new string[] { CommonUtil.TextDate(bolDateRangeProblem.Start), CommonUtil.TextDate(bolDateRangeProblem.End) },
                                             new string[] { });
                            return res;
                        }
                        else
                        {
                            // MSG6031
                            //Billing period ({0}-{1}) must be continuous period with existing billing detail's billing period.
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_BILLING,
                                             MessageUtil.MessageList.MSG6031,
                                             new string[] { CommonUtil.TextDate(bolDateRangeProblem.Start), CommonUtil.TextDate(bolDateRangeProblem.End) },
                                             new string[] { });
                            return res;
                        }
                    }

                }
                // check all billing type must be create logic
                /* example
                 * Data Delete Load A A C C B D E 
                 * Input Delete - C(1) C(2) D
                 * Input Issue - D D C
                
                 * delete check - loop A A C C B D E add C and D to check object
                 * issue D - loop check object C , D <-- found D mark status ok
                 * issue C - loop check object C <-- found C mark status ok
                 * loop check object - check all status is ok
                 * 
                 * if not ok then error MSG6024
                */

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            // return "1" to js is every thing OK
            res.ResultData = "1";
            return res;

        }

        /// <summary>
        /// Validate business in cancel billing detail screen mode
        /// </summary>
        /// <param name="RegisterData">input data from screen</param>
        /// <returns></returns>
        public ObjectResultData ValidateCancelBillingDetail(BLS050_RegisterData RegisterData, bool bolCheckCombine, List<BLS050_WarningMessage> confirmWarningMessageID)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            bool bolCheckDatePeriod = false;

            string strContractCode = RegisterData.Header.strContractCode;
            string strBillingOCC = RegisterData.Header.strBillingOCC;
            string rdoProcessTypeSpe = RegisterData.Header.rdoProcessTypeSpe;

            //string strchkDel_id = "";
            string strDel = "";
            string strInvoiceno = "";
            string strInvoiceOCC = "";
            //string strRunningno = "";
            string strBillingDetailsno = "";
            string strBillingtype = "";
            string strPaymentstatus = "";
            string strBillingperiod = "";
            string strBillingamount = "";

            Boolean bolCheckDelMoreThanZeroRow = false;
            try
            {
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                BusinessStartEndDate bolDateRangeProblem = new BusinessStartEndDate();
                BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();
                CommonUtil comUtil = new CommonUtil();
                DateTime? dtFirstBillingStartDate = null;
                // Do Check Business 

                List<BusinessStartEndDate> RegisterDataDateRangesList = new List<BusinessStartEndDate>();
                int iRowCancelBillingDetail = RegisterData.Detail1 != null ? RegisterData.Detail1.Count : 0;
                for (int i = 0; i < iRowCancelBillingDetail; i++)
                {

                    if (RegisterData.Detail1[i].bolDel)
                    {
                        bolCheckDelMoreThanZeroRow = true;
                    }
                }

                if (!(bolCheckDelMoreThanZeroRow))
                {
                    //check send param for screen
                    //if 0 show
                    //MSG6054
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6054,
                                                 new string[] { },
                                                 new string[] { });
                    return res;
                }

                if (!(bolCheckCombine))
                {
                    // load all billing detail with no condition
                    List<doGetBillingDetailContinues> _doChecktbt_BillingDetailList = billingHandler.GetBillingDetailContinuesList(
                        sParam._doBLS050GetBillingBasic.ContractCode,
                        sParam._doBLS050GetBillingBasic.BillingOCC,
                        string.Empty);

                    if (_doChecktbt_BillingDetailList != null && _doChecktbt_BillingDetailList.Count > 0)
                    {
                        dtFirstBillingStartDate = _doChecktbt_BillingDetailList[0].BillingStartDate;
                    }
                    doGetBillingDetailContinues _doTempDeletetbt_BillingDetail = new doGetBillingDetailContinues();
                    List<BusinessStartEndDate> DateRangesForDataList = new List<BusinessStartEndDate>();

                    for (int i = 0; i < iRowCancelBillingDetail; i++)
                    {
                        if (RegisterData.Detail1[i].bolDel && _doChecktbt_BillingDetailList != null)
                        {
                            // compare _doChecktbt_BillingDetailList and remove check del out
                            _doTempDeletetbt_BillingDetail = new doGetBillingDetailContinues();
                            foreach (doGetBillingDetailContinues _dotbt_BillingDetail in _doChecktbt_BillingDetailList)
                            {
                                if (_dotbt_BillingDetail.BillingDetailNo.ToString() == RegisterData.Detail1[i].strRunningno)
                                {
                                    _doTempDeletetbt_BillingDetail = _dotbt_BillingDetail;
                                }
                            }
                            _doChecktbt_BillingDetailList.Remove(_doTempDeletetbt_BillingDetail);

                            if (BLS050_fnGetBillingTypeGroup(_doTempDeletetbt_BillingDetail.BillingTypeCode)
                            == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                            {
                                bolCheckDatePeriod = true;
                            }

                        }
                    }

                    // after del item
                    if (_doChecktbt_BillingDetailList != null)
                    {
                        foreach (doGetBillingDetailContinues _dotbt_BillingDetail in _doChecktbt_BillingDetailList)
                        {
                            if (_dotbt_BillingDetail.BillingStartDate == null)
                            {
                                _dotbt_BillingDetail.BillingStartDate = _dotbt_BillingDetail.BillingEndDate;
                            }
                            if (_dotbt_BillingDetail.BillingEndDate == null)
                            {
                                _dotbt_BillingDetail.BillingEndDate = _dotbt_BillingDetail.BillingStartDate;
                            }

                            DateRangesForDataList.Add(new BusinessStartEndDate
                            {
                                Start = (DateTime)_dotbt_BillingDetail.BillingStartDate,
                                End = (DateTime)_dotbt_BillingDetail.BillingEndDate
                            });
                        }
                    }

                    bolDateRangeProblem = BLS050_fnCheckBusinessDate(DateRangesForDataList, RegisterDataDateRangesList);
                    //// case check continue bill and cancel from last date of bill back to begin

                    if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DETAIL, FunctionID.C_FUNC_ID_SPECIAL_CANCEL))
                    {
                        //MSG6053
                        if (((bolDateRangeProblem != null && bolDateRangeProblem.ProblemCase != 0) || (DateRangesForDataList.Count > 0 && dtFirstBillingStartDate.Value != DateRangesForDataList[0].Start))
                            && bolCheckDatePeriod)
                        {
                            // MSG6053
                            // not continue
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_BILLING,
                                             MessageUtil.MessageList.MSG6053,
                                             new string[] { CommonUtil.TextDate(bolDateRangeProblem.Start), CommonUtil.TextDate(bolDateRangeProblem.End) },
                                             new string[] { });
                            return res;
                        }
                    }
                    //BillingStartDate Max -> Min



                }
                for (int i = 0; i < iRowCancelBillingDetail; i++)
                {

                    if (RegisterData.Detail1[i].bolDel)
                    {
                        bolCheckDelMoreThanZeroRow = true;

                        strInvoiceno = RegisterData.Detail1[i].strInvoiceno;
                        strInvoiceOCC = RegisterData.Detail1[i].strInvoiceOCC;
                        strBillingDetailsno = RegisterData.Detail1[i].strRunningno;
                        strBillingtype = RegisterData.Detail1[i].strBillingtype;
                        strPaymentstatus = RegisterData.Detail1[i].strPaymentstatus;
                        strBillingperiod = RegisterData.Detail1[i].strBillingperiod;
                        strBillingamount = RegisterData.Detail1[i].strBillingamount;

                        //Payment status: 01, 03, 11, 21, 31, 07
                        if (strPaymentstatus == PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                            || strPaymentstatus == PaymentStatus.C_PAYMENT_STATUS_COUNTER_BAL
                            || strPaymentstatus == PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                            || strPaymentstatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                            || strPaymentstatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                            || strPaymentstatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK)
                        {

                            List<doGetBillingDetailOfInvoice> _doGetBillingDetailOfInvoice = billingHandler.GetBillingDetailOfInvoiceList
                            (strInvoiceno
                            , Convert.ToInt32(strInvoiceOCC));

                            if (_doGetBillingDetailOfInvoice != null
                                && _doGetBillingDetailOfInvoice.Count > 1)
                            {
                                // MSG MSG6035
                                // popup continue
                                //res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                //                 "BLS050",
                                //                 MessageUtil.MODULE_BILLING,
                                //                 MessageUtil.MessageList.MSG6035,
                                //                 new string[] { },
                                //                 new string[] { });
                                //return res;

                                //res.ResultData = "MSG6035";
                                confirmWarningMessageID.Add(new BLS050_WarningMessage() { Code = "MSG6035" });
                                return res;
                            }

                        }
                    }
                }
                if (res.IsError)
                {
                    return res;
                }



            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            // return "1" to js is every thing OK
            res.ResultData = "1";
            return res;

        }

        /// <summary>
        /// Validate business in force create billing detail screen mode
        /// </summary>
        /// <param name="RegisterData">input data from screen</param>
        /// <returns></returns>
        public ObjectResultData ValidateForceCreateBillingDetail(BLS050_RegisterData RegisterData, bool bolCheckCombine, List<BLS050_WarningMessage> confirmWarningMessageID)
        {
            string conModeRadio1rdoReCreateBillingDetail = "1";
            string conModeRadio1rdoCancelBillingDetail = "2";
            string conModeRadio1rdoForceCreateBillingDetail = "3";
            string conModeRadio1rdoRegisterAdjustOnNextPeriodAmount = "4";

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            string strContractCode = RegisterData.Header.strContractCode;
            string strBillingOCC = RegisterData.Header.strBillingOCC;
            string rdoProcessTypeSpe = RegisterData.Header.rdoProcessTypeSpe;

            string strBillingtype = "";
            string strContractOCC = "";
            DateTime? dtpFrom = null;
            DateTime? dtpTo = null;
            decimal intBillingamount = 0;
            string strIssueinvoice = "";
            string strPaymentmethod = "";
            string strBillingdetailinvoiceformat = "";
            DateTime? dtpExpectedissueautotransferdate = null;

            Boolean bolCheckDelMoreThanZeroRow = false;
            IViewContractHandler viewContractHandler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
            try
            {
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();
                ValidatorUtil validator = new ValidatorUtil();
                CommonUtil comUtil = new CommonUtil();

                bool bolCheckDatePeriod = false;
                // Do Check Input

                for (int i = 0; i < RegisterData.Detail2.Count; i++)
                {
                    strBillingtype = RegisterData.Detail2[i].strBillingtype;
                    strContractOCC = RegisterData.Detail2[i].ContractOCC;
                    dtpFrom = RegisterData.Detail2[i].dtpFrom;
                    dtpTo = RegisterData.Detail2[i].dtpTo;
                    intBillingamount = Convert.ToDecimal(RegisterData.Detail2[i].intBillingamount);
                    strIssueinvoice = RegisterData.Detail2[i].strIssueinvoice;
                    strPaymentmethod = RegisterData.Detail2[i].strPaymentmethod;
                    strBillingdetailinvoiceformat = RegisterData.Detail2[i].strBillingdetailinvoiceformat;
                    dtpExpectedissueautotransferdate = RegisterData.Detail2[i].dtpExpectedissueautotransferdate;

                    if (!((String.IsNullOrEmpty(strBillingtype))
                        //(String.IsNullOrEmpty(strBillingtype)) ||
                        //(String.IsNullOrEmpty(dtpFrom.ToString()))
                        //(String.IsNullOrEmpty(strBillingtype)) ||
                        //(String.IsNullOrEmpty(dtpFrom.ToString())) ||
                        //(String.IsNullOrEmpty(intBillingamount.ToString())) ||
                        //(String.IsNullOrEmpty(strPaymentmethod)) ||
                        //(String.IsNullOrEmpty(dtpExpectedissueautotransferdate.ToString()))
                        ))
                    {
                        string strBillingTypeGroup = BLS050_fnGetBillingTypeGroup(strBillingtype);
                        if (String.IsNullOrEmpty(strBillingtype))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].strBillingtypeID,
                                                 "lblHeader3BillingtypeErrorMsg",
                                                 RegisterData.Detail2[i].strBillingtypeID);

                        }
                        if (string.IsNullOrEmpty(strContractOCC))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].cboContractOCCId,
                                                 "lblHeaderContractOCC",
                                                 RegisterData.Detail2[i].cboContractOCCId);
                        }
                        if (String.IsNullOrEmpty(dtpFrom.ToString()))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].dtpFromID,
                                                 "lblHeader3FromErrorMsg",
                                                 RegisterData.Detail2[i].dtpFromID);


                        }
                        if (!(String.IsNullOrEmpty(strBillingtype)))
                        {
                            if (strBillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES || strBillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_DIFF_AMOUNT)
                            {
                                if (String.IsNullOrEmpty(dtpTo.ToString()))
                                {
                                    validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                         "BLS050",
                                                         MessageUtil.MODULE_COMMON,
                                                         MessageUtil.MessageList.MSG0007,
                                                         RegisterData.Detail2[i].dtpToID,
                                                         "lblHeader3ToErrorMsg",
                                                         RegisterData.Detail2[i].dtpToID);


                                }
                            }
                        }
                        if (String.IsNullOrEmpty(intBillingamount.ToString()) || intBillingamount.Equals(0))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].intBillingamountID,
                                                 "lblHeader3BillingamountErrorMsg",
                                                 RegisterData.Detail2[i].intBillingamountID);


                        }
                        if (String.IsNullOrEmpty(strPaymentmethod))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].strPaymentmethodID,
                                                 "lblHeader3PaymentmethodErrorMsg",
                                                 RegisterData.Detail2[i].strPaymentmethodID);


                        }
                        if (String.IsNullOrEmpty(dtpExpectedissueautotransferdate.ToString()))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_COMMON,
                                                 MessageUtil.MessageList.MSG0007,
                                                 RegisterData.Detail2[i].dtpExpectedissueautotransferdateID,
                                                 "lblHeader3ExpectedissueautotransferdateErrorMsg",
                                                 RegisterData.Detail2[i].dtpExpectedissueautotransferdateID);

                        }

                        ValidatorUtil.BuildErrorMessage(res, validator, null);
                        if (res.IsError)
                        {
                            return res;
                        }


                        bolCheckDelMoreThanZeroRow = true;
                    }


                }

                if (!(bolCheckDelMoreThanZeroRow))
                {
                    //check send param for screen
                    //if 0 show
                    //MSG6055
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6055,
                                                 new string[] { "lblHeader3BillingtypeErrorMsg" },
                                                 new string[] { RegisterData.Detail2[0].strBillingtypeID });

                    return res;
                }
                if (res.MessageList != null)
                {
                    if (res.MessageList.Count > 0)
                    {
                        return res;
                    }
                }
                //-------------------------------------------------------------------------
                // Do Check Business

                if (!(bolCheckCombine))
                {
                    BusinessStartEndDate bolDateRangeProblem = new BusinessStartEndDate();

                    List<BusinessStartEndDate> DateRangesForDataList = new List<BusinessStartEndDate>();
                    List<BusinessStartEndDate> RegisterDataDateRangesList = new List<BusinessStartEndDate>();
                    for (int i = 0; i < RegisterData.Detail2.Count; i++)
                    {
                        strBillingtype = RegisterData.Detail2[i].strBillingtype;
                        dtpFrom = RegisterData.Detail2[i].dtpFrom;
                        dtpTo = RegisterData.Detail2[i].dtpTo;
                        intBillingamount = Convert.ToDecimal(RegisterData.Detail2[i].intBillingamount);
                        strIssueinvoice = RegisterData.Detail2[i].strIssueinvoice;
                        strPaymentmethod = RegisterData.Detail2[i].strPaymentmethod;
                        strBillingdetailinvoiceformat = RegisterData.Detail2[i].strBillingdetailinvoiceformat;
                        dtpExpectedissueautotransferdate = RegisterData.Detail2[i].dtpExpectedissueautotransferdate;

                        if (!(
                                (String.IsNullOrEmpty(strBillingtype)) ||
                                (String.IsNullOrEmpty(dtpFrom.ToString())) ||
                                (String.IsNullOrEmpty(dtpTo.ToString())) ||
                                (String.IsNullOrEmpty(intBillingamount.ToString())) ||
                                (String.IsNullOrEmpty(strPaymentmethod)) ||
                                (String.IsNullOrEmpty(dtpExpectedissueautotransferdate.ToString()))
                            ))
                        {
                            if (BLS050_fnGetBillingTypeGroup(strBillingtype)
                                == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                            {
                                bolCheckDatePeriod = true;
                                DateRangesForDataList.Add(new BusinessStartEndDate
                                {
                                    Start = (DateTime)RegisterData.Detail2[i].dtpFrom,
                                    End = (DateTime)RegisterData.Detail2[i].dtpTo
                                });

                                RegisterDataDateRangesList.Add(new BusinessStartEndDate
                                {
                                    Start = (DateTime)RegisterData.Detail2[i].dtpFrom,
                                    End = (DateTime)RegisterData.Detail2[i].dtpTo
                                });
                            }
                        }
                    }



                    // load all billing detail with no condition
                    List<doGetBillingDetailContinues> _doChecktbt_BillingDetailList = billingHandler.GetBillingDetailContinuesList(
                        sParam._doBLS050GetBillingBasic.ContractCode,
                        sParam._doBLS050GetBillingBasic.BillingOCC,
                        string.Empty);

                    doGetBillingDetailContinues _doTempDeletetbt_BillingDetail = new doGetBillingDetailContinues();

                    // add to insert data for check period
                    if (_doChecktbt_BillingDetailList != null)
                    {

                        foreach (doGetBillingDetailContinues _dotbt_BillingDetail in _doChecktbt_BillingDetailList)
                        {
                            if (_dotbt_BillingDetail.BillingStartDate == null)
                            {
                                _dotbt_BillingDetail.BillingStartDate = _dotbt_BillingDetail.BillingEndDate;
                            }
                            if (_dotbt_BillingDetail.BillingEndDate == null)
                            {
                                _dotbt_BillingDetail.BillingEndDate = _dotbt_BillingDetail.BillingStartDate;
                            }

                            DateRangesForDataList.Add(new BusinessStartEndDate
                            {
                                Start = (DateTime)_dotbt_BillingDetail.BillingStartDate,
                                End = (DateTime)_dotbt_BillingDetail.BillingEndDate
                            });

                        }
                    }
                    bolDateRangeProblem = BLS050_fnCheckBusinessDate(DateRangesForDataList, RegisterDataDateRangesList);
                    //// case check continue bill and cancel from last date of bill back to begin

                    //BillingStartDate Max -> Min
                    if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DETAIL, FunctionID.C_FUNC_ID_SPECIAL_CREATE))
                    {
                        if (bolDateRangeProblem != null &&
                            bolDateRangeProblem.ProblemCase != 0 &&
                            bolCheckDatePeriod)
                        {
                            // 1 duplicate period
                            // 2 overlap period
                            // 9 not continue
                            if (bolDateRangeProblem.ProblemCase == 1)
                            {
                                // MSG6088
                                //Billing period ({0}-{1}) must not be duplicate with exiting billing detail's billing period.
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6088,
                                                 new string[] { CommonUtil.TextDate(bolDateRangeProblem.Start), CommonUtil.TextDate(bolDateRangeProblem.End) },
                                                 new string[] { });
                                return res;
                            }
                            else if (bolDateRangeProblem.ProblemCase == 2)
                            {
                                // MSG6032
                                //Billing period ({0}-{1}) must not be overlapped with exiting billing detail's billing period.
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6032,
                                                 new string[] { CommonUtil.TextDate(bolDateRangeProblem.Start), CommonUtil.TextDate(bolDateRangeProblem.End) },
                                                 new string[] { });
                                return res;
                            }
                            else
                            {
                                // MSG6031
                                //Billing period ({0}-{1}) must be continuous period with existing billing detail's billing period.
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6031,
                                                 new string[] { CommonUtil.TextDate(bolDateRangeProblem.Start), CommonUtil.TextDate(bolDateRangeProblem.End) },
                                                 new string[] { });
                                return res;
                            }

                        }

                    }

                }

                for (int i = 0; i < RegisterData.Detail2.Count; i++)
                {
                    strBillingtype = RegisterData.Detail2[i].strBillingtype;
                    dtpFrom = RegisterData.Detail2[i].dtpFrom;
                    dtpTo = RegisterData.Detail2[i].dtpTo;
                    intBillingamount = Convert.ToDecimal(RegisterData.Detail2[i].intBillingamount);
                    strIssueinvoice = RegisterData.Detail2[i].strIssueinvoice;
                    strPaymentmethod = RegisterData.Detail2[i].strPaymentmethod;
                    strBillingdetailinvoiceformat = RegisterData.Detail2[i].strBillingdetailinvoiceformat;
                    dtpExpectedissueautotransferdate = RegisterData.Detail2[i].dtpExpectedissueautotransferdate;

                    // every require input is keyin
                    if (!(
                            (String.IsNullOrEmpty(strBillingtype)) ||
                            (String.IsNullOrEmpty(dtpFrom.ToString())) ||
                            (String.IsNullOrEmpty(intBillingamount.ToString())) ||
                            (String.IsNullOrEmpty(strPaymentmethod)) ||
                            (String.IsNullOrEmpty(dtpExpectedissueautotransferdate.ToString()))
                        ))
                    {
                        /*Comment by Jutarat A. on 09072013 (Not validate)
                        // today or less than today then use date stamp - 1 day
                        // eg today is 6-Mar user key input 6 Mar then compare with (6-1) Mar
                        if (dtpExpectedissueautotransferdate < CommonUtil.dsTransData.dtOperationData.ProcessDateTime.AddDays(-1))
                        {
                            //MSG6025
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                 "BLS050",
                                                 MessageUtil.MODULE_BILLING,
                                                 MessageUtil.MessageList.MSG6025,
                                                 new string[] { "lblHeader3ExpectedissueautotransferdateErrorMsg" },
                                                 new string[] { RegisterData.Detail2[i].dtpExpectedissueautotransferdateID });
                            return res;
                        }*/

                        /*Comment by Jutarat A. on 08072013 (Implement later)
                        // today or less than today then use date stamp - 1 day
                        // eg today is 6-Mar user key input 6 Mar then compare with (6-1) Mar
                        if (strPaymentmethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER || strPaymentmethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                        {
                            if (dtpExpectedissueautotransferdate.Value.Date < CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Date)
                            {
                                //MSG6025
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                     "BLS050",
                                                     MessageUtil.MODULE_BILLING,
                                                     MessageUtil.MessageList.MSG6025,
                                                     new string[] { "lblHeader3ExpectedissueautotransferdateErrorMsg" },
                                                     new string[] { RegisterData.Detail2[i].dtpExpectedissueautotransferdateID });
                                return res;
                            }
                        }
                        else // Paymentmethod = bank transfer or messenger
                        {
                            if (dtpExpectedissueautotransferdate.Value.Date < CommonUtil.dsTransData.dtOperationData.ProcessDateTime.AddDays(1 - CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Day).AddMonths(-1).Date)
                            {
                                //MSG6025
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                     "BLS050",
                                                     MessageUtil.MODULE_BILLING,
                                                     MessageUtil.MessageList.MSG6025,
                                                     new string[] { "lblHeader3ExpectedissueautotransferdateErrorMsg" },
                                                     new string[] { RegisterData.Detail2[i].dtpExpectedissueautotransferdateID });
                                return res;
                            }
                        }*/

                        if (strPaymentmethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                        {

                            List<SECOM_AJIS.DataEntity.Master.tbm_AutoTransferScheduleList> _dotbm_AutoTransferScheduleList = new List<DataEntity.Master.tbm_AutoTransferScheduleList>();
                            string strTempDateNo = string.Empty;
                            strTempDateNo = dtpExpectedissueautotransferdate.Value.Day.ToString();
                            strTempDateNo = strTempDateNo.PadLeft(2, '0');

                            if (sParam.dotbt_AutoTransferBankAccount == null || sParam.dotbt_AutoTransferBankAccount.Count == 0)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                         "BLS050",
                                                         MessageUtil.MODULE_BILLING,
                                                         MessageUtil.MessageList.MSG6006,
                                                         new string[] { "lblHeader3PaymentmethodErrorMsg" },
                                                         new string[] { RegisterData.Detail2[i].strPaymentmethodID });

                                return res;
                            }
                            _dotbm_AutoTransferScheduleList = masterHandler.GetTbm_AutoTransferScheduleList(
                                sParam.dotbt_AutoTransferBankAccount[0].BankCode, strTempDateNo);

                            if (_dotbm_AutoTransferScheduleList == null)
                            {
                                // MSG6026 
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                         "BLS050",
                                                         MessageUtil.MODULE_BILLING,
                                                         MessageUtil.MessageList.MSG6026,
                                                         new string[] { "lblHeader3ExpectedissueautotransferdateErrorMsg" },
                                                         new string[] { RegisterData.Detail2[i].dtpExpectedissueautotransferdateID });
                                ;
                                return res;
                            }
                            else
                            {
                                if (_dotbm_AutoTransferScheduleList.Count == 0)
                                {
                                    // MSG6026 
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                         "BLS050",
                                                         MessageUtil.MODULE_BILLING,
                                                         MessageUtil.MessageList.MSG6026,
                                                         new string[] { "lblHeader3ExpectedissueautotransferdateErrorMsg" },
                                                         new string[] { RegisterData.Detail2[i].dtpExpectedissueautotransferdateID });
                                    return res;
                                }
                            }

                            tbt_ExportAutoTransfer _dotbt_ExportAutoTransfer = new tbt_ExportAutoTransfer();
                            _dotbt_ExportAutoTransfer = billingHandler.GetExportAutoTransfer
                                (sParam.dotbt_AutoTransferBankAccount[0].BankCode, Convert.ToDateTime(dtpExpectedissueautotransferdate));

                            if (_dotbt_ExportAutoTransfer != null)
                            {
                                // MSG6028 
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             "BLS050",
                                                             MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6028,
                                                             new string[] { "lblHeader3ExpectedissueautotransferdateErrorMsg" },
                                                               new string[] { RegisterData.Detail2[i].dtpExpectedissueautotransferdateID });
                                return res;
                            }

                        }
                        else if (strPaymentmethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                        {
                            List<SECOM_AJIS.DataEntity.Master.tbm_AutoTransferScheduleList> _dotbm_AutoTransferScheduleList = new List<DataEntity.Master.tbm_AutoTransferScheduleList>();
                            string strTempDateNo = string.Empty;
                            strTempDateNo = dtpExpectedissueautotransferdate.Value.Day.ToString();
                            strTempDateNo = strTempDateNo.PadLeft(2, '0');


                            if (sParam.doTbt_CreditCard == null || sParam.doTbt_CreditCard.Count == 0)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                         "BLS050",
                                                         MessageUtil.MODULE_BILLING,
                                                         MessageUtil.MessageList.MSG6007,
                                                         new string[] { "lblHeader3PaymentmethodErrorMsg" },
                                                         new string[] { RegisterData.Detail2[i].strPaymentmethodID });

                                return res;
                            }
                            _dotbm_AutoTransferScheduleList = masterHandler.GetTbm_AutoTransferScheduleList(
                                sParam.doTbt_CreditCard[0].CreditCardCompanyCode, strTempDateNo);

                            if (_dotbm_AutoTransferScheduleList == null)
                            {
                                // MSG6027 
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             "BLS050",
                                                             MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6027,
                                                             new string[] { "lblHeader3ExpectedissueautotransferdateErrorMsg" },
                                                               new string[] { RegisterData.Detail2[i].dtpExpectedissueautotransferdateID });
                                return res;
                            }
                            else
                            {
                                if (_dotbm_AutoTransferScheduleList.Count == 0)
                                {
                                    // MSG6027 
                                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                                 "BLS050",
                                                                 MessageUtil.MODULE_BILLING,
                                                                 MessageUtil.MessageList.MSG6027,
                                                             new string[] { "lblHeader3ExpectedissueautotransferdateErrorMsg" },
                                                               new string[] { RegisterData.Detail2[i].dtpExpectedissueautotransferdateID });
                                    return res;
                                }
                            }
                            tbt_ExportAutoTransfer _dotbt_ExportAutoTransfer = new tbt_ExportAutoTransfer();
                            _dotbt_ExportAutoTransfer = billingHandler.GetExportAutoTransfer
                                (sParam.doTbt_CreditCard[0].CreditCardCompanyCode,
                                Convert.ToDateTime(dtpExpectedissueautotransferdate));

                            if (_dotbt_ExportAutoTransfer != null)
                            {
                                // MSG6029 
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             "BLS050",
                                                             MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6029,
                                                             new string[] { "lblHeader3ExpectedissueautotransferdateErrorMsg" },
                                                               new string[] { RegisterData.Detail2[i].dtpExpectedissueautotransferdateID });
                                return res;
                            }
                        }

                        if (BLS050_fnGetBillingTypeGroup(strBillingtype) == BillingTypeGroup.C_BILLING_TYPE_GROUP_DIFF_AMOUNT
                             || strBillingtype == BillingType.C_BILLING_TYPE_MA_RESULT_BASE)
                        {

                            //5.2.1.	Billing type as difference amount of continuous fee or maintenance fee (result-based), Billing period (from) must be the same date as Start operation date or after.

                            if (dtpFrom < sParam._doBLS050GetBillingBasic.StartOperationDate)
                            {
                                //MSG6030
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                    "BLS050",
                                                    MessageUtil.MODULE_BILLING,
                                                    MessageUtil.MessageList.MSG6030,
                                                    new string[] { "lblHeader3FromErrorMsg" },
                                                    new string[] { RegisterData.Detail2[i].dtpFromID });
                                return res;
                            }


                        }
                        else if (BLS050_fnGetBillingTypeGroup(strBillingtype) != BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES) //Billing type as one-time fee
                        {

                            List<doGetContractProjectInfo> _doGetContractProjectInfo = viewContractHandler.GetContractProjectInfo(
                                sParam._doBLS050GetBillingBasic.ContractCode);
                            if (_doGetContractProjectInfo != null)
                            {
                                if (_doGetContractProjectInfo.Count > 0)
                                {
                                    if (dtpFrom < _doGetContractProjectInfo[0].ApproveContractDate ||
                                        _doGetContractProjectInfo[0].ApproveContractDate == null)
                                    {
                                        //MSG6014
                                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                            "BLS050",
                                                            MessageUtil.MODULE_BILLING,
                                                            MessageUtil.MessageList.MSG6014,
                                                            new string[] { "lblHeader3FromErrorMsg" },
                                                            new string[] { RegisterData.Detail2[i].dtpFromID });
                                        return res;
                                    }
                                }
                            }

                        }

                    }
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_DETAIL, FunctionID.C_FUNC_ID_SPECIAL_CREATE))
                {
                    res = ValidateBillingDetailMoreCondition(RegisterData);
                    if (res.MessageList != null)
                    {
                        if (res.MessageList.Count > 0)
                        {
                            return res;
                        }
                    }
                }

                //MSG6036
                if (sParam._doBLS050GetBillingBasic.AdjustType != null)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                    //                "BLS050",
                    //                MessageUtil.MODULE_BILLING,
                    //                MessageUtil.MessageList.MSG6036,
                    //                new string[] { "lblHeader3BillingtypeErrorMsg" },
                    //                new string[] { RegisterData.Detail2[0].strBillingtypeID });
                    //res.ResultData = "MSG6036";
                    confirmWarningMessageID.Add(new BLS050_WarningMessage() { Code = "MSG6036" });
                    return res;
                }

                if (rdoProcessTypeSpe == conModeRadio1rdoForceCreateBillingDetail)
                {
                    #region Validating First Fee Flag
                    var lstBillingDetail = billingHandler.GetTbt_BillingDetailData(
                        sParam._doBLS050GetBillingBasic.ContractCode,
                        sParam._doBLS050GetBillingBasic.BillingOCC,
                        null
                    );

                    lstBillingDetail = lstBillingDetail.Where(d =>
                        d.PaymentStatus != PaymentStatus.C_PAYMENT_STATUS_CANCEL
                        && d.PaymentStatus != PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED
                        && d.PaymentStatus != PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL
                        && d.PaymentStatus != PaymentStatus.C_PAYMENT_STATUS_POST_FAIL
                    ).ToList();

                    var lstBillingType = (
                        from d in RegisterData.Detail2
                        where !string.IsNullOrEmpty(d.strBillingtype)
                        group d by d.strBillingtype into grpBillingType
                        select grpBillingType.Key
                    ).ToList();

                    IBillingMasterHandler billingMaster = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                    List<tbm_BillingType> lstTmpBillingType = billingMaster.GetTbm_BillingType();
                    CommonUtil.MappingObjectLanguage(lstTmpBillingType);

                    bool hasBillingTypeWarning = false;
                    foreach (string strTmpBillingTypeCode in lstBillingType)
                    {
                        string strTmpBillingTypeName = (
                            from d in lstTmpBillingType
                            where d.BillingTypeCode == strTmpBillingTypeCode
                            select d.BillingTypeCodeName
                        ).FirstOrDefault();

                        int firstFeeCountFromDB = 0;
                        int firstFeeCountCancel = 0;
                        int firstFeeCountNew = 0;
                        int firstFeeCountTotal = 0;

                        firstFeeCountFromDB = (
                            from d in lstBillingDetail
                            where d.BillingTypeCode == strTmpBillingTypeCode
                            && (d.FirstFeeFlag ?? false) == true
                            select d
                        ).Count();

                        if (RegisterData.Detail1 != null && RegisterData.Detail1.Count > 0)
                        {
                            firstFeeCountCancel = (
                                from d in RegisterData.Detail1
                                where d.strBillingtypeCode == strTmpBillingTypeCode
                                && (d.FirstFeeFlag ?? false) == true
                                && d.bolDel == true
                                select d
                            ).Count();
                        }

                        if (RegisterData.Detail2 != null && RegisterData.Detail2.Count > 0)
                        {
                            firstFeeCountNew = (
                                from d in RegisterData.Detail2
                                where d.strBillingtype == strTmpBillingTypeCode
                                && (d.FirstFeeFlag ?? false) == true
                                select d
                            ).Count();
                        }

                        firstFeeCountTotal = firstFeeCountFromDB - firstFeeCountCancel + firstFeeCountNew;
                        if (firstFeeCountNew == 0 && firstFeeCountTotal == 0)
                        {
                            confirmWarningMessageID.Add(new BLS050_WarningMessage()
                            {
                                Code = MessageUtil.MessageList.MSG6093.ToString(),
                                Params = new List<string>()
                            {
                                strTmpBillingTypeName
                            }
                            });
                            hasBillingTypeWarning = true;
                        }
                        else if (firstFeeCountNew > 0 && firstFeeCountTotal > 1)
                        {
                            confirmWarningMessageID.Add(new BLS050_WarningMessage()
                            {
                                Code = MessageUtil.MessageList.MSG6094.ToString(),
                                Params = new List<string>()
                            {
                                strTmpBillingTypeName
                            }
                            });
                            hasBillingTypeWarning = true;
                        }
                    }

                    if (hasBillingTypeWarning)
                    {
                        return res;
                    }
                    #endregion
                }

                if (res.MessageList != null)
                {
                    if (res.MessageList.Count > 0)
                    {
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            // return "1" to js is every thing OK
            res.ResultData = "1";
            return res;

        }

        /// <summary>
        /// Validate business for special case when create billing details 
        /// </summary>
        /// <param name="RegisterData">input data from screen</param>
        /// <returns></returns>
        public ObjectResultData ValidateBillingDetailMoreCondition(BLS050_RegisterData RegisterData)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            string strContractCode = RegisterData.Header.strContractCode;
            string strBillingOCC = RegisterData.Header.strBillingOCC;
            string rdoProcessTypeSpe = RegisterData.Header.rdoProcessTypeSpe;

            //string strchkDel_id = "";
            string strDel = "";
            string strInvoiceno = "";
            string strInvoiceOCC = "";
            //string strRunningno = "";
            string strBillingDetailsno = "";
            string strBillingtype = "";
            string strBillingTypeGroup = "";
            string strPaymentstatus = "";
            string strBillingperiod = "";
            string strBillingamount = "";

            DateTime? dtpFrom = null;
            DateTime? dtpTo = null;
            decimal intBillingamount = 0;
            string strIssueinvoice = "";
            string strPaymentmethod = "";
            string strBillingdetailinvoiceformat = "";
            DateTime? dtpExpectedissueautotransferdate = null;

            string conModeRadio1rdoReCreateBillingDetail = "1";
            string conModeRadio1rdoCancelBillingDetail = "2";
            string conModeRadio1rdoForceCreateBillingDetail = "3";
            string conModeRadio1rdoRegisterAdjustOnNextPeriodAmount = "4";

            Boolean bolCheckDelMoreThanZeroRow = false;

            Boolean bCancelFirstHistory = false;
            Boolean bIssueFirstHistory = false;
            try
            {
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                IViewContractHandler viewContractHandler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;

                BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();
                ValidatorUtil validator = new ValidatorUtil();

                List<doGetBillingDetailContinues> _doGetBillingDetailContinuesList = new List<doGetBillingDetailContinues>();
                tbt_MonthlyBillingHistory _dotbt_MonthlyBillingHistory = new tbt_MonthlyBillingHistory();


                _doGetBillingDetailContinuesList = billingHandler.GetBillingDetailContinuesList(strContractCode
                    , strBillingOCC
                    , string.Empty);

                _dotbt_MonthlyBillingHistory = billingHandler.GetFirstBillingHistoryData(strContractCode
                    , strBillingOCC);

                int iRowCancelBillingDetail = RegisterData.Detail1 != null ? RegisterData.Detail1.Count : 0;

                if (rdoProcessTypeSpe == conModeRadio1rdoReCreateBillingDetail)
                {    // delete first
                    for (int i = 0; i < iRowCancelBillingDetail; i++)
                    {

                        if (RegisterData.Detail1[i].bolDel)
                        {
                            bolCheckDelMoreThanZeroRow = true;

                            strInvoiceno = RegisterData.Detail1[i].strInvoiceno;
                            strInvoiceOCC = RegisterData.Detail1[i].strInvoiceOCC;
                            strBillingDetailsno = RegisterData.Detail1[i].strRunningno;
                            strBillingtype = RegisterData.Detail1[i].strBillingtype;
                            strPaymentstatus = RegisterData.Detail1[i].strPaymentstatus;
                            strBillingperiod = RegisterData.Detail1[i].strBillingperiod;
                            strBillingamount = RegisterData.Detail1[i].strBillingamount;

                            if (_doGetBillingDetailContinuesList != null && _doGetBillingDetailContinuesList.Count > 0) //Add by Jutarat A. on 07022014
                            {
                                for (int j = 0; j < _doGetBillingDetailContinuesList.Count; j++)
                                {
                                    if (strContractCode == _doGetBillingDetailContinuesList[j].ContractCode &&
                                        strBillingOCC == _doGetBillingDetailContinuesList[j].BillingOCC &&
                                        strBillingDetailsno == _doGetBillingDetailContinuesList[j].BillingDetailNo.ToString())
                                    {
                                        //if (_doGetBillingDetailContinuesList[j].BillingStartDate == _dotbt_MonthlyBillingHistory.BillingStartDate)
                                        if (_dotbt_MonthlyBillingHistory != null && _doGetBillingDetailContinuesList[j].BillingStartDate == _dotbt_MonthlyBillingHistory.BillingStartDate) //Modify by Jutarat A. on 06032013
                                        {
                                            bCancelFirstHistory = true;
                                        }
                                        //_doGetBillingDetailContinuesList.RemoveAt(j);

                                    }
                                    // _doGetBillingDetailContinuesList[0].BillingStartDate is first billing detail of this billing basic

                                    //if (_doGetBillingDetailContinuesList[0].BillingStartDate != _dotbt_MonthlyBillingHistory.BillingStartDate)
                                    if (_dotbt_MonthlyBillingHistory != null && _doGetBillingDetailContinuesList[0].BillingStartDate != _dotbt_MonthlyBillingHistory.BillingStartDate) //Modify by Jutarat A. on 06032013
                                    {
                                        bCancelFirstHistory = true;
                                    }

                                }
                            }

                        }
                    }
                }

                // issue section
                for (int i = 0; i < RegisterData.Detail2.Count; i++)
                {

                    if (RegisterData.Detail2[i].strBillingtype != "" && RegisterData.Detail2[i].strBillingtype != null)
                    {
                        strBillingtype = RegisterData.Detail2[i].strBillingtype;
                        strBillingTypeGroup = this.BLS050_fnGetBillingTypeGroup(strBillingtype);
                        dtpFrom = RegisterData.Detail2[i].dtpFrom;
                        dtpTo = RegisterData.Detail2[i].dtpTo;
                        intBillingamount = Convert.ToDecimal(RegisterData.Detail2[i].intBillingamount);
                        strIssueinvoice = RegisterData.Detail2[i].strIssueinvoice;
                        strPaymentmethod = RegisterData.Detail2[i].strPaymentmethod;
                        strBillingdetailinvoiceformat = RegisterData.Detail2[i].strBillingdetailinvoiceformat;
                        dtpExpectedissueautotransferdate = RegisterData.Detail2[i].dtpExpectedissueautotransferdate;

                        if (strBillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                        {
                            if (sParam._doBLS050GetBillingBasic.StartOperationDate == null)
                            {
                                //MSG6062
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS050",
                                        MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6062,
                                        null,
                                        null);
                                return res;
                            }

                            //if (dtpFrom == _doGetBillingDetailContinuesList[j].BillingStartDate &&
                            //dtpTo == _doGetBillingDetailContinuesList[j].BillingEndDate)
                            //{
                            //    //MSG6063
                            //    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                            //                        "BLS050",
                            //                        MessageUtil.MODULE_BILLING,
                            //                        MessageUtil.MessageList.MSG6063,
                            //                        new string[] { "lblHeader3BillingtypeErrorMsg" },
                            //                        new string[] { RegisterData.Detail2[i].dtpFromID });
                            //    return res;
                            //}

                            if (dtpFrom < sParam._doBLS050GetBillingBasic.StartOperationDate)
                            {
                                //MSG6030
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                    "BLS050",
                                                    MessageUtil.MODULE_BILLING,
                                                    MessageUtil.MessageList.MSG6030,
                                                    new string[] { "lblHeader3FromErrorMsg" },
                                                    new string[] { RegisterData.Detail2[i].dtpFromID });
                                return res;
                            }

                            //if (dtpFrom < _dotbt_MonthlyBillingHistory.BillingStartDate)
                            if (_dotbt_MonthlyBillingHistory != null && dtpFrom < _dotbt_MonthlyBillingHistory.BillingStartDate) //Modify by Jutarat A. on 06032013
                            {
                                //MSG6034
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                    "BLS050",
                                                    MessageUtil.MODULE_BILLING,
                                                    MessageUtil.MessageList.MSG6034,
                                                    new string[] { "lblHeader3FromErrorMsg" },
                                                    new string[] { RegisterData.Detail2[i].dtpFromID });
                                return res;
                            }

                            //if (dtpFrom == _dotbt_MonthlyBillingHistory.BillingStartDate)
                            if (_dotbt_MonthlyBillingHistory != null && dtpFrom == _dotbt_MonthlyBillingHistory.BillingStartDate) //Modify by Jutarat A. on 06032013
                            {
                                bIssueFirstHistory = true;
                            }
                        }

                        if (res.IsError)
                        {
                            return res;
                        }

                    }

                }
                //In case of re-create billing detail, about billing detail of continuous fee, show error when cancel and re-create on CF14 that causes no billing detail which is the same date as 'Start date of billing period' of the first billing history of such billing OCC. 
                if (bCancelFirstHistory == true && bIssueFirstHistory == false)
                {
                    //MSG6033 Cannot register because there will be no billing detail from 'Start date of billing period' of the first billing history
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS050",
                                        MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6033,
                                        null,
                                        null);
                    return res;
                }


                if (res.IsError)
                {
                    return res;
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            // return "1" to js is every thing OK
            res.ResultData = "1";
            return res;
        }

        /// <summary>
        /// Validate business in adjust on next period screen mode
        /// </summary>
        /// <param name="RegisterData">input data from screen</param>
        /// <returns></returns>
        public ObjectResultData ValidateAdjustOnNextPeriod(BLS050_RegisterData RegisterData)
        {
            string conModeRadio2rdoRegister = "1";
            string conModeRadio2rdoDelete = "2";

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();

            //No need validation for adjust type 'delete'
            if (RegisterData.Detail3.rdoProcessTypeAdj == conModeRadio2rdoDelete)
            {
                res.ResultData = "1";
                return res;
            }

            //RegisterData.Header.rdoProcessTypeSpe
            string conModeRadio1rdoRegisterAdjustOnNextPeriodAmount = "4";
            if (RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoRegisterAdjustOnNextPeriodAmount
                || RegisterData.Detail3.rdoProcessTypeAdj == conModeRadio2rdoRegister)
            {
                string strContractCode = RegisterData.Header.strContractCode;
                string strBillingOCC = RegisterData.Header.strBillingOCC;
                string rdoProcessTypeSpe = RegisterData.Header.rdoProcessTypeSpe;

                string rdoProcessTypeAdj = RegisterData.Detail3.rdoProcessTypeAdj;
                string cboAdjustmentType = RegisterData.Detail3.cboAdjustmentType;
                string intBillingAmountAdj = RegisterData.Detail3.intBillingAmountAdj;
                string intBillingAmountAdjCurrency = RegisterData.Detail3.intBillingAmountAdjCurrency;
                DateTime? dptAdjustBillingPeriodDateFrom = RegisterData.Detail3.dptAdjustBillingPeriodDateFrom;
                DateTime? dptAdjustBillingPeriodDateTo = RegisterData.Detail3.dptAdjustBillingPeriodDateTo;

                try
                {
                    IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                    ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();
                    // Do Check Business 

                    if (String.IsNullOrEmpty(RegisterData.Detail3.rdoProcessTypeAdj))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                             "ProcessType",
                                             "lblProcessTypeAdj",
                                             "ProcessType");
                    }

                    if (String.IsNullOrEmpty(cboAdjustmentType))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                             "cboAdjustmentType",
                                             "lblAdjustmentType",
                                             "cboAdjustmentType");
                    }

                    if (String.IsNullOrEmpty(intBillingAmountAdj) || Convert.ToDecimal(intBillingAmountAdj) <= 0)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                             "intBillingAmountAdj",
                                             "lblBillingAmount",
                                             "intBillingAmountAdj");
                    }
                    if (String.IsNullOrEmpty(dptAdjustBillingPeriodDateFrom.ToString()))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                             "dptAdjustBillingPeriodDateFrom",
                                             "lblAdjustBillingPeriod",
                                             "dptAdjustBillingPeriodDateFrom");
                    }

                    if (String.IsNullOrEmpty(dptAdjustBillingPeriodDateTo.ToString()))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                             "dptAdjustBillingPeriodDateTo",
                                             "lblAdjustBillingPeriod",
                                             "dptAdjustBillingPeriodDateTo");
                    }

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    if (res.IsError)
                    {
                        return res;
                    }

                    if (RegisterData.Detail3.dptAdjustBillingPeriodDateFrom < sParam._doBLS050GetBillingBasic.StartOperationDate)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_BILLING,
                                             MessageUtil.MessageList.MSG6037,
                                             new string[] { "lblAdjustBillingPeriod" },
                                             new string[] { "dptAdjustBillingPeriodDateFrom" });

                        return res;
                    }

                    if (RegisterData.Detail3.dptAdjustBillingPeriodDateTo > sParam._doBLS050GetBillingBasic.LastBillingDate)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS050",
                                             MessageUtil.MODULE_BILLING,
                                             MessageUtil.MessageList.MSG6038,
                                             new string[] { "lblAdjustBillingPeriod" },
                                             new string[] { "dptAdjustBillingPeriodDateTo" });

                        return res;
                    }

                }
                catch (Exception ex)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(ex);
                }

            }



            // return "1" to js is every thing OK
            res.ResultData = "1";
            return res;

        }
        /// <summary>
        /// check date overlap for check date continue period
        /// </summary>
        /// <param name="dtBillingStartDate">Billing Start Date</param>
        /// <param name="dtBillingEndDate">Billing End Date</param>
        /// <param name="dtIssueTo">Issue To Date</param>
        /// <param name="dtIssueFrom">Issue From Date</param>
        /// <returns></returns>
        public Boolean CheckDateOverlap(DateTime dtBillingStartDate,
                                        DateTime dtBillingEndDate,
                                        DateTime dtIssueTo,
                                        DateTime dtIssueFrom)
        {
            // false = not over lap
            Boolean bolOpt = false;
            if (dtBillingStartDate <= dtIssueTo)
            {
                bolOpt = true;
            }
            if (dtBillingEndDate >= dtIssueFrom)
            {
                bolOpt = true;
            }

            return bolOpt;
        }

        /// <summary>
        /// validate input data confirm and register data into database
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS050_Confirm()
        {

            string conModeRadio1rdoReCreateBillingDetail = "1";
            string conModeRadio1rdoCancelBillingDetail = "2";
            string conModeRadio1rdoForceCreateBillingDetail = "3";
            string conModeRadio1rdoRegisterAdjustOnNextPeriodAmount = "4";

            string conModeRadio2rdoRegister = "1";
            string conModeRadio2rdoDelete = "2";

            string slipNo = string.Empty;

            BLS050_ScreenParameter param = GetScreenObject<BLS050_ScreenParameter>();
            BLS050_RegisterData RegisterData = new BLS050_RegisterData();
            if (param != null)
            {
                RegisterData = param.RegisterData;
            }

            ObjectResultData res = new ObjectResultData();
            ObjectResultData resDel = new ObjectResultData();
            ObjectResultData resIns = new ObjectResultData();
            ObjectResultData resAdj = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                IBillingHandler handlerInventory = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                // Is suspend ?
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Do Business 


                //Modify by Jutarat A. on 06012013
                /*if (RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoReCreateBillingDetail
                    || RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoCancelBillingDetail)
                {

                    resDel = CancelBillingDetail(RegisterData);
                    if (resDel.MessageList != null)
                    {
                        if (resDel.MessageList.Count > 0)
                        {
                            return Json(resDel);
                        }
                    }
                }
                if (RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoReCreateBillingDetail
                    || RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoForceCreateBillingDetail)
                {
                    resIns = ForceCreateBillingDetail(RegisterData);
                    if (resIns.MessageList != null)
                    {
                        if (resIns.MessageList.Count > 0)
                        {
                            return Json(resIns);
                        }
                    }
                }

                if (RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoReCreateBillingDetail
                    || RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoRegisterAdjustOnNextPeriodAmount)
                {
                    resAdj = AdjustOnNextPeriod(RegisterData);
                    if (resAdj.MessageList != null)
                    {
                        if (resAdj.MessageList.Count > 0)
                        {
                            return Json(resAdj);
                        }
                    }
                }*/

                 using (TransactionScope scope = new TransactionScope())
                  {
                try
                {
                        if (RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoReCreateBillingDetail
                            || RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoCancelBillingDetail)
                        {

                            resDel = CancelBillingDetail(RegisterData);
                            if (resDel.MessageList != null)
                            {
                                if (resDel.MessageList.Count > 0)
                                {
                                    scope.Dispose();
                                return Json(resDel);
                                }
                            }
                        }
                        if (RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoReCreateBillingDetail
                            || RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoForceCreateBillingDetail)
                        {
                            resIns = ForceCreateBillingDetail(RegisterData);
                            if (resIns.MessageList != null)
                            {
                                if (resIns.MessageList.Count > 0)
                                {
                                  scope.Dispose(); 
                                return Json(resIns);
                                }
                            }
                        }

                        if (RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoReCreateBillingDetail
                            || RegisterData.Header.rdoProcessTypeSpe == conModeRadio1rdoRegisterAdjustOnNextPeriodAmount)
                        {
                            resAdj = AdjustOnNextPeriod(RegisterData);
                            if (resAdj.MessageList != null)
                            {
                                if (resAdj.MessageList.Count > 0)
                                {
                                    scope.Dispose(); 
                                return Json(resAdj);
                                }
                            }
                        }

                      scope.Complete(); 
                }
                catch (Exception ex)
                    {
                     scope.Dispose();  

                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(ex);
                        return Json(res);
                    }
                } 
                //End Modify

                // mark ok flag to javascript outside

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

            //res.ResultData = slipNo; // Success ! return slip no.
            return Json(res);

        }

        /// <summary>
        /// cancel billing detail data in database 
        /// </summary>
        /// <param name="RegisterData">delete billing detail criteria</param>
        /// <returns></returns>
        public ObjectResultData CancelBillingDetail(BLS050_RegisterData RegisterData)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            string strContractCode = RegisterData.Header.strContractCode;
            string strBillingOCC = RegisterData.Header.strBillingOCC;
            string rdoProcessTypeSpe = RegisterData.Header.rdoProcessTypeSpe;

            //string strchkDel_id = "";
            string strDel = "";
            string strInvoiceno = "";
            string strInvoiceOCC = "";
            //string strRunningno = "";
            string strBillingDetailsno = "";
            string strBillingtype = "";
            string strPaymentstatus = "";
            string strBillingperiod = "";
            string strBillingamount = "";

            List<string> arrCancelInvoiceNo = new List<string> { };
            List<string> arrCancelInvoiceOCC = new List<string> { };

            tbt_Invoice doUpdatetbt_Invoice = new tbt_Invoice();

            List<tbt_BillingDetail> doUpdatetbt_BillingDetailListTemp = new List<tbt_BillingDetail>();
            List<tbt_BillingDetail> doUpdatetbt_BillingDetailList = new List<tbt_BillingDetail>();

            Boolean bolCancelInvoiceNoDubChk = false;

              using (TransactionScope scope = new TransactionScope())
             {

            try
            {
                    IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                    ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    BLS050_ScreenParameter param = GetScreenObject<BLS050_ScreenParameter>();
                    tbt_Invoice _dotbt_Invoice = new tbt_Invoice();

                    // Do Business 
                    // cancel billing details by billing details no
                    for (int i = RegisterData.Detail1.Count - 1; i >= 0; i--)
                    {

                        //strchkDel_id = RegisterData.Detail1[i].strchkDel_id;
                        //strDel = ;
                        if (RegisterData.Detail1[i].bolDel)
                        {
                            strInvoiceno = RegisterData.Detail1[i].strInvoiceno;
                            strInvoiceOCC = RegisterData.Detail1[i].strInvoiceOCC;
                            strBillingDetailsno = RegisterData.Detail1[i].strRunningno;
                            strBillingtype = RegisterData.Detail1[i].strBillingtype;
                            strPaymentstatus = RegisterData.Detail1[i].strPaymentstatus;
                            strBillingperiod = RegisterData.Detail1[i].strBillingperiod;
                            strBillingamount = RegisterData.Detail1[i].strBillingamount;
                            


                            // Select Billing Details in each invoid and then Cancel
                            // must select only 1 row to cancel by check list
                            List<tbt_BillingDetail> dotbt_BillingDetailList = billingHandler.GetTbt_BillingDetailData(
                                                                                            strContractCode,
                                                                                            strBillingOCC,
                                                                                            Convert.ToInt32(strBillingDetailsno));

                            dotbt_BillingDetailList[0].PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CANCEL;

                            //Add by Jutarat A. on 25112013
                            var doBillingDetailTemp = (from t in param._doBLS050GetBillingDetailForCancelList
                                                       where t.ContractCode == strContractCode
                                                           && t.BillingOCC == strBillingOCC
                                                           && t.BillingDetailNo == Convert.ToInt32(strBillingDetailsno)
                                                       select t).ToList<doBLS050GetBillingDetailForCancelList>();

                            DateTime? dtUpdateDate = null;
                            if (doBillingDetailTemp != null && doBillingDetailTemp.Count > 0)
                                dtUpdateDate = doBillingDetailTemp[0].UpdateDate;
                            //End Add

                            billingHandler.Updatetbt_BillingDetail(dotbt_BillingDetailList[0], dtUpdateDate); //Modify (Add dtUpdateDate) by Jutarat A. on 25112013

                            // Update last billing date
                            if (BLS050_fnGetBillingTypeGroup(strBillingtype) == BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES)
                            {
                                List<tbt_BillingBasic> doBillingBasicList = billingHandler.GetTbt_BillingBasic(dotbt_BillingDetailList[0].ContractCode, dotbt_BillingDetailList[0].BillingOCC);

                                List<doGetBillingDetailContinues> billingDetailTemp = billingHandler.GetBillingDetailContinuesList(strContractCode, strBillingOCC, string.Empty); //Add by Jutarat A. on 02092013
                                if (billingDetailTemp == null || billingDetailTemp.Count == 0) //Add by Jutarat A. on 02092013
                                {
                                    if (doBillingBasicList[0].LastBillingDate == dotbt_BillingDetailList[0].BillingEndDate)
                                    {
                                        doBillingBasicList[0].LastBillingDate = dotbt_BillingDetailList[0].BillingStartDate.Value.AddDays(-1);
                                        billingHandler.UpdateTbt_BillingBasic(doBillingBasicList[0]);
                                    }
                                }
                                //Add by Jutarat A. on 02092013
                                else
                                {
                                    billingDetailTemp = billingDetailTemp.OrderByDescending(t => t.BillingEndDate).ToList<doGetBillingDetailContinues>();

                                    doBillingBasicList[0].LastBillingDate = billingDetailTemp[0].BillingEndDate;
                                    billingHandler.UpdateTbt_BillingBasic(doBillingBasicList[0]);
                                }
                                //End Add
                            }
                            bolCancelInvoiceNoDubChk = false;
                            foreach (string strCancelInvoiceNo in arrCancelInvoiceNo)
                            {
                                if (strCancelInvoiceNo == strInvoiceno)
                                {
                                    bolCancelInvoiceNoDubChk = true;
                                }
                            }
                            if (!(bolCancelInvoiceNoDubChk))
                            {
                                arrCancelInvoiceNo.Add(strInvoiceno);
                                arrCancelInvoiceOCC.Add(strInvoiceOCC);
                            }

                        }
                    }

                    // cancel all invoice of cancel billing details by billing details no

                    string strBusinessCancelInvoiceNo = string.Empty;
                    string strBusinessCancelInvoiceOCC = string.Empty;

                    for (int i = 0; i < arrCancelInvoiceNo.Count; i++)
                    {

                        strBusinessCancelInvoiceNo = arrCancelInvoiceNo[i];
                        strBusinessCancelInvoiceOCC = arrCancelInvoiceOCC[i];

                        List<doGetUnpaidInvoiceData> _doGetUnpaidInvoiceDataList = billingHandler.GetUnpaidInvoiceDataList(strBusinessCancelInvoiceNo);

                        if (_doGetUnpaidInvoiceDataList != null)
                        {
                            // incase have billing details that still have
                            // payment not equal cancel
                            foreach (doGetUnpaidInvoiceData _doGetUnpaidInvoiceData in _doGetUnpaidInvoiceDataList)
                            {
                                doUpdatetbt_BillingDetailList = new List<tbt_BillingDetail>(); //Add by Jutarat A. on 11122013

                                // get real invoice data for update
                                doUpdatetbt_Invoice = billingHandler.GetTbt_InvoiceData(
                                    _doGetUnpaidInvoiceData.InvoiceNo
                                    , _doGetUnpaidInvoiceData.InvoiceOCC);
                                // get details data
                                List<doGetBillingDetailOfInvoice> _doGetBillingDetailOfInvoiceList
                                    = billingHandler.GetBillingDetailOfInvoiceList(
                                                     _doGetUnpaidInvoiceData.InvoiceNo
                                                    , _doGetUnpaidInvoiceData.InvoiceOCC);

                                if (_doGetBillingDetailOfInvoiceList != null)
                                {
                                    if (_doGetBillingDetailOfInvoiceList.Count > 0)
                                    {
                                        foreach (doGetBillingDetailOfInvoice _doGetBillingDetailOfInvoice in _doGetBillingDetailOfInvoiceList)
                                        {
                                            // get real details data for update
                                            doUpdatetbt_BillingDetailListTemp = billingHandler.GetTbt_BillingDetailData(
                                                        _doGetBillingDetailOfInvoice.ContractCode
                                                        , _doGetBillingDetailOfInvoice.BillingOCC
                                                        , _doGetBillingDetailOfInvoice.BillingDetailNo);

                                            doUpdatetbt_BillingDetailList.Add(doUpdatetbt_BillingDetailListTemp[0]);

                                        };
                                    }
                                }

                                // update invoide obj with billing details
                                if (billingHandler.UpdateInvoicePaymentStatus(
                                   doUpdatetbt_Invoice
                                   , doUpdatetbt_BillingDetailList
                                   , PaymentStatus.C_PAYMENT_STATUS_CANCEL))
                                {
                                    // error here
                                };
                                // re create new invoice
                                // new invoice number same billing details
                                if (_doGetBillingDetailOfInvoiceList != null)
                                {
                                    _dotbt_Invoice = new tbt_Invoice();
                                    _dotbt_Invoice.InvoiceNo = doUpdatetbt_Invoice.InvoiceNo;
                                    _dotbt_Invoice.InvoiceOCC = doUpdatetbt_Invoice.InvoiceOCC;
                                    _dotbt_Invoice.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    _dotbt_Invoice.AutoTransferDate = doUpdatetbt_Invoice.AutoTransferDate;
                                    _dotbt_Invoice.BillingTargetCode = doUpdatetbt_Invoice.BillingTargetCode;
                                    _dotbt_Invoice.BillingTypeCode = doUpdatetbt_Invoice.BillingTypeCode;
                                    //_dotbt_Invoice.InvoiceAmount
                                    //_dotbt_Invoice.PaidAmountIncVat
                                    //_dotbt_Invoice.VatRate
                                    //_dotbt_Invoice.VatAmount
                                    //_dotbt_Invoice.WHTRate
                                    //_dotbt_Invoice.WHTAmount
                                    //_dotbt_Invoice.RegisteredWHTAmount
                                    _dotbt_Invoice.InvoicePaymentStatus = doUpdatetbt_Invoice.InvoicePaymentStatus;
                                    if (doUpdatetbt_Invoice.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                        || doUpdatetbt_Invoice.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                    {
                                        _dotbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;
                                    }
                                    else
                                    {
                                        _dotbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                                    }

                                    _dotbt_Invoice.IssueInvFlag = doUpdatetbt_Invoice.IssueInvFlag;
                                    //_dotbt_Invoice.FirstIssueInvDate = null;
                                    //_dotbt_Invoice.FirstIssueInvFlag = null;
                                    _dotbt_Invoice.PaymentMethod = doUpdatetbt_Invoice.PaymentMethod;
                                    //_dotbt_Invoice.CorrectReason
                                    _dotbt_Invoice.RefOldInvoiceNo = doUpdatetbt_Invoice.InvoiceNo;

                                    //Dummy concept to support ef,
                                    List<tbt_BillingDetail> newBillingDetails = CommonUtil.ClonsObjectList<tbt_BillingDetail, tbt_BillingDetail>(doUpdatetbt_BillingDetailList);

                                    foreach (tbt_BillingDetail _dotbt_BillingDetail in newBillingDetails)
                                    {
                                        // Update billing detail
                                        _dotbt_BillingDetail.InvoiceNo = null;
                                        _dotbt_BillingDetail.InvoiceOCC = null;
                                        _dotbt_BillingDetail.BillingDetailNo = 0;
                                        _dotbt_BillingDetail.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                    };
                                    tbt_Invoice tempdotbt_Invoice = billingHandler.ManageInvoiceByCommand(
                                        _dotbt_Invoice
                                        , newBillingDetails
                                        , true);
                                }
                            }
                        }

                        //param.RegisterData = RegisterData;
                    }

                    res.ResultData = "1";

                // == COMMIT TRANSACTION ==
                  scope.Complete(); 

            }
            catch (Exception ex)
                {

                // ==   ROLLBACK TRACNSCTION
                scope.Dispose(); 
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(ex);
                }
                }

            return res;
        }

        /// <summary>
        /// force create billing detail data in database 
        /// </summary>
        /// <param name="RegisterData">create billing detail input</param>
        /// <returns></returns>
        public ObjectResultData ForceCreateBillingDetail(BLS050_RegisterData RegisterData)
        {
            string conModeRadio1rdoReCreateBillingDetail = "1";
            string conModeRadio1rdoCancelBillingDetail = "2";
            string conModeRadio1rdoForceCreateBillingDetail = "3";
            string conModeRadio1rdoRegisterAdjustOnNextPeriodAmount = "4";

            string strContractCode = RegisterData.Header.strContractCode;
            string strBillingOCC = RegisterData.Header.strBillingOCC;
            string rdoProcessTypeSpe = RegisterData.Header.rdoProcessTypeSpe;

            string strBillingtype = "";
            DateTime? dtpFrom = null;
            DateTime? dtpTo = null;
            decimal intBillingamount = 0;
            string strBillingAmountCurrency = "";
            string strIssueinvoice = "";
            string strPaymentmethod = "";
            string strBillingdetailinvoiceformat = "";
            DateTime? dtpExpectedissueautotransferdate = null;
            bool? isFirstFee = null;
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            tbt_Invoice _dotbt_Invoice = new tbt_Invoice();
            tbt_BillingDetail _doUpdatetbt_BillingDetail = new tbt_BillingDetail();
            List<tbt_BillingDetail> doUpdatetbt_BillingDetailList = new List<tbt_BillingDetail>();
            List<string> lstFilePath = new List<string>();

              using (TransactionScope scope = new TransactionScope())
             {

            try
            {

                    IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                    ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    BLS050_ScreenParameter param = GetScreenObject<BLS050_ScreenParameter>();
                    CommonUtil comUtil = new CommonUtil();

                    /////// Add by Patcharee T. set first fee flag to in case Re-create billing detail  07-Jun-2013 /////// 
                    List<BusinessBillingTypeCheck> BusinessBillingTypeCheckList = new List<BusinessBillingTypeCheck>();
                    int iRowCancelBillingDetail = RegisterData.Detail1 != null ? RegisterData.Detail1.Count : 0;
                    bool bolAddFlag = true;
                    for (int i = 0; i < iRowCancelBillingDetail; i++)
                    {
                        if (RegisterData.Detail1[i].bolDel)
                        {
                            bolAddFlag = true;
                            foreach (BusinessBillingTypeCheck _BusinessBillingTypeCheck in BusinessBillingTypeCheckList)
                            {
                                if (_BusinessBillingTypeCheck.strBillingType == RegisterData.Detail1[i].strBillingtypeCode)
                                {
                                    bolAddFlag = false;
                                }
                            }
                            if (bolAddFlag)
                            {
                                //Comment by Jutarat A. on 29072013
                                //List<tbt_BillingDetail> bd1 = billingHandler.GetTbt_BillingDetailData(
                                //                                RegisterData.Header.strContractCode
                                //                                , RegisterData.Header.strBillingOCC
                                //                                , Convert.ToInt32(RegisterData.Detail1[i].strRunningno));
                                //End Comment
                                BusinessBillingTypeCheckList.Add(new BusinessBillingTypeCheck
                                {
                                    strBillingType = RegisterData.Detail1[i].strBillingtypeCode,
                                    //isFirstFee = bd1 != null && bd1.Count > 0 ? bd1[0].FirstFeeFlag : null
                                    isFirstFee = RegisterData.Detail1[i].FirstFeeFlag //Modify by Jutarat A. on 29072013
                                });

                            }
                        }
                    }
                    /////// Add by Patcharee T. set first fee flag to in case Re-create billing detail  07-Jun-2013 /////// 

                    // Do Business 
                    // gen Billing Details Loop
                    for (int i = 0; i < RegisterData.Detail2.Count; i++)
                    {

                        if (RegisterData.Detail2[i].strBillingtype != null)
                        {
                            strBillingtype = RegisterData.Detail2[i].strBillingtype;
                            dtpFrom = RegisterData.Detail2[i].dtpFrom;
                            dtpTo = RegisterData.Detail2[i].dtpTo;
                            intBillingamount = Convert.ToDecimal(RegisterData.Detail2[i].intBillingamount);
                            strBillingAmountCurrency = RegisterData.Detail2[i].initBillingamountCurrency;
                            strIssueinvoice = RegisterData.Detail2[i].strIssueinvoice;
                            strPaymentmethod = RegisterData.Detail2[i].strPaymentmethod;
                            strBillingdetailinvoiceformat = RegisterData.Detail2[i].strBillingdetailinvoiceformat;
                            dtpExpectedissueautotransferdate = RegisterData.Detail2[i].dtpExpectedissueautotransferdate;

                            if (rdoProcessTypeSpe == conModeRadio1rdoReCreateBillingDetail)
                            {

                                /////// Add by Patcharee T. set first fee flag to in case Re-create billing detail  07-Jun-2013 /////// 
                                foreach (BusinessBillingTypeCheck _BusinessBillingTypeCheck in BusinessBillingTypeCheckList)
                                {
                                    if (_BusinessBillingTypeCheck.strBillingType == RegisterData.Detail2[i].strBillingtype)
                                    {
                                        isFirstFee = _BusinessBillingTypeCheck.isFirstFee;
                                    }
                                    else if (_BusinessBillingTypeCheck.strBillingType != BillingType.C_BILLING_TYPE_CARD
                                                && RegisterData.Detail2[i].strBillingtype != BillingType.C_BILLING_TYPE_CARD
                                                && BLS050_fnGetBillingTypeGroup(_BusinessBillingTypeCheck.strBillingType) == BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE
                                                && BLS050_fnGetBillingTypeGroup(RegisterData.Detail2[i].strBillingtype) == BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE)
                                    {
                                        isFirstFee = _BusinessBillingTypeCheck.isFirstFee;
                                    }
                                }
                                /////// Add by Patcharee T. set first fee flag to in case Re-create billing detail  07-Jun-2013 /////// 
                            }
                            else
                            {
                                isFirstFee = RegisterData.Detail2[i].FirstFeeFlag;
                            }

                            if (String.IsNullOrEmpty(dtpExpectedissueautotransferdate.ToString()))
                            {
                                if (strPaymentmethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER || strPaymentmethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD)
                                {
                                    DateTime? dtIssueInvDate = billingHandler.GetNextAutoTransferDate(RegisterData.Header.strContractCode, RegisterData.Header.strBillingOCC, strPaymentmethod);
                                    if (dtIssueInvDate == null)
                                    {
                                        strPaymentmethod = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                                        dtpExpectedissueautotransferdate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    }
                                    else
                                    {
                                        dtpExpectedissueautotransferdate = dtIssueInvDate;
                                    }
                                }
                                else
                                {
                                    dtpExpectedissueautotransferdate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                }
                            }

                            _dotbt_Invoice = new tbt_Invoice();
                            _doUpdatetbt_BillingDetail = new tbt_BillingDetail();
                            doUpdatetbt_BillingDetailList = new List<tbt_BillingDetail>();

                            _doUpdatetbt_BillingDetail.ContractCode = strContractCode;
                            _doUpdatetbt_BillingDetail.BillingOCC = strBillingOCC;
                            //_doUpdatetbt_BillingDetail.BillingDetailNo = 0;
                            //_doUpdatetbt_BillingDetail.InvoiceNo = null;
                            //_doUpdatetbt_BillingDetail.InvoiceOCC = null;


                            if (strIssueinvoice == IssueInv.C_ISSUE_INV_NOT_ISSUE || strBillingdetailinvoiceformat == BillingInvFormatType.C_BILLING_INV_FORMAT_SPECIFIC)
                            {
                                _doUpdatetbt_BillingDetail.IssueInvFlag = false;
                            }
                            else
                            {
                                _doUpdatetbt_BillingDetail.IssueInvFlag = true;
                            }

                            _doUpdatetbt_BillingDetail.BillingTypeCode = strBillingtype;
                            _doUpdatetbt_BillingDetail.BillingTypeGroup = BLS050_fnGetBillingTypeGroup(strBillingtype);

                            _doUpdatetbt_BillingDetail.BillingAmountCurrencyType = strBillingAmountCurrency;
                            if (strBillingAmountCurrency == "1")
                            {
                                _doUpdatetbt_BillingDetail.BillingAmount = intBillingamount;
                                _doUpdatetbt_BillingDetail.BillingAmountUsd = null;
                            }
                            else
                            {
                                _doUpdatetbt_BillingDetail.BillingAmount = null;
                                _doUpdatetbt_BillingDetail.BillingAmountUsd = intBillingamount;
                            }

                            //_doUpdatetbt_BillingDetail.AdjustBillingAmount = null;
                            _doUpdatetbt_BillingDetail.BillingStartDate = dtpFrom;
                            _doUpdatetbt_BillingDetail.BillingEndDate = dtpTo;
                            _doUpdatetbt_BillingDetail.PaymentMethod = strPaymentmethod;
                            if (strPaymentmethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                || strPaymentmethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                            {
                                _doUpdatetbt_BillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT;
                                _doUpdatetbt_BillingDetail.AutoTransferDate = dtpExpectedissueautotransferdate;
                                if (dtpExpectedissueautotransferdate.Value.AddDays(-30) < CommonUtil.dsTransData.dtOperationData.ProcessDateTime)
                                {
                                    _doUpdatetbt_BillingDetail.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                }
                                else
                                {
                                    _doUpdatetbt_BillingDetail.IssueInvDate = dtpExpectedissueautotransferdate.Value.AddDays(-30);
                                }
                            }
                            else
                            {
                                _doUpdatetbt_BillingDetail.PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT;
                                //_doUpdatetbt_BillingDetail.AutoTransferDate = null;
                                _doUpdatetbt_BillingDetail.IssueInvDate = dtpExpectedissueautotransferdate;
                            }

                            _doUpdatetbt_BillingDetail.FirstFeeFlag = isFirstFee;   /////// Add by Patcharee T. set first fee flag to in case Re-create billing detail  07-Jun-2013
                            //_doUpdatetbt_BillingDetail.DelayedMonth = null;
                            _doUpdatetbt_BillingDetail.StartOperationDate = param._doBLS050GetBillingBasic.StartOperationDate;

                            _doUpdatetbt_BillingDetail.ForceIssueFlag = true;
                            _doUpdatetbt_BillingDetail.ContractOCC = RegisterData.Detail2[i].ContractOCC;

                            // create Billing Details and Get object back for Billing Details No
                            doUpdatetbt_BillingDetailList.Add(
                                billingHandler.ManageBillingDetail(_doUpdatetbt_BillingDetail));

                            // end gen Billing Details Loop
                            if (strIssueinvoice != IssueInv.C_ISSUE_INV_NORMAL)
                            {
                                if (strIssueinvoice == IssueInv.C_ISSUE_INV_NOT_ISSUE || strBillingdetailinvoiceformat == BillingInvFormatType.C_BILLING_INV_FORMAT_SPECIFIC)
                                {

                                    //_dotbt_Invoice.InvoiceNo = null;
                                    //_dotbt_Invoice.InvoiceOCC = null;
                                    _dotbt_Invoice.IssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;


                                    _dotbt_Invoice.BillingTargetCode = param._doBLS050GetBillingBasic.BillingTargetCode;
                                    _dotbt_Invoice.BillingTypeCode = strBillingtype;

                                    //_dotbt_Invoice.InvoiceAmount = null;
                                    //_dotbt_Invoice.PaidAmountIncVat = null;
                                    //_dotbt_Invoice.VatRate = null;
                                    //_dotbt_Invoice.VatAmount = null;
                                    //_dotbt_Invoice.WHTRate = null;
                                    //_dotbt_Invoice.WHTAmount = null;
                                    //_dotbt_Invoice.RegisteredWHTAmount = null;

                                    if (strPaymentmethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                        || strPaymentmethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                    {
                                        _dotbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;
                                        _dotbt_Invoice.AutoTransferDate = dtpExpectedissueautotransferdate;
                                    }
                                    else
                                    {
                                        _dotbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                                        //_dotbt_Invoice.AutoTransferDate = null;
                                    }

                                    _dotbt_Invoice.IssueInvFlag = false;
                                    //_dotbt_Invoice.FirstIssueInvDate = null;
                                    //_dotbt_Invoice.FirstIssueInvFlag = null;
                                    _dotbt_Invoice.PaymentMethod = strPaymentmethod;
                                    //_dotbt_Invoice.CorrectReason = null;
                                    //_dotbt_Invoice.RefOldInvoiceNo = null;

                                    tbt_Invoice tempdotbt_Invoice = billingHandler.ManageInvoiceByCommand(
                                        _dotbt_Invoice
                                        , doUpdatetbt_BillingDetailList
                                        , true
                                        , true
                                        , (RegisterData.Detail2[i].strBillingdetailinvoiceformat != BillingInvFormatType.C_BILLING_INV_FORMAT_SPECIFIC ? true : false)
                                        , true
                                    );
                                }
                                else if (strIssueinvoice == IssueInv.C_ISSUE_INV_REALTIME)
                                {

                                    //_dotbt_Invoice.InvoiceNo = null;
                                    //_dotbt_Invoice.InvoiceOCC = null;
                                    _dotbt_Invoice.IssueInvDate = dtpExpectedissueautotransferdate;


                                    _dotbt_Invoice.BillingTargetCode = param._doBLS050GetBillingBasic.BillingTargetCode;
                                    _dotbt_Invoice.BillingTypeCode = strBillingtype;

                                    //_dotbt_Invoice.InvoiceAmount = null;
                                    //_dotbt_Invoice.PaidAmountIncVat = null;
                                    //_dotbt_Invoice.VatRate = null;
                                    //_dotbt_Invoice.VatAmount = null;
                                    //_dotbt_Invoice.WHTRate = null;
                                    //_dotbt_Invoice.WHTAmount = null;
                                    //_dotbt_Invoice.RegisteredWHTAmount = null;

                                    if (strPaymentmethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                        || strPaymentmethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                    {
                                        _dotbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;
                                        _dotbt_Invoice.AutoTransferDate = dtpExpectedissueautotransferdate;
                                    }
                                    else
                                    {
                                        _dotbt_Invoice.InvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                                        //_dotbt_Invoice.AutoTransferDate = null;
                                    }

                                    _dotbt_Invoice.IssueInvFlag = true;
                                    _dotbt_Invoice.FirstIssueInvDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    _dotbt_Invoice.FirstIssueInvFlag = true;
                                    _dotbt_Invoice.PaymentMethod = strPaymentmethod;
                                    //_dotbt_Invoice.CorrectReason = null;
                                    //_dotbt_Invoice.RefOldInvoiceNo = null;

                                    tbt_Invoice tempdotbt_Invoice = billingHandler.ManageInvoiceByCommand(
                                         _dotbt_Invoice
                                         , doUpdatetbt_BillingDetailList
                                         , true // (RegisterData.Detail2[i].strBillingdetailinvoiceformat == BillingInvFormatType.C_BILLING_INV_FORMAT_INV_TAXINV ? true : false)
                                         , false
                                         , true //(RegisterData.Detail2[i].strBillingdetailinvoiceformat != BillingInvFormatType.C_BILLING_INV_FORMAT_SPECIFIC ? true : false)
                                         , true
                                    );

                                    if (tempdotbt_Invoice.FilePath != null)
                                    {
                                        lstFilePath.Add(tempdotbt_Invoice.FilePath);
                                    }
                                }
                            }
                        }
                    }

                    string mergeOutputFilename = string.Empty;
                    string encryptOutputFileName = string.Empty;

                    if (lstFilePath.Count > 0)
                    {

                        mergeOutputFilename = PathUtil.GetTempFileName(".pdf");
                        encryptOutputFileName = PathUtil.GetTempFileName(".pdf");

                        bool isSuccess = ReportUtil.MergePDF(lstFilePath.ToArray(), mergeOutputFilename, true, encryptOutputFileName, null);
                        // open by popup not here
                        //if (isSuccess)
                        //{
                        //    FileStream streamFile = new FileStream(encryptOutputFileName, FileMode.Open, FileAccess.Read);
                        //    using (MemoryStream ms = new MemoryStream())
                        //    {
                        //        streamFile.CopyTo(ms);
                        //        result.ResultData = ms.ToArray();Va
                        //    }
                        //}
                    }

                    RegisterData.strFilePath = encryptOutputFileName;
                    param.RegisterData = RegisterData;

                // === COMMIT TRANSACTION ==
                    scope.Complete();

            }
            catch (Exception ex)
                {

                // == ROLLBACK TRANSACTION ==
                   scope.Dispose();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(ex);
                }
                }
            return res;
        }

        /// <summary>
        /// create or update adjust on next period billing basic detail data in database 
        /// </summary>
        /// <param name="RegisterData"> adjust on next period detail input</param>
        /// <returns></returns>
        public ObjectResultData AdjustOnNextPeriod(BLS050_RegisterData RegisterData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            string conModeRadio2rdoRegister = "1";
            string conModeRadio2rdoDelete = "2";

            string strContractCode = RegisterData.Header.strContractCode;
            string strBillingOCC = RegisterData.Header.strBillingOCC;
            string rdoProcessTypeSpe = RegisterData.Header.rdoProcessTypeSpe;

            string rdoProcessTypeAdj = RegisterData.Detail3.rdoProcessTypeAdj;
            string cboAdjustmentType = RegisterData.Detail3.cboAdjustmentType;
            string intBillingAmountAdj = RegisterData.Detail3.intBillingAmountAdj;
            string intBillingAmountAdjCurrency = RegisterData.Detail3.intBillingAmountAdjCurrency;
            DateTime? dptAdjustBillingPeriodDateFrom = RegisterData.Detail3.dptAdjustBillingPeriodDateFrom;
            DateTime? dptAdjustBillingPeriodDateTo = RegisterData.Detail3.dptAdjustBillingPeriodDateTo;


             using (TransactionScope scope = new TransactionScope())
             {

            try
            {
                    IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                    ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                    // Do Business 

                    List<tbt_BillingBasic> dotbt_BillingBasicList = billingHandler.GetTbt_BillingBasic(strContractCode, strBillingOCC);

                    tbt_BillingBasic dotbt_BillingBasic = dotbt_BillingBasicList[0];

                    if (rdoProcessTypeAdj == conModeRadio2rdoDelete)
                    {
                        dotbt_BillingBasic.AdjustType = null;
                        dotbt_BillingBasic.AdjustBillingPeriodAmount = null;
                        dotbt_BillingBasic.AdjustBillingPeriodStartDate = null;
                        dotbt_BillingBasic.AdjustBillingPeriodEndDate = null;
                    }
                    else
                    {
                        dotbt_BillingBasic.AdjustType = cboAdjustmentType;
                        dotbt_BillingBasic.AdjustBillingPeriodAmountCurrencyType = intBillingAmountAdjCurrency;
                        if (cboAdjustmentType == AdjustType.C_ADJUST_TYPE_ADD)
                        {
                            if (intBillingAmountAdjCurrency == "1")
                            {
                                dotbt_BillingBasic.AdjustBillingPeriodAmount = Convert.ToDecimal(intBillingAmountAdj);
                                dotbt_BillingBasic.AdjustBillingPeriodAmountUsd = null;
                            }
                            else
                            {
                                dotbt_BillingBasic.AdjustBillingPeriodAmount = null;
                                dotbt_BillingBasic.AdjustBillingPeriodAmountUsd = Convert.ToDecimal(intBillingAmountAdj);
                            }
                            //dotbt_BillingBasic.AdjustBillingPeriodAmount = Convert.ToDecimal(intBillingAmountAdj);
                        }
                        else
                        {
                            if (intBillingAmountAdjCurrency == "1")
                            {
                                dotbt_BillingBasic.AdjustBillingPeriodAmount = Convert.ToDecimal(intBillingAmountAdj) * (decimal)(-1);
                                dotbt_BillingBasic.AdjustBillingPeriodAmountUsd = null;
                            }
                            else
                            {
                                dotbt_BillingBasic.AdjustBillingPeriodAmount = null;
                                dotbt_BillingBasic.AdjustBillingPeriodAmountUsd = Convert.ToDecimal(intBillingAmountAdj) * (decimal)(-1);
                            }
                            //dotbt_BillingBasic.AdjustBillingPeriodAmount = Convert.ToDecimal(intBillingAmountAdj) * (decimal)(-1);
                        }

                        dotbt_BillingBasic.AdjustBillingPeriodStartDate = dptAdjustBillingPeriodDateFrom;
                        dotbt_BillingBasic.AdjustBillingPeriodEndDate = dptAdjustBillingPeriodDateTo;
                    }

                    int iRowEffect = billingHandler.UpdateTbt_BillingBasic(dotbt_BillingBasic);

                // == COMMIT TRANSACTION ==
                scope.Complete();
            }
            catch (Exception ex)
                {

                // == ROLLBANK TRANSACTION ==
                  scope.Dispose();

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(ex);
                }
              }
            return res;
        }

        /// <summary>
        /// retrieve billing type group detail data in database by billing type code
        /// </summary>
        /// <param name="ContractCode">contract code input</param>
        /// <param name="BillingOCC">billing occ input</param>
        /// <param name="BillingTypeCode">billing type code input</param>
        /// <returns>billing type group code</returns>
        string BLS050_fnGetBillingTypeGroup(string BillingTypeCode)
        {
            string strBillingTypeGroup = string.Empty;

            IBillingMasterHandler billingMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

            if (BillingTypeCode == null)
            {
                BillingTypeCode = "xx";
            }
            tbm_BillingType _dotbm_BillingType = billingMasterHandler.GetTbm_BillingType(BillingTypeCode);

            if (_dotbm_BillingType != null)
            {
                strBillingTypeGroup = _dotbm_BillingType.BillingTypeGroup;
            }

            return strBillingTypeGroup;
        }

        /// <summary>
        /// retrieve billing type group detail data in database by billing type code for client jscript operate
        /// </summary>
        /// <param name="BillingTypeCode">billing type code input</param>
        /// <returns>billing type group code in Json format</returns>
        public ActionResult BLS050_GetBillingTypeGroup(string BillingTypeCode)
        {

            try
            {
                BLS050_ScreenParameter sParam = GetScreenObject<BLS050_ScreenParameter>();
                CommonUtil comUtil = new CommonUtil();

                string billingTypeGroup = BLS050_fnGetBillingTypeGroup(BillingTypeCode);
                bool OCCMode = false;

                ICommonHandler common = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                doSystemConfig config = null;
                if (sParam.ContractCode != null && sParam.ContractCode.StartsWith("Q"))
                {
                    config = common.GetSystemConfig(ConfigName.C_CONFIG_BLS050_Q_BILLINGTYPEFOROCC).FirstOrDefault();
                }
                else
                {
                    config = common.GetSystemConfig(ConfigName.C_CONFIG_BLS050_N_BILLINGTYPEFOROCC).FirstOrDefault();
                }

                if (config != null && config.ConfigValue != null)
                {
                    OCCMode = config.ConfigValue.Split(',').Contains(BillingTypeCode);
                }

                return Json(new { BillingTypeGroup = billingTypeGroup, OCCMode = OCCMode });

            }
            catch (Exception ex)
            {
                //ObjectResultData res = new ObjectResultData();
                //res.AddErrorMessage(ex);
                //return Json(res);
                return Json("");
            }
        }

        class BusinessStartEndDate
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public int ProblemCase { get; set; }
        }

        /// <summary>
        /// check continue date period function
        /// </summary>
        /// <param name="DateRangesForDataList">Start and End date on every input transaction or/and database select</param>
        /// <returns>true:date period is continue, false: date period is not continue</returns>
        BusinessStartEndDate BLS050_fnCheckBusinessDate(List<BusinessStartEndDate> DateRangesForDataList, List<BusinessStartEndDate> RegisterDataDateRangesList)
        {
            // example how to add date

            //List<BusinessStartEndDate> DateRangesForDataList = new List<BusinessStartEndDate>();
            //DateRangesForDataList.Add(new BusinessStartEndDate {
            //Start = DateTime.Now,
            //End = DateTime.Now}

            BusinessStartEndDate bolReturn = new BusinessStartEndDate();

            IEnumerable<BusinessStartEndDate> query =
                DateRangesForDataList.OrderBy(
                BusinessStartEndDate => BusinessStartEndDate.Start);

            // example perfect period is some thing like 
            // 1 - 2
            // 3 - 4
            // 5 - 6
            // when compare compare only 2 loop or array count - 1
            // 2 + 1day = 3
            // 4 + 1day = 5

            BusinessStartEndDate TempDateRangesForData = null;
            foreach (BusinessStartEndDate DateRangesForData in query)
            {
                // first row of date
                if (TempDateRangesForData == null)
                {
                    TempDateRangesForData = DateRangesForData;
                }
                else
                {
                    // if not continue period
                    if (TempDateRangesForData.End.AddDays(1) != DateRangesForData.Start)
                    {

                        var isRegisterData = (from RegisterData in RegisterDataDateRangesList
                                              where RegisterData.Start == DateRangesForData.Start || RegisterData.End == DateRangesForData.End
                                              select RegisterData).Any();

                        if (isRegisterData) //Error in case Register data is incorrect
                        {
                            if (TempDateRangesForData.Start == DateRangesForData.Start &&
                            TempDateRangesForData.End == DateRangesForData.End)
                            {
                                // duplicate period
                                DateRangesForData.ProblemCase = 1;
                            }
                            else if (DateRangesForData.Start > TempDateRangesForData.End)
                            {
                                // not continue
                                DateRangesForData.ProblemCase = 9;
                            }
                            else
                            {
                                // overlap period
                                DateRangesForData.ProblemCase = 2;
                            }
                            // return problem date period
                            return DateRangesForData;
                        }
                        else
                        {
                            // continue date shift to next date period
                            TempDateRangesForData = DateRangesForData;
                        }


                    }
                    else
                    {
                        // continue date shift to next date period
                        TempDateRangesForData = DateRangesForData;
                    }
                }
            }

            return bolReturn;
        }


        /// <summary>
        /// Mothod for download document (PDF) and write history to download log
        /// </summary>
        /// <param name="strDocumentNo"></param>
        /// <param name="documentOCC"></param>
        /// <param name="strDocumentCode"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult BLS050_GetInvoiceReport(string fileName)
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
