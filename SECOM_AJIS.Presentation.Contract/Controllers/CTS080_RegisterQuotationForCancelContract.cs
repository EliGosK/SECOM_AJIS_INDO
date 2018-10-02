//*********************************
// Create by: Jutarat A.
// Create date: 12/Sep/2011
// Update date: 12/Sep/2011
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
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority
        
        /// <summary>
        /// Check system suspending, user’s permission and user’s authority of screen
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ActionResult CTS080_Authority(CTS080_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            string strContractCodeLong = string.Empty;
            dsRentalContractData dsRentalContract = null;
            CTS080_doRentalContractBasicAuthority doRentalContractBasicAuthority;

            //IInstallationHandler installHandler;
            //dtTbt_InstallationBasic dtTbt_InstallationBasicData;
            //dsCancelContractQuotation dsCancelContractQuotationData;

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                /*--- HasAuthority ---*/
                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_PRE_CP14, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //if (String.IsNullOrEmpty(sParam.ContractCode))
                //    sParam.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                if (String.IsNullOrEmpty(sParam.ContractCode) && sParam.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(sParam.CommonSearch.ContractCode) == false)
                        sParam.ContractCode = sParam.CommonSearch.ContractCode;
                }

                //Check required field
                if (String.IsNullOrEmpty(sParam.ContractCode))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                    //                    ScreenID.C_SCREEN_ID_PRE_CP14,
                    //                    MessageUtil.MODULE_COMMON,
                    //                    MessageUtil.MessageList.MSG0007,
                    //                    new string[] { "lblContractCode" },
                    //                    null);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147);

                    return Json(res);
                }
                else
                {
                    //Get rental contract data
                    strContractCodeLong = comUtil.ConvertContractCode(sParam.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    dsRentalContract = CheckDataAuthority_CTS080(res, strContractCodeLong, true);
                    if (res.IsError)
                        return Json(res);

                    //Check data authority
                    doRentalContractBasicAuthority = CommonUtil.CloneObject<tbt_RentalContractBasic, CTS080_doRentalContractBasicAuthority>(dsRentalContract.dtTbt_RentalContractBasic[0]);
                    ValidatorUtil.BuildErrorMessage(res, new object[] { doRentalContractBasicAuthority }, null, false);
                    if (res.IsError)
                        return Json(res);
                }
                /*-------------------------*/

                //Comment by Jutarat A. on 06082012
                //if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL
                //    || dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_END
                //    || dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3105, new string[] { sParam.ContractCode });
                    return Json(res);
                }

                //sParam = new CTS080_ScreenParameter();
                sParam.CTS080_Session = new CTS080_RegisterQuotationTargetData();
                sParam.CTS080_Session.InitialData = new CTS080_InitialRegisterQuotationTargetData();
                sParam.CTS080_Session.InitialData.ContractCode = strContractCodeLong;
                sParam.CTS080_Session.InitialData.OCCCode = dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC;
                sParam.CTS080_Session.InitialData.RentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];
                sParam.CTS080_Session.RegisterRentalContractData = dsRentalContract;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<CTS080_ScreenParameter>("CTS080", sParam, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS080")]
        public ActionResult CTS080()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            IUserControlHandler userCtrlHandler;
            IRentralContractHandler rentralHandler;
            doRentalContractBasicInformation doRentalContractBasic;
            List<tbs_ContractDocTemplate> contractDocTemplateList;
            dsCancelContractQuotation dsCancelContractQuotationData;

            try
            {
                CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                string strContractCodeLong = sParam.CTS080_Session.InitialData.ContractCode;
                string strOCC = sParam.CTS080_Session.InitialData.OCCCode;

                userCtrlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                doRentalContractBasic = userCtrlHandler.GetRentalContactBasicInformationData(strContractCodeLong);

                //Get cancel contract quotation template data
                rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                contractDocTemplateList = rentralHandler.GetTbsContractDocTemplate(DocumentCode.C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION);

                //Get cancel contract quotation data
                rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                dsCancelContractQuotationData = rentralHandler.GetCancelContractQuotation(strContractCodeLong, strOCC);

                //Map data to screen
                Bind_CTS080(doRentalContractBasic);

                sParam.CTS080_Session.InitialData.CancelContractQuotationData = dsCancelContractQuotationData;
                sParam.CTS080_Session.InitialData.doRentalContractBasicData = doRentalContractBasic;
                UpdateScreenObject(sParam);

                //Add by Jutarat A. on 19072012
                if (sParam.CTS080_Session.InitialData.RentalContractBasicData != null)
                {
                    ViewBag.ProductTypeCode = sParam.CTS080_Session.InitialData.RentalContractBasicData.ProductTypeCode;
                    ViewBag.FirstInstallCompleteFlag = sParam.CTS080_Session.InitialData.RentalContractBasicData.FirstInstallCompleteFlag;
                    ViewBag.StartOperationDate = CommonUtil.TextDate(sParam.CTS080_Session.InitialData.RentalContractBasicData.FirstSecurityStartDate);
                }
                else
                {
                    ViewBag.ProductTypeCode = null;
                    ViewBag.FirstInstallCompleteFlag = null;
                }
                //End Add

                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get detail of CancelContract data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS080_GetCancelContractConditionDetail()
        {
            ObjectResultData res = new ObjectResultData();
            dsCancelContractQuotation dsCancelContractQuotationData;
            List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;
            CommonUtil comUtil = new CommonUtil();

            //string lblNormalFee = "Normal fee";
            //string lblContractCodeForSlideFee = "Contract code for slide fee";

            try
            {
                string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_PRE_CP14, "lblNormalFee");
                string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_PRE_CP14, "lblContractCodeForSlideFee");

                CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                dsCancelContractQuotationData = sParam.CTS080_Session.InitialData.CancelContractQuotationData;
                memoDetailTempList = new List<CTS110_CancelContractMemoDetailTemp>();

                int intSequence = -1;
                if (dsCancelContractQuotationData.dtTbt_CancelContractMemoDetail != null)
                {
                    foreach (tbt_CancelContractMemoDetail memoDetail in dsCancelContractQuotationData.dtTbt_CancelContractMemoDetail)
                    {
                        string strStartPeriodDate = memoDetail.StartPeriodDate == null ? string.Empty : CommonUtil.TextDate(memoDetail.StartPeriodDate.Value);
                        string strEndPeriodDate = memoDetail.EndPeriodDate == null ? string.Empty : string.Format("TO {0}", CommonUtil.TextDate(memoDetail.EndPeriodDate.Value));
                        string strContractCodeCounterBal = comUtil.ConvertContractCode(memoDetail.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        CTS110_CancelContractConditionGridData data = new CTS110_CancelContractConditionGridData();
                        data.FeeType = memoDetail.BillingTypeName;
                        data.HandlingType = memoDetail.HandlingTypeName;
                        //data.Fee = CommonUtil.TextNumeric(memoDetail.FeeAmount);
                        //data.Tax = CommonUtil.TextNumeric(memoDetail.TaxAmount);

                        ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        List<doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                        #region Fee Amount

                        if (memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            data.Fee = CommonUtil.TextNumeric(memoDetail.FeeAmountUsd);
                        else
                            data.Fee = CommonUtil.TextNumeric(memoDetail.FeeAmount);

                        if (CommonUtil.IsNullOrEmpty(data.Fee) == false)
                        {
                            DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.FeeAmountCurrencyType);
                            if (curr != null)
                                data.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, data.Fee);
                        }

                        #endregion
                        #region Tax Amount

                        if (memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            data.Tax = CommonUtil.TextNumeric(memoDetail.TaxAmountUsd);
                        else
                            data.Tax = CommonUtil.TextNumeric(memoDetail.TaxAmount);

                        if (CommonUtil.IsNullOrEmpty(data.Tax) == false)
                        {
                            DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.TaxAmountCurrencyType);
                            if (curr != null)
                                data.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, data.Tax);
                        }

                        #endregion

                        data.Period = String.Format("{0} {1}", strStartPeriodDate, strEndPeriodDate);

                        //data.Remark = String.Format("{0} {1} {2}", memoDetail.Remark
                        //    , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, strContractCodeCounterBal)
                        //    , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount)));
                        //data.Remark = String.Format("{0}{1}{2}", string.IsNullOrEmpty(memoDetail.Remark) ? string.Empty : string.Format("{0}<br/>", memoDetail.Remark)
                        //    , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("{0}: {1}<br/>", lblContractCodeForSlideFee, strContractCodeCounterBal)
                        //    , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value)));

                        #region Remark

                        CommonUtil cmm = new CommonUtil();

                        data.Remark = string.Empty;
                        if (string.IsNullOrEmpty(memoDetail.Remark) == false)
                            data.Remark = memoDetail.Remark;
                        if (memoDetail.ContractCode_CounterBalance != null)
                        {
                            if (string.IsNullOrEmpty(data.Remark) == false)
                                data.Remark += "<br/>";

                            data.Remark += string.Format("{0}: {1}", lblContractCodeForSlideFee,
                                cmm.ConvertContractCode(memoDetail.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_SHORT));
                        }
                        if (memoDetail.NormalFeeAmount != null
                            || memoDetail.NormalFeeAmountUsd != null)
                        {
                            if (string.IsNullOrEmpty(data.Remark) == false)
                                data.Remark += "<br/>";

                            string txtcurr = string.Empty;
                            DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.NormalFeeAmountCurrencyType);
                            if (curr != null)
                                txtcurr = curr.ValueDisplayEN;

                            if (memoDetail.NormalFeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                if (memoDetail.NormalFeeAmountUsd != null)
                                    data.Remark += string.Format("{0}: {1} {2}", lblNormalFee, txtcurr, CommonUtil.TextNumeric(memoDetail.NormalFeeAmountUsd.Value));
                            }
                            else
                            {
                                if (memoDetail.NormalFeeAmount != null)
                                    data.Remark += string.Format("{0}: {1} {2}", lblNormalFee, txtcurr, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value));
                            }
                        }

                        #endregion

                        data.FeeTypeCode = memoDetail.BillingType;
                        data.HandlingTypeCode = memoDetail.HandlingType;
                        data.Sequence = (intSequence + 1).ToString();
                        gridDataList.Add(data);

                        CTS110_CancelContractMemoDetailTemp memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_CancelContractMemoDetailTemp>(memoDetail);
                        memoDetailTemp.Sequence = (intSequence + 1).ToString();
                        memoDetailTempList.Add(memoDetailTemp);

                        intSequence++;
                    }
                }

                //Test
                //CTS110_CancelContractConditionGridData dataTest = new CTS110_CancelContractConditionGridData();
                //dataTest.FeeType = "Contract fee";
                //dataTest.HandlingType = "Refund";
                //dataTest.Fee = ((decimal?)9999999.99).Value.ToString("#,##0.00");
                //dataTest.Tax = ((decimal?)9999.99).Value.ToString("#,##0.00");
                //dataTest.Period = String.Format("{0} {1}", DateTime.Now.Date.ToString("dd-MMM-yyyy"), DateTime.Now.Date.AddMonths(1) == null ? string.Empty : string.Format("TO {0}", DateTime.Now.Date.AddMonths(1).ToString("dd-MMM-yyyy")));
                //dataTest.Remark = String.Format("{0} {1} {2}", "xxxxxxxxxx"
                //            , "N1234567" == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, "N1234567")
                //            , (decimal?)9999.99 == null ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, ((decimal?)9999.99).Value.ToString("#,##0.00")));

                //dataTest.FeeTypeCode = ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE;
                //dataTest.HandlingTypeCode = HandlingType.C_HANDLING_TYPE_REFUND;
                //dataTest.Sequence = (intSequence + 1).ToString();
                //gridDataList.Add(dataTest);
                //End Test

                sParam.CTS080_Session.InitialData.CancelContractMemoDetailTempData = memoDetailTempList;
                UpdateScreenObject(sParam);

                res.ResultData = CommonUtil.ConvertToXml<CTS110_CancelContractConditionGridData>(gridDataList, "Contract\\CTS080", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get CancelContract data
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS080_GetCancelContractCondition()
        {
            ObjectResultData res = new ObjectResultData();
            dsCancelContractQuotation dsCancelContractQuotationData;
            List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
            try
            {
                CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                dsCancelContractQuotationData = sParam.CTS080_Session.InitialData.CancelContractQuotationData;

                if (dsCancelContractQuotationData.dtTbt_CancelContractMemo != null && dsCancelContractQuotationData.dtTbt_CancelContractMemo.Count > 0)
                {
                    tbt_CancelContractMemo cancelContractMemoData = CommonUtil.CloneObject<tbt_CancelContractMemo, tbt_CancelContractMemo>(dsCancelContractQuotationData.dtTbt_CancelContractMemo[0]);
                    CTS110_CancelContractConditionGridData memoData = new CTS110_CancelContractConditionGridData();

                    #region Total Slide Amount

                    memoData.TotalSlideAmt = CommonUtil.TextNumeric(cancelContractMemoData.TotalSlideAmt);
                    memoData.TotalSlideAmtUsd = CommonUtil.TextNumeric(cancelContractMemoData.TotalSlideAmtUsd);

                    #endregion
                    #region Total Return Amount

                    memoData.TotalReturnAmt = CommonUtil.TextNumeric(cancelContractMemoData.TotalReturnAmt);
                    memoData.TotalReturnAmtUsd = CommonUtil.TextNumeric(cancelContractMemoData.TotalReturnAmtUsd);

                    #endregion
                    #region Total Billing Amount

                    memoData.TotalBillingAmt = CommonUtil.TextNumeric(cancelContractMemoData.TotalBillingAmt);
                    memoData.TotalBillingAmtUsd = CommonUtil.TextNumeric(cancelContractMemoData.TotalBillingAmtUsd);

                    #endregion
                    #region Total Amount After Counter Balance

                    memoData.TotalAmtAfterCounterBalance = CommonUtil.TextNumeric(cancelContractMemoData.TotalAmtAfterCounterBalance);
                    memoData.TotalAmtAfterCounterBalanceUsd = CommonUtil.TextNumeric(cancelContractMemoData.TotalAmtAfterCounterBalanceUsd);

                    #endregion

                    memoData.ProcessAfterCounterBalanceType = cancelContractMemoData.ProcessAfterCounterBalanceType;
                    memoData.OtherRemarks = cancelContractMemoData.OtherRemarks;

                    #region Total Slide Amount

                    sParam.CTS080_Session.InitialData.TotalSlideAmount = cancelContractMemoData.TotalSlideAmt;
                    sParam.CTS080_Session.InitialData.TotalSlideAmountUsd = cancelContractMemoData.TotalSlideAmtUsd;

                    #endregion
                    #region Total Refund Amount

                    sParam.CTS080_Session.InitialData.TotalRefundAmount = cancelContractMemoData.TotalReturnAmt;
                    sParam.CTS080_Session.InitialData.TotalRefundAmountUsd = cancelContractMemoData.TotalReturnAmtUsd;

                    #endregion
                    #region Total Billing Amount

                    sParam.CTS080_Session.InitialData.TotalBillingAmount = cancelContractMemoData.TotalBillingAmt;
                    sParam.CTS080_Session.InitialData.TotalBillingAmountUsd = cancelContractMemoData.TotalBillingAmtUsd;

                    #endregion
                    #region Total Counter Blanance Amount

                    sParam.CTS080_Session.InitialData.TotalCounterBalAmount = cancelContractMemoData.TotalAmtAfterCounterBalance;
                    sParam.CTS080_Session.InitialData.TotalCounterBalAmountUsd = cancelContractMemoData.TotalAmtAfterCounterBalanceUsd;

                    #endregion

                    UpdateScreenObject(sParam);

                    res.ResultData = memoData;
                }

                //Test
                //CTS110_CancelContractConditionGridData cancelContractMemoData = new CTS110_CancelContractConditionGridData();
                //cancelContractMemoData.TotalSlideAmt = ((decimal?)9999999.99).Value.ToString("#,##0.00");
                //cancelContractMemoData.TotalReturnAmt = ((decimal?)9999999.99).Value.ToString("#,##0.00");
                //cancelContractMemoData.TotalBillingAmt = ((decimal?)9999999.99).Value.ToString("#,##0.00");
                //cancelContractMemoData.TotalAmtAfterCounterBalance = ((decimal?)9999999.99).Value.ToString("#,##0.00");
                //cancelContractMemoData.ProcessAfterCounterBalanceType = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE;
                //cancelContractMemoData.OtherRemarks = "ttttttttttt";
                //res.ResultData = cancelContractMemoData;
                //End Test
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate data when Add CancelContract
        /// </summary>
        /// <param name="res"></param>
        /// <param name="cancelContractMemoDetailData"></param>
        public ActionResult CTS080_ValidateAddCancelContractData(tbt_CancelContractMemoDetail cancelContractMemoDetailData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (cancelContractMemoDetailData != null)
                {
                    //Check mandatory
                    object memoDetailTemp = null;
                    if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
                        || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                    {
                        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateContractMaintenanceFee>(cancelContractMemoDetailData);
                    }
                    else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE
                                || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE
                                || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
                    {
                        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateDepositCardOtherFee>(cancelContractMemoDetailData);
                    }
                    else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
                    {
                        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateRemovalFee>(cancelContractMemoDetailData);
                    }
                    else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                    {
                        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateChangeFee>(cancelContractMemoDetailData);
                    }
                    else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
                    {
                        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateCancelFee>(cancelContractMemoDetailData);
                    }
                    else
                    {
                        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateFeeTypeHandlingType>(cancelContractMemoDetailData);
                    }

                    ValidatorUtil.BuildErrorMessage(res, new object[] { memoDetailTemp });
                    if (res.IsError)
                        return Json(res);

                    //ValidateBusiness
                    ValidateBusinessCancelContract_CTS080(res, cancelContractMemoDetailData);
                    if (res.IsError)
                        return Json(res);

                    //ValidateBusinessForWarning
                    ValidateBusinessCancelContractForWarning_CTS080(res, cancelContractMemoDetailData);

                    res.ResultData = true;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Add Fee to grid when click [Add] button on ‘Cancel contract condition’ section
        /// </summary>
        /// <param name="cancelContractMemoDetailData"></param>
        /// <returns></returns>
        public ActionResult CTS080_AddCancelContractData(tbt_CancelContractMemoDetail cancelContractMemoDetailData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;
            CommonUtil comUtil = new CommonUtil();

            //string lblNormalFee = "Normal fee";
            //string lblContractCodeForSlideFee = "Contract code for slide fee";
            string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_PRE_CP14, "lblNormalFee");
            string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_PRE_CP14, "lblContractCodeForSlideFee");

            try
            {
                if (cancelContractMemoDetailData != null)
                {
                    #region Move to CTS080_ValidateAddCancelContractData()
                    ////Check mandatory
                    //object memoDetailTemp = null;
                    //if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
                    //    || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                    //{
                    //    memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateContractMaintenanceFee>(cancelContractMemoDetailData);
                    //}
                    //else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE
                    //            || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE
                    //            || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
                    //{
                    //    memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateDepositCardOtherFee>(cancelContractMemoDetailData);
                    //}
                    //else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
                    //{
                    //    memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateRemovalFee>(cancelContractMemoDetailData);
                    //}
                    //else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                    //{
                    //    memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateChangeFee>(cancelContractMemoDetailData);
                    //}
                    //else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
                    //{
                    //    memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateCancelFee>(cancelContractMemoDetailData);
                    //}
                    //else
                    //{
                    //    memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateFeeTypeHandlingType>(cancelContractMemoDetailData);
                    //}

                    //ValidatorUtil.BuildErrorMessage(res, new object[] { memoDetailTemp });
                    //if (res.IsError)
                    //    return Json(res);

                    ////ValidateBusiness
                    //ValidateBusinessCancelContract_CTS080(res, cancelContractMemoDetailData);
                    //if (res.IsError)
                    //    return Json(res);

                    ////ValidateBusinessForWarning
                    //ValidateBusinessCancelContractForWarning_CTS080(res, cancelContractMemoDetailData);
                    #endregion

                    //Add fee information to cancel contract condition list
                    tbt_CancelContractMemoDetail memoDetail = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, tbt_CancelContractMemoDetail>(cancelContractMemoDetailData);
                    
                    #region Fee Amount

                    if (memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        memoDetail.FeeAmountUsd = memoDetail.FeeAmount;
                        memoDetail.FeeAmount = null;
                    }
                    else
                    {
                        memoDetail.FeeAmountUsd = null;
                    }

                    #endregion
                    #region Tax Amount

                    if (memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        memoDetail.TaxAmountUsd = memoDetail.TaxAmount;
                        memoDetail.TaxAmount = null;
                    }
                    else
                    {
                        memoDetail.TaxAmountUsd = null;
                    }

                    #endregion
                    #region Normal Fee Amount

                    if (memoDetail.NormalFeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        memoDetail.NormalFeeAmountUsd = memoDetail.NormalFeeAmount;
                        memoDetail.NormalFeeAmount = null;
                    }
                    else
                    {
                        memoDetail.NormalFeeAmountUsd = null;
                    }

                    #endregion

                    ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    MiscTypeMappingList miscList = new MiscTypeMappingList();
                    miscList.AddMiscType(memoDetail);
                    comHandler.MiscTypeMappingList(miscList);

                    CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                    memoDetailTempList = sParam.CTS080_Session.InitialData.CancelContractMemoDetailTempData;

                    string strSequence = "0";
                    if (memoDetailTempList.Count == 0)
                        strSequence = "0";
                    else
                        strSequence = (int.Parse(memoDetailTempList.Max(t => t.Sequence)[0].ToString()) + 1).ToString();

                    string strStartDate = memoDetail.StartPeriodDate == null ? string.Empty : CommonUtil.TextDate(memoDetail.StartPeriodDate.Value);
                    string strEndDate = memoDetail.EndPeriodDate == null ? string.Empty : CommonUtil.TextDate(memoDetail.EndPeriodDate.Value);

                    CTS110_CancelContractConditionGridData gridData = new CTS110_CancelContractConditionGridData();
                    gridData.FeeType = memoDetail.BillingTypeName;
                    gridData.HandlingType = memoDetail.HandlingTypeName;
                    
                    ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                    #region Fee Amount

                    if (memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        gridData.Fee = CommonUtil.TextNumeric(memoDetail.FeeAmountUsd);
                    else
                        gridData.Fee = CommonUtil.TextNumeric(memoDetail.FeeAmount);

                    if (CommonUtil.IsNullOrEmpty(gridData.Fee) == false)
                    {
                        DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.FeeAmountCurrencyType);
                        if (curr != null)
                            gridData.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, gridData.Fee);
                    }

                    #endregion
                    #region Tax Amount

                    if (memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        gridData.Tax = CommonUtil.TextNumeric(memoDetail.TaxAmountUsd);
                    else
                        gridData.Tax = CommonUtil.TextNumeric(memoDetail.TaxAmount);

                    if (CommonUtil.IsNullOrEmpty(gridData.Tax) == false)
                    {
                        DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.TaxAmountCurrencyType);
                        if (curr != null)
                            gridData.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, gridData.Tax);
                    }

                    #endregion

                    gridData.Period = String.Format("{0} {1}", strStartDate, memoDetail.EndPeriodDate == null ? string.Empty : string.Format("TO {0}", strEndDate));

                    //gridData.Remark = String.Format("{0} {1} {2}", memoDetail.Remark
                    //                , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                    //                , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount)));

                    //gridData.Remark = String.Format("{0}{1}{2}", 
                    //    string.IsNullOrEmpty(memoDetail.Remark) ? string.Empty : string.Format("{0}<br/>", memoDetail.Remark)
                    //    , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("{0}: {1}<br/>", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                    //    , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value)));

                    #region Remark

                    CommonUtil cmm = new CommonUtil();

                    gridData.Remark = string.Empty;
                    if (string.IsNullOrEmpty(memoDetail.Remark) == false)
                        gridData.Remark = memoDetail.Remark;
                    if (memoDetail.ContractCode_CounterBalance != null)
                    {
                        if (string.IsNullOrEmpty(gridData.Remark) == false)
                            gridData.Remark += "<br/>";

                        gridData.Remark += string.Format("{0}: {1}", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance);
                    }
                    if (memoDetail.NormalFeeAmount != null
                        || memoDetail.NormalFeeAmountUsd != null)
                    {
                        if (string.IsNullOrEmpty(gridData.Remark) == false)
                            gridData.Remark += "<br/>";

                        string txtcurr = string.Empty;
                        DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.NormalFeeAmountCurrencyType);
                        if (curr != null)
                            txtcurr = curr.ValueDisplayEN;

                        if (memoDetail.NormalFeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            if (memoDetail.NormalFeeAmountUsd != null)
                                gridData.Remark += string.Format("{0}: {1} {2}", lblNormalFee, txtcurr, CommonUtil.TextNumeric(memoDetail.NormalFeeAmountUsd.Value));
                        }
                        else
                        {
                            if (memoDetail.NormalFeeAmount != null)
                                gridData.Remark += string.Format("{0}: {1} {2}", lblNormalFee, txtcurr, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value));
                        }
                    }

                    #endregion
                    
                    gridData.FeeTypeCode = memoDetail.BillingType;
                    gridData.HandlingTypeCode = memoDetail.HandlingType;
                    gridData.Sequence = strSequence;
                    
                    CTS110_CancelContractMemoDetailTemp cacelMemoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_CancelContractMemoDetailTemp>(memoDetail);
                    cacelMemoDetailTemp.ContractCode_CounterBalance = comUtil.ConvertContractCode(cacelMemoDetailTemp.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_LONG);
                    cacelMemoDetailTemp.Sequence = strSequence;

                    memoDetailTempList.Add(cacelMemoDetailTemp);

                    sParam.CTS080_Session.InitialData.CancelContractMemoDetailTempData = memoDetailTempList;
                    UpdateScreenObject(sParam);

                    res.ResultData = gridData;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Calculate total amount of Fee
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS080_CalculateTotalAmount()
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            try
            {
                CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                memoDetailTempList = sParam.CTS080_Session.InitialData.CancelContractMemoDetailTempData;

                decimal decSlideAmount = 0;
                decimal decSlideAmountUsd = 0;
                decimal decRefundAmount = 0;
                decimal decRefundAmountUsd = 0;
                decimal decBillingAmount = 0;
                decimal decBillingAmountUsd = 0;
                decimal decCounterBalAmount = 0;
                decimal decCounterBalAmountUsd = 0;

                decimal decFeeAmount = 0;
                decimal decFeeAmountUsd = 0;
                decimal decTaxAmount = 0;
                decimal decTaxAmountUsd = 0;
                decimal decTotalAmount = 0;
                decimal decTotalAmountUsd = 0;
                foreach (CTS110_CancelContractMemoDetailTemp memoDetail in memoDetailTempList)
                {
                    #region Fee Amount

                    decFeeAmount = memoDetail.FeeAmount == null ? 0 : memoDetail.FeeAmount.Value;
                    decFeeAmountUsd = memoDetail.FeeAmountUsd == null ? 0 : memoDetail.FeeAmountUsd.Value;

                    #endregion
                    #region Tax Amount

                    decTaxAmount = memoDetail.TaxAmount == null ? 0 : memoDetail.TaxAmount.Value;
                    decTaxAmountUsd = memoDetail.TaxAmountUsd == null ? 0 : memoDetail.TaxAmountUsd.Value;

                    #endregion
                    #region Total Amount

                    decTotalAmount = (decFeeAmount + decTaxAmount);
                    decTotalAmountUsd = (decFeeAmountUsd + decTaxAmountUsd);

                    #endregion

                    if (memoDetail.HandlingType == HandlingType.C_HANDLING_TYPE_SLIDE)
                    {
                        decSlideAmount += decTotalAmount;
                        decSlideAmountUsd += decTotalAmountUsd;
                    }
                    else if (memoDetail.HandlingType == HandlingType.C_HANDLING_TYPE_REFUND)
                    {
                        decRefundAmount += decTotalAmount;
                        decRefundAmountUsd += decTotalAmountUsd;
                    }
                    else if (memoDetail.HandlingType == HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE)
                    {
                        decBillingAmount += decTotalAmount;
                        decBillingAmountUsd += decTotalAmountUsd;
                    }
                }

                #region Counter Balance Amount

                decCounterBalAmount = decRefundAmount - decBillingAmount;
                decCounterBalAmountUsd = decRefundAmountUsd - decBillingAmountUsd;

                #endregion
                #region Total Slide Amount

                sParam.CTS080_Session.InitialData.TotalSlideAmount = decSlideAmount;
                sParam.CTS080_Session.InitialData.TotalSlideAmountUsd = decSlideAmountUsd;

                #endregion
                #region Total Refund Amount

                sParam.CTS080_Session.InitialData.TotalRefundAmount = decRefundAmount;
                sParam.CTS080_Session.InitialData.TotalRefundAmountUsd = decRefundAmountUsd;

                #endregion
                #region Total Billing Amount

                sParam.CTS080_Session.InitialData.TotalBillingAmount = decBillingAmount;
                sParam.CTS080_Session.InitialData.TotalBillingAmountUsd = decBillingAmountUsd;

                #endregion
                #region Total Counter Blanace Amount

                sParam.CTS080_Session.InitialData.TotalCounterBalAmount = decCounterBalAmount;
                sParam.CTS080_Session.InitialData.TotalCounterBalAmountUsd = decCounterBalAmountUsd;

                #endregion

                UpdateScreenObject(sParam);

                res.ResultData = new object[] {
                    CommonUtil.TextNumeric(decSlideAmount),
                    CommonUtil.TextNumeric(decRefundAmount),
                    CommonUtil.TextNumeric(decBillingAmount),
                    CommonUtil.TextNumeric(decCounterBalAmount),

                    CommonUtil.TextNumeric(decSlideAmountUsd),
                    CommonUtil.TextNumeric(decRefundAmountUsd),
                    CommonUtil.TextNumeric(decBillingAmountUsd),
                    CommonUtil.TextNumeric(decCounterBalAmountUsd)
                };

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Remove Fee from grid when click [Remove] button on grid
        /// </summary>
        /// <param name="strSequence"></param>
        /// <returns></returns>
        public ActionResult CTS080_RemoveDataCancelCond(string strSequence)
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            try
            {
                CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                memoDetailTempList = sParam.CTS080_Session.InitialData.CancelContractMemoDetailTempData;

                foreach (CTS110_CancelContractMemoDetailTemp memoDetail in memoDetailTempList)
                {
                    if (memoDetail.Sequence == strSequence)
                    {
                        memoDetailTempList.Remove(memoDetail);
                        break;
                    }
                }

                sParam.CTS080_Session.InitialData.CancelContractMemoDetailTempData = memoDetailTempList;
                UpdateScreenObject(sParam);

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Reload detail of CancelContract data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS080_RefreshCancelContractConditionDetail()
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblNormalFee");
            string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblContractCodeForSlideFee");

            try
            {
                CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                memoDetailTempList = sParam.CTS080_Session.InitialData.CancelContractMemoDetailTempData;

                if (memoDetailTempList != null)
                {
                    foreach (CTS110_CancelContractMemoDetailTemp memoDetail in memoDetailTempList)
                    {
                        string strStartPeriodDate = memoDetail.StartPeriodDate == null ? string.Empty : CommonUtil.TextDate(memoDetail.StartPeriodDate.Value);
                        string strEndPeriodDate = memoDetail.EndPeriodDate == null ? string.Empty : string.Format("TO {0}", CommonUtil.TextDate(memoDetail.EndPeriodDate.Value));

                        CTS110_CancelContractConditionGridData data = new CTS110_CancelContractConditionGridData();
                        data.FeeType = memoDetail.BillingTypeName;
                        data.HandlingType = memoDetail.HandlingTypeName;
                        //data.Fee = CommonUtil.TextNumeric(memoDetail.FeeAmount);
                        //data.Tax = CommonUtil.TextNumeric(memoDetail.TaxAmount);

                        ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        List<doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                        #region Fee Amount

                        if (memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            data.Fee = CommonUtil.TextNumeric(memoDetail.FeeAmountUsd);
                        else
                            data.Fee = CommonUtil.TextNumeric(memoDetail.FeeAmount);

                        if (CommonUtil.IsNullOrEmpty(data.Fee) == false)
                        {
                            DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.FeeAmountCurrencyType);
                            if (curr != null)
                                data.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, data.Fee);
                        }

                        #endregion
                        #region Tax Amount

                        if (memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            data.Tax = CommonUtil.TextNumeric(memoDetail.TaxAmountUsd);
                        else
                            data.Tax = CommonUtil.TextNumeric(memoDetail.TaxAmount);

                        if (CommonUtil.IsNullOrEmpty(data.Tax) == false)
                        {
                            DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.TaxAmountCurrencyType);
                            if (curr != null)
                                data.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, data.Tax);
                        }

                        #endregion
                        
                        data.Period = String.Format("{0} {1}", strStartPeriodDate, strEndPeriodDate);

                        //data.Remark = String.Format("{0} {1} {2}", memoDetail.Remark
                        //    , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                        //    , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount)));
                        //data.Remark = String.Format("{0}{1}{2}", string.IsNullOrEmpty(memoDetail.Remark) ? string.Empty : string.Format("{0}<br/>", memoDetail.Remark)
                        //    , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("{0}: {1}<br/>", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                        //    , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value)));

                        #region Remark

                        CommonUtil cmm = new CommonUtil();

                        data.Remark = string.Empty;
                        if (string.IsNullOrEmpty(memoDetail.Remark) == false)
                            data.Remark = memoDetail.Remark;
                        if (memoDetail.ContractCode_CounterBalance != null)
                        {
                            if (string.IsNullOrEmpty(data.Remark) == false)
                                data.Remark += "<br/>";

                            data.Remark += string.Format("{0}: {1}", lblContractCodeForSlideFee,
                                cmm.ConvertContractCode(memoDetail.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_SHORT));
                        }
                        if (memoDetail.NormalFeeAmount != null
                            || memoDetail.NormalFeeAmountUsd != null)
                        {
                            if (string.IsNullOrEmpty(data.Remark) == false)
                                data.Remark += "<br/>";

                            string txtcurr = string.Empty;
                            DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.NormalFeeAmountCurrencyType);
                            if (curr != null)
                                txtcurr = curr.ValueDisplayEN;

                            if (memoDetail.NormalFeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                if (memoDetail.NormalFeeAmountUsd != null)
                                    data.Remark += string.Format("{0}: {1} {2}", lblNormalFee, txtcurr, CommonUtil.TextNumeric(memoDetail.NormalFeeAmountUsd.Value));
                            }
                            else
                            {
                                if (memoDetail.NormalFeeAmount != null)
                                    data.Remark += string.Format("{0}: {1} {2}", lblNormalFee, txtcurr, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value));
                            }
                            
                        }

                        #endregion

                        data.FeeTypeCode = memoDetail.BillingType;
                        data.HandlingTypeCode = memoDetail.HandlingType;
                        data.Sequence = memoDetail.Sequence;
                        gridDataList.Add(data);
                    }
                }

                res.ResultData = CommonUtil.ConvertToXml<CTS110_CancelContractConditionGridData>(gridDataList, "Contract\\CTS080", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate business when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS080_RegisterCancelContractData()
        {
            ObjectResultData res = new ObjectResultData();
            tbt_RentalContractBasic dtRentalContractBasicData;
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempDataList;

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_PRE_CP14, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                dtRentalContractBasicData = sParam.CTS080_Session.InitialData.RentalContractBasicData;
                memoDetailTempDataList = sParam.CTS080_Session.InitialData.CancelContractMemoDetailTempData;

                //ValidateBusiness
                ValidateBusiness_CTS080(res, sParam.CTS080_Session.InitialData.ContractCode);
                if (res.IsError)
                    return Json(res);

                //ValidateBusinessForWarning
                //res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                if (memoDetailTempDataList == null || memoDetailTempDataList.Count < 1)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3113);
                    //return Json(res);
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
        /// Update data to database when click [Confirm] button in ‘Action button’ section
        /// </summary>
        /// <param name="registerQuotationData"></param>
        /// <returns></returns>
        public ActionResult CTS080_ConfirmRegisterCancelData(CTS080_RegisterQuotationData registerQuotationData)
        {
            ObjectResultData res = new ObjectResultData();
            IRentralContractHandler rentralHandler;
            IContractDocumentHandler docHandler;

            CommonUtil comUtil = new CommonUtil();
            tbt_RentalContractBasic dtRentalContractBasicData;
            dsCancelContractQuotation dsCancelContractQuotationData = null;
            dsContractDocData dsContractDocDataData = null;
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempDataList;

            MessageModel msgModelResult;
            string strDocNoResult = string.Empty;

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_PRE_CP14, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                dtRentalContractBasicData = sParam.CTS080_Session.InitialData.RentalContractBasicData;
                memoDetailTempDataList = sParam.CTS080_Session.InitialData.CancelContractMemoDetailTempData;
                string strContractCode = sParam.CTS080_Session.InitialData.ContractCode;
                string strOCC = sParam.CTS080_Session.InitialData.OCCCode;
                doRentalContractBasicInformation doRentalContractBasic = sParam.CTS080_Session.InitialData.doRentalContractBasicData;

                //ValidateBusiness
                ValidateBusiness_CTS080(res, sParam.CTS080_Session.InitialData.ContractCode);
                if (res.IsError)
                    return Json(res);

                using (TransactionScope scope = new TransactionScope())
                {
                    /*------- Save Pre CP-14 data -------*/
                    //MapCancelContractQuotationData
                    dsCancelContractQuotationData = new dsCancelContractQuotation();
                    dsCancelContractQuotationData.dtTbt_CancelContractMemo = new List<tbt_CancelContractMemo>();
                    tbt_CancelContractMemo dtTbt_CancelContractMemo = new tbt_CancelContractMemo();

                    if (sParam.CTS080_Session.InitialData.CancelContractQuotationData != null
                        && sParam.CTS080_Session.InitialData.CancelContractQuotationData.dtTbt_CancelContractMemo != null
                        && sParam.CTS080_Session.InitialData.CancelContractQuotationData.dtTbt_CancelContractMemo.Count > 0)
                    {
                        dtTbt_CancelContractMemo = CommonUtil.CloneObject<tbt_CancelContractMemo, tbt_CancelContractMemo>(sParam.CTS080_Session.InitialData.CancelContractQuotationData.dtTbt_CancelContractMemo[0]);
                    }
                    dsCancelContractQuotationData.dtTbt_CancelContractMemo.Add(dtTbt_CancelContractMemo);

                    //doTbt_CancelContractMemo
                    if (dsCancelContractQuotationData.dtTbt_CancelContractMemo.Count > 0)
                    {
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].ContractCode = strContractCode;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].OCC = strOCC;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].QuotationFlag = FlagType.C_FLAG_ON;

                        #region Total Return Amount

                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalReturnAmt = sParam.CTS080_Session.InitialData.TotalRefundAmount;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalReturnAmtUsd = sParam.CTS080_Session.InitialData.TotalRefundAmountUsd;

                        #endregion
                        #region Total Billing Amount

                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalBillingAmt = sParam.CTS080_Session.InitialData.TotalBillingAmount;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalBillingAmtUsd = sParam.CTS080_Session.InitialData.TotalBillingAmountUsd;

                        #endregion
                        #region Total Slide Amount

                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalSlideAmt = sParam.CTS080_Session.InitialData.TotalSlideAmount;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalSlideAmtUsd = sParam.CTS080_Session.InitialData.TotalSlideAmountUsd;

                        #endregion
                        #region Total Amount After Counter Balance

                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalAmtAfterCounterBalance = sParam.CTS080_Session.InitialData.TotalCounterBalAmount;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalAmtAfterCounterBalanceUsd = sParam.CTS080_Session.InitialData.TotalCounterBalAmountUsd;

                        #endregion

                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType = null;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].AutoTransferBillingType = null;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].AutoTransferBillingAmt = 0;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].BankTransferBillingType = null;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].BankTransferBillingAmt = 0;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].CustomerSignatureName = registerQuotationData.EmpName;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].OtherRemarks = registerQuotationData.OtherRemarks;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    }

                    //dtTbt_CancelContractMemoDetail
                    List<tbt_CancelContractMemoDetail> cancelContractMemoDetailList = new List<tbt_CancelContractMemoDetail>();
                    if (memoDetailTempDataList != null)
                    {
                        int intSequenceNo = 1;
                        foreach (CTS110_CancelContractMemoDetailTemp memoDetailTemp in memoDetailTempDataList)
                        {
                            tbt_CancelContractMemoDetail memoDetail = new tbt_CancelContractMemoDetail();
                            memoDetail.ContractCode = strContractCode;
                            memoDetail.OCC = strOCC;
                            memoDetail.SequenceNo = intSequenceNo;
                            memoDetail.BillingType = memoDetailTemp.BillingType;
                            memoDetail.HandlingType = memoDetailTemp.HandlingType;
                            memoDetail.StartPeriodDate = memoDetailTemp.StartPeriodDate;
                            memoDetail.EndPeriodDate = memoDetailTemp.EndPeriodDate;

                            #region Normal Fee Amount

                            memoDetail.NormalFeeAmountCurrencyType = memoDetailTemp.NormalFeeAmountCurrencyType;
                            memoDetail.NormalFeeAmount = memoDetailTemp.NormalFeeAmount;
                            memoDetail.NormalFeeAmountUsd = memoDetailTemp.NormalFeeAmountUsd;

                            #endregion
                            #region Fee Amount

                            memoDetail.FeeAmountCurrencyType = memoDetailTemp.FeeAmountCurrencyType;
                            memoDetail.FeeAmount = memoDetailTemp.FeeAmount;
                            memoDetail.FeeAmountUsd = memoDetailTemp.FeeAmountUsd;

                            #endregion
                            #region Tax Amount

                            memoDetail.TaxAmountCurrencyType = memoDetailTemp.TaxAmountCurrencyType;
                            memoDetail.TaxAmount = memoDetailTemp.TaxAmount;
                            memoDetail.TaxAmountUsd = memoDetailTemp.TaxAmountUsd;

                            #endregion

                            memoDetail.ContractCode_CounterBalance = memoDetailTemp.ContractCode_CounterBalance;
                            memoDetail.Remark = memoDetailTemp.Remark;
                            memoDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            memoDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            memoDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            memoDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            cancelContractMemoDetailList.Add(memoDetail);

                            intSequenceNo++;
                        }
                    }

                    dsCancelContractQuotationData.dtTbt_CancelContractMemoDetail = cancelContractMemoDetailList;

                    //Save cancel contract memo data
                    rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    bool blnCreateSuccess = rentralHandler.DeleteCancelContractMemo(strContractCode, strOCC);
                    dsCancelContractQuotation dsCancelContractResult = rentralHandler.CreateCancelContractMemo(dsCancelContractQuotationData);
                    /*-----------------------------------*/

                    //Generate contract document occurrence
                    docHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
                    string strContractDocOCC = docHandler.GenerateDocOCC(strContractCode, strOCC);

                    /*----- Save contract document data -----*/
                    //MapContractDocumentData
                    dsContractDocDataData = new dsContractDocData();
                    dsContractDocDataData.dtTbt_ContractDocument = new List<tbt_ContractDocument>();
                    dsContractDocDataData.dtTbt_DocCancelContractMemo = new List<tbt_DocCancelContractMemo>();
                    dsContractDocDataData.dtTbt_DocCancelContractMemoDetail = new List<tbt_DocCancelContractMemoDetail>();

                    //dtTbt_ContractDocument
                    tbt_ContractDocument contractDoc = new tbt_ContractDocument();
                    //contractDoc.DocID => Auto-run
                    contractDoc.QuotationTargetCode = null;
                    contractDoc.Alphabet = null;
                    contractDoc.ContractCode = strContractCode;
                    contractDoc.OCC = strOCC;
                    contractDoc.ContractDocOCC = strContractDocOCC;
                    contractDoc.DocNo = strContractCode + "-" + strOCC + "-" + strContractDocOCC;
                    contractDoc.DocumentCode = DocumentCode.C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION;
                    contractDoc.SECOMSignatureFlag = registerQuotationData.SECOMSignatureFlag;
                    contractDoc.EmpName = registerQuotationData.EmpName;
                    contractDoc.EmpPosition = registerQuotationData.EmpPosition;
                    contractDoc.ContractTargetCustCode = dtRentalContractBasicData.ContractTargetCustCode;
                    contractDoc.ContractTargetNameLC = doRentalContractBasic.ContractTargetNameLC; //ContractTargetNameLC;	
                    contractDoc.ContractTargetNameEN = doRentalContractBasic.ContractTargetNameEN; //ContractTargetNameEN;	
                    contractDoc.ContractTargetAddressLC = doRentalContractBasic.ContractTargetAddressLC; //ContractTargetAddressLC;
                    contractDoc.ContractTargetAddressEN = doRentalContractBasic.ContractTargetAddressEN; //ContractTargetAddressEN;
                    contractDoc.RealCustomerCustCode = doRentalContractBasic.RealCustomerCustCode;
                    contractDoc.RealCustomerNameLC = doRentalContractBasic.RealCustomerNameLC;
                    contractDoc.RealCustomerNameEN = doRentalContractBasic.RealCustomerNameEN;
                    contractDoc.SiteNameLC = doRentalContractBasic.SiteNameLC;
                    contractDoc.SiteNameEN = doRentalContractBasic.SiteNameEN;
                    contractDoc.SiteAddressLC = doRentalContractBasic.SiteAddressLC;
                    contractDoc.SiteAddressEN = doRentalContractBasic.SiteAddressEN;
                    contractDoc.ContractOfficeCode = dtRentalContractBasicData.ContractOfficeCode;
                    contractDoc.OperationOfficeCode = dtRentalContractBasicData.OperationOfficeCode;

                    string NegotiationStaffEmpNo = sParam.CTS080_Session.RegisterRentalContractData.dtTbt_RentalSecurityBasic != null
                                                    && sParam.CTS080_Session.RegisterRentalContractData.dtTbt_RentalSecurityBasic.Count > 0 ?
                                                    sParam.CTS080_Session.RegisterRentalContractData.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1 : null;

                    contractDoc.NegotiationStaffEmpNo = NegotiationStaffEmpNo;
                    contractDoc.IssuedDate = null;
                    contractDoc.DocEditFlag = null;
                    contractDoc.DocEditor = null;
                    contractDoc.DocEditDate = null;
                    contractDoc.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_NOT_ISSUED;
                    contractDoc.DocAuditResult = DocAuditResult.C_DOC_AUDIT_RESULT_NO_NEED_TO_RECEIVE;
                    contractDoc.CollectDocDate = null;
                    contractDoc.SubjectEN = null;
                    contractDoc.SubjectLC = null;
                    contractDoc.RelatedNo1 = null;
                    contractDoc.RelatedNo2 = null;
                    contractDoc.AttachDoc1 = null;
                    contractDoc.AttachDoc2 = null;
                    contractDoc.AttachDoc3 = null;
                    contractDoc.AttachDoc4 = null;
                    contractDoc.AttachDoc5 = null;
                    contractDoc.ApproveNo1 = null;
                    contractDoc.ApproveNo2 = null;
                    contractDoc.ApproveNo3 = null;
                    contractDoc.ContactMemo = null;
                    contractDoc.QuotationFee = dtRentalContractBasicData.LastNormalContractFee;
                    contractDoc.CreateOfficeCode = CommonUtil.dsTransData.dtUserBelongingData[0].OfficeCode;
                    contractDoc.ProductCode = null;
                    contractDoc.PhoneLineTypeCode = null;
                    contractDoc.PhoneLineOwnerTypeCode = null;
                    contractDoc.ContractFee = dtRentalContractBasicData.LastOrderContractFee;
                    contractDoc.DepositFee = dtRentalContractBasicData.OrderDepositFee;
                    contractDoc.ContractFeePayMethod = null;
                    contractDoc.CreditTerm = null;
                    contractDoc.PaymentCycle = null;
                    contractDoc.FireSecurityFlag = null;
                    contractDoc.CrimePreventFlag = null;
                    contractDoc.EmergencyReportFlag = null;
                    contractDoc.BusinessTypeCode = null;
                    contractDoc.BuildingUsageCode = null;
                    contractDoc.ContractDurationMonth = null;
                    contractDoc.OldContractCode = dtRentalContractBasicData.OldContractCode;
                    contractDoc.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    contractDoc.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    contractDoc.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    contractDoc.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    dsContractDocDataData.dtTbt_ContractDocument.Add(contractDoc);

                    //dtTbt_DocCancelContractMemo
                    tbt_DocCancelContractMemo docCancelContractMemo = new tbt_DocCancelContractMemo();
                    docCancelContractMemo.DocID = contractDoc.DocID;
                    docCancelContractMemo.CancelContractDate = registerQuotationData.CancelContractDate;
                    docCancelContractMemo.StartServiceDate = registerQuotationData.StartServiceDate;

                    #region Total Slide Amount

                    docCancelContractMemo.TotalSlideAmt = sParam.CTS080_Session.InitialData.TotalSlideAmount;
                    docCancelContractMemo.TotalSlideAmtUsd = sParam.CTS080_Session.InitialData.TotalSlideAmountUsd;

                    #endregion
                    #region Total Return Amount

                    docCancelContractMemo.TotalReturnAmt = sParam.CTS080_Session.InitialData.TotalRefundAmount;
                    docCancelContractMemo.TotalReturnAmtUsd = sParam.CTS080_Session.InitialData.TotalRefundAmountUsd;

                    #endregion
                    #region Total Billing Amount

                    docCancelContractMemo.TotalBillingAmt = sParam.CTS080_Session.InitialData.TotalBillingAmount;
                    docCancelContractMemo.TotalBillingAmtUsd = sParam.CTS080_Session.InitialData.TotalBillingAmountUsd;

                    #endregion
                    #region Total Amount After Counter Balance

                    docCancelContractMemo.TotalAmtAfterCounterBalance = sParam.CTS080_Session.InitialData.TotalCounterBalAmount;
                    docCancelContractMemo.TotalAmtAfterCounterBalanceUsd = sParam.CTS080_Session.InitialData.TotalCounterBalAmountUsd;

                    #endregion

                    docCancelContractMemo.ProcessAfterCounterBalanceType = null;
                    docCancelContractMemo.ProcessAfterCounterBalanceTypeName = null;
                    docCancelContractMemo.AutoTransferBillingType = null;
                    docCancelContractMemo.AutoTransferBillingAmt = 0;
                    docCancelContractMemo.BankTransferBillingType = null;
                    docCancelContractMemo.BankTransferBillingAmt = 0;
                    docCancelContractMemo.CustomerSignatureName = null;
                    docCancelContractMemo.OtherRemarks = registerQuotationData.OtherRemarks;
                    docCancelContractMemo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    docCancelContractMemo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    docCancelContractMemo.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    docCancelContractMemo.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    dsContractDocDataData.dtTbt_DocCancelContractMemo.Add(docCancelContractMemo);

                    //dtTbt_DocCancelContractMemoDetail
                    List<tbt_DocCancelContractMemoDetail> docCancelContractMemoDetailList = new List<tbt_DocCancelContractMemoDetail>();
                    if (memoDetailTempDataList != null)
                    {
                        List<string> strMiscList = new List<string>();
                        strMiscList.Add(MiscType.C_CONTRACT_BILLING_TYPE);
                        strMiscList.Add(MiscType.C_HANDLING_TYPE);

                        ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        List<doMiscTypeCode> miscTypeList = comHandler.GetMiscTypeCodeListByFieldName(strMiscList);

                        List<doMiscTypeCode> miscBillingType = new List<doMiscTypeCode>();
                        List<doMiscTypeCode> miscHandlingType = new List<doMiscTypeCode>();
                        foreach (CTS110_CancelContractMemoDetailTemp memoDetailTemp in memoDetailTempDataList)
                        {
                            tbt_DocCancelContractMemoDetail docMemoDetail = new tbt_DocCancelContractMemoDetail();
                            //docMemoDetail.DocCancelMemoID	=> Auto-run
                            docMemoDetail.DocID = contractDoc.DocID;


                            //docMemoDetail.BillingType = memoDetailTemp.BillingType;
                            //docMemoDetail.BillingTypeName = memoDetailTemp.BillingTypeName;
                            //docMemoDetail.HandlingType = memoDetailTemp.HandlingType;
                            //docMemoDetail.HandlingTypeName = memoDetailTemp.HandlingTypeName;
                            miscBillingType = (from t in miscTypeList
                                               where t.FieldName == MiscType.C_CONTRACT_BILLING_TYPE
                                               && t.ValueCode == memoDetailTemp.BillingType
                                               select t).ToList<doMiscTypeCode>();

                            docMemoDetail.BillingType = memoDetailTemp.BillingType;
                            if (miscBillingType != null && miscBillingType.Count > 0)
                                docMemoDetail.BillingTypeName = miscBillingType[0].ValueDisplayLC;
                            else
                                docMemoDetail.BillingTypeName = null;


                            miscHandlingType = (from t in miscTypeList
                                                where t.FieldName == MiscType.C_HANDLING_TYPE
                                                && t.ValueCode == memoDetailTemp.HandlingType
                                                select t).ToList<doMiscTypeCode>();

                            docMemoDetail.HandlingType = memoDetailTemp.HandlingType;
                            if (miscHandlingType != null && miscHandlingType.Count > 0)
                                docMemoDetail.HandlingTypeName = miscHandlingType[0].ValueDisplayLC;
                            else
                                docMemoDetail.HandlingTypeName = null;


                            docMemoDetail.StartPeriodDate = memoDetailTemp.StartPeriodDate;
                            docMemoDetail.EndPeriodDate = memoDetailTemp.EndPeriodDate;

                            #region Normal Fee Amount

                            docMemoDetail.NormalFeeAmountCurrencyType = memoDetailTemp.NormalFeeAmountCurrencyType;
                            docMemoDetail.NormalFeeAmount = memoDetailTemp.NormalFeeAmount;
                            docMemoDetail.NormalFeeAmountUsd = memoDetailTemp.NormalFeeAmountUsd;

                            #endregion
                            #region Fee Amount

                            docMemoDetail.FeeAmountCurrencyType = memoDetailTemp.FeeAmountCurrencyType;
                            docMemoDetail.FeeAmount = memoDetailTemp.FeeAmount;
                            docMemoDetail.FeeAmountUsd = memoDetailTemp.FeeAmountUsd;
                            #endregion
                            #region Tax Amount

                            docMemoDetail.TaxAmountCurrencyType = memoDetailTemp.TaxAmountCurrencyType;
                            docMemoDetail.TaxAmount = memoDetailTemp.TaxAmount;
                            docMemoDetail.TaxAmountUsd = memoDetailTemp.TaxAmountUsd;

                            #endregion

                            docMemoDetail.ContractCode_CounterBalance = memoDetailTemp.ContractCode_CounterBalance;
                            docMemoDetail.Remark = memoDetailTemp.Remark;
                            docMemoDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docMemoDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            docMemoDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docMemoDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            docCancelContractMemoDetailList.Add(docMemoDetail);
                        }
                    }
                    dsContractDocDataData.dtTbt_DocCancelContractMemoDetail = docCancelContractMemoDetailList;

                    //Save contract document data
                    dsContractDocData dsContractDocResult = rentralHandler.CreateContractDocData(dsContractDocDataData);
                    /*----------------------------------*/

                    scope.Complete();

                    msgModelResult = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
                    if (dsContractDocResult.dtTbt_ContractDocument != null && dsContractDocResult.dtTbt_ContractDocument.Count > 0)
                        strDocNoResult = comUtil.ConvertContractCode(dsContractDocResult.dtTbt_ContractDocument[0].DocNo, CommonUtil.CONVERT_TYPE.TO_SHORT);

                    res.ResultData = new object[] { msgModelResult, strDocNoResult };
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Method

        /// <summary>
        /// Bind data to control on screen
        /// </summary>
        /// <param name="doRentalContractBasic"></param>
        private void Bind_CTS080(doRentalContractBasicInformation doRentalContractBasic)
        {
            ViewBag.SaleContractBasicInformation = doRentalContractBasic;
            ViewBag.ContractCodeLong = doRentalContractBasic.ContractCode;
            ViewBag.ContractCodeShort = doRentalContractBasic.ContractCodeShort;
            ViewBag.UserCode = doRentalContractBasic.UserCode;
            ViewBag.ContractTargetCustCodeShort = doRentalContractBasic.ContractTargetCustCodeShort;
            ViewBag.RealCustomerCustCodeShort = doRentalContractBasic.RealCustomerCustCodeShort;
            ViewBag.SiteCodeShort = doRentalContractBasic.SiteCodeShort;
            ViewBag.ContractTargetCustomerImportant = doRentalContractBasic.ContractTargetCustomerImportant;
            ViewBag.CustFullNameEN = doRentalContractBasic.ContractTargetNameEN;
            ViewBag.AddressFullEN = doRentalContractBasic.ContractTargetAddressEN;
            ViewBag.SiteNameEN = doRentalContractBasic.SiteNameEN;
            ViewBag.SiteAddressEN = doRentalContractBasic.SiteAddressEN;
            ViewBag.CustFullNameLC = doRentalContractBasic.ContractTargetNameLC;
            ViewBag.AddressFullLC = doRentalContractBasic.ContractTargetAddressLC;
            ViewBag.SiteNameLC = doRentalContractBasic.SiteNameLC;
            ViewBag.SiteAddressLC = doRentalContractBasic.SiteAddressLC;
            ViewBag.OfficeName = doRentalContractBasic.OperationOfficeCodeName;
        }

        /// <summary>
        /// Check authority data of screen
        /// </summary>
        /// <param name="res"></param>
        /// <param name="strContractCodeLong"></param>
        /// <param name="isInitScreen"></param>
        /// <returns></returns>
        private dsRentalContractData CheckDataAuthority_CTS080(ObjectResultData res, string strContractCodeLong, bool isInitScreen = false)
        {
            CommonUtil comUtil = new CommonUtil();
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;

            rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            dsRentalContract = rentralHandler.GetEntireContract(strContractCodeLong, null);
            if (dsRentalContract == null || dsRentalContract.dtTbt_RentalContractBasic == null || dsRentalContract.dtTbt_RentalContractBasic.Count < 1)
            {
                if (isInitScreen)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124);
                }
                else
                {
                    string strContractCode = comUtil.ConvertContractCode(strContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { strContractCode });
                }
            }

            return dsRentalContract;
        }

        /// <summary>
        /// Validate business for CancelContract
        /// </summary>
        /// <param name="res"></param>
        /// <param name="cancelContractMemoDetailData"></param>
        private void ValidateBusinessCancelContract_CTS080(ObjectResultData res, tbt_CancelContractMemoDetail cancelContractMemoDetailData)
        {
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            CommonUtil comUtil = new CommonUtil();
            ICommonContractHandler comContractHandler;

            try
            {
                comContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;

                if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
                    || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE
                    || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
                {
                    if (cancelContractMemoDetailData.StartPeriodDate > cancelContractMemoDetailData.EndPeriodDate)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0101, null, new string[] { "dpPeriodFrom" });
                        return;
                    }
                }

                CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                memoDetailTempList = sParam.CTS080_Session.InitialData.CancelContractMemoDetailTempData;

                foreach (CTS110_CancelContractMemoDetailTemp memoDetail in memoDetailTempList)
                {
                    if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                    {
                        if (memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                        {
                            //Move to ValidateBusinessCancelContractForWarning_CTS080()
                            //if (cancelContractMemoDetailData.StartPeriodDate == memoDetail.StartPeriodDate
                            //    && cancelContractMemoDetailData.EndPeriodDate == memoDetail.EndPeriodDate)
                            //{
                            //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3108, null, new string[] { "dpPeriodFrom", "dpPeriodTo" });
                            //    return;
                            //}
                            //if (CheckOverlapPeriodDate_CTS110(memoDetail.StartPeriodDate, memoDetail.EndPeriodDate, cancelContractMemoDetailData.StartPeriodDate, cancelContractMemoDetailData.EndPeriodDate))
                            //{
                            //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null, new string[] { "dpPeriodFrom", "dpPeriodTo" });
                            //    return;
                            //}
                        }

                        if (memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3109, null, new string[] { "ddlFeeType" });
                            return;
                        }
                    }
                    else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                    {
                        if (memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                        {
                            //Move to ValidateBusinessCancelContractForWarning_CTS080()
                            //if (cancelContractMemoDetailData.StartPeriodDate == memoDetail.StartPeriodDate
                            //    && cancelContractMemoDetailData.EndPeriodDate == memoDetail.EndPeriodDate)
                            //{
                            //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3110, null, new string[] { "dpPeriodFrom", "dpPeriodTo" });
                            //    return;
                            //}
                            //if (CheckOverlapPeriodDate_CTS110(memoDetail.StartPeriodDate, memoDetail.EndPeriodDate, cancelContractMemoDetailData.StartPeriodDate, cancelContractMemoDetailData.EndPeriodDate))
                            //{
                            //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null, new string[] { "dpPeriodFrom", "dpPeriodTo" });
                            //    return;
                            //}
                        }

                        if (memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3111, null, new string[] { "ddlFeeType" });
                            return;
                        }
                    }
                    else if (cancelContractMemoDetailData.BillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
                    {
                        if (cancelContractMemoDetailData.BillingType == memoDetail.BillingType)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null, new string[] { "ddlFeeType" });
                            return;
                        }
                    }
                }

                if (String.IsNullOrEmpty(cancelContractMemoDetailData.ContractCode_CounterBalance) == false)
                {
                    string strContractCode_CounterBalanceLong = comUtil.ConvertContractCode(cancelContractMemoDetailData.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_LONG);
                    if (comContractHandler.IsContractExistInRentalOrSale(strContractCode_CounterBalanceLong) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, new string[] { cancelContractMemoDetailData.ContractCode_CounterBalance }, new string[] { "txtContractCodeForSlideFee" });
                        return;
                    }

                    //Bug report CT-148
                    if (sParam.CTS080_Session.InitialData.ContractCode == strContractCode_CounterBalanceLong.ToUpper())
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281, null, new string[] { "txtContractCodeForSlideFee" });
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Validate business for CancelContract (Warning)
        /// </summary>
        /// <param name="res"></param>
        /// <param name="cancelContractMemoDetailData"></param>
        private void ValidateBusinessCancelContractForWarning_CTS080(ObjectResultData res, tbt_CancelContractMemoDetail cancelContractMemoDetailData)
        {
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            try
            {
                CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                memoDetailTempList = sParam.CTS080_Session.InitialData.CancelContractMemoDetailTempData;

                foreach (CTS110_CancelContractMemoDetailTemp memoDetail in memoDetailTempList)
                {
                    if ((cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE && memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                        || (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE && memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                       )
                    {
                        if ((cancelContractMemoDetailData.StartPeriodDate == memoDetail.StartPeriodDate && cancelContractMemoDetailData.EndPeriodDate == memoDetail.EndPeriodDate)
                            || (CheckOverlapPeriodDate_CTS110(memoDetail.StartPeriodDate, memoDetail.EndPeriodDate, cancelContractMemoDetailData.StartPeriodDate, cancelContractMemoDetailData.EndPeriodDate))
                            )
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3282);
                            return;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Validate business of screen
        /// </summary>
        /// <param name="res"></param>
        /// <param name="strContractCodeLong"></param>
        private void ValidateBusiness_CTS080(ObjectResultData res, string strContractCodeLong)
        {
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            dsRentalContractData dsRentalContract = null;
            tbt_RentalContractBasic dtRentalContractBasicData;

            decimal maximumDigit = 999999999999.99M;
            decimal minimumDigit = -999999999999.99M;

            try
            {
                dsRentalContract = CheckDataAuthority_CTS080(res, strContractCodeLong);
                if (res.IsError)
                    return;

                if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
                {
                    dtRentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];

                    string strContractCodeShort = comUtil.ConvertContractCode(strContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);

                    //Comment by Jutarat A. on 06082012
                    //if (dtRentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL
                    //        || dtRentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_END
                    //        || dtRentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                    if (dtRentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3105, new string[] { strContractCodeShort }, new string[] { "txtSpecifyContractCode" });
                        return;
                    }
                }

                CTS080_ScreenParameter sParam = GetScreenObject<CTS080_ScreenParameter>();
                string strTotalLabel = string.Empty;
                if (sParam.CTS080_Session.InitialData.TotalSlideAmount > maximumDigit || sParam.CTS080_Session.InitialData.TotalSlideAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalSlideAmountRp";
                }
                else if (sParam.CTS080_Session.InitialData.TotalSlideAmountUsd > maximumDigit || sParam.CTS080_Session.InitialData.TotalSlideAmountUsd < minimumDigit)
                {
                    strTotalLabel = "lblTotalSlideAmountUsd";
                }
                else if (sParam.CTS080_Session.InitialData.TotalRefundAmount > maximumDigit || sParam.CTS080_Session.InitialData.TotalRefundAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalRefundAmountRp";
                }
                else if (sParam.CTS080_Session.InitialData.TotalRefundAmountUsd > maximumDigit || sParam.CTS080_Session.InitialData.TotalRefundAmountUsd < minimumDigit)
                {
                    strTotalLabel = "lblTotalRefundAmountUsd";
                }
                else if (sParam.CTS080_Session.InitialData.TotalBillingAmount > maximumDigit || sParam.CTS080_Session.InitialData.TotalBillingAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalBillingAmountRp";
                }
                else if (sParam.CTS080_Session.InitialData.TotalBillingAmountUsd > maximumDigit || sParam.CTS080_Session.InitialData.TotalBillingAmountUsd < minimumDigit)
                {
                    strTotalLabel = "lblTotalBillingAmountUsd";
                }
                else if (sParam.CTS080_Session.InitialData.TotalCounterBalAmount > maximumDigit || sParam.CTS080_Session.InitialData.TotalCounterBalAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalCounterBalAmountRp";
                }
                else if (sParam.CTS080_Session.InitialData.TotalCounterBalAmountUsd > maximumDigit || sParam.CTS080_Session.InitialData.TotalCounterBalAmountUsd < minimumDigit)
                {
                    strTotalLabel = "lblTotalCounterBalAmountUsd";
                }

                if (String.IsNullOrEmpty(strTotalLabel) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_PRE_CP14,
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3252,
                                        new string[] { strTotalLabel },
                                        null);
                    return;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        #endregion

    }
}
