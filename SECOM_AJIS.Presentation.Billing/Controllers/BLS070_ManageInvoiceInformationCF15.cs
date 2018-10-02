
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
        public ActionResult BLS070_Authority(BLS070_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();

            // System Suspend
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            if (handlerCommon.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_MANAGE_INVOICE, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            if (param != null && param.CommonSearch != null)
            {
                //param.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                param.ContractCode = param.CommonSearch.ContractCode;

            }


            return InitialScreenEnvironment<BLS070_ScreenParameter>("BLS070", param, res);

        }

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS070")]
        public ActionResult BLS070()
        {
            return View();
        }

        /// <summary>
        /// Generate xml for initial separate invoice grid
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS070_InitialSeparateInvoiceGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS070_SeparateInvoice", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Generate xml for initial separate invoice detail grid
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS070_InitialSeparateInvoiceDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS070_SeparateInvoiceDetail", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Generate xml for initial combine invoice grid
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS070_InitialCombineInvoiceGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS070_CombineInvoice", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Generate xml for initial combine iInvoice detail grid
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS070_InitialCombineInvoiceDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS070_CombineInvoiceDetail", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Generate issue invoice of separate detail comboitem list 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult BLS070_GetComboBoxIssueInvoiceofSeparateDetail(string id)
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
        /// Initial Screen Load by CMS210 data
        /// </summary>
        /// <param name="data">Parameter From CMS210</param>
        /// <returns></returns>
        public ActionResult BLS070_InitDataFromCMS210(BLS070_RegisterData data)
        {
            BLS070_ScreenParameter sParam = GetScreenObject<BLS070_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                string mode_SeparateInvoice = "1";
                string mode_CombineInvoice = "2";
                string mode_SaleInvoice = "3";

                if (sParam.ProcessType == mode_SaleInvoice)
                {
                    sParam.defSelectProcessType = mode_SaleInvoice;
                    sParam.defstrContractCode = sParam.ContractCode;
                    sParam.defstrBillingOCC = sParam.OCC;
                }

                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                {
                    res.ResultData = sParam;
                }
                else
                {
                    res.ResultData = null;
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve invoice information list of specific screen mode and search criteria information
        /// </summary>
        /// <param name="data">Search criteria</param>
        /// <returns></returns>
        public ActionResult BLS070_RetrieveData(BLS070_RegisterData data)
        {
            string mode_SeparateInvoice = "1";
            string mode_CombineInvoice = "2";
            string mode_IssueSaleInvoice = "3";

            BLS070_ScreenParameter param = GetScreenObject<BLS070_ScreenParameter>();
            BLS070_RegisterData RegisterData = new BLS070_RegisterData();
            CommonUtil cm = new CommonUtil();
            List<doMiscTypeCode> currencies = new List<doMiscTypeCode>();
            List<dtTbt_BillingTargetForView> lstBillingTarget = new List<dtTbt_BillingTargetForView>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                // Add by Jirawat Jannet : 2016-08-24

                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                // End Add

                // Common Check Sequence

                // System Suspend
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                IContractHandler contractHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                ISaleContractHandler handlerSaleContract = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                //tbt_Invoice dtTbt_Invoice = new tbt_Invoice();
                doInvoice dtInvoice = new doInvoice();

                List<doGetBillingDetailOfInvoice> dtBillingDetailOfInvoice = new List<doGetBillingDetailOfInvoice>();
                List<tbt_BillingBasic> dtTbt_BillingBasic = new List<tbt_BillingBasic>();

                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }


                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                BLS070_ScreenParameter sParam = GetScreenObject<BLS070_ScreenParameter>();
                ValidatorUtil validator = new ValidatorUtil();




                if (data == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                     "BLS070",
                                     MessageUtil.MODULE_COMMON,
                                     MessageUtil.MessageList.MSG0007,
                                     new string[] { "lblSelSeparateFromInvoiceNo" },
                                     new string[] { "txtSelSeparateFromInvoiceNo" });

                    if (res.IsError)
                    {
                        return Json(res);
                    }
                }
                if (data.Header == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                     "BLS070",
                                     MessageUtil.MODULE_COMMON,
                                     MessageUtil.MessageList.MSG0007,
                                     new string[] { "lblSelSeparateFromInvoiceNo" },
                                     new string[] { "txtSelSeparateFromInvoiceNo" });

                    if (res.IsError)
                    {
                        return Json(res);
                    }
                }


                // === Have 3 modes ===


                // == Separate Invoice mode
                if (data.Header.rdoProcessSelect == mode_SeparateInvoice)
                {

                    if (String.IsNullOrEmpty(data.Header.txtSelSeparateFromInvoiceNo))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         new string[] { "lblSelSeparateFromInvoiceNo" },
                                         new string[] { "txtSelSeparateFromInvoiceNo" });
                    }

                    if (res.IsError)
                    {
                        return Json(res);
                    }


                    dtInvoice = handlerBilling.GetInvoice(data.Header.txtSelSeparateFromInvoiceNo);

                    if (dtInvoice == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6043, null, new string[] { "txtSelSeparateFromInvoiceNo" });

                        if (res.IsError)
                        {
                            return Json(res);
                        }
                    }

                    param.VatCurrencyCode = dtInvoice.VatAmountCurrencyType;
                    param.WHTCurrencyCode = dtInvoice.WHTAmountCurrencyType;
                    param.VatCurrency = currencies.Where(m=>m.ValueCode == dtInvoice.VatAmountCurrencyType).Select(m=>m.ValueDisplayEN).FirstOrDefault();
                    param.WHTCurrency = currencies.Where(m => m.ValueCode == dtInvoice.WHTAmountCurrencyType).Select(m => m.ValueDisplayEN).FirstOrDefault();

                    if(string.IsNullOrEmpty(param.VatCurrency) || string.IsNullOrEmpty(param.WHTCurrency))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0063,
                                            null,
                                            new string[] { "txtSelSeparateFromInvoiceNo" });
                        return Json(res);
                    }

                    List<doInvoice> dtInvoice_list = new List<doInvoice>();
                    dtInvoice_list.Add(dtInvoice);

                    // Misc Mapping  
                    MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                    miscMapping.AddMiscType(dtInvoice_list.ToArray());
                    handlerCommon.MiscTypeMappingList(miscMapping);

                    dtInvoice = dtInvoice_list[0];

                    lstBillingTarget = handlerBilling.GetTbt_BillingTargetForView(dtInvoice.BillingTargetCode, MiscType.C_CUST_TYPE);
                    if (lstBillingTarget.Count > 0)
                    {
                        var existsBillingOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == lstBillingTarget[0].BillingOfficeCode);
                        if (existsBillingOffice.Count() <= 0)
                        {
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0063,
                                                null,
                                                new string[] { "txtSelSeparateFromInvoiceNo" });
                            return Json(res);
                        }
                    }

                    if (dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                                && dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                                && dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                                && dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                                && dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                                && dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_COUNTER_BAL)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             MessageUtil.MessageList.MSG6073,
                                             new string[] { dtInvoice.InvoiceNo, dtInvoice.InvoicePaymentStatusDisplayName },
                                             new string[] { "txtSelSeparateFromInvoiceNo" });

                        if (res.IsError)
                        {
                            return Json(res);
                        }
                    }
                    else
                    {
                        param.decInvoiceTotal = 0;
                        param.intInvoiceCount = 0;



                        dtBillingDetailOfInvoice = handlerBilling.GetBillingDetailOfInvoiceList(dtInvoice.InvoiceNo, dtInvoice.InvoiceOCC);

                        if (dtBillingDetailOfInvoice != null)
                        {
                            foreach (doGetBillingDetailOfInvoice billingDetail in dtBillingDetailOfInvoice)
                            {
                                billingDetail.BillingAmountCurrencyTypeName = currencies.Where(m => m.ValueCode == billingDetail.BillingAmountCurrencyType).Select(m => m.ValueDisplayEN).FirstOrDefault();
                                dtTbt_BillingBasic = handlerBilling.GetTbt_BillingBasic(billingDetail.ContractCode, billingDetail.BillingOCC);

                                if (dtTbt_BillingBasic.Count > 0)
                                {
                                    if (dtTbt_BillingBasic[0].CarefulFlag == true)
                                    {
                                        //MSG6056
                                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             "BLS070",
                                                             MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6056,
                                                             new string[] { billingDetail.ContractCodeShort + "-" + dtTbt_BillingBasic[0].BillingOCC },
                                                             new string[] { "txtSelSeparateFromInvoiceNo" });

                                        if (res.IsError)
                                        {
                                            return Json(res);
                                        }
                                    }

                                }

                                param.decInvoiceTotal = (param.decInvoiceTotal ?? 0M) + (billingDetail.BillingAmount ?? 0M);
                            }

                            param.intInvoiceCount = dtBillingDetailOfInvoice.Count;

                        }
                    }


                    param.doGetInvoiceWithBillingClientName = dtInvoice;
                    param.doGetBillingDetailOfInvoiceList = dtBillingDetailOfInvoice;
                    param.dotbt_BillingBasicList = dtTbt_BillingBasic;


                }

                // === Combine Invoice mode
                else if (data.Header.rdoProcessSelect == mode_CombineInvoice)
                {

                    if (String.IsNullOrEmpty(data.Header.txtSelCombineToInvoiceNo))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                             "BLS070",
                                             MessageUtil.MODULE_COMMON,
                                             MessageUtil.MessageList.MSG0007,
                                             new string[] { "lblSelCombineToInvoiceNo" },
                                             new string[] { "txtSelCombineToInvoiceNo" });
                    }

                    if (res.IsError)
                    {
                        return Json(res);
                    }


                    dtInvoice = handlerBilling.GetInvoice(data.Header.txtSelCombineToInvoiceNo);

                    if (dtInvoice == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6043, null, new string[] { "txtSelCombineToInvoiceNo" });

                        if (res.IsError)
                        {
                            return Json(res);
                        }
                    }
                    //Add by Yasinthon
                    param.VatCurrencyCode = dtInvoice.VatAmountCurrencyType ?? SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    param.WHTCurrencyCode = dtInvoice.WHTAmountCurrencyType ?? SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    param.VatCurrency = currencies.Where(m => m.ValueCode == dtInvoice.VatAmountCurrencyType).Select(m => m.ValueDisplayEN).FirstOrDefault();
                    param.WHTCurrency = currencies.Where(m => m.ValueCode == dtInvoice.WHTAmountCurrencyType).Select(m => m.ValueDisplayEN).FirstOrDefault();

                    //Add by Jutarat A. on 30012013
                    // Misc Mapping  
                    MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                    miscMapping.AddMiscType(dtInvoice);
                    handlerCommon.MiscTypeMappingList(miscMapping);
                    //End Add

                    lstBillingTarget = handlerBilling.GetTbt_BillingTargetForView(dtInvoice.BillingTargetCode, MiscType.C_CUST_TYPE);
                    if (lstBillingTarget.Count > 0)
                    {
                        var existsBillingOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == lstBillingTarget[0].BillingOfficeCode);
                        if (existsBillingOffice.Count() <= 0)
                        {
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0063,
                                                null,
                                                new string[] { "txtSelCombineToInvoiceNo" });
                            return Json(res);
                        }
                    }
                    if (dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                        && dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                        && dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                        && dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                        && dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                        && dtInvoice.InvoicePaymentStatus != PaymentStatus.C_PAYMENT_STATUS_COUNTER_BAL)
                    {

                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                            MessageUtil.MessageList.MSG6074,
                                            new string[] { dtInvoice.InvoiceNo, dtInvoice.InvoicePaymentStatusDisplayName },
                                            new string[] { "txtSelCombineToInvoiceNo" });

                        if (res.IsError)
                        {
                            return Json(res);
                        }
                    }
                    else
                    {
                        param.decInvoiceTotal = 0;
                        param.intInvoiceCount = 0;

                        dtBillingDetailOfInvoice = handlerBilling.GetBillingDetailOfInvoiceList(dtInvoice.InvoiceNo, dtInvoice.InvoiceOCC);

                        CommonUtil.MappingObjectLanguage<doGetBillingDetailOfInvoice>(dtBillingDetailOfInvoice);

                        if (dtBillingDetailOfInvoice != null)
                        {
                            foreach (doGetBillingDetailOfInvoice billingDetail in dtBillingDetailOfInvoice)
                            {

                                dtTbt_BillingBasic = handlerBilling.GetTbt_BillingBasic(billingDetail.ContractCode, billingDetail.BillingOCC);

                                if (dtTbt_BillingBasic.Count > 0)
                                {
                                    if (dtTbt_BillingBasic[0].CarefulFlag == true)
                                    {
                                        //MSG6056
                                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             "BLS070",
                                                             MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6056,
                                                             new string[] { billingDetail.ContractCodeShort + "-" + billingDetail.BillingOCC },
                                                             new string[] { "txtSelCombineToInvoiceNo" });

                                        if (res.IsError)
                                        {
                                            return Json(res);
                                        }
                                    }

                                }

                                param.decInvoiceTotal = (param.decInvoiceTotal ?? 0) + (billingDetail.BillingAmount ?? 0);
                            }

                            param.intInvoiceCount = dtBillingDetailOfInvoice.Count;
                        }
                    }

                    param.doGetInvoiceWithBillingClientName = dtInvoice;
                    param.doGetBillingDetailOfInvoiceList = dtBillingDetailOfInvoice;
                    param.dotbt_BillingBasicList = dtTbt_BillingBasic;


                    // Add by Jirawat Jannet : 2016-08-24
                    // Currenct list datas
                    ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                    lst = hand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();
                    if(lst != null && lst.Count > 0)
                    {
                        foreach (var d in param.doGetBillingDetailOfInvoiceList)
                        {
                            d.BillingAmountCurrencyTypeName = lst.Where(m => m.ValueCode == d.BillingAmountCurrencyType).Select(m => m.ValueDisplayEN).FirstOrDefault();
                            if (d.BillingAmountCurrencyType == null) d.BillingAmountCurrencyType = lst[0].ValueDisplayEN;
                        }
                    }
                }

                // === Issue Sale Invoice mode
                else if (data.Header.rdoProcessSelect == mode_IssueSaleInvoice)
                {

                    if (String.IsNullOrEmpty(data.Header.txtSelContractCode))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "txtSelContractCode",
                                         "lblSelContractCode",
                                         "txtSelContractCode");
                    };

                    if (String.IsNullOrEmpty(data.Header.txtSelSaleOCC))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         "txtSelSaleOCC",
                                         "lblSelSaleOCC",
                                         "txtSelSaleOCC");
                    };

                    

                    ValidatorUtil.BuildErrorMessage(res, validator, null);
                    if (res.IsError)
                    {
                        return Json(res);
                    }

                    List<doGetSaleDataForIssueInvoice> dtSaleDataForIssueInvoice = new List<doGetSaleDataForIssueInvoice>();


                    dtSaleDataForIssueInvoice = handlerSaleContract.GetSaleDataForIssueInvoice(cm.ConvertBillingCode(data.Header.txtSelContractCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                                                                                               , data.Header.txtSelSaleOCC);

                    if (dtSaleDataForIssueInvoice == null || dtSaleDataForIssueInvoice.Count == 0)
                    {
                        //MSG6071
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6071, null, new string[] { "txtSelContractCode", "txtSelSaleOCC" });

                        if (res.IsError)
                        {
                            return Json(res);
                        }
                    }
                    else
                    {
                        param.VatCurrencyCode = dtSaleDataForIssueInvoice[0].BillingAmtCurrencyType;
                        param.WHTCurrencyCode = dtSaleDataForIssueInvoice[0].BillingAmtCurrencyType;
                        param.VatCurrency = currencies.Where(m => m.ValueCode == dtSaleDataForIssueInvoice[0].BillingAmtCurrencyType).Select(m => m.ValueDisplayEN).FirstOrDefault();
                        param.WHTCurrency = currencies.Where(m => m.ValueCode == dtSaleDataForIssueInvoice[0].BillingAmtCurrencyType).Select(m => m.ValueDisplayEN).FirstOrDefault();

                        lstBillingTarget = handlerBilling.GetTbt_BillingTargetForView(dtSaleDataForIssueInvoice[0].BillingTargetCode, MiscType.C_CUST_TYPE);
                        if (lstBillingTarget.Count > 0)
                        {
                            var existsBillingOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == lstBillingTarget[0].BillingOfficeCode);
                            if (existsBillingOffice.Count() <= 0)
                            {
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0063,
                                                    null,
                                                    new string[] { "txtSelContractCode", "txtSelSaleOCC" });
                                return Json(res);
                            }
                        }
                        if (dtSaleDataForIssueInvoice[0].SaleProcessManageStatus != SaleProcessManageStatus.C_SALE_PROCESS_STATUS_SHIP
                            && dtSaleDataForIssueInvoice[0].SaleProcessManageStatus != SaleProcessManageStatus.C_SALE_PROCESS_STATUS_COMPLETE_NOTACCEPT)
                        {
                            //MSG6064
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6064);

                            if (res.IsError)
                            {
                                return Json(res);
                            }
                        }
                    }

                    doTax dtTax = new doTax();

                    dtTax = handlerBilling.GetTaxChargedData(dtSaleDataForIssueInvoice[0].ContractCode
                                                            , dtSaleDataForIssueInvoice[0].BillingOCC
                                                            , BillingType.C_BILLING_TYPE_SALE_PRICE
                                                            , DateTime.Now);

                    sParam.ContractCode_short = data.Header.txtSelContractCode;

                    if (dtTax != null)
                    {
                        sParam.txtVATAmount = ((dtSaleDataForIssueInvoice[0].BillingAmt ?? 0M) * (dtTax.VATRate ?? 0M)).ToString("N2");
                        sParam.txtWHTAmount = ((dtSaleDataForIssueInvoice[0].BillingAmt ?? 0M) * (dtTax.WHTRate ?? 0M)).ToString("N2");
                    }
                    else
                    {
                        sParam.txtVATAmount = "0.00";
                        sParam.txtWHTAmount = "0.00";
                    }



                    if (dtSaleDataForIssueInvoice != null && dtSaleDataForIssueInvoice.Count > 0)
                    {
                        sParam.doGetSaleDataForIssueInvoice = dtSaleDataForIssueInvoice; //Merge at 14032017 By Pachara S.
                    }

                    sParam.bIssuePartialFee = false;


                }
                else
                {
                    // not select option
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        "BLS070",
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblSelSeparateFromInvoiceNo" },
                                        new string[] { "txtSelSeparateFromInvoiceNo" });

                    if (res.IsError)
                    {
                        return Json(res);
                    }
                }

                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                {
                    res.ResultData = sParam;
                }
                else
                {
                    res.ResultData = null;
                }

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
        public ActionResult BLS070_Register(BLS070_RegisterData data)
        {
            string mode_SeparateInvoice = "1";
            string mode_CombineInvoice = "2";
            string mode_IssueSaleInvoice = "3";


            BLS070_ScreenParameter param = GetScreenObject<BLS070_ScreenParameter>();
            BLS070_RegisterData RegisterData = new BLS070_RegisterData();
            CommonUtil cm = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ObjectResultData res_SepatateInvoice = new ObjectResultData();
            ObjectResultData res_CombineInvoice = new ObjectResultData();
            ObjectResultData res_IssueInvoice = new ObjectResultData();

            try
            {
                // Common Check Sequence

                // System Suspend
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // === Separate Invoice mode ==
                if (data.Header.rdoProcessSelect == mode_SeparateInvoice)
                {
                    res_SepatateInvoice = BLS070_ValidateSeparateInvoice(data, param.doGetInvoiceWithBillingClientName, param.doGetBillingDetailOfInvoiceList);

                    if (res_SepatateInvoice.MessageList != null)
                    {
                        if (res_SepatateInvoice.MessageList.Count > 0)
                        {
                            return Json(res_SepatateInvoice);
                        }
                    }
                }

                // === Combine Invoice mode ==
                else if (data.Header.rdoProcessSelect == mode_CombineInvoice)
                {
                    //
                    res_CombineInvoice = BLS070_ValidateCombineInvoice(data
                                                                , param.doGetInvoiceWithBillingClientName
                                                                , param.doGetBillingDetailOfInvoiceList);

                    if (res_CombineInvoice.MessageList != null)
                    {
                        if (res_CombineInvoice.MessageList.Count > 0)
                        {
                            return Json(res_CombineInvoice);
                        }
                    }
                }

                // === Issue Sale Invoice mode ==
                else if (data.Header.rdoProcessSelect == mode_IssueSaleInvoice)
                {
                    //
                    res_IssueInvoice = BLS070_ValidateIssueSaleInvoice(data
                                                                , param.doGetInvoiceWithBillingClientName
                                                                , param.doGetBillingDetailOfInvoiceList);

                    if (res_IssueInvoice.MessageList != null)
                    {
                        if (res_IssueInvoice.MessageList.Count > 0)
                        {
                            return Json(res_IssueInvoice);
                        }
                    }
                }

                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }

                // return "0" to js is not OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                {
                    // Alert_SeparateAll

                    var result = new { bAlertConfirmDialog = data.bAlertConfirmDialog, Alert_SeparateAll = (res_SepatateInvoice.ResultData == "Alert_SeparateAll") };
                    res.ResultData = result;
                }
                else
                {
                    res.ResultData = "0";
                }


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
        /// cancel invoice and re create data in database 
        /// </summary>
        /// <param name="doGetInvoiceWithBillingClientName">delete invoice criteria</param>
        /// <param name="dotbt_BillingDetailList">billing details of delete invoice</param>
        /// <param name="bNotChangeInvoiceNo">change invoice number flag</param>
        /// <param name="strPaymentMethod">input payment method</param>
        /// <param name="strIssueInvoice">input issue invoice</param>
        /// <returns></returns>
        public ObjectResultData BLS070_CancelAndCreateInvoice(doInvoice dtInvoice, List<tbt_BillingDetail> dtTbt_BillingDetail, bool bNotChangeInvoiceNo, string strPaymentMethod, string strIssueInvoice, List<string> reportList)
        {
            ObjectResultData res = new ObjectResultData();

            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                List<tbt_BillingDetail> doManagetbt_BillingDetailList = new List<tbt_BillingDetail>(); //Add by Jutarat A. on 07052013
                List<tbt_BillingDetail> doAfterManagetbt_BillingDetailList = new List<tbt_BillingDetail>(); //Add by Jutarat A. on 07052013

                // Prepare
                tbt_Invoice dtTbt_Invoice = new tbt_Invoice()
                {
                    InvoiceNo = dtInvoice.InvoiceNo,
                    InvoiceOCC = dtInvoice.InvoiceOCC,
                    IssueInvDate = dtInvoice.IssueInvDate,
                    AutoTransferDate = dtInvoice.AutoTransferDate,
                    BillingTargetCode = dtInvoice.BillingTargetCode,
                    BillingTypeCode = dtInvoice.BillingTypeCode,
                    InvoiceAmount = dtInvoice.InvoiceAmount,
                    PaidAmountIncVat = dtInvoice.PaidAmountIncVat,
                    VatRate = dtInvoice.VatRate,
                    VatAmount = dtInvoice.VatAmount,
                    WHTRate = dtInvoice.WHTRate,
                    WHTAmount = dtInvoice.WHTAmount,
                    RegisteredWHTAmount = dtInvoice.RegisteredWHTAmount,
                    InvoicePaymentStatus = dtInvoice.InvoicePaymentStatus,
                    IssueInvFlag = dtInvoice.IssueInvFlag,
                    FirstIssueInvDate = dtInvoice.FirstIssueInvDate,
                    FirstIssueInvFlag = dtInvoice.FirstIssueInvFlag,
                    PaymentMethod = strPaymentMethod,
                    CorrectReason = dtInvoice.CorrectReason,
                    RefOldInvoiceNo = dtInvoice.RefOldInvoiceNo,

                    VatAmountUsd = dtInvoice.VatAmountUsd,
                    VatAmountCurrencyType = dtInvoice.VatAmountCurrencyType,
                    WHTAmountUsd = dtInvoice.WHTAmountUsd,
                    WHTAmountCurrencyType = dtInvoice.WHTAmountCurrencyType,
                    InvoiceAmountUsd = dtInvoice.InvoiceAmountUsd,
                    InvoiceAmountCurrencyType = dtInvoice.InvoiceAmountCurrencyType
                };


                // UpdateInvoicePaymentStatus !!
                handlerBilling.UpdateInvoicePaymentStatus(dtTbt_Invoice, dtTbt_BillingDetail, PaymentStatus.C_PAYMENT_STATUS_CANCEL, dtInvoice.UpdateDate); //Modify (Add dtUpdateDate) by Jutarat A. on 25112013

                if (dtTbt_BillingDetail.Count > 0) //Have binlling detail in invoice
                {
                    // == Get Next Auto transfer date incase payment method is autotransfer or credit card ==
                    if (strPaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                        || strPaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                    {

                        dtInvoice.AutoTransferDate = handlerBilling.GetNextAutoTransferDate(dtTbt_BillingDetail[0].ContractCode
                                                                                       , dtTbt_BillingDetail[0].BillingOCC
                                                                                       , strPaymentMethod);

                        if (dtInvoice.AutoTransferDate.HasValue == false)
                        {
                            strPaymentMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                        }

                        else if (!bNotChangeInvoiceNo)
                        {
                            if (dtInvoice.AutoTransferDate.Value.AddDays(-30) < DateTime.Now)
                            {
                                dtInvoice.IssueInvDate = DateTime.Now;
                            }
                            else
                            {
                                dtInvoice.IssueInvDate = dtInvoice.AutoTransferDate.Value.AddDays(-30);
                            }
                        }
                    }
                    else
                    {
                        dtInvoice.AutoTransferDate = null;
                        if (!bNotChangeInvoiceNo)
                        {
                            dtInvoice.IssueInvDate = DateTime.Now;
                        }
                    }

                    string strInvoicePaymentStatus = string.Empty;
                    if (strPaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER || strPaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                    {
                        strInvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;
                    }
                    else
                    {
                        strInvoicePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT;
                    }

                    string strBillingTargetCode = null;
                    foreach (var item in dtTbt_BillingDetail)
                    {
                        var billingBasic = handlerBilling.GetTbt_BillingBasic(item.ContractCode, item.BillingOCC);

                        if (billingBasic.Count > 0)
                        {
                            if (strBillingTargetCode == null)
                            {
                                strBillingTargetCode = billingBasic[0].BillingTargetCode;
                            }

                            if (strBillingTargetCode != billingBasic[0].BillingTargetCode)
                            {
                                // Error
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6081);
                                return res;
                            }
                        }

                        // === Issue billing detail === 
                        tbt_BillingDetail billingDeatail = new tbt_BillingDetail()
                        {
                            ContractCode = item.ContractCode,
                            BillingOCC = item.BillingOCC,
                            BillingDetailNo = 0,
                            InvoiceNo = bNotChangeInvoiceNo == true ? item.InvoiceNo : null,
                            InvoiceOCC = null,
                            IssueInvDate = dtInvoice.IssueInvDate,
                            IssueInvFlag = item.IssueInvFlag,
                            BillingTypeCode = item.BillingTypeCode,
                            BillingTypeGroup = null,
                            BillingAmount = item.BillingAmount,
                            AdjustBillingAmount = item.AdjustBillingAmount, //Add by Jutarat A. on 07052013
                            AdjustBillingAmountUsd = item.AdjustBillingAmountUsd,
                            AdjustBillingAmountCurrencyType = item.AdjustBillingAmountCurrencyType,
                            BillingStartDate = item.BillingStartDate,
                            BillingEndDate = item.BillingEndDate,
                            PaymentMethod = strPaymentMethod,
                            PaymentStatus = (strPaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER ||
                                             strPaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                             ? PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT : PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT,
                            AutoTransferDate = dtInvoice.AutoTransferDate,
                            FirstFeeFlag = item.FirstFeeFlag,
                            DelayedMonth = item.DelayedMonth,
                            StartOperationDate = null,
                            ContractOCC = item.ContractOCC,
                            ForceIssueFlag = item.ForceIssueFlag,

                            BillingAmountUsd = item.BillingAmountUsd,
                            BillingAmountCurrencyType = item.BillingAmountCurrencyType

                        };

                        //Modify by Jutarat A. on 07052013
                        //var billingDetail_result = handlerBilling.ManageBillingDetail(billingDeatail);
                        //item.BillingDetailNo = billingDetail_result != null ? billingDetail_result.BillingDetailNo : item.BillingDetailNo;
                        doManagetbt_BillingDetailList.Add(billingDeatail);
                        //End Modify

                    }// end loop

                    //Add by Jutarat A. on 07052013
                    if (doManagetbt_BillingDetailList != null && doManagetbt_BillingDetailList.Count > 0)
                        doAfterManagetbt_BillingDetailList.AddRange(handlerBilling.ManageBillingDetail(doManagetbt_BillingDetailList));
                    //End Add

                    if (strIssueInvoice != IssueInv.C_ISSUE_INV_NORMAL || bNotChangeInvoiceNo == true) // == Create new invoice
                    {
                        tbt_Invoice invoice = new tbt_Invoice()
                        {
                            InvoiceNo = (bNotChangeInvoiceNo == true) ? dtInvoice.InvoiceNo : null,
                            InvoiceOCC = 0,
                            IssueInvDate = dtInvoice.IssueInvDate,
                            AutoTransferDate = dtInvoice.AutoTransferDate,
                            BillingTargetCode = (bNotChangeInvoiceNo == true) ? dtInvoice.BillingTargetCode : strBillingTargetCode,
                            BillingTypeCode = dtInvoice.BillingTypeCode,
                            InvoiceAmount = null,
                            PaidAmountIncVat = null,
                            VatRate = null,
                            VatAmount = null,
                            WHTRate = null,
                            WHTAmount = null,
                            RegisteredWHTAmount = null,
                            InvoicePaymentStatus = strInvoicePaymentStatus,
                            IssueInvFlag = (strIssueInvoice == IssueInv.C_ISSUE_INV_NOT_ISSUE) ? false : true,
                            FirstIssueInvDate = null,
                            FirstIssueInvFlag = false,
                            PaymentMethod = strPaymentMethod,
                            CorrectReason = null,
                            RefOldInvoiceNo = dtInvoice.InvoiceNo,

                            VatAmountUsd = null,
                            VatAmountCurrencyType = null,
                            WHTAmountUsd = null,
                            WHTAmountCurrencyType = null,
                            InvoiceAmountUsd = null,
                            InvoiceAmountCurrencyType = null
                        };

                        // ManageInvoiceByCommand !!  + Report output
                        if (strIssueInvoice == IssueInv.C_ISSUE_INV_REALTIME)
                        {
                            invoice.FirstIssueInvDate = DateTime.Now;
                            invoice.FirstIssueInvFlag = true;

                            //var invoice_realTime_result = handlerBilling.ManageInvoiceByCommand(invoice, dtTbt_BillingDetail, false, false);
                            tbt_Invoice invoice_realTime_result = handlerBilling.ManageInvoiceByCommand(
                                                                        invoice, 
                                                                        doAfterManagetbt_BillingDetailList, 
                                                                        true, 
                                                                        false); //Modify by Jutarat A. on 07052013

                            

                            if (invoice_realTime_result != null)
                            {
                                if (string.IsNullOrEmpty(invoice_realTime_result.FilePath) == false)
                                {
                                    reportList.Add(invoice_realTime_result.FilePath);
                                }
                            }
                        }
                        else
                        {
                            //handlerBilling.ManageInvoiceByCommand(invoice, dtTbt_BillingDetail, false, true);
                            handlerBilling.ManageInvoiceByCommand(invoice, doAfterManagetbt_BillingDetailList, true, true); //Modify by Jutarat A. on 07052013
                        }


                    }

                }

                if (res.MessageList == null || res.MessageList.Count == 0)
                {
                    res.ResultData = "1";
                }
                else
                {
                    res.ResultData = "0";
                }
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
        /// Validate business in separate invoice screen mode
        /// </summary>
        /// <param name="RegisterData">input data from screen</param>
        /// <param name="doGetInvoiceWithBillingClientName">load master data from input screen</param>
        /// <param name="doGetBillingDetailOfInvoiceList">load master data from input screen</param>
        /// <returns></returns>
        public ObjectResultData BLS070_ValidateSeparateInvoice(BLS070_RegisterData RegisterData
            , doInvoice doGetInvoiceWithBillingClientName, List<doGetBillingDetailOfInvoice> doGetBillingDetailOfInvoiceList)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;


            try
            {

                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                // == Has at least 1 secleced record in detail grid
                var billingDetail_selected_list = new List<BLS070_DetailRegisterDataSection1>();
                int totalBillingDetail = 0;
                if (RegisterData.Detail1 != null)
                {
                    billingDetail_selected_list = (from p in RegisterData.Detail1
                                                   where p.chkSelectSeparateDetail == true
                                                   select p).ToList<BLS070_DetailRegisterDataSection1>();

                    totalBillingDetail = RegisterData.Detail1.Count;
                }

                if (billingDetail_selected_list.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6065,
                                        null,
                                        new string[] { "txtSelSeparateFromInvoiceNo" });

                    return res;
                }

                // == If select all not allow
                if (billingDetail_selected_list.Count == totalBillingDetail)
                {
                    res.ResultData = "Alert_SeparateAll";
                }


                // == Business validate ==

                var dtbillingDetail_not_selected = new List<doGetBillingDetailOfInvoice>();
                if (RegisterData.Detail1 != null)
                {
                    var billingDetail_not_selected_list = (from p in RegisterData.Detail1
                                                           where p.chkSelectSeparateDetail == false
                                                           select p.ContractCode + "-" + p.BillingOCC + "-" + p.BillingDetailNo).ToArray<string>();

                    dtbillingDetail_not_selected = (from p in doGetBillingDetailOfInvoiceList
                                                    where billingDetail_not_selected_list.Contains(p.ContractCode + "-" + p.BillingOCC + "-" + p.BillingDetailNo)
                                                    select p).ToList<doGetBillingDetailOfInvoice>();
                }


                string strBillingTargetCode = null;
                List<string> BankAccount_CreditCard_No_list = new List<string>();

                foreach (var item in dtbillingDetail_not_selected)
                {
                    var billingBasic = handlerBilling.GetTbt_BillingBasic(item.ContractCode, item.BillingOCC);

                    if (billingBasic.Count > 0)
                    {
                        if (strBillingTargetCode == null)
                        {
                            strBillingTargetCode = billingBasic[0].BillingTargetCode;
                        }

                        if (strBillingTargetCode != billingBasic[0].BillingTargetCode) //---------- (1)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                             MessageUtil.MessageList.MSG6079,
                             new string[] { doGetInvoiceWithBillingClientName.InvoiceNo },
                             new string[] { "txtSelSeparateFromInvoiceNo" });

                            return res;
                        }


                        if (RegisterData.Details1.cboPaymentMethodsOfSeparateFrom == UsedPaymentMethod.C_USED_PAYMENT_METHOD_AUTO_CREDIT) // -------- (2)
                        {
                            if (billingBasic[0].PaymentMethod != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER && billingBasic[0].PaymentMethod != PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                   MessageUtil.MessageList.MSG6044,
                                                   null,
                                                   new string[] { "txtSelSeparateFromInvoiceNo" });

                                return res;
                            }
                            else
                            {
                                doGetInvoiceWithBillingClientName.PaymentMethod = billingBasic[0].PaymentMethod;
                            }
                        }

                    } // end if billingBasic.Count > 0



                    // == Check these billing detail has same account no. (Bank/Credit)  ----------------- (3)

                    if (doGetInvoiceWithBillingClientName.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER || doGetInvoiceWithBillingClientName.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                    {
                        if (doGetInvoiceWithBillingClientName.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                        {
                            var bankAccount = handlerBilling.GetAutoTransferBankAccountByContract(item.ContractCode, item.BillingOCC);
                            if (bankAccount.Count > 0)
                            {
                                if (string.IsNullOrEmpty(bankAccount[0].AccountNo) == false)
                                {
                                    BankAccount_CreditCard_No_list.Add(bankAccount[0].AccountNo);
                                }
                            }
                            else
                            {
                                BankAccount_CreditCard_No_list.Add(string.Empty);
                            }

                        }
                        else
                        {
                            var creditCard = handlerBilling.GetCreditCardByContract(item.ContractCode, item.BillingOCC);
                            if (creditCard.Count > 0)
                            {
                                if (string.IsNullOrEmpty(creditCard[0].CreditCardNo) == false)
                                {
                                    BankAccount_CreditCard_No_list.Add(creditCard[0].CreditCardNo);
                                }
                            }
                            else
                            {
                                BankAccount_CreditCard_No_list.Add(string.Empty);
                            }

                        }


                        if (BankAccount_CreditCard_No_list.Distinct().ToList().Count > 1)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                MessageUtil.MessageList.MSG6050,
                                                null,
                                                new string[] { "txtSelSeparateFromInvoiceNo" });

                            return res;
                        }

                    }


                } // end loop



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
        /// Validate business in combine invoice screen mode
        /// </summary>
        /// <param name="RegisterData">input data from screen</param>
        /// <param name="doGetInvoiceWithBillingClientName">load master data from input screen</param>
        /// <param name="doGetBillingDetailOfInvoiceList">load master data from input screen</param>
        /// <returns></returns>
        public ObjectResultData BLS070_ValidateCombineInvoice(BLS070_RegisterData RegisterData
            , doInvoice doGetInvoiceWithBillingClientName, List<doGetBillingDetailOfInvoice> doGetBillingDetailOfInvoiceList)
        {


            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;


            try
            {

                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                BLS070_ScreenParameter param = GetScreenObject<BLS070_ScreenParameter>();


                // == Has at least 1 conbine billing detail ==
                if (RegisterData.NewBillingDetail == null || RegisterData.NewBillingDetail.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                   MessageUtil.MessageList.MSG6067,
                                                   null,
                                                   new string[] { "txtSelCombineToInvoiceNo" });

                    return res;
                }


                // === Merge Old and New billing detail together ===
                if (param.ConbineBillingDetail == null)
                {
                    param.ConbineBillingDetail = new List<doGetBillingDetailOfInvoice>();
                }

                param.ConbineBillingDetail.Clear();
                param.ConbineBillingDetail.AddRange(doGetBillingDetailOfInvoiceList);

                foreach (var item in RegisterData.NewBillingDetail)
                {
                    var billingDetail = handlerBilling.GetBillingDetailByKey(item.ContractCode, item.BillingOCC, item.BillingDetailNo, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                    param.ConbineBillingDetail.AddRange(billingDetail);
                }


                // == Business validate ==

                string strBillingTargetCode = null;
                List<string> BankAccount_CreditCard_No_list = new List<string>();
                foreach (var item in param.ConbineBillingDetail)
                {

                    var billingBasic = handlerBilling.GetTbt_BillingBasic(item.ContractCode, item.BillingOCC);

                    if (billingBasic.Count > 0)
                    {
                        if (strBillingTargetCode == null)
                        {
                            strBillingTargetCode = billingBasic[0].BillingTargetCode;
                        }

                        if (strBillingTargetCode != billingBasic[0].BillingTargetCode) //---------- (1)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6080,
                                                             new string[] { item.InvoiceNo },
                                                             new string[] { "txtSelCombineToInvoiceNo" });

                            return res;
                        }

                        if (doGetInvoiceWithBillingClientName.BillingTypeCode != item.BillingTypeCode) //---------- (1)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                             MessageUtil.MessageList.MSG6068,
                                                             null,
                                                             null);

                            return res;
                        }

                        if (RegisterData.Details2.cboPaymentMethodsOfCombineToInvoice == UsedPaymentMethod.C_USED_PAYMENT_METHOD_AUTO_CREDIT) // -------- (2)
                        {
                            if (billingBasic[0].PaymentMethod != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER && billingBasic[0].PaymentMethod != PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                    MessageUtil.MessageList.MSG6044,
                                                    null,
                                                    new string[] { "txtSelCombineToInvoiceNo" });

                                return res;
                            }
                            else
                            {
                                doGetInvoiceWithBillingClientName.PaymentMethod = billingBasic[0].PaymentMethod;
                            }

                        }


                    } // end if billingBasic.Count > 0


                    // == Check these billing detail has same account no. (Bank/Credit)  ----------------- (3)

                    if (doGetInvoiceWithBillingClientName.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER || doGetInvoiceWithBillingClientName.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                    {
                        if (doGetInvoiceWithBillingClientName.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                        {
                            var bankAccount = handlerBilling.GetAutoTransferBankAccountByContract(item.ContractCode, item.BillingOCC);
                            if (bankAccount.Count > 0)
                            {
                                if (string.IsNullOrEmpty(bankAccount[0].AccountNo) == false)
                                {
                                    BankAccount_CreditCard_No_list.Add(bankAccount[0].AccountNo);
                                }
                            }
                            else
                            {
                                BankAccount_CreditCard_No_list.Add(string.Empty);
                            }
                        }
                        else
                        {
                            var creditCard = handlerBilling.GetCreditCardByContract(item.ContractCode, item.BillingOCC);
                            if (creditCard.Count > 0)
                            {
                                if (string.IsNullOrEmpty(creditCard[0].CreditCardNo) == false)
                                {
                                    BankAccount_CreditCard_No_list.Add(creditCard[0].CreditCardNo);
                                }
                            }
                            else
                            {
                                BankAccount_CreditCard_No_list.Add(string.Empty);
                            }

                        }


                        if (BankAccount_CreditCard_No_list.Distinct().ToList().Count > 1)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                                MessageUtil.MessageList.MSG6050,
                                                null,
                                                new string[] { "txtSelCombineToInvoiceNo" });

                            return res;
                        }

                    }


                } // end foreach


                if (res.MessageList == null || res.MessageList.Count == 0)
                {
                    res.ResultData = "1";
                }
                else
                {
                    res.ResultData = "0";
                }
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
        /// Validate business in issue sale invoice screen mode
        /// </summary>
        /// <param name="RegisterData">input data from screen</param>
        /// <param name="doGetInvoiceWithBillingClientName">load master data from input screen</param>
        /// <param name="doGetBillingDetailOfInvoiceList">load master data from input screen</param>
        /// <returns></returns>
        public ObjectResultData BLS070_ValidateIssueSaleInvoice(BLS070_RegisterData RegisterData
            , doInvoice doGetInvoiceWithBillingClientName, List<doGetBillingDetailOfInvoice> doGetBillingDetailOfInvoiceList)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil cm = new CommonUtil();

            try
            {

                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ISaleContractHandler handlerSaleContract = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                BLS070_ScreenParameter param = GetScreenObject<BLS070_ScreenParameter>();

                // == Validate required fied
                if (RegisterData.Details3.dtpCustomerAcceptanceDate.HasValue == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0007,
                                         new string[] { "dtpCustomerAcceptanceDate" },
                                         new string[] { "dtpCustomerAcceptanceDate" });
                    return res;
                }

                //Add by Jutarat A. on 14062013
                DateTime dtEndDateOfMonth = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddMonths(1).AddDays(-1);
                if (RegisterData.Details3.dtpCustomerAcceptanceDate.Value > dtEndDateOfMonth)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                         "BLS070",
                                         MessageUtil.MODULE_COMMON,
                                         MessageUtil.MessageList.MSG0009,
                                         new string[] { "dtpCustomerAcceptanceDate" },
                                         new string[] { "dtpCustomerAcceptanceDate" });
                    return res;
                }
                //End Add

                // == Business validate ==
                string contrctCode_long = cm.ConvertContractCode(RegisterData.Header.txtSelContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                var dtSaleDataForIssueInvoice = handlerSaleContract.GetSaleDataForIssueInvoice(contrctCode_long, RegisterData.Header.txtSelSaleOCC);


                if (dtSaleDataForIssueInvoice.Count == 0)
                {
                    //MSG6071
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6071,
                                        null,
                                        new string[] { "txtSelContractCode", "txtSelSaleOCC" });

                    return res;
                }
                else
                {
                    if (dtSaleDataForIssueInvoice[0].SaleProcessManageStatus != SaleProcessManageStatus.C_SALE_PROCESS_STATUS_SHIP
                       && dtSaleDataForIssueInvoice[0].SaleProcessManageStatus != SaleProcessManageStatus.C_SALE_PROCESS_STATUS_COMPLETE_NOTACCEPT) // ------------ (1)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING,
                                        MessageUtil.MessageList.MSG6064,
                                        null,
                                        new string[] { "txtSelContractCode", "txtSelSaleOCC" });


                        return res;
                    }
                }


                // === Partial invoice , do you want to issue also ? ==
                var dtBillingDetailPartialFee = handlerBilling.GetBillingDetailPartialFee(contrctCode_long, dtSaleDataForIssueInvoice[0].BillingOCC);  // ----------- (2)

                if (dtBillingDetailPartialFee.Count > 0)
                {
                    // Keep billing detail (Partial Fee data)
                    param.doGetBillingDetailPartialFee = dtBillingDetailPartialFee;

                    if (dtBillingDetailPartialFee[0].PaymentStatus == PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT ||
                        dtBillingDetailPartialFee[0].PaymentStatus == PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT)
                    {
                        RegisterData.bAlertConfirmDialog = true;
                    }
                }


                if (res.MessageList == null || res.MessageList.Count == 0)
                {
                    res.ResultData = "1";
                }
                else
                {
                    res.ResultData = "0";
                }
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
        public ActionResult BLS070_Confirm()
        {
            string mode_SeparateInvoice = "1";
            string mode_CombineInvoice = "2";
            string mode_IssueSaleInvoice = "3";


            BLS070_ScreenParameter param = GetScreenObject<BLS070_ScreenParameter>();
            BLS070_RegisterData RegisterData = new BLS070_RegisterData();
            CommonUtil cm = new CommonUtil();

            // reuse param that send on Register Click
            if (param != null)
            {
                RegisterData = param.RegisterData;
            }

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ObjectResultData res_SeparateInvoice = new ObjectResultData();
            ObjectResultData res_CombineInvoice = new ObjectResultData();
            ObjectResultData res_IssueInvoice = new ObjectResultData();

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


                // === SeparateInvoice mode ==
                if (RegisterData.Header.rdoProcessSelect == mode_SeparateInvoice)
                {

                    res_SeparateInvoice = BLS070_SeparateInvoice(RegisterData, param.doGetInvoiceWithBillingClientName, param.doGetBillingDetailOfInvoiceList);

                    if (res_SeparateInvoice.MessageList != null) // error !
                    {
                        if (res_SeparateInvoice.MessageList.Count > 0)
                        {
                            return Json(res_SeparateInvoice);
                        }
                    }
                }

                // === CombineInvoice mode ==
                if (RegisterData.Header.rdoProcessSelect == mode_CombineInvoice)
                {
                    res_CombineInvoice = BLS070_CombineInvoice(RegisterData, param.doGetInvoiceWithBillingClientName, param.ConbineBillingDetail);

                    if (res_CombineInvoice.MessageList != null)  // error !
                    {
                        if (res_CombineInvoice.MessageList.Count > 0)
                        {
                            return Json(res_CombineInvoice);
                        }
                    }
                }

                // === IssueSaleInvoice mode ===
                if (RegisterData.Header.rdoProcessSelect == mode_IssueSaleInvoice)
                {
                    res_IssueInvoice = BLS070_IssueSaleInvoice(RegisterData
                                                        , param.doGetInvoiceWithBillingClientName
                                                        , param.doGetSaleDataForIssueInvoice
                                                        , param.doGetBillingDetailPartialFee
                                                        , param.bIssuePartialFee
                                                        );


                    if (res_IssueInvoice.MessageList != null) // error !
                    {
                        if (res_IssueInvoice.MessageList.Count > 0)
                        {
                            return Json(res_IssueInvoice);
                        }
                    }
                }

                // return "1" to js is every thing OK
                if (res.MessageList == null || res.MessageList.Count == 0)
                {
                    var result = new
                    {
                        strFilePath = param.RegisterData.strFilePath,
                        isIssue = (string.IsNullOrEmpty(param.RegisterData.strFilePath) == false)
                    };

                    res.ResultData = result;
                }
                else
                {
                    res.ResultData = "0";
                }

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
        /// register data into database mode separate invoice
        /// </summary>
        /// <param name="RegisterData">confirm register data</param>
        /// <param name="doGetInvoiceWithBillingClientName">doGetInvoiceWithBillingClientName register data</param>
        /// <param name="doGetBillingDetailOfInvoiceList">doGetBillingDetailOfInvoiceList register data</param>
        /// <returns></returns>
        public ObjectResultData BLS070_SeparateInvoice(BLS070_RegisterData RegisterData
             , doInvoice doGetInvoiceWithBillingClientName, List<doGetBillingDetailOfInvoice> doGetBillingDetailOfInvoiceList)
        {

            ObjectResultData res = new ObjectResultData();

            string txtBillingCode = "";
            string txtRunningNo = "";
            string txtBillingType = "";
            decimal? decBillingAmount = null;
            string txtSiteName = "";
            string txtIssueInvoiceofSeparateDetail = "";

             using (TransactionScope scope = new TransactionScope())
             {

            try
            {

                    List<doMiscTypeCode>  currencyDatas = new List<doMiscTypeCode>();
                    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                    {
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_CURRENCT,
                            ValueCode = "%"
                        }
                    };

                    ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    currencyDatas = hand.GetMiscTypeCodeList(miscs).ToList();

                    IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                    ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                    BLS070_ScreenParameter param = GetScreenObject<BLS070_ScreenParameter>();
                    List<string> MergeFileList = new List<string>();

                    string strIssueInvoice = string.Empty;

                    for (int i = 0; i < RegisterData.Detail1.Count; i++)
                    {
                        txtBillingCode = RegisterData.Detail1[i].BillingCode;
                        txtRunningNo = RegisterData.Detail1[i].RunningNo;
                        txtBillingType = RegisterData.Detail1[i].BillingType;
                        decBillingAmount = Convert.ToDecimal(RegisterData.Detail1[i].BillingAmount);
                        txtSiteName = RegisterData.Detail1[i].SiteName;
                        txtIssueInvoiceofSeparateDetail = RegisterData.Detail1[i].cboIssueInvoiceofSeparateDetail;

                        // Check
                        if (RegisterData.Detail1[i].chkSelectSeparateDetail)
                        {
                            // Cancel and Re-create billing details

                            // == Prepare for Cancel ==
                            var dtTbt_BillingDetail = new tbt_BillingDetail()
                            {
                                ContractCode = doGetBillingDetailOfInvoiceList[i].ContractCode,
                                BillingOCC = doGetBillingDetailOfInvoiceList[i].BillingOCC,
                                BillingDetailNo = doGetBillingDetailOfInvoiceList[i].BillingDetailNo,
                                InvoiceNo = doGetBillingDetailOfInvoiceList[i].InvoiceNo,
                                InvoiceOCC = doGetBillingDetailOfInvoiceList[i].InvoiceOCC,
                                IssueInvDate = doGetBillingDetailOfInvoiceList[i].IssueInvDate,
                                IssueInvFlag = doGetBillingDetailOfInvoiceList[i].IssueInvFlag,
                                BillingTypeCode = doGetBillingDetailOfInvoiceList[i].BillingTypeCode,
                                BillingAmount = doGetBillingDetailOfInvoiceList[i].BillingAmount,
                                AdjustBillingAmount = doGetBillingDetailOfInvoiceList[i].AdjustBillingAmount,
                                AdjustBillingAmountUsd = doGetBillingDetailOfInvoiceList[i].AdjustBillingAmountUsd,
                                AdjustBillingAmountCurrencyType = doGetBillingDetailOfInvoiceList[i].AdjustBillingAmountCurrencyType,
                                BillingStartDate = doGetBillingDetailOfInvoiceList[i].BillingStartDate,
                                BillingEndDate = doGetBillingDetailOfInvoiceList[i].BillingEndDate,
                                PaymentMethod = doGetBillingDetailOfInvoiceList[i].PaymentMethod,
                                PaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CANCEL,
                                AutoTransferDate = doGetBillingDetailOfInvoiceList[i].AutoTransferDate,
                                FirstFeeFlag = doGetBillingDetailOfInvoiceList[i].FirstFeeFlag,
                                DelayedMonth = doGetBillingDetailOfInvoiceList[i].DelayedMonth,
                                ContractOCC = doGetBillingDetailOfInvoiceList[i].ContractOCC,
                                ForceIssueFlag = doGetBillingDetailOfInvoiceList[i].ForceIssueFlag,
                                BillingAmountCurrencyType = doGetBillingDetailOfInvoiceList[i].BillingAmountCurrencyType,
                                BillingAmountUsd = doGetBillingDetailOfInvoiceList[i].BillingAmountUsd
                            };

                            // UPDATE !!
                            handlerBilling.Updatetbt_BillingDetail(dtTbt_BillingDetail);


                            // Prepare for re-create billing detail 
                            dtTbt_BillingDetail = new tbt_BillingDetail()
                            {
                                ContractCode = doGetBillingDetailOfInvoiceList[i].ContractCode,
                                BillingOCC = doGetBillingDetailOfInvoiceList[i].BillingOCC,
                                BillingDetailNo = 0,
                                InvoiceNo = null,
                                InvoiceOCC = null,
                                IssueInvDate = System.DateTime.Now,
                                IssueInvFlag = doGetBillingDetailOfInvoiceList[i].IssueInvFlag,
                                BillingTypeCode = doGetBillingDetailOfInvoiceList[i].BillingTypeCode,
                                BillingTypeGroup = null,
                                BillingAmount = doGetBillingDetailOfInvoiceList[i].BillingAmount,
                                AdjustBillingAmount = doGetBillingDetailOfInvoiceList[i].AdjustBillingAmount,
                                AdjustBillingAmountUsd = doGetBillingDetailOfInvoiceList[i].AdjustBillingAmountUsd,
                                AdjustBillingAmountCurrencyType = doGetBillingDetailOfInvoiceList[i].AdjustBillingAmountCurrencyType,
                                BillingStartDate = doGetBillingDetailOfInvoiceList[i].BillingStartDate,
                                BillingEndDate = doGetBillingDetailOfInvoiceList[i].BillingEndDate,
                                PaymentMethod = doGetBillingDetailOfInvoiceList[i].PaymentMethod,
                                PaymentStatus = (doGetBillingDetailOfInvoiceList[i].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                                || doGetBillingDetailOfInvoiceList[i].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                                ? PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT : PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT,

                                AutoTransferDate = doGetBillingDetailOfInvoiceList[i].AutoTransferDate,
                                FirstFeeFlag = doGetBillingDetailOfInvoiceList[i].FirstFeeFlag,
                                DelayedMonth = doGetBillingDetailOfInvoiceList[i].DelayedMonth,
                                StartOperationDate = null,
                                ContractOCC = doGetBillingDetailOfInvoiceList[i].ContractOCC,
                                ForceIssueFlag = doGetBillingDetailOfInvoiceList[i].ForceIssueFlag,

                                BillingAmountCurrencyType = doGetBillingDetailOfInvoiceList[i].BillingAmountCurrencyType,
                                BillingAmountUsd = doGetBillingDetailOfInvoiceList[i].BillingAmountUsd
                            };

                            // ManageBillingDetail !!
                            var billingDetail_result = handlerBilling.ManageBillingDetail(dtTbt_BillingDetail);
                            List<tbt_BillingDetail> billingDetail_result_list = new List<tbt_BillingDetail>();
                            billingDetail_result_list.Add(billingDetail_result);


                            // Keep new BillingDetailNo
                            if (billingDetail_result != null)
                            {
                                doGetBillingDetailOfInvoiceList[i].BillingDetailNo = billingDetail_result.BillingDetailNo;
                            }

                            // ===== Create New Invoice =====

                            // get billing target code
                            var billingBasic = handlerBilling.GetTbt_BillingBasic(doGetBillingDetailOfInvoiceList[i].ContractCode, doGetBillingDetailOfInvoiceList[i].BillingOCC);

                            // === Case: Not issue ==
                            if (RegisterData.Detail1[i].cboIssueInvoiceofSeparateDetail == IssueInv.C_ISSUE_INV_NOT_ISSUE)
                            {
                                // Prepare 
                                tbt_Invoice invoice_notIssue = new tbt_Invoice()
                                {
                                    InvoiceNo = null,
                                    InvoiceOCC = 0,
                                    IssueInvDate = System.DateTime.Now,
                                    AutoTransferDate = doGetInvoiceWithBillingClientName.AutoTransferDate,
                                    BillingTargetCode = billingBasic[0].BillingTargetCode, //--- *
                                    BillingTypeCode = doGetInvoiceWithBillingClientName.BillingTypeCode,
                                    InvoiceAmount = null,
                                    PaidAmountIncVat = null,
                                    VatRate = null,
                                    VatAmount = null,
                                    WHTRate = null,
                                    WHTAmount = null,
                                    RegisteredWHTAmount = null,
                                    InvoicePaymentStatus = (doGetBillingDetailOfInvoiceList[i].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                                            || doGetBillingDetailOfInvoiceList[i].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                                            ? PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT : PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,

                                    IssueInvFlag = false, // ---- **
                                    FirstIssueInvDate = null,
                                    FirstIssueInvFlag = null,
                                    PaymentMethod = doGetBillingDetailOfInvoiceList[i].PaymentMethod,
                                    CorrectReason = null,
                                    RefOldInvoiceNo = doGetInvoiceWithBillingClientName.InvoiceNo,

                                    VatAmountUsd = null,
                                    VatAmountCurrencyType = null,
                                    WHTAmountUsd = null,
                                    WHTAmountCurrencyType = null,
                                    InvoiceAmountUsd = null,
                                    InvoiceAmountCurrencyType = null
                                };

                                // ManageInvoiceByCommand !!
                                handlerBilling.ManageInvoiceByCommand(invoice_notIssue
                                                                    , billingDetail_result_list
                                                                    , true
                                                                    , true); // isEncrypt = true ==> not issue

                            }
                            else if (RegisterData.Detail1[i].cboIssueInvoiceofSeparateDetail == IssueInv.C_ISSUE_INV_REALTIME) // === Case: Real time ==
                            {
                                // Prepare 
                                tbt_Invoice invoice_realTime = new tbt_Invoice()
                                {
                                    InvoiceNo = null,
                                    InvoiceOCC = 0,
                                    IssueInvDate = System.DateTime.Now,
                                    AutoTransferDate = doGetInvoiceWithBillingClientName.AutoTransferDate,
                                    BillingTargetCode = billingBasic[0].BillingTargetCode, //--- *
                                    BillingTypeCode = doGetInvoiceWithBillingClientName.BillingTypeCode,
                                    InvoiceAmount = null,
                                    PaidAmountIncVat = null,
                                    VatRate = null,
                                    VatAmount = null,
                                    WHTRate = null,
                                    WHTAmount = null,
                                    RegisteredWHTAmount = null,
                                    InvoicePaymentStatus = (doGetBillingDetailOfInvoiceList[i].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                                            || doGetBillingDetailOfInvoiceList[i].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                                            ? PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT : PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                                    IssueInvFlag = true, // ---- **
                                    FirstIssueInvDate = System.DateTime.Now, // ---- **
                                    FirstIssueInvFlag = true, // ---- **
                                    PaymentMethod = doGetBillingDetailOfInvoiceList[i].PaymentMethod,
                                    CorrectReason = null,
                                    RefOldInvoiceNo = doGetInvoiceWithBillingClientName.InvoiceNo,

                                    VatAmountUsd = null,
                                    VatAmountCurrencyType = null,
                                    WHTAmountUsd = null,
                                    WHTAmountCurrencyType = null,
                                    InvoiceAmountUsd = null,
                                    InvoiceAmountCurrencyType = null

                                };

                                // ManageInvoiceByCommand !! + Report output
                                var invoice_realTime_result = handlerBilling.ManageInvoiceByCommand(
                                                                    invoice_realTime
                                                                    , billingDetail_result_list
                                                                    , true
                                                                    , false); // isEncrypt = false ==> can merge report ==> issue to user

                                //  === Keep output report for merge ===
                                if (invoice_realTime_result != null)
                                {
                                    if (string.IsNullOrEmpty(invoice_realTime_result.FilePath) == false)
                                    {
                                        MergeFileList.Add(invoice_realTime_result.FilePath);
                                    }
                                }
                            }
                        } // end is checked
                    } // end loop


                    // === Unpaid (group of not check in grid) === 

                    var invoice_unpaid = handlerBilling.GetUnpaidInvoiceDataList(doGetInvoiceWithBillingClientName.InvoiceNo);

                    List<doGetBillingDetailOfInvoice> billingDetail_unpaid = new List<doGetBillingDetailOfInvoice>();
                    if (invoice_unpaid != null && invoice_unpaid.Count > 0)
                    {
                        billingDetail_unpaid = handlerBilling.GetBillingDetailOfInvoiceList(invoice_unpaid[0].InvoiceNo, invoice_unpaid[0].InvoiceOCC);
                    }

                    List<tbt_BillingDetail> bilingDetail_unpaid = new List<tbt_BillingDetail>();

                    tbt_BillingDetail billingDetail = new tbt_BillingDetail();

                    if (billingDetail_unpaid != null && billingDetail_unpaid.Count > 0)
                    {
                        foreach (doGetBillingDetailOfInvoice item in billingDetail_unpaid)
                        {

                            billingDetail = new tbt_BillingDetail()
                            {
                                ContractCode = item.ContractCode,
                                BillingOCC = item.BillingOCC,
                                BillingDetailNo = item.BillingDetailNo,
                                InvoiceNo = item.InvoiceNo,
                                InvoiceOCC = item.InvoiceOCC,
                                IssueInvDate = item.IssueInvDate,
                                IssueInvFlag = item.IssueInvFlag,
                                BillingTypeCode = item.BillingTypeCode,
                                BillingAmount = item.BillingAmount,
                                AdjustBillingAmount = item.AdjustBillingAmount,
                                AdjustBillingAmountUsd = item.AdjustBillingAmountUsd,
                                AdjustBillingAmountCurrencyType = item.AdjustBillingAmountCurrencyType,
                                BillingStartDate = item.BillingStartDate,
                                BillingEndDate = item.BillingEndDate,
                                PaymentMethod = item.PaymentMethod,
                                PaymentStatus = item.PaymentStatus,
                                AutoTransferDate = item.AutoTransferDate,
                                FirstFeeFlag = item.FirstFeeFlag,
                                DelayedMonth = item.DelayedMonth,
                                ContractOCC = item.ContractOCC,
                                ForceIssueFlag = item.ForceIssueFlag,

                                BillingAmountUsd = item.BillingAmountUsd,
                                BillingAmountCurrencyType = item.BillingAmountCurrencyType
                            };


                            bilingDetail_unpaid.Add(billingDetail);

                        } // end foreach
                    }

                    //string strPaymentMethod = string.Empty;
                    //if (RegisterData.Details1.cboPaymentMethodsOfSeparateFrom == UsedPaymentMethod.C_USED_PAYMENT_METHOD_CURRENT)
                    //{
                    //    strPaymentMethod = doGetInvoiceWithBillingClientName.PaymentMethod;
                    //}
                    //else
                    //{
                    //    strPaymentMethod = PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT;
                    //}

                    res = BLS070_CancelAndCreateInvoice(doGetInvoiceWithBillingClientName
                                                    , bilingDetail_unpaid
                                                    , RegisterData.Details1.chkSepNotChangeInvoiceNo
                                                    , doGetInvoiceWithBillingClientName.PaymentMethod
                                                    , RegisterData.Details1.cboIssueInvoiceAfterSeparate
                                                    , MergeFileList);

                    if (res.IsError)
                    {
                    // Rollback transaction..
                      scope.Dispose(); 

                    res.ResultData = "0";
                        return res;
                    }
                    else
                    {
                        string mergeOutputFilename = string.Empty;
                        string encryptOutputFileName = string.Empty;

                        mergeOutputFilename = PathUtil.GetTempFileName(".pdf");
                        encryptOutputFileName = PathUtil.GetTempFileName(".pdf");

                        if (MergeFileList.Count > 0)
                        {
                            if (MergeFileList.Count == 1)
                            {
                                // encrypt
                                var isSuccess = ReportUtil.EncryptPDF(MergeFileList[0], encryptOutputFileName, null);
                            }
                            else
                            {
                                // merge + encrypt
                                var isSuccess = ReportUtil.MergePDF(MergeFileList.ToArray(), mergeOutputFilename, true, encryptOutputFileName, null);
                            }

                            // keep
                            RegisterData.strFilePath = encryptOutputFileName;
                        }


                        param.RegisterData = RegisterData;

                    // COMMIT TRANSACTION !!!
                    scope.Complete(); 
                }


                if (res.MessageList == null || res.MessageList.Count == 0)
                    {
                        res.ResultData = "1";
                    }
                    else
                    {
                        res.ResultData = "0";
                    }

                }
                catch (Exception ex)
                {
                // Rollback transaction ...
                scope.Dispose(); 

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.ResultData = "0";
                    res.AddErrorMessage(ex);
                }

             } 

            return res;

        }

        /// <summary>
        /// register data into database mode combine invoice
        /// </summary>
        /// <param name="RegisterData">confirm register data</param>
        /// <param name="doGetInvoiceWithBillingClientName">doGetInvoiceWithBillingClientName register data</param>
        /// <param name="doGetBillingDetailOfInvoiceList">doGetBillingDetailOfInvoiceList register data</param>
        /// <returns></returns>
        public ObjectResultData BLS070_CombineInvoice(BLS070_RegisterData RegisterData
            , doInvoice doGetInvoiceWithBillingClientName, List<doGetBillingDetailOfInvoice> doGetBillingDetailOfInvoiceList)
        {

            ObjectResultData res = new ObjectResultData();
            BLS070_ScreenParameter param = GetScreenObject<BLS070_ScreenParameter>();

              using (TransactionScope scope = new TransactionScope())
            {

            try
            {

                    List<string> MergeFileList = new List<string>();

                    //string strPaymentMethod = string.Empty;
                    //if (RegisterData.Details2.cboPaymentMethodsOfCombineToInvoice == UsedPaymentMethod.C_USED_PAYMENT_METHOD_CURRENT)
                    //{
                    //    strPaymentMethod = doGetInvoiceWithBillingClientName.PaymentMethod;
                    //}
                    //else
                    //{
                    //    strPaymentMethod = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;
                    //}

                    string strIssueInvoice = RegisterData.Details2.cboIssueInvoiceAfterCombine;

                    List<tbt_BillingDetail> billingDetail_list = new List<tbt_BillingDetail>();
                    tbt_BillingDetail billingDetail = new tbt_BillingDetail();
                    foreach (doGetBillingDetailOfInvoice item in doGetBillingDetailOfInvoiceList)
                    {
                        billingDetail = new tbt_BillingDetail()
                        {
                            ContractCode = item.ContractCode,
                            BillingOCC = item.BillingOCC,
                            BillingDetailNo = item.BillingDetailNo,
                            InvoiceNo = item.InvoiceNo,
                            InvoiceOCC = item.InvoiceOCC,
                            IssueInvDate = item.IssueInvDate,
                            IssueInvFlag = item.IssueInvFlag,
                            BillingTypeCode = item.BillingTypeCode,
                            BillingAmount = item.BillingAmount,
                            AdjustBillingAmount = item.AdjustBillingAmount,
                            AdjustBillingAmountUsd = item.AdjustBillingAmountUsd,
                            AdjustBillingAmountCurrencyType = item.AdjustBillingAmountCurrencyType,
                            BillingStartDate = item.BillingStartDate,
                            BillingEndDate = item.BillingEndDate,
                            PaymentMethod = doGetInvoiceWithBillingClientName.PaymentMethod,
                            PaymentStatus = item.PaymentStatus,
                            AutoTransferDate = item.AutoTransferDate,
                            FirstFeeFlag = item.FirstFeeFlag,
                            DelayedMonth = item.DelayedMonth,
                            ContractOCC = item.ContractOCC,
                            ForceIssueFlag = item.ForceIssueFlag,

                            BillingAmountUsd = item.BillingAmountUsd,
                            BillingAmountCurrencyType = item.BillingAmountCurrencyType
                        };


                        billingDetail_list.Add(billingDetail);
                    }


                    res = BLS070_CancelAndCreateInvoice(doGetInvoiceWithBillingClientName
                                                , billingDetail_list
                                                , RegisterData.Details2.chkComNotChangeInvoiceNo
                                                , doGetInvoiceWithBillingClientName.PaymentMethod
                                                , strIssueInvoice
                                                , MergeFileList); // MergeFileList for keep output report

                    if (res.IsError)
                    {
                    // Rollback transaction..
                     scope.Dispose();

                    res.ResultData = "0";
                        return res;
                    }
                    else
                    {
                        string mergeOutputFilename = string.Empty;
                        string encryptOutputFileName = string.Empty;

                        mergeOutputFilename = PathUtil.GetTempFileName(".pdf");
                        encryptOutputFileName = PathUtil.GetTempFileName(".pdf");

                        if (MergeFileList.Count > 0)
                        {
                            if (MergeFileList.Count == 1)
                            {
                                // encrypt
                                var isSuccess = ReportUtil.EncryptPDF(MergeFileList[0], encryptOutputFileName, null);
                            }
                            else
                            {
                                // merge + encrype
                                var isSuccess = ReportUtil.MergePDF(MergeFileList.ToArray(), mergeOutputFilename, true, encryptOutputFileName, null);
                            }

                            // keep
                            RegisterData.strFilePath = encryptOutputFileName;
                        }

                        // keep
                        param.RegisterData = RegisterData;

                    // COMMIT TRANSACTION !!!
                      scope.Complete();
                }


                if (res.MessageList == null || res.MessageList.Count == 0)
                    {
                        res.ResultData = "1";
                    }
                    else
                    {
                        res.ResultData = "0";
                    }
                }
                catch (Exception ex)
                {
                // Rollback transaction ...
                   scope.Dispose();

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.ResultData = "0";
                    res.AddErrorMessage(ex);
                }
              }

            return res;

        }

        /// <summary>
        /// register data into database mode issue sale invoice
        /// </summary>
        /// <param name="RegisterData"></param>
        /// <param name="doGetInvoiceWithBillingClientName"></param>
        /// <param name="doGetBillingDetailOfInvoiceList"></param>
        /// <param name="doGetBillingDetailPartialFee"></param>
        /// <param name="bIssuePartialFee"></param>
        /// <returns></returns>
        public ObjectResultData BLS070_IssueSaleInvoice(BLS070_RegisterData RegisterData
                    , doInvoice doGetInvoiceWithBillingClientName
                    , List<doGetSaleDataForIssueInvoice> doGetBillingDetailOfInvoiceList
                    , List<tbt_BillingDetail> doGetBillingDetailPartialFee
                    , bool bIssuePartialFee)
        {


            ObjectResultData res = new ObjectResultData();
            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

             using (TransactionScope scope = new TransactionScope())
           {

            try
            {
                List<string> MergeFileList = new List<string>();


                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ISaleContractHandler handlerSaleContract = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                BLS070_ScreenParameter param = GetScreenObject<BLS070_ScreenParameter>();

                DateTime? dtNextAutoTransferDate = null;

                string strPaymentMethod = string.Empty;
                string strIssueInvoice = string.Empty;
                string strSaleOCC = null;

                if (doGetBillingDetailOfInvoiceList != null)
                {
                    // UpdateCustomerAcceptance
                    //Merge at 14032017 By Pachara S.
                    handlerSaleContract.UpdateCustomerAcceptance(doGetBillingDetailOfInvoiceList[0].ContractCode,
                                                                 doGetBillingDetailOfInvoiceList[0].OCC,
                                                                 RegisterData.Details3.dtpCustomerAcceptanceDate);

                    strSaleOCC = doGetBillingDetailOfInvoiceList[0].OCC;
                }

                if (doGetBillingDetailOfInvoiceList != null)
                {
                    //Merge at 14032017 By Pachara S.
                    foreach (var item in doGetBillingDetailOfInvoiceList)
                    {
                        if (item.BillingAmt.GetValueOrDefault(0) > 0 || item.BillingAmtUsd.GetValueOrDefault(0) > 0)
                        {
                            // === Create billing type detail ===

                            // Prepare
                            tbt_BillingTypeDetail dtTbt_BillingTypeDetail = new tbt_BillingTypeDetail()
                            {
                                ContractCode = item.ContractCode,
                                BillingOCC = item.BillingOCC,
                                BillingTypeCode = BillingType.C_BILLING_TYPE_SALE_PRICE,
                                InvoiceDescriptionEN = null,
                                InvoiceDescriptionLC = null,
                                IssueInvoiceFlag = true,
                                ProductCode = item.ProductCode,
                                ProductTypeCode = item.ProductTypeCode,
                                BillingTypeGroup = BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE
                            };

                            // Create !!
                            handlerBilling.CreateBillingTypeDetail(dtTbt_BillingTypeDetail);


                            // == Issue Sale Invoice ==
                            if (item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                || item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                            {
                                dtNextAutoTransferDate = handlerBilling.GetNextAutoTransferDate(item.ContractCode, item.BillingOCC, item.PayMethod);
                            }

                            if (dtNextAutoTransferDate.HasValue == false)
                            {
                                item.PayMethod = PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER;
                            }

                            // Prepare billing detail
                            List<tbt_BillingDetail> billingDetail_result_list = new List<tbt_BillingDetail>();
                            if (item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE) //Type 13
                            {
                                tbt_BillingDetail dtTbt_BillingDetail = new tbt_BillingDetail()
                                {
                                    ContractCode = item.ContractCode,
                                    BillingOCC = item.BillingOCC,
                                    BillingDetailNo = 0,
                                    InvoiceNo = null,
                                    InvoiceOCC = null,
                                    IssueInvDate = RegisterData.Details3.dtpCustomerAcceptanceDate,
                                    IssueInvFlag = true,
                                    BillingTypeCode = BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE, // 45
                                    BillingTypeGroup = BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE,
                                    BillingAmount = item.BillingAmt,
                                    AdjustBillingAmount = null,
                                    AdjustBillingAmountUsd = null,
                                    AdjustBillingAmountCurrencyType = null,
                                    BillingStartDate = RegisterData.Details3.dtpCustomerAcceptanceDate,
                                    BillingEndDate = null,
                                    PaymentMethod = item.PayMethod,
                                    PaymentStatus = (item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                            || item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER) ? PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT : PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT,
                                    AutoTransferDate = dtNextAutoTransferDate,
                                    FirstFeeFlag = strSaleOCC == OCCType.C_FIRST_SALE_CONTRACT_OCC ? true : false,
                                    DelayedMonth = null,
                                    StartOperationDate = null,
                                    ContractOCC = item.OCC,
                                    ForceIssueFlag = false,

                                    BillingAmountUsd = item.BillingAmtUsd,
                                    BillingAmountCurrencyType = item.BillingAmtCurrencyType
                                };
                                var billingDetail_result = handlerBilling.ManageBillingDetail(dtTbt_BillingDetail);
                                billingDetail_result_list.Add(billingDetail_result);
                            }

                            if (item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE) //Type 06
                            {
                                tbt_BillingDetail dtTbt_BillingDetail = new tbt_BillingDetail()
                                {
                                    ContractCode = item.ContractCode,
                                    BillingOCC = item.BillingOCC,
                                    BillingDetailNo = 0,
                                    InvoiceNo = null,
                                    InvoiceOCC = null,
                                    IssueInvDate = RegisterData.Details3.dtpCustomerAcceptanceDate,
                                    IssueInvFlag = true,
                                    BillingTypeCode = BillingType.C_BILLING_TYPE_INSTALL_SALE, // 61
                                    BillingTypeGroup = BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE,
                                    BillingAmount = item.BillingAmt,
                                    AdjustBillingAmount = null,
                                    AdjustBillingAmountUsd = null,
                                    AdjustBillingAmountCurrencyType = null,
                                    BillingStartDate = RegisterData.Details3.dtpCustomerAcceptanceDate,
                                    BillingEndDate = null,
                                    PaymentMethod = item.PayMethod,
                                    PaymentStatus = (item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                            || item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER) ? PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT : PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT,
                                    AutoTransferDate = dtNextAutoTransferDate,
                                    FirstFeeFlag = strSaleOCC == OCCType.C_FIRST_SALE_CONTRACT_OCC ? true : false,
                                    DelayedMonth = null,
                                    StartOperationDate = null,
                                    ContractOCC = item.OCC,
                                    ForceIssueFlag = false,

                                    BillingAmountUsd = item.BillingAmtUsd,
                                    BillingAmountCurrencyType = item.BillingAmtCurrencyType
                                };
                                var billingDetail_result = handlerBilling.ManageBillingDetail(dtTbt_BillingDetail);
                                billingDetail_result_list.Add(billingDetail_result);
                            }

                            // ManageBillingDetail !!
                            //var billingDetail_result = handlerBilling.ManageBillingDetail(dtTbt_BillingDetail);
                            //List<tbt_BillingDetail> billingDetail_result_list = new List<tbt_BillingDetail>();
                            //billingDetail_result_list.Add(billingDetail_result);

                            // === Create new invoice ===

                            tbt_Invoice dtTbt_Invoice = new tbt_Invoice()
                            {
                                InvoiceNo = null,
                                InvoiceOCC = 0,
                                IssueInvDate = RegisterData.Details3.dtpCustomerAcceptanceDate,
                                AutoTransferDate = dtNextAutoTransferDate,
                                BillingTypeCode = BillingType.C_BILLING_TYPE_SALE_PRICE,
                                BillingTargetCode = item.BillingTargetCode,
                                InvoiceAmount = null,
                                PaidAmountIncVat = null,
                                VatRate = null,
                                VatAmount = null,
                                WHTRate = null,
                                WHTAmount = null,
                                RegisteredWHTAmount = null,
                                InvoicePaymentStatus = (item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                                        || item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER) ? PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT : PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,

                                IssueInvFlag = true,
                                FirstIssueInvDate = System.DateTime.Now,
                                FirstIssueInvFlag = true,
                                PaymentMethod = item.PayMethod,
                                CorrectReason = null,
                                RefOldInvoiceNo = null,

                                VatAmountUsd = null,
                                VatAmountCurrencyType = null,
                                WHTAmountUsd = null,
                                WHTAmountCurrencyType = null,
                                InvoiceAmountUsd = null,
                                InvoiceAmountCurrencyType = null
                            };


                            // ManageInvoiceByCommand !! + Report output
                            var invoice_result = handlerBilling.ManageInvoiceByCommand(
                                                                    dtTbt_Invoice
                                                                    , billingDetail_result_list
                                                                    , true
                                                                    , false);

                            // Keep report file name for merge
                            if (invoice_result != null)
                            {
                                if (string.IsNullOrEmpty(invoice_result.FilePath) == false)
                                {
                                    MergeFileList.Add(invoice_result.FilePath);
                                }
                            }
                        }


                        // === Issue sale invoice (Partial fee) ===

                        if (bIssuePartialFee && doGetBillingDetailPartialFee != null && doGetBillingDetailPartialFee.Count > 0)
                        {
                            // Prepare billing detail
                            tbt_BillingDetail dtTbt_BillingDetail_partialFee = new tbt_BillingDetail()
                            {
                                ContractCode = doGetBillingDetailPartialFee[0].ContractCode,
                                BillingOCC = doGetBillingDetailPartialFee[0].BillingOCC,
                                BillingDetailNo = doGetBillingDetailPartialFee[0].BillingDetailNo,
                                InvoiceNo = null,
                                InvoiceOCC = null,
                                IssueInvDate = System.DateTime.Now,
                                IssueInvFlag = true,
                                BillingTypeCode = doGetBillingDetailPartialFee[0].BillingTypeCode,
                                BillingTypeGroup = BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE,
                                BillingAmount = doGetBillingDetailPartialFee[0].BillingAmount,
                                AdjustBillingAmount = doGetBillingDetailPartialFee[0].AdjustBillingAmount,
                                AdjustBillingAmountUsd = doGetBillingDetailPartialFee[0].AdjustBillingAmountUsd,
                                AdjustBillingAmountCurrencyType = doGetBillingDetailPartialFee[0].AdjustBillingAmountCurrencyType,
                                BillingStartDate = doGetBillingDetailPartialFee[0].BillingStartDate,
                                BillingEndDate = doGetBillingDetailPartialFee[0].BillingEndDate,
                                PaymentMethod = item.PayMethod,
                                PaymentStatus = (item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                                || item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                                ? PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT : PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT,

                                AutoTransferDate = dtNextAutoTransferDate,
                                FirstFeeFlag = strSaleOCC == OCCType.C_FIRST_SALE_CONTRACT_OCC ? true : false,
                                DelayedMonth = doGetBillingDetailPartialFee[0].DelayedMonth,
                                StartOperationDate = null,
                                ContractOCC = doGetBillingDetailPartialFee[0].ContractOCC,
                                ForceIssueFlag = doGetBillingDetailPartialFee[0].ForceIssueFlag,

                                BillingAmountUsd = doGetBillingDetailPartialFee[0].BillingAmountUsd,
                                BillingAmountCurrencyType = doGetBillingDetailPartialFee[0].BillingAmountCurrencyType
                            };

                            // Update tbt_BillingDetail !!
                            var billingDetail_partialFee_result = handlerBilling.Updatetbt_BillingDetail(dtTbt_BillingDetail_partialFee);
                            List<tbt_BillingDetail> billingDetail_partialFee_result_list = new List<tbt_BillingDetail>();
                            billingDetail_partialFee_result_list.Add(dtTbt_BillingDetail_partialFee);



                            // === Create new invoice (partial fee) ===

                            // === Preate invoice === 
                            tbt_Invoice dtTbt_Invoice_partialFee = new tbt_Invoice()
                            {
                                InvoiceNo = null,
                                InvoiceOCC = 0,
                                IssueInvDate = System.DateTime.Now,
                                AutoTransferDate = dtNextAutoTransferDate,
                                BillingTypeCode = BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL,
                                BillingTargetCode = item.BillingTargetCode,
                                InvoiceAmount = null,
                                PaidAmountIncVat = null,
                                VatRate = null,
                                VatAmount = null,
                                WHTRate = null,
                                WHTAmount = null,
                                RegisteredWHTAmount = null,
                                InvoicePaymentStatus = (item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                                                        || item.PayMethod == PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER)
                                                        ? PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT : PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,

                                IssueInvFlag = true,
                                FirstIssueInvDate = System.DateTime.Now,
                                FirstIssueInvFlag = true,
                                PaymentMethod = item.PayMethod,
                                CorrectReason = null,
                                RefOldInvoiceNo = null,

                                VatAmountUsd = null,
                                VatAmountCurrencyType = null,
                                WHTAmountUsd = null,
                                WHTAmountCurrencyType = null,
                                InvoiceAmountUsd = null,
                                InvoiceAmountCurrencyType = null
                            };


                            // ManageInvoiceByCommand !! + Report output
                            var invoice_partialFee_result = handlerBilling.ManageInvoiceByCommand(dtTbt_Invoice_partialFee
                                                                                                    , billingDetail_partialFee_result_list
                                                                                                    , true
                                                                                                    , false); // isEncrype = false ==> need to merge file for issue
                                                                                                                // Keep reprot fie name for merge
                            if (invoice_partialFee_result != null)
                            {
                                if (string.IsNullOrEmpty(invoice_partialFee_result.FilePath) == false)
                                {
                                    MergeFileList.Add(invoice_partialFee_result.FilePath);
                                }
                            }
                        }
                    }
                }

                // == Merge report ==
                string mergeOutputFilename = string.Empty;
                string encryptOutputFileName = string.Empty;

                mergeOutputFilename = PathUtil.GetTempFileName(".pdf");
                encryptOutputFileName = PathUtil.GetTempFileName(".pdf");
                bool isSuccess = false;
                if (MergeFileList.Count > 0)
                {

                    if (MergeFileList.Count == 1)
                    {
                        // encrypt
                        isSuccess = ReportUtil.EncryptPDF(MergeFileList[0], encryptOutputFileName, null);
                    }
                    else
                    {
                        // merge + encrypt
                        isSuccess = ReportUtil.MergePDF(MergeFileList.ToArray(), mergeOutputFilename, true, encryptOutputFileName, null);
                    }

                    RegisterData.strFilePath = isSuccess ? encryptOutputFileName : string.Empty;
                }


                param.RegisterData = RegisterData;

                // COMMIT TRANSACTION !!
                scope.Complete();

                if (res.MessageList == null || res.MessageList.Count == 0)
                {
                    res.ResultData = "1";
                }
                else
                {
                    res.ResultData = "0";
                }
            }
            catch (Exception ex)
            {
                // Rollback transaction ...
                scope.Dispose();

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.ResultData = "0";
                res.AddErrorMessage(ex);
            }
             }
            return res;
        }

        public ActionResult BLS070_UpdateIssuePartialFeeFlag()
        {

            ObjectResultData res = new ObjectResultData();
            try
            {
                BLS070_ScreenParameter param = GetScreenObject<BLS070_ScreenParameter>();
                param.bIssuePartialFee = true;
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.ResultData = "0";
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }


        public ActionResult BLS070_CheckExistFile(string fileName)
        {
            try
            {

                if (System.IO.File.Exists(fileName) == true)
                {
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }

            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }



        }

        /// <summary>
        /// Mothod for download document (PDF) and write history to download log
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult BLS070_GetInvoiceReport(string fileName)
        {

            try
            {

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
