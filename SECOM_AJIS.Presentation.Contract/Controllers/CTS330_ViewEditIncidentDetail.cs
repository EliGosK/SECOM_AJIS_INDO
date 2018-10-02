//*********************************
// Create by: Natthavat S.
// Create date: 18/Nov/2011
// Update date: 18/Nov/2011
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
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;
namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check user’s permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS330_Authority(CTS330_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;

            try
            {
                // Check System Suspend
                if (CheckIsSuspending(res))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                    return Json(res);
                }

                // Check Screen Permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_EDIT_INCIDENT))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
                    return Json(res);
                }

                // Check Screen Param
                //string strIncidentID = GetIncidentIDFromParameter_CTS330();
                int IncidentID = 0;
                if (String.IsNullOrEmpty(param.strIncidentID) || !int.TryParse(param.strIncidentID, out IncidentID))
                {
                    // Not Valid
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { "Incident ID" }, null);
                    return Json(res);
                }

                var incidentItem = incidenthandler.GetIncidentDetail(IncidentID);
                if (incidentItem.dtIncident == null)
                {
                    // Not Found
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { "Incident ID: " + IncidentID }, null);
                    return Json(res);
                }

                param.sessObj = CreateIncidentDataWithPermission_CTS330(IncidentID);

                // Check Confidential
                var incidentPermit = incidenthandler.HasIncidentPermission(IncidentID);

                //if (!incidentPermit.ViewConfidentialIncidentFlag && (incidentItem.dtIncident.ConfidentialFlag == FlagType.C_FLAG_ON))
                if (incidentPermit.ViewConfidentialIncidentFlag == false
                    && incidentPermit.EditConfidentailIncidentFlag == false //Add by Jutarat A. on 12102012
                    && (param.sessObj != null && param.sessObj.IsSpecialView == false)
                    && incidentItem.dtIncident.ConfidentialFlag == FlagType.C_FLAG_ON) //Modify by Jutarat A. on 05102012
                {
                    // No Confidential Permission
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053, null, null);
                    return Json(res);
                }

                //Session.Add("CTS330_" + IncidentID.ToString(), sessObj);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS330_ScreenParameter>("CTS330", param, res);
        }
        

        //public ActionResult CTS330_Authority(string param, string strIncidentID)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    CommonUtil comUtil = new CommonUtil();
        //    IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
        //    CTS330_ScreenParameter scrparam = null;

        //    try
        //    {
        //        // Check System Suspend
        //        if (CheckIsSuspending(res))
        //        {
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
        //            return Json(res);
        //        }

        //        // Check Screen Permission
        //        if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_EDIT_INCIDENT))
        //        {
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
        //            return Json(res);
        //        }

        //        // Check Screen Param
        //        //string strIncidentID = GetIncidentIDFromParameter_CTS330();
        //        int IncidentID = 0;
        //        if (String.IsNullOrEmpty(strIncidentID) || !int.TryParse(strIncidentID, out IncidentID))
        //        {
        //            // Not Valid
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { "Incident ID" }, null);
        //            return Json(res);
        //        }

        //        var incidentItem = incidenthandler.GetIncidentDetail(IncidentID);
        //        if (incidentItem.dtIncident == null)
        //        {
        //            // Not Found
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { "Incident ID: " + IncidentID }, null);
        //            return Json(res);
        //        }

        //        // Check Confidential
        //        var incidentPermit = incidenthandler.HasIncidentPermission(IncidentID);
        //        if (!incidentPermit.ViewConfidentialIncidentFlag && (incidentItem.dtIncident.ConfidentialFlag == FlagType.C_FLAG_ON))
        //        {
        //            // No Confidential Permission
        //            res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
        //            return Json(res);
        //        }

        //        scrparam = new CTS330_ScreenParameter()
        //        {
        //            strIncidentID = strIncidentID
        //        };

        //        CTS330_SessionData sessObj = CreateIncidentDataWithPermission_CTS330(IncidentID);

        //        Session.Add("CTS330_" + IncidentID.ToString(), sessObj);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }


        //    return InitialScreenEnvironment("CTS330", scrparam);
        //}

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS330")]
        public ActionResult CTS330()
        {
            CTS330_ScreenParameter sParam = GetScreenObject<CTS330_ScreenParameter>();
            ViewBag.useKey = sParam.strIncidentID;
            ViewBag.AttachKey = GetCurrentKey();

            sParam.newAttachLst = new Dictionary<int, string>();
            sParam.delAttachLst = new Dictionary<int, string>();
            return View();
        }

        /// <summary>
        /// Retrieve incident data for view or edit
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS330_LoadIncidentData()
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();

            try
            {
                IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
                IRentralContractHandler rentralhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IProductMasterHandler producthandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                CTS330_ScreenParameter sParam = GetScreenObject<CTS330_ScreenParameter>();
                int incidentID = int.Parse(sParam.strIncidentID);
                CTS330_SessionData incidentDat = GetIncidentData_CTS330(incidentID);

                dtRentalContractBasicForView rentralcontract = null;
                dtSaleContractBasicForView salecontract = null;
                int contractFrom = 0;

                List<string> miscItem = new List<string>();
                miscItem.Add(MiscType.C_RENTAL_CHANGE_TYPE);
                miscItem.Add(MiscType.C_SALE_CHANGE_TYPE);
                miscItem.Add(MiscType.C_STOP_CANCEL_REASON_TYPE);
                miscItem.Add(MiscType.C_INCIDENT_TYPE);
                miscItem.Add(MiscType.C_INCIDENT_RECEIVED_METHOD);
                var miscList = commonhandler.GetMiscTypeCodeListByFieldName(miscItem);
                
                if (incidentDat.incidentData.dtIncident.IncidentRelavantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                {
                    var rentralRes = rentralhandler.GetRentalContractBasicForView(incidentDat.incidentData.dtIncident.ContractCode, null);
                    if (rentralRes.Count == 1)
                    {
                        rentralcontract = rentralRes[0];
                        contractFrom = 1;
                    }
                    else
                    {
                        var saleRes = salehandler.GetSaleContractBasicForView(incidentDat.incidentData.dtIncident.ContractCode);
                        if (saleRes.Count == 1)
                        {
                            salecontract = saleRes[0];
                            contractFrom = 2;
                        }
                    }
                }

                if (rentralcontract != null)
                    CommonUtil.MappingObjectLanguage(rentralcontract);

                if (salecontract != null)
                    CommonUtil.MappingObjectLanguage(salecontract);

                string strLastChangeType = "";
                string strIncidentType = "";
                string strReasonType = "";
                string strProductName = "";
                string strReceivedType = "";

                //Comment by Jutarat A. on 04042013 (Not Use)
                //string strCustFullNameEN = "";
                //string strCustFullNameLC = "";
                //End Comment

                var tmpLastChangeType = from a in miscList where (a.FieldName == MiscType.C_RENTAL_CHANGE_TYPE) && (a.ValueCode == rentralcontract.LastChangeType) select a;
                if ((rentralcontract != null) && (tmpLastChangeType.Count() == 1))
                {
                    strLastChangeType = tmpLastChangeType.First().ValueCodeDisplay;
                }

                tmpLastChangeType = from a in miscList where (a.FieldName == MiscType.C_SALE_CHANGE_TYPE) && (a.ValueCode == salecontract.ChangeType) select a;
                if ((salecontract != null) && (tmpLastChangeType.Count() == 1))
                {
                    strLastChangeType = tmpLastChangeType.First().ValueCodeDisplay;
                }

                var tmpIncidentType = from a in miscList where (a.FieldName == MiscType.C_INCIDENT_TYPE) && (a.ValueCode == incidentDat.incidentData.dtIncident.IncidentType) select a;
                if ((incidentDat.incidentData.dtIncident != null) && (tmpIncidentType.Count() == 1))
                {
                    strIncidentType = tmpIncidentType.First().ValueCodeDisplay;
                }

                var reasonTypeList = incidenthandler.GetTbs_IncidentReasonType(incidentDat.incidentData.dtIncident.IncidentType);
                CommonUtil.MappingObjectLanguage<tbs_IncidentReasonType>(reasonTypeList);
                //var tmpReasonType = from a in miscList where (a.FieldName == MiscType.C_STOP_CANCEL_REASON_TYPE) && (a.ValueCode == incidentDat.incidentData.dtIncident.ReasonType) select a;
                var tmpReasonType = from a in reasonTypeList where a.ReasonType == incidentDat.incidentData.dtIncident.ReasonType select a;
                if ((incidentDat.incidentData.dtIncident != null) && (tmpReasonType.Count() == 1))
                {
                    strReasonType = tmpReasonType.First().ReasonTypeName;
                }

                if (rentralcontract != null)
                {
                    var tmpProductName = producthandler.GetTbm_Product(rentralcontract.ProductCode, null);
                    CommonUtil.MappingObjectLanguage<tbm_Product>(tmpProductName);
                    if (tmpProductName.Count == 1)
                    {
                        strProductName = CommonUtil.TextCodeName(tmpProductName[0].ProductCode, tmpProductName[0].ProductName);
                    }
                }

                //Comment by Jutarat A. on 04042013 (Not Use)
                //if (incidentDat.incidentData.dtIncident != null)
                //{
                //    var tmpCustItem = custhandler.GetCustomer(incidentDat.incidentData.dtIncident.CustCode);
                //    if (tmpCustItem.Count == 1)
                //    {
                //        strCustFullNameEN = tmpCustItem[0].CustFullNameEN;
                //        strCustFullNameLC = tmpCustItem[0].CustFullNameLC;
                //    }
                //}
                //End Comment

                var tmpReceivedType = from a in miscList where (a.FieldName == MiscType.C_INCIDENT_RECEIVED_METHOD) && (a.ValueCode == incidentDat.incidentData.dtIncident.ReceivedMethod) select a;
                if ((incidentDat.incidentData.dtIncident != null) && (tmpReceivedType.Count() == 1))
                {
                    strReceivedType = tmpReceivedType.First().ValueDisplay;
                }

                CTS330_DisplayData dispItem = new CTS330_DisplayData()
                {
                    IncidentRelevantType = incidentDat.incidentData.dtIncident.IncidentRelavantType,
                    IncidentNo = (String.IsNullOrEmpty(incidentDat.incidentData.dtIncident.IncidentNo) ? CommonUtil.GetLabelFromResource("Contract", "CTS330", "lblNA") : incidentDat.incidentData.dtIncident.IncidentNo),
                    CustomerCode = util.ConvertCustCode(incidentDat.incidentData.dtIncident.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    
                    //Modify by Jutarat A. on 04042013
                    //CustomerNameEN = strCustFullNameEN,
                    //CustomerNameLC = strCustFullNameLC,
                    CustomerNameEN = incidentDat.incidentData.dtIncident.CustFullNameEN,
                    CustomerNameLC = incidentDat.incidentData.dtIncident.CustFullNameLC,
                    //End Modify

                    SiteCode = util.ConvertSiteCode(incidentDat.incidentData.dtIncident.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    SiteNameEN = incidentDat.incidentData.dtIncident.SiteNameEN,
                    SiteNameLC = incidentDat.incidentData.dtIncident.SiteNameLC,
                    SiteAddressEN = incidentDat.incidentData.dtIncident.AddressFullEN,
                    ContractCode = util.ConvertContractCode(incidentDat.incidentData.dtIncident.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    UserCode = incidentDat.incidentData.dtIncident.UserCode,
                    ProjectCode = incidentDat.incidentData.dtIncident.ProjectCode,
                    ProjectName = incidentDat.incidentData.dtIncident.ProjectName,
                    Registrant = incidentDat.incidentData.dtIncident.RegistrantName,
                    //Registrant = GetNameFromLang(incidentDat.incidentData.dtIncident.RegistrantNameEN, null, incidentDat.incidentData.dtIncident.RegistrantNameLC),

                    StartDealDate = (rentralcontract != null) ? CommonUtil.TextDate(rentralcontract.StartDealDate) : null,

                    //MonthlyContractFee = (rentralcontract != null) ? CommonUtil.TextNumeric(rentralcontract.OrderContractFee) : null,

                    ContractDurationFrom = (rentralcontract != null) ? CommonUtil.TextDate(rentralcontract.ContractStartDate) : null,
                    ContractDurationTo = (rentralcontract != null) ? CommonUtil.TextDate(rentralcontract.ContractEndDate) : null,
                    FirstOperationDate = (rentralcontract != null) ? CommonUtil.TextDate(rentralcontract.FirstSecurityStartDate) : null,
                    ContractDuration = (rentralcontract != null) ? rentralcontract.ContractDurationMonth.GetValueOrDefault().ToString() : null,
                    AutoRenew = (rentralcontract != null) ? rentralcontract.AutoRenewMonth.GetValueOrDefault().ToString() : null,
                    LastOperationDate = (rentralcontract != null) ? CommonUtil.TextDate(rentralcontract.LastChangeImplementDate) : null,
                    OldContractCode = (rentralcontract != null) ? util.ConvertContractCode(rentralcontract.OldContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT) : null,
                    SecurityTypeCode = (rentralcontract != null) ? rentralcontract.SecurityTypeCode : null,
                    
                    ProcessManagementStatus = (salecontract != null) ? CommonUtil.TextCodeName( salecontract.SaleProcessManageStatus , salecontract.SaleProcessManageStatusName) : null,
                    ExpectedCompleteInstallationDate = (salecontract != null) ? CommonUtil.TextDate(salecontract.ExpectedInstallCompleteDate) : null,
                    MaintenanceContractCode = (salecontract != null) ? salecontract.MaintenanceContractCode : null,
                    CompleteInstallationDate = (salecontract != null) ? CommonUtil.TextDate(salecontract.InstallCompleteDate) : null,
                    SaleType = (salecontract != null) ? salecontract.SalesTypeName : null,
                    //SaleType = (salecontract != null) ? GetNameFromLang(salecontract.SalesTypeNameEN, salecontract.SalesTypeNameJP, salecontract.SalesTypeNameLC) : null,
                    ExpectedCustomerAcceptanceDate = (salecontract != null) ? CommonUtil.TextDate(salecontract.ExpectedCustAcceptanceDate) : null,
                    CustomerAcceptanceDate = (salecontract != null) ? CommonUtil.TextDate(salecontract.CustAcceptanceDate) : null,

                    ProductName = (rentralcontract != null) ? strProductName : null,
                    OperationOffice = (rentralcontract != null) ? rentralcontract.OperationOfficeCode + " : " + rentralcontract.OfficeName : (salecontract != null) ? CommonUtil.TextCodeName(salecontract.OperationOfficeCode, salecontract.OfficeName) : null,
                    
                    LastChangeType = (rentralcontract != null) ? strLastChangeType : null,

                    IncidentTitle = incidentDat.incidentData.dtIncident.IncidentTitle,
                    IncidentType = strIncidentType,
                    ReasonType = strReasonType,
                    IsConfidential = incidentDat.incidentData.dtIncident.ConfidentialFlag.GetValueOrDefault(),
                    IsImportance = incidentDat.incidentData.dtIncident.ImportanceFlag.GetValueOrDefault(),
                    ContractDetail = String.Format("{0} {1} {2} {3} {4}"
                        , CommonUtil.TextDate(incidentDat.incidentData.dtIncident.ReceivedDate)
                        , CommonUtil.TextTime(incidentDat.incidentData.dtIncident.ReceivedTime)
                        //, incidentDat.incidentData.dtIncident.ReceivedMethod
                        , strReceivedType
                        , incidentDat.incidentData.dtIncident.ContactPerson
                        , incidentDat.incidentData.dtIncident.ContactPersonDep),
                    ReceivedDetails = incidentDat.incidentData.dtIncident.ReceivedDetail,

                    DueDateDeadLineType = incidentDat.incidentData.dtIncident.DueDate.HasValue ? "1" : "2",
                    DueDate_Date = CommonUtil.TextDate(incidentDat.incidentData.dtIncident.DueDate),
                    DueDate_Time = CommonUtil.TextTime(incidentDat.incidentData.dtIncident.DueDateTime),
                    Deadline_Date = CommonUtil.TextDate(incidentDat.incidentData.dtIncident.DeadLine),
                    Deadline_Until = incidentDat.incidentData.dtIncident.DeadLineTime,
                    StatusAfterUpdate = incidentDat.incidentData.dtIncident.IncidentStatus,

                    ContractDataFrom = contractFrom,
                    HasChief = (incidentDat.incidentData.dtIncidentRole.Where(x => x.IncidentRoleType == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF).Count() > 0)
                };

                if (rentralcontract != null)
                {
                    dispItem.MonthlyContractFeeCurrencyType = rentralcontract.OrderContractFeeCurrencyType;
                    if (dispItem.MonthlyContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        dispItem.MonthlyContractFee = (rentralcontract != null) ? CommonUtil.TextNumeric(rentralcontract.OrderContractFeeUsd) : null;
                    else
                        dispItem.MonthlyContractFee = (rentralcontract != null) ? CommonUtil.TextNumeric(rentralcontract.OrderContractFee) : null;
                }
                
                //dispItem.MonthlyContractFee = (rentralcontract != null) ? CommonUtil.TextNumeric(rentralcontract.OrderContractFee) : null;


                if (incidentDat.IsSpecialView)
                {
                    dispItem.IsViewMode = false;
                }
                else if ((incidentDat.IsEditable || incidentDat.incidentPermit.EditConfidentailIncidentFlag || incidentDat.incidentPermit.EditIncidentFlag)
                    && ((incidentDat.incidentData.dtIncident.IncidentStatus != IncidentStatus.C_INCIDENT_STATUS_COMPLETE) && (incidentDat.incidentData.dtIncident.IncidentStatus != IncidentStatus.C_INCIDENT_STATUS_CONTROL_CHIEF_UNREGISTERED)))
                {
                    if (incidentDat.incidentData.dtIncident.ConfidentialFlag.GetValueOrDefault())
                    {
                        if (incidentDat.incidentPermit.EditConfidentailIncidentFlag)
                        {
                            dispItem.IsViewMode = false;
                        }
                        else
                        {
                            dispItem.IsViewMode = true;
                        }
                    }
                    else
                    {
                        if (incidentDat.incidentPermit.EditIncidentFlag)
                        {
                            dispItem.IsViewMode = false;
                        }
                        else
                        {
                            dispItem.IsViewMode = true;
                        }
                    }
                }
                else
                {
                    dispItem.IsViewMode = true;
                }

                string normalTxt = "", allTxt = "";
                CreateHistoryIncident_CTS330(incidentDat, out normalTxt, out allTxt);

                dispItem.RespondingProgress_Normal = normalTxt;
                dispItem.RespondingProgress_All = allTxt;
                List<string> exceptedEmpNo = new List<string>();

                if (!dispItem.IsViewMode)
                {
                    dispItem.CanChangeStatus = incidentDat.IsSpecialView;
                    if (incidentDat.IsSpecialView
                        || (incidentDat.incidentPermit.AssignChiefFlag)
                        || (incidentDat.incidentPermit.AssignCorrespondentFlag)
                        || (incidentDat.incidentPermit.AssignAssistantFlag))
                    {
                        dispItem.CanViewPIC = true;
                        dispItem.CanModPIC = true;

                        //foreach (var item in incidentDat.incidentData.dtIncidentRole)
                        //{
                        //    if ((item.EmpNo == CommonUtil.dsTransData.dtUserData.EmpNo) && !exceptedEmpNo.Contains(item.EmpNo))
                        //    {
                        //        exceptedEmpNo.Add(item.EmpNo);
                        //    }
                        //}

                        foreach (dtIncidentHistory item in incidentDat.incidentData.dtIncidentHistory)
                        {
                            var filterEmp = from a in incidentDat.incidentData.dtIncidentRole where a.EmpNo == item.CreateBy select a;
                            if ((filterEmp.Count() > 0) && !exceptedEmpNo.Contains(item.CreateBy))
                            {
                                exceptedEmpNo.Add(item.CreateBy);
                            }
                        }

                        foreach (tbt_IncidentHistoryDetail itemTbt in incidentDat.incidentData.Tbt_IncidentHistoryDetail)
                        {
                            var filterEmp = from a in incidentDat.incidentData.dtIncidentRole where a.EmpNo == itemTbt.CreateBy select a;
                            if ((filterEmp.Count() > 0) && !exceptedEmpNo.Contains(itemTbt.CreateBy))
                            {
                                exceptedEmpNo.Add(itemTbt.CreateBy);
                            }
                        }
                    }
                    else
                    {
                        dispItem.CanViewPIC = true;
                        dispItem.CanModPIC = false;
                    }
                }
                else
                {
                    dispItem.CanViewPIC = true;
                    dispItem.CanModPIC = false;
                }

                dispItem.ExceptedEmp = ", ";

                foreach (var item in exceptedEmpNo)
                {
                    dispItem.ExceptedEmp += item + ", ";
                }

                commonhandler.ClearTemporaryUploadFile(incidentID.ToString());
                sParam.delAttachLst = new Dictionary<int, string>();
                sParam.newAttachLst = new Dictionary<int, string>();

                res.ResultData = dispItem;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve incident data for view or edit with clearing data in memory
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS330_ReLoadIncidentData()
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                CTS330_ScreenParameter sParam = GetScreenObject<CTS330_ScreenParameter>();
                sParam.sessObj = null;
                sParam.newAttachLst = new Dictionary<int, string>();
                sParam.delAttachLst = new Dictionary<int, string>();

                commonhandler.ClearTemporaryUploadFile(sParam.strIncidentID);

                ActionResult trueRes = CTS330_LoadIncidentData();
                return trueRes;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial person in charge grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS330_InitialAssignPersonInChargeGrid()
        {
            ObjectResultData res = new ObjectResultData();
            int incidentID = int.Parse(GetScreenObject<CTS330_ScreenParameter>().strIncidentID);
            CTS330_SessionData incidentDat = GetIncidentData_CTS330(incidentID);
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                List<CTS330_PersonIncharge> gridLst = new List<CTS330_PersonIncharge>();
                List<string> miscType = new List<string>();

                miscType.Add(MiscType.C_INCIDENT_ROLE);

                var miscLst = commonhandler.GetMiscTypeCodeListByFieldName(miscType);
                // Dummie Data
                //if (incidentDat.incidentData.dtIncidentRole.Count == 0)
                //{
                //    dtIncidentRole tmpRole = new dtIncidentRole()
                //    {
                //        EmpNo = "11111",
                //        OfficeCode = "O1111",
                //        OfficeNameEN = "O1111_EN",
                //        OfficeNameJP = "O1111_JP",
                //        OfficeNameLC = "O1111_LC",
                //        DepartmentCode = "D11111",
                //        DepartmentName = "D11111_Name",
                //        IncidentRoleType = IncidentRole.C_INCIDENT_ROLE_ADMINISTRATOR,
                //        IncidentRoleTypeNameEN = "Admin_EN",
                //        IncidentRoleTypeNameJP = "Admin_JP",
                //        IncidentRoleTypeNameLC = "Admin_LC",
                //        IncidentRoleID = 1,
                //        EmpFirstNameEN = "FirstName_EN",
                //        EmpFirstNameLC = "FirstName_LC",
                //        EmpLastNameEN = "LastName_EN",
                //        EmpLastNameLC = "FirstName_LC",
                //    };

                //    incidentDat.incidentData.dtIncidentRole.Add(tmpRole);

                //    tmpRole = new dtIncidentRole()
                //    {
                //        EmpNo = "22222",
                //        OfficeCode = "O2222",
                //        OfficeNameEN = "O2222_EN",
                //        OfficeNameJP = "O2222_JP",
                //        OfficeNameLC = "O2222_LC",
                //        DepartmentCode = "D2222",
                //        DepartmentName = "D2222_Name",
                //        IncidentRoleType = IncidentRole.C_INCIDENT_ROLE_ADMINISTRATOR,
                //        IncidentRoleTypeNameEN = "Admin_EN 2",
                //        IncidentRoleTypeNameJP = "Admin_JP 2",
                //        IncidentRoleTypeNameLC = "Admin_LC 2",
                //        IncidentRoleID = 2,
                //        EmpFirstNameEN = "FirstName_EN 2",
                //        EmpFirstNameLC = "FirstName_LC 2",
                //        EmpLastNameEN = "LastName_EN 2",
                //        EmpLastNameLC = "FirstName_LC 2",
                //    };

                //    incidentDat.incidentData.dtIncidentRole.Add(tmpRole);
                //}

                foreach (var item in incidentDat.incidentData.dtIncidentRole)
                {
                    var tmpRoleLst = miscLst.Where(x => x.ValueCode == item.IncidentRoleType);

                    CTS330_PersonIncharge tmp = new CTS330_PersonIncharge()
                    {
                        DepartmentCode = item.DepartmentCode,
                        DepartmentName = item.DepartmentName,
                        EmpNo = item.EmpNo,
                        EmpName = item.EmpFirstName + " " + item.EmpLastName,
                        IncidentRoleCode = item.IncidentRoleType,
                        IncidentRoleName = (tmpRoleLst.Count() == 1) ? tmpRoleLst.First().ValueDisplay : "",
                        OfficeCode = item.OfficeCode,
                        OfficeName = item.OfficeName
                    };

                    gridLst.Add(tmp);
                }

                res.ResultData = CommonUtil.ConvertToXml<CTS330_PersonIncharge>(gridLst, "Contract\\CTS330_AssignPIC", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
            //return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS330_AssignPIC", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Initial attach document grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS330_IntialGridAttachedDocList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS330_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        }

        /// <summary>
        /// Retrieve attach document list
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS330_LoadGridAttachedDocList()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS330_ScreenParameter sParam = GetScreenObject<CTS330_ScreenParameter>();

                List<dtAttachFileForGridView> lstAttachedName = new List<dtAttachFileForGridView>();

                if (sParam != null && sParam.strIncidentID != null) //Add by Jutarat A. on 26022013
                    lstAttachedName = commonhandler.GetAttachFileForGridView(sParam.strIncidentID);

                //if (Session["CTS330_AttachFile"] == null)
                //{
                //    lstAttachedName = commonhandler.GetAttachFileName(GetCurrentKey(), null, false);
                //    Session.Add("CTS330_AttachFile", lstAttachedName);
                //}
                //else
                //{
                //    lstAttachedName = (List<dtAttachFileNameID>)Session["CTS330_AttachFile"];
                //}

                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Contract\\CTS330_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate and attach new document
        /// </summary>
        /// <param name="fileSelect"></param>
        /// <param name="DocumentName"></param>
        /// <param name="sParam"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult CTS330_AttachFile(HttpPostedFileBase fileSelect, string DocumentName, string sParam, string k)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                var param = GetScreenObject<CTS330_ScreenParameter>(k);

                if (fileSelect == null)
                {
                    // File not select
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0050, null);
                }

                if (String.IsNullOrEmpty(DocumentName))
                {
                    // DocName is not input
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS300", "lblDocumentName") });
                }

                byte[] fileData;

                using (BinaryReader reader = new BinaryReader(fileSelect.InputStream))
                {
                    var fList = commonhandler.GetAttachFileForGridView(sParam);

                    var filterDupItem = from a in fList where a.FileName.ToUpper().Equals(DocumentName.ToUpper() + Path.GetExtension(fileSelect.FileName).ToUpper()) select a;

                    if (filterDupItem.Count() > 0)
                    {
                        // Docname duplicate
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115, new string[] { DocumentName });
                        //outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115, new string[] { DocumentName });
                    }
                    else
                    {
                        fileData = reader.ReadBytes(fileSelect.ContentLength);

                        if (commonhandler.CanAttachFile(DocumentName, fileData.Length, Path.GetExtension(fileSelect.FileName), sParam, k))
                        {
                            DateTime currDate = DateTime.Now;
                            var attachedItem = commonhandler.InsertAttachFile(sParam
                            , DocumentName
                            , Path.GetExtension(fileSelect.FileName)
                            , fileData.Length
                            , fileData
                            , false);

                            param.newAttachLst.Add(attachedItem[0].AttachFileID, DocumentName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            if (res.IsError)
            {
                ViewBag.Message = res.Message.Message;
                ViewBag.MsgCode = res.Message.Code;
            }

            ViewBag.K = k;
            ViewBag.sKey = sParam;

            return View("CTS330_Upload");
        }

        /* Old code
        /// <summary>
        /// Download attach document
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult CTS330_DownloadAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                CTS330_ScreenParameter sParam = GetScreenObject<CTS330_ScreenParameter>();
                var downloadFileStream = commonhandler.GetAttachFileForDownload(int.Parse(AttachID), GetCurrentKey());
                var downloadFileName = commonhandler.GetTbt_AttachFile(sParam.strIncidentID, int.Parse(AttachID), null);
                //var downloadFileName = commonhandler.GetAttachFileName(sParam.strIncidentID, int.Parse(AttachID), null);
                //var attachFile = commonhandler.GetAttachFile(AttachmentModule.Incident, ReleateID, int.Parse(AttachID));
                //var fileNameLst = commonhandler.GetAttachFileName(ReleateID, int.Parse(AttachID), true);

                string fileName = downloadFileName[0].FileName;
                return File(downloadFileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }*/

        /// <summary>
        /// Download attach document
        /// </summary>
        /// <param name="AttachID"></param>
        public void CTS330_DownloadAttach(string AttachID)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            CTS330_ScreenParameter sParam = GetScreenObject<CTS330_ScreenParameter>();

            Stream downloadFileStream = commonhandler.GetAttachFileForDownload(int.Parse(AttachID), GetCurrentKey());
            List<tbt_AttachFile> downloadFileName = commonhandler.GetTbt_AttachFile(sParam.strIncidentID, int.Parse(AttachID), null);

            this.DownloadAllFile(downloadFileName[0].FileName, downloadFileStream);
        }

        /// <summary>
        /// Remove exists attach document
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult CTS330_RemoveAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                CTS330_ScreenParameter sParam = GetScreenObject<CTS330_ScreenParameter>();
                int _attachID = int.Parse(AttachID);

                var fileList = commonhandler.GetAttachFileForGridView(sParam.strIncidentID);
                var fileTarg = from a in fileList where a.AttachFileID == _attachID select a;
                string fileName = null;

                if (fileTarg.Count() == 1)
                {
                    fileName = fileTarg.First().FileName;
                }

                commonhandler.DeleteAttachFileByID(_attachID, sParam.strIncidentID);

                if (sParam.newAttachLst.ContainsKey(_attachID))
                {
                    sParam.newAttachLst.Remove(_attachID);
                }
                else if (!sParam.delAttachLst.ContainsKey(_attachID))
                {
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        sParam.delAttachLst.Add(_attachID, fileName);
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
        /// Clear all attach document
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS330_ClearAttach()
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                CTS330_ScreenParameter sParam = GetScreenObject<CTS330_ScreenParameter>();
                commonhandler.ClearTemporaryUploadFile(sParam.strIncidentID);
                sParam.delAttachLst = new Dictionary<int, string>();
                sParam.newAttachLst = new Dictionary<int, string>();
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial attach section
        /// </summary>
        /// <param name="sK"></param>
        /// <returns></returns>
        public ActionResult CTS330_Upload(string sK = "")
        {
            ViewBag.sKey = sK;
            ViewBag.K = GetCurrentKey();
            return View("CTS330_Upload");
        }

        /// <summary>
        /// Retrieve office item for combobox when initial screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS330_LoadOfficeCombobox()
        {
            ObjectResultData res = new ObjectResultData();
            int incidentID = int.Parse(GetScreenObject<CTS330_ScreenParameter>().strIncidentID);
            CTS330_SessionData incidentDat = GetIncidentData_CTS330(incidentID);
            CTS330_ComboboxData result = new CTS330_ComboboxData();

            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

            try
            {
                List<dtBelongingOffice> officeLstRaw = null;
                List<dtBelongingOfficeMap> officeList = null;

                if (incidentDat.IsSpecialView || incidentDat.incidentPermit.AssignChiefFlag)
                {
                    officeLstRaw = emphandler.GetBelongingOfficeList(null);
                }
                else if ((incidentDat.incidentPermit.AssignCorrespondentFlag || incidentDat.incidentPermit.AssignAssistantFlag)
                    && (incidentDat.incidentData.dtIncidentRole.Where(x => x.EmpNo == CommonUtil.dsTransData.dtUserData.EmpNo).Count() == 1))
                {
                    officeLstRaw = emphandler.GetBelongingOfficeList(CommonUtil.dsTransData.dtUserData.EmpNo);
                }

                officeList = CommonUtil.ClonsObjectList<dtBelongingOffice, dtBelongingOfficeMap>(officeLstRaw);
                if (officeList != null)
                    CommonUtil.MappingObjectLanguage<dtBelongingOfficeMap>(officeList);
                result.CBBMarkup = CommonUtil.CommonComboBox<dtBelongingOfficeMap>("{BlankID}", officeList, "OfficeCodeName", "OfficeCode", null, true).ToString();

                if ((officeLstRaw != null) && (officeLstRaw.Count > 0))
                {
                    result.IsEnablePIC = true;
                }
                else
                {
                    //result.CBBMarkup = String.Empty;
                    result.IsEnablePIC = false;
                }

                res.ResultData = result;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve department item for combobox when change select [Office] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="strOfficeCode"></param>
        /// <returns></returns>
        public ActionResult CTS330_LoadDepartmentCombobox(string strOfficeCode)
        {
            ObjectResultData res = new ObjectResultData();
            int incidentID = int.Parse(GetScreenObject<CTS330_ScreenParameter>().strIncidentID);
            CTS330_SessionData incidentDat = GetIncidentData_CTS330(incidentID);

            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler; //Add by jutarat A. on 20082012
            List<dtDepartment> dat = null;

            CTS330_ScreenParameter param = GetScreenObject<CTS330_ScreenParameter>(); //Add by jutarat A. on 20082012

            try
            {
                //Add by jutarat A. on 20082012
                if (String.IsNullOrEmpty(strOfficeCode))
                {
                    res.ResultData = String.Empty;
                }
                else
                {
                    param.blnIncidentIsHeadOfficeFlag = officehandler.CheckHeadOffice(strOfficeCode);
                    if (param.blnIncidentIsHeadOfficeFlag == false)
                    {
                        res.ResultData = false;
                    }
                    //End Add
                    else
                    {
                        if (incidentDat.IsSpecialView || (incidentDat.incidentPermit.AssignChiefFlag == FlagType.C_FLAG_ON))
                        {
                            dat = emphandler.GetBelongingDepartmentList(strOfficeCode, null);
                        }
                        else if ((incidentDat.incidentPermit.AssignCorrespondentFlag || incidentDat.incidentPermit.AssignAssistantFlag)
                           && (incidentDat.incidentData.dtIncidentRole.Where(x => x.EmpNo == CommonUtil.dsTransData.dtUserData.EmpNo).Count() == 1))
                        {
                            dat = emphandler.GetBelongingDepartmentList(strOfficeCode, incidentDat.incidentData.dtIncidentRole.Where(x => x.EmpNo == CommonUtil.dsTransData.dtUserData.EmpNo).ToList()[0].DepartmentCode);
                        }

                        if (dat != null && dat.Count > 0)
                        {
                            foreach (dtDepartment data in dat)
                            {
                                data.DepartmentName = data.DepartmentCode + " : " + data.DepartmentName;
                            }
                        }

                        if ((dat != null) || (dat.Count > 0))
                        {
                            res.ResultData = CommonUtil.CommonComboBox<dtDepartment>("{BlankID}", dat, "DepartmentName", "DepartmentCode", null, true).ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve incident role item for combobox when change select [Department] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="strOfficeCode"></param>
        /// <param name="strDepartmentCode"></param>
        /// <returns></returns>
        public ActionResult CTS330_LoadIncidentRoleCombobox(string strOfficeCode, string strDepartmentCode)
        {
            ObjectResultData res = new ObjectResultData();
            int incidentID = int.Parse(GetScreenObject<CTS330_ScreenParameter>().strIncidentID);
            CTS330_SessionData incidentDat = GetIncidentData_CTS330(incidentID);
            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                //if (incidentDat.incidentPermit.AssignAssistantFlag)
                //{
                    lst.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_INCIDENT_ROLE,
                        ValueCode = IncidentRole.C_INCIDENT_ROLE_ADMINISTRATOR
                    });
                    lst.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_INCIDENT_ROLE,
                        ValueCode = IncidentRole.C_INCIDENT_ROLE_ASSISTANT
                    });
                    lst.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_INCIDENT_ROLE,
                        ValueCode = IncidentRole.C_INCIDENT_ROLE_CHIEF
                    });
                    lst.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_INCIDENT_ROLE,
                        ValueCode = IncidentRole.C_INCIDENT_ROLE_CHIEF_OF_RELATED_OFFICE
                    });
                    lst.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_INCIDENT_ROLE,
                        ValueCode = IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF
                    });
                    lst.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_INCIDENT_ROLE,
                        ValueCode = IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT
                    });

                    //if (incidentDat.IsSpecialView)
                    //{
                    //    lst.RemoveAll(x => x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF);
                    //}
                    //else
                    //{
                    //    lst.RemoveAll(x => (x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF) || (x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CHIEF));
                    //}
                    //if (!incidentDat.incidentPermit.AssignCorrespondentFlag)
                    //{
                    //    lst.RemoveAll(x => (x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF) || (x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CHIEF) || (x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT));
                    //}

                    if (!incidentDat.IsSpecialView)
                    {
                        if (incidentDat.incidentPermit.AssignChiefFlag)
                        {
                            lst.RemoveAll(x => x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF);         
                        }
                        else
                        {
                            lst.RemoveAll(x => (x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF) || (x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CHIEF));
                        }

                        if (!incidentDat.incidentPermit.AssignCorrespondentFlag)
                        {
                            lst.RemoveAll(x => (x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF) || (x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CHIEF) || (x.ValueCode == IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT));
                        }
                    }

                    
                //}

                var outlst = hand.GetMiscTypeCodeList(lst);
                if (outlst == null)
                    outlst = new List<doMiscTypeCode>();

                string display = "ValueCodeDisplay";
                res.ResultData = CommonUtil.CommonComboBox<doMiscTypeCode>("{BlankID}", outlst, display, "ValueCode", null, true).ToString();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve employee item for combobox when change select [IncidentRole] drop down list on Assign person in charge information subsection
        /// </summary>
        /// <param name="strOfficeCode"></param>
        /// <param name="strDepartmentCode"></param>
        /// <param name="strIncidentRoleCode"></param>
        /// <returns></returns>
        public ActionResult CTS330_LoadEmployeeCombobox(string strOfficeCode, string strDepartmentCode, string strIncidentRoleCode)
        {
            ObjectResultData res = new ObjectResultData();
            int incidentID = int.Parse(GetScreenObject<CTS330_ScreenParameter>().strIncidentID);
            CTS330_SessionData incidentDat = GetIncidentData_CTS330(incidentID);
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

            CTS330_ScreenParameter param = GetScreenObject<CTS330_ScreenParameter>(); //Add by jutarat A. on 20082012

            try
            {
                var datTemp = emphandler.GetBelongingEmpList(strOfficeCode
                    , param.blnIncidentIsHeadOfficeFlag == false ? null : strDepartmentCode //strDepartmentCode  //Modify by jutarat A. on 20082012
                    , ((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CHIEF) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF)) ? FlagType.C_FLAG_ON : (bool?)null
                    , null
                    , ((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_ASSISTANT)) ? FlagType.C_FLAG_ON : (bool?)null
                    );

                //Add by jutarat A. on 21082012
                var datDist = (from t in datTemp
                               group t by new
                               {
                                   EmpNo = t.EmpNo,
                                   EmpFirstNameEN = t.EmpFirstNameEN,
                                   EmpLastNameEN = t.EmpLastNameEN,
                                   EmpFirstNameLC = t.EmpFirstNameLC,
                                   EmpLastNameLC = t.EmpLastNameLC
                               } into g
                               select g.FirstOrDefault());

                List<dtEmployeeBelonging> empLst = datDist.ToList<dtEmployeeBelonging>();
                //End Add

                if (empLst != null && empLst.Count > 0 )
                {
                    foreach(dtEmployeeBelonging data in empLst)
                    {
                        data.EmpFirstName = data.EmpNo + " : " + data.EmpFullNameEN; 
                    }
                }

                if (param.blnIncidentIsHeadOfficeFlag) //Add by jutarat A. on 20082012
                {
                    if (((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CHIEF) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF)) && (empLst.Count < 1))
                    {
                        res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3184, null, null);
                    }
                    else if (((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CHIEF) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF)) && (empLst.Count > 1))
                    {
                        res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3183, null, null);
                    }
                    else if ((strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CHIEF) || (strIncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF))
                    {
                        if (CommonUtil.CurrentLanguage(false) == CommonUtil.LANGUAGE_LIST.LANGUAGE_1)
                        {
                            res.ResultData = CommonUtil.CommonComboBox<dtEmployeeBelonging>("{BlankID}", empLst, "EmpFirstName", "EmpNo", null, false).ToString();
                        }
                        else
                        {
                            res.ResultData = CommonUtil.CommonComboBox<dtEmployeeBelonging>("{BlankID}", empLst, "EmpFirstName", "EmpNo", null, false).ToString();
                        }
                    } 
                    else
                    {
                        if (CommonUtil.CurrentLanguage(false) == CommonUtil.LANGUAGE_LIST.LANGUAGE_1)
                        {
                            res.ResultData = CommonUtil.CommonComboBox<dtEmployeeBelonging>("{BlankID}", empLst, "EmpFirstName", "EmpNo", null, true).ToString();
                        }
                        else
                        {
                            res.ResultData = CommonUtil.CommonComboBox<dtEmployeeBelonging>("{BlankID}", empLst, "EmpFirstName", "EmpNo", null, true).ToString();
                        }
                    }
                }
                //Add by jutarat A. on 20082012
                else
                {
                    if (CommonUtil.CurrentLanguage(false) == CommonUtil.LANGUAGE_LIST.LANGUAGE_1)
                    {
                        res.ResultData = CommonUtil.CommonComboBox<dtEmployeeBelonging>("{BlankID}", empLst, "EmpFirstName", "EmpNo", null, true).ToString();
                    }
                    else
                    {
                        res.ResultData = CommonUtil.CommonComboBox<dtEmployeeBelonging>("{BlankID}", empLst, "EmpFirstName", "EmpNo", null, true).ToString();
                    }
                }
                //End Add
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve interaction type item for combobox when initial screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS330_LoadInteractionCombobox()
        {
            ObjectResultData res = new ObjectResultData();
            //int incidentID = int.Parse(GetIncidentIDFromParameter_CTS330());
            int incidentID = int.Parse(GetScreenObject<CTS330_ScreenParameter>().strIncidentID);
            CTS330_SessionData incidentDat = GetIncidentData_CTS330(incidentID);
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                if (incidentDat.IsSpecialView || (incidentDat.incidentPermit.IncidentInteractionTypeList.Contains(InteractionTypeAdministrator.C_INTERACTION_TYPE_ADMINISTRATOR)))
                {
                    miscs.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_INCIDENT_INTERACTION_TYPE,
                        ValueCode = IncidentInteractionType.C_INCIDENT_INTERACTION_TYPE_REGISTER_BY_ADMIN
                    });
                }
                else if (incidentDat.incidentPermit.IncidentInteractionTypeList.Contains(InteractionTypeChief.C_INTERACTION_TYPE_CONTROL_CHIEF))
                {
                    int min = int.Parse(InteractionTypeControlChief.C_INTERACTION_TYPE_CONTROL_CHIEF_MIN);
                    int max = int.Parse(InteractionTypeControlChief.C_INTERACTION_TYPE_CONTROL_CHIEF_MAX);
                    for (int i = min; i <= max; i++)
                    {
                        miscs.Add(new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_INCIDENT_INTERACTION_TYPE,
                            ValueCode = i.ToString("00")
                        });
                    }

                    miscs.Add(new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_INCIDENT_INTERACTION_TYPE,
                        ValueCode = IncidentInteractionType.C_INCIDENT_INTERACTION_TYPE_NOTIFY
                    });
                }
                else if (incidentDat.incidentPermit.IncidentInteractionTypeList.Contains(InteractionTypeChief.C_INTERACTION_TYPE_CHIEF))
                {
                    int min = int.Parse(InteractionTypeChief.C_INTERACTION_TYPE_CHIEF_MIN);
                    int max = int.Parse(InteractionTypeMax.C_INTERACTION_TYPE_MAX);
                    for (int i = min; i <= max; i++)
                    {
                        miscs.Add(new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_INCIDENT_INTERACTION_TYPE,
                            ValueCode = i.ToString("00")
                        });
                    }
                }
                else if (incidentDat.incidentPermit.IncidentInteractionTypeList.Contains(InteractionTypeCorrespondent.C_INTERACTION_TYPE_CORRESPONDENT))
                {
                    int min = int.Parse(InteractionTypeCorrespondent.C_INTERACTION_TYPE_CORRESPONDENT_MIN);
                    int max = int.Parse(InteractionTypeMax.C_INTERACTION_TYPE_MAX);
                    for (int i = min; i <= max; i++)
                    {
                        miscs.Add(new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_INCIDENT_INTERACTION_TYPE,
                            ValueCode = i.ToString("00")
                        });
                    }
                }

                var outlst = commonhandler.GetMiscTypeCodeList(miscs);
                if (outlst == null)
                    outlst = new List<doMiscTypeCode>();

                string display = "ValueCodeDisplay";
                res.ResultData = CommonUtil.CommonComboBox<doMiscTypeCode>("{BlankID}", outlst, display, "ValueCode", null, true).ToString();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve incident status item for combobox when change select [InteractionType] button
        /// </summary>
        /// <param name="strInteractionTypeCode"></param>
        /// <returns></returns>
        public ActionResult CTS330_LoadSelectStatusCombobox(string strInteractionTypeCode)
        {
            ObjectResultData res = new ObjectResultData();
            int incidentID = int.Parse(GetScreenObject<CTS330_ScreenParameter>().strIncidentID);
            CTS330_SessionData incidentDat = GetIncidentData_CTS330(incidentID);

            try
            {
                string outVal = "";

                if (incidentDat.IsSpecialView)
                {
                    outVal = IncidentStatus.C_INCIDENT_STATUS_INCIDENT_CHIEF_RESPONDING;
                }
                else if (strInteractionTypeCode == IncidentInteractionType.C_INCIDENT_INTERACTION_TYPE_ANSWER)
                {
                    outVal = IncidentStatus.C_INCIDENT_STATUS_HAVE_REPLY_FROM_CHIEF;
                }
                else if (strInteractionTypeCode == IncidentInteractionType.C_INCIDENT_INTERACTION_TYPE_INSTRUCTION)
                {
                    outVal = IncidentStatus.C_INCIDENT_STATUS_HAVE_UNREAD_INSTRUCTION;
                }
                else if (strInteractionTypeCode == IncidentInteractionType.C_INCIDENT_INTERACTION_TYPE_INCIDENT_CHIEF_RESPONSE)
                {
                    outVal = IncidentStatus.C_INCIDENT_STATUS_INCIDENT_CHIEF_RESPONDING;
                }
                else if (strInteractionTypeCode == IncidentInteractionType.C_INCIDENT_INTERACTION_TYPE_APPROVE_COMPLETION)
                {
                    outVal = IncidentStatus.C_INCIDENT_STATUS_COMPLETE;
                }
                else if (strInteractionTypeCode == IncidentInteractionType.C_INCIDENT_INTERACTION_TYPE_REPORT)
                {
                    outVal = IncidentStatus.C_INCIDENT_STATUS_REPORT;
                }
                else if (strInteractionTypeCode == IncidentInteractionType.C_INCIDENT_INTERACTION_TYPE_REQUEST_INSTRUCTION)
                {
                    outVal = IncidentStatus.C_INCIDENT_STATUS_WAIT_FOR_INSTRUCTION;
                }
                else if (strInteractionTypeCode == IncidentInteractionType.C_INCIDENT_INTERACTION_TYPE_REPORT_COMPLETION)
                {
                    outVal = IncidentStatus.C_INCIDENT_STATUS_WAIT_FOR_COMPLETE_APPROVAL;
                }
                else if (strInteractionTypeCode == IncidentInteractionType.C_INCIDENT_INTERACTION_TYPE_ACKNOWLEDGE)
                {
                    outVal = IncidentStatus.C_INCIDENT_STATUS_RESPONDING;
                }

                if (string.IsNullOrEmpty(outVal))
                {
                    outVal = incidentDat.incidentData.dtIncident.IncidentStatus;
                }

                res.ResultData = outVal;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate require field and business
        /// </summary>
        /// <param name="entryDat"></param>
        /// <returns></returns>
        public ActionResult CTS330_ValidateBusiness(CTS330_EntryData entryDat)
        {
            ObjectResultData res = new ObjectResultData();

            List<string> controlLst = new List<string>()
            , labelList = new List<string>();

            try
            {
                if (CheckIsSuspending(res))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                    return Json(res);
                }

                if (String.IsNullOrEmpty(entryDat.InteractionType))
                {
                    controlLst.Add("InteractionType");
                    labelList.Add("lblInteractionType");
                }

                if (entryDat.DueDateDeadLineType == "1")
                {
                    if (!entryDat.DueDate_Date.HasValue || !entryDat.DueDate_Time.HasValue)
                    {
                        if (!entryDat.DueDate_Date.HasValue)
                            controlLst.Add("DueDate_Date");

                        if (!entryDat.DueDate_Time.HasValue)
                            controlLst.Add("DueDate_Time");

                        labelList.Add("lblDueDate_Date");
                    }
                }
                else if (entryDat.DueDateDeadLineType == "2")
                {
                    if (!entryDat.Deadline_Date.HasValue || String.IsNullOrEmpty(entryDat.Deadline_Until))
                    {
                        if (!entryDat.Deadline_Date.HasValue)
                            controlLst.Add("Deadline_Date");

                        if (String.IsNullOrEmpty(entryDat.Deadline_Until))
                            controlLst.Add("Deadline_Until");

                        labelList.Add("lblDeadline_Date");
                    }
                }

                if (labelList.Count > 0)
                {
                    var labelTextList = new List<string>();

                    foreach (var lblName in labelList)
                    {
                        labelTextList.Add(CommonUtil.GetLabelFromResource("Contract", "CTS330", lblName));
                    }

                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.TextList(labelTextList.ToArray(), ", ") }, controlLst.ToArray());
                }

                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                int incidentID = int.Parse(GetScreenObject<CTS330_ScreenParameter>().strIncidentID);
                CTS330_SessionData incidentDat = GetIncidentData_CTS330(incidentID);

                //if (incidentDat.incidentData.dtIncidentRole.Count == 0)
                //{
                //    // If old not have
                //    if (entryDat.IncidentRoleList.Count > 0)
                //    {
                //        // And new have
                //        var filterControlChief = entryDat.IncidentRoleList.Where(x => x.IncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF);
                //        if (filterControlChief.Count() == 0)
                //        {
                //            // But don't have CONTROL CHIEF
                //            // Alert Here. "Must have chief"

                //            res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3276, null, null);
                //            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //            return Json(res);
                //        }
                //    }
                //}
                //else
                //{
                //    // If old have
                //    var filterControlChief = entryDat.IncidentRoleList.Where(x => x.IncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF);
                //    if (filterControlChief.Count() == 0)
                //    {
                //        // But don't have CONTROL CHIEF
                //        // Alert Here. "Don't remove chief"

                //        res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3277, null, null);
                //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //        return Json(res);
                //    }
                //}
                if (entryDat.IncidentRoleList != null && entryDat.IncidentRoleList.Count > 0)
                {
                    var filterControlChief = entryDat.IncidentRoleList.Where(x => x.IncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF);
                    if (filterControlChief.Count() == 0)
                    {
                        // If old not have
                        // And new have
                        if (incidentDat.incidentData.dtIncidentRole.Count == 0)
                        {
                            // But don't have CONTROL CHIEF
                            // Alert Here. "Must have chief"
                            res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3276, null, null);
                        }
                        else
                        {
                            // But don't have CONTROL CHIEF
                            // Alert Here. "Don't remove chief"
                            res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3277, null, null);
                        }

                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
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
        /// Edit incident data
        /// </summary>
        /// <param name="entryDat"></param>
        /// <returns></returns>
        public ActionResult CTS330_RegisterData(CTS330_EntryData entryDat)
        {
            ObjectResultData res = new ObjectResultData();
            

            try
            {
                if (CheckIsSuspending(res))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                    return Json(res);
                }

                res.ResultData = EditIncident_CTS330(entryDat);
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
        /// Edit incident data
        /// </summary>
        /// <param name="entryDat"></param>
        /// <returns></returns>
        private bool EditIncident_CTS330(CTS330_EntryData entryDat)
        {
            bool result = false;

            try
            {
                int incidentID = int.Parse(GetScreenObject<CTS330_ScreenParameter>().strIncidentID);
                CTS330_SessionData incidentDat = GetIncidentData_CTS330(incidentID);
                CommonUtil util = new CommonUtil();
                dsIncidentDetail currDat = incidentDat.incidentData;
                dsIncidentDetailIn updDat = new dsIncidentDetailIn();

                List<tbt_IncidentRole> addRoleLst = new List<tbt_IncidentRole>()
                    , editRoleLst = new List<tbt_IncidentRole>();
                List<int> delRoleLst = new List<int>();

                List<tbt_IncidentHistoryDetail> changeList = new List<tbt_IncidentHistoryDetail>();

                IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
                ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IEmployeeMasterHandler emphandper = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                CTS330_ScreenParameter sParam = GetScreenObject<CTS330_ScreenParameter>();

                // Update to incidentDat
                if (entryDat.HistoryList == null)
                {
                    entryDat.HistoryList = new List<CTS330_HistoryLog>();
                }

                if (entryDat.IncidentRoleList == null)
                {
                    entryDat.IncidentRoleList = new List<CTS330_PersonIncharge>();
                }

                if (entryDat.OriginList == null)
                {
                    entryDat.OriginList = new List<CTS330_HistoryLog>();
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    updDat.dtIncident = currDat.dtIncident;
                    updDat.dtIncident.hasRespondingDetailFlag = (currDat.dtIncident.hasRespondingDetailFlag.GetValueOrDefault() || (!string.IsNullOrEmpty(entryDat.CurrentRespondingDetail)));
                    updDat.dtIncident.IncidentStatus = entryDat.StatusAfterUpdate;
                    updDat.dtIncident.InteractionType = entryDat.InteractionType;
                    updDat.tbt_IncidentHistory = new tbt_IncidentHistory()
                    {
                        InteractionType = entryDat.InteractionType,
                        IncidentID = currDat.dtIncident.IncidentID,
                        RespondingDetail = entryDat.CurrentRespondingDetail,
                    };

                    foreach (var newAttachItem in sParam.newAttachLst)
                    {
                        changeList.Add(new tbt_IncidentHistoryDetail()
                        {
                            ChangeItemName = "Attached document",
                            ItemNewValue = newAttachItem.Value,
                            ItemOldValue = "Add"
                        });
                    }

                    foreach (var delAttachItem in sParam.delAttachLst)
                    {
                        changeList.Add(new tbt_IncidentHistoryDetail()
                        {
                            ChangeItemName = "Attached document",
                            ItemNewValue = "Delete",
                            ItemOldValue = delAttachItem.Value,
                        });
                    }

                    string currDueDateType = (incidentDat.incidentData.dtIncident.DueDate.HasValue) ? "1" : "2";
                    if ((entryDat.DueDateDeadLineType == currDueDateType) && (currDueDateType == "1"))
                    {
                        if ((entryDat.DueDate_Date != currDat.dtIncident.DueDate) || (entryDat.DueDate_Time != currDat.dtIncident.DueDateTime))
                        {
                            // Due date Changed
                            changeList.Add(new tbt_IncidentHistoryDetail()
                            {
                                ChangeItemName = "DueDate",
                                ItemNewValue = CommonUtil.TextDate(entryDat.DueDate_Date) + " " + CommonUtil.TextTime(entryDat.DueDate_Time),
                                ItemOldValue = CommonUtil.TextDate(currDat.dtIncident.DueDate) + " " + CommonUtil.TextTime(currDat.dtIncident.DueDateTime),
                            });
                        }
                    }
                    else if ((entryDat.DueDateDeadLineType == currDueDateType) && (currDueDateType == "2"))
                    {
                        if ((entryDat.Deadline_Date != currDat.dtIncident.DeadLine) || (entryDat.Deadline_Until != currDat.dtIncident.DeadLineTime))
                        {
                            // Due date Changed
                            string strOldVal = "", strNewVal = "";
                            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_DEADLINE_TIME_TYPE,
                                ValueCode = entryDat.Deadline_Until
                            });

                            miscs.Add(new doMiscTypeCode()
                            {
                                FieldName = MiscType.C_DEADLINE_TIME_TYPE,
                                ValueCode = currDat.dtIncident.DeadLineTime
                            });

                            var outlst = commonhandler.GetMiscTypeCodeList(miscs);

                            var newObj = from a in outlst where a.ValueCode == entryDat.Deadline_Until select a;
                            var oldObj = from a in outlst where a.ValueCode == currDat.dtIncident.DeadLineTime select a;

                            strNewVal = newObj.First().ValueDisplay;
                            strOldVal = oldObj.First().ValueDisplay;

                            if ((outlst != null) && (outlst.Count == 2))
                            {
                                CommonUtil.MappingObjectLanguage<doMiscTypeCode>(outlst);
                                changeList.Add(new tbt_IncidentHistoryDetail()
                                {
                                    ChangeItemName = "Deadline",
                                    ItemNewValue = CommonUtil.TextDate(entryDat.Deadline_Date) + " " + strNewVal,
                                    ItemOldValue = CommonUtil.TextDate(currDat.dtIncident.DeadLine) + " " + strOldVal,
                                });
                            }
                        }
                    }
                    else if ((entryDat.DueDateDeadLineType == "2") && (currDueDateType == "1"))
                    {
                        List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                        miscs.Add(new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_DEADLINE_TIME_TYPE,
                            ValueCode = entryDat.Deadline_Until
                        });

                        var outlst = commonhandler.GetMiscTypeCodeList(miscs);

                        if ((outlst != null) && (outlst.Count == 1))
                        {
                            CommonUtil.MappingObjectLanguage<doMiscTypeCode>(outlst);
                            changeList.Add(new tbt_IncidentHistoryDetail()
                            {
                                ChangeItemName = "DueDate",
                                ItemNewValue = "DeadLine " + CommonUtil.TextDate(entryDat.Deadline_Date) + " " + outlst[0].ValueDisplay,
                                ItemOldValue = "DueDate " + CommonUtil.TextDate(currDat.dtIncident.DueDate) + " " + CommonUtil.TextTime(currDat.dtIncident.DueDateTime),
                            });
                        }
                    }
                    else if ((entryDat.DueDateDeadLineType == "1") && (currDueDateType == "2"))
                    {
                        List<doMiscTypeCode> miscs = new List<doMiscTypeCode>();
                        miscs.Add(new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_DEADLINE_TIME_TYPE,
                            ValueCode = currDat.dtIncident.DeadLineTime
                        });

                        var outlst = commonhandler.GetMiscTypeCodeList(miscs);

                        if ((outlst != null) && (outlst.Count == 1))
                        {
                            CommonUtil.MappingObjectLanguage<doMiscTypeCode>(outlst);
                            changeList.Add(new tbt_IncidentHistoryDetail()
                            {
                                ChangeItemName = "DeadLine",
                                ItemNewValue = "DueDate " + CommonUtil.TextDate(entryDat.DueDate_Date) + " " + CommonUtil.TextTime(entryDat.DueDate_Time),
                                ItemOldValue = "DeadLine " + CommonUtil.TextDate(currDat.dtIncident.DeadLine) + " " + outlst[0].ValueDisplay,
                            });
                        }
                    }

                    if (entryDat.DueDateDeadLineType == "1")
                    {
                        updDat.dtIncident.DueDate = entryDat.DueDate_Date;
                        updDat.dtIncident.DueDateTime = entryDat.DueDate_Time;
                        updDat.dtIncident.DeadLine = null;
                        updDat.dtIncident.DeadLineTime = null;
                    }
                    else
                    {
                        updDat.dtIncident.DueDate = null;
                        updDat.dtIncident.DueDateTime = null;
                        updDat.dtIncident.DeadLine = entryDat.Deadline_Date;
                        updDat.dtIncident.DeadLineTime = entryDat.Deadline_Until;
                    }

                    List<string> miscNameList = new List<string>();
                    miscNameList.Add(MiscType.C_INCIDENT_ROLE);
                    var miscListFull = commonhandler.GetMiscTypeCodeListByFieldName(miscNameList);
                    var currentRoleList = incidentDat.incidentData.dtIncidentRole;

                    foreach (CTS330_HistoryLog item in entryDat.HistoryList)
                    {
                        //var currRoleItem = (from a in entryDat.IncidentRoleList where a.EmpNo == item.EmpNo select a).ToList();
                        var currEmp = emphandper.GetTbm_Employee(item.EmpNo);
                        var currRoleName = (from a in miscListFull where a.ValueCode == item.IncidentRoleCode select a).ToList();

                        CommonUtil.MappingObjectLanguage<tbm_Employee>(currEmp);
                        CommonUtil.MappingObjectLanguage<doMiscTypeCode>(currRoleName);

                        if (item.FunctionType == FunctionID.C_FUNC_ID_ADD)
                        {
                            addRoleLst.Add(new tbt_IncidentRole()
                                {
                                    DepartmentCode = item.DepartmentCode,
                                    EmpNo = item.EmpNo,
                                    IncidentID = currDat.dtIncident.IncidentID,
                                    IncidentRoleType = item.IncidentRoleCode,
                                    OfficeCode = item.OfficeCode,
                                });

                            changeList.Add(new tbt_IncidentHistoryDetail()
                            {
                                ChangeItemName = "Person in charge " + currRoleName[0].ValueDisplay,
                                ItemOldValue = "Add",
                                ItemNewValue = currEmp[0].EmpFullName
                            });
                        }
                        else if (item.FunctionType == FunctionID.C_FUNC_ID_EDIT)
                        {
                            editRoleLst.Add(new tbt_IncidentRole()
                            {
                                DepartmentCode = item.DepartmentCode,
                                EmpNo = item.EmpNo,
                                IncidentID = currDat.dtIncident.IncidentID,
                                IncidentRoleType = item.IncidentRoleCode,
                                OfficeCode = item.OfficeCode,
                            });

                            changeList.Add(new tbt_IncidentHistoryDetail()
                            {
                                ChangeItemName = "Person in charge " + currRoleName[0].ValueDisplay,
                                ItemOldValue = currEmp[0].EmpFullName,
                                ItemNewValue = "Edit"
                            });
                        }
                        else if (item.FunctionType == FunctionID.C_FUNC_ID_DEL)
                        {
                            var delItem = from a in currentRoleList where a.EmpNo == item.EmpNo select a;
                            delRoleLst.Add(delItem.First().IncidentRoleID);

                            changeList.Add(new tbt_IncidentHistoryDetail()
                            {
                                ChangeItemName = "Person in charge " + currRoleName[0].ValueDisplay,
                                ItemOldValue = currEmp[0].EmpFullName,
                                ItemNewValue = "Remove"
                            });
                        }
                    }

                    List<CTS330_HistoryLog> trueDelList = new List<CTS330_HistoryLog>();
                    foreach (var item in entryDat.OriginList)
                    {
                        //var filterItem = from a in entryDat.HistoryList where a.EmpNo == item.EmpNo select a;

                        if (item.FunctionType == FunctionID.C_FUNC_ID_DEL)
                        {
                            trueDelList.Add(item);

                            var currRoleItem = (from a in entryDat.IncidentRoleList where a.EmpNo == item.EmpNo select a).ToList();
                            var currEmp = emphandper.GetTbm_Employee(currRoleItem[0].EmpNo);
                            var currRoleName = (from a in miscListFull where a.ValueCode == currRoleItem[0].IncidentRoleCode select a).ToList();

                            var delItem = from a in currentRoleList where a.EmpNo == item.EmpNo select a;
                            delRoleLst.Add(delItem.First().IncidentRoleID);

                            CommonUtil.MappingObjectLanguage<tbm_Employee>(currEmp);
                            CommonUtil.MappingObjectLanguage<doMiscTypeCode>(currRoleName);

                            changeList.Add(new tbt_IncidentHistoryDetail()
                            {
                                ChangeItemName = "Person in charge " + currRoleName[0].ValueDisplay,
                                ItemOldValue = currEmp[0].EmpFullName,
                                ItemNewValue = "Remove"
                            });
                        }
                    }

                    entryDat.HistoryList.AddRange(trueDelList);

                    if (String.IsNullOrEmpty(updDat.dtIncident.IncidentNo))
                    {
                        if (entryDat.IncidentRoleList != null)
                        {
                            var chiefRole = from a in entryDat.IncidentRoleList where a.IncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF select a;
                            if (chiefRole.Count() > 0)
                            {
                                var newCode = incidenthandler.GenerateIncidentNo(currDat.dtIncident.IncidentRelavantType
                                    , (currDat.dtIncident.IncidentRelavantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                                        ? currDat.dtIncident.CustCode
                                        : (currDat.dtIncident.IncidentRelavantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
                                        ? currDat.dtIncident.SiteCode
                                        : (currDat.dtIncident.IncidentRelavantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
                                        ? currDat.dtIncident.ProjectCode
                                        : currDat.dtIncident.ContractCode
                                    , ((currDat.dtIncident.IncidentRelavantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                                    || (currDat.dtIncident.IncidentRelavantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT))
                                        ? chiefRole.ToList()[0].OfficeCode
                                        : null
                                    );

                                updDat.dtIncident.IncidentNo = newCode[0];
                                updDat.dtIncident.IncidentOfficeCode = newCode[1];
                            }
                        }
                    }

                    if ((incidentDat.incidentData.dtIncident.IncidentRelavantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                        || (incidentDat.incidentData.dtIncident.IncidentRelavantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT))
                    {
                        var lastControlChief = from a in entryDat.IncidentRoleList where a.IncidentRoleCode == IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF select a;
                        if ((lastControlChief != null) && (lastControlChief.Count() > 0))
                        {
                            var personInCharge = lastControlChief.First();
                            updDat.dtIncident.IncidentOfficeCode = personInCharge.OfficeCode;
                        }
                    }

                    updDat.tbt_IncidentRoleAdd = addRoleLst;
                    updDat.tbt_IncidentRoleEdit = editRoleLst;
                    updDat.tbt_IncidentRoleDelete = delRoleLst;
                    updDat.tbt_IncidentHistoryDetail = changeList;

                    // Update
                    result = incidenthandler.UpdateIncidentDetail(updDat);


                    if (result)
                    {
                        UpdateIncidentData_CTS330(updDat.dtIncident.IncidentID);
                    }

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        //private String GetNameFromLang(string strEN, string strJP = null, string strLC = null)
        //{
        //    if (CommonUtil.CurrentLanguage() == CommonUtil.LANGUAGE_LIST.LANGUAGE_1)
        //    {
        //        return strEN;
        //    }
        //    else if (CommonUtil.CurrentLanguage() == CommonUtil.LANGUAGE_LIST.LANGUAGE_2)
        //    {
        //        if (strJP == null)
        //        {
        //            return strEN;
        //        }
        //        else
        //        {
        //            return strJP;
        //        }
        //    }
        //    else
        //    {
        //        if (strLC == null)
        //        {
        //            return strEN;
        //        }
        //        else
        //        {
        //            return strLC;
        //        }
        //    }
        //}

        /// <summary>
        /// Retrieve incident data from memory
        /// </summary>
        /// <param name="incidentID"></param>
        /// <returns></returns>
        private CTS330_SessionData GetIncidentData_CTS330(int incidentID)
        {
            CTS330_ScreenParameter sParam = GetScreenObject<CTS330_ScreenParameter>();
            CTS330_SessionData incidentDat = null;

            if (sParam.sessObj == null)
            {
                UpdateIncidentData_CTS330(int.Parse(sParam.strIncidentID));
                sParam = GetScreenObject<CTS330_ScreenParameter>();
            }

            incidentDat = sParam.sessObj;
            return incidentDat;
        }

        //private void SetScreenParameter_CTS330(CTS330_ScreenParameter obj)
        //{
        //    //UpdateScreenObject(obj);
        //    Session.Remove("CTS330_INCIDENTID");
        //    Session.Add("CTS330_INCIDENTID", obj);
        //}

        /// <summary>
        /// Get last incident data from database
        /// </summary>
        /// <param name="incidentID"></param>
        private void UpdateIncidentData_CTS330(int incidentID)
        {
            CTS330_ScreenParameter sParam = GetScreenObject<CTS330_ScreenParameter>();
            sParam.sessObj = CreateIncidentDataWithPermission_CTS330(incidentID);
        }

        /// <summary>
        /// Create data and permission object for view
        /// </summary>
        /// <param name="incidentID"></param>
        /// <returns></returns>
        private CTS330_SessionData CreateIncidentDataWithPermission_CTS330(int incidentID)
        {
            IIncidentHandler incidenthandler = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            CTS330_SessionData result = new CTS330_SessionData();

            try
            {
                var incidentData = incidenthandler.GetIncidentDetail(incidentID);
                var incidentPermit = incidenthandler.HasIncidentPermission(incidentID);

                bool IsSpecialView = false;
                bool IsEditable = false;

                if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_EDIT_INCIDENT, FunctionID.C_FUNC_ID_SPECIAL_VIEW_EDIT_INCIDENT))
                {
                    IsSpecialView = true;
                }

                if (CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_EDIT_INCIDENT, FunctionID.C_FUNC_ID_EDIT))
                {
                    IsEditable = true;
                }

                if (incidentData != null) //Add by Jutarat A. on 05032013
                {
                    CommonUtil.MappingObjectLanguage(incidentData.dtIncident);
                    CommonUtil.MappingObjectLanguage<dtIncidentRole>(incidentData.dtIncidentRole);
                    CommonUtil.MappingObjectLanguage<dtIncidentHistory>(incidentData.dtIncidentHistory);
                }

                result.incidentData = incidentData;
                result.incidentPermit = incidentPermit;
                result.IsEditable = IsEditable;
                result.IsSpecialView = IsSpecialView;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        //private string GetIncidentIDFromParameter_CTS330()
        //{
        //    string incidentID = null;

        //    //CTS330_ScreenParameter scrParam = GetScreenObject<CTS330_ScreenParameter>();
        //    CTS330_ScreenParameter scrParam = null;

        //    if (Session["CTS330_INCIDENTID"] != null)
        //    {
        //        scrParam = (CTS330_ScreenParameter)Session["CTS330_INCIDENTID"];
        //    }

        //    if ((scrParam != null) && (!String.IsNullOrEmpty(scrParam.strIncidentID)))
        //    {
        //        incidentID = scrParam.strIncidentID;
        //    }
        //        // dummie
        //    //else
        //    //{
        //    //    incidentID = "77";
        //    //}

        //    return incidentID;
        //}

        /// <summary>
        /// Generate history text
        /// </summary>
        /// <param name="incidentDat"></param>
        /// <param name="normalText"></param>
        /// <param name="allText"></param>
        private void CreateHistoryIncident_CTS330(CTS330_SessionData incidentDat, out string normalText, out string allText)
        {
            //historyTxt
            //historyTxtIndent
            //historyTxtIndentRed
            string normaltxt_html = "<span class=\"historyTxt\">{0}</span><br />";
            string normaltxtindent_html = "<span class=\"historyTxtIndent\">{0}</span><br />";
            string normaltxtbr_html = "<br />";

            normalText = "";
            allText = "";

            // dummie data
            //incidentDat.incidentData.dtIncidentHistory.Add(new dtIncidentHistory()
            //{
            //    CreateDate = DateTime.Now.AddDays(-8),
            //    IncidentInteractionTypeNameEN = "Hello",
            //    IncidentInteractionTypeNameJP = "Konichiwa",
            //    IncidentInteractionTypeNameLC = "Sa-wad-dee",
            //    IncidentHistoryID = 5,
            //    RespondingDetail = "hello wording"
            //});

            //incidentDat.incidentData.dtIncidentHistory.Add(new dtIncidentHistory()
            //{
            //    CreateDate = DateTime.Now.AddDays(-8),
            //    IncidentInteractionTypeNameEN = "GoodBye",
            //    IncidentInteractionTypeNameJP = "Sayonara",
            //    IncidentInteractionTypeNameLC = "Sa-wad-dee",
            //    IncidentHistoryID = 6,
            //    RespondingDetail = "Content will wrap to the next line when necessary, and a word-break will also occur if needed. Example. Ext/DocContent will wrap to the next line when necessary, and a word-break will also occur if needed. Example. Ext/DocContent will wrap to the next line when necessary, and a word-break will also occur if needed. Example. Ext/Doc"
            //});

            //incidentDat.incidentData.Tbt_IncidentHistoryDetail.Add(new tbt_IncidentHistoryDetail()
            //{
            //    IncidentHistoryID = 5,
            //    ChangeItemName = "Name",
            //    ItemOldValue = "TH",
            //    ItemNewValue = "EN"
            //});

            //incidentDat.incidentData.Tbt_IncidentHistoryDetail.Add(new tbt_IncidentHistoryDetail()
            //{
            //    IncidentHistoryID = 5,
            //    ChangeItemName = "Name 2",
            //    ItemOldValue = "TH 2",
            //    ItemNewValue = "EN 3"
            //});

            //incidentDat.incidentData.Tbt_IncidentHistoryDetail.Add(new tbt_IncidentHistoryDetail()
            //{
            //    IncidentHistoryID = 5,
            //    ChangeItemName = "Name 3",
            //    ItemOldValue = "TH 5",
            //    ItemNewValue = "EN 4"
            //});

            //incidentDat.incidentData.Tbt_IncidentHistoryDetail.Add(new tbt_IncidentHistoryDetail()
            //{
            //    IncidentHistoryID = 6,
            //    ChangeItemName = "Full Name",
            //    ItemOldValue = "",
            //    ItemNewValue = "html - Is there a way to word-wrap text in a div? - Stack Overflowhtml - Is there a way to word-wrap text in a div? - Stack Overflow"
            //});

            var hisListSort = from a in incidentDat.incidentData.dtIncidentHistory orderby a.CreateDate descending select a;

            foreach (dtIncidentHistory hisItem in hisListSort)
            {
                var currOffice = (from a in incidentDat.incidentData.dtEmployeeOffice where a.EmpNo == hisItem.CreateBy select a).ToList();

                if (currOffice != null)
                    CommonUtil.MappingObjectLanguage<dtEmployeeOffice>(currOffice);

                string tmpText = String.Format("{0} : {1} : {2} : {3}"
                    , CommonUtil.TextDate(hisItem.CreateDate)
                    , (currOffice.Count > 0)
                    ? currOffice[0].OfficeName
                        : String.Empty
                    , (currOffice.Count > 0)
                        ? currOffice[0].EmpFirstName + " " + currOffice[0].EmpLastName
                        : String.Empty
                    , hisItem.IncidentInteractionTypeName);
                normalText += normaltxt_html.Clone().ToString().Replace("{0}", tmpText);
                allText += normaltxt_html.Clone().ToString().Replace("{0}", tmpText);

                if (!String.IsNullOrEmpty(hisItem.RespondingDetail))
                {
                    normalText += normaltxt_html.Clone().ToString().Replace("{0}", hisItem.RespondingDetail);
                    allText += normaltxt_html.Clone().ToString().Replace("{0}", hisItem.RespondingDetail);
                }

                var hisDetail = from a in incidentDat.incidentData.Tbt_IncidentHistoryDetail where a.IncidentHistoryID == hisItem.IncidentHistoryID select a;
                foreach (var detailItem in hisDetail.ToList())
                {
                    string subTxt = String.Format("{0} : {1} => {2}", detailItem.ChangeItemName, detailItem.ItemOldValue, detailItem.ItemNewValue);
                    allText += normaltxtindent_html.Clone().ToString().Replace("{0}", subTxt);
                }

                normalText += normaltxtbr_html;
                allText += normaltxtbr_html;
            }

            //return result;
        }

        #endregion
    }
}
