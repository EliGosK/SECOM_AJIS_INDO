using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Transactions;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Configuration;
using System.Printing;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Billing;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.ActionFilters;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of CMS450
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS460_Authority(CMS460_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

                //Check permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_REPRINT_BILLING_RELATED_DOCUMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS460_ScreenParameter>("CMS460", param, res);
        }

        /// <summary>
        /// Initialize screen of CMS460
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS460")]
        public ActionResult CMS460()
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil cm = new CommonUtil();
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Reprint Data and get document list for printing
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult CMS460_ReprintData(CMS460_ScreenInputValidate data)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {

                //Check permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_REPRINT_BILLING_RELATED_DOCUMENT, FunctionID.C_FUNC_ID_VIEW) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007);
                    return Json(res);
                }

                //Check Require field
                if (ModelState.IsValid == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                ILogHandler WriteLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler; //Add by Jutarat A. on 17092013

                List<dtDocumentListForPrining> lst = handler.GetDocumentListForPrining(DocumentType.C_DOCUMENT_TYPE_INCOME, DocumentType.C_DOCUMENT_TYPE_COMMON, data.IssueDate, data.ManagementNoFrom, data.ManagementNoTo);
                if (lst == null || lst.Count <= 0)
                {
                    res.ResultData = false;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }
                else
                {
                    //WriteLog.SearchFoxitProcess("CMS460"); //Add by Jutarat A. on 20082013

                    //Add by Jutarat A. on 17092013
                    string strErrorMessage = string.Empty;
                    bool bResult = comHandler.AllocatePrintingProcess(PrintingFlag.C_PRINTING_FLAG_CMS460, ScreenID.C_SCREEN_ID_REPRINT_BILLING_RELATED_DOCUMENT, ref strErrorMessage);
                    if (bResult)
                    //End Add
                    {
                        foreach (dtDocumentListForPrining doclist in lst)
                        {
                            // doDocumentDownloadLog
                            doDocumentDownloadLog cond = new doDocumentDownloadLog();
                            cond.DocumentNo = doclist.DocumentNo;
                            cond.DocumentCode = doclist.DocumentCode;
                            cond.DocumentOCC = doclist.DocumentOCC;
                            cond.DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            cond.DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo;


                            int isSuccess = WriteLog.WriteDocumentDownloadLog(cond);

                            if (doclist.DocumentCode == ReportID.C_REPORT_ID_ISSUE_LIST)
                            {
                                // print per page

                                string temPath = PathUtil.GetTempFileName(".pdf");
                                FileInfo fileInfo = new FileInfo(temPath);

                                if (Directory.Exists(fileInfo.DirectoryName) == false)
                                {
                                    Directory.CreateDirectory(fileInfo.DirectoryName);
                                }

                                int? PageFrom = 0;
                                int? PageTo = 0;
                                if (data.ManagementNoTo <= doclist.MaxManagementNo)
                                {
                                    if (data.ManagementNoFrom < doclist.MinManagementNo)
                                    {
                                        PageFrom = 1;
                                        PageTo = (data.ManagementNoTo - doclist.MinManagementNo) + 1;
                                    }
                                    else
                                    {
                                        PageFrom = (data.ManagementNoFrom - doclist.MinManagementNo) + 1;
                                        PageTo = (data.ManagementNoTo - doclist.MinManagementNo) + 1;
                                    }
                                }
                                else
                                {
                                    if (data.ManagementNoFrom < doclist.MinManagementNo)
                                    {
                                        PageFrom = 1;
                                        PageTo = (doclist.MaxManagementNo - doclist.MinManagementNo) + 1;
                                    }
                                    else
                                    {
                                        PageFrom = (data.ManagementNoFrom - doclist.MinManagementNo) + 1;
                                        PageTo = (doclist.MaxManagementNo - doclist.MinManagementNo) + 1;
                                    }
                                }

                                bool iSuccess = ReportUtil.PDFSplitPage(doclist.FilePathDocument, (PageFrom ?? 0), (PageTo ?? 0), temPath);

                                if (iSuccess)
                                {
                                    //PrintPDF(temPath);
                                    comHandler.PrintPDF(temPath); //Modify by Jutarat A. on 17092013
                                }

                                System.IO.File.Delete(temPath);

                            }
                            else
                            {
                                FileInfo fileInfo = new FileInfo(doclist.FilePathDocument);
                                if (fileInfo.Exists)
                                {
                                    //PrintPDF(doclist.FilePathDocument);
                                    comHandler.PrintPDF(doclist.FilePathDocument); //Modify by Jutarat A. on 17092013
                                }
                            }
                        }

                        bResult = comHandler.ResetPrintingProcess(ProcessID.C_PROCESS_ID_PRINTING_SERVICE); //Add by Jutarat A. on 17092013
                    }
                    //Add by Jutarat A. on 17092013
                    else
                    {
                        res.ResultData = false;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0160);
                        return Json(res);
                    }
                    //End Add

                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
            return Json(res);

        }

        /// <summary>
        /// Printing PDF 
        /// </summary>
        /// <param name="strPathFilename"></param>
        public void PrintPDF(string strPathFilename)
        {
            ILogHandler WriteLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = ConfigurationManager.AppSettings["PrintPDFFoxit"];
                //process.StartInfo.Arguments = "/p " + @strPathFilename;
                process.StartInfo.Arguments = string.Format("/t \"{0}\" \"{1}\"", strPathFilename, ConfigurationManager.AppSettings["PrinterName"]);
                process.StartInfo.Verb = "Print";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                //WriteLog.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, string.Format("PrintPDF error: {0} {1}" , ex.Message , ex.InnerException == null ? "" : ex.InnerException.Message) );
                throw ex;
            }

        }

    }
}
