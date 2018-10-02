
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
using SECOM_AJIS.Presentation.Contract.Controllers;

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
using System.Configuration;
using SECOM_AJIS.Presentation.Common.Service;

namespace SECOM_AJIS.Presentation.Installation.Controllers
{
    public partial class InstallationController : BaseController
    {
        /// <summary>
        /// Authority screen ISS050
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ISS050_Authority(ISS050_ScreenParameter param)
        {
            // permission

            ObjectResultData res = new ObjectResultData();
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            #region ================== Check suspend and permission ===================
            if (chandler.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_PO, FunctionID.C_FUNC_ID_OPERATE) )
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }
            #endregion
            #region //=============== Add new spec 26/04/2012 Common Contract COde for Search ==============
            
            if (String.IsNullOrEmpty(param.strContractProjectCode) && param.CommonSearch != null)
            {
                if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                    param.strContractProjectCode = param.CommonSearch.ContractCode;
                if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ProjectCode) == false)
                    param.strContractProjectCode = param.CommonSearch.ProjectCode;
            }      
            #endregion //======================================================================================
            // parameter
            //ISS050_ScreenParameter param = new ISS050_ScreenParameter();
            param.ContractProjectCodeShort = param.strContractProjectCode;

            # region //=============== Validate Business ====================================================
            dtRentalContractBasicForInstall dtRentalContractBasic = null;
            dtSaleBasic dtSaleContractBasic = null;
            if (!CommonUtil.IsNullOrEmpty(param.ContractProjectCodeShort))
            {
                CommonUtil comUtil = new CommonUtil();
                string strProjectCodeLong = comUtil.ConvertProjectCode(param.ContractProjectCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
                IProjectHandler ProjectHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                dtProjectForInstall dtProjectDataForInstall = ProjectHandler.GetProjectDataForInstall(strProjectCodeLong);
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                #region ======================== Get Contract data ==========================
                if (dtProjectDataForInstall != null)
                {
                    //param.ContractProjectCodeLong = strProjectCodeLong;
                    param.ContractProjectCodeLong = dtProjectDataForInstall.ProjectCode;
                    param.ContractProjectCodeShort = comUtil.ConvertProjectCode(param.ContractProjectCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);

                    param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_PROJECT;
                    res = ISS050_ValidateProjectError(strProjectCodeLong);
                }
                else
                {
                    string strContractCodeLong = comUtil.ConvertContractCode(param.ContractProjectCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
                    IRentralContractHandler RentalContactHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    dtRentalContractBasic = RentalContactHandler.GetRentalContractBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);
                    dtSaleContractBasic = new dtSaleBasic();

                    if (dtRentalContractBasic != null)
                    {
                        //param.ContractProjectCodeLong = strContractCodeLong;
                        param.ContractProjectCodeLong = dtRentalContractBasic.ContractCode;
                        param.ContractProjectCodeShort = comUtil.ConvertContractCode(param.ContractProjectCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                        //res = ISS050_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, param.ServiceTypeCode);
                    }
                    else
                    {
                        ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);

                        if (dtSaleContractBasic != null)
                        {
                            //param.ContractProjectCodeLong = strContractCodeLong;
                            param.ContractProjectCodeLong = dtSaleContractBasic.ContractCode;
                            param.ContractProjectCodeShort = comUtil.ConvertContractCode(param.ContractProjectCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);

                            param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                            //res = ISS050_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, param.ServiceTypeCode);
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5063, new string[] { "ContractCode" });
                            return Json(res);
                        }
                    }
                }
                #endregion

                if (res.IsError)
                {
                    res.ResultData = null;
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(param.ContractProjectCodeLong);

                if (tmpdoTbt_InstallationBasic == null)
                {
                    res.ResultData = null;
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5098, null, new string[] { "ContractCodeProjectCode" });
                    return Json(res);
                }
                else if (tmpdoTbt_InstallationBasic.Count > 0)
                {
                    List<tbt_InstallationManagement> tmpdoInstallationManagement = handler.GetTbt_InstallationManagementData(tmpdoTbt_InstallationBasic[0].MaintenanceNo);
                    if (tmpdoInstallationManagement == null)
                    {
                        res.ResultData = null;
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5098, null, new string[] { "ContractCodeProjectCode" });
                        return Json(res);
                    }
                    else if (tmpdoInstallationManagement.Count > 0)
                    {
                        if (tmpdoInstallationManagement[0].ManagementStatus != InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_PROCESSING
                            && tmpdoInstallationManagement[0].ManagementStatus != InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REJECTED)
                        {
                            ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                            string strManagementStatusName = comHandler.GetMiscDisplayValue(MiscType.C_INSTALL_MANAGE_STATUS, tmpdoInstallationManagement[0].ManagementStatus);
                            res.ResultData = null;
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5062, new string[] { strManagementStatusName });
                            return Json(res);
                        }
                    }

                    if (param.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    {
                        if (dtRentalContractBasic.FirstInstallCompleteFlag == true && (tmpdoTbt_InstallationBasic[0].SlipNo == null && dtRentalContractBasic.InstallationCompleteFlag == true))
                        {
                            res.ResultData = null;
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5089, null);
                            return Json(res);
                        }
                    }
                    else if (param.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        if (dtSaleContractBasic.InstallationCompleteFlag ==true && tmpdoTbt_InstallationBasic[0].SlipNo == null)
                        {
                            res.ResultData = null;
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5089,null);
                            return Json(res);
                        }
                    }
                    else if (param.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                    {
                        //no need to check for project
                    }
                }

            }
            #endregion //======================================================================================

            return InitialScreenEnvironment<ISS050_ScreenParameter>("ISS050", param,res);
            
        }

        /// <summary>
        /// Initial screen ISS050
        /// </summary>
        /// <returns></returns>
        [Initialize("ISS050")]
        public ActionResult ISS050()
        {
            //ISS050_ScreenParameter param = new ISS050_ScreenParameter();
            ISS050_ScreenParameter param = GetScreenObject<ISS050_ScreenParameter>();
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
        public ActionResult ISS050_InitialGridEmail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS050_Email", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        // InitialGridEmail
        /// <summary>
        /// Initial grid po info schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS050_InitialGridPOInfo()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS050_POInformation", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        //InitialGridAttachedFile
  
        /// <summary>
        /// Retrieve data for show in screen ISS050
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public ActionResult ISS050_RetrieveData(string strContractCode)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();           
            dtRentalContractBasicForInstall dtRentalContractBasic = new dtRentalContractBasicForInstall();
            dtSaleBasic dtSaleContractBasic = new dtSaleBasic();
            string lang = CommonUtil.GetCurrentLanguage();
            string ContractProjectCodeForShow = "";
            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            MiscTypeMappingList miscMapping = new MiscTypeMappingList();

            try
            {
                
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
                sParam.ContractProjectCodeShort = strContractCode;
                if (sParam.ISS050_Session == null)
                {
                    sParam.ISS050_Session = new ISS050_RegisterStartResumeTargetData();
                    sParam.ISS050_Session.dtRentalContractBasic = new dtRentalContractBasicForInstall();
                }

                //Check mandatory
                if (String.IsNullOrEmpty(strContractCode))
                {
                    res.ResultData = null;
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5057, new string[] { "ContractCode" }); 
                    return Json(res);
                }

                //Get rental contract data
                #region =============== Get Project,Contract data ======================
                //=============== RETRIEVE PROJECT DATA ==================
                string strProjectCodeLong = comUtil.ConvertProjectCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                IProjectHandler ProjectHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                dtProjectForInstall dtProjectDataForInstall = ProjectHandler.GetProjectDataForInstall(strProjectCodeLong);
                //========================================================
                if (dtProjectDataForInstall != null)
                {
                    sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_PROJECT;
                    
                    sParam.ISS050_Session.dtProject = dtProjectDataForInstall;

                    sParam.ContractProjectCodeLong = dtProjectDataForInstall.ProjectCode;
                    ContractProjectCodeForShow = comUtil.ConvertProjectCode(dtProjectDataForInstall.ProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    sParam.ContractProjectCodeShort = ContractProjectCodeForShow; 
                    
                    dtSaleContractBasic = null;
                    dtRentalContractBasic = null;
                    res = ISS050_ValidateProjectError(sParam.ContractProjectCodeLong);
                    if (res.IsError)
                        return Json(res);
                }
                else
                {
                    string strContractCodeLong = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                                      

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
                        sParam.ISS050_Session.dtRentalContractBasic = dtRentalContractBasic;

                        sParam.ContractProjectCodeLong = dtRentalContractBasic.ContractCode;
                        ContractProjectCodeForShow = comUtil.ConvertContractCode(dtRentalContractBasic.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        sParam.ContractProjectCodeShort = ContractProjectCodeForShow;
                        #region =========== prepare rental data ===============
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
                        #endregion
                        dtSaleContractBasic = null;
                        dtProjectDataForInstall = null;
                        res = ISS050_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, sParam.ServiceTypeCode);
                        if (res.IsError)
                            return Json(res);
                    }
                    else
                    {
                        ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);

                        if (dtSaleContractBasic != null)
                        {
                            sParam.ContractProjectCodeLong = dtSaleContractBasic.ContractCode;
                            ContractProjectCodeForShow = comUtil.ConvertContractCode(dtSaleContractBasic.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                            sParam.ContractProjectCodeShort = ContractProjectCodeForShow;

                            sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                            sParam.InstallType = MiscType.C_SALE_INSTALL_TYPE;
                            sParam.ISS050_Session.dtSale = dtSaleContractBasic;
                            #region ====================== prepare sale data ========================
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
                            #endregion
                            dtRentalContractBasic = null;
                            dtProjectDataForInstall = null;
                            res = ISS050_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, sParam.ServiceTypeCode);
                            if (res.IsError)
                                return Json(res);
                        }
                        else
                        {
                            res.ResultData = null;
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5063, new string[] { "ContractCode" }); 
                            return Json(res);
                        }
                    }

                }
                #endregion



                //************************************* GET DATA doIB doIM Email PO ************************************************

                //sParam.dtInstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractProjectCodeLong);
                sParam.dtInstallationBasic = null;
                List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractProjectCodeLong);
                if (tmpdoTbt_InstallationBasic != null)
                {
                    //==================== map misc type =========================
                    miscMapping = new MiscTypeMappingList();
                    miscMapping.AddMiscType(tmpdoTbt_InstallationBasic.ToArray());
                    chandler.MiscTypeMappingList(miscMapping);
                    //============================================================
                    sParam.dtInstallationBasic = new tbt_InstallationBasic();
                    sParam.dtInstallationBasic = tmpdoTbt_InstallationBasic[0];
                }
                if (sParam.dtInstallationBasic == null)
                {
                    res.ResultData = null;
                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5061);
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5098, null, new string[] { "ContractCodeProjectCode" }); 
                    return Json(res);
                }
                sParam.MaintenanceNo = sParam.dtInstallationBasic.MaintenanceNo;
                if (sParam.MaintenanceNo != null)
                {
                    chandler.ClearTemporaryUploadFile(sParam.MaintenanceNo);
                }
                List<tbt_InstallationManagement> tmpdoInstallationManagement = handler.GetTbt_InstallationManagementData(sParam.dtInstallationBasic.MaintenanceNo);
                if (tmpdoInstallationManagement != null)
                {
                    sParam.dtInstallationManagement = new tbt_InstallationManagement();
                    sParam.dtInstallationManagement = tmpdoInstallationManagement[0];
                }

                # region //================= start validate business=======================================
                if (sParam.dtInstallationManagement == null)
                {
                    res.ResultData = null;
                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5061);
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5098, null, new string[] { "ContractCodeProjectCode" }); 
                    return Json(res);
                }
                
                if (sParam.dtInstallationManagement.ManagementStatus != InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_PROCESSING && sParam.dtInstallationManagement.ManagementStatus != InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REJECTED)
                {
                    //////// GET DTMISCDATA VALUE DISPLAY ///////////////
                    //List<doMiscTypeCode> dtMiscTypeCode = new List<doMiscTypeCode>();
                    //dtMiscTypeCode[0].FieldName = ;
                    //chandler.GetMiscTypeCodeList(
                    /////////////////////////////////////////////////////
                   
                    ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    string strManagementStatusName = comHandler.GetMiscDisplayValue(MiscType.C_INSTALL_MANAGE_STATUS, sParam.dtInstallationManagement.ManagementStatus);
                    res.ResultData = null;
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5062, new string[] { strManagementStatusName });
                    return Json(res);
                }
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    if (dtRentalContractBasic.FirstInstallCompleteFlag == true && (sParam.dtInstallationBasic.SlipNo == null && dtRentalContractBasic.InstallationCompleteFlag == true))
                    {
                        res.ResultData = null;
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5089, null);
                        return Json(res);
                    }
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    if (dtSaleContractBasic.InstallationCompleteFlag == true && sParam.dtInstallationBasic.SlipNo == null)
                    {
                        res.ResultData = null;
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5089, null);
                        return Json(res);
                    }
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    //no need to check for project
                }
                #endregion //================= End validate business=======================================

                sParam.ListPOInfo = handler.GetTbt_InstallationPOManagementData(sParam.dtInstallationBasic.MaintenanceNo);
                if (sParam.ListPOInfo != null)
                {
                    sParam.ListPOInfo.ForEach(d => d.OldActualPOAmount = d.ActualPOAmount);
                }
                sParam.OldListPOInfo = new List<tbt_InstallationPOManagement>();


                #region ////=========================== Map Subcontractor Name =========================
                if (sParam.ListPOInfo != null)
                {
                    foreach (tbt_InstallationPOManagement data in sParam.ListPOInfo)
                    {
                        sParam.OldListPOInfo.Add(data);
                    }
                    sParam.arrayPOName = new string[sParam.ListPOInfo.Count];
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

                        //Check Amount is NULL
                        if (dataPO.ActualPOAmount == null) {
                            dataPO.ActualPOAmount = 0;
                        }
                        if (dataPO.NormalSubPOAmount == null) {
                            dataPO.NormalSubPOAmount = 0;
                        }

                        int IndexSubcon = lstSubconName.FindIndex(delegate(tbm_SubContractor s) { return s.SubContractorCode.Trim() == dataPO.SubcontractorCode.Trim(); });
                        if (IndexSubcon >= 0)
                        {
                            sParam.arrayPOName[sParam.ListPOInfo.FindIndex(delegate(tbt_InstallationPOManagement s) { return s.SubcontractorCode.Trim() == dataPO.SubcontractorCode.Trim(); })] = lstSubconName[IndexSubcon].SubContractorName;
                        }
                        else
                        {
                            sParam.arrayPOName[sParam.ListPOInfo.FindIndex(delegate(tbt_InstallationPOManagement s) { return s.SubcontractorCode.Trim() == dataPO.SubcontractorCode.Trim(); })] = "";
                        }
                            //sParam.ListPOName[sParam.ListPOInfo.FindIndex(delegate(tbt_InstallationPOManagement s) { return s.SubcontractorCode == dataPO.SubcontractorCode; })].SubcontractorName = lstSubconName[lstSubconName.FindIndex(delegate(tbm_SubContractor s) { return s.SubContractorCode == dataPO.SubcontractorCode; })].SubContractorName;
                        
                    }
                }
                #endregion ////============================================================================

                List<tbt_InstallationEmail> ListEmail = handler.GetTbt_InstallationEmailData(sParam.dtInstallationBasic.MaintenanceNo);
                /////////////////// RETRIEVE EMAIL ////////////////////////
                ISS050_DOEmailData TempEmail;
                List<ISS050_DOEmailData> listAddEmail = new List<ISS050_DOEmailData>();
                if (ListEmail != null)
                {
                    foreach (tbt_InstallationEmail dataEmail in ListEmail)
                    {
                        TempEmail = new ISS050_DOEmailData();
                        TempEmail.EmailAddress = dataEmail.EmailNoticeTarget;
                        listAddEmail.Add(TempEmail);
                    }
                }

                sParam.ListDOEmail = listAddEmail;
                
                //********************************************************************************************************************

                
                if (res.IsError)
                    return Json(res);

                //---- Keep ContractCode ----- Add by Teerapong S. 26/Apr/2012 =======================
                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = sParam.ContractProjectCodeShort;
                sParam.CommonSearch = new ScreenParameter.CommonSearchDo();
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE || sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    sParam.CommonSearch.ContractCode = sParam.ContractProjectCodeShort;
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    sParam.CommonSearch.ProjectCode = sParam.ContractProjectCodeShort;
                }
                //====================================================================================

                UpdateScreenObject(sParam);

                ISS050_RegisterStartResumeTargetData result = new ISS050_RegisterStartResumeTargetData();
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
                result.ContractProjectCodeForShow = ContractProjectCodeForShow;
                sParam.ContractProjectCodeShort = ContractProjectCodeForShow;
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
        public ActionResult ISS050_GetInstallEmail(string strEmail)
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
                    List<ISS050_DOEmailData> listEmail;
                    ISS050_ScreenParameter session = GetScreenObject<ISS050_ScreenParameter>();
                    ISS050_DOEmailData doISS050EmailADD = new ISS050_DOEmailData();                 

                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS050_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    doISS050EmailADD.EmpNo = emailList[0].EmpNo;
                    doISS050EmailADD.EmailAddress = emailList[0].EmailAddress;

                    if (listEmail.FindAll(delegate(ISS050_DOEmailData s) { return s.EmailAddress == doISS050EmailADD.EmailAddress; }).Count() == 0)
                        listEmail.Add(doISS050EmailADD);
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0107, new string[] { strEmail });
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
        /// Validate and add PO information to session parameter
        /// </summary>
        /// <param name="POInfo"></param>
        /// <returns></returns>
        public ActionResult ISS050_AddPOInfo(tbt_InstallationPOManagement POInfo)
        {           

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                #region ////======================= VALIDATE REQUIRE FIELD ==============================
                if (POInfo.SubcontractorCode == null)
                {                                      
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "SubcontractorCode",
                                                    "lblSubContractorName",
                                                    "SubcontractorCode");
                }
                if (POInfo.NormalSubPOAmount == null)
                {                    
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "NormalSubPOAmount",
                                                    "lblNormalSubContractorPOAmount",
                                                    "NormalSubPOAmount");
                }
                if (POInfo.ActualPOAmount == null)
                {                    
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "ActualPOAmount",
                                                    "lblActualPOAmount",
                                                    "ActualPOAmount");
                }
                if (POInfo.ExpectInstallStartDate == null)
                {                    
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "ExpectInstallStartDate",
                                                    "lblExpectInstallStartDate",
                                                    "ExpectInstallStartDate");
                }
                if (POInfo.ExpectInstallCompleteDate == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "ExpectInstallCompleteDate",
                                                    "lblExpectInstallCompleteDate",
                                                    "ExpectInstallCompleteDate");
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);

                if (res.IsError)
                    return Json(res);
                #endregion ////=============================================================================


                List<tbt_InstallationPOManagement> listPOInfo;
                ISS050_ScreenParameter session = GetScreenObject<ISS050_ScreenParameter>();
                    tbt_InstallationPOManagement doISS050POInfoADD = new tbt_InstallationPOManagement();

                    if (session.ListPOInfo == null)
                        listPOInfo = new List<tbt_InstallationPOManagement>();
                    else
                        listPOInfo = session.ListPOInfo;

                    //doISS050POInfoADD.SubcontractorName = POInfo.SubcontractorName;
                    doISS050POInfoADD.SubcontractorCode = POInfo.SubcontractorCode;
                    doISS050POInfoADD.SubcontractorGroupName = POInfo.SubcontractorGroupName;
                    doISS050POInfoADD.NormalSubPOAmount = POInfo.NormalSubPOAmount;
                    doISS050POInfoADD.ActualPOAmount = POInfo.ActualPOAmount;
                    doISS050POInfoADD.PONo = POInfo.PONo;
                    doISS050POInfoADD.ExpectInstallStartDate = POInfo.ExpectInstallStartDate;
                    doISS050POInfoADD.ExpectInstallCompleteDate = POInfo.ExpectInstallCompleteDate;

                    if (listPOInfo.FindAll(delegate(tbt_InstallationPOManagement s) { return s.SubcontractorCode.Trim() == doISS050POInfoADD.SubcontractorCode.Trim(); }).Count() == 0)
                        listPOInfo.Add(doISS050POInfoADD);
                    else
                    {
                        ICommonContractHandler hand = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                        List<tbm_SubContractor> SubConList2 = hand.GetTbm_SubContractorData(POInfo.SubcontractorCode);
                        
                        string StrSubContractTorName = "";
                        if (SubConList2.Count > 0)
                        {
                            StrSubContractTorName = SubConList2[0].SubContractorName;
                        }

                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5059, new string[] { StrSubContractTorName });
                        return Json(res);
                    }
                    session.ListPOInfo = listPOInfo;
                    UpdateScreenObject(session);

                    res.ResultData = doISS050POInfoADD;
            }
            catch (Exception ex)
            {                
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            
            return Json(res);
        }

        /// <summary>
        /// Update PO information in session parameter
        /// </summary>
        /// <param name="POInfo"></param>
        /// <returns></returns>
        public ActionResult ISS050_UpdatePOInfo(tbt_InstallationPOManagement POInfo)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                #region ////======================= VALIDATE REQUIRE FIELD ==============================
                if (POInfo.SubcontractorCode == null)
                {                    
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "SubcontractorCode",
                                                    "lblSubContractorCode",
                                                    "SubcontractorCode");
                }
                if (POInfo.NormalSubPOAmount == null)
                {                    
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "NormalSubPOAmount",
                                                    "lblNormalSubContractorPOAmount",
                                                    "NormalSubPOAmount");
                }
                if (POInfo.ActualPOAmount == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "ActualPOAmount",
                                                    "lblActualPOAmount",
                                                    "ActualPOAmount");
                }
                if (POInfo.ExpectInstallStartDate == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "ExpectInstallStartDate",
                                                    "lblExpectInstallStartDate",
                                                    "ExpectInstallStartDate");
                }
                if (POInfo.ExpectInstallCompleteDate == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "ExpectInstallCompleteDate",
                                                    "lblExpectInstallCompleteDate",
                                                    "ExpectInstallCompleteDate");
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);

                if (res.IsError)
                    return Json(res);
                #endregion ////=============================================================================


                List<tbt_InstallationPOManagement> listPOInfo;
                ISS050_ScreenParameter session = GetScreenObject<ISS050_ScreenParameter>();
                tbt_InstallationPOManagement doISS050POInfoADD = new tbt_InstallationPOManagement();

                if (session.ListPOInfo == null)
                    listPOInfo = new List<tbt_InstallationPOManagement>();
                else
                    listPOInfo = session.ListPOInfo;

                //doISS050POInfoADD.SubcontractorName = POInfo.SubcontractorName;
                doISS050POInfoADD.SubcontractorCode = POInfo.SubcontractorCode;
                doISS050POInfoADD.SubcontractorGroupName = POInfo.SubcontractorGroupName;
                doISS050POInfoADD.NormalSubPOAmount = POInfo.NormalSubPOAmount;
                doISS050POInfoADD.ActualPOAmount = POInfo.ActualPOAmount;
                doISS050POInfoADD.PONo = POInfo.PONo;
                doISS050POInfoADD.ExpectInstallStartDate = POInfo.ExpectInstallStartDate;
                doISS050POInfoADD.ExpectInstallCompleteDate = POInfo.ExpectInstallCompleteDate;

                if (listPOInfo.FindAll(delegate(tbt_InstallationPOManagement s) { return s.SubcontractorCode.Trim() == doISS050POInfoADD.SubcontractorCode.Trim(); }).Count() == 0)
                    listPOInfo.Add(doISS050POInfoADD);
                else
                {
                    //listPOInfo[listPOInfo.FindIndex(delegate(tbt_InstallationPOManagement s) { return s.SubcontractorCode == doISS050POInfoADD.SubcontractorCode; })] = doISS050POInfoADD;
                    int indexPO = listPOInfo.FindIndex(delegate(tbt_InstallationPOManagement s) { return s.SubcontractorCode.Trim() == doISS050POInfoADD.SubcontractorCode.Trim(); });
                                        
                    listPOInfo[indexPO].SubcontractorGroupName = doISS050POInfoADD.SubcontractorGroupName;
                    listPOInfo[indexPO].NormalSubPOAmount = doISS050POInfoADD.NormalSubPOAmount;
                    listPOInfo[indexPO].ActualPOAmount = doISS050POInfoADD.ActualPOAmount;
                    listPOInfo[indexPO].PONo = doISS050POInfoADD.PONo;
                    listPOInfo[indexPO].ExpectInstallStartDate = doISS050POInfoADD.ExpectInstallStartDate;
                    listPOInfo[indexPO].ExpectInstallCompleteDate = doISS050POInfoADD.ExpectInstallCompleteDate;
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG5059);
                    //return Json(res);
                }
                session.ListPOInfo = listPOInfo;
                UpdateScreenObject(session);

                res.ResultData = doISS050POInfoADD;
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }


            return Json(res);
        }
        /// <summary>
        /// Validate register data
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS050_ValidateRegisterData()
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

        public ActionResult ISS050_Validate(ISS050_RegisterData data)
        {
            ObjectResultData res = new ObjectResultData();

            return Json(res);
        }
        /// <summary>
        /// Validate email
        /// </summary>
        /// <param name="doISS050Email"></param>
        /// <returns></returns>
        public ActionResult ValidateEmail_ISS050(ISS050_DOEmailData doISS050Email)
        {
            List<dtGetEmailAddress> dtEmail;
            ISS050_DOEmailData doISS050EmailADD;
            IEmployeeMasterHandler employeeMasterHandler;
            ObjectResultData res = new ObjectResultData();
            ISS050_ScreenParameter session;
            employeeMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            dtEmail = employeeMasterHandler.GetEmailAddress(doISS050Email.EmpFirstNameEN, doISS050Email.EmailAddress, doISS050Email.OfficeCode, doISS050Email.DepartmentCode);
            
            try
            {
                session = GetScreenObject<ISS050_ScreenParameter>();
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                    {
                        res.ResultData = false;
                        return Json(res);
                    }
                }
                                
                if (dtEmail != null)
                {
                    if (dtEmail.Count() == 0)
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, doISS050Email.EmailAddress);
                    else
                    {
                        doISS050EmailADD = new ISS050_DOEmailData();
                        doISS050EmailADD.EmpNo = dtEmail[0].EmpNo;
                        doISS050EmailADD.EmailAddress = doISS050Email.EmailAddress;
                        session.DOEmail = doISS050EmailADD;

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
        /// Upload attach file
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS050_Upload()
        {
            ViewBag.K = GetCurrentKey();
            return View();
        }        


        
        /// <summary>
        /// Get data for installation type combobox
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS050_GetMiscInstallationtype(string strFieldName)
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
        /// Get email data
        /// </summary>
        /// <param name="doCTS053Email"></param>
        /// <returns></returns>
        public ActionResult GetEmail_ISS050(ISS050_DOEmailData doCTS053Email)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            ISS050_DOEmailData doISS050EmailADD;
            List<ISS050_DOEmailData> listEmail;
            ISS050_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS050_ScreenParameter>();
                doISS050EmailADD = new ISS050_DOEmailData();
                if (session.ListDOEmail == null)
                    listEmail = new List<ISS050_DOEmailData>();
                else
                    listEmail = session.ListDOEmail;

                doISS050EmailADD.EmpNo = session.DOEmail.EmpNo;
                doISS050EmailADD.EmailAddress = doCTS053Email.EmailAddress;

                if (listEmail.FindAll(delegate(ISS050_DOEmailData s) { return s.EmpNo == doISS050EmailADD.EmpNo; }).Count() == 0)
                    listEmail.Add(doISS050EmailADD);

                session.DOEmail = null;
                session.ListDOEmail = listEmail;
                res.ResultData = CommonUtil.ConvertToXml<ISS050_DOEmailData>(session.ListDOEmail, "Contract\\CTS053Email", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get email list data
        /// </summary>
        /// <param name="listEmailAdd"></param>
        /// <returns></returns>        
        public ActionResult GetEmailList_ISS050(List<ISS050_DOEmailData> listEmailAdd)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            List<ISS050_DOEmailData> listEmail;
            List<ISS050_DOEmailData> listNewEmail;
            ISS050_ScreenParameter session;

            try
            {
                listNewEmail = new List<ISS050_DOEmailData>();
                if (listEmailAdd != null)
                {
                    session = GetScreenObject<ISS050_ScreenParameter>();
                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS050_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    foreach (var item in listEmailAdd)
                    {
                        if (listEmail.FindAll(delegate(ISS050_DOEmailData s) { return s.EmailAddress == item.EmailAddress; }).Count() == 0)
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
        /// Remove email and send data to show in grid email
        /// </summary>
        /// <param name="doISS050Email"></param>
        /// <returns></returns>
        public ActionResult RemoveMailClick_ISS050(ISS050_DOEmailData doISS050Email)
        {
            List<ISS050_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS050_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS050_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate(ISS050_DOEmailData s) { return s.EmailAddress == doISS050Email.EmailAddress; });
                if (listEmailDelete.Count() != 0)
                    session.ListDOEmail.Remove(listEmailDelete[0]);

                res.ResultData = CommonUtil.ConvertToXml<ISS050_DOEmailData>(session.ListDOEmail, "Installation\\ISS050Email", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); ;
            }

            return Json(res);
        }
        /// <summary>
        /// Remove email from session parameter
        /// </summary>
        /// <param name="doISS050Email"></param>
        /// <returns></returns>
        public ActionResult ISS050_RemoveMailClick(ISS050_DOEmailData doISS050Email)
        {
            List<ISS050_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS050_ScreenParameter session;
            try
            {
                session = GetScreenObject<ISS050_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate(ISS050_DOEmailData s) { return s.EmailAddress == doISS050Email.EmailAddress; });
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
        /// Remove PO information from session parameter
        /// </summary>
        /// <param name="doISS050POInfo"></param>
        /// <returns></returns>
        public ActionResult ISS050_RemovePOInfoClick(tbt_InstallationPOManagement doISS050POInfo)
        {
            List<tbt_InstallationPOManagement> listPOInfoDelete;
            ObjectResultData res = new ObjectResultData();
            ISS050_ScreenParameter session;
            try
            {
                session = GetScreenObject<ISS050_ScreenParameter>();
                listPOInfoDelete = session.ListPOInfo.FindAll(delegate(tbt_InstallationPOManagement s) { return s.PONo == doISS050POInfo.PONo; });
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
        /// Register installation PO data
        /// </summary>
        /// <param name="screenInput"></param>
        /// <param name="IEStaffEmpNo1"></param>
        /// <param name="IEStaffEmpNo2"></param>
        /// <returns></returns>
        public ActionResult ISS050_RegisterData(ISS050_ScreenInput screenInput, string IEStaffEmpNo1, string IEStaffEmpNo2)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;

            try
            {
                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();

                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                #region //////////////////// VALIDATE SUSPEND ///////////////////////
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_PO, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                #endregion /////////////////////////////////////////////////////////////

                //================= Teerapong S. 15/10/2012 =======================
                List<tbt_InstallationBasic> doValidate_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractProjectCodeLong);

                if ((sParam.dtInstallationBasic != null && doValidate_InstallationBasic != null && DateTime.Compare(sParam.dtInstallationBasic.UpdateDate.Value, doValidate_InstallationBasic[0].UpdateDate.Value) != 0))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    return Json(res);
                }
                else
                {
                    List<tbt_InstallationManagement> doValidate_InstallationManagement = handler.GetTbt_InstallationManagementData(sParam.dtInstallationBasic.MaintenanceNo);
                    if ((sParam.dtInstallationManagement != null && doValidate_InstallationManagement != null && DateTime.Compare(sParam.dtInstallationManagement.UpdateDate.Value, doValidate_InstallationManagement[0].UpdateDate.Value) != 0))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                        return Json(res);
                    }
                }
                //=================================================================

                if (!CommonUtil.IsNullOrEmpty(IEStaffEmpNo1))
                {
                    List<tbm_Employee> tbtEmp = masterHandler.GetActiveEmployee(IEStaffEmpNo1);
                    if (tbtEmp.Count <= 0)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, IEStaffEmpNo1);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { IEStaffEmpNo1 }, new string[] { "IEStaffEmpNo1" });
                        return Json(res);
                    }
                }
                if (!CommonUtil.IsNullOrEmpty(IEStaffEmpNo2))
                {
                    List<tbm_Employee> tbtEmp = masterHandler.GetActiveEmployee(IEStaffEmpNo2);
                    if (tbtEmp.Count <= 0)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, IEStaffEmpNo2);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { IEStaffEmpNo2 }, new string[] { "IEStaffEmpNo2" });
                        return Json(res);
                    }
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    //===== change to check in issue action ============
                    ////================ UPDATE InstallationBasic ======================
                    //if (sParam.dtInstallationBasic.InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED)
                    //{
                    //    sParam.dtInstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_REGISTERED;
                    //    handler.UpdateTbt_InstallationBasic(sParam.dtInstallationBasic);
                    //}
                    //================================================================
                    #region //================ UPDATE InstallationManagement ======================
                    sParam.dtInstallationManagement.ManagementStatus = InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_PROCESSING;
                    sParam.dtInstallationManagement.IEStaffEmpNo1 = screenInput.IEStaffEmpNo1;
                    sParam.dtInstallationManagement.IEStaffEmpNo2 = screenInput.IEStaffEmpNo2;
                    sParam.dtInstallationManagement.POMemo = screenInput.POMemo;
                    handler.UpdateTbt_InstallationManagement(sParam.dtInstallationManagement);
                    #endregion //================================================================
                    #region /////////////////// INSERT MEMO ////////////////////////
                    if (!CommonUtil.IsNullOrEmpty(screenInput.POMemo))
                    {
                        tbt_InstallationMemo doMemo = new tbt_InstallationMemo();
                        doMemo.ContractProjectCode = sParam.ContractProjectCodeLong;
                        doMemo.ReferenceID = sParam.dtInstallationBasic.MaintenanceNo;
                        doMemo.ObjectID = ScreenID.C_SCREEN_ID_INSTALL_PO;
                        doMemo.Memo = screenInput.POMemo;
                        doMemo.OfficeCode = CommonUtil.dsTransData.dtUserData.MainOfficeCode;
                        doMemo.DepartmentCode = CommonUtil.dsTransData.dtUserData.MainDepartmentCode;
                        //doMemo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //doMemo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        handler.InsertTbt_InstallationMemo(doMemo);
                    }
                    #endregion /////////////////////////////////////////////////////////
                    #region ///////////////////// INSERT POManagement /////////////////////////////////
                    if (sParam.ListPOInfo != null)
                    {
                        int countReportOrder = 0;
                        foreach (tbt_InstallationPOManagement dataPOManagement in sParam.ListPOInfo)
                        {
                            // Pachara S. 05102016 ADD FixCurrency
                            dataPOManagement.NormalSubPOAmountCurrencyType = "1";
                            dataPOManagement.ActualPOAmountCurrencyType = "1";
                            dataPOManagement.MaintenanceNo = sParam.dtInstallationBasic.MaintenanceNo;
                            dataPOManagement.PaidFlag = false;
                            if (CommonUtil.IsNullOrEmpty(dataPOManagement.CreateDate))
                            {
                                dataPOManagement.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                dataPOManagement.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            }
                            if (sParam.OldListPOInfo != null)
                            {
                                var oldPOInfo = sParam.OldListPOInfo.FirstOrDefault(d => d.SubcontractorCode.Trim() == dataPOManagement.SubcontractorCode.Trim());
                                if (oldPOInfo == null)
                                {
                                    countReportOrder++;
                                    dataPOManagement.ReportOrder = countReportOrder;
                                    dataPOManagement.POAmountRevision = 0;
                                    //dataPOManagement.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    //dataPOManagement.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    handler.InsertTbt_InstallationPOManagement(dataPOManagement);
                                }
                                else
                                {
                                    countReportOrder++;
                                    dataPOManagement.ReportOrder = countReportOrder;
                                    if (oldPOInfo.OldActualPOAmount != dataPOManagement.ActualPOAmount)
                                    {
                                        dataPOManagement.POAmountRevision = (oldPOInfo.POAmountRevision ?? 0) + 1;
                                    }
                                    //dataPOManagement.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    //dataPOManagement.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    handler.UpdateTbt_InstallationPOManagement(dataPOManagement);
                                }
                            }
                            else
                            {
                                countReportOrder++;
                                dataPOManagement.ReportOrder = countReportOrder;
                                dataPOManagement.POAmountRevision = 0;
                                //dataPOManagement.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                //dataPOManagement.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                handler.InsertTbt_InstallationPOManagement(dataPOManagement);
                            }
                        }

                    }
                    #endregion ///////////////////////////////////////////////////////////////////////////
                    //==================== INSERT EMAIL 15/08/2012 ==========================
                    handler.DeleteTbt_InstallationEmail(sParam.MaintenanceNo);
                    if (sParam.ListDOEmail != null)
                    {
                        foreach (ISS050_DOEmailData dataEmail in sParam.ListDOEmail)
                        {
                            tbt_InstallationEmail doEmail = new tbt_InstallationEmail();
                            doEmail.ReferenceID = sParam.MaintenanceNo;
                            doEmail.EmailNoticeTarget = dataEmail.EmailAddress;
                            doEmail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            doEmail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            handler.InsertTbt_InstallationEmail(doEmail);
                        }
                    }
                    //=======================================================================
                    //=========================== SEND EMAIL ================================
                    #region Send Email

                    //if (sParam.ListDOEmail != null)
                    //{

                    //    doEmailTemplate template = ISS050_GenerateMailRegisterInstallationPO(sParam);
                    //    if (template != null)
                    //    {
                    //        foreach (ISS050_DOEmailData e in sParam.ListDOEmail)
                    //        {
                    //            doEmailProcess mail = new doEmailProcess()
                    //            {
                    //                MailTo = e.EmailAddress,
                    //                Subject = template.TemplateSubject,
                    //                Message = template.TemplateContent
                    //            };
                    //            chandler.SendMail(mail);
                    //        }
                    //    }
                    //}
                    SendMailObject obj = new SendMailObject();
                    if (sParam.ListDOEmail != null)
                    {
                        obj.EmailList = new List<doEmailProcess>();
                        doEmailTemplate template = ISS050_GenerateMailRegisterInstallationPO(sParam);
                        if (template != null)
                        {
                            foreach (ISS050_DOEmailData e in sParam.ListDOEmail)
                            {
                                doEmailProcess mail = new doEmailProcess()
                                {
                                    MailTo = e.EmailAddress,
                                    Subject = template.TemplateSubject,
                                    Message = template.TemplateContent
                                };
                                obj.EmailList.Add(mail);
                            }
                            if (!CommonUtil.IsNullOrEmpty(sParam.ListDOEmail) && sParam.ListDOEmail.Count > 0)
                            {
                                System.Threading.Thread t = new System.Threading.Thread(SendMail);
                                t.Start(obj);
                            }
                        }

                    }

                    #endregion
                    //=======================================================================
                    #region //================= Update Attach ======================
                    List<tbt_AttachFile> tmpFileList = chandler.GetTbt_AttachFile(sParam.dtInstallationBasic.MaintenanceNo, null, null);
                    if (tmpFileList.Count > 0)
                    {
                        chandler.UpdateFlagAttachFile(AttachmentModule.Installation, sParam.dtInstallationBasic.MaintenanceNo, sParam.dtInstallationBasic.MaintenanceNo);
                        List<tbt_AttachFile> fileList = chandler.GetTbt_AttachFile(sParam.dtInstallationBasic.MaintenanceNo, null, true);
                        foreach (tbt_AttachFile file in fileList)
                        {
                            int? attachID = (int?)Convert.ToInt32(file.AttachFileID);
                            List<tbt_InstallationAttachFile> InsAttachFileList = handler.GetTbt_InstallationAttachFile(attachID, sParam.dtInstallationBasic.MaintenanceNo, null);
                            if (InsAttachFileList == null)
                            {
                                tbt_InstallationAttachFile doTbt_InstallAttachFile = new tbt_InstallationAttachFile();
                                doTbt_InstallAttachFile.MaintenanceNo = sParam.dtInstallationBasic.MaintenanceNo;
                                doTbt_InstallAttachFile.AttachFileID = file.AttachFileID;
                                doTbt_InstallAttachFile.ObjectID = ScreenID.C_SCREEN_ID_INSTALL_PO;
                                handler.InsertTbt_InstallationAttachFile(doTbt_InstallAttachFile);
                            }
                        }
                    }
                    #endregion //======================================================
                    scope.Complete();
                    
                }

                ISS050_RegisterStartResumeTargetData result = new ISS050_RegisterStartResumeTargetData();
                
                res.ResultData = result;
                
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Send Email Installation PO
        /// </summary>
        /// <param name="o"></param>
        public static void SendMail(object o)
        {
            try
            {
                SendMailObject obj = o as SendMailObject;
                if (obj == null)
                    return;

                if (obj.EmailList != null)
                {
                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    foreach (doEmailProcess mail in obj.EmailList)
                    {
                        chandler.SendMail(mail);
                    }
                }
            }
            catch
            {
            }
        }

        public class SendMailObject
        {
            public List<doEmailProcess> EmailList { get; set; }
        }
        /// <summary>
        /// Generate Email Template
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        private doEmailTemplate ISS050_GenerateMailRegisterInstallationPO(ISS050_ScreenParameter sParam)
        {
            try
            {
                doRegisterInstallationPOMailObject obj = new doRegisterInstallationPOMailObject();
                #region ============ prepare data ==================
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    obj.ContractTargetNameLC = sParam.ISS050_Session.dtRentalContractBasic.ContractTargetNameLC;
                    obj.ContractTargetNameEN = sParam.ISS050_Session.dtRentalContractBasic.ContractTargetNameEN;
                    obj.SiteNameLC = sParam.ISS050_Session.dtRentalContractBasic.SiteNameLC;
                    obj.SiteNameEN = sParam.ISS050_Session.dtRentalContractBasic.SiteNameEN;
                    obj.ProductNameLC = sParam.ISS050_Session.dtRentalContractBasic.ProductNameLC;
                    obj.ProductNameEN = sParam.ISS050_Session.dtRentalContractBasic.ProductNameEN;
                    obj.ContractOfficeNameLC = sParam.ISS050_Session.dtRentalContractBasic.ContractOfficeNameLC;
                    obj.ContractOfficeNameEN = sParam.ISS050_Session.dtRentalContractBasic.ContractOfficeNameEN;
                    obj.Salesman1NameLC = sParam.ISS050_Session.dtRentalContractBasic.SalesmanLC1;
                    obj.Salesman1NameEN = sParam.ISS050_Session.dtRentalContractBasic.SalesmanEN1;
                    obj.Salesman2NameLC = sParam.ISS050_Session.dtRentalContractBasic.SalesmanLC2;
                    obj.Salesman2NameEN = sParam.ISS050_Session.dtRentalContractBasic.SalesmanEN2;
                    obj.OperationOfficeNameLC = sParam.ISS050_Session.dtRentalContractBasic.OperationOfficeNameLC;
                    obj.OperationOfficeNameEN = sParam.ISS050_Session.dtRentalContractBasic.OperationOfficeNameEN;
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    obj.ContractTargetNameLC = sParam.ISS050_Session.dtSale.ContractTargetNameLC;
                    obj.ContractTargetNameEN = sParam.ISS050_Session.dtSale.ContractTargetNameEN;
                    obj.SiteNameLC = sParam.ISS050_Session.dtSale.SiteNameLC;
                    obj.SiteNameEN = sParam.ISS050_Session.dtSale.SiteNameEN;
                    obj.ProductNameLC = sParam.ISS050_Session.dtSale.ProductNameLC;
                    obj.ProductNameEN = sParam.ISS050_Session.dtSale.ProductNameEN;
                    obj.ContractOfficeNameLC = sParam.ISS050_Session.dtSale.ContractOfficeNameLC;
                    obj.ContractOfficeNameEN = sParam.ISS050_Session.dtSale.ContractOfficeNameEN;
                    obj.Salesman1NameLC = sParam.ISS050_Session.dtSale.SalesmanLC1;
                    obj.Salesman1NameEN = sParam.ISS050_Session.dtSale.SalesmanEN1;
                    obj.Salesman2NameLC = sParam.ISS050_Session.dtSale.SalesmanLC2;
                    obj.Salesman2NameEN = sParam.ISS050_Session.dtSale.SalesmanEN2;
                    obj.OperationOfficeNameLC = sParam.ISS050_Session.dtSale.OperationOfficeNameLC;
                    obj.OperationOfficeNameEN = sParam.ISS050_Session.dtSale.OperationOfficeNameEN;
                }
                obj.ContractProjectCode = sParam.ContractProjectCodeShort;
                obj.MaintenanceNo = sParam.dtInstallationBasic.MaintenanceNo;
                //obj.InstallationTypeLC = sParam.dtInstallationBasic.InstallationType;
                //obj.InstallationTypeEN = sParam.dtInstallationBasic.InstallationType;

                #region //======== GET INSTALLATION TYPE NAME =============================
                if (sParam.dtInstallationBasic.InstallationType != null)
                {
                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                    {
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_RENTAL_INSTALL_TYPE,
                            ValueCode = sParam.dtInstallationBasic.InstallationType
                        }
                    };

                    lst = chandler.GetMiscTypeCodeList(miscs);
                    if (lst.Count == 0)
                    {
                        miscs[0].FieldName = MiscType.C_SALE_INSTALL_TYPE;
                        lst = chandler.GetMiscTypeCodeList(miscs);
                    }
                    if (lst != null && lst.Count > 0)
                    {
                        obj.InstallationTypeEN = lst[0].ValueDisplayEN;
                        obj.InstallationTypeLC = lst[0].ValueDisplayLC;
                    }
                }
                else
                {
                    obj.InstallationTypeEN = "";
                    obj.InstallationTypeLC = "";
                }
                #endregion //=================================================================
                #region //======== find min max install date ============================
                obj.CountSubcontractor = sParam.ListPOInfo.Count.ToString();
                DateTime MinimumInstallDate = new DateTime();
                DateTime MaximumCompleteDate = new DateTime();
                if (sParam.ListPOInfo.Count > 0)
                {
                    MinimumInstallDate = sParam.ListPOInfo[0].ExpectInstallStartDate.Value;
                    MaximumCompleteDate = sParam.ListPOInfo[0].ExpectInstallCompleteDate.Value;
                }
                foreach(tbt_InstallationPOManagement POInfo in sParam.ListPOInfo)
                {
                    if(MinimumInstallDate > POInfo.ExpectInstallStartDate.Value)
                    {
                        MinimumInstallDate = POInfo.ExpectInstallStartDate.Value;
                    }
                    if (MaximumCompleteDate < POInfo.ExpectInstallCompleteDate.Value)
                    {
                        MaximumCompleteDate = POInfo.ExpectInstallCompleteDate.Value;
                    }
                }
                obj.MinimumExpectedInstallationStartDate = MinimumInstallDate.ToString("dd-MMM-yyyy");
                obj.MaximumExpectedInstallationCompleteDate = MaximumCompleteDate.ToString("dd-MMM-yyyy");
                #endregion ===========================================================
                obj.SenderLC = EmailSenderName.C_EMAIL_SENDER_NAME_LC;
                obj.SenderEN = EmailSenderName.C_EMAIL_SENDER_NAME_EN;
                #endregion ============================================================
                
                EmailTemplateUtil util = new EmailTemplateUtil(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_INSTALL_PO);
                return util.LoadTemplate(obj);
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Validate business contract
        /// </summary>
        /// <param name="contractData"></param>
        /// <param name="saleData"></param>
        /// <param name="strServiceTypeCode"></param>
        /// <returns></returns>
        public ObjectResultData ISS050_ValidateContractError(dtRentalContractBasicForInstall contractData, dtSaleBasic saleData, string strServiceTypeCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            string strContractCode = "";
            try
            {

                //No validate authorize for this screem Phoomsak L. 2012-05-27
                //IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                //IRentralContractHandler ReantalContracthandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                //ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                //{
                //    if (ISS050_ValidExistOffice(contractData.OperationOfficeCode) == false)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                //        return res;
                //    }
                //    strContractCode = contractData.ContractCode;
                   
                //}
                //else if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                //{
                //    if (ISS050_ValidExistOffice(saleData.OperationOfficeCode) == false)
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
        /// Validate before register installation PO
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <param name="IEStaffEmpNo1"></param>
        /// <param name="IEStaffEmpNo2"></param>
        /// <returns></returns>
        public ActionResult ISS050_ValidateBeforeRegister(string strContractProjectCode, string IEStaffEmpNo1, string IEStaffEmpNo2)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                #region =============== check suspend and permission ================
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_PO, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                #endregion ============================================================
                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                ISS050_ScreenParameter session = GetScreenObject<ISS050_ScreenParameter>();

                //================= Teerapong S. 15/10/2012 =======================     
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
                List<tbt_InstallationBasic> doValidate_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractProjectCodeLong);

                if ((sParam.dtInstallationBasic != null && doValidate_InstallationBasic != null && DateTime.Compare(sParam.dtInstallationBasic.UpdateDate.Value, doValidate_InstallationBasic[0].UpdateDate.Value) != 0))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }
                else
                {
                    List<tbt_InstallationManagement> doValidate_InstallationManagement = handler.GetTbt_InstallationManagementData(sParam.dtInstallationBasic.MaintenanceNo);
                    if ((sParam.dtInstallationManagement != null && doValidate_InstallationManagement != null && DateTime.Compare(sParam.dtInstallationManagement.UpdateDate.Value, doValidate_InstallationManagement[0].UpdateDate.Value) != 0))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    }
                }
                //=================================================================

                if(!CommonUtil.IsNullOrEmpty(IEStaffEmpNo1))
                {
                    List<tbm_Employee> tbtEmp = masterHandler.GetActiveEmployee(IEStaffEmpNo1);
                    if (tbtEmp.Count <= 0)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, IEStaffEmpNo1);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { IEStaffEmpNo1 }, new string[] { "IEStaffEmpNo1" });
                        return Json(res);
                    }
                }
                if (!CommonUtil.IsNullOrEmpty(IEStaffEmpNo2))
                {
                    List<tbm_Employee> tbtEmp = masterHandler.GetActiveEmployee(IEStaffEmpNo2);
                    if (tbtEmp.Count <= 0)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, IEStaffEmpNo2);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { IEStaffEmpNo2 }, new string[] { "IEStaffEmpNo2" });
                        return Json(res);
                    }
                }
                if (!CommonUtil.IsNullOrEmpty(IEStaffEmpNo1) && !CommonUtil.IsNullOrEmpty(IEStaffEmpNo2))
                {
                    if (IEStaffEmpNo1 == IEStaffEmpNo2)
                    {
                        res.ResultData = false;
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5102, null, new string[] { "IEStaffEmpNo1", "IEStaffEmpNo2" });
                        return Json(res);
                    }
                }
                if (CommonUtil.IsNullOrEmpty(IEStaffEmpNo1) && !CommonUtil.IsNullOrEmpty(IEStaffEmpNo2))
                {                    
                    res.ResultData = false;
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5103, null, new string[] { "IEStaffEmpNo1", "IEStaffEmpNo2" });
                    return Json(res);                    
                }


                if (session.ListPOInfo != null)
                {
                    if (session.ListPOInfo.Count == 0)
                    {                       
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5060,
                                                    "NewPhoneLineOpenDate",
                                                    "lblNewTelLineOpenDate",
                                                    "NewPhoneLineOpenDate");
                    }
                }
                else
                {                   
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS050",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5060,
                                                    "NewPhoneLineOpenDate",
                                                    "lblNewTelLineOpenDate",
                                                    "NewPhoneLineOpenDate");
                }
                                    
               

                ValidatorUtil.BuildErrorMessage(res, validator, null);

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
        public bool ISS050_ValidExistOffice(string OfficeCode)
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
        /// Validate business project
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ObjectResultData ISS050_ValidateProjectError(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try 
            {
                
                IProjectHandler handler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<string> strStatus = handler.GetProjectStatus(strProjectCode);               
                
                if(strStatus.Count > 0)
                {
                    //if (strStatus[0] == ProjectStatus.C_PROJECT_STATUS_CANCEL || strStatus[0] == ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5002);
                    //}
                    if (strStatus[0] == ProjectStatus.C_PROJECT_STATUS_CANCEL)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5014);
                    }
                    else if (strStatus[0] == ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5015);
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
        /// <summary>
        /// Validate business
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <param name="strServiceTypeCode"></param>
        /// <param name="strInstallType"></param>
        /// <returns></returns>
        public ObjectResultData ISS050_ValidateBusiness(string strContractProjectCode, string strServiceTypeCode, string strInstallType)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {

                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL || strInstallType == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    //tbt_InstallationBasic doTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(strContractProjectCode);
                    tbt_InstallationBasic doTbt_InstallationBasic = null;
                    List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(strContractProjectCode);
                    if (tmpdoTbt_InstallationBasic != null)
                    {
                        doTbt_InstallationBasic = new tbt_InstallationBasic();
                        doTbt_InstallationBasic = tmpdoTbt_InstallationBasic[0];
                    }
                    if(strInstallType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW )
                    {
                        if (doTbt_InstallationBasic == null || doTbt_InstallationBasic.InstallationStatus != InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5064);
                        }
                    }
                    else
                    {
                        if (doTbt_InstallationBasic != null && doTbt_InstallationBasic.InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5065);
                        }
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
        /// Clear all email data from session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS050_ClearInstallEmail(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS050_ScreenParameter session = GetScreenObject<ISS050_ScreenParameter>();
            session.ListDOEmail = null;
            UpdateScreenObject(session);
            
            return Json(res);
        }
        /// <summary>
        /// Clear all PO information from session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS050_ClearPOInfo(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS050_ScreenParameter session = GetScreenObject<ISS050_ScreenParameter>();
            session.ListPOInfo = null;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Load employee name from employee no
        /// </summary>
        /// <param name="MaintEmpNo"></param>
        /// <returns></returns>
        public ActionResult ISS050_LoadEmployeeName(string MaintEmpNo)
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
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { MaintEmpNo }, new string[] { "MaintEmpNo" });
                    return Json(res);
                }

                //5.3 Show employee name on screen
                res.ResultData = ISS050_getEmployeeDisplayName(MaintEmpNo);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get Employee name for display
        /// </summary>
        /// <param name="MaintEmpNo"></param>
        /// <returns></returns>
        private String ISS050_getEmployeeDisplayName(string MaintEmpNo)
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
        /// Upload attach file
        /// </summary>
        /// <param name="fileSelect"></param>
        /// <param name="DocumentName"></param>
        /// <param name="ReferKey"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult ISS050_AttachFile(HttpPostedFileBase fileSelect, string DocumentName, string ReferKey, string k)
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
                ViewBag.ReferKey = ReferKey;
            }
            catch (Exception ex)
            {
                outmsg = new MessageModel();
                outmsg.Message = ((SECOM_AJIS.Common.Models.ApplicationErrorException)(ex)).ErrorResult.Message.Message;
                outmsg.Code = CommonValue.SYSTEM_MESSAGE_CODE;
                ViewBag.ReferKey = ReferKey;
            }

            if (outmsg != null)
            {
                ViewBag.Message = outmsg.Message;
                ViewBag.MsgCode = outmsg.Code;
            }
            ViewBag.K = k;
            return View("ISS050_Upload");
        }
        /// <summary>
        /// Initial grid attach file
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS050_IntialGridAttachedDocList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS010_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// Remove attach file
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult ISS050_RemoveAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            try
            {
                int _attachID = int.Parse(AttachID);
                ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
                //commonhandler.DeleteAttachFileByID(_attachID, GetCurrentKey());
                commonhandler.DeleteAttachFileByID(_attachID, sParam.MaintenanceNo);
                handler.DeleteTbt_InstallationAttachFile(_attachID, sParam.MaintenanceNo, null);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Load data attach to show in grid attach
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS050_LoadGridAttachedDocList()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                List<dtAttachFileForGridView> lstAttachedName = new List<dtAttachFileForGridView>();
                ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();

                if (sParam != null && sParam.MaintenanceNo != null) //Add by Jutarat A. on 05022013
                    lstAttachedName = commonhandler.GetAttachFileForGridView(sParam.MaintenanceNo);                    
              
                //List<dtAttachFileForGridView> lstAttachedName = commonhandler.GetAttachFileForGridView(Session.SessionID);
                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Installation\\ISS010_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Send ReferID for get attach data
        /// </summary>
        /// <param name="sK"></param>
        /// <returns></returns>
        public ActionResult ISS050_SendAttachKey(string sK = "")
        {
            ViewBag.ReferKey = sK;
            return View("ISS050_Upload");
        }
        /// <summary>
        /// Get misc type by code
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="ValueCode"></param>
        /// <returns></returns>
        public ActionResult ISS050_GetMiscTypeByValueCode(string FieldName, string ValueCode)
        {
            ObjectResultData res = new ObjectResultData();

            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();           
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = FieldName,
                        ValueCode = ValueCode
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);

                if (lst.Count > 0)
                {
                    res.ResultData =  lst[0].ValueCodeDisplay;
                }
                else
                {
                    res.ResultData = null;
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
        /// Generate report PO and subprice
        /// </summary>
        /// <param name="strSubContracttorCode"></param>
        /// <returns></returns>
        public ActionResult ISS050_CreateReportInstallationPOandSubPrice(string strSubContracttorCode)
        { 
            ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
            IInstallationDocumentHandler hand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            // 2017.2.14 modify nakajima start
            var doc = DocumentOutput.getDocumentData("ISR050", 1, DateTime.Now.Date);
            string nameSignature = "";
            if (!CommonUtil.IsNullOrEmpty(doc))
            {
                nameSignature = doc.EmpName;
            }
            //string nameSignature = ConfigurationManager.AppSettings["NameSignature"];
            // 2017.2.14 modify nakajima end
            return File(hand.CreateReportInstallationPOandSubPrice(sParam.MaintenanceNo, strSubContracttorCode, nameSignature), "application/pdf","ISR050.pdf");   
        }
        /// <summary>
        /// Generate report installation spec complete
        /// </summary>
        /// <param name="strSubContracttorCode"></param>
        /// <returns></returns>
        public ActionResult ISS050_CreateReportInstallationSpecCompleteData(string strSubContracttorCode)
        {
            ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
            IInstallationDocumentHandler hand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            return File(hand.CreateReportInstallSpecCompleteData(sParam.MaintenanceNo, strSubContracttorCode), "application/pdf","ISR070.pdf");
        }
        /// <summary>
        /// Generate report IE check sheet
        /// </summary>
        /// <param name="strSubContracttorCode"></param>
        /// <returns></returns>
        public ActionResult ISS050_CreateReportIECheckSheet(string strSubContracttorCode)
        {
            ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
            IInstallationDocumentHandler hand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            return File(hand.CreateReportIECheckSheetData(sParam.MaintenanceNo, strSubContracttorCode), "application/pdf","ISR080.pdf");
        }
        /// <summary>
        /// Merged all report to show
        /// </summary>
        /// <param name="strSubContracttorCode"></param>
        /// <returns></returns>
        public ActionResult ISS050_CreateReportMergedAll(string strSubContracttorCode)
        {
            ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
            IInstallationDocumentHandler hand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            string nameSignature = ConfigurationManager.AppSettings["NameSignature"];
            return File(hand.CreateReportISS050MergedAll(sParam.MaintenanceNo, strSubContracttorCode, nameSignature), "application/pdf", "ISR080.pdf");
        }
        /// <summary>
        /// Clear contract code from session parameter
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public ActionResult ISS050_ClearCommonContractCode(string temp)
        {
            ObjectResultData res = new ObjectResultData();
            ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
            //---- Clear ContractCode ----- Add by Teerapong S. 26/Apr/2012 =======================
            //CommonUtil.dsTransData.dtCommonSearch.ContractCode = null;
            //CommonUtil.dsTransData.dtCommonSearch.ProjectCode = null;
            
            //====================================================================================
            
            if (sParam != null)
            {
                sParam.ContractProjectCodeShort = null;
                sParam.MaintenanceNo = null;
                sParam.strContractProjectCode = null;
                sParam.CommonSearch = new ScreenParameter.CommonSearchDo();
            }
            return Json(res);
        }
        /// <summary>
        /// Clear all attach data from session parameter
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public ActionResult ISS050_ClearAllAttach(string temp)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
            try
            {
                List<tbt_AttachFile> tmpFileList = commonhandler.GetTbt_AttachFile(sParam.dtInstallationBasic.MaintenanceNo, null, false);
                if (tmpFileList.Count > 0)
                {
                    foreach (tbt_AttachFile attachData in tmpFileList)
                    {
                        commonhandler.DeleteAttachFileByID(attachData.AttachFileID, sParam.dtInstallationBasic.MaintenanceNo);
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
        /// Process Issue 
        /// </summary>
        /// <param name="SubcontractorCode"></param>
        /// <returns></returns>
        public ActionResult ISS050_IssueButtonClick(string SubcontractorCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
                    IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    #region ====================== Update tbt_installationPOManagement =========================
                    //if (sParam.ListPOInfo != null)
                    if (sParam != null && sParam.ListPOInfo != null) //Modify by Jutarat A. on 15052013
                    {
                        foreach (tbt_InstallationPOManagement dataPOManagement in sParam.ListPOInfo)
                        {
                            if (dataPOManagement.SubcontractorCode.Trim() == SubcontractorCode.Trim())
                            {
                                if (dataPOManagement.IssuePOFlag != FlagType.C_FLAG_ON)
                                {
                                    dataPOManagement.IssuePOFlag = FlagType.C_FLAG_ON;
                                    dataPOManagement.IssuePODate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    handler.UpdateTbt_InstallationPOManagement(dataPOManagement);
                                }
                            }
                        }

                    }
                    #endregion =================================================================================
                    #region //================ UPDATE InstallationBasic ======================
                    if (sParam != null && sParam.ListPOInfo != null) //Add by Jutarat A. on 15052013
                    {
                        if (sParam.ListPOInfo.FindAll(delegate(tbt_InstallationPOManagement s) { return s.IssuePOFlag != FlagType.C_FLAG_ON; }).Count() == 0)
                        {
                            if (sParam.dtInstallationBasic.InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED)
                            {
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                                res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5094);
                                sParam.dtInstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_REGISTERED;
                                handler.UpdateTbt_InstallationBasic(sParam.dtInstallationBasic);
                            }
                        }
                    }
                    #endregion //================================================================

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
    }
}
