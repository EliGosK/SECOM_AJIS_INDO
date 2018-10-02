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




namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {

        // Authority

        /// <summary>
        /// - Check system suspending.<br />
        /// - Check user permission for screen IVS150.<br />
        /// - Check freezed data.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS150_Authority(IVS150_ScreenParameter param)
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
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_START_STOP_CHECKING, FunctionID.C_FUNC_ID_OPERATE) == false)
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

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS150_ScreenParameter>("IVS150", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS150")]
        public ActionResult IVS150()
        {
            ViewBag.EnableBtnStartingChecking = "0";
            ViewBag.EnableBtnStopChecking = "0";

            try
            {
                //List<UserBelongingData> lsUserBelonging = (from p in CommonUtil.dsTransData.dtUserBelongingData
                //                                           where p.DepartmentCode == DepartmentMaster.C_DEPT_PURCHASE
                //                                           select p).ToList<UserBelongingData>();
                //if (lsUserBelonging.Count > 0)
                //{
                //    IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                //    List<tbt_InventoryCheckingSchedule> list = handlerInventory.GetLastCheckingSchedule();
                //    if (list.Count > 0)
                //    {
                //        if (list[0].CheckingStatus == CheckingStatus.C_INV_CHECKING_STATUS_PREPARING)
                //        {
                //            ViewBag.EnableBtnStartingChecking = "1";
                //            ViewBag.EnableBtnStopChecking = "0";
                //        }
                //        else if (list[0].CheckingStatus == CheckingStatus.C_INV_CHECKING_STATUS_IMPLEMENTING)
                //        {
                //            ViewBag.EnableBtnStartingChecking = "0";
                //            ViewBag.EnableBtnStopChecking = "1";
                //        }
                //        else if (list[0].CheckingStatus == CheckingStatus.C_INV_CHECKING_STATUS_STOPPING)
                //        {
                //            ViewBag.EnableBtnStartingChecking = "0";
                //            ViewBag.EnableBtnStopChecking = "0";
                //        }
                //    }
                //}



                // Narupon  
                var enableButton = IVS150_GetPermission();
                string EnableBtnStartingChecking = enableButton.EnableBtnStartingChecking == true ? "1" : "0";
                string EnableBtnStopChecking = enableButton.EnableBtnStopChecking == true ? "1" : "0";

                ViewBag.EnableBtnStartingChecking = EnableBtnStartingChecking;
                ViewBag.EnableBtnStopChecking = EnableBtnStopChecking;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }

        /// <summary>
        /// Check for enable button.
        /// </summary>
        /// <returns></returns>
        private EnableButton IVS150_GetPermission()
        {
            EnableButton en = new EnableButton();

            try
            {
                List<UserBelongingData> lsUserBelonging = (from p in CommonUtil.dsTransData.dtUserBelongingData
                                                           where p.DepartmentCode == DepartmentMaster.C_DEPT_PURCHASE
                                                           select p).ToList<UserBelongingData>();
                if (lsUserBelonging.Count > 0)
                {
                    IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    List<tbt_InventoryCheckingSchedule> list = handlerInventory.GetLastCheckingSchedule();
                    if (list.Count > 0)
                    {
                        if (list[0].CheckingStatus == CheckingStatus.C_INV_CHECKING_STATUS_PREPARING)
                        {
                            en.EnableBtnStartingChecking = true;
                            en.EnableBtnStopChecking = false;
                        }
                        else if (list[0].CheckingStatus == CheckingStatus.C_INV_CHECKING_STATUS_IMPLEMENTING)
                        {
                            en.EnableBtnStartingChecking = false;
                            en.EnableBtnStopChecking = true;
                        }
                        else if (list[0].CheckingStatus == CheckingStatus.C_INV_CHECKING_STATUS_STOPPING)
                        {
                            en.EnableBtnStartingChecking = false;
                            en.EnableBtnStopChecking = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return en;
        }

        /// <summary>
        /// Get config for history table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS150_InitialHistoryGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS150_History", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get config for office checking table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS150_InitialOfficeCheckingListGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS150_OfficeChecking", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Check for enable start/stop button.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS150_GetEnableStartStopButton()
        {
            bool enableBtnStartingChecking = false;
            bool enableBtnStopChecking = false;
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<UserBelongingData> lsUserBelonging = (from p in CommonUtil.dsTransData.dtUserBelongingData
                                                           where p.DepartmentCode == DepartmentMaster.C_DEPT_PURCHASE
                                                           select p).ToList<UserBelongingData>();
                if (lsUserBelonging.Count > 0)
                {
                    IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    List<tbt_InventoryCheckingSchedule> list = handlerInventory.GetLastCheckingSchedule();
                    if (list.Count > 0)
                    {
                        if (list[0].CheckingStatus == CheckingStatus.C_INV_CHECKING_STATUS_PREPARING)
                        {
                            enableBtnStartingChecking = true;
                            enableBtnStopChecking = false;
                        }
                        else if (list[0].CheckingStatus == CheckingStatus.C_INV_CHECKING_STATUS_IMPLEMENTING)
                        {
                            enableBtnStartingChecking = false;
                            enableBtnStopChecking = true;
                        }
                        else if (list[0].CheckingStatus == CheckingStatus.C_INV_CHECKING_STATUS_STOPPING)
                        {
                            enableBtnStartingChecking = false;
                            enableBtnStopChecking = false;
                        }
                    }
                }


                EnableButton result = new EnableButton()
                {
                    EnableBtnStartingChecking = enableBtnStartingChecking,
                    EnableBtnStopChecking = enableBtnStopChecking
                };

                res.ResultData = result;

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get checking status history of selected year.
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public ActionResult IVS150_GetCheckingStatusHistory(string Year)
        {
            ObjectResultData res = new ObjectResultData();
            List<dtCheckingStatusList> list = new List<dtCheckingStatusList>();
            try
            {
                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                list = handlerInventory.GetCheckingStatusList(Year, MiscType.C_INV_CHECKING_STATUS);

                // Language Mapping
                CommonUtil.MappingObjectLanguage<dtCheckingStatusList>(list);


            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtCheckingStatusList>(list, "Inventory\\IVS150_History", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);
        }

        /// <summary>
        /// Get office checking list.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS150_GetOfficeCheckingList()
        {
            ObjectResultData res = new ObjectResultData();
            List<dtOfficeCheckingList> list = new List<dtOfficeCheckingList>();
            try
            {
                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                list = handlerInventory.GetOfficeCheckingList(MiscType.C_INV_LOC);

                // Language Mapping
                CommonUtil.MappingObjectLanguage<dtOfficeCheckingList>(list);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtOfficeCheckingList>(list, "Inventory\\IVS150_OfficeChecking", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);
        }

        /// <summary>
        /// Start checking.<br />
        /// - Check enable start button.<br />
        /// - Check system suspending.<br />
        /// - Check status of last checking schedule must be preparing.<br />
        /// - Update inventory checking schedule.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS150_StartChecking()
        {
            ObjectResultData res = new ObjectResultData();
            string result = "0";
            try
            {
                var enableButton = IVS150_GetPermission();

                if (enableButton.EnableBtnStartingChecking)
                {
                    IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                    // Is suspend ?
                    ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    if (handler.IsSystemSuspending())
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                        return Json(res);
                    }

                    var list = handlerInventory.GetLastCheckingSchedule();

                    if (list.Count > 0)
                    {
                        if (list[0].CheckingStatus != CheckingStatus.C_INV_CHECKING_STATUS_PREPARING)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4073);
                            return Json(res);
                        }

                        // update field for update Tbt_InventoryCheckingSchedule
                        list[0].CheckingStartDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        list[0].CheckingStatus = CheckingStatus.C_INV_CHECKING_STATUS_IMPLEMENTING;
                        list[0].StartCheckingBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        list[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        list[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                        using (TransactionScope scope = new TransactionScope())
                        {
                            string xml = CommonUtil.ConvertToXml_Store(list);
                            handlerInventory.UpdateTbt_InventoryCheckingSchedule(xml);

                            scope.Complete();

                            result = "1";
                        }
                    }
                }

                res.ResultData = result;

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Stop checking.<br />
        /// - Check enable start button.<br />
        /// - Check system suspending.<br />
        /// - Check status of last checking schedule must be implementing.<br />
        /// - Update inventory checking schedule.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS150_StopChecking()
        {
            ObjectResultData res = new ObjectResultData();
            string result = "0";
            try
            {
                var enableButton = IVS150_GetPermission();

                if (enableButton.EnableBtnStopChecking)
                {
                    IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                    // Is suspend ?
                    ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    if (handler.IsSystemSuspending())
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                        return Json(res);
                    }

                    var list = handlerInventory.GetLastCheckingSchedule();


                    if (list.Count > 0)
                    {
                        if (list[0].CheckingStatus != CheckingStatus.C_INV_CHECKING_STATUS_IMPLEMENTING)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4075);
                            return Json(res);
                        }

                        // update field for update Tbt_InventoryCheckingSchedule
                        list[0].CheckingFinishDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        list[0].CheckingStatus = CheckingStatus.C_INV_CHECKING_STATUS_STOPPING;
                        list[0].FinishCheckingBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        list[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        list[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                        using (TransactionScope scope = new TransactionScope())
                        {
                            string xml = CommonUtil.ConvertToXml_Store(list);
                            handlerInventory.UpdateTbt_InventoryCheckingSchedule(xml);

                            scope.Complete();

                            result = "1";
                        }
                    }
                }

                res.ResultData = result;

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }


    }

}


