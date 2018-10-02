using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Text;
using System.IO;

using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.Diagnostics;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Billing.Models;
using System.Threading;
using System.Printing;
using System.Transactions;

namespace SECOM_AJIS.Presentation.Billing.Controllers
{
    public partial class BillingController : BaseController
    {
        public ActionResult TestBilling_Authority(TestBilling_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            return InitialScreenEnvironment<object>("TestBilling", param, res);
        }

        [Initialize("TestBilling")]
        public ActionResult TestBilling()
        {
            return View();
        }

        public ActionResult BLR010_Invoice(string invoiceNo)
        {
            try
            {
                IBillingDocumentHandler handlerBillingDocument = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;

                //// Mothed #1: Directly generate
                //List<dtRptInvoice> rptList = handlerBillingDocument.GetRptInvoice("201205A00121", MiscType.C_SHOW_DUEDATE, ShowDueDate.C_SHOW_DUEDATE_7, ShowDueDate.C_SHOW_DUEDATE_30);

                //// Update value
                //foreach (var item in rptList)
                //{
                //    item.RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_EN + "\n" + InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_TH;
                //}


                //ReportDocument rptH = new ReportDocument();
                //string path = ReportUtil.GetReportPath("Reports/BLR010_Invoice.rpt", Server.MapPath("/"));

                //rptH.Load(path);


                //rptH.SetDataSource(rptList);
                //rptH.SetParameterValue("C_PAYMENT_METHOD_BANK_TRANSFER", PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER);

                //Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //rptH.Close();

                //return File(stream, "application/pdf");



                // Mothed #2: Official generate
                Stream stream2 = handlerBillingDocument.GenerateBLR010(invoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, DateTime.Now);

                return File(stream2, "application/pdf");


                ////Test by Jutarat A.
                //for (int i = 0; i < 300; i++)
                //{
                //    handlerBillingDocument.GenerateBLR010FilePath("201210A00001", "490430", DateTime.Now);

                //    handlerBillingDocument.GenerateBLR020FilePath("201210B00001", "490430", DateTime.Now);
                //}

                //return null;
                ////End Test

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public ActionResult BLR020_Invoice(string invoiceNo)
        {
            try
            {
                IBillingDocumentHandler handlerBillingDocument = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;
                
                // Mothed #2: Official generate
                Stream stream2 = handlerBillingDocument.GenerateBLR020(invoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, DateTime.Now);

                return File(stream2, "application/pdf");


            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public ActionResult BLR030_Invoice(string invoiceNo)
        {
            try
            {
                IBillingDocumentHandler handlerBillingDocument = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;

                // Mothed #2: Official generate
                Stream stream2 = handlerBillingDocument.GenerateBLR030(invoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, DateTime.Now);

                return File(stream2, "application/pdf");


            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public ActionResult BLR040_Invoice(string invoiceNo)
        {
            try
            {
                IBillingDocumentHandler handlerBillingDocument = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;

                // Mothed #2: Official generate
                Stream stream2 = handlerBillingDocument.GenerateBLR040(invoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, DateTime.Now);

                return File(stream2, "application/pdf");


            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public ActionResult BLR050_DocReceive(string invoiceNo)
        {
            try
            {
                IBillingDocumentHandler handlerBillingDocument = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;

                // Mothed #2: Official generate
                Stream stream2 = handlerBillingDocument.GenerateBLR050(invoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, DateTime.Now);

                return File(stream2, "application/pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BLR020_TaxInvoice(string taxInvoice)
        {
            try
            {
                IBillingDocumentHandler handlerBillingDocument = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;

                //List<dtRptTaxInvoice> rptList = handlerBillingDocument.GetRptTaxInvoice("201112B90046", MiscType.C_SHOW_DUEDATE, ShowDueDate.C_SHOW_DUEDATE_7, ShowDueDate.C_SHOW_DUEDATE_30);

                //// Update value
                //foreach (var item in rptList)
                //{
                //    item.RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_EN + "\n" + InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_TH;
                //}


                //ReportDocument rptH = new ReportDocument();
                //string path = ReportUtil.GetReportPath("Reports/BLR020_TaxInvoice.rpt", Server.MapPath("/"));

                //rptH.Load(path);


                //rptH.SetDataSource(rptList);

                //Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //rptH.Close();

                //return File(stream, "application/pdf");

                // Mothed #2: Official generate
                //Stream stream2 = handlerBillingDocument.GenerateBLR020("201112B90046", "500575", DateTime.Now);
                // Comment by Jirawat Jannet
                //Stream stream2 = handlerBillingDocument.GenerateBLR020(taxInvoice, CommonUtil.dsTransData.dtUserData.EmpNo, DateTime.Now);
                //return File(stream2, "application/pdf");
                throw new Exception("ไม่ใช้แล้ว");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public ActionResult BLR030_PaymentForm(string invoiceNo)
        {
            try
            {
                IBillingDocumentHandler handlerBillingDocument = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;

                // Mothed #1: Directly generate

                //List<dtRptPaymentForm> rptList = handlerBillingDocument.GetRptPaymentForm("201205A00204");

                //ReportDocument rptH = new ReportDocument();
                //string path = ReportUtil.GetReportPath("Reports/BLR030_Payment.rpt", Server.MapPath("/"));

                //rptH.Load(path);

                //rptH.SetDataSource(rptList);

                //Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //rptH.Close();

                //return File(stream, "application/pdf");


                // Mothed #2: Official generate
                //Stream stream2 =  handlerBillingDocument.GenerateBLR030("201205A00204", "500575", DateTime.Now);
                // Comment by Jirawat Jannet
                //Stream stream2 = handlerBillingDocument.GenerateBLR030(invoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, DateTime.Now);
                //return File(stream2, "application/pdf");
                throw new Exception("กำลังดำเนินการแก้ไข report BLR030");

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public ActionResult CMS999_TestPrintFoxitByCommand()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = @"D:\FoxitReaderPortable\FoxitReaderPortable.exe";
                //process.StartInfo.Arguments = @"/p D:\TestFoxit.pdf" ;
                process.StartInfo.Arguments = string.Format("/t \"{0}\" \"{1}\"", @"D:\TestFoxit.pdf", @"FX Document Centre 236 PCL 6");//string.Format("/t \"{0}\" \"{1}\"", strPathFilename, ConfigurationManager.AppSettings["PrinterName"]);
                process.StartInfo.Verb = "Print";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {

            }

            return Json(1);
        }

        public ActionResult BLP030_ManageInvoiceProcessTemp()
        {
            ObjectResultData res = new ObjectResultData();
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IBillingDocumentHandler billingDocumentHandler = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;

            try
            {
                //using (TransactionScope scope = new TransactionScope())
                //{
                List<tbt_Invoice> invoiceList = billingHandler.GetTbt_Invoice(null, null);
                List<tbt_DocumentList> documentList = comHandler.GetTbt_DocumentList(null, null, null);

                invoiceList = (from t in invoiceList
                               where
                                   //t.CreateBy == "BLP030" 
                                   //&& t.InvoicePaymentStatus != "08"
                                   //&& 
                                t.CreateDate <= new DateTime(2013, 6, 21)
                                && t.CreateDate > new DateTime(2013, 6, 20)
                                && !documentList.Any(d => (d.DocumentNo == t.InvoiceNo))
                               select t).ToList<tbt_Invoice>();

                foreach (tbt_Invoice data in invoiceList)
                {
                    billingDocumentHandler.GenerateBLR010FilePath(data.InvoiceNo, ProcessID.C_PROCESS_ID_MANAGE_INVOICE, data.CreateDate.Value);
                }


                // Comment by Jirawat Jannet
                //List<tbt_TaxInvoice> taxInvoiceList = billingHandler.GetTbt_TaxInvoice(null);

                //taxInvoiceList = (from t in taxInvoiceList
                //                  where
                //                      //t.CreateBy == "BLP030"
                //                      //&& 
                //                    t.CreateDate <= new DateTime(2013, 6, 21)
                //                    && t.CreateDate > new DateTime(2013, 6, 20)
                //                    && !documentList.Any(d => (d.DocumentNo == t.TaxInvoiceNo))
                //                  select t).ToList<tbt_TaxInvoice>();

                // Comment by Jirawat Jannet
                //foreach (tbt_TaxInvoice data in taxInvoiceList)
                //{
                //    billingDocumentHandler.GenerateBLR020FilePath(data.TaxInvoiceNo, ProcessID.C_PROCESS_ID_MANAGE_INVOICE, data.CreateDate.Value);
                //}


                //    scope.Complete();
                //    res.ResultData = "Process Complete.";
                //}

                res.ResultData = "Process Complete.";
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult BLP010_GenerateReport(string strInvoiceNo)
        {
            ObjectResultData res = new ObjectResultData();
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            IBillingDocumentHandler billingDocumentHandler = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;

            try
            {
                List<tbt_Invoice> invoiceList = billingHandler.GetTbt_Invoice(strInvoiceNo, null).OrderByDescending(t => t.InvoiceOCC).ToList<tbt_Invoice>();

                if (invoiceList != null && invoiceList.Count > 0)
                {
                    billingDocumentHandler.GenerateBLR010FilePath(invoiceList[0].InvoiceNo, ProcessID.C_PROCESS_ID_MANAGE_INVOICE, invoiceList[0].CreateDate.Value);
                }

                //ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //List<tbt_DocumentList> documentList = comHandler.GetTbt_DocumentList(null, null, null);

                //if (documentList != null && documentList.Count > 0)
                //{
                //    foreach (tbt_DocumentList data in documentList)
                //    {
                //        billingDocumentHandler.GenerateBLR010FilePath(data.DocumentNo, ProcessID.C_PROCESS_ID_MANAGE_INVOICE, data.CreateDate.Value);
                //    }
                //}
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult BLP020_GenerateReport(string strInvoiceNo)
        {
            ObjectResultData res = new ObjectResultData();
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            IBillingDocumentHandler billingDocumentHandler = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;

            try
            {
                // Comment by Jirawat Jannet
                //List<tbt_TaxInvoice> taxInvoiceList = billingHandler.GetTbt_TaxInvoice(strInvoiceNo);
                //if (taxInvoiceList != null && taxInvoiceList.Count > 0)
                //{
                //    billingDocumentHandler.GenerateBLR020FilePath(taxInvoiceList[0].TaxInvoiceNo, ProcessID.C_PROCESS_ID_MANAGE_INVOICE, taxInvoiceList[0].CreateDate.Value);
                //}
                throw new Exception("กำลังดำเนินการแก้ไข report BLR020");

                //ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //List<tbt_DocumentList> documentList = comHandler.GetTbt_DocumentList(null, null, null);

                //if (documentList != null && documentList.Count > 0)
                //{
                //    foreach (tbt_DocumentList data in documentList)
                //    {
                //        billingDocumentHandler.GenerateBLR020FilePath(data.DocumentNo, ProcessID.C_PROCESS_ID_MANAGE_INVOICE, data.CreateDate.Value);
                //    }
                //}
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        
        public ActionResult BLP010_GenerateReportFromTable()
        {
            ObjectResultData res = new ObjectResultData();
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            IBillingDocumentHandler billingDocumentHandler = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;

            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            res.ResultData = true;

            try
            {
                List<string> lstInvoiceNo = billingHandler.GetTbt_InvoiceReprint();
                foreach (string invoiceno in lstInvoiceNo)
                {
                    try
                    {
                        billingDocumentHandler.GenerateBLR010FilePath(invoiceno, ProcessID.C_PROCESS_ID_MANAGE_INVOICE, DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        res.ResultData = false;
                        res.AddErrorMessage(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
    }

    public class TestBilling_ScreenParameter : ScreenParameter
    {

    }


}
