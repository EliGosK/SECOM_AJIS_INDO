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
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract.Model;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class ProjectHandler : BizCTDataEntities, IProjectHandler
    {
        /// <summary>
        /// To generate project code
        /// </summary>
        /// <returns></returns>
        public string GenerateProjectCode()
        {
            try
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                
                //1.	Call method for get next running code
                List<doRunningNo> doRunningNo = null;
                try
                {
                    doRunningNo = hand.GetNextRunningCode(NameCode.C_NAME_CODE_PROJECT_CODE, true);
                }
                catch
                {
                }

                bool isFound = false;
                if (doRunningNo != null)
                {
                    if (doRunningNo.Count > 0)
                    {
                        if (CommonUtil.IsNullOrEmpty(doRunningNo[0].RunningNo) == false)
                            isFound = true;
                    }
                }
                if (isFound == false)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3131);
                }

                //2.	Call method for get the check digit
                string strCheckDigit = hand.GenerateCheckDigit(doRunningNo[0].RunningNo);

                //3.	Create project code
                string strProjectCode = ProjectCode.C_PROJECT_CODE_PREFIX + doRunningNo[0].RunningNo + strCheckDigit;

                //4.	Return strProjectCode
                return strProjectCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create project data
        /// </summary>
        /// <param name="RegProjectData"></param>
        /// <returns></returns>
        public bool CreateProjectData(doRegisterProjectData RegProjectData)
        {
            try
            {
                List<tbt_Project_CTS230> lstProject = new List<tbt_Project_CTS230>();
                lstProject.Add(RegProjectData.doTbt_Project);
                doTransactionLog log = new doTransactionLog();
                log.TransactionType = doTransactionLog.eTransactionType.Insert;

                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;

                IProjectHandler hand = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                List<tbt_Project> lsttbtProject = new List<tbt_Project>();
                List<tbt_Project_CTS230> tbtProject230 = new List<tbt_Project_CTS230>();
                tbtProject230.Add(RegProjectData.doTbt_Project);
                lsttbtProject = CommonUtil.ClonsObjectList<tbt_Project_CTS230, tbt_Project>(tbtProject230);

                InsertTbt_Project(lsttbtProject);
                if (!CommonUtil.IsNullOrEmpty(RegProjectData.doTbt_ProjectExpectedInstrumentDetail))
                    InsertTbt_ProjectExpectedInstrumentDetail(RegProjectData.doTbt_ProjectExpectedInstrumentDetail);
                if (!CommonUtil.IsNullOrEmpty(RegProjectData.doTbt_ProjectOtherRalatedCompany))
                    InsertTbt_ProjectOtherRalatedCompany(RegProjectData.doTbt_ProjectOtherRalatedCompany);
                if (!CommonUtil.IsNullOrEmpty(RegProjectData.doTbt_ProjectPurchaserCustomer))
                    InsertTbt_ProjectPurchaserCustomer(RegProjectData.doTbt_ProjectPurchaserCustomer);
                if (!CommonUtil.IsNullOrEmpty(RegProjectData.doTbt_ProjectSupportStaffDetails))
                    InsertTbt_ProjectSupportStaffDetail(RegProjectData.doTbt_ProjectSupportStaffDetails);
                if (!CommonUtil.IsNullOrEmpty(RegProjectData.doTbt_ProjectSystemDetails))
                    InsertTbt_ProjectSystemDetail(RegProjectData.doTbt_ProjectSystemDetails);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert project stockout instrument
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_ProjectStockoutInstrument> InsertTbt_ProjectStockoutInstrument(tbt_ProjectStockoutInstrument doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_ProjectStockoutInstrument> doInsertList = new List<tbt_ProjectStockoutInstrument>();
                doInsertList.Add(doInsert);
                List<tbt_ProjectStockoutInstrument> insertList = base.InsertTbt_ProjectStockoutInstrument(CommonUtil.ConvertToXml_Store<tbt_ProjectStockoutInstrument>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_PROJ_STOCKOUT_INST;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert project
        /// </summary>
        /// <param name="tbtProject"></param>
        /// <returns></returns>
        public int InsertTbt_Project(List<tbt_Project> tbtProject)
        {
            try
            {
                foreach (tbt_Project i in tbtProject)
                {
                    i.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    i.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    i.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    i.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                }
                string xmlProject = null;
                xmlProject = CommonUtil.ConvertToXml_Store<tbt_Project>(tbtProject);
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog log = new doTransactionLog();
                List<tbt_Project> lst = base.InsertTbt_Project(xmlProject);
                if (lst.Count > 0)
                {
                    log.TransactionType = doTransactionLog.eTransactionType.Insert;
                    log.TableName = TableName.C_TBL_NAME_PRJ;

                    log.TableData = CommonUtil.ConvertToXml<tbt_Project>(lst);
                    logHand.WriteTransactionLog(log);
                    return lst.Count;
                }
                else
                {
                    return -1;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert project expected instrument detail
        /// </summary>
        /// <param name="tbtProjectExpectIntrumentDetails"></param>
        /// <returns></returns>
        public int InsertTbt_ProjectExpectedInstrumentDetail(List<tbt_ProjectExpectedInstrumentDetails> tbtProjectExpectIntrumentDetails)
        {
            try
            {
                foreach (tbt_ProjectExpectedInstrumentDetails i in tbtProjectExpectIntrumentDetails)
                {
                    i.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    i.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    i.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    i.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                }
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog log = new doTransactionLog();
                List<tbt_ProjectExpectedInstrumentDetails> lst = base.InsertTbt_ProjectExpectedInstrumentDetail(CommonUtil.ConvertToXml_Store<tbt_ProjectExpectedInstrumentDetails>(tbtProjectExpectIntrumentDetails));
                if (lst.Count > 0)
                {
                    log.TransactionType = doTransactionLog.eTransactionType.Insert;
                    log.TableName = TableName.C_TBL_NAME_PRJ_EXP_INST;
                    log.TableData = CommonUtil.ConvertToXml<tbt_ProjectExpectedInstrumentDetails>(lst);
                    logHand.WriteTransactionLog(log);
                    return lst.Count;
                }
                else
                {
                    return -1;
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Insert project other related company for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public int InsertTbt_ProjectOtherRalatedCompany(List<tbt_ProjectOtherRalatedCompany> tbtProjectOtherRelated)
        {
            try
            {
                int maxSequence = 1;
                if (tbtProjectOtherRelated.Count > 0)
                {
                    List<tbt_ProjectOtherRalatedCompany> OriginOther = GetTbt_ProjectOtherRalatedCompanyForView(tbtProjectOtherRelated[0].ProjectCode);
                    if (OriginOther.Count > 0)
                        maxSequence = OriginOther.Max(a => a.SequenceNo) + 1;
                }

                foreach (tbt_ProjectOtherRalatedCompany i in tbtProjectOtherRelated)
                {
                    i.SequenceNo = maxSequence++;
                    i.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    i.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    i.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    i.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                }
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog log = new doTransactionLog();
                List<tbt_ProjectOtherRalatedCompany> lst = base.InsertTbt_ProjectOtherRalatedCompany(CommonUtil.ConvertToXml_Store<tbt_ProjectOtherRalatedCompany>(tbtProjectOtherRelated));
                if (lst.Count > 0)
                {
                    log.TransactionType = doTransactionLog.eTransactionType.Insert;
                    log.TableName = TableName.C_TBL_NAME_PRJ_OTH_COMP;
                    log.TableData = CommonUtil.ConvertToXml<tbt_ProjectOtherRalatedCompany>(lst);
                    logHand.WriteTransactionLog(log);
                    return lst.Count;
                }
                else
                {
                    return -1;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert project support staff detail
        /// </summary>
        /// <param name="tbtSupport"></param>
        /// <returns></returns>
        public int InsertTbt_ProjectSupportStaffDetail(List<tbt_ProjectSupportStaffDetails> tbtSupport)
        {
            try
            {
                foreach (tbt_ProjectSupportStaffDetails i in tbtSupport)
                {
                    i.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    i.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    i.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    i.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                }
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog log = new doTransactionLog();
                List<tbt_ProjectSupportStaffDetails> lst = base.InsertTbt_ProjectSupportStaffDetail(CommonUtil.ConvertToXml_Store<tbt_ProjectSupportStaffDetails>(tbtSupport));
                if (lst.Count > 0)
                {
                    log.TransactionType = doTransactionLog.eTransactionType.Insert;
                    log.TableName = TableName.C_TBL_NAME_PRJ_SUP_STAFF;
                    log.TableData = CommonUtil.ConvertToXml<tbt_ProjectSupportStaffDetails>(lst);
                    logHand.WriteTransactionLog(log);
                    return lst.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Insert project system detail
        /// </summary>
        /// <param name="tbtProductSystem"></param>
        /// <returns></returns>
        public int InsertTbt_ProjectSystemDetail(List<tbt_ProjectSystemDetails> tbtProductSystem)
        {
            try
            {
                foreach (tbt_ProjectSystemDetails i in tbtProductSystem)
                {
                    i.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    i.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    i.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    i.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                }
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog log = new doTransactionLog();
                List<tbt_ProjectSystemDetails> lst = base.InsertTbt_ProjectSystemDetail(CommonUtil.ConvertToXml_Store<tbt_ProjectSystemDetails>(tbtProductSystem));
                if (lst.Count > 0)
                {
                    log.TransactionType = doTransactionLog.eTransactionType.Insert;
                    log.TableName = TableName.C_TBL_NAME_PRJ_PRJ_SYSTEM;
                    log.TableData = CommonUtil.ConvertToXml<tbt_ProjectSystemDetails>(lst);
                    logHand.WriteTransactionLog(log);
                    return lst.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Get project purchaser customer for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public int InsertTbt_ProjectPurchaserCustomer(tbt_ProjectPurchaserCustomer tbtProjectPurchaser)
        {

            try
            {

                tbtProjectPurchaser.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                tbtProjectPurchaser.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                tbtProjectPurchaser.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                tbtProjectPurchaser.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog log = new doTransactionLog();
                List<tbt_ProjectPurchaserCustomer> lstPurchaser = new List<tbt_ProjectPurchaserCustomer>();
                lstPurchaser.Add(tbtProjectPurchaser);

                List<tbt_ProjectPurchaserCustomer> lst = base.InsertTbt_ProjectPurchaserCustomer(CommonUtil.ConvertToXml_Store<tbt_ProjectPurchaserCustomer>(lstPurchaser));
                if (lst.Count > 0)
                {
                    log.TransactionType = doTransactionLog.eTransactionType.Insert;
                    log.TableName = TableName.C_TBL_NAME_PRJ_CUST;
                    log.TableData = CommonUtil.ConvertToXml<tbt_ProjectPurchaserCustomer>(lst);
                    logHand.WriteTransactionLog(log);
                    return lst.Count;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Insert project stockout branch instrument detail
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public List<tbt_ProjectStockoutBranchIntrumentDetails> InsertTbt_ProjectStockoutBranchIntrumentDetails(List<tbt_ProjectStockoutBranchIntrumentDetails> lst)
        {
            foreach (tbt_ProjectStockoutBranchIntrumentDetails i in lst)
            {
                i.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                i.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                i.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                i.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            }
            List<tbt_ProjectStockoutBranchIntrumentDetails> Inserted = base.InsertTbt_ProjectStockoutBranchIntrumentDetails(CommonUtil.ConvertToXml_Store<tbt_ProjectStockoutBranchIntrumentDetails>(lst));
            // Transaction log
            if (Inserted.Count > 0)
            {
                ILogHandler logH = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog log = new doTransactionLog();
                log.TableName = TableName.C_TBL_NAME_PRJ_BRA_STOCKOUT;
                log.TransactionType = doTransactionLog.eTransactionType.Insert;
                log.TableData = CommonUtil.ConvertToXml_Store<tbt_ProjectStockoutBranchIntrumentDetails>(Inserted);
                logH.WriteTransactionLog(log);
            }
            return Inserted;
        }

        /// <summary>
        /// Using when Inventory stock-out intrument by project code
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="doInstrumentList"></param>
        /// <param name="strStockOutMemo"></param>
        public void UpdateStockOutInstrument(string strProjectCode, List<doInstrument> doInstrumentList, string strStockOutMemo)
        {
            try
            {
                //1.	Check mandatory data
                //strProjectCode and instrument at least 1 are require fields.
                doUpdateStockOutInstrumentData updateDo = new doUpdateStockOutInstrumentData();
                updateDo.ProjectCode = strProjectCode;
                ApplicationErrorException.CheckMandatoryField(updateDo);
                if (CommonUtil.IsNullOrEmpty(strProjectCode))
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "Project Code");
                if (doInstrumentList.Count <= 0)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "Instrument");

                //2.	Begin tran;

                //3.	Validate business 
                //3.1	Project code must exist in project table
                List<tbt_Project> projectList = base.GetTbt_Project(strProjectCode);

                //3.1.2.	Check project exist 
                //If doTbt_Project	is null Then
                if (projectList.Count <= 0 || projectList[0] == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, strProjectCode);

                //3.2	Project status must not be ‘Last completed’ or ‘Canceled’
                //3.2.1.	Project status must not be ‘Last completed’		
                if (projectList[0].ProjectStatus == ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3075);

                //3.2.2.	Project status must not be ‘Canceled’		
                if (projectList[0].ProjectStatus == ProjectStatus.C_PROJECT_STATUS_CANCEL)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3076);

                //3.3	Check exist in instrument master for all instruments in doInstrumentList
                //3.3.1.	Loop all instrument in doInstrumentList
                IInstrumentMasterHandler hand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                foreach (doInstrument doIn in doInstrumentList)
                {
                    //3.3.1.1.	Set local variable 
                    // blnExistInstrument = False

                    //3.3.1.2.	Check exist instrument in master
                    List<bool?> blnExistInstrument = hand.CheckExistInstrument(doIn.InstrumentCode);

                    //3.3.1.3.	If blnExistInstrument = False Then
                    if (blnExistInstrument[0].Value == false)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, doIn.InstrumentCode);
                    }
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    //Prepare insert data
                    tbt_ProjectStockoutInstrument doInsert = new tbt_ProjectStockoutInstrument();
                    doInsert.ProjectCode = strProjectCode;

                    //4.	Insert/update data in project stock-out intrument table
                    //4.1.1.	Loop all instrument in doInstrumentList
                    foreach (doInstrument doIn in doInstrumentList)
                    {
                        //4.1.1.2.	Check exist project stock-out intrument table
                        List<tbt_ProjectStockoutInstrument> doTbt_ProjectStockoutIntrument = this.GetTbt_ProjectStockoutInstrument(strProjectCode, doIn.InstrumentCode);

                        if (doTbt_ProjectStockoutIntrument.Count <= 0)
                        {
                            //4.1.1.3.	In case not exist: insert data to project stock-out intrument table
                            doInsert.InstrumentCode = doIn.InstrumentCode;
                            doInsert.InstrumentQty = doIn.InstrumentQty;
                            this.InsertTbt_ProjectStockoutInstrument(doInsert);
                        }
                        else
                        {
                            //4.1.1.4.	In case exist: update data to project stock-out intrument table
                            doTbt_ProjectStockoutIntrument[0].InstrumentQty = doTbt_ProjectStockoutIntrument[0].InstrumentQty + doIn.InstrumentQty;
                            this.UpdateTbt_ProjectStockoutInstrument(doTbt_ProjectStockoutIntrument[0]);
                        }
                    }

                    //=============== TRS update 11/06/2012 =======================
                    tbt_ProjectStockOutMemo doStockoutMemo = new tbt_ProjectStockOutMemo(); 
                    doStockoutMemo.ProjectCode = strProjectCode;
                    doStockoutMemo.Memo = strStockOutMemo;
                    InsertTbt_ProjectStockOutMemo(doStockoutMemo);   
                    //=============================================================

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update project data
        /// </summary>
        /// <param name="Project"></param>
        /// <returns></returns>
        public int UpdateTbt_ProjectData(tbt_Project Project)
        {
            try
            {
                Project.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                Project.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                List<tbt_Project> lst = new List<tbt_Project>();
                lst.Add(Project);
                List<tbt_Project> lstProjectUpdated = base.UpdateTbt_ProjectData(CommonUtil.ConvertToXml_Store<tbt_Project>(lst));
                if (lstProjectUpdated.Count > 0)
                {
                    doTransactionLog log = new doTransactionLog();
                    log.TableName = TableName.C_TBL_NAME_PRJ;
                    log.TransactionType = doTransactionLog.eTransactionType.Update;
                    log.TableData = CommonUtil.ConvertToXml_Store<tbt_Project>(lstProjectUpdated);
                    ILogHandler LogH = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    LogH.WriteTransactionLog(log);
                }
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update project purchaser customer
        /// </summary>
        /// <param name="PurchaserCustomer"></param>
        /// <returns></returns>
        public List<tbt_ProjectPurchaserCustomer> UpdateTbt_ProjectPurchaseCustomer(tbt_ProjectPurchaserCustomer PurchaserCustomer)
        {
            try
            {
                PurchaserCustomer.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                PurchaserCustomer.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                if (PurchaserCustomer != null)
                {
                    List<tbt_ProjectPurchaserCustomer> lst = new List<tbt_ProjectPurchaserCustomer>();
                    lst.Add(PurchaserCustomer);
                    List<tbt_ProjectPurchaserCustomer> Updated = base.UpdateTbt_ProjectPurchaseCustomer(CommonUtil.ConvertToXml_Store<tbt_ProjectPurchaserCustomer>(lst));
                    if (Updated.Count > 0)
                    {
                        // Transaction log

                        ILogHandler logH = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        doTransactionLog log = new doTransactionLog();
                        log.TableName = TableName.C_TBL_NAME_PRJ_CUST;
                        log.TransactionType = doTransactionLog.eTransactionType.Update;
                        log.TableData = CommonUtil.ConvertToXml_Store<tbt_ProjectPurchaserCustomer>(Updated);
                        logH.WriteTransactionLog(log);
                    }

                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Update project status
        /// </summary>
        /// <param name="Project"></param>
        /// <param name="NewProjectStatus"></param>
        /// <returns></returns>
        public int UpdateProjectStatus(string Project, string NewProjectStatus)
        {
            try
            {
                List<tbt_Project> Updated = new List<tbt_Project>();
                if (NewProjectStatus == ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE)
                    Updated = base.UpdateProjectStatus(Project, ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE, DateTime.Now, null, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                if (NewProjectStatus == ProjectStatus.C_PROJECT_STATUS_CANCEL)
                    Updated = base.UpdateProjectStatus(Project, ProjectStatus.C_PROJECT_STATUS_CANCEL, null, DateTime.Now, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                if (Updated.Count > 0)
                {
                    // Transaction log

                    ILogHandler logH = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    doTransactionLog log = new doTransactionLog();
                    log.TableName = TableName.C_TBL_NAME_PRJ;
                    log.TransactionType = doTransactionLog.eTransactionType.Update;
                    log.TableData = CommonUtil.ConvertToXml_Store<tbt_Project>(Updated);
                    logH.WriteTransactionLog(log);




                }
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update project expected instrument detail
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="strInstrumentCode"></param>
        /// <param name="intInstrumentQty"></param>
        /// <returns></returns>
        public List<tbt_ProjectExpectedInstrumentDetails> UpdateTbt_ProjectExpectedInstrumentDetails(string strProjectCode, string strInstrumentCode, int? intInstrumentQty)
        {
            List<tbt_ProjectExpectedInstrumentDetails> lst = base.UpdateTbt_ProjectExpectedInstrumentDetails(strProjectCode, strInstrumentCode, intInstrumentQty, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
            if (lst.Count > 0)
            {
                ILogHandler logH = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog log = new doTransactionLog();
                log.TableName = TableName.C_TBL_NAME_PRJ_EXP_INST;
                log.TransactionType = doTransactionLog.eTransactionType.Update;
                log.TableData = CommonUtil.ConvertToXml_Store<tbt_ProjectExpectedInstrumentDetails>(lst);
                logH.WriteTransactionLog(log);
            }

            return lst;
        }

        /// <summary>
        /// Update project stockout branch instrument detail
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="branchNo"></param>
        /// <param name="assignBranchQty"></param>
        /// <param name="instrumentCode"></param>
        /// <returns></returns>
        public List<tbt_ProjectStockoutBranchIntrumentDetails> UpdateTbt_ProjectStockoutBranchIntrumentDetails(string strProjectCode, int branchNo, int assignBranchQty, string instrumentCode)
        {
            List<tbt_ProjectStockoutBranchIntrumentDetails> lst = base.UpdateTbt_ProjectStockoutBranchIntrumentDetails(strProjectCode, branchNo, assignBranchQty, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, instrumentCode);
            if (lst.Count > 0)
            {
                // Transaction log
                ILogHandler logH = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog log = new doTransactionLog();
                log.TableName = TableName.C_TBL_NAME_PRJ_BRA_STOCKOUT;
                log.TransactionType = doTransactionLog.eTransactionType.Update;
                log.TableData = CommonUtil.ConvertToXml_Store<tbt_ProjectStockoutBranchIntrumentDetails>(lst);
                logH.WriteTransactionLog(log);
            }
            return lst;
        }

        /// <summary>
        /// Update project stockout instrument
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        public List<tbt_ProjectStockoutInstrument> UpdateTbt_ProjectStockoutInstrument(tbt_ProjectStockoutInstrument doUpdate)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbt_ProjectStockoutInstrument> rList = this.GetTbt_ProjectStockoutInstrument(doUpdate.ProjectCode, doUpdate.InstrumentCode);
                //if (rList[0].UpdateDate != doUpdate.UpdateDate)
                if (rList[0].UpdateDate != null && doUpdate.UpdateDate != null && DateTime.Compare(rList[0].UpdateDate.Value, doUpdate.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                //set updateDate and updateBy
                doUpdate.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doUpdate.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_ProjectStockoutInstrument> doUpdateList = new List<tbt_ProjectStockoutInstrument>();
                doUpdateList.Add(doUpdate);
                List<tbt_ProjectStockoutInstrument> updatedList = base.UpdateTbt_ProjectStockoutInstrument(CommonUtil.ConvertToXml_Store<tbt_ProjectStockoutInstrument>(doUpdateList));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_PROJ_STOCKOUT_INST;
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
        /// Getting project data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<dtProjectData> GetProjectDataForSearch(doSearchProjectCondition cond)
        {
            try
            {
                List<dtProjectData> lst = null;
                if (cond != null)
                {
                    lst = this.GetProjectDataForSearch(cond.ProjectCode
                                                        , cond.ContractCode
                                                        , cond.ProductCode
                                                        , cond.ProjectName
                                                        , cond.ProjectAddress
                                                        , cond.PJPurchaseName
                                                        , cond.Owner1Name
                                                        , cond.PJManagementCompanyName
                                                        , cond.OtherProjectRelatedPersonName
                                                        , cond.HeadSalesmanEmpName
                                                        , cond.ProjectManagerEmpName);

                }

                if (lst == null)
                    lst = new List<dtProjectData>();
                else
                    CommonUtil.MappingObjectLanguage<dtProjectData>(lst);

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get project information
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public dtProjectForInstall GetProjectDataForInstall(string strProjectCode)
        {
            try
            {
                List<dtProjectForInstall> dtProjectData = base.GetProjectForInstall(strProjectCode);
                if (dtProjectData.Count != 0)
                {
                    return dtProjectData[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get project system detail for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public override List<dtTbt_ProjectSystemDetailForView> GetTbt_ProjectSystemDetailForView(string strProjectCode)
        {
            try
            {
                List<dtTbt_ProjectSystemDetailForView> lstSysProd = base.GetTbt_ProjectSystemDetailForView(strProjectCode);
                CommonUtil.MappingObjectLanguage<dtTbt_ProjectSystemDetailForView>(lstSysProd);
                return lstSysProd;
            }
            catch (Exception)
            {
                throw;
            }



        }

        /// <summary>
        /// Get all tables of project for view
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <returns></returns>
        public List<doTbt_Project> GetTbt_ProjectForView(string strProjectCode)
        {
            List<tbt_Project> lstTbt_Project = base.GetTbt_ProjectForViewSQL(strProjectCode);
            List<doTbt_Project> doTbt_Project = CommonUtil.ClonsObjectList<tbt_Project, doTbt_Project>(lstTbt_Project);
            EmployeeMappingList emlst = new EmployeeMappingList();
            emlst.AddEmployee(doTbt_Project.ToArray());
            IEmployeeMasterHandler Emph = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            Emph.EmployeeListMapping(emlst);
            MiscTypeMappingList miscMapList = new MiscTypeMappingList();
            miscMapList.AddMiscType(doTbt_Project.ToArray());
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            comh.MiscTypeMappingList(miscMapList);

            return doTbt_Project;
        }

        /// <summary>
        /// Get all tables of project stockout memo for view
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public List<tbt_ProjectStockOutMemo> GetTbt_ProjectStockoutMemoForView(string projectCode)
        {
            try
            {
                List<tbt_ProjectStockOutMemo> lst = base.GetTbt_ProjectStockoutMemoForView(projectCode);
                EmployeeMappingList emlst = new EmployeeMappingList();
                emlst.AddEmployee(lst.ToArray());
                IEmployeeMasterHandler Emph = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                Emph.EmployeeListMapping(emlst);
                return lst;
            }
            catch (Exception)
            {

                throw;
            }


        }

        /// <summary>
        /// Delete project expected instrument detail
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="instrumentCode"></param>
        /// <returns></returns>
        public List<tbt_ProjectExpectedInstrumentDetails> DeleteTbt_ProjectExpectedInstrumentDetails(string strProjectCode, string strInstrumentCode)
        {
            try
            {
                //Delete data from DB
                List<tbt_ProjectExpectedInstrumentDetails> deletedList = base.DeleteTbt_ProjectExpectedInstrumentDetails(strProjectCode, strInstrumentCode);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_PRJ_EXP_INST;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete project system detail
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public List<tbt_ProjectSystemDetails> DeleteTbt_ProjectSystemDetails(string strProjectCode, string strProductCode)
        {
            try
            {
                //Delete data from DB
                List<tbt_ProjectSystemDetails> deletedList = base.DeleteTbt_ProjectSystemDetails(strProjectCode, strProductCode);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_PRJ_PRJ_SYSTEM;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete project support staff detail
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public List<tbt_ProjectSupportStaffDetails> DeleteTbt_ProjectSupportStaffDetails(string strProjectCode, string empNo)
        {
            try
            {
                //Delete data from DB
                List<tbt_ProjectSupportStaffDetails> deletedList = base.DeleteTbt_ProjectSupportStaffDetails(strProjectCode, empNo);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_PRJ_SUP_STAFF;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete project other relate company
        /// </summary>
        /// <param name="strProjectCode"></param>
        /// <param name="sequenceNo"></param>
        /// <returns></returns>
        public List<tbt_ProjectOtherRalatedCompany> DeleteTbt_ProjectOtherRalatedCompany(string strProjectCode, int sequenceNo)
        {
            try
            {
                //Delete data from DB
                List<tbt_ProjectOtherRalatedCompany> deletedList = base.DeleteTbt_ProjectOtherRalatedCompany(strProjectCode, sequenceNo);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_PRJ_OTH_COMP;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert project stockout memo
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_ProjectStockOutMemo> InsertTbt_ProjectStockOutMemo(tbt_ProjectStockOutMemo doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_ProjectStockOutMemo> doInsertList = new List<tbt_ProjectStockOutMemo>();
                doInsertList.Add(doInsert);
                List<tbt_ProjectStockOutMemo> insertList = base.InsertTbt_ProjectStockOutMemo(CommonUtil.ConvertToXml_Store<tbt_ProjectStockOutMemo>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_PRJ_STOCKOUT;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
