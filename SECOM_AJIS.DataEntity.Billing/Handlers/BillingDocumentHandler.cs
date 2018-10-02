using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;

using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;

using System.Transactions;

using CSI.WindsorHelper;
using SECOM_AJIS.Common;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;

using SECOM_AJIS.DataEntity.Billing.CustomEntity;
using SECOM_AJIS.Presentation.Common.Service;

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class BillingDocumentHandler : BizBLDataEntities, IBillingDocumentHandler
    {
        // BLR010- Invoice report

        /// <summary>
        /// Generate report BLR010- Invoice report
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        public Stream GenerateBLR010(string strInvoiceNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateBLR010FilePath(strInvoiceNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }
        /// <summary>
        ///  Generate report BLR010- Invoice report
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <param name="isUseForMerge"></param>
        /// <returns></returns>
        public string GenerateBLR010FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false)
        {
            try
            {
                // Test: strInvoiceNo = "201205A00121"

                string strFilePath = string.Empty;
                

                List<dtRptInvoice> blr010 = base.GetRptInvoice(strInvoiceNo, MiscType.C_SHOW_DUEDATE, ShowDueDate.C_SHOW_DUEDATE_7, ShowDueDate.C_SHOW_DUEDATE_30, ShowDueDate.C_SHOW_DUEDATE_14, ShowDueDate.C_SHOW_DUEDATE_NONE);

                if (blr010.Count == 0)
                {
                    return null;
                }

                // Add by Jirawat Jannet @ 2016-10-04
                #region Summary billing amount

                decimal sumAmount = 0;

                if (blr010[0].BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    sumAmount = blr010.Select(m => m.BillingAmount).Sum();
                else
                    sumAmount = blr010.Select(m => m.BillingAmountUsd).Sum();

                foreach (var item in blr010)
                    item.RPT_SumAmount = string.Format("{0} {1}", item.RPT_BillingAmountCurrencyName, sumAmount.ToString("N2"));

                #endregion


                // tt -> Order by ... (4/Sep/2012)
                if (blr010[0].SeparateInvType == SeparateInvType.C_SEP_INV_SORT_ASCE || blr010[0].SeparateInvType == SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_ASCE)
                {
                    blr010 = (from p in blr010 orderby p.SortingType ascending select p).ToList<dtRptInvoice>();
                }
                else if (blr010[0].SeparateInvType == SeparateInvType.C_SEP_INV_SORT_DESC || blr010[0].SeparateInvType == SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_DESC)
                {
                    blr010 = (from p in blr010 orderby p.SortingType descending select p).ToList<dtRptInvoice>();
                }
                else
                {
                    blr010 = blr010.OrderBy(a => a.ContractCode).ThenBy(b => b.BillingOCC).ThenBy(c => c.BillingDetailNo).ToList<dtRptInvoice>();
                }

                // If payment method is bank transfer then attach BLR030-PaymentForm also
                bool isAttachPaymentForm = false;
                if (blr010[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER)
                {
                    isAttachPaymentForm = true;
                }

                List<List<dtRptInvoice>> blr010_list = new List<List<dtRptInvoice>>();
                blr010_list.Add(blr010.Clone()); // for Original customer
                //blr010_list.Add(blr010.Clone()); // for Copy customer // Comment by Jirawat Jannet
                blr010_list.Add(blr010.Clone()); // for Copy account


                int itemPerPage = 10;

                for (int i = 0; i < blr010_list.Count; i++)
                {
                    //// Calcualate page   1st  10/p , >= 2nd 18/p
                    //int totalPage = ((blr010_list[i].Count - 1 + 10) / 18) + 1;

                    // tt
                    // Calcualate page   1st  10/p , >= 2nd 10/p
                    int totalPage = ((blr010_list[i].Count - 1 + itemPerPage) / itemPerPage) + 1;

                    for (int j = 0; j < blr010_list[i].Count; j++)
                    {
                        // Calcualate page   1st  10/p , >= 2nd 18/p
                        //blr010_list[i][j].Page = ((j + 10) / 18) + 1;

                        //
                        blr010_list[i][j].Page = ((j + itemPerPage) / itemPerPage) + 1;
                        blr010_list[i][j].TotalPage = totalPage;

                        // Update footer
                        if (i == 0)
                        {
                            blr010_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_EN;// + "\n" + InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_TH; Comment by Jirawat Jannet
                        }
                        // Comment by Jirawat Jannet
                        //else if (i == 1)
                        //{
                        //    blr010_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_COPY_CUST_EN + "\n" + InvoiceDocument.C_INVOICE_DOC_COPY_CUST_TH;
                        //}
                        else if (i == 1) //else if (i == 2) change by jirawat jannet
                        {
                            blr010_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_COPY_ACC_EN;// + "\n" + InvoiceDocument.C_INVOICE_DOC_COPY_ACC_TH; Comment by Jirawat Jannet
                        }

                    }


                }

                // Prepare List of doDocumentDataGenerate
                List<doDocumentDataGenerate> doMainDoc = new List<doDocumentDataGenerate>();
                List<doDocumentDataGenerate> doSlaveDoc = new List<doDocumentDataGenerate>();

                for (int i = 0; i < blr010_list.Count; i++)
                {
                    List<ReportParameterObject> reportParameter = new List<ReportParameterObject>();
                    ReportParameterObject visibleRecordParam = new ReportParameterObject()
                    {
                        ParameterName = "RPT_VisibleRecord"
                    };
                    ReportParameterObject param = new ReportParameterObject()
                    {
                        ParameterName = "C_PAYMENT_METHOD_BANK_TRANSFER",
                        Value = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                    };
                    reportParameter.Add(param);

                    doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                    doDocument.DocumentNo = strInvoiceNo;

                    if (blr010[0].BillingOfficeCode == OfficeCode.C_INV_OFFICE_INDO)
                    {
                        if (blr010[0].PaymentMethod == MethodType.C_PAYMENT_METHOD_AUTO_TRANSFER)
                        {
                            doDocument.DocumentCode = ReportID.C_REPORT_ID_INVOICE2;

                            // Add by Jirawat Jannet @ 2016-12-06
                            #region Set common data

                            foreach (var d in blr010)
                            {
                                var doc = DocumentOutput.getDocumentData("BLR020", 1, DateTime.Now.Date);
                                if(doc != null)
                                {
                                    d.RPT_SignatureImageFullPath = doc.ImageSignaturePath;
                                    d.RPT_CompanyName = doc.CompanyName;
                                    d.SignatureExecutive = doc.EmpPosition;
                                    d.SignatureName = doc.EmpName;
                                }
                                else
                                {
                                    d.RPT_SignatureImageFullPath = "";
                                    d.RPT_CompanyName = "";
                                    d.SignatureExecutive = "";
                                    d.SignatureName = "";
                                }
                                
                            }


                            #endregion
                        }
                        else
                        {
                            doDocument.DocumentCode = ReportID.C_REPORT_ID_INVOICE;

                            // Add by Jirawat Jannet @ 2016-12-06
                            #region Set common data

                            foreach (var d in blr010)
                            {
                                var doc = DocumentOutput.getDocumentData("BLR010", 1, DateTime.Now.Date);
                                if (doc != null)
                                {
                                    d.RPT_SignatureImageFullPath = doc.ImageSignaturePath;
                                    d.RPT_CompanyName = doc.CompanyName;
                                    d.SignatureExecutive = doc.EmpPosition;
                                    d.SignatureName = doc.EmpName;
                                }
                                else
                                {
                                    d.RPT_SignatureImageFullPath = "";
                                    d.RPT_CompanyName = "";
                                    d.SignatureExecutive = "";
                                    d.SignatureName = "";
                                }
                            }


                            #endregion
                        }

                        visibleRecordParam.Value = 6;
                    }
                    else
                    {
                        if (blr010[0].PaymentMethod == MethodType.C_PAYMENT_METHOD_AUTO_TRANSFER)
                        {
                            doDocument.DocumentCode = ReportID.C_REPORT_ID_INVOICE4;

                            // Add by Jirawat Jannet @ 2016-12-06
                            #region Set common data

                            foreach (var d in blr010)
                            {
                                var doc = DocumentOutput.getDocumentData("BLR040", 2, DateTime.Now.Date);
                                if (doc != null)
                                {
                                    d.RPT_SignatureImageFullPath = doc.ImageSignaturePath;
                                    d.RPT_CompanyName = doc.CompanyName;
                                    d.SignatureExecutive = doc.EmpPosition;
                                    d.SignatureName = doc.EmpName;
                                }
                                else
                                {
                                    d.RPT_SignatureImageFullPath = "";
                                    d.RPT_CompanyName = "";
                                    d.SignatureExecutive = "";
                                    d.SignatureName = "";
                                }
                            }


                            #endregion
                        }
                        else
                        {
                            doDocument.DocumentCode = ReportID.C_REPORT_ID_INVOICE3;

                            // Add by Jirawat Jannet @ 2016-12-06
                            #region Set common data

                            foreach (var d in blr010)
                            {
                                var doc = DocumentOutput.getDocumentData("BLR030", 2, DateTime.Now.Date);
                                if (doc != null)
                                {
                                    d.RPT_SignatureImageFullPath = doc.ImageSignaturePath;
                                    d.RPT_CompanyName = doc.CompanyName;
                                    d.SignatureExecutive = doc.EmpPosition;
                                    d.SignatureName = doc.EmpName;
                                }
                                else
                                {
                                    d.RPT_SignatureImageFullPath = "";
                                    d.RPT_CompanyName = "";
                                    d.SignatureExecutive = "";
                                    d.SignatureName = "";
                                }
                            }


                            #endregion
                        }

                        visibleRecordParam.Value = 9;
                    }

                    blr010_list[i][0].RPT_CompanyName = blr010[0].RPT_CompanyName;

                    reportParameter.Add(visibleRecordParam);


                    doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                    doDocument.DocumentData = blr010_list[i]; // **
                    doDocument.MainReportParam = reportParameter;

                    // OtherKey
                    doDocument.OtherKey.BillingOffice = blr010[0].BillingOfficeCode;
                    doDocument.OtherKey.BillingTargetCode = blr010[0].BillingTargetCode;

                    // Additional
                    doDocument.EmpNo = strEmpNo;
                    doDocument.ProcessDateTime = dtDateTime;

                    if (i == 0)
                    {
                        doMainDoc.Add(doDocument);
                    }
                    else
                    {
                        doSlaveDoc.Add(doDocument);
                    }
                }

                // Coment by Jirawat Jannet
                //if (isAttachPaymentForm)
                //{
                //    List<dtRptPaymentForm> blr030 = base.GetRptPaymentForm(strInvoiceNo);

                //    if (blr030.Count > 0)
                //    {
                //        doDocumentDataGenerate doDocument_BLR030 = new doDocumentDataGenerate();
                //        doDocument_BLR030.DocumentNo = strInvoiceNo;
                //        doDocument_BLR030.DocumentCode = ReportID.C_REPORT_ID_PAYMENT_FORM;
                //        doDocument_BLR030.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                //        doDocument_BLR030.DocumentData = blr030;

                //        // OtherKey
                //        doDocument_BLR030.OtherKey.BillingOffice = blr030[0].BillingOfficeCode;
                //        doDocument_BLR030.OtherKey.BillingTargetCode = blr030[0].BillingTargetCode;


                //        // Additional
                //        doDocument_BLR030.EmpNo = strEmpNo;
                //        doDocument_BLR030.ProcessDateTime = dtDateTime;

                //        doSlaveDoc.Add(doDocument_BLR030);
                //    }

                //}

                IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                if (isUseForMerge == false)
                {
                    //strFilePath = documentHandler.GenerateDocumentFilePath(doMainDoc[0], doSlaveDoc);
                    strFilePath = documentHandler.GenerateDocumentFilePath(doMainDoc[0], doSlaveDoc, null, true); //Modify by Jutarat A. on 13112012 (Add isReuseRptDoc)
                }
                else
                {
                    //Without encrypt for merge report
                    strFilePath = documentHandler.GenerateDocumentWithoutEncrypt(doMainDoc[0], doSlaveDoc);
                }


                return strFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // BLR020- Tax Invoice report

        /// <summary>
        /// Generate report BLR020- Tax Invoice report
        /// </summary>
        /// <param name="strTaxInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        // Comment by Jirawat Jannet
        //public Stream GenerateBLR020(string strTaxInvoiceNo, string strEmpNo, DateTime dtDateTime)
        //{
        //    string strFilePath = GenerateBLR020FilePath(strTaxInvoiceNo, strEmpNo, dtDateTime);
        //    IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

        //    return handlerDocument.GetDocumentReportFileStream(strFilePath);
        //}
        /// <summary>
        /// Generate report BLR020- Tax Invoice report
        /// </summary>
        /// <param name="strTaxInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <param name="isUseForMerge"></param>
        /// <returns></returns>
        // Comment by Jirawat Jannet
        //public string GenerateBLR020FilePath(string strTaxInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false)
        //{
        //    try
        //    {
        //        // Test: strTaxInvoiceNo = "201112B90046"

        //        string strFilePath = string.Empty;

        //        List<dtRptTaxInvoice> blr020 = base.GetRptTaxInvoice(strTaxInvoiceNo, MiscType.C_SHOW_DUEDATE, ShowDueDate.C_SHOW_DUEDATE_7, ShowDueDate.C_SHOW_DUEDATE_30, ShowDueDate.C_SHOW_DUEDATE_14, ShowDueDate.C_SHOW_DUEDATE_NONE);

        //        if (blr020.Count == 0)
        //        {
        //            return null;
        //        }

        //        // tt -> Order by ... (4/Sep/2012)
        //        if (blr020[0].SeparateInvType == SeparateInvType.C_SEP_INV_SORT_ASCE || blr020[0].SeparateInvType == SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_ASCE)
        //        {
        //            blr020 = (from p in blr020 orderby p.SortingType ascending select p).ToList<dtRptTaxInvoice>();
        //        }
        //        else if (blr020[0].SeparateInvType == SeparateInvType.C_SEP_INV_SORT_DESC || blr020[0].SeparateInvType == SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_DESC)
        //        {
        //            blr020 = (from p in blr020 orderby p.SortingType descending select p).ToList<dtRptTaxInvoice>();
        //        }
        //        else
        //        {
        //            blr020 = blr020.OrderBy(a => a.ContractCode).ThenBy(b => b.BillingOCC).ThenBy(c => c.BillingDetailNo).ToList<dtRptTaxInvoice>();
        //        }

        //        List<List<dtRptTaxInvoice>> blr020_list = new List<List<dtRptTaxInvoice>>();
        //        blr020_list.Add(blr020.Clone()); // for Original customer
        //        blr020_list.Add(blr020.Clone()); // for Copy customer
        //        blr020_list.Add(blr020.Clone()); // for Copy account 

        //        for (int i = 0; i < blr020_list.Count; i++)
        //        {
        //            //// Calcualate page   1st  10/p , >= 2nd 18/p
        //            //int totalPage = ((blr020_list[i].Count - 1 + 10) / 18) + 1;

        //            // tt
        //            // Calcualate page   1st  10/p , >= 2nd 11/p
        //            int totalPage = ((blr020_list[i].Count - 1 + 10) / 10) + 1;

        //            for (int j = 0; j < blr020_list[i].Count; j++)
        //            {
        //                // Calcualate page   1st  10/p , >= 2nd 18/p
        //                //blr020_list[i][j].Page = ((j + 10) / 18) + 1;

        //                //
        //                blr020_list[i][j].Page = ((j + 10) / 10) + 1;
        //                blr020_list[i][j].TotalPage = totalPage;

        //                // Update footer
        //                if (i == 0)
        //                {
        //                    blr020_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_EN + "\n" + InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_TH;
        //                }
        //                else if (i == 1)
        //                {
        //                    blr020_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_COPY_CUST_EN + "\n" + InvoiceDocument.C_INVOICE_DOC_COPY_CUST_TH;
        //                }
        //                else if (i == 2)
        //                {
        //                    blr020_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_COPY_ACC_EN + "\n" + InvoiceDocument.C_INVOICE_DOC_COPY_ACC_TH;
        //                }

        //            }


        //        }




        //        // Prepare List of doDocumentDataGenerate
        //        List<doDocumentDataGenerate> doMainDoc = new List<doDocumentDataGenerate>();
        //        List<doDocumentDataGenerate> doSlaveDoc = new List<doDocumentDataGenerate>();

        //        for (int i = 0; i < blr020_list.Count; i++)
        //        {
        //            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
        //            doDocument.DocumentNo = strTaxInvoiceNo;
        //            doDocument.DocumentCode = ReportID.C_REPORT_ID_TAX_INVOICE;
        //            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
        //            doDocument.DocumentData = blr020_list[i]; // ***

        //            // OtherKey
        //            doDocument.OtherKey.BillingOffice = blr020[0].BillingOfficeCode;
        //            doDocument.OtherKey.BillingTargetCode = blr020[0].BillingTargetCode;


        //            // Additional
        //            doDocument.EmpNo = strEmpNo;
        //            doDocument.ProcessDateTime = dtDateTime;

        //            if (i == 0)
        //            {
        //                doMainDoc.Add(doDocument);
        //            }
        //            else
        //            {
        //                doSlaveDoc.Add(doDocument);
        //                if (i == blr020_list.Count - 1)
        //                {
        //                    doSlaveDoc.Add(doDocument);
        //                }
        //            }

        //        }



        //        IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;


        //        if (isUseForMerge == false)
        //        {
        //            //strFilePath = documentHandler.GenerateDocumentFilePath(doMainDoc[0], doSlaveDoc);
        //            strFilePath = documentHandler.GenerateDocumentFilePath(doMainDoc[0], doSlaveDoc, null, true); //Modify by Jutarat A. on 13112012 (Add isReuseRptDoc)
        //        }
        //        else
        //        {
        //            //Without encrypt for merge report
        //            strFilePath = documentHandler.GenerateDocumentWithoutEncrypt(doMainDoc[0], doSlaveDoc);
        //        }
        //        return strFilePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        // BLR030- Payment form

        /// <summary>
        /// Generate report BLR030- Payment form
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        // Comment by Jirawat Jannet
        //public Stream GenerateBLR030(string strInvoiceNo, string strEmpNo, DateTime dtDateTime)
        //{
        //    string strFilePath = GenerateBLR030FilePath(strInvoiceNo, strEmpNo, dtDateTime);
        //    IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

        //    return handlerDocument.GetDocumentReportFileStream(strFilePath);
        //}
        /// <summary>
        /// Generate report BLR030- Payment form
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <param name="isUseForMerge"></param>
        /// <returns></returns>
        // Comment by Jirawat Jannet
        //public string GenerateBLR030FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false)
        //{
        //    try
        //    {
        //        // Test: strInvoiceNo = "201205A00204"

        //        string strFilePath = string.Empty;

        //        List<dtRptPaymentForm> blr030 = base.GetRptPaymentForm(strInvoiceNo);

        //        if (blr030.Count == 0)
        //        {
        //            return null;
        //        }

        //        doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
        //        doDocument.DocumentNo = strInvoiceNo;
        //        doDocument.DocumentCode = ReportID.C_REPORT_ID_PAYMENT_FORM;
        //        doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
        //        doDocument.DocumentData = blr030;

        //        // OtherKey
        //        doDocument.OtherKey.BillingOffice = blr030[0].BillingOfficeCode;
        //        doDocument.OtherKey.BillingTargetCode = blr030[0].BillingTargetCode;



        //        // Additional
        //        doDocument.EmpNo = strEmpNo;
        //        doDocument.ProcessDateTime = dtDateTime;

        //        IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;


        //        if (isUseForMerge == false)
        //        {
        //            strFilePath = documentHandler.GenerateDocumentFilePath(doDocument);
        //        }
        //        else
        //        {
        //            //Without encrypt for merge report
        //            strFilePath = documentHandler.GenerateDocumentWithoutEncrypt(doDocument);
        //        }

        //        return strFilePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        // BLR010 BLR020- Invoice report / Tax Invoice

        /// <summary>
        /// Generate report BLR010 BLR020- Invoice report / Tax Invoice
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strTaxInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        public Stream GenerateBLR010_BLR020(string strInvoiceNo, string strTaxInvoiceNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateBLR010_BLR020FilePath(strInvoiceNo, strTaxInvoiceNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }
        /// <summary>
        /// Generate report BLR010 BLR020- Invoice report / Tax Invoice
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strTaxInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        public string GenerateBLR010_BLR020FilePath(string strInvoiceNo, string strTaxInvoiceNo, string strEmpNo, DateTime dtDateTime)
        {
            try
            {
                List<string> mergeList = new List<string>();
                string blr010 = GenerateBLR010FilePath(strInvoiceNo, strEmpNo, dtDateTime, true);
                if (string.IsNullOrEmpty(blr010) == false)
                {
                    mergeList.Add(blr010);
                }

                // Comment by Jirawat Jannet
                //string blr020 = GenerateBLR020FilePath(strTaxInvoiceNo, strEmpNo, dtDateTime, true);
                //if (string.IsNullOrEmpty(blr020) == false)
                //{
                //    mergeList.Add(blr020);
                //}


                string mergeOutputFilename = PathUtil.GetTempFileName(".pdf");
                string encryptOutputFileName = PathUtil.GetTempFileName(".pdf");


                // Comment by Jirawat Jannet
                //bool isSuccess = ReportUtil.MergePDF(mergeList.ToArray(), mergeOutputFilename, true, encryptOutputFileName, null);
                //return isSuccess ? encryptOutputFileName : string.Empty;

                // Add by Jirawat Jannet
                return encryptOutputFileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Generate report BLR050
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        public Stream GenerateBLR050(string strInvoiceNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateBLR050FilePath(strInvoiceNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }

        /// <summary>
        /// Generate reoirt BLR050
        /// </summary>
        public string GenerateBLR050FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false)
        {
            try
            {
                // Test: strInvoiceNo = "201205A00121"

                string strFilePath = string.Empty;

                List<dtGetRptDocReceipt> blr050 = base.BL_GetRptDocReceipt(strInvoiceNo);

                if (blr050.Count == 0)
                {
                    return null;
                }

                if (blr050.Count % 3 != 0)
                {
                    dtGetRptDocReceipt blankData = new dtGetRptDocReceipt()
                    {
                        IssueInvDate = "",
                        ContactPersonName = "",
                        InvoiceNo = ""
                    };
                    for (int i = 0; i < blr050.Count % 3; i++) blr050.Add(blankData);
                }

                // add by jirawat jannet on 2016-12-06
                #region Add company name

                string CompanyName = DocumentOutput.getCompanyName("BLR050", 1, DateTime.Now.Date);
                foreach (var d in blr050)
                {
                    d.RPT_CompanyName = CompanyName;
                }

                #endregion




                doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                List<doDocumentDataGenerate> slavDocs = new List<doDocumentDataGenerate>();
                doDocument.DocumentNo = strInvoiceNo;
                doDocument.DocumentCode = ReportID.C_REPORT_ID_DOC_RECEIVE;
                doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                doDocument.DocumentData = blr050; 

                // Additional
                doDocument.EmpNo = strEmpNo;
                doDocument.ProcessDateTime = dtDateTime;

                IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                if (isUseForMerge == false)
                {
                    strFilePath = documentHandler.GenerateDocumentFilePath(doDocument, slavDocs, null, true); //Modify by Jutarat A. on 13112012 (Add isReuseRptDoc)
                }
                else
                {
                    //Without encrypt for merge report
                    strFilePath = documentHandler.GenerateDocumentWithoutEncrypt(doDocument);
                }

                return strFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region BLR020

        public Stream GenerateBLR020(string strInvoiceNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateBLR020FilePath(strInvoiceNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }

        public string GenerateBLR020FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false)
        {
            try
            {
                string strFilePath = string.Empty;

                List<dtRptInvoice> blr020 = base.GetRptInvoice(strInvoiceNo, MiscType.C_SHOW_DUEDATE, ShowDueDate.C_SHOW_DUEDATE_7, ShowDueDate.C_SHOW_DUEDATE_30, ShowDueDate.C_SHOW_DUEDATE_14, ShowDueDate.C_SHOW_DUEDATE_NONE);

                if (blr020.Count == 0)
                {
                    return null;
                }

                // Add by Jirawat Jannet @ 2016-10-04
                #region Summary billing amount

                decimal sumAmount = 0;

                if (blr020[0].BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    sumAmount = blr020.Select(m => m.BillingAmount).Sum();
                else
                    sumAmount = blr020.Select(m => m.BillingAmountUsd).Sum();

                foreach (var item in blr020)
                    item.RPT_SumAmount = string.Format("{0} {1}", item.RPT_BillingAmountCurrencyName, sumAmount.ToString("N2"));

                #endregion


                if (blr020[0].SeparateInvType == SeparateInvType.C_SEP_INV_SORT_ASCE || blr020[0].SeparateInvType == SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_ASCE)
                {
                    blr020 = (from p in blr020 orderby p.SortingType ascending select p).ToList<dtRptInvoice>();
                }
                else if (blr020[0].SeparateInvType == SeparateInvType.C_SEP_INV_SORT_DESC || blr020[0].SeparateInvType == SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_DESC)
                {
                    blr020 = (from p in blr020 orderby p.SortingType descending select p).ToList<dtRptInvoice>();
                }
                else
                {
                    blr020 = blr020.OrderBy(a => a.ContractCode).ThenBy(b => b.BillingOCC).ThenBy(c => c.BillingDetailNo).ToList<dtRptInvoice>();
                }

                // If payment method is bank transfer then attach BLR030-PaymentForm also
                bool isAttachPaymentForm = false;
                if (blr020[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER)
                {
                    isAttachPaymentForm = true;
                }

                List<List<dtRptInvoice>> blr020_list = new List<List<dtRptInvoice>>();
                blr020_list.Add(blr020.Clone()); // for Original customer
                blr020_list.Add(blr020.Clone()); // for Copy account


                int itemPerPage = 10;

                for (int i = 0; i < blr020_list.Count; i++)
                {
                    int totalPage = ((blr020_list[i].Count - 1 + itemPerPage) / itemPerPage) + 1;

                    for (int j = 0; j < blr020_list[i].Count; j++)
                    {
                        blr020_list[i][j].Page = ((j + itemPerPage) / itemPerPage) + 1;
                        blr020_list[i][j].TotalPage = totalPage;

                        // Update footer
                        if (i == 0)
                        {
                            blr020_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_EN;// + "\n" + InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_TH; Comment by Jirawat Jannet
                        }
                        else if (i == 1) //else if (i == 2) change by jirawat jannet
                        {
                            blr020_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_COPY_ACC_EN;// + "\n" + InvoiceDocument.C_INVOICE_DOC_COPY_ACC_TH; Comment by Jirawat Jannet
                        }
                    }
                }

                // Prepare List of doDocumentDataGenerate
                List<doDocumentDataGenerate> doMainDoc = new List<doDocumentDataGenerate>();
                List<doDocumentDataGenerate> doSlaveDoc = new List<doDocumentDataGenerate>();

                for (int i = 0; i < blr020_list.Count; i++)
                {
                    List<ReportParameterObject> reportParameter = new List<ReportParameterObject>();
                    ReportParameterObject param = new ReportParameterObject()
                    {
                        ParameterName = "C_PAYMENT_METHOD_BANK_TRANSFER",
                        Value = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                    };
                    reportParameter.Add(param);

                    doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                    doDocument.DocumentNo = strInvoiceNo;
                    doDocument.DocumentCode = ReportID.C_REPORT_ID_INVOICE2;
                    doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                    doDocument.DocumentData = blr020_list[i]; // **
                    doDocument.MainReportParam = reportParameter;

                    // OtherKey
                    doDocument.OtherKey.BillingOffice = blr020[0].BillingOfficeCode;
                    doDocument.OtherKey.BillingTargetCode = blr020[0].BillingTargetCode;

                    // Additional
                    doDocument.EmpNo = strEmpNo;
                    doDocument.ProcessDateTime = dtDateTime;

                    if (i == 0)
                    {
                        doMainDoc.Add(doDocument);
                    }
                    else
                    {
                        doSlaveDoc.Add(doDocument);
                    }
                }

                IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                if (isUseForMerge == false)
                {
                    strFilePath = documentHandler.GenerateDocumentFilePath(doMainDoc[0], doSlaveDoc, null, true); //Modify by Jutarat A. on 13112012 (Add isReuseRptDoc)
                }
                else
                {
                    //Without encrypt for merge report
                    strFilePath = documentHandler.GenerateDocumentWithoutEncrypt(doMainDoc[0], doSlaveDoc);
                }


                return strFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region BLR030

        public Stream GenerateBLR030(string strInvoiceNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateBLR030FilePath(strInvoiceNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }

        public string GenerateBLR030FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false)
        {
            try
            {
                string strFilePath = string.Empty;

                List<dtRptInvoice> blr030 = base.GetRptInvoice(strInvoiceNo, MiscType.C_SHOW_DUEDATE, ShowDueDate.C_SHOW_DUEDATE_7, ShowDueDate.C_SHOW_DUEDATE_30, ShowDueDate.C_SHOW_DUEDATE_14, ShowDueDate.C_SHOW_DUEDATE_NONE);

                if (blr030.Count == 0)
                {
                    return null;
                }

                // Add by Jirawat Jannet @ 2016-10-04
                #region Summary billing amount

                decimal sumAmount = 0;

                if (blr030[0].BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    sumAmount = blr030.Select(m => m.BillingAmount).Sum();
                else
                    sumAmount = blr030.Select(m => m.BillingAmountUsd).Sum();

                foreach (var item in blr030)
                    item.RPT_SumAmount = string.Format("{0} {1}", item.RPT_BillingAmountCurrencyName, sumAmount.ToString("N2"));

                #endregion

                if (blr030[0].SeparateInvType == SeparateInvType.C_SEP_INV_SORT_ASCE || blr030[0].SeparateInvType == SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_ASCE)
                {
                    blr030 = (from p in blr030 orderby p.SortingType ascending select p).ToList<dtRptInvoice>();
                }
                else if (blr030[0].SeparateInvType == SeparateInvType.C_SEP_INV_SORT_DESC || blr030[0].SeparateInvType == SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_DESC)
                {
                    blr030 = (from p in blr030 orderby p.SortingType descending select p).ToList<dtRptInvoice>();
                }
                else
                {
                    blr030 = blr030.OrderBy(a => a.ContractCode).ThenBy(b => b.BillingOCC).ThenBy(c => c.BillingDetailNo).ToList<dtRptInvoice>();
                }

                // If payment method is bank transfer then attach BLR030-PaymentForm also
                bool isAttachPaymentForm = false;
                if (blr030[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER)
                {
                    isAttachPaymentForm = true;
                }

                List<List<dtRptInvoice>> blr030_list = new List<List<dtRptInvoice>>();
                blr030_list.Add(blr030.Clone()); // for Original customer
                blr030_list.Add(blr030.Clone()); // for Copy account


                int itemPerPage = 10;

                for (int i = 0; i < blr030_list.Count; i++)
                {
                    int totalPage = ((blr030_list[i].Count - 1 + itemPerPage) / itemPerPage) + 1;

                    for (int j = 0; j < blr030_list[i].Count; j++)
                    {
                        blr030_list[i][j].Page = ((j + itemPerPage) / itemPerPage) + 1;
                        blr030_list[i][j].TotalPage = totalPage;

                        // Update footer
                        if (i == 0)
                        {
                            blr030_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_EN;// + "\n" + InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_TH; Comment by Jirawat Jannet
                        }
                        else if (i == 1) //else if (i == 2) change by jirawat jannet
                        {
                            blr030_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_COPY_ACC_EN;// + "\n" + InvoiceDocument.C_INVOICE_DOC_COPY_ACC_TH; Comment by Jirawat Jannet
                        }
                    }
                }

                // Prepare List of doDocumentDataGenerate
                List<doDocumentDataGenerate> doMainDoc = new List<doDocumentDataGenerate>();
                List<doDocumentDataGenerate> doSlaveDoc = new List<doDocumentDataGenerate>();

                for (int i = 0; i < blr030_list.Count; i++)
                {
                    List<ReportParameterObject> reportParameter = new List<ReportParameterObject>();
                    ReportParameterObject param = new ReportParameterObject()
                    {
                        ParameterName = "C_PAYMENT_METHOD_BANK_TRANSFER",
                        Value = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                    };
                    reportParameter.Add(param);

                    doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                    doDocument.DocumentNo = strInvoiceNo;
                    doDocument.DocumentCode = ReportID.C_REPORT_ID_INVOICE3;
                    doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                    doDocument.DocumentData = blr030_list[i]; // **
                    doDocument.MainReportParam = reportParameter;

                    // OtherKey
                    doDocument.OtherKey.BillingOffice = blr030[0].BillingOfficeCode;
                    doDocument.OtherKey.BillingTargetCode = blr030[0].BillingTargetCode;

                    // Additional
                    doDocument.EmpNo = strEmpNo;
                    doDocument.ProcessDateTime = dtDateTime;

                    if (i == 0)
                    {
                        doMainDoc.Add(doDocument);
                    }
                    else
                    {
                        doSlaveDoc.Add(doDocument);
                    }
                }

                IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                if (isUseForMerge == false)
                {
                    strFilePath = documentHandler.GenerateDocumentFilePath(doMainDoc[0], doSlaveDoc, null, true); //Modify by Jutarat A. on 13112012 (Add isReuseRptDoc)
                }
                else
                {
                    //Without encrypt for merge report
                    strFilePath = documentHandler.GenerateDocumentWithoutEncrypt(doMainDoc[0], doSlaveDoc);
                }


                return strFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region BLR040

        public Stream GenerateBLR040(string strInvoiceNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateBLR040FilePath(strInvoiceNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }

        public string GenerateBLR040FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false)
        {
            try
            {
                string strFilePath = string.Empty;

                List<dtRptInvoice> blr040 = base.GetRptInvoice(strInvoiceNo, MiscType.C_SHOW_DUEDATE, ShowDueDate.C_SHOW_DUEDATE_7, ShowDueDate.C_SHOW_DUEDATE_30, ShowDueDate.C_SHOW_DUEDATE_14, ShowDueDate.C_SHOW_DUEDATE_NONE);

                if (blr040 == null || blr040.Count == 0)
                {
                    return null;
                }

                // Add by Jirawat Jannet @ 2016-10-04
                #region Summary billing amount

                decimal sumAmount = 0;

                if (blr040[0].BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    sumAmount = blr040.Select(m => m.BillingAmount).Sum();
                else
                    sumAmount = blr040.Select(m => m.BillingAmountUsd).Sum();

                foreach (var item in blr040)
                    item.RPT_SumAmount = string.Format("{0} {1}", item.RPT_BillingAmountCurrencyName, sumAmount.ToString("N2"));

                #endregion

                if (blr040[0].SeparateInvType == SeparateInvType.C_SEP_INV_SORT_ASCE || blr040[0].SeparateInvType == SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_ASCE)
                {
                    blr040 = (from p in blr040 orderby p.SortingType ascending select p).ToList<dtRptInvoice>();
                }
                else if (blr040[0].SeparateInvType == SeparateInvType.C_SEP_INV_SORT_DESC || blr040[0].SeparateInvType == SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_DESC)
                {
                    blr040 = (from p in blr040 orderby p.SortingType descending select p).ToList<dtRptInvoice>();
                }
                else
                {
                    blr040 = blr040.OrderBy(a => a.ContractCode).ThenBy(b => b.BillingOCC).ThenBy(c => c.BillingDetailNo).ToList<dtRptInvoice>();
                }

                // If payment method is bank transfer then attach BLR030-PaymentForm also
                bool isAttachPaymentForm = false;
                if (blr040[0].PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER)
                {
                    isAttachPaymentForm = true;
                }

                List<List<dtRptInvoice>> blr040_list = new List<List<dtRptInvoice>>();
                blr040_list.Add(blr040.Clone()); // for Original customer
                blr040_list.Add(blr040.Clone()); // for Copy account


                int itemPerPage = 10;

                for (int i = 0; i < blr040_list.Count; i++)
                {
                    int totalPage = ((blr040_list[i].Count - 1 + itemPerPage) / itemPerPage) + 1;

                    for (int j = 0; j < blr040_list[i].Count; j++)
                    {
                        blr040_list[i][j].Page = ((j + itemPerPage) / itemPerPage) + 1;
                        blr040_list[i][j].TotalPage = totalPage;

                        // Update footer
                        if (i == 0)
                        {
                            blr040_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_EN;// + "\n" + InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_TH; Comment by Jirawat Jannet
                        }
                        else if (i == 1) //else if (i == 2) change by jirawat jannet
                        {
                            blr040_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_COPY_ACC_EN;// + "\n" + InvoiceDocument.C_INVOICE_DOC_COPY_ACC_TH; Comment by Jirawat Jannet
                        }
                    }
                }

                // Prepare List of doDocumentDataGenerate
                List<doDocumentDataGenerate> doMainDoc = new List<doDocumentDataGenerate>();
                List<doDocumentDataGenerate> doSlaveDoc = new List<doDocumentDataGenerate>();

                for (int i = 0; i < blr040_list.Count; i++)
                {
                    List<ReportParameterObject> reportParameter = new List<ReportParameterObject>();
                    ReportParameterObject param = new ReportParameterObject()
                    {
                        ParameterName = "C_PAYMENT_METHOD_BANK_TRANSFER",
                        Value = PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                    };
                    reportParameter.Add(param);

                    doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                    doDocument.DocumentNo = strInvoiceNo;
                    doDocument.DocumentCode = ReportID.C_REPORT_ID_INVOICE4;
                    doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                    doDocument.DocumentData = blr040_list[i]; // **
                    doDocument.MainReportParam = reportParameter;

                    // OtherKey
                    doDocument.OtherKey.BillingOffice = blr040[0].BillingOfficeCode;
                    doDocument.OtherKey.BillingTargetCode = blr040[0].BillingTargetCode;

                    // Additional
                    doDocument.EmpNo = strEmpNo;
                    doDocument.ProcessDateTime = dtDateTime;

                    if (i == 0)
                    {
                        doMainDoc.Add(doDocument);
                    }
                    else
                    {
                        doSlaveDoc.Add(doDocument);
                    }
                }

                IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                if (isUseForMerge == false)
                {
                    strFilePath = documentHandler.GenerateDocumentFilePath(doMainDoc[0], doSlaveDoc, null, true); //Modify by Jutarat A. on 13112012 (Add isReuseRptDoc)
                }
                else
                {
                    //Without encrypt for merge report
                    strFilePath = documentHandler.GenerateDocumentWithoutEncrypt(doMainDoc[0], doSlaveDoc);
                }


                return strFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}