//*********************************
// Create by: Fikree S.
// Create date: 13/Jan/2012
// Update date: 13/Jan/2012
//*********************************

using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Inventory;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Master.Models;

namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of MAS120
        /// </summary>
        /// <param name="screenParam"></param>
        /// <returns></returns>
        public ActionResult MAS120_Authority(MAS120_ScreenParameter screenParam)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                return InitialScreenEnvironment<MAS120_ScreenParameter>("MAS120", screenParam, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen MAS120
        /// </summary>
        /// <returns></returns>
        [Initialize("MAS120")]
        public ActionResult MAS120()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ViewBag.HasPermissionAdd = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_ADD);
                ViewBag.HasPermissionEdit = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_EDIT);
                ViewBag.HasPermissionDelete = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_DEL);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return View();
        }

        /// <summary>
        /// Initial grid for MAS120
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS120_InitGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS120"));
        }

        /// <summary>
        /// Search shelf data
        /// </summary>
        /// <param name="txtShelfNo"></param>
        /// <param name="txtShelfName"></param>
        /// <param name="txtShelfType"></param>
        /// <param name="txtAreaCode"></param>
        /// <returns></returns>
        public ActionResult MAS120_Search(string txtShelfNo, string txtShelfName, string txtShelfType, string txtAreaCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                txtShelfNo = (txtShelfNo == "" ? null : txtShelfNo);
                txtShelfName = (txtShelfName == "" ? null : txtShelfName);
                txtShelfType = (txtShelfType == "" ? null : txtShelfType);
                txtAreaCode = (txtAreaCode == "" ? null : txtAreaCode);

                if (txtShelfNo == null && txtShelfName == null && txtShelfType == null && txtAreaCode == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                IShelfMasterHandler hand = ServiceContainer.GetService<IShelfMasterHandler>() as IShelfMasterHandler;
                List<doShelf> list = hand.GetShelf(txtShelfNo, txtShelfName, txtShelfType, txtAreaCode);

                List<doShelf> tmpList = (from t in list
                                        where t.DeleteFlag != true
                                        select t).ToList<doShelf>();
                foreach(doShelf tmp in tmpList)
                {
                    ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    string strShelfTypeCodeName = comHandler.GetMiscDisplayValue(MiscType.C_INV_SHELF_TYPE, tmp.ShelfTypeCode);
                    tmp.ShelfTypeCodeName = strShelfTypeCodeName;
                    string strAreaCodeName = comHandler.GetMiscDisplayValue(MiscType.C_INV_AREA, tmp.AreaCode);
                    tmp.AreaCodeName = strAreaCodeName;
                }

                string xml = CommonUtil.ConvertToXml<doShelf>(tmpList, "Master\\MAS120", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return null;
            }
        }

        /// <summary>
        /// Get shelf detail data
        /// </summary>
        /// <param name="ShelfNo"></param>
        /// <returns></returns>
        public ActionResult MAS120_GetShelfDetail(string ShelfNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

                IShelfMasterHandler hand = ServiceContainer.GetService<IShelfMasterHandler>() as IShelfMasterHandler;
                ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                doShelf detail = null;
                List<doShelf> list = hand.GetShelf(ShelfNo,null,null,null);
                if (list.Count > 0)
                {
                    detail = list[0];

                    MAS120_ScreenParameter MAS120Param = GetScreenObject<MAS120_ScreenParameter>();
                    if (detail.UpdateDate.HasValue)
                    {
                        MAS120Param.updateDate = detail.UpdateDate.Value;
                    }

                    detail.AreaCodeName = comHandler.GetMiscDisplayValue(MiscType.C_INV_AREA, detail.AreaCode);
                    
                   
                }

                res.ResultData = detail;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return null;
            }
        }

        /// <summary>
        /// Insert shelf data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult MAS120_InsertShelf(MAS120_ShelfData data)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                IShelfMasterHandler hand = ServiceContainer.GetService<IShelfMasterHandler>() as IShelfMasterHandler;
                tbm_Shelf result = new tbm_Shelf();
                ValidatorUtil validator = new ValidatorUtil();
                // Check required field.
                //if (ModelState.IsValid == false)
                //{
                //    ValidatorUtil.BuildErrorMessage(res, this);
                //    if (res.IsError)
                //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    return Json(res);
                //}
                if(CommonUtil.IsNullOrEmpty(data.ShelfNo))
                {
                     validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                                    "MAS120",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "ShelfNo",
                                                    "lblShelfNo",
                                                    "ShelfNo");
                }
                
                if (CommonUtil.IsNullOrEmpty(data.ShelfName))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                                   "MAS120",
                                                   MessageUtil.MODULE_COMMON,
                                                   MessageUtil.MessageList.MSG0007,
                                                   "ShelfName",
                                                   "lblShelfName",
                                                   "ShelfName");
                }

                if (CommonUtil.IsNullOrEmpty(data.ShelfTypeCode))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                                   "MAS120",
                                                   MessageUtil.MODULE_COMMON,
                                                   MessageUtil.MessageList.MSG0007,
                                                   "ShelfTypeCode",
                                                   "lblShelfType",
                                                   "ShelfTypeCode");
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);

                if (res.IsError)
                    return Json(res);

                if(hand.CheckDuplicateShelf(data.ShelfNo))
                {
                    List<doShelf> doViewShelf = hand.GetShelf(data.ShelfNo, null, null, null);
                    if (doViewShelf.Count > 0)
                    {
                        if (doViewShelf[0].DeleteFlag == true)
                        {
                            res.ResultData = "ConfirmUpdate";
                            return Json(res);
                            //res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1055);
                            //return Json(res);
                        }
                    }
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1048);
                    return Json(res);
                }

                data.ShelfNo = String.IsNullOrEmpty(data.ShelfNo) == false ? data.ShelfNo.ToUpper() : data.ShelfNo; //Add by Jutarat A. on 28022013

                List<tbm_Shelf> list = hand.InsertShelf(data);
                if (list.Count > 0)
                {
                    result = list[0];
                }

                res.ResultData = result;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return null;
            }
        }

        /// <summary>
        /// Update shelf data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult MAS120_UpdateShelf(MAS120_ShelfData data)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                IShelfMasterHandler hand = ServiceContainer.GetService<IShelfMasterHandler>() as IShelfMasterHandler;
                IInventoryHandler invenHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                tbm_Shelf result = new tbm_Shelf();

                if ((bool)data.DeleteFlag)
                {
                    if (invenHand.IsEmptyShelf(data.ShelfNo) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1049,null,new string[] {""});
                        return Json(res);
                    }
                   
                }
                else 
                {
                    if (data.ShelfTypeCode != ShelfType.C_INV_SHELF_TYPE_NORMAL)
                    {
                        data.InstrumentCode = null;
                        data.AreaCode = null;
                    }
                }

                // Check required field.
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                data.ShelfNo = String.IsNullOrEmpty(data.ShelfNo) == false ? data.ShelfNo.ToUpper() : data.ShelfNo; //Add by Jutarat A. on 28022013

                List<tbm_Shelf> list = hand.UpdateShelf(data);
                if (list.Count > 0)
                {
                    result = list[0];
                }

                res.ResultData = result;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return null;
            }
        }

        /// <summary>
        /// Delete shelf data
        /// </summary>
        /// <param name="ShelfNo"></param>
        /// <returns></returns>
        public ActionResult MAS120_DeleteShelf(string ShelfNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SHELF_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                tbm_Shelf data = new tbm_Shelf();
                data.ShelfNo = ShelfNo;
                data.DeleteFlag = true;

                IShelfMasterHandler hand = ServiceContainer.GetService<IShelfMasterHandler>() as IShelfMasterHandler;

                // Check required field.
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                List<tbm_Shelf> list = hand.UpdateShelf(data);

                string result = string.Empty;
                if (list.Count == 0)
                {
                    result = "0";
                }
                else
                {
                    result = "1";
                }

                res.ResultData = result;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return null;
            }
        }

        /// <summary>
        /// Check shelf data exist
        /// </summary>
        /// <param name="ShelfNo"></param>
        /// <returns></returns>
        public ActionResult MAS120_CheckShelfNo(string ShelfNo) 
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInventoryHandler invenHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                res.ResultData = invenHand.IsEmptyShelf(ShelfNo);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return null;
            }
        }

    }
}

