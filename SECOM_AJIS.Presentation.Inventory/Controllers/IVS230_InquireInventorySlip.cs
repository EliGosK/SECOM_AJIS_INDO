//*********************************
// Create by: Natthavat S.
// Create date: 02/FEB/2012
// Update date: 02/FEB/2012
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Transactions;

using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Presentation.Inventory.Models;
using SECOM_AJIS.DataEntity.Master;

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
        public ActionResult IVS230_Authority(IVS230_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {

                ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //if (srvCommon.IsSystemSuspending())
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                //    return Json(res);
                //}

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_INQUIRE_TRANSFER, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                if ((from p in CommonUtil.dsTransData.dtOfficeData where p.FunctionLogistic != "0" select p).Count() == 0) 
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4120);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS230_ScreenParameter>("IVS230", param, res);
        }

        #endregion

        #region Action
        /// <summary>
        /// Initialize screen.
        /// </summary>
        /// <returns>Return ActionResult of the screen.</returns>
        [Initialize("IVS230")]
        public ActionResult IVS230()
        {
            return View();
        }

        /// <summary>
        /// Get data for initialize search result grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for initialize search result grid.</returns>
        public ActionResult IVS230_InitialSearchGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS230_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get data for initialize detail grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for initialize detail grid.</returns>
        public ActionResult IVS230_InitialSlipDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS230_SlipDetailResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Search inventory slip data.
        /// </summary>
        /// <param name="param">DO of searching parameter</param>
        /// <returns>Return ActionResult of JSON data contains inventory slip data.</returns>
        public ActionResult IVS230_GetInventorySlipIVS230(doGetInventorySlipIVS230 param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                // Akat K. modify
                if (CommonUtil.IsNullAllField(param, "OfficeCode") && CommonUtil.IsNullOrEmpty(param.cboSearchCreateOffice))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                    var lst = service.GetInventorySlipIVS230(param);
                    res.ResultData = CommonUtil.ConvertToXml<dtResultInventorySlipIVS230>(lst, @"Inventory\IVS230_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get inventory slip detail data.
        /// </summary>
        /// <param name="strInventorySlipNo">Inventory Slip No.</param>
        /// <returns>Return ActionResult of JSON data contains inventory slip detail data.</returns>
        public ActionResult IVS230_GetInventorySlipDetail(string strInventorySlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                var lst = service.GetInventorySlipDetail(strInventorySlipNo);
                res.ResultData = CommonUtil.ConvertToXml<dtResultInventorySlipDetail>(lst, @"Inventory\IVS230_SlipDetailResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Check exist file before download file
        /// </summary>
        /// <param name="strDocumentNo"></param>
        /// <param name="documentOCC"></param>
        /// <param name="strDocumentCode"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult IVS230_CheckExistFile(string inventorySlipNo)
        {
            IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            try
            {
                //List<dtDocumentData> list = handler.GetDocumentDataList(new doDocumentDataCondition() { DocumentNo = inventorySlipNo }, false);
                List<dtDocumentData> list = handler.GetDocumentDataListByInventorySlipNo(inventorySlipNo);
                if (list != null && list.Count > 0)
                {
                    string path = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, list[0].FilePath);// ReportUtil.GetGeneratedReportPath(fileName);

                    if (System.IO.File.Exists(path) == true)
                    {
                        return Json(1);
                    }
                    else
                    {
                        return Json(0);
                    }
                }
                else
                {
                    return Json(0);
                }

            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Download document (PDF)
        /// </summary>
        /// <param name="inventorySlipNo"></param>
        /// <returns></returns>
        public ActionResult IVS230_DownloadInventorySlip(string inventorySlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            try
            {
                //List<dtDocumentData> list = handler.GetDocumentDataList(new doDocumentDataCondition() { DocumentNo = inventorySlipNo });
                List<dtDocumentData> list = handler.GetDocumentDataListByInventorySlipNo(inventorySlipNo);
                if (list != null && list.Count > 0)
                {
                    Stream reportStream = handler.GetDocumentReportFileStream(list[0].FilePath);
                    return File(reportStream, "application/octet-stream", Path.GetFileName(list[0].FilePath));
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Method

        #endregion
    }
}
