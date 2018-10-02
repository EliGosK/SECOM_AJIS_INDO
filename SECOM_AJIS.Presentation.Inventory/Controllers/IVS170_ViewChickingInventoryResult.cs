using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Inventory.Models;
using System.Transactions;
using System.IO;


namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {

        /// <summary>
        /// - Check system suspending.<br />
        /// - Check user permission for screen IVS170.<br />
        /// - Check freezed data.<br />
        /// - Check started stock checking.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS170_Authority(IVS170_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // Is suspend ?
                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check permission
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_COMPARE_CHECKING, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                // Check freezed data
                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                if (handlerInventory.CheckFreezedData() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4002);
                    return Json(res);
                }

                // Check for the stock is started
                if (handlerInventory.CheckStartedStockChecking() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS170_ScreenParameter>("IVS170", param, res);
        }

        /// <summary>
        /// Get config for stock different table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS170_InitialSearchResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS170", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS170")]
        public ActionResult IVS170()
        {
            return View();
        }

        /// <summary>
        /// Search stock checking list.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult IVS170_SearchResponse(IVS170_doGetStockCheckingList cond)
        {

            List<dtStockCheckingList> list = new List<dtStockCheckingList>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                // Model validation
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                //Validate #1
                //if (CommonUtil.IsNullAllField(cond))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                //    return Json(res);
                //}

                //Validate #2
                if ((!CommonUtil.IsNullOrEmpty(cond.ShelfNoFrom))
                    && (!CommonUtil.IsNullOrEmpty(cond.ShelfNoTo))
                    && (String.Compare(cond.ShelfNoFrom, cond.ShelfNoTo) > 0))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4015, null, new string[] { "ShelfNoFrom", "ShelfNoTo" });
                    return Json(res);
                }

                IInventoryHandler handler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                list = handler.GetStockCheckingList(cond);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtStockCheckingList>(list, "Inventory\\IVS170", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(res);


        }


        public ActionResult IVS170_GenerateReport(IVS170_doGetStockCheckingList cond)
        {
            List<dtStockCheckingList> list = new List<dtStockCheckingList>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                // Model validation
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                //Validate #1
                //if (CommonUtil.IsNullAllField(cond))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                //    return Json(res);
                //}

                //Validate #2
                if ((!CommonUtil.IsNullOrEmpty(cond.ShelfNoFrom))
                    && (!CommonUtil.IsNullOrEmpty(cond.ShelfNoTo))
                    && (String.Compare(cond.ShelfNoFrom, cond.ShelfNoTo) > 0))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4015, null, new string[] { "ShelfNoFrom", "ShelfNoTo" });
                    return Json(res);
                }

                IInventoryHandler handler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                list = handler.GetStockCheckingList(cond);

                if (list == null || list.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null, null);
                    return Json(res);
                }

                IVS170_ScreenParameter param = GetScreenObject<IVS170_ScreenParameter>();
                IInventoryDocumentHandler docService = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                param.PendingDownloadFilePath = docService.GenerateIVS170StockTakingResult(list, cond);
                param.PendingDownloadFileName = "StockDifferenceList.xlsx";

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        public ActionResult IVS170_Download()
        {
            IVS170_ScreenParameter param = GetScreenObject<IVS170_ScreenParameter>();
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


