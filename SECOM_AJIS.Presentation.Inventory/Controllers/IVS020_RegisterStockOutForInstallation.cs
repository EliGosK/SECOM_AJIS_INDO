using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Inventory;
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
        public ActionResult IVS020_Authority(IVS020_ScreenParameter param)
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

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_STOCKOUT, FunctionID.C_FUNC_ID_OPERATE))
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

                List<doOffice> lstHeadOffice = srvInv.GetInventoryHeadOffice();
                if (lstHeadOffice.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4016);
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS020_ScreenParameter>("IVS020", param, res);
        }

        /// <summary>
        /// Initialize screen.
        /// </summary>
        /// <returns>Return ActionResult of the screen.</returns>
        [Initialize("IVS020")]
        public ActionResult IVS020()
        {
            var srvInv = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            var today = DateTime.Today;
            var endoflastmonth = today.AddDays(-today.Day);
            var dateof5businessday = srvInv.GetNextBusinessDateByDefaultOffset(endoflastmonth);

            if (dateof5businessday >= today)
            {
                var beginoflastmonth = endoflastmonth.AddDays(1).AddMonths(-1);
                ViewBag.MinDate = (beginoflastmonth - today).TotalDays;
            }
            else
            {
                var beginofthismonth = endoflastmonth.AddDays(1);
                ViewBag.MinDate = (beginofthismonth - today).TotalDays;
            }
            ViewBag.MaxDate = 0;
            return View();
        }
        #endregion

        #region View
        /// <summary>
        /// Get data for initialize search result grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for initialize search result grid.</returns>
        public ActionResult IVS020_InitialSearchGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS020_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get data for initialize register detail grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data data for initialize register detail grid.</returns>
        public ActionResult IVS020_InitialRegisterGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS020_RegisterStockOut", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Search installation slip data for stock-out.
        /// </summary>
        /// <param name="param">DO of searching parameter.</param>
        /// <param name="OfficeCodeList">List of office code which user is belonging.</param>
        /// <returns>Return ActionResult of JSON data for search result grid.</returns>
        public ActionResult IVS020_SearchInstallationSlipForStockOut(doGetInstallationSlipForStockOut param, List<string> OfficeCodeList)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (CommonUtil.IsNullAllField(param))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else
                {
                    if (!string.IsNullOrEmpty(param.ContractCode))
                    {
                        CommonUtil cmm = new CommonUtil();
                        param.ContractCode = cmm.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }

                    if (string.IsNullOrEmpty(param.OfficeCode))
                    {
                        param.OfficeCode = "," + string.Join(",", OfficeCodeList) + ",";
                    }
                    else
                    {
                        param.OfficeCode = "," + param.OfficeCode + ",";
                    }

                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    var lst = service.GetInstallationSlipForStockOut(param);
                    //if (lst == null || lst.Count == 0)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4085);
                    //}
                    res.ResultData = CommonUtil.ConvertToXml<doResultInstallationSlipForStockOut>(lst, "inventory\\IVS020_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);

                    //Add by Jutarat A. on 27112012
                    IVS020_ScreenParameter sParam = this.GetScreenObject<IVS020_ScreenParameter>();
                    sParam.InstallationSlipForStockOutList = lst;
                    //End Add
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
        /// Search installation slip detail data for stock-out.
        /// </summary>
        /// <param name="param">DO of searchig parameter.</param>
        /// <param name="strProjectCode">Project code of searching installation.</param>
        /// <returns>Return ActionResult of JSON data for register detail grid.</returns>
        public ActionResult IVS020_SearchInstallationDetailForStockOut(doGetInstallationDetailForStockOut param, string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (string.IsNullOrEmpty(param.SlipNo))
                {
                    throw new ArgumentNullException("strSlipNo", "Slip no. can't be empty.");
                }
                else
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    var lst = service.GetInstallationDetailForStockOut(param.SlipNo);


                    IOfficeMasterHandler hMaster = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                    var lstAllOffice = hMaster.GetTbm_Office();
                    var lstInvHeadOfficeCode = (
                        from p in lstAllOffice
                        where p.FunctionLogistic == FunctionLogistic.C_FUNC_LOGISTIC_HQ
                        select p.OfficeCode
                    ).ToList<string>();

                    bool bProjectStatus = false;

                    //Add by Jutarat A. on 12122012
                    if (String.IsNullOrEmpty(strProjectCode))
                    {
                        bProjectStatus = true; //false; Modify by Jutarat A. on 26122012
                    }
                    //End Add
                    else
                    {
                        var srvProject = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                        var lstTbtProj = srvProject.GetTbt_Project(strProjectCode);
                        if (lstTbtProj != null &&
                            lstTbtProj.Any(p =>
                                p.ProjectStatus == ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE ||
                                p.ProjectStatus == ProjectStatus.C_PROJECT_STATUS_CANCEL
                            )
                        )
                        {
                            bProjectStatus = true;
                        }
                    }

                    foreach (doResultInstallationDetailForStockOut p in lst)
                    {
                        if (!lstInvHeadOfficeCode.Contains(param.OfficeCode) || !bProjectStatus)
                        {
                            p.SaleShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                            p.RentalShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                            p.SampleShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                            p.SecondShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        }
                    }

                    //res.ResultData = CommonUtil.ConvertToXml<doResultInstallationDetailForStockOut>(lst, "inventory\\IVS020_RegisterStockOut", CommonUtil.GRID_EMPTY_TYPE.VIEW);
                    res.ResultData = IVS020_SearchInstallationDetailForStockOut_CreateResult(true, lst, bProjectStatus);
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                res.ResultData = IVS020_SearchInstallationDetailForStockOut_CreateResult(true, null, null);
            }

            return Json(res);
        }

        /// <summary>
        /// Registering installation data for stock-out.
        /// </summary>
        /// <param name="param">DO of registration's parameter.</param>
        /// <returns>Return ActionResult of registration process result.</returns>
        public ActionResult IVS020_RegisterInstallationForStockOut(IVS020_RegisterInstallationForStockOut_Param param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (srvCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.ResultData = this.IVS020_RegisterInstallationForStockOut_CreateResult(false);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_STOCKOUT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    res.ResultData = this.IVS020_RegisterInstallationForStockOut_CreateResult(false);
                    return Json(res);
                }

                IInventoryHandler srvInv = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                if (srvInv.CheckStartedStockChecking() == 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    res.ResultData = this.IVS020_RegisterInstallationForStockOut_CreateResult(false);
                    return Json(res);
                }

                IInstallationHandler srvInstall = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                var lstInstallation = srvInstall.GetTbt_InstallationSlipData(param.header.SlipNo);
                if (lstInstallation == null
                    || lstInstallation.SlipStatus != SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT
                )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4142);
                    res.ResultData = this.IVS020_RegisterInstallationForStockOut_CreateResult(false);
                    return Json(res);
                }

                //Check row count on ‘Register stock-out list’ subsection
                if (param.details.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    res.ResultData = this.IVS020_RegisterInstallationForStockOut_CreateResult(false);
                    return Json(res);
                }

                // LINQ Sort detail by ParentInstrumentCode, ChildInstrumentCode
                var qSortedDetail = (
                    from d in param.details
                    orderby d.ParentInstrumentCode, d.ChildInstrumentCode
                    select d
                );

                string strParentInstCode = null;
                int intParentStkOutQty = 0;

                //Validate inputted value in list
                foreach (var d in qSortedDetail)
                {
                    var intCurrentStkOutQty = (d.NewInstSale + d.NewInstSample + d.NewInstRental + d.SecondhandInstRental);

                    try
                    {
                        if (d.AddInstalledQty != intCurrentStkOutQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4090,
                                new string[] { d.ChildInstrumentCode },
                                new string[] { d.NewInstSaleCtrlID, d.NewInstSampleCtrlID, d.NewInstRentalCtrlID, d.SecondhandInstRentalCtrlID });
                            continue;
                        }
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(d.ParentInstrumentCode))
                        {
                            if (d.ParentInstrumentCode != strParentInstCode)
                            {
                                strParentInstCode = d.ParentInstrumentCode;
                                intParentStkOutQty = intCurrentStkOutQty;
                            }
                        }
                    }
                }

                // LINQ Group data by ChildInstrumentCode, xxxShelfNo
                var qGroupByChildInst = (
                    from d in param.details
                    group d by new
                    {
                        InstrumentCode = d.ChildInstrumentCode,
                        SaleShelfNo = d.SaleShelfNo,
                        RentalShelfNo = d.RentalShelfNo,
                        SampleShelfNo = d.SampleShelfNo,
                        SecondShelfNo = d.SecondShelfNo
                    }
                        into GroupByChildInst
                        orderby GroupByChildInst.Key.InstrumentCode
                        select GroupByChildInst
                );

                foreach (var gInstrument in qGroupByChildInst)
                {
                    int intTotalNewInstSale = gInstrument.Sum(d => d.NewInstSale);
                    int intTotalNewInstSample = gInstrument.Sum(d => d.NewInstSample);
                    int intTotalNewInstRental = gInstrument.Sum(d => d.NewInstRental);
                    int intTotalSecondhandInstRental = gInstrument.Sum(d => d.SecondhandInstRental);

                    var pCheck = new doGetInstallationStockOutForChecking()
                    {
                        OfficeCode = param.header.OfficeCode,
                        SlipNo = param.header.SlipNo,
                        InstrumentCode = gInstrument.Key.InstrumentCode,
                        ProjectCode = param.header.ProjectCode,
                        SaleShelfNo = gInstrument.Key.SaleShelfNo,
                        RentalShelfNo = gInstrument.Key.RentalShelfNo,
                        SampleShelfNo = gInstrument.Key.SampleShelfNo,
                        SecondShelfNo = gInstrument.Key.SecondShelfNo
                    };

                    var lstChecking = srvInv.GetInstallationStockOutForChecking(pCheck);
                    if (lstChecking.Count > 0)
                    {
                        if (intTotalNewInstSale != 0 && intTotalNewInstSale > lstChecking[0].SaleQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, (string.IsNullOrEmpty(param.header.ProjectCode) ? MessageUtil.MessageList.MSG4086 : MessageUtil.MessageList.MSG4135),
                                new string[] { gInstrument.Key.InstrumentCode },
                                (from d in gInstrument select d.NewInstSaleCtrlID).ToArray());
                        }
                        if (intTotalNewInstSample != 0 && intTotalNewInstSample > lstChecking[0].SampleQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, (string.IsNullOrEmpty(param.header.ProjectCode) ? MessageUtil.MessageList.MSG4087 : MessageUtil.MessageList.MSG4136),
                                new string[] { gInstrument.Key.InstrumentCode },
                                (from d in gInstrument select d.NewInstSampleCtrlID).ToArray());
                        }
                        if (intTotalNewInstRental != 0 && intTotalNewInstRental > lstChecking[0].RentalQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, (string.IsNullOrEmpty(param.header.ProjectCode) ? MessageUtil.MessageList.MSG4088 : MessageUtil.MessageList.MSG4137),
                                new string[] { gInstrument.Key.InstrumentCode },
                                (from d in gInstrument select d.NewInstRentalCtrlID).ToArray());
                        }
                        if (intTotalSecondhandInstRental != 0 && intTotalSecondhandInstRental > lstChecking[0].SecondQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, (string.IsNullOrEmpty(param.header.ProjectCode) ? MessageUtil.MessageList.MSG4089 : MessageUtil.MessageList.MSG4138),
                                new string[] { gInstrument.Key.InstrumentCode },
                                (from d in gInstrument select d.SecondhandInstRentalCtrlID).ToArray());
                        }
                    }
                }

                if (res.MessageList != null)
                {
                    res.ResultData = this.IVS020_RegisterInstallationForStockOut_CreateResult(false);
                }
                else
                {
                    res.ResultData = this.IVS020_RegisterInstallationForStockOut_CreateResult(true);
                }

                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                res.ResultData = this.IVS020_RegisterInstallationForStockOut_CreateResult(false);
                return Json(res);
            }

        }

        /// <summary>
        /// Confirm registering installation data for stock-out.
        /// </summary>
        /// <param name="param">DO of confirmation's parameter.</param>
        /// <returns>Return ActionResult of confirmation process result.</returns>
        public ActionResult IVS020_ConfirmInstallationForStockOut(IVS020_RegisterInstallationForStockOut_Param param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION; //All message during confirmation process must be information dialog box.

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                IVS020_ScreenParameter sParam = this.GetScreenObject<IVS020_ScreenParameter>(); //Add by Jutarat A. on 27112012

                ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (srvCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.ResultData = this.IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_STOCKOUT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    res.ResultData = this.IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                    return Json(res);
                }

                IInventoryHandler srvInv = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                IInstallationHandler srvInstall = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                if (srvInv.CheckStartedStockChecking() == 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    res.ResultData = this.IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                    return Json(res);
                }

                var lstInstallation = srvInstall.GetTbt_InstallationSlipData(param.header.SlipNo);
                if (lstInstallation == null
                    || lstInstallation.SlipStatus != SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT
                )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4142);
                    res.ResultData = this.IVS020_RegisterInstallationForStockOut_CreateResult(false);
                    return Json(res);
                }

                //Check row count on ‘Register stock-out list’ subsection
                if (param.details.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    res.ResultData = this.IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                    return Json(res);
                }

                // LINQ Sort detail by ParentInstrumentCode, ChildInstrumentCode
                var qSortedDetail = (
                    from d in param.details
                    orderby d.ParentInstrumentCode, d.ChildInstrumentCode
                    select d
                );

                string strParentInstCode = null;
                int intParentStkOutQty = 0;

                //Validate inputted value in list
                foreach (var d in qSortedDetail)
                {
                    var intCurrentStkOutQty = (d.NewInstSale + d.NewInstSample + d.NewInstRental + d.SecondhandInstRental);

                    try
                    {
                        if (d.AddInstalledQty != intCurrentStkOutQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4090,
                                new string[] { d.ChildInstrumentCode },
                                new string[] { d.NewInstSaleCtrlID, d.NewInstSampleCtrlID, d.NewInstRentalCtrlID, d.SecondhandInstRentalCtrlID });
                            //continue;
                            res.ResultData = this.IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                            return Json(res);
                        }
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(d.ParentInstrumentCode))
                        {
                            if (d.ParentInstrumentCode != strParentInstCode)
                            {
                                strParentInstCode = d.ParentInstrumentCode;
                                intParentStkOutQty = intCurrentStkOutQty;
                            }
                        }
                    }
                }

                // LINQ Group data by ChildInstrumentCode, xxxShelfNo
                var qGroupByChildInst = (
                    from d in param.details
                    group d by new
                    {
                        InstrumentCode = d.ChildInstrumentCode,
                        SaleShelfNo = d.SaleShelfNo,
                        RentalShelfNo = d.RentalShelfNo,
                        SampleShelfNo = d.SampleShelfNo,
                        SecondShelfNo = d.SecondShelfNo
                    }
                        into GroupByChildInst
                        orderby GroupByChildInst.Key.InstrumentCode
                        select GroupByChildInst
                );

                foreach (var gInstrument in qGroupByChildInst)
                {
                    int intTotalNewInstSale = gInstrument.Sum(d => d.NewInstSale);
                    int intTotalNewInstSample = gInstrument.Sum(d => d.NewInstSample);
                    int intTotalNewInstRental = gInstrument.Sum(d => d.NewInstRental);
                    int intTotalSecondhandInstRental = gInstrument.Sum(d => d.SecondhandInstRental);

                    var pCheck = new doGetInstallationStockOutForChecking()
                    {
                        OfficeCode = param.header.OfficeCode,
                        SlipNo = param.header.SlipNo,
                        InstrumentCode = gInstrument.Key.InstrumentCode,
                        ProjectCode = param.header.ProjectCode,
                        SaleShelfNo = gInstrument.Key.SaleShelfNo,
                        RentalShelfNo = gInstrument.Key.RentalShelfNo,
                        SampleShelfNo = gInstrument.Key.SampleShelfNo,
                        SecondShelfNo = gInstrument.Key.SecondShelfNo
                    };

                    var lstChecking = srvInv.GetInstallationStockOutForChecking(pCheck);
                    if (lstChecking.Count > 0)
                    {
                        if (intTotalNewInstSale != 0 && intTotalNewInstSale > lstChecking[0].SaleQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, (string.IsNullOrEmpty(param.header.ProjectCode) ? MessageUtil.MessageList.MSG4086 : MessageUtil.MessageList.MSG4135),
                                new string[] { gInstrument.Key.InstrumentCode },
                                (from d in gInstrument select d.NewInstSaleCtrlID).ToArray());
                            res.ResultData = this.IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                            return Json(res);
                        }
                        if (intTotalNewInstSample != 0 && intTotalNewInstSample > lstChecking[0].SampleQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, (string.IsNullOrEmpty(param.header.ProjectCode) ? MessageUtil.MessageList.MSG4087 : MessageUtil.MessageList.MSG4136),
                                new string[] { gInstrument.Key.InstrumentCode },
                                (from d in gInstrument select d.NewInstSampleCtrlID).ToArray());
                            res.ResultData = this.IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                            return Json(res);
                        }
                        if (intTotalNewInstRental != 0 && intTotalNewInstRental > lstChecking[0].RentalQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, (string.IsNullOrEmpty(param.header.ProjectCode) ? MessageUtil.MessageList.MSG4088 : MessageUtil.MessageList.MSG4137),
                                new string[] { gInstrument.Key.InstrumentCode },
                                (from d in gInstrument select d.NewInstRentalCtrlID).ToArray());
                            res.ResultData = this.IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                            return Json(res);
                        }
                        if (intTotalSecondhandInstRental != 0 && intTotalSecondhandInstRental > lstChecking[0].SecondQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, (string.IsNullOrEmpty(param.header.ProjectCode) ? MessageUtil.MessageList.MSG4089 : MessageUtil.MessageList.MSG4138),
                                new string[] { gInstrument.Key.InstrumentCode },
                                (from d in gInstrument select d.SecondhandInstRentalCtrlID).ToArray());
                            res.ResultData = this.IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                            return Json(res);
                        }
                    }
                }

                //if (res.MessageList != null)
                //{
                //    res.ResultData = IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                //    return Json(res);
                //}

                var lstHOffice = srvInv.GetInventoryHeadOffice();
                var objRegister = new doRegisterTransferInstrumentData()
                {
                    SlipId = SlipID.C_INV_SLIPID_STOCK_OUT,
                    InventorySlip = new tbt_InventorySlip()
                    {
                        SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE,
                        InstallationSlipNo = param.header.SlipNo,
                        ProjectCode = param.header.ProjectCode,
                        ContractCode = param.header.ContractCode,
                        SlipIssueDate = DateTime.Now,
                        StockOutDate = param.StockOutDate,
                        SourceLocationCode = (param.ProjectStatus.GetValueOrDefault(false) ? InstrumentLocation.C_INV_LOC_INSTOCK : InstrumentLocation.C_INV_LOC_PROJECT_WIP),
                        DestinationLocationCode = InstrumentLocation.C_INV_LOC_WIP,
                        SourceOfficeCode = param.header.OfficeCode,
                        DestinationOfficeCode = lstHOffice[0].OfficeCode,
                        ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL,
                        CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                        UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                        Memo = param.Memo
                    }
                };

                if (param.header.SlipType == SlipID.C_INV_SLIPID_NEW_SALE)
                {
                    objRegister.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20;
                }
                else if (param.header.SlipType == SlipID.C_INV_SLIPID_NEW_RENTAL)
                {
                    objRegister.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03;
                }
                else if (param.header.SlipType == SlipID.C_INV_SLIPID_CHANGE_INSTALLATION)
                {
                    objRegister.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29;
                }

                objRegister.lstTbt_InventorySlipDetail = param.details.ToTbt_InventorySlipDetail();

                //Begin Transaction
                using (TransactionScope scope = new TransactionScope())
                {
                    //Update inventory transfer data
                    var strInvSlipNo = srvInv.RegisterTransferInstrument(objRegister);

                    //Update decrease in Project WIP WH
                    if (!string.IsNullOrEmpty(param.header.ProjectCode))
                    {
                        List<tbt_InventoryProjectWIP> lstUpdateProjectWIP = (
                            from p in objRegister.lstTbt_InventorySlipDetail
                            select new tbt_InventoryProjectWIP()
                            {
                                ProjectCode = objRegister.InventorySlip.ProjectCode,
                                AreaCode = p.SourceAreaCode,
                                InstrumentCode = p.InstrumentCode,
                                InstrumentQty = -p.TransferQty
                            }
                        ).ToList();

                        srvInv.UpdateTbt_InventoryProjectWIP(lstUpdateProjectWIP);
                    }

                    //7.8	Check there are new instrument
                    if (srvInv.CheckNewInstrument(strInvSlipNo) == 1)
                    {
                        //7.8.1	Prepare data for update new instrument to account stock data
                        var lstNewInst = srvInv.GetGroupNewInstrument(strInvSlipNo);

                        //7.8.2
                        foreach (var objNewInst in lstNewInst)
                        {
                            //7.8.2.1	Calculate Moving average price
                            #region Monthly Price @ 2015
                            //var dblMovingAvgPrice = srvInv.CalculateMovingAveragePrice(objNewInst);
                            var dblMovingAvgPrice = srvInv.GetMonthlyAveragePrice(objNewInst.Instrumentcode, objRegister.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                            #endregion

                            //7.8.2.2	Update transfer data to Account DB
                            var bUpdated = srvInv.UpdateAccountTransferNewInstrument(objNewInst, (decimal)dblMovingAvgPrice);

                            //7.8.2.3
                            if (!bUpdated)
                            {
                                scope.Dispose();
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(new Exception("Failed to call UpdateAccountTransferNewInstrument()"));
                                res.ResultData = IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                                return Json(res);
                            }

                            //7.8.2.4	Update transfer data to account moving stock
                            var lstTbtAccountInStock = srvInv.GetTbt_AccountInStock(
                                objNewInst.Instrumentcode, objNewInst.SourceLocationCode, objNewInst.SourceOfficeCode);

                            int iMovingNo = srvInv.GetMovingNumber();
                            var objAccMoving = new tbt_AccountStockMoving()
                            {
                                MovingNo = iMovingNo,
                                SlipNo = strInvSlipNo,
                                SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                                DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                SourceLocationCode = objNewInst.SourceLocationCode,
                                DestinationLocationCode = objNewInst.DestinationLocationCode,
                                InstrumentCode = objNewInst.Instrumentcode,
                                InstrumentQty = objNewInst.TransferQty,
                                InstrumentPrice = (decimal)dblMovingAvgPrice,
                                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                            };

                            if (param.header.SlipType == SlipID.C_INV_SLIPID_NEW_SALE)
                            {
                                objAccMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20;
                            }
                            else if (param.header.SlipType == SlipID.C_INV_SLIPID_NEW_RENTAL)
                            {
                                objAccMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03;
                            }
                            else if (param.header.SlipType == SlipID.C_INV_SLIPID_CHANGE_INSTALLATION)
                            {
                                objAccMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29;
                            }

                            var lstInsAccStkResult = srvInv.InsertAccountStockMoving(new List<tbt_AccountStockMoving>() { objAccMoving });

                            if (lstInsAccStkResult == null || lstInsAccStkResult.Count == 0)
                            {
                                scope.Dispose();
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(new Exception("Failed to call InsertAccountStockMoving()"));
                                res.ResultData = IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                                return Json(res);
                            }
                        }
                    }

                    //7.9	Check there are secondhand instrument
                    if (srvInv.CheckSecondhandInstrument(strInvSlipNo) == 1)
                    {
                        //7.9.1	Prepare data for update secondhand instrument to account stock data
                        var lstSecondInst = srvInv.GetGroupSecondhandInstrument(strInvSlipNo);

                        //7.9.2
                        foreach (var objSecondInst in lstSecondInst)
                        {
                            //7.9.2.1
                            var bUpdated = srvInv.UpdateAccountTransferSecondhandInstrument(objSecondInst);

                            //7.9.2.2
                            if (!bUpdated)
                            {
                                scope.Dispose();
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(new Exception("Failed to call UpdateAccountTransferSecondhandInstrument()"));
                                res.ResultData = IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                                return Json(res);
                            }
                        }
                    }

                    //7.10	Check there are sample instrument
                    if (srvInv.CheckSampleInstrument(strInvSlipNo) == 1)
                    {
                        //7.10.1	Prepare data for update sample instrument to account stock data
                        var lstSampleInst = srvInv.GetGroupSampleInstrument(strInvSlipNo);

                        //7.10.2
                        foreach (var objSampleInst in lstSampleInst)
                        {
                            //7.10.2.1
                            var bUpdated = srvInv.UpdateAccountTransferSampleInstrument(objSampleInst, null);

                            //7.10.2.2
                            if (!bUpdated)
                            {
                                scope.Dispose();
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(new Exception("Failed to call UpdateAccountTransferSampleInstrument()"));
                                res.ResultData = IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                                return Json(res);
                            }

                            //7.10.2.3
                            int iMovingNo = srvInv.GetMovingNumber();
                            var objAccMoving = new tbt_AccountStockMoving()
                            {
                                MovingNo = iMovingNo,
                                SlipNo = strInvSlipNo,
                                SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                                DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                SourceLocationCode = objSampleInst.SourceLocationCode,
                                DestinationLocationCode = objSampleInst.DestinationLocationCode,
                                InstrumentCode = objSampleInst.Instrumentcode,
                                InstrumentQty = objSampleInst.TransferQty,
                                InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT,
                                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                            };

                            if (param.header.SlipType == SlipID.C_INV_SLIPID_NEW_SALE)
                            {
                                objAccMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20;
                            }
                            else if (param.header.SlipType == SlipID.C_INV_SLIPID_NEW_RENTAL)
                            {
                                objAccMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03;
                            }
                            else if (param.header.SlipType == SlipID.C_INV_SLIPID_CHANGE_INSTALLATION)
                            {
                                objAccMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29;
                            }

                            var lstInsAccStkResult = srvInv.InsertAccountStockMoving(new List<tbt_AccountStockMoving>() { objAccMoving });

                            if (lstInsAccStkResult == null || lstInsAccStkResult.Count == 0)
                            {
                                scope.Dispose();
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(new Exception("Failed to call tbt_AccountStockMoving()"));
                                res.ResultData = IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                                return Json(res);
                            }
                        }
                    }

                    //7.12	Update installation slip data
                    var lstInstList = param.details.ToDoInstrument();

                    //Add by Jutarat A. on 27112012
                    DateTime? InstallSlipUpdateDate = null;
                    if (sParam.InstallationSlipForStockOutList != null && sParam.InstallationSlipForStockOutList.Count > 0)
                    {
                        List<doResultInstallationSlipForStockOut>  doResultList = (from t in sParam.InstallationSlipForStockOutList
                                                                                    where t.SlipNo == param.header.SlipNo
                                                                                    select t).ToList<doResultInstallationSlipForStockOut>();
                        if (doResultList != null && doResultList.Count > 0)
                            InstallSlipUpdateDate = doResultList[0].UpdateDate;
                    }
                    //End Add

                    //7.12.2
                    var blnProcessResult = srvInstall.UpdateStockOutInstrument(param.header.SlipNo, FlagType.C_FLAG_ON, param.header.OfficeCode, lstInstList, InstallSlipUpdateDate); //Modify by Jutarat A. on 27112012 (Add InstallSlipUpdateDate)

                    //7.12.3
                    if (!blnProcessResult)
                    {
                        scope.Dispose();
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(new Exception("Failed to call UpdateStockOutInstrument()"));
                        res.ResultData = IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
                        return Json(res);
                    }

                    //7.13	Update stock-out data to booking DB
                    var lstBooking = srvInv.GetTbt_InventoryBooking(param.header.ContractCode);
                    //7.13.2
                    if (lstBooking.Count > 0)
                    {
                        //7.13.2.1
                        var objBooking = new doBooking()
                        {
                            ContractCode = param.header.ContractCode
                        };

                        var qGrpByChildInst = (
                            from d in param.details
                            where (d.NewInstSale + d.NewInstRental + d.NewInstSample + d.SecondhandInstRental) != 0
                            group d by d.ChildInstrumentCode into grpChildInst
                            orderby grpChildInst.Key
                            select grpChildInst
                        );
                        objBooking.InstrumentCode = new List<string>(qGrpByChildInst.Select(grpChild => grpChild.Key));
                        objBooking.InstrumentQty = new List<int>(qGrpByChildInst.Select(grpChild => grpChild.Sum(d => d.NewInstSale + d.NewInstRental + d.NewInstSample + d.SecondhandInstRental)));

                        srvInv.UpdateStockOutInstrument(objBooking);

                    }

                    //7.14	Genereate inventory slip report  // ReportID.C_INV_REPORT_ID_STOCKOUT = IVR180
                    IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                    string strSlipNoReportPath = handlerInventoryDocument.GenerateIVR180FilePath(strInvSlipNo, param.header.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                    // 7.15	Update Sale contract
                    var srvSale = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                    string strLatestSaleOCC = srvSale.GetLastOCC(param.header.ContractCode);
                    if (!string.IsNullOrEmpty(strLatestSaleOCC))
                    {
                        var lstSaleBasic = srvSale.GetTbt_SaleBasic(param.header.ContractCode, strLatestSaleOCC, null);

                        if (lstSaleBasic != null && lstSaleBasic.Count > 0)
                        {
                            foreach (var sb in lstSaleBasic)
                            {
                                if (sb.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_GENCODE) //Add by Jutarat A. on 11092013
                                    sb.SaleProcessManageStatus = SaleProcessManageStatus.C_SALE_PROCESS_STATUS_SHIP;

                                sb.InstrumentStockOutDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                srvSale.UpdateTbt_SaleBasic(sb);
                            }
                        }
                    }

                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
                    res.ResultData = IVS020_ConfirmInstallationForStockOut_CreateResult(true, strInvSlipNo);

                    scope.Complete();
                }


            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                res.ResultData = IVS020_ConfirmInstallationForStockOut_CreateResult(false, null);
            }

            return Json(res);
        }

        /// <summary>
        /// Get document's data for downloading.
        /// </summary>
        /// <param name="strInvSlipNo">Inventory Slip No.</param>
        /// <returns>Return ActionResult of document's data.</returns>
        public ActionResult IVS020_DownloadDocument(string strInvSlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //var docCond = new doDocumentDataCondition()
                //{
                //    DocumentNo = strInvSlipNo,
                //    OCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT,
                //    DocumentCode = ReportID.C_INV_REPORT_ID_STOCKOUT
                //};
                //IDocumentHandler docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                //var lstDocs = docHandler.GetDocumentDataList(docCond);

                var srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var lstDocs = srvCommon.GetTbt_DocumentList(strInvSlipNo, ReportID.C_INV_REPORT_ID_STOCKOUT, ConfigName.C_CONFIG_DOC_OCC_DEFAULT);

                if (lstDocs == null || lstDocs.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                    res.ResultData = null;
                }
                else
                {
                    string path = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, lstDocs[0].FilePath); // ReportUtil.GetGeneratedReportPath(lstDocs[0].FilePath);

                    if (System.IO.File.Exists(path) == true)
                    {
                        res.ResultData = lstDocs[0];
                    }
                    else
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                        res.ResultData = null;
                    }
                }
                return Json(res);

                //doDocumentDownloadLog cond = new doDocumentDownloadLog();
                //cond.DocumentNo = docCond.DocumentNo;
                //cond.DocumentCode = docCond.DocumentCode;
                //cond.DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //cond.DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                //cond.DocumentOCC = docCond.OCC;

                //ILogHandler handlerLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                //int isOK = handlerLog.WriteDocumentDownloadLog(cond);

                //string strRptFilePath = lstDocs[0].FilePath;
                //var stream = docHandler.GetDocumentReportFileStream(strRptFilePath);
                //if (stream != null)
                //{
                //    return File(stream, "application/pdf");
                //}
                //else
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                //    res.AddErrorMessage(new Exception("Report file not found."));
                //    return Json(res);
                //}


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
        public ActionResult IVS020_DownloadPdfAndWriteLog(string strDocumentNo, string documentOCC, string strDocumentCode, string fileName)
        {
            try
            {
                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = strDocumentNo,
                    DocumentCode = strDocumentCode,
                    DocumentOCC = documentOCC,
                    DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo
                };

                ILogHandler handlerLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                int isSuccess = handlerLog.WriteDocumentDownloadLog(doDownloadLog);

                IDocumentHandler handlerDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                Stream reportStream = handlerDoc.GetDocumentReportFileStream(fileName);

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
        /// Create result data for IVS020_RegisterInstallationForStockOut controller.
        /// </summary>
        /// <param name="bIsSuccess">Boolean, indicate the controller's successful.</param>
        /// <returns>Object of result data.</returns>
        private object IVS020_RegisterInstallationForStockOut_CreateResult(bool bIsSuccess)
        {
            return new
            {
                IsSuccess = bIsSuccess
            };
        }

        /// <summary>
        /// Create result data for IVS020_ConfirmInstallationForStockOut controller.
        /// </summary>
        /// <param name="bIsSuccess">Boolean, indicate the controller's successful.</param>
        /// <param name="strInvSlipNo">Inventory Slip No.</param>
        /// <returns>Object of result data</returns>
        private object IVS020_ConfirmInstallationForStockOut_CreateResult(bool bIsSuccess, string strInvSlipNo)
        {
            return new
            {
                IsSuccess = bIsSuccess,
                InvSlipNo = strInvSlipNo
            };
        }

        private object IVS020_SearchInstallationDetailForStockOut_CreateResult(bool bIsSuccess, List<doResultInstallationDetailForStockOut> lstResultData, bool? bProjectStatus)
        {
            return new
            {
                IsSuccess = bIsSuccess,
                ResultData = lstResultData,
                ProjectStatus = bProjectStatus
            };
        }
        #endregion

    }

}


