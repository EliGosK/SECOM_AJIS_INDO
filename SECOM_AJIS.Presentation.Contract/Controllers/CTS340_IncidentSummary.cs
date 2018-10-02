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

        private const string CTS340_Screen = "CTS340";

        /// <summary>
        /// Check user’s permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS340_Authority(CTS340_ScreenParameter param) {
            ObjectResultData res = new ObjectResultData();
            try {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_SUMMARY_INCIDENT)) {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                if (res.IsError) {
                    return Json(res);
                }

                param.hasPermission310 = CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_INCIDENT, FunctionID.C_FUNC_ID_OPERATE);

                return InitialScreenEnvironment<CTS340_ScreenParameter>(CTS340_Screen, param, res);
            } catch (Exception ex) {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize(CTS340_Screen)]
        public ActionResult CTS340() {
            CTS340_ScreenParameter param = GetScreenObject<CTS340_ScreenParameter>();
            ViewBag.HasPermission310 = param.hasPermission310;
            ViewBag.SummaryThisWeek = IncidentSummaryPeriod.C_INCIDENT_SUMMARY_PERIOD_THISWEEK;
            ViewBag.CTS310Caller = ScreenID.C_SCREEN_ID_SUMMARY_INCIDENT;
            return View();
        }

        /// <summary>
        /// Initial grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS340_InitGrid() {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS340"));
        }

        /// <summary>
        /// Search incident by summary period when click [Select] button on Incident summary section
        /// </summary>
        /// <param name="summaryPeriod"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CTS340_searchIncident(string summaryPeriod) {
            ObjectResultData res = new ObjectResultData();
            
            try {
                doSummaryPeriod condition = CTS340_getPeriodDate(summaryPeriod);
                IIncidentHandler hand = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
                List<dtSummaryIncident> summary = hand.SummaryIncident(condition.dateFrom, condition.dateTo, condition.current);
                CommonUtil.MappingObjectLanguage<dtSummaryIncident>(summary);

                string xml = CommonUtil.ConvertToXml<dtSummaryIncident>(summary, "Contract\\CTS340", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        private doSummaryPeriod CTS340_getPeriodDate(string summaryPeriod)
        {
            doSummaryPeriod result = new doSummaryPeriod();
            result.current = DateTime.Now;
            // All
            result.dateFrom = null;
            result.dateTo = null;
            DateTime temp = DateTime.Now;

            if (IncidentSummaryPeriod.C_INCIDENT_SUMMARY_PERIOD_THISWEEK.Equals(summaryPeriod)) {
                int dayOfWeek = (int)DateTime.Now.DayOfWeek;
                temp = DateTime.Now.AddDays(1.0 - dayOfWeek);
                result.dateFrom = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);
                result.dateTo = new DateTime(result.current.Value.Year, result.current.Value.Month, result.current.Value.Day, 23, 59, 59);
            } else if (IncidentSummaryPeriod.C_INCIDENT_SUMMARY_PERIOD_THISMONTH.Equals(summaryPeriod)) {
                temp = CommonUtil.FirstDayOfMonthFromDateTime(result.current.Value.Month, result.current.Value.Year);
                result.dateFrom = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);
                result.dateTo = new DateTime(result.current.Value.Year, result.current.Value.Month, result.current.Value.Day, 23, 59, 59);
            } else if (IncidentSummaryPeriod.C_INCIDENT_SUMMARY_PERIOD_LASTWEEK.Equals(summaryPeriod)) {
                int dayOfWeek = (int)DateTime.Now.DayOfWeek;
                temp = DateTime.Now.AddDays(-6.0 - dayOfWeek); //(1.0 - dayOfWeek - 7)
                result.dateFrom = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);
                temp = DateTime.Now.AddDays(0 - dayOfWeek);
                result.dateTo = new DateTime(temp.Year, temp.Month, temp.Day, 23, 59, 59);
            } else if (IncidentSummaryPeriod.C_INCIDENT_SUMMARY_PERIOD_LASTMONTH.Equals(summaryPeriod)) {
                temp = CommonUtil.FirstDayOfMonthFromDateTime(result.current.Value.Month - 1, result.current.Value.Year);
                result.dateFrom = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);
                temp = CommonUtil.LastDayOfMonthFromDateTime(result.current.Value.Month - 1, result.current.Value.Year);
                result.dateTo = new DateTime(temp.Year, temp.Month, temp.Day, 23, 59, 59);
            } else if (IncidentSummaryPeriod.C_INCIDENT_SUMMARY_PERIOD_TODAY.Equals(summaryPeriod)) {
                result.dateFrom = new DateTime(result.current.Value.Year, result.current.Value.Month, result.current.Value.Day, 0, 0, 0);
                result.dateTo = new DateTime(result.current.Value.Year, result.current.Value.Month, result.current.Value.Day, 23, 59, 59);
            }

            return result;
        }
    }
}
