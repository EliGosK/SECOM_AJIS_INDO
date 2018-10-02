
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
using SECOM_AJIS.Presentation.Installation.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Helpers;

using System.ComponentModel;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using SECOM_AJIS.Common.Models.EmailTemplates;

namespace SECOM_AJIS.Presentation.Installation.Controllers
{
    public partial class InstallationController : BaseController
    {
        /// <summary>
        /// Authority screen ISS100
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ISS100_Authority(ISS100_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();


            if (CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_REPORT, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            return InitialScreenEnvironment<ISS100_ScreenParameter>("ISS100", param, res);

        }


        /// <summary>
        /// Initial screen ISS100
        /// </summary>
        /// <returns></returns>
        [Initialize("ISS100")]
        public ActionResult ISS100()
        {

            ISS100_ScreenParameter param = GetScreenObject<ISS100_ScreenParameter>();

            return View();
        }

        public ActionResult ISS100_ExportExcelData(doInstallationReport dtInstallation)
        {
            ISS100_ScreenParameter param = GetScreenObject<ISS100_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (!this.ModelState.IsValid)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                if (dtInstallation == null || CommonUtil.IsNullAllField(dtInstallation))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else
                {
                    IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    var lst = handler.GetInstallationReportExcelFile(dtInstallation);

                    if (lst == null || lst.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }

                    IInstallationDocumentHandler docService = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
                    param.PendingDownloadFilePath = docService.GenerateISR120Report(lst, dtInstallation);
                    param.PendingDownloadFileName = "ISR120Report.xlsx";
                    res.ResultData = true;
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ISS100_ExportExcelDataMonthly(doInstallationReportMonthly dtInstallation)
        {
            ISS100_ScreenParameter param = GetScreenObject<ISS100_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (!this.ModelState.IsValid)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                if (dtInstallation == null || CommonUtil.IsNullAllField(dtInstallation))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else
                {
                    IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    var lst = handler.GetInstallationReportMonthlyExcelFile(dtInstallation);

                    if (lst == null || lst.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }

                    IInstallationDocumentHandler docService = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

                    if (dtInstallation.ReportType == "0")
                    {

                        param.PendingDownloadFilePath = docService.GenerateISR130Report(lst, dtInstallation);
                        param.PendingDownloadFileName = "ISR130NewReport.xlsx";
                    }
                    else
                    {
                        param.PendingDownloadFilePath = docService.GenerateISR140Report(lst, dtInstallation);
                        param.PendingDownloadFileName = "ISR140ClaimReport.xlsx";
                    }
                    res.ResultData = true;
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        
        public ActionResult ISS100_Download()
        {
            ISS100_ScreenParameter param = GetScreenObject<ISS100_ScreenParameter>();
            if (!string.IsNullOrEmpty(param.PendingDownloadFilePath))
            {
                var stream = new FileStream(param.PendingDownloadFilePath, FileMode.Open, FileAccess.Read);
                param.PendingDownloadFilePath = null;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", param.PendingDownloadFileName);
            }
            else
            {
                ObjectResultData res = new ObjectResultData();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(new FileNotFoundException("Report file not found.", param.PendingDownloadFilePath));
                return Json(res);
            }
        }

    }
}