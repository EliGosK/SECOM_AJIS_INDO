using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CSI.WindsorHelper;
//using System.Transactions;
using System.IO;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Accounting.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Accounting.Handlers;
using SECOM_AJIS.DataEntity.Accounting;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.Presentation.Accounting.Controllers
{   
    public partial class AccountingController: BaseController
    {
        public ActionResult ACS010_Authority(ACS010_ScreenParameter param)
        {

            ObjectResultData res = new ObjectResultData();

            try
            {

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_IN_STOCK_REPORT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    //res.ResultData = MessageUtil.MessageList.MSG0053.ToString();
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<ACS010_ScreenParameter>("ACS010", param, res);
        }

        [Initialize("ACS010")]
        public ActionResult ACS010()
        {
            return View();
        }

        public ActionResult GetDocumentTimingName(string documentCode)
        {
            AccountingHandler handler = new AccountingHandler();
            return Json(handler.getDocumentTimingByDocumentCode(documentCode));
        }


        /// <summary>
        ///ACS010_CheckSearchReqField
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public ActionResult ACS010_CheckSearchReqField(ACS010_Search searchCondition)
        {
            ObjectResultData res = new ObjectResultData();
            List<MessageModel> wLst = new List<MessageModel>();
            try
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { searchCondition });

                if (res.IsError)
                {
                    return Json(res);
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        public ActionResult ACS010_CheckGenerateReqField(ACS010_Generate generateCondition)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                AccountingHandler handler = new AccountingHandler();
                List<dtAccountingDocument> documents = handler.GetAccountingDocument(generateCondition.documentCode);

                //Validate
                if (documents[0].DocumentTimingType == "D2")
                {
                    if (CommonUtil.IsNullOrEmpty(generateCondition.generateTargetFrom) || CommonUtil.IsNullOrEmpty(generateCondition.generateTargetTo))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_ACCOUNTING,
                                            ScreenID.C_SCREEN_ID_OTHER_ACCOUNTING_REPORT,
                                            MessageUtil.MODULE_ACCOUNTING,
                                            MessageUtil.MessageList.MSG8001,
                                            new string[] { "lblTargetPeriod" },
                                            new string[] { "GenerateTargetFrom" });
                        return Json(res);
                    }
                }
                else
                {
                    if (CommonUtil.IsNullOrEmpty(generateCondition.generateTargetTo))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_ACCOUNTING,
                                            ScreenID.C_SCREEN_ID_OTHER_ACCOUNTING_REPORT,
                                            MessageUtil.MODULE_ACCOUNTING,
                                            MessageUtil.MessageList.MSG8001,
                                            new string[] { "lblTargetPeriod" },
                                            new string[] { "GenerateTargetTo" });
                        return Json(res);
                    }
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        ///ACS010_Search
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public ActionResult ACS010_Search(ACS010_Search searchCondition)
        {
            CommonUtil c = new CommonUtil();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<dtAccountingDocumentList> list = new List<dtAccountingDocumentList>();

            AccountingHandler handler = new AccountingHandler();

            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

            try
            {
                var officeItem = officehandler.GetTbm_Office(CommonUtil.dsTransData.dtUserData.MainOfficeCode);
                list = handler.GetAccountingDocumentList(searchCondition, officeItem[0].HQCode);

            }
            catch (Exception ex)
            {
                list = new List<dtAccountingDocumentList>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            res.ResultData = CommonUtil.ConvertToXml<dtAccountingDocumentList>(list, "Accounting\\ACS010", CommonUtil.GRID_EMPTY_TYPE.SEARCH);

            return Json(res);
        }

        public ActionResult ACS010_GenerateReport(ACS010_Generate generateCondition)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<dtAccountingDocumentList> documentList = new List<dtAccountingDocumentList>();

            try
            {
                List<tbm_Office> list = new List<tbm_Office>();
                AccountingHandler handler = new AccountingHandler();
                IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                var officeItem = officehandler.GetTbm_Office(CommonUtil.dsTransData.dtUserData.MainOfficeCode);

                List<dtAccountingDocument> documents = handler.GetAccountingDocument(generateCondition.documentCode);

                //Generate
                DocumentContext context = new DocumentContext();
                context.DocumentCode = documents[0].DocumentCode;
                context.DocumentGeneratorName = documents[0].DocumentGeneratorName;
                context.UserID = CommonUtil.dsTransData.dtUserData.EmpNo;
                context.DocumentTimingTypeDesc = documents[0].DocumentTimingTypeDesc;
                context.GenerateDate = DateTime.Now;
                context.TargetPeriodFrom = generateCondition.generateTargetFrom;
                context.TargetPeriodTo = generateCondition.generateTargetTo;
                context.UserHQCode = officeItem[0].HQCode;
                doGenerateDocumentResult result = DocumentGenerator.Generate(context);
                //Success
                if (result.ErrorFlag == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;

                    res.AddErrorMessage(MessageUtil.MODULE_ACCOUNTING,
                                        ScreenID.C_SCREEN_ID_OTHER_ACCOUNTING_REPORT,
                                        MessageUtil.MODULE_ACCOUNTING,
                                        MessageUtil.MessageList.MSG8004,
                                        null,
                                        new string[] { "AccountingGenerateDocument" });

                    //Result list

                    ACS010_Search searchCondition = new ACS010_Search();

                    searchCondition.SearchDocumentCode = result.DocumentContext.DocumentCode;
                    searchCondition.SearchDocumentNo = result.ResultDocumentNoList;

                    documentList = handler.GetAccountingDocumentList(searchCondition, result.DocumentContext.UserHQCode);

                }
                //Fail
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_ACCOUNTING,
                                        ScreenID.C_SCREEN_ID_OTHER_ACCOUNTING_REPORT,
                                        MessageUtil.MODULE_ACCOUNTING,
                                        result.ErrorCode,
                                        null,
                                        new string[] { "AccountingGenerateDocument" });
                }

            }
            catch (Exception ex)
            {
                documentList = new List<dtAccountingDocumentList>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }
            res.ResultData = CommonUtil.ConvertToXml<dtAccountingDocumentList>(documentList, "Accounting\\ACS010", CommonUtil.GRID_EMPTY_TYPE.SEARCH);

            return Json(res);

        }

        /// <summary>
        /// Initial grid of screen ACS010
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_ACS010()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Accounting\\ACS010", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Check exist file before download file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult ACS010_CheckExistFile(string fileName)
        {
            try
            {
                ACS010_ScreenParameter sParam = GetScreenObject<ACS010_ScreenParameter>();
                sParam.FileName = string.Empty;
                sParam.StreamReport = null;
                string path = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, fileName);

                if (System.IO.File.Exists(path) == true)
                {
                    sParam.FileName = fileName;
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Mothod for download document (PDF) and write history to download log
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult ACS010_DownloadDocument()
        {
            ObjectResultData res = new ObjectResultData();
            ACS010_ScreenParameter sParam = GetScreenObject<ACS010_ScreenParameter>();

            try
            {
                IDocumentHandler handlerDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                FileInfo fileinfo = new FileInfo(sParam.FileName);
                string fileExtension = fileinfo.Extension.ToUpper();

                Stream reportStream = handlerDoc.GetDocumentReportFileStream(sParam.FileName);

                if (fileExtension.Equals(".CSV"))
                {

                    sParam.StreamReport = reportStream;
                    res.ResultData = true;
                    return Json(res);
                }
                else
                {
                    return File(reportStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileinfo.Name);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Download the result list as CSV file when click [Download] button on Grid
        /// </summary>
        public void ACS010_DownloadAsCSV()
        {
            ACS010_ScreenParameter sParam = GetScreenObject<ACS010_ScreenParameter>();
            MemoryStream msStreamReport = new MemoryStream();
            sParam.StreamReport.CopyTo(msStreamReport);

            this.DownloadCSVFile(sParam.FileName, msStreamReport.ToArray());
        }
    }
}
