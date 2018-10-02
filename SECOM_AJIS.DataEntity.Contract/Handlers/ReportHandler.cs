using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using System.IO;
using System.Reflection;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class ReportHandler : BizCTDataEntities, IReportHandler
    {
        /// <summary>
        /// To get CTR100 – Maintenance check-up slip report
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="ProductCode"></param>
        /// <param name="InstructionDate"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        public string GetMaintenanceCheckupSlip(string contractCode, string productCode, DateTime? instructionDate, string owner_pwd = null)
        {            
            List<string> listInstrumentContract;            
            List<tbt_MaintenanceCheckupDetails> listMaintenanceCheckupDetails;
            List<tbm_DocumentTemplate> listDocumentTemplate;
            List<tbm_DocumentTemplate> listDocumentTemplateFilter;

            List<RPTMACheckupSlipDo> listRPTMACheckupSlipDo;
            List<RPTInstrumentCheckupDo> listRPTInstrumentCheckupDo;
            List<DateTime?> listDateTime;

            IMaintenanceHandler maintenanceHandler;
            IDocumentHandler documentHandler;
            IMasterHandler masterHandler;

            doDocumentDataGenerate doDocumentData1;
            doDocumentDataGenerate doDocumentData2;
            RPTSlipReport rptSlip;

            string occList = "";            
            string contractCodeList = "";
            List<string> occListTemp = new List<string>();
            List<string> contractCodeListTemp = new List<string>();
            Stream streamFile;
            string strFilePath;

            string strHeaderName1 = "ใบตรวจสอบการบำรุงรักษา";
            string strHeaderName2 = "ใบสรุปผลการบำรุงรักษา";

            try
            {
                listInstrumentContract = new List<string>();
                rptSlip = new RPTSlipReport();
                
                maintenanceHandler = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler; 

                //1. Get maintenance schedule and maintenance contract data
                //1.1. Query data from stored procedure as dtMACheckupSlip

                //ใส่ไว้เทสก่อนนะครับ
                //DateTime dt = new DateTime(2011,1,1);
                // GetMaintenanceCheckupSlipReport(string paramContractCode, string paramProductCode, Nullable<System.DateTime> paramInstructionDate, string paramContractStatusAfterStart)
                //listRPTMACheckupSlipDo = GetMaintenanceCheckupSlipReport("MA0002700012", "001", dt, ContractStatus.C_CONTRACT_STATUS_AFTER_START);
                listRPTMACheckupSlipDo = base.GetMaintenanceCheckupSlipReport(contractCode, productCode, instructionDate, ContractStatus.C_CONTRACT_STATUS_AFTER_START);

                //2. Get maintenance contract target
                //2.1. Get maintenance schedule detail
                listMaintenanceCheckupDetails = this.GetTbt_MaintenanceCheckupDetails(contractCode, productCode, instructionDate, null, null);

                //2.2. Prepare a list of maintenance target contract
                //foreach (var item in listMaintenanceCheckupDetails)
                //{
                //    contractCodeList += "'" + item.MATargetContractCode.ToString() + "',";
                //}
                //// Akat K. prevent index of string less than 0
                //if (contractCodeList.Length > 0)
                //{
                //    contractCodeList = contractCodeList.Remove(contractCodeList.Count() - 1, 1);
                //} else {
                //    contractCodeList = null;
                //}

                ////----------------------------------------------------------------------

                //foreach (var item in listMaintenanceCheckupDetails)
                //{
                //    occList += "'" + item.MATargetOCC.ToString() + "',";
                //}
                //// Akat K. prevent index of string less than 0
                //if (occList.Length > 0)
                //{
                //    occList = occList.Remove(occList.Count() - 1, 1);
                //} else {
                //    occList = null;
                //}
                foreach (var item in listMaintenanceCheckupDetails)
                {
                    contractCodeListTemp.Add(item.MATargetContractCode);
                    occListTemp.Add(item.MATargetOCC);
                }
                contractCodeList = CommonUtil.CreateCSVString(contractCodeListTemp);
                occList = CommonUtil.CreateCSVString(occListTemp);

                //2.3. Get instrument list
                listRPTInstrumentCheckupDo = this.GetInstrument(contractCodeList, occList);

                //------------------------------------------------              


                //3. Get last maintenance date
                //3.1. Query data from this query
                //foreach (var item in listRPTMACheckupSlipDo)
                //{
                //    listDateTime = this.GetLastMaintenanceDate(item.ContractCode, item.OCC);
                //    if (listDateTime.Count != 0)
                //    {
                //        if (this.GetLastMaintenanceDate(item.ContractCode, item.OCC).Count != 0)
                //            if (this.GetLastMaintenanceDate(item.ContractCode, item.OCC)[0] != null)
                //                item.LastMaintenanceDate = CommonUtil.TextDate(this.GetLastMaintenanceDate(item.ContractCode, item.OCC)[0]);
                //    }
                //}
                if (listRPTMACheckupSlipDo != null && listRPTMACheckupSlipDo.Count > 0)
                {
                    listDateTime = this.GetLastMaintenanceDate(contractCode, productCode);
                    if (listDateTime != null && listDateTime.Count > 0)
                    {
                        listRPTMACheckupSlipDo[0].LastMaintenanceDate = CommonUtil.TextDate(listDateTime[0]);
                    }
                }
                

                listDocumentTemplate = documentHandler.GetTbm_DocumentTemplate(DocumentType.C_DOCUMENT_TYPE_MA);
                if (listDocumentTemplate.Count == 0)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0018);

                listDocumentTemplateFilter = listDocumentTemplate.FindAll(delegate(tbm_DocumentTemplate s) { return s.DocumentCode == DocumentCode.C_DOCUMENT_CODE_MAINTENANCE_CHECKUP_SLIP; });
                if (listDocumentTemplateFilter.Count == 0)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0018);

                List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocumentVersion", Value = "( " + listDocumentTemplateFilter[0].DocumentVersion + " )" });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocumentNameLC", Value = listDocumentTemplateFilter[0].DocumentNameLC });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "HeaderName", Value = strHeaderName2 });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "IsShowRemark", Value = true }); //Add by Jutarat A. on 22012013

                List<ReportParameterObject> listSubReportDataSource = new List<ReportParameterObject>();
                listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "CTR100_1", Value = listRPTInstrumentCheckupDo });

                doDocumentData1 = new doDocumentDataGenerate();
                doDocumentData1.DocumentNo = listRPTMACheckupSlipDo[0].CheckupNo.ToString();
                doDocumentData1.DocumentCode = DocumentCode.C_DOCUMENT_CODE_MAINTENANCE_CHECKUP_SLIP;
                doDocumentData1.DocumentData = listRPTMACheckupSlipDo;  
                doDocumentData1.MainReportParam = listMainReportParam;
                doDocumentData1.SubReportDataSource = listSubReportDataSource;
                doDocumentData1.OtherKey.ContractCode = listRPTMACheckupSlipDo[0].ContractCode;
                doDocumentData1.OtherKey.ContractOCC = listRPTMACheckupSlipDo[0].OCC;
                doDocumentData1.OtherKey.QuotationTargetCode = listRPTMACheckupSlipDo[0].QuotationTargetCode;
                doDocumentData1.OtherKey.QuotationAlphabet = listRPTMACheckupSlipDo[0].QuotationAlphabet;
                doDocumentData1.OtherKey.ContractOffice = listRPTMACheckupSlipDo[0].ContractOfficeCode;
                doDocumentData1.OtherKey.OperationOffice = listRPTMACheckupSlipDo[0].OperationOfficeCode;
                doDocumentData1.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocumentData1.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;


                listMainReportParam = new List<ReportParameterObject>();
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocumentVersion", Value = "( " + listDocumentTemplateFilter[0].DocumentVersion + " )" });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocumentNameLC", Value = listDocumentTemplateFilter[0].DocumentNameLC });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "HeaderName", Value = strHeaderName1 });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "IsShowRemark", Value = false }); //Add by Jutarat A. on 22012013

                doDocumentData2 = new doDocumentDataGenerate();
                doDocumentData2.DocumentNo = listRPTMACheckupSlipDo[0].CheckupNo.ToString();
                doDocumentData2.DocumentCode = DocumentCode.C_DOCUMENT_CODE_MAINTENANCE_CHECKUP_SLIP;
                doDocumentData2.DocumentData = listRPTMACheckupSlipDo;
                doDocumentData2.MainReportParam = listMainReportParam;
                doDocumentData2.SubReportDataSource = listSubReportDataSource;
                doDocumentData2.OtherKey.ContractCode = listRPTMACheckupSlipDo[0].ContractCode;
                doDocumentData2.OtherKey.ContractOCC = listRPTMACheckupSlipDo[0].OCC;
                doDocumentData2.OtherKey.QuotationTargetCode = listRPTMACheckupSlipDo[0].QuotationTargetCode;
                doDocumentData2.OtherKey.QuotationAlphabet = listRPTMACheckupSlipDo[0].QuotationAlphabet;
                doDocumentData2.OtherKey.ContractOffice = listRPTMACheckupSlipDo[0].ContractOfficeCode;
                doDocumentData2.OtherKey.OperationOffice = listRPTMACheckupSlipDo[0].OperationOfficeCode;
                doDocumentData2.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocumentData2.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;


                //4. Generate PDF
                //4.1. Prepare parameters
                //streamFile = documentHandler.GenerateDocument(doDocumentData1, owner_pwd);
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    streamFile.CopyTo(ms);
                //    return ms.ToArray();
                //}    
                strFilePath = documentHandler.GenerateDocumentWithoutEncrypt(doDocumentData1, doDocumentData2);
                return strFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public string GetMaintenanceCheckupList(List<Object[]> MACheckupKey)
        /// <summary>
        /// To get CTR120 – Maintenance check-up list
        /// </summary>
        /// <param name="MACheckupList"></param>
        /// <returns></returns>
        public string GetMaintenanceCheckupList(List<tbt_MaintenanceCheckup> MACheckupList)
        {       
            //List<tbt_MaintenanceCheckup> listMaintenanceCheckup;
            List<dtGetMaintenanceCheckupList> listDTGetMaintenanceCheckupList;
            //tbt_MaintenanceCheckup tbtMaintenanceCheckup;
            string csv = "";

            CommonUtil comUtil = new CommonUtil();
            List<dtMACheckupCSVData> dtMACheckupCSVDataList = new List<dtMACheckupCSVData>();

            try
            {   
                //listMaintenanceCheckup = new List<tbt_MaintenanceCheckup>();
                //tbtMaintenanceCheckup = new tbt_MaintenanceCheckup();    
                //tbtMaintenanceCheckup.ContractCode = MACheckupKey[0][0].ToString();
                //tbtMaintenanceCheckup.ProductCode = MACheckupKey[0][1].ToString();
                //tbtMaintenanceCheckup.InstructionDate = Convert.ToDateTime(MACheckupKey[0][2]);
                //listMaintenanceCheckup.Add(tbtMaintenanceCheckup);

                //listDTGetMaintenanceCheckupList = this.GetMaintenanceCheckupList(CommonUtil.ConvertToXml_Store<tbt_MaintenanceCheckup>(listMaintenanceCheckup), MACheckupKey[0][1].ToString(), Convert.ToDateTime(MACheckupKey[0][2]), ContractStatus.C_CONTRACT_STATUS_AFTER_START);
                listDTGetMaintenanceCheckupList = this.GetMaintenanceCheckupList(CommonUtil.ConvertToXml_Store<tbt_MaintenanceCheckup>(MACheckupList));   

                //var sb = new StringBuilder();
                //sb.Append(string.Format("OperationOfficeCode,ContractCode,UserCode,ProductCode,SiteNameEN,SiteNameLC,LastMaintenanceDate,InstructionDate,ExpectedMaintenanceDate") + Environment.NewLine);

                //string strContractCodeShort = string.Empty;
                //string strLastMaintenanceDate = string.Empty;
                //string strInstructionDate = string.Empty;
                //string strExpectedMaintenanceDate = string.Empty;
                //foreach (var o in listDTGetMaintenanceCheckupList)
                //{
                //    strContractCodeShort = comUtil.ConvertContractCode(o.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //    strLastMaintenanceDate = o.LastMaintenanceDate == null ? string.Empty : o.LastMaintenanceDate.Value.ToShortDateString();
                //    strInstructionDate = o.InstructionDate.ToShortDateString();
                //    strExpectedMaintenanceDate = o.ExpectedMaintenanceDate == null ? string.Empty : o.ExpectedMaintenanceDate.Value.ToShortDateString();

                //    //sb.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", o.OperationOfficeCode, o.ContractCode, o.UserCode, o.ProductCode, o.SiteNameEN, o.SiteNameLC, o.LastMaintenanceDate, o.InstructionDate, o.ExpectedMaintenanceDate) + Environment.NewLine);                
                //    sb.Append(string.Format("{0},{1},{2},{3},\"{4}\",\"{5}\",{6},{7},{8}", o.OperationOfficeCode, strContractCodeShort, o.UserCode, o.ProductCode, o.SiteNameEN, o.SiteNameLC, strLastMaintenanceDate, strInstructionDate, strExpectedMaintenanceDate) + Environment.NewLine);

                //    csv = sb.ToString(0, sb.Length - 1);
                //}
                dtMACheckupCSVDataList = CommonUtil.ClonsObjectList<dtGetMaintenanceCheckupList, dtMACheckupCSVData>(listDTGetMaintenanceCheckupList);
                csv = CSVReportUtil.GenerateCSVData<dtMACheckupCSVData>(dtMACheckupCSVDataList);

                return csv; 
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }
        
        /// <summary>
        /// Get CTR101 - Maintenance check sheet report
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="ProductCode"></param>
        /// <param name="InstructionDate"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        public string GetMaintenanceCompletionReport(string ContractCode, string ProductCode, DateTime? InstructionDate, string owner_pwd = null)
        {
            IMaintenanceHandler maintenanceHandler;
            IDocumentHandler documentHandler;
            IMasterHandler masterHandler;

            doDocumentDataGenerate doDocumentData;

            string strFilePath;

            try
            {
                maintenanceHandler = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                var lstReport = base.GetMaintenanceCompletionReport(ContractCode, ProductCode, InstructionDate, FlagType.C_FLAG_ON);
                if (lstReport == null || lstReport.Count <= 0)
                {
                    return null;
                }

                var lstDocTemplate = documentHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_MAINTENANCE_COMPLETION_REPORT);
                if (lstDocTemplate.Count == 0)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0018);

                List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocumentVersion", Value = "( " + lstDocTemplate[0].DocumentVersion + " )" });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocumentNameLC", Value = lstDocTemplate[0].DocumentNameLC });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "RowsPerPage", Value = lstReport[0].RowsPerPage });

                doDocumentData = new doDocumentDataGenerate();
                doDocumentData.DocumentNo = lstReport[0].CheckupNo;
                doDocumentData.DocumentCode = DocumentCode.C_DOCUMENT_CODE_MAINTENANCE_COMPLETION_REPORT;
                doDocumentData.DocumentData = lstReport;
                doDocumentData.MainReportParam = listMainReportParam;
                doDocumentData.OtherKey.ContractCode = lstReport[0].ContractCode;
                doDocumentData.OtherKey.ContractOCC = lstReport[0].OCC;
                doDocumentData.OtherKey.QuotationTargetCode = lstReport[0].QuotationTargetCode;
                doDocumentData.OtherKey.QuotationAlphabet = lstReport[0].QuotationAlphabet;
                doDocumentData.OtherKey.ContractOffice = lstReport[0].ContractOfficeCode;
                doDocumentData.OtherKey.OperationOffice = lstReport[0].OperationOfficeCode;
                doDocumentData.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocumentData.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                strFilePath = documentHandler.GenerateDocumentWithoutEncrypt(doDocumentData);
                return strFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class dtMACheckupCSVData
    {
        [CSVMapping(SequenceNo = 1)]
        public long? RunNo { get; set; }

        [CSVMapping(SequenceNo = 2)]
        public string OperationOfficeCode { get; set; }

        [CSVMapping(HeaderName = "Contract no.", SequenceNo = 3)]
        public string ContractCodeShort
        {
            get
            {
                CommonUtil comUtil = new CommonUtil();
                return comUtil.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        [CSVMapping(SequenceNo = 4)]
        public string UserCode { get; set; }

        [CSVMapping(SequenceNo = 5)]
        public string ProductCode { get; set; }

        [CSVMapping(HeaderName = "Premise's name EN", SequenceNo = 6)]
        public string SiteNameEN { get; set; }

        [CSVMapping(HeaderName = "Premise's name LC", SequenceNo = 7)]
        public string SiteNameLC { get; set; }

        [CSVMapping(HeaderName = "LastMaintenanceDate", SequenceNo = 8)]
        public string LastMaintenanceDateDisplay
        {
            get
            {
                //return this.LastMaintenanceDate == null ? string.Empty : this.LastMaintenanceDate.Value.ToShortDateString();
                return CommonUtil.TextDate(this.LastMaintenanceDate);
            }
        }

        [CSVMapping(HeaderName = "InstructionDate", SequenceNo = 9)]
        public string InstructionDateDisplay
        {
            get
            {
                //return this.InstructionDate.ToShortDateString();
                return CommonUtil.TextDate(this.InstructionDate);
            }
        }

        [CSVMapping(HeaderName = "ExpectedMaintenanceDate", SequenceNo = 10)]
        public string ExpectedMaintenanceDateDisplay
        {
            get
            {
                //return this.ExpectedMaintenanceDate == null ? string.Empty : this.ExpectedMaintenanceDate.Value.ToShortDateString();
                return CommonUtil.TextDate(this.ExpectedMaintenanceDate);
            }
        }

        [CSVMapping(HeaderName = "MaintenanceDate", SequenceNo = 11)]
        public string MaintenanceDateDisplay
        {
            get
            {
                return CommonUtil.TextDate(this.MaintenanceDate);
            }
        }

        public string ContractCode { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? InstructionDate { get; set; }
        public DateTime? ExpectedMaintenanceDate { get; set; }
        public DateTime? MaintenanceDate { get; set; }
    }
}
