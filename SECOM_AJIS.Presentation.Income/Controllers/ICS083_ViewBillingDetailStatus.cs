using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Income.Models;
using System.Transactions;
using System.Globalization;

namespace SECOM_AJIS.Presentation.Income.Controllers
{
    public partial class IncomeController : BaseController
    {
        #region Authority
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ICS083_Authority(ICS083_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS083_IsAllowOperate(res))
                    return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return InitialScreenEnvironment<ICS083_ScreenParameter>("ICS083", param, res);
        }
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool ICS083_IsAllowOperate(ObjectResultData res)
        {
            //Check by caller
            //Pass
            return true;
        }
        #endregion

        #region View
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS083")]
        public ActionResult ICS083()
        {
            if (ICS083_ScreenData != null && !string.IsNullOrEmpty(ICS083_ScreenData.BillingTargetCode))
            {
                //Get Data
                IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                List<doUnpaidBillingTarget> unpaidList = handler.GetUnpaidBillingTargetByCode(ICS083_ScreenData.BillingTargetCode);
                if (unpaidList != null && unpaidList.Count > 0)
                {
                    ViewBag.BillingTargetCode = new CommonUtil().ConvertBillingTargetCode(unpaidList[0].BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.BillingClientNameEN = unpaidList[0].BillingClientNameEN;
                    ViewBag.BillingClientNameLC = unpaidList[0].BillingClientNameLC;
                }
            }
            return View();
        }
        #endregion

        #region Actions
        /// <summary>
        /// Generate xml for initial unpaid billing detail grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS083_InitialDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS083", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Generate xml for unpaid billing detail list of specific invoice no. of screen session
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS083_GetDetailGrid()
        {
            List<doUnpaidBillingDetail> doUnpaidDetail = new List<doUnpaidBillingDetail>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                //Get Unpaid billing detail
                IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                if (ICS083_ScreenData != null)
                {
                    if (string.IsNullOrEmpty(ICS083_ScreenData.InvoiceNo))
                    {
                        doUnpaidDetail = handler.GetUnpaidBillingDetailByBillingTarget(ICS083_ScreenData.BillingTargetCode);
                    }
                    else
                    {
                        doUnpaidDetail = handler.GetUnpaidBillingDetailByInvoice(ICS083_ScreenData.InvoiceNo, ICS083_ScreenData.InvoiceOCC);
                    }
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<doUnpaidBillingDetail>(doUnpaidDetail, "Income\\ICS083", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Screen session
        /// </summary>
        private ICS083_ScreenParameter ICS083_ScreenData
        {
            get
            {
                return GetScreenObject<ICS083_ScreenParameter>();
            }
            set
            {
                UpdateScreenObject(value);
            }
        }
        #endregion
    }
}