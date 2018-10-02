using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SECOM_AJIS.Common;
using System.Net;
using System.Net.Mail;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Util.ConstantValue;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CSI.WindsorHelper;

using System.Threading;



namespace SECOM_AJIS.DataEntity.Common
{
    public class DocumentHandler : BizCMDataEntities, IDocumentHandler
    {
        private static Dictionary<string, Semaphore> _dicTemplatePools = new Dictionary<string, Semaphore>();

        private bool _isUseOldCompanyTemplate = false;

        /// <summary>
        /// Used to limit the concurrent accesses to a template file by 3 sessions.
        /// This function block the current thread until at least one slot was freed.
        /// </summary>
        /// <param name="strTemplatePath">Name/Filepath of report template file.</param>
        public static void ReserveTemplatePool(string strTemplatePath)
        {
            Semaphore pool;

            lock (_dicTemplatePools)
            {
                if (!_dicTemplatePools.TryGetValue(strTemplatePath, out pool))
                {
                    pool = new Semaphore(3, 3);
                    _dicTemplatePools.Add(strTemplatePath, pool);
                }
            }

            pool.WaitOne();
        }

        /// <summary>
        /// Used to free a single of usage slot for the specify template report.
        /// </summary>
        /// <param name="strTemplatePath">Name/Filepath of report template file.</param>
        public static void ReleaseTemplatePool(string strTemplatePath)
        {
            try
            {
                Semaphore pool;
                if (_dicTemplatePools.TryGetValue(strTemplatePath, out pool))
                {
                    pool.Release();
                }
            }
            catch (Exception)
            {
                //Do Nothing
            }
        }

        /// <summary>
        /// Process - get all data of tbm_DocumantTemplate
        /// </summary>
        /// <param name="pchrServiceTypeCode"></param>
        /// <param name="bReportFlag"></param>
        /// <returns></returns>
        public override List<tbm_DocumentTemplate> GetTbm_DocumentTemplate(string pchrServiceTypeCode, bool? bReportFlag = null)
        {
            try
            {
                return base.GetTbm_DocumentTemplate(pchrServiceTypeCode, bReportFlag);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Process - get document data list
        /// </summary>
        /// <param name="C_DOCUMENT_TYPE"></param>
        /// <param name="ObjectIdList"></param>
        /// <returns></returns>
        public override List<dtDocumentType> GetDocumentTypeDataList(string C_DOCUMENT_TYPE, string ObjectIdList)
        {
            try
            {
                //ApplicationErrorException.CheckMandatoryField(C_DOCUMENT_TYPE, ObjectIdList);
                return base.GetDocumentTypeDataList(C_DOCUMENT_TYPE, ObjectIdList);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        /// <summary>
        /// Get document data by document code
        /// </summary>
        /// <param name="pchrDocumentCode"></param>
        /// <returns></returns>
        public override List<doDocumentNoName> GetDocumentNoNameByDocumentCode(string pchrDocumentCode)
        {
            try
            {
                // ApplicationErrorException.CheckMandatoryField(pchrDocumentCode);
                return base.GetDocumentNoNameByDocumentCode(pchrDocumentCode);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        /// <summary>
        /// Process - get document data list by search condition
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<dtDocumentData> GetDocumentDataList(doDocumentDataCondition cond, bool isCheckOfficeCode = true) //Modify (Add isCheckOfficeCode) by Jutarat A. on 11112013 
        {

            //Add by Jutarat A. on 11112013 
            if (isCheckOfficeCode == false)
            {
                cond.OfficeCodeList = null;
            }
            //End Add
            else
            {
                // --- Keep office code from dsTrans to cond.OfficeCodeList (in csv format) -- //
                List<string> lst = new List<string>();
                for (int i = 0; i < CommonUtil.dsTransData.dtOfficeData.Count; i++)
                {
                    lst.Add(CommonUtil.dsTransData.dtOfficeData[i].OfficeCode);
                }
                cond.OfficeCodeList = CommonUtil.CreateCSVString(lst);
            }

            try
            {
                return base.GetDocumentDataList(cond.DocumentType,
                                                cond.DocumentCode,
                                                cond.GenerateDateFrom,
                                                cond.GenerateDateTo,
                                                cond.Month,
                                                cond.Year,
                                                cond.ContractOfficeCode,
                                                cond.OperationOfficeCode,
                                                cond.BillingOfficeCode,
                                                cond.IssueOfficeCode,
                                                cond.DocumentNo,
                                                cond.QuotationTargetCode,
                                                cond.Alphabet,
                                                cond.ProjectCode,
                                                cond.ContractCode,
                                                cond.OCC,
                                                cond.BillingTargetCode,
                                                cond.InstrumentCode,
                                                DocumentType.C_DOCUMENT_TYPE_CONTRACT,
                                                DocumentType.C_DOCUMENT_TYPE_MA,
                                                DocumentType.C_DOCUMENT_TYPE_INSTALLATION,
                                                DocumentType.C_DOCUMENT_TYPE_INVENTORY,
                                                DocumentType.C_DOCUMENT_TYPE_INCOME,
                                                cond.OfficeCodeList,
                                                DocumentType.C_DOCUMENT_TYPE_COMMON,
                                                cond.LocationCode);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Test by Jutarat A. on 13112012
        private static Dictionary<string, ReportDocument> _dicReportDocumentList = new Dictionary<string, ReportDocument>();

        void IDocumentHandler.ClearReportCache()
        {
            DocumentHandler.ClearReportCache();
        }

        public static void ClearReportCache()
        {
            foreach (var rpt in _dicReportDocumentList)
            {
                if (rpt.Value.IsLoaded)
                {
                    rpt.Value.Close();
                }
                rpt.Value.Dispose();
            }
            _dicReportDocumentList.Clear();
        }

        public static ReportDocument GetReportDocument(string strDocumentCode)
        {
            ReportDocument rptDocument;

            lock (_dicReportDocumentList)
            {
                if (!_dicReportDocumentList.TryGetValue(strDocumentCode, out rptDocument))
                {
                    rptDocument = new ReportDocument();
                    _dicReportDocumentList.Add(strDocumentCode, rptDocument);
                }
            }

            return rptDocument;
        }
        //End Test

        //Comment by Jutarat A. on 21112013 (Move to delete temp file when Login)
        ////Add by Jutarat A. on 29102013
        /*public void DeleteTempFile(List<string> filePathList)
        {
            if (filePathList != null && filePathList.Count > 0)
            {
                foreach (string rpt in filePathList)
                {
                    if (File.Exists(rpt))
                        File.Delete(rpt);

                    string rptTemp = rpt.Replace(".pdf", "");
                    if (File.Exists(rptTemp))
                        File.Delete(rptTemp);

                    rptTemp = rptTemp.Replace(".tmp", "");
                    if (File.Exists(rptTemp))
                        File.Delete(rptTemp);
                }
            }
        }*/
        ////End Add
        //End Comment

        //---- Generate document process ----

        /// <summary>
        /// Generate report (retrun as file stream)
        /// </summary>
        /// <param name="doDoc"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        public Stream GenerateDocument(doDocumentDataGenerate doDoc, string owner_pwd = null)
        {
            string tempPath = GenerateReportPDF(doDoc);
            string outputPath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, doDoc.GeneratedReportName);// ReportUtil.GetGeneratedReportPath(doDoc.GeneratedReportName);

            //// Akat K. 2011-12-21 : prevent directory not found exception
            //FileInfo fileInfo = new FileInfo(outputPath);
            //if (Directory.Exists(fileInfo.DirectoryName) == false)
            //{
            //    Directory.CreateDirectory(fileInfo.DirectoryName);
            //}


            if (owner_pwd == null)
            {
                ReportUtil.EncryptPDF(tempPath, outputPath, DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            else
            {
                ReportUtil.EncryptPDF(tempPath, outputPath, owner_pwd);
            }

            Stream stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);

            //Comment by Jutarat A. on 21112013
            ////Add by Jutarat A. on 29102013
            //List<string> filePathList = new List<string>();
            //filePathList.Add(tempPath);

            //this.DeleteTempFile(filePathList);
            ////End Add
            //End Comment

            return stream;
        }
        /// <summary>
        /// Generate report (retrun as file stream)
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="doCover"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        public Stream GenerateDocument(doDocumentDataGenerate doMainDoc, doDocumentDataGenerate doCover, string owner_pwd = null)
        {
            try
            {
                string mainDocTempPath = GenerateReportPDF(doMainDoc);
                string coverDocTempPath = GenerateReportPDF(doCover, true);
                string outputFilePath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, doMainDoc.GeneratedReportName); // ReportUtil.GetGeneratedReportPath(doMainDoc.GeneratedReportName);
                string mergeTempFilePath = PathUtil.GetTempFileName(".pdf");

                if (String.IsNullOrEmpty(owner_pwd))
                {
                    owner_pwd = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                ReportUtil.MergePDF(coverDocTempPath, mainDocTempPath, mergeTempFilePath, true, outputFilePath, owner_pwd);

                Stream stream = new FileStream(outputFilePath, FileMode.Open, FileAccess.Read);

                //Comment by Jutarat A. on 21112013
                ////Add by Jutarat A. on 29102013
                //List<string> filePathList = new List<string>();
                //filePathList.Add(mainDocTempPath);
                //filePathList.Add(coverDocTempPath);
                //filePathList.Add(mergeTempFilePath);

                //this.DeleteTempFile(filePathList);
                ////End Add
                //End Comment

                return stream;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// Generate report (retrun as file stream)
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="lstSlaveDoc"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        public Stream GenerateDocument(doDocumentDataGenerate doMainDoc, List<doDocumentDataGenerate> lstSlaveDoc, string owner_pwd = null)
        {
            try
            {
                List<string> reportList = new List<string>();

                string mainDocTempPath = GenerateReportPDF(doMainDoc);
                reportList.Add(mainDocTempPath);

                string slaveDocTempPath = string.Empty;
                foreach (var doDoc in lstSlaveDoc)
                {
                    slaveDocTempPath = GenerateReportPDF(doDoc, true);
                    reportList.Add(slaveDocTempPath);
                }

                string outputFilePath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, doMainDoc.GeneratedReportName);// ReportUtil.GetGeneratedReportPath(doMainDoc.GeneratedReportName);


                string mergeTempFilePath = PathUtil.GetTempFileName(".pdf");

                if (String.IsNullOrEmpty(owner_pwd))
                {
                    owner_pwd = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                ReportUtil.MergePDF(reportList.ToArray(), mergeTempFilePath, true, outputFilePath, owner_pwd);

                Stream stream = new FileStream(outputFilePath, FileMode.Open, FileAccess.Read);

                //Comment by Jutarat A. on 21112013
                ////Add by Jutarat A. on 29102013
                //List<string> filePathList = new List<string>();
                //filePathList.AddRange(reportList);
                //filePathList.Add(mergeTempFilePath);

                //this.DeleteTempFile(filePathList);
                ////End Add
                //End Comment

                return stream;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Stream GenerateDocumentISR110(doDocumentDataGenerate doMainDoc, string owner_pwd = null)
        {
            try
            {
                List<string> reportList = new List<string>();

                string mainDocTempPath = GenerateReportPDF(doMainDoc);
                reportList.Add(mainDocTempPath);

                string outputFilePath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, doMainDoc.GeneratedReportName);// ReportUtil.GetGeneratedReportPath(doMainDoc.GeneratedReportName);


                string mergeTempFilePath = PathUtil.GetTempFileName(".pdf");

                if (String.IsNullOrEmpty(owner_pwd))
                {
                    owner_pwd = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                ReportUtil.MergePDF(reportList.ToArray(), mergeTempFilePath, true, outputFilePath, owner_pwd);

                Stream stream = new FileStream(outputFilePath, FileMode.Open, FileAccess.Read);

                //Comment by Jutarat A. on 21112013
                ////Add by Jutarat A. on 29102013
                //List<string> filePathList = new List<string>();
                //filePathList.AddRange(reportList);
                //filePathList.Add(mergeTempFilePath);

                //this.DeleteTempFile(filePathList);
                ////End Add
                //End Comment

                return stream;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //---- Generate document process ----

        /// <summary>
        /// Generate report (retrun as file path)
        /// </summary>
        /// <param name="doDoc"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        public string GenerateDocumentFilePath(doDocumentDataGenerate doDoc, string owner_pwd = null, bool isManageDocument = false)
        {
            string tempPath = GenerateReportPDF(doDoc, false, 0, isManageDocument); // start download cout with zero
            string outputPath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, doDoc.GeneratedReportName); //ReportUtil.GetGeneratedReportPath(doDoc.GeneratedReportName);

            //// Akat K. 2011-12-21 : prevent directory not found exception
            //FileInfo fileInfo = new FileInfo(outputPath);
            //if (Directory.Exists(fileInfo.DirectoryName) == false)
            //{
            //    Directory.CreateDirectory(fileInfo.DirectoryName);
            //}

            if (owner_pwd == null)
            {
                ReportUtil.EncryptPDF(tempPath, outputPath, DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            else
            {
                ReportUtil.EncryptPDF(tempPath, outputPath, owner_pwd);
            }

            //Comment by Jutarat A. on 21112013
            ////Add by Jutarat A. on 29102013
            //List<string> filePathList = new List<string>();
            //filePathList.Add(tempPath);

            //this.DeleteTempFile(filePathList);
            ////End Add
            //End Comment

            return outputPath;
        }

        /// <summary>
        /// Generate report (retrun as file path)
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="doCover"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        public string GenerateDocumentFilePath(doDocumentDataGenerate doMainDoc, doDocumentDataGenerate doCover, string owner_pwd = null)
        {
            try
            {
                string mainDocTempPath = GenerateReportPDF(doMainDoc, false, 0); // start download cout with zero
                string coverDocTempPath = GenerateReportPDF(doCover, true);
                string outputFilePath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, doMainDoc.GeneratedReportName); //ReportUtil.GetGeneratedReportPath(doMainDoc.GeneratedReportName);
                string mergeTempFilePath = PathUtil.GetTempFileName(".pdf");

                if (String.IsNullOrEmpty(owner_pwd))
                {
                    owner_pwd = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                ReportUtil.MergePDF(coverDocTempPath, mainDocTempPath, mergeTempFilePath, true, outputFilePath, owner_pwd);

                //Comment by Jutarat A. on 21112013
                ////Add by Jutarat A. on 29102013
                //List<string> filePathList = new List<string>();
                //filePathList.Add(mainDocTempPath);
                //filePathList.Add(coverDocTempPath);
                //filePathList.Add(mergeTempFilePath);

                //this.DeleteTempFile(filePathList);
                ////End Add
                //End Comment

                return outputFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// Generate report (retrun as file path)
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="lstSlaveDoc"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        public string GenerateDocumentFilePath(doDocumentDataGenerate doMainDoc, List<doDocumentDataGenerate> lstSlaveDoc, string owner_pwd = null, bool isReuseRptDoc = false) //Modify by Jutarat A. on 13112012 (Add isReuseRptDoc)
        {
            try
            {
                List<string> reportList = new List<string>();

                // Main report
                //string mainDocTempPath = GenerateReportPDF(doMainDoc, false, 0); // start download cout with zero
                string mainDocTempPath = GenerateReportPDF(doMainDoc, false, 0, false, isReuseRptDoc); //Modify by Jutarat A. on 13112021 (Add isReuseRptDoc)

                reportList.Add(mainDocTempPath);

                // Slave report
                string slaveDocTempPath = string.Empty;
                foreach (var doDoc in lstSlaveDoc)
                {
                    //slaveDocTempPath = GenerateReportPDF(doDoc, true);
                    slaveDocTempPath = GenerateReportPDF(doDoc, true, 1, false, isReuseRptDoc); //Modify by Jutarat A. on 13112021 (Add isReuseRptDoc)

                    reportList.Add(slaveDocTempPath);
                }

                string outputFilePath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, doMainDoc.GeneratedReportName); //ReportUtil.GetGeneratedReportPath(doMainDoc.GeneratedReportName);
                string mergeTempFilePath = PathUtil.GetTempFileName(".pdf");

                if (String.IsNullOrEmpty(owner_pwd))
                {
                    owner_pwd = DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                ReportUtil.MergePDF(reportList.ToArray(), mergeTempFilePath, true, outputFilePath, owner_pwd, true); //Modify by Jutarat A. on 14112012 (Add isClearTempFile)

                //Comment by Jutarat A. on 21112013
                ////Add by Jutarat A. on 29102013
                //List<string> filePathList = new List<string>();
                //filePathList.AddRange(reportList);
                //filePathList.Add(mergeTempFilePath);

                //this.DeleteTempFile(filePathList);
                ////End Add
                //End Comment

                return outputFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //---- Generate document process ----

        /// <summary>
        /// Generate report without encrypt output file
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <returns></returns>
        public string GenerateDocumentWithoutEncrypt(doDocumentDataGenerate doMainDoc)
        {
            string tempPath = GenerateReportPDF(doMainDoc);

            string outputPath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, doMainDoc.GeneratedReportName); // ReportUtil.GetGeneratedReportPath(doMainDoc.GeneratedReportName);

            // encrypt for file that save to server
            ReportUtil.EncryptPDF(tempPath, outputPath, DateTime.Now.ToString("yyyyMMddHHmmss"));

            return tempPath;
        }
        /// <summary>
        /// Generate report without encrypt output file
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="doCover"></param>
        /// <returns></returns>
        public string GenerateDocumentWithoutEncrypt(doDocumentDataGenerate doMainDoc, doDocumentDataGenerate doCover)
        {
            string mainDocTempPath = GenerateReportPDF(doMainDoc);
            string coverDocTempPath = GenerateReportPDF(doCover, true);
            string outputFilePath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, doMainDoc.GeneratedReportName); //ReportUtil.GetGeneratedReportPath(doMainDoc.GeneratedReportName);
            string mergeTempFilePath = PathUtil.GetTempFileName(".pdf");

            ReportUtil.MergePDF(coverDocTempPath, mainDocTempPath, mergeTempFilePath, false, outputFilePath, null);


            // encrypt for file that save to server
            ReportUtil.EncryptPDF(mergeTempFilePath, outputFilePath, DateTime.Now.ToString("yyyyMMddHHmmss"));

            //Comment by Jutarat A. on 21112013
            ////Add by Jutarat A. on 29102013
            //List<string> filePathList = new List<string>();
            //filePathList.Add(mainDocTempPath);
            //filePathList.Add(coverDocTempPath);

            //this.DeleteTempFile(filePathList);
            ////End Add
            //End Comment

            return mergeTempFilePath;
        }
        /// <summary>
        /// Generate report without encrypt output file
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="lstSlaveDoc"></param>
        /// <returns></returns>
        public string GenerateDocumentWithoutEncrypt(doDocumentDataGenerate doMainDoc, List<doDocumentDataGenerate> lstSlaveDoc)
        {
            try
            {
                List<string> reportList = new List<string>();

                // Main report
                string mainDocTempPath = GenerateReportPDF(doMainDoc, false, 0); // start download cout with zero
                reportList.Add(mainDocTempPath);

                // Slave report
                string slaveDocTempPath = string.Empty;
                foreach (var doDoc in lstSlaveDoc)
                {
                    slaveDocTempPath = GenerateReportPDF(doDoc, true);
                    reportList.Add(slaveDocTempPath);
                }

                string outputFilePath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, doMainDoc.GeneratedReportName); //ReportUtil.GetGeneratedReportPath(doMainDoc.GeneratedReportName);
                string mergeTempFilePath = PathUtil.GetTempFileName(".pdf");


                ReportUtil.MergePDF(reportList.ToArray(), mergeTempFilePath, false, outputFilePath, null);


                // encrypt for file that save to server
                ReportUtil.EncryptPDF(mergeTempFilePath, outputFilePath, DateTime.Now.ToString("yyyyMMddHHmmss"));

                //Comment by Jutarat A. on 21112013
                ////Add by Jutarat A. on 29102013
                //List<string> filePathList = new List<string>();
                //filePathList.AddRange(reportList);

                //this.DeleteTempFile(filePathList);
                ////End Add
                //End Comment

                return mergeTempFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// Generate document from report template , output file as pdf format (encrypted)
        /// </summary>
        /// <param name="doDoc"></param>
        /// <param name="isCoverPage"></param>
        /// <param name="initialDownloadCount"></param>
        /// <returns></returns>
        private string GenerateReportPDF(doDocumentDataGenerate doDoc, bool isCoverPage = false, int initialDownloadCount = 1, bool isManageDocument = false, bool isReuseRptDoc = true) //Modify by Jutarat A. on 13112012 (Add isReuseRptDoc)
        {
            string resultFileName = string.Empty;

            //string tempReportTemplate = string.Empty;

            // Akat K. 2011-10-27 : add mandatory check
            List<string> messageParam = new List<string>();
            if (doDoc.DocumentNo == null)
            {
                messageParam.Add("DocumentNo");
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, messageParam.ToArray<string>());
            }
            if (doDoc.DocumentCode == null)
            {
                messageParam.Add("DocumentCode");
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, messageParam.ToArray<string>());
            }

            if (doDoc.DocumentData == null)
            {
                messageParam.Add("DocumentData");
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, messageParam.ToArray<string>());
            }

            //Add by Jutarat A. on 13112012
            //ReportDocument rpt = new ReportDocument();
            ReportDocument rpt;
            if (isReuseRptDoc)
                rpt = DocumentHandler.GetReportDocument(doDoc.DocumentCode);
            else
                rpt = new ReportDocument();
            //End Add 

            lock (rpt)
            {
                LogHandler lh = new LogHandler();
                CommonHandler ch = new CommonHandler();

                // Set report template
                // Akat K. : modify for support document code and document template not the same
                var lst = base.GetReportTemplatePath(doDoc.GetDocumentTemplateCode);
                if (lst.Count == 0 || lst[0].FilePath == null || lst[0].FilePath == string.Empty)
                {

                    lh.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, LogMessage.C_LOG_REPORT_NOT_FOUND, EventID.C_EVENT_ID_REPORT_TEMPLATE_NOT_FOUND);

                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0064, doDoc.DocumentCode);
                }

                //string strReportTempletePath = ReportUtil.GetReportTemplatePath(lst[0].FilePath);
                string strReportTempletePath = PathUtil.GetPathValue(
                    (_isUseOldCompanyTemplate ? PathUtil.PathName.ReportTempatePathOldCompany : PathUtil.PathName.ReportTempatePath)
                    , lst[0].FilePath
                );
                try
                {
                    DocumentHandler.ReserveTemplatePool(strReportTempletePath);

                    //tempReportTemplate = PathUtil.GetTemporaryPath(Path.GetTempFileName() + ".rpt");

                    //File.Copy(strReportTempletePath, tempReportTemplate);

                    if (File.Exists(strReportTempletePath))
                    {
                        // tt
                        int iRetryLoadReport = 5;
                        do
                        {
                            try
                            {
                                rpt.Load(strReportTempletePath, OpenReportMethod.OpenReportByTempCopy);
                                //rpt.Load(strReportTempletePath);
                                //rpt.Load(tempReportTemplate);
                                break;
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("[GenerateReportPDF] Error while executing rpt.Load(), Error: {0}, RetryRemain: {1}", ex.Message, iRetryLoadReport);
                                if (ex.Message.Contains("The process cannot access the file because it is being used by another process") || ex.Message.Contains("Disk full"))
                                {
                                    Thread.Sleep(1000);
                                    continue;
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }
                        while (--iRetryLoadReport >= 0);
                    }
                    else
                    {
                        lh.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, LogMessage.C_LOG_REPORT_NOT_FOUND, EventID.C_EVENT_ID_REPORT_TEMPLATE_NOT_FOUND);
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0065, lst[0].FilePath);
                    }

                    // Set datasource (Main report)
                    if (rpt.Database.Tables.Count > 0)
                    {
                        rpt.DataSourceConnections.Clear(); //Add by Jutarat A. on 19062013
                        rpt.SetDataSource(doDoc.DocumentData);
                    }

                    // rptH.Subreports["CTR060_1"].SetDataSource(rptListDetail);
                    // Set datasource (Sub report)
                    if (doDoc.SubReportDataSource != null)
                    {
                        foreach (var item in doDoc.SubReportDataSource)
                        {
                            if (rpt.Subreports[item.SubReportName] != null)
                            {
                                rpt.Subreports[item.SubReportName].DataSourceConnections.Clear(); //Add by Jutarat A. on 19062013
                                rpt.Subreports[item.SubReportName].SetDataSource(item.Value);
                            }

                        }
                    }


                    // Set report parameter (Main report)
                    if (doDoc.MainReportParam != null)
                    {
                        foreach (var item in doDoc.MainReportParam)
                        {
                            if (rpt.ParameterFields[item.ParameterName] != null)
                            {
                                rpt.SetParameterValue(item.ParameterName, item.Value);

                            }

                        }
                    }


                    // rptH.SetParameterValue("AutoBillingTypeNone", AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_NONE, "CTR060_1");
                    // Set report parameter (Sub report)
                    if (doDoc.SubReportParam != null)
                    {
                        foreach (var item in doDoc.SubReportParam)
                        {
                            if (rpt.ParameterFields[item.ParameterName, item.SubReportName] != null)
                            {
                                rpt.SetParameterValue(item.ParameterName, item.Value, item.SubReportName);
                            }

                        }
                    }



                    // Set parameter field
                    if (rpt.ParameterFields["DocumentNo"] != null)
                    {
                        rpt.SetParameterValue("DocumentNo", doDoc.DocumentNo);
                    }

                    if (rpt.ParameterFields["Alphabet"] != null)
                    {
                        rpt.SetParameterValue("Alphabet", doDoc.OtherKey.Alphabet);
                    }
                    if (rpt.ParameterFields["BillingCode"] != null)
                    {
                        rpt.SetParameterValue("BillingCode", doDoc.OtherKey.BillingCode);
                    }
                    if (rpt.ParameterFields["BillingOffice"] != null)
                    {
                        rpt.SetParameterValue("BillingOffice", doDoc.OtherKey.BillingOffice);
                    }
                    if (rpt.ParameterFields["BillingTargetCode"] != null)
                    {
                        rpt.SetParameterValue("BillingTargetCode", doDoc.OtherKey.BillingTargetCode);
                    }
                    if (rpt.ParameterFields["ContactOffice"] != null)
                    {
                        rpt.SetParameterValue("ContactOffice", doDoc.OtherKey.ContractOffice);
                    }
                    if (rpt.ParameterFields["ContractCode"] != null)
                    {
                        rpt.SetParameterValue("ContractCode", doDoc.OtherKey.ContractCode);
                    }
                    if (rpt.ParameterFields["ContractOCC"] != null)
                    {
                        rpt.SetParameterValue("ContractOCC", doDoc.OtherKey.ContractOCC);
                    }
                    if (rpt.ParameterFields["InstallationSlipIssueOffice"] != null)
                    {
                        rpt.SetParameterValue("InstallationSlipIssueOffice", doDoc.OtherKey.InstallationSlipIssueOffice);
                    }
                    if (rpt.ParameterFields["InstrumentCode"] != null)
                    {
                        rpt.SetParameterValue("InstrumentCode", doDoc.OtherKey.InstrumentCode);
                    }
                    if (rpt.ParameterFields["InventorySlipIssueOffice"] != null)
                    {
                        rpt.SetParameterValue("InventorySlipIssueOffice", doDoc.OtherKey.InventorySlipIssueOffice);
                    }
                    if (rpt.ParameterFields["MonthYear"] != null)
                    {
                        rpt.SetParameterValue("MonthYear", doDoc.OtherKey.MonthYear);
                    }
                    if (rpt.ParameterFields["OperationOffice"] != null)
                    {
                        rpt.SetParameterValue("OperationOffice", doDoc.OtherKey.OperationOffice);
                    }
                    if (rpt.ParameterFields["ProjectCode"] != null)
                    {
                        rpt.SetParameterValue("ProjectCode", doDoc.OtherKey.ProjectCode);
                    }
                    if (rpt.ParameterFields["QuatationTargetCode"] != null)
                    {
                        rpt.SetParameterValue("QuatationTargetCode", doDoc.OtherKey.QuotationTargetCode);
                    }
                    if (rpt.ParameterFields["LocationCode"] != null)
                    {
                        rpt.SetParameterValue("LocationCode", doDoc.OtherKey.LocationCode);
                    }

                    // additional

                    if (rpt.ParameterFields["MinManagementNo"] != null)
                    {
                        rpt.SetParameterValue("MinManagementNo", doDoc.OtherKey.MinManagementNo);
                    }
                    if (rpt.ParameterFields["MaxManagementNo"] != null)
                    {
                        rpt.SetParameterValue("MaxManagementNo", doDoc.OtherKey.MaxManagementNo);
                    }

                    //Comment by Jutarat A. on 14112012 (Change to ExportToDisk)
                    //// ----------- Create tempolary file -----------
                    //Stream stream = null;

                    //int iRetryExport = 5;
                    //do
                    //{
                    //    try
                    //    {
                    //        stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
                    //        break;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        System.Diagnostics.Debug.WriteLine("[GenerateReportPDF] Error while executing rpt.ExportToStream(), Error: {0}, RetryRemain: {1}", ex.Message, iRetryExport);
                    //        if (ex.Message.Contains("The process cannot access the file because it is being used by another process") || ex.Message.Contains("Disk full"))
                    //        {
                    //            Thread.Sleep(1000);
                    //            continue;
                    //        }
                    //        else
                    //        {
                    //            throw;
                    //        }
                    //    }
                    //}
                    //while (--iRetryExport >= 0);

                    //byte[] brpt = SECOM_AJIS.Common.Util.ReportUtil.StreamToBytes(stream);
                    //stream.Dispose(); //Add by Jutarat A. on 12112012
                    //End Comment

                    resultFileName = PathUtil.GetTempFileName(".pdf");

                    FileInfo fileInfo = new FileInfo(resultFileName);

                    if (Directory.Exists(fileInfo.DirectoryName) == false)
                    {
                        Directory.CreateDirectory(fileInfo.DirectoryName);
                    }

                    //Modify by Jutarat A. on 14112012
                    //// Save as to backup report path (byte to file pdf)
                    //System.IO.File.WriteAllBytes(resultFileName, brpt);
                    int iRetryExport = 5;
                    do
                    {
                        try
                        {
                            rpt.ExportToDisk(ExportFormatType.PortableDocFormat, resultFileName);
                            break;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("[GenerateReportPDF] Error while executing rpt.ExportToDisk(), Error: {0}, RetryRemain: {1}", ex.Message, iRetryExport);
                            if (ex.Message.Contains("The process cannot access the file because it is being used by another process") || ex.Message.Contains("Disk full"))
                            {
                                Thread.Sleep(1000);
                                continue;
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                    while (--iRetryExport >= 0);
                    //End Modify

                    //Move to finally block.
                    //rpt.Close();

                    // tt
                    //File.Delete(tempReportTemplate);


                    // Keep record to tbm_DocumentList
                    if (isCoverPage == false)
                    {

                        // Narupon
                        if (isManageDocument)
                        {
                            // Get total PDF page
                            int iTotalPage = ReportUtil.GetTotalPageCount(fileInfo.FullName);

                            doDoc.OtherKey.MinManagementNo += 1;
                            doDoc.OtherKey.MaxManagementNo += iTotalPage;
                            doDoc.OtherKey.ManagementNo = doDoc.OtherKey.MinManagementNo;
                        }


                        // Insert int tbt_DocumentList
                        InsertDocumentList(doDoc, initialDownloadCount);

                    }

                }
                catch (Exception excep)
                {
                    throw excep;
                }
                finally
                {
                    if (isReuseRptDoc == false) //Add by Jutarat A. on 13112012
                    {
                        if (rpt != null)
                        {
                            if (rpt.IsLoaded)
                            {
                                rpt.Close();
                            }
                            rpt.Dispose();

                        }
                    }

                    DocumentHandler.ReleaseTemplatePool(strReportTempletePath);
                    GC.Collect(); //Add by Jutarat A. on 14112012
                }
            }

            return resultFileName;
        }
        /// <summary>
        /// Read physical file convert to file stream
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Stream GetDocumentReportFileStream(string filename)
        {
            try
            {
                string fullPath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, filename); //ReportUtil.GetGeneratedReportPath(filename);

                if (System.IO.File.Exists(fullPath) == true)
                {
                    return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Insert data to tbt_DocumentList
        /// </summary>
        /// <param name="doDoc"></param>
        /// <param name="initialDownloadCount"></param>
        public void InsertDocumentList(doDocumentDataGenerate doDoc, int initialDownloadCount = 1)
        {

            List<dtDocumentList> result = base.InsertDocumentList(doDoc.DocumentNo,
                                                                    doDoc.DocumentOCC,
                                                                    doDoc.DocumentCode,
                                                                    doDoc.OtherKey.ContractCode,
                                                                    doDoc.OtherKey.ContractOCC,
                                                                    doDoc.OtherKey.QuotationTargetCode,
                                                                    doDoc.OtherKey.Alphabet,
                                                                    doDoc.OtherKey.ProjectCode,
                                                                    doDoc.OtherKey.BillingTargetCode,
                                                                    doDoc.OtherKey.InstrumentCode,
                                                                    doDoc.OtherKey.ContractOffice,
                                                                    doDoc.OtherKey.OperationOffice,
                                                                    doDoc.OtherKey.BillingOffice,
                                                                    string.IsNullOrEmpty(doDoc.OtherKey.InstallationSlipIssueOffice) ? doDoc.OtherKey.InventorySlipIssueOffice : doDoc.OtherKey.InstallationSlipIssueOffice,
                                                                    doDoc.ProcessDateTime,
                                                                    doDoc.OtherKey.MonthYear.Month,
                                                                    doDoc.OtherKey.MonthYear.Year,
                                                                    doDoc.DocumentReportType == doDocumentDataGenerate.ReportType.Pdf ? doDoc.GeneratedReportName : doDoc.GeneratedCsvReportName,
                                                                    initialDownloadCount,
                                                                    doDoc.OtherKey.ManagementNo,
                                                                    doDoc.ProcessDateTime,
                                                                    doDoc.EmpNo,
                                                                    doDoc.ProcessDateTime,
                                                                    doDoc.EmpNo,
                                                                    doDoc.OtherKey.LocationCode,
                                                                    doDoc.OtherKey.MinManagementNo, // add 26 Mar 2012 (Narupon W.)
                                                                    doDoc.OtherKey.MaxManagementNo // add 26 Mar 2012 (Narupon W.)
                                                                    );
            if (result != null)
            {
                if (result.Count > 0)
                {
                    doDoc.DocumentOCC = result[0].DocumentOCC;
                }
            }

        }

        /// <summary>
        /// Get next issue list no.
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        public string GetNextIssueListNo(string nameCode)
        {
            string strIssueNo = string.Empty;
            var result = base.GetNextIssueListNo(nameCode);
            if (result.Count > 0)
            {
                strIssueNo = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMdd"), result[0].Value.ToString("00"));
            }
            return strIssueNo;
        }

        /// <summary>
        /// Process - generate report CMR010 - Issue list report 
        /// </summary>
        /// <param name="strIssueListNo"></param>
        /// <param name="BillingOffice"></param>
        /// <param name="maxManageNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        public Stream GenerateCMR010(string strIssueListNo, string BillingOffice, int maxManageNo, string strEmpNo, DateTime dtDateTime)
        {
            //Add by Jutarat A. on 04012013
            bool hasIssueList = false;
            bool isSuccess = false;
            //End Add

            //string strFilePath = GenerateCMR010FilePath(strIssueListNo, BillingOffice, maxManageNo, strEmpNo, dtDateTime);
            string strFilePath = GenerateCMR010FilePath(strIssueListNo, BillingOffice, maxManageNo, strEmpNo, dtDateTime, ref hasIssueList, ref isSuccess); //Modify by Jutarat A. on 04012013

            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }
        /// <summary>
        /// Process - generate report CMR010 - Issue list report 
        /// </summary>
        /// <param name="strIssueListNo"></param>
        /// <param name="BillingOffice"></param>
        /// <param name="maxManageNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        public string GenerateCMR010FilePath(string strIssueListNo, string BillingOffice, int? maxManageNo, string strEmpNo, DateTime dtDateTime, ref bool hasIssueList, ref bool isSuccess) //Modify by Jutarat A. (Add hasIssueList and isSuccess) on 04012013
        {

            List<dtIssueListData> dtIssueList = base.GetIssueList(DocumentType.C_DOCUMENT_TYPE_INCOME,
                                                                    ReportID.C_REPORT_ID_ISSUE_LIST,
                                                                     dtDateTime,
                                                                     BillingOffice,
                                                                     maxManageNo);
            if (dtIssueList.Count == 0)
            {
                return null;
            }

            hasIssueList = true; //Add by Jutarat A. on 04012013

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strIssueListNo;
            doDocument.DocumentCode = ReportID.C_REPORT_ID_ISSUE_LIST;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = dtIssueList;
            foreach (var dt in dtIssueList)
            {
                dt.MinManagementNo++;
            }
            doDocument.OtherKey.MinManagementNo = (maxManageNo ?? 0);
            doDocument.OtherKey.MaxManagementNo = (maxManageNo ?? 0);
            doDocument.OtherKey.BillingOffice = BillingOffice;
            doDocument.OtherKey.MonthYear = dtDateTime;


            // // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            string strFilePath = "";
            string outputPath = ""; //Add by Jutarat A. on 29102013
            try
            {
                // Old
                //strFilePath = this.GenerateDocumentFilePath(doDocument, null, true);

                // Narupon Edit 8/8/2012 (without encrypt)
                strFilePath = this.GenerateReportPDF(doDocument, false, 0, true);

                outputPath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, doDocument.GeneratedReportName);


                File.Copy(strFilePath, outputPath, true);



                // Update value of ManagementNo
                List<tbt_DocumentList> updateDocList = new List<tbt_DocumentList>();
                tbt_DocumentList updateDoc;
                int totalPage = ReportUtil.GetTotalPageCount(strFilePath);
                //For test 
                //int totalPage  = 1;
                foreach (var item in dtIssueList)
                {
                    //========= Teerapong 30/07/2012 ==========
                    int NewManagementNo;
                    if (item.ManagementNo == -1)
                    {
                        NewManagementNo = item.ManagementNo;
                    }
                    else
                    {
                        NewManagementNo = item.ManagementNo + totalPage;
                    }
                    //=========================================
                    updateDoc = new tbt_DocumentList()
                    {
                        DocumentNo = item.DocumentNo,
                        DocumentCode = item.DocumentCode,
                        DocumentOCC = item.DocumentOCC,
                        //ManagementNo = item.ManagementNo + totalPage,
                        ManagementNo = NewManagementNo,
                        UpdateBy = strEmpNo,
                        UpdateDate = dtDateTime
                    };

                    updateDocList.Add(updateDoc);
                }
                //Comment by budd
                //updateDocList.RemoveRange(1, updateDocList.Count - 1);
                string xml = CommonUtil.ConvertToXml_Store(updateDocList);

                // Update managementNo                
                base.UpdateManageNo(xml);

                isSuccess = true; //Add by Jutarat A. on 04012013

                //Comment by Jutarat A. on 21112013
                ////Add by Jutarat A. on 29102013
                //List<string> filePathList = new List<string>();
                //filePathList.Add(strFilePath);

                //this.DeleteTempFile(filePathList);
                ////End Add
                //End Comment
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return strFilePath;
            return outputPath; //Modify by Jutarat A. on 29102013
        }


        /// <summary>
        /// Generate report CMR020 - Account data of carry-over & Profit  (CSV format) to the server
        /// </summary>
        /// <param name="reportYear">year of data</param>
        /// <param name="reportMonth">month of data</param>
        /// <param name="productType">product type</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <returns></returns>
        public string GenerateCMR020FilePath(string reportYear, string reportMonth, string productType, string strEmpNo, DateTime dtDateTime)
        {
            return GenerateCMR020FilePath(reportYear, reportMonth, productType, strEmpNo, dtDateTime, true);
        }
        public string GenerateCMR020FilePath(string reportYear, string reportMonth, string productType, string strEmpNo, DateTime dtDateTime, bool isInsertDocumentList)
        {
            List<dtRptAccountDataOfCarryOverAndProfit> dtReport = base.GetRptAccountDataOfCarryOverAndProfit(reportYear, reportMonth, productType);
            if (dtReport.Count == 0)
                return null;

            //Format: 201206_N
            string documentNo = string.Format("{0}{1}_{2}", reportYear, reportMonth, productType);
            //Format: CMR020_201206_N.csv
            string csvFileName = string.Format("CMR020_{0}{1}_{2}.csv", reportYear, reportMonth, productType);
            //Format: 201206\CMR020_201206_N.csv
            string csvGenerateFilePath = string.Format(@"{0}{1}\{2}", reportYear, reportMonth, csvFileName);
            //Format: [Generated report path]\201207\N201207.csv
            string csvGenerateFullFilePath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, csvGenerateFilePath);

            if (File.Exists(csvGenerateFullFilePath))
                File.Delete(csvGenerateFullFilePath);

            //Convert to csv format and export
            string csvResultData = CSVReportUtil.GenerateCSVData<dtRptAccountDataOfCarryOverAndProfit>(dtReport, false, true);
            //TIS-620   support: thai,eng  open with office 2003
            using (StreamWriter sw = new StreamWriter(csvGenerateFullFilePath, false, Encoding.GetEncoding("TIS-620")))
            {
                sw.WriteLine(csvResultData);
                sw.Close();
            }


            if (isInsertDocumentList)
            {
                //Set document info
                doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = documentNo;
                doDocument.DocumentCode = ReportID.C_REPORT_ID_GEN_ACCOUNT_CARRY_OVER_AND_PROFIT;
                doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                doDocument.DocumentData = dtReport;
                doDocument.DocumentReportType = doDocumentDataGenerate.ReportType.Csv;
                //Other key
                doDocument.OtherKey.ContractCode = null;
                doDocument.OtherKey.QuotationTargetCode = null;
                doDocument.OtherKey.Alphabet = null;
                doDocument.OtherKey.ProjectCode = null;
                doDocument.OtherKey.BillingTargetCode = null;
                doDocument.OtherKey.InstrumentCode = null;
                doDocument.OtherKey.ContractOffice = null;
                doDocument.OtherKey.OperationOffice = null;
                doDocument.OtherKey.BillingOffice = null;
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.OtherKey.InventorySlipIssueOffice = null;
                doDocument.OtherKey.MonthYear = new DateTime(int.Parse(reportYear), int.Parse(reportMonth), 1);
                doDocument.GeneratedCsvReportName = csvGenerateFilePath;
                doDocument.OtherKey.MinManagementNo = 0;
                doDocument.OtherKey.MaxManagementNo = 0;
                doDocument.EmpNo = strEmpNo;
                doDocument.ProcessDateTime = dtDateTime;

                //Download count = 0
                InsertDocumentList(doDocument, 0);
            }

            return csvGenerateFullFilePath;
        }

        /// <summary>
        /// Generate text report document (common method)
        /// </summary>
        /// <param name="doDocument"></param>
        /// <param name="dtGenerateDate"></param>
        /// <param name="strFileName"></param>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public string GenerateTextReportFilePath(doDocumentDataGenerate doDocument, DateTime dtGenerateDate, string strFileName, string strContent)
        {
            string strGenerateFilePath = string.Format(@"{0}\{1}", dtGenerateDate.ToString("yyyyMM"), strFileName);
            string strGenerateFullFilePath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, strGenerateFilePath);

            //TIS-620   support: thai,eng  open with office 2003
            using (StreamWriter sw = new StreamWriter(strGenerateFullFilePath, false, Encoding.GetEncoding("TIS-620")))
            {
                sw.WriteLine(strContent);
                sw.Close();
            }

            if (doDocument != null)
            {
                doDocument.DocumentReportType = doDocumentDataGenerate.ReportType.Csv;
                doDocument.GeneratedCsvReportName = strGenerateFullFilePath;
                InsertDocumentList(doDocument, 0);
            }

            return strGenerateFullFilePath;
        }


 #region Re-Issue CMS490
        public string GetReIssueInvoice(string DocNo, string UpdateBy)
        {
            try
            {
                List<dtReIssueInvoice> dtReIssue = base.CM_ReIssue_Invoice(DocNo,UpdateBy);
                return dtReIssue[0].Msg.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetReIssueTaxInvoice(string DocNo, string UpdateBy)
        {
            try
            {
                List<dtReIssueTaxInvoice> dtReIssue = base.CM_ReIssue_TaxInvoice(DocNo,UpdateBy);
                return dtReIssue[0].Msg.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetReIssueCreditNote(string DocNo, string UpdateBy)
        {
            try
            {
                List<dtReIssueCreditNote> dtReIssue = base.CM_ReIssue_CreditNote(DocNo,UpdateBy);
                return dtReIssue[0].Msg.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetReIssueReceipt(string DocNo, string UpdateBy)
        {
            try
            {
                List<dtReIssueReceipt> dtReIssue = base.CM_ReIssue_Receipt(DocNo,UpdateBy);
                return dtReIssue[0].Msg.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#endregion

        public void UseOldCompanyTemplate(bool isUse)
        {
            _isUseOldCompanyTemplate = isUse;
        }
    }
}
