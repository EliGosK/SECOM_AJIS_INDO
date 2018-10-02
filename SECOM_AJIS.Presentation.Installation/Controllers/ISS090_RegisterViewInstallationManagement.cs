
//*********************************
// Create by: Teerapong
// Create date: 2/Nov/2011
// Update date: 2/Nov/2011
//*********************************



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Transactions;
using SECOM_AJIS.Presentation.Installation.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Helpers;

using System.ComponentModel;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using SECOM_AJIS.Common.Models.EmailTemplates;

namespace SECOM_AJIS.Presentation.Installation.Controllers
{
    public partial class InstallationController : BaseController
    {
        /// <summary>
        /// Authority screen ISS090
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ISS090_Authority(ISS090_ScreenParameter param)
        {
            // permission

            ObjectResultData res = new ObjectResultData();
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IInstallationHandler ihandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            if (chandler.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_MANAGE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            param.dtInstallationManagement = null;
            List<tbt_InstallationManagement> tmpdoInstallationManagement = ihandler.GetTbt_InstallationManagementData(param.MaintenanceNo);
            if (tmpdoInstallationManagement != null)
            {
                param.dtInstallationManagement = new tbt_InstallationManagement();
                param.dtInstallationManagement = tmpdoInstallationManagement[0];
            }
            else
            {
                res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5016);
                return Json(res);
            }

            param.ListPOInfo = ihandler.GetTbt_InstallationPOManagementData(param.MaintenanceNo);
            //if (param.ListPOInfo == null)
            //{           
            //    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5017);
            //    return Json(res);
            //}
            //foreach(tbt_InstallationPOManagement obj in param.ListPOInfo)
            //{
            //    obj.CheckChargeIEEmpName = ISS090_getEmployeeDisplayName(obj.CheckChargeIEEmpNo);
            //}
            CommonUtil comUtil = new CommonUtil();

            //param.ContractProjectCodeShort = comUtil.ConvertContractCode(param.dtInstallationManagement.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            IProjectHandler ProjectHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            dtProjectForInstall dtProjectDataForInstall = ProjectHandler.GetProjectDataForInstall(param.dtInstallationManagement.ContractProjectCode);
            if (dtProjectDataForInstall != null)
            {
                param.ContractProjectCodeShort = comUtil.ConvertProjectCode(param.dtInstallationManagement.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_PROJECT;
                res = ISS090_ValidateProjectError(param.dtInstallationManagement.ContractProjectCode);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
            }
            else
            {
                IRentralContractHandler RentalContactHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                dtRentalContractBasicForInstall dtRentalContractBasic = RentalContactHandler.GetRentalContractBasicDataForInstall(param.dtInstallationManagement.ContractProjectCode, MiscType.C_BUILDING_TYPE);
                dtSaleBasic dtSaleContractBasic = null;
                if (dtRentalContractBasic != null)
                {
                    param.ContractProjectCodeShort = comUtil.ConvertContractCode(param.dtInstallationManagement.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                    param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                    dtSaleContractBasic = null;
                    dtProjectDataForInstall = null;
                    res = ISS090_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, param.ServiceTypeCode);
                    if (res.IsError)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }
                else
                {
                    ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                    dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(param.dtInstallationManagement.ContractProjectCode, MiscType.C_BUILDING_TYPE);
                    if (dtSaleContractBasic != null)
                    {
                        param.ContractProjectCodeShort = comUtil.ConvertContractCode(param.dtInstallationManagement.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                        dtRentalContractBasic = null;
                        dtProjectDataForInstall = null;
                        res = ISS090_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, param.ServiceTypeCode);

                        if (res.IsError)
                        {
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }
                    }
                }
            }



            //======================== 19/03/2012 Change DDS =============================
            //CommonUtil comUtil = new CommonUtil();
            //string strProjectCodeLong = comUtil.ConvertProjectCode(param.strContractProjectCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            //string strContractCodeLong = comUtil.ConvertContractCode(param.strContractProjectCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            //if (CommonUtil.IsNullOrEmpty(ihandler.CheckInstallationDataToOpenScreenData(strProjectCodeLong)) && CommonUtil.IsNullOrEmpty(ihandler.CheckInstallationDataToOpenScreenData(strContractCodeLong)))
            //{
            //    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5034);
            //    return Json(res);
            //}

            //============================================================================

            // parameter
            //ISS090_ScreenParameter param = new ISS090_ScreenParameter();
            //param.ContractProjectCodeShort = param.strContractProjectCode;
            UpdateScreenObject(param);
            //param = new ISS090_ScreenParameter()
            //{
            //    ContractCodeProjectCode = strContractProjectCode,
            //    ServiceTypeCode = strServiceTypeCode
            //};

            ////  ?? data
            //param.ServiceTypeCode = "TEST";
            return InitialScreenEnvironment<ISS090_ScreenParameter>("ISS090", param, res);

        }
        /// <summary>
        /// Initial screen ISS090
        /// </summary>
        /// <returns></returns>
        [Initialize("ISS090")]
        public ActionResult ISS090()
        {
            //ISS090_ScreenParameter param = new ISS090_ScreenParameter();
            ISS090_ScreenParameter param = GetScreenObject<ISS090_ScreenParameter>();
            if (param != null)
            {
                ViewBag.ContractProjectCode = param.ContractProjectCodeShort;
            }
            ViewBag.AttachKey = GetCurrentKey();
            return View();
        }

        // InitialGridEmail
        /// <summary>
        /// Initial grid email schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS090_InitialGridEmail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS060_Email", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        // InitialGridEmail
        /// <summary>
        /// Initial grid PO information schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS090_InitialGridPOInfo()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS090_POInformation", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        // InitialGridManagement
        /// <summary>
        /// Initial grid management information
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS090_InitialGridManagementInfo()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS090_ManagementInformation", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        //InitialGridAttachedFile

        /// <summary>
        /// Retrieve data to show in screen
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public ActionResult ISS090_RetrieveData(string strContractCode)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            dtRentalContractBasicForInstall dtRentalContractBasic = new dtRentalContractBasicForInstall();
            dtSaleBasic dtSaleContractBasic = new dtSaleBasic();
            string lang = CommonUtil.GetCurrentLanguage();


            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                sParam.ContractProjectCodeShort = strContractCode;
                if (sParam.ISS090_Session == null)
                {
                    sParam.ISS090_Session = new ISS090_RegisterStartResumeTargetData();
                    sParam.ISS090_Session.dtRentalContractBasic = new dtRentalContractBasicForInstall();
                }
                if (sParam.MaintenanceNo != null)
                {
                    chandler.ClearTemporaryUploadFile(sParam.MaintenanceNo);
                }
                //Check mandatory
                if (String.IsNullOrEmpty(strContractCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5057, new string[] { "ContractCode" });
                    return Json(res);
                }

                //Get rental contract data

                //=============== RETRIEVE PROJECT DATA ==================
                string strProjectCodeLong = comUtil.ConvertProjectCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                IProjectHandler ProjectHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                dtProjectForInstall dtProjectDataForInstall = ProjectHandler.GetProjectDataForInstall(strProjectCodeLong);
                List<ContractCodeList> doContractCodeListByProject = null;
                //========================================================
                if (dtProjectDataForInstall != null)
                {
                    sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_PROJECT;

                    sParam.ISS090_Session.dtProject = dtProjectDataForInstall;

                    sParam.ContractProjectCodeLong = strProjectCodeLong;

                    doContractCodeListByProject = handler.GetInstallationBasicContractByProject(strProjectCodeLong);

                    dtSaleContractBasic = null;
                    dtRentalContractBasic = null;
                    res = ISS090_ValidateProjectError(sParam.ContractProjectCodeLong);
                    if (res.IsError)
                        return Json(res);
                }
                else
                {
                    string strContractCodeLong = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);


                    sParam.ContractProjectCodeLong = strContractCodeLong;
                    //dsRentalContract = CheckDataAuthority_ISS090(res, strContractCodeLong);
                    //if (res.IsError)
                    //    return Json(res);

                    //userCtrlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                    //doRentalContractBasic = userCtrlHandler.GetRentalContactBasicInformationData(strContractCodeLong);

                    IRentralContractHandler RentalContactHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    dtRentalContractBasic = RentalContactHandler.GetRentalContractBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);


                    //tbt_RentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];

                    //Validate business
                    //ValidateBusiness_CTS070(res, strContractCodeLong, tbt_RentalContractBasicData);
                    if (res.IsError)
                        return Json(res);

                    if (dtRentalContractBasic != null)
                    {
                        sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                        sParam.InstallType = MiscType.C_RENTAL_INSTALL_TYPE;
                        sParam.ISS090_Session.dtRentalContractBasic = dtRentalContractBasic;
                        sParam.dtRentalContractBasic = new dtRentalContractBasicForInstall();
                        sParam.dtRentalContractBasic = dtRentalContractBasic;
                        if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                        {
                            dtRentalContractBasic.OperationOfficeName = dtRentalContractBasic.OperationOfficeNameEN;
                            dtRentalContractBasic.ProductName = dtRentalContractBasic.ProductNameEN;
                        }
                        else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_JP)
                        {
                            dtRentalContractBasic.OperationOfficeName = dtRentalContractBasic.OperationOfficeNameJP;
                            dtRentalContractBasic.ProductName = dtRentalContractBasic.ProductNameJP;
                        }
                        else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_LC)
                        {
                            dtRentalContractBasic.OperationOfficeName = dtRentalContractBasic.OperationOfficeNameLC;
                            dtRentalContractBasic.ProductName = dtRentalContractBasic.ProductNameLC;
                        }

                        dtSaleContractBasic = null;
                        dtProjectDataForInstall = null;
                        res = ISS090_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, sParam.ServiceTypeCode);
                        if (res.IsError)
                            return Json(res);
                    }
                    else
                    {
                        ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);

                        if (dtSaleContractBasic != null)
                        {

                            sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                            sParam.InstallType = MiscType.C_SALE_INSTALL_TYPE;
                            sParam.ISS090_Session.dtSale = dtSaleContractBasic;

                            sParam.dtSaleContractBasic = new dtSaleBasic();
                            sParam.dtSaleContractBasic = dtSaleContractBasic;
                            if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                            {
                                dtSaleContractBasic.OperationOfficeName = dtSaleContractBasic.OperationOfficeNameEN;
                                dtSaleContractBasic.ProductName = dtSaleContractBasic.ProductNameEN;
                            }
                            else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_JP)
                            {
                                dtSaleContractBasic.OperationOfficeName = dtSaleContractBasic.OperationOfficeNameJP;
                                dtSaleContractBasic.ProductName = dtSaleContractBasic.ProductNameJP;
                            }
                            else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_LC)
                            {
                                dtSaleContractBasic.OperationOfficeName = dtSaleContractBasic.OperationOfficeNameLC;
                                dtSaleContractBasic.ProductName = dtSaleContractBasic.ProductNameLC;
                            }

                            dtRentalContractBasic = null;
                            dtProjectDataForInstall = null;
                            res = ISS090_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, sParam.ServiceTypeCode);
                            if (res.IsError)
                                return Json(res);
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5018, new string[] { "ContractCode" });
                            return Json(res);
                        }
                    }


                }

                //************************************* GET DATA doIB doIM Email PO ************************************************

                //sParam.dtInstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractProjectCodeLong);
                sParam.dtInstallationBasic = null;

                //==================== Change req to get data from history before basic ====================
                //List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractProjectCodeLong);
                //if (tmpdoTbt_InstallationBasic != null)
                //{
                //    sParam.dtInstallationBasic = new tbt_InstallationBasic();
                //    sParam.dtInstallationBasic = tmpdoTbt_InstallationBasic[0];
                //}
                bool blnHavetbtInstallationBasic = false;
                List<tbt_InstallationHistory> tmpInstallationHistory = handler.GetTbt_InstallationHistory(null, sParam.MaintenanceNo, null);
                if (tmpInstallationHistory != null && tmpInstallationHistory.Count > 0)
                {
                    tbt_InstallationBasic tmpTbt_InstallationBasic = CommonUtil.CloneObject<tbt_InstallationHistory, tbt_InstallationBasic>(tmpInstallationHistory[0]);
                    sParam.dtInstallationBasic = tmpTbt_InstallationBasic;
                }
                else
                {
                    List<tbt_InstallationBasic> tmpdoTbt_InstallationBasicList = handler.GetTbt_InstallationBasicData(sParam.ContractProjectCodeLong);
                    if (tmpdoTbt_InstallationBasicList != null)
                    {
                        sParam.dtInstallationBasic = tmpdoTbt_InstallationBasicList[0];
                        blnHavetbtInstallationBasic = true;
                    }
                }
                //==========================================================================================

                //if (sParam.dtInstallationBasic == null)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5061);
                //    return Json(res);
                //}
                //sParam.MaintenanceNo = sParam.dtInstallationBasic.MaintenanceNo;
                if (sParam.dtInstallationBasic != null)
                {
                    if (sParam.dtInstallationBasic.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    {
                        sParam.dtInstallationBasic.InstallationTypeName = chandler.GetMiscDisplayValue(MiscType.C_RENTAL_INSTALL_TYPE, sParam.dtInstallationBasic.InstallationType);

                        if (!CommonUtil.IsNullOrEmpty(sParam.dtInstallationBasic.InstallationType) && !CommonUtil.IsNullOrEmpty(sParam.dtInstallationBasic.InstallationTypeName))
                            sParam.dtInstallationBasic.InstallationTypeName = sParam.dtInstallationBasic.InstallationType + " : " + sParam.dtInstallationBasic.InstallationTypeName;
                    }
                    else if (sParam.dtInstallationBasic.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {

                        sParam.dtInstallationBasic.InstallationTypeName = chandler.GetMiscDisplayValue(MiscType.C_SALE_INSTALL_TYPE, sParam.dtInstallationBasic.InstallationType);

                        if (!CommonUtil.IsNullOrEmpty(sParam.dtInstallationBasic.InstallationType) && !CommonUtil.IsNullOrEmpty(sParam.dtInstallationBasic.InstallationTypeName))
                            sParam.dtInstallationBasic.InstallationTypeName = sParam.dtInstallationBasic.InstallationType + " : " + sParam.dtInstallationBasic.InstallationTypeName;
                    }
                }
                //sParam.dtInstallationManagement = handler.GetTbt_InstallationManagementData(sParam.dtInstallationBasic.MaintenanceNo);
                sParam.dtInstallationManagement = null;
                List<tbt_InstallationManagement> tmpdtInstallationManagement = handler.GetTbt_InstallationManagementData(sParam.MaintenanceNo);
                if (tmpdtInstallationManagement != null)
                {
                    sParam.dtInstallationManagement = new tbt_InstallationManagement();
                    sParam.dtInstallationManagement = tmpdtInstallationManagement[0];
                }

                if (sParam.dtInstallationManagement == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5061);
                    return Json(res);
                }

                sParam.ListPOInfo = handler.GetTbt_InstallationPOManagementData(sParam.MaintenanceNo);

                ////=========================== Map Subcontractor Name =========================
                //if (sParam.ListPOInfo != null)
                //{
                //    sParam.arrayPOName = new string[sParam.ListPOInfo.Count];
                //    ICommonContractHandler hand = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                //    List<tbm_SubContractor> lstSubconName = hand.GetTbm_SubContractorData(null);
                //    foreach (tbt_InstallationPOManagement dataPO in sParam.ListPOInfo)
                //    {
                //        sParam.arrayPOName[sParam.ListPOInfo.FindIndex(delegate(tbt_InstallationPOManagement s) { return s.SubcontractorCode == dataPO.SubcontractorCode; })] = lstSubconName[lstSubconName.FindIndex(delegate(tbm_SubContractor s) { return s.SubContractorCode == dataPO.SubcontractorCode; })].SubContractorName;
                //        if (!CommonUtil.IsNullOrEmpty(dataPO.CheckChargeIEEmpNo))
                //            dataPO.CheckChargeIEEmpName = ISS090_getEmployeeDisplayName(dataPO.CheckChargeIEEmpNo);
                //    }
                //}
                if (sParam.ListPOInfo != null)
                {
                    ICommonContractHandler hand = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                    List<tbm_SubContractor> lstSubconName = hand.GetTbm_SubContractorData(null);

                    ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                    foreach (tbt_InstallationPOManagement dataPO in sParam.ListPOInfo)
                    {
                        //Check Amount Currency, Set TextCurrency
                        if (dataPO.ActualPOAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            dataPO.ActualPOAmount = dataPO.ActualPOAmountUsd;
                            dataPO.TxtCurrencyActualPOAmount = tmpCurrencies[1].ValueDisplayEN;
                        }
                        else
                        {
                            dataPO.TxtCurrencyActualPOAmount = tmpCurrencies[0].ValueDisplayEN;
                        }

                        if (dataPO.NormalSubPOAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            dataPO.NormalSubPOAmount = dataPO.NormalSubPOAmountUsd;
                            dataPO.TxtCurrencySubPOAmount = tmpCurrencies[1].ValueDisplayEN;
                        }
                        else
                        {
                            dataPO.TxtCurrencySubPOAmount = tmpCurrencies[0].ValueDisplayEN;
                        }

                        if (dataPO.LastInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            dataPO.LastInstallationFee = dataPO.LastInstallationFeeUsd;
                            dataPO.TxtCurrencyLastInstallationFee = tmpCurrencies[1].ValueDisplayEN;
                        }
                        else
                        {
                            dataPO.TxtCurrencyLastInstallationFee = tmpCurrencies[0].ValueDisplayEN;
                        }

                        //Check Amount is NULL
                        if (dataPO.ActualPOAmount == null)
                        {
                            dataPO.ActualPOAmount = 0;
                        }
                        if (dataPO.NormalSubPOAmount == null)
                        {
                            dataPO.NormalSubPOAmount = 0;
                        }
                        if (dataPO.LastInstallationFee == null)
                        {
                            dataPO.LastInstallationFee = 0;
                        }

                        if (lstSubconName.FindIndex(delegate (tbm_SubContractor s) { return s.SubContractorCode.Trim() == dataPO.SubcontractorCode.Trim(); }) >= 0)
                        {
                            dataPO.SubContractorName = lstSubconName[lstSubconName.FindIndex(delegate (tbm_SubContractor s) { return s.SubContractorCode.Trim() == dataPO.SubcontractorCode.Trim(); })].SubContractorName;
                        }
                    }
                    //Mapping Employee
                    IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                    EmployeeMappingList empLst = new EmployeeMappingList();
                    empLst.AddEmployee(sParam.ListPOInfo.ToArray());
                    ehandler.EmployeeListMapping(empLst);
                    ////============================================================================
                }




                List<tbt_InstallationEmail> ListEmail = handler.GetTbt_InstallationEmailData(sParam.MaintenanceNo);
                /////////////////// RETRIEVE EMAIL ////////////////////////
                ISS090_DOEmailData TempEmail;
                List<ISS090_DOEmailData> listAddEmail = new List<ISS090_DOEmailData>();
                if (ListEmail != null)
                {
                    foreach (tbt_InstallationEmail dataEmail in ListEmail)
                    {
                        TempEmail = new ISS090_DOEmailData();
                        TempEmail.EmailAddress = dataEmail.EmailNoticeTarget;
                        listAddEmail.Add(TempEmail);
                    }
                }

                sParam.ListDOEmail = listAddEmail;

                //============================ Retrieve Approve Email ========================
                List<dtRequestApproveInstallation> ListEmail2 = handler.GetEmailForApprove();
                /////////////////// RETRIEVE EMAIL ////////////////////////

                List<ISS090_DOEmailData> listAddEmail2 = new List<ISS090_DOEmailData>();
                if (ListEmail2 != null)
                {
                    foreach (dtRequestApproveInstallation dataEmail in ListEmail2)
                    {
                        TempEmail = new ISS090_DOEmailData();
                        TempEmail.EmailAddress = dataEmail.EmailAddress;
                        listAddEmail2.Add(TempEmail);
                    }
                }

                sParam.ListApproveEmail = listAddEmail2;
                //============================================================================

                ////========================== Retrieve Memo ==============================
                List<tbt_InstallationMemo> ListMemo = handler.GetTbt_InstallationMemo(sParam.MaintenanceNo);
                if (sParam.dtInstallationBasic != null)
                {
                    if (sParam.dtInstallationBasic.SlipNo != null)
                    {
                        List<tbt_InstallationMemo> ListMemoSlip = handler.GetTbt_InstallationMemo(sParam.dtInstallationBasic.SlipNo);
                        if (ListMemoSlip != null)
                        {
                            if (ListMemo == null)
                            {
                                ListMemo = new List<tbt_InstallationMemo>();
                            }
                            ListMemo.AddRange(ListMemoSlip);
                        }
                    }
                }

                IEmployeeMasterHandler empHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                string strInstallationMemoHistory = "";
                if (ListMemo != null)
                {
                    ListMemo = ListMemo.OrderByDescending(x => ((DateTime)x.CreateDate).ToString("yyyyMMddHHmm")).ToList<tbt_InstallationMemo>();
                    //ListMemo.OrderBy(c => ((DateTime)c.CreateDate).Date).ThenBy(c => ((DateTime)c.CreateDate).TimeOfDay);
                    foreach (tbt_InstallationMemo Memo in ListMemo)
                    {
                        List<dtEmployee> emp = empHandler.GetEmployee(Memo.CreateBy, null, null);
                        List<tbm_Department> department = masterHandler.GetTbm_Department();
                        department = (from c in department where c.DepartmentCode == Memo.DepartmentCode select c).ToList<tbm_Department>();
                        List<tbm_Object> obj = masterHandler.GetTbm_Object();
                        obj = (from c in obj where c.ObjectID == Memo.ObjectID select c).ToList<tbm_Object>();
                        string EmpFirstName = "";
                        string EmpLastName = "";
                        string OfficeName = "";
                        string DepartmentName = "";
                        string ObjectName = "";
                        if (emp.Count > 0)
                        {
                            if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                            {
                                EmpFirstName = emp[0].EmpFirstNameEN;
                                EmpLastName = emp[0].EmpLastNameEN;
                            }
                            else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_JP)
                            {
                                EmpFirstName = emp[0].EmpFirstNameEN;
                                EmpLastName = emp[0].EmpLastNameEN;
                            }
                            else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_LC)
                            {
                                EmpFirstName = emp[0].EmpFirstNameLC;
                                EmpLastName = emp[0].EmpLastNameLC;
                            }
                        }
                        OfficeName = ISS090_GetOfficeNameByCode(Memo.OfficeCode);

                        if (department.Count > 0)
                        {
                            DepartmentName = department[0].DepartmentName;
                        }
                        if (obj.Count > 0)
                        {
                            if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                            {
                                ObjectName = obj[0].ObjectNameEN;
                            }
                            else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_JP)
                            {
                                ObjectName = obj[0].ObjectNameJP;
                            }
                            else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_LC)
                            {
                                ObjectName = obj[0].ObjectNameLC;
                            }

                        }

                        strInstallationMemoHistory = strInstallationMemoHistory + ((DateTime)Memo.CreateDate).ToString("dd-MMM-yyyy HH:mm") + " : " + EmpFirstName + " " + EmpLastName + " : " + OfficeName + " : " + DepartmentName + " : " + ObjectName + "\r\n" + Memo.Memo + " \r\n\r\n";


                    }
                }
                ////=======================================================================

                //********************************************************************************************************************

                //Add by Jutarat A. on 31072013
                decimal decNormalInstallationFee = 0;
                decimal decBillingInstallationFee = 0;
                if (sParam.dtInstallationBasic != null)
                {
                    tbt_InstallationSlip installSlip = handler.GetTbt_InstallationSlipData(sParam.dtInstallationBasic.SlipNo);
                    if (installSlip != null)
                    {
                        decNormalInstallationFee = installSlip.NormalInstallFee ?? 0;
                        decBillingInstallationFee = installSlip.OrderInstallFee ?? 0;
                    }
                    //Add by Jutarat A. on 02082013
                    else
                    {
                        IRentralContractHandler rentalHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        List<tbt_RentalSecurityBasic> rentalSecurityBasicList = rentalHandler.GetTbt_RentalSecurityBasic(sParam.dtInstallationBasic.ContractProjectCode, sParam.dtInstallationBasic.OCC);
                        if (rentalSecurityBasicList != null && rentalSecurityBasicList.Count > 0)
                        {
                            decNormalInstallationFee = rentalSecurityBasicList[0].NormalInstallFee ?? 0;
                            decBillingInstallationFee = rentalSecurityBasicList[0].OrderInstallFee ?? 0;
                        }
                        else
                        {
                            ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                            List<tbt_SaleBasic> saleBasicList = saleHandler.GetTbt_SaleBasic(sParam.dtInstallationBasic.ContractProjectCode, sParam.dtInstallationBasic.OCC, null);
                            if (saleBasicList != null && saleBasicList.Count > 0)
                            {
                                decNormalInstallationFee = saleBasicList[0].NormalInstallFee ?? 0;
                                decBillingInstallationFee = saleBasicList[0].OrderInstallFee ?? 0;
                            }
                        }
                    }
                    //End Add
                }
                //End Add

                if (res.IsError)
                    return Json(res);

                UpdateScreenObject(sParam);

                ISS090_RegisterStartResumeTargetData result = new ISS090_RegisterStartResumeTargetData();
                result.dtRentalContractBasic = dtRentalContractBasic;
                result.dtSale = dtSaleContractBasic;
                result.dtProject = dtProjectDataForInstall;
                result.ServiceTypeCode = sParam.ServiceTypeCode;
                result.InstallType = sParam.InstallType;
                result.dtInstallationBasic = sParam.dtInstallationBasic;
                result.dtInstallationManagement = sParam.dtInstallationManagement;
                result.ListDOEmail = sParam.ListDOEmail;
                result.ListPOInfo = sParam.ListPOInfo;
                result.arrayPOName = sParam.arrayPOName;
                result.strMemoHistory = strInstallationMemoHistory;
                result.ListApproveEmail = sParam.ListApproveEmail;
                result.MaintenanceNo = sParam.MaintenanceNo;
                result.doContractCodeListByProject = doContractCodeListByProject;
                result.blnHavetbtInstallationBasic = blnHavetbtInstallationBasic;

                result.NormalInstallationFee = decNormalInstallationFee; //Add by Jutarat A. on 31072013
                result.BillingInstallationFee = decBillingInstallationFee; //Add by Jutarat A. on 31072013

                if (sParam.dtInstallationManagement != null)
                {
                    result.strMode = ISS090_CheckScreenMode(sParam.dtInstallationManagement.ManagementStatus);
                }
                ViewBag.ReferKey = sParam.MaintenanceNo;
                res.ResultData = result;

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get email data
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        public ActionResult ISS090_GetInstallEmail(string strEmail)
        {
            List<dtGetEmailAddress> emailList = new List<dtGetEmailAddress>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                emailList = handler.GetEmailAddress(null, strEmail, null, null);

                if (emailList.Count == 0)
                {

                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { strEmail });
                    return Json(res);
                }
                else
                {
                    if (emailList.Count > 1)
                    {
                        emailList.RemoveRange(1, emailList.Count - 1);
                    }

                    // Keep select email to session (emailList  --> session)
                    List<ISS090_DOEmailData> listEmail;
                    ISS090_ScreenParameter session = GetScreenObject<ISS090_ScreenParameter>();
                    ISS090_DOEmailData doISS090EmailADD = new ISS090_DOEmailData();

                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS090_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    doISS090EmailADD.EmpNo = emailList[0].EmpNo;
                    doISS090EmailADD.EmailAddress = emailList[0].EmailAddress;

                    if (listEmail.FindAll(delegate (ISS090_DOEmailData s) { return s.EmpNo == doISS090EmailADD.EmpNo; }).Count() == 0)
                        listEmail.Add(doISS090EmailADD);
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0107);
                        return Json(res);
                    }
                    session.ListDOEmail = listEmail;
                    UpdateScreenObject(session);
                }


            }
            catch (Exception ex)
            {
                emailList = new List<dtGetEmailAddress>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = emailList;
            return Json(res);
        }
        /// <summary>
        /// Validate register data
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS090_ValidateRegisterData()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                //IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                //emailList = handler.GetEmailAddress(null, strEmail, null, null);

            }
            catch (Exception ex)
            {
                //emailList = new List<dtGetEmailAddress>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            //res.ResultData = emailList;
            return Json(res);
        }

        //public ActionResult ISS090_Validate(ISS090_RegisterData data)
        //{
        //    ObjectResultData res = new ObjectResultData();

        //    return Json(res);
        //}
        /// <summary>
        /// Validate email data
        /// </summary>
        /// <param name="doISS090Email"></param>
        /// <returns></returns>
        public ActionResult ValidateEmail_ISS090(ISS090_DOEmailData doISS090Email)
        {
            List<dtGetEmailAddress> dtEmail;
            ISS090_DOEmailData doISS090EmailADD;
            IEmployeeMasterHandler employeeMasterHandler;
            ObjectResultData res = new ObjectResultData();
            ISS090_ScreenParameter session;
            employeeMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            dtEmail = employeeMasterHandler.GetEmailAddress(doISS090Email.EmpFirstNameEN, doISS090Email.EmailAddress, doISS090Email.OfficeCode, doISS090Email.DepartmentCode);

            try
            {
                session = GetScreenObject<ISS090_ScreenParameter>();
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                    {
                        res.ResultData = false;
                        return Json(res);
                    }
                }


                //doISS090EmailADD = new ISS090_DOEmailData();
                //doISS090EmailADD.EmpNo = "540886";
                //doISS090EmailADD.EmailAddress = "Nattapong@csithai.com";
                //session.DOEmail = doISS090EmailADD;

                if (dtEmail != null)
                {
                    if (dtEmail.Count() == 0)
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, doISS090Email.EmailAddress);
                    else
                    {
                        doISS090EmailADD = new ISS090_DOEmailData();
                        doISS090EmailADD.EmpNo = dtEmail[0].EmpNo;
                        doISS090EmailADD.EmailAddress = doISS090Email.EmailAddress;
                        session.DOEmail = doISS090EmailADD;

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Json(res);
        }
        /// <summary>
        /// Upload attach
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS090_Upload()
        {
            ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
            ViewBag.ReferKey = sParam.MaintenanceNo;
            ViewBag.K = GetCurrentKey();
            return View();
        }



        /// <summary>
        /// Get data installation type for combobox
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS090_GetMiscInstallationtype(string strFieldName)
        {

            string strDisplayName = "ValueCodeDisplay";
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = strFieldName,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);


                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(lst, strDisplayName, "ValueCode");

                return Json(cboModel);
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
        /// <summary>
        /// Get email data and send to show in grid
        /// </summary>
        /// <param name="doCTS053Email"></param>
        /// <returns></returns>
        public ActionResult GetEmail_ISS090(ISS090_DOEmailData doCTS053Email)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            ISS090_DOEmailData doISS090EmailADD;
            List<ISS090_DOEmailData> listEmail;
            ISS090_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS090_ScreenParameter>();
                doISS090EmailADD = new ISS090_DOEmailData();
                if (session.ListDOEmail == null)
                    listEmail = new List<ISS090_DOEmailData>();
                else
                    listEmail = session.ListDOEmail;

                doISS090EmailADD.EmpNo = session.DOEmail.EmpNo;
                doISS090EmailADD.EmailAddress = doCTS053Email.EmailAddress;

                if (listEmail.FindAll(delegate (ISS090_DOEmailData s) { return s.EmpNo == doISS090EmailADD.EmpNo; }).Count() == 0)
                    listEmail.Add(doISS090EmailADD);

                session.DOEmail = null;
                session.ListDOEmail = listEmail;
                res.ResultData = CommonUtil.ConvertToXml<ISS090_DOEmailData>(session.ListDOEmail, "Contract\\CTS053Email", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //public ActionResult GetEmailList_ISS090(List<ISS090_DOEmailData> listEmailAdd)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    ObjectResultData resEmail = new ObjectResultData();
        //    List<ISS090_DOEmailData> listEmail;
        //    ISS090_Parameter session;

        //    try
        //    {
        //        if (listEmailAdd != null)
        //        {
        //            session = GetScreenObject<ISS090_Parameter>();
        //            if (session.ListDOEmail == null)
        //                listEmail = new List<ISS090_DOEmailData>();
        //            else
        //                listEmail = session.ListDOEmail;

        //            foreach (var item in listEmailAdd)
        //            {
        //                if (listEmail.FindAll(delegate(ISS090_DOEmailData s) { return s.EmpNo == item.EmpNo; }).Count() == 0)
        //                    listEmail.Add(item);
        //            }

        //            session.ListDOEmail = listEmail;
        //            res.ResultData = CommonUtil.ConvertToXml<ISS090_DOEmailData>(session.ListDOEmail, "Contract\\CTS053Email", CommonUtil.GRID_EMPTY_TYPE.INSERT);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}
        /// <summary>
        /// Get email data list
        /// </summary>
        /// <param name="listEmailAdd"></param>
        /// <returns></returns>
        public ActionResult GetEmailList_ISS090(List<ISS090_DOEmailData> listEmailAdd)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            List<ISS090_DOEmailData> listEmail;
            List<ISS090_DOEmailData> listNewEmail;
            ISS090_ScreenParameter session;

            try
            {
                listNewEmail = new List<ISS090_DOEmailData>();
                if (listEmailAdd != null)
                {
                    session = GetScreenObject<ISS090_ScreenParameter>();
                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS090_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    foreach (var item in listEmailAdd)
                    {
                        if (listEmail.FindAll(delegate (ISS090_DOEmailData s) { return s.EmailAddress == item.EmailAddress; }).Count() == 0)
                        {
                            listEmail.Add(item);
                            listNewEmail.Add(item);
                        }
                    }

                    session.ListDOEmail = listEmail;
                    res.ResultData = listNewEmail;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Remove email and send data to show in grid
        /// </summary>
        /// <param name="doISS090Email"></param>
        /// <returns></returns>
        public ActionResult RemoveMailClick_ISS090(ISS090_DOEmailData doISS090Email)
        {
            List<ISS090_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS090_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS090_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate (ISS090_DOEmailData s) { return s.EmailAddress == doISS090Email.EmailAddress; });
                if (listEmailDelete.Count() != 0)
                    session.ListDOEmail.Remove(listEmailDelete[0]);

                res.ResultData = CommonUtil.ConvertToXml<ISS090_DOEmailData>(session.ListDOEmail, "Installation\\ISS090Email", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); ;
            }

            return Json(res);
        }
        /// <summary>
        /// Remove email data
        /// </summary>
        /// <param name="doISS090Email"></param>
        /// <returns></returns>
        public ActionResult ISS090_RemoveMailClick(ISS090_DOEmailData doISS090Email)
        {
            List<ISS090_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS090_ScreenParameter session;
            try
            {
                session = GetScreenObject<ISS090_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate (ISS090_DOEmailData s) { return s.EmailAddress == doISS090Email.EmailAddress; });
                if (listEmailDelete.Count() != 0)
                    session.ListDOEmail.Remove(listEmailDelete[0]);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); ;
            }
            return Json(res);
        }
        /// <summary>
        /// Remove PO information
        /// </summary>
        /// <param name="doISS090POInfo"></param>
        /// <returns></returns>
        public ActionResult ISS090_RemovePOInfoClick(tbt_InstallationPOManagement doISS090POInfo)
        {
            List<tbt_InstallationPOManagement> listPOInfoDelete;
            ObjectResultData res = new ObjectResultData();
            ISS090_ScreenParameter session;
            try
            {
                session = GetScreenObject<ISS090_ScreenParameter>();
                listPOInfoDelete = session.ListPOInfo.FindAll(delegate (tbt_InstallationPOManagement s) { return s.PONo == doISS090POInfo.PONo; });
                if (listPOInfoDelete.Count() != 0)
                    session.ListPOInfo.Remove(listPOInfoDelete[0]);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); ;
            }
            return Json(res);
        }

        /// <summary>
        /// Register data
        /// </summary>
        /// <param name="dtManagement"></param>
        /// <param name="InstallationMemo"></param>
        /// <returns></returns>
        public ActionResult ISS090_RegisterData(tbt_InstallationManagement dtManagement, string InstallationMemo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;

            try
            {
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_MANAGE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();

                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //===================== UPDATE TBT_INSTALLATION MANAGEMENT ==================
                    sParam.dtInstallationManagement.IEManPower = dtManagement.IEManPower;
                    sParam.dtInstallationManagement.MaterialFee = dtManagement.MaterialFee;
                    sParam.dtInstallationManagement.ChangeReasonCode = dtManagement.ChangeReasonCode;
                    sParam.dtInstallationManagement.ChangeReasonOther = dtManagement.ChangeReasonOther;
                    sParam.dtInstallationManagement.ChangeRequestorCode = dtManagement.ChangeRequestorCode;
                    sParam.dtInstallationManagement.ChangeRequestorOther = dtManagement.ChangeRequestorOther;
                    sParam.dtInstallationManagement.ManagementStatus = InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_PROCESSING;
                    handler.UpdateTbt_InstallationManagement(sParam.dtInstallationManagement);
                    //============================================================================
                    //====================== UPDATE TBT_INSTALLATIONPOMANAGEMENT =================
                    if (sParam.ListPOInfo != null)
                    {
                        List<tbt_InstallationPOManagement> tempTbt_InstallationPOManagement = handler.GetTbt_InstallationPOManagementData(sParam.MaintenanceNo);
                        foreach (tbt_InstallationPOManagement dataPOManagement in sParam.ListPOInfo)
                        {
                            dataPOManagement.PaidFlag = FlagType.C_FLAG_OFF;
                            //dataPOManagement.IssuePOFlag = FlagType.C_FLAG_OFF;
                            //dataPOManagement.IssuePODate = null;
                            dataPOManagement.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            dataPOManagement.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                            List<tbt_InstallationPOManagement> tempPrepareUpdateData = (from c in tempTbt_InstallationPOManagement where c.SubcontractorCode.Trim() == dataPOManagement.SubcontractorCode.Trim() select c).ToList<tbt_InstallationPOManagement>();
                            if (tempPrepareUpdateData != null && tempPrepareUpdateData.Count > 0)
                            {
                                tempPrepareUpdateData[0].InstallStartDate = dataPOManagement.InstallStartDate;
                                tempPrepareUpdateData[0].InstallCompleteDate = dataPOManagement.InstallCompleteDate;
                                tempPrepareUpdateData[0].ExpectInstallStartDate = dataPOManagement.ExpectInstallStartDate;
                                tempPrepareUpdateData[0].ExpectInstallCompleteDate = dataPOManagement.ExpectInstallCompleteDate;
                                tempPrepareUpdateData[0].ManPower = dataPOManagement.ManPower;
                                tempPrepareUpdateData[0].ComplainCode = dataPOManagement.ComplainCode;
                                tempPrepareUpdateData[0].IEFirstCheckDate = dataPOManagement.IEFirstCheckDate;
                                tempPrepareUpdateData[0].IELastInspectionDate = dataPOManagement.IELastInspectionDate;
                                tempPrepareUpdateData[0].AdjustmentCode = dataPOManagement.AdjustmentCode;
                                tempPrepareUpdateData[0].AdjustmentContentCode = dataPOManagement.AdjustmentContentCode;
                                tempPrepareUpdateData[0].IEEvaluationCode = dataPOManagement.IEEvaluationCode;
                                tempPrepareUpdateData[0].LastInstallationFee = dataPOManagement.LastInstallationFee;
                                tempPrepareUpdateData[0].PaidFlag = dataPOManagement.PaidFlag;
                                tempPrepareUpdateData[0].CheckChargeIEEmpNo = dataPOManagement.CheckChargeIEEmpNo;
                                tempPrepareUpdateData[0].AdvancePaymentFlag = dataPOManagement.AdvancePaymentFlag;

                                //Add by Jutarat A. on 11102013
                                tempPrepareUpdateData[0].LastPaidDate = dataPOManagement.LastPaidDate;
                                tempPrepareUpdateData[0].IMFee = dataPOManagement.IMFee;
                                tempPrepareUpdateData[0].OtherFee = dataPOManagement.OtherFee;
                                tempPrepareUpdateData[0].IMRemark = dataPOManagement.IMRemark;
                                tempPrepareUpdateData[0].InstallationFee1 = dataPOManagement.InstallationFee1;
                                tempPrepareUpdateData[0].ApproveNo1 = dataPOManagement.ApproveNo1;
                                tempPrepareUpdateData[0].PaidDate1 = dataPOManagement.PaidDate1;
                                tempPrepareUpdateData[0].InstallationFee2 = dataPOManagement.InstallationFee2;
                                tempPrepareUpdateData[0].ApproveNo2 = dataPOManagement.ApproveNo2;
                                tempPrepareUpdateData[0].PaidDate2 = dataPOManagement.PaidDate2;
                                tempPrepareUpdateData[0].InstallationFee3 = dataPOManagement.InstallationFee3;
                                tempPrepareUpdateData[0].ApproveNo3 = dataPOManagement.ApproveNo3;
                                tempPrepareUpdateData[0].PaidDate3 = dataPOManagement.PaidDate3;
                                //End Add
                                tempPrepareUpdateData[0].NoCheckFlag = dataPOManagement.NoCheckFlag; //Add by Jutarat A. on 07112013
                                tempPrepareUpdateData[0].ActualPOAmount = dataPOManagement.ActualPOAmount; //Add by Jutarat A. on 07112013

                                handler.UpdateTbt_InstallationPOManagement(tempPrepareUpdateData[0]);
                            }
                            //handler.UpdateTbt_InstallationPOManagement(dataPOManagement);
                        }

                    }
                    //============================================================================

                    //================= Update Attach ======================
                    List<dtAttachFileForGridView> tmpFileList = chandler.GetAttachFileForGridView(sParam.MaintenanceNo);
                    if (tmpFileList != null && tmpFileList.Count > 0)
                    {
                        chandler.UpdateFlagAttachFile(AttachmentModule.Installation, sParam.MaintenanceNo, sParam.MaintenanceNo);
                        foreach (dtAttachFileForGridView file in tmpFileList)
                        {
                            int? attachID = (int?)Convert.ToInt32(file.AttachFileID);
                            List<tbt_InstallationAttachFile> InsAttachFileList = handler.GetTbt_InstallationAttachFile(attachID, sParam.MaintenanceNo, null);
                            if (InsAttachFileList == null)
                            {
                                tbt_InstallationAttachFile doTbt_InstallAttachFile = new tbt_InstallationAttachFile();
                                doTbt_InstallAttachFile.MaintenanceNo = sParam.MaintenanceNo;
                                doTbt_InstallAttachFile.AttachFileID = file.AttachFileID;
                                doTbt_InstallAttachFile.ObjectID = ScreenID.C_SCREEN_ID_INSTALL_PO;
                                handler.InsertTbt_InstallationAttachFile(doTbt_InstallAttachFile);
                            }
                        }
                    }

                    /////////////////// INSERT MEMO ////////////////////////
                    if (!CommonUtil.IsNullOrEmpty(InstallationMemo))
                    {
                        tbt_InstallationMemo doMemo = new tbt_InstallationMemo();
                        doMemo.ContractProjectCode = sParam.ContractProjectCodeLong;
                        doMemo.ReferenceID = sParam.MaintenanceNo;
                        doMemo.ObjectID = ScreenID.C_SCREEN_ID_INSTALL_MANAGE;
                        doMemo.Memo = InstallationMemo;
                        doMemo.OfficeCode = CommonUtil.dsTransData.dtUserData.MainOfficeCode;
                        doMemo.DepartmentCode = CommonUtil.dsTransData.dtUserData.MainDepartmentCode;
                        handler.InsertTbt_InstallationMemo(doMemo);
                    }
                    /////////////////////////////////////////////////////////
                    //======================================================


                    scope.Complete();

                }

                ISS090_RegisterStartResumeTargetData result = new ISS090_RegisterStartResumeTargetData();

                res.ResultData = result;

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }


        /// <summary>
        /// Validate business contract
        /// </summary>
        /// <param name="contractData"></param>
        /// <param name="saleData"></param>
        /// <param name="strServiceTypeCode"></param>
        /// <returns></returns>
        public ObjectResultData ISS090_ValidateContractError(dtRentalContractBasicForInstall contractData, dtSaleBasic saleData, string strServiceTypeCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            string strContractCode = "";
            try
            {

                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                IRentralContractHandler ReantalContracthandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                //{
                //    if (ISS090_ValidExistOffice(contractData.OperationOfficeCode) == false)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                //        return res;
                //    }
                //    strContractCode = contractData.ContractCode;

                //}
                //else if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                //{
                //    if (ISS090_ValidExistOffice(saleData.OperationOfficeCode) == false)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                //        return res;
                //    }
                //    strContractCode = saleData.ContractCode;

                //}       


                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Validate data before register
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <param name="ChangeRequestorCode"></param>
        /// <param name="ChangeReasonCode"></param>
        /// <returns></returns>
        public ActionResult ISS090_ValidateBeforeRegister(string strContractProjectCode, string ChangeRequestorCode, string ChangeReasonCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_MANAGE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                if (!CommonUtil.IsNullOrEmpty(ChangeReasonCode) && CommonUtil.IsNullOrEmpty(ChangeRequestorCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5086, null, new string[] { "ChangeRequestorCode" });
                    return Json(res);
                }
                if (!CommonUtil.IsNullOrEmpty(ChangeRequestorCode) && CommonUtil.IsNullOrEmpty(ChangeReasonCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5087, null, new string[] { "ChangeReasonCode" });
                    return Json(res);
                }

                if (handler.CheckCancelInstallationManagement(sParam.MaintenanceNo))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                UpdateScreenObject(sParam);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
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
        /// Check existing office
        /// </summary>
        /// <param name="OfficeCode"></param>
        /// <returns></returns>
        public bool ISS090_ValidExistOffice(string OfficeCode)
        {
            if (CommonUtil.IsNullOrEmpty(OfficeCode) == false)
            {
                List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
                if (clst != null)
                {
                    foreach (OfficeDataDo off in clst)
                    {
                        if (off.OfficeCode == OfficeCode)
                            return true;
                    }
                }

            }
            return false;

        }
        /// <summary>
        /// Validate business Project
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ObjectResultData ISS090_ValidateProjectError(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {

                IProjectHandler handler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<string> strStatus = handler.GetProjectStatus(strProjectCode);

                if (strStatus.Count > 0)
                {
                    if (strStatus[0] == ProjectStatus.C_PROJECT_STATUS_CANCEL || strStatus[0] == ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5002);
                    }
                }

                //IInstallationHandler installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                //bool blnInstallationRegistered = installHandler.CheckInstallationRegister(strProjectCode);
                //if (blnInstallationRegistered)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5003);
                //}
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return res;
        }

        //public ObjectResultData ISS090_ValidateBusiness(string strContractProjectCode, string strServiceTypeCode, string strInstallType)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
        //    try
        //    {

        //        IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
        //        ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();

        //        if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL || strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
        //        {
        //            //tbt_InstallationBasic doTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(strContractProjectCode);
        //            tbt_InstallationBasic doTbt_InstallationBasic = null;
        //            List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(strContractProjectCode);
        //            if (tmpdoTbt_InstallationBasic != null)
        //            {
        //                doTbt_InstallationBasic = new tbt_InstallationBasic();
        //                doTbt_InstallationBasic = tmpdoTbt_InstallationBasic[0];
        //            }
        //            if(strInstallType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW )
        //            {
        //                if (doTbt_InstallationBasic == null || doTbt_InstallationBasic.InstallationStatus != InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
        //                {
        //                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG5064);
        //                }
        //            }
        //            else
        //            {
        //                if (doTbt_InstallationBasic != null && doTbt_InstallationBasic.InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
        //                {
        //                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG5065);
        //                }
        //            }
        //        }                               
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }
        //    return res;
        //}

        /// <summary>
        /// Clear all email data in session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS090_ClearInstallEmail(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS090_ScreenParameter session = GetScreenObject<ISS090_ScreenParameter>();
            session.ListDOEmail = null;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Clear all PO information in session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS090_ClearPOInfo(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS090_ScreenParameter session = GetScreenObject<ISS090_ScreenParameter>();
            session.ListPOInfo = null;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Get active Employee name by code
        /// </summary>
        /// <param name="MaintEmpNo"></param>
        /// <returns></returns>
        public ActionResult ISS090_LoadEmployeeName(string MaintEmpNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //5.1 Get employee
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
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
                res.ResultData = ISS090_getEmployeeDisplayName(MaintEmpNo);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get employee display by code
        /// </summary>
        /// <param name="MaintEmpNo"></param>
        /// <returns></returns>
        private String ISS090_getEmployeeDisplayName(string MaintEmpNo)
        {
            String empDisplayName = String.Empty;
            try
            {
                IEmployeeMasterHandler employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<dtEmpNo> dtEmpNo = employeeHandler.GetEmployeeNameByEmpNo(MaintEmpNo);
                if (dtEmpNo.Count > 0)
                    empDisplayName = dtEmpNo[0].EmployeeNameDisplay;
            }
            catch (Exception)
            {
                throw;
            }
            return empDisplayName;
        }
        /// <summary>
        /// Get mode for screen display
        /// </summary>
        /// <param name="strInstallManagementStatus"></param>
        /// <returns></returns>
        public string ISS090_CheckScreenMode(string strInstallManagementStatus)
        {
            string strScreenMode = "";
            string strMode = "";
            try
            {
                if (strInstallManagementStatus == InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_PROCESSING || strInstallManagementStatus == InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REJECTED)
                {
                    strScreenMode = FunctionID.C_FUNC_ID_OPERATE;
                }
                else if (strInstallManagementStatus == InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REQUEST_APPROVE)
                {
                    strScreenMode = FunctionID.C_FUNC_ID_APPROVE;
                }
                else if (strInstallManagementStatus == InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_APPROVED)
                {
                    strScreenMode = FunctionID.C_FUNC_ID_COMPLETE;
                }
                else
                {
                    strScreenMode = FunctionID.C_FUNC_ID_VIEW;
                }

                strMode = FunctionID.C_FUNC_ID_VIEW;

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_MANAGE, strScreenMode))
                {
                    strScreenMode = strMode;
                }


            }
            catch (Exception ex)
            {
                throw;
            }


            return strScreenMode;
        }
        /// <summary>
        /// Check Can use this permission in screen if can,t set to view mode
        /// </summary>
        /// <param name="strInstallManagementStatus"></param>
        /// <returns></returns>
        public ActionResult ISS090_CheckScreenModePermission(string ModeFunction)
        {

            string strMode = "";
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                strMode = FunctionID.C_FUNC_ID_VIEW;

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_MANAGE, ModeFunction))
                {
                    ModeFunction = strMode;
                }
                res.ResultData = ModeFunction;

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }


            return Json(res);
        }
        /// <summary>
        /// Get PO information data from screen 
        /// </summary>
        /// <param name="ScreenParam"></param>
        /// <returns></returns>
        public ActionResult ISS090_SendGridDetailsData(ISS090_ScreenParameter ScreenParam)
        {
            ObjectResultData res = new ObjectResultData();

            ISS090_ScreenParameter session = GetScreenObject<ISS090_ScreenParameter>();
            //====== Get for insert

            int IndexPO = -1;
            if (ScreenParam.ListPOInfo != null)
            {
                foreach (tbt_InstallationPOManagement POData in ScreenParam.ListPOInfo)
                {
                    IndexPO = session.ListPOInfo.FindIndex(delegate (tbt_InstallationPOManagement s) { return s.SubcontractorCode.Trim() == POData.SubcontractorCode.Trim(); });
                    if (IndexPO >= 0)
                    {
                        session.ListPOInfo[IndexPO].InstallStartDate = POData.InstallStartDate;
                        session.ListPOInfo[IndexPO].InstallCompleteDate = POData.InstallCompleteDate;
                        session.ListPOInfo[IndexPO].ExpectInstallStartDate = POData.ExpectInstallStartDate;
                        session.ListPOInfo[IndexPO].ExpectInstallCompleteDate = POData.ExpectInstallCompleteDate;
                        session.ListPOInfo[IndexPO].ManPower = POData.ManPower;
                        session.ListPOInfo[IndexPO].ComplainCode = POData.ComplainCode;
                        session.ListPOInfo[IndexPO].IEFirstCheckDate = POData.IEFirstCheckDate;
                        session.ListPOInfo[IndexPO].IELastInspectionDate = POData.IELastInspectionDate;
                        session.ListPOInfo[IndexPO].AdjustmentCode = POData.AdjustmentCode;
                        session.ListPOInfo[IndexPO].AdjustmentContentCode = POData.AdjustmentContentCode;
                        session.ListPOInfo[IndexPO].IEEvaluationCode = POData.IEEvaluationCode;
                        session.ListPOInfo[IndexPO].LastInstallationFee = POData.LastInstallationFee;
                        session.ListPOInfo[IndexPO].PaidFlag = POData.PaidFlag;
                        session.ListPOInfo[IndexPO].IssuePOFlag = POData.IssuePOFlag;
                        session.ListPOInfo[IndexPO].IssuePODate = POData.IssuePODate;
                        session.ListPOInfo[IndexPO].CheckChargeIEEmpNo = POData.CheckChargeIEEmpNo;
                        session.ListPOInfo[IndexPO].AdvancePaymentFlag = POData.AdvancePaymentFlag;

                        //Add by Jutarat A. on 11102013
                        session.ListPOInfo[IndexPO].NoCheckFlag = POData.NoCheckFlag;
                        session.ListPOInfo[IndexPO].LastPaidDate = POData.LastPaidDate;
                        session.ListPOInfo[IndexPO].IMFee = POData.IMFee;
                        session.ListPOInfo[IndexPO].OtherFee = POData.OtherFee;
                        session.ListPOInfo[IndexPO].IMRemark = POData.IMRemark;
                        session.ListPOInfo[IndexPO].InstallationFee1 = POData.InstallationFee1;
                        session.ListPOInfo[IndexPO].ApproveNo1 = POData.ApproveNo1;
                        session.ListPOInfo[IndexPO].PaidDate1 = POData.PaidDate1;
                        session.ListPOInfo[IndexPO].InstallationFee2 = POData.InstallationFee2;
                        session.ListPOInfo[IndexPO].ApproveNo2 = POData.ApproveNo2;
                        session.ListPOInfo[IndexPO].PaidDate2 = POData.PaidDate2;
                        session.ListPOInfo[IndexPO].InstallationFee3 = POData.InstallationFee3;
                        session.ListPOInfo[IndexPO].ApproveNo3 = POData.ApproveNo3;
                        session.ListPOInfo[IndexPO].PaidDate3 = POData.PaidDate3;
                        //End Add
                        session.ListPOInfo[IndexPO].NoCheckFlag = POData.NoCheckFlag; //Add by Jutarat A. on 07112013
                        session.ListPOInfo[IndexPO].ActualPOAmount = POData.ActualPOAmount; //Add by Jutarat A. on 07112013
                    }
                }
            }


            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Validate business before request approve
        /// </summary>
        /// <param name="dtManagement"></param>
        /// <returns></returns>
        public ActionResult ISS090_ValidateBeforeRequestApprove(tbt_InstallationManagement dtManagement)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (handler.CheckCancelInstallationManagement(sParam.MaintenanceNo))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5038, new string[] { "request approve" }, null);
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS090",
                                        MessageUtil.MODULE_INSTALLATION,
                                        MessageUtil.MessageList.MSG5038,
                                        new string[] { "lblRequestApprove" },
                                        new string[] { "" });
                    return Json(res);
                }

                if (sParam.ListPOInfo == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5036);
                    return Json(res);
                }
                else
                {
                    foreach (tbt_InstallationPOManagement dataPO in sParam.ListPOInfo)
                    {
                        if (dataPO.NormalSubPOAmount != null && dataPO.NormalSubPOAmount > 0)
                        {
                            ICommonContractHandler comHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                            List<tbm_SubContractor> dtSubContractorList = comHandler.GetTbm_SubContractorData(dataPO.SubcontractorCode);
                            string strSubContractorName = "";
                            if (dtSubContractorList != null && dtSubContractorList.Count > 0)
                            {
                                CommonUtil.MappingObjectLanguage<tbm_SubContractor>(dtSubContractorList);
                                strSubContractorName = dtSubContractorList[0].SubContractorName;
                            }
                            //============== Change to show Error All Field in Subcontractor ================
                            string FieldError = string.Empty;
                            //===============================================================================

                            if (dataPO.NoCheckFlag == false) //Add by Jutarat A. on 07112013
                            {
                                if (CommonUtil.IsNullOrEmpty(dataPO.InstallStartDate))
                                {
                                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                    //                "ISS090",
                                    //                MessageUtil.MODULE_INSTALLATION,
                                    //                MessageUtil.MessageList.MSG5019,
                                    //                new string[] { strSubContractorName, "lblInstallationStartDate" },
                                    //                new string[] { "" });
                                    //return Json(res);
                                    string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblInstallationStartDate");
                                    if (CommonUtil.IsNullOrEmpty(FieldError))
                                        FieldError = label;
                                    else
                                        FieldError = FieldError + "," + label;
                                }
                                if (CommonUtil.IsNullOrEmpty(dataPO.InstallCompleteDate))
                                {
                                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                    //                "ISS090",
                                    //                MessageUtil.MODULE_INSTALLATION,
                                    //                MessageUtil.MessageList.MSG5019,
                                    //                new string[] { strSubContractorName, "lblInstallationCompleteDate" },
                                    //                new string[] { "" });
                                    //return Json(res);
                                    string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblInstallationCompleteDate");
                                    if (CommonUtil.IsNullOrEmpty(FieldError))
                                        FieldError = label;
                                    else
                                        FieldError = FieldError + "," + label;
                                }
                                if (CommonUtil.IsNullOrEmpty(dataPO.ManPower))
                                {
                                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                    //                "ISS090",
                                    //                MessageUtil.MODULE_INSTALLATION,
                                    //                MessageUtil.MessageList.MSG5019,
                                    //                new string[] { strSubContractorName, "lblManpower" },
                                    //                new string[] { "" });
                                    //return Json(res);
                                    string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblManpower");
                                    if (CommonUtil.IsNullOrEmpty(FieldError))
                                        FieldError = label;
                                    else
                                        FieldError = FieldError + "," + label;
                                }
                                if (CommonUtil.IsNullOrEmpty(dataPO.ComplainCode))
                                {
                                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                    //                "ISS090",
                                    //                MessageUtil.MODULE_INSTALLATION,
                                    //                MessageUtil.MessageList.MSG5019,
                                    //                new string[] { strSubContractorName, "lblComplain" },
                                    //                new string[] { "" });
                                    //return Json(res);
                                    string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblComplain");
                                    if (CommonUtil.IsNullOrEmpty(FieldError))
                                        FieldError = label;
                                    else
                                        FieldError = FieldError + "," + label;
                                }

                                //if (dataPO.NoCheck == false) //Add by Jutarat A. on 11102013
                                //{
                                if (CommonUtil.IsNullOrEmpty(dataPO.IEFirstCheckDate))
                                {
                                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                    //                "ISS090",
                                    //                MessageUtil.MODULE_INSTALLATION,
                                    //                MessageUtil.MessageList.MSG5019,
                                    //                new string[] { strSubContractorName, "lblIEFirstCheckDate" },
                                    //                new string[] { "" });
                                    //return Json(res);
                                    string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblIEFirstCheckDate");
                                    if (CommonUtil.IsNullOrEmpty(FieldError))
                                        FieldError = label;
                                    else
                                        FieldError = FieldError + "," + label;
                                }
                                if (CommonUtil.IsNullOrEmpty(dataPO.AdjustmentCode))
                                {
                                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                    //                "ISS090",
                                    //                MessageUtil.MODULE_INSTALLATION,
                                    //                MessageUtil.MessageList.MSG5019,
                                    //                new string[] { strSubContractorName, "lblAdjustment" },
                                    //                new string[] { "" });
                                    //return Json(res);
                                    string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblAdjustment");
                                    if (CommonUtil.IsNullOrEmpty(FieldError))
                                        FieldError = label;
                                    else
                                        FieldError = FieldError + "," + label;
                                }
                                if (CommonUtil.IsNullOrEmpty(dataPO.AdjustmentContentCode) && dataPO.AdjustmentCode == AdjustmentType.C_INSTALL_ADJUSTMENT_HAVE)
                                {
                                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                    //                "ISS090",
                                    //                MessageUtil.MODULE_INSTALLATION,
                                    //                MessageUtil.MessageList.MSG5019,
                                    //                new string[] { strSubContractorName, "lblAdjustmentContents" },
                                    //                new string[] { "" });
                                    //return Json(res);
                                    string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblAdjustmentContents");
                                    if (CommonUtil.IsNullOrEmpty(FieldError))
                                        FieldError = label;
                                    else
                                        FieldError = FieldError + "," + label;
                                }
                                if (CommonUtil.IsNullOrEmpty(dataPO.IELastInspectionDate))
                                {
                                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                    //                "ISS090",
                                    //                MessageUtil.MODULE_INSTALLATION,
                                    //                MessageUtil.MessageList.MSG5019,
                                    //                new string[] { strSubContractorName, "lblIELastInspectionDate" },
                                    //                new string[] { "" });
                                    //return Json(res);
                                    string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblIELastInspectionDate");
                                    if (CommonUtil.IsNullOrEmpty(FieldError))
                                        FieldError = label;
                                    else
                                        FieldError = FieldError + "," + label;
                                }
                                //}

                                if (CommonUtil.IsNullOrEmpty(dataPO.IEEvaluationCode))
                                {
                                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                    //                "ISS090",
                                    //                MessageUtil.MODULE_INSTALLATION,
                                    //                MessageUtil.MessageList.MSG5019,
                                    //                new string[] { strSubContractorName, "lblIEEvaluation" },
                                    //                new string[] { "" });
                                    //return Json(res);
                                    string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblIEEvaluation");
                                    if (CommonUtil.IsNullOrEmpty(FieldError))
                                        FieldError = label;
                                    else
                                        FieldError = FieldError + "," + label;
                                }
                            }//End NoCheck

                            //Modify by Jutarat A. on 08112013
                            /*if (CommonUtil.IsNullOrEmpty(dataPO.LastInstallationFee))
                            {
                                //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                //                "ISS090",
                                //                MessageUtil.MODULE_INSTALLATION,
                                //                MessageUtil.MessageList.MSG5019,
                                //                new string[] { strSubContractorName, "lblLastInstallationFee" },
                                //                new string[] { "" });
                                //return Json(res);
                                string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblLastInstallationFee");
                                if (CommonUtil.IsNullOrEmpty(FieldError))
                                    FieldError = label;
                                else
                                    FieldError = FieldError + "," + label;
                            }*/
                            if (CommonUtil.IsNullOrEmpty(dataPO.LastPaidDate))
                            {
                                string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblLastPaidDate");
                                if (CommonUtil.IsNullOrEmpty(FieldError))
                                    FieldError = label;
                                else
                                    FieldError = FieldError + "," + label;
                            }
                            //End modify

                            if (!CommonUtil.IsNullOrEmpty(FieldError))
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                "ISS090",
                                                MessageUtil.MODULE_INSTALLATION,
                                                MessageUtil.MessageList.MSG5019,
                                                new string[] { strSubContractorName, FieldError },
                                                new string[] { "" });
                                return Json(res);
                            }
                        }
                    }
                }
                //============== Change to show Error IE ManPower and Material Fee  ================
                string FieldError2 = string.Empty;
                string[] ControlError = new string[2];
                if (CommonUtil.IsNullOrEmpty(dtManagement.IEManPower))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                    "ISS090",
                    //                    MessageUtil.MODULE_COMMON,
                    //                    MessageUtil.MessageList.MSG0007,
                    //                    new string[] { "lblIEManPower" },
                    //                    new string[] { "" });
                    //return Json(res);
                    string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblIEManPower");
                    if (CommonUtil.IsNullOrEmpty(FieldError2))
                        FieldError2 = label;
                    else
                        FieldError2 = FieldError2 + "," + label;
                    ControlError[0] = "IEManPower";
                }
                if (CommonUtil.IsNullOrEmpty(dtManagement.MaterialFee))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                    "ISS090",
                    //                    MessageUtil.MODULE_COMMON,
                    //                    MessageUtil.MessageList.MSG0007,
                    //                    new string[] { "lblMaterialFee" },
                    //                    new string[] { "" });
                    //return Json(res);
                    string label = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INSTALLATION, "ISS090", "lblMaterialFee");
                    if (CommonUtil.IsNullOrEmpty(FieldError2))
                        FieldError2 = label;
                    else
                        FieldError2 = FieldError2 + "," + label;
                    ControlError[1] = "MaterialFee";
                }
                if (!CommonUtil.IsNullOrEmpty(FieldError2))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS090",
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { FieldError2 },
                                        ControlError);
                    return Json(res);
                }

                //===============================================================================



                if (res.IsError)
                    return Json(res);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Request approve installation
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS090_RequestApprove(tbt_InstallationManagement dtManagement, string InstallationMemo)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //===================== UPDATE TBT_INSTALLATION MANAGEMENT ==================

                    //=================== Teerapong 06/08/2012 ==================================
                    //sParam.dtInstallationManagement.ManagementStatus = InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REQUEST_APPROVE;
                    //handler.UpdateTbt_InstallationManagement(sParam.dtInstallationManagement);
                    //===================== UPDATE TBT_INSTALLATION MANAGEMENT ==================
                    sParam.dtInstallationManagement.IEManPower = dtManagement.IEManPower;
                    sParam.dtInstallationManagement.MaterialFee = dtManagement.MaterialFee;
                    sParam.dtInstallationManagement.ChangeReasonCode = dtManagement.ChangeReasonCode;
                    sParam.dtInstallationManagement.ChangeReasonOther = dtManagement.ChangeReasonOther;
                    sParam.dtInstallationManagement.ChangeRequestorCode = dtManagement.ChangeRequestorCode;
                    sParam.dtInstallationManagement.ChangeRequestorOther = dtManagement.ChangeRequestorOther;
                    sParam.dtInstallationManagement.ManagementStatus = InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REQUEST_APPROVE;
                    handler.UpdateTbt_InstallationManagement(sParam.dtInstallationManagement);
                    //============================================================================
                    //====================== UPDATE TBT_INSTALLATIONPOMANAGEMENT =================
                    if (sParam.ListPOInfo != null)
                    {
                        List<tbt_InstallationPOManagement> tempTbt_InstallationPOManagement = handler.GetTbt_InstallationPOManagementData(sParam.MaintenanceNo);
                        foreach (tbt_InstallationPOManagement dataPOManagement in sParam.ListPOInfo)
                        {
                            dataPOManagement.PaidFlag = FlagType.C_FLAG_OFF;
                            //dataPOManagement.IssuePOFlag = FlagType.C_FLAG_OFF;
                            //dataPOManagement.IssuePODate = null;
                            dataPOManagement.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            dataPOManagement.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                            List<tbt_InstallationPOManagement> tempPrepareUpdateData = (from c in tempTbt_InstallationPOManagement where c.SubcontractorCode.Trim() == dataPOManagement.SubcontractorCode.Trim() select c).ToList<tbt_InstallationPOManagement>();
                            if (tempPrepareUpdateData != null && tempPrepareUpdateData.Count > 0)
                            {
                                tempPrepareUpdateData[0].InstallStartDate = dataPOManagement.InstallStartDate;
                                tempPrepareUpdateData[0].InstallCompleteDate = dataPOManagement.InstallCompleteDate;
                                tempPrepareUpdateData[0].ExpectInstallStartDate = dataPOManagement.ExpectInstallStartDate;
                                tempPrepareUpdateData[0].ExpectInstallCompleteDate = dataPOManagement.ExpectInstallCompleteDate;
                                tempPrepareUpdateData[0].ManPower = dataPOManagement.ManPower;
                                tempPrepareUpdateData[0].ComplainCode = dataPOManagement.ComplainCode;
                                tempPrepareUpdateData[0].IEFirstCheckDate = dataPOManagement.IEFirstCheckDate;
                                tempPrepareUpdateData[0].IELastInspectionDate = dataPOManagement.IELastInspectionDate;
                                tempPrepareUpdateData[0].AdjustmentCode = dataPOManagement.AdjustmentCode;
                                tempPrepareUpdateData[0].AdjustmentContentCode = dataPOManagement.AdjustmentContentCode;
                                tempPrepareUpdateData[0].IEEvaluationCode = dataPOManagement.IEEvaluationCode;
                                tempPrepareUpdateData[0].LastInstallationFee = dataPOManagement.LastInstallationFee;
                                tempPrepareUpdateData[0].PaidFlag = dataPOManagement.PaidFlag;
                                tempPrepareUpdateData[0].CheckChargeIEEmpNo = dataPOManagement.CheckChargeIEEmpNo;
                                tempPrepareUpdateData[0].AdvancePaymentFlag = dataPOManagement.AdvancePaymentFlag;

                                //Add by Jutarat A. on 11102013
                                tempPrepareUpdateData[0].LastPaidDate = dataPOManagement.LastPaidDate;
                                tempPrepareUpdateData[0].IMFee = dataPOManagement.IMFee;
                                tempPrepareUpdateData[0].OtherFee = dataPOManagement.OtherFee;
                                tempPrepareUpdateData[0].IMRemark = dataPOManagement.IMRemark;
                                tempPrepareUpdateData[0].InstallationFee1 = dataPOManagement.InstallationFee1;
                                tempPrepareUpdateData[0].ApproveNo1 = dataPOManagement.ApproveNo1;
                                tempPrepareUpdateData[0].PaidDate1 = dataPOManagement.PaidDate1;
                                tempPrepareUpdateData[0].InstallationFee2 = dataPOManagement.InstallationFee2;
                                tempPrepareUpdateData[0].ApproveNo2 = dataPOManagement.ApproveNo2;
                                tempPrepareUpdateData[0].PaidDate2 = dataPOManagement.PaidDate2;
                                tempPrepareUpdateData[0].InstallationFee3 = dataPOManagement.InstallationFee3;
                                tempPrepareUpdateData[0].ApproveNo3 = dataPOManagement.ApproveNo3;
                                tempPrepareUpdateData[0].PaidDate3 = dataPOManagement.PaidDate3;
                                //End Add
                                tempPrepareUpdateData[0].NoCheckFlag = dataPOManagement.NoCheckFlag; //Add by Jutarat A. on 07112013
                                tempPrepareUpdateData[0].ActualPOAmount = dataPOManagement.ActualPOAmount; //Add by Jutarat A. on 07112013

                                handler.UpdateTbt_InstallationPOManagement(tempPrepareUpdateData[0]);
                            }
                            //handler.UpdateTbt_InstallationPOManagement(dataPOManagement);
                        }

                    }
                    //============================================================================

                    //================= Update Attach ======================
                    List<dtAttachFileForGridView> tmpFileList = chandler.GetAttachFileForGridView(sParam.MaintenanceNo);
                    if (tmpFileList != null && tmpFileList.Count > 0)
                    {
                        chandler.UpdateFlagAttachFile(AttachmentModule.Installation, sParam.MaintenanceNo, sParam.MaintenanceNo);
                        foreach (dtAttachFileForGridView file in tmpFileList)
                        {
                            int? attachID = (int?)Convert.ToInt32(file.AttachFileID);
                            List<tbt_InstallationAttachFile> InsAttachFileList = handler.GetTbt_InstallationAttachFile(attachID, sParam.MaintenanceNo, null);
                            if (InsAttachFileList == null)
                            {
                                tbt_InstallationAttachFile doTbt_InstallAttachFile = new tbt_InstallationAttachFile();
                                doTbt_InstallAttachFile.MaintenanceNo = sParam.MaintenanceNo;
                                doTbt_InstallAttachFile.AttachFileID = file.AttachFileID;
                                doTbt_InstallAttachFile.ObjectID = ScreenID.C_SCREEN_ID_INSTALL_PO;
                                handler.InsertTbt_InstallationAttachFile(doTbt_InstallAttachFile);
                            }
                        }
                    }

                    /////////////////// INSERT MEMO ////////////////////////
                    if (!CommonUtil.IsNullOrEmpty(InstallationMemo))
                    {
                        tbt_InstallationMemo doMemo = new tbt_InstallationMemo();
                        doMemo.ContractProjectCode = sParam.ContractProjectCodeLong;
                        doMemo.ReferenceID = sParam.MaintenanceNo;
                        doMemo.ObjectID = ScreenID.C_SCREEN_ID_INSTALL_MANAGE;
                        doMemo.Memo = InstallationMemo;
                        doMemo.OfficeCode = CommonUtil.dsTransData.dtUserData.MainOfficeCode;
                        doMemo.DepartmentCode = CommonUtil.dsTransData.dtUserData.MainDepartmentCode;
                        handler.InsertTbt_InstallationMemo(doMemo);
                    }
                    /////////////////////////////////////////////////////////
                    //===========================================================================

                    //============================================================================
                    ////====================== UPDATE TBT_INSTALLATIONPOMANAGEMENT =================
                    //if (sParam.ListPOInfo != null)
                    //{
                    //    foreach (tbt_InstallationPOManagement dataPOManagement in sParam.ListPOInfo)
                    //    {
                    //        dataPOManagement.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //        dataPOManagement.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //        handler.UpdateTbt_InstallationPOManagement(dataPOManagement);
                    //    }

                    //}
                    ////============================================================================
                    //UpdateScreenObject(sParam);
                    //====================== SEND EMAIL ============================================
                    SendMailObject obj = new SendMailObject();
                    if (sParam.ListApproveEmail != null)
                    {
                        obj.EmailList = new List<doEmailProcess>();
                        doEmailTemplate template = ISS090_GenerateMailRequestApproveInstallation(sParam);
                        if (template != null)
                        {
                            foreach (ISS090_DOEmailData e in sParam.ListApproveEmail)
                            {
                                doEmailProcess mail = new doEmailProcess()
                                {
                                    MailTo = e.EmailAddress,
                                    Subject = template.TemplateSubject,
                                    Message = template.TemplateContent
                                };
                                obj.EmailList.Add(mail);
                            }
                            if (!CommonUtil.IsNullOrEmpty(sParam.ListApproveEmail) && sParam.ListApproveEmail.Count > 0)
                            {
                                System.Threading.Thread t = new System.Threading.Thread(SendMail);
                                t.Start(obj);
                            }
                        }

                    }
                    //==============================================================================
                    scope.Complete();

                }

                if (res.IsError)
                    return Json(res);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }


        /// <summary>
        /// Validate business before approve
        /// </summary>
        /// <param name="dtManagement"></param>
        /// <returns></returns>
        public ActionResult ISS090_ValidateBeforeApprove(tbt_InstallationManagement dtManagement)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (handler.CheckCancelInstallationManagement(sParam.MaintenanceNo))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5038,new string[] {"lblApprove"},null);
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS090",
                                        MessageUtil.MODULE_INSTALLATION,
                                        MessageUtil.MessageList.MSG5038,
                                        new string[] { "lblApprove" },
                                        new string[] { "" });
                    return Json(res);
                }


                if (res.IsError)
                    return Json(res);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Approve installation data
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS090_Approve(tbt_InstallationManagement dtManagement)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //===================== UPDATE TBT_INSTALLATION MANAGEMENT ==================
                    sParam.dtInstallationManagement.ManagementStatus = InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_APPROVED;
                    handler.UpdateTbt_InstallationManagement(sParam.dtInstallationManagement);
                    //============================================================================
                    ////====================== UPDATE TBT_INSTALLATIONPOMANAGEMENT =================
                    //if (sParam.ListPOInfo != null)
                    //{
                    //    foreach (tbt_InstallationPOManagement dataPOManagement in sParam.ListPOInfo)
                    //    {
                    //        dataPOManagement.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //        dataPOManagement.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //        handler.UpdateTbt_InstallationPOManagement(dataPOManagement);
                    //    }

                    //}
                    ////============================================================================
                    UpdateScreenObject(sParam);
                    scope.Complete();

                }

                if (res.IsError)
                    return Json(res);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Update management status in Tbt_InstallationManagement
        /// </summary>
        /// <param name="ManagementStatus"></param>
        /// <returns></returns>
        public ActionResult ISS090_UpdateManagementStatus(string ManagementStatus)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //===================== UPDATE TBT_INSTALLATION MANAGEMENT ==================
                    sParam.dtInstallationManagement.ManagementStatus = ManagementStatus;
                    handler.UpdateTbt_InstallationManagement(sParam.dtInstallationManagement);
                    //============================================================================                   
                    //UpdateScreenObject(sParam);
                    scope.Complete();

                }

                if (res.IsError)
                    return Json(res);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult ISS090_UpdateLastInstallation(string MaintenanceNo, int rowIndex, decimal LastInstallationFee)
        {
            ObjectResultData res = new ObjectResultData();
            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

            List<tbt_InstallationPOManagement> ListPOInfo = new List<tbt_InstallationPOManagement>();
            ListPOInfo = handler.GetTbt_InstallationPOManagementData(MaintenanceNo);

            for (int i = 0; i < ListPOInfo.Count(); i++)
            {
                ListPOInfo[i].Currencies = tmpCurrencies;
            }

            //Update Amt for convert sting with Currency
            if (ListPOInfo[rowIndex].LastInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
            {
                ListPOInfo[rowIndex].TxtCurrencyLastInstallationFee = tmpCurrencies[1].ValueDisplayEN;
                ListPOInfo[rowIndex].LastInstallationFee = LastInstallationFee;
            }
            else
            {
                ListPOInfo[rowIndex].LastInstallationFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                ListPOInfo[rowIndex].TxtCurrencyLastInstallationFee = tmpCurrencies[0].ValueDisplayEN;
                ListPOInfo[rowIndex].LastInstallationFee = LastInstallationFee;
            }

            res.ResultData = ListPOInfo;
            return Json(res);
        }

        /// <summary>
        /// Validate business before reject
        /// </summary>
        /// <param name="dtManagement"></param>
        /// <returns></returns>
        public ActionResult ISS090_ValidateBeforeReject(tbt_InstallationManagement dtManagement)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (handler.CheckCancelInstallationManagement(sParam.MaintenanceNo))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5038, new string[] { "reject" }, null);
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS090",
                                        MessageUtil.MODULE_INSTALLATION,
                                        MessageUtil.MessageList.MSG5038,
                                        new string[] { "lblApprove" },
                                        new string[] { "" });
                    return Json(res);
                }


                if (res.IsError)
                    return Json(res);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// update ManagementStatus to Reject
        /// </summary>
        /// <param name="dtManagement"></param>
        /// <returns></returns>
        public ActionResult ISS090_Reject(tbt_InstallationManagement dtManagement)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //===================== UPDATE TBT_INSTALLATION MANAGEMENT ==================
                    sParam.dtInstallationManagement.ManagementStatus = InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REJECTED;
                    handler.UpdateTbt_InstallationManagement(sParam.dtInstallationManagement);
                    //============================================================================
                    ////====================== UPDATE TBT_INSTALLATIONPOMANAGEMENT =================
                    //if (sParam.ListPOInfo != null)
                    //{
                    //    foreach (tbt_InstallationPOManagement dataPOManagement in sParam.ListPOInfo)
                    //    {
                    //        dataPOManagement.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //        dataPOManagement.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //        handler.UpdateTbt_InstallationPOManagement(dataPOManagement);
                    //    }

                    //}
                    ////============================================================================
                    UpdateScreenObject(sParam);
                    scope.Complete();

                }

                if (res.IsError)
                    return Json(res);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get office name by code
        /// </summary>
        /// <param name="ValueCode"></param>
        /// <returns></returns>
        public string ISS090_GetOfficeNameByCode(string ValueCode)
        {
            ObjectResultData res = new ObjectResultData();

            List<tbm_Office> list = new List<tbm_Office>();
            string lang = CommonUtil.GetCurrentLanguage();
            string OfficeName = "";
            try
            {
                IOfficeMasterHandler OMHandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                list = OMHandler.GetTbm_Office();
                list = (from c in list where c.OfficeCode == ValueCode select c).ToList<tbm_Office>();
                if (list != null && list.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage<tbm_Office>(list);
                    foreach (tbm_Office officeData in list)
                    {
                        OfficeName = CommonUtil.TextCodeName(officeData.OfficeCode, officeData.OfficeName);
                    }
                }
                return OfficeName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Upload attach file
        /// </summary>
        /// <param name="fileSelect"></param>
        /// <param name="DocumentName"></param>
        /// <param name="ReferKey"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult ISS090_AttachFile(HttpPostedFileBase fileSelect, string DocumentName, string ReferKey, string k)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            MessageModel outmsg = null;

            try
            {
                byte[] fileData;
                if (fileSelect == null)
                {
                    outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0050, null);
                }
                else
                {
                    using (BinaryReader reader = new BinaryReader(fileSelect.InputStream))
                    {
                        var fList = commonhandler.GetAttachFileForGridView(ReferKey);

                        var filterDupItem = from a in fList where a.FileName.ToUpper().Equals(DocumentName.ToUpper() + Path.GetExtension(fileSelect.FileName).ToUpper()) select a;

                        if (filterDupItem.Count() > 0)
                        {
                            // Docname duplicate
                            outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115, new string[] { DocumentName });
                        }
                        else if (DocumentName == null || DocumentName == "")
                        {
                            string nparam = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS300", "lblDocumentName");
                            outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { nparam });
                        }
                        else
                        {
                            fileData = reader.ReadBytes(fileSelect.ContentLength);

                            if (commonhandler.CanAttachFile(DocumentName, fileData.Length, Path.GetExtension(fileSelect.FileName), ReferKey, GetCurrentKey()))
                            {
                                DateTime currDate = DateTime.Now;
                                commonhandler.InsertAttachFile(ReferKey
                                , DocumentName
                                , Path.GetExtension(fileSelect.FileName)
                                , fileData.Length
                                , fileData
                                , false);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                outmsg = new MessageModel();
                outmsg.Message = ((SECOM_AJIS.Common.Models.ApplicationErrorException)(ex)).ErrorResult.Message.Message;
                outmsg.Code = CommonValue.SYSTEM_MESSAGE_CODE;
            }

            if (outmsg != null)
            {
                ViewBag.Message = outmsg.Message;
                ViewBag.MsgCode = outmsg.Code;
            }
            ViewBag.K = k;
            ViewBag.ReferKey = ReferKey;

            return View("ISS090_Upload");
        }
        /// <summary>
        /// Initial grid attach schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS090_IntialGridAttachedDocList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS090_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// Remove attach data from session parameter
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult ISS090_RemoveAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                int _attachID = int.Parse(AttachID);
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                //commonhandler.DeleteAttachFileByID(_attachID, Session.SessionID);
                commonhandler.DeleteAttachFileByID(_attachID, sParam.MaintenanceNo);
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Load attach data and send to show in grid attach
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS090_LoadGridAttachedDocList()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();
            List<dtAttachFileForGridView> lstAttachedName = new List<dtAttachFileForGridView>(); //Add by Jutarat A. on 25032013

            try
            {
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();

                //List<dtAttachFileForGridView> lstAttachedName = commonhandler.GetAttachFileForGridView(sParam.MaintenanceNo);
                if (sParam != null)
                    lstAttachedName = commonhandler.GetAttachFileForGridView(sParam.MaintenanceNo); //Modify by Jutarat A. on 25032013

                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Installation\\ISS090_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Download attach file
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult ISS090_DownloadAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                var downloadFileStream = commonhandler.GetAttachFileForDownload(int.Parse(AttachID), sParam.MaintenanceNo);
                var downloadFileName = commonhandler.GetTbt_AttachFile(sParam.MaintenanceNo, int.Parse(AttachID), null);
                //var downloadFileName = commonhandler.GetAttachFileName(sParam.strIncidentID, int.Parse(AttachID), null);
                //var attachFile = commonhandler.GetAttachFile(AttachmentModule.Incident, ReleateID, int.Parse(AttachID));
                //var fileNameLst = commonhandler.GetAttachFileName(ReleateID, int.Parse(AttachID), true);

                string fileName = downloadFileName[0].FileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileName = Uri.EscapeDataString(fileName);
                }
                return File(downloadFileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Send referid to get attach data
        /// </summary>
        /// <param name="sK"></param>
        /// <returns></returns>
        public ActionResult ISS090_SendAttachKey(string sK = "")
        {
            ViewBag.ReferKey = sK;
            return View("ISS090_Upload");
        }
        /// <summary>
        /// Issue data installation PO management
        /// </summary>
        /// <param name="SubcontractorCode"></param>
        /// <returns></returns>
        public ActionResult ISS090_IssueButtonClick(string SubcontractorCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                    IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    if (sParam.ListPOInfo != null)
                    {
                        foreach (tbt_InstallationPOManagement dataPOManagement in sParam.ListPOInfo)
                        {
                            if (dataPOManagement.SubcontractorCode.Trim() == SubcontractorCode.Trim())
                            {
                                dataPOManagement.IssuePOFlag = FlagType.C_FLAG_ON;
                                dataPOManagement.IssuePODate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                handler.UpdateTbt_InstallationPOManagement(dataPOManagement);
                            }
                        }

                    }
                    scope.Complete();
                    res.ResultData = true;


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
        /// Update Paidflag data installation PO Management
        /// </summary>
        /// <param name="SubcontractorCode"></param>
        /// <returns></returns>
        public ActionResult ISS090_PaidButtonClick(string SubcontractorCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ISS090_ScreenParameter sParam = GetScreenObject<ISS090_ScreenParameter>();
                    IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    if (sParam.ListPOInfo != null)
                    {
                        foreach (tbt_InstallationPOManagement dataPOManagement in sParam.ListPOInfo)
                        {
                            if (dataPOManagement.SubcontractorCode.Trim() == SubcontractorCode.Trim())
                            {
                                dataPOManagement.PaidFlag = FlagType.C_FLAG_ON;
                                handler.UpdateTbt_InstallationPOManagement(dataPOManagement);
                            }
                        }

                    }
                    scope.Complete();
                    res.ResultData = true;


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
        /// Generate report accept inspec notice
        /// </summary>
        /// <param name="MaintenanceNo"></param>
        /// <param name="strSubContracttorCode"></param>
        /// <returns></returns>
        public ActionResult ISS090_CreateReportAcceptInspecNotice(string MaintenanceNo, string strSubContracttorCode)
        {
            IInstallationDocumentHandler hand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            return File(hand.CreateReportAcceptanceInspectionNotice(MaintenanceNo, strSubContracttorCode), "application/pdf", "ISR100.pdf");
        }
        /// <summary>
        /// Generate Email request approve
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        private doEmailTemplate ISS090_GenerateMailRequestApproveInstallation(ISS090_ScreenParameter sParam)
        {
            try
            {
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                tbt_InstallationSlip doTbtInstallationSlip = new tbt_InstallationSlip();
                tbt_InstallationHistory doTbtInstallationHistory = new tbt_InstallationHistory();
                doEmailContentRequestApprrove obj = new doEmailContentRequestApprrove();
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                string MiscInstallType = "";
                List<tbt_InstallationHistory> doTbtInstallationHistoryList = handler.GetTbt_InstallationHistory(sParam.dtInstallationManagement.ContractProjectCode, sParam.dtInstallationManagement.MaintenanceNo, null);
                if (doTbtInstallationHistoryList != null && doTbtInstallationHistoryList.Count > 0)
                {
                    doTbtInstallationHistory = doTbtInstallationHistoryList[0];
                    obj.ContractProjectCode = sParam.ContractProjectCodeShort;
                    obj.MaintenanceNo = doTbtInstallationHistory.MaintenanceNo;
                    if (doTbtInstallationHistory.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    {
                        MiscInstallType = MiscType.C_RENTAL_INSTALL_TYPE;
                    }
                    else if (doTbtInstallationHistory.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        MiscInstallType = MiscType.C_SALE_INSTALL_TYPE;
                    }
                }
                if (doTbtInstallationHistory.SlipNo != null)
                {
                    doTbtInstallationSlip = handler.GetTbt_InstallationSlipData(doTbtInstallationHistory.SlipNo);
                }
                if (doTbtInstallationSlip != null)
                {
                    //======== GET INSTALLATION TYPE NAME =============================
                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                    List<doMiscTypeCode> miscs;
                    if (doTbtInstallationSlip != null && doTbtInstallationSlip.InstallationType != null)
                    {
                        miscs = new List<doMiscTypeCode>()
                        {
                            new doMiscTypeCode()
                            {
                                FieldName = MiscInstallType,
                                ValueCode = doTbtInstallationSlip.InstallationType
                            }
                        };

                        lst = chandler.GetMiscTypeCodeList(miscs);
                    }
                    if (lst != null && lst.Count > 0)
                    {
                        obj.InstallationTypeNameEN = lst[0].ValueDisplayEN;
                        obj.InstallationTypeNameLC = lst[0].ValueDisplayLC;
                    }
                    //========== GET CHANGE REASON NAME ===============================
                    if (doTbtInstallationSlip != null && doTbtInstallationSlip.ChangeReasonCode != null)
                    {
                        miscs = new List<doMiscTypeCode>()
                        {
                            new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_INSTALL_CHANGE_REASON,
                                ValueCode = doTbtInstallationSlip.ChangeReasonCode
                            }
                        };

                        lst = chandler.GetMiscTypeCodeList(miscs);
                    }
                    if (lst != null && lst.Count > 0)
                    {
                        obj.ChangeReasonNameEN = lst[0].ValueDisplayEN;
                        obj.ChangeReasonNameLC = lst[0].ValueDisplayLC;
                    }
                    //========== GET CAUSE REASON NAME ===============================
                    if (doTbtInstallationSlip != null && doTbtInstallationSlip.ChangeReasonCode != null && doTbtInstallationSlip.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_CUS)
                    {
                        miscs = new List<doMiscTypeCode>()
                        {
                            new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_CUSTOMER_REASON,
                                ValueCode = doTbtInstallationSlip.CauseReason
                            }
                        };

                        lst = chandler.GetMiscTypeCodeList(miscs);
                        if (lst != null && lst.Count > 0)
                        {
                            obj.InstallationCauseReasonNameEN = lst[0].ValueDisplayEN;
                            obj.InstallationCauseReasonNameLC = lst[0].ValueDisplayLC;
                        }
                    }
                    else if (doTbtInstallationSlip != null && doTbtInstallationSlip.ChangeReasonCode != null && doTbtInstallationSlip.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_SECOM)
                    {
                        miscs = new List<doMiscTypeCode>()
                        {
                            new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_SECOM_REASON,
                                ValueCode = doTbtInstallationSlip.CauseReason
                            }
                        };

                        lst = chandler.GetMiscTypeCodeList(miscs);
                        if (lst != null && lst.Count > 0)
                        {
                            obj.InstallationCauseReasonNameEN = lst[0].ValueDisplayEN;
                            obj.InstallationCauseReasonNameLC = lst[0].ValueDisplayLC;
                        }
                    }
                    //========================== GET EMPLOYEE NAME =============================
                    IEmployeeMasterHandler empHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                    List<dtEmpNo> dtEmpnoList = null;
                    if (sParam.dtInstallationManagement.IEStaffEmpNo1 != null)
                    {
                        dtEmpnoList = empHandler.GetEmployeeNameByEmpNo(sParam.dtInstallationManagement.IEStaffEmpNo1);
                        if (dtEmpnoList != null && dtEmpnoList.Count > 0)
                        {
                            obj.IEStaff1NameEN = dtEmpnoList[0].EmpFirstNameEN + " " + dtEmpnoList[0].EmpLastNameEN;
                            obj.IEStaff1NameLC = dtEmpnoList[0].EmpFirstNameLC + " " + dtEmpnoList[0].EmpLastNameLC;
                        }
                    }
                    else
                    {
                        obj.IEStaff1NameEN = "";
                        obj.IEStaff1NameLC = "";
                    }
                    if (sParam.dtInstallationManagement.IEStaffEmpNo2 != null)
                    {
                        dtEmpnoList = empHandler.GetEmployeeNameByEmpNo(sParam.dtInstallationManagement.IEStaffEmpNo2);
                        if (dtEmpnoList != null && dtEmpnoList.Count > 0)
                        {
                            obj.IEStaff2NameEN = dtEmpnoList[0].EmpFirstNameEN + " " + dtEmpnoList[0].EmpLastNameEN;
                            obj.IEStaff2NameLC = dtEmpnoList[0].EmpFirstNameLC + " " + dtEmpnoList[0].EmpLastNameLC;
                        }
                    }
                    else
                    {
                        obj.IEStaff2NameEN = "";
                        obj.IEStaff2NameLC = "";
                    }

                    List<tbt_InstallationPOManagement> dtInstallationPOManagementList = new List<tbt_InstallationPOManagement>();
                    dtInstallationPOManagementList = handler.GetTbt_InstallationPOManagementData(sParam.dtInstallationManagement.MaintenanceNo);

                    if (dtInstallationPOManagementList != null && dtInstallationPOManagementList.Count > 0)
                    {
                        ICommonContractHandler comHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                        foreach (tbt_InstallationPOManagement dtInstallationPOManagement in dtInstallationPOManagementList)
                        {
                            List<tbm_SubContractor> dtSubContractorList = comHandler.GetTbm_SubContractorData(dtInstallationPOManagement.SubcontractorCode);
                            if (dtSubContractorList != null && dtSubContractorList.Count > 0)
                            {
                                obj.SubcontractorListEN = obj.SubcontractorListEN + "- Subcontractor name: " + dtSubContractorList[0].SubContractorNameEN
                                    + " , Subcontractor group name: " + dtInstallationPOManagement.SubcontractorGroupName
                                    + " , Last installation fee: " + ((dtInstallationPOManagement.LastInstallationFee == null) ? "0.00" : Convert.ToDecimal(dtInstallationPOManagement.LastInstallationFee.ToString()).ToString("#,##0.00")) + " \r\n";
                                obj.SubcontractorListLC = obj.SubcontractorListLC + "- ชื่อผู้รับเหมาติดตั้ง: " + dtSubContractorList[0].SubContractorNameLC
                                    + " , ชื่อกลุ่มผู้รับเหมาติดตั้ง: " + dtInstallationPOManagement.SubcontractorGroupName
                                    + " , ค่าติดตั้งล่าสุด: " + ((dtInstallationPOManagement.LastInstallationFee == null) ? "0.00" : Convert.ToDecimal(dtInstallationPOManagement.LastInstallationFee.ToString()).ToString("#,##0.00")) + " \r\n";
                            }
                        }
                    }
                }

                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    obj.ContractTargetNameLC = sParam.dtRentalContractBasic.ContractTargetNameLC;
                    obj.ContractTargetNameEN = sParam.dtRentalContractBasic.ContractTargetNameEN;
                    obj.SiteNameLC = sParam.dtRentalContractBasic.SiteNameLC;
                    obj.SiteNameEN = sParam.dtRentalContractBasic.SiteNameEN;
                    obj.ProductNameLC = sParam.dtRentalContractBasic.ProductNameLC;
                    obj.ProductNameEN = sParam.dtRentalContractBasic.ProductNameEN;
                    obj.OperationOfficeNameLC = sParam.dtRentalContractBasic.OperationOfficeNameLC;
                    obj.OperationOfficeNameEN = sParam.dtRentalContractBasic.OperationOfficeNameEN;
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    obj.ContractTargetNameLC = sParam.dtSaleContractBasic.ContractTargetNameLC;
                    obj.ContractTargetNameEN = sParam.dtSaleContractBasic.ContractTargetNameEN;
                    obj.SiteNameLC = sParam.dtSaleContractBasic.SiteNameLC;
                    obj.SiteNameEN = sParam.dtSaleContractBasic.SiteNameEN;
                    obj.ProductNameLC = sParam.dtSaleContractBasic.ProductNameLC;
                    obj.ProductNameEN = sParam.dtSaleContractBasic.ProductNameEN;
                    obj.OperationOfficeNameLC = sParam.dtSaleContractBasic.OperationOfficeNameLC;
                    obj.OperationOfficeNameEN = sParam.dtSaleContractBasic.OperationOfficeNameEN;
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    obj.ContractTargetNameLC = sParam.ISS090_Session.dtProject.ProjectPurchaserNameLC;
                    obj.ContractTargetNameEN = sParam.ISS090_Session.dtProject.ProjectPurchaserNameEN;
                    obj.SiteNameLC = sParam.ISS090_Session.dtProject.ProjectName;
                    obj.SiteNameEN = sParam.ISS090_Session.dtProject.ProjectName;
                    obj.ProductNameLC = "";
                    obj.ProductNameEN = "";
                    obj.OperationOfficeNameLC = "";
                    obj.OperationOfficeNameEN = "";

                }

                obj.SenderLC = EmailSenderName.C_EMAIL_SENDER_NAME_LC;
                obj.SenderEN = EmailSenderName.C_EMAIL_SENDER_NAME_EN;

                EmailTemplateUtil util = new EmailTemplateUtil(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_INSTALL_MA);
                return util.LoadTemplate(obj);
            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
