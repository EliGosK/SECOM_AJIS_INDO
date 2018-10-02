using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IProjectHandler
    {
        /// <summary>
        /// Getting project data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtProjectData> GetProjectDataForSearch(doSearchProjectCondition cond);

        /// <summary>
        /// To generate project code
        /// </summary>
        /// <returns></returns>
        string GenerateProjectCode();

        /// <summary>
        /// Using when Inventory stock-out intrument by project code
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="doInstrumentList"></param>
        /// <param name="strStockOutMemo"></param>
        void UpdateStockOutInstrument(string strProjectCode, List<doInstrument> doInstrumentList, string strStockOutMemo);

        /// <summary>
        /// Get list of contract data
        /// </summary>
        /// <param name="pProjectCode"></param>
        /// <param name="pC_DOC_AUDIT_RESULT"></param>
        /// <returns></returns>
        List<doProjectContractDetail> GetContractDetailList(string pProjectCode, string pC_DOC_AUDIT_RESULT);

        /// <summary>
        /// Get all tables of project 
        /// </summary>
        /// <param name="pProjectCode"></param>
        /// <returns></returns>
        List<tbt_Project> GetTbt_Project(string pProjectCode);

        /// <summary>
        /// Get all tables of project for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        List<doTbt_Project> GetTbt_ProjectForView(string strProjectCode);

        #region Insertion method
        int InsertTbt_ProjectPurchaserCustomer(tbt_ProjectPurchaserCustomer tbtProjectPurchaser);
        int InsertTbt_ProjectExpectedInstrumentDetail(List<tbt_ProjectExpectedInstrumentDetails> tbtProjectExpectIntrumentDetails);
        int InsertTbt_ProjectOtherRalatedCompany(List<tbt_ProjectOtherRalatedCompany> tbtProjectOtherRelated);
        int InsertTbt_ProjectSupportStaffDetail(List<tbt_ProjectSupportStaffDetails> tbtSupport);
        int InsertTbt_ProjectSystemDetail(List<tbt_ProjectSystemDetails> tbtProductSystem);
        int InsertTbt_Project(List<tbt_Project> tbtProject);

        #endregion

        /// <summary>
        /// Create project data
        /// </summary>
        /// <param name="RegProjectData"></param>
        /// <returns></returns>
        bool CreateProjectData(doRegisterProjectData RegProjectData);

        /// <summary>
        /// Get project support staff detail for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        List<dtTbt_ProjectSupportStaffDetailForView> GetTbt_ProjectSupportStaffDetailForView(string strProjectCode);

        /// <summary>
        /// Get project expected instrument detail for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        List<dtTbt_ProjectExpectedInstrumentDetailsForView> GetTbt_ProjectExpectedInstrumentDetailsForView(string strProjectCode);

        /// <summary>
        /// Get project system detail for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        List<dtTbt_ProjectSystemDetailForView> GetTbt_ProjectSystemDetailForView(string strProjectCode);

        /// <summary>
        /// Get project stockout instrument for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        List<dtTbt_ProjectStockoutIntrumentForView> GetTbt_ProjectStockoutIntrumentForView(string strProjectCode);

        /// <summary>
        /// Get project oter related company for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        List<tbt_ProjectOtherRalatedCompany> GetTbt_ProjectOtherRalatedCompanyForView(string strProjectCode);

        /// <summary>
        /// Get project stockout branch instrument detail
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        List<tbt_ProjectStockoutBranchIntrumentDetails> GetTbt_ProjectStockoutBranchIntrumentDetails(string strProjectCode);

        /// <summary>
        /// Get project purchaser customer for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        List<dtTbt_ProjectPurchaserCustomerForView> GetTbt_ProjectPurchaserCustomerForView(string strProjectCode);

        /// <summary>
        /// Get project stockout branch instrument detail for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="iBranchNo"></param>
        /// <returns></returns>
        List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> GetTbt_ProjectStockoutBranchIntrumentDetailForView(string strProjectCode, int? iBranchNo);

        /// <summary>
        /// Get project status
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        List<string> GetProjectStatus(string strProjectCode);

        /// <summary>
        /// Update project data 
        /// </summary>
        /// <param name="Project"></param>
        /// <returns></returns>
        int UpdateTbt_ProjectData(tbt_Project Project);

        /// <summary>
        /// Update project expected instrument detail
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="strInstrumentCode"></param>
        /// <param name="intInstrumentQty"></param>
        /// <returns></returns>
        List<tbt_ProjectExpectedInstrumentDetails> UpdateTbt_ProjectExpectedInstrumentDetails(string strProjectCode, string strInstrumentCode, int? intInstrumentQty);

        /// <summary>
        /// Update project expected instrument detail
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="instrumentCode"></param>
        /// <returns></returns>
        List<tbt_ProjectExpectedInstrumentDetails> DeleteTbt_ProjectExpectedInstrumentDetails(string strProjectCode, string instrumentCode);

        /// <summary>
        /// Delete project system detail
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        List<tbt_ProjectSystemDetails> DeleteTbt_ProjectSystemDetails(string strProjectCode, string productCode);
        /// <summary>
        /// Delete project support staff detail
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        List<tbt_ProjectSupportStaffDetails> DeleteTbt_ProjectSupportStaffDetails(string strProjectCode, string empNo);

        /// <summary>
        /// Delete project other relate company
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="sequenceNo"></param>
        /// <returns></returns>
        List<tbt_ProjectOtherRalatedCompany> DeleteTbt_ProjectOtherRalatedCompany(string strProjectCode, int sequenceNo);

        /// <summary>
        /// Update project other relate company
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="sequenceNo"></param>
        /// <param name="companyName"></param>
        /// <param name="name"></param>
        /// <param name="telNo"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        List<tbt_ProjectOtherRalatedCompany> UpdateTbt_ProjectOtherRalatedCompany(string strProjectCode, int? sequenceNo, string companyName, string name, string telNo, string remark);

        /// <summary>
        /// Insert project stockout branch instrument detail
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        List<tbt_ProjectStockoutBranchIntrumentDetails> InsertTbt_ProjectStockoutBranchIntrumentDetails(List<tbt_ProjectStockoutBranchIntrumentDetails> lst);

        /// <summary>
        /// Update project stockout branch instrument detail
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="branchNo"></param>
        /// <param name="assignBranchQty"></param>
        /// <param name="instrumentCode"></param>
        /// <returns></returns>
        List<tbt_ProjectStockoutBranchIntrumentDetails> UpdateTbt_ProjectStockoutBranchIntrumentDetails(string strProjectCode, int branchNo, int assignBranchQty, string instrumentCode);

        /// <summary>
        /// Get project information
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        dtProjectForInstall GetProjectDataForInstall(string strProjectCode);

        /// <summary>
        /// Update project status
        /// </summary>
        /// <param name="Project"></param>
        /// <param name="NewProjectStatus"></param>
        /// <returns></returns>
        int UpdateProjectStatus(string Project, string NewProjectStatus);

        /// <summary>
        /// Update project purchaser customer
        /// </summary>
        /// <param name="PurchaserCustomer"></param>
        /// <returns></returns>
        List<tbt_ProjectPurchaserCustomer> UpdateTbt_ProjectPurchaseCustomer(tbt_ProjectPurchaserCustomer PurchaserCustomer);

        /// <summary>
        /// Get project stocketout memo for view
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        List<tbt_ProjectStockOutMemo> GetTbt_ProjectStockoutMemoForView(string projectCode);

        /// <summary>
        /// Get project stockout branch
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        List<doProjectBranch> GetProjectStockOutBranch(string projectCode);

        /// <summary>
        /// Check exist project code 
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        List<bool?> IsProjectExist(string strProjectCode);
        
        /// <summary>
        /// Insert project stockout memo
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        List<tbt_ProjectStockOutMemo> InsertTbt_ProjectStockOutMemo(tbt_ProjectStockOutMemo doInsert);
    }
}
