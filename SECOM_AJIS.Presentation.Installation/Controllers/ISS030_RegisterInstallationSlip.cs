
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

namespace SECOM_AJIS.Presentation.Installation.Controllers
{
    public partial class InstallationController : BaseController
    {
        /// <summary>
        /// Authority screen ISS030
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ISS030_Authority(ISS030_ScreenParameter param)
        {
            //IInstallationDocumentHandler docHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            //docHand.CreateReportDeliveryConfirmData("60000120120101");
            #region ================ Check suspend and permission =================
            ObjectResultData res = new ObjectResultData();
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            if (chandler.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_SLIP, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }
            #endregion

            // parameter
            //ISS030_ScreenParameter param = new ISS030_ScreenParameter();
            #region //=============== Add new spec 26/04/2012 Get Common Contract COde for Search ==============

            if (String.IsNullOrEmpty(param.strContractCode) && param.CommonSearch != null)
            {
                if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                {
                    param.strContractCode = param.CommonSearch.ContractCode;
                }
                else if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ProjectCode) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0034);
                    return Json(res);
                }
            }

            #endregion //======================================================================================
            param.ContractCodeShort = param.strContractCode;

            #region //=============== Add new spec 21/03/2012 Validate Contract Error ===================
            if (CommonUtil.IsNullOrEmpty(param.ContractCodeShort))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147);
                return Json(res);
            }

            CommonUtil comUtil = new CommonUtil();
            string strContractCodeLong = comUtil.ConvertContractCode(param.ContractCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
            param.ContractCodeLong = strContractCodeLong;
            dtRentalContractBasicForInstall dtRentalContractBasic = new dtRentalContractBasicForInstall();
            dtSaleBasic dtSaleContractBasic = new dtSaleBasic();
            IRentralContractHandler RentalContactHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            dtRentalContractBasic = RentalContactHandler.GetRentalContractBasicDataForInstall(param.ContractCodeLong, MiscType.C_BUILDING_TYPE);
            if (dtRentalContractBasic != null)
            {
                param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                param.InstallType = MiscType.C_RENTAL_INSTALL_TYPE;

                param.ContractCodeLong = dtRentalContractBasic.ContractCode;
                param.ContractCodeShort = comUtil.ConvertContractCode(param.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);

                res = ISS030_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, param.ServiceTypeCode, param.ContractCodeLong);
                if (res.IsError)
                {
                    return Json(res);
                }
            }
            else
            {
                ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);
                if (dtSaleContractBasic != null)
                {
                    param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                    param.InstallType = MiscType.C_SALE_INSTALL_TYPE;

                    param.ContractCodeLong = dtSaleContractBasic.ContractCode;
                    param.ContractCodeShort = comUtil.ConvertContractCode(param.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);

                    res = ISS030_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, param.ServiceTypeCode, param.ContractCodeLong);
                    if (res.IsError)
                    {
                        return Json(res);
                    }
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124);
                    return Json(res);
                }
            }

            if (param.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
            {
                if (ISS030_ValidExistOffice(dtRentalContractBasic.OperationOfficeCode) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    return Json(res);
                }
            }
            else if (param.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
            {
                if (ISS030_ValidExistOffice(dtSaleContractBasic.OperationOfficeCode) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    return Json(res);
                }
            }
            //======================================================================================
            #endregion

            return InitialScreenEnvironment<ISS030_ScreenParameter>("ISS030", param, res);

        }
        /// <summary>
        /// Initial screen ISS030
        /// </summary>
        /// <returns></returns>
        [Initialize("ISS030")]
        public ActionResult ISS030()
        {
            //ISS030_ScreenParameter param = new ISS030_ScreenParameter();
            ISS030_ScreenParameter param = GetScreenObject<ISS030_ScreenParameter>();
            if (param != null)
            {
                ViewBag.ContractProjectCode = param.ContractCodeShort;
                ViewBag.ContractCodeLong = param.ContractCodeLong;
            }
            ViewBag.AttachKey = GetCurrentKey();
            return View();
        }

        // InitialGridEmail
        /// <summary>
        /// Initial grid email schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS030_InitialGridEmail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS010_Email", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// Initial grid instrument schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS030_InitialGridInstrumentInfo()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS030_InstrumentDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// Initial grid facility schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS030_InitialGridFacility()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS030_Facility", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        //InitialGridAttachedFile

        /// <summary>
        /// Retrieve data for show in screen ISS030
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public ActionResult ISS030_RetrieveData(string strContractCode)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            dtRentalContractBasicForInstall dtRentalContractBasic = new dtRentalContractBasicForInstall();
            dtSaleBasic dtSaleContractBasic = new dtSaleBasic();
            string lang = CommonUtil.GetCurrentLanguage();
            string ContractProjectCodeForShow = "";
            try
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ISS030_ScreenParameter sParam = GetScreenObject<ISS030_ScreenParameter>();
                sParam.ContractCodeShort = strContractCode;

                //Check mandatory
                if (String.IsNullOrEmpty(strContractCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { strContractCode });
                    return Json(res);
                }

                //Get rental contract data

                ///////////// START RETRIEVE DATA ////////////////////////
                #region ============== Get Contract Data ================
                string strContractCodeLong = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                sParam.ContractCodeLong = strContractCodeLong;
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                IRentralContractHandler RentalContactHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                dtRentalContractBasic = RentalContactHandler.GetRentalContractBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);
                IRentralContractHandler cHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                if (res.IsError)
                    return Json(res);

                if (dtRentalContractBasic != null)
                {
                    #region ================ prepare data Rental =====================
                    ContractProjectCodeForShow = comUtil.ConvertContractCode(dtRentalContractBasic.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                    sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                    sParam.InstallType = MiscType.C_RENTAL_INSTALL_TYPE;
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
                    #endregion
                }
                else
                {

                    dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);

                    if (dtSaleContractBasic != null)
                    {
                        #region ===================== prepare data sale ======================
                        ContractProjectCodeForShow = comUtil.ConvertContractCode(dtSaleContractBasic.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                        sParam.InstallType = MiscType.C_SALE_INSTALL_TYPE;
                        sParam.dtSale = dtSaleContractBasic;

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
                        #endregion
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5063, new string[] { "ContractCode" });
                        return Json(res);
                    }
                }
                #endregion

                #region ////////////////////// RETRIEVE Installation Basic /////////////////

                sParam.do_TbtInstallationBasic = null;
                List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                if (tmpdoTbt_InstallationBasic != null)
                {
                    sParam.do_TbtInstallationBasic = new tbt_InstallationBasic();
                    sParam.do_TbtInstallationBasic = tmpdoTbt_InstallationBasic[0];
                }

                if (sParam.do_TbtInstallationBasic != null)
                {
                    sParam.do_TbtInstallationBasic.ServiceTypeCode = sParam.ServiceTypeCode;
                    if (!CommonUtil.IsNullOrEmpty(sParam.do_TbtInstallationBasic.SlipNo))
                    {
                        sParam.m_blnbFirstTimeRegister = false;
                        sParam.MaintenanceNo = sParam.do_TbtInstallationBasic.SlipNo;
                        /////////////////// RETRIEVE EMAIL ////////////////////////
                        List<tbt_InstallationEmail> ListEmail = handler.GetTbt_InstallationEmailData(sParam.do_TbtInstallationBasic.SlipNo);
                        ISS030_DOEmailData TempEmail;
                        List<ISS030_DOEmailData> listAddEmail = new List<ISS030_DOEmailData>();
                        if (ListEmail != null)
                        {
                            foreach (tbt_InstallationEmail dataEmail in ListEmail)
                            {
                                TempEmail = new ISS030_DOEmailData();
                                TempEmail.EmailAddress = dataEmail.EmailNoticeTarget;
                                listAddEmail.Add(TempEmail);
                            }
                        }

                        sParam.ListDOEmail = listAddEmail;
                    }
                    else
                    {
                        sParam.m_blnbFirstTimeRegister = true;
                        sParam.ListDOEmail = null;
                    }
                }
                else
                {
                    sParam.m_blnbFirstTimeRegister = true;
                    sParam.ListDOEmail = null;
                }
                #endregion

                //*******************************************************************************************************************
                ////////////////// INITIAL INSTRUMENT DETAILS /////////////////////
                //sParam.do_TbtInstallationSlipDetails
                List<dtTbt_RentalInstrumentDetailsListForView> list = new List<dtTbt_RentalInstrumentDetailsListForView>();


                if (!sParam.m_blnbFirstTimeRegister)
                {
                    ////////////////// RETRIEVE SLIP ///////////////////////////
                    sParam.do_TbtInstallationSlip = handler.GetTbt_InstallationSlipData(sParam.do_TbtInstallationBasic.SlipNo);
                    if (sParam.do_TbtInstallationSlip != null)
                    {
                        IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                        List<tbs_MiscellaneousTypeCode> masterlist = masterHandler.GetTbs_MiscellaneousTypeCode("slipstatus");
                        foreach (var item in masterlist)
                        {
                            if (item.ValueCode == sParam.do_TbtInstallationSlip.SlipStatus)
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
                    }
                }
                else
                {
                    sParam.do_TbtInstallationSlip = null;
                }

                string strInstallationType = null; //Add by Jutarat A. on 17062013
                string strSlipNo = null; //Add by Jutarat A. on 17062013

                //======================= TRS 01/06/2012 InitialInstrumentDetail ========================
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    string strOCC = null;
                    string strUnimplementOCC = cHandler.GetLastUnimplementedOCC(sParam.dtRentalContractBasic.ContractCode);
                    if (strUnimplementOCC == null)
                    {
                        strOCC = sParam.dtRentalContractBasic.OCC;
                    }
                    else
                    {
                        strOCC = strUnimplementOCC;
                    }

                    List<tbt_RentalSecurityBasic> doTbt_RentalSecurityBasic = cHandler.GetTbt_RentalSecurityBasic(sParam.dtRentalContractBasic.ContractCode, strOCC);
                    bool blnUseContractData = false;

                    if ((sParam.do_TbtInstallationSlip == null)
                        || (sParam.do_TbtInstallationSlip != null
                            && doTbt_RentalSecurityBasic != null
                            && doTbt_RentalSecurityBasic.Count > 0
                            //&& doTbt_RentalSecurityBasic[0].UpdateDate > sParam.do_TbtInstallationSlip.UpdateDate))
                            && doTbt_RentalSecurityBasic[0].UpdateDate > sParam.do_TbtInstallationSlip.CreateDate)) //Modify by Jutarat A. on 21062013
                    {
                        blnUseContractData = true;
                    }
                    else
                    {
                        blnUseContractData = false;
                    }
                    sParam.blnUseContractData = blnUseContractData;

                    //Add by Jutarat A. on 17062013
                    if (sParam.do_TbtInstallationBasic != null)
                    {
                        strInstallationType = sParam.do_TbtInstallationBasic.InstallationType;
                        strSlipNo = sParam.do_TbtInstallationBasic.SlipNo;
                    }
                    //End Add

                    //Modify by Jutarat A. on 17062013
                    //if (sParam.do_TbtInstallationBasic != null)
                    //    sParam.doRentalInstrumentdataList = handler.GetRentalInstrumentdataList(strContractCodeLong, strOCC, sParam.do_TbtInstallationBasic.SlipNo, InstrumentType.C_INST_TYPE_GENERAL);
                    //else
                    //    sParam.doRentalInstrumentdataList = handler.GetRentalInstrumentdataList(strContractCodeLong, strOCC, null, InstrumentType.C_INST_TYPE_GENERAL);

                    sParam.doRentalInstrumentdataList = handler.GetRentalInstrumentdataList(strContractCodeLong, strOCC, strSlipNo, InstrumentType.C_INST_TYPE_GENERAL, strInstallationType);

                    if (strInstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                    {
                        sParam.doRentalInstrumentdataExchangeList = CommonUtil.ClonsObjectList<doRentalInstrumentdataList, doRentalInstrumentdataList>(sParam.doRentalInstrumentdataList);
                    }
                    else
                    {
                        sParam.doRentalInstrumentdataExchangeList = handler.GetRentalInstrumentdataList(strContractCodeLong, strOCC, strSlipNo, InstrumentType.C_INST_TYPE_GENERAL, RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE);
                    }
                    //End Modify

                    IInstrumentMasterHandler InstrumentHandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    doInstrumentSearchCondition dCond = new doInstrumentSearchCondition() { };
                    List<doInstrumentData> lst = InstrumentHandler.GetInstrumentDataForSearch(dCond);

                    if (sParam.doRentalInstrumentdataList != null && sParam.doRentalInstrumentdataList.Count > 0)
                    {
                        foreach (doRentalInstrumentdataList instData in sParam.doRentalInstrumentdataList)
                        {
                            List<doInstrumentData> instTemp = (from t in lst
                                                               where t.InstrumentCode == instData.InstrumentCode
                                                               select t).ToList<doInstrumentData>();
                            if (instTemp != null && instTemp.Count > 0)
                            {
                                instData.InstrumentName = instTemp[0].InstrumentName;
                                instData.InstrumentPrice = (instTemp[0].RentalUnitPrice == null) ? 0 : (decimal)instTemp[0].RentalUnitPrice;
                            }
                        }
                    }

                    //Add by Jutarat A. on 17062013
                    if (sParam.doRentalInstrumentdataExchangeList != null && sParam.doRentalInstrumentdataExchangeList.Count > 0)
                    {
                        foreach (doRentalInstrumentdataList instData in sParam.doRentalInstrumentdataExchangeList)
                        {
                            List<doInstrumentData> instTemp = (from t in lst
                                                               where t.InstrumentCode == instData.InstrumentCode
                                                               select t).ToList<doInstrumentData>();
                            if (instTemp != null && instTemp.Count > 0)
                            {
                                instData.InstrumentName = instTemp[0].InstrumentName;
                                instData.InstrumentPrice = (instTemp[0].RentalUnitPrice == null) ? 0 : (decimal)instTemp[0].RentalUnitPrice;
                            }
                        }
                    }
                    //End Add
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    //Add by Jutarat A. on 17062013
                    string strSaleProcessManageStatus = null;
                    if (sParam.dtSale != null)
                        strSaleProcessManageStatus = sParam.dtSale.SaleProcessManageStatus;

                    if (sParam.do_TbtInstallationBasic != null)
                    {
                        strInstallationType = sParam.do_TbtInstallationBasic.InstallationType;
                        strSlipNo = sParam.do_TbtInstallationBasic.SlipNo;
                    }
                    //End Add

                    //Modify by Jutarat A. on 17062013
                    //if (sParam.do_TbtInstallationBasic != null)
                    //    sParam.doSaleInstrumentdataList = handler.GetSaleInstrumentdataList(strContractCodeLong, sParam.dtSale.OCC, sParam.do_TbtInstallationBasic.SlipNo, InstrumentType.C_INST_TYPE_GENERAL, sParam.dtSale.ChangeType, sParam.dtSale.InstallationCompleteFlag); //Add (ChangeType and  InstallCompleteFlag) by Jutarat A. on 27052013
                    //else
                    //    sParam.doSaleInstrumentdataList = handler.GetSaleInstrumentdataList(strContractCodeLong, sParam.dtSale.OCC, null, InstrumentType.C_INST_TYPE_GENERAL, sParam.dtSale.ChangeType, sParam.dtSale.InstallationCompleteFlag); //Add (ChangeType and InstallCompleteFlag) by Jutarat A. on 27052013

                    sParam.doSaleInstrumentdataList = handler.GetSaleInstrumentdataList(strContractCodeLong, sParam.dtSale.OCC, strSlipNo, InstrumentType.C_INST_TYPE_GENERAL, sParam.dtSale.ChangeType, sParam.dtSale.InstallationCompleteFlag, strInstallationType, strSaleProcessManageStatus);

                    if (strInstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                    {
                        sParam.doSaleInstrumentdataExchangeList = CommonUtil.ClonsObjectList<doSaleInstrumentdataList, doSaleInstrumentdataList>(sParam.doSaleInstrumentdataList);
                    }
                    else
                    {
                        sParam.doSaleInstrumentdataExchangeList = handler.GetSaleInstrumentdataList(strContractCodeLong, sParam.dtSale.OCC, strSlipNo, InstrumentType.C_INST_TYPE_GENERAL, sParam.dtSale.ChangeType, sParam.dtSale.InstallationCompleteFlag, SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE, strSaleProcessManageStatus);
                    }
                    //End Modify

                    IInstrumentMasterHandler InstrumentHandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    doInstrumentSearchCondition dCond = new doInstrumentSearchCondition() { };
                    List<doInstrumentData> lst = InstrumentHandler.GetInstrumentDataForSearch(dCond);

                    if (sParam.doSaleInstrumentdataList != null && sParam.doSaleInstrumentdataList.Count > 0)
                    {
                        foreach (doSaleInstrumentdataList instData in sParam.doSaleInstrumentdataList)
                        {
                            List<doInstrumentData> instTemp = (from t in lst
                                                               where t.InstrumentCode == instData.InstrumentCode
                                                               select t).ToList<doInstrumentData>();
                            if (instTemp != null && instTemp.Count > 0)
                            {
                                instData.InstrumentName = instTemp[0].InstrumentName;
                                instData.InstrumentPrice = (instTemp[0].SaleUnitPrice == null) ? 0 : (decimal)instTemp[0].SaleUnitPrice;
                            }
                        }
                    }

                    //Add by Jutarat A. on 17062013
                    if (sParam.doSaleInstrumentdataExchangeList != null && sParam.doSaleInstrumentdataExchangeList.Count > 0)
                    {
                        foreach (doSaleInstrumentdataList instData in sParam.doSaleInstrumentdataExchangeList)
                        {
                            List<doInstrumentData> instTemp = (from t in lst
                                                               where t.InstrumentCode == instData.InstrumentCode
                                                               select t).ToList<doInstrumentData>();
                            if (instTemp != null && instTemp.Count > 0)
                            {
                                instData.InstrumentName = instTemp[0].InstrumentName;
                                instData.InstrumentPrice = (instTemp[0].SaleUnitPrice == null) ? 0 : (decimal)instTemp[0].SaleUnitPrice;
                            }
                        }
                    }
                    //End Add
                }
                //======================================================================================
                ///////////////////////////////////////////////////////////////////

                //////////////////////// InitialFacilityDetail ////////////////////////////////

                if (sParam.m_blnbFirstTimeRegister == false)
                {
                    List<tbt_InstallationSlipDetails> ListTemp_TbtInstallationSlipDetails = new List<tbt_InstallationSlipDetails>();
                    ListTemp_TbtInstallationSlipDetails = handler.GetTbt_InstallationSlipDetailsData(sParam.do_TbtInstallationBasic.SlipNo, null, InstrumentType.C_INST_TYPE_MONITOR);
                    if (ListTemp_TbtInstallationSlipDetails != null)
                    {
                        sParam.do_TbtInstallationSlipDetailsForFacility = ListTemp_TbtInstallationSlipDetails;

                        IInstrumentMasterHandler InstrumentHandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;

                        List<dtInstrument> lst = InstrumentHandler.GetInstrument(null, null, null, null);
                        foreach (tbt_InstallationSlipDetails dataInstrument in sParam.do_TbtInstallationSlipDetailsForFacility)
                        {
                            int IndexInstrument = lst.FindIndex(delegate (dtInstrument s) { return s.InstrumentCode == dataInstrument.InstrumentCode; });

                            if (IndexInstrument >= 0)
                            {
                                dataInstrument.InstrumentName = lst[IndexInstrument].InstrumentName;
                            }
                        }
                    }
                }
                else
                {
                    string strOCC = null;
                    string strUnimplementOCC = null;
                    if (sParam.dtRentalContractBasic != null && sParam.dtRentalContractBasic.ContractCode != null)
                        strUnimplementOCC = cHandler.GetLastUnimplementedOCC(sParam.dtRentalContractBasic.ContractCode);

                    if (strUnimplementOCC == null)
                    {
                        if (sParam.dtRentalContractBasic != null && sParam.dtRentalContractBasic.ContractCode != null)
                            strOCC = sParam.dtRentalContractBasic.OCC;
                    }
                    else
                    {
                        strOCC = strUnimplementOCC;
                    }

                    if (sParam.dtRentalContractBasic != null)
                        sParam.dtRentalInstrumentDetailsForFacility = cHandler.GetTbt_RentalInstrumentDetailsListForView(sParam.dtRentalContractBasic.ContractCode, strOCC, null, InstrumentType.C_INST_TYPE_MONITOR);

                }
                ///////////////////////////////////////////////////////////////////////////////

                ///////////////// Initial Installation Slip Output Target //////////////////
                IInventoryHandler iHandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                if ((sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL && dtRentalContractBasic.ProjectCode != null)
                || (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE && dtSaleContractBasic.ProjectCode != null)
                || sParam.m_blnbFirstTimeRegister)
                {
                    List<doOffice> doOfficeList = iHandler.GetInventoryHeadOffice();
                    if (doOfficeList != null && doOfficeList.Count > 0)
                    {
                        sParam.InstallationOfficeOutputTarget = doOfficeList[0].OfficeCode;
                    }
                }
                ////////////////////////////////////////////////////////////////////////////

                //====================== Get Rental Fee =================================
                List<doRentalFeeResult> doRentalFeeList = null;
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    doRentalFeeList = handler.GetRentalFee(sParam.ContractCodeLong);
                }
                //=======================================================================

                //====================== get for initial installation type ==========================
                IRentralContractHandler rentalContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                string tmpStrOCC = rentalContractHandler.GetLastUnimplementedOCC(sParam.ContractCodeLong);
                bool blnCheckCP12 = false;
                if (!CommonUtil.IsNullOrEmpty(tmpStrOCC))
                {
                    blnCheckCP12 = rentalContractHandler.CheckCP12(sParam.ContractCodeLong, tmpStrOCC);
                }
                //===================================================================================

                if (res.IsError)
                    return Json(res);

                UpdateScreenObject(sParam);

                ISS030_RegisterStartResumeTargetData result = new ISS030_RegisterStartResumeTargetData();

                result.ContractProjectCodeForShow = ContractProjectCodeForShow;

                sParam.ContractCodeShort = ContractProjectCodeForShow;
                result.blnCheckCP12 = blnCheckCP12;
                result.dtRentalContractBasic = dtRentalContractBasic;
                result.dtSale = dtSaleContractBasic;
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

                result.do_TbtInstallationSlipDetailsForFacility = sParam.do_TbtInstallationSlipDetailsForFacility;
                result.dtRentalInstrumentDetailsForFacility = sParam.dtRentalInstrumentDetailsForFacility;
                result.ContractCodeShort = sParam.ContractCodeShort;
                result.blnUseContractData = sParam.blnUseContractData;
                result.doRentalInstrumentdataList = sParam.doRentalInstrumentdataList;
                result.doSaleInstrumentdataList = sParam.doSaleInstrumentdataList;
                result.InstallationOfficeOutputTarget = sParam.InstallationOfficeOutputTarget;

                result.doRentalInstrumentdataExchangeList = sParam.doRentalInstrumentdataExchangeList; //Add by Jutarat A. on 17062013
                result.doSaleInstrumentdataExchangeList = sParam.doSaleInstrumentdataExchangeList; //Add by Jutarat A. on 17062013

                if (doRentalFeeList != null && doRentalFeeList.Count > 0)
                {
                    result.doRentalFeeResult = doRentalFeeList[0];
                }
                else
                {
                    result.doRentalFeeResult = null;
                }

                var rentalContractBasic = RentalContactHandler.GetTbt_RentalContractBasic(strContractCodeLong, null).FirstOrDefault();
                result.RentalContactBasicLastChangeType = (rentalContractBasic == null ? null : rentalContractBasic.LastChangeType);

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
        public ActionResult ISS030_GetInstallEmail(string strEmail)
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
                    List<ISS030_DOEmailData> listEmail;
                    ISS030_ScreenParameter session = GetScreenObject<ISS030_ScreenParameter>();
                    ISS030_DOEmailData doISS030EmailADD = new ISS030_DOEmailData();

                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS030_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    doISS030EmailADD.EmpNo = emailList[0].EmpNo;
                    doISS030EmailADD.EmailAddress = emailList[0].EmailAddress;

                    if (listEmail.FindAll(delegate (ISS030_DOEmailData s) { return s.EmailAddress == doISS030EmailADD.EmailAddress; }).Count() == 0)
                        listEmail.Add(doISS030EmailADD);
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
        /// Validate register data
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS030_ValidateRegisterData()
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
        /// Validate Email
        /// </summary>
        /// <param name="doISS030Email"></param>
        /// <returns></returns>
        public ActionResult ValidateEmail_ISS030(ISS030_DOEmailData doISS030Email)
        {
            List<dtGetEmailAddress> dtEmail;
            ISS030_DOEmailData doISS030EmailADD;
            IEmployeeMasterHandler employeeMasterHandler;
            ObjectResultData res = new ObjectResultData();
            ISS030_ScreenParameter session;
            employeeMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            dtEmail = employeeMasterHandler.GetEmailAddress(doISS030Email.EmpFirstNameEN, doISS030Email.EmailAddress, doISS030Email.OfficeCode, doISS030Email.DepartmentCode);

            try
            {
                session = GetScreenObject<ISS030_ScreenParameter>();
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                    {
                        res.ResultData = false;
                        return Json(res);
                    }
                }


                //doISS030EmailADD = new ISS030_DOEmailData();
                //doISS030EmailADD.EmpNo = "540886";
                //doISS030EmailADD.EmailAddress = "Nattapong@csithai.com";
                //session.DOEmail = doISS030EmailADD;

                if (dtEmail != null)
                {
                    if (dtEmail.Count() == 0)
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, doISS030Email.EmailAddress);
                    else
                    {
                        doISS030EmailADD = new ISS030_DOEmailData();
                        doISS030EmailADD.EmpNo = dtEmail[0].EmpNo;
                        doISS030EmailADD.EmailAddress = doISS030Email.EmailAddress;
                        session.DOEmail = doISS030EmailADD;

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
        public ActionResult ISS030_Upload()
        {
            ViewBag.K = GetCurrentKey();
            return View();
        }

        /// <summary>
        /// Initial attach file section
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <param name="DocName"></param>
        /// <param name="action"></param>
        /// <param name="delID"></param>
        /// <returns></returns>
        public ActionResult ISS030_AttachFile(HttpPostedFileBase uploadedFile, string DocName, string action, string delID)
        {

            return View("ISS030_Upload");
        }
        /// <summary>
        /// Get data for installation type combobox
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS030_GetMiscInstallationtype(string strFieldName)
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
        /// Get email from screen to session parameter
        /// </summary>
        /// <param name="doCTS053Email"></param>
        /// <returns></returns>
        public ActionResult GetEmail_ISS030(ISS030_DOEmailData doCTS053Email)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            ISS030_DOEmailData doISS030EmailADD;
            List<ISS030_DOEmailData> listEmail;
            ISS030_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS030_ScreenParameter>();
                doISS030EmailADD = new ISS030_DOEmailData();
                if (session.ListDOEmail == null)
                    listEmail = new List<ISS030_DOEmailData>();
                else
                    listEmail = session.ListDOEmail;

                doISS030EmailADD.EmpNo = session.DOEmail.EmpNo;
                doISS030EmailADD.EmailAddress = doCTS053Email.EmailAddress;

                if (listEmail.FindAll(delegate (ISS030_DOEmailData s) { return s.EmpNo == doISS030EmailADD.EmpNo; }).Count() == 0)
                    listEmail.Add(doISS030EmailADD);

                session.DOEmail = null;
                session.ListDOEmail = listEmail;
                res.ResultData = CommonUtil.ConvertToXml<ISS030_DOEmailData>(session.ListDOEmail, "Contract\\CTS053Email", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get email list from screen to session parameter
        /// </summary>
        /// <param name="listEmailAdd"></param>
        /// <returns></returns>
        public ActionResult GetEmailList_ISS030(List<ISS030_DOEmailData> listEmailAdd)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            List<ISS030_DOEmailData> listEmail;
            List<ISS030_DOEmailData> listNewEmail;
            ISS030_ScreenParameter session;

            try
            {
                listNewEmail = new List<ISS030_DOEmailData>();
                if (listEmailAdd != null)
                {
                    session = GetScreenObject<ISS030_ScreenParameter>();
                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS030_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    foreach (var item in listEmailAdd)
                    {
                        if (listEmail.FindAll(delegate (ISS030_DOEmailData s) { return s.EmailAddress == item.EmailAddress; }).Count() == 0)
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
        /// Remove email and show data to grid email
        /// </summary>
        /// <param name="doISS030Email"></param>
        /// <returns></returns>
        public ActionResult RemoveMailClick_ISS030(ISS030_DOEmailData doISS030Email)
        {
            List<ISS030_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS030_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS030_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate (ISS030_DOEmailData s) { return s.EmailAddress == doISS030Email.EmailAddress; });
                if (listEmailDelete.Count() != 0)
                    session.ListDOEmail.Remove(listEmailDelete[0]);

                res.ResultData = CommonUtil.ConvertToXml<ISS030_DOEmailData>(session.ListDOEmail, "Installation\\ISS010Email", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); ;
            }

            return Json(res);
        }
        /// <summary>
        /// Remove email data from session parameter
        /// </summary>
        /// <param name="doISS030Email"></param>
        /// <returns></returns>
        public ActionResult ISS030_RemoveMailClick(ISS030_DOEmailData doISS030Email)
        {
            List<ISS030_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS030_ScreenParameter session;
            try
            {
                session = GetScreenObject<ISS030_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate (ISS030_DOEmailData s) { return s.EmailAddress == doISS030Email.EmailAddress; });
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
        /// Register installation slip
        /// </summary>
        /// <param name="doIB"></param>
        /// <param name="doIS"></param>
        /// <param name="Memo"></param>
        /// <param name="DataValid"></param>
        /// <param name="FacilityList"></param>
        /// <returns></returns>
        public ActionResult ISS030_RegisterData(tbt_InstallationBasic doIB, tbt_InstallationSlip doIS, string Memo, ISS030_ValidateData DataValid, List<tbt_InstallationSlipDetails> FacilityList)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
            string StrSlipNo;
            try
            {
                //////////////////// VALIDATE SUSPEND ///////////////////////
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_SLIP, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                /////////////////////////////////////////////////////////////

                ISS030_ScreenParameter sParam = GetScreenObject<ISS030_ScreenParameter>();

                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                doGenerateInstallationSlipNoCond doGenSlipNo = new doGenerateInstallationSlipNoCond();

                //================= Teerapong S. 15/10/2012 =======================
                List<tbt_InstallationBasic> doValidate_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);

                if ((sParam.do_TbtInstallationBasic == null && doValidate_InstallationBasic != null)
                    || (sParam.do_TbtInstallationBasic != null && doValidate_InstallationBasic == null)
                    || (sParam.do_TbtInstallationBasic != null && doValidate_InstallationBasic != null && DateTime.Compare(sParam.do_TbtInstallationBasic.UpdateDate.Value, doValidate_InstallationBasic[0].UpdateDate.Value) != 0)
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    return Json(res);
                }
                //=================================================================

                //////////////////// Check HeadOffice Flag //////////////////
                IInventoryHandler iHandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                string strHeadOfficeFlag;
                List<doOffice> doOfficeList = iHandler.GetInventoryHeadOffice();
                if (doOfficeList != null && doOfficeList.Count > 0 && doIS.SlipIssueOfficeCode == doOfficeList[0].OfficeCode)
                {
                    strHeadOfficeFlag = InstallationSlipNo.C_INSTALL_SLIP_NO_HEAD_OFFICE;
                }
                else
                {
                    strHeadOfficeFlag = InstallationSlipNo.C_INSTALL_SLIP_NO_OTHER_OFFICE;
                }
                /////////////////////////////////////////////////////////////

                //////////////////// VALIDATE BUSINESS //////////////////////
                //validator = ISS030_ValidateBusiness(sParam.ContractCodeLong, sParam.ServiceTypeCode, sParam.InstallType);
                if (res.IsError)
                    return Json(res);
                /////////////////////////////////////////////////////////////

                //////////////////// PREPARE DATA //////////////////////////////////////////////////////////
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    doGenSlipNo.strOfficeCode = sParam.dtRentalContractBasic.OperationOfficeCode;
                    doIB.OperationOfficeCode = sParam.dtRentalContractBasic.OperationOfficeCode;
                    doIB.OCC = sParam.dtRentalContractBasic.OCC;
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    doGenSlipNo.strOfficeCode = sParam.dtSale.OperationOfficeCode;
                    doIB.OperationOfficeCode = sParam.dtSale.OperationOfficeCode;
                    doIB.OCC = sParam.dtSale.OCC;
                }

                //======================== SLIPID ==========================
                if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW
                    || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW)
                {
                    doGenSlipNo.strSlipID = SlipID.C_INV_SLIPID_NEW_RENTAL;
                }
                else if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
                    || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                    || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                    || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE
                    || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                    || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                    || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                    || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_CHANGE_WIRING
                    || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                    || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MOVE
                    || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_PARTIAL_REMOVE
                    )
                {
                    doGenSlipNo.strSlipID = SlipID.C_INV_SLIPID_CHANGE_INSTALLATION;
                }
                else if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL)
                {
                    doGenSlipNo.strSlipID = SlipID.C_INV_SLIPID_REMOVE_INSTALLATION;
                }
                else if (doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD)
                {
                    doGenSlipNo.strSlipID = SlipID.C_INV_SLIPID_NEW_SALE;
                }
                //==========================================================
                doIS.SlipType = doGenSlipNo.strSlipID;
                string strSlipID2Digit = doGenSlipNo.strSlipID;
                doGenSlipNo.strSlipID = doGenSlipNo.strSlipID + strHeadOfficeFlag;

                using (TransactionScope scope = new TransactionScope())
                {
                    StrSlipNo = handler.GenerateInstallationSlipNo(doGenSlipNo);

                    doIB.ContractProjectCode = sParam.ContractCodeLong;
                    doIB.SlipNo = StrSlipNo;
                    doIB.ServiceTypeCode = sParam.ServiceTypeCode;

                    doIS.ContractCode = sParam.ContractCodeLong;
                    doIS.SlipNo = StrSlipNo;
                    doIS.SlipStatus = SlipStatus.C_SLIP_STATUS_REPLACED;
                    doIS.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doIS.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;


                    if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL && sParam.dtRentalContractBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    {
                        doIB.SalesmanEmpNo1 = sParam.dtRentalContractBasic.SalesmanCode1;
                        doIB.SalesmanEmpNo2 = sParam.dtRentalContractBasic.SalesmanCode2;

                    }
                    else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        doIB.SalesmanEmpNo1 = sParam.dtSale.SalesmanCode1;
                        doIB.SalesmanEmpNo2 = sParam.dtSale.SalesmanCode2;
                    }
                    else
                    {
                        doIB.SalesmanEmpNo1 = null;
                        doIB.SalesmanEmpNo2 = null;
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////////
                    CommonUtil cmm = new CommonUtil();

                    ////================== GET FOR CHECK EXIST InstallationBasic InstallationSlip ============

                    //tbt_InstallationBasic doTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                    tbt_InstallationBasic doTbt_InstallationBasic = null;
                    List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                    if (tmpdoTbt_InstallationBasic != null)
                    {
                        doTbt_InstallationBasic = new tbt_InstallationBasic();
                        doTbt_InstallationBasic = tmpdoTbt_InstallationBasic[0];
                    }

                    tbt_InstallationSlip doTbt_InstallationSlip = null;
                    if (sParam.do_TbtInstallationBasic != null)
                    {
                        doTbt_InstallationSlip = handler.GetTbt_InstallationSlipData(sParam.do_TbtInstallationBasic.SlipNo);
                    }
                    ////======================================================================================

                    if (doIB.InstallationBy == InstallationBy.C_INSTALLATION_BY_OTHER || doIB.InstallationBy == InstallationBy.C_INSTALLATION_BY_SECOM)
                    {
                        doIB.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_UNDER_INSTALLATION;
                    }
                    else if (doTbt_InstallationBasic == null || doTbt_InstallationBasic.InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                    {
                        doIB.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REQUESTED;
                    }
                    else
                    {
                        doIB.InstallationStatus = doTbt_InstallationBasic.InstallationStatus;
                    }

                    string strPreviousSlipStatus = "";
                    string strCurrentSlipStatus = "";
                    string strNewSlipStatus = "";
                    bool bCompleteStockOut = false;

                    DateTime? dtStockOutDate = null; //Add by Jutarat A. on 11092013
                    bool bUpdateSaleBasic = false; //Add by Jutarat A. on 11092013

                    if (doTbt_InstallationBasic == null)
                    {
                        handler.InsertTbt_InstallationBasic(doIB);
                    }
                    else
                    {
                        List<tbt_InstallationBasic> tmpdo_TbtInstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                        if (tmpdo_TbtInstallationBasic != null && tmpdo_TbtInstallationBasic.Count > 0)
                        {
                            tmpdo_TbtInstallationBasic[0].OCC = doIB.OCC;
                            tmpdo_TbtInstallationBasic[0].ServiceTypeCode = doIB.ServiceTypeCode;
                            tmpdo_TbtInstallationBasic[0].InstallationStatus = doIB.InstallationStatus;
                            tmpdo_TbtInstallationBasic[0].SlipNo = doIB.SlipNo;
                            tmpdo_TbtInstallationBasic[0].InstallationBy = doIB.InstallationBy;
                            tmpdo_TbtInstallationBasic[0].InstallationType = doIB.InstallationType;
                            handler.UpdateTbt_InstallationBasic(tmpdo_TbtInstallationBasic[0]);
                        }
                    }

                    //============== update old SLIP =====================
                    if (doTbt_InstallationSlip != null)
                    {
                        strPreviousSlipStatus = doTbt_InstallationSlip.SlipStatus;
                        dtStockOutDate = doTbt_InstallationSlip.StockOutDate; //Add by Jutarat A. on 11092013
                        sParam.do_TbtInstallationSlip.SlipStatus = SlipStatus.C_SLIP_STATUS_REPLACED;

                        handler.UpdateTbt_InstallationSlip(sParam.do_TbtInstallationSlip);
                    }
                    else
                    {
                        strPreviousSlipStatus = null;
                    }
                    //====================================================
                    //=============== insert new slip ====================
                    strCurrentSlipStatus = SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT;
                    if (sParam.do_TbtInstallationSlipDetails != null)
                    {
                        foreach (tbt_InstallationSlipDetails slipDetail in sParam.do_TbtInstallationSlipDetails)
                        {

                            //if ((((slipDetail.AddInstalledQty == null) ? 0 : slipDetail.AddInstalledQty) + ((slipDetail.ReturnQty == null) ? null : slipDetail.ReturnQty)) > (((slipDetail.TotalStockOutQty == null) ? 0 : slipDetail.TotalStockOutQty) + ((slipDetail.CurrentStockOutQty == null) ? 0 : slipDetail.CurrentStockOutQty)))
                            //if ((((slipDetail.AddInstalledQty == null) ? 0 : slipDetail.AddInstalledQty) + ((slipDetail.ReturnQty == null) ? null : slipDetail.ReturnQty)) > ((slipDetail.CurrentStockOutQty == null) ? 0 : slipDetail.CurrentStockOutQty))

                            //if (((slipDetail.AddInstalledQty == null) ? 0 : slipDetail.AddInstalledQty) > ((slipDetail.CurrentStockOutQty == null) ? 0 : slipDetail.CurrentStockOutQty))
                            if (((slipDetail.AddInstalledQty == null) ? 0 : slipDetail.AddInstalledQty) > 0 && (doIB.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE || doIB.InstallationType != SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)) //Modify by Jutarat A. on 08072013
                            {
                                strCurrentSlipStatus = SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT;
                                break;
                            }
                            //====================== TRS 13/06/2012 ======================
                            //if (((slipDetail.MAExchangeQty == null) ? 0 : slipDetail.MAExchangeQty) > (((slipDetail.TotalStockOutQty == null) ? 0 : slipDetail.TotalStockOutQty) + ((slipDetail.CurrentStockOutQty == null) ? 0 : slipDetail.CurrentStockOutQty)))
                            if (((slipDetail.MAExchangeQty == null) ? 0 : slipDetail.MAExchangeQty) > (((slipDetail.TotalStockOutQty == null) ? 0 : slipDetail.TotalStockOutQty))) //Modify by Jutarat A. on 08072013
                            {
                                strCurrentSlipStatus = SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT;
                                break;
                            }
                        }
                    }
                    //===== set new slip status
                    if (strPreviousSlipStatus == SlipStatus.C_SLIP_STATUS_STOCK_OUT
                        && strCurrentSlipStatus == SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT)
                    {
                        strNewSlipStatus = SlipStatus.C_SLIP_STATUS_STOCK_OUT;
                    }
                    else if (strPreviousSlipStatus == SlipStatus.C_SLIP_STATUS_STOCK_OUT
                        && strCurrentSlipStatus == SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT)
                    {
                        strNewSlipStatus = SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT;
                    }
                    else if (strPreviousSlipStatus == SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT
                        && strCurrentSlipStatus == SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT)
                    {
                        strNewSlipStatus = SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT;
                    }
                    else if (strPreviousSlipStatus == SlipStatus.C_SLIP_STATUS_STOCK_OUT
                        && strCurrentSlipStatus == SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT)
                    {
                        strNewSlipStatus = SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT;
                    }
                    else if (strPreviousSlipStatus == SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT
                        && strCurrentSlipStatus == SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT)
                    {
                        strNewSlipStatus = SlipStatus.C_SLIP_STATUS_STOCK_OUT;
                        bCompleteStockOut = true;
                        bUpdateSaleBasic = true; //Add by Jutarat A. on 11092013
                    }
                    else if (strPreviousSlipStatus == SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT
                        && strCurrentSlipStatus == SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT)
                    {
                        strNewSlipStatus = SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT;
                        bCompleteStockOut = true;
                    }
                    else
                    {
                        strNewSlipStatus = strCurrentSlipStatus;
                    }

                    //====== initial data for save slip 
                    doIS.SlipNo = StrSlipNo;
                    doIS.ServiceTypeCode = sParam.ServiceTypeCode;
                    doIS.SlipStatus = strNewSlipStatus;

                    //doIS.ChangeReasonCode 
                    //doIS.InstallationType
                    //doIS.PlanCode
                    //doIS.CauseReason 
                    //doIS.NormalContractFee
                    //doIS.NormalInstallFee
                    //doIS.InstallFeeBillingType
                    //doIS.BillingInstallFee

                    //Add by Jutarat A. on 11042013
                    //Edit by Pachara S. on 15102016
                    if (doIS.NormalContractFee != null || doIS.NormalContractFeeUsd != null)
                    {
                        doIS.NormalContractFee = doIS.NormalContractFee;
                        doIS.NormalContractFeeUsd = doIS.NormalContractFeeUsd;
                        doIS.NormalContractFeeCurrencyType = doIS.NormalContractFeeCurrencyType;
                        //doIS.NormalContractFee = doIS.NormalContractFee;
                    }

                    if (doIS.BillingInstallFee != null || doIS.BillingInstallFeeUsd != null)
                    {
                        doIS.BillingInstallFeeCurrencyType = doIS.BillingInstallFeeCurrencyType;
                        doIS.BillingInstallFee = doIS.BillingInstallFee;
                        doIS.BillingInstallFeeUsd = doIS.BillingInstallFeeUsd;
                        //doIS.OrderInstallFee = doIS.BillingInstallFee;
                    }
                    else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    {
                        IRentralContractHandler rentalHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                        //Get last unimplement OCC from contract module
                        string strUnimplementOCC = rentalHandler.GetLastUnimplementedOCC(sParam.dtRentalContractBasic.ContractCode);

                        //Get rental security basic 
                        List<tbt_RentalSecurityBasic> doTbt_RentalSecurityBasic = rentalHandler.GetTbt_RentalSecurityBasic(sParam.dtRentalContractBasic.ContractCode, strUnimplementOCC);
                        if (doTbt_RentalSecurityBasic != null && doTbt_RentalSecurityBasic.Count > 0)
                            doIS.OrderInstallFee = doTbt_RentalSecurityBasic[0].OrderInstallFee;
                    }
                    else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        if (sParam.dtSale != null)
                            doIS.OrderInstallFee = sParam.dtSale.OrderInstallFee;
                    }
                    else
                    {
                        doIS.OrderInstallFee = null;
                    }
                    //End Add

                    //doIS.BillingOCC
                    if (doTbt_InstallationSlip != null)
                    {
                        doIS.PreviousSlipNo = doTbt_InstallationSlip.SlipNo;
                    }

                    doIS.PreviousSlipStatus = strPreviousSlipStatus;
                    doIS.ContractCode = sParam.ContractCodeLong;
                    //doIS.SlipIssueDate
                    //doIS.SlipIssueOfficeCode
                    //doIS.ApproveNo1
                    //doIS.ApproveNo2
                    //doIS.ChangeContents
                    //doIS.ExpectedInstrumentArrivalDate
                    //doIS.StockOutTypeCode

                    //-- Add by Phoomsak L. 2012-10-11 for setting Slip issue flag for prevent dead lock in gen slip doc
                    doIS.SlipIssueFlag = FlagType.C_FLAG_OFF;
                    if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        doIS.SlipIssueFlag = FlagType.C_FLAG_ON;
                    }
                    else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    {
                        if ((doIS.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_SECOM)
                             || (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW
                                )
                             || (
                                    (
                                        doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                                        || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
                                        || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                                    )
                                    && sParam.dtRentalContractBasic.InstallationCompleteFlag == false
                                )
                          )
                        {
                            doIS.SlipIssueFlag = FlagType.C_FLAG_ON; ;
                        }
                    }

                    handler.InsertTbt_InstallationSlip(doIS);
                    sParam.do_TbtInstallationSlip = doIS;
                    //====================================================

                    //handler.InsertTbt_InstallationManagement(doIM);
                    /////////////////// INSERT MEMO ////////////////////////
                    if (!CommonUtil.IsNullOrEmpty(Memo))
                    {
                        tbt_InstallationMemo doMemo = new tbt_InstallationMemo();
                        doMemo.ContractProjectCode = sParam.ContractCodeLong;
                        doMemo.ReferenceID = StrSlipNo;
                        doMemo.ObjectID = ScreenID.C_SCREEN_ID_INSTALL_SLIP;
                        doMemo.Memo = Memo;
                        doMemo.OfficeCode = CommonUtil.dsTransData.dtUserData.MainOfficeCode;
                        doMemo.DepartmentCode = CommonUtil.dsTransData.dtUserData.MainDepartmentCode;
                        doMemo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doMemo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        handler.InsertTbt_InstallationMemo(doMemo);
                    }
                    /////////////////////////////////////////////////////////
                    /////////////////// INSERT EMAIL ////////////////////////
                    if (sParam.ListDOEmail != null)
                    {
                        foreach (ISS030_DOEmailData dataEmail in sParam.ListDOEmail)
                        {
                            tbt_InstallationEmail doEmail = new tbt_InstallationEmail();
                            doEmail.ReferenceID = StrSlipNo;
                            doEmail.EmailNoticeTarget = dataEmail.EmailAddress;
                            doEmail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            doEmail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            handler.InsertTbt_InstallationEmail(doEmail);
                        }
                    }
                    /////////////////////////////////////////////////////////

                    //sParam.do_TbtInstallationSlipDetails

                    // ================= Update Attach ======================
                    string StrMANo = StrSlipNo;         // Use "Installation Slip No." instead of "Maintenance No."
                    List<tbt_AttachFile> tmpFileList = chandler.GetTbt_AttachFile(GetCurrentKey(), null, false);
                    if (tmpFileList.Count > 0)
                    {
                        chandler.UpdateFlagAttachFile(AttachmentModule.Installation, GetCurrentKey(), StrMANo);
                        List<tbt_AttachFile> fileList = chandler.GetTbt_AttachFile(StrMANo, null, true);
                        foreach (tbt_AttachFile file in fileList)
                        {
                            tbt_InstallationAttachFile doTbt_InstallAttachFile = new tbt_InstallationAttachFile();
                            doTbt_InstallAttachFile.MaintenanceNo = StrMANo;
                            doTbt_InstallAttachFile.AttachFileID = file.AttachFileID;
                            doTbt_InstallAttachFile.ObjectID = ScreenID.C_SCREEN_ID_INSTALL_REQUEST;
                            handler.InsertTbt_InstallationAttachFile(doTbt_InstallAttachFile);
                        }
                    }
                    //======================================================

                    List<tbt_InstallationSlipDetails> OldSlipDetails;
                    if (sParam.do_TbtInstallationBasic != null && sParam.do_TbtInstallationBasic.SlipNo != null)
                        OldSlipDetails = handler.GetTbt_InstallationSlipDetailsData(sParam.do_TbtInstallationBasic.SlipNo, null, InstrumentType.C_INST_TYPE_GENERAL);
                    else
                        OldSlipDetails = null;

                    if (sParam.do_TbtInstallationSlipDetails != null)
                    {
                        foreach (tbt_InstallationSlipDetails slipDetail in sParam.do_TbtInstallationSlipDetails)
                        {
                            int IndexInstrument = -1;
                            if (OldSlipDetails != null)
                            {
                                IndexInstrument = OldSlipDetails.FindIndex(delegate (tbt_InstallationSlipDetails s) { return s.InstrumentCode == slipDetail.InstrumentCode; });
                            }


                            if (IndexInstrument >= 0)
                            {
                                tbt_InstallationSlipDetails DataSlipDetailsForInsert = new tbt_InstallationSlipDetails();
                                DataSlipDetailsForInsert = CommonUtil.CloneObject<tbt_InstallationSlipDetails, tbt_InstallationSlipDetails>(OldSlipDetails[IndexInstrument]);
                                DataSlipDetailsForInsert.SlipNo = StrSlipNo;

                                //======================= TRS 13/06/2012 =====================
                                //DataSlipDetailsForInsert.AddInstalledQty = slipDetail.AddInstalledQty;
                                //DataSlipDetailsForInsert.AddRemovedQty = slipDetail.AddRemovedQty;
                                if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                                 || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                                 || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                                {
                                    DataSlipDetailsForInsert.AddInstalledQty = ((slipDetail.MAExchangeQty == null) ? 0 : slipDetail.MAExchangeQty) - ((slipDetail.TotalStockOutQty == null) ? 0 : slipDetail.TotalStockOutQty);
                                }
                                else
                                {
                                    DataSlipDetailsForInsert.AddInstalledQty = slipDetail.AddInstalledQty;
                                }


                                if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                                 || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                                 || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                                {
                                    DataSlipDetailsForInsert.AddRemovedQty = ((slipDetail.MAExchangeQty == null) ? 0 : slipDetail.MAExchangeQty);
                                }
                                else
                                {
                                    DataSlipDetailsForInsert.AddRemovedQty = slipDetail.AddRemovedQty;
                                }
                                //============================================================
                                DataSlipDetailsForInsert.ContractInstalledQty = slipDetail.ContractInstalledQty;
                                DataSlipDetailsForInsert.ReturnQty = slipDetail.ReturnQty;
                                DataSlipDetailsForInsert.MoveQty = slipDetail.MoveQty;
                                DataSlipDetailsForInsert.MAExchangeQty = slipDetail.MAExchangeQty;
                                DataSlipDetailsForInsert.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                                // DataSlipDetailsForInsert.InstrumentTypeCode = slipDetail.InstrumentTypeCode;
                                DataSlipDetailsForInsert.InstrumentPrice = slipDetail.InstrumentPrice;
                                DataSlipDetailsForInsert.CurrentStockOutQty = 0;

                                //Modify by Jutarat A. on 13062013 (Keep data from grid)
                                //DataSlipDetailsForInsert.TotalStockOutQty = ((slipDetail.TotalStockOutQty == null) ? 0 : slipDetail.TotalStockOutQty) + ((slipDetail.CurrentStockOutQty == null) ? 0 : slipDetail.CurrentStockOutQty);
                                DataSlipDetailsForInsert.TotalStockOutQty = slipDetail.TotalStockOutQty;

                                handler.InsertTbt_InstallationSlipDetails(DataSlipDetailsForInsert);
                            }
                            else
                            {
                                slipDetail.SlipNo = StrSlipNo;
                                slipDetail.ContractInstalledQty = slipDetail.ContractInstalledQty;
                                slipDetail.CurrentStockOutQty = 0;
                                slipDetail.TotalStockOutQty = 0;
                                slipDetail.NotInstalledQty = 0;
                                slipDetail.UnremovableQty = 0;
                                slipDetail.ReturnRemoveQty = 0;
                                slipDetail.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                                //======================= TRS 13/06/2012 =====================                              
                                if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                                 || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                                 || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                                {
                                    slipDetail.AddInstalledQty = slipDetail.MAExchangeQty;
                                }
                                if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                                 || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                                 || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                                {
                                    slipDetail.AddRemovedQty = slipDetail.MAExchangeQty;
                                }
                                //============================================================

                                handler.InsertTbt_InstallationSlipDetails(slipDetail);
                            }
                        }
                    }
                    //========================= Insert Facility Detail ======================
                    if (FacilityList != null && FacilityList.Count > 0)
                    {
                        foreach (tbt_InstallationSlipDetails facilityDetail in FacilityList)
                        {
                            facilityDetail.SlipNo = StrSlipNo;
                            facilityDetail.InstrumentTypeCode = InstrumentType.C_INST_TYPE_MONITOR;
                            facilityDetail.ContractInstalledQty = 0;
                            facilityDetail.CurrentStockOutQty = 0;
                            facilityDetail.TotalStockOutQty = 0;
                            facilityDetail.ReturnQty = 0;
                            facilityDetail.AddRemovedQty = 0;
                            facilityDetail.MoveQty = 0;
                            facilityDetail.MAExchangeQty = 0;
                            facilityDetail.UnremovableQty = 0;
                            facilityDetail.ReturnRemoveQty = 0;
                            facilityDetail.NotInstalledQty = 0;
                            handler.InsertTbt_InstallationSlipDetails(facilityDetail);
                        }
                    }
                    //=======================================================================
                    //=========================== SEND EMAIL ================================
                    #region Send Email

                    //if (sParam.ListDOEmail != null)
                    //{

                    //    doEmailTemplate template = ISS030_GenerateMailRegisterInstallationPO(sParam);
                    //    if (template != null)
                    //    {
                    //        foreach (ISS030_DOEmailData e in sParam.ListDOEmail)
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
                    obj.EmailList = new List<doEmailProcess>();
                    doEmailTemplate template = ISS030_GenerateMailRegisterInstallationPO(sParam);
                    if (template != null)
                    {
                        if (sParam.ListDOEmail != null)
                        {
                            foreach (ISS030_DOEmailData e in sParam.ListDOEmail)
                            {
                                doEmailProcess mail = new doEmailProcess()
                                {
                                    MailTo = e.EmailAddress,
                                    Subject = template.TemplateSubject,
                                    Message = template.TemplateContent
                                };
                                obj.EmailList.Add(mail);
                            }
                        }

                        if (doIS.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW
                            || doIS.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
                            || doIS.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW
                            || doIS.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD
                            || doIS.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                            )
                        {
                            ICommonHandler commonh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                            var cfgPurchaseEmail = commonh.GetSystemConfig(ConfigName.C_CONFIG_PURCHASE_EMAIL).FirstOrDefault();
                            if (cfgPurchaseEmail != null && !string.IsNullOrEmpty(cfgPurchaseEmail.ConfigValue))
                            {
                                string purEmails = cfgPurchaseEmail.ConfigValue;
                                foreach (string email in purEmails.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (!string.IsNullOrEmpty(email))
                                    {
                                        if (sParam.ListDOEmail == null || !sParam.ListDOEmail.Any(d => d.EmailAddress == email))
                                        {
                                            doEmailProcess mail = new doEmailProcess()
                                            {
                                                MailTo = email,
                                                Subject = template.TemplateSubject,
                                                Message = template.TemplateContent
                                            };
                                            obj.EmailList.Add(mail);
                                        }
                                    }
                                }
                            }
                        }

                        if (!CommonUtil.IsNullOrEmpty(obj.EmailList) && obj.EmailList.Count > 0)
                        {
                            System.Threading.Thread t = new System.Threading.Thread(SendMail);
                            t.Start(obj);
                        }
                    }

                    #endregion
                    //=======================================================================

                    //========================== Generate report ==================================
                    if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        handler.GenerateInstallationSlipDoc(sParam.ContractCodeLong, false); //Add isCheckSlipIssueFlag by Jutarat A. on 25072013
                    }
                    else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    {
                        if ((doIS.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_SECOM)
                             || (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                                  || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW
                                )
                             || (
                                    (
                                        doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                                        || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW
                                        || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                                    )
                                    && sParam.dtRentalContractBasic.InstallationCompleteFlag == false
                                )
                          )
                        {
                            handler.GenerateInstallationSlipDoc(sParam.ContractCodeLong, false); //Add isCheckSlipIssueFlag by Jutarat A. on 25072013
                        }
                    }
                    //=============================================================================

                    //=========================== Generate Quotation Data =========================
                    #region GENERATE QUOTATION
                    if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL
                        && doIS.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW
                        && doIS.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                        && doIS.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE //Add by Jutarat A. on 19032013
                        && doIS.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE) //Add by Jutarat A. on 19032013
                    {
                        IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                        dsGenerateData doUpdate = new dsGenerateData();
                        //header
                        doUpdate.dtHeader = new dtHeader();
                        doUpdate.dtHeader.ContractCode = sParam.ContractCodeLong;

                        //doUpdate.dtHeader.InstallationFee = doIB.NormalInstallFee;
                        if (doIB.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                        {
                            doUpdate.dtHeader.InstallationFee = doIB.NormalInstallFee;
                            doUpdate.dtHeader.InstallationFeeUsd = null;
                            doUpdate.dtHeader.InstallationFeeCurrencyType = doIB.NormalInstallFeeCurrencyType;
                        }
                        if (doIB.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            doUpdate.dtHeader.InstallationFee = null;
                            doUpdate.dtHeader.InstallationFeeUsd = doIB.NormalInstallFeeUsd;
                            doUpdate.dtHeader.InstallationFeeCurrencyType = doIB.NormalInstallFeeCurrencyType;
                        }

                        doUpdate.dtHeader.InstallationSlipNo = StrSlipNo;
                        doUpdate.dtHeader.InstallationEngineerEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doUpdate.dtHeader.ApproveNo1 = doIB.ApproveNo1;
                        doUpdate.dtHeader.ApproveNo2 = doIB.ApproveNo2;
                        //Phoomsak L. 2012-07-18 Quotation no calculate normal contract fee
                        if (doIB.NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                        {
                            doUpdate.dtHeader.NormalContractFee = doIB.NormalContractFee;
                            doUpdate.dtHeader.NormalContractFeeUsd = null;
                            doUpdate.dtHeader.NormalContractFeeCurrencyType = doIB.NormalContractFeeCurrencyType;
                        }
                        if (doIB.NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            doUpdate.dtHeader.NormalContractFee = null;
                            doUpdate.dtHeader.NormalContractFeeUsd = doIB.NormalContractFeeUsd;
                            doUpdate.dtHeader.NormalContractFeeCurrencyType = doIB.NormalContractFeeCurrencyType;
                        }

                        //detail
                        doUpdate.dtInstrumentDetails = new List<dtInstrumentDetails>();

                        dtInstrumentDetails dt1;
                        if (sParam.do_TbtInstallationSlipDetails != null)
                        {
                            foreach (tbt_InstallationSlipDetails slipDetail in sParam.do_TbtInstallationSlipDetails)
                            {
                                //detail[]
                                dt1 = new dtInstrumentDetails();
                                dt1.InstrumentCode = slipDetail.InstrumentCode;
                                if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW)
                                {
                                    dt1.InstallQty = slipDetail.TotalStockOutQty + slipDetail.AddInstalledQty - slipDetail.ReturnQty;
                                    dt1.AcmAddQty = 0;
                                    dt1.AcmRemoveQty = 0;
                                }
                                else
                                {
                                    dt1.InstallQty = slipDetail.ContractInstalledQty;
                                    dt1.AcmAddQty = slipDetail.TotalStockOutQty + slipDetail.AddInstalledQty - slipDetail.ReturnQty;
                                    dt1.AcmRemoveQty = slipDetail.AddRemovedQty;
                                }

                                doUpdate.dtInstrumentDetails.Add(dt1);
                            }
                        }

                        doUpdate.dtFacilityDetails = new List<dtFacilityDetails>();
                        dtFacilityDetails dt2;
                        if (FacilityList != null)
                        {
                            foreach (tbt_InstallationSlipDetails facilityDetail in FacilityList)
                            {
                                //detail[]
                                dt2 = new dtFacilityDetails();
                                dt2.FacilityCode = facilityDetail.InstrumentCode;
                                dt2.FacilityQty = facilityDetail.AddInstalledQty;
                                doUpdate.dtFacilityDetails.Add(dt2);
                            }
                        }

                        string strNewAlphabet = target.GenerateQuotation(doUpdate);
                        //Phoomsak L. 2012-07-10 no need to lock quotation this time, lock when installation complete
                        //bool blnLockQuotationResult = target.LockQuotation(sParam.ContractCodeLong, strNewAlphabet, LockStyle.C_LOCK_STYLE_BACKWARD);
                    }
                    #endregion
                    //=============================================================================

                    //========================= Update Inventory Data =============================
                    if (bCompleteStockOut)
                    {
                        doUpdateCompleteStockoutForPartial doCompleteForPartial = new doUpdateCompleteStockoutForPartial();
                        doCompleteForPartial.ContractCode = sParam.ContractCodeLong;
                        doCompleteForPartial.InstallationSlipNo = StrSlipNo;
                        doCompleteForPartial.SlipType = strSlipID2Digit;
                        iHandler.UpdateCompleteStockoutForPartial(doCompleteForPartial);
                    }
                    //=============================================================================

                    //Add by Jutarat A. on 11092013
                    if (bUpdateSaleBasic && sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        var srvSale = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        string strLatestSaleOCC = srvSale.GetLastOCC(sParam.ContractCodeLong);
                        if (!string.IsNullOrEmpty(strLatestSaleOCC))
                        {
                            var lstSaleBasic = srvSale.GetTbt_SaleBasic(sParam.ContractCodeLong, strLatestSaleOCC, null);

                            if (lstSaleBasic != null && lstSaleBasic.Count > 0)
                            {
                                foreach (var sb in lstSaleBasic)
                                {
                                    if (sb.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_GENCODE)
                                        sb.SaleProcessManageStatus = SaleProcessManageStatus.C_SALE_PROCESS_STATUS_SHIP;

                                    sb.InstrumentStockOutDate = dtStockOutDate;
                                    srvSale.UpdateTbt_SaleBasic(sb);
                                }
                            }
                        }
                    }
                    //End Add

                    #region Update booking.
                    //bool isInstalled = false;
                    //string strBookingAreaCode = null;
                    //if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    //{
                    //    isInstalled = (sParam.dtRentalContractBasic.FirstInstallCompleteFlag ?? false);
                    //    strBookingAreaCode = InstrumentArea.C_INV_AREA_NEW_RENTAL;
                    //}
                    //else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    //{
                    //    isInstalled = false;
                    //    strBookingAreaCode = InstrumentArea.C_INV_AREA_NEW_SALE;
                    //}
                    //
                    //if (!isInstalled && sParam.do_TbtInstallationSlipDetails != null)
                    //{
                    //    var lstBooking = handler.GetInstallationBooking(StrSlipNo);
                    //    var currentBooking = iHandler.GetTbt_InventoryBookingDetail(sParam.ContractCodeLong, null);
                    //    var updateBooking = new List<tbt_InventoryBookingDetail>();
                    //    var newBooking = new List<tbt_InventoryBookingDetail>();
                    //    foreach (var addInstall in lstBooking)
                    //    {
                    //        var booking = currentBooking.FirstOrDefault(d => d.InstrumentCode == addInstall.InstrumentCode);
                    //        if (booking == null)
                    //        {
                    //            newBooking.Add(new tbt_InventoryBookingDetail()
                    //            {
                    //                ContractCode = sParam.ContractCodeLong,
                    //                InstrumentCode = addInstall.InstrumentCode,
                    //                AreaCode = strBookingAreaCode,
                    //                BookingQty = addInstall.InstrumentQty,
                    //                StockOutQty = 0,
                    //                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    //                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                    //                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    //                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                    //            });
                    //        }
                    //        else
                    //        {
                    //            booking.BookingQty = addInstall.InstrumentQty;
                    //            booking.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //            booking.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //            updateBooking.Add(booking);
                    //        }
                    //    }
                    //    if (updateBooking.Count > 0) iHandler.UpdateTbt_InventoryBookingDetail(updateBooking);
                    //    if (newBooking.Count > 0) iHandler.InsertTbt_InventoryBookingDetail(newBooking);
                    //}
                    #endregion

                    scope.Complete();
                }

                ISS030_RegisterStartResumeTargetData result = new ISS030_RegisterStartResumeTargetData();
                result.SlipNo = StrSlipNo;
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
        /// <param name="strContractCodeLong"></param>
        /// <returns></returns>
        public ObjectResultData ISS030_ValidateContractError(dtRentalContractBasicForInstall contractData, dtSaleBasic saleData, string strServiceTypeCode, string strContractCodeLong)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;

            try
            {
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                IRentralContractHandler ReantalContracthandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler SaleContracthandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    //if (ISS030_ValidExistOffice(contractData.OperationOfficeCode) == false)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    //    return res;
                    //}
                    if (contractData.ProductTypeCode != ProductType.C_PROD_TYPE_AL
                        && contractData.ProductTypeCode != ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5013);
                        return res;
                    }

                    if (contractData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    {
                        if (CommonUtil.dsTransData.dtUserBelongingData.Any(d => d.DepartmentCode == "5000") == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5126);
                            return res;
                        }
                    }

                }
                else if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    //if (ISS030_ValidExistOffice(saleData.OperationOfficeCode) == false)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    //    return res;
                    //}
                    if (saleData.ProductTypeCode != ProductType.C_PROD_TYPE_SALE)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5013);
                        return res;
                    }

                    List<tbt_InstallationBasic> tmpdo_TbtInstallationBasic = handler.GetTbt_InstallationBasicData(strContractCodeLong);
                    if (tmpdo_TbtInstallationBasic != null && tmpdo_TbtInstallationBasic.Count > 0 && tmpdo_TbtInstallationBasic[0].SlipNo != null)
                    {
                        if (tmpdo_TbtInstallationBasic[0].InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD
                            || tmpdo_TbtInstallationBasic[0].InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW) //Add by Jutarat A. on 17062013
                        {
                            if (!SaleContracthandler.CheckCanReplaceInstallSlip(saleData.ContractCode))
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5070);
                                return res;
                            }
                        }
                    }

                }

                bool blnAllRemoval = handler.CheckAllRemoval(strContractCodeLong);
                if (blnAllRemoval)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5001);
                    return res;
                }

                //res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Validate before register insytallation slip
        /// </summary>
        /// <param name="doIB"></param>
        /// <param name="doIS"></param>
        /// <param name="doValidate"></param>
        /// <param name="BlnHaveNewRow"></param>
        /// <param name="disabledInstallFeeBillingType"></param>
        /// <returns></returns>
        public ActionResult ISS030_ValidateBeforeRegister(tbt_InstallationBasic doIB, tbt_InstallationSlip doIS, ISS030_ValidateData doValidate, string BlnHaveNewRow, bool disabledInstallFeeBillingType)
        {
            ObjectResultData res = new ObjectResultData();

            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_SLIP, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                if (string.IsNullOrEmpty(doIB.PlanCode))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "PlanCode",
                                                    "lblPlanCode",
                                                    "PlanCode");
                }
                if (disabledInstallFeeBillingType == false && CommonUtil.IsNullOrEmpty(doIS.InstallFeeBillingType))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "InstallFeeBillingType",
                                                    "lblInstallFeeBilling",
                                                    "InstallFeeBillingType");
                }
                if (CommonUtil.IsNullOrEmpty(doValidate.InstallationType))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "InstallationType",
                                                    "lblInstallationType",
                                                    "InstallationType");
                }
                if (CommonUtil.IsNullOrEmpty(doValidate.CauseReason))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "CauseReason",
                                                    "lblInstallationCauseReason",
                                                    "CauseReason");
                }
                if (CommonUtil.IsNullOrEmpty(doValidate.InstallationBy))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "InstallationBy",
                                                    "lblInstallationBy",
                                                    "InstallationBy");
                }
                if (CommonUtil.IsNullOrEmpty(doValidate.ChangeReasonCode))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "ChangeReasonCode",
                                                    "lblChangeReason",
                                                    "ChangeReasonCode");
                }
                if (CommonUtil.IsNullOrEmpty(doValidate.SlipIssueDate))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "SlipIssueDate",
                                                    "lblInstallSlipIssueDate",
                                                    "SlipIssueDate");
                }
                if (CommonUtil.IsNullOrEmpty(doValidate.SlipIssueOfficeCode))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "SlipIssueOfficeCode",
                                                    "lblInstallSlipOutputTarget",
                                                    "SlipIssueOfficeCode");
                }
                if (CommonUtil.IsNullOrEmpty(doValidate.NormalInstallFee))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "NormalInstallFee",
                                                    "lblNormalInstallationFee",
                                                    "NormalInstallFee");
                }
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    res.ResultData = false;
                    return Json(res);
                }
                //if (ModelState.IsValid == false)
                //{
                //    ValidatorUtil.BuildErrorMessage(res, this,null);
                //    if (res.IsError)
                //    {
                //        res.ResultData = false;
                //        return Json(res);
                //    }
                //}
                ISS030_ScreenParameter sParam = GetScreenObject<ISS030_ScreenParameter>();

                //ValidatorUtil.BuildErrorMessage(res, this);


                //========================= VALIDATE BUSINESS ===============================
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL || sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    if (doIB.InstallationType == null)
                    {
                        //validator.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007,
                        //    "InstallationType", "Installation type", 
                        //    "InstallationType");

                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "InstallationType",
                                                    "lblInstallationType",
                                                    "InstallationType");
                    }

                }


                ValidatorUtil.BuildErrorMessage(res, validator, null);

                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                //////////////////// VALIDATE BUSINESS //////////////////////
                validator = ISS030_ValidateBusiness(doIS, doIB, BlnHaveNewRow);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                /////////////////////////////////////////////////////////////                

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
        public bool ISS030_ValidExistOffice(string OfficeCode)
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
        public ObjectResultData ISS030_ValidateProjectError(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();

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

                IInstallationHandler installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                bool blnInstallationRegistered = installHandler.CheckInstallationRegister(strProjectCode);
                if (!blnInstallationRegistered)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5003);
                }
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
        /// <param name="doIS"></param>
        /// <param name="doIB"></param>
        /// <param name="BlnHaveNewRow"></param>
        /// <returns></returns>
        public ValidatorUtil ISS030_ValidateBusiness(tbt_InstallationSlip doIS, tbt_InstallationBasic doIB, string BlnHaveNewRow)
        {
            ObjectResultData res = new ObjectResultData();
            ValidatorUtil validator = new ValidatorUtil();
            try
            {

                ISS030_ScreenParameter sParam = GetScreenObject<ISS030_ScreenParameter>();
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                IRentralContractHandler rentalHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                //================= Teerapong S. 15/10/2012 =======================
                List<tbt_InstallationBasic> doValidate_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);

                if ((sParam.do_TbtInstallationBasic == null && doValidate_InstallationBasic != null)
                    || (sParam.do_TbtInstallationBasic != null && doValidate_InstallationBasic == null)
                    || (sParam.do_TbtInstallationBasic != null && doValidate_InstallationBasic != null && DateTime.Compare(sParam.do_TbtInstallationBasic.UpdateDate.Value, doValidate_InstallationBasic[0].UpdateDate.Value) != 0)
                    )
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                                      MessageUtil.MessageList.MSG0019,
                                                      "", "", "");
                }
                //=================================================================

                //========= 1
                if (sParam.dtRentalContractBasic != null)
                {
                    if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL && sParam.dtRentalContractBasic.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL && doIS.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL)
                    {
                        //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5022, "InstallationType", "Installation type","InstallationType");
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5022,
                                                    "InstallationType",
                                                    "lblInstallationType",
                                                    "InstallationType");
                    }
                }
                //========== 2
                if (doIS.BillingInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    if (doIS.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_SECOM
                    && (doIS.BillingInstallFee != null && doIS.BillingInstallFee > 0))
                    {
                        //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                        //                          MessageUtil.MessageList.MSG5023,
                        //                          "BillingInstallFee", "Billing installation fee",
                        //                          "BillingInstallFee");
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        "ISS030",
                                                        MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5023,
                                                        "BillingInstallFee",
                                                        "lblBillingInstallFee",
                                                        "BillingInstallFee");
                    }
                }else
                {
                    if (doIS.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_SECOM
                  && (doIS.BillingInstallFeeUsd != null && doIS.BillingInstallFeeUsd > 0))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        "ISS030",
                                                        MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5023,
                                                        "BillingInstallFee",
                                                        "lblBillingInstallFee",
                                                        "BillingInstallFee");
                    }   
                }
                //========== 3 
                if (doIS.BillingInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    if (doIS.InstallFeeBillingType == InstallFeeBillingType.C_INSTALL_FEE_BILLING_TYPE_BILLING
                 && ((doIS.BillingInstallFee == null || doIS.BillingInstallFee == 0))) //doIS.BillingInstallFee == 0 || doIS.BillingInstallFee == null
                    {
                        //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                        //                          MessageUtil.MessageList.MSG5024,
                        //                          "BillingInstallFee", "Billing installation fee",
                        //                          "BillingInstallFee");
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        "ISS030",
                                                        MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5024,
                                                        "BillingInstallFee",
                                                        "lblBillingInstallFee",
                                                        "BillingInstallFee");
                    }
                }
                else
                {
                    if (doIS.InstallFeeBillingType == InstallFeeBillingType.C_INSTALL_FEE_BILLING_TYPE_BILLING
                 && ((doIS.BillingInstallFeeUsd == null || doIS.BillingInstallFeeUsd == 0))) //doIS.BillingInstallFee == 0 || doIS.BillingInstallFee == null
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        "ISS030",
                                                        MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5024,
                                                        "BillingInstallFee",
                                                        "lblBillingInstallFee",
                                                        "BillingInstallFee");
                    }
                }


                //=========== 4
                if (doIS.BillingInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    if (doIS.InstallFeeBillingType == InstallFeeBillingType.C_INSTALL_FEE_BILLING_TYPE_PAY_ALL_AMOUNT
                    && (doIS.BillingInstallFee != 0 && doIS.BillingInstallFee != null))
                    {
                        //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                        //                          MessageUtil.MessageList.MSG5025,
                        //                          "BillingInstallFee", "Billing installation fee",
                        //                          "BillingInstallFee");
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        "ISS030",
                                                        MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5025,
                                                        "BillingInstallFee",
                                                        "lblBillingInstallFee",
                                                        "BillingInstallFee");
                    }
                }else
                {
                    if (doIS.InstallFeeBillingType == InstallFeeBillingType.C_INSTALL_FEE_BILLING_TYPE_PAY_ALL_AMOUNT
                    && (doIS.BillingInstallFeeUsd != 0 && doIS.BillingInstallFeeUsd != null))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        "ISS030",
                                                        MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5025,
                                                        "BillingInstallFee",
                                                        "lblBillingInstallFee",
                                                        "BillingInstallFee");
                    }
                }

                //=========== New
                //if (doIB.InstallationBy == InstallationBy.C_INSTALLATION_BY_SUBCONTRACTOR
                //    && (doIB.MaintenanceNo == "" || doIB.MaintenanceNo == null))
                //{
                //    //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                //    //                          MessageUtil.MessageList.MSG5080,
                //    //                          "BillingInstallFee", "Billing installation fee",
                //    //                          "BillingInstallFee");
                //    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                //                                    "ISS030",
                //                                    MessageUtil.MODULE_INSTALLATION,
                //                                    MessageUtil.MessageList.MSG5080,
                //                                    "InstallationBy",
                //                                    "lblInstallationBy",
                //                                    "InstallationBy");
                //}

                //=========== 5
                DateTime pToDay = new DateTime();
                pToDay = DateTime.Today;
                if (doIS.SlipIssueDate < pToDay)
                {
                    //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                          MessageUtil.MessageList.MSG5026,
                    //                          "SlipIssueDate", "Installation Slip Issue Date",
                    //                          "SlipIssueDate");

                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5026,
                                                    "SlipIssueDate",
                                                    "lblInstallSlipIssueDate",
                                                    "SlipIssueDate");
                }
                //============ 7 
                if ((doIS.ApproveNo2 != null && doIS.ApproveNo2 != "") && (doIS.ApproveNo1 == null || doIS.ApproveNo1 == ""))
                {
                    //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                          MessageUtil.MessageList.MSG5029,
                    //                          "ApproveNo1", "Approve No. 1",
                    //                          "ApproveNo1");
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5028,
                                                    "ApproveNo1",
                                                    "lblApproveNo1",
                                                    "ApproveNo1");
                }
                //=========== 6 
                if ((doIS.BillingInstallFee != null) && (doIS.NormalInstallFee != doIS.BillingInstallFee) && (doIS.ApproveNo1 == null || doIS.ApproveNo1 == ""))
                {
                    //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                          MessageUtil.MessageList.MSG5027,
                    //                          "ApproveNo1", "Approve No. 1",
                    //                          "ApproveNo1");
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5027,
                                                    "ApproveNo1",
                                                    "lblApproveNo1",
                                                    "ApproveNo1");
                }

                //============= 8
                if (doIS.ExpectedInstrumentArrivalDate < doIS.SlipIssueDate)
                {
                    //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                          MessageUtil.MessageList.MSG5030,
                    //                          "ExpectedInstrumentArrivalDate", "Expected Instrument Arrival Date",
                    //                          "ExpectedInstrumentArrivalDate");
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5030,
                                                    "ExpectedInstrumentArrivalDate",
                                                    "lblExpectedInstrumentArrivalDate",
                                                    "ExpectedInstrumentArrivalDate");
                }
                //============= 9 
                string strLastUnimplementOCC = "";
                strLastUnimplementOCC = rentalHandler.GetLastUnimplementedOCC(sParam.ContractCodeLong);
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    bool blnCheckCP12 = rentalHandler.CheckCP12(sParam.ContractCodeLong, strLastUnimplementOCC);
                    if (blnCheckCP12 && doIS.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_SECOM)
                    {
                        //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                        //                          MessageUtil.MessageList.MSG5033,
                        //                          "ChangeReasonCode", "Change Reason Code",
                        //                          "ChangeReasonCode");
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS030",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5033,
                                                    "ChangeReasonCode",
                                                    "lblChangeReason",
                                                    "ChangeReasonCode");
                    }
                }
                string tempLineUpTypeCode = "";
                int intNormalContactFee = 0;
                int intChangeNormalContactFee = 0;
                if (sParam.do_TbtInstallationSlipDetails != null)
                {
                    //=========================== Decalre counter ================================
                    int? iRemoveQty = 0;
                    int? iMoveQty = 0;
                    int? iMAQty = 0;
                    //============================================================================
                    foreach (tbt_InstallationSlipDetails doSlipDetailData in sParam.do_TbtInstallationSlipDetails)
                    {
                        //=============== Counter for validate =======================
                        iRemoveQty = iRemoveQty + doSlipDetailData.AddRemovedQty;
                        iMoveQty = iMoveQty + doSlipDetailData.MoveQty;
                        iMAQty = iMAQty + doSlipDetailData.MAExchangeQty;
                        //============================================================
                        if (doSlipDetailData.AddInstalledQty > 0 && doIS.ExpectedInstrumentArrivalDate == null)
                        {
                            //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                            //                      MessageUtil.MessageList.MSG5031,
                            //                      "ExpectedInstrumentArrivalDate", "Expected Instrument Arrival Date",
                            //                      "ExpectedInstrumentArrivalDate");
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        "ISS030",
                                                        MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5031,
                                                        "ExpectedInstrumentArrivalDate",
                                                        "",
                                                        "ExpectedInstrumentArrivalDate");
                            break;
                        }
                        //if (doSlipDetailData.MAExchangeQty > 0 && doIS.ExpectedInstrumentArrivalDate == null)
                        //{
                        //    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                        //                                "ISS030",
                        //                                MessageUtil.MODULE_INSTALLATION,
                        //                                MessageUtil.MessageList.MSG5021,
                        //                                "ExpectedInstrumentArrivalDate",
                        //                                "",
                        //                                "ExpectedInstrumentArrivalDate");
                        //}
                        if (doSlipDetailData.ReturnQty > doSlipDetailData.TotalStockOutQty)
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                  MessageUtil.MessageList.MSG5008,
                                                  "instrument", doSlipDetailData.InstrumentCode,
                                                  "InstrumentRow" + doSlipDetailData.InstrumentCode);
                            break;
                        }
                        if (doSlipDetailData.AddRemovedQty > doSlipDetailData.ContractInstalledQty)
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                  MessageUtil.MessageList.MSG5029,
                                                  "instrument", doSlipDetailData.InstrumentCode,
                                                  "InstrumentRow" + doSlipDetailData.InstrumentCode);
                            break;
                        }

                        int IndexInstrument = sParam.do_TbtInstallationSlipDetails.FindIndex(delegate (tbt_InstallationSlipDetails s) { return s.InstrumentCode == doSlipDetailData.InstrumentCode; });
                        tempLineUpTypeCode = sParam.ListInstrumentData[IndexInstrument].LineUpTypeCode;

                        if (BlnHaveNewRow == "true" && (tempLineUpTypeCode == LineUpType.C_LINE_UP_TYPE_ONE_TIME || tempLineUpTypeCode == LineUpType.C_LINE_UP_TYPE_TEMPORARY) && (doIS.ApproveNo1 == null || doIS.ApproveNo1 == ""))
                        {
                            //validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                            //                      MessageUtil.MessageList.MSG5032,
                            //                      "ApproveNo1", "Approve No. 1",
                            //                      "ApproveNo1");
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                       "ISS030",
                                                       MessageUtil.MODULE_INSTALLATION,
                                                       MessageUtil.MessageList.MSG5032,
                                                       "ApproveNo1",
                                                       "lblApproveNo1",
                                                       "ApproveNo1");
                            break;

                        }

                        if (doSlipDetailData.AddInstalledQty > 0 && tempLineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                  MessageUtil.MessageList.MSG5122,
                                                  "instrument", doSlipDetailData.InstrumentCode,
                                                  "InstrumentRow" + doSlipDetailData.InstrumentCode);
                            break;
                        }

                        //Comment by Jutarat A. on 08072013
                        //if ((doSlipDetailData.AddInstalledQty + doSlipDetailData.ReturnQty) < doSlipDetailData.CurrentStockOutQty)
                        //{
                        //    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                        //                          MessageUtil.MessageList.MSG5066,
                        //                          "instrument", doSlipDetailData.InstrumentCode,
                        //                          "InstrumentRow" + doSlipDetailData.InstrumentCode);
                        //    break;
                        //}
                        //End Comment

                        if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                           || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                           || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                        {
                            if (doSlipDetailData.MAExchangeQty > doSlipDetailData.ContractInstalledQty)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                      MessageUtil.MessageList.MSG5020,
                                                      "instrument", doSlipDetailData.InstrumentCode,
                                                      "InstrumentRow" + doSlipDetailData.InstrumentCode);
                                break;
                            }
                            //====================== TRS 13/06/2012 ======================
                            if (doSlipDetailData.MAExchangeQty < doSlipDetailData.TotalStockOutQty)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                      MessageUtil.MessageList.MSG5088,
                                                      "instrument", doSlipDetailData.InstrumentCode,
                                                      "InstrumentRow" + doSlipDetailData.InstrumentCode);
                                break;
                            }
                        }
                        else if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE
                                || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MOVE)
                        {
                            if (doSlipDetailData.MoveQty > doSlipDetailData.ContractInstalledQty)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                      MessageUtil.MessageList.MSG5099,
                                                      "instrument", doSlipDetailData.InstrumentCode,
                                                      "InstrumentRow" + doSlipDetailData.InstrumentCode);
                                break;
                            }
                        }
                    }

                    //===================== Check data from counter =======================
                    if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                           || doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                           || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                    {
                        if (iMAQty == 0)
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                      MessageUtil.MessageList.MSG5112,
                                                      "", "", "");
                        }
                        if (iMAQty > 0 && doIS.ExpectedInstrumentArrivalDate == null)
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        "ISS030",
                                                        MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5021,
                                                        "ExpectedInstrumentArrivalDate",
                                                        "",
                                                        "ExpectedInstrumentArrivalDate");
                        }
                    }
                    else if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE
                                || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MOVE)
                    {
                        if (iMoveQty == 0)
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                      MessageUtil.MessageList.MSG5111,
                                                      "", "", "");
                        }

                    }
                    else if (doIB.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                                || doIB.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_PARTIAL_REMOVE)
                    {
                        if (iRemoveQty == 0)
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                      MessageUtil.MessageList.MSG5110,
                                                      "", "", "");
                        }
                    }


                    //=====================================================================
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return validator;
        }

        /// <summary>
        /// Clear all email data in session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS030_ClearInstallEmail(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS030_ScreenParameter session = GetScreenObject<ISS030_ScreenParameter>();
            session.ListDOEmail = null;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Clear all instrument data in session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS030_ClearInstrumentInfo(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS030_ScreenParameter session = GetScreenObject<ISS030_ScreenParameter>();
            session.do_TbtInstallationSlipDetails = null;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Add instrument data
        /// </summary>
        /// <param name="InstrumentInfo"></param>
        /// <returns></returns>
        public ActionResult ISS030_AddInstrumentInfo(tbt_InstallationInstrumentDetails InstrumentInfo)
        {

            ObjectResultData res = new ObjectResultData();

            ValidatorUtil validator = new ValidatorUtil();
            if (string.IsNullOrEmpty(InstrumentInfo.InstrumentCode))
            {
                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                            "ISS030",
                                            MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0082,
                                            "InstrumentCode",
                                            "Instrument Code",
                                            "InstrumentCode");
            }

            ValidatorUtil.BuildErrorMessage(res, validator, null);

            if (res.IsError)
                return Json(res);


            return Json(res);
        }

        /// <summary>
        /// Recieve data slip detail from screen to session parameter
        /// </summary>
        /// <param name="ScreenParam"></param>
        /// <returns></returns>
        public ActionResult ISS030_SendGridSlipDetailsData(ISS030_ScreenParameter ScreenParam)
        {
            ObjectResultData res = new ObjectResultData();

            ISS030_ScreenParameter session = GetScreenObject<ISS030_ScreenParameter>();
            //====== Get for insert
            session.do_TbtInstallationSlipDetails = ScreenParam.do_TbtInstallationSlipDetails;
            //====== Get for valid data
            session.ListInstrumentData = ScreenParam.ListInstrumentData;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Get instrument detail
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult ISS030_GetInstrumentDetailInfo(ISS030_GetInstrumentDataCondition cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                #region Get Session

                string ProductTypeCode = null;

                ISS030_ScreenParameter sParam = GetScreenObject<ISS030_ScreenParameter>();
                if (sParam.dtRentalContractBasic != null)
                {
                    ProductTypeCode = sParam.dtRentalContractBasic.ProductTypeCode;
                }
                else if (sParam.dtSale != null)
                {
                    ProductTypeCode = sParam.dtSale.ProductTypeCode;
                }


                #endregion

                doInstrumentSearchCondition dCond = new doInstrumentSearchCondition()
                {
                    InstrumentCode = cond.InstrumentCode
                };

                //========================= TRS 30/05/2012 Comment ==========================
                //if (ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SALE)
                //    dCond.SaleFlag = 1;
                //if (ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL)
                //    dCond.RentalFlag = 1;
                //===========================================================================

                //dCond.ExpansionType = new List<string>() { SECOM_AJIS.Common.Util.ConstantValue.ExpansionType.C_EXPANSION_TYPE_PARENT };
                //dCond.InstrumentType = new List<string>() { SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_GENERAL };

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
        /// Generate email template
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        private doEmailTemplate ISS030_GenerateMailRegisterInstallationPO(ISS030_ScreenParameter sParam)
        {
            try
            {
                doEmailContentSlip obj = new doEmailContentSlip();

                //obj.ContractCode = sParam.do_TbtInstallationBasic.ContractProjectCode;

                if (sParam.dtRentalContractBasic != null)
                {
                    obj.ContractTargetNameLC = sParam.dtRentalContractBasic.ContractTargetNameLC;
                    obj.ContractTargetNameEN = sParam.dtRentalContractBasic.ContractTargetNameEN;
                    obj.SiteNameLC = sParam.dtRentalContractBasic.SiteNameLC;
                    obj.SiteNameEN = sParam.dtRentalContractBasic.SiteNameEN;
                    obj.ProductNameLC = sParam.dtRentalContractBasic.ProductNameLC;
                    obj.ProductNameEN = sParam.dtRentalContractBasic.ProductNameEN;
                    obj.Salesman1NameLC = sParam.dtRentalContractBasic.SalesmanLC1;
                    obj.Salesman1NameEN = sParam.dtRentalContractBasic.SalesmanEN1;
                    obj.Salesman2NameLC = sParam.dtRentalContractBasic.SalesmanLC2;
                    obj.Salesman2NameEN = sParam.dtRentalContractBasic.SalesmanEN2;
                    obj.OperationOfficeNameLC = sParam.dtRentalContractBasic.OperationOfficeNameLC;
                    obj.OperationOfficeNameEN = sParam.dtRentalContractBasic.OperationOfficeNameEN;
                }
                else
                {
                    obj.ContractTargetNameLC = sParam.dtSale.ContractTargetNameLC;
                    obj.ContractTargetNameEN = sParam.dtSale.ContractTargetNameEN;
                    obj.SiteNameLC = sParam.dtSale.SiteNameLC;
                    obj.SiteNameEN = sParam.dtSale.SiteNameEN;
                    obj.ProductNameLC = sParam.dtSale.ProductNameLC;
                    obj.ProductNameEN = sParam.dtSale.ProductNameEN;
                    obj.Salesman1NameLC = sParam.dtSale.SalesmanLC1;
                    obj.Salesman1NameEN = sParam.dtSale.SalesmanEN1;
                    obj.Salesman2NameLC = sParam.dtSale.SalesmanLC2;
                    obj.Salesman2NameEN = sParam.dtSale.SalesmanEN2;
                    obj.OperationOfficeNameLC = sParam.dtSale.OperationOfficeNameLC;
                    obj.OperationOfficeNameEN = sParam.dtSale.OperationOfficeNameEN;
                }
                if (sParam.do_TbtInstallationSlip != null)
                {
                    //========== GET CHANGE REASON NAME ===============================
                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                    List<doMiscTypeCode> miscs;
                    if (sParam.do_TbtInstallationSlip.ChangeReasonCode != null)
                    {
                        miscs = new List<doMiscTypeCode>()
                        {
                            new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_INSTALL_CHANGE_REASON,
                                ValueCode = sParam.do_TbtInstallationSlip.ChangeReasonCode
                            }
                        };

                        lst = chandler.GetMiscTypeCodeList(miscs);
                    }
                    if (lst != null && lst.Count > 0)
                    {
                        obj.ChangeReasonTypeEN = lst[0].ValueDisplayEN;
                        obj.ChangeReasonTypeLC = lst[0].ValueDisplayLC;
                    }
                    //==================================================================
                    //============ GET CAUSE REASON ====================================
                    lst = new List<doMiscTypeCode>();

                    if (sParam.do_TbtInstallationSlip.ChangeReasonCode != null && sParam.do_TbtInstallationSlip.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_CUS)
                    {
                        miscs = new List<doMiscTypeCode>()
                        {
                            new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_CUSTOMER_REASON,
                                ValueCode = sParam.do_TbtInstallationSlip.CauseReason
                            }
                        };

                        lst = chandler.GetMiscTypeCodeList(miscs);
                        if (lst != null && lst.Count > 0)
                        {
                            obj.InstallationCauseReasonEN = lst[0].ValueDisplayEN;
                            obj.InstallationCauseReasonLC = lst[0].ValueDisplayLC;
                        }
                    }
                    else if (sParam.do_TbtInstallationSlip.ChangeReasonCode != null && sParam.do_TbtInstallationSlip.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_SECOM)
                    {
                        miscs = new List<doMiscTypeCode>()
                        {
                            new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_SECOM_REASON,
                                ValueCode = sParam.do_TbtInstallationSlip.CauseReason
                            }
                        };

                        lst = chandler.GetMiscTypeCodeList(miscs);
                        if (lst != null && lst.Count > 0)
                        {
                            obj.InstallationCauseReasonEN = lst[0].ValueDisplayEN;
                            obj.InstallationCauseReasonLC = lst[0].ValueDisplayLC;
                        }
                    }
                    //==================================================================
                    //======== GET INSTALLATION TYPE NAME =============================
                    string MiscInstallType = "";

                    lst = new List<doMiscTypeCode>();

                    if (sParam.do_TbtInstallationBasic != null && sParam.do_TbtInstallationBasic.InstallationType != null)
                    {
                        if (sParam.do_TbtInstallationBasic.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                        {
                            MiscInstallType = MiscType.C_RENTAL_INSTALL_TYPE;
                        }
                        else if (sParam.do_TbtInstallationBasic.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                        {
                            MiscInstallType = MiscType.C_SALE_INSTALL_TYPE;
                        }
                        miscs = new List<doMiscTypeCode>()
                        {
                            new doMiscTypeCode()
                            {
                                FieldName = MiscInstallType,
                                ValueCode = sParam.do_TbtInstallationBasic.InstallationType
                            }
                        };

                        lst = chandler.GetMiscTypeCodeList(miscs);
                    }
                    if (lst != null && lst.Count > 0)
                    {
                        obj.InstallationTypeEN = lst[0].ValueDisplayEN;
                        obj.InstallationTypeLC = lst[0].ValueDisplayLC;
                    }
                }
                //obj.MinimumExpectedInstallationStartDate = MinimumInstallDate.ToString("dd-MMM-yyyy");
                //obj.MaximumExpectedInstallationCompleteDate = MaximumCompleteDate.ToString("dd-MMM-yyyy");

                //obj.ContractCode = sParam.do_TbtInstallationBasic.ContractProjectCode;
                obj.ContractCode = sParam.ContractCodeShort;
                if (sParam.do_TbtInstallationBasic != null)
                    obj.InstallationMaintenanceNo = sParam.do_TbtInstallationBasic.MaintenanceNo;
                //obj.InstallationTypeLC = sParam.do_TbtInstallationBasic.InstallationType;
                //obj.InstallationTypeEN = sParam.do_TbtInstallationBasic.InstallationType;
                obj.SenderLC = EmailSenderName.C_EMAIL_SENDER_NAME_LC;
                obj.SenderEN = EmailSenderName.C_EMAIL_SENDER_NAME_EN;

                EmailTemplateUtil util = new EmailTemplateUtil(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_INSTALL_SLIP);
                return util.LoadTemplate(obj);
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Get facility code
        /// </summary>
        /// <param name="FacilityCode"></param>
        /// <returns></returns>
        public ActionResult ISS030_GetFacilityCode(string FacilityCode)
        {
            ObjectResultData res = new ObjectResultData();
            IInstrumentMasterHandler InstrumentHandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
            doInstrumentSearchCondition doSearchCon = new doInstrumentSearchCondition();
            doSearchCon.InstrumentCode = FacilityCode;
            List<doInstrumentData> doResult = InstrumentHandler.GetInstrumentDataForSearch(doSearchCon);
            res.ResultData = doResult;
            return Json(res);
        }

        /// <summary>
        /// Get rental installation type other case
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS030_GetRentalInstalltypeOtherCase(string strFieldName)
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
                lst = (from c in lst
                       where c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                       select c).ToList<doMiscTypeCode>();

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
        /// Get rental installation type exclude remove all
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS030_GetRentalInstalltypeExceptRemoveAll(string strFieldName)
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
                lst = (from c in lst
                       where c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                       //|| c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                       || c.ValueCode == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                       select c).ToList<doMiscTypeCode>();

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
        /// Get sale installation type other case
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS030_GetSaleInstalltypeOtherCase(string strFieldName)
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
                lst = (from c in lst
                       where c.ValueCode == SaleInstallationType.C_SALE_INSTALL_TYPE_CHANGE_WIRING
                       || c.ValueCode == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                       || c.ValueCode == SaleInstallationType.C_SALE_INSTALL_TYPE_MOVE
                       || c.ValueCode == SaleInstallationType.C_SALE_INSTALL_TYPE_PARTIAL_REMOVE
                       || c.ValueCode == SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL
                       select c).ToList<doMiscTypeCode>();

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
        /// Check CP12
        /// </summary>
        /// <param name="FacilityCode"></param>
        /// <returns></returns>
        public ActionResult ISS030_GetCheckCP12(string temp)
        {
            ObjectResultData res = new ObjectResultData();
            ISS030_ScreenParameter sParam = GetScreenObject<ISS030_ScreenParameter>();
            //====================== get for initial installation type ==========================
            IRentralContractHandler rentalContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            string tmpStrOCC = rentalContractHandler.GetLastUnimplementedOCC(sParam.ContractCodeLong);
            bool blnCheckCP12 = false;
            if (!CommonUtil.IsNullOrEmpty(tmpStrOCC))
            {
                blnCheckCP12 = rentalContractHandler.CheckCP12(sParam.ContractCodeLong, tmpStrOCC);
            }
            //===================================================================================
            res.ResultData = blnCheckCP12;
            return Json(res);
        }

        /// <summary>
        /// Get Customer reason combo box without new work
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS030_GetCustomerReasonWithoutNewWork(string strFieldName)
        {

            string strDisplayName = "ValueCodeDisplay";
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_CUSTOMER_REASON,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);

                lst = (from t in lst
                       where t.ValueCode != CustomerReason.C_CUSTOMER_REASON_NEW_WORK
                       select t).ToList<doMiscTypeCode>();

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
        /// Upload attach file
        /// </summary>
        /// <param name="fileSelect"></param>
        /// <param name="DocumentName"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult ISS030_AttachFileDoc(HttpPostedFileBase fileSelect, string DocumentName, string k)
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
                        var fList = commonhandler.GetAttachFileForGridView(GetCurrentKey());

                        var filterDupItem = from a in fList where a.FileName.ToUpper().Equals(DocumentName.ToUpper() + Path.GetExtension(fileSelect.FileName).ToUpper()) select a;

                        if (filterDupItem.Count() > 0)
                        {
                            // Docname duplicate
                            outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115, new string[] { DocumentName });
                        }
                        else if (DocumentName == null || DocumentName == "")
                        {
                            string nparam = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "ISS030", "lblDocumentName");
                            outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { nparam });
                        }
                        else
                        {
                            fileData = reader.ReadBytes(fileSelect.ContentLength);

                            if (commonhandler.CanAttachFile(DocumentName, fileData.Length, Path.GetExtension(fileSelect.FileName), GetCurrentKey(), GetCurrentKey()))
                            {
                                DateTime currDate = DateTime.Now;
                                commonhandler.InsertAttachFile(GetCurrentKey()
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

            return View("ISS030_Upload");
        }

        /// <summary>
        /// Remove attach file
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult ISS030_RemoveAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                int _attachID = int.Parse(AttachID);

                commonhandler.DeleteAttachFileByID(_attachID, GetCurrentKey());

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Clear all attach files in session parameter
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS030_ClearAllAttach()
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                List<tbt_AttachFile> tmpFileList = commonhandler.GetTbt_AttachFile(GetCurrentKey(), null, false);
                if (tmpFileList.Count > 0)
                {
                    foreach (tbt_AttachFile attachData in tmpFileList)
                    {
                        commonhandler.DeleteAttachFileByID(attachData.AttachFileID, GetCurrentKey());
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
        /// Load data attach to show in grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS030_LoadGridAttachedDocList()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                List<dtAttachFileForGridView> lstAttachedName = commonhandler.GetAttachFileForGridView(GetCurrentKey());
                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Installation\\ISS030_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

    }
}



