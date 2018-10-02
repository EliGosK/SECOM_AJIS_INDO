//*********************************
// Create by: Akat K.
// Create date: 13/Sep/2011
// Update date: 13/Sep/2011
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

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController {

        private const string C_CORRESPONDENT = "1";
        private const string C_CHIEF = "2";
        private const string C_ADMIN = "3";

        private const string CTS310_Screen = "CTS310";

        /// <summary>
        /// Check user’s permission and administrator permission 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS310_Authority(CTS310_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_INCIDENT)) {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                param.isAdmin = CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_INCIDENT, FunctionID.C_FUNC_ID_SPECIAL_VIEW_CONFIDENTIAL);
                if (!param.isAdmin && C_ADMIN.Equals(param.screenMode)) {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3152);
                    return Json(res);
                }

                if (res.IsError) {
                    return Json(res);
                }

                return InitialScreenEnvironment<CTS310_ScreenParameter>(CTS310_Screen, param, res);
            } catch (Exception ex) {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize(CTS310_Screen)]
        public ActionResult CTS310() {
            CTS310_ScreenParameter param = GetScreenObject<CTS310_ScreenParameter>();

            ViewBag.IncidentOfficeCode = param.incidentOfficeCode;

            if (C_CORRESPONDENT.Equals(param.SubObjectID)) {
                ViewBag.ScreenMode = "Correspondent";
                ViewBag.DefaultRole = IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT;
                ViewBag.DefaultDueDate = IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_2WEEKS;
                ViewBag.DefaultStatus = IncidentSearchStatus.C_INCIDENT_SEARCH_STATUS_HANDLING;
            } else if (C_CHIEF.Equals(param.SubObjectID)) {
                ViewBag.ScreenMode = "Chief";
                ViewBag.DefaultRole = IncidentRole.C_INCIDENT_ROLE_CHIEF;
                ViewBag.DefaultDueDate = IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_2WEEKS;
                ViewBag.DefaultStatus = IncidentSearchStatus.C_INCIDENT_SEARCH_STATUS_HANDLING;
            } else if (C_ADMIN.Equals(param.SubObjectID)) {
                ViewBag.ScreenMode = "Admin";
                ViewBag.DefaultRole = IncidentRole.C_INCIDENT_ROLE_ADMINISTRATOR;
                ViewBag.DefaultDueDate = IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_ALL;
                ViewBag.DefaultStatus = IncidentSearchStatus.C_INCIDENT_SEARCH_STATUS_ALL;
            } else if (ScreenID.C_SCREEN_ID_SUMMARY_INCIDENT.Equals(param.callerID) && ViewBag.IncidentOfficeCode != null) {
                ViewBag.ScreenMode = "Office";
            } else {
                ViewBag.ScreenMode = "Search";
            }

            ViewBag.DueDate1Week = IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1WEEK;
            ViewBag.DueDate2Week = IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_2WEEKS;
            ViewBag.DueDate1Month = IncidentSearchDuedate.C_INCIDENT_SEARCH_DUEDATE_1MONTH;

            ViewBag.UnImplemented = ContractStatus.C_CONTRACT_STATUS_BEF_START;
            ViewBag.Implemented = ContractStatus.C_CONTRACT_STATUS_AFTER_START;
            ViewBag.StopService = ContractStatus.C_CONTRACT_STATUS_STOPPING;
            ViewBag.Cancel = ContractStatus.C_CONTRACT_STATUS_END+","
                            +","+ContractStatus.C_CONTRACT_STATUS_CANCEL+","
                            +","+ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL;

            ViewBag.AL = ProductType.C_PROD_TYPE_AL+"," + ProductType.C_PROD_TYPE_RENTAL_SALE+","+ProductType.C_PROD_TYPE_ONLINE;
            ViewBag.Sales = ServiceType.C_SERVICE_TYPE_SALE;
            ViewBag.Maintenance = ProductType.C_PROD_TYPE_MA;
            ViewBag.SentryGuard = ProductType.C_PROD_TYPE_BE+","+ProductType.C_PROD_TYPE_SG;

            ViewBag.Handling = "," + IncidentStatus.C_INCIDENT_STATUS_CONTROL_CHIEF_UNREGISTERED +
                                "," + IncidentStatus.C_INCIDENT_STATUS_HAVE_REPLY_FROM_CHIEF +
                                "," + IncidentStatus.C_INCIDENT_STATUS_HAVE_UNREAD_INSTRUCTION +
                                "," + IncidentStatus.C_INCIDENT_STATUS_INCIDENT_CHIEF_RESPONDING +
                                "," + IncidentStatus.C_INCIDENT_STATUS_NEW_REGISTER +
                                "," + IncidentStatus.C_INCIDENT_STATUS_REPORT +
                                "," + IncidentStatus.C_INCIDENT_STATUS_RESPONDING +
                                "," + IncidentStatus.C_INCIDENT_STATUS_WAIT_FOR_COMPLETE_APPROVAL +
                                "," + IncidentStatus.C_INCIDENT_STATUS_WAIT_FOR_INSTRUCTION +
                                ",";
            ViewBag.Complete = ","+IncidentStatus.C_INCIDENT_STATUS_COMPLETE+",";

            return View();
        }

        /// <summary>
        /// Initial grid schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS310_InitGrid() {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS310"));
        }
        
        /// <summary>
        /// Search incident by role when change mode to "Admistrator List Mode", "Correspondent List Mode", "Office List Mode", or "Chief List Mode"
        /// </summary>
        /// <param name="conditionByRole"></param>
        /// <returns></returns>
        public ActionResult CTS310_searchByRole(SearchIncidentListByRoleCondition conditionByRole) {
            ObjectResultData res = new ObjectResultData();
            
            try {
                doIncidentListByRole condition = new doIncidentListByRole();
                condition.incidentRole = conditionByRole.strIncidentRole;
                condition.incidentStatus = conditionByRole.strIncidentStatus;
                condition.empNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                if (conditionByRole.intAddDate.HasValue && conditionByRole.intAddDate.Value != 0) {
                    condition.dueDate = System.DateTime.Now.AddDays(conditionByRole.intAddDate.Value);
                }

                IIncidentHandler hand = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
                List<dtIncidentList> incidentList = hand.GetIncidentListByRole(condition);
                CommonUtil.MappingObjectLanguage<dtIncidentList>(incidentList);

                MiscTypeMappingList miscMapList = new MiscTypeMappingList();
                miscMapList.AddMiscType(incidentList.ToArray());
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                comh.MiscTypeMappingList(miscMapList);

                List<tbs_IncidentPermissionConfiguration> permissions = hand.GetTbs_IncidentPermissionConfiguration("," + conditionByRole.strIncidentRole + ",");
                bool canViewConfidential = false;
                foreach (var permission in permissions) {
                    if (permission.ViewConfidentialIncidentFlag.HasValue && permission.ViewConfidentialIncidentFlag.Value) {
                        canViewConfidential = true;
                        break;
                    }
                }

                CTS310_ScreenParameter param = GetScreenObject<CTS310_ScreenParameter>();
                foreach (var incident in incidentList) {
                    if (String.IsNullOrEmpty(incident.IncidentNo))
                    {
                        incident.IncidentNo = CommonUtil.GetLabelFromResource("Contract", "CTS310", "lblNA");
                    }

                    if (incident.DueDateDeadLine.HasValue)
                    {
                        incident.DueDateDeadLine = new DateTime(incident.DueDateDeadLine.Value.Year,
                                incident.DueDateDeadLine.Value.Month,
                                incident.DueDateDeadLine.Value.Day,
                                23, 59, 59);

                        if (incident.DueDateTime.HasValue)
                        {
                            incident.DueDateDeadLine = new DateTime(incident.DueDateDeadLine.Value.Year,
                                incident.DueDateDeadLine.Value.Month,
                                incident.DueDateDeadLine.Value.Day,
                                incident.DueDateTime.Value.Hours,
                                incident.DueDateTime.Value.Minutes,
                                incident.DueDateTime.Value.Seconds);
                        }
                    }

                    if (CommonUtil.IsNullOrEmpty(incident.IncidentNo)) {
                        incident.IncidentNo = incident.IncidentID.ToString();
                    }

                    //Midify by Jutarat A. on 04032013
                    //incident.canViewConfidential = param.isAdmin || (!incident.ConfidentialFlag.Value) || canViewConfidential;
                    bool blConfidentialFlag = incident.ConfidentialFlag == null ? false : incident.ConfidentialFlag.Value;
                    incident.canViewConfidential = param.isAdmin || (!blConfidentialFlag) || canViewConfidential;
                    //End Modify

                    setColStyle(incident);
                }

                string xml = CommonUtil.ConvertToXml<dtIncidentList>(incidentList, "Contract\\CTS310", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
            } catch (Exception ex) {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate search condition before proceed search incident
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult CTS310_ValidateSearch(SearchIncidentCondition condition) {
            ObjectResultData res = new ObjectResultData();
            try {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { condition });

                if (condition.SpecfyPeriod == null && (condition.SpecifyPeriodFrom.HasValue || condition.SpecifyPeriodTo.HasValue)) {
                    res.AddErrorMessage(    MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0116, null, new string[] { "cboSpecifiedPeriod" });
                }

                if (condition.SpecfyPeriod != null &&
                    condition.SpecifyPeriodFrom.HasValue && condition.SpecifyPeriodTo.HasValue &&
                    condition.SpecifyPeriodFrom.Value.CompareTo(condition.SpecifyPeriodTo.Value) > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0090, null, new string[] { "datePeriodFrom", "datePeriodTo" });
                }

                if (!res.IsError) {
                    res.ResultData = condition;
                }

            } catch (Exception ex) {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Search incident by criteria when click [Search] button on Incident list by role section
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult CTS310_searchIncidentList(SearchIncidentCondition condition)
        {
            ObjectResultData res = new ObjectResultData();
            try {
                CommonUtil c = new CommonUtil();
                condition.ContractCode = c.ConvertContractCode(condition.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                 
                IIncidentHandler hand = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
                List<dtIncidentList> incidentList = hand.SearchIncidentList(condition);

                foreach (var item in incidentList)
                {
                    // Set default
                    if (item.DueDateDeadLine.HasValue)
                    {
                        item.DueDateDeadLine = new DateTime(item.DueDateDeadLine.Value.Year,
                                item.DueDateDeadLine.Value.Month,
                                item.DueDateDeadLine.Value.Day,
                                23, 59, 59);

                        if (item.DueDateTime.HasValue)
                        {
                            item.DueDateDeadLine = new DateTime(item.DueDateDeadLine.Value.Year,
                                item.DueDateDeadLine.Value.Month,
                                item.DueDateDeadLine.Value.Day,
                                item.DueDateTime.Value.Hours,
                                item.DueDateTime.Value.Minutes,
                                item.DueDateTime.Value.Seconds);
                        }
                    }

                    item.IncidentNo = (String.IsNullOrEmpty(item.IncidentNo)) ? CommonUtil.GetLabelFromResource("Contract", "CTS310", "lblNA") : item.IncidentNo;
                }

                CommonUtil.MappingObjectLanguage<dtIncidentList>(incidentList);

                MiscTypeMappingList miscMapList = new MiscTypeMappingList();
                miscMapList.AddMiscType(incidentList.ToArray());
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                comh.MiscTypeMappingList(miscMapList);

                CTS310_ScreenParameter param = GetScreenObject<CTS310_ScreenParameter>();
                foreach (var incident in incidentList) {
                    if (CommonUtil.IsNullOrEmpty(incident.IncidentNo)) {
                        incident.IncidentNo = incident.IncidentID.ToString();
                    }
                    doHasIncidentPermission hasPermission = hand.HasIncidentPermission(incident.IncidentID);
                    incident.canViewConfidential = param.isAdmin || (!incident.ConfidentialFlag.Value) || hasPermission.ViewConfidentialIncidentFlag;
                    setColStyle(incident);
                }

                string xml = CommonUtil.ConvertToXml<dtIncidentList>(incidentList, "Contract\\CTS310", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
            } catch (Exception ex) {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Setting column and data style (color, and text)
        /// </summary>
        /// <param name="incident"></param>
        private void setColStyle(dtIncidentList incident) {
            if (incident.hasRespondingDetailFlag.HasValue
                //&& incident.hasRespondingDetailFlag.Value
                && incident.hasRespondingDetailFlag.Value != FlagType.C_FLAG_ON)
            {
                incident.OverNewImportant = "n";
            }

            //if ((incident.DueDateDeadLine.HasValue && incident.DueDateDeadLine.Value.CompareTo(System.DateTime.Today) < 0)
            if ((incident.DueDateDeadLine.HasValue && incident.DueDateDeadLine.Value.CompareTo(DateTime.Now) < 0)
                && (!IncidentStatus.C_INCIDENT_STATUS_COMPLETE.Equals(incident.IncidentStatus)))
            {
                incident.OverNewImportant = "o";
            }

            if (incident.ImportanceFlag.HasValue && incident.ImportanceFlag.Value) {
                incident.Important = "i";
            }

            if ((incident.CorrEmpNo_EXT != null && incident.CorrEmpNo_EXT.IndexOf(CommonUtil.dsTransData.dtUserData.EmpNo) >= 0)
                || (incident.ChiefEmpNo_EXT != null && incident.ChiefEmpNo_EXT.IndexOf(CommonUtil.dsTransData.dtUserData.EmpNo) >= 0)
                || (incident.AsstEmpNo_EXT != null && incident.AsstEmpNo_EXT.IndexOf(CommonUtil.dsTransData.dtUserData.EmpNo) >= 0)
                || (incident.ConChiefEmpNo_EXT != null && incident.ConChiefEmpNo_EXT.IndexOf(CommonUtil.dsTransData.dtUserData.EmpNo) >= 0)
                )
            {
                incident.isStatusYellow = true;
            } else {
                incident.isStatusYellow = false;
            }

            CTS310_ScreenParameter param = GetScreenObject<CTS310_ScreenParameter>();
            if (IncidentRole.C_INCIDENT_ROLE_CHIEF.Equals(param.SubObjectID) && CommonUtil.IsNullOrEmpty(incident.CorrEmpNo_EXT))
            {
                incident.isCorrespondentYellow = true;
            } else {
                incident.isCorrespondentYellow = false;
            }
        }

        /// <summary>
        /// Get incdient role combo as html format
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAdminCombo(string filter) {
            try {
                string strDisplay = "ValueDisplay";
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>() {
                    new doMiscTypeCode() {
                        FieldName = MiscType.C_INCIDENT_ROLE,
                        ValueCode = IncidentRole.C_INCIDENT_ROLE_CHIEF
                    }
                };
                miscs.Add(new doMiscTypeCode()
                {
                    FieldName = MiscType.C_INCIDENT_ROLE,
                    ValueCode = IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT
                });

                List<doMiscTypeCode> MiscLock = hand.GetMiscTypeCodeList(miscs);

                foreach (doMiscTypeCode i in MiscLock)
                    i.ValueDisplay = i.ValueCode + ':' + i.ValueDisplay;

                string administrator = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_SEARCH_INCIDENT, "txtAdmin");
                doMiscTypeCode first = new doMiscTypeCode();
                first.ValueCode = "";
                first.ValueDisplay = administrator;
                MiscLock.Insert(0, first);

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(MiscLock, strDisplay, "ValueCode", false);

                return Json(cboModel);
            } catch (Exception ex) {
               // return Json(MessageUtil.GetMessage(ex));
                return null;
            }
        }
    }
}
