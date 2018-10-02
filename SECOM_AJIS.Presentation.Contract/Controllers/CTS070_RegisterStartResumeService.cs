//*********************************
// Create by: Jutarat A.
// Create date: 10/Oct/2011
// Update date: 10/Oct/2011
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
using SECOM_AJIS.DataEntity.Inventory;

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
        public ActionResult CTS070_Authority(CTS070_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            string strContractCodeLong = string.Empty;
            dsRentalContractData dsRentalContract = null;
            tbt_RentalContractBasic tbt_RentalContractBasicData = null;
            //CTS070_doRentalContractBasicAuthority doRentalContractBasicAuthority;

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                /*--- HasAuthority ---*/
                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP05, FunctionID.C_FUNC_ID_OPERATE) == false)
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

                //Check user’s authority to view data
                if (String.IsNullOrEmpty(sParam.ContractCode) == false)
                {
                    //Check data authority
                    strContractCodeLong = comUtil.ConvertContractCode(sParam.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    dsRentalContract = CheckDataAuthority_CTS070(res, strContractCodeLong, true);
                    if (res.IsError)
                        return Json(res);

                    if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
                    {
                        tbt_RentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];
                        
                        //Move to validate at ValidateDataBusiness()
                        //doRentalContractBasicAuthority = CommonUtil.CloneObject<tbt_RentalContractBasic, CTS070_doRentalContractBasicAuthority>(tbt_RentalContractBasicData);
                        //ValidatorUtil.BuildErrorMessage(res, new object[] { doRentalContractBasicAuthority }, null, false);
                        //if (res.IsError)
                        //    return Json(res);

                        //ValidateDataBusiness
                        ValidateBusiness_CTS070(res, strContractCodeLong, tbt_RentalContractBasicData);
                        if (res.IsError)
                            return Json(res);
                    }
                }
                /*-------------------------*/

                //sParam = new CTS070_ScreenParameter();
                sParam.CTS070_Session = new CTS070_RegisterStartResumeTargetData();
                sParam.CTS070_Session.InitialData = new CTS070_InitialRegisterStartResumeTargetData();
                sParam.CTS070_Session.InitialData.ContractCode = strContractCodeLong;
                sParam.CTS070_Session.InitialData.RentalContractBasicData = tbt_RentalContractBasicData;
                sParam.CTS070_Session.RegisterStartResumeData = dsRentalContract;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<CTS070_ScreenParameter>("CTS070", sParam, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS070")]
        public ActionResult CTS070()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            IUserControlHandler userCtrlHandler;
            doRentalContractBasicInformation doRentalContractBasic;
            string strContractCodeLong = string.Empty;

            try
            {
                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();
                strContractCodeLong = sParam.CTS070_Session.InitialData.ContractCode;

                userCtrlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                doRentalContractBasic = userCtrlHandler.GetRentalContactBasicInformationData(strContractCodeLong);

                //Map data to screen
                if (doRentalContractBasic != null)
                {
                    Bind_CTS100(doRentalContractBasic);

                    if (sParam.CTS070_Session.InitialData.RentalContractBasicData != null)
                        ViewBag.ProductTypeCode = sParam.CTS070_Session.InitialData.RentalContractBasicData.ProductTypeCode;
                }

                DateTime dtCurrentDate = DateTime.Now.Date;
                DateTime dtLastDate = new DateTime(dtCurrentDate.Year, dtCurrentDate.Month, 1).AddMonths(1).AddDays(-1);

                ViewBag.CurrentDate = dtCurrentDate;
                ViewBag.LastDate = dtLastDate;

                //Set data to CTS070_Session
                sParam.CTS070_Session.InitialData.doRentalContractBasicData = doRentalContractBasic;
                UpdateScreenObject(sParam);

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
        public ActionResult CTS070_RetrieveData(string strContractCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            IUserControlHandler userCtrlHandler;
            doRentalContractBasicInformation doRentalContractBasic;
            dsRentalContractData dsRentalContract = null;
            tbt_RentalContractBasic tbt_RentalContractBasicData;

            try
            {
                //Check mandatory
                if (String.IsNullOrEmpty(strContractCode))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" }); //TODO: (Jutarat) Must get lbl from resouce
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP05,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblContractCode" },
                                        new string[] { "txtSpecifyContractCode" });

                    return Json(res);
                }

                //Get rental contract data
                string strContractCodeLong = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                dsRentalContract = CheckDataAuthority_CTS070(res, strContractCodeLong);
                if (res.IsError)
                    return Json(res);

                userCtrlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                doRentalContractBasic = userCtrlHandler.GetRentalContactBasicInformationData(strContractCodeLong);                
                tbt_RentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];

                //Validate business
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidateBusiness_CTS070(res, strContractCodeLong, tbt_RentalContractBasicData);
                if (res.IsError)
                    return Json(res);

                //Set data to CTS070_Session
                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();
                if (sParam.CTS070_Session.InitialData == null)
                    sParam.CTS070_Session.InitialData = new CTS070_InitialRegisterStartResumeTargetData();

                sParam.CTS070_Session.InitialData.ContractCode = strContractCodeLong;
                sParam.CTS070_Session.InitialData.doRentalContractBasicData = doRentalContractBasic;
                sParam.CTS070_Session.InitialData.RentalContractBasicData = tbt_RentalContractBasicData;
                sParam.CTS070_Session.RegisterStartResumeData = dsRentalContract;
                sParam.ContractCode = strContractCode;
                UpdateScreenObject(sParam);

                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = strContractCode;
                sParam.CommonSearch = new ScreenParameter.CommonSearchDo()
                {
                    ContractCode = strContractCode
                };

                CTS070_InitialRegisterStartResumeTargetData result = new CTS070_InitialRegisterStartResumeTargetData();
                result.doRentalContractBasicData = doRentalContractBasic;
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
        public ActionResult CTS070_ClearData()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                //Set data to CTS070_Session
                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();
                sParam.CTS070_Session.InitialData = null;
                sParam.ContractCode = string.Empty;
                sParam.CommonSearch = new ScreenParameter.CommonSearchDo();

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
        /// <param name="doStartResumeService"></param>
        /// <returns></returns>
        public ActionResult CTS070_RegisterStartResumeData(CTS070_doStartResumeService doStartResumeService)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP05, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();

                //CheckMandatory
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                object doStartResumeServiceTemp;
                if (doStartResumeService.StartType == StartType.C_START_TYPE_NEW_START)
                {
                    string strProductTypeCode = sParam.CTS070_Session.InitialData.RentalContractBasicData.ProductTypeCode;
                    if (strProductTypeCode == ProductType.C_PROD_TYPE_AL 
                        || strProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                        || strProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {
                        doStartResumeServiceTemp = CommonUtil.CloneObject<CTS070_doStartResumeService, CTS070_ValidateStartTypeNewStart>(doStartResumeService);
                    }
                    else
                    {
                        doStartResumeServiceTemp = CommonUtil.CloneObject<CTS070_doStartResumeService, CTS070_ValidateStartResumeService>(doStartResumeService);
                    }
                }
                else if (doStartResumeService.StartType == StartType.C_START_TYPE_ALTER_START)
                {
                    doStartResumeServiceTemp = CommonUtil.CloneObject<CTS070_doStartResumeService, CTS070_ValidateStartTypeAlterStart>(doStartResumeService);
                }
                //Add by Jutarat A. on 15082012
                else if (doStartResumeService.StartType == StartType.C_START_TYPE_RESUME)
                {
                    doStartResumeServiceTemp = CommonUtil.CloneObject<CTS070_doStartResumeService, CTS070_ValidateStartTypeResume>(doStartResumeService);
                }
                //End Add
                else
                {
                    doStartResumeServiceTemp = CommonUtil.CloneObject<CTS070_doStartResumeService, CTS070_ValidateStartResumeService>(doStartResumeService);
                }

                ValidatorUtil.BuildErrorMessage(res, new object[] { doStartResumeServiceTemp }, null, false);
                if (res.IsError)
                    return Json(res);

                //Validate common business
                ValidateBusinessOnRegister_CTS070(res, doStartResumeService);
                if (res.IsError)
                    return Json(res);


                //ValidateScreenBusiness
                if (doStartResumeService.StartType == StartType.C_START_TYPE_NEW_START)
                {
                    ValidateBusinessNewStart_CTS070(res, doStartResumeService);
                }
                else if (doStartResumeService.StartType == StartType.C_START_TYPE_ALTER_START)
                {
                    ValidateBusinessAlternativeStart_CTS070(res, doStartResumeService);
                }
                else if (doStartResumeService.StartType == StartType.C_START_TYPE_RESUME)
                {
                    ValidateBusinessResume_CTS070(res, doStartResumeService);
                }

                if (res.IsError)
                    return Json(res);

                //ValidateBusinessForWarning
                //res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                if (doStartResumeService.StartResumeOperationDate < DateTime.Now.AddMonths(-3))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3177);
                    //return Json(res);
                }

                //ValidateBusinessForWarningNewStart
                if (doStartResumeService.StartType == StartType.C_START_TYPE_NEW_START)
                {
                    if (sParam.CTS070_Session.RegisterStartResumeData != null && sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalContractBasic != null
                        && (sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalContractBasic[0].StartMemoAuditResult == null
                            || sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalContractBasic[0].StartMemoAuditResult == DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED
                            || sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalContractBasic[0].StartMemoAuditResult == DocAuditResult.C_DOC_AUDIT_RESULT_RETURNED))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3307);
                    }
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
        /// <param name="doStartResumeService"></param>
        /// <returns></returns>
        public ActionResult CTS070_ConfirmRegisterStartResumeData(CTS070_doStartResumeService doStartResumeService)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP05, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                //Validate common business
                ValidateBusinessOnRegister_CTS070(res, doStartResumeService);
                if (res.IsError)
                    return Json(res);


                //ValidateScreenBusiness
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (doStartResumeService.StartType == StartType.C_START_TYPE_NEW_START)
                {
                    ValidateBusinessNewStart_CTS070(res, doStartResumeService);
                }
                else if (doStartResumeService.StartType == StartType.C_START_TYPE_ALTER_START)
                {
                    ValidateBusinessAlternativeStart_CTS070(res, doStartResumeService);
                }
                else if (doStartResumeService.StartType == StartType.C_START_TYPE_RESUME)
                {
                    ValidateBusinessResume_CTS070(res, doStartResumeService);
                }

                if (res.IsError)
                    return Json(res);

                //Perform save operation
                if (doStartResumeService.StartType == StartType.C_START_TYPE_NEW_START)
                {
                    SaveNewStart_CTS070(res, doStartResumeService);
                }
                else if (doStartResumeService.StartType == StartType.C_START_TYPE_ALTER_START)
                {
                    SaveAlternativeStart_CTS070(res, doStartResumeService);
                }
                else if (doStartResumeService.StartType == StartType.C_START_TYPE_RESUME)
                {
                    SaveResume_CTS070(res, doStartResumeService);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get StartType data to StartType ComboBox
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS070_GetStartType()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                string strProductTypeCode = string.Empty;
                string strContractStatus = string.Empty;
                string[] strStartTypeList = new string[] { string.Empty, string.Empty };

                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();

                if (sParam.CTS070_Session.RegisterStartResumeData != null
                    && sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalContractBasic != null
                    && sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalContractBasic.Count > 0)
                {
                    strProductTypeCode = sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalContractBasic[0].ProductTypeCode;
                    strContractStatus = sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalContractBasic[0].ContractStatus;

                    if (strProductTypeCode == ProductType.C_PROD_TYPE_AL 
                        || strProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                        || strProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {
                        if (strContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                        {
                            strStartTypeList[0] = StartType.C_START_TYPE_RESUME;
                        }
                        else if (strContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            strStartTypeList[0] = StartType.C_START_TYPE_ALTER_START;
                            strStartTypeList[1] = StartType.C_START_TYPE_NEW_START;
                        }
                        else if (strContractStatus != ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            strStartTypeList[0] = StartType.C_START_TYPE_NEW_START;
                        }
                    }
                    else if (strProductTypeCode == ProductType.C_PROD_TYPE_MA)
                    {
                        if (strContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            strStartTypeList[0] = StartType.C_START_TYPE_NEW_START;
                        }
                    }
                    else if (strProductTypeCode == ProductType.C_PROD_TYPE_SG || strProductTypeCode == ProductType.C_PROD_TYPE_BE)
                    {
                        if (strContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                        {
                            strStartTypeList[0] = StartType.C_START_TYPE_RESUME;
                        }
                        else if (strContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            strStartTypeList[0] = StartType.C_START_TYPE_NEW_START;
                        }
                    }

                    lst = (from t in GetStartTypeMiscType_CTS070()
                           where strStartTypeList.Contains<string>(t.ValueCode)
                           select t).ToList<doMiscTypeCode>();

                }

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(lst, "ValueCodeDisplay", "ValueCode");
                res.ResultData = cboModel;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        #endregion

        #region Method

        /// <summary>
        /// Check authority data of screen
        /// </summary>
        /// <param name="res"></param>
        /// <param name="strContractCodeLong"></param>
        /// <param name="isInitScreen"></param>
        /// <returns></returns>
        private dsRentalContractData CheckDataAuthority_CTS070(ObjectResultData res, string strContractCodeLong, bool isInitScreen = false)
        {
            CommonUtil comUtil = new CommonUtil();
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;
            string strLastOCC;

            rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

            /*--- Get contract data ---*/
            //Get last OCC for create change
            //•	Get last unimplemented OCC
            strLastOCC = rentralHandler.GetLastUnimplementedOCC(strContractCodeLong);
            if (String.IsNullOrEmpty(strLastOCC))
            {
                strLastOCC = rentralHandler.GetLastImplementedOCC(strContractCodeLong);
            }

            //Get entire contract data
            dsRentalContract = rentralHandler.GetEntireContract(strContractCodeLong, strLastOCC);

            if (dsRentalContract == null || dsRentalContract.dtTbt_RentalContractBasic == null || dsRentalContract.dtTbt_RentalContractBasic.Count < 1)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                if (isInitScreen)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124);
                }
                else
                {
                    string strContractCode = comUtil.ConvertContractCode(strContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { strContractCode }, new string[] { "txtSpecifyContractCode" });
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
        private void ValidateBusiness_CTS070(ObjectResultData res, string strContractCodeLong, tbt_RentalContractBasic tbt_RentalContractBasicData)
        {
            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            dsRentalContractData dsRentalContract = null;
            //CTS070_doRentalContractBasicAuthority doRentalContractBasicAuthority;

            try
            {
                //Check authority
                //doRentalContractBasicAuthority = CommonUtil.CloneObject<tbt_RentalContractBasic, CTS070_doRentalContractBasicAuthority>(tbt_RentalContractBasicData);
                //ValidatorUtil.BuildErrorMessage(res, new object[] { doRentalContractBasicAuthority }, null, false);
                //if (res.IsError)
                //    return;
                if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == tbt_RentalContractBasicData.ContractOfficeCode; }).Count() == 0
                    && CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == tbt_RentalContractBasicData.OperationOfficeCode; }).Count() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    return;
                }

                if (tbt_RentalContractBasicData == null)
                {
                    dsRentalContract = CheckDataAuthority_CTS070(res, strContractCodeLong);
                    if (res.IsError)
                        return;

                    tbt_RentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];
                }

                if (tbt_RentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
                    || (tbt_RentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                        && tbt_RentalContractBasicData.StartType == StartType.C_START_TYPE_ALTER_START)
                    || tbt_RentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                {
                    //Change spec to move error to else case
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3167, null, new string[] { "txtSpecifyContractCode" });
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3167, null, new string[] { "txtSpecifyContractCode" });
                    return;
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Validate business when register data
        /// </summary>
        /// <param name="res"></param>
        /// <param name="doStartResumeService"></param>
        private void ValidateBusinessOnRegister_CTS070(ObjectResultData res, CTS070_doStartResumeService doStartResumeService)
        {
            IBillingHandler billingHandler;
            IRentralContractHandler rentalHandler;

            try
            {
                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();

                billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                rentalHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                //Validate dtpAdjustBillingTerm can be input when there is only one billing target
                if (doStartResumeService.AdjustBillingTerm != null)
                {
                    //Get billing basic
                    List<tbt_BillingBasic> tbt_BillingBasicList = billingHandler.GetTbt_BillingBasic(sParam.CTS070_Session.InitialData.ContractCode, null);

                    if (tbt_BillingBasicList != null && tbt_BillingBasicList.Count > 1 && tbt_BillingBasicList[0].StopBillingFlag == FlagType.C_FLAG_OFF)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3259);
                        return;
                    }

                }

                //Add by Jutarat A. on 12092012
                //Validate data must not be updated by another user 
                //Get rental contract basic data
                List<tbt_RentalContractBasic> doValidate_RentalContractBasic = rentalHandler.GetTbt_RentalContractBasic(sParam.CTS070_Session.InitialData.ContractCode, null);

                //Compare update date
                if (sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalContractBasic != null && doValidate_RentalContractBasic != null
                    && sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalContractBasic.Count > 0 && doValidate_RentalContractBasic.Count > 0)
                {
                    if (DateTime.Compare(sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalContractBasic[0].UpdateDate.GetValueOrDefault(), doValidate_RentalContractBasic[0].UpdateDate.GetValueOrDefault()) != 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                        return;
                    }
                }

                string strLastUnimpleOCC = rentalHandler.GetLastUnimplementedOCC(sParam.CTS070_Session.InitialData.ContractCode);
                if (String.IsNullOrEmpty(strLastUnimpleOCC) == false)
                {
                    //Get rental security basic data
                    List<tbt_RentalSecurityBasic> doValidate_RentalSecurityBasic = rentalHandler.GetTbt_RentalSecurityBasic(sParam.CTS070_Session.InitialData.ContractCode, strLastUnimpleOCC);

                    //Compare update date
                    if (sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalSecurityBasic != null && doValidate_RentalSecurityBasic != null
                        && sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalSecurityBasic.Count > 0 && doValidate_RentalSecurityBasic.Count > 0)
                    {
                        if (DateTime.Compare(sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RentalSecurityBasic[0].UpdateDate.GetValueOrDefault(), doValidate_RentalSecurityBasic[0].UpdateDate.GetValueOrDefault()) != 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                            return;
                        }
                    }
                    //End Add
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Validate business when StartType as NewStart
        /// </summary>
        /// <param name="res"></param>
        /// <param name="doStartResumeService"></param>
        private void ValidateBusinessNewStart_CTS070(ObjectResultData res, CTS070_doStartResumeService doStartResumeService)
        {
            tbt_RentalContractBasic doRentalContractBasic = null;
            IRentralContractHandler rentalHandler;
            IContractHandler contHandler;
            IInstallationHandler installHandler;
            ISaleContractHandler saleHandler; //Add by Jutarat A. on 14082012

            string lblStartResumeDate = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CP05, "lblStartResumeOperationDate");

            try
            {
                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();
                doRentalContractBasic = sParam.CTS070_Session.InitialData.RentalContractBasicData;

                rentalHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                contHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler; //Add by Jutarat A. on 14082012
                installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                //Validate Start/resume operation date
                if (doStartResumeService.StartResumeOperationDate > DateTime.Now.Date)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0009, new string[] { "Start/resume operation date" }, new string[] { "dpStartResumeOperationDate" });
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0009, new string[] { lblStartResumeDate }, new string[] { "dpStartResumeOperationDate" });
                    return;
                }

                if (doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                {
                    if (doStartResumeService.StartResumeOperationDate.Value.Month != DateTime.Now.Month)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3170, null, new string[] { "dpStartResumeOperationDate" });
                        return;
                    }
                }
                else
                {
                    if (doStartResumeService.StartResumeOperationDate < DateTime.Now.AddYears(-1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3169, null, new string[] { "dpStartResumeOperationDate" });
                        return;
                    }
                }

                //Add by Jutarat A. on 30042013
                //Validate Adjust billing term
                if (doStartResumeService.AdjustBillingTerm != null && doStartResumeService.AdjustBillingTerm < doStartResumeService.StartResumeOperationDate)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3308, null, new string[] { "dpAdjustBillingTerm" });
                    return;
                }
                //End Add

                //Validate installation complete status of last occurrence in security basic table (unimplemented) must be installation complete (In case of  alarm)
                if (doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_AL || doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    //Get last unimplemented OCC and check installation complete
                    string strLastUnimpleOCC = rentalHandler.GetLastUnimplementedOCC(sParam.CTS070_Session.InitialData.ContractCode);
                    if (String.IsNullOrEmpty(strLastUnimpleOCC) == false)
                    {
                        List<tbt_RentalSecurityBasic> doUnimpleTbt_RentalSecurityBasic = rentalHandler.GetTbt_RentalSecurityBasic(sParam.CTS070_Session.InitialData.ContractCode, strLastUnimpleOCC);
                        if (doUnimpleTbt_RentalSecurityBasic != null && doUnimpleTbt_RentalSecurityBasic.Count > 0)
                        {
                            if (doUnimpleTbt_RentalSecurityBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_OFF)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3171);
                                return;
                            }
                        }
                    }

                }

                //Validate contarct report or P/O
                if (( doRentalContractBasic.PODocAuditResult == DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED
                      || doRentalContractBasic.PODocAuditResult == DocAuditResult.C_DOC_AUDIT_RESULT_RETURNED 
                      || doRentalContractBasic.PODocAuditResult == null)
                    && ( doRentalContractBasic.ContractDocAuditResult == DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED
                         || doRentalContractBasic.ContractDocAuditResult == DocAuditResult.C_DOC_AUDIT_RESULT_RETURNED
                         || doRentalContractBasic.ContractDocAuditResult == null))
                {
                    if (String.IsNullOrEmpty(doStartResumeService.ApproveNo))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3172, null, new string[] { "txtApproveNo" });
                        return;
                    }
                }

                //In case start maintenance contract code and there is relation with N-code (Alarm contract), N-code (Alarm contract) must be started service
                if (doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                {
                    if (sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RelationType != null)
                    {
                        foreach (tbt_RelationType data in sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RelationType)
                        {
                            //Change spec (remove)
                            ////Get contract basic data of related MA
                            //List<tbt_RentalContractBasic> doRelatedTbt_RentalContractBasic = rentalHandler.GetTbt_RentalContractBasic(data.RelatedContractCode, null);

                            ////In case of rental contract, check contract status, must be after start service
                            //if (doRelatedTbt_RentalContractBasic != null && doRelatedTbt_RentalContractBasic.Count > 0)
                            //{
                            //    if (doRelatedTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
                            //        || doRelatedTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL
                            //        || doRelatedTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_END
                            //        || doRelatedTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL
                            //        || doRelatedTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                            //    {
                            //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3173);
                            //        return;
                            //    }
                            //}

                            //Check contract status of MA Target contract
                            doContractHeader doHeader = contHandler.CheckMaintenanceTargetContract(data.RelatedContractCode, sParam.CTS070_Session.InitialData.ContractCode);

                        }
                    }

                }

                //Change spec (remove)
                ////Validate IE installation checking from Installation module
                //if (doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_AL || doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                //{
                //    //Get IE installation checking from Installation module
                //    bool blnInstallationChecking = installHandler.CheckAllIEComplete(sParam.CTS070_Session.InitialData.ContractCode);

                //    if (blnInstallationChecking == FlagType.C_FLAG_OFF)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3178);
                //        return;
                //    }
                //}

                //Add by Jutarat A. on 14082012
                //Check linkage sale contract (It must be completed)
                List<tbt_SaleBasic> dtTbt_SaleBasic = saleHandler.GetLinkageSaleContractData(sParam.CTS070_Session.InitialData.ContractCode);
                if (dtTbt_SaleBasic != null && dtTbt_SaleBasic.Count > 0)
                {
                    if ((dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE 
                            || dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE)
                        && (dtTbt_SaleBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_OFF))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3297);
                        return;
                    }
                }
                //End Add

                //Add by Jutarat A. on 12092012
                //In  case alarm or rental sale, installation module must not be registed
                if (doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                    || doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    //Get installation status
                    string strInstallationStatus = installHandler.GetInstallationStatus(sParam.CTS070_Session.InitialData.ContractCode);
                    if (strInstallationStatus != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3171);
                        return;
                    }
                }
                //End Add

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);

                //Check contract status of MA Target contract
                if (doRentalContractBasic != null && doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                {
                    if (res.MessageList != null && res.MessageList.Count > 0)
                    {
                        if (res.MessageList[0].Code == "MSG3044"
                            || res.MessageList[0].Code == "MSG3105"
                            || res.MessageList[0].Code == "MSG3116")
                        {
                            res.MessageList.Clear();
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3173);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validate business when StartType as AlternativeStart
        /// </summary>
        /// <param name="res"></param>
        /// <param name="doStartResumeService"></param>
        private void ValidateBusinessAlternativeStart_CTS070(ObjectResultData res, CTS070_doStartResumeService doStartResumeService)
        {
            tbt_RentalContractBasic doRentalContractBasic;
            ISaleContractHandler saleHandler; //Add by Jutarat A. on 14082012

            string lblStartResumeDate = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CP05, "lblStartResumeOperationDate");

            try
            {
                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();
                doRentalContractBasic = sParam.CTS070_Session.InitialData.RentalContractBasicData;

                saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler; //Add by Jutarat A. on 14082012

                //Validate Start/resume operation date
                if (doStartResumeService.StartResumeOperationDate > DateTime.Now.Date)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0009, new string[] { "Start/resume operation date" }, new string[] { "dpStartResumeOperationDate" });
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0009, new string[] { lblStartResumeDate }, new string[] { "dpStartResumeOperationDate" });
                    return;
                }

                if (doStartResumeService.StartResumeOperationDate < DateTime.Now.AddYears(-1))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3169, null, new string[] { "dpStartResumeOperationDate" });
                    return;
                }

                //Add by Jutarat A. on 30042013
                //Validate Adjust billing term
                if (doStartResumeService.AdjustBillingTerm != null && doStartResumeService.AdjustBillingTerm < doStartResumeService.StartResumeOperationDate)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3308, null, new string[] { "dpAdjustBillingTerm" });
                    return;
                }
                //End Add

                //Validate product type 
                if (doRentalContractBasic.ProductTypeCode != ProductType.C_PROD_TYPE_AL && doRentalContractBasic.ProductTypeCode != ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3174);
                    return;
                }

                if (doRentalContractBasic.ContractStatus != ContractStatus.C_CONTRACT_STATUS_BEF_START)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3268);
                    return;
                }

                //Add by Jutarat A. on 14082012
                //Check linkage sale contract (It must be completed)
                List<tbt_SaleBasic> dtTbt_SaleBasic = saleHandler.GetLinkageSaleContractData(sParam.CTS070_Session.InitialData.ContractCode);
                if (dtTbt_SaleBasic != null && dtTbt_SaleBasic.Count > 0)
                {
                    if ((dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE
                            || dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE)
                        && (dtTbt_SaleBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_OFF))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3297);
                        return;
                    }
                }
                //End Add

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Validate business when StartType as Resume
        /// </summary>
        /// <param name="res"></param>
        /// <param name="doStartResumeService"></param>
        private void ValidateBusinessResume_CTS070(ObjectResultData res, CTS070_doStartResumeService doStartResumeService)
        {
            tbt_RentalContractBasic doRentalContractBasic;
            IRentralContractHandler rentalHandler;
            ISaleContractHandler saleHandler; //Add by Jutarat A. on 14082012

            string lblStartResumeDate = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CP05, "lblStartResumeOperationDate");

            try
            {
                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();
                doRentalContractBasic = sParam.CTS070_Session.InitialData.RentalContractBasicData;

                rentalHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler; //Add by Jutarat A. on 14082012

                //Validate Start/resume operation date
                if (doStartResumeService.StartResumeOperationDate > DateTime.Now.Date)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0009, new string[] { "Start/resume operation date" }, new string[] { "dpStartResumeOperationDate" });
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0009, new string[] { lblStartResumeDate }, new string[] { "dpStartResumeOperationDate" });
                    return;
                }

                if (doStartResumeService.StartResumeOperationDate < DateTime.Now.AddYears(-1))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3169, null, new string[] { "dpStartResumeOperationDate" });
                    return;
                }

                //Add by Jutarat A. on 30042013
                //Validate Adjust billing term
                if (doStartResumeService.AdjustBillingTerm != null && doStartResumeService.AdjustBillingTerm < doStartResumeService.StartResumeOperationDate)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3308, null, new string[] { "dpAdjustBillingTerm" });
                    return;
                }
                //End Add

                //Validate unimplemented contract data, unimplement data must not exist
                //Get last OCC of unimplement contract data
                string strUnimpleOCC = rentalHandler.GetLastUnimplementedOCC(doRentalContractBasic.ContractCode);

                //Unimplement data must not exist
                if (String.IsNullOrEmpty(strUnimpleOCC) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3175);
                    return;
                }

                //Validate contract status must be stop service
                if (doRentalContractBasic.ContractStatus != ContractStatus.C_CONTRACT_STATUS_STOPPING)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3176);
                    return;
                }

                //Add by Jutarat A. on 14082012
                //Check linkage sale contract (It must be completed)
                List<tbt_SaleBasic> dtTbt_SaleBasic = saleHandler.GetLinkageSaleContractData(sParam.CTS070_Session.InitialData.ContractCode);
                if (dtTbt_SaleBasic != null && dtTbt_SaleBasic.Count > 0)
                {
                    if ((dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE
                            || dtTbt_SaleBasic[0].ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE)
                        && (dtTbt_SaleBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_OFF))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3297);
                        return;
                    }
                }
                //End Add

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Save data when StartType as NewStart
        /// </summary>
        /// <param name="res"></param>
        /// <param name="doStartResumeService"></param>
        private void SaveNewStart_CTS070(ObjectResultData res, CTS070_doStartResumeService doStartResumeService)
        {
            CTS070_RegisterStartResumeTargetData registerStartResumeTargetData;
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;
            IBillingInterfaceHandler billingHandler;
            IBillingTempHandler billingTempHandler;
            IInventoryHandler inventHandler;
            IMaintenanceHandler maintenHandler;
            ISaleContractHandler saleHandler;
            IContractDocumentHandler contractDocHandler;
            IQuotationHandler quotHandler;
            ICommonContractHandler comContractHandler;

            try
            {
                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();

                using (TransactionScope scope = new TransactionScope())
                {
                    /*--- RegisterStopService ---*/
                    registerStartResumeTargetData = sParam.CTS070_Session;
                    if (registerStartResumeTargetData.RegisterStartResumeData != null)
                    {
                        dsRentalContract = new dsRentalContractData();

                        //Generate security occurrence
                        rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        string strNewOCC = rentralHandler.GenerateContractOCC(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode, true);

                        string strStartType = registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalContractBasic[0].StartType;

                        //Prepare data for saving
                        /*--- PrepareForSaveNewStart ---*/
                        //Get summary fee data from unimplement data
                        List<doSummaryFee> sumFeeList = rentralHandler.SumFeeUnimplementData(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode);

                        //dtTbt_RentalContractBasic
                        dsRentalContract.dtTbt_RentalContractBasic = new List<tbt_RentalContractBasic>();

                        tbt_RentalContractBasic rentalContractBasic = CommonUtil.CloneObject<tbt_RentalContractBasic, tbt_RentalContractBasic>(registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalContractBasic[0]);
                        rentalContractBasic.ContractCode = registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode;
                        rentalContractBasic.LastOCC = strNewOCC;
                        rentalContractBasic.LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_NEW_START;
                        rentalContractBasic.ContractStatus = ContractStatus.C_CONTRACT_STATUS_AFTER_START;
                        rentalContractBasic.StartType = StartType.C_START_TYPE_NEW_START;

                        if (sumFeeList != null && sumFeeList.Count > 0)
                        {
                            rentalContractBasic.NormalDepositFee = sumFeeList[0].NormalAdditionalDepositFee;
                            rentalContractBasic.OrderDepositFee = sumFeeList[0].OrderAdditionalDepositFee;
                        }

                        if (registerStartResumeTargetData.InitialData.RentalContractBasicData.StartDealDate == null
                            && registerStartResumeTargetData.InitialData.RentalContractBasicData.OldContractCode == null)
                        {
                            rentalContractBasic.StartDealDate = doStartResumeService.StartResumeOperationDate;
                        }
                        else
                        {
                            //Bug report CT-142
                            if (String.IsNullOrEmpty(registerStartResumeTargetData.InitialData.RentalContractBasicData.OldContractCode) == false)
                            {
                                List<tbt_RentalContractBasic> doOldContractCodeBasic = rentralHandler.GetTbt_RentalContractBasic(registerStartResumeTargetData.InitialData.RentalContractBasicData.OldContractCode, null);
                                if (doOldContractCodeBasic != null && doOldContractCodeBasic.Count > 0)
                                {
                                    rentalContractBasic.StartDealDate = doOldContractCodeBasic[0].StartDealDate;
                                }
                            }
                        }

                        if (registerStartResumeTargetData.InitialData.RentalContractBasicData.FirstSecurityStartDate == null)
                        {
                            rentalContractBasic.FirstSecurityStartDate = doStartResumeService.StartResumeOperationDate;
                        }

                        rentalContractBasic.StartResumeProcessDate = DateTime.Now.Date;
                        rentalContractBasic.UserCode = doStartResumeService.UserCode;
                        rentalContractBasic.LastChangeImplementDate = doStartResumeService.StartResumeOperationDate;
                        dsRentalContract.dtTbt_RentalContractBasic.Add(rentalContractBasic);

                        //dtTbt_RentalSecurityBasic
                        dsRentalContract.dtTbt_RentalSecurityBasic = new List<tbt_RentalSecurityBasic>();

                        tbt_RentalSecurityBasic rentalSecurityBasicData = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSecurityBasic[0]);
                        rentalSecurityBasicData.ContractCode = registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode;
                        rentalSecurityBasicData.OCC = strNewOCC;
                        rentalSecurityBasicData.ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_NEW_START;
                        rentalSecurityBasicData.ImplementFlag = FlagType.C_FLAG_ON;
                        rentalSecurityBasicData.PhoneLineTypeCode1 = doStartResumeService.LineTypeNormal;
                        rentalSecurityBasicData.PhoneLineOwnerTypeCode1 = doStartResumeService.TelephoneOwnerNormal;
                        rentalSecurityBasicData.PhoneNo1 = doStartResumeService.TelephoneNoNormal;
                        rentalSecurityBasicData.PhoneLineTypeCode2 = doStartResumeService.LineTypeImage;
                        rentalSecurityBasicData.PhoneLineOwnerTypeCode2 = doStartResumeService.TelephoneOwnerImage;
                        rentalSecurityBasicData.PhoneNo2 = doStartResumeService.TelephoneNoImage;
                        rentalSecurityBasicData.PhoneLineTypeCode3 = doStartResumeService.LineTypeDisconnection;
                        rentalSecurityBasicData.PhoneLineOwnerTypeCode3 = doStartResumeService.TelephoneOwnerDisconnection;
                        rentalSecurityBasicData.PhoneNo3 = doStartResumeService.TelephoneNoDisconnection;

                        //Get data of last implement (in case already alternative start)
                        List<tbt_RentalSecurityBasic> doImplementSecurityBasic = null;
                        string strLastOCC = rentralHandler.GetLastImplementedOCC(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode);
                        if (String.IsNullOrEmpty(strLastOCC) == false)
                        {
                            doImplementSecurityBasic = rentralHandler.GetTbt_RentalSecurityBasic(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode, strLastOCC);
                        }

                        //rentalSecurityBasicData.ContractStartDate = doStartResumeService.StartResumeOperationDate;
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            rentalSecurityBasicData.ContractStartDate = doStartResumeService.StartResumeOperationDate;
                            if (rentalSecurityBasicData.ContractEndDate == null)
                            {
                                rentalSecurityBasicData.CalContractEndDate = doStartResumeService.StartResumeOperationDate.Value.AddMonths(rentalSecurityBasicData.ContractDurationMonth.Value).AddDays(-1);
                            }
                            else
                            {
                                rentalSecurityBasicData.CalContractEndDate = rentalSecurityBasicData.ContractEndDate;
                            }
                        }
                        else if (doImplementSecurityBasic != null && doImplementSecurityBasic.Count > 0)
                        {
                            rentalSecurityBasicData.ContractStartDate = doImplementSecurityBasic[0].ContractStartDate;
                            rentalSecurityBasicData.CalContractEndDate = doImplementSecurityBasic[0].CalContractEndDate;
                        }

                        /*
                        if (doStartResumeService.StartResumeOperationDate != null)
                        {
                            int intContractDurationMonth = rentalSecurityBasicData.ContractDurationMonth != null ? rentalSecurityBasicData.ContractDurationMonth.Value : 0;
                            if (rentalSecurityBasicData.ContractEndDate != null)
                                rentalSecurityBasicData.ContractEndDate = doStartResumeService.StartResumeOperationDate.Value.AddMonths(intContractDurationMonth).AddDays(-1);

                            rentalSecurityBasicData.CalContractEndDate = doStartResumeService.StartResumeOperationDate.Value.AddMonths(intContractDurationMonth).AddDays(-1);
                        }*/

                        rentalSecurityBasicData.ChangeImplementDate = doStartResumeService.StartResumeOperationDate;

                        //Modify by Jutarat A. on 14082012
                        //rentalSecurityBasicData.ApproveNo1 = doStartResumeService.ApproveNo;
                        //rentalSecurityBasicData.ApproveNo2 = null;
                        //rentalSecurityBasicData.ApproveNo3 = null;
                        //rentalSecurityBasicData.ApproveNo4 = null;
                        //rentalSecurityBasicData.ApproveNo5 = null;
                        if (String.IsNullOrEmpty(rentalSecurityBasicData.ApproveNo1))
                        {
                            rentalSecurityBasicData.ApproveNo1 = doStartResumeService.ApproveNo;
                        }
                        else if (String.IsNullOrEmpty(rentalSecurityBasicData.ApproveNo2))
                        {
                            rentalSecurityBasicData.ApproveNo2 = doStartResumeService.ApproveNo;
                        }
                        else if (String.IsNullOrEmpty(rentalSecurityBasicData.ApproveNo3))
                        {
                            rentalSecurityBasicData.ApproveNo3 = doStartResumeService.ApproveNo;
                        }
                        else if (String.IsNullOrEmpty(rentalSecurityBasicData.ApproveNo4))
                        {
                            rentalSecurityBasicData.ApproveNo4 = doStartResumeService.ApproveNo;
                        }
                        else if (!String.IsNullOrEmpty(doStartResumeService.ApproveNo))
                        {
                            rentalSecurityBasicData.ApproveNo5 = doStartResumeService.ApproveNo;
                        }  
                        //End Modify

                        rentalSecurityBasicData.CounterNo = 0;
                        rentalSecurityBasicData.ChangeReasonType = null;
                        rentalSecurityBasicData.ChangeNameReasonType = null;
                        rentalSecurityBasicData.StopCancelReasonType = null;
                        rentalSecurityBasicData.UninstallType = null;
                        rentalSecurityBasicData.ContractDocPrintFlag = null;

                        
                        //use old value
                        //rentalSecurityBasicData.ExpectedInstallationCompleteDate = null;
                        //rentalSecurityBasicData.InstallationCompleteFlag = FlagType.C_FLAG_OFF;
                        //rentalSecurityBasicData.InstallationSlipNo = null;
                        //rentalSecurityBasicData.InstallationCompleteDate = null;

                        if (rentalSecurityBasicData.InstallationTypeCode != null)
                        {
                            rentalSecurityBasicData.InstallationTypeCode = RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW;
                        }
                        //Modify by Phoomsak L. 2012-10-12 CT-266
                        //rentalSecurityBasicData.NegotiationStaffEmpNo1 = null;
                        //rentalSecurityBasicData.NegotiationStaffEmpNo2 = null;

                        if (sumFeeList != null && sumFeeList.Count > 0)
                        { 
                            rentalSecurityBasicData.NormalInstallFee = sumFeeList[0].NormalInstallFee;
                            rentalSecurityBasicData.OrderInstallFee = sumFeeList[0].OrderInstallFee;
                            rentalSecurityBasicData.OrderInstallFee_ApproveContract = sumFeeList[0].OrderInstallFee_ApproveContract;
                            rentalSecurityBasicData.OrderInstallFee_CompleteInstall = sumFeeList[0].OrderInstallFee_CompleteInstall;
                            rentalSecurityBasicData.OrderInstallFee_StartService = sumFeeList[0].OrderInstallFee_StartService;
                            rentalSecurityBasicData.AdditionalFee1 = sumFeeList[0].AdditionalFee1;
                            rentalSecurityBasicData.AdditionalFee2 = sumFeeList[0].AdditionalFee2;
                            rentalSecurityBasicData.AdditionalFee3 = sumFeeList[0].AdditionalFee3;
                            rentalSecurityBasicData.MaintenanceFee1 = sumFeeList[0].MaintenanceFee1;
                            rentalSecurityBasicData.MaintenanceFee2 = sumFeeList[0].MaintenanceFee2;
                            rentalSecurityBasicData.NormalAdditionalDepositFee = sumFeeList[0].NormalAdditionalDepositFee;
                            rentalSecurityBasicData.OrderAdditionalDepositFee = sumFeeList[0].OrderAdditionalDepositFee;
                        }

                        rentalSecurityBasicData.DepositFeeBillingTiming = null;
                        rentalSecurityBasicData.CompleteChangeOperationDate = null;
                        rentalSecurityBasicData.CompleteChangeOperationEmpNo = null;
                        rentalSecurityBasicData.ExpectedResumeDate = null; //Add by Jutarat A. on 02122013
                        dsRentalContract.dtTbt_RentalSecurityBasic.Add(rentalSecurityBasicData);

                        //dtTbt_RentalBEDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalBEDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalBEDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>();
                            tbt_RentalBEDetails rentalBEDetailsData;
                            foreach (tbt_RentalBEDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalBEDetails)
                            {
                                rentalBEDetailsData = CommonUtil.CloneObject<tbt_RentalBEDetails, tbt_RentalBEDetails>(data);
                                rentalBEDetailsData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalBEDetails.Add(rentalBEDetailsData);
                            }
                        }

                        //dtTbt_RentalInstrumentDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalInstrumentDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalInstrumentDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                            tbt_RentalInstrumentDetails rentalInstrumentDetailsData;
                            foreach (tbt_RentalInstrumentDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalInstrumentDetails)
                            {
                                rentalInstrumentDetailsData = CommonUtil.CloneObject<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(data);
                                rentalInstrumentDetailsData.OCC = strNewOCC;
                                rentalInstrumentDetailsData.InstrumentQty = rentalInstrumentDetailsData.InstrumentQty + rentalInstrumentDetailsData.AdditionalInstrumentQty - rentalInstrumentDetailsData.RemovalInstrumentQty;
                                rentalInstrumentDetailsData.AdditionalInstrumentQty = 0;
                                rentalInstrumentDetailsData.RemovalInstrumentQty = 0;
                                dsRentalContract.dtTbt_RentalInstrumentDetails.Add(rentalInstrumentDetailsData);
                            }
                        }

                        //dtTbt_RentalMaintenanceDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalMaintenanceDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalMaintenanceDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                            tbt_RentalMaintenanceDetails rentalMaintenanceDetailsData;
                            foreach (tbt_RentalMaintenanceDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalMaintenanceDetails)
                            {
                                rentalMaintenanceDetailsData = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(data);
                                rentalMaintenanceDetailsData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalMaintenanceDetails.Add(rentalMaintenanceDetailsData);
                            }
                        }

                        //dtTbt_RentalOperationType
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalOperationType != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalOperationType.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                            tbt_RentalOperationType rentalOperationTypeData;
                            foreach (tbt_RentalOperationType data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalOperationType)
                            {
                                rentalOperationTypeData = CommonUtil.CloneObject<tbt_RentalOperationType, tbt_RentalOperationType>(data);
                                rentalOperationTypeData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalOperationType.Add(rentalOperationTypeData);
                            }
                        }

                        //dtTbt_RentalSentryGuard
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuard != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuard.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>();
                            tbt_RentalSentryGuard rentalSentryGuardData;
                            foreach (tbt_RentalSentryGuard data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuard)
                            {
                                rentalSentryGuardData = CommonUtil.CloneObject<tbt_RentalSentryGuard, tbt_RentalSentryGuard>(data);
                                rentalSentryGuardData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalSentryGuard.Add(rentalSentryGuardData);
                            }
                        }

                        //dtTbt_RentalSentryGuardDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuardDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuardDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                            tbt_RentalSentryGuardDetails rentalSentryGuardDetailsData;
                            foreach (tbt_RentalSentryGuardDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuardDetails)
                            {
                                rentalSentryGuardDetailsData = CommonUtil.CloneObject<tbt_RentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(data);
                                rentalSentryGuardDetailsData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalSentryGuardDetails.Add(rentalSentryGuardDetailsData);
                            }
                        }

                        //dtTbt_RelationType
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RelationType != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RelationType.Count > 0)
                        {
                            dsRentalContract.dtTbt_RelationType = new List<tbt_RelationType>();
                            tbt_RelationType relationTypeData;
                            foreach (tbt_RelationType data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RelationType)
                            {
                                relationTypeData = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(data);
                                relationTypeData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RelationType.Add(relationTypeData);
                            }

                            if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalContractBasic != null
                                && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                            {
                                List<string> listContractCode = new List<string>();
                                foreach (tbt_RelationType item in dsRentalContract.dtTbt_RelationType)
                                {
                                    listContractCode.Add(item.RelatedContractCode);
                                }

                                //Generate MA relation type
                                comContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                                List<tbt_RelationType> listGenerate = comContractHandler.GenerateMaintenanceRelationType(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode, listContractCode);

                                //Delete all rows in dtTbt_RelationType which have RelationType = C_RELATION_TYPE_MA
                                List<tbt_RelationType> listRelationType = dsRentalContract.dtTbt_RelationType.FindAll(delegate(tbt_RelationType s) { return s.RelationType == RelationType.C_RELATION_TYPE_MA; });
                                foreach (var item in listRelationType)
                                {
                                    dsRentalContract.dtTbt_RelationType.Remove(item);
                                }

                                //Insert new_dtTbt_RelationType to dtEntireContract.dtTbt_RelationType
                                foreach (tbt_RelationType item in listGenerate)
                                {
                                    item.OCC = strNewOCC;
                                    dsRentalContract.dtTbt_RelationType.Add(item);
                                }
                            }

                        }

                        /*---------------------------*/

                        //Save contract data
                        //dsRentalContractData dsRentalContractResult = rentralHandler.InsertEntireContract(dsRentalContract);
                        rentralHandler.InsertEntireContractForCTS010(dsRentalContract);

                        //Send billing data to billing module
                        //if (String.IsNullOrEmpty(strStartType))
                        //{
                            billingHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                            billingHandler.SendBilling_StartService(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode, doStartResumeService.StartResumeOperationDate.Value, doStartResumeService.AdjustBillingTerm, strStartType);
                        //}

                        //Delete unimplemented rental contract data
                        string strOCCout = rentralHandler.GetLastUnimplementedOCC(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode);
                        while (String.IsNullOrEmpty(strOCCout) == false)
                        {
                            rentralHandler.DeleteEntireOCC(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode, strOCCout, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                            strOCCout = rentralHandler.GetLastUnimplementedOCC(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode);
                        }

                        //Delete all billing data in billing temp
                        billingTempHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                        List<tbt_BillingTemp> dtDeletedTbt_BillingTemp = billingTempHandler.DeleteBillingTempByContractCode(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode);

#if !ROUND1
                        inventHandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                        //Update inventory process (In case contract type is alarm only)
                        if (registerStartResumeTargetData.InitialData.RentalContractBasicData.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                            || registerStartResumeTargetData.InitialData.RentalContractBasicData.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                        {
                            inventHandler.UpdateContractStartService(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode);
                        }

#endif

                        maintenHandler = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;

                        //Generate maintenance check-up schedule (In case contract type is maintenance or alarm only)
                        if (registerStartResumeTargetData.InitialData.RentalContractBasicData.ProductTypeCode == ProductType.C_PROD_TYPE_MA ||
                            registerStartResumeTargetData.InitialData.RentalContractBasicData.ProductTypeCode == ProductType.C_PROD_TYPE_AL ||
                            registerStartResumeTargetData.InitialData.RentalContractBasicData.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                        {
                            maintenHandler.GenerateMaintenanceSchedule(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode, GenerateMAProcessType.C_GEN_MA_TYPE_CREATE);
                        }

                        //Update maintenance data to sale basic table  (In case sale contract have relation with maintenance contract)
                        if (registerStartResumeTargetData.InitialData.RentalContractBasicData.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                        {
                            //Update MA date
                            if (dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0
                                && dsRentalContract.dtTbt_RentalContractBasic[0].StartResumeProcessDate != null)
                            {
                                maintenHandler.UpdateMADateInSaleContract(dsRentalContract.dtTbt_RentalContractBasic[0].StartResumeProcessDate.Value, dsRentalContract);
                            }

                            //Change spec (remove)
                            //if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RelationType != null)
                            //{
                            //    foreach (tbt_RelationType data in sParam.CTS070_Session.RegisterStartResumeData.dtTbt_RelationType)
                            //    {
                            //        saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                            //        //Get last OCC 
                            //        string strLastOCC = saleHandler.GetLastOCC(data.RelatedContractCode);
                            //        if (String.IsNullOrEmpty(strLastOCC) == false)
                            //        {
                            //            //Update start & end maintenance date
                            //            List<tbt_SaleBasic> doSaleBasicList = saleHandler.GetTbt_SaleBasic(data.RelatedContractCode, strLastOCC, true);
                            //            if (doSaleBasicList != null)
                            //            {
                            //                foreach (tbt_SaleBasic doSale in doSaleBasicList)
                            //                {
                            //                    doSale.StartMaintenanceDate = doStartResumeService.StartResumeOperationDate;
                            //                    doSale.EndMaintenanceDate = doStartResumeService.StartResumeOperationDate.Value.AddDays(rentalSecurityBasicData.ContractDurationMonth.Value - 1);

                            //                    saleHandler.UpdateTbt_SaleBasic(doSale);
                            //                }
                            //            }

                            //        }

                            //    }
                            //}

                        }

                        //Comment by Jutarat A. on 22042013
                        ////Generate start memorandum report
                        ////Generate contract doc OCC
                        //contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
                        //string strContractDocOCC = contractDocHandler.GenerateDocOCC(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode, strNewOCC);

                        ////Prepare contract document data
                        //List<tbt_ContractDocument> contractDocList = new List<tbt_ContractDocument>();
                        //tbt_ContractDocument contractDoc = new tbt_ContractDocument();
                        //contractDoc.ContractCode = registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode;
                        //contractDoc.OCC = ParticularOCC.C_PARTICULAR_OCC_START_CONFIRM_LETTER;
                        //contractDoc.ContractDocOCC = strContractDocOCC;
                        //contractDoc.DocNo = contractDoc.ContractCode + '-' + contractDoc.OCC + '-' + contractDoc.ContractDocOCC;
                        //contractDoc.DocumentCode = DocumentCode.C_DOCUMENT_CODE_START_OPER_CONFIRM_LETTER;
                        //contractDoc.IssuedDate = DateTime.Now.Date;
                        //contractDoc.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_ISSUED;
                        //contractDoc.DocAuditResult = DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED;
                        //contractDocList.Add(contractDoc);

                        ////Insert data to contract document 
                        //contractDocHandler.InsertTbt_ContractDocument(contractDocList);
                        //End Comment

                        //Update operation office to Quotation module
                        doUpdateQuotationData doQuotData = new doUpdateQuotationData();
                        doQuotData.ActionTypeCode = ActionType.C_ACTION_TYPE_START;
                        if (dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
                        {
                            doQuotData.QuotationTargetCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode;
                            doQuotData.QuotationOfficeCode = dsRentalContract.dtTbt_RentalContractBasic[0].OperationOfficeCode;
                        }

                        quotHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                        quotHandler.UpdateQuotationData(doQuotData);

                        //Lock quotation
                        bool blnLockQuotationResult = quotHandler.LockQuotation(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode
                                                    , null
                                                    , LockStyle.C_LOCK_STYLE_ALL);
                    }
                    /*--------------------------*/

                    scope.Complete();
                    res.ResultData = new object[] { MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046), sParam.ContractCode };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save data when StartType as AlternativeStart
        /// </summary>
        /// <param name="res"></param>
        /// <param name="doStartResumeService"></param>
        private void SaveAlternativeStart_CTS070(ObjectResultData res, CTS070_doStartResumeService doStartResumeService)
        {
            CTS070_RegisterStartResumeTargetData registerStartResumeTargetData;
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;
            IBillingInterfaceHandler billingHandler;
            IQuotationHandler quotHandler;

            try
            {
                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();

                using (TransactionScope scope = new TransactionScope())
                {
                    /*--- RegisterStopService ---*/
                    registerStartResumeTargetData = sParam.CTS070_Session;
                    if (registerStartResumeTargetData.RegisterStartResumeData != null)
                    {
                        dsRentalContract = new dsRentalContractData();

                        //Generate security occurrence
                        rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        string strNewOCC = rentralHandler.GenerateContractOCC(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode, true);

                        //Prepare data for saving
                        /*--- PrepareForSaveAlternativeStart ---*/
                        //dtTbt_RentalContractBasic
                        dsRentalContract.dtTbt_RentalContractBasic = new List<tbt_RentalContractBasic>();

                        tbt_RentalContractBasic rentalContractBasic = CommonUtil.CloneObject<tbt_RentalContractBasic, tbt_RentalContractBasic>(registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalContractBasic[0]);
                        rentalContractBasic.ContractCode = registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode;
                        rentalContractBasic.LastOCC = strNewOCC;
                        rentalContractBasic.LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_ALTERNATIVE_START;
                        rentalContractBasic.ContractStatus = ContractStatus.C_CONTRACT_STATUS_AFTER_START;
                        rentalContractBasic.StartType = StartType.C_START_TYPE_ALTER_START;

                        if (registerStartResumeTargetData.InitialData.RentalContractBasicData.OldContractCode == null)
                        {
                            rentalContractBasic.StartDealDate = doStartResumeService.StartResumeOperationDate;
                        }
                        else
                        {
                            //Bug report CT-142
                            if (String.IsNullOrEmpty(registerStartResumeTargetData.InitialData.RentalContractBasicData.OldContractCode) == false)
                            {
                                List<tbt_RentalContractBasic> doOldContractCodeBasic = rentralHandler.GetTbt_RentalContractBasic(registerStartResumeTargetData.InitialData.RentalContractBasicData.OldContractCode, null);
                                if (doOldContractCodeBasic != null && doOldContractCodeBasic.Count > 0)
                                {
                                    rentalContractBasic.StartDealDate = doOldContractCodeBasic[0].StartDealDate;
                                }
                            }
                        }

                        if (registerStartResumeTargetData.InitialData.RentalContractBasicData.FirstSecurityStartDate == null)
                        {
                            rentalContractBasic.FirstSecurityStartDate = doStartResumeService.StartResumeOperationDate;
                        }
                        rentalContractBasic.StartResumeProcessDate = DateTime.Now.Date;
                        rentalContractBasic.UserCode = doStartResumeService.UserCode;
                        rentalContractBasic.LastChangeImplementDate = doStartResumeService.StartResumeOperationDate;
                        dsRentalContract.dtTbt_RentalContractBasic.Add(rentalContractBasic);

                        //dtTbt_RentalSecurityBasic
                        dsRentalContract.dtTbt_RentalSecurityBasic = new List<tbt_RentalSecurityBasic>();
                        
                        tbt_RentalSecurityBasic rentalSecurityBasicData = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSecurityBasic[0]);
                        rentalSecurityBasicData.ContractCode = registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode;
                        rentalSecurityBasicData.OCC = strNewOCC;
                        rentalSecurityBasicData.ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_ALTERNATIVE_START;
                        rentalSecurityBasicData.ImplementFlag = FlagType.C_FLAG_ON;
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            rentalSecurityBasicData.ContractStartDate = doStartResumeService.StartResumeOperationDate;
                        }

                        /*
                        if (rentalSecurityBasicData.ContractEndDate != null)
                        {
                            rentalSecurityBasicData.ContractEndDate = doStartResumeService.StartResumeOperationDate.Value.AddMonths(rentalSecurityBasicData.ContractDurationMonth.Value - 1);
                        }
                        */

                        if (rentalSecurityBasicData.ContractEndDate == null)
                        {
                            rentalSecurityBasicData.CalContractEndDate = doStartResumeService.StartResumeOperationDate.Value.AddMonths(rentalSecurityBasicData.ContractDurationMonth.Value).AddDays(-1);
                        }
                        else
                        {
                            rentalSecurityBasicData.CalContractEndDate = rentalSecurityBasicData.ContractEndDate;
                        }

                        rentalSecurityBasicData.ChangeImplementDate = doStartResumeService.StartResumeOperationDate;

                        //Modify by Jutarat A. on 14082012
                        //rentalSecurityBasicData.ApproveNo1 = doStartResumeService.ApproveNo;
                        //rentalSecurityBasicData.ApproveNo2 = null;
                        //rentalSecurityBasicData.ApproveNo3 = null;
                        //rentalSecurityBasicData.ApproveNo4 = null;
                        //rentalSecurityBasicData.ApproveNo5 = null;
                        if (String.IsNullOrEmpty(rentalSecurityBasicData.ApproveNo1))
                        {
                            rentalSecurityBasicData.ApproveNo1 = doStartResumeService.ApproveNo;
                        }
                        else if (String.IsNullOrEmpty(rentalSecurityBasicData.ApproveNo2))
                        {
                            rentalSecurityBasicData.ApproveNo2 = doStartResumeService.ApproveNo;
                        }
                        else if (String.IsNullOrEmpty(rentalSecurityBasicData.ApproveNo3))
                        {
                            rentalSecurityBasicData.ApproveNo3 = doStartResumeService.ApproveNo;
                        }
                        else if (String.IsNullOrEmpty(rentalSecurityBasicData.ApproveNo4))
                        {
                            rentalSecurityBasicData.ApproveNo4 = doStartResumeService.ApproveNo;
                        }
                        else if (!String.IsNullOrEmpty(doStartResumeService.ApproveNo))
                        {
                            rentalSecurityBasicData.ApproveNo5 = doStartResumeService.ApproveNo;
                        }
                        //End Modify

                        rentalSecurityBasicData.CounterNo = 0;
                        rentalSecurityBasicData.ChangeReasonType = null;
                        rentalSecurityBasicData.ChangeNameReasonType = null;
                        rentalSecurityBasicData.StopCancelReasonType = null;
                        rentalSecurityBasicData.UninstallType = null;
                        rentalSecurityBasicData.ContractDocPrintFlag = null;
                        rentalSecurityBasicData.ExpectedInstallationCompleteDate = null;
                        rentalSecurityBasicData.InstallationCompleteFlag = FlagType.C_FLAG_OFF;
                        rentalSecurityBasicData.InstallationSlipNo = null;
                        rentalSecurityBasicData.InstallationCompleteDate = null;
                        rentalSecurityBasicData.InstallationTypeCode = null;
                        //Modify by Phoomsak L. 2012-10-12 CT-266
                        //rentalSecurityBasicData.NegotiationStaffEmpNo1 = null;
                        //rentalSecurityBasicData.NegotiationStaffEmpNo2 = null;
                        rentalSecurityBasicData.NormalInstallFee = null;
                        rentalSecurityBasicData.OrderInstallFee = null;
                        rentalSecurityBasicData.OrderInstallFee_ApproveContract = null;
                        rentalSecurityBasicData.OrderInstallFee_CompleteInstall = null;
                        rentalSecurityBasicData.OrderInstallFee_StartService = null;
                        rentalSecurityBasicData.AdditionalFee1 = null;
                        rentalSecurityBasicData.AdditionalFee2 = null;
                        rentalSecurityBasicData.AdditionalFee3 = null;
                        rentalSecurityBasicData.MaintenanceFee1 = null;
                        rentalSecurityBasicData.MaintenanceFee2 = null;
                        rentalSecurityBasicData.CompleteChangeOperationDate = null;
                        rentalSecurityBasicData.CompleteChangeOperationEmpNo = null;
                        rentalSecurityBasicData.ExpectedResumeDate = null; //Add by Jutarat A. on 02122013
                        dsRentalContract.dtTbt_RentalSecurityBasic.Add(rentalSecurityBasicData);

                        //dtTbt_RentalBEDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalBEDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalBEDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>();
                            tbt_RentalBEDetails rentalBEDetailsData;
                            foreach (tbt_RentalBEDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalBEDetails)
                            {
                                rentalBEDetailsData = CommonUtil.CloneObject<tbt_RentalBEDetails, tbt_RentalBEDetails>(data);
                                rentalBEDetailsData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalBEDetails.Add(rentalBEDetailsData);
                            }
                        }

                        //dtTbt_RentalInstrumentDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalInstrumentDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalInstrumentDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                            

                            //List<tbt_RentalInstrumentDetails> rentalInstrumentDetailsList = null;
                            
                            ////Add by Jutarat A. on 28092012
                            ////In  case installation complete status of last unimplemnt OCC is not complete
                            //if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSecurityBasic != null
                            //    && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSecurityBasic.Count > 0
                            //    && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag != FlagType.C_FLAG_ON)
                            //{
                            //    //Get previous OCC 
                            //    string strPreviousOCC = rentralHandler.GetPreviousUnimplementedOCC(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode, registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSecurityBasic[0].OCC);

                            //    //Get instrument detail of previous OCC
                            //    if (String.IsNullOrEmpty(strPreviousOCC) == false)
                            //    {
                            //        rentalInstrumentDetailsList = rentralHandler.GetTbt_RentalInstrumentDetails(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode, strPreviousOCC);
                            //    }
                            //}
                            //else
                            //{
                            //    rentalInstrumentDetailsList = registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalInstrumentDetails;
                            //}
                            ////End Add

                            //if (rentalInstrumentDetailsList != null)
                            //{
                                tbt_RentalInstrumentDetails rentalInstrumentDetailsData;
                                foreach (tbt_RentalInstrumentDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalInstrumentDetails)
                                {
                                    rentalInstrumentDetailsData = CommonUtil.CloneObject<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(data);
                                    rentalInstrumentDetailsData.OCC = strNewOCC;

                                    //Add by Jutarat A. on 28092012
                                    rentalInstrumentDetailsData.InstrumentQty = (((data.InstrumentQty ?? 0) + (data.AdditionalInstrumentQty ?? 0)) - (data.RemovalInstrumentQty ?? 0));
                                    rentalInstrumentDetailsData.AdditionalInstrumentQty = 0;
                                    rentalInstrumentDetailsData.RemovalInstrumentQty = 0;
                                    //End Add

                                    dsRentalContract.dtTbt_RentalInstrumentDetails.Add(rentalInstrumentDetailsData);
                                }
                            //}
                        }

                        //dtTbt_RentalMaintenanceDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalMaintenanceDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalMaintenanceDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                            tbt_RentalMaintenanceDetails rentalMaintenanceDetailsData;
                            foreach (tbt_RentalMaintenanceDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalMaintenanceDetails)
                            {
                                rentalMaintenanceDetailsData = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(data);
                                rentalMaintenanceDetailsData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalMaintenanceDetails.Add(rentalMaintenanceDetailsData);
                            }
                        }

                        //dtTbt_RentalOperationType
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalOperationType != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalOperationType.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                            tbt_RentalOperationType rentalOperationTypeData;
                            foreach (tbt_RentalOperationType data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalOperationType)
                            {
                                rentalOperationTypeData = CommonUtil.CloneObject<tbt_RentalOperationType, tbt_RentalOperationType>(data);
                                rentalOperationTypeData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalOperationType.Add(rentalOperationTypeData);
                            }
                        }

                        //dtTbt_RentalSentryGuard
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuard != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuard.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>();
                            tbt_RentalSentryGuard rentalSentryGuardData;
                            foreach (tbt_RentalSentryGuard data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuard)
                            {
                                rentalSentryGuardData = CommonUtil.CloneObject<tbt_RentalSentryGuard, tbt_RentalSentryGuard>(data);
                                rentalSentryGuardData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalSentryGuard.Add(rentalSentryGuardData);
                            }
                        }

                        //dtTbt_RentalSentryGuardDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuardDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuardDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                            tbt_RentalSentryGuardDetails rentalSentryGuardDetailsData;
                            foreach (tbt_RentalSentryGuardDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuardDetails)
                            {
                                rentalSentryGuardDetailsData = CommonUtil.CloneObject<tbt_RentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(data);
                                rentalSentryGuardDetailsData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalSentryGuardDetails.Add(rentalSentryGuardDetailsData);
                            }
                        }

                        //dtTbt_RelationType
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RelationType != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RelationType.Count > 0)
                        {
                            dsRentalContract.dtTbt_RelationType = new List<tbt_RelationType>();
                            tbt_RelationType relationTypeData;
                            foreach (tbt_RelationType data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RelationType)
                            {
                                relationTypeData = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(data);
                                relationTypeData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RelationType.Add(relationTypeData);
                            }
                        }

                        /*---------------------------*/

                        //Save contract data
                        //dsRentalContractData dsRentalContractResult = rentralHandler.InsertEntireContract(dsRentalContract);
                        rentralHandler.InsertEntireContractForCTS010(dsRentalContract);

                   

                        //Send billing data
                        billingHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                        billingHandler.SendBilling_StartService(registerStartResumeTargetData.InitialData.ContractCode, doStartResumeService.StartResumeOperationDate.Value, doStartResumeService.AdjustBillingTerm, null);

                        //Lock quotation
                        quotHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                        bool blnLockQuotationResult = quotHandler.LockQuotation(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode
                                                    , null
                                                    , LockStyle.C_LOCK_STYLE_ALL);
                    }
                    /*--------------------------*/

                    scope.Complete();
                    res.ResultData = new object[] { MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046), sParam.ContractCode };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save data when StartType as SaveResume
        /// </summary>
        /// <param name="res"></param>
        /// <param name="doStartResumeService"></param>
        private void SaveResume_CTS070(ObjectResultData res, CTS070_doStartResumeService doStartResumeService)
        {
            CTS070_RegisterStartResumeTargetData registerStartResumeTargetData;
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;
            IBillingInterfaceHandler billingHandler;
            IQuotationHandler quotHandler;

            try
            {
                CTS070_ScreenParameter sParam = GetScreenObject<CTS070_ScreenParameter>();

                using (TransactionScope scope = new TransactionScope())
                {
                    /*--- RegisterStopService ---*/
                    registerStartResumeTargetData = sParam.CTS070_Session;
                    if (registerStartResumeTargetData.RegisterStartResumeData != null)
                    {
                        dsRentalContract = new dsRentalContractData();

                        //Generate security occurrence
                        rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        string strNewOCC = rentralHandler.GenerateContractOCC(registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode, true);

                        //Prepare data for saving
                        /*--- PrepareForSaveResume ---*/
                        //dtTbt_RentalContractBasic
                        dsRentalContract.dtTbt_RentalContractBasic = new List<tbt_RentalContractBasic>();

                        tbt_RentalContractBasic rentalContractBasic = CommonUtil.CloneObject<tbt_RentalContractBasic, tbt_RentalContractBasic>(registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalContractBasic[0]);
                        rentalContractBasic.ContractCode = registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode;
                        rentalContractBasic.LastOCC = strNewOCC;
                        rentalContractBasic.LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_RESUME;
                        rentalContractBasic.ContractStatus = ContractStatus.C_CONTRACT_STATUS_AFTER_START;
                        rentalContractBasic.StartType = rentalContractBasic.StartType == StartType.C_START_TYPE_ALTER_START ? StartType.C_START_TYPE_ALTER_START : StartType.C_START_TYPE_RESUME;
                        rentalContractBasic.StartResumeProcessDate = DateTime.Now.Date;
                        rentalContractBasic.UserCode = doStartResumeService.UserCode;
                        rentalContractBasic.LastChangeImplementDate = doStartResumeService.StartResumeOperationDate;
                        dsRentalContract.dtTbt_RentalContractBasic.Add(rentalContractBasic);

                        //dtTbt_RentalSecurityBasic
                        dsRentalContract.dtTbt_RentalSecurityBasic = new List<tbt_RentalSecurityBasic>();

                        tbt_RentalSecurityBasic rentalSecurityBasicData = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSecurityBasic[0]);
                        rentalSecurityBasicData.ContractCode = registerStartResumeTargetData.InitialData.RentalContractBasicData.ContractCode;
                        rentalSecurityBasicData.OCC = strNewOCC;
                        rentalSecurityBasicData.ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_RESUME;
                        rentalSecurityBasicData.ImplementFlag = FlagType.C_FLAG_ON;
                        // 2012-10-04 Add by Phoomsak L. CT-266
                        rentalSecurityBasicData.QuotationTargetCode = null;
                        rentalSecurityBasicData.QuotationAlphabet = null;
                        rentalSecurityBasicData.SalesmanEmpNo1 = null;
                        rentalSecurityBasicData.SalesmanEmpNo2 = null;
                        rentalSecurityBasicData.SalesSupporterEmpNo = null;
                        rentalSecurityBasicData.PlanCode = null;
                        rentalSecurityBasicData.InstallationCompleteFlag = null;
                        rentalSecurityBasicData.PlannerEmpNo = null;
                        rentalSecurityBasicData.PlanCheckerEmpNo = null;
                        rentalSecurityBasicData.PlanCheckDate = null;
                        rentalSecurityBasicData.PlanApproverEmpNo = null;
                        rentalSecurityBasicData.PlanApproveDate = null;
                        // -------------------------------------
                        rentalSecurityBasicData.ChangeImplementDate = doStartResumeService.StartResumeOperationDate;
                        rentalSecurityBasicData.ApproveNo1 = null;
                        rentalSecurityBasicData.ApproveNo2 = null;
                        rentalSecurityBasicData.ApproveNo3 = null;
                        rentalSecurityBasicData.ApproveNo4 = null;
                        rentalSecurityBasicData.ApproveNo5 = null;
                        rentalSecurityBasicData.CounterNo = 0;
                        rentalSecurityBasicData.ChangeReasonType = null;
                        rentalSecurityBasicData.ChangeNameReasonType = null;
                        rentalSecurityBasicData.StopCancelReasonType = null;
                        rentalSecurityBasicData.UninstallType = null;
                        rentalSecurityBasicData.ContractDocPrintFlag = null;
                        rentalSecurityBasicData.ExpectedInstallationCompleteDate = null;
                        rentalSecurityBasicData.InstallationCompleteFlag = null;
                        rentalSecurityBasicData.InstallationSlipNo = null;
                        rentalSecurityBasicData.InstallationCompleteDate = null;
                        rentalSecurityBasicData.InstallationTypeCode = null;
                        rentalSecurityBasicData.NegotiationStaffEmpNo1 = null;
                        rentalSecurityBasicData.NegotiationStaffEmpNo2 = null;
                        rentalSecurityBasicData.NormalInstallFee = null;
                        rentalSecurityBasicData.OrderInstallFee = null;
                        rentalSecurityBasicData.OrderInstallFee_ApproveContract = null;
                        rentalSecurityBasicData.OrderInstallFee_CompleteInstall = null;
                        rentalSecurityBasicData.OrderInstallFee_StartService = null;
                        //rentalSecurityBasicData.InstallFeePaidBySECOM = 0;
                        //rentalSecurityBasicData.InstallFeeRevenueBySECOM = 0;
                        // 2012-10-04 Add by Phoomsak L. CT-266
                        //rentalSecurityBasicData.AdditionalFee1 = 0;
                        //rentalSecurityBasicData.AdditionalFee2 = 0;
                        //rentalSecurityBasicData.AdditionalFee3 = 0;
                        //rentalSecurityBasicData.AdditionalApproveNo1 = null;
                        //rentalSecurityBasicData.AdditionalApproveNo2 = null;
                        //rentalSecurityBasicData.AdditionalApproveNo3 = null;
                        //rentalSecurityBasicData.MaintenanceFee1 = 0;
                        //rentalSecurityBasicData.MaintenanceFee2 = 0;
                        rentalSecurityBasicData.CompleteChangeOperationDate = null;
                        rentalSecurityBasicData.CompleteChangeOperationEmpNo = null;
                        //For remaining issue list no 128
                        rentalSecurityBasicData.ContractFeeOnStop = null;
                        //
                        rentalSecurityBasicData.ExpectedResumeDate = null; //Add by Jutarat A. on 02122013
                        dsRentalContract.dtTbt_RentalSecurityBasic.Add(rentalSecurityBasicData);

                        //dtTbt_RentalBEDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalBEDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalBEDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>();
                            tbt_RentalBEDetails rentalBEDetailsData;
                            foreach (tbt_RentalBEDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalBEDetails)
                            {
                                rentalBEDetailsData = CommonUtil.CloneObject<tbt_RentalBEDetails, tbt_RentalBEDetails>(data);
                                rentalBEDetailsData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalBEDetails.Add(rentalBEDetailsData);
                            }
                        }

                        //dtTbt_RentalInstrumentDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalInstrumentDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalInstrumentDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                            tbt_RentalInstrumentDetails rentalInstrumentDetailsData;
                            foreach (tbt_RentalInstrumentDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalInstrumentDetails)
                            {
                                rentalInstrumentDetailsData = CommonUtil.CloneObject<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(data);
                                rentalInstrumentDetailsData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalInstrumentDetails.Add(rentalInstrumentDetailsData);
                            }
                        }

                        //dtTbt_RentalMaintenanceDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalMaintenanceDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalMaintenanceDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                            tbt_RentalMaintenanceDetails rentalMaintenanceDetailsData;
                            foreach (tbt_RentalMaintenanceDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalMaintenanceDetails)
                            {
                                rentalMaintenanceDetailsData = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(data);
                                rentalMaintenanceDetailsData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalMaintenanceDetails.Add(rentalMaintenanceDetailsData);
                            }
                        }

                        //dtTbt_RentalOperationType
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalOperationType != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalOperationType.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                            tbt_RentalOperationType rentalOperationTypeData;
                            foreach (tbt_RentalOperationType data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalOperationType)
                            {
                                rentalOperationTypeData = CommonUtil.CloneObject<tbt_RentalOperationType, tbt_RentalOperationType>(data);
                                rentalOperationTypeData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalOperationType.Add(rentalOperationTypeData);
                            }
                        }

                        //dtTbt_RentalSentryGuard
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuard != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuard.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>();
                            tbt_RentalSentryGuard rentalSentryGuardData;
                            foreach (tbt_RentalSentryGuard data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuard)
                            {
                                rentalSentryGuardData = CommonUtil.CloneObject<tbt_RentalSentryGuard, tbt_RentalSentryGuard>(data);
                                rentalSentryGuardData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalSentryGuard.Add(rentalSentryGuardData);
                            }
                        }

                        //dtTbt_RentalSentryGuardDetails
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuardDetails != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuardDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                            tbt_RentalSentryGuardDetails rentalSentryGuardDetailsData;
                            foreach (tbt_RentalSentryGuardDetails data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RentalSentryGuardDetails)
                            {
                                rentalSentryGuardDetailsData = CommonUtil.CloneObject<tbt_RentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(data);
                                rentalSentryGuardDetailsData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RentalSentryGuardDetails.Add(rentalSentryGuardDetailsData);
                            }
                        }

                        //dtTbt_RelationType
                        if (registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RelationType != null && registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RelationType.Count > 0)
                        {
                            dsRentalContract.dtTbt_RelationType = new List<tbt_RelationType>();
                            tbt_RelationType relationTypeData;
                            foreach (tbt_RelationType data in registerStartResumeTargetData.RegisterStartResumeData.dtTbt_RelationType)
                            {
                                relationTypeData = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(data);
                                relationTypeData.OCC = strNewOCC;
                                dsRentalContract.dtTbt_RelationType.Add(relationTypeData);
                            }
                        }

                        /*---------------------------*/

                        //Save contract data
                        //dsRentalContractData dsRentalContractResult = rentralHandler.InsertEntireContract(dsRentalContract);
                        rentralHandler.InsertEntireContractForCTS010(dsRentalContract);


                        //Send billing data
                        billingHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                        billingHandler.SendBilling_ResumeService(registerStartResumeTargetData.InitialData.ContractCode, doStartResumeService.StartResumeOperationDate.Value);

                        //Lock quotation
                        quotHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                        bool blnLockQuotationResult = quotHandler.LockQuotation(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode
                                                    , null
                                                    , LockStyle.C_LOCK_STYLE_ALL);
                    }
                    /*--------------------------*/

                    scope.Complete();
                    res.ResultData = new object[] { MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046), sParam.ContractCode };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get MiscType of StartType
        /// </summary>
        /// <returns></returns>
        private List<doMiscTypeCode> GetStartTypeMiscType_CTS070()
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
            {
                new doMiscTypeCode()
                {
                    FieldName = MiscType.C_START_TYPE,
                    ValueCode = "%"
                }
            };

            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            lst = hand.GetMiscTypeCodeList(miscs);

            return lst;
        }

        #endregion

    }
}
