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
        /// - Check user permission for screen IVS160.<br />
        /// - Check freezed data.<br />
        /// - Check implement stock checking.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS160_Authority(IVS160_ScreenParameter param)
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
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_CHECKING_INSTRUMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
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

                //// Check for the stock is started
                //if (handlerInventory.CheckStartedStockChecking() == 0)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                //    return Json(res);
                //}

                // Check for the implement stock checkint
                if (handlerInventory.CheckImplementStockChecking() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS160_ScreenParameter>("IVS160", param, res);
        }

        /// <summary>
        /// Get config for Check Detail table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS160_InitialSearchResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS160", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS160")]
        public ActionResult IVS160()
        {
            IVS160_ScreenParameter param = GetScreenObject<IVS160_ScreenParameter>();
            IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            List<tbt_InventoryCheckingSchedule> list = handlerInventory.GetLastCheckingSchedule();

            if (list.Count > 0)
            {
                ViewBag.CheckingYearMonth = list[0].CheckingYearMonth;
                param.CheckingYearMonth = list[0].CheckingYearMonth;
                param.CheckingStartDate = list[0].CheckingStartDate.HasValue ? list[0].CheckingStartDate.Value : DateTime.Now;
            }
            else
            {
                ViewBag.CheckingYearMonth = DateTime.Now.ToString("yyyyMM");
                param.CheckingYearMonth = DateTime.Now.ToString("yyyyMM");
                param.CheckingStartDate = DateTime.Now;
            }

            var lstHQOffice = handlerInventory.GetInventoryHeadOffice();
            if (lstHQOffice == null || lstHQOffice.Count <= 0)
            {
                throw new ApplicationException("Cannot get inventory head office.");
            }
            ViewBag.HeadOfficeCode = lstHQOffice[0].OfficeCode;

            return View();
        }

        /// <summary>
        /// Search checking detail.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult IVS160_SearchResponse(IVS160_SearchCondition cond)
        {
            IVS160_ScreenParameter param = GetScreenObject<IVS160_ScreenParameter>();

            List<dtCheckingDetailList> list = new List<dtCheckingDetailList>();
            List<dtCheckingDetailList> list_ForView = new List<dtCheckingDetailList>();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {

                // Check required field.
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);

                }

                // Keep office code
                param.OfficeCode = cond.OfficeCode;

                // Keep location code
                param.LocationCode = cond.LocationCode;

                param.AreaCode = cond.AreaCode;

                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                list = handlerInventory.GetCheckingDetailList(cond);

                // clear
                param.DetailList = new List<tbt_InventoryCheckingSlipDetail>();
                param.DetailList_ForView = new List<dtCheckingDetailList>();
                param.CurrentPage = 0;

                if (list.Count > 0)
                {
                    // clone from dtCheckingDetailList -> tbt_InventoryCheckingSlipDetail
                    tbt_InventoryCheckingSlipDetail newRowItem;
                    foreach (var item in list)
                    {
                        newRowItem = new tbt_InventoryCheckingSlipDetail()
                        {
                            InstrumentCode = item.InstrumentCode,
                            RunningNoInSlip = item.RunningNoInSlip,
                            AreaCode = item.AreaCode,
                            ShelfNo = item.ShelfNo,
                            StockQty = item.StockQty,
                            CheckingQty = item.CheckingQty,
                            DefaultCheckingQty = item.CheckingQty,
                            AddFlag = item.AddFlag,
                            CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                            UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                        };
                        item.DefaultCheckingQty = item.CheckingQty;

                        param.DetailList.Add(newRowItem);
                    };

                    param.DetailList_ForView.AddRange(list);
                    param.CurrentPage = 1;
                }


                list_ForView = (from p in param.DetailList_ForView where p.Page == param.CurrentPage select p).ToList<dtCheckingDetailList>();

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtCheckingDetailList>(list_ForView, "Inventory\\IVS160", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);


        }

        /// <summary>
        /// Get instrument name of given instrument code.
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <returns></returns>
        public ActionResult IVS160_GetInstrumentName(string InstrumentCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<tbm_Instrument> list = hand.GetTbm_Instrument(InstrumentCode);
                tbm_Instrument item = new tbm_Instrument();
                if (list.Count > 0)
                {
                    res.ResultData = new IVS160_InstrumetInfo()
                    {
                        InstrumentCode = list[0].InstrumentCode,
                        InstrumentName = list[0].InstrumentName
                    };
                }
                else
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4037);
                }

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Add check instrument.<br />
        /// - Validate require field.<br />
        /// - Check shelf type.<br />
        /// - Check duplicate add.<br />
        /// - Add to list.
        /// </summary>
        /// <param name="OfficeCode"></param>
        /// <param name="gridCurrentPage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult IVS160_AddInstrumentToList(string OfficeCode, List<tbt_InventoryCheckingSlipDetail> gridCurrentPage, IVS160_AddData data)
        {
            IVS160_ScreenParameter param = GetScreenObject<IVS160_ScreenParameter>();

            List<dtCheckingDetailList> list = new List<dtCheckingDetailList>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (param.DetailList == null)
                {
                    param.DetailList = new List<tbt_InventoryCheckingSlipDetail>();
                    param.CurrentPage = 1;
                }
                if (param.DetailList_ForView == null)
                {
                    param.DetailList_ForView = new List<dtCheckingDetailList>();
                }

                // Check required field.
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                // Business checking # 1
                IShelfMasterHandler handlerShelfMaster = ServiceContainer.GetService<IShelfMasterHandler>() as IShelfMasterHandler;
                List<tbm_Shelf> listShelf = handlerShelfMaster.GetTbm_Shelf(data.ShelfNo);
                if (listShelf.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4048, new string[] { data.ShelfNo }, new string[] { "ShelfNo" });
                    return Json(res);
                }
                else
                {
                    data.ShelfNo = listShelf[0].ShelfNo;

                    // Business checking # 2 (duplicate)
                    var isExist = param.DetailList.Count == 0 ? false : param.DetailList.Any(d => d.key == data.key);  //(from p in param.DetailList where p.key == data.key select p).Any<tbt_InventoryCheckingSlipDetail>();

                    if (isExist)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4081, null, null);
                        return Json(res);
                    }

                    //if (listShelf[0].ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_NORMAL
                    //    && (listShelf[0].AreaCode != data.AreaCode || string.Compare(listShelf[0].InstrumentCode, data.InstrumentCode, true) == 0)
                    //    && OfficeCode == ViewBag.HeadOfficeCode)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4079, new string[] { data.ShelfNo }, new string[] { "ShelfNo" });
                    //    return Json(res);
                    //}

                    //if (listShelf[0].ShelfTypeCode == ShelfType.C_INV_SHELF_TYPE_EMPTY)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4080, null, new string[] { "ShelfNo" });
                    //    return Json(res);
                    //}
                }


                // Update current page  (update value of CheckingQty)

                // 1. Update DetailList
                var currentPage = (from p in param.DetailList where p.Page == param.CurrentPage select p).ToList<tbt_InventoryCheckingSlipDetail>();
                for (int i = 0; i < currentPage.Count; i++)
                {
                    if (currentPage[i].key == gridCurrentPage[i].key)
                    {
                        currentPage[i].CheckingQty = gridCurrentPage[i].CheckingQty; // gridCurrentPage[i].CheckingQty.HasValue ? gridCurrentPage[i].CheckingQty.Value : 0;
                        currentPage[i].txtCheckingQtyID = gridCurrentPage[i].txtCheckingQtyID;
                    }

                }

                // 2. Update DetailList_ForView
                var currentPage_ForView = (from p in param.DetailList_ForView where p.Page == param.CurrentPage select p).ToList<dtCheckingDetailList>();
                for (int i = 0; i < currentPage_ForView.Count; i++)
                {
                    if (currentPage_ForView[i].key == gridCurrentPage[i].key)
                    {
                        currentPage_ForView[i].CheckingQty = gridCurrentPage[i].CheckingQty; // gridCurrentPage[i].CheckingQty.HasValue ? gridCurrentPage[i].CheckingQty.Value : 0;
                        currentPage_ForView[i].txtCheckingQtyID = gridCurrentPage[i].txtCheckingQtyID;
                    }
                }



                // Add new entry
                int nextRunningNo = param.DetailList.Count == 0 ? 1 : (param.DetailList[param.DetailList.Count - 1].RunningNoInSlip + 1);

                // Prepare

                // 1. RowItem (for save)
                tbt_InventoryCheckingSlipDetail newRowItem = new tbt_InventoryCheckingSlipDetail()
                {
                    InstrumentCode = data.InstrumentCode,
                    RunningNoInSlip = nextRunningNo,
                    AreaCode = data.AreaCode,
                    ShelfNo = data.ShelfNo,
                    StockQty = 0, // 0 for add new
                    CheckingQty = null, //0,  // defualt 0 , and user will input again in grid
                    AddFlag = true,
                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,

                };

                // 2. RowItem (for view)
                dtCheckingDetailList newRowItem_ForView = CommonUtil.CloneObject<IVS160_AddData, dtCheckingDetailList>(data);
                newRowItem_ForView.CheckingQty = null; // 0;
                newRowItem_ForView.AddFlag = true;

                // Add to list
                param.DetailList.Add(newRowItem);
                param.DetailList_ForView.Add(newRowItem_ForView);


                // Prepare list for return to show in grid
                int maxpage = param.DetailList_ForView.Max(m => m.Page);
                currentPage_ForView = (from p in param.DetailList_ForView where p.Page == maxpage select p).ToList<dtCheckingDetailList>();

                param.CurrentPage = maxpage;

                list = currentPage_ForView;
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtCheckingDetailList>(list, "Inventory\\IVS160", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);


        }

        /// <summary>
        /// Get current page information from screen parameter.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS160_GetPageInfo()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS160_ScreenParameter param = GetScreenObject<IVS160_ScreenParameter>();
                int iTotalItem = 0;
                int iTotalPage = 0;

                if (param.DetailList != null)
                {
                    if (param.DetailList.Count > 0)
                    {
                        iTotalItem = param.DetailList == null ? 0 : param.DetailList.Count;
                        iTotalPage = param.DetailList == null ? 0 : param.DetailList.Max(m => m.Page);
                    }
                }

                IVS160_PageInfo pageInfo = new IVS160_PageInfo()
                {
                    CurrentPage = param.CurrentPage,
                    TotalItem = iTotalItem,
                    TotalPage = iTotalPage
                };

                res.ResultData = pageInfo;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Change page in screen.
        /// </summary>
        /// <param name="gridCurrentPage"></param>
        /// <param name="type"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult IVS160_GotoPage(List<tbt_InventoryCheckingSlipDetail> gridCurrentPage, string type, int page = 0)
        {
            ObjectResultData res = new ObjectResultData();
            List<dtCheckingDetailList> list = new List<dtCheckingDetailList>();
            try
            {
                IVS160_ScreenParameter param = GetScreenObject<IVS160_ScreenParameter>();
                int iTotalItem = 0;
                int iTotalPage = 0;

                if (param.DetailList != null)
                {
                    if (param.DetailList.Count > 0)
                    {
                        iTotalItem = param.DetailList == null ? 0 : param.DetailList.Count;
                        iTotalPage = param.DetailList == null ? 0 : param.DetailList.Max(m => m.Page);
                    }
                }

                if (param.DetailList == null)
                {
                    param.DetailList = new List<tbt_InventoryCheckingSlipDetail>();
                    param.CurrentPage = 1;
                }
                if (param.DetailList_ForView == null)
                {
                    param.DetailList_ForView = new List<dtCheckingDetailList>();
                }

                if (param.DetailList.Count > 0)
                {
                    // Update data of current page  (update value of CheckingQty)

                    // 1. Update DetailList
                    var currentPage = (from p in param.DetailList where p.Page == param.CurrentPage select p).ToList<tbt_InventoryCheckingSlipDetail>();
                    for (int i = 0; i < currentPage.Count; i++)
                    {
                        if (currentPage[i].key == gridCurrentPage[i].key)
                        {
                            currentPage[i].CheckingQty = gridCurrentPage[i].CheckingQty;
                            currentPage[i].txtCheckingQtyID = gridCurrentPage[i].txtCheckingQtyID;
                        }

                    }

                    // 2. Update DetailList_ForView
                    var currentPage_ForView = (from p in param.DetailList_ForView where p.Page == param.CurrentPage select p).ToList<dtCheckingDetailList>();
                    for (int i = 0; i < currentPage_ForView.Count; i++)
                    {
                        if (currentPage_ForView[i].key == gridCurrentPage[i].key)
                        {
                            currentPage_ForView[i].CheckingQty = gridCurrentPage[i].CheckingQty;
                            currentPage_ForView[i].txtCheckingQtyID = gridCurrentPage[i].txtCheckingQtyID;
                        }
                    }


                    int targetPage = param.CurrentPage;
                    if (type == "back")
                    {
                        if (param.CurrentPage > 1)
                        {
                            targetPage--;
                            param.CurrentPage--;
                        }
                    }
                    else if (type == "goto")
                    {
                        if (page > 0 && page <= iTotalPage)
                        {
                            targetPage = page;
                            param.CurrentPage = page;
                        }
                    }
                    else if (type == "forward")
                    {
                        if (param.CurrentPage < iTotalPage)
                        {
                            targetPage++;
                            param.CurrentPage++;
                        }
                    }

                    list = (from p in param.DetailList_ForView where p.Page == targetPage select p).ToList<dtCheckingDetailList>();

                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtCheckingDetailList>(list, "Inventory\\IVS160", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);
        }

        /// <summary>
        /// Validate before register.<br />
        /// - Check system suspending.<br />
        /// - Check user permission.<br />
        /// - Check list not empty.<br />
        /// - Check checking date.<br />
        /// - Check checking quantity must have value.
        /// </summary>
        /// <param name="gridCurrentPage"></param>
        /// <param name="checkingDate"></param>
        /// <returns></returns>
        public ActionResult IVS160_Register(List<tbt_InventoryCheckingSlipDetail> gridCurrentPage, DateTime? checkingDate)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<dtCheckingDetailList> list = new List<dtCheckingDetailList>();
            try
            {
                // Check permission
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_CHECKING_INSTRUMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                // Is suspend ?
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                IVS160_ScreenParameter param = GetScreenObject<IVS160_ScreenParameter>();
                int iTotalItem = 0;
                int iTotalPage = 0;

                if (param.DetailList != null)
                {
                    if (param.DetailList.Count > 0)
                    {
                        iTotalItem = param.DetailList == null ? 0 : param.DetailList.Count;
                        iTotalPage = param.DetailList == null ? 0 : param.DetailList.Max(m => m.Page);
                    }
                }

                if (param.DetailList == null)
                {
                    param.DetailList = new List<tbt_InventoryCheckingSlipDetail>();
                    param.CurrentPage = 1;
                }
                if (param.DetailList_ForView == null)
                {
                    param.DetailList_ForView = new List<dtCheckingDetailList>();
                }

                if (param.DetailList.Count > 0)
                {
                    // Update data of current page  (update value of CheckingQty)

                    // 1. Update DetailList
                    var currentPage = (from p in param.DetailList where p.Page == param.CurrentPage select p).ToList<tbt_InventoryCheckingSlipDetail>();
                    for (int i = 0; i < currentPage.Count; i++)
                    {
                        if (currentPage[i].key == gridCurrentPage[i].key)
                        {
                            currentPage[i].CheckingQty = gridCurrentPage[i].CheckingQty;
                            currentPage[i].txtCheckingQtyID = gridCurrentPage[i].txtCheckingQtyID;
                        }

                    }

                    // 2. Update DetailList_ForView
                    var currentPage_ForView = (from p in param.DetailList_ForView where p.Page == param.CurrentPage select p).ToList<dtCheckingDetailList>();
                    for (int i = 0; i < currentPage_ForView.Count; i++)
                    {
                        if (currentPage_ForView[i].key == gridCurrentPage[i].key)
                        {
                            currentPage_ForView[i].CheckingQty = gridCurrentPage[i].CheckingQty;
                            currentPage_ForView[i].txtCheckingQtyID = gridCurrentPage[i].txtCheckingQtyID;
                        }
                    }

                    // Validate # 1
                    if (param.DetailList.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4082);
                        return Json(res);
                    }

                    if (param.DetailList.All(d => d.CheckingQty.GetValueOrDefault(0) == 0))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4082);
                        return Json(res);
                    }

                    // Validate # 2
                    param.CheckingDate = checkingDate;
                    if (CommonUtil.IsNullOrEmpty(checkingDate))
                    {
                        string lblCheckingDate = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INVENTORY, "IVS160", "lblCheckingDate");
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { lblCheckingDate }, new string[] { "CheckingDate" });

                        return Json(res);
                    }
                    else
                    {
                        if (DateTime.Compare(checkingDate.Value.Date, param.CheckingStartDate.Value.Date) < 0
                            || DateTime.Compare(checkingDate.Value.Date, DateTime.Today) > 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4077, new string[] { CommonUtil.TextDate(param.CheckingStartDate) });
                            return Json(res);
                        }
                    }

                    //foreach (var d in param.DetailList)
                    //{
                    //    if (!d.CheckingQty.HasValue)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4083
                    //            , new string[] { d.ShelfNo, d.AreaCode, d.InstrumentCode }
                    //            , new string[] { d.txtCheckingQtyID } 
                    //        );
                    //    }
                    //}

                    foreach (var d in param.DetailList)
                    {
                        if (!d.CheckingQty.HasValue)
                        {
                            d.CheckingQty = 0;
                        }
                    }

                    foreach (var d in param.DetailList_ForView)
                    {
                        if (!d.CheckingQty.HasValue)
                        {
                            d.CheckingQty = 0;
                        }
                    }

                    if (res.MessageList != null && res.MessageList.Count > 0)
                    {
                        return Json(res);
                    }

                    res.ResultData = "1";
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }


            return Json(res);
        }

        /// <summary>
        /// Get detail in screen parameter for view.
        /// </summary>
        /// <param name="isShowAtCurrentPage"></param>
        /// <returns></returns>
        public ActionResult IVS160_GetAllDetailForView(bool isShowAtCurrentPage = false)
        {
            ObjectResultData res = new ObjectResultData();
            List<dtCheckingDetailList> list = new List<dtCheckingDetailList>();
            try
            {
                IVS160_ScreenParameter param = GetScreenObject<IVS160_ScreenParameter>();
                int iTotalItem = 0;
                int iTotalPage = 0;

                if (param.DetailList != null)
                {
                    if (param.DetailList.Count > 0)
                    {
                        iTotalItem = param.DetailList == null ? 0 : param.DetailList.Count;
                        iTotalPage = param.DetailList == null ? 0 : param.DetailList.Max(m => m.Page);
                    }
                }

                if (param.DetailList == null)
                {
                    param.DetailList = new List<tbt_InventoryCheckingSlipDetail>();
                    param.CurrentPage = 1;
                }
                if (param.DetailList_ForView == null)
                {
                    param.DetailList_ForView = new List<dtCheckingDetailList>();
                }

                if (param.DetailList.Count > 0)
                {
                    if (isShowAtCurrentPage)
                    {
                        list = (from p in param.DetailList_ForView where p.Page == param.CurrentPage select p).ToList<dtCheckingDetailList>();
                    }
                    else
                    {
                        list = param.DetailList_ForView;
                    }
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtCheckingDetailList>(list, "Inventory\\IVS160", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);
        }

        /// <summary>
        /// Register checking instrument.<br />
        /// - Check system suspending.<br />
        /// - Check implement stock checking.<br />
        /// - Insert checking slip.<br />
        /// - Generate report.
        /// </summary>
        /// <param name="gridCurrentPage"></param>
        /// <param name="checkingDate"></param>
        /// <returns></returns>
        public ActionResult IVS160_Confirm()
        {
            string strInventorySlipNo = string.Empty;
            string slipNoReportPath = string.Empty;

            ObjectResultData res = new ObjectResultData();

            try
            {
                IVS160_ScreenParameter param = GetScreenObject<IVS160_ScreenParameter>();
                int iTotalItem = 0;
                int iTotalPage = 0;

                if (param.DetailList != null)
                {
                    if (param.DetailList.Count > 0)
                    {
                        iTotalItem = param.DetailList == null ? 0 : param.DetailList.Count;
                        iTotalPage = param.DetailList == null ? 0 : param.DetailList.Max(m => m.Page);
                    }
                }

                if (param.DetailList == null)
                {
                    param.DetailList = new List<tbt_InventoryCheckingSlipDetail>();
                    param.CurrentPage = 1;
                }
                if (param.DetailList_ForView == null)
                {
                    param.DetailList_ForView = new List<dtCheckingDetailList>();
                }


                // Is suspend ?
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                // Check for the implement stock checkint
                if (handlerInventory.CheckImplementStockChecking() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }

                ILogHandler handlerLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

                if (param.DetailList.Count > 0)
                {
                    // TODO: (Narupon) Uncomment for use TransactionScope

                    // Save data to database..
                    using (TransactionScope scope = new TransactionScope())
                    {
                        strInventorySlipNo = handlerInventory.GenerateInventorySlipNo(param.OfficeCode, SlipID.C_INV_SLIPID_CHEKCING_INSTRUMENT);
                        param.SlipNo = strInventorySlipNo;

                        // Save header ...
                        List<tbt_InventoryCheckingSlip> header = new List<tbt_InventoryCheckingSlip>();
                        tbt_InventoryCheckingSlip headerItem = new tbt_InventoryCheckingSlip()
                        {
                            SlipNo = strInventorySlipNo,
                            SlipStatus = InventoryCheckingSlipStatus.C_INV_SLIP_STATUS_CHECKING,
                            CheckingYearMonth = param.CheckingYearMonth,
                            CheckingStartDate = param.CheckingDate,
                            LocationCode = param.LocationCode,
                            OfficeCode = param.OfficeCode,
                            CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                            UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                        };
                        header.Add(headerItem);
                        List<tbt_InventoryCheckingSlip> headerSavedResult = handlerInventory.InsertTbt_InventoryCheckingSlip(header);

                        // Save detail..
                        var lstLatestSlip = handlerInventory.GetCheckingDetailList(new doGetCheckingDetailList()
                        {
                            CheckingYearMonth = param.CheckingYearMonth,
                            OfficeCode = param.OfficeCode,
                            LocationCode = param.LocationCode,
                            AreaCode = param.AreaCode,
                        });

                        List<tbt_InventoryCheckingSlipDetail> lstUpdatingDetail = new List<tbt_InventoryCheckingSlipDetail>();
                        foreach (var item in lstLatestSlip)
                        {
                            tbt_InventoryCheckingSlipDetail newRowItem = new tbt_InventoryCheckingSlipDetail()
                            {
                                SlipNo = strInventorySlipNo,
                                InstrumentCode = item.InstrumentCode,
                                RunningNoInSlip = item.RunningNoInSlip,
                                AreaCode = item.AreaCode,
                                ShelfNo = item.ShelfNo,
                                StockQty = item.StockQty,
                                CheckingQty = item.CheckingQty,
                                DefaultCheckingQty = item.CheckingQty,
                                AddFlag = item.AddFlag,
                                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                            };

                            lstUpdatingDetail.Add(newRowItem);
                        };

                        foreach (var newdetail in param.DetailList.Where(d => d.DefaultCheckingQty != d.CheckingQty))
                        {
                            var editdetail = lstUpdatingDetail.Where(d => d.key == newdetail.key).FirstOrDefault();
                            if (editdetail == null)
                            {
                                newdetail.SlipNo = strInventorySlipNo;
                                newdetail.RunningNoInSlip = lstUpdatingDetail.Max(d => d.RunningNoInSlip) + 1;
                                lstUpdatingDetail.Add(newdetail);
                            }
                            else
                            {
                                editdetail.CheckingQty = newdetail.CheckingQty;
                            }
                        }

                        List<tbt_InventoryCheckingSlipDetail> detailSavedResult = handlerInventory.InsertTbt_InventoryCheckingSlipDetail(lstUpdatingDetail);

                        // Generate report ...  // C_INV_REPORT_ID_CHECKING_INSTRUMENT_RESULT = IVR110

                        slipNoReportPath = handlerInventoryDocument.GenerateIVR110FilePath(strInventorySlipNo, param.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        param.SlipNoReportPath = slipNoReportPath;

                        scope.Complete(); // Commit transtion.
                    }

                    // TODO: (Narupon) Uncomment for use TransactionScope

                    res.ResultData = strInventorySlipNo;
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return Json(res);
        }

        ///// <summary>
        ///// Check is report file exist.
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult IVS160_CheckExistFile()
        //{
        //    try
        //    {
        //        IVS160_ScreenParameter param = GetScreenObject<IVS160_ScreenParameter>();
        //        string path = param.SlipNoReportPath;
        //        if (System.IO.File.Exists(path) == true)
        //        {
        //            return Json(1);
        //        }
        //        else
        //        {
        //            return Json(0);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ObjectResultData res = new ObjectResultData();
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }
        //}

        ///// <summary>
        ///// Download report and write log.
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult IVS160_DownloadPdfAndWriteLog()
        //{
        //    try
        //    {

        //        IVS160_ScreenParameter param = GetScreenObject<IVS160_ScreenParameter>();
        //        string fileName = param.SlipNoReportPath;

        //        doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
        //        {
        //            DocumentNo = param.SlipNo,
        //            DocumentCode = ReportID.C_INV_REPORT_ID_CHECKING_INSTRUMENT_RESULT, // IVR110
        //            DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT,
        //            DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
        //            DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo
        //        };


        //        ILogHandler handlerLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
        //        int isSuccess = handlerLog.WriteDocumentDownloadLog(doDownloadLog);

        //        IDocumentHandler handlerDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
        //        Stream reportStream = handlerDoc.GetDocumentReportFileStream(fileName);

        //        return File(reportStream, "application/pdf");


        //    }
        //    catch (Exception ex)
        //    {
        //        ObjectResultData res = new ObjectResultData();
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }
        //}
    }

}


