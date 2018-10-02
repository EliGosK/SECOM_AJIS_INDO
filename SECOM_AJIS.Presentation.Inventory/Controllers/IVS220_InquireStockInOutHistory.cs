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
        public ActionResult IVS220_Authority(IVS220_ScreenParameter param)
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

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_INQUIRE_IN_OUT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS220_ScreenParameter>("IVS220", param, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initialize screen.
        /// </summary>
        /// <returns>Return ActionResult of the screen.</returns>
        [Initialize("IVS220")]
        public ActionResult IVS220()
        {
            ViewBag.HeadOfficeCode = this.IVS220_GetHeadOffice();
            return View();
        }

        /// <summary>
        /// Get data for intialize search result grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for initialize search result grid.</returns>
        public ActionResult IVS220_InitialSearchGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS220_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Search stock-in/stock-out history.
        /// </summary>
        /// <param name="param">DO of searching parameter.</param>
        /// <returns>Return ActionResult of JSON data for search result grid.</returns>
        public ActionResult IVS220_GetIVS220(doGetIVS220 param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (CommonUtil.IsNullAllField(param, "LocationCode"))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                    var lst = service.GetIVS220(param);
                    res.ResultData = CommonUtil.ConvertToXml<dtResultIVS220>(lst, @"Inventory\IVS220_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //public ActionResult IVS220_CheckHeadOffice(string strOfficeCode)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
        //
        //    try
        //    {
        //        IOfficeMasterHandler srvOffice = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
        //        res.ResultData = srvOffice.CheckHeadOffice(strOfficeCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
        //        res.AddErrorMessage(ex);
        //    }
        //
        //    return Json(res);
        //}

        
        /// <summary>
        /// Check exist file before download file
        /// </summary>
        /// <param name="strDocumentNo"></param>
        /// <param name="documentOCC"></param>
        /// <param name="strDocumentCode"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult IVS220_CheckExistFile(string inventorySlipNo)
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
        public ActionResult IVS220_DownloadInventorySlip(string inventorySlipNo)
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

        public ActionResult IVS220_PrepareDownloadPO(string strPurchaseOrderNo, string strReportID)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS220_ScreenParameter prm = GetScreenObject<IVS220_ScreenParameter>();
                var srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var lstDocs = srvCommon.GetTbt_DocumentList(strPurchaseOrderNo, strReportID).OrderByDescending(d => d.DocumentOCC).FirstOrDefault();

                if (lstDocs == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                    res.ResultData = false;
                }
                else
                {
                    string path = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, lstDocs.FilePath);  //ReportUtil.GetGeneratedReportPath(lstDocs.FilePath);

                    if (System.IO.File.Exists(path) == true)
                    {
                        prm.PreparedDownloadPO = lstDocs;
                        res.ResultData = true;
                    }
                    else
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                        res.ResultData = false;
                    }
                }
                return Json(res);

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        public ActionResult IVS220_DownloadPreparedPO()
        {
            try
            {
                IVS220_ScreenParameter prm = GetScreenObject<IVS220_ScreenParameter>();
                if (prm.PreparedDownloadPO == null)
                {
                    return HttpNotFound();
                }

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = prm.PreparedDownloadPO.DocumentNo,
                    DocumentCode = prm.PreparedDownloadPO.DocumentCode,
                    DocumentOCC = prm.PreparedDownloadPO.DocumentOCC,
                    DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo
                };

                ILogHandler handlerLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                int isSuccess = handlerLog.WriteDocumentDownloadLog(doDownloadLog);

                IDocumentHandler handlerDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                Stream reportStream = handlerDoc.GetDocumentReportFileStream(prm.PreparedDownloadPO.FilePath);

                prm.PreparedDownloadPO = null;

                return File(reportStream, "application/pdf");
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }

        #endregion

        #region Method

        /// <summary>
        /// Get inventory head office.
        /// </summary>
        /// <returns>Return inventory head office code.</returns>
        private string IVS220_GetHeadOffice()
        {
            var srvInv = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            var lstOffice = srvInv.GetInventoryHeadOffice();
            if (lstOffice != null && lstOffice.Count > 0)
            {
                return lstOffice[0].OfficeCode;
            }
            else
            {
                throw new ApplicationException("Unable to get inventory head office data.");
            }
        }

        #endregion
    }
}
