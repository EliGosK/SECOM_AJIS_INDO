using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using System.Transactions;
using SECOM_AJIS.DataEntity.Billing;
using System.IO;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class MaintenanceHandler : BizCTDataEntities, IMaintenanceHandler
    {
        private string OWNER_PASSWORD = "P@$$w0rd";

        /// <summary>
        /// To generate maintenance check-up schedule
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strGenMAType"></param>
        public void GenerateMaintenanceSchedule(string strContractCode, string strMAProcessType, DateTime? createDate = null, string createBy = null, bool isWriteTransLog = true) //Modify by Jutarat A. on 09052013 (Add createDate, createBy, isWriteTransLog)
        {
            try
            {
                //1. Check Mandatory Data
                //strContractCode and intGenMAType are required fields
                doGenerateMACheckupSchedule doInput = new doGenerateMACheckupSchedule();
                doInput.ContractCode = strContractCode;
                doInput.MAProcessType = strMAProcessType;
                ApplicationErrorException.CheckMandatoryField(doInput);

                //2. Get product type from rental contract table
                IRentralContractHandler rentalHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<tbt_RentalContractBasic> doTbt_RentalContractBasic = rentalHand.GetTbt_RentalContractBasic(strContractCode, null);

                //3. Validate business
                //3.1 Contract code must exist
                if (doTbt_RentalContractBasic.Count <= 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, "ContractCode");
                }
                tbt_RentalContractBasic doRentalContractBasic = doTbt_RentalContractBasic[0];

                //3.2 Product type must be alarm or maintenance
                if (doRentalContractBasic.ProductTypeCode != ProductType.C_PROD_TYPE_AL 
                    && doRentalContractBasic.ProductTypeCode != ProductType.C_PROD_TYPE_MA
                    && doRentalContractBasic.ProductTypeCode != ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3161, "ProductTypeCode");
                }

                //4. Begin Trans
                using (TransactionScope scope = new TransactionScope())
                {
                    DateTime dtFirstDayOfNextMonth = GetFirstDayOfNextMonth(DateTime.Now);
                    DateTime dtFirstDayOfCurrentMonth = FirstDayOfMonthFromDateTime(DateTime.Now);

                    //5. In case re-generate schedule, delete check-up schedule
                    if (strMAProcessType == GenerateMAProcessType.C_GEN_MA_TYPE_RE_CREATE)
                    {
                        //this.DeleteMACheckupDetail(strContractCode, dtFirstDayOfNextMonth);
                        //this.DeleteMACheckup(strContractCode, dtFirstDayOfNextMonth);
                        this.DeleteMACheckupDetail(strContractCode, dtFirstDayOfNextMonth, null, isWriteTransLog); //Modify by Jutarat A. on 09052013
                        this.DeleteMACheckup(strContractCode, dtFirstDayOfNextMonth, null, isWriteTransLog); //Modify by Jutarat A. on 09052013
                    }

                    //6. Get maintenance data from rental security basic table
                    List<tbt_RentalSecurityBasic> doTbt_RentalSecurityBasic
                        = rentalHand.GetTbt_RentalSecurityBasic(strContractCode, doRentalContractBasic.LastOCC);
                    if (doTbt_RentalSecurityBasic.Count <= 0)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, "ContractCode");
                    }
                    tbt_RentalSecurityBasic doRentalSecurityBasic = doTbt_RentalSecurityBasic[0];

                    //7.	Create data object doCreateMASchedule[] and doCreateMAScheduleDetail[]
                    List<doCreateMASchedule> doCreateMAScheduleList = new List<doCreateMASchedule>();
                    List<doCreateMAScheduleDetail> doCreateMAScheduleDetailList = new List<doCreateMAScheduleDetail>();

                    //8. Get product code for generate schedule check-up header
                    bool bCreateMASchedule = false;
                    //8.1 In case of MA contract
                    if (doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                    {
                        //8.1.1 Get MA Data from rental maintenance details table
                        List<tbt_RentalMaintenanceDetails> dtTbt_RentalMaintenanceDetails
                            = rentalHand.GetTbt_RentalMaintenanceDetails(strContractCode, doRentalContractBasic.LastOCC);
                        if (dtTbt_RentalMaintenanceDetails.Count <= 0 || dtTbt_RentalMaintenanceDetails == null)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, "ContractCode");
                        }

                        //8.1.2 In case of SECOM's product
                        if (dtTbt_RentalMaintenanceDetails[0].MaintenanceTargetProductTypeCode == MaintenanceTargetProductType.C_MA_TARGET_PROD_TYPE_SECOM)
                        {
                            //8.1.2.1.	Create MA schedule by getting data group by MA code and product code
                            doCreateMAScheduleList = this.GetMAforCreateScheduleByMA(strContractCode);

                            //8.1.2.2.	Create MA schedule detail by getting data group by MA code, product code, contract code and OCC
                            doCreateMAScheduleDetailList = this.GetMAforCreateScheduleDetailByMA(strContractCode);
                        }
                        //8.1.3. In case of Customer’s product and other
                        else
                        {
                            bCreateMASchedule = true;
                        }
                    }
                    //9.	In case of alarm contract
                    else if (doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_AL || doRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {
                        bCreateMASchedule = true;
                    }

                    //Set data object
                    if (bCreateMASchedule)
                    {
                        doCreateMASchedule doMASchedule = new doCreateMASchedule();
                        doMASchedule.ContractCode = strContractCode;
                        doMASchedule.ProductCode = doRentalSecurityBasic.ProductCode;
                        doCreateMAScheduleList.Add(doMASchedule);

                        doCreateMAScheduleDetail doMAScheduleDetail = new doCreateMAScheduleDetail();
                        doMAScheduleDetail.ContractCode = strContractCode;
                        doMAScheduleDetail.ProductCode = doRentalSecurityBasic.ProductCode;
                        doMAScheduleDetail.MATargetContractCode = strContractCode;
                        doMAScheduleDetail.OCC = doRentalContractBasic.LastOCC;
                        doCreateMAScheduleDetailList.Add(doMAScheduleDetail);
                    }

                    //10. Create maintenance check-up schedule
                    //10.1	 Prepare variable 
                    //    Set dtStartCycle = First date of doTbt_RentalSecurityBasic.ContractStartDate 	//Ex. 01-08-2011
                    //    Set dtEndCycle = Last date of doTbt_RentalSecurityBasic.CalContractEndDate 	//Ex. 31-08-2011
                    //DateTime dtStartCycle = FirstDayOfMonthFromDateTime(doRentalSecurityBasic.ContractStartDate.Value);
                    //DateTime dtEndCycle = LastDayOfMonthFromDateTime(doRentalSecurityBasic.CalContractEndDate.Value);
                    DateTime dtStartCycle = DateTime.MinValue;
                    if (doRentalSecurityBasic.ContractStartDate != null)
                        dtStartCycle = FirstDayOfMonthFromDateTime(doRentalSecurityBasic.ContractStartDate.Value);

                    DateTime dtEndCycle = DateTime.MinValue;
                    if (doRentalSecurityBasic.CalContractEndDate != null)
                        dtEndCycle = LastDayOfMonthFromDateTime(doRentalSecurityBasic.CalContractEndDate.Value);

                    //	Set next cycle month to dtStartCycle
                    if (doRentalSecurityBasic.MaintenanceCycle != null)
                        dtStartCycle = dtStartCycle.AddMonths(doRentalSecurityBasic.MaintenanceCycle.Value-1);

                    //10.2	  Loop contract duration from next month until end contract date
                    while (dtStartCycle <= dtEndCycle)
                    {
                        //10.2.1.	If dtStartCycle > First date of next month Then
                        if ((strMAProcessType == GenerateMAProcessType.C_GEN_MA_TYPE_RE_CREATE && dtStartCycle >= dtFirstDayOfNextMonth) || 
                             (strMAProcessType == GenerateMAProcessType.C_GEN_MA_TYPE_CREATE && dtStartCycle >= dtFirstDayOfCurrentMonth))
                        {
                            //10.2.2.		Loop for each product code in doCreateMASchedule.ProductCode[]
                            //10.2.2.1.		Insert maintenance schedule to maintenance check-up table
                            foreach (doCreateMASchedule doMASch in doCreateMAScheduleList)
                            {
                                tbt_MaintenanceCheckup mainChk = new tbt_MaintenanceCheckup();
                                mainChk.ContractCode = doMASch.ContractCode;
                                mainChk.ProductCode = doMASch.ProductCode;
                                mainChk.InstructionDate = dtStartCycle;
                                mainChk.DeleteFlag = FlagType.C_FLAG_OFF;

                                //this.InsertTbt_MaintenanceCheckup(mainChk);
                                this.InsertTbt_MaintenanceCheckup(mainChk, createDate, createBy, isWriteTransLog); //Modify by Jutarat A. on 09052013

                                //10.2.2.2.		Loop Insert maintenance schedule detail to maintenance check-up details table
                                foreach (doCreateMAScheduleDetail doMASchDetail in doCreateMAScheduleDetailList)
                                {
                                    if (doMASch.ProductCode == doMASchDetail.ProductCode)
                                    {
                                        tbt_MaintenanceCheckupDetails mainChkDetail = new tbt_MaintenanceCheckupDetails();
                                        mainChkDetail.ContractCode = doMASchDetail.ContractCode;
                                        mainChkDetail.ProductCode = doMASchDetail.ProductCode;
                                        mainChkDetail.MATargetContractCode = doMASchDetail.MATargetContractCode;
                                        mainChkDetail.MATargetOCC = doMASchDetail.OCC;
                                        mainChkDetail.InstructionDate = dtStartCycle;

                                        //this.InsertTbt_MaintenanceCheckupDetails(mainChkDetail);
                                        this.InsertTbt_MaintenanceCheckupDetails(mainChkDetail, createDate, createBy, isWriteTransLog); //Modify by Jutarat A. on 09052013
                                    }
                                }
                            }
                        }

                        //10.2.3.	Set next cycle month to dtStartCycle
                        if (doRentalSecurityBasic.MaintenanceCycle != null)
                            dtStartCycle = dtStartCycle.AddMonths(doRentalSecurityBasic.MaintenanceCycle.Value);
                    }

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To delete check-up report
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtMaintenanceDate"></param>
        /// <returns></returns>
        public List<tbt_MaintenanceCheckup> DeleteMACheckup(string strContractCode, DateTime? dtMaintenanceDate = null, bool? deleteFlag = null, bool isWriteTransLog = true) //Modify by Jutarat A. on 09052013 (Add deleteFlag and isWriteTransLog)
        {
            try
            {
                //Delete data from DB
                List<tbt_MaintenanceCheckup> deletedList = base.DeleteMACheckup(strContractCode, dtMaintenanceDate, deleteFlag); //Modify by Jutarat A. on 09052013 (Add deleteFlag)

                if (isWriteTransLog == true) //Add by Jutarat A. on 09052013
                {
                    //Insert Log
                    if (deletedList.Count > 0)
                    {
                        doTransactionLog logData = new doTransactionLog();
                        logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                        logData.TableName = TableName.C_TBL_NAME_MAIN_CHKUP;
                        logData.TableData = CommonUtil.ConvertToXml(deletedList);
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To delete check-up detail report
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtMaintenanceDate"></param>
        /// <returns></returns>
        public List<tbt_MaintenanceCheckupDetails> DeleteMACheckupDetail(string strContractCode, DateTime? dtMaintenanceDate = null, bool? deleteFlag = null, bool isWriteTransLog = true) //Modify by Jutarat A. on 09052013 (Add deleteFlag and isWriteTransLog)
        {
            try
            {
                //Delete data from DB
                List<tbt_MaintenanceCheckupDetails> deletedList = base.DeleteMACheckupDetail(strContractCode, dtMaintenanceDate, deleteFlag); //Modify by Jutarat A. on 09052013 (Add deleteFlag)

                if (isWriteTransLog == true) //Add by Jutarat A. on 09052013
                {
                    //Insert Log
                    if (deletedList.Count > 0)
                    {
                        doTransactionLog logData = new doTransactionLog();
                        logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                        logData.TableName = TableName.C_TBL_NAME_MAIN_CHKUP_DET;
                        logData.TableData = CommonUtil.ConvertToXml(deletedList);
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get MA for create scheduak by MA
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public List<doCreateMASchedule> GetMAforCreateScheduleByMA(string strContractCode)
        {
            return base.GetMAforCreateScheduleByMA(strContractCode, RelationType.C_RELATION_TYPE_MA, FlagType.C_FLAG_ON);
        }

        /// <summary>
        /// Get product code, contract code and OCC with MA contract code using for create maintenance check-up schedule detail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public List<doCreateMAScheduleDetail> GetMAforCreateScheduleDetailByMA(string strContractCode)
        {
            return base.GetMAforCreateScheduleDetailByMA(strContractCode, RelationType.C_RELATION_TYPE_MA, FlagType.C_FLAG_ON);
        }

        /// <summary>
        /// Insert maintenance checkup
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_MaintenanceCheckup> InsertTbt_MaintenanceCheckup(tbt_MaintenanceCheckup doInsert, DateTime? createDate = null, string createBy = null, bool isWriteTransLog = true) //Modify by Jutarat A. on 09052013 (Add createDate, createBy, isWriteTransLog)
        {
            try
            {
                //Modify by Jutarat A. on 09052013
                if (createDate == null && createBy == null)
                {
                    createDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    createBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }

                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = createDate; //CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = createBy; //CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = createDate; //CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = createBy; //CommonUtil.dsTransData.dtUserData.EmpNo;
                //End Modify

                List<tbt_MaintenanceCheckup> doInsertList = new List<tbt_MaintenanceCheckup>();
                doInsertList.Add(doInsert);
                List<tbt_MaintenanceCheckup> insertList = base.InsertTbt_MaintenanceCheckup(CommonUtil.ConvertToXml_Store<tbt_MaintenanceCheckup>(doInsertList));

                if (isWriteTransLog == true) //Modify by Jutarat A. on 09052013
                {
                    //Insert Log
                    if (insertList.Count > 0)
                    {
                        doTransactionLog logData = new doTransactionLog();
                        logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                        logData.TableName = TableName.C_TBL_NAME_MAIN_CHKUP;
                        logData.TableData = CommonUtil.ConvertToXml(insertList);
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert maintenance checkuo detail
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_MaintenanceCheckupDetails> InsertTbt_MaintenanceCheckupDetails(tbt_MaintenanceCheckupDetails doInsert, DateTime? createDate = null, string createBy = null, bool isWriteTransLog = true) //Modify by Jutarat A. on 09052013 (Add createDate, createBy, isWriteTransLog)
        {
            try
            {
                //Modify by Jutarat A. on 09052013
                if (createDate == null && createBy == null)
                {
                    createDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    createBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }

                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = createDate; //CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = createBy; //CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = createDate; //CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = createBy; //CommonUtil.dsTransData.dtUserData.EmpNo;
                //End Modify

                List<tbt_MaintenanceCheckupDetails> doInsertList = new List<tbt_MaintenanceCheckupDetails>();
                doInsertList.Add(doInsert);
                List<tbt_MaintenanceCheckupDetails> insertList = base.InsertTbt_MaintenanceCheckupDetails(CommonUtil.ConvertToXml_Store<tbt_MaintenanceCheckupDetails>(doInsertList));

                if (isWriteTransLog == true) //Modify by Jutarat A. on 09052013
                {
                    //Insert Log
                    if (insertList.Count > 0)
                    {
                        doTransactionLog logData = new doTransactionLog();
                        logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                        logData.TableName = TableName.C_TBL_NAME_MAIN_CHKUP_DET;
                        logData.TableData = CommonUtil.ConvertToXml(insertList);
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update maintenance checkup
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        public List<tbt_MaintenanceCheckup> UpdateTbt_MaintenanceCheckup(tbt_MaintenanceCheckup doUpdate)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbt_MaintenanceCheckup> rList = this.GetTbt_MaintenanceCheckup(doUpdate.ContractCode, doUpdate.ProductCode, doUpdate.InstructionDate);
                if (DateTime.Compare(rList[0].UpdateDate.Value, doUpdate.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                //set updateDate and updateBy
                doUpdate.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doUpdate.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_MaintenanceCheckup> doUpdateList = new List<tbt_MaintenanceCheckup>();
                doUpdateList.Add(doUpdate);
                List<tbt_MaintenanceCheckup> updatedList = base.UpdateTbt_MaintenanceCheckup(CommonUtil.ConvertToXml_Store<tbt_MaintenanceCheckup>(doUpdateList));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_MAIN_CHKUP;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update maintenance checkup detail
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        public List<tbt_MaintenanceCheckupDetails> UpdateTbt_MaintenanceCheckupDetails(tbt_MaintenanceCheckupDetails doUpdate)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbt_MaintenanceCheckupDetails> rList
                    = this.GetTbt_MaintenanceCheckupDetails(doUpdate.ContractCode, doUpdate.ProductCode, doUpdate.InstructionDate, doUpdate.MATargetContractCode, doUpdate.MATargetOCC);
                if (DateTime.Compare(rList[0].UpdateDate.Value, doUpdate.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                //set updateDate and updateBy
                doUpdate.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doUpdate.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_MaintenanceCheckupDetails> doUpdateList = new List<tbt_MaintenanceCheckupDetails>();
                doUpdateList.Add(doUpdate);
                List<tbt_MaintenanceCheckupDetails> updatedList = base.UpdateTbt_MaintenanceCheckupDetails(CommonUtil.ConvertToXml_Store<tbt_MaintenanceCheckupDetails>(doUpdateList));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_MAIN_CHKUP_DET;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the first day of next input month
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public DateTime GetFirstDayOfNextMonth(DateTime datetime)
        {
            if (datetime.Month == 12) // its end of year , we need to add another year to new date:
            {
                datetime = new DateTime((datetime.Year + 1), 1, 1);
            }
            else
            {
                datetime = new DateTime(datetime.Year, (datetime.Month + 1), 1);
            }
            return datetime;
        }

        /// <summary>
        /// Get the first day of month from datetime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private DateTime FirstDayOfMonthFromDateTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// Get the last day of month from datetime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private DateTime LastDayOfMonthFromDateTime(DateTime dateTime)
        {
            DateTime firstDayOfTheMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// To register the maintenance checkup result from CTS280
        /// </summary>
        /// <param name="dtTbt_MaintenanceCheckup"></param>
        public void RegisterMaintenanceCheckupResult(tbt_MaintenanceCheckup dtTbt_MaintenanceCheckup)
        {
            try
            {
                IRentralContractHandler renHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                //1.	Check the last maintenance result record
                bool isLastResultToRegister
                    = this.IsLastResultToRegisterData(
                        dtTbt_MaintenanceCheckup.ContractCode, 
                        dtTbt_MaintenanceCheckup.ProductCode, 
                        dtTbt_MaintenanceCheckup.InstructionDate);

                
                using (TransactionScope scope = new TransactionScope())
                {
                    //2.	Save the maintenance checkup result
                    this.UpdateTbt_MaintenanceCheckup(dtTbt_MaintenanceCheckup);

                    //3.	Check whether all result is registered or not
                    bool isAllResultRegistered
                        = this.IsAllResultRegisteredData(dtTbt_MaintenanceCheckup.ContractCode, dtTbt_MaintenanceCheckup.InstructionDate);

                    bool isMAFeeTypeRB = false;
                    dsRentalContractData contractData = renHand.GetEntireContract(dtTbt_MaintenanceCheckup.ContractCode, null);
                    if (contractData != null)
                    {
                        if (contractData.dtTbt_RentalMaintenanceDetails != null)
                        {
                            if (contractData.dtTbt_RentalMaintenanceDetails.Count > 0)
                            {
                                if (contractData.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
                                    isMAFeeTypeRB = true;
                            }
                        }
                    }
                    if (isMAFeeTypeRB == true)
                    {
                        //4.	Update to all of maintenance checkup result in the same maintenance contract
                        //If isAllResultRegistered == TRUE Then
                        if (isAllResultRegistered == true)
                        {
                            //4.1	Get dtTbt_MaintenanceCheckup[] in the same contract code and instruction date
                            List<tbt_MaintenanceCheckup> maintList = 
                                this.GetTbt_MaintenanceCheckup(dtTbt_MaintenanceCheckup.ContractCode, null, dtTbt_MaintenanceCheckup.InstructionDate);

                            //4.2	Set data MaintenanceFee & ApproveNo1
                            foreach (tbt_MaintenanceCheckup tbtmaint in maintList)
                            {
                                tbtmaint.MaintenanceFee = dtTbt_MaintenanceCheckup.MaintenanceFee;
                                tbtmaint.ApproveNo1 = dtTbt_MaintenanceCheckup.ApproveNo1;

                                //4.3	Update back to database
                                this.UpdateTbt_MaintenanceCheckup(tbtmaint);
                            }

                            
                        }
                    }

                    if (isLastResultToRegister == true
                                    && dtTbt_MaintenanceCheckup.MaintenanceFee.HasValue
                                    && dtTbt_MaintenanceCheckup.MaintenanceFee.Value > 0)
                    {
                     
                            //5.1	Get billing occ for payment
                            tbt_BillingBasic resultBase = renHand.GetBillingBasicForMAResultBasedFeePayment(dtTbt_MaintenanceCheckup.ContractCode);
                            if (resultBase != null)
                            {
                                //5.2	Send
                                //Call BillingInterfaceHandler. SendBilling_MAResultBase()
                                IBillingInterfaceHandler billHand = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                                billHand.SendBilling_MAResultBase(dtTbt_MaintenanceCheckup.ContractCode, resultBase.BillingOCC, dtTbt_MaintenanceCheckup.MaintenanceFee.Value);
                            }
                        
                    }

                    scope.Complete();
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// To check whether the maintenance result is the last result for registering or not
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pProductCode"></param>
        /// <param name="pInstructionDate"></param>
        /// <returns></returns>
        public bool IsLastResultToRegisterData(string pContractCode, string pProductCode, DateTime? pInstructionDate)
        {
            bool isLastResult = false;
            try
            {
                List<int?> resultList = base.IsLastResultToRegister(pContractCode, pProductCode, pInstructionDate);
                if (resultList.Count > 0)
                    isLastResult = true;
            }
            catch (Exception)
            {
                throw;
            }
            return isLastResult;
        }

        /// <summary>
        /// To check the all of maintenance checkup records are registered or not
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pInstructionDate"></param>
        /// <returns></returns>
        public bool IsAllResultRegisteredData(string pContractCode, DateTime pInstructionDate)
        {
            bool isAllResultRegistered = false;
            try
            {
                List<int?> resultList = base.IsAllResultRegistered(pContractCode, pInstructionDate);
                if (resultList.Count > 0 && resultList[0] == 0)
                    isAllResultRegistered = true;
            }
            catch (Exception)
            {
                throw;
            }
            return isAllResultRegistered;
        }

        /// <summary>
        /// To check the result of maintenance checkup records which has registered at lease one record or not
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pInstructionDate"></param>
        /// <returns></returns>
        public bool IsSomeResultRegistered(string pContractCode, DateTime pInstructionDate)
        {
            bool isSomeResultRegistered = false;
            try
            {
                List<int?> resultList = base.IsSomeResultRegistered(pContractCode, pInstructionDate);
                if (resultList.Count > 0 && resultList[0] > 0)
                    isSomeResultRegistered = true;
            }
            catch (Exception)
            {
                throw;
            }
            return isSomeResultRegistered;
        }

        /// <summary>
        /// To get the basic information of maintenance check-up
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pProductCode"></param>
        /// <param name="pInstructionDate"></param>
        /// <returns></returns>
        public List<doMaintenanceCheckupInformation> GetMaintenanceCheckupInformationData(string pContractCode, string pProductCode, DateTime? pInstructionDate)
        {
            try
            {
                List<doMaintenanceCheckupInformation> lst
                    = base.GetMaintenanceCheckupInformation(pContractCode, pProductCode, pInstructionDate);
                if (lst == null)
                    lst = new List<doMaintenanceCheckupInformation>();
                else
                    CommonUtil.MappingObjectLanguage<doMaintenanceCheckupInformation>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To search maintenance check-up for CTS270
        /// </summary>
        /// <param name="doSearch"></param>
        /// <returns></returns>
        public List<dtSearchMACheckupResult> SearchMACheckup(doSearchMACheckupCriteria doSearch)
        {
            try
            {
                //1.	Prepare criteria before searching
                //1.1	Prepare check-up instruction criteria
                doSearch.InstructionDateFrom = CommonUtil.FirstDayOfMonthFromDateTime(doSearch.CheckupInstructionMonthFrom, doSearch.CheckupInstructionYearFrom);
                doSearch.InstructionDateTo = CommonUtil.LastDayOfMonthFromDateTime(doSearch.CheckupInstructionMonthTo, doSearch.CheckupInstructionYearTo);

                //5.	Define SQL for perform search operation
                //5.1	SearchAlarmPeriodMaintenance
                if (doSearch.RelatedContractType == RelatedContractType.C_RELATED_CONTRACT_TYPE_ALARM_PERIOD)
                {
                    return this.SearchAlarmPeriodMaintenance(doSearch);
                }

                //5.2	SearchSaleMaintenance
                else if (doSearch.RelatedContractType == RelatedContractType.C_RELATED_CONTRACT_TYPE_SALE)
                {
                    return this.SearchSaleMaintenance(doSearch);
                }

                //5.3	SearchSeparatedMaintenance
                else if (doSearch.RelatedContractType == RelatedContractType.C_RELATED_CONTRACT_TYPE_SEPARATED_MA)
                {
                    return this.SearchSeparatedMaintenance(doSearch);
                }

                //5.4	//SearchAlarmPeriodMaintenance UNION ALL SearchSaleMaintenance UNION ALL SearchSeparatedMaintenance
                else if (doSearch.RelatedContractType == RelatedContractType.C_RELATED_CONTRACT_TYPE_ANY)
                {
                    var SearchMACheckupList
                        = this.SearchAlarmPeriodMaintenance(doSearch)
                            .Union(this.SearchSeparatedMaintenance(doSearch))
                            .Union(this.SearchSaleMaintenance(doSearch));

                    SearchMACheckupList = ( from ma in SearchMACheckupList
                                                orderby ma.InstructionDate, ma.ContractCode, ma.ProductCode
                                                select ma); 

                    return SearchMACheckupList.ToList<dtSearchMACheckupResult>();
                }
                else
                {
                    return new List<dtSearchMACheckupResult>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<dtSearchMACheckupResult> SearchAlarmPeriodMaintenance(doSearchMACheckupCriteria doSearch)
        {
            try
            {
                List<dtSearchMACheckupResult> lst
                    = base.SearchAlarmPeriodMaintenance(
                       doSearch.ProductName,
                       doSearch.SiteName,
                       doSearch.MAEmployeeName,
                       ProductType.C_PROD_TYPE_AL,
                       ProductType.C_PROD_TYPE_RENTAL_SALE,
                       doSearch.OperationOffice,
                       doSearch.InstructionDateFrom,
                       doSearch.InstructionDateTo,
                       doSearch.UserCode,
                       doSearch.ContractCode,
                       doSearch.MACheckupNo,
                       doSearch.HasCheckupResult,
                       doSearch.HaveInstrumentMalfunction,
                       doSearch.NeedToContactSalesman);
                if (lst == null)
                    lst = new List<dtSearchMACheckupResult>();
                else
                    CommonUtil.MappingObjectLanguage<dtSearchMACheckupResult>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<dtSearchMACheckupResult> SearchSeparatedMaintenance(doSearchMACheckupCriteria doSearch)
        {
            try
            {
                // Akat K. : change from 'SearchAlarmPeriodMaintenance' to 'SearchSeparatedMaintenance' due to change spec
                List<dtSearchMACheckupResult> lst
                    = base.SearchSeparatedMaintenance(
                       doSearch.ProductName,
                       doSearch.SiteName,
                       doSearch.MAEmployeeName,
                       ProductType.C_PROD_TYPE_MA,
                       doSearch.OperationOffice,
                       doSearch.InstructionDateFrom,
                       doSearch.InstructionDateTo,
                       doSearch.UserCode,
                       doSearch.ContractCode,
                       doSearch.MACheckupNo,
                       doSearch.HasCheckupResult,
                       doSearch.HaveInstrumentMalfunction,
                       doSearch.NeedToContactSalesman);
                if (lst == null)
                    lst = new List<dtSearchMACheckupResult>();
                else
                    CommonUtil.MappingObjectLanguage<dtSearchMACheckupResult>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<dtSearchMACheckupResult> SearchSaleMaintenance(doSearchMACheckupCriteria doSearch)
        {
            try
            {
                List<dtSearchMACheckupResult> lst
                    = base.SearchSaleMaintenance(
                       doSearch.ProductName,
                       doSearch.SiteName,
                       doSearch.MAEmployeeName,
                       doSearch.OperationOffice,
                       doSearch.InstructionDateFrom,
                       doSearch.InstructionDateTo,
                       doSearch.UserCode,
                       doSearch.ContractCode,
                       doSearch.MACheckupNo,
                       doSearch.HasCheckupResult,
                       doSearch.HaveInstrumentMalfunction,
                       doSearch.NeedToContactSalesman,
                       FlagType.C_FLAG_ON,
                       ProductType.C_PROD_TYPE_MA);
                if (lst == null)
                    lst = new List<dtSearchMACheckupResult>();
                else
                    CommonUtil.MappingObjectLanguage<dtSearchMACheckupResult>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To delete maintenance check-up schedule for CTS270
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="productCode"></param>
        /// <param name="instructionDate"></param>
        /// <param name="updateDate"></param>
        public void DeleteMaintenanceCheckupSchedule(string contractCode, string productCode, DateTime instructionDate, DateTime updateDate)
        {
            try
            {
                //1.	Get maintenance check-up schedule
                List<tbt_MaintenanceCheckup> doTbt_MaintenanceCheckup = GetTbt_MaintenanceCheckup(contractCode, productCode, instructionDate);

                //3.	Delete the record by mark the delete flag
                doTbt_MaintenanceCheckup[0].DeleteFlag = FlagType.C_FLAG_ON;
                doTbt_MaintenanceCheckup[0].UpdateDate = updateDate;

                //3.1	Update to database
                List<tbt_MaintenanceCheckup> updatedList
                    = this.UpdateTbt_MaintenanceCheckup(doTbt_MaintenanceCheckup[0]);

                //3.2	If cannot update Then
                if (updatedList.Count < 1)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0094, "maintenance checkup no.", doTbt_MaintenanceCheckup[0].CheckupNo.ToString().PadLeft(7, '0'));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public int? ToNullableInt32(this string s)
        //{
        //    int i;
        //    if (Int32.TryParse(s, out i)) return i;
        //    return null;
        //}

        //public byte[] GenerateMACheckupSlip(List<dtSearchMACheckupResult> MAlist)
        public doMACheckupSlipResult GenerateMACheckupSlip(List<tbt_MaintenanceCheckup> MAlist)
        {
            doMACheckupSlipResult result = new doMACheckupSlipResult();
            
            try
            {
                List<string> lstFilePath = new List<string>();


                //1.	Sort all fields in MACheckupKey[] by ascending
                var sortedMAResult =
                    from MAResult in MAlist
                    orderby MAResult.InstructionDate, MAResult.ContractCode, MAResult.ProductCode
                    select MAResult;

                //2.	For each item in MACheckupKey
                //List<byte[]> filesByte = new List<byte[]>();
                foreach (var item in sortedMAResult)
                {
                    try
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            //2.1	Generate check-up no.
                            string iCheckupNo = GenerateMACheckupNo(item.ContractCode, item.ProductCode, item.InstructionDate);

                            if (iCheckupNo != null)
                            {
                                //2.2	Get maintenance check-up slip report
                                IReportHandler rptHand = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                                //byte[] report = rptHand.GetMaintenanceCheckupSlip(item.ContractCode, item.ProductCode, item.InstructionDate, OWNER_PASSWORD);
                                //filesByte.Add(report);
                                //string strFilePath = rptHand.GetMaintenanceCheckupSlip(item.ContractCode, item.ProductCode, item.InstructionDate, OWNER_PASSWORD);
                                string strFilePath = rptHand.GetMaintenanceCompletionReport(item.ContractCode, item.ProductCode, item.InstructionDate, OWNER_PASSWORD);
                                lstFilePath.Add(strFilePath);

                                item.CheckupNo = iCheckupNo;
                            }
                            scope.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Error = ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3269);

                        if (ex.InnerException != null)
                            result.ErrorDetail = ex.InnerException.Message;
                        else
                            result.ErrorDetail = ex.Message;

                        break;
                    }

                    //result.Error = ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3269);
                    //result.ErrorDetail = "xxxxxxxx";
                    //break;
                }

                //2.3	Merge the pdf file from slipReportFilePath to combinedReport
                //merge = PdfMerger.MergeFiles(filesByte, OWNER_PASSWORD);
                if (lstFilePath.Count > 0)
                {
                    string mergeOutputFilename = PathUtil.GetTempFileName(".pdf");
                    string encryptOutputFileName = PathUtil.GetTempFileName(".pdf");

                    //for (int i = 0; i < 1000; i++)
                    //{
                    //    lstFilePath.Add(lstFilePath[0]);
                    //}

                    bool isSuccess = ReportUtil.MergePDF(lstFilePath.ToArray(), mergeOutputFilename, true, encryptOutputFileName, null);
                    if (isSuccess)
                    {
                        FileStream streamFile = new FileStream(encryptOutputFileName, FileMode.Open, FileAccess.Read);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            streamFile.CopyTo(ms);
                            result.ResultData = ms.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error = ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3269);
                result.ErrorDetail = ex.Message;
            }

            //3.	Return combinedReport
            return result;
        }

        //No use
        //public string GenerateMACheckupList(List<dtSearchMACheckupResult> MAlist)
        //{
        //    try
        //    {
        //        IReportHandler rptHand = ServiceContainer.GetService<IReportHandler>() as IReportHandler;

        //        List<Object[]> list = new List<object[]>();
        //        foreach (dtSearchMACheckupResult dt in MAlist)
        //        {
        //            Object[] obj = new Object[3];
        //            obj[0] = dt.ContractCode;
        //            obj[1] = dt.ProductCode;
        //            obj[2] = dt.InstructionDate;

        //            list.Add(obj);
        //        }

        //        string csv = rptHand.GetMaintenanceCheckupList(list);

        //        return csv;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public string GenerateMACheckupNo(string ContractCode, string ProductCode, DateTime InstructionDate)
        {
            string checkupNo = null;
            try
            {
                //1.	Get maintenance check-up data
                List<tbt_MaintenanceCheckup> dtTbt_MaintenanceCheckup
                    = this.GetTbt_MaintenanceCheckup(ContractCode, ProductCode, InstructionDate);

                String strYear = string.Empty;
                String strRunningNo = string.Empty;

                //2.	Get last maintenance check-up no.
                List<string> lst = base.GetLastMACheckupNo(); //(ContractCode, ProductCode);

                if (CommonUtil.IsNullOrEmpty(lst[0]))
                {
                    strYear = DateTime.Now.ToString("yy");
                    strRunningNo = "1".PadLeft(6, '0');
                }
                else
                {
                    String lastCheckupNo = lst[0];
                    
                    strYear = lastCheckupNo.Substring(0, 2);
                    if (strYear == DateTime.Now.ToString("yy"))
                    {
                        int iRunningNo = Int32.Parse(lastCheckupNo.Substring(2, 6)) + 1;
                        strRunningNo = iRunningNo.ToString().PadLeft(6, '0');
                    }
                    else
                    {
                        strYear = DateTime.Now.ToString("yy");
                        strRunningNo = "1".PadLeft(6, '0');
                    }
                }

                //3.	Generate new maintenance check-up no.
                //o	YYRRRR
                //	YY = Year  (2 digits)
                //	RRRRRR = Running number (6 digits), separated by year
                //int iLastCheckupNo = Int32.Parse(strYear + strRunningNo);

                //4.	Update database
                //dtTbt_MaintenanceCheckup[0].CheckupNo = iLastCheckupNo;
                dtTbt_MaintenanceCheckup[0].CheckupNo = strYear + strRunningNo;

                this.UpdateTbt_MaintenanceCheckup(dtTbt_MaintenanceCheckup[0]);

                //5.	Return dtTbt_MaintenanceCheckup.CheckupNo
                checkupNo = dtTbt_MaintenanceCheckup[0].CheckupNo;
            }
            catch (Exception)
            {
                throw;
            }

            return checkupNo;
        }

        public string GetMaintenanceCheckupSlipReport(string contractCode, string productCode, DateTime instructionDate)
        {
            IMaintenanceHandler maintenanceHandler;
            List<tbt_MaintenanceCheckupDetails> listMaintenanceCheckupDetails;
            //string[] InstrumentContract;

            try
            {
                maintenanceHandler = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                listMaintenanceCheckupDetails = this.GetTbt_MaintenanceCheckupDetails(contractCode, productCode, instructionDate, null, null);


            }
            catch (Exception ex)
            {
                throw;
            }

            return "";
        }

        /// <summary>
        /// To search sale warranty expire list
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<dtSearchSaleWarrantyExpireResult> SearchSaleWarrantyExpireList(doSearchSaleWarrantyExpireCondition cond)
        {
            try
            {
                DateTime dtExpireWarrantyFrom = new DateTime(cond.ExpireWarrantyYearFrom, cond.ExpireWarrantyMonthFrom, 1);
                DateTime dtExpireWarrantyTo = new DateTime(cond.ExpireWarrantyYearTo, cond.ExpireWarrantyMonthTo, 1);
                dtExpireWarrantyTo = dtExpireWarrantyTo.AddMonths(1).AddDays(-1);

                List<dtSearchSaleWarrantyExpireResult> dtSearchResultList = this.SearchSaleWarrantyExpireList(true, dtExpireWarrantyFrom, dtExpireWarrantyTo, cond.OperationOfficeCode, cond.SaleContractOfficeCode
                                                                                                                , ContractStatus.C_CONTRACT_STATUS_BEF_START, ContractStatus.C_CONTRACT_STATUS_AFTER_START
                                                                                                                ,RelationType.C_RELATION_TYPE_MA, SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE, SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE);

                if (dtSearchResultList == null)
                {
                    dtSearchResultList = new List<dtSearchSaleWarrantyExpireResult>();
                }
                else
                {
                    CommonUtil.MappingObjectLanguage<dtSearchSaleWarrantyExpireResult>(dtSearchResultList);
                }

                return dtSearchResultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the maintenance contract code of specified maintenance target contract code
        /// </summary>
        /// <param name="strMATargetContractCode"></param>
        /// <returns></returns>
        public List<string> GetMAContractCodeOf(string strMATargetContractCode)
        {
            try
            {
                //Check mandatory data
                if (String.IsNullOrEmpty(strMATargetContractCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "Maintenance target contract code" });
                }

                List<string> contractCodeList = this.GetMAContractCodeOf(strMATargetContractCode, RelationType.C_RELATION_TYPE_MA);
                if (contractCodeList == null || contractCodeList.Count == 0)
                {
                    contractCodeList = null;
                }

                return contractCodeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To update start maintenance date and end maintenance date in sale contract
        /// </summary>
        /// <param name="paramRegisterDate"></param>
        /// <param name="paramMAEntireContract"></param>
        public void UpdateMADateInSaleContract(DateTime paramRegisterDate, dsRentalContractData paramMAEntireContract)
        {
            ISaleContractHandler salecontracthandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

            //DateTime nextMADate = new DateTime(paramMAEntireContract.dtTbt_RentalSecurityBasic[0].ContractStartDate.GetValueOrDefault().Year
            //    , paramMAEntireContract.dtTbt_RentalSecurityBasic[0].ContractStartDate.GetValueOrDefault().Month
            //    , 1);

            //while (nextMADate < paramRegisterDate)
            //{
            //    nextMADate = nextMADate.AddMonths(paramMAEntireContract.dtTbt_RentalSecurityBasic[0].MaintenanceCycle.GetValueOrDefault());
            //}

            //DateTime startMADate = nextMADate;
            DateTime startMADate = paramMAEntireContract.dtTbt_RentalSecurityBasic[0].ContractStartDate.GetValueOrDefault();
            DateTime endMADate = paramMAEntireContract.dtTbt_RentalSecurityBasic[0].CalContractEndDate.GetValueOrDefault();

            if (paramMAEntireContract.dtTbt_RelationType != null)
            {
                foreach (var relationType in paramMAEntireContract.dtTbt_RelationType)
                {
                    bool isExists = false;
                    List<bool?> rLst = salecontracthandler.IsContractExist(relationType.RelatedContractCode);
                    if (rLst.Count > 0)
                        isExists = rLst[0].GetValueOrDefault();
                    if (isExists)
                    {
                        string currentOCC = relationType.RelatedOCC;

                        while (!String.IsNullOrEmpty(currentOCC))
                        {
                            var currSaleContract = salecontracthandler.GetTbt_SaleBasic(relationType.RelatedContractCode, currentOCC, null);
                            if (currSaleContract.Count == 1)
                            {
                                List<tbt_SaleBasic> updateRes = new List<tbt_SaleBasic>();

                                if (!currSaleContract[0].StartMaintenanceDate.HasValue && !currSaleContract[0].EndMaintenanceDate.HasValue)
                                {
                                    currSaleContract[0].StartMaintenanceDate = startMADate;
                                    currSaleContract[0].EndMaintenanceDate = endMADate;

                                    updateRes = salecontracthandler.UpdateTbt_SaleBasic(currSaleContract[0]);
                                }

                                //Add By Pachara S. 22032017
                                currSaleContract[0].MaintenanceContractFlag = FlagType.C_FLAG_ON;
                                updateRes = salecontracthandler.UpdateTbt_SaleBasic(currSaleContract[0]);
                            }

                            List<string> prevOCC = salecontracthandler.GetPreviousOCC(relationType.RelatedContractCode, currentOCC);
                            if (prevOCC.Count > 0)
                                currentOCC = prevOCC[0];
                            else
                                currentOCC = null;
                        }
                    }
                }
            }
        }
    }
}
