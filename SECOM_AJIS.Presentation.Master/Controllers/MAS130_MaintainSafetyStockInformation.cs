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
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Master.Models;

namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of MAS130
        /// </summary>
        /// <param name="screenParam"></param>
        /// <returns></returns>
        public ActionResult MAS130_Authority(MAS130_ScreenParameter screenParam)
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

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                    ))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                return InitialScreenEnvironment<MAS130_ScreenParameter>("MAS130", screenParam, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen MAS130
        /// </summary>
        /// <returns></returns>
        [Initialize("MAS130")]
        public ActionResult MAS130()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ViewBag.HasPermissionAdd = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO, FunctionID.C_FUNC_ID_ADD);
                ViewBag.HasPermissionEdit = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO, FunctionID.C_FUNC_ID_EDIT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return View();
        }

        /// <summary>
        /// Initial grid for MAS130
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS130_InitGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS130"));
        }

        /// <summary>
        /// Get instrument data
        /// </summary>
        /// <param name="InstrumentCodeSearch"></param>
        /// <returns></returns>
        public ActionResult MAS130_GetInstrumentName(string InstrumentCodeSearch)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<tbm_Instrument> list = hand.GetTbm_Instrument(InstrumentCodeSearch);

                string InstumentName = "";
                if (list.Count > 0)
                {
                    InstumentName = list[0].InstrumentName;
                }

                res.ResultData = InstumentName;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Search safety stock ata
        /// </summary>
        /// <param name="InstrumentCodeSearch"></param>
        /// <returns></returns>
        public ActionResult MAS130_Search(string InstrumentCodeSearch)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                InstrumentCodeSearch = (InstrumentCodeSearch == "" ? null : InstrumentCodeSearch);

                if (InstrumentCodeSearch == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                ISafetyStockMasterHandler hand = ServiceContainer.GetService<ISafetyStockMasterHandler>() as ISafetyStockMasterHandler;
                List<doSafetyStock> list = hand.GetSafetyStock(InstrumentCodeSearch);

                string xml = CommonUtil.ConvertToXml<doSafetyStock>(list, "Master\\MAS130", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Search safety stock detail data
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <returns></returns>
        public ActionResult MAS130_SearchDetail(string InstrumentCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ISafetyStockMasterHandler hand = ServiceContainer.GetService<ISafetyStockMasterHandler>() as ISafetyStockMasterHandler;
                List<doSafetyStock> list = hand.GetSafetyStock(InstrumentCode);

                doSafetyStock data = null;
                if (list.Count > 0)
                {
                    data = list[0];

                    MAS130_ScreenParameter MAS130Param = GetScreenObject<MAS130_ScreenParameter>();
                    MAS130Param.currentSafeStock = CommonUtil.CloneObject<doSafetyStock, tbm_SafetyStock>(data);
                    
                }

                res.ResultData = data;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Insert safety stock
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult MAS130_Insert(MAS130_SafetyStockData data)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                    ))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                // Check required field.
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)                        
                        return Json(res);
                }

                data.CreateBy = data.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                data.CreateDate = data.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                ISafetyStockMasterHandler hand = ServiceContainer.GetService<ISafetyStockMasterHandler>() as ISafetyStockMasterHandler;

                // Check exist in tbm_SaftyStock
                List<doSafetyStock> listSafetyStock = hand.GetSafetyStock(data.InstrumentCode);
                if (listSafetyStock.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1037);
                    return Json(res);
                }


                List<tbm_SafetyStock> list = hand.InsertSafetyStock(data);

                tbm_SafetyStock result = null;
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
                return Json(res);
            }
        }

        /// <summary>
        /// Update safety stock
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult MAS130_Update(MAS130_SafetyStockData data)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                    ))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                // Check required field.
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                tbm_SafetyStock curSafetystock = GetScreenObject<MAS130_ScreenParameter>().currentSafeStock;

                ISafetyStockMasterHandler hand = ServiceContainer.GetService<ISafetyStockMasterHandler>() as ISafetyStockMasterHandler;
                List<doSafetyStock> oldSafetystock = hand.GetSafetyStock(data.InstrumentCode);

                if (!(oldSafetystock.Count > 0) || DateTime.Compare(oldSafetystock[0].UpdateDate.Value, curSafetystock.UpdateDate.Value) != 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    return Json(res);
                }

                curSafetystock.InventoryFixedQuantity = data.InventoryFixedQuantity;
                curSafetystock.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                curSafetystock.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                List<tbm_SafetyStock> list = hand.UpdateSafetyStock(curSafetystock);

                string result = (list.Count > 0) ? "1" : null;
                
                res.ResultData = result;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
    }
}

