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
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Inventory;

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
        public ActionResult CTS110_Authority(CTS110_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            string strContractCodeLong = string.Empty;
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;
            CTS110_doRentalContractBasicAuthority doRentalContractBasicAuthority;

            IInstallationHandler installHandler;
            tbt_InstallationBasic dtTbt_InstallationBasicData;
            List<tbt_InstallationBasic> dtTbt_InstallationBasicList;

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                /*--- HasAuthority ---*/
                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, FunctionID.C_FUNC_ID_OPERATE) == false)
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
                    //                    ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT,
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
                    dsRentalContract = CheckDataAuthority_CTS110(res, strContractCodeLong, true);
                    if (res.IsError)
                        return Json(res);

                    //Check data authority
                    //If Open screen from “Before” menu
                    if (sParam.SubObjectID == "0")
                    {
                        if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == dsRentalContract.dtTbt_RentalContractBasic[0].ContractOfficeCode; }).Count() == 0
                            && CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == dsRentalContract.dtTbt_RentalContractBasic[0].OperationOfficeCode; }).Count() == 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                            return Json(res);
                        }
                    }
                    //If Open screen from “After” menu
                    else if (sParam.SubObjectID == "1")
                    {
                        doRentalContractBasicAuthority = CommonUtil.CloneObject<tbt_RentalContractBasic, CTS110_doRentalContractBasicAuthority>(dsRentalContract.dtTbt_RentalContractBasic[0]);
                        ValidatorUtil.BuildErrorMessage(res, new object[] { doRentalContractBasicAuthority }, null, false);
                        if (res.IsError)
                            return Json(res);
                    }
                }
                /*-------------------------*/


                //1.8.1.1	If Open screen from “Before” menu
                if (sParam.SubObjectID == "0")
                {
                    if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3236);
                        return Json(res);
                    }
                }
                //1.8.1.2	If Open screen from “After” menu
                else if (sParam.SubObjectID == "1")
                {
                    if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3237);
                        return Json(res);
                    }
                }

                //1.8.1.3
                if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3104);
                    return Json(res);
                }

                //1.8.1.4
                if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL
                    || dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_END
                    || dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3105, new string[] { sParam.ContractCode });
                    return Json(res);
                }

                //1.8.1.5
                if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                {
                    //Check existing of unimplemented rental contract data
                    rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    string strOCCout = rentralHandler.GetLastUnimplementedOCC(strContractCodeLong);
                    if (String.IsNullOrEmpty(strOCCout) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3106);
                        return Json(res);
                    }
                }

                //Get installation basic data
                installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                dtTbt_InstallationBasicList = installHandler.GetTbt_InstallationBasicData(strContractCodeLong);
                if (dtTbt_InstallationBasicList != null && dtTbt_InstallationBasicList.Count > 0)
                {
                    dtTbt_InstallationBasicData = dtTbt_InstallationBasicList[0];

                    //if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
                    //    && 
                    if (dtTbt_InstallationBasicData != null
                        && dtTbt_InstallationBasicData.InstallationStatus != InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED
                        && (dtTbt_InstallationBasicData.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                            && dtTbt_InstallationBasicData.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                            && dtTbt_InstallationBasicData.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL)
                        )
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3103, new string[] { sParam.ContractCode });
                        return Json(res);
                    }
                }

                //string strDefaultRemovalFee = "0";
                //if (dtTbt_InstallationBasicData != null
                //        && dtTbt_InstallationBasicData.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL)
                //{
                //    if (dtTbt_InstallationBasicData.NormalInstallFee != null)
                //    {
                //        strDefaultRemovalFee = dtTbt_InstallationBasicData.NormalInstallFee.Value.ToString("#,##0.00");
                //    }
                //}
                
                //decimal? decDefaultRemovalFee = installHandler.GetNormalRemovalFee(strContractCodeLong);
                List<doGetRemovalData> doGetRemovalDataList = installHandler.GetRemovalData(strContractCodeLong);

                //sParam = new CTS110_ScreenParameter();
                sParam.CTS110_Session = new CTS110_RegisterRentalContractTargetData();
                sParam.CTS110_Session.InitialData = new CTS110_InitialRegisterRentalContractTargetData();
                sParam.CTS110_Session.InitialData.ContractCode = strContractCodeLong;
                sParam.CTS110_Session.InitialData.RentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];

                //sParam.CTS110_Session.InitialData.DefaultRemovalFee = decDefaultRemovalFee;
                sParam.CTS110_Session.InitialData.RemovalDataList = doGetRemovalDataList;

                sParam.CTS110_Session.RegisterRentalContractData = dsRentalContract;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<CTS110_ScreenParameter>("CTS110", sParam, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS110")]
        public ActionResult CTS110()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            IUserControlHandler userCtrlHandler;
            IRentralContractHandler rentralHandler;
            doRentalContractBasicInformation doRentalContractBasic;
            dsCancelContractQuotation dsCancelContractQuotationData;

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                if (sParam != null)
                {
                    string strContractCodeLong = sParam.CTS110_Session.InitialData.ContractCode;

                    //Get cancel contract quotation data
                    rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    dsCancelContractQuotationData = rentralHandler.GetLastCancelContractQuotation(strContractCodeLong);

                    userCtrlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                    doRentalContractBasic = userCtrlHandler.GetRentalContactBasicInformationData(strContractCodeLong);

                    ViewBag.ContractStatus = sParam.CTS110_Session.InitialData.RentalContractBasicData.ContractStatus;
                    
                    //ViewBag.DefaultRemovalFee = sParam.CTS110_Session.InitialData.DefaultRemovalFee;
                    if (sParam.CTS110_Session.InitialData.RemovalDataList != null && sParam.CTS110_Session.InitialData.RemovalDataList.Count > 0)
                    {
                        #region Normal Install Fee

                        ViewBag.DefaultNormalRemovalFeeCurrencyType = sParam.CTS110_Session.InitialData.RemovalDataList[0].NormalInstallFeeCurrencyType;

                        if (sParam.CTS110_Session.InitialData.RemovalDataList[0].NormalInstallFeeCurrencyType == 
                            SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.DefaultNormalRemovalFee = sParam.CTS110_Session.InitialData.RemovalDataList[0].NormalInstallFeeUsd;
                        else
                            ViewBag.DefaultNormalRemovalFee = sParam.CTS110_Session.InitialData.RemovalDataList[0].NormalInstallFee;

                        #endregion
                        #region Order Install Fee

                        ViewBag.DefaultOrderRemovalFeeCurrencyType = sParam.CTS110_Session.InitialData.RemovalDataList[0].OrderInstallFeeCurrencyType;

                        if (sParam.CTS110_Session.InitialData.RemovalDataList[0].OrderInstallFeeCurrencyType ==
                            SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.DefaultOrderRemovalFee = sParam.CTS110_Session.InitialData.RemovalDataList[0].OrderInstallFeeUsd;
                        else
                            ViewBag.DefaultOrderRemovalFee = sParam.CTS110_Session.InitialData.RemovalDataList[0].OrderInstallFee;

                        #endregion

                        ViewBag.InstallationType = sParam.CTS110_Session.InitialData.RemovalDataList[0].InstallationType;
                    }
                    else
                    {
                        ViewBag.DefaultNormalRemovalFee = 0;
                        ViewBag.DefaultOrderRemovalFee = 0;
                        ViewBag.InstallationType = null;
                    }

                    //Map data to screen
                    Bind_CTS110(doRentalContractBasic);

                    bool isShowRemovalSection = false;
                    decimal? decRemovalFeeAmount = 0;
                    List<tbt_CancelContractMemoDetail> cancelMomoDetailList = dsCancelContractQuotationData.dtTbt_CancelContractMemoDetail;
                    if (cancelMomoDetailList != null)
                    {
                        List<tbt_CancelContractMemoDetail> cancelMomoDetailRemovalList = cancelMomoDetailList.FindAll(delegate(tbt_CancelContractMemoDetail s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; });
                        
                        //if (sParam.CTS110_Session.InitialData.DefaultRemovalFee > 0 
                        if ((sParam.CTS110_Session.InitialData.RemovalDataList != null && sParam.CTS110_Session.InitialData.RemovalDataList.Count > 0)
                            || (cancelMomoDetailRemovalList != null && cancelMomoDetailRemovalList.Count > 0 && cancelMomoDetailRemovalList[0].FeeAmount == 0)
                            || (sParam.CTS110_Session.InitialData.RentalContractBasicData != null && sParam.CTS110_Session.InitialData.RentalContractBasicData.FirstInstallCompleteFlag != true)) //Add by Jutarat A. on 24072012
                        {
                            isShowRemovalSection = false;
                            cancelMomoDetailList = (from t in dsCancelContractQuotationData.dtTbt_CancelContractMemoDetail
                                                    where t.BillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE
                                                    select t).ToList<tbt_CancelContractMemoDetail>();
                        }
                        else if (sParam.CTS110_Session.InitialData.RemovalDataList != null && sParam.CTS110_Session.InitialData.RemovalDataList.Count > 0
                                && sParam.CTS110_Session.InitialData.RemovalDataList[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL)
                        {
                            isShowRemovalSection = false;
                        }
                        else if (cancelMomoDetailRemovalList != null && cancelMomoDetailRemovalList.Count > 0)
                        {
                            isShowRemovalSection = true;
                            decRemovalFeeAmount = cancelMomoDetailRemovalList[0].FeeAmount;
                        }
                    }
                    ViewBag.ShowRemovalSection = isShowRemovalSection;
                    ViewBag.RemovalFeeAmount = decRemovalFeeAmount;

                    //Add by Jutarat A. on 19072012
                    if (sParam.CTS110_Session.InitialData.RentalContractBasicData != null)
                    {
                        ViewBag.ProductTypeCode = sParam.CTS110_Session.InitialData.RentalContractBasicData.ProductTypeCode;
                        ViewBag.FirstInstallCompleteFlag = sParam.CTS110_Session.InitialData.RentalContractBasicData.FirstInstallCompleteFlag;
                    }
                    else
                    {
                        ViewBag.ProductTypeCode = null;
                        ViewBag.FirstInstallCompleteFlag = null;
                    }
                    //End Add

                    dsCancelContractQuotationData.dtTbt_CancelContractMemoDetail = cancelMomoDetailList;

                    sParam.CTS110_Session.InitialData.CancelContractQuotationData = dsCancelContractQuotationData;
                    sParam.CTS110_Session.InitialData.doRentalContractBasicData = doRentalContractBasic;
                    UpdateScreenObject(sParam);
                }

                bool isP1 = true;

                #if !ROUND1
                isP1 = false;
                #endif

                ViewBag.IsP1 = isP1;

                return View();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get PaymentMethod data to ComboBox
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CTS110_GetComboBoxPaymentMethod(string id)
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

            return Json(CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, "ValueCodeDisplay", "ValueCode", new { style = "width:195px" }, false).ToString());
        }

        /// <summary>
        /// Get detail of CancelContract data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS110_GetCancelContractConditionDetail()
        {
            ObjectResultData res = new ObjectResultData();
            dsCancelContractQuotation dsCancelContractQuotationData = new dsCancelContractQuotation();
            List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;
            CommonUtil comUtil = new CommonUtil();

            //string lblNormalFee = "Normal fee";
            //string lblContractCodeForSlideFee = "Contract code for slide fee";

            try
            {
                string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblNormalFee");
                string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblContractCodeForSlideFee");

                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                dsCancelContractQuotationData = sParam.CTS110_Session.InitialData.CancelContractQuotationData;
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

                        DataEntity.Common.doMiscTypeCode curr = null;
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
                            if (memoDetail.FeeAmountCurrencyType == null)
                                memoDetail.FeeAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                            if (memoDetail.TaxAmountCurrencyType == null)
                                memoDetail.TaxAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;

                            #region Fee

                            curr = currencies.Find(x => x.ValueCode == memoDetail.FeeAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];

                            if (memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                if (memoDetail.FeeAmountUsd != null)
                                    data.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.FeeAmountUsd.Value));
                            }
                            else
                            {
                                if (memoDetail.FeeAmount != null)
                                    data.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.FeeAmount.Value));
                            }
                            
                            #endregion
                            #region Tax

                            curr = currencies.Find(x => x.ValueCode == memoDetail.TaxAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];

                            if (memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                if (memoDetail.TaxAmountUsd != null)
                                    data.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.TaxAmountUsd.Value));
                            }
                            else
                            {
                                if (memoDetail.TaxAmount != null)
                                    data.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.TaxAmount.Value));
                            }

                            #endregion
                        }

                        data.Period = String.Format("{0} {1}", strStartPeriodDate, strEndPeriodDate);

                        //data.Remark = String.Format("{0} {1} {2}", memoDetail.Remark
                        //    , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, strContractCodeCounterBal)
                        //    , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount)));

                        string txtNormalFeeAmount = "";
                        if (currencies != null)
                        {
                            curr = currencies.Find(x => x.ValueCode == memoDetail.NormalFeeAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];

                            if (memoDetail.NormalFeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                if (memoDetail.NormalFeeAmountUsd != null)
                                    txtNormalFeeAmount = string.Format("{0}: {1} {2}", lblNormalFee, curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.NormalFeeAmountUsd.Value));
                            }
                            else
                            {
                                if (memoDetail.NormalFeeAmount != null)
                                    txtNormalFeeAmount = string.Format("{0}: {1} {2}", lblNormalFee, curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value));
                            }
                        }

                        data.Remark = String.Format("{0}{1}{2}", string.IsNullOrEmpty(memoDetail.Remark) ? string.Empty : string.Format("{0}<br/>", memoDetail.Remark)
                            , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("{0}: {1}<br/>", lblContractCodeForSlideFee, strContractCodeCounterBal)
                            , txtNormalFeeAmount);

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

                sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData = memoDetailTempList;
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
        /// Get Removal InstallationFee data
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS110_GetRemovalInstallationFee()
        {
            ObjectResultData res = new ObjectResultData();

            //string lblRemovalFee = "Removal installation fee";
            string lblRemovalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblRemovalInstallationFee");

            try
            {
                List<CTS110_RemovalInstallationFeeGridData> gridData = new List<CTS110_RemovalInstallationFeeGridData>();
                CTS110_RemovalInstallationFeeGridData data = new CTS110_RemovalInstallationFeeGridData();
                data.InstallationFee = lblRemovalFee;
                data.Amount = 0;
                data.PaymentMethod = string.Empty;
                gridData.Add(data);

                res.ResultData = CommonUtil.ConvertToXml<CTS110_RemovalInstallationFeeGridData>(gridData, "Contract\\CTS110_RemovalInstallationFee", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get Removal Fee of BillingTarget data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS110_GetRemovalFeeBillingTarget()
        {
            ObjectResultData res = new ObjectResultData();
            ICommonContractHandler commonHandler;
            IBillingMasterHandler billMasterHandler;
            IBillingInterfaceHandler billInterfaceHandler;
            List<CTS110_BillingClientData> dtBillingClientList = new List<CTS110_BillingClientData>();
            List<CTS110_BillingTargetData> dtBillingTargetList = new List<CTS110_BillingTargetData>();
            List<CTS110_BillingTemp> billingTempList = new List<CTS110_BillingTemp>();

            try
            {
                commonHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                billMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                billInterfaceHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

                CommonUtil comUtil = new CommonUtil();

                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                tbt_RentalContractBasic rentalContractBasicData = sParam.CTS110_Session.InitialData.RentalContractBasicData;
                List<tbt_BillingTemp> dtBillingTemp = commonHandler.GetTbt_BillingTargetForEditing(rentalContractBasicData.ContractCode, rentalContractBasicData.LastOCC);

                int intSequence = -1;
                List<CTS110_RemovalInstallationFeeGridData> gridDataList = new List<CTS110_RemovalInstallationFeeGridData>();
                foreach (tbt_BillingTemp dataTemp in dtBillingTemp)
                {
                    string strBillingClientCodeShort = string.Empty;
                    string strFullNameEN = string.Empty;
                    string strFullNameLC = string.Empty;

                    //Set BillingClient Data
                    CTS110_BillingClientData billingClient = new CTS110_BillingClientData();
                    List<dtBillingClientData> billingClientList = billMasterHandler.GetBillingClient(dataTemp.BillingClientCode);
                    if (billingClientList != null && billingClientList.Count > 0)
                        billingClient = CommonUtil.CloneObject<dtBillingClientData, CTS110_BillingClientData>(billingClientList[0]);

                    billingClient.Sequence = (intSequence + 1).ToString();
                    billingClient.Status = "";
                    dtBillingClientList.Add(billingClient);

                    strBillingClientCodeShort = billingClient.BillingClientCodeShort;
                    strFullNameEN = billingClient.FullNameEN;
                    strFullNameLC = billingClient.FullNameLC;

                    //Set BillingTarget Data
                    CTS110_BillingTargetData billingTarget = new CTS110_BillingTargetData();
                    List<tbt_BillingTarget> dtBillingTarget = billInterfaceHandler.GetBillingTarget(dataTemp.BillingTargetCode);
                    if (dtBillingTarget != null && dtBillingTarget.Count > 0)
                        billingTarget = CommonUtil.CloneObject<tbt_BillingTarget, CTS110_BillingTargetData>(dtBillingTarget[0]);

                    billingTarget.Sequence = (intSequence + 1).ToString();
                    billingTarget.Status = "";
                    dtBillingTargetList.Add(billingTarget);

                    //Set RemovalInstallationFee GridData
                    CTS110_RemovalInstallationFeeGridData data = new CTS110_RemovalInstallationFeeGridData();
                    data.BillingOCC = dataTemp.BillingOCC;
                    data.BillingClientCode = strBillingClientCodeShort; //dataTemp.BillingClientCode;
                    data.BillingOfficeCode = dataTemp.BillingOfficeCode;
                    data.BillingOfficeName = GetBillingOfficeName_CTS110(dataTemp.BillingOfficeCode);

                    //data.BillingTargetCode = dataTemp.BillingTargetCode;
                    data.BillingTargetCode = comUtil.ConvertBillingTargetCode(dataTemp.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                    data.BillingTargetName = string.Format("(1) {0} <br/>(2) {1}", strFullNameEN, strFullNameLC);
                    data.Sequence = (intSequence + 1).ToString();
                    data.Status = "";
                    gridDataList.Add(data);

                    CTS110_BillingTemp billTemp = CommonUtil.CloneObject<tbt_BillingTemp, CTS110_BillingTemp>(dataTemp);
                    billTemp.Sequence = (intSequence + 1).ToString();
                    billTemp.Status = "";
                    billingTempList.Add(billTemp);

                    intSequence++;
                }

                sParam.CTS110_Session.InitialData.BillingClientData = dtBillingClientList;
                sParam.CTS110_Session.InitialData.BillingTargetData = dtBillingTargetList;
                sParam.CTS110_Session.InitialData.BillingTempData = billingTempList;
                UpdateScreenObject(sParam);

                res.ResultData = CommonUtil.ConvertToXml<CTS110_RemovalInstallationFeeGridData>(gridDataList, "Contract\\CTS110_RemovalFeeBillingTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW);
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
        public ActionResult CTS110_GetCancelContractCondition()
        {
            ObjectResultData res = new ObjectResultData();
            dsCancelContractQuotation dsCancelContractQuotationData;
            List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                dsCancelContractQuotationData = sParam.CTS110_Session.InitialData.CancelContractQuotationData;

                if (dsCancelContractQuotationData.dtTbt_CancelContractMemo != null && dsCancelContractQuotationData.dtTbt_CancelContractMemo.Count > 0)
                {
                    tbt_CancelContractMemo cancelContractMemoData = CommonUtil.CloneObject<tbt_CancelContractMemo, tbt_CancelContractMemo>(dsCancelContractQuotationData.dtTbt_CancelContractMemo[0]);
                    CTS110_CancelContractConditionGridData memoData = new CTS110_CancelContractConditionGridData();
                    //memoData.TotalSlideAmt = cancelContractMemoData.TotalSlideAmt == null ? string.Empty : cancelContractMemoData.TotalSlideAmt.Value.ToString("#,##0.00");
                    //memoData.TotalReturnAmt = cancelContractMemoData.TotalReturnAmt == null ? string.Empty : cancelContractMemoData.TotalReturnAmt.Value.ToString("#,##0.00");
                    //memoData.TotalBillingAmt = cancelContractMemoData.TotalBillingAmt == null ? string.Empty : cancelContractMemoData.TotalBillingAmt.Value.ToString("#,##0.00");
                    //memoData.TotalAmtAfterCounterBalance = cancelContractMemoData.TotalAmtAfterCounterBalance == null ? string.Empty : cancelContractMemoData.TotalAmtAfterCounterBalance.Value.ToString("#,##0.00");

                    memoData.ProcessAfterCounterBalanceType = cancelContractMemoData.ProcessAfterCounterBalanceType;
                    memoData.ProcessAfterCounterBalanceTypeUsd = cancelContractMemoData.ProcessAfterCounterBalanceTypeUsd;

                    memoData.OtherRemarks = cancelContractMemoData.OtherRemarks;

                    CTS110_CalculateTotalAmount();

                    sParam = GetScreenObject<CTS110_ScreenParameter>();
                    memoData.TotalSlideAmt = CommonUtil.TextNumeric(sParam.CTS110_Session.InitialData.TotalSlideAmount);
                    memoData.TotalReturnAmt = CommonUtil.TextNumeric(sParam.CTS110_Session.InitialData.TotalRefundAmount);
                    memoData.TotalBillingAmt = CommonUtil.TextNumeric(sParam.CTS110_Session.InitialData.TotalBillingAmount);
                    memoData.TotalAmtAfterCounterBalance = CommonUtil.TextNumeric(sParam.CTS110_Session.InitialData.TotalCounterBalAmount);

                    memoData.TotalSlideAmtUsd = CommonUtil.TextNumeric(sParam.CTS110_Session.InitialData.TotalSlideAmountUsd);
                    memoData.TotalReturnAmtUsd = CommonUtil.TextNumeric(sParam.CTS110_Session.InitialData.TotalRefundAmountUsd);
                    memoData.TotalBillingAmtUsd = CommonUtil.TextNumeric(sParam.CTS110_Session.InitialData.TotalBillingAmountUsd);
                    memoData.TotalAmtAfterCounterBalanceUsd = CommonUtil.TextNumeric(sParam.CTS110_Session.InitialData.TotalCounterBalAmountUsd);

                    res.ResultData = memoData;
                }

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
        public ActionResult CTS110_ValidateAddCancelContractData(tbt_CancelContractMemoDetail cancelContractMemoDetailData)
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
                    ValidateBusinessCancelContract_CTS110(res, cancelContractMemoDetailData);
                    if (res.IsError)
                        return Json(res);

                    //ValidateBusinessForWarning
                    ValidateBusinessCancelContractForWarning_CTS110(res, cancelContractMemoDetailData);

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
        public ActionResult CTS110_AddCancelContractData(tbt_CancelContractMemoDetail cancelContractMemoDetailData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;
            CommonUtil comUtil = new CommonUtil();

            //string lblNormalFee = "Normal fee";
            //string lblContractCodeForSlideFee = "Contract code for slide fee";
            string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblNormalFee");
            string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblContractCodeForSlideFee");

            try
            {
                if (cancelContractMemoDetailData != null)
                {
                    #region Move to CTS110_ValidateAddCancelContractData()
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
                    //ValidateBusinessCancelContract_CTS110(res, cancelContractMemoDetailData);
                    //if (res.IsError)
                    //    return Json(res);

                    ////ValidateBusinessForWarning
                    //ValidateBusinessCancelContractForWarning_CTS110(res, cancelContractMemoDetailData);
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

                    CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                    memoDetailTempList = sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData;

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

                    //gridData.Fee = CommonUtil.TextNumeric(memoDetail.FeeAmount); //memoDetail.FeeAmount;
                    //gridData.Tax = CommonUtil.TextNumeric(memoDetail.TaxAmount); //memoDetail.TaxAmount;

                    DataEntity.Common.doMiscTypeCode curr = null;
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

                        curr = currencies.Find(x => x.ValueCode == memoDetail.FeeAmountCurrencyType);
                        if (curr == null)
                            curr = currencies[0];

                        if (memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            if (memoDetail.FeeAmountUsd != null)
                                gridData.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.FeeAmountUsd.Value));
                        }
                        else
                        {
                            if (memoDetail.FeeAmount != null)
                                gridData.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.FeeAmount.Value));
                        }
                                                
                        #endregion
                        #region Tax

                        curr = currencies.Find(x => x.ValueCode == memoDetail.TaxAmountCurrencyType);
                        if (curr == null)
                            curr = currencies[0];

                        if (memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            if (memoDetail.TaxAmountUsd != null)
                                gridData.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.TaxAmountUsd.Value));
                        }
                        else
                        {
                            if (memoDetail.TaxAmount != null)
                                gridData.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.TaxAmount.Value));
                        }
                                                
                        #endregion
                    }

                    gridData.Period = String.Format("{0} {1}", strStartDate, memoDetail.EndPeriodDate == null ? string.Empty : string.Format("TO {0}", strEndDate));

                    //gridData.Remark = String.Format("{0} {1} {2}", memoDetail.Remark
                    //                , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                    //                , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount)));

                    string txtNormalFeeAmount = "";
                    if (currencies != null)
                    {
                        curr = currencies.Find(x => x.ValueCode == memoDetail.NormalFeeAmountCurrencyType);
                        if (curr == null)
                            curr = currencies[0];

                        if (memoDetail.NormalFeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            if (memoDetail.NormalFeeAmountUsd != null)
                                txtNormalFeeAmount = string.Format("{0}: {1} {2}", lblNormalFee, curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.NormalFeeAmountUsd.Value));
                        }
                        else
                        {
                            if (memoDetail.NormalFeeAmount != null)
                                txtNormalFeeAmount = string.Format("{0}: {1} {2}", lblNormalFee, curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value));
                        }
                    }

                    gridData.Remark = String.Format("{0}{1}{2}", string.IsNullOrEmpty(memoDetail.Remark) ? string.Empty : string.Format("{0}<br/>", memoDetail.Remark)
                        , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("{0}: {1}<br/>", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                        , txtNormalFeeAmount);

                    gridData.FeeTypeCode = memoDetail.BillingType;
                    gridData.HandlingTypeCode = memoDetail.HandlingType;
                    gridData.Sequence = strSequence;

                    CTS110_CancelContractMemoDetailTemp cacelMemoDetailTemp = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS110_CancelContractMemoDetailTemp>(memoDetail);
                    cacelMemoDetailTemp.ContractCode_CounterBalance = comUtil.ConvertContractCode(cacelMemoDetailTemp.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_LONG);
                    cacelMemoDetailTemp.Sequence = strSequence;
                    memoDetailTempList.Add(cacelMemoDetailTemp);

                    sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData = memoDetailTempList;
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
        public ActionResult CTS110_CalculateTotalAmount()
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList = new List<CTS110_CancelContractMemoDetailTemp>();

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                memoDetailTempList = sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData;

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
                    //decFeeAmount = memoDetail.FeeAmount == null ? 0 : memoDetail.FeeAmount.Value;
                    //decTaxAmount = memoDetail.TaxAmount == null ? 0 : memoDetail.TaxAmount.Value;
                    //decTotalAmount = (decFeeAmount + decTaxAmount);

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

                sParam.CTS110_Session.InitialData.TotalSlideAmount = decSlideAmount;
                sParam.CTS110_Session.InitialData.TotalRefundAmount = decRefundAmount;
                sParam.CTS110_Session.InitialData.TotalBillingAmount = decBillingAmount;
                sParam.CTS110_Session.InitialData.TotalCounterBalAmount = decCounterBalAmount;

                sParam.CTS110_Session.InitialData.TotalSlideAmountUsd = decSlideAmountUsd;
                sParam.CTS110_Session.InitialData.TotalRefundAmountUsd = decRefundAmountUsd;
                sParam.CTS110_Session.InitialData.TotalBillingAmountUsd = decBillingAmountUsd;
                sParam.CTS110_Session.InitialData.TotalCounterBalAmountUsd = decCounterBalAmountUsd;

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
        public ActionResult CTS110_RemoveDataCancelCond(string strSequence)
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                memoDetailTempList = sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData;

                foreach (CTS110_CancelContractMemoDetailTemp memoDetail in memoDetailTempList)
                {
                    if (memoDetail.Sequence == strSequence)
                    {
                        memoDetailTempList.Remove(memoDetail);
                        break;
                    }
                }

                sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData = memoDetailTempList;
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
        /// Get detail of BillingTarget
        /// </summary>
        /// <param name="strSequence"></param>
        /// <returns></returns>
        public ActionResult CTS110_GetBillingTargetDetail(string strSequence)
        {
            ObjectResultData res = new ObjectResultData();

            List<CTS110_BillingTemp> billingTempList;
            List<CTS110_BillingClientData> dtBillingClientList;
            List<CTS110_BillingTargetData> billingTargetList;

            CTS110_BillingClientData billingClient = new CTS110_BillingClientData();
            CTS110_BillingTargetData billingTarget = new CTS110_BillingTargetData();

            List<CTS110_BillingClientData> billingClientListTemp;
            List<CTS110_BillingTargetData> billingTargetListTemp;

            CTS051_DOBillingTargetDetailData doBillingTargetDetail = new CTS051_DOBillingTargetDetailData();

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                billingTempList = sParam.CTS110_Session.InitialData.BillingTempData;
                dtBillingClientList = sParam.CTS110_Session.InitialData.BillingClientData;
                billingTargetList = sParam.CTS110_Session.InitialData.BillingTargetData;

                billingTempList = billingTempList.FindAll(delegate(CTS110_BillingTemp s) { return s.Sequence == strSequence; });

                billingClientListTemp = dtBillingClientList.FindAll(delegate(CTS110_BillingClientData s) { return s.Sequence == strSequence; });
                if (billingClientListTemp != null && billingClientListTemp.Count > 0)
                    billingClient = billingClientListTemp[0];

                billingTargetListTemp = billingTargetList.FindAll(delegate(CTS110_BillingTargetData s) { return s.Sequence == strSequence; });
                if (billingTargetListTemp != null && billingTargetListTemp.Count > 0)
                    billingTarget = billingTargetListTemp[0];

                if (billingTempList != null && billingTempList.Count() > 0)
                {
                    doBillingTargetDetail.BillingTargetCodeDetail = billingTempList[0].BillingTargetCodeShort;
                    doBillingTargetDetail.BillingOCC = billingTempList[0].BillingOCC;
                    doBillingTargetDetail.BillingOfficeCode = billingTempList[0].BillingOfficeCode;
                    doBillingTargetDetail.Status = billingTempList[0].Status;
                }

                if (billingClient != null)
                {
                    doBillingTargetDetail.BillingClientCodeDetail = billingClient.BillingClientCodeShort;
                    doBillingTargetDetail.FullNameEN = billingClient.FullNameEN;
                    doBillingTargetDetail.BranchNameEN = billingClient.BranchNameEN;
                    doBillingTargetDetail.AddressEN = billingClient.AddressEN;
                    doBillingTargetDetail.FullNameLC = billingClient.FullNameLC;
                    doBillingTargetDetail.BranchNameLC = billingClient.BranchNameLC;
                    doBillingTargetDetail.AddressLC = billingClient.AddressLC;
                    doBillingTargetDetail.Nationality = billingClient.Nationality;
                    doBillingTargetDetail.PhoneNo = billingClient.PhoneNo;
                    doBillingTargetDetail.BusinessType = billingClient.BusinessTypeName;
                    doBillingTargetDetail.IDNo = billingClient.IDNo;
                }

                doBillingTargetDetail.Sequence = strSequence;

                sParam.CTS110_Session.InitialData.SequenceTemp = strSequence;
                sParam.CTS110_Session.InitialData.BillingTempDataTemp = billingTempList.Count > 0 ? CommonUtil.CloneObject<CTS110_BillingTemp, CTS110_BillingTemp>(billingTempList[0]) : null;
                sParam.CTS110_Session.InitialData.BillingClientDataTemp = CommonUtil.CloneObject<CTS110_BillingClientData, CTS110_BillingClientData>(billingClient);
                sParam.CTS110_Session.InitialData.BillingTargetDataTemp = CommonUtil.CloneObject<CTS110_BillingTargetData, CTS110_BillingTargetData>(billingTarget);
                UpdateScreenObject(sParam);

                res.ResultData = doBillingTargetDetail;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Remove BillingTarget item from grid when click [Remove] button on grid
        /// </summary>
        /// <param name="strSequence"></param>
        /// <returns></returns>
        public ActionResult CTS110_RemoveDataBillingTarget(string strSequence)
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_BillingClientData> dtBillingClientList;
            List<CTS110_BillingTargetData> dtBillingTargetList;
            List<CTS110_BillingTemp> billingTempList;

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                dtBillingClientList = sParam.CTS110_Session.InitialData.BillingClientData;
                dtBillingTargetList = sParam.CTS110_Session.InitialData.BillingTargetData;
                billingTempList = sParam.CTS110_Session.InitialData.BillingTempData;

                foreach (CTS110_BillingClientData billClient in dtBillingClientList)
                {
                    if (billClient.Sequence == strSequence)
                    {
                        dtBillingClientList.Remove(billClient);
                        break;
                    }
                }

                foreach (CTS110_BillingTargetData billTarget in dtBillingTargetList)
                {
                    if (billTarget.Sequence == strSequence)
                    {
                        dtBillingTargetList.Remove(billTarget);
                        break;
                    }
                }

                foreach (CTS110_BillingTemp billTemp in billingTempList)
                {
                    if (billTemp.Sequence == strSequence)
                    {
                        billingTempList.Remove(billTemp);
                        break;
                    }
                }

                sParam.CTS110_Session.InitialData.BillingClientData = dtBillingClientList;
                sParam.CTS110_Session.InitialData.BillingTargetData = dtBillingTargetList;
                sParam.CTS110_Session.InitialData.BillingTempData = billingTempList;
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
        /// Retrieve BillingTarget or BillingClient when click [Retrieve] button on ‘Specify code’ section
        /// </summary>
        /// <param name="bIsTargetCode"></param>
        /// <param name="strBillingTargetCode"></param>
        /// <param name="strBillingClientCode"></param>
        /// <returns></returns>
        public ActionResult CTS110_RetrieveBillingTargetData(string strBillingTargetCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            IBillingInterfaceHandler billingHandler;
            IBillingMasterHandler billMasterHandler;
            List<tbt_BillingTarget> billingTargetList = null;
            List<dtBillingClientData> billingClientDataList;
            dtBillingClientData billingClientData;
            CTS051_DOBillingTargetDetailData doBillingTargetDetail;

            try
            {
                if (String.IsNullOrEmpty(strBillingTargetCode))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "Billing target code" }); //TODO: (Jutarat) Must get lbl from resouce
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblBillingTargetCode" },
                                        new string[] { "BillingTargetCode" });

                    return Json(res);
                }

                //Get billing target information
                billingHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                string strBillingTargetCodeLong = comUtil.ConvertBillingTargetCode(strBillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                billingTargetList = billingHandler.GetBillingTarget(strBillingTargetCodeLong);

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                if (billingTargetList == null || billingTargetList.Count < 1)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { strBillingTargetCode }, new string[] { "BillingTargetCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                string strBillingClientCode = comUtil.ConvertBillingClientCode(billingTargetList[0].BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                //Get billing client information
                billMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                string strBillingClientCodeLong = comUtil.ConvertBillingClientCode(strBillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                billingClientDataList = billMasterHandler.GetBillingClient(strBillingClientCodeLong);

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                if (billingClientDataList == null || billingClientDataList.Count < 1)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0138);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Refesh master data for billing target
                billingClientData = billingClientDataList[0];
                LoadMasterData_CTS110(res, billingClientData);
                if (res.IsError)
                    return Json(res);

                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                CTS110_BillingTemp dtRetriveBillingTempData = new CTS110_BillingTemp();
                dtRetriveBillingTempData.ContractCode = sParam.CTS110_Session.InitialData.ContractCode;
                dtRetriveBillingTempData.BillingClientCode = billingClientDataList[0].BillingClientCode;
                if (billingTargetList != null && billingTargetList.Count > 0)
                {
                    dtRetriveBillingTempData.BillingTargetCode = billingTargetList[0].BillingTargetCode;
                    dtRetriveBillingTempData.BillingOfficeCode = billingTargetList[0].BillingOfficeCode;
                }

                doBillingTargetDetail = GetBillingTargetDetailData_CTS110(dtRetriveBillingTempData, billingClientData);

                sParam.CTS110_Session.InitialData.BillingClientDataTemp = CommonUtil.CloneObject<dtBillingClientData, CTS110_BillingClientData>(billingClientData);
                sParam.CTS110_Session.InitialData.BillingClientDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;

                sParam.CTS110_Session.InitialData.BillingTempDataTemp = dtRetriveBillingTempData;
                sParam.CTS110_Session.InitialData.BillingTempDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;

                sParam.CTS110_Session.InitialData.BillingTargetDataTemp = new CTS110_BillingTargetData();
                sParam.CTS110_Session.InitialData.BillingTargetDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;
                if (billingTargetList != null && billingTargetList.Count > 0)
                    sParam.CTS110_Session.InitialData.BillingTargetDataTemp = CommonUtil.CloneObject<tbt_BillingTarget, CTS110_BillingTargetData>(billingTargetList[0]);

                UpdateScreenObject(sParam);

                res.ResultData = doBillingTargetDetail;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Retrieve BillingTarget or BillingClient when click [Retrieve] button on ‘Specify code’ section
        /// </summary>
        /// <param name="bIsTargetCode"></param>
        /// <param name="strBillingTargetCode"></param>
        /// <param name="strBillingClientCode"></param>
        /// <returns></returns>
        public ActionResult CTS110_RetrieveBillingClientData(string strBillingClientCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            IBillingInterfaceHandler billingHandler;
            IBillingMasterHandler billMasterHandler;
            List<tbt_BillingTarget> billingTargetList = null;
            List<dtBillingClientData> billingClientDataList;
            dtBillingClientData billingClientData;
            CTS051_DOBillingTargetDetailData doBillingTargetDetail;

            try
            {
                if (String.IsNullOrEmpty(strBillingClientCode))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "Billing client code" }); //TODO: (Jutarat) Must get lbl from resouce
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblBillingClientCode" },
                                        new string[] { "BillingClientCode" });

                    return Json(res);
                }

                //Get billing client information
                billMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                string strBillingClientCodeLong = comUtil.ConvertBillingClientCode(strBillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                billingClientDataList = billMasterHandler.GetBillingClient(strBillingClientCodeLong);

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                if (billingClientDataList == null || billingClientDataList.Count < 1)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { strBillingClientCode });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                //Refesh master data for billing target
                billingClientData = billingClientDataList[0];
                LoadMasterData_CTS110(res, billingClientData);
                if (res.IsError)
                    return Json(res);

                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                CTS110_BillingTemp dtRetriveBillingTempData = new CTS110_BillingTemp();
                dtRetriveBillingTempData.ContractCode = sParam.CTS110_Session.InitialData.ContractCode;
                dtRetriveBillingTempData.BillingClientCode = billingClientDataList[0].BillingClientCode;
                if (billingTargetList != null && billingTargetList.Count > 0)
                {
                    dtRetriveBillingTempData.BillingTargetCode = billingTargetList[0].BillingTargetCode;
                    dtRetriveBillingTempData.BillingOfficeCode = billingTargetList[0].BillingOfficeCode;
                }

                doBillingTargetDetail = GetBillingTargetDetailData_CTS110(dtRetriveBillingTempData, billingClientData);

                sParam.CTS110_Session.InitialData.BillingClientDataTemp = CommonUtil.CloneObject<dtBillingClientData, CTS110_BillingClientData>(billingClientData);
                sParam.CTS110_Session.InitialData.BillingClientDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;

                sParam.CTS110_Session.InitialData.BillingTempDataTemp = dtRetriveBillingTempData;
                sParam.CTS110_Session.InitialData.BillingTempDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;

                sParam.CTS110_Session.InitialData.BillingTargetDataTemp = new CTS110_BillingTargetData();
                sParam.CTS110_Session.InitialData.BillingTargetDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;
                if (billingTargetList != null && billingTargetList.Count > 0)
                    sParam.CTS110_Session.InitialData.BillingTargetDataTemp = CommonUtil.CloneObject<tbt_BillingTarget, CTS110_BillingTargetData>(billingTargetList[0]);

                UpdateScreenObject(sParam);

                res.ResultData = doBillingTargetDetail;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Search BillingClient when click [Search billing client] button on ‘Specify code’ section
        /// </summary>
        /// <param name="billingClientData"></param>
        /// <returns></returns>
        public ActionResult CTS110_SearchBillingClient(dtBillingClientData billingClientData)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();

            try
            {
                if (billingClientData.BillingClientCode != null
                    && billingClientData.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT)
                {
                    billingClientData.BillingClientCode = comUtil.ConvertBillingClientCode(billingClientData.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                }

                LoadMasterData_CTS110(res, billingClientData);
                if (res.IsError)
                    return Json(res);

                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                CTS110_BillingTemp dtRetriveBillingTempData = new CTS110_BillingTemp();
                dtRetriveBillingTempData.ContractCode = sParam.CTS110_Session.InitialData.ContractCode;
                dtRetriveBillingTempData.BillingClientCode = billingClientData.BillingClientCode;
                
                CTS051_DOBillingTargetDetailData doBillingTargetDetail = GetBillingTargetDetailData_CTS110(dtRetriveBillingTempData, billingClientData);

                sParam.CTS110_Session.InitialData.BillingClientDataTemp = CommonUtil.CloneObject<dtBillingClientData, CTS110_BillingClientData>(billingClientData);
                sParam.CTS110_Session.InitialData.BillingClientDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;

                sParam.CTS110_Session.InitialData.BillingTempDataTemp = dtRetriveBillingTempData;
                sParam.CTS110_Session.InitialData.BillingTempDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;

                sParam.CTS110_Session.InitialData.BillingTargetDataTemp = new CTS110_BillingTargetData();
                sParam.CTS110_Session.InitialData.BillingTargetDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;

                UpdateScreenObject(sParam);

                res.ResultData = doBillingTargetDetail;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Copy data and show on ‘Billing target detail’ section when click [Copy] button on ‘Copy name and address information’ section
        /// </summary>
        /// <param name="strCopyType"></param>
        /// <returns></returns>
        public ActionResult CTS110_CopyBillingTarget(string strCopyType)
        {
            ObjectResultData res = new ObjectResultData();
            IMasterHandler masHandler;
            List<tbm_Customer> customerList;
            dtBillingClientData billingClientData = new dtBillingClientData();

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                tbt_RentalContractBasic rentalContractBasicData = sParam.CTS110_Session.InitialData.RentalContractBasicData;

                masHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                if (strCopyType == "0")
                {
                    //Load customer data of contract target
                    customerList = masHandler.GetTbm_Customer(rentalContractBasicData.ContractTargetCustCode);

                    //Replace existing data
                    if (customerList.Count > 0)
                    {
                        billingClientData.NameEN = customerList[0].CustNameEN;
                        billingClientData.NameLC = customerList[0].CustNameLC;
                        billingClientData.FullNameEN = customerList[0].CustFullNameEN;
                        billingClientData.FullNameLC = customerList[0].CustFullNameLC;
                        billingClientData.AddressEN = customerList[0].AddressFullEN;
                        billingClientData.AddressLC = customerList[0].AddressFullLC;
                        billingClientData.RegionCode = customerList[0].RegionCode;
                        billingClientData.PhoneNo = customerList[0].PhoneNo;
                        billingClientData.BusinessTypeCode = customerList[0].BusinessTypeCode;
                        billingClientData.IDNo = customerList[0].IDNo;
                        billingClientData.CustTypeCode = customerList[0].CustTypeCode;
                        billingClientData.CompanyTypeCode = customerList[0].CompanyTypeCode; //Add by Jutarat A. on 17072012
                    }
                }
                else if (strCopyType == "1")
                {
                    //Load customer data of contract target
                    customerList = masHandler.GetTbm_Customer(rentalContractBasicData.ContractTargetCustCode);

                    //Replace existing data
                    if (customerList.Count > 0)
                    {
                        billingClientData.NameEN = customerList[0].CustNameEN;
                        billingClientData.NameLC = customerList[0].CustNameLC;
                        billingClientData.FullNameEN = customerList[0].CustFullNameEN;
                        billingClientData.FullNameLC = customerList[0].CustFullNameLC;
                        billingClientData.BranchNameEN = rentalContractBasicData.BranchNameEN;
                        billingClientData.BranchNameLC = rentalContractBasicData.BranchNameLC;
                        billingClientData.AddressEN = rentalContractBasicData.BranchAddressEN;
                        billingClientData.AddressLC = rentalContractBasicData.BranchAddressLC;
                        billingClientData.RegionCode = customerList[0].RegionCode;
                        //billingClientData.PhoneNo = customerList[0].PhoneNo; //Comment by Jutarat A. on 17072012
                        billingClientData.BusinessTypeCode = customerList[0].BusinessTypeCode;
                        billingClientData.IDNo = customerList[0].IDNo;
                        billingClientData.CustTypeCode = customerList[0].CustTypeCode;
                        billingClientData.CompanyTypeCode = customerList[0].CompanyTypeCode; //Add by Jutarat A. on 17072012
                    }
                }
                else if (strCopyType == "2")
                {
                    //Load customer data of real customer
                    customerList = masHandler.GetTbm_Customer(rentalContractBasicData.RealCustomerCustCode);

                    //Replace existing data
                    if (customerList.Count > 0)
                    {
                        billingClientData.NameEN = customerList[0].CustNameEN;
                        billingClientData.NameLC = customerList[0].CustNameLC;
                        billingClientData.FullNameEN = customerList[0].CustFullNameEN;
                        billingClientData.FullNameLC = customerList[0].CustFullNameLC;
                        billingClientData.AddressEN = customerList[0].AddressFullEN;
                        billingClientData.AddressLC = customerList[0].AddressFullLC;
                        billingClientData.RegionCode = customerList[0].RegionCode;
                        billingClientData.PhoneNo = customerList[0].PhoneNo;
                        billingClientData.BusinessTypeCode = customerList[0].BusinessTypeCode;
                        billingClientData.IDNo = customerList[0].IDNo;
                        billingClientData.CustTypeCode = customerList[0].CustTypeCode;
                        billingClientData.CompanyTypeCode = customerList[0].CompanyTypeCode; //Add by Jutarat A. on 17072012
                    }
                }
                else if (strCopyType == "3")
                {
                    //Load customer data of real customer
                    customerList = masHandler.GetTbm_Customer(rentalContractBasicData.RealCustomerCustCode);

                    //Load site data
                    List<doGetTbm_Site> siteList = masHandler.GetTbm_Site(rentalContractBasicData.SiteCode);

                    //Replace existing data
                    if (customerList.Count > 0 && siteList.Count > 0)
                    {
                        billingClientData.NameEN = customerList[0].CustNameEN;
                        billingClientData.NameLC = customerList[0].CustNameLC;
                        billingClientData.FullNameEN = customerList[0].CustFullNameEN;
                        billingClientData.FullNameLC = customerList[0].CustFullNameLC;
                        billingClientData.BranchNameEN = siteList[0].SiteNameEN;
                        billingClientData.BranchNameLC = siteList[0].SiteNameLC;
                        billingClientData.AddressEN = siteList[0].AddressFullEN;
                        billingClientData.AddressLC = siteList[0].AddressFullLC;
                        billingClientData.RegionCode = customerList[0].RegionCode;
                        billingClientData.PhoneNo = siteList[0].PhoneNo;
                        billingClientData.BusinessTypeCode = customerList[0].BusinessTypeCode;
                        billingClientData.IDNo = customerList[0].IDNo;
                        billingClientData.CustTypeCode = customerList[0].CustTypeCode;
                        billingClientData.CompanyTypeCode = customerList[0].CompanyTypeCode; //Add by Jutarat A. on 17072012
                    }
                }

                //Refesh master data for billing target
                LoadMasterData_CTS110(res, billingClientData);
                if (res.IsError)
                    return Json(res);

                CTS110_BillingTemp dtRetriveBillingTempData = new CTS110_BillingTemp();
                dtRetriveBillingTempData.ContractCode = sParam.CTS110_Session.InitialData.ContractCode;
                dtRetriveBillingTempData.BillingClientCode = billingClientData.BillingClientCode;

                CTS051_DOBillingTargetDetailData doBillingTargetDetail = GetBillingTargetDetailData_CTS110(dtRetriveBillingTempData, billingClientData);

                sParam.CTS110_Session.InitialData.BillingClientDataTemp = CommonUtil.CloneObject<dtBillingClientData, CTS110_BillingClientData>(billingClientData);
                sParam.CTS110_Session.InitialData.BillingClientDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;

                sParam.CTS110_Session.InitialData.BillingTempDataTemp = dtRetriveBillingTempData;
                sParam.CTS110_Session.InitialData.BillingTempDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;

                sParam.CTS110_Session.InitialData.BillingTargetDataTemp = new CTS110_BillingTargetData();
                sParam.CTS110_Session.InitialData.BillingTargetDataTemp.Sequence = sParam.CTS110_Session.InitialData.SequenceTemp;
                UpdateScreenObject(sParam);

                res.ResultData = doBillingTargetDetail;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get data of BillingClient
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS110_GetTempBillingClientData()
        {
            ObjectResultData res = new ObjectResultData();
            CTS110_BillingClientData billingClientData = null;

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                billingClientData = sParam.CTS110_Session.InitialData.BillingClientDataTemp;
                if (billingClientData == null)
                    billingClientData = new CTS110_BillingClientData();

                res.ResultData = billingClientData;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Clear data of BillingTarget when click [Clear billing target] button on ‘Billing target detail’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS110_ClearBillingTarget()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                if (sParam.CTS110_Session.InitialData.BillingTempDataTemp != null)
                {
                    sParam.CTS110_Session.InitialData.BillingTempDataTemp.BillingOCC = null;
                    sParam.CTS110_Session.InitialData.BillingTempDataTemp.BillingTargetRunningNo = null;
                    sParam.CTS110_Session.InitialData.BillingTempDataTemp.BillingClientCode = null;
                    sParam.CTS110_Session.InitialData.BillingTempDataTemp.BillingTargetCode = null;
                    sParam.CTS110_Session.InitialData.BillingTempDataTemp.BillingOfficeCode = null;
                }

                sParam.CTS110_Session.InitialData.BillingClientDataTemp = null;
                sParam.CTS110_Session.InitialData.BillingTargetDataTemp = null;
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
        /// Add or Update detail of BillingTarget when click [Add/Update] button on ‘Billing target detail’ section
        /// </summary>
        /// <param name="strOperationMode"></param>
        /// <param name="strBillingOffice"></param>
        /// <returns></returns>
        public ActionResult CTS110_AddUpdateBillingTargetDetail(string strOperationMode, string strBillingOffice)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<CTS110_BillingTemp> billingTempDataList;
            List<CTS110_BillingClientData> billingClientDataList;
            List<CTS110_BillingTargetData> billingTargetDataList;

            CTS110_BillingTemp tempBillingTempData;
            CTS110_BillingClientData tempBillingClientData;
            CTS110_BillingTargetData tempBillingTargetData;

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                billingTempDataList = sParam.CTS110_Session.InitialData.BillingTempData;
                billingClientDataList = sParam.CTS110_Session.InitialData.BillingClientData;
                billingTargetDataList = sParam.CTS110_Session.InitialData.BillingTargetData;
                tempBillingTempData = sParam.CTS110_Session.InitialData.BillingTempDataTemp;
                tempBillingClientData = sParam.CTS110_Session.InitialData.BillingClientDataTemp;
                tempBillingTargetData = sParam.CTS110_Session.InitialData.BillingTargetDataTemp;

                //Validate require fields in Billing target detail section
                CTS110_ValidateBillingClientData validBillingClient = CommonUtil.CloneObject<CTS110_BillingClientData, CTS110_ValidateBillingClientData>(tempBillingClientData);
                CTS110_ValidateBillingTempData validBillingTemp = new CTS110_ValidateBillingTempData();
                validBillingTemp.BillingOfficeCode = strBillingOffice;

                ValidatorUtil.BuildErrorMessage(res, new object[] { validBillingClient, validBillingTemp });
                if (res.IsError)
                    return Json(res);

                //Validate business
                foreach (CTS110_BillingTemp data in billingTempDataList)
                {
                    if (tempBillingTempData != null)
                    {
                        if (tempBillingTempData.BillingClientCode == data.BillingClientCode && strBillingOffice == data.BillingOfficeCode)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3032);
                            return Json(res);
                        }
                    }
                }

                if (tempBillingTempData == null)
                    tempBillingTempData = new CTS110_BillingTemp();

                tempBillingTempData.BillingOfficeCode = strBillingOffice;

                if (strOperationMode == "Update")
                {
                    if (tempBillingTempData != null)
                    {
                        tempBillingTempData.Status = strOperationMode;

                        foreach (CTS110_BillingTemp data in billingTempDataList)
                        {
                            if (data.Sequence == tempBillingTempData.Sequence)
                            {
                                billingTempDataList.Remove(data);
                                billingTempDataList.Add(tempBillingTempData);
                                break;
                            }
                        }
                    }

                    if (tempBillingClientData != null)
                    {
                        tempBillingClientData.Status = strOperationMode;

                        foreach (CTS110_BillingClientData data in billingClientDataList)
                        {
                            if (data.Sequence == tempBillingClientData.Sequence)
                            {
                                billingClientDataList.Remove(data);
                                billingClientDataList.Add(tempBillingClientData);
                                break;
                            }
                        }
                    }

                    if (tempBillingTargetData != null)
                    {
                        tempBillingTargetData.Status = strOperationMode;

                        foreach (CTS110_BillingTargetData data in billingTargetDataList)
                        {
                            if (data.Sequence == tempBillingTargetData.Sequence)
                            {
                                billingTargetDataList.Remove(data);
                                billingTargetDataList.Add(tempBillingTargetData);
                                break;
                            }
                        }
                    }
                }
                else if (strOperationMode == "Add")
                {
                    string strSequence = "0";
                    if (billingTempDataList.Count == 0)
                        strSequence = "0";
                    else
                        strSequence = (int.Parse(billingTempDataList.Max(t => t.Sequence)[0].ToString()) + 1).ToString();

                    if (tempBillingTempData != null)
                    {
                        tempBillingTempData.Sequence = strSequence;
                        tempBillingTempData.Status = strOperationMode;
                        billingTempDataList.Add(tempBillingTempData);
                    }

                    if (tempBillingClientData != null)
                    {
                        tempBillingClientData.Sequence = strSequence;
                        tempBillingClientData.Status = strOperationMode;
                        billingClientDataList.Add(tempBillingClientData);
                    }

                    if (tempBillingTargetData != null)
                    {
                        tempBillingTargetData.Sequence = strSequence;
                        tempBillingTargetData.Status = strOperationMode;
                        billingTargetDataList.Add(tempBillingTargetData);
                    }
                }

                sParam.CTS110_Session.InitialData.BillingTempData = billingTempDataList;
                sParam.CTS110_Session.InitialData.BillingClientData = billingClientDataList;
                sParam.CTS110_Session.InitialData.BillingTargetData = billingTargetDataList;
                sParam.CTS110_Session.InitialData.BillingTempDataTemp = null;
                sParam.CTS110_Session.InitialData.BillingClientDataTemp = null;
                sParam.CTS110_Session.InitialData.BillingTargetDataTemp = null;
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
        /// Reload detail of BillingTarget to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS110_RefreshBillingTargetDetail()
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_BillingTemp> billingTempDataList;
            List<CTS110_BillingClientData> billingClientDataList;

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                billingTempDataList = sParam.CTS110_Session.InitialData.BillingTempData;
                billingClientDataList = sParam.CTS110_Session.InitialData.BillingClientData;

                CommonUtil comUtil = new CommonUtil();

                List<CTS110_RemovalInstallationFeeGridData> gridDataList = new List<CTS110_RemovalInstallationFeeGridData>();
                foreach (CTS110_BillingTemp dataTemp in billingTempDataList)
                {
                    foreach (CTS110_BillingClientData dataClient in billingClientDataList)
                    {
                        if (dataTemp.Sequence == dataClient.Sequence)
                        {
                            CTS110_RemovalInstallationFeeGridData data = new CTS110_RemovalInstallationFeeGridData();
                            data.BillingOCC = dataTemp.BillingOCC;
                            data.BillingClientCode = dataClient.BillingClientCodeShort;
                            data.BillingOfficeCode = dataTemp.BillingOfficeCode;
                            data.BillingOfficeName = GetBillingOfficeName_CTS110(dataTemp.BillingOfficeCode);

                            //data.BillingTargetCode = dataTemp.BillingTargetCode;
                            data.BillingTargetCode = comUtil.ConvertBillingTargetCode(dataTemp.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                            data.BillingTargetName = string.Format("(1) {0} <br/>(2) {1}", dataClient.FullNameEN, dataClient.FullNameLC);
                            data.Sequence = dataClient.Sequence;
                            data.Status = dataClient.Status;
                            gridDataList.Add(data);
                        }
                    }
                }

                gridDataList = (from t in gridDataList
                                orderby t.Sequence
                                select t).ToList<CTS110_RemovalInstallationFeeGridData>();

                res.ResultData = CommonUtil.ConvertToXml<CTS110_RemovalInstallationFeeGridData>(gridDataList, "Contract\\CTS110_RemovalFeeBillingTarget", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Reload detail of CancelContract to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS110_RefreshCancelContractConditionDetail()
        {
            ObjectResultData res = new ObjectResultData();
            List<CTS110_CancelContractConditionGridData> gridDataList = new List<CTS110_CancelContractConditionGridData>();
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblNormalFee");
            string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblContractCodeForSlideFee");

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                memoDetailTempList = sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData;

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
                        
                        DataEntity.Common.doMiscTypeCode curr = null;
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
                            if (memoDetail.FeeAmountCurrencyType == null)
                                memoDetail.FeeAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                            if (memoDetail.TaxAmountCurrencyType == null)
                                memoDetail.TaxAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;

                            #region Fee

                            curr = currencies.Find(x => x.ValueCode == memoDetail.FeeAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];

                            if (memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                if (memoDetail.FeeAmountUsd != null)
                                    data.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.FeeAmountUsd.Value));
                            }
                            else
                            {
                                if (memoDetail.FeeAmount != null)
                                    data.Fee = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.FeeAmount.Value));
                            }
                                                        
                            #endregion
                            #region Tax

                            curr = currencies.Find(x => x.ValueCode == memoDetail.TaxAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];

                            if (memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                if (memoDetail.TaxAmountUsd != null)
                                    data.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.TaxAmountUsd.Value));
                            }
                            else
                            {
                                if (memoDetail.TaxAmount != null)
                                    data.Tax = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.TaxAmount.Value));
                            }
                            
                            #endregion
                        }

                        data.Period = String.Format("{0} {1}", strStartPeriodDate, strEndPeriodDate);

                        //data.Remark = String.Format("{0} {1} {2}", memoDetail.Remark
                        //    , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                        //    , memoDetail.NormalFeeAmount == null ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount)));

                        string txtNormalFeeAmount = "";
                        if (currencies != null)
                        {
                            curr = currencies.Find(x => x.ValueCode == memoDetail.NormalFeeAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];

                            if (memoDetail.NormalFeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                if (memoDetail.NormalFeeAmountUsd != null)
                                    txtNormalFeeAmount = string.Format("{0}: {1} {2}", lblNormalFee, curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.NormalFeeAmountUsd.Value));
                            }
                            else
                            {
                                if (memoDetail.NormalFeeAmount != null)
                                    txtNormalFeeAmount = string.Format("{0}: {1} {2}", lblNormalFee, curr.ValueDisplayEN, CommonUtil.TextNumeric(memoDetail.NormalFeeAmount.Value));
                            }
                        }

                        data.Remark = String.Format("{0}{1}{2}", string.IsNullOrEmpty(memoDetail.Remark) ? string.Empty : string.Format("{0}<br/>", memoDetail.Remark)
                            , memoDetail.ContractCode_CounterBalance == null ? string.Empty : string.Format("{0}: {1}<br/>", lblContractCodeForSlideFee, memoDetail.ContractCode_CounterBalance)
                            , txtNormalFeeAmount);

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

        /// <summary>
        /// Validate business when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <param name="doRegisterCancelData"></param>
        /// <returns></returns>
        public ActionResult CTS110_RegisterCancelContractData(CTS110_RegisterCancelData doRegisterCancelData)
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
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                dtRentalContractBasicData = sParam.CTS110_Session.InitialData.RentalContractBasicData;
                memoDetailTempDataList = sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData;

                //CheckMandatory
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                object cancelContractTemp = null;
                if (dtRentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                {
                    cancelContractTemp = CommonUtil.CloneObject<CTS110_RegisterCancelData, CTS110_ValidateCancelContractBefStart>(doRegisterCancelData);
                }
                else
                {
                    cancelContractTemp = CommonUtil.CloneObject<CTS110_RegisterCancelData, CTS110_ValidateCancelContract>(doRegisterCancelData);
                }

                CTS110_ValidateProcessAfterCounterBalanceType validProcAftCntBal = CommonUtil.CloneObject<CTS110_RegisterCancelData, CTS110_ValidateProcessAfterCounterBalanceType>(doRegisterCancelData);

                if (sParam.CTS110_Session.InitialData.TotalRefundAmount > sParam.CTS110_Session.InitialData.TotalBillingAmount)
                {
                    if (validProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND
                        && validProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE)
                    {
                        validProcAftCntBal.ProcessAfterCounterBalanceType = null;
                    }
                }
                else if (sParam.CTS110_Session.InitialData.TotalRefundAmount < sParam.CTS110_Session.InitialData.TotalBillingAmount)
                {
                    if (validProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL
                        && validProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT)
                    {
                        validProcAftCntBal.ProcessAfterCounterBalanceType = null;
                    }
                }
                else
                {
                    validProcAftCntBal.ProcessAfterCounterBalanceType = "0";
                }

                if (sParam.CTS110_Session.InitialData.TotalRefundAmountUsd > sParam.CTS110_Session.InitialData.TotalBillingAmountUsd)
                {
                    if (validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND
                        && validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE)
                    {
                        validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd = null;
                    }
                }
                else if (sParam.CTS110_Session.InitialData.TotalRefundAmountUsd < sParam.CTS110_Session.InitialData.TotalBillingAmountUsd)
                {
                    if (validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL
                        && validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT)
                    {
                        validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd = null;
                    }
                }
                else
                {
                    validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd = "0";
                }

                ValidatorUtil.BuildErrorMessage(res, new object[] { cancelContractTemp, validProcAftCntBal }, null, false);
                if (res.IsError)
                    return Json(res);

                //ValidateBusiness
                ValidateBusiness_CTS110(res, sParam.CTS110_Session.InitialData.ContractCode, doRegisterCancelData);
                if (res.IsError)
                    return Json(res);

                if (doRegisterCancelData.IsShowBillingTagetDetail == true)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3253);
                    return Json(res);
                }

                //ValidateBusinessForWarning
                //res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                if (memoDetailTempDataList.Count < 1)
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
        /// <param name="doRegisterCancelData"></param>
        /// <returns></returns>
        public ActionResult CTS110_ConfirmRegisterCancelData(CTS110_RegisterCancelData doRegisterCancelData)
        {
            ObjectResultData res = new ObjectResultData();
            CTS110_RegisterRentalContractTargetData registerRentalContractData;
            tbt_RentalContractBasic dtRentalContractBasicData;
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;
            IBillingInterfaceHandler billingInterfaceHandler;
            IBillingMasterHandler billingMasterHandler;
            IBillingTempHandler billingTempHandler;
            IQuotationHandler quotHandler;
            IMasterHandler masHandler;
            IInventoryHandler iventHandler;
            CommonUtil comUtil = new CommonUtil();

            List<CTS110_BillingTemp> billingTempDataList;
            List<CTS110_BillingClientData> billingClientDataList;
            List<CTS110_BillingTargetData> billingTargetDataList;

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                dtRentalContractBasicData = sParam.CTS110_Session.InitialData.RentalContractBasicData;
                billingTempDataList = sParam.CTS110_Session.InitialData.BillingTempData;
                billingClientDataList = sParam.CTS110_Session.InitialData.BillingClientData;
                billingTargetDataList = sParam.CTS110_Session.InitialData.BillingTargetData;

                //ValidateBusiness
                ValidateBusiness_CTS110(res, sParam.CTS110_Session.InitialData.ContractCode, doRegisterCancelData);
                if (res.IsError)
                    return Json(res);

                using (TransactionScope scope = new TransactionScope())
                {
                    /*--- RegisterCancelContract ---*/
                    registerRentalContractData = sParam.CTS110_Session;
                    if (registerRentalContractData.RegisterRentalContractData != null)
                    {
                        dsRentalContract = new dsRentalContractData();

                        //Generate security occurrence
                        rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        string strOCC = rentralHandler.GenerateContractOCC(registerRentalContractData.InitialData.ContractCode, true);

                        /*--- MapRentalContractData ---*/
                        //dtTbt_RentalContractBasic
                        dsRentalContract.dtTbt_RentalContractBasic = new List<tbt_RentalContractBasic>();

                        tbt_RentalContractBasic rentalContractBasic = CommonUtil.CloneObject<tbt_RentalContractBasic, tbt_RentalContractBasic>(registerRentalContractData.RegisterRentalContractData.dtTbt_RentalContractBasic[0]);
                        rentalContractBasic.ContractCode = registerRentalContractData.InitialData.ContractCode;
                        rentalContractBasic.LastOCC = strOCC;
                        rentalContractBasic.LastChangeImplementDate = doRegisterCancelData.ChangeImplementDate;
                        dsRentalContract.dtTbt_RentalContractBasic.Add(rentalContractBasic);

                        //dtTbt_RentalSecurityBasic
                        dsRentalContract.dtTbt_RentalSecurityBasic = new List<tbt_RentalSecurityBasic>();

                        tbt_RentalSecurityBasic rentalSecurityBasicData = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(registerRentalContractData.RegisterRentalContractData.dtTbt_RentalSecurityBasic[0]);
                        rentalSecurityBasicData.ContractCode = registerRentalContractData.InitialData.ContractCode;
                        rentalSecurityBasicData.OCC = strOCC;
                        rentalSecurityBasicData.ImplementFlag = FlagType.C_FLAG_ON;

                        rentalSecurityBasicData.NormalContractFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                        rentalSecurityBasicData.NormalContractFee = 0;
                        rentalSecurityBasicData.NormalContractFeeUsd = null;
                        
                        //Keep default
                        //rentalSecurityBasicData.NormalAdditionalDepositFee = 0;
                        //rentalSecurityBasicData.OrderAdditionalDepositFee = 0;
                        //rentalSecurityBasicData.DepositFeeBillingTiming = null;

                        rentalSecurityBasicData.ApproveNo1 = doRegisterCancelData.ApproveNo1;
                        rentalSecurityBasicData.ApproveNo2 = doRegisterCancelData.ApproveNo2;
                        rentalSecurityBasicData.AlmightyProgramEmpNo = null;
                        rentalSecurityBasicData.CounterNo = 0;
                        rentalSecurityBasicData.ChangeReasonType = null;
                        rentalSecurityBasicData.ChangeNameReasonType = null;
                        rentalSecurityBasicData.StopCancelReasonType = null;
                        rentalSecurityBasicData.ContractDocPrintFlag = FlagType.C_FLAG_OFF;

                        //Move to Update cancel contract data
                        //rentalSecurityBasicData.InstallationCompleteFlag = FlagType.C_FLAG_OFF;
                        //rentalSecurityBasicData.InstallationSlipNo = null;
                        //rentalSecurityBasicData.InstallationCompleteDate = null;
                        //rentalSecurityBasicData.InstallationTypeCode = null;

                        rentalSecurityBasicData.NegotiationStaffEmpNo1 = null;
                        rentalSecurityBasicData.NegotiationStaffEmpNo2 = null;
                        rentalSecurityBasicData.ExpectedResumeDate = null;

                        //Keep default
                        //rentalSecurityBasicData.OrderInstallFee_ApproveContract = 0;
                        ////rentalSecurityBasicData.NormalInstallFee = 0;
                        ////rentalSecurityBasicData.OrderInstallFee = 0;
                        ////rentalSecurityBasicData.OrderInstallFee_CompleteInstall = 0;
                        //if (sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData != null)
                        //{
                        //    List<CTS110_CancelContractMemoDetailTemp> dtCancelContractMemoDetailList = sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData;
                        //    dtCancelContractMemoDetailList = dtCancelContractMemoDetailList.FindAll(delegate(CTS110_CancelContractMemoDetailTemp s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; });
                        //    if (dtCancelContractMemoDetailList != null && dtCancelContractMemoDetailList.Count > 0)
                        //    {
                        //        rentalSecurityBasicData.NormalInstallFee = dtCancelContractMemoDetailList[0].NormalFeeAmount;
                        //        rentalSecurityBasicData.OrderInstallFee = dtCancelContractMemoDetailList[0].FeeAmount;
                        //        rentalSecurityBasicData.OrderInstallFee_CompleteInstall = dtCancelContractMemoDetailList[0].FeeAmount;
                        //    }
                        //}
                        //rentalSecurityBasicData.OrderInstallFee_StartService = 0;

                        dsRentalContract.dtTbt_RentalSecurityBasic.Add(rentalSecurityBasicData);

                        //dtTbt_RentalBEDetails
                        if (registerRentalContractData.RegisterRentalContractData.dtTbt_RentalBEDetails != null && registerRentalContractData.RegisterRentalContractData.dtTbt_RentalBEDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>();
                            tbt_RentalBEDetails rentalBEDetailsData;
                            foreach (tbt_RentalBEDetails data in registerRentalContractData.RegisterRentalContractData.dtTbt_RentalBEDetails)
                            {
                                rentalBEDetailsData = CommonUtil.CloneObject<tbt_RentalBEDetails, tbt_RentalBEDetails>(data);
                                rentalBEDetailsData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalBEDetails.Add(rentalBEDetailsData);
                            }
                        }

                        //dtTbt_RentalInstrumentDetails
                        if (registerRentalContractData.RegisterRentalContractData.dtTbt_RentalInstrumentDetails != null && registerRentalContractData.RegisterRentalContractData.dtTbt_RentalInstrumentDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                            tbt_RentalInstrumentDetails rentalInstrumentDetailsData;
                            foreach (tbt_RentalInstrumentDetails data in registerRentalContractData.RegisterRentalContractData.dtTbt_RentalInstrumentDetails)
                            {
                                rentalInstrumentDetailsData = CommonUtil.CloneObject<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(data);
                                rentalInstrumentDetailsData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalInstrumentDetails.Add(rentalInstrumentDetailsData);
                            }
                        }

                        //dtTbt_RentalMaintenanceDetails
                        if (registerRentalContractData.RegisterRentalContractData.dtTbt_RentalMaintenanceDetails != null && registerRentalContractData.RegisterRentalContractData.dtTbt_RentalMaintenanceDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                            tbt_RentalMaintenanceDetails rentalMaintenanceDetailsData;
                            foreach (tbt_RentalMaintenanceDetails data in registerRentalContractData.RegisterRentalContractData.dtTbt_RentalMaintenanceDetails)
                            {
                                rentalMaintenanceDetailsData = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(data);
                                rentalMaintenanceDetailsData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalMaintenanceDetails.Add(rentalMaintenanceDetailsData);
                            }
                        }

                        //dtTbt_RentalOperationType
                        if (registerRentalContractData.RegisterRentalContractData.dtTbt_RentalOperationType != null && registerRentalContractData.RegisterRentalContractData.dtTbt_RentalOperationType.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                            tbt_RentalOperationType rentalOperationTypeData;
                            foreach (tbt_RentalOperationType data in registerRentalContractData.RegisterRentalContractData.dtTbt_RentalOperationType)
                            {
                                rentalOperationTypeData = CommonUtil.CloneObject<tbt_RentalOperationType, tbt_RentalOperationType>(data);
                                rentalOperationTypeData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalOperationType.Add(rentalOperationTypeData);
                            }
                        }

                        //dtTbt_RentalSentryGuard
                        if (registerRentalContractData.RegisterRentalContractData.dtTbt_RentalSentryGuard != null && registerRentalContractData.RegisterRentalContractData.dtTbt_RentalSentryGuard.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>();
                            tbt_RentalSentryGuard rentalSentryGuardData;
                            foreach (tbt_RentalSentryGuard data in registerRentalContractData.RegisterRentalContractData.dtTbt_RentalSentryGuard)
                            {
                                rentalSentryGuardData = CommonUtil.CloneObject<tbt_RentalSentryGuard, tbt_RentalSentryGuard>(data);
                                rentalSentryGuardData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalSentryGuard.Add(rentalSentryGuardData);
                            }
                        }

                        //dtTbt_RentalSentryGuardDetails
                        if (registerRentalContractData.RegisterRentalContractData.dtTbt_RentalSentryGuardDetails != null && registerRentalContractData.RegisterRentalContractData.dtTbt_RentalSentryGuardDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                            tbt_RentalSentryGuardDetails rentalSentryGuardDetailsData;
                            foreach (tbt_RentalSentryGuardDetails data in registerRentalContractData.RegisterRentalContractData.dtTbt_RentalSentryGuardDetails)
                            {
                                rentalSentryGuardDetailsData = CommonUtil.CloneObject<tbt_RentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(data);
                                rentalSentryGuardDetailsData.OCC = strOCC;
                                dsRentalContract.dtTbt_RentalSentryGuardDetails.Add(rentalSentryGuardDetailsData);
                            }
                        }

                        //dtTbt_RelationType
                        if (registerRentalContractData.RegisterRentalContractData.dtTbt_RelationType != null && registerRentalContractData.RegisterRentalContractData.dtTbt_RelationType.Count > 0)
                        {
                            dsRentalContract.dtTbt_RelationType = new List<tbt_RelationType>();
                            tbt_RelationType relationTypeData;
                            foreach (tbt_RelationType data in registerRentalContractData.RegisterRentalContractData.dtTbt_RelationType)
                            {
                                relationTypeData = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(data);
                                relationTypeData.OCC = strOCC;
                                dsRentalContract.dtTbt_RelationType.Add(relationTypeData);
                            }
                        }
                        /*---------------------------*/

                        dsRentalContractData dsRentalContractResult;
                        dsRentalContractData dsRentalContractResultSecond;
                        if (dtRentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            if (dtRentalContractBasicData.FirstInstallCompleteFlag == true)
                            {
                                //Update start service data
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC = strOCC;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate = doRegisterCancelData.ChangeImplementDate;

                                //Add by Jutarat A. on 10092012
                                //Update dtTbt_RentalInstrumentDetails 
                                if (dsRentalContract.dtTbt_RentalInstrumentDetails != null && dsRentalContract.dtTbt_RentalInstrumentDetails.Count > 0)
                                {
                                    foreach (tbt_RentalInstrumentDetails data in dsRentalContract.dtTbt_RentalInstrumentDetails)
                                    {
                                        data.InstrumentQty = (((data.InstrumentQty?? 0) + (data.AdditionalInstrumentQty?? 0)) - (data.RemovalInstrumentQty?? 0));
                                        data.AdditionalInstrumentQty = 0;
                                        data.RemovalInstrumentQty = 0;
                                    }
                                }
                                //End Add

                                //Save first OCC of cancel contract data
                                dsRentalContractResult = rentralHandler.InsertEntireContract(dsRentalContract);

                                //Generate security occurrence
                                strOCC = rentralHandler.GenerateContractOCC(registerRentalContractData.InitialData.ContractCode, true);

                                //Update cancel contract data
                                dsRentalContractData dsRentalContractSecond = new dsRentalContractData();

                                //dtTbt_RentalContractBasic
                                dsRentalContractSecond.dtTbt_RentalContractBasic = new List<tbt_RentalContractBasic>();
                                tbt_RentalContractBasic rentalContractBasicSecond = CommonUtil.CloneObject<tbt_RentalContractBasic, tbt_RentalContractBasic>(dsRentalContractResult.dtTbt_RentalContractBasic[0]);
                                rentalContractBasicSecond.LastOCC = strOCC;
                                rentalContractBasicSecond.LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START;
                                rentalContractBasicSecond.ContractStatus = ContractStatus.C_CONTRACT_STATUS_CANCEL;
                                rentalContractBasicSecond.CancelBeforeStartProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                dsRentalContractSecond.dtTbt_RentalContractBasic.Add(rentalContractBasicSecond);

                                //dtTbt_RentalSecurityBasic
                                dsRentalContractSecond.dtTbt_RentalSecurityBasic = new List<tbt_RentalSecurityBasic>();
                                tbt_RentalSecurityBasic rentalSecurityBasicDataSecond = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(dsRentalContractResult.dtTbt_RentalSecurityBasic[0]);
                                rentalSecurityBasicDataSecond.OCC = strOCC;
                                rentalSecurityBasicDataSecond.ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START;
                                rentalSecurityBasicDataSecond.StopCancelReasonType = doRegisterCancelData.StopCancelReasonType;

                                //Add by Jutarat A. on 160072012
                                rentalSecurityBasicDataSecond.InstallationCompleteFlag = FlagType.C_FLAG_OFF;
                                rentalSecurityBasicDataSecond.InstallationSlipNo = null;
                                rentalSecurityBasicDataSecond.InstallationCompleteDate = null;
                                rentalSecurityBasicDataSecond.InstallationTypeCode = null;
                                //End Add

                                //Add by Jutarat A. on 27072012
                                rentalSecurityBasicDataSecond.InstallationCompleteEmpNo = null;

                                rentalSecurityBasicDataSecond.NormalAdditionalDepositFeeCurrencyType = null;
                                rentalSecurityBasicDataSecond.NormalAdditionalDepositFee = null;
                                rentalSecurityBasicDataSecond.NormalAdditionalDepositFeeUsd = null;
                                
                                rentalSecurityBasicDataSecond.OrderAdditionalDepositFeeCurrencyType = null;
                                rentalSecurityBasicDataSecond.OrderAdditionalDepositFee = null;
                                rentalSecurityBasicDataSecond.OrderAdditionalDepositFeeUsd = null;

                                rentalSecurityBasicDataSecond.DepositFeeBillingTiming = null;

                                rentalSecurityBasicDataSecond.NormalInstallFeeCurrencyType = null;
                                rentalSecurityBasicDataSecond.NormalInstallFee = null;
                                rentalSecurityBasicDataSecond.NormalInstallFeeUsd = null;

                                rentalSecurityBasicDataSecond.OrderInstallFeeCurrencyType = null;
                                rentalSecurityBasicDataSecond.OrderInstallFee = null;
                                rentalSecurityBasicDataSecond.OrderInstallFeeUsd = null;

                                rentalSecurityBasicDataSecond.OrderInstallFee_CompleteInstallCurrencyType = null;
                                rentalSecurityBasicDataSecond.OrderInstallFee_CompleteInstall = null;
                                rentalSecurityBasicDataSecond.OrderInstallFee_CompleteInstallUsd = null;

                                if (sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData != null)
                                {
                                    foreach (CTS110_CancelContractMemoDetailTemp memoDetailTemp in sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData)
                                    {
                                        if (memoDetailTemp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
                                        {
                                            rentalSecurityBasicDataSecond.OrderInstallFeeCurrencyType = memoDetailTemp.FeeAmountCurrencyType;
                                            rentalSecurityBasicDataSecond.OrderInstallFee = memoDetailTemp.FeeAmount;
                                            rentalSecurityBasicDataSecond.OrderInstallFeeUsd = memoDetailTemp.FeeAmountUsd;

                                            rentalSecurityBasicDataSecond.OrderInstallFee_CompleteInstallCurrencyType = memoDetailTemp.FeeAmountCurrencyType;
                                            rentalSecurityBasicDataSecond.OrderInstallFee_CompleteInstall = memoDetailTemp.FeeAmount;
                                            rentalSecurityBasicDataSecond.OrderInstallFee_CompleteInstallUsd = memoDetailTemp.FeeAmountUsd;
                                            break;
                                        }
                                    }
                                }
                                
                                rentalSecurityBasicDataSecond.OrderInstallFee_ApproveContractCurrencyType = null;
                                rentalSecurityBasicDataSecond.OrderInstallFee_ApproveContract = null;
                                rentalSecurityBasicDataSecond.OrderInstallFee_ApproveContractUsd = null;

                                rentalSecurityBasicDataSecond.OrderInstallFee_StartServiceCurrencyType = null;
                                rentalSecurityBasicDataSecond.OrderInstallFee_StartService = null;
                                rentalSecurityBasicDataSecond.OrderInstallFee_StartServiceUsd = null;
                                //End Add

                                dsRentalContractSecond.dtTbt_RentalSecurityBasic.Add(rentalSecurityBasicDataSecond);

                                //dtTbt_RentalBEDetails
                                if (dsRentalContractResult.dtTbt_RentalBEDetails != null && dsRentalContractResult.dtTbt_RentalBEDetails.Count > 0)
                                {
                                    dsRentalContractSecond.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>();
                                    tbt_RentalBEDetails rentalBEDetailsDataSecond;
                                    foreach (tbt_RentalBEDetails data in dsRentalContractResult.dtTbt_RentalBEDetails)
                                    {
                                        rentalBEDetailsDataSecond = CommonUtil.CloneObject<tbt_RentalBEDetails, tbt_RentalBEDetails>(data);
                                        rentalBEDetailsDataSecond.OCC = strOCC;
                                        dsRentalContractSecond.dtTbt_RentalBEDetails.Add(rentalBEDetailsDataSecond);
                                    }
                                }

                                //dtTbt_RentalInstrumentDetails
                                if (dsRentalContractResult.dtTbt_RentalInstrumentDetails != null && dsRentalContractResult.dtTbt_RentalInstrumentDetails.Count > 0)
                                {
                                    dsRentalContractSecond.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                                    tbt_RentalInstrumentDetails rentalInstrumentDetailsDataSecond;
                                    foreach (tbt_RentalInstrumentDetails data in dsRentalContractResult.dtTbt_RentalInstrumentDetails)
                                    {
                                        rentalInstrumentDetailsDataSecond = CommonUtil.CloneObject<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(data);
                                        rentalInstrumentDetailsDataSecond.OCC = strOCC;
                                        dsRentalContractSecond.dtTbt_RentalInstrumentDetails.Add(rentalInstrumentDetailsDataSecond);
                                    }
                                }

                                //dtTbt_RentalMaintenanceDetails
                                if (dsRentalContractResult.dtTbt_RentalMaintenanceDetails != null && dsRentalContractResult.dtTbt_RentalMaintenanceDetails.Count > 0)
                                {
                                    dsRentalContractSecond.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                                    tbt_RentalMaintenanceDetails rentalMaintenanceDetailsDataSecond;
                                    foreach (tbt_RentalMaintenanceDetails data in dsRentalContractResult.dtTbt_RentalMaintenanceDetails)
                                    {
                                        rentalMaintenanceDetailsDataSecond = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(data);
                                        rentalMaintenanceDetailsDataSecond.OCC = strOCC;
                                        dsRentalContractSecond.dtTbt_RentalMaintenanceDetails.Add(rentalMaintenanceDetailsDataSecond);
                                    }
                                }

                                //dtTbt_RentalOperationType
                                if (dsRentalContractResult.dtTbt_RentalOperationType != null && dsRentalContractResult.dtTbt_RentalOperationType.Count > 0)
                                {
                                    dsRentalContractSecond.dtTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                                    tbt_RentalOperationType rentalOperationTypeDataSecond;
                                    foreach (tbt_RentalOperationType data in dsRentalContractResult.dtTbt_RentalOperationType)
                                    {
                                        rentalOperationTypeDataSecond = CommonUtil.CloneObject<tbt_RentalOperationType, tbt_RentalOperationType>(data);
                                        rentalOperationTypeDataSecond.OCC = strOCC;
                                        dsRentalContractSecond.dtTbt_RentalOperationType.Add(rentalOperationTypeDataSecond);
                                    }
                                }

                                //dtTbt_RentalSentryGuard
                                if (dsRentalContractResult.dtTbt_RentalSentryGuard != null && dsRentalContractResult.dtTbt_RentalSentryGuard.Count > 0)
                                {
                                    dsRentalContractSecond.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>();
                                    tbt_RentalSentryGuard rentalSentryGuardDataSecond;
                                    foreach (tbt_RentalSentryGuard data in dsRentalContractResult.dtTbt_RentalSentryGuard)
                                    {
                                        rentalSentryGuardDataSecond = CommonUtil.CloneObject<tbt_RentalSentryGuard, tbt_RentalSentryGuard>(data);
                                        rentalSentryGuardDataSecond.OCC = strOCC;
                                        dsRentalContractSecond.dtTbt_RentalSentryGuard.Add(rentalSentryGuardDataSecond);
                                    }
                                }

                                //dtTbt_RentalSentryGuardDetails
                                if (dsRentalContractResult.dtTbt_RentalSentryGuardDetails != null && dsRentalContractResult.dtTbt_RentalSentryGuardDetails.Count > 0)
                                {
                                    dsRentalContractSecond.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                                    tbt_RentalSentryGuardDetails rentalSentryGuardDetailsDataSecond;
                                    foreach (tbt_RentalSentryGuardDetails data in dsRentalContractResult.dtTbt_RentalSentryGuardDetails)
                                    {
                                        rentalSentryGuardDetailsDataSecond = CommonUtil.CloneObject<tbt_RentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(data);
                                        rentalSentryGuardDetailsDataSecond.OCC = strOCC;
                                        dsRentalContractSecond.dtTbt_RentalSentryGuardDetails.Add(rentalSentryGuardDetailsDataSecond);
                                    }
                                }

                                //dtTbt_RelationType
                                if (dsRentalContractResult.dtTbt_RelationType != null && dsRentalContractResult.dtTbt_RelationType.Count > 0)
                                {
                                    dsRentalContractSecond.dtTbt_RelationType = new List<tbt_RelationType>();
                                    tbt_RelationType relationTypeDataSecond;
                                    foreach (tbt_RelationType data in dsRentalContractResult.dtTbt_RelationType)
                                    {
                                        relationTypeDataSecond = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(data);
                                        relationTypeDataSecond.OCC = strOCC;
                                        dsRentalContractSecond.dtTbt_RelationType.Add(relationTypeDataSecond);
                                    }
                                }

                                //Save second OCC of cancel contract data
                                //dsRentalContractResultSecond = rentralHandler.InsertEntireContract(dsRentalContractSecond);
                                rentralHandler.InsertEntireContractForCTS010(dsRentalContractSecond); //Modify by Jutarat A. on 19092013
                            }
                            else
                            {
                                //Update cancel contract data
                                dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC = strOCC;
                                dsRentalContract.dtTbt_RentalContractBasic[0].LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START;
                                dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus = ContractStatus.C_CONTRACT_STATUS_CANCEL;
                                dsRentalContract.dtTbt_RentalContractBasic[0].CancelBeforeStartProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC = strOCC;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate = doRegisterCancelData.ChangeImplementDate;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].StopCancelReasonType = doRegisterCancelData.StopCancelReasonType;

                                //Add by Jutarat A. on 160072012
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag = FlagType.C_FLAG_OFF;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationSlipNo = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteDate = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationTypeCode = null;
                                //End Add

                                //Add by Jutarat A. on 06082012
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeCurrencyType = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeUsd = null;

                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallCurrencyType = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallUsd = null;

                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractCurrencyType = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractUsd = null;

                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceCurrencyType = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceUsd = null;

                                //End Add

                                dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteEmpNo = null; //Add by Jutarat A. on 27072012

                                //Save OCC of cancel contract data
                                //dsRentalContractResult = rentralHandler.InsertEntireContract(dsRentalContract);
                                rentralHandler.InsertEntireContractForCTS010(dsRentalContract); //Modify by Jutarat A. on 19092013

                                //Cancel booking instrument   
                                iventHandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                                
                                doBooking doBook = new doBooking();
                                doBook.ContractCode = registerRentalContractData.InitialData.ContractCode;
                                iventHandler.CancelBooking(doBook);
                            }
                        }
                        else
                        {
                            //Update cancel contract data
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC = strOCC;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate = doRegisterCancelData.ChangeImplementDate;
                            dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC = strOCC;
                            dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus = ContractStatus.C_CONTRACT_STATUS_END; //Add by Jutarat A. on 28092012
                            
                            if (doRegisterCancelData.ChangeType == "1")
                            {
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT;
                                dsRentalContract.dtTbt_RentalContractBasic[0].LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT;
                                //dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus = ContractStatus.C_CONTRACT_STATUS_END; //Comment by Jutarat A. on 28092012
                            }
                            else
                            {
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL;
                                dsRentalContract.dtTbt_RentalContractBasic[0].LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL;
                                //dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus = ContractStatus.C_CONTRACT_STATUS_CANCEL; //Comment by Jutarat A. on 28092012
                            }

                            dsRentalContract.dtTbt_RentalContractBasic[0].StopConditionProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].StopCancelReasonType = doRegisterCancelData.StopCancelReasonType;

                            //Add by Jutarat A. on 160072012
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag = null; //FlagType.C_FLAG_OFF;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationSlipNo = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteDate = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationTypeCode = null;
                            //End Add

                            //Add by Jutarat A. on 27072012
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteEmpNo = null;

                            dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFeeCurrencyType = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFeeUsd = null;

                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFeeCurrencyType = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFeeUsd = null;

                            dsRentalContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming = null;

                            dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFeeCurrencyType = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFee = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFeeUsd = null;

                            if (sParam.CTS110_Session.InitialData.RemovalDataList != null && sParam.CTS110_Session.InitialData.RemovalDataList.Count > 0
                                && sParam.CTS110_Session.InitialData.RemovalDataList[0].InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL)
                            {
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeCurrencyType = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeUsd = null;

                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallCurrencyType = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallUsd = null;
                            }
                            else
                            {

                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeCurrencyType = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeUsd = null;

                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallCurrencyType = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = null;
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallUsd = null;

                                if (sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData != null)
                                {
                                    foreach (CTS110_CancelContractMemoDetailTemp memoDetailTemp in sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData)
                                    {
                                        if (memoDetailTemp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
                                        {
                                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeCurrencyType = memoDetailTemp.FeeAmountCurrencyType;
                                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee = memoDetailTemp.FeeAmount;
                                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeUsd = memoDetailTemp.FeeAmountUsd;

                                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallCurrencyType = memoDetailTemp.FeeAmountCurrencyType;
                                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = memoDetailTemp.FeeAmount;
                                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallUsd = memoDetailTemp.FeeAmountUsd;

                                            break;
                                        }
                                    }
                                }
                            }
                            
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractCurrencyType = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractUsd = null;

                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceCurrencyType = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceUsd = null;

                            //End Add

                            //Add by Jutarat A. 03102012
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1 = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2 = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].SalesSupporterEmpNo = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanCode = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo3 = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo4 = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo5 = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].DispatchTypeCode = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].PlannerEmpNo = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanCheckerEmpNo = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanCheckDate = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanApproverEmpNo = null;
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanApproveDate = null;
                            //End Add

                            //Save OCC of cancel contract data
                            //dsRentalContractResult = rentralHandler.InsertEntireContract(dsRentalContract);
                            rentralHandler.InsertEntireContractForCTS010(dsRentalContract); //Modify by Jutarat A. on 19092013
                        }

                        /*--- Save CancelContractQuotationData ---*/
                        //MapCancelContractQuotationData
                        dsCancelContractQuotation dsCancelContractQuotationData = new dsCancelContractQuotation();
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo = new List<tbt_CancelContractMemo>();
                        tbt_CancelContractMemo dtTbt_CancelContractMemo = new tbt_CancelContractMemo();

                        if (sParam.CTS110_Session.InitialData.CancelContractQuotationData != null
                            && sParam.CTS110_Session.InitialData.CancelContractQuotationData.dtTbt_CancelContractMemo != null
                            && sParam.CTS110_Session.InitialData.CancelContractQuotationData.dtTbt_CancelContractMemo.Count > 0)
                        {
                            dtTbt_CancelContractMemo = CommonUtil.CloneObject<tbt_CancelContractMemo, tbt_CancelContractMemo>(sParam.CTS110_Session.InitialData.CancelContractQuotationData.dtTbt_CancelContractMemo[0]);
                        }
                        dsCancelContractQuotationData.dtTbt_CancelContractMemo.Add(dtTbt_CancelContractMemo);

                        //doTbt_CancelContractMemo
                        if (dsCancelContractQuotationData.dtTbt_CancelContractMemo.Count > 0)
                        {
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].ContractCode = sParam.CTS110_Session.InitialData.ContractCode;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].OCC = strOCC;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].QuotationFlag = FlagType.C_FLAG_ON;

                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalReturnAmt = sParam.CTS110_Session.InitialData.TotalRefundAmount;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalBillingAmt = sParam.CTS110_Session.InitialData.TotalBillingAmount;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalSlideAmt = sParam.CTS110_Session.InitialData.TotalSlideAmount;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalAmtAfterCounterBalance = sParam.CTS110_Session.InitialData.TotalCounterBalAmount;

                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalReturnAmtUsd = sParam.CTS110_Session.InitialData.TotalRefundAmountUsd;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalBillingAmtUsd = sParam.CTS110_Session.InitialData.TotalBillingAmountUsd;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalSlideAmtUsd = sParam.CTS110_Session.InitialData.TotalSlideAmountUsd;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].TotalAmtAfterCounterBalanceUsd = sParam.CTS110_Session.InitialData.TotalCounterBalAmountUsd;

                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType = doRegisterCancelData.ProcessAfterCounterBalanceType;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceTypeUsd = doRegisterCancelData.ProcessAfterCounterBalanceTypeUsd;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].AutoTransferBillingType = null;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].AutoTransferBillingAmt = 0;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].BankTransferBillingType = null;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].BankTransferBillingAmt = 0;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].CustomerSignatureName = null;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].OtherRemarks = doRegisterCancelData.OtherRemarks;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            dsCancelContractQuotationData.dtTbt_CancelContractMemo[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        }

                        //dtTbt_CancelContractMemoDetail
                        List<tbt_CancelContractMemoDetail> cancelContractMemoDetailList = new List<tbt_CancelContractMemoDetail>();
                        if (sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData != null)
                        {
                            int intSequenceNo = 1;
                            foreach (CTS110_CancelContractMemoDetailTemp memoDetailTemp in sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData)
                            {
                                tbt_CancelContractMemoDetail memoDetail = new tbt_CancelContractMemoDetail();
                                memoDetail.ContractCode = sParam.CTS110_Session.InitialData.ContractCode;
                                memoDetail.OCC = strOCC;
                                memoDetail.SequenceNo = intSequenceNo;
                                memoDetail.BillingType = memoDetailTemp.BillingType;
                                memoDetail.HandlingType = memoDetailTemp.HandlingType;
                                memoDetail.StartPeriodDate = memoDetailTemp.StartPeriodDate;
                                memoDetail.EndPeriodDate = memoDetailTemp.EndPeriodDate;

                                memoDetail.FeeAmountCurrencyType = memoDetailTemp.FeeAmountCurrencyType;
                                memoDetail.FeeAmount = memoDetailTemp.FeeAmount;
                                memoDetail.FeeAmountUsd = memoDetailTemp.FeeAmountUsd;

                                memoDetail.TaxAmountCurrencyType = memoDetailTemp.TaxAmountCurrencyType;
                                memoDetail.TaxAmount = memoDetailTemp.TaxAmount;
                                memoDetail.TaxAmountUsd = memoDetailTemp.TaxAmountUsd;

                                memoDetail.ContractCode_CounterBalance = memoDetailTemp.ContractCode_CounterBalance;

                                #region Normal Fee Amount

                                memoDetail.NormalFeeAmountCurrencyType = memoDetailTemp.NormalFeeAmountCurrencyType;
                                memoDetail.NormalFeeAmount = memoDetailTemp.NormalFeeAmount;
                                memoDetail.NormalFeeAmountUsd = memoDetailTemp.NormalFeeAmountUsd;

                                #endregion

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
                        dsCancelContractQuotation dsCancelContractResult = rentralHandler.CreateCancelContractMemo(dsCancelContractQuotationData);
                        /*-----------------------------------*/

                        //Modify by Jutarat A. on 05102012
                        //Delete unimplemented rental contract data
                        //string strOCCout = rentralHandler.GetLastUnimplementedOCC(registerRentalContractData.InitialData.ContractCode);
                        //if (String.IsNullOrEmpty(strOCCout) == false)
                        //{
                        //    dsRentalContractData dsRentalContractUnimplement = rentralHandler.DeleteEntireOCC(registerRentalContractData.InitialData.ContractCode, strOCCout, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                        //}
                        string strOCCout = rentralHandler.GetLastUnimplementedOCC(registerRentalContractData.InitialData.ContractCode);
                        while (String.IsNullOrEmpty(strOCCout) == false)
                        {
                            rentralHandler.DeleteEntireOCC(registerRentalContractData.InitialData.ContractCode, strOCCout, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                            strOCCout = rentralHandler.GetLastUnimplementedOCC(registerRentalContractData.InitialData.ContractCode);
                        }
                        //End Modify

                        //Delete installation basic
                        IInstallationHandler installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                        if (dtRentalContractBasicData.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            bool blnProcessResult = installHandler.DeleteInstallationBasicData(registerRentalContractData.InitialData.ContractCode);
                        }

                        //Lock quotation
                        quotHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                        bool blnLockQuotationResult = quotHandler.LockQuotation(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode
                                                    , null
                                                    , LockStyle.C_LOCK_STYLE_ALL);

                        //Update quotation data
                        doUpdateQuotationData doUpdateQuot = new doUpdateQuotationData();
                        doUpdateQuot.QuotationTargetCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
                        doUpdateQuot.Alphabet = null;
                        doUpdateQuot.LastUpdateDate = DateTime.MinValue; //null;
                        doUpdateQuot.ContractCode = registerRentalContractData.InitialData.ContractCode;
                        doUpdateQuot.ActionTypeCode = ActionType.C_ACTION_TYPE_CANCEL;
                        int iUpdateQuotRowCount = quotHandler.UpdateQuotationData(doUpdateQuot);

                        
                        billingMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                        billingTempHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

                        //Add by Jutarat A. on 26092012
                        //Keep installation fee of start service in case new installation is completete and contract status is before
                        List<tbt_BillingTemp> billingTempList = null;
                        if (dsRentalContract.dtTbt_RentalContractBasic != null
                            && dsRentalContract.dtTbt_RentalContractBasic.Count > 0
                            && dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_ON
                            && dsRentalContract.dtTbt_RentalSecurityBasic != null
                            && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0
                            && dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                        {
                            //Get installation fee for billing detail of start service from billing temp
                            billingTempList = billingTempHandler.GetTbt_BillingTemp(sParam.CTS110_Session.InitialData.ContractCode, null);
                            if (billingTempList != null && billingTempList.Count > 0)
                            {
                                billingTempList = (from t in billingTempList
                                                   where (t.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
                                                            || t.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                                                       && t.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE
                                                   select t).ToList<tbt_BillingTemp>();
                            }
                        }

                        //Delete all billing data in billing temp
                        billingTempHandler.DeleteBillingTempByContractCode(sParam.CTS110_Session.InitialData.ContractCode);


                        //Keep  billing data of installation fee of start service
                        if (billingTempList != null && billingTempList.Count > 0)
                        {
                            foreach (tbt_BillingTemp data in billingTempList)
                            {
                                tbt_BillingTemp dataTemp = CommonUtil.CloneObject<tbt_BillingTemp, tbt_BillingTemp>(data);
                                billingTempHandler.InsertBillingTemp(dataTemp);
                            }
                        }
                        //End Add

                        //Keep billing data
                        List<tbt_CancelContractMemoDetail> cancelContractMemoDetailListTemp = cancelContractMemoDetailList.FindAll(delegate(tbt_CancelContractMemoDetail s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; });
                        if (cancelContractMemoDetailListTemp != null && cancelContractMemoDetailListTemp.Count > 0)
                        {
                        
                            foreach (CTS110_BillingTemp data in billingTempDataList)
                            {
                                if (data.Sequence == doRegisterCancelData.RemovalRowSequence)
                                {
                                    string strBillingClientCode = string.Empty;
                                    if (String.IsNullOrEmpty(data.BillingOCC))
                                    {
                                        //Manage billing client
                                        foreach (CTS110_BillingClientData billClient in billingClientDataList)
                                        {
                                            if (billClient.Sequence == doRegisterCancelData.RemovalRowSequence)
                                            {
                                                if (String.IsNullOrEmpty(billClient.BillingClientCode))
                                                {
                                                    SECOM_AJIS.DataEntity.Master.tbm_BillingClient billClientTemp = CommonUtil.CloneObject<CTS110_BillingClientData, SECOM_AJIS.DataEntity.Master.tbm_BillingClient>(billClient);
                                                    billClient.BillingClientCode = billingMasterHandler.ManageBillingClient(billClientTemp);
                                                }

                                                strBillingClientCode = billClient.BillingClientCode;
                                            }
                                        }
                                    }

                                    //Comment by Jutarat A. on 26092012
                                    ////Delete all billing data in billing temp
                                    //billingTempHandler.DeleteBillingTempByContractCode(sParam.CTS110_Session.InitialData.ContractCode); //Add by Jutarat A. on 24072012
                                    //End Comment
                             

                                    tbt_BillingTemp doBillingTemp = CommonUtil.CloneObject<CTS110_BillingTemp, tbt_BillingTemp>(data);
                                    doBillingTemp.ContractCode = sParam.CTS110_Session.InitialData.ContractCode;
                                    doBillingTemp.OCC = strOCC;
                                    doBillingTemp.BillingClientCode = String.IsNullOrEmpty(data.BillingOCC) ? strBillingClientCode : data.BillingClientCode;
                                    //doBillingTemp.SequenceNo = intBillTmpSeqNo;
                                    doBillingTemp.BillingTiming = BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION;
                                    doBillingTemp.SendFlag = "0";
                                    doBillingTemp.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;

                                    doBillingTemp.BillingAmtCurrencyType = doRegisterCancelData.RemovalAmountCurrencyType;
                                    if (doBillingTemp.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                    {
                                        doBillingTemp.BillingAmt = null;
                                        doBillingTemp.BillingAmtUsd = doRegisterCancelData.RemovalAmount;
                                    }
                                    else
                                    {
                                        doBillingTemp.BillingAmt = doRegisterCancelData.RemovalAmount;
                                        doBillingTemp.BillingAmtUsd = null;
                                    }
                                    
                                    doBillingTemp.PayMethod = doRegisterCancelData.PaymentMethod;

                                    //Keep billing data
                                    billingTempHandler.InsertBillingTemp(doBillingTemp);
                                }

                            }
                        }

                        doBillingTempBasic billingBasicData = null;
                        doBillingTempDetail billingDetailData = null;
                        bool blnCompleteInstallFlag = false;

                        foreach (CTS110_CancelContractMemoDetailTemp memoDetailTemp in sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData)
                        {
                            if (memoDetailTemp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
                            {
                                //New billing client for cancel contract fee
                                masHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                                tbm_BillingClient billingClientData = new tbm_BillingClient();

                                //Load customer data of contract target
                                List<tbm_Customer> customerList = masHandler.GetTbm_Customer(dtRentalContractBasicData.ContractTargetCustCode);
                                if (customerList.Count > 0)
                                {
                                    billingClientData.NameEN = customerList[0].CustNameEN;
                                    billingClientData.NameLC = customerList[0].CustNameLC;
                                    billingClientData.FullNameEN = customerList[0].CustFullNameEN;
                                    billingClientData.FullNameLC = customerList[0].CustFullNameLC;
                                    billingClientData.AddressEN = customerList[0].AddressFullEN;
                                    billingClientData.AddressLC = customerList[0].AddressFullLC;
                                    billingClientData.RegionCode = customerList[0].RegionCode;
                                    billingClientData.PhoneNo = customerList[0].PhoneNo;
                                    billingClientData.BusinessTypeCode = customerList[0].BusinessTypeCode;
                                    billingClientData.IDNo = customerList[0].IDNo;
                                    billingClientData.CustTypeCode = customerList[0].CustTypeCode;
                                }

                                //Manage billing client
                                billingClientData.BillingClientCode = billingMasterHandler.ManageBillingClient(billingClientData);

                                //Create doBillingBasicCancel
                                billingBasicData = new doBillingTempBasic();
                                billingBasicData.ContractCode = dtRentalContractBasicData.ContractCode;
                                billingBasicData.BillingClientCode = billingClientData.BillingClientCode;
                                billingBasicData.BillingOfficeCode = dtRentalContractBasicData.OperationOfficeCode;
                                billingBasicData.BillingTargetCode = null;
                                billingBasicData.PaymentMethod = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeePayMethod;
                                billingBasicData.ContractBillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE;

                                billingBasicData.BillingAmountCurrencyType = memoDetailTemp.FeeAmountCurrencyType;
                                billingBasicData.BillingAmount = memoDetailTemp.FeeAmount;
                                billingBasicData.BillingAmountUsd = memoDetailTemp.FeeAmountUsd;

                                billingBasicData.BillingCycle = null;
                                billingBasicData.CalculationDailyFee = null;

                                //Create doBillingDetailCancel
                                billingDetailData = new doBillingTempDetail();
                                billingDetailData.ContractCode = dtRentalContractBasicData.ContractCode;
                                billingDetailData.BillingOCC = null;
                                billingDetailData.ContractBillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE;
                                billingDetailData.BillingDate = doRegisterCancelData.ChangeImplementDate.Value;

                                billingDetailData.BillingAmountCurrencyType = memoDetailTemp.FeeAmountCurrencyType;
                                billingDetailData.BillingAmount = memoDetailTemp.FeeAmount;
                                billingDetailData.BillingAmountUsd = memoDetailTemp.FeeAmountUsd;
                                
                                billingDetailData.PaymentMethod = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeePayMethod;



                                //billingInterfaceHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                                //billingInterfaceHandler.SendBilling_RentalCancel(registerRentalContractData.InitialData.ContractCode, doRegisterCancelData.ChangeImplementDate.Value, billingBasicData, billingDetailData, blnCompleteInstallFlag);
                                //break;
                            }
                        }

                        //Send billing data
                        //bool blnCompleteInstallFlag = false;
                        if (sParam.CTS110_Session.InitialData.RemovalDataList != null && sParam.CTS110_Session.InitialData.RemovalDataList.Count > 0
                            && sParam.CTS110_Session.InitialData.RemovalDataList[0].InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL)
                        {
                            blnCompleteInstallFlag = true;
                        }

                        billingInterfaceHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                        billingInterfaceHandler.SendBilling_RentalCancel(registerRentalContractData.InitialData.ContractCode, doRegisterCancelData.ChangeImplementDate.Value, billingBasicData, billingDetailData, blnCompleteInstallFlag);
                    }
                    /*--------------------------*/

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

        /// <summary>
        /// Get HandlingType data to ComboBox
        /// </summary>
        /// <param name="strFeeType"></param>
        /// <returns></returns>
        public ActionResult CTS110_GetHandlingType(string strFeeType)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doMiscTypeCode> lst = new List<doMiscTypeCode>();

                if (String.IsNullOrEmpty(strFeeType) == false)
                {
                    if (strFeeType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE
                        || strFeeType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE
                        || strFeeType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE
                        || strFeeType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
                    {
                        string[] strHandlingList = new string[2];

                        //Comment by Jutarat A. on 07082012
                        //if (strFeeType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE
                        //    || strFeeType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE
                        //    || strFeeType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE)
                        if (strFeeType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE
                            || strFeeType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE)
                        {
                            strHandlingList[0] = HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
                            strHandlingList[1] = HandlingType.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE;
                        }
                        else if (strFeeType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE
                            || strFeeType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE) //Add by Jutarat A. on 07082012
                        {
                            strHandlingList[0] = HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
                            strHandlingList[1] = string.Empty;
                        }

                        lst = (from t in GetHandlingMiscType_CTS110()
                               where strHandlingList.Contains<string>(t.ValueCode)
                               select t).ToList<doMiscTypeCode>();
                    }
                    else
                    {
                        lst = GetHandlingMiscType_CTS110();
                    }
                }

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(lst, "ValueCodeDisplay", "ValueCode");
                res.ResultData = cboModel;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get FeeType data to ComboBox
        /// </summary>
        /// <param name="strProductType"></param>
        /// <returns></returns>
        public ActionResult CTS110_GetFeeType(string strProductType)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doMiscTypeCode> lst = new List<doMiscTypeCode>();

                if (String.IsNullOrEmpty(strProductType) == false)
                {
                    if (strProductType != ProductType.C_PROD_TYPE_AL
                        && strProductType == ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {
                        string[] strHandlingList = new string[2];

                        //lst = (from t in GetHandlingMiscType_CTS110()
                        //       where strHandlingList.Contains<string>(t.ValueCode)
                        //       select t).ToList<doMiscTypeCode>();
                    }
                    else
                    {
                        //lst = GetHandlingMiscType_CTS110();
                    }
                }

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(lst, "ValueCodeDisplay", "ValueCode");
                res.ResultData = cboModel;
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
        private void Bind_CTS110(doRentalContractBasicInformation doRentalContractBasic)
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
            ViewBag.LastChangeType = doRentalContractBasic.LastChangeTypeCodeName;
        }

        /// <summary>
        /// Check authority data of screen
        /// </summary>
        /// <param name="res"></param>
        /// <param name="strContractCodeLong"></param>
        /// <param name="isInitScreen"></param>
        /// <returns></returns>
        private dsRentalContractData CheckDataAuthority_CTS110(ObjectResultData res, string strContractCodeLong, bool isInitScreen = false)
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
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, new string[] { strContractCode });
                }
            }

            return dsRentalContract;
        }

        /// <summary>
        /// Validate business for CancelContract
        /// </summary>
        /// <param name="res"></param>
        /// <param name="cancelContractMemoDetailData"></param>
        private void ValidateBusinessCancelContract_CTS110(ObjectResultData res, tbt_CancelContractMemoDetail cancelContractMemoDetailData)
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

                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                memoDetailTempList = sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData;

                foreach (CTS110_CancelContractMemoDetailTemp memoDetail in memoDetailTempList)
                {
                    if (cancelContractMemoDetailData.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                    {
                        if (memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                        {
                            //Move to ValidateBusinessCancelContractForWarning_CTS110()
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
                            //Move to ValidateBusinessCancelContractForWarning_CTS110()
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
                    if (sParam.CTS110_Session.InitialData.ContractCode == strContractCode_CounterBalanceLong.ToUpper())
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
        private void ValidateBusinessCancelContractForWarning_CTS110(ObjectResultData res, tbt_CancelContractMemoDetail cancelContractMemoDetailData)
        {
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
            List<CTS110_CancelContractMemoDetailTemp> memoDetailTempList;

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                memoDetailTempList = sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData;

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
        /// <param name="doRegisterCancelData"></param>
        private void ValidateBusiness_CTS110(ObjectResultData res, string strContractCodeLong, CTS110_RegisterCancelData doRegisterCancelData)
        {
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;
            IInstallationHandler installHandler;
            List<tbt_InstallationBasic> dtTbt_InstallationBasicList;
            tbt_InstallationBasic dtTbt_InstallationBasicData;

            tbt_RentalContractBasic dtRentalContractBasicData;
            tbt_RentalContractBasic dtRentalContractBasicDataResult;

            decimal maximumDigit = 999999999999.99M;
            decimal minimumDigit = -999999999999.99M;

            try
            {
                CTS110_ScreenParameter sParam = GetScreenObject<CTS110_ScreenParameter>();
                dtRentalContractBasicData = sParam.CTS110_Session.InitialData.RentalContractBasicData;

                //Validate cancel date
                if (doRegisterCancelData.ChangeImplementDate > DateTime.Now.Date)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3154, null, new string[] { "dpCancelDate" });
                    return;
                }

                if (dtRentalContractBasicData.ContractStatus != ContractStatus.C_CONTRACT_STATUS_BEF_START
                    && doRegisterCancelData.ChangeImplementDate < dtRentalContractBasicData.LastChangeImplementDate)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3155, null, new string[] { "dpCancelDate" });
                    return;
                }

                //Validate approve no.
                if (String.IsNullOrEmpty(doRegisterCancelData.ApproveNo1) && (String.IsNullOrEmpty(doRegisterCancelData.ApproveNo2) == false))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3009);
                    return;
                }

                /*--- Validate rental contract data ---*/
                dsRentalContract = CheckDataAuthority_CTS100(res, strContractCodeLong);
                if (res.IsError)
                    return;

                if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
                {
                    dtRentalContractBasicDataResult = dsRentalContract.dtTbt_RentalContractBasic[0];

                    string strContractCodeShort = comUtil.ConvertContractCode(strContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    if (dtRentalContractBasicDataResult.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3104, null, new string[] { "txtSpecifyContractCode" });
                        return;
                    }
                    else if (dtRentalContractBasicDataResult.ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL
                            || dtRentalContractBasicDataResult.ContractStatus == ContractStatus.C_CONTRACT_STATUS_END
                            || dtRentalContractBasicDataResult.ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3105, new string[] { strContractCodeShort }, new string[] { "txtSpecifyContractCode" });
                        return;
                    }
                    else if (dtRentalContractBasicDataResult.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                    {
                        rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        string strOCCout = rentralHandler.GetLastUnimplementedOCC(strContractCodeLong);
                        if (String.IsNullOrEmpty(strOCCout) == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3106, null, new string[] { "txtSpecifyContractCode" });
                            return;
                        }
                    }
                    /*----------------------------------------*/

                    /*--- Validate installation basic data ---*/
                    //if (dtRentalContractBasicDataResult.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    //{
                        installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                        dtTbt_InstallationBasicList = installHandler.GetTbt_InstallationBasicData(strContractCodeLong);
                        if (dtTbt_InstallationBasicList != null && dtTbt_InstallationBasicList.Count > 0)
                        {
                            dtTbt_InstallationBasicData = dtTbt_InstallationBasicList[0];

                            if (dtTbt_InstallationBasicData.InstallationStatus != InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED
                               && (dtTbt_InstallationBasicData.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL
                                    && dtTbt_InstallationBasicData.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                                    && dtTbt_InstallationBasicData.InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL)
                                )
                            {
                                string strContractCode = comUtil.ConvertContractCode(strContractCodeLong, CommonUtil.CONVERT_TYPE.TO_SHORT);
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3103, new string[] { strContractCode }, new string[] { "txtSpecifyContractCode" });
                                return;
                            }
                        }
                    //}
                }

                //if (sParam.CTS110_Session.InitialData.DefaultRemovalFee > 0)
                if (sParam.CTS110_Session.InitialData.RemovalDataList != null && sParam.CTS110_Session.InitialData.RemovalDataList.Count > 0)
                {
                    if (sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData != null && sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData.Count > 0)
                    {
                        List<CTS110_CancelContractMemoDetailTemp> cancelMemoList = sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData.FindAll(delegate(CTS110_CancelContractMemoDetailTemp s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; });
                        if (cancelMemoList == null || cancelMemoList.Count == 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3219);
                            return;
                        }
                    }
                    else   
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3219);
                        return;
                    } 
                }

                //Add by Jutarat A. on 17072012
                //Validate billing target
                if (sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData != null)
                {
                    List<CTS110_CancelContractMemoDetailTemp> cancelMemoList = sParam.CTS110_Session.InitialData.CancelContractMemoDetailTempData.FindAll(delegate(CTS110_CancelContractMemoDetailTemp s)
                                                                                { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE && s.HandlingType == HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE; });
                    if (cancelMemoList != null || cancelMemoList.Count > 0)
                    {
                        if (String.IsNullOrEmpty(doRegisterCancelData.RemovalRowSequence))
                        {
                            if (sParam.CTS110_Session.InitialData.RemovalDataList != null && sParam.CTS110_Session.InitialData.RemovalDataList.Count > 0
                                && sParam.CTS110_Session.InitialData.RemovalDataList[0].InstallationType != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL) //Add by Jutarat A. on 26072012
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3290);
                                return;
                            }

                        }
                    }
                }
                //End Add

                string strTotalLabel = string.Empty;
                if (sParam.CTS110_Session.InitialData.TotalSlideAmount > maximumDigit || sParam.CTS110_Session.InitialData.TotalSlideAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalSlideAmountRp";
                }
                else if (sParam.CTS110_Session.InitialData.TotalSlideAmountUsd > maximumDigit || sParam.CTS110_Session.InitialData.TotalSlideAmountUsd < minimumDigit)
                {
                    strTotalLabel = "lblTotalSlideAmountUsd";
                }
                else if (sParam.CTS110_Session.InitialData.TotalRefundAmount > maximumDigit || sParam.CTS110_Session.InitialData.TotalRefundAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalRefundAmountRp";
                }
                else if (sParam.CTS110_Session.InitialData.TotalRefundAmountUsd > maximumDigit || sParam.CTS110_Session.InitialData.TotalRefundAmountUsd < minimumDigit)
                {
                    strTotalLabel = "lblTotalRefundAmountUsd";
                }
                else if (sParam.CTS110_Session.InitialData.TotalBillingAmount > maximumDigit || sParam.CTS110_Session.InitialData.TotalBillingAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalBillingAmountRp";
                }
                else if (sParam.CTS110_Session.InitialData.TotalBillingAmountUsd > maximumDigit || sParam.CTS110_Session.InitialData.TotalBillingAmountUsd < minimumDigit)
                {
                    strTotalLabel = "lblTotalBillingAmountUsd";
                }
                else if (sParam.CTS110_Session.InitialData.TotalCounterBalAmount > maximumDigit || sParam.CTS110_Session.InitialData.TotalCounterBalAmount < minimumDigit)
                {
                    strTotalLabel = "lblTotalCounterBalAmountRp";
                }
                else if (sParam.CTS110_Session.InitialData.TotalCounterBalAmountUsd > maximumDigit || sParam.CTS110_Session.InitialData.TotalCounterBalAmountUsd < minimumDigit)
                {
                    strTotalLabel = "lblTotalCounterBalAmountUsd";
                }

                if (String.IsNullOrEmpty(strTotalLabel) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT,
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3252,
                                        new string[] { strTotalLabel },
                                        null);
                    return;
                }
                /*----------------------------------------*/
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Load data of Master and Mapping to BillingClient
        /// </summary>
        /// <param name="res"></param>
        /// <param name="billingClientData"></param>
        private void LoadMasterData_CTS110(ObjectResultData res, dtBillingClientData billingClientData)
        {
            IMasterHandler masterHandler;

            try
            {
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                //Get nationality of billing client
                List<tbm_Region> regionList = masterHandler.GetTbm_Region();
                regionList = regionList.FindAll(delegate(tbm_Region s) { return s.RegionCode == billingClientData.RegionCode; });
                if (regionList.Count > 0)
                {
                    billingClientData.Nationality = regionList[0].Nationality;
                    billingClientData.NationalityEN = regionList[0].NationalityEN;
                    billingClientData.NationalityJP = regionList[0].NationalityJP;
                    billingClientData.NationalityLC = regionList[0].NationalityLC;
                }

                //Get business type of billing client
                List<tbm_BusinessType> bizTypeList = masterHandler.GetTbm_BusinessType();
                bizTypeList = bizTypeList.FindAll(delegate(tbm_BusinessType s) { return s.BusinessTypeCode == billingClientData.BusinessTypeCode; });
                if (bizTypeList.Count > 0)
                {
                    billingClientData.BusinessTypeName = bizTypeList[0].BusinessTypeName;
                    billingClientData.BusinessTypeNameEN = bizTypeList[0].BusinessTypeNameEN;
                    billingClientData.BusinessTypeNameJP = bizTypeList[0].BusinessTypeNameJP;
                    billingClientData.BusinessTypeNameLC = bizTypeList[0].BusinessTypeNameLC;
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Get detail of BillingTarget data
        /// </summary>
        /// <param name="dtRetriveBillingTempData"></param>
        /// <param name="billingClientData"></param>
        /// <returns></returns>
        private CTS051_DOBillingTargetDetailData GetBillingTargetDetailData_CTS110(CTS110_BillingTemp dtRetriveBillingTempData, dtBillingClientData billingClientData)
        {
            CTS051_DOBillingTargetDetailData doBillingTargetDetail = new CTS051_DOBillingTargetDetailData();
            doBillingTargetDetail.BillingTargetCodeDetail = dtRetriveBillingTempData.BillingTargetCodeShort;
            doBillingTargetDetail.BillingOCC = dtRetriveBillingTempData.BillingOCC;
            doBillingTargetDetail.BillingOfficeCode = dtRetriveBillingTempData.BillingOfficeCode;
            //doBillingTargetDetail.Status = dtRetriveBillingTempData.Status;
            doBillingTargetDetail.BillingClientCodeDetail = billingClientData.BillingClientCodeShort;
            doBillingTargetDetail.FullNameEN = billingClientData.FullNameEN;
            doBillingTargetDetail.BranchNameEN = billingClientData.BranchNameEN;
            doBillingTargetDetail.AddressEN = billingClientData.AddressEN;
            doBillingTargetDetail.FullNameLC = billingClientData.FullNameLC;
            doBillingTargetDetail.BranchNameLC = billingClientData.BranchNameLC;
            doBillingTargetDetail.AddressLC = billingClientData.AddressLC;
            doBillingTargetDetail.Nationality = billingClientData.Nationality;
            doBillingTargetDetail.PhoneNo = billingClientData.PhoneNo;
            doBillingTargetDetail.BusinessType = billingClientData.BusinessTypeName;
            doBillingTargetDetail.IDNo = billingClientData.IDNo;

            return doBillingTargetDetail;
        }

        /// <summary>
        /// Get MiscType of Handling data
        /// </summary>
        /// <returns></returns>
        private List<doMiscTypeCode> GetHandlingMiscType_CTS110()
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
            {
                new doMiscTypeCode()
                {
                    FieldName = MiscType.C_HANDLING_TYPE,
                    ValueCode = "%"
                }
            };

            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            lst = hand.GetMiscTypeCodeList(miscs);

            return lst;
        }

        /// <summary>
        /// Get Name of BillingOffice
        /// </summary>
        /// <param name="strBillingOfficeCode"></param>
        /// <returns></returns>
        private string GetBillingOfficeName_CTS110(string strBillingOfficeCode)
        {
            string strBillingOfficeName = string.Empty;
            if (CommonUtil.dsTransData != null)
            {
                List<OfficeDataDo> billingOfficeTemp = (from t in CommonUtil.dsTransData.dtOfficeData
                                                        where t.FunctionBilling == FunctionBilling.C_FUNC_BILLING_YES
                                                        && t.OfficeCode == strBillingOfficeCode
                                                        select t).ToList<OfficeDataDo>();

                CommonUtil.MappingObjectLanguage<OfficeDataDo>(billingOfficeTemp);

                if (billingOfficeTemp != null && billingOfficeTemp.Count > 0)
                {
                    strBillingOfficeName = billingOfficeTemp[0].OfficeName;
                }
            }

            return strBillingOfficeName;
        }

        /// <summary>
        /// Check PeriodDate is Overlap
        /// </summary>
        /// <param name="firstDateFrom"></param>
        /// <param name="firstDateTo"></param>
        /// <param name="secondDateFrom"></param>
        /// <param name="secondDateTo"></param>
        /// <returns></returns>
        private bool CheckOverlapPeriodDate_CTS110(DateTime? firstDateFrom, DateTime? firstDateTo, DateTime? secondDateFrom, DateTime? secondDateTo)
        {
            bool result = false;

            if ((secondDateFrom <= firstDateFrom && secondDateTo >= firstDateFrom)
                || (secondDateFrom >= firstDateFrom && secondDateTo <= firstDateTo)
                || (secondDateFrom <= firstDateTo && secondDateTo >= firstDateTo)
               )
            {
                result = true;
            }

            return result;
        }

        #endregion

    }
}
