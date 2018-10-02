using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using System.Xml;
using SECOM_AJIS.DataEntity.Quotation;
using System.Reflection;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Quotation.Controllers
{
    /// <summary>
    /// Summary description for ImportFileHandler
    /// </summary>
    public class ImportFileHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                bool isSizeMoreThanLimit = false;
                HttpPostedFile uploadedfile = null;
                if (context.Request.Files.Count > 0)
                {
                    uploadedfile = context.Request.Files[0];
                    if (uploadedfile.InputStream.Length == 0)
                        uploadedfile = null;
                    else if (uploadedfile.InputStream.Length > 3000000)
                        isSizeMoreThanLimit = true;
                }

                if (uploadedfile == null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2017);
                }
                else if (isSizeMoreThanLimit)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2028);
                }
                else if (uploadedfile.FileName.ToUpper().EndsWith(".CSV") == false
                        || (uploadedfile.ContentType != "application/vnd.ms-excel"
                            && uploadedfile.ContentType != "text/comma-separated-values"
                            && uploadedfile.ContentType != "text/csv"
                            && uploadedfile.ContentType != "application/excel"
                            && uploadedfile.ContentType != "application/CSV"
                            && uploadedfile.ContentType != "application/vnd.msexcel"
                            && uploadedfile.ContentType != "application/octet-stream"))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2019);
                }

                if (res.IsError == false)
                {
                    System.IO.StreamReader rd = new System.IO.StreamReader(uploadedfile.InputStream, System.Text.Encoding.UTF8);
                    List<string> lines = new List<string>();
                    while (rd.EndOfStream == false)
                    {
                        lines.Add(rd.ReadLine());
                    }
                    rd.Close();

                    res.ResultData = lines;
                }
            }
            catch (Exception ex)
            {
                if (ex is HttpException)
                {
                    HttpException hex = ex as HttpException;
                    if (hex.WebEventCode == 3004)
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2028);
                }
                if (res.IsError == false)
                    res.AddErrorMessage(ex);
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(CommonUtil.CreateJsonString(res));
        }

        //public void ProcessRequest(HttpContext context)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    try
        //    {
        //        dsImportData importData = new dsImportData()
        //        {
        //            dtTbt_QuotationCustomer = new List<tbt_QuotationCustomer>(),
        //            dtTbt_QuotationSite =  new List<tbt_QuotationSite>(),
        //            dtTbt_QuotationTarget = new List<tbt_QuotationTarget>(),
        //            dtTbt_QuotationBasic = new List<tbt_QuotationBasic>(),
        //            dtTbt_QuotationOperationType = new List<tbt_QuotationOperationType>(),
        //            dtTbt_QuotationInstrumentDetails = new List<tbt_QuotationInstrumentDetails>(),
        //            dtTbt_QuotationFacilityDetails = new List<tbt_QuotationFacilityDetails>(),
        //            dtTbt_QuotationBeatGuardDetails = new List<tbt_QuotationBeatGuardDetails>(),
        //            dtTbt_QuotationSentryGuardDetails = new List<tbt_QuotationSentryGuardDetails>(),
        //            dtTbt_QuotationMaintenanceLinkage = new List<tbt_QuotationMaintenanceLinkage>()
        //        };
        //        List<object> impLst = new List<object>()
        //        {
        //            importData.dtTbt_QuotationCustomer,
        //            importData.dtTbt_QuotationSite,
        //            importData.dtTbt_QuotationTarget,
        //            importData.dtTbt_QuotationBasic,
        //            importData.dtTbt_QuotationOperationType,
        //            importData.dtTbt_QuotationInstrumentDetails,
        //            importData.dtTbt_QuotationFacilityDetails,
        //            importData.dtTbt_QuotationBeatGuardDetails,
        //            importData.dtTbt_QuotationSentryGuardDetails,
        //            importData.dtTbt_QuotationMaintenanceLinkage
        //        };

        //        HttpPostedFile uploadedfile = null;
        //        if (context.Request.Files.Count > 0)
        //        {
        //            uploadedfile = context.Request.Files[0];
        //            if (uploadedfile.InputStream.Length == 0)
        //                uploadedfile = null;
        //        }

        //        if (uploadedfile == null)
        //        {
        //            //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Quotation_MessageList.MSG2017);
        //            //res = ResultDataUtil.GetResultData(null, msg);
        //        }
        //        else
        //        {
        //            if (uploadedfile.ContentType != "application/vnd.ms-excel"
        //                    && uploadedfile.ContentType != "application/octet-stream")
        //            {
        //                //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Quotation_MessageList.MSG2019);
        //                //res = ResultDataUtil.GetResultData(null, msg);
        //            }
        //            else
        //            {
        //                #region Mapping Data

        //                System.IO.StreamReader rd = new System.IO.StreamReader(uploadedfile.InputStream, System.Text.Encoding.UTF8);
        //                List<string> lines = new List<string>();
        //                while (rd.EndOfStream == false)
        //                {
        //                    lines.Add(rd.ReadLine());
        //                }
        //                rd.Close();

        //                string filePath = CommonUtil.WebPath + SECOM_AJIS.Common.Util.ConstantValue.CommonValue.IMPORT_TEMPLATE_FILE;

        //                XmlDocument doc = new XmlDocument();
        //                doc.Load(filePath);
        //                XmlNodeList nodes = doc.SelectNodes("tables/table");

        //                bool isError = false;
        //                int lineIdx = 0;
        //                int nodeIdx = 0;
        //                for (; nodeIdx < nodes.Count; nodeIdx++)
        //                {
        //                    if (lineIdx < lines.Count)
        //                    {
        //                        /* --- Check Table name --- */
        //                        string[] tbName = lines[lineIdx].Split(",".ToCharArray());
        //                        if (nodes[nodeIdx].Attributes["name"].Value != tbName[0]
        //                            || lineIdx + 1 >= lines.Count)
        //                        {
        //                            isError = true;
        //                            break;
        //                        }

        //                        lineIdx += 1;

        //                        /* --- Check Column --- */
        //                        bool isSameCol = false;
        //                        string[] cols = lines[lineIdx].Split(",".ToCharArray());
        //                        if (cols != null)
        //                        {
        //                            if (nodes[nodeIdx].ChildNodes.Count <= cols.Length)
        //                            {
        //                                int colIdx = 0;
        //                                for (; colIdx < nodes[nodeIdx].ChildNodes.Count; colIdx++)
        //                                {
        //                                    if (cols[colIdx] != nodes[nodeIdx].ChildNodes[colIdx].Attributes["name"].Value)
        //                                        break;
        //                                }

        //                                if (colIdx == nodes[nodeIdx].ChildNodes.Count)
        //                                    isSameCol = true;
        //                            }
        //                        }
        //                        if (isSameCol == false)
        //                        {
        //                            isError = true;
        //                            break;
        //                        }

        //                        /* --- Get next Table --- */
        //                        string nextTable = null;
        //                        if (nodeIdx + 1 < nodes.Count)
        //                        {
        //                            nextTable = nodes[nodeIdx + 1].Attributes["name"].Value;
        //                        }

        //                        /* --- Loop fill data to each table --- */
        //                        lineIdx += 1;
        //                        while (lineIdx < lines.Count)
        //                        {
        //                            tbName = lines[lineIdx].Split(",".ToCharArray());
        //                            if (nextTable == tbName[0])
        //                                break;

        //                            string data = lines[lineIdx];

        //                            string[] lst = new string[nodes[nodeIdx].ChildNodes.Count];
        //                            for (int dIdx = 0; dIdx < nodes[nodeIdx].ChildNodes.Count; dIdx++)
        //                            {
        //                                if (data.Length <= 0 && dIdx < nodes[nodeIdx].ChildNodes.Count)
        //                                {
        //                                    isError = true;
        //                                    break;
        //                                }

        //                                int tIdx = 0;
        //                                int cmIdx = data.IndexOf(",");
        //                                int ccIdx = data.IndexOf("\"");

        //                                string val = string.Empty;
        //                                if (cmIdx < 0)
        //                                {
        //                                    val = data;
        //                                }
        //                                else if (cmIdx < ccIdx || ccIdx < 0)
        //                                {
        //                                    val = data.Substring(tIdx, cmIdx);
        //                                    tIdx += cmIdx + 1;
        //                                }
        //                                else
        //                                {
        //                                    int cceIdx = data.IndexOf("\"", ccIdx + 1);
        //                                    if (cceIdx <= 0)
        //                                        val = data;
        //                                    else
        //                                    {
        //                                        val = data.Substring(tIdx + 1, cceIdx - 1);
        //                                        tIdx += cceIdx + 2;
        //                                    }
        //                                }

        //                                lst[dIdx] = val;
        //                                data = data.Substring(tIdx);
        //                            }

        //                            lineIdx += 1;
        //                            if (isError)
        //                                break;
        //                            else
        //                            {
        //                                if (nodeIdx < impLst.Count)
        //                                {
        //                                    object obj = impLst[nodeIdx];

        //                                    /* --- Create Object --- */
        //                                    object objDo = Activator.CreateInstance(obj.GetType().GetGenericArguments()[0]);

        //                                    MethodInfo mf = obj.GetType().GetMethod("Add");
        //                                    if (mf != null)
        //                                        mf.Invoke(obj, new object[] { objDo });

        //                                    for (int colIdx = 0; colIdx < nodes[nodeIdx].ChildNodes.Count; colIdx++)
        //                                    {
        //                                        CommonUtil.SetObjectValue(objDo, nodes[nodeIdx].ChildNodes[colIdx].Attributes["name"].Value, lst[colIdx] != string.Empty? lst[colIdx] : null);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }

        //                    if (isError)
        //                        break;
        //                }

        //                if (nodeIdx < nodes.Count)
        //                {
        //                    isError = true;
        //                    //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Quotation_MessageList.MSG2020);
        //                    //res = ResultDataUtil.GetResultData(null, msg);
        //                }

        //                #endregion
        //                #region Check Mandatory

        //                if (isError == false)
        //                {
        //                    List<string> nullLst = new List<string>();

        //                    string screenID = context.Request["ScreenID"];
        //                    if (screenID == SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_QTN_TARGET)
        //                    {
        //                        if (importData.dtTbt_QuotationCustomer.Count == 0)
        //                            importData.dtTbt_QuotationCustomer.Add(new tbt_QuotationCustomer());
        //                        if (importData.dtTbt_QuotationSite.Count == 0)
        //                            importData.dtTbt_QuotationSite.Add(new tbt_QuotationSite());
        //                        if (importData.dtTbt_QuotationTarget.Count == 0)
        //                            importData.dtTbt_QuotationTarget.Add(new tbt_QuotationTarget());

        //                        /* --- Quotation Customer --- */
        //                        ObjectMandatoryField qcMf = new ObjectMandatoryField();
        //                        qcMf.AddProperty("CustPartTypeCode");
        //                        qcMf.AddProperty("CustCode");

        //                        ListMandatoryField qcLstMf = new ListMandatoryField();
        //                        qcLstMf.FieldName = "dtTbt_QuotationCustomer";
        //                        qcLstMf.MandatoryMessage = "#tbt_QuotationCustomer";
        //                        qcLstMf.Field = qcMf;

        //                        string[][] qcNullLst = CommonUtil.CheckMandatoryFiled(importData.dtTbt_QuotationCustomer, qcLstMf);
        //                        if (qcNullLst != null)
        //                        {
        //                            qcMf = new ObjectMandatoryField();
        //                            qcMf.AddProperty("CustNameEN");
        //                            qcMf.AddProperty("CustNameLC");
        //                            qcMf.AddProperty("CustTypeCode");

        //                            qcLstMf.Field = qcMf;

        //                            string[][] nullLst2 = CommonUtil.CheckMandatoryFiled(importData.dtTbt_QuotationCustomer, qcLstMf);
        //                            if (nullLst2 == null)
        //                                qcNullLst = null;
        //                        }
        //                        if (qcNullLst != null)
        //                            nullLst.AddRange(qcNullLst[0]);

        //                        /* --- Quotation Site --- */
        //                        ObjectMandatoryField qsMf = new ObjectMandatoryField();
        //                        qsMf.AddProperty("SiteCode");

        //                        ListMandatoryField qsLstMf = new ListMandatoryField();
        //                        qsLstMf.FieldName = "dtTbt_QuotationSite";
        //                        qsLstMf.MandatoryMessage = "#tbt_QuotationSite";
        //                        qsLstMf.Field = qsMf;

        //                        string[][] qsNullLst = CommonUtil.CheckMandatoryFiled(importData.dtTbt_QuotationSite, qsLstMf);
        //                        if (qsNullLst != null)
        //                        {
        //                            qsMf = new ObjectMandatoryField();
        //                            qsMf.AddProperty("SiteNameEN");
        //                            qsMf.AddProperty("SiteNameLC");
        //                            qsMf.AddProperty("AddressEN");
        //                            qsMf.AddProperty("AddressLC");
        //                            qsMf.AddProperty("RoadEN");
        //                            qsMf.AddProperty("RoadLC");
        //                            qsMf.AddProperty("SubDistrictEN");
        //                            qsMf.AddProperty("SubDistrictLC");
        //                            qsMf.AddProperty("Usage");
        //                            qsMf.AddProperty("ProvinceCode");
        //                            qsMf.AddProperty("DistrictCode");

        //                            qsLstMf.Field = qsMf;

        //                            string[][] nullLst2 = CommonUtil.CheckMandatoryFiled(importData.dtTbt_QuotationSite, qsLstMf);
        //                            if (nullLst2 == null)
        //                                qsNullLst = null;
        //                        }
        //                        if (qsNullLst != null)
        //                            nullLst.AddRange(qsNullLst[0]);

        //                        /* --- Quotation Target --- */
        //                        ObjectMandatoryField qtMf = new ObjectMandatoryField();
        //                        qtMf.AddProperty("ProductType");
        //                        qtMf.AddProperty("QuotationOfficeCode");

        //                        ListMandatoryField qtLstMf = new ListMandatoryField();
        //                        qtLstMf.FieldName = "dtTbt_QuotationTarget";
        //                        qtLstMf.MandatoryMessage = "#tbt_QuotationTarget";
        //                        qtLstMf.Field = qtMf;

        //                        string[][] qtNullLst = CommonUtil.CheckMandatoryFiled(importData.dtTbt_QuotationTarget, qtLstMf);
        //                        if (qtNullLst != null)
        //                            nullLst.AddRange(qtNullLst[0]);
        //                    }
        //                    else
        //                    {
        //                        if (importData.dtTbt_QuotationBasic.Count == 0)
        //                            importData.dtTbt_QuotationBasic.Add(new tbt_QuotationBasic());

        //                        /* --- Quotation Basic --- */
        //                        ObjectMandatoryField qbMf = new ObjectMandatoryField();
        //                        qbMf.AddProperty("QuotationTargetCode");
        //                        qbMf.AddProperty("ProductCode");

        //                        ListMandatoryField qbLstMf = new ListMandatoryField();
        //                        qbLstMf.FieldName = "dtTbt_QuotationBasic";
        //                        qbLstMf.MandatoryMessage = "#tbt_QuotationBasic";
        //                        qbLstMf.Field = qbMf;

        //                        string[][] qbNullLst = CommonUtil.CheckMandatoryFiled(importData.dtTbt_QuotationBasic, qbLstMf);
        //                        if (qbNullLst != null)
        //                            nullLst.AddRange(qbNullLst[0]);
        //                    }

        //                    if (nullLst != null)
        //                    {
        //                        if (nullLst.Count > 0)
        //                        {
        //                            //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Common_MessageList.MSG0007, CommonUtil.TextList(nullLst.ToArray()));
        //                            //res = ResultDataUtil.GetResultData(null, msg);
        //                            //isError = true;
        //                        }
        //                    }
        //                }

        //                #endregion
        //                #region Business Check

        //                if (isError == false)
        //                {
        //                    string screenID = context.Request["ScreenID"];
        //                    if (screenID == SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_QTN_TARGET)
        //                    {
        //                        bool isDataNotComplete = false;
        //                        bool isFoundTarget = false;
        //                        bool isFoundReal = false;
        //                        if (importData.dtTbt_QuotationCustomer.Count > 0 && importData.dtTbt_QuotationCustomer.Count <= 2)
        //                        {
        //                            foreach (tbt_QuotationCustomer cust in importData.dtTbt_QuotationCustomer)
        //                            {
        //                                if (isFoundTarget == false
        //                                    && cust.CustPartTypeCode == SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET)
        //                                    isFoundTarget = true;
        //                                if (isFoundReal == false
        //                                    && cust.CustPartTypeCode == SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_REAL_CUST)
        //                                    isFoundReal = true;
        //                                if (isDataNotComplete == false)
        //                                {
        //                                    if (cust.CustCode != null)
        //                                    {
        //                                        string[][] nullLst = CommonUtil.CheckMandatoryFiled(cust,
        //                                            "CustNameEN",
        //                                            "CustNameLC",
        //                                            "RepPersonName",
        //                                            "ContactPersonName",
        //                                            "SECOMContactPerson",
        //                                            "CustTypeCode",
        //                                            "CompanyTypeCode",
        //                                            "FinancialMarketTypeCode",
        //                                            "BusinessTypeCode",
        //                                            "PhoneNo",
        //                                            "IDNo",
        //                                            "RegionCode",
        //                                            "URL",
        //                                            "AddressEN",
        //                                            "AlleyEN",
        //                                            "RoadEN",
        //                                            "SubDistrictEN",
        //                                            "AddressLC",
        //                                            "AlleyLC",
        //                                            "RoadLC",
        //                                            "SubDistrictLC",
        //                                            "DistrictCode",
        //                                            "ProvinceCode",
        //                                            "ZipCode");

        //                                        if (nullLst != null)
        //                                        {
        //                                            if (nullLst[0].Length < 24)
        //                                                isDataNotComplete = true;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                            if (isFoundTarget == false && isFoundReal == false)
        //                            {
        //                                //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Quotation_MessageList.MSG2025);
        //                                //res = ResultDataUtil.GetResultData(null, msg);
        //                                //isError = true;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Quotation_MessageList.MSG2026);
        //                            //res = ResultDataUtil.GetResultData(null, msg);
        //                            isError = true;
        //                        }

        //                        if (isError == false && isFoundTarget == false)
        //                        {
        //                            //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Quotation_MessageList.MSG2024);
        //                            //res = ResultDataUtil.GetResultData(null, msg);
        //                            isError = true;
        //                        }
        //                        if (isError == false && isDataNotComplete == true)
        //                        {
        //                            //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Quotation_MessageList.MSG2021);
        //                            //res = ResultDataUtil.GetResultData(null, msg);
        //                            isError = true;
        //                        }
        //                        if (isError == false)
        //                        {
        //                            isError = true;
        //                            if (importData.dtTbt_QuotationSite != null)
        //                            {
        //                                if (importData.dtTbt_QuotationSite.Count > 0)
        //                                {
        //                                    if (importData.dtTbt_QuotationSite[0].SiteCode == null)
        //                                    {
        //                                        isError = false;
        //                                    }
        //                                    else
        //                                    {
        //                                        string[][] nullLst = CommonUtil.CheckMandatoryFiled(importData.dtTbt_QuotationSite[0],
        //                                            "SiteNameEN",
        //                                            "SiteNameLC",
        //                                            "SECOMContactPerson",
        //                                            "PersonInCharge",
        //                                            "PhoneNo",
        //                                            "BuildingUsageCode",
        //                                            "AddressEN",
        //                                            "AlleyEN",
        //                                            "RoadEN",
        //                                            "SubDistrictEN",
        //                                            "AddressLC",
        //                                            "AlleyLC",
        //                                            "RoadLC",
        //                                            "SubDistrictLC",
        //                                            "DistrictCode",
        //                                            "SubDistrictEN",
        //                                            "ProvinceCode",
        //                                            "ZipCode");

        //                                        isError = false;
        //                                        if (nullLst != null)
        //                                        {
        //                                            if (nullLst[0].Length < 18)
        //                                            {
        //                                                isError = true;
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }

        //                            if (isError)
        //                            {
        //                                //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Quotation_MessageList.MSG2022);
        //                                //res = ResultDataUtil.GetResultData(null, msg);
        //                            }
        //                        }
        //                    }
        //                }

        //                #endregion
        //                #region Data Authority Check

        //                if (isError == false)
        //                {
        //                    string screenID = context.Request["ScreenID"];
        //                    if (screenID != SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_QTN_TARGET)
        //                    {
        //                        IQuotationHandler handler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
        //                        if (handler != null)
        //                        {
        //                            if (importData.dtTbt_QuotationBasic != null)
        //                            {
        //                                if (importData.dtTbt_QuotationBasic.Count > 0)
        //                                {
        //                                    isError = true;
        //                                    doGetQuotationDataCondition cond = new doQuotationDataCondition()
        //                                    {
        //                                        QuotationTargetCode = importData.dtTbt_QuotationBasic[0].QuotationTargetCode
        //                                    };
        //                                    List<tbt_QuotationTarget> lst = handler.GetTbt_QuotationTarget(cond);
        //                                    if (lst != null)
        //                                    {
        //                                        if (lst.Count > 0)
        //                                            isError = false;
        //                                    }
        //                                    if (isError)
        //                                    {
        //                                        //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Quotation_MessageList.MSG2003,
        //                                        //    new string[]{importData.dtTbt_QuotationBasic[0].QuotationTargetCode});
        //                                        //res = ResultDataUtil.GetResultData(null, msg);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    if (isError == false)
        //                    {
        //                        string QuotationOfficeCode = null;
        //                        if (importData.dtTbt_QuotationTarget != null)
        //                        {
        //                            if (importData.dtTbt_QuotationTarget.Count > 0)
        //                                QuotationOfficeCode = importData.dtTbt_QuotationTarget[0].QuotationOfficeCode;
        //                        }
        //                        if (QuotationOfficeCode != null)
        //                        {
        //                            isError = true;

        //                            string[] officeLst = null;
        //                            if (context.Request["dtOfficeData"] != null)
        //                                officeLst = context.Request["dtOfficeData"].Split(",".ToCharArray());
        //                            if (officeLst != null)
        //                            {
        //                                if (officeLst.Contains<string>(QuotationOfficeCode) == true)
        //                                    isError = false;
        //                            }
        //                        }
        //                        if (isError)
        //                        {
        //                            //MessageModel msg = MessageUtil.GetMessage(MessageUtil.Quotation_MessageList.MSG2023);
        //                            //res = ResultDataUtil.GetResultData(null, msg);
        //                        }
        //                    }
        //                }

        //                #endregion
        //            }
        //        }

        //        if (res.IsError == false)
        //        {
        //            string screenID = context.Request["ScreenID"];
        //            if (screenID == SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_QTN_TARGET)
        //            {
        //                doInitImportData initImportDo = new doInitImportData();

        //                foreach (tbt_QuotationCustomer cust in importData.dtTbt_QuotationCustomer)
        //                {
        //                    if (cust.CustPartTypeCode == SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET)
        //                    {
        //                        initImportDo.doContractTargetData = CommonUtil.CloneObject<tbt_QuotationCustomer, doCustomer>(cust);
        //                    }
        //                    else if (cust.CustPartTypeCode == SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_REAL_CUST)
        //                    {
        //                        initImportDo.doRealCustomerData = CommonUtil.CloneObject<tbt_QuotationCustomer, doCustomer>(cust);
        //                    }
        //                }

        //                if (importData.dtTbt_QuotationSite != null)
        //                {
        //                    if (importData.dtTbt_QuotationSite.Count > 0)
        //                    {
        //                        initImportDo.doQuotationSiteData = CommonUtil.CloneObject<tbt_QuotationSite, View_dtSite>(importData.dtTbt_QuotationSite[0]);
        //                    }
        //                }

        //                if (importData.dtTbt_QuotationTarget != null)
        //                {
        //                    if (importData.dtTbt_QuotationTarget.Count > 0)
        //                    {
        //                        initImportDo.doQuotationTargetData = importData.dtTbt_QuotationTarget[0];
        //                    }
        //                }

        //                res.ResultData = initImportDo;
        //            }
        //            else
        //            {
        //                res.ResultData = importData;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //res = ResultDataUtil.GetResultData(ex);
        //    }

        //    context.Response.ContentType = "text/plain";
        //    context.Response.Write(res.ToJson);
        //}

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}