
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
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Inventory;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using System.ComponentModel;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using SECOM_AJIS.Common.Models.EmailTemplates;


using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;   

namespace SECOM_AJIS.Presentation.Installation.Controllers
{
    public partial class InstallationController : BaseController
    {
        /// <summary>
        /// Authority screen ISS070
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ISS070_Authority(ISS070_ScreenParameter param)
        {
            // permission

            ObjectResultData res = new ObjectResultData();
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            if (chandler.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_CANCEL, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }


            //=============== Add new spec 26/04/2012 Common Contract COde for Search ==============
            //if (CommonUtil.IsNullOrEmpty(param.strContractProjectCode) && !CommonUtil.IsNullOrEmpty(CommonUtil.dsTransData.dtCommonSearch.ContractCode))
            //{
            //    param.strContractProjectCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
            //}
            //else if (CommonUtil.IsNullOrEmpty(param.strContractProjectCode) && !CommonUtil.IsNullOrEmpty(CommonUtil.dsTransData.dtCommonSearch.ProjectCode))
            //{
            //    param.strContractProjectCode = CommonUtil.dsTransData.dtCommonSearch.ProjectCode;
            //}
            if (String.IsNullOrEmpty(param.strContractProjectCode) && param.CommonSearch != null)
            {
                if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                    param.strContractProjectCode = param.CommonSearch.ContractCode;
                if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ProjectCode) == false)
                    param.strContractProjectCode = param.CommonSearch.ProjectCode;
            }      
            //======================================================================================
            // parameter
            //ISS070_ScreenParameter param = new ISS070_ScreenParameter();
            param.ContractCodeShort = param.strContractProjectCode;
            //UpdateScreenObject(param);

            //=============== Validate Business ====================================================
            if (!CommonUtil.IsNullOrEmpty(param.ContractCodeShort))
            {
                CommonUtil comUtil = new CommonUtil();
                string strProjectCodeLong = comUtil.ConvertProjectCode(param.ContractCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
                IProjectHandler ProjectHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                dtProjectForInstall dtProjectDataForInstall = ProjectHandler.GetProjectDataForInstall(strProjectCodeLong);
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                string strContractCodeLong = null;

                if (dtProjectDataForInstall != null)
                {
                    //param.ContractCodeLong = strProjectCodeLong;
                    param.ContractCodeLong = dtProjectDataForInstall.ProjectCode;
                    param.ContractCodeShort = comUtil.ConvertProjectCode(param.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_PROJECT;
                }
                else
                {
                    strContractCodeLong = comUtil.ConvertContractCode(param.ContractCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
                    IRentralContractHandler RentalContactHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    dtRentalContractBasicForInstall dtRentalContractBasic = RentalContactHandler.GetRentalContractBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);
                    dtSaleBasic dtSaleContractBasic = new dtSaleBasic();

                    if (dtRentalContractBasic != null)
                    {
                        //param.ContractCodeLong = strContractCodeLong;
                        param.ContractCodeLong = dtRentalContractBasic.ContractCode;
                        param.ContractCodeShort = comUtil.ConvertContractCode(param.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;

                        if (ISS070_ValidExistOffice(dtRentalContractBasic.OperationOfficeCode) == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS070",
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0063,
                                        new string[] { "" },
                                        new string[] { "ContractCodeProjectCode" });
                            return Json(res);
                        }
                    }
                    else
                    {
                        ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);

                        if (dtSaleContractBasic != null)
                        {
                            //param.ContractCodeLong = strContractCodeLong;
                            param.ContractCodeLong = dtSaleContractBasic.ContractCode;
                            param.ContractCodeShort = comUtil.ConvertContractCode(param.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                            param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;

                            if (ISS070_ValidExistOffice(dtSaleContractBasic.OperationOfficeCode) == false)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS070",
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0063,
                                        new string[] { "" },
                                        new string[] { "ContractCodeProjectCode" });
                                return Json(res);
                            }


                            if ((dtSaleContractBasic.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE
                                || dtSaleContractBasic.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE)
                                && dtSaleContractBasic.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE
                                )
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                "ISS070",
                                                MessageUtil.MODULE_INSTALLATION,
                                                MessageUtil.MessageList.MSG5051,
                                                new string[] { "lblContractProjectCode" },
                                                new string[] { "ContractCodeProjectCode" });
                                return Json(res);
                            }
                            
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5063, new string[] { "ContractCodeProjectCode" });
                            return Json(res);
                        }
                    }
                }

                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(param.ContractCodeLong);

                if (tmpdoTbt_InstallationBasic == null || tmpdoTbt_InstallationBasic.Count == 0 || tmpdoTbt_InstallationBasic[0].InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                {
                    res.ResultData = null;
                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5061, null, new string[] { "ContractCodeProjectCode" });
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS070",
                                        MessageUtil.MODULE_INSTALLATION,
                                        MessageUtil.MessageList.MSG5018,
                                        new string[] { "" },
                                        new string[] { "ContractCodeProjectCode" });

                    return Json(res);
                }
                //else
                //{
                //    List<tbt_InstallationManagement> tmpdoInstallationManagement = handler.GetTbt_InstallationManagementData(tmpdoTbt_InstallationBasic[0].MaintenanceNo);

                //    if (tmpdoInstallationManagement == null || tmpdoInstallationManagement.Count == 0)
                //    {
                //        res.ResultData = null;
                //        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5061, null, new string[] { "ContractCodeProjectCode" });
                //        return Json(res);
                //    }
                //    else
                //    {
                //        if (tmpdoInstallationManagement[0].ManagementStatus != InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_PROCESSING
                //        && tmpdoInstallationManagement[0].ManagementStatus != InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REJECTED)
                //        {
                //            ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //            string strManagementStatusName = comHandler.GetMiscDisplayValue(MiscType.C_INSTALL_MANAGE_STATUS, tmpdoInstallationManagement[0].ManagementStatus);
                //            res.ResultData = null;
                //            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5062, new string[] { strManagementStatusName });
                //            return Json(res);
                //        }
                //    }
                //}

                // - Comment by Nontawat L. on 09-Jul-2014 : Phase4: 3.44 
                //else {
                //    tbt_InstallationSlip do_TbtInstallationSlip = handler.GetTbt_InstallationSlipData(tmpdoTbt_InstallationBasic[0].SlipNo);
                //    if (do_TbtInstallationSlip != null)
                //    {

                //        if (do_TbtInstallationSlip.SlipStatus == SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                //                            "ISS070",
                //                            MessageUtil.MODULE_INSTALLATION,
                //                            MessageUtil.MessageList.MSG5079,
                //                            new string[] { "" },
                //                            new string[] { "ContractCodeProjectCode" });
                //            return Json(res);
                //        }
                //    }
                //}
            }
            //======================================================================================
            
            return InitialScreenEnvironment<ISS070_ScreenParameter>("ISS070",param,res);
            
        }
        /// <summary>
        /// Initial screen ISS070
        /// </summary>
        /// <returns></returns>
        [Initialize("ISS070")]
        public ActionResult ISS070()
        {
            //ISS070_ScreenParameter param = new ISS070_ScreenParameter();
            ISS070_ScreenParameter param = GetScreenObject<ISS070_ScreenParameter>();
            if(param != null)
            {
                ViewBag.ContractProjectCode = param.ContractCodeShort;
            }
            return View();
        }

        // InitialGridEmail
        /// <summary>
        /// Initial grid email schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS070_InitialGridEmail()
        {
            return Json( CommonUtil.ConvertToXml<object>(null, "Installation\\ISS060_Email", CommonUtil.GRID_EMPTY_TYPE.VIEW) );
        }
        /// <summary>
        /// Initial grid Instrument information schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS070_InitialGridInstrumentInfo()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS030_InstrumentDetail", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }        

        /// <summary>
        /// Retrieve data for show in screen
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public ActionResult ISS070_RetrieveData(string strContractCode)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();           
            dtRentalContractBasicForInstall dtRentalContractBasic = new dtRentalContractBasicForInstall();
            dtSaleBasic dtSaleContractBasic = new dtSaleBasic();
            string lang = CommonUtil.GetCurrentLanguage();
            ISS070_RegisterStartResumeTargetData result = new ISS070_RegisterStartResumeTargetData();
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            MiscTypeMappingList miscMapping = new MiscTypeMappingList();
            string ContractProjectCodeForShow = "";
            try
            {
                
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ISS070_ScreenParameter sParam = GetScreenObject<ISS070_ScreenParameter>();
                sParam.ContractCodeShort = strContractCode;
                
                //sParam.dtRentalContractBasic = new dtRentalContractBasicForInstall();
               

                //Check mandatory
                if (String.IsNullOrEmpty(strContractCode))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5057, new string[] { "ContractCode" }, new string[] { "ContractCodeProjectCode" });
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS070",
                                        MessageUtil.MODULE_INSTALLATION,
                                        MessageUtil.MessageList.MSG5057,
                                        new string[] { "lblContractProjectCode" },
                                        new string[] { "ContractCodeProjectCode" });
                    return Json(res);
                }

                //Get rental contract data
                
                ///////////// START RETRIEVE DATA ////////////////////////
                
                string strContractCodeLong = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                sParam.ContractCodeLong = strContractCodeLong;
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                IRentralContractHandler RentalContactHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                dtRentalContractBasic = RentalContactHandler.GetRentalContractBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);

                if (res.IsError)
                    return Json(res);

                //=============== RETRIEVE PROJECT DATA ==================
                string strProjectCodeLong = comUtil.ConvertProjectCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                IProjectHandler ProjectHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                dtProjectForInstall dtProjectDataForInstall = ProjectHandler.GetProjectDataForInstall(strProjectCodeLong);
                //========================================================

                if (dtProjectDataForInstall != null)
                {
                    sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_PROJECT;
                    sParam.dtProject = dtProjectDataForInstall;
                    //sParam.ContractCodeLong = strProjectCodeLong;       
                    sParam.ContractCodeLong = dtProjectDataForInstall.ProjectCode;
                    sParam.ContractCodeShort = comUtil.ConvertProjectCode(sParam.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    dtSaleContractBasic = null;
                    dtRentalContractBasic = null;

                    ContractProjectCodeForShow = sParam.ContractCodeShort;
                }
                else                
                {

                    if (dtRentalContractBasic != null)
                    {
                    
                        sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                        sParam.InstallType = MiscType.C_RENTAL_INSTALL_TYPE;
                        sParam.dtRentalContractBasic = dtRentalContractBasic;

                        sParam.ContractCodeLong = dtRentalContractBasic.ContractCode;
                        sParam.ContractCodeShort = comUtil.ConvertContractCode(sParam.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        ContractProjectCodeForShow = sParam.ContractCodeShort;

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
                        if (ISS070_ValidExistOffice(dtRentalContractBasic.OperationOfficeCode) == false) {
                            //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS070",
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0063,
                                        new string[] { "" },
                                        new string[] { "ContractCodeProjectCode" });
                            return Json(res);
                        }
                       
                    }
                    else
                    {
                    
                        dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);
                    
                        if (dtSaleContractBasic != null)
                        {
                            sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                            sParam.InstallType = MiscType.C_SALE_INSTALL_TYPE;
                            sParam.dtSale = dtSaleContractBasic;

                            sParam.ContractCodeLong = dtSaleContractBasic.ContractCode;
                            sParam.ContractCodeShort = comUtil.ConvertContractCode(sParam.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);

                            ContractProjectCodeForShow = sParam.ContractCodeShort;

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
                            if (ISS070_ValidExistOffice(dtSaleContractBasic.OperationOfficeCode) == false)
                            {
                                //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                        "ISS070",
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0063,
                                        new string[] { "" },
                                        new string[] { "ContractCodeProjectCode" });
                                return Json(res);
                            }
                        
                        }
                        else
                        {
                            //res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5063, null, new string[] { "ContractCodeProjectCode" }); 
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS070",
                                        MessageUtil.MODULE_INSTALLATION,
                                        MessageUtil.MessageList.MSG5063,
                                        new string[] { "" },
                                        new string[] { "ContractCodeProjectCode" });
                            return Json(res);
                        }
                    }

                }

                

                //************************************* GET DATA doIB Email ************************************************
               ////////////////////// RETRIEVE Installation Basic /////////////////
                //sParam.do_TbtInstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                sParam.do_TbtInstallationBasic = null;
                List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                if (tmpdoTbt_InstallationBasic != null)
                {
                    //==================== map misc type =========================
                    miscMapping = new MiscTypeMappingList();
                    miscMapping.AddMiscType(tmpdoTbt_InstallationBasic.ToArray());
                    handlerCommon.MiscTypeMappingList(miscMapping);
                    //============================================================
                    sParam.do_TbtInstallationBasic = new tbt_InstallationBasic();
                    sParam.do_TbtInstallationBasic = tmpdoTbt_InstallationBasic[0];
                }
                
                if (sParam.do_TbtInstallationBasic != null && sParam.do_TbtInstallationBasic.InstallationStatus != InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                {
                    //sParam.m_blnbFirstTimeRegister = false;
                    sParam.MaintenanceNo = sParam.do_TbtInstallationBasic.MaintenanceNo;
                    if (sParam.MaintenanceNo != null)
                    {
                        handlerCommon.ClearTemporaryUploadFile(sParam.MaintenanceNo);
                    }
                    
                    /////////////////// RETRIEVE EMAIL ////////////////////////
                    List<tbt_InstallationEmail> ListEmail = handler.GetTbt_InstallationEmailData(sParam.do_TbtInstallationBasic.SlipNo);
                    ISS070_DOEmailData TempEmail;
                    List<ISS070_DOEmailData> listAddEmail = new List<ISS070_DOEmailData>();
                    if (ListEmail != null)
                    {
                        foreach (tbt_InstallationEmail dataEmail in ListEmail)
                        {
                            TempEmail = new ISS070_DOEmailData();
                            TempEmail.EmailAddress = dataEmail.EmailNoticeTarget;
                            listAddEmail.Add(TempEmail);
                        }
                    }

                    sParam.ListDOEmail = listAddEmail;
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS070",
                                        MessageUtil.MODULE_INSTALLATION,
                                        MessageUtil.MessageList.MSG5018,
                                        new string[] { "" },
                                        new string[] { "ContractCodeProjectCode" });
                    return Json(res);
                    sParam.ListDOEmail = null;
                }
                               
                

                
                               
                //*******************************************************************************************************************
                ////////////////// INITIAL INSTRUMENT DETAILS /////////////////////
                //sParam.do_TbtInstallationSlipDetails
                
                
                ////////////////// RETRIEVE SLIP ///////////////////////////
                sParam.do_TbtInstallationSlip = handler.GetTbt_InstallationSlipData(sParam.do_TbtInstallationBasic.SlipNo);
                if (sParam.do_TbtInstallationSlip != null)
                {
                    //================= mapping ================================
                    List<tbt_InstallationSlip> do_TbtInstallationSlipList = new List<tbt_InstallationSlip>();
                    do_TbtInstallationSlipList.Add(sParam.do_TbtInstallationSlip);
                    miscMapping = new MiscTypeMappingList();
                    miscMapping.AddMiscType(do_TbtInstallationSlipList.ToArray());
                    handlerCommon.MiscTypeMappingList(miscMapping);
                    sParam.do_TbtInstallationSlip = do_TbtInstallationSlipList[0];
                    //=========================================================== 

                    //
                    IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                    List<tbs_MiscellaneousTypeCode>  masterlist = masterHandler.GetTbs_MiscellaneousTypeCode("slipstatus");
                    foreach (var item in masterlist)
                    {
                        if(item.ValueCode == sParam.do_TbtInstallationSlip.SlipStatus)
                        {
                            if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                            {
                                sParam.LastSlipStatusName = item.ValueDisplayEN;
                            }
                            else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                            {
                                sParam.LastSlipStatusName = item.ValueDisplayJP;
                            }
                            else
                            {
                                sParam.LastSlipStatusName = item.ValueDisplayLC;
                            }
                        }

                    }


                    //===================== Retrieve Slip Details ======================
                    List<tbt_InstallationSlipDetails> ListTemp_TbtInstallationSlipDetails = new List<tbt_InstallationSlipDetails>();
                    ListTemp_TbtInstallationSlipDetails = handler.GetTbt_InstallationSlipDetailsData(sParam.do_TbtInstallationBasic.SlipNo, null, InstrumentType.C_INST_TYPE_GENERAL);
                    if (ListTemp_TbtInstallationSlipDetails != null)
                    {
                        sParam.arrayInstrumentName = new string[ListTemp_TbtInstallationSlipDetails.Count];
                        int countInstrumentList = 0;
                        IInstrumentMasterHandler InstrumentHandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                        foreach (tbt_InstallationSlipDetails dataInstrument in ListTemp_TbtInstallationSlipDetails)
                        {
                            doInstrumentSearchCondition dCond = new doInstrumentSearchCondition()
                            {
                                InstrumentCode = dataInstrument.InstrumentCode.Replace(" ", "")
                            };
                            List<doInstrumentData> lst = InstrumentHandler.GetInstrumentDataForSearch(dCond);
                            if (lst.Count > 0)
                            {
                                sParam.arrayInstrumentName[countInstrumentList] = lst[0].InstrumentName;
                            }

                            countInstrumentList++;
                        }

                        sParam.do_TbtInstallationSlipDetails = ListTemp_TbtInstallationSlipDetails;
                    }
                    //==================================================================

                    // - Comment by Nontawat L. on 09-Jul-2014 : Phase4: 3.44
                    //if (sParam.do_TbtInstallationSlip.SlipStatus == SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                    "ISS070",
                    //                    MessageUtil.MODULE_INSTALLATION,
                    //                    MessageUtil.MessageList.MSG5079,
                    //                    new string[] { "" },
                    //                    new string[] { "ContractCodeProjectCode" });
                    //    return Json(res);
                    //}
                }
                if (sParam.do_TbtInstallationBasic.SlipNo != null)
                {
                    List<tbt_InstallationMemo> ListMemo = handler.GetTbt_InstallationMemo(sParam.do_TbtInstallationBasic.SlipNo);
                    if (ListMemo != null && ListMemo.Count > 0)
                    {
                        sParam.InstallationMemo = ListMemo[0].Memo;
                    }
                }

                if (sParam.do_TbtInstallationBasic.MaintenanceNo != null)
                {
                    List<tbt_InstallationPOManagement> ListPOManagement = handler.GetTbt_InstallationPOManagementData(sParam.do_TbtInstallationBasic.MaintenanceNo);
                    if (ListPOManagement != null && ListPOManagement.Count > 0)
                    {
                        sParam.doTbt_InstallationPOManagement = ListPOManagement;
                    }
                }

                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    if ((sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE
                        || sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE)
                        && sParam.dtSale.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE
                        )
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS070",
                                        MessageUtil.MODULE_INSTALLATION,
                                        MessageUtil.MessageList.MSG5051,
                                        new string[] { "lblContractProjectCode" },
                                        new string[] { "ContractCodeProjectCode" });
                        return Json(res);
                    }
                }
                
                //res = ISS070_ValidateContractError(sParam);
                //if (res.IsError)
                //{
                //    result.blnValidateContractError = false;
                //    res.ResultData = result;
                //    return Json(res);
                //}

                
                //if (res.IsError)
                //    return Json(res);

                //---- Keep ContractCode ----- Add by Teerapong S. 26/Apr/2012 =======================
                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = sParam.ContractCodeShort;
                sParam.CommonSearch = new ScreenParameter.CommonSearchDo();
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE || sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    sParam.CommonSearch.ContractCode = sParam.ContractCodeShort;
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    sParam.CommonSearch.ProjectCode = sParam.ContractCodeShort;
                }
                //====================================================================================

                UpdateScreenObject(sParam);

                result.ContractCodeShort = sParam.ContractCodeShort;
                result.dtRentalContractBasic = dtRentalContractBasic;
                result.dtSale = dtSaleContractBasic;
                result.dtProject = sParam.dtProject;
                result.do_TbtInstallationBasic = sParam.do_TbtInstallationBasic;
                result.do_TbtInstallationSlip = sParam.do_TbtInstallationSlip;
                result.do_TbtInstallationInstrumentDetail = sParam.do_TbtInstallationInstrumentDetail;
                result.ServiceTypeCode = sParam.ServiceTypeCode;
                result.InstallType = sParam.InstallType;
                result.m_blnbFirstTimeRegister = sParam.m_blnbFirstTimeRegister;
                result.do_TbtInstallationSlipDetails = sParam.do_TbtInstallationSlipDetails;
                result.arrayInstrumentName = sParam.arrayInstrumentName;
                result.ListDOEmail = sParam.ListDOEmail;
                result.LastSlipStatusName = sParam.LastSlipStatusName;
                result.InstallationMemo = sParam.InstallationMemo;
                result.doTbt_InstallationPOManagement = sParam.doTbt_InstallationPOManagement;
                result.ContractProjectCodeForShow = ContractProjectCodeForShow;
                sParam.ContractCodeShort = ContractProjectCodeForShow;
                res.ResultData = result;
                
                
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Retrieve data before confirm
        /// </summary>
        /// <param name="strContractCodeShort"></param>
        /// <returns></returns>
        public ObjectResultData ISS070_RetrieveDataBeforeConfirm(string strContractCodeShort)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            dtRentalContractBasicForInstall dtRentalContractBasic = new dtRentalContractBasicForInstall();
            dtSaleBasic dtSaleContractBasic = new dtSaleBasic();
            string lang = CommonUtil.GetCurrentLanguage();
            ISS070_RegisterStartResumeTargetData result = new ISS070_RegisterStartResumeTargetData();
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            MiscTypeMappingList miscMapping = new MiscTypeMappingList();
            try
            {

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ISS070_ScreenParameter sParam = GetScreenObject<ISS070_ScreenParameter>();


                ///////////// START RETRIEVE DATA ////////////////////////

                //string strContractCodeLong = comUtil.ConvertContractCode(strContractCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
                //sParam.ContractCodeLong = strContractCodeLong;
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                IRentralContractHandler RentalContactHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
               
                //=============== RETRIEVE PROJECT DATA ==================
                string strProjectCodeLong = comUtil.ConvertProjectCode(strContractCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
                IProjectHandler ProjectHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                dtProjectForInstall dtProjectDataForInstall = ProjectHandler.GetProjectDataForInstall(strProjectCodeLong);
                //========================================================

                if (sParam.InstallType == MiscType.C_SALE_INSTALL_TYPE)
                {

                    dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(sParam.ContractCodeLong, MiscType.C_BUILDING_TYPE);

                    if (dtSaleContractBasic != null)
                    {                        
                        sParam.dtSale = dtSaleContractBasic;                        
                        
                        if (ISS070_ValidExistOffice(dtSaleContractBasic.OperationOfficeCode) == false)
                        {
                            //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                    "ISS070",
                                    MessageUtil.MODULE_COMMON,
                                    MessageUtil.MessageList.MSG0063,
                                    new string[] { "" },
                                    new string[] { "ContractCodeProjectCode" });
                            return res;
                        }

                    }                    
                }

                ////////////////////// RETRIEVE Installation Basic /////////////////
                sParam.do_TbtInstallationBasic = null;
                List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                if (tmpdoTbt_InstallationBasic != null)
                {
                    sParam.do_TbtInstallationBasic = new tbt_InstallationBasic();
                    sParam.do_TbtInstallationBasic = tmpdoTbt_InstallationBasic[0];
                }

                if (sParam.do_TbtInstallationBasic == null)                
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS070",
                                        MessageUtil.MODULE_INSTALLATION,
                                        MessageUtil.MessageList.MSG5018,
                                        new string[] { "" },
                                        new string[] { "ContractCodeProjectCode" });
                    return res;
                }                

                ////////////////// RETRIEVE SLIP ///////////////////////////
                sParam.do_TbtInstallationSlip = handler.GetTbt_InstallationSlipData(sParam.do_TbtInstallationBasic.SlipNo);
                if (sParam.do_TbtInstallationSlip != null)
                {
                    //================= mapping ================================
                    List<tbt_InstallationSlip> do_TbtInstallationSlipList = new List<tbt_InstallationSlip>();
                    do_TbtInstallationSlipList.Add(sParam.do_TbtInstallationSlip);
                    miscMapping = new MiscTypeMappingList();
                    miscMapping.AddMiscType(do_TbtInstallationSlipList.ToArray());
                    handlerCommon.MiscTypeMappingList(miscMapping);
                    sParam.do_TbtInstallationSlip = do_TbtInstallationSlipList[0];
                    //===========================================================                     
                                       
                    // - Comment by Nontawat L. on 09-Jul-2014 : Phase4: 3.44
                    //if (sParam.do_TbtInstallationSlip.SlipStatus == SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                    "ISS070",
                    //                    MessageUtil.MODULE_INSTALLATION,
                    //                    MessageUtil.MessageList.MSG5079,
                    //                    new string[] { "lblLastSlipStatus" },
                    //                    new string[] { "SlipStatusName" });
                    //    return res;
                    //}
                }
                
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    if ((sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE
                        || sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE)
                        && sParam.dtSale.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE
                        )
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                        "ISS070",
                                        MessageUtil.MODULE_INSTALLATION,
                                        MessageUtil.MessageList.MSG5051,
                                        new string[] { "lblContractProjectCode" },
                                        new string[] { "ContractCodeProjectCode" });
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
        /// Get email data
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        public ActionResult ISS070_GetInstallEmail(string strEmail)
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
                    List<ISS070_DOEmailData> listEmail;
                    ISS070_ScreenParameter session = GetScreenObject<ISS070_ScreenParameter>();
                    ISS070_DOEmailData doISS070EmailADD = new ISS070_DOEmailData();                 

                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS070_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    doISS070EmailADD.EmpNo = emailList[0].EmpNo;
                    doISS070EmailADD.EmailAddress = emailList[0].EmailAddress;

                    if (listEmail.FindAll(delegate(ISS070_DOEmailData s) { return s.EmpNo == doISS070EmailADD.EmpNo; }).Count() == 0)
                        listEmail.Add(doISS070EmailADD);
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
        public ActionResult ISS070_ValidateRegisterData()
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
        /// <summary>
        /// Get data installation type combobox
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS070_GetMiscInstallationtype(string strFieldName)
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
        /// Get misctype data by code
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="ValueCode"></param>
        /// <returns></returns>
        public ActionResult ISS070_GetMiscTypeByValueCode(string FieldName,string ValueCode)
        {
            ObjectResultData res = new ObjectResultData();
          
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            string lang = CommonUtil.GetCurrentLanguage();
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

                if(lst.Count > 0)
                {
                    if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        res.ResultData = lst[0].ValueCode + " : " + lst[0].ValueDisplayEN;
                    }
                    else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_JP)
                    {
                        res.ResultData = lst[0].ValueCode + " : " + lst[0].ValueDisplayJP;
                    }
                    else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_LC)
                    {
                        res.ResultData = lst[0].ValueCode + " : " + lst[0].ValueDisplayLC;
                    }
                    
                }
                else{
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
        /// Get office name by code
        /// </summary>
        /// <param name="ValueCode"></param>
        /// <returns></returns>
        public ActionResult ISS070_GetOfficeNameByCode(string ValueCode)
        {
            ObjectResultData res = new ObjectResultData();

            List<dtOffice> list = new List<dtOffice>();
            string lang = CommonUtil.GetCurrentLanguage();
            try
            {
                IOfficeMasterHandler OMHandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                list = OMHandler.GetFunctionLogistic();

                foreach (var item in list)
                {
                    if (ValueCode == item.OfficeCode)
                    {
                        if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                        {
                            item.OfficeNameEN = CommonUtil.TextCodeName(item.OfficeCode, item.OfficeNameEN);
                            res.ResultData = item.OfficeNameEN;
                        }
                        else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                        {
                            item.OfficeNameJP = CommonUtil.TextCodeName(item.OfficeCode, item.OfficeNameJP);
                            res.ResultData = item.OfficeNameJP;
                        }
                        else
                        {
                            item.OfficeNameLC = CommonUtil.TextCodeName(item.OfficeCode, item.OfficeNameLC);
                            res.ResultData = item.OfficeNameLC;
                        }
                    }
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
        /// Get email data and send to showw in grid
        /// </summary>
        /// <param name="doCTS053Email"></param>
        /// <returns></returns>
        public ActionResult GetEmail_ISS070(ISS070_DOEmailData doCTS053Email)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            ISS070_DOEmailData doISS070EmailADD;
            List<ISS070_DOEmailData> listEmail;
            ISS070_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS070_ScreenParameter>();
                doISS070EmailADD = new ISS070_DOEmailData();
                if (session.ListDOEmail == null)
                    listEmail = new List<ISS070_DOEmailData>();
                else
                    listEmail = session.ListDOEmail;

                doISS070EmailADD.EmpNo = session.DOEmail.EmpNo;
                doISS070EmailADD.EmailAddress = doCTS053Email.EmailAddress;

                if (listEmail.FindAll(delegate(ISS070_DOEmailData s) { return s.EmpNo == doISS070EmailADD.EmpNo; }).Count() == 0)
                    listEmail.Add(doISS070EmailADD);

                session.DOEmail = null;
                session.ListDOEmail = listEmail;
                res.ResultData = CommonUtil.ConvertToXml<ISS070_DOEmailData>(session.ListDOEmail, "Contract\\CTS053Email", CommonUtil.GRID_EMPTY_TYPE.INSERT);
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
        public ActionResult ISS070_GetEmailList(List<ISS070_DOEmailData> listEmailAdd)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            List<ISS070_DOEmailData> listEmail;
            List<ISS070_DOEmailData> listNewEmail;
            ISS070_ScreenParameter session;

            try
            {
                listNewEmail = new List<ISS070_DOEmailData>();
                if (listEmailAdd != null)
                {
                    session = GetScreenObject<ISS070_ScreenParameter>();
                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS070_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    foreach (var item in listEmailAdd)
                    {
                        if (listEmail.FindAll(delegate(ISS070_DOEmailData s) { return s.EmailAddress == item.EmailAddress; }).Count() == 0)
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
        /// <param name="doISS070Email"></param>
        /// <returns></returns>
        public ActionResult RemoveMailClick_ISS070(ISS070_DOEmailData doISS070Email)
        {
            List<ISS070_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS070_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS070_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate(ISS070_DOEmailData s) { return s.EmailAddress == doISS070Email.EmailAddress; });
                if (listEmailDelete.Count() != 0)
                    session.ListDOEmail.Remove(listEmailDelete[0]);

                res.ResultData = CommonUtil.ConvertToXml<ISS070_DOEmailData>(session.ListDOEmail, "Installation\\ISS010Email", CommonUtil.GRID_EMPTY_TYPE.INSERT);
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
        /// <param name="doISS070Email"></param>
        /// <returns></returns>
        public ActionResult ISS070_RemoveMailClick(ISS070_DOEmailData doISS070Email)
        {
            List<ISS070_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS070_ScreenParameter session;
            try
            {
                session = GetScreenObject<ISS070_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate(ISS070_DOEmailData s) { return s.EmailAddress == doISS070Email.EmailAddress; });
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
        /// Register cancel data
        /// </summary>
        /// <param name="conditionCancel"></param>
        /// <returns></returns>
        public ActionResult ISS070_RegisterData(string conditionCancel)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;

            try
            {
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                
                ISS070_ScreenParameter sParam = GetScreenObject<ISS070_ScreenParameter>();
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                using (TransactionScope scope = new TransactionScope())
                {
                    res = ISS070_RetrieveDataBeforeConfirm(sParam.ContractCodeShort);
                    if (res.IsError)
                    {
                        return Json(res);
                    }
                    //===========================================================================================
                    if (conditionCancel == "CANCELALL")
                    {
                        res = ISS070_CancelAll(sParam);
                    }
                    else if (conditionCancel == "CANCELPO")
                    {
                        res = ISS070_CancelPO(sParam);
                    }
                    else if (conditionCancel == "CANCELPOANDSLIP")
                    {
                        res = ISS070_CancelPOAndSlip(sParam);
                    }
                    else if (conditionCancel == "CANCELSLIPUSEPREVIOUS")
                    {
                        res = ISS070_CancelSlipUsePrevious(sParam);
                    }
                    else if (conditionCancel == "CANCELSLIP")
                    {
                        res = ISS070_CancelSlip(sParam);
                    }
                    if (res.IsError)
                    {                        
                        return Json(res);
                    }
                    //===========================================================================================
                    scope.Complete();
                }                

                //ISS070_RegisterStartResumeTargetData result = new ISS070_RegisterStartResumeTargetData();
                //result.SlipNo = StrSlipNo;
                res.ResultData = true;
                
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Validate before register data
        /// </summary>
        /// <param name="doValidate"></param>
        /// <param name="ScreenInput"></param>
        /// <returns></returns>               
        public ActionResult ISS070_ValidateBeforeRegister(ISS070_ValidateData doValidate, ISS070_ScreenInput ScreenInput)
        {

            ObjectResultData res = new ObjectResultData();
            
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();

            try
            {
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_CANCEL, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                ISS070_ScreenParameter sParam = GetScreenObject<ISS070_ScreenParameter>();

                List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                if (tmpdoTbt_InstallationBasic == null || tmpdoTbt_InstallationBasic.Count == 0 || tmpdoTbt_InstallationBasic[0].InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED )
                {
                    //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                                MessageUtil.MessageList.MSG5050,
                    //                                "ContractCodeProjectCode", "Contract/Project Code",
                    //                                "ContractCodeProjectCode");
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS070",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5018,
                                                    "ContractCodeProjectCode",
                                                    "lblContractProjectCode",
                                                    "ContractCodeProjectCode");
                }
                // - Comment by Nontawat L. on 09-Jul-2014 : Phase4:3.44
                //else
                //{
                //    if (sParam.do_TbtInstallationSlip != null)
                //    {
                //        if (sParam.do_TbtInstallationSlip.SlipStatus == SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT)
                //        {
                //            //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                //            //                            MessageUtil.MessageList.MSG5079,
                //            //                            "SlipStatusName", "Last Slip Status",
                //            //                            "SlipStatusName");
                //            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                //                                    "ISS070",
                //                                    MessageUtil.MODULE_INSTALLATION,
                //                                    MessageUtil.MessageList.MSG5079,
                //                                    "SlipStatusName",
                //                                    "lblLastSlipStatus",
                //                                    "SlipStatusName");
                //        }
                //    }
                //}

                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE && tmpdoTbt_InstallationBasic != null && tmpdoTbt_InstallationBasic.Count > 0)
                {
                    if ((tmpdoTbt_InstallationBasic[0].InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW
                        || tmpdoTbt_InstallationBasic[0].InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD)
                        && sParam.dtSale.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE
                        )
                    {
                        //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                        //                            MessageUtil.MessageList.MSG5051,
                        //                            "ContractCodeProjectCode", "Contract/Project Code",
                        //                            "ContractCodeProjectCode");
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS070",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5051,
                                                    "ContractCodeProjectCode",
                                                    "lblContractProjectCode",
                                                    "ContractCodeProjectCode");
                    }
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
        public bool ISS070_ValidExistOffice(string OfficeCode)
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
        /// Validate business
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ValidatorUtil ISS070_ValidateBusiness(ISS070_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            ValidatorUtil validator = new ValidatorUtil();
            try
            {              

                if (sParam.do_TbtInstallationSlip.NormalInstallFee != sParam.do_TbtInstallationSlip.BillingInstallFee && (sParam.do_TbtInstallationSlip.ApproveNo1 == null || sParam.do_TbtInstallationSlip.ApproveNo1 == ""))
                {
                    //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                            MessageUtil.MessageList.MSG5027,
                    //                            "ApproveNo1", "Approve No. 1",
                    //                            "ApproveNo1");
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS070",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5027,
                                                    "ApproveNo1",
                                                    "lblApproveNo1",
                                                    "ApproveNo1");
                }
                if ((sParam.do_TbtInstallationSlip.ApproveNo2 != null && sParam.do_TbtInstallationSlip.ApproveNo2 != "") && (sParam.do_TbtInstallationSlip.ApproveNo1 == null || sParam.do_TbtInstallationSlip.ApproveNo1 == ""))
                {
                    //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                            MessageUtil.MessageList.MSG5029,
                    //                            "ApproveNo1", "Approve No. 1",
                    //                            "ApproveNo1");
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS070",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5029,
                                                    "ApproveNo1",
                                                    "lblApproveNo1",
                                                    "ApproveNo1");
                }
                DateTime pToDay = new DateTime();
                pToDay = DateTime.Today;
                if (sParam.do_TbtInstallationBasic.InstallationCompleteDate < pToDay)
                {
                    //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                            MessageUtil.MessageList.MSG5042,
                    //                            "InstallCompleteDate", "Installation Complete Date",
                    //                            "InstallCompleteDate");
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS070",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5051,
                                                    "ContractCodeProjectCode",
                                                    "lblContractProjectCode",
                                                    "ContractCodeProjectCode");
                }
                if (sParam.dtRentalContractBasic != null)
                {
                    if (sParam.do_TbtInstallationBasic.InstallationCompleteDate < sParam.dtRentalContractBasic.ChangeImplementDate)
                    {
                        //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                        //                            MessageUtil.MessageList.MSG5043,
                        //                            "InstallCompleteDate", "Installation Complete Date",
                        //                            "InstallCompleteDate");
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS070",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5051,
                                                    "ContractCodeProjectCode",
                                                    "lblContractProjectCode",
                                                    "ContractCodeProjectCode");
                    }
                }
                
                if(sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                    &&
                    (
                        sParam.dtRentalContractBasic.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL ||
                        sParam.dtRentalContractBasic.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START ||
                        sParam.dtRentalContractBasic.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT 
                    ))
                {
                    //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                                MessageUtil.MessageList.MSG5047,
                    //                                "InstallationType", "Installation Type",
                    //                                "InstallationType");
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS070",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5051,
                                                    "ContractCodeProjectCode",
                                                    "lblContractProjectCode",
                                                    "ContractCodeProjectCode");
                }

                if((sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL ||
                    sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                    )
                    &&
                    (
                        sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL &&
                        sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START &&
                        sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT 
                    ))
                {
                    //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                                MessageUtil.MessageList.MSG5048,
                    //                                "InstallationType", "Installation Type",
                    //                                "InstallationType");
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS070",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5051,
                                                    "ContractCodeProjectCode",
                                                    "lblContractProjectCode",
                                                    "ContractCodeProjectCode");
                }
                //===================== ValidateCP12 =========================================
                IRentralContractHandler rentalHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                string strLastUnimplementOCC = "";
                strLastUnimplementOCC = rentalHandler.GetLastUnimplementedOCC(sParam.ContractCodeLong);
                bool blnCheckCP12 = rentalHandler.CheckCP12(sParam.ContractCodeLong,strLastUnimplementOCC);
                bool blnValidateCP12;
                if (blnCheckCP12)
                {
                    int RentalInstrumentQty = 0;
                    List<tbt_RentalInstrumentDetails> rentalInstrumentData = rentalHandler.GetTbt_RentalInstrumentDetails(sParam.ContractCodeLong, strLastUnimplementOCC);
                    if (rentalInstrumentData.Count > 0)
                    {
                        RentalInstrumentQty = (int)rentalInstrumentData[0].InstrumentQty + (int)rentalInstrumentData[0].AdditionalInstrumentQty - (int)rentalInstrumentData[0].RemovalInstrumentQty; 
                    }
                    blnValidateCP12 = true;
                    foreach (ISS070_GridInstrumentData GridValidData in sParam.GridInstrumentForValid)
                    {
                            if (GridValidData.ContractInstalledAfterChange != RentalInstrumentQty)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5046,
                                                        "", "",
                                                        "");
                                blnValidateCP12 = false;
                                break;
                            }                        
                    }
                }
                else
                {
                    blnValidateCP12 = false;
                    if (sParam.do_TbtInstallationSlip.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_CUS)
                    {                        
                        if(sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE 
                            || sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE
                            || sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING)
                        {
                            blnValidateCP12 = true;
                        }
                        else if (sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW
                            && sParam.do_TbtInstallationSlip.PreviousSlipStatus == null)
                        {
                            blnValidateCP12 = true;
                        }
                    }
                    else if (sParam.do_TbtInstallationSlip.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_SECOM)
                    {
                        blnValidateCP12 = true;
                    }
                    else {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5045,
                                                        "", "",
                                                        "");
                        blnValidateCP12 = false;
                    }
                }
                
                //============================================================================
               
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return validator;
        }
        /// <summary>
        /// Get data instrument detail
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult ISS070_GetInstrumentDetailInfo(ISS070_GetInstrumentDataCondition cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                #region Get Session

                string ProductTypeCode = null;

                ISS070_ScreenParameter sParam = GetScreenObject<ISS070_ScreenParameter>();
                if (sParam.dtRentalContractBasic != null)
                {
                    ProductTypeCode = sParam.dtRentalContractBasic.ProductTypeCode;
                }
                else
                {
                    ProductTypeCode = sParam.dtSale.ProductTypeCode;
                }
                

                #endregion

                doInstrumentSearchCondition dCond = new doInstrumentSearchCondition()
                {
                    InstrumentCode = cond.InstrumentCode
                };

                if (ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SALE)
                    dCond.SaleFlag = 1;
                if (ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL)
                    dCond.RentalFlag = 1;
                dCond.ExpansionType = new List<string>() { SECOM_AJIS.Common.Util.ConstantValue.ExpansionType.C_EXPANSION_TYPE_PARENT };
                dCond.InstrumentType = new List<string>() { SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_GENERAL };

                IInstrumentMasterHandler handler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<doInstrumentData> lst = handler.GetInstrumentDataForSearch(dCond);
                if (lst != null)
                {
                    if (lst.Count > 0)
                        res.ResultData = lst[0];
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Process cancel all installation data
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ObjectResultData ISS070_CancelAll(ISS070_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            IInventoryHandler iHandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            ICommonHandler cHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            try
            {
                if (sParam.do_TbtInstallationBasic == null || sParam.do_TbtInstallationBasic.InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5018,
                                                    "ContractCodeProjectCode", "Contract/Project Code",
                                                    "ContractCodeProjectCode");
                }
                // - Comment by Nontawat L. on 09-Jul-2014: Phase4: 3.44
                //if (sParam.do_TbtInstallationSlip != null)
                //{
                //    if ((sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL || sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE) && sParam.do_TbtInstallationBasic.SlipNo != null)
                //    {
                //        if (sParam.do_TbtInstallationSlip.SlipStatus == SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT)
                //        {
                //            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                //                                            MessageUtil.MessageList.MSG5079,
                //                                            "SlipStatusName", "Slip Status",
                //                                            "SlipStatusName");
                //        }
                //    }
                //}
                if (sParam.dtSale != null)
                {
                    //if((sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW
                    //    || sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD)
                    //    && sParam.dtSale.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE )
                    if((sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE
                        || sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE)
                        && sParam.dtSale.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5051,
                                                        "ContractCodeProjectCode", "Contract/Project Code",
                                                        "ContractCodeProjectCode");
                    }
                }
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return res; 

                var blnNewInstallation = true;
                if((sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL || sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    &&  (sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW
                        || sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW 
                        || sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW 
                        || sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD ))
                {
                    sParam.do_TbtInstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED;
                    blnNewInstallation = true;
                }
                else
                {
                    sParam.do_TbtInstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_CANCELLED;
                    blnNewInstallation = false;
                }


                int SumTotalStockOut = 0;
                if (blnNewInstallation)
                {
                    //Comment by Jutarat A. on 13022014 (Move)
                    ////Add by Jutarat A. on 18102013
                    ////6.1	Copy installation history
                    //sParam.do_TbtInstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_CANCELLED;
                    //ISS070_CopyInstallationHistory(sParam, blnNewInstallation); //Modify (Add blnNewInstallation) by Jutarat A. on 06022014
                    ////End Add
                    //End Comment

                    //============ Call Inventory process ============
                    doBooking BookData = new doBooking();
                    BookData.ContractCode = sParam.do_TbtInstallationBasic.ContractProjectCode;
                    bool blnComplete = iHandler.Rebooking(BookData);                    
                }
                else
                {
                    //============ Update installation basic (Status only) ============
                    handler.UpdateTbt_InstallationBasic(sParam.do_TbtInstallationBasic);

                    //Modify by Jutarat A. on 18102013
                    //============ INSERT INSTALLATION HISTORY ========================
                    /*List<tbt_InstallationBasic> tmpTbt_InstallationBasicList = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                    if (tmpTbt_InstallationBasicList != null)
                    {
                        tbt_InstallationBasic tempTbt_InstallationBasic = tmpTbt_InstallationBasicList[0];
                        tbt_InstallationHistory do_TbtInstallationHistory = CommonUtil.CloneObject<tbt_InstallationBasic, tbt_InstallationHistory>(tempTbt_InstallationBasic);
                        do_TbtInstallationHistory.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        do_TbtInstallationHistory.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        handler.InsertTbt_InstallationHistory(do_TbtInstallationHistory);
                    }
                    //=================================================================

                    //======================= Insert SLIP Detail ====================== 
                    List<tbt_InstallationSlipDetails> TempTbt_InstallationSlipDetailsList = new List<tbt_InstallationSlipDetails>();
                    TempTbt_InstallationSlipDetailsList = handler.GetTbt_InstallationSlipDetailsData(sParam.do_TbtInstallationBasic.SlipNo, null,InstrumentType.C_INST_TYPE_GENERAL);
                    if (TempTbt_InstallationSlipDetailsList != null)
                    {
                        foreach (tbt_InstallationSlipDetails slipDetail in TempTbt_InstallationSlipDetailsList)
                        {
                            tbt_InstallationHistoryDetails do_TbtInstallationHistoryDetail = new tbt_InstallationHistoryDetails();
                            do_TbtInstallationHistoryDetail.ContractCode = sParam.do_TbtInstallationBasic.ContractProjectCode;
                            do_TbtInstallationHistoryDetail.InstrumentCode = slipDetail.InstrumentCode;
                            do_TbtInstallationHistoryDetail.InstrumentTypeCode = slipDetail.InstrumentTypeCode;
                            do_TbtInstallationHistoryDetail.ContractInstalledQty = slipDetail.TotalStockOutQty - slipDetail.ReturnQty - slipDetail.NotInstalledQty;
                            do_TbtInstallationHistoryDetail.ContractRemovedQty = slipDetail.AddRemovedQty;
                            do_TbtInstallationHistoryDetail.ContractMovedQty = slipDetail.MoveQty;
                            do_TbtInstallationHistoryDetail.CreateDate = slipDetail.CreateDate;
                            do_TbtInstallationHistoryDetail.CreateBy = slipDetail.CreateBy;
                            //do_TbtInstallationHistoryDetail.UpdateDate = slipDetail.UpdateDate;
                            //do_TbtInstallationHistoryDetail.UpdateBy = slipDetail.UpdateBy;
                            handler.InsertTbt_InstallationHistoryDetail(do_TbtInstallationHistoryDetail);

                            //SumTotalStockOut = SumTotalStockOut + (int)slipDetail.TotalStockOutQty;
                        }
                    }*/
                    //=================================================================
                    ISS070_CopyInstallationHistory(sParam, blnNewInstallation); //Modify (Add blnNewInstallation) by Jutarat A. on 06022014
                    //End Modify

                    ///////////////// DELETE Instrument Detail //////////////////
                    handler.DeleteTbt_InstallationInstrumentDetail(sParam.do_TbtInstallationBasic.ContractProjectCode, null);
                    /////////////////////////////////////////////////////////////
                    ///////////////// DELETE Installation Basic //////////////////
                    handler.DeleteTbt_InstallationBasic(sParam.do_TbtInstallationBasic.ContractProjectCode);
                    //////////////////////////////////////////////////////////////
                }

                if (sParam.do_TbtInstallationBasic.MaintenanceNo != null)
                {
                    //sParam.dtInstallationManagement = handler.GetTbt_InstallationManagementData(sParam.do_TbtInstallationBasic.MaintenanceNo);
                    List<tbt_InstallationManagement> tmpdoInstallationManagement = handler.GetTbt_InstallationManagementData(sParam.do_TbtInstallationBasic.MaintenanceNo);
                    if (tmpdoInstallationManagement != null)
                    {
                        sParam.dtInstallationManagement = new tbt_InstallationManagement();
                        sParam.dtInstallationManagement = tmpdoInstallationManagement[0];
                    }
                    if (sParam.dtInstallationManagement != null)
                    {
                        sParam.dtInstallationManagement.ManagementStatus = InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_CANCELED;
                        handler.UpdateTbt_InstallationManagement(sParam.dtInstallationManagement);
                    }
                    //========================== DELETE ATTACH FILES ==================================
                    ISS070_DeleteAllAttachFile(sParam.do_TbtInstallationBasic.MaintenanceNo, null);
                    cHandler.UpdateFlagAttachFile(AttachmentModule.Installation, sParam.MaintenanceNo, sParam.MaintenanceNo);
                    //=================================================================================
                }
                                
                if (sParam.do_TbtInstallationSlip != null)
                {
                    bool isPartialOut = sParam.do_TbtInstallationSlip.SlipStatus == SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT;
                    
                    if ((sParam.do_TbtInstallationBasic.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL || sParam.do_TbtInstallationBasic.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                        && sParam.do_TbtInstallationBasic.SlipNo != null
                        )
                    {
                        //Calculate total stock-out
                        List<tbt_InstallationSlipDetails> doSlipDetailList = new List<tbt_InstallationSlipDetails>();
                        doSlipDetailList = handler.GetTbt_InstallationSlipDetailsData(sParam.do_TbtInstallationBasic.SlipNo, null, InstrumentType.C_INST_TYPE_GENERAL);
                        if (doSlipDetailList != null)
                        {
                            foreach (tbt_InstallationSlipDetails slipDetail in doSlipDetailList)
                            {
                                SumTotalStockOut = SumTotalStockOut + (int)slipDetail.TotalStockOutQty + (int)slipDetail.CurrentStockOutQty;
                            }
                        }
                        //Check total stock-out
                        if (SumTotalStockOut > 0)
                        {
                            sParam.do_TbtInstallationSlip.SlipStatus = SlipStatus.C_SLIP_STATUS_WAIT_FOR_RETURN;
                        }
                        else
                        {
                            sParam.do_TbtInstallationSlip.SlipStatus = SlipStatus.C_SLIP_STATUS_INSTALL_SLIP_CANCELED;
                        }
                        handler.UpdateTbt_InstallationSlip(sParam.do_TbtInstallationSlip);
                    }
                    if (sParam.do_TbtInstallationSlip.SlipStatus == SlipStatus.C_SLIP_STATUS_WAIT_FOR_RETURN)
                    {
                        IInventoryHandler inventHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                        bool blnProcessResult = inventHand.UpdateCancelInstallation(sParam.do_TbtInstallationBasic.ContractProjectCode, sParam.do_TbtInstallationSlip.SlipNo, sParam.ServiceTypeCode, isPartialOut);
                    }
                }

                if (blnNewInstallation)
                {
                    //Add by Jutarat A. on 13022014 (Move)
                    //6.1	Copy installation history
                    sParam.do_TbtInstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_CANCELLED;
                    ISS070_CopyInstallationHistory(sParam, blnNewInstallation);
                    //End Add

                    //============ Update installation basic (Status only) ============
                    sParam.do_TbtInstallationBasic.SlipNo = null;
                    sParam.do_TbtInstallationBasic.MaintenanceNo = null;
                    sParam.do_TbtInstallationBasic.InstallationBy = null;
                    sParam.do_TbtInstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED; //Add by Jutarat A. on 18102013
                    handler.UpdateTbt_InstallationBasic(sParam.do_TbtInstallationBasic);
                }

                //==================== DELETE MEMO ===============================
                handler.DeleteTbt_InstallationMemo(sParam.do_TbtInstallationBasic.ContractProjectCode, null, null);
                //================================================================
                


            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }

        //Add by Jutarat A. on 18102013
        public void ISS070_CopyInstallationHistory(ISS070_ScreenParameter sParam, bool blnNewInstallation = false) //Modify (Add blnNewInstallation) by Jutarat A. on 06022014
        {
            try
            {
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                //============ INSERT INSTALLATION HISTORY ========================

                //Modify by Jutarat A. on 06022014
                //List<tbt_InstallationBasic> tmpTbt_InstallationBasicList = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                List<tbt_InstallationBasic> tmpTbt_InstallationBasicList = new List<tbt_InstallationBasic>();
                if (blnNewInstallation)
                {
                    tbt_InstallationBasic doTbt_InstallationBasic = CommonUtil.CloneObject<tbt_InstallationBasic, tbt_InstallationBasic>(sParam.do_TbtInstallationBasic);
                    tmpTbt_InstallationBasicList.Add(doTbt_InstallationBasic);
                }
                else
                {
                    tmpTbt_InstallationBasicList = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                }
                //End Modify
                
                if (tmpTbt_InstallationBasicList != null)
                {
                    tbt_InstallationBasic tempTbt_InstallationBasic = tmpTbt_InstallationBasicList[0];
                    tbt_InstallationHistory do_TbtInstallationHistory = CommonUtil.CloneObject<tbt_InstallationBasic, tbt_InstallationHistory>(tempTbt_InstallationBasic);
                    do_TbtInstallationHistory.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    do_TbtInstallationHistory.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    handler.InsertTbt_InstallationHistory(do_TbtInstallationHistory);
                }
                //=================================================================

                //======================= Insert SLIP Detail ====================== 
                List<tbt_InstallationSlipDetails> TempTbt_InstallationSlipDetailsList = new List<tbt_InstallationSlipDetails>();
                TempTbt_InstallationSlipDetailsList = handler.GetTbt_InstallationSlipDetailsData(sParam.do_TbtInstallationBasic.SlipNo, null, InstrumentType.C_INST_TYPE_GENERAL);
                if (TempTbt_InstallationSlipDetailsList != null)
                {
                    foreach (tbt_InstallationSlipDetails slipDetail in TempTbt_InstallationSlipDetailsList)
                    {
                        tbt_InstallationHistoryDetails do_TbtInstallationHistoryDetail = new tbt_InstallationHistoryDetails();
                        do_TbtInstallationHistoryDetail.ContractCode = sParam.do_TbtInstallationBasic.ContractProjectCode;
                        do_TbtInstallationHistoryDetail.InstrumentCode = slipDetail.InstrumentCode;
                        do_TbtInstallationHistoryDetail.InstrumentTypeCode = slipDetail.InstrumentTypeCode;
                        do_TbtInstallationHistoryDetail.ContractInstalledQty = slipDetail.TotalStockOutQty - slipDetail.ReturnQty - slipDetail.NotInstalledQty;
                        do_TbtInstallationHistoryDetail.ContractRemovedQty = slipDetail.AddRemovedQty;
                        do_TbtInstallationHistoryDetail.ContractMovedQty = slipDetail.MoveQty;
                        do_TbtInstallationHistoryDetail.CreateDate = slipDetail.CreateDate;
                        do_TbtInstallationHistoryDetail.CreateBy = slipDetail.CreateBy;

                        handler.InsertTbt_InstallationHistoryDetail(do_TbtInstallationHistoryDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //End Add

        /// <summary>
        /// Process cancel PO
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ObjectResultData ISS070_CancelPO(ISS070_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            ICommonHandler cHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            try
            {
                if (sParam.do_TbtInstallationBasic != null && sParam.do_TbtInstallationBasic.InstallationStatus != InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                {
                    if (sParam.do_TbtInstallationBasic.MaintenanceNo == null)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5052,
                                                        "InstallationMANo", "Maintenance No.",
                                                        "InstallationMANo");
                    }
                }
                else
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5018,
                                                        "", "",
                                                        "");
                }
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return res;
                //================ Update Installation Basic ==========================
                sParam.do_TbtInstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED;
                handler.UpdateTbt_InstallationBasic(sParam.do_TbtInstallationBasic);
                //=====================================================================

                //================= Update Installation Management ====================
                if (sParam.do_TbtInstallationBasic.MaintenanceNo != null)
                {
                    //sParam.dtInstallationManagement = handler.GetTbt_InstallationManagementData(sParam.do_TbtInstallationBasic.MaintenanceNo);
                    List<tbt_InstallationManagement> tmpdoInstallationManagement = handler.GetTbt_InstallationManagementData(sParam.do_TbtInstallationBasic.MaintenanceNo);
                    if (tmpdoInstallationManagement != null)
                    {
                        sParam.dtInstallationManagement = new tbt_InstallationManagement();
                        sParam.dtInstallationManagement = tmpdoInstallationManagement[0];
                    }
                    if (sParam.dtInstallationManagement != null)
                    {
                        sParam.dtInstallationManagement.IEStaffEmpNo1 =	null;
                        sParam.dtInstallationManagement.IEStaffEmpNo2	=	null;
                        sParam.dtInstallationManagement.IEManPower	=	null;
                        sParam.dtInstallationManagement.MaterialFee	=	null;
                        sParam.dtInstallationManagement.ChangeReasonCode	=	null;
                        sParam.dtInstallationManagement.ChangeReasonOther	=	null;
                        sParam.dtInstallationManagement.ChangeRequestorCode	=	null;
                        sParam.dtInstallationManagement.ChangeRequestorOther	=	null;
                        sParam.dtInstallationManagement.POMemo = null;
                        handler.UpdateTbt_InstallationManagement(sParam.dtInstallationManagement);
                    }
                }
                //=====================================================================

                //================== Delete PO Management =============================
                if (sParam.do_TbtInstallationBasic.MaintenanceNo != null)
                {
                    handler.DeleteTbt_InstallationPOManagement(sParam.do_TbtInstallationBasic.MaintenanceNo);

                    //========================== DELETE ATTACH FILES ==================================
                    ISS070_DeleteAllAttachFile(sParam.do_TbtInstallationBasic.MaintenanceNo, ScreenID.C_SCREEN_ID_INSTALL_PO);
                    ISS070_DeleteAllAttachFile(sParam.do_TbtInstallationBasic.MaintenanceNo, ScreenID.C_SCREEN_ID_INSTALL_MANAGE);
                    cHandler.UpdateFlagAttachFile(AttachmentModule.Installation, sParam.MaintenanceNo, sParam.MaintenanceNo);
                    //=================================================================================
                }
                //=====================================================================

                //==================== DELETE MEMO ===============================
                handler.DeleteTbt_InstallationMemo(sParam.do_TbtInstallationBasic.ContractProjectCode, sParam.do_TbtInstallationBasic.MaintenanceNo, ScreenID.C_SCREEN_ID_INSTALL_PO);
                handler.DeleteTbt_InstallationMemo(sParam.do_TbtInstallationBasic.ContractProjectCode, sParam.do_TbtInstallationBasic.MaintenanceNo, ScreenID.C_SCREEN_ID_INSTALL_MANAGE);
                //================================================================


            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }
        /// <summary>
        /// Process cancel PO and slip
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ObjectResultData ISS070_CancelPOAndSlip(ISS070_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            ICommonHandler cHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            try
            {
                
                if(sParam.do_TbtInstallationSlip == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5053,
                                                    "SlipNo", "Slip No.",
                                                    "SlipNo");
                }
                else if (sParam.do_TbtInstallationSlip.PreviousSlipNo != null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5053,
                                                    "SlipNo", "Slip No.",
                                                    "SlipNo");
                }
                if (sParam.do_TbtInstallationSlip.SlipStatus == SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5054,
                                                    "SlipStatusName", "Slip status",
                                                    "SlipStatusName");
                }

                if (sParam.do_TbtInstallationBasic != null && sParam.do_TbtInstallationBasic.InstallationStatus != InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                {
                    if (sParam.do_TbtInstallationBasic.SecurityTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        //if ((sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW || sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD)
                        if ((sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE || sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE)  
                        && sParam.dtSale.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE)
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5051,
                                                    "ContractCodeProjectCode", "Contract/Project code",
                                                    "ContractCodeProjectCode");
                        }
                    }
                    if (sParam.do_TbtInstallationBasic.MaintenanceNo == null)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5052,
                                                        "InstallationMANo", "Maintenance No.",
                                                        "InstallationMANo");
                    }
                }
                else
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5018,
                                                        "", "",
                                                        "");
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return res;
                //============================ Update Installation Basic ====================
                sParam.do_TbtInstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED;
                sParam.do_TbtInstallationBasic.SlipNo = null;
                handler.UpdateTbt_InstallationBasic(sParam.do_TbtInstallationBasic);
                //===========================================================================
                //================= Update Installation Management ====================
                if (sParam.do_TbtInstallationBasic.MaintenanceNo != null)
                {
                    //sParam.dtInstallationManagement = handler.GetTbt_InstallationManagementData(sParam.do_TbtInstallationBasic.MaintenanceNo);
                    List<tbt_InstallationManagement> tmpdoInstallationManagement = handler.GetTbt_InstallationManagementData(sParam.do_TbtInstallationBasic.MaintenanceNo);
                    if (tmpdoInstallationManagement != null)
                    {
                        sParam.dtInstallationManagement = new tbt_InstallationManagement();
                        sParam.dtInstallationManagement = tmpdoInstallationManagement[0];
                    }
                    if (sParam.dtInstallationManagement != null)
                    {
                        sParam.dtInstallationManagement.IEStaffEmpNo1 = null;
                        sParam.dtInstallationManagement.IEStaffEmpNo2 = null;
                        sParam.dtInstallationManagement.IEManPower = null;
                        sParam.dtInstallationManagement.MaterialFee = null;
                        sParam.dtInstallationManagement.ChangeReasonCode = null;
                        sParam.dtInstallationManagement.ChangeReasonOther = null;
                        sParam.dtInstallationManagement.ChangeRequestorCode = null;
                        sParam.dtInstallationManagement.ChangeRequestorOther = null;

                        handler.UpdateTbt_InstallationManagement(sParam.dtInstallationManagement);
                    }
                }
                //=====================================================================

                //================== Delete PO Management =============================
                if (sParam.do_TbtInstallationBasic.MaintenanceNo != null)
                {
                    handler.DeleteTbt_InstallationPOManagement(sParam.do_TbtInstallationBasic.MaintenanceNo);
                    //========================== DELETE ATTACH FILES ==================================
                    ISS070_DeleteAllAttachFile(sParam.do_TbtInstallationBasic.MaintenanceNo, ScreenID.C_SCREEN_ID_INSTALL_PO);
                    ISS070_DeleteAllAttachFile(sParam.do_TbtInstallationBasic.MaintenanceNo, ScreenID.C_SCREEN_ID_INSTALL_MANAGE);
                    cHandler.UpdateFlagAttachFile(AttachmentModule.Installation, sParam.MaintenanceNo, sParam.MaintenanceNo);
                    //=================================================================================
                }
                //=====================================================================

                //================== Update Installation Slip =========================
                if (sParam.do_TbtInstallationSlip != null)
                {
                    sParam.do_TbtInstallationSlip.SlipStatus = SlipStatus.C_SLIP_STATUS_INSTALL_SLIP_CANCELED;
                    handler.UpdateTbt_InstallationSlip(sParam.do_TbtInstallationSlip);
                }
                
                //=====================================================================
                

                //==================== DELETE MEMO ===============================
                handler.DeleteTbt_InstallationMemo(sParam.do_TbtInstallationBasic.ContractProjectCode, sParam.do_TbtInstallationBasic.MaintenanceNo, ScreenID.C_SCREEN_ID_INSTALL_PO);
                handler.DeleteTbt_InstallationMemo(sParam.do_TbtInstallationBasic.ContractProjectCode, sParam.do_TbtInstallationBasic.MaintenanceNo, ScreenID.C_SCREEN_ID_INSTALL_MANAGE);
                handler.DeleteTbt_InstallationMemo(sParam.do_TbtInstallationBasic.ContractProjectCode, sParam.do_TbtInstallationBasic.SlipNo, ScreenID.C_SCREEN_ID_INSTALL_SLIP);
                //================================================================
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }
        /// <summary>
        /// Process cancel Slip
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ObjectResultData ISS070_CancelSlip(ISS070_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            try
            {
               
                if (sParam.do_TbtInstallationSlip == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5053,
                                                    "SlipNo", "Slip No.",
                                                    "SlipNo");
                }
                else if (sParam.do_TbtInstallationSlip.PreviousSlipNo != null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5053,
                                                    "SlipNo", "Slip No.",
                                                    "SlipNo");
                }
                if (sParam.do_TbtInstallationSlip.SlipStatus == SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5054,
                                                    "SlipStatusName", "Slip status",
                                                    "SlipStatusName");
                }

                if (sParam.do_TbtInstallationBasic != null && sParam.do_TbtInstallationBasic.InstallationStatus != InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                {
                    if (sParam.do_TbtInstallationBasic.SecurityTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        if ((sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE || sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE)
                            && sParam.dtSale.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE)
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5051,
                                                    "ContractCodeProjectCode", "Contract/Project code",
                                                    "ContractCodeProjectCode");
                        }
                    }
                }
                else
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5018,
                                                        "", "",
                                                        "");
                }

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return res;

                //============================ Update Installation Basic ====================
                if (sParam.do_TbtInstallationBasic.MaintenanceNo == null)
                {
                    handler.DeleteTbt_InstallationBasic(sParam.do_TbtInstallationBasic.ContractProjectCode);
                }
                else
                {
                    sParam.do_TbtInstallationBasic.SlipNo = null;
                    handler.UpdateTbt_InstallationBasic(sParam.do_TbtInstallationBasic);
                }                
                //===========================================================================

                
                //================== Update Installation Slip =========================
                if (sParam.do_TbtInstallationSlip != null)
                {
                    sParam.do_TbtInstallationSlip.SlipStatus = SlipStatus.C_SLIP_STATUS_INSTALL_SLIP_CANCELED;
                    handler.UpdateTbt_InstallationSlip(sParam.do_TbtInstallationSlip);
                }
                //=====================================================================

                //==================== DELETE MEMO ===============================
                handler.DeleteTbt_InstallationMemo(sParam.do_TbtInstallationBasic.ContractProjectCode, sParam.do_TbtInstallationBasic.SlipNo, ScreenID.C_SCREEN_ID_INSTALL_SLIP);
                //================================================================
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }
        /// <summary>
        /// Process canel slip and use previous slip
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ObjectResultData ISS070_CancelSlipUsePrevious(ISS070_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ValidatorUtil validator = new ValidatorUtil();
            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            try
            {
                if (sParam.do_TbtInstallationBasic == null || sParam.do_TbtInstallationBasic.InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                {                
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5018,
                                                        "", "",
                                                        "");                
                }

                if (sParam.do_TbtInstallationSlip == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5055,
                                                    "SlipNo", "Slip No.",
                                                    "SlipNo");
                }
                else
                {
                    if (sParam.do_TbtInstallationSlip.PreviousSlipNo == null)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5056,
                                                    "SlipNo", "Slip No.",
                                                    "SlipNo");
                    }

                    if (sParam.do_TbtInstallationSlip.SlipStatus == SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5054,
                                                    "SlipNo", "Slip No.",
                                                    "SlipNo");
                    }
                }

                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    if ( (sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE || sParam.dtSale.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE)
                        && sParam.dtSale.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE )
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5051,
                                                    "SlipNo", "Slip No.",
                                                    "SlipNo");
                    }
                    
                }
                

                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                    return res;

                //============================ Update Installation Basic ====================
                if (sParam.do_TbtInstallationBasic != null)
                {
                    sParam.do_TbtInstallationBasic.SlipNo = sParam.do_TbtInstallationSlip.PreviousSlipNo;
                    sParam.do_TbtInstallationBasic.InstallationType = sParam.do_TbtInstallationSlip.InstallationType;
                    sParam.do_TbtInstallationBasic.PlanCode = sParam.do_TbtInstallationSlip.PlanCode;
                    handler.UpdateTbt_InstallationBasic(sParam.do_TbtInstallationBasic);
                }
                //===========================================================================

                //================== Update Installation Slip =========================
                if (sParam.do_TbtInstallationSlip != null)
                {
                    sParam.do_TbtInstallationSlip.SlipStatus = SlipStatus.C_SLIP_STATUS_INSTALL_SLIP_CANCELED;
                    handler.UpdateTbt_InstallationSlip(sParam.do_TbtInstallationSlip);
                }
                //=====================================================================
                
                //=================== Update Previous Slip ============================
                if (sParam.do_TbtInstallationSlip.PreviousSlipNo != null)
                {
                    tbt_InstallationSlip doPreviousSlip = handler.GetTbt_InstallationSlipData(sParam.do_TbtInstallationSlip.PreviousSlipNo);

                    doPreviousSlip.SlipStatus = sParam.do_TbtInstallationSlip.PreviousSlipStatus;
                    handler.UpdateTbt_InstallationSlip(doPreviousSlip);
                }
                //=====================================================================

                //==================== DELETE MEMO ===============================
                handler.DeleteTbt_InstallationMemo(sParam.do_TbtInstallationBasic.ContractProjectCode, sParam.do_TbtInstallationBasic.SlipNo, ScreenID.C_SCREEN_ID_INSTALL_SLIP);
                //================================================================
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }
        /// <summary>
        /// Clear all email in session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS070_ClearInstallEmail(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS070_ScreenParameter session = GetScreenObject<ISS070_ScreenParameter>();
            session.ListDOEmail = null;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Clear all instrument detail in session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS070_ClearInstrumentInfo(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS070_ScreenParameter session = GetScreenObject<ISS070_ScreenParameter>();
            session.do_TbtInstallationSlipDetails = null;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Clear all Attach data from session parameter
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <param name="ObjectID"></param>
        public void ISS070_DeleteAllAttachFile(string strMaintenanceNo,string ObjectID)
        {
            IInstallationHandler iHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            ICommonHandler cHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<tbt_InstallationAttachFile> doTbt_InstallationAttachFile = iHandler.GetTbt_InstallationAttachFile(null, strMaintenanceNo, ObjectID);
            if (doTbt_InstallationAttachFile != null)
            {
                foreach (tbt_InstallationAttachFile AttachData in doTbt_InstallationAttachFile)
                {
                    cHandler.DeleteAttachFileByID(AttachData.AttachFileID, strMaintenanceNo);
                }
                iHandler.DeleteTbt_InstallationAttachFile(null, strMaintenanceNo, ObjectID);
            }
            
        }
        /// <summary>
        /// Clear contract code data in session parameter
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public ActionResult ISS070_ClearCommonContractCode(string temp)
        {
            ObjectResultData res = new ObjectResultData();

            //---- Clear ContractCode ----- Add by Teerapong S. 26/Apr/2012 =======================
            //CommonUtil.dsTransData.dtCommonSearch.ContractCode = null;
            //CommonUtil.dsTransData.dtCommonSearch.ProjectCode = null;
            //====================================================================================
            ISS070_ScreenParameter param = GetScreenObject<ISS070_ScreenParameter>();
            if (param != null)
            {
                param.ContractCodeShort = null;
                param.strContractProjectCode = null;
                param.CommonSearch = new ScreenParameter.CommonSearchDo();
            }
            return Json(res);
        }

    }
}
