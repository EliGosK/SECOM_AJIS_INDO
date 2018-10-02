//*********************************
// Create by: Jutarat A.
// Create date: 12/Sep/2011
// Update date: 12/Sep/2011
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
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Installation;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check system suspending, user’s permission and user’s authority of screen
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ActionResult CTS100_Authority(CTS100_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            string strContractCodeLong = string.Empty;
            dsRentalContractData dsRentalContract = null;
            tbt_RentalContractBasic tbt_RentalContractBasicData = null;
            CTS100_doRentalContractBasicAuthority doRentalContractBasicAuthority;

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                /*--- HasAuthority ---*/
                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP13, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //if (String.IsNullOrEmpty(sParam.ContractCode))
                //    sParam.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                if (String.IsNullOrEmpty(sParam.ContractCode) && sParam.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(sParam.CommonSearch.ContractCode) == false)
                        sParam.ContractCode = sParam.CommonSearch.ContractCode;
                }

                if (String.IsNullOrEmpty(sParam.ContractCode) == false)
                {
                    //Check data authority
                    strContractCodeLong = comUtil.ConvertContractCode(sParam.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    dsRentalContract = CheckDataAuthority_CTS100(res, strContractCodeLong, true);
                    if (res.IsError)
                        return Json(res);

                    tbt_RentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];

                    doRentalContractBasicAuthority = CommonUtil.CloneObject<tbt_RentalContractBasic, CTS100_doRentalContractBasicAuthority>(tbt_RentalContractBasicData);
                    ValidatorUtil.BuildErrorMessage(res, new object[] { doRentalContractBasicAuthority }, null, false);
                    if (res.IsError)
                        return Json(res);

                    //ValidateDataBusiness
                    ValidateBusiness_CTS100(res, strContractCodeLong, tbt_RentalContractBasicData);
                    if (res.IsError)
                        return Json(res);
                }
                /*-------------------------*/

                //sParam = new CTS100_ScreenParameter();
                sParam.CTS100_Session = new CTS100_RegisterStopTargetData();
                sParam.CTS100_Session.InitialData = new CTS100_InitialRegisterStopTargetData();
                sParam.CTS100_Session.InitialData.ContractCode = strContractCodeLong;
                sParam.CTS100_Session.RegisterStopData = dsRentalContract;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            //**** Change Session ****//
            //return InitialScreenEnvironment("CTS100", new object[] { strContractCodeLong, dsRentalContract });
            return InitialScreenEnvironment < CTS100_ScreenParameter>("CTS100", sParam, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS100")]
        public ActionResult CTS100()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            IUserControlHandler userCtrlHandler;
            doRentalContractBasicInformation doRentalContractBasic;
            bool hasStopFee;
            string strContractCodeLong = string.Empty;

            try
            {
                //**** Change Session ****//
                //ScreenParameter sParam = GetScreenObject();
                //if (sParam.Parameter.Length > 0)
                //{
                //    strContractCodeLong = sParam.Parameter[(int)CTS100_InitialRegisterStopTargetData.eParam.CONTRACT_CODE].ToString();
                //    dsRentalContract = (dsRentalContractData)sParam.Parameter[(int)CTS100_InitialRegisterStopTargetData.eParam.RENTAL_BASIC_DATA];
                //}
                CTS100_ScreenParameter sParam = GetScreenObject<CTS100_ScreenParameter>();
                strContractCodeLong = sParam.CTS100_Session.InitialData.ContractCode;

                userCtrlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                doRentalContractBasic = userCtrlHandler.GetRentalContactBasicInformationData(strContractCodeLong);

                hasStopFee = IsBillingHasStopFee(strContractCodeLong);
                ViewBag.HasStopFee = hasStopFee ? 1 : 0;

                //Map data to screen
                if (doRentalContractBasic != null)
                {
                    Bind_CTS100(doRentalContractBasic);
                }

                ViewBag.CurrentDate = DateTime.Now.Date;

                //Set data to CTS100_Session
                sParam.CTS100_Session.InitialData.doRentalContractBasicData = doRentalContractBasic;
                sParam.CTS100_Session.InitialData.HasStopFee = hasStopFee ? 1 : 0;
                UpdateScreenObject(sParam);

                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve Contrct data when click [Retrieve] button on ‘Specify contract code’ section
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public ActionResult CTS100_RetrieveData(string strContractCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            IUserControlHandler userCtrlHandler;
            doRentalContractBasicInformation doRentalContractBasic;
            bool hasStopFee;
            dsRentalContractData dsRentalContract = null;
            tbt_RentalContractBasic tbt_RentalContractBasicData;
            CTS100_doRentalContractBasicAuthority doRentalContractBasicAuthority;

            try
            {
                //Check mandatory
                if (String.IsNullOrEmpty(strContractCode))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" }); //TODO: (Jutarat) Must get lbl from resouce
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP13,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblContractCode" },
                                        new string[] { "ContractCodeShort" });

                    return Json(res);
                }

                //Get rental contract data
                string strContractCodeLong = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                dsRentalContract = CheckDataAuthority_CTS100(res, strContractCodeLong);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                userCtrlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                doRentalContractBasic = userCtrlHandler.GetRentalContactBasicInformationData(strContractCodeLong);

                tbt_RentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];

                hasStopFee = IsBillingHasStopFee(strContractCodeLong);

                //CheckDataAuthority
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                doRentalContractBasicAuthority = CommonUtil.CloneObject<tbt_RentalContractBasic, CTS100_doRentalContractBasicAuthority>(tbt_RentalContractBasicData);
                ValidatorUtil.BuildErrorMessage(res, new object[] { doRentalContractBasicAuthority }, null, false);
                if (res.IsError)
                    return Json(res);

                //Validate business
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidateBusiness_CTS100(res, strContractCodeLong, tbt_RentalContractBasicData);
                if (res.IsError)
                    return Json(res);

                //Set data to CTS100_Session
                CTS100_ScreenParameter sParam = GetScreenObject<CTS100_ScreenParameter>();
                if (sParam.CTS100_Session.InitialData == null)
                    sParam.CTS100_Session.InitialData = new CTS100_InitialRegisterStopTargetData();

                sParam.CTS100_Session.InitialData.ContractCode = strContractCodeLong;
                sParam.CTS100_Session.InitialData.doRentalContractBasicData = doRentalContractBasic;
                sParam.CTS100_Session.InitialData.HasStopFee = hasStopFee ? 1 : 0;
                sParam.CTS100_Session.InitialData.RentalContractBasicData = tbt_RentalContractBasicData;
                sParam.CTS100_Session.RegisterStopData = dsRentalContract;
                sParam.ContractCode = strContractCode;

                sParam.CommonSearch = new ScreenParameter.CommonSearchDo()
                {
                    ContractCode = strContractCode
                };

                UpdateScreenObject(sParam);

                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = strContractCode;

                CTS100_InitialRegisterStopTargetData result = new CTS100_InitialRegisterStopTargetData();
                result.doRentalContractBasicData = doRentalContractBasic;
                result.HasStopFee = hasStopFee ? 1 : 0;
                result.RentalContractBasicData = tbt_RentalContractBasicData;

                res.ResultData = result;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Clear all data when click [Clear] button on ‘Specify contract code’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS100_ClearData(bool clearAll)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                //Set data to CTS100_Session
                CTS100_ScreenParameter sParam = GetScreenObject<CTS100_ScreenParameter>();
                sParam.CTS100_Session.InitialData = null;
                sParam.CTS100_Session.RegisterStopData = null;
                sParam.ContractCode = string.Empty;

                if (clearAll == true)
                {
                    sParam.CommonSearch = new ScreenParameter.CommonSearchDo();
                }

                UpdateScreenObject(sParam);

                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = null;
                //CommonUtil.dsTransData.dtCommonSearch.ProjectCode = null;

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate business when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <param name="doStopReason"></param>
        /// <returns></returns>
        public ActionResult CTS100_RegisterStopData(CTS100_doStopReason doStopReason)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP13, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                CTS100_ScreenParameter sParam = GetScreenObject<CTS100_ScreenParameter>();

                //CheckMandatory
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (ModelState.IsValid == false)
                {
                    CTS100_doStopReason doStopReasonTemp = CommonUtil.CloneObject<CTS100_doStopReason, CTS100_doStopReason>(doStopReason);
                    if (sParam.CTS100_Session.InitialData.HasStopFee == 0)
                    {
                        doStopReasonTemp.ContractFeeOnStopCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                        doStopReasonTemp.ContractFeeOnStop = 0;
                    }

                    ValidatorUtil.BuildErrorMessage(res, new object[] { doStopReasonTemp }, null, false);
                }
                if (res.IsError)
                    return Json(res);

                //ValidateScreenBusiness
                ValidateScreenBusiness_CTS100(res, doStopReason);
                if (res.IsError)
                    return Json(res);

                //ValidateDataBusiness
                ValidateBusiness_CTS100(res, sParam.CTS100_Session.InitialData.ContractCode);
                if (res.IsError)
                    return Json(res);

                //ValidateBusinessForWarning
                //res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                if (doStopReason.ExpectedResumeDate >= ((DateTime)doStopReason.ChangeImplementDate).AddMonths(6))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3159);
                    //return Json(res);
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
        /// Update data to database when click [Confirm] button in ‘Action button’ section
        /// </summary>
        /// <param name="doStopReason"></param>
        /// <returns></returns>
        public ActionResult CTS100_ConfirmRegisterCancelData(CTS100_doStopReason doStopReason)
        {
            ObjectResultData res = new ObjectResultData();
            CTS100_RegisterStopTargetData registerStopData;
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;
            IBillingInterfaceHandler billingHandler;
            IMaintenanceHandler maintenHandler;

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP13, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                CTS100_ScreenParameter sParam = GetScreenObject<CTS100_ScreenParameter>();

                //ValidateScreenBusiness
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidateScreenBusiness_CTS100(res, doStopReason);
                if (res.IsError)
                    return Json(res);

                //ValidateDataBusiness
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidateBusiness_CTS100(res, sParam.CTS100_Session.InitialData.ContractCode);
                if (res.IsError)
                    return Json(res);

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                using (TransactionScope scope = new TransactionScope())
                {
                    /*--- RegisterStopService ---*/
                    registerStopData = sParam.CTS100_Session;
                    if (registerStopData.RegisterStopData != null)
                    {
                        dsRentalContract = new dsRentalContractData();

                        //Generate security occurrence
                        rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        string strOCC = rentralHandler.GenerateContractOCC(registerStopData.InitialData.ContractCode, true);

                        /*--- MapRentalContractData ---*/
                        //dtTbt_RentalContractBasic
                        dsRentalContract.dtTbt_RentalContractBasic = new List<tbt_RentalContractBasic>();
                        tbt_RentalContractBasic rentalContractBasic = CommonUtil.CloneObject<tbt_RentalContractBasic, tbt_RentalContractBasic>(registerStopData.RegisterStopData.dtTbt_RentalContractBasic[0]);
                        rentalContractBasic.LastOCC = strOCC;
                        rentalContractBasic.LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_STOP;
                        rentalContractBasic.ContractStatus = ContractStatus.C_CONTRACT_STATUS_STOPPING;
                        rentalContractBasic.LastChangeImplementDate = doStopReason.ChangeImplementDate; //DateTime.Now;
                        rentalContractBasic.StopProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        dsRentalContract.dtTbt_RentalContractBasic.Add(rentalContractBasic);

                        //dtTbt_RentalSecurityBasic
                        dsRentalContract.dtTbt_RentalSecurityBasic = new List<tbt_RentalSecurityBasic>();
                        tbt_RentalSecurityBasic rentalSecurityBasicData = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(registerStopData.RegisterStopData.dtTbt_RentalSecurityBasic[0]);
                        rentalSecurityBasicData.ContractCode = registerStopData.InitialData.ContractCode;
                        rentalSecurityBasicData.OCC = strOCC;
                        rentalSecurityBasicData.ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_STOP;

                        //Add by Jutarat A. 03102012
                        rentalSecurityBasicData.QuotationTargetCode = null;
                        rentalSecurityBasicData.QuotationAlphabet = null;
                        rentalSecurityBasicData.SalesmanEmpNo1 = null;
                        rentalSecurityBasicData.SalesmanEmpNo2 = null;
                        rentalSecurityBasicData.SalesSupporterEmpNo = null;
                        //End Add

                        rentalSecurityBasicData.ChangeImplementDate = doStopReason.ChangeImplementDate;

                        #region Contract Fee on Stop

                        rentalSecurityBasicData.ContractFeeOnStopCurrencyType = doStopReason.ContractFeeOnStopCurrencyType;
                        if (rentalSecurityBasicData.ContractFeeOnStopCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            rentalSecurityBasicData.ContractFeeOnStopUsd = doStopReason.ContractFeeOnStop;
                            rentalSecurityBasicData.ContractFeeOnStop = null;
                        }
                        else
                        {
                            rentalSecurityBasicData.ContractFeeOnStopUsd = null;
                            rentalSecurityBasicData.ContractFeeOnStop = doStopReason.ContractFeeOnStop;
                        }

                        #endregion
                        
                        rentalSecurityBasicData.NormalAdditionalDepositFee = null; //0;
                        rentalSecurityBasicData.OrderAdditionalDepositFee = null; //0;
                        rentalSecurityBasicData.DepositFeeBillingTiming = null;
                        rentalSecurityBasicData.PlanCode = null; //Add by Jutarat A. 03102012
                        rentalSecurityBasicData.ApproveNo1 = doStopReason.ApproveNo1;
                        rentalSecurityBasicData.ApproveNo2 = doStopReason.ApproveNo2;

                        //Add by Jutarat A. 03102012
                        rentalSecurityBasicData.ApproveNo3 = null; 
                        rentalSecurityBasicData.ApproveNo4 = null;
                        rentalSecurityBasicData.ApproveNo5 = null;
                        //End Add

                        rentalSecurityBasicData.AlmightyProgramEmpNo = null;
                        rentalSecurityBasicData.CounterNo = 0;
                        rentalSecurityBasicData.ChangeReasonType = null;
                        rentalSecurityBasicData.ChangeNameReasonType = null;
                        rentalSecurityBasicData.StopCancelReasonType = doStopReason.StopCancelReasonType;
                        rentalSecurityBasicData.ContractDocPrintFlag = FlagType.C_FLAG_OFF;

                        //Add by Jutarat A. 24072012
                        rentalSecurityBasicData.InstallationCompleteFlag = null;
                        rentalSecurityBasicData.InstallationSlipNo = null;
                        rentalSecurityBasicData.InstallationCompleteDate = null;
                        rentalSecurityBasicData.InstallationCompleteEmpNo = null;
                        rentalSecurityBasicData.InstallationTypeCode = null;
                        //End Add

                        //Add by Jutarat A. 03102012
                        rentalSecurityBasicData.NegotiationStaffEmpNo1 = null;
                        rentalSecurityBasicData.NegotiationStaffEmpNo2 = null;
                        //End Add

                        rentalSecurityBasicData.ExpectedResumeDate = doStopReason.ExpectedResumeDate;
                        rentalSecurityBasicData.NormalInstallFee = null;
                        rentalSecurityBasicData.OrderInstallFee = null;
                        rentalSecurityBasicData.OrderInstallFee_ApproveContract = null;
                        rentalSecurityBasicData.OrderInstallFee_CompleteInstall = null;
                        rentalSecurityBasicData.OrderInstallFee_StartService = null;

                        //Add by Jutarat A. 03102012
                        rentalSecurityBasicData.DispatchTypeCode = null;
                        rentalSecurityBasicData.PlannerEmpNo = null;
                        rentalSecurityBasicData.PlanCheckerEmpNo = null;
                        rentalSecurityBasicData.PlanCheckDate = null;
                        rentalSecurityBasicData.PlanApproverEmpNo = null;
                        rentalSecurityBasicData.PlanApproveDate = null;
                        //End Add

                        rentalSecurityBasicData.DocumentCode = null;
                        rentalSecurityBasicData.DocAuditResult = null;
                        rentalSecurityBasicData.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        rentalSecurityBasicData.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        rentalSecurityBasicData.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        rentalSecurityBasicData.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        dsRentalContract.dtTbt_RentalSecurityBasic.Add(rentalSecurityBasicData);

                        //dtTbt_RentalBEDetails
                        if (registerStopData.RegisterStopData.dtTbt_RentalBEDetails != null && registerStopData.RegisterStopData.dtTbt_RentalBEDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>();
                            tbt_RentalBEDetails rentalBEDetailsData;
                            foreach (tbt_RentalBEDetails data in registerStopData.RegisterStopData.dtTbt_RentalBEDetails)
                            {
                                rentalBEDetailsData = CommonUtil.CloneObject<tbt_RentalBEDetails, tbt_RentalBEDetails>(data);
                                rentalBEDetailsData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalBEDetails.Add(rentalBEDetailsData);
                            }
                        }

                        //dtTbt_RentalInstrumentDetails
                        if (registerStopData.RegisterStopData.dtTbt_RentalInstrumentDetails != null && registerStopData.RegisterStopData.dtTbt_RentalInstrumentDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                            tbt_RentalInstrumentDetails rentalInstrumentDetailsData;
                            foreach (tbt_RentalInstrumentDetails data in registerStopData.RegisterStopData.dtTbt_RentalInstrumentDetails)
                            {
                                rentalInstrumentDetailsData = CommonUtil.CloneObject<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(data);
                                rentalInstrumentDetailsData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalInstrumentDetails.Add(rentalInstrumentDetailsData);
                            }
                        }

                        //dtTbt_RentalMaintenanceDetails
                        if (registerStopData.RegisterStopData.dtTbt_RentalMaintenanceDetails != null && registerStopData.RegisterStopData.dtTbt_RentalMaintenanceDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                            tbt_RentalMaintenanceDetails rentalMaintenanceDetailsData;
                            foreach (tbt_RentalMaintenanceDetails data in registerStopData.RegisterStopData.dtTbt_RentalMaintenanceDetails)
                            {
                                rentalMaintenanceDetailsData = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(data);
                                rentalMaintenanceDetailsData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalMaintenanceDetails.Add(rentalMaintenanceDetailsData);
                            }
                        }

                        //dtTbt_RentalOperationType
                        if (registerStopData.RegisterStopData.dtTbt_RentalOperationType != null && registerStopData.RegisterStopData.dtTbt_RentalOperationType.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                            tbt_RentalOperationType rentalOperationTypeData;
                            foreach (tbt_RentalOperationType data in registerStopData.RegisterStopData.dtTbt_RentalOperationType)
                            {
                                rentalOperationTypeData = CommonUtil.CloneObject<tbt_RentalOperationType, tbt_RentalOperationType>(data);
                                rentalOperationTypeData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalOperationType.Add(rentalOperationTypeData);
                            }
                        }

                        //dtTbt_RentalSentryGuard
                        if (registerStopData.RegisterStopData.dtTbt_RentalSentryGuard != null && registerStopData.RegisterStopData.dtTbt_RentalSentryGuard.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>();
                            tbt_RentalSentryGuard rentalSentryGuardData;
                            foreach (tbt_RentalSentryGuard data in registerStopData.RegisterStopData.dtTbt_RentalSentryGuard)
                            {
                                rentalSentryGuardData = CommonUtil.CloneObject<tbt_RentalSentryGuard, tbt_RentalSentryGuard>(data);
                                rentalSentryGuardData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalSentryGuard.Add(rentalSentryGuardData);
                            }
                        }

                        //dtTbt_RentalSentryGuardDetails
                        if (registerStopData.RegisterStopData.dtTbt_RentalSentryGuardDetails != null && registerStopData.RegisterStopData.dtTbt_RentalSentryGuardDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                            tbt_RentalSentryGuardDetails rentalSentryGuardDetailsData;
                            foreach (tbt_RentalSentryGuardDetails data in registerStopData.RegisterStopData.dtTbt_RentalSentryGuardDetails)
                            {
                                rentalSentryGuardDetailsData = CommonUtil.CloneObject<tbt_RentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(data);
                                rentalSentryGuardDetailsData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalSentryGuardDetails.Add(rentalSentryGuardDetailsData);
                            }
                        }

                        //dtTbt_RelationType
                        if (registerStopData.RegisterStopData.dtTbt_RelationType != null && registerStopData.RegisterStopData.dtTbt_RelationType.Count > 0)
                        {
                            dsRentalContract.dtTbt_RelationType = new List<tbt_RelationType>();
                            tbt_RelationType relationTypeData;
                            foreach (tbt_RelationType data in registerStopData.RegisterStopData.dtTbt_RelationType)
                            {
                                relationTypeData = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(data);
                                relationTypeData.OCC = strOCC;
                                dsRentalContract.dtTbt_RelationType.Add(relationTypeData);
                            }
                        }
                        /*---------------------------*/

                        //Save stop service contract data
                        //dsRentalContractData dsRentalContractResult = rentralHandler.InsertEntireContract(dsRentalContract);
                        rentralHandler.InsertEntireContractForCTS010(dsRentalContract);

#if !ROUND2
                        //Send billing data
                        billingHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                        billingHandler.SendBilling_StopService(
                            registerStopData.InitialData.ContractCode, 
                            doStopReason.ChangeImplementDate.Value, 
                            doStopReason.ContractFeeOnStop);
#endif

                        //Delete maintenance check-up schedule
                        maintenHandler = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                        List<tbt_MaintenanceCheckupDetails> tbt_MaintenanceCheckupDetailsList = maintenHandler.DeleteMACheckupDetail(registerStopData.InitialData.ContractCode, doStopReason.ChangeImplementDate.Value);
                        List<tbt_MaintenanceCheckup> tbt_MaintenanceCheckupList = maintenHandler.DeleteMACheckup(registerStopData.InitialData.ContractCode, doStopReason.ChangeImplementDate.Value);

                    }
                    /*--------------------------*/

                    scope.Complete();
                    res.ResultData = new object[] {MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046), sParam.ContractCode};
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

        //Change to Screen param
        //public CTS100_RegisterStopTargetData CTS100_Session
        //{
        //    get
        //    {
        //        return CommonUtil.GetSession<CTS100_RegisterStopTargetData>(ScreenID.C_SCREEN_ID_REGISTER_STOP_SERVICE);
        //    }
        //    set
        //    {
        //        CommonUtil.SetSession(ScreenID.C_SCREEN_ID_REGISTER_STOP_SERVICE, value);
        //    }
        //}

        /// <summary>
        /// Bind data to control on screen
        /// </summary>
        /// <param name="doRentalContractBasic"></param>
        private void Bind_CTS100(doRentalContractBasicInformation doRentalContractBasic)
        {
            ViewBag.SaleContractBasicInformation = doRentalContractBasic;
            ViewBag.ContractCodeLong = doRentalContractBasic.ContractCode;
            ViewBag.ContractCodeShort = doRentalContractBasic.ContractCodeShort;
            ViewBag.UserCode = doRentalContractBasic.UserCode;
            ViewBag.ContractTargetCustCodeShort = doRentalContractBasic.ContractTargetCustCodeShort;
            ViewBag.RealCustomerCustCodeShort = doRentalContractBasic.RealCustomerCustCodeShort;
            ViewBag.SiteCodeShort = doRentalContractBasic.SiteCodeShort;
            ViewBag.ContractTargetCustomerImportant = doRentalContractBasic.ContractTargetCustomerImportant;
            ViewBag.CustFullNameEN = doRentalContractBasic.ContractTargetNameEN;
            ViewBag.AddressFullEN = doRentalContractBasic.ContractTargetAddressEN;
            ViewBag.SiteNameEN = doRentalContractBasic.SiteNameEN;
            ViewBag.SiteAddressEN = doRentalContractBasic.SiteAddressEN;
            ViewBag.CustFullNameLC = doRentalContractBasic.ContractTargetNameLC;
            ViewBag.AddressFullLC = doRentalContractBasic.ContractTargetAddressLC;
            ViewBag.SiteNameLC = doRentalContractBasic.SiteNameLC;
            ViewBag.SiteAddressLC = doRentalContractBasic.SiteAddressLC;
            ViewBag.OfficeName = doRentalContractBasic.OperationOfficeCodeName;
        }

        /// <summary>
        /// Check authority data of screen
        /// </summary>
        /// <param name="res"></param>
        /// <param name="strContractCodeLong"></param>
        /// <param name="isInitScreen"></param>
        /// <returns></returns>
        private dsRentalContractData CheckDataAuthority_CTS100(ObjectResultData res, string strContractCodeLong, bool isInitScreen = false)
        {
            CommonUtil comUtil = new CommonUtil();
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;

            rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            dsRentalContract = rentralHandler.GetEntireContract(strContractCodeLong, null);

            if (dsRentalContract == null || dsRentalContract.dtTbt_RentalContractBasic == null || dsRentalContract.dtTbt_RentalContractBasic.Count < 1)
            {
                if (isInitScreen)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124);
                }
                else
                {
                    string strContractCode = comUtil.ConvertContractCode(strContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, new string[] { strContractCode }, new string[] { "txtSpecifyContractCode" });
                }
            }

            return dsRentalContract;
        }

        /// <summary>
        /// Validate business of screen
        /// </summary>
        /// <param name="res"></param>
        /// <param name="strContractCodeLong"></param>
        /// <param name="tbt_RentalContractBasicData"></param>
        private void ValidateBusiness_CTS100(ObjectResultData res, string strContractCodeLong, tbt_RentalContractBasic tbt_RentalContractBasicData = null)
        {
            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;
            IInstallationHandler installHandler;
            List<tbt_InstallationBasic> dtTbt_InstallationBasicData;

            try
            {
                /*--- Validate rental contract data ---*/
                if (tbt_RentalContractBasicData == null)
                {
                    dsRentalContract = CheckDataAuthority_CTS100(res, strContractCodeLong);
                    if (res.IsError)
                        return;

                    tbt_RentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];
                }

                string strContractCodeShort = comUtil.ConvertContractCode(strContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                if (tbt_RentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3119, new string[] { strContractCodeShort }, new string[] { "txtSpecifyContractCode" });
                    return;
                }
                else if (tbt_RentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3156, new string[] { strContractCodeShort }, new string[] { "txtSpecifyContractCode" });
                    return;
                }
                else if (tbt_RentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL
                        || tbt_RentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_END
                        || tbt_RentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3105, new string[] { strContractCodeShort }, new string[] { "txtSpecifyContractCode" });
                    return;
                }

                rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                string strOCCout = rentralHandler.GetLastUnimplementedOCC(strContractCodeLong);
                if (String.IsNullOrEmpty(strOCCout) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3106, null, new string[] { "txtSpecifyContractCode" });
                    return;
                }
                /*----------------------------------------*/

                /*--- Validate installation basic data ---*/
                installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                dtTbt_InstallationBasicData = installHandler.GetTbt_InstallationBasicData(strContractCodeLong);
                if (dtTbt_InstallationBasicData != null && dtTbt_InstallationBasicData.Count > 0)
                {
                    if (dtTbt_InstallationBasicData[0].InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                        && dtTbt_InstallationBasicData[0].InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                        && dtTbt_InstallationBasicData[0].InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL)
                    {
                        string strContractCode = comUtil.ConvertContractCode(strContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3103, new string[] { strContractCode }, new string[] { "txtSpecifyContractCode" });
                        return;
                    }
                }
                /*----------------------------------------*/
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Validate business of screen
        /// </summary>
        /// <param name="res"></param>
        /// <param name="doStopReason"></param>
        private void ValidateScreenBusiness_CTS100(ObjectResultData res, CTS100_doStopReason doStopReason)
        {
            try
            {
                CTS100_ScreenParameter sParam = GetScreenObject<CTS100_ScreenParameter>();

                if (doStopReason.ChangeImplementDate > DateTime.Now.Date)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3157, null, new string[] { "dpStopDate" });
                    return;
                }

                if (doStopReason.ChangeImplementDate <= (DateTime.Now.Date.AddYears(-1)))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3158, null, new string[] { "dpStopDate" });
                    return;
                }

                if (doStopReason.ExpectedResumeDate < doStopReason.ChangeImplementDate)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3182, null, new string[] { "dpOperationDate" });
                    return;
                }

                if (doStopReason.ChangeImplementDate < sParam.CTS100_Session.RegisterStopData.dtTbt_RentalContractBasic[0].LastChangeImplementDate)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3185, null, new string[] { "dpStopDate" });
                    return;
                }

                //Validate stop fee for keeping contract
                if (doStopReason.ContractFeeOnStop > CommonValue.C_MAX_MONTHLY_FEE_INPUT)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3287, new string[] { CommonValue.C_MAX_MONTHLY_FEE_INPUT.ToString("N2") }, new string[] { "txtStopFee" });
                    return;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Check Billing has StopFee
        /// </summary>
        /// <param name="strContractCodeLong"></param>
        /// <returns></returns>
        public bool IsBillingHasStopFee(string strContractCodeLong)
        {
            IBillingHandler billingHandler;
            bool hasStopFee = false;

            try
            {
                billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                List<tbt_BillingBasic> billingBasicList = billingHandler.GetTbt_BillingBasic(strContractCodeLong, null);
                if (billingBasicList != null)
                {
                    billingBasicList = (from t in billingBasicList
                                        where (t.MonthlyBillingAmount > 0 || t.MonthlyBillingAmountUsd > 0) && t.StopBillingFlag == false
                                        select t).ToList<tbt_BillingBasic>();
                }

                if (billingBasicList != null && billingBasicList.Count == 1)
                {
                    hasStopFee = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return hasStopFee;
        }

        #endregion

    }
}
