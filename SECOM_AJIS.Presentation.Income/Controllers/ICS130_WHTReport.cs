
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
        public ActionResult ICS130_Authority(ICS130_ScreenParameter param)
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

            return InitialScreenEnvironment<ICS130_ScreenParameter>("ICS130", param, res);

        }

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("ICS130")]
        public ActionResult ICS130()
        {
            return View();
        }

        public ActionResult ICS130_InitialSearchResultIMS()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS130_IMS", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        public ActionResult ICS130_InitialSearchResultAccount()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Income\\ICS130_Account", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        public ActionResult ICS130_SearchWHTReportForAccount(DateTime period)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var param = this.GetScreenObject<ICS130_ScreenParameter>();

                var list = hand.GetWHTReportForAccount(period, period);
                if (list != null & list.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage(list);
                }

                param.CachedReportType = ICS130_ScreenParameter.eCacheReportType.Account;
                param.CachedPeriodFrom = period;
                param.CachedPeriodTo = period;
                param.CachedIMSReport = null;
                param.CachedAccountReport = list;

                res.ResultData = CommonUtil.ConvertToXml(list, "Income\\ICS130_Account", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS130_SearchWHTReportForIMS(DateTime periodFrom, DateTime periodTo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var param = this.GetScreenObject<ICS130_ScreenParameter>();

                var list = hand.GetWHTReportForIMS(periodFrom, periodTo);
                if (list != null & list.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage(list);
                }

                param.CachedReportType = ICS130_ScreenParameter.eCacheReportType.IMS;
                param.CachedPeriodFrom = periodFrom;
                param.CachedPeriodTo = periodTo;
                param.CachedIMSReport = list;
                param.CachedAccountReport = null;

                res.ResultData = CommonUtil.ConvertToXml(list, "Income\\ICS130_IMS", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ICS130_ValidateDownloadReport()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var param = this.GetScreenObject<ICS130_ScreenParameter>();

                string reportPath = "", reportName = "";

                if (param.CachedReportType == ICS130_ScreenParameter.eCacheReportType.Account)
                {
                    if (param.CachedAccountReport == null)
                    {
                        ObjectResultData re = new ObjectResultData();
                        re.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        re.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS130"
                           , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0018);
                        return Json(re);
                    }
                    else
                    {
                        IIncomeDocumentHandler docService = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;
                        reportPath = docService.GenerateICS130ForAccount(param.CachedAccountReport, param.CachedPeriodFrom);
                        reportName = "WHTReport_ACC.xlsx";
                    }
                }
                else if (param.CachedReportType == ICS130_ScreenParameter.eCacheReportType.IMS)
                {
                    if (param.CachedIMSReport == null)
                    {
                        // add by jirawat jannet on 2016-11-08
                        ObjectResultData re = new ObjectResultData();
                        re.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        re.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS130"
                           , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0018);
                        return Json(re);
                        //throw new HttpException(404, "NotFound");
                    }
                    else
                    {
                        IIncomeDocumentHandler docService = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;
                        reportPath = docService.GenerateICS130ForIMS(param.CachedIMSReport, param.CachedPeriodFrom, param.CachedPeriodTo);
                        reportName = "WHTReport_IMS.xlsx";
                    }

                }

                if (string.IsNullOrEmpty(reportPath) || !System.IO.File.Exists(reportPath))
                {
                    // add by jirawat jannet on 2016-11-08
                    ObjectResultData re = new ObjectResultData();
                    re.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    re.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS130"
                       , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0018);
                    return Json(re);
                    //throw new HttpException(404, "NotFound");
                }

                res.ResultData = true;

                return Json(res);
            }
            catch (Exception ex)
            {
                // add by jirawat jannet on 2016-11-08
                ObjectResultData re = new ObjectResultData();
                re.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                re.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS130"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0018);
                return Json(re);
                //throw new HttpException(404, "NotFound", ex);
            }
        }

        public ActionResult ICS130_DownloadReport()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ValidatorUtil validator = new ValidatorUtil();
                var hand = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var param = this.GetScreenObject<ICS130_ScreenParameter>();

                string reportPath = "", reportName = "";

                if (param.CachedReportType == ICS130_ScreenParameter.eCacheReportType.Account)
                {
                    if (param.CachedAccountReport == null)
                    {
                        // add by jirawat jannet on 2016-11-08
                        ObjectResultData re = new ObjectResultData();
                        re.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        re.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS130"
                           , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0018);
                        return Json(re);
                        //throw new HttpException(404, "NotFound");
                    }
                    else
                    {
                        IIncomeDocumentHandler docService = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;
                        reportPath = docService.GenerateICS130ForAccount(param.CachedAccountReport, param.CachedPeriodFrom);
                        reportName = "WHTReport_ACC.xlsx";
                    }
                }
                else if (param.CachedReportType == ICS130_ScreenParameter.eCacheReportType.IMS)
                {
                    if (param.CachedIMSReport == null)
                    {
                        // add by jirawat jannet on 2016-11-08
                        ObjectResultData re = new ObjectResultData();
                        re.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        re.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS130"
                           , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0018);
                        return Json(re);
                        //throw new HttpException(404, "NotFound");
                    }
                    else
                    {
                        IIncomeDocumentHandler docService = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;
                        reportPath = docService.GenerateICS130ForIMS(param.CachedIMSReport, param.CachedPeriodFrom, param.CachedPeriodTo);
                        reportName = "WHTReport_IMS.xlsx";
                    }

                }

                if (string.IsNullOrEmpty(reportPath) || !System.IO.File.Exists(reportPath))
                {
                    // add by jirawat jannet on 2016-11-08
                    ObjectResultData re = new ObjectResultData();
                    re.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    re.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS130"
                       , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0018);
                    return Json(re);
                    //throw new HttpException(404, "NotFound");
                }

                var stream = new FileStream(reportPath, FileMode.Open, FileAccess.Read);
                reportPath = null;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportName);
            }
            catch (Exception ex)
            {
                // add by jirawat jannet on 2016-11-08
                ObjectResultData re = new ObjectResultData();
                re.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                re.AddErrorMessage(MessageUtil.MODULE_INCOME, "ICS130"
                   , MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0018);
                return Json(re);
                //throw new HttpException(404, "NotFound", ex);
            }
        }

    }
}
