//*********************************
// Create by: Non A.
// Create date: 12/Jan/2015
// Update date: 12/Jan/2015
//*********************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Inventory.Models;

namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {
        #region Authority

        /// <summary>
        /// Checking user's permission.
        /// </summary>
        /// <param name="param">Screen's parameter.</param>
        /// <returns>Return ActionResult of the screen.</returns>
        public ActionResult IVS282_Authority(IVS282_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {

                ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (srvCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    //res.ResultData = MessageUtil.MessageList.MSG0049.ToString();
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_IN_STOCK_REPORT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    //res.ResultData = MessageUtil.MessageList.MSG0053.ToString();
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS282_ScreenParameter>("IVS282", param, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initialize screen.
        /// </summary>
        /// <returns>Return ActionResult of the screen.</returns>
        [Initialize("IVS282")]
        public ActionResult IVS282()
        {
            return View();
        }

        public ActionResult IVS282_InitialSearchResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS282_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        public ActionResult IVS282_InitialDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS282_Detail", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        public ActionResult IVS282_SearchSlip(doIVS282SearchCondition searchParam)
        {
            IVS282_ScreenParameter param = GetScreenObject<IVS282_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (searchParam == null || CommonUtil.IsNullAllField(searchParam))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    var lst = service.GetStockReport_ReturnReport_Header(searchParam);
                    param.LastSearchParam = searchParam;
                    res.ResultData = CommonUtil.ConvertToXml(lst, @"Inventory\IVS282_SearchResult", CommonUtil.GRID_EMPTY_TYPE.VIEW);
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult IVS282_GetDetail(string[] slipNo)
        {
            IVS282_ScreenParameter param = GetScreenObject<IVS282_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (slipNo == null || slipNo.Length == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else if (param.LastSearchParam != null)
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    var lst = service.GetStockReport_ReturnReport_Detail(param.LastSearchParam.ReportType, string.Join(",", slipNo));
                    res.ResultData = CommonUtil.ConvertToXml(lst, @"Inventory\IVS282_Detail", CommonUtil.GRID_EMPTY_TYPE.VIEW);
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult IVS282_GenerateReport(string[] slipNo)
        {
            IVS282_ScreenParameter param = GetScreenObject<IVS282_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (slipNo == null || slipNo.Length == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else if (param.LastSearchParam != null)
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    var lst = service.GetStockReport_ReturnReport_Detail(param.LastSearchParam.ReportType, string.Join(",", slipNo));
                    IInventoryDocumentHandler docService = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                    param.PendingDownloadFilePath = docService.GenerateIVS282ReturnReport(param.LastSearchParam.ReportType, lst);
                    param.PendingDownloadFileName = "ReturnReport.xlsx";
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

        public ActionResult IVS282_GenerateReportSummary(string[] slipNo)
        {
            IVS282_ScreenParameter param = GetScreenObject<IVS282_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (slipNo == null || slipNo.Length == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else if (param.LastSearchParam != null)
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    var lst = service.GetStockReport_ReturnReport_Detail(param.LastSearchParam.ReportType, string.Join(",", slipNo));
                    IInventoryDocumentHandler docService = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                    param.PendingDownloadFilePath = docService.GenerateIVS282ReturnReportSummary(param.LastSearchParam.ReportType, lst);
                    param.PendingDownloadFileName = "ReturnReportSummary.xlsx";
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

        public ActionResult IVS282_Download()
        {
            IVS282_ScreenParameter param = GetScreenObject<IVS282_ScreenParameter>();
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
        #endregion

        #region Method

        #endregion
    }
}
