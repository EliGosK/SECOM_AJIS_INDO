//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Contract;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority & Initialize

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS040_Authority(CTS040_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_APPROVE_RESULT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS040_ScreenParameter>("CTS040", param, res);
        }
        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS040")]
        public ActionResult CTS040()
        {
            #region Test CTP
            #region CheckMaintenanceTargetContractList
            //IContractHandler hand = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            //List<string> lst = new List<string>();
            //lst.Add("Q0000000252");
            //lst.Add("Q0030000302");
            //lst.Add("Q0000000088");
            //hand.CheckMaintenanceTargetContractList(lst);
            #endregion
            #region BillingTemp
            //IBillingTempHandler hand = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            //tbt_BillingTemp billing = new tbt_BillingTemp();
            //billing.ContractCode = "N0000000099";
            //billing.OCC = "0001";
            //billing.SequenceNo = 1;
            //billing.BillingType = "99";
            //billing.BillingAmt = 9999;
            //billing.PayMethod = "9";
            //billing.BillingOCC = "99"; 
            //billing.BillingTargetRunningNo = "999";
            //billing.BillingClientCode = "9999999999";
            //billing.BillingTargetCode = "9999999999-999"; 
            //billing.BillingOfficeCode = "9999";
            //billing.DebtTracingOfficeCode = "99";
            //billing.BillingType = "99";
            //billing.BillingTiming = "9";
            //billing.BillingCycle = 99;
            //billing.CalDailyFeeStatus = "9";
            //billing.SendFlag = "9";
            //CommonUtil.dsTransData.dtOperationData.ProcessDateTime = DateTime.Now;
            //CommonUtil.dsTransData.dtUserData.EmpNo = "490440";
            //hand.InsertBillingTemp(billing);
            //hand.UpdateBillingTempByBillingClientAndOffice(billing.ContractCode, billing.BillingClientCode, billing.BillingOfficeCode, billing.BillingOCC, billing.BillingTargetCode);
            //hand.UpdateBillingTempByBillingTarget(billing.ContractCode, billing.BillingClientCode, billing.BillingOfficeCode, billing.BillingTargetCode, "0010000059", "7019", "0010000051-969");
            //hand.UpdateBillingTempByKey(billing);
            //hand.DeleteBillingTempByContractCode("N0000000091");
            //hand.DeleteBillingTempByContractCodeOCC("N0000000092", "0001");
            //hand.DeleteBillingTempByKey("N0000000093", "0001", 1);
            #endregion
            #region BillingInterface
            //CommonUtil.dsTransData.dtOperationData.ProcessDateTime = DateTime.Now;
            //CommonUtil.dsTransData.dtUserData.EmpNo = "490440";
            //IBillingTempHandler hand = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            //List<doBillingBasic> basiclist = hand.GetBillingBasicData("N0000000001", "0001", "03", "3");
            //List<doBillingDetail> detailList = hand.GetBillingDetailData("X0000000001", null, null, "0");

            //IBillingInterfaceHandler hand = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            //hand.SendBilling_RentalApprove("X0000000099");
            #endregion
            #region Lot4
            //ISaleContractHandler hand = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            //hand.GenerateContractOCC("Q0030000999");
            //hand.GenerateContractCounter("Q0030000999");
            
            //IRentralContractHandler hand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            //hand.GenerateContractOCC("N0000000001", false);
            //hand.GenerateContractOCC("N0000000003", true);
            //hand.GenerateContractCounter("N0000000001");

            //IContractHandler hand2 = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            //hand2.GenerateContractCode("1");

            //IContractDocumentHandler hand2 = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            //hand2.GenerateDocOCC("N0000000006", "0001");
            //hand2.GenerateDocOCC("Q0000000489", "AB");

            #endregion
            #region ProjectHandler
            //IProjectHandler hand = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            //hand.GenerateProjectCode();
            #endregion
            #region UpdateCustomerAcceptance
            //ISaleContractHandler hand = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            //CommonUtil.dsTransData.dtUserData.EmpNo = "490440";
            //CommonUtil.dsTransData.dtOperationData.ProcessDateTime = DateTime.Now;
            //hand.UpdateCustomerAcceptance("Q0000000999", "0001", DateTime.Now);
            #endregion
            #region AutoRenew
            //CommonUtil.dsTransData.dtUserData.EmpNo = "490440";
            //CommonUtil.dsTransData.dtOperationData.ProcessDateTime = DateTime.Now;
            //IRentralContractHandler hand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            //hand.AutoRenew();
            #endregion
            #region UpdateStockOUtInstrument
            //CommonUtil.dsTransData.dtUserData.EmpNo = "490440";
            //CommonUtil.dsTransData.dtOperationData.ProcessDateTime = DateTime.Now;
            //IProjectHandler hand = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
            //List<doInstrument> instList = new List<doInstrument>();
            //doInstrument doIn = new doInstrument();
            //doIn.InstrumentCode = "1001";
            //doIn.InstrumentQty = 1;
            //instList.Add(doIn);
            ////doInstrument doIn2 = new doInstrument();
            ////doIn2.InstrumentCode = "1002";
            ////doIn2.InstrumentQty = 2;
            ////instList.Add(doIn2);
            //hand.UpdateStockOutInstrument("12345678", instList);
            //instList = new List<doInstrument>();
            //doInstrument doIn3 = new doInstrument();
            //doIn3.InstrumentCode = "1001";
            //doIn3.InstrumentQty = 10;
            //instList.Add(doIn3);
            //hand.UpdateStockOutInstrument("12345678", instList);
            #endregion
            #region CheckRelationType
            //ISaleContractHandler hand = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            //doSaleContractData doSale = hand.CheckLinkageSaleContract("Q0000000084");
            #endregion
            #region Installation
            //CommonUtil.dsTransData.dtOperationData.ProcessDateTime = DateTime.Now;
            //CommonUtil.dsTransData.dtUserData.EmpNo = "490440";

            //doCompleteInstallationData doComplete = new doCompleteInstallationData();
            //doComplete.ContractCode = "N0000000015";
            //doComplete.InstallationCompleteDate = DateTime.Now;
            //doComplete.IEInchargeEmpNo = "1234567890";
            //doComplete.InstallationSlipNo = "1234567890123456";
            //doComplete.InstallationType = SaleInstallationType.C_SALE_INSTALL_TYPE_NEW;
            //doComplete.SECOMPaymentFee = 1000;
            //doComplete.SECOMRevenueFee = 1000;
            //doComplete.NormalInstallationFee = 1000;
            //doComplete.BillingInstallationFee = 1000;

            //doComplete.doInstrumentDetailsList = new List<doInstrumentDetails>();

            //doInstrumentDetails saleDetailDo1 = new doInstrumentDetails();
            //saleDetailDo1.InstrumentCode = "ABCDEFG1";
            //saleDetailDo1.AddQty = 10;
            //saleDetailDo1.RemoveQty = 5;
            //doComplete.doInstrumentDetailsList.Add(saleDetailDo1);

            //doInstrumentDetails saleDetailDo2 = new doInstrumentDetails();
            //saleDetailDo2.InstrumentCode = "ABCDEFG2";
            //saleDetailDo2.AddQty = 6;
            //saleDetailDo2.RemoveQty = 4;
            //doComplete.doInstrumentDetailsList.Add(saleDetailDo2);

            //doComplete.doSubcontractorDetailsList = new List<doSubcontractorDetails>();
            
            //doSubcontractorDetails saleSub1 = new doSubcontractorDetails();
            //saleSub1.SubcontractorCode = "1111";
            //doComplete.doSubcontractorDetailsList.Add(saleSub1);

            //doSubcontractorDetails saleSub2 = new doSubcontractorDetails();
            //saleSub2.SubcontractorCode = "2222";
            //doComplete.doSubcontractorDetailsList.Add(saleSub2);

            ////ISaleContractHandler hand = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            ////hand.CancelInstallation(doComplete);
            ////hand.CompleteInstallation(doComplete);
            //IRentralContractHandler hand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            //hand.CompleteInstallation(doComplete);
            #endregion
            #region SendNotifyEmailForChangeFee
            //IContractHandler hand = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            //doBatchProcessResult doResult = hand.SendNotifyEmailForChangeFee();
            #endregion
            #region GenerateMaintenanceSchedule
            //IMaintenanceHandler hand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
            //hand.GenerateMaintenanceSchedule("N0000000004", GenerateMAProcessType.C_GEN_MA_TYPE_CREATE);
            //hand.GenerateMaintenanceSchedule("N0000000002", GenerateMAProcessType.C_GEN_MA_TYPE_CREATE);
            //hand.GenerateMaintenanceSchedule("N0000000004", GenerateMAProcessType.C_GEN_MA_TYPE_RE_CREATE);
            //hand.GenerateMaintenanceSchedule("N0000000002", GenerateMAProcessType.C_GEN_MA_TYPE_RE_CREATE);
            #endregion
            #endregion

            ViewBag.PageRow = CommonValue.ROWS_PER_PAGE_FOR_SEARCHPAGE;

            return View("CTS040");
        }

        #endregion

        /// <summary>
        /// Initial grid
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_CTS040()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS040", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        /// <summary>
        /// Check required field
        /// </summary>
        /// <param name="QuotationCode"></param>
        /// <param name="Alphabet"></param>
        /// <returns></returns>
        public ActionResult CTS040_CheckReqField(string QuotationCode, string Alphabet)
        {
            
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                
                if (!CommonUtil.IsNullOrEmpty(Alphabet) && CommonUtil.IsNullOrEmpty(QuotationCode))
                {
                    res.AddErrorMessage (MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_SEARCH_APPROVE_RESULT,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblQuotationTargetCode" },
                                        new string[] { "QuotationCode" });
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
        /// <summary>
        /// Load search result to grid
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS040_Search(CTS040_Search cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { cond }); //AtLeast1FieldNotNullOrEmptyAttribute
                if (res.IsError)
                  return Json(res);
      

                CommonUtil c = new CommonUtil();
                cond.QuotationCode = c.ConvertQuotationTargetCode(cond.QuotationCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                setContractOfficeCode(cond);
                setOperationOfficeCode(cond);

                IDraftContractHandler hand = ServiceContainer.GetService<IDraftContractHandler>() as IDraftContractHandler;
                List<dtSearchDraftContractResult> list = hand.SearchDraftContractList(cond);

                res.ResultData = CommonUtil.ConvertToXml<dtSearchDraftContractResult>(list, "Contract\\CTS040", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Set contract office code
        /// </summary>
        /// <param name="cond"></param>
        private void setContractOfficeCode(doSearchDraftContractCondition cond)
        {
            if (cond.ContractOfficeCode == null)
            {
                List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
                StringBuilder sbContractOffice = new StringBuilder("");
                foreach (OfficeDataDo off in clst)
                {
                    if (off.FunctionSale != SECOM_AJIS.Common.Util.ConstantValue.FunctionSale.C_FUNC_SALE_NO)
                        sbContractOffice.AppendFormat("\'{0}\',", off.OfficeCode);
                }

                if (sbContractOffice.Length > 0)
                    cond.ContractOfficeCode = sbContractOffice.Remove(sbContractOffice.Length - 1, 1).ToString();
            }
        }
        /// <summary>
        /// Set operation office code
        /// </summary>
        /// <param name="cond"></param>
        private void setOperationOfficeCode(doSearchDraftContractCondition cond)
        {
            if (cond.OperationOfficeCode == null)
            {
                List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
                StringBuilder sbOperationOffice = new StringBuilder("");
                foreach (OfficeDataDo off in clst)
                {
                    if (off.FunctionSecurity != SECOM_AJIS.Common.Util.ConstantValue.FunctionSecurity.C_FUNC_SECURITY_NO)
                        sbOperationOffice.AppendFormat("\'{0}\',", off.OfficeCode);
                }

                if (sbOperationOffice.Length > 0)
                    cond.OperationOfficeCode = sbOperationOffice.Remove(sbOperationOffice.Length - 1, 1).ToString();
            }
        }

        #region Remove not used code
        //private void setApproveContractStatus(doSearchDraftContractCondition cond)
        //{
        //    if (cond.ApproveContractStatus == null)
        //    {
        //        #region GetMisc
        //        List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
        //        try
        //        {
        //            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
        //                {
        //                    new doMiscTypeCode()
        //                    {
        //                        FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_APPROVE_STATUS,
        //                        ValueCode = "%"
        //                    }
        //                };
        //            ICommonHandler commonHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //            lst = commonHand.GetMiscTypeCodeList(miscs);
        //        }
        //        catch
        //        {
        //            lst = new List<doMiscTypeCode>();
        //        }
        //        #endregion

        //        ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        List<string> lsFieldNames = new List<string>();
        //        lsFieldNames.Add(MiscType.C_APPROVE_STATUS);
        //        List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

        //        StringBuilder sbApprovalStatus = new StringBuilder("");
        //        foreach (doMiscTypeCode view in MiscTypeList)
        //        {
        //            sbApprovalStatus.AppendFormat("\'{0}\',", view.ValueCode);
        //        }

        //        if (sbApprovalStatus.Length > 0)
        //            cond.OperationOfficeCode = sbApprovalStatus.Remove(sbApprovalStatus.Length - 1, 1).ToString();
        //    }
        //}
        #endregion
    }
}
