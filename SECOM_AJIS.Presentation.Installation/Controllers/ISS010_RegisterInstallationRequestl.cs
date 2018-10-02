
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
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Contract;
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
        /// Authority screen ISS010
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ISS010_Authority(ISS010_ScreenParameter param)
        {
            // permission

            ObjectResultData res = new ObjectResultData();
            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            #region ================ Check suspend and permission ======================
            if (chandler.IsSystemSuspending())
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_REQUEST, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }
            #endregion
            #region =============== Add new spec 26/04/2012 Get Common Contract COde for Search ==============
            if (String.IsNullOrEmpty(param.strContractProjectCode) && param.CommonSearch != null)
            {
                if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                    param.strContractProjectCode = param.CommonSearch.ContractCode;
                if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ProjectCode) == false)
                    param.strContractProjectCode = param.CommonSearch.ProjectCode;
            }
            #endregion

            param.ContractProjectCodeShort = param.strContractProjectCode;

            #region =============== Validate Business ====================================================
            if (!CommonUtil.IsNullOrEmpty(param.ContractProjectCodeShort))
            {
                CommonUtil comUtil = new CommonUtil();
                string strProjectCodeLong = comUtil.ConvertProjectCode(param.ContractProjectCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
                IProjectHandler ProjectHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                dtProjectForInstall dtProjectDataForInstall = ProjectHandler.GetProjectDataForInstall(strProjectCodeLong);
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                if (dtProjectDataForInstall != null)
                {
                    param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_PROJECT;
                    res = ISS010_ValidateProjectError(strProjectCodeLong);
                }
                else
                {
                    string strContractCodeLong = comUtil.ConvertContractCode(param.ContractProjectCodeShort, CommonUtil.CONVERT_TYPE.TO_LONG);
                    IRentralContractHandler RentalContactHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    dtRentalContractBasicForInstall dtRentalContractBasic = RentalContactHandler.GetRentalContractBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);
                    dtSaleBasic dtSaleContractBasic = new dtSaleBasic();

                    if (dtRentalContractBasic != null)
                    {
                        param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                        res = ISS010_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, param.ServiceTypeCode);
                    }
                    else
                    {
                        ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);

                        if (dtSaleContractBasic != null)
                        {
                            param.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                            res = ISS010_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, param.ServiceTypeCode);
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5063, new string[] { "ContractCode" });
                            return Json(res);
                        }
                    }
                }

                if (res.IsError)
                {
                    res.ResultData = null;
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
            }
            #endregion

            return InitialScreenEnvironment<ISS010_ScreenParameter>("ISS010", param, res);

        }
        /// <summary>
        /// Initial screen ISS010
        /// </summary>
        /// <returns></returns>
        [Initialize("ISS010")]
        public ActionResult ISS010()
        {
            //ISS010_ScreenParameter param = new ISS010_ScreenParameter();
            ISS010_ScreenParameter param = GetScreenObject<ISS010_ScreenParameter>();
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
        public ActionResult ISS010_InitialGridEmail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS010_Email", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        //InitialGridAttachedFile

        /// <summary>
        /// Retrieve data screen ISS010
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public ActionResult ISS010_RetrieveData(string strContractCode)
        {
            IRentralContractHandler rentalContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            dtRentalContractBasicForInstall dtRentalContractBasic = new dtRentalContractBasicForInstall();
            dtSaleBasic dtSaleContractBasic = new dtSaleBasic();
            string lang = CommonUtil.GetCurrentLanguage();
            string ContractProjectCodeForShow = "";
            try
            {

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ISS010_ScreenParameter sParam = GetScreenObject<ISS010_ScreenParameter>();
                sParam.ContractProjectCodeShort = strContractCode;
                if (sParam.ISS010_Session == null)
                {
                    sParam.ISS010_Session = new ISS010_RegisterStartResumeTargetData();
                    sParam.ISS010_Session.dtRentalContractBasic = new dtRentalContractBasicForInstall();
                }

                //Check mandatory
                if (String.IsNullOrEmpty(strContractCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5057, new string[] { "ContractCode" });
                    return Json(res);
                }

                #region ================ Get Project,Contract data =================

                //=============== RETRIEVE PROJECT DATA ==================
                string strProjectCodeLong = comUtil.ConvertProjectCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                IProjectHandler ProjectHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                dtProjectForInstall dtProjectDataForInstall = ProjectHandler.GetProjectDataForInstall(strProjectCodeLong);
                //========================================================
                if (dtProjectDataForInstall != null)
                {
                    sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_PROJECT;
                    sParam.ISS010_Session.dtProject = dtProjectDataForInstall;
                    //sParam.ContractProjectCodeLong = strProjectCodeLong;
                    sParam.ContractProjectCodeLong = dtProjectDataForInstall.ProjectCode;
                    ContractProjectCodeForShow = comUtil.ConvertProjectCode(dtProjectDataForInstall.ProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    sParam.ContractProjectCodeShort = ContractProjectCodeForShow;

                    dtSaleContractBasic = null;
                    dtRentalContractBasic = null;
                    res = ISS010_ValidateProjectError(sParam.ContractProjectCodeLong);
                }
                else
                {
                    string strContractCodeLong = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);


                    //sParam.ContractProjectCodeLong = strContractCodeLong;
                    //dsRentalContract = CheckDataAuthority_ISS010(res, strContractCodeLong);
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
                        sParam.ContractProjectCodeLong = dtRentalContractBasic.ContractCode;
                        sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                        sParam.InstallType = MiscType.C_RENTAL_INSTALL_TYPE;
                        sParam.ISS010_Session.dtRentalContractBasic = dtRentalContractBasic;
                        ContractProjectCodeForShow = comUtil.ConvertContractCode(dtRentalContractBasic.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        sParam.ContractProjectCodeShort = ContractProjectCodeForShow;

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
                        res = ISS010_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, sParam.ServiceTypeCode);
                        CommonUtil.MappingObjectLanguage(dtRentalContractBasic);
                    }
                    else
                    {
                        ISaleContractHandler SaleContactHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        dtSaleContractBasic = SaleContactHandler.GetSaleBasicDataForInstall(strContractCodeLong, MiscType.C_BUILDING_TYPE);

                        if (dtSaleContractBasic != null)
                        {
                            sParam.ContractProjectCodeLong = dtSaleContractBasic.ContractCode;
                            sParam.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                            sParam.InstallType = MiscType.C_SALE_INSTALL_TYPE;
                            sParam.ISS010_Session.dtSale = dtSaleContractBasic;
                            ContractProjectCodeForShow = comUtil.ConvertContractCode(dtSaleContractBasic.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                            sParam.ContractProjectCodeShort = ContractProjectCodeForShow;

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
                            res = ISS010_ValidateContractError(dtRentalContractBasic, dtSaleContractBasic, sParam.ServiceTypeCode);
                            CommonUtil.MappingObjectLanguage(dtSaleContractBasic);
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5063, new string[] { "ContractCode" });
                            return Json(res);
                        }
                    }
                }

                //====================== get for initial installation type ==========================
                string strOCC = rentalContractHandler.GetLastUnimplementedOCC(sParam.ContractProjectCodeLong);
                bool blnCheckCP12 = false;
                if (!CommonUtil.IsNullOrEmpty(strOCC))
                {
                    blnCheckCP12 = rentalContractHandler.CheckCP12(sParam.ContractProjectCodeLong, strOCC);
                }
                tbt_InstallationBasic doTbt_InstallationBasic = null;
                List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractProjectCodeLong);
                if (tmpdoTbt_InstallationBasic != null)
                {
                    doTbt_InstallationBasic = tmpdoTbt_InstallationBasic[0];
                    //=========== Teerapong S. 15/10/2012 ==============
                    sParam.doIB = tmpdoTbt_InstallationBasic[0];
                }
                //===================================================================================

                if (res.IsError)
                    return Json(res);

                #endregion

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
                ISS010_RegisterStartResumeTargetData result = new ISS010_RegisterStartResumeTargetData();
                result.doTbt_InstallationBasic = doTbt_InstallationBasic;
                result.blnCheckCP12 = blnCheckCP12;
                result.dtRentalContractBasic = dtRentalContractBasic;
                result.dtSale = dtSaleContractBasic;
                result.dtProject = dtProjectDataForInstall;
                result.ServiceTypeCode = sParam.ServiceTypeCode;
                result.InstallType = sParam.InstallType;
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
        /// Get installation email data for show
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        public ActionResult ISS010_GetInstallEmail(string strEmail)
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
                    List<ISS010_DOEmailData> listEmail;
                    ISS010_ScreenParameter session = GetScreenObject<ISS010_ScreenParameter>();
                    ISS010_DOEmailData doISS010EmailADD = new ISS010_DOEmailData();

                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS010_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    doISS010EmailADD.EmpNo = emailList[0].EmpNo;
                    doISS010EmailADD.EmailAddress = emailList[0].EmailAddress;

                    if (listEmail.FindAll(delegate(ISS010_DOEmailData s) { return s.EmpNo == doISS010EmailADD.EmpNo; }).Count() == 0)
                        listEmail.Add(doISS010EmailADD);
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0107, new string[] { strEmail }, null);
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
        /// Validate email
        /// </summary>
        /// <param name="doISS010Email"></param>
        /// <returns></returns>
        public ActionResult ValidateEmail_ISS010(ISS010_DOEmailData doISS010Email)
        {
            List<dtGetEmailAddress> dtEmail;
            ISS010_DOEmailData doISS010EmailADD;
            IEmployeeMasterHandler employeeMasterHandler;
            ObjectResultData res = new ObjectResultData();
            ISS010_ScreenParameter session;
            employeeMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            dtEmail = employeeMasterHandler.GetEmailAddress(doISS010Email.EmpFirstNameEN, doISS010Email.EmailAddress, doISS010Email.OfficeCode, doISS010Email.DepartmentCode);

            try
            {
                session = GetScreenObject<ISS010_ScreenParameter>();
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
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, doISS010Email.EmailAddress);
                    else
                    {
                        doISS010EmailADD = new ISS010_DOEmailData();
                        doISS010EmailADD.EmpNo = dtEmail[0].EmpNo;
                        doISS010EmailADD.EmailAddress = doISS010Email.EmailAddress;
                        session.DOEmail = doISS010EmailADD;

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
        public ActionResult ISS010_Upload()
        {
            ViewBag.K = GetCurrentKey();
            return View();
        }
        /// <summary>
        /// Get data installation type combobox
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS010_GetMiscInstallationtype(string strFieldName)
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
        public ActionResult GetEmail_ISS010(ISS010_DOEmailData doCTS053Email)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            ISS010_DOEmailData doISS010EmailADD;
            List<ISS010_DOEmailData> listEmail;
            ISS010_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS010_ScreenParameter>();
                doISS010EmailADD = new ISS010_DOEmailData();
                if (session.ListDOEmail == null)
                    listEmail = new List<ISS010_DOEmailData>();
                else
                    listEmail = session.ListDOEmail;

                doISS010EmailADD.EmpNo = session.DOEmail.EmpNo;
                doISS010EmailADD.EmailAddress = doCTS053Email.EmailAddress;

                if (listEmail.FindAll(delegate(ISS010_DOEmailData s) { return s.EmpNo == doISS010EmailADD.EmpNo; }).Count() == 0)
                    listEmail.Add(doISS010EmailADD);

                session.DOEmail = null;
                session.ListDOEmail = listEmail;
                res.ResultData = CommonUtil.ConvertToXml<ISS010_DOEmailData>(session.ListDOEmail, "Contract\\CTS053Email", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Add list email to session parameter
        /// </summary>
        /// <param name="listEmailAdd"></param>
        /// <returns></returns>
        public ActionResult GetEmailList_ISS010(List<ISS010_DOEmailData> listEmailAdd)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resEmail = new ObjectResultData();
            List<ISS010_DOEmailData> listEmail;
            List<ISS010_DOEmailData> listNewEmail;
            ISS010_ScreenParameter session;

            try
            {
                listNewEmail = new List<ISS010_DOEmailData>();
                if (listEmailAdd != null)
                {
                    session = GetScreenObject<ISS010_ScreenParameter>();
                    if (session.ListDOEmail == null)
                        listEmail = new List<ISS010_DOEmailData>();
                    else
                        listEmail = session.ListDOEmail;

                    foreach (var item in listEmailAdd)
                    {
                        if (listEmail.FindAll(delegate(ISS010_DOEmailData s) { return s.EmailAddress == item.EmailAddress; }).Count() == 0)
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
        /// Remove email and show to grid
        /// </summary>
        /// <param name="doISS010Email"></param>
        /// <returns></returns>
        public ActionResult RemoveMailClick_ISS010(ISS010_DOEmailData doISS010Email)
        {
            List<ISS010_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS010_ScreenParameter session;

            try
            {
                session = GetScreenObject<ISS010_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate(ISS010_DOEmailData s) { return s.EmailAddress == doISS010Email.EmailAddress; });
                if (listEmailDelete.Count() != 0)
                    session.ListDOEmail.Remove(listEmailDelete[0]);

                res.ResultData = CommonUtil.ConvertToXml<ISS010_DOEmailData>(session.ListDOEmail, "Installation\\ISS010Email", CommonUtil.GRID_EMPTY_TYPE.INSERT);
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
        /// <param name="doISS010Email"></param>
        /// <returns></returns>
        public ActionResult ISS010_RemoveMailClick(ISS010_DOEmailData doISS010Email)
        {
            List<ISS010_DOEmailData> listEmailDelete;
            ObjectResultData res = new ObjectResultData();
            ISS010_ScreenParameter session;
            try
            {
                session = GetScreenObject<ISS010_ScreenParameter>();
                listEmailDelete = session.ListDOEmail.FindAll(delegate(ISS010_DOEmailData s) { return s.EmailAddress == doISS010Email.EmailAddress; });
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
        /// Register installation request data
        /// </summary>
        /// <param name="doIB"></param>
        /// <param name="doIM"></param>
        /// <param name="DataValid"></param>
        /// <returns></returns>
        public ActionResult ISS010_RegisterData(tbt_InstallationBasic doIB, tbt_InstallationManagement doIM, ISS010_ValidateData DataValid)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;

            try
            {
                #region //////////////////// VALIDATE SUSPEND ///////////////////////
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_REQUEST, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                #endregion /////////////////////////////////////////////////////////////
                ISS010_ScreenParameter sParam = GetScreenObject<ISS010_ScreenParameter>();

                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                doGenerateInstallationMANo doGenMANo = new doGenerateInstallationMANo();

                #region //////////////////// VALIDATE BUSINESS //////////////////////
                res = ISS010_ValidateBusiness(sParam.ContractProjectCodeLong, sParam.ServiceTypeCode, doIB.InstallationType, DataValid);
                if (res.IsError)
                    return Json(res);
                #endregion /////////////////////////////////////////////////////////////

                #region //////////////////// PREPARE DATA //////////////////////////////
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    doGenMANo.strOfficeCode = sParam.ISS010_Session.dtRentalContractBasic.OperationOfficeCode;
                    doIB.OperationOfficeCode = sParam.ISS010_Session.dtRentalContractBasic.OperationOfficeCode;
                    doIB.OCC = sParam.ISS010_Session.dtRentalContractBasic.OCC;
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    doGenMANo.strOfficeCode = sParam.ISS010_Session.dtSale.OperationOfficeCode;
                    doIB.OperationOfficeCode = sParam.ISS010_Session.dtSale.OperationOfficeCode;
                    doIB.OCC = sParam.ISS010_Session.dtSale.OCC;
                }
                else
                {
                    doGenMANo.strOfficeCode = "0000";
                    //doIB.OperationOfficeCode = "";
                }
                doGenMANo.strServiceTypeCode = sParam.ServiceTypeCode;

                string StrMANo = handler.GenerateInstallationMANo(doGenMANo);

                doIB.ContractProjectCode = sParam.ContractProjectCodeLong;
                doIB.MaintenanceNo = StrMANo;
                doIB.ServiceTypeCode = sParam.ServiceTypeCode;
                doIB.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED;
                doIB.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doIB.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;


                doIM.ContractProjectCode = sParam.ContractProjectCodeLong;
                doIM.MaintenanceNo = StrMANo;
                doIM.ManagementStatus = InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_PROCESSING;
                doIM.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doIM.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;


                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    doIB.SalesmanEmpNo1 = sParam.ISS010_Session.dtRentalContractBasic.SalesmanCode1;
                    doIB.SalesmanEmpNo2 = sParam.ISS010_Session.dtRentalContractBasic.SalesmanCode2;
                    doIB.SecurityTypeCode = sParam.ISS010_Session.dtRentalContractBasic.SecurityTypeCode;
                    doIM.NewBldMgmtFlag = sParam.ISS010_Session.dtRentalContractBasic.NewBldMgmtFlag;
                    //doIM.NewBldMgmtCost = sParam.ISS010_Session.dtRentalContractBasic.NewBldMgmtCost;

                    if(sParam.ISS010_Session.dtRentalContractBasic.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    {
                        doIM.NewBldMgmtCostCurrencyType = sParam.ISS010_Session.dtRentalContractBasic.NewBldMgmtCostCurrencyType;
                        doIM.NewBldMgmtCost = sParam.ISS010_Session.dtRentalContractBasic.NormalContractFee;
                        doIM.NewBldMgmtCostUsd = null;
                    }
                    if (sParam.ISS010_Session.dtRentalContractBasic.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        doIM.NewBldMgmtCostCurrencyType = sParam.ISS010_Session.dtRentalContractBasic.NewBldMgmtCostCurrencyType;
                        doIM.NewBldMgmtCostUsd = sParam.ISS010_Session.dtRentalContractBasic.NormalContractFee;
                        doIM.NewBldMgmtCost = null;
                    }
                }
                else if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    doIB.SalesmanEmpNo1 = sParam.ISS010_Session.dtSale.SalesmanCode1;
                    doIB.SalesmanEmpNo2 = sParam.ISS010_Session.dtSale.SalesmanCode2;
                    doIB.SecurityTypeCode = sParam.ISS010_Session.dtSale.SecurityTypeCode;
                    doIM.NewBldMgmtFlag = sParam.ISS010_Session.dtSale.NewBldMgmtFlag;
                    //doIM.NewBldMgmtCost = sParam.ISS010_Session.dtSale.NewBldMgmtCost;

                    if (sParam.ISS010_Session.dtSale.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    {
                        doIM.NewBldMgmtCostCurrencyType = sParam.ISS010_Session.dtSale.NewBldMgmtCostCurrencyType;
                        doIM.NewBldMgmtCost = sParam.ISS010_Session.dtSale.NewBldMgmtCost;
                        doIM.NewBldMgmtCostUsd = null;
                    }
                    if (sParam.ISS010_Session.dtSale.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        doIM.NewBldMgmtCostCurrencyType = sParam.ISS010_Session.dtSale.NewBldMgmtCostCurrencyType;
                        doIM.NewBldMgmtCostUsd = sParam.ISS010_Session.dtSale.NewBldMgmtCost;
                        doIM.NewBldMgmtCost = null;
                    }
                }
                doIB.InstallationBy = InstallationBy.C_INSTALLATION_BY_SUBCONTRACTOR;
                #endregion ////////////////////////////////////////////////////////////////////////////////////////////
                CommonUtil cmm = new CommonUtil();
                #region ================== Get Tbt_InstallationBasic ====================
                //tbt_InstallationBasic doTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractProjectCodeLong);
                tbt_InstallationBasic doTbt_InstallationBasic = null;
                List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(sParam.ContractProjectCodeLong);
                if (tmpdoTbt_InstallationBasic != null)
                {
                    doTbt_InstallationBasic = new tbt_InstallationBasic();
                    doTbt_InstallationBasic = tmpdoTbt_InstallationBasic[0];
                }
                #endregion
                using (TransactionScope scope = new TransactionScope())
                {
                    if (doTbt_InstallationBasic == null)
                    {
                        handler.InsertTbt_InstallationBasic(doIB);
                    }
                    else
                    {
                        #region ==================== Update tbt_installationbasic ==============================
                        if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL || sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                        {
                            doTbt_InstallationBasic.ContractProjectCode = doIB.ContractProjectCode;
                            doTbt_InstallationBasic.OCC = doIB.OCC;
                            doTbt_InstallationBasic.ServiceTypeCode = doIB.ServiceTypeCode;
                            doTbt_InstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED;
                            doTbt_InstallationBasic.InstallationType = doIB.InstallationType;
                            doTbt_InstallationBasic.PlanCode = doIB.PlanCode;
                            doTbt_InstallationBasic.MaintenanceNo = doIB.MaintenanceNo;
                            doTbt_InstallationBasic.OperationOfficeCode = doIB.OperationOfficeCode;
                            doTbt_InstallationBasic.SecurityTypeCode = doIB.SecurityTypeCode;
                            doTbt_InstallationBasic.InstallationBy = InstallationBy.C_INSTALLATION_BY_SUBCONTRACTOR;
                            doTbt_InstallationBasic.SalesmanEmpNo1 = doIB.SalesmanEmpNo1;
                            doTbt_InstallationBasic.SalesmanEmpNo2 = doIB.SalesmanEmpNo2;

                        }
                        else
                        {
                            doTbt_InstallationBasic.ContractProjectCode = doIB.ContractProjectCode;
                            //doTbt_InstallationBasic.OCC = "";
                            doTbt_InstallationBasic.ServiceTypeCode = doIB.ServiceTypeCode;
                            doTbt_InstallationBasic.InstallationStatus = InstallationStatus.C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED;
                            //doTbt_InstallationBasic.InstallationType = "";
                            doTbt_InstallationBasic.PlanCode = doIB.PlanCode;
                            doTbt_InstallationBasic.MaintenanceNo = doIB.MaintenanceNo;
                            doTbt_InstallationBasic.InstallationBy = InstallationBy.C_INSTALLATION_BY_SUBCONTRACTOR;
                            doTbt_InstallationBasic.SalesmanEmpNo1 = doIB.SalesmanEmpNo1;
                            doTbt_InstallationBasic.SalesmanEmpNo2 = doIB.SalesmanEmpNo2;
                        }
                        handler.UpdateTbt_InstallationBasic(doTbt_InstallationBasic);
                        #endregion
                    }
                    handler.InsertTbt_InstallationManagement(doIM);
                    #region /////////////////// INSERT MEMO ////////////////////////
                    if (!CommonUtil.IsNullOrEmpty(doIM.RequestMemo))
                    {
                        tbt_InstallationMemo doMemo = new tbt_InstallationMemo();
                        doMemo.ContractProjectCode = sParam.ContractProjectCodeLong;
                        doMemo.ReferenceID = StrMANo;
                        doMemo.ObjectID = ScreenID.C_SCREEN_ID_INSTALL_REQUEST;
                        doMemo.Memo = doIM.RequestMemo;
                        doMemo.OfficeCode = CommonUtil.dsTransData.dtUserData.MainOfficeCode;
                        doMemo.DepartmentCode = CommonUtil.dsTransData.dtUserData.MainDepartmentCode;
                        doMemo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doMemo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        handler.InsertTbt_InstallationMemo(doMemo);
                    }
                    #endregion /////////////////////////////////////////////////////////
                    #region /////////////////// INSERT EMAIL ////////////////////////
                    if (sParam.ListDOEmail != null)
                    {
                        foreach (ISS010_DOEmailData dataEmail in sParam.ListDOEmail)
                        {
                            tbt_InstallationEmail doEmail = new tbt_InstallationEmail();
                            doEmail.ReferenceID = StrMANo;
                            doEmail.EmailNoticeTarget = dataEmail.EmailAddress;
                            doEmail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            doEmail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            handler.InsertTbt_InstallationEmail(doEmail);
                        }
                    }
                    #endregion /////////////////////////////////////////////////////////
                    #region //================= Update Attach ======================
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
                    #endregion //======================================================

                    scope.Complete();

                }

                // - Add by Nontawat L. on 07-Jul-2014: Send E-mail
                // ////////////////// Send Email //////////////////
                List<doSystemConfig> mailto = chandler.GetSystemConfig(ConfigName.C_CONFIG_INSTALLATION_REQUEST_EMAIL);

                if (mailto != null && mailto.Count > 0 && !string.IsNullOrEmpty(mailto[0].ConfigValue))
                {
                    SendMailObject obj = new SendMailObject();
                    obj.EmailList = new List<doEmailProcess>();

                    doEmailTemplate template = ISS010_GenerateMailRegisterInstallationRequest(StrMANo);
                    if (template != null)
                    {
                        doEmailProcess mail = new doEmailProcess()
                        {
                            MailTo = mailto[0].ConfigValue,
                            Subject = template.TemplateSubject,
                            Message = template.TemplateContent,
                            IsBodyHtml = true,
                        };
                        obj.EmailList.Add(mail);

                        System.Threading.Thread t = new System.Threading.Thread(SendMail);
                        t.Start(obj);
                    }
                }
                // - End add



                ISS010_RegisterStartResumeTargetData result = new ISS010_RegisterStartResumeTargetData();
                result.MaintenanceNo = StrMANo;
                res.ResultData = result;

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get installation data and business check Contract
        /// </summary>
        /// <param name="contractData"></param>
        /// <param name="saleData"></param>
        /// <param name="strServiceTypeCode"></param>
        /// <returns></returns>
        public ObjectResultData ISS010_ValidateContractError(dtRentalContractBasicForInstall contractData, dtSaleBasic saleData, string strServiceTypeCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                IRentralContractHandler ReantalContracthandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    #region ====================== Validate Case Rental =====================
                    if (ISS010_ValidExistOffice(contractData.OperationOfficeCode) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                        return res;
                    }
                    if (contractData.ProductTypeCode != ProductType.C_PROD_TYPE_AL && contractData.ProductTypeCode != ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5013);
                        return res;
                    }

                    if (handler.CheckAllRemoval(contractData.ContractCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5001);
                        return res;
                    }

                    if (ReantalContracthandler.CheckCancelContractBeforeStartService(contractData.ContractCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5002);
                        return res;
                    }

                    bool blnInstallationRegistered = handler.CheckInstallationRegister(contractData.ContractCode);
                    if (blnInstallationRegistered)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5003);
                        return res;
                    }

                    List<tbt_InstallationBasic> tmpTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(contractData.ContractCode);
                    if (tmpTbt_InstallationBasic != null && tmpTbt_InstallationBasic.Count > 0 && tmpTbt_InstallationBasic[0].InstallationBy != null && tmpTbt_InstallationBasic[0].InstallationBy != InstallationBy.C_INSTALLATION_BY_SUBCONTRACTOR)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5100);
                        return res;
                    }
                    #endregion
                }
                else if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    #region ====================== Validate Case Sale ======================
                    if (ISS010_ValidExistOffice(saleData.OperationOfficeCode) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                        return res;
                    }

                    if (handler.CheckAllRemoval(saleData.ContractCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5001);
                        return res;
                    }

                    //In case sale, no need to check this condition //Phoomsak L. 2012-05-24
                    //if (ReantalContracthandler.CheckCancelContractBeforeStartService(saleData.ContractCode))
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5002);
                    //    return res;
                    //}

                    bool blnInstallationRegistered = handler.CheckInstallationRegister(saleData.ContractCode);
                    if (blnInstallationRegistered)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5003);
                        return res;
                    }
                    List<tbt_InstallationBasic> tmpTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(saleData.ContractCode);
                    if (tmpTbt_InstallationBasic != null && tmpTbt_InstallationBasic.Count > 0 && tmpTbt_InstallationBasic[0].InstallationBy != null && tmpTbt_InstallationBasic[0].InstallationBy != InstallationBy.C_INSTALLATION_BY_SUBCONTRACTOR)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5100);
                        return res;
                    }
                    #endregion
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Validate before Register installation request data
        /// </summary>
        /// <param name="doIB"></param>
        /// <param name="doIM"></param>
        /// <param name="screenParam"></param>
        /// <param name="ValidData"></param>
        /// <returns></returns>
        public ActionResult ISS010_ValidateBeforeRegister(tbt_InstallationBasic doIB, tbt_InstallationManagement doIM, ISS010_ScreenParameter screenParam, ISS010_ValidateData ValidData)
        {
            ObjectResultData res = new ObjectResultData();

            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            ValidatorUtil validator = new ValidatorUtil();
            try
            {
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                #region ==================== Check suspend and permission ==================
                if (chandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_REQUEST, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                #endregion
                #region ================= Check Require Field ============================
                if (CommonUtil.IsNullOrEmpty(ValidData.ProposeInstallStartDate))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS010",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "ProposeInstallStartDate",
                                                    "lblProposedInstallationStartDate",
                                                    "ProposeInstallStartDate");
                }
                if (CommonUtil.IsNullOrEmpty(ValidData.CustomerStaffBelonging))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS010",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "CustomerStaffBelonging",
                                                    "lblCustomerStaffBelonging",
                                                    "CustomerStaffBelonging");
                }
                if (CommonUtil.IsNullOrEmpty(ValidData.CustomerStaffName))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS010",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "CustomerStaffName",
                                                    "lblCustomerStaffName",
                                                    "CustomerStaffName");
                }
                if (CommonUtil.IsNullOrEmpty(ValidData.CustomerStaffPhoneNo))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS010",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "CustomerStaffPhoneNo",
                                                    "lblCustomerStaffTelNo",
                                                    "CustomerStaffPhoneNo");
                }
                #endregion
                ISS010_ScreenParameter sParam = GetScreenObject<ISS010_ScreenParameter>();

                //ValidatorUtil.BuildErrorMessage(res, this);

                #region ======================= Validate Business field ==========================
                if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL || sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    if (doIB.InstallationType == null)
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                    "ISS010",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "InstallationType",
                                                    "lblInstallationType",
                                                    "InstallationType");
                    }

                }
                if (!CommonUtil.IsNullOrEmpty(doIM.NewPhoneLineOpenDate) || !CommonUtil.IsNullOrEmpty(doIM.NewConnectionPhoneNo) || !CommonUtil.IsNullOrEmpty(doIM.NewPhoneLineOwnerTypeCode))
                {
                    if (CommonUtil.IsNullOrEmpty(doIM.NewPhoneLineOpenDate))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                "ISS010",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "NewPhoneLineOpenDate",
                                                "lblNewTelLineOpenDate",
                                                "NewPhoneLineOpenDate");
                    }
                    if (CommonUtil.IsNullOrEmpty(doIM.NewConnectionPhoneNo))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                "ISS010",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "NewConnectionPhoneNo",
                                                "lblNewConnectTelNo",
                                                "NewConnectionPhoneNo");
                    }
                    if (CommonUtil.IsNullOrEmpty(doIM.NewPhoneLineOwnerTypeCode))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                "ISS010",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "NewPhoneLineOwnerTypeCode",
                                                "lblNewTelLineOwner",
                                                "NewPhoneLineOwnerTypeCode");
                    }
                }


                if (string.IsNullOrEmpty(doIB.PlanCode))
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                                                "ISS010",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "PlanCode",
                                                "lblPlanCode",
                                                "PlanCode");
                }

                ////Add by Jutarat A. on 17042013
                //if (sParam.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL
                //    && (sParam.ISS010_Session.dtRentalContractBasic != null
                //        && (sParam.ISS010_Session.dtRentalContractBasic.PODocAuditResult == DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED
                //            || sParam.ISS010_Session.dtRentalContractBasic.PODocAuditResult == DocAuditResult.C_DOC_AUDIT_RESULT_RETURNED
                //            || sParam.ISS010_Session.dtRentalContractBasic.PODocAuditResult == null))
                //    && (sParam.ISS010_Session.dtRentalContractBasic != null
                //        && (sParam.ISS010_Session.dtRentalContractBasic.ContractDocAuditResult == DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED
                //            || sParam.ISS010_Session.dtRentalContractBasic.ContractDocAuditResult == DocAuditResult.C_DOC_AUDIT_RESULT_RETURNED
                //            || sParam.ISS010_Session.dtRentalContractBasic.ContractDocAuditResult == null))
                //    )
                //{
                //    if (doIM.ApproveNo == null)
                //    {
                //        validator.AddErrorMessage(MessageUtil.MODULE_INSTALLATION,
                //                                    "ISS010",
                //                                    MessageUtil.MODULE_INSTALLATION,
                //                                    MessageUtil.MessageList.MSG5123,
                //                                    "ApproveNo",
                //                                    "lblApproveNo",
                //                                    "ApproveNo");
                //    }
                //}
                ////End Add

                #endregion

                ValidatorUtil.BuildErrorMessage(res, validator, null);

                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                #region //////////////////// VALIDATE BUSINESS //////////////////////
                res = ISS010_ValidateBusiness(sParam.ContractProjectCodeLong, sParam.ServiceTypeCode, doIB.InstallationType, ValidData);
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
        public bool ISS010_ValidExistOffice(string OfficeCode)
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
        /// Get installation data and business check Project
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public ObjectResultData ISS010_ValidateProjectError(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                IProjectHandler handler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<string> strStatus = handler.GetProjectStatus(strProjectCode);

                if (strStatus.Count > 0)
                {
                    if (strStatus[0] == ProjectStatus.C_PROJECT_STATUS_CANCEL)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5014);
                        return res;
                    }
                    else if (strStatus[0] == ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5015);
                        return res;
                    }
                }

                IInstallationHandler installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                bool blnInstallationRegistered = installHandler.CheckInstallationRegister(strProjectCode);
                if (blnInstallationRegistered)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5003);
                    return res;
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
        /// <param name="strContractProjectCode"></param>
        /// <param name="strServiceTypeCode"></param>
        /// <param name="strInstallType"></param>
        /// <returns></returns>
        public ObjectResultData ISS010_ValidateBusiness(string strContractProjectCode, string strServiceTypeCode, string strInstallType, ISS010_ValidateData doValidate)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {

                IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                //================= Teerapong S. 15/10/2012 =======================
                List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(strContractProjectCode);
                List<tbt_InstallationBasic> doValidate_InstallationBasic = tmpdoTbt_InstallationBasic;
                ISS010_ScreenParameter sParam = GetScreenObject<ISS010_ScreenParameter>();
                if ((sParam.doIB == null && doValidate_InstallationBasic != null)
                    || (sParam.doIB != null && doValidate_InstallationBasic == null)
                    || (sParam.doIB != null && doValidate_InstallationBasic != null && DateTime.Compare(sParam.doIB.UpdateDate.Value, doValidate_InstallationBasic[0].UpdateDate.Value) != 0)
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }
                //=================================================================

                if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL || strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    //tbt_InstallationBasic doTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(strContractProjectCode);
                    tbt_InstallationBasic doTbt_InstallationBasic = null;
                    //List<tbt_InstallationBasic> tmpdoTbt_InstallationBasic = handler.GetTbt_InstallationBasicData(strContractProjectCode);
                    if (tmpdoTbt_InstallationBasic != null)
                    {
                        doTbt_InstallationBasic = new tbt_InstallationBasic();
                        doTbt_InstallationBasic = tmpdoTbt_InstallationBasic[0];
                    }
                    if (strInstallType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW || strInstallType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW || strInstallType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD)
                    {
                        if (doTbt_InstallationBasic == null
                                || (doTbt_InstallationBasic.InstallationStatus != InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED && string.IsNullOrEmpty(doTbt_InstallationBasic.InstallationType))
                                || (doTbt_InstallationBasic.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW && doTbt_InstallationBasic.InstallationType != SaleInstallationType.C_SALE_INSTALL_TYPE_NEW && doTbt_InstallationBasic.InstallationType != SaleInstallationType.C_SALE_INSTALL_TYPE_ADD))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5064);
                        }
                    }
                    else
                    {
                        if (!(doTbt_InstallationBasic == null
                                || (doTbt_InstallationBasic.InstallationStatus != InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED && string.IsNullOrEmpty(doTbt_InstallationBasic.InstallationType))
                                || (doTbt_InstallationBasic.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW && doTbt_InstallationBasic.InstallationType != SaleInstallationType.C_SALE_INSTALL_TYPE_NEW && doTbt_InstallationBasic.InstallationType != SaleInstallationType.C_SALE_INSTALL_TYPE_ADD)))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5065);
                        }
                    }
                }
                if (!CommonUtil.IsNullOrEmpty(doValidate.ProposeInstallCompleteDate))
                {
                    if (Convert.ToDateTime(doValidate.ProposeInstallStartDate) > Convert.ToDateTime(doValidate.ProposeInstallCompleteDate))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INSTALLATION, MessageUtil.MessageList.MSG5113, null, new string[] { "ProposeInstallStartDate", "ProposeInstallCompleteDate" });
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
        /// Clear all email data in session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS010_ClearInstallEmail(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS010_ScreenParameter session = GetScreenObject<ISS010_ScreenParameter>();
            session.ListDOEmail = null;
            UpdateScreenObject(session);

            return Json(res);
        }
        /// <summary>
        /// Upload attach file
        /// </summary>
        /// <param name="fileSelect"></param>
        /// <param name="DocumentName"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult ISS010_AttachFile(HttpPostedFileBase fileSelect, string DocumentName, string k)
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

            return View("ISS010_Upload");
        }
        /// <summary>
        /// Initial grid attach schema 
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS010_IntialGridAttachedDocList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS010_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// Remove attach file
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult ISS010_RemoveAttach(string AttachID)
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
        /// <param name="temp"></param>
        /// <returns></returns>
        public ActionResult ISS010_ClearAllAttach(string temp)
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
        public ActionResult ISS010_LoadGridAttachedDocList()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                List<dtAttachFileForGridView> lstAttachedName = commonhandler.GetAttachFileForGridView(GetCurrentKey());
                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Installation\\ISS010_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //========= ISR060 
        /// <summary>
        /// Create report installation request
        /// </summary>
        /// <param name="MaintenanceNo"></param>
        /// <returns></returns>
        public ActionResult ISS010_CreateReportInstallationRequest(string MaintenanceNo)
        {
            ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
            IInstallationDocumentHandler hand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            return File(hand.CreateReportInstallationRequestData(MaintenanceNo), "application/pdf", "ISR010.pdf");
        }

        public ActionResult ISS010_CreateReportInstallationRequestFilePath(string MaintenanceNo)
        {
            ObjectResultData res = new ObjectResultData();
            ISS050_ScreenParameter sParam = GetScreenObject<ISS050_ScreenParameter>();
            IInstallationDocumentHandler hand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            res.ResultData = hand.CreateReportInstallationRequestFilePath(MaintenanceNo);
            //return File(hand.CreateReportInstallationRequestData(MaintenanceNo), "application/pdf", "ISR010.pdf");
            return Json(res);
        }
        /// <summary>
        /// Clear contract code in session parameter
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS010_ClearCommonContractCode(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            ISS010_ScreenParameter sParam = GetScreenObject<ISS010_ScreenParameter>();
            //---- Clear ContractCode ----- Add by Teerapong S. 26/Apr/2012 =======================
            //CommonUtil.dsTransData.dtCommonSearch.ContractCode = null;
            //CommonUtil.dsTransData.dtCommonSearch.ProjectCode = null;            
            //====================================================================================            
            if (sParam != null)
            {
                sParam.ContractProjectCodeShort = null;
                sParam.strContractProjectCode = null;
                sParam.CommonSearch = new ScreenParameter.CommonSearchDo();
            }

            return Json(res);
        }
        /// <summary>
        /// Reset Data
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        public ActionResult ISS010_ResetButtonClick(string strContractProjectCode)
        {
            ObjectResultData res = new ObjectResultData();

            ISS010_ScreenParameter param = GetScreenObject<ISS010_ScreenParameter>();
            res.ResultData = param.ContractProjectCodeShort;

            return Json(res);
        }

        /// <summary>
        /// Get rental installation type other case
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS010_GetRentalInstalltypeOtherCase(string strFieldName)
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
        /// Get sale installation type other case
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS010_GetSaleInstalltypeOtherCase(string strFieldName)
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
        /// Download report
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ActionResult ISS010_DownloadPdfReport(string filePath)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IDocumentHandler handlerDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                Stream reportStream = handlerDoc.GetDocumentReportFileStream(filePath);

                //Add by Jutarat A. on 04032013
                if (reportStream == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                    return Json(res);
                }
                //End Add

                return File(reportStream, "application/pdf");
            }
            catch (Exception ex)
            {
                //ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }


        /// <summary>
        /// Generate email template
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        private doEmailTemplate ISS010_GenerateMailRegisterInstallationRequest(string strMaintenanceNo)
        {
            try
            {
                var reportHandler = ServiceContainer.GetService<SECOM_AJIS.DataEntity.Installation.IReportHandler>() as SECOM_AJIS.DataEntity.Installation.IReportHandler;
                var rptList = reportHandler.GetRptInstallationRequestData(strMaintenanceNo);

                doRegisterInstallationRequest obj = new doRegisterInstallationRequest();

                if (rptList.Count > 0)
                {
                    obj.ContractProjectCode = (rptList[0].ContractCodeShort ?? "-");
                    obj.UserCode = (rptList[0].UserCode ?? "-");
                    obj.SecurityTypeCode = (rptList[0].SecurityTypeCode ?? "-");
                    obj.ProductNameLC = (rptList[0].ProductNameLC ?? "-");
                    obj.ContractPurchaserName = (rptList[0].ContractPurchaserName ?? "-");
                    obj.BranchNameLC = (rptList[0].BranchNameLC ?? "-");
                    obj.SiteNameLC = (rptList[0].SiteNameLC ?? "-");
                    obj.AddressFullLC = (rptList[0].AddressFullLC ?? "-");
                    obj.OfficeNameLC = (rptList[0].OfficeNameLC ?? "-");
                    obj.MaintenanceNo = (rptList[0].MaintenanceNo ?? "-");
                    obj.PlanCode = (rptList[0].PlanCode ?? "-");
                    obj.InstallationTypeName = (rptList[0].InstallationTypeName ?? "-");
                    obj.Salesman1 = (rptList[0].Salesman1 ?? "-");
                    obj.Salesman2 = (rptList[0].Salesman2 ?? "-");
                    obj.BuildingTypeName = (rptList[0].BuildingTypeName ?? "-");
                    obj.NewBuildingManagementType = (rptList[0].NewBuildingManagementType ?? "-");
                    obj.ProposeInstallStartDate = (rptList[0].ProposeInstallStartDate ?? "-");
                    obj.ProposeInstallCompleteDate = (rptList[0].ProposeInstallCompleteDate ?? "-");
                    obj.NewPhoneLineOpenDate = (rptList[0].NewPhoneLineOpenDate ?? "-");
                    obj.NewConnectionPhoneNo = (rptList[0].NewConnectionPhoneNo ?? "-");
                    obj.NewPhoneLineOwnerTypeName = (rptList[0].NewPhoneLineOwnerTypeName ?? "-");
                    obj.RequestMemo = (rptList[0].RequestMemo ?? "-");
                }
                else
                {
                    obj.ContractProjectCode = "-";
                    obj.UserCode = "-";
                    obj.SecurityTypeCode = "-";
                    obj.ProductNameLC = "-";
                    obj.ContractPurchaserName = "-";
                    obj.BranchNameLC = "-";
                    obj.SiteNameLC = "-";
                    obj.AddressFullLC = "-";
                    obj.OfficeNameLC = "-";
                    obj.MaintenanceNo = "-";
                    obj.PlanCode = "-";
                    obj.InstallationTypeName = "-";
                    obj.Salesman1 = "-";
                    obj.Salesman2 = "-";
                    obj.BuildingTypeName = "-";
                    obj.NewBuildingManagementType = "-";
                    obj.ProposeInstallStartDate = "-";
                    obj.ProposeInstallCompleteDate = "-";
                    obj.NewPhoneLineOpenDate = "-";
                    obj.NewConnectionPhoneNo = "-";
                    obj.NewPhoneLineOwnerTypeName = "-";
                    obj.RequestMemo = "-";
                }

                EmailTemplateUtil util = new EmailTemplateUtil(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_INSTALL_REQUEST);
                return util.LoadTemplate(obj);
            }
            catch (Exception)
            {
                throw;
            }

        }


    }
}
