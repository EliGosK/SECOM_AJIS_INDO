using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.ActionFilters;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Common.Models;

using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of CMS420
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS420_Authority(CMS420_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // If param.ContractCode is null then set to  CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                if (String.IsNullOrEmpty(param.ContractCode))
                {
                    //param.strContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                    if (param.CommonSearch != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                            param.ContractCode = param.CommonSearch.ContractCode;
                    }
                }


                // Check permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_BILLING_BASIC_INFORMATION, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                if (param.CallerScreenID == ScreenID.C_SCREEN_ID_SEARCH_BILLING_INFORMATION)
                {
                    // is parameter OK ?
                    if (CommonUtil.IsNullOrEmpty(param.ContractCode))
                    {
                        //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0155);
                        return Json(res);
                    }

                    if (CommonUtil.IsNullOrEmpty(param.ContractCode) && CommonUtil.IsNullOrEmpty(param.BillingOCC))
                    {
                        //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0155);
                        return Json(res);
                    }

                    // Check data exist
                    CommonUtil cm = new CommonUtil();
                    string strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    IViewBillingHandler viewBillingHandler = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                    ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();
                    List<dtViewBillingBasic> viewBillingBasicData = viewBillingHandler.GetViewBillingBasic(strContractCode, param.BillingOCC, null, null, null, null);
                    for (int i = 0; i < viewBillingBasicData.Count(); i++)
                    {
                        viewBillingBasicData[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }
                    if (viewBillingBasicData.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001); // data not found
                        return Json(res);
                    }
                }
                else
                {
                    // is parameter OK ?
                    if (CommonUtil.IsNullOrEmpty(param.ContractCode))
                    {
                        //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147);
                        return Json(res);
                    }

                    // Check data exist
                    CommonUtil cm = new CommonUtil();
                    string strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    IViewBillingHandler viewBillingHandler = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                    List<dtViewBillingOccList> viewBillingOccListData = viewBillingHandler.GetViewBillingOccList(strContractCode);
                    if (viewBillingOccListData.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001); // data not found
                        return Json(res);
                    }
                }

                param.CommonSearch = new ScreenParameter.CommonSearchDo()
                {
                    ContractCode = param.ContractCode
                };

                return InitialScreenEnvironment<CMS420_ScreenParameter>("CMS420", param, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen CMS420
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS420")]
        public ActionResult CMS420()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS420_ScreenParameter param = GetScreenObject<CMS420_ScreenParameter>();
                CommonUtil cm = new CommonUtil();

                ViewBag._ContractCode = param.ContractCode;
                ViewBag._BillingOCC = param.BillingOCC;

                string strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                ViewBag._CallerScreen = param.CallerScreenID;

                IViewBillingHandler viewBillingHandler = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtViewBillingOccList> viewBillingOccListData = viewBillingHandler.GetViewBillingOccList(strContractCode);
                List<dtViewBillingBasic> viewBillingBasicData = viewBillingHandler.GetViewBillingBasic(strContractCode, param.BillingOCC, null, null, null, null);
                List<doTbt_MonthlyBillingHistoryList> monthlyBillingHistoryListData = billingHandler.GetBillingHistoryList(strContractCode, param.BillingOCC,CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);

                for (int i = 0; i < viewBillingOccListData.Count(); i++)
                {
                    viewBillingOccListData[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }
                for (int i = 0; i < viewBillingBasicData.Count(); i++)
                {
                    viewBillingBasicData[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }
                for (int i = 0; i < monthlyBillingHistoryListData.Count(); i++)
                {
                    monthlyBillingHistoryListData[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                ViewBag.IsSpecialCareful = "0";
                ViewBag.txtVATUnchargedBillingTarget = false;
                ViewBag.txtResultBasedMaintenanceBillingFlag = false;

                if (viewBillingOccListData.Count > 0)
                {
                    //Language mapping
                    CommonUtil.MappingObjectLanguage<dtViewBillingOccList>(viewBillingOccListData);

                    ViewBag.txtContractCode = cm.ConvertContractCode(viewBillingOccListData[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }

                if (param.BillingOCC != null)
                {
                    if (viewBillingBasicData.Count > 0)
                    {
                        //Language mapping
                        CommonUtil.MappingObjectLanguage<dtViewBillingBasic>(viewBillingBasicData);

                        //Misc mapping
                        MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                        miscMapping.AddMiscType(viewBillingBasicData.ToArray<dtViewBillingBasic>());
                        handlerCommon.MiscTypeMappingList(miscMapping);

                        if (viewBillingBasicData[0].CarefulFlag.HasValue)
                        {
                            ViewBag.IsSpecialCareful = viewBillingBasicData[0].CarefulFlag.Value == true ? "1" : "0";
                        }

                        ViewBag.IsPaymentMethod = viewBillingBasicData[0].PaymentMethod;

                        ViewBag.txtBillingCode = string.Format("{0}-{1}", cm.ConvertContractCode(viewBillingBasicData[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT), viewBillingBasicData[0].BillingOCC);
                        ViewBag.txtBillingOffice = CommonUtil.TextCodeName(viewBillingBasicData[0].BillingOfficeCode, viewBillingBasicData[0].OfficeName);
                        ViewBag.txtDebtTracingOffice = CommonUtil.TextCodeName(viewBillingBasicData[0].DebtTracingOfficeCode, viewBillingBasicData[0].DebtTracingOfficeName);
                        ViewBag.txtBillingTargetCode = cm.ConvertBillingTargetCode(viewBillingBasicData[0].BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        ViewBag.txtPreviousBillingTargetCode = cm.ConvertBillingTargetCode(viewBillingBasicData[0].PreviousBillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        ViewBag.txtCustomerType = viewBillingBasicData[0].CustTypeCodeName;
                        ViewBag.txtBillingClientNameEN = viewBillingBasicData[0].FullNameEN;
                        ViewBag.txtBillingClientBranchNameEN = viewBillingBasicData[0].BranchNameEN;
                        ViewBag.txtBillingClientAddressEN = viewBillingBasicData[0].AddressEN;
                        ViewBag.txtBillingClientNameLC = viewBillingBasicData[0].FullNameLC;
                        ViewBag.txtBillingClientBranchNameLC = viewBillingBasicData[0].BranchNameLC;
                        ViewBag.txtBillingClientAddressLC = viewBillingBasicData[0].AddressLC;
                        ViewBag.txtMonthlyBillingAmount = viewBillingBasicData[0].TextTransferMonthlyBillingAmount;
                        //ViewBag.txtMonthlyBillingAmount = CommonUtil.TextNumeric(viewBillingBasicData[0].MonthlyBillingAmount);
                        ViewBag.txtPaymentMethod = viewBillingBasicData[0].PaymentMethodName;
                        ViewBag.txtBillingCycle = CommonUtil.TextNumeric(viewBillingBasicData[0].BillingCycle, 0);
                        ViewBag.txtCreditTerm = CommonUtil.TextNumeric(viewBillingBasicData[0].CreditTerm);
                        ViewBag.txtCalculationDailyFee = viewBillingBasicData[0].CalDailyFeeStatusName;
                        ViewBag.txtLastBillingDate = CommonUtil.TextDate(viewBillingBasicData[0].LastBillingDate);
                        ViewBag.txtManagementCodeForSortDetails = viewBillingBasicData[0].SortingType;
                        ViewBag.txtAdjustEndingDateOfBillingPeriod = CommonUtil.TextDate(viewBillingBasicData[0].AdjustEndDate);
                        ViewBag.txtBillingFlag = viewBillingBasicData[0].StopBillingFlagCodeName;
                        ViewBag.txtVATUnchargedBillingTarget = viewBillingBasicData[0].VATUnchargedFlag.HasValue ? viewBillingBasicData[0].VATUnchargedFlag.Value : false;
                        ViewBag.txtBalanceOfDepositFee = viewBillingBasicData[0].TextTransferBalanceDeposit;
                        //ViewBag.txtBalanceOfDepositFee = CommonUtil.TextNumeric(viewBillingBasicData[0].BalanceDeposit);
                        ViewBag.txtMonthlyFeeBeforeStop = viewBillingBasicData[0].TextTransferMonthlyFeeBeforeStop;
                        //ViewBag.txtMonthlyFeeBeforeStop = CommonUtil.TextNumeric(viewBillingBasicData[0].MonthlyFeeBeforeStop);
                        ViewBag.txtResultBasedMaintenanceBillingFlag = viewBillingBasicData[0].ResultBasedMaintenanceFlag.HasValue ? viewBillingBasicData[0].ResultBasedMaintenanceFlag.Value : false;
                        ViewBag.txtLastPaymentConditionChangingDate = CommonUtil.TextDate(viewBillingBasicData[0].ChangeDate);
                        ViewBag.txtRegisteringDateOfLastChanging = CommonUtil.TextDate(viewBillingBasicData[0].ChangeDate);
                        ViewBag.txtApproveNo = viewBillingBasicData[0].ApproveNo;
                        ViewBag.txtDocumentReceiving = viewBillingBasicData[0].DocAuditResultName;

                        ViewBag.txtAdjustmentType = viewBillingBasicData[0].AdjustType;
                        ViewBag.txtAdjustBillingAmount = viewBillingBasicData[0].TextTransferAdjustBillingPeriodAmount;
                        //ViewBag.txtAdjustBillingAmount = CommonUtil.TextNumeric(viewBillingBasicData[0].AdjustBillingPeriodAmount);

                        ViewBag.txtAdjustBillingPeriodStartDate = CommonUtil.TextDate(viewBillingBasicData[0].AdjustBillingPeriodStartDate);
                        ViewBag.txtAdjustBillingPeriodEndDate = CommonUtil.TextDate(viewBillingBasicData[0].AdjustBillingPeriodEndDate);

                        ViewBag.txtIDNo = viewBillingBasicData[0].IDNo; //Add by Jutarat A. on 12122013
                    }

                    if (monthlyBillingHistoryListData != null)
                    {
                        if (monthlyBillingHistoryListData.Count > 0)
                        {
                            ViewBag.txtLastMonthlyBillingAmount = monthlyBillingHistoryListData[0].TextTransferMonthlyBillingAmount;
                            ViewBag.txtLastDate = CommonUtil.TextDate(monthlyBillingHistoryListData[0].BillingStartDate);
                            ViewBag.txtBillingAmountBeforeChanging1 = monthlyBillingHistoryListData.Count < 2 ? null : monthlyBillingHistoryListData[1].TextTransferMonthlyBillingAmount;
                            ViewBag.txtDateBeforeChanging1 = monthlyBillingHistoryListData.Count < 2 ? null : CommonUtil.TextDate(monthlyBillingHistoryListData[1].BillingStartDate);
                            ViewBag.txtBillingAmountBeforeChanging2 = monthlyBillingHistoryListData.Count < 3 ? null : monthlyBillingHistoryListData[2].TextTransferMonthlyBillingAmount;
                            ViewBag.txtDateBeforeChanging2 = monthlyBillingHistoryListData.Count < 3 ? null : CommonUtil.TextDate(monthlyBillingHistoryListData[2].BillingStartDate);
                            ViewBag.txtBillingAmountBeforeChanging3 = monthlyBillingHistoryListData.Count < 4 ? null : monthlyBillingHistoryListData[3].TextTransferMonthlyBillingAmount;
                            ViewBag.txtDateBeforeChanging3 = monthlyBillingHistoryListData.Count < 4 ? null : CommonUtil.TextDate(monthlyBillingHistoryListData[3].BillingStartDate);
                            ViewBag.txtBillingAmountBeforeChanging4 = monthlyBillingHistoryListData.Count < 5 ? null : monthlyBillingHistoryListData[4].TextTransferMonthlyBillingAmount;
                            ViewBag.txtDateBeforeChanging4 = monthlyBillingHistoryListData.Count < 5 ? null : CommonUtil.TextDate(monthlyBillingHistoryListData[4].BillingStartDate);
                            ViewBag.txtBillingAmountBeforeChanging5 = monthlyBillingHistoryListData.Count < 6 ? null : monthlyBillingHistoryListData[5].TextTransferMonthlyBillingAmount;
                            ViewBag.txtDateBeforeChanging5 = monthlyBillingHistoryListData.Count < 6 ? null : CommonUtil.TextDate(monthlyBillingHistoryListData[5].BillingStartDate);
                        }
                    }
                }

                //Finding service type code 
                if (string.IsNullOrEmpty(strContractCode) == false)
                {
                    //Rental
                    IRentralContractHandler handlerR = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    List<tbt_RentalContractBasic> dtRentalContract = handlerR.GetTbt_RentalContractBasic(strContractCode, null);
                    if (dtRentalContract.Count > 0)
                    {
                        param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                        ViewBag.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                        ViewBag.ProductTypeCode = dtRentalContract[0].ProductTypeCode;
                    }
                    else
                    {
                        // Sale
                        ISaleContractHandler handlerS = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        List<tbt_SaleBasic> dtSaleContract = handlerS.GetTbt_SaleBasic(strContractCode, null, true);
                        if (dtSaleContract.Count > 0)
                        {
                            param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                            ViewBag.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                            ViewBag.ProductTypeCode = dtSaleContract[0].ProductTypeCode;
                        }
                    }
                }
                
                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Getting billing basic information
        /// </summary>
        /// <param name="BillingOCC"></param>
        /// <returns></returns>
        public ActionResult CMS420_LoadBillingBasicInformation(string BillingOCC)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS420_ScreenParameter param = GetScreenObject<CMS420_ScreenParameter>();
                CommonUtil cm = new CommonUtil();

                string strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                param.BillingOCC = BillingOCC;

                IViewBillingHandler viewBillingHandler = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtViewBillingBasic> viewBillingBasicData = viewBillingHandler.GetViewBillingBasic(strContractCode, BillingOCC, null, null, null, null);
                List<doTbt_MonthlyBillingHistoryList> monthlyBillingHistoryListData = billingHandler.GetBillingHistoryList(strContractCode, BillingOCC, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);

                for (int i = 0; i < viewBillingBasicData.Count(); i++)
                {
                    viewBillingBasicData[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                for (int i = 0; i < monthlyBillingHistoryListData.Count(); i++)
                {
                    monthlyBillingHistoryListData[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                List<object> result = new List<object>();
                bool carefulFlag = false;
                string paymentMethod = "0";

                if (viewBillingBasicData.Count > 0)
                {
                    //Language mapping
                    CommonUtil.MappingObjectLanguage<dtViewBillingBasic>(viewBillingBasicData);

                    //Misc mapping
                    MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                    miscMapping.AddMiscType(viewBillingBasicData.ToArray<dtViewBillingBasic>());
                    handlerCommon.MiscTypeMappingList(miscMapping);

                    carefulFlag = viewBillingBasicData[0].CarefulFlag.HasValue ? viewBillingBasicData[0].CarefulFlag.Value : false;
                    paymentMethod = viewBillingBasicData[0].PaymentMethod;

                    dtViewBillingBasic_ForView billingBasic = new dtViewBillingBasic_ForView()
                    {

                        BillingCode = string.Format("{0}-{1}", cm.ConvertContractCode(viewBillingBasicData[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                                                                        viewBillingBasicData[0].BillingOCC),
                        BillingOffice = CommonUtil.TextCodeName(viewBillingBasicData[0].BillingOfficeCode, viewBillingBasicData[0].OfficeName),
                        DebtTracingOffice = CommonUtil.TextCodeName(viewBillingBasicData[0].DebtTracingOfficeCode, viewBillingBasicData[0].DebtTracingOfficeName),
                        BillingTargetCode = cm.ConvertBillingTargetCode(viewBillingBasicData[0].BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                        PreviousBillingTargetCode = cm.ConvertBillingTargetCode(viewBillingBasicData[0].PreviousBillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                        CustomerType = viewBillingBasicData[0].CustTypeCodeName,
                        BillingClientNameEN = viewBillingBasicData[0].FullNameEN,
                        BillingClientBranchNameEN = viewBillingBasicData[0].BranchNameEN,
                        BillingClientAddressEN = viewBillingBasicData[0].AddressEN,
                        BillingClientNameLC = viewBillingBasicData[0].FullNameLC,
                        BillingClientBranchNameLC = viewBillingBasicData[0].BranchNameLC,
                        BillingClientAddressLC = viewBillingBasicData[0].AddressLC,
                        MonthlyBillingAmount = viewBillingBasicData[0].TextTransferMonthlyBillingAmount,
                        PaymentMethod = viewBillingBasicData[0].PaymentMethodName,
                        BillingCycle = CommonUtil.TextNumeric(viewBillingBasicData[0].BillingCycle, 0),
                        CreditTerm = CommonUtil.TextNumeric(viewBillingBasicData[0].CreditTerm),
                        CalculationDailyFee = viewBillingBasicData[0].CalDailyFeeStatusName,
                        LastBillingDate = CommonUtil.TextDate(viewBillingBasicData[0].LastBillingDate),
                        ManagementCodeForSortDetails = viewBillingBasicData[0].SortingType,
                        AdjustEndingDateOfBillingPeriod = CommonUtil.TextDate(viewBillingBasicData[0].AdjustEndDate),
                        BillingFlag = viewBillingBasicData[0].StopBillingFlagCodeName,
                        VATUnchargedBillingTarget = viewBillingBasicData[0].VATUnchargedFlag.HasValue ? viewBillingBasicData[0].VATUnchargedFlag.Value : false,
                        BalanceOfDepositFee = viewBillingBasicData[0].TextTransferBalanceDeposit,
                        MonthlyFeeBeforeStop = viewBillingBasicData[0].TextTransferMonthlyFeeBeforeStop,
                        ResultBasedMaintenanceBillingFlag = viewBillingBasicData[0].ResultBasedMaintenanceFlag.HasValue ? viewBillingBasicData[0].ResultBasedMaintenanceFlag.Value : false,
                        LastPaymentConditionChangingDate = CommonUtil.TextDate(viewBillingBasicData[0].ChangeDate),
                        RegisteringDateOfLastChanging = CommonUtil.TextDate(viewBillingBasicData[0].ChangeDate),
                        ApproveNo = viewBillingBasicData[0].ApproveNo,
                        DocumentReceiving = viewBillingBasicData[0].DocAuditResultName,

                        AdjustmentType = viewBillingBasicData[0].AdjustType,
                        AdjustBillingAmount = viewBillingBasicData[0].TextTransferAdjustBillingPeriodAmount,

                        AdjustBillingPeriodStartDate = CommonUtil.TextDate(viewBillingBasicData[0].AdjustBillingPeriodStartDate),
                        AdjustBillingPeriodEndDate = CommonUtil.TextDate(viewBillingBasicData[0].AdjustBillingPeriodEndDate)

                        ,IDNo = viewBillingBasicData[0].IDNo //Add by Jutarat A. on 12122013
                    };

                    result.Add(billingBasic);

                }
                else
                {
                    result.Add(new dtViewBillingBasic_ForView());
                }

                if (monthlyBillingHistoryListData.Count > 0)
                {
                    doTbt_MonthlyBillingHistoryList_ForView billingHistory = new doTbt_MonthlyBillingHistoryList_ForView()
                    {
                        LastMonthlyBillingAmount = monthlyBillingHistoryListData[0].TextTransferMonthlyBillingAmount,
                        LastDate = CommonUtil.TextDate(monthlyBillingHistoryListData[0].BillingStartDate),
                        BillingAmountBeforeChanging1 = monthlyBillingHistoryListData.Count < 2 ? null : monthlyBillingHistoryListData[1].TextTransferMonthlyBillingAmount,
                        DateBeforeChanging1 = monthlyBillingHistoryListData.Count < 2 ? null : CommonUtil.TextDate(monthlyBillingHistoryListData[1].BillingStartDate),
                        BillingAmountBeforeChanging2 = monthlyBillingHistoryListData.Count < 3 ? null : monthlyBillingHistoryListData[2].TextTransferMonthlyBillingAmount,
                        DateBeforeChanging2 = monthlyBillingHistoryListData.Count < 3 ? null : CommonUtil.TextDate(monthlyBillingHistoryListData[2].BillingStartDate),
                        BillingAmountBeforeChanging3 = monthlyBillingHistoryListData.Count < 4 ? null : monthlyBillingHistoryListData[3].TextTransferMonthlyBillingAmount,
                        DateBeforeChanging3 = monthlyBillingHistoryListData.Count < 4 ? null : CommonUtil.TextDate(monthlyBillingHistoryListData[3].BillingStartDate),
                        BillingAmountBeforeChanging4 = monthlyBillingHistoryListData.Count < 5 ? null : monthlyBillingHistoryListData[4].TextTransferMonthlyBillingAmount,
                        DateBeforeChanging4 = monthlyBillingHistoryListData.Count < 5 ? null : CommonUtil.TextDate(monthlyBillingHistoryListData[4].BillingStartDate),
                        BillingAmountBeforeChanging5 = monthlyBillingHistoryListData.Count < 6 ? null : monthlyBillingHistoryListData[5].TextTransferMonthlyBillingAmount,
                        DateBeforeChanging5 = monthlyBillingHistoryListData.Count < 6 ? null : CommonUtil.TextDate(monthlyBillingHistoryListData[5].BillingStartDate)
                    };
                    
                    result.Add(billingHistory);
                }
                else
                {
                    result.Add(new doTbt_MonthlyBillingHistoryList_ForView());
                }

                result.Add(carefulFlag);
                result.Add(paymentMethod);

                res.ResultData = result.ToArray();

                return Json(res);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial grid billing OCC
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS420_InitGrid_BillingOCC()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\BillingOCC"));
        }

        /// <summary>
        /// Initial grid billing type detail
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS420_InitGrid_BillingTypeDetail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\BillingTypeDetail"));
        }

        /// <summary>
        /// Getting billing occ list for load to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS420_LoadGridBillingOCC()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS420_ScreenParameter param = GetScreenObject<CMS420_ScreenParameter>();
                CommonUtil cm = new CommonUtil();

                string strContractCode_short = param.ContractCode;
                string strContractCode = cm.ConvertContractCode(strContractCode_short, CommonUtil.CONVERT_TYPE.TO_LONG);

                IViewBillingHandler viewBillingHandler = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();
                List<dtViewBillingOccList> viewBillingOccListData = viewBillingHandler.GetViewBillingOccList(strContractCode);
                for (int i = 0; i < viewBillingOccListData.Count(); i++)
                {
                    viewBillingOccListData[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                CommonUtil.MappingObjectLanguage<dtViewBillingOccList>(viewBillingOccListData);

                string xml = CommonUtil.ConvertToXml<dtViewBillingOccList>(viewBillingOccListData, "Common\\CMS420_BillingOCC", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Getting billing type detail list for load to grid with initial
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS420_LoadGridBillingTypeDetailWithInitial()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS420_ScreenParameter param = GetScreenObject<CMS420_ScreenParameter>();
                CommonUtil cm = new CommonUtil();

                string strContractCode_short = param.ContractCode;
                string strContractCode = cm.ConvertContractCode(strContractCode_short, CommonUtil.CONVERT_TYPE.TO_LONG);

                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<doBillingTypeDetailList> billingTypeDetailListData = billingHandler.GetBillingTypeDetailList(strContractCode, param.BillingOCC);
                CommonUtil.MappingObjectLanguage<doBillingTypeDetailList>(billingTypeDetailListData);

                if (billingTypeDetailListData == null)
                {
                    billingTypeDetailListData = new List<doBillingTypeDetailList>();
                }

                string xml = CommonUtil.ConvertToXml<doBillingTypeDetailList>(billingTypeDetailListData, "Common\\CMS420_BillingTypeDetail", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Getting billing type detail list for load to grid
        /// </summary>
        /// <param name="BillingOCC"></param>
        /// <returns></returns>
        public ActionResult CMS420_LoadGridBillingTypeDetail(string BillingOCC)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS420_ScreenParameter param = GetScreenObject<CMS420_ScreenParameter>();
                CommonUtil cm = new CommonUtil();

                string strContractCode_short = param.ContractCode;
                string strContractCode = cm.ConvertContractCode(strContractCode_short, CommonUtil.CONVERT_TYPE.TO_LONG);

                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                List<doBillingTypeDetailList> billingTypeDetailListData = billingHandler.GetBillingTypeDetailList(strContractCode, BillingOCC);
                CommonUtil.MappingObjectLanguage<doBillingTypeDetailList>(billingTypeDetailListData);
                
                if (billingTypeDetailListData == null)
                {
                    billingTypeDetailListData = new List<doBillingTypeDetailList>();
                }
                
                string xml = CommonUtil.ConvertToXml<doBillingTypeDetailList>(billingTypeDetailListData, "Common\\CMS420_BillingTypeDetail", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
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
