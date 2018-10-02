

using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;


using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.DataEntity.Installation;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.ActionFilters;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Common.Models;


using System.Transactions;
using System.IO;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Billing;
using System.Configuration;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        public ActionResult CMS999_Authority(CMS999_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (CheckUserPermission("CMS999", FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CMS070_ScreenParameter>("CMS999", param, res);
        }

        [Initialize("CMS999")]
        public ActionResult CMS999()
        {
            return View();
        }

        public ActionResult CMS999_InitialGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS999", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }


        public ActionResult CMS999_SearchResponse()
        {

            List<View_dtEmailAddress> list = new List<View_dtEmailAddress>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                View_dtEmailAddress item;
                for (int i = 0; i < 5; i++)
                {
                    item = new View_dtEmailAddress();
                    list.Add(item);
                }



            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtEmailAddress>(list, "Common\\CMS999", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);


        }

        public ActionResult CMS060_GetComboBoxPaymentMethod(string id)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();

            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_PAYMENT_METHOD,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);
            }
            catch
            {
            }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            return Json(CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, "ValueCodeDisplay", "ValueCode", null, false).ToString());
        }

        public ActionResult CMS999_GetIVR100(string strInventorySlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

                var srvInv = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

                //var lstIVR100 = srvInv.GetIVR100(strInventorySlipNo);
                var objNewDoc = new doDocumentDataGenerate()
                {
                    DocumentNo = strInventorySlipNo,
                    DocumentCode = ReportID.C_INV_REPORT_ID_CHECKING_INSTRUMENT,
                    DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT,
                    DocumentData = null, //lstIVR100,
                    OtherKey = new OtherKeyData()
                    {
                        InventorySlipIssueOffice = strInventorySlipNo
                    }
                };

                var srvDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                var strFilePath = srvInv.GenerateIVR100FilePath(strInventorySlipNo, "IVR100", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                ////Stream reportStream = srvDoc.GetDocumentReportFileStream(strFilePath);
                ////return File(reportStream, "application/pdf");

                res.ResultData = new
                {
                    DocumentNo = objNewDoc.DocumentNo,
                    DocumentOCC = objNewDoc.DocumentOCC,
                    DocumentCode = objNewDoc.DocumentCode,
                    FilePath = strFilePath
                };


            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        public ActionResult CMS999_GetIVR110(string strInventorySlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

                var srvInv = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

                //var lstIVR110 = srvInv.GetIVR110(strInventorySlipNo);
                var objNewDoc = new doDocumentDataGenerate()
                {
                    DocumentNo = strInventorySlipNo,
                    DocumentCode = ReportID.C_INV_REPORT_ID_CHECKING_INSTRUMENT,
                    DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT,
                    DocumentData = null, //lstIVR110,
                    OtherKey = new OtherKeyData()
                    {
                        InventorySlipIssueOffice = strInventorySlipNo
                    }
                };

                var srvDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                var strFilePath = srvInv.GenerateIVR110FilePath(strInventorySlipNo, "IVR110", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                ////Stream reportStream = srvDoc.GetDocumentReportFileStream(strFilePath);
                ////return File(reportStream, "application/pdf");

                res.ResultData = new
                {
                    DocumentNo = objNewDoc.DocumentNo,
                    DocumentOCC = objNewDoc.DocumentOCC,
                    DocumentCode = objNewDoc.DocumentCode,
                    FilePath = strFilePath
                };


            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }


        public void CMS999_TestDownload(string strFileName)
        {
            if (string.IsNullOrEmpty(strFileName))
            {
                strFileName = "test";
            }
            Response.AddHeader("Content-Type", "text/csv");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName + ".csv");
            Response.Charset = "windows-874";
            Response.Write("a,b,c,d");
        }

        // WriteDocumentDownloadLog and return filestream
        public ActionResult CMS999_DownloadPdfAndWriteLog(string strDocumentNo, string documentOCC, string strDocumentCode, string fileName)
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


                IDocumentHandler handlerDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                Stream reportStream = handlerDoc.GetDocumentReportFileStream(fileName);

                return File(reportStream, "application/pdf");


            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        #region AKAT REPORT

        public ActionResult CMS999_GetIVR010(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR010 = invHand.GenerateIVR010(strInventorySlipNo, "", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR010(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR010, "application/pdf");
        }

        public ActionResult CMS999_GetIVR020(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR020 = invHand.GenerateIVR020(strInventorySlipNo, "", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR020(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR020, "application/pdf");
        }

        public ActionResult CMS999_GetIVR030(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR030 = invHand.GenerateIVR030(strInventorySlipNo, "", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR030(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR030, "application/pdf");
        }

        public ActionResult CMS999_GetIVR040(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR040 = invHand.GenerateIVR040(strInventorySlipNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR040(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR040, "application/pdf");
        }

        public ActionResult CMS999_GetIVR050(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR050 = invHand.GenerateIVR050(strInventorySlipNo, "", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR050(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR050, "application/pdf");
        }

        public ActionResult CMS999_GetIVR060(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR060 = invHand.GenerateIVR060(strInventorySlipNo, "", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR060(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR060, "application/pdf");
        }

        public ActionResult CMS999_GetIVR150(DateTime? batchdate)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR150 = invHand.GenerateIVR150(batchdate.Value.ToString("yyyyMM"), CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR150(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR150, "application/pdf");
        }

        public ActionResult CMS999_GetIVR140(DateTime? batchdate)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR140 = invHand.GenerateIVR140(batchdate.Value, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR140(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR140, "application/pdf");
        }

        #endregion

        #region Akat Process

        public ActionResult CMS999_TestProcessIVR070()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            DateTime testdate = DateTime.Now.AddMonths(-2);

            using (TransactionScope scope = new TransactionScope())
            {
                //invHand.GenerateSummaryInventoryInOutReport(testdate);
                scope.Complete();
            }

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_TestProcessIVR080()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

            using (TransactionScope scope = new TransactionScope())
            {
                //invHand.GenerateInventorySummaryAsset(CommonUtil.dsTransData.dtUserData.EmpNo, DateTime.Now.AddMonths(1));
                scope.Complete();
            }

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_TestProcessIVR090()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

            string poNumber = null;
            using (TransactionScope scope = new TransactionScope())
            {
                poNumber = invHand.GeneratePurchaseOrderNo("");
                scope.Complete();
            }

            ObjectResultData res = new ObjectResultData();
            res.ResultData = poNumber;
            return Json(res);
        }

        public ActionResult CMS999_TestProcessIVP110()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            using (TransactionScope scope = new TransactionScope())
            {
                invHand.AutoCompleteStockChecking();
                scope.Complete();
            }

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_TestProcessIVP120()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            tbs_LotRunningNo lotRunningNo = new tbs_LotRunningNo();
            lotRunningNo.InstrumentCode = "xxxxxYY";
            lotRunningNo.DepreciationPeriodForContract = "12";
            lotRunningNo.StartYearMonth = "201203";
            string lotNo = null;
            using (TransactionScope scope = new TransactionScope())
            {
                lotNo = invHand.GenerateLotNo(lotRunningNo);
                scope.Complete();
            }

            ObjectResultData res = new ObjectResultData();
            res.ResultData = lotNo;
            return Json(res);
        }

        public ActionResult CMS999_TestProcessIVP040()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            string lotNo = null;
            using (TransactionScope scope = new TransactionScope())
            {
                lotNo = invHand.GenerateInventorySlipNo("3040", "02");
                scope.Complete();
            }

            ObjectResultData res = new ObjectResultData();
            res.ResultData = lotNo;
            return Json(res);
        }

        public ActionResult CMS999_TestProcessIVP060()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

            doGroupNewInstrument doGroupNew1 = new doGroupNewInstrument();
            doGroupNewInstrument doGroupNew2 = new doGroupNewInstrument();
            doGroupNewInstrument doGroupNew3 = new doGroupNewInstrument();
            doGroupNew1.ObjectID = ScreenID.C_INV_SCREEN_ID_STOCKIN_ASSET;
            doGroupNew1.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
            doGroupNew1.Instrumentcode = "1001";
            doGroupNew1.TransferQty = 10;
            doGroupNew1.UnitPrice = 10;

            doGroupNew2.ObjectID = InstrumentLocation.C_INV_LOC_WIP;
            doGroupNew2.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
            doGroupNew2.SourceLocationCode = "19";
            doGroupNew2.ContractCode = "P0000013";
            doGroupNew2.Instrumentcode = "1001";
            doGroupNew2.TransferQty = 10;
            doGroupNew2.UnitPrice = 10;

            doGroupNew3.ObjectID = "ssssss";
            doGroupNew3.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
            doGroupNew3.Instrumentcode = "100000001";
            doGroupNew3.SourceLocationCode = "01";
            doGroupNew3.SourceOfficeCode = "3030";
            doGroupNew3.TransferQty = 10;
            doGroupNew3.UnitPrice = 10;

            decimal lotNo = invHand.CalculateMovingAveragePrice(doGroupNew1);
            lotNo = invHand.CalculateMovingAveragePrice(doGroupNew2);
            lotNo = invHand.CalculateMovingAveragePrice(doGroupNew3);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = lotNo;
            return Json(res);
        }

        public ActionResult CMS999_TestUpdateCompleteInstallation()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            ObjectResultData res = new ObjectResultData();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //res.ResultData = invHand.UpdateCompleteInstallation("0000000IVP03001", "N00IVP03001");
                    res.ResultData = invHand.UpdateCompleteInstallation("0000000IVP03002", "N00IVP03002");
                    scope.Complete();
                }
                catch (Exception e)
                {
                    res.ResultData = e;
                }
            }
            return Json(res);
        }

        public ActionResult CMS999_TestUpdateContractStartService()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            ObjectResultData res = new ObjectResultData();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    res.ResultData = invHand.UpdateContractStartService("N00IVP03003");
                    scope.Complete();
                }
                catch (Exception e)
                {
                    res.ResultData = false;
                }
            }
            return Json(res);
        }

        public ActionResult CMS999_TestUpdateCancelInstallation()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            ObjectResultData res = new ObjectResultData();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    res.ResultData = invHand.UpdateCancelInstallation("N00IVP03004", "0000000IVP03004", "2");
                    scope.Complete();
                }
                catch (Exception e)
                {
                    res.ResultData = false;
                }
            }
            return Json(res);
        }

        public ActionResult CMS999_TestUpdateRealInvestigation()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            ObjectResultData res = new ObjectResultData();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    List<tbt_QuotationInstrumentDetails> quoDetailList = new List<tbt_QuotationInstrumentDetails>();
                    tbt_QuotationInstrumentDetails quoDetail = new tbt_QuotationInstrumentDetails();
                    quoDetail.QuotationTargetCode = "FN0000000080";
                    quoDetail.Alphabet = "AA";
                    quoDetail.InstrumentCode = "1001";
                    quoDetail.InstrumentQty = 100;
                    quoDetail.AddQty = null;
                    quoDetail.RemoveQty = 100;
                    quoDetail.CreateDate = DateTime.Now;
                    quoDetail.CreateBy = "420022";
                    quoDetail.UpdateDate = DateTime.Now;
                    quoDetail.UpdateBy = "420022";
                    quoDetailList.Add(quoDetail);
                    quoDetail = new tbt_QuotationInstrumentDetails();
                    quoDetail.QuotationTargetCode = "FN0000000080";
                    quoDetail.Alphabet = "AA";
                    quoDetail.InstrumentCode = "1002";
                    quoDetail.InstrumentQty = 100;
                    quoDetail.AddQty = 100;
                    quoDetail.RemoveQty = null;
                    quoDetail.CreateDate = DateTime.Now;
                    quoDetail.CreateBy = "420022";
                    quoDetail.UpdateDate = DateTime.Now;
                    quoDetail.UpdateBy = "420022";
                    quoDetailList.Add(quoDetail);
                    quoDetail = new tbt_QuotationInstrumentDetails();
                    quoDetail.QuotationTargetCode = "FN0000000080";
                    quoDetail.Alphabet = "AA";
                    quoDetail.InstrumentCode = "1003";
                    quoDetail.InstrumentQty = 100;
                    quoDetail.AddQty = 100;
                    quoDetail.RemoveQty = 150;
                    quoDetail.CreateDate = DateTime.Now;
                    quoDetail.CreateBy = "420022";
                    quoDetail.UpdateDate = DateTime.Now;
                    quoDetail.UpdateBy = "420022";
                    quoDetailList.Add(quoDetail);

                    res.ResultData = invHand.UpdateRealInvestigation("N00IVP03005", quoDetailList);
                    scope.Complete();
                }
                catch (Exception e)
                {
                    res.ResultData = false;
                }
            }
            return Json(res);
        }

        public ActionResult CMS999_TestUpdateCompleteProject()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            ObjectResultData res = new ObjectResultData();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    res.ResultData = invHand.UpdateCompleteProject("P0000001");
                    scope.Complete();
                }
                catch (Exception e)
                {
                    res.ResultData = false;
                }
            }
            return Json(res);
        }

        public ActionResult CMS999_TestUpdateCustomerAcceptance()
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            ObjectResultData res = new ObjectResultData();
            using (TransactionScope scope = new TransactionScope())
            {
                res.ResultData = invHand.UpdateCustomerAcceptance("", "", DateTime.Now);
                scope.Complete();
            }
            return Json(res);
        }

        #endregion

        #region Test IVP010, IVP020, IVP050, IVP100, IVP130
        public ActionResult CMS999_TestIVP(string procname)
        {

            ObjectResultData res = new ObjectResultData();
            try
            {
                var srvInv = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                switch (procname)
                {
                    case "IVP010.InventoryHandler.NewBooking":
                        CommonUtil.dsTransData.dtTransHeader.ScreenID = ScreenID.C_SCREEN_ID_FN99;
                        res.ResultData += "1:" + srvInv.NewBooking(new doBooking()
                        {
                            ContractCode = "N9000000015",
                            ExpectedStartServiceDate = DateTime.Now,
                            InstrumentCode = new List<string>() { "100000001", "100000002", "100000003", "100000004" },
                            InstrumentQty = new List<int>() { 10, 20, 30, 40 }
                        }).ToString();
                        CommonUtil.dsTransData.dtTransHeader.ScreenID = ScreenID.C_SCREEN_ID_FQ99;
                        res.ResultData += ", 2:" + srvInv.NewBooking(new doBooking()
                        {
                            ContractCode = "N9000000016",
                            ExpectedStartServiceDate = DateTime.Now,
                            InstrumentCode = new List<string>() { "100000001", "100000002", "100000003", "100000004" },
                            InstrumentQty = new List<int>() { 10, 20, 30, 40 }
                        }).ToString();
                        CommonUtil.dsTransData.dtTransHeader.ScreenID = ScreenID.C_SCREEN_ID_FQ99;
                        res.ResultData += ", 3:" + srvInv.NewBooking(new doBooking()
                        {
                            ContractCode = "N9000000017",
                            ExpectedStartServiceDate = DateTime.Now,
                            InstrumentCode = new List<string>() { "1001", "A-WV-CF284", "A-WV-CP254", "IN-F0381", "IN-T0210", "1007", "1008" },
                            InstrumentQty = new List<int>() { 10, 20, 30, 40, 50, 60, 70 }
                        }).ToString();
                        break;
                    case "IVP010.InventoryHandler.ChangeExpectedStartServiceDate":
                        res.ResultData += "1:" + srvInv.ChangeExpectedStartServiceDate(new doBooking()
                        {
                            ContractCode = "N0000000015",
                            ExpectedStartServiceDate = DateTime.Now
                        }).ToString();
                        res.ResultData += "2:" + srvInv.ChangeExpectedStartServiceDate(new doBooking()
                        {
                            ContractCode = "  Never Found  ",
                            ExpectedStartServiceDate = DateTime.Now
                        }).ToString();
                        break;
                    case "IVP010.InventoryHandler.CancelBooking":
                        res.ResultData += "1:" + srvInv.CancelBooking(new doBooking()
                        {
                            ContractCode = "N0000000015"
                        }).ToString();
                        res.ResultData += "2:" + srvInv.CancelBooking(new doBooking()
                        {
                            ContractCode = "  Never Found  "
                        }).ToString();
                        break;
                    case "IVP010.InventoryHandler.ReBooking":
                        res.ResultData += "1:" + srvInv.Rebooking(new doBooking()
                        {
                            ContractCode = "N0000000015"
                        }).ToString();
                        res.ResultData += "2:" + srvInv.Rebooking(new doBooking()
                        {
                            ContractCode = "  Never Found  "
                        }).ToString();
                        break;
                    case "IVP010.InventoryHandler.UpdateStockOutInstrument":
                        res.ResultData += "1:" + CommonUtil.ConvertToXml<doBooking>(new List<doBooking>() { 
                            srvInv.UpdateStockOutInstrument(new doBooking()
                            {
                                ContractCode = "N0000000015",
                                InstrumentCode = new List<string>() { "100000001", "100000002", "100000003", "100000004" },
                                InstrumentQty = new List<int>() { 10, 20, 30, 40 }
                            })
                        });
                        res.ResultData += "2:" + CommonUtil.ConvertToXml<doBooking>(new List<doBooking>() { 
                            srvInv.UpdateStockOutInstrument(new doBooking()
                            {
                                ContractCode = "  Never Found  ",
                                InstrumentCode = new List<string>() { "100000001", "100000002", "100000003", "100000004" },
                                InstrumentQty = new List<int>() { 10, 20, 30, 40 }
                            })
                        });
                        break;
                    case "IVP020.InventoryHandler.UpdateCalculateDepreciation":

                    case "IVP040.InventoryHandler.UpdateStockOutInstrument Office=3040 SlipID=22":
                        res.ResultData += "1:" + srvInv.GenerateInventorySlipNo("3040", "22");
                        break;
                    case "IVP040.InventoryHandler.UpdateStockOutInstrument Office=3040 SlipID=11":
                        res.ResultData += "1:" + srvInv.GenerateInventorySlipNo("3040", "11");
                        break;

                    case "IVP050.InventoryHandler.FreezeInStrumentDataForStockCheckingProcess":
                        res.ResultData = "This must be test on CMS050 as batch job.";
                        break;
                    case "IVP020.InventoryHandler.InsertDepreciationData":
                        res.ResultData += "1:" + srvInv.InsertDepreciationData(new doInsertDepreciationData()
                        {
                            InstrumentCode = "100000004",
                            StartYearMonth = "201201"
                        });
                        res.ResultData += "2:" + srvInv.InsertDepreciationData(new doInsertDepreciationData()
                        {
                            InstrumentCode = "100000001",
                            StartYearMonth = "201201"
                        });
                        break;

                    case "IVP090.InventoryHandler.GeneratePurchaseOrderNo":
                        res.ResultData += "1:" + srvInv.GeneratePurchaseOrderNo("");
                        break;

                    case "IVP100.InventoryHandler.GenerateInventoryAccountData":
                        res.ResultData = srvInv.GenerateInventoryAccountData(CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                        break;
                    case "IVP110.InventoryHandler.AutoCompleteStockChecking":
                        srvInv.AutoCompleteStockChecking();
                        res.ResultData = "Finished";
                        break;
                    case "IVP120.InventoryHandler.GenerateLotNo #1":
                        res.ResultData = srvInv.GenerateLotNo(new tbs_LotRunningNo() { InstrumentCode = "100000001", DepreciationPeriodForContract = "48", StartYearMonth = "201204" });
                        break;
                    case "IVP120.InventoryHandler.GenerateLotNo #2":
                        res.ResultData = srvInv.GenerateLotNo(new tbs_LotRunningNo() { InstrumentCode = "100000001", DepreciationPeriodForContract = "60", StartYearMonth = "201204" });
                        break;
                    case "IVP120.InventoryHandler.GenerateLotNo #3":
                        res.ResultData = srvInv.GenerateLotNo(new tbs_LotRunningNo() { InstrumentCode = "100000001", DepreciationPeriodForContract = "60", StartYearMonth = "201205" });
                        break;
                    case "IVP120.InventoryHandler.GenerateLotNo #4":
                        res.ResultData = srvInv.GenerateLotNo(new tbs_LotRunningNo() { InstrumentCode = "100000002", DepreciationPeriodForContract = "24", StartYearMonth = "201204" });
                        break;
                    case "IVP120.InventoryHandler.GenerateLotNo #5":
                        res.ResultData = srvInv.GenerateLotNo(new tbs_LotRunningNo() { InstrumentCode = "100000003", DepreciationPeriodForContract = "10", StartYearMonth = "201204" });
                        break;
                    case "IVP120.InventoryHandler.GenerateLotNo #6":
                        res.ResultData = srvInv.GenerateLotNo(new tbs_LotRunningNo() { InstrumentCode = "100000004", DepreciationPeriodForContract = "10", StartYearMonth = "201204" });
                        break;
                    case "IVP120.InventoryHandler.GenerateLotNo #7":
                        res.ResultData = srvInv.GenerateLotNo(new tbs_LotRunningNo() { InstrumentCode = "100000004", DepreciationPeriodForContract = "11", StartYearMonth = "201204" });
                        break;
                    case "IVP120.InventoryHandler.GenerateLotNo #8":
                        res.ResultData = srvInv.GenerateLotNo(new tbs_LotRunningNo() { InstrumentCode = "100000005", DepreciationPeriodForContract = "50", StartYearMonth = "201204" });
                        break;
                    case "IVP030.InventoryHandler.UpdateCompleteInstallation #1":
                        res.ResultData = srvInv.UpdateCompleteInstallation("0000000IVP03001", "N00IVP03001");
                        break;
                    case "IVP030.InventoryHandler.UpdateCompleteInstallation #2":
                        res.ResultData = srvInv.UpdateCompleteInstallation("0000000IVP03002", "N00IVP03002");
                        break;
                    case "IVP030.InventoryHandler.UpdateContractStartService":
                        res.ResultData = srvInv.UpdateContractStartService("N00IVP03003");
                        break;
                    case "IVP030.InventoryHandler.UpdateCancelInstallation":
                        res.ResultData = srvInv.UpdateCancelInstallation("N00IVP03004", "0000000IVP03004", "2");
                        break;
                    case "IVP030.InventoryHandler.UpdateRealInvestigation":
                        List<tbt_QuotationInstrumentDetails> lstParam = new List<tbt_QuotationInstrumentDetails>() {
                            new tbt_QuotationInstrumentDetails() { QuotationTargetCode = "FN0000000080", Alphabet = "AA", InstrumentCode = "1001", InstrumentQty = 100, AddQty = null, RemoveQty = 100, CreateDate = new DateTime(2011,8,26,10,21,31,593), CreateBy = "420022", UpdateDate = new DateTime(2011,8,26,10,21,31,593), UpdateBy = "420022" },
                            new tbt_QuotationInstrumentDetails() { QuotationTargetCode = "FN0000000080", Alphabet = "AA", InstrumentCode = "1002", InstrumentQty = 100, AddQty = 100, RemoveQty = null, CreateDate = new DateTime(2011,8,26,10,21,31,593), CreateBy = "420022", UpdateDate = new DateTime(2011,8,26,10,21,31,593), UpdateBy = "420022" },
                            new tbt_QuotationInstrumentDetails() { QuotationTargetCode = "FN0000000080", Alphabet = "AA", InstrumentCode = "1003", InstrumentQty = 100, AddQty = 100, RemoveQty = 150, CreateDate = new DateTime(2011,8,26,10,21,31,593), CreateBy = "420022", UpdateDate = new DateTime(2011,8,26,10,21,31,593), UpdateBy = "420022" }
                        };
                        res.ResultData = srvInv.UpdateRealInvestigation("N00IVP03005", lstParam);
                        break;
                    case "IVP030.InventoryHandler.UpdateCompleteProject":
                        res.ResultData = srvInv.UpdateCompleteProject("P0000001");
                        break;

                    case "IVP060.CalculateMovingAveragePrice #1":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "17",
                            DestinationLocationCode = "01",
                            Instrumentcode = "1001",
                            TransferQty = 50,
                            UnitPrice = 70,
                            ObjectID = "IVS012",
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #2":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "01",
                            DestinationLocationCode = "10",
                            ContractCode = "P0000003",
                            Instrumentcode = "1001",
                            TransferQty = 50,
                            ObjectID = "0",
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #3":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "10",
                            DestinationLocationCode = "12",
                            ContractCode = "P0000003",
                            Instrumentcode = "1001",
                            TransferQty = 80,
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #4":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "06",
                            DestinationLocationCode = "10",
                            ContractCode = "P0000003",
                            Instrumentcode = "1002",
                            TransferQty = 50,
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #5":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "19",
                            DestinationLocationCode = "10",
                            ProjectCode = "P0000003",
                            ContractCode = "P0000003",
                            Instrumentcode = "1003",
                            TransferQty = 60,
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #6":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "20",
                            DestinationLocationCode = "11",
                            ContractCode = "P0000003",
                            Instrumentcode = "1001",
                            TransferQty = 70,
                            LotNo = "003",
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #7":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "01",
                            DestinationLocationCode = "04",
                            Instrumentcode = "1002",
                            TransferQty = 20,
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #8":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "02",
                            DestinationLocationCode = "08",
                            Instrumentcode = "1001",
                            TransferQty = 50,
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #9":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "09",
                            DestinationLocationCode = "01",
                            Instrumentcode = "1002",
                            TransferQty = 60,
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #10":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "01",
                            DestinationLocationCode = "10",
                            Instrumentcode = "1003",
                            TransferQty = 75,
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #11":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "01",
                            DestinationLocationCode = "06",
                            Instrumentcode = "1001",
                            TransferQty = 50,
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #12":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "01",
                            DestinationLocationCode = "19",
                            Instrumentcode = "1002",
                            TransferQty = 60,
                        });
                        break;
                    case "IVP060.CalculateMovingAveragePrice #13":
                        res.ResultData = srvInv.CalculateMovingAveragePrice(new doGroupNewInstrument()
                        {
                            SourceOfficeCode = "3040",
                            DestinationOfficeCode = "3040",
                            SourceLocationCode = "02",
                            DestinationLocationCode = "01",
                            Instrumentcode = "1001",
                            TransferQty = 20,
                            LotNo = "001",
                        });
                        break;

                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        #endregion

        #region TON Report

        public ActionResult CMS999_GetIVR070(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR070 = invHand.GenerateIVR070(strInventorySlipNo, "", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR070(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR070, "application/pdf");
        }

        public ActionResult CMS999_GetIVR080(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR080 = invHand.GenerateIVR080(strInventorySlipNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR080(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR080, "application/pdf");
        }

        public ActionResult CMS999_GetIVR090(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR090 = invHand.GenerateIVR090(strInventorySlipNo, "", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR090(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR090, "application/pdf");
        }

        public ActionResult CMS999_GetIVR120(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR120 = invHand.GenerateIVR120(strInventorySlipNo, "", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR120(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR120, "application/pdf");
        }

        public ActionResult CMS999_GetIVR130(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR130 = invHand.GenerateIVR130(strInventorySlipNo, "", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR130(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR130, "application/pdf");
        }

        public ActionResult CMS999_GetIVR170(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR170 = invHand.GenerateIVR170(strInventorySlipNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR170(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR170, "application/pdf");
        }

        public ActionResult CMS999_GetIVR180(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR180 = invHand.GenerateIVR180(strInventorySlipNo, "", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR180(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR180, "application/pdf");
        }

        public ActionResult CMS999_GetIVR190(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR190 = invHand.GenerateIVR190(strInventorySlipNo, "3040", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR190(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR190, "application/pdf");
        }

        public ActionResult CMS999_GetIVR191(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR191 = invHand.GenerateIVR191(strInventorySlipNo, "3040", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR191(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR191, "application/pdf");
        }
        
        public ActionResult CMS999_GetIVR192(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR192 = invHand.GenerateIVR192(strInventorySlipNo, "3040", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }
        
        public ActionResult CMS999_DownloadIVR192(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR192, "application/pdf");
        }

        public ActionResult CMS999_GetIVR210(string strInventorySlipNo)
        {
            IInventoryDocumentHandler invHand = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.IVR210 = invHand.GenerateIVR210(strInventorySlipNo, "3040", CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadIVR210(string strInventorySlipNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.IVR210, "application/pdf");
        }

        #endregion


        public ActionResult CMS999_CTP060_TestSendNotifyEmail()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IContractHandler handler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                doBatchProcessResult result = handler.SendNotifyEmailForChangeFee("420022", DateTime.Now);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        public ActionResult CMS999_CTP070_TestUpdateCustomerAcceptance(int mode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                string ContractCode = null;
                string SaleOCC = null;
                DateTime? CustomerAcceptanceDate = null;

                switch (mode)
                {
                    case 1:
                        {
                            ContractCode = null;
                            SaleOCC = null;
                            CustomerAcceptanceDate = null;
                        } break;
                    case 2:
                        {
                            ContractCode = "Q0000000001";
                            SaleOCC = "0001";
                            CustomerAcceptanceDate = DateTime.Now;
                        } break;
                    case 3:
                        {
                            ContractCode = "Q0000700015";
                            SaleOCC = "0001";
                            CustomerAcceptanceDate = DateTime.Now;
                        } break;
                    case 4:
                        {
                            ContractCode = "Q0000700025";
                            SaleOCC = "0001";
                            CustomerAcceptanceDate = DateTime.Now;
                        } break;
                    case 5:
                        {
                            ContractCode = "Q0000700035";
                            SaleOCC = "0001";
                            CustomerAcceptanceDate = DateTime.Now;
                        } break;
                }

                ISaleContractHandler saleContractHanlder = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                saleContractHanlder.UpdateCustomerAcceptance(ContractCode, SaleOCC, CustomerAcceptanceDate);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        public ActionResult CMS999_CTP080_TestGenerateProjecCode()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IProjectHandler handler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                string code1 = handler.GenerateProjectCode();
                string code2 = handler.GenerateProjectCode();

                res.ResultData = new string[] { code1, code2 };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        public ActionResult CMS999_CTP110_TestGenerateMaintenanceCheckupSchedule(string mode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                string ContractCode = null;
                string MAProcessType = null;

                switch (mode)
                {
                    case "1":
                        {
                            ContractCode = null;
                            MAProcessType = null;
                        } break;
                    case "2":
                        {
                            ContractCode = null;
                            MAProcessType = GenerateMAProcessType.C_GEN_MA_TYPE_CREATE;
                        } break;
                    case "3.1":
                        {
                            ContractCode = "N0001100012";
                            MAProcessType = GenerateMAProcessType.C_GEN_MA_TYPE_CREATE;
                        } break;
                    case "3.2":
                        {
                            ContractCode = "N0001100022";
                            MAProcessType = GenerateMAProcessType.C_GEN_MA_TYPE_CREATE;
                        } break;
                    case "3.3":
                        {
                            ContractCode = "N0001100032";
                            MAProcessType = GenerateMAProcessType.C_GEN_MA_TYPE_CREATE;
                        } break;
                    case "4":
                        {
                            ContractCode = "MA0001100042";
                            MAProcessType = GenerateMAProcessType.C_GEN_MA_TYPE_CREATE;
                        } break;
                    case "5":
                        {
                            ContractCode = "MA0001100072";
                            MAProcessType = GenerateMAProcessType.C_GEN_MA_TYPE_CREATE;
                        } break;
                    case "6":
                        {
                            ContractCode = "N0001100082";
                            MAProcessType = GenerateMAProcessType.C_GEN_MA_TYPE_CREATE;
                        } break;
                    case "7":
                        {
                            ContractCode = "N0001100092";
                            MAProcessType = GenerateMAProcessType.C_GEN_MA_TYPE_RE_CREATE;
                        } break;
                }

                IMaintenanceHandler handler = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                handler.GenerateMaintenanceSchedule(ContractCode, MAProcessType);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #region Installation Report

        public ActionResult CMS999_DownloadISR010(string strSlipNo)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(insHand.CreateInstallationReport(strSlipNo, DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL), "application/pdf");
        }
        public ActionResult CMS999_DownloadISR020(string strSlipNo)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(insHand.CreateInstallationReport(strSlipNo, DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP), "application/pdf");
        }
        public ActionResult CMS999_DownloadISR030(string strSlipNo)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(insHand.CreateInstallationReport(strSlipNo, DocumentCode.C_DOCUMENT_CODE_REMOVAL_INSTALL_SLIP), "application/pdf");
        }
        public ActionResult CMS999_DownloadISR040(string strSlipNo)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(insHand.CreateInstallationReport(strSlipNo, DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE), "application/pdf");
        }

        public ActionResult CMS999_ISRReprint(string strSlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            res.ResultData = true;

            try
            {
                List<tbt_InstallationReprint> lstSlipNo = insHand.GetTbt_InstallationReprint();
                foreach (tbt_InstallationReprint slip in lstSlipNo)
                {
                    try
                    {
                        if (slip.DocumentCode == DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_RENTAL
                            || slip.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_INSTALL_SLIP
                            || slip.DocumentCode == DocumentCode.C_DOCUMENT_CODE_REMOVAL_INSTALL_SLIP
                            || slip.DocumentCode == DocumentCode.C_DOCUMENT_CODE_NEW_INSTALL_SLIP_SALE)
                        {
                            Stream tmp = insHand.CreateInstallationReport(slip.SlipNo, slip.DocumentCode);

                            if (tmp != null)
                            {
                                tmp.Close();
                            }
                        }
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

        public ActionResult CMS999_DownloadISR050(string strMaintenanceNo, string strSubcontractorCode)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            string nameSignature = ConfigurationManager.AppSettings["NameSignature"];
            return File(insHand.CreateReportInstallationPOandSubPrice(strMaintenanceNo, strSubcontractorCode, nameSignature), "application/pdf");
        }
        public ActionResult CMS999_DownloadISR060(string strMaintenanceNo)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(insHand.CreateReportInstallationRequestData(strMaintenanceNo), "application/pdf");
        }
        public ActionResult CMS999_DownloadISR070(string strMaintenanceNo, string strSubcontractorCode)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(insHand.CreateReportInstallSpecCompleteData(strMaintenanceNo, strSubcontractorCode), "application/pdf");
        }
        public ActionResult CMS999_DownloadISR080(string strMaintenanceNo, string strSubcontractorCode)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(insHand.CreateReportIECheckSheetData(strMaintenanceNo, strSubcontractorCode), "application/pdf");
        }
        public ActionResult CMS999_DownloadISR090(string strSlipNo)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(insHand.CreateReportInstallCompleteConfirmData(strSlipNo), "application/pdf");
        }
        public ActionResult CMS999_DownloadISR100(string strMaintenanceNo, string strSubcontractorCode)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(insHand.CreateReportAcceptanceInspectionNotice(strMaintenanceNo, strSubcontractorCode), "application/pdf");
        }
        public ActionResult CMS999_DownloadISR110(string strSlipNo)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(insHand.CreateReportInstallationCompleteConfirmation(strSlipNo), "application/pdf");
        }
        public ActionResult CMS999_DownloadISR111(string strSlipNo)
        {
            IInstallationDocumentHandler insHand = ServiceContainer.GetService<IInstallationDocumentHandler>() as IInstallationDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(insHand.CreateReportDeliveryConfirmData(strSlipNo), "application/pdf");
        }
        #endregion

        public ActionResult CMS999_GenerateTempFile(int numOfFiles)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                for(int i = 1; i <= numOfFiles; i++)
                {
                    PathUtil.GetTempFileName(".pdf");
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult CMS999_ClearReportCache()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var handler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                handler.ClearReportCache();
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        public ActionResult CMS999_RunIVP140(DateTime? batchdate)
        {
            IInventoryHandler invHand = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

            if (batchdate == null)
            {
                throw new ArgumentNullException("batchDate");
            }

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            invHand.UpdateMonthlyAveragePrice(CommonUtil.dsTransData.dtUserData.EmpNo, new DateTime?(CommonUtil.dsTransData.dtOperationData.ProcessDateTime), batchdate.Value);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }
        
        public ActionResult CMS999_GetICR030(string strDocumentNo)
        {
            IIncomeDocumentHandler hand = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.ICR030 = hand.GenerateICR030(strDocumentNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadICR030(string strDocumentNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.ICR030, "application/pdf");
        }

        public ActionResult CMS999_GetICR040(string strDocumentNo)
        {
            IIncomeDocumentHandler hand = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;

            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            sParam.ICR040 = hand.GenerateICR040(strDocumentNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            ObjectResultData res = new ObjectResultData();
            res.ResultData = true;
            return Json(res);
        }

        public ActionResult CMS999_DownloadICR040(string strDocumentNo)
        {
            CMS999_ScreenParameter sParam = GetScreenObject<CMS999_ScreenParameter>();
            return File(sParam.ICR040, "application/pdf");
        }

    }
}
