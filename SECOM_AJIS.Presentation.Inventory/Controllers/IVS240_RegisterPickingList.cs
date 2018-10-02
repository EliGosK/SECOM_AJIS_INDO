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
        public ActionResult IVS240_Authority(IVS240_ScreenParameter param)
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

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_PICKING_LIST, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    //res.ResultData = MessageUtil.MessageList.MSG0053.ToString();
                    return Json(res);
                }

                IInventoryHandler srvInv = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                if (srvInv.CheckFreezedData() == 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4002);
                    //res.ResultData = MessageUtil.MessageList.MSG4002.ToString();
                    return Json(res);
                }

                if (srvInv.CheckStartedStockChecking() == 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    //res.ResultData = MessageUtil.MessageList.MSG4003.ToString();
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS240_ScreenParameter>("IVS240", param, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initialize screen.
        /// </summary>
        /// <returns>Return ActionResult of the screen.</returns>
        [Initialize("IVS240")]
        public ActionResult IVS240()
        {
            return View();
        }

        /// <summary>
        /// Get data for initialize search result grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for intialize search result grid.</returns>
        public ActionResult IVS240_InitialSearchGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS240_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get data for initialize detail grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON dat for initialize detail grid.</returns>
        public ActionResult IVS240_InitialDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS240_DetailResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Search installation slip for register picking list.
        /// </summary>
        /// <param name="param">DO of searching parameter.</param>
        /// <returns>Return ActionResult of JSON data for search result grid.</returns>
        public ActionResult IVS240_SearchInstallationSlip(doSearchInstallationSlipCond param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (CommonUtil.IsNullAllField(param, "OperationOfficeCode", "OperationOfficeCodeList"))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                    if (!string.IsNullOrEmpty(param.ContractCode))
                    {
                        CommonUtil cmm = new CommonUtil();
                        param.ContractCode = cmm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }

                    var lst = service.SearchInstallationSlip(param);
                    res.ResultData = CommonUtil.ConvertToXml<dtSearchInstallationSlipResult>(lst, @"Inventory\IVS240_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Get stock-out data of installatio slip.
        /// </summary>
        /// <param name="strInstallationSlipNo">Installation Slip No.</param>
        /// <returns>Return ActionResult of JSON data contains installation slip's stock-out data.</returns>
        public ActionResult IVS240_GetStockOutByInstallationSlip(string strInstallationSlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                var lst = service.GetStockOutByInstallationSlip(strInstallationSlipNo);
                if (lst.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4065);
                    res.ResultData = this.IVS240_GetStockOutByInstallationSlip_CreateResult(false, null);
                }
                else
                {
                    res.ResultData = this.IVS240_GetStockOutByInstallationSlip_CreateResult(true, lst); ;
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
        /// Registering picking list.
        /// </summary>
        /// <param name="lstInstallationSlipNo">List of installation slip no. for registering.</param>
        /// <returns>Return ActionResult of registration process result.</returns>
        public ActionResult IVS240_RegisterPickingList(List<string> lstInstallationSlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (srvCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.ResultData = this.IVS240_RegisterPickingList_CreateResult(false, null);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_PICKING_LIST, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    res.ResultData = this.IVS240_RegisterPickingList_CreateResult(false, null);
                    return Json(res);
                }

                if (lstInstallationSlipNo == null || lstInstallationSlipNo.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4063);
                    res.ResultData = this.IVS240_RegisterPickingList_CreateResult(false, null);
                    return Json(res);
                }

                IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<string> lstError = new List<string>();

                foreach (var strInstallationSlipNo in lstInstallationSlipNo)
                {
                    var lstTempInvSlip = service.GetTbt_InventorySlip(null, strInstallationSlipNo);
                    //if (lstInvSlip.Count > 0 && lstInvSlip[0].PickingListNo != null)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4064
                    //        , new string[] { strInstallationSlipNo });
                    //    lstError.Add(strInstallationSlipNo);
                    //}

                    if (lstTempInvSlip.Any(p => p.PickingListNo != null))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4064
                            , new string[] { strInstallationSlipNo });
                        lstError.Add(strInstallationSlipNo);
                    }

                }

                if (lstError.Count > 0)
                {
                    res.ResultData = this.IVS240_RegisterPickingList_CreateResult(false, lstError); ;
                }
                else
                {
                    res.ResultData = this.IVS240_RegisterPickingList_CreateResult(true, null); ;
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                res.ResultData = this.IVS240_RegisterPickingList_CreateResult(false, null);
            }

            return Json(res);
        }

        /// <summary>
        /// Confirming picking list registration.
        /// </summary>
        /// <param name="lstInstallationSlipNo">List of installation slip no. for registering.</param>
        /// <returns>Return ActionResult of confirmation process result.</returns>
        public ActionResult IVS240_ConfirmPickingList(List<string> lstInstallationSlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION; //All message during confirmation process must be information dialog box.

            try
            {
                IVS240_ScreenParameter sParam = GetScreenObject<IVS240_ScreenParameter>(); //Add by Jutarat A. on 04122012

                ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (srvCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.ResultData = this.IVS240_ConfirmPickingList_CreateResult(false, null, null);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_PICKING_LIST, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    res.ResultData = this.IVS240_ConfirmPickingList_CreateResult(false, null, null);
                    return Json(res);
                }

                if (lstInstallationSlipNo == null || lstInstallationSlipNo.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4063);
                    res.ResultData = this.IVS240_ConfirmPickingList_CreateResult(false, null, null);
                    return Json(res);
                }

                IInventoryHandler srvInv = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<string> lstError = new List<string>();

                List<tbt_InventorySlip> lstInvSlip = new List<tbt_InventorySlip>();
                foreach (var strInstallationSlipNo in lstInstallationSlipNo)
                {
                    var lstTempInvSlip = srvInv.GetTbt_InventorySlip(null, strInstallationSlipNo);
                    //if (lstInvSlip.Count > 0 && lstInvSlip[0].PickingListNo != null)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4064
                    //        , new string[] { strInstallationSlipNo });
                    //    lstError.Add(strInstallationSlipNo);
                    //}
                    
                    if (lstTempInvSlip.Any(p => p.PickingListNo != null)) 
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4064
                            , new string[] { strInstallationSlipNo });
                        lstError.Add(strInstallationSlipNo);
                        res.ResultData = this.IVS240_ConfirmPickingList_CreateResult(false, lstError, null);
                        return Json(res);
                    }

                    lstInvSlip.AddRange(lstTempInvSlip);
                }

                //if (lstError.Count > 0)
                //{
                //    res.ResultData = this.IVS240_ConfirmPickingList_CreateResult(false, lstError, null);
                //    return Json(res);
                //}

                string strPickingListNo = srvInv.GeneratePickingListNo();

                #region //R2
                foreach (var slip in lstInvSlip)
                {
                    slip.PickingListNo = strPickingListNo;

                    //Comment by Jutarat A. on 30052013 (Set at UpdateTbt_InventorySlip())
                    //slip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //slip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //End Comment
                }
                #endregion

                srvInv.UpdateTbt_InventorySlip(lstInvSlip);
                
                IInventoryDocumentHandler srvInvDoc = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                
                //srvInvDoc.GenerateIVR170FilePath(strPickingListNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                sParam.ResultStream = srvInvDoc.GenerateIVR170(strPickingListNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime); //Modify by Jutarat A. on 04122012

                res.ResultData = this.IVS240_ConfirmPickingList_CreateResult(true, null, strPickingListNo);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                res.ResultData = this.IVS240_ConfirmPickingList_CreateResult(false, null, null); ;
                return Json(res);
            }

        }

        /// <summary>
        /// Get document's data for downloading.
        /// </summary>
        /// <param name="strPickingListNo">Picking List No.</param>
        /// <returns>Return ActionResult of document's data.</returns>
        public ActionResult IVS240_DownloadPickingList(string strPickingListNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS240_ScreenParameter sParam = GetScreenObject<IVS240_ScreenParameter>(); //Add by Jutarat A. on 04122012

                //var docCond = new doDocumentDataCondition()
                //{
                //    DocumentNo = strPickingListNo,
                //    OCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT,
                //    DocumentCode = ReportID.C_INV_REPORT_ID_PICKING_LIST
                //};
                //IDocumentHandler docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                //var lstDocs = docHandler.GetDocumentDataList(docCond);

                var srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var lstDocs = srvCommon.GetTbt_DocumentList(strPickingListNo, ReportID.C_INV_REPORT_ID_PICKING_LIST, ConfigName.C_CONFIG_DOC_OCC_DEFAULT);

                if (lstDocs == null || lstDocs.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                    res.ResultData = null;
                }
                else
                {
                    string path = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, lstDocs[0].FilePath);// ReportUtil.GetGeneratedReportPath(lstDocs[0].FilePath);

                    if (System.IO.File.Exists(path) == true)
                    {
                        res.ResultData = lstDocs[0];
                        sParam.ResultDocument = lstDocs[0]; //Add by Jutarat A. on 04122012
                    }
                    else
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                        res.ResultData = null;
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

        /// <summary>
        /// Download document.
        /// </summary>
        /// <param name="strDocumentNo">Document No.</param>
        /// <param name="documentOCC">Document OCC.</param>
        /// <param name="strDocumentCode">Document Code.</param>
        /// <param name="fileName">File Name.</param>
        /// <returns>Return ActionResult of file's stream.</returns>
        //public ActionResult IVS240_DownloadPdfAndWriteLog(string strDocumentNo, string documentOCC, string strDocumentCode, string fileName)
        public ActionResult IVS240_DownloadPdfAndWriteLog(string k) //Modify by Jutarat A. on 04122012
        {
            Stream reportStream = null; //Add by Jutarat A. on 04122012

            try
            {
                //Modify by Jutarat A. on 04122012
                IVS240_ScreenParameter sParam = GetScreenObject<IVS240_ScreenParameter>(k);
                if (sParam.ResultDocument != null)
                {
                    doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                    {
                        DocumentNo = sParam.ResultDocument.DocumentNo, //strDocumentNo,
                        DocumentCode = sParam.ResultDocument.DocumentCode, //strDocumentCode,
                        DocumentOCC = sParam.ResultDocument.DocumentOCC, //documentOCC,
                        DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo
                    };

                    ILogHandler handlerLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    int isSuccess = handlerLog.WriteDocumentDownloadLog(doDownloadLog);

                    if (sParam.ResultStream != null)
                    {
                        reportStream = sParam.ResultStream;
                        sParam.ResultStream = null;
                    }
                    else
                    {
                        IDocumentHandler handlerDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                        reportStream = handlerDoc.GetDocumentReportFileStream(sParam.ResultDocument.FilePath); //(fileName); 
                    }
                }
                //End Modify

                return File(reportStream, "application/pdf");
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        #endregion

        #region Method
        /// <summary>
        /// Create result data for IVS240_GetStockOutByInstallationSlip controller.
        /// </summary>
        /// <param name="IsSuccess">Boolean, indicate the controller's successful.</param>
        /// <param name="lstStockData">List of stock-out data.</param>
        /// <returns>Object of result data.</returns>
        private object IVS240_GetStockOutByInstallationSlip_CreateResult(bool IsSuccess, List<dtStockOutByInstallationSlipResult> lstStockData)
        {
            return new
            {
                Action = "IVS240_GetStockOutByInstallationSlip",
                IsSuccess = IsSuccess,
                StockData = lstStockData
            };
        }

        /// <summary>
        /// Create result data for IVS240_RegisterPickingList controller.
        /// </summary>
        /// <param name="IsSuccess">Boolean, indicate the controller's successful.</param>
        /// <param name="lstErrorInstallSlipNo">List of error installation slip no.</param>
        /// <returns>Object of result data.</returns>
        private object IVS240_RegisterPickingList_CreateResult(bool IsSuccess, List<string> lstErrorInstallSlipNo)
        {
            return new
            {
                Action = "IVS240_RegisterPickingList",
                IsSuccess = IsSuccess,
                ErrorInstallSlipNo = lstErrorInstallSlipNo
            };
        }

        /// <summary>
        /// Create result data for IVS240_ConfirmPickingList controller.
        /// </summary>
        /// <param name="IsSuccess">Boolean, indicate the controller's successful.</param>
        /// <param name="lstErrorInstallSlipNo">List of error installation slip no.</param>
        /// <param name="strPickingListNo">Picking List No.</param>
        /// <returns>Object of result data.</returns>
        private object IVS240_ConfirmPickingList_CreateResult(bool IsSuccess, List<string> lstErrorInstallSlipNo, string strPickingListNo)
        {
            return new {
                Action = "IVS240_ConfirmPickingList",
                IsSuccess = IsSuccess,
                ErrorInstallSlipNo = lstErrorInstallSlipNo,
                PickingListNo = strPickingListNo
            };
        }

        #endregion
    }
}
