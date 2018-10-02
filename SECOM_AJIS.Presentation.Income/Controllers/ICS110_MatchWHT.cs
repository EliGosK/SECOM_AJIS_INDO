
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
        public ActionResult ICS110_Authority(ICS110_ScreenParameter param)
        {
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

            ObjectResultData res = new ObjectResultData();

            if (handlerCommon.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            // Check User Permission
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_MATCH_WHT, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            return InitialScreenEnvironment<ICS110_ScreenParameter>("ICS110", param, res);

        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS110")]
        public ActionResult ICS110()
        {
            return View();
        }

        /// <summary>
        /// Get data for initialize match payment detail grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data data for initialize match payment detail grid.</returns>
        public ActionResult ICS110_InitialMatchWHTDetail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS110_MatchWHTDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Get data for initialize search payment result grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data data for initialize search payment result grid.</returns>
        public ActionResult ICS110_InitialSearchPaymentResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS110_SearchPaymentResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        public ActionResult ICS110_GetWHTData(string WHTNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var wht = hand.GetTbt_IncomeWHT(WHTNo);
                res.ResultData = (wht == null ? null : wht.FirstOrDefault());
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS110_GetWHTDetail(string WHTNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var list = hand.GetMatchWHTDetail(WHTNo, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (list != null & list.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage(list);
                }
                res.ResultData = CommonUtil.ConvertToXml<doMatchWHTDetail>(list, "Income\\ICS110_MatchWHTDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS110_SearchPayment(doPaymentForWHTSearchCriteria param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                if (param == null)
                {
                    param = new doPaymentForWHTSearchCriteria();
                }

                var list = hand.SearchPaymentForWHT(param);
                if (list != null & list.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage(list);
                }
                res.ResultData = CommonUtil.ConvertToXml<doPaymentForWHT>(list, "Income\\ICS110_SearchPaymentResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS110_RegisterWHT(doPaymentForWHTRegister param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

                var obj = CommonUtil.CloneObject<doPaymentForWHTRegister, doPaymentForWHTRegister>(param);
                ValidatorUtil.BuildErrorMessage(res, new object[] { obj }, null);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                if ((param.TotalMatchedAmount ?? 0) > (param.Amount ?? 0))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INCOME, ScreenID.C_SCREEN_ID_MATCH_WHT
                        , MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7126);
                    return Json(res);
                }

                ICS110_ScreenParameter sParam = GetScreenObject<ICS110_ScreenParameter>();
                sParam.RegisterParam = param;

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS110_ConfirmWHT()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICS110_ScreenParameter sParam = GetScreenObject<ICS110_ScreenParameter>();
                if (sParam.RegisterParam == null)
                {
                    throw new ApplicationException("RegisterParam is null");
                }

                var param = CommonUtil.CloneObject<doPaymentForWHTRegister, doPaymentForWHTRegister>(sParam.RegisterParam);
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                tbt_IncomeWHT wht = null;
                using (TransactionScope scope = new TransactionScope())
                {
                if (param.WHTNo == null)
                {
                    wht = new tbt_IncomeWHT()
                    {
                        WHTNo = hand.GenerateWHTNo(param.WHTMatchingDate),
                        Amount = param.AmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL ? param.Amount : 0,
                        AmountUsd = param.AmountCurrencyType == CurrencyUtil.C_CURRENCY_US ? param.Amount.Value : 0,
                        AmountCurrencyType = param.AmountCurrencyType,
                        DocumentDate = param.DocumentDate ?? DateTime.MinValue,
                        WHTMatchingDate = param.WHTMatchingDate ?? DateTime.MinValue,
                        CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                        CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                    };
                    hand.InsertTbt_IncomeWHT(new List<tbt_IncomeWHT>() { wht });
                }
                else
                {
                    var tmpWht = hand.GetTbt_IncomeWHT(param.WHTNo);
                    if (tmpWht == null || tmpWht.Count <= 0)
                    {
                        throw new ApplicationException("Missing data tbt_IncomeWHT of WHTNo. " + param.WHTNo);
                    }
                    wht = tmpWht.FirstOrDefault();
                    wht.Amount = param.AmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL ? param.Amount : 0;
                    wht.AmountUsd = param.AmountCurrencyType == CurrencyUtil.C_CURRENCY_US ? param.Amount.Value : 0;
                    wht.AmountCurrencyType = param.AmountCurrencyType;
                    wht.DocumentDate = param.DocumentDate ?? DateTime.MinValue;
                    //WHTMatchingDate is not allowed to changed after created.
                    //wht.WHTMatchingDate = param.WHTMatchingDate ?? DateTime.MinValue;
                    wht.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    wht.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    hand.UpdateTbt_IncomeWHT(new List<tbt_IncomeWHT>() { wht });
                }

                hand.UpdateWHTNoToPayment(
                    wht.WHTNo,
                    param.PaymentTransNoList,
                    CommonUtil.dsTransData.dtUserData.EmpNo,
                    CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                );

                scope.Complete();
                }

                res.ResultData = new
                {
                    IsSuccess = true,
                    WHTNo = wht.WHTNo
                };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS110_GetLoadingWHTNo()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICS110_ScreenParameter sParam = GetScreenObject<ICS110_ScreenParameter>();
                if (sParam != null && sParam.LoadWHTNo != null)
                {
                    res.ResultData = sParam.LoadWHTNo;
                    sParam.LoadWHTNo = null;
                }
                else
                {
                    res.ResultData = "";
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

    }
}
