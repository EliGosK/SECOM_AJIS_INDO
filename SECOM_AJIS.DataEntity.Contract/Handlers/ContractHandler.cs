using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using System.Net.Mail;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.Common.Models.EmailTemplates;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class ContractHandler : BizCTDataEntities, IContractHandler
    {
        /// <summary>
        /// Get contract header from data
        /// </summary>
        /// <param name="contracts"></param>
        /// <returns></returns>
        public List<doContractHeader> GetContractHeaderData(List<tbt_SaleBasic> contracts)
        {
            try
            {
                if (contracts != null)
                {
                    foreach (tbt_SaleBasic c in contracts)
                    {
                        c.OCC = "0001";
                        c.DistributedOriginCode = "Q0000000001";
                    }
                }

                string xml = CommonUtil.ConvertToXml_Store<tbt_SaleBasic>(contracts, "ContractCode", "OCC", "DistributedOriginCode");
                return this.GetContractHeaderData(xml);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get contract header by language from data
        /// </summary>
        /// <param name="contracts"></param>
        /// <returns></returns>
        public List<doContractHeader> GetContractHeaderDataByLanguage(List<tbt_SaleBasic> contracts)
        {
            try
            {
                return CommonUtil.ConvertObjectbyLanguage<doContractHeader, doContractHeader>(
                                                        this.GetContractHeaderData(contracts),
                                                        "ProductName");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To check whether all contract have the same site
        /// </summary>
        /// <param name="contracts"></param>
        /// <returns></returns>
        public bool IsSameSite(List<string> contracts)
        {
            bool bSameSite = true;
            try
            {
                if (contracts != null & contracts.Count > 1)
                {
                    //Prepare ContarctCode data
                    StringBuilder sb = new StringBuilder("");
                    String contractCodeList = string.Empty;
                    foreach (string contract in contracts)
                    {
                        sb.AppendFormat("\'{0}\',", contract);
                    }

                    if (sb.Length > 0)
                        contractCodeList = sb.Remove(sb.Length - 1, 1).ToString();

                    //1. Get site of rental contract & 2. Get site of sale contract
                    IContractHandler hand = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                    List<dsGetSiteContractList> list = hand.GetSiteContractList(contractCodeList, FlagType.C_FLAG_ON);

                    //3. Check site of all contract codes
                    if (list != null && list.Count > 0)
                    {
                        string strSite = list[0].SiteCode;

                        foreach (dsGetSiteContractList site in list)
                        {
                            if (site.SiteCode != strSite)
                            {
                                bSameSite = false;
                                return bSameSite;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return bSameSite;
        }

        /// <summary>
        /// To get site name from provided contract code.
        /// </summary>
        /// <param name="strContractCodeList"></param>
        /// <param name="bLastestOCCFlag"></param>
        /// <returns></returns>
        public List<dsGetSiteContractList> GetSiteContractList(string strContractCodeList, bool bLastestOCCFlag)
        {
            return base.GetSiteContractList(strContractCodeList, bLastestOCCFlag);
        }

        /// <summary>
        /// To check maintenance target contract
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strQuotationTargetCode"></param>
        /// <returns></returns>
        public doContractHeader CheckMaintenanceTargetContract(string strContractCode, string strQuotationTargetCode)
        {
            doContractHeader headerDo = new doContractHeader();
            try
            {
                //1. Get rental contract & 2. Get sale contract
                IContractHandler hand = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                List<dtGetMaintenanceTargetContract> contractList = hand.GetMaintenanceTargetContract(strContractCode, FlagType.C_FLAG_ON);

                //6. Check contract data
                if (contractList != null && contractList.Count > 0)
                {
                    CommonUtil cmm = new CommonUtil();
                    string contractCode = cmm.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    string strProductTypeCode = contractList[0].ProductTypeCode;
                    string strContractStatus = (contractList[0].ContractStatus != null) ? contractList[0].ContractStatus.Trim() : contractList[0].ContractStatus;

                    if (strProductTypeCode != ProductType.C_PROD_TYPE_SALE
                        && strProductTypeCode != ProductType.C_PROD_TYPE_AL
                        && strProductTypeCode != ProductType.C_PROD_TYPE_ONLINE
                        && strProductTypeCode != ProductType.C_PROD_TYPE_RENTAL_SALE)
                        throw ApplicationErrorException.ThrowErrorException(
                            MessageUtil.MODULE_CONTRACT,
                            MessageUtil.MessageList.MSG3118,
                            new string[] { contractCode });

                    if (CommonUtil.dsTransData.dtTransHeader.ScreenID != ScreenID.C_SCREEN_ID_QTN_DETAIL)
                    {
                        if (strContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                            throw ApplicationErrorException.ThrowErrorException(
                                MessageUtil.MODULE_CONTRACT,
                                MessageUtil.MessageList.MSG3119,
                                new string[] { contractCode });
                    }

                    if (strContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                        throw ApplicationErrorException.ThrowErrorException(
                            MessageUtil.MODULE_CONTRACT,
                            MessageUtil.MessageList.MSG3044,
                            new string[] { contractCode });

                    if (strContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL
                        || strContractStatus == ContractStatus.C_CONTRACT_STATUS_END)
                        throw ApplicationErrorException.ThrowErrorException(
                            MessageUtil.MODULE_CONTRACT,
                            MessageUtil.MessageList.MSG3105,
                            new string[] { contractCode });

                    if (strContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                        throw ApplicationErrorException.ThrowErrorException(
                            MessageUtil.MODULE_CONTRACT,
                            MessageUtil.MessageList.MSG3116,
                            new string[] { contractCode });

                    List<tbt_RelationType> relationTypeList
                        = base.CheckRelationType(strContractCode
                                                    , ContractStatus.C_CONTRACT_STATUS_END
                                                    , ContractStatus.C_CONTRACT_STATUS_CANCEL
                                                    , ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL
                                                    , strQuotationTargetCode
                                                    , null);
                    //                                  , (strProductTypeCode == ProductType.C_PROD_TYPE_SALE? RelationType.C_RELATION_TYPE_SALE : RelationType.C_RELATION_TYPE_MA));

                    if (relationTypeList != null && relationTypeList.Count > 0)
                    {
                        CommonUtil c = new CommonUtil();

                        /* --- Merge --- */
                        /*  throw ApplicationErrorException.ThrowErrorException(
                            MessageUtil.MODULE_CONTRACT, 
                            MessageUtil.MessageList.MSG3120,
                            c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                            c.ConvertContractCode(relationTypeList[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT)
                            ); */
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT,
                            MessageUtil.MessageList.MSG3120,
                            contractCode,
                            c.ConvertContractCode(relationTypeList[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                        /* ------------- */

                    }

                    //7. Map dtContractData to doContractHeader
                    contractList = CommonUtil.ConvertObjectbyLanguage<dtGetMaintenanceTargetContract, dtGetMaintenanceTargetContract>(contractList, "ProductName");
                    headerDo.ContractCode = contractList[0].ContractCode;
                    headerDo.CreateDate = contractList[0].CreateDate;
                    headerDo.ProductCode = contractList[0].ProductCode;
                    headerDo.ProductName = contractList[0].ProductName;
                    headerDo.ProductNameEN = contractList[0].ProductNameEN;
                    headerDo.ProductNameJP = contractList[0].ProductNameJP;
                    headerDo.ProductNameLC = contractList[0].ProductNameLC;
                    headerDo.ProductTypeCode = contractList[0].ProductTypeCode;

                    /* --- Merge --- */
                    headerDo.SiteCode = contractList[0].SiteCode;
                    /* ------------- */
                }
                else
                {
                    CommonUtil c = new CommonUtil();
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return headerDo;
        }

        /// <summary>
        /// To check maintenance target contract list
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="bLastestOCCFlag"></param>
        /// <returns></returns>
        public List<dtGetMaintenanceTargetContract> GetMaintenanceTargetContract(string strContractCode, bool bLastestOCCFlag)
        {
            return base.GetMaintenanceTargetContract(strContractCode, bLastestOCCFlag);
        }

        /// <summary>
        /// To check maintenance target contract list
        /// </summary>
        /// <param name="contracts"></param>
        /// <param name="quotationTargetCode"></param>
        /// <returns></returns>
        public List<doContractHeader> CheckMaintenanceTargetContractList(List<string> contracts, string quotationTargetCode)
        {
            bool isEmpty = true;
            if (contracts != null)
            {
                if (contracts.Count > 0)
                    isEmpty = false;
            }
            if (isEmpty)
                return new List<doContractHeader>();

            try
            {
                //1.	Initial data
                //1.1.	Create data set
                dsMaintenanceTargetList dsTargetList = new dsMaintenanceTargetList();
                dsTargetList.doContractHeaderList = new List<doContractHeader>();


                /* --- Merge --- */
                //2.	Check site of maintenance target contract
                //bool bSameSite = this.IsSameSite(contracts);
                //if(bSameSite == false)
                //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3093);
                /* ------------- */

                //3. Loop to check maintenance target contract of dtContractCode
                int iAlarmCnt = 0;
                int iSaleCnt = 0;

                /* --- Merge --- */
                string bSiteCode = null;
                /* ------------- */

                foreach (string contractCode in contracts)
                {
                    //3.1.	Call ContractHandler.CheckMaintenanceTargetContract
                    doContractHeader doHeader = this.CheckMaintenanceTargetContract(contractCode, quotationTargetCode);

                    //3.2 Add doContractHeader to dsMaintenanceTargetList
                    dsTargetList.doContractHeaderList.Add(doHeader);

                    /* --- Merge --- */
                    if (bSiteCode == null)
                        bSiteCode = doHeader.SiteCode;
                    if (doHeader.SiteCode != bSiteCode)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3093);
                    /* ------------- */

                    //if not error
                    if (doHeader.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                        || doHeader.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                        || doHeader.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                        iAlarmCnt++;

                    if (doHeader.ProductTypeCode == ProductType.C_PROD_TYPE_SALE)
                        iSaleCnt++;
                }

                if (iAlarmCnt > 1)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3224);

                if (iAlarmCnt == 1 && iSaleCnt > 0)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0132);

                return dsTargetList.doContractHeaderList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To generate contract code
        /// </summary>
        /// <param name="strProductTypeCode"></param>
        /// <returns></returns>
        public string GenerateContractCode(string strProductTypeCode)
        {

            try
            {
                //1.	Get product type data
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<tbs_ProductType> productTypeList = hand.GetTbs_ProductType(null, strProductTypeCode);

                //1.2.	Check existing of returned data
                if (productTypeList.Count <= 0 || productTypeList[0].ContractPrefix == null || productTypeList[0].ContractPrefix == string.Empty)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3022);
                }

                //2.	Call method for get next running code
                List<doRunningNo> runningNoList = hand.GetNextRunningCode(NameCode.C_NAME_CODE_CONTRACT_CODE);

                if (runningNoList.Count <= 0 || runningNoList[0].RunningNo == null || runningNoList[0].RunningNo == string.Empty)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3130);
                }

                //3.	Call method for get the check digit
                //3.1.	Call		CommonHandler.GenerateCheckDigit
                string iCheckDigit = hand.GenerateCheckDigit(runningNoList[0].RunningNo);

                //4.	Create contract code
                //4.1.	Set strContractCode = dtTbs_ProductType[0].ContractPrefix + strRunningCode + iCheckDigit
                string strContractCode = productTypeList[0].ContractPrefix + runningNoList[0].RunningNo + iCheckDigit;

                //5.	Return strContractCode
                return strContractCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update data in case new/add sale and will insert new occurrence in case other installation type.
        /// </summary>
        /// <param name="doComplete"></param>
        public void CompleteInstallation(doCompleteInstallationData doComplete)
        {
            try
            {
                //Validation ServiceTypeCode and CompleteInstallationProcessFlag
                ApplicationErrorException.CheckMandatoryField<doCompleteInstallationData, doCompleteContractCompleteInstallation>(doComplete);

                //2. Check service type code and complete process flag
                if (doComplete.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    if (doComplete.CompleteInstallationProcessFlag == FlagType.C_FLAG_ON)
                    {
                        IRentralContractHandler hand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        hand.CompleteInstallation(doComplete);

                    }
                    else if (doComplete.CompleteInstallationProcessFlag == FlagType.C_FLAG_OFF)
                    {
                        //do nothing
                    }
                }
                else if (doComplete.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    ISaleContractHandler hand = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                    if (doComplete.CompleteInstallationProcessFlag == FlagType.C_FLAG_ON)
                    {
                        hand.CompleteInstallation(doComplete);
                    }
                    else if (doComplete.CompleteInstallationProcessFlag == FlagType.C_FLAG_OFF)
                    {
                        hand.CancelInstallation(doComplete);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To send notify email for change contract fee
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="BatchDate"></param>
        /// <returns></returns>
        public SECOM_AJIS.Common.Util.doBatchProcessResult SendNotifyEmailForChangeFee(string UserId, DateTime BatchDate)
        {
            SECOM_AJIS.Common.Util.doBatchProcessResult doResult = new SECOM_AJIS.Common.Util.doBatchProcessResult();
            try
            {
                ILogHandler handLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;

                //1. Get list of unsent email
                List<tbt_ContractEmail> dtTbt_ContractEmail = this.GetUnsentNotifyEmail();

                //2. For each unsent email in dtTbt_ContractEmail
                int failedItem = 0;
                int completdeItem = 0;
                foreach (tbt_ContractEmail email in dtTbt_ContractEmail)
                {
                    //2.1 Prepare destination email
                    //2.1.1 Get employee data of destination email
                    IEmployeeMasterHandler handMaster = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                    List<tbm_Employee> dtEmployee = handMaster.GetTbm_Employee(email.ToEmpNo);

                    //2.1.2 If dtEmployee is empty
                    if (dtEmployee.Count <= 0)
                    {
                        handLog.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, (MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3077, email.ToEmpNo)).Message, EventID.C_EVENT_ID_NOTIFY_EMAIL_ERROR);
                        failedItem += 1;
                        continue;
                    }

                    //2.1.3 Get default email if endDate is not empty
                    String strEmailTo = (dtEmployee[0].EndDate != null) ?
                        this.getEmailsOfDefaultDepartment() :
                        dtEmployee[0].EmailAddress;

                    //2.2 Prepare email object for sending
                    doEmailProcess emailProc = new doEmailProcess();
                    emailProc.MailTo = strEmailTo;
                    emailProc.MailFrom = email.EmailFrom;
                    emailProc.Subject = email.EmailSubject;
                    emailProc.Message = email.EmailContent;

                    //2.3 Send email
                    //2.3.1 Retry for 3 times while strStatus is still fails
                    int retry = 0;
                    do
                    {
                        try
                        {
                            ICommonHandler commonHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                            commonHand.SendMail(emailProc);
                            break;
                        }
                        catch (Exception)
                        {
                            retry += 1;
                        }

                    } while (retry < 3);

                    ICommonContractHandler comContractHand = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                    if (retry == 3)
                    {
                        failedItem += 1;
                        email.FailSendingCounter = (email.FailSendingCounter == null) ? 1 : email.FailSendingCounter + 1;
                        email.UpdateBy = UserId;
                        //Update to database
                        comContractHand.UpdateTbt_ContractEmail(email);

                        //Check the number of fail for reporting error
                        if (email.FailSendingCounter >= 6 && email.FailSendingCounter % 3 == 0)
                        {
                            handLog.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, (MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3078, strEmailTo)).Message, EventID.C_EVENT_ID_NOTIFY_EMAIL_ERROR);
                        }
                    }
                    else
                    {
                        completdeItem += 1;

                        //Delete sent email
                        comContractHand.DeleteTbt_ContractEmail(email.ContractEmailID, UserId);
                    }
                }

                //3. Prepare process result for returning
                doResult.Result = FlagType.C_FLAG_ON;
                doResult.Total = dtTbt_ContractEmail.Count;
                doResult.Complete = completdeItem;
                doResult.Failed = failedItem;

            }
            catch (Exception)
            {
                throw;
            }
            return doResult;
        }

        /// <summary>
        /// Get unsent notify email
        /// </summary>
        /// <returns></returns>
        public List<tbt_ContractEmail> GetUnsentNotifyEmail()
        {
            return base.GetUnsentNotifyEmail(ContractEmailType.C_CONTRACT_EMAIL_TYPE_NOTIFY_CHANGE_FEE, FlagType.C_FLAG_OFF);
        }

        /// <summary>
        /// Get email of default department
        /// </summary>
        /// <returns></returns>
        private String getEmailsOfDefaultDepartment()
        {
            //1.1 Get default department to send email
            ICommonHandler handCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemConfig> doSystem = handCommon.GetSystemConfig(ConfigName.C_CONFIG_DEPARTMENT_NOTIFY_CHANGE_FEE);
            String strDeptCode = doSystem[0].ConfigValue;

            //1.2 Get employee belonging to default department
            //1.2.1 Get belonging list
            IEmployeeMasterHandler handMaster = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            List<dtBelonging> dtBelonging = handMaster.GetBelonging(null, strDeptCode, null, null);

            List<tbm_Employee> empList = new List<tbm_Employee>();
            foreach (dtBelonging belonging in dtBelonging)
            {
                tbm_Employee employee = new tbm_Employee();
                employee.EmpNo = belonging.EmpNo;
                empList.Add(employee);
            }

            //1.2.2 Get employee list
            List<tbm_Employee> dtEmployee = handMaster.GetEmployeeList(empList);

            //1.3 Generate list of email
            //1.3.1 Set emailTo = Combine all dtEmployee.EmailAddrss with seperated by semi-colon
            StringBuilder sbEmailTo = new StringBuilder("");
            foreach (tbm_Employee employee in dtEmployee)
            {
                sbEmailTo.Append(employee.EmailAddress + ";");
            }

            if (sbEmailTo.Length > 0)
                sbEmailTo = sbEmailTo.Remove(sbEmailTo.Length - 1, 1);

            return sbEmailTo.ToString();
        }

        /// <summary>
        /// To generate notify email for change contract fee
        /// </summary>
        /// <param name="doNotifyEmail"></param>
        /// <returns></returns>
        public doNotifyChangeFeeContract GenerateNotifyEmail(doNotifyChangeFeeContract doNotifyEmail)
        {
            //ICommonHandler commonHandler;
            //doTbs_EmailTemplate doTbsEmailTemplate;

            //try
            //{
            //    commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            //    doTbsEmailTemplate = commonHandler.GetTbs_EmailTemplate(C_EMAIL_TEMPLATE_NAME_CHANGE_FEE);
            //    doTbsEmailTemplate.TemplateContent.Replace("{1}",doNotifyEmail.ContractCode);
            //    doTbsEmailTemplate.TemplateContent.Replace("{2}", doNotifyEmail.ContractTargetNameEN);
            //    doTbsEmailTemplate.TemplateContent.Replace("{3}", doNotifyEmail.ContractTargetNameLC);
            //    doTbsEmailTemplate.TemplateContent.Replace("{4}", doNotifyEmail.SiteNameEN);
            //    doTbsEmailTemplate.TemplateContent.Replace("{5}", doNotifyEmail.SiteNameLC);
            //    doTbsEmailTemplate.TemplateContent.Replace("{6}", doNotifyEmail.ChangeDateOfContractFee);
            //    doTbsEmailTemplate.TemplateContent.Replace("{7}", doNotifyEmail.ContractFeeBeforeChange);
            //    doTbsEmailTemplate.TemplateContent.Replace("{8}", doNotifyEmail.ContractFeeAfterChange);
            //    doTbsEmailTemplate.TemplateContent.Replace("{9}", doNotifyEmail.ReturnToOriginalFeeDate);
            //    doTbsEmailTemplate.TemplateContent.Replace("{10}", doNotifyEmail.OperationOffice);
            //    doTbsEmailTemplate.TemplateContent.Replace("{11}", doNotifyEmail.RegisterChangeEmpNameEN);
            //    doTbsEmailTemplate.TemplateContent.Replace("{12}", doNotifyEmail.RegisterChangeEmpNameLC);
            //    doTbsEmailTemplate.TemplateContent.Replace("{13}", doNotifyEmail.BillingOffice);
            //    doTbsEmailTemplate.TemplateContent.Replace("{14}", doNotifyEmail.Sender);

            //    doNotifyEmail.Subject = doTbs_EmailTemplate.TemplateSubject;
            //    doNotifyEmail.Content = doTbs_EmailTemplate.TemplateContent;

            //}
            //catch (Exception ex)
            //{                
            //    throw ex;
            //}

            return doNotifyEmail;
        }

        /// <summary>
        /// Insert contract email data
        /// </summary>
        /// <param name="listEmail"></param>
        /// <returns></returns>
        public int InsertTbt_ContractEmail(List<tbt_ContractEmail> listEmail)
        {
            try
            {
                if (listEmail != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_ContractEmail relationType in listEmail)
                    {
                        relationType.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        relationType.CreateBy = dsTrans.dtUserData.EmpNo;
                        relationType.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        relationType.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_ContractEmail> res = this.InsertTbt_ContractEmail(
                    CommonUtil.ConvertToXml_Store<tbt_ContractEmail>(listEmail));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_CON_EMAIL,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Update contract email data
        /// </summary>
        /// <param name="listEmail"></param>
        /// <returns></returns>
        public int UpdateTbt_ContractEmail(List<tbt_ContractEmail> listEmail) //Add by Jutarat A. on 14012014
        {
            try
            {
                if (listEmail != null)
                {
                    List<tbt_ContractEmail> rList;
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_ContractEmail data in listEmail)
                    {
                        rList = this.GetTbt_ContractEmail(data.ContractEmailID);
                        if (DateTime.Compare(rList[0].UpdateDate.Value, data.UpdateDate.Value) != 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                        }

                        data.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        data.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_ContractEmail> res = this.UpdateTbt_ContractEmail(CommonUtil.ConvertToXml_Store<tbt_ContractEmail>(listEmail));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_CON_EMAIL,
                    TableData = CommonUtil.ConvertToXml(res)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);

                #endregion

                return res.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check site code whether be used in draft contract, contract , AR and incident 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public bool IsUsedSiteData(string siteCode)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(siteCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "SiteCode" });
                }

                List<int?> list = base.IsUsedSite(siteCode);

                bool result = false;
                if (list.Count > 0)
                {
                    if (list[0].Value == 1)
                        result = true;
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check customer code whether be used in draft contract, contract , AR, incident or Project
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public bool IsUsedCustomerData(string custCode)
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(custCode))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "CustCode" });
                }

                List<int?> list = base.IsUsedCustomer(custCode);

                bool result = false;
                if (list.Count > 0)
                {
                    if (list[0].Value == 1)
                        result = true;
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// GetContralLastOCC
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        public List<string> GetContractLastOCC(string ContractCode)
        {
            try
            {
                List<string> result = new List<string>();
                result = base.GetRentalLastOCC(ContractCode);

                if (result.Count == 0)
                {
                    result = base.GetSaleLastOCC(ContractCode);
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendEmailChangePlanBeforeStart(doChangePlanBeforeStartEmail templateObj)
        {
            try
            {
                if (templateObj == null)
                {
                    throw new ArgumentNullException("templateObj");
                }

                ICommonHandler common = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var config = common.GetSystemConfig(ConfigName.C_CONFIG_CHANGEPLAN_BEFORE_START_EMAIL).FirstOrDefault();

                if (config == null)
                {
                    throw new ApplicationException("Missing tbs_configuration : " + ConfigName.C_CONFIG_CHANGEPLAN_BEFORE_START_EMAIL);
                }

                EmailTemplateUtil mailUtil = new EmailTemplateUtil(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_CHANGEPLAN_BEFORE_START);
                var mailTemplate = mailUtil.LoadTemplate(templateObj);

                doEmailProcess mailMsg = new doEmailProcess();
                mailMsg.MailFrom = CommonUtil.dsTransData.dtUserData.EmailAddress;
                mailMsg.MailFromAlias = null;
                mailMsg.MailTo = config.ConfigValue;
                mailMsg.Subject = mailTemplate.TemplateSubject;
                mailMsg.Message = mailTemplate.TemplateContent;
                mailMsg.IsBodyHtml = true;

                ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                comHandler.SendMail(mailMsg);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<dtUnreceivedContractDocuemntCTR095> GetUnreceivedContractDocuemntCTR095(DateTime? GenerateDateFrom, DateTime? GenerateDateTo)
        {
            try
            {
                List<dtUnreceivedContractDocuemntCTR095> result = new List<dtUnreceivedContractDocuemntCTR095>();
                result = base.GetUnreceivedContractDocumentCTR095(GenerateDateFrom, GenerateDateTo);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

}
