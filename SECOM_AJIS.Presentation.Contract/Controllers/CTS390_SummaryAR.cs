//*********************************
// Create by: Akat K.
// Create date: 13/Sep/2011
// Update date: 13/Sep/2011
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Contract;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController {

        private const string CTS390_Screen = "CTS390";

        /// <summary>
        /// Check user’s permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS390_Authority(CTS390_ScreenParameter param) {
            ObjectResultData res = new ObjectResultData();
            try {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_SUMMARY_AR)) {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                if (res.IsError) {
                    return Json(res);
                }

                param.hasPermission360 = CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_AR, FunctionID.C_FUNC_ID_OPERATE);

                return InitialScreenEnvironment<CTS390_ScreenParameter>(CTS390_Screen, param, res);
            } catch (Exception ex) {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize(CTS390_Screen)]
        public ActionResult CTS390() {
            CTS390_ScreenParameter param = GetScreenObject<CTS390_ScreenParameter>();
            ViewBag.HasPermission360 = param.hasPermission360;
            ViewBag.CTS360ScreenMode = ScreenID.C_SCREEN_ID_SUMMARY_AR;
            return View();
        }

        /// <summary>
        /// Initial grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS390_InitGrid() {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS390"));
        }

        /// <summary>
        /// Search AR by summary period when click [Select] button on AR summary section
        /// </summary>
        /// <param name="summaryPeriod"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CTS390_searchAR(string summaryPeriod) {
            ObjectResultData res = new ObjectResultData();
            
            try {
                doSummaryARPeriod condition = CTS390_getPeriodDate(summaryPeriod);
                IARHandler hand = ServiceContainer.GetService<IARHandler>() as IARHandler;
                List<dtSummaryAR> summary = hand.SummaryAR(condition.dateFrom, condition.dateTo, condition.current);
                CommonUtil.MappingObjectLanguage<dtSummaryAR>(summary);

                string xml = CommonUtil.ConvertToXml<dtSummaryAR>(summary, "Contract\\CTS390", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                xml = xml.Replace("&amp;amp;nbsp;", "&amp;nbsp;"); //decode &nbsp; back 
                res.ResultData = xml;
            } catch (Exception ex) {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Create search condition from selected summary period
        /// </summary>
        /// <param name="summaryPeriod"></param>
        /// <returns></returns>
        private doSummaryARPeriod CTS390_getPeriodDate(string summaryPeriod) {
            doSummaryARPeriod result = new doSummaryARPeriod();
            // ALL
            result.dateTo = null;
            result.dateTo = null;
            result.current = DateTime.Now;
            DateTime temp = DateTime.Now;

            if (ARSummaryPeriod.C_AR_SUMMARY_PERIOD_TODAY.Equals(summaryPeriod)) {
                result.dateFrom = new DateTime(result.current.Value.Year, result.current.Value.Month, result.current.Value.Day, 0, 0, 0);
                result.dateTo = new DateTime(result.current.Value.Year, result.current.Value.Month, result.current.Value.Day, 23, 59, 59);
            } else if (ARSummaryPeriod.C_AR_SUMMARY_PERIOD_THISWEEK.Equals(summaryPeriod)) {
                int dayOfWeek = (int)DateTime.Now.DayOfWeek;
                temp = DateTime.Now.AddDays(1.0 - dayOfWeek);
                result.dateFrom = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);
                result.dateTo = new DateTime(result.current.Value.Year, result.current.Value.Month, result.current.Value.Day, 23, 59, 59);
            } else if (ARSummaryPeriod.C_AR_SUMMARY_PERIOD_THISMONTH.Equals(summaryPeriod)) {
                temp = CommonUtil.FirstDayOfMonthFromDateTime(result.current.Value.Month, result.current.Value.Year);
                result.dateFrom = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);
                result.dateTo = new DateTime(result.current.Value.Year, result.current.Value.Month, result.current.Value.Day, 23, 59, 59);
            } else if (ARSummaryPeriod.C_AR_SUMMARY_PERIOD_LASTWEEK.Equals(summaryPeriod)) {
                int dayOfWeek = (int)DateTime.Now.DayOfWeek;
                temp = DateTime.Now.AddDays(-6.0 - dayOfWeek); //(1.0 - dayOfWeek - 7)
                result.dateFrom = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);
                temp = DateTime.Now.AddDays(0 - dayOfWeek);
                result.dateTo = new DateTime(temp.Year, temp.Month, temp.Day, 23, 59, 59);
            } else if (ARSummaryPeriod.C_AR_SUMMARY_PERIOD_LASTMONTH.Equals(summaryPeriod)) {
                temp = CommonUtil.FirstDayOfMonthFromDateTime(result.current.Value.Month - 1, result.current.Value.Year);
                result.dateFrom = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);
                temp = CommonUtil.LastDayOfMonthFromDateTime(result.current.Value.Month - 1, result.current.Value.Year);
                result.dateTo = new DateTime(temp.Year, temp.Month, temp.Day, 23, 59, 59);
            }

            return result;
        }
    }
}
