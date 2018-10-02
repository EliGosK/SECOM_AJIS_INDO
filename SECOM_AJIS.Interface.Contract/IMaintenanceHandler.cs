using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IMaintenanceHandler
    {
        /// <summary>
        /// To generate maintenance check-up schedule
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strGenMAType"></param>
        void GenerateMaintenanceSchedule(string strContractCode, string strGenMAType, DateTime? createDate = null, string createBy = null, bool isWriteTransLog = true); //Modify by Jutarat A. on 09052013 (Add createDate, createBy, isWriteTransLog)

        /// <summary>
        /// To get the basic information of maintenance check-up
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pProductCode"></param>
        /// <param name="pInstructionDate"></param>
        /// <returns></returns>
        List<doMaintenanceCheckupInformation> GetMaintenanceCheckupInformationData(string pContractCode, string pProductCode, DateTime? pInstructionDate);

        /// <summary>
        /// To register the maintenance checkup result from CTS280
        /// </summary>
        /// <param name="dtTbt_MaintenanceCheckup"></param>
        void RegisterMaintenanceCheckupResult(tbt_MaintenanceCheckup dtTbt_MaintenanceCheckup);

        /// <summary>
        /// To check whether the maintenance result is the last result for registering or not
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pProductCode"></param>
        /// <param name="pInstructionDate"></param>
        /// <returns></returns>
        bool IsLastResultToRegisterData(string pContractCode, string pProductCode, DateTime? pInstructionDate);

        /// <summary>
        /// To check the all of maintenance checkup records are registered or not
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pInstructionDate"></param>
        /// <returns></returns>
        bool IsAllResultRegisteredData(string pContractCode, DateTime pInstructionDate);

        /// <summary>
        /// To check the result of maintenance checkup records which has registered at lease one record or not
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pInstructionDate"></param>
        /// <returns></returns>
        bool IsSomeResultRegistered(string pContractCode, DateTime pInstructionDate);

        /// <summary>
        /// Insert maintenance checkup
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        List<tbt_MaintenanceCheckup> InsertTbt_MaintenanceCheckup(tbt_MaintenanceCheckup doInsert, DateTime? createDate = null, string createBy = null, bool isWriteTransLog = true); //Modify by Jutarat A. on 09052013 (Add createDate, createBy, isWriteTransLog)

        /// <summary>
        /// Insert maintenance checkuo detail
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        List<tbt_MaintenanceCheckupDetails> InsertTbt_MaintenanceCheckupDetails(tbt_MaintenanceCheckupDetails doInsert, DateTime? createDate = null, string createBy = null, bool isWriteTransLog = true); //Modify by Jutarat A. on 09052013 (Add createDate, createBy, isWriteTransLog)

        /// <summary>
        /// Update maintenance checkup
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        List<tbt_MaintenanceCheckup> UpdateTbt_MaintenanceCheckup(tbt_MaintenanceCheckup doUpdate);

        /// <summary>
        /// Update maintenance checkup detail
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        List<tbt_MaintenanceCheckupDetails> UpdateTbt_MaintenanceCheckupDetails(tbt_MaintenanceCheckupDetails doUpdate);
        List<tbt_MaintenanceCheckup> GetTbt_MaintenanceCheckup(string pContractCode, string pProductCode, DateTime? pInstructionDate);

        /// <summary>
        /// To search maintenance check-up for CTS270
        /// </summary>
        /// <param name="doSearch"></param>
        /// <returns></returns>
        List<dtSearchMACheckupResult> SearchMACheckup(doSearchMACheckupCriteria doSearch);

        /// <summary>
        /// To delete maintenance check-up schedule for CTS270
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="productCode"></param>
        /// <param name="instructionDate"></param>
        /// <param name="updateDate"></param>
        void DeleteMaintenanceCheckupSchedule(string contractCode, string productCode, DateTime instructionDate, DateTime updateDate);

        /// <summary>
        /// Get product code, contract code and OCC with MA contract code using for create maintenance check-up schedule detail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<doCreateMAScheduleDetail> GetMAforCreateScheduleDetailByMA(string strContractCode);

        //byte[] GenerateMACheckupSlip(List<dtSearchMACheckupResult> MAlist);

        /// <summary>
        /// To generate maintenance check-up slip report for CTS270
        /// </summary>
        /// <param name="MAlist"></param>
        /// <returns></returns>
        doMACheckupSlipResult GenerateMACheckupSlip(List<tbt_MaintenanceCheckup> MAlist);

        //string GenerateMACheckupList(List<dtSearchMACheckupResult> MAlist); //No use
        
        /// <summary>
        /// To delete check-up report
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtMaintenanceDate"></param>
        /// <returns></returns>
        List<tbt_MaintenanceCheckup> DeleteMACheckup(string strContractCode, DateTime? dtMaintenanceDate = null, bool? deleteFlag = null, bool isWriteTransLog = true); //Modify by Jutarat A. on 09052013 (Add deleteFlag and isWriteTransLog)

        /// <summary>
        /// To delete check-up detail report
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtMaintenanceDate"></param>
        /// <returns></returns>
        List<tbt_MaintenanceCheckupDetails> DeleteMACheckupDetail(string strContractCode, DateTime? dtMaintenanceDate = null, bool? deleteFlag = null, bool isWriteTransLog = true); //Modify by Jutarat A. on 09052013 (Add deleteFlag and isWriteTransLog)

        /// <summary>
        /// To search sale warranty expire list
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtSearchSaleWarrantyExpireResult> SearchSaleWarrantyExpireList(doSearchSaleWarrantyExpireCondition cond);

        /// <summary>
        /// Get the maintenance contract code of specified maintenance target contract code
        /// </summary>
        /// <param name="strMATargetContractCode"></param>
        /// <returns></returns>
        List<string> GetMAContractCodeOf(string strMATargetContractCode);

        /// <summary>
        /// To update start maintenance date and end maintenance date in sale contract
        /// </summary>
        /// <param name="paramRegisterDate"></param>
        /// <param name="paramMAEntireContract"></param>
        void UpdateMADateInSaleContract(DateTime paramRegisterDate, dsRentalContractData paramMAEntireContract);
    }
}
