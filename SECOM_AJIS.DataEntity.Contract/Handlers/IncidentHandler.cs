using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using CSI.WindsorHelper;
using System.Data;
using System.Reflection;
using System.Collections;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class IncidentHandler : BizCTDataEntities, IIncidentHandler
    {
        /// <summary>
        /// To generate incident no.
        /// </summary>
        /// <param name="strIncidentRelevantType"></param>
        /// <param name="strIncidentRelevantCode"></param>
        /// <param name="strIncidentOfficeCode"></param>
        /// <returns></returns>
        public string[] GenerateIncidentNo(string strIncidentRelevantType, string strIncidentRelevantCode, string strIncidentOfficeCode)
        {
            try
            {
                string strIncidentOffice = string.Empty;
                //1. Set incident office from DB
                #region Set incident office from DB
                //1.1 C_INCIDENT_RELEVANT_TYPE_CUSTOMER
                if (strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                {
                    strIncidentOffice = strIncidentOfficeCode;
                }
                //1.2 C_INCIDENT_RELEVANT_TYPE_SITE
                else if (strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
                {
                    //1.2.1 Get same site of contract
                    IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                    List<dtContractsSameSite> contractList = hand.GetContractsSameSiteList(strIncidentRelevantCode, null);

                    //1.2.2. If dtContractsSameSite[].Rows.Count <= 0 Then
                    if (contractList.Count <= 0)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3190);

                    //1.2.3 Operation office code from search oldest contract which related to such site in the order of N/Q/MA/S
                    strIncidentOffice = findMinimumContractCode(contractList);
                }
                //1.3 C_INCIDENT_RELEVANT_TYPE_SITE
                else if (strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT)
                {
                    strIncidentOffice = strIncidentOfficeCode;
                }
                //1.4 C_INCIDENT_RELEVANT_TYPE_CONTRACT
                else if (strIncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                {
                    //1.4.1 Get operation office of rental contract
                    IRentralContractHandler hand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    List<tbt_RentalContractBasic> rentalList = hand.GetTbt_RentalContractBasic(strIncidentRelevantCode, null);

                    //1.4.2 If list is not empty
                    if (rentalList != null && rentalList.Count > 0)
                    {
                        strIncidentOffice = rentalList[0].OperationOfficeCode;
                    }
                    else
                    {
                        //Get operation office of sale contract
                        ISaleContractHandler saleHand = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        List<tbt_SaleBasic> saleList = saleHand.GetTbt_SaleBasic(strIncidentRelevantCode,null,null);

                        if (saleList != null && saleList.Count > 0)
                        {
                            strIncidentOffice = saleList[0].OperationOfficeCode;
                        }
                        else
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3190);
                        }
                    }

                }
                #endregion

                //2. Get max running no. of incident from DB.
                //2.1 Set year
                string strYear = DateTime.Now.ToString("yy");
                string strPrefix = IncidentNo.C_INCIDENT_NO_PREFIX;

                //2.2 Call sp_CT_GetTbs_IncidentRunningNo
                List<dtTbs_IncidentRunningNo> runningNoList = base.GetTbs_IncidentRunningNo(strIncidentOffice, strYear, strPrefix);

                //2.3 Check return result
                #region Check return result
                string strRunningNo = string.Empty;
                if (runningNoList != null && runningNoList.Count > 0)
                {
                    //2.3.1 Set running no.
                    //strRunningNo = runningNoList[0].IncidentRunningNo;
                    int runningNo = runningNoList[0].IncidentRunningNo.GetValueOrDefault();
                    runningNo = runningNo + 1;
                    strRunningNo = runningNo.ToString().PadLeft(5, '0');

                    //2.3.2 Check over maximum range of running no.
                    int approveNoMax = Convert.ToInt32(ARNo.C_AR_NO_MAXIMUM);
                    if (runningNo > approveNoMax)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3016);
                    }
                    else if (runningNo <= approveNoMax)
                    {
                        //Comment by Jutarat A. 17102012
                        ////Check whether this record is the most updated data
                        //List<dtTbs_IncidentRunningNo> rList = base.GetTbs_IncidentRunningNo(strIncidentOffice, strYear, strPrefix);
                        ////if (rList[0].UpdateDate != runningNoList[0].UpdateDate)
                        //if (DateTime.Compare(rList[0].UpdateDate.Value, runningNoList[0].UpdateDate.Value) != 0)
                        //{
                        //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG0019);
                        //}
                        //else
                        //{
                            //Update running no. to DB
                            List<tbs_IncidentRunningNo> updatedList
                                = base.UpdateTbs_IncidentRunningNo(strIncidentOffice, strYear, strPrefix, runningNo, CommonUtil.dsTransData.dtUserData.EmpNo);

                            //Insert Log
                            if (updatedList.Count > 0)
                            {
                                doTransactionLog logData = new doTransactionLog();
                                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                                logData.TableName = TableName.C_TBL_NAME_INCIDENT_RUNNO;
                                logData.TableData = CommonUtil.ConvertToXml(updatedList);
                                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                                hand.WriteTransactionLog(logData);
                            }
                        //}
                    }
                }
                else
                {
                    //2.3.2 Set running no.
                    strRunningNo = IncidentNo.C_INCIDENT_NO_MINIMUM;
                    int runningNo = int.Parse(strRunningNo);

                    //2.3.4 Insert running no. to DB
                    List<tbs_IncidentRunningNo> insertedList
                        = base.InsertTbs_IncidentRunningNo(strIncidentOffice, strYear, strPrefix, runningNo, CommonUtil.dsTransData.dtUserData.EmpNo);

                    //Insert Log
                    if (insertedList.Count > 0)
                    {
                        doTransactionLog logData = new doTransactionLog();
                        logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                        logData.TableName = TableName.C_TBL_NAME_INCIDENT_RUNNO;
                        logData.TableData = CommonUtil.ConvertToXml(insertedList);
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }
                }
                #endregion

                //3. Create incident no. [Incident office][Year][Prefix][Running no.]
                string strIncidentNo = String.Format("{0}{1}{2}{3}", strIncidentOffice, strYear, strPrefix, strRunningNo);

                //4. return strARNo, strIncidentOffice
                string[] result = new string[] { strIncidentNo, strIncidentOffice };
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Find minimum contract code
        /// </summary>
        /// <param name="contractList"></param>
        /// <returns></returns>
        private String findMinimumContractCode(List<dtContractsSameSite> contractList)
        {
            String strOfficeCode = string.Empty;
            if (contractList != null && contractList.Count > 0)
            {
                #region gen datatable
                //DataTable dt = new DataTable();
                //dt.Columns.Add("ContractCode", typeof(string));
                //dt.Columns.Add("ServiceTypeCode", typeof(string));
                //dt.Columns.Add("OperationOfficeCode", typeof(string));
                //foreach (dtContractsSameSite contract in contractList)
                //{
                //    dt.Rows.Add(contract.ContractCode, contract.ServiceTypeCode, contract.OperationOfficeCode);
                //}
                #endregion

                //Generate datatable
                //DataTable dt = CommonUtil.ConvertDoListToDataTable(contractList);

                //Check for C_PROD_TYPE_AL, C_PROD_TYPE_ONLINE
                strOfficeCode = getOperationOfficeCode(contractList, ProductType.C_PROD_TYPE_AL, ProductType.C_PROD_TYPE_ONLINE, ProductType.C_PROD_TYPE_RENTAL_SALE);
                if (strOfficeCode != string.Empty)
                    return strOfficeCode;

                //Check for C_PROD_TYPE_SALE
                strOfficeCode = getOperationOfficeCode(contractList, ProductType.C_PROD_TYPE_SALE);
                if (strOfficeCode != string.Empty)
                    return strOfficeCode;

                //Check for C_PROD_TYPE_MA
                strOfficeCode = getOperationOfficeCode(contractList, ProductType.C_PROD_TYPE_MA);
                if (strOfficeCode != string.Empty)
                    return strOfficeCode;

                //Check for C_PROD_TYPE_BE, C_PROD_TYPE_SG
                strOfficeCode = getOperationOfficeCode(contractList, ProductType.C_PROD_TYPE_BE, ProductType.C_PROD_TYPE_SG);
                if (strOfficeCode != string.Empty)
                    return strOfficeCode;
            }
            return strOfficeCode;
        }

        /// <summary>
        /// Get operation office code
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="paramProductType"></param>
        /// <returns></returns>
        private string getOperationOfficeCode(List<dtContractsSameSite> ls, params string[] paramProductType )
        {
            string strOfficeCode = string.Empty;

            //string strOfficeCode = string.Empty;
            //string expression = string.Empty;
            //if (productType2 == null)
            //    expression = String.Format("ServiceTypeCode = '{0}'", productType);
            //else
            //    expression = String.Format("ServiceTypeCode in ('{0}','{1}')", productType, productType2);

            //string sortOrder = "ContractCode ASC";
            //DataRow[] foundRows = dt.Select(expression, sortOrder);
            //if (foundRows.Length > 0)
            //    strOfficeCode = foundRows[0]["OperationOfficeCode"].ToString();

            //return strOfficeCode;


            // edit by Narupon W.
            List<dtContractsSameSite> list = (from t in ls
                                              where paramProductType.Contains<string>(t.ProductTypeCode)
                                              orderby t.ContractCode ascending
                                              select t).ToList<dtContractsSameSite>();

            if (list.Count > 0)
            {
                strOfficeCode = list[0].OperationOfficeCode;
            }

            return strOfficeCode;


        }

        /// <summary>
        /// Get rental contract basic information
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<dtIncidentList> GetIncidentListByRole(doIncidentListByRole condition)
        {
            return base.GetIncidentListByRole(condition.incidentRole, condition.empNo, condition.dueDate, condition.incidentStatus
                , IncidentSearchStatus.C_INCIDENT_SEARCH_STATUS_COMPLETE
                , IncidentSearchStatus.C_INCIDENT_SEARCH_STATUS_HANDLING
                , IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF
                , IncidentRole.C_INCIDENT_ROLE_CHIEF 
                , IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT
                , IncidentRole.C_INCIDENT_ROLE_ASSISTANT
                , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT
                , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE
                , MiscType.C_INCIDENT_STATUS
                , IncidentStatus.C_INCIDENT_STATUS_COMPLETE
                , MiscType.C_INCIDENT_TYPE
                , MiscType.C_DEADLINE_TIME_TYPE);
        }

        /// <summary>
        /// Search incident data
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<dtIncidentList> SearchIncidentList(doSearchIncidentListCondition condition) {
            if (condition.ContractTargetPurchaserName == null && condition.ContractCode == null && condition.UserCode == null
                && condition.ContractOfficeCode == null && condition.OperationOfficeCode == null && condition.ContractStatus == null
                && condition.ContractType == null && condition.CustomerName == null && condition.CustomerGroupName == null
                && condition.SiteName == null && condition.ProjectName == null && condition.IncidentNo == null
                && condition.IncidentTitle == null && condition.IncidentType == null && condition.IncidentStatusHandling == null
                && condition.IncidentStatusComplete == null && condition.IncidentOfficeCode == null && condition.SpecfyPeriod == null
                && !condition.SpecifyPeriodFrom.HasValue && !condition.SpecifyPeriodTo.HasValue && condition.Registrant == null
                && condition.Correspondent == null && condition.Chief == null && condition.Assistant == null && condition.ControlChief == null)
            {
                ApplicationErrorException ex = new ApplicationErrorException();
                ex.AddErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                throw ex;
            }

            return base.SearchIncidentList(condition.IncidentNo, condition.IncidentTitle, condition.IncidentType, condition.IncidentStatusHandling,
                condition.IncidentStatusComplete, condition.IncidentOfficeCode, condition.SpecfyPeriod, condition.SpecifyPeriodFrom, condition.SpecifyPeriodTo
                , condition.Registrant, condition.ControlChief, condition.Correspondent, condition.Chief, condition.Assistant, condition.ContractTargetPurchaserName
                , condition.SiteName, condition.CustomerGroupName, condition.ContractCode, condition.UserCode, condition.ContractOfficeCode, condition.OperationOfficeCode
                , condition.ContractStatus, condition.ContractType, condition.CustomerName, condition.ProjectName
                , IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF
                , IncidentRole.C_INCIDENT_ROLE_CHIEF
                , IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT
                , IncidentRole.C_INCIDENT_ROLE_ASSISTANT
                , MiscType.C_INCIDENT_TYPE
                , MiscType.C_INCIDENT_STATUS
                , MiscType.C_DEADLINE_TIME_TYPE
                , IncidentSearchPeriod.C_INCIDENT_SEARCH_PERIOD_OCCURRING
                , IncidentSearchPeriod.C_INCIDENT_SEARCH_PERIOD_DUEDATE
                , IncidentSearchPeriod.C_INCIDENT_SEARCH_PERIOD_COMPLETE
                , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT
                , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER
                , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT
                , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE
                , IncidentStatus.C_INCIDENT_STATUS_COMPLETE);
        }

        /// <summary>
        /// Get incident data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<dtIncidentListCTS320> GetIncidentList(doRetrieveIncidentListCondition cond)
        {
            List<dtIncidentListCTS320> result = new List<dtIncidentListCTS320>();

            if ((cond == null) || (cond.IncidentRelevantType == null) || ((cond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER) && (String.IsNullOrEmpty(cond.CustomerCode)))
                || ((cond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE) && (string.IsNullOrEmpty(cond.SiteCode)))
                || ((cond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT) && (string.IsNullOrEmpty(cond.ContractCode)))
                || ((cond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT) && (string.IsNullOrEmpty(cond.ProjectCode))))
            {
                ApplicationErrorException ex = new ApplicationErrorException();
                ex.AddErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                throw ex;
            }

            result = base.GetIncidentList(cond.IncidentRelevantType
                , cond.CustomerCode
                , cond.SiteCode
                , cond.ContractCode
                , cond.ProjectCode
                , cond.IncidentType
                , cond.DuedateDeadline
                , cond.IncidentStatus
                , IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF
                , IncidentRole.C_INCIDENT_ROLE_CHIEF
                , IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT
                , IncidentRole.C_INCIDENT_ROLE_ASSISTANT
                , MiscType.C_INCIDENT_TYPE
                , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER
                , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE
                , MiscType.C_INSTALL_STATUS
                , IncidentStatus.C_INCIDENT_STATUS_COMPLETE
                , IncidentSearchStatus.C_INCIDENT_SEARCH_STATUS_COMPLETE
                , IncidentSearchStatus.C_INCIDENT_SEARCH_STATUS_HANDLING
                , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT
                , MiscType.C_DEADLINE_TIME_TYPE
                , FlagType.C_FLAG_ON ? "1" : "0"
                , (cond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER)
                , (cond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE)
                , (cond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT)
                , (cond.IncidentRelevantType == IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT));

            return result;
        }

        /// <summary>
        /// Get incident occurring site data
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public List<dtIncidentOccSite> GetOccurringSiteList(string custCode)
        {
            List<dtIncidentOccSite> result = new List<dtIncidentOccSite>();

            result = GetIncidentOccurringSite(custCode, IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE);

            return result;
        }

        /// <summary>
        /// Get incident occurring contract data
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public List<dtIncidentOccContract> GetOccurringContractList(string siteCode)
        {
            List<dtIncidentOccContract> result = new List<dtIncidentOccContract>();

            result = GetIncidentOccurringContract(siteCode, IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT);

            return result;
        }

        /// <summary>
        /// Get incident permission configuration data
        /// </summary>
        /// <param name="incidentID"></param>
        /// <returns></returns>
        public doHasIncidentPermission HasIncidentPermission(int? incidentID) 
        {
            doHasIncidentPermission result;
            IEmployeeMasterHandler empHandler;
            IOfficeMasterHandler officeHandler;

            try
            {
                empHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                officeHandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                if (!incidentID.HasValue) {
                    ApplicationErrorException ex = new ApplicationErrorException();
                    ex.AddErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                    throw ex;
                }

                List<tbt_IncidentRole> roles = base.GetTbt_IncidentRole(incidentID, CommonUtil.dsTransData.dtUserData.EmpNo);
                string permissionType = ",";
                foreach (var role in roles) {
                    if (IncidentRole.C_INCIDENT_ROLE_CONTROL_CHIEF.Equals(role.IncidentRoleType)) {
                        permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_CONTROL_CHIEF + ",";
                    } else if (IncidentRole.C_INCIDENT_ROLE_CHIEF.Equals(role.IncidentRoleType)) {
                        permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_CHIEF + ",";
                    } else if (IncidentRole.C_INCIDENT_ROLE_CORRESPONDENT.Equals(role.IncidentRoleType)) {
                        permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_CORRESPONDENT + ",";
                    } else if (IncidentRole.C_INCIDENT_ROLE_ASSISTANT.Equals(role.IncidentRoleType)) {
                        permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_ASSISTANT + ",";
                    }

                    if (IncidentRole.C_INCIDENT_ROLE_CHIEF_OF_RELATED_OFFICE.Equals(role.IncidentRoleType)) {
                        permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_RELATED_OFFICE + ",";
                    }
                }

                //2.2.	Is Administrator // Check from screen permission

                List<dtIncident> incidents = base.GetIncidentData(incidentID
                    , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT
                    , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER
                    , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE
                    , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT
                    , MiscType.C_INCIDENT_TYPE, MiscType.C_REASON_TYPE);

                foreach (var incident in incidents) {

                    //Comment by Jutarat A. on 24092012
                    //foreach (var office in CommonUtil.dsTransData.dtOfficeData) {
                    //    if (office.OfficeCode.Equals(incident.IncidentOfficeCode)) {
                    //        permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_HAS_DATA_AUTHORITY + ",";
                    //        break;
                    //    }
                    //}
                    //End Comment

                    if (CommonUtil.dsTransData.dtUserData.EmpNo.Equals(incident.ProjectManagerEmpNo)) {
                        permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_PROJECT_MANAGER + ",";
                    }

                    if (incident.IncidentDepartmentCode != null) {
                        List<string> incidentDepartmentChief = base.GetIncidentDepartmentChief(incidentID, FlagType.C_FLAG_ON);
                        if (incidentDepartmentChief.Count > 0 && incidentDepartmentChief.Contains(CommonUtil.dsTransData.dtUserData.EmpNo)) {
                            permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_OFFICE_CHIEF + ",";
                        }
                    } else {
                        List<string> incidentOfficeChief = base.GetIncidentOfficeChief(incidentID, FlagType.C_FLAG_ON);
                        if (incidentOfficeChief.Count > 0 && incidentOfficeChief.Contains(CommonUtil.dsTransData.dtUserData.EmpNo)) {
                            permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_OFFICE_CHIEF + ",";
                        }
                    }

                    //Add by Jutarat A. on 24092012
                    //Get OfficeCode, BranchCode, HQCode
                    if (String.IsNullOrEmpty(incident.IncidentOfficeCode) == false) //Add by Jutarat A. on 05102012
                    {
                        List<tbm_Office> dtTbt_Office = officeHandler.GetTbm_Office(incident.IncidentOfficeCode);
                        if (dtTbt_Office != null && dtTbt_Office.Count > 0)
                        {
                            List<dtUserBelonging> dt_Belonging = empHandler.getBelongingByEmpNo(CommonUtil.dsTransData.dtUserData.EmpNo);

                            //05 : Person who belong to same office of AR office
                            var belongOffice = (from t in dt_Belonging where t.OfficeCode == dtTbt_Office[0].OfficeCode select t).ToList();
                            if (belongOffice != null && belongOffice.Count > 0)
                            {
                                permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_BELONG_SAME_OFFICE + ",";
                            }

                            //06 : Person who belong to same branch of branch of AR office
                            var belongBranch = (from t in dt_Belonging where t.OfficeCode == dtTbt_Office[0].BranchCode select t).ToList();
                            if (belongBranch != null && belongBranch.Count > 0)
                            {
                                permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_BELONG_SAME_BRANCH + ",";
                            }

                            //07 : Person whe belong headquarter
                            var belongHQ = (from t in dt_Belonging where t.OfficeCode == dtTbt_Office[0].HQCode select t).ToList();
                            if (belongHQ != null && belongHQ.Count > 0)
                            {
                                permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_BELONG_HQ + ",";
                            }
                        }
                    }
                    //End Add
                }

                List<dtEmailAddressForIncident> emails = empHandler.GetEmailAddressForIncident(FlagType.C_FLAG_ON);

                foreach (var email in emails) {
                    if (email.EmpNo.Equals(CommonUtil.dsTransData.dtUserData.EmpNo)) {
                        permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_EXECUTIVE + ",";
                    }
                }

                permissionType += IncidentPermissionType.C_INCIDENT_PERMISSION_TYPE_GENERAL + ",";

                List<tbs_IncidentPermissionConfiguration> arPermission = base.GetTbs_IncidentPermissionConfiguration(permissionType);

                result = new doHasIncidentPermission();

                var permission =
                    from p in arPermission
                    group p by p.GetType() into g
                    select new { AssignChiefFlag = g.Max(p => p.AssignChiefFlag)
                        , AssignCorrespondentFlag = g.Max(p => p.AssignCorrespondentFlag)
                        , AssignAssistantFlag = g.Max(p => p.AssignAssistantFlag)
                        , ViewConfidentialIncidentFlag = g.Max(p => p.ViewConfidentialIncidentFlag)
                        , EditIncidentFlag = g.Max(p => p.EditIncidentFlag)
                        , EditConfidentailIncidentFlag = g.Max(p => p.EditConfidentailIncidentFlag)
                    };

                var incidentInderactionType = (
                    from p in arPermission
                    select p.IncidentInteractionTypeSet)
                    .Distinct();

                foreach (var permiss in permission) {
                    result.AssignAssistantFlag = permiss.AssignAssistantFlag.HasValue ? permiss.AssignAssistantFlag.Value : false;
                    result.AssignChiefFlag = permiss.AssignChiefFlag.HasValue ? permiss.AssignChiefFlag.Value : false;
                    result.AssignCorrespondentFlag = permiss.AssignCorrespondentFlag.HasValue ? permiss.AssignCorrespondentFlag.Value : false;
                    result.EditConfidentailIncidentFlag = permiss.EditConfidentailIncidentFlag.HasValue ? permiss.EditConfidentailIncidentFlag.Value : false;
                    result.EditIncidentFlag = permiss.EditIncidentFlag.HasValue ? permiss.EditIncidentFlag.Value : false;
                    result.ViewConfidentialIncidentFlag = permiss.ViewConfidentialIncidentFlag.HasValue ? permiss.ViewConfidentialIncidentFlag.Value : false;
                }

                result.IncidentInteractionTypeList = incidentInderactionType.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Summary incident data
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="currentdate"></param>
        /// <returns></returns>
        public List<dtSummaryIncident> SummaryIncident(Nullable<System.DateTime> dateFrom, Nullable<System.DateTime> dateTo, Nullable<System.DateTime> currentdate)
        {
            // Akat K. : allow for search All
            //if (!dateFrom.HasValue && !dateTo.HasValue) {
            //    ApplicationErrorException ex = new ApplicationErrorException();
            //    ex.AddErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
            //    throw ex;
            //}

            return base.SummaryIncident(dateFrom, dateTo, currentdate,
                IncidentStatus.C_INCIDENT_STATUS_WAIT_FOR_INSTRUCTION,
                IncidentStatus.C_INCIDENT_STATUS_COMPLETE,
                IncidentType.C_INCIDENT_TYPE_CANCEL_CONTRACT,
                IncidentType.C_INCIDENT_TYPE_COMPLAIN);
        }

        //public List<dtTbs_IncidentTypePattern> GetTbs_IncidentTypePattern(string strIncidentTypeCode)
        //{
        //    return base.GetTbs_IncidentTypePattern(strIncidentTypeCode);
        //}

        /// <summary>
        /// Insert new incident (list)
        /// </summary>
        /// <param name="listIncident"></param>
        /// <returns></returns>
        public List<tbt_Incident> InsertTbt_Incident(List<tbt_Incident> listIncident)
        {
            try
            {
                if (listIncident != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_Incident incidentData in listIncident)
                    {
                        incidentData.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        incidentData.CreateBy = dsTrans.dtUserData.EmpNo;
                        incidentData.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        incidentData.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_Incident> res = this.InsertTbt_Incident(
                    CommonUtil.ConvertToXml_Store<tbt_Incident>(listIncident));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_INCIDENT,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert new incident
        /// </summary>
        /// <param name="itemIncident"></param>
        /// <returns></returns>
        public tbt_Incident InsertTbt_Incident(tbt_Incident itemIncident)
        {
            try
            {
                if (itemIncident != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    itemIncident.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    itemIncident.CreateBy = dsTrans.dtUserData.EmpNo;
                    itemIncident.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    itemIncident.UpdateBy = dsTrans.dtUserData.EmpNo;
                }

                List<tbt_Incident> tmpLst = new List<tbt_Incident>();
                tmpLst.Add(itemIncident);

                List<tbt_Incident> res = this.InsertTbt_Incident(
                    CommonUtil.ConvertToXml_Store<tbt_Incident>(tmpLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_INCIDENT,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res[0];
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert new incident role (list)
        /// </summary>
        /// <param name="listIncidentRole"></param>
        /// <returns></returns>
        public List<tbt_IncidentRole> InsertTbt_IncidentRole(List<tbt_IncidentRole> listIncidentRole)
        {
            try
            {
                if (listIncidentRole != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_IncidentRole incidentRoleData in listIncidentRole)
                    {
                        incidentRoleData.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        incidentRoleData.CreateBy = dsTrans.dtUserData.EmpNo;
                        incidentRoleData.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        incidentRoleData.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_IncidentRole> res = this.InsertTbt_IncidentRole(
                    CommonUtil.ConvertToXml_Store<tbt_IncidentRole>(listIncidentRole));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_INCIDENT_ROLE,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert new incident role
        /// </summary>
        /// <param name="itemIncidentRole"></param>
        /// <returns></returns>
        public tbt_IncidentRole InsertTbt_IncidentRole(tbt_IncidentRole itemIncidentRole)
        {
            try
            {
                if (itemIncidentRole != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    itemIncidentRole.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    itemIncidentRole.CreateBy = dsTrans.dtUserData.EmpNo;
                    itemIncidentRole.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    itemIncidentRole.UpdateBy = dsTrans.dtUserData.EmpNo;
                }

                List<tbt_IncidentRole> tmpLst = new List<tbt_IncidentRole>();
                tmpLst.Add(itemIncidentRole);

                List<tbt_IncidentRole> res = this.InsertTbt_IncidentRole(
                    CommonUtil.ConvertToXml_Store<tbt_IncidentRole>(tmpLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_INCIDENT_ROLE,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res[0];
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get related contract code of incident that related with customer
        /// </summary>
        /// <param name="strSiteCodeList"></param>
        /// <returns></returns>
        public List<dtContractOfAllSite> GetReleatedContractOfCustIncident(string strSiteCodeList)
        {
            return this.GetReleatedContractOfCustIncident(strSiteCodeList, FlagType.C_FLAG_ON);
        }

        /// <summary>
        /// Create site code (list)
        /// </summary>
        /// <param name="siteList"></param>
        /// <returns></returns>
        private String CreateSiteCodeList(List<doSite> siteList)
        {
            String SiteCodeList = "";

            if (siteList != null)
            {
                foreach (doSite item in siteList)
                {
                    if (SiteCodeList.Length > 0)
                    {
                        SiteCodeList += ", ";
                    }
                    SiteCodeList += item.SiteCode;
                }
            }

            return SiteCodeList;
        }

        /// <summary>
        /// Get chief of related office with customer incident 
        /// </summary>
        /// <param name="CustCode"></param>
        /// <returns></returns>
        public List<dtOfficeChief> GetSiteRelatedOfficeChief(string CustCode)
        {
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;

            List<doSite> siteList = sitehandler.GetSite(null, CustCode);
            List<dtContractOfAllSite> res = GetReleatedContractOfCustIncident(CreateSiteCodeList(siteList));
            string strIncidentOffice = "";

            var condAL = from a in res where (a.ProductTypeCode == ProductType.C_PROD_TYPE_AL) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE) orderby a.ContractCode ascending select a;
            if (condAL.Count() > 0)
            {
                strIncidentOffice = condAL.ToList()[0].OperationOfficeCode;
            }
            else
            {
                var condSale = from a in res where (a.ProductTypeCode == ProductType.C_PROD_TYPE_SALE) orderby a.ContractCode ascending select a;
                if (condSale.Count() > 0)
                {
                    strIncidentOffice = condSale.ToList()[0].OperationOfficeCode;
                }
                else
                {
                    var condMA = from a in res where (a.ProductTypeCode == ProductType.C_PROD_TYPE_MA) orderby a.ContractCode ascending select a;
                    if (condMA.Count() > 0)
                    {
                        strIncidentOffice = condMA.ToList()[0].OperationOfficeCode;
                    }
                    else
                    {
                        var condBE = from a in res where (a.ProductTypeCode == ProductType.C_PROD_TYPE_BE) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_SG) orderby a.ContractCode ascending select a;
                        if (condBE.Count() > 0)
                        {
                            strIncidentOffice = condBE.ToList()[0].OperationOfficeCode;
                        }
                    }
                }
            }

            List<dtOfficeChief> empLst = emphandler.GetOfficeChiefList(strIncidentOffice);
            return empLst;
        }

        /// <summary>
        /// Get chief of related office with site incident
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <returns></returns>
        public List<dtOfficeChief> GetContractRelatedOfficeChief(string SiteCode)
        {
            IViewContractHandler viewconthandler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            List<dtContractsSameSite> res = viewconthandler.GetContractsSameSiteList(SiteCode);
            string strIncidentOffice = "";

            var condAL = from a in res where (a.ProductTypeCode == ProductType.C_PROD_TYPE_AL) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE) orderby a.ContractCode ascending select a;
            if (condAL.Count() > 0)
            {
                strIncidentOffice = condAL.ToList()[0].OperationOfficeCode;
            }
            else
            {
                var condSale = from a in res where (a.ProductTypeCode == ProductType.C_PROD_TYPE_SALE) orderby a.ContractCode ascending select a;
                if (condSale.Count() > 0)
                {
                    strIncidentOffice = condSale.ToList()[0].OperationOfficeCode;
                }
                else
                {
                    var condMA = from a in res where (a.ProductTypeCode == ProductType.C_PROD_TYPE_MA) orderby a.ContractCode ascending select a;
                    if (condMA.Count() > 0)
                    {
                        strIncidentOffice = condMA.ToList()[0].OperationOfficeCode;
                    }
                    else
                    {
                        var condBE = from a in res where (a.ProductTypeCode == ProductType.C_PROD_TYPE_BE) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_SG) orderby a.ContractCode ascending select a;
                        if (condBE.Count() > 0)
                        {
                            strIncidentOffice = condBE.ToList()[0].OperationOfficeCode;
                        }
                    }
                }
            }

            List<dtOfficeChief> empLst = emphandler.GetOfficeChiefList(strIncidentOffice);
            return empLst;
        }

        /// <summary>
        /// Get incident history data
        /// </summary>
        /// <param name="IncidentID"></param>
        /// <returns></returns>
        public List<dtIncidentHistory> GetIncidentHistoryData(int IncidentID)
        {
            return base.GetIncidentHistoryData(IncidentID, MiscType.C_INCIDENT_INTERACTION_TYPE);
        }

        /// <summary>
        /// Get incident detail data
        /// </summary>
        /// <param name="IncidentID"></param>
        /// <returns></returns>
        public dsIncidentDetail GetIncidentDetail(int IncidentID)
        {
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            SECOM_AJIS.DataEntity.Contract.dsIncidentDetail result = new dsIncidentDetail();

            try
            {
                List<dtIncident> incidentDat = GetIncidentData(IncidentID
                    , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CONTRACT
                    , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_CUSTOMER
                    , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_SITE
                    , IncidentRelevant.C_INCIDENT_RELEVANT_TYPE_PROJECT
                    , MiscType.C_INCIDENT_TYPE
                    , MiscType.C_REASON_TYPE);

                List<dtIncidentRole> incidentRoleDat = GetIncidentRoleData(IncidentID);

                List<dtIncidentHistory> incidentHisDat = GetIncidentHistoryData(IncidentID);

                List<tbt_IncidentHistoryDetail> incidentHisDetailDat = new List<tbt_IncidentHistoryDetail>();
                foreach (var item in incidentHisDat)
                {
                    incidentHisDetailDat.AddRange(GetTbt_IncidentHistoryDetail(item.IncidentHistoryID));
                }

                List<tbt_AttachFile> attachDat = commonhandler.GetAttachFile(IncidentID.ToString());

                if (incidentDat != null && incidentDat.Count == 1)
                {
                    result.dtIncident = incidentDat[0];
                }

                List<dtEmployeeOffice> empListDat = new List<dtEmployeeOffice>();
                foreach (var item in incidentHisDat)
                {
                    empListDat.AddRange(emphandler.GetEmployeeOffice(item.CreateBy, true));
                }
            
                result.dtIncidentRole = incidentRoleDat;
                result.dtIncidentHistory = incidentHisDat;
                result.dtEmployeeOffice = empListDat;
                result.Tbt_IncidentHistoryDetail = incidentHisDetailDat;
                result.Tbt_AttachFile = attachDat;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Get incident role data
        /// </summary>
        /// <param name="IncidentID"></param>
        /// <returns></returns>
        public List<dtIncidentRole> GetIncidentRoleData(int IncidentID)
        {
            return base.GetIncidentRoleData(IncidentID, MiscType.C_INCIDENT_ROLE, IncidentRole.C_INCIDENT_ROLE_CHIEF_OF_RELATED_OFFICE);
        }

        /// <summary>
        /// Delete incident role by id (list)
        /// </summary>
        /// <param name="IncidentRoleID"></param>
        /// <returns></returns>
        public List<bool> DeleteTbt_IncidentRole(List<int> IncidentRoleID)
        {
            List<bool> resList = new List<bool>();

            foreach (var item in IncidentRoleID)
            {
                resList.Add(DeleteTbt_IncidentRole(item));
            }

            return resList;
        }

        /// <summary>
        /// Delete incident role by id
        /// </summary>
        /// <param name="IncidentRoleID"></param>
        /// <returns></returns>
        public bool DeleteTbt_IncidentRole(int IncidentRoleID)
        {
            List<tbt_IncidentRole> lst = base.DeleteTbt_IncidentRole(IncidentRoleID);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                logData.TableName = TableName.C_TBL_NAME_INCIDENT_ROLE;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return (lst.Count  == 1);
        }

        /// <summary>
        /// Update incident data (list)
        /// </summary>
        /// <param name="tbt_IncidentLst"></param>
        /// <returns></returns>
        public List<tbt_Incident> UpdateTbt_Incident(List<tbt_Incident> tbt_IncidentLst)
        {
            string updBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime updDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            List<tbt_Incident> oldLst = new List<tbt_Incident>();

            if (tbt_IncidentLst.Count > 0)
            {
                oldLst = GetTbt_Incident(oldLst[0].IncidentID);
            }

            foreach (var item in tbt_IncidentLst)
            {
                var oldItem = from a in oldLst where a.IncidentID == item.IncidentID select a;
                if (oldItem.Count() == 1)
                {
                    var tmpOld = oldItem.First();
                    if (DateTime.Compare(tmpOld.UpdateDate.GetValueOrDefault(), item.UpdateDate.GetValueOrDefault()) != 0)
                    {
                        //throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON
                        //    , MessageUtil.MessageList.MSG0019, null).Message);
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019); //Modify by Jutarat A. on 18022013
                    }
                }

                item.UpdateBy = updBy;
                item.UpdateDate = updDate;
            }

            string xml = CommonUtil.ConvertToXml_Store(tbt_IncidentLst);

            var lst = base.UpdateTbt_Incident(xml);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_INCIDENT;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lst;
        }

        /// <summary>
        /// Update incident data
        /// </summary>
        /// <param name="tbt_Incident"></param>
        /// <returns></returns>
        public List<tbt_Incident> UpdateTbt_Incident(tbt_Incident tbt_Incident)
        {
            var oldItem = GetTbt_Incident(tbt_Incident.IncidentID);

            if (oldItem.Count() == 1)
            {
                var tmpOld = oldItem.First();
                if (DateTime.Compare(tmpOld.UpdateDate.GetValueOrDefault(), tbt_Incident.UpdateDate.GetValueOrDefault()) != 0)
                {
                    //throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON
                    //    , MessageUtil.MessageList.MSG0019, null).Message);
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019); //Modify by Jutarat A. on 18022013
                }
            }

            string updBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime updDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            List<tbt_Incident> updateList = new List<Contract.tbt_Incident>();

            tbt_Incident.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            tbt_Incident.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            tbt_Incident.UpdateBy = updBy;
            tbt_Incident.UpdateDate = updDate;

            updateList.Add(tbt_Incident);
            string xml = CommonUtil.ConvertToXml_Store(updateList);

            var lst = base.UpdateTbt_Incident(xml);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_INCIDENT;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lst;
        }

        /// <summary>
        /// Update incident role data (list)
        /// </summary>
        /// <param name="tbt_Incident"></param>
        /// <returns></returns>
        public List<tbt_IncidentRole> UpdateTbt_IncidentRole(List<tbt_IncidentRole> tbt_IncidentRoleLst)
        {
            string updBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime updDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            List<tbt_IncidentRole> oldLst = new List<tbt_IncidentRole>();

            if (tbt_IncidentRoleLst.Count > 0)
            {
                oldLst = GetTbt_IncidentRole(tbt_IncidentRoleLst[0].IncidentID, null);
            }

            foreach (var item in tbt_IncidentRoleLst)
            {
                var oldItem = from a in oldLst where a.IncidentID == item.IncidentID select a;
                if (oldItem.Count() == 1)
                {
                    var tmpOld = oldItem.First();
                    if (DateTime.Compare(tmpOld.UpdateDate.GetValueOrDefault(), item.UpdateDate.GetValueOrDefault()) != 0)
                    {
                        //throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON
                        //    , MessageUtil.MessageList.MSG0019, null).Message);
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019); //Modify by Jutarat A. on 18022013
                    }
                }

                item.UpdateBy = updBy;
                item.UpdateDate = updDate;
            }

            string xml = CommonUtil.ConvertToXml_Store(tbt_IncidentRoleLst);

            var lst = base.UpdateTbt_IncidentRole(xml);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_INCIDENT_ROLE;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lst;
        }

        /// <summary>
        /// Update incident role data
        /// </summary>
        /// <param name="tbt_Incident"></param>
        /// <returns></returns>
        public List<tbt_IncidentRole> UpdateTbt_IncidentRole(tbt_IncidentRole tbt_IncidentRole)
        {
            string updBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime updDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            List<tbt_IncidentRole> updateList = new List<tbt_IncidentRole>();
            updateList.Add(tbt_IncidentRole);

            List<tbt_IncidentRole> oldLst = new List<tbt_IncidentRole>();

            if (updateList.Count > 0)
            {
                oldLst = GetTbt_IncidentRole(updateList[0].IncidentID, null);
            }

            foreach (var item in updateList)
            {
                var oldItem = from a in oldLst where a.IncidentID == item.IncidentID select a;
                if (oldItem.Count() == 1)
                {
                    var tmpOld = oldItem.First();
                    if (DateTime.Compare(tmpOld.UpdateDate.GetValueOrDefault(), item.UpdateDate.GetValueOrDefault()) != 0)
                    {
                        //throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON
                        //    , MessageUtil.MessageList.MSG0019, null).Message);
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019); //Modify by Jutarat A. on 18022013
                    }
                }

                item.UpdateBy = updBy;
                item.UpdateDate = updDate;
            }

            string xml = CommonUtil.ConvertToXml_Store(updateList);

            var lst = base.UpdateTbt_IncidentRole(xml);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_INCIDENT_ROLE;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lst;
        }

        /// <summary>
        /// Insert new incident history (list)
        /// </summary>
        /// <param name="tbt_IncidentHistoryLst"></param>
        /// <returns></returns>
        public List<tbt_IncidentHistory> InsertTbt_IncidentHistory(List<tbt_IncidentHistory> tbt_IncidentHistoryLst)
        {
            string crtBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime crtDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            foreach (var item in tbt_IncidentHistoryLst)
            {
                item.CreateBy = crtBy;
                item.CreateDate = crtDate;
                item.UpdateBy = crtBy;
                item.UpdateDate = crtDate;
            }

            string xml = CommonUtil.ConvertToXml_Store(tbt_IncidentHistoryLst);

            var lst = base.InsertTbt_IncidentHistory(xml);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_INCIDENT_HISTORY;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lst;
        }

        /// <summary>
        /// Insert new incident history
        /// </summary>
        /// <param name="tbt_IncidentHistory"></param>
        /// <returns></returns>
        public List<tbt_IncidentHistory> InsertTbt_IncidentHistory(tbt_IncidentHistory tbt_IncidentHistory)
        {
            string crtBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime crtDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            tbt_IncidentHistory.CreateBy = crtBy;
            tbt_IncidentHistory.CreateDate = crtDate;
            tbt_IncidentHistory.UpdateBy = crtBy;
            tbt_IncidentHistory.UpdateDate = crtDate;

            List<tbt_IncidentHistory> tmpLst = new List<Contract.tbt_IncidentHistory>();
            tmpLst.Add(tbt_IncidentHistory);

            string xml = CommonUtil.ConvertToXml_Store(tmpLst);

            var lst = base.InsertTbt_IncidentHistory(xml);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_INCIDENT_HISTORY;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lst;
        }

        /// <summary>
        /// Insert incident history detail
        /// </summary>
        /// <param name="tbt_IncidentHistoryDetailLst"></param>
        /// <returns></returns>
        public List<tbt_IncidentHistoryDetail> InsertTbt_IncidentHistoryDetail(List<tbt_IncidentHistoryDetail> tbt_IncidentHistoryDetailLst)
        {
            string crtBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime crtDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            foreach (var item in tbt_IncidentHistoryDetailLst)
            {
                item.CreateBy = crtBy;
                item.CreateDate = crtDate;
                item.UpdateBy = crtBy;
                item.UpdateDate = crtDate;
            }

            string xml = CommonUtil.ConvertToXml_Store(tbt_IncidentHistoryDetailLst);

            var lst = base.InsertTbt_IncidentHistoryDetail(xml);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_INCIDENT_HISTORY_DETAIL;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lst;
        }

        /// <summary>
        /// Insert incident history
        /// </summary>
        /// <param name="tbt_IncidentHistoryDetail"></param>
        /// <returns></returns>
        public List<tbt_IncidentHistoryDetail> InsertTbt_IncidentHistory(tbt_IncidentHistoryDetail tbt_IncidentHistoryDetail)
        {
            string crtBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime crtDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            tbt_IncidentHistoryDetail.CreateBy = crtBy;
            tbt_IncidentHistoryDetail.CreateDate = crtDate;
            tbt_IncidentHistoryDetail.UpdateBy = crtBy;
            tbt_IncidentHistoryDetail.UpdateDate = crtDate;

            List<tbt_IncidentHistoryDetail> tmpLst = new List<Contract.tbt_IncidentHistoryDetail>();
            tmpLst.Add(tbt_IncidentHistoryDetail);

            string xml = CommonUtil.ConvertToXml_Store(tmpLst);

            var lst = base.InsertTbt_IncidentHistoryDetail(xml);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_INCIDENT_HISTORY_DETAIL;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lst;
        }

        /// <summary>
        /// Update incident detail data
        /// </summary>
        /// <param name="incidentDetail"></param>
        /// <returns></returns>
        public bool UpdateIncidentDetail(dsIncidentDetailIn incidentDetail)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool result = false;

            try
            {
                var lastIncident = GetTbt_Incident(incidentDetail.dtIncident.IncidentID);
                if ((lastIncident.Count != 1) || (DateTime.Compare(lastIncident[0].UpdateDate.GetValueOrDefault(), incidentDetail.dtIncident.UpdateDate.GetValueOrDefault()) != 0))
                {
                    //throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019).Message);
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019); //Modify by Jutarat A. on 18022013
                }

                // Update Incident
                tbt_Incident currIncident = lastIncident[0];
                currIncident.IncidentNo = incidentDetail.dtIncident.IncidentNo;
                currIncident.IncidentOfficeCode = incidentDetail.dtIncident.IncidentOfficeCode;
                currIncident.IncidentStatus = incidentDetail.dtIncident.IncidentStatus;
                currIncident.InteractionType = incidentDetail.dtIncident.InteractionType;
                currIncident.DueDate = incidentDetail.dtIncident.DueDate;
                currIncident.DueDateTime = incidentDetail.dtIncident.DueDateTime;
                currIncident.DeadLine = incidentDetail.dtIncident.DeadLine;
                currIncident.DeadLineTime = incidentDetail.dtIncident.DeadLineTime;
                currIncident.hasRespondingDetailFlag = incidentDetail.dtIncident.hasRespondingDetailFlag;

                var updIncdLst = UpdateTbt_Incident(currIncident);

                var addIncdRoleLst = InsertTbt_IncidentRole(incidentDetail.tbt_IncidentRoleAdd);
                var editIncdRoleLst = UpdateTbt_IncidentRole(incidentDetail.tbt_IncidentRoleEdit);
                var delIncdRoleLst = DeleteTbt_IncidentRole(incidentDetail.tbt_IncidentRoleDelete);

                foreach (var item in incidentDetail.tbt_IncidentRoleDelete)
                {
                    delIncdRoleLst.Add(DeleteTbt_IncidentRole(item));
                }

                var addHistIncd = InsertTbt_IncidentHistory(incidentDetail.tbt_IncidentHistory);

                foreach (var item in incidentDetail.tbt_IncidentHistoryDetail)
                {
                    item.IncidentHistoryID = addHistIncd[0].IncidentHistoryID;
                }

                var addHistDetailIncdLst = InsertTbt_IncidentHistoryDetail(incidentDetail.tbt_IncidentHistoryDetail);

                //List<tbt_AttachFile> tmpFileList = commonhandler.GetTbt_AttachFile(updIncdLst[0].IncidentID.ToString(), null, false);
                //if (tmpFileList != null && tmpFileList.Count > 0)
                //{
                    commonhandler.UpdateFlagAttachFile(AttachmentModule.Incident, updIncdLst[0].IncidentID.ToString(), updIncdLst[0].IncidentID.ToString());
                //}
                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
    }
}
