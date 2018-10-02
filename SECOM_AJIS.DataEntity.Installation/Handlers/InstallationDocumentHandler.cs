using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Configuration;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;

using CSI.WindsorHelper;
using System.Reflection;
using System.IO;
using System.Web.Mvc;
using SpreadsheetLight;
using System.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;



namespace SECOM_AJIS.DataEntity.Installation
{
    class InstallationDocumentHandler : BizISDataEntities, IInstallationDocumentHandler
    {
        public Stream CreateInstallationReport(string strSlipNo, string strDocumentCode)
        {
            Stream stream = null;
            InstallationReportData installationData = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            doDocumentDataGenerate doDocument1;
            doDocumentDataGenerate doDocument2;
            doDocumentDataGenerate doDocument3;
            doDocumentDataGenerate doDocument4;

            List<ReportParameterObject> mainReportParam;
            List<ReportParameterObject> mainReportParam3;
            List<ReportParameterObject> mainReportParam4;

            List<ReportParameterObject> listReportParam;
            List<ReportParameterObject> listReportParam1;
            List<ReportParameterObject> listReportParam2;
            List<ReportParameterObject> listReportParam3;
            List<ReportParameterObject> listReportParam4;

            List<tbm_DocumentTemplate> dLst;
            List<doDocumentDataGenerate> listSlaveDoc;

            string strDocumentCode1 = string.Empty;
            string strDocumentCode2 = string.Empty;
            string strDocumentCode3 = string.Empty;
            string strDocumentCode4 = string.Empty;

            object rptList = null;
            object rptListB = null;
            object rptListC = null;
            object rptList3 = null;
            object rptList4 = null;
            object rptList5 = null;

            try
            {
                if (strSlipNo == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                mainReportParam = new List<ReportParameterObject>();
                mainReportParam3 = new List<ReportParameterObject>();
                mainReportParam4 = new List<ReportParameterObject>();

                listReportParam = new List<ReportParameterObject>();
                listReportParam1 = new List<ReportParameterObject>();
                listReportParam2 = new List<ReportParameterObject>();
                listReportParam3 = new List<ReportParameterObject>();
                listReportParam4 = new List<ReportParameterObject>();

                listSlaveDoc = new List<doDocumentDataGenerate>();

                //======================== ISR010 ==========================
                if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL)
                {
                    rptList = reportHandler.GetRptNewRentalSlipReport(strSlipNo);
                    if (rptList != null && ((List<RPTNewRentalSlipDo>)rptList).Count > 0)
                    {
                        installationData = CommonUtil.CloneObject<RPTNewRentalSlipDo, InstallationReportData>(((List<RPTNewRentalSlipDo>)rptList)[0]);

                        List<RPTNewRentalSlipDo> rptListDetail = (List<RPTNewRentalSlipDo>)rptList;
                        listReportParam.Add(new ReportParameterObject() { SubReportName = "ISR010_1", Value = rptListDetail });
                        listReportParam.Add(new ReportParameterObject() { SubReportName = "ISR010_2", Value = rptListDetail });

                        listReportParam1.Add(new ReportParameterObject() { SubReportName = "ISR011_1", Value = rptListDetail });
                        listReportParam1.Add(new ReportParameterObject() { SubReportName = "ISR011_2", Value = rptListDetail });

                        listReportParam2.Add(new ReportParameterObject() { SubReportName = "ISR012_1", Value = rptListDetail });
                        listReportParam2.Add(new ReportParameterObject() { SubReportName = "ISR012_2", Value = rptListDetail });

                        bool isShowInstrument = true;
                        if (rptListDetail.Count == 1)
                        {
                            if (rptListDetail[0].InstrumentCode == "-")
                                isShowInstrument = false;
                        }
                        mainReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });

                        List<RPTNewRentalSlipDo> lst = new List<RPTNewRentalSlipDo>();
                        lst.Add(((List<RPTNewRentalSlipDo>)rptList)[0]);

                        dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL);
                        if (dLst.Count > 0)
                        {
                            lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                            lst[0].DocumentVersion = dLst[0].DocumentVersion;
                        }

                        rptList = lst;

                        //Add report ISR013
                        rptList3 = reportHandler.GetRptInstallCompleteConfirmData(strSlipNo);
                        if (rptList3 != null && ((List<RPTInstallCompleteDo>)rptList3).Count > 0)
                        {
                            List<RPTInstallCompleteDo> lstISR013 = (List<RPTInstallCompleteDo>)rptList3;

                            isShowInstrument = true;
                            if (lstISR013.Count == 1)
                            {
                                if (lstISR013[0].InstrumentCode == "-")
                                    isShowInstrument = false;
                            }
                            mainReportParam3.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });
                            mainReportParam3.Add(new ReportParameterObject() { ParameterName = "C_RENTAL_INSTALL_TYPE_REMOVE_ALL", Value = RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL });
                            mainReportParam3.Add(new ReportParameterObject() { ParameterName = "C_SALE_INSTALL_TYPE_REMOVE_ALL", Value = SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL });

                            List<tbm_DocumentTemplate> dLstISR013 = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL_D);
                            if (dLstISR013.Count > 0)
                            {
                                foreach (var row in lstISR013)
                                {
                                    row.DocumentNameEN = dLstISR013[0].DocumentNameEN;
                                    row.DocumentVersion = dLstISR013[0].DocumentVersion;
                                }
                            }

                            strDocumentCode3 = DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL_D;
                        }
                        //End Add
                    }
                    else
                    {
                        ((List<RPTNewRentalSlipDo>)rptList).Add(new RPTNewRentalSlipDo());
                        mainReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = false });
                    }

                    strDocumentCode1 = DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL_B;
                    strDocumentCode2 = DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL_C;

                }//======================== ISR020 ==========================
                else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP)
                {
                    rptList = reportHandler.GetRptChangeSlipReport(strSlipNo);
                    if (rptList != null && ((List<RPTChangeSlipDo>)rptList).Count > 0)
                    {
                        installationData = CommonUtil.CloneObject<RPTChangeSlipDo, InstallationReportData>(((List<RPTChangeSlipDo>)rptList)[0]);

                        List<RPTChangeSlipDo> rptListDetail = (List<RPTChangeSlipDo>)rptList;
                        listReportParam.Add(new ReportParameterObject() { SubReportName = "ISR020_1", Value = rptListDetail });
                        listReportParam1.Add(new ReportParameterObject() { SubReportName = "ISR021_1", Value = rptListDetail });
                        listReportParam2.Add(new ReportParameterObject() { SubReportName = "ISR022_1", Value = rptListDetail });

                        bool isShowInstrument = true;
                        if (rptListDetail.Count == 1)
                        {
                            if (rptListDetail[0].InstrumentCode == "-")
                                isShowInstrument = false;
                        }
                        mainReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });

                        List<RPTChangeSlipDo> lst = new List<RPTChangeSlipDo>();
                        lst.Add(((List<RPTChangeSlipDo>)rptList)[0]);

                        dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP);
                        if (dLst.Count > 0)
                        {
                            lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                            lst[0].DocumentVersion = dLst[0].DocumentVersion;
                        }

                        rptList = lst;

                        if (lst != null && lst.Count > 0)
                        {
                            //Change spec
                            //if (lst[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                            //{
                            //Add report ISR023
                            rptList3 = reportHandler.GetRptInstallCompleteConfirmData(strSlipNo);
                            if (rptList3 != null && ((List<RPTInstallCompleteDo>)rptList3).Count > 0)
                            {
                                List<RPTInstallCompleteDo> lstISR023 = (List<RPTInstallCompleteDo>)rptList3;

                                isShowInstrument = true;
                                if (lstISR023.Count == 1)
                                {
                                    if (lstISR023[0].InstrumentCode == "-")
                                        isShowInstrument = false;
                                }
                                mainReportParam3.Add(new ReportParameterObject() { ParameterName = "ReportType", Value = "1" });
                                mainReportParam3.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });
                                mainReportParam3.Add(new ReportParameterObject() { ParameterName = "C_RENTAL_INSTALL_TYPE_REMOVE_ALL", Value = RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL });
                                mainReportParam3.Add(new ReportParameterObject() { ParameterName = "C_SALE_INSTALL_TYPE_REMOVE_ALL", Value = SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL });

                                List<tbm_DocumentTemplate> dLstISR023 = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP_D);
                                if (dLstISR023.Count > 0)
                                {
                                    foreach (var row in lstISR023)
                                    {
                                        row.DocumentNameEN = dLstISR023[0].DocumentNameEN;
                                        row.DocumentVersion = dLstISR023[0].DocumentVersion;
                                    }
                                }

                                strDocumentCode3 = DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP_D;

                                // ISR023 - Completion Confirmation
                                rptList4 = rptList3;

                                mainReportParam4.Add(new ReportParameterObject() { ParameterName = "ReportType", Value = "2" });
                                mainReportParam4.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });
                                mainReportParam4.Add(new ReportParameterObject() { ParameterName = "C_RENTAL_INSTALL_TYPE_REMOVE_ALL", Value = RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL });
                                mainReportParam4.Add(new ReportParameterObject() { ParameterName = "C_SALE_INSTALL_TYPE_REMOVE_ALL", Value = SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL });

                                strDocumentCode4 = DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP_D;
                            }
                            //End Add
                            //}
                            //else if (lst[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                            //{
                            //    //Add report ISR110
                            //    rptList3 = reportHandler.GetRptDeliveryConfirmData(strSlipNo);
                            //    if (rptList3 != null || ((List<RPTDeliveryConfirmDo>)rptList3).Count > 0)
                            //    {
                            //        List<RPTDeliveryConfirmDo> lstSubReport110 = (List<RPTDeliveryConfirmDo>)rptList3;
                            //        listReportParam3.Add(new ReportParameterObject() { ParameterName = "Page1", Value = lstSubReport110 });
                            //        listReportParam3.Add(new ReportParameterObject() { ParameterName = "Page2", Value = lstSubReport110 });

                            //        List<RPTDeliveryConfirmDo> lst110 = new List<RPTDeliveryConfirmDo>();
                            //        lst110.Add(((List<RPTDeliveryConfirmDo>)rptList3)[0]);

                            //        List<tbm_DocumentTemplate> dLst110 = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY);
                            //        if (dLst110.Count > 0)
                            //        {
                            //            lst110[0].DocumentNameEN = dLst110[0].DocumentNameEN;
                            //            lst110[0].DocumentVersion = dLst110[0].DocumentVersion;
                            //        }

                            //        rptList3 = lst110;
                            //        strDocumentCode3 = DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY;
                            //    }
                            //    //End Add
                            //}
                        }
                    }
                    else
                    {
                        ((List<RPTChangeSlipDo>)rptList).Add(new RPTChangeSlipDo());
                        mainReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = false });
                    }

                    strDocumentCode1 = DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP_B;
                    strDocumentCode2 = DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP_C;

                }//======================== ISR030 ==========================
                else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_REMOVAL_INSTALL_SLIP)
                {
                    rptList = reportHandler.GetRptRemoveSlipReport(strSlipNo);
                    if (rptList != null && ((List<RPTRemoveSlipDo>)rptList).Count > 0)
                    {
                        installationData = CommonUtil.CloneObject<RPTRemoveSlipDo, InstallationReportData>(((List<RPTRemoveSlipDo>)rptList)[0]);

                        List<RPTRemoveSlipDo> rptListDetail = (List<RPTRemoveSlipDo>)rptList;
                        listReportParam.Add(new ReportParameterObject() { SubReportName = "ISR030_1", Value = rptListDetail });
                        listReportParam.Add(new ReportParameterObject() { SubReportName = "ISR030_2", Value = rptListDetail });

                        listReportParam1.Add(new ReportParameterObject() { SubReportName = "ISR031_1", Value = rptListDetail });
                        listReportParam1.Add(new ReportParameterObject() { SubReportName = "ISR031_2", Value = rptListDetail });

                        bool isShowInstrument = true;
                        if (rptListDetail.Count == 1)
                        {
                            if (rptListDetail[0].InstrumentCode == "-")
                                isShowInstrument = false;
                        }
                        mainReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });

                        List<RPTRemoveSlipDo> lst = new List<RPTRemoveSlipDo>();
                        lst.Add(((List<RPTRemoveSlipDo>)rptList)[0]);

                        dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_REMOVAL_INSTALL_SLIP);
                        if (dLst.Count > 0)
                        {
                            lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                            lst[0].DocumentVersion = dLst[0].DocumentVersion;
                        }

                        rptList = lst;

                        if (lst != null && lst.Count > 0)
                        {
                            //Change spec
                            //if (lst[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                            //{
                            //Add report ISR090
                            rptList3 = reportHandler.GetRptInstallCompleteConfirmData(strSlipNo);
                            if (rptList3 != null && ((List<RPTInstallCompleteDo>)rptList3).Count > 0)
                            {
                                List<RPTInstallCompleteDo> lstSubReport090 = (List<RPTInstallCompleteDo>)rptList3;
                                listReportParam3.Add(new ReportParameterObject() { SubReportName = "ISR090_1", Value = lstSubReport090 });

                                isShowInstrument = true;
                                if (lstSubReport090.Count == 1)
                                {
                                    if (lstSubReport090[0].InstrumentCode == "-")
                                        isShowInstrument = false;
                                }
                                mainReportParam3.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });
                                mainReportParam3.Add(new ReportParameterObject() { ParameterName = "C_RENTAL_INSTALL_TYPE_REMOVE_ALL", Value = RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL });
                                mainReportParam3.Add(new ReportParameterObject() { ParameterName = "C_SALE_INSTALL_TYPE_REMOVE_ALL", Value = SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL });

                                List<RPTInstallCompleteDo> lst090 = new List<RPTInstallCompleteDo>();
                                lst090.Add(((List<RPTInstallCompleteDo>)rptList3)[0]);

                                List<tbm_DocumentTemplate> dLst090 = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_COMPLETE_CONFIRM);
                                if (dLst090.Count > 0)
                                {
                                    lst090[0].DocumentNameEN = dLst090[0].DocumentNameEN;
                                    lst090[0].DocumentVersion = dLst090[0].DocumentVersion;
                                }

                                rptList3 = lst090;
                                strDocumentCode3 = DocumentCode.C_DOCUMENT_CODE_INSTALL_COMPLETE_CONFIRM;
                            }
                            //End Add
                            //}
                            //else if (lst[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                            //{
                            //    //Add report ISR110
                            //    rptList3 = reportHandler.GetRptDeliveryConfirmData(strSlipNo);
                            //    if (rptList3 != null || ((List<RPTDeliveryConfirmDo>)rptList3).Count > 0)
                            //    {
                            //        List<RPTDeliveryConfirmDo> lstSubReport110 = (List<RPTDeliveryConfirmDo>)rptList3;
                            //        listReportParam3.Add(new ReportParameterObject() { ParameterName = "Page1", Value = lstSubReport110 });
                            //        listReportParam3.Add(new ReportParameterObject() { ParameterName = "Page2", Value = lstSubReport110 });

                            //        List<RPTDeliveryConfirmDo> lst110 = new List<RPTDeliveryConfirmDo>();
                            //        lst110.Add(((List<RPTDeliveryConfirmDo>)rptList3)[0]);

                            //        List<tbm_DocumentTemplate> dLst110 = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY);
                            //        if (dLst110.Count > 0)
                            //        {
                            //            lst110[0].DocumentNameEN = dLst110[0].DocumentNameEN;
                            //            lst110[0].DocumentVersion = dLst110[0].DocumentVersion;
                            //        }

                            //        rptList3 = lst110;
                            //        strDocumentCode3 = DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY;
                            //    }
                            //    //End Add
                            //}
                        }
                    }
                    else
                    {
                        ((List<RPTRemoveSlipDo>)rptList).Add(new RPTRemoveSlipDo());
                        mainReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = false });
                    }

                    strDocumentCode1 = DocumentCode.C_DOCUMENT_CODE_REMOVAL_INSTALL_SLIP_B;

                }//======================== ISR040 ==========================
                else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE)
                {
                    rptList = reportHandler.GetRptNewSaleSlipReport(strSlipNo);
                    if (rptList != null && ((List<RPTNewSaleSlipDo>)rptList).Count > 0)
                    {
                        installationData = CommonUtil.CloneObject<RPTNewSaleSlipDo, InstallationReportData>(((List<RPTNewSaleSlipDo>)rptList)[0]);

                        List<RPTNewSaleSlipDo> rptListDetail = (List<RPTNewSaleSlipDo>)rptList;
                        listReportParam.Add(new ReportParameterObject() { SubReportName = "ISR040_1", Value = rptListDetail });
                        listReportParam1.Add(new ReportParameterObject() { SubReportName = "ISR041_1", Value = rptListDetail });
                        listReportParam2.Add(new ReportParameterObject() { SubReportName = "ISR042_1", Value = rptListDetail });

                        bool isShowInstrument = true;
                        if (rptListDetail.Count == 1)
                        {
                            if (rptListDetail[0].InstrumentCode == "-")
                                isShowInstrument = false;
                        }
                        mainReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });

                        //MK-20A
                        List<RPTNewSaleSlipDo> lst = new List<RPTNewSaleSlipDo>();
                        lst.Add(((List<RPTNewSaleSlipDo>)rptList)[0]);

                        dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE);
                        if (dLst.Count > 0)
                        {
                            lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                            lst[0].DocumentVersion = dLst[0].DocumentVersion;
                        }

                        rptList = lst;

                        //MK-20B
                        lst = new List<RPTNewSaleSlipDo>();
                        lst.Add(CommonUtil.CloneObject<RPTNewSaleSlipDo, RPTNewSaleSlipDo>(((List<RPTNewSaleSlipDo>)rptList)[0]));

                        dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE_B);
                        if (dLst.Count > 0)
                        {
                            lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                            lst[0].DocumentVersion = dLst[0].DocumentVersion;
                        }

                        rptListB = lst;

                        //MK-20C
                        lst = new List<RPTNewSaleSlipDo>();
                        lst.Add(CommonUtil.CloneObject<RPTNewSaleSlipDo, RPTNewSaleSlipDo>(((List<RPTNewSaleSlipDo>)rptList)[0]));

                        dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE_C);
                        if (dLst.Count > 0)
                        {
                            lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                            lst[0].DocumentVersion = dLst[0].DocumentVersion;
                        }

                        rptListC = lst;

                        //Add report ISR110
                        //rptList3 = reportHandler.GetRptDeliveryConfirmData(strSlipNo);
                        //if (rptList3 != null || ((List<RPTDeliveryConfirmDo>)rptList3).Count > 0)
                        //{
                        //    List<RPTDeliveryConfirmDo> lstSubReport110 = (List<RPTDeliveryConfirmDo>)rptList3;
                        //    listReportParam3.Add(new ReportParameterObject() { ParameterName = "Page1", Value = lstSubReport110 });
                        //    listReportParam3.Add(new ReportParameterObject() { ParameterName = "Page2", Value = lstSubReport110 });

                        //    List<RPTDeliveryConfirmDo> lst110 = new List<RPTDeliveryConfirmDo>();
                        //    lst110.Add(((List<RPTDeliveryConfirmDo>)rptList3)[0]);

                        //    List<tbm_DocumentTemplate> dLst110 = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY);
                        //    if (dLst110.Count > 0)
                        //    {
                        //        lst110[0].DocumentNameEN = dLst110[0].DocumentNameEN;
                        //        lst110[0].DocumentVersion = dLst110[0].DocumentVersion;
                        //    }

                        //    rptList3 = lst110;
                        //    strDocumentCode3 = DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY;
                        //}

                        rptList3 = reportHandler.GetRptISR110InstallCompleteConfirmData(strSlipNo);
                        if (rptList3 != null && ((List<RptISR110InstallCompleteConfirmDo>)rptList3).Count > 0)
                        {
                            //ISR110_1
                            List<tbm_DocumentTemplate> dLst110 = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY);
                            List<RptSignatureDo> rptSignature1List = reportHandler.GetRptSignatureData(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY, "1");
                            List<RptSignatureDo> rptSignature2List = reportHandler.GetRptSignatureData(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY, "2");
                            foreach (RptISR110InstallCompleteConfirmDo rptDataRow in (List<RptISR110InstallCompleteConfirmDo>)rptList3)
                            {
                                rptDataRow.NameEN1 = rptSignature1List[0].NameEN;
                                rptDataRow.Position1 = rptSignature1List[0].PositionEN;

                                rptDataRow.NameEN2 = rptSignature2List[0].NameEN;
                                rptDataRow.Position2 = rptSignature2List[0].PositionEN;
                            }

                            List<RptISR110InstallCompleteConfirmDo> lst110 = (List<RptISR110InstallCompleteConfirmDo>)rptList3;

                            if (dLst110.Count > 0)
                            {
                                foreach (var row in lst110)
                                {
                                    row.DocumentNameEN = dLst110[0].DocumentNameEN;
                                    row.DocumentVersion = dLst110[0].DocumentVersion;
                                }
                            }

                            isShowInstrument = true;
                            //if (lstSubReport110.Count == 1)
                            //{
                            //    if (lstSubReport110[0].InstrumentName == "-")
                            //        isShowInstrument = false;
                            //}
                            mainReportParam3.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });

                            strDocumentCode3 = DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY;

                            rptList4 = reportHandler.GetRptDeliveryConfirmData(strSlipNo);
                            //ISR111_1
                            rptSignature1List = reportHandler.GetRptSignatureData(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B, "1");
                            foreach (RPTDeliveryConfirmDo rptDataRow in (List<RPTDeliveryConfirmDo>)rptList4)
                            {
                                rptDataRow.NameEN1 = rptSignature1List[0].NameEN;
                                rptDataRow.Position1 = rptSignature1List[0].PositionEN;
                            }
                            List<RPTDeliveryConfirmDo> lst111 = (List<RPTDeliveryConfirmDo>)rptList4;
                            List<RPTDeliveryConfirmDo> lst111_2 = CommonUtil.ClonsObjectList<RPTDeliveryConfirmDo, RPTDeliveryConfirmDo>(lst111);

                            List<tbm_DocumentTemplate> dLst111 = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B);
                            
                            if (dLst111.Count > 0)
                            {
                                foreach (var row in lst111)
                                {
                                    row.DocumentNameEN = dLst111[0].DocumentNameEN;
                                    row.DocumentVersion = dLst111[0].DocumentVersion;
                                    row.TypeRptName = "ORIGINAL CUSTOMER";
                                }
                            }

                            if (lst111_2.Count > 0)
                            {
                                foreach (var row2 in lst111_2)
                                {
                                    row2.DocumentNameEN = dLst111[0].DocumentNameEN;
                                    row2.DocumentVersion = dLst111[0].DocumentVersion;
                                    row2.TypeRptName = "COPY SECOM";
                                }
                            }

                            //listReportParam4.Add(new ReportParameterObject() { SubReportName = "ISR111_1", Value = lstSubReport110 });
                            mainReportParam4.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });

                            rptList4 = lst111;
                            rptList5 = lst111_2;
                            strDocumentCode4 = DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B;
                        }
                        //End Add
                    }
                    else
                    {
                        ((List<RPTNewSaleSlipDo>)rptList).Add(new RPTNewSaleSlipDo());
                        mainReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = false });
                    }

                    strDocumentCode1 = DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE_B;
                    strDocumentCode2 = DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE_C;
                }

                if (installationData == null)
                    installationData = new InstallationReportData();

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strSlipNo;
                doDocument.DocumentCode = strDocumentCode;
                doDocument.DocumentData = rptList;
                doDocument.OtherKey.ContractCode = installationData.ContractCode;
                doDocument.OtherKey.OperationOffice = installationData.OperationOfficeCode;
                doDocument.OtherKey.InstallationSlipIssueOffice = installationData.SlipIssueOfficeCode;
                doDocument.SubReportDataSource = listReportParam;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocument.MainReportParam = mainReportParam;

                if (String.IsNullOrEmpty(strDocumentCode1) == false)
                {
                    doDocument1 = new doDocumentDataGenerate();
                    doDocument1.DocumentNo = strSlipNo;
                    doDocument1.DocumentCode = strDocumentCode1;
                    doDocument1.DocumentData = (rptListB ?? rptList);
                    doDocument1.OtherKey.ContractCode = installationData.ContractCode;
                    doDocument1.OtherKey.OperationOffice = installationData.OperationOfficeCode;
                    doDocument1.OtherKey.InstallationSlipIssueOffice = installationData.SlipIssueOfficeCode;
                    doDocument1.SubReportDataSource = listReportParam1;
                    doDocument1.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doDocument1.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doDocument1.MainReportParam = mainReportParam;
                    listSlaveDoc.Add(doDocument1);
                }

                if (String.IsNullOrEmpty(strDocumentCode2) == false)
                {
                    doDocument2 = new doDocumentDataGenerate();
                    doDocument2.DocumentNo = strSlipNo;
                    doDocument2.DocumentCode = strDocumentCode2;
                    doDocument2.DocumentData = (rptListC ?? rptList);
                    doDocument2.OtherKey.ContractCode = installationData.ContractCode;
                    doDocument2.OtherKey.OperationOffice = installationData.OperationOfficeCode;
                    doDocument2.OtherKey.InstallationSlipIssueOffice = installationData.SlipIssueOfficeCode;
                    doDocument2.SubReportDataSource = listReportParam2;
                    doDocument2.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doDocument2.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doDocument2.MainReportParam = mainReportParam;
                    listSlaveDoc.Add(doDocument2);
                }

                if (String.IsNullOrEmpty(strDocumentCode3) == false)
                {
                    doDocument3 = new doDocumentDataGenerate();
                    doDocument3.DocumentNo = strSlipNo;
                    doDocument3.DocumentCode = strDocumentCode3;
                    doDocument3.DocumentData = rptList3;
                    doDocument3.OtherKey.ContractCode = installationData.ContractCode;
                    doDocument3.OtherKey.OperationOffice = installationData.OperationOfficeCode;
                    doDocument3.OtherKey.InstallationSlipIssueOffice = installationData.SlipIssueOfficeCode;
                    doDocument3.SubReportDataSource = listReportParam3;
                    doDocument3.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doDocument3.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doDocument3.MainReportParam = mainReportParam3;
                    listSlaveDoc.Add(doDocument3);
                }

                if (String.IsNullOrEmpty(strDocumentCode4) == false)
                {
                    doDocument4 = new doDocumentDataGenerate();
                    doDocument4.DocumentNo = strSlipNo;
                    doDocument4.DocumentCode = strDocumentCode4;
                    doDocument4.DocumentData = rptList4;
                    doDocument4.OtherKey.ContractCode = installationData.ContractCode;
                    doDocument4.OtherKey.OperationOffice = installationData.OperationOfficeCode;
                    doDocument4.OtherKey.InstallationSlipIssueOffice = installationData.SlipIssueOfficeCode;
                    doDocument4.SubReportDataSource = listReportParam4;
                    doDocument4.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    doDocument4.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                    doDocument4.MainReportParam = mainReportParam4;
                    listSlaveDoc.Add(doDocument4);

                    if (strDocumentCode4 == DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B)
                    {
                        doDocument4 = new doDocumentDataGenerate();
                        doDocument4.DocumentNo = strSlipNo;
                        doDocument4.DocumentCode = strDocumentCode4;
                        doDocument4.DocumentData = rptList5;
                        doDocument4.OtherKey.ContractCode = installationData.ContractCode;
                        doDocument4.OtherKey.OperationOffice = installationData.OperationOfficeCode;
                        doDocument4.OtherKey.InstallationSlipIssueOffice = installationData.SlipIssueOfficeCode;
                        doDocument4.SubReportDataSource = listReportParam4;
                        doDocument4.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doDocument4.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doDocument4.MainReportParam = mainReportParam4;
                        listSlaveDoc.Add(doDocument4); // Add one more copy
                    }
                }

                if (listSlaveDoc != null && listSlaveDoc.Count > 0)
                    stream = docHandler.GenerateDocument(doDocument, listSlaveDoc);
                else
                    stream = docHandler.GenerateDocument(doDocument);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return stream;
        }

        public Stream CreateReportInstallationPOandSubPrice(string strMaintenanceNo, string strSubcontractorCode, string nameSignature)
        {
            Stream stream = null;
            InstallationReportData installationData = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            List<ReportParameterObject> listReportParam;
            List<tbm_DocumentTemplate> dLst;

            object rptList = null;

            try
            {
                if (strMaintenanceNo == null || strSubcontractorCode == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                listReportParam = new List<ReportParameterObject>();


                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_PO);

                rptList = reportHandler.GetRptPOSubPriceData(strMaintenanceNo, strSubcontractorCode);
                List<RPTPOSubPriceDo> lst = new List<RPTPOSubPriceDo>();
                lst.Add(((List<RPTPOSubPriceDo>)rptList)[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strMaintenanceNo + "-" + strSubcontractorCode;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_INSTALL_PO;
                doDocument.DocumentData = lst;
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ContractCode = lst[0].ContractProjectCode;
                }
                if (lst[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ProjectCode = lst[0].ContractProjectCode;
                    doDocument.OtherKey.OperationOffice = OfficeDummy.C_PROJECT_OFFICE_CODE_DUMMY;
                }
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.OperationOffice = lst[0].OperationOfficeName;
                }
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                doDocument.MainReportParam = new List<ReportParameterObject>();
                doDocument.MainReportParam.Add(new ReportParameterObject() { ParameterName = "DirectorName", Value = nameSignature });

                stream = docHandler.GenerateDocument(doDocument);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return File(stream, "application/pdf");

            return stream;
        }

        public Stream CreateReportInstallationRequestData(string strMaintenanceNo)
        {
            Stream stream = null;
            InstallationReportData installationData = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            List<ReportParameterObject> listReportParam;
            List<tbm_DocumentTemplate> dLst;

            object rptList = null;

            try
            {
                if (strMaintenanceNo == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                listReportParam = new List<ReportParameterObject>();
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeRental", Value = ServiceType.C_SERVICE_TYPE_RENTAL });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeSale", Value = ServiceType.C_SERVICE_TYPE_SALE });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeProject", Value = ServiceType.C_SERVICE_TYPE_PROJECT });

                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_REQUEST);

                rptList = reportHandler.GetRptInstallationRequestData(strMaintenanceNo);
                List<RptSignatureDo> rptSignature1List = reportHandler.GetRptSignatureData("ISR060", "1");
                List<RptSignatureDo> rptSignature2List = reportHandler.GetRptSignatureData("ISR060", "2");
                List<RptSignatureDo> rptSignature3List = reportHandler.GetRptSignatureData("ISR060", "3");
                List<RptSignatureDo> rptSignature4List = reportHandler.GetRptSignatureData("ISR060", "4");
                List<RptSignatureDo> rptSignature5List = reportHandler.GetRptSignatureData("ISR060", "5");

                foreach (RPTInstallRequestDo rptDataRow in (List< RPTInstallRequestDo>)rptList)
                {
                    rptDataRow.NameEN1 = rptSignature1List[0].NameEN;
                    rptDataRow.Position1 = rptSignature1List[0].PositionEN;

                    rptDataRow.NameEN2 = rptSignature2List[0].NameEN;
                    rptDataRow.Position2 = rptSignature2List[0].PositionEN;

                    rptDataRow.NameEN3 = rptSignature3List[0].NameEN;
                    rptDataRow.Position3 = rptSignature3List[0].PositionEN;

                    rptDataRow.NameEN4 = rptSignature4List[0].NameEN;
                    rptDataRow.Position4 = rptSignature4List[0].PositionEN;

                    rptDataRow.NameEN5 = rptSignature5List[0].NameEN;
                    rptDataRow.Position5 = rptSignature5List[0].PositionEN;
                }

                List<RPTInstallRequestDo> lst = new List<RPTInstallRequestDo>();
                lst.Add(((List<RPTInstallRequestDo>)rptList)[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strMaintenanceNo;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_INSTALL_REQUEST;
                doDocument.DocumentData = lst;
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ContractCode = lst[0].ContractProjectCode;
                }
                if (lst[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ProjectCode = lst[0].ContractProjectCode;
                    doDocument.OtherKey.OperationOffice = OfficeDummy.C_PROJECT_OFFICE_CODE_DUMMY;
                }
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.OperationOffice = lst[0].OperationOfficeCode;
                }
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocument.MainReportParam = listReportParam;

                stream = docHandler.GenerateDocument(doDocument);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return File(stream, "application/pdf");

            return stream;
        }

        public string CreateReportInstallationRequestFilePath(string strMaintenanceNo)
        {
            Stream stream = null;
            InstallationReportData installationData = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            List<ReportParameterObject> listReportParam;
            List<tbm_DocumentTemplate> dLst;
            string OutputPath = "";
            object rptList = null;

            try
            {
                if (strMaintenanceNo == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                listReportParam = new List<ReportParameterObject>();
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeRental", Value = ServiceType.C_SERVICE_TYPE_RENTAL });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeSale", Value = ServiceType.C_SERVICE_TYPE_SALE });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeProject", Value = ServiceType.C_SERVICE_TYPE_PROJECT });

                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_REQUEST);

                rptList = reportHandler.GetRptInstallationRequestData(strMaintenanceNo);
                List<RptSignatureDo> rptSignature1List = reportHandler.GetRptSignatureData("ISR060", "1");
                List<RptSignatureDo> rptSignature2List = reportHandler.GetRptSignatureData("ISR060", "2");
                List<RptSignatureDo> rptSignature3List = reportHandler.GetRptSignatureData("ISR060", "3");
                List<RptSignatureDo> rptSignature4List = reportHandler.GetRptSignatureData("ISR060", "4");
                List<RptSignatureDo> rptSignature5List = reportHandler.GetRptSignatureData("ISR060", "5");

                foreach (RPTInstallRequestDo rptDataRow in (List<RPTInstallRequestDo>)rptList)
                {
                    rptDataRow.NameEN1 = rptSignature1List[0].NameEN;
                    rptDataRow.Position1 = rptSignature1List[0].PositionEN;

                    rptDataRow.NameEN2 = rptSignature2List[0].NameEN;
                    rptDataRow.Position2 = rptSignature2List[0].PositionEN;

                    rptDataRow.NameEN3 = rptSignature3List[0].NameEN;
                    rptDataRow.Position3 = rptSignature3List[0].PositionEN;

                    rptDataRow.NameEN4 = rptSignature4List[0].NameEN;
                    rptDataRow.Position4 = rptSignature4List[0].PositionEN;

                    rptDataRow.NameEN5 = rptSignature5List[0].NameEN;
                    rptDataRow.Position5 = rptSignature5List[0].PositionEN;
                }

                List<RPTInstallRequestDo> lst = new List<RPTInstallRequestDo>();
                lst.Add(((List<RPTInstallRequestDo>)rptList)[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strMaintenanceNo;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_INSTALL_REQUEST;
                doDocument.DocumentData = lst;
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ContractCode = lst[0].ContractProjectCode;
                }
                if (lst[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ProjectCode = lst[0].ContractProjectCode;
                    doDocument.OtherKey.OperationOffice = OfficeDummy.C_PROJECT_OFFICE_CODE_DUMMY;
                }
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.OperationOffice = lst[0].OperationOfficeCode;
                }
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocument.MainReportParam = listReportParam;

                OutputPath = docHandler.GenerateDocumentFilePath(doDocument);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return File(stream, "application/pdf");

            return OutputPath;
        }

        public Stream CreateReportInstallSpecCompleteData(string strMaintenanceNo, string strSubcontractorCode)
        {
            Stream stream = null;
            InstallationReportData installationData = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            List<ReportParameterObject> listReportParam;
            List<tbm_DocumentTemplate> dLst;

            object rptList = null;

            try
            {
                if (strMaintenanceNo == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                listReportParam = new List<ReportParameterObject>();
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeRental", Value = ServiceType.C_SERVICE_TYPE_RENTAL });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeSale", Value = ServiceType.C_SERVICE_TYPE_SALE });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeProject", Value = ServiceType.C_SERVICE_TYPE_PROJECT });

                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_SPEC_AND_COMPLETE);

                rptList = reportHandler.GetRptInstallSpecCompleteData(strMaintenanceNo, strSubcontractorCode);
                if (rptList == null || ((List<RPTInstallSpecCompleteDo>)rptList).Count == 0)
                    return null;

                List<RPTInstallSpecCompleteDo> lst = new List<RPTInstallSpecCompleteDo>();
                lst.Add(((List<RPTInstallSpecCompleteDo>)rptList)[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strMaintenanceNo + "-" + strSubcontractorCode;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_INSTALL_SPEC_AND_COMPLETE;
                doDocument.DocumentData = lst;
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ContractCode = lst[0].ContractProjectCode;
                }
                if (lst[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ProjectCode = lst[0].ContractProjectCode;
                    doDocument.OtherKey.OperationOffice = OfficeDummy.C_PROJECT_OFFICE_CODE_DUMMY;
                }
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.OperationOffice = lst[0].OperationOfficeCode;
                }
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocument.MainReportParam = listReportParam;

                stream = docHandler.GenerateDocument(doDocument);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return File(stream, "application/pdf");

            return stream;
        }

        public Stream CreateReportIECheckSheetData(string strMaintenanceNo, string strSubcontractorCode)
        {
            //strMaintenanceNo = "5020N20110030";
            //strSubcontractorCode = "00002";
            Stream stream = null;
            InstallationReportData installationData = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            List<ReportParameterObject> listReportParam;
            List<tbm_DocumentTemplate> dLst;

            object rptList = null;

            try
            {
                if (strMaintenanceNo == null || strSubcontractorCode == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                listReportParam = new List<ReportParameterObject>();
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeRental", Value = ServiceType.C_SERVICE_TYPE_RENTAL });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeSale", Value = ServiceType.C_SERVICE_TYPE_SALE });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeProject", Value = ServiceType.C_SERVICE_TYPE_PROJECT });

                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_IE_CHECK_SHEET);

                rptList = reportHandler.GetRptIECheckSheetData(strMaintenanceNo, strSubcontractorCode);
                if (rptList == null || ((List<RPTIECheckSheetDo>)rptList).Count == 0)
                    return null;

                List<RPTIECheckSheetDo> lst = new List<RPTIECheckSheetDo>();
                lst.Add(((List<RPTIECheckSheetDo>)rptList)[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strMaintenanceNo + "-" + strSubcontractorCode;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_IE_CHECK_SHEET;
                doDocument.DocumentData = lst;
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ContractCode = lst[0].ContractProjectCode;
                }
                if (lst[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ProjectCode = lst[0].ContractProjectCode;
                    doDocument.OtherKey.OperationOffice = OfficeDummy.C_PROJECT_OFFICE_CODE_DUMMY;
                }
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.OperationOffice = lst[0].OperationOfficeCode;
                }
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocument.MainReportParam = listReportParam;

                stream = docHandler.GenerateDocument(doDocument);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return File(stream, "application/pdf");

            return stream;
        }

        public Stream CreateReportISS050MergedAll(string strMaintenanceNo, string strSubcontractorCode,string nameSignature)
        {
            Stream stream = null;
            InstallationReportData installationData = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            List<ReportParameterObject> listReportParam;
            List<tbm_DocumentTemplate> dLst;
            string docPath = "";
            List<string> lstFilePath = new List<string>();

            object rptList = null;
            Stream streamFile = null;
            try
            {
                List<doDocumentDataGenerate> doCover = new List<doDocumentDataGenerate>();
                //====================================== ISR050 =========================================
                if (strMaintenanceNo == null || strSubcontractorCode == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                listReportParam = new List<ReportParameterObject>();


                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_PO);

                rptList = reportHandler.GetRptPOSubPriceData(strMaintenanceNo, strSubcontractorCode);
                List<RPTPOSubPriceDo> lst = new List<RPTPOSubPriceDo>();
                lst.Add(((List<RPTPOSubPriceDo>)rptList)[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strMaintenanceNo + "-" + strSubcontractorCode;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_INSTALL_PO;
                doDocument.DocumentData = lst;
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ContractCode = lst[0].ContractProjectCode;
                }
                if (lst[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ProjectCode = lst[0].ContractProjectCode;
                    doDocument.OtherKey.OperationOffice = OfficeDummy.C_PROJECT_OFFICE_CODE_DUMMY;
                }
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.OperationOffice = lst[0].OperationOfficeCode;
                }
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                doDocument.MainReportParam = new List<ReportParameterObject>();
                doDocument.MainReportParam.Add(new ReportParameterObject() { ParameterName = "DirectorName", Value = nameSignature });

                docPath = docHandler.GenerateDocumentWithoutEncrypt(doDocument, doCover);
                lstFilePath.Add(docPath);

                //================================== ISR070 =======================================================
                rptList = null;
                listReportParam = new List<ReportParameterObject>();
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeRental", Value = ServiceType.C_SERVICE_TYPE_RENTAL });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeSale", Value = ServiceType.C_SERVICE_TYPE_SALE });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeProject", Value = ServiceType.C_SERVICE_TYPE_PROJECT });

                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_SPEC_AND_COMPLETE);

                rptList = reportHandler.GetRptInstallSpecCompleteData(strMaintenanceNo, strSubcontractorCode);

                List<RPTInstallSpecCompleteDo> lst2 = new List<RPTInstallSpecCompleteDo>();
                lst2.Add(((List<RPTInstallSpecCompleteDo>)rptList)[0]);

                if (dLst.Count > 0)
                {
                    lst2[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst2[0].DocumentVersion = dLst[0].DocumentVersion;
                }

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strMaintenanceNo + "-" + strSubcontractorCode;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_INSTALL_SPEC_AND_COMPLETE;
                doDocument.DocumentData = lst2;
                if (lst2[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ContractCode = lst2[0].ContractProjectCode;
                }
                if (lst2[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ProjectCode = lst2[0].ContractProjectCode;
                    doDocument.OtherKey.OperationOffice = OfficeDummy.C_PROJECT_OFFICE_CODE_DUMMY;
                }
                if (lst2[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.OperationOffice = lst2[0].OperationOfficeCode;
                }
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocument.MainReportParam = listReportParam;

                docPath = docHandler.GenerateDocumentWithoutEncrypt(doDocument, doCover);
                lstFilePath.Add(docPath);
                //================================== ISR080 =======================================================
                rptList = null;
                if (strMaintenanceNo == null || strSubcontractorCode == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                listReportParam = new List<ReportParameterObject>();
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeRental", Value = ServiceType.C_SERVICE_TYPE_RENTAL });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeSale", Value = ServiceType.C_SERVICE_TYPE_SALE });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeProject", Value = ServiceType.C_SERVICE_TYPE_PROJECT });

                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_IE_CHECK_SHEET);

                rptList = reportHandler.GetRptIECheckSheetData(strMaintenanceNo, strSubcontractorCode);
                if (rptList == null || ((List<RPTIECheckSheetDo>)rptList).Count == 0)
                    return null;

                List<RPTIECheckSheetDo> lst3 = new List<RPTIECheckSheetDo>();
                lst3.Add(((List<RPTIECheckSheetDo>)rptList)[0]);

                if (dLst.Count > 0)
                {
                    lst3[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst3[0].DocumentVersion = dLst[0].DocumentVersion;
                }

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strMaintenanceNo + "-" + strSubcontractorCode;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_IE_CHECK_SHEET;
                doDocument.DocumentData = lst3;
                if (lst3[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ContractCode = lst3[0].ContractProjectCode;
                }
                if (lst3[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ProjectCode = lst3[0].ContractProjectCode;
                    doDocument.OtherKey.OperationOffice = OfficeDummy.C_PROJECT_OFFICE_CODE_DUMMY;
                }
                if (lst3[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.OperationOffice = lst3[0].OperationOfficeCode;
                }
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocument.MainReportParam = listReportParam;

                docPath = docHandler.GenerateDocumentWithoutEncrypt(doDocument, doCover);
                lstFilePath.Add(docPath);
                //============================ Merge PDF =====================================================
                if (lstFilePath.Count > 0)
                {
                    string mergeOutputFilename = PathUtil.GetTempFileName(".pdf");
                    string encryptOutputFileName = PathUtil.GetTempFileName(".pdf");

                    //for (int i = 0; i < 1000; i++)
                    //{
                    //    lstFilePath.Add(lstFilePath[0]);
                    //}

                    bool isSuccess = ReportUtil.MergePDF(lstFilePath.ToArray(), mergeOutputFilename, true, encryptOutputFileName, null);

                    if (isSuccess)
                    {
                        //FileStream streamFile = new FileStream(encryptOutputFileName, FileMode.Open, FileAccess.Read);
                        //using (MemoryStream ms = new MemoryStream())
                        //{
                        //    streamFile.CopyTo(ms);
                        //    stream = ms;
                        //}
                        streamFile = new FileStream(mergeOutputFilename, FileMode.Open, FileAccess.Read);
                    }
                }
                //============================================================================================
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return streamFile;


        }

        public Stream CreateReportInstallCompleteConfirmData(string strSlipNo)
        {
            Stream stream = null;
            InstallationReportData installationData = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            List<ReportParameterObject> listReportParam;
            List<tbm_DocumentTemplate> dLst;
            List<ReportParameterObject> listSubReportDataSource;

            object rptList = null;

            try
            {
                if (strSlipNo == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                listReportParam = new List<ReportParameterObject>();

                listReportParam.Add(new ReportParameterObject() { ParameterName = "C_RENTAL_INSTALL_TYPE_REMOVE_ALL", Value = RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "C_SALE_INSTALL_TYPE_REMOVE_ALL", Value = SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL });
                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_INSTALL_COMPLETE_CONFIRM);

                rptList = reportHandler.GetRptInstallCompleteConfirmData(strSlipNo);
                List<RptSignatureDo> rptSignature1List = reportHandler.GetRptSignatureData("ISR090", "1");
                List<RptSignatureDo> rptSignature2List = reportHandler.GetRptSignatureData("ISR090", "2");
                foreach (RPTInstallCompleteDo rptDataRow in (List<RPTInstallCompleteDo>)rptList)
                {
                    rptDataRow.NameEN1 = rptSignature1List[0].NameEN;
                    rptDataRow.Position1 = rptSignature1List[0].PositionEN;

                    rptDataRow.NameEN2 = rptSignature2List[0].NameEN;
                    rptDataRow.Position2 = rptSignature2List[0].PositionEN;
                }

                if (rptList == null || ((List<RPTInstallCompleteDo>)rptList).Count == 0)
                    return null;

                List<RPTInstallCompleteDo> lst = new List<RPTInstallCompleteDo>();
                lst.Add(((List<RPTInstallCompleteDo>)rptList)[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }

                List<RPTInstallCompleteDo> lstSubReport = (List<RPTInstallCompleteDo>)rptList;
                listSubReportDataSource = new List<ReportParameterObject>();
                listSubReportDataSource.Add(new ReportParameterObject() { SubReportName = "ISR090_1", Value = lstSubReport });

                bool isShowInstrument = true;
                if (lstSubReport.Count == 1)
                {
                    if (lstSubReport[0].InstrumentCode == "-")
                        isShowInstrument = false;
                }
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strSlipNo;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_INSTALL_COMPLETE_CONFIRM;
                doDocument.DocumentData = lst;
                doDocument.OtherKey.ContractCode = lst[0].ContractCode;
                doDocument.OtherKey.OperationOffice = lst[0].OperationOfficeCode;
                doDocument.MainReportParam = listReportParam;
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocument.SubReportDataSource = listSubReportDataSource;

                stream = docHandler.GenerateDocument(doDocument);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return File(stream, "application/pdf");

            return stream;
        }

        public Stream CreateReportAcceptanceInspectionNotice(string strMaintenanceNo, string strSubcontractorCode)
        {
            //strMaintenanceNo = "5020N20110030";
            //strSubcontractorCode = "00002";
            Stream stream = null;
            InstallationReportData installationData = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            List<ReportParameterObject> listReportParam;
            List<tbm_DocumentTemplate> dLst;

            object rptList = null;

            try
            {
                if (strMaintenanceNo == null || strSubcontractorCode == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                listReportParam = new List<ReportParameterObject>();
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeRental", Value = ServiceType.C_SERVICE_TYPE_RENTAL });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeSale", Value = ServiceType.C_SERVICE_TYPE_SALE });
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ServiceTypeProject", Value = ServiceType.C_SERVICE_TYPE_PROJECT });

                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_ACCEPT_INSPECT_NOTICE);

                rptList = reportHandler.GetRptAcceptInspecNocticeData(strMaintenanceNo, strSubcontractorCode);
                if (rptList == null || ((List<RPTAcceptInspecDo>)rptList).Count == 0)
                    return null;

                List<RPTAcceptInspecDo> lst = new List<RPTAcceptInspecDo>();
                lst.Add(((List<RPTAcceptInspecDo>)rptList)[0]);

                if (dLst.Count > 0)
                {
                    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                }

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strMaintenanceNo + "-" + strSubcontractorCode;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_ACCEPT_INSPECT_NOTICE;
                doDocument.DocumentData = lst;
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ContractCode = lst[0].ContractProjectCode;
                }
                if (lst[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.ProjectCode = lst[0].ContractProjectCode;
                    doDocument.OtherKey.OperationOffice = OfficeDummy.C_PROJECT_OFFICE_CODE_DUMMY;
                }
                if (lst[0].ServiceTypeCode != ServiceType.C_SERVICE_TYPE_PROJECT)
                {
                    doDocument.OtherKey.OperationOffice = lst[0].OperationOfficeCode;
                }
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                doDocument.MainReportParam = listReportParam;

                stream = docHandler.GenerateDocument(doDocument);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return File(stream, "application/pdf");

            return stream;
        }

        //ISS060 DownloadReport ISR110 and ISR111
        public Stream ISS060CreateReportISR11_ISR111(string strSlipNo)
        {
            Stream stream = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            doDocumentDataGenerate doDocument1;
            doDocumentDataGenerate doDocument2;
            List<ReportParameterObject> listReportParam;
            List<tbm_DocumentTemplate> dLst110;
            List<doDocumentDataGenerate> listSlaveDoc;

            object rptList = null;
            object rptList2 = null;

            try
            {
                if (strSlipNo == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                //ISR110
                rptList = reportHandler.GetRptISR110InstallCompleteConfirmData(strSlipNo);
                dLst110 = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY);
                List<RptSignatureDo> rptSignature1List = reportHandler.GetRptSignatureData(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY, "1");
                List<RptSignatureDo> rptSignature2List = reportHandler.GetRptSignatureData(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY, "2");
                foreach (RptISR110InstallCompleteConfirmDo rptDataRow in (List<RptISR110InstallCompleteConfirmDo>)rptList)
                {
                    rptDataRow.NameEN1 = rptSignature1List[0].NameEN;
                    rptDataRow.Position1 = rptSignature1List[0].PositionEN;

                    rptDataRow.NameEN2 = rptSignature2List[0].NameEN;
                    rptDataRow.Position2 = rptSignature2List[0].PositionEN;
                }

                if (rptList == null || ((List<RptISR110InstallCompleteConfirmDo>)rptList).Count == 0)
                    return null;

                List<RptISR110InstallCompleteConfirmDo> lst110 = (List<RptISR110InstallCompleteConfirmDo>)rptList;

                if (dLst110.Count > 0)
                {
                    foreach (var row in lst110)
                    {
                        row.DocumentNameEN = dLst110[0].DocumentNameEN;
                        row.DocumentVersion = dLst110[0].DocumentVersion;
                    }
                }

                //ISR111
                rptList2 = reportHandler.GetRptDeliveryConfirmData(strSlipNo);
                rptSignature1List = reportHandler.GetRptSignatureData(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B, "1");
                foreach (RPTDeliveryConfirmDo rptDataRow in (List<RPTDeliveryConfirmDo>)rptList2)
                {
                    rptDataRow.NameEN1 = rptSignature1List[0].NameEN;
                    rptDataRow.Position1 = rptSignature1List[0].PositionEN;
                }

                if (rptList2 == null || ((List<RPTDeliveryConfirmDo>)rptList2).Count == 0)
                    return null;

                List<RPTDeliveryConfirmDo> lst111_1 = (List<RPTDeliveryConfirmDo>)rptList2;

                List<tbm_DocumentTemplate> dLst111_1 = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B);
                if (dLst111_1.Count > 0)
                {
                    foreach (var row in lst111_1)
                    {
                        row.DocumentNameEN = dLst111_1[0].DocumentNameEN;
                        row.DocumentVersion = dLst111_1[0].DocumentVersion;
                        row.TypeRptName = "ORIGINAL CUSTOMER";
                    }
                }

                List<RPTDeliveryConfirmDo> lst111_2 = CommonUtil.ClonsObjectList<RPTDeliveryConfirmDo, RPTDeliveryConfirmDo>(lst111_1);

                List<tbm_DocumentTemplate> dLst111_2 = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B);
                if (dLst111_2.Count > 0)
                {
                    foreach (var row in lst111_2)
                    {
                        row.DocumentNameEN = dLst111_2[0].DocumentNameEN;
                        row.DocumentVersion = dLst111_2[0].DocumentVersion;
                        row.TypeRptName = "COPY SECOM";
                    }
                }

                bool isShowInstrument = true;
                if (lst110.Count == 1)
                {
                    if (lst110[0].SecomInstrumentCode == "-")
                        isShowInstrument = false;
                }

                listReportParam = new List<ReportParameterObject>();
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strSlipNo;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY;
                doDocument.DocumentData = lst110;
                doDocument.OtherKey.ContractCode = lst110[0].ContractCode;
                doDocument.OtherKey.OperationOffice = lst110[0].OperationOfficeCode;
                doDocument.MainReportParam = listReportParam;
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                isShowInstrument = true;
                if (lst111_1.Count == 1)
                {
                    if (lst111_1[0].InstrumentCode == "-")
                        isShowInstrument = false;
                }

                listReportParam = new List<ReportParameterObject>();
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });

                doDocument1 = new doDocumentDataGenerate();
                doDocument1.DocumentNo = strSlipNo;
                doDocument1.DocumentCode = DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B;
                doDocument1.DocumentData = lst111_1;
                doDocument1.OtherKey.ContractCode = lst111_1[0].ContractCode;
                doDocument1.OtherKey.OperationOffice = lst111_1[0].OperationOfficeCode;
                doDocument1.MainReportParam = listReportParam;
                doDocument1.OtherKey.InstallationSlipIssueOffice = null;
                doDocument1.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument1.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                listSlaveDoc = new List<doDocumentDataGenerate>();
                listSlaveDoc.Add(doDocument1);

                doDocument2 = new doDocumentDataGenerate();
                doDocument2.DocumentNo = strSlipNo;
                doDocument2.DocumentCode = DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B;
                doDocument2.DocumentData = lst111_2;
                doDocument2.OtherKey.ContractCode = lst111_2[0].ContractCode;
                doDocument2.OtherKey.OperationOffice = lst111_2[0].OperationOfficeCode;
                doDocument2.MainReportParam = listReportParam;
                doDocument2.OtherKey.InstallationSlipIssueOffice = null;
                doDocument2.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument2.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                listSlaveDoc.Add(doDocument2);

                stream = docHandler.GenerateDocument(doDocument, listSlaveDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return File(stream, "application/pdf");

            return stream;
        }

        public Stream CreateReportInstallationCompleteConfirmation(string strSlipNo)
        {
            Stream stream = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            List<ReportParameterObject> listReportParam;
            List<tbm_DocumentTemplate> dLst;

            object rptList = null;

            try
            {
                if (strSlipNo == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY);
                rptList = reportHandler.GetRptISR110InstallCompleteConfirmData(strSlipNo);
                List<RptSignatureDo> rptSignature1List = reportHandler.GetRptSignatureData(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY, "1");
                List<RptSignatureDo> rptSignature2List = reportHandler.GetRptSignatureData(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY, "2");
                foreach (RptISR110InstallCompleteConfirmDo rptDataRow in (List<RptISR110InstallCompleteConfirmDo>)rptList)
                {
                    rptDataRow.NameEN1 = rptSignature1List[0].NameEN;
                    rptDataRow.Position1 = rptSignature1List[0].PositionEN;

                    rptDataRow.NameEN2 = rptSignature2List[0].NameEN;
                    rptDataRow.Position2 = rptSignature2List[0].PositionEN;
                }

                if (rptList == null || ((List<RptISR110InstallCompleteConfirmDo>)rptList).Count == 0)
                    return null;

                List<RptISR110InstallCompleteConfirmDo> lst = (List<RptISR110InstallCompleteConfirmDo>)rptList;

                if (dLst.Count > 0)
                {
                    foreach (var row in lst)
                    {
                        row.DocumentNameEN = dLst[0].DocumentNameEN;
                        row.DocumentVersion = dLst[0].DocumentVersion;
                    }
                }

                bool isShowInstrument = true;
                if (lst.Count == 1)
                {
                    if (lst[0].InstrumentCode == "-")
                        isShowInstrument = false;
                }
                listReportParam = new List<ReportParameterObject>();
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strSlipNo;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY;
                doDocument.DocumentData = lst;
                doDocument.OtherKey.ContractCode = lst[0].ContractCode;
                doDocument.OtherKey.OperationOffice = lst[0].OperationOfficeCode;
                doDocument.MainReportParam = listReportParam;
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                stream = docHandler.GenerateDocumentISR110(doDocument);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return File(stream, "application/pdf");

            return stream;
        }

        public Stream CreateReportDeliveryConfirmData(string strSlipNo)
        {
            Stream stream = null;

            IReportHandler reportHandler;
            IDocumentHandler docHandler;
            doDocumentDataGenerate doDocument;
            doDocumentDataGenerate doDocument1;
            List<ReportParameterObject> listReportParam;
            List<tbm_DocumentTemplate> dLst;
            List<doDocumentDataGenerate> listSlaveDoc;

            object rptList = null;

            try
            {
                if (strSlipNo == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);

                reportHandler = ServiceContainer.GetService<IReportHandler>() as IReportHandler;
                docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

                //dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY);
                //listReportParam.Add(new ReportParameterObject() { ParameterName = "Page1", Value = dLst });
                //listReportParam.Add(new ReportParameterObject() { ParameterName = "Page2", Value = dLst });
                //rptList = reportHandler.GetRptDeliveryConfirmData(strSlipNo);
                //List<RPTDeliveryConfirmDo> lst = new List<RPTDeliveryConfirmDo>();
                //lst.Add(((List<RPTDeliveryConfirmDo>)rptList)[0]);

                //if (dLst.Count > 0)
                //{
                //    lst[0].DocumentNameEN = dLst[0].DocumentNameEN;
                //    lst[0].DocumentVersion = dLst[0].DocumentVersion;
                //}
                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B);
                rptList = reportHandler.GetRptDeliveryConfirmData(strSlipNo);
                List<RptSignatureDo> rptSignature1List = reportHandler.GetRptSignatureData(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B, "1");
                //List<RptSignatureDo> rptSignature2List = reportHandler.GetRptSignatureData(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY, "2");
                foreach (RPTDeliveryConfirmDo rptDataRow in (List<RPTDeliveryConfirmDo>)rptList)
                {
                    rptDataRow.NameEN1 = rptSignature1List[0].NameEN;
                    rptDataRow.Position1 = rptSignature1List[0].PositionEN;
                }

                if (rptList == null || ((List<RPTDeliveryConfirmDo>)rptList).Count == 0)
                    return null;

                List<RPTDeliveryConfirmDo> lst = (List<RPTDeliveryConfirmDo>)rptList;
                List<RPTDeliveryConfirmDo> lstB = CommonUtil.ClonsObjectList<RPTDeliveryConfirmDo, RPTDeliveryConfirmDo>(lst);

                if (dLst.Count > 0)
                {
                    foreach (var row in lst)
                    {
                        row.DocumentNameEN = dLst[0].DocumentNameEN;
                        row.DocumentVersion = dLst[0].DocumentVersion;
                        row.TypeRptName = "ORIGINAL CUSTOMER";
                    }
                }

                bool isShowInstrument = true;
                if (lst.Count == 1)
                {
                    if (lst[0].InstrumentName == "-")
                        isShowInstrument = false;
                }
                listReportParam = new List<ReportParameterObject>();
                listReportParam.Add(new ReportParameterObject() { ParameterName = "ShowInstrumentFlag", Value = isShowInstrument });

                doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = strSlipNo;
                doDocument.DocumentCode = DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B;
                doDocument.DocumentData = lst;
                doDocument.OtherKey.ContractCode = lst[0].ContractCode;
                doDocument.OtherKey.OperationOffice = lst[0].OperationOfficeCode;
                doDocument.MainReportParam = listReportParam;
                doDocument.OtherKey.InstallationSlipIssueOffice = null;
                doDocument.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                dLst = docHandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B);

                if (dLst.Count > 0)
                {
                    foreach (var row in lstB)
                    {
                        row.DocumentNameEN = dLst[0].DocumentNameEN;
                        row.DocumentVersion = dLst[0].DocumentVersion;
                        row.TypeRptName = "COPY SECOM";
                    }
                }

                doDocument1 = new doDocumentDataGenerate();
                doDocument1.DocumentNo = strSlipNo;
                doDocument1.DocumentCode = DocumentCode.C_DOCUMENT_CODE_DELIVERY_CONFIRMATION_AND_WARRANTY_B;
                doDocument1.DocumentData = lstB;
                doDocument1.OtherKey.ContractCode = lst[0].ContractCode;
                doDocument1.OtherKey.OperationOffice = lst[0].OperationOfficeCode;
                doDocument1.MainReportParam = listReportParam;
                doDocument1.OtherKey.InstallationSlipIssueOffice = null;
                doDocument1.ProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDocument1.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                listSlaveDoc = new List<doDocumentDataGenerate>();
                listSlaveDoc.Add(doDocument1);

                //stream = docHandler.GenerateDocument(doDocument);
                stream = docHandler.GenerateDocument(doDocument, listSlaveDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return File(stream, "application/pdf");

            return stream;
        }

        public string GenerateISR120Report(List<dtGetInstallationReport> data, doInstallationReport paramSearch)
        {
            const string TEMPLATE_NAME = "ISR120.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER_DATE = 3;
            const int COL_HEADER_DATE = 2;

            const int ROW_START_DATA = 6;

            int rowindex = ROW_START_DATA;
            int columncount = 0;
            int COL_TEAMNO = ++columncount;
            int COL_CONTRACTCODE = ++columncount;
            int COL_INSTALLTIONTYPE = ++columncount;
            int COL_SITENAME = ++columncount;
            int COL_MAINTENANCENO = ++columncount;
            int COL_LASTPAYMENT = ++columncount;
            int COL_LASTPAYMENTUSD = ++columncount;
            int COL_IMFEE = ++columncount;
            int COL_IMREMARK = ++columncount;
            int COL_ADV_PAIDDATE = ++columncount;
            int COL_ADV_PAYMENTTYPE = ++columncount;
            int COL_ADV_APPROVENO = ++columncount;
            int COL_ADV_AMOUNT = ++columncount;
            int COL_ADV_AMOUNTUSD = ++columncount;
            int COL_PAIDDATE = ++columncount;
            int COL_PAYMENTTYPE = ++columncount;
            int COL_APPROVENO = ++columncount;
            int COL_AMOUNT = ++columncount;
            int COL_AMOUNTUSD = ++columncount;
            int COL_SLIPSTATUS = ++columncount;

            int COL_BILLINGINSFEE = ++columncount;
            int COL_PAIDFLAG = ++columncount;

            int COL_TOTAL = COL_SITENAME;
            int COL_TOTAL_VALUE = COL_AMOUNT;
            int COL_TOTAL_VALUE_USD = COL_AMOUNTUSD;

            int COL_MIN = COL_TEAMNO;
            int COL_MAX = COL_TOTAL_VALUE;

            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");

            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                doc.SetCellValue(ROW_HEADER_DATE, COL_HEADER_DATE, string.Format("{0} - {1}",
                    paramSearch.PaidDateFrom.Value.ToString("dd-MMM-yyyy"),
                    paramSearch.PaidDateTo.Value.ToString("dd-MMM-yyyy")
                ));

                var qSubContract = (
                    from d in data
                    group d by d.SubContractorNameEN into subcontractor
                    orderby subcontractor.Key
                    select new
                    {
                        SubContractorNameEN = subcontractor.Key,
                        Contracts = (
                            from sub in subcontractor
                            group sub by new { sub.ContractProjectCode, sub.MaintenanceNo } into grpContract
                            orderby grpContract.Key.ContractProjectCode, grpContract.Key.MaintenanceNo
                            select new
                            {
                                Info = grpContract.FirstOrDefault(),
                                AdvPayments = (from advp in grpContract where advp.IsMatchPaidDate == 0 select advp),
                                Payments = (from p in grpContract where p.IsMatchPaidDate == 1 select p),
                            }
                            //orderby contract.ContractProjectCode, contract.MaintenanceNo, contract.IsMatchPaidDate, contract.PaymentIndex
                            //select contract
                        )
                    }
                );

                decimal? totalvalue = 0;
                decimal? totalvalueUsd = 0;
                foreach (var subcontractor in qSubContract)
                {
                    rowindex++;
                    SLStyle styBorderDotted = new SLStyle();
                    styBorderDotted.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Dotted;

                    SLStyle style = new SLStyle();
                    style.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;

                    SLStyle styleRigth = new SLStyle();
                    style.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right;

                    doc.CopyCellFromWorksheet(WSNAME_DETAIL, rowindex, COL_MIN, rowindex, COL_MAX, rowindex, 1);
                    doc.SetRowHeight(rowindex, docTemp.GetRowHeight(rowindex));

                    doc.SetCellStyle(rowindex, COL_TEAMNO, rowindex, COL_PAIDFLAG, styBorderDotted);

                    doc.SetCellValue(rowindex, COL_TEAMNO, subcontractor.SubContractorNameEN);
                    rowindex++;

                    int rowno = 1;
                    string checksite = string.Empty;

                    int firstrowtotal = rowindex;
                    int lastrowtotal = 0;
                    decimal? tmpTotal = 0;
                    decimal? tmpTotalUsd = 0;
                    foreach (var contract in subcontractor.Contracts)
                    {
                        doc.SetCellValue(rowindex, COL_TEAMNO, rowno);
                        doc.SetCellValue(rowindex, COL_CONTRACTCODE, commonUtil.ConvertContractCode(contract.Info.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                        doc.SetCellValue(rowindex, COL_INSTALLTIONTYPE, contract.Info.InstallationType);
                        doc.SetCellValue(rowindex, COL_SITENAME, contract.Info.SiteNameEN);
                        doc.SetCellValue(rowindex, COL_MAINTENANCENO, contract.Info.MaintenanceNo);

                        if (contract.Info.ActualPOAmount != null) doc.SetCellValue(rowindex, COL_LASTPAYMENT, "Rp. " + contract.Info.ActualPOAmount.Value);
                        if (contract.Info.ActualPOAmountUsd != null) doc.SetCellValue(rowindex, COL_LASTPAYMENTUSD, "US$ " + contract.Info.ActualPOAmountUsd.Value);
                        if (contract.Info.IMFee != null) doc.SetCellValue(rowindex, COL_IMFEE, contract.Info.IMFee.Value);
                        doc.SetCellValue(rowindex, COL_IMREMARK, contract.Info.IMRemark);

                        if (contract.Info.BillingInstallFee != null) doc.SetCellValue(rowindex, COL_BILLINGINSFEE, contract.Info.BillingInstallFee.Value);
                        doc.SetCellValue(rowindex, COL_PAIDFLAG, (contract.Info.NoCheckFlag == true ? "Yes" : "No"));

                        rowno++;

                        int tmpAdvPaidRowIndex = 0;

                        foreach (var payment in contract.AdvPayments)
                        {
                            if (tmpAdvPaidRowIndex == 0)
                            {
                                tmpAdvPaidRowIndex = rowindex;
                            }
                            else
                            {
                                tmpAdvPaidRowIndex++;
                            }

                            if (payment.PaidDate != null) doc.SetCellValue(tmpAdvPaidRowIndex, COL_ADV_PAIDDATE, payment.PaidDate.Value);
                            doc.SetCellValue(tmpAdvPaidRowIndex, COL_ADV_PAYMENTTYPE, payment.PaymentType);
                            doc.SetCellValue(tmpAdvPaidRowIndex, COL_ADV_APPROVENO, payment.ApproveNo);
                            if (payment.PaidAmount != null) doc.SetCellValue(tmpAdvPaidRowIndex, COL_ADV_AMOUNT, "Rp. " + payment.PaidAmount.Value);
                            if (payment.PaidAmountUsd != null) doc.SetCellValue(tmpAdvPaidRowIndex, COL_ADV_AMOUNTUSD, "US$ " + payment.PaidAmountUsd.Value);

                            doc.SetCellStyle(tmpAdvPaidRowIndex, COL_TEAMNO, tmpAdvPaidRowIndex, COL_PAIDFLAG, styBorderDotted);
                        }

                        int tmpPaidRowIndex = 0;
                        foreach (var payment in contract.Payments)
                        {
                            if (tmpPaidRowIndex == 0)
                            {
                                tmpPaidRowIndex = rowindex;
                            }
                            else
                            {
                                tmpPaidRowIndex++;
                            }

                            if (payment.PaidDate != null) doc.SetCellValue(tmpPaidRowIndex, COL_PAIDDATE, payment.PaidDate.Value);
                            doc.SetCellValue(tmpPaidRowIndex, COL_PAYMENTTYPE, payment.PaymentType);
                            doc.SetCellValue(tmpPaidRowIndex, COL_APPROVENO, payment.ApproveNo);
                            if (payment.PaidAmount != null)
                            {
                                doc.SetCellValue(tmpPaidRowIndex, COL_AMOUNT, "Rp. " + payment.PaidAmount.Value);
                                tmpTotal = tmpTotal + payment.PaidAmount;
                                totalvalue = totalvalue + payment.PaidAmount;
                            }
                            if (payment.PaidAmountUsd != null)
                            {
                                doc.SetCellValue(tmpPaidRowIndex, COL_AMOUNTUSD, "US$ " + payment.PaidAmountUsd.Value);
                                tmpTotalUsd = tmpTotalUsd + payment.PaidAmountUsd;
                                totalvalueUsd = totalvalueUsd + payment.PaidAmountUsd;
                            }
                            if (payment.SlipStatus != null) doc.SetCellValue(tmpPaidRowIndex, COL_SLIPSTATUS, payment.SlipStatus);

                            if (payment.PaymentIndex < 4)
                            {
                                var redfont = new SLStyle();
                                redfont.SetFontColor(System.Drawing.Color.Red);
                                doc.SetCellStyle(tmpPaidRowIndex, COL_PAIDDATE, rowindex, COL_AMOUNT, redfont);
                            }

                            if (tmpPaidRowIndex > tmpAdvPaidRowIndex)
                            {
                                doc.SetCellStyle(tmpPaidRowIndex, COL_TEAMNO, tmpPaidRowIndex, COL_PAIDFLAG, styBorderDotted);
                            }
                        }

                        rowindex = Math.Max(tmpAdvPaidRowIndex, tmpPaidRowIndex);
                        rowindex++;
                    }
                    lastrowtotal = rowindex - 1;

                    doc.SetCellStyle(rowindex, COL_TOTAL, new SLStyle()
                    {
                        Font = new SLFont() { FontName = "Tahoma", FontSize = 12, Bold = true }

                    });

                    SLStyle styleBG = new SLStyle();
                    styleBG.Fill.SetPattern(PatternValues.LightGray, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    doc.SetCellStyle(rowindex, COL_TEAMNO, rowindex, COL_PAIDFLAG, styleBG);
                    doc.SetCellStyle(rowindex, COL_TOTAL, style);
                    doc.SetCellValue(rowindex, COL_TOTAL, "Total");

                    doc.SetCellValue(rowindex, COL_TOTAL_VALUE, "Rp. " + tmpTotal);
                    doc.SetCellValue(rowindex, COL_TOTAL_VALUE_USD, "US$ " + tmpTotalUsd);
                }

                SLStyle styleBGGrandTotal = new SLStyle();
                styleBGGrandTotal.Fill.SetPattern(PatternValues.LightGray, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                doc.SetCellStyle(rowindex + 1, COL_TEAMNO, rowindex + 1, COL_PAIDFLAG, styleBGGrandTotal);
                doc.SetCellValue(rowindex + 1, COL_TOTAL_VALUE, "Rp. " + totalvalue.Value);
                doc.SetCellValue(rowindex + 1, COL_TOTAL_VALUE_USD, "US$ " + totalvalueUsd.Value);

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);
                doc.SelectWorksheet(doc.GetWorksheetNames().First());

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateISR130Report(List<dtGetInstallationReportMonthly> data, doInstallationReportMonthly paramSearch)
        {

            //prepare data

            List<string> ListGroupheader = new List<string>();

            string checkdataHeader = string.Empty;
            foreach (var items in data)
            {
                if (checkdataHeader != items.SubContractorNameEN)
                {
                    ListGroupheader.Add(items.SubContractorNameEN);
                    checkdataHeader = items.SubContractorNameEN;
                }
            }



            const string TEMPLATE_NAME = "ISR130.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER_RECEIVEDATE = 3;
            const int COL_HEADER_RECEIVEDATE = 3;
            const int ROW_HEADER_COMPLETEDATE = 4;
            const int COL_HEADER_COMPLETEDATE = 3;
            const int ROW_HEADER_EXPECTEDSTARTDATE = 5;
            const int COL_HEADER_EXPECTEDSTARTDATE = 3;
            const int ROW_HEADER_EXPECTEDCOMPLETEDATE = 6;
            const int COL_HEADER_EXPECTEDCOMPLETEDATE = 3;

            const int ROW_START_DATA = 9;

            int rowindex = ROW_START_DATA;
            int columncount = 0;
            int COL_NO = ++columncount;
            int COL_CONTRACTCODE = ++columncount;
            int COL_SITENAME = ++columncount;
            int COL_SYSTEM = ++columncount;
            int COL_MANAGEMENT_STATUS = ++columncount;
            int COL_DEPOT = ++columncount;
            int COL_SA = ++columncount;
            int COL_IE = ++columncount;
            int COL_RECEIVEDATE = ++columncount;
            int COL_PROPOSTSTARTDATE = ++columncount;
            int COL_PROPOSTCOMPLETEDATE = ++columncount;
            int COL_SUB = ++columncount;
            int COL_EXCEPTEDSTARTDATE = ++columncount;
            int COL_EXCEPTEDCOMPLETEDATE = ++columncount;
            int COL_EXCEPTEDCOMPLETEDATECOUNT = ++columncount;
            int COL_SLIPCOUNT = ++columncount;
            int COL_STARTDATE = ++columncount;
            int COL_FINISHDATE = ++columncount;
            int COL_COMPLETEDATE = ++columncount;
            int COL_COMPLETEPROCESSDATE = ++columncount;
            int COL_OPERATIONSTARTDATE = ++columncount;
            int COL_SUBPRE = ++columncount;
            int COL_SUBPREUSD = ++columncount;
            int COL_PAIDDATE = ++columncount;
            int COL_PAIDDATEUSD = ++columncount;
            int COL_PAYDATE = ++columncount;
            int COL_CONTRACTFINISHBILLINGAMT = ++columncount;
            int COL_CONTRACTFINISHBILLINGAMTUSD = ++columncount;
            int COL_QUOTATIONFINISHNORMALAMT = ++columncount;
            int COL_QUOTATIONFINISHNORMALAMTUSD = ++columncount;
            int COL_TOTALPAIDSUB = ++columncount;
            int COL_TOTALPAIDSUBUSD = ++columncount;
            int COL_BUILDINGTYPE = ++columncount;
            int COL_REMARK = ++columncount;
            int COL_QUOTATIONCUSTOMER = ++columncount;
            int COL_QUOTATIONCUSTOMERPERCENT = ++columncount;
            int COL_CUSTOMERPAIDSUB = ++columncount;
            int COL_PROFIT = ++columncount;


            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");

            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                doc.SelectWorksheet(WSNAME_Working);

                if (paramSearch.ReceiveDateFrom != null && paramSearch.ReceiveDateTo != null)
                {
                    string tmpDate = paramSearch.ReceiveDateFrom.Value.ToString("dd-MMM-yyyy") + " to " + paramSearch.ReceiveDateTo.Value.ToString("dd-MMM-yyyy");
                    doc.SetCellValue(ROW_HEADER_RECEIVEDATE, COL_HEADER_RECEIVEDATE, tmpDate);
                }

                if (paramSearch.CompleteDateFrom != null && paramSearch.CompleteDateTo != null)
                {
                    string tmpDate = paramSearch.CompleteDateFrom.Value.ToString("dd-MMM-yyyy") + " to " + paramSearch.CompleteDateTo.Value.ToString("dd-MMM-yyyy");
                    doc.SetCellValue(ROW_HEADER_COMPLETEDATE, COL_HEADER_COMPLETEDATE, tmpDate);
                }

                if (paramSearch.ExpectedStartDateFrom != null && paramSearch.ExpectedStartDateTo != null)
                {
                    string tmpDate = paramSearch.ExpectedStartDateFrom.Value.ToString("dd-MMM-yyyy") + " to " + paramSearch.ExpectedStartDateTo.Value.ToString("dd-MMM-yyyy");
                    doc.SetCellValue(ROW_HEADER_EXPECTEDSTARTDATE, COL_HEADER_EXPECTEDSTARTDATE, tmpDate);
                }

                if (paramSearch.ExpectedCompleteDateFrom != null && paramSearch.ExpectedCompleteDateTo != null)
                {
                    string tmpDate = paramSearch.ExpectedCompleteDateFrom.Value.ToString("dd-MMM-yyyy") + " to " + paramSearch.ExpectedCompleteDateTo.Value.ToString("dd-MMM-yyyy");
                    doc.SetCellValue(ROW_HEADER_EXPECTEDCOMPLETEDATE, COL_HEADER_EXPECTEDCOMPLETEDATE, tmpDate);
                }

                SLStyle styleBGDateBetween = new SLStyle();
                styleBGDateBetween.Fill.SetPattern(PatternValues.LightGray, System.Drawing.Color.PeachPuff, System.Drawing.Color.PeachPuff);

                int runno = 1;
                long? checkRowData = 0;
                decimal? totalSubpre = 0;
                decimal? totalSubpreUsd = 0;
                decimal? totalPaiddate = 0;
                decimal? totalPaiddateUsd = 0;
                decimal? totalContractfinishbillingamt = 0;
                decimal? totalContractfinishbillingamtUsd = 0;
                decimal? totalQuotationfinishnormalamt = 0;
                decimal? totalQuotationfinishnormalamtUsd = 0;
                decimal? totalPaidsub = 0;
                decimal? totalPaidsubUsd = 0;
                foreach (var items in data)
                {
                    if (checkRowData != items.runningNumber)
                    {
                        doc.SetCellValue(rowindex, COL_NO, runno);
                        if (items.ContractProjectCode != null) doc.SetCellValue(rowindex, COL_CONTRACTCODE, commonUtil.ConvertContractCode(items.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                        if (items.SiteNameEN != null) doc.SetCellValue(rowindex, COL_SITENAME, items.SiteNameEN);
                        if (items.ProductNameEN != null) doc.SetCellValue(rowindex, COL_SYSTEM, items.ProductNameEN);
                        if (items.ManagementStatus != null) doc.SetCellValue(rowindex, COL_MANAGEMENT_STATUS, items.ManagementStatus);
                        if (items.OfficeNameEN != null) doc.SetCellValue(rowindex, COL_DEPOT, items.OfficeNameEN);
                        if (items.SalesPerson != null) doc.SetCellValue(rowindex, COL_SA, items.SalesPerson);
                        if (items.IEPerson != null) doc.SetCellValue(rowindex, COL_IE, items.IEPerson);
                        if (items.RecieveDate != null) doc.SetCellValue(rowindex, COL_RECEIVEDATE, items.RecieveDate.Value);
                        if (items.ProposeInstallStartDate != null) doc.SetCellValue(rowindex, COL_PROPOSTSTARTDATE, items.ProposeInstallStartDate.Value);
                        if (items.ProposeInstallCompleteDate != null) doc.SetCellValue(rowindex, COL_PROPOSTCOMPLETEDATE, items.ProposeInstallCompleteDate.Value);
                        if (items.SubContractorNameEN != null) doc.SetCellValue(rowindex, COL_SUB, items.SubContractorNameEN);
                        if (items.ExpectInstallStartDate != null) doc.SetCellValue(rowindex, COL_EXCEPTEDSTARTDATE, items.ExpectInstallStartDate.Value);
                        if (items.ExpectInstallCompleteDate != null) doc.SetCellValue(rowindex, COL_EXCEPTEDCOMPLETEDATE, items.ExpectInstallCompleteDate.Value);
                        if (items.ExpectCompleteDateUpdateCount != null) doc.SetCellValue(rowindex, COL_EXCEPTEDCOMPLETEDATECOUNT, items.ExpectCompleteDateUpdateCount.Value);
                        if (items.SlipCount != null) doc.SetCellValue(rowindex, COL_SLIPCOUNT, items.SlipCount.Value);
                        if (items.installationStartDate != null) doc.SetCellValue(rowindex, COL_STARTDATE, items.installationStartDate.Value);
                        if (items.InstallationFinishDate != null) doc.SetCellValue(rowindex, COL_FINISHDATE, items.InstallationFinishDate.Value);
                        if (items.InstallationCompleteDate != null) doc.SetCellValue(rowindex, COL_COMPLETEDATE, items.InstallationCompleteDate.Value);
                        if (items.InstallationCompleteProcessingDate != null) doc.SetCellValue(rowindex, COL_COMPLETEPROCESSDATE, items.InstallationCompleteProcessingDate.Value);
                        if (items.StartServiceDate != null) doc.SetCellValue(rowindex, COL_OPERATIONSTARTDATE, items.StartServiceDate.Value);

                        if (items.NormalSubPOAmount != null)
                        {
                            doc.SetCellValue(rowindex, COL_SUBPRE, "Rp. " + items.NormalSubPOAmount.Value);
                            totalSubpre = totalSubpre + items.NormalSubPOAmount;
                        }
                        if (items.NormalSubPOAmountUsd != null)
                        {
                            doc.SetCellValue(rowindex, COL_SUBPREUSD, "US$ " + items.NormalSubPOAmountUsd.Value);
                            totalSubpreUsd = totalSubpreUsd + items.NormalSubPOAmountUsd;
                        }
                        if (items.ActualPOAmount != null)
                        {
                            doc.SetCellValue(rowindex, COL_PAIDDATE, "Rp. " + items.ActualPOAmount.Value);
                            totalPaiddate = totalPaiddate + items.ActualPOAmount;
                        }
                        if (items.ActualPOAmountUsd != null)
                        {
                            doc.SetCellValue(rowindex, COL_PAIDDATEUSD, "US$ " + items.ActualPOAmountUsd.Value);
                            totalPaiddateUsd = totalPaiddateUsd + items.ActualPOAmountUsd;
                        }
                        if (items.LastPaidDate != null) doc.SetCellValue(rowindex, COL_PAYDATE, items.LastPaidDate.Value);

                        if (items.OrderInstallFee != null)
                        {
                            doc.SetCellValue(rowindex, COL_CONTRACTFINISHBILLINGAMT, "Rp. " + items.OrderInstallFee.Value);
                            totalContractfinishbillingamt = totalContractfinishbillingamt + items.OrderInstallFee;
                        }
                        if (items.OrderInstallFeeUsd != null)
                        {
                            doc.SetCellValue(rowindex, COL_CONTRACTFINISHBILLINGAMTUSD, "US$ " + items.OrderInstallFeeUsd.Value);
                            totalContractfinishbillingamtUsd = totalContractfinishbillingamtUsd + items.OrderInstallFeeUsd;
                        }
                        if (items.NormalInstallFee != null)
                        {
                            doc.SetCellValue(rowindex, COL_QUOTATIONFINISHNORMALAMT, "Rp. " + items.NormalInstallFee.Value);
                            totalQuotationfinishnormalamt = totalQuotationfinishnormalamt + items.NormalInstallFee;
                        }
                        if (items.NormalInstallFeeUsd != null)
                        {
                            doc.SetCellValue(rowindex, COL_QUOTATIONFINISHNORMALAMTUSD, "US$ " + items.NormalInstallFeeUsd.Value);
                            totalQuotationfinishnormalamtUsd = totalQuotationfinishnormalamtUsd + items.NormalInstallFeeUsd;
                        }
                        if (items.TotalActualPOAmount != null)
                        {
                            doc.SetCellValue(rowindex, COL_TOTALPAIDSUB, "Rp. " + items.TotalActualPOAmount.Value);
                            totalPaidsub = totalPaidsub + items.TotalActualPOAmount;
                        }
                        if (items.TotalActualPOAmountUsd != null)
                        {
                            doc.SetCellValue(rowindex, COL_TOTALPAIDSUBUSD, "US$ " + items.TotalActualPOAmountUsd.Value);
                            totalPaidsubUsd = totalPaidsubUsd + items.TotalActualPOAmountUsd;
                        }
                        if (items.BuildingType != null) doc.SetCellValue(rowindex, COL_BUILDINGTYPE, items.BuildingType);
                        if (items.RequestMemo != null) doc.SetCellValue(rowindex, COL_REMARK, items.RequestMemo);

                        doc.SetCellValue(rowindex, COL_QUOTATIONCUSTOMER, string.Format(
                            "=IFERROR(({0}/{1})*100, \"-\")",
                            SLConvert.ToCellReference(rowindex, COL_CONTRACTFINISHBILLINGAMT),
                            SLConvert.ToCellReference(rowindex, COL_QUOTATIONFINISHNORMALAMT)
                        ));

                        doc.SetCellValue(rowindex, COL_QUOTATIONCUSTOMERPERCENT, string.Format(
                          "=IFERROR(-1*(100 - {0}), \"-\")",
                          SLConvert.ToCellReference(rowindex, COL_QUOTATIONCUSTOMER)
                         ));

                        doc.SetCellValue(rowindex, COL_CUSTOMERPAIDSUB, string.Format(
                            "=IFERROR(({0}/{1})*100, \"-\")",
                            SLConvert.ToCellReference(rowindex, COL_TOTALPAIDSUB),
                            SLConvert.ToCellReference(rowindex, COL_CONTRACTFINISHBILLINGAMT)
                        ));

                        doc.SetCellValue(rowindex, COL_PROFIT, string.Format(
                            "=IFERROR(-1*({0}-100), \"-\")",
                            SLConvert.ToCellReference(rowindex, COL_CUSTOMERPAIDSUB)
                        ));

                        checkRowData = items.runningNumber;
                        runno++;

                    }
                    else
                    {
                        doc.SetCellValue(rowindex, COL_NO, "");
                        doc.SetCellValue(rowindex, COL_CONTRACTCODE, "");
                        doc.SetCellValue(rowindex, COL_SITENAME, "");
                        doc.SetCellValue(rowindex, COL_SYSTEM, "");
                        doc.SetCellValue(rowindex, COL_DEPOT, "");
                        doc.SetCellValue(rowindex, COL_SA, "");
                        doc.SetCellValue(rowindex, COL_IE, "");
                        doc.SetCellValue(rowindex, COL_RECEIVEDATE, "");
                        doc.SetCellValue(rowindex, COL_PROPOSTSTARTDATE, "");
                        doc.SetCellValue(rowindex, COL_PROPOSTCOMPLETEDATE, "");
                        doc.SetCellValue(rowindex, COL_STARTDATE, "");
                        doc.SetCellValue(rowindex, COL_FINISHDATE, "");
                        doc.SetCellValue(rowindex, COL_COMPLETEDATE, "");
                        doc.SetCellValue(rowindex, COL_COMPLETEPROCESSDATE, "");
                        doc.SetCellValue(rowindex, COL_OPERATIONSTARTDATE, "");
                        if (items.SubContractorNameEN != null) doc.SetCellValue(rowindex, COL_SUB, items.SubContractorNameEN);
                        if (items.ExpectInstallStartDate != null) doc.SetCellValue(rowindex, COL_EXCEPTEDSTARTDATE, items.ExpectInstallStartDate.Value);
                        if (items.ExpectInstallCompleteDate != null) doc.SetCellValue(rowindex, COL_EXCEPTEDCOMPLETEDATE, items.ExpectInstallCompleteDate.Value);
                        if (items.ExpectCompleteDateUpdateCount != null) doc.SetCellValue(rowindex, COL_EXCEPTEDCOMPLETEDATECOUNT, items.ExpectCompleteDateUpdateCount.Value);
                        if (items.NormalSubPOAmount != null) doc.SetCellValue(rowindex, COL_SUBPRE, items.NormalSubPOAmount.Value);
                        if (items.ActualPOAmount != null) doc.SetCellValue(rowindex, COL_PAIDDATE, items.ActualPOAmount.Value);
                        if (items.LastPaidDate != null) doc.SetCellValue(rowindex, COL_PAYDATE, items.LastPaidDate.Value);
                        if (items.OrderInstallFee != null) doc.SetCellValue(rowindex, COL_CONTRACTFINISHBILLINGAMT, "");
                        if (items.NormalInstallFee != null) doc.SetCellValue(rowindex, COL_QUOTATIONFINISHNORMALAMT, "");
                        if (items.TotalActualPOAmount != null) doc.SetCellValue(rowindex, COL_TOTALPAIDSUB, "");
                        if (items.RequestMemo != null) doc.SetCellValue(rowindex, COL_REMARK, "");
                        doc.SetCellValue(rowindex, COL_QUOTATIONCUSTOMER, "");
                        doc.SetCellValue(rowindex, COL_QUOTATIONCUSTOMERPERCENT, "");
                        doc.SetCellValue(rowindex, COL_CUSTOMERPAIDSUB, "");
                        doc.SetCellValue(rowindex, COL_PROFIT, "");

                    }
                    rowindex++;
                }

                SLStyle style = new SLStyle();
                style.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;


                SLStyle styBorder = new SLStyle();
                styBorder.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                styBorder.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                styBorder.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                styBorder.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                styBorder.Font.Bold = true;

                doc.MergeWorksheetCells(rowindex, COL_NO, rowindex, COL_IE);
                doc.SetCellStyle(rowindex, COL_NO, rowindex, COL_PROFIT, styBorder);
                doc.SetCellStyle(rowindex, COL_NO, style);
                doc.SetCellValue(rowindex, COL_NO, "Jobs are " + (runno - 1));

                doc.SetCellValue(rowindex, COL_SUBPRE, "Rp. " + totalSubpre.Value);
                doc.SetCellValue(rowindex, COL_SUBPREUSD, "US$ " + totalSubpreUsd.Value);

                doc.SetCellValue(rowindex, COL_PAIDDATE, "Rp. " + totalPaiddate.Value);
                doc.SetCellValue(rowindex, COL_PAIDDATEUSD, "US$ " + totalPaiddateUsd.Value);

                doc.SetCellValue(rowindex, COL_CONTRACTFINISHBILLINGAMT, "Rp. "+ totalContractfinishbillingamt.Value);
                doc.SetCellValue(rowindex, COL_CONTRACTFINISHBILLINGAMTUSD, "US$ " + totalContractfinishbillingamtUsd.Value);

                doc.SetCellValue(rowindex, COL_QUOTATIONFINISHNORMALAMT, "Rp. " + totalQuotationfinishnormalamt.Value);
                doc.SetCellValue(rowindex, COL_QUOTATIONFINISHNORMALAMTUSD, "US$ " + totalQuotationfinishnormalamtUsd.Value);

                doc.SetCellValue(rowindex, COL_TOTALPAIDSUB, "Rp. " + totalPaidsub.Value);
                doc.SetCellValue(rowindex, COL_TOTALPAIDSUBUSD, "US$ " + totalPaidsubUsd.Value);

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);
                doc.SelectWorksheet(doc.GetWorksheetNames().First());

                doc.SaveAs(strOutputPath);
            }

            return strOutputPath;
        }

        public string GenerateISR140Report(List<dtGetInstallationReportMonthly> data, doInstallationReportMonthly paramSearch)
        {


            //prepare data

            List<string> ListGroupheader = new List<string>();

            string checkdataHeader = string.Empty;
            foreach (var items in data)
            {
                if (checkdataHeader != items.SubContractorNameEN)
                {
                    ListGroupheader.Add(items.SubContractorNameEN);
                    checkdataHeader = items.SubContractorNameEN;
                }
            }

            const string TEMPLATE_NAME = "ISR140.xlsx";
            const string WSNAME_DETAIL = "Detail";
            const string WSNAME_SUMMARY = "Summary";
            const string WSNAME_Working = "Sheet1";

            const int ROW_HEADER_RECEIVEDATE = 3;
            const int COL_HEADER_RECEIVEDATE = 3;
            const int ROW_HEADER_COMPLETEDATE = 4;
            const int COL_HEADER_COMPLETEDATE = 3;
            const int ROW_HEADER_EXPECTEDSTARTDATE = 5;
            const int COL_HEADER_EXPECTEDSTARTDATE = 3;
            const int ROW_HEADER_EXPECTEDCOMPLETEDATE = 6;
            const int COL_HEADER_EXPECTEDCOMPLETEDATE = 3;

            const int ROW_START_DATA = 9;

            int rowindex = ROW_START_DATA;
            int columncount = 0;
            int COL_NO = ++columncount;
            int COL_CONTRACTCODE = ++columncount;
            int COL_SITENAME = ++columncount;
            int COL_SYSTEM = ++columncount;
            int COL_DEPOT = ++columncount;
            int COL_SA = ++columncount;
            int COL_IE = ++columncount;
            int COL_RECEIVEDATE = ++columncount;
            int COL_PROPOSTSTARTDATE = ++columncount;
            int COL_PROPOSTCOMPLETEDATE = ++columncount;
            int COL_SUB = ++columncount;
            int COL_EXCEPTEDSTARTDATE = ++columncount;
            int COL_EXCEPTEDCOMPLETEDATE = ++columncount;
            int COL_EXCEPTEDCOMPLETEDATECOUNT = ++columncount;
            int COL_STARTDATE = ++columncount;
            int COL_FINISHDATE = ++columncount;
            int COL_COMPLETEDATE = ++columncount;
            int COL_COMPLETEPROCESSDATE = ++columncount;
            int COL_OPERATIONSTARTDATE = ++columncount;
            int COL_SUBPRE = ++columncount;
            int COL_SUBPREUSD = ++columncount;
            int COL_PAIDDATE = ++columncount;
            int COL_PAIDDATEUSD = ++columncount;
            int COL_PAYDATE = ++columncount;
            int COL_CONTRACTFINISHBILLINGAMT = ++columncount;
            int COL_CONTRACTFINISHBILLINGAMTUSD = ++columncount;
            int COL_QUOTATIONFINISHNORMALAMT = ++columncount;
            int COL_QUOTATIONFINISHNORMALAMTUSD = ++columncount;
            int COL_TOTALPAIDSUB = ++columncount;
            int COL_TOTALPAIDSUBUSD = ++columncount;
            int COL_REMARK = ++columncount;



            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");

            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_DETAIL);
                doc.SelectWorksheet(WSNAME_Working);

                if (paramSearch.ReceiveDateFrom != null && paramSearch.ReceiveDateTo != null)
                {
                    string tmpDate = paramSearch.ReceiveDateFrom.Value.ToString("dd-MMM-yyyy") + " to " + paramSearch.ReceiveDateTo.Value.ToString("dd-MMM-yyyy");
                    doc.SetCellValue(ROW_HEADER_RECEIVEDATE, COL_HEADER_RECEIVEDATE, tmpDate);
                }

                if (paramSearch.CompleteDateFrom != null && paramSearch.CompleteDateTo != null)
                {
                    string tmpDate = paramSearch.CompleteDateFrom.Value.ToString("dd-MMM-yyyy") + " to " + paramSearch.CompleteDateTo.Value.ToString("dd-MMM-yyyy");
                    doc.SetCellValue(ROW_HEADER_COMPLETEDATE, COL_HEADER_COMPLETEDATE, tmpDate);
                }

                if (paramSearch.ExpectedStartDateFrom != null && paramSearch.ExpectedStartDateTo != null)
                {
                    string tmpDate = paramSearch.ExpectedStartDateFrom.Value.ToString("dd-MMM-yyyy") + " to " + paramSearch.ExpectedStartDateTo.Value.ToString("dd-MMM-yyyy");
                    doc.SetCellValue(ROW_HEADER_EXPECTEDSTARTDATE, COL_HEADER_EXPECTEDSTARTDATE, tmpDate);
                }

                if (paramSearch.ExpectedCompleteDateFrom != null && paramSearch.ExpectedCompleteDateTo != null)
                {
                    string tmpDate = paramSearch.ExpectedCompleteDateFrom.Value.ToString("dd-MMM-yyyy") + " to " + paramSearch.ExpectedCompleteDateTo.Value.ToString("dd-MMM-yyyy");
                    doc.SetCellValue(ROW_HEADER_EXPECTEDCOMPLETEDATE, COL_HEADER_EXPECTEDCOMPLETEDATE, tmpDate);
                }

                SLStyle styleBGDateBetween = new SLStyle();
                styleBGDateBetween.Fill.SetPattern(PatternValues.LightGray, System.Drawing.Color.PeachPuff, System.Drawing.Color.PeachPuff);

                int runno = 1;
                long? checkRowData = 0;
                decimal? totalSubpre = 0;
                decimal? totalSubpreUsd = 0;
                decimal? totalPaiddate = 0;
                decimal? totalPaiddateUsd = 0;
                decimal? totalContractfinishbillingamt = 0;
                decimal? totalContractfinishbillingamtUsd = 0;
                decimal? totalQuotationfinishnormalamt = 0;
                decimal? totalQuotationfinishnormalamtUsd = 0;
                decimal? totalPaidsub = 0;
                decimal? totalPaidsubUsd = 0;
                foreach (var items in data)
                {
                    if (checkRowData != items.runningNumber)
                    {
                        doc.SetCellValue(rowindex, COL_NO, runno);
                        if (items.ContractProjectCode != null) doc.SetCellValue(rowindex, COL_CONTRACTCODE, commonUtil.ConvertContractCode(items.ContractProjectCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                        if (items.SiteNameEN != null) doc.SetCellValue(rowindex, COL_SITENAME, items.SiteNameEN);
                        if (items.ProductNameEN != null) doc.SetCellValue(rowindex, COL_SYSTEM, items.ProductNameEN);
                        if (items.OfficeNameEN != null) doc.SetCellValue(rowindex, COL_DEPOT, items.OfficeNameEN);
                        if (items.SalesPerson != null) doc.SetCellValue(rowindex, COL_SA, items.SalesPerson);
                        if (items.IEPerson != null) doc.SetCellValue(rowindex, COL_IE, items.IEPerson);
                        if (items.RecieveDate != null) doc.SetCellValue(rowindex, COL_RECEIVEDATE, items.RecieveDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.ProposeInstallStartDate != null) doc.SetCellValue(rowindex, COL_PROPOSTSTARTDATE, items.ProposeInstallStartDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.ProposeInstallCompleteDate != null) doc.SetCellValue(rowindex, COL_PROPOSTCOMPLETEDATE, items.ProposeInstallCompleteDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.installationStartDate != null) doc.SetCellValue(rowindex, COL_STARTDATE, items.installationStartDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.InstallationFinishDate != null) doc.SetCellValue(rowindex, COL_FINISHDATE, items.InstallationFinishDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.InstallationCompleteDate != null) doc.SetCellValue(rowindex, COL_COMPLETEDATE, items.InstallationCompleteDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.InstallationCompleteProcessingDate != null) doc.SetCellValue(rowindex, COL_COMPLETEPROCESSDATE, items.InstallationCompleteProcessingDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.StartServiceDate != null) doc.SetCellValue(rowindex, COL_OPERATIONSTARTDATE, items.StartServiceDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.SubContractorNameEN != null) doc.SetCellValue(rowindex, COL_SUB, items.SubContractorNameEN);
                        if (items.ExpectInstallStartDate != null) doc.SetCellValue(rowindex, COL_EXCEPTEDSTARTDATE, items.ExpectInstallStartDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.ExpectInstallCompleteDate != null) doc.SetCellValue(rowindex, COL_EXCEPTEDCOMPLETEDATE, items.ExpectInstallCompleteDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.ExpectCompleteDateUpdateCount != null) doc.SetCellValue(rowindex, COL_EXCEPTEDCOMPLETEDATECOUNT, items.ExpectCompleteDateUpdateCount.Value);
                        if (items.NormalSubPOAmount != null)
                        {
                            doc.SetCellValue(rowindex, COL_SUBPRE, "Rp. " + items.NormalSubPOAmount.Value);
                            totalSubpre = totalSubpre + items.NormalSubPOAmount;
                        }
                        if (items.NormalSubPOAmountUsd != null)
                        {
                            doc.SetCellValue(rowindex, COL_SUBPREUSD, "US$ " + items.NormalSubPOAmountUsd.Value);
                            totalSubpreUsd = totalSubpreUsd + items.NormalSubPOAmountUsd;
                        }
                        if (items.ActualPOAmount != null)
                        {
                            doc.SetCellValue(rowindex, COL_PAIDDATE, "Rp. " + items.ActualPOAmount.Value);
                            totalPaiddate = totalPaiddate + items.ActualPOAmount;
                        }
                        if (items.ActualPOAmountUsd != null)
                        {
                            doc.SetCellValue(rowindex, COL_PAIDDATEUSD, "US$ " + items.ActualPOAmountUsd.Value);
                            totalPaiddateUsd = totalPaiddateUsd + items.ActualPOAmountUsd;
                        }
                        if (items.LastPaidDate != null) doc.SetCellValue(rowindex, COL_PAYDATE, items.LastPaidDate.Value.ToString("dd-MMM-yyyy"));

                        if (items.OrderInstallFee != null)
                        {
                            doc.SetCellValue(rowindex, COL_CONTRACTFINISHBILLINGAMT, "Rp. " + items.OrderInstallFee.Value);
                            totalContractfinishbillingamt = totalContractfinishbillingamt + items.OrderInstallFee;
                        }
                        if (items.OrderInstallFeeUsd != null)
                        {
                            doc.SetCellValue(rowindex, COL_CONTRACTFINISHBILLINGAMTUSD, "US$ " + items.OrderInstallFeeUsd.Value);
                            totalContractfinishbillingamtUsd = totalContractfinishbillingamtUsd + items.OrderInstallFeeUsd;
                        }
                        if (items.NormalInstallFee != null)
                        {
                            doc.SetCellValue(rowindex, COL_QUOTATIONFINISHNORMALAMT, "Rp. " + items.NormalInstallFee.Value);
                            totalQuotationfinishnormalamt = totalQuotationfinishnormalamt + items.NormalInstallFee;
                        }
                        if (items.NormalInstallFeeUsd != null)
                        {
                            doc.SetCellValue(rowindex, COL_QUOTATIONFINISHNORMALAMTUSD, "US$ " + items.NormalInstallFeeUsd.Value);
                            totalQuotationfinishnormalamtUsd = totalQuotationfinishnormalamtUsd + items.NormalInstallFeeUsd;
                        }
                        if (items.TotalActualPOAmount != null)
                        {
                            doc.SetCellValue(rowindex, COL_TOTALPAIDSUB, "Rp. " + items.TotalActualPOAmount.Value);
                            totalPaidsub = totalPaidsub + items.TotalActualPOAmount;
                        }
                        if (items.TotalActualPOAmountUsd != null)
                        {
                            doc.SetCellValue(rowindex, COL_TOTALPAIDSUBUSD, "US$ " + items.TotalActualPOAmountUsd.Value);
                            totalPaidsubUsd = totalPaidsubUsd + items.TotalActualPOAmountUsd;
                        }
                        if (items.RequestMemo != null) doc.SetCellValue(rowindex, COL_REMARK, items.RequestMemo);


                        checkRowData = items.runningNumber;
                        runno++;

                    }
                    else
                    {
                        doc.SetCellValue(rowindex, COL_NO, "");
                        doc.SetCellValue(rowindex, COL_CONTRACTCODE, "");
                        doc.SetCellValue(rowindex, COL_SITENAME, "");
                        doc.SetCellValue(rowindex, COL_SYSTEM, "");
                        doc.SetCellValue(rowindex, COL_DEPOT, "");
                        doc.SetCellValue(rowindex, COL_SA, "");
                        doc.SetCellValue(rowindex, COL_IE, "");
                        doc.SetCellValue(rowindex, COL_RECEIVEDATE, "");
                        doc.SetCellValue(rowindex, COL_PROPOSTSTARTDATE, "");
                        doc.SetCellValue(rowindex, COL_PROPOSTCOMPLETEDATE, "");
                        doc.SetCellValue(rowindex, COL_STARTDATE, "");
                        doc.SetCellValue(rowindex, COL_FINISHDATE, "");
                        doc.SetCellValue(rowindex, COL_COMPLETEDATE, "");
                        doc.SetCellValue(rowindex, COL_COMPLETEPROCESSDATE, "");
                        doc.SetCellValue(rowindex, COL_OPERATIONSTARTDATE, "");
                        if (items.SubContractorNameEN != null) doc.SetCellValue(rowindex, COL_SUB, items.SubContractorNameEN);
                        if (items.ExpectInstallStartDate != null) doc.SetCellValue(rowindex, COL_EXCEPTEDSTARTDATE, items.ExpectInstallStartDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.ExpectInstallCompleteDate != null) doc.SetCellValue(rowindex, COL_EXCEPTEDCOMPLETEDATE, items.ExpectInstallCompleteDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.ExpectCompleteDateUpdateCount != null) doc.SetCellValue(rowindex, COL_EXCEPTEDCOMPLETEDATECOUNT, items.ExpectCompleteDateUpdateCount.Value);
                        if (items.NormalSubPOAmount != null) doc.SetCellValue(rowindex, COL_SUBPRE, items.NormalSubPOAmount.Value);
                        if (items.ActualPOAmount != null) doc.SetCellValue(rowindex, COL_PAIDDATE, items.ActualPOAmount.Value);
                        if (items.LastPaidDate != null) doc.SetCellValue(rowindex, COL_PAYDATE, items.LastPaidDate.Value.ToString("dd-MMM-yyyy"));
                        if (items.OrderInstallFee != null) doc.SetCellValue(rowindex, COL_CONTRACTFINISHBILLINGAMT, "");
                        if (items.NormalInstallFee != null) doc.SetCellValue(rowindex, COL_QUOTATIONFINISHNORMALAMT, "");
                        if (items.TotalActualPOAmount != null) doc.SetCellValue(rowindex, COL_TOTALPAIDSUB, "");
                        if (items.RequestMemo != null) doc.SetCellValue(rowindex, COL_REMARK, "");

                    }
                    rowindex++;
                }

                SLStyle style = new SLStyle();
                style.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;


                SLStyle styBorder = new SLStyle();
                styBorder.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                styBorder.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                styBorder.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                styBorder.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                styBorder.Font.Bold = true;

                doc.MergeWorksheetCells(rowindex, COL_NO, rowindex, COL_IE);
                doc.SetCellStyle(rowindex, COL_NO, rowindex, COL_REMARK, styBorder);
                doc.SetCellStyle(rowindex, COL_NO, style);
                doc.SetCellValue(rowindex, COL_NO, "Jobs are " + (runno - 1));

                doc.SetCellValue(rowindex, COL_SUBPRE, "Rp. " + totalSubpre.Value);
                doc.SetCellValue(rowindex, COL_SUBPREUSD, "US$ " + totalSubpreUsd.Value);

                doc.SetCellValue(rowindex, COL_PAIDDATE, "Rp. " + totalPaiddate.Value);
                doc.SetCellValue(rowindex, COL_PAIDDATEUSD, "US$ " + totalPaiddateUsd.Value);

                doc.SetCellValue(rowindex, COL_CONTRACTFINISHBILLINGAMT, "Rp. " + totalContractfinishbillingamt.Value);
                doc.SetCellValue(rowindex, COL_CONTRACTFINISHBILLINGAMTUSD, "US$ " + totalContractfinishbillingamtUsd.Value);

                doc.SetCellValue(rowindex, COL_QUOTATIONFINISHNORMALAMT, "Rp. " + totalQuotationfinishnormalamt.Value);
                doc.SetCellValue(rowindex, COL_QUOTATIONFINISHNORMALAMTUSD, "US$ " + totalQuotationfinishnormalamtUsd.Value);

                doc.SetCellValue(rowindex, COL_TOTALPAIDSUB, "Rp. " + totalPaidsub.Value);
                doc.SetCellValue(rowindex, COL_TOTALPAIDSUBUSD, "US$ " + totalPaidsubUsd.Value);

                doc.DeleteWorksheet(WSNAME_DETAIL);
                doc.DeleteWorksheet(WSNAME_SUMMARY);
                doc.SelectWorksheet(doc.GetWorksheetNames().First());

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();

            }

            return strOutputPath;
        }

        public List<tbt_InstallationReprint> GetTbt_InstallationReprint()
        {
            ISDataEntities context = new ISDataEntities();
            return context.tbt_InstallationReprint.ToList();
        }
    }


}
