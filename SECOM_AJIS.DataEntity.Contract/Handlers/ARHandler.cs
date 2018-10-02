using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using System.Data;
using System.Reflection;
using System.Collections;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Common.Models.EmailTemplates;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class ARHandler : BizCTDataEntities, IARHandler
    {
        /// <summary>
        /// To generate AR approve no.
        /// </summary>
        /// <param name="strARInteractionType"></param>
        /// <returns></returns>
        public string GenerateARApproveNo(string strARStatus) //strARInteractionType //Modify by Jutarat A. on 28082012
        {
            try
            {
                string strPrefix = String.Empty;

                //Modify by Jutarat A. on 28082012
                //1. Set prefix of approve no.
                //if (strARInteractionType == ARInteractionType.C_AR_INTERACTION_TYPE_APPROVE)
                //    strPrefix = ARApproveNo.C_AR_APPROVE_NO_PREFIX_APPROVE;
                //else if (strARInteractionType == ARInteractionType.C_AR_INTERACTION_TYPE_REJECT)
                //    strPrefix = ARApproveNo.C_AR_APPROVE_NO_PREFIX_REJECT;
                //else if (strARInteractionType == ARInteractionType.C_AR_INTERACTION_TYPE_INSTRUCTION)
                //    strPrefix = ARApproveNo.C_AR_APPROVE_NO_PREFIX_INSTRUCTION;
                if (strARStatus == ARStatus.C_AR_STATUS_APPROVED)
                    strPrefix = ARApproveNo.C_AR_APPROVE_NO_PREFIX_APPROVE;
                else if (strARStatus == ARStatus.C_AR_STATUS_REJECTED)
                    strPrefix = ARApproveNo.C_AR_APPROVE_NO_PREFIX_REJECT;
                else if (strARStatus == ARStatus.C_AR_STATUS_INSTRUCTED)
                    strPrefix = ARApproveNo.C_AR_APPROVE_NO_PREFIX_INSTRUCTION;
                //End Modify

                //2. Set year of approve no.
                string strYear = DateTime.Now.ToString("yy");

                //3. Get max running no. of approve no. from DB.
                #region Get max running no. of approve no. from DB.
                //3.1 Call sp_CT_GetTbs_ARApproveNoRunningNo 
                List <dtTbs_ARApproveNoRunningNo> runningNoList = base.GetTbs_ARApproveNoRunningNo(strYear, strPrefix);

                //3.2 Check return result
                string strRunningNo = string.Empty;
                if (runningNoList != null && runningNoList.Count > 0)
                {
                    //3.2.1 Set running no.
                    //strRunningNo = runningNoList[0].RunningNo;
                    int runningNo = runningNoList[0].RunningNo.GetValueOrDefault();
                    runningNo = runningNo + 1;
                    strRunningNo = runningNo.ToString().PadLeft(5, '0');

                    //3.2.2 Check over maximum range of running no.
                    int approveNoMax = Convert.ToInt32(ARApproveNo.C_AR_APPROVE_NO_MAXIMUM);
                    if (runningNo > approveNoMax)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3018);
                    }
                    else if (runningNo <= approveNoMax)
                    {
                        //Comment by Jutarat A. 17102012
                        ////Check whether this record is the most updated data
                        //List<dtTbs_ARApproveNoRunningNo> rList = base.GetTbs_ARApproveNoRunningNo(strYear, strPrefix);
                        ////if (rList[0].UpdateDate != runningNoList[0].UpdateDate)
                        //if(DateTime.Compare(rList[0].UpdateDate.Value, runningNoList[0].UpdateDate.Value) != 0) 
                        //{
                        //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG0019);
                        //}
                        //else
                        //{
                            //Update running no. to DB
                            List<tbs_ARApproveNoRunningNo> updatedList
                                = base.UpdateTbs_ARApproveNoRunningNo(strYear, strPrefix, runningNo, CommonUtil.dsTransData.dtUserData.EmpNo);

                            //Insert Log
                            if (updatedList.Count > 0)
                            {
                                doTransactionLog logData = new doTransactionLog();
                                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                                logData.TableName = TableName.C_TBL_NAME_AR_APVNO_RUNNO;
                                logData.TableData = CommonUtil.ConvertToXml(updatedList);
                                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                                hand.WriteTransactionLog(logData);
                            }
                        //}

                    }
                }
                else
                {
                    //3.2.2 Set running no.
                    strRunningNo = ARApproveNo.C_AR_APPROVE_NO_MINIMUN;
                    int runningNo = int.Parse(strRunningNo);

                    //3.2.4 Insert running no. to DB
                    List<tbs_ARApproveNoRunningNo> insertedList
                        = base.InsertTbs_ARApproveNoRunningNo(strYear, strPrefix, runningNo, CommonUtil.dsTransData.dtUserData.EmpNo);

                    //Insert Log
                    if (insertedList.Count > 0)
                    {
                        doTransactionLog logData = new doTransactionLog();
                        logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                        logData.TableName = TableName.C_TBL_NAME_AR_APVNO_RUNNO;
                        logData.TableData = CommonUtil.ConvertToXml(insertedList);
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }
                }
                #endregion

                //4. Create AR approve no. [ApproveNoPrefix]-[Year]-[Running no.]
                string strApproveNo = String.Format("{0}-{1}-{2}", strPrefix, strYear, strRunningNo);

                //5. return strApproveNo
                return strApproveNo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To generate AR request no.
        /// </summary>
        /// <param name="strARRelevantType"></param>
        /// <param name="strARRelevantCode"></param>
        /// <returns></returns>
        public string[] GenerateARRequestNo(String strARRelevantType, String strARRelevantCode)
        {
            try
            {
                string strAROffice = string.Empty;
                //1. Set AR office from DB
                #region Set AR office from DB
                //1.1. C_AR_RELEVANT_TYPE_CUSTOMER
                if (strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
                {
                    strAROffice = CommonUtil.dsTransData.dtUserData.MainOfficeCode;
                }
                //1.2 C_AR_RELEVANT_TYPE_SITE
                else if (strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
                {
                    //1.2.1 Get same site of contract
                    IViewContractHandler hand = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                    List<dtContractsSameSite> contractList 
                        = hand.GetContractsSameSiteList(strARRelevantCode, null);

                    //1.2.2. If dtContractsSameSite[].Rows.Count <= 0 Then
                    if (contractList.Count <= 0)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3192);

                    //1.2.3 Operation office code from search oldest contract which related to such site in the order of N/Q/MA/S
                    strAROffice = findMinimumContractCode(contractList);
                }
                //1.3 C_AR_RELEVANT_TYPE_PROJECT
                else if (strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT)
                {
                    strAROffice = CommonUtil.dsTransData.dtUserData.MainOfficeCode;
                }
                //1.4 C_AR_RELEVANT_TYPE_CONTRACT
                else if (strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                {
                    //1.4.1 Get operation office of rental contract
                    IRentralContractHandler hand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    List<tbt_RentalContractBasic> rentalList = hand.GetTbt_RentalContractBasic(strARRelevantCode, null);

                    //1.4.2 If list is not empty
                    if (rentalList != null && rentalList.Count > 0)
                    {
                        strAROffice = rentalList[0].OperationOfficeCode;
                    }
                    else
                    {
                        //Get operation office of sale contract
                        ISaleContractHandler saleHand = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        List<tbt_SaleBasic> saleList = saleHand.GetTbt_SaleBasic(strARRelevantCode,null,null);

                        if (saleList != null && saleList.Count > 0)
                        {
                            strAROffice = saleList[0].OperationOfficeCode;
                        }
                        else
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3192);
                        }
                    }
                }
                else if (strARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
                {
                    //1.5.1.	Get quotation target information
                    IQuotationHandler hand = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
                    cond.QuotationTargetCode = strARRelevantCode;
                    List<tbt_QuotationTarget> quotationList = hand.GetTbt_QuotationTarget(cond);

                    if (quotationList != null && quotationList.Count > 0)
                    {
                        strAROffice = quotationList[0].OperationOfficeCode;
                    }
                    else
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3192);
                    }
                }
                #endregion
            
                //2. Get max running no. of AR from DB.
                //2.1 Set year
                string strYear = DateTime.Now.ToString("yy");
                string strPrefix = ARNo.C_AR_NO_PREFIX;

                //2.2 Call sp_CT_GetTbs_ARRunningNo
                List<dtTbs_ARRunningNo> runningNoList = base.GetTbs_ARRunningNo(strAROffice, strYear, strPrefix);

                //2.3 Check return result
                #region Check return result
                string strRunningNo = string.Empty;
                if (runningNoList != null && runningNoList.Count > 0)
                {
                    //2.3.1 Set running no.
                    //strRunningNo = runningNoList[0].ARRunningNo;
                    int runningNo = runningNoList[0].ARRunningNo.GetValueOrDefault();
                    runningNo = runningNo + 1;
                    strRunningNo = runningNo.ToString().PadLeft(5, '0');

                    //2.3.2 Check over maximum range of running no.
                    int approveNoMax = Convert.ToInt32(ARNo.C_AR_NO_MAXIMUM);
                    if (runningNo > approveNoMax)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3017);
                    }
                    else if (runningNo <= approveNoMax)
                    {
                        //Comment by Jutarat A. 17102012
                        ////Check whether this record is the most updated data
                        //List<dtTbs_ARRunningNo> rList = base.GetTbs_ARRunningNo(strAROffice, strYear, strPrefix);
                        ////if (rList[0].UpdateDate != runningNoList[0].UpdateDate)
                        //if (DateTime.Compare(rList[0].UpdateDate.Value, runningNoList[0].UpdateDate.Value) != 0)
                        //{
                        //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG0019);
                        //}
                        //else
                        //{
                            //Update running no. to DB
                            List<tbs_ARRunningNo> updatedList
                                = base.UpdateTbs_ARRunningNo(strAROffice, strYear, strPrefix, runningNo, CommonUtil.dsTransData.dtUserData.EmpNo);

                            //Insert Log
                            if (updatedList.Count > 0)
                            {
                                doTransactionLog logData = new doTransactionLog();
                                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                                logData.TableName = TableName.C_TBL_NAME_AR_NO_RUNNO;
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
                    strRunningNo = ARNo.C_AR_NO_MINIMUM;
                    int runningNo = int.Parse(strRunningNo);

                    //2.3.4 Insert running no. to DB
                    List<tbs_ARRunningNo> insertedList
                        = base.InsertTbs_ARRunningNo(strAROffice, strYear, strPrefix, runningNo, CommonUtil.dsTransData.dtUserData.EmpNo);

                    //Insert Log
                    if (insertedList.Count > 0)
                    {
                        doTransactionLog logData = new doTransactionLog();
                        logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                        logData.TableName = TableName.C_TBL_NAME_AR_NO_RUNNO;
                        logData.TableData = CommonUtil.ConvertToXml(insertedList);
                        ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        hand.WriteTransactionLog(logData);
                    }
                }
                #endregion

                //3. Create AR no. [AR office][Year][Prefix][Running no.]
                string strARNo = String.Format("{0}{1}{2}{3}", strAROffice, strYear, strPrefix, strRunningNo);

                //4. return strARNo, strAROffice
                string[] result = new string[] { strARNo, strAROffice };
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get minimum contract code from contract list
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
        /// Retrieve operation office code from datatable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="productType"></param>
        /// <param name="productType2"></param>
        /// <returns></returns>
        private string getOperationOfficeCode(DataTable dt, string productType, string productType2 = null)
        {
            string strOfficeCode = string.Empty;
            string expression = string.Empty;
            if(productType2 == null)
                expression = String.Format("ProductTypeCode = '{0}'", productType);
            else
                expression = String.Format("ProductTypeCode in ('{0}','{1}')", productType, productType2);

            string sortOrder = "ContractCode ASC";
            DataRow[] foundRows = dt.Select(expression, sortOrder);
            if (foundRows.Length > 0)
                strOfficeCode = foundRows[0]["OperationOfficeCode"].ToString();

            return strOfficeCode;
        }

        /// <summary>
        /// Retrieve operation office code from entity list
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="paramProductType"></param>
        /// <returns></returns>
        private string getOperationOfficeCode(List<dtContractsSameSite> ls, params string[] paramProductType)
        {
            string strOfficeCode = string.Empty;

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
        /// Get AR information
        /// </summary>
        /// <param name="conditionByRole"></param>
        /// <returns></returns>
        public List<dtARList> GetARListByRole(doSearchARListByRole conditionByRole)
        {
            DateTime current = DateTime.Now;
            return base.GetARListByRole(conditionByRole.ARStatus, conditionByRole.ARSpecifyPeriod, conditionByRole.ARSpecifyPeriodFrom, conditionByRole.ARSpecifyPeriodTo
                , CommonUtil.dsTransData.dtUserData.EmpNo, conditionByRole.ARRole, current
                , ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT
                , ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION
                , ARRelevant.C_AR_RELEVANT_TYPE_SITE
                , CustRoleType.C_CUST_ROLE_TYPE_REAL_CUST
                , ARRole.C_AR_ROLE_APPROVER
                , ARRole.C_AR_ROLE_AUDITOR
                , ARRole.C_AR_ROLE_REQUESTER
                , MiscType.C_AR_TYPE
                , MiscType.C_AR_STATUS
                , MiscType.C_DEADLINE_TIME_TYPE
                , ARSearchStatus.C_AR_SEARCH_STATUS_COMPLETE
                , ARSearchStatus.C_AR_SEARCH_STATUS_HANDLING
                , ARSearchPeriod.C_AR_SEARCH_PERIOD_REQUEST_DATE
                , ARSearchPeriod.C_AR_SEARCH_PERIOD_APPROVE_DATE
                , ARSearchPeriod.C_AR_SEARCH_PERIOD_DUEDATE
                , ARSearchPeriod.C_AR_SEARCH_PERIOD_LASTACTION_DATE
                , ARStatus.C_AR_STATUS_INSTRUCTED
                , ARStatus.C_AR_STATUS_REJECTED
                , ARStatus.C_AR_STATUS_APPROVED);

        }

        /// <summary>
        /// Get AR data
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<dtARListCTS370> GetARList(doRetrieveARListCondition condition)
        {
            if (((condition == null) || 
                ((condition.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER) && (string.IsNullOrEmpty(condition.CustomerCode))) ||
                ((condition.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE) && (string.IsNullOrEmpty(condition.SiteCode))) ||
                ((condition.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT) && (string.IsNullOrEmpty(condition.ContractCode))) ||
                ((condition.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION) && (string.IsNullOrEmpty(condition.QuotationCode))) ||
                ((condition.ARRelevantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT) && (string.IsNullOrEmpty(condition.ProjectCode)))))
            {
                ApplicationErrorException ex = new ApplicationErrorException();
                ex.AddErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                throw ex;
            }

            return base.GetARList(condition.ARRelevantType
                , condition.CustomerCode
                , condition.SiteCode
                , condition.ContractCode
                , condition.QuotationCode
                , condition.ProjectCode
                , condition.ARType
                , condition.DuedateDeadline
                , condition.ARStatus
                , ARRole.C_AR_ROLE_APPROVER
                , ARRole.C_AR_ROLE_REQUESTER
                , ARRole.C_AR_ROLE_AUDITOR
                , MiscType.C_AR_TYPE
                , MiscType.C_AR_STATUS
                , ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER
                , ARRelevant.C_AR_RELEVANT_TYPE_SITE
                , ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT
                , ARRelevant.C_AR_RELEVANT_TYPE_PROJECT
                , ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION
                , ARSearchStatus.C_AR_SEARCH_STATUS_COMPLETE
                , ARSearchStatus.C_AR_SEARCH_STATUS_HANDLING
                , ARStatus.C_AR_STATUS_INSTRUCTED
                , ARStatus.C_AR_STATUS_REJECTED
                , ARStatus.C_AR_STATUS_APPROVED
                , MiscType.C_DEADLINE_TIME_TYPE
                , CustRoleType.C_CUST_ROLE_TYPE_CONTRACT_TARGET
                , FlagType.C_FLAG_ON);
        }

        /// <summary>
        /// Search AR data
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<dtARList> SearchARList(doSearchARListCondition condition)
        {
            return base.SearchARList(condition.CustomerName, condition.ProjectName, condition.RequestNo, condition.ApproveNo, condition.ARTitle
                , condition.ARType, condition.ARStatusHandling, condition.ARStatusComplete, condition.AROfficeCode, condition.SpecfyPeriod
                , condition.SpecifyPeriodFrom, condition.SpecifyPeriodTo, condition.Requester, condition.Approver, condition.Auditor, condition.ContractTargetPurchaserName
                , condition.SiteName, condition.CustomerGroupName, condition.ContractCode, condition.UserCode, condition.QuotationTargetCode
                , condition.ContractOfficeCode, condition.OperationOfficeCode, condition.ContractStatus, condition.ContractType
                , ARRole.C_AR_ROLE_APPROVER
                , ARRole.C_AR_ROLE_REQUESTER
                , ARRole.C_AR_ROLE_AUDITOR
                , MiscType.C_AR_TYPE
                , MiscType.C_AR_STATUS
                , MiscType.C_DEADLINE_TIME_TYPE
                , ARSearchPeriod.C_AR_SEARCH_PERIOD_REQUEST_DATE
                , ARSearchPeriod.C_AR_SEARCH_PERIOD_APPROVE_DATE
                , ARSearchPeriod.C_AR_SEARCH_PERIOD_DUEDATE
                , ARSearchPeriod.C_AR_SEARCH_PERIOD_LASTACTION_DATE
                , ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER
                , ARRelevant.C_AR_RELEVANT_TYPE_SITE
                , ARRelevant.C_AR_RELEVANT_TYPE_PROJECT
                , ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT
                , ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION
                , CustRoleType.C_CUST_ROLE_TYPE_CONTRACT_TARGET
                , CustRoleType.C_CUST_ROLE_TYPE_REAL_CUST
                , ARStatus.C_AR_STATUS_INSTRUCTED
                , ARStatus.C_AR_STATUS_REJECTED
                , ARStatus.C_AR_STATUS_APPROVED);
        }

        /// <summary>
        /// Get occurring site list
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public List<dtAROccSite> GetOccurringSiteList(string custCode)
        {
            List<dtAROccSite> result = new List<dtAROccSite>();

            result = base.GetAROccurringSite(custCode, ARRelevant.C_AR_RELEVANT_TYPE_SITE);

            return result;
        }

        /// <summary>
        /// Get occurring contract list
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public List<dtAROccContract> GetOccurringContractList(string siteCode)
        {
            List<dtAROccContract> result = new List<dtAROccContract>();

            result = base.GetAROccurringContract(siteCode, ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT);

            return result;
        }

        /// <summary>
        /// Summary AR data
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public List<dtSummaryAR> SummaryAR(DateTime? dateFrom, DateTime? dateTo, DateTime? currentDate)
        {
            return base.SummaryAR(dateFrom, dateTo, currentDate, ARStatus.C_AR_STATUS_INSTRUCTED, ARStatus.C_AR_STATUS_REJECTED, ARStatus.C_AR_STATUS_APPROVED); //ARStatus.C_AR_STATUS_RETURNED_REQUEST); //Modify by Jutarat A. 20120611
        }

        /// <summary>
        /// Get related contract code of AR that related with customer
        /// </summary>
        /// <param name="requestNo"></param>
        /// <returns></returns>
        public List<dtRelatedOfficeChief> GetReleatedContractOfCustAR(string requestNo)
        {
            return this.GetReleatedContractOfCustAR(requestNo, FlagType.C_FLAG_ON);
        }

        /// <summary>
        /// Get chief of related office with customer AR 
        /// </summary>
        /// <param name="RequestNo"></param>
        /// <returns></returns>
        public List<dtOfficeChief> GetSiteRelatedOfficeChief(string RequestNo)
        {
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            var siteCtLst = GetReleatedContractOfCustAR(RequestNo);
            string officeCode = "";

            var lstALOnline = from a in siteCtLst where (a.ProductTypeCode == ProductType.C_PROD_TYPE_AL) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE) orderby a.ContractCode ascending select a;
            if (lstALOnline.Count() > 0)
            {
                officeCode = lstALOnline.ToList()[0].OperationOfficeCode;
            }
            else
            {
                var lstSale = from a in siteCtLst where (a.ProductTypeCode == ProductType.C_PROD_TYPE_SALE) orderby a.ContractCode ascending select a;
                if (lstSale.Count() > 0)
                {
                    officeCode = lstSale.ToList()[0].OperationOfficeCode;
                }
                else
                {
                    var lstMA = from a in siteCtLst where (a.ProductTypeCode == ProductType.C_PROD_TYPE_MA) orderby a.ContractCode ascending select a;
                    if (lstMA.Count() > 0)
                    {
                        officeCode = lstMA.ToList()[0].OperationOfficeCode;
                    }
                    else
                    {
                        var lstBESG = from a in siteCtLst where (a.ProductTypeCode == ProductType.C_PROD_TYPE_SG) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_BE) orderby a.ContractCode ascending select a;
                        if (lstBESG.Count() > 0)
                        {
                            officeCode = lstBESG.ToList()[0].OperationOfficeCode;
                        }
                    }
                }
            }

            var officeList = emphandler.GetOfficeChiefList(officeCode);
            return officeList;
        }

        /// <summary>
        /// Get chief of related office with site AR 
        /// </summary>
        /// <param name="strSiteCode"></param>
        /// <returns></returns>
        public List<dtOfficeChief> GetContractRelatedOfficeChief(string strSiteCode)
        {
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            IViewContractHandler vcontracthandler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
            string officeCode = "";

            var siteLst = vcontracthandler.GetContractsSameSiteList(strSiteCode);

            var lstALOnline = from a in siteLst where (a.ProductTypeCode == ProductType.C_PROD_TYPE_AL) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE) orderby a.ContractCode ascending select a;
            if (lstALOnline.Count() > 0)
            {
                officeCode = lstALOnline.ToList()[0].OperationOfficeCode;
            }
            else
            {
                var lstSale = from a in siteLst where (a.ProductTypeCode == ProductType.C_PROD_TYPE_SALE) orderby a.ContractCode ascending select a;
                if (lstSale.Count() > 0)
                {
                    officeCode = lstSale.ToList()[0].OperationOfficeCode;
                }
                else
                {
                    var lstMA = from a in siteLst where (a.ProductTypeCode == ProductType.C_PROD_TYPE_MA) orderby a.ContractCode ascending select a;
                    if (lstMA.Count() > 0)
                    {
                        officeCode = lstMA.ToList()[0].OperationOfficeCode;
                    }
                    else
                    {
                        var lstBESG = from a in siteLst where (a.ProductTypeCode == ProductType.C_PROD_TYPE_SG) || (a.ProductTypeCode == ProductType.C_PROD_TYPE_BE) orderby a.ContractCode ascending select a;
                        if (lstBESG.Count() > 0)
                        {
                            officeCode = lstBESG.ToList()[0].OperationOfficeCode;
                        }
                    }
                }
            }

            var officeList = emphandler.GetOfficeChiefList(officeCode);
            return officeList;
        }

        /// <summary>
        /// Insert AR data (list)
        /// </summary>
        /// <param name="listAR"></param>
        /// <returns></returns>
        public List<tbt_AR> InsertTbt_AR(List<tbt_AR> listAR)
        {
            try
            {
                if (listAR != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_AR arData in listAR)
                    {
                        arData.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        arData.CreateBy = dsTrans.dtUserData.EmpNo;
                        arData.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        arData.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_AR> res = this.InsertTbt_AR(
                    CommonUtil.ConvertToXml_Store<tbt_AR>(listAR));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_AR,
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
        /// Insert AR data
        /// </summary>
        /// <param name="itemAR"></param>
        /// <returns></returns>
        public tbt_AR InsertTbt_AR(tbt_AR itemAR)
        {
            try
            {
                if (itemAR != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    itemAR.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    itemAR.CreateBy = dsTrans.dtUserData.EmpNo;
                    itemAR.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    itemAR.UpdateBy = dsTrans.dtUserData.EmpNo;
                }

                List<tbt_AR> tmpLst = new List<tbt_AR>();
                tmpLst.Add(itemAR);

                List<tbt_AR> res = this.InsertTbt_AR(
                    CommonUtil.ConvertToXml_Store<tbt_AR>(tmpLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_AR,
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
        /// Insert AR role data (list)
        /// </summary>
        /// <param name="listARRole"></param>
        /// <returns></returns>
        public List<tbt_ARRole> InsertTbt_ARRole(List<tbt_ARRole> listARRole)
        {
            try
            {
                List<tbt_ARRole> res = new List<tbt_ARRole>();

                if ((listARRole != null) && (listARRole.Count > 0))
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_ARRole ARRoleData in listARRole)
                    {
                        ARRoleData.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        ARRoleData.CreateBy = dsTrans.dtUserData.EmpNo;
                        ARRoleData.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        ARRoleData.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }


                    res = this.InsertTbt_ARRole(
                        CommonUtil.ConvertToXml_Store<tbt_ARRole>(listARRole));

                    #region Log

                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_AR_ROLE,
                        TableData = CommonUtil.ConvertToXml(res)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert AR role data
        /// </summary>
        /// <param name="itemARRole"></param>
        /// <returns></returns>
        public tbt_ARRole InsertTbt_ARRole(tbt_ARRole itemARRole)
        {
            try
            {
                if (itemARRole != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    itemARRole.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    itemARRole.CreateBy = dsTrans.dtUserData.EmpNo;
                    itemARRole.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    itemARRole.UpdateBy = dsTrans.dtUserData.EmpNo;
                }

                List<tbt_ARRole> tmpLst = new List<tbt_ARRole>();
                tmpLst.Add(itemARRole);

                List<tbt_ARRole> res = this.InsertTbt_ARRole(
                    CommonUtil.ConvertToXml_Store<tbt_ARRole>(tmpLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_AR_ROLE,
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
        /// Get AR history data (list)
        /// </summary>
        /// <param name="tbt_ARHistory"></param>
        /// <returns></returns>
        public List<tbt_ARHistory> InsertTbt_ARHistory(List<tbt_ARHistory> listARHistory)
        {
            try
            {
                if (listARHistory != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_ARHistory item in listARHistory)
                    {
                        item.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        item.CreateBy = dsTrans.dtUserData.EmpNo;
                        item.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        item.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_ARHistory> res = this.InsertTbt_ARHistory(
                    CommonUtil.ConvertToXml_Store<tbt_ARHistory>(listARHistory));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_AR_HISTORY,
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
        /// Get AR history data
        /// </summary>
        /// <param name="tbt_ARHistory"></param>
        /// <returns></returns>
        public List<tbt_ARHistory> InsertTbt_ARHistory(tbt_ARHistory itemARHistory)
        {
            try
            {
                List<tbt_ARHistory> listARHistory = new List<tbt_ARHistory>();
                listARHistory.Add(itemARHistory);

                if (listARHistory != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_ARHistory item in listARHistory)
                    {
                        item.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        item.CreateBy = dsTrans.dtUserData.EmpNo;
                        item.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        item.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_ARHistory> res = this.InsertTbt_ARHistory(
                    CommonUtil.ConvertToXml_Store<tbt_ARHistory>(listARHistory));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_AR_HISTORY,
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
        /// Get AR history detail data
        /// </summary>
        /// <param name="tbt_ARHistoryDetail"></param>
        /// <returns></returns>
        public List<tbt_ARHistoryDetail> InsertTbt_ARHistoryDetail(List<tbt_ARHistoryDetail> listARHistoryDetail)
        {
            try
            {
                if (listARHistoryDetail != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_ARHistoryDetail item in listARHistoryDetail)
                    {
                        item.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        item.CreateBy = dsTrans.dtUserData.EmpNo;
                        item.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        item.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_ARHistoryDetail> res = this.InsertTbt_ARHistoryDetail(
                    CommonUtil.ConvertToXml_Store<tbt_ARHistoryDetail>(listARHistoryDetail));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_AR_HISTORY_DETAIL,
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
        /// Insert AR fee adjustment data (list)
        /// </summary>
        /// <param name="listARFee"></param>
        /// <returns></returns>
        public List<tbt_ARFeeAdjustment> InsertTbt_ARFeeAdjustment(List<tbt_ARFeeAdjustment> listARFee)
        {
            try
            {
                if (listARFee != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_ARFeeAdjustment ARFeeData in listARFee)
                    {
                        ARFeeData.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        ARFeeData.CreateBy = dsTrans.dtUserData.EmpNo;
                        ARFeeData.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        ARFeeData.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_ARFeeAdjustment> res = this.InsertTbt_ARFeeAdjustment(
                    CommonUtil.ConvertToXml_Store<tbt_ARFeeAdjustment>(listARFee));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_AR_FEE_ADJUSTMENT,
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
        /// Insert AR fee adjustment data
        /// </summary>
        /// <param name="itemARFee"></param>
        /// <returns></returns>
        public tbt_ARFeeAdjustment InsertTbt_ARFeeAdjustment(tbt_ARFeeAdjustment itemARFee)
        {
            try
            {
                if (itemARFee != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    itemARFee.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                    itemARFee.CreateBy = dsTrans.dtUserData.EmpNo;
                    itemARFee.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    itemARFee.UpdateBy = dsTrans.dtUserData.EmpNo;
                }

                List<tbt_ARFeeAdjustment> tmpLst = new List<tbt_ARFeeAdjustment>();
                tmpLst.Add(itemARFee);

                List<tbt_ARFeeAdjustment> res = this.InsertTbt_ARFeeAdjustment(
                    CommonUtil.ConvertToXml_Store<tbt_ARFeeAdjustment>(tmpLst));

                #region Log

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_AR_FEE_ADJUSTMENT,
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
        /// Update AR data (list)
        /// </summary>
        /// <param name="tbt_AR"></param>
        /// <returns></returns>
        public List<tbt_AR> UpdateTbt_AR(List<tbt_AR> tbt_AR)
        {
            string updBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime updDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            List<tbt_AR> oldLst = new List<tbt_AR>();

            if (tbt_AR.Count > 0)
            {
                oldLst = GetTbt_AR(oldLst[0].RequestNo);
            }

            foreach (var item in tbt_AR)
            {
                var oldItem = from a in oldLst where a.RequestNo == item.RequestNo select a;
                if (oldItem.Count() == 1)
                {
                    var tmpOld = oldItem.First();
                    if (DateTime.Compare(tmpOld.UpdateDate.GetValueOrDefault(), item.UpdateDate.GetValueOrDefault()) != 0)
                    {
                        throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON
                            , MessageUtil.MessageList.MSG0019, null).Message);
                    }
                }

                item.UpdateBy = updBy;
                item.UpdateDate = updDate;
            }

            string xml = CommonUtil.ConvertToXml_Store(tbt_AR);

            var lst = base.UpdateTbt_AR(xml);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_AR;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lst;
        }

        /// <summary>
        /// Update AR data
        /// </summary>
        /// <param name="tbt_AR"></param>
        /// <returns></returns>
        public List<tbt_AR> UpdateTbt_AR(tbt_AR tbt_AR)
        {
            string updBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime updDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            var tmplst = new List<tbt_AR>();
            tmplst.Add(tbt_AR);

            List<tbt_AR> oldLst = new List<tbt_AR>();

            if (tmplst.Count > 0)
            {
                oldLst = GetTbt_AR(tbt_AR.RequestNo);
            }

            foreach (var item in tmplst)
            {
                var oldItem = from a in oldLst where a.RequestNo == item.RequestNo select a;
                if (oldItem.Count() == 1)
                {
                    var tmpOld = oldItem.First();
                    if (DateTime.Compare(tmpOld.UpdateDate.GetValueOrDefault(), item.UpdateDate.GetValueOrDefault()) != 0)
                    {
                        throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON
                            , MessageUtil.MessageList.MSG0019, null).Message);
                    }
                }

                item.UpdateBy = updBy;
                item.UpdateDate = updDate;
            }

            string xml = CommonUtil.ConvertToXml_Store(tmplst);

            var lst = base.UpdateTbt_AR(xml);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_AR;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lst;
        }

        /// <summary>
        /// Update AR role data
        /// </summary>
        /// <param name="tbt_ARRole"></param>
        /// <returns></returns>
        public List<tbt_ARRole> UpdateTbt_ARRole(List<tbt_ARRole> tbt_ARRole)
        {
            string updBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime updDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            List<tbt_ARRole> oldRoleLst = new List<tbt_ARRole>();
            List<tbt_ARRole> resLst = new List<tbt_ARRole>();

            if (tbt_ARRole.Count > 0)
            {
                oldRoleLst = GetTbt_ARRole(tbt_ARRole[0].RequestNo);


                foreach (var item in tbt_ARRole)
                {
                    var oldItem = from a in oldRoleLst where a.ARRoleID == item.ARRoleID select a;
                    if (oldItem.Count() == 1)
                    {
                        var tmpOld = oldItem.First();
                        if (DateTime.Compare(tmpOld.UpdateDate.GetValueOrDefault(), item.UpdateDate.GetValueOrDefault()) != 0)
                        {
                            throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON
                                , MessageUtil.MessageList.MSG0019, null).Message);
                        }
                    }

                    item.UpdateBy = updBy;
                    item.UpdateDate = updDate;
                }

                string xml = CommonUtil.ConvertToXml_Store(tbt_ARRole);

                resLst = base.UpdateTbt_ARRole(xml);

                if (resLst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_AR_ROLE;
                    logData.TableData = CommonUtil.ConvertToXml(resLst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }
            }

            return resLst;
        }

        /// <summary>
        /// Update ARFeeAdjustment data
        /// </summary>
        /// <param name="tbt_ARFeeAdjustment"></param>
        /// <returns></returns>
        public List<tbt_ARFeeAdjustment> UpdateTbt_ARFeeAdjustment(List<tbt_ARFeeAdjustment> tbt_ARFeeAdjustment) //Add by Jutarat A. on 03042013
        {
            string updBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            DateTime updDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            List<tbt_ARFeeAdjustment> oldARFeeLst = new List<tbt_ARFeeAdjustment>();
            List<tbt_ARFeeAdjustment> resLst = new List<tbt_ARFeeAdjustment>();

            if (tbt_ARFeeAdjustment.Count > 0)
            {
                oldARFeeLst = GetTbt_ARFeeAdjustment(tbt_ARFeeAdjustment[0].RequestNo);


                foreach (var item in tbt_ARFeeAdjustment)
                {
                    var oldItem = from a in oldARFeeLst where a.RequestNo == item.RequestNo select a;
                    if (oldItem.Count() == 1)
                    {
                        var tmpOld = oldItem.First();
                        if (DateTime.Compare(tmpOld.UpdateDate.GetValueOrDefault(), item.UpdateDate.GetValueOrDefault()) != 0)
                        {
                            throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON
                                , MessageUtil.MessageList.MSG0019, null).Message);
                        }
                    }

                    item.UpdateBy = updBy;
                    item.UpdateDate = updDate;
                }

                string xml = CommonUtil.ConvertToXml_Store(tbt_ARFeeAdjustment);

                resLst = base.UpdateTbt_ARFeeAdjustment(xml);

                if (resLst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_AR_FEE_ADJUSTMENT;
                    logData.TableData = CommonUtil.ConvertToXml(resLst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }
            }

            return resLst;
        }

        /// <summary>
        /// To generate and send email of AR
        /// </summary>
        /// <param name="strMailTo"></param>
        /// <param name="strStatus"></param>
        /// <param name="strLinkEN"></param>
        /// <param name="strLinkLC"></param>
        /// <returns></returns>
        public bool SendAREmail(string strMailTo, string strStatus, string strLinkEN, string strLinkLC, tbt_AR ARData, string strAuditDetailHistory = null) //Add strLinkLC,ARData,strAuditDetailHistory by Jutarat A. on 28092012
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IARHandler arHandler = ServiceContainer.GetService<IARHandler>() as IARHandler;

            bool res = false;

            string strEmailTemplateName = "";
            if (strStatus == ARStatus.C_AR_STATUS_RETURNED_REQUEST)
            {
                strEmailTemplateName = EmailTemplateName.C_EMAIL_TEMPLATE_NAME_AR_RETURNED;
            }
            else if (strStatus == ARStatus.C_AR_STATUS_WAIT_FOR_APPROVAL)
            {
                strEmailTemplateName = EmailTemplateName.C_EMAIL_TEMPLATE_NAME_AR_WAIT_APPROVAL;
            }
            else if (strStatus == ARStatus.C_AR_STATUS_AUDITING)
            {
                strEmailTemplateName = EmailTemplateName.C_EMAIL_TEMPLATE_NAME_AR_AUDITING;
            }
            else if (strStatus == ARStatus.C_AR_STATUS_APPROVED)
            {
                strEmailTemplateName = EmailTemplateName.C_EMAIL_TEMPLATE_NAME_AR_APPROVED;
            }
            else if (strStatus == ARStatus.C_AR_STATUS_INSTRUCTED)
            {
                strEmailTemplateName = EmailTemplateName.C_EMAIL_TEMPLATE_NAME_AR_INSTRUCTED;
            }
            else if (strStatus == ARStatus.C_AR_STATUS_REJECTED)
            {
                strEmailTemplateName = EmailTemplateName.C_EMAIL_TEMPLATE_NAME_AR_REJECTED;
            }

            EmailTemplateUtil mailUtil = new EmailTemplateUtil(strEmailTemplateName);
            doEmailWithURL templateObj = new doEmailWithURL();
            templateObj.ViewURL = strLinkEN;
            templateObj.ViewURLLC = strLinkLC; //Add by Jutarat A. on 28092012

            //Add by Jutarat A. on 11072013
            if (ARData != null)
            {
                CommonUtil comUtil = new CommonUtil();

                string strARRelatedCode = string.Empty;

                if (ARData.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT)
                {
                    strARRelatedCode = comUtil.ConvertContractCode(ARData.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }
                else if (ARData.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER)
                {
                    strARRelatedCode = comUtil.ConvertCustCode(ARData.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }
                else if (ARData.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_SITE)
                {
                    strARRelatedCode = comUtil.ConvertSiteCode(ARData.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }
                else if (ARData.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION)
                {
                    strARRelatedCode = comUtil.ConvertQuotationTargetCode(ARData.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }
                else if (ARData.ARRelavantType == ARRelevant.C_AR_RELEVANT_TYPE_PROJECT)
                {
                    strARRelatedCode = ARData.ProjectCode;
                }

                templateObj.ARRelatedCode = strARRelatedCode;
                templateObj.ARRequestNo = ARData.RequestNo;

                List<doMiscTypeCode> miscTypeCodeList = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_AR_TYPE,
                        ValueCode = ARData.ARType
                    }
                };
                
                List<doMiscTypeCode> miscTypeList = commonhandler.GetMiscTypeCodeList(miscTypeCodeList);
                if (miscTypeList != null && miscTypeList.Count > 0)
                {
                    templateObj.ARTypeEN = miscTypeList[0].ValueDisplayEN;
                    templateObj.ARTypeLC = miscTypeList[0].ValueDisplayLC;
                }

                List<tbs_ARTypeTitle> arTypeDat = arHandler.GetTbs_ARTypeTitle(ARData.ARType, ARData.ARTitleType);
                if (arTypeDat != null && arTypeDat.Count > 0)
                {
                    templateObj.ARTitleEN = arTypeDat[0].ARTitleNameEN;
                    templateObj.ARTitleLC = arTypeDat[0].ARTitleNameLC;
                }

                templateObj.ARSubtitle = ARData.ARSubtitle;
                templateObj.ARPurpose = ARData.ARPurpose;
            }
            //End Add

            //Add by Jutarat A. on 19072013
            if (String.IsNullOrEmpty(strAuditDetailHistory) == false)
                templateObj.AuditDetailHistory = strAuditDetailHistory;
            //End Add

            var mailTemplate = mailUtil.LoadTemplate(templateObj);
            //var mailtemplate = commonhandler.GetTbt_EmailTemplate(strEmailTemplateName);

            doEmailProcess mailMsg = new doEmailProcess();
            mailMsg.MailFrom = CommonUtil.dsTransData.dtUserData.EmailAddress;
            mailMsg.MailFromAlias = null;
            mailMsg.MailTo = strMailTo;
            mailMsg.Subject = mailTemplate.TemplateSubject;
            mailMsg.Message = mailTemplate.TemplateContent;
            mailMsg.IsBodyHtml = true; //Add by Jutarat A. on 15072013

            //Modify by Jutarat A. on 27092012
            //commonhandler.SendMail(mailMsg);
            System.Threading.Thread t = new System.Threading.Thread(SendMail);
            t.Start(mailMsg);
            //End Modify

            res = true;
            
            return res;
        }

        public static void SendMail(object o)
        {
            try
            {
                doEmailProcess obj = o as doEmailProcess;
                if (obj == null)
                    return;

                if (obj != null)
                {
                    ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    comHandler.SendMail(obj);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Get AR data
        /// </summary>
        /// <param name="pRequestNo"></param>
        /// <returns></returns>
        public List<dtAR> GetARData(string pRequestNo)
        {
            //Modify by Jutarat A. on 04042013
            //return base.GetARData(pRequestNo, ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT
            //    , ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER
            //    , ARRelevant.C_AR_RELEVANT_TYPE_SITE
            //    , ARRelevant.C_AR_RELEVANT_TYPE_PROJECT
            //    , MiscType.C_AR_TYPE
            //    , MiscType.C_AR_STATUS);
            return base.GetARData(pRequestNo, ARRelevant.C_AR_RELEVANT_TYPE_CONTRACT
                , ARRelevant.C_AR_RELEVANT_TYPE_CUSTOMER, ARRelevant.C_AR_RELEVANT_TYPE_SITE
                , ARRelevant.C_AR_RELEVANT_TYPE_PROJECT, ARRelevant.C_AR_RELEVANT_TYPE_QUOTATION
                , MiscType.C_AR_TYPE, MiscType.C_AR_STATUS, CustPartType.C_CUST_PART_TYPE_REAL_CUST);
            //End Modify
        }

        /// <summary>
        /// Get AR permission configuration data
        /// </summary>
        /// <param name="strRequestNo"></param>
        /// <returns></returns>
        public doARPermission HasARPermission(string strRequestNo)
        {
            doARPermission res;
            IEmployeeMasterHandler empHandler;
            IOfficeMasterHandler officeHandler;

            try
            {
                empHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                officeHandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                if (string.IsNullOrEmpty(strRequestNo))
                {
                    throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006, null).Message);
                }

                var arRole = GetTbt_ARRole(strRequestNo);
                
                arRole = (from c in arRole where c.EmpNo == CommonUtil.dsTransData.dtUserData.EmpNo select c).ToList<tbt_ARRole>();

                List<string> aPermissionType = new List<string>();

                if (arRole.Count > 0)
                {
                    foreach (var item in arRole)
                    {
                        if (item.ARRoleType == ARRole.C_AR_ROLE_REQUESTER)
                        {
                            aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_REQUESTER);
                        }
                        else if (item.ARRoleType == ARRole.C_AR_ROLE_APPROVER)
                        {
                            aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_APPROVER);
                        }
                        else if (item.ARRoleType == ARRole.C_AR_ROLE_AUDITOR)
                        {
                            aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_AUDITOR);
                        }
                        //Add by Jutarat A. on 31082012
                        else if (item.ARRoleType == ARRole.C_AR_ROLE_RECEIPIENT)
                        {
                            aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_RECEIPIENT);
                        }
                        //End Add

                        if (item.ARRoleType == ARRole.C_AR_ROLE_CHIEF_OF_RELATED_OFFICE)
                        {
                            aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_RELATED_OFFICE);
                        }
                    }
                }

                var ardat = GetARData(strRequestNo);
                if (ardat.Count > 0)
                {
                    var officeLstDat = (from a in CommonUtil.dsTransData.dtOfficeData select a.OfficeCode).ToList();

                    //Comment by Jutarat A. on 24092012
                    //var authorDat = from a in ardat where officeLstDat.Contains(a.AROfficeCode) select a;
                    //if (authorDat.Count() > 0)
                    //{
                    //    aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_HAS_DATA_AUTHORITY);
                    //}
                    //End Comment

                    var projectDat = from a in ardat where a.ProjectManagerEmpNo == CommonUtil.dsTransData.dtUserData.EmpNo select a;
                    if (projectDat.Count() > 0)
                    {
                        aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_PROJECT_MANAGER);
                    }

                    //Add by Jutarat A. on 24092012
                    //Get OfficeCode, BranchCode, HQCode
                    if (String.IsNullOrEmpty(ardat[0].AROfficeCode) == false) //Add by Jutarat A. on 05102012
                    {
                        List<tbm_Office> dtTbt_Office = officeHandler.GetTbm_Office(ardat[0].AROfficeCode);
                        if (dtTbt_Office != null && dtTbt_Office.Count > 0)
                        {
                            List<dtUserBelonging> dt_Belonging = empHandler.getBelongingByEmpNo(CommonUtil.dsTransData.dtUserData.EmpNo);

                            //05 : Person who belong to same office of AR office
                            var belongOffice = (from t in dt_Belonging where t.OfficeCode == dtTbt_Office[0].OfficeCode select t).ToList();
                            if (belongOffice != null && belongOffice.Count > 0)
                            {
                                aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_BELONG_SAME_OFFICE);
                            }

                            //06 : Person who belong to same branch of branch of AR office
                            var belongBranch = (from t in dt_Belonging where t.OfficeCode == dtTbt_Office[0].BranchCode select t).ToList();
                            if (belongBranch != null && belongBranch.Count > 0)
                            {
                                aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_BELONG_SAME_BRANCH);
                            }

                            //07 : Person whe belong headquarter
                            var belongHQ = (from t in dt_Belonging where t.OfficeCode == dtTbt_Office[0].HQCode select t).ToList();
                            if (belongHQ != null && belongHQ.Count > 0)
                            {
                                aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_BELONG_HQ);
                            }
                        }
                    }
                    //End Add
                }

                var mailIncident = empHandler.GetEmailAddressForIncident(FlagType.C_FLAG_OFF);

                if (mailIncident.Count > 0)
                {
                    var matchMail = from a in mailIncident where a.EmpNo == CommonUtil.dsTransData.dtUserData.EmpNo select a;
                    if (matchMail.Count() > 0)
                    {
                        aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_EXECUTIVE);
                    }
                }

                if (!String.IsNullOrEmpty(ardat[0].ARDepartmentCode))
                {
                    List<dtAROfficeChief> officeLst = GetAROfficeChief(strRequestNo, false);
                    if (officeLst.Count > 0)
                    {
                        if (officeLst.Where(x => x.EmpNo == CommonUtil.dsTransData.dtUserData.EmpNo).Count() > 0)
                        {
                            aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_OFFICE_CHIEF);
                        }
                    }
                }
                else
                {
                    List<dtARDepartmentChief> departmentLst = GetARDepartmentChief(strRequestNo, false);
                    if (departmentLst.Count > 0)
                    {
                        if (departmentLst.Where(x => x.EmpNo == CommonUtil.dsTransData.dtUserData.EmpNo).Count() > 0)
                        {
                            aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_OFFICE_CHIEF);
                        }
                    }
                }

                aPermissionType.Add(ARPermissionType.C_AR_PERMISSION_TYPE_GENERAL);

                string permissionType = "";
                foreach (string item in aPermissionType)
                {
                    if (permissionType.Length > 0)
                        permissionType += ", ";

                    permissionType += String.Format("'{0}'", item);
                }

                var permitConfig = GetTbs_ARPermissionConfiguration(permissionType);

                var approveFlag = from a in permitConfig where a.AssignApproverFlag.GetValueOrDefault() select a;
                var auditFlag = from a in permitConfig where a.AssignAuditorFlag.GetValueOrDefault() select a;
                var viewFlag = from a in permitConfig where a.ViewARDetailFlag.GetValueOrDefault() select a;
                var editFlag = from a in permitConfig where a.EditARDetailFlag.GetValueOrDefault() select a;

                res = new doARPermission()
                {
                    AssignApproverFlag = approveFlag.Count() > 0,
                    AssignAuditorFlag = auditFlag.Count() > 0,
                    ViewARDetailFlag = viewFlag.Count() > 0,
                    EditARDetailFlag = editFlag.Count() > 0,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        /// <summary>
        /// Get AR role data
        /// </summary>
        /// <param name="strRequestNo"></param>
        /// <returns></returns>
        public List<dtARRole> GetARRoleData(string strRequestNo)
        {
            //return base.GetARRoleData(strRequestNo, MiscType.C_AR_ROLE, IncidentRole.C_INCIDENT_ROLE_CHIEF_OF_RELATED_OFFICE);
            return base.GetARRoleData(strRequestNo, MiscType.C_AR_ROLE, ARRole.C_AR_ROLE_CHIEF_OF_RELATED_OFFICE); //Modify by Jutarat A. on 03092012
        }

        /// <summary>
        /// Get AR detail data
        /// </summary>
        /// <param name="strRequestNo"></param>
        /// <returns></returns>
        public dsARDetail GetARDetail(string strRequestNo)
        {
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            dsARDetail res = new dsARDetail();

            if (string.IsNullOrEmpty(strRequestNo))
            {
                throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006).Message);
            }

            var arDat = GetARData(strRequestNo);
            if (arDat.Count == 1)
            {
                CommonUtil.MappingObjectLanguage(arDat[0]);
                res.dtAR = arDat[0];
            }

            var arRoleDat = GetARRoleData(strRequestNo);
            CommonUtil.MappingObjectLanguage<dtARRole>(arRoleDat);
            res.dtARRole = arRoleDat;

            var arHisDat = GetTbt_ARHistory(strRequestNo);
            res.tbt_ARHistory = arHisDat;

            var empListDat = new List<dtEmployeeOffice>();
            foreach (var item in arHisDat)
            {
                empListDat.AddRange(emphandler.GetEmployeeOffice(item.CreateBy, true));
            }

            res.dtEmployeeOffice = empListDat;

            var arHisDetailDat = new List<tbt_ARHistoryDetail>();
            foreach (var item in res.tbt_ARHistory)
            {
                var tmpArHisDetailDat = GetTbt_ARHistoryDetail(item.ARHistoryID);
                arHisDetailDat.AddRange(tmpArHisDetailDat);
            }

            res.tbt_ARHistoryDetail = arHisDetailDat;

            var arFeeDat = GetTbt_ARFeeAdjustment(strRequestNo);
            if (arFeeDat.Count == 1)
            {
                CommonUtil.MappingObjectLanguage(arDat[0]);
                res.tbt_ARFeeAdjustment = arFeeDat[0];
            }

            return res;
        }

        //public List<dtIncidentRole> GetIncidentRoleData(int IncidentID)
        //{
        //    return base.GetIncidentRoleData(IncidentID, MiscType.C_INCIDENT_ROLE, IncidentRole.C_INCIDENT_ROLE_CHIEF_OF_RELATED_OFFICE);
        //}

        /// <summary>
        /// Delete AR role by id
        /// </summary>
        /// <param name="ARRoleID"></param>
        /// <returns></returns>
        public bool DeleteTbt_ARRole(int ARRoleID)
        {
            List<tbt_ARRole> lst = base.DeleteTbt_ARRole(ARRoleID);

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                logData.TableName = TableName.C_TBL_NAME_AR_ROLE;
                logData.TableData = CommonUtil.ConvertToXml(lst);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return (lst.Count == 1);
        }

        /// <summary>
        /// Update AR detail data
        /// </summary>
        /// <param name="arDat"></param>
        /// <param name="resARStatus"></param>
        /// <returns></returns>
        public bool UpdateARDetail(dsARDetailIn arDat, out string arStatus)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool res = false;
            arStatus = "";

            var oldAR = GetARDetail(arDat.tbt_AR.RequestNo);

            if ((oldAR == null) || (DateTime.Compare(oldAR.dtAR.UpdateDate.GetValueOrDefault(), arDat.tbt_AR.UpdateDate.GetValueOrDefault()) != 0))
            {
                throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON
                    , MessageUtil.MessageList.MSG0019, null).Message);
            }

            var updAR = UpdateTbt_AR(arDat.tbt_AR);
            arStatus = updAR[0].ARStatus;

            var updARFeeAdjustment = UpdateTbt_ARFeeAdjustment(arDat.tbt_ARFeeAdjustment); //Add by Jutarat A. on 03042013

            foreach (int item in arDat.tbt_ARRoleDelete)
            {
                DeleteTbt_ARRole(item);
            }

            var updARRole_Add = InsertTbt_ARRole(arDat.tbt_ARRoleAdd);
            var updARRole_Edit = UpdateTbt_ARRole(arDat.tbt_ARRoleEdit);


            var updHis = InsertTbt_ARHistory(arDat.tbt_ARHistory);
            foreach (var item in arDat.tbt_ARHistoryDetail)
            {
                item.ARHistoryID = updHis[0].ARHistoryID;
            }

            var updHisDetail = InsertTbt_ARHistoryDetail(arDat.tbt_ARHistoryDetail);

            // Update Attachment
            commonhandler.UpdateFlagAttachFile(AttachmentModule.AR, updAR[0].RequestNo.ToString(), updAR[0].RequestNo.ToString());

            res = true;

            return res;
        }

        /// <summary>
        /// Update contract code to AR data when approve contract
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public bool UpdateContractCode(tbt_AR cond)
        {
            try
            {
                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                List<tbt_AR> lst = this.UpdateContractCode(
                cond.QuotationTargetCode,
                cond.ContractCode,
                dsTrans.dtOperationData.ProcessDateTime,
                dsTrans.dtUserData.EmpNo);

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_AR;
                    logData.TableData = CommonUtil.ConvertToXml(lst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
