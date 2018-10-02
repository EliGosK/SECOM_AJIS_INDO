using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;

using CSI.WindsorHelper;

using System.Configuration;
using SECOM_AJIS.Common.Controllers;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SECOM_AJIS.Common.Util.ConstantValue;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.Common.Models.EmailTemplates;
using System.Transactions;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        private string OWNER_PASSWORD = "P@$$w0rd";

        public ActionResult TestPDFTK_Encrypt()
        {
            ReportUtil.EncryptPDF(@"D:\1.pdf", @"D:\1.128.pdf", "foo");

            return null;
        }

        public ActionResult TestPDFTK_Merge()
        {
            ReportUtil.MergePDF(@"D:\1.pdf", @"D:\2.pdf", @"D:\12.pdf");

            return null;
        }

        public ActionResult TestPDFTK_MergeWithEncrypt()
        {
            ReportUtil.MergePDF(@"D:\1.pdf", @"D:\2.pdf", @"D:\12.pdf", true);

            return null;
        }

        [HttpPost] // GetContractBranchName
        public ActionResult GetContractBranchName(string cond)
        {
            try
            {
                IViewContractHandler handler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtContractBranchName> lst = handler.GetContractBranchName(cond);

                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.BranchName);
                }

                //string xml = CommonUtil.ConvertToXml<dtContractBranchName>(lst);
                return Json(strList.ToArray());
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        public ActionResult TestReports()
        {
            IContractDocumentHandler hand = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            IList<RPTChangeNoticeDo> list = hand.GetRptChangeNoticeData(0);
            //IList<Presentation.Contract.Models.RPTChangeNoticeDo> listRpt = new List<Presentation.Contract.Models.RPTChangeNoticeDo>();
            //foreach(RPTChangeNoticeDo r in list){
            //    var t = new Presentation.Contract.Models.RPTChangeNoticeDo();
            //    t=CommonUtil.CloneObject<RPTChangeNoticeDo, Presentation.Contract.Models.RPTChangeNoticeDo>(r);
            //    listRpt.Add(t);
            //}



            ReportDocument rptH = new ReportDocument();
            //var path = ReportUtil.GetReportPath("Reports/CTR020_ChangeNotice.rpt",Server.MapPath("/"));
            //string path = ReportUtil.GetReportTemplatePath("CTR020_ChangeNotice.rpt");
            string path = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, "CTR020_ChangeNotice.rpt");
            rptH.Load(path);

            rptH.SetDataSource(list);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();


            return File(stream, "application/pdf");
        }

        public ActionResult CTR020_ChangeNotice(int iDocID)
        {
            try
            {
                IContractDocumentHandler contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
                List<RPTChangeNoticeDo> rptList = contractDocHandler.GetRptChangeNoticeData(iDocID);

                IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                //doDocumentDataGenerate doDoc = new doDocumentDataGenerate();
                //if (rptList.Count > 0)
                //{
                //    doDoc.DocumentNo = rptList[0].DocNo;
                //    doDoc.DocumentCode = rptList[0].DocumentCode;
                //    doDoc.DocumentData = rptList;
                //}

                //Stream stream = documentHandler.GenerateDocument(doDoc);

                string path = ReportUtil.GetReportPath("Reports/CTR020_ChangeNotice.rpt", Server.MapPath("/"));
                //string path = ReportUtil.GetReportTemplatePath("CTR020_ChangeNotice.rpt");

                ReportDocument rptH = new ReportDocument();
                rptH.Load(path);
                rptH.SetDataSource(rptList);
                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                rptH.Close();

                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public ActionResult CTR030_ChangeMemorandum(int iDocID)
        {
            IContractDocumentHandler contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            List<RPTChangeMemoDo> rptList = contractDocHandler.GetRptChangeMemoData(iDocID);

            ReportDocument rptH = new ReportDocument();
            string path = ReportUtil.GetReportPath("Reports/CTR030_ChangeMemorandum.rpt", Server.MapPath("/"));
            //string path = ReportUtil.GetReportTemplatePath("CTR030_ChangeMemorandum.rpt");

            rptH.Load(path);

            rptH.SetDataSource(rptList);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }

        public ActionResult CTR010_ContractEnglish(int iDocID)
        {
            IContractDocumentHandler contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            List<RPTContractReportDo> rptList = contractDocHandler.GetRptContractReportData(iDocID);

            ReportDocument rptH = new ReportDocument();
            string path = ReportUtil.GetReportPath("Reports/CTR010_ContractEnglish.rpt", Server.MapPath("/"));
            //string path = ReportUtil.GetReportTemplatePath("CTR010_ContractEnglish.rpt");

            rptH.Load(path);
            rptH.SetDataSource(rptList);

            rptH.Subreports["CTR010_1"].SetDataSource(rptList);
            rptH.SetParameterValue("FlagOn", FlagType.C_FLAG_ON, "CTR010_1");

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }

        public ActionResult CTR011_ContractThai(int iDocID)
        {
            IContractDocumentHandler contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            List<RPTContractReportDo> rptList = contractDocHandler.GetRptContractReportData(iDocID);

            ReportDocument rptH = new ReportDocument();
            string path = ReportUtil.GetReportPath("Reports/CTR011_ContractThai.rpt", Server.MapPath("/"));
            //string path = ReportUtil.GetReportTemplatePath("CTR011_ContractThai.rpt");

            rptH.Load(path);
            rptH.SetDataSource(rptList);

            rptH.Subreports["CTR011_1"].SetDataSource(rptList);
            rptH.SetParameterValue("FlagOn", FlagType.C_FLAG_ON, "CTR011_1");

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }

        public ActionResult CTR040_StartResumeMemorandum(int iDocID)
        {
            IContractDocumentHandler contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            List<RPTStartResumeMemoDo> rptList = contractDocHandler.GetRptStartResumeMemoData(iDocID);

            ReportDocument rptH = new ReportDocument();
            string path = ReportUtil.GetReportPath("Reports/CTR040_StartResumeMemorandum.rpt", Server.MapPath("/"));
            //string path = ReportUtil.GetReportTemplatePath("CTR040_StartResumeMemorandum.rpt");

            rptH.Load(path);
            rptH.SetDataSource(rptList);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }

        public ActionResult CTR050_ConfirmCurrentInstrumentMemorandum(int iDocID)
        {
            IContractDocumentHandler contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            List<RPTConfirmCurrInstMemoDo> rptList = contractDocHandler.GetRptConfirmCurrentInstrumentMemoData(iDocID);

            ReportDocument rptH = new ReportDocument();
            string path = ReportUtil.GetReportPath("Reports/CTR050_ConfirmCurrentInstrumentMemorandum.rpt", Server.MapPath("/"));
            //string path = ReportUtil.GetReportTemplatePath("CTR050_ConfirmCurrentInstrumentMemorandum.rpt");

            rptH.Load(path);
            rptH.SetDataSource(rptList);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }

        public ActionResult CTR060_CancelContractMemorandum(int iDocID)
        {
            try
            {
                IContractDocumentHandler contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;

                List<RPTCancelContractMemoDo> rptList = contractDocHandler.GetRptCancelContractMemoData(iDocID);
                List<RPTCancelContractMemoDetailDo> rptListDetail = contractDocHandler.GetRptCancelContractMemoDetail(iDocID);

                ReportDocument rptH = new ReportDocument();
                string path = ReportUtil.GetReportPath("Reports/CTR060_CancelContractMemorandum.rpt", Server.MapPath("/"));
                //string path = ReportUtil.GetReportTemplatePath("CTR060_CancelContractMemorandum.rpt");

                rptH.Load(path);


                rptH.SetDataSource(rptList);
                rptH.Subreports["CTR060_1"].SetDataSource(rptListDetail);

                rptH.SetParameterValue("AutoBillingTypeNone", AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_NONE, "CTR060_1");
                rptH.SetParameterValue("AutoBillingTypeAll", AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_ALL, "CTR060_1");
                rptH.SetParameterValue("AutoBillingTypePartial", AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_PARTIAL, "CTR060_1");
                rptH.SetParameterValue("BankBillingTypeNone", BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_NONE, "CTR060_1");
                rptH.SetParameterValue("BankBillingTypeAll", BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_ALL, "CTR060_1");
                rptH.SetParameterValue("BankBillingTypePartial", BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_PARTIAL, "CTR060_1");

                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                rptH.Close();

                //IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                //doDocumentDataGenerate doDoc = new doDocumentDataGenerate();
                //if (rptList.Count > 0)
                //{
                //    doDoc.DocumentNo = rptList[0].DocNo;
                //    doDoc.DocumentCode = rptList[0].DocumentCode;
                //    doDoc.DocumentData = rptList;
                //}

                //List<ReportParameterObject> listSubReportDataSource = new List<ReportParameterObject>();
                //listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "CTR060_1", Value = rptListDetail });
                //doDoc.SubReportDataSource = listSubReportDataSource;

                //doDoc.MainReportParam = null;

                //List<ReportParameterObject> listSubReportParam = new List<ReportParameterObject>();
                //listSubReportParam.Add(new ReportParameterObject() { ParameterName = "AutoBillingTypeNone", Value = AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_NONE, SubReportName = "CTR060_1" });
                //listSubReportParam.Add(new ReportParameterObject() { ParameterName = "AutoBillingTypeAll", Value = AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_ALL, SubReportName = "CTR060_1" });
                //listSubReportParam.Add(new ReportParameterObject() { ParameterName = "AutoBillingTypePartial", Value = AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_PARTIAL, SubReportName = "CTR060_1" });
                //listSubReportParam.Add(new ReportParameterObject() { ParameterName = "BankBillingTypeNone", Value = BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_NONE, SubReportName = "CTR060_1" });
                //listSubReportParam.Add(new ReportParameterObject() { ParameterName = "BankBillingTypeAll", Value = BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_ALL, SubReportName = "CTR060_1" });
                //listSubReportParam.Add(new ReportParameterObject() { ParameterName = "BankBillingTypePartial", Value = BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_PARTIAL, SubReportName = "CTR060_1" });


                //doDoc.SubReportParam = listSubReportParam;


                //// doDoc2

                //List<RPTCoverLetterDo> rptListCover = contractDocHandler.GetRptCoverLetterData(100); // iDocID = 100 , fixed to 100 for test
                //List<RPTInstrumentDetailDo> rptListInst = contractDocHandler.GetRptInstrumentDetailData(100); // iDocID = 100 , fixed to 100 for test

                //doDocumentDataGenerate doDoc2 = new doDocumentDataGenerate();
                //if (rptListCover.Count > 0)
                //{
                //    rptListCover[0].DocumentCode = "CTR090";
                //    doDoc2.DocumentNo = rptListCover[0].DocNo;
                //    doDoc2.DocumentCode = rptListCover[0].DocumentCode;
                //    doDoc2.DocumentData = rptListCover;
                //}

                //List<ReportParameterObject> listSubReportDataSource2 = new List<ReportParameterObject>();
                //listSubReportDataSource2.Add(new ReportParameterObject() { SubReportName = "CTR090_1", Value = rptListInst });
                //listSubReportDataSource2.Add(new ReportParameterObject() { SubReportName = "CTR090_2", Value = rptListInst });
                //listSubReportDataSource2.Add(new ReportParameterObject() { SubReportName = "CTR090_3", Value = rptListInst });

                //doDoc2.SubReportDataSource = listSubReportDataSource2;

                //List<ReportParameterObject> listMainReportParam2 = new List<ReportParameterObject>();
                //listMainReportParam2.Add(new ReportParameterObject() { ParameterName = "FlagOn", Value = FlagType.C_FLAG_ON });
                //doDoc2.MainReportParam = listMainReportParam2;
                //doDoc2.SubReportParam = null;


                //Stream stream = documentHandler.GenerateDocument(doDoc, doDoc2);

                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public ActionResult CTR070_ChangeFeeMemorandum(int iDocID)
        {
            IContractDocumentHandler contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            List<RPTChangeFeeMemoDo> rptList = contractDocHandler.GetRptChangeFeeMemoData(iDocID);

            ReportDocument rptH = new ReportDocument();
            string path = ReportUtil.GetReportPath("Reports/CTR070_ChangeFeeMemorandum.rpt", Server.MapPath("/"));
            //string path = ReportUtil.GetReportTemplatePath("CTR070_ChangeFeeMemorandum.rpt");

            rptH.Load(path);
            rptH.SetDataSource(rptList);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }

        public ActionResult CTR080_QuotationForCancelContractMemorandum(int iDocID)
        {
            IContractDocumentHandler contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;

            List<RPTCancelContractMemoDo> rptList = contractDocHandler.GetRptCancelContractMemoData(iDocID);
            List<RPTCancelContractMemoDetailDo> rptListDetail = contractDocHandler.GetRptCancelContractMemoDetail(iDocID);

            bool isShowDefaultData = false;
            if (rptListDetail == null || rptListDetail.Count == 0)
            {
                rptListDetail = GetDefaultCancelContractMemoDetailData();
                isShowDefaultData = true;
            }

            ReportDocument rptH = new ReportDocument();
            string path = ReportUtil.GetReportPath("Reports/CTR080_QuotationForCancelContractMemorandum.rpt", Server.MapPath("/"));
            //string path = ReportUtil.GetReportTemplatePath("CTR080_QuotationForCancelContractMemorandum.rpt");

            rptH.Load(path);
            rptH.SetDataSource(rptList);
            rptH.Subreports["CTR080_1"].SetDataSource(rptListDetail);

            rptH.SetParameterValue("ShowDefaultData", isShowDefaultData, "CTR080_1");

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
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

        public ActionResult CTR090_CoverLetter(int iDocID)
        {
            try
            {
                IContractDocumentHandler contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;

                List<RPTCoverLetterDo> rptListCover = contractDocHandler.GetRptCoverLetterData(iDocID);
                List<RPTInstrumentDetailDo> rptListInst = contractDocHandler.GetRptInstrumentDetailData(iDocID);

                ReportDocument rptH = new ReportDocument();
                string path = ReportUtil.GetReportPath("Reports/CTR090_CoverLetter.rpt", Server.MapPath("/"));
                //string path = ReportUtil.GetReportTemplatePath("CTR090_CoverLetter.rpt");

                rptH.Load(path);
                rptH.SetDataSource(rptListCover);
                rptH.Subreports["CTR090_1"].SetDataSource(rptListInst);
                rptH.Subreports["CTR090_2"].SetDataSource(rptListInst);
                rptH.Subreports["CTR090_3"].SetDataSource(rptListInst);

                rptH.SetParameterValue("FlagOn", FlagType.C_FLAG_ON);
                rptH.SetParameterValue("ShowInstrument", (rptListInst != null && rptListInst.Count > 0));
                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                rptH.Close();


                //IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                //doDocumentDataGenerate doDoc = new doDocumentDataGenerate();
                //if (rptListCover.Count > 0)
                //{
                //    rptListCover[0].DocumentCode = "CTR090";
                //    doDoc.DocumentNo = rptListCover[0].DocNo;
                //    doDoc.DocumentCode = rptListCover[0].DocumentCode;
                //    doDoc.DocumentData = rptListCover;
                //}

                //List<ReportParameterObject> listSubReportDataSource = new List<ReportParameterObject>();
                //listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "CTR090_1", Value = rptListInst });
                //listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "CTR090_2", Value = rptListInst });
                //listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "CTR090_3", Value = rptListInst });

                //doDoc.SubReportDataSource = listSubReportDataSource;

                //List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
                //listMainReportParam.Add(new ReportParameterObject() { ParameterName = "FlagOn", Value = FlagType.C_FLAG_ON });
                //doDoc.MainReportParam = listMainReportParam;
                //doDoc.SubReportParam = null;

                //Stream stream = documentHandler.GenerateDocument(doDoc);


                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public byte[] GetMaintenanceCheckupSlipReport(string contractCode, string productCode, DateTime? instructionDate)
        {
            List<string> listInstrumentContract;
            List<tbt_MaintenanceCheckupDetails> listMaintenanceCheckupDetails;
            IMaintenanceHandler maintenanceHandler;
            IDocumentHandler documentHandler;
            IReportHandler reportHandler;
            doDocumentDataGenerate doDocumentData;
            RPTSlipReport rptSlip;

            try
            {
                listInstrumentContract = new List<string>();
                rptSlip = new RPTSlipReport();
                doDocumentData = new doDocumentDataGenerate();
                maintenanceHandler = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                return null; //reportHandler.GetMaintenanceCheckupSlip(null, null, null, OWNER_PASSWORD);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetMaintenanceCheckupList()
        {
            List<string> listInstrumentContract;
            List<tbt_MaintenanceCheckupDetails> listMaintenanceCheckupDetails;
            List<Object[]> ListObject;
            tbt_MaintenanceCheckup tbtMaintenanceCheckup;
            Object[] obj;
            IReportHandler reportHandler;

            try
            {
                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                List<Object[]> list = new List<object[]>();
                DateTime dt = new DateTime(2011, 1, 1);

                obj = new Object[3];
                obj[0] = "MA0002700012";
                obj[1] = "001";
                obj[2] = dt;

                list.Add(obj);


                return ""; //return reportHandler.GetMaintenanceCheckupList(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult CTR_GenerateReport(int? iDocID, string strDocNo, string strDocumentCode)
        {
            ObjectResultData res = new ObjectResultData();
            IContractDocumentHandler conDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;

            try
            {
                conDocHandler.CreateContractReport(iDocID, strDocNo, strDocumentCode);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult CTP060_UpdateEmailContentOfNotifyEmail()
        {
            ObjectResultData res = new ObjectResultData();
            IContractHandler conHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            IRentralContractHandler rentalConHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            IEmployeeMasterHandler empMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            IBillingMasterHandler billingmasterhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            IBillingHandler billinghandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

            List<tbm_Customer> dtCustomer;
            List<doGetTbm_Site> dtSite;
            List<tbm_Employee> dtEmployee;
            decimal? contractFeeBeforeChange;
            decimal? ChangeContractFee;

            try
            {
                CommonUtil comUtil = new CommonUtil();
                doNotifyChangeFeeContract doNotifyEmail = new doNotifyChangeFeeContract();
                List<tbt_ContractEmail> updateContractEmailList = new List<tbt_ContractEmail>();

                using (TransactionScope scope = new TransactionScope())
                {
                    List<tbt_ContractEmail> contractEmailList = conHandler.GetUnsentNotifyEmail();
                    foreach (tbt_ContractEmail data in contractEmailList)
                    {
                        List<tbt_BillingBasic> doBillingBasic = billinghandler.GetTbt_BillingBasic(data.ContractCode, "01");
                        List<tbt_BillingTarget> doBillingTarget = billinghandler.GetTbt_BillingTarget(doBillingBasic[0].BillingTargetCode, null, null);

                        dsRentalContractData dsRentalContract = rentalConHandler.GetEntireContract(data.ContractCode, data.OCC);
                        if (dsRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate != null)
                        {
                            var operationOfficeDat = officehandler.GetTbm_Office(dsRentalContract.dtTbt_RentalContractBasic[0].OperationOfficeCode);
                            var billingOfficeDat = officehandler.GetTbm_Office(doBillingTarget[0].BillingOfficeCode);

                            EmailTemplateUtil mailUtil = new EmailTemplateUtil(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_CHANGE_FEE);
                            dtCustomer = masterHandler.GetTbm_Customer(dsRentalContract.dtTbt_RentalContractBasic[0].ContractTargetCustCode);
                            dtSite = masterHandler.GetTbm_Site(dsRentalContract.dtTbt_RentalContractBasic[0].SiteCode);
                            dtEmployee = empMasterHandler.GetTbm_Employee(data.CreateBy);
                            contractFeeBeforeChange = rentalConHandler.GetContractFeeBeforeChange(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC, dsRentalContract);

                            doNotifyEmail.ContractCode = comUtil.ConvertContractCode(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                            doNotifyEmail.ContractTargetNameEN = dtCustomer[0].CustFullNameEN;
                            doNotifyEmail.ContractTargetNameLC = dtCustomer[0].CustFullNameLC;
                            doNotifyEmail.SiteNameEN = dtSite[0].SiteNameEN;
                            doNotifyEmail.SiteNameLC = dtSite[0].SiteNameLC;
                            doNotifyEmail.ChangeDateOfContractFee = CommonUtil.TextDate(dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate);
                            doNotifyEmail.ContractFeeBeforeChange = CommonUtil.TextNumeric(contractFeeBeforeChange);

                            if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                                ChangeContractFee = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop;
                            else
                                ChangeContractFee = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee;

                            doNotifyEmail.ContractFeeAfterChange = CommonUtil.TextNumeric(ChangeContractFee);
                            doNotifyEmail.ReturnToOriginalFeeDate = CommonUtil.TextDate(dsRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate);
                            doNotifyEmail.OperationOfficeEN = operationOfficeDat[0].OfficeNameEN;
                            doNotifyEmail.OperationOfficeLC = operationOfficeDat[0].OfficeNameLC;
                            doNotifyEmail.RegisterChangeEmpNameEN = dtEmployee[0].EmpFirstNameEN + ' ' + dtEmployee[0].EmpLastNameEN;
                            doNotifyEmail.RegisterChangeEmpNameLC = dtEmployee[0].EmpFirstNameLC + ' ' + dtEmployee[0].EmpLastNameLC;
                            doNotifyEmail.BillingOfficeEN = billingOfficeDat[0].OfficeNameEN;
                            doNotifyEmail.BillingOfficeLC = billingOfficeDat[0].OfficeNameLC;

                            var mailTemplate = mailUtil.LoadTemplate(doNotifyEmail);
                            data.EmailContent = mailTemplate.TemplateContent;

                            tbt_ContractEmail conEmailTemp = CommonUtil.CloneObject<tbt_ContractEmail, tbt_ContractEmail>(data);
                            updateContractEmailList.Add(conEmailTemp);
                        }
                    }

                    if (updateContractEmailList != null && updateContractEmailList.Count > 0)
                        conHandler.UpdateTbt_ContractEmail(updateContractEmailList);

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
    }

}
