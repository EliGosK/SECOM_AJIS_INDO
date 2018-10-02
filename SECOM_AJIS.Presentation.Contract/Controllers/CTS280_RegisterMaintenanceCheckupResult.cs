//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Contract;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Billing;
using System.Transactions;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS280_Authority(CTS280_ScreenParameter CTS280Param)
        {
            #region Mock Input Data
            //Mock input data
            //ContractCodeShow = "N0000008";
            //ProductCode = "001";
            //InstructionDate = new DateTime().AddDays(9).AddMonths(0).AddYears(2010); 
            //Mode = FunctionID.C_FUNC_ID_VIEW;
            //CallerSessionKey = "234567890";
            #endregion

            CommonUtil c = new CommonUtil();
            String ContractCode = c.ConvertContractCode(CTS280Param.ContractCodeShow, CommonUtil.CONVERT_TYPE.TO_LONG);

            ObjectResultData res = new ObjectResultData();

            //1. Check system suspending
            res = checkSuspending();
            if (res.IsError)
                return Json(res);

            //2. Check user's permission
            if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_VIEW) == true
                  || CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_ADD) == true
                  || CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_EDIT) == true
                ))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }


            //3. Check user's authority to view data
            #region Not use this data
            //1.3.1 Get maintenace contract data
            //1.3.11 Get maintenace check-up data
            //IMaintenanceHandler mainHand = ServiceContainer.GetService <IMaintenanceHandler>() as IMaintenanceHandler;
            //List<tbt_MaintenanceCheckup> dtTbt_MaintenanceCheckup  
            //    = mainHand.GetTbt_MaintenanceCheckup(ContractCode, ProductCode, InstructionDate);
            #endregion


            CTS280_InitMACheckupResultData(CTS280Param, res);
            if (res.IsError == true)
                return Json(res);

            return InitialScreenEnvironment<CTS280_ScreenParameter>("CTS280", CTS280Param, res);
        }

        #endregion

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS280")]
        public ActionResult CTS280()
        {
            CTS280_ScreenParameter CTS280Param = GetScreenObject<CTS280_ScreenParameter>();

            //Set ViewBag for Maintenance check-up informaion
            ViewBag.CheckupNo = CTS280Param.data.doMaintCheckupInformation.CheckupNoShow;
            ViewBag.ContractCode = CTS280Param.ContractCodeShow;
            ViewBag.ProductName = CTS280Param.data.doMaintCheckupInformation.ProductName;
            ViewBag.RealCustomerCustCode = CTS280Param.data.doMaintCheckupInformation.RealCustomerCustCodeShow;
            ViewBag.SiteCode = CTS280Param.data.doMaintCheckupInformation.SiteCodeShow;
            ViewBag.UserCode = CTS280Param.data.doMaintCheckupInformation.UserCode;
            ViewBag.RealCustomerNameEN = CTS280Param.data.doMaintCheckupInformation.RealCustomerNameEN;
            ViewBag.SiteNameEN = CTS280Param.data.doMaintCheckupInformation.SiteNameEN;
            ViewBag.RealCustomerNameLC = CTS280Param.data.doMaintCheckupInformation.RealCustomerNameLC;
            ViewBag.SiteNameLC = CTS280Param.data.doMaintCheckupInformation.SiteNameLC;

            //1.2 Get maintenace check-up data
            IMaintenanceHandler mainHand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
            List<tbt_MaintenanceCheckup> dtTbt_MaintenanceCheckup
                = mainHand.GetTbt_MaintenanceCheckup(CTS280Param.ContractCode, CTS280Param.ProductCode, CTS280Param.InstructionDate);
            CTS280Param.data.dtMaintenanceCheckup = dtTbt_MaintenanceCheckup[0];

            //1.3 Get maintenance detail in maintenance contract
            IRentralContractHandler renHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            dsRentalContractData dtEntireContract = renHand.GetEntireContract(CTS280Param.ContractCode, null);
            CTS280Param.data.dtEntireContract = dtEntireContract;


            //Modify by Narut T. 2017-02-10
            //  CTS280Param.data.MaintenanceFeeInit = dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFee;
            //CTS280Param.data.MaintenanceFeeInit = CTS280Param.data.dtMaintenanceCheckup.MaintenanceFee ?? CTS280Param.data.dtMaintenanceCheckup.MaintenanceFeeUsd;

            if (dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                CTS280Param.data.MaintenanceFeeInit = dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFeeUsd;
            else
                CTS280Param.data.MaintenanceFeeInit = dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFee;

            //  CTS280Param.data.MaintenanceFeeInitUsd = 
            //End Modified

            //Set ViewBag for Maintenance check-up result
            if (CTS280Param.Mode == FunctionID.C_FUNC_ID_VIEW)
            {
                //Set Viewbag for show result section
                ViewBag.ExpectedMaintenanceDate = (dtTbt_MaintenanceCheckup[0].ExpectedMaintenanceDate == null) ? "" : dtTbt_MaintenanceCheckup[0].ExpectedMaintenanceDate.Value.ToString("dd-MMM-yyyy");
                ViewBag.MaintenanceDate = (dtTbt_MaintenanceCheckup[0].MaintenanceDate == null) ? "" : dtTbt_MaintenanceCheckup[0].MaintenanceDate.Value.ToString("dd-MMM-yyyy");

                ViewBag.MaintenanceFeeCurrencyType = dtTbt_MaintenanceCheckup[0].MaintenanceFeeCurrencyType;
                if (dtTbt_MaintenanceCheckup[0].MaintenanceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    ViewBag.MaintenanceFee = CommonUtil.TextNumeric(dtTbt_MaintenanceCheckup[0].MaintenanceFeeUsd);
                else
                    ViewBag.MaintenanceFee = CommonUtil.TextNumeric(dtTbt_MaintenanceCheckup[0].MaintenanceFee);

                ViewBag.ApproveNo1 = dtTbt_MaintenanceCheckup[0].ApproveNo1 ?? "";
                ViewBag.SubcontractCode = dtTbt_MaintenanceCheckup[0].SubcontractCode ?? "";
                ViewBag.PICName = dtTbt_MaintenanceCheckup[0].PICName ?? "";
                ViewBag.MaintEmpNo = dtTbt_MaintenanceCheckup[0].MaintEmpNo ?? "";
                ViewBag.MaintEmpName = getEmployeeDisplayName(dtTbt_MaintenanceCheckup[0].MaintEmpNo);
                ViewBag.UsageTime = (dtTbt_MaintenanceCheckup[0].UsageTime == null) ? "" : dtTbt_MaintenanceCheckup[0].UsageTime.Value.ToString();
                ViewBag.InstrumentMalfunctionFlag = dtTbt_MaintenanceCheckup[0].InstrumentMalfunctionFlag ?? false;
                ViewBag.Location = dtTbt_MaintenanceCheckup[0].Location ?? "";
                ViewBag.NeedSalesmanFlag = dtTbt_MaintenanceCheckup[0].NeedSalesmanFlag ?? false;
                ViewBag.MalfunctionDetail = dtTbt_MaintenanceCheckup[0].MalfunctionDetail ?? "";
                ViewBag.Remark = dtTbt_MaintenanceCheckup[0].Remark ?? "";
            }
            else if (CTS280Param.Mode == FunctionID.C_FUNC_ID_ADD)
            {
                ViewBag.ExpectedMaintenanceDate = (dtTbt_MaintenanceCheckup[0].ExpectedMaintenanceDate == null) ? "" : dtTbt_MaintenanceCheckup[0].ExpectedMaintenanceDate.Value.ToString("dd-MMM-yyyy");
                ViewBag.InstrumentMalfunctionFlag = false;
                ViewBag.NeedSalesmanFlag = false;
            }

            ViewBag.Mode = CTS280Param.Mode;
            ViewBag.FunctionIdView = FunctionID.C_FUNC_ID_VIEW;
            ViewBag.FunctionIdAdd = FunctionID.C_FUNC_ID_ADD;
            ViewBag.CallerScreenID = CTS280Param.CallerScreenID;

            UpdateScreenObject(CTS280Param);

            return View("CTS280");
        }
        /// <summary>
        /// Render screen in case view
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS280_RenderViewMode()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS280_ScreenParameter CTS280Param = GetScreenObject<CTS280_ScreenParameter>();
                CTS280_RenderViewMode viewMode = new CTS280_RenderViewMode();
                viewMode.MaintenanceFeeFlag = showMaintFeeFlagViewMode(CTS280Param);
                viewMode.EnableEditButton = enableEditButton(CTS280Param);
                res.ResultData = viewMode;

                CTS280Param.MaintenanceFeeFlag = viewMode.MaintenanceFeeFlag;
                UpdateScreenObject(CTS280Param);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Render screen in case edit
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public ActionResult CTS280_RenderEditMode(int mode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS280_RenderEditMode editMode = new CTS280_RenderEditMode();
                CTS280_ScreenParameter CTS280Param = GetScreenObject<CTS280_ScreenParameter>();

                IMaintenanceHandler mainHand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                bool isAllResultRegistered = mainHand.IsAllResultRegisteredData(CTS280Param.ContractCode, CTS280Param.InstructionDate);

                if (mode == 1) // C_REGISTER_MA_PROCESS_TYPE_INPUT_EXPECTED 
                {
                    editMode.ShowOnlyExpectedMaintenanceDate = true;
                }
                else if (mode == 2) // C_REGISTER_MA_PROCESS_TYPE_REGISTER_RESULT 
                {
                    bool isLastResultRegister = mainHand.IsLastResultToRegisterData(
                                                                    CTS280Param.ContractCode,
                                                                    CTS280Param.ProductCode,
                                                                    CTS280Param.InstructionDate);

                    if (isLastResultRegister == true
                        && CTS280Param.data.dtEntireContract != null)
                    {
                        if (CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails != null)
                        {
                            if (CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails.Count > 0)
                            {
                                if (CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
                                {
                                    editMode.ShowMaintenanceFee = true;

                                    if (CTS280Param.data.dtEntireContract.dtTbt_RentalContractBasic != null)
                                    {
                                        if (CTS280Param.data.dtEntireContract.dtTbt_RentalContractBasic.Count > 0)
                                        {
                                            editMode.IsSetMaintenanceFee = true;

                                            editMode.MetenanceFeeCurrencyType = CTS280Param.data.dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFeeCurrencyType;
                                            if (editMode.MetenanceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                                editMode.MetenanceFee = CTS280Param.data.dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFeeUsd;
                                            else
                                                editMode.MetenanceFee = CTS280Param.data.dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFee;
                                        }
                                    }
                                }
                            }
                        }
                    }   
                }
                else // View mode
                {
                    if (isAllResultRegistered == true
                        && CTS280Param.data.dtEntireContract != null)
                    {
                        if (CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails != null)
                        {
                            if (CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails.Count > 0)
                            {
                                if (CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
                                {
                                    editMode.ShowMaintenanceFee = true;
                                }
                            }
                        }
                    }
                }

                res.ResultData = editMode;
                CTS280Param.MaintenanceFeeFlag = editMode.ShowMaintenanceFee;

                UpdateScreenObject(CTS280Param);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check is show maintenace fee flag section
        /// </summary>
        /// <param name="CTS280Param"></param>
        /// <returns></returns>
        private bool showMaintFeeFlagViewMode(CTS280_ScreenParameter CTS280Param)
        {
            bool result = false;

            IMaintenanceHandler mainHand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
            bool IsAllResultRegistered = mainHand.IsAllResultRegisteredData(CTS280Param.ContractCode, CTS280Param.InstructionDate);
            if (IsAllResultRegistered
                && CTS280Param.data.dtEntireContract != null
                && CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails.Count > 0
                && CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
            {
                return true;
            }

            return result;
        }

        //private Boolean[] showMaintFeeFlagEditMode(CTS280_ScreenParameter CTS280Param)
        //{
        //    bool result = false;
        //    bool setDefaultMaintFee = false;
        //    bool showMaintenanceFee = false;
        //    string txtMaintenanceFee = null;

        //    IMaintenanceHandler mainHand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
        //    bool isLastResultRegister = mainHand.IsLastResultToRegisterData(
        //                                                            CTS280Param.ContractCode,
        //                                                            CTS280Param.ProductCode,
        //                                                            CTS280Param.InstructionDate);

        //    if (isLastResultRegister == true
        //        && CTS280Param.data.dtEntireContract != null)
        //    {
        //        if (CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails != null)
        //        {
        //            if (CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails.Count > 0)
        //            {
        //                if (CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
        //                {
        //                    showMaintenanceFee = true;

        //                    if (CTS280Param.data.dtEntireContract.dtTbt_RentalContractBasic != null)
        //                    {
        //                        if (CTS280Param.data.dtEntireContract.dtTbt_RentalContractBasic.Count > 0)
        //                        {
        //                            txtMaintenanceFee = CommonUtil.TextNumeric(CTS280Param.data.dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFee);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }   
        //    if (CTS280Param.Mode == FunctionID.C_FUNC_ID_VIEW)
        //    {
        //        bool IsAllResultRegistered = mainHand.IsAllResultRegisteredData(CTS280Param.ContractCode, CTS280Param.InstructionDate);
        //        if (IsAllResultRegistered)
        //        {
        //            result = true;
        //            setDefaultMaintFee = false;
        //        }
        //        else
        //        {
        //            result = false;
        //            setDefaultMaintFee = false;
        //        }
        //    }
        //    else
        //    {
        //        //Check whether show MaintenenceFee or not by IsLastResultToRegisterData
        //        bool isLastResult = mainHand.IsLastResultToRegisterData(CTS280Param.ContractCode, CTS280Param.ProductCode, CTS280Param.InstructionDate);
        //        if (isLastResult
        //            && CTS280Param.data.dtEntireContract != null
        //            && CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails.Count > 0
        //            && CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
        //        {
        //            result = true;
        //            setDefaultMaintFee = true;
        //        }
        //        else if (IsAllResultRegistered)
        //        {
        //            result = true;
        //            setDefaultMaintFee = false;
        //        }
        //    }

        //    Boolean[] obj = new Boolean[2];
        //    obj[0] = result;
        //    obj[1] = setDefaultMaintFee;

        //    return obj;
        //}

        /// <summary>
        /// Set Enable flag for each button
        /// </summary>
        /// <param name="CTS280Param"></param>
        /// <returns></returns>
        private bool enableEditButton(CTS280_ScreenParameter CTS280Param)
        {
            bool result = false;

            if (CTS280Param.hasEditPermission)
            //&& CommonUtil.IsNullOrEmpty(CTS280Param.data.dtMaintenanceCheckup.CheckupNo)
            //&& CTS280Param.InstructionDate.Month <= DateTime.Now.Month
            //&& CommonUtil.IsNullOrEmpty(CTS280Param.data.dtMaintenanceCheckup.MaintenanceDate))
            {
                result = true;
            }

            return result;
        }

        //[HttpPost]
        //public ActionResult CTS280_LoadInformationSection(CTS280_InputParam cond)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
        //    try
        //    {
        //        //1.1 Initial 'Maintenance check-up information' section
        //        //1.1.1 Get maintenance check-up information
        //        IMaintenanceHandler mainHand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
        //        List<doMaintenanceCheckupInformation> doMaint = mainHand.GetMaintenanceCheckupInformationData(cond.ParamContractCode, cond.ParamProductCode, cond.ParamInstructionDate);

        //        if (doMaint.Count <= 0)
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3102, new String[] { cond.ParamContractCode, cond.ParamProductCode, cond.ParamInstructionDateShow });
        //            return Json(res);
        //        }
        //        res.ResultData = doMaint[0];
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }
        //    return Json(res);
        //}

        //[HttpPost]
        //public ActionResult CTS280_LoadResultSection(CTS280_InputParam cond)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
        //    try
        //    {
        //        CTS280_Result result = new CTS280_Result();

        //        //1.2 Get maintenance check-up data
        //        IMaintenanceHandler mainHand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
        //        List<tbt_MaintenanceCheckup> tbtMaint = mainHand.GetTbt_MaintenanceCheckup(cond.ParamContractCode, cond.ParamProductCode, cond.ParamInstructionDate);

        //        //1.3 Get maintenance detail in maintenance contract
        //        IRentralContractHandler rentalHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
        //        dsRentalContractData dsRentalData = rentalHand.GetEntireContract(cond.ParamContractCode, null);
        //        decimal? LastOrderContractFee = dsRentalData.dtTbt_RentalContractBasic[0].LastOrderContractFee;

        //        //Mode register or Mode view when edit
        //        if (cond.ParamMode == "edit") //|| (cond.ParamMode == "view" && cond.ParamCurrentMode == "edit"))
        //        {
        //            //Mode Edit Show only ExpectedMaintenanceDate
        //            result.ExpectedMaintenanceDate = tbtMaint[0].ExpectedMaintenanceDate;

        //            //Check whether show MaintenenceFee or not by IsLastResultToRegisterData
        //            bool isLastResult = mainHand.IsLastResultToRegisterData(cond.ParamContractCode, cond.ParamProductCode, cond.ParamInstructionDate);
        //            if (isLastResult && dsRentalData != null && dsRentalData.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
        //            {
        //                result.MaintenanceFee = LastOrderContractFee; //set default MaintenanceFee
        //                result.MaintenanceFeeFlag = true;

        //            }
        //            else
        //            {
        //                result.MaintenanceFeeFlag = false;
        //            }
        //        }
        //        else //Mode view
        //        {
        //            //Mode View Show All Data from tbt_MaintenanceCheckup
        //            result = CommonUtil.CloneObject<tbt_MaintenanceCheckup, CTS280_Result>(tbtMaint[0]);
        //            result.MaintEmpName = getEmployeeDisplayName(result.MaintEmpNo);

        //            //Check whether show MaintenenceFee or not by IsAllResultRegisteredData
        //            bool IsAllResultRegistered = mainHand.IsAllResultRegisteredData(cond.ParamContractCode, cond.ParamInstructionDate);
        //            if (IsAllResultRegistered && dsRentalData != null && dsRentalData.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
        //            {
        //                result.MaintenanceFeeFlag = true;
        //            }
        //            else
        //            {
        //                result.MaintenanceFeeFlag = false;
        //            }
        //        }

        //        result.MaintainenceFeeInit = LastOrderContractFee;
        //        res.ResultData = result;
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }
        //    return Json(res);
        //}

        /// <summary>
        /// Load result data to screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS280_FillResultSection()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS280_ScreenParameter param = GetScreenObject<CTS280_ScreenParameter>();
                CTS280_InitMACheckupResultData(param, res);
                if (res.IsError == true)
                    return Json(res);
                UpdateScreenObject(param);

                res.ResultData = param.data;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Load employee name by code
        /// </summary>
        /// <param name="MaintEmpNo"></param>
        /// <returns></returns>
        public ActionResult CTS280_LoadEmployeeName(string MaintEmpNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //5.1 Get employee
                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_Employee> tbtEmp = masterHandler.GetActiveEmployee(MaintEmpNo);

                //5.2 If can't get employee data from database
                if (tbtEmp.Count <= 0)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, MaintEmpNo);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { MaintEmpNo }, new string[] { "MaintEmpNo" });
                    return Json(res);
                }

                //5.3 Show employee name on screen
                res.ResultData = getEmployeeDisplayName(MaintEmpNo);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get employee name
        /// </summary>
        /// <param name="MaintEmpNo"></param>
        /// <returns></returns>
        private String getEmployeeDisplayName(string MaintEmpNo)
        {
            String empDisplayName = String.Empty;
            try
            {
                IEmployeeMasterHandler employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<dtEmpNo> dtEmpNo = employeeHandler.GetEmployeeNameByEmpNo(MaintEmpNo);
                if (dtEmpNo.Count > 0 && CommonUtil.IsNullOrEmpty(MaintEmpNo) == false)
                    empDisplayName = dtEmpNo[0].EmployeeNameDisplay;
            }
            catch (Exception)
            {
                throw;
            }
            return empDisplayName;
        }
        /// <summary>
        /// Registe event
        /// </summary>
        /// <param name="resultData"></param>
        /// <returns></returns>
        public ActionResult CTS280_RegisterAction(CTS280_Result resultData)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS280_ScreenParameter CTS280Param = GetScreenObject<CTS280_ScreenParameter>();

                //6.1 Check suspending
                //6.1.1 Get suspending status
                ObjectResultData resSuspend = checkSuspending();
                if (resSuspend.IsError)
                {
                    return Json(resSuspend);
                }

                //Check user's permission
                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_EDIT) == true
                    ))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                //6.3 Validate require field
                //Case Input Register actual check-up data, check all required fields
                if (CTS280Param.Mode == FunctionID.C_FUNC_ID_VIEW || resultData.ProcessType == RegisterMAProcessType.C_REGISTER_MA_PROCESS_TYPE_REGISTER_RESULT)
                {
                    #region Validate Old Code
                    //ValidatorUtil.BuildErrorMessage(res, this);

                    ////if open maintenanceFeeFlag, check maintenanceFee
                    //if (resultData.MaintenanceFeeFlag && CommonUtil.IsNullOrEmpty(resultData.MaintenanceFee))
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3101);

                    //if (res.IsError)
                    //    return Json(res);
                    #endregion

                    ValidatorUtil validator = new ValidatorUtil(this);

                    /* --- Special Validate (if open maintenanceFeeFlag is true, check maintenanceFee) --- */
                    if (CTS280Param.MaintenanceFeeFlag && CommonUtil.IsNullOrEmpty(resultData.MaintenanceFee))
                        validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                    "CTS280",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "MaintenanceFeeID",//solve case duplicate key when AddErrorMessage
                                                    "lblMaintenanceFee",
                                                    "MaintenanceFee",
                                                    "2");
                    if (resultData.InstrumentMalfunctionFlagData == true 
                        && CommonUtil.IsNullOrEmpty(resultData.Location))
                        validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                    "CTS280",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "LocationID",//solve case duplicate key when AddErrorMessage
                                                    "lblLocation",
                                                    "Location",
                                                    "7");
                    /* ------------------------ */

                    ValidatorUtil.BuildErrorMessage(res, validator);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    if (res.IsError)
                        return Json(res);

                    if (resultData.MaintEmpNo != null)
                    {
                        IEmployeeMasterHandler employeeMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                        if (employeeMasterHandler.GetActiveEmployee(resultData.MaintEmpNo).Count() == 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { resultData.MaintEmpNo }, new string[] { "MaintEmpNo" });
                            return Json(res);
                        }
                    }
                }
                //Case Input Expected maintenance date, check only Expected maintenance date
                else if (resultData.ProcessType == RegisterMAProcessType.C_REGISTER_MA_PROCESS_TYPE_INPUT_EXPECTED)
                {
                    if (CommonUtil.IsNullOrEmpty(resultData.ExpectedMaintenanceDate))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        "CTS280",
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblExpectedMaintenanceDate" },
                                        new string[] { "ExpectedMaintenanceDate" });
                        return Json(res);
                    }
                }

                //6.4 Validate Business
                ObjectResultData resValidateBiz = validateBusiness(resultData, CTS280Param);
                if (resValidateBiz.IsError)
                {
                    return Json(resValidateBiz);
                }

                //6.5	Validate business for warning
                #region Old Code
                //ObjectResultData resValidateBizWarning = validateBusinessForWarning(resultData);
                //res = validateBusinessForWarning(resultData, CTS280Param);
                //if (res.IsError)
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    //return Json(resValidateBiz);
                //}

                //MessageModel msgModel = validateBusinessForWarning(resultData, CTS280Param);
                //if (msgModel == null)
                //    res.ResultData = true;
                //else
                //    res.ResultData = msgModel;
                #endregion
                if ((CTS280Param.Mode == FunctionID.C_FUNC_ID_VIEW || resultData.ProcessType == RegisterMAProcessType.C_REGISTER_MA_PROCESS_TYPE_INPUT_EXPECTED)
                    && CTS280Param.MaintenanceFeeFlag == true)
                {
                    //Validate expected maintenance fee
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3181);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Confirm event
        /// </summary>
        /// <param name="resultData"></param>
        /// <returns></returns>
        public ActionResult CTS280_ConfirmAction(CTS280_Result resultData)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS280_ScreenParameter CTS280Param = GetScreenObject<CTS280_ScreenParameter>();

                //9.1 Check suspending
                ObjectResultData resSuspend = checkSuspending();
                if (resSuspend.IsError)
                {
                    return Json(resSuspend);
                }

                //Check user's permission
                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_EDIT) == true
                    ))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                //9.2 Validate Business
                ObjectResultData resValidateBiz = validateBusiness(resultData, CTS280Param);
                if (resValidateBiz.IsError)
                {
                    return Json(resValidateBiz);
                }

                //9.3 Perform save operation
                tbt_MaintenanceCheckup tbtMaint = CTS280Param.data.dtMaintenanceCheckup; //use data in session

                //Update New Value
                tbtMaint.ExpectedMaintenanceDate = resultData.ExpectedMaintenanceDate;
                if (CTS280Param.Mode == FunctionID.C_FUNC_ID_VIEW || resultData.ProcessType == RegisterMAProcessType.C_REGISTER_MA_PROCESS_TYPE_REGISTER_RESULT)
                {
                    tbtMaint.MaintenanceDate = resultData.MaintenanceDate;

                    tbtMaint.MaintenanceFeeCurrencyType = resultData.MaintenanceFeeCurrencyType;
                    if (tbtMaint.MaintenanceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        tbtMaint.MaintenanceFee = null;
                        tbtMaint.MaintenanceFeeUsd = resultData.MaintenanceFee;
                    }
                    else
                    {
                        tbtMaint.MaintenanceFee = resultData.MaintenanceFee;
                        tbtMaint.MaintenanceFeeUsd = null;
                    }

                    tbtMaint.ApproveNo1 = resultData.ApproveNo1;
                    tbtMaint.SubcontractCode = resultData.SubcontractCode;
                    tbtMaint.PICName = resultData.PICName;
                    tbtMaint.MaintEmpNo = resultData.MaintEmpNo;
                    tbtMaint.UsageTime = resultData.UsageTime;
                    tbtMaint.InstrumentMalfunctionFlag = resultData.InstrumentMalfunctionFlagData;
                    tbtMaint.Location = resultData.Location;
                    tbtMaint.NeedSalesmanFlag = resultData.NeedSalesmanFlagData;
                    tbtMaint.MalfunctionDetail = resultData.MalfunctionDetail;
                    tbtMaint.Remark = resultData.Remark;
                }

                IMaintenanceHandler mainHand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                mainHand.RegisterMaintenanceCheckupResult(tbtMaint);

                //Update result for CTS270
                CTS270_UpdateDataFromChildPage(
                    CTS280Param.CallerSessionKey, CTS280Param.ContractCode,
                    CTS280Param.ProductCode, CTS280Param.InstructionDate,
                    tbtMaint.MaintenanceDate, tbtMaint.ExpectedMaintenanceDate);

                //Update new data to session
                CTS280Param.data.dtMaintenanceCheckup = tbtMaint;
                UpdateScreenObject(CTS280Param);

                //when finish with out error
                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get screen URL
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS280_CallScreenURL()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS280_ScreenParameter CTS280Param = GetScreenObject<CTS280_ScreenParameter>();
                if (CTS280Param != null)
                {
                    CTS270_ScreenParameter CTS270Param = GetScreenObject<CTS270_ScreenParameter>(CTS280Param.CallerKey);
                    if (CTS270Param != null)
                    {
                        CTS270Param.IsLoaded = false;
                        
                        CTS270Param.CallerScreenID = CTS280Param.ScreenID;
                        CTS270Param.CallerModule = CTS280Param.Module;
                        CTS270Param.CallerKey = CTS280Param.Key;
                        CTS270Param.BackStep = true;
                    }

                    res.ResultData = CallScreenURL(CTS280Param, true);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Check system is suspending
        /// </summary>
        /// <returns></returns>
        private ObjectResultData checkSuspending()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                bool bSuspendingStatus = commonHandler.IsSystemSuspending();

                //6.1.2 Not allow to continue operation if system is suspended
                if (bSuspendingStatus)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }
        /// <summary>
        /// Validate business data
        /// </summary>
        /// <param name="resultData"></param>
        /// <param name="CTS280Param"></param>
        /// <returns></returns>
        private ObjectResultData validateBusiness(CTS280_Result resultData, CTS280_ScreenParameter CTS280Param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                if (CTS280Param.Mode == FunctionID.C_FUNC_ID_VIEW || resultData.ProcessType == RegisterMAProcessType.C_REGISTER_MA_PROCESS_TYPE_REGISTER_RESULT)
                {
                    //Validate instrument malfunction
                    if (resultData.InstrumentMalfunctionFlagData == true && CommonUtil.IsNullOrEmpty(resultData.Location))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        "CTS280",
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3101,
                                        new string[] { "lblLocation" },
                                        new string[] { "Location" });
                        return res;
                    }

                    //Validate maintenance date
                    if (resultData.MaintenanceDate.Value.Date > DateTime.Now.Date)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        "CTS280",
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3126,
                                        new string[] { "lblMaintenanceDate" },
                                        new string[] { "MaintenanceDate" });
                        return res;
                    }

                    //Validate approve no.
                    if (CTS280Param.MaintenanceFeeFlag == true)
                    {
                        if (resultData.MaintenanceFee != CTS280Param.data.MaintenanceFeeInit && CommonUtil.IsNullOrEmpty(resultData.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        "CTS280",
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3153,
                                        new string[] { "lblApproveNo1" },
                                        new string[] { "ApproveNo1" });
                            return res;
                        }
                    }


                
                    //Validate billing basic
                    string maintenanceFeeTypeCode = null;
                    if (CTS280Param.data != null)
                    {
                        if (CTS280Param.data.dtEntireContract != null)
                        {
                            if (CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails != null)
                            {
                                if (CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails.Count > 0)
                                {
                                    maintenanceFeeTypeCode = CTS280Param.data.dtEntireContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode;
                                }
                            }
                        }
                    }

#if !ROUND2
                    if (maintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
                    {
                        CommonUtil c = new CommonUtil();
                        String ContractCode = c.ConvertContractCode(CTS280Param.ContractCodeShow, CommonUtil.CONVERT_TYPE.TO_LONG);

                        //5.2.4.1	Call method for checking
                        IRentralContractHandler renHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        tbt_BillingBasic resultBase = renHand.GetBillingBasicForMAResultBasedFeePayment(ContractCode);

                        //5.2.4.2	If ResultBasedBillingBasic = NULL Then
                        if (CommonUtil.IsNullOrEmpty(resultBase))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3195);
                            return res;
                        }
                    }

#endif
                }
                else if (resultData.ProcessType == RegisterMAProcessType.C_REGISTER_MA_PROCESS_TYPE_INPUT_EXPECTED)
                {
                    //Validate expected maintenance date
                    if (resultData.ExpectedMaintenanceDate.Value.Date < DateTime.Now.Date)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        "CTS280",
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3180,
                                        new string[] { "lblExpectedMaintenanceDate" },
                                        new string[] { "ExpectedMaintenanceDate" });
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }
        /// <summary>
        /// Initial maintenance check-up data
        /// </summary>
        /// <param name="param"></param>
        /// <param name="res"></param>
        public void CTS280_InitMACheckupResultData(CTS280_ScreenParameter param, ObjectResultData res)
        {
            try
            {
                //1.3.12 Get contract basic data
                IRentralContractHandler renHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<tbt_RentalContractBasic> dtTbt_RentalContractBasic
                    = renHand.GetTbt_RentalContractBasic(param.ContractCode, null);
                if (dtTbt_RentalContractBasic.Count == 0)
                {

                    res.AddErrorMessage(
                        MessageUtil.MODULE_COMMON,
                        MessageUtil.MessageList.MSG0105,
                        new String[] { param.ContractCodeShow });
                    return;
                }

                //1.3.2 Check authority of contract office
                var contractOfficeData = from l in CommonUtil.dsTransData.dtOfficeData
                                         where l.OfficeCode == dtTbt_RentalContractBasic[0].ContractOfficeCode
                                         select l;

                var operationOfficeData = from l in CommonUtil.dsTransData.dtOfficeData
                                          where l.OfficeCode == dtTbt_RentalContractBasic[0].OperationOfficeCode
                                          select l;

                //if (contractOfficeData.Count() == 0 || operationOfficeData.Count() == 0)
                if (contractOfficeData.Count() == 0 && operationOfficeData.Count() == 0) //Modify by Jutarat A. on 18092013
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                }

                //Get maintenance checkup information
                IMaintenanceHandler mainHand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                List<doMaintenanceCheckupInformation> doMaintCheckupInfoList
                    = mainHand.GetMaintenanceCheckupInformationData(param.ContractCode, param.ProductCode, param.InstructionDate);

                if (doMaintCheckupInfoList.Count <= 0 || CommonUtil.IsNullOrEmpty(doMaintCheckupInfoList[0]))
                {
                    res.AddErrorMessage(
                        MessageUtil.MODULE_CONTRACT,
                        MessageUtil.MessageList.MSG3102,
                        new String[] { param.ContractCodeShow, param.ProductCode, CommonUtil.TextDate(param.InstructionDate) });
                    return;
                }

                param.hasEditPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP, FunctionID.C_FUNC_ID_EDIT);
                param.data = new dsCTS280Data();
                param.data.doMaintCheckupInformation = doMaintCheckupInfoList[0];

                //1.2 Get maintenace check-up data
                List<tbt_MaintenanceCheckup> dtTbt_MaintenanceCheckup
                    = mainHand.GetTbt_MaintenanceCheckup(param.ContractCode, param.ProductCode, param.InstructionDate);
                param.data.dtMaintenanceCheckup = dtTbt_MaintenanceCheckup[0];
                param.data.dtMaintenanceCheckup.MaintEmpName = getEmployeeDisplayName(param.data.dtMaintenanceCheckup.MaintEmpNo);

                //1.3 Get maintenance detail in maintenance contract
                dsRentalContractData dtEntireContract = renHand.GetEntireContract(param.ContractCode, null);
                param.data.dtEntireContract = dtEntireContract;

                //Modified By Narut T. 2017-02-10
                //Modified By Pachara S.
                if (dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    param.data.MaintenanceFeeInit = dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFeeUsd;
                else
                    param.data.MaintenanceFeeInit = dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFee;
                //param.data.MaintenanceFeeInit = dtEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFee;
                //param.data.MaintenanceFeeInit = param.data.dtMaintenanceCheckup.MaintenanceFee ?? param.data.dtMaintenanceCheckup.MaintenanceFeeUsd; 
                //End Modified By Narut T.
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Old Code
        //private ObjectResultData validateBusinessForWarning(CTS280_Result resultData, CTS280_ScreenParameter CTS280Param)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
        //    try
        //    {
        //        if ((CTS280Param.Mode == FunctionID.C_FUNC_ID_VIEW || resultData.ProcessType == RegisterMAProcessType.C_REGISTER_MA_PROCESS_TYPE_INPUT_EXPECTED)
        //            && CTS280Param.MaintenanceFeeFlag == true)
        //        {
        //            //Validate expected maintenance fee
        //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3181);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return res;
        //}


        //private MessageModel validateBusinessForWarning(CTS280_Result resultData, CTS280_ScreenParameter CTS280Param)
        //{
        //    MessageModel msg = null;
        //    if ((CTS280Param.Mode == FunctionID.C_FUNC_ID_VIEW || resultData.ProcessType == RegisterMAProcessType.C_REGISTER_MA_PROCESS_TYPE_REGISTER_RESULT)
        //        && CTS280Param.MaintenanceFeeFlag == true)
        //    {
        //        IMaintenanceHandler mainHand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
        //        bool IsAllResultRegistered = mainHand.IsAllResultRegisteredData(CTS280Param.ContractCode, CTS280Param.InstructionDate);
        //        if (IsAllResultRegistered)
        //        {
        //            //Validate expected maintenance fee
        //            msg = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3181);
        //        }
        //    }

        //    return msg;
        //}
        #endregion
    }
}
