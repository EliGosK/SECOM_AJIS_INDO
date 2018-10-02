using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Controllers;
using System.Web.Mvc;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.Common.Models.EmailTemplates;
using System.Reflection;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        private const string CTS020_SCREEN_NAME = "CTS020";

        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS020_Authority(CTS020_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                #region Check Authority

                bool hasPermission = false;
                if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.NEW)
                    hasPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_FQ99, FunctionID.C_FUNC_ID_ADD);
                else if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.EDIT)
                    hasPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_FQ99, FunctionID.C_FUNC_ID_EDIT);
                else if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE)
                {
                    hasPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_SEARCH_APPROVE, FunctionID.C_FUNC_ID_OPERATE);
                    if (hasPermission)
                    {
                        //IApprovalPermissionHandler handler = ServiceContainer.GetService<IApprovalPermissionHandler>() as IApprovalPermissionHandler;
                        //hasPermission = handler.isPermittedIPAddress();
                        hasPermission = isPermittedIPAddress();
                    }
                }

                if (hasPermission == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                #endregion
                #region Check is Suspending

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion
                #region Retrieve Data for Approve

                if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE)
                {
                    CTS020_RetrieveQuotationCondition_Edit cond = new CTS020_RetrieveQuotationCondition_Edit();
                    cond.QuotationTargetCode = param.QuotationTargetCode;

                    #region Validate

                    ValidatorUtil.BuildErrorMessage(res, new object[] { cond });
                    if (res.IsError)
                        return Json(res);

                    #endregion
                    #region Retrieve Data

                    IDraftSaleContractHandler shandler = ServiceContainer.GetService<IDraftSaleContractHandler>() as IDraftSaleContractHandler;

                    doDraftSaleContractData draftData = shandler.GetEntireDraftSaleContract(cond, doDraftSaleContractData.SALE_CONTRACT_MODE.APPROVE,
                        param.ProcessType);
                    if (draftData == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011,
                            new string[] { cond.QuotationTargetCode });
                    }
                    else
                    {
                        param.doDraftSaleContractData = draftData;

                        param.doRegisterDraftSaleContractData = new doDraftSaleContractData();
                        param.doRegisterDraftSaleContractData.doPurchaserCustomer =
                            CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(draftData.doPurchaserCustomer);
                        param.doRegisterDraftSaleContractData.doRealCustomer =
                            CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(draftData.doRealCustomer);
                        param.doRegisterDraftSaleContractData.doSite =
                            CommonUtil.CloneObject<doSite, doSite>(draftData.doSite);
                        param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail =
                            CommonUtil.ClonsObjectList<tbt_DraftSaleEmail, tbt_DraftSaleEmail>(draftData.doTbt_DraftSaleEmail);
                        param.doRegisterDraftSaleContractData.Mode = draftData.Mode;
                        param.doRegisterDraftSaleContractData.LastUpdateDateQuotationData = draftData.LastUpdateDateQuotationData;

                        param.ProcessType = doDraftSaleContractData.PROCESS_TYPE.NEW;
                        if (draftData.doTbt_DraftSaleContract.SaleProcessType == SaleProcessType.C_SALE_PROCESS_TYPE_ADD_SALE)
                        {
                            param.ProcessType = doDraftSaleContractData.PROCESS_TYPE.ADD;
                        }

                        param.FirstLoad = false;
                    }

                    #endregion
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<CTS020_ScreenParameter>(CTS020_SCREEN_NAME, param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize(CTS020_SCREEN_NAME)]
        public ActionResult CTS020()
        {
            ViewBag.ScreenMode = (int)CTS020_ScreenParameter.SCREEN_MODE.NEW;
            ViewBag.HideChangeQuotationButton = false;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    #region Retrieve Data for Approve

                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE && param.FirstLoad)
                    {
                        CTS020_RetrieveQuotationCondition_Edit cond = new CTS020_RetrieveQuotationCondition_Edit();
                        cond.QuotationTargetCode = param.QuotationTargetCode;

                        IDraftSaleContractHandler shandler = ServiceContainer.GetService<IDraftSaleContractHandler>() as IDraftSaleContractHandler;

                        param.doDraftSaleContractData = shandler.GetEntireDraftSaleContract(
                            cond,
                            doDraftSaleContractData.SALE_CONTRACT_MODE.APPROVE,
                            param.ProcessType);

                        param.doRegisterDraftSaleContractData = new doDraftSaleContractData();
                        param.doRegisterDraftSaleContractData.doPurchaserCustomer =
                            CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(param.doDraftSaleContractData.doPurchaserCustomer);
                        param.doRegisterDraftSaleContractData.doRealCustomer =
                            CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(param.doDraftSaleContractData.doRealCustomer);
                        param.doRegisterDraftSaleContractData.doSite =
                            CommonUtil.CloneObject<doSite, doSite>(param.doDraftSaleContractData.doSite);
                        param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail =
                            CommonUtil.ClonsObjectList<tbt_DraftSaleEmail, tbt_DraftSaleEmail>(param.doDraftSaleContractData.doTbt_DraftSaleEmail);
                        param.doRegisterDraftSaleContractData.Mode = param.doDraftSaleContractData.Mode;
                        param.doRegisterDraftSaleContractData.LastUpdateDateQuotationData = param.doDraftSaleContractData.LastUpdateDateQuotationData;
                    }
                    else
                        param.FirstLoad = true;

                    #endregion

                    ViewBag.ScreenMode = param.ScreenMode;

                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.NEW)
                    {
                        ViewBag.HideChangeQuotationButton = true;
                    }
                    else if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE)
                    {
                        ViewBag.HideChangeQuotationButton = true;
                        ViewBag.QuotationTargetCode = param.QuotationTargetCode;
                    }

                    ViewBag.ProcessType = Convert.ToInt32(param.ProcessType); //Add by Jutarat A. on 16102012
                }
            }
            catch (Exception)
            {
            }

            return View();
        }
        /// <summary>
        /// Generate FQ-99 information section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_03()
        {
            ViewBag.IsApproveMode = false;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    ViewBag.UseInstallFee = (param.OneShotFlag == true) ? 0 : 1; ;

                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE)
                        ViewBag.IsApproveMode = true;

                    if (param.doDraftSaleContractData != null)
                    {
                        ViewBag.ExpectedInstallCompleteDate = CommonUtil.TextDate(param.doDraftSaleContractData.doTbt_DraftSaleContract.ExpectedInstallCompleteDate);
                        ViewBag.ExpectedAcceptanceAgreeDate = CommonUtil.TextDate(param.doDraftSaleContractData.doTbt_DraftSaleContract.ExpectedAcceptanceAgreeDate);
                        ViewBag.SaleType = param.doDraftSaleContractData.doTbt_DraftSaleContract.SaleType;
                        ViewBag.ProjectCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.ProjectCode;


                        ViewBag.ConnectTargetCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.ConnectTargetCodeShort;

                        #region Normal Product Price

                        ViewBag.NormalProductPriceCurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType;

                        string txtNormalProductPrice;
                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            txtNormalProductPrice = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalProductPriceUsd);
                        else
                            txtNormalProductPrice = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalProductPrice);
                        
                        if (CommonUtil.IsNullOrEmpty(txtNormalProductPrice))
                            txtNormalProductPrice = "0.00";
                        ViewBag.NormalProductPrice = txtNormalProductPrice;

                        #endregion
                        #region Order Product Price

                        if (string.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderProductPriceCurrencyType))
                            param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderProductPriceCurrencyType =
                                param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType;

                        ViewBag.OrderProductPriceCurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderProductPriceCurrencyType;

                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.OrderProductPrice = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderProductPriceUsd);
                        else
                            ViewBag.OrderProductPrice = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderProductPrice);

                        #endregion
                        #region Order Product Price Billing Approval

                        if (string.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_ApproveContractCurrencyType))
                            param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_ApproveContractCurrencyType =
                                param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType;

                        ViewBag.BillingAmt_ApproveContractCurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_ApproveContractCurrencyType;

                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.BillingAmt_ApproveContract = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_ApproveContractUsd);
                        else
                            ViewBag.BillingAmt_ApproveContract = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_ApproveContract);

                        #endregion
                        #region Order Product Price Billing Partial Fee

                        if (string.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_PartialFeeCurrencyType))
                            param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_PartialFeeCurrencyType =
                                param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType;

                        ViewBag.BillingAmt_PartialFeeCurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_PartialFeeCurrencyType;

                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_PartialFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.BillingAmt_PartialFee = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_PartialFeeUsd);
                        else
                            ViewBag.BillingAmt_PartialFee = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_PartialFee);

                        #endregion
                        #region Order Product Price Billing Acceptance

                        if (string.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_AcceptanceCurrencyType))
                            param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_AcceptanceCurrencyType =
                                param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType;

                        ViewBag.BillingAmt_AcceptanceCurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_AcceptanceCurrencyType;

                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.BillingAmt_Acceptance = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_AcceptanceUsd);
                        else
                            ViewBag.BillingAmt_Acceptance = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_Acceptance);
                        
                        #endregion
                        #region Normal Install Fee

                        ViewBag.NormalInstallFeeCurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType;

                        string txtNormalInstallFee;
                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            txtNormalInstallFee = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalInstallFeeUsd);
                        else
                            txtNormalInstallFee = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalInstallFee);
                        
                        if (CommonUtil.IsNullOrEmpty(txtNormalInstallFee))
                            txtNormalInstallFee = "0.00";
                        ViewBag.NormalInstallFee = txtNormalInstallFee;

                        #endregion
                        #region Order Install Fee

                        if (string.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderInstallFeeCurrencyType))
                            param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderInstallFeeCurrencyType =
                                param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType;

                        ViewBag.OrderInstallFeeCurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderInstallFeeCurrencyType;

                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.OrderInstallFee = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderInstallFeeUsd);
                        else
                            ViewBag.OrderInstallFee = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderInstallFee);

                        #endregion
                        #region Order Install Fee Billing Approval

                        if (string.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_ApproveContractCurrencyType))
                            param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_ApproveContractCurrencyType =
                                param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType;

                        ViewBag.BillingAmtInstallation_ApproveContractCurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_ApproveContractCurrencyType;

                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.BillingAmtInstallation_ApproveContract = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_ApproveContractUsd);
                        else
                            ViewBag.BillingAmtInstallation_ApproveContract = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_ApproveContract);

                        #endregion
                        #region Order Install Fee Billing Partial Fee

                        if (string.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_PartialFeeCurrencyType))
                            param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_PartialFeeCurrencyType =
                                param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType;

                        ViewBag.BillingAmtInstallation_PartialFeeCurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_PartialFeeCurrencyType;

                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_PartialFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.BillingAmtInstallation_PartialFee = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_PartialFeeUsd);
                        else
                            ViewBag.BillingAmtInstallation_PartialFee = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_PartialFee);

                        #endregion
                        #region Order Install Fee Billing Acceptance

                        if (string.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_AcceptanceCurrencyType))
                            param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_AcceptanceCurrencyType =
                                param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType;

                        ViewBag.BillingAmtInstallation_AcceptanceCurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_AcceptanceCurrencyType;

                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.BillingAmtInstallation_Acceptance = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_AcceptanceUsd);
                        else
                            ViewBag.BillingAmtInstallation_Acceptance = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmtInstallation_Acceptance);
                        
                        #endregion
                        
                        //ViewBag.OrderSalePrice = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderSalePrice);
                        //ViewBag.BillingAmt_ApproveContract = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_ApproveContract);
                        //ViewBag.BillingAmt_PartialFee = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_PartialFee);
                        //ViewBag.BillingAmt_Acceptance = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_Acceptance);

                        ViewBag.DistributedInstallTypeCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.DistributedInstallTypeCode;
                        ViewBag.DistributedOriginCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.DistributedOriginCode;
                        ViewBag.ContractOfficeCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.ContractOfficeCode;
                        ViewBag.OperationOfficeCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.OperationOfficeCode;
                        ViewBag.SalesOfficeCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.OperationOfficeCode;

                        ViewBag.SalesmanEmpNo1 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNo1;
                        ViewBag.SalesmanEmpNameNo1 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNameNo1;
                        ViewBag.SalesmanEmpNo2 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNo2;
                        ViewBag.SalesmanEmpNameNo2 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNameNo2;
                        ViewBag.SalesmanEmpNo3 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNo3;
                        ViewBag.SalesmanEmpNameNo3 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNameNo3;
                        ViewBag.SalesmanEmpNo4 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNo4;
                        ViewBag.SalesmanEmpNameNo4 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNameNo4;
                        ViewBag.SalesmanEmpNo5 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNo5;
                        ViewBag.SalesmanEmpNameNo5 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNameNo5;
                        ViewBag.SalesmanEmpNo6 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNo6;
                        ViewBag.SalesmanEmpNameNo6 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNameNo6;
                        ViewBag.SalesmanEmpNo7 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNo7;
                        ViewBag.SalesmanEmpNameNo7 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNameNo7;
                        ViewBag.SalesmanEmpNo8 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNo8;
                        ViewBag.SalesmanEmpNameNo8 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNameNo8;
                        ViewBag.SalesmanEmpNo9 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNo9;
                        ViewBag.SalesmanEmpNameNo9 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNameNo9;
                        ViewBag.SalesmanEmpNo10 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNo10;
                        ViewBag.SalesmanEmpNameNo10 = param.doDraftSaleContractData.doTbt_DraftSaleContract.SalesmanEmpNameNo10;

                        ViewBag.ApproveNo1 = param.doDraftSaleContractData.doTbt_DraftSaleContract.ApproveNo1;
                        ViewBag.ApproveNo2 = param.doDraftSaleContractData.doTbt_DraftSaleContract.ApproveNo2;
                        ViewBag.ApproveNo3 = param.doDraftSaleContractData.doTbt_DraftSaleContract.ApproveNo3;
                        ViewBag.ApproveNo4 = param.doDraftSaleContractData.doTbt_DraftSaleContract.ApproveNo4;
                        ViewBag.ApproveNo5 = param.doDraftSaleContractData.doTbt_DraftSaleContract.ApproveNo5;
                        ViewBag.BICContractCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.BICContractCode;

                        #region Bid Guanrantee Amount 1
                        
                        ViewBag.BidGuaranteeAmount1CurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.BidGuaranteeAmount1CurrencyType;

                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.BidGuaranteeAmount1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.BidGuaranteeAmount1 = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BidGuaranteeAmount1Usd);
                        else
                            ViewBag.BidGuaranteeAmount1 = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BidGuaranteeAmount1);
                        
                        #endregion
                        #region Bid Guanrantee Amount 2

                        ViewBag.BidGuaranteeAmount2CurrencyType = param.doDraftSaleContractData.doTbt_DraftSaleContract.BidGuaranteeAmount2CurrencyType;

                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract.BidGuaranteeAmount2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.BidGuaranteeAmount2 = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BidGuaranteeAmount2Usd);
                        else
                            ViewBag.BidGuaranteeAmount2 = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BidGuaranteeAmount2);
                        
                        #endregion

                        if (param.doDraftSaleContractData.doTbt_RelationType != null)
                        {
                            foreach (tbt_RelationType rt in param.doDraftSaleContractData.doTbt_RelationType)
                            {
                                if (rt.RelationType == RelationType.C_RELATION_TYPE_SALE)
                                {
                                    ViewBag.LinkageSaleContractCode = rt.RelatedContractCodeShort;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS020/_CTS020_03");
        }
        /// <summary>
        /// Generate instrument detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_04()
        {
            return View("CTS020/_CTS020_04");
        }
        /// <summary>
        /// Generate recipient about approve contract result section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_05()
        {
            #region Get Email suffix

            string emailSuffix = "";

            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemConfig> emlst = chandler.GetSystemConfig(ConfigName.C_EMAIL_SUFFIX);
            if (emlst.Count > 0)
                emailSuffix = emlst[0].ConfigValue;

            #endregion

            ViewBag.EmailAddress = emailSuffix;
            return View("CTS020/_CTS020_05");
        }
        /// <summary>
        /// Generate purchaser customer section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_06()
        {
            ViewBag.HasCustomerData = false;
            ViewBag.IsCheckBranch = false;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doDraftSaleContractData != null)
                    {
                        if (param.doDraftSaleContractData.doPurchaserCustomer != null)
                        {
                            ViewBag.HasCustomerData = true;

                            ViewBag.PurchaserSignerTypeCode = param.PurchaserSignerTypeCode;
                            if (param.doDraftSaleContractData.doTbt_DraftSaleContract.PurchaserSignerTypeCode != null)
                                ViewBag.PurchaserSignerTypeCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.PurchaserSignerTypeCode;

                            ViewBag.CustCode = param.doDraftSaleContractData.doPurchaserCustomer.CustCodeShort;
                            ViewBag.CustStatusCodeName = param.doDraftSaleContractData.doPurchaserCustomer.CustStatusCodeName;
                            ViewBag.CustTypeCodeName = param.doDraftSaleContractData.doPurchaserCustomer.CustTypeCodeName;
                            ViewBag.CustFullNameEN = param.doDraftSaleContractData.doPurchaserCustomer.CustFullNameEN;
                            ViewBag.CustFullNameLC = param.doDraftSaleContractData.doPurchaserCustomer.CustFullNameLC;
                            ViewBag.AddressFullEN = param.doDraftSaleContractData.doPurchaserCustomer.AddressFullEN;
                            ViewBag.AddressFullLC = param.doDraftSaleContractData.doPurchaserCustomer.AddressFullLC;
                            ViewBag.Nationality = param.doDraftSaleContractData.doPurchaserCustomer.Nationality;
                            ViewBag.PhoneNo = param.doDraftSaleContractData.doPurchaserCustomer.PhoneNo;
                            ViewBag.BusinessTypeName = param.doDraftSaleContractData.doPurchaserCustomer.BusinessTypeName;
                            ViewBag.IDNo = param.doDraftSaleContractData.doPurchaserCustomer.IDNo;
                            ViewBag.URL = param.doDraftSaleContractData.doPurchaserCustomer.URL;

                            if (CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.BranchNameEN) == false
                                || CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.BranchNameLC) == false
                                || CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.BranchAddressEN) == false
                                || CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.BranchAddressLC) == false)
                            {
                                ViewBag.IsCheckBranch = true;
                            }
                            ViewBag.BranchNameEN = param.doDraftSaleContractData.doTbt_DraftSaleContract.BranchNameEN;
                            ViewBag.BranchNameLC = param.doDraftSaleContractData.doTbt_DraftSaleContract.BranchNameLC;
                            ViewBag.BranchAddressEN = param.doDraftSaleContractData.doTbt_DraftSaleContract.BranchAddressEN;
                            ViewBag.BranchAddressLC = param.doDraftSaleContractData.doTbt_DraftSaleContract.BranchAddressLC;

                            ViewBag.PurchaserMemo = param.doDraftSaleContractData.doTbt_DraftSaleContract.PurchaserMemo;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS020/_CTS020_06");
        }
        /// <summary>
        /// Generate real customer section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_07()
        {
            ViewBag.HasRealCustomerData = false;
            ViewBag.SameCustomer = false;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doDraftSaleContractData != null)
                    {
                        ViewBag.SameCustomer =
                            CTS020_IsSameCustomer(param.doDraftSaleContractData.doPurchaserCustomer, param.doDraftSaleContractData.doRealCustomer);

                        if (param.doDraftSaleContractData.doRealCustomer != null)
                        {
                            ViewBag.HasRealCustomerData = true;
                            ViewBag.CustCode = param.doDraftSaleContractData.doRealCustomer.CustCodeShort;
                            ViewBag.CustStatusCodeName = param.doDraftSaleContractData.doRealCustomer.CustStatusCodeName;
                            ViewBag.CustTypeCodeName = param.doDraftSaleContractData.doRealCustomer.CustTypeCodeName;
                            ViewBag.CustFullNameEN = param.doDraftSaleContractData.doRealCustomer.CustFullNameEN;
                            ViewBag.CustFullNameLC = param.doDraftSaleContractData.doRealCustomer.CustFullNameLC;
                            ViewBag.AddressFullEN = param.doDraftSaleContractData.doRealCustomer.AddressFullEN;
                            ViewBag.AddressFullLC = param.doDraftSaleContractData.doRealCustomer.AddressFullLC;
                            ViewBag.Nationality = param.doDraftSaleContractData.doRealCustomer.Nationality;
                            ViewBag.PhoneNo = param.doDraftSaleContractData.doRealCustomer.PhoneNo;
                            ViewBag.BusinessTypeName = param.doDraftSaleContractData.doRealCustomer.BusinessTypeName;
                            ViewBag.IDNo = param.doDraftSaleContractData.doRealCustomer.IDNo;
                            ViewBag.URL = param.doDraftSaleContractData.doRealCustomer.URL;

                            ViewBag.RealCustomerMemo = param.doDraftSaleContractData.doTbt_DraftSaleContract.RealCustomerMemo;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS020/_CTS020_07");
        }
        /// <summary>
        /// Generate site section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_08()
        {
            ViewBag.HasSiteData = false;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doDraftSaleContractData != null)
                    {
                        if (param.doDraftSaleContractData.doSite != null)
                        {
                            ViewBag.HasSiteData = true;

                            if (CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doSite.SiteCodeShort) == false)
                                ViewBag.SiteCodeForSearch = param.doDraftSaleContractData.doSite.SiteCodeShort.Split("-".ToCharArray())[0];
                            else if (param.doDraftSaleContractData.doRealCustomer != null)
                            {
                                if (CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doRealCustomer.SiteCustCodeShort) == false)
                                    ViewBag.SiteCodeForSearch = param.doDraftSaleContractData.doRealCustomer.SiteCustCodeShort;
                            }

                            ViewBag.SiteNo = param.doDraftSaleContractData.doSite.SiteNo;
                            ViewBag.SiteCode = param.doDraftSaleContractData.doSite.SiteCodeShort;
                            ViewBag.SiteNameEN = param.doDraftSaleContractData.doSite.SiteNameEN;
                            ViewBag.SiteNameLC = param.doDraftSaleContractData.doSite.SiteNameLC;
                            ViewBag.AddressFullEN = param.doDraftSaleContractData.doSite.AddressFullEN;
                            ViewBag.AddressFullLC = param.doDraftSaleContractData.doSite.AddressFullLC;
                            ViewBag.PhoneNo = param.doDraftSaleContractData.doSite.PhoneNo;
                            ViewBag.BuildingUsageName = param.doDraftSaleContractData.doSite.BuildingUsageName;

                            if (CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doSite.SiteCode))
                            {
                                doSite s = CommonUtil.CloneObject<doSite, doSite>(param.doDraftSaleContractData.doSite);

                                ISiteMasterHandler shandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                                shandler.ValidateSiteData(s);
                                if (s != null)
                                {
                                    if (s.ValidateSiteData == false)
                                    {
                                        param.doDraftSaleContractData.doSite.ValidateSiteData = false;
                                        CTS020_ScreenData = param;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS020/_CTS020_08");
        }
        /// <summary>
        /// Generate contract point section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_09()
        {
            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    ViewBag.ContactPoint = param.ContactPoint;
                    if (param.doDraftSaleContractData != null)
                    {
                        if (param.doDraftSaleContractData.doTbt_DraftSaleContract != null)
                        {
                            if (CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.ContactPoint) == false)
                                ViewBag.ContactPoint = param.doDraftSaleContractData.doTbt_DraftSaleContract.ContactPoint;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS020/_CTS020_09");
        }
        /// <summary>
        /// Generate billing target section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_10()
        {
            ViewBag.HasBillingData = false;
            ViewBag.InstallationFee_Enabled = true;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    ViewBag.UseInstallFee = (param.OneShotFlag == true) ? 0 : 1;

                    if (param.BillingTarget != null)
                    {
                        if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.NEW)
                        {
                            ViewBag.SalePrice_ApprovalCurrencyType = param.BillingTarget.SalePrice_ApprovalCurrencyType;
                            ViewBag.SalePrice_PartialCurrencyType = param.BillingTarget.SalePrice_PartialCurrencyType;
                            ViewBag.SalePrice_AcceptanceCurrencyType = param.BillingTarget.SalePrice_AcceptanceCurrencyType;
                            ViewBag.InstallationFee_ApprovalCurrencyType = param.BillingTarget.InstallationFee_ApprovalCurrencyType;
                            ViewBag.InstallationFee_PartialCurrencyType = param.BillingTarget.InstallationFee_PartialCurrencyType;
                            ViewBag.InstallationFee_AcceptanceCurrencyType = param.BillingTarget.InstallationFee_AcceptanceCurrencyType;

                            ViewBag.SalePrice_PaymentMethod_Approval = param.BillingTarget.SalePrice_PaymentMethod_Approval;
                            ViewBag.SalePrice_PaymentMethod_Partial = param.BillingTarget.SalePrice_PaymentMethod_Partial;
                            ViewBag.SalePrice_PaymentMethod_Acceptance = param.BillingTarget.SalePrice_PaymentMethod_Acceptance;
                            ViewBag.InstallationFee_PaymentMethod_Approval = param.BillingTarget.InstallationFee_PaymentMethod_Approval;
                            ViewBag.InstallationFee_PaymentMethod_Partial = param.BillingTarget.InstallationFee_PaymentMethod_Partial;
                            ViewBag.InstallationFee_PaymentMethod_Acceptance = param.BillingTarget.InstallationFee_PaymentMethod_Acceptance;
                        }
                        else
                        {
                            ViewBag.HasBillingData = true;

                            ViewBag.BillingTargetCode = param.BillingTarget.BillingTargetCodeShort;
                            ViewBag.BillingClientCode = param.BillingTarget.BillingClientCodeShort;

                            if (param.BillingTarget.BillingClient != null)
                            {
                                ViewBag.FullNameEN = param.BillingTarget.BillingClient.FullNameEN;
                                ViewBag.BranchNameEN = param.BillingTarget.BillingClient.BranchNameEN;
                                ViewBag.AddressEN = param.BillingTarget.BillingClient.AddressEN;
                                ViewBag.FullNameLC = param.BillingTarget.BillingClient.FullNameLC;
                                ViewBag.BranchNameLC = param.BillingTarget.BillingClient.BranchNameLC;
                                ViewBag.AddressLC = param.BillingTarget.BillingClient.AddressLC;
                                ViewBag.Nationality = param.BillingTarget.BillingClient.Nationality;
                                ViewBag.PhoneNo = param.BillingTarget.BillingClient.PhoneNo;
                                ViewBag.BusinessTypeName = param.BillingTarget.BillingClient.BusinessTypeName;
                                ViewBag.IDNo = param.BillingTarget.BillingClient.IDNo;
                            }

                            ViewBag.BillingOfficeCode = param.BillingTarget.BillingOfficeCode;

                            #region Sale Price Approval

                            ViewBag.SalePrice_ApprovalCurrencyType = param.BillingTarget.SalePrice_ApprovalCurrencyType;

                            if (param.BillingTarget.SalePrice_ApprovalCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                ViewBag.SalePrice_Approval = CommonUtil.TextNumeric(param.BillingTarget.SalePrice_ApprovalUsd);
                            else
                                ViewBag.SalePrice_Approval = CommonUtil.TextNumeric(param.BillingTarget.SalePrice_Approval);

                            ViewBag.SalePrice_PaymentMethod_Approval = param.BillingTarget.SalePrice_PaymentMethod_Approval;

                            #endregion
                            #region Sale Price Partial

                            ViewBag.SalePrice_PartialCurrencyType = param.BillingTarget.SalePrice_PartialCurrencyType;

                            if (param.BillingTarget.SalePrice_PartialCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                ViewBag.SalePrice_Partial = CommonUtil.TextNumeric(param.BillingTarget.SalePrice_PartialUsd);
                            else
                                ViewBag.SalePrice_Partial = CommonUtil.TextNumeric(param.BillingTarget.SalePrice_Partial);

                            ViewBag.SalePrice_PaymentMethod_Partial = param.BillingTarget.SalePrice_PaymentMethod_Partial;

                            #endregion
                            #region Sale Price _Acceptance

                            ViewBag.SalePrice_AcceptanceCurrencyType = param.BillingTarget.SalePrice_AcceptanceCurrencyType;

                            if (param.BillingTarget.SalePrice_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                ViewBag.SalePrice_Acceptance = CommonUtil.TextNumeric(param.BillingTarget.SalePrice_AcceptanceUsd);
                            else
                                ViewBag.SalePrice_Acceptance = CommonUtil.TextNumeric(param.BillingTarget.SalePrice_Acceptance);

                            ViewBag.SalePrice_PaymentMethod_Acceptance = param.BillingTarget.SalePrice_PaymentMethod_Acceptance;

                            #endregion
                            #region Installation Fee Approval

                            ViewBag.InstallationFee_ApprovalCurrencyType = param.BillingTarget.InstallationFee_ApprovalCurrencyType;

                            if (param.BillingTarget.InstallationFee_ApprovalCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                ViewBag.InstallationFee_Approval = CommonUtil.TextNumeric(param.BillingTarget.InstallationFee_ApprovalUsd);
                            else
                                ViewBag.InstallationFee_Approval = CommonUtil.TextNumeric(param.BillingTarget.InstallationFee_Approval);

                            ViewBag.InstallationFee_PaymentMethod_Approval = param.BillingTarget.InstallationFee_PaymentMethod_Approval;

                            #endregion
                            #region Installation Fee Partial

                            ViewBag.InstallationFee_PartialCurrencyType = param.BillingTarget.InstallationFee_PartialCurrencyType;

                            if (param.BillingTarget.InstallationFee_PartialCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                ViewBag.InstallationFee_Partial = CommonUtil.TextNumeric(param.BillingTarget.InstallationFee_PartialUsd);
                            else
                                ViewBag.InstallationFee_Partial = CommonUtil.TextNumeric(param.BillingTarget.InstallationFee_Partial);

                            ViewBag.InstallationFee_PaymentMethod_Partial = param.BillingTarget.InstallationFee_PaymentMethod_Partial;

                            #endregion
                            #region Installation Fee Acceptance

                            ViewBag.InstallationFee_AcceptanceCurrencyType = param.BillingTarget.InstallationFee_AcceptanceCurrencyType;

                            if (param.BillingTarget.InstallationFee_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                ViewBag.InstallationFee_Acceptance = CommonUtil.TextNumeric(param.BillingTarget.InstallationFee_AcceptanceUsd);
                            else
                                ViewBag.InstallationFee_Acceptance = CommonUtil.TextNumeric(param.BillingTarget.InstallationFee_Acceptance);

                            ViewBag.InstallationFee_PaymentMethod_Acceptance = param.BillingTarget.InstallationFee_PaymentMethod_Acceptance;

                            #endregion

                            //decimal total = 0;
                            //if (param.BillingTarget.SalePrice_Approval != null)
                            //    total += param.BillingTarget.SalePrice_Approval.Value;
                            //if (param.BillingTarget.SalePrice_Partial != null)
                            //    total += param.BillingTarget.SalePrice_Partial.Value;
                            //if (param.BillingTarget.SalePrice_Acceptance != null)
                            //    total += param.BillingTarget.SalePrice_Acceptance.Value;

                            //ViewBag.TotalPrice = CommonUtil.TextNumeric(total);
                        }
                    }
                    //else
                    //{
                    //    ViewBag.SalePrice_PaymentMethod_Approval = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                    //    ViewBag.SalePrice_PaymentMethod_Partial = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                    //    ViewBag.SalePrice_PaymentMethod_Acceptance = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                    //    ViewBag.InstallationFee_PaymentMethod_Approval = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                    //    ViewBag.InstallationFee_PaymentMethod_Partial = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                    //    ViewBag.InstallationFee_PaymentMethod_Acceptance = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER;                        
                    //}
                }
            }
            catch (Exception)
            {
            }

            return View("CTS020/_CTS020_10");
        }
        /// <summary>
        /// Generate result of register sale contract section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_11()
        {
            ViewBag.ShowButtonRegisterNext = false;
            ViewBag.ShowButtonEditNext = false;

            try
            {
                tbt_DraftSaleContract contract = null;

                #region Get Session

                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doDraftSaleContractData != null)
                        contract = param.doDraftSaleContractData.doTbt_DraftSaleContract;
                }
                if (contract == null)
                    contract = new tbt_DraftSaleContract();

                #endregion

                if (contract != null)
                {
                    CommonUtil cmm = new CommonUtil();
                    ViewBag.QuotationTargetCodeFull = contract.QuotationTargetCodeFull;

                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.NEW)
                        ViewBag.ShowButtonRegisterNext = true;
                    else
                        ViewBag.ShowButtonEditNext = true;
                }
            }
            catch
            {
            }

            return View("CTS020/_CTS020_11");
        }

        #endregion
        #region Actions

        /// <summary>
        /// Get data from session
        /// </summary>
        /// <param name="ObjectTypeID"></param>
        /// <returns></returns>
        public ActionResult CTS020_GetInitialData(int ObjectTypeID)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftSaleContractData != null)
                    {
                        switch (ObjectTypeID)
                        {
                            case 1:
                                res.ResultData = param.doRegisterDraftSaleContractData.doPurchaserCustomer;
                                break;
                            case 2:
                                res.ResultData = param.doRegisterDraftSaleContractData.doRealCustomer;
                                break;
                            case 3:
                                res.ResultData = param.doRegisterDraftSaleContractData.doSite;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Set data to session
        /// </summary>
        /// <param name="initData"></param>
        /// <param name="ObjectTypeID"></param>
        /// <returns></returns>
        public ActionResult CTS020_SetInitialData(doDraftSaleContractData initData, int ObjectTypeID)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftSaleContractData != null)
                    {
                        bool isSameCustomer =
                            CTS020_IsSameCustomer(param.doRegisterDraftSaleContractData.doPurchaserCustomer, param.doRegisterDraftSaleContractData.doRealCustomer);

                        switch (ObjectTypeID)
                        {
                            case 1:
                                if (initData != null)
                                {
                                    param.doRegisterDraftSaleContractData.doPurchaserCustomer = initData.doPurchaserCustomer;
                                }
                                else
                                {
                                    param.doRegisterDraftSaleContractData.doPurchaserCustomer = null;
                                }

                                //if (isSameCustomer)
                                //{
                                //    param.doRegisterDraftSaleContractData.doRealCustomer =
                                //            CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(param.doRegisterDraftSaleContractData.doPurchaserCustomer);

                                //    if (param.doRegisterDraftSaleContractData.doSite != null)
                                //    {
                                //        bool isChanged = true;
                                //        if (param.doRegisterDraftSaleContractData.doRealCustomer != null)
                                //        {
                                //            if (param.doRegisterDraftSaleContractData.doSite.SiteCode == param.doRegisterDraftSaleContractData.doRealCustomer.SiteCustCode)
                                //                isChanged = false;
                                //        }
                                //        if (isChanged && CommonUtil.IsNullOrEmpty(param.doRegisterDraftSaleContractData.doSite.SiteCode) == false)
                                //            param.doRegisterDraftSaleContractData.doSite = null;
                                //    }
                                //}
                                //else
                                //{
                                //    //Check again
                                //    isSameCustomer =
                                //        CTS020_IsSameCustomer(param.doRegisterDraftSaleContractData.doPurchaserCustomer, param.doRegisterDraftSaleContractData.doRealCustomer);
                                //}

                                break;
                            case 2:
                                if (initData != null)
                                {
                                    param.doRegisterDraftSaleContractData.doRealCustomer = initData.doRealCustomer;

                                    if (param.doRegisterDraftSaleContractData.doSite != null)
                                    {
                                        bool isChanged = true;
                                        if (param.doRegisterDraftSaleContractData.doRealCustomer != null)
                                        {
                                            if (param.doRegisterDraftSaleContractData.doSite.SiteCode == param.doRegisterDraftSaleContractData.doRealCustomer.SiteCustCode)
                                                isChanged = false;
                                        }
                                        if (isChanged && CommonUtil.IsNullOrEmpty(param.doRegisterDraftSaleContractData.doSite.SiteCode) == false)
                                            param.doRegisterDraftSaleContractData.doSite = null;
                                    }
                                }
                                else
                                    param.doRegisterDraftSaleContractData.doRealCustomer = null;

                                isSameCustomer =
                                        CTS020_IsSameCustomer(param.doRegisterDraftSaleContractData.doPurchaserCustomer, param.doRegisterDraftSaleContractData.doRealCustomer);

                                break;
                            case 3:
                                if (initData != null)
                                {
                                    CommonUtil cmm = new CommonUtil();

                                    DateTime? updateDate = null;
                                    if (param.doRegisterDraftSaleContractData.doSite != null)
                                        updateDate = param.doRegisterDraftSaleContractData.doSite.UpdateDate;
                                    param.doRegisterDraftSaleContractData.doSite = initData.doSite;
                                    param.doRegisterDraftSaleContractData.doSite.UpdateDate = updateDate;
                                }
                                else
                                    param.doRegisterDraftSaleContractData.doSite = null;

                                break;
                        }

                        CTS020_ScreenData = param;
                        res.ResultData = new object[] { true, isSameCustomer };
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Event when clicking "Select process"
        /// </summary>
        /// <param name="ProcessType"></param>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        public ActionResult CTS020_SelectProcess(int ProcessType, string ContractCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    param = new CTS020_ScreenParameter();
                param.ProcessType = doDraftSaleContractData.PROCESS_TYPE.NEW;

                #region Validate business

                if (ProcessType == 2) //Add
                {
                    if (CommonUtil.IsNullOrEmpty(ContractCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                            "CTS020",
                                            MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0007,
                                            new string[] { "lblContractCode" },
                                            new string[] { "ContractCode" });
                        return Json(res);
                    }

                    #region Check exist sale contract

                    CommonUtil cmm = new CommonUtil();
                    string ContractCodeLong = cmm.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                    ISaleContractHandler shandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                    List<tbt_SaleBasic> lst = shandler.GetTbt_SaleBasic(ContractCodeLong, null, null);
                    if (lst.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0093,
                                            new string[] { ContractCode },
                                            new string[] { "ContractCode" });
                        return Json(res);
                    }
                    else
                    {
                        param.PurchaserSignerTypeCode = lst[0].PurchaserSignerTypeCode;
                        param.ContactPoint = lst[0].ContactPoint;
                        ContractCode = cmm.ConvertContractCode(lst[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                    #endregion

#if !ROUND2

                    #region Check exist installation basic data

                    IInstallationHandler ihandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    string status = ihandler.GetInstallationStatus(ContractCodeLong);
                    if (status != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                            MessageUtil.MessageList.MSG3098,
                                            new string[] { ContractCode },
                                            new string[] { "ContractCode" });
                        return Json(res);
                    }

                    #endregion

#endif
                    // Add by Jirawat Jannet on 2016-11-30
                    #region Validate one shot contract 

                    CommonUtil comUtil = new CommonUtil();
                    IContractHandler handler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;

                    var doGetSaleBasicOneShotFlag = handler.GetSaleBasicOneShotFlag(
                        comUtil.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                    
                    if (doGetSaleBasicOneShotFlag != null && doGetSaleBasicOneShotFlag.Count > 0)
                    {
                        if (doGetSaleBasicOneShotFlag[0].OneShotFlag ?? true)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3320,
                                                    new string[] { ContractCode }
                                                    , new string[] { "ContractCode" });
                            return Json(res);
                        }
                    }

                    #endregion

                    param.QuotationTargetCode = ContractCode;
                    param.ProcessType = doDraftSaleContractData.PROCESS_TYPE.ADD;
                }

                #endregion

                CTS020_ScreenData = param;
                res.ResultData = ContractCode;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Retrieve quotation data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS020_RetrieveQuotationData(CTS020_RetrieveQuotationCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    return Json(res);

                if (CommonUtil.IsNullOrEmpty(cond.QuotationTargetCode) == false)
                    cond.QuotationTargetCode = cond.QuotationTargetCode.ToUpper();
                if (CommonUtil.IsNullOrEmpty(cond.Alphabet) == false)
                    cond.Alphabet = cond.Alphabet.ToUpper();

                IDraftSaleContractHandler shandler = ServiceContainer.GetService<IDraftSaleContractHandler>() as IDraftSaleContractHandler;
                IQuotationHandler quotation = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

                // Begin Add: ny Jirawat Jannet on 2016-11-30
                if (cond.IsAddSale)
                {
                    doQuotationOneShotFlag doQuotationOneShotFlag = quotation.GetQuotationOneShotFlagData(new doGetQuotationDataCondition()
                    {
                        QuotationTargetCode = cond.QuotationTargetCode,
                        Alphabet = cond.Alphabet
                    });

                    if (doQuotationOneShotFlag.OneShotFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3321,
                                                    new string[] { cond.QuotationTargetCode }
                                                    , new string[] { "QuotationTargetCode" });
                        return Json(res);
                    }
                }
                // End Add

                doDraftSaleContractData draftData = null;
                if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE)
                {
                    draftData = param.doDraftSaleContractData;

                    #region Set Billing Target List

                    param.BillingTarget = CTS020_SetBilling(draftData.doTbt_DraftSaleBillingTarget);

                    #endregion
                }
                else if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.NEW
                    || (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.EDIT && cond.IsChangeQuotationSite == true))
                {
                    #region Validate in case Search

                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.EDIT && cond.IsChangeQuotationSite == true)
                    {
                        if (param.doDraftSaleContractData != null)
                        {
                            if (param.doDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCode != cond.QuotationTargetCodeLong)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3204,
                                                    new string[] {
                                                        cond.QuotationTargetCode,
                                                        param.doDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCodeShort
                                                    },
                                                    new string[] { "QuotationTargetCode" });
                                res.ResultData = param.doDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCodeShort;
                                return Json(res);
                            }
                        }
                    }

                    

                    #endregion
                    #region Validate

                    object[] obj = new object[1];
                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.NEW)
                        obj[0] = CommonUtil.CloneObject<CTS020_RetrieveQuotationCondition, CTS020_RetrieveQuotationCondition_New>(cond);
                    else
                        obj[0] = CommonUtil.CloneObject<CTS020_RetrieveQuotationCondition, CTS020_RetrieveQuotationCondition_EditChanged>(cond);

                    ValidatorUtil.BuildErrorMessage(res, obj);
                    if (res.IsError)
                        return Json(res);

                    #endregion
                    #region Check existing Draft rental contract data

                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.NEW)
                    {
                        List<tbt_DraftSaleContract> lst = shandler.GetTbt_DraftSaleContract(cond.QuotationTargetCodeLong);
                        if (lst.Count > 0)
                        {
                            bool isError = true;
                            MessageUtil.MessageList msg = MessageUtil.MessageList.MSG3246;

                            if (lst[0].DraftSaleContractStatus == ApprovalStatus.C_APPROVE_STATUS_WAITFORAPPROVE)
                                msg = MessageUtil.MessageList.MSG3243;
                            else if (lst[0].DraftSaleContractStatus == ApprovalStatus.C_APPROVE_STATUS_RETURNED)
                                msg = MessageUtil.MessageList.MSG3245;
                            else if (param.ProcessType != doDraftSaleContractData.PROCESS_TYPE.ADD)
                            {
                                if (lst[0].DraftSaleContractStatus == ApprovalStatus.C_APPROVE_STATUS_REJECTED)
                                    msg = MessageUtil.MessageList.MSG3244;
                                else
                                    isError = false;
                            }
                            else
                            {
                                isError = false;
                            }

                            if (isError)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                    msg,
                                                    null,
                                                    new string[] { "QuotationTargetCode" });
                                return Json(res);
                            }
                        }
                    }

                    #endregion
                    #region Retrieve Data

                    draftData = shandler.GetEntireDraftSaleContract((doDraftSaleContractCondition)cond,
                        doDraftSaleContractData.SALE_CONTRACT_MODE.QUOTATION,
                        param.ProcessType);
                    if (draftData == null)
                    {
                        string[] ctrls = new string[] { "QuotationTargetCode", "Alphabet" };
                        if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.EDIT && cond.IsChangeQuotationSite == true)
                            ctrls = new string[] { "Alphabet" };

                        res.AddErrorMessage(
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0137,
                            null,
                            ctrls);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }

                    #endregion

                    param.BillingTarget = null;

                    if (param.ProcessType == doDraftSaleContractData.PROCESS_TYPE.ADD)
                    {
                        List<tbt_DraftSaleBillingTarget> btLst = new List<tbt_DraftSaleBillingTarget>();

                        IBillingHandler bhandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                        List<BillingBasicList> bsLst = bhandler.GetBillingBasicListData(cond.QuotationTargetCodeLong, MiscType.C_CUST_TYPE);
                        if (bsLst.Count > 0)
                        {
                            foreach (BillingBasicList bs in bsLst)
                            {
                                btLst.Add(new tbt_DraftSaleBillingTarget()
                                {
                                    QuotationTargetCode = cond.QuotationTargetCodeLong,
                                    BillingTargetCode = bs.BillingTargetCode,
                                    BillingClientCode = bs.BillingClientCode,
                                    BillingOfficeCode = bs.BillingOfficeCode
                                });
                            }
                        }

                        #region Set Billing Target List

                        draftData.doTbt_DraftSaleBillingTarget = btLst;
                        param.BillingTarget = CTS020_SetBilling(draftData.doTbt_DraftSaleBillingTarget);

                        #endregion
                    }
                    else if (param.ProcessType == doDraftSaleContractData.PROCESS_TYPE.NEW)
                    {
                        param.BillingTarget = new CTS020_BillingTargetData()
                        {
                            SalePrice_ApprovalCurrencyType = draftData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType,
                            SalePrice_PartialCurrencyType = draftData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType,
                            SalePrice_AcceptanceCurrencyType = draftData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType,
                            InstallationFee_ApprovalCurrencyType = draftData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType,
                            InstallationFee_PartialCurrencyType = draftData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType,
                            InstallationFee_AcceptanceCurrencyType = draftData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType,

                            SalePrice_PaymentMethod_Approval = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER,
                            SalePrice_PaymentMethod_Partial = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER,
                            SalePrice_PaymentMethod_Acceptance = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER,
                            InstallationFee_PaymentMethod_Approval = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER,
                            InstallationFee_PaymentMethod_Partial = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER,
                            InstallationFee_PaymentMethod_Acceptance = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                        };
                    }
                }
                else
                {
                    #region Validate

                    object[] obj = new object[]
                    {
                        CommonUtil.CloneObject<CTS020_RetrieveQuotationCondition, CTS020_RetrieveQuotationCondition_Edit>(cond)
                    };

                    ValidatorUtil.BuildErrorMessage(res, obj);
                    if (res.IsError)
                        return Json(res);

                    #endregion
                    #region Retrieve Data

                    draftData = shandler.GetEntireDraftSaleContract((doDraftSaleContractCondition)cond, doDraftSaleContractData.SALE_CONTRACT_MODE.DRAFT,
                        param.ProcessType);
                    if (draftData == null)
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0137,
                            null,
                            new string[] { "QuotationTargetCode" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }

                    if (draftData.doTbt_DraftSaleContract != null)
                    {
                        if (draftData.doTbt_DraftSaleContract.SaleProcessType == ((int)doDraftSaleContractData.PROCESS_TYPE.NEW).ToString())
                            param.ProcessType = doDraftSaleContractData.PROCESS_TYPE.NEW;
                        else
                            param.ProcessType = doDraftSaleContractData.PROCESS_TYPE.ADD;
                    }

                    #endregion
                    #region Set Billing Target List

                    param.BillingTarget = CTS020_SetBilling(draftData.doTbt_DraftSaleBillingTarget);

                    #endregion
                }

                if (draftData != null)
                {
                    if (draftData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType == null)
                        draftData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    if (draftData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType == null)
                        draftData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;

                    //Add by Jutarat A. on 16102012
                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.EDIT && cond.IsChangeQuotationSite == true)
                    {
                        if (draftData.doTbt_DraftSaleContract != null)
                        {
                            param.ProcessType = doDraftSaleContractData.PROCESS_TYPE.NEW;
                            if (draftData.doTbt_DraftSaleContract.SaleProcessType == SaleProcessType.C_SALE_PROCESS_TYPE_ADD_SALE)
                            {
                                param.ProcessType = doDraftSaleContractData.PROCESS_TYPE.ADD;
                            }
                        }
                    }
                    //End Add
                    //Defafult Amount NormalInstallFee, OrderInstallFee is Null
                    if (draftData.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (draftData.doTbt_DraftSaleContract.NormalInstallFeeUsd == null)
                            draftData.doTbt_DraftSaleContract.NormalInstallFeeUsd = 0;
                    }
                    else
                    {
                        if (draftData.doTbt_DraftSaleContract.NormalInstallFee == null)
                            draftData.doTbt_DraftSaleContract.NormalInstallFee = 0;
                    }

                    if (draftData.doTbt_DraftSaleContract.OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (draftData.doTbt_DraftSaleContract.OrderInstallFeeUsd == null)
                            draftData.doTbt_DraftSaleContract.OrderInstallFeeUsd = 0;
                    }
                    else
                    {
                        if (draftData.doTbt_DraftSaleContract.OrderInstallFee == null)
                            draftData.doTbt_DraftSaleContract.OrderInstallFee = 0;
                    }

                    if(draftData.doTbt_DraftSaleContract.OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (draftData.doTbt_DraftSaleContract.OrderProductPriceUsd == null)
                            draftData.doTbt_DraftSaleContract.OrderProductPriceUsd = 0;
                    }
                    else
                    {
                        if (draftData.doTbt_DraftSaleContract.OrderProductPrice == null)
                            draftData.doTbt_DraftSaleContract.OrderProductPrice = 0;
                    }
  
                    if (draftData.doTbt_DraftSaleContract.NormalProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (draftData.doTbt_DraftSaleContract.NormalProductPriceUsd == null)
                            draftData.doTbt_DraftSaleContract.NormalProductPriceUsd = 0;
                    }
                    else
                    {
                        if (draftData.doTbt_DraftSaleContract.NormalProductPrice == null)
                            draftData.doTbt_DraftSaleContract.NormalProductPrice = 0;
                    }

                    res.ResultData = new CTS020_SpecifyQuotationData()
                    {
                        doTbt_DraftSaleContract = draftData.doTbt_DraftSaleContract,
                        doPurchaserCustomer = draftData.doPurchaserCustomer != null ? draftData.doPurchaserCustomer : new doCustomerWithGroup(),
                        doRealCustomer = draftData.doRealCustomer != null ? draftData.doRealCustomer : new doCustomerWithGroup(),
                        doSite = draftData.doSite != null ? draftData.doSite : new doSite(),
                        ProcessType = Convert.ToInt32(param.ProcessType) //Add by Jutarat A. on 16102012
                    };
                }

                param.doDraftSaleContractData = draftData;

                if (draftData != null)
                {
                    param.doRegisterDraftSaleContractData = new doDraftSaleContractData();
                    param.doRegisterDraftSaleContractData.doPurchaserCustomer =
                        CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(draftData.doPurchaserCustomer);
                    param.doRegisterDraftSaleContractData.doRealCustomer =
                        CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(draftData.doRealCustomer);
                    param.doRegisterDraftSaleContractData.doSite =
                        CommonUtil.CloneObject<doSite, doSite>(draftData.doSite);
                    param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail =
                        CommonUtil.ClonsObjectList<tbt_DraftSaleEmail, tbt_DraftSaleEmail>(draftData.doTbt_DraftSaleEmail);
                    param.doRegisterDraftSaleContractData.Mode = draftData.Mode;
                    param.doRegisterDraftSaleContractData.LastUpdateDateQuotationData = draftData.LastUpdateDateQuotationData;

                    IMasterHandler mHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                    List<tbm_Product> products = mHandler.GetTbm_Product(param.doDraftSaleContractData.doTbt_DraftSaleContract.ProductCode,
                                                                            param.doDraftSaleContractData.doTbt_DraftSaleContract.ProductTypeCode);
                    if (products != null)
                    {
                        if (products.Count > 0)
                            param.OneShotFlag = (products[0].OneShotFlag == true) ? true : false;
                    }
                }

                if (cond.IsAddSale == true)
                {
                    if (param.OneShotFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3321,
                                                new string[] {
                                                        cond.QuotationTargetCode
                                                },
                                                new string[] { "QuotationTargetCode" });
                        res.ResultData = param.doDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCodeShort;
                        return Json(res);
                    }
                }

                #region Check Same Customer mode

                if (param.doDraftSaleContractData != null)
                {
                    if (param.doDraftSaleContractData.doPurchaserCustomer != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doPurchaserCustomer.CustCode))
                        {
                            doCustomer cust1 = CommonUtil.CloneObject
                                <doCustomer, doCustomer>(param.doDraftSaleContractData.doPurchaserCustomer);

                            ICustomerMasterHandler chandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                            chandler.ValidateCustomerData(cust1, true);
                            if (cust1 != null)
                            {
                                if (cust1.ValidateCustomerData == false)
                                    param.doDraftSaleContractData.doPurchaserCustomer.ValidateCustomerData = false;
                            }
                        }
                    }
                    if (param.doDraftSaleContractData.doRealCustomer != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doRealCustomer.CustCode))
                        {
                            doCustomer cust1 = CommonUtil.CloneObject
                                <doCustomer, doCustomer>(param.doDraftSaleContractData.doRealCustomer);

                            ICustomerMasterHandler chandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                            chandler.ValidateCustomerData(cust1, true);
                            if (cust1 != null)
                            {
                                if (cust1.ValidateCustomerData == false)
                                    param.doDraftSaleContractData.doRealCustomer.ValidateCustomerData = false;
                            }
                        }
                    }
                }

                #endregion

                CTS020_ScreenData = param;
            }
            catch (ApplicationErrorException erx)
            {
                res.AddErrorMessage(erx);
                foreach (MessageModel msg in res.MessageList)
                {
                    if (msg.Code == MessageUtil.MessageList.MSG3100.ToString())
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        msg.Controls = new string[] { "QuotationTargetCode" };
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        //public ActionResult CTS020_GetFeeInformation()
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    try
        //    {
        //        List<CTS020_FeeInformation> lst = new List<CTS020_FeeInformation>();

        //        // Line 1 (Contract fee)
        //        CTS020_FeeInformation l1 = new CTS020_FeeInformation();
        //        lst.Add(l1);
        //        // Line 2 (Installation fee)
        //        CTS020_FeeInformation l2 = new CTS020_FeeInformation();
        //        lst.Add(l2);
        //        // Line 3 (Deposit fee)
        //        CTS020_FeeInformation l3 = new CTS020_FeeInformation();
        //        lst.Add(l3);

        //        CTS020_ScreenParameter param = CTS020_ScreenData;
        //        if (param != null)
        //        {
        //            if (param.doDraftSaleContractData != null)
        //            {
        //                // Line 1 (Product price)
        //                l1.Normal = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalProductPrice);
        //                if (CommonUtil.IsNullOrEmpty(l1.Normal))
        //                    l1.Normal = "0.00";

        //                l1.Order = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderProductPrice);
        //                if (CommonUtil.IsNullOrEmpty(l1.Order))
        //                    l1.Order = "0.00";

        //                // Line 2 (Installation fee)
        //                l2.Normal = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalInstallFee);
        //                if (CommonUtil.IsNullOrEmpty(l2.Normal))
        //                    l2.Normal = "0.00";

        //                l2.Order = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderInstallFee);
        //                if (CommonUtil.IsNullOrEmpty(l2.Order))
        //                    l2.Order = "0.00";

        //                // Line 3 (Sale price)
        //                l3.Normal = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.NormalSalePrice);
        //                if (CommonUtil.IsNullOrEmpty(l3.Normal))
        //                    l3.Normal = "0.00";

        //                l3.Order = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.OrderSalePrice);
        //                if (CommonUtil.IsNullOrEmpty(l3.Order))
        //                    l3.Order = "0.00";

        //                l3.Approve = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_ApproveContract);
        //                l3.Partial = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_PartialFee);
        //                l3.Acceptance = CommonUtil.TextNumeric(param.doDraftSaleContractData.doTbt_DraftSaleContract.BillingAmt_Acceptance);
        //            }
        //        }

        //        res.ResultData = CommonUtil.ConvertToXml<CTS020_FeeInformation>(lst, "\\Contract\\CTS020_FeeInformation");
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }
        //    return Json(res);
        //}
        /// <summary>
        /// Load instrument detail data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_GetInstrumentDetailData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doInstrumentDetail> lst = null;

                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doDraftSaleContractData != null)
                    {
                        if (param.doDraftSaleContractData.doTbt_DraftSaleInstrument != null)
                        {
                            lst = new List<doInstrumentDetail>();
                            foreach (tbt_DraftSaleInstrument inst in param.doDraftSaleContractData.doTbt_DraftSaleInstrument)
                            {
                                if (inst.InstrumentTypeCode != InstrumentType.C_INST_TYPE_GENERAL)
                                    continue;

                                lst.Add(CommonUtil.CloneObject<tbt_DraftSaleInstrument, doInstrumentDetail>(inst));
                            }
                        }
                    }
                }

                if (lst != null)
                {
                    lst = (
                        from x in lst
                        orderby x.InstrumentFlag descending, x.InstrumentCode
                        select x).ToList();
                }

                res.ResultData = CommonUtil.ConvertToXml<doInstrumentDetail>(lst, "\\Contract\\CTS020_InstrumentDetail");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Load E-mail data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_GetEmailList()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<tbt_DraftSaleEmail> lst = null;

                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftSaleContractData != null)
                        lst = param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail;
                }
                res.ResultData = CommonUtil.ConvertToXml<tbt_DraftSaleEmail>(lst, "\\Contract\\CTS020_Email");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Check E-mail before add data
        /// </summary>
        /// <param name="doEmail"></param>
        /// <returns></returns>
        public ActionResult CTS020_CheckBeforeAddEmailAddress(CTS020_AddEmailAddressCondition doEmail)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                bool isExist = false;
                if (doEmail.NewEmailAddressList != null)
                {
                    if (doEmail.NewEmailAddressList.Count > 0)
                    {
                        if (CommonUtil.IsNullOrEmpty(doEmail.NewEmailAddressList[0].EmailAddress) == false)
                            isExist = true;
                    }
                }
                if (isExist == false)
                {
                    res.AddErrorMessage(
                        MessageUtil.MODULE_CONTRACT,
                        "CTS020",
                        MessageUtil.MODULE_COMMON,
                        MessageUtil.MessageList.MSG0007,
                        new string[] { "lblMailAddress" },
                        new string[] { "EmailAddress" });
                    return Json(res);
                }

                #region Get Data

                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    param = new CTS020_ScreenParameter();
                if (param.doRegisterDraftSaleContractData == null)
                    param.doRegisterDraftSaleContractData = new doDraftSaleContractData();
                if (param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail == null)
                    param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail = new List<tbt_DraftSaleEmail>();

                #endregion
                #region Get Email suffix

                string emailSuffix = "";

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doSystemConfig> emlst = chandler.GetSystemConfig(ConfigName.C_EMAIL_SUFFIX);
                if (emlst.Count > 0)
                    emailSuffix = emlst[0].ConfigValue;

                #endregion

                List<tbt_DraftSaleEmail> newLst = new List<tbt_DraftSaleEmail>();

                IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                foreach (tbt_DraftSaleEmail email in doEmail.NewEmailAddressList)
                {
                    //if (email.EmailAddress.IndexOf(emailSuffix) < 0)
                    if (String.IsNullOrEmpty(email.EmailAddress) == false
                        && email.EmailAddress.ToUpper().IndexOf(emailSuffix.ToUpper()) < 0) //Modify by Jutarat A. on 18012013
                    {
                        email.EmailAddress += emailSuffix;
                    }

                    bool isError = false;

                    List<dtGetEmailAddress> lst = handler.GetEmailAddress(null, email.EmailAddress, null, null);
                    if (lst.Count <= 0)
                    {
                        if (doEmail.FromPopUp == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011,
                                                    new string[] { email.EmailAddress },
                                                    new string[] { "EmailAddress" });
                            return Json(res);
                        }
                        isError = true;
                    }
                    if (isError == false)
                    {
                        foreach (tbt_DraftSaleEmail oemail in param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail)
                        {
                            if (email.EmailAddress.ToUpper() == oemail.EmailAddress.ToUpper())
                            {
                                if (doEmail.FromPopUp == false)
                                {
                                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0107,
                                                    new string[] { email.EmailAddress },
                                                    new string[] { "EmailAddress" });
                                    return Json(res);
                                }

                                isError = true;
                                break;
                            }
                        }
                    }

                    if (isError == false)
                    {
                        tbt_DraftSaleEmail e = new tbt_DraftSaleEmail()
                        {
                            EmailAddress = lst[0].EmailAddress,
                            ToEmpNo = lst[0].EmpNo,
                            SendFlag = "0"
                        };
                        param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail.Add(e);
                        newLst.Add(e);
                    }
                }

                #region Update Data

                CTS020_ScreenData = param;

                #endregion

                res.ResultData = newLst;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Add E-mail data to session
        /// </summary>
        /// <param name="doEmail"></param>
        /// <returns></returns>
        public ActionResult CTS020_AddEmailAddressFromPopup(CTS020_AddEmailAddressCondition doEmail)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                bool isExist = false;
                if (doEmail.NewEmailAddressList != null)
                {
                    if (doEmail.NewEmailAddressList.Count > 0)
                    {
                        if (CommonUtil.IsNullOrEmpty(doEmail.NewEmailAddressList[0].EmailAddress) == false)
                            isExist = true;
                    }
                }
                if (isExist == false)
                {
                    res.AddErrorMessage(
                        MessageUtil.MODULE_CONTRACT,
                        "CTS020",
                        MessageUtil.MODULE_COMMON,
                        MessageUtil.MessageList.MSG0007,
                        new string[] { "lblMailAddress" },
                        new string[] { "EmailAddress" });
                    return Json(res);
                }

                #region Get Data

                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    param = new CTS020_ScreenParameter();
                if (param.doRegisterDraftSaleContractData == null)
                    param.doRegisterDraftSaleContractData = new doDraftSaleContractData();
                if (param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail == null)
                    param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail = new List<tbt_DraftSaleEmail>();

                #endregion
                #region Get Email suffix

                string emailSuffix = "";

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doSystemConfig> emlst = chandler.GetSystemConfig(ConfigName.C_EMAIL_SUFFIX);
                if (emlst.Count > 0)
                    emailSuffix = emlst[0].ConfigValue;

                #endregion

                List<tbt_DraftSaleEmail> newLst = new List<tbt_DraftSaleEmail>();
                param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail.Clear();
                IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                foreach (tbt_DraftSaleEmail email in doEmail.NewEmailAddressList)
                {
                    //if (email.EmailAddress.IndexOf(emailSuffix) < 0)
                    if (String.IsNullOrEmpty(email.EmailAddress) == false
                        && email.EmailAddress.ToUpper().IndexOf(emailSuffix.ToUpper()) < 0) //Modify by Jutarat A. on 18012013
                    {
                        email.EmailAddress += emailSuffix;
                    }
                  

                    List<dtGetEmailAddress> lst = handler.GetEmailAddress(null, email.EmailAddress, null, null);
                    

                    
                    tbt_DraftSaleEmail e = new tbt_DraftSaleEmail()
                    {
                        EmailAddress = lst[0].EmailAddress,
                        ToEmpNo = lst[0].EmpNo,
                        SendFlag = "0",
                        EmpNo = lst[0].EmpNo
                    };
                    param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail.Add(e);
                    newLst.Add(e);
                   
                }

                #region Update Data

                CTS020_ScreenData = param;

                #endregion

                res.ResultData = newLst;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Remove E-mail data from session
        /// </summary>
        /// <param name="doEmail"></param>
        /// <returns></returns>
        public ActionResult CTS020_RemoveEmailAddress(tbt_DraftSaleEmail doEmail)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftSaleContractData != null)
                    {
                        if (param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail != null)
                        {
                            foreach (tbt_DraftSaleEmail oemail in param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail)
                            {
                                if (doEmail.EmailAddress == oemail.EmailAddress)
                                {
                                    param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail.Remove(oemail);
                                    CTS020_ScreenData = param;
                                    break;
                                }
                            }
                        }
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
        /// Retrieve customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS020_RetrieveCustomer(CTS020_RetrieveCustomerCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Validate Data

                if (false == ModelState.IsValid)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }


                #endregion
                #region Get Data

                doCustomerWithGroup custDo = null;
                CommonUtil cmm = new CommonUtil();
                ICustomerMasterHandler handler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                string CustCodeLong = cmm.ConvertCustCode(cond.CustCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                List<doCustomerWithGroup> lst = handler.GetCustomerWithGroup(CustCodeLong);
                if (lst.Count > 0)
                    custDo = lst[0];

                if (custDo != null)
                {
                    #region Update Customer Data

                    CTS020_ScreenParameter param = CTS020_ScreenData;
                    if (param == null)
                        param = new CTS020_ScreenParameter();
                    if (param.doRegisterDraftSaleContractData == null)
                        param.doRegisterDraftSaleContractData = new doDraftSaleContractData();

                    if (cond.CustType == 1)
                        param.doRegisterDraftSaleContractData.doPurchaserCustomer = custDo;
                    else
                        param.doRegisterDraftSaleContractData.doRealCustomer = custDo;

                    bool isSameCustomer =
                        CTS020_IsSameCustomer(param.doRegisterDraftSaleContractData.doPurchaserCustomer, param.doRegisterDraftSaleContractData.doRealCustomer);

                    CTS020_ScreenData = param;

                    #endregion

                    res.ResultData = new object[] { custDo, isSameCustomer };
                }
                else
                {
                    MessageUtil.MessageList msgCode = MessageUtil.MessageList.MSG0068;
                    if (cond.CustType == 2)
                        msgCode = MessageUtil.MessageList.MSG0078;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, msgCode, null, new string[] { "CustCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Load purchaser customer group data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_GetPurchaserCustomerGroupData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<dtCustomeGroupData> lst = null;

                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftSaleContractData != null)
                    {
                        if (param.doRegisterDraftSaleContractData.doPurchaserCustomer != null)
                            lst = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustomerGroupData;
                    }
                }
                res.ResultData = CommonUtil.ConvertToXml<dtCustomeGroupData>(lst, "\\Contract\\CTS020_CustomerGroupData");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Load real customer group data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_GetRealCustomerGroupData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<dtCustomeGroupData> lst = null;

                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftSaleContractData != null)
                    {
                        if (param.doRegisterDraftSaleContractData.doRealCustomer != null)
                            lst = param.doRegisterDraftSaleContractData.doRealCustomer.CustomerGroupData;
                    }
                }
                res.ResultData = CommonUtil.ConvertToXml<dtCustomeGroupData>(lst, "\\Contract\\CTS020_CustomerGroupData");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Copy contract customer data to read customer data
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_CopyCustomer()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                doCustomerWithGroup doCustomer = null;

                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftSaleContractData != null)
                        doCustomer = param.doRegisterDraftSaleContractData.doPurchaserCustomer;
                }
                if (doCustomer != null)
                {
                    bool isSameSite = true;

                    #region Update Data

                    param.doRegisterDraftSaleContractData.doRealCustomer = CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(doCustomer);
                    if (param.doRegisterDraftSaleContractData.doSite != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doRegisterDraftSaleContractData.doSite.SiteCode) == false)
                        {
                            string rSiteCustCode = param.doRegisterDraftSaleContractData.doSite.SiteCode;
                            if (rSiteCustCode.IndexOf("-") > 0)
                                rSiteCustCode = rSiteCustCode.Substring(0, rSiteCustCode.IndexOf("-"));

                            if (rSiteCustCode != param.doRegisterDraftSaleContractData.doRealCustomer.SiteCustCode)
                            {
                                param.doRegisterDraftSaleContractData.doSite = null;
                                isSameSite = false;
                            }
                        }
                    }
                    CTS020_ScreenData = param;

                    #endregion

                    List<object> objRes = new List<object>();
                    objRes.Add(param.doRegisterDraftSaleContractData.doRealCustomer);
                    objRes.Add(isSameSite);
                    res.ResultData = objRes;
                }
                else
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0073);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Retrieve site data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS020_RetrieveSiteData(CTS020_RetrieveSiteCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                #region Get Site Data

                doSite siteDo = null;

                CommonUtil cmm = new CommonUtil();
                ISiteMasterHandler handler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;

                string fullSiteCustCode = CommonUtil.TextCodeName(
                                                cmm.ConvertSiteCode(cond.SiteCustCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                                cond.SiteNo,
                                                "-");
                string longCustCode = cmm.ConvertCustCode(cond.CustCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                List<doSite> lst = handler.GetSite(fullSiteCustCode, longCustCode);
                if (lst.Count > 0)
                    siteDo = lst[0];

                if (siteDo != null)
                {
                    #region Update Data

                    CTS020_ScreenParameter param = CTS020_ScreenData;
                    if (param == null)
                        param = new CTS020_ScreenParameter();
                    if (param.doRegisterDraftSaleContractData == null)
                        param.doRegisterDraftSaleContractData = new doDraftSaleContractData();

                    param.doRegisterDraftSaleContractData.doSite = siteDo;
                    CTS020_ScreenData = param;

                    #endregion

                    res.ResultData = siteDo;
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0071, null, new string[] { "SiteCustCodeNo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check real customer code has value when clicking seach site 
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS020_CheckRealCustomer(CTS020_RetrieveCustomerCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                ValidatorUtil.BuildErrorMessage(res, this);
                if (res.IsError)
                    return Json(res);
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Copy contract customer data or real customer data to site data
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="initData"></param>
        /// <returns></returns>
        public ActionResult CTS020_CopySiteInfomation(CTS020_RetrieveCustomerCondition cond, tbt_DraftSaleContract initData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                doSite siteDo = null;
                doCustomer doCustomer = null;
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftSaleContractData != null)
                    {
                        if (cond.CustType == 1)
                            doCustomer = param.doRegisterDraftSaleContractData.doPurchaserCustomer;
                        else if (cond.CustType == 2)
                            doCustomer = param.doRegisterDraftSaleContractData.doRealCustomer;
                    }
                }
                if (cond.CustType.ToString() == SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET)
                {
                    if (doCustomer == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0073);
                        return Json(res);
                    }

                    siteDo = CommonUtil.CloneObject<doCustomer, doSite>(doCustomer);
                    if (siteDo != null)
                    {
                        siteDo.SiteNameEN = CommonUtil.TextList(new string[] { doCustomer.CustFullNameEN, initData.BranchNameEN }, " ");
                        siteDo.SiteNameLC = CommonUtil.TextList(new string[] { doCustomer.CustFullNameLC, initData.BranchNameLC }, " ");
                    }
                }
                else if (cond.CustType.ToString() == SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_REAL_CUST)
                {
                    if (doCustomer == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0074);
                        return Json(res);
                    }

                    siteDo = CommonUtil.CloneObject<doCustomer, doSite>(doCustomer);
                    if (siteDo != null)
                    {
                        siteDo.SiteNameEN = doCustomer.CustFullNameEN;
                        siteDo.SiteNameLC = doCustomer.CustFullNameLC;
                    }
                }

                #region Update Data

                if (siteDo != null)
                {
                    param.doRegisterDraftSaleContractData.doSite = siteDo;
                    param.doRegisterDraftSaleContractData.doSite.ValidateSiteData = false;
                    CTS020_ScreenData = param;
                }

                #endregion

                res.ResultData = siteDo;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve billing target data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS020_RetrieveBillingTargetDetailData(CTS020_RetrieveBillingTargetCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                CommonUtil cmm = new CommonUtil();
                IBillingMasterHandler handler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

                CTS020_BillingTargetData billingTargetData = null;

#if !ROUND2
                billingTargetData = new CTS020_BillingTargetData();
                billingTargetData.BillingTargetCode = cmm.ConvertBillingTargetCode(cond.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IBillingInterfaceHandler bihandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                List<tbt_BillingTarget> billingTData = bihandler.GetBillingTarget(billingTargetData.BillingTargetCode);
                if (billingTData.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, 
                                        MessageUtil.MessageList.MSG0011, 
                                        new string[]{ cond.BillingTargetCode });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                billingTargetData.BillingClientCode = billingTData[0].BillingClientCode;
                billingTargetData.BillingOfficeCode = billingTData[0].BillingOfficeCode;

#endif

                if (billingTargetData != null)
                {
                    List<dtBillingClientData> billingData = handler.GetBillingClient(billingTargetData.BillingClientCode);
                    if (billingData.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0138);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }

                    billingTargetData.BillingClient = CommonUtil.CloneObject<dtBillingClientData, tbm_BillingClient>(billingData[0]);
                    if (billingTargetData.BillingClient != null)
                    {
                        #region Load Master Data

                        IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                        List<tbm_Region> rgLst = mhandler.GetTbm_Region();
                        if (rgLst.Count > 0)
                        {
                            foreach (tbm_Region rg in rgLst)
                            {
                                if (rg.RegionCode == billingTargetData.BillingClient.RegionCode)
                                {
                                    billingTargetData.BillingClient.Nationality = rg.Nationality;
                                    break;
                                }
                            }
                        }

                        List<tbm_BusinessType> bLst = mhandler.GetTbm_BusinessType();
                        if (bLst.Count > 0)
                        {
                            foreach (tbm_BusinessType b in bLst)
                            {
                                if (b.BusinessTypeCode == billingTargetData.BillingClient.BusinessTypeCode)
                                {
                                    billingTargetData.BillingClient.BusinessTypeName = b.BusinessTypeName;
                                    break;
                                }
                            }
                        }

                        #endregion
                    }

                    #region Update Temp

                    CTS020_ScreenParameter param = CTS020_ScreenData;
                    if (param == null)
                        param = new CTS020_ScreenParameter();
                    param.BillingTarget = billingTargetData;
                    CTS020_ScreenData = param;

                    #endregion

                    res.ResultData = billingTargetData;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Retrieve billing client data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS020_RetrieveBillingClientDetailData(CTS020_RetrieveBillingClientCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                CommonUtil cmm = new CommonUtil();
                IBillingMasterHandler handler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

                CTS020_BillingTargetData billingTargetData = new CTS020_BillingTargetData();
                billingTargetData.BillingClientCode = cmm.ConvertBillingClientCode(cond.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                
                if (billingTargetData != null)
                {
                    List<dtBillingClientData> billingData = handler.GetBillingClient(billingTargetData.BillingClientCode);
                    if (billingData.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0011,
                                        new string[] { cond.BillingClientCode });
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }

                    billingTargetData.BillingClient = CommonUtil.CloneObject<dtBillingClientData, tbm_BillingClient>(billingData[0]);
                    if (billingTargetData.BillingClient != null)
                    {
                        #region Load Master Data

                        IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                        List<tbm_Region> rgLst = mhandler.GetTbm_Region();
                        if (rgLst.Count > 0)
                        {
                            foreach (tbm_Region rg in rgLst)
                            {
                                if (rg.RegionCode == billingTargetData.BillingClient.RegionCode)
                                {
                                    billingTargetData.BillingClient.Nationality = rg.Nationality;
                                    break;
                                }
                            }
                        }

                        List<tbm_BusinessType> bLst = mhandler.GetTbm_BusinessType();
                        if (bLst.Count > 0)
                        {
                            foreach (tbm_BusinessType b in bLst)
                            {
                                if (b.BusinessTypeCode == billingTargetData.BillingClient.BusinessTypeCode)
                                {
                                    billingTargetData.BillingClient.BusinessTypeName = b.BusinessTypeName;
                                    break;
                                }
                            }
                        }

                        #endregion
                    }

                    #region Update Temp

                    CTS020_ScreenParameter param = CTS020_ScreenData;
                    if (param == null)
                        param = new CTS020_ScreenParameter();
                    param.BillingTarget = billingTargetData;
                    CTS020_ScreenData = param;

                    #endregion

                    res.ResultData = billingTargetData;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Copy billing information from contract target customer, real customer or site
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS020_CopyBillingNameAddressData(CTS020_CopyBillingNameAddressCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS020_BillingTargetData billingTargetData = null;

                doDraftSaleContractData draftData = null;
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftSaleContractData != null)
                        draftData = param.doRegisterDraftSaleContractData;
                }
                if (draftData != null)
                {
                    string strIDNo = null; //Add by Jutarat A. on 17122013
                    if (cond.CopyMode == CTS020_CopyBillingNameAddressCondition.COPY_BILLING_NAME_ADDRESS_MODE.CONTRACT_TARGET)
                    {
                        if (param.doRegisterDraftSaleContractData.doPurchaserCustomer != null)
                        {
                            //Add by Jutarat A. on 17122013
                            if (param.doRegisterDraftSaleContractData.doPurchaserCustomer.DummyIDFlag != true)
                                strIDNo = param.doRegisterDraftSaleContractData.doPurchaserCustomer.IDNo;
                            //End Add

                            billingTargetData = new CTS020_BillingTargetData();
                            billingTargetData.BillingClient = new tbm_BillingClient()
                            {
                                NameEN = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustNameEN,
                                NameLC = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustNameLC,
                                FullNameEN = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustFullNameEN,
                                FullNameLC = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustFullNameLC,
                                AddressEN = param.doRegisterDraftSaleContractData.doPurchaserCustomer.AddressFullEN,
                                AddressLC = param.doRegisterDraftSaleContractData.doPurchaserCustomer.AddressFullLC,
                                RegionCode = param.doRegisterDraftSaleContractData.doPurchaserCustomer.RegionCode,
                                Nationality = param.doRegisterDraftSaleContractData.doPurchaserCustomer.Nationality,
                                PhoneNo = param.doRegisterDraftSaleContractData.doPurchaserCustomer.PhoneNo,
                                BusinessTypeCode = param.doRegisterDraftSaleContractData.doPurchaserCustomer.BusinessTypeCode,
                                BusinessTypeName = param.doRegisterDraftSaleContractData.doPurchaserCustomer.BusinessTypeName,
                                IDNo = strIDNo, //param.doRegisterDraftSaleContractData.doPurchaserCustomer.IDNo, //Modify by Jutarat A. on 17122013
                                CustTypeCode = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustTypeCode,
                                CompanyTypeCode = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CompanyTypeCode
                            };
                        }
                    }
                    else if (cond.CopyMode == CTS020_CopyBillingNameAddressCondition.COPY_BILLING_NAME_ADDRESS_MODE.BRANCH_CONTRACT_TARGET)
                    {
                        if (param.doRegisterDraftSaleContractData.doPurchaserCustomer != null)
                        {
                            //Add by Jutarat A. on 17122013
                            if (param.doRegisterDraftSaleContractData.doPurchaserCustomer.DummyIDFlag != true)
                                strIDNo = param.doRegisterDraftSaleContractData.doPurchaserCustomer.IDNo;
                            //End Add

                            billingTargetData = new CTS020_BillingTargetData();
                            billingTargetData.BillingClient = new tbm_BillingClient()
                            {
                                NameEN = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustNameEN,
                                NameLC = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustNameLC,
                                FullNameEN = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustFullNameEN,
                                FullNameLC = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustFullNameLC,
                                //BranchNameEN = cond.BranchNameEN, //Comment by Jutarat A. on 17122013
                                //BranchNameLC = cond.BranchNameLC, //Comment by Jutarat A. on 17122013
                                AddressEN = cond.BranchAddressEN,
                                AddressLC = cond.BranchAddressLC,
                                RegionCode = param.doRegisterDraftSaleContractData.doPurchaserCustomer.RegionCode,
                                Nationality = param.doRegisterDraftSaleContractData.doPurchaserCustomer.Nationality,
                                BusinessTypeCode = param.doRegisterDraftSaleContractData.doPurchaserCustomer.BusinessTypeCode,
                                BusinessTypeName = param.doRegisterDraftSaleContractData.doPurchaserCustomer.BusinessTypeName,
                                IDNo = strIDNo, //param.doRegisterDraftSaleContractData.doPurchaserCustomer.IDNo, //Modify by Jutarat A. on 17122013
                                CustTypeCode = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustTypeCode,
                                CompanyTypeCode = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CompanyTypeCode
                            };
                        }
                    }
                    else if (cond.CopyMode == CTS020_CopyBillingNameAddressCondition.COPY_BILLING_NAME_ADDRESS_MODE.REAL_CUSTOMERE)
                    {
                        if (param.doRegisterDraftSaleContractData.doRealCustomer != null)
                        {
                            //Add by Jutarat A. on 17122013
                            if (param.doRegisterDraftSaleContractData.doRealCustomer.DummyIDFlag != true)
                                strIDNo = param.doRegisterDraftSaleContractData.doRealCustomer.IDNo;
                            //End Add

                            billingTargetData = new CTS020_BillingTargetData();
                            billingTargetData.BillingClient = new tbm_BillingClient()
                            {
                                NameEN = param.doRegisterDraftSaleContractData.doRealCustomer.CustNameEN,
                                NameLC = param.doRegisterDraftSaleContractData.doRealCustomer.CustNameLC,
                                FullNameEN = param.doRegisterDraftSaleContractData.doRealCustomer.CustFullNameEN,
                                FullNameLC = param.doRegisterDraftSaleContractData.doRealCustomer.CustFullNameLC,
                                AddressEN = param.doRegisterDraftSaleContractData.doRealCustomer.AddressFullEN,
                                AddressLC = param.doRegisterDraftSaleContractData.doRealCustomer.AddressFullLC,
                                RegionCode = param.doRegisterDraftSaleContractData.doRealCustomer.RegionCode,
                                Nationality = param.doRegisterDraftSaleContractData.doRealCustomer.Nationality,
                                PhoneNo = param.doRegisterDraftSaleContractData.doRealCustomer.PhoneNo,
                                BusinessTypeCode = param.doRegisterDraftSaleContractData.doRealCustomer.BusinessTypeCode,
                                BusinessTypeName = param.doRegisterDraftSaleContractData.doRealCustomer.BusinessTypeName,
                                IDNo = strIDNo, //param.doRegisterDraftSaleContractData.doRealCustomer.IDNo, //Modify by Jutarat A. on 17122013
                                CustTypeCode = param.doRegisterDraftSaleContractData.doRealCustomer.CustTypeCode,
                                CompanyTypeCode = param.doRegisterDraftSaleContractData.doRealCustomer.CompanyTypeCode
                            };
                        }
                    }
                    else if (cond.CopyMode == CTS020_CopyBillingNameAddressCondition.COPY_BILLING_NAME_ADDRESS_MODE.SITE)
                    {
                        if (param.doRegisterDraftSaleContractData.doRealCustomer != null
                            && param.doRegisterDraftSaleContractData.doSite != null)
                        {
                            //Add by Jutarat A. on 17122013
                            if (param.doRegisterDraftSaleContractData.doRealCustomer.DummyIDFlag != true)
                                strIDNo = param.doRegisterDraftSaleContractData.doRealCustomer.IDNo;
                            //End Add

                            billingTargetData = new CTS020_BillingTargetData();
                            billingTargetData.BillingClient = new tbm_BillingClient()
                            {
                                NameEN = param.doRegisterDraftSaleContractData.doRealCustomer.CustNameEN,
                                NameLC = param.doRegisterDraftSaleContractData.doRealCustomer.CustNameLC,
                                FullNameEN = param.doRegisterDraftSaleContractData.doRealCustomer.CustFullNameEN,
                                FullNameLC = param.doRegisterDraftSaleContractData.doRealCustomer.CustFullNameLC,
                                //BranchNameEN = param.doRegisterDraftSaleContractData.doSite.SiteNameEN, //Comment by Jutarat A. on 17122013
                                //BranchNameLC = param.doRegisterDraftSaleContractData.doSite.SiteNameLC, //Comment by Jutarat A. on 17122013
                                AddressEN = param.doRegisterDraftSaleContractData.doSite.AddressFullEN,
                                AddressLC = param.doRegisterDraftSaleContractData.doSite.AddressFullLC,
                                RegionCode = param.doRegisterDraftSaleContractData.doRealCustomer.RegionCode,
                                Nationality = param.doRegisterDraftSaleContractData.doRealCustomer.Nationality,
                                PhoneNo = param.doRegisterDraftSaleContractData.doSite.PhoneNo,
                                BusinessTypeCode = param.doRegisterDraftSaleContractData.doRealCustomer.BusinessTypeCode,
                                BusinessTypeName = param.doRegisterDraftSaleContractData.doRealCustomer.BusinessTypeName,
                                IDNo = strIDNo, //param.doRegisterDraftSaleContractData.doRealCustomer.IDNo, //Modify by Jutarat A. on 17122013
                                CustTypeCode = param.doRegisterDraftSaleContractData.doRealCustomer.CustTypeCode,
                                CompanyTypeCode = param.doRegisterDraftSaleContractData.doRealCustomer.CompanyTypeCode
                            };
                        }
                    }

                    #region Update Temp

                    param.BillingTarget = billingTargetData;
                    CTS020_ScreenData = param;

                    #endregion
                }

                res.ResultData = billingTargetData;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Get temp billing clicent data from session
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_GetBillingClientData()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    param = new CTS020_ScreenParameter();

                //if (param.BillingTarget != null)
                //{
                //    if (param.BillingTarget.BillingClient != null)
                //        param.BillingTarget.BillingClient.BillingClientCode = param.BillingTarget.BillingClient.BillingClientCodeShort;
                //}

                CTS020_BillingTargetData temp = param.BillingTarget;
                if (temp == null)
                {
                    temp = new CTS020_BillingTargetData();
                    temp.BillingClient = new tbm_BillingClient();
                }

                res.ResultData = temp;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Set temp billing client data to session
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public ActionResult CTS020_UpdateBillingClientData(tbm_BillingClient temp)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Update Data

                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    param = new CTS020_ScreenParameter();
                if (param.BillingTarget == null)
                    param.BillingTarget = new CTS020_BillingTargetData();

                CommonUtil cmm = new CommonUtil();
                temp.BillingClientCode = cmm.ConvertBillingClientCode(temp.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                if (CTS020_IsSameBillingClient(param.BillingTarget.BillingClient, temp) == false)
                {
                    param.BillingTarget.BillingClient = temp;
                    param.BillingTarget.BillingClientCode = null;
                    param.BillingTarget.BillingOfficeCode = null;
                    param.BillingTarget.BillingTargetCode = null;

                    CTS020_ScreenData = param;
                }

                #endregion

                res.ResultData = param.BillingTarget;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Clear temp billing target data
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_ClearBillingTargetDetailData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                    param.BillingTarget = null;
                CTS020_ScreenData = param;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Event when clicking Register
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="maintenance"></param>
        /// <returns></returns>
        public ActionResult CTS020_RegisterSaleContractData(CTS020_RegisterSaleContractData contract, CTS020_UpdateBillingTargetData billingData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    param = new CTS020_ScreenParameter();

                #region Check is Suspending

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion
                #region Check Mandatory

                if (param.BillingTarget == null)
                    param.BillingTarget = new CTS020_BillingTargetData();
                if (param.BillingTarget.BillingClient == null)
                    param.BillingTarget.BillingClient = new tbm_BillingClient();

                ValidatorUtil validator = new ValidatorUtil(this);


                //Comment by Jutarat A. on 16102012
                //if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE)
                //{
                //    if (CommonUtil.IsNullOrEmpty(contract.BICContractCode))
                //    {
                //        validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                //                                    "CTS020",
                //                                    MessageUtil.MODULE_COMMON,
                //                                    MessageUtil.MessageList.MSG0007,
                //                                    "BICContractCode",
                //                                    "lblBICContractCode",
                //                                    "BICContractCode",
                //                                    "12");
                //    }
                //}
                //End Comment

                if (contract.OrderProductPrice == null
                    && contract.OrderProductPriceUsd == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                "CTS020",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "OrderProductPrice",
                                                "lblOrderProductPrice",
                                                "OrderProductPrice",
                                                "6");
                }
                if (contract.OrderInstallFee == null
                    && contract.OrderInstallFeeUsd == null)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                "CTS020",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "OrderInstallFee",
                                                "lblOrderInstallationFee",
                                                "OrderInstallFee",
                                                "7");
                }

                bool isEmailEmpty = true;
                if (param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail != null)
                {
                    if (param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail.Count > 0)
                        isEmailEmpty = false;
                }
                if (isEmailEmpty)
                {
                    validator.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS020",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "draftSaleEmail",
                            "headerEmail",
                            "Email",
                            "13");
                }

                if (param.doRegisterDraftSaleContractData.doPurchaserCustomer == null)
                {
                    validator.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS020",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "doPurchaserCustomer",
                            "lblPurchaser",
                            null,
                            "14");
                }
                if (param.doRegisterDraftSaleContractData.doRealCustomer == null)
                {
                    validator.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS020",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "doRealCustomer",
                            "lblRealCustomerInfo",
                            null,
                            "20");
                }
                if (param.doRegisterDraftSaleContractData.doSite == null)
                {
                    validator.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS020",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "doSite",
                            "lblSiteInfo",
                            null,
                            "21");
                }

                //Add by Jutarat A. on 25122013
                CTS020_UpdateBillingClientBranchName validateBranchName = null;
                if (param.BillingTarget.BillingClient.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC
                    || param.BillingTarget.BillingClient.CustTypeCode == CustomerType.C_CUST_TYPE_ASSOCIATION
                    || param.BillingTarget.BillingClient.CustTypeCode == CustomerType.C_CUST_TYPE_PUBLIC_OFFICE)
                {
                    /*if (CommonUtil.IsNullOrEmpty(param.BillingTarget.BillingClient.BranchNameEN))
                        validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                  "CTS020",
                                                  MessageUtil.MODULE_COMMON,
                                                  MessageUtil.MessageList.MSG0007,
                                                  "BillingBranchNameEN",
                                                  "lblBranchNameEN",
                                                  "BillingBranchNameEN");

                    if (CommonUtil.IsNullOrEmpty(param.BillingTarget.BillingClient.BranchNameLC))
                        validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                  "CTS020",
                                                  MessageUtil.MODULE_COMMON,
                                                  MessageUtil.MessageList.MSG0007,
                                                  "BillingBranchNameLC",
                                                  "lblBranchNameLC",
                                                  "BillingBranchNameLC");*/

                    // 2016/05/17 Remove branch name validation
                    //if (CommonUtil.IsNullOrEmpty(param.BillingTarget.BillingClient.BranchNameEN) 
                    //    || CommonUtil.IsNullOrEmpty(param.BillingTarget.BillingClient.BranchNameLC)
                    //    || CommonUtil.IsNullOrEmpty(param.BillingTarget.BillingClient.IDNo))
                    //    validateBranchName = CommonUtil.CloneObject<tbm_BillingClient, CTS020_UpdateBillingClientBranchName>(param.BillingTarget.BillingClient);
                }
                //End Add

                ValidatorUtil.BuildErrorMessage(res, validator, new object[]
                    {
                        CommonUtil.CloneObject<tbm_BillingClient, CTS020_UpdateBillingClient>(param.BillingTarget.BillingClient)
                        // 2016/05/17 Remove branch name validation
                        //, validateBranchName //Add by Jutarat A. on 02012014
                    });

                if (res.IsError)
                    return Json(res);


                #endregion
                #region Mapping Data

                CTS020_MappingDraftContractData(param, contract, billingData);

                #endregion
                #region Validate Business

                if (CTS020_ValidateBusiness(res,
                                            param.doRegisterDraftSaleContractData,
                                            param.BillingTarget) == false)
                {
                    return Json(res);
                }

                //Add by Jutarat A. on 02012014
                if (param.BillingTarget.BillingClient.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC
                    || param.BillingTarget.BillingClient.CustTypeCode == CustomerType.C_CUST_TYPE_ASSOCIATION
                    || param.BillingTarget.BillingClient.CustTypeCode == CustomerType.C_CUST_TYPE_PUBLIC_OFFICE)
                {
                    if (CommonUtil.IsNullOrEmpty(param.BillingTarget.BillingClient.IDNo) == false && param.BillingTarget.BillingClient.IDNo.Length != 15)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        "CTS020",
                                        MessageUtil.MODULE_MASTER,
                                        MessageUtil.MessageList.MSG1060,
                                        null,
                                        new string[] { "BillingIDNo" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }
                //End Add
                #endregion
                #region Validate Customer

                CommonUtil cmm = new CommonUtil();
                ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                if (param.doRegisterDraftSaleContractData.doPurchaserCustomer != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustCode) == true)
                    {
                        if (custhandler.CheckDuplicateCustomer_IDNo(param.doRegisterDraftSaleContractData.doPurchaserCustomer) == true)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1003);
                            return Json(res);
                        }
                    }
                }
                if (param.doRegisterDraftSaleContractData.doRealCustomer != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.doRegisterDraftSaleContractData.doRealCustomer.CustCode) == true)
                    {
                        if (custhandler.CheckDuplicateCustomer_IDNo(param.doRegisterDraftSaleContractData.doRealCustomer) == true)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1003);
                            return Json(res);
                        }
                    }
                }

                #endregion

                #region Validate Business for Warning

                if (contract.NormalProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    //if (contract.NormalSalePriceUsd > 0
                    //    && contract.OrderInstallFeeUsd > 0
                    //    && CommonUtil.IsNullOrEmpty(contract.ApproveNo1) == false)
                    //{
                    //    decimal? fee30 = (contract.NormalSalePriceUsd * 0.3M);
                    //    decimal? fee1000 = (contract.NormalSalePriceUsd * 10.0M);
                    //    if (contract.OrderInstallFeeUsd <= fee30)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3239);
                    //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    //    }
                    //    if (contract.OrderInstallFeeUsd >= fee1000)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3239);
                    //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    //    }
                    //}
                    //else 

                    //if (contract.NormalSalePrice > 0 // Comment by jirawat jannet on 2016-12-01
                    if (contract.NormalProductPriceUsd > 0  // Add by jirawat jannet on 2016-12-01
                            && (contract.OrderProductPriceUsd ?? 0) == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3337);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }
                else
                {
                    //if (contract.NormalSalePrice > 0
                    //    && contract.OrderInstallFee > 0
                    //    && CommonUtil.IsNullOrEmpty(contract.ApproveNo1) == false)
                    //{

                    //    decimal? fee30 = (contract.NormalSalePrice * 0.3M);
                    //    decimal? fee1000 = (contract.NormalSalePrice * 10.0M);
                    //    if (contract.OrderInstallFee <= fee30)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3239);
                    //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    //    }
                    //    if (contract.OrderInstallFee >= fee1000)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3239);
                    //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    //    }
                    //}
                    //else 

                    //if (contract.NormalSalePrice > 0 // Comment by jirawat jannet on 2016-12-01
                    if (contract.NormalProductPrice > 0  // Add by jirawat jannet on 2016-12-01
                            && (contract.OrderProductPrice ?? 0) == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3337);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }

                }


                if (contract.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.NormalInstallFeeUsd > 0
                            && (contract.OrderInstallFeeUsd ?? 0) == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3338);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }
                else
                {
                    if (contract.NormalInstallFee > 0
                            && (contract.OrderInstallFee ?? 0) == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3338);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                //#region Contract fee

                //if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFee > 0
                //    && (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee)
                //            || param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee == 0)
                //    && CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.ApproveNo1) == false)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3083);
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                //}
                //if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFee > 0
                //    && param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee > 0
                //    && CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.ApproveNo1) == false)
                //{
                //    decimal? fee10 = (param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFee * 0.1M);
                //    decimal? fee1000 = (param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFee * 10.0M);
                //    if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee <= fee10)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3084);
                //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                //    }
                //    if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee >= fee1000)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3084);
                //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                //    }
                //}

                //#endregion
                //#region Out of regulations about contract duration

                //if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.IrregulationContractDurationFlag == "1"
                //    && (param.doDraftRentalContractData.doTbt_DraftRentalContrat.ContractDurationMonth > 36
                //        || param.doDraftRentalContractData.doTbt_DraftRentalContrat.AutoRenewMonth > 12))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3085);
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                //}

                //#endregion
                //#region Contract payment term

                //if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.CalDailyFeeStatus != CalculationDailyFeeType.C_CALC_DAILY_FEE_TYPE_CALENDAR)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3086);
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                //}

                //#endregion

                #region Validate 2-22, 2-23 (Customer Document): 

                if (contract.NormalProductPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.NormalProductPriceUsd > 0 && contract.OrderProductPriceUsd > 0
                        && (
                            contract.OrderProductPriceUsd <= (contract.NormalProductPriceUsd ?? 0) * 0.3M
                            || contract.OrderProductPriceUsd >= (contract.NormalProductPriceUsd ?? 0) * 10M))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3352);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }
                else
                {
                    if (contract.NormalProductPrice > 0 && contract.OrderProductPrice > 0
                        && (
                            contract.OrderProductPrice <= (contract.NormalProductPrice ?? 0) * 0.3M
                            || contract.OrderProductPrice >= (contract.NormalProductPrice ?? 0) * 10M))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3352);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                #endregion

                #region Validate 2-24, 2-25 (Customer Document): 

                if (contract.NormalInstallFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.NormalInstallFeeUsd > 0 && contract.OrderInstallFeeUsd > 0
                        && (
                            contract.OrderInstallFeeUsd <= (contract.NormalInstallFeeUsd ?? 0) * 0.3M
                            || contract.OrderInstallFeeUsd >= (contract.NormalInstallFeeUsd ?? 0) * 10M))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3353);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }
                else
                {
                    if (contract.NormalInstallFee > 0 && contract.OrderInstallFee > 0
                        && (
                            contract.OrderInstallFee <= (contract.NormalInstallFee ?? 0) * 0.3M
                            || contract.OrderInstallFee >= (contract.NormalInstallFee ?? 0) * 10M))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3353);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                #endregion 

                #region Validate 2-42 (Customer Document): 

                if (contract.BillingAmt_ApproveContract > 0
                    || contract.BillingAmt_ApproveContractUsd > 0)
                {
                    if (contract.OrderProductPriceCurrencyType != contract.BillingAmt_ApproveContractCurrencyType)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3354);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                #endregion

                #region Validate 2-43 (Customer Document): 

                if (contract.BillingAmt_PartialFee > 0
                    || contract.BillingAmt_PartialFeeUsd > 0)
                {
                    if (contract.OrderProductPriceCurrencyType != contract.BillingAmt_PartialFeeCurrencyType)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3355);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                #endregion

                #region Validate 2-44 (Customer Document): 

                if (contract.BillingAmt_Acceptance > 0
                    || contract.BillingAmt_AcceptanceUsd > 0)
                {
                    if (contract.OrderProductPriceCurrencyType != contract.BillingAmt_AcceptanceCurrencyType)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3356);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                #endregion

                #region Validate 2-45 (Customer Document): 

                if (contract.BillingAmtInstallation_ApproveContract > 0
                    || contract.BillingAmtInstallation_ApproveContractUsd > 0)
                {
                    if (contract.OrderInstallFeeCurrencyType != contract.BillingAmtInstallation_ApproveContractCurrencyType)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3357);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                #endregion

                #region Validate 2-46 (Customer Document): 

                if (contract.BillingAmtInstallation_PartialFee > 0
                    || contract.BillingAmtInstallation_PartialFeeUsd > 0)
                {
                    if (contract.OrderInstallFeeCurrencyType != contract.BillingAmtInstallation_PartialFeeCurrencyType)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3358);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                #endregion

                #region Validate 2-47 (Customer Document): 

                if (contract.BillingAmtInstallation_Acceptance > 0
                    || contract.BillingAmtInstallation_AcceptanceUsd > 0)
                {
                    if (contract.OrderInstallFeeCurrencyType != contract.BillingAmtInstallation_AcceptanceCurrencyType)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3359);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                }

                #endregion



                #region Line-up type

                if (param.doRegisterDraftSaleContractData.doTbt_DraftSaleInstrument != null)
                {
                    foreach (tbt_DraftSaleInstrument inst in param.doRegisterDraftSaleContractData.doTbt_DraftSaleInstrument)
                    {
                        if (inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE
                            || inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_ONE_TIME
                            || inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_TEMPORARY)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3296);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                            break;
                        }
                    }
                }

                #endregion

                #endregion

                CTS020_ScreenData = param;
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        private decimal getPriceByCurrency(decimal? localPrice, decimal? usdPrice, string currencyType)
        {
            if (currencyType == CurrencyUtil.C_CURRENCY_US) return usdPrice ?? 0;
            else return localPrice ?? 0;
        }
        /// <summary>
        /// Event when clicking Comfirm (Step 1: check purchaser customer is duplicate?)
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_ConfirmSaleContractData_P1()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    param = new CTS020_ScreenParameter();

                #region Check is Suspending

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion
                #region Validate Business

                if (CTS020_ValidateBusiness(res, param.doRegisterDraftSaleContractData, param.BillingTarget) == false)
                    return Json(res);

                #endregion


                doDraftSaleContractData draft = param.doRegisterDraftSaleContractData;

                #region Validate quotation is registered?

                IDraftSaleContractHandler shandler = ServiceContainer.GetService<IDraftSaleContractHandler>() as IDraftSaleContractHandler;
                List<tbt_DraftSaleContract> sList = shandler.GetTbt_DraftSaleContract(draft.doTbt_DraftSaleContract.QuotationTargetCodeFull);
                if (sList.Count > 0)
                {
                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.NEW)
                    {

                        if (sList[0].DraftSaleContractStatus == ApprovalStatus.C_APPROVE_STATUS_WAITFORAPPROVE)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3243);
                            return Json(res);
                        }
                        else if (sList[0].DraftSaleContractStatus == ApprovalStatus.C_APPROVE_STATUS_REJECTED)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3244);
                            return Json(res);
                        }
                        else if (sList[0].DraftSaleContractStatus == ApprovalStatus.C_APPROVE_STATUS_RETURNED)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3245);
                            return Json(res);
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3246);
                            return Json(res);
                        }
                    }
                    else if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.EDIT)
                    {
                        if (sList[0].DraftSaleContractStatus != ApprovalStatus.C_APPROVE_STATUS_RETURNED)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3100);
                            return Json(res);
                        }
                    }
                }

                #endregion

                CommonUtil cmm = new CommonUtil();
                ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                if (draft.doPurchaserCustomer != null)
                {
                    if (CommonUtil.IsNullOrEmpty(draft.doPurchaserCustomer.CustCode))
                    {
                        if (custhandler.CheckDuplicateCustomer_IDNo(draft.doPurchaserCustomer) == true)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1003);
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }
                    }
                    if (draft.doPurchaserCustomer.IsNameLCChanged == true
                        && custhandler.CheckDuplicateCustomer_CustNameLC(draft.doPurchaserCustomer) == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1004);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.ResultData = 1;
                        return Json(res);
                    }
                }

                return CTS020_ConfirmSaleContractData_P2();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Event when clicking Confirm (Step 2: check real customer is duplcate?)
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_ConfirmSaleContractData_P2()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    param = new CTS020_ScreenParameter();

                doDraftSaleContractData draft = param.doRegisterDraftSaleContractData;

                CommonUtil cmm = new CommonUtil();
                ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                if (draft.doRealCustomer != null)
                {
                    if (CommonUtil.IsNullOrEmpty(draft.doRealCustomer.CustCode))
                    {
                        if (custhandler.CheckDuplicateCustomer_IDNo(draft.doRealCustomer) == true)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1003);
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }
                    }
                    if (draft.doRealCustomer.IsNameLCChanged == true
                        && custhandler.CheckDuplicateCustomer_CustNameLC(draft.doRealCustomer) == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1004);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.ResultData = 2;
                        return Json(res);
                    }
                }

                return CTS020_ConfirmSaleContractData_P3();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Event when clicking Confirm (Step 3)
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_ConfirmSaleContractData_P3()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    param = new CTS020_ScreenParameter();

                doDraftSaleContractData draft = param.doRegisterDraftSaleContractData;

                #region Check Data is changed?

                CTS020_RetrieveQuotationCondition_Edit cond = new CTS020_RetrieveQuotationCondition_Edit();
                cond.QuotationTargetCode = draft.doTbt_DraftSaleContract.QuotationTargetCodeShort;

                IDraftSaleContractHandler shandler = ServiceContainer.GetService<IDraftSaleContractHandler>() as IDraftSaleContractHandler;
                List<tbt_DraftSaleContract> contractLst = shandler.GetTbt_DraftSaleContract(cond.QuotationTargetCodeLong);
                if (contractLst.Count > 0)
                {
                    if (CommonUtil.IsNullOrEmpty(param.doRegisterDraftSaleContractData.doTbt_DraftSaleContract.UpdateDate)
                        || contractLst[0].UpdateDate > param.doRegisterDraftSaleContractData.doTbt_DraftSaleContract.UpdateDate)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }

                #endregion

                CommonUtil cmm = new CommonUtil();
                ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                #region Update customer

                bool isSameCustomer = CTS020_IsSameCustomer(draft.doPurchaserCustomer, draft.doRealCustomer);
                    if (draft.doPurchaserCustomer != null)
                    {
                        doCustomerTarget custTarget = new doCustomerTarget();
                        custTarget.doCustomer = draft.doPurchaserCustomer;
                        custTarget.dtCustomerGroup = CommonUtil.ClonsObjectList<dtCustomeGroupData, dtCustomerGroup>(draft.doPurchaserCustomer.CustomerGroupData);

                        custTarget.doSite = null;
                        if (isSameCustomer)
                            custTarget.doSite = draft.doSite;

                        custTarget = custhandler.ManageCustomerTarget(custTarget);

                        if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE)
                        {
                            custhandler.ManageCustomerInformation(custTarget.doCustomer.CustCode);
                        }

                        draft.doTbt_DraftSaleContract.PurchaserCustCode = custTarget.doCustomer.CustCode;

                        if (isSameCustomer)
                        {
                            draft.doTbt_DraftSaleContract.RealCustomerCustCode = custTarget.doCustomer.CustCode;
                            draft.doTbt_DraftSaleContract.SiteCode = null;
                            if (custTarget.doSite != null)
                                draft.doTbt_DraftSaleContract.SiteCode = custTarget.doSite.SiteCode;

                            if (draft.doRealCustomer != null)
                            {
                                draft.doRealCustomer.IDNo = custTarget.doCustomer.IDNo;
                                draft.doRealCustomer.CustStatus = custTarget.doCustomer.CustStatus;
                                draft.doRealCustomer.CustStatusName = custTarget.doCustomer.CustStatusName;
                            }
                        }
                    }
                    if (isSameCustomer == false && draft.doRealCustomer != null)
                    {
                        doCustomerTarget realCust = new doCustomerTarget();
                        realCust.doCustomer = draft.doRealCustomer;
                        realCust.dtCustomerGroup = CommonUtil.ClonsObjectList<dtCustomeGroupData, dtCustomerGroup>(draft.doRealCustomer.CustomerGroupData);
                        realCust.doSite = draft.doSite;
                        realCust = custhandler.ManageCustomerTarget(realCust);

                        if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE)
                        {
                            custhandler.ManageCustomerInformation(realCust.doCustomer.CustCode);
                        }

                        draft.doTbt_DraftSaleContract.RealCustomerCustCode = realCust.doCustomer.CustCode;

                        draft.doTbt_DraftSaleContract.SiteCode = null;
                        if (realCust.doSite != null)
                            draft.doTbt_DraftSaleContract.SiteCode = realCust.doSite.SiteCode;
                    }

                    #endregion
                    #region Billing Target

                    param.doRegisterDraftSaleContractData.doTbt_DraftSaleBillingTarget = new List<tbt_DraftSaleBillingTarget>();
                    if (param.BillingTarget != null)
                    {
                        IBillingMasterHandler bhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

                        int sequenceNo = 1;
                        CTS020_BillingTargetData billing = param.BillingTarget;

                        if (CommonUtil.IsNullOrEmpty(billing.BillingClientCode))
                            billing.BillingClientCode = bhandler.ManageBillingClient(billing.BillingClient);

                        #region Sale Price

                        for (int idx = 0; idx < 3; idx++)
                        {
                            tbt_DraftSaleBillingTarget bt = new tbt_DraftSaleBillingTarget();
                            bt.QuotationTargetCode = param.doRegisterDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCode;
                            bt.SequenceNo = sequenceNo;
                            bt.BillingTargetCode = billing.BillingTargetCode;
                            bt.BillingClientCode = billing.BillingClientCode;
                            bt.BillingOfficeCode = billing.BillingOfficeCode;
                            bt.DocLanguage = billing.DocLanguage; //Add by Jutarat A. on 20122013
                            //bt.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE;
                            bt.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE;


                            if (idx == 0)
                            {
                                bt.BillingTiming = BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT;
                                bt.PayMethod = billing.SalePrice_PaymentMethod_Approval;
                                bt.BillingAmtCurrencyType = billing.SalePrice_ApprovalCurrencyType;
                                bt.BillingAmtUsd = billing.SalePrice_ApprovalUsd;
                                bt.BillingAmt = billing.SalePrice_Approval;
                            }
                            
                            else if (idx == 1)
                            {
                                bt.BillingTiming = BillingTiming.C_BILLING_TIMING_PARTIAL;
                                bt.PayMethod = billing.SalePrice_PaymentMethod_Partial;
                                bt.BillingAmtCurrencyType = billing.SalePrice_PartialCurrencyType;
                                bt.BillingAmtUsd = billing.SalePrice_PartialUsd;
                                bt.BillingAmt = billing.SalePrice_Partial;

                            }

                            else if (idx == 2)
                            {
                                bt.BillingTiming = BillingTiming.C_BILLING_TIMING_ACCEPTANCE;
                                bt.PayMethod = billing.SalePrice_PaymentMethod_Acceptance;
                                bt.BillingAmtCurrencyType = billing.SalePrice_AcceptanceCurrencyType;
                                bt.BillingAmtUsd = billing.SalePrice_AcceptanceUsd != null ? billing.SalePrice_AcceptanceUsd : null;
                                bt.BillingAmt = billing.SalePrice_Acceptance != null ? billing.SalePrice_Acceptance : null;
                            }
                            if (bt.BillingAmt != null || bt.BillingAmtUsd != null)
                            {
                                param.doRegisterDraftSaleContractData.doTbt_DraftSaleBillingTarget.Add(bt);
                                sequenceNo++;
                            }
                        }

                        #endregion
                        #region Installation Fee

                        for (int idx = 0; idx < 3; idx++)
                        {
                            tbt_DraftSaleBillingTarget bt = new tbt_DraftSaleBillingTarget();
                            bt.QuotationTargetCode = param.doRegisterDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCode;
                            bt.SequenceNo = sequenceNo;
                            bt.BillingTargetCode = billing.BillingTargetCode;
                            bt.BillingClientCode = billing.BillingClientCode;
                            bt.BillingOfficeCode = billing.BillingOfficeCode;
                            bt.DocLanguage = billing.DocLanguage; //Add by Jutarat A. on 20122013
                            bt.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE;
                            

                            if (idx == 0) //Approve
                            {
                                bt.BillingTiming = BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT;
                                bt.PayMethod = billing.InstallationFee_PaymentMethod_Approval;
                                bt.BillingAmtCurrencyType = billing.InstallationFee_ApprovalCurrencyType;
                                bt.BillingAmtUsd = billing.InstallationFee_ApprovalUsd;
                                bt.BillingAmt = billing.InstallationFee_Approval;
                               
                            }
                            else if (idx == 1) //Partial
                            {
                                bt.BillingTiming = BillingTiming.C_BILLING_TIMING_PARTIAL;
                                bt.PayMethod = billing.InstallationFee_PaymentMethod_Partial;
                                bt.BillingAmtCurrencyType = billing.InstallationFee_PartialCurrencyType;
                                bt.BillingAmtUsd = billing.InstallationFee_PartialUsd;
                                bt.BillingAmt = billing.InstallationFee_Partial;
                                
                            }
                            else if (idx == 2) //Acceptance
                            {
                                bt.BillingTiming = BillingTiming.C_BILLING_TIMING_ACCEPTANCE;
                                bt.PayMethod = billing.InstallationFee_PaymentMethod_Acceptance;
                                bt.BillingAmtCurrencyType = billing.InstallationFee_AcceptanceCurrencyType;
                                bt.BillingAmtUsd = billing.InstallationFee_AcceptanceUsd != null ? billing.InstallationFee_AcceptanceUsd : null;
                                bt.BillingAmt = billing.InstallationFee_Acceptance != null ? billing.InstallationFee_Acceptance : null;
                            }

                            if (bt.BillingAmt != null || bt.BillingAmtUsd != null)
                            {
                                param.doRegisterDraftSaleContractData.doTbt_DraftSaleBillingTarget.Add(bt);
                                sequenceNo++;
                            }
                        }

                        #endregion
                    }

                    #endregion
                    #region Update Draft Sale Data

                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.doTbt_DraftSaleContract.ApproveContractDate = null;

                    //IDraftSaleContractHandler shandler = ServiceContainer.GetService<IDraftSaleContractHandler>() as IDraftSaleContractHandler;
                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.NEW)
                    {
                        if (contractLst.Count > 0)
                        {
                            shandler.EditDraftSaleContractData(draft);
                        }
                        else
                        {
                            draft.doTbt_DraftSaleContract.RegisterDate = dsTrans.dtOperationData.ProcessDateTime;
                            shandler.CreateDraftSaleContractData(draft);
                        }
                    }
                    else
                    {
                        if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.EDIT)
                            shandler.EditDraftSaleContractData(draft);
                        else if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE)
                        {
                            draft.doTbt_DraftSaleContract.ApproveContractDate = dsTrans.dtOperationData.ProcessDateTime;
                            CTS020_ApproveContract(draft);
                        }
                    }

                    #endregion
                    #region Update Quotation Data

                    doUpdateQuotationData qData = new doUpdateQuotationData()
                    {
                        QuotationTargetCode = draft.doTbt_DraftSaleContract.QuotationTargetCode,
                        Alphabet = draft.doTbt_DraftSaleContract.Alphabet,
                        LastUpdateDate = draft.LastUpdateDateQuotationData,
                        ContractCode = null,
                        ActionTypeCode = ActionType.C_ACTION_TYPE_DRAFT
                    };

                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE)
                    {
                        qData.ContractCode = draft.doTbt_DraftSaleContract.ContractCode;
                        qData.ActionTypeCode = ActionType.C_ACTION_TYPE_APPROVE;
                    }

                    IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    qhandler.UpdateQuotationData(qData);


                    #endregion
                    #region Update Data

                    param.BillingTarget = CTS020_SetBilling(draft.doTbt_DraftSaleBillingTarget);
                    CTS020_ScreenData = param;

                    #endregion

                    bool isComplete = true;
                    if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.APPROVE)
                    {
                        #region Update to CTS030


                        CTS030_UpdateDataFromChildPage(param.CallerKey,
                                                        draft.doTbt_DraftSaleContract.QuotationTargetCode,
                                                        draft.doTbt_DraftSaleContract.ContractCode,
                                                        ApprovalStatus.C_APPROVE_STATUS_APPROVED);

                        #endregion
                        #region Send Email

                        try
                        {
                            if (param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail != null)
                            {
                                List<string> mailAddress = new List<string>();
                                foreach (tbt_DraftSaleEmail e in param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail)
                                {
                                    mailAddress.Add(e.EmailAddress);
                                }
                                SendMail(CTS020_GenerateApproveMailTemplate(draft), mailAddress);
                            }
                        }
                        catch (Exception)
                        {
                            isComplete = false;
                        }

                        #endregion

                        if (isComplete)
                            res.ResultData = CallScreenURL(param, true);
                    }
                    else
                    {
                        CTS020_RegisterResult registerResult = new CTS020_RegisterResult()
                        {
                            PurchaserCustCode = cmm.ConvertCustCode(draft.doTbt_DraftSaleContract.PurchaserCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                            RealCustomerCustCode = cmm.ConvertCustCode(draft.doTbt_DraftSaleContract.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                            SiteCode = cmm.ConvertSiteCode(draft.doTbt_DraftSaleContract.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT)
                        };

                        if (draft.doPurchaserCustomer != null)
                        {
                            registerResult.PurchaserIDNo = draft.doPurchaserCustomer.IDNo;
                            registerResult.PurchaserStatusCodeName = draft.doPurchaserCustomer.CustStatusCodeName;
                        }
                        if (draft.doRealCustomer != null)
                        {
                            registerResult.RealCustomerIDNo = draft.doRealCustomer.IDNo;
                            registerResult.RealCustomerStatusCodeName = draft.doRealCustomer.CustStatusCodeName;
                        }

                        if (draft.doTbt_DraftSaleBillingTarget != null)
                        {
                            if (draft.doTbt_DraftSaleBillingTarget.Count > 0)
                            {
                                registerResult.BillingTargetCode = cmm.ConvertBillingTargetCode(draft.doTbt_DraftSaleBillingTarget[0].BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                                registerResult.BillingClientCode = cmm.ConvertBillingClientCode(draft.doTbt_DraftSaleBillingTarget[0].BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                            }
                        }
                        res.ResultData = new object[]{
                            MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3265),
                            registerResult
                        };
                    }

                    scope.Complete();

                    if (isComplete == false)
                    {
                        res.ResultData = new object[]{
                                MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3232),
                                CallScreenURL(param, true)
                            };
                        return Json(res);
                    }
                    }
                }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Event when clicking Reject (Step 1)
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_RejectSaleContractData_P1()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                #region Check is Suspending

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion

                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3189);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Event when clicking Reject (Step 2)
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_RejectSaleContractData_P2()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    param = new CTS020_ScreenParameter();

                if (param.doDraftSaleContractData != null)
                {
                    #region Check Data is changed?

                    CTS020_RetrieveQuotationCondition_Edit cond = new CTS020_RetrieveQuotationCondition_Edit();
                    cond.QuotationTargetCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCodeShort;

                    IDraftSaleContractHandler shandler = ServiceContainer.GetService<IDraftSaleContractHandler>() as IDraftSaleContractHandler;
                    List<tbt_DraftSaleContract> contractLst = shandler.GetTbt_DraftSaleContract(cond.QuotationTargetCodeLong);
                    if (contractLst.Count > 0)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.UpdateDate)
                        || contractLst[0].UpdateDate > param.doDraftSaleContractData.doTbt_DraftSaleContract.UpdateDate)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }
                    }

                    #endregion

                    using (TransactionScope scope = new TransactionScope())
                    {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                        param.doDraftSaleContractData.doTbt_DraftSaleContract.DraftSaleContractStatus = ApprovalStatus.C_APPROVE_STATUS_REJECTED;
                        param.doDraftSaleContractData.doTbt_DraftSaleContract.ApproveContractDate = dsTrans.dtOperationData.ProcessDateTime;

                        //IDraftSaleContractHandler shandler = ServiceContainer.GetService<IDraftSaleContractHandler>() as IDraftSaleContractHandler;
                        shandler.UpdateTbt_DraftSaleContract(param.doDraftSaleContractData.doTbt_DraftSaleContract);

                        #region Lock Quotation Data

                        IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                        qhandler.LockQuotation(
                            param.doDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCode,
                            param.doDraftSaleContractData.doTbt_DraftSaleContract.Alphabet,
                            LockStyle.C_LOCK_STYLE_ALL);

                        #endregion

                        /*Comment by Jutarat A. on 25062013 (No cancel data)
                        #region Update Quotation Data

                        doUpdateQuotationData qData = new doUpdateQuotationData()
                        {
                            QuotationTargetCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCode,
                            Alphabet = param.doDraftSaleContractData.doTbt_DraftSaleContract.Alphabet,
                            LastUpdateDate = param.doDraftSaleContractData.LastUpdateDateQuotationData,
                            ContractCode = null,
                            ActionTypeCode = ActionType.C_ACTION_TYPE_CANCEL
                        };
                        qhandler.UpdateQuotationData(qData);

                        #endregion*/

                        scope.Complete();
                        }

                        #region Update to CTS030

                        CTS030_UpdateDataFromChildPage(param.CallerKey,
                                                    param.doDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCode,
                                                    param.doDraftSaleContractData.doTbt_DraftSaleContract.ContractCode,
                                                    ApprovalStatus.C_APPROVE_STATUS_REJECTED);

                    #endregion
                    #region Send Email

                    try
                    {
                        if (param.doDraftSaleContractData.doTbt_DraftSaleEmail != null)
                        {
                            List<string> mailAddress = new List<string>();
                            foreach (tbt_DraftSaleEmail e in param.doDraftSaleContractData.doTbt_DraftSaleEmail)
                            {
                                mailAddress.Add(e.EmailAddress);
                            }
                            SendMail(CTS020_GenerateRejectMailTemplate(param.doDraftSaleContractData), mailAddress);
                        }
                    }
                    catch (Exception)
                    {
                        res.ResultData = new object[]{
                                MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3232),
                                CallScreenURL(param, true)
                            };
                        return Json(res);
                    }

                    #endregion

                    res.ResultData = CallScreenURL(param, true);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Event when clicking Return (Step 1)
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_ReturnSaleContractData_P1()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                #region Check is Suspending

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion

                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3191);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Event when clicking Return (Step 2)
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_ReturnSaleContractData_P2()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param == null)
                    param = new CTS020_ScreenParameter();

                if (param.doDraftSaleContractData != null)
                {
                    #region Check Data is changed?

                    CTS020_RetrieveQuotationCondition_Edit cond = new CTS020_RetrieveQuotationCondition_Edit();
                    cond.QuotationTargetCode = param.doDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCodeShort;

                    IDraftSaleContractHandler shandler = ServiceContainer.GetService<IDraftSaleContractHandler>() as IDraftSaleContractHandler;
                    List<tbt_DraftSaleContract> contractLst = shandler.GetTbt_DraftSaleContract(cond.QuotationTargetCodeLong);
                    if (contractLst.Count > 0)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doDraftSaleContractData.doTbt_DraftSaleContract.UpdateDate)
                        || contractLst[0].UpdateDate > param.doDraftSaleContractData.doTbt_DraftSaleContract.UpdateDate)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }
                    }

                    #endregion

                    using (TransactionScope scope = new TransactionScope())
                    {
                        dsTransDataModel dsTrans = CommonUtil.dsTransData;
                        param.doDraftSaleContractData.doTbt_DraftSaleContract.DraftSaleContractStatus = ApprovalStatus.C_APPROVE_STATUS_RETURNED;
                        param.doDraftSaleContractData.doTbt_DraftSaleContract.ApproveContractDate = dsTrans.dtOperationData.ProcessDateTime;

                        //IDraftSaleContractHandler shandler = ServiceContainer.GetService<IDraftSaleContractHandler>() as IDraftSaleContractHandler;
                        shandler.UpdateTbt_DraftSaleContract(param.doDraftSaleContractData.doTbt_DraftSaleContract);

                    scope.Complete();
                    }

                    #region Update to CTS030

                    CTS030_UpdateDataFromChildPage(param.CallerKey,
                                                    param.doDraftSaleContractData.doTbt_DraftSaleContract.QuotationTargetCode,
                                                    param.doDraftSaleContractData.doTbt_DraftSaleContract.ContractCode,
                                                    ApprovalStatus.C_APPROVE_STATUS_RETURNED);

                    #endregion

                    res.ResultData = CallScreenURL(param, true);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Event when clicking Close
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS020_CloseSaleContract()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                if (param != null)
                {
                    CTS030_UpdateDataFromChildPage(param.CallerKey, null, null, null);
                    res.ResultData = CallScreenURL(param, true);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region Methods

        /// <summary>
        /// Get/Set data in session
        /// </summary>
        private CTS020_ScreenParameter CTS020_ScreenData
        {
            get
            {
                return GetScreenObject<CTS020_ScreenParameter>();
            }
            set
            {
                UpdateScreenObject(value);
            }
        }
        /// <summary>
        /// Mapping Billing target data
        /// </summary>
        /// <param name="draftBillingList"></param>
        /// <returns></returns>
        private CTS020_BillingTargetData CTS020_SetBilling(List<tbt_DraftSaleBillingTarget> draftBillingList)
        {
            try
            {
                CTS020_BillingTargetData billing = null;
                if (draftBillingList != null)
                {
                    IBillingMasterHandler bhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

                    foreach (tbt_DraftSaleBillingTarget db in draftBillingList)
                    {
                        /* --- Check Is Exist --- */
                        if (billing == null)
                        {
                            billing = new CTS020_BillingTargetData()
                            {
                                QuotationTargetCode = db.QuotationTargetCode,
                                BillingClientCode = db.BillingClientCode,
                                BillingOfficeCode = db.BillingOfficeCode,
                                BillingTargetCode = db.BillingTargetCode
                                ,DocLanguage = db.DocLanguage //Add by Jutarat A. on 20122013
                            };

                            if (CommonUtil.IsNullOrEmpty(billing.BillingClientCode) == false)
                            {
                                List<dtBillingClientData> bc = bhandler.GetBillingClient(billing.BillingClientCode);
                                if (bc.Count > 0)
                                    billing.BillingClient = CommonUtil.CloneObject<dtBillingClientData, tbm_BillingClient>(bc[0]);
                            }
                        }

                        //if (db.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE)
                        if (db.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE)
                        {
                            if (db.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                            {
                                billing.SalePrice_ApprovalCurrencyType = db.BillingAmtCurrencyType;
                                billing.SalePrice_Approval = db.BillingAmt;
                                billing.SalePrice_ApprovalUsd = db.BillingAmtUsd;
                                billing.SalePrice_PaymentMethod_Approval = db.PayMethod;
                            }
                            else if (db.BillingTiming == BillingTiming.C_BILLING_TIMING_PARTIAL)
                            {
                                billing.SalePrice_PartialCurrencyType = db.BillingAmtCurrencyType;
                                billing.SalePrice_Partial = db.BillingAmt;
                                billing.SalePrice_PartialUsd = db.BillingAmtUsd;
                                billing.SalePrice_PaymentMethod_Partial = db.PayMethod;
                            }
                            else if (db.BillingTiming == BillingTiming.C_BILLING_TIMING_ACCEPTANCE)
                            {
                                billing.SalePrice_AcceptanceCurrencyType = db.BillingAmtCurrencyType;
                                billing.SalePrice_Acceptance = db.BillingAmt;
                                billing.SalePrice_AcceptanceUsd = db.BillingAmtUsd;
                                billing.SalePrice_PaymentMethod_Acceptance = db.PayMethod;
                            }
                        }
                        if (db.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE)
                        {
                            if (db.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                            {
                                billing.InstallationFee_ApprovalCurrencyType = db.BillingAmtCurrencyType;
                                billing.InstallationFee_Approval = db.BillingAmt;
                                billing.InstallationFee_ApprovalUsd = db.BillingAmtUsd;
                                billing.InstallationFee_PaymentMethod_Approval = db.PayMethod;
                            }
                            else if (db.BillingTiming == BillingTiming.C_BILLING_TIMING_PARTIAL)
                            {
                                billing.InstallationFee_PartialCurrencyType = db.BillingAmtCurrencyType;
                                billing.InstallationFee_Partial = db.BillingAmt;
                                billing.InstallationFee_PartialUsd = db.BillingAmtUsd;
                                billing.InstallationFee_PaymentMethod_Partial = db.PayMethod;
                            }
                            else if (db.BillingTiming == BillingTiming.C_BILLING_TIMING_ACCEPTANCE)
                            {
                                billing.InstallationFee_AcceptanceCurrencyType = db.BillingAmtCurrencyType;
                                billing.InstallationFee_Acceptance = db.BillingAmt;
                                billing.InstallationFee_AcceptanceUsd = db.BillingAmtUsd;
                                billing.InstallationFee_PaymentMethod_Acceptance = db.PayMethod;
                            }
                        }
                    }
                }

                return billing;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Mapping object data
        /// </summary>
        /// <param name="param"></param>
        /// <param name="contract"></param>
        /// <param name="maintenance"></param>
        private void CTS020_MappingDraftContractData(CTS020_ScreenParameter param, CTS020_RegisterSaleContractData contract, CTS020_BillingTargetData billingData)
        {
            try
            {
                if (param.doRegisterDraftSaleContractData == null)
                    param.doRegisterDraftSaleContractData = new doDraftSaleContractData();

                #region Draft sale contract

                param.doRegisterDraftSaleContractData.doTbt_DraftSaleContract =
                    CommonUtil.CloneObject<tbt_DraftSaleContract, tbt_DraftSaleContract>(param.doDraftSaleContractData.doTbt_DraftSaleContract);
                tbt_DraftSaleContract draftContract = param.doRegisterDraftSaleContractData.doTbt_DraftSaleContract;

                draftContract.QuotationTargetCode = contract.QuotationTargetCodeLong;
                draftContract.Alphabet = contract.Alphabet;
                draftContract.SaleType = contract.SaleType;

                draftContract.SaleProcessType = SaleProcessType.C_SALE_PROCESS_TYPE_NEW_SALE;
                if (param.ProcessType == doDraftSaleContractData.PROCESS_TYPE.ADD)
                    draftContract.SaleProcessType = SaleProcessType.C_SALE_PROCESS_TYPE_ADD_SALE;

                if (param.doRegisterDraftSaleContractData.doPurchaserCustomer != null)
                    draftContract.PurchaserCustCode = param.doRegisterDraftSaleContractData.doPurchaserCustomer.CustCode;

                draftContract.PurchaserSignerTypeCode = contract.PurchaserSignerTypeCode;
                draftContract.BranchNameEN = contract.BranchNameEN;
                draftContract.BranchNameLC = contract.BranchNameLC;
                draftContract.BranchAddressEN = contract.BranchAddressEN;
                draftContract.BranchAddressLC = contract.BranchAddressLC;

                if (param.doDraftSaleContractData.doTbt_DraftSaleContract != null)
                {
                    draftContract.PurchaserMemo = param.doDraftSaleContractData.doTbt_DraftSaleContract.PurchaserMemo;
                    draftContract.RealCustomerMemo = param.doDraftSaleContractData.doTbt_DraftSaleContract.RealCustomerMemo;
                }

                if (param.doRegisterDraftSaleContractData.doRealCustomer != null)
                    draftContract.RealCustomerCustCode = param.doRegisterDraftSaleContractData.doRealCustomer.CustCode;

                draftContract.ContactPoint = contract.ContactPoint;

                if (param.doRegisterDraftSaleContractData.doSite != null)
                    draftContract.SiteCode = param.doRegisterDraftSaleContractData.doSite.SiteCode;

                if (param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.NEW
                    || param.ScreenMode == CTS020_ScreenParameter.SCREEN_MODE.EDIT)
                {
                    draftContract.DraftSaleContractStatus = ApprovalStatus.C_APPROVE_STATUS_WAITFORAPPROVE;
                    draftContract.ContractCode = null;
                }
                else
                {
                    draftContract.DraftSaleContractStatus = ApprovalStatus.C_APPROVE_STATUS_APPROVED;
                    draftContract.ContractCode = param.ContractCode;
                }

                draftContract.ProjectCode = contract.ProjectCode;
                draftContract.ContractOfficeCode = contract.ContractOfficeCode;
                draftContract.OperationOfficeCode = contract.OperationOfficeCode;
                draftContract.SalesOfficeCode = contract.OperationOfficeCode;
                draftContract.ExpectedInstallCompleteDate = contract.ExpectedInstallCompleteDate;
                draftContract.ExpectedAcceptanceAgreeDate = contract.ExpectedAcceptanceAgreeDate;

                #region Order Product Price

                draftContract.OrderProductPriceCurrencyType = contract.OrderProductPriceCurrencyType;
                draftContract.OrderProductPrice = contract.OrderProductPrice;
                draftContract.OrderProductPriceUsd = contract.OrderProductPriceUsd;

                #endregion
                #region Product Price Acceptance

                draftContract.BillingAmt_AcceptanceCurrencyType = contract.BillingAmt_AcceptanceCurrencyType;
                draftContract.BillingAmt_Acceptance = contract.BillingAmt_Acceptance;
                draftContract.BillingAmt_AcceptanceUsd = contract.BillingAmt_AcceptanceUsd;

                #endregion
                #region Product Price  Approve Contract

                draftContract.BillingAmt_ApproveContractCurrencyType = contract.BillingAmt_ApproveContractCurrencyType;
                draftContract.BillingAmt_ApproveContract = contract.BillingAmt_ApproveContract;
                draftContract.BillingAmt_ApproveContractUsd = contract.BillingAmt_ApproveContractUsd;

                #endregion
                #region Product Price Partial Fee

                draftContract.BillingAmt_PartialFeeCurrencyType = contract.BillingAmt_PartialFeeCurrencyType;
                draftContract.BillingAmt_PartialFee = contract.BillingAmt_PartialFee;
                draftContract.BillingAmt_PartialFeeUsd = contract.BillingAmt_PartialFeeUsd;

                #endregion
                #region Order Install Fee

                draftContract.OrderInstallFeeCurrencyType = contract.OrderInstallFeeCurrencyType;
                draftContract.OrderInstallFee = contract.OrderInstallFee;
                draftContract.OrderInstallFeeUsd = contract.OrderInstallFeeUsd;

                #endregion
                #region Installation Acceptance

                draftContract.BillingAmtInstallation_AcceptanceCurrencyType = contract.BillingAmtInstallation_AcceptanceCurrencyType;
                draftContract.BillingAmtInstallation_Acceptance = contract.BillingAmtInstallation_Acceptance;
                draftContract.BillingAmtInstallation_AcceptanceUsd = contract.BillingAmtInstallation_AcceptanceUsd;

                #endregion
                #region Installation Approve Contract

                draftContract.BillingAmtInstallation_ApproveContractCurrencyType = contract.BillingAmtInstallation_ApproveContractCurrencyType;
                draftContract.BillingAmtInstallation_ApproveContract = contract.BillingAmtInstallation_ApproveContract;
                draftContract.BillingAmtInstallation_ApproveContractUsd = contract.BillingAmtInstallation_ApproveContractUsd;

                #endregion
                #region Installation Partial Fee

                draftContract.BillingAmtInstallation_PartialFeeCurrencyType = contract.BillingAmtInstallation_PartialFeeCurrencyType;
                draftContract.BillingAmtInstallation_PartialFee = contract.BillingAmtInstallation_PartialFee;
                draftContract.BillingAmtInstallation_PartialFeeUsd = contract.BillingAmtInstallation_PartialFeeUsd;

                #endregion

                //draftContract.OrderSalePrice = contract.OrderSalePrice;

                draftContract.DistributedInstallTypeCode = contract.DistributedInstallTypeCode;
                draftContract.DistributedOriginCode = contract.DistributedOriginCode;
                draftContract.ConnectionFlag = contract.IsConnectTargetCode;

                CommonUtil cmm = new CommonUtil();
                draftContract.ConnectTargetCode = cmm.ConvertContractCode(contract.ConnectTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                draftContract.SalesmanEmpNo1 = contract.SalesmanEmpNo1;
                draftContract.SalesmanEmpNo2 = contract.SalesmanEmpNo2;
                draftContract.SalesmanEmpNo3 = contract.SalesmanEmpNo3;
                draftContract.SalesmanEmpNo4 = contract.SalesmanEmpNo4;
                draftContract.SalesmanEmpNo5 = contract.SalesmanEmpNo5;
                draftContract.SalesmanEmpNo6 = contract.SalesmanEmpNo6;
                draftContract.SalesmanEmpNo7 = contract.SalesmanEmpNo7;
                draftContract.SalesmanEmpNo8 = contract.SalesmanEmpNo8;
                draftContract.SalesmanEmpNo9 = contract.SalesmanEmpNo9;
                draftContract.SalesmanEmpNo10 = contract.SalesmanEmpNo10;

                #region Bid Guarantee Amount 1

                draftContract.BidGuaranteeAmount1CurrencyType = contract.BidGuaranteeAmount1CurrencyType;
                draftContract.BidGuaranteeAmount1 = contract.BidGuaranteeAmount1;
                draftContract.BidGuaranteeAmount1Usd = contract.BidGuaranteeAmount1Usd;

                #endregion
                #region Bid Guarantee Amount 2

                draftContract.BidGuaranteeAmount2CurrencyType = contract.BidGuaranteeAmount2CurrencyType;
                draftContract.BidGuaranteeAmount2 = contract.BidGuaranteeAmount2;
                draftContract.BidGuaranteeAmount2Usd = contract.BidGuaranteeAmount2Usd;

                #endregion

                draftContract.ApproveNo1 = contract.ApproveNo1;
                draftContract.ApproveNo2 = contract.ApproveNo2;
                draftContract.ApproveNo3 = contract.ApproveNo3;
                draftContract.ApproveNo4 = contract.ApproveNo4;
                draftContract.ApproveNo5 = contract.ApproveNo5;
                draftContract.Memo = null;
                draftContract.BICContractCode = contract.BICContractCode;
                draftContract.MaintenanceContractFlag = "0";

                #endregion
                #region Billing

                if (param.BillingTarget != null)
                {
                    param.BillingTarget.BillingOfficeCode = billingData.BillingOfficeCode;

                    #region Sale Price Approval

                    param.BillingTarget.SalePrice_ApprovalCurrencyType = billingData.SalePrice_ApprovalCurrencyType;
                    param.BillingTarget.SalePrice_Approval = billingData.SalePrice_Approval;
                    param.BillingTarget.SalePrice_ApprovalUsd = billingData.SalePrice_ApprovalUsd;
                    param.BillingTarget.SalePrice_PaymentMethod_Approval = billingData.SalePrice_PaymentMethod_Approval;

                    #endregion
                    #region Sale Price Partial

                    param.BillingTarget.SalePrice_PartialCurrencyType = billingData.SalePrice_PartialCurrencyType;
                    param.BillingTarget.SalePrice_Partial = billingData.SalePrice_Partial;
                    param.BillingTarget.SalePrice_PartialUsd = billingData.SalePrice_PartialUsd;
                    param.BillingTarget.SalePrice_PaymentMethod_Partial = billingData.SalePrice_PaymentMethod_Partial;

                    #endregion
                    #region Sale Price Acceptance

                    param.BillingTarget.SalePrice_AcceptanceCurrencyType = billingData.SalePrice_AcceptanceCurrencyType;
                    param.BillingTarget.SalePrice_Acceptance = billingData.SalePrice_Acceptance;
                    param.BillingTarget.SalePrice_AcceptanceUsd = billingData.SalePrice_AcceptanceUsd;
                    param.BillingTarget.SalePrice_PaymentMethod_Acceptance = billingData.SalePrice_PaymentMethod_Acceptance;

                    #endregion

                    #region Installation Fee Approval

                    param.BillingTarget.InstallationFee_ApprovalCurrencyType = billingData.InstallationFee_ApprovalCurrencyType;
                    param.BillingTarget.InstallationFee_Approval = billingData.InstallationFee_Approval;
                    param.BillingTarget.InstallationFee_ApprovalUsd = billingData.InstallationFee_ApprovalUsd;
                    param.BillingTarget.InstallationFee_PaymentMethod_Approval = billingData.InstallationFee_PaymentMethod_Approval;

                    #endregion
                    #region Installation Fee Partial

                    param.BillingTarget.InstallationFee_PartialCurrencyType = billingData.InstallationFee_PartialCurrencyType;
                    param.BillingTarget.InstallationFee_Partial = billingData.InstallationFee_Partial;
                    param.BillingTarget.InstallationFee_PartialUsd = billingData.InstallationFee_PartialUsd;
                    param.BillingTarget.InstallationFee_PaymentMethod_Partial = billingData.InstallationFee_PaymentMethod_Partial;

                    #endregion
                    #region Installation Fee Acceptance

                    param.BillingTarget.InstallationFee_AcceptanceCurrencyType = billingData.InstallationFee_AcceptanceCurrencyType;
                    param.BillingTarget.InstallationFee_Acceptance = billingData.InstallationFee_Acceptance;
                    param.BillingTarget.InstallationFee_AcceptanceUsd = billingData.InstallationFee_AcceptanceUsd;
                    param.BillingTarget.InstallationFee_PaymentMethod_Acceptance = billingData.InstallationFee_PaymentMethod_Acceptance;

                    #endregion

                    //param.BillingTarget.TotalPrice = billingData.TotalPrice;
                    param.BillingTarget.DocLanguage = billingData.DocLanguage; //Add by Jutarat A. on 20122013
                }

                #endregion
                #region Draft sale Email

                if (param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail != null)
                {
                    List<tbt_DraftSaleEmail> emailLst = new List<tbt_DraftSaleEmail>();

                    int emailID = 1;
                    foreach (tbt_DraftSaleEmail email in param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail)
                    {
                        tbt_DraftSaleEmail e = new tbt_DraftSaleEmail()
                        {
                            DraftSaleEmailID = emailID,
                            QuotationTargetCode = contract.QuotationTargetCodeLong,
                            Alphabet = contract.Alphabet,
                            EmailAddress = email.EmailAddress,
                            SendFlag = email.SendFlag,
                            ToEmpNo = email.ToEmpNo,
                            CreateDate = email.CreateDate,
                            CreateBy = email.CreateBy,
                            UpdateDate = email.UpdateDate,
                            UpdateBy = email.UpdateBy
                        };
                        emailLst.Add(e);

                        emailID++;
                    }

                    param.doRegisterDraftSaleContractData.doTbt_DraftSaleEmail = emailLst;
                }

                #endregion
                #region Draft sale instrument

                if (param.doDraftSaleContractData.doTbt_DraftSaleInstrument != null)
                {
                    param.doRegisterDraftSaleContractData.doTbt_DraftSaleInstrument = new List<tbt_DraftSaleInstrument>();
                    foreach (tbt_DraftSaleInstrument inst in param.doDraftSaleContractData.doTbt_DraftSaleInstrument)
                    {
                        tbt_DraftSaleInstrument ninst = CommonUtil.CloneObject<tbt_DraftSaleInstrument, tbt_DraftSaleInstrument>(inst);
                        if (CommonUtil.IsNullOrEmpty(ninst.QuotationTargetCode))
                            ninst.QuotationTargetCode = contract.QuotationTargetCodeLong;

                        param.doRegisterDraftSaleContractData.doTbt_DraftSaleInstrument.Add(ninst);
                    }
                }

                #endregion
                #region Relation type

                if (param.doDraftSaleContractData.doTbt_RelationType != null)
                {
                    param.doRegisterDraftSaleContractData.doTbt_RelationType = new List<tbt_RelationType>();
                    foreach (tbt_RelationType rt in param.doDraftSaleContractData.doTbt_RelationType)
                    {
                        tbt_RelationType nrt = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(rt);
                        nrt.ContractCode = contract.QuotationTargetCodeLong;
                        nrt.OCC = contract.Alphabet;

                        param.doRegisterDraftSaleContractData.doTbt_RelationType.Add(nrt);
                    }
                }

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Validate bussiness data
        /// </summary>
        /// <param name="res"></param>
        /// <param name="draft"></param>
        /// <param name="billingList"></param>
        /// <param name="isContractCodeForDepositFeeSlide"></param>
        /// <param name="isBranchChecked"></param>
        /// <param name="isBillingEditMode"></param>
        /// <returns></returns>
        private bool CTS020_ValidateBusiness(ObjectResultData res,
                                                doDraftSaleContractData draft,
                                                CTS020_BillingTargetData billing)
        {
            try
            {
                if (draft == null)
                    return false;

                tbt_DraftSaleContract contract = draft.doTbt_DraftSaleContract;

                DateTime currentDate = DateTime.Now;
                currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                //DateTime future = new DateTime(currentDate.Year + 3, currentDate.Month, currentDate.Day);
                DateTime future = currentDate.AddYears(3);

                #region Expect installation date

                //if (contract.ExpectedInstallCompleteDate.Value.CompareTo(currentDate) < 0)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3205,
                //                        null, new string[] { "ExpectedInstallCompleteDate" });
                //    return false;
                //}
                if (contract.ExpectedInstallCompleteDate.Value.CompareTo(future) > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3206,
                                        null, new string[] { "ExpectedInstallCompleteDate" });
                    return false;
                }

                #endregion
                #region Expected operation date

                //if (contract.ExpectedAcceptanceAgreeDate.Value.CompareTo(currentDate) < 0)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3207,
                //                        null, new string[] { "ExpectedAcceptanceAgreeDate" });
                //    return false;
                //}
                if (contract.ExpectedAcceptanceAgreeDate.Value.CompareTo(future) > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3208,
                                        null, new string[] { "ExpectedAcceptanceAgreeDate" });
                    return false;
                }

                #endregion

                decimal? totalSalePriceApproval = 0;
                decimal? totalSalePriceApprovalUsd = 0;
                decimal? totalSalePricePartial = 0;
                decimal? totalSalePricePartialUsd = 0;
                decimal? totalSalePriceAcceptance = 0;
                decimal? totalSalePriceAcceptanceUsd = 0;
                decimal? totalInstallFeeApproval = 0;
                decimal? totalInstallFeeApprovalUsd = 0;
                decimal? totalInstallFeePartial = 0;
                decimal? totalInstallFeePartialUsd = 0;
                decimal? totalInstallFeeAcceptance = 0;
                decimal? totalInstallFeeAcceptanceUsd = 0;

                bool isSameSalePriceApprovalCurrency = true;
                bool isSameSalePricePartialCurrency = true;
                bool isSameSalePriceAcceptanceCurrency = true;
                bool isSameInstallFeeApprovalCurrency = true;
                bool isSameInstallFeePartialCurrency = true;
                bool isSameInstallFeeAcceptanceCurrency = true;

                bool hasSalePricePaymentApproval = false;
                bool hasSalePricePaymentPartial = false;
                bool hasSalePricePaymentAcceptance = false;
                bool hasInstallFeePaymentApproval = false;
                bool hasInstallFeePaymentPartial = false;
                bool hasInstallFeePaymentAcceptance = false;

                bool isCorrectSalePricePaymentApproval = true;
                bool isCorrectSalePricePaymentPartial = true;
                bool isCorrectSalePricePaymentAcceptance = true;
                bool isCorrectInstallFeePaymentApproval = true;
                bool isCorrectInstallFeePaymentPartial = true;
                bool isCorrectInstallFeePaymentAcceptance = true;

                if (billing != null)
                {
                    #region Sale Price Approval

                    if (billing.SalePrice_Approval > 0
                        || billing.SalePrice_ApprovalUsd > 0)
                    {
                        if (contract.BillingAmt_ApproveContractCurrencyType != billing.SalePrice_ApprovalCurrencyType)
                            isSameSalePriceApprovalCurrency = false;

                        if (billing.SalePrice_Approval != null)
                            totalSalePriceApproval += billing.SalePrice_Approval;
                        if (billing.SalePrice_ApprovalUsd != null)
                            totalSalePriceApprovalUsd += billing.SalePrice_ApprovalUsd;
                    }
                    if (CommonUtil.IsNullOrEmpty(billing.SalePrice_PaymentMethod_Approval) == false)
                    {
                        hasSalePricePaymentApproval = true;

                        if (billing.SalePrice_PaymentMethod_Approval != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.SalePrice_PaymentMethod_Approval != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_AUTO_TRANSFER)
                            isCorrectSalePricePaymentApproval = false;
                    }

                    #endregion
                    #region Sale Price Partial

                    if (billing.SalePrice_Partial > 0
                        || billing.SalePrice_PartialUsd > 0)
                    {
                        if (contract.BillingAmt_PartialFeeCurrencyType != billing.SalePrice_PartialCurrencyType)
                            isSameSalePricePartialCurrency = false;

                        if (billing.SalePrice_Partial != null)
                            totalSalePricePartial += billing.SalePrice_Partial;
                        if (billing.SalePrice_PartialUsd != null)
                            totalSalePricePartialUsd += billing.SalePrice_PartialUsd;
                    }
                    if (CommonUtil.IsNullOrEmpty(billing.SalePrice_PaymentMethod_Partial) == false)
                    {
                        hasSalePricePaymentPartial = true;

                        if (billing.SalePrice_PaymentMethod_Partial != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.SalePrice_PaymentMethod_Partial != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_AUTO_TRANSFER)
                            isCorrectSalePricePaymentPartial = false;
                    }

                    #endregion
                    #region Sale Price Acceptance

                    if (billing.SalePrice_Acceptance > 0
                        || billing.SalePrice_AcceptanceUsd > 0)
                    {
                        if (contract.BillingAmt_AcceptanceCurrencyType != billing.SalePrice_AcceptanceCurrencyType)
                            isSameSalePriceAcceptanceCurrency = false;

                        if (billing.SalePrice_Acceptance != null)
                            totalSalePriceAcceptance += billing.SalePrice_Acceptance;
                        if (billing.SalePrice_AcceptanceUsd != null)
                            totalSalePriceAcceptanceUsd += billing.SalePrice_AcceptanceUsd;
                    }
                    if (CommonUtil.IsNullOrEmpty(billing.SalePrice_PaymentMethod_Acceptance) == false)
                    {
                        hasSalePricePaymentAcceptance = true;

                        if (billing.SalePrice_PaymentMethod_Acceptance != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.SalePrice_PaymentMethod_Acceptance != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_AUTO_TRANSFER)
                            isCorrectSalePricePaymentAcceptance = false;
                    }

                    #endregion

                    #region Install Fee Approval

                    if (billing.InstallationFee_Approval > 0
                        || billing.InstallationFee_ApprovalUsd > 0)
                    {
                        if (contract.BillingAmtInstallation_ApproveContractCurrencyType != billing.InstallationFee_ApprovalCurrencyType)
                            isSameInstallFeeApprovalCurrency = false;

                        if (billing.InstallationFee_Approval != null)
                            totalInstallFeeApproval += billing.InstallationFee_Approval;
                        if (billing.InstallationFee_ApprovalUsd != null)
                            totalInstallFeeApprovalUsd += billing.InstallationFee_ApprovalUsd;
                    }
                    if (CommonUtil.IsNullOrEmpty(billing.InstallationFee_PaymentMethod_Approval) == false)
                    {
                        hasInstallFeePaymentApproval = true;

                        if (billing.InstallationFee_PaymentMethod_Approval != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.InstallationFee_PaymentMethod_Approval != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_AUTO_TRANSFER)
                            isCorrectInstallFeePaymentApproval = false;
                    }

                    #endregion
                    #region Install Fee Partial

                    if (billing.InstallationFee_Partial > 0
                        || billing.InstallationFee_PartialUsd > 0)
                    {
                        if (contract.BillingAmtInstallation_PartialFeeCurrencyType != billing.InstallationFee_PartialCurrencyType)
                            isSameInstallFeePartialCurrency = false;

                        if (billing.InstallationFee_Partial != null)
                            totalInstallFeePartial += billing.InstallationFee_Partial;
                        if (billing.InstallationFee_PartialUsd != null)
                            totalInstallFeePartialUsd += billing.InstallationFee_PartialUsd;
                    }
                    if (CommonUtil.IsNullOrEmpty(billing.InstallationFee_PaymentMethod_Partial) == false)
                    {
                        hasInstallFeePaymentPartial = true;

                        if (billing.InstallationFee_PaymentMethod_Partial != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.InstallationFee_PaymentMethod_Partial != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_AUTO_TRANSFER)
                            isCorrectInstallFeePaymentPartial = false;
                    }

                    #endregion
                    #region Install Fee Acceptance

                    if (billing.InstallationFee_Acceptance > 0
                        || billing.InstallationFee_AcceptanceUsd > 0)
                    {
                        if (contract.BillingAmtInstallation_AcceptanceCurrencyType != billing.InstallationFee_AcceptanceCurrencyType)
                            isSameInstallFeeAcceptanceCurrency = false;

                        if (billing.InstallationFee_Acceptance != null)
                            totalInstallFeeAcceptance += billing.InstallationFee_Acceptance;
                        if (billing.InstallationFee_AcceptanceUsd != null)
                            totalInstallFeeAcceptanceUsd += billing.InstallationFee_AcceptanceUsd;
                    }
                    if (CommonUtil.IsNullOrEmpty(billing.InstallationFee_PaymentMethod_Acceptance) == false)
                    {
                        hasInstallFeePaymentAcceptance = true;

                        if (billing.InstallationFee_PaymentMethod_Acceptance != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.InstallationFee_PaymentMethod_Acceptance != SECOM_AJIS.Common.Util.ConstantValue.MethodType.C_PAYMENT_METHOD_AUTO_TRANSFER)
                            isCorrectInstallFeePaymentAcceptance = false;
                    }

                    #endregion
                }

                //Add requirement
                decimal? sumOrderInstallFee = 0;
                
                if (contract.BillingAmt_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    sumOrderInstallFee = sumOrderInstallFee + (contract.BillingAmt_ApproveContractUsd ?? 0);
                else
                    sumOrderInstallFee = sumOrderInstallFee + (contract.BillingAmt_ApproveContract ?? 0);

                if (contract.BillingAmt_PartialFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    sumOrderInstallFee = sumOrderInstallFee + (contract.BillingAmt_PartialFeeUsd ?? 0);
                else
                    sumOrderInstallFee = sumOrderInstallFee + (contract.BillingAmt_PartialFee ?? 0);

                if (contract.BillingAmt_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    sumOrderInstallFee = sumOrderInstallFee + (contract.BillingAmt_AcceptanceUsd ?? 0);
                else
                    sumOrderInstallFee = sumOrderInstallFee + (contract.BillingAmt_Acceptance ?? 0);

                decimal? sumOrderProDuctPrice = 0;
                if (contract.BillingAmtInstallation_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    sumOrderProDuctPrice = sumOrderProDuctPrice + (contract.BillingAmtInstallation_ApproveContractUsd ?? 0);
                else
                    sumOrderProDuctPrice = sumOrderProDuctPrice + (contract.BillingAmtInstallation_ApproveContract ?? 0);

                if (contract.BillingAmtInstallation_PartialFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    sumOrderProDuctPrice = sumOrderProDuctPrice + (contract.BillingAmtInstallation_PartialFeeUsd ?? 0);
                else
                    sumOrderProDuctPrice = sumOrderProDuctPrice + (contract.BillingAmtInstallation_PartialFee ?? 0);

                if (contract.BillingAmtInstallation_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    sumOrderProDuctPrice = sumOrderProDuctPrice + (contract.BillingAmtInstallation_AcceptanceUsd ?? 0);
                else
                    sumOrderProDuctPrice = sumOrderProDuctPrice + (contract.BillingAmtInstallation_Acceptance ?? 0);


                if (contract.OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if ((contract.OrderProductPriceUsd != sumOrderInstallFee
                        && contract.OrderProductPriceCurrencyType == contract.BillingAmt_ApproveContractCurrencyType
                        && contract.OrderProductPriceCurrencyType == contract.BillingAmt_PartialFeeCurrencyType
                        && contract.OrderProductPriceCurrencyType == contract.BillingAmt_AcceptanceCurrencyType)
                        || (contract.OrderProductPriceUsd != sumOrderInstallFee
                        && contract.OrderProductPriceCurrencyType != contract.BillingAmt_ApproveContractCurrencyType
                        && (contract.BillingAmt_ApproveContract == 0 || contract.BillingAmt_ApproveContractUsd == 0))

                        || (contract.OrderProductPriceUsd != sumOrderInstallFee
                        && contract.OrderProductPriceCurrencyType != contract.BillingAmt_PartialFeeCurrencyType
                        && (contract.BillingAmt_PartialFee == 0 || contract.BillingAmt_PartialFeeUsd == 0))

                        || (contract.OrderProductPriceUsd != sumOrderInstallFee
                        && contract.OrderProductPriceCurrencyType != contract.BillingAmt_AcceptanceCurrencyType
                        && (contract.BillingAmt_Acceptance == 0 || contract.BillingAmt_AcceptanceUsd == 0)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3088, null, new string[] { "OrderProductPrice" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return false;
                    }
                }
                else
                {
                    if ((contract.OrderProductPrice != sumOrderInstallFee
                        && contract.OrderProductPriceCurrencyType == contract.BillingAmt_ApproveContractCurrencyType
                        && contract.OrderProductPriceCurrencyType == contract.BillingAmt_PartialFeeCurrencyType
                        && contract.OrderProductPriceCurrencyType == contract.BillingAmt_AcceptanceCurrencyType)
                        || (contract.OrderProductPrice != sumOrderInstallFee
                        && contract.OrderProductPriceCurrencyType != contract.BillingAmt_ApproveContractCurrencyType
                        && (contract.BillingAmt_ApproveContract == 0 || contract.BillingAmt_ApproveContractUsd == 0))

                        || (contract.OrderProductPrice != sumOrderInstallFee
                        && contract.OrderProductPriceCurrencyType != contract.BillingAmt_PartialFeeCurrencyType
                        && (contract.BillingAmt_PartialFee == 0 || contract.BillingAmt_PartialFeeUsd == 0))

                        || (contract.OrderProductPrice != sumOrderInstallFee
                        && contract.OrderProductPriceCurrencyType != contract.BillingAmt_AcceptanceCurrencyType
                        && (contract.BillingAmt_Acceptance == 0 || contract.BillingAmt_AcceptanceUsd == 0)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3088, null, new string[] { "OrderProductPrice" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return false;
                    }
                }

                if (contract.OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if ((contract.OrderInstallFeeUsd != sumOrderProDuctPrice
                        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_ApproveContractCurrencyType
                        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_PartialFeeCurrencyType
                        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_AcceptanceCurrencyType)
                        || (contract.OrderInstallFeeUsd != sumOrderProDuctPrice
                        && contract.OrderInstallFeeCurrencyType != contract.BillingAmtInstallation_ApproveContractCurrencyType
                        && (contract.BillingAmtInstallation_ApproveContract == 0 || contract.BillingAmtInstallation_ApproveContractUsd == 0))

                        || (contract.OrderInstallFeeUsd != sumOrderProDuctPrice
                        && contract.OrderInstallFeeCurrencyType != contract.BillingAmtInstallation_PartialFeeCurrencyType
                        && (contract.BillingAmtInstallation_PartialFee == 0 || contract.BillingAmtInstallation_PartialFeeUsd == 0))

                        || (contract.OrderInstallFeeUsd != sumOrderProDuctPrice
                        && contract.OrderInstallFeeCurrencyType != contract.BillingAmtInstallation_AcceptanceCurrencyType
                        && (contract.BillingAmtInstallation_Acceptance == 0 || contract.BillingAmtInstallation_AcceptanceUsd == 0)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3088, null, new string[] { "OrderInstallFee" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return false;
                    }
                }
                else
                {
                    if ((contract.OrderInstallFee != sumOrderProDuctPrice
                        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_ApproveContractCurrencyType
                        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_PartialFeeCurrencyType
                        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_AcceptanceCurrencyType)
                        || (contract.OrderInstallFee != sumOrderProDuctPrice
                        && contract.OrderInstallFeeCurrencyType != contract.BillingAmtInstallation_ApproveContractCurrencyType
                        && (contract.BillingAmtInstallation_ApproveContract == 0 || contract.BillingAmtInstallation_ApproveContractUsd == 0))

                        || (contract.OrderInstallFee != sumOrderProDuctPrice
                        && contract.OrderInstallFeeCurrencyType != contract.BillingAmtInstallation_PartialFeeCurrencyType
                        && (contract.BillingAmtInstallation_PartialFee == 0 || contract.BillingAmtInstallation_PartialFeeUsd == 0))

                        || (contract.OrderInstallFee != sumOrderProDuctPrice
                        && contract.OrderInstallFeeCurrencyType != contract.BillingAmtInstallation_AcceptanceCurrencyType
                        && (contract.BillingAmtInstallation_Acceptance == 0 || contract.BillingAmtInstallation_AcceptanceUsd == 0)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3088, null, new string[] { "OrderProductPrice" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return false;
                    }
                }

                // Add by Jirawat Jannet on 2016-12-01
                #region Validate 4.2: Validate Product Price

                // Edit by Jirawat on 2016-12-02
                #region Valdiate max data


                if (contract.NormalProductPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (CommonUtil.IsNullOrEmpty(contract.NormalProductPriceUsd) == false)
                    {
                        if (contract.NormalProductPriceUsd > 999999999999.99M)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233);
                            return false;
                        }
                    }
                }
                else
                {
                    if (CommonUtil.IsNullOrEmpty(contract.NormalProductPrice) == false)
                    {
                        if (contract.NormalProductPrice > 999999999999.99M)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233);
                            return false;
                        }
                    }
                }


                if (contract.OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (CommonUtil.IsNullOrEmpty(contract.OrderProductPriceUsd) == false)
                    {
                        if (contract.OrderProductPriceUsd > 999999999999.99M)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233,
                                            null, new string[] { "OrderProductPrice" });
                            return false;
                        }
                    }
                }
                else
                {
                    if (CommonUtil.IsNullOrEmpty(contract.OrderProductPrice) == false)
                    {
                        if (contract.OrderProductPrice > 999999999999.99M)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233,
                                            null, new string[] { "OrderProductPrice" });
                            return false;
                        }
                    }
                }

                #endregion

                // Add by Jirawat on 2016-12-01
                #region Validate 4.2.1: 

                if (contract.NormalProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.NormalProductPriceUsd > 0
                            && (CommonUtil.IsNullOrEmpty(contract.OrderProductPriceUsd)
                                || contract.OrderProductPriceUsd == 0)
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3322, null, new string[] { "ApproveNo1" });
                        return false;
                    }
                }
                else
                {
                    if (contract.NormalProductPrice > 0 
                            && (CommonUtil.IsNullOrEmpty(contract.OrderProductPrice)
                                || contract.OrderProductPrice == 0)
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3322, null, new string[] { "ApproveNo1" });
                         return false;
                    }

                }

                #endregion
                // Add by Jirawat Jannet on 2016-12-01
                #region Validate 4.2.2: The order product price can not equal to summary of all product price in billing timing


                //if ((contract.OrderProductPriceCurrencyType == contract.BillingAmt_ApproveContractCurrencyType
                //            && getPriceByCurrency(contract.BillingAmt_ApproveContract, contract.BillingAmt_ApproveContractUsd, contract.BillingAmt_ApproveContractCurrencyType) > 0)

                //        || (contract.OrderProductPriceCurrencyType == contract.BillingAmt_PartialFeeCurrencyType
                //            && getPriceByCurrency(contract.BillingAmt_PartialFee, contract.BillingAmt_PartialFeeUsd, contract.BillingAmt_PartialFeeCurrencyType) > 0)

                //        || (contract.OrderProductPriceCurrencyType == contract.BillingAmt_AcceptanceCurrencyType
                //            && getPriceByCurrency(contract.BillingAmt_Acceptance, contract.BillingAmt_AcceptanceUsd, contract.BillingAmt_AcceptanceCurrencyType) > 0))


                if (contract.NormalProductPriceCurrencyType == contract.OrderProductPriceCurrencyType)
                {
                    decimal oprod = 0;
                    decimal oprod_ap = 0;
                    decimal oprod_p = 0;
                    decimal oprod_ac = 0;
                    bool skpprod = false;

                    #region Order Product Price

                    if (contract.OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.OrderProductPriceUsd) == false)
                            oprod = contract.OrderProductPriceUsd.Value;
                    }
                    else
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.OrderProductPrice) == false)
                            oprod = contract.OrderProductPrice.Value;
                    }

                    #endregion
                    #region Billing Amount Approve Contract

                    if (contract.BillingAmt_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_ApproveContractUsd) == false)
                            oprod_ap = contract.BillingAmt_ApproveContractUsd.Value;
                    }
                    else
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_ApproveContract) == false)
                            oprod_ap = contract.BillingAmt_ApproveContract.Value;
                    }
                    if (oprod_ap > 0 && contract.BillingAmt_ApproveContractCurrencyType != contract.OrderProductPriceCurrencyType)
                        skpprod = true;

                    #endregion
                    #region Billing Amount Partial Fee

                    if (contract.BillingAmt_PartialFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_PartialFeeUsd) == false)
                            oprod_p = contract.BillingAmt_PartialFeeUsd.Value;
                    }
                    else
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_PartialFee) == false)
                            oprod_p = contract.BillingAmt_PartialFee.Value;
                    }
                    if (oprod_p > 0 && contract.BillingAmt_PartialFeeCurrencyType != contract.OrderProductPriceCurrencyType)
                        skpprod = true;

                    #endregion
                    #region Billing Amount Acceptance

                    if (contract.BillingAmt_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_AcceptanceUsd) == false)
                            oprod_ac = contract.BillingAmt_AcceptanceUsd.Value;
                    }
                    else
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_Acceptance) == false)
                            oprod_ac = contract.BillingAmt_Acceptance.Value;
                    }
                    if (oprod_ac > 0 && contract.BillingAmt_AcceptanceCurrencyType != contract.OrderProductPriceCurrencyType)
                        skpprod = true;

                    #endregion
                    
                    if (skpprod == false)
                    {
                        decimal? totalProduct = oprod_ap + oprod_p + oprod_ac;
                        if (oprod != totalProduct)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3323, null, new string[] { "OrderProductPrice" });
                            return false;
                        }
                    }
                }
                
                #endregion
                // Add by Jirawat Jannet on 2016-12-02
                #region 4.2.3: Validate Approve no.1

                if (contract.NormalProductPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.NormalProductPriceUsd != contract.OrderProductPriceUsd
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3324, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }
                else
                {
                    if (contract.NormalProductPrice != contract.OrderProductPrice
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3324, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }

                #endregion
                // Add by Jirawat Jannet on 2016-12-02
                #region Validate 4.2.4: Payment method of billing product price at approval. 

                if (contract.BillingAmt_ApproveContractCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.BillingAmt_ApproveContractUsd > 0
                            && CommonUtil.IsNullOrEmpty(billing.SalePrice_PaymentMethod_Approval))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3325, null, new string[] { "SalePrice_PaymentMethod_Approval" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmt_ApproveContract > 0
                            && CommonUtil.IsNullOrEmpty(billing.SalePrice_PaymentMethod_Approval))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3325, null, new string[] { "SalePrice_PaymentMethod_Approval" });
                         return false;
                    }
                }

                #endregion
                // Add by Jirawat Jannet on 2016-12-02
                #region Validate 4.2.5: Payment method of billing partial fee for product price.

                if (contract.BillingAmt_PartialFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.BillingAmt_PartialFeeUsd > 0
                        && CommonUtil.IsNullOrEmpty(billing.SalePrice_PaymentMethod_Partial))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3326, null, new string[] { "SalePrice_PaymentMethod_Partial" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmt_PartialFee > 0
                        && CommonUtil.IsNullOrEmpty(billing.SalePrice_PaymentMethod_Partial))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3326, null, new string[] { "SalePrice_PaymentMethod_Partial" });
                         return false;
                    }
                }

                #endregion
                // Add by Jirawat Jannet on 2016-12-02
                #region Validate 4.2.6: Payment method of billing product price at customer acceptance.

                if (contract.BillingAmt_AcceptanceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.BillingAmt_AcceptanceUsd > 0
                        && CommonUtil.IsNullOrEmpty(billing.SalePrice_PaymentMethod_Acceptance))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3327, null, new string[] { "SalePrice_PaymentMethod_Acceptance" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmt_Acceptance > 0
                        && CommonUtil.IsNullOrEmpty(billing.SalePrice_PaymentMethod_Acceptance))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3327, null, new string[] { "SalePrice_PaymentMethod_Acceptance" });
                         return false;
                    }
                }

                #endregion
                // Add by Jirawat Jannet on 2016-12-02
                #region Validate 4.2.7: If there is Billing product price at approval, The payment method is not a bank transfer or bank transfer (other account). Please register approve no.

                if (contract.BillingAmt_ApproveContractCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.BillingAmt_ApproveContractUsd > 0
                            && billing.SalePrice_PaymentMethod_Approval != PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.SalePrice_PaymentMethod_Approval != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmt_ApproveContract > 0
                            && billing.SalePrice_PaymentMethod_Approval != PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.SalePrice_PaymentMethod_Approval != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }

                #endregion
                // Add by Jirawat Jannet on 2016-12-02
                #region Validate 4.2.8: If there is Partial fee for product price,  The payment method is not a bank transfer or bank transfer (other account). Please register approve no.

                if (contract.BillingAmt_PartialFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.BillingAmt_PartialFeeUsd > 0
                            && billing.SalePrice_PaymentMethod_Partial != PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.SalePrice_PaymentMethod_Partial != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmt_PartialFee > 0
                            && billing.SalePrice_PaymentMethod_Partial != PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.SalePrice_PaymentMethod_Partial != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }

                #endregion
                // Add by Jirawat Jannet on 2016-12-02
                #region Valdiate 4.2.9: If there is Product price Billing price at customer acceptance,  The payment method is not a bank transfer or bank transfer (other account). Please register approve no.

                if (contract.BillingAmt_AcceptanceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.BillingAmt_AcceptanceUsd > 0
                            && billing.SalePrice_PaymentMethod_Acceptance != PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.SalePrice_PaymentMethod_Acceptance != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmt_Acceptance > 0
                            && billing.SalePrice_PaymentMethod_Acceptance != PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.SalePrice_PaymentMethod_Acceptance != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }

                #endregion
                // Add by Jirawat Jannet on 2016-12-02
                #region Validate 4.2.10: 

                if (contract.NormalProductPriceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.NormalProductPriceUsd > 0 && contract.OrderProductPriceUsd > 0
                        && (
                            contract.OrderProductPriceUsd <= (contract.NormalProductPriceUsd ?? 0) * 0.3M
                            || contract.OrderProductPriceUsd >= (contract.NormalProductPriceUsd ?? 0) * 10M)
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3335, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }
                else
                {
                    if (contract.NormalProductPrice > 0 && contract.OrderProductPrice > 0
                        && (
                            contract.OrderProductPrice <= (contract.NormalProductPrice ?? 0) * 0.3M
                            || contract.OrderProductPrice >= (contract.NormalProductPrice ?? 0) * 10M)
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3335, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }

                #endregion

                #endregion

                // Comment by Jirawat Jannet
                #region Product Price

                //if (CommonUtil.IsNullOrEmpty(contract.NormalProductPrice) == false)
                //{
                //    if (contract.NormalProductPrice > 999999999999.99M)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233);
                //        return false;
                //    }
                //}

                //if (contract.NormalSalePriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                //{
                //    if (CommonUtil.IsNullOrEmpty(contract.OrderProductPriceUsd) == false)
                //    {
                //        if (contract.OrderProductPriceUsd > 999999999999.99M)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233,
                //                            null, new string[] { "OrderProductPrice" });
                //            return false;
                //        }
                //    }

                //    if ((CommonUtil.IsNullOrEmpty(contract.NormalProductPrice)
                //           || contract.NormalProductPrice == 0)
                //        && contract.OrderProductPriceUsd > 0
                //        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3322,
                //                            null, new string[] { "ApproveNo1" });
                //        return false;
                //    }

                //    if (contract.NormalProductPrice != contract.OrderProductPriceUsd
                //        && contract.OrderProductPriceUsd > 0
                //        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3324,
                //                            null, new string[] { "ApproveNo1" });
                //        return false;
                //    }

                //    if (contract.NormalProductPrice > 0
                //            && contract.OrderProductPriceUsd > 0
                //            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        decimal? fee30 = (contract.NormalProductPrice * 0.3M);
                //        decimal? fee1000 = (contract.OrderProductPriceUsd * 10.0M);
                //        if (contract.OrderProductPriceUsd <= fee30)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3335,
                //                                null, new string[] { "ApproveNo1" });
                //            return false;
                //        }
                //        if (contract.OrderProductPriceUsd >= fee1000)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3335,
                //                                null, new string[] { "ApproveNo1" });
                //            return false;
                //        }
                //    }

                //    // Comment by Jirawat Jannet on 2016-12-02
                //    //if (contract.NormalProductPriceCurrencyType == contract.OrderProductPriceCurrencyType
                //    //    && contract.OrderProductPriceCurrencyType == contract.BillingAmt_ApproveContractCurrencyType
                //    //    && contract.OrderProductPriceCurrencyType == contract.BillingAmt_PartialFeeCurrencyType
                //    //    && contract.OrderProductPriceCurrencyType == contract.BillingAmt_AcceptanceCurrencyType)
                //    // Add by Jirawat Jannet on 2016-12-02
                //    if (contract.NormalProductPriceCurrencyType == contract.OrderProductPriceCurrencyType
                //        && ((contract.OrderProductPriceCurrencyType == contract.BillingAmt_ApproveContractCurrencyType
                //                && (contract.BillingAmt_ApproveContractUsd ?? 0) > 0)

                //            || (contract.OrderProductPriceCurrencyType == contract.BillingAmt_PartialFeeCurrencyType
                //                && (contract.BillingAmt_PartialFeeUsd ?? 0) > 0)

                //            || (contract.OrderProductPriceCurrencyType == contract.BillingAmt_AcceptanceCurrencyType
                //                && (contract.BillingAmt_AcceptanceUsd ?? 0) > 0)))
                //    {
                //        decimal o_appv = 0;
                //        decimal o_part = 0;
                //        decimal o_acct = 0;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_ApproveContractUsd) == false)
                //            o_appv = contract.BillingAmt_ApproveContract.Value;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_PartialFeeUsd) == false)
                //            o_part = contract.BillingAmt_PartialFeeUsd.Value;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_AcceptanceUsd) == false)
                //            o_acct = contract.BillingAmt_AcceptanceUsd.Value;

                //        decimal? totalProductPrice = o_appv + o_part + o_acct;
                //        if (contract.OrderProductPriceUsd != totalProductPrice)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3323,
                //                                null, new string[] { "OrderProductPrice" });
                //            return false;
                //        }

                //        if (o_appv > 0 && hasSalePricePaymentApproval == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3325,
                //                                null, new string[] { "SalePrice_PaymentMethod_Approval" });
                //            return false;
                //        }
                //        if (o_part > 0 && hasSalePricePaymentPartial == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3326,
                //                                null, new string[] { "SalePrice_PaymentMethod_Partial" });
                //            return false;
                //        }
                //        if (o_acct > 0 && hasSalePricePaymentAcceptance == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3327,
                //                                null, new string[] { "SalePrice_PaymentMethod_Acceptance" });
                //            return false;
                //        }

                //        if (o_appv > 0 && isCorrectSalePricePaymentApproval == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "SalePrice_PaymentMethod_Approval" });
                //            return false;
                //        }
                //        if (o_part > 0 && isCorrectSalePricePaymentPartial == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "SalePrice_PaymentMethod_Partial" });
                //            return false;
                //        }
                //        if (o_acct > 0 && isCorrectSalePricePaymentAcceptance == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "SalePrice_PaymentMethod_Acceptance" });
                //            return false;
                //        }
                //    }
                //}
                //else
                //{
                //    if (CommonUtil.IsNullOrEmpty(contract.OrderProductPrice) == false)
                //    {
                //        if (contract.OrderProductPrice > 999999999999.99M)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233,
                //                            null, new string[] { "OrderProductPrice" });
                //            return false;
                //        }
                //    }

                //    if ((CommonUtil.IsNullOrEmpty(contract.NormalProductPrice)
                //           || contract.NormalProductPrice == 0)
                //        && contract.OrderProductPrice > 0
                //        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3322,
                //                            null, new string[] { "ApproveNo1" });
                //        return false;
                //    }

                //    if (contract.NormalProductPrice != contract.OrderProductPrice
                //        && contract.OrderProductPrice > 0
                //        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3324,
                //                            null, new string[] { "ApproveNo1" });
                //        return false;
                //    }

                //    if (contract.NormalProductPrice > 0
                //            && contract.OrderProductPrice > 0
                //            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        decimal? fee30 = (contract.NormalProductPrice * 0.3M);
                //        decimal? fee1000 = (contract.OrderProductPrice * 10.0M);
                //        if (contract.OrderProductPrice <= fee30)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3335,
                //                                null, new string[] { "ApproveNo1" });
                //            return false;
                //        }
                //        if (contract.OrderProductPrice >= fee1000)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3335,
                //                                null, new string[] { "ApproveNo1" });
                //            return false;
                //        }
                //    }


                //    // Comment by Jirawat Jannet on 2016-12-02
                //    //if (contract.NormalProductPriceCurrencyType == contract.OrderProductPriceCurrencyType
                //    //    && contract.OrderProductPriceCurrencyType == contract.BillingAmt_ApproveContractCurrencyType
                //    //    && contract.OrderProductPriceCurrencyType == contract.BillingAmt_PartialFeeCurrencyType
                //    //    && contract.OrderProductPriceCurrencyType == contract.BillingAmt_AcceptanceCurrencyType)
                //    // Add by Jirawat Jannet on 2016-12-02
                //    if (contract.NormalProductPriceCurrencyType == contract.OrderProductPriceCurrencyType
                //        && ((contract.OrderProductPriceCurrencyType == contract.BillingAmt_ApproveContractCurrencyType
                //                && (contract.BillingAmt_ApproveContract ?? 0) > 0)

                //            || (contract.OrderProductPriceCurrencyType == contract.BillingAmt_PartialFeeCurrencyType
                //                && (contract.BillingAmt_PartialFee ?? 0) > 0)

                //            || (contract.OrderProductPriceCurrencyType == contract.BillingAmt_AcceptanceCurrencyType
                //                && (contract.BillingAmt_Acceptance ?? 0) > 0)))
                //    {
                //        decimal o_appv = 0;
                //        decimal o_part = 0;
                //        decimal o_acct = 0;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_ApproveContract) == false)
                //            o_appv = contract.BillingAmt_ApproveContract.Value;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_PartialFee) == false)
                //            o_part = contract.BillingAmt_PartialFee.Value;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmt_Acceptance) == false)
                //            o_acct = contract.BillingAmt_Acceptance.Value;

                //        decimal? totalProductPrice = o_appv + o_part + o_acct;
                //        if (contract.OrderProductPrice != totalProductPrice)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3323,
                //                                null, new string[] { "OrderProductPrice" });
                //            return false;
                //        }

                //        // Validate 4.2.4
                //        if (o_appv > 0 && hasSalePricePaymentApproval == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3325,
                //                                null, new string[] { "SalePrice_PaymentMethod_Approval" });
                //            return false;
                //        }
                //        if (o_part > 0 && hasSalePricePaymentPartial == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3326,
                //                                null, new string[] { "SalePrice_PaymentMethod_Partial" });
                //            return false;
                //        }
                //        if (o_acct > 0 && hasSalePricePaymentAcceptance == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3327,
                //                                null, new string[] { "SalePrice_PaymentMethod_Acceptance" });
                //            return false;
                //        }

                //        if (o_appv > 0 && isCorrectSalePricePaymentApproval == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "SalePrice_PaymentMethod_Approval" });
                //            return false;
                //        }
                //        if (o_part > 0 && isCorrectSalePricePaymentPartial == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "SalePrice_PaymentMethod_Partial" });
                //            return false;
                //        }
                //        if (o_acct > 0 && isCorrectSalePricePaymentAcceptance == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "SalePrice_PaymentMethod_Acceptance" });
                //            return false;
                //        }
                //    }
                //}

                #endregion


                // Add by Jirawat jannet on 2016-12-02
                #region Validate 4.3: Validate Installation Fee

                #region Validate max data

                if (contract.NormalInstallFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.NormalInstallFeeUsd > 999999999999.99M)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233);
                        return false;
                    }
                }
                else
                {
                    if (contract.NormalInstallFee > 999999999999.99M)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233);
                        return false;
                    }
                }

                if (contract.OrderInstallFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.OrderInstallFeeUsd > 999999999999.99M)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233,
                                        null, new string[] { "OrderInstallFee" });
                        return false;
                    }
                }
                else
                {
                    if (contract.OrderInstallFee > 999999999999.99M)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233,
                                        null, new string[] { "OrderInstallFee" });
                        return false;
                    }
                }

                

                #endregion

                #region Validate 4.3.1: Validate order installation fee is 0

                if (contract.NormalInstallFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.NormalInstallFeeUsd > 0 && contract.OrderInstallFeeUsd == 0
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3329, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }
                else
                {
                    if (contract.NormalInstallFee > 0 && contract.OrderInstallFee == 0
                       && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3329, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }

                #endregion

                #region Validate 4.3.2: Valdiate The order installation price can not equal to summary of all installation price in billing timing


                if (contract.NormalProductPriceCurrencyType == contract.OrderProductPriceCurrencyType)
                {
                    decimal oinst = 0;
                    decimal oinst_ap = 0;
                    decimal oinst_p = 0;
                    decimal oinst_ac = 0;
                    bool skpinst = false;

                    #region Order Install Fee

                    if (contract.OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFeeUsd) == false)
                            oinst = contract.OrderInstallFeeUsd.Value;
                    }
                    else
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFee) == false)
                            oinst = contract.OrderInstallFee.Value;
                    }

                    #endregion
                    #region Billing Amount Approve Contract

                    if (contract.BillingAmtInstallation_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_ApproveContractUsd) == false)
                            oinst_ap = contract.BillingAmtInstallation_ApproveContractUsd.Value;
                    }
                    else
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_ApproveContract) == false)
                            oinst_ap = contract.BillingAmtInstallation_ApproveContract.Value;
                    }
                    if (oinst_ap > 0 && contract.BillingAmtInstallation_ApproveContractCurrencyType != contract.OrderInstallFeeCurrencyType)
                        skpinst = true;

                    #endregion
                    #region Billing Amount Partial Fee

                    if (contract.BillingAmtInstallation_PartialFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_PartialFeeUsd) == false)
                            oinst_p = contract.BillingAmtInstallation_PartialFeeUsd.Value;
                    }
                    else
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_PartialFee) == false)
                            oinst_p = contract.BillingAmtInstallation_PartialFee.Value;
                    }
                    if (oinst_p > 0 && contract.BillingAmtInstallation_PartialFeeCurrencyType != contract.OrderInstallFeeCurrencyType)
                        skpinst = true;

                    #endregion
                    #region Billing Amount Acceptance

                    if (contract.BillingAmtInstallation_AcceptanceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_AcceptanceUsd) == false)
                            oinst_ac = contract.BillingAmtInstallation_AcceptanceUsd.Value;
                    }
                    else
                    {
                        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_Acceptance) == false)
                            oinst_ac = contract.BillingAmtInstallation_Acceptance.Value;
                    }
                    if (oinst_ac > 0 && contract.BillingAmtInstallation_AcceptanceCurrencyType != contract.OrderInstallFeeCurrencyType)
                        skpinst = true;

                    #endregion

                    if (skpinst == false)
                    {
                        decimal? totalInstall = oinst_ap + oinst_p + oinst_ac;
                        if (oinst != totalInstall)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3330, null, new string[] { "OrderInstallFee" });
                            return false;
                        }
                    }
                }

                //if ((contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_ApproveContractCurrencyType
                //            && getPriceByCurrency(contract.BillingAmtInstallation_ApproveContract, contract.BillingAmtInstallation_ApproveContractUsd
                //                                    , contract.BillingAmtInstallation_ApproveContractCurrencyType) > 0)

                //        || (contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_PartialFeeCurrencyType
                //            && getPriceByCurrency(contract.BillingAmtInstallation_PartialFee, contract.BillingAmtInstallation_PartialFeeUsd
                //                                    , contract.BillingAmtInstallation_PartialFeeCurrencyType) > 0)

                //        || (contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_AcceptanceCurrencyType
                //            && getPriceByCurrency(contract.BillingAmtInstallation_Acceptance, contract.BillingAmtInstallation_AcceptanceUsd
                //                                    , contract.BillingAmtInstallation_AcceptanceCurrencyType) > 0))
                
                
                
                //if (contract.NormalInstallFeeCurrencyType == contract.OrderInstallFeeCurrencyType
                //    && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_ApproveContractCurrencyType
                //    && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_PartialFeeCurrencyType
                //    && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_AcceptanceCurrencyType)
                //{
                //    decimal billingInstallationPrice = 0;
                //    decimal orderInstallationPrice = 0;

                //    if (contract.OrderInstallFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                //    {
                //        billingInstallationPrice += contract.BillingAmtInstallation_ApproveContractUsd ?? 0;
                //        billingInstallationPrice += contract.BillingAmtInstallation_PartialFeeUsd ?? 0;
                //        billingInstallationPrice += contract.BillingAmtInstallation_AcceptanceUsd ?? 0;

                //        orderInstallationPrice = contract.OrderInstallFeeUsd ?? 0;
                //    }
                //    else
                //    {
                //        billingInstallationPrice += contract.BillingAmtInstallation_ApproveContract ?? 0;
                //        billingInstallationPrice += contract.BillingAmtInstallation_PartialFee ?? 0;
                //        billingInstallationPrice += contract.BillingAmtInstallation_Acceptance ?? 0;

                //        orderInstallationPrice = contract.OrderInstallFee ?? 0;
                //    }

                //    if (orderInstallationPrice != billingInstallationPrice)
                //    {

                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3330, null , new string[] { "OrderInstallFee" });
                //         return false;
                //    }
                //}

                #endregion

                #region Validate 4.3.3: Validate Normal installtion fee <> Order installation fee

                if (contract.NormalInstallFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.NormalInstallFeeUsd != contract.OrderInstallFeeUsd && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3331, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }
                else
                {
                    if (contract.NormalInstallFee != contract.OrderInstallFee && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3331, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }

                #endregion

                #region Validate 4.3.4: 

                if (contract.BillingAmtInstallation_ApproveContractCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.BillingAmtInstallation_ApproveContractUsd > 0
                        && CommonUtil.IsNullOrEmpty(billing.InstallationFee_PaymentMethod_Approval))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3332, null, new string[] { "InstallationFee_PaymentMethod_Approval" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmtInstallation_ApproveContract > 0
                        && CommonUtil.IsNullOrEmpty(billing.InstallationFee_PaymentMethod_Approval))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3332, null, new string[] { "InstallationFee_PaymentMethod_Approval" });
                         return false;
                    }
                }

                #endregion

                #region Validate 4.3.5

                if (contract.BillingAmtInstallation_PartialFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.BillingAmtInstallation_PartialFeeUsd > 0
                            && CommonUtil.IsNullOrEmpty(billing.InstallationFee_PaymentMethod_Partial))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3333, null, new string[] { "InstallationFee_PaymentMethod_Partial" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmtInstallation_PartialFee> 0
                            && CommonUtil.IsNullOrEmpty(billing.InstallationFee_PaymentMethod_Partial))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3333, null, new string[] { "InstallationFee_PaymentMethod_Partial" });
                         return false;
                    }
                }

                #endregion

                #region Valdiate 4.3.6

                if (contract.BillingAmtInstallation_AcceptanceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.BillingAmtInstallation_AcceptanceUsd > 0
                            && CommonUtil.IsNullOrEmpty(billing.InstallationFee_PaymentMethod_Acceptance))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3334, null, new string[] { "InstallationFee_PaymentMethod_Acceptance" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmtInstallation_Acceptance > 0
                            && CommonUtil.IsNullOrEmpty(billing.InstallationFee_PaymentMethod_Acceptance))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3334, null, new string[] { "InstallationFee_PaymentMethod_Acceptance" });
                         return false;
                    }
                }

                #endregion

                #region Validate 4.3.7

                if (contract.BillingAmtInstallation_ApproveContractCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.BillingAmtInstallation_ApproveContractUsd > 0
                            && billing.InstallationFee_PaymentMethod_Approval != PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.InstallationFee_PaymentMethod_Approval != PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "InstallationFee_PaymentMethod_Approval" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmtInstallation_ApproveContract > 0
                            && billing.InstallationFee_PaymentMethod_Approval != PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.InstallationFee_PaymentMethod_Approval != PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "InstallationFee_PaymentMethod_Approval" });
                         return false;
                    }
                }

                #endregion

                #region Validate 4.3.8

                if (contract.BillingAmtInstallation_PartialFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if(contract.BillingAmtInstallation_PartialFeeUsd > 0
                            && billing.InstallationFee_PaymentMethod_Partial != PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.InstallationFee_PaymentMethod_Partial != PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "InstallationFee_PaymentMethod_Partial" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmtInstallation_PartialFee > 0
                            && billing.InstallationFee_PaymentMethod_Partial != PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.InstallationFee_PaymentMethod_Partial != PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "InstallationFee_PaymentMethod_Partial" });
                         return false;
                    }
                }

                #endregion

                #region Validate 4.3.9

                if (contract.BillingAmtInstallation_AcceptanceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.BillingAmtInstallation_AcceptanceUsd > 0
                            && billing.InstallationFee_PaymentMethod_Acceptance != PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.InstallationFee_PaymentMethod_Acceptance != PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "InstallationFee_PaymentMethod_Acceptance" });
                         return false;
                    }
                }
                else
                {
                    if (contract.BillingAmtInstallation_Acceptance > 0
                            && billing.InstallationFee_PaymentMethod_Acceptance != PaymentMethodType.C_PAYMENT_METHOD_BANK_TRANSFER
                            && billing.InstallationFee_PaymentMethod_Acceptance != PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328, null, new string[] { "InstallationFee_PaymentMethod_Acceptance" });
                         return false;
                    }
                }

                #endregion

                #region Validate 4.3.10

                if (contract.NormalInstallFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                {
                    if (contract.NormalInstallFeeUsd > 0 && contract.OrderInstallFeeUsd > 0
                        && (
                            contract.OrderInstallFeeUsd <= contract.NormalInstallFeeUsd * 0.3M
                            || contract.OrderInstallFeeUsd >= contract.NormalInstallFeeUsd * 10M)
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3336, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }
                else
                {
                    if (contract.NormalInstallFee > 0 && contract.OrderInstallFee > 0
                        && (
                            contract.OrderInstallFee <= contract.NormalInstallFee * 0.3M
                            || contract.OrderInstallFee >= contract.NormalInstallFee * 10M)
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3336, null, new string[] { "ApproveNo1" });
                         return false;
                    }
                }

                #endregion

                #endregion

                // Comment by Jirawat Jannet on 2016-12-02
                #region Installation Fee

                //if (CommonUtil.IsNullOrEmpty(contract.NormalInstallFee) == false)
                //{
                //    if (contract.NormalInstallFee > 999999999999.99M)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233);
                //        return false;
                //    }
                //}

                //if (contract.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                //{
                //    if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFeeUsd) == false)
                //    {
                //        if (contract.OrderInstallFeeUsd > 999999999999.99M)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233,
                //                            null, new string[] { "OrderInstallFee" });
                //            return false;
                //        }
                //    }

                //    if ((CommonUtil.IsNullOrEmpty(contract.NormalInstallFee)
                //           || contract.NormalInstallFee == 0)
                //        && contract.OrderInstallFeeUsd > 0
                //        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3329,
                //                            null, new string[] { "ApproveNo1" });
                //        return false;
                //    }

                //    if (contract.NormalInstallFee != contract.OrderInstallFeeUsd
                //        && contract.OrderInstallFeeUsd > 0
                //        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3331,
                //                            null, new string[] { "ApproveNo1" });
                //        return false;
                //    }

                //    if (contract.NormalInstallFee > 0
                //        && contract.OrderInstallFeeUsd > 0
                //        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        decimal? fee30 = (contract.NormalInstallFee * 0.3M);
                //        decimal? fee1000 = (contract.OrderInstallFeeUsd * 10.0M);
                //        if (contract.OrderInstallFeeUsd <= fee30)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3336,
                //                                null, new string[] { "ApproveNo1" });
                //            return false;
                //        }
                //        if (contract.OrderInstallFeeUsd >= fee1000)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3336,
                //                                null, new string[] { "ApproveNo1" });
                //            return false;
                //        }
                //    }

                //    if (contract.NormalInstallFeeCurrencyType == contract.OrderInstallFeeCurrencyType
                //        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_ApproveContractCurrencyType
                //        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_PartialFeeCurrencyType
                //        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_AcceptanceCurrencyType)
                //    {
                //        decimal o_appv = 0;
                //        decimal o_part = 0;
                //        decimal o_acct = 0;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_ApproveContractUsd) == false)
                //            o_appv = contract.BillingAmtInstallation_ApproveContractUsd.Value;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_PartialFeeUsd) == false)
                //            o_part = contract.BillingAmtInstallation_PartialFeeUsd.Value;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_AcceptanceUsd) == false)
                //            o_acct = contract.BillingAmtInstallation_AcceptanceUsd.Value;

                //        decimal? totalInstallFee = o_appv + o_part + o_acct;
                //        if (contract.OrderInstallFeeUsd != totalInstallFee)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3330,
                //                                null, new string[] { "OrderProductPrice" });
                //            return false;
                //        }

                //        if (o_appv > 0 && hasInstallFeePaymentApproval == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3332,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Approval" });
                //            return false;
                //        }
                //        if (o_part > 0 && hasInstallFeePaymentPartial == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3333,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Partial" });
                //            return false;
                //        }
                //        if (o_acct > 0 && hasInstallFeePaymentAcceptance == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3334,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Acceptance" });
                //            return false;
                //        }

                //        if (o_appv > 0 && isCorrectInstallFeePaymentApproval == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Approval" });
                //            return false;
                //        }
                //        if (o_part > 0 && isCorrectInstallFeePaymentPartial == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Partial" });
                //            return false;
                //        }
                //        if (o_acct > 0 && isCorrectInstallFeePaymentAcceptance == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Acceptance" });
                //            return false;
                //        }
                //    }
                //}
                //else
                //{
                //    if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFee) == false)
                //    {
                //        if (contract.OrderInstallFee > 999999999999.99M)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3233,
                //                            null, new string[] { "OrderInstallFee" });
                //            return false;
                //        }
                //    }

                //    if ((CommonUtil.IsNullOrEmpty(contract.NormalInstallFee)
                //           || contract.NormalInstallFee == 0)
                //        && contract.OrderInstallFee > 0
                //        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3329,
                //                            null, new string[] { "ApproveNo1" });
                //        return false;
                //    }

                //    if (contract.NormalInstallFee != contract.OrderInstallFee
                //        && contract.OrderInstallFee > 0
                //        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3331,
                //                            null, new string[] { "ApproveNo1" });
                //        return false;
                //    }

                //    if (contract.NormalInstallFee > 0
                //        && contract.OrderInstallFee > 0
                //        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                //    {
                //        decimal? fee30 = (contract.NormalInstallFee * 0.3M);
                //        decimal? fee1000 = (contract.OrderInstallFee * 10.0M);
                //        if (contract.OrderInstallFee <= fee30)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3336,
                //                                null, new string[] { "ApproveNo1" });
                //            return false;
                //        }
                //        if (contract.OrderInstallFee >= fee1000)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3336,
                //                                null, new string[] { "ApproveNo1" });
                //            return false;
                //        }
                //    }

                //    if (contract.NormalInstallFeeCurrencyType == contract.OrderInstallFeeCurrencyType
                //        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_ApproveContractCurrencyType
                //        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_PartialFeeCurrencyType
                //        && contract.OrderInstallFeeCurrencyType == contract.BillingAmtInstallation_AcceptanceCurrencyType)
                //    {
                //        decimal o_appv = 0;
                //        decimal o_part = 0;
                //        decimal o_acct = 0;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_ApproveContract) == false)
                //            o_appv = contract.BillingAmtInstallation_ApproveContract.Value;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_PartialFee) == false)
                //            o_part = contract.BillingAmtInstallation_PartialFee.Value;
                //        if (CommonUtil.IsNullOrEmpty(contract.BillingAmtInstallation_Acceptance) == false)
                //            o_acct = contract.BillingAmtInstallation_Acceptance.Value;

                //        decimal? totalInstallFee = o_appv + o_part + o_acct;
                //        if (contract.OrderInstallFee != totalInstallFee)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3330,
                //                                null, new string[] { "OrderProductPrice" });
                //            return false;
                //        }

                //        if (o_appv > 0 && hasInstallFeePaymentApproval == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3332,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Approval" });
                //            return false;
                //        }
                //        if (o_part > 0 && hasInstallFeePaymentPartial == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3333,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Partial" });
                //            return false;
                //        }
                //        if (o_acct > 0 && hasInstallFeePaymentAcceptance == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3334,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Acceptance" });
                //            return false;
                //        }

                //        if (o_appv > 0 && isCorrectInstallFeePaymentApproval == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Approval" });
                //            return false;
                //        }
                //        if (o_part > 0 && isCorrectInstallFeePaymentPartial == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Partial" });
                //            return false;
                //        }
                //        if (o_acct > 0 && isCorrectInstallFeePaymentAcceptance == false)
                //        {
                //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3328,
                //                                null, new string[] { "InstallationFee_PaymentMethod_Acceptance" });
                //            return false;
                //        }
                //    }
                //}

                #endregion
                #region Project code

                if (CommonUtil.IsNullOrEmpty(contract.ProjectCode) == false)
                {
                    IProjectHandler phandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                    List<tbt_Project> pLst = phandler.GetTbt_Project(contract.ProjectCode);
                    if (pLst.Count == 0)
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0091,
                            new string[] { contract.ProjectCode },
                            new string[] { "ProjectCode" });
                        return false;
                    }
                }

                #endregion
                #region Connect to online

                if (contract.ConnectionFlag == true
                    && CommonUtil.IsNullOrEmpty(contract.ConnectTargetCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3213,
                                        null, new string[] { "ConnectTargetCode" });
                    return false;
                }
                else if (CommonUtil.IsNullOrEmpty(contract.ConnectTargetCode) == false)
                {
                    if (contract.ConnectTargetCode.StartsWith("Q")
                        || contract.ConnectTargetCode.StartsWith("q")
                        || contract.ConnectTargetCode.StartsWith("MA")
                        || contract.ConnectTargetCode.StartsWith("ma")
                        || contract.ConnectTargetCode.StartsWith("SG")
                        || contract.ConnectTargetCode.StartsWith("sg"))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3263,
                            new string[] { contract.ConnectTargetCodeShort },
                            new string[] { "ConnectTargetCode" });
                        return false;
                    }

                    IRentralContractHandler rhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                    List<tbt_RentalContractBasic> lst =
                        rhandler.GetTbt_RentalContractBasic(contract.ConnectTargetCode, null);
                    if (lst.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0118,
                            new string[] { contract.ConnectTargetCodeShort },
                            new string[] { "ConnectTargetCode" });
                        return false;
                    }
                }

                #endregion
                #region Distributed information

                if (contract.DistributedInstallTypeCode == DistributeType.C_DISTRIBUTE_TYPE_TARGET
                    && CommonUtil.IsNullOrEmpty(contract.DistributedOriginCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3214,
                                        null, new string[] { "DistributedOriginCode" });
                    return false;
                }
                else if (CommonUtil.IsNullOrEmpty(contract.DistributedOriginCode) == false)
                {
                    CommonUtil cmm = new CommonUtil();
                    ISaleContractHandler shandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                    List<tbt_SaleBasic> lst =
                        shandler.GetTbt_SaleBasic(
                            cmm.ConvertQuotationTargetCode(contract.DistributedOriginCode, CommonUtil.CONVERT_TYPE.TO_LONG), null, null);
                    if (lst.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0119,
                            new string[] { contract.DistributedOriginCode },
                            new string[] { "DistributedOriginCode" });
                        return false;
                    }
                    else if (lst[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3215,
                            null, new string[] { "DistributedOriginCode" });
                        return false;
                    }
                }

                #endregion
                #region Salesman

                object template = CommonUtil.CloneObject<tbt_DraftSaleContract, CTS020_RegisterSaleContractData_Employee>(contract);
                EmployeeExistList existLst = new EmployeeExistList();
                existLst.AddEmployee(template);

                IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                handler.CheckActiveEmployeeExist(existLst);

                string txt = existLst.EmployeeNoExist;
                if (txt != string.Empty)
                {
                    res.AddErrorMessage(
                        MessageUtil.MODULE_COMMON,
                        MessageUtil.MessageList.MSG0095,
                        new string[] { txt },
                        existLst.ControlNoExist);
                    return false;
                }

                #endregion
                #region Check Salesman is duplicate

                string[] salesmanLst = new[]
                {
                    contract.SalesmanEmpNo1,
                    contract.SalesmanEmpNo2,
                    contract.SalesmanEmpNo3,
                    contract.SalesmanEmpNo4,
                    contract.SalesmanEmpNo5,
                    contract.SalesmanEmpNo6,
                    contract.SalesmanEmpNo7,
                    contract.SalesmanEmpNo8,
                    contract.SalesmanEmpNo9,
                    contract.SalesmanEmpNo10
                };
                List<string> ctrslLst = new List<string>();
                for (int sidx = 0; sidx < salesmanLst.Length; sidx++)
                {
                    if (CommonUtil.IsNullOrEmpty(salesmanLst[sidx]))
                        continue;

                    List<string> cLst = new List<string>();
                    for (int ssidx = sidx + 1; ssidx < salesmanLst.Length; ssidx++)
                    {
                        if (salesmanLst[sidx] == salesmanLst[ssidx])
                        {
                            string ctrl = "SalesmanEmpNo" + (ssidx + 1).ToString();
                            if (ctrslLst.IndexOf(ctrl) < 0)
                                cLst.Add(ctrl);
                        }
                    }
                    if (cLst.Count > 0)
                    {
                        string ctrl = "SalesmanEmpNo" + (sidx + 1).ToString();
                        cLst.Insert(0, ctrl);
                        ctrslLst.AddRange(cLst);
                    }
                }
                if (ctrslLst.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3247,
                            null, ctrslLst.ToArray());
                    return false;
                }

                #endregion
                #region Approve No.

                string[] approveLst = new[]
                {
                    contract.ApproveNo1,
                    contract.ApproveNo2,
                    contract.ApproveNo3,
                    contract.ApproveNo4,
                    contract.ApproveNo5
                };
                bool[] nullLst = new[] { false, false, false, false, false };
                int maxValue = -1;
                for (int idx = 0; idx < 5; idx++)
                {
                    if (CommonUtil.IsNullOrEmpty(approveLst[idx]))
                    {
                        nullLst[idx] = true;
                    }
                    else
                        maxValue = idx;
                }

                List<string> ctrlLst = new List<string>();
                if (maxValue > 0)
                {
                    for (int idx = maxValue; idx >= 0; idx--)
                    {
                        if (nullLst[idx])
                            ctrlLst.Insert(0, "ApproveNo" + (idx + 1).ToString());
                    }
                }
                if (ctrlLst.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3009,
                            null, ctrlLst.ToArray());
                    return false;
                }

                #endregion
                #region Line-up type

                if (draft.doTbt_DraftSaleInstrument != null)
                {
                    foreach (tbt_DraftSaleInstrument inst in draft.doTbt_DraftSaleInstrument)
                    {
                        if (inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3295);
                            return false;
                        }
                    }
                }

                #endregion

                ICustomerMasterHandler chandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                #region Check Existing Contract customer

                CTS010_RegisterContractCutomer cust1v = CommonUtil.CloneObject<doCustomerWithGroup, CTS010_RegisterContractCutomer>(draft.doPurchaserCustomer);

                ValidatorUtil.BuildErrorMessage(res, new object[] { cust1v });
                if (res.IsError)
                    return false;

                if (CommonUtil.IsNullOrEmpty(cust1v.CustCode) == false)
                {
                    if (chandler.CheckActiveCustomerData(cust1v.CustCode) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0068, null, new string[] { "CPSearchCustCode" });
                        return false;
                    }
                }

                doCustomer cust1 = CommonUtil.CloneObject<CTS010_RegisterContractCutomer, doCustomer>(cust1v);
                chandler.ValidateCustomerData(cust1, true);
                if (cust1 != null)
                {
                    if (cust1.ValidateCustomerData == false)
                    {
                        CTS020_ScreenParameter param = CTS020_ScreenData;
                        param.doDraftSaleContractData.doPurchaserCustomer.ValidateCustomerData = false;
                        CTS020_ScreenData = param;

                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3256);
                        return false;
                    }
                }

                #endregion
                #region Check Existing Real customer

                CTS010_RegisterContractCutomer cust2v = CommonUtil.CloneObject<doCustomerWithGroup, CTS010_RegisterContractCutomer>(draft.doRealCustomer);

                ValidatorUtil.BuildErrorMessage(res, new object[] { cust2v });
                if (res.IsError)
                    return false;

                if (CommonUtil.IsNullOrEmpty(cust2v.CustCode) == false)
                {
                    if (chandler.CheckActiveCustomerData(cust2v.CustCode) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0068, null, new string[] { "CPSearchCustCode" });
                        return false;
                    }
                }

                doCustomer cust2 = CommonUtil.CloneObject<CTS010_RegisterContractCutomer, doCustomer>(cust2v);
                chandler.ValidateCustomerData(cust2, true);
                if (cust2 != null)
                {
                    if (cust2.ValidateCustomerData == false)
                    {
                        CTS020_ScreenParameter param = CTS020_ScreenData;
                        param.doDraftSaleContractData.doRealCustomer.ValidateCustomerData = false;
                        CTS020_ScreenData = param;

                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3257);
                        return false;
                    }
                }

                #endregion
                #region Check Duplicate Tax ID

                if (draft.doPurchaserCustomer != null
                    && draft.doRealCustomer != null)
                {
                    if (CTS010_IsSameCustomer(draft.doPurchaserCustomer, draft.doRealCustomer) == false)
                    {
                        if (CommonUtil.IsNullOrEmpty(draft.doPurchaserCustomer.IDNo) == false
                            && CommonUtil.IsNullOrEmpty(draft.doRealCustomer.IDNo) == false
                            && draft.doPurchaserCustomer.IDNo == draft.doRealCustomer.IDNo)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3229);
                            return false;
                        }
                    }
                }

                #endregion
                #region Site

                bool hasSiteCode = false;
                if (draft.doRealCustomer != null && draft.doSite != null)
                {
                    if (CommonUtil.IsNullOrEmpty(draft.doRealCustomer.CustCode) == false
                        && CommonUtil.IsNullOrEmpty(draft.doSite.SiteCode) == false)
                    {
                        hasSiteCode = true;
                    }
                }

                ISiteMasterHandler sshandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                if (hasSiteCode)
                {
                    if (sshandler.CheckExistSiteData(draft.doSite.SiteCode,
                                                        draft.doRealCustomer.CustCode) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2036, null, new string[] { "SiteCustCodeNo" });
                        return false;
                    }
                }
                else
                {
                    doSite s = CommonUtil.CloneObject<doSite, doSite>(draft.doSite);
                    sshandler.ValidateSiteData(s);
                    if (s != null)
                    {
                        if (s.ValidateSiteData == false)
                        {
                            CTS020_ScreenParameter param = CTS020_ScreenData;
                            param.doDraftSaleContractData.doSite.ValidateSiteData = false;
                            CTS020_ScreenData = param;

                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3258);
                            return false;
                        }
                    }
                }

                #endregion
                #region Payment method

                CTS020_UpdateBillingTargetData2 b = CommonUtil.CloneObject<CTS020_BillingTargetData, CTS020_UpdateBillingTargetData2>(billing);
                b.ApproveNo1 = contract.ApproveNo1;
                ValidatorUtil.BuildErrorMessage(res, new object[] { b });
                if (res.IsError)
                    return false;

                #endregion
                #region Contract payment term

                if (billing != null)
                {
                    List<string> paymentLst = new List<string>();
                    if (billing.SalePrice_Approval  > 0
                        || billing.SalePrice_ApprovalUsd > 0)
                    {
                        paymentLst.Add(billing.SalePrice_PaymentMethod_Approval);
                    }
                    if (billing.SalePrice_Partial > 0
                        || billing.SalePrice_PartialUsd > 0)
                    {
                        paymentLst.Add(billing.SalePrice_PaymentMethod_Partial);
                    }
                    if (billing.SalePrice_Acceptance > 0
                       || billing.SalePrice_AcceptanceUsd > 0)
                    {
                        paymentLst.Add(billing.SalePrice_PaymentMethod_Acceptance);
                    }
                    if (billing.InstallationFee_Approval > 0
                       || billing.InstallationFee_ApprovalUsd > 0)
                    {
                        paymentLst.Add(billing.InstallationFee_PaymentMethod_Approval);
                    }
                    if (billing.InstallationFee_Partial > 0
                       || billing.InstallationFee_PartialUsd > 0)
                    {
                        paymentLst.Add(billing.InstallationFee_PaymentMethod_Partial);
                    }
                    if (billing.InstallationFee_Acceptance > 0
                       || billing.InstallationFee_AcceptanceUsd > 0)
                    {
                        paymentLst.Add(billing.InstallationFee_PaymentMethod_Acceptance);
                    }

                    foreach (string payment in paymentLst)
                    {
                        if (CommonUtil.IsNullOrEmpty(payment))
                            continue;

                        if (payment != PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                             && payment != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                             && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3064);
                            return false;
                        }
                    }
                }

                #endregion

                //Add by Jutarat A. on 30042013
                #region Check exist installation basic data

                IInstallationHandler ihandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                string status = ihandler.GetInstallationStatus(contract.QuotationTargetCode);
                if (status != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3098,
                                        new string[] { contract.QuotationTargetCodeShort },
                                        new string[] { "ContractCode" });
                    return false;
                }

                #endregion
                //End Add

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Method for approve contract
        /// </summary>
        /// <param name="draft"></param>
        private void CTS020_ApproveContract(doDraftSaleContractData draft)
        {
            try
            {
                CTS020_ScreenParameter param = CTS020_ScreenData;
                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                IContractHandler cthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                ISaleContractHandler scthandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                IDraftSaleContractHandler shandler = ServiceContainer.GetService<IDraftSaleContractHandler>() as IDraftSaleContractHandler;
                ICommonHandler cmhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IRentralContractHandler rcthandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler; //Add by Jutarat A. on 18122012
                
                List<dtQuotationNoData> lstQuotationData = rcthandler.GetQuotationNo(draft.doTbt_DraftSaleContract.QuotationTargetCode, draft.doTbt_DraftSaleContract.Alphabet);
                #region Generate Contract code

                string ContractCode = draft.doTbt_DraftSaleContract.QuotationTargetCode;
                if (draft.doTbt_DraftSaleContract.SaleProcessType == SaleProcessType.C_SALE_PROCESS_TYPE_NEW_SALE)
                {
                    ContractCode = cthandler.GenerateContractCode(draft.doTbt_DraftSaleContract.ProductTypeCode);
                }

                #endregion
                #region Generate Security occurrence

                string OCC = scthandler.GenerateContractOCC(ContractCode);

                #endregion
                #region Update Draft contract

                draft.doTbt_DraftSaleContract.ContractCode = ContractCode;
                draft.doTbt_DraftSaleContract.OCC = OCC;
                shandler.EditDraftSaleContractData(draft);

                #endregion

                dsSaleContractData register = new dsSaleContractData();

                #region Sale contract basic

                tbt_SaleBasic basic = CommonUtil.CloneObject<tbt_DraftSaleContract, tbt_SaleBasic>(draft.doTbt_DraftSaleContract);
                if (basic != null)
                {
                    basic.ContractCode = ContractCode;
                    basic.OCC = OCC;
                    basic.SalesType = draft.doTbt_DraftSaleContract.SaleType;
                    basic.LatestOCCFlag = FlagType.C_FLAG_ON;

                    //Get and Add QuotationNo for save
                    if(lstQuotationData != null)
                        basic.QuotationNo = lstQuotationData[0].QuotationNo;

                    basic.ChangeType = SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE;
                    basic.ContractStatus = ContractStatus.C_CONTRACT_STATUS_BEF_START;
                    if (draft.doTbt_DraftSaleContract.SaleProcessType == SaleProcessType.C_SALE_PROCESS_TYPE_ADD_SALE)
                    {
                        basic.ChangeType = SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE;
                        basic.ContractStatus = ContractStatus.C_CONTRACT_STATUS_AFTER_START;
                    }

                    basic.SaleProcessManageStatus = SaleProcessManageStatus.C_SALE_PROCESS_STATUS_GENCODE;
                    basic.CounterNo = 0;
                    basic.InstallationCompleteFlag = FlagType.C_FLAG_OFF;
                    basic.MaintenanceContractFlag = FlagType.C_FLAG_OFF;

                    List<tbs_ProductType> lst = cmhandler.GetTbs_ProductType(ServiceType.C_SERVICE_TYPE_SALE, draft.doTbt_DraftSaleContract.ProductTypeCode);
                    if (lst.Count > 0)
                        basic.PrefixCode = lst[0].ContractPrefix;

                    basic.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                    basic.FirstContractDate = draft.doTbt_DraftSaleContract.ApproveContractDate;
                    basic.FirstContractEmpNo = dsTrans.dtUserData.EmpNo;

                    //basic.SaleAdjAmt = 0;
                    //basic.InstallFeePaidBySECOM = null;
                    //basic.InstallFeeRevenueBySECOM = null;

                    basic.ChangeImplementDate = null;
                    basic.NewAddInstallCompleteEmpNo = draft.doTbt_DraftSaleContract.Memo;

                    basic.QuotationStaffEmpNo = draft.doTbt_DraftSaleContract.QuotationStaffEmpNo;
                    basic.ExpectedCustAcceptanceDate = draft.doTbt_DraftSaleContract.ExpectedAcceptanceAgreeDate;
                    basic.BICContractCode = draft.doTbt_DraftSaleContract.BICContractCode;

                    basic.ContractConditionProcessDate = draft.doTbt_DraftSaleContract.CreateDate;
                    basic.ContractConditionProcessEmpNo = draft.doTbt_DraftSaleContract.CreateBy;

                    #region NewBldMgmtCost

                    basic.NewBldMgmtCostCurrencyType = draft.doTbt_DraftSaleContract.NewBldMgmtCostCurrencyType;
                    basic.NewBldMgmtCost = draft.doTbt_DraftSaleContract.NewBldMgmtCost;
                    basic.NewBldMgmtCost = draft.doTbt_DraftSaleContract.NewBldMgmtCostUsd;

                    #endregion
                    #region Normal Product Price

                    basic.NormalProductPriceCurrencyType = draft.doTbt_DraftSaleContract.NormalProductPriceCurrencyType;
                    basic.NormalProductPrice = draft.doTbt_DraftSaleContract.NormalProductPrice;
                    basic.NormalProductPriceUsd = draft.doTbt_DraftSaleContract.NormalProductPriceUsd;

                    #endregion
                    #region Normal Install Fee

                    basic.NormalInstallFeeCurrencyType = draft.doTbt_DraftSaleContract.NormalInstallFeeCurrencyType;
                    basic.NormalInstallFee = draft.doTbt_DraftSaleContract.NormalInstallFee;
                    basic.NormalInstallFeeUsd = draft.doTbt_DraftSaleContract.NormalInstallFeeUsd;

                    #endregion
                    #region Normal Sale Price

                    basic.NormalSalePriceCurrencyType = draft.doTbt_DraftSaleContract.NormalSalePriceCurrencyType;
                    basic.NormalSalePrice = draft.doTbt_DraftSaleContract.NormalSalePrice;
                    basic.NormalSalePriceUsd = draft.doTbt_DraftSaleContract.NormalSalePriceUsd;

                    #endregion
                    #region Order Product Price

                    basic.OrderProductPriceCurrencyType = draft.doTbt_DraftSaleContract.OrderProductPriceCurrencyType;
                    basic.OrderProductPrice = draft.doTbt_DraftSaleContract.OrderProductPrice;
                    basic.OrderProductPriceUsd = draft.doTbt_DraftSaleContract.OrderProductPriceUsd;

                    #endregion
                    #region Order Install Fee

                    basic.OrderInstallFeeCurrencyType = draft.doTbt_DraftSaleContract.OrderInstallFeeCurrencyType;
                    basic.OrderInstallFee = draft.doTbt_DraftSaleContract.OrderInstallFee;
                    basic.OrderInstallFeeUsd = draft.doTbt_DraftSaleContract.OrderInstallFeeUsd;

                    #endregion
                    #region Order Sale Price

                    basic.OrderSalePriceCurrencyType = draft.doTbt_DraftSaleContract.NormalSalePriceCurrencyType;
                    basic.OrderSalePrice = null;
                    basic.OrderSalePriceUsd = null;

                    #endregion
                    #region Billing Amt for Approve Contract

                    basic.BillingAmt_ApproveContractCurrencyType = draft.doTbt_DraftSaleContract.BillingAmt_ApproveContractCurrencyType;
                    basic.BillingAmt_ApproveContract = draft.doTbt_DraftSaleContract.BillingAmt_ApproveContract;
                    basic.BillingAmt_ApproveContractUsd = draft.doTbt_DraftSaleContract.BillingAmt_ApproveContractUsd;

                    #endregion
                    #region Billing Amt for Partial Fee

                    basic.BillingAmt_PartialFeeCurrencyType = draft.doTbt_DraftSaleContract.BillingAmt_PartialFeeCurrencyType;
                    basic.BillingAmt_PartialFee = draft.doTbt_DraftSaleContract.BillingAmt_PartialFee;
                    basic.BillingAmt_PartialFeeUsd = draft.doTbt_DraftSaleContract.BillingAmt_PartialFeeUsd;

                    #endregion
                    #region Billing Amt for Acceptance

                    basic.BillingAmt_AcceptanceCurrencyType = draft.doTbt_DraftSaleContract.BillingAmt_AcceptanceCurrencyType;
                    basic.BillingAmt_Acceptance = draft.doTbt_DraftSaleContract.BillingAmt_Acceptance;
                    basic.BillingAmt_AcceptanceUsd = draft.doTbt_DraftSaleContract.BillingAmt_AcceptanceUsd;

                    #endregion
                    #region Sale Adj Amt

                    basic.SaleAdjAmtCurrencyType = draft.doTbt_DraftSaleContract.NormalSalePriceCurrencyType;
                    basic.SaleAdjAmt = 0;
                    basic.SaleAdjAmtUsd = 0;

                    #endregion
                    #region Install Fee Paid by SECOM

                    basic.InstallFeePaidBySECOMCurrencyType = draft.doTbt_DraftSaleContract.NormalSalePriceCurrencyType;
                    basic.InstallFeePaidBySECOM = null;
                    basic.InstallFeePaidBySECOMUsd = null;

                    #endregion
                    #region Install Fee Revenue by SECOM

                    basic.InstallFeeRevenueBySECOMCurrencyType = draft.doTbt_DraftSaleContract.NormalSalePriceCurrencyType;
                    basic.InstallFeeRevenueBySECOM = null;
                    basic.InstallFeeRevenueBySECOMUsd = null;

                    #endregion
                    #region Bid Guarantee Amount 1

                    basic.BidGuaranteeAmount1CurrencyType = draft.doTbt_DraftSaleContract.BidGuaranteeAmount1CurrencyType;
                    basic.BidGuaranteeAmount1 = draft.doTbt_DraftSaleContract.BidGuaranteeAmount1;
                    basic.BidGuaranteeAmount1Usd = draft.doTbt_DraftSaleContract.BidGuaranteeAmount1Usd;

                    #endregion
                    #region Bid Guarantee Amount 2

                    basic.BidGuaranteeAmount2CurrencyType = draft.doTbt_DraftSaleContract.BidGuaranteeAmount2CurrencyType;
                    basic.BidGuaranteeAmount2 = draft.doTbt_DraftSaleContract.BidGuaranteeAmount2;
                    basic.BidGuaranteeAmount2Usd = draft.doTbt_DraftSaleContract.BidGuaranteeAmount2Usd;

                    #endregion
                    #region Billing Amt Installation for Approve Contract

                    basic.BillingAmtInstallation_ApproveContractCurrencyType = draft.doTbt_DraftSaleContract.BillingAmtInstallation_ApproveContractCurrencyType;
                    basic.BillingAmtInstallation_ApproveContract = draft.doTbt_DraftSaleContract.BillingAmtInstallation_ApproveContract;
                    basic.BillingAmtInstallation_ApproveContractUsd = draft.doTbt_DraftSaleContract.BillingAmtInstallation_ApproveContractUsd;

                    #endregion
                    #region Billing Amt Installation for Partial Fee

                    basic.BillingAmtInstallation_PartialFeeCurrencyType = draft.doTbt_DraftSaleContract.BillingAmtInstallation_PartialFeeCurrencyType;
                    basic.BillingAmtInstallation_PartialFee = draft.doTbt_DraftSaleContract.BillingAmtInstallation_PartialFee;
                    basic.BillingAmtInstallation_PartialFeeUsd = draft.doTbt_DraftSaleContract.BillingAmtInstallation_PartialFeeUsd;

                    #endregion
                    #region Billing Amt Installation for Acceptance

                    basic.BillingAmtInstallation_AcceptanceCurrencyType = draft.doTbt_DraftSaleContract.BillingAmtInstallation_AcceptanceCurrencyType;
                    basic.BillingAmtInstallation_Acceptance = draft.doTbt_DraftSaleContract.BillingAmtInstallation_Acceptance;
                    basic.BillingAmtInstallation_AcceptanceUsd = draft.doTbt_DraftSaleContract.BillingAmtInstallation_AcceptanceUsd;

                    #endregion
                }

                register.dtTbt_SaleBasic = new List<tbt_SaleBasic>() { basic };

                #endregion
                #region Sale Instrument details

                register.dtTbt_SaleInstrumentDetails = new List<tbt_SaleInstrumentDetails>();
                if (draft.doTbt_DraftSaleInstrument != null)
                {
                    foreach (tbt_DraftSaleInstrument inst in draft.doTbt_DraftSaleInstrument)
                    {
                        tbt_SaleInstrumentDetails rinst = CommonUtil.CloneObject<tbt_DraftSaleInstrument, tbt_SaleInstrumentDetails>(inst);

                        rinst.ContractCode = ContractCode;
                        rinst.OCC = OCC;

                        register.dtTbt_SaleInstrumentDetails.Add(rinst);
                    }
                }

                #endregion
                #region Relation type

                //register.dtTbt_RelationType = new List<tbt_RelationType>();
                //if (draft.doTbt_RelationType != null)
                //{
                //    foreach (tbt_RelationType rel in draft.doTbt_RelationType)
                //    {
                //        tbt_RelationType rrel = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(rel);

                //        rrel.ContractCode = ContractCode;
                //        rrel.OCC = OCC;

                //        register.dtTbt_RelationType.Add(rrel);
                //    }
                //}

                #endregion

                #region Update previous OCC

                if (draft.doTbt_DraftSaleContract.SaleProcessType == SaleProcessType.C_SALE_PROCESS_TYPE_ADD_SALE)
                {
                    dsSaleContractData prevDo = scthandler.GetEntireContract(basic.ContractCode, null);
                    if (prevDo.dtTbt_SaleBasic != null)
                    {
                        if (prevDo.dtTbt_SaleBasic.Count > 0)
                        {
                            prevDo.dtTbt_SaleBasic[0].LatestOCCFlag = false;
                            scthandler.UpdateTbt_SaleBasic(prevDo.dtTbt_SaleBasic[0]);
                        }
                    }
                }

                #endregion
                #region Update Sale Contract

                scthandler.InsertEntireContract(register);

                #endregion
                #region Lock Quotation Data

                IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                qhandler.LockQuotation(draft.doTbt_DraftSaleContract.QuotationTargetCode,
                                        draft.doTbt_DraftSaleContract.Alphabet,
                                        LockStyle.C_LOCK_STYLE_ALL);

                #endregion
                #region Update Document data

                IContractDocumentHandler cdchandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;

                List<tbt_ContractDocument> docLst = cdchandler.GetContractDocHeaderByQuotationCode(
                    draft.doTbt_DraftSaleContract.QuotationTargetCode,
                    draft.doTbt_DraftSaleContract.Alphabet,
                    null);
                if (docLst.Count > 0)
                {
                    foreach (tbt_ContractDocument doc in docLst)
                    {
                        doc.ContractCode = ContractCode;
                        doc.OCC = ParticularOCC.C_PARTICULAR_OCC_CONTRACT_REP_BEF_START;
                    }
                    cdchandler.UpdateTbt_ContractDocument(docLst);
                }
                else
                {
                    #region Document OCC

                    string DocOCC = cdchandler.GenerateDocOCC(ContractCode, ParticularOCC.C_PARTICULAR_OCC_CONTRACT_REP_BEF_START);

                    #endregion
                    #region Create Document Do

                    tbt_ContractDocument doc = new tbt_ContractDocument();
                    doc.QuotationTargetCode = draft.doTbt_DraftSaleContract.QuotationTargetCode;
                    doc.Alphabet = draft.doTbt_DraftSaleContract.Alphabet;
                    doc.ContractCode = ContractCode;
                    doc.OCC = OCC;
                    doc.ContractDocOCC = DocOCC;
                    doc.DocNo = CommonUtil.TextList(new string[]{   ContractCode, 
                                                                    OCC,
                                                                    DocOCC }, "-");
                    doc.DocumentCode = DocumentCode.C_DOCUMENT_CODE_ORDER_DOC;
                    doc.ContractOfficeCode = draft.doTbt_DraftSaleContract.ContractOfficeCode;
                    doc.OperationOfficeCode = draft.doTbt_DraftSaleContract.OperationOfficeCode;
                    doc.IssuedDate = dsTrans.dtOperationData.ProcessDateTime;
                    doc.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_ISSUED;
                    doc.DocAuditResult = DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED;

                    //Add by Jutarat A. on 18122012
                    IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                    List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_ORDER_DOC);
                    if (dLst.Count > 0)
                    {
                        doc.SubjectEN = dLst[0].DocumentNameEN;
                        doc.SubjectLC = dLst[0].DocumentNameLC;
                    }
                    //End Add

                    //Comment by Jutarat A. on 18122012 (Move to CreateContractDocData())
                    //List<tbt_ContractDocument> docResLst = cdchandler.InsertTbt_ContractDocument(new List<tbt_ContractDocument>() { doc });

                    #endregion

                    //Add by Jutarat A. on 18122012
                    //Create dtTbt_DocInstrumentDetails
                    List<tbt_DocInstrumentDetails> dtTbt_DocInstrumentDetails = null;
                    if (draft.doTbt_DraftSaleInstrument != null)
                    {
                        dtTbt_DocInstrumentDetails = new List<tbt_DocInstrumentDetails>();
                        foreach (tbt_DraftSaleInstrument detail in draft.doTbt_DraftSaleInstrument)
                        {
                            tbt_DocInstrumentDetails nd = new tbt_DocInstrumentDetails();
                            nd.DocID = doc.DocID;
                            nd.InstrumentCode = detail.InstrumentCode;
                            nd.InstrumentQty = detail.InstrumentQty;

                            dtTbt_DocInstrumentDetails.Add(nd);
                        }
                    }

                    //CreateContractDocData
                    dsContractDocData dsContractDoc = new dsContractDocData();
                    if (doc != null)
                        dsContractDoc.dtTbt_ContractDocument = new List<tbt_ContractDocument>() { doc };

                    if (dtTbt_DocInstrumentDetails != null && dtTbt_DocInstrumentDetails.Count > 0)
                        dsContractDoc.dtTbt_DocInstrumentDetail = dtTbt_DocInstrumentDetails;

                    rcthandler.CreateContractDocData(dsContractDoc);
                    //End Add
                }

                #endregion
                #region Update Contract Customer History Data

                tbt_ContractCustomerHistory custHist = new tbt_ContractCustomerHistory()
                {
                    ContractCode = ContractCode,
                    SequenceNo = 1,
                    ChangeDate = DateTime.Now,
                    OCC = OCC,
                    ChangeNameReasonType = null,
                    BranchNameEN = draft.doTbt_DraftSaleContract.BranchNameEN,
                    BranchNameLC = draft.doTbt_DraftSaleContract.BranchNameLC,
                    BranchAddressEN = draft.doTbt_DraftSaleContract.BranchAddressEN,
                    BranchAddressLC = draft.doTbt_DraftSaleContract.BranchAddressLC,
                    CSChangeFlag = false,
                    RCChangeFlag = false,
                    SiteChangeFlag = false
                };

                if (draft.doPurchaserCustomer != null)
                {
                    custHist.CSCustCode = draft.doPurchaserCustomer.CustCode;
                    custHist.ContractSignerTypeCode = draft.doTbt_DraftSaleContract.PurchaserSignerTypeCode;
                    custHist.CSAddressEN = draft.doPurchaserCustomer.AddressEN;
                    custHist.CSAddressFullEN = draft.doPurchaserCustomer.AddressFullEN;
                    custHist.CSAddressFullLC = draft.doPurchaserCustomer.AddressFullLC;
                    custHist.CSAddressLC = draft.doPurchaserCustomer.AddressLC;
                    custHist.CSAlleyEN = draft.doPurchaserCustomer.AlleyEN;
                    custHist.CSAlleyLC = draft.doPurchaserCustomer.AlleyLC;
                    custHist.CSBusinessTypeCode = draft.doPurchaserCustomer.BusinessTypeCode;
                    custHist.CSCompanyTypeCode = draft.doPurchaserCustomer.CompanyTypeCode;
                    custHist.CSContactPersonName = draft.doPurchaserCustomer.ContactPersonName;
                    custHist.CSCustFullNameEN = draft.doPurchaserCustomer.CustFullNameEN;
                    custHist.CSCustFullNameLC = draft.doPurchaserCustomer.CustFullNameLC;
                    custHist.CSCustNameEN = draft.doPurchaserCustomer.CustNameEN;
                    custHist.CSCustNameLC = draft.doPurchaserCustomer.CustNameLC;
                    custHist.CSCustStatus = draft.doPurchaserCustomer.CustStatus;
                    custHist.CSCustTypeCode = draft.doPurchaserCustomer.CustTypeCode;
                    custHist.CSDistrictCode = draft.doPurchaserCustomer.DistrictCode;
                    custHist.CSDummyIDFlag = draft.doPurchaserCustomer.DummyIDFlag;
                    custHist.CSFaxNo = draft.doPurchaserCustomer.FaxNo;
                    custHist.CSFinancialMarketTypeCode = draft.doPurchaserCustomer.FinancialMarketTypeCode;
                    custHist.CSIDNo = draft.doPurchaserCustomer.IDNo;
                    custHist.CSImportantFlag = draft.doPurchaserCustomer.ImportantFlag;
                    custHist.CSMemo = draft.doPurchaserCustomer.Memo;
                    custHist.CSPhoneNo = draft.doPurchaserCustomer.PhoneNo;
                    custHist.CSProvinceCode = draft.doPurchaserCustomer.ProvinceCode;
                    custHist.CSRegionCode = draft.doPurchaserCustomer.RegionCode;
                    custHist.CSRepPersonName = draft.doPurchaserCustomer.RepPersonName;
                    custHist.CSRoadEN = draft.doPurchaserCustomer.RoadEN;
                    custHist.CSRoadLC = draft.doPurchaserCustomer.RoadLC;
                    custHist.CSSECOMContactPerson = draft.doPurchaserCustomer.SECOMContactPerson;
                    custHist.CSSubDistrictEN = draft.doPurchaserCustomer.SubDistrictEN;
                    custHist.CSSubDistrictLC = draft.doPurchaserCustomer.SubDistrictLC;
                    custHist.CSURL = draft.doPurchaserCustomer.URL;
                    custHist.CSZipCode = draft.doPurchaserCustomer.ZipCode;
                }
                if (draft.doRealCustomer != null)
                {
                    custHist.RCCustCode = draft.doRealCustomer.CustCode;
                    custHist.RCAddressEN = draft.doRealCustomer.AddressEN;
                    custHist.RCAddressFullEN = draft.doRealCustomer.AddressFullEN;
                    custHist.RCAddressFullLC = draft.doRealCustomer.AddressFullLC;
                    custHist.RCAddressLC = draft.doRealCustomer.AddressLC;
                    custHist.RCAlleyEN = draft.doRealCustomer.AlleyEN;
                    custHist.RCAlleyLC = draft.doRealCustomer.AlleyLC;
                    custHist.RCBusinessTypeCode = draft.doRealCustomer.BusinessTypeCode;
                    custHist.RCCompanyTypeCode = draft.doRealCustomer.CompanyTypeCode;
                    custHist.RCContactPersonName = draft.doRealCustomer.ContactPersonName;
                    custHist.RCCustFullNameEN = draft.doRealCustomer.CustFullNameEN;
                    custHist.RCCustFullNameLC = draft.doRealCustomer.CustFullNameLC;
                    custHist.RCCustNameEN = draft.doRealCustomer.CustNameEN;
                    custHist.RCCustNameLC = draft.doRealCustomer.CustNameLC;
                    custHist.RCCustStatus = draft.doRealCustomer.CustStatus;
                    custHist.RCCustTypeCode = draft.doRealCustomer.CustTypeCode;
                    custHist.RCDistrictCode = draft.doRealCustomer.DistrictCode;
                    custHist.RCDummyIDFlag = draft.doRealCustomer.DummyIDFlag;
                    custHist.RCFaxNo = draft.doRealCustomer.FaxNo;
                    custHist.RCFinancialMarketTypeCode = draft.doRealCustomer.FinancialMarketTypeCode;
                    custHist.RCIDNo = draft.doRealCustomer.IDNo;
                    custHist.RCImportantFlag = draft.doRealCustomer.ImportantFlag;
                    custHist.RCMemo = draft.doRealCustomer.Memo;
                    custHist.RCPhoneNo = draft.doRealCustomer.PhoneNo;
                    custHist.RCProvinceCode = draft.doRealCustomer.ProvinceCode;
                    custHist.RCRegionCode = draft.doRealCustomer.RegionCode;
                    custHist.RCRepPersonName = draft.doRealCustomer.RepPersonName;
                    custHist.RCRoadEN = draft.doRealCustomer.RoadEN;
                    custHist.RCRoadLC = draft.doRealCustomer.RoadLC;
                    custHist.RCSECOMContactPerson = draft.doRealCustomer.SECOMContactPerson;
                    custHist.RCSubDistrictEN = draft.doRealCustomer.SubDistrictEN;
                    custHist.RCSubDistrictLC = draft.doRealCustomer.SubDistrictLC;
                    custHist.RCURL = draft.doRealCustomer.URL;
                    custHist.RCZipCode = draft.doRealCustomer.ZipCode;
                }
                if (draft.doSite != null)
                {
                    custHist.SiteCode = draft.doSite.SiteCode;
                    custHist.SiteCustCode = draft.doSite.CustCode;
                    custHist.SiteAddressEN = draft.doSite.AddressEN;
                    custHist.SiteAddressFullEN = draft.doSite.AddressFullEN;
                    custHist.SiteAddressFullLC = draft.doSite.AddressFullLC;
                    custHist.SiteAddressLC = draft.doSite.AddressLC;
                    custHist.SiteAlleyEN = draft.doSite.AlleyEN;
                    custHist.SiteAlleyLC = draft.doSite.AlleyLC;
                    custHist.SiteBuildingUsageCode = draft.doSite.BuildingUsageCode;
                    custHist.SiteDistrictCode = draft.doSite.DistrictCode;
                    custHist.SitePhoneNo = draft.doSite.PhoneNo;
                    custHist.SiteProvinceCode = draft.doSite.ProvinceCode;
                    custHist.SiteRoadEN = draft.doSite.RoadEN;
                    custHist.SiteRoadLC = draft.doSite.RoadLC;
                    custHist.SiteSECOMContactPerson = draft.doSite.SECOMContactPerson;
                    custHist.SiteSubDistrictEN = draft.doSite.SubDistrictEN;
                    custHist.SiteSubDistrictLC = draft.doSite.SubDistrictLC;
                    custHist.SiteZipCode = draft.doSite.ZipCode;
                    custHist.SiteNameEN = draft.doSite.SiteNameEN;
                    custHist.SiteNameLC = draft.doSite.SiteNameLC;
                    custHist.SiteNo = draft.doSite.SiteNo;
                    custHist.SitePersonInCharge = draft.doSite.PersonInCharge;
                }

                ICommonContractHandler cmmchandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                cmmchandler.InsertTbt_ContractCustomerHistory(new List<tbt_ContractCustomerHistory>() { custHist });

                #endregion

                #region Update AR

                tbt_AR ar = new tbt_AR()
                {
                    QuotationTargetCode = draft.doTbt_DraftSaleContract.QuotationTargetCode,
                    ContractCode = draft.doTbt_DraftSaleContract.ContractCode
                };

                IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
                arhandler.UpdateContractCode(ar);

                #endregion
                #region Keep billing data

                IBillingTempHandler bthandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

                // Get total billing temp.
                List<tbt_BillingTemp> btLst = bthandler.GetTbt_BillingTemp(basic.ContractCode, basic.OCC);
                if (draft.doTbt_DraftSaleBillingTarget != null)
                {
                    int count = btLst.Count;
                    foreach (tbt_DraftSaleBillingTarget bt in draft.doTbt_DraftSaleBillingTarget)
                    {
                        tbt_BillingTemp temp = new tbt_BillingTemp()
                        {
                            ContractCode = basic.ContractCode,
                            OCC = basic.OCC,
                            SequenceNo = count + 1,
                            BillingClientCode = bt.BillingClientCode,
                            BillingTargetCode = bt.BillingTargetCode,
                            BillingOfficeCode = bt.BillingOfficeCode,
                            DocLanguage = bt.DocLanguage, //Add by Jutarat A. on 20122013
                            //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE,
                            BillingType = bt.BillingType,
                            CreditTerm = null,
                            BillingTiming = bt.BillingTiming,

                            BillingAmtCurrencyType = bt.BillingAmtCurrencyType,
                            BillingAmt = bt.BillingAmt,
                            BillingAmtUsd = bt.BillingAmtUsd,

                            PayMethod = bt.PayMethod,
                            BillingCycle = null,
                            CalDailyFeeStatus = null,
                            SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                        };

                        bthandler.InsertBillingTemp(temp);
                        count++;
                    }
                }

                #endregion
                #region Send billing data

                IBillingInterfaceHandler bihandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                bihandler.SendBilling_SaleApprove(draft.doTbt_DraftSaleContract.ContractCode, draft.doTbt_DraftSaleContract.OCC);

                #endregion

#if !ROUND1

                #region Generate Installation basic

                doGenInstallationBasic instBasic = new doGenInstallationBasic()
                {
                    ContractProjectCode = basic.ContractCode,
                    InstallationType = draft.doTbt_DraftSaleContract.SaleProcessType == SaleProcessType.C_SALE_PROCESS_TYPE_ADD_SALE?
                                             SaleInstallationType.C_SALE_INSTALL_TYPE_ADD : SaleInstallationType.C_SALE_INSTALL_TYPE_NEW,
                    OCC = basic.OCC,
                    ServiceTypeCode = basic.ServiceTypeCode,
                    OperationOfficeCode = basic.OperationOfficeCode,
                    SecurityTypeCode = null,
                    NormalInstallFee = basic.NormalInstallFee != null ? basic.NormalInstallFee.Value : 0,
                    NormalContractFee = basic.NormalSalePrice != null ? basic.NormalSalePrice.Value : 0,
                    ApproveNo1 = basic.ApproveNo1,
                    ApproveNo2 = basic.ApproveNo2
                };

                if (draft.doTbt_DraftSaleInstrument != null)
                {
                    instBasic.doInstrumentDetails = new List<tbt_InstallationInstrumentDetails>();
                    foreach (tbt_DraftSaleInstrument inst in draft.doTbt_DraftSaleInstrument)
                    {
                        instBasic.doInstrumentDetails.Add(new tbt_InstallationInstrumentDetails()
                        {
                            ContractCode = basic.ContractCode,
                            InstrumentCode = inst.InstrumentCode,
                            InstrumentTypeCode = inst.InstrumentTypeCode,
                            ContractInstalledQty = inst.InstrumentQty,
                            ContractMovedQty = 0,
                            ContractRemovedQty = 0
                        });
                    }
                }

                IInstallationHandler ihandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                if (!param.OneShotFlag)
                {
                    ihandler.GenerateInstallationBasic(instBasic);
                }


                #endregion
                #region Book instrument

                if (draft.doTbt_DraftSaleInstrument != null)
                {
                    if (draft.doTbt_DraftSaleInstrument.Count > 0)
                    {
                        doBooking booking = new doBooking()
                        {
                            ContractCode = basic.ContractCode,
                            ExpectedStartServiceDate = basic.ExpectedInstallCompleteDate != null ? basic.ExpectedInstallCompleteDate.Value : DateTime.Now
                        };

                        booking.InstrumentCode = new List<string>();
                        booking.InstrumentQty = new List<int>();
                        foreach (tbt_DraftSaleInstrument inst in draft.doTbt_DraftSaleInstrument)
                        {
                            booking.InstrumentCode.Add(inst.InstrumentCode);
                            booking.InstrumentQty.Add(inst.InstrumentQty != null ? inst.InstrumentQty.Value : 0);
                        }

                        IInventoryHandler ivhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                        if (!param.OneShotFlag)
                            ivhandler.NewBooking(booking);
                    }
                }

                #endregion

#endif
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Reject E-mail template
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        private doEmailTemplate CTS020_GenerateRejectMailTemplate(doDraftSaleContractData draft)
        {
            try
            {
                doRejectContractEmailObject obj = new doRejectContractEmailObject();
                obj.QuotationTargetCode = draft.doTbt_DraftSaleContract.QuotationTargetCodeFull;

                #region Customer / Site

                if (draft.doPurchaserCustomer != null)
                {
                    obj.CustFullNameEN = draft.doPurchaserCustomer.CustFullNameEN;
                    obj.CustFullNameLC = draft.doPurchaserCustomer.CustFullNameLC;
                }
                if (draft.doSite != null)
                {
                    obj.SiteNameEN = draft.doSite.SiteNameEN;
                    obj.SiteNameLC = draft.doSite.SiteNameLC;
                }

                #endregion
                #region Set Product Name

                IProductMasterHandler mhandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                List<View_tbm_Product> pLst = mhandler.GetTbm_ProductByLanguage(
                        draft.doTbt_DraftSaleContract.ProductCode,
                        draft.doTbt_DraftSaleContract.ProductTypeCode);

                if (pLst.Count > 0)
                {
                    obj.ProductNameEN = pLst[0].ProductNameEN;
                    obj.ProductNameLC = pLst[0].ProductNameLC;
                }

                #endregion
                #region Set Employee Name

                List<tbm_Employee> empLst = new List<tbm_Employee>();
                if (CommonUtil.IsNullOrEmpty(draft.doTbt_DraftSaleContract.SalesmanEmpNo1) == false)
                    empLst.Add(new tbm_Employee() { EmpNo = draft.doTbt_DraftSaleContract.SalesmanEmpNo1 });
                if (CommonUtil.IsNullOrEmpty(draft.doTbt_DraftSaleContract.SalesmanEmpNo2) == false)
                    empLst.Add(new tbm_Employee() { EmpNo = draft.doTbt_DraftSaleContract.SalesmanEmpNo2 });

                IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                empLst = emphandler.GetEmployeeList(empLst);
                foreach (tbm_Employee emp in empLst)
                {
                    if (emp.EmpNo == draft.doTbt_DraftSaleContract.SalesmanEmpNo1)
                    {
                        obj.SalesmanEmpNameENNo1 = CommonUtil.TextFullName(emp.EmpFirstNameEN, emp.EmpLastNameEN);
                        obj.SalesmanEmpNameLCNo1 = CommonUtil.TextFullName(emp.EmpFirstNameLC, emp.EmpLastNameLC);
                    }
                    else if (emp.EmpNo == draft.doTbt_DraftSaleContract.SalesmanEmpNo2)
                    {
                        obj.SalesmanEmpNameENNo2 = CommonUtil.TextFullName(emp.EmpFirstNameEN, emp.EmpLastNameEN);
                        obj.SalesmanEmpNameLCNo2 = CommonUtil.TextFullName(emp.EmpFirstNameLC, emp.EmpLastNameLC);
                    }
                }

                #endregion
                #region Office

                List<OfficeDataDo> officeLst = CommonUtil.dsTransData.dtOfficeData;
                foreach (OfficeDataDo office in officeLst)
                {
                    if (office.OfficeCode == draft.doTbt_DraftSaleContract.ContractOfficeCode)
                    {
                        obj.ContractOfficeNameEN = office.OfficeNameEN;
                        obj.ContractOfficeNameLC = office.OfficeNameLC;
                    }
                    if (office.OfficeCode == draft.doTbt_DraftSaleContract.OperationOfficeCode)
                    {
                        obj.OperationOfficeNameEN = office.OfficeNameEN;
                        obj.OperationOfficeNameLC = office.OfficeNameLC;
                    }
                }

                #endregion

                EmailTemplateUtil util = new EmailTemplateUtil(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_REJECT_CONTRACT);
                return util.LoadTemplate(obj);
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Approve E-mail template
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        private doEmailTemplate CTS020_GenerateApproveMailTemplate(doDraftSaleContractData draft)
        {
            try
            {
                doApproveContractEmailObject obj = new doApproveContractEmailObject();
                obj.ContractCode = draft.doTbt_DraftSaleContract.ContractCodeShort;

                #region Customer / Site

                if (draft.doPurchaserCustomer != null)
                {
                    obj.CustFullNameEN = draft.doPurchaserCustomer.CustFullNameEN;
                    obj.CustFullNameLC = draft.doPurchaserCustomer.CustFullNameLC;
                }
                if (draft.doSite != null)
                {
                    obj.SiteNameEN = draft.doSite.SiteNameEN;
                    obj.SiteNameLC = draft.doSite.SiteNameLC;
                }

                #endregion
                #region Set Product Name

                IProductMasterHandler mhandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                List<View_tbm_Product> pLst = mhandler.GetTbm_ProductByLanguage(
                        draft.doTbt_DraftSaleContract.ProductCode,
                        draft.doTbt_DraftSaleContract.ProductTypeCode);

                if (pLst.Count > 0)
                {
                    obj.ProductNameEN = pLst[0].ProductNameEN;
                    obj.ProductNameLC = pLst[0].ProductNameLC;
                }

                #endregion
                #region Set Employee Name

                List<tbm_Employee> empLst = new List<tbm_Employee>();
                if (CommonUtil.IsNullOrEmpty(draft.doTbt_DraftSaleContract.SalesmanEmpNo1) == false)
                    empLst.Add(new tbm_Employee() { EmpNo = draft.doTbt_DraftSaleContract.SalesmanEmpNo1 });
                if (CommonUtil.IsNullOrEmpty(draft.doTbt_DraftSaleContract.SalesmanEmpNo2) == false)
                    empLst.Add(new tbm_Employee() { EmpNo = draft.doTbt_DraftSaleContract.SalesmanEmpNo2 });

                IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                empLst = emphandler.GetEmployeeList(empLst);
                foreach (tbm_Employee emp in empLst)
                {
                    if (emp.EmpNo == draft.doTbt_DraftSaleContract.SalesmanEmpNo1)
                    {
                        obj.SalesmanEmpNameENNo1 = CommonUtil.TextFullName(emp.EmpFirstNameEN, emp.EmpLastNameEN);
                        obj.SalesmanEmpNameLCNo1 = CommonUtil.TextFullName(emp.EmpFirstNameLC, emp.EmpLastNameLC);
                    }
                    else if (emp.EmpNo == draft.doTbt_DraftSaleContract.SalesmanEmpNo2)
                    {
                        obj.SalesmanEmpNameENNo2 = CommonUtil.TextFullName(emp.EmpFirstNameEN, emp.EmpLastNameEN);
                        obj.SalesmanEmpNameLCNo2 = CommonUtil.TextFullName(emp.EmpFirstNameLC, emp.EmpLastNameLC);
                    }
                }

                #endregion
                #region Office

                List<OfficeDataDo> officeLst = CommonUtil.dsTransData.dtOfficeData;
                foreach (OfficeDataDo office in officeLst)
                {
                    if (office.OfficeCode == draft.doTbt_DraftSaleContract.ContractOfficeCode)
                    {
                        obj.ContractOfficeNameEN = office.OfficeNameEN;
                        obj.ContractOfficeNameLC = office.OfficeNameLC;
                    }
                    if (office.OfficeCode == draft.doTbt_DraftSaleContract.OperationOfficeCode)
                    {
                        obj.OperationOfficeNameEN = office.OfficeNameEN;
                        obj.OperationOfficeNameLC = office.OfficeNameLC;
                    }
                }

                #endregion

                EmailTemplateUtil util = new EmailTemplateUtil(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_APPROVE_CONTRACT);
                return util.LoadTemplate(obj);
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Check customer has changed?
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="real"></param>
        /// <returns></returns>
        private bool CTS020_IsSameCustomer(doCustomerWithGroup contract, doCustomerWithGroup real)
        {
            try
            {
                if (contract != null && real != null)
                {
                    if (CommonUtil.IsNullOrEmpty(contract.CustCode) && CommonUtil.IsNullOrEmpty(real.CustCode))
                    {
                        bool same = true;
                        PropertyInfo[] props = typeof(doCustomerWithGroup).GetProperties();
                        foreach (PropertyInfo prop in props)
                        {
                            if (prop.CanWrite == false)
                                continue;

                            if (prop.Name == "CustomerGroupData")
                            {
                                if (contract.CustomerGroupData != null
                                    && real.CustomerGroupData != null)
                                {
                                    if (contract.CustomerGroupData.Count == real.CustomerGroupData.Count)
                                    {
                                        foreach (dtCustomeGroupData gc in contract.CustomerGroupData)
                                        {
                                            bool sameGroup = false;
                                            foreach (dtCustomeGroupData gr in real.CustomerGroupData)
                                            {
                                                if (gc.GroupCode == gr.GroupCode)
                                                {
                                                    sameGroup = true;
                                                    break;
                                                }
                                            }
                                            if (sameGroup == false)
                                            {
                                                same = false;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                        same = false;
                                }
                                else
                                    same = false;
                            }
                            else
                            {
                                object obj1 = prop.GetValue(contract, null);
                                object obj2 = prop.GetValue(real, null);

                                if (CommonUtil.IsNullOrEmpty(obj1) == false || CommonUtil.IsNullOrEmpty(obj2) == false)
                                {
                                    same = false;
                                    if (CommonUtil.IsNullOrEmpty(obj1) == false && CommonUtil.IsNullOrEmpty(obj2) == false)
                                    {
                                        if (obj1.ToString() == obj2.ToString())
                                            same = true;
                                    }

                                    if (same == false)
                                        break;
                                }
                            }
                        }

                        return same;
                    }
                    else
                    {
                        return contract.CustCode == real.CustCode;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check billing client has changed?
        /// </summary>
        /// <param name="client"></param>
        /// <param name="new_client"></param>
        /// <returns></returns>
        private bool CTS020_IsSameBillingClient(tbm_BillingClient currentC, tbm_BillingClient newC)
        {
            try
            {
                if (currentC != null && newC != null)
                {
                    bool same = true;
                    string[] ignoreProperty = new string[] {
                        "CreateDate",
                        "CreateBy",
                        "UpdateDate",
                        "UpdateBy"
                    };
                    PropertyInfo[] props = typeof(tbm_BillingClient).GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.CanWrite == false || ignoreProperty.Contains(prop.Name))
                            continue;

                        object obj1 = prop.GetValue(currentC, null);
                        object obj2 = prop.GetValue(newC, null);

                        if (CommonUtil.IsNullOrEmpty(obj1) == false || CommonUtil.IsNullOrEmpty(obj2) == false)
                        {
                            same = false;
                            if (CommonUtil.IsNullOrEmpty(obj1) == false && CommonUtil.IsNullOrEmpty(obj2) == false)
                            {
                                if (obj1.ToString() == obj2.ToString())
                                    same = true;
                            }

                            if (same == false)
                                break;
                        }
                    }

                    return same;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
