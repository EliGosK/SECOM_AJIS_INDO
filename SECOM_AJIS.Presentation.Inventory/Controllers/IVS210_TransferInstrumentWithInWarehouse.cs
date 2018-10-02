


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
        #region Authority
        /// <summary>
        /// Check screen permission
        /// </summary>
        /// <param name="param">ScreenParameter</param>
        /// <returns></returns>
        public ActionResult IVS210_Authority(IVS210_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInventoryHandler handInven = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                if (comh.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_MOVE_SHELF, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                if (handInven.CheckFreezedData() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4002);
                    return Json(res);
                }
                if (handInven.CheckStartedStockChecking() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }

                List<doOffice> IvHeadOffice = handInven.GetInventoryHeadOffice();
                if (IvHeadOffice.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage<doOffice>(IvHeadOffice);
                    param.Office = IvHeadOffice[0];
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4016);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS210_ScreenParameter>("IVS210", param, res);
        }

        /// <summary>
        /// Initial Screen
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS210")]
        public ActionResult IVS210()
        {
            IVS210_ScreenParameter param = GetScreenObject<IVS210_ScreenParameter>();
            CommonUtil.MappingObjectLanguage<doOffice>(new List<doOffice>() { param.Office });
            ViewBag.OfficeName = param.Office.OfficeName;
            ViewBag.OfficeCode = param.Office.OfficeCode;

            tbs_MiscellaneousTypeCode m = IVS210_GetLocation();

            ViewBag.Location = m.ValueDisplay;
            ViewBag.LocationCode = m.ValueCode;

            return View();
        }
        #endregion

        /// <summary>
        /// Search instrument information
        /// </summary>
        /// <param name="Cond">Search condition object</param>
        /// <returns></returns>
        public ActionResult IVS210_SearchInventoryInstrument(IVS210SearchCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {    //Valid Cond

                ValidatorUtil.BuildErrorMessage(res, this, new object[] { Cond });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                if (!string.IsNullOrEmpty(Cond.ShelfNoFrom) && !string.IsNullOrEmpty(Cond.ShelfNoTo)
                    && Cond.ShelfNoFrom.CompareTo(Cond.ShelfNoTo) > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4015, null, new string[] { "ShelfNoFrom", "ShelfNoTo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                IVS210_ScreenParameter param = GetScreenObject<IVS210_ScreenParameter>();

                doSearchInstrumentListCondition searchCon = new doSearchInstrumentListCondition();
                searchCon.OfficeCode = Cond.Office;
                searchCon.LocationCode = Cond.Location;
                searchCon.AreaCode = Cond.InstArea;
                searchCon.StartShelfNo = Cond.ShelfNoFrom;
                searchCon.EndShelfNo = Cond.ShelfNoTo;
                searchCon.Instrumentcode = Cond.InstCode;
                searchCon.InstrumentName = Cond.InstName;

                List<dtSearchInstrumentListResult> lstResult = InvH.SearchInventoryInstrumentListAllShelf(searchCon);

                if (lstResult.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                else if (lstResult.Count > 1000)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4004);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                res.ResultData = CommonUtil.ConvertToXml<dtSearchInstrumentListResult>(lstResult, "inventory\\IVS210_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Initial instrument grid control
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS210_InstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS210_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial transfer instrument grid control
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS210_TransferStockInstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS210_TransferStock", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Get location
        /// </summary>
        /// <returns></returns>
        public tbs_MiscellaneousTypeCode IVS210_GetLocation()
        {
            ObjectResultData res = new ObjectResultData();
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            List<tbs_MiscellaneousTypeCode> ResMisc = new List<tbs_MiscellaneousTypeCode>();
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_INV_LOC);

                CommonUtil.MappingObjectLanguage<tbs_MiscellaneousTypeCode>(list);

                ResMisc = (from c in list
                           where (c.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK)
                           select c).ToList<tbs_MiscellaneousTypeCode>();

                if (ResMisc != null && ResMisc.Count > 0)
                    return ResMisc[0];
                else
                    return new tbs_MiscellaneousTypeCode();
            }
            catch (Exception)
            {
                return new tbs_MiscellaneousTypeCode();
            }
        }

        /// <summary>
        /// Validate register transfer instrument data
        /// </summary>
        /// <param name="Cond">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS210_cmdReg(IVS210RegisterCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS210_ScreenParameter prm = GetScreenObject<IVS210_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS210INST>();
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                IShelfMasterHandler ShelfH = ServiceContainer.GetService<IShelfMasterHandler>() as IShelfMasterHandler;
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_MOVE_SHELF, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                if (InvH.CheckStartedStockChecking() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }

                //6.1.1
                if (prm.ElemInstrument.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    return Json(res);
                }

                prm.ElemInstrument = Cond.StockInInstrument;
                prm.Location = Cond.Location;

                List<IVS210INST> arrayEmptyShelf = new List<IVS210INST>();

                bool isError = false;
                foreach (IVS210INST i in prm.ElemInstrument)
                {
                    //6.1.2
                    if (i.TransferQty <= 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4047, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        isError = true;
                        continue;
                    }

                    //6.1.3
                    if (string.IsNullOrEmpty(i.DestinationShelfNo))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4049, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                        isError = true;
                        continue;
                    }

                    //6.1.4
                    if (i.DestinationShelfNo == ShelfNo.C_INV_SHELF_NO_NOT_PRICE || i.DestinationShelfNo == ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF || i.DestinationShelfNo == ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4050, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                        isError = true;
                        continue;
                    }

                    //6.1.5
                    if (i.DestinationShelfNo == i.SourceShelfNo)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4053, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                        isError = true;
                        continue;
                    }

                    //6.1.6
                    doCheckTransferQty doCheck = new doCheckTransferQty();
                    doCheck.OfficeCode = prm.Office.OfficeCode;
                    doCheck.LocationCode = prm.Location;
                    doCheck.AreaCode = i.AreaCode;
                    doCheck.ShelfNo = i.SourceShelfNo;
                    doCheck.InstrumentCode = i.InstrumentCode;
                    doCheck.TransferQty = i.TransferQty;
                    doCheckTransferQtyResult doCheckResult = InvH.CheckTransferQty(doCheck);

                    i.InstrumentQty = doCheckResult.CurrentyQty;

                    if (doCheckResult.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        isError = true;
                        continue;
                    }
                    else if (doCheckResult.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        isError = true;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(i.DestinationShelfNo))
                    {
                        //6.1.7
                        List<tbm_Shelf> GetTbm_Shelf = ShelfH.GetTbm_Shelf(i.DestinationShelfNo);

                        //6.1.8
                        if (GetTbm_Shelf.Count <= 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4048, new string[] { i.DestinationShelfNo }, new string[] { i.DestShelfNo_id });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            isError = true;
                            continue;
                        }
                        else
                        {
                            i.DestinationShelfNo = GetTbm_Shelf[0].ShelfNo;
                        }

                        //6.1.9
                        if (GetTbm_Shelf.Count > 0 && GetTbm_Shelf[0].ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_PROJECT)
                        {
                            continue;
                        }

                        // 6.1.10
                        doGetShelfOfArea SourceShelf = InvH.GetShelfOfArea(i.AreaCode, i.InstrumentCode);
                        
                        // 6.1.11
                        if ((SourceShelf != null && SourceShelf.InstrumentQty > 0)
                            && SourceShelf.ShelfNo != i.DestinationShelfNo
                            && SourceShelf.ShelfNo != i.SourceShelfNo)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4054, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            isError = true;
                            continue;
                        }

                        // 6.1.12
                        if ((SourceShelf == null || SourceShelf.InstrumentQty == 0) || ((SourceShelf != null || SourceShelf.InstrumentQty > 0) && i.SourceShelfNo == SourceShelf.ShelfNo))
                        {
                            List<tbt_InventoryCurrent> doTbt_InventoryCurrent = InvH.GetTbt_InventoryCurrent(
                                prm.Office.OfficeCode, 
                                InstrumentLocation.C_INV_LOC_INSTOCK,
                                GetTbm_Shelf[0].AreaCode,
                                GetTbm_Shelf[0].ShelfNo,
                                GetTbm_Shelf[0].InstrumentCode);

                            if (doTbt_InventoryCurrent != null && doTbt_InventoryCurrent.Count != 0 && doTbt_InventoryCurrent[0] != null && doTbt_InventoryCurrent[0].InstrumentQty > 0)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4054, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                isError = true;
                                continue;
                            }

                            if ((i.ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL)
                                && i.SourceShelfNo != ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF
                                && i.InstrumentQty != i.TransferQty)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4056, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.StockOutQty_id });
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                isError = true;
                                continue;
                            }

                            // 6.1.14.3
                            bool blnDuplicateEmptyShelf = false;

                            foreach (IVS210INST item in arrayEmptyShelf)
                            {
                                //if (item.DestinationShelfNo.ToUpper() == i.DestinationShelfNo.ToUpper() && item.AreaCode != i.AreaCode)
                                if (item.DestinationShelfNo.ToUpper() == i.DestinationShelfNo.ToUpper() && item.InstrumentCode != i.InstrumentCode) //Modify by Jutarat A. on 21062013
                                {
                                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4128, new string[] { i.DestinationShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                    isError = true;
                                    blnDuplicateEmptyShelf = true;
                                    break;
                                }
                            }

                            if (blnDuplicateEmptyShelf == false)
                            {
                                arrayEmptyShelf.Add(i);
                            }
                        }

                        #region OLD CODE
                        ////6.1.9
                        //if (GetTbm_Shelf.Count > 0 && GetTbm_Shelf[0].ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL)
                        //{
                        //    //6.1.9.1
                        //    List<tbt_InventoryCurrent> doTbt_InventoryCurrent = InvH.GetTbt_InventoryCurrent(prm.Office.OfficeCode, InstrumentLocation.C_INV_LOC_INSTOCK, GetTbm_Shelf[0].AreaCode, GetTbm_Shelf[0].ShelfNo, GetTbm_Shelf[0].InstrumentCode);

                        //    if ((i.AreaCode != GetTbm_Shelf[0].AreaCode || string.Compare(i.InstrumentCode, GetTbm_Shelf[0].InstrumentCode, true) != 0)
                        //        //&& i.ShelfTypeCode != ShelfType.C_INV_SHELF_TYPE_NORMAL
                        //        && (doTbt_InventoryCurrent.Count > 0 && doTbt_InventoryCurrent[0].InstrumentQty > 0)
                        //    )
                        //    {
                        //        //6.1.9.2
                        //        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4054, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                        //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        //        isError = true;
                        //        continue;
                        //    }

                        //    else if (
                        //        //(i.AreaCode != GetTbm_Shelf[0].AreaCode || string.Compare(i.InstrumentCode, GetTbm_Shelf[0].InstrumentCode, true) != 0) 
                        //        //&& (doTbt_InventoryCurrent.Count > 0 && doTbt_InventoryCurrent[0].InstrumentQty > 0) 
                        //        //&& i.ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL
                        //        (doTbt_InventoryCurrent.Count == 0 || (doTbt_InventoryCurrent.Count > 0 && doTbt_InventoryCurrent[0].InstrumentQty == 0))
                        //        && i.ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL
                        //        && i.InstrumentQty != i.TransferQty
                        //    )
                        //    {
                        //        //6.1.9.3
                        //        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4056, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        //        isError = true;
                        //        continue;
                        //    }
                        //} //dtTbm_Shelf.ShelfTypeCode = C_INV_SHELF_TYPE_NORMAL

                        ////6.1.10
                        //if (GetTbm_Shelf.Count > 0 && GetTbm_Shelf[0].ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_EMPTY)
                        //{
                        //    //6.1.10.1
                        //    //                           if (i.ShelfTypeCode != ShelfType.C_INV_SHELF_TYPE_NORMAL)
                        //    //                           {
                        //    doGetShelfOfArea SourceShelf = InvH.GetShelfOfArea(i.AreaCode, i.InstrumentCode);

                        //    if (SourceShelf != null && !string.IsNullOrEmpty(SourceShelf.ShelfNo) && SourceShelf.ShelfNo != i.DestinationShelfNo)
                        //    {
                        //        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4055, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                        //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        //        isError = true;
                        //        continue;
                        //    }
                        //    //                            }

                        //    //6.1.10.2
                        //    if (i.ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL && i.ShelfNo != ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF)
                        //    {
                        //        if (i.InstrumentQty != i.TransferQty)
                        //        {
                        //            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4056, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        //            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        //            isError = true;
                        //            continue;
                        //        }
                        //    }

                        //    //6.1.10.3
                        //    bool blnDuplicateEmptyShelf = false;

                        //    foreach (IVS210INST item in arrayEmptyShelf)
                        //    {
                        //        if (item.DestinationShelfNo.ToUpper() == i.DestinationShelfNo.ToUpper() && item.AreaCode != i.AreaCode)
                        //        {
                        //            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4128, new string[] { i.DestinationShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                        //            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        //            isError = true;
                        //            blnDuplicateEmptyShelf = true;
                        //            break;
                        //        }
                        //    }

                        //    if (blnDuplicateEmptyShelf == false)
                        //    {
                        //        arrayEmptyShelf.Add(i);
                        //    }
                        //}
                        #endregion
                    }
                }

                res.ResultData = prm.ElemInstrument;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Register transfer instrument data
        /// </summary>
        /// <param name="Con">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS210_cmdConfirm(IVS210RegisterCond Con)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS210_ScreenParameter prm = GetScreenObject<IVS210_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS210INST>();

                //8.1
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_MOVE_SHELF, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                IShelfMasterHandler ShelfH = ServiceContainer.GetService<IShelfMasterHandler>() as IShelfMasterHandler;

                if (InvH.CheckStartedStockChecking() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }

                foreach (IVS210INST i in prm.ElemInstrument)
                {
                    //8.2.1
                    if (i.DestinationShelfNo == ShelfNo.C_INV_SHELF_NO_NOT_PRICE
                        || i.DestinationShelfNo == ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF
                        || i.DestinationShelfNo == ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4050, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                        res.ResultData = i.InstrumentCode + "," + i.row_id;
                        i.IsError = true;
                        return Json(res);
                    }

                    //8.2.2
                    if (i.DestinationShelfNo == i.SourceShelfNo)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4053, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                        res.ResultData = i.InstrumentCode + "," + i.row_id;
                        i.IsError = true;
                        return Json(res);
                    }

                    //8.2.3
                    doCheckTransferQty doCheck = new doCheckTransferQty();
                    doCheck.OfficeCode = prm.Office.OfficeCode;
                    doCheck.LocationCode = prm.Location;
                    doCheck.AreaCode = i.AreaCode;
                    doCheck.ShelfNo = i.SourceShelfNo;
                    doCheck.InstrumentCode = i.InstrumentCode;
                    doCheck.TransferQty = i.TransferQty;
                    doCheckTransferQtyResult doCheckResult = InvH.CheckTransferQty(doCheck);

                    i.InstrumentQty = doCheckResult.CurrentyQty;

                    if (doCheckResult.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.ResultData = prm.ElemInstrument;
                        i.IsError = true;
                        return Json(res);
                    }
                    else if (doCheckResult.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.ResultData = prm.ElemInstrument;
                        i.IsError = true;
                        return Json(res);
                    }

                    //8.2.4
                    List<tbm_Shelf> GetTbm_Shelf = ShelfH.GetTbm_Shelf(i.DestinationShelfNo);

                    // 8.2.5
                    if (GetTbm_Shelf.Count > 0 && GetTbm_Shelf[0].ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_PROJECT)
                    {
                        continue;
                    }

                    // 8.2.6
                    doGetShelfOfArea SourceShelf = InvH.GetShelfOfArea(i.AreaCode, i.InstrumentCode);

                    // 8.2.7
                    if ((SourceShelf != null && SourceShelf.InstrumentQty > 0)
                        && SourceShelf.ShelfNo != i.DestinationShelfNo
                        && SourceShelf.ShelfNo != i.SourceShelfNo)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4054, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        i.IsError = true;
                        continue;
                    }

                    // 8.2.8
                    if ((SourceShelf == null || SourceShelf.InstrumentQty == 0))
                    {
                        List<tbt_InventoryCurrent> doTbt_InventoryCurrent = InvH.GetTbt_InventoryCurrent(
                            prm.Office.OfficeCode,
                            InstrumentLocation.C_INV_LOC_INSTOCK,
                            GetTbm_Shelf[0].AreaCode,
                            GetTbm_Shelf[0].ShelfNo,
                            GetTbm_Shelf[0].InstrumentCode);

                        if (doTbt_InventoryCurrent != null && doTbt_InventoryCurrent.Count != 0 && doTbt_InventoryCurrent[0] != null && doTbt_InventoryCurrent[0].InstrumentQty > 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4054, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            i.IsError = true;
                            continue;
                        }

                        if ((i.ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL)
                            && i.SourceShelfNo != ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF
                            && i.InstrumentQty != i.TransferQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4056, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.StockOutQty_id });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            i.IsError = true;
                            continue;
                        }
                    }

                    #region OLD CODE
                    //8.2.5
                    //if (GetTbm_Shelf.Count > 0 && GetTbm_Shelf[0].ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL)
                    //{
                    //    //8.2.5.1
                    //    List<tbt_InventoryCurrent> doTbt_InventoryCurrent = InvH.GetTbt_InventoryCurrent(prm.Office.OfficeCode, InstrumentLocation.C_INV_LOC_INSTOCK, i.AreaCode, GetTbm_Shelf[0].ShelfNo, i.InstrumentCode);

                    //    if ((i.AreaCode != GetTbm_Shelf[0].AreaCode || string.Compare(i.InstrumentCode, GetTbm_Shelf[0].InstrumentCode, true) != 0) 
                    //        //&& i.ShelfTypeCode != ShelfType.C_INV_SHELF_TYPE_NORMAL
                    //        && (doTbt_InventoryCurrent.Count > 0 && doTbt_InventoryCurrent[0].InstrumentQty > 0)
                    //    ) {
                    //        //8.2.5.2
                    //        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4054, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                    //        res.ResultData = i.InstrumentCode + "," + i.row_id;
                    //        i.IsError = true;
                    //        return Json(res);
                    //    }
                    //    else if (
                    //        //(i.AreaCode != GetTbm_Shelf[0].AreaCode || string.Compare(i.InstrumentCode, GetTbm_Shelf[0].InstrumentCode, true) != 0) 
                    //        //&& (doTbt_InventoryCurrent.Count > 0 && doTbt_InventoryCurrent[0].InstrumentQty > 0) 
                    //        //&& i.ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL
                    //        (doTbt_InventoryCurrent.Count == 0 || (doTbt_InventoryCurrent.Count > 0 && doTbt_InventoryCurrent[0].InstrumentQty == 0))
                    //        && i.ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL
                    //        && i.InstrumentQty != i.TransferQty
                    //    ) {
                    //        //8.2.5.3
                    //        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4056, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.StockOutQty_id });
                    //        res.ResultData = i.InstrumentCode + "," + i.row_id;
                    //        i.IsError = true;
                    //        return Json(res);
                    //    }
                    //} //dtTbm_Shelf.ShelfTypeCode = C_INV_SHELF_TYPE_NORMAL

                    ////8.2.6
                    //if (GetTbm_Shelf.Count > 0 && GetTbm_Shelf[0].ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_EMPTY)
                    //{
                    //    //8.2.6.1
                    //    if (i.ShelfTypeCode != ShelfType.C_INV_SHELF_TYPE_NORMAL)
                    //    {
                    //        doGetShelfOfArea SourceShelf = InvH.GetShelfOfArea(i.AreaCode, i.InstrumentCode);

                    //        if (SourceShelf != null && !string.IsNullOrEmpty(SourceShelf.ShelfNo) && SourceShelf.ShelfNo != i.DestinationShelfNo)
                    //        {
                    //            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4055, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.DestShelfNo_id });
                    //            res.ResultData = i.InstrumentCode + "," + i.row_id;
                    //            i.IsError = true;
                    //            return Json(res);
                    //        }
                    //    }

                    //    //8.2.6.2
                    //    if (i.ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL)
                    //    {
                    //        if (i.InstrumentQty != i.TransferQty)
                    //        {
                    //            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4056, new string[] { i.SourceShelfNo, i.InstrumentCode }, new string[] { i.StockOutQty_id });
                    //            res.ResultData = i.InstrumentCode + "," + i.row_id;
                    //            i.IsError = true;
                    //            return Json(res);
                    //        }
                    //    }
                    //}
                    #endregion
                }

                using (TransactionScope scope = new TransactionScope()) //Add by Jutarat A. on 18022013
                {
                    //8.3
                    foreach (IVS210INST i in prm.ElemInstrument)
                    {
                        //8.3.1
                        List<tbt_InventoryCurrent> lstSourceCurr = InvH.GetTbt_InventoryCurrent(prm.Office.OfficeCode, prm.Location, i.AreaCode, i.SourceShelfNo, i.InstrumentCode);

                        if (lstSourceCurr.Count > 0)
                        {
                            lstSourceCurr[0].InstrumentQty = lstSourceCurr[0].InstrumentQty - i.TransferQty;
                            lstSourceCurr[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            lstSourceCurr[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        }

                        InvH.UpdateTbt_InventoryCurrent(lstSourceCurr);

                        //8.3.2
                        List<tbt_InventoryCurrent> lstDestCurr = InvH.GetTbt_InventoryCurrent(prm.Office.OfficeCode, prm.Location, i.AreaCode, i.DestinationShelfNo, i.InstrumentCode);

                        if (lstDestCurr.Count <= 0)
                        {
                            doNormalShelfExistCurrent destShelf = InvH.GetNormalShelfExistCurrent(i.DestinationShelfNo);
                            if (destShelf != null)
                            {
                                InvH.DeleteTbt_InventoryCurrent(
                                    prm.Office.OfficeCode,
                                    InstrumentLocation.C_INV_LOC_INSTOCK,
                                    destShelf.AreaCode,
                                    i.DestinationShelfNo,
                                    destShelf.InstrumentCode);
                            }

                            tbt_InventoryCurrent newCurr = new tbt_InventoryCurrent();
                            newCurr.OfficeCode = prm.Office.OfficeCode;
                            newCurr.LocationCode = prm.Location;
                            newCurr.AreaCode = i.AreaCode;
                            newCurr.ShelfNo = i.DestinationShelfNo;
                            newCurr.InstrumentCode = i.InstrumentCode;
                            newCurr.InstrumentQty = i.TransferQty;
                            newCurr.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            newCurr.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            List<tbt_InventoryCurrent> CurrentForInsert = new List<tbt_InventoryCurrent>();
                            CurrentForInsert.Add(newCurr);

                            InvH.InsertTbt_InventoryCurrent(CurrentForInsert);
                        }
                        else
                        {
                            lstDestCurr[0].InstrumentQty = lstDestCurr[0].InstrumentQty + i.TransferQty;
                            lstDestCurr[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            lstDestCurr[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            InvH.UpdateTbt_InventoryCurrent(lstDestCurr);
                        }

                        //8.3.3
                        List<doShelf> dtTbm_DestShelf = ShelfH.GetShelf(i.DestinationShelfNo, null, null, null);

                        //8.3.4
                        if (dtTbm_DestShelf[0].ShelfTypeCode != ShelfType.C_INV_SHELF_TYPE_PROJECT)
                        {
                            //8.3.5
                            doGetShelfOfArea SourceShelf = InvH.GetShelfOfArea(i.AreaCode, i.InstrumentCode);

                            //8.3.6
                            //if (dtTbm_DestShelf.Count > 0 && i.ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL
                            //    && (dtTbm_DestShelf[0].ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_EMPTY || dtTbm_DestShelf[0].ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL))
                            if (SourceShelf != null && SourceShelf.ShelfNo != i.DestinationShelfNo)
                            {
                                //Update Source Shelf No
                                //List<tbm_Shelf> Tbm_SourceShelf = ShelfH.GetTbm_Shelf(i.SourceShelfNo);
                                List<tbm_Shelf> Tbm_SourceShelf = ShelfH.GetTbm_Shelf(SourceShelf.ShelfNo);

                                if (Tbm_SourceShelf.Count > 0)
                                {
                                    Tbm_SourceShelf[0].ShelfTypeCode = ShelfType.C_INV_SHELF_TYPE_EMPTY;
                                    Tbm_SourceShelf[0].AreaCode = null;
                                    Tbm_SourceShelf[0].InstrumentCode = null;
                                    Tbm_SourceShelf[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    Tbm_SourceShelf[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                    ShelfH.UpdateShelf(Tbm_SourceShelf[0]);
                                }

                                //InvH.DeleteTbt_InventoryCurrent(prm.Office.OfficeCode, InstrumentLocation.C_INV_LOC_INSTOCK, i.AreaCode, i.SourceShelfNo, i.InstrumentCode);
                                InvH.DeleteTbt_InventoryCurrent(prm.Office.OfficeCode, InstrumentLocation.C_INV_LOC_INSTOCK, i.AreaCode, SourceShelf.ShelfNo, i.InstrumentCode);

                                //Update Destination Shelf No
                                List<tbm_Shelf> Tbm_DestShelf = ShelfH.GetTbm_Shelf(i.DestinationShelfNo);

                                if (Tbm_DestShelf.Count > 0)
                                {
                                    Tbm_DestShelf[0].ShelfTypeCode = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                                    Tbm_DestShelf[0].AreaCode = i.AreaCode;
                                    Tbm_DestShelf[0].InstrumentCode = i.InstrumentCode;
                                    Tbm_DestShelf[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    Tbm_DestShelf[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                    ShelfH.UpdateShelf(Tbm_DestShelf[0]);
                                }
                            }

                            //8.3.7
                            //if (dtTbm_DestShelf.Count > 0 && dtTbm_DestShelf[0].ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_EMPTY)
                            else if (SourceShelf == null)
                            {
                                List<tbm_Shelf> Tbm_DestShelf = ShelfH.GetTbm_Shelf(i.DestinationShelfNo);

                                if (Tbm_DestShelf.Count > 0)
                                {
                                    Tbm_DestShelf[0].ShelfTypeCode = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                                    Tbm_DestShelf[0].AreaCode = i.AreaCode;
                                    Tbm_DestShelf[0].InstrumentCode = i.InstrumentCode;
                                    Tbm_DestShelf[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    Tbm_DestShelf[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                    ShelfH.UpdateShelf(Tbm_DestShelf[0]);
                                }
                            }
                        }
                    }

                    scope.Complete(); //Add by Jutarat A. on 18022013
                }

                //8.4
                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4019);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Update row index to add instrument data
        /// </summary>
        /// <param name="Cond">instrument data</param>
        /// <returns></returns>
        public ActionResult IVS210_UpdateRowIDElem(IVS210INST Cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IVS210_ScreenParameter prm = GetScreenObject<IVS210_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS210INST>();

                foreach (IVS210INST i in prm.ElemInstrument)
                {
                    //if (Cond.InstrumentCode == i.InstrumentCode && Cond.AreaCode == i.AreaCode)
                    //{
                    //    i.row_id = Cond.row_id;
                    //    break;
                    //}
                }
                UpdateScreenObject(prm);

                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Initial screen parameter
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS210_InitParam()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS210_ScreenParameter prm = GetScreenObject<IVS210_ScreenParameter>();
                prm.ElemInstrument = new List<IVS210INST>();
                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Check instrument data before add instrument
        /// </summary>
        /// <param name="Cond">instrument data</param>
        /// <returns></returns>
        public ActionResult IVS210_beforeAddElem(IVS210INST Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS210_ScreenParameter prm = GetScreenObject<IVS210_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS210INST>();


                foreach (IVS210INST i in prm.ElemInstrument)
                {
                    if (i.InstrumentCode == Cond.InstrumentCode && i.ShelfNo == Cond.ShelfNo && i.AreaCode == Cond.AreaCode)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4005);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }

                prm.ElemInstrument.Add(Cond);

                UpdateScreenObject(prm);

                res.ResultData = true;
                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Check instrument data before remove instrument
        /// </summary>
        /// <param name="InstrumentCode">Remove selecedt instrument code</param>
        /// <param name="ShelfNo">Remove selected  Shelf no</param>
        /// <param name="AreaCode">Remove selected  Area code</param>
        /// <returns></returns>
        public ActionResult IVS210_DelElem(string InstrumentCode, string ShelfNo, string AreaCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS210_ScreenParameter prm = GetScreenObject<IVS210_ScreenParameter>();

                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS210INST>();

                for (int i = 0; i < prm.ElemInstrument.Count; i++)
                    if (prm.ElemInstrument[i].InstrumentCode == InstrumentCode
                        && prm.ElemInstrument[i].ShelfNo == ShelfNo
                        && prm.ElemInstrument[i].AreaCode == AreaCode)
                    {
                        prm.ElemInstrument.RemoveAt(i);
                        break;
                    }

                UpdateScreenObject(prm);
                res.ResultData = true;

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }
    }
}