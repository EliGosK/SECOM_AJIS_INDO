
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
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Quotation;
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
        /// Authority screen ISS060
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ISS060_Authority(ISS060_ScreenParameter param)
        {
            // permission
            #region ============== Check suspend and permission =======================
            ObjectResultData res = new ObjectResultData();
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            if (chandler.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_COMPLETE, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }
            #endregion =================================================================


            //=============== Add new spec 26/04/2012 Common Contract COde for Search ==============
            //if (CommonUtil.IsNullOrEmpty(param.strContractCode) && !CommonUtil.IsNullOrEmpty(CommonUtil.dsTransData.dtCommonSearch.ContractCode))
            //{
            //    param.strContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
            //}
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
            //======================================================================================
            // parameter
            //ISS060_ScreenParameter param = new ISS060_ScreenParameter();
            param.ContractCodeShort = param.strContractCode;

            //===================================== NEW REQ ======================================================
            CommonUtil comUtil = new CommonUtil();
            dtRentalContractBasicForInstall dtRentalContractBasic = new dtRentalContractBasicForInstall();
            dtSaleBasic dtSaleContractBasic = new dtSaleBasic();
            string lang = CommonUtil.GetCurrentLanguage();

            //Check mandatory
            if (String.IsNullOrEmpty(param.ContractCodeShort))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147, new string[] { "ContractCode" });
                return Json(res);
            }

            //Get rental contract data

            ///////////// START RETRIEVE DATA ////////////////////////
            #region =============== Get contract data =====================
            string strContractCodeLong = comUtil.ConvertContractCode(param.ContractCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
            param.ContractCodeLong = strContractCodeLong;
            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            IRentralContractHandler RentalContactHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            dtRentalContractBasic = RentalContactHandler.GetRentalContractBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);

            if (res.IsError)
                return Json(res);

            if (dtRentalContractBasic != null)
            {
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

                param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                param.InstallType = MiscType.C_RENTAL_INSTALL_TYPE;
                param.dtRentalContractBasic = dtRentalContractBasic;

                //param.ContractProjectCodeForShow = comUtil.ConvertContractCode(dtRentalContractBasic.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                param.ContractCodeLong = dtRentalContractBasic.ContractCode;
                param.ContractCodeShort = comUtil.ConvertContractCode(param.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                param.ContractProjectCodeForShow = param.ContractCodeShort;

                dtSaleContractBasic = null;
                if (ISS060_ValidExistOffice(dtRentalContractBasic.OperationOfficeCode) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    return Json(res);
                }
            }
            else
            {

                dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);

                if (dtSaleContractBasic != null)
                {
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

                    param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                    param.InstallType = MiscType.C_SALE_INSTALL_TYPE;
                    param.dtSale = dtSaleContractBasic;
                    //param.ContractProjectCodeForShow = comUtil.ConvertContractCode(dtSaleContractBasic.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    param.ContractCodeLong = dtSaleContractBasic.ContractCode;
                    param.ContractCodeShort = comUtil.ConvertContractCode(param.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    param.ContractProjectCodeForShow = param.ContractCodeShort;

                    dtRentalContractBasic = null;
                    if (ISS060_ValidExistOffice(dtSaleContractBasic.OperationOfficeCode) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                        return Json(res);
                    }
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124, null);
                    return Json(res);
                }
            }
            #endregion ========================================================================
            #region ================== Get Tbt_InstallationBasic data ======================
            List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(strContractCodeLong);
            if (tmpdoTbt_InstallationBasic != null && tmpdoTbt_InstallationBasic.Count > 0)
            {
                tbt_InstallationSlip do_TbtInstallationSlip = handler.GetTbt_InstallationSlipData(tmpdoTbt_InstallationBasic[0].SlipNo);
                if (do_TbtInstallationSlip == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5071);
                    return Json(res);
                }
                else
                {
                    //================ Teerapong S. 31/08/2012 =====================
                    param.do_TbtInstallationSlip = do_TbtInstallationSlip;
                    //==============================================================
                }

                if (do_TbtInstallationSlip.SlipStatus != SlipStatus.C_SLIP_STATUS_STOCK_OUT && do_TbtInstallationSlip.SlipStatus != SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5039);
                    return Json(res);
                }
                if (tmpdoTbt_InstallationBasic[0].InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5093);
                    return Json(res);
                }

                if (tmpdoTbt_InstallationBasic[0].InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REQUESTED)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5101);
                    return Json(res);
                }
                //================ Teerapong S. 31/08/2012 =====================
                param.do_TbtInstallationBasic = tmpdoTbt_InstallationBasic[0];
                param.TbtInstallationBasicUpdateDate = tmpdoTbt_InstallationBasic[0].UpdateDate;
                //==============================================================
            }
            else
            {
                res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5072);
                return Json(res);
            }
            #endregion =====================================================================
            //===================================== NEW REQ =============================================
            UpdateScreenObject(param);
            return InitialScreenEnvironment<ISS060_ScreenParameter>("ISS060", param, res);
        }
        /// <summary>
        /// Initial screen ISS060
        /// </summary>
        /// <returns></returns>
        [Initialize("ISS060")]
        public ActionResult ISS060()
        {
            //ISS060_ScreenParameter param = new ISS060_ScreenParameter();
            ISS060_ScreenParameter param = GetScreenObject<ISS060_ScreenParameter>();
            if (param != null)
            {
                ViewBag.ContractCodeLong = param.ContractCodeLong;
                ViewBag.ContractProjectCode = param.ContractCodeShort;
            }
            ViewBag.AttachKey = GetCurrentKey();

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

        // InitialGridEmail
        /// <summary>
        /// Initial grid email schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS060_InitialGridEmail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS060_Email", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// Initial grid instrument schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS060_InitialGridInstrumentInfo()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS060_InstrumentDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// Retrieve data for show screen ISS060
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public ActionResult ISS060_RetrieveData(string strContractCode)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            dtRentalContractBasicForInstall dtRentalContractBasic = new dtRentalContractBasicForInstall();
            dtSaleBasic dtSaleContractBasic = new dtSaleBasic();
            string lang = CommonUtil.GetCurrentLanguage();
            ISS060_RegisterStartResumeTargetData result = new ISS060_RegisterStartResumeTargetData();
            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            MiscTypeMappingList miscMapping = new MiscTypeMappingList();
            try
            {

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ISS060_ScreenParameter sParam = GetScreenObject<ISS060_ScreenParameter>();
                sParam.ContractCodeShort = strContractCode;

                //sParam.dtRentalContractBasic = new dtRentalContractBasicForInstall();
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                #region //=========================== Change Req to check at Authority =====================================
                ////Check mandatory
                //if (String.IsNullOrEmpty(strContractCode))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5057, new string[] { "ContractCode" }); 
                //    return Json(res);
                //}

                ////Get rental contract data

                /////////////// START RETRIEVE DATA ////////////////////////

                ///////////// START RETRIEVE DATA ////////////////////////
                #region =============== Get contract data =====================

                IRentralContractHandler RentalContactHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                dtRentalContractBasic = RentalContactHandler.GetRentalContractBasicDataForInstall(sParam.ContractCodeLong, MiscType.C_BUILDING_TYPE);

                if (res.IsError)
                    return Json(res);

                if (dtRentalContractBasic != null)
                {
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
                    sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                    sParam.InstallType = MiscType.C_RENTAL_INSTALL_TYPE;
                    sParam.dtRentalContractBasic = dtRentalContractBasic;

                    sParam.ContractCodeLong = dtRentalContractBasic.ContractCode;
                    sParam.ContractCodeShort = comUtil.ConvertContractCode(sParam.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    sParam.ContractProjectCodeForShow = sParam.ContractCodeShort;

                    dtSaleContractBasic = null;
                    if (ISS060_ValidExistOffice(dtRentalContractBasic.OperationOfficeCode) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                        return Json(res);
                    }
                }
                else
                {
                    dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(sParam.ContractCodeLong, MiscType.C_BUILDING_TYPE);

                    if (dtSaleContractBasic != null)
                    {
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
                        sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                        sParam.InstallType = MiscType.C_SALE_INSTALL_TYPE;
                        sParam.dtSale = dtSaleContractBasic;

                        sParam.ContractCodeLong = dtSaleContractBasic.ContractCode;
                        sParam.ContractCodeShort = comUtil.ConvertContractCode(sParam.ContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        sParam.ContractProjectCodeForShow = sParam.ContractCodeShort;

                        dtRentalContractBasic = null;
                        if (ISS060_ValidExistOffice(dtSaleContractBasic.OperationOfficeCode) == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                            return Json(res);
                        }
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124, null);
                        return Json(res);
                    }
                }
                #endregion ========================================================================
                #endregion //==================================================================================================

                //************************************* GET DATA doIB Email ************************************************
                #region ////////////////////// RETRIEVE Installation Basic /////////////////
                //sParam.do_TbtInstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);

                //================ Teerapong S. 31/08/2012 =====================
                //sParam.do_TbtInstallationBasic = null;
                List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic;
                //if (sParam.do_TbtInstallationBasic != null)
                //{
                //    tmpdoTbt_InstallationBasic = new List<tbt_InstallationBasic>();
                //    tmpdoTbt_InstallationBasic.Add(sParam.do_TbtInstallationBasic);
                //}
                //else
                //{
                //    tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);    
                //}
                tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);
                //==============================================================



                if (tmpdoTbt_InstallationBasic != null)
                {
                    //==================== map misc type =========================
                    miscMapping = new MiscTypeMappingList();
                    miscMapping.AddMiscType(tmpdoTbt_InstallationBasic.ToArray());
                    handlerCommon.MiscTypeMappingList(miscMapping);
                    //============================================================
                    sParam.do_TbtInstallationBasic = new tbt_InstallationBasic();
                    sParam.do_TbtInstallationBasic = tmpdoTbt_InstallationBasic[0];
                    sParam.TbtInstallationBasicUpdateDate = tmpdoTbt_InstallationBasic[0].UpdateDate; //Add by Jutarat A. on 16092013
                }
                #endregion ===================================================================
                if (sParam.do_TbtInstallationBasic != null)
                {
                    //sParam.m_blnbFirstTimeRegister = false;
                    sParam.MaintenanceNo = sParam.do_TbtInstallationBasic.SlipNo;
                    #region /////////////////// RETRIEVE EMAIL ////////////////////////
                    List<tbt_InstallationEmail> ListEmail = handler.GetTbt_InstallationEmailData(sParam.do_TbtInstallationBasic.SlipNo);
                    ISS060_DOEmailData TempEmail;
                    List<ISS060_DOEmailData> listAddEmail = new List<ISS060_DOEmailData>();
                    if (ListEmail != null)
                    {
                        foreach (tbt_InstallationEmail dataEmail in ListEmail)
                        {
                            TempEmail = new ISS060_DOEmailData();
                            TempEmail.EmailAddress = dataEmail.EmailNoticeTarget;
                            listAddEmail.Add(TempEmail);
                        }
                    }

                    sParam.ListDOEmail = listAddEmail;
                    #endregion
                    #region ============ get memo ============================
                    List<tbt_InstallationMemo> doInstallMemo = handler.GetTbt_InstallationMemo(sParam.do_TbtInstallationBasic.SlipNo);
                    if (doInstallMemo != null && doInstallMemo.Count > 0)
                    {
                        result.Memo = doInstallMemo[0].Memo;
                    }
                    #endregion ================================================
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5072, null);
                    return Json(res);
                    //sParam.m_blnbFirstTimeRegister = true;
                    sParam.ListDOEmail = null;
                }
                //*******************************************************************************************************************
                ////////////////// INITIAL INSTRUMENT DETAILS /////////////////////
                //sParam.do_TbtInstallationSlipDetails


                ////////////////// RETRIEVE SLIP ///////////////////////////
                //================ Teerapong S. 31/08/2012 =====================
                //if (sParam.do_TbtInstallationSlip == null)
                //{                
                sParam.do_TbtInstallationSlip = handler.GetTbt_InstallationSlipData(sParam.do_TbtInstallationBasic.SlipNo);
                //}
                //==============================================================
                if (sParam.do_TbtInstallationSlip != null)
                {
                    #region ==== get install fee billing type name ==========
                    ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> lstmiscs = new List<doMiscTypeCode>();
                    if (sParam.do_TbtInstallationSlip.InstallFeeBillingType != null)
                    {
                        List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                        {
                            new doMiscTypeCode()
                            {
                                FieldName = "InstallFeeBillingType",
                                ValueCode = sParam.do_TbtInstallationSlip.InstallFeeBillingType
                            }
                        };
                        lstmiscs = hand.GetMiscTypeCodeList(miscs);
                    }
                    if (lstmiscs != null && lstmiscs.Count > 0)
                    {
                        sParam.do_TbtInstallationSlip.InstallFeeBillingTypeName = lstmiscs[0].ValueCodeDisplay;
                    }
                    #endregion ==============================================
                    //=============================================================
                    #region ========= get issue office name ==============
                    if (CommonUtil.dsTransData != null)
                    {
                        List<OfficeDataDo> OfficeTemp = (from t in CommonUtil.dsTransData.dtOfficeData
                                                         where t.OfficeCode == sParam.do_TbtInstallationSlip.SlipIssueOfficeCode
                                                         select t).ToList<OfficeDataDo>();

                        CommonUtil.MappingObjectLanguage<OfficeDataDo>(OfficeTemp);

                        if (OfficeTemp != null && OfficeTemp.Count > 0)
                        {
                            sParam.do_TbtInstallationSlip.SlipIssueOfficeName = OfficeTemp[0].OfficeName;
                        }
                    }

                    if(sParam.do_TbtInstallationSlip.NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    {
                        sParam.do_TbtInstallationSlip.NormalContractFee = sParam.do_TbtInstallationSlip.NormalContractFee;
                    }
                    else { sParam.do_TbtInstallationSlip.NormalContractFee = sParam.do_TbtInstallationSlip.NormalContractFeeUsd; }

                    // 20170217 nakajima add start
                    if (sParam.do_TbtInstallationSlip.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    {
                        sParam.do_TbtInstallationSlip.NormalInstallFee = sParam.do_TbtInstallationSlip.NormalInstallFee;
                    }
                    else { sParam.do_TbtInstallationSlip.NormalInstallFee = sParam.do_TbtInstallationSlip.NormalInstallFeeUsd; }

                    if (sParam.do_TbtInstallationSlip.BillingInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    {
                        sParam.do_TbtInstallationSlip.BillingInstallFeeCurrencyType = sParam.do_TbtInstallationSlip.BillingInstallFeeCurrencyType;
                    }
                    else { sParam.do_TbtInstallationSlip.BillingInstallFee = sParam.do_TbtInstallationSlip.BillingInstallFeeUsd; }
                    // 20170217 nakajima add end

                    #endregion ==========================================
                    //=============================================================

                    List<tbt_InstallationSlip> do_TbtInstallationSlipList = new List<tbt_InstallationSlip>();
                    do_TbtInstallationSlipList.Add(sParam.do_TbtInstallationSlip);
                    miscMapping = new MiscTypeMappingList();
                    miscMapping.AddMiscType(do_TbtInstallationSlipList.ToArray());
                    handlerCommon.MiscTypeMappingList(miscMapping);
                    sParam.do_TbtInstallationSlip = do_TbtInstallationSlipList[0];
                    //============================================================================
                    #region ======== get LastSlipStatusName ================
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
                    #endregion ===========================================

                    #region //===================== Retrieve Slip Details ======================
                    List<tbt_InstallationSlipDetails> ListTemp_TbtInstallationSlipDetails = new List<tbt_InstallationSlipDetails>();
                    ListTemp_TbtInstallationSlipDetails = handler.GetTbt_InstallationSlipDetailsData(sParam.do_TbtInstallationBasic.SlipNo, null, InstrumentType.C_INST_TYPE_GENERAL);
                    //*********************** TRS 22/05/2012 *********************************
                    List<tbt_InstallationSlipDetails> ListTbtInstallationSlipDetailsForChild = new List<tbt_InstallationSlipDetails>();
                    //************************************************************************
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

                            //************************ TRS 22/05/2011 CHANGE NEW SPEC **************************
                            #region =========== Get child Instrument =================
                            dataInstrument.IsUnremovable = true;
                            dataInstrument.IsParent = true;
                            dataInstrument.ParentCode = dataInstrument.InstrumentCode;
                            List<tbm_InstrumentExpansion> ListExpansion = InstrumentHandler.GetTbm_InstrumentExpansion(dataInstrument.InstrumentCode, null);
                            if (ListExpansion != null && ListExpansion.Count > 0)
                            {
                                dataInstrument.IsUnremovable = false;
                                foreach (tbm_InstrumentExpansion ExpData in ListExpansion)
                                {
                                    tbt_InstallationSlipDetails tempInstData = new tbt_InstallationSlipDetails();
                                    tempInstData.InstrumentCode = ExpData.ChildInstrumentCode;
                                    tempInstData.UnremovableQty = 0;
                                    tempInstData.ParentCode = dataInstrument.InstrumentCode;
                                    tempInstData.IsUnremovable = true;
                                    tempInstData.IsParent = false;

                                    ListTbtInstallationSlipDetailsForChild.Add(tempInstData);
                                }
                            }
                            #endregion ==================================================
                            //**********************************************************************************

                            countInstrumentList++;
                        }

                        //************************ TRS 22/05/2011 CHANGE NEW SPEC **************************
                        #region ============= Add Child Instrument ====================
                        foreach (tbt_InstallationSlipDetails childInstrument in ListTbtInstallationSlipDetailsForChild)
                        {
                            ListTemp_TbtInstallationSlipDetails.Add(childInstrument);
                        }
                        ListTemp_TbtInstallationSlipDetails = (from t in ListTemp_TbtInstallationSlipDetails
                                                               orderby t.ParentCode,
                                                               t.IsParent descending, t.InstrumentCode
                                                               select t).ToList<tbt_InstallationSlipDetails>();
                        #endregion =========================================================
                        //**********************************************************************************

                        sParam.do_TbtInstallationSlipDetails = ListTemp_TbtInstallationSlipDetails;

                    }
                    #endregion //==================================================================
                }

                //Add by Jutarat A. on 11042013
                //2. Compare contract data and installation data
                IRentralContractHandler rentalHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                //2.1 Get last unimplement OCC from contract module
                string strUnimplementOCC = rentalHandler.GetLastUnimplementedOCC(sParam.ContractCodeLong);

                //2.2 Get rental security basic 
                List<tbt_RentalSecurityBasic> doTbt_RentalSecurityBasic = rentalHandler.GetTbt_RentalSecurityBasic(sParam.ContractCodeLong, strUnimplementOCC);
                sParam.do_TbtRentalSecurityBasicLastUnimp = doTbt_RentalSecurityBasic;

                //2.3 Create local variable (for compare data between contact and installation)
                bool blnUseContractData = false;

                //2.4 Compare contract data and installation data
                if (sParam.do_TbtInstallationSlip == null
                    || (sParam.do_TbtInstallationSlip != null && doTbt_RentalSecurityBasic != null && doTbt_RentalSecurityBasic.Count > 0
                        && doTbt_RentalSecurityBasic[0].UpdateDate > sParam.do_TbtInstallationSlip.CreateDate)
                    || (sParam.do_TbtInstallationSlip != null && sParam.dtSale != null && sParam.dtSale.UpdateDate > sParam.do_TbtInstallationSlip.CreateDate)
                    )
                {
                    blnUseContractData = true;
                }
                else
                {
                    blnUseContractData = false;
                }

                sParam.IsUseContractData = blnUseContractData;
                //End Add

                res = ISS060_ValidateContractError(sParam);
                if (res.IsError)
                {
                    result.blnValidateContractError = false;
                    res.ResultData = result;
                    return Json(res);
                }
                else
                {
                    result.blnValidateContractError = true;
                }

                //---------- Validate data -----------
                if (sParam.do_TbtInstallationSlip.SlipStatus != SlipStatus.C_SLIP_STATUS_STOCK_OUT && sParam.do_TbtInstallationSlip.SlipStatus != SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5039);
                    return Json(res);
                }
                if (sParam.do_TbtInstallationBasic.InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5093);
                    return Json(res);
                }

                if (sParam.do_TbtInstallationBasic.InstallationStatus == InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REQUESTED)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5101);
                    return Json(res);
                }

                result.dtRentalContractBasic = sParam.dtRentalContractBasic;
                result.dtSale = sParam.dtSale;
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
                sParam.ContractCodeShort = sParam.ContractProjectCodeForShow;
                result.ContractCodeShort = sParam.ContractCodeShort;
                result.ContractProjectCodeForShow = sParam.ContractProjectCodeForShow;
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
        public ActionResult ISS060_GetInstallEmail(string strEmail)
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
                    List<ISS060_DOEmailData> listEmail;
                    ISS060_ScreenParameter session = GetScreenObject<ISS060_ScreenParameter>();
                    ISS060_DOEmailData doISS060EmailADD = new ISS060_DOEmailData();

                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS060_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    doISS060EmailADD.EmpNo = emailList[0].EmpNo;
                    doISS060EmailADD.EmailAddress = emailList[0].EmailAddress;

                    if (listEmail.FindAll(delegate(ISS060_DOEmailData s) { return s.EmpNo == doISS060EmailADD.EmpNo; }).Count() == 0)
                        listEmail.Add(doISS060EmailADD);
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
        public ActionResult ISS060_ValidateRegisterData()
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
        /// Validate email data
        /// </summary>
        /// <param name="doISS060Email"></param>
        /// <returns></returns>
        public ActionResult ValidateEmail_ISS060(ISS060_DOEmailData doISS060Email)
        {
            List<dtGetEmailAddress> dtEmail;
            ISS060_DOEmailData doISS060EmailADD;
            IEmployeeMasterHandler employeeMasterHandler;
            ObjectResultData res = new ObjectResultData();
            ISS060_ScreenParameter session;
            employeeMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            dtEmail = employeeMasterHandler.GetEmailAddress(doISS060Email.EmpFirstNameEN, doISS060Email.EmailAddress, doISS060Email.OfficeCode, doISS060Email.DepartmentCode);

            try
            {
                session = GetScreenObject<ISS060_ScreenParameter>();
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                    {
                        res.ResultData = false;
                        return Json(res);
                    }
                }


                //doISS060EmailADD = new ISS060_DOEmailData();
                //doISS060EmailADD.EmpNo = "540886";
                //doISS060EmailADD.EmailAddress = "Nattapong@csithai.com";
                //session.DOEmail = doISS060EmailADD;

                if (dtEmail != null)
                {
                    if (dtEmail.Count() == 0)
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, doISS060Email.EmailAddress);
                    else
                    {
                        doISS060EmailADD = new ISS060_DOEmailData();
                        doISS060EmailADD.EmpNo = dtEmail[0].EmpNo;
                        doISS060EmailADD.EmailAddress = doISS060Email.EmailAddress;
                        session.DOEmail = doISS060EmailADD;

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
        /// upload attach file
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS060_Upload()
        {
            ViewBag.K = GetCurrentKey();
            return View();
        }
        /// <summary>
        /// Get data for Installation type combobox
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>       
        [HttpPost]
        public ActionResult ISS060_GetMiscInstallationtype(string strFieldName)
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
        /// Get data installation type by code
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="ValueCode"></param>
        /// <returns></returns>
        public ActionResult ISS060_GetMiscTypeByValueCode(string FieldName, string ValueCode)
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

                if (lst.Count > 0)
                {
                    res.ResultData = lst[0].ValueCodeDisplay;
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
        /// Get office name by code
        /// </summary>
        /// <param name="ValueCode"></param>
        /// <returns></returns>
        public ActionResult ISS060_GetOfficeNameByCode(string ValueCode)
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
        /// Get email data and show in grid email 
        /// </summary>
        /// <param name="doCTS053Email"></param>
        /// <returns></returns>
        public ActionResult GetEmail_ISS060(ISS060_DOEmailData doCTS053Email)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            ISS060_DOEmailData doISS060EmailADD;
            List<ISS060_DOEmailData> listEmail;
            ISS060_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS060_ScreenParameter>();
                doISS060EmailADD = new ISS060_DOEmailData();
                if (session.ListDOEmail == null)
                    listEmail = new List<ISS060_DOEmailData>();
                else
                    listEmail = session.ListDOEmail;

                doISS060EmailADD.EmpNo = session.DOEmail.EmpNo;
                doISS060EmailADD.EmailAddress = doCTS053Email.EmailAddress;

                if (listEmail.FindAll(delegate(ISS060_DOEmailData s) { return s.EmpNo == doISS060EmailADD.EmpNo; }).Count() == 0)
                    listEmail.Add(doISS060EmailADD);

                session.DOEmail = null;
                session.ListDOEmail = listEmail;
                res.ResultData = CommonUtil.ConvertToXml<ISS060_DOEmailData>(session.ListDOEmail, "Contract\\CTS053Email", CommonUtil.GRID_EMPTY_TYPE.INSERT);
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
        public ActionResult ISS060_GetEmailList(List<ISS060_DOEmailData> listEmailAdd)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            List<ISS060_DOEmailData> listEmail;
            List<ISS060_DOEmailData> listNewEmail;
            ISS060_ScreenParameter session;

            try
            {
                listNewEmail = new List<ISS060_DOEmailData>();
                if (listEmailAdd != null)
                {
                    session = GetScreenObject<ISS060_ScreenParameter>();
                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS060_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    foreach (var item in listEmailAdd)
                    {
                        if (listEmail.FindAll(delegate(ISS060_DOEmailData s) { return s.EmailAddress == item.EmailAddress; }).Count() == 0)
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
        /// <param name="doISS060Email"></param>
        /// <returns></returns>
        public ActionResult RemoveMailClick_ISS060(ISS060_DOEmailData doISS060Email)
        {
            List<ISS060_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS060_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS060_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate(ISS060_DOEmailData s) { return s.EmailAddress == doISS060Email.EmailAddress; });
                if (listEmailDelete.Count() != 0)
                    session.ListDOEmail.Remove(listEmailDelete[0]);

                res.ResultData = CommonUtil.ConvertToXml<ISS060_DOEmailData>(session.ListDOEmail, "Installation\\ISS010Email", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); ;
            }

            return Json(res);
        }
        /// <summary>
        /// remove email data from session parameter
        /// </summary>
        /// <param name="doISS060Email"></param>
        /// <returns></returns>
        public ActionResult ISS060_RemoveMailClick(ISS060_DOEmailData doISS060Email)
        {
            List<ISS060_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS060_ScreenParameter session;
            try
            {
                session = GetScreenObject<ISS060_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate(ISS060_DOEmailData s) { return s.EmailAddress == doISS060Email.EmailAddress; });
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
        /// Register complete installation data
        /// </summary>
        /// <param name="doIS"></param>
        /// <param name="Memo"></param>
        /// <param name="ScreenInput"></param>
        /// <returns></returns>
        public ActionResult ISS060_RegisterData(tbt_InstallationSlip doIS, string Memo, ISS060_ScreenInput ScreenInput)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;

            try
            {
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                ISS060_ScreenParameter sParam = GetScreenObject<ISS060_ScreenParameter>();
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                //================= Teerapong S. 15/10/2012 =======================                
                List<tbt_InstallationBasic> doValidate_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);

                if ((sParam.TbtInstallationBasicUpdateDate == null)
                    || (sParam.TbtInstallationBasicUpdateDate != null && doValidate_InstallationBasic != null && DateTime.Compare(sParam.TbtInstallationBasicUpdateDate.Value, doValidate_InstallationBasic[0].UpdateDate.Value) != 0)
                    || doValidate_InstallationBasic == null
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    return Json(res);
                }
                //=================================================================

                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_COMPLETE, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    #region ////=========== UPDATE INSTALLATION BASIC =========================
                    //sParam.do_TbtInstallationBasic.ServiceTypeCode = InstallationStatus.C_INSTALL_STATUS_COMPLETED;
                    //sParam.do_TbtInstallationBasic.NormalInstallFee = ScreenInput.NewNormalInstallFee;
                    //sParam.do_TbtInstallationBasic.BillingInstallFee = ScreenInput.NewBillingInstallFee;
                    //sParam.do_TbtInstallationBasic.InstallFeeBillingType = ScreenInput.NewInstallFeeBillingType;
                    //sParam.do_TbtInstallationBasic.InstallationCompleteDate = ScreenInput.InstallCompleteDate;
                    //sParam.do_TbtInstallationBasic.InstallationCompleteProcessingDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //sParam.do_TbtInstallationBasic.ApproveNo1 = ScreenInput.NewApproveNo1;
                    //sParam.do_TbtInstallationBasic.ApproveNo2 = ScreenInput.NewApproveNo2;
                    //sParam.do_TbtInstallationBasic.InstallationStartDate = ScreenInput.InstallStartDate;
                    //sParam.do_TbtInstallationBasic.InstallationFinishDate = ScreenInput.InstallFinishDate;
                    //sParam.do_TbtInstallationBasic.NormalContractFee = ScreenInput.NewNormalContractFee;
                    //sParam.do_TbtInstallationBasic.BillingOCC = ScreenInput.NewBillingOCC;

                    //Add by Jutarat A. on 17052013
                    string strNewOCC = null;
                    IRentralContractHandler rentalHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                    string strLastOCC = null;
                    if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    {
                        //Get last unimplemented OCC
                        strLastOCC = rentalHandler.GetLastUnimplementedOCC(sParam.do_TbtInstallationBasic.ContractProjectCode);
                        if (String.IsNullOrEmpty(strLastOCC))
                        {
                            //If unimplement not exist, get last implemented OCC
                            strLastOCC = rentalHandler.GetLastImplementedOCC(sParam.do_TbtInstallationBasic.ContractProjectCode);
                        }

                        //Get entire contract data
                        dsRentalContractData dsRentalContract = rentalHandler.GetEntireContract(sParam.do_TbtInstallationBasic.ContractProjectCode, strLastOCC);

                        if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0
                            && (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
                                || (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                                    && dsRentalContract.dtTbt_RentalContractBasic[0].StartType == StartType.C_START_TYPE_ALTER_START)))
                        {
                            if (dsRentalContract != null && dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0
                                && dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_ON)
                            {
                                //Generate new occurrence (unimplement)
                                strNewOCC = rentalHandler.GenerateContractOCC(sParam.do_TbtInstallationBasic.ContractProjectCode, FlagType.C_FLAG_OFF);
                            }
                        }
                        else
                        {
                            //Generate new occurrence (implement)
                            strNewOCC = rentalHandler.GenerateContractOCC(sParam.do_TbtInstallationBasic.ContractProjectCode, FlagType.C_FLAG_ON);
                        }
                    }
                    else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        if (sParam.do_TbtInstallationBasic.InstallationType != SaleInstallationType.C_SALE_INSTALL_TYPE_NEW && sParam.do_TbtInstallationBasic.InstallationType != SaleInstallationType.C_SALE_INSTALL_TYPE_ADD)
                        {
                            //Generate sale occurrence
                            strNewOCC = saleHandler.GenerateContractOCC(sParam.do_TbtInstallationBasic.ContractProjectCode);
                        }
                    }

                    if (String.IsNullOrEmpty(strNewOCC) == false)
                        sParam.do_TbtInstallationBasic.OCC = strNewOCC;
                    //End Add

                    handler.UpdateTbt_InstallationBasic(sParam.do_TbtInstallationBasic);
                    #endregion //=================================================================

                    ///////////////// DELETE Instrument Detail //////////////////
                    handler.DeleteTbt_InstallationInstrumentDetail(sParam.do_TbtInstallationBasic.ContractProjectCode, null);
                    /////////////////////////////////////////////////////////////

                    ///////////////// DELETE Installation Basic //////////////////
                    handler.DeleteTbt_InstallationBasic(sParam.do_TbtInstallationBasic.ContractProjectCode);
                    //////////////////////////////////////////////////////////////    

                    #region //============ INSERT INSTALLATION HISTORY ========================
                    tbt_InstallationHistory do_TbtInstallationHistory = new tbt_InstallationHistory();
                    do_TbtInstallationHistory.ContractProjectCode = sParam.do_TbtInstallationBasic.ContractProjectCode;
                    do_TbtInstallationHistory.OCC = sParam.do_TbtInstallationBasic.OCC;
                    do_TbtInstallationHistory.ServiceTypeCode = sParam.do_TbtInstallationBasic.ServiceTypeCode;
                    do_TbtInstallationHistory.InstallationStatus = sParam.do_TbtInstallationBasic.InstallationStatus;
                    do_TbtInstallationHistory.InstallationType = sParam.do_TbtInstallationBasic.InstallationType;
                    do_TbtInstallationHistory.PlanCode = sParam.do_TbtInstallationBasic.PlanCode;
                    do_TbtInstallationHistory.SlipNo = sParam.do_TbtInstallationBasic.SlipNo;
                    do_TbtInstallationHistory.MaintenanceNo = sParam.do_TbtInstallationBasic.MaintenanceNo;
                    do_TbtInstallationHistory.OperationOfficeCode = sParam.do_TbtInstallationBasic.OperationOfficeCode;
                    do_TbtInstallationHistory.SecurityTypeCode = sParam.do_TbtInstallationBasic.SecurityTypeCode;
                    do_TbtInstallationHistory.ChangeReasonTypeCode = sParam.do_TbtInstallationBasic.ChangeReasonTypeCode;
                    do_TbtInstallationHistory.NormalInstallFee = sParam.do_TbtInstallationBasic.NormalInstallFee;
                    do_TbtInstallationHistory.BillingInstallFee = sParam.do_TbtInstallationBasic.BillingInstallFee;
                    do_TbtInstallationHistory.InstallFeeBillingType = sParam.do_TbtInstallationBasic.InstallFeeBillingType;
                    do_TbtInstallationHistory.NormalSaleProductPrice = sParam.do_TbtInstallationBasic.NormalSaleProductPrice;
                    do_TbtInstallationHistory.BillingSalePrice = sParam.do_TbtInstallationBasic.BillingSalePrice;
                    do_TbtInstallationHistory.InstallationSlipProcessingDate = sParam.do_TbtInstallationBasic.InstallationSlipProcessingDate;
                    do_TbtInstallationHistory.InstallationCompleteDate = sParam.do_TbtInstallationBasic.InstallationCompleteDate;
                    do_TbtInstallationHistory.InstallationCompleteProcessingDate = sParam.do_TbtInstallationBasic.InstallationCompleteProcessingDate;
                    do_TbtInstallationHistory.InstallationBy = sParam.do_TbtInstallationBasic.InstallationBy;
                    do_TbtInstallationHistory.SalesmanEmpNo1 = sParam.do_TbtInstallationBasic.SalesmanEmpNo1;
                    do_TbtInstallationHistory.SalesmanEmpNo2 = sParam.do_TbtInstallationBasic.SalesmanEmpNo2;
                    do_TbtInstallationHistory.ApproveNo1 = sParam.do_TbtInstallationBasic.ApproveNo1;
                    do_TbtInstallationHistory.ApproveNo2 = sParam.do_TbtInstallationBasic.ApproveNo2;
                    do_TbtInstallationHistory.InstallationStartDate = sParam.do_TbtInstallationBasic.InstallationStartDate;
                    do_TbtInstallationHistory.InstallationFinishDate = sParam.do_TbtInstallationBasic.InstallationFinishDate;
                    do_TbtInstallationHistory.NormalContractFee = sParam.do_TbtInstallationBasic.NormalContractFee;
                    do_TbtInstallationHistory.BillingOCC = sParam.do_TbtInstallationBasic.BillingOCC;
                    do_TbtInstallationHistory.CreateDate = sParam.do_TbtInstallationBasic.CreateDate;
                    do_TbtInstallationHistory.CreateBy = sParam.do_TbtInstallationBasic.CreateBy;
                    do_TbtInstallationHistory.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    do_TbtInstallationHistory.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    List<tbt_InstallationHistory> InsertedTbt_InstallationHistory = handler.InsertTbt_InstallationHistory(do_TbtInstallationHistory);
                    #endregion //=================================================================


                    string strCurrentSlipStatus = SlipStatus.C_SLIP_STATUS_NO_NEED_TO_RETURN;

                    if (sParam.do_TbtInstallationSlip.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                    {
                        strCurrentSlipStatus = SlipStatus.C_SLIP_STATUS_NO_NEED_TO_RETURN;
                    }
                    else
                    {
                        var lstInstrumentCheckList = (
                            from d in sParam.do_TbtInstallationSlipDetails
                            where d.IsParent == true
                            select new
                            {
                                d.InstrumentCode,
                                d.AddRemovedQty,
                                d.ReturnQty,
                                d.IsUnremovable,
                                d.UnremovableQty,
                                Childs = (
                                    from d2 in sParam.do_TbtInstallationSlipDetails
                                    where d2.ParentCode == d.InstrumentCode
                                    && d2.IsParent == false
                                    select d2
                                ).ToList()
                            }
                        );

                        foreach (var slipDetail in lstInstrumentCheckList)
                        {
                            if (slipDetail.AddRemovedQty > 0)
                            {
                                // Every detail must input unremoveableqty equals to addremoveqty to be qualified for no-need-to-return
                                if (slipDetail.IsUnremovable
                                    && slipDetail.Childs.Count == 0
                                    && slipDetail.AddRemovedQty == (slipDetail.UnremovableQty ?? 0)
                                )
                                {
                                    strCurrentSlipStatus = SlipStatus.C_SLIP_STATUS_NO_NEED_TO_RETURN;
                                }
                                // Every childs detail must input unremoveableqty equals to parent's addremoveqty to be qualified for no-need-to-return
                                else if (slipDetail.Childs.Count > 0
                                    && slipDetail.Childs.All(d => d.IsUnremovable && slipDetail.AddRemovedQty == (d.UnremovableQty ?? 0))
                                )
                                {
                                    strCurrentSlipStatus = SlipStatus.C_SLIP_STATUS_NO_NEED_TO_RETURN;
                                }
                                else
                                {
                                    strCurrentSlipStatus = SlipStatus.C_SLIP_STATUS_WAIT_FOR_RETURN;
                                    break;
                                }
                            }

                            if (slipDetail.ReturnQty > 0)
                            {
                                strCurrentSlipStatus = SlipStatus.C_SLIP_STATUS_WAIT_FOR_RETURN;
                                break;
                            }
                        }
                    }

                    #region //============== update old SLIP =====================
                    if (sParam.do_TbtInstallationSlip != null)
                    {
                        sParam.do_TbtInstallationSlip.SlipStatus = strCurrentSlipStatus;
                        sParam.do_TbtInstallationSlip.ChangeContents = doIS.ChangeContents;
                        sParam.do_TbtInstallationSlip.AdditionalStockOutOfficeCode = doIS.AdditionalStockOutOfficeCode;
                        sParam.do_TbtInstallationSlip.UnremoveApproveNo = doIS.UnremoveApproveNo;

                        if (sParam.do_TbtInstallationSlip.NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                        {
                            sParam.do_TbtInstallationSlip.NormalContractFee = sParam.do_TbtInstallationBasic.NormalContractFee;
                            sParam.do_TbtInstallationSlip.NormalContractFeeUsd = null;
                            sParam.do_TbtInstallationSlip.NormalContractFeeCurrencyType = sParam.do_TbtInstallationBasic.NormalContractFeeCurrencyType;
                        }
                        if (sParam.do_TbtInstallationSlip.NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            sParam.do_TbtInstallationSlip.NormalContractFee = null;
                            sParam.do_TbtInstallationSlip.NormalContractFeeUsd = sParam.do_TbtInstallationBasic.NormalContractFeeUsd;
                            sParam.do_TbtInstallationSlip.NormalContractFeeCurrencyType = sParam.do_TbtInstallationBasic.NormalContractFeeCurrencyType;
                        }

                        if (sParam.do_TbtInstallationSlip.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                        {
                            sParam.do_TbtInstallationSlip.NormalInstallFee = sParam.do_TbtInstallationBasic.NormalInstallFee;
                            sParam.do_TbtInstallationSlip.NormalInstallFeeUsd = null;
                            sParam.do_TbtInstallationSlip.NormalInstallFeeCurrencyType = sParam.do_TbtInstallationBasic.NormalInstallFeeCurrencyType;
                        }
                        if (sParam.do_TbtInstallationSlip.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            sParam.do_TbtInstallationSlip.NormalInstallFee = null; ;
                            sParam.do_TbtInstallationSlip.NormalInstallFeeUsd = sParam.do_TbtInstallationBasic.NormalInstallFeeUsd;
                            sParam.do_TbtInstallationSlip.NormalInstallFeeCurrencyType = sParam.do_TbtInstallationBasic.NormalInstallFeeCurrencyType;
                        }

                        if (sParam.do_TbtInstallationSlip.BillingInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                        {
                            sParam.do_TbtInstallationSlip.BillingInstallFee = sParam.do_TbtInstallationBasic.BillingInstallFee;
                            sParam.do_TbtInstallationSlip.BillingInstallFeeUsd = null;
                            sParam.do_TbtInstallationSlip.BillingInstallFeeCurrencyType = sParam.do_TbtInstallationBasic.BillingInstallFeeCurrencyType;
                        }
                        if (sParam.do_TbtInstallationSlip.BillingInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            sParam.do_TbtInstallationSlip.BillingInstallFee = null;
                            sParam.do_TbtInstallationSlip.BillingInstallFeeUsd = sParam.do_TbtInstallationBasic.BillingInstallFeeUsd;
                            sParam.do_TbtInstallationSlip.BillingInstallFeeCurrencyType = sParam.do_TbtInstallationBasic.BillingInstallFeeCurrencyType;
                        }

                        //sParam.do_TbtInstallationSlip.NormalInstallFee = sParam.do_TbtInstallationBasic.NormalInstallFee;
                        sParam.do_TbtInstallationSlip.InstallFeeBillingType = sParam.do_TbtInstallationBasic.InstallFeeBillingType;
                        //sParam.do_TbtInstallationSlip.BillingInstallFee = sParam.do_TbtInstallationBasic.BillingInstallFee;

                        //Add by Jutarat A. on 11042013
                        if (sParam.IsUseContractData)
                        {
                            if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                            {
                                if (sParam.do_TbtRentalSecurityBasicLastUnimp != null && sParam.do_TbtRentalSecurityBasicLastUnimp.Count > 0)
                                    sParam.do_TbtInstallationSlip.OrderInstallFee = sParam.do_TbtRentalSecurityBasicLastUnimp[0].OrderInstallFee;
                            }
                            else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                            {
                                if (sParam.dtSale != null)
                                    sParam.do_TbtInstallationSlip.OrderInstallFee = sParam.dtSale.OrderInstallFee;
                            }
                        }
                        //End Add

                        sParam.do_TbtInstallationSlip.BillingOCC = sParam.do_TbtInstallationBasic.BillingOCC;
                        sParam.do_TbtInstallationSlip.ApproveNo1 = sParam.do_TbtInstallationBasic.ApproveNo1;
                        sParam.do_TbtInstallationSlip.ApproveNo2 = sParam.do_TbtInstallationBasic.ApproveNo2;

                        handler.UpdateTbt_InstallationSlip(sParam.do_TbtInstallationSlip);
                    }
                    #endregion ===================================================
                    #region //============== Update SLIP Details ==========================
                    List<tbt_InstallationSlipDetails> OldSlipDetails = handler.GetTbt_InstallationSlipDetailsData(sParam.do_TbtInstallationBasic.SlipNo, null, InstrumentType.C_INST_TYPE_GENERAL);
                    foreach (tbt_InstallationSlipDetails slipDetail in sParam.do_TbtInstallationSlipDetails)
                    {
                        int IndexInstrument = -1;
                        if (OldSlipDetails != null)
                        {
                            IndexInstrument = OldSlipDetails.FindIndex(delegate(tbt_InstallationSlipDetails s) { return s.InstrumentCode == slipDetail.InstrumentCode; });
                        }

                        if (IndexInstrument >= 0)
                        {
                            if (slipDetail.IsParent == true)
                            {
                                #region  =========== UpdateTbt_InstallationSlipDetails ================
                                OldSlipDetails[IndexInstrument].AddRemovedQty = slipDetail.AddRemovedQty;
                                OldSlipDetails[IndexInstrument].NotInstalledQty = slipDetail.ReturnQty;
                                OldSlipDetails[IndexInstrument].MoveQty = slipDetail.MoveQty;
                                OldSlipDetails[IndexInstrument].MAExchangeQty = slipDetail.MAExchangeQty;
                                OldSlipDetails[IndexInstrument].ReturnQty = 0;
                                //OldSlipDetails[IndexInstrument].UnremovableQty = slipDetail.UnremovableQty;
                                OldSlipDetails[IndexInstrument].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                OldSlipDetails[IndexInstrument].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                handler.UpdateTbt_InstallationSlipDetails(OldSlipDetails[IndexInstrument]);
                                #endregion ==============================================================

                                //Add by Jutarat A. on 12032014 (Move)
                                #region //======================== INSERT INSTALLATION HISTORY DETAIL ===========================
                                tbt_InstallationHistoryDetails do_TbtInstallationHistoryDetail = new tbt_InstallationHistoryDetails();
                                do_TbtInstallationHistoryDetail.ContractCode = sParam.do_TbtInstallationBasic.ContractProjectCode;
                                do_TbtInstallationHistoryDetail.InstrumentCode = OldSlipDetails[IndexInstrument].InstrumentCode;
                                do_TbtInstallationHistoryDetail.InstrumentTypeCode = OldSlipDetails[IndexInstrument].InstrumentTypeCode;
                                do_TbtInstallationHistoryDetail.ContractInstalledQty = OldSlipDetails[IndexInstrument].ContractInstalledQty;
                                do_TbtInstallationHistoryDetail.ContractRemovedQty = OldSlipDetails[IndexInstrument].AddRemovedQty;
                                do_TbtInstallationHistoryDetail.ContractMovedQty = OldSlipDetails[IndexInstrument].MoveQty;
                                do_TbtInstallationHistoryDetail.CreateDate = OldSlipDetails[IndexInstrument].CreateDate;
                                do_TbtInstallationHistoryDetail.CreateBy = OldSlipDetails[IndexInstrument].CreateBy;
                                do_TbtInstallationHistoryDetail.UpdateDate = OldSlipDetails[IndexInstrument].UpdateDate;
                                do_TbtInstallationHistoryDetail.UpdateBy = OldSlipDetails[IndexInstrument].UpdateBy;
                                if (InsertedTbt_InstallationHistory.Count > 0)
                                {
                                    do_TbtInstallationHistoryDetail.HistoryNo = InsertedTbt_InstallationHistory[0].HistoryNo;
                                }
                                handler.InsertTbt_InstallationHistoryDetail(do_TbtInstallationHistoryDetail);
                                #endregion //=======================================================================================
                                //End Add
                            }
                            //Comment by Jutarat A. on 12032014 (Move)
                            //#region //======================== INSERT INSTALLATION HISTORY DETAIL ===========================
                            //tbt_InstallationHistoryDetails do_TbtInstallationHistoryDetail = new tbt_InstallationHistoryDetails();
                            //do_TbtInstallationHistoryDetail.ContractCode = sParam.do_TbtInstallationBasic.ContractProjectCode;
                            //do_TbtInstallationHistoryDetail.InstrumentCode = OldSlipDetails[IndexInstrument].InstrumentCode;
                            //do_TbtInstallationHistoryDetail.InstrumentTypeCode = OldSlipDetails[IndexInstrument].InstrumentTypeCode;
                            //do_TbtInstallationHistoryDetail.ContractInstalledQty = OldSlipDetails[IndexInstrument].ContractInstalledQty;
                            //do_TbtInstallationHistoryDetail.ContractRemovedQty = OldSlipDetails[IndexInstrument].AddRemovedQty;
                            //do_TbtInstallationHistoryDetail.ContractMovedQty = OldSlipDetails[IndexInstrument].MoveQty;
                            //do_TbtInstallationHistoryDetail.CreateDate = OldSlipDetails[IndexInstrument].CreateDate;
                            //do_TbtInstallationHistoryDetail.CreateBy = OldSlipDetails[IndexInstrument].CreateBy;
                            //do_TbtInstallationHistoryDetail.UpdateDate = OldSlipDetails[IndexInstrument].UpdateDate;
                            //do_TbtInstallationHistoryDetail.UpdateBy = OldSlipDetails[IndexInstrument].UpdateBy;
                            //if (InsertedTbt_InstallationHistory.Count > 0)
                            //{
                            //    do_TbtInstallationHistoryDetail.HistoryNo = InsertedTbt_InstallationHistory[0].HistoryNo;
                            //}
                            //handler.InsertTbt_InstallationHistoryDetail(do_TbtInstallationHistoryDetail);
                            //#endregion //=======================================================================================
                            //End Comment
                        }

                        if (slipDetail.IsUnremovable == true && slipDetail.UnremovableQty > 0)
                        {
                            #region =========== insert tbt_installationSlipExpansion =================
                            tbt_InstallationSlipExpansion dtSlipExpansion = new tbt_InstallationSlipExpansion();
                            dtSlipExpansion.SlipNo = sParam.do_TbtInstallationBasic.SlipNo;
                            dtSlipExpansion.InstrumentCode = slipDetail.ParentCode;
                            dtSlipExpansion.ChildInstrumentCode = slipDetail.InstrumentCode;
                            dtSlipExpansion.UnremovableQty = slipDetail.UnremovableQty;
                            handler.InsertTbt_InstallationSlipExpansion(dtSlipExpansion);
                            #endregion ====================================================================
                        }

                    }
                    #endregion //=============================================================
                    #region /////////////////// INSERT MEMO ////////////////////////
                    if (!CommonUtil.IsNullOrEmpty(Memo))
                    {
                        tbt_InstallationMemo doMemo = new tbt_InstallationMemo();
                        doMemo.ContractProjectCode = sParam.ContractCodeLong;
                        doMemo.ReferenceID = sParam.do_TbtInstallationBasic.SlipNo;
                        doMemo.ObjectID = ScreenID.C_SCREEN_ID_INSTALL_COMPLETE;
                        doMemo.Memo = Memo;
                        doMemo.OfficeCode = CommonUtil.dsTransData.dtUserData.MainOfficeCode;
                        doMemo.DepartmentCode = CommonUtil.dsTransData.dtUserData.MainDepartmentCode;
                        doMemo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doMemo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        handler.InsertTbt_InstallationMemo(doMemo);
                    }
                    #endregion /////////////////////////////////////////////////////////


                    ///////////////// UPDATE DATA INVENTORY MODULE ///////////////
                    IInventoryHandler inventHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                    {
                        bool blnProcessResult = inventHand.UpdateCompleteInstallation(sParam.do_TbtInstallationBasic.SlipNo, sParam.do_TbtInstallationBasic.ContractProjectCode);
                    }
                    //============= Teerapong S. 17/08/2012 =====================
                    //else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE && sParam.dtSale.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE)
                    //{
                    //    bool blnProcessResult = inventHand.UpdateCustomerAcceptance(sParam.do_TbtInstallationBasic.ContractProjectCode, sParam.do_TbtInstallationBasic.OCC, (DateTime)sParam.dtSale.CustAcceptanceDate);                   
                    //}
                    else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    {
                        if (sParam.dtSale.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE)
                        {
                            bool blnProcessResult = inventHand.UpdateCustomerAcceptance(sParam.do_TbtInstallationBasic.ContractProjectCode, sParam.do_TbtInstallationBasic.OCC, sParam.dtSale.CustAcceptanceDate);
                        }
                        if (sParam.do_TbtInstallationBasic != null && sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                        {
                            inventHand.UpdateCompleteSaleMA(sParam.do_TbtInstallationBasic.SlipNo, sParam.do_TbtInstallationBasic.ContractProjectCode);
                        }

                        //Add by Jutarat A. on 10042013
                        if (sParam.dtSale.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_SHIP)
                        {
                            if (sParam.do_TbtInstallationBasic != null
                                && (sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD
                                    || sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW))
                            {
                                bool blnProcessResult = inventHand.UpdateReturnSaleInstrument(sParam.do_TbtInstallationBasic.ContractProjectCode, sParam.do_TbtInstallationBasic.OCC, sParam.dtSale.CustAcceptanceDate);
                            }
                        }
                        //End Add
                    }
                    //===========================================================

                    //////////////////////////////////////////////////////////////
                    #region ==================== Complete Installation =======================
                    //======= Update installation information and generating new contract information ==========
                    doCompleteInstallationData doComplete = new doCompleteInstallationData();
                    IContractHandler contractHand = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                    List<tbt_InstallationPOManagement> ListPOInfo = handler.GetTbt_InstallationPOManagementData(sParam.do_TbtInstallationBasic.MaintenanceNo);

                    doComplete.ContractCode = sParam.do_TbtInstallationBasic.ContractProjectCode;
                    if (sParam.dtRentalContractBasic != null)
                    {
                        doComplete.OCC = sParam.dtRentalContractBasic.OCC;
                    }
                    else
                    {
                        doComplete.OCC = sParam.dtSale.OCC;
                    }
                    doComplete.ServiceTypeCode = sParam.do_TbtInstallationBasic.ServiceTypeCode;
                    doComplete.InstallationType = sParam.do_TbtInstallationBasic.InstallationType;
                    doComplete.NormalInstallationFee = sParam.do_TbtInstallationBasic.NormalInstallFee;
                    //=============== Teerapong S. 5/10/2012 CT-266 ==============================
                    //doComplete.BillingInstallationFee = sParam.do_TbtInstallationBasic.BillingInstallFee;
                    if (sParam.do_TbtInstallationBasic.BillingInstallFee == null && sParam.do_TbtInstallationBasic.NormalInstallFee != null && sParam.do_TbtInstallationBasic.NormalInstallFee > 0 && sParam.do_TbtInstallationBasic.InstallFeeBillingType == InstallFeeBillingType.C_INSTALL_FEE_BILLING_TYPE_PAY_ALL_AMOUNT)
                    {
                        doComplete.BillingInstallationFee = 0;
                    }
                    else
                    {
                        doComplete.BillingInstallationFee = sParam.do_TbtInstallationBasic.BillingInstallFee;
                    }
                    //============================================================================

                    doComplete.BillingOCC = sParam.do_TbtInstallationBasic.BillingOCC;
                    doComplete.InstallationSlipNo = sParam.do_TbtInstallationBasic.SlipNo;
                    if (!CommonUtil.IsNullOrEmpty(sParam.do_TbtInstallationBasic.InstallationCompleteDate))
                        doComplete.InstallationCompleteDate = (DateTime)sParam.do_TbtInstallationBasic.InstallationCompleteDate;
                    if (!CommonUtil.IsNullOrEmpty(sParam.do_TbtInstallationBasic.InstallationCompleteProcessingDate))
                        doComplete.InstallationCompleteProcessDate = (DateTime)sParam.do_TbtInstallationBasic.InstallationCompleteProcessingDate;
                    doComplete.InstallationMemo = sParam.do_TbtInstallationSlip.ChangeContents;
                    doComplete.IEInchargeEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doComplete.CompleteInstallationProcessFlag = FlagType.C_FLAG_ON;

                    doSubcontractorDetails SubcontractorDetail;
                    if (ListPOInfo != null)
                    {
                        doComplete.doSubcontractorDetailsList = new List<doSubcontractorDetails>();
                        foreach (tbt_InstallationPOManagement POInfo in ListPOInfo)
                        {
                            SubcontractorDetail = new doSubcontractorDetails();
                            SubcontractorDetail.SubcontractorCode = POInfo.SubcontractorCode;

                            doComplete.doSubcontractorDetailsList.Add(SubcontractorDetail);
                        }
                    }
                    doInstrumentDetails InstrumentDetail;
                    doComplete.doInstrumentDetailsList = new List<doInstrumentDetails>();

                    if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL
                            && sParam.do_TbtInstallationBasic != null
                            && sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                    {
                        List<tbt_RentalInstrumentDetails> doRentalInstrumentDetailsList = rentalHandler.GetTbt_RentalInstrumentDetails(sParam.ContractCodeLong, strLastOCC);

                        foreach (tbt_RentalInstrumentDetails instruDetail in doRentalInstrumentDetailsList)
                        {
                            InstrumentDetail = new doInstrumentDetails();
                            InstrumentDetail.InstrumentCode = instruDetail.InstrumentCode;
                            InstrumentDetail.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                            InstrumentDetail.InstrumentQty = instruDetail.InstrumentQty ?? 0;
                            InstrumentDetail.AddQty = instruDetail.AdditionalInstrumentQty ?? 0;
                            InstrumentDetail.RemoveQty = instruDetail.RemovalInstrumentQty ?? 0;

                            doComplete.doInstrumentDetailsList.Add(InstrumentDetail);
                        }
                    }
                    else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE
                        && sParam.do_TbtInstallationBasic != null
                        && sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                    {
                        //Do Nothing
                    }
                    else
                    {
                        if (sParam.do_TbtInstallationSlipDetails != null)
                        {
                            foreach (tbt_InstallationSlipDetails slipDetail in sParam.do_TbtInstallationSlipDetails)
                            {
                                if (slipDetail.IsParent)
                                {
                                    InstrumentDetail = new doInstrumentDetails();
                                    InstrumentDetail.InstrumentCode = slipDetail.InstrumentCode;
                                    InstrumentDetail.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                                    InstrumentDetail.InstrumentQty = (int)slipDetail.ContractInstalledQty;
                                    InstrumentDetail.AddQty = (int)(slipDetail.TotalStockOutQty - slipDetail.ReturnQty - slipDetail.NotInstalledQty);
                                    InstrumentDetail.RemoveQty = (int)slipDetail.AddRemovedQty;

                                    doComplete.doInstrumentDetailsList.Add(InstrumentDetail);
                                }
                            }
                        }
                        //=========================== TRS 31/05/2012 ==============================
                        List<tbt_InstallationSlipDetails> NewTemp_TbtInstallationSlipDetails = new List<tbt_InstallationSlipDetails>();
                        NewTemp_TbtInstallationSlipDetails = handler.GetTbt_InstallationSlipDetailsData(sParam.do_TbtInstallationBasic.SlipNo, null, InstrumentType.C_INST_TYPE_MONITOR);
                        if (NewTemp_TbtInstallationSlipDetails != null && NewTemp_TbtInstallationSlipDetails.Count > 0)
                        {
                            foreach (tbt_InstallationSlipDetails slipDetail in NewTemp_TbtInstallationSlipDetails)
                            {
                                InstrumentDetail = new doInstrumentDetails();

                                InstrumentDetail.InstrumentCode = slipDetail.InstrumentCode;
                                InstrumentDetail.InstrumentTypeCode = slipDetail.InstrumentTypeCode;
                                InstrumentDetail.InstrumentQty = 0;
                                InstrumentDetail.AddQty = (int)(slipDetail.AddInstalledQty);
                                InstrumentDetail.RemoveQty = 0;

                                doComplete.doInstrumentDetailsList.Add(InstrumentDetail);
                            }
                        }
                        //=========================================================================
                    }

                    contractHand.CompleteInstallation(doComplete);
                    #endregion ==============================================================
                    IQuotationHandler quotationHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    bool blnLockQuotationResult = quotationHandler.LockQuotation(sParam.ContractCodeLong, null, LockStyle.C_LOCK_STYLE_ALL);
                    //===========================================================================================



                    #region //================= Update Attach ======================
                    List<tbt_AttachFile> tmpFileList = chandler.GetTbt_AttachFile(GetCurrentKey(), null, false);
                    if (tmpFileList.Count > 0)
                    {
                        string relatedId = sParam.do_TbtInstallationSlip.SlipNo + "-COMPLETE";
                        chandler.UpdateFlagAttachFile(AttachmentModule.Installation, GetCurrentKey(), relatedId);
                        List<tbt_AttachFile> fileList = chandler.GetTbt_AttachFile(relatedId, null, true);
                        foreach (tbt_AttachFile file in fileList)
                        {
                            tbt_InstallationAttachFile doTbt_InstallAttachFile = new tbt_InstallationAttachFile();
                            doTbt_InstallAttachFile.MaintenanceNo = relatedId;
                            doTbt_InstallAttachFile.AttachFileID = file.AttachFileID;
                            doTbt_InstallAttachFile.ObjectID = ScreenID.C_SCREEN_ID_INSTALL_COMPLETE;
                            handler.InsertTbt_InstallationAttachFile(doTbt_InstallAttachFile);
                        }
                    }
                    #endregion //======================================================

                    inventHand.DeleteTbt_InventoryBookingDetailWithLog(sParam.ContractCodeLong, null);
                    inventHand.DeleteTbt_InventoryBookingWithLog(sParam.ContractCodeLong);

                    scope.Complete();
                    res.ResultData = sParam.do_TbtInstallationBasic;
                }


                //ISS060_RegisterStartResumeTargetData result = new ISS060_RegisterStartResumeTargetData();
                //result.SlipNo = StrSlipNo;
                //res.ResultData = result;

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
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ObjectResultData ISS060_ValidateContractError(ISS060_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (sParam.do_TbtInstallationSlip == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5071);
                    return res;
                }
                //if (sParam.do_TbtInstallationSlip.SlipStatus != SlipStatus.C_SLIP_STATUS_STOCK_OUT && sParam.do_TbtInstallationSlip.SlipStatus != SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5039);
                //    return res;
                //}

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }
        /// <summary>
        /// Validate before register complete installation data
        /// </summary>
        /// <param name="doValidate"></param>
        /// <param name="ScreenInput"></param>
        /// <param name="blnReadOnlyBillingInstallFee"></param>
        /// <returns></returns>
        public ActionResult ISS060_ValidateBeforeRegister(ISS060_ValidateData doValidate, ISS060_ScreenInput ScreenInput, bool blnReadOnlyBillingInstallFee)
        {

            ObjectResultData res = new ObjectResultData();

            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();

            try
            {
                #region ================== Check suspend and permission ====================
                ISS060_ScreenParameter sParam = GetScreenObject<ISS060_ScreenParameter>();
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_COMPLETE, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                #endregion ==================================================================
                #region ================ Check require field ==============================
                if (ScreenInput.chkHaveUnremove == true && CommonUtil.IsNullOrEmpty(ScreenInput.UnremoveApproveNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "UnremoveApproveNo",
                                                    "lblApproveNo",
                                                    "UnremoveApproveNo");
                }
                if (CommonUtil.IsNullOrEmpty(ScreenInput.InstallStartDate))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "InstallStartDate",
                                                    "lblInstallationStartDate",
                                                    "InstallStartDate");
                }
                if (CommonUtil.IsNullOrEmpty(ScreenInput.InstallFinishDate))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "InstallFinishDate",
                                                    "lblInstallationFinishDate",
                                                    "InstallFinishDate");
                }
                if (CommonUtil.IsNullOrEmpty(ScreenInput.InstallCompleteDate))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "InstallCompleteDate",
                                                    "lblInstallationCompleteDate",
                                                    "InstallCompleteDate");
                }
                if (CommonUtil.IsNullOrEmpty(ScreenInput.NewNormalInstallFee))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "NewNormalInstallFee",
                                                    "lblNormalInstallationFee",
                                                    "NewNormalInstallFee");
                }
                if (
                    (sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                    sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE ||
                    sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING ||
                    sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL ||
                    sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL ||
                    sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE ||
                    sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                    sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MOVE ||
                    sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_CHANGE_WIRING ||
                    sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_PARTIAL_REMOVE ||
                    sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL)
                    && sParam.do_TbtInstallationSlip.InstallFeeBillingType != InstallFeeBillingType.C_INSTALL_FEE_BILLING_TYPE_PAY_ALL_AMOUNT
                    && (ScreenInput.NewBillingInstallFee ?? 0) > 0
                    )
                {
                    if (CommonUtil.IsNullOrEmpty(ScreenInput.NewBillingOCC))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        "ISS060",
                                                        MessageUtil.MODULE_COMMON,
                                                        MessageUtil.MessageList.MSG0007,
                                                        "NewBillingOCC",
                                                        "lblBillingOCC",
                                                        "NewBillingOCC");
                    }
                }
                #endregion ==================================================================
                if (res.IsError)
                {
                    res.ResultData = false;
                    //return Json(res);
                }

                #region //==================== Get value from screen ============================
                sParam.do_TbtInstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_COMPLETED;
                decimal amountNormalInstallFee;

                sParam.do_TbtInstallationBasic.NormalInstallFeeCurrencyType = ScreenInput.NormalInstallFeeCurrencyType;
                if (ScreenInput.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    sParam.do_TbtInstallationBasic.NormalInstallFeeUsd = ScreenInput.NormalInstallFeeUsd;
                    sParam.do_TbtInstallationBasic.NormalInstallFee = null;
                }
                else
                {
                    sParam.do_TbtInstallationBasic.NormalInstallFee = ScreenInput.NormalInstallFee;
                    sParam.do_TbtInstallationBasic.NormalInstallFeeUsd = null;
                }

                bool blnConvert = Decimal.TryParse(ScreenInput.NewNormalInstallFee, out amountNormalInstallFee);
                //if (blnConvert)
                //{
                //    sParam.do_TbtInstallationBasic.NormalInstallFee = amountNormalInstallFee;
                //}

                sParam.do_TbtInstallationBasic.BillingInstallFee = ScreenInput.NewBillingInstallFee;
                sParam.do_TbtInstallationBasic.InstallFeeBillingType = ScreenInput.NewInstallFeeBillingType;
                DateTime dteInstallCompleteDate;
                blnConvert = DateTime.TryParse(ScreenInput.InstallCompleteDate, out dteInstallCompleteDate);
                if (blnConvert)
                {
                    sParam.do_TbtInstallationBasic.InstallationCompleteDate = dteInstallCompleteDate;
                }
                sParam.do_TbtInstallationBasic.InstallationCompleteProcessingDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                sParam.do_TbtInstallationBasic.ApproveNo1 = ScreenInput.NewApproveNo1;
                sParam.do_TbtInstallationBasic.ApproveNo2 = ScreenInput.NewApproveNo2;

                DateTime dteInstallStartDate;
                blnConvert = DateTime.TryParse(ScreenInput.InstallStartDate, out dteInstallStartDate);
                if (blnConvert)
                {
                    sParam.do_TbtInstallationBasic.InstallationStartDate = dteInstallStartDate;
                }
                DateTime dteInstallFinishDate;
                blnConvert = DateTime.TryParse(ScreenInput.InstallFinishDate, out dteInstallFinishDate);
                if (blnConvert)
                {
                    sParam.do_TbtInstallationBasic.InstallationFinishDate = dteInstallFinishDate;
                }

                sParam.do_TbtInstallationBasic.NormalContractFeeCurrencyType = ScreenInput.NormalContractFeeCurrencyType;
                if (ScreenInput.NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    sParam.do_TbtInstallationBasic.NormalContractFeeUsd = ScreenInput.NormalContractFeeUsd;
                    sParam.do_TbtInstallationBasic.NormalContractFee = null;
                }    
                else
                {
                    sParam.do_TbtInstallationBasic.NormalContractFee = ScreenInput.NormalContractFee;
                    sParam.do_TbtInstallationBasic.NormalContractFeeUsd = null;
                }

                //sParam.do_TbtInstallationBasic.NormalContractFee = ScreenInput.NewNormalContractFee;
                sParam.do_TbtInstallationBasic.BillingOCC = ScreenInput.NewBillingOCC;
                UpdateScreenObject(sParam);
                #endregion //=======================================================================

                //ValidatorUtil.BuildErrorMessage(res, this);


                ValidatorUtil.BuildErrorMessage(res, validator, null);

                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                #region //////////////////// VALIDATE BUSINESS //////////////////////
                validator = ISS060_ValidateBusiness(sParam, ScreenInput, blnReadOnlyBillingInstallFee);
                ValidatorUtil.BuildErrorMessage(res, validator, null);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                #endregion /////////////////////////////////////////////////////////////

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
        public bool ISS060_ValidExistOffice(string OfficeCode)
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
        /// <param name="ScreenInput"></param>
        /// <param name="blnReadOnlyBillingInstallFee"></param>
        /// <returns></returns>
        public ValidatorUtil ISS060_ValidateBusiness(ISS060_ScreenParameter sParam, ISS060_ScreenInput ScreenInput, bool blnReadOnlyBillingInstallFee)
        {
            ObjectResultData res = new ObjectResultData();
            ValidatorUtil validator = new ValidatorUtil();
            IRentralContractHandler rentalContractHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            try
            {
                ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler; //Add by Jutarat A. on 16092013 (Move)

                //================= Teerapong S. 15/10/2012 =======================
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                List<tbt_InstallationBasic> doValidate_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractCodeLong);

                if ((sParam.TbtInstallationBasicUpdateDate == null)
                    || (sParam.TbtInstallationBasicUpdateDate != null && doValidate_InstallationBasic != null && DateTime.Compare(sParam.TbtInstallationBasicUpdateDate.Value, doValidate_InstallationBasic[0].UpdateDate.Value) != 0)
                    )
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                                      MessageUtil.MessageList.MSG0019,
                                                      "MSG0019", //"", //Modify by Jutarat A. on 13022013
                                                      "", "");
                }
                //=================================================================

                if ((ScreenInput.NewApproveNo2 != null && ScreenInput.NewApproveNo2 != "") && (ScreenInput.NewApproveNo1 == null || ScreenInput.NewApproveNo1 == ""))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5028,
                                                    "MSG5028", //"NewApproveNo1", //Modify by Jutarat A. on 13022013
                                                    "lblApproveNo1",
                                                    "NewApproveNo1");
                }
                if (!blnReadOnlyBillingInstallFee && (sParam.do_TbtInstallationBasic.NormalInstallFee != sParam.do_TbtInstallationBasic.BillingInstallFee && (ScreenInput.NewApproveNo1 == null || ScreenInput.NewApproveNo1 == "")))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5027,
                                                    "MSG5027", //"NewApproveNo1", //Modify by Jutarat A. on 13022013
                                                    "lblApproveNo1",
                                                    "NewApproveNo1");
                }
                //========================= TRS 24/05/2012 NEW SPEC ==========================================
                DateTime dteInstallStartDate;
                bool blnConvert = DateTime.TryParse(ScreenInput.InstallStartDate, out dteInstallStartDate);
                DateTime dteInstallFinishDate;
                blnConvert = DateTime.TryParse(ScreenInput.InstallFinishDate, out dteInstallFinishDate);
                if (dteInstallStartDate > dteInstallFinishDate)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5084,
                                                    "MSG5084_InstallStartDate", //"InstallStartDate", //Modify by Jutarat A. on 13022013
                                                    "InstallStartDate"
                                                    , "InstallStartDate");
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5084,
                                                    "MSG5084_InstallFinishDate", //"InstallFinishDate", //Modify by Jutarat A. on 13022013
                                                     "InstallFinishDate"
                                                    , "InstallFinishDate");
                }
                DateTime dteCompleteDate;
                blnConvert = DateTime.TryParse(ScreenInput.InstallCompleteDate, out dteCompleteDate);
                if (dteInstallFinishDate > dteCompleteDate)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5085,
                                                    "MSG5085_InstallCompleteDate", //"InstallCompleteDate", //Modify by Jutarat A. on 13022013
                                                    ""
                                                    , "InstallCompleteDate");
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5085,
                                                    "MSG5085_InstallFinishDate", //"InstallFinishDate", //Modify by Jutarat A. on 13022013
                                                     ""
                                                    , "InstallFinishDate");
                }

                //============================================================================================

                DateTime pToDay = new DateTime();
                pToDay = DateTime.Today;
                if (sParam.do_TbtInstallationBasic.InstallationCompleteDate > pToDay)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5042,
                                                    "MSG5042", //"InstallCompleteDate", //Modify by Jutarat A. on 13022013
                                                    "lblInstallationCompleteDate",
                                                    "InstallCompleteDate");
                }
                if (sParam.dtRentalContractBasic != null)
                {

                    string strLastOCC = rentalContractHand.GetLastImplementedOCC(sParam.ContractCodeLong);
                    if (!CommonUtil.IsNullOrEmpty(strLastOCC))
                    {
                        List<tbt_RentalSecurityBasic> doImplementSecurityBasic = rentalContractHand.GetTbt_RentalSecurityBasic(sParam.ContractCodeLong, strLastOCC);
                        if (doImplementSecurityBasic != null && doImplementSecurityBasic.Count > 0)
                        {
                            if (sParam.do_TbtInstallationBasic.InstallationCompleteDate < doImplementSecurityBasic[0].ChangeImplementDate)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                            "ISS060",
                                                            MessageUtil.MODULE_INSTALLATION,
                                                            MessageUtil.MessageList.MSG5043,
                                                            "MSG5043", //"InstallCompleteDate", //Modify by Jutarat A. on 13022013
                                                            "lblInstallationCompleteDate",
                                                            "InstallCompleteDate");
                            }
                        }
                    }
                    //if (sParam.do_TbtInstallationBasic.InstallationCompleteDate < sParam.dtRentalContractBasic.ChangeImplementDate)
                    //{
                    //    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                    //                                "ISS060",
                    //                                MessageUtil.MODULE_INSTALLATION,
                    //                                MessageUtil.MessageList.MSG5043,
                    //                                "InstallCompleteDate",
                    //                                "lblInstallationCompleteDate",
                    //                                "InstallCompleteDate");
                    //}
                }

                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    if (sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                    {
                        //ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler; //Comment by Jutarat A. on 16092013 (Move)
                        List<tbt_SaleBasic> SaleBasicData = saleHandler.GetTbt_SaleBasic(sParam.ContractCodeLong, OCCType.C_FIRST_SALE_CONTRACT_OCC, null);
                        if (SaleBasicData != null && SaleBasicData[0].WarranteeTo < DateTime.Today && CommonUtil.IsNullOrEmpty(ScreenInput.NewApproveNo1))
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5117,
                                                    "MSG5117", //"NewApproveNo1", //Modify by Jutarat A. on 13022013
                                                    "lblApproveNo1",
                                                    "NewApproveNo1");
                        }
                    }
                }

                //============ Teerapong S. 17/08/2012 ==================
                List<tbt_RelationType> doRelationTypeList = new List<tbt_RelationType>();
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    if (sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL)
                    {
                        doRelationTypeList = rentalContractHand.GetTbt_RelationType(null, null, sParam.ContractCodeLong);
                        if (doRelationTypeList != null && doRelationTypeList.Count > 0)
                        {
                            foreach (tbt_RelationType doRelationType in doRelationTypeList)
                            {
                                if (doRelationType.RelationType == RelationType.C_RELATION_TYPE_SALE)
                                {
                                    List<tbt_RentalContractBasic> doRentalContractBasic = rentalContractHand.GetTbt_RentalContractBasic(doRelationType.ContractCode, null);
                                    if (doRentalContractBasic != null && doRentalContractBasic.Count > 0)
                                    {
                                        if (doRentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_CANCEL
                                            && doRentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_END
                                            && doRentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL
                                            && doRentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_STOPPING)
                                        {
                                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                                "ISS060",
                                                                MessageUtil.MODULE_INSTALLATION,
                                                                MessageUtil.MessageList.MSG5120,
                                                                "MSG5120", //"", //Modify by Jutarat A. on 13022013
                                                                "",
                                                                "");
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                //=======================================================

                //if(sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                //    &&
                //    (
                //        sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL &&
                //        sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START &&
                //        sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT &&
                //        sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_TERMINATED 
                //    ))
                //{
                //    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                //                                    "ISS060",
                //                                    MessageUtil.MODULE_INSTALLATION,
                //                                    MessageUtil.MessageList.MSG5047,
                //                                    "InstallationType",
                //                                    "lblInstallationType",
                //                                    "InstallationType");
                //}

                if ((sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL ||
                    sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                    )
                    &&
                    (
                    //Modify by Jutarat A. on 19122013
                    /*sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP &&
                    sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP &&
                    sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_QTY_DURING_STOP &&
                    sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP &&
                    sParam.dtRentalContractBasic.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_STOP*/

                        sParam.dtRentalContractBasic.ContractStatus != ContractStatus.C_CONTRACT_STATUS_STOPPING
                        && sParam.dtRentalContractBasic.ContractStatus != ContractStatus.C_CONTRACT_STATUS_CANCEL
                        && sParam.dtRentalContractBasic.ContractStatus != ContractStatus.C_CONTRACT_STATUS_END
                        && sParam.dtRentalContractBasic.ContractStatus != ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL
                    //End Modify
                    ))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS060",
                                                    MessageUtil.MODULE_INSTALLATION,
                                                    MessageUtil.MessageList.MSG5048,
                                                    "MSG5048", //"InstallationType", //Modify by Jutarat A. on 13022013
                                                    "lblInstallationType",
                                                    "InstallationType");
                }
                //===================== ValidateCP12 =========================================
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    IRentralContractHandler rentalHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    //IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    string strLastUnimplementOCC = "";
                    strLastUnimplementOCC = rentalHandler.GetLastUnimplementedOCC(sParam.ContractCodeLong);
                    bool blnCheckCP12 = rentalHandler.CheckCP12(sParam.ContractCodeLong, strLastUnimplementOCC);
                    bool blnValidateCP12;
                    if (blnCheckCP12)
                    {
                        int RentalInstrumentQty = 0;
                        List<tbt_RentalInstrumentDetails> rentalInstrumentData = rentalHandler.GetTbt_RentalInstrumentDetails(sParam.ContractCodeLong, strLastUnimplementOCC);
                        //================ Modified 4/5/2012 ==================================
                        //if (rentalInstrumentData.Count > 0)
                        //{
                        //    RentalInstrumentQty = (int)rentalInstrumentData[0].InstrumentQty + (int)rentalInstrumentData[0].AdditionalInstrumentQty - (int)rentalInstrumentData[0].RemovalInstrumentQty; 
                        //}
                        //blnValidateCP12 = true;
                        //if (sParam.GridInstrumentForValid != null)
                        //{
                        //    foreach (ISS060_GridInstrumentData GridValidData in sParam.GridInstrumentForValid)
                        //    {
                        //            if (GridValidData.ContractInstalledAfterChange != RentalInstrumentQty)
                        //            {
                        //                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                        //                                        MessageUtil.MessageList.MSG5046,
                        //                                        "LineInstrumentCode" + GridValidData.InstrumentCode, "",
                        //                                        "LineInstrumentCode" + GridValidData.InstrumentCode);
                        //                blnValidateCP12 = false;
                        //            }                        
                        //    }
                        //}
                        //====================== Get facility for valid TRS 20/06/2012 ============================
                        List<tbt_InstallationSlipDetails> TempFacility = handler.GetTbt_InstallationSlipDetailsData(sParam.do_TbtInstallationBasic.SlipNo, null, InstrumentType.C_INST_TYPE_MONITOR);

                        if (TempFacility != null && TempFacility.Count > 0)
                        {
                            foreach (tbt_InstallationSlipDetails FacilityData in TempFacility)
                            {
                                ISS060_GridInstrumentData TempInstrumentData = CommonUtil.CloneObject<tbt_InstallationSlipDetails, ISS060_GridInstrumentData>(FacilityData);
                                sParam.GridInstrumentForValid.Add(TempInstrumentData);
                            }

                        }
                        //=========================================================================================
                        if (sParam.GridInstrumentForValid != null)
                        {
                            int countParentandFacility = 0;
                            foreach (ISS060_GridInstrumentData data in sParam.GridInstrumentForValid)
                            {
                                if (data.IsParent || data.InstrumentTypeCode == InstrumentType.C_INST_TYPE_MONITOR)
                                {
                                    if (data.IsParent && ((data.ContractInstalledQty ?? 0) == 0 && ((data.TotalStockOutQty ?? 0) - (data.ReturnQty ?? 0) == 0))) //Add by Jutarat A. on 24012013
                                    {
                                        //Do nothing
                                    }
                                    else
                                    {
                                        countParentandFacility = countParentandFacility + 1;
                                    }
                                }
                            }

                            var rentalInstrumentTemp = (from t in rentalInstrumentData
                                                        where ((t.InstrumentQty ?? 0) == 0 && ((t.AdditionalInstrumentQty ?? 0) - (t.RemovalInstrumentQty ?? 0) == 0)) == false
                                                        select t).ToList<tbt_RentalInstrumentDetails>(); //Add by Jutarat A. on 30042013

                            //if (rentalInstrumentData.Count != countParentandFacility)
                            if (rentalInstrumentTemp.Count != countParentandFacility) //Modify by Jutarat A. on 30042013
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                                    MessageUtil.MessageList.MSG5046,
                                                                    "MSG5046", //"", //Modify by Jutarat A. on 13022013
                                                                    "");
                                blnValidateCP12 = false;
                            }
                            else
                            {
                                foreach (ISS060_GridInstrumentData GridValidData in sParam.GridInstrumentForValid)
                                {
                                    if (GridValidData.IsParent || GridValidData.InstrumentTypeCode == InstrumentType.C_INST_TYPE_MONITOR)
                                    {
                                        if (GridValidData.IsParent && ((GridValidData.ContractInstalledQty ?? 0) == 0 && ((GridValidData.TotalStockOutQty ?? 0) - (GridValidData.ReturnQty ?? 0) == 0))) //Add by Jutarat A. on 24012013
                                        {
                                            //Do nothing
                                        }
                                        else
                                        {
                                            List<tbt_RentalInstrumentDetails> InstrumentListTemp = (from t in rentalInstrumentData
                                                                                                    where t.InstrumentCode == GridValidData.InstrumentCode
                                                                                                    select t).ToList<tbt_RentalInstrumentDetails>();
                                            if (InstrumentListTemp == null || InstrumentListTemp.Count <= 0)
                                            {
                                                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                                            MessageUtil.MessageList.MSG5046,
                                                                            "MSG5046", //"", //Modify by Jutarat A. on 13022013
                                                                            "");
                                                blnValidateCP12 = false;
                                                break;
                                            }
                                            else
                                            {
                                                //================== TRS 20/06/2012 ===========================================================================
                                                //================== Case instrument from grid and from facility use difference field to check ================
                                                //int GridQTY = Convert.ToInt32(GridValidData.ContractInstalledQty 
                                                //                            + GridValidData.TotalStockOutQty 
                                                //                            - GridValidData.ReturnQty 
                                                //                            + GridValidData.NotInstalledQty 
                                                //                            - GridValidData.AddRemovedQty 
                                                //                            + GridValidData.AddRemovedQTYTemp);
                                                int GridQTY = 0;
                                                if (GridValidData.InstrumentTypeCode == InstrumentType.C_INST_TYPE_MONITOR)
                                                {
                                                    GridQTY = Convert.ToInt32(GridValidData.AddInstalledQty);
                                                }
                                                else
                                                {
                                                    //GridQTY = Convert.ToInt32(GridValidData.ContractInstalledQty
                                                    //                            + GridValidData.TotalStockOutQty
                                                    //                            - GridValidData.ReturnQty
                                                    //                            + GridValidData.NotInstalledQty
                                                    //                            - GridValidData.AddRemovedQty
                                                    //                            + GridValidData.AddRemovedQTYTemp);
                                                    GridQTY = Convert.ToInt32(GridValidData.ContractInstalledQty
                                                                                + GridValidData.TotalStockOutQty
                                                                                - GridValidData.ReturnQty
                                                        //- GridValidData.NotInstalledQty
                                                                                - GridValidData.AddRemovedQty
                                                        //- GridValidData.AddRemovedQTYTemp
                                                                                );
                                                }
                                                //=============================================================================================================
                                                int RentalQTY = Convert.ToInt32(InstrumentListTemp[0].InstrumentQty
                                                                            + InstrumentListTemp[0].AdditionalInstrumentQty
                                                                            - InstrumentListTemp[0].RemovalInstrumentQty);
                                                if (GridQTY != RentalQTY)
                                                {
                                                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                                            MessageUtil.MessageList.MSG5046,
                                                                            "MSG5046", //"LineInstrumentCode" + GridValidData.InstrumentCode, //Modify by Jutarat A. on 13022013
                                                                            "",
                                                                            "LineInstrumentCode" + GridValidData.InstrumentCode);
                                                    blnValidateCP12 = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        //=====================================================================
                        //=================== TRS 25/06/2012 2.2.3 ============================
                        List<tbt_RentalSecurityBasic> doValidateRental = rentalHandler.GetTbt_RentalSecurityBasic(sParam.ContractCodeLong, strLastUnimplementOCC);
                        if (doValidateRental != null && doValidateRental.Count > 0)
                        {
                            // 20170222 nakajima modify start
                            //if (doValidateRental[0].NormalContractFee != ScreenInput.NewNormalContractFee)
                            if ((doValidateRental[0].NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                                && doValidateRental[0].NormalContractFee != ScreenInput.NormalContractFee)
                                || (doValidateRental[0].NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                                && doValidateRental[0].NormalContractFeeUsd != ScreenInput.NormalContractFee))
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                                        MessageUtil.MessageList.MSG5096,
                                                                        "MSG5096", //"NewNormalContractFee", //Modify by Jutarat A. on 13022013
                                                                        "NewNormalContractFee");
                                blnValidateCP12 = false;
                            }
                            // 20170222 nakajima modify end

                            //decimal amountTmpNormalInstallFee;
                            //bool blnTmpConvert = Decimal.TryParse(ScreenInput.NewNormalInstallFee, out amountTmpNormalInstallFee);
                            //if (blnTmpConvert)
                            //{
                            //    if (doValidateRental[0].NormalInstallFee != amountTmpNormalInstallFee)
                            //    {
                            //        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                            //                                                MessageUtil.MessageList.MSG5097,
                            //                                                "NewNormalInstallFee",
                            //                                                "NewNormalInstallFee");
                            //        blnValidateCP12 = false;
                            //    }
                            //}
                            if (doValidateRental[0].NormalInstallFee != ScreenInput.NormalInstallFee)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                                        MessageUtil.MessageList.MSG5097,
                                                                        "MSG5097", "NewNormalInstallFee", //Modify by Jutarat A. on 13022013
                                                                        "NewNormalInstallFee");
                                blnValidateCP12 = false;
                            }
                        }
                        //=====================================================================
                    }
                    else
                    {
                        blnValidateCP12 = false;
                        if (sParam.do_TbtInstallationSlip.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_CUS)
                        {
                            if (sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                                || sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                                || sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE
                                || sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
                                || sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                                || sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                                || sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                                )
                            {
                                blnValidateCP12 = true;
                            }
                            else if (sParam.do_TbtInstallationBasic.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW
                                && sParam.do_TbtInstallationSlip.PreviousSlipStatus == null)
                            {
                                blnValidateCP12 = true;
                            }
                            else
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                                MessageUtil.MessageList.MSG5045,
                                                                "MSG5045", //"", //Modify by Jutarat A. on 13022013
                                                                "",
                                                                "");
                                blnValidateCP12 = false;
                            }
                        }
                        else if (sParam.do_TbtInstallationSlip.ChangeReasonCode == InstallChangeReason.C_INSTALL_CHANGE_REASON_SECOM)
                        {
                            blnValidateCP12 = true;
                        }
                        else
                        {
                            validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                            MessageUtil.MessageList.MSG5045,
                                                            "MSG5045", //"", //Modify by Jutarat A. on 13022013
                                                            "",
                                                            "");
                            blnValidateCP12 = false;
                        }
                    }
                }
                //============================================================================

                //Add by Jutarat A. on 16092013
                //In case of new sale or add sale, CQ-12 has been registered after registering installation slip
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    if (sParam.do_TbtInstallationBasic != null)
                    {
                        if (sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW
                            || sParam.do_TbtInstallationBasic.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD)
                        {
                            //Get last occ of sale contract
                            string strLastOCC = saleHandler.GetLastOCC(sParam.ContractCodeLong);

                            //Get last occ of sale instrument
                            List<tbt_SaleInstrumentDetails> doSaleInstrumentDetails = saleHandler.GetTbt_SaleInstrumentDetails(sParam.ContractCodeLong, strLastOCC);

                            if ((doSaleInstrumentDetails != null && doSaleInstrumentDetails.Count > 0) && sParam.do_TbtInstallationSlip != null)
                            {
                                if (doSaleInstrumentDetails[0].UpdateDate > sParam.do_TbtInstallationSlip.CreateDate)
                                {
                                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                        MessageUtil.MessageList.MSG5124,
                                                        "MSG5124",
                                                        "",
                                                        "");
                                }
                            }
                        }
                    }
                }
                //End Add
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return validator;
        }
        /// <summary>
        /// Clear all email data from session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>       
        public ActionResult ISS060_ClearInstallEmail(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS060_ScreenParameter session = GetScreenObject<ISS060_ScreenParameter>();
            session.ListDOEmail = null;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Clear all instrument data from session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS060_ClearInstrumentInfo(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS060_ScreenParameter session = GetScreenObject<ISS060_ScreenParameter>();
            session.do_TbtInstallationSlipDetails = null;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Generate report delivery confirm 
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS060_PrintISR110(string strContractProjectCode)
        {
            IInstallationDocumentHandler handler = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            ISS060_ScreenParameter session = GetScreenObject<ISS060_ScreenParameter>();
            //Stream StreamReport = handler.CreateReportDeliveryConfirmData(session.do_TbtInstallationSlip.SlipNo);
            //return File(StreamReport, "application/pdf", "ISR110.pdf");
            return File(insHand.CreateReportInstallationCompleteConfirmation(session.do_TbtInstallationSlip.SlipNo), "application/pdf");
        }
        /// <summary>
        /// Generate report complete confirm
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS060_PrintISR090(string strContractProjectCode)
        {
            IInstallationDocumentHandler handler = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            ISS060_ScreenParameter session = GetScreenObject<ISS060_ScreenParameter>();
            Stream StreamReport = handler.CreateReportInstallCompleteConfirmData(session.do_TbtInstallationSlip.SlipNo);
            return File(StreamReport, "application/pdf", "ISR090.pdf");
        }
        /// <summary>
        /// Recieve data slip details from screen to session parameter
        /// </summary>
        /// <param name="ScreenParam"></param>
        /// <returns></returns>
        public ActionResult ISS060_SendGridSlipDetailsData(ISS060_ScreenParameter ScreenParam)
        {
            ObjectResultData res = new ObjectResultData();

            ISS060_ScreenParameter session = GetScreenObject<ISS060_ScreenParameter>();
            //====== Get for insert
            session.do_TbtInstallationSlipDetails = ScreenParam.do_TbtInstallationSlipDetails;
            //====== Get for valid data
            session.ListInstrumentData = ScreenParam.ListInstrumentData;
            session.GridInstrumentForValid = ScreenParam.GridInstrumentForValid;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Get instrument data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult ISS060_GetInstrumentDetailInfo(ISS060_GetInstrumentDataCondition cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                #region Get Session

                string ProductTypeCode = null;

                ISS060_ScreenParameter sParam = GetScreenObject<ISS060_ScreenParameter>();
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
        /// Load data attach to show in grid
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS060_LoadGridAttachedDocList()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                List<dtAttachFileForGridView> lstAttachedName = commonhandler.GetAttachFileForGridView(GetCurrentKey());
                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Installation\\ISS060_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.INSERT);
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
        /// <param name="temp"></param>
        /// <returns></returns>
        public ActionResult ISS060_ClearAllAttach(string temp)
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
        /// Remove attach file
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult ISS060_RemoveAttach(string AttachID)
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
        /// Upload attach file
        /// </summary>
        /// <param name="fileSelect"></param>
        /// <param name="DocumentName"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult ISS060_AttachFile(HttpPostedFileBase fileSelect, string DocumentName, string k)
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
                            string nparam = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS300", "lblDocumentName");
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

            return View("ISS060_Upload");
        }
    }
}
