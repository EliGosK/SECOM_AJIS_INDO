using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Installation
{
    public interface IInstallationHandler
    {
        /// <summary>
        /// Generate Installation MA No.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        string GenerateInstallationMANo(doGenerateInstallationMANo cond);
        /// <summary>
        /// Generate Installation Slip No.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        string GenerateInstallationSlipNo(doGenerateInstallationSlipNoCond cond);
        /// <summary>
        /// Check registered Installation
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        bool CheckInstallationRegister(string strContractCode);
        /// <summary>
        /// Insert tbt_installationBasic
        /// </summary>
        /// <param name="doTbt_InstallationBasic"></param>
        /// <returns></returns>
        int InsertTbt_InstallationBasic(tbt_InstallationBasic doTbt_InstallationBasic);
        /// <summary>
        /// Get data tbt_installationBasic
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        List<tbt_InstallationBasic> GetTbt_InstallationBasicData(string strContractProjectCode);
        /// <summary>
        /// Get data tbt_installationManagement
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <returns></returns>
        List<tbt_InstallationManagement> GetTbt_InstallationManagementData(string strMaintenanceNo);
        /// <summary>
        /// Check all removal
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        bool CheckAllRemoval(string strContractCode);
        /// <summary>
        /// Insert data tbt_installationManagement
        /// </summary>
        /// <param name="doTbt_InstallationManagement"></param>
        /// <returns></returns>
        int InsertTbt_InstallationManagement(tbt_InstallationManagement doTbt_InstallationManagement);
        /// <summary>
        /// Update tbt_installationBasic
        /// </summary>
        /// <param name="doTbt_InstallationBasic"></param>
        /// <returns></returns>
        int UpdateTbt_InstallationBasic(tbt_InstallationBasic doTbt_InstallationBasic);
        /// <summary>
        /// Insert tbt_installationmemo
        /// </summary>
        /// <param name="doTbt_InstallationMemo"></param>
        /// <returns></returns>
        int InsertTbt_InstallationMemo(tbt_InstallationMemo doTbt_InstallationMemo);
        /// <summary>
        /// Insert tbt_installationemail
        /// </summary>
        /// <param name="doTbt_InstallationEmail"></param>
        /// <returns></returns>
        int InsertTbt_InstallationEmail(tbt_InstallationEmail doTbt_InstallationEmail);
        /// <summary>
        /// Gete data tbt_installationPOManagement
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <returns></returns>
        List<tbt_InstallationPOManagement> GetTbt_InstallationPOManagementData(string strMaintenanceNo);
        /// <summary>
        /// Get data tbt_installationEmail
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <returns></returns>
        List<tbt_InstallationEmail> GetTbt_InstallationEmailData(string strMaintenanceNo);
        /// <summary>
        /// insert tbt_installationPOManagement
        /// </summary>
        /// <param name="doTbt_InstallationPOManagement"></param>
        /// <returns></returns>
        int InsertTbt_InstallationPOManagement(tbt_InstallationPOManagement doTbt_InstallationPOManagement);
        /// <summary>
        /// Update tbt_installationPOManagement
        /// </summary>
        /// <param name="doTbt_InstallationPOManagement"></param>
        /// <returns></returns>
        int UpdateTbt_InstallationPOManagement(tbt_InstallationPOManagement doTbt_InstallationPOManagement);
        /// <summary>
        /// Update tbt_installationManagement
        /// </summary>
        /// <param name="doTbt_InstallationManagement"></param>
        /// <returns></returns>
        int UpdateTbt_InstallationManagement(tbt_InstallationManagement doTbt_InstallationManagement);
        /// <summary>
        /// Get tbt_installationSlip
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        tbt_InstallationSlip GetTbt_InstallationSlipData(string strSlipNo);
        /// <summary>
        /// Get tbt_installationSlip List
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <returns></returns>
        List<tbt_InstallationSlip> GetTbt_InstallationSlip(string strSlipNo); //Add by Jutarat A. on 25062013
        /// <summary>
        /// Get tbt_installationInstrumentDetails
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strInstrumentCode"></param>
        /// <returns></returns>
        List<tbt_InstallationInstrumentDetails> GetTbt_InstallationInstrumentDetailsData(string strContractCode, string strInstrumentCode);
        /// <summary>
        /// Insert tbt_installationSlip
        /// </summary>
        /// <param name="doTbt_InstallationSlip"></param>
        /// <returns></returns>
        int InsertTbt_InstallationSlip(tbt_InstallationSlip doTbt_InstallationSlip);
        /// <summary>
        /// Insert tbt_installationSlipDetails
        /// </summary>
        /// <param name="doTbt_InstallationSlipDetails"></param>
        /// <returns></returns>
        int InsertTbt_InstallationSlipDetails(tbt_InstallationSlipDetails doTbt_InstallationSlipDetails);
        /// <summary>
        /// Update tbt_installationSlip
        /// </summary>
        /// <param name="doTbt_InstallationSlip"></param>
        /// <returns></returns>
        int UpdateTbt_InstallationSlip(tbt_InstallationSlip doTbt_InstallationSlip);
        /// <summary>
        /// Get data tbt_installationSlipDetails
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <param name="strInstrumentCode"></param>
        /// <param name="strInstrumentTypeCode"></param>
        /// <returns></returns>
        List<tbt_InstallationSlipDetails> GetTbt_InstallationSlipDetailsData(string strSlipNo, string strInstrumentCode,string strInstrumentTypeCode);
        /// <summary>
        /// Update tbt_installationSlipDetail
        /// </summary>
        /// <param name="doTbt_InstallationSlipDetails"></param>
        /// <returns></returns>
        int UpdateTbt_InstallationSlipDetails(tbt_InstallationSlipDetails doTbt_InstallationSlipDetails);
        /// <summary>
        /// Delete tbt_installationInstrumentDetails
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strInstrumentCode"></param>
        /// <returns></returns>
        List<tbt_InstallationInstrumentDetails> DeleteTbt_InstallationInstrumentDetail(string strContractCode, string strInstrumentCode);
        /// <summary>
        /// Delete tbt_installationBasic
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        List<tbt_InstallationBasic> DeleteTbt_InstallationBasic(string strContractProjectCode);
        /// <summary>
        /// Insert tbt_installationHistory
        /// </summary>
        /// <param name="doTbt_InstallationHistory"></param>
        /// <returns></returns>
        List<tbt_InstallationHistory> InsertTbt_InstallationHistory(tbt_InstallationHistory doTbt_InstallationHistory);
        /// <summary>
        /// Insert tbt_installationHistoryDetails
        /// </summary>
        /// <param name="doTbt_InstallationHistoryDetail"></param>
        /// <returns></returns>
        int InsertTbt_InstallationHistoryDetail(tbt_InstallationHistoryDetails doTbt_InstallationHistoryDetail);
        /// <summary>
        /// Delete tbt_installationMemo
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <param name="strReferenceID"></param>
        /// <param name="strObjectID"></param>
        /// <returns></returns>
        List<tbt_InstallationMemo> DeleteTbt_InstallationMemo(string strContractProjectCode, string strReferenceID, string strObjectID);
        /// <summary>
        /// Delete tbt_installationPOManagement
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <returns></returns>
        List<tbt_InstallationPOManagement> DeleteTbt_InstallationPOManagement(string strMaintenanceNo);
        /// <summary>
        /// Search data Installation management list
        /// </summary>
        /// <param name="doCondition"></param>
        /// <returns></returns>
        List<doSearchInstallManagementResult> SearchInstallationManagementList(doSearchInstallManageCriteria doCondition);
        /// <summary>
        /// Check cancel Installation Management
        /// </summary>
        /// <param name="strMaintenanceNo"></param>
        /// <returns></returns>
        bool CheckCancelInstallationManagement(string strMaintenanceNo);
        /// <summary>
        /// Get data tbt_installtionMemo
        /// </summary>
        /// <param name="strReferenceID"></param>
        /// <returns></returns>
        List<tbt_InstallationMemo> GetTbt_InstallationMemo(string strReferenceID);
        /// <summary>
        /// Generate Installation Basic data
        /// </summary>
        /// <param name="doGenInstallationBasicData"></param>
        /// <returns></returns>
        bool GenerateInstallationBasic(doGenInstallationBasic doGenInstallationBasicData);
        /// <summary>
        /// Delete installation basic data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        bool DeleteInstallationBasicData(string strContractCode);
        /// <summary>
        /// recieve return instrument
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <param name="strReturnOfficecode"></param>
        /// <returns></returns>
        bool ReceiveReturnInstrument(string strSlipNo, string strReturnOfficecode);
        /// <summary>
        /// Get installation status
        /// </summary>
        /// <param name="strContractProjectCode"></param>
        /// <returns></returns>
        string GetInstallationStatus(string strContractProjectCode);
        /// <summary>
        /// Check can open screen installation 
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        string CheckInstallationDataToOpenScreenData(string strCode);
        /// <summary>
        /// Update stockout instrument
        /// </summary>
        /// <param name="strSlipNo"></param>
        /// <param name="blnStockOutFlag"></param>
        /// <param name="strStockOutOfficeCode"></param>
        /// <param name="doInstrumentlist"></param>
        /// <returns></returns>
        bool UpdateStockOutInstrument(string strSlipNo, bool blnStockOutFlag, string strStockOutOfficeCode, List<doInstrument> doInstrumentlist, DateTime? InstallSlipUpdateDate = null); //Modify by Jutarat A. on 27112012 (Add InstallSlipUpdateDate)
        /// <summary>
        /// Insert tbt_installationAttachFile
        /// </summary>
        /// <param name="doTbt_AttachFile"></param>
        /// <returns></returns>
        int InsertTbt_InstallationAttachFile(tbt_InstallationAttachFile doTbt_AttachFile);
        /// <summary>
        /// Generate installation slip document
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        bool GenerateInstallationSlipDoc(string strContractCode, bool? isCheckSlipIssueFlag = true); //Add isCheckSlipIssueFlag by Jutarat A. on 25072013
        /// <summary>
        /// Get normal removal fee
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        decimal GetNormalRemovalFee(string strContractCode);
        /// <summary>
        /// Get approve email
        /// </summary>
        /// <returns></returns>
        List<dtRequestApproveInstallation> GetEmailForApprove();
        /// <summary>
        /// Get data tbt_installationhistory
        /// </summary>
        /// <param name="ContractProjectCode"></param>
        /// <param name="MaintenanceNo"></param>
        /// <param name="HistoryNo"></param>
        /// <returns></returns>
        List<tbt_InstallationHistory> GetTbt_InstallationHistory(string ContractProjectCode, string MaintenanceNo, Nullable<int> HistoryNo);
        /// <summary>
        /// Get installation data for view
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtInstallation> GetInstallationDataListForView(doSearchInstallationCondition cond);
        /// <summary>
        /// Get data installation PO management for view 
        /// </summary>
        /// <param name="pMaintenanceNo"></param>
        /// <returns></returns>
        List<dtInstallationPOManagementForView> GetTbt_InstallationPOManagementForView(string pMaintenanceNo);
        /// <summary>
        /// Get data tbt_installationhistory for view
        /// </summary>
        /// <param name="c_SERVICE_TYPE_SALE"></param>
        /// <param name="c_SERVICE_TYPE_RENTAL"></param>
        /// <param name="c_SALE_INSTALL_TYPE"></param>
        /// <param name="c_RENTAL_INSTALL_TYPE"></param>
        /// <param name="c_CHANGE_REASON_TYPE_CUSTOMER"></param>
        /// <param name="c_CHANGE_REASON_TYPE_SECOM"></param>
        /// <param name="c_CUSTOMER_REASON"></param>
        /// <param name="c_SECOM_REASON"></param>
        /// <param name="contractProjectCode"></param>
        /// <param name="maintenanceNo"></param>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<dtInstallationHistoryForView> GetTbt_InstallationHistoryForView(string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_SALE_INSTALL_TYPE, string c_RENTAL_INSTALL_TYPE, string c_CHANGE_REASON_TYPE_CUSTOMER, string c_CHANGE_REASON_TYPE_SECOM, string c_CUSTOMER_REASON, string c_SECOM_REASON, string contractProjectCode, string maintenanceNo, string slipNo, string C_CURRENCY_LOCAL, string C_CURRENCY_US);
        /// <summary>
        /// get tbt_installation memo for view
        /// </summary>
        /// <param name="contractProjectCode"></param>
        /// <param name="maintenanceNo"></param>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<dtInstallationMemoForView> GetTbt_InstallationMemoForView(string contractProjectCode, string maintenanceNo, string slipNo);
        /// <summary>
        /// Get installation data for gen CSV file
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtInstallation> GetInstallationDataListForCsvFile(doSearchInstallationCondition cond);
        /// <summary>
        /// delete tbt_installtionAttachFile
        /// </summary>
        /// <param name="AttachFileID"></param>
        /// <param name="MaintenanceNo"></param>
        /// <param name="ObjectID"></param>
        /// <returns></returns>
        List<tbt_InstallationAttachFile> DeleteTbt_InstallationAttachFile(Nullable<int> AttachFileID, string MaintenanceNo, string ObjectID);
        /// <summary>
        /// Get data tbt_installationAttachFile
        /// </summary>
        /// <param name="AttachFileID"></param>
        /// <param name="MaintenanceNo"></param>
        /// <param name="ObjectID"></param>
        /// <returns></returns>
        List<tbt_InstallationAttachFile> GetTbt_InstallationAttachFile(Nullable<int> AttachFileID, string MaintenanceNo, string ObjectID);
        /// <summary>
        /// Get Installation data for complete installation
        /// </summary>
        /// <param name="strInstallationSlipNo"></param>
        /// <returns></returns>
        List<doInstallationDetailForCompleteInstallation> GetInstallationDetailForCompleteInstallation(string strInstallationSlipNo);
        /// <summary>
        /// Get data tbt_installationSlipDetails for view
        /// </summary>
        /// <param name="slipNo"></param>
        /// <param name="instrumentCode"></param>
        /// <returns></returns>
        List<dtInstallationSlipDetailsForView> GetTbt_InstallationSlipDetailsForView(string slipNo, string instrumentCode);
        //bool CheckAllIEComplete(string strContractCode); //No use
        /// <summary>
        /// Insert tbt_installationSlipExpansion
        /// </summary>
        /// <param name="doTbt_InstallationSlipExpansion"></param>
        /// <returns></returns>
        int InsertTbt_InstallationSlipExpansion(tbt_InstallationSlipExpansion doTbt_InstallationSlipExpansion);
        /// <summary>
        /// Complete Installation Rental
        /// </summary>
        /// <param name="RentalContractCode"></param>
        void Temp_CompleteInstallation_Rental(string RentalContractCode);
        /// <summary>
        /// Complete Installation Sale
        /// </summary>
        /// <param name="SaleContractCode"></param>
        void Temp_CompleteInstallation_Sale(string SaleContractCode);
        /// <summary>
        /// Get Rental Instrument data
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <param name="SlipNo"></param>
        /// <param name="InstrumentTypeCode"></param>
        /// <returns></returns>
        List<doRentalInstrumentdataList> GetRentalInstrumentdataList(string ContractCode, string OCC, string SlipNo, string InstrumentTypeCode, string RentalInstallType = null); //Add (RentalInstallType) by Jutarat A. on 17062013

        /// <summary>
        /// Get Sale Instrument data
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <param name="SlipNo"></param>
        /// <param name="InstrumentTypeCode"></param>
        /// <param name="ChangeType"></param>
        /// <returns></returns>
        List<doSaleInstrumentdataList> GetSaleInstrumentdataList(string ContractCode, string OCC, string SlipNo, string InstrumentTypeCode, string ChangeType = null, bool? InstallCompleteFlag = null, string SaleInstallType = null, string strSaleProcessManageStatus = null); //Add (ChangeType, InstallCompleteFlag, SaleInstallType and strSaleProcessManageStatus) by Jutarat A. on 27052013

        /// <summary>
        /// Get data Installation Basic Contract By Project
        /// </summary>
        /// <param name="ProjectCode"></param>
        /// <returns></returns>
        List<ContractCodeList> GetInstallationBasicContractByProject(string ProjectCode);
        /// <summary>
        /// Get Rental fee (Normal contract fee , Normal install fee)
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<doRentalFeeResult> GetRentalFee(string strContractCode);
        /// <summary>
        /// Get removal data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<doGetRemovalData> GetRemovalData(string strContractCode);
        /// <summary>
        /// Check Can Register CP12
        /// </summary>
        /// <param name="strContractCode"></param>
        void CheckCanRegisterCP12(string strContractCode);
        /// <summary>
        /// Delete data Tbt_InstallationEmail from referenceid
        /// </summary>
        /// <param name="ReferenceID"></param>
        /// <returns></returns>
        List<tbt_InstallationEmail> DeleteTbt_InstallationEmail(string ReferenceID);
        /// <summary>
        /// Get all installation slip no series.
        /// </summary>
        /// <param name="pContractProjectCode"></param>
        /// <param name="pOCC"></param>
        /// <param name="pLatestSlipNo"></param>
        /// <returns></returns>
        List<doAllSlipNoSeries> GetAllSlipNoSeries(string pContractProjectCode, string pOCC, string pLatestSlipNo);

        /// <summary>
        /// Get contract's installation slip no for update customer acceptance.
        /// </summary>
        /// <param name="pContractProjectCode"></param>
        /// <param name="pOCC"></param>
        /// <returns></returns>
        string GetInstallationSlipNoForAcceptant(string pContractProjectCode, string pOCC);

        /// <summary>
        /// Get SlipNo History
        /// </summary>
        /// <param name="SlipNo"></param>
        /// <returns></returns>
        List<dtInsHistory> GetSlipNoHistory(string SlipNo);

        /// <summary>
        /// Calculate and get booking data for installation slip.
        /// </summary>
        /// <param name="slipNo"></param>
        /// <returns></returns>
        List<InstallationBooking> GetInstallationBooking(string slipNo);

        /// <summary>
        /// Get Installation report.
        /// </summary>
        /// <param name="Subconstractor"></param>
        /// <param name="lastPaidDate"></param>
        /// <returns></returns>
        List<dtGetInstallationReport> GetInstallationReportExcelFile(doInstallationReport dtInstallationReport);

        /// <summary>
        /// Get Installation report Monthly.
        /// </summary>
        /// <param name="doInstallationReport"></param>
        /// <returns></returns>
        List<dtGetInstallationReportMonthly> GetInstallationReportMonthlyExcelFile(doInstallationReportMonthly dtInstallationReport);

        List<tbt_InstallationHistory> UpdateTbt_InstallationHistory(List<tbt_InstallationHistory> lstHistory);
    }
    
}
