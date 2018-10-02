using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

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
using SECOM_AJIS.DataEntity.Installation;
using System.Linq;

namespace SECOM_AJIS.Presentation.Installation.Controllers
{
    public partial class InstallationController : BaseController
    { 
        public ActionResult ISR010_GetNewInstallationSlip(string strSlipNo)
        {
            IReportHandler reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
            List<RPTNewRentalSlipDo> rptList = reportHandler.GetRptNewRentalSlipReport(strSlipNo);

            IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL);
            
            ReportDocument rptH = new ReportDocument();
            //string path = ReportUtil.GetReportTemplatePath("ISR010_NewInstallationSlip.rpt");
            string path = ReportUtil.GetReportPath("Reports/ISR010_NewInstallationSlip.rpt", Server.MapPath("/"));            

            rptH.Load(path);

            List<RPTNewRentalSlipDo> lst = new List<RPTNewRentalSlipDo>();
            if (rptList != null && rptList.Count > 0)
            {
                lst.Add(rptList[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }
            }
            else
            {
                rptList.Add(new RPTNewRentalSlipDo());
            }

            rptH.SetDataSource(lst);
            rptH.Subreports["Page1"].SetDataSource(rptList);
            rptH.Subreports["Page2"].SetDataSource(rptList);
            rptH.Subreports["Page3"].SetDataSource(rptList);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }

        public ActionResult ISR020_GetChangeSlipData(string strSlipNo)
        {
            IReportHandler reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
            List<RPTChangeSlipDo> rptList = reportHandler.GetRptChangeSlipReport(strSlipNo);

            IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP);

            ReportDocument rptH = new ReportDocument();
            //string path = ReportUtil.GetReportTemplatePath("ISR020_ChangeInstallationSlip.rpt");
            string path = ReportUtil.GetReportPath("Reports/ISR020_ChangeInstallationSlip.rpt", Server.MapPath("/"));

            rptH.Load(path);
            List<RPTChangeSlipDo> lst = new List<RPTChangeSlipDo>();
            if (rptList != null && rptList.Count > 0)
            {
                lst.Add(rptList[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }
            }
            else
            {
                rptList.Add(new RPTChangeSlipDo());
            }

            rptH.SetDataSource(lst);
            rptH.Subreports["Page1"].SetDataSource(rptList);
            rptH.Subreports["Page2"].SetDataSource(rptList);
            rptH.Subreports["Page3"].SetDataSource(rptList);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }

        public ActionResult ISR030_GetRemoveSlipData(string strSlipNo)
        {
            IReportHandler reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
            List<RPTRemoveSlipDo> rptList = reportHandler.GetRptRemoveSlipReport(strSlipNo);

            IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_REMOVAL_INSTALL_SLIP);

            ReportDocument rptH = new ReportDocument();
            //string path = ReportUtil.GetReportTemplatePath("Reports/ISR030_RemovalInstallationSlip.rpt");
            string path = ReportUtil.GetReportPath("Reports/ISR030_RemovalInstallationSlip.rpt", Server.MapPath("/"));

            rptH.Load(path);
            List<RPTRemoveSlipDo> lst = new List<RPTRemoveSlipDo>();
            if (rptList != null && rptList.Count > 0)
            {
                lst.Add(rptList[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }
            }
            else
            {
                rptList.Add(new RPTRemoveSlipDo());
            }

            rptH.SetDataSource(lst);
            rptH.Subreports["Page1"].SetDataSource(rptList);
            rptH.Subreports["Page2"].SetDataSource(rptList);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }

        public ActionResult ISR040_GetNewInstallationSaleSlipData(string strSlipNo)
        {
            IReportHandler reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
            List<RPTNewSaleSlipDo> rptList = reportHandler.GetRptNewSaleSlipReport(strSlipNo);

            IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE);

            ReportDocument rptH = new ReportDocument();
            //string path = ReportUtil.GetReportTemplatePath("Reports/ISR040_NewInstallationSaleSlip.rpt");
            string path = ReportUtil.GetReportPath("Reports/ISR040_NewInstallationSaleSlip.rpt", Server.MapPath("/"));

            rptH.Load(path);

            List<RPTNewSaleSlipDo> lst = new List<RPTNewSaleSlipDo>();
            if (rptList != null && rptList.Count > 0)
            {
                lst.Add(rptList[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }
            }
            else
            {
                rptList.Add(new RPTNewSaleSlipDo());
            }

            rptH.SetDataSource(lst);
            rptH.Subreports["Page1"].SetDataSource(rptList);
            rptH.Subreports["Page2"].SetDataSource(rptList);
            rptH.Subreports["Page3"].SetDataSource(rptList);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }        

        public ActionResult ISR050_GetRptPOSubPriceData(string strMaintenanceNo, string strSubcontractorCode)
        {
            strMaintenanceNo = "5020N20110030";
            strSubcontractorCode = "00002";
            IReportHandler reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
            List<RPTPOSubPriceDo> rptList = reportHandler.GetRptPOSubPriceData(strMaintenanceNo,strSubcontractorCode);

            IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_PO);

            ReportDocument rptH = new ReportDocument();
           
            string path = ReportUtil.GetReportPath("Reports/ISR050_InstallationPO.rpt", Server.MapPath("/"));

            rptH.Load(path);

            List<RPTPOSubPriceDo> lst = new List<RPTPOSubPriceDo>();
            lst.Add(rptList[0]);

            if (dLst.Count > 0)
            {
                lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                lst[0].DocumentVersion = dLst[0].DocumentVersion;
            }

            rptH.SetDataSource(lst);          

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }

        public ActionResult ISR060_GetRptInstallationRequestData(string strMaintenanceNo)
        {
            strMaintenanceNo = "5020N20110030";
            IReportHandler reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
            List<RPTInstallRequestDo> rptList = reportHandler.GetRptInstallationRequestData(strMaintenanceNo);

            IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_REQUEST);

            ReportDocument rptH = new ReportDocument();

            string path = ReportUtil.GetReportPath("Reports/ISR060_InstallationRequest.rpt", Server.MapPath("/"));

            rptH.Load(path);

            List<RPTInstallRequestDo> lst = new List<RPTInstallRequestDo>();
            lst.Add(rptList[0]);

            if (dLst.Count > 0)
            {
                lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                lst[0].DocumentVersion = dLst[0].DocumentVersion;
            }

            rptH.SetDataSource(lst);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }
                
        public ActionResult ISR070_GetRptInstallSpecCompleteData(string strMaintenanceNo, string strSubcontractorCode)
        {
            strMaintenanceNo = "5020N20110030";
            strSubcontractorCode = "00002";
            IReportHandler reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
            List<RPTInstallSpecCompleteDo> rptList = reportHandler.GetRptInstallSpecCompleteData(strMaintenanceNo, strSubcontractorCode);

            IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_SPEC_AND_COMPLETE);

            ReportDocument rptH = new ReportDocument();

            string path = ReportUtil.GetReportPath("Reports/ISR070_InstallSpecCompleteData.rpt", Server.MapPath("/"));

            rptH.Load(path);

            List<RPTInstallSpecCompleteDo> lst = new List<RPTInstallSpecCompleteDo>();
            lst.Add(rptList[0]);

            if (dLst.Count > 0)
            {
                lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                lst[0].DocumentVersion = dLst[0].DocumentVersion;
            }

            rptH.SetDataSource(lst);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }
                
        public ActionResult ISR080_GetRptIECheckSheetData(string strMaintenanceNo, string strSubcontractorCode)
        {
            strMaintenanceNo = "5020N20110030";
            strSubcontractorCode = "00002";
            IReportHandler reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
            List<RPTIECheckSheetDo> rptList = reportHandler.GetRptIECheckSheetData(strMaintenanceNo, strSubcontractorCode);

            IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_IE_CHECK_SHEET);

            ReportDocument rptH = new ReportDocument();

            string path = ReportUtil.GetReportPath("Reports/ISR080_IECheckSheet.rpt", Server.MapPath("/"));

            rptH.Load(path);

            List<RPTIECheckSheetDo> lst = new List<RPTIECheckSheetDo>();
            lst.Add(rptList[0]);

            if (dLst.Count > 0)
            {
                lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                lst[0].DocumentVersion = dLst[0].DocumentVersion;
            }

            rptH.SetDataSource(lst);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }
          
        public ActionResult ISR090_GetRptInstallCompleteConfirmData(string strSlipNo)
        {
            strSlipNo = "401029201112043";
          
            IReportHandler reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
            List<RPTInstallCompleteDo> rptList = reportHandler.GetRptInstallCompleteConfirmData(strSlipNo);

            IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_COMPLETE_CONFIRM);

            ReportDocument rptH = new ReportDocument();

            string path = ReportUtil.GetReportPath("Reports/ISR090_InstallCompleteConfirmData.rpt", Server.MapPath("/"));

            rptH.Load(path);

            List<RPTInstallCompleteDo> lst = new List<RPTInstallCompleteDo>();
            int i = 0;
            foreach (RPTInstallCompleteDo rptDataRow in rptList)
            {
                lst.Add(rptDataRow);
                if (dLst.Count > 0)
                {
                    lst[i].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[i].DocumentVersion = dLst[0].DocumentVersion;
                }
                i++;
            }

            rptH.SetDataSource(lst);
            rptH.SetParameterValue("C_RENTAL_INSTALL_TYPE_REMOVE_ALL", RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL);
            rptH.SetParameterValue("C_SALE_INSTALL_TYPE_REMOVE_ALL", SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL);
            

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }
              
        public ActionResult ISR100_GetRptAcceptanceInspectionNotice(string strMaintenanceNo, string strSubcontractorCode)
        {
            strMaintenanceNo = "5020N20110030";
            strSubcontractorCode = "00002";
            IReportHandler reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
            List<RPTAcceptInspecDo> rptList = reportHandler.GetRptAcceptInspecNocticeData(strMaintenanceNo, strSubcontractorCode);

            IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_ACCEPT_INSPECT_NOTICE);

            ReportDocument rptH = new ReportDocument();

            string path = ReportUtil.GetReportPath("Reports/ISR100_AcceptanceInspectionNotice.rpt", Server.MapPath("/"));

            rptH.Load(path);

            List<RPTAcceptInspecDo> lst = new List<RPTAcceptInspecDo>();
            int i = 0;
            foreach (RPTAcceptInspecDo rptDataRow in rptList)
            {
                lst.Add(rptDataRow);
                if (dLst.Count > 0)
                {
                    lst[i].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[i].DocumentVersion = dLst[0].DocumentVersion;
                }
                i++;
            }
            rptH.SetDataSource(lst);

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.Close();

            return File(stream, "application/pdf");
        }

        #region Unused
        //public ActionResult ISR110_GetRptDeliveryConfirmData(string strSlipNo)
        //{
        //    strSlipNo = "401029201112043";
        //    IReportHandler reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
        //    List<RPTDeliveryConfirmDo> rptList = reportHandler.GetRptDeliveryConfirmData(strSlipNo);

        //    IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
        //    List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY);

        //    ReportDocument rptH = new ReportDocument();

        //    string path = ReportUtil.GetReportPath("Reports/ISR110_DeliveryConfirmData.rpt", Server.MapPath("/"));

        //    rptH.Load(path);

        //    List<RPTDeliveryConfirmDo> lst = new List<RPTDeliveryConfirmDo>();
        //    int i = 0;
        //    foreach (RPTDeliveryConfirmDo rptDataRow in rptList)
        //    {
        //        lst.Add(rptDataRow);
        //        if (dLst.Count > 0)
        //        {
        //            lst[i].DocumentNameEN = dLst[0].DocumentNameEN;
        //            lst[i].DocumentVersion = dLst[0].DocumentVersion;
        //        }
        //        i++;
        //    }
        //    rptH.SetDataSource(lst);
        //    rptH.Subreports["Page1"].SetDataSource(lst);
        //    rptH.Subreports["Page2"].SetDataSource(lst);
        //    Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    rptH.Close();

        //    return File(stream, "application/pdf");
        //}
        #endregion

        public ActionResult GenerateInstallationSlipDocBySlipNo()
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IInstallationDocumentHandler docHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;
            IInstallationHandler installHand = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

            try
            {
                //using (TransactionScope scope = new TransactionScope())
                //{
                List<tbt_InstallationSlip> installList = installHand.GetTbt_InstallationSlip(null);
                List<tbt_DocumentList> documentList = comHandler.GetTbt_DocumentList(null, null, null);

                installList = (from t in installList
                               where //(
                                        //t.SlipStatus == "01" 
                                        //|| t.SlipStatus == "02"
                                        //|| t.SlipStatus == "03"
                                        //|| t.SlipStatus == "04"
                                        //|| t.SlipStatus == "06")
                                //&& t.SlipIssueFlag == true
                                //&& t.CreateDate >= new DateTime(2013,7,24,12,20,0)
                                t.CreateBy == "INITIAL"
                                && documentList.Any(d => (d.DocumentNo == t.SlipNo))
                               select t).ToList<tbt_InstallationSlip>();

                foreach (tbt_InstallationSlip data in installList)
                {
                    tbt_InstallationSlip dataSlip = installHand.GetTbt_InstallationSlipData(data.SlipNo);

                    //if (dataSlip != null 
                    //        && dataSlip.SlipStatus != SlipStatus.C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT
                    //        && dataSlip.SlipStatus != SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT //Modify by Jutarat A. on 07022013
                    //        && dataSlip.SlipStatus != SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT //Add by Jutarat A. on 17062013
                    //        && dataSlip.SlipStatus != SlipStatus.C_SLIP_STATUS_STOCK_OUT) //Add by Jutarat A. on 24062013
                    //{
                    //    return Json(res);
                    //}

                    if (dataSlip.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW || dataSlip.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW)
                    {
                        //6.1.1.	Call		InstallationDocumentHandler.CreateISR010
                        //Parameter	doTbt_InstallationSlip.SlipNo
                        //Return		fsPDFFileISR010
                        docHand.CreateInstallationReport(dataSlip.SlipNo, DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL);
                    }

                    if (dataSlip.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING ||
                        dataSlip.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW ||
                        dataSlip.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                        dataSlip.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE ||
                        dataSlip.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE ||
                        dataSlip.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL ||
                        dataSlip.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL ||
                        dataSlip.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_CHANGE_WIRING ||
                        dataSlip.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE ||
                        dataSlip.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_MOVE ||
                        dataSlip.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_PARTIAL_REMOVE
                        )
                    {
                        //6.2.1.	Call		InstallationDocumentHandler.CreateISR020
                        //Parameter	doTbt_InstallationSlip.SlipNo
                        //Return		fsPDFFileISR020
                        docHand.CreateInstallationReport(dataSlip.SlipNo, DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP);
                    }
                    if (dataSlip.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL ||
                        dataSlip.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL)
                    {
                        //6.3.1.	Call		InstallationDocumentHandler.CreateISR030
                        //Parameter	doTbt_InstallationSlip.SlipNo
                        //Return		fsPDFFileISR030
                        docHand.CreateInstallationReport(dataSlip.SlipNo, DocumentCode.C_DOCUMENT_CODE_REMOVAL_INSTALL_SLIP);
                    }
                    if (dataSlip.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_NEW ||
                        dataSlip.InstallationType == SaleInstallationType.C_SALE_INSTALL_TYPE_ADD)
                    {
                        //6.4.1.	Call		InstallationDocumentHandler.CreateISR040
                        //Parameter	doTbt_InstallationSlip.SlipNo
                        //Return		fsPDFFileISR040
                        docHand.CreateInstallationReport(dataSlip.SlipNo, DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE);
                    }

                    if (dataSlip != null && dataSlip.SlipIssueFlag != FlagType.C_FLAG_ON)
                    {
                        dataSlip.SlipIssueFlag = FlagType.C_FLAG_ON;
                        int updatedRpw = installHand.UpdateTbt_InstallationSlip(dataSlip);
                    }                    
                }

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
    }    
}
