//*********************************
// Create by: Jutarat A.
// Create date: 13/Dec/2011
// Update date: 13/Dec/2011
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
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check system suspending, user’s permission and user’s authority of screen
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS160_Authority(CTS160_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                #region Check is Suspending

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion
                #region Check screen permission

                if (CheckUserPermission(ScreenID.C_SCREEN_ID_QTN_DETAIL, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                #endregion

                #region Get contract code from previous screen

                //param.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                if (param.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                        param.ContractCode = param.CommonSearch.ContractCode;
                }

                #endregion

                if (CommonUtil.IsNullOrEmpty(param.ContractCode) == false)
                {
                    CTS160_InitialContractData(res, param);
                    if (res.IsError == true)
                        return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS160_ScreenParameter>("CTS160", param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS160")]
        public ActionResult CTS160()
        {
            ViewBag.DisabledrdoContract = true;
            ViewBag.DisabledrdoMemorandum = true;
            ViewBag.DisabledrdoNotice = true;
            ViewBag.DisabledrdoCoverLetter = true;

            try
            {
                CTS160_ScreenParameter param = GetScreenObject<CTS160_ScreenParameter>();
                if (CommonUtil.IsNullOrEmpty(param.ContractCode) == false)
                {
                    CTS160_DocumentTemplateData template = SetDocumentTemplateData(param);
                    CTS160_DocumentState state = SetSelectDocumentState(param);

                    ViewBag.ContractType = ((int)template.ContractType).ToString();
                    ViewBag.ContractCode = template.ContractCode;
                    ViewBag.OCCAlphabet = template.OCCAlphabet;
                    ViewBag.ContractTargetNameEN = template.ContractTargetNameEN;
                    ViewBag.RealCustomerNameEN = template.RealCustomerNameEN;
                    ViewBag.SiteNameEN = template.SiteNameEN;
                    ViewBag.ContractTargetNameLC = template.ContractTargetNameLC;
                    ViewBag.RealCustomerNameLC = template.RealCustomerNameLC;
                    ViewBag.SiteNameLC = template.SiteNameLC;
                    ViewBag.ChangeTypeName = template.ChangeTypeName;

                    ViewBag.DisabledrdoContract = !state.ContractReportFlag;
                    ViewBag.DisabledrdoMemorandum = !state.MemorandumFlag;
                    ViewBag.DisabledrdoNotice = !state.NoticeFlag;
                    ViewBag.DisabledrdoCoverLetter = !state.CoverLetterFlag;
                }
            }
            catch (Exception)
            {
            }

            return View();
        }

        #endregion
        #region Actions

        /// <summary>
        /// Validate and retrieve data when click [Retrieve] button on ‘Specify contract code/quotation code’ section
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <returns></returns>
        public ActionResult CTS160_RetrieveContractData(string ContractCode, string OCC)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (CommonUtil.IsNullOrEmpty(ContractCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblContractQuotationTargetCode" },
                                        new string[] { "ContractCode" });

                    return Json(res);
                }

                CTS160_ScreenParameter param = GetScreenObject<CTS160_ScreenParameter>();
                param.ContractCode = ContractCode;
                param.OCCAlphabet = CommonUtil.IsNullOrEmpty(OCC) ? "" : OCC.ToUpper(); //OCC; //Modify by Jutarat A. on 02052013

                CTS160_InitialContractData(res, param, true, true);
                if (res.IsError == true)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                res.ResultData = new object[] {
                    SetDocumentTemplateData(param),
                    SetSelectDocumentState(param)
                };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Map data and show ‘Cover letter information’ section when click [Select] button on ‘Document template’ section
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS160_SelectDocumentTemplate(CTS160_DocumentTemplateCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS160_ScreenParameter param = GetScreenObject<CTS160_ScreenParameter>();

                #region Validate condition

                if (cond.ContractReportFlag == false
                    && cond.MemorandumFlag == false
                    && cond.NoticeFlag == false
                    && cond.CoverLetterFlag == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "lblTitleDocumentTemplate" });

                    return Json(res);
                }
                if (cond.ContractReportFlag == true
                    && CommonUtil.IsNullOrEmpty(cond.ContractLanguage))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                            ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT,
                                            MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0007,
                                            new string[] { "lblContractDocumentLanguage" },
                                            new string[] { "ContractDocumentLanguage" });

                    return Json(res);
                }
                if (cond.CoverLetterFlag == true
                    && CommonUtil.IsNullOrEmpty(cond.DocumentName))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                            ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT,
                                            MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0007,
                                            new string[] { "lblDocumentName" },
                                            new string[] { param.ContractType == CTS160_CONTRACT_TYPE.SALE ? "SaleDocumentName" : "RentalDocumentName" });

                    return Json(res);
                }

                #endregion
                #region Mapping data

                dsTransDataModel dsTrans = CommonUtil.dsTransData;

                #region Get office name

                string mainOfficeName = null;
                List<OfficeDataDo> officeDataList = null;
                officeDataList = (from t in CommonUtil.dsTransData.dtOfficeData
                                  where t.OfficeCode == dsTrans.dtUserData.MainOfficeCode
                                  select t).ToList<OfficeDataDo>();
                if (officeDataList != null)
                {
                    if (officeDataList.Count > 0)
                        mainOfficeName = officeDataList[0].OfficeName;
                }

                #endregion
                #region code by contract type

                string changeType = null;
                string productCode = null;
                string productTypeCode = null;
                string lineTypeCode = null;
                string lineOwnerCode = null;
                decimal? contractFee = null;
                string contractFeeCurrencyType = null;
                decimal? quotationFee = null;
                string quotationFeeCurrencyType = null;
                decimal? depositFee = null;
                string depositFeeCurrencyType = null;
                string paymentMethod = null;
                int? billingCycle = null;
                int? creditTerm = null;
                bool? fireMonitoringFlag = false;
                bool? crimePreventionFlag = false;
                bool? emergencyReportFlag = false;
                bool? facilityMonitoringFlag = false;
                int? contractDuration = null;
                string operationOffice = null;
                string occ = null;

                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    changeType = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeType;
                    productCode = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ProductCode;
                    productTypeCode = param.dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode;
                    lineTypeCode = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1;
                    lineOwnerCode = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1;

                    //contractFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFee;
                    //quotationFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalContractFee;
                    //depositFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee;

                    #region Contract Fee

                    contractFeeCurrencyType = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType;
                    if (contractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        contractFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd;
                    else
                        contractFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFee;

                    #endregion
                    #region Quotation Fee

                    quotationFeeCurrencyType = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalContractFeeCurrencyType;
                    if (quotationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        quotationFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalContractFeeUsd;
                    else
                        quotationFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalContractFee;

                    #endregion
                    #region Deposit Fee

                    depositFeeCurrencyType = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFeeCurrencyType;
                    if (depositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        depositFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFeeUsd;
                    else
                        depositFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee;

                    #endregion

                    paymentMethod = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeePayMethod;

                    if (param.dtTbt_BillingBasic != null)
                    {
                        //Modify by Jutarat A. on 18122012
                        //List<tbt_BillingBasic> dtTbt_BillingBasicNoStop =
                        //    (from t in param.dtTbt_BillingBasic
                        //     where t.MonthlyBillingAmount > 0
                        //     && t.StopBillingFlag == FlagType.C_FLAG_OFF
                        //     select t).ToList<tbt_BillingBasic>();
                        //if (dtTbt_BillingBasicNoStop != null)
                        //{
                        //    if (dtTbt_BillingBasicNoStop.Count > 0)
                        //    {
                        //        billingCycle = dtTbt_BillingBasicNoStop[0].BillingCycle;
                        //        creditTerm = dtTbt_BillingBasicNoStop[0].CreditTerm;
                        //    }
                        //}

                        List<tbt_BillingBasic> dtTbt_BillingBasicNoStop = null;
                        if (param.dsRentalContractData.dtTbt_RentalContractBasic != null
                            && param.dsRentalContractData.dtTbt_RentalContractBasic.Count > 0)
                        {
                            // 20170302 nakajima modify start
                            dtTbt_BillingBasicNoStop = (from t in param.dtTbt_BillingBasic
                                                             where t.MonthlyBillingAmount > 0 || t.MonthlyBillingAmountUsd > 0
                                                             orderby t.BillingOCC
                                                             select t).ToList<tbt_BillingBasic>();
                            // 20170302 nakajima modify end
                            if (dtTbt_BillingBasicNoStop != null && dtTbt_BillingBasicNoStop.Count > 0)
                            {
                                if (param.dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                                {
                                    billingCycle = dtTbt_BillingBasicNoStop.Count > 1 ? null : dtTbt_BillingBasicNoStop[0].BillingCycle;
                                    creditTerm = dtTbt_BillingBasicNoStop.Count > 1 ? null : dtTbt_BillingBasicNoStop[0].CreditTerm;
                                }
                                else
                                {
                                    billingCycle = dtTbt_BillingBasicNoStop[0].BillingCycle;
                                    creditTerm = dtTbt_BillingBasicNoStop[0].CreditTerm;
                                }
                            }
                        }
                        //End Modify
                    }

                    fireMonitoringFlag = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].FireMonitorFlag;
                    crimePreventionFlag = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].CrimePreventFlag;
                    emergencyReportFlag = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag;
                    facilityMonitoringFlag = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag;
                    contractDuration = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractDurationMonth;
                    operationOffice = param.dsRentalContractData.dtTbt_RentalContractBasic[0].OperationOfficeCode;
                    //occ = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OCC; //Comment by Jutarat A. on 19122012
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.DRAFT_RENTAL)
                {
                    productCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.ProductCode;
                    productTypeCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.ProductTypeCode;
                    lineTypeCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.PhoneLineTypeCode1;
                    lineOwnerCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.PhoneLineOwnerTypeCode1;

                    //contractFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderContractFee;
                    //quotationFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.NormalContractFee;
                    //depositFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderDepositFee;

                    #region Contract Fee

                    contractFeeCurrencyType = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderContractFeeCurrencyType;
                    if (contractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        contractFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderContractFeeUsd;
                    else
                        contractFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderContractFee;

                    #endregion
                    #region Quotation Fee

                    quotationFeeCurrencyType = param.dsEntireDraftContract.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType;
                    if (quotationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        quotationFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.NormalContractFeeUsd;
                    else
                        quotationFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.NormalContractFee;

                    #endregion
                    #region Deposit Fee

                    depositFeeCurrencyType = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderDepositFeeCurrencyType;
                    if (depositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        depositFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderDepositFeeUsd;
                    else
                        depositFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderDepositFee;

                    #endregion

                    paymentMethod = param.dsEntireDraftContract.doTbt_DraftRentalContrat.PayMethod;
                    billingCycle = param.dsEntireDraftContract.doTbt_DraftRentalContrat.BillingCycle;
                    creditTerm = param.dsEntireDraftContract.doTbt_DraftRentalContrat.CreditTerm;
                    fireMonitoringFlag = param.dsEntireDraftContract.doTbt_DraftRentalContrat.FireMonitorFlag;
                    crimePreventionFlag = param.dsEntireDraftContract.doTbt_DraftRentalContrat.CrimePreventFlag;
                    emergencyReportFlag = param.dsEntireDraftContract.doTbt_DraftRentalContrat.EmergencyReportFlag;
                    facilityMonitoringFlag = param.dsEntireDraftContract.doTbt_DraftRentalContrat.FacilityMonitorFlag;
                    contractDuration = param.dsEntireDraftContract.doTbt_DraftRentalContrat.ContractDurationMonth;
                    operationOffice = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OperationOfficeCode;
                    //occ = null; //Comment by Jutarat A. on 19122012
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.SALE)
                {
                    productCode = param.doSaleContractData.dtTbt_SaleBasic.ProductCode;
                    productTypeCode = param.doSaleContractData.dtTbt_SaleBasic.ProductTypeCode;
                    lineTypeCode = null;
                    lineOwnerCode = null;

                    //contractFee = param.doSaleContractData.dtTbt_SaleBasic.OrderSalePrice;
                    //quotationFee = param.doSaleContractData.dtTbt_SaleBasic.NormalSalePrice;
                    //depositFee = null;

                    #region Contract Fee

                    contractFeeCurrencyType = param.doSaleContractData.dtTbt_SaleBasic.OrderSalePriceCurrencyType;
                    if (contractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        contractFee = param.doSaleContractData.dtTbt_SaleBasic.OrderSalePriceUsd;
                    else
                        contractFee = param.doSaleContractData.dtTbt_SaleBasic.OrderSalePrice;

                    #endregion
                    #region Quotation Fee

                    quotationFeeCurrencyType = param.doSaleContractData.dtTbt_SaleBasic.NormalSalePriceCurrencyType;
                    if (quotationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        quotationFee = param.doSaleContractData.dtTbt_SaleBasic.NormalSalePriceUsd;
                    else
                        quotationFee = param.doSaleContractData.dtTbt_SaleBasic.NormalSalePrice;

                    #endregion
                    #region Deposit Fee

                    depositFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    depositFee = null;

                    #endregion

                    paymentMethod = null;
                    billingCycle = null;
                    creditTerm = null;
                    fireMonitoringFlag = null;
                    crimePreventionFlag = null;
                    emergencyReportFlag = null;
                    facilityMonitoringFlag = null;
                    contractDuration = null;
                    operationOffice = param.doSaleContractData.dtTbt_SaleBasic.OperationOfficeCode;
                    //occ = param.doSaleContractData.dtTbt_SaleBasic.OCC; //Comment by Jutarat A. on 19122012
                }

                //Add by Jutarat A. on 19122012
                if (param.ContractType == CTS160_CONTRACT_TYPE.DRAFT_RENTAL)
                {
                    occ = null;
                }
                else
                {
                    //Modify by Jutarat A. on 07022013
                    //if (param.dsRentalContractData.dtTbt_RentalContractBasic != null
                    //        && param.dsRentalContractData.dtTbt_RentalContractBasic.Count > 0)
                    //{
                    //    if (cond.ContractReportFlag == true
                    //        && param.dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    //    {
                        if (cond.ContractReportFlag == true
                                && (param.dsRentalContractData != null
                                        && param.dsRentalContractData.dtTbt_RentalContractBasic != null && param.dsRentalContractData.dtTbt_RentalContractBasic.Count > 0
                                        && param.dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                            )
                        {
                            occ = ParticularOCC.C_PARTICULAR_OCC_CONTRACT_REP_BEF_START;
                        }
                        else if (cond.CoverLetterFlag == true
                                && CommonUtil.IsNullOrEmpty(param.OCCAlphabet) == true)
                        {
                            occ = ParticularOCC.C_PARTICULAR_OCC_COVER_LETTER;
                        }
                        else
                        {
                            occ = param.OCCAlphabet;
                        }
                    //}
                    //End Modify
                }
                //End Add

                #endregion
                #region Get product name

                string productName = null;
                IProductMasterHandler pHandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                List<View_tbm_Product> pLst = pHandler.GetTbm_ProductByLanguage(productCode, productTypeCode);
                if (pLst.Count > 0)
                    productName = pLst[0].ProductName;

                #endregion

                CTS160_CoverLetterInformation data = new CTS160_CoverLetterInformation()
                {
                    IssuePerson = dsTrans.dtUserData.EmpFullName,
                    IssueOffice = mainOfficeName,
                    Subject = GetDocumentName(cond, param.ContractType, changeType),
                    CIContractTargetNameEN = param.doContractTargetCustomer != null ? param.doContractTargetCustomer.CustFullNameEN : null,
                    CIContractTargetNameLC = param.doContractTargetCustomer != null ? param.doContractTargetCustomer.CustFullNameLC : null,
                    CISiteNameEN = param.doSiteData != null ? param.doSiteData.SiteNameEN : null,
                    CISiteNameLC = param.doSiteData != null ? param.doSiteData.SiteNameLC : null,
                    CIContractCode = param.ContractCode,
                    CIOCC = occ,
                    Product = productName,
                    LineTypeCode = lineTypeCode,
                    TelephoneOwnerCode = lineOwnerCode,

                    ContractFee = contractFee,
                    ContractFeeCurrencyType = contractFeeCurrencyType,
                    QuotationFee = quotationFee,
                    QuotationFeeCurrencyType = quotationFeeCurrencyType,
                    ReceivedDepositFee = depositFee,
                    ReceivedDepositFeeCurrencyType = depositFeeCurrencyType,

                    PaymentMethodCode = paymentMethod,
                    BillingCycle = billingCycle,
                    CreditTerm = creditTerm,
                    FireMonitoringFlag = fireMonitoringFlag,
                    CrimePreventionFlag = crimePreventionFlag,
                    EmergencyReportFlag = emergencyReportFlag,
                    FacilityMonitoringFlag = facilityMonitoringFlag,
                    BusinessType = param.doContractTargetCustomer != null ? param.doContractTargetCustomer.BusinessTypeName : null,
                    Usage = param.doSiteData != null ? param.doSiteData.BuildingUsageName : null,
                    ContractDuration = contractDuration,
                    OperationOfficeCode = operationOffice
                };

                MiscTypeMappingList miscLst = new MiscTypeMappingList();
                miscLst.AddMiscType(data);

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                chandler.MiscTypeMappingList(miscLst);

                #region Get operation office name

                officeDataList = (from t in CommonUtil.dsTransData.dtOfficeData
                                  where t.OfficeCode == data.OperationOfficeCode
                                  select t).ToList<OfficeDataDo>();
                if (officeDataList != null)
                {
                    if (officeDataList.Count > 0)
                        data.OperationOffice = officeDataList[0].OfficeName;
                }

                #endregion

                #endregion

                param.CoverLetterInfo = data; //Add by Jutarat A. on 19122012

                res.ResultData = data;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get data of Instrument to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS160_GetInstrumentListData()
        {
            ObjectResultData res = new ObjectResultData();

            List<CTS160_InstrumentDetail> lst = new List<CTS160_InstrumentDetail>();
            try
            {
                CTS160_ScreenParameter param = GetScreenObject<CTS160_ScreenParameter>();
                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    List<tbt_RentalInstrumentDetails> rLst = param.dsRentalContractData.dtTbt_RentalInstrumentDetails;
                    if (rLst != null)
                    {
                        foreach (tbt_RentalInstrumentDetails r in rLst)
                        {
                            if (r.InstrumentTypeCode == InstrumentType.C_INST_TYPE_GENERAL)
                            {
                                lst.Add(new CTS160_InstrumentDetail()
                                {
                                    InstrumentCode = r.InstrumentCode,
                                    InstrumentQty = (((r.InstrumentQty ?? 0) + (r.AdditionalInstrumentQty ?? 0)) - (r.RemovalInstrumentQty ?? 0)) //InstrumentQty = r.InstrumentQty //Modify by Jutarat A. on 12102012
                                });
                            }
                        }
                    }
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.DRAFT_RENTAL)
                {
                    List<tbt_DraftRentalInstrument> rLst = param.dsEntireDraftContract.doTbt_DraftRentalInstrument;
                    if (rLst != null)
                    {
                        foreach (tbt_DraftRentalInstrument r in rLst)
                        {
                            if (r.InstrumentTypeCode == InstrumentType.C_INST_TYPE_GENERAL)
                            {
                                lst.Add(new CTS160_InstrumentDetail()
                                {
                                    InstrumentCode = r.InstrumentCode,
                                    InstrumentQty = r.InstrumentQty
                                });
                            }
                        }
                    }
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.SALE)
                {
                    List<dsSaleInstrumentDetails> sLst = param.doSaleContractData.dtTbt_SaleInstrumentDetails;
                    if (sLst != null)
                    {
                        foreach (dsSaleInstrumentDetails s in sLst)
                        {
                            lst.Add(new CTS160_InstrumentDetail()
                            {
                                InstrumentCode = s.InstrumentCode,
                                InstrumentQty = s.InstrumentQty
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<CTS160_InstrumentDetail>(lst, "Contract\\CTS160", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            return Json(res);
        }

        /// <summary>
        /// Validate business when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS160_RegisterDocumentTemplate()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Check is Suspending

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion
                #region Check screen permission

                if (CheckUserPermission(ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                #endregion

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
        /// <param name="template"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult CTS160_ConfirmDocumentTemplate(CTS160_DocumentTemplateCondition template, CTS160_CoverLetterInformation data)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Check is Suspending

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion
                #region Check screen permission

                if (CheckUserPermission(ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                #endregion

                CTS160_ScreenParameter param = GetScreenObject<CTS160_ScreenParameter>();

                #region Get Old contract fee

                string prevOldContractFeeCurrencyType = null;
                decimal? prevOldContractFee = null;
                decimal? prevOldContractFeeUsd = null;

                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    //Get OldContractFee of previous OCC
                    IRentralContractHandler rentContHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    string strPrevOCC = rentContHandler.GetPreviousImplementedOCC(
                        param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractCode,
                        param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OCC);
                    if (String.IsNullOrEmpty(strPrevOCC) == true)
                    {
                        strPrevOCC = rentContHandler.GetPreviousUnimplementedOCC(
                            param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractCode,
                            param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OCC);
                    }

                    List<tbt_RentalSecurityBasic> dtRentalSecurityBasic = rentContHandler.GetTbt_RentalSecurityBasic(
                        param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractCode, strPrevOCC);
                    if (dtRentalSecurityBasic != null && dtRentalSecurityBasic.Count > 0)
                    {
                        prevOldContractFeeCurrencyType = dtRentalSecurityBasic[0].OrderContractFeeCurrencyType;
                        prevOldContractFee = dtRentalSecurityBasic[0].OrderContractFee;
                        prevOldContractFeeUsd = dtRentalSecurityBasic[0].OrderContractFeeUsd;
                    }
                }

                #endregion
                #region Generate Document object

                tbt_ContractDocument dtTbt_ContractDocument = CreateContractDocumentData(param, template, data);
                tbt_DocContractReport dtTbt_DocContractReport = CreateDocumentContractReport(param, template, data, dtTbt_ContractDocument);
                tbt_DocChangeMemo dtTbt_DocChangeMemo = CreateDocumentChangeMemo(param, dtTbt_ContractDocument, prevOldContractFeeCurrencyType, prevOldContractFee, prevOldContractFeeUsd);
                tbt_DocChangeNotice dtTbt_DocChangeNotice = CreateDocumentChangeNotice(param, dtTbt_ContractDocument);
                tbt_DocConfirmCurrentInstrumentMemo dtTbt_DocConfirmCurrentInstrumentMemo = CreateDocumentConfirmCurrentInstrumentMemo(param, dtTbt_ContractDocument);
                tbt_DocCancelContractMemo dtTbt_DocCancelContractMemo = CreateDocumentCancelContractMemo(param, dtTbt_ContractDocument);
                List<tbt_DocCancelContractMemoDetail> dtTbt_DocCancelContractMemoDetail = CreateDocumentCancelContractMemoDetail(param, dtTbt_ContractDocument);
                tbt_DocChangeFeeMemo dtTbt_DocChangeFeeMemo = CreateDocumentChangeFeeMemo(param, dtTbt_ContractDocument, prevOldContractFeeCurrencyType, prevOldContractFee, prevOldContractFeeUsd);
                List<tbt_DocInstrumentDetails> dtTbt_DocInstrumentDetail = CreateDocumentInstrumentDetails(param, dtTbt_ContractDocument);
                tbt_DocStartMemo dtTbt_DocStartMemo = CreateDocumentStartMemo(param, dtTbt_ContractDocument); //Add by Jutarat A. on 22042013

                #endregion

                using (TransactionScope scope = new TransactionScope())
                {
                    dsContractDocData dsContractDocDataData = new dsContractDocData();
                    dsContractDocDataData.dtTbt_ContractDocument = new List<tbt_ContractDocument>() { dtTbt_ContractDocument };

                    if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN
                        || dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH)
                    {
                        dsContractDocDataData.dtTbt_DocContractReport = new List<tbt_DocContractReport>() { dtTbt_DocContractReport };
                    }
                    else if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_MEMO)
                    {
                        dsContractDocDataData.dtTbt_DocChangeMemo = new List<tbt_DocChangeMemo>() { dtTbt_DocChangeMemo };
                    }
                    else if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_NOTICE)
                    {
                        dsContractDocDataData.dtTbt_DocChangeNotice = new List<tbt_DocChangeNotice>() { dtTbt_DocChangeNotice };
                    }
                    else if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO)
                    {
                        dsContractDocDataData.dtTbt_DocConfirmCurrentInstrumentMemo = new List<tbt_DocConfirmCurrentInstrumentMemo>() { dtTbt_DocConfirmCurrentInstrumentMemo };
                    }
                    else if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO
                        || dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION)
                    {
                        dsContractDocDataData.dtTbt_DocCancelContractMemo = new List<tbt_DocCancelContractMemo>() { dtTbt_DocCancelContractMemo };
                        dsContractDocDataData.dtTbt_DocCancelContractMemoDetail = dtTbt_DocCancelContractMemoDetail;
                    }
                    else if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO)
                    {
                        dsContractDocDataData.dtTbt_DocChangeFeeMemo = new List<tbt_DocChangeFeeMemo>() { dtTbt_DocChangeFeeMemo };
                    }
                    //Add by Jutarat A. on 22042013
                    else if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_START_OPER_CONFIRM_LETTER)
                    {
                        dsContractDocDataData.dtTbt_DocStartMemo = new List<tbt_DocStartMemo>() { dtTbt_DocStartMemo };
                    }
                    //End Add

                    dsContractDocDataData.dtTbt_DocInstrumentDetail = dtTbt_DocInstrumentDetail;

                    IRentralContractHandler rHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    dsContractDocData dsContractDocResult = rHandler.CreateContractDocData(dsContractDocDataData);

                    scope.Complete();

                    MessageModel msgModelResult = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);

                    string docNoResult = "";
                    if (dsContractDocResult.dtTbt_ContractDocument != null
                        && dsContractDocResult.dtTbt_ContractDocument.Count > 0)
                    {
                        CommonUtil cmm = new CommonUtil();
                        if (param.ContractType == CTS160_CONTRACT_TYPE.DRAFT_RENTAL)
                            docNoResult = cmm.ConvertQuotationTargetCode(dsContractDocResult.dtTbt_ContractDocument[0].DocNo, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        else
                            docNoResult = cmm.ConvertContractCode(dsContractDocResult.dtTbt_ContractDocument[0].DocNo, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                    res.ResultData = new object[] { msgModelResult, docNoResult };
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Clear ContractCode of screen
        /// </summary>
        public void CTS160_ClearContractCode()
        {
            try
            {
                CTS160_ScreenParameter param = GetScreenObject<CTS160_ScreenParameter>();
                param.ContractCode = null;
                param.OCCAlphabet = null;
                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = null;
                //CommonUtil.dsTransData.dtCommonSearch.ProjectCode = null;
                param.CommonSearch = new ScreenParameter.CommonSearchDo();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Initial data of Rental document name to ComboBox
        /// </summary>
        /// <param name="OCC"></param>
        /// <returns></returns>
        public ActionResult CTS160_InitialRentalDocumentName()
        {
            ComboBoxModel cboModel = new ComboBoxModel();
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doMiscTypeCode> listDoMiscTypeCode = new List<doMiscTypeCode>();
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                doMiscTypeCode doMiscType = null;

                CTS160_ScreenParameter param = GetScreenObject<CTS160_ScreenParameter>();

                if (param.OCCAlphabet.ToUpper().CompareTo(ParticularOCC.C_PARTICULAR_OCC_MIN_BILLING_MEMO.ToUpper()) >= 0
                    && param.OCCAlphabet.ToUpper().CompareTo(ParticularOCC.C_PARTICULAR_OCC_MAX_BILLING_MEMO.ToUpper()) <= 0)
                {
                    doMiscType = new doMiscTypeCode();
                    doMiscType.ValueCode = RentalCoverLetterDocCode.C_RENTAL_COVER_LETTER_DOC_CODE_CHANGE_PAY;
                    doMiscType.FieldName = MiscType.C_RENTAL_COVER_LETTER_DOC_CODE;
                    listDoMiscTypeCode.Add(doMiscType);
                }
                else
                {
                    doMiscType = new doMiscTypeCode();
                    doMiscType.FieldName = MiscType.C_RENTAL_COVER_LETTER_DOC_CODE;
                    doMiscType.ValueCode = "%";
                    listDoMiscTypeCode.Add(doMiscType);
                }
                
                List<doMiscTypeCode> docLst = commonHandler.GetMiscTypeCodeList(listDoMiscTypeCode);
                cboModel.SetList<doMiscTypeCode>(docLst, "ValueDisplay", "ValueCode");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return Json(cboModel);
        }

        #endregion
        #region Methods

        /// <summary>
        /// Initial data of Contract
        /// </summary>
        /// <param name="res"></param>
        /// <param name="param"></param>
        /// <param name="includeDraft"></param>
        private void CTS160_InitialContractData(ObjectResultData res, CTS160_ScreenParameter param, bool includeDraft = false, bool isRetrieve = false)
        {
            try
            {
                ICommonContractHandler ccHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                bool hasContract = ccHandler.IsContractExistInRentalOrSale(param.ContractCodeLong);
                
                param.ContractType = CTS160_CONTRACT_TYPE.RENTAL;
                #region Get rental contract data

                string OCCAlphabet = CommonUtil.IsNullOrEmpty(param.OCCAlphabet) ? null : param.OCCAlphabet;
                string OCC = param.OCCAlphabet;
                if (isRetrieve == true && OCC != null)
                {
                    if (OCC.ToUpper() == ParticularOCC.C_PARTICULAR_OCC_COVER_LETTER
                        || (OCC.ToUpper().CompareTo(ParticularOCC.C_PARTICULAR_OCC_MIN_BILLING_MEMO.ToUpper()) >= 0
                                && OCC.ToUpper().CompareTo(ParticularOCC.C_PARTICULAR_OCC_MAX_BILLING_MEMO.ToUpper()) <= 0)
                        || OCC.ToUpper() == ParticularOCC.C_PARTICULAR_OCC_START_CONFIRM_LETTER) //Add by Jutarat A. on 19042013
                    {
                        OCC = null;
                    }
                }

                IRentralContractHandler rHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                param.dsRentalContractData = rHandler.GetEntireContract(param.ContractCodeLong, OCC);

                #endregion

                bool isNull = true;
                if (param.dsRentalContractData != null)
                {
                    if (param.dsRentalContractData.dtTbt_RentalContractBasic != null)
                    {
                        if (param.dsRentalContractData.dtTbt_RentalContractBasic.Count > 0)
                            isNull = false;
                    }
                }
                if (isNull == true)
                {
                    param.ContractType = CTS160_CONTRACT_TYPE.SALE;
                    #region Get sale contract data

                    ISaleContractHandler scHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                    param.doSaleContractData = scHandler.GetSaleContractData(param.ContractCodeLong, OCCAlphabet);

                    #endregion

                    if (param.doSaleContractData != null)
                    {
                        if (param.doSaleContractData.dtTbt_SaleBasic != null)
                            isNull = false;
                    }

                    if (includeDraft == true && isNull == true)
                    {
                        param.ContractType = CTS160_CONTRACT_TYPE.DRAFT_RENTAL;

                        #region Get draft rental contract data

                        doDraftRentalContractCondition cond = new doDraftRentalContractCondition();
                        cond.QuotationTargetCode = param.ContractCode;
                        //cond.Alphabet = param.OCCAlphabet;

                        IDraftRentalContractHandler drHandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
                        param.dsEntireDraftContract = drHandler.GetEntireDraftRentalContract(cond, doDraftRentalContractData.RENTAL_CONTRACT_MODE.OTHER);
                        
                        #endregion

                        if (param.dsEntireDraftContract != null)
                        {
                            if (param.dsEntireDraftContract.doTbt_DraftRentalContrat != null)
                            {
                                hasContract = true;

                                string iOCC = OCCAlphabet;
                                if (CommonUtil.IsNullOrEmpty(iOCC) == false)
                                {
                                    if (param.dsEntireDraftContract.doTbt_DraftRentalContrat.Alphabet == iOCC.ToUpper())
                                        isNull = false;
                                }
                                else
                                    isNull = false;
                            }
                        }
                    }

                    if (isNull == true)
                    {
                        if (hasContract == false)   // Contract not exist.
                        {
                            res.AddErrorMessage(
                                MessageUtil.MODULE_COMMON,
                                MessageUtil.MessageList.MSG0011,
                                new string[] { param.ContractCode },
                                new string[] { "ContractCode" });
                            return;
                        }
                        else // OCC not exist.
                        {
                            res.AddErrorMessage(
                                MessageUtil.MODULE_COMMON,
                                MessageUtil.MessageList.MSG0011,
                                new string[] { OCCAlphabet },
                                new string[] { "OCCAlphabet" });
                            return;
                        }
                    }
                }

                IBillingHandler bHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                if (isRetrieve == true
                    && CommonUtil.IsNullOrEmpty(param.OCCAlphabet) == false)
                {
                    if (param.OCCAlphabet.ToUpper().CompareTo(ParticularOCC.C_PARTICULAR_OCC_MIN_BILLING_MEMO.ToUpper()) >= 0
                        && param.OCCAlphabet.ToUpper().CompareTo(ParticularOCC.C_PARTICULAR_OCC_MAX_BILLING_MEMO.ToUpper()) <= 0)
                    {
                        string occ = param.OCCAlphabet.Trim();
                        if (occ.Length > 2)
                            occ = occ.Substring(occ.Length - 2);

                        doTbt_BillingBasic bs = bHandler.GetBillingBasicData(param.ContractCodeLong, occ, null, null, null);
                        if (bs == null)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3242);
                            return;
                        }
                    }
                }

                #region Get other data.

                IUserControlHandler ucHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                ICustomerMasterHandler cmHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                ISiteMasterHandler sHandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
               

                string condContractCode = null;
                string condOCC = null;
                string condCPCustomerCode = null;
                string condRealCustomerCode = null;
                string condSiteCode = null;
                string condContractOfficeCode = null;
                string condOperationOfficeCode = null;
                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    condContractCode = param.dsRentalContractData.dtTbt_RentalContractBasic[0].ContractCode;
                    condOCC = param.dsRentalContractData.dtTbt_RentalContractBasic[0].LastOCC;
                    
                    condCPCustomerCode = param.dsRentalContractData.dtTbt_RentalContractBasic[0].ContractTargetCustCode;
                    condRealCustomerCode = param.dsRentalContractData.dtTbt_RentalContractBasic[0].RealCustomerCustCode;
                    condSiteCode = param.dsRentalContractData.dtTbt_RentalContractBasic[0].SiteCode;
                    condContractOfficeCode = param.dsRentalContractData.dtTbt_RentalContractBasic[0].ContractOfficeCode;
                    condOperationOfficeCode = param.dsRentalContractData.dtTbt_RentalContractBasic[0].OperationOfficeCode;

                    //Get rental contract basic information.
                    param.doRentalContractBasicInformation = ucHandler.GetRentalContactBasicInformationData(condContractCode);
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.DRAFT_RENTAL)
                {
                    condContractCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.QuotationTargetCode;
                    condOCC = param.dsEntireDraftContract.doTbt_DraftRentalContrat.Alphabet;

                    condCPCustomerCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.ContractTargetCustCode;
                    condRealCustomerCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.RealCustomerCustCode;
                    condSiteCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.SiteCode;
                    condContractOfficeCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.ContractOfficeCode;
                    condOperationOfficeCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OperationOfficeCode;

                    //Get draft rental contract basic information.
                    IDraftRentalContractHandler drHandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
                    List<doDraftRentalContractInformation> drLst = drHandler.GetDraftRentalContractInformationData(condContractCode);
                    if (drLst.Count > 0)
                        param.doDraftRentalContractInformation = drLst[0];
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.SALE)
                {
                    condContractCode = param.doSaleContractData.dtTbt_SaleBasic.ContractCode;
                    condOCC = param.doSaleContractData.dtTbt_SaleBasic.OCC;

                    condCPCustomerCode = param.doSaleContractData.dtTbt_SaleBasic.PurchaserCustCode;
                    condRealCustomerCode = param.doSaleContractData.dtTbt_SaleBasic.RealCustomerCustCode;
                    condSiteCode = param.doSaleContractData.dtTbt_SaleBasic.SiteCode;
                    condContractOfficeCode = param.doSaleContractData.dtTbt_SaleBasic.ContractOfficeCode;
                    condOperationOfficeCode = param.doSaleContractData.dtTbt_SaleBasic.OperationOfficeCode;

                    //Get sale contract basic information.
                    List<doSaleContractBasicInformation> scLst = ucHandler.GetSaleContractBasicInformationData(condContractCode, condOCC);
                    if (scLst.Count > 0)
                        param.doSaleContractBasicInformation = scLst[0];
                }

                //Get contract target customer data.
                List<doCustomer> cLst = cmHandler.GetCustomer(condCPCustomerCode);
                if (cLst.Count > 0)
                    param.doContractTargetCustomer = cLst[0];

                //Get real customer data.
                List<doCustomer> rLst = cmHandler.GetCustomer(condRealCustomerCode);
                if (rLst.Count > 0)
                    param.doRealCustomer = rLst[0];

                //Get site data.
                List<doSite> sLst = sHandler.GetSite(condSiteCode, null);
                if (sLst.Count > 0)
                    param.doSiteData = sLst[0];

                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    //Get billing temp data.
                    param.dtBillingTemp = ccHandler.GetTbt_BillingTempListForView(
                        condContractCode,
                        condOCC);

                    //Get billing basic data.
                    param.dtTbt_BillingBasic = bHandler.GetTbt_BillingBasic(
                        condContractCode,
                        null); //condOCC); //Modify by Jutarat A. on 19122012
                }

                #endregion
                #region Mapping data.

                CommonUtil cmm = new CommonUtil();
                param.ContractCode = cmm.ConvertContractCode(condContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                if (isRetrieve == false)
                    param.OCCAlphabet = condOCC;

                if (param.ContractType != CTS160_CONTRACT_TYPE.DRAFT_RENTAL)
                {
                    //CommonUtil.dsTransData.dtCommonSearch.ContractCode = param.ContractCode;
                    param.CommonSearch = new ScreenParameter.CommonSearchDo()
                    {
                        ContractCode = param.ContractCode
                    };
                }

                #endregion
                #region Check data authority

                bool found = false;
                List<OfficeDataDo> officeLst = CommonUtil.dsTransData.dtOfficeData;
                if (officeLst != null)
                {
                    foreach (OfficeDataDo office in officeLst)
                    {
                        if (office.OfficeCode == condContractOfficeCode
                            || office.OfficeCode == condOperationOfficeCode)
                        {
                            found = true;
                            break;
                        }
                    }
                }
                if (found == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    return;
                }

                #endregion

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set data to DO of DocumentTemplate
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private CTS160_DocumentTemplateData SetDocumentTemplateData(CTS160_ScreenParameter param)
        {
            CTS160_DocumentTemplateData template = new CTS160_DocumentTemplateData();

            try
            {
                template.ContractType = param.ContractType;
                template.ContractCode = param.ContractCode;
                template.OCCAlphabet = param.OCCAlphabet;

                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    template.ContractTargetNameEN = param.doRentalContractBasicInformation.ContractTargetNameEN;
                    template.RealCustomerNameEN = param.doRentalContractBasicInformation.RealCustomerNameEN;
                    template.SiteNameEN = param.doRentalContractBasicInformation.SiteNameEN;
                    template.ContractTargetNameLC = param.doRentalContractBasicInformation.ContractTargetNameLC;
                    template.RealCustomerNameLC = param.doRentalContractBasicInformation.RealCustomerNameLC;
                    template.SiteNameLC = param.doRentalContractBasicInformation.SiteNameLC;

                    ICommonHandler cmHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    template.ChangeTypeName = cmHandler.GetMiscDisplayValue(MiscType.C_RENTAL_CHANGE_TYPE, param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeType);
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.DRAFT_RENTAL)
                {
                    template.ContractTargetNameEN = param.doDraftRentalContractInformation.ContractTargetNameEN;
                    template.RealCustomerNameEN = param.doDraftRentalContractInformation.RealCustomerNameEN;
                    template.SiteNameEN = param.doDraftRentalContractInformation.SiteNameEN;
                    template.ContractTargetNameLC = param.doDraftRentalContractInformation.ContractTargetNameLC;
                    template.RealCustomerNameLC = param.doDraftRentalContractInformation.RealCustomerNameLC;
                    template.SiteNameLC = param.doDraftRentalContractInformation.SiteNameLC;
                    template.ChangeTypeName = null;
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.SALE)
                {
                    template.ContractTargetNameEN = param.doSaleContractBasicInformation.PurchaserNameEN;
                    template.RealCustomerNameEN = param.doSaleContractBasicInformation.RealCustomerNameEN;
                    template.SiteNameEN = param.doSaleContractBasicInformation.SiteNameEN;
                    template.ContractTargetNameLC = param.doSaleContractBasicInformation.PurchaserNameLC;
                    template.RealCustomerNameLC = param.doSaleContractBasicInformation.RealCustomerNameLC;
                    template.SiteNameLC = param.doSaleContractBasicInformation.SiteNameLC;

                    template.ChangeTypeName = param.doSaleContractBasicInformation.LastChangeTypeCodeName;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return template;
        }

        /// <summary>
        /// Set data to DO of DocumentState
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private CTS160_DocumentState SetSelectDocumentState(CTS160_ScreenParameter param)
        {
            try
            {
                CTS160_DocumentState state = new CTS160_DocumentState();

                string changeType = null;
                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    if (param.dsRentalContractData.dtTbt_RentalSecurityBasic != null)
                    {
                        if (param.dsRentalContractData.dtTbt_RentalSecurityBasic.Count > 0)
                            changeType = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeType;
                    }

                    if (CommonUtil.IsNullOrEmpty(param.OCCAlphabet)
                        || param.OCCAlphabet == ParticularOCC.C_PARTICULAR_OCC_COVER_LETTER
                        || (param.OCCAlphabet.ToUpper().CompareTo(ParticularOCC.C_PARTICULAR_OCC_MIN_BILLING_MEMO.ToUpper()) >= 0
                                && param.OCCAlphabet.ToUpper().CompareTo(ParticularOCC.C_PARTICULAR_OCC_MAX_BILLING_MEMO.ToUpper()) <= 0))
                    {
                        state.CoverLetterFlag = true;
                    }
                    //Add by Jutarat A. on 19042013
                    else if (param.OCCAlphabet == ParticularOCC.C_PARTICULAR_OCC_START_CONFIRM_LETTER)
                    {
                        state.MemorandumFlag = true;
                    }
                    //End Add
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_APPROVE
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_ALTERNATIVE_START
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_TERMINATED)
                    {
                        state.ContractReportFlag = true;
                        state.CoverLetterFlag = true;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_NEW_START
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_EXCHANGE_INSTRU_AT_MA
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MOVE_INSTRU
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_QTY_DURING_STOP
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_INSTRUMENT_QTY
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_WIRING) //Add by Jutarat A. on 21052013
                    {
                        state.ContractReportFlag = true;
                        state.MemorandumFlag = true;
                        state.NoticeFlag = true;
                        state.CoverLetterFlag = true;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_RESUME
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_STOP
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP)
                    {
                        state.ContractReportFlag = true;
                        state.CoverLetterFlag = true;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE_DURING_STOP) //Add by Jutarat A. on 30102012
                    {
                        state.ContractReportFlag = true;
                        state.MemorandumFlag = true;
                        state.CoverLetterFlag = true;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                    {
                        state.MemorandumFlag = true;
                        state.CoverLetterFlag = true;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_ALL)
                    {
                        //Disable all.
                    }

                    if (param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OCC != param.dsRentalContractData.dtTbt_RentalContractBasic[0].LastOCC)
                    {
                        state.ContractReportFlag = false;
                    }

                    if (param.dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_BEF_START
                        && changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE)
                    {
                        state.CoverLetterFlag = false;
                    }
                    else if (param.dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
                        && param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OCC != param.dsRentalContractData.dtTbt_RentalContractBasic[0].LastOCC)
                    {
                        state.CoverLetterFlag = false;
                    }
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.DRAFT_RENTAL)
                {
                    if (param.dsEntireDraftContract.doTbt_DraftRentalContrat.DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_APPROVED
                        || param.dsEntireDraftContract.doTbt_DraftRentalContrat.DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_REJECTED)
                    {
                        //Disable all.
                    }
                    else if (param.dsEntireDraftContract.doTbt_DraftRentalContrat.DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_WAITFORAPPROVE
                        || param.dsEntireDraftContract.doTbt_DraftRentalContrat.DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_RETURNED)
                    {
                        state.ContractReportFlag = true;
                        state.CoverLetterFlag = true;
                    }
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.SALE)
                {
                    state.CoverLetterFlag = true;
                }

                return state;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Get name of Document
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="contractType"></param>
        /// <param name="changeType"></param>
        /// <returns></returns>
        private string GetDocumentName(CTS160_DocumentTemplateCondition cond, CTS160_CONTRACT_TYPE contractType, string changeType)
        {
            string docName = null;

            if (cond.CoverLetterFlag == true)
            {
                string miscTypeCode = MiscType.C_RENTAL_COVER_LETTER_DOC_CODE;
                if (contractType == CTS160_CONTRACT_TYPE.SALE)
                {
                    miscTypeCode = MiscType.C_SALE_COVER_LETTER_DOC_CODE;
                }
                
                ICommonHandler cHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                docName = cHandler.GetMiscDisplayValue(miscTypeCode, cond.DocumentName);
            }
            else
            {
                string docCode = null;
                if (cond.ContractReportFlag == true)
                {
                    if (cond.ContractLanguage == DocLanguage.C_DOC_LANGUAGE_EN)
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN;
                    else
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH;
                }
                else if (cond.MemorandumFlag == true)
                {
                    //Add by Jutarat A. on 23042013
                    CTS160_ScreenParameter param = GetScreenObject<CTS160_ScreenParameter>();
                    if (param.OCCAlphabet == ParticularOCC.C_PARTICULAR_OCC_START_CONFIRM_LETTER)
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_START_OPER_CONFIRM_LETTER;
                    }
                    //End Add
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_NEW_START
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_EXCHANGE_INSTRU_AT_MA
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MOVE_INSTRU
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_WIRING) //Add by Jutarat A. on 21052013
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_MEMO;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_QTY_DURING_STOP
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_INSTRUMENT_QTY)
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE_DURING_STOP) //Add by Jutarat A. on 30102012
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO;
                    }
                }
                else if (cond.NoticeFlag == true)
                {
                    docCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_NOTICE;
                }

                if (docCode != null)
                {
                    IDocumentHandler docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                    List<tbm_DocumentTemplate> docTemplateList = docHandler.GetTbm_DocumentTemplate(DocumentType.C_DOCUMENT_TYPE_CONTRACT);
                    docTemplateList = (from t in docTemplateList
                                       where t.DocumentCode == docCode
                                       select t).ToList<tbm_DocumentTemplate>();
                    if (docTemplateList != null)
                    {
                        if (docTemplateList.Count > 0)
                        {
                            CommonUtil.MappingObjectLanguage(docTemplateList[0]);
                            docName = docTemplateList[0].DocumentName;
                        }
                    }
                }
            }

            return docName;
        }

        /// <summary>
        /// Get name of Document (EN/LC)
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="contractType"></param>
        /// <param name="changeType"></param>
        /// <returns></returns>
        private string[] GetDocumentNameENLC(CTS160_DocumentTemplateCondition cond, CTS160_CONTRACT_TYPE contractType, string changeType)
        {
            string[] docName = new string[]{null, null};

            if (cond.CoverLetterFlag == true)
            {
                string miscTypeCode = MiscType.C_RENTAL_COVER_LETTER_DOC_CODE;
                if (contractType == CTS160_CONTRACT_TYPE.SALE)
                {
                    miscTypeCode = MiscType.C_SALE_COVER_LETTER_DOC_CODE;
                }

                ICommonHandler cHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                List<string> listFieldName = new List<string>();
                listFieldName.Add(miscTypeCode);
                List<doMiscTypeCode> listMisc = cHandler.GetMiscTypeCodeListByFieldName(listFieldName); // This result has language mapping already
                foreach (doMiscTypeCode m in listMisc)
                {
                    if (m.FieldName == miscTypeCode
                        && m.ValueCode == cond.DocumentName)
                    {
                        docName[0] = m.ValueDisplayEN;
                        docName[1] = m.ValueDisplayLC;
                    }
                }
            }
            else
            {
                string docCode = null;
                if (cond.ContractReportFlag == true)
                {
                    if (cond.ContractLanguage == DocLanguage.C_DOC_LANGUAGE_EN)
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN;
                    else
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH;
                }
                else if (cond.MemorandumFlag == true)
                {
                    //Add by Jutarat A. on 23042013
                    CTS160_ScreenParameter param = GetScreenObject<CTS160_ScreenParameter>();
                    if (param.OCCAlphabet == ParticularOCC.C_PARTICULAR_OCC_START_CONFIRM_LETTER)
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_START_OPER_CONFIRM_LETTER;
                    }
                    //End Add
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_NEW_START
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_EXCHANGE_INSTRU_AT_MA
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MOVE_INSTRU
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_WIRING) //Add by Jutarat A. on 21052013
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_MEMO;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_QTY_DURING_STOP
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_INSTRUMENT_QTY)
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE_DURING_STOP) //Add by Jutarat A. on 30102012
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO;
                    }
                }
                else if (cond.NoticeFlag == true)
                {
                    docCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_NOTICE;
                }

                if (docCode != null)
                {
                    IDocumentHandler docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                    List<tbm_DocumentTemplate> docTemplateList = docHandler.GetTbm_DocumentTemplate(DocumentType.C_DOCUMENT_TYPE_CONTRACT);
                    docTemplateList = (from t in docTemplateList
                                       where t.DocumentCode == docCode
                                       select t).ToList<tbm_DocumentTemplate>();
                    if (docTemplateList != null)
                    {
                        if (docTemplateList.Count > 0)
                        {
                            docName[0] = docTemplateList[0].DocumentNameEN;
                            docName[1] = docTemplateList[0].DocumentNameLC;
                        }
                    }
                }
            }

            return docName;
        }

        /// <summary>
        /// Create DO of tbt_ContractDocument
        /// </summary>
        /// <param name="param"></param>
        /// <param name="template"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private tbt_ContractDocument CreateContractDocumentData(CTS160_ScreenParameter param, CTS160_DocumentTemplateCondition template, 
            CTS160_CoverLetterInformation data)
        {
            try
            {
                dsTransDataModel dsTrans = CommonUtil.dsTransData;

                tbt_ContractDocument dtTbt_ContractDocument = new tbt_ContractDocument();
                dtTbt_ContractDocument.SECOMSignatureFlag = template.SignatureFlag;
                dtTbt_ContractDocument.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_NOT_ISSUED;
                dtTbt_ContractDocument.DocAuditResult = DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED;

                string changeType = null;
                string productCode = null;
                string productTypeCode = null;
                if (param.ContractType == CTS160_CONTRACT_TYPE.DRAFT_RENTAL)
                {
                    dtTbt_ContractDocument.QuotationTargetCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.QuotationTargetCode;
                    dtTbt_ContractDocument.Alphabet = param.dsEntireDraftContract.doTbt_DraftRentalContrat.Alphabet;

                    productCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.ProductCode;
                    productTypeCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.ProductTypeCode;

                    #region Contract target customer

                    dtTbt_ContractDocument.ContractTargetCustCode = param.doDraftRentalContractInformation.ContractTargetCustCode;
                    dtTbt_ContractDocument.ContractTargetNameEN = param.doDraftRentalContractInformation.ContractTargetNameEN;
                    dtTbt_ContractDocument.ContractTargetNameLC = param.doDraftRentalContractInformation.ContractTargetNameLC;
                    dtTbt_ContractDocument.ContractTargetAddressEN = param.doDraftRentalContractInformation.ContractTargetAddressEN;
                    dtTbt_ContractDocument.ContractTargetAddressLC = param.doDraftRentalContractInformation.ContractTargetAddressLC;

                    #endregion
                    #region Real customer

                    dtTbt_ContractDocument.RealCustomerCustCode = param.doDraftRentalContractInformation.RealCustomerCustCode;
                    dtTbt_ContractDocument.RealCustomerNameEN = param.doDraftRentalContractInformation.RealCustomerNameEN;
                    dtTbt_ContractDocument.RealCustomerNameLC = param.doDraftRentalContractInformation.RealCustomerNameLC;

                    #endregion
                    #region site

                    dtTbt_ContractDocument.SiteNameEN = param.doDraftRentalContractInformation.SiteNameEN;
                    dtTbt_ContractDocument.SiteNameLC = param.doDraftRentalContractInformation.SiteNameLC;
                    dtTbt_ContractDocument.SiteAddressEN = param.doDraftRentalContractInformation.SiteAddressEN;
                    dtTbt_ContractDocument.SiteAddressLC = param.doDraftRentalContractInformation.SiteAddressLC;

                    #endregion
                    #region Office

                    dtTbt_ContractDocument.ContractOfficeCode = param.doDraftRentalContractInformation.ContractOfficeCode;
                    dtTbt_ContractDocument.OperationOfficeCode = param.doDraftRentalContractInformation.OperationOfficeCode;

                    #endregion
                    #region Negotiation staff emp no

                    dtTbt_ContractDocument.NegotiationStaffEmpNo = param.dsEntireDraftContract.doTbt_DraftRentalContrat.SalesmanEmpNo1;

                    #endregion
                    #region Other

                    #region Quotation Fee

                    dtTbt_ContractDocument.QuotationFeeCurrencyType = param.dsEntireDraftContract.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType;
                    dtTbt_ContractDocument.QuotationFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.NormalContractFee;
                    dtTbt_ContractDocument.QuotationFeeUsd = param.dsEntireDraftContract.doTbt_DraftRentalContrat.NormalContractFeeUsd;

                    #endregion
                    #region Contract Fee

                    dtTbt_ContractDocument.ContractFeeCurrencyType = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderContractFeeCurrencyType;
                    dtTbt_ContractDocument.ContractFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderContractFee;
                    dtTbt_ContractDocument.ContractFeeUsd = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderContractFeeUsd;

                    #endregion
                    #region Deposit Fee

                    dtTbt_ContractDocument.DepositFeeCurrencyType = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderDepositFeeCurrencyType;
                    dtTbt_ContractDocument.DepositFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderDepositFee;
                    dtTbt_ContractDocument.DepositFeeUsd = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderDepositFeeUsd;

                    #endregion

                    dtTbt_ContractDocument.ContractFeePayMethod = param.dsEntireDraftContract.doTbt_DraftRentalContrat.PayMethod;
                    dtTbt_ContractDocument.CreditTerm = param.dsEntireDraftContract.doTbt_DraftRentalContrat.CreditTerm;
                    dtTbt_ContractDocument.PaymentCycle = param.dsEntireDraftContract.doTbt_DraftRentalContrat.BillingCycle;

                    dtTbt_ContractDocument.PhoneLineTypeCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.PhoneLineTypeCode1;
                    dtTbt_ContractDocument.PhoneLineOwnerTypeCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.PhoneLineOwnerTypeCode1;
                    dtTbt_ContractDocument.FireSecurityFlag = param.dsEntireDraftContract.doTbt_DraftRentalContrat.FireMonitorFlag;
                    dtTbt_ContractDocument.CrimePreventFlag = param.dsEntireDraftContract.doTbt_DraftRentalContrat.CrimePreventFlag;
                    dtTbt_ContractDocument.EmergencyReportFlag = param.dsEntireDraftContract.doTbt_DraftRentalContrat.EmergencyReportFlag;
                    dtTbt_ContractDocument.FacilityMonitorFlag = param.dsEntireDraftContract.doTbt_DraftRentalContrat.FacilityMonitorFlag;

                    dtTbt_ContractDocument.ContractDurationMonth = param.dsEntireDraftContract.doTbt_DraftRentalContrat.ContractDurationMonth;
                    dtTbt_ContractDocument.OldContractCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OldContractCode;

                    #endregion
                }
                else
                {
                    dtTbt_ContractDocument.ContractCode = param.ContractCodeLong;
                    dtTbt_ContractDocument.Alphabet = null;

                    string contractStatus = null;
                    if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                    {
                        changeType = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeType;
                        productCode = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ProductCode;
                        productTypeCode = param.dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode;
                        contractStatus = param.dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus;

                        #region Contract target customer

                        dtTbt_ContractDocument.ContractTargetCustCode = param.doRentalContractBasicInformation.ContractTargetCustCode;
                        dtTbt_ContractDocument.ContractTargetNameEN = param.doRentalContractBasicInformation.ContractTargetNameEN;
                        dtTbt_ContractDocument.ContractTargetNameLC = param.doRentalContractBasicInformation.ContractTargetNameLC;
                        dtTbt_ContractDocument.ContractTargetAddressEN = param.doRentalContractBasicInformation.ContractTargetAddressEN;
                        dtTbt_ContractDocument.ContractTargetAddressLC = param.doRentalContractBasicInformation.ContractTargetAddressLC;

                        #endregion
                        #region Real customer

                        dtTbt_ContractDocument.RealCustomerCustCode = param.doRentalContractBasicInformation.RealCustomerCustCode;
                        dtTbt_ContractDocument.RealCustomerNameEN = param.doRentalContractBasicInformation.RealCustomerNameEN;
                        dtTbt_ContractDocument.RealCustomerNameLC = param.doRentalContractBasicInformation.RealCustomerNameLC;

                        #endregion
                        #region site

                        dtTbt_ContractDocument.SiteNameEN = param.doRentalContractBasicInformation.SiteNameEN;
                        dtTbt_ContractDocument.SiteNameLC = param.doRentalContractBasicInformation.SiteNameLC;
                        dtTbt_ContractDocument.SiteAddressEN = param.doRentalContractBasicInformation.SiteAddressEN;
                        dtTbt_ContractDocument.SiteAddressLC = param.doRentalContractBasicInformation.SiteAddressLC;

                        #endregion
                        #region Office

                        dtTbt_ContractDocument.ContractOfficeCode = param.doRentalContractBasicInformation.ContractOfficeCode;
                        dtTbt_ContractDocument.OperationOfficeCode = param.doRentalContractBasicInformation.OperationOfficeCode;

                        #endregion
                        #region Negotiation staff emp no

                        if (CommonUtil.IsNullOrEmpty(param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1) == false)
                            dtTbt_ContractDocument.NegotiationStaffEmpNo = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1;
                        else
                            dtTbt_ContractDocument.NegotiationStaffEmpNo = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1;

                        #endregion
                        #region Other

                        #region Quotation Fee

                        dtTbt_ContractDocument.QuotationFeeCurrencyType = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalContractFeeCurrencyType;
                        dtTbt_ContractDocument.QuotationFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalContractFee;
                        dtTbt_ContractDocument.QuotationFeeUsd = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalContractFeeUsd;

                        #endregion
                        #region Contract Fee

                        dtTbt_ContractDocument.ContractFeeCurrencyType = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType;
                        dtTbt_ContractDocument.ContractFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFee;
                        dtTbt_ContractDocument.ContractFeeUsd = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd;

                        #endregion

                        decimal? depositFee = null;
                        string depositFeeCurrencyType = null;
                        string paymethod = null;
                        int? creditTerm = null;
                        int? billingCycle = null;

                        // 20170224 nakajima modify start
                        if (param.dtBillingTemp != null)
                        {
                            foreach (dtTbt_BillingTempListForView b in param.dtBillingTemp)
                            {
                                if (b.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                                {
                                    if (depositFee == null)
                                        depositFee = 0;
                                    depositFeeCurrencyType = b.BillingAmtCurrencyType;

                                    if (b.BillingAmtCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && b.BillingAmt != null)
                                    {
                                        depositFee += b.BillingAmt;
                                    }
                                    else if (b.BillingAmtCurrencyType == CurrencyUtil.C_CURRENCY_US && b.BillingAmtUsd != null)
                                    {
                                        depositFee += b.BillingAmtUsd;
                                    }
                                }
                                else if (b.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                                {
                                    paymethod = b.PayMethod;
                                }
                            }
                        }

                        //Add by Jutarat A. on 19122012
                        if (param.CoverLetterInfo != null)
                        {
                            creditTerm = param.CoverLetterInfo.CreditTerm;
                            billingCycle = param.CoverLetterInfo.BillingCycle;
                        }
                        //End Add
                        else
                        {
                            if (param.dtTbt_BillingBasic != null)
                            {
                                foreach (tbt_BillingBasic b in param.dtTbt_BillingBasic)
                                {
                                    if (b.MonthlyBillingAmount > 0
                                        && b.StopBillingFlag == false)
                                    {
                                        creditTerm = b.CreditTerm;
                                        billingCycle = b.BillingCycle;
                                    }
                                }
                            }
                        }

                        dtTbt_ContractDocument.DepositFeeCurrencyType = depositFeeCurrencyType;
                        if (depositFeeCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                        {
                            dtTbt_ContractDocument.DepositFee = depositFee;
                        }
                        else if (depositFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                        {
                            dtTbt_ContractDocument.DepositFeeUsd = depositFee;
                        }
                        // 20170224 nakajima modify end

                        dtTbt_ContractDocument.ContractFeePayMethod = paymethod;
                        dtTbt_ContractDocument.CreditTerm = creditTerm;
                        dtTbt_ContractDocument.PaymentCycle = billingCycle;

                        dtTbt_ContractDocument.PhoneLineTypeCode = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1;
                        dtTbt_ContractDocument.PhoneLineOwnerTypeCode = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1;
                        dtTbt_ContractDocument.FireSecurityFlag = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].FireMonitorFlag;
                        dtTbt_ContractDocument.CrimePreventFlag = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].CrimePreventFlag;
                        dtTbt_ContractDocument.EmergencyReportFlag = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag;
                        dtTbt_ContractDocument.FacilityMonitorFlag = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag;

                        dtTbt_ContractDocument.ContractDurationMonth = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractDurationMonth;
                        dtTbt_ContractDocument.OldContractCode = param.dsRentalContractData.dtTbt_RentalContractBasic[0].OldContractCode;

                        #endregion
                    }
                    else if (param.ContractType == CTS160_CONTRACT_TYPE.SALE)
                    {
                        productCode = param.doSaleContractData.dtTbt_SaleBasic.ProductCode;
                        productTypeCode = param.doSaleContractData.dtTbt_SaleBasic.ProductTypeCode;
                        contractStatus = param.doSaleContractData.dtTbt_SaleBasic.ContractStatus;
                        
                        #region purchaser customer

                        dtTbt_ContractDocument.ContractTargetCustCode = param.doSaleContractBasicInformation.PurchaserCustCode;
                        dtTbt_ContractDocument.ContractTargetNameEN = param.doSaleContractBasicInformation.PurchaserNameEN;
                        dtTbt_ContractDocument.ContractTargetNameLC = param.doSaleContractBasicInformation.PurchaserNameLC;
                        dtTbt_ContractDocument.ContractTargetAddressEN = param.doSaleContractBasicInformation.PurchaserAddressEN;
                        dtTbt_ContractDocument.ContractTargetAddressLC = param.doSaleContractBasicInformation.PurchaserAddressLC;

                        #endregion
                        #region Real customer

                        dtTbt_ContractDocument.RealCustomerCustCode = param.doSaleContractBasicInformation.RealCustomerCustCode;
                        dtTbt_ContractDocument.RealCustomerNameEN = param.doSaleContractBasicInformation.RealCustomerNameEN;
                        dtTbt_ContractDocument.RealCustomerNameLC = param.doSaleContractBasicInformation.RealCustomerNameLC;

                        #endregion
                        #region site

                        dtTbt_ContractDocument.SiteNameEN = param.doSaleContractBasicInformation.SiteNameEN;
                        dtTbt_ContractDocument.SiteNameLC = param.doSaleContractBasicInformation.SiteNameLC;
                        dtTbt_ContractDocument.SiteAddressEN = param.doSaleContractBasicInformation.SiteAddressEN;
                        dtTbt_ContractDocument.SiteAddressLC = param.doSaleContractBasicInformation.SiteAddressLC;

                        #endregion
                        #region Office

                        dtTbt_ContractDocument.ContractOfficeCode = param.doSaleContractBasicInformation.ContractOfficeCode;
                        dtTbt_ContractDocument.OperationOfficeCode = param.doSaleContractBasicInformation.OperationOfficeCode;

                        #endregion
                        #region Negotiation staff emp no

                        if (CommonUtil.IsNullOrEmpty(param.doSaleContractData.dtTbt_SaleBasic.NegotiationStaffEmpNo1) == false)
                            dtTbt_ContractDocument.NegotiationStaffEmpNo = param.doSaleContractData.dtTbt_SaleBasic.NegotiationStaffEmpNo1;
                        else
                            dtTbt_ContractDocument.NegotiationStaffEmpNo = param.doSaleContractData.dtTbt_SaleBasic.SalesmanEmpNo1;

                        #endregion
                        #region Other

                        #region Quotation Fee

                        dtTbt_ContractDocument.QuotationFeeCurrencyType = param.doSaleContractData.dtTbt_SaleBasic.NormalSalePriceCurrencyType;
                        dtTbt_ContractDocument.QuotationFee = param.doSaleContractData.dtTbt_SaleBasic.NormalSalePrice;
                        dtTbt_ContractDocument.QuotationFeeUsd = param.doSaleContractData.dtTbt_SaleBasic.NormalSalePriceUsd;

                        #endregion
                        #region Contract Fee

                        dtTbt_ContractDocument.ContractFeeCurrencyType = param.doSaleContractData.dtTbt_SaleBasic.OrderSalePriceCurrencyType;
                        dtTbt_ContractDocument.ContractFee = param.doSaleContractData.dtTbt_SaleBasic.OrderSalePrice;
                        dtTbt_ContractDocument.ContractFeeUsd = param.doSaleContractData.dtTbt_SaleBasic.OrderSalePriceUsd;

                        #endregion

                        #endregion
                    }

                    #region OCC

                    if (template.ContractReportFlag == true
                        && contractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    {
                        dtTbt_ContractDocument.OCC = ParticularOCC.C_PARTICULAR_OCC_CONTRACT_REP_BEF_START;
                    }
                    else if (template.CoverLetterFlag == true
                            && CommonUtil.IsNullOrEmpty(param.OCCAlphabet) == true)
                    {
                        dtTbt_ContractDocument.OCC = ParticularOCC.C_PARTICULAR_OCC_COVER_LETTER;
                    }
                    else
                    {
                        dtTbt_ContractDocument.OCC = param.OCCAlphabet;
                    }

                    #endregion
                }

                #region Document code

                string docCode = null;
                if (template.ContractReportFlag == true)
                {
                    if (template.ContractLanguage == DocLanguage.C_DOC_LANGUAGE_EN)
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN;
                    else
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH;
                }
                else if (template.MemorandumFlag == true)
                {
                    //Add by Jutarat A. on 19042013
                    if (param.OCCAlphabet == ParticularOCC.C_PARTICULAR_OCC_START_CONFIRM_LETTER)
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_START_OPER_CONFIRM_LETTER;
                    }
                    //End Add
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_NEW_START
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_EXCHANGE_INSTRU_AT_MA
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MOVE_INSTRU
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_WIRING) //Add by Jutarat A. on 21052013
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_MEMO;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_QTY_DURING_STOP
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_INSTRUMENT_QTY)
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL
                        || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO;
                    }
                    else if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE
                            || changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE_DURING_STOP) //Add by Jutarat A. on 30102012
                    {
                        docCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO;
                    }
                }
                else if (template.NoticeFlag == true)
                {
                    docCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_NOTICE;
                }
                else if (template.CoverLetterFlag == true)
                {
                    docCode = DocumentCode.C_DOCUMENT_CODE_COVER_LETTER;
                }
                dtTbt_ContractDocument.DocumentCode = docCode;

                #endregion
                #region Cover letter code

                if (template.CoverLetterFlag == true)
                {
                    dtTbt_ContractDocument.CoverLetterDocCode = template.DocumentName;
                }

                #endregion
                #region Generate contract document occ

                string contractCode = null;
                string occ = null;
                if (CommonUtil.IsNullOrEmpty(dtTbt_ContractDocument.ContractCode) == false)
                {
                    contractCode = dtTbt_ContractDocument.ContractCode;
                    occ = dtTbt_ContractDocument.OCC;
                }
                else
                {
                    contractCode = dtTbt_ContractDocument.QuotationTargetCode;
                    occ = dtTbt_ContractDocument.Alphabet;
                }

                IContractDocumentHandler cdHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
                dtTbt_ContractDocument.ContractDocOCC = cdHandler.GenerateDocOCC(contractCode, occ);

                #endregion
                #region Document No.

                if (CommonUtil.IsNullOrEmpty(dtTbt_ContractDocument.ContractCode) == false)
                {
                    dtTbt_ContractDocument.DocNo = string.Format("{0}-{1}-{2}",
                        dtTbt_ContractDocument.ContractCode,
                        dtTbt_ContractDocument.OCC,
                        dtTbt_ContractDocument.ContractDocOCC);
                }
                else
                {
                    dtTbt_ContractDocument.DocNo = string.Format("{0}-{1}-{2}",
                        dtTbt_ContractDocument.QuotationTargetCode,
                        dtTbt_ContractDocument.Alphabet,
                        dtTbt_ContractDocument.ContractDocOCC);
                }

                #endregion
                #region Create office code

                dtTbt_ContractDocument.CreateOfficeCode = dsTrans.dtUserData.MainOfficeCode;
                List<OfficeDataDo> officeDataList = null;
                officeDataList = (from t in CommonUtil.dsTransData.dtOfficeData
                                  where t.OfficeCode == dsTrans.dtUserData.MainOfficeCode
                                  select t).ToList<OfficeDataDo>();
                if (officeDataList != null)
                {
                    if (officeDataList.Count > 0)
                    {
                        dtTbt_ContractDocument.CreateOfficeNameEN = officeDataList[0].OfficeNameEN;
                        dtTbt_ContractDocument.CreateOfficeNameLC = officeDataList[0].OfficeNameLC;
                    }
                }

                #endregion
                #region Product name

                IProductMasterHandler pHandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                List<View_tbm_Product> pLst = pHandler.GetTbm_ProductByLanguage(productCode, productTypeCode);
                if (pLst.Count > 0)
                {
                    dtTbt_ContractDocument.ProductCode = productCode;
                    dtTbt_ContractDocument.ProductNameEN = pLst[0].ProductNameEN;
                    dtTbt_ContractDocument.ProductNameLC = pLst[0].ProductNameLC;
                }
                #endregion
                #region Phone line type name

                List<string> listFieldName = new List<string>();

                if (CommonUtil.IsNullOrEmpty(dtTbt_ContractDocument.PhoneLineTypeCode) == false)
                    listFieldName.Add(MiscType.C_PHONE_LINE_TYPE);
                if (CommonUtil.IsNullOrEmpty(dtTbt_ContractDocument.PhoneLineOwnerTypeCode) == false)
                    listFieldName.Add(MiscType.C_PHONE_LINE_OWNER_TYPE);

                if (listFieldName.Count > 0)
                {
                    ICommonHandler cHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> listMisc = cHandler.GetMiscTypeCodeListByFieldName(listFieldName);
                    foreach (doMiscTypeCode m in listMisc)
                    {
                        if (m.FieldName == MiscType.C_PHONE_LINE_TYPE
                            && m.ValueCode == dtTbt_ContractDocument.PhoneLineTypeCode)
                        {
                            dtTbt_ContractDocument.PhoneLineTypeNameEN = m.ValueDisplayEN;
                            dtTbt_ContractDocument.PhoneLineTypeNameLC = m.ValueDisplayLC;
                        }
                        else if (m.FieldName == MiscType.C_PHONE_LINE_OWNER_TYPE
                            && m.ValueCode == dtTbt_ContractDocument.PhoneLineOwnerTypeCode)
                        {
                            dtTbt_ContractDocument.PhoneLineOwnerTypeNameEN = m.ValueDisplayEN;
                            dtTbt_ContractDocument.PhoneLineOwnerTypeNameLC = m.ValueDisplayLC;
                        }
                    }
                }

                #endregion
                #region Business type

                if (param.doContractTargetCustomer != null)
                {
                    dtTbt_ContractDocument.BusinessTypeCode = param.doContractTargetCustomer.BusinessTypeCode;
                    dtTbt_ContractDocument.BusinessTypeNameEN = param.doContractTargetCustomer.BusinessTypeNameEN;
                    dtTbt_ContractDocument.BusinessTypeNameLC = param.doContractTargetCustomer.BusinessTypeNameLC;
                }

                #endregion
                #region Usage

                if (param.doSiteData != null)
                {
                    dtTbt_ContractDocument.BuildingUsageCode = param.doSiteData.BuildingUsageCode;
                    dtTbt_ContractDocument.BuildingUsageNameEN = param.doSiteData.BuildingUsageNameEN;
                    dtTbt_ContractDocument.BuildingUsageNameLC = param.doSiteData.BuildingUsageNameLC;
                }

                #endregion
                #region Subject

                string[] subj = GetDocumentNameENLC(template, param.ContractType, changeType);
                dtTbt_ContractDocument.SubjectEN = subj[0];
                dtTbt_ContractDocument.SubjectLC = subj[1];

                #endregion
                #region Mapping from secreen

                dtTbt_ContractDocument.RelatedNo1 = data.RelatedOCCIncidentNo1;
                dtTbt_ContractDocument.RelatedNo2 = data.RelatedOCCIncidentNo2;
                dtTbt_ContractDocument.AttachDoc1 = data.AttachDocument1;
                dtTbt_ContractDocument.AttachDoc2 = data.AttachDocument2;
                dtTbt_ContractDocument.AttachDoc3 = data.AttachDocument3;
                dtTbt_ContractDocument.AttachDoc4 = data.AttachDocument4;
                dtTbt_ContractDocument.AttachDoc5 = data.AttachDocument5;
                dtTbt_ContractDocument.ApproveNo1 = data.ApproveNo1;
                dtTbt_ContractDocument.ApproveNo2 = data.ApproveNo2;
                dtTbt_ContractDocument.ApproveNo3 = data.ApproveNo3;
                dtTbt_ContractDocument.ContactMemo = data.ContractMemo;

                #endregion

                //Add by Jutarat A. on 15012013
                #region Operation Office Name
                
                IOfficeMasterHandler officeHandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                List<tbm_Office> tbm_OfficeList = officeHandler.GetTbm_Office();
                
                if (tbm_OfficeList != null)
                {
                    List<tbm_Office> operationOfficeList = (from t in tbm_OfficeList
                                                            where t.OfficeCode == dtTbt_ContractDocument.OperationOfficeCode
                                                            select t).ToList<tbm_Office>();

                    if (operationOfficeList != null && operationOfficeList.Count > 0)
                    {
                        dtTbt_ContractDocument.OperationOfficeNameEN = operationOfficeList[0].OfficeNameEN;
                        dtTbt_ContractDocument.OperationOfficeNameLC = operationOfficeList[0].OfficeNameLC;
                    }
                }
                #endregion
                //End Add

                return dtTbt_ContractDocument;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create DO of tbt_DocContractReport
        /// </summary>
        /// <param name="param"></param>
        /// <param name="template"></param>
        /// <param name="data"></param>
        /// <param name="dtTbt_ContractDocument"></param>
        /// <returns></returns>
        private tbt_DocContractReport CreateDocumentContractReport(CTS160_ScreenParameter param, CTS160_DocumentTemplateCondition template,
            CTS160_CoverLetterInformation data, tbt_ContractDocument dtTbt_ContractDocument)
        {
            try
            {
                tbt_DocContractReport dtTbt_DocContractReport = new tbt_DocContractReport();
                dtTbt_DocContractReport.DocID = dtTbt_ContractDocument.DocID;
                dtTbt_DocContractReport.DocumentLanguage = template.ContractLanguage;

                string dispatchType = null;
                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    dtTbt_DocContractReport.PlanCode = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].PlanCode;
                    dispatchType = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].DispatchTypeCode;
                    dtTbt_DocContractReport.OperationDate = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractStartDate;
                    dtTbt_DocContractReport.AutoRenewMonth = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].AutoRenewMonth;
                    dtTbt_DocContractReport.InstallFee_ApproveContract = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract;
                    dtTbt_DocContractReport.InstallFee_CompleteInstall = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall;
                    dtTbt_DocContractReport.InstallFee_StartService = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService;
                    dtTbt_DocContractReport.NegotiationTotalInstallFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee;

                    #region Mapping from billing

                    if (param.dtBillingTemp != null)
                    {
                        bool isSetDeptPayMethod = false;
                        bool isSetAPPayMethod = false;
                        bool isSetCPPayMethod = false;
                        bool isSetSTPayMethod = false;
                        foreach (dtTbt_BillingTempListForView b in param.dtBillingTemp)
                        {
                            if (b.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                            {
                                dtTbt_DocContractReport.DepositFeePhase = b.BillingTiming; //Add by Jutarat A. on 18122012

                                if (CommonUtil.IsNullOrEmpty(dtTbt_DocContractReport.DepositFeePayMethod)
                                    && isSetDeptPayMethod == false)
                                {
                                    dtTbt_DocContractReport.DepositFeePayMethod = b.PayMethod;
                                    isSetDeptPayMethod = true;
                                }
                                else if (dtTbt_DocContractReport.DepositFeePayMethod != b.PayMethod)
                                {
                                    dtTbt_DocContractReport.DepositFeePayMethod = null;
                                }
                            }
                            else if (b.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
                                && b.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                            {
                                if (CommonUtil.IsNullOrEmpty(dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract)
                                    && isSetAPPayMethod == false)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract = b.PayMethod;
                                    isSetAPPayMethod = true;
                                }
                                else if (dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract != b.PayMethod)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract = null;
                                }
                            }
                            else if (b.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
                            && b.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION)
                            {
                                if (CommonUtil.IsNullOrEmpty(dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall)
                                    && isSetCPPayMethod == false)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall = b.PayMethod;
                                    isSetCPPayMethod = true;
                                }
                                else if (dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall != b.PayMethod)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall = null;
                                }
                            }
                            else if (b.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
                                && b.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE)
                            {
                                if (CommonUtil.IsNullOrEmpty(dtTbt_DocContractReport.InstallFeePayMethod_StartService)
                                    && isSetSTPayMethod == false)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_StartService = b.PayMethod;
                                    isSetSTPayMethod = true;
                                }
                                else if (dtTbt_DocContractReport.InstallFeePayMethod_StartService != b.PayMethod)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_StartService = null;
                                }
                            }
                        }
                    }

                    #endregion
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.DRAFT_RENTAL)
                {
                    dtTbt_DocContractReport.PlanCode = param.dsEntireDraftContract.doTbt_DraftRentalContrat.PlanCode;
                    dispatchType = param.dsEntireDraftContract.doTbt_DraftRentalContrat.DispatchTypeCode;
                    dtTbt_DocContractReport.OperationDate = param.dsEntireDraftContract.doTbt_DraftRentalContrat.ExpectedStartServiceDate;
                    dtTbt_DocContractReport.AutoRenewMonth = param.dsEntireDraftContract.doTbt_DraftRentalContrat.AutoRenewMonth;
                    dtTbt_DocContractReport.InstallFee_ApproveContract = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContract;
                    dtTbt_DocContractReport.InstallFee_CompleteInstall = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstall;
                    dtTbt_DocContractReport.InstallFee_StartService = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderInstallFee_StartService;
                    dtTbt_DocContractReport.NegotiationTotalInstallFee = param.dsEntireDraftContract.doTbt_DraftRentalContrat.OrderInstallFee;

                    #region Mapping from billing

                    if (param.dsEntireDraftContract.doTbt_DraftRentalBillingTarget != null)
                    {
                        bool isSetDeptPayMethod = false;
                        bool isSetAPPayMethod = false;
                        bool isSetCPPayMethod = false;
                        bool isSetSTPayMethod = false;
                        foreach (tbt_DraftRentalBillingTarget b in param.dsEntireDraftContract.doTbt_DraftRentalBillingTarget)
                        {
                            if (b.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                            {
                                dtTbt_DocContractReport.DepositFeePhase = b.BillingTiming;

                                if (CommonUtil.IsNullOrEmpty(dtTbt_DocContractReport.DepositFeePayMethod)
                                    && isSetDeptPayMethod == false)
                                {
                                    dtTbt_DocContractReport.DepositFeePayMethod = b.PayMethod;
                                    isSetDeptPayMethod = true;
                                }
                                else if (dtTbt_DocContractReport.DepositFeePayMethod != b.PayMethod)
                                {
                                    dtTbt_DocContractReport.DepositFeePayMethod = null;
                                }
                            }
                            else if (b.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
                                && b.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                            {
                                if (CommonUtil.IsNullOrEmpty(dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract)
                                    && isSetAPPayMethod == false)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract = b.PayMethod;
                                    isSetAPPayMethod = true;
                                }
                                else if (dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract != b.PayMethod)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract = null;
                                }
                            }
                            else if (b.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
                            && b.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION)
                            {
                                if (CommonUtil.IsNullOrEmpty(dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall)
                                    && isSetCPPayMethod == false)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall = b.PayMethod;
                                    isSetCPPayMethod = true;
                                }
                                else if (dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall != b.PayMethod)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall = null;
                                }
                            }
                            else if (b.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
                                && b.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE)
                            {
                                if (CommonUtil.IsNullOrEmpty(dtTbt_DocContractReport.InstallFeePayMethod_StartService)
                                    && isSetSTPayMethod == false)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_StartService = b.PayMethod;
                                    isSetSTPayMethod = true;
                                }
                                else if (dtTbt_DocContractReport.InstallFeePayMethod_StartService != b.PayMethod)
                                {
                                    dtTbt_DocContractReport.InstallFeePayMethod_StartService = null;
                                }
                            }
                        }
                    }

                    #endregion
                }

                #region dispatch type

                ICommonHandler cHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> lst = cHandler.GetMiscTypeCodeListByFieldName(new List<string>() { MiscType.C_DISPATCH_TYPE });
                foreach (doMiscTypeCode m in lst)
                {
                    if (m.ValueCode == dispatchType)
                    {
                        if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN)
                            dtTbt_DocContractReport.DispatchType = m.ValueDisplayEN;
                        else
                            dtTbt_DocContractReport.DispatchType = m.ValueDisplayLC;
                        break;
                    }
                }
                
                #endregion

                return dtTbt_DocContractReport;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create DO of tbt_DocChangeMemo
        /// </summary>
        /// <param name="param"></param>
        /// <param name="dtTbt_ContractDocument"></param>
        /// <param name="prevOldContractFee"></param>
        /// <returns></returns>
        private tbt_DocChangeMemo CreateDocumentChangeMemo(CTS160_ScreenParameter param, tbt_ContractDocument dtTbt_ContractDocument, 
            string prevOldContractFeeCurrencyType, decimal? prevOldContractFee, decimal? prevOldContractFeeUsd)
        {
            try
            {
                tbt_DocChangeMemo dtTbt_DocChangeMemo = new tbt_DocChangeMemo();

                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    dtTbt_DocChangeMemo.DocID = dtTbt_ContractDocument.DocID;
                    dtTbt_DocChangeMemo.EffectiveDate = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;

                    dtTbt_DocChangeMemo.OldContractFeeCurrencyType = prevOldContractFeeCurrencyType;
                    dtTbt_DocChangeMemo.OldContractFee = prevOldContractFee;
                    dtTbt_DocChangeMemo.OldContractFeeUsd = prevOldContractFeeUsd;

                    dtTbt_DocChangeMemo.NewContractFeeCurrencyType = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType;
                    dtTbt_DocChangeMemo.NewContractFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFee;
                    dtTbt_DocChangeMemo.NewContractFeeUsd = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd;
                }

                return dtTbt_DocChangeMemo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Create DO of tbt_DocChangeNotice
        /// </summary>
        /// <param name="param"></param>
        /// <param name="dtTbt_ContractDocument"></param>
        /// <returns></returns>
        private tbt_DocChangeNotice CreateDocumentChangeNotice(CTS160_ScreenParameter param, tbt_ContractDocument dtTbt_ContractDocument)
        {
            try
            {
                tbt_DocChangeNotice dtTbt_DocChangeNotice = new tbt_DocChangeNotice();

                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    dtTbt_DocChangeNotice.DocID = dtTbt_ContractDocument.DocID;
                    dtTbt_DocChangeNotice.EffectiveDate = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;
                }

                return dtTbt_DocChangeNotice;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Create DO of tbt_DocConfirmCurrentInstrumentMemo
        /// </summary>
        /// <param name="param"></param>
        /// <param name="dtTbt_ContractDocument"></param>
        /// <returns></returns>
        private tbt_DocConfirmCurrentInstrumentMemo CreateDocumentConfirmCurrentInstrumentMemo(CTS160_ScreenParameter param, tbt_ContractDocument dtTbt_ContractDocument)
        {
            try
            {
                tbt_DocConfirmCurrentInstrumentMemo dtTbt_DocConfirmCurrentInstrumentMemo = new tbt_DocConfirmCurrentInstrumentMemo();

                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    dtTbt_DocConfirmCurrentInstrumentMemo.DocID = dtTbt_ContractDocument.DocID;
                    dtTbt_DocConfirmCurrentInstrumentMemo.RealInvestigationDate = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;
                }

                return dtTbt_DocConfirmCurrentInstrumentMemo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create DO of tbt_DocStartMemo
        /// </summary>
        /// <param name="param"></param>
        /// <param name="dtTbt_ContractDocument"></param>
        /// <returns></returns>
        private tbt_DocStartMemo CreateDocumentStartMemo(CTS160_ScreenParameter param, tbt_ContractDocument dtTbt_ContractDocument) //Add by Jutarat A. on 22042013
        {
            try
            {
                string strPlanCode = null;
                if (param.dsRentalContractData != null && param.dsRentalContractData.dtTbt_RentalSecurityBasic != null && param.dsRentalContractData.dtTbt_RentalSecurityBasic.Count > 0)
                    strPlanCode = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].PlanCode;

                tbt_DocStartMemo dtTbt_DocStartMemo = new tbt_DocStartMemo();
                dtTbt_DocStartMemo.DocID = dtTbt_ContractDocument.DocID;
                dtTbt_DocStartMemo.PlanCode = strPlanCode;

                return dtTbt_DocStartMemo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Create DO of tbt_DocCancelContractMemo
        /// </summary>
        /// <param name="param"></param>
        /// <param name="dtTbt_ContractDocument"></param>
        /// <returns></returns>
        private tbt_DocCancelContractMemo CreateDocumentCancelContractMemo(CTS160_ScreenParameter param, tbt_ContractDocument dtTbt_ContractDocument)
        {
            try
            {
                tbt_DocCancelContractMemo dtTbt_DocCancelContractMemo = new tbt_DocCancelContractMemo();

                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    dtTbt_DocCancelContractMemo.DocID = dtTbt_ContractDocument.DocID;
                    dtTbt_DocCancelContractMemo.CancelContractDate = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;
                    dtTbt_DocCancelContractMemo.StartServiceDate = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractStartDate;
                    dtTbt_DocCancelContractMemo.AutoTransferBillingType = "0";
                    dtTbt_DocCancelContractMemo.BankTransferBillingType = "0";

                    #region Mapping from tbt_CancelContractMemo

                    if (param.dsRentalContractData.dtTbt_CancelContractMemo != null)
                    {
                        if (param.dsRentalContractData.dtTbt_CancelContractMemo.Count > 0)
                        {
							// 20170306 nakajima modify start
                            dtTbt_DocCancelContractMemo.TotalSlideAmt = param.dsRentalContractData.dtTbt_CancelContractMemo[0].TotalSlideAmt;
                            dtTbt_DocCancelContractMemo.TotalSlideAmtUsd = param.dsRentalContractData.dtTbt_CancelContractMemo[0].TotalSlideAmtUsd;
                            dtTbt_DocCancelContractMemo.TotalReturnAmt = param.dsRentalContractData.dtTbt_CancelContractMemo[0].TotalReturnAmt;
                            dtTbt_DocCancelContractMemo.TotalReturnAmtUsd = param.dsRentalContractData.dtTbt_CancelContractMemo[0].TotalReturnAmtUsd;
                            dtTbt_DocCancelContractMemo.TotalBillingAmt = param.dsRentalContractData.dtTbt_CancelContractMemo[0].TotalBillingAmt;
                            dtTbt_DocCancelContractMemo.TotalBillingAmtUsd = param.dsRentalContractData.dtTbt_CancelContractMemo[0].TotalBillingAmtUsd;
                            dtTbt_DocCancelContractMemo.TotalAmtAfterCounterBalance = param.dsRentalContractData.dtTbt_CancelContractMemo[0].TotalAmtAfterCounterBalance;
                            dtTbt_DocCancelContractMemo.TotalAmtAfterCounterBalanceUsd = param.dsRentalContractData.dtTbt_CancelContractMemo[0].TotalAmtAfterCounterBalanceUsd;
                            // 20170306 nakajima modify end
                            #region Process after counter balance type

                            dtTbt_DocCancelContractMemo.ProcessAfterCounterBalanceType = param.dsRentalContractData.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType;
                            dtTbt_DocCancelContractMemo.ProcessAfterCounterBalanceTypeUsd = param.dsRentalContractData.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceTypeUsd;

                            ICommonHandler cHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                            dtTbt_DocCancelContractMemo.ProcessAfterCounterBalanceTypeName = 
                                cHandler.GetMiscDisplayValue(   MiscType.C_PROC_AFT_COUNTER_BALANCE_TYPE,
                                                                dtTbt_DocCancelContractMemo.ProcessAfterCounterBalanceType);

                            #endregion

                            dtTbt_DocCancelContractMemo.OtherRemarks = param.dsRentalContractData.dtTbt_CancelContractMemo[0].OtherRemarks;
                        }
                    }

                    #endregion
                }

                return dtTbt_DocCancelContractMemo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Create DO List of tbt_DocCancelContractMemoDetail
        /// </summary>
        /// <param name="param"></param>
        /// <param name="dtTbt_ContractDocument"></param>
        /// <returns></returns>
        private List<tbt_DocCancelContractMemoDetail> CreateDocumentCancelContractMemoDetail(CTS160_ScreenParameter param, tbt_ContractDocument dtTbt_ContractDocument)
        {
            try
            {
                List<tbt_DocCancelContractMemoDetail> dtTbt_DocCancelContractMemoDetail = new List<tbt_DocCancelContractMemoDetail>();

                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    if (param.dsRentalContractData.dtTbt_CancelContractMemoDetail != null)
                    {
                        //Add by Jutarat A. on 09082012
                        List<string> strMiscList = new List<string>();
                        strMiscList.Add(MiscType.C_CONTRACT_BILLING_TYPE);
                        strMiscList.Add(MiscType.C_HANDLING_TYPE);

                        ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        List<doMiscTypeCode> miscTypeList = comHandler.GetMiscTypeCodeListByFieldName(strMiscList);

                        List<doMiscTypeCode> miscBillingType = new List<doMiscTypeCode>();
                        List<doMiscTypeCode> miscHandlingType = new List<doMiscTypeCode>();
                        //End Add

                        foreach (tbt_CancelContractMemoDetail detail in param.dsRentalContractData.dtTbt_CancelContractMemoDetail)
                        {
                            tbt_DocCancelContractMemoDetail nd = new tbt_DocCancelContractMemoDetail();
                            dtTbt_DocCancelContractMemoDetail.Add(nd);

                            nd.DocID = dtTbt_ContractDocument.DocID;

                            //Modify by Jutarat A. on 09082012
                            //nd.BillingType = detail.BillingType;
                            //nd.BillingTypeName = detail.BillingTypeName;
                            //nd.HandlingType = detail.HandlingType;
                            //nd.HandlingTypeName = detail.HandlingTypeName;
                            miscBillingType = (from t in miscTypeList
                                               where t.FieldName == MiscType.C_CONTRACT_BILLING_TYPE
                                               && t.ValueCode == detail.BillingType
                                               select t).ToList<doMiscTypeCode>();

                            nd.BillingType = detail.BillingType;
                            if (miscBillingType != null && miscBillingType.Count > 0)
                                nd.BillingTypeName = miscBillingType[0].ValueDisplayLC;
                            else
                                nd.BillingTypeName = null;


                            miscHandlingType = (from t in miscTypeList
                                                where t.FieldName == MiscType.C_HANDLING_TYPE
                                                && t.ValueCode == detail.HandlingType
                                                select t).ToList<doMiscTypeCode>();

                            nd.HandlingType = detail.HandlingType;
                            if (miscHandlingType != null && miscHandlingType.Count > 0)
                                nd.HandlingTypeName = miscHandlingType[0].ValueDisplayLC;
                            else
                                nd.HandlingTypeName = null;
                            //End Modify

                            nd.StartPeriodDate = detail.StartPeriodDate;
                            nd.EndPeriodDate = detail.EndPeriodDate;

                            // if Amount and AmountUsd is null -> set Currency to null value
                            if(detail.NormalFeeAmount == null && detail.NormalFeeAmountUsd == null)
                            {
                                nd.NormalFeeAmountCurrencyType = null;
                                nd.NormalFeeAmountUsd = null;
                                nd.NormalFeeAmount = null;
                            }
                            else
                            {
                                if(detail.NormalFeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    nd.NormalFeeAmountCurrencyType = detail.NormalFeeAmountCurrencyType;
                                    nd.NormalFeeAmountUsd = detail.NormalFeeAmountUsd;
                                    nd.NormalFeeAmount = null;
                                }
                                else
                                {
                                    nd.NormalFeeAmountCurrencyType = detail.NormalFeeAmountCurrencyType;
                                    nd.NormalFeeAmountUsd = null;
                                    nd.NormalFeeAmount = detail.NormalFeeAmount;
                                }
                            }

                            if(detail.FeeAmount == null && detail.FeeAmountUsd == null)
                            {
                                nd.FeeAmountCurrencyType = null;
                                nd.FeeAmountUsd = null;
                                nd.FeeAmount = null;
                            }
                            else
                            {
                                if (detail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    nd.FeeAmountCurrencyType = detail.FeeAmountCurrencyType;
                                    nd.FeeAmountUsd = detail.FeeAmountUsd;
                                    nd.FeeAmount = null;
                                }
                                else
                                {
                                    nd.FeeAmountCurrencyType = detail.FeeAmountCurrencyType;
                                    nd.FeeAmountUsd = null;
                                    nd.FeeAmount = detail.FeeAmount;
                                }
                            }
                            
                            if(detail.TaxAmount == null && detail.TaxAmountUsd == null)
                            {
                                nd.TaxAmountCurrencyType = null;
                                nd.TaxAmountUsd = null;
                                nd.TaxAmount = null;
                            }
                            else
                            {
                                if (detail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    nd.TaxAmountCurrencyType = detail.TaxAmountCurrencyType;
                                    nd.TaxAmountUsd = detail.TaxAmountUsd;
                                    nd.TaxAmount = null;
                                }
                                else
                                {
                                    nd.TaxAmountCurrencyType = detail.TaxAmountCurrencyType;
                                    nd.TaxAmountUsd = null;
                                    nd.TaxAmount = detail.TaxAmount;
                                }
                            }
                            
                            nd.ContractCode_CounterBalance = detail.ContractCode_CounterBalance;
                            nd.Remark = detail.Remark;
                        }
                    }
                }

                return dtTbt_DocCancelContractMemoDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Create DO of tbt_DocChangeFeeMemo
        /// </summary>
        /// <param name="param"></param>
        /// <param name="dtTbt_ContractDocument"></param>
        /// <param name="prevOldContractFee"></param>
        /// <returns></returns>
        private tbt_DocChangeFeeMemo CreateDocumentChangeFeeMemo(CTS160_ScreenParameter param, tbt_ContractDocument dtTbt_ContractDocument,
            string prevOldContractFeeCurrencyType, decimal? prevOldContractFee, decimal? prevOldContractFeeUsd)
        {
            try
            {
                tbt_DocChangeFeeMemo dtTbt_DocChangeFeeMemo = new tbt_DocChangeFeeMemo();

                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    dtTbt_DocChangeFeeMemo.DocID = dtTbt_ContractDocument.DocID;
                    dtTbt_DocChangeFeeMemo.EffectiveDate = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;

                    dtTbt_DocChangeFeeMemo.OldContractFeeCurrencyType = prevOldContractFeeCurrencyType;
                    dtTbt_DocChangeFeeMemo.OldContractFee = prevOldContractFee;
                    dtTbt_DocChangeFeeMemo.OldContractFeeUsd = prevOldContractFeeUsd;


                    dtTbt_DocChangeFeeMemo.NewContractFeeCurrencyType = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType;
                    dtTbt_DocChangeFeeMemo.NewContractFee = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFee;
                    dtTbt_DocChangeFeeMemo.NewContractFeeUsd = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd;

                    dtTbt_DocChangeFeeMemo.ChangeContractFeeDate = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;
                    dtTbt_DocChangeFeeMemo.ReturnToOriginalFeeDate = param.dsRentalContractData.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate;
                }

                return dtTbt_DocChangeFeeMemo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Create DO List of tbt_DocInstrumentDetails
        /// </summary>
        /// <param name="param"></param>
        /// <param name="dtTbt_ContractDocument"></param>
        /// <returns></returns>
        private List<tbt_DocInstrumentDetails> CreateDocumentInstrumentDetails(CTS160_ScreenParameter param, tbt_ContractDocument dtTbt_ContractDocument)
        {
            try
            {
                List<tbt_DocInstrumentDetails> dtTbt_DocInstrumentDetails = new List<tbt_DocInstrumentDetails>();

                if (param.ContractType == CTS160_CONTRACT_TYPE.RENTAL)
                {
                    if (param.dsRentalContractData.dtTbt_RentalInstrumentDetails != null)
                    {
                        foreach (tbt_RentalInstrumentDetails detail in param.dsRentalContractData.dtTbt_RentalInstrumentDetails)
                        {
                            if (detail.InstrumentTypeCode == InstrumentType.C_INST_TYPE_GENERAL)
                            {
                                tbt_DocInstrumentDetails nd = new tbt_DocInstrumentDetails();
                                dtTbt_DocInstrumentDetails.Add(nd);

                                nd.DocID = dtTbt_ContractDocument.DocID;
                                nd.InstrumentCode = detail.InstrumentCode;
                                nd.InstrumentQty = (((detail.InstrumentQty ?? 0) + (detail.AdditionalInstrumentQty ?? 0)) - (detail.RemovalInstrumentQty ?? 0)); //nd.InstrumentQty = detail.InstrumentQty; //Modify by Jutarat A. on 12102012
                            }
                        }
                    }
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.DRAFT_RENTAL)
                {
                    if (param.dsEntireDraftContract.doTbt_DraftRentalInstrument != null)
                    {
                        foreach (tbt_DraftRentalInstrument detail in param.dsEntireDraftContract.doTbt_DraftRentalInstrument)
                        {
                            if (detail.InstrumentTypeCode == InstrumentType.C_INST_TYPE_GENERAL)
                            {
                                tbt_DocInstrumentDetails nd = new tbt_DocInstrumentDetails();
                                dtTbt_DocInstrumentDetails.Add(nd);

                                nd.DocID = dtTbt_ContractDocument.DocID;
                                nd.InstrumentCode = detail.InstrumentCode;
                                nd.InstrumentQty = detail.InstrumentQty;
                            }
                        }
                    }
                }
                else if (param.ContractType == CTS160_CONTRACT_TYPE.SALE)
                {
                    if (param.doSaleContractData.dtTbt_SaleInstrumentDetails != null)
                    {
                        foreach (dsSaleInstrumentDetails detail in param.doSaleContractData.dtTbt_SaleInstrumentDetails)
                        {
                            tbt_DocInstrumentDetails nd = new tbt_DocInstrumentDetails();
                            dtTbt_DocInstrumentDetails.Add(nd);

                            nd.DocID = dtTbt_ContractDocument.DocID;
                            nd.InstrumentCode = detail.InstrumentCode;
                            nd.InstrumentQty = detail.InstrumentQty;
                        }
                    }
                }

                return dtTbt_DocInstrumentDetails.Count > 0 ? dtTbt_DocInstrumentDetails : null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion



        //#region Authority

        //public ActionResult CTS160_Authority(CTS160_ScreenParameter sParam)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    CommonUtil comUtil = new CommonUtil();
        //    string strContractCodeLong = string.Empty;
        //    string strOCCCode = string.Empty;

        //    ICustomerMasterHandler cusMasterHandler;
        //    ISiteMasterHandler siteMasHandler;
        //    ICommonContractHandler comContractHandler;
        //    IUserControlHandler userCtrlHandler;

        //    dsRentalContractData dsRentalContract = null;
        //    doRentalContractBasicInformation doRentalContractBasic = null;
        //    List<doCustomer> doContractTargetCustomerList = null;
        //    List<doCustomer> doRealCustomerList = null;
        //    List<doSite> doSiteList = null;
        //    List<dtTbt_BillingTempListForView> dtBillingTempListForView = null;
        //    tbt_RentalContractBasic tbt_RentalContractBasicData = null;
        //    CTS070_doRentalContractBasicAuthority doRentalContractBasicAuthority;

        //    try
        //    {
        //        //CheckSystemStatus
        //        if (CheckIsSuspending(res) == true)
        //            return Json(res);

        //        //Check screen permission
        //        if (CheckUserPermission(ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
        //            return Json(res);
        //        }

        //        if (String.IsNullOrEmpty(sParam.ContractCode) == false)
        //        {
        //            //Get rental contract data
        //            strContractCodeLong = comUtil.ConvertContractCode(sParam.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
        //            dsRentalContract = CheckDataAuthority_CTS100(res, strContractCodeLong, true);
        //            if (res.IsError)
        //                return Json(res);

        //            strOCCCode = dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC;

        //            userCtrlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
        //            doRentalContractBasic = userCtrlHandler.GetRentalContactBasicInformationData(strContractCodeLong);

        //            cusMasterHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

        //            //Get contract target data
        //            doContractTargetCustomerList = cusMasterHandler.GetCustomer(dsRentalContract.dtTbt_RentalContractBasic[0].ContractTargetCustCode);

        //            //Get real customer data
        //            doRealCustomerList = cusMasterHandler.GetCustomer(dsRentalContract.dtTbt_RentalContractBasic[0].RealCustomerCustCode);

        //            //Get site data
        //            siteMasHandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
        //            doSiteList = siteMasHandler.GetSite(dsRentalContract.dtTbt_RentalContractBasic[0].SiteCode, null);

        //            //Get billing temp data
        //            comContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
        //            dtBillingTempListForView = comContractHandler.GetTbt_BillingTempListForView(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

        //            //Check data authority
        //            tbt_RentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];
        //            doRentalContractBasicAuthority = CommonUtil.CloneObject<tbt_RentalContractBasic, CTS070_doRentalContractBasicAuthority>(tbt_RentalContractBasicData);
        //            ValidatorUtil.BuildErrorMessage(res, new object[] { doRentalContractBasicAuthority }, null, false);
        //            if (res.IsError)
        //                return Json(res);

        //        }
        //        /*-------------------------*/

        //        //sParam = new CTS160_ScreenParameter();
        //        sParam.ContractCode = strContractCodeLong;
        //        sParam.OCCAlphabet = strOCCCode;
        //        sParam.RentalContractData = dsRentalContract;
        //        sParam.doRentalContractBasicData = doRentalContractBasic;
        //        sParam.ContractTargetCustomerList = doContractTargetCustomerList;
        //        sParam.RealCustomerList = doRealCustomerList;
        //        sParam.SiteList = doSiteList;
        //        sParam.BillingTempListForView = dtBillingTempListForView;

        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }

        //    return InitialScreenEnvironment<CTS160_ScreenParameter>("CTS160", sParam, res);
        //}

        //#endregion

        //#region Action

        //[Initialize("CTS160")]
        //public ActionResult CTS160()
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
        //    CommonUtil comUtil = new CommonUtil();

        //    IDocumentHandler docHandler;
        //    List<tbm_DocumentTemplate> dtTbm_DocumentTemplate = new List<tbm_DocumentTemplate>();

        //    try
        //    {
        //        CTS160_ScreenParameter sParam = GetScreenObject<CTS160_ScreenParameter>();

        //        docHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
        //        dtTbm_DocumentTemplate = docHandler.GetTbm_DocumentTemplate(DocumentType.C_DOCUMENT_TYPE_CONTRACT);

        //        //Map data to screen
        //        if (sParam.doRentalContractBasicData != null)
        //        {
        //            ViewBag.ContractCode = comUtil.ConvertContractCode(sParam.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
        //            ViewBag.LastOCC = sParam.OCCAlphabet;
        //            ViewBag.ContractTargetNameEN = sParam.doRentalContractBasicData.ContractTargetNameEN;
        //            ViewBag.RealCustomerNameEN = sParam.doRentalContractBasicData.RealCustomerNameEN;
        //            ViewBag.SiteNameEN = sParam.doRentalContractBasicData.SiteNameEN;
        //            ViewBag.ContractTargetNameLC = sParam.doRentalContractBasicData.ContractTargetNameLC;
        //            ViewBag.RealCustomerNameLC = sParam.doRentalContractBasicData.RealCustomerNameLC;
        //            ViewBag.SiteNameLC = sParam.doRentalContractBasicData.SiteNameLC;
        //            ViewBag.ChangeTypeName = sParam.doRentalContractBasicData.LastChangeTypeName;

        //            //ViewBag.ChangeType = sParam.doRentalContractBasicData.LastChangeType;
        //            if (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalSecurityBasic != null && sParam.RentalContractData.dtTbt_RentalSecurityBasic.Count > 0)
        //                ViewBag.ChangeType = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].ChangeType;

        //            ViewBag.DraftRentalContractStatus = string.Empty;
        //            ViewBag.SecOCC = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].OCC;
        //        }

        //        sParam.DocumentTemplateData = dtTbm_DocumentTemplate;
        //        UpdateScreenObject(sParam);

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}

        //public ActionResult CTS160_InitialGridInstrumentList()
        //{
        //    return Json(CommonUtil.ConvertToXml<object>(null, "Contract\\CTS160", CommonUtil.GRID_EMPTY_TYPE.VIEW));
        //}

        //public ActionResult CTS160_RetrieveData(CTS160_SpecifyContractCondition cond)
        //{
        //    ObjectResultData res = new ObjectResultData();

        //    CommonUtil comUtil = new CommonUtil();
        //    IRentralContractHandler rentralHandler;
        //    IDraftRentalContractHandler drafRentralHandler;
        //    ICustomerMasterHandler cusMasterHandler;
        //    ISiteMasterHandler siteMasHandler;
        //    ICommonContractHandler comContractHandler;
        //    IUserControlHandler userCtrlHandler;

        //    dsRentalContractData dsRentalContract = null;
        //    doDraftRentalContractData doDraftRentalContract = null;
        //    doRentalContractBasicInformation doRentalContractBasicInfo = null;
        //    List<doCustomer> doContractTargetCustomerList = null;
        //    List<doCustomer> doRealCustomerList = null;
        //    List<doSite> doSiteList = null;
        //    List<dtTbt_BillingTempListForView> dtBillingTempListForView = null;
        //    tbt_RentalContractBasic tbt_RentalContractBasicData = null;
        //    CTS070_doRentalContractBasicAuthority doRentalContractBasicAuthority = null;

        //    tbt_RentalSecurityBasic tbt_RentalSecurityBasicData = null;
        //    doDraftRentalContractInformation doDraftRentalContractInfo = null;
        //    CTS160_SpecifyContractData specifyContractData = new CTS160_SpecifyContractData();

        //    try
        //    {
        //        //Check mandatory
        //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
        //        if (String.IsNullOrEmpty(cond.ContractCode))
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
        //                                ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT,
        //                                MessageUtil.MODULE_COMMON,
        //                                MessageUtil.MessageList.MSG0007,
        //                                new string[] { "lblContractQuotationTargetCode" },
        //                                new string[] { "txtSpecifyContractCode" });

        //            return Json(res);
        //        }

        //        //Get rental contract data
        //        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
        //        rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
        //        dsRentalContract = rentralHandler.GetEntireContract(cond.ContractCodeLong, cond.OCCAlphabet);
        //        if (dsRentalContract == null || dsRentalContract.dtTbt_RentalContractBasic == null || dsRentalContract.dtTbt_RentalContractBasic.Count < 1)
        //        {
        //            //Get draft rental contract data
        //            drafRentralHandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
        //            doDraftRentalContractCondition draftCond = new doDraftRentalContractCondition();
        //            draftCond.QuotationTargetCode = cond.QuotationTargetCode;
        //            draftCond.Alphabet = cond.OCCAlphabet;
        //            doDraftRentalContract = drafRentralHandler.GetEntireDraftRentalContract(draftCond, doDraftRentalContractData.RENTAL_CONTRACT_MODE.APPROVE);
        //            if (doDraftRentalContract == null || doDraftRentalContract.doTbt_DraftRentalContrat == null)
        //            {
        //                res.ResultData = false;
        //                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { cond.ContractCode }, new string[] { "txtSpecifyContractCode" });
        //                return Json(res);
        //            }
        //            else
        //            {
        //                List<doDraftRentalContractInformation> doDraftRentalList = drafRentralHandler.GetDraftRentalContractInformationData(cond.QuotationTargetCodeLong);
        //                if (doDraftRentalList != null && doDraftRentalList.Count > 0)
        //                    doDraftRentalContractInfo = doDraftRentalList[0];
        //            }
        //        }
        //        else
        //        {
        //            userCtrlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
        //            doRentalContractBasicInfo = userCtrlHandler.GetRentalContactBasicInformationData(cond.ContractCodeLong);
        //        }

        //        cusMasterHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

        //        //Get contract target data
        //        string strCustCode = doDraftRentalContract != null ? doDraftRentalContract.doTbt_DraftRentalContrat.ContractTargetCustCode : dsRentalContract.dtTbt_RentalContractBasic[0].ContractTargetCustCode;
        //        doContractTargetCustomerList = cusMasterHandler.GetCustomer(strCustCode);

        //        //Get real customer data
        //        strCustCode = doDraftRentalContract != null ? doDraftRentalContract.doTbt_DraftRentalContrat.RealCustomerCustCode : dsRentalContract.dtTbt_RentalContractBasic[0].RealCustomerCustCode;
        //        doRealCustomerList = cusMasterHandler.GetCustomer(strCustCode);

        //        //Get site data
        //        siteMasHandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
        //        string strSiteCode = doDraftRentalContract != null ? doDraftRentalContract.doTbt_DraftRentalContrat.SiteCode : dsRentalContract.dtTbt_RentalContractBasic[0].SiteCode;
        //        doSiteList = siteMasHandler.GetSite(strSiteCode, null);

        //        if (dsRentalContract != null && dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
        //        {
        //            //Get billing temp data
        //            tbt_RentalSecurityBasicData = dsRentalContract.dtTbt_RentalSecurityBasic[0];
        //            comContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
        //            dtBillingTempListForView = comContractHandler.GetTbt_BillingTempListForView(tbt_RentalSecurityBasicData.ContractCode, null);
        //        }

        //        //Check data authority
        //        if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
        //        {
        //            tbt_RentalContractBasicData = dsRentalContract.dtTbt_RentalContractBasic[0];
        //            doRentalContractBasicAuthority = CommonUtil.CloneObject<tbt_RentalContractBasic, CTS070_doRentalContractBasicAuthority>(tbt_RentalContractBasicData);
        //            ValidatorUtil.BuildErrorMessage(res, new object[] { doRentalContractBasicAuthority }, null, false);
        //            if (res.IsError)
        //                return Json(res);
        //        }

        //        CTS160_ScreenParameter sParam = GetScreenObject<CTS160_ScreenParameter>();
        //        sParam.ContractCode = cond.ContractCodeLong;
        //        sParam.QuotationTargetCode = cond.QuotationTargetCodeLong;

        //        if (dsRentalContract != null && dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
        //            sParam.OCCAlphabet = String.IsNullOrEmpty(cond.OCCAlphabet) == true ? dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC : cond.OCCAlphabet;

        //        sParam.RentalContractData = dsRentalContract;
        //        sParam.DraftRentalContractData = doDraftRentalContract;
        //        sParam.doRentalContractBasicData = doRentalContractBasicInfo;
        //        sParam.doDraftRentalContractData = doDraftRentalContractInfo;
        //        sParam.ContractTargetCustomerList = doContractTargetCustomerList;
        //        sParam.RealCustomerList = doRealCustomerList;
        //        sParam.SiteList = doSiteList;
        //        sParam.BillingTempListForView = dtBillingTempListForView;


        //        //string strDraftRentalContractStatus = (doDraftRentalContract != null && doDraftRentalContract.doTbt_DraftRentalContrat != null) ?
        //        //                                        doDraftRentalContract.doTbt_DraftRentalContrat.DraftRentalContractStatus : string.Empty;
        //        //string strSecOCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
        //        if (doRentalContractBasicInfo != null)
        //        {
        //            specifyContractData = CommonUtil.CloneObject<doRentalContractBasicInformation, CTS160_SpecifyContractData>(doRentalContractBasicInfo);

        //            if (dsRentalContract != null && dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
        //            {
        //                specifyContractData.ChangeType = dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType;
        //                specifyContractData.SecOCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
        //            }
        //        }
        //        else if (doDraftRentalContractInfo != null)
        //        {
        //            specifyContractData = CommonUtil.CloneObject<doDraftRentalContractInformation, CTS160_SpecifyContractData>(doDraftRentalContractInfo);
        //            specifyContractData.DraftStatus = doDraftRentalContract.doTbt_DraftRentalContrat.DraftRentalContractStatus;
        //        }

        //        res.ResultData = specifyContractData;
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}

        //public ActionResult CTS160_SelectData(CTS160_DocumentTemplateCondition cond)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
        //    CTS160_CoverLetterData coverLetterData = new CTS160_CoverLetterData();
        //    List<tbt_BillingBasic> dtTbt_BillingBasic;

        //    IMasterHandler masHandler;
        //    ICommonHandler comHandler;
        //    IBillingHandler billHandler;

        //    try
        //    {
        //        if (cond.ContractFlag == false && cond.MemorandumFlag == false && cond.NoticeFlag == false && cond.CoverLetterFlag == false)
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
        //                                ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT,
        //                                MessageUtil.MODULE_COMMON,
        //                                MessageUtil.MessageList.MSG0007,
        //                                new string[] { "lblTitleDocumentTemplate" });

        //            return Json(res);
        //        }

        //        if (cond.ContractFlag == true)
        //        {
        //            if (cond.ContractDocumentLanguage == null)
        //            {
        //                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
        //                                    ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT,
        //                                    MessageUtil.MODULE_COMMON,
        //                                    MessageUtil.MessageList.MSG0007,
        //                                    new string[] { "lblContractDocumentLanguage" },
        //                                    new string[] { "ddlContractDocumentLanguage" });

        //                return Json(res);
        //            }
        //        }

        //        if (cond.CoverLetterFlag == true)
        //        {
        //            if (cond.DocumentName == null)
        //            {
        //                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
        //                                    ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT,
        //                                    MessageUtil.MODULE_COMMON,
        //                                    MessageUtil.MessageList.MSG0007,
        //                                    new string[] { "lblDocumentName" },
        //                                    new string[] { "ddlDocumentName" });

        //                return Json(res);
        //            }
        //        }

        //        masHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
        //        comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;


        //        CTS160_ScreenParameter sParam = GetScreenObject<CTS160_ScreenParameter>();

        //        //Get tbt_BillingBasic data
        //        dtTbt_BillingBasic = billHandler.GetTbt_BillingBasic(sParam.ContractCode, null);

        //        //coverLetterData.EmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
        //        coverLetterData.EmpFullName = CommonUtil.dsTransData.dtUserData.EmpFullName;

        //        coverLetterData.MainOfficeCode = CommonUtil.dsTransData.dtUserData.MainOfficeCode;
        //        List<OfficeDataDo> officeDataList = null;
        //        officeDataList = (from t in CommonUtil.dsTransData.dtOfficeData
        //                          where t.OfficeCode == coverLetterData.MainOfficeCode
        //                          select t).ToList<OfficeDataDo>();

        //        if (officeDataList != null && officeDataList.Count > 0)
        //            coverLetterData.MainOfficeName = officeDataList[0].OfficeName;


        //        if (sParam.DocumentTemplateData != null && sParam.DocumentTemplateData.Count > 0)
        //        {
        //            List<tbm_DocumentTemplate> docTemplateList = CommonUtil.ClonsObjectList<tbm_DocumentTemplate, tbm_DocumentTemplate>(sParam.DocumentTemplateData);

        //            string strChangeType = string.Empty;
        //            if (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalSecurityBasic != null && sParam.RentalContractData.dtTbt_RentalSecurityBasic.Count > 0)
        //                strChangeType = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].ChangeType;

        //            //coverLetterData.DocumentCode = GetDocumentCode_CTS160(cond, sParam.doRentalContractBasicData.LastChangeType);
        //            coverLetterData.DocumentCode = GetDocumentCode_CTS160(cond, strChangeType);

        //            List<tbm_DocumentTemplate> dtTbm_DocumentTemplate = (from t in docTemplateList
        //                                                                 where t.DocumentCode == coverLetterData.DocumentCode
        //                                                                 select t).ToList<tbm_DocumentTemplate>();

        //            if (dtTbm_DocumentTemplate != null && dtTbm_DocumentTemplate.Count > 0)
        //            {
        //                //coverLetterData.DocumentName = (cond.ContractDocumentLanguage != null && cond.ContractDocumentLanguage == DocLanguage.C_DOC_LANGUAGE_EN) ?
        //                //                             dtTbm_DocumentTemplate[0].DocumentNameEN : dtTbm_DocumentTemplate[0].DocumentNameLC;

        //                coverLetterData.DocumentNameEN = dtTbm_DocumentTemplate[0].DocumentNameEN;
        //                coverLetterData.DocumentNameLC = dtTbm_DocumentTemplate[0].DocumentNameLC;
        //            }
        //        }

        //        CommonUtil comUtil = new CommonUtil();
        //        if (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalContractBasic != null && sParam.RentalContractData.dtTbt_RentalContractBasic.Count > 0)
        //        {
        //            coverLetterData.ContractCode = sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContractCodeShort;
        //            coverLetterData.OCC = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].OCC;
        //            coverLetterData.OrderContractFee = CommonUtil.TextNumeric(sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFee);
        //            coverLetterData.NormalContractFee = CommonUtil.TextNumeric(sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].NormalContractFee);
        //            coverLetterData.OrderAdditionalDepositFee = CommonUtil.TextNumeric(sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee);
        //            coverLetterData.OrderContractFeePayMethod = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeePayMethod;
        //            coverLetterData.FireMonitorFlag = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].FireMonitorFlag;
        //            coverLetterData.CrimePreventFlag = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].CrimePreventFlag;
        //            coverLetterData.EmergencyReportFlag = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag;
        //            coverLetterData.FacilityMonitorFlag = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag;
        //            coverLetterData.ContractDurationMonth = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].ContractDurationMonth;
        //            coverLetterData.OldContractCode = comUtil.ConvertContractCode(sParam.RentalContractData.dtTbt_RentalContractBasic[0].OldContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

        //            coverLetterData.ProductCode = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].ProductCode;
        //            coverLetterData.PhoneLineTypeCode = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1;
        //            coverLetterData.PhoneLineOwnerTypeCode = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1;


        //            List<tbt_BillingBasic> dtTbt_BillingBasicNoStop = null;
        //            if (dtTbt_BillingBasic != null && dtTbt_BillingBasic.Count > 0)
        //            {
        //                dtTbt_BillingBasicNoStop = (from t in dtTbt_BillingBasic
        //                                            where t.MonthlyBillingAmount > 0
        //                                            && t.StopBillingFlag == FlagType.C_FLAG_OFF
        //                                            select t).ToList<tbt_BillingBasic>();
        //            }

        //            //After approve contract &　Before Complete 1st installation
        //            if (sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
        //                && sParam.RentalContractData.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == (bool)FlagType.C_FLAG_OFF)
        //            {
        //                coverLetterData.ContractFeePayMethod = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeePayMethod;

        //                if (dtTbt_BillingBasicNoStop != null && dtTbt_BillingBasicNoStop.Count == 1)
        //                {
        //                    coverLetterData.CreditTerm = dtTbt_BillingBasicNoStop[0].CreditTerm;
        //                    coverLetterData.BillingCycle = dtTbt_BillingBasicNoStop[0].BillingCycle;
        //                }
        //                else
        //                {
        //                    coverLetterData.CreditTerm = null;
        //                    coverLetterData.BillingCycle = null;
        //                }
        //            }
        //            //After complete 1st installation
        //            else if (sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
        //                    && sParam.RentalContractData.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == (bool)FlagType.C_FLAG_ON)
        //            {
        //                coverLetterData.ContractFeePayMethod = sParam.RentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeePayMethod;

        //                if (dtTbt_BillingBasicNoStop != null && dtTbt_BillingBasicNoStop.Count == 1)
        //                {
        //                    coverLetterData.CreditTerm = dtTbt_BillingBasicNoStop[0].CreditTerm;
        //                    coverLetterData.BillingCycle = dtTbt_BillingBasicNoStop[0].BillingCycle;
        //                }
        //                else
        //                {
        //                    coverLetterData.CreditTerm = null;
        //                    coverLetterData.BillingCycle = null;
        //                }
        //            }
        //            //After start service
        //            else if (sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
        //                    || sParam.RentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
        //            {
        //                if (dtTbt_BillingBasicNoStop != null)
        //                {
        //                    var billingBasicNoStopMinOCC = (from t in dtTbt_BillingBasicNoStop
        //                                                    group t by new
        //                                                    {
        //                                                        ContractCode = t.ContractCode,
        //                                                        PaymentMethod = t.PaymentMethod,
        //                                                        CreditTerm = t.CreditTerm,
        //                                                        BillingCycle = t.BillingCycle,
        //                                                    } into grp
        //                                                    select new
        //                                                    {
        //                                                        PaymentMethod = grp.Key.PaymentMethod,
        //                                                        CreditTerm = grp.Key.CreditTerm,
        //                                                        BillingCycle = grp.Key.BillingCycle,
        //                                                        MinOCC = grp.Min(o => o.BillingOCC)
        //                                                    });

        //                    foreach (var obj in billingBasicNoStopMinOCC)
        //                    {
        //                        coverLetterData.ContractFeePayMethod = obj.PaymentMethod;
        //                        coverLetterData.CreditTerm = obj.CreditTerm;
        //                        coverLetterData.BillingCycle = obj.BillingCycle;
        //                    }
        //                }

        //            }

        //        }
        //        else if (sParam.DraftRentalContractData != null && sParam.DraftRentalContractData.doTbt_DraftRentalContrat != null)
        //        {
        //            coverLetterData.ContractCode = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.QuotationTargetCodeShort;
        //            coverLetterData.OCC = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.Alphabet;

        //            coverLetterData.OrderContractFee = CommonUtil.TextNumeric(sParam.DraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee);
        //            coverLetterData.NormalContractFee = CommonUtil.TextNumeric(sParam.DraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFee);
        //            coverLetterData.OrderAdditionalDepositFee = CommonUtil.TextNumeric(sParam.DraftRentalContractData.doTbt_DraftRentalContrat.OrderDepositFee);
        //            coverLetterData.OrderContractFeePayMethod = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.PayMethod;
        //            coverLetterData.FireMonitorFlag = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.FireMonitorFlag;
        //            coverLetterData.CrimePreventFlag = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.CrimePreventFlag;
        //            coverLetterData.EmergencyReportFlag = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.EmergencyReportFlag;
        //            coverLetterData.FacilityMonitorFlag = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.FacilityMonitorFlag;
        //            coverLetterData.ContractDurationMonth = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.ContractDurationMonth;
        //            coverLetterData.OldContractCode = comUtil.ConvertContractCode(sParam.DraftRentalContractData.doTbt_DraftRentalContrat.OldContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

        //            coverLetterData.ProductCode = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.ProductCode;
        //            coverLetterData.PhoneLineTypeCode = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.PhoneLineTypeCode1;
        //            coverLetterData.PhoneLineOwnerTypeCode = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.PhoneLineOwnerTypeCode1;

        //            coverLetterData.CreditTerm = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.CreditTerm;
        //            coverLetterData.BillingCycle = sParam.DraftRentalContractData.doTbt_DraftRentalContrat.BillingCycle;
        //        }

        //        if (sParam.ContractTargetCustomerList != null && sParam.ContractTargetCustomerList.Count > 0)
        //        {
        //            coverLetterData.ContractTargetNameEN = sParam.ContractTargetCustomerList[0].CustFullNameEN;
        //            coverLetterData.ContractTargetNameLC = sParam.ContractTargetCustomerList[0].CustFullNameLC;

        //            coverLetterData.BusinessTypeNameEN = sParam.ContractTargetCustomerList[0].BusinessTypeNameEN;
        //            coverLetterData.BusinessTypeNameLC = sParam.ContractTargetCustomerList[0].BusinessTypeNameLC;
        //            coverLetterData.BusinessTypeNameJP = sParam.ContractTargetCustomerList[0].BusinessTypeNameJP;
        //        }

        //        if (sParam.SiteList != null && sParam.SiteList.Count > 0)
        //        {
        //            coverLetterData.SiteNameEN = sParam.SiteList[0].SiteNameEN;
        //            coverLetterData.SiteNameLC = sParam.SiteList[0].SiteNameLC;

        //            coverLetterData.BuildingUsageNameEN = sParam.SiteList[0].BuildingUsageNameEN;
        //            coverLetterData.BuildingUsageNameLC = sParam.SiteList[0].BuildingUsageNameLC;
        //            coverLetterData.BuildingUsageNameJP = sParam.SiteList[0].BuildingUsageNameJP;  
        //        }

        //        List<tbm_Product> productList = masHandler.GetTbm_Product(coverLetterData.ProductCode, null);
        //        if (productList != null && productList.Count > 0)
        //        {
        //            coverLetterData.ProductNameEN = productList[0].ProductNameEN;
        //            coverLetterData.ProductNameLC = productList[0].ProductNameLC;
        //            coverLetterData.ProductNameJP = productList[0].ProductNameJP;
        //        }

        //        List<string> strMiscList = new List<string>();
        //        strMiscList.Add(MiscType.C_PHONE_LINE_TYPE);
        //        strMiscList.Add(MiscType.C_PHONE_LINE_OWNER_TYPE);
        //        strMiscList.Add(MiscType.C_PAYMENT_METHOD);

        //        List<doMiscTypeCode> miscTypeList = comHandler.GetMiscTypeCodeListByFieldName(strMiscList);

        //        List<doMiscTypeCode> miscPhoneLineType = (from t in miscTypeList
        //                                                    where t.FieldName == MiscType.C_PHONE_LINE_TYPE
        //                                                    && t.ValueCode == coverLetterData.PhoneLineTypeCode
        //                                                    select t).ToList<doMiscTypeCode>();
        //        if (miscPhoneLineType != null && miscPhoneLineType.Count > 0)
        //        {
        //            coverLetterData.PhoneLineTypeNameEN = miscPhoneLineType[0].ValueDisplayEN;
        //            coverLetterData.PhoneLineTypeNameLC = miscPhoneLineType[0].ValueDisplayLC;
        //            coverLetterData.PhoneLineTypeNameJP = miscPhoneLineType[0].ValueDisplayJP;
        //        }

        //        List<doMiscTypeCode> miscPhoneLineOwnerType = (from t in miscTypeList
        //                                                        where t.FieldName == MiscType.C_PHONE_LINE_OWNER_TYPE
        //                                                        && t.ValueCode == coverLetterData.PhoneLineOwnerTypeCode
        //                                                        select t).ToList<doMiscTypeCode>();
        //        if (miscPhoneLineOwnerType != null && miscPhoneLineOwnerType.Count > 0)
        //        {
        //            coverLetterData.PhoneLineOwnerTypeNameEN = miscPhoneLineOwnerType[0].ValueDisplayEN;
        //            coverLetterData.PhoneLineOwnerTypeNameLC = miscPhoneLineOwnerType[0].ValueDisplayLC;
        //            coverLetterData.PhoneLineOwnerTypeNameJP = miscPhoneLineOwnerType[0].ValueDisplayJP;
        //        }

        //        if (String.IsNullOrEmpty(coverLetterData.OrderContractFeePayMethod) == false)
        //        {
        //            List<doMiscTypeCode> miscPayMethod = (from t in miscTypeList
        //                                                    where t.FieldName == MiscType.C_PAYMENT_METHOD
        //                                                    && t.ValueCode == coverLetterData.OrderContractFeePayMethod
        //                                                    select t).ToList<doMiscTypeCode>();
        //            if (miscPayMethod != null && miscPayMethod.Count > 0)
        //            {
        //                coverLetterData.OrderContractFeePayMethodNameEN = miscPayMethod[0].ValueDisplayEN;
        //                coverLetterData.OrderContractFeePayMethodNameLC = miscPayMethod[0].ValueDisplayLC;
        //                coverLetterData.OrderContractFeePayMethodNameJP = miscPayMethod[0].ValueDisplayJP;
        //            }
        //        }

        //        if (sParam.doRentalContractBasicData != null)
        //        {
        //            coverLetterData.OperationOfficeName = sParam.doRentalContractBasicData.OperationOfficeName;
        //        }
        //        else if (sParam.doDraftRentalContractData != null)
        //        {
        //            coverLetterData.OperationOfficeName = sParam.doDraftRentalContractData.OperationOfficeName;
        //        }

        //        CommonUtil.MappingObjectLanguage(coverLetterData);
        //        res.ResultData = coverLetterData;

        //        sParam.DocumentTemplateCondition = cond;
        //        sParam.CoverLetterData = coverLetterData;
        //        sParam.BillingBasicData = dtTbt_BillingBasic;
        //        UpdateScreenObject(sParam);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}

        //public ActionResult CTS160_GetInstrumentListData()
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    List<CTS160_InstrumentGridData> gridData = null;

        //    try
        //    {
        //        CTS160_ScreenParameter sParam = GetScreenObject<CTS160_ScreenParameter>();
        //        if (sParam.RentalContractData != null && sParam.RentalContractData.dtTbt_RentalInstrumentDetails != null)
        //        {
        //            gridData = CommonUtil.ClonsObjectList<tbt_RentalInstrumentDetails, CTS160_InstrumentGridData>(sParam.RentalContractData.dtTbt_RentalInstrumentDetails);
        //        }
        //        else if (sParam.DraftRentalContractData != null && sParam.DraftRentalContractData.doTbt_DraftRentalInstrument != null)
        //        {
        //            gridData = CommonUtil.ClonsObjectList<tbt_DraftRentalInstrument, CTS160_InstrumentGridData>(sParam.DraftRentalContractData.doTbt_DraftRentalInstrument);
        //        }

        //        res.ResultData = CommonUtil.ConvertToXml<CTS160_InstrumentGridData>(gridData, "Contract\\CTS160", CommonUtil.GRID_EMPTY_TYPE.VIEW);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}

        //public ActionResult CTS160_RegisterGenerateDocument()
        //{
        //    ObjectResultData res = new ObjectResultData();

        //    try
        //    {
        //        //CheckSystemStatus
        //        if (CheckIsSuspending(res) == true)
        //            return Json(res);

        //        //Check screen permission
        //        if (CheckUserPermission(ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
        //            return Json(res);
        //        }

        //        res.ResultData = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}

        //public ActionResult CTS160_ConfirmGenerateDocument(CTS160_CoverLetterData confirmData)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    //IBillingHandler billHandler;
        //    IContractDocumentHandler contractDocHandler;
        //    IOfficeMasterHandler officeHandler;
        //    ICommonHandler comHandler;
        //    IRentralContractHandler rentContHandler;

        //    dsRentalContractData dsRentalContract;
        //    doDraftRentalContractData doDraftRentalContract;
        //    List<tbt_BillingBasic> dtTbt_BillingBasic;
        //    List<tbm_Office> tbm_OfficeList;

        //    dsContractDocData dsContractDocDataData = null;
        //    tbt_ContractDocument dtTbt_ContractDocument = new tbt_ContractDocument();
        //    tbt_DocContractReport dtTbt_DocContractReport = new tbt_DocContractReport();
        //    tbt_DocChangeMemo dtTbt_DocChangeMemo = new tbt_DocChangeMemo();
        //    tbt_DocChangeNotice dtTbt_DocChangeNotice = new tbt_DocChangeNotice();
        //    tbt_DocConfirmCurrentInstrumentMemo dtTbt_DocConfirmCurrentInstrumentMemo = new tbt_DocConfirmCurrentInstrumentMemo();
        //    tbt_DocCancelContractMemo dtTbt_DocCancelContractMemo = new tbt_DocCancelContractMemo();
        //    List<tbt_DocCancelContractMemoDetail> dtTbt_DocCancelContractMemoDetail = new List<tbt_DocCancelContractMemoDetail>();
        //    tbt_DocChangeFeeMemo dtTbt_DocChangeFeeMemo = new tbt_DocChangeFeeMemo();

        //    List<dtTbt_BillingTempListForView> dtTbt_BillingTempListAllOCC = null;
        //    List<dtTbt_BillingTempListForView> dtTbt_BillingTempListFirstOCC = null;
        //    List<dtTbt_BillingTempListForView> dtTbt_BillingTempListLastOCC = null;
        //    List<dtTbt_BillingTempListForView> dtTbt_BillingTempListDepositFee = null;

        //    try
        //    {
        //        //CheckSystemStatus
        //        if (CheckIsSuspending(res) == true)
        //            return Json(res);

        //        //Check screen permission
        //        if (CheckUserPermission(ScreenID.C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
        //            return Json(res);
        //        }


        //        CTS160_ScreenParameter sParam = GetScreenObject<CTS160_ScreenParameter>();
        //        dsRentalContract = sParam.RentalContractData;
        //        doDraftRentalContract = sParam.DraftRentalContractData;

        //        //RegisterContractDocument
        //        using (TransactionScope scop = new TransactionScope())
        //        {
        //            //Create contract document
        //            tbt_ContractDocument dtTbt_ContractDocumentTemp = new tbt_ContractDocument();
        //            if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
        //            {
        //                dtTbt_ContractDocumentTemp.ContractCode = sParam.ContractCode;

        //                if (sParam.DocumentTemplateCondition.ContractFlag == true)
        //                {
        //                    if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
        //                    {
        //                        dtTbt_ContractDocumentTemp.OCC = ParticularOCC.C_PARTICULAR_OCC_CONTRACT_REP_BEF_START;                              
        //                    }
        //                    else
        //                    {
        //                        dtTbt_ContractDocumentTemp.OCC = sParam.OCCAlphabet;
        //                    }

        //                }
        //                else if (sParam.DocumentTemplateCondition.CoverLetterFlag == true)
        //                {
        //                    dtTbt_ContractDocumentTemp.OCC = ParticularOCC.C_PARTICULAR_OCC_COVER_LETTER;
        //                }
        //                else if (sParam.DocumentTemplateCondition.MemorandumFlag == true || sParam.DocumentTemplateCondition.NoticeFlag == true)
        //                {
        //                    dtTbt_ContractDocumentTemp.OCC = sParam.OCCAlphabet;
        //                }
        //                else
        //                {
        //                    dtTbt_ContractDocumentTemp.OCC = null;
        //                }
        //            }
        //            else if (doDraftRentalContract != null && doDraftRentalContract.doTbt_DraftRentalContrat != null)
        //            {
        //                dtTbt_ContractDocumentTemp.QuotationTargetCode = sParam.QuotationTargetCode;
        //                dtTbt_ContractDocumentTemp.Alphabet = sParam.OCCAlphabet;
        //            }

        //            dtTbt_ContractDocumentTemp.DocumentCode = sParam.CoverLetterData.DocumentCode;
        //            dtTbt_ContractDocumentTemp.CoverLetterDocCode = sParam.DocumentTemplateCondition.CoverLetterFlag == true ? sParam.DocumentTemplateCondition.DocumentName : null;

        //            //Get tbt_BillingBasic data
        //            //billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
        //            //dtTbt_BillingBasic = billHandler.GetTbt_BillingBasic(sParam.ContractCode, null);
        //            dtTbt_BillingBasic = sParam.BillingBasicData;

        //            //Generate contract document occurrence
        //            string strContractCode = string.Empty;
        //            string strOCC = string.Empty;

        //            if (String.IsNullOrEmpty(dtTbt_ContractDocumentTemp.ContractCode) == false)
        //            {
        //                strContractCode = dtTbt_ContractDocumentTemp.ContractCode;
        //                strOCC = dtTbt_ContractDocumentTemp.OCC;
        //            }
        //            else
        //            {
        //                strContractCode = dtTbt_ContractDocumentTemp.QuotationTargetCode;
        //                strOCC = dtTbt_ContractDocumentTemp.Alphabet;
        //            }

        //            contractDocHandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
        //            string strContractDocOCC = contractDocHandler.GenerateDocOCC(strContractCode, strOCC);

        //            /*--- Save contract document data ---*/
        //            dsContractDocDataData = new dsContractDocData();

        //            if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
        //            {
        //                dtTbt_BillingTempListAllOCC = CommonUtil.ClonsObjectList<dtTbt_BillingTempListForView, dtTbt_BillingTempListForView>(sParam.BillingTempListForView);

        //                if (dtTbt_BillingTempListAllOCC != null)
        //                { 
        //                    dtTbt_BillingTempListFirstOCC = (from t in dtTbt_BillingTempListAllOCC
        //                                                     where t.OCC == OCCType.C_FIRST_UNIMPLEMENTED_SECURITY_OCC
        //                                                     select t).ToList<dtTbt_BillingTempListForView>();

        //                    dtTbt_BillingTempListLastOCC = (from t in dtTbt_BillingTempListAllOCC
        //                                                    where t.OCC == dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC
        //                                                    select t).ToList<dtTbt_BillingTempListForView>();

        //                    dtTbt_BillingTempListDepositFee = (from t in dtTbt_BillingTempListAllOCC
        //                                                           where t.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE
        //                                                           select t).ToList<dtTbt_BillingTempListForView>();                        
        //                }
        //            }

        //            //Create contract document data object
        //            dtTbt_ContractDocument = CommonUtil.CloneObject<tbt_ContractDocument, tbt_ContractDocument>(dtTbt_ContractDocumentTemp);
        //            //dtTbt_ContractDocument.DocID = null; => Auto-run
        //            dtTbt_ContractDocument.ContractDocOCC = strContractDocOCC;
        //            dtTbt_ContractDocument.DocNo = String.IsNullOrEmpty(dtTbt_ContractDocument.ContractCode) == false ?  
        //                                            String.Format("{0}-{1}-{2}", dtTbt_ContractDocument.ContractCode, dtTbt_ContractDocument.OCC, strContractDocOCC)
        //                                            : String.Format("{0}-{1}-{2}", dtTbt_ContractDocument.QuotationTargetCode, dtTbt_ContractDocument.Alphabet, strContractDocOCC);

        //            dtTbt_ContractDocument.SECOMSignatureFlag =	sParam.DocumentTemplateCondition.ShowSignature;

        //            if (sParam.ContractTargetCustomerList != null && sParam.ContractTargetCustomerList.Count > 0)
        //            {
        //                dtTbt_ContractDocument.ContractTargetNameLC = sParam.ContractTargetCustomerList[0].CustFullNameLC;
        //                dtTbt_ContractDocument.ContractTargetNameEN = sParam.ContractTargetCustomerList[0].CustFullNameEN;

        //                dtTbt_ContractDocument.ContractTargetAddressLC = sParam.ContractTargetCustomerList[0].AddressFullLC;
        //                dtTbt_ContractDocument.ContractTargetAddressEN = sParam.ContractTargetCustomerList[0].AddressFullEN;

        //                dtTbt_ContractDocument.BusinessTypeCode = sParam.ContractTargetCustomerList[0].BusinessTypeCode;
        //                dtTbt_ContractDocument.BusinessTypeNameEN = sParam.ContractTargetCustomerList[0].BusinessTypeNameEN;
        //                dtTbt_ContractDocument.BusinessTypeNameLC = sParam.ContractTargetCustomerList[0].BusinessTypeNameLC;
        //            }

        //            if (sParam.RealCustomerList != null && sParam.RealCustomerList.Count > 0)
        //            {
        //                dtTbt_ContractDocument.RealCustomerNameLC = sParam.RealCustomerList[0].CustFullNameLC;
        //                dtTbt_ContractDocument.RealCustomerNameEN = sParam.RealCustomerList[0].CustFullNameEN;
        //            }

        //            if (sParam.SiteList != null && sParam.SiteList.Count > 0)
        //            {
        //                dtTbt_ContractDocument.SiteNameLC = sParam.SiteList[0].SiteNameLC;       
        //                dtTbt_ContractDocument.SiteNameEN = sParam.SiteList[0].SiteNameEN;

        //                dtTbt_ContractDocument.SiteAddressLC = sParam.SiteList[0].AddressFullLC;
        //                dtTbt_ContractDocument.SiteAddressEN = sParam.SiteList[0].AddressFullEN;

        //                dtTbt_ContractDocument.BuildingUsageCode = sParam.SiteList[0].BuildingUsageCode;
        //                dtTbt_ContractDocument.BuildingUsageNameEN = sParam.SiteList[0].BuildingUsageNameEN;
        //                dtTbt_ContractDocument.BuildingUsageNameLC = sParam.SiteList[0].BuildingUsageNameLC;
        //            }

        //            if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
        //            {
        //                dtTbt_ContractDocument.ContractTargetCustCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractTargetCustCode;
        //                dtTbt_ContractDocument.RealCustomerCustCode = dsRentalContract.dtTbt_RentalContractBasic[0].RealCustomerCustCode;
        //                dtTbt_ContractDocument.ContractOfficeCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractOfficeCode;
        //                dtTbt_ContractDocument.OperationOfficeCode = dsRentalContract.dtTbt_RentalContractBasic[0].OperationOfficeCode;

        //                dtTbt_ContractDocument.NegotiationStaffEmpNo = dsRentalContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1 != null ?
        //                                                                dsRentalContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1
        //                                                                : dsRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1;

        //                dtTbt_ContractDocument.QuotationFee = dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalContractFee;
        //                dtTbt_ContractDocument.ProductCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].ProductCode;
        //                dtTbt_ContractDocument.PhoneLineTypeCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1;
        //                dtTbt_ContractDocument.PhoneLineOwnerTypeCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1;

        //                dtTbt_ContractDocument.FireSecurityFlag = dsRentalContract.dtTbt_RentalSecurityBasic[0].FireMonitorFlag;
        //                dtTbt_ContractDocument.CrimePreventFlag = dsRentalContract.dtTbt_RentalSecurityBasic[0].CrimePreventFlag;
        //                dtTbt_ContractDocument.EmergencyReportFlag = dsRentalContract.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag;
        //                dtTbt_ContractDocument.FacilityMonitorFlag = dsRentalContract.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag;

        //                dtTbt_ContractDocument.ContractDurationMonth = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractDurationMonth;
        //                dtTbt_ContractDocument.OldContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].OldContractCode;

        //                //Set payment information

        //                //Move to set data at CTS160_SelectData()
        //                //List<tbt_BillingBasic> dtTbt_BillingBasicNoStop = null;
        //                //if (dtTbt_BillingBasic != null && dtTbt_BillingBasic.Count > 0)
        //                //{
        //                //    dtTbt_BillingBasicNoStop = (from t in dtTbt_BillingBasic
        //                //                                where t.MonthlyBillingAmount > 0
        //                //                                && t.StopBillingFlag == FlagType.C_FLAG_OFF
        //                //                                select t).ToList<tbt_BillingBasic>();
        //                //}

        //                decimal? decBillingAmtDepositFee = null;
        //                if (dtTbt_BillingTempListDepositFee != null && dtTbt_BillingTempListDepositFee.Count > 0)
        //                {
        //                    var sumBillingAmt = (from t in dtTbt_BillingTempListDepositFee
        //                                         group t by new { ContractCode = t.ContractCode } into grp
        //                                         select new
        //                                         {
        //                                             ContractCode = grp.Key.ContractCode,
        //                                             SumBillingAmt = grp.Sum(o => o.BillingAmt)
        //                                         });

        //                    foreach (var obj in sumBillingAmt)
        //                    {
        //                        decBillingAmtDepositFee = obj.SumBillingAmt;
        //                    }
        //                }

        //                dtTbt_ContractDocument.ContractFee = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee;
        //                dtTbt_ContractDocument.DepositFee = decBillingAmtDepositFee;

        //                //Move to set data at CTS160_SelectData()
        //                ////After approve contract &　Before Complete 1st installation
        //                //if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
        //                //    && dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == (bool)FlagType.C_FLAG_OFF)
        //                //{
        //                //    dtTbt_ContractDocument.ContractFeePayMethod = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeePayMethod;

        //                //    if (dtTbt_BillingBasicNoStop != null && dtTbt_BillingBasicNoStop.Count == 1)
        //                //    {
        //                //        dtTbt_ContractDocument.CreditTerm = dtTbt_BillingBasicNoStop[0].CreditTerm;
        //                //        dtTbt_ContractDocument.PaymentCycle = dtTbt_BillingBasicNoStop[0].BillingCycle;
        //                //    }
        //                //    else
        //                //    {
        //                //        dtTbt_ContractDocument.CreditTerm = null;
        //                //        dtTbt_ContractDocument.PaymentCycle = null;
        //                //    }
        //                //}
        //                ////After complete 1st installation
        //                //else if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
        //                //        && dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == (bool)FlagType.C_FLAG_ON)
        //                //{
        //                //    dtTbt_ContractDocument.ContractFeePayMethod = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeePayMethod;

        //                //    if (dtTbt_BillingBasicNoStop != null && dtTbt_BillingBasicNoStop.Count == 1)
        //                //    {
        //                //        dtTbt_ContractDocument.CreditTerm = dtTbt_BillingBasicNoStop[0].CreditTerm;
        //                //        dtTbt_ContractDocument.PaymentCycle = dtTbt_BillingBasicNoStop[0].BillingCycle;
        //                //    }
        //                //    else
        //                //    {
        //                //        dtTbt_ContractDocument.CreditTerm = null;
        //                //        dtTbt_ContractDocument.PaymentCycle = null;
        //                //    }
        //                //}
        //                ////After start service
        //                //else if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
        //                //        || dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
        //                //{
        //                //    if (dtTbt_BillingBasicNoStop != null)
        //                //    {
        //                //        var billingBasicNoStopMinOCC = (from t in dtTbt_BillingBasicNoStop
        //                //                                        group t by new
        //                //                                        {
        //                //                                            ContractCode = t.ContractCode,
        //                //                                            PaymentMethod = t.PaymentMethod,
        //                //                                            CreditTerm = t.CreditTerm,
        //                //                                            BillingCycle = t.BillingCycle,
        //                //                                        } into grp
        //                //                                        select new
        //                //                                        {
        //                //                                            PaymentMethod = grp.Key.PaymentMethod,
        //                //                                            CreditTerm = grp.Key.CreditTerm,
        //                //                                            BillingCycle = grp.Key.BillingCycle,
        //                //                                            MinOCC = grp.Min(o => o.BillingOCC)
        //                //                                        });

        //                //        foreach (var obj in billingBasicNoStopMinOCC)
        //                //        {
        //                //            dtTbt_ContractDocument.ContractFeePayMethod = obj.PaymentMethod;
        //                //            dtTbt_ContractDocument.CreditTerm = obj.CreditTerm;
        //                //            dtTbt_ContractDocument.PaymentCycle = obj.BillingCycle;
        //                //        }
        //                //    }

        //                //}

        //                dtTbt_ContractDocument.ContractFeePayMethod = sParam.CoverLetterData.ContractFeePayMethod;
        //                dtTbt_ContractDocument.CreditTerm = sParam.CoverLetterData.CreditTerm;
        //                dtTbt_ContractDocument.PaymentCycle = sParam.CoverLetterData.BillingCycle;

        //                //End payment information
        //            }
        //            else if (doDraftRentalContract != null && doDraftRentalContract.doTbt_DraftRentalContrat != null)
        //            {
        //                dtTbt_ContractDocument.ContractTargetCustCode = doDraftRentalContract.doTbt_DraftRentalContrat.ContractTargetCustCode;
        //                dtTbt_ContractDocument.RealCustomerCustCode = doDraftRentalContract.doTbt_DraftRentalContrat.RealCustomerCustCode;
        //                dtTbt_ContractDocument.ContractOfficeCode = doDraftRentalContract.doTbt_DraftRentalContrat.ContractOfficeCode;
        //                dtTbt_ContractDocument.OperationOfficeCode = doDraftRentalContract.doTbt_DraftRentalContrat.OperationOfficeCode;

        //                dtTbt_ContractDocument.NegotiationStaffEmpNo = doDraftRentalContract.doTbt_DraftRentalContrat.SalesmanEmpNo1;
        //                dtTbt_ContractDocument.QuotationFee	= doDraftRentalContract.doTbt_DraftRentalContrat.NormalContractFee;
        //                dtTbt_ContractDocument.ProductCode = doDraftRentalContract.doTbt_DraftRentalContrat.ProductCode;
        //                dtTbt_ContractDocument.PhoneLineTypeCode = doDraftRentalContract.doTbt_DraftRentalContrat.PhoneLineTypeCode1;
        //                dtTbt_ContractDocument.PhoneLineOwnerTypeCode = doDraftRentalContract.doTbt_DraftRentalContrat.PhoneLineOwnerTypeCode1;

        //                dtTbt_ContractDocument.FireSecurityFlag = doDraftRentalContract.doTbt_DraftRentalContrat.FireMonitorFlag;
        //                dtTbt_ContractDocument.CrimePreventFlag = doDraftRentalContract.doTbt_DraftRentalContrat.CrimePreventFlag;
        //                dtTbt_ContractDocument.EmergencyReportFlag = doDraftRentalContract.doTbt_DraftRentalContrat.EmergencyReportFlag;
        //                dtTbt_ContractDocument.FacilityMonitorFlag = doDraftRentalContract.doTbt_DraftRentalContrat.FacilityMonitorFlag;

        //                dtTbt_ContractDocument.ContractDurationMonth = doDraftRentalContract.doTbt_DraftRentalContrat.ContractDurationMonth;
        //                dtTbt_ContractDocument.OldContractCode = doDraftRentalContract.doTbt_DraftRentalContrat.OldContractCode;

        //                //Set payment information
        //                dtTbt_ContractDocument.ContractFee = doDraftRentalContract.doTbt_DraftRentalContrat.OrderContractFee;
        //                dtTbt_ContractDocument.ContractFeePayMethod = doDraftRentalContract.doTbt_DraftRentalContrat.PayMethod;
        //                dtTbt_ContractDocument.CreditTerm = doDraftRentalContract.doTbt_DraftRentalContrat.CreditTerm;
        //                dtTbt_ContractDocument.PaymentCycle = doDraftRentalContract.doTbt_DraftRentalContrat.BillingCycle;
        //                dtTbt_ContractDocument.DepositFee = doDraftRentalContract.doTbt_DraftRentalContrat.OrderDepositFee;
        //                //End payment information
        //            }

        //            officeHandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
        //            tbm_OfficeList = officeHandler.GetTbm_Office();
        //            if (tbm_OfficeList != null)
        //            {
        //                List<tbm_Office> operationOfficeList = (from t in tbm_OfficeList
        //                                                        where t.OfficeCode == dtTbt_ContractDocument.OperationOfficeCode
        //                                                        select t).ToList<tbm_Office>();

        //                dtTbt_ContractDocument.OperationOfficeNameEN = operationOfficeList[0].OfficeNameEN;
        //                dtTbt_ContractDocument.OperationOfficeNameLC = operationOfficeList[0].OfficeNameLC;
        //            }

        //            dtTbt_ContractDocument.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_NOT_ISSUED;
        //            dtTbt_ContractDocument.DocAuditResult = DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED;
        //            dtTbt_ContractDocument.CollectDocDate = null;

        //            dtTbt_ContractDocument.SubjectEN = sParam.CoverLetterData.DocumentNameEN;
        //            dtTbt_ContractDocument.SubjectLC = sParam.CoverLetterData.DocumentNameLC;

        //            dtTbt_ContractDocument.RelatedNo1 = confirmData.RelatedNo1;
        //            dtTbt_ContractDocument.RelatedNo2 = confirmData.RelatedNo2;
        //            dtTbt_ContractDocument.AttachDoc1 = confirmData.AttachDoc1;
        //            dtTbt_ContractDocument.AttachDoc2 = confirmData.AttachDoc2;
        //            dtTbt_ContractDocument.AttachDoc3 = confirmData.AttachDoc3;
        //            dtTbt_ContractDocument.AttachDoc4 = confirmData.AttachDoc4;
        //            dtTbt_ContractDocument.AttachDoc5 = confirmData.AttachDoc5;
        //            dtTbt_ContractDocument.ApproveNo1 = confirmData.ApproveNo1;
        //            dtTbt_ContractDocument.ApproveNo2 = confirmData.ApproveNo2;
        //            dtTbt_ContractDocument.ApproveNo3 = confirmData.ApproveNo3;
        //            dtTbt_ContractDocument.ContactMemo = confirmData.ContactMemo;

        //            dtTbt_ContractDocument.CreateOfficeCode = CommonUtil.dsTransData.dtUserData.MainOfficeCode;
        //            if (tbm_OfficeList != null)
        //            {
        //                List<tbm_Office> createOfficeList = (from t in tbm_OfficeList
        //                                                     where t.OfficeCode == dtTbt_ContractDocument.CreateOfficeCode
        //                                                     select t).ToList<tbm_Office>();
        //                if (createOfficeList != null && createOfficeList.Count > 0)
        //                {
        //                    dtTbt_ContractDocument.CreateOfficeNameEN = createOfficeList[0].OfficeNameEN;
        //                    dtTbt_ContractDocument.CreateOfficeNameLC = createOfficeList[0].OfficeNameLC;
        //                }
        //            }

        //            dtTbt_ContractDocument.ProductNameEN = sParam.CoverLetterData.ProductNameEN;
        //            dtTbt_ContractDocument.ProductNameLC = sParam.CoverLetterData.ProductNameLC;

        //            dtTbt_ContractDocument.PhoneLineTypeNameEN = sParam.CoverLetterData.PhoneLineTypeNameEN;
        //            dtTbt_ContractDocument.PhoneLineTypeNameLC = sParam.CoverLetterData.PhoneLineTypeNameLC;

        //            dtTbt_ContractDocument.PhoneLineOwnerTypeNameEN = sParam.CoverLetterData.PhoneLineOwnerTypeNameEN;
        //            dtTbt_ContractDocument.PhoneLineOwnerTypeNameLC = sParam.CoverLetterData.PhoneLineOwnerTypeNameLC;

        //            dtTbt_ContractDocument.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //            dtTbt_ContractDocument.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //            dtTbt_ContractDocument.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //            dtTbt_ContractDocument.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

        //            dsContractDocDataData.dtTbt_ContractDocument = new List<tbt_ContractDocument>();
        //            dsContractDocDataData.dtTbt_ContractDocument.Add(dtTbt_ContractDocument);
        //            //End dtTbt_ContractDocument


        //            List<string> strMiscList = new List<string>();
        //            strMiscList.Add(MiscType.C_PROC_AFT_COUNTER_BALANCE_TYPE);
        //            strMiscList.Add(MiscType.C_CONTRACT_BILLING_TYPE);
        //            strMiscList.Add(MiscType.C_HANDLING_TYPE);
        //            strMiscList.Add(MiscType.C_DISPATCH_TYPE);

        //            comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //            List<doMiscTypeCode> miscTypeList = comHandler.GetMiscTypeCodeListByFieldName(strMiscList);


        //            //Set data in dtTbt_DocContractReport 
        //            if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN
        //                || dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH)
        //            {
        //                //dtTbt_DocContractReport.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
        //                if (dsRentalContract != null && dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
        //                {
        //                    dtTbt_DocContractReport.PlanCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanCode;

        //                    //dtTbt_DocContractReport.DispatchType = dsRentalContract.dtTbt_RentalSecurityBasic[0].DispatchTypeCode;
        //                    if (miscTypeList != null)
        //                    {
        //                        List<doMiscTypeCode> miscDispatchType = (from t in miscTypeList
        //                                                                 where t.FieldName == MiscType.C_DISPATCH_TYPE
        //                                                                 && t.ValueCode == dsRentalContract.dtTbt_RentalSecurityBasic[0].DispatchTypeCode
        //                                                                 select t).ToList<doMiscTypeCode>();
        //                        if (miscDispatchType != null && miscDispatchType.Count > 0)
        //                        {
        //                            dtTbt_DocContractReport.DispatchType = dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN ? miscDispatchType[0].ValueDisplayEN : miscDispatchType[0].ValueDisplayLC;
        //                        }
        //                    }

        //                    dtTbt_DocContractReport.OperationDate = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractStartDate;
        //                    dtTbt_DocContractReport.AutoRenewMonth = dsRentalContract.dtTbt_RentalSecurityBasic[0].AutoRenewMonth;
        //                    dtTbt_DocContractReport.NegotiationTotalInstallFee = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee;

        //                    //Set payment information
        //                    decimal? decBillingAmtInstallFeeAprv = null;
        //                    List<dtTbt_BillingTempListForView> dtTbt_BillingTempListInstallFeeAprv = null;
        //                    if (dtTbt_BillingTempListFirstOCC != null)
        //                    {
        //                        dtTbt_BillingTempListInstallFeeAprv = (from t in dtTbt_BillingTempListFirstOCC
        //                                                               where t.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
        //                                                                    && t.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT
        //                                                               select t).ToList<dtTbt_BillingTempListForView>();

        //                        if (dtTbt_BillingTempListInstallFeeAprv != null && dtTbt_BillingTempListInstallFeeAprv.Count > 0)
        //                        {
        //                            var sumBillingAmt = (from t in dtTbt_BillingTempListInstallFeeAprv
        //                                                 group t by new { ContractCode = t.ContractCode } into grp
        //                                                 select new
        //                                                 {
        //                                                     ContractCode = grp.Key.ContractCode,
        //                                                     SumBillingAmt = grp.Sum(o => o.BillingAmt)
        //                                                 });

        //                            foreach (var obj in sumBillingAmt)
        //                            {
        //                                decBillingAmtInstallFeeAprv = obj.SumBillingAmt;
        //                            }
        //                        }
        //                    }

        //                    decimal? decBillingAmtInstallFeeComp = null;
        //                    List<dtTbt_BillingTempListForView> dtTbt_BillingTempListInstallFeeComp = null;
        //                    if (dtTbt_BillingTempListFirstOCC != null)
        //                    {
        //                        dtTbt_BillingTempListInstallFeeComp = (from t in dtTbt_BillingTempListFirstOCC
        //                                                               where t.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
        //                                                                    && t.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION
        //                                                               select t).ToList<dtTbt_BillingTempListForView>();

        //                        if (dtTbt_BillingTempListInstallFeeComp != null && dtTbt_BillingTempListInstallFeeComp.Count > 0)
        //                        {
        //                            var sumBillingAmt = (from t in dtTbt_BillingTempListInstallFeeComp
        //                                                 group t by new { ContractCode = t.ContractCode } into grp
        //                                                 select new
        //                                                 {
        //                                                     ContractCode = grp.Key.ContractCode,
        //                                                     SumBillingAmt = grp.Sum(o => o.BillingAmt)
        //                                                 });

        //                            foreach (var obj in sumBillingAmt)
        //                            {
        //                                decBillingAmtInstallFeeComp = obj.SumBillingAmt;
        //                            }
        //                        }
        //                    }

        //                    decimal? decBillingAmtInstallFeeServ = null;
        //                    List<dtTbt_BillingTempListForView> dtTbt_BillingTempListInstallFeeServ = null;
        //                    if (dtTbt_BillingTempListFirstOCC != null)
        //                    {
        //                        dtTbt_BillingTempListInstallFeeServ = (from t in dtTbt_BillingTempListFirstOCC
        //                                                               where t.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
        //                                                                    && t.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE
        //                                                               select t).ToList<dtTbt_BillingTempListForView>();

        //                        if (dtTbt_BillingTempListInstallFeeServ != null && dtTbt_BillingTempListInstallFeeServ.Count > 0)
        //                        {
        //                            var sumBillingAmt = (from t in dtTbt_BillingTempListInstallFeeServ
        //                                                 group t by new { ContractCode = t.ContractCode } into grp
        //                                                 select new
        //                                                 {
        //                                                     ContractCode = grp.Key.ContractCode,
        //                                                     SumBillingAmt = grp.Sum(o => o.BillingAmt)
        //                                                 });

        //                            foreach (var obj in sumBillingAmt)
        //                            {
        //                                decBillingAmtInstallFeeServ = obj.SumBillingAmt;
        //                            }
        //                        }
        //                    }

        //                    //After approve contract &　Before Complete 1st installation
        //                    if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
        //                        && dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == (bool)FlagType.C_FLAG_OFF)
        //                    {
        //                        if (dtTbt_BillingTempListDepositFee != null && dtTbt_BillingTempListDepositFee.Count == 1)
        //                        {
        //                            dtTbt_DocContractReport.DepositFeePayMethod = dtTbt_BillingTempListDepositFee[0].PayMethod;
        //                        }
        //                        else
        //                        {
        //                            dtTbt_DocContractReport.DepositFeePayMethod = null;
        //                        }

        //                        dtTbt_DocContractReport.DepositFeePhase = dsRentalContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming;

        //                        dtTbt_DocContractReport.InstallFee_ApproveContract = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract;
        //                        if (dtTbt_BillingTempListInstallFeeAprv != null && dtTbt_BillingTempListInstallFeeAprv.Count == 1)
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract = dtTbt_BillingTempListInstallFeeAprv[0].PayMethod;
        //                        }
        //                        else
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract = null;
        //                        }

        //                        dtTbt_DocContractReport.InstallFee_CompleteInstall = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall;
        //                        if (dtTbt_BillingTempListInstallFeeComp != null && dtTbt_BillingTempListInstallFeeComp.Count == 1)
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall = dtTbt_BillingTempListInstallFeeComp[0].PayMethod;
        //                        }
        //                        else
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall = null;
        //                        }

        //                        dtTbt_DocContractReport.InstallFee_StartService = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService;
        //                        if (dtTbt_BillingTempListInstallFeeServ != null && dtTbt_BillingTempListInstallFeeServ.Count == 1)
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_StartService = dtTbt_BillingTempListInstallFeeServ[0].PayMethod;
        //                        }
        //                        else
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_StartService = null;
        //                        }
        //                    }
        //                    //After complete 1st installation
        //                    else if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
        //                            && dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == (bool)FlagType.C_FLAG_ON)
        //                    {
        //                        dtTbt_DocContractReport.DepositFeePayMethod = null;
        //                        dtTbt_DocContractReport.DepositFeePhase = null;

        //                        dtTbt_DocContractReport.InstallFee_ApproveContract = decBillingAmtInstallFeeAprv;
        //                        if (dtTbt_BillingTempListInstallFeeAprv != null && dtTbt_BillingTempListInstallFeeAprv.Count == 1)
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract = dtTbt_BillingTempListInstallFeeAprv[0].PayMethod;
        //                        }
        //                        else
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract = null;
        //                        }

        //                        dtTbt_DocContractReport.InstallFee_CompleteInstall = decBillingAmtInstallFeeComp;
        //                        if (dtTbt_BillingTempListInstallFeeComp != null && dtTbt_BillingTempListInstallFeeComp.Count == 1)
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall = dtTbt_BillingTempListInstallFeeComp[0].PayMethod;
        //                        }
        //                        else
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall = null;
        //                        }

        //                        dtTbt_DocContractReport.InstallFee_StartService = decBillingAmtInstallFeeServ;
        //                        if (dtTbt_BillingTempListInstallFeeServ != null && dtTbt_BillingTempListInstallFeeServ.Count == 1)
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_StartService = dtTbt_BillingTempListInstallFeeServ[0].PayMethod;
        //                        }
        //                        else
        //                        {
        //                            dtTbt_DocContractReport.InstallFeePayMethod_StartService = null;
        //                        }
        //                    }
        //                    //After start service
        //                    else if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
        //                            || dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
        //                    {
        //                        dtTbt_DocContractReport.DepositFeePayMethod = null;
        //                        dtTbt_DocContractReport.DepositFeePhase = null;
        //                        dtTbt_DocContractReport.InstallFee_ApproveContract = null;
        //                        dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract = null;
        //                        dtTbt_DocContractReport.InstallFee_CompleteInstall = null;
        //                        dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall = null;
        //                        dtTbt_DocContractReport.InstallFee_StartService = null;
        //                        dtTbt_DocContractReport.InstallFeePayMethod_StartService = null;
        //                    }
        //                    //End payment information
        //                }
        //                else if (doDraftRentalContract != null && doDraftRentalContract.doTbt_DraftRentalContrat != null)
        //                {
        //                    dtTbt_DocContractReport.PlanCode = doDraftRentalContract.doTbt_DraftRentalContrat.PlanCode;

        //                    //dtTbt_DocContractReport.DispatchType = doDraftRentalContract.doTbt_DraftRentalContrat.DispatchTypeCode;
        //                    if (miscTypeList != null)
        //                    {
        //                        List<doMiscTypeCode> miscDispatchType = (from t in miscTypeList
        //                                                                 where t.FieldName == MiscType.C_DISPATCH_TYPE
        //                                                                 && t.ValueCode == doDraftRentalContract.doTbt_DraftRentalContrat.DispatchTypeCode
        //                                                                 select t).ToList<doMiscTypeCode>();
        //                        if (miscDispatchType != null && miscDispatchType.Count > 0)
        //                        {
        //                            dtTbt_DocContractReport.DispatchType = dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN ? miscDispatchType[0].ValueDisplayEN : miscDispatchType[0].ValueDisplayLC;
        //                        }
        //                    }

        //                    dtTbt_DocContractReport.OperationDate = doDraftRentalContract.doTbt_DraftRentalContrat.ExpectedStartServiceDate;
        //                    dtTbt_DocContractReport.AutoRenewMonth = doDraftRentalContract.doTbt_DraftRentalContrat.AutoRenewMonth;
        //                    dtTbt_DocContractReport.NegotiationTotalInstallFee = doDraftRentalContract.doTbt_DraftRentalContrat.OrderInstallFee;

        //                    //Set payment information
        //                    List<tbt_DraftRentalBillingTarget> doTbt_DraftRentalBillingTargetDepositFee = null;
        //                    if (doDraftRentalContract.doTbt_DraftRentalBillingTarget != null && doDraftRentalContract.doTbt_DraftRentalBillingTarget.Count > 0)
        //                    {
        //                        doTbt_DraftRentalBillingTargetDepositFee = (from t in doDraftRentalContract.doTbt_DraftRentalBillingTarget
        //                                                                    where t.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE
        //                                                                    select t).ToList<tbt_DraftRentalBillingTarget>();
        //                    }

        //                    List<tbt_DraftRentalBillingTarget> doTbt_DraftRentalBillingTargetInstallFeeAprv = null;
        //                    if (doDraftRentalContract.doTbt_DraftRentalBillingTarget != null && doDraftRentalContract.doTbt_DraftRentalBillingTarget.Count > 0)
        //                    {
        //                        doTbt_DraftRentalBillingTargetInstallFeeAprv = (from t in doDraftRentalContract.doTbt_DraftRentalBillingTarget
        //                                                                        where t.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
        //                                                                                && t.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT
        //                                                                        select t).ToList<tbt_DraftRentalBillingTarget>();
        //                    }

        //                    List<tbt_DraftRentalBillingTarget> doTbt_DraftRentalBillingTargetInstallFeeComp = null;
        //                    if (doDraftRentalContract.doTbt_DraftRentalBillingTarget != null && doDraftRentalContract.doTbt_DraftRentalBillingTarget.Count > 0)
        //                    {
        //                        doTbt_DraftRentalBillingTargetInstallFeeComp = (from t in doDraftRentalContract.doTbt_DraftRentalBillingTarget
        //                                                                        where t.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
        //                                                                                && t.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION
        //                                                                        select t).ToList<tbt_DraftRentalBillingTarget>();
        //                    }

        //                    List<tbt_DraftRentalBillingTarget> doTbt_DraftRentalBillingTargetInstallFeeServ = null;
        //                    if (doDraftRentalContract.doTbt_DraftRentalBillingTarget != null && doDraftRentalContract.doTbt_DraftRentalBillingTarget.Count > 0)
        //                    {
        //                        doTbt_DraftRentalBillingTargetInstallFeeServ = (from t in doDraftRentalContract.doTbt_DraftRentalBillingTarget
        //                                                                        where t.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
        //                                                                                && t.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE
        //                                                                        select t).ToList<tbt_DraftRentalBillingTarget>();
        //                    }

        //                    dtTbt_DocContractReport.DepositFeePayMethod = (doTbt_DraftRentalBillingTargetDepositFee != null && doTbt_DraftRentalBillingTargetDepositFee.Count > 0)?
        //                                                                    doTbt_DraftRentalBillingTargetDepositFee[0].PayMethod : null;

        //                    dtTbt_DocContractReport.DepositFeePhase = doDraftRentalContract.doTbt_DraftRentalContrat.BillingTimingDepositFee;

        //                    dtTbt_DocContractReport.InstallFee_ApproveContract = doDraftRentalContract.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContract;
        //                    if (doTbt_DraftRentalBillingTargetInstallFeeAprv != null && doTbt_DraftRentalBillingTargetInstallFeeAprv.Count == 1)
        //                    {
        //                        dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract = doTbt_DraftRentalBillingTargetInstallFeeAprv[0].PayMethod;
        //                    }
        //                    else
        //                    {
        //                        dtTbt_DocContractReport.InstallFeePayMethod_ApproveContract = null;
        //                    }

        //                    dtTbt_DocContractReport.InstallFee_CompleteInstall = doDraftRentalContract.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstall;
        //                    if (doTbt_DraftRentalBillingTargetInstallFeeComp != null && doTbt_DraftRentalBillingTargetInstallFeeComp.Count == 1)
        //                    {
        //                        dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall = doTbt_DraftRentalBillingTargetInstallFeeComp[0].PayMethod;
        //                    }
        //                    else
        //                    {
        //                        dtTbt_DocContractReport.InstallFeePayMethod_CompleteInstall = null;
        //                    }

        //                    dtTbt_DocContractReport.InstallFee_StartService = doDraftRentalContract.doTbt_DraftRentalContrat.OrderInstallFee_StartService;
        //                    if (doTbt_DraftRentalBillingTargetInstallFeeServ != null && doTbt_DraftRentalBillingTargetInstallFeeServ.Count == 1)
        //                    {
        //                        dtTbt_DocContractReport.InstallFeePayMethod_StartService = doTbt_DraftRentalBillingTargetInstallFeeServ[0].PayMethod;
        //                    }
        //                    else
        //                    {
        //                        dtTbt_DocContractReport.InstallFeePayMethod_StartService = null;
        //                    }
        //                    //End payment information
        //                }

        //                dtTbt_DocContractReport.DocumentLanguage = sParam.DocumentTemplateCondition.ContractDocumentLanguage;
        //                dtTbt_DocContractReport.CustomerSignatureName = null;
        //                dtTbt_DocContractReport.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                dtTbt_DocContractReport.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //                dtTbt_DocContractReport.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                dtTbt_DocContractReport.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

        //                dsContractDocDataData.dtTbt_DocContractReport = new List<tbt_DocContractReport>();
        //                dsContractDocDataData.dtTbt_DocContractReport.Add(dtTbt_DocContractReport);
        //            }
        //            //End dtTbt_DocContractReport

        //            //Get OldContractFee of previous OCC
        //            rentContHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
        //            string strPrevOCC = rentContHandler.GetPreviousImplementedOCC(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC);
        //            if (String.IsNullOrEmpty(strPrevOCC) == true)
        //            {
        //                strPrevOCC = rentContHandler.GetPreviousUnimplementedOCC(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC);
        //            }

        //            decimal? decPrevOldContractFee = null;
        //            List<tbt_RentalSecurityBasic> dtRentalSecurityBasic = rentContHandler.GetTbt_RentalSecurityBasic(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, strPrevOCC);
        //            if (dtRentalSecurityBasic != null && dtRentalSecurityBasic.Count > 0)
        //            {
        //                decPrevOldContractFee = dtRentalSecurityBasic[0].OrderContractFee;
        //            }


        //            //Set data in dtTbt_DocChangeMemo
        //            if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_MEMO)
        //            {
        //                if (dsRentalContract != null && dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
        //                {
        //                    //dtTbt_DocChangeMemo.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
        //                    dtTbt_DocChangeMemo.EffectiveDate = dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;
        //                    dtTbt_DocChangeMemo.OldContractFee = decPrevOldContractFee; //OldContractFee of previous OCC
        //                    dtTbt_DocChangeMemo.NewContractFee = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee;
        //                    dtTbt_DocChangeMemo.ChangeContent = null;
        //                    dtTbt_DocChangeMemo.CustomerSignatureName = null;
        //                    dtTbt_DocChangeMemo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                    dtTbt_DocChangeMemo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //                    dtTbt_DocChangeMemo.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                    dtTbt_DocChangeMemo.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

        //                    dsContractDocDataData.dtTbt_DocChangeMemo = new List<tbt_DocChangeMemo>();
        //                    dsContractDocDataData.dtTbt_DocChangeMemo.Add(dtTbt_DocChangeMemo);
        //                }
        //            }
        //            //End dtTbt_DocChangeMemo

        //            //Set data in dtTbt_DocChangeNotice
        //            if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_NOTICE)
        //            {
        //                if (dsRentalContract != null && dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
        //                {
        //                    //dtTbt_DocChangeNotice.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
        //                    dtTbt_DocChangeNotice.EffectiveDate = dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;

        //                    dtTbt_DocChangeNotice.ChangeContent = null;
        //                    dtTbt_DocChangeNotice.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                    dtTbt_DocChangeNotice.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //                    dtTbt_DocChangeNotice.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                    dtTbt_DocChangeNotice.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

        //                    dsContractDocDataData.dtTbt_DocChangeNotice = new List<tbt_DocChangeNotice>();
        //                    dsContractDocDataData.dtTbt_DocChangeNotice.Add(dtTbt_DocChangeNotice);
        //                }
        //            }
        //            //End dtTbt_DocChangeNotice

        //            //Set data in dtTbt_DocConfirmCurrentInstrumentMemo
        //            if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO)
        //            {
        //                if (dsRentalContract != null && dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
        //                {
        //                    //dtTbt_DocConfirmCurrentInstrumentMemo.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
        //                    dtTbt_DocConfirmCurrentInstrumentMemo.RealInvestigationDate = dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;

        //                    dtTbt_DocConfirmCurrentInstrumentMemo.CustomerSignatureName = null;
        //                    dtTbt_DocConfirmCurrentInstrumentMemo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                    dtTbt_DocConfirmCurrentInstrumentMemo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //                    dtTbt_DocConfirmCurrentInstrumentMemo.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                    dtTbt_DocConfirmCurrentInstrumentMemo.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

        //                    dsContractDocDataData.dtTbt_DocConfirmCurrentInstrumentMemo = new List<tbt_DocConfirmCurrentInstrumentMemo>();
        //                    dsContractDocDataData.dtTbt_DocConfirmCurrentInstrumentMemo.Add(dtTbt_DocConfirmCurrentInstrumentMemo);
        //                }
        //            }
        //            //End dtTbt_DocConfirmCurrentInstrumentMemo


        //            if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO
        //                || dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CANCEL_CONTRACT_QUOTATION)
        //            {
        //                //Set data in dtTbt_DocCancelContractMemo
        //                if (dsRentalContract != null && dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
        //                {
        //                    //dtTbt_DocCancelContractMemo.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
        //                    dtTbt_DocCancelContractMemo.CancelContractDate = dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;
        //                    dtTbt_DocCancelContractMemo.StartServiceDate = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractStartDate;

        //                    if(dsRentalContract.dtTbt_CancelContractMemo != null && dsRentalContract.dtTbt_CancelContractMemo.Count > 0)
        //                    {
        //                        dtTbt_DocCancelContractMemo.TotalSlideAmt = dsRentalContract.dtTbt_CancelContractMemo[0].TotalSlideAmt;
        //                        dtTbt_DocCancelContractMemo.TotalReturnAmt = dsRentalContract.dtTbt_CancelContractMemo[0].TotalReturnAmt;
        //                        dtTbt_DocCancelContractMemo.TotalBillingAmt = dsRentalContract.dtTbt_CancelContractMemo[0].TotalBillingAmt;
        //                        dtTbt_DocCancelContractMemo.TotalAmtAfterCounterBalance = dsRentalContract.dtTbt_CancelContractMemo[0].TotalAmtAfterCounterBalance;

        //                        if (miscTypeList != null)
        //                        {
        //                            List<doMiscTypeCode> miscProcAfterCounterBalanceType = (from t in miscTypeList
        //                                                                                    where t.FieldName == MiscType.C_PROC_AFT_COUNTER_BALANCE_TYPE
        //                                                                                    && t.ValueCode == dsRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType
        //                                                                                    select t).ToList<doMiscTypeCode>();

        //                            dtTbt_DocCancelContractMemo.ProcessAfterCounterBalanceType = dsRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType;
        //                            if (miscProcAfterCounterBalanceType != null && miscProcAfterCounterBalanceType.Count > 0)
        //                            {
        //                                dtTbt_DocCancelContractMemo.ProcessAfterCounterBalanceTypeName = miscProcAfterCounterBalanceType[0].ValueDisplayLC;
        //                            }
        //                        }

        //                        dtTbt_DocCancelContractMemo.OtherRemarks = dsRentalContract.dtTbt_CancelContractMemo[0].OtherRemarks;
        //                    }

        //                    dtTbt_DocCancelContractMemo.AutoTransferBillingType = "0";
        //                    dtTbt_DocCancelContractMemo.AutoTransferBillingAmt = null;
        //                    dtTbt_DocCancelContractMemo.BankTransferBillingType = "0";
        //                    dtTbt_DocCancelContractMemo.BankTransferBillingAmt = null;
        //                    dtTbt_DocCancelContractMemo.CustomerSignatureName = null;
        //                    dtTbt_DocCancelContractMemo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                    dtTbt_DocCancelContractMemo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //                    dtTbt_DocCancelContractMemo.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                    dtTbt_DocCancelContractMemo.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

        //                    dsContractDocDataData.dtTbt_DocCancelContractMemo = new List<tbt_DocCancelContractMemo>();
        //                    dsContractDocDataData.dtTbt_DocCancelContractMemo.Add(dtTbt_DocCancelContractMemo);
        //                }
        //                //End dtTbt_DocCancelContractMemo

        //                //Set data in dtTbt_DocCancelContractMemoDetail 
        //                if (dsRentalContract != null && dsRentalContract.dtTbt_CancelContractMemoDetail != null && dsRentalContract.dtTbt_CancelContractMemoDetail.Count > 0)
        //                {
        //                    List<tbt_DocCancelContractMemoDetail> docCancelContractMemoDetailList = new List<tbt_DocCancelContractMemoDetail>();
        //                    foreach (tbt_CancelContractMemoDetail memoDetailTemp in dsRentalContract.dtTbt_CancelContractMemoDetail)
        //                    {
        //                        tbt_DocCancelContractMemoDetail docMemoDetail = new tbt_DocCancelContractMemoDetail();
        //                        //docMemoDetail.DocCancelMemoID	=> Auto-run
        //                        //docMemoDetail.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()

        //                        if (miscTypeList != null)
        //                        {
        //                            List<doMiscTypeCode> miscBillingType = (from t in miscTypeList
        //                                                                    where t.FieldName == MiscType.C_CONTRACT_BILLING_TYPE
        //                                                                    && t.ValueCode == docMemoDetail.BillingType
        //                                                                    select t).ToList<doMiscTypeCode>();

        //                            docMemoDetail.BillingType = memoDetailTemp.BillingType;
        //                            if (miscBillingType != null && miscBillingType.Count > 0)
        //                            {
        //                                docMemoDetail.BillingTypeName = miscBillingType[0].ValueDisplayLC;
        //                            }

        //                            List<doMiscTypeCode> miscHandlingType = (from t in miscTypeList
        //                                                                     where t.FieldName == MiscType.C_HANDLING_TYPE
        //                                                                     && t.ValueCode == docMemoDetail.HandlingType
        //                                                                     select t).ToList<doMiscTypeCode>();

        //                            docMemoDetail.HandlingType = memoDetailTemp.HandlingType;
        //                            if (miscHandlingType != null && miscHandlingType.Count > 0)
        //                            {
        //                                docMemoDetail.HandlingTypeName = miscHandlingType[0].ValueDisplayLC;
        //                            }
        //                        }

        //                        docMemoDetail.StartPeriodDate = memoDetailTemp.StartPeriodDate;
        //                        docMemoDetail.EndPeriodDate = memoDetailTemp.EndPeriodDate;
        //                        docMemoDetail.NormalFeeAmount = memoDetailTemp.NormalFeeAmount;
        //                        docMemoDetail.FeeAmount = memoDetailTemp.FeeAmount;
        //                        docMemoDetail.TaxAmount = memoDetailTemp.TaxAmount;
        //                        docMemoDetail.ContractCode_CounterBalance = memoDetailTemp.ContractCode_CounterBalance;
        //                        docMemoDetail.Remark = memoDetailTemp.Remark;
        //                        docMemoDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                        docMemoDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //                        docMemoDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                        docMemoDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

        //                        docCancelContractMemoDetailList.Add(docMemoDetail);
        //                    }

        //                    dsContractDocDataData.dtTbt_DocCancelContractMemoDetail = docCancelContractMemoDetailList;
        //                }
        //                //End dtTbt_DocCancelContractMemoDetail 
        //            }

        //            //Set data in dtTbt_DocChangeFeeMemo
        //            if (dtTbt_ContractDocument.DocumentCode == DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO)
        //            {
        //                if (dsRentalContract != null && dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
        //                {
        //                    //dtTbt_DocChangeFeeMemo.DocID = dtTbt_ContractDocument.DocID => set at CreateContractDocData()
        //                    dtTbt_DocChangeFeeMemo.EffectiveDate = dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;
        //                    dtTbt_DocChangeMemo.OldContractFee = decPrevOldContractFee; //OldContractFee of previous OCC
        //                    dtTbt_DocChangeFeeMemo.NewContractFee = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee;
        //                    dtTbt_DocChangeFeeMemo.ChangeContractFeeDate = dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;
        //                    dtTbt_DocChangeFeeMemo.ReturnToOriginalFeeDate = dsRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate;
        //                    dtTbt_DocChangeFeeMemo.CustomerSignatureName = null;
        //                    dtTbt_DocChangeFeeMemo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                    dtTbt_DocChangeFeeMemo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //                    dtTbt_DocChangeFeeMemo.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //                    dtTbt_DocChangeFeeMemo.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

        //                    dsContractDocDataData.dtTbt_DocChangeFeeMemo = new List<tbt_DocChangeFeeMemo>();
        //                    dsContractDocDataData.dtTbt_DocChangeFeeMemo.Add(dtTbt_DocChangeFeeMemo);
        //                }
        //            }
        //            //End dtTbt_DocChangeFeeMemo


        //            //Save contract document data
        //            rentContHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
        //            dsContractDocData dsContractDocResult = rentContHandler.CreateContractDocData(dsContractDocDataData);
        //            /*-----------------------------------*/

        //            scop.Complete();

        //            MessageModel msgModelResult = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
        //            string strDocNoResult = "";
        //            if (dsContractDocResult.dtTbt_ContractDocument != null && dsContractDocResult.dtTbt_ContractDocument.Count > 0)
        //            {
        //                CommonUtil comUtil = new CommonUtil();
        //                if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
        //                {
        //                    strDocNoResult = comUtil.ConvertContractCode(dsContractDocResult.dtTbt_ContractDocument[0].DocNo, CommonUtil.CONVERT_TYPE.TO_SHORT);                            
        //                }
        //                else if (doDraftRentalContract != null && doDraftRentalContract.doTbt_DraftRentalContrat != null)
        //                {
        //                    strDocNoResult = comUtil.ConvertQuotationTargetCode(dsContractDocResult.dtTbt_ContractDocument[0].DocNo, CommonUtil.CONVERT_TYPE.TO_SHORT);                        
        //                }
        //            }

        //            res.ResultData = new object[] { msgModelResult, strDocNoResult };
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}

        //#endregion

        //#region Method

        //private string GetDocumentCode_CTS160(CTS160_DocumentTemplateCondition cond, string strChangeType)
        //{
        //    string strDocumentCode = string.Empty;
        //    if (cond != null)
        //    {
        //        if (cond.ContractFlag == true)
        //        {
        //            if (cond.ContractDocumentLanguage == DocLanguage.C_DOC_LANGUAGE_EN)
        //            {
        //                strDocumentCode = DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN;
        //            }
        //            else
        //            {
        //                strDocumentCode = DocumentCode.C_DOCUMENT_CODE_CONTRACT_TH;
        //            }
        //        }
        //        else if (cond.MemorandumFlag == true)
        //        { 
        //            if (strChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_NEW_START
        //                || strChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE
        //                || strChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_EXCHANGE_INSTRU_AT_MA
        //                || strChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MOVE_INSTRU
        //                || strChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP)
        //            {
        //                strDocumentCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_MEMO;
        //            }

        //            if (strChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_QTY_DURING_STOP
        //                || strChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_INSTRUMENT_QTY) 
        //            {
        //                strDocumentCode = DocumentCode.C_DOCUMENT_CODE_CONFIRM_CURRENT_INSTRUMENT_MEMO;
        //            }

        //            if (strChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT
        //                || strChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL
        //                || strChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START) 
        //            {
        //                strDocumentCode = DocumentCode.C_DOCUMENT_CODE_CONCEL_CONTRACT_MEMO;
        //            }

        //            if (strChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE) 
        //            {
        //                strDocumentCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_FEE_MEMO;
        //            }
        //        }
        //        else if (cond.NoticeFlag == true)
        //        {
        //            strDocumentCode = DocumentCode.C_DOCUMENT_CODE_CHANGE_NOTICE;
        //        }
        //        else if (cond.CoverLetterFlag == true)
        //        {
        //            strDocumentCode = DocumentCode.C_DOCUMENT_CODE_COVER_LETTER;
        //        }
        //    }

        //    return strDocumentCode;
        //}

        //#endregion

    }
}
