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
        public ActionResult ICS082_Authority(ICS082_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (IsSuspend(res))
                    return Json(res);

                if (!ICS082_IsAllowOperate(res))
                    return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return InitialScreenEnvironment<ICS082_ScreenParameter>("ICS082", param, res);
        }
        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool ICS082_IsAllowOperate(ObjectResultData res)
        {
            //Check by caller
            //Pass
            return true;
        }
        #endregion

        #region Views
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS082")]
        public ActionResult ICS082()
        {
            if (ICS082_ScreenData != null && !string.IsNullOrEmpty(ICS082_ScreenData.BillingTargetCode))
            {
                //Get Data
                IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                List<doUnpaidBillingTarget> unpaidList = handler.GetUnpaidBillingTargetByCode(ICS082_ScreenData.BillingTargetCode);
                if (unpaidList != null && unpaidList.Count > 0)
                {
                    ViewBag.ics082BillingTargetCodeLongFormat = unpaidList[0].BillingTargetCode;
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
        /// Generate xml for initial invoice grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS082_InitialInvoiceGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS082", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Generate xml for invoice list of specific billing target code of screen session
        /// </summary>
        /// <returns></returns>
        public ActionResult ICS082_GetInvoiceGrid()
        {
            List<doUnpaidInvoice> doUnpaidInvoice = new List<doUnpaidInvoice>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                if (ICS082_ScreenData != null && !string.IsNullOrEmpty(ICS082_ScreenData.BillingTargetCode))
                {
                    //Get Data
                    IIncomeHandler handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                    doUnpaidInvoice = handler.GetUnpaidInvoiceByBillingTarget(ICS082_ScreenData.BillingTargetCode);
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
            res.ResultData = CommonUtil.ConvertToXml<doUnpaidInvoice>(doUnpaidInvoice, "Income\\ICS082", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Screen session
        /// </summary>
        private ICS082_ScreenParameter ICS082_ScreenData
        {
            get
            {
                return GetScreenObject<ICS082_ScreenParameter>();
            }
            set
            {
                UpdateScreenObject(value);
            }
        }
        #endregion
    }
}