using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Billing;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.ActionFilters;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of CMS430
        /// </summary>
        /// <param name="param">Screen parameter</param>
        /// <returns></returns>
        public ActionResult CMS430_Authority(CMS430_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil cm = new CommonUtil();
            try
            {
                string strContractCode = null;

                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                // If param.ContractCode is null then set to  CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                if (CommonUtil.IsNullOrEmpty(param.ContractCode) == true
                    && param.CommonSearch != null && string.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                {
                    //param.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                    param.ContractCode = param.CommonSearch.ContractCode;
                }

                // Check permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_DIPOSIT_INFORMATION, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                // is parameter OK ?
                if (CommonUtil.IsNullOrEmpty(param.ContractCode))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147);
                    return Json(res);
                }
                else
                {
                    if (!CommonUtil.IsNullOrEmpty(param.BillingOCC))
                    {
                        List<dtViewDepositDetailInformation> lst = new List<dtViewDepositDetailInformation>();                       
                        strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                        IViewBillingHandler viewHandler = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;

                        lst = viewHandler.GetViewDepositDetailInformation(strContractCode, param.BillingOCC);
                        if (lst.Count <= 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                            return Json(res);
                        }
                        else
                        {
                            for (int i = 0; i < lst.Count(); i++)
                            {
                                lst[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                            }
                        }
                    }
                }

                // Check Data Exist
                
                strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                List<dtTbt_RentalContractBasicForView> lstBaisc = handler.GetTbt_RentalContractBasicForView(strContractCode);
                if (lstBaisc.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }
                else
                {
                    for (int i = 0; i < lstBaisc.Count(); i++)
                    {
                        lstBaisc[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }
                    param.dtRentalContract = lstBaisc[0];
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS430_ScreenParameter>("CMS430", param, res);
        }


        [Initialize("CMS430")]
        /// <summary>
        /// Initialize screen of CMS430
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS430()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS430_ScreenParameter param = GetScreenObject<CMS430_ScreenParameter>();
                CommonUtil cm = new CommonUtil();
                string strContractCode = cm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IViewBillingHandler viewHandler = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();
                //List<dtViewBillingOccList> lst = viewHandler.GetViewBillingOccList(strContractCode);
                List<dtViewBillingOccList> lst = viewHandler.GetViewBillingOCCListForDepositFree(strContractCode);
                for (int i = 0; i < lst.Count(); i++)
                {
                    lst[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                if (lst.Count > 0)
                {
                    foreach(dtViewBillingOccList data in lst){
                        if (CommonUtil.IsNullOrEmpty(data.MonthlyBillingAmount))
                        {
                            data.MonthlyBillingAmount = 0.00M;
                        }
                        if (CommonUtil.IsNullOrEmpty(data.BalanceDeposit))
                        {
                            data.BalanceDeposit = 0.00M;
                        }                        
                        if (CommonUtil.IsNullOrEmpty(data.BillingOCC))
                        {
                            data.BillingOCC = "-";
                        }
                        if (CommonUtil.IsNullOrEmpty(data.PaymentName))
                        {
                            data.PaymentName = "-";
                        }
                        if (CommonUtil.IsNullOrEmpty(data.IssueInvoiceName))
                        {
                            data.IssueInvoiceName = "-";
                        }
                        if (CommonUtil.IsNullOrEmpty(data.OfficeName))
                        {
                            data.OfficeName = "-";
                        }                       
                    }

                    param.dtOCCList = lst;
                }

                // Prepare for show section
                ViewBag.txtContractCode = param.dtRentalContract.ContractCodeShort;
                ViewBag.txtNormalDepositFee = param.dtRentalContract.TextTransferNormalDepositFee_CMS430;
                ViewBag.txtNormalDepositFeeUsd = param.dtRentalContract.TextTransferNormalDepositFeeUsd_CMS430; //Merge at 14032017 By Pachara S.
                //ViewBag.txtNormalDepositFee = param.dtRentalContract.NormalDepositFeeNumeric;
                ViewBag.txtBillingDepositFee = param.dtRentalContract.TextTransferOrderDepositFee_CMS430;
                ViewBag.txtBillingDepositFeeUsd = param.dtRentalContract.TextTransferOrderDepositFeeUsd_CMS430; //Merge at 14032017 By Pachara S.
                //ViewBag.txtBillingDepositFee = param.dtRentalContract.OrderDepositFeeNumeric;
                ViewBag.txtExemptedDepositFee = param.dtRentalContract.TextTransferExemptedDepositFee_CMS430;
                ViewBag.txtExemptedDepositFeeUsd = param.dtRentalContract.TextTransferExemptedDepositFeeUsd_CMS430; //Merge at 14032017 By Pachara S.
                //ViewBag.txtExemptedDepositFee = param.dtRentalContract.ExemptedDepositFeeNumeric;
                ViewBag.txtContractTargetNameEN = param.dtRentalContract.CustFullNameEN_Cust;
                ViewBag.txtBranchNameEN = param.dtRentalContract.BranchNameEN;
                ViewBag.txtSiteNameEN = param.dtRentalContract.SiteNameEN_Site;
                ViewBag.txtContractTargetNameLC = param.dtRentalContract.CustFullNameLC_Cust;
                ViewBag.txtBranchNameLC = param.dtRentalContract.BranchNameLC;
                ViewBag.txtSiteNameLC = param.dtRentalContract.SiteNameLC_Site;

                //To for test
                //ViewBag.BillingOCC = "01";
                ViewBag.BillingOCC = param.BillingOCC;
                ViewBag.ContractCode = param.ContractCode;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return View();
        }

        /// <summary>
        /// Initial billing occ grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS430_InitialBillingOCCGrid()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CMS430_ScreenParameter param = GetScreenObject<CMS430_ScreenParameter>();
                return Json(CommonUtil.ConvertToXml<dtViewBillingOccList>(param.dtOCCList, "Common\\CMS430_ViewBillingOCCList", CommonUtil.GRID_EMPTY_TYPE.VIEW));
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Initial deposit detail grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS430_InitialDeposiDetailGrid()
        {

            return Json(CommonUtil.ConvertToXml<dtViewDepositDetailInformation>(null, "Common\\CMS430_ViewDepositDetail", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Get view deposit detail grid
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="BillingOCC"></param>
        /// <returns></returns>
        public ActionResult CMS430_GetViewDepositDetailGrid(string ContractCode, string BillingOCC)
        {
            ObjectResultData res = new ObjectResultData();
            CMS430_ScreenParameter param = GetScreenObject<CMS430_ScreenParameter>();
            return Json(CommonUtil.ConvertToXml<dtViewDepositDetailInformation>(param.dtDepositDetail, "Common\\CMS430_ViewDepositDetail", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Get view deposit detail
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="BillingOCC"></param>
        /// <returns></returns>
        public ActionResult CMS430_GetViewDepositDetailControl(string ContractCode, string BillingOCC)
        {
            ObjectResultData res = new ObjectResultData();
            List<dtViewDepositDetailInformation> lst = new List<dtViewDepositDetailInformation>();
            try
            {
                CMS430_ScreenParameter param = GetScreenObject<CMS430_ScreenParameter>();
                CommonUtil cm = new CommonUtil();
                string strContractCode = cm.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                IViewBillingHandler viewHandler = ServiceContainer.GetService<IViewBillingHandler>() as IViewBillingHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();
                lst = viewHandler.GetViewDepositDetailInformation(strContractCode, BillingOCC);
                for (int i = 0; i < lst.Count(); i++)
                {
                    lst[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }
                if (lst.Count > 0)
                {
                    if (lst.Count == 1 && CommonUtil.IsNullOrEmpty(lst[0].DepositFeeNo))
                    {
                        param.dtDepositDetail = null;
                    }
                    else
                    {
                        decimal BalanceOfDeposit = 0;
                        foreach(dtViewDepositDetailInformation data in lst)
                        {
                            if (data.DepositStatus == DepositStatus.C_DEPOSIT_STATUS_PAYMENT)
                            {
                                BalanceOfDeposit += (data.ProcessAmount ?? 0);
                            }
                            else if (data.DepositStatus == DepositStatus.C_DEPOSIT_STATUS_RETURN
                                     || data.DepositStatus == DepositStatus.C_DEPOSIT_STATUS_SLIDE
                                     || data.DepositStatus == DepositStatus.C_DEPOSIT_STATUS_REVENUE
                                     || data.DepositStatus == DepositStatus.C_DEPOSIT_STATUS_CANCEL_PAYMENT
                                     || data.DepositStatus == DepositStatus.C_DEPOSIT_STATUS_CANCEL_SLIDE)
                            {
                                BalanceOfDeposit += (data.ProcessAmount ?? 0);
                            }

                            data.BalanceOfDeposit = BalanceOfDeposit;

                            if (CommonUtil.IsNullOrEmpty(data.ReceivedFee))
                            {
                                //Receive Fee & Currency is NULL , Default it.
                                data.ReceivedFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                                data.ReceivedFee = 0;
                            }
                            if (CommonUtil.IsNullOrEmpty(data.ProcessAmount))
                            {
                                data.ProcessAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                                data.ProcessAmount = 0;
                            }
                        }
                        param.dtDepositDetail = lst;                        
                    }
                    res.ResultData = lst[0];
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
            return Json(res);
        }
    }
}
