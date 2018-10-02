using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IIncidentHandler
    {
        /// <summary>
        /// To generate incident no.
        /// </summary>
        /// <param name="strIncidentRelevantType"></param>
        /// <param name="strIncidentRelevantCode"></param>
        /// <param name="strIncidentOfficeCode"></param>
        /// <returns></returns>
        string[] GenerateIncidentNo(string strIncidentRelevantType, string strIncidentRelevantCode, string strIncidentOfficeCode);

        /// <summary>
        /// Get rental contract basic information
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<dtIncidentList> GetIncidentListByRole(doIncidentListByRole condition);

        /// <summary>
        /// Search incident data
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<dtIncidentList> SearchIncidentList(doSearchIncidentListCondition condition);
        
        /// <summary>
        /// Get incident role by id
        /// </summary>
        /// <param name="incidentID"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        List<tbt_IncidentRole> GetTbt_IncidentRole(Nullable<int> incidentID, string empNo);

        /// <summary>
        /// Get incident data
        /// </summary>
        /// <param name="incidentID"></param>
        /// <param name="c_INCIDENT_RELEVANT_TYPE_CONTRACT"></param>
        /// <param name="c_INCIDENT_RELEVANT_TYPE_CUSTOMER"></param>
        /// <param name="c_INCIDENT_RELEVANT_TYPE_SITE"></param>
        /// <param name="c_INCIDENT_RELEVANT_TYPE_PROJECT"></param>
        /// <param name="c_INCIDENT_TYPE"></param>
        /// <param name="c_REASON_TYPE"></param>
        /// <returns></returns>
        List<dtIncident> GetIncidentData(Nullable<int> incidentID, string c_INCIDENT_RELEVANT_TYPE_CONTRACT, string c_INCIDENT_RELEVANT_TYPE_CUSTOMER, string c_INCIDENT_RELEVANT_TYPE_SITE, string c_INCIDENT_RELEVANT_TYPE_PROJECT, string c_INCIDENT_TYPE, string c_REASON_TYPE);

        /// <summary>
        /// Get chief of incident office or chief of branch of incident office
        /// </summary>
        /// <param name="incidentID"></param>
        /// <param name="c_FLAG_ON"></param>
        /// <returns></returns>
        List<string> GetIncidentOfficeChief(Nullable<int> incidentID, Nullable<bool> c_FLAG_ON);

        /// <summary>
        /// Get chief of department 
        /// </summary>
        /// <param name="incidentID"></param>
        /// <param name="c_FLAG_ON"></param>
        /// <returns></returns>
        List<string> GetIncidentDepartmentChief(Nullable<int> incidentID, Nullable<bool> c_FLAG_ON);
        List<tbs_ARPermissionConfiguration> GetTbs_ARPermissionConfiguration(string strPermissionType);

        /// <summary>
        /// Get Permission Configuration for Incident
        /// </summary>
        /// <param name="incidentRole">format ,role1,role2,...,rolen, support query many incident role</param>
        /// <returns></returns>
        List<tbs_IncidentPermissionConfiguration> GetTbs_IncidentPermissionConfiguration(string incidentRole);

        /// <summary>
        /// Get incident permission configuration data
        /// </summary>
        /// <param name="incidentID"></param>
        /// <returns></returns>
        doHasIncidentPermission HasIncidentPermission(int? incidentID);

        /// <summary>
        /// Summary incident data
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="currentdate"></param>
        /// <returns></returns>
        List<dtSummaryIncident> SummaryIncident(Nullable<System.DateTime> dateFrom, Nullable<System.DateTime> dateTo, Nullable<System.DateTime> currentdate);

        /// <summary>
        /// Get incident data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtIncidentListCTS320> GetIncidentList(doRetrieveIncidentListCondition cond);

        /// <summary>
        /// Get incident occurring site data
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        List<dtIncidentOccSite> GetOccurringSiteList(string custCode);
        
        /// <summary>
        /// Get incident occurring contract data
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        List<dtIncidentOccContract> GetOccurringContractList(string siteCode);

        /// <summary>
        /// Get incident type pattern data
        /// </summary>
        /// <param name="strIncidentTypeCode"></param>
        /// <returns></returns>
        List<dtTbs_IncidentTypePattern> GetTbs_IncidentTypePattern(string strIncidentTypeCode);

        /// <summary>
        /// Insert new incident (list)
        /// </summary>
        /// <param name="listIncident"></param>
        /// <returns></returns>
        List<tbt_Incident> InsertTbt_Incident(List<tbt_Incident> listIncident);

        /// <summary>
        /// Insert new incident
        /// </summary>
        /// <param name="itemIncident"></param>
        /// <returns></returns>
        tbt_Incident InsertTbt_Incident(tbt_Incident itemIncident);

        /// <summary>
        /// Insert new incident role (list)
        /// </summary>
        /// <param name="listIncidentRole"></param>
        /// <returns></returns>
        List<tbt_IncidentRole> InsertTbt_IncidentRole(List<tbt_IncidentRole> listIncidentRole);

        /// <summary>
        /// Insert new incident role
        /// </summary>
        /// <param name="itemIncidentRole"></param>
        /// <returns></returns>
        tbt_IncidentRole InsertTbt_IncidentRole(tbt_IncidentRole itemIncidentRole);

        /// <summary>
        /// Get related contract code of incident that related with customer
        /// </summary>
        /// <param name="strSiteCodeList"></param>
        /// <returns></returns>
        List<dtContractOfAllSite> GetReleatedContractOfCustIncident(string strSiteCodeList);

        /// <summary>
        /// Get chief of related office with customer incident 
        /// </summary>
        /// <param name="CustCode"></param>
        /// <returns></returns>
        List<dtOfficeChief> GetSiteRelatedOfficeChief(string CustCode);

        /// <summary>
        /// Get chief of related office with site incident
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <returns></returns>
        List<dtOfficeChief> GetContractRelatedOfficeChief(string SiteCode);

        /// <summary>
        /// Get incident history data
        /// </summary>
        /// <param name="IncidentID"></param>
        /// <returns></returns>
        List<dtIncidentHistory> GetIncidentHistoryData(int IncidentID);

        /// <summary>
        /// Get incident detail data
        /// </summary>
        /// <param name="IncidentID"></param>
        /// <returns></returns>
        dsIncidentDetail GetIncidentDetail(int IncidentID);

        /// <summary>
        /// Get incident role data
        /// </summary>
        /// <param name="IncidentID"></param>
        /// <returns></returns>
        List<dtIncidentRole> GetIncidentRoleData(int IncidentID);

        /// <summary>
        /// Delete incident role by id
        /// </summary>
        /// <param name="IncidentRoleID"></param>
        /// <returns></returns>
        bool DeleteTbt_IncidentRole(int IncidentRoleID);

        /// <summary>
        /// Delete incident role by id (list)
        /// </summary>
        /// <param name="IncidentRoleID"></param>
        /// <returns></returns>
        List<bool> DeleteTbt_IncidentRole(List<int> IncidentRoleID);

        /// <summary>
        /// Update incident data (list)
        /// </summary>
        /// <param name="tbt_IncidentLst"></param>
        /// <returns></returns>
        List<tbt_Incident> UpdateTbt_Incident(List<tbt_Incident> tbt_IncidentLst);

        /// <summary>
        /// Update incident data
        /// </summary>
        /// <param name="tbt_Incident"></param>
        /// <returns></returns>
        List<tbt_Incident> UpdateTbt_Incident(tbt_Incident tbt_Incident);

        /// <summary>
        /// Insert new incident history (list)
        /// </summary>
        /// <param name="tbt_IncidentHistoryLst"></param>
        /// <returns></returns>
        List<tbt_IncidentHistory> InsertTbt_IncidentHistory(List<tbt_IncidentHistory> tbt_IncidentHistoryLst);

        /// <summary>
        /// Insert new incident history
        /// </summary>
        /// <param name="tbt_IncidentHistory"></param>
        /// <returns></returns>
        List<tbt_IncidentHistory> InsertTbt_IncidentHistory(tbt_IncidentHistory tbt_IncidentHistory);

        /// <summary>
        /// Insert incident history detail
        /// </summary>
        /// <param name="tbt_IncidentHistoryDetailLst"></param>
        /// <returns></returns>
        List<tbt_IncidentHistoryDetail> InsertTbt_IncidentHistoryDetail(List<tbt_IncidentHistoryDetail> tbt_IncidentHistoryDetailLst);

        /// <summary>
        /// Insert incident history
        /// </summary>
        /// <param name="tbt_IncidentHistoryDetail"></param>
        /// <returns></returns>
        List<tbt_IncidentHistoryDetail> InsertTbt_IncidentHistory(tbt_IncidentHistoryDetail tbt_IncidentHistoryDetail);

        /// <summary>
        /// Update incident detail data
        /// </summary>
        /// <param name="incidentDetail"></param>
        /// <returns></returns>
        bool UpdateIncidentDetail(dsIncidentDetailIn incidentDetail);

        /// <summary>
        /// Get incident reason type
        /// </summary>
        /// <param name="strIncidentType"></param>
        /// <returns></returns>
        List<tbs_IncidentReasonType> GetTbs_IncidentReasonType(string strIncidentType);
    }
}