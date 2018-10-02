
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
        /// Authority Screen ISS080
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult ISS080_Authority(ISS080_ScreenParameter param)
        {
            // permission
            
            


            ObjectResultData res = new ObjectResultData();
            


            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_INSTALL_SEACH_MANAGE, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            

            // parameter
            //ISS080_Parameter param = new ISS080_Parameter();


            return InitialScreenEnvironment<ISS080_ScreenParameter>("ISS080", param, res);
            
        }
        /// <summary>
        /// Initial screen ISS080
        /// </summary>
        /// <returns></returns>
        [Initialize("ISS080")]
        public ActionResult ISS080()
        {
            //ISS080_ScreenParameter param = new ISS080_ScreenParameter();
            ISS080_ScreenParameter param = GetScreenObject<ISS080_ScreenParameter>();
            //if(param != null)
            //{
            //    ViewBag.ContractProjectCode = param.ContractCodeShort;
            //}
            return View();
        }

        // InitialGridEmail
        /// <summary>
        /// Initial grid email schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS080_InitialGridEmail()
        {
            return Json( CommonUtil.ConvertToXml<object>(null, "Installation\\ISS060_Email", CommonUtil.GRID_EMPTY_TYPE.INSERT) );
        }
        /// <summary>
        /// Initial grid instrument information schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS080_InitialGridInstrumentInfo()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS030_InstrumentDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        /// <summary>
        /// ISS080 retrieve data
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public ActionResult ISS080_RetrieveData(doSearchInstallManageCriteria searchCriteria)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            CommonUtil comUtil = new CommonUtil();           
            
            string lang = CommonUtil.GetCurrentLanguage();
            ISS080_RegisterStartResumeTargetData result = new ISS080_RegisterStartResumeTargetData();
            try
            {
                searchCriteria.ContractCode = comUtil.ConvertContractCode(searchCriteria.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                searchCriteria.ProjectCode = comUtil.ConvertProjectCode(searchCriteria.ProjectCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                if (searchCriteria.ContractCode == null
                    && searchCriteria.IEStaffCode == null
                    && searchCriteria.InstallationCompleteDateFrom == null
                    && searchCriteria.InstallationCompleteDateTo == null
                    && searchCriteria.InstallationFinishDateFrom == null
                    && searchCriteria.InstallationFinishDateTo == null
                    && searchCriteria.InstallationManagementStatus == null
                    && searchCriteria.InstallationStartDateFrom == null
                    && searchCriteria.InstallationStartDateTo == null
                    && searchCriteria.InstallationType == null
                    && searchCriteria.OperationOfficeCode == null
                    && searchCriteria.ProjectCode == null
                    && searchCriteria.ProposedInstallationCompleteDateFrom == null
                    && searchCriteria.ProposedInstallationCompleteDateTo == null
                    && searchCriteria.SiteAddress == null
                    && searchCriteria.SiteName == null
                    && searchCriteria.SubcontractorCode == null
                    && searchCriteria.SubcontractorGroupName == null
                    && searchCriteria.InstallationRequestDateFrom == null //Add by Jutarat A. on 22102013
                    && searchCriteria.InstallationRequestDateTo == null //Add by Jutarat A. on 22102013
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                    return Json(res);
                }
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                
                ///////////// START RETRIEVE DATA ////////////////////////
                IInstallationHandler iHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                List<doSearchInstallManagementResult> dtSearchResult = iHandler.SearchInstallationManagementList(searchCriteria);

                if (dtSearchResult == null)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    //return Json(res);
                }
                else
                {

                    foreach (doSearchInstallManagementResult dtSearchData in dtSearchResult)
                    {
                        //================== Get Installation Type Name ===================
                        if (!CommonUtil.IsNullOrEmpty(dtSearchData.InstallationType))
                        {
                            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                            {
                                new doMiscTypeCode()
                                {
                                    FieldName = "RentalInstallationType",
                                    ValueCode = dtSearchData.InstallationType
                                }
                            };

                            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                            List<doMiscTypeCode> lst = hand.GetMiscTypeCodeList(miscs);

                            if (lst.Count > 0)
                            {
                                dtSearchData.InstallationType = lst[0].ValueDisplay;
                            }
                            else
                            {
                                miscs = new List<doMiscTypeCode>()
                                {
                                    new doMiscTypeCode()
                                    {
                                        FieldName = "SaleInstallationType",
                                        ValueCode = dtSearchData.InstallationType
                                    }
                                };                            
                                lst = hand.GetMiscTypeCodeList(miscs);
                                if (lst.Count > 0)
                                {
                                    dtSearchData.InstallationType = lst[0].ValueDisplay;
                                }
                            }
                        }
                        //=======================================================
                        //================ Get management Status Name ==============
                        if (dtSearchData.InstallationManagementStatus == InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_PROCESSING)
                        {
                            dtSearchData.InstallationManagementStatus = ISS080_GetMiscDisplayByValueCode("InstallationManagementStatus", InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_PROCESSING);//"Processing";
                        }
                        else if (dtSearchData.InstallationManagementStatus == InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REQUEST_APPROVE)
                        {
                            dtSearchData.InstallationManagementStatus = ISS080_GetMiscDisplayByValueCode("InstallationManagementStatus", InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REQUEST_APPROVE);//"Request approve";
                        }
                        else if (dtSearchData.InstallationManagementStatus == InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_APPROVED)
                        {
                            dtSearchData.InstallationManagementStatus = ISS080_GetMiscDisplayByValueCode("InstallationManagementStatus", InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_APPROVED);//"Approved";
                        }
                        else if (dtSearchData.InstallationManagementStatus == InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REJECTED)
                        {
                            dtSearchData.InstallationManagementStatus = ISS080_GetMiscDisplayByValueCode("InstallationManagementStatus", InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_REJECTED);//"Rejected";
                        }
                        else if (dtSearchData.InstallationManagementStatus == InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_COMPLETED)
                        {
                            dtSearchData.InstallationManagementStatus = ISS080_GetMiscDisplayByValueCode("InstallationManagementStatus", InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_COMPLETED);//"Completed";
                        }
                        else if (dtSearchData.InstallationManagementStatus == InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_CANCELED)
                        {
                            dtSearchData.InstallationManagementStatus = ISS080_GetMiscDisplayByValueCode("InstallationManagementStatus", InstallationManagementStatus.C_INSTALL_MANAGE_STATUS_CANCELED);//"Canceled";
                        }
                        //==========================================================

                        //=================== Convert Contract Code ========================
                        
                        if (dtSearchData.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                        {
                            dtSearchData.ContractProjectCode = comUtil.ConvertProjectCode(dtSearchData.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        }
                        else
                        {
                            dtSearchData.ContractProjectCode = comUtil.ConvertContractCode(dtSearchData.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        }
                        //==================================================================


                        if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                        {
                            dtSearchData.IEStaffName1 = dtSearchData.IEStaffName1EN;
                            dtSearchData.IEStaffName2 = dtSearchData.IEStaffName2EN;
                            dtSearchData.SubcontractorName = dtSearchData.SubcontractorNameEN;
                            dtSearchData.OperationOfficeName = dtSearchData.OperationOfficeNameEN;
                        }
                        else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_JP)
                        {
                            dtSearchData.IEStaffName1 = dtSearchData.IEStaffName1EN;
                            dtSearchData.IEStaffName2 = dtSearchData.IEStaffName2EN;
                            dtSearchData.SubcontractorName = dtSearchData.SubcontractorNameEN;
                            dtSearchData.OperationOfficeName = dtSearchData.OperationOfficeNameEN;
                        }
                        else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_LC)
                        {
                            dtSearchData.IEStaffName1 = dtSearchData.IEStaffName1LC;
                            dtSearchData.IEStaffName2 = dtSearchData.IEStaffName2LC;
                            dtSearchData.SubcontractorName = dtSearchData.SubcontractorNameLC;
                            dtSearchData.OperationOfficeName = dtSearchData.OperationOfficeNameLC;
                        }
                    }
                }
                
                result.doSearchData = dtSearchResult;
                res.ResultData = result;
                
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Validate data
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS080_ValidateRegisterData()
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
        /// Get data for installation type combobox
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS080_GetMiscInstallationtype(string strFieldName)
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
        public ActionResult ISS080_GetMiscTypeByValueCode(string FieldName,string ValueCode)
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
        /// Get misctype display by code
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="ValueCode"></param>
        /// <returns></returns>
        public string ISS080_GetMiscDisplayByValueCode(string FieldName, string ValueCode)
        {
            ObjectResultData res = new ObjectResultData();
            string strDisPlay = "";
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
                    if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        strDisPlay = lst[0].ValueDisplayEN;
                    }
                    else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_JP)
                    {
                        strDisPlay = lst[0].ValueDisplayJP;
                    }
                    else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_LC)
                    {
                        strDisPlay = lst[0].ValueDisplayLC;
                    }

                }
                else
                {
                    strDisPlay = "";
                }
                
            }
            catch (Exception ex)
            {                
                
            }
            return strDisPlay;
        }
        /// <summary>
        /// Get office name by code
        /// </summary>
        /// <param name="ValueCode"></param>
        /// <returns></returns>
        public ActionResult ISS080_GetOfficeNameByCode(string ValueCode)
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
        /// Check existing office
        /// </summary>
        /// <param name="OfficeCode"></param>
        /// <returns></returns>
        public bool ISS080_ValidExistOffice(string OfficeCode)
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
        /// Initial grid management schema
        /// </summary>
        /// <returns></returns>
        public ActionResult ISS080_InitialGridManagementList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS080_InstallManagementList", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Get data all installation type
        /// </summary>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ISS080_GetAllInstallationtype(string strFieldName)
        {

            string strDisplayName = "ValueCodeDisplay";
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            List<doMiscTypeCode> lst2 = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = "RentalInstallationType",
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);

                miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = "SaleInstallationType",
                        ValueCode = "%"
                    }
                };
                lst2 = hand.GetMiscTypeCodeList(miscs);

                foreach(doMiscTypeCode dtl in lst2){
                    lst.Add(dtl);
                }


                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(lst, "ValueCodeDisplay", "ValueCode");

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
        /// Search data and send to show in grid search result
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public ActionResult ISS080_SearchDataToGrid(doSearchInstallManageCriteria searchCriteria)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            CommonUtil comUtil = new CommonUtil();

            string lang = CommonUtil.GetCurrentLanguage();
            ISS080_RegisterStartResumeTargetData result = new ISS080_RegisterStartResumeTargetData();
            try
            {
                searchCriteria.ContractCode = comUtil.ConvertContractCode(searchCriteria.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                searchCriteria.ProjectCode = comUtil.ConvertProjectCode(searchCriteria.ProjectCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                if (searchCriteria.ContractCode == null
                    && searchCriteria.IEStaffCode == null
                    && searchCriteria.InstallationCompleteDateFrom == null
                    && searchCriteria.InstallationCompleteDateTo == null
                    && searchCriteria.InstallationFinishDateFrom == null
                    && searchCriteria.InstallationFinishDateTo == null
                    && searchCriteria.InstallationManagementStatus == null
                    && searchCriteria.InstallationStartDateFrom == null
                    && searchCriteria.InstallationStartDateTo == null
                    && searchCriteria.InstallationType == null
                    && searchCriteria.OperationOfficeCode == null
                    && searchCriteria.ProjectCode == null
                    && searchCriteria.ProposedInstallationCompleteDateFrom == null
                    && searchCriteria.ProposedInstallationCompleteDateTo == null
                    && searchCriteria.SiteAddress == null
                    && searchCriteria.SiteName == null
                    && searchCriteria.SubcontractorCode == null
                    && searchCriteria.SubcontractorGroupName == null
                    && searchCriteria.InstallationRequestDateFrom == null //Add by Jutarat A. on 22102013
                    && searchCriteria.InstallationRequestDateTo == null //Add by Jutarat A. on 22102013
                    && searchCriteria.ExpectedInstallationStartDateFrom == null
                    && searchCriteria.ExpectedInstallationStartDateTo == null
                    && searchCriteria.ExpectedInstallationFinishDateFrom == null
                    && searchCriteria.ExpectedInstallationFinishDateTo == null
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                    return Json(res);
                }
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

                ///////////// START RETRIEVE DATA ////////////////////////
                IInstallationHandler iHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                List<doSearchInstallManagementResult> dtSearchResult = iHandler.SearchInstallationManagementList(searchCriteria);

                if (dtSearchResult == null)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    //return Json(res);
                }
                else
                {

                    foreach (doSearchInstallManagementResult dtSearchData in dtSearchResult)
                    {
                        //================== Get Installation Type Name ===================
                        if (!CommonUtil.IsNullOrEmpty(dtSearchData.InstallationType))
                        {
                            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                            {
                                new doMiscTypeCode()
                                {
                                    FieldName = "RentalInstallationType",
                                    ValueCode = dtSearchData.InstallationType
                                }
                            };

                            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                            List<doMiscTypeCode> lst = hand.GetMiscTypeCodeList(miscs);

                            if (lst.Count > 0)
                            {
                                dtSearchData.InstallationType = lst[0].ValueDisplay;
                            }
                            else
                            {
                                miscs = new List<doMiscTypeCode>()
                                {
                                    new doMiscTypeCode()
                                    {
                                        FieldName = "SaleInstallationType",
                                        ValueCode = dtSearchData.InstallationType
                                    }
                                };
                                lst = hand.GetMiscTypeCodeList(miscs);
                                if (lst.Count > 0)
                                {
                                    dtSearchData.InstallationType = lst[0].ValueDisplay;
                                }
                            }
                        }
                        //=======================================================
                        //================ Get management Status Name ==============
                        dtSearchData.InstallationManagementStatus = ISS080_GetMiscDisplayByValueCode("InstallationManagementStatus", dtSearchData.InstallationManagementStatus);
                        //==========================================================

                        //=================== Convert Contract Code ========================

                        if (dtSearchData.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                        {
                            dtSearchData.ContractProjectCode = comUtil.ConvertProjectCode(dtSearchData.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        }
                        else
                        {
                            dtSearchData.ContractProjectCode = comUtil.ConvertContractCode(dtSearchData.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        }
                        //==================================================================


                        if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                        {
                            dtSearchData.IEStaffName1 = dtSearchData.IEStaffName1EN;
                            dtSearchData.IEStaffName2 = dtSearchData.IEStaffName2EN;
                            dtSearchData.SubcontractorName = dtSearchData.SubcontractorNameEN;
                            dtSearchData.OperationOfficeName = dtSearchData.OperationOfficeNameEN;
                        }
                        else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_JP)
                        {
                            dtSearchData.IEStaffName1 = dtSearchData.IEStaffName1EN;
                            dtSearchData.IEStaffName2 = dtSearchData.IEStaffName2EN;
                            dtSearchData.SubcontractorName = dtSearchData.SubcontractorNameEN;
                            dtSearchData.OperationOfficeName = dtSearchData.OperationOfficeNameEN;
                        }
                        else if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_LC)
                        {
                            dtSearchData.IEStaffName1 = dtSearchData.IEStaffName1LC;
                            dtSearchData.IEStaffName2 = dtSearchData.IEStaffName2LC;
                            dtSearchData.SubcontractorName = dtSearchData.SubcontractorNameLC;
                            dtSearchData.OperationOfficeName = dtSearchData.OperationOfficeNameLC;
                        }
                    }
                }

                result.doSearchData = dtSearchResult;
                if (dtSearchResult != null && dtSearchResult.Count > 0)
                {
                    foreach(doSearchInstallManagementResult data in dtSearchResult)
                    {
                        string strInstallationStartDate = (CommonUtil.IsNullOrEmpty(data.InstallationStartDate) ? "-" : ((DateTime)data.InstallationStartDate).ToString("dd-MMM-yyyy"));
                        string strInstallationFinishDate = (CommonUtil.IsNullOrEmpty(data.InstallationFinishDate) ? "-" : ((DateTime)data.InstallationFinishDate).ToString("dd-MMM-yyyy"));
                        string strProposedInstallationCompleteDate = (CommonUtil.IsNullOrEmpty(data.ProposedInstallationCompleteDate) ? "-" : ((DateTime)data.ProposedInstallationCompleteDate).ToString("dd-MMM-yyyy"));
                        string strActualInstallationCompleteDate = (CommonUtil.IsNullOrEmpty(data.ActualInstallationCompleteDate) ? "-" : ((DateTime)data.ActualInstallationCompleteDate).ToString("dd-MMM-yyyy"));

                        string strInstallationRequestDate = (CommonUtil.IsNullOrEmpty(data.InstallationRequestDate) ? "-" : ((DateTime)data.InstallationRequestDate).ToString("dd-MMM-yyyy")); //Add by Jutarat A. on 22102013

                        if (CommonUtil.IsNullOrEmpty(data.ContractProjectCode))
                            data.ContractProjectCode = "-";
                        if (CommonUtil.IsNullOrEmpty(data.InstallationType))
                            data.InstallationType = "-";
                        if (CommonUtil.IsNullOrEmpty(data.IEStaffName1))
                            data.IEStaffName1 = "-";
                        if (CommonUtil.IsNullOrEmpty(data.IEStaffName2))
                            data.IEStaffName2 = "-";
                        if (CommonUtil.IsNullOrEmpty(data.SubcontractorName))
                            data.SubcontractorName = "-";
                        if (CommonUtil.IsNullOrEmpty(data.SubcontractorGroupName))
                            data.SubcontractorGroupName = "-";

                        if (CommonUtil.IsNullOrEmpty(data.SiteCode))
                            data.SiteCode = "-";
                        else
                            data.SiteCode = comUtil.ConvertSiteCode(data.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        if (CommonUtil.IsNullOrEmpty(data.SiteNameEN))
                            data.SiteNameEN = "-";
                        if (CommonUtil.IsNullOrEmpty(data.SiteNameLC))
                            data.SiteNameLC = "-";
                        if (CommonUtil.IsNullOrEmpty(data.OperationOfficeName))
                            data.OperationOfficeName = "-";
                        if (CommonUtil.IsNullOrEmpty(data.InstallationManagementStatus))
                            data.InstallationManagementStatus = "-";
                    
                        data.ContractProjectCode = data.ContractProjectCode + "<br />" + data.InstallationType;
                        data.IEStaffName = "(1) " + data.IEStaffName1 + "<br />" + "(2) " + data.IEStaffName2;
                        data.Subcontractor = "(1) " + data.SubcontractorName + "<br />" + "(2) " + data.SubcontractorGroupName + "<br />" + "(3) " + data.NoOfSubcontractor;
                        
                        //data.IntsallationDate = "(1) " + strInstallationStartDate + "<br />" +
                        //                        "(2) " + strInstallationFinishDate +
                        //                        "<br />(3) " + strProposedInstallationCompleteDate + "<br />" +
                        //                        "(4) " + strActualInstallationCompleteDate;
                        data.IntsallationDate = "(1) " + strInstallationRequestDate + "<br />" +
                                                "(2) " + strProposedInstallationCompleteDate + "<br />" +
                                                "(3) " + data.ExpectInstallStartDate + "<br />" +
                                                "(4) " + data.ExpectInstallFinishDate + "<br />" +
                                                "(5) " + strInstallationStartDate + "<br />" +
                                                "(6) " + strInstallationFinishDate + "<br />" +
                                                "(7) " + strActualInstallationCompleteDate;                 //Modify (Add InstallationRequestDate) by Jutarat A. on 22102013

                        data.Site = "(1) "+data.SiteCode + "<br />" + "(2) "+data.SiteNameEN + "<br />" + "(3) "+data.SiteNameLC;
                        data.OperationOffice = data.OperationOfficeName;
                    }
                }
                //res.ResultData = result;
                res.ResultData = CommonUtil.ConvertToXml<doSearchInstallManagementResult>(dtSearchResult, "Installation\\ISS080_InstallManagementList", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

    }
}
