using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IARHandler
    {
        /// <summary>
        /// To generate AR approve no.
        /// </summary>
        /// <param name="strARInteractionType"></param>
        /// <returns></returns>
        string GenerateARApproveNo(String strARStatus); //strARInteractionType //Modify by Jutarat A. on 28082012

        /// <summary>
        /// To generate AR request no.
        /// </summary>
        /// <param name="strARRelevantType"></param>
        /// <param name="strARRelevantCode"></param>
        /// <returns></returns>
        string[] GenerateARRequestNo(String strARRelevantType, String strARRelevantCode);

        /// <summary>
        /// Get AR information
        /// </summary>
        /// <param name="conditionByRole"></param>
        /// <returns></returns>
        List<dtARList> GetARListByRole(doSearchARListByRole conditionByRole);

        /// <summary>
        /// Search AR data
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<dtARList> SearchARList(doSearchARListCondition condition);

        /// <summary>
        /// Summary AR data
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        List<dtSummaryAR> SummaryAR(DateTime? dateFrom, DateTime? dateTo, DateTime? currentDate);

        /// <summary>
        /// Get AR data
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<dtARListCTS370> GetARList(doRetrieveARListCondition condition);

        /// <summary>
        /// Get occurring site list
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        List<dtAROccSite> GetOccurringSiteList(string custCode);

        /// <summary>
        /// Get occurring contract list
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        List<dtAROccContract> GetOccurringContractList(string siteCode);

        /// <summary>
        /// Get AR type title data
        /// </summary>
        /// <param name="strARType"></param>
        /// <param name="strARTitleType"></param>
        /// <returns></returns>
        List<tbs_ARTypeTitle> GetTbs_ARTypeTitle(string strARType, string strARTitleType);

        /// <summary>
        /// Get AR type pattern data
        /// </summary>
        /// <param name="strARType"></param>
        /// <param name="strARTitleType"></param>
        /// <returns></returns>
        List<tbs_ARTypePattern> GetTbs_ARTypePattern(string strARType, string strARTitleType);

        /// <summary>
        /// Get related contract code of AR that related with customer
        /// </summary>
        /// <param name="requestNo"></param>
        /// <returns></returns>
        List<dtRelatedOfficeChief> GetReleatedContractOfCustAR(string requestNo);

        /// <summary>
        /// Get chief of related office with site AR 
        /// </summary>
        /// <param name="strSiteCode"></param>
        /// <returns></returns>
        List<dtOfficeChief> GetContractRelatedOfficeChief(string strSiteCode);

        /// <summary>
        /// Get chief of related office with customer AR 
        /// </summary>
        /// <param name="RequestNo"></param>
        /// <returns></returns>
        List<dtOfficeChief> GetSiteRelatedOfficeChief(string RequestNo);

        /// <summary>
        /// Insert AR data (list)
        /// </summary>
        /// <param name="listAR"></param>
        /// <returns></returns>
        List<tbt_AR> InsertTbt_AR(List<tbt_AR> listAR);

        /// <summary>
        /// Insert AR data
        /// </summary>
        /// <param name="itemAR"></param>
        /// <returns></returns>
        tbt_AR InsertTbt_AR(tbt_AR itemAR);

        /// <summary>
        /// Insert AR role data (list)
        /// </summary>
        /// <param name="listARRole"></param>
        /// <returns></returns>
        List<tbt_ARRole> InsertTbt_ARRole(List<tbt_ARRole> listARRole);

        /// <summary>
        /// Insert AR role data
        /// </summary>
        /// <param name="itemARRole"></param>
        /// <returns></returns>
        tbt_ARRole InsertTbt_ARRole(tbt_ARRole itemARRole);

        /// <summary>
        /// Insert AR fee adjustment data (list)
        /// </summary>
        /// <param name="listARFee"></param>
        /// <returns></returns>
        List<tbt_ARFeeAdjustment> InsertTbt_ARFeeAdjustment(List<tbt_ARFeeAdjustment> listARFee);

        /// <summary>
        /// Insert AR fee adjustment data
        /// </summary>
        /// <param name="itemARFee"></param>
        /// <returns></returns>
        tbt_ARFeeAdjustment InsertTbt_ARFeeAdjustment(tbt_ARFeeAdjustment itemARFee);

        /// <summary>
        /// To generate and send email of AR
        /// </summary>
        /// <param name="strMailTo"></param>
        /// <param name="strStatus"></param>
        /// <param name="strLinkEN"></param>
        /// <param name="strLinkLC"></param>
        /// <returns></returns>
        bool SendAREmail(string strMailTo, string strStatus, string strLinkEN, string strLinkLC, tbt_AR ARData, string strAuditDetailHistory = null); //Add strLinkLC,ARData,strAuditDetailHistory by Jutarat A. on 28092012

        /// <summary>
        /// Get AR role data
        /// </summary>
        /// <param name="pRequestNo"></param>
        /// <returns></returns>
        List<tbt_ARRole> GetTbt_ARRole(string pRequestNo);

        /// <summary>
        /// Get AR data
        /// </summary>
        /// <param name="pRequestNo"></param>
        /// <returns></returns>
        List<dtAR> GetARData(string pRequestNo);

        /// <summary>
        /// Get chief of AR office or chief of branch of AR office
        /// </summary>
        /// <param name="pRequestNo"></param>
        /// <param name="C_FLAG_TYPE"></param>
        /// <returns></returns>
        List<dtAROfficeChief> GetAROfficeChief(string pRequestNo, bool? C_FLAG_TYPE);

        /// <summary>
        /// Get chief of department 
        /// </summary>
        /// <param name="pRequestNo"></param>
        /// <param name="C_FLAG_TYPE"></param>
        /// <returns></returns>
        List<dtARDepartmentChief> GetARDepartmentChief(string pRequestNo, bool? C_FLAG_TYPE);

        /// <summary>
        /// Get AR permission configuration data
        /// </summary>
        /// <param name="strRequestNo"></param>
        /// <returns></returns>
        doARPermission HasARPermission(string strRequestNo);

        /// <summary>
        /// Get AR role data
        /// </summary>
        /// <param name="strRequestNo"></param>
        /// <returns></returns>
        List<dtARRole> GetARRoleData(string strRequestNo);

        /// <summary>
        /// Get AR history detail data
        /// </summary>
        /// <param name="pARHistoryID"></param>
        /// <returns></returns>
        List<tbt_ARHistoryDetail> GetTbt_ARHistoryDetail(int? pARHistoryID);

        /// <summary>
        /// Get AR history data
        /// </summary>
        /// <param name="pRequestNo"></param>
        /// <returns></returns>
        List<tbt_ARHistory> GetTbt_ARHistory(string pRequestNo);

        /// <summary>
        /// Get AR fee adjustment data
        /// </summary>
        /// <param name="pRequestNo"></param>
        /// <returns></returns>
        List<tbt_ARFeeAdjustment> GetTbt_ARFeeAdjustment(string pRequestNo);

        /// <summary>
        /// Get AR detail data
        /// </summary>
        /// <param name="strRequestNo"></param>
        /// <returns></returns>
        dsARDetail GetARDetail(string strRequestNo);

        /// <summary>
        /// Update AR data
        /// </summary>
        /// <param name="tbt_AR"></param>
        /// <returns></returns>
        List<tbt_AR> UpdateTbt_AR(List<tbt_AR> tbt_AR);

        /// <summary>
        /// Update AR role data
        /// </summary>
        /// <param name="tbt_ARRole"></param>
        /// <returns></returns>
        List<tbt_ARRole> UpdateTbt_ARRole(List<tbt_ARRole> tbt_ARRole);

        /// <summary>
        /// Update ARFeeAdjustment data
        /// </summary>
        /// <param name="tbt_ARFeeAdjustment"></param>
        /// <returns></returns>
        List<tbt_ARFeeAdjustment> UpdateTbt_ARFeeAdjustment(List<tbt_ARFeeAdjustment> tbt_ARFeeAdjustment); //Add by Jutarat A. on 03042013

        /// <summary>
        /// Delete AR role by id
        /// </summary>
        /// <param name="ARRoleID"></param>
        /// <returns></returns>
        bool DeleteTbt_ARRole(int ARRoleID);

        /// <summary>
        /// Get AR history data
        /// </summary>
        /// <param name="tbt_ARHistory"></param>
        /// <returns></returns>
        List<tbt_ARHistory> InsertTbt_ARHistory(List<tbt_ARHistory> tbt_ARHistory);

        /// <summary>
        /// Get AR history detail data
        /// </summary>
        /// <param name="tbt_ARHistoryDetail"></param>
        /// <returns></returns>
        List<tbt_ARHistoryDetail> InsertTbt_ARHistoryDetail(List<tbt_ARHistoryDetail> tbt_ARHistoryDetail);

        /// <summary>
        /// Get AR data
        /// </summary>
        /// <param name="pRequestNo"></param>
        /// <returns></returns>
        List<tbt_AR> GetTbt_AR(string pRequestNo);

        /// <summary>
        /// Update AR detail data
        /// </summary>
        /// <param name="arDat"></param>
        /// <param name="resARStatus"></param>
        /// <returns></returns>
        bool UpdateARDetail(dsARDetailIn arDat, out string resARStatus);
        
        /// <summary>
        /// Update contract code to AR data when approve contract
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        bool UpdateContractCode(tbt_AR cond);
    }
}
