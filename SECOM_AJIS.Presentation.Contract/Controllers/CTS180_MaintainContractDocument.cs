//*********************************
// Create by: Jutarat A.
// Create date: 16/Nov/2011
// Update date: 16/Nov/2011
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

using System.Reflection;
using System.Data;

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
        public ActionResult CTS180_Authority(CTS180_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_VIEW) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                sParam = new CTS180_ScreenParameter();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<CTS180_ScreenParameter>("CTS180", sParam, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS180")]
        public ActionResult CTS180()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                ViewBag.CanEditFee = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_EDIT_FEE);

                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Inintail DocumentList grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS180_InitialGridDocumentList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS180_DocumentList", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get data to DocumentList grid
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS180_GetDocumentListData(CTS180_doSearchContractDocCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            IContractDocumentHandler contDocHandler;
            List<dtContractDocumentList> dtContractDocList;

            try
            {
                List<CTS180_DocumentListGridData> resultGridData = new List<CTS180_DocumentListGridData>();

                ValidatorUtil.BuildErrorMessage(res, this, new object[] { cond });
                if (res.IsError)
                {
                    res.ResultData = CommonUtil.ConvertToXml<CTS180_DocumentListGridData>(resultGridData, "Contract\\CTS180_DocumentList", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                    return Json(res);
                }

                string strDocStatus = cond.DocStatus;
                if (String.IsNullOrEmpty(strDocStatus) == false && strDocStatus.Length > 0)
                {
                    String[] arrDocStatus = strDocStatus.Split(',');

                    //StringBuilder sbDocStatus = new StringBuilder();
                    List<string> docStatusList = new List<string>();
                    foreach (string s in arrDocStatus)
                    {
                        //sbDocStatus.AppendFormat("\'{0}\',", s);
                        docStatusList.Add(s);
                    }

                    //cond.DocStatus = sbDocStatus.ToString();
                    cond.DocStatus = CommonUtil.CreateCSVString(docStatusList);
                }

                CommonUtil comUtil = new CommonUtil();
                cond.ContractCode = comUtil.ConvertContractCode(cond.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                cond.QuotationTargetCode = comUtil.ConvertQuotationTargetCode(cond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                //if (String.IsNullOrEmpty(cond.OperationOfficeCode))
                //    cond.OperationOfficeCode = GetAllOperationOfficeCode_CTS290();

                //if (String.IsNullOrEmpty(cond.ContractOfficeCode))
                //    cond.ContractOfficeCode = GetAllContractOfficeCode_CTS290();

                cond.OperationOfficeCodeAuthority = GetAllOperationOfficeCode_CTS290();
                cond.ContractOfficeCodeAuthority = GetAllContractOfficeCode_CTS290();

                contDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
                dtContractDocList = contDocHandler.SearchContractDocument(cond);

                if (dtContractDocList != null && dtContractDocList.Count > 0)
                {
                    resultGridData = CommonUtil.ClonsObjectList<dtContractDocumentList, CTS180_DocumentListGridData>(dtContractDocList);
                    SetEnableSelect_CTS180(resultGridData);
                }

                CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                if (sParam.ContractDocumentList == null)
                    sParam.ContractDocumentList = new List<dtContractDocumentList>();

                sParam.ContractDocumentList = dtContractDocList;
                UpdateScreenObject(sParam);

                res.ResultData = CommonUtil.ConvertToXml<CTS180_DocumentListGridData>(resultGridData, "Contract\\CTS180_DocumentList", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Open section relating with selected data when click [Select] button on Contract document result list at ‘Contract document list’ section 
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        public ActionResult CTS180_SelectDocumentReport(int iDocID)
        {
            ObjectResultData res = new ObjectResultData();
            IContractDocumentHandler contDocHandler;
            List<tbt_ContractDocument> dtTbt_ContractDocument = new List<tbt_ContractDocument>();
            List<tbs_ContractDocTemplate> dtTbs_ContractDocTemplate = new List<tbs_ContractDocTemplate>();
            List<tbt_DocContractReport> dtTbt_DocContractReport = new List<tbt_DocContractReport>();
            List<tbt_DocChangeMemo> dtTbt_DocChangeMemo = new List<tbt_DocChangeMemo>();
            List<tbt_DocChangeNotice> dtTbt_DocChangeNotice = new List<tbt_DocChangeNotice>();
            List<tbt_DocConfirmCurrentInstrumentMemo> dtTbt_DocConfirmCurrentInstrumentMemo = new List<tbt_DocConfirmCurrentInstrumentMemo>();
            List<tbt_DocCancelContractMemo> dtTbt_DocCancelContractMemo = new List<tbt_DocCancelContractMemo>();
            List<tbt_DocCancelContractMemoDetail> dtTbt_DocCancelContractMemoDetail = new List<tbt_DocCancelContractMemoDetail>();
            List<tbt_DocChangeFeeMemo> dtTbt_DocChangeFeeMemo = new List<tbt_DocChangeFeeMemo>();

            try
            {
                CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                if (sParam.ContractDocumentList != null)
                {
                    contDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;

                    foreach (dtContractDocumentList data in sParam.ContractDocumentList)
                    {
                        if (data.DocID == iDocID)
                        {
                            dtTbt_ContractDocument = contDocHandler.GetTbt_ContractDocument(data.DocID);
                            dtTbs_ContractDocTemplate = contDocHandler.GetTbs_ContractDocTemplate(data.DocumentCode);

                            if (dtTbt_ContractDocument != null && dtTbt_ContractDocument.Count > 0)
                            {
                                if (dtTbt_ContractDocument[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN
                                    || dtTbt_ContractDocument[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH)
                                {
                                    dtTbt_DocContractReport = contDocHandler.GetTbt_DocContractReport(dtTbt_ContractDocument[0].DocID);
                                    if (dtTbt_DocContractReport != null && dtTbt_DocContractReport.Count > 0)
                                        res.ResultData = new object[] { dtTbt_ContractDocument[0], dtTbt_DocContractReport[0] };
                                }

                                if (dtTbt_ContractDocument[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_MEMO)
                                {
                                    dtTbt_DocChangeMemo = contDocHandler.GetTbt_DocChangeMemo(dtTbt_ContractDocument[0].DocID);
                                    if (dtTbt_DocChangeMemo != null && dtTbt_DocChangeMemo.Count > 0)
                                        res.ResultData = new object[] { dtTbt_ContractDocument[0], dtTbt_DocChangeMemo[0] };
                                }

                                if (dtTbt_ContractDocument[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_NOTICE)
                                {
                                    dtTbt_DocChangeNotice = contDocHandler.GetTbt_DocChangeNotice(dtTbt_ContractDocument[0].DocID);
                                    if (dtTbt_DocChangeNotice != null && dtTbt_DocChangeNotice.Count > 0)
                                        res.ResultData = new object[] { dtTbt_ContractDocument[0], dtTbt_DocChangeNotice[0] };
                                }

                                if (dtTbt_ContractDocument[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO)
                                {
                                    dtTbt_DocConfirmCurrentInstrumentMemo = contDocHandler.GetTbt_DocConfirmCurrentInstrumentMemo(dtTbt_ContractDocument[0].DocID);
                                    if (dtTbt_DocConfirmCurrentInstrumentMemo != null && dtTbt_DocConfirmCurrentInstrumentMemo.Count > 0)
                                        res.ResultData = new object[] { dtTbt_ContractDocument[0], dtTbt_DocConfirmCurrentInstrumentMemo[0] };
                                }

                                if (dtTbt_ContractDocument[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO
                                    || dtTbt_ContractDocument[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION)
                                {
                                    dtTbt_DocCancelContractMemo = contDocHandler.GetTbt_DocCancelContractMemo(dtTbt_ContractDocument[0].DocID);
                                    if (dtTbt_DocCancelContractMemo != null && dtTbt_DocCancelContractMemo.Count > 0)
                                    {
                                        res.ResultData = new object[] { dtTbt_ContractDocument[0], dtTbt_DocCancelContractMemo[0] };

                                        sParam.TotalSlideAmount = dtTbt_DocCancelContractMemo[0].TotalSlideAmt;
                                        sParam.TotalRefundAmount = dtTbt_DocCancelContractMemo[0].TotalReturnAmt;
                                        sParam.TotalBillingAmount = dtTbt_DocCancelContractMemo[0].TotalBillingAmt;
                                        sParam.TotalCounterBalAmount = dtTbt_DocCancelContractMemo[0].TotalAmtAfterCounterBalance;
                                    }

                                    dtTbt_DocCancelContractMemoDetail = contDocHandler.GetTbt_DocCancelContractMemoDetail(dtTbt_ContractDocument[0].DocID);
                                }

                                if (dtTbt_ContractDocument[0].DocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO)
                                {
                                    dtTbt_DocChangeFeeMemo = contDocHandler.GetTbt_DocChangeFeeMemo(dtTbt_ContractDocument[0].DocID);
                                    if (dtTbt_DocChangeFeeMemo != null && dtTbt_DocChangeFeeMemo.Count > 0)
                                        res.ResultData = new object[] { dtTbt_ContractDocument[0], dtTbt_DocChangeFeeMemo[0] };
                                }

                            }

                            break;  
                        }

                    }

                    sParam.DocumentCode = dtTbt_ContractDocument[0].DocumentCode;
                    sParam.dtTbt_ContractDocument = dtTbt_ContractDocument;
                    sParam.dtTbs_ContractDocTemplate = dtTbs_ContractDocTemplate;
                    sParam.dtTbt_DocContractReport = dtTbt_DocContractReport;
                    sParam.dtTbt_DocChangeMemo = dtTbt_DocChangeMemo;
                    sParam.dtTbt_DocChangeNotice = dtTbt_DocChangeNotice;
                    sParam.dtTbt_DocConfirmCurrentInstrumentMemo = dtTbt_DocConfirmCurrentInstrumentMemo;
                    sParam.dtTbt_DocCancelContractMemo = dtTbt_DocCancelContractMemo;
                    sParam.dtTbt_DocCancelContractMemoDetail = dtTbt_DocCancelContractMemoDetail;
                    sParam.dtTbt_DocChangeFeeMemo = dtTbt_DocChangeFeeMemo;
                    UpdateScreenObject(sParam);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        ///// <summary>
        ///// Get data of ContractReport
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult CTS180_GetContractReportList()
        //{
        //    ObjectResultData res = new ObjectResultData();

        //    List<CTS180_ContractReportGridData> gridData = new List<CTS180_ContractReportGridData>();
        //    List<tbt_ContractDocument> dtTbt_ContractDocument;
        //    List<tbt_DocContractReport> dtTbt_DocContractReport;

        //    string lblContractFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_MAINTAIN_CONTRACT_DOCUMENT, "lblContractFee");
        //    string lblInstallationFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_MAINTAIN_CONTRACT_DOCUMENT, "lblInstallationFee");
        //    string lblDepositFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_MAINTAIN_CONTRACT_DOCUMENT, "lblDepositFee");

        //    try
        //    {
        //        CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
        //        dtTbt_ContractDocument = sParam.dtTbt_ContractDocument;
        //        dtTbt_DocContractReport = sParam.dtTbt_DocContractReport;

        //        if (dtTbt_DocContractReport != null && dtTbt_DocContractReport.Count > 0)
        //        {
        //            CTS180_ContractReportGridData data = new CTS180_ContractReportGridData();
        //            data.Fee = lblContractFee;
        //            data.Order = CommonUtil.TextNumeric(dtTbt_ContractDocument[0].ContractFee);
        //            gridData.Add(data);

        //            data = new CTS180_ContractReportGridData();
        //            data.Fee = lblInstallationFee;
        //            data.Order = CommonUtil.TextNumeric(dtTbt_DocContractReport[0].NegotiationTotalInstallFee);
        //            data.ApproveContract = CommonUtil.TextNumeric(dtTbt_DocContractReport[0].InstallFee_ApproveContract);
        //            data.CompleteInstallation = CommonUtil.TextNumeric(dtTbt_DocContractReport[0].InstallFee_CompleteInstall);
        //            data.StartService = CommonUtil.TextNumeric(dtTbt_DocContractReport[0].InstallFee_StartService);
        //            gridData.Add(data);

        //            data = new CTS180_ContractReportGridData();
        //            data.Fee = lblDepositFee;
        //            data.Order = CommonUtil.TextNumeric(dtTbt_ContractDocument[0].DepositFee);
        //            gridData.Add(data);
        //        }

        //        res.ResultData = CommonUtil.ConvertToXml<CTS180_ContractReportGridData>(gridData, "Contract\\CTS180_ContractReport", CommonUtil.GRID_EMPTY_TYPE.VIEW);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}

        /// <summary>
        /// Get data to CancelContract grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS180_GetCancelContractMemoDetail()
        {
            ObjectResultData res = new ObjectResultData();
            List<tbt_DocCancelContractMemoDetail> dtTbt_DocCancelContractMemoDetail;
            List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;
            CommonUtil comUtil = new CommonUtil();

            try
            {
                string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblNormalFee");
                string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblContractCodeForSlideFee");

                CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                dtTbt_DocCancelContractMemoDetail = sParam.dtTbt_DocCancelContractMemoDetail;
                memoDetailTempList = new List<CTS110_CancelContractMemoDetailTemp>();

                int intSequence = -1;
                if (dtTbt_DocCancelContractMemoDetail != null)
                {
                    foreach (tbt_DocCancelContractMemoDetail memoDetail in dtTbt_DocCancelContractMemoDetail)
                    {
                        string strStartPeriodDate = memoDetail.StartPeriodDate == null ? string.Empty : CommonUtil.TextDate(memoDetail.StartPeriodDate.Value);
                        string strEndPeriodDate = memoDetail.EndPeriodDate == null ? string.Empty : string.Format("TO {0}", CommonUtil.TextDate(memoDetail.EndPeriodDate.Value));
                        string strContractCodeCounterBal = comUtil.ConvertContractCode(memoDetail.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        CTS110_CancelContractConditionGridData data = new CTS110_CancelContractConditionGridData();
                        memoDetail.BillingTypeName = memoDetail.BillingTypeNameForShow;
                        memoDetail.HandlingTypeName = memoDetail.HandlingTypeNameForShow;
                        data.FeeType = memoDetail.BillingTypeName;
                        data.HandlingType = memoDetail.HandlingTypeName;


                        //data.Fee = memoDetail.FeeAmount == null ? string.Empty : CommonUtil.TextNumeric(memoDetail.FeeAmount.Value);
                        //data.Tax = memoDetail.TaxAmount == null ? string.Empty : CommonUtil.TextNumeric(memoDetail.TaxAmount.Value);

                        ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        List<doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                        if (currencies != null)
                        {
                            //if (memoDetail.FeeAmountCurrencyType == null)
                            //    memoDetail.FeeAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                            //if (memoDetail.TaxAmountCurrencyType == null)
                            //    memoDetail.TaxAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                            
                            #region Fee

                            DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.FeeAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];

                            if (memoDetail.FeeAmountCurrencyType != null)
                                data.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(
                                    memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US ? memoDetail.FeeAmountUsd.Value : memoDetail.FeeAmount.Value));

                            #endregion
                            #region Tax

                            curr = currencies.Find(x => x.ValueCode == memoDetail.TaxAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];

                            if (memoDetail.TaxAmountCurrencyType != null)
                                data.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric((
                                    memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US ? memoDetail.TaxAmountUsd : memoDetail.TaxAmount) ?? 0));

                            #endregion
                        }

                        data.Period = String.Format("{0} {1}", strStartPeriodDate, strEndPeriodDate);

                        //data.Remark = String.Format("{0} {1} {2}", memoDetail.Remark
                        //    , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, strContractCodeCounterBal)
                        //    , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value)));
                        data.Remark = String.Format("{0}{1}{2}", string.IsNullOrEmpty(memoDetail.Remark) ? string.Empty : string.Format("{0}<br/>", memoDetail.Remark)
                            , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("{0}: {1}<br/>", lblContractCodeForSlideFee, strContractCodeCounterBal)
                            , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value)));

                        data.FeeTypeCode = memoDetail.BillingType;
                        data.HandlingTypeCode = memoDetail.HandlingType;
                        data.Sequence = (intSequence + 1).ToString();
                        gridDataList.Add(data);

                        CTS110_CancelContractMemoDetailTemp memoDetailTemp = CommonUtil.CloneObject<tbt_DocCancelContractMemoDetail, CTS110_CancelContractMemoDetailTemp>(memoDetail);
                        memoDetailTemp.Sequence = (intSequence + 1).ToString();
                        memoDetailTempList.Add(memoDetailTemp);

                        intSequence++;
                    }
                }

                sParam.CancelContractMemoDetailTempData = memoDetailTempList;
                UpdateScreenObject(sParam);

                res.ResultData = CommonUtil.ConvertToXml<CTS110_CancelContractConditionGridData>(gridDataList, "Contract\\CTS110_CancelContractCondition", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Reload data of CancelContract grid 
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS180_RefreshCancelContractMemoDetail()
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblNormalFee");
            string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblContractCodeForSlideFee");

            try
            {
                CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                memoDetailTempList = sParam.CancelContractMemoDetailTempData;

                if (memoDetailTempList != null)
                {
                    ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                    foreach (CTS110_CancelContractMemoDetailTemp memoDetail in memoDetailTempList)
                    {
                        string strStartPeriodDate = memoDetail.StartPeriodDate == null ? string.Empty : CommonUtil.TextDate(memoDetail.StartPeriodDate.Value);
                        string strEndPeriodDate = memoDetail.EndPeriodDate == null ? string.Empty : string.Format("TO {0}", CommonUtil.TextDate(memoDetail.EndPeriodDate.Value));

                        CTS110_CancelContractConditionGridData data = new CTS110_CancelContractConditionGridData();
                        data.FeeType = memoDetail.BillingTypeName;
                        data.HandlingType = memoDetail.HandlingTypeName;

                        //data.Fee = memoDetail.FeeAmount == null ? string.Empty : CommonUtil.TextNumeric(memoDetail.FeeAmount.Value);
                        //data.Tax = memoDetail.TaxAmount == null ? string.Empty : CommonUtil.TextNumeric(memoDetail.TaxAmount.Value);

                        if (currencies != null)
                        {
                            //if (memoDetail.FeeAmountCurrencyType == null)
                            //    memoDetail.FeeAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                            //if (memoDetail.TaxAmountCurrencyType == null)
                            //    memoDetail.TaxAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;

                            #region Fee

                            DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.FeeAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];

                            if (memoDetail.FeeAmountCurrencyType != null)
                                data.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(
                                    memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US ? memoDetail.FeeAmountUsd.Value : memoDetail.FeeAmount.Value));

                            #endregion
                            #region Tax

                            curr = currencies.Find(x => x.ValueCode == memoDetail.TaxAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];

                            if (memoDetail.TaxAmountCurrencyType != null)
                            {
                                decimal val = 0;
                                if (memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    if (memoDetail.TaxAmountUsd != null)
                                        val = memoDetail.TaxAmountUsd.Value;
                                }
                                else
                                {
                                    if (memoDetail.TaxAmount != null)
                                        val = memoDetail.TaxAmount.Value;
                                }

                                data.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(val));
                            }
                            
                            #endregion
                        }

                        data.Period = String.Format("{0} {1}", strStartPeriodDate, strEndPeriodDate);
                        
                        //data.Remark = String.Format("{0} {1} {2}", memoDetail.Remark
                        //    , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                        //    , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value)));
                        data.Remark = String.Format("{0}{1}{2}", string.IsNullOrEmpty(memoDetail.Remark) ? string.Empty : string.Format("{0}<br/>", memoDetail.Remark)
                            , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("{0}: {1}<br/>", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                            , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value)));

                        data.FeeTypeCode = memoDetail.BillingType;
                        data.HandlingTypeCode = memoDetail.HandlingType;
                        data.Sequence = memoDetail.Sequence;
                        gridDataList.Add(data);
                    }
                }

                res.ResultData = CommonUtil.ConvertToXml<CTS110_CancelContractConditionGridData>(gridDataList, "Contract\\CTS110_CancelContractCondition", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #region No use CTS180_GetCancelContractCondition

        //public ActionResult CTS180_GetCancelContractCondition()
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    List<tbt_DocCancelContractMemo> dtTbt_DocCancelContractMemo;
        //    List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
        //    try
        //    {
        //        CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
        //        dtTbt_DocCancelContractMemo = sParam.dtTbt_DocCancelContractMemo;

        //        if (dtTbt_DocCancelContractMemo != null && dtTbt_DocCancelContractMemo.Count > 0)
        //        {
        //            tbt_DocCancelContractMemo cancelContractMemoData = CommonUtil.CloneObject<tbt_DocCancelContractMemo, tbt_DocCancelContractMemo>(dtTbt_DocCancelContractMemo[0]);

        //            CTS110_CancelContractConditionGridData memoData = new CTS110_CancelContractConditionGridData();
        //            memoData.TotalSlideAmt = cancelContractMemoData.TotalSlideAmt == null ? string.Empty : CommonUtil.TextNumeric(cancelContractMemoData.TotalSlideAmt.Value);
        //            memoData.TotalReturnAmt = cancelContractMemoData.TotalReturnAmt == null ? string.Empty : CommonUtil.TextNumeric(cancelContractMemoData.TotalReturnAmt.Value);
        //            memoData.TotalBillingAmt = cancelContractMemoData.TotalBillingAmt == null ? string.Empty : CommonUtil.TextNumeric(cancelContractMemoData.TotalBillingAmt.Value);
        //            memoData.TotalAmtAfterCounterBalance = cancelContractMemoData.TotalAmtAfterCounterBalance == null ? string.Empty : CommonUtil.TextNumeric(cancelContractMemoData.TotalAmtAfterCounterBalance);
        //            memoData.ProcessAfterCounterBalanceType = cancelContractMemoData.ProcessAfterCounterBalanceType;
        //            memoData.OtherRemarks = cancelContractMemoData.OtherRemarks;

        //            sParam.TotalSlideAmount = cancelContractMemoData.TotalSlideAmt;
        //            sParam.TotalRefundAmount = cancelContractMemoData.TotalReturnAmt;
        //            sParam.TotalBillingAmount = cancelContractMemoData.TotalBillingAmt;
        //            sParam.TotalCounterBalAmount = cancelContractMemoData.TotalAmtAfterCounterBalance;
        //            UpdateScreenObject(sParam);

        //            res.ResultData = memoData;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}

        #endregion

        /// <summary>
        /// Calculate total amount of Fee
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS180_CalculateTotalAmount()
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            try
            {
                CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                memoDetailTempList = sParam.CancelContractMemoDetailTempData;

                decimal decSlideAmount = 0;
                decimal decRefundAmount = 0;
                decimal decBillingAmount = 0;
                decimal decCounterBalAmount = 0;
                decimal decSlideAmountUsd = 0;
                decimal decRefundAmountUsd = 0;
                decimal decBillingAmountUsd = 0;
                decimal decCounterBalAmountUsd = 0;

                
                foreach (CTS110_CancelContractMemoDetailTemp memoDetail in memoDetailTempList)
                {
                    decimal decFeeAmount = 0;
                    decimal decTaxAmount = 0;
                    decimal decTotalAmount = 0;
                    decimal decFeeAmountUsd = 0;
                    decimal decTaxAmountUsd = 0;
                    decimal decTotalAmountUsd = 0;

                    if (memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        decFeeAmountUsd = memoDetail.FeeAmountUsd == null ? 0 : memoDetail.FeeAmountUsd.Value; 
                    else
                        decFeeAmount = memoDetail.FeeAmount == null ? 0 : memoDetail.FeeAmount.Value;

                    if (memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        decTaxAmountUsd = memoDetail.TaxAmountUsd == null ? 0 : memoDetail.TaxAmountUsd.Value;
                    else
                        decTaxAmount = memoDetail.TaxAmount == null ? 0 : memoDetail.TaxAmount.Value;


                    decTotalAmount = (decFeeAmount + decTaxAmount);
                    decTotalAmountUsd = (decFeeAmountUsd + decTaxAmountUsd);

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

                decCounterBalAmount = decRefundAmount - decBillingAmount;
                decCounterBalAmountUsd = decRefundAmountUsd - decBillingAmountUsd;

                sParam.TotalSlideAmount = decSlideAmount;
                sParam.TotalRefundAmount = decRefundAmount;
                sParam.TotalBillingAmount = decBillingAmount;
                sParam.TotalCounterBalAmount = decCounterBalAmount;

                sParam.TotalSlideAmountUsd = decSlideAmountUsd;
                sParam.TotalRefundAmountUsd = decRefundAmountUsd;
                sParam.TotalBillingAmountUsd = decBillingAmountUsd;
                sParam.TotalCounterBalAmountUsd = decCounterBalAmountUsd;

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
        /// Validate data when Add Fee to grid
        /// </summary>
        /// <param name="cancelContractMemoDetailData"></param>
        /// <returns></returns>
        public ActionResult CTS180_ValidateAddCancelContractData(CTS180_CancelContractMemoDetailData cancelContractMemoDetailData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            string strCancelContractTypeCC = "CC";

            try
            {
                if (cancelContractMemoDetailData != null)
                {
                    //Check mandatory
                    object memoDetailTemp = null;
                    if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
                        || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                    {
                        if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateContractMaintenanceFeeCC>(cancelContractMemoDetailData);
                        else
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateContractMaintenanceFeeQC>(cancelContractMemoDetailData);
                    }
                    else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE
                                || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE
                                || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
                    {
                        if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateDepositCardOtherFeeCC>(cancelContractMemoDetailData);
                        else
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateDepositCardOtherFeeQC>(cancelContractMemoDetailData);

                    }
                    else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
                    {
                        if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateRemovalFeeCC>(cancelContractMemoDetailData);
                        else
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateRemovalFeeQC>(cancelContractMemoDetailData);
                    }
                    else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                    {
                        if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateChangeFeeCC>(cancelContractMemoDetailData);
                        else
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateChangeFeeQC>(cancelContractMemoDetailData);
                    }
                    else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
                    {
                        if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateCancelFeeCC>(cancelContractMemoDetailData);
                        else
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateCancelFeeQC>(cancelContractMemoDetailData);
                    }
                    else
                    {
                        if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateFeeTypeHandlingType>(cancelContractMemoDetailData);
                        else
                            memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateFeeTypeHandlingType>(cancelContractMemoDetailData);
                    }

                    ValidatorUtil.BuildErrorMessage(res, new object[] { memoDetailTemp });
                    if (res.IsError)
                        return Json(res);

                    //ValidateBusiness
                    ValidateBusinessCancelContract_CTS180(res, cancelContractMemoDetailData);
                    if (res.IsError)
                        return Json(res);

                    //ValidateBusinessForWarning
                    ValidateBusinessCancelContractForWarning_CTS180(res, cancelContractMemoDetailData);

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
        public ActionResult CTS180_AddCancelContractData(CTS180_CancelContractMemoDetailData cancelContractMemoDetailData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;
            CommonUtil comUtil = new CommonUtil();

            string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblNormalFee");
            string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblContractCodeForSlideFee");

            string strCancelContractTypeCC = "CC";

            try
            {
                if (cancelContractMemoDetailData != null)
                {
                    #region Move to CTS180_ValidateAddCancelContractData()
                    ////Check mandatory
                    //object memoDetailTemp = null;
                    //if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
                    //    || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                    //{
                    //    if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateContractMaintenanceFeeCC>(cancelContractMemoDetailData);
                    //    else
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateContractMaintenanceFeeQC>(cancelContractMemoDetailData);
                    //}
                    //else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE
                    //            || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE
                    //            || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
                    //{
                    //    if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateDepositCardOtherFeeCC>(cancelContractMemoDetailData);
                    //    else
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateDepositCardOtherFeeQC>(cancelContractMemoDetailData);
                        
                    //}
                    //else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
                    //{
                    //    if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateRemovalFeeCC>(cancelContractMemoDetailData);
                    //    else
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateRemovalFeeQC>(cancelContractMemoDetailData);
                    //}
                    //else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                    //{
                    //    if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateChangeFeeCC>(cancelContractMemoDetailData);
                    //    else
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateChangeFeeQC>(cancelContractMemoDetailData);
                    //}
                    //else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
                    //{
                    //    if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateCancelFeeCC>(cancelContractMemoDetailData);
                    //    else
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS180_ValidateCancelFeeQC>(cancelContractMemoDetailData);
                    //}
                    //else
                    //{
                    //    if (cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC)
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateFeeTypeHandlingType>(cancelContractMemoDetailData);
                    //    else
                    //        memoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_ValidateFeeTypeHandlingType>(cancelContractMemoDetailData);
                    //}

                    //ValidatorUtil.BuildErrorMessage(res, new object[] { memoDetailTemp });
                    //if (res.IsError)
                    //    return Json(res);

                    ////ValidateBusiness
                    //ValidateBusinessCancelContract_CTS180(res, cancelContractMemoDetailData);
                    //if (res.IsError)
                    //    return Json(res);

                    ////ValidateBusinessForWarning
                    //ValidateBusinessCancelContractForWarning_CTS180(res, cancelContractMemoDetailData);
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

                    CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                    memoDetailTempList = sParam.CancelContractMemoDetailTempData;

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

                    if (currencies != null)
                    {
                        #region Fee

                        DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == memoDetail.FeeAmountCurrencyType);
                        if (curr == null)
                            curr = currencies[0];

                        if(memoDetail.FeeAmountUsd != null || memoDetail.FeeAmount != null)
                            gridData.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(
                                 memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US ? memoDetail.FeeAmountUsd.Value : memoDetail.FeeAmount.Value));

                        #endregion
                        #region Tax

                        curr = currencies.Find(x => x.ValueCode == memoDetail.TaxAmountCurrencyType);
                        if (curr == null)
                            curr = currencies[0];

                        if(memoDetail.TaxAmount != null || memoDetail.TaxAmountUsd != null)
                            gridData.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(
                                memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US ? memoDetail.TaxAmountUsd.Value : memoDetail.TaxAmount.Value));

                        #endregion
                    }

                    //gridData.Fee = memoDetail.FeeAmount == null ? string.Empty : CommonUtil.TextNumeric(memoDetail.FeeAmount.Value);
                    //gridData.Tax = memoDetail.TaxAmount == null ? string.Empty : CommonUtil.TextNumeric(memoDetail.TaxAmount.Value);


                    gridData.Period = String.Format("{0} {1}", strStartDate, memoDetail.EndPeriodDate == null ? string.Empty : string.Format("TO {0}", strEndDate));
                    
                    //gridData.Remark = String.Format("{0} {1} {2}", memoDetail.Remark
                    //                , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                    //                , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value)));
                    gridData.Remark = String.Format("{0}{1}{2}", string.IsNullOrEmpty(memoDetail.Remark) ? string.Empty : string.Format("{0}<br/>", memoDetail.Remark)
                        , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("{0}: {1}<br/>", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                        , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value)));


                    gridData.FeeTypeCode = memoDetail.BillingType;
                    gridData.HandlingTypeCode = memoDetail.HandlingType;
                    gridData.Sequence = strSequence;

                    CTS110_CancelContractMemoDetailTemp cacelMemoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_CancelContractMemoDetailTemp>(memoDetail);
                    cacelMemoDetailTemp.ContractCode_CounterBalance = comUtil.ConvertContractCode(cacelMemoDetailTemp.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_LONG);
                    cacelMemoDetailTemp.Sequence = strSequence;
                    memoDetailTempList.Add(cacelMemoDetailTemp);

                    sParam.CancelContractMemoDetailTempData = memoDetailTempList;
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
        /// Remove Fee from grid when click [Remove] button on grid
        /// </summary>
        /// <param name="strSequence"></param>
        /// <returns></returns>
        public ActionResult CTS180_RemoveDataCancelCond(string strSequence)
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            try
            {
                CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                memoDetailTempList = sParam.CancelContractMemoDetailTempData;

                foreach (CTS110_CancelContractMemoDetailTemp memoDetail in memoDetailTempList)
                {
                    if (memoDetail.Sequence == strSequence)
                    {
                        memoDetailTempList.Remove(memoDetail);
                        break;
                    }
                }

                sParam.CancelContractMemoDetailTempData = memoDetailTempList;
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
        /// Validate business when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <param name="registerData"></param>
        /// <returns></returns>
        public ActionResult CTS180_RegisterContractDocument(CTS180_RegisterContractDocumentData registerData)
        {
            ObjectResultData res = new ObjectResultData();
            
            try
            {
                //Check system status
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_VIEW) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                //Validate required fields
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

                CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                string strDocumentCode = sParam.DocumentCode;

                if (sParam.dtTbt_ContractDocument != null && sParam.dtTbt_ContractDocument.Count > 0)
                {
                    object requireData = null;
                    object requireAutoTransfer = null;
                    object requireBankTransfer = null;
                    object requireBankTransferUsd = null;
                    CTS180_ValidateProcessAfterCounterBalanceType requireProcAftCntBal = null;

                    if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN
                        || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH)
                    {
                        //requireData = CommonUtil.CloneObject<CTS180_RegisterContractDocumentData, CTS180_ValidateContract>(registerData);
                        if (registerData.ContractFee == null)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_MAINTAIN_CONTRACT_DOCUMENT,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblContractFee" },
                                        new string[] { registerData.ContractFeeControlName });
                            return Json(res);
                        }
                    }
                    else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_MEMO
                        || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_NOTICE)
                    {
                        requireData = CommonUtil.CloneObject<CTS180_RegisterContractDocumentData, CTS180_ValidateChangeMemo>(registerData);
                    }
                    else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO)
                    {
                        requireData = CommonUtil.CloneObject<CTS180_RegisterContractDocumentData, CTS180_ValidateCancelContract>(registerData);

                        if (registerData.AutoTransferBillingType == AutoTransferBillingType.C_AUTO_TRANSFER_BILLING_TYPE_PARTIAL)
                        {
                            requireAutoTransfer = CommonUtil.CloneObject<CTS180_RegisterContractDocumentData, CTS180_ValidateAutoTransfer>(registerData);
                        }

                        if (registerData.BankTransferBillingType == BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_PARTIAL)
                        {
                            requireBankTransfer = CommonUtil.CloneObject<CTS180_RegisterContractDocumentData, CTS180_ValidateBankTransfer>(registerData);
                        }
                        if (registerData.BankTransferBillingTypeUsd == BankTransferBillingType.C_BANK_TRANSFER_BILLING_TYPE_PARTIAL)
                        {
                            requireBankTransferUsd = CommonUtil.CloneObject<CTS180_RegisterContractDocumentData, CTS180_ValidateBankTransferUsd>(registerData);
                        }

                        requireProcAftCntBal = CommonUtil.CloneObject<CTS180_RegisterContractDocumentData, CTS180_ValidateProcessAfterCounterBalanceType>(registerData);
                        if (sParam.TotalRefundAmount > sParam.TotalBillingAmount)
                        {
                            if (requireProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND
                                && requireProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE)
                            {
                                requireProcAftCntBal.ProcessAfterCounterBalanceType = null;
                            }
                        }
                        else if (sParam.TotalRefundAmount < sParam.TotalBillingAmount)
                        {
                            if (requireProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL
                                && requireProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT)
                            {
                                requireProcAftCntBal.ProcessAfterCounterBalanceType = null;
                            }
                        }
                        else
                        {
                            requireProcAftCntBal.ProcessAfterCounterBalanceType = "0";
                        }

                        if (sParam.TotalRefundAmountUsd > sParam.TotalBillingAmountUsd)
                        {
                            if (requireProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND
                                && requireProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE)
                            {
                                requireProcAftCntBal.ProcessAfterCounterBalanceTypeUsd = null;
                            }
                        }
                        else if (sParam.TotalRefundAmountUsd < sParam.TotalBillingAmountUsd)
                        {
                            if (requireProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL
                                && requireProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT)
                            {
                                requireProcAftCntBal.ProcessAfterCounterBalanceTypeUsd = null;
                            }
                        }
                        else
                        {
                            requireProcAftCntBal.ProcessAfterCounterBalanceTypeUsd = "0";
                        }

                    }
                    else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION)
                    {
                        requireData = CommonUtil.CloneObject<CTS180_RegisterContractDocumentData, CTS180_ValidateCancelQuotationContract>(registerData);
                    }
                    else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO)
                    {
                        requireData = CommonUtil.CloneObject<CTS180_RegisterContractDocumentData, CTS180_ValidateChangeFeeMemo>(registerData);
                    }

                    ValidatorUtil.BuildErrorMessage(res, new object[] { requireData, requireAutoTransfer, requireBankTransfer, requireBankTransferUsd, requireProcAftCntBal }, null, false);
                    if (res.IsError)
                        return Json(res);

                }

                //Validate business
                ValidateBusiness_CTS180(res, registerData, strDocumentCode);
                if (res.IsError)
                {
                    if(res.MessageType != MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST)
                    {
                    return Json(res);
                    }
                }      

                //ValidateBusinessForWarning
                if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO
                    || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION)
                {
                    if (sParam.CancelContractMemoDetailTempData == null || sParam.CancelContractMemoDetailTempData.Count == 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3113);
                    }
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
        /// <param name="registerData"></param>
        /// <returns></returns>
        public ActionResult CTS180_ConfirmContractDocument(CTS180_RegisterContractDocumentData registerData)
        {
            ObjectResultData res = new ObjectResultData();

            dsContractDocData dsContractDocDataData = null;
            List<tbt_ContractDocument> dtTbt_ContractDocument = new List<tbt_ContractDocument>();
            List<tbs_ContractDocTemplate> dtTbs_ContractDocTemplate = new List<tbs_ContractDocTemplate>();
            List<tbt_DocContractReport> dtTbt_DocContractReport = new List<tbt_DocContractReport>();
            List<tbt_DocChangeMemo> dtTbt_DocChangeMemo = new List<tbt_DocChangeMemo>();
            List<tbt_DocChangeNotice> dtTbt_DocChangeNotice = new List<tbt_DocChangeNotice>();
            List<tbt_DocConfirmCurrentInstrumentMemo> dtTbt_DocConfirmCurrentInstrumentMemo = new List<tbt_DocConfirmCurrentInstrumentMemo>();
            List<tbt_DocCancelContractMemo> dtTbt_DocCancelContractMemo = new List<tbt_DocCancelContractMemo>();
            List<tbt_DocCancelContractMemoDetail> dtTbt_DocCancelContractMemoDetail = new List<tbt_DocCancelContractMemoDetail>();
            List<tbt_DocChangeFeeMemo> dtTbt_DocChangeFeeMemo = new List<tbt_DocChangeFeeMemo>();

            IContractDocumentHandler contDocHandler;
            IRentralContractHandler rentContHandler;

            try
            {
                //Check system status
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_VIEW) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                //Validate business
                CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                string strDocumentCode = sParam.DocumentCode;

                ValidateBusiness_CTS180(res, registerData, strDocumentCode);
                if (res.IsError)
                    return Json(res);

                //RegisterContractDocument
                using (TransactionScope scope = new TransactionScope())
                {
                    dtTbt_ContractDocument = CommonUtil.ClonsObjectList<tbt_ContractDocument, tbt_ContractDocument>(sParam.dtTbt_ContractDocument);
                    dtTbs_ContractDocTemplate = CommonUtil.ClonsObjectList<tbs_ContractDocTemplate, tbs_ContractDocTemplate>(sParam.dtTbs_ContractDocTemplate);
                    dtTbt_DocContractReport = CommonUtil.ClonsObjectList<tbt_DocContractReport, tbt_DocContractReport>(sParam.dtTbt_DocContractReport);
                    dtTbt_DocChangeMemo = CommonUtil.ClonsObjectList<tbt_DocChangeMemo, tbt_DocChangeMemo>(sParam.dtTbt_DocChangeMemo);
                    dtTbt_DocChangeNotice = CommonUtil.ClonsObjectList<tbt_DocChangeNotice, tbt_DocChangeNotice>(sParam.dtTbt_DocChangeNotice);
                    dtTbt_DocConfirmCurrentInstrumentMemo = CommonUtil.ClonsObjectList<tbt_DocConfirmCurrentInstrumentMemo, tbt_DocConfirmCurrentInstrumentMemo>(sParam.dtTbt_DocConfirmCurrentInstrumentMemo);
                    dtTbt_DocCancelContractMemo = CommonUtil.ClonsObjectList<tbt_DocCancelContractMemo, tbt_DocCancelContractMemo>(sParam.dtTbt_DocCancelContractMemo);
                    dtTbt_DocCancelContractMemoDetail = CommonUtil.ClonsObjectList<tbt_DocCancelContractMemoDetail, tbt_DocCancelContractMemoDetail>(sParam.dtTbt_DocCancelContractMemoDetail);
                    dtTbt_DocChangeFeeMemo = CommonUtil.ClonsObjectList<tbt_DocChangeFeeMemo, tbt_DocChangeFeeMemo>(sParam.dtTbt_DocChangeFeeMemo);

                    //Generate contract document occurrence
                    string strContractCode = string.Empty;
                    string strOCC = string.Empty;
                    if (dtTbt_ContractDocument != null && dtTbt_ContractDocument.Count > 0)
                    {
                        if (String.IsNullOrEmpty(dtTbt_ContractDocument[0].ContractCode) == false)
                        {
                            strContractCode = dtTbt_ContractDocument[0].ContractCode;
                            strOCC = dtTbt_ContractDocument[0].OCC;
                        }
                        else
                        {
                            strContractCode = dtTbt_ContractDocument[0].QuotationTargetCode;
                            strOCC = dtTbt_ContractDocument[0].Alphabet;
                        }
                    }

                    contDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
                    string strContractDocOCC = contDocHandler.GenerateDocOCC(strContractCode, strOCC);


                    //Create contract document data object
                    dsContractDocDataData = new dsContractDocData();

                    //MapContractDocumentData
                    IMasterHandler masHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                    List<tbm_Product> productList;

                    List<string> strMiscList = new List<string>();
                    strMiscList.Add(MiscType.C_PHONE_LINE_TYPE);
                    strMiscList.Add(MiscType.C_PHONE_LINE_OWNER_TYPE);
                    strMiscList.Add(MiscType.C_PROC_AFT_COUNTER_BALANCE_TYPE);
                    strMiscList.Add(MiscType.C_CONTRACT_BILLING_TYPE);
                    strMiscList.Add(MiscType.C_HANDLING_TYPE);

                    ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> miscTypeList = comHandler.GetMiscTypeCodeListByFieldName(strMiscList);

                    //Set data in dtTbt_ContractDocument
                    if (dtTbt_ContractDocument != null && dtTbt_ContractDocument.Count > 0)
                    {
                        dsContractDocDataData.dtTbt_ContractDocument = new List<tbt_ContractDocument>();
                        tbt_ContractDocument contractDoc = dtTbt_ContractDocument[0];

                        if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN
                            || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH)
                        {
                            //contractDoc.DocID = null; => Auto-run
                            contractDoc.ContractDocOCC = strContractDocOCC;

                            if (String.IsNullOrEmpty(contractDoc.ContractCode) == false)
                            {
                                contractDoc.DocNo = String.Format("{0}-{1}-{2}", contractDoc.ContractCode, contractDoc.OCC, strContractDocOCC);
                            }
                            else
                            {
                                contractDoc.DocNo = String.Format("{0}-{1}-{2}", contractDoc.QuotationTargetCode, contractDoc.Alphabet, strContractDocOCC);
                            }

                            contractDoc.SECOMSignatureFlag = registerData.SECOMSignatureFlag;
                            contractDoc.EmpName = registerData.EmpName;
                            contractDoc.EmpPosition = registerData.EmpPosition;

                            if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN)
                            {
                                contractDoc.RealCustomerNameEN = registerData.RealCustomerNameEN;
                                contractDoc.SiteAddressEN = registerData.SiteAddressEN;
                            }
                            else
                            {
                                contractDoc.RealCustomerNameLC = registerData.RealCustomerNameLC;
                                contractDoc.SiteAddressLC = registerData.SiteAddressLC;
                            }

                            contractDoc.SiteNameLC = registerData.SiteNameLC;
                            contractDoc.SiteNameEN = registerData.SiteNameEN;
                            contractDoc.IssuedDate = null;
                            contractDoc.DocEditFlag = "1";
                            contractDoc.DocEditor = CommonUtil.dsTransData.dtUserData.EmpNo;
                            contractDoc.DocEditDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            contractDoc.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_NOT_ISSUED;
                            contractDoc.DocAuditResult = DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED;
                            contractDoc.CollectDocDate = null;
                            contractDoc.CreateOfficeCode = CommonUtil.dsTransData.dtUserData.MainOfficeCode;

                            productList = masHandler.GetTbm_Product(registerData.ProductCode, null);

                            contractDoc.ProductCode = registerData.ProductCode;
                            if (productList != null && productList.Count > 0)
                            {
                                contractDoc.ProductNameEN = productList[0].ProductNameEN;
                                contractDoc.ProductNameLC = productList[0].ProductNameLC;
                            }
                            else
                            {
                                contractDoc.ProductNameEN = null;
                                contractDoc.ProductNameLC = null;
                            }


                            List<doMiscTypeCode> miscPhoneLineType = (from t in miscTypeList
                                                                      where t.FieldName == MiscType.C_PHONE_LINE_TYPE
                                                                      && t.ValueCode == registerData.PhoneLineTypeCode
                                                                      select t).ToList<doMiscTypeCode>();

                            contractDoc.PhoneLineTypeCode = registerData.PhoneLineTypeCode;
                            if (miscPhoneLineType != null && miscPhoneLineType.Count > 0)
                            {
                                contractDoc.PhoneLineTypeNameEN = miscPhoneLineType[0].ValueDisplayEN;
                                contractDoc.PhoneLineTypeNameLC = miscPhoneLineType[0].ValueDisplayLC;
                            }
                            else
                            {
                                contractDoc.PhoneLineTypeNameEN = null;
                                contractDoc.PhoneLineTypeNameLC = null;
                            }


                            List<doMiscTypeCode> miscPhoneLineOwnerType = (from t in miscTypeList
                                                                           where t.FieldName == MiscType.C_PHONE_LINE_OWNER_TYPE
                                                                           && t.ValueCode == registerData.PhoneLineOwnerTypeCode
                                                                           select t).ToList<doMiscTypeCode>();

                            contractDoc.PhoneLineOwnerTypeCode = registerData.PhoneLineOwnerTypeCode;
                            if (miscPhoneLineOwnerType != null && miscPhoneLineOwnerType.Count > 0)
                            {
                                contractDoc.PhoneLineOwnerTypeNameEN = miscPhoneLineOwnerType[0].ValueDisplayEN;
                                contractDoc.PhoneLineOwnerTypeNameLC = miscPhoneLineOwnerType[0].ValueDisplayLC;
                            }
                            else
                            {
                                contractDoc.PhoneLineOwnerTypeNameEN = null;
                                contractDoc.PhoneLineOwnerTypeNameLC = null;
                            }

                            //contractDoc.ContractFee = registerData.ContractFee;
                            //contractDoc.DepositFee = registerData.DepositFee;
                            #region Contract Fee

                            contractDoc.ContractFeeCurrencyType = registerData.ContractFeeCurrencyType;
                            if (contractDoc.ContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                contractDoc.ContractFee = null;
                                contractDoc.ContractFeeUsd = registerData.ContractFee;
                            }
                            else
                            {
                                contractDoc.ContractFee = registerData.ContractFee;
                                contractDoc.ContractFeeUsd = null;
                            }

                            #endregion
                            #region Deposit Fee

                            contractDoc.DepositFeeCurrencyType = registerData.DepositFeeCurrencyType;
                            if (contractDoc.DepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                contractDoc.DepositFee = null;
                                contractDoc.DepositFeeUsd = registerData.DepositFee;
                            }
                            else
                            {
                                contractDoc.DepositFee = registerData.DepositFee;
                                contractDoc.DepositFeeUsd = null;
                            }

                            #endregion

                            contractDoc.ContractFeePayMethod = registerData.ContractFeePayMethod;
                            contractDoc.CreditTerm = registerData.CreditTerm;
                            contractDoc.PaymentCycle = registerData.PaymentCycle;
                            contractDoc.FireSecurityFlag = registerData.FireSecurityFlag;
                            contractDoc.CrimePreventFlag = registerData.CrimePreventFlag;
                            contractDoc.EmergencyReportFlag = registerData.EmergencyReportFlag;
                            contractDoc.ContractDurationMonth = registerData.ContractDurationMonth;
                            contractDoc.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            contractDoc.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            contractDoc.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            contractDoc.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            dsContractDocDataData.dtTbt_ContractDocument.Add(contractDoc);
                        }

                        if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_MEMO
                            || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_NOTICE
                            || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO
                            || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO
                            || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO)
                        {
                            //contractDoc.DocID = null; => Auto-run
                            contractDoc.ContractDocOCC = strContractDocOCC;

                            if (String.IsNullOrEmpty(contractDoc.ContractCode) == false)
                            {
                                contractDoc.DocNo = String.Format("{0}-{1}-{2}", contractDoc.ContractCode, contractDoc.OCC, strContractDocOCC);
                            }
                            else
                            {
                                contractDoc.DocNo = String.Format("{0}-{1}-{2}", contractDoc.QuotationTargetCode, contractDoc.Alphabet, strContractDocOCC);
                            }

                            contractDoc.SECOMSignatureFlag = registerData.SECOMSignatureFlag;
                            contractDoc.EmpName = registerData.EmpName;
                            contractDoc.EmpPosition = registerData.EmpPosition;
                            contractDoc.RealCustomerNameLC = registerData.RealCustomerNameLC;
                            contractDoc.SiteAddressLC = registerData.SiteAddressLC;
                            contractDoc.SiteNameLC = registerData.SiteNameLC;
                            contractDoc.SiteNameEN = registerData.SiteNameEN;
                            contractDoc.IssuedDate = null;
                            contractDoc.DocEditFlag = "1";
                            contractDoc.DocEditor = CommonUtil.dsTransData.dtUserData.EmpNo;
                            contractDoc.DocEditDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            contractDoc.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_NOT_ISSUED;
                            contractDoc.DocAuditResult = DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED;
                            contractDoc.CollectDocDate = null;
                            contractDoc.CreateOfficeCode = CommonUtil.dsTransData.dtUserData.MainOfficeCode;

                            if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO)
                            {
                                contractDoc.ContractFeeCurrencyType = registerData.NewContractFeeCurrencyType;
                                if (registerData.NewContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    contractDoc.ContractFeeUsd = registerData.NewContractFee;
                                    contractDoc.ContractFee = null;   
                                }
                                else
                                {
                                    contractDoc.ContractFeeUsd = null;
                                contractDoc.ContractFee = registerData.NewContractFee;
                            }
                            }

                            contractDoc.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            contractDoc.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            contractDoc.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            contractDoc.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            dsContractDocDataData.dtTbt_ContractDocument.Add(contractDoc);
                        }

                        if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION)
                        {
                            //contractDoc.DocID = null; => Auto-run
                            contractDoc.ContractDocOCC = strContractDocOCC;

                            if (String.IsNullOrEmpty(contractDoc.ContractCode) == false)
                            {
                                contractDoc.DocNo = String.Format("{0}-{1}-{2}", contractDoc.ContractCode, contractDoc.OCC, strContractDocOCC);
                            }
                            else
                            {
                                contractDoc.DocNo = String.Format("{0}-{1}-{2}", contractDoc.QuotationTargetCode, contractDoc.Alphabet, strContractDocOCC);
                            }

                            contractDoc.SECOMSignatureFlag = registerData.SECOMSignatureFlag;
                            contractDoc.EmpName = registerData.EmpName;
                            contractDoc.EmpPosition = registerData.EmpPosition;
                            contractDoc.IssuedDate = null;
                            contractDoc.DocEditFlag = "1";
                            contractDoc.DocEditor = CommonUtil.dsTransData.dtUserData.EmpNo;
                            contractDoc.DocEditDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            contractDoc.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_NOT_ISSUED;
                            contractDoc.DocAuditResult = DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED;
                            contractDoc.CollectDocDate = null;
                            contractDoc.CreateOfficeCode = CommonUtil.dsTransData.dtUserData.MainOfficeCode;
                            contractDoc.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            contractDoc.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            contractDoc.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            contractDoc.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            dsContractDocDataData.dtTbt_ContractDocument.Add(contractDoc);
                        }
                    }

                    if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN
                            || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH)
                    {
                        //Set data in dtTbt_DocContractReport 
                        if (dtTbt_DocContractReport != null && dtTbt_DocContractReport.Count > 0)
                        {
                            dsContractDocDataData.dtTbt_DocContractReport = new List<tbt_DocContractReport>();
                            tbt_DocContractReport docContRpt = dtTbt_DocContractReport[0];

                            //docContRpt.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
                            docContRpt.PlanCode = registerData.PlanCode;
                            docContRpt.AutoRenewMonth = registerData.AutoRenewMonth;
                            docContRpt.DepositFeePhase = registerData.DepositFeePhase;


                            //docContRpt.InstallFee_ApproveContract = registerData.InstallFee_ApproveContract;
                            //docContRpt.InstallFee_CompleteInstall = registerData.InstallFee_CompleteInstall;
                            //docContRpt.InstallFee_StartService = registerData.InstallFee_StartService;
                            //docContRpt.NegotiationTotalInstallFee = registerData.NegotiationTotalInstallFee;

                            #region Install Fee Approve Contract

                            docContRpt.InstallFee_ApproveContractCurrencyType = registerData.NegotiationTotalInstallFeeCurrencyType;
                            if (docContRpt.InstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                docContRpt.InstallFee_ApproveContract = null;
                                docContRpt.InstallFee_ApproveContractUsd = registerData.InstallFee_ApproveContract;
                            }
                            else
                            {
                                docContRpt.InstallFee_ApproveContract = registerData.InstallFee_ApproveContract;
                                docContRpt.InstallFee_ApproveContractUsd = null;
                            }

                            #endregion
                            #region Install Fee Complete Install

                            docContRpt.InstallFee_CompleteInstallCurrencyType = registerData.InstallFee_CompleteInstallCurrencyType;
                            if (docContRpt.InstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                docContRpt.InstallFee_CompleteInstall = null;
                                docContRpt.InstallFee_CompleteInstallUsd = registerData.InstallFee_CompleteInstall;
                            }
                            else
                            {
                                docContRpt.InstallFee_CompleteInstall = registerData.InstallFee_CompleteInstall;
                                docContRpt.InstallFee_CompleteInstallUsd = null;
                            }

                            #endregion
                            #region Install Fee Start Service

                            docContRpt.InstallFee_StartServiceCurrencyType = registerData.InstallFee_StartServiceCurrencyType;
                            if (docContRpt.InstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                docContRpt.InstallFee_StartService = null;
                                docContRpt.InstallFee_StartServiceUsd = registerData.InstallFee_StartService;
                            }
                            else
                            {
                                docContRpt.InstallFee_StartService = registerData.InstallFee_StartService;
                                docContRpt.InstallFee_StartServiceUsd = null;
                            }

                            #endregion
                            #region Negotiation Total Install Fee 

                            docContRpt.NegotiationTotalInstallFeeCurrencyType = registerData.NegotiationTotalInstallFeeCurrencyType;
                            if (docContRpt.NegotiationTotalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                docContRpt.NegotiationTotalInstallFee = null;
                                docContRpt.NegotiationTotalInstallFeeUsd = registerData.NegotiationTotalInstallFee;
                            }
                            else
                            {
                                docContRpt.NegotiationTotalInstallFee = registerData.NegotiationTotalInstallFee;
                                docContRpt.NegotiationTotalInstallFeeUsd = null;
                            }

                            #endregion

                            docContRpt.CustomerSignatureName = registerData.CustomerSignatureName;
                            docContRpt.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docContRpt.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            docContRpt.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docContRpt.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            dsContractDocDataData.dtTbt_DocContractReport.Add(docContRpt);
                        }
                    }

                    if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_MEMO)
                    {
                        //Set data in dtTbt_DocChangeMemo
                        if (dtTbt_DocChangeMemo != null && dtTbt_DocChangeMemo.Count > 0)
                        {
                            dsContractDocDataData.dtTbt_DocChangeMemo = new List<tbt_DocChangeMemo>();
                            tbt_DocChangeMemo docChgMemo = dtTbt_DocChangeMemo[0];

                            //docChgMemo.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
                            docChgMemo.EffectiveDate = registerData.EffectiveDate;
                            docChgMemo.ChangeContent = registerData.ChangeContent;

                            #region Old Contract Fee

                            docChgMemo.OldContractFeeCurrencyType = registerData.OldContractFeeCurrencyType;
                            if (docChgMemo.OldContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                docChgMemo.OldContractFee = null;
                                docChgMemo.OldContractFeeUsd = registerData.OldContractFee;                                
                            }
                            else
                            {
                                docChgMemo.OldContractFee = registerData.OldContractFee;
                                docChgMemo.OldContractFeeUsd = null;
                            }

                            #endregion
                            #region New Contract Fee

                            docChgMemo.NewContractFeeCurrencyType = registerData.NewContractFeeCurrencyType;
                            if (docChgMemo.NewContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                docChgMemo.NewContractFeeUsd = registerData.NewContractFee;
                                docChgMemo.NewContractFee = null;
                            }
                            else
                            {
                                docChgMemo.NewContractFee = registerData.NewContractFee;
                                docChgMemo.NewContractFeeUsd = null;
                            }

                            #endregion

                            docChgMemo.CustomerSignatureName = registerData.CustomerSignatureName;
                            docChgMemo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docChgMemo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            docChgMemo.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docChgMemo.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            dsContractDocDataData.dtTbt_DocChangeMemo.Add(docChgMemo);
                        }
                    }

                    if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_NOTICE)
                    {
                        //Set data in dtTbt_DocChangeNotice
                        if (dtTbt_DocChangeNotice != null && dtTbt_DocChangeNotice.Count > 0)
                        {
                            dsContractDocDataData.dtTbt_DocChangeNotice = new List<tbt_DocChangeNotice>();
                            tbt_DocChangeNotice docChgNotice = dtTbt_DocChangeNotice[0];

                            //docChgNotice.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
                            docChgNotice.EffectiveDate = registerData.EffectiveDate;
                            docChgNotice.ChangeContent = registerData.ChangeContent;
                            docChgNotice.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docChgNotice.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            docChgNotice.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docChgNotice.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            dsContractDocDataData.dtTbt_DocChangeNotice.Add(docChgNotice);
                        }
                    }

                    if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO)
                    {
                        //Set data in dtTbt_DocConfirmCurrentInstrumentMemo
                        if (dtTbt_DocConfirmCurrentInstrumentMemo != null && dtTbt_DocConfirmCurrentInstrumentMemo.Count > 0)
                        {
                            dsContractDocDataData.dtTbt_DocConfirmCurrentInstrumentMemo = new List<tbt_DocConfirmCurrentInstrumentMemo>();
                            tbt_DocConfirmCurrentInstrumentMemo docConfirm = dtTbt_DocConfirmCurrentInstrumentMemo[0];

                            //docConfirm.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
                            docConfirm.RealInvestigationDate = registerData.RealInvestigationDate;
                            docConfirm.CustomerSignatureName = registerData.CustomerSignatureName;
                            docConfirm.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docConfirm.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            docConfirm.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docConfirm.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            dsContractDocDataData.dtTbt_DocConfirmCurrentInstrumentMemo.Add(docConfirm);
                        }
                    }

                    if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO
                            || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION)
                    {
                        //Set data in dtTbt_DocCancelContractMemo
                        if (dtTbt_DocCancelContractMemo != null && dtTbt_DocCancelContractMemo.Count > 0)
                        {
                            dsContractDocDataData.dtTbt_DocCancelContractMemo = new List<tbt_DocCancelContractMemo>();
                            tbt_DocCancelContractMemo docCancel = dtTbt_DocCancelContractMemo[0];

                            //docCancel.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
                            docCancel.CancelContractDate = registerData.CancelContractDate;
                            docCancel.StartServiceDate = registerData.StartServiceDate;
                            docCancel.TotalSlideAmt = sParam.TotalSlideAmount;
                            docCancel.TotalReturnAmt = sParam.TotalRefundAmount;
                            docCancel.TotalBillingAmt = sParam.TotalBillingAmount;
                            docCancel.TotalAmtAfterCounterBalance = sParam.TotalCounterBalAmount;

                            List<doMiscTypeCode> miscProcAfterCounterBalanceType = (from t in miscTypeList
                                                                                    where t.FieldName == MiscType.C_PROC_AFT_COUNTER_BALANCE_TYPE
                                                                                    && t.ValueCode == registerData.ProcessAfterCounterBalanceType
                                                                                    select t).ToList<doMiscTypeCode>();

                            docCancel.ProcessAfterCounterBalanceType = registerData.ProcessAfterCounterBalanceType;
                            if (miscProcAfterCounterBalanceType != null && miscProcAfterCounterBalanceType.Count > 0)
                                docCancel.ProcessAfterCounterBalanceTypeName = miscProcAfterCounterBalanceType[0].ValueDisplayLC;
                            else
                                docCancel.ProcessAfterCounterBalanceTypeName = null;

                            docCancel.ProcessAfterCounterBalanceTypeUsd = registerData.ProcessAfterCounterBalanceTypeUsd;

                            docCancel.AutoTransferBillingType = registerData.AutoTransferBillingType;
                            docCancel.AutoTransferBillingAmt = registerData.AutoTransferBillingAmt;

                            docCancel.BankTransferBillingType = registerData.BankTransferBillingType;
                            docCancel.BankTransferBillingAmt = registerData.BankTransferBillingAmt;

                            //docCancel.BankTransferBillingType = registerData.BankTransferBillingTypeUsd;
                            docCancel.BankTransferBillingAmtUsd = registerData.BankTransferBillingAmtUsd;

                            docCancel.CustomerSignatureName = registerData.CustomerSignatureName;
                            docCancel.OtherRemarks = registerData.OtherRemarks;
                            docCancel.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docCancel.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            docCancel.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docCancel.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            dsContractDocDataData.dtTbt_DocCancelContractMemo.Add(docCancel);
                        }

                        //Set data in dtTbt_DocCancelContractMemoDetail 
                        if (sParam.CancelContractMemoDetailTempData != null && sParam.CancelContractMemoDetailTempData.Count > 0)
                        {
                            List<tbt_DocCancelContractMemoDetail> docCancelContractMemoDetailList = new List<tbt_DocCancelContractMemoDetail>();
                            List<doMiscTypeCode> miscBillingType = new List<doMiscTypeCode>();
                            List<doMiscTypeCode> miscHandlingType = new List<doMiscTypeCode>();
                            foreach (CTS110_CancelContractMemoDetailTemp memoDetailTemp in sParam.CancelContractMemoDetailTempData)
                            {
                                tbt_DocCancelContractMemoDetail docMemoDetail = new tbt_DocCancelContractMemoDetail();
                                //docMemoDetail.DocCancelMemoID	=> Auto-run
                                //docMemoDetail.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()

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

                                docMemoDetail.NormalFeeAmountCurrencyType = memoDetailTemp.NormalFeeAmountCurrencyType;
                                docMemoDetail.NormalFeeAmount = memoDetailTemp.NormalFeeAmount;
                                docMemoDetail.NormalFeeAmountUsd = memoDetailTemp.NormalFeeAmountUsd;

                                docMemoDetail.FeeAmountCurrencyType = memoDetailTemp.FeeAmountCurrencyType;
                                docMemoDetail.FeeAmount = memoDetailTemp.FeeAmount;
                                docMemoDetail.FeeAmountUsd = memoDetailTemp.FeeAmountUsd;

                                docMemoDetail.TaxAmountCurrencyType = memoDetailTemp.TaxAmountCurrencyType;
                                docMemoDetail.TaxAmount = memoDetailTemp.TaxAmount;
                                docMemoDetail.TaxAmountUsd = memoDetailTemp.TaxAmountUsd;

                                docMemoDetail.ContractCode_CounterBalance = memoDetailTemp.ContractCode_CounterBalance;
                                docMemoDetail.Remark = memoDetailTemp.Remark;
                                docMemoDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                docMemoDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                docMemoDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                docMemoDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                docCancelContractMemoDetailList.Add(docMemoDetail);
                            }

                            dsContractDocDataData.dtTbt_DocCancelContractMemoDetail = docCancelContractMemoDetailList;
                        }
                    }

                    if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO)
                    {
                        //Set data in dtTbt_DocChangeFeeMemo
                        if (dtTbt_DocChangeFeeMemo != null && dtTbt_DocChangeFeeMemo.Count > 0)
                        {
                            dsContractDocDataData.dtTbt_DocChangeFeeMemo = new List<tbt_DocChangeFeeMemo>();
                            tbt_DocChangeFeeMemo docChgFee = new tbt_DocChangeFeeMemo();

                            //docChgFee.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
                            docChgFee.EffectiveDate = registerData.EffectiveDate;

                            #region Old Contract Fee
                            docChgFee.OldContractFeeCurrencyType = registerData.OldContractFeeCurrencyType;
                            if(docChgFee.OldContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                docChgFee.OldContractFeeUsd = registerData.OldContractFee;
                                docChgFee.OldContractFee = null;
                            }
                            else
                            {
                                docChgFee.OldContractFeeUsd = null;
                            docChgFee.OldContractFee = registerData.OldContractFee;
                            }

                            #endregion
                            #region New Contract Fee

                            docChgFee.NewContractFeeCurrencyType = registerData.NewContractFeeCurrencyType;
                            if (docChgFee.NewContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                docChgFee.NewContractFee = null;
                                docChgFee.NewContractFeeUsd = registerData.NewContractFee;                                
                            }
                            else
                            {
                                docChgFee.NewContractFee = registerData.NewContractFee;
                                docChgFee.NewContractFeeUsd = null;
                            }

                            #endregion

                            docChgFee.ChangeContractFeeDate = registerData.ChangeContractFeeDate;
                            docChgFee.ReturnToOriginalFeeDate = registerData.ReturnToOriginalFeeDate;
                            docChgFee.CustomerSignatureName = registerData.CustomerSignatureName;
                            docChgFee.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docChgFee.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            docChgFee.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            docChgFee.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            dsContractDocDataData.dtTbt_DocChangeFeeMemo.Add(docChgFee);
                        }
                    }

                    //Save contract document data
                    rentContHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    dsContractDocData dsContractDocResult = rentContHandler.CreateContractDocData(dsContractDocDataData);

                    scope.Complete();
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
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
        /// Set enable/disable [Select] button on Contract document result list at ‘Contract document list’ section 
        /// </summary>
        /// <param name="resultGridData"></param>
        private void SetEnableSelect_CTS180(List<CTS180_DocumentListGridData> resultGridData)
        {
            bool bIsEnable;
            foreach (CTS180_DocumentListGridData data in resultGridData)
            {
                bIsEnable = true;

                if ((data.ReportFlag == null || data.ReportFlag == FlagType.C_FLAG_OFF)
                    || (data.DocumentCode == DocumentCode.C_DOCUMENT_CODE_COVER_LETTER))
                {
                    bIsEnable = false;
                }

                if (CommonUtil.dsTransData.ContainsPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_EDIT) == false)
                {
                    bIsEnable = false;
                }

                data.IsEnableSelect = bIsEnable;
            }
        }

        /// <summary>
        /// Validate business for CancelContract
        /// </summary>
        /// <param name="res"></param>
        /// <param name="cancelContractMemoDetailData"></param>
        private void ValidateBusinessCancelContract_CTS180(ObjectResultData res, CTS180_CancelContractMemoDetailData cancelContractMemoDetailData)
        {
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            CommonUtil comUtil = new CommonUtil();
            ICommonContractHandler comContractHandler;

            string strCancelContractTypeCC = "CC";
            string strControlName1 = string.Empty;
            string strControlName2 = string.Empty;

            try
            {
                comContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;

                if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
                    || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE
                    || cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
                {
                    if (cancelContractMemoDetailData.StartPeriodDate > cancelContractMemoDetailData.EndPeriodDate)
                    {
                        strControlName1 = cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC ? "CC_PeriodFrom" : "QC_PeriodFrom";
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0101, null, new string[] { strControlName1 });
                        return;
                    }
                }

                CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                memoDetailTempList = sParam.CancelContractMemoDetailTempData;

                foreach (CTS110_CancelContractMemoDetailTemp memoDetail in memoDetailTempList)
                {
                    if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                    {
                        if (memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                        {
                            //Move to ValidateBusinessCancelContractForWarning_CTS180()
                            //strControlName1 = cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC ? "CC_PeriodFrom" : "QC_PeriodFrom";
                            //strControlName2 = cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC ? "CC_PeriodTo" : "QC_PeriodTo";

                            //if (cancelContractMemoDetailData.StartPeriodDate == memoDetail.StartPeriodDate
                            //    && cancelContractMemoDetailData.EndPeriodDate == memoDetail.EndPeriodDate)
                            //{
                            //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3108, null, new string[] { strControlName1, strControlName2 });
                            //    return;
                            //}

                            //if (CheckOverlapPeriodDate_CTS110(memoDetail.StartPeriodDate, memoDetail.EndPeriodDate, cancelContractMemoDetailData.StartPeriodDate, cancelContractMemoDetailData.EndPeriodDate))
                            //{
                            //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null, new string[] { strControlName1, strControlName2 });
                            //    return;
                            //}
                        }

                        if (memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                        {
                            strControlName1 = cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC ? "CC_FeeType" : "QC_FeeType";
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3109, null, new string[] { strControlName1 });
                            return;
                        }
                    }
                    else if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                    {
                        if (memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                        {
                            //Move to ValidateBusinessCancelContractForWarning_CTS180()
                            //strControlName1 = cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC ? "CC_PeriodFrom" : "QC_PeriodFrom";
                            //strControlName2 = cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC ? "CC_PeriodTo" : "QC_PeriodTo";

                            //if (cancelContractMemoDetailData.StartPeriodDate == memoDetail.StartPeriodDate
                            //    && cancelContractMemoDetailData.EndPeriodDate == memoDetail.EndPeriodDate)
                            //{
                            //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3110, null, new string[] { strControlName1, strControlName2 });
                            //    return;
                            //}

                            //if (CheckOverlapPeriodDate_CTS110(memoDetail.StartPeriodDate, memoDetail.EndPeriodDate, cancelContractMemoDetailData.StartPeriodDate, cancelContractMemoDetailData.EndPeriodDate))
                            //{
                            //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null, new string[] { strControlName1, strControlName2 });
                            //    return;
                            //}
                        }

                        if (memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                        {
                            strControlName1 = cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC ? "CC_FeeType" : "QC_FeeType";
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3111, null, new string[] { strControlName1 });
                            return;
                        }
                    }
                    else if (cancelContractMemoDetailData.BillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
                    {
                        if (cancelContractMemoDetailData.BillingType == memoDetail.BillingType)
                        {
                            strControlName1 = cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC ? "CC_FeeType" : "QC_FeeType";
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null, new string[] { strControlName1 });
                            return;
                        }
                    }
                }

                if (String.IsNullOrEmpty(cancelContractMemoDetailData.ContractCode_CounterBalance) == false)
                {
                    string strContractCode_CounterBalanceLong = comUtil.ConvertContractCode(cancelContractMemoDetailData.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_LONG);
                    if (comContractHandler.IsContractExistInRentalOrSale(strContractCode_CounterBalanceLong) == false)
                    {
                        strControlName1 = cancelContractMemoDetailData.CancelContractType == strCancelContractTypeCC ? "CC_ContractCodeForSlideFee" : "QC_ContractCodeForSlideFee";
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, new string[] { cancelContractMemoDetailData.ContractCode_CounterBalance }, new string[] { strControlName1 });
                        return;
                    }

                    //Bug report CT-148
                    if (sParam.dtTbt_ContractDocument != null && sParam.dtTbt_ContractDocument.Count > 0)
                    {
                        if (sParam.dtTbt_ContractDocument[0].ContractCode == strContractCode_CounterBalanceLong.ToUpper())
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281, null, new string[] { strControlName1 });
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
        /// Validate business for CancelContract (Warning)
        /// </summary>
        /// <param name="res"></param>
        /// <param name="cancelContractMemoDetailData"></param>
        private void ValidateBusinessCancelContractForWarning_CTS180(ObjectResultData res, CTS180_CancelContractMemoDetailData cancelContractMemoDetailData)
        {
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            string strCancelContractTypeCC = "CC";
            string strControlName1 = string.Empty;
            string strControlName2 = string.Empty;

            try
            {
                CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                memoDetailTempList = sParam.CancelContractMemoDetailTempData;

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
        /// <param name="registerData"></param>
        /// <param name="strDocumentCode"></param>
        private void ValidateBusiness_CTS180(ObjectResultData res, CTS180_RegisterContractDocumentData registerData, string strDocumentCode)
        {
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            decimal maximumDigit = 999999999999.99M;
            decimal minimumDigit = -999999999999.99M;

            try
            {
                decimal? checkSum = 0;
                if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN
                    || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH)
                {
                    //Sum Amount
                    if(registerData.InstallFee_ApproveContract > 0)
                    {
                        if(registerData.NegotiationTotalInstallFeeCurrencyType == registerData.InstallFee_ApproveContractCurrencyType)
                            checkSum = checkSum + registerData.InstallFee_ApproveContract;
                    }
                    if (registerData.InstallFee_CompleteInstall > 0)
                    {
                        if (registerData.NegotiationTotalInstallFeeCurrencyType == registerData.InstallFee_CompleteInstallCurrencyType)
                            checkSum = checkSum + registerData.InstallFee_CompleteInstall;
                    }
                    if (registerData.InstallFee_StartService > 0)
                    {
                        if (registerData.NegotiationTotalInstallFeeCurrencyType == registerData.InstallFee_StartServiceCurrencyType)
                            checkSum = checkSum + registerData.InstallFee_StartService;
                    }

                    if((registerData.NegotiationTotalInstallFeeCurrencyType != registerData.InstallFee_ApproveContractCurrencyType)
                        && (registerData.InstallFee_ApproveContract > 0)) {}
                    else if ((registerData.NegotiationTotalInstallFeeCurrencyType != registerData.InstallFee_CompleteInstallCurrencyType)
                        && (registerData.InstallFee_CompleteInstall > 0)) {}
                    else if ((registerData.NegotiationTotalInstallFeeCurrencyType != registerData.InstallFee_StartServiceCurrencyType)
                        && (registerData.InstallFee_StartService > 0)) {}
                    else if (registerData.NegotiationTotalInstallFee != checkSum)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3088);
                        return;
                    }

                    if(registerData.NegotiationTotalInstallFeeCurrencyType != registerData.InstallFee_ApproveContractCurrencyType)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3316);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                    if (registerData.NegotiationTotalInstallFeeCurrencyType != registerData.InstallFee_CompleteInstallCurrencyType)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3317);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                    if (registerData.NegotiationTotalInstallFeeCurrencyType != registerData.InstallFee_StartServiceCurrencyType)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3318);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                if (registerData.DepositFee > 0 && registerData.DepositFeePhase == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3090);
                        return;
                }
                // else if (strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO
                //    || strDocumentCode == DocumentCode.C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION)
                //{
                //    CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                //    if (sParam.dtTbt_DocCancelContractMemoDetail == null || sParam.dtTbt_DocCancelContractMemoDetail.Count == 0)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3113);
                //        return;
                //    }
                //}

                CTS180_ScreenParameter sParam = GetScreenObject<CTS180_ScreenParameter>();
                string strTotalLabel = string.Empty;
                if (sParam.TotalSlideAmount > maximumDigit || sParam.TotalSlideAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalSlideAmountRp";
                }
                if (sParam.TotalSlideAmountUsd > maximumDigit || sParam.TotalSlideAmountUsd < minimumDigit)
                {
                    strTotalLabel = "lblTotalSlideAmountUsd";
                }
                else if (sParam.TotalRefundAmount > maximumDigit || sParam.TotalRefundAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalRefundAmountRp";
                }
                else if (sParam.TotalRefundAmountUsd > maximumDigit || sParam.TotalRefundAmountUsd < minimumDigit)
                {
                    strTotalLabel = "lblTotalRefundAmountUsd";
                }
                else if (sParam.TotalBillingAmount > maximumDigit || sParam.TotalBillingAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalBillingAmountRp";
                }
                else if (sParam.TotalBillingAmountUsd > maximumDigit || sParam.TotalBillingAmountUsd < minimumDigit)
                {
                    strTotalLabel = "lblTotalBillingAmountUsd";
                }
                else if (sParam.TotalCounterBalAmount > maximumDigit || sParam.TotalCounterBalAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalCounterBalAmountRp";
                }
                else if (sParam.TotalCounterBalAmountUsd > maximumDigit || sParam.TotalCounterBalAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalCounterBalAmountUsd";
                }

                if (String.IsNullOrEmpty(strTotalLabel) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_MAINTAIN_CONTRACT_DOCUMENT,
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
