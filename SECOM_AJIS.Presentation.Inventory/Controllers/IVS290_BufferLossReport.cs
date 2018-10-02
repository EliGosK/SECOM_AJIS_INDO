//*********************************
// Create by: Non A.
// Create date: 03/Sep/2015
// Update date: 03/Sep/2015
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
        public ActionResult IVS290_Authority(IVS290_ScreenParameter param)
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

            return InitialScreenEnvironment<IVS290_ScreenParameter>("IVS290", param, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initialize screen.
        /// </summary>
        /// <returns>Return ActionResult of the screen.</returns>
        [Initialize("IVS290")]
        public ActionResult IVS290()
        {
            return View();
        }

        public ActionResult IVS290_InitialSearchResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS290_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        public ActionResult IVS290_InitialDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS290_Detail", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        public ActionResult IVS290_SearchSlip(doIVS290SearchCondition searchParam)
        {
            IVS290_ScreenParameter param = GetScreenObject<IVS290_ScreenParameter>();

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
                    var lst = service.GetStockReport_BufferLoss_Header(searchParam);
                    param.LastSearchParam = searchParam;
                    res.ResultData = CommonUtil.ConvertToXml(lst, @"Inventory\IVS290_SearchResult", CommonUtil.GRID_EMPTY_TYPE.VIEW);
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult IVS290_GetDetail(string[] slipNo)
        {
            IVS290_ScreenParameter param = GetScreenObject<IVS290_ScreenParameter>();

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
                    var lst = service.GetStockReport_BufferLoss_Detail(string.Join(",", slipNo));
                    res.ResultData = CommonUtil.ConvertToXml(lst, @"Inventory\IVS290_Detail", CommonUtil.GRID_EMPTY_TYPE.VIEW);
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult IVS290_GenerateReport(string[] slipNo)
        {
            IVS290_ScreenParameter param = GetScreenObject<IVS290_ScreenParameter>();

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
                    var lst = service.GetStockReport_BufferLoss_Detail(string.Join(",", slipNo));
                    IInventoryDocumentHandler docService = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                    param.PendingDownloadFilePath = docService.GenerateIVS290BufferLossReport(lst);
                    param.PendingDownloadFileName = "BufferLossReport.xlsx";
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

        public ActionResult IVS290_GenerateReportSummary(string[] slipNo)
        {
            IVS290_ScreenParameter param = GetScreenObject<IVS290_ScreenParameter>();

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
                    var lst = service.GetStockReport_BufferLoss_Detail(string.Join(",", slipNo));
                    IInventoryDocumentHandler docService = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                    param.PendingDownloadFilePath = docService.GenerateIVS290BufferLossReportSummary(lst);
                    param.PendingDownloadFileName = "BufferLossReport.xlsx";
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

        public ActionResult IVS290_Download()
        {
            IVS290_ScreenParameter param = GetScreenObject<IVS290_ScreenParameter>();
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
