using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Text;
using System.Transactions;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Billing.Models;
using SECOM_AJIS.Presentation.Common.Helpers;
using SECOM_AJIS.Presentation.Common;

namespace SECOM_AJIS.Presentation.Billing.Controllers
{
    public partial class BillingController : BaseController
    {
        /// <summary>
        /// Check permission for access screen BLS090
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult BLS090_Authority(BLS090_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            // Is suspend ?
            ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            if (handler.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            // Check permission
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_EDIT_MONTHLY, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }


            return InitialScreenEnvironment<object>("BLS090", param, res);
        }

        /// <summary>
        /// Method for return view of screen BLS090
        /// </summary>
        /// <returns></returns>
        [Initialize("BLS090")]
        public ActionResult BLS090()
        {
            return View();
        }

        /// <summary>
        ///  Initial grid of screen BLS090
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS090_InitialSearchResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Billing\\BLS090_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get header data of screen BLS090
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        public ActionResult BLS090_GetHeader(string ContractCode)
        {
            CommonUtil cm = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<dtBillingContract> list = new List<dtBillingContract>();

            try
            {
                BLS090_ScreenParameter param = GetScreenObject<BLS090_ScreenParameter>();

                if (CommonUtil.IsNullOrEmpty(ContractCode))
                {
                    string lblContractCode = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_BILLING, "BLS090", "lblContractCode");
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { lblContractCode }, new string[] { "ContractCode" });
                    return Json(res);
                }

                //string strContract_long = cm.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                string strContract_long = cm.ConvertBillingCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                list = handlerBilling.GetBillingContract(strContract_long, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);


                if (list.Count > 0)
                {
                    var detailList = handlerBilling.GetBillingBasicForRentalList(strContract_long);
                    

                    if (detailList.Count > 0)
                    {
                        param.DetailData = detailList;

                        // Akat K. : ContractFee get from OrderContractFee (see sp_BL_GetBillingContract)
                        var sumOfFee = detailList.Sum(m => m.MonthlyBillingAmount);
                        //list[0].ContractFeeForDisplay = CommonUtil.TextNumeric(sumOfFee, 2);
                        //list[0].ContractFee = sumOfFee;
                        param.TotalFee = sumOfFee;

                        list[0].ContractFeeForDisplay = CommonUtil.TextNumeric(list[0].ContractFee, 2);
                        list[0].TotalFee = sumOfFee;
                        list[0].TotalFeeForDisplay = CommonUtil.TextNumeric(sumOfFee, 2);
                        list[0].details = detailList;

                        param.OrderContractFee = list[0].ContractFee;
                        //list[0].ContractFeeCurrency = MiscellaneousTypeUtil.getCurrenctName(list[0].ContractFeeCurrency) + " " + list[0].ContractFee?.ToString("#,##0.00");
                        ViewBag.Currency = MiscellaneousTypeCommon.getCurrencyName(list[0].ContractFeeCurrencyType);

                        res.ResultData = list[0];
                    }
                    else
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6082,
                                        new string[] { "lblContractCode" },
                                        new string[] { "ContractCode" });
                    }
                    
                }
                else
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6057,
                                        new string[] { "lblContractCode" },
                                        new string[] { "ContractCode" });
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
        /// Get detail data of screen BLS090
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        public ActionResult BLS090_GetDetail(string ContractCode)
        {
            CommonUtil cm = new CommonUtil();
            string strContract_long = cm.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<dtBillingBasicForRentalList> list = new List<dtBillingBasicForRentalList>();

            try
            {
                BLS090_ScreenParameter param = GetScreenObject<BLS090_ScreenParameter>();
                if (param.DetailData == null)
                {
                    param.DetailData = new List<dtBillingBasicForRentalList>();
                }

                list = param.DetailData;
                
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtBillingBasicForRentalList>(list, "Billing\\BLS090_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);


        }

        /// <summary>
        /// Validate input data before save to database (BLS090)
        /// </summary>
        /// <param name="RegisterData"></param>
        /// <returns></returns>
        public ActionResult BLS090_Register(List<BLS090_DetailData> RegisterData)
        {
            CommonUtil cm = new CommonUtil();

            ObjectResultData res = new ObjectResultData();
            List<dtBillingContract> list = new List<dtBillingContract>();

            try
            {

                BLS090_ScreenParameter param = GetScreenObject<BLS090_ScreenParameter>();

                // Check permission
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_TRANSFER_BUFFER, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                // Is suspend ?
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                var newFee = RegisterData.Sum(m => m.NewBillingFee);

                // Akat K. : compare to OrderContractFee
                //if (newFee != param.TotalFee)
                if (newFee != param.OrderContractFee)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6058);
                    return Json(res);
                }

                param.RegisterData = RegisterData;

                for(int i = 0; i < param.DetailData.Count; i++)
                {
                    param.DetailData[i].NewMonthlyBillingAmountCurrency = RegisterData[i].NewMonthlyBillingAmountCurrency;
                }

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
        /// Register input data to database (BLS090)
        /// </summary>
        /// <returns></returns>
        public ActionResult BLS090_Confirm()
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            List<dtBillingContract> list = new List<dtBillingContract>();

            try
            {
                // Is suspend ?
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                BLS090_ScreenParameter param = GetScreenObject<BLS090_ScreenParameter>();
                IBillingHandler handlerBilling = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                if (param.RegisterData == null)
                {
                    param.RegisterData = new List<BLS090_DetailData>();
                }

                var updateList = (from p in param.RegisterData where p.MonthlyBillingAmount != p.NewBillingFee || p.MonthlyBillingAmountCurrencyType != p.NewMonthlyBillingAmountCurrency select p).ToList<BLS090_DetailData>();

                // Prepare
                List<tbt_BillingBasic> billiingBasicList = new List<tbt_BillingBasic>();
                tbt_BillingBasic billiingBasic;

                List<tbt_MonthlyBillingHistory> billingHistoryList_ForInsert = new List<tbt_MonthlyBillingHistory>();
                List<tbt_MonthlyBillingHistory> billingHistoryList_ForUpdate = new List<tbt_MonthlyBillingHistory>();
                tbt_MonthlyBillingHistory billingHistory;
                foreach (var item in updateList)
                {
                    decimal? amount = null;
                    decimal? amountUS = null;
                    if (item.NewMonthlyBillingAmountCurrency == CurrencyUtil.C_CURRENCY_LOCAL)
                        amount = item.NewBillingFee;
                    else
                        amountUS = item.NewBillingFee;

                    // Billig basic
                    billiingBasic = new tbt_BillingBasic()
                    {
                        ContractCode = item.ContractCode,
                        BillingOCC = item.BillingOCC,
                        StopBillingFlag = (Convert.ToDecimal(item.NewBillingFee) <= 0), // ***
                        //MonthlyBillingAmount = item.NewBillingFee,  // ***
                        MonthlyBillingAmount = amount, // add by jirawat jannet @ 2016-08-29
                        MonthlyBillingAmountUsd = amountUS, // add by jirawat jannet @ 2016-08-29
                        UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                        UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        MonthlyBillingAmountCurrencyType = item.NewMonthlyBillingAmountCurrency
                    };
                    billiingBasicList.Add(billiingBasic);



                    // Billing history
                    
                    var lastBillingHistory = handlerBilling.GetLastBillingHistory(item.ContractCode, item.BillingOCC, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);

                    if (lastBillingHistory.Count > 0)
                    {
                        // Case UPDATE.
                        var billingHistory_update = lastBillingHistory[0];
                        billingHistory_update.MonthlyBillingAmount = amount;// item.NewBillingFee; // ***
                        billingHistory_update.MonthlyBillingAmountUsd = amountUS;
                        billingHistory_update.MonthlyBillingAmountCurrencyType = item.NewMonthlyBillingAmountCurrency;
                        billingHistory_update.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        billingHistory_update.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        billingHistoryList_ForUpdate.Add(billingHistory_update);
                        

                    }
                    else
                    {
                        // Case CREATE.
                        billingHistory = new tbt_MonthlyBillingHistory()
                        {
                            ContractCode = item.ContractCode,
                            BillingOCC = item.BillingOCC,
                            MonthlyBillingAmount = item.NewBillingFee,  // ***
                            BillingStartDate = DateTime.Now,
                            CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                            UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                        };
                        billingHistoryList_ForInsert.Add(billingHistory);
                    }

                    
                }

                // TODO : (Narupon W.) , Uncomment as finally
                using (TransactionScope scope = new TransactionScope())
                {
                try
                {
                        // Save to DB !!
                        
                        handlerBilling.UpdateMonthlyBillingAmount(billiingBasicList);

                        // Case Create.
                        handlerBilling.InsertTbt_MonthlyBillingHistory(CommonUtil.ConvertToXml_Store(billingHistoryList_ForInsert));
                        // Case Update.
                        handlerBilling.UpdateTbt_MonthlyBillingHistoryData(CommonUtil.ConvertToXml_Store(billingHistoryList_ForUpdate));
                    scope.Complete();
                }
                catch (Exception ex)
                    {
                    scope.Dispose();
                    throw ex;
                    }
                }

                res.ResultData = "1";
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046, null, null);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
          
            return Json(res);
        }

        public ActionResult BLS090_GenerateCurrencyNumericTextBox(string id, string value, string currency, bool readOnly)
        {
            string html = string.Empty;
            if (readOnly)
                html = TextBoxHelper.NumericTextBoxWithMultipleCurrency(null, id, value, currency, new { style = "width: 140px;", @readonly = "readonly" }).ToString();
            else
                html = TextBoxHelper.NumericTextBoxWithMultipleCurrency(null, id, value, currency, new { style = "width: 140px;", }).ToString();
            return Json(html);
        }

        public ActionResult BLS090_GetCurrencyDisplay(string currencyCode)
        {
            return Json(MiscellaneousTypeCommon.getCurrencyName(currencyCode));
        }

    }


}
