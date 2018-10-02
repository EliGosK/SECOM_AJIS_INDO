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
using CSI.WindsorHelper;
using System.IO;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class ContractDocumentHandler : BizCTDataEntities, IContractDocumentHandler
    {
        #region Insert
        /// <summary>
        /// To generate contract document occurrence
        /// </summary>
        /// <param name="strCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public string GenerateDocOCC(string strCode, string strOCC)
        {
            try
            {
                //1.	Get contract document occurrence
                List<string> contractDocOCC = base.GetContractDocOCC(strCode, strOCC);

                //2.	Check result data
                string strContractDocOCC = string.Empty;
                if (contractDocOCC == null || contractDocOCC[0] == null || contractDocOCC[0] == string.Empty)
                {
                    strContractDocOCC = "01";
                }
                else if (contractDocOCC[0] == "99")
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3025);
                }
                else
                {
                    strContractDocOCC = (Int32.Parse(contractDocOCC[0]) + 1).ToString();
                    strContractDocOCC = strContractDocOCC.PadLeft(2, '0');
                }

                //3.	Return strContractDocOCC
                return strContractDocOCC;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert contract document
        /// </summary>
        /// <param name="docLst"></param>
        /// <returns></returns>
        public List<tbt_ContractDocument> InsertTbt_ContractDocument(List<tbt_ContractDocument> docLst)
        {
            try
            {
                if (docLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_ContractDocument doc in docLst)
                    {
                        doc.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        doc.CreateBy = dsTrans.dtUserData.EmpNo;
                        doc.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        doc.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                //Modify by Jutarat A. on 18122012
                //return this.InsertTbt_ContractDocument(CommonUtil.ConvertToXml_Store<tbt_ContractDocument>(docLst));
                List<tbt_ContractDocument> insertList = this.InsertTbt_ContractDocument(CommonUtil.ConvertToXml_Store<tbt_ContractDocument>(docLst));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_CONTRACT_DOC;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
                //End Modify
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Insert document contract report
        /// </summary>
        /// <param name="docLst"></param>
        /// <returns></returns>
        public List<tbt_DocContractReport> InsertTbt_DocContractReport(List<tbt_DocContractReport> docLst)
        {
            try
            {
                if (docLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_DocContractReport doc in docLst)
                    {
                        doc.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        doc.CreateBy = dsTrans.dtUserData.EmpNo;
                        doc.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        doc.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                //Modify by Jutarat A. on 18122012
                //return this.InsertTbt_DocContractReport(CommonUtil.ConvertToXml_Store<tbt_DocContractReport>(docLst));
                List<tbt_DocContractReport> insertList = this.InsertTbt_DocContractReport(CommonUtil.ConvertToXml_Store<tbt_DocContractReport>(docLst));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DOC_CONTRACT_RPT;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
                //End Modify
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Create report for contract document
        /// </summary>
        /// <param name="iDocID"></param>
        /// <param name="strDocNo"></param>
        /// <param name="strDocumentCode"></param>
        /// <returns></returns>
        public Stream CreateContractReport(int? iDocID, string strDocNo, string strDocumentCode)
        {
            Stream stream = null;
            IContractDocumentHandler contractDocHandler;
            IDocumentHandler docHandler;

            doDocumentDataGenerate doDocument;
            ContractReportData contractData = null;
            List<ReportParameterObject> listSubReportDataSource;
            List<ReportParameterObject> listMainReportParam;
            List<ReportParameterObject> listSubReportParam;

            doDocumentDataGenerate doDocumentCover;
            ContractReportData contractDataCover = null;
            List<ReportParameterObject> listSubReportDataSourceCover;
            List<ReportParameterObject> listMainReportParamCover;
            List<ReportParameterObject> listSubReportParamCover;

            object rptList = null;
            object rptListDetail = null;
            object rptListCover = null;
            object rptListCoverDetail = null;

            try
            {
                //Check mandatory data
                if (iDocID == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                //Get contract document data
                contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                listSubReportDataSource = new List<ReportParameterObject>();
                listMainReportParam = new List<ReportParameterObject>();
                listSubReportParam = new List<ReportParameterObject>();

                bool bHasCoverLetter = false;
                if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH)  //CTR010, CTR011
                {
                    rptList = contractDocHandler.GetRptContractReportData(iDocID.Value);  //List<RPTContractReportDo>
                    if (rptList != null && ((List<RPTContractReportDo>)rptList).Count > 0)
                    {
                        ((List<RPTContractReportDo>)rptList)[0].PrintDate = CommonUtil.TextDate(CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        contractData = CommonUtil.CloneObject<RPTContractReportDo, ContractReportData>(((List<RPTContractReportDo>)rptList)[0]);
                        bHasCoverLetter = true;

                        string strSubReportName = string.Empty;
                        if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN)
                            strSubReportName = "CTR010_1";
                        else
                            strSubReportName = "CTR011_1";

                        listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = strSubReportName, Value = rptList });

                        listSubReportParam.Add(new ReportParameterObject() { ParameterName = "FlagOn", Value = FlagType.C_FLAG_ON, SubReportName = strSubReportName });
                    }
                }
                else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_NOTICE)  //CTR020
                {
                    rptList = contractDocHandler.GetRptChangeNoticeData(iDocID.Value);  //List<RPTChangeNoticeDo>
                    if (rptList != null && ((List<RPTChangeNoticeDo>)rptList).Count > 0)
                    {
                        ((List<RPTChangeNoticeDo>)rptList)[0].PrintDate = CommonUtil.TextDate(CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        contractData = CommonUtil.CloneObject<RPTChangeNoticeDo, ContractReportData>(((List<RPTChangeNoticeDo>)rptList)[0]);
                        bHasCoverLetter = true;
                    }
                }
                else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_MEMO)  //CTR030
                {
                    rptList = contractDocHandler.GetRptChangeMemoData(iDocID.Value);  //List<RPTChangeMemoDo> 
                    
                    if (rptList != null && ((List<RPTChangeMemoDo>)rptList).Count > 0)
                    {
                        ((List<RPTChangeMemoDo>)rptList)[0].PrintDate = CommonUtil.TextDate(CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        contractData = CommonUtil.CloneObject<RPTChangeMemoDo, ContractReportData>(((List<RPTChangeMemoDo>)rptList)[0]);
                        bHasCoverLetter = true;
                    }
                }
                else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_START_OPER_CONFIRM_LETTER)  //CTR040
                {
                    rptList = contractDocHandler.GetRptStartResumeMemoData(iDocID.Value);  //List<RPTStartResumeMemoDo> 
                    if (rptList != null && ((List<RPTStartResumeMemoDo>)rptList).Count > 0)
                    {
                        ((List<RPTStartResumeMemoDo>)rptList)[0].PrintDate = CommonUtil.TextDate(CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        contractData = CommonUtil.CloneObject<RPTStartResumeMemoDo, ContractReportData>(((List<RPTStartResumeMemoDo>)rptList)[0]);
                        bHasCoverLetter = true;
                    }
                }
                else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO)  //CTR050
                {
                    rptList = contractDocHandler.GetRptConfirmCurrentInstrumentMemoData(iDocID.Value);  //List<RPTConfirmCurrInstMemoDo> 
                    if (rptList != null && ((List<RPTConfirmCurrInstMemoDo>)rptList).Count > 0)
                    {
                        ((List<RPTConfirmCurrInstMemoDo>)rptList)[0].PrintDate = CommonUtil.TextDate(CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        contractData = CommonUtil.CloneObject<RPTConfirmCurrInstMemoDo, ContractReportData>(((List<RPTConfirmCurrInstMemoDo>)rptList)[0]);
                        bHasCoverLetter = true;
                    }
                }
                else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO)  //CTR060
                {
                    rptList = contractDocHandler.GetRptCancelContractMemoData(iDocID.Value);  //List<RPTCancelContractMemoDo> 
                    rptListDetail = contractDocHandler.GetRptCancelContractMemoDetail(iDocID.Value);  //List<RPTCancelContractMemoDetailDo> 
                    if (rptList != null && ((List<RPTCancelContractMemoDo>)rptList).Count > 0)
                    {
                        ((List<RPTCancelContractMemoDo>)rptList)[0].PrintDate = CommonUtil.TextDate(CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        contractData = CommonUtil.CloneObject<RPTCancelContractMemoDo, ContractReportData>(((List<RPTCancelContractMemoDo>)rptList)[0]);
                        bHasCoverLetter = true;

                        listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "CTR060_1", Value = rptListDetail });

                        listSubReportParam.Add(new ReportParameterObject() { ParameterName = "AutoBillingTypeNone", Value = AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_NONE, SubReportName = "CTR060_1" });
                        listSubReportParam.Add(new ReportParameterObject() { ParameterName = "AutoBillingTypeAll", Value = AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_ALL, SubReportName = "CTR060_1" });
                        listSubReportParam.Add(new ReportParameterObject() { ParameterName = "AutoBillingTypePartial", Value = AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_PARTIAL, SubReportName = "CTR060_1" });
                        listSubReportParam.Add(new ReportParameterObject() { ParameterName = "BankBillingTypeNone", Value = BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_NONE, SubReportName = "CTR060_1" });
                        listSubReportParam.Add(new ReportParameterObject() { ParameterName = "BankBillingTypeAll", Value = BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_ALL, SubReportName = "CTR060_1" });
                        listSubReportParam.Add(new ReportParameterObject() { ParameterName = "BankBillingTypePartial", Value = BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_PARTIAL, SubReportName = "CTR060_1" });
                    }
                }
                else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION)  //CTR080
                {
                    rptList = contractDocHandler.GetRptCancelContractMemoData(iDocID.Value);  //List<RPTCancelContractMemoDo> 
                    rptListDetail = contractDocHandler.GetRptCancelContractMemoDetail(iDocID.Value);  //List<RPTCancelContractMemoDetailDo> 
                    if (rptList != null && ((List<RPTCancelContractMemoDo>)rptList).Count > 0)
                    {
                        ((List<RPTCancelContractMemoDo>)rptList)[0].PrintDate = CommonUtil.TextDate(CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        contractData = CommonUtil.CloneObject<RPTCancelContractMemoDo, ContractReportData>(((List<RPTCancelContractMemoDo>)rptList)[0]);

                        bool isShowDefaultData = false;
                        if (rptListDetail == null || ((List<RPTCancelContractMemoDetailDo>)rptListDetail).Count == 0)
                        {
                            rptListDetail = GetDefaultCancelContractMemoDetailData();
                            isShowDefaultData = true;
                        }

                        listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "CTR080_1", Value = rptListDetail });

                        listSubReportParam.Add(new ReportParameterObject() { ParameterName = "ShowDefaultData", Value = isShowDefaultData, SubReportName = "CTR080_1" });
                    }
                }
                else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO)  //CTR070
                {
                    rptList = contractDocHandler.GetRptChangeFeeMemoData(iDocID.Value);  //List<RPTChangeFeeMemoDo> 
                    if (rptList != null && ((List<RPTChangeFeeMemoDo>)rptList).Count > 0)
                    {
                        ((List<RPTChangeFeeMemoDo>)rptList)[0].PrintDate = CommonUtil.TextDate(CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        contractData = CommonUtil.CloneObject<RPTChangeFeeMemoDo, ContractReportData>(((List<RPTChangeFeeMemoDo>)rptList)[0]);
                        bHasCoverLetter = true;
                    }
                }
                else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_COVER_LETTER)  //CTR090
                {
                    rptList = contractDocHandler.GetRptCoverLetterData(iDocID.Value);  //List<RPTCoverLetterDo> 
                    rptListDetail = contractDocHandler.GetRptInstrumentDetailData(iDocID.Value);  //List<RPTInstrumentDetailDo>
                    if (rptList != null && ((List<RPTCoverLetterDo>)rptList).Count > 0)
                    {
                        ((List<RPTCoverLetterDo>)rptList)[0].IssuedDate = CommonUtil.TextDate(CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        contractData = CommonUtil.CloneObject<RPTCoverLetterDo, ContractReportData>(((List<RPTCoverLetterDo>)rptList)[0]);

                        listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "CTR090_1", Value = rptListDetail });
                        listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "CTR090_2", Value = rptListDetail });
                        listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "CTR090_3", Value = rptListDetail });

                        listMainReportParam.Add(new ReportParameterObject() { ParameterName = "FlagOn", Value = FlagType.C_FLAG_ON });
                        listMainReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrument", Value = (rptListCoverDetail != null && ((List<RPTInstrumentDetailDo>)rptListCoverDetail).Count > 0) });
                    }
                }

                listSubReportDataSourceCover = new List<ReportParameterObject>();
                listMainReportParamCover = new List<ReportParameterObject>();
                listSubReportParamCover = new List<ReportParameterObject>();

                if (bHasCoverLetter == true)
                {
                    rptListCover = contractDocHandler.GetRptCoverLetterData(iDocID.Value);  //List<RPTCoverLetterDo> 
                    rptListCoverDetail = contractDocHandler.GetRptInstrumentDetailData(iDocID.Value);  //List<RPTInstrumentDetailDo> 
                    if (rptListCover != null && ((List<RPTCoverLetterDo>)rptListCover).Count > 0)
                    {
                        ((List<RPTCoverLetterDo>)rptListCover)[0].IssuedDate = CommonUtil.TextDate(CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        contractDataCover = CommonUtil.CloneObject<RPTCoverLetterDo, ContractReportData>(((List<RPTCoverLetterDo>)rptListCover)[0]);

                        listSubReportDataSourceCover.Add(new ReportParameterObject() { SubReportName = "CTR090_1", Value = rptListCoverDetail });
                        listSubReportDataSourceCover.Add(new ReportParameterObject() { SubReportName = "CTR090_2", Value = rptListCoverDetail });
                        listSubReportDataSourceCover.Add(new ReportParameterObject() { SubReportName = "CTR090_3", Value = rptListCoverDetail });

                        listMainReportParamCover.Add(new ReportParameterObject() { ParameterName = "FlagOn", Value = FlagType.C_FLAG_ON });
                        listMainReportParamCover.Add(new ReportParameterObject() { ParameterName = "ShowInstrument", Value = (rptListCoverDetail != null && ((List<RPTInstrumentDetailDo>)rptListCoverDetail).Count > 0) });

                    }
                }

                if (contractData != null)
                {
                    //Prepare doDocumentGenerated
                    doDocument = new doDocumentDataGenerate();
                    doDocument.DocumentNo = strDocNo;
                    doDocument.DocumentCode = strDocumentCode; //contractData.DocumentCode;
                    doDocument.DocumentData = rptList;
                    doDocument.OtherKey.ContractCode = contractData.ContractCode;
                    doDocument.OtherKey.ContractOCC = contractData.OCC; //contractData.ContractDocOCC;
                    doDocument.OtherKey.QuotationTargetCode = contractData.QuotationTargetCode;
                    doDocument.OtherKey.Alphabet = contractData.Alphabet;
                    doDocument.OtherKey.ContractOffice = contractData.ContractOfficeCode;
                    doDocument.OtherKey.OperationOffice = contractData.OperationOfficeCode;
                    doDocument.SubReportDataSource = listSubReportDataSource;
                    doDocument.MainReportParam = listMainReportParam;
                    doDocument.SubReportParam = listSubReportParam;
                    doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                    if (bHasCoverLetter == false)
                    { 
                        stream = docHandler.GenerateDocument(doDocument);                    
                    }
                    else
                    {
                        //Prepare doDocumentGenerated CoverLetter
                        doDocumentCover = new doDocumentDataGenerate();
                        doDocumentCover.DocumentNo = strDocNo;
                        doDocumentCover.DocumentCode = DocumentCode.C_DOCUMENT_CODE_COVER_LETTER; //contractDataCover.DocumentCode; 
                        doDocumentCover.DocumentData = rptListCover;
                        doDocumentCover.OtherKey.ContractCode = contractDataCover.ContractCode;
                        doDocumentCover.OtherKey.ContractOCC = contractDataCover.OCC; //contractDataCover.ContractDocOCC;
                        doDocumentCover.OtherKey.QuotationTargetCode = contractDataCover.QuotationTargetCode;
                        doDocumentCover.OtherKey.Alphabet = contractDataCover.Alphabet;
                        doDocumentCover.OtherKey.ContractOffice = contractDataCover.ContractOfficeCode;
                        doDocumentCover.OtherKey.OperationOffice = contractDataCover.OperationOfficeCode;
                        doDocumentCover.SubReportDataSource = listSubReportDataSourceCover;
                        doDocumentCover.MainReportParam = listMainReportParamCover;
                        doDocumentCover.SubReportParam = listSubReportParamCover;
                        doDocumentCover.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doDocumentCover.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                        stream = docHandler.GenerateDocument(doDocument, doDocumentCover);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return stream;
        }

        private List<RPTCancelContractMemoDetailDo> GetDefaultCancelContractMemoDetailData()
        {
            List<RPTCancelContractMemoDetailDo> cancelContractMemoDetailList = new List<RPTCancelContractMemoDetailDo>();

            RPTCancelContractMemoDetailDo cancelContractMemoDetail = new RPTCancelContractMemoDetailDo();
            cancelContractMemoDetail.BillingType = "";
            cancelContractMemoDetail.BillingTypeName = "";
            cancelContractMemoDetail.HandlingType = "";
            cancelContractMemoDetail.HandlingTypeName = "";
            cancelContractMemoDetail.StartPeriodDate = "";
            cancelContractMemoDetail.EndPeriodDate = "";
            cancelContractMemoDetail.FeeAmount = 0;
            cancelContractMemoDetail.TaxAmount = 0;
            cancelContractMemoDetail.NormalFeeAmount = 0;
            cancelContractMemoDetail.ContractCode_CounterBalance = "";
            cancelContractMemoDetail.Remark = "";
            cancelContractMemoDetailList.Add(cancelContractMemoDetail);

            return cancelContractMemoDetailList;
        }

        #endregion

        #region Update
        /// <summary>
        /// Update contract document
        /// </summary>
        /// <param name="docLst"></param>
        /// <returns></returns>
        public List<tbt_ContractDocument> UpdateTbt_ContractDocument(List<tbt_ContractDocument> docLst)
        {
            try
            {
                if (docLst != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_ContractDocument doc in docLst)
                    {
                        doc.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        doc.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                //Modify by Jutarat A. on 18122012
                //return UpdateTbt_ContractDocument(CommonUtil.ConvertToXml_Store<tbt_ContractDocument>(docLst));
                List<tbt_ContractDocument> updatedList = UpdateTbt_ContractDocument(CommonUtil.ConvertToXml_Store<tbt_ContractDocument>(docLst));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_CONTRACT_DOC;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;
                //End Modify
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Update document status
        /// </summary>
        /// <param name="strDocStatus"></param>
        /// <param name="iDocID"></param>
        /// <param name="dLastUpdateDate"></param>
        /// <returns></returns>
        public List<tbt_ContractDocument> UpdateDocumentStatus(string strDocStatus, int? iDocID, DateTime? dLastUpdateDate)
        {
            try
            {
                //Check mandatory data
                if (iDocID == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                //Check last update datetime 
                List<tbt_ContractDocument> dtTbt_ContractDocument = base.GetTbt_ContractDocument(iDocID);
                if (dtTbt_ContractDocument != null && dtTbt_ContractDocument.Count > 0)
                {
                    if (dtTbt_ContractDocument[0].UpdateDate != dLastUpdateDate)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);// Record already updated by another user.
                    }
                }

                List<tbt_ContractDocument> updatedList = base.UpdateDocumentStatus(strDocStatus, iDocID, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, CommonUtil.dsTransData.dtUserData.EmpNo);

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_CONTRACT_DOC;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Select

        #region GetRptChangeNoticeData

        /// <summary>
        /// Get change notice report data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public List<RPTChangeNoticeDo> GetRptChangeNoticeData(int iDocID)
        {
            try
            {
                List<RPTChangeNoticeDo> rptChangeNoticeList = base.GetRptChangeNoticeData(iDocID, FlagType.C_FLAG_ON);
                if (rptChangeNoticeList == null)
                    rptChangeNoticeList = new List<RPTChangeNoticeDo>();

                //Test
                //List<RPTChangeNoticeDo> rptChangeNoticeList;
                //rptChangeNoticeList = GetRptChangeNoticeMockup();

                return rptChangeNoticeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get change notice report mockup
        /// </summary>
        /// <returns></returns>
        private List<RPTChangeNoticeDo> GetRptChangeNoticeMockup()
        {
            List<RPTChangeNoticeDo> rptChangeNoticeList = new List<RPTChangeNoticeDo>();

            RPTChangeNoticeDo rptChangeNotice = new RPTChangeNoticeDo();
            rptChangeNotice.ContractTargetNameLC = "บริษัท ซิลเวอร์เจมส์การ์เด้น จำกัด";
            rptChangeNotice.PrintDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptChangeNotice.SiteNameLC = "อาคารซิลเวอร์เจมส์การ์เด้น";
            rptChangeNotice.SiteAddressLC = "10 ถนนเจริญกรุง ซอย 41 แขวงสี่พระยา เขตบางรัก กรุงเทพ 10500";
            rptChangeNotice.RealCustomerNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            rptChangeNotice.ContractCode = "N0000000009";
            rptChangeNotice.EffectiveDate = DateTime.Now.AddDays(5).ToString("dd-MMM-yyyy");
            rptChangeNotice.ChangeContent = "การก่อสร้างจะเริ่มในวันที่ 16-Mar-2010";
            rptChangeNotice.EmployeeName = "KAIJI ISHISAKI";
            rptChangeNotice.EmployeePosition = "DIRECTOR";
            rptChangeNotice.ImageSignaturePath = "signature_test2.png";
            rptChangeNotice.DocumentVersion = "Dec.2011";
            rptChangeNotice.DocNo = "N0000000009-0001-01";
            rptChangeNotice.DocID = 1234;
            rptChangeNotice.DocumentCode = "CTR020";
            rptChangeNotice.QuotationTargetCode = "";
            rptChangeNotice.Alphabet = "";
            rptChangeNotice.OCC = "0001";
            rptChangeNotice.ContractDocOCC = "01";
            rptChangeNotice.ContractOfficeCode = "";
            rptChangeNotice.OperationOfficeCode = "";
            rptChangeNotice.DocumentNameEN = "Change notice";
            rptChangeNotice.DocumentNameLC = "(ไทย) Change notice";

            rptChangeNoticeList.Add(rptChangeNotice);

            return rptChangeNoticeList;
        }

        #endregion

        #region GetRptChangeMemoData

        /// <summary>
        /// Get change fee memorandum data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public List<RPTChangeMemoDo> GetRptChangeMemoData(int iDocID)
        {
            try
            {
                List<RPTChangeMemoDo> rptChangeMemoList = base.GetRptChangeMemoData(iDocID, FlagType.C_FLAG_ON);
                if (rptChangeMemoList == null)
                    rptChangeMemoList = new List<RPTChangeMemoDo>();

                //Test
                //List<RPTChangeMemoDo> rptChangeMemoList;
                //rptChangeMemoList = GetRptChangeMemoMockup();

                return rptChangeMemoList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get change fee memorandum mockup
        /// </summary>
        /// <returns></returns>
        private List<RPTChangeMemoDo> GetRptChangeMemoMockup()
        {
            List<RPTChangeMemoDo> rptChangeMemoList = new List<RPTChangeMemoDo>();

            RPTChangeMemoDo rptChangeMemo = new RPTChangeMemoDo();
            rptChangeMemo.ContractTargetNameLC = "บริษัท ซิลเวอร์เจมส์การ์เด้น จำกัด";
            rptChangeMemo.PrintDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptChangeMemo.RealCustomerNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            rptChangeMemo.SiteNameLC = "อาคารซิลเวอร์เจมส์การ์เด้น";
            rptChangeMemo.SiteAddressLC = "10 ถนนเจริญกรุง ซอย 41 แขวงสี่พระยา เขตบางรัก กรุงเทพ 10500";
            rptChangeMemo.ContractCode = "N0000000009";
            rptChangeMemo.EffectiveDate = DateTime.Now.AddDays(5).ToString("dd-MMM-yyyy");
            rptChangeMemo.ChangeContent = "การก่อสร้างจะเริ่มในวันที่ 16-Mar-2010";
            rptChangeMemo.OldContractFee = 4500;
            rptChangeMemo.NewContractFee = 4000;
            rptChangeMemo.CustomerSignatureName = "บริษัท ซิลเวอร์เจมส์การ์เด้น จำกัด";
            rptChangeMemo.EmployeeName = "KAIJI ISHISAKI";
            rptChangeMemo.EmployeePosition = "DIRECTOR";
            rptChangeMemo.ImageSignaturePath = "signature_test2.png";
            rptChangeMemo.DocumentVersion = "Dec.2011";
            rptChangeMemo.DocNo = "N0000000009-0001-01";
            rptChangeMemo.DocID = 1234;
            rptChangeMemo.DocumentCode = "CTR030";
            rptChangeMemo.QuotationTargetCode = "";
            rptChangeMemo.Alphabet = "";
            rptChangeMemo.OCC = "0001";
            rptChangeMemo.ContractDocOCC = "01";
            rptChangeMemo.ContractOfficeCode = "";
            rptChangeMemo.OperationOfficeCode = "";
            rptChangeMemo.DocumentNameEN = "Change memorandum";
            rptChangeMemo.DocumentNameLC = "(ไทย) Change memorandum";

            rptChangeMemoList.Add(rptChangeMemo);

            return rptChangeMemoList;
        }

        #endregion

        #region GetRptContractReportData

        /// <summary>
        /// Get contract report data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public List<RPTContractReportDo> GetRptContractReportData(int iDocID)
        {
            try
            {
                List<RPTContractReportDo> rptContractReportList = base.GetRptContractReportData(iDocID, FlagType.C_FLAG_ON, MiscType.C_PAYMENT_METHOD_FOR_CUSTOMER_DISP, MiscType.C_BILLING_TIMING_FOR_CUSTOMER_DISP);
                if (rptContractReportList == null)
                    rptContractReportList = new List<RPTContractReportDo>();

                //Test
                //List<RPTContractReportDo> rptContractReportList;
                //rptContractReportList = GetRptContractReportMockup();

                return rptContractReportList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get contract report mockup
        /// </summary>
        /// <returns></returns>
        private List<RPTContractReportDo> GetRptContractReportMockup()
        {
            List<RPTContractReportDo> rptContractReportList = new List<RPTContractReportDo>();

            RPTContractReportDo rptContractReport = new RPTContractReportDo();
            rptContractReport.ContractTargetNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            rptContractReport.ContractTargetNameEN = "CSI (Thailand) Co.,Ltd.";
            rptContractReport.ContractTargetAddressLC = "ตึกสีลมคอมเพล็กซ์ ชั้น 28 ถนนสีลม เขตบางรัก กรุงเทพฯ";
            rptContractReport.ContractTargetAddressEN = "Silom complex building floor 28, Silom rd, Bangrak Bangkok";
            rptContractReport.SiteNameLC = "ตึกเอ็กซ์เชนจ์ ทาวเวอร์";
            rptContractReport.SiteNameEN = "Exchange Tower";
            rptContractReport.SiteAddressLC = "338 ตึกเอ็กซ์เชนจ์ ทาวเวอร์ ชั้น 33 ห้องหมายเลข 3304 ถนนสุขุมวิท แขวงคลองเตย เขตวัฒนา กรุงเทพ 10310";
            rptContractReport.SiteAddressEN = "338 Exchang Tower, 33rd FL, Unit No. 3304 Sukhumvit Klongtoey Wattana Bangkok 10310";
            rptContractReport.RealCustomerNameLC = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            rptContractReport.RealCustomerNameEN = "CSI (Thailand) Co.,Ltd.";
            rptContractReport.ProductCode = "FN12345567";
            rptContractReport.ProductNameEN = "Product name1";
            rptContractReport.ProductNameLC = "Product name1";
            rptContractReport.PhoneLineTypeCode = "";
            rptContractReport.PhoneLineTypeNameEN = "ISDN";
            rptContractReport.PhoneLineTypeNameLC = "ISDN";
            rptContractReport.FireSecurityFlag = false;
            rptContractReport.CrimePreventFlag = true;
            rptContractReport.EmergencyReportFlag = true;
            rptContractReport.ContractFee = 99999.99m;
            rptContractReport.DepositFee = 99999.99m;
            rptContractReport.ContractFeePayMethod = "1";
            rptContractReport.ContractPaymentMethodNameEN = "Auto transfer";
            rptContractReport.ContractPaymentMethodNameLC = "โอนอัตโนมัติ";
            rptContractReport.ContractPaymentMethodNameJP = "Auto transfer";
            rptContractReport.CreditTerm = 3;
            rptContractReport.PaymentCycle = 5;
            rptContractReport.ContractDurationMonth = 30;
            rptContractReport.PlanCode = "FN1234567";
            rptContractReport.DispatchType = "Notification only";
            rptContractReport.AutoRenewMonth = 15;
            rptContractReport.DepositFeePayMethod = "0";
            rptContractReport.DepositPaymentMethodNameEN = "Bank transfer";
            rptContractReport.DepositPaymentMethodNameLC = "โอนผ่านธนาคาร";
            rptContractReport.DepositPaymentMethodNameJP = "Bank transfer";
            rptContractReport.DepositFeePhase = "1";
            rptContractReport.DepositFeePhaseNameEN = "Complete installation";
            rptContractReport.DepositFeePhaseNameLC = "เมื่อติดตั้งเสร็จสิ้น";
            rptContractReport.DepositFeePhaseNameJP = "Complete installation";
            rptContractReport.InstallFee_ApproveContract = 99999.99m;
            rptContractReport.InstallFeePayMethod_ApproveContract = "2";
            rptContractReport.InstallApprovePaymentMethodNameEN = "Credit card";
            rptContractReport.InstallApprovePaymentMethodNameLC = "บัตรเครดิต";
            rptContractReport.InstallApprovePaymentMethodNameJP = "Credit card";
            rptContractReport.InstallFee_CompleteInstall = 99999.99m;
            rptContractReport.InstallFeePayMethod_CompleteInstall = "2";
            rptContractReport.InstallCompletePaymentMethodNameEN = "Credit card";
            rptContractReport.InstallCompletePaymentMethodNameLC = "บัตรเครดิต";
            rptContractReport.InstallCompletePaymentMethodNameJP = "Credit card";
            rptContractReport.InstallFee_StartService = 99999.99m;
            rptContractReport.InstallFeePayMethod_StartService = "2";
            rptContractReport.InstallStartServicePaymentMethodNameEN = "Credit card";
            rptContractReport.InstallStartServicePaymentMethodNameLC = "บัตรเครดิต";
            rptContractReport.InstallStartServicePaymentMethodNameJP = "Credit card";
            rptContractReport.CustomerSignatureName = "CSI (Thailand) Co.,Ltd.";
            rptContractReport.EmployeeName = "KAIJI ISHISAKI";
            rptContractReport.EmployeePosition = "DIRECTOR";
            rptContractReport.ImageSignaturePath = "signature_test2.png";
            rptContractReport.DocumentVersion = "Dec.2011";
            rptContractReport.DocNo = "N0000000009-0001-01";
            rptContractReport.DocID = 1234;
            rptContractReport.DocumentCode = "CTR010";
            rptContractReport.QuotationTargetCode = "";
            rptContractReport.Alphabet = "";
            rptContractReport.ContractCode = "N0000000009";
            rptContractReport.OCC = "0001";
            rptContractReport.ContractDocOCC = "01";
            rptContractReport.ContractOfficeCode = "";
            rptContractReport.OperationOfficeCode = "";
            rptContractReport.DocumentNameEN = "Contract";
            rptContractReport.DocumentNameLC = "(ไทย) Contract";

            rptContractReportList.Add(rptContractReport);

            return rptContractReportList;
        }

        #endregion


        #region GetRptStartResumeMemoData

        /// <summary>
        /// Get Start/Resume memorandum data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public List<RPTStartResumeMemoDo> GetRptStartResumeMemoData(int iDocID)
        {
            try
            {
                List<RPTStartResumeMemoDo> rptStartResumeMemoList = base.GetRptStartResumeMemoData(iDocID, FlagType.C_FLAG_ON);
                if (rptStartResumeMemoList == null)
                    rptStartResumeMemoList = new List<RPTStartResumeMemoDo>();

                return rptStartResumeMemoList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region GetRptConfirmCurrentInstrumentMemoData

        /// <summary>
        /// Get confirm current instrument memorandum data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public List<RPTConfirmCurrInstMemoDo> GetRptConfirmCurrentInstrumentMemoData(int iDocID)
        {
            try
            {
                List<RPTConfirmCurrInstMemoDo> rptConfirmCurrInstMemoList = base.GetRptConfirmCurrentInstrumentMemoData(iDocID, FlagType.C_FLAG_ON);
                if (rptConfirmCurrInstMemoList == null)
                    rptConfirmCurrInstMemoList = new List<RPTConfirmCurrInstMemoDo>();

                //Test
                //List<RPTConfirmCurrInstMemoDo> rptConfirmCurrInstMemoList;
                //rptConfirmCurrInstMemoList = GetRptConfirmCurrentInstrumentMemoMockup();

                return rptConfirmCurrInstMemoList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get confirm current instrument memorandum mockup
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        private List<RPTConfirmCurrInstMemoDo> GetRptConfirmCurrentInstrumentMemoMockup()
        {
            List<RPTConfirmCurrInstMemoDo> rptConfirmCurrInstMemoList = new List<RPTConfirmCurrInstMemoDo>();

            RPTConfirmCurrInstMemoDo rptConfirmCurrInstMemo = new RPTConfirmCurrInstMemoDo();
            rptConfirmCurrInstMemo.ContractTargetNameEN = "บริษัท ซิลเวอร์เจมส์การ์เด้น จำกัด";
            rptConfirmCurrInstMemo.PrintDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptConfirmCurrInstMemo.ContractCode = "N0000000009";
            rptConfirmCurrInstMemo.RealCustomerNameEN = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            rptConfirmCurrInstMemo.SiteNameEN = "อาคารซิลเวอร์เจมส์การ์เด้น";
            rptConfirmCurrInstMemo.SiteAddressEN = "10 ถนนเจริญกรุง ซอย 41 แขวงสี่พระยา เขตบางรัก กรุงเทพ 10500";
            rptConfirmCurrInstMemo.RealInvestigationDate = DateTime.Now.AddDays(5).ToString("dd-MMM-yyyy");
            rptConfirmCurrInstMemo.CustomerSignatureName = "บริษัท ซิลเวอร์เจมส์การ์เด้น จำกัด";
            rptConfirmCurrInstMemo.EmployeeName = "KAIJI ISHISAKI";
            rptConfirmCurrInstMemo.EmployeePosition = "DIRECTOR";
            rptConfirmCurrInstMemo.ImageSignaturePath = "signature_test2.png";
            rptConfirmCurrInstMemo.DocumentVersion = "Dec.2011";
            rptConfirmCurrInstMemo.DocNo = "N0000000009-0001-01";
            rptConfirmCurrInstMemo.DocID = 1234;
            rptConfirmCurrInstMemo.DocumentCode = "CTR050";
            rptConfirmCurrInstMemo.QuotationTargetCode = "";
            rptConfirmCurrInstMemo.Alphabet = "";
            rptConfirmCurrInstMemo.OCC = "0001";
            rptConfirmCurrInstMemo.ContractDocOCC = "01";
            rptConfirmCurrInstMemo.ContractOfficeCode = "";
            rptConfirmCurrInstMemo.OperationOfficeCode = "";
            rptConfirmCurrInstMemo.DocumentNameEN = "Confirm current instrument memorandum";
            rptConfirmCurrInstMemo.DocumentNameLC = "(ไทย) Confirm current instrument memorandum";

            rptConfirmCurrInstMemoList.Add(rptConfirmCurrInstMemo);

            return rptConfirmCurrInstMemoList;
        }

        #endregion

        #region GetRptCancelContractMemoData

        /// <summary>
        /// Get cancel contract memorandum data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public List<RPTCancelContractMemoDo> GetRptCancelContractMemoData(int iDocID)
        {
            try
            {
                List<RPTCancelContractMemoDo> rptCancelContractMemoList = base.GetRptCancelContractMemoData(iDocID, FlagType.C_FLAG_ON);
                if (rptCancelContractMemoList == null)
                    rptCancelContractMemoList = new List<RPTCancelContractMemoDo>();

                //Test
                //List<RPTCancelContractMemoDo> rptCancelContractMemoList;
                //rptCancelContractMemoList = GetRptCancelContractMemoMockup();

                return rptCancelContractMemoList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get cancel contract memorandum mockup
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        private List<RPTCancelContractMemoDo> GetRptCancelContractMemoMockup()
        {
            List<RPTCancelContractMemoDo> rptCancelMemoList = new List<RPTCancelContractMemoDo>();

            RPTCancelContractMemoDo rptCancelMemo = new RPTCancelContractMemoDo();
            rptCancelMemo.ContractTargetNameEN = "บริษัท ซิลเวอร์เจมส์การ์เด้น จำกัด";
            rptCancelMemo.PrintDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptCancelMemo.ContractCode = "N0000000009";
            rptCancelMemo.RealCustomerNameEN = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            rptCancelMemo.SiteNameEN = "อาคารซิลเวอร์เจมส์การ์เด้น";
            rptCancelMemo.SiteAddressEN = "10 ถนนเจริญกรุง ซอย 41 แขวงสี่พระยา เขตบางรัก กรุงเทพ 10500";
            rptCancelMemo.CancelContractDate = DateTime.Now.AddDays(5).ToString("dd-MMM-yyyy");
            rptCancelMemo.StartServiceDate = DateTime.Now.AddDays(5).ToString("dd-MMM-yyyy");
            rptCancelMemo.TotalSlideAmt = 99999.00m;
            rptCancelMemo.TotalReturnAmt = 99999.00m;
            rptCancelMemo.TotalBillingAmt = 99999.00m;
            rptCancelMemo.TotalAmtAfterCounterBalance = 99999.00m;
            rptCancelMemo.ProcessAfterCounterBalanceType = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND;
            rptCancelMemo.ProcessAfterCounterBalanceTypeName = "เงินคืน";
            rptCancelMemo.AutoTransferBillingType = AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_ALL;
            rptCancelMemo.AutoTransferBillingAmt = 99999.00m;
            rptCancelMemo.BankTransferBillingType = BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_PARTIAL;
            rptCancelMemo.BankTransferBillingAmt = 99999.00m;
            rptCancelMemo.CustomerSignatureName = "บริษัท ซิลเวอร์เจมส์การ์เด้น จำกัด";
            rptCancelMemo.OtherRemarks = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            rptCancelMemo.EmployeeName = "KAIJI ISHISAKI";
            rptCancelMemo.EmployeePosition = "DIRECTOR";
            rptCancelMemo.ImageSignaturePath = "signature_test2.png";
            rptCancelMemo.DocumentVersion = "Dec.2011";
            rptCancelMemo.DocNo = "N0000000009-0001-01";
            rptCancelMemo.DocID = 1234;
            rptCancelMemo.DocumentCode = "CTR060";
            rptCancelMemo.QuotationTargetCode = "";
            rptCancelMemo.Alphabet = "";
            rptCancelMemo.OCC = "0001";
            rptCancelMemo.ContractDocOCC = "01";
            rptCancelMemo.ContractOfficeCode = "";
            rptCancelMemo.OperationOfficeCode = "";
            rptCancelMemo.DocumentNameEN = "Cancel contract memorandum";

            rptCancelMemoList.Add(rptCancelMemo);

            return rptCancelMemoList;
        }

        #endregion

        #region GetRptCancelContractMemoDetail

        /// <summary>
        /// Get Cancel contract memo detail
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public List<RPTCancelContractMemoDetailDo> GetRptCancelContractMemoDetail(int iDocID)
        {
            try
            {
                List<RPTCancelContractMemoDetailDo> rptCancelContractMemoDetailList = base.GetRptCancelContractMemoDetailData(iDocID);
                if (rptCancelContractMemoDetailList == null)
                    rptCancelContractMemoDetailList = new List<RPTCancelContractMemoDetailDo>();

                //Test
                //List<RPTCancelContractMemoDetailDo> rptCancelContractMemoDetailList;
                //rptCancelContractMemoDetailList = GetRptCancelContractMemoDetailMockup();

                return rptCancelContractMemoDetailList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Cancel contract memo mockup
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        private List<RPTCancelContractMemoDetailDo> GetRptCancelContractMemoDetailMockup()
        {
            List<RPTCancelContractMemoDetailDo> rptCancelMemoList = new List<RPTCancelContractMemoDetailDo>();
            RPTCancelContractMemoDetailDo rptCancelMemo;

            rptCancelMemo = new RPTCancelContractMemoDetailDo();
            rptCancelMemo.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE;
            rptCancelMemo.BillingTypeName = "ค่ามัดจำ";
            rptCancelMemo.HandlingType = HandlingType.C_HANDLING_TYPE_SLIDE;
            rptCancelMemo.HandlingTypeName = "โอนไปสัญญาอื่น";
            rptCancelMemo.StartPeriodDate = "";
            rptCancelMemo.EndPeriodDate = "";
            rptCancelMemo.FeeAmount = 99999.00m;
            rptCancelMemo.TaxAmount = 99999.00m;
            rptCancelMemo.NormalFeeAmount = 99999.00m;
            rptCancelMemo.ContractCode_CounterBalance = "";
            rptCancelMemo.Remark = "aaaaaaaaaaaaaa";
            rptCancelMemoList.Add(rptCancelMemo);

            rptCancelMemo = new RPTCancelContractMemoDetailDo();
            rptCancelMemo.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON;
            rptCancelMemo.BillingTypeName = "ค่าบริการ";
            rptCancelMemo.HandlingType = HandlingType.C_HANDLING_TYPE_REFUND;
            rptCancelMemo.HandlingTypeName = "จ่ายเงินคืน";
            rptCancelMemo.StartPeriodDate = DateTime.Now.AddDays(5).ToString("dd-MMM-yyyy");
            rptCancelMemo.EndPeriodDate = DateTime.Now.AddDays(20).ToString("dd-MMM-yyyy");
            rptCancelMemo.FeeAmount = 99999.00m;
            rptCancelMemo.TaxAmount = 99999.00m;
            rptCancelMemo.NormalFeeAmount = 0;
            rptCancelMemo.ContractCode_CounterBalance = "N00000203";
            rptCancelMemo.Remark = "bbbbbbbbbbbbbb";
            rptCancelMemoList.Add(rptCancelMemo);

            rptCancelMemo = new RPTCancelContractMemoDetailDo();
            rptCancelMemo.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;
            rptCancelMemo.BillingTypeName = "ค่าถอดถอน";
            rptCancelMemo.HandlingType = HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
            rptCancelMemo.HandlingTypeName = "เก็บเงินเพิ่ม";
            rptCancelMemo.StartPeriodDate = DateTime.Now.AddDays(5).ToString("dd-MMM-yyyy");
            rptCancelMemo.EndPeriodDate = DateTime.Now.AddDays(20).ToString("dd-MMM-yyyy");
            rptCancelMemo.FeeAmount = 99999.00m;
            rptCancelMemo.TaxAmount = 99999.00m;
            rptCancelMemo.NormalFeeAmount = 99999.00m;
            rptCancelMemo.ContractCode_CounterBalance = "";
            rptCancelMemo.Remark = "cccccccccccccccc";
            rptCancelMemoList.Add(rptCancelMemo);

            rptCancelMemo = new RPTCancelContractMemoDetailDo();
            rptCancelMemo.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;
            rptCancelMemo.BillingTypeName = "ค่าถอดถอน";
            rptCancelMemo.HandlingType = HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
            rptCancelMemo.HandlingTypeName = "เก็บเงินเพิ่ม";
            rptCancelMemo.StartPeriodDate = "";
            rptCancelMemo.EndPeriodDate = "";
            rptCancelMemo.FeeAmount = 99999.00m;
            rptCancelMemo.TaxAmount = 99999.00m;
            rptCancelMemo.NormalFeeAmount = 0;
            rptCancelMemo.ContractCode_CounterBalance = "";
            rptCancelMemo.Remark = "dddddddddddddddddd";
            rptCancelMemoList.Add(rptCancelMemo);

            //rptCancelMemo = new RPTCancelContractMemoDetailDo();
            //rptCancelMemo.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;
            //rptCancelMemo.BillingTypeName = "ค่าถอดถอน";
            //rptCancelMemo.HandlingType = HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
            //rptCancelMemo.HandlingTypeName = "เก็บเงินเพิ่ม";
            //rptCancelMemo.StartPeriodDate = DateTime.Now.AddDays(5).ToString("dd-MMM-yyyy");
            //rptCancelMemo.EndPeriodDate = DateTime.Now.AddDays(20).ToString("dd-MMM-yyyy");
            //rptCancelMemo.FeeAmount = 99999.00m;
            //rptCancelMemo.TaxAmount = 99999.00m;
            //rptCancelMemo.NormalFeeAmount = 99999.00m;
            //rptCancelMemo.ContractCode_CounterBalance = "";
            //rptCancelMemo.Remark = "eeeeeeeeeeeeeeeeeeee";
            //rptCancelMemoList.Add(rptCancelMemo);

            return rptCancelMemoList;
        }

        #endregion

        #region GetRptChangeFeeMemoData

        /// <summary>
        /// Get change fee memorandum data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public List<RPTChangeFeeMemoDo> GetRptChangeFeeMemoData(int iDocID)
        {
            try
            {
                List<RPTChangeFeeMemoDo> rptChangeFeeMemoList = base.GetRptChangeFeeMemoData(iDocID, FlagType.C_FLAG_ON);
                if (rptChangeFeeMemoList == null)
                    rptChangeFeeMemoList = new List<RPTChangeFeeMemoDo>();

                //Test
                //List<RPTChangeFeeMemoDo> rptChangeFeeMemoList;
                //rptChangeFeeMemoList = GetRptChangeFeeMemoMockup();

                return rptChangeFeeMemoList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get change fee memorandum mockup
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        private List<RPTChangeFeeMemoDo> GetRptChangeFeeMemoMockup()
        {
            List<RPTChangeFeeMemoDo> rptChangeMemoList = new List<RPTChangeFeeMemoDo>();

            RPTChangeFeeMemoDo rptChangeMemo = new RPTChangeFeeMemoDo();
            rptChangeMemo.ContractTargetNameEN = "บริษัท ซิลเวอร์เจมส์การ์เด้น จำกัด";
            rptChangeMemo.PrintDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptChangeMemo.RealCustomerNameEN = "บริษัท ซีเอสไอ ไทยแลนด์ จำกัด";
            rptChangeMemo.SiteNameEN = "อาคารซิลเวอร์เจมส์การ์เด้น";
            rptChangeMemo.SiteAddressEN = "10 ถนนเจริญกรุง ซอย 41 แขวงสี่พระยา เขตบางรัก กรุงเทพ 10500";
            rptChangeMemo.ContractCode = "N0000000009";
            rptChangeMemo.EffectiveDate = DateTime.Now.AddDays(5).ToString("dd-MMM-yyyy");
            rptChangeMemo.OldContractFee = 4500;
            rptChangeMemo.NewContractFee = 4000;
            rptChangeMemo.ChangeContractFeeDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptChangeMemo.ReturnToOriginalFeeDate = DateTime.Now.AddDays(5).ToString("dd-MMM-yyyy");
            rptChangeMemo.CustomerSignatureName = "บริษัท ซิลเวอร์เจมส์การ์เด้น จำกัด";
            rptChangeMemo.EmployeeName = "KAIJI ISHISAKI";
            rptChangeMemo.EmployeePosition = "DIRECTOR";
            rptChangeMemo.ImageSignaturePath = "signature_test2.png";
            rptChangeMemo.DocumentVersion = "Dec.2011";
            rptChangeMemo.DocNo = "N0000000009-0001-01";
            rptChangeMemo.DocID = 1234;
            rptChangeMemo.DocumentCode = "CTR070";
            rptChangeMemo.QuotationTargetCode = "";
            rptChangeMemo.Alphabet = "";
            rptChangeMemo.OCC = "0001";
            rptChangeMemo.ContractDocOCC = "01";
            rptChangeMemo.ContractOfficeCode = "";
            rptChangeMemo.OperationOfficeCode = "";
            rptChangeMemo.DocumentNameEN = "Change fee memorandum";
            rptChangeMemoList.Add(rptChangeMemo);

            return rptChangeMemoList;
        }

        #endregion

        #region GetRptCoverLetterData

        /// <summary>
        /// Get contract cover letter data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public List<RPTCoverLetterDo> GetRptCoverLetterData(int iDocID)
        {
            try
            {
                List<RPTCoverLetterDo> rptCoverLetterList = base.GetRptCoverLetterData(iDocID, MiscType.C_PAYMENT_METHOD);
                if (rptCoverLetterList == null)
                    rptCoverLetterList = new List<RPTCoverLetterDo>();

                return rptCoverLetterList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region GetRptInstrumentDetailData

        /// <summary>
        /// Get instrument detail data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public List<RPTInstrumentDetailDo> GetRptInstrumentDetailData(int iDocID)
        {
            try
            {
                List<RPTInstrumentDetailDo> rptInstrumentDetailList = base.GetRptInstrumentDetailData(iDocID);
                if (rptInstrumentDetailList == null)
                    rptInstrumentDetailList = new List<RPTInstrumentDetailDo>();

                //Test
                //List<RPTInstrumentDetailDo> rptInstrumentDetailList;
                //rptInstrumentDetailList = GetInstrumentDetailMockup();

                return rptInstrumentDetailList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get instrument detail mockup
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        private List<RPTInstrumentDetailDo> GetInstrumentDetailMockup()
        {
            List<RPTInstrumentDetailDo> rptInstrumentDetailList = new List<RPTInstrumentDetailDo>();
            RPTInstrumentDetailDo rptInstrumentDetail;

            rptInstrumentDetail = new RPTInstrumentDetailDo();
            rptInstrumentDetail.DocID = 100;
            rptInstrumentDetail.InstrumentCode = "00000001";
            rptInstrumentDetail.InstrumentQty = 100;
            rptInstrumentDetail.CreateDate = DateTime.Now.AddDays(-5).ToString("dd-MMM-yyyy");
            rptInstrumentDetail.CreateBy = "";
            rptInstrumentDetail.UpdateDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptInstrumentDetail.UpdateBy = "";
            rptInstrumentDetailList.Add(rptInstrumentDetail);

            rptInstrumentDetail = new RPTInstrumentDetailDo();
            rptInstrumentDetail.DocID = 100;
            rptInstrumentDetail.InstrumentCode = "00000002";
            rptInstrumentDetail.InstrumentQty = 200;
            rptInstrumentDetail.CreateDate = DateTime.Now.AddDays(-5).ToString("dd-MMM-yyyy");
            rptInstrumentDetail.CreateBy = "";
            rptInstrumentDetail.UpdateDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptInstrumentDetail.UpdateBy = "";
            rptInstrumentDetailList.Add(rptInstrumentDetail);

            rptInstrumentDetail = new RPTInstrumentDetailDo();
            rptInstrumentDetail.DocID = 100;
            rptInstrumentDetail.InstrumentCode = "00000003";
            rptInstrumentDetail.InstrumentQty = 300;
            rptInstrumentDetail.CreateDate = DateTime.Now.AddDays(-5).ToString("dd-MMM-yyyy");
            rptInstrumentDetail.CreateBy = "";
            rptInstrumentDetail.UpdateDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptInstrumentDetail.UpdateBy = "";
            rptInstrumentDetailList.Add(rptInstrumentDetail);

            rptInstrumentDetail = new RPTInstrumentDetailDo();
            rptInstrumentDetail.DocID = 100;
            rptInstrumentDetail.InstrumentCode = "00000004";
            rptInstrumentDetail.InstrumentQty = 400;
            rptInstrumentDetail.CreateDate = DateTime.Now.AddDays(-5).ToString("dd-MMM-yyyy");
            rptInstrumentDetail.CreateBy = "";
            rptInstrumentDetail.UpdateDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptInstrumentDetail.UpdateBy = "";
            rptInstrumentDetailList.Add(rptInstrumentDetail);

            rptInstrumentDetail = new RPTInstrumentDetailDo();
            rptInstrumentDetail.DocID = 100;
            rptInstrumentDetail.InstrumentCode = "00000005";
            rptInstrumentDetail.InstrumentQty = 500;
            rptInstrumentDetail.CreateDate = DateTime.Now.AddDays(-5).ToString("dd-MMM-yyyy");
            rptInstrumentDetail.CreateBy = "";
            rptInstrumentDetail.UpdateDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptInstrumentDetail.UpdateBy = "";
            rptInstrumentDetailList.Add(rptInstrumentDetail);

            rptInstrumentDetail = new RPTInstrumentDetailDo();
            rptInstrumentDetail.DocID = 100;
            rptInstrumentDetail.InstrumentCode = "00000006";
            rptInstrumentDetail.InstrumentQty = 600;
            rptInstrumentDetail.CreateDate = DateTime.Now.AddDays(-5).ToString("dd-MMM-yyyy");
            rptInstrumentDetail.CreateBy = "";
            rptInstrumentDetail.UpdateDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptInstrumentDetail.UpdateBy = "";
            rptInstrumentDetailList.Add(rptInstrumentDetail);

            rptInstrumentDetail = new RPTInstrumentDetailDo();
            rptInstrumentDetail.DocID = 100;
            rptInstrumentDetail.InstrumentCode = "00000007";
            rptInstrumentDetail.InstrumentQty = 700;
            rptInstrumentDetail.CreateDate = DateTime.Now.AddDays(-5).ToString("dd-MMM-yyyy");
            rptInstrumentDetail.CreateBy = "";
            rptInstrumentDetail.UpdateDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptInstrumentDetail.UpdateBy = "";
            rptInstrumentDetailList.Add(rptInstrumentDetail);

            rptInstrumentDetail = new RPTInstrumentDetailDo();
            rptInstrumentDetail.DocID = 100;
            rptInstrumentDetail.InstrumentCode = "00000008";
            rptInstrumentDetail.InstrumentQty = 800;
            rptInstrumentDetail.CreateDate = DateTime.Now.AddDays(-5).ToString("dd-MMM-yyyy");
            rptInstrumentDetail.CreateBy = "";
            rptInstrumentDetail.UpdateDate = DateTime.Now.ToString("dd-MMM-yyyy");
            rptInstrumentDetail.UpdateBy = "";
            rptInstrumentDetailList.Add(rptInstrumentDetail);

            return rptInstrumentDetailList;
        }

        #endregion

        /// <summary>
        /// Getting contract document data for issue
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strOccAlphabet"></param>
        /// <returns></returns>
        public List<dtContractDoc> GetContractDocDataList(string strContractCode, string strQuotationTargetCode, string strOccAlphabet)
        {
            try
            {
                string strOfficeCode = string.Empty;
                List<OfficeDataDo> oplst = CommonUtil.dsTransData.dtOfficeData;

                StringBuilder sbOperationOffice = new StringBuilder("");
                foreach (OfficeDataDo off in oplst)
                {
                    sbOperationOffice.AppendFormat("\'{0}\',", off.OfficeCode);
                }

                if (sbOperationOffice.Length > 0)
                    strOfficeCode = sbOperationOffice.ToString().Substring(0, sbOperationOffice.Length - 1);

                List<dtContractDoc> dtContractDocList = base.GetContractDocDataList(strContractCode, strQuotationTargetCode, strOccAlphabet, strOfficeCode, ModuleID.C_MODULE_ID_CONTRACT, MiscType.C_AR_STATUS, FlagType.C_FLAG_ON);
                if (dtContractDocList == null)
                {
                    dtContractDocList = new List<dtContractDoc>();
                }
                else
                {
                    CommonUtil.MappingObjectLanguage<dtContractDoc>(dtContractDocList);
                }

                return dtContractDocList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get contract document data for issue
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strOccAlphabet"></param>
        /// <returns></returns>
        public dsContractDocForIssue GetContractDocForIssue(string strContractCode, string strQuotationTargetCode, string strOccAlphabet)
        {
            dsContractDocForIssue dsContractDoc = new dsContractDocForIssue();

            try
            {
                //Check mandatory data
                if (String.IsNullOrEmpty(strContractCode) || String.IsNullOrEmpty(strQuotationTargetCode) || String.IsNullOrEmpty(strOccAlphabet))
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                //Get contract document data
                List<dtContractDoc> dtContractDocList = GetContractDocDataList(strContractCode, strQuotationTargetCode, strOccAlphabet);
                dsContractDoc.dtContractDocList = dtContractDocList;

                //Get rental contract information data
                IRentralContractHandler rentralContHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<dtRentalContractBasicForView> dtRentalContractBasicForViewList = rentralContHandler.GetRentalContractBasicForView(strContractCode, null);
                dsContractDoc.dtRentalContractBasicForViewList = dtRentalContractBasicForViewList;

                //Get rental contract information data
                ISaleContractHandler saleContHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                List<dtSaleContractBasicForView> dtSaleContractBasicForViewList = saleContHandler.GetSaleContractBasicForView(strContractCode);
                dsContractDoc.dtSaleContractBasicForViewList = dtSaleContractBasicForViewList;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dsContractDoc;
        }

        /// <summary>
        /// To search contract document data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<dtContractDocumentList> SearchContractDocument(doSearchContractDocCondition cond)
        {
            try
            {
                List<dtContractDocumentList> dtContractDocList = base.SearchContractDocument(cond.DocStatus
                                                                                            , cond.ContractCode
                                                                                            , cond.QuotationTargetCode
                                                                                            , cond.ProjectCode
                                                                                            , cond.OCC
                                                                                            , cond.Alphabet
                                                                                            , cond.ContractOfficeCode
                                                                                            , cond.OperationOfficeCode
                                                                                            , cond.ContractOfficeCodeAuthority
                                                                                            , cond.OperationOfficeCodeAuthority
                                                                                            , cond.NegotiationStaffEmpNo
                                                                                            , cond.NegotiationStaffEmpName
                                                                                            , cond.DocumentCode
                                                                                            , MiscType.C_CONTRACT_DOC_STATUS
                                                                                            , MiscType.C_DOC_AUDIT_RESULT
                                                                                            , FlagType.C_FLAG_ON);

                if (dtContractDocList == null)
                {
                    dtContractDocList = new List<dtContractDocumentList>();
                }
                else
                {
                    CommonUtil.MappingObjectLanguage<dtContractDocumentList>(dtContractDocList);
                }

                return dtContractDocList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get contract document
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public override List<tbt_DocContractReport> GetTbt_DocContractReport(int? iDocID)
        {
            try
            {
                List<tbt_DocContractReport> dtTbt_DocContractReport = base.GetTbt_DocContractReport(iDocID);
                if (dtTbt_DocContractReport == null)
                {
                    dtTbt_DocContractReport = new List<tbt_DocContractReport>();
                }
                else
                {
                    MiscTypeMappingList miscList = new MiscTypeMappingList();
                    miscList.AddMiscType(dtTbt_DocContractReport.ToArray());

                    ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    comHandler.MiscTypeMappingList(miscList);
                }

                return dtTbt_DocContractReport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get contract document template
        /// </summary>
        /// <param name="strDocumentCode"></param>
        /// <returns></returns>
        public override List<tbt_DocCancelContractMemoDetail> GetTbt_DocCancelContractMemoDetail(int? iDocID)
        {
            try
            {
                List<tbt_DocCancelContractMemoDetail> dtTbt_DocCancelMemoDetail = base.GetTbt_DocCancelContractMemoDetail(iDocID);
                if (dtTbt_DocCancelMemoDetail == null)
                {
                    dtTbt_DocCancelMemoDetail = new List<tbt_DocCancelContractMemoDetail>();
                }
                else
                {
                    MiscTypeMappingList miscList = new MiscTypeMappingList();
                    miscList.AddMiscType(dtTbt_DocCancelMemoDetail.ToArray());

                    ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    comHandler.MiscTypeMappingList(miscList);
                }

                return dtTbt_DocCancelMemoDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To check existing in contract document
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strOCCAlphabet"></param>
        /// <param name="strDocOCC"></param>
        /// <returns></returns>
        public bool IsContractDocExist(string strContractCode, string strQuotationTargetCode, string strOCCAlphabet, string strDocOCC)
        {
            bool bResult = false;

            try
            {
                List<bool?> bResultList = base.IsContractDocExist(strContractCode, strQuotationTargetCode, strOCCAlphabet, strDocOCC);
                if (bResultList != null && bResultList.Count > 0 && bResultList[0] != null)
                    bResult = bResultList[0].Value;

                return bResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }

    public class ContractReportData
    {
        public string DocumentCode { get; set; }
        public string ContractCode { get; set; }
        public string ContractDocOCC { get; set; }
        public string QuotationTargetCode { get; set; }
        public string Alphabet { get; set; }
        public string ContractOfficeCode { get; set; }
        public string OperationOfficeCode { get; set; }
        public string OCC { get; set; }
    }
}
