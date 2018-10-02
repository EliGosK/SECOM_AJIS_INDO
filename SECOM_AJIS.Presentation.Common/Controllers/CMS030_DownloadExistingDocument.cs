//*********************************
// Create by: Narupon W.
// Create date: 20/Jun/2010
// Update date: 20/Jun/2010
//*********************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Models;


using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Common.Models;
using System.Transactions;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS030
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS030_Authority(CMS030_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_DOWNLOAD_DOC, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            return InitialScreenEnvironment<CMS030_ScreenParameter>("CMS030", param, res);

        }

        /// <summary>
        /// Method for return view of screen CMS030
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS030")]
        public ActionResult CMS030()
        {
            return View();
        }

        /// <summary>
        /// Initial grid of screen CMS030
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS030()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS030", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial grid (mode 1) of screen CMS030
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS030_Mode1()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS030_Mode1", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial grid (mode 2) of screen CMS030
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS030_Mode2()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS030_Mode2", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial grid (mode 3) of screen CMS030
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS030_Mode3()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS030_Mode3", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial grid (mode 4) of screen CMS030
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS030_Mode4()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS030_Mode4", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial grid (mode 5) of screen CMS030
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS030_Mode5()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS030_Mode5", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial grid (mode 6) of screen CMS030
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS030_Mode6()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS030_Mode6", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial grid (mode 7) of screen CMS030
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS030_Mode7()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS030_Mode7_CMR020", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Initial grid (mode 8) of screen CMS030
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS030_Mode8()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS030_Mode8", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Initial grid (mode 9) of screen BLR060
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CMS030_Mode9()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS030_Mode9", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Method for validate search condition of screen CMS030
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS030_Validate(doDocumentDataCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            if (ModelState.IsValid == false)
            {
                res.ResultData = false;
                ValidatorUtil.BuildErrorMessage(res, this);
                if (res.IsError)
                    return Json(res);
            }
            else
            {
                res.ResultData = true;
            }

            return Json(res);
        }

        /// <summary>
        /// Method for convert document no. (one of search condition)
        /// </summary>
        /// <param name="strDocumentTypeNo"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        private string ConvertDocumentNo(string strDocumentTypeNo, string ctype)
        {
            if (CommonUtil.IsNullOrEmpty(strDocumentTypeNo) == false)
            {
                char[] separator = { '-' };
                string[] strsDocumentNo = strDocumentTypeNo.Split(separator);

                if (strsDocumentNo.Length > 0)
                {
                    CommonUtil cm = new CommonUtil(); // strsDocumentNo[0] = ContractCode or QuotationTargetCode
                    if (ctype == CommonUtil.CONVERT_TYPE.TO_LONG.ToString())
                    {
                        strsDocumentNo[0] = cm.ConvertContractCode(strsDocumentNo[0], CommonUtil.CONVERT_TYPE.TO_LONG);
                    }
                    else
                    {
                        strsDocumentNo[0] = cm.ConvertContractCode(strsDocumentNo[0], CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                    strDocumentTypeNo = string.Empty;
                    for (int i = 0; i < strsDocumentNo.Length; i++)
                    {
                        strDocumentTypeNo += strsDocumentNo[i];
                        if (i != strsDocumentNo.Length - 1)
                        {
                            strDocumentTypeNo += "-";
                        }
                    }
                }

            }

            return strDocumentTypeNo;
        }

        /// <summary>
        /// Get searach result of document data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CMS030_SearchResponse(doDocumentDataCondition cond)
        {

            CommonUtil c = new CommonUtil();
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<dtDocumentData> list = new List<dtDocumentData>();
            //List<dtDocumentData> vw_list = new List<dtDocumentData>();

            try
            {
                cond.QuotationTargetCode = c.ConvertQuotationTargetCode(cond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                //cond.ProjectCode = c.ConvertProjectCode(cond.ProjectCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                cond.ContractCode = c.ConvertContractCode(cond.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                cond.BillingTargetCode = c.ConvertBillingTargetCode(cond.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                if (cond.DocumentType == DocumentType.C_DOCUMENT_TYPE_CONTRACT)
                {
                    cond.DocumentNo = ConvertDocumentNo(cond.DocumentNo, CommonUtil.CONVERT_TYPE.TO_LONG.ToString());
                }

                if (cond.DocumentType == DocumentType.C_DOCUMENT_TYPE_INCOME)
                {
                    if (cond.DocumentCode == "BLR060")
                    {
                        List<string> fieldName = new List<string>();
                        List<string> controlName = new List<string>();
                        List<string> realFieldName = new List<string>();

                        if (CommonUtil.IsNullOrEmpty(cond.GenerateDateFrom))
                        {
                            fieldName.Add("lblGenerateDate");
                            controlName.Add("GenerateDateFrom");
                        }

                        if (CommonUtil.IsNullOrEmpty(cond.GenerateDateTo))
                        {
                            fieldName.Add("lblGenerateDate");
                            controlName.Add("GenerateDateTo");
                        }

                        foreach (var rawFieldName in fieldName.Distinct())
                        {
                            realFieldName.Add(CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS030", rawFieldName));
                        }

                        if ((realFieldName.Count > 0) || (controlName.Count > 0))
                        {
                            if ((realFieldName.Count > 0) && (controlName.Count > 0))
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, realFieldName.ToArray(), controlName.ToArray());
                            }

                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return Json(res);
                        }
                    }
                }



                if (cond.DocumentType == null || cond.DocumentCode == null)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0098);

                }
                else
                {
                    ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                    IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                    list = handler.GetDocumentDataList(cond);

                    //vw_list = CommonUtil.ConvertObjectbyLanguage<dtDocumentData, dtDocumentData>(list,
                    //                            "ConOfficeCodeName",
                    //                            "OperOfficeCodeName",
                    //                            "BillOfficeCodeName",
                    //                            "IssueOfficeCodeName",
                    //                            "DocumentName"
                    //                            );


                    // Misc Mapping  
                    MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                    miscMapping.AddMiscType(list.ToArray());
                    handlerCommon.MiscTypeMappingList(miscMapping);

                    // Language Mapping
                    CommonUtil.MappingObjectLanguage<dtDocumentData>(list);

                    if (cond.DocumentType == DocumentType.C_DOCUMENT_TYPE_CONTRACT)
                    {
                        foreach (var item in list)
                        {
                            item.DocumentNo = ConvertDocumentNo(item.DocumentNo, CommonUtil.CONVERT_TYPE.TO_SHORT.ToString());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                list = new List<dtDocumentData>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);

            }

            if (cond.Mode == 0)
            {
                res.ResultData = CommonUtil.ConvertToXml<dtDocumentData>(list, "Common\\CMS030", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            else if (cond.Mode == 1)
            {
                res.ResultData = CommonUtil.ConvertToXml<dtDocumentData>(list, "Common\\CMS030_Mode1", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            else if (cond.Mode == 2)
            {
                res.ResultData = CommonUtil.ConvertToXml<dtDocumentData>(list, "Common\\CMS030_Mode2", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            else if (cond.Mode == 3)
            {
                res.ResultData = CommonUtil.ConvertToXml<dtDocumentData>(list, "Common\\CMS030_Mode3", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            else if (cond.Mode == 4)
            {
                res.ResultData = CommonUtil.ConvertToXml<dtDocumentData>(list, "Common\\CMS030_Mode4", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            else if (cond.Mode == 5)
            {
                res.ResultData = CommonUtil.ConvertToXml<dtDocumentData>(list, "Common\\CMS030_Mode5", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            else if (cond.Mode == 6)
            {
                res.ResultData = CommonUtil.ConvertToXml<dtDocumentData>(list, "Common\\CMS030_Mode6", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            else if (cond.Mode == 7)
            {
                res.ResultData = CommonUtil.ConvertToXml<dtDocumentData>(list, "Common\\CMS030_Mode7_CMR020", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            else if (cond.Mode == 8)
            {
                res.ResultData = CommonUtil.ConvertToXml<dtDocumentData>(list, "Common\\CMS030_Mode8", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            else if (cond.Mode == 9)
            {
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                var datas = billingHandler.GetRptInvoiceIssueList(cond.GenerateDateFrom, cond.GenerateDateTo);

                List<dtDocumentData> ls = new List<dtDocumentData>();

                if (datas != null && datas.Count > 0)
                {
                    ls.Add(new dtDocumentData()
                    {
                        GenerateDateFrom = cond.GenerateDateFrom,
                        GenerateDateTo = cond.GenerateDateTo
                    });
                }

                res.ResultData = CommonUtil.ConvertToXml<dtDocumentData>(ls, "Common\\CMS030_Mode9", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }

            return Json(res);
        }

        /// <summary>
        /// Method for get document name by document code
        /// </summary>
        /// <param name="strDocumentCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CMS030_GetDocumentNoName(string strDocumentCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                var list = handler.GetDocumentNoNameByDocumentCode(strDocumentCode);

                foreach (var item in list)
                {
                    if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        item.DocumentNoName = item.DocumentNoNameEN;
                    }
                    else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                    {
                        item.DocumentNoName = item.DocumentNoNameJP;
                    }
                    else
                    {
                        item.DocumentNoName = item.DocumentNoNameLC;
                    }

                }

                return Json(list);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);

            }
        }

        /// <summary>
        /// Method for get document name by document type
        /// </summary>
        /// <param name="strDocumentType"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDocumentName(string strDocumentType)
        {
            string strDisplayName = "DocumentNameEN";
            List<string> lsObjectID = new List<string>();
            foreach (var item in CommonUtil.dsTransData.dtUserPermissionData.Values)
            {
                lsObjectID.Add(item.ObjectID);
            }

            string strObjectIDList = CommonUtil.CreateCSVString(lsObjectID);


            try
            {
                IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                List<dtDocumentNameDataList> lst = handler.GetDocumentNameDataList(strDocumentType, strObjectIDList);

                // 1. Connect string DocumentCode:DocumentName
                // 2. Select language



                //foreach (var item in lst)
                //{
                //    item.DocumentNameEN = CommonUtil.TextCodeName(item.DocumentCode, item.DocumentNameEN);
                //    item.DocumentNameJP = CommonUtil.TextCodeName(item.DocumentCode, item.DocumentNameJP);
                //    item.DocumentNameLC = CommonUtil.TextCodeName(item.DocumentCode, item.DocumentNameLC);

                //}

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "DocumentNameEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "DocumentNameJP";
                }
                else
                {
                    strDisplayName = "DocumentNameLC";
                }

                if (lst != null && lst.Count > 0)
                {
                    if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        lst = (from t in lst
                               orderby t.DocumentNameEN
                               select t).ToList<dtDocumentNameDataList>();
                    }
                    else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                    {
                        lst = (from t in lst
                               orderby t.DocumentNameJP
                               select t).ToList<dtDocumentNameDataList>();
                    }
                    else
                    {
                        lst = (from t in lst
                               orderby t.DocumentNameLC
                               select t).ToList<dtDocumentNameDataList>();
                    }

                }


                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<dtDocumentNameDataList>(lst, strDisplayName, "DocumentCode", true, CommonUtil.eFirstElementType.Select);

                return Json(cboModel);
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Mothod for get Issue Office by document type
        /// </summary>
        /// <param name="strDocumentType"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetIssueOfficeCode(string strDocumentType)
        {
            string strDisplayName = "OfficeCodeName";
            try
            {

                List<OfficeDataDo> lsOffice = new List<OfficeDataDo>();


                if (strDocumentType == DocumentType.C_DOCUMENT_TYPE_INSTALLATION) // where t.FunctionSecurity == FunctionSecurity.C_FUNC_SECURITY_NO
                {

                    lsOffice = (from t in CommonUtil.dsTransData.dtOfficeData
                                where t.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO
                                select t).ToList<OfficeDataDo>();


                }
                else if (strDocumentType == DocumentType.C_DOCUMENT_TYPE_INVENTORY) // where t.FunctionLogistic == FunctionLogistic.C_FUNC_LOGISTIC_WH
                {

                    lsOffice = (from t in CommonUtil.dsTransData.dtOfficeData
                                where t.FunctionLogistic != FunctionLogistic.C_FUNC_LOGISTIC_NO
                                select t).ToList<OfficeDataDo>();


                }



                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<OfficeDataDo>(lsOffice, strDisplayName, "OfficeCode", true, CommonUtil.eFirstElementType.All);

                return Json(cboModel);
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
        /// <param name="strDocumentNo"></param>
        /// <param name="documentOCC"></param>
        /// <param name="strDocumentCode"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult CMS030_DownloadPdfAndWriteLog(string strDocumentNo, string documentOCC, string strDocumentCode, string fileName)
        {
            ObjectResultData res = new ObjectResultData();
            CMS030_ScreenParameter sParam = GetScreenObject<CMS030_ScreenParameter>(); //Add by Jutarat A. on 17082012

            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    // doDocumentDownloadLog
                    doDocumentDownloadLog cond = new doDocumentDownloadLog();
                    cond.DocumentNo = strDocumentNo;
                    cond.DocumentCode = strDocumentCode;
                    cond.DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    cond.DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    cond.DocumentOCC = documentOCC;

                    ILogHandler handlerLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    int isOK = handlerLog.WriteDocumentDownloadLog(cond);

                    //Update firstIssueFlag for invoice report
                    if (cond.DocumentCode == ReportID.C_REPORT_ID_INVOICE)
                    {
                        IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                        billingHandler.UpdateFirstIssue(cond.DocumentNo, cond.DocumentOCC, cond.DownloadDate.Value, CommonUtil.dsTransData.dtUserData.EmpNo);
                    }

                    IDocumentHandler handlerDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                    Stream reportStream = handlerDoc.GetDocumentReportFileStream(fileName);

                    FileInfo fileinfo = new FileInfo(fileName);
                    if (fileinfo.Extension.ToUpper().Equals(".CSV"))
                    {
                        //Modify by Jutarat A. on 17082012
                        //FileStreamResult result = File(reportStream, "text/csv");
                        //result.FileDownloadName = fileinfo.Name;
                        //t.Complete();
                        //return result;
                        sParam.FileName = fileinfo.Name;
                        sParam.StreamReport = reportStream;
                        res.ResultData = true;

                        t.Complete();
                        return Json(res);
                        //End Modify
                    }
                    else
                    {
                        t.Complete();
                        return File(reportStream, "application/pdf");
                    }
                }
                catch (Exception ex)
                {
                    t.Dispose();
                    res.AddErrorMessage(ex);
                    return Json(res);
                }
            }
        }

        /// <summary>
        /// Download the result list as CSV file when click [Download] button on Grid
        /// </summary>
        public void CMS030_DownloadAsCSV()
        {
            CMS030_ScreenParameter sParam = GetScreenObject<CMS030_ScreenParameter>();
            MemoryStream msStreamReport = new MemoryStream();
            sParam.StreamReport.CopyTo(msStreamReport);

            this.DownloadCSVFile(sParam.FileName, msStreamReport.ToArray());
        }

        /// <summary>
        /// Check exist file before download file
        /// </summary>
        /// <param name="strDocumentNo"></param>
        /// <param name="documentOCC"></param>
        /// <param name="strDocumentCode"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult CMS030_CheckExistFile(string strDocumentNo, string documentOCC, string strDocumentCode, string fileName)
        {
            try
            {
                string path = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, fileName);// ReportUtil.GetGeneratedReportPath(fileName);

                if (System.IO.File.Exists(path) == true)
                {
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }


                // Old version

                /*
                IDocumentHandler handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                List<int?> list = handler.IsExistReport(strDocumentNo, documentOCC, strDocumentCode);
                if (list[0].HasValue)
                {
                    if (list[0].Value > 0)
                    {
                        return Json(1);
                    }
                    else
                    {
                        return Json(0);
                    }
                }
                else
                {
                    return Json(0);
                }
                
                */
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }



        }

        /// <summary>
        /// Get data operation office combobox for CMS030
        /// </summary>
        /// <param name="DocumentType"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CMS030_GetOperationOffice(string strDocumentType)
        {
            List<OfficeDataDo> nLst = new List<OfficeDataDo>();
            string strDisplayName = "OfficeCodeName";

            try
            {
                List<OfficeDataDo> lst = (from p in CommonUtil.dsTransData.dtOfficeData
                                          where p.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO
                                          orderby p.OfficeCode ascending
                                          select p).ToList<OfficeDataDo>();

                if (strDocumentType == DocumentType.C_DOCUMENT_TYPE_INSTALLATION) {
                    ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                    {
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_PROJECT_OFFICE_DUMMY,
                            ValueCode = "%"
                        }
                    };

                    List<doMiscTypeCode> doMiscs = handlerCommon.GetMiscTypeCodeList(miscs);

                    if (doMiscs != null && doMiscs.Count > 0)
                    {
                        OfficeDataDo tmpOffice = new OfficeDataDo();
                        tmpOffice.OfficeCode = doMiscs[0].ValueCode;
                        tmpOffice.OfficeName = doMiscs[0].ValueDisplay;
                        tmpOffice.OfficeNameEN = doMiscs[0].ValueDisplayEN;
                        tmpOffice.OfficeNameLC = doMiscs[0].ValueDisplayLC;
                        tmpOffice.OfficeNameJP = doMiscs[0].ValueDisplayJP;
                        lst.Add(tmpOffice);
                    }
                }

                lst = (from p in lst orderby p.OfficeCode ascending
                       select p).ToList<OfficeDataDo>();

                nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);


                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<OfficeDataDo>(nLst, strDisplayName, "OfficeCode");

                return Json(cboModel);
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        public void CMW030_DownloadBLR060CsvReport(DateTime? GenerateDateFrom, DateTime? GenerateDateTo)
        {
            ObjectResultData res = new ObjectResultData();
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

            var datas = billingHandler.GetRptInvoiceIssueList(GenerateDateFrom, GenerateDateTo);

            MemoryStream csv = CSVCreator.CreateCSVSteam<doGetRptInvoiceIssueList>(datas, "Common\\CMS030_BLR060Csv");
            string fName = string.Format("{0}BLR060_{1}-{2}.csv"
                ,DateTime.Now.ToString("yyyyMM"), GenerateDateFrom.Value.ToString("yyyyMMdd"), GenerateDateTo.Value.ToString("yyyyMMdd"));
            this.DownloadCSVFile(fName, csv.ToArray());
        }

        //Add MA By Pachara S. 29032017
        public void CMS030_DownloadCTR950CsvReport(DateTime? GenerateDateFrom, DateTime? GenerateDateTo)
        {
            ObjectResultData res = new ObjectResultData();
            IContractHandler contractHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;

            var datas = contractHandler.GetUnreceivedContractDocuemntCTR095(GenerateDateFrom, GenerateDateTo);

            MemoryStream csv = CSVCreator.CreateCSVSteam<dtUnreceivedContractDocuemntCTR095>(datas, "Common\\CMS030_CTR095Csv");
            string fName = null;
            if (GenerateDateFrom != null && GenerateDateTo != null)
            {
                fName = string.Format("UnreceivedContractDocuemnt_{0}-{1}.csv", GenerateDateFrom.Value.ToString("yyyyMMdd"), GenerateDateTo.Value.ToString("yyyyMMdd"));
            }
            if (GenerateDateFrom == null && GenerateDateTo == null)
            {
                fName = string.Format("UnreceivedContractDocuemnt.csv");
            }
            if (GenerateDateFrom == null && GenerateDateTo != null)
            {
                fName = string.Format("UnreceivedContractDocuemnt_-{0}.csv", GenerateDateTo.Value.ToString("yyyyMMdd"));
            }
            if (GenerateDateFrom != null && GenerateDateTo == null)
            {
                fName = string.Format("UnreceivedContractDocuemnt_{0}-.csv", GenerateDateFrom.Value.ToString("yyyyMMdd"));
            }
            this.DownloadCSVFile(fName, csv.ToArray());
        }

    }
}
