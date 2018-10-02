using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract.CustomEntity;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Contract.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Master;
using System.Collections.Generic;
using System.Transactions;
using SECOM_AJIS.Common.Models.EmailTemplates;
using System.Linq;
using System.Reflection;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        private const string CTS010_SCREEN_NAME = "CTS010";

        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS010_Authority(CTS010_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                #region Check Authority

                bool hasPermission = false;
                if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.NEW)
                    hasPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_FN99, FunctionID.C_FUNC_ID_ADD);
                else if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.EDIT)
                    hasPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_FN99, FunctionID.C_FUNC_ID_EDIT);
                else if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE)
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

                if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE)
                {
                    CTS010_RetrieveQuotationCondition_Edit cond = new CTS010_RetrieveQuotationCondition_Edit();
                    cond.QuotationTargetCode = param.QuotationTargetCode;

                    #region Validate

                    ValidatorUtil.BuildErrorMessage(res, new object[] { cond });
                    if (res.IsError)
                        return Json(res);

                    #endregion
                    #region Retrieve Data

                    IDraftRentalContractHandler rhandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;

                    doDraftRentalContractData draftData = rhandler.GetEntireDraftRentalContract(cond, doDraftRentalContractData.RENTAL_CONTRACT_MODE.APPROVE);
                    if (draftData == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011,
                            new string[] { cond.QuotationTargetCode });
                    }
                    else
                    {
                        param.doDraftRentalContractData = draftData;

                        param.doRegisterDraftRentalContractData = new doDraftRentalContractData();
                        param.doRegisterDraftRentalContractData.doContractCustomer = 
                            CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(draftData.doContractCustomer);
                        param.doRegisterDraftRentalContractData.doRealCustomer = 
                            CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(draftData.doRealCustomer);
                        param.doRegisterDraftRentalContractData.doSite = 
                            CommonUtil.CloneObject<doSite, doSite>(draftData.doSite);
                        param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail =
                            CommonUtil.ClonsObjectList<tbt_DraftRentalEmail, tbt_DraftRentalEmail>(draftData.doTbt_DraftRentalEmail);
                        param.doRegisterDraftRentalContractData.Mode = draftData.Mode;
                        param.doRegisterDraftRentalContractData.LastUpdateDateQuotationData = draftData.LastUpdateDateQuotationData;

                        param.FirstLoad = false;
                    }

                    #endregion
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS010_ScreenParameter>(CTS010_SCREEN_NAME, param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize(CTS010_SCREEN_NAME)]
        public ActionResult CTS010()
        {
            ViewBag.ScreenMode = (int)CTS010_ScreenParameter.SCREEN_MODE.NEW;
            ViewBag.HideChangeQuotationButton = false;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    #region Retrieve Data for Approve

                    if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE && param.FirstLoad)
                    {
                        CTS010_RetrieveQuotationCondition_Edit cond = new CTS010_RetrieveQuotationCondition_Edit();
                        cond.QuotationTargetCode = param.QuotationTargetCode;

                        IDraftRentalContractHandler rhandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
                        param.doDraftRentalContractData = rhandler.GetEntireDraftRentalContract(cond, doDraftRentalContractData.RENTAL_CONTRACT_MODE.APPROVE);

                        param.doRegisterDraftRentalContractData = new doDraftRentalContractData();
                        param.doRegisterDraftRentalContractData.doContractCustomer =
                            CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(param.doDraftRentalContractData.doContractCustomer);
                        param.doRegisterDraftRentalContractData.doRealCustomer =
                            CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(param.doDraftRentalContractData.doRealCustomer);
                        param.doRegisterDraftRentalContractData.doSite =
                            CommonUtil.CloneObject<doSite, doSite>(param.doDraftRentalContractData.doSite);
                        param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail =
                            CommonUtil.ClonsObjectList<tbt_DraftRentalEmail, tbt_DraftRentalEmail>(param.doDraftRentalContractData.doTbt_DraftRentalEmail);
                        param.doRegisterDraftRentalContractData.Mode = param.doDraftRentalContractData.Mode;
                        param.doRegisterDraftRentalContractData.LastUpdateDateQuotationData = param.doDraftRentalContractData.LastUpdateDateQuotationData;
                    }
                    else
                        param.FirstLoad = true;

                    #endregion

                    ViewBag.ScreenMode = param.ScreenMode;

                    if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.NEW)
                    {
                        ViewBag.HideChangeQuotationButton = true;
                    }
                    else if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE)
                    {
                        ViewBag.HideChangeQuotationButton = true;
                        ViewBag.QuotationTargetCode = param.QuotationTargetCode;
                    }
                }
            }
            catch (Exception)
            {
            }

            return View();
        }
        /// <summary>
        /// Generate FN-99 information section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_02()
        {
            ViewBag.IsApproveMode = false;
            ViewBag.DisabledOperationType = false;
            ViewBag.DisabledExpectedInstallCompleteDate = false;
            ViewBag.DisabledOrderInstallFee = false;
            ViewBag.DisabledOrderInstallFee_ApproveContract = false;
            ViewBag.DisabledOrderInstallFee_CompleteInstall = false;
            ViewBag.DisabledOrderInstallFee_StartService = false;
            ViewBag.RequiredOrderDepositFee = true;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE)
                        ViewBag.IsApproveMode = true;

                    if (param.doDraftRentalContractData != null)
                    {
                        ViewBag.ProductTypeCode = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ProductTypeCode;
                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.ProductTypeCode != ProductType.C_PROD_TYPE_AL
                            && param.doDraftRentalContractData.doTbt_DraftRentalContrat.ProductTypeCode != ProductType.C_PROD_TYPE_RENTAL_SALE)
                        {
                            if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.ProductTypeCode != ProductType.C_PROD_TYPE_ONLINE)
                            {
                                ViewBag.DisabledOperationType = true;
                            }

                            ViewBag.DisabledExpectedInstallCompleteDate = true;
                            ViewBag.DisabledOrderInstallFee = true;
                            ViewBag.DisabledOrderInstallFee_ApproveContract = true;
                            ViewBag.DisabledOrderInstallFee_CompleteInstall = true;
                            ViewBag.DisabledOrderInstallFee_StartService = true;
                            ViewBag.RequiredOrderDepositFee = false;
                        }

                        if (param.doDraftRentalContractData.doTbt_DraftRentalOperationType != null)
                        {
                            List<string> lstOpt = new List<string>();
                            foreach (tbt_DraftRentalOperationType opt in param.doDraftRentalContractData.doTbt_DraftRentalOperationType)
                            {
                                lstOpt.Add(opt.OperationTypeCode);
                            }
                            ViewBag.OperationTypeList = lstOpt.ToArray();
                        }


                        ViewBag.ExpectedInstallCompleteDate = CommonUtil.TextDate(param.doDraftRentalContractData.doTbt_DraftRentalContrat.ExpectedInstallCompleteDate);
                        ViewBag.ExpectedStartServiceDate = CommonUtil.TextDate(param.doDraftRentalContractData.doTbt_DraftRentalContrat.ExpectedStartServiceDate);
                        ViewBag.BillingTimingDepositFee = param.doDraftRentalContractData.doTbt_DraftRentalContrat.BillingTimingDepositFee;

                        CommonUtil cmm = new CommonUtil();
                        ViewBag.CounterBalanceOriginContractCode = 
                            cmm.ConvertContractCode(param.doDraftRentalContractData.doTbt_DraftRentalContrat.CounterBalanceOriginContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        ViewBag.ProjectCode = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ProjectCode;
                        ViewBag.ApproveNo1 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ApproveNo1;
                        ViewBag.ApproveNo2 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ApproveNo2;
                        ViewBag.ApproveNo3 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ApproveNo3;
                        ViewBag.ApproveNo4 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ApproveNo4;
                        ViewBag.ApproveNo5 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ApproveNo5;
                        ViewBag.BICContractCode = param.doDraftRentalContractData.doTbt_DraftRentalContrat.BICContractCode;
                        
                        #region Normal Contract Fee

                        ViewBag.NormalContractFeeCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType;

                        string txtNormalContractFee;
                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            txtNormalContractFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFeeUsd);
                        else
                            txtNormalContractFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFee);
                            
                        if (CommonUtil.IsNullOrEmpty(txtNormalContractFee))
                            txtNormalContractFee = "0.00";
                        ViewBag.NormalContractFee = txtNormalContractFee;

                        #endregion
                        #region Order Contract Fee

                        if (string.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFeeCurrencyType))
                            param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFeeCurrencyType =
                                param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType;

                        ViewBag.OrderContractFeeCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFeeCurrencyType;
                        
                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.OrderContractFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFeeUsd);
                        else
                            ViewBag.OrderContractFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee);
                        
                        #endregion
                        #region Normal Install Fee

                        ViewBag.NormalInstallFeeCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalInstallFeeCurrencyType;

                        string txtNormalInstallFee;
                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            txtNormalInstallFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalInstallFeeUsd);
                        else
                            txtNormalInstallFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalInstallFee);
                        
                        if (CommonUtil.IsNullOrEmpty(txtNormalInstallFee))
                            txtNormalInstallFee = "0.00";
                        ViewBag.NormalInstallFee = txtNormalInstallFee;

                        #endregion
                        #region Order Install Fee

                        #region Order Install Fee

                        if (string.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType))
                            param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType =
                                param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalInstallFeeCurrencyType;

                        ViewBag.OrderInstallFeeCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType;

                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.OrderInstallFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeUsd);
                        else
                            ViewBag.OrderInstallFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee);

                        #endregion
                        #region Order Install Fee / Approve Contract

                        if (string.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType))
                            param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType =
                                param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalInstallFeeCurrencyType;

                        ViewBag.OrderInstallFee_ApproveContractCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType;

                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.OrderInstallFee_ApproveContract = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractUsd);
                        else
                            ViewBag.OrderInstallFee_ApproveContract = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContract);

                        #endregion
                        #region Order Install Fee / Complete Install

                        if (string.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType))
                            param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType =
                                param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalInstallFeeCurrencyType;

                        ViewBag.OrderInstallFee_CompleteInstallCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType;

                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.OrderInstallFee_CompleteInstall = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallUsd);
                        else
                            ViewBag.OrderInstallFee_CompleteInstall = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstall);

                        #endregion
                        #region Order Install Fee / Start Servive

                        if (string.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType))
                            param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType =
                                param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalInstallFeeCurrencyType;

                        ViewBag.OrderInstallFee_StartServiceCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType;
                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.OrderInstallFee_StartService = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceUsd);
                        else
                            ViewBag.OrderInstallFee_StartService = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartService);

                        #endregion

                        #endregion
                        #region Normal Deposit Fee

                        ViewBag.NormalDepositFeeCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType;

                        string txtNormalDepositFee;
                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            txtNormalDepositFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalDepositFeeUsd);
                        else
                            txtNormalDepositFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalDepositFee);
                        
                        if (CommonUtil.IsNullOrEmpty(txtNormalDepositFee))
                            txtNormalDepositFee = "0.00";
                        ViewBag.NormalDepositFee = txtNormalDepositFee;

                        #endregion
                        #region Order Deposit Fee

                        if (string.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderDepositFeeCurrencyType))
                            param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderDepositFeeCurrencyType =
                                param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType;

                        ViewBag.OrderDepositFeeCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderDepositFeeCurrencyType;

                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.OrderDepositFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderDepositFeeUsd);
                        else
                            ViewBag.OrderDepositFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderDepositFee);
                        
                        #endregion
                        
                        if (param.doDraftRentalContractData.doTbt_RelationType != null)
                        {
                            foreach (tbt_RelationType rt in param.doDraftRentalContractData.doTbt_RelationType)
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

            return View("CTS010/_CTS010_02");
        }
        /// <summary>
        /// Generate intrument detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_03()
        {
            return View("CTS010/_CTS010_03");
        }
        /// <summary>
        /// Generate facility detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_03_F()
        {
            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                        ViewBag.FacilityMemo = param.doDraftRentalContractData.doTbt_DraftRentalContrat.FacilityMemo;
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_03_F");
        }
        /// <summary>
        /// Generate maintenance information in case of alarm, sale online section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_04()
        {
            ViewBag.chkMaintenanceCycle = false;

            try
            {
                /* --- Get Default Maintenance Cycle --- */
                /* ------------------------------------- */
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doSystemConfig> lst = chandler.GetSystemConfig(ConfigName.C_CONFIG_DEFAULT_MA_CYCLE);
                if (lst.Count > 0)
                {
                    ViewBag.DefaultMACycle = lst[0].ConfigValue;
                }
                ViewBag.MaintenanceCycle = ViewBag.DefaultMACycle;
                /* ------------------------------------- */

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.MaintenanceCycle) == false)
                        {
                            if (ViewBag.DefaultMACycle != param.doDraftRentalContractData.doTbt_DraftRentalContrat.MaintenanceCycle.Value.ToString())
                            {
                                ViewBag.chkMaintenanceCycle = true;
                                ViewBag.MaintenanceCycle = param.doDraftRentalContractData.doTbt_DraftRentalContrat.MaintenanceCycle;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_04");
        }
        /// <summary>
        /// Generate maintenance information in case of maintenance section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_05()
        {
            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat != null)
                        {
                            if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.MaintenanceCycle) == false)
                                ViewBag.MaintenanceCycle = param.doDraftRentalContractData.doTbt_DraftRentalContrat.MaintenanceCycle.ToString();
                        }
                        if (param.doDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails != null)
                        {
                            ViewBag.MaintenanceTargetProductTypeCodeName = param.doDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceTargetProductTypeCodeName;
                            ViewBag.MaintenanceTypeCodeName = param.doDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceTypeCodeName;
                            ViewBag.MaintenanceFeeTypeCode = param.doDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceFeeTypeCode;
                            ViewBag.MaintenanceMemo = param.doDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceMemo;

                            if (param.doDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceContractStartMonth != null)
                                ViewBag.MaintenanceContractStartMonth = param.doDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceContractStartMonth;
                            if (param.doDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceContractStartYear != null)
                                ViewBag.MaintenanceContractStartYear = param.doDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceContractStartYear;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_05");
        }
        /// <summary>
        /// Generate contract duration information section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_06()
        {
            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        ViewBag.IrregulationContractDurationFlag = param.doDraftRentalContractData.doTbt_DraftRentalContrat.IrregulationContractDurationFlag;
                        ViewBag.ContractDurationMonth = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.ContractDurationMonth, 0);
                        ViewBag.AutoRenewMonth = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.AutoRenewMonth, 0);
                        ViewBag.ContractEndDate = CommonUtil.TextDate(param.doDraftRentalContractData.doTbt_DraftRentalContrat.ContractEndDate);
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_06");
        }
        /// <summary>
        /// Generate contract payment term section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_07()
        {
            try
            {
                /* --- Get Default Billing Cycle --- */
                /* ------------------------------------- */
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doSystemConfig> lst = chandler.GetSystemConfig(ConfigName.C_CONFIG_DEFAULT_BILLING_CYCLE);
                if (lst.Count > 0)
                {
                    ViewBag.BillingCycle = lst[0].ConfigValue;
                }
                /* ------------------------------------- */

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.BillingCycle) == false)
                            ViewBag.BillingCycle = param.doDraftRentalContractData.doTbt_DraftRentalContrat.BillingCycle.Value.ToString();

                        ViewBag.PayMethod = param.doDraftRentalContractData.doTbt_DraftRentalContrat.PayMethod;

                        if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.CreditTerm) == false)
                            ViewBag.CreditTerm = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.CreditTerm, 0);
                        else
                            ViewBag.CreditTerm = "0";

                        ViewBag.CalDailyFeeStatus = param.doDraftRentalContractData.doTbt_DraftRentalContrat.CalDailyFeeStatus != null ?
                            param.doDraftRentalContractData.doTbt_DraftRentalContrat.CalDailyFeeStatus : "0";
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_07");
        }
        /// <summary>
        /// Generate in change information section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_08()
        {
            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        ViewBag.ContractOfficeCode = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ContractOfficeCode;
                        ViewBag.OperationOfficeCode = param.doDraftRentalContractData.doTbt_DraftRentalContrat.OperationOfficeCode;
                        ViewBag.SalesmanEmpNo1 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.SalesmanEmpNo1;
                        ViewBag.SalesmanEmpNameNo1 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.SalesmanEmpNameNo1;
                        ViewBag.SalesmanEmpNo2 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.SalesmanEmpNo2;
                        ViewBag.SalesmanEmpNameNo2 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.SalesmanEmpNameNo2;
                        ViewBag.SalesSupporterEmpNo = param.doDraftRentalContractData.doTbt_DraftRentalContrat.SalesSupporterEmpNo;
                        ViewBag.SalesSupporterEmpName = param.doDraftRentalContractData.doTbt_DraftRentalContrat.SalesSupporterEmpName;

                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_08");
        }
        /// <summary>
        /// Generate insurance information section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_09()
        {
            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        ViewBag.InsuranceTypeCodeName = param.doDraftRentalContractData.doTbt_DraftRentalContrat.InsuranceTypeCodeName;

                        #region Insurance Coverage Amount

                        ViewBag.InsuranceCoverageAmountCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.InsuranceCoverageAmountCurrencyType;
                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.InsuranceCoverageAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.InsuranceCoverageAmount = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.InsuranceCoverageAmountUsd);
                        else
                            ViewBag.InsuranceCoverageAmount = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.InsuranceCoverageAmount);
                        
                        #endregion
                        #region Monthly Insurance Fee

                        ViewBag.MonthlyInranceFeeCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.MonthlyInsuranceFeeCurrencyType;
                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.MonthlyInsuranceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.MonthlyInranceFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.MonthlyInsuranceFeeUsd);
                        else
                            ViewBag.MonthlyInranceFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.MonthlyInsuranceFee);
                        
                        #endregion
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_09");
        }
        /// <summary>
        /// Generate other fee information section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_10()
        {
            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        #region Out Sourcing Fee

                        ViewBag.OutSourcingFeeCurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.MaintenanceFee1CurrencyType;

                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.OutSourcingFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.MaintenanceFee1Usd);
                        else
                            ViewBag.OutSourcingFee = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.MaintenanceFee1);
                        
                        #endregion
                        #region Additional Fee 1

                        ViewBag.AdditionalFee1CurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee1CurrencyType;

                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.AdditionalFee1 = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee1Usd);
                        else
                            ViewBag.AdditionalFee1 = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee1);
                        
                        #endregion
                        #region Additional Fee 2

                        ViewBag.AdditionalFee2CurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee1CurrencyType;

                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.AdditionalFee2 = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee2Usd);
                        else
                            ViewBag.AdditionalFee2 = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee2);
                        
                        #endregion
                        #region Additional Fee 3

                        ViewBag.AdditionalFee3CurrencyType = param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee3CurrencyType;

                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            ViewBag.AdditionalFee3 = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee3Usd);
                        else
                            ViewBag.AdditionalFee3 = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalFee3);
                        
                        #endregion

                        ViewBag.AdditionalApproveNo1 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalApproveNo1;
                        ViewBag.AdditionalApproveNo2 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalApproveNo2;
                        ViewBag.AdditionalApproveNo3 = param.doDraftRentalContractData.doTbt_DraftRentalContrat.AdditionalApproveNo3;
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_10");
        }
        /// <summary>
        /// Generate recipient about approve contract result section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_11()
        {
            #region Get Email suffix

            string emailSuffix = "";

            ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doSystemConfig> emlst = chandler.GetSystemConfig(ConfigName.C_EMAIL_SUFFIX);
            if (emlst.Count > 0)
                emailSuffix = emlst[0].ConfigValue;

            #endregion

            ViewBag.EmailAddress = emailSuffix;

            return View("CTS010/_CTS010_11");
        }
        /// <summary>
        /// Generate contract target section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_12()
        {
            ViewBag.HasCustomerData = false;
            ViewBag.IsCheckBranch = false;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        if (param.doDraftRentalContractData.doContractCustomer != null)
                        {
                            ViewBag.HasCustomerData = true;

                            ViewBag.ContractTargetSignerTypeCode = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ContractTargetSignerTypeCode;

                            ViewBag.CustCode = param.doDraftRentalContractData.doContractCustomer.CustCodeShort;
                            ViewBag.CustStatusCodeName = param.doDraftRentalContractData.doContractCustomer.CustStatusCodeName;
                            ViewBag.CustTypeCodeName = param.doDraftRentalContractData.doContractCustomer.CustTypeCodeName;
                            ViewBag.CustFullNameEN = param.doDraftRentalContractData.doContractCustomer.CustFullNameEN;
                            ViewBag.CustFullNameLC = param.doDraftRentalContractData.doContractCustomer.CustFullNameLC;
                            ViewBag.AddressFullEN = param.doDraftRentalContractData.doContractCustomer.AddressFullEN;
                            ViewBag.AddressFullLC = param.doDraftRentalContractData.doContractCustomer.AddressFullLC;
                            ViewBag.Nationality = param.doDraftRentalContractData.doContractCustomer.Nationality;
                            ViewBag.PhoneNo = param.doDraftRentalContractData.doContractCustomer.PhoneNo;
                            ViewBag.BusinessTypeName = param.doDraftRentalContractData.doContractCustomer.BusinessTypeName;
                            ViewBag.IDNo = param.doDraftRentalContractData.doContractCustomer.IDNo;
                            ViewBag.URL = param.doDraftRentalContractData.doContractCustomer.URL;

                            if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.BranchNameEN) == false
                                || CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.BranchNameLC) == false
                                || CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.BranchAddressEN) == false
                                || CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.BranchAddressLC) == false)
                            {
                                ViewBag.IsCheckBranch = true;
                            }
                            ViewBag.BranchNameEN = param.doDraftRentalContractData.doTbt_DraftRentalContrat.BranchNameEN;
                            ViewBag.BranchNameLC = param.doDraftRentalContractData.doTbt_DraftRentalContrat.BranchNameLC;
                            ViewBag.BranchAddressEN = param.doDraftRentalContractData.doTbt_DraftRentalContrat.BranchAddressEN;
                            ViewBag.BranchAddressLC = param.doDraftRentalContractData.doTbt_DraftRentalContrat.BranchAddressLC;

                            ViewBag.ContractTargetMemo = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ContractTargetMemo;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_12");
        }
        /// <summary>
        /// Generate real customer section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_13()
        {
            ViewBag.HasRealCustomerData = false;
            ViewBag.SameCustomer = false;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        ViewBag.SameCustomer =
                        CTS010_IsSameCustomer(param.doDraftRentalContractData.doContractCustomer, param.doDraftRentalContractData.doRealCustomer);

                        if (param.doDraftRentalContractData.doRealCustomer != null)
                        {
                            ViewBag.HasRealCustomerData = true;
                            ViewBag.CustCode = param.doDraftRentalContractData.doRealCustomer.CustCodeShort;
                            ViewBag.CustStatusCodeName = param.doDraftRentalContractData.doRealCustomer.CustStatusCodeName;
                            ViewBag.CustTypeCodeName = param.doDraftRentalContractData.doRealCustomer.CustTypeCodeName;
                            ViewBag.CustFullNameEN = param.doDraftRentalContractData.doRealCustomer.CustFullNameEN;
                            ViewBag.CustFullNameLC = param.doDraftRentalContractData.doRealCustomer.CustFullNameLC;
                            ViewBag.AddressFullEN = param.doDraftRentalContractData.doRealCustomer.AddressFullEN;
                            ViewBag.AddressFullLC = param.doDraftRentalContractData.doRealCustomer.AddressFullLC;
                            ViewBag.Nationality = param.doDraftRentalContractData.doRealCustomer.Nationality;
                            ViewBag.PhoneNo = param.doDraftRentalContractData.doRealCustomer.PhoneNo;
                            ViewBag.BusinessTypeName = param.doDraftRentalContractData.doRealCustomer.BusinessTypeName;
                            ViewBag.IDNo = param.doDraftRentalContractData.doRealCustomer.IDNo;
                            ViewBag.URL = param.doDraftRentalContractData.doRealCustomer.URL;

                            ViewBag.RealCustomerMemo = param.doDraftRentalContractData.doTbt_DraftRentalContrat.RealCustomerMemo;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_13");
        }
        /// <summary>
        /// Generate site section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_14()
        {
            ViewBag.HasSiteData = false;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        if (param.doDraftRentalContractData.doSite != null)
                        {
                            ViewBag.HasSiteData = true;

                            if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doSite.SiteCodeShort) == false)
                                ViewBag.SiteCodeForSearch = param.doDraftRentalContractData.doSite.SiteCodeShort.Split("-".ToCharArray())[0];
                            else if (param.doDraftRentalContractData.doRealCustomer != null)
                            {
                                if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doRealCustomer.SiteCustCodeShort) == false)
                                    ViewBag.SiteCodeForSearch = param.doDraftRentalContractData.doRealCustomer.SiteCustCodeShort;
                            }

                            ViewBag.SiteNo = param.doDraftRentalContractData.doSite.SiteNo;
                            ViewBag.SiteCode = param.doDraftRentalContractData.doSite.SiteCodeShort;
                            ViewBag.SiteNameEN = param.doDraftRentalContractData.doSite.SiteNameEN;
                            ViewBag.SiteNameLC = param.doDraftRentalContractData.doSite.SiteNameLC;
                            ViewBag.AddressFullEN = param.doDraftRentalContractData.doSite.AddressFullEN;
                            ViewBag.AddressFullLC = param.doDraftRentalContractData.doSite.AddressFullLC;
                            ViewBag.PhoneNo = param.doDraftRentalContractData.doSite.PhoneNo;
                            ViewBag.BuildingUsageName = param.doDraftRentalContractData.doSite.BuildingUsageName;

                            if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doSite.SiteCode))
                            {
                                doSite s = CommonUtil.CloneObject<doSite, doSite>(param.doDraftRentalContractData.doSite);

                                ISiteMasterHandler shandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                                shandler.ValidateSiteData(s);
                                if (s != null)
                                {
                                    if (s.ValidateSiteData == false)
                                    {
                                        param.doDraftRentalContractData.doSite.ValidateSiteData = false;
                                        CTS010_ScreenData = param;
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

            return View("CTS010/_CTS010_14");
        }
        /// <summary>
        /// Generate contract point section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_15()
        {
            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        if (param.doDraftRentalContractData.doTbt_DraftRentalContrat != null)
                            ViewBag.ContactPoint = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ContactPoint;
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_15");
        }
        /// <summary>
        /// Generate billing target section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_16()
        {
            ViewBag.TotalContractFee = "0.00";
            ViewBag.TotalContractFeeUsd = "0.00";
            ViewBag.TotalInstallationFee = "0.00";
            ViewBag.TotalInstallationFeeUsd = "0.00";
            ViewBag.TotalDepositFee = "0.00";
            ViewBag.TotalDepositFeeUsd = "0.00";

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        ViewBag.DevideContractFeeBillingFlag = param.doDraftRentalContractData.doTbt_DraftRentalContrat.DivideContractFeeBillingFlag;

                        if (param.BillingTargetList != null)
                        {
                            decimal? totalContractFee = 0;
                            decimal? totalContractFeeUS = 0;
                            decimal? totalInstallationFee = 0;
                            decimal? totalInstallationFeeUS = 0;
                            decimal? totalDepositFee = 0;
                            decimal? totalDepositFeeUS = 0;

                            foreach (CTS010_BillingTargetData bill in param.BillingTargetList)
                            {
                                #region Contract Fee

                                if (bill.ContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    if (CommonUtil.IsNullOrEmpty(bill.ContractFeeUsd) == false)
                                        totalContractFeeUS += bill.ContractFeeUsd;                                    
                                }
                                else
                                {
                                    if (CommonUtil.IsNullOrEmpty(bill.ContractFee) == false)
                                        totalContractFee += bill.ContractFee;
                                }

                                #endregion
                                #region Install Fee for Approval

                                if (bill.InstallFee_ApprovalCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    if (CommonUtil.IsNullOrEmpty(bill.InstallFee_ApprovalUsd) == false)
                                        totalInstallationFeeUS += bill.InstallFee_ApprovalUsd; 
                                }
                                else
                                {
                                    if (CommonUtil.IsNullOrEmpty(bill.InstallFee_Approval) == false)
                                        totalInstallationFee += bill.InstallFee_Approval;
                                }

                                #endregion
                                #region Install Fee for Complete Installation

                                if (bill.InstallFee_CompleteInstallationCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    if (CommonUtil.IsNullOrEmpty(bill.InstallFee_CompleteInstallationUsd) == false)
                                        totalInstallationFeeUS += bill.InstallFee_CompleteInstallationUsd;
                                }
                                else
                                {
                                    if (CommonUtil.IsNullOrEmpty(bill.InstallFee_CompleteInstallation) == false)
                                        totalInstallationFee += bill.InstallFee_CompleteInstallation;
                                }

                                #endregion
                                #region Install Fee for Start Service

                                if (bill.InstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    if (CommonUtil.IsNullOrEmpty(bill.InstallFee_StartServiceUsd) == false)
                                        totalInstallationFeeUS += bill.InstallFee_StartServiceUsd;
                                }
                                else
                                {
                                    if (CommonUtil.IsNullOrEmpty(bill.InstallFee_StartService) == false)
                                        totalInstallationFee += bill.InstallFee_StartService;
                                }

                                #endregion
                                #region Deposit Fee

                                if (bill.DepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    if (CommonUtil.IsNullOrEmpty(bill.DepositFeeUsd) == false)
                                        totalDepositFeeUS += bill.DepositFeeUsd;
                                }
                                else
                                {
                                    if (CommonUtil.IsNullOrEmpty(bill.DepositFee) == false)
                                        totalDepositFee += bill.DepositFee;
                                }

                                #endregion
                            }

                            ViewBag.TotalContractFee = CommonUtil.TextNumeric(totalContractFee);
                            ViewBag.TotalContractFeeUsd = CommonUtil.TextNumeric(totalContractFeeUS);
                            ViewBag.TotalInstallationFee = CommonUtil.TextNumeric(totalInstallationFee);
                            ViewBag.TotalInstallationFeeUsd = CommonUtil.TextNumeric(totalInstallationFeeUS);
                            ViewBag.TotalDepositFee = CommonUtil.TextNumeric(totalDepositFee);
                            ViewBag.TotalDepositFeeUsd = CommonUtil.TextNumeric(totalDepositFeeUS);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return View("CTS010/_CTS010_16");
        }
        /// <summary>
        /// Generate result of register rental contract section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_18()
        {
            ViewBag.ShowButtonRegisterNext = false;
            ViewBag.ShowButtonEditNext = false;

            try
            {
                tbt_DraftRentalContract contract = null;

                #region Get Session

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                        contract = param.doDraftRentalContractData.doTbt_DraftRentalContrat;
                }
                if (contract == null)
                    contract = new tbt_DraftRentalContract();

                #endregion

                if (contract != null)
                {
                    CommonUtil cmm = new CommonUtil();
                    ViewBag.QuotationTargetCodeFull = contract.QuotationTargetCodeFull;

                    if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.NEW)
                        ViewBag.ShowButtonRegisterNext = true;
                    else
                        ViewBag.ShowButtonEditNext = true;
                }
            }
            catch
            {
            }

            return View("CTS010/_CTS010_18");
        }

        #endregion
        #region Actions

        /// <summary>
        /// Get data from session
        /// </summary>
        /// <param name="ObjectTypeID"></param>
        /// <returns></returns>
        public ActionResult CTS010_GetInitialData(int ObjectTypeID)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftRentalContractData != null)
                    {
                        switch (ObjectTypeID)
                        {
                            case 1:
                                if (param.doRegisterDraftRentalContractData.doContractCustomer != null)
                                    res.ResultData = param.doRegisterDraftRentalContractData.doContractCustomer;
                                else
                                    res.ResultData = new doCustomerWithGroup();

                                break;
                            case 2:
                                if (param.doRegisterDraftRentalContractData.doRealCustomer != null)
                                    res.ResultData = param.doRegisterDraftRentalContractData.doRealCustomer;
                                else
                                    res.ResultData = new doCustomerWithGroup();

                                break;
                            case 3:
                                if (param.doRegisterDraftRentalContractData.doSite != null)
                                    res.ResultData = param.doRegisterDraftRentalContractData.doSite;
                                else
                                    res.ResultData = new doSite();

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
        public ActionResult CTS010_SetInitialData(doDraftRentalContractData initData, int ObjectTypeID)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftRentalContractData != null)
                    {
                        bool isSameCustomer =
                            CTS010_IsSameCustomer(param.doRegisterDraftRentalContractData.doContractCustomer, param.doRegisterDraftRentalContractData.doRealCustomer);

                        switch (ObjectTypeID)
                        {
                            case 1:
                                if (initData != null)
                                {
                                    param.doRegisterDraftRentalContractData.doContractCustomer = initData.doContractCustomer;
                                }
                                else
                                {
                                    param.doRegisterDraftRentalContractData.doContractCustomer = null;
                                }

                                //if (isSameCustomer)
                                //{
                                //    param.doDraftRentalContractData.doRealCustomer =
                                //        CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(param.doDraftRentalContractData.doContractCustomer);

                                //    if (param.doDraftRentalContractData.doSite != null)
                                //    {
                                //        bool isChanged = true;
                                //        if (param.doDraftRentalContractData.doRealCustomer != null)
                                //        {
                                //            if (param.doDraftRentalContractData.doSite.SiteCode == param.doDraftRentalContractData.doRealCustomer.SiteCustCode)
                                //                isChanged = false;
                                //        }
                                //        if (isChanged && CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doSite.SiteCode) == false)
                                //            param.doDraftRentalContractData.doSite = null;
                                //    }
                                //}
                                //else
                                //{
                                //    //Check again
                                //    isSameCustomer = 
                                //        CTS010_IsSameCustomer(param.doDraftRentalContractData.doContractCustomer, param.doDraftRentalContractData.doRealCustomer);
                                //}

                                break;
                            case 2:
                                if (initData != null)
                                {
                                    param.doRegisterDraftRentalContractData.doRealCustomer = initData.doRealCustomer;

                                    if (param.doRegisterDraftRentalContractData.doSite != null)
                                    {
                                        bool isChanged = true;
                                        if (param.doRegisterDraftRentalContractData.doRealCustomer != null)
                                        {
                                            if (param.doRegisterDraftRentalContractData.doSite.SiteCode == param.doRegisterDraftRentalContractData.doRealCustomer.SiteCustCode)
                                                isChanged = false;
                                        }
                                        if (isChanged && CommonUtil.IsNullOrEmpty(param.doRegisterDraftRentalContractData.doSite.SiteCode) == false)
                                            param.doRegisterDraftRentalContractData.doSite = null;
                                    }
                                }
                                else
                                    param.doRegisterDraftRentalContractData.doRealCustomer = null;

                                isSameCustomer =
                                    CTS010_IsSameCustomer(param.doRegisterDraftRentalContractData.doContractCustomer, param.doRegisterDraftRentalContractData.doRealCustomer);

                                break;
                            case 3:
                                if (initData != null)
                                {
                                    CommonUtil cmm = new CommonUtil();

                                    DateTime? updateDate = null;
                                    if (param.doRegisterDraftRentalContractData.doSite != null)
                                        updateDate = param.doRegisterDraftRentalContractData.doSite.UpdateDate;
                                    param.doRegisterDraftRentalContractData.doSite = initData.doSite;
                                    param.doRegisterDraftRentalContractData.doSite.UpdateDate = updateDate;
                                }
                                else
                                    param.doRegisterDraftRentalContractData.doSite = null;

                                break;
                        }

                        CTS010_ScreenData = param;
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
        /// Retrieve quotation data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS010_RetrieveQuotationData(CTS010_RetrieveQuotationCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    return Json(res);

                if (CommonUtil.IsNullOrEmpty(cond.QuotationTargetCode) == false)
                    cond.QuotationTargetCode = cond.QuotationTargetCode.ToUpper();
                if (CommonUtil.IsNullOrEmpty(cond.Alphabet) == false)
                    cond.Alphabet = cond.Alphabet.ToUpper();

                IDraftRentalContractHandler rhandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;

                doDraftRentalContractData draftData = null;
                if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE)
                {
                    draftData = param.doDraftRentalContractData;

                    #region Set Billing Target List

                    param.BillingTargetList = CTS010_SetBillingList(draftData.doTbt_DraftRentalBillingTarget);

                    #endregion
                }
                else if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.NEW
                    || (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.EDIT && cond.IsChangeQuotationSite == true))
                {
                    #region Validate in case Search

                    if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.EDIT && cond.IsChangeQuotationSite == true)
                    {
                        if (param.doDraftRentalContractData != null)
                        {
                            if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.QuotationTargetCode != cond.QuotationTargetCodeLong)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                    MessageUtil.MessageList.MSG3095,
                                                    null,
                                                    new string[] { "QuotationTargetCode" });
                                res.ResultData = param.doDraftRentalContractData.doTbt_DraftRentalContrat.QuotationTargetCodeShort;
                                return Json(res);
                            }
                        }
                    }

                    #endregion
                    #region Validate

                    object[] obj = new object[1];
                    if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.NEW)
                        obj[0] = CommonUtil.CloneObject<CTS010_RetrieveQuotationCondition, CTS010_RetrieveQuotationCondition_New>(cond);
                    else
                        obj[0] = CommonUtil.CloneObject<CTS010_RetrieveQuotationCondition, CTS010_RetrieveQuotationCondition_EditChanged>(cond);

                    ValidatorUtil.BuildErrorMessage(res, obj);
                    if (res.IsError)
                        return Json(res);

                    #endregion
                    #region Check existing Draft rental contract data

                    if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.NEW)
                    {
                        List<tbt_DraftRentalContract> lst = rhandler.GetTbt_DraftRentalContract(cond.QuotationTargetCodeLong);
                        if (lst.Count > 0)
                        {
                            MessageUtil.MessageList msg = MessageUtil.MessageList.MSG3246;
                            if (lst[0].DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_WAITFORAPPROVE)
                                msg = MessageUtil.MessageList.MSG3243;
                            else if (lst[0].DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_REJECTED)
                                msg = MessageUtil.MessageList.MSG3244;
                            else if (lst[0].DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_RETURNED)
                                msg = MessageUtil.MessageList.MSG3245;

                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                msg,
                                                null,
                                                new string[] { "QuotationTargetCode" });
                            return Json(res);
                        }
                    }

                    #endregion
                    #region Retrieve Data

                    draftData = rhandler.GetEntireDraftRentalContract((doDraftRentalContractCondition)cond, doDraftRentalContractData.RENTAL_CONTRACT_MODE.QUOTATION);
                    if (draftData == null)
                    {
                        string[] ctrls = new string[] { "QuotationTargetCode", "Alphabet" };
                        if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.EDIT && cond.IsChangeQuotationSite == true)
                            ctrls = new string[] { "Alphabet" };

                        res.AddErrorMessage(
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0137,
                            null,
                            ctrls);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    }

                    #endregion

                    param.BillingTargetList = null;
                }
                else
                {
                    #region Validate

                    object[] obj = new object[]
                    {
                        CommonUtil.CloneObject<CTS010_RetrieveQuotationCondition, CTS010_RetrieveQuotationCondition_Edit>(cond)
                    };

                    ValidatorUtil.BuildErrorMessage(res, obj);
                    if (res.IsError)
                        return Json(res);

                    #endregion
                    #region Retrieve Data

                    draftData = rhandler.GetEntireDraftRentalContract((doDraftRentalContractCondition)cond, doDraftRentalContractData.RENTAL_CONTRACT_MODE.DRAFT);
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

                    #endregion
                    #region Set Billing Target List

                    param.BillingTargetList = CTS010_SetBillingList(draftData.doTbt_DraftRentalBillingTarget);

                    #endregion
                }
                if (draftData != null)
                {
                    if (draftData.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType == null)
                        draftData.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    if (draftData.doTbt_DraftRentalContrat.NormalInstallFeeCurrencyType == null)
                        draftData.doTbt_DraftRentalContrat.NormalInstallFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    if (draftData.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType == null)
                        draftData.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;

                    
                    res.ResultData = new CTS010_SpecifyQuotationData()
                    {
                        doTbt_DraftRentalContrat = draftData.doTbt_DraftRentalContrat,
                        doContractCustomer = draftData.doContractCustomer != null ? draftData.doContractCustomer : new doCustomerWithGroup(),
                        doRealCustomer = draftData.doRealCustomer != null ? draftData.doRealCustomer : new doCustomerWithGroup(),
                        doSite = draftData.doSite != null ? draftData.doSite : new doSite()
                    };
                }

                param.doDraftRentalContractData = draftData;

                param.doRegisterDraftRentalContractData = new doDraftRentalContractData();
                if (draftData != null)
                {
                    param.doRegisterDraftRentalContractData.doContractCustomer =
                        CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(draftData.doContractCustomer);
                    param.doRegisterDraftRentalContractData.doRealCustomer =
                        CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(draftData.doRealCustomer);
                    param.doRegisterDraftRentalContractData.doSite =
                        CommonUtil.CloneObject<doSite, doSite>(draftData.doSite);
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail =
                        CommonUtil.ClonsObjectList<tbt_DraftRentalEmail, tbt_DraftRentalEmail>(draftData.doTbt_DraftRentalEmail);
                    param.doRegisterDraftRentalContractData.Mode = draftData.Mode;
                    param.doRegisterDraftRentalContractData.LastUpdateDateQuotationData = draftData.LastUpdateDateQuotationData;
                }

                param.TempBillingTargetData = null;

                #region Check Same Customer mode

                if (param.doDraftRentalContractData != null)
                {
                    if (param.doDraftRentalContractData.doContractCustomer != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doContractCustomer.CustCode))
                        {
                            doCustomer cust1 = CommonUtil.CloneObject
                                <doCustomer, doCustomer>(param.doDraftRentalContractData.doContractCustomer);

                            ICustomerMasterHandler chandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                            chandler.ValidateCustomerData(cust1, true);
                            if (cust1 != null)
                            {
                                if (cust1.ValidateCustomerData == false)
                                    param.doDraftRentalContractData.doContractCustomer.ValidateCustomerData = false;
                            }
                        }
                    }
                    if (param.doDraftRentalContractData.doRealCustomer != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doRealCustomer.CustCode))
                        {
                            doCustomer cust1 = CommonUtil.CloneObject
                                <doCustomer, doCustomer>(param.doDraftRentalContractData.doRealCustomer);

                            ICustomerMasterHandler chandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                            chandler.ValidateCustomerData(cust1, true);
                            if (cust1 != null)
                            {
                                if (cust1.ValidateCustomerData == false)
                                    param.doDraftRentalContractData.doRealCustomer.ValidateCustomerData = false;
                            }
                        }
                    }
                }

                #endregion

                CTS010_ScreenData = param;
            }
            catch (ApplicationErrorException erx)
            {
                res.AddErrorMessage(erx);
                foreach (MessageModel msg in res.MessageList)
                {
                    if (msg.Code == MessageUtil.MessageList.MSG3068.ToString())
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
        /// <summary>
        /// Load instrument detail data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_GetInstrumentDetailData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doInstrumentDetail> lst = null;

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        if (param.doDraftRentalContractData.doTbt_DraftRentalInstrument != null)
                        {
                            lst = new List<doInstrumentDetail>();
                            foreach (tbt_DraftRentalInstrument inst in param.doDraftRentalContractData.doTbt_DraftRentalInstrument)
                            {
                                if (inst.InstrumentTypeCode != InstrumentType.C_INST_TYPE_GENERAL)
                                    continue;

                                lst.Add(CommonUtil.CloneObject<tbt_DraftRentalInstrument, doInstrumentDetail>(inst));
                            }
                        }
                    }

                }

                if (lst != null)
                {
                    lst = (
                        from x in lst
                        orderby x.ControllerFlag descending, x.InstrumentCode
                        select x).ToList();
                }

                res.ResultData = CommonUtil.ConvertToXml<doInstrumentDetail>(lst, "\\Contract\\CTS010_InstrumentDetail");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Load facility detail data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_GetFacilityDetailData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doInstrumentDetail> lst = null;

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                    {
                        if (param.doDraftRentalContractData.doTbt_DraftRentalInstrument != null)
                        {
                            lst = new List<doInstrumentDetail>();
                            foreach (tbt_DraftRentalInstrument inst in param.doDraftRentalContractData.doTbt_DraftRentalInstrument)
                            {
                                if (inst.InstrumentTypeCode != InstrumentType.C_INST_TYPE_MONITOR)
                                    continue;

                                lst.Add(CommonUtil.CloneObject<tbt_DraftRentalInstrument, doInstrumentDetail>(inst));
                            }
                        }
                    }

                }

                if (lst != null)
                {
                    lst = (
                        from x in lst
                        orderby x.ControllerFlag descending, x.InstrumentCode
                        select x).ToList();
                }

                res.ResultData = CommonUtil.ConvertToXml<doInstrumentDetail>(lst, "\\Contract\\CTS010_FacilityDetail");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        /// <summary>
        /// Load contract customer group data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_GetContractCustomerGroupData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<dtCustomeGroupData> lst = null;

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    //Modify by Jutarat A. on 03012014
                    //if (param.doDraftRentalContractData != null)
                    //{
                    //    if (param.doDraftRentalContractData.doContractCustomer != null)
                    //        lst = param.doDraftRentalContractData.doContractCustomer.CustomerGroupData;
                    //}
                    if (param.doRegisterDraftRentalContractData != null)
                    {
                        if (param.doRegisterDraftRentalContractData.doContractCustomer != null)
                            lst = param.doRegisterDraftRentalContractData.doContractCustomer.CustomerGroupData;
                    }
                    //End Modify
                }
                res.ResultData = CommonUtil.ConvertToXml<dtCustomeGroupData>(lst, "\\Contract\\CTS010_CustomerGroupData");
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
        public ActionResult CTS010_GetRealCustomerGroupData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<dtCustomeGroupData> lst = null;

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    //Modify by Jutarat A. on 03012014
                    //if (param.doDraftRentalContractData != null)
                    //{
                    //    if (param.doDraftRentalContractData.doRealCustomer != null)
                    //        lst = param.doDraftRentalContractData.doRealCustomer.CustomerGroupData;
                    //}
                    if (param.doRegisterDraftRentalContractData != null)
                    {
                        if (param.doRegisterDraftRentalContractData.doRealCustomer != null)
                            lst = param.doRegisterDraftRentalContractData.doRealCustomer.CustomerGroupData;
                    }
                    //End Modify
                }
                res.ResultData = CommonUtil.ConvertToXml<dtCustomeGroupData>(lst, "\\Contract\\CTS010_CustomerGroupData");
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
        public ActionResult CTS010_RetrieveCustomer(CTS010_RetrieveCustomerCondition cond)
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

                    CTS010_ScreenParameter param = CTS010_ScreenData;
                    if (param == null)
                        param = new CTS010_ScreenParameter();
                    if (param.doRegisterDraftRentalContractData == null)
                        param.doRegisterDraftRentalContractData = new doDraftRentalContractData();

                    if (cond.CustType == 1)
                        param.doRegisterDraftRentalContractData.doContractCustomer = custDo;
                    else
                        param.doRegisterDraftRentalContractData.doRealCustomer = custDo;

                    bool isSameCustomer =
                        CTS010_IsSameCustomer(param.doRegisterDraftRentalContractData.doContractCustomer, param.doRegisterDraftRentalContractData.doRealCustomer);

                    CTS010_ScreenData = param;

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
        /// Copy contract customer data to read customer data
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_CopyCustomer()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG;

            try
            {
                doCustomerWithGroup doCustomer = null;

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftRentalContractData != null)
                        doCustomer = param.doRegisterDraftRentalContractData.doContractCustomer;
                }
                if (doCustomer != null)
                {
                    bool isSameSite = true;

                    #region Update Data

                    param.doRegisterDraftRentalContractData.doRealCustomer = CommonUtil.CloneObject<doCustomerWithGroup, doCustomerWithGroup>(doCustomer);
                    if (param.doRegisterDraftRentalContractData.doSite != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doRegisterDraftRentalContractData.doSite.SiteCode) == false)
                        {
                            string rSiteCustCode = param.doRegisterDraftRentalContractData.doSite.SiteCode;
                            if (rSiteCustCode.IndexOf("-") > 0)
                                rSiteCustCode = rSiteCustCode.Substring(0, rSiteCustCode.IndexOf("-"));   

                            if (rSiteCustCode != param.doRegisterDraftRentalContractData.doRealCustomer.SiteCustCode)
                            {
                                param.doRegisterDraftRentalContractData.doSite = null;
                                isSameSite = false;
                            }
                        }
                    }
                    CTS010_ScreenData = param;

                    #endregion

                    List<object> objRes = new List<object>();
                    objRes.Add(param.doRegisterDraftRentalContractData.doRealCustomer);
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
        public ActionResult CTS010_RetrieveSiteData(CTS010_RetrieveSiteCondition cond)
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

                    CTS010_ScreenParameter param = CTS010_ScreenData;
                    if (param == null)
                        param = new CTS010_ScreenParameter();
                    if (param.doRegisterDraftRentalContractData == null)
                        param.doRegisterDraftRentalContractData = new doDraftRentalContractData();

                    param.doRegisterDraftRentalContractData.doSite = siteDo;
                    CTS010_ScreenData = param;

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
        public ActionResult CTS010_CheckRealCustomer(CTS010_RetrieveCustomerCondition cond)
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
        public ActionResult CTS010_CopySiteInfomation(CTS010_RetrieveCustomerCondition cond, tbt_DraftRentalContract initData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                doSite siteDo = null;
                doCustomer doCustomer = null;
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftRentalContractData != null)
                    {
                        if (cond.CustType == 1)
                            doCustomer = param.doRegisterDraftRentalContractData.doContractCustomer;
                        else if (cond.CustType == 2)
                            doCustomer = param.doRegisterDraftRentalContractData.doRealCustomer;
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
                    param.doRegisterDraftRentalContractData.doSite = siteDo;
                    param.doRegisterDraftRentalContractData.doSite.ValidateSiteData = false;
                    CTS010_ScreenData = param;
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
        //public ActionResult CTS010_GetEmployeeName(string empNo)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    try
        //    {
        //        if (empNo != null)
        //        {
        //            IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

        //            List<tbm_Employee> empLst = new List<tbm_Employee>();
        //            empLst.Add(new tbm_Employee()
        //            {
        //                EmpNo = empNo
        //            });
        //            List<tbm_Employee> lst = handler.GetEmployeeList(empLst);
        //            if (lst.Count > 0)
        //                res.ResultData = lst[0].EmpFullName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}
        /// <summary>
        /// Load E-mail data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_GetEmailList()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<tbt_DraftRentalEmail> lst = null;

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftRentalContractData != null)
                        lst = param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail;
                }
                res.ResultData = CommonUtil.ConvertToXml<tbt_DraftRentalEmail>(lst, "\\Contract\\CTS010_Email");
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
        public ActionResult CTS010_CheckBeforeAddEmailAddress(CTS010_AddEmailAddressCondition doEmail)
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
                        "CTS010",
                        MessageUtil.MODULE_COMMON,
                        MessageUtil.MessageList.MSG0007,
                        new string[] { "lblMailAddress" },
                        new string[] { "EmailAddress" });
                    return Json(res);
                }

                #region Get Data

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();
                if (param.doRegisterDraftRentalContractData == null)
                    param.doRegisterDraftRentalContractData = new doDraftRentalContractData();
                if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail == null)
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail = new List<tbt_DraftRentalEmail>();

                #endregion
                #region Get Email suffix

                string emailSuffix = "";

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doSystemConfig> emlst = chandler.GetSystemConfig(ConfigName.C_EMAIL_SUFFIX);
                if (emlst.Count > 0)
                    emailSuffix = emlst[0].ConfigValue;

                #endregion

                List<tbt_DraftRentalEmail> newLst = new List<tbt_DraftRentalEmail>();

                IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                foreach (tbt_DraftRentalEmail email in doEmail.NewEmailAddressList)
                {
                    //if (email.EmailAddress.IndexOf(emailSuffix) < 0)
                    if (String.IsNullOrEmpty(email.EmailAddress) == false
                        && email.EmailAddress.ToUpper().IndexOf(emailSuffix.ToUpper()) < 0) //Modify by Jutarat A. on 21112012
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
                        foreach (tbt_DraftRentalEmail oemail in param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail)
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
                        tbt_DraftRentalEmail e = new tbt_DraftRentalEmail()
                        {
                            EmailAddress = lst[0].EmailAddress,
                            ToEmpNo = lst[0].EmpNo,
                            SendFlag = "0"
                        };
                        param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail.Add(e);
                        newLst.Add(e);
                    }
                }

                #region Update Data

                CTS010_ScreenData = param;

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
        public ActionResult CTS010_AddEmailAddressFromPopup(CTS010_AddEmailAddressCondition doEmail)
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
                        "CTS010",
                        MessageUtil.MODULE_COMMON,
                        MessageUtil.MessageList.MSG0007,
                        new string[] { "lblMailAddress" },
                        new string[] { "EmailAddress" });
                    return Json(res);
                }

                #region Get Data

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();
                if (param.doRegisterDraftRentalContractData == null)
                    param.doRegisterDraftRentalContractData = new doDraftRentalContractData();
                if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail == null)
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail = new List<tbt_DraftRentalEmail>();

                #endregion
                #region Get Email suffix

                string emailSuffix = "";

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doSystemConfig> emlst = chandler.GetSystemConfig(ConfigName.C_EMAIL_SUFFIX);
                if (emlst.Count > 0)
                    emailSuffix = emlst[0].ConfigValue;

                #endregion

                List<tbt_DraftRentalEmail> newLst = new List<tbt_DraftRentalEmail>();

                IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail.Clear();
                foreach (tbt_DraftRentalEmail email in doEmail.NewEmailAddressList)
                {
                    //if (email.EmailAddress.IndexOf(emailSuffix) < 0)
                    if (String.IsNullOrEmpty(email.EmailAddress) == false
                        && email.EmailAddress.ToUpper().IndexOf(emailSuffix.ToUpper()) < 0) //Modify by Jutarat A. on 21112012
                    {
                        email.EmailAddress += emailSuffix;
                    }

                    List<dtGetEmailAddress> lst = handler.GetEmailAddress(null, email.EmailAddress, null, null);
                    if (lst != null && lst.Count > 0) //Add by Jutarat A. on 21112012
                    {
                        tbt_DraftRentalEmail e = new tbt_DraftRentalEmail()
                        {
                            EmailAddress = lst[0].EmailAddress,
                            ToEmpNo = lst[0].EmpNo,
                            SendFlag = "0",
                            EmpNo = lst[0].EmpNo,
                        };
                        param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail.Add(e);
                        newLst.Add(e);
                    }
                }

                #region Update Data

                CTS010_ScreenData = param;

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
        public ActionResult CTS010_RemoveEmailAddress(tbt_DraftRentalEmail doEmail)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftRentalContractData != null)
                    {
                        if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail != null)
                        {
                            foreach (tbt_DraftRentalEmail oemail in param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail)
                            {
                                if (doEmail.EmailAddress == oemail.EmailAddress)
                                {
                                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail.Remove(oemail);
                                    CTS010_ScreenData = param;
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
        /// Load maintenance detail data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_GetMaintenanceList()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<tbt_RelationType> lst = null;

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doDraftRentalContractData != null)
                        lst = param.doDraftRentalContractData.doTbt_RelationType;
                }
                res.ResultData = CommonUtil.ConvertToXml<tbt_RelationType>(lst, "\\Contract\\CTS010_Maintenance");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
        //public ActionResult CTS010_GetFeeInformation()
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    try
        //    {
        //        List<CTS010_FeeInformation> lst = new List<CTS010_FeeInformation>();

        //        // Line 1 (Contract fee)
        //        CTS010_FeeInformation l1 = new CTS010_FeeInformation();
        //        lst.Add(l1);
        //        // Line 2 (Installation fee)
        //        CTS010_FeeInformation l2 = new CTS010_FeeInformation();
        //        lst.Add(l2);
        //        // Line 3 (Deposit fee)
        //        CTS010_FeeInformation l3 = new CTS010_FeeInformation();
        //        lst.Add(l3);

        //        CTS010_ScreenParameter param = CTS010_ScreenData;
        //        if (param != null)
        //        {
        //            if (param.doDraftRentalContractData != null)
        //            {
        //                // Line 1 (Contract fee)
        //                l1.Normal = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFee);
        //                if (CommonUtil.IsNullOrEmpty(l1.Normal))
        //                    l1.Normal = "0.00";

        //                l1.Order = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee);
        //                if (CommonUtil.IsNullOrEmpty(l1.Order))
        //                    l1.Order = "0.00";

        //                // Line 2 (Installation fee)
        //                l2.Normal = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalInstallFee);
        //                if (CommonUtil.IsNullOrEmpty(l2.Normal))
        //                    l2.Normal = "0.00";

        //                l2.Order = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee);
        //                if (CommonUtil.IsNullOrEmpty(l2.Order))
        //                    l2.Order = "0.00";

        //                l2.Approve = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContract);
        //                l2.Complete = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstall);
        //                l2.Start = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartService);

        //                // Line 3 (Deposit fee)
        //                l3.Normal = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.NormalDepositFee);
        //                if (CommonUtil.IsNullOrEmpty(l3.Normal))
        //                    l3.Normal = "0.00";

        //                l3.Order = CommonUtil.TextNumeric(param.doDraftRentalContractData.doTbt_DraftRentalContrat.OrderDepositFee);
        //                if (CommonUtil.IsNullOrEmpty(l3.Order))
        //                    l3.Order = "0.00";
        //            }
        //        }

        //        res.ResultData = CommonUtil.ConvertToXml<CTS010_FeeInformation>(lst, "\\Contract\\CTS010_FeeInformation");
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }
        //    return Json(res);
        //}
        /// <summary>
        /// Load billing target detail data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_GetBillingTargetList()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<CTS010_BillingTargetData> lst = null;

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.BillingTargetList != null)
                        lst = param.BillingTargetList;
                }

                res.ResultData = CommonUtil.ConvertToXml<CTS010_BillingTargetData>(lst, "\\Contract\\CTS010_BillingTarget", CommonUtil.GRID_EMPTY_TYPE.INSERT);
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
        public ActionResult CTS010_RetrieveBillingTargetDetailData(CTS010_RetrieveBillingTargetCondition cond)
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

                CTS010_BillingTargetData billingTargetData = null;

#if !ROUND2

                billingTargetData = new CTS010_BillingTargetData();
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
                billingTargetData.DocLanguage = billingTData[0].DocLanguage; //Add by Jutarat A. on 18122013
#endif

                if (billingTargetData != null)
                {
                    IBillingMasterHandler handler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
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

                    CTS010_ScreenParameter param = CTS010_ScreenData;
                    if (param == null)
                        param = new CTS010_ScreenParameter();
                    param.TempBillingTargetData = billingTargetData;
                    CTS010_ScreenData = param;

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
        public ActionResult CTS010_RetrieveBillingClientDetailData(CTS010_RetrieveBillingClientCondition cond)
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

                CTS010_BillingTargetData billingTargetData = new CTS010_BillingTargetData();
                billingTargetData.BillingClientCode = cmm.ConvertBillingClientCode(cond.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                
                if (billingTargetData != null)
                {
                    IBillingMasterHandler handler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
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

                    CTS010_ScreenParameter param = CTS010_ScreenData;
                    if (param == null)
                        param = new CTS010_ScreenParameter();
                    param.TempBillingTargetData = billingTargetData;
                    CTS010_ScreenData = param;

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
        /// Get selected billing target data from session
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public ActionResult CTS010_GetSelectedBillingTargetDetailData(int rowIndex)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();

                CTS010_BillingTargetData bData = null;
                if (param.BillingTargetList != null)
                {
                    int idx = 0;
                    foreach (CTS010_BillingTargetData bt in param.BillingTargetList)
                    {
                        //if (bt.IsBillingTarget(key))
                        if (idx == rowIndex)
                        {
                            bData = bt;

                            #region Update Temp

                            param.TempBillingTargetData = bt;
                            CTS010_ScreenData = param;

                            #endregion

                            break;
                        }

                        idx++;
                    }
                }

                res.ResultData = bData;
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
        public ActionResult CTS010_GetTempBillingClientData()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();

                CTS010_BillingTargetData temp = param.TempBillingTargetData;
                if (temp == null)
                {
                    temp = new CTS010_BillingTargetData();
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
        public ActionResult CTS010_UpdateTempBillingClientData(tbm_BillingClient temp)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Update Data

                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();
                if (param.TempBillingTargetData == null)
                    param.TempBillingTargetData = new CTS010_BillingTargetData();

                CommonUtil cmm = new CommonUtil();
                temp.BillingClientCode = cmm.ConvertBillingClientCode(temp.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                if (CTS010_IsSameBillingClient(param.TempBillingTargetData.BillingClient, temp) == false)
                {
                    param.TempBillingTargetData.BillingClient = temp;
                    param.TempBillingTargetData.BillingClientCode = null;
                    param.TempBillingTargetData.BillingOfficeCode = null;
                    param.TempBillingTargetData.BillingTargetCode = null;

                    CTS010_ScreenData = param;
                }

                #endregion

                res.ResultData = param.TempBillingTargetData;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Remove selected billing target data from session
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public ActionResult CTS010_RemoveSelectedBillingTargetDetailData(int rowIndex)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();

                decimal? totalContractFee = 0;
                decimal? totalContractFeeUS = 0;
                decimal? totalInstallationFee = 0;
                decimal? totalInstallationFeeUS = 0;
                decimal? totalDepositFee = 0;
                decimal? totalDepositFeeUS = 0;
                if (param.BillingTargetList != null)
                {
                    int idx = 0;
                    CTS010_BillingTargetData bData = null;
                    foreach (CTS010_BillingTargetData bt in param.BillingTargetList)
                    {
                        //if (bt.IsBillingTarget(key))
                        if (idx == rowIndex)
                        {
                            bData = bt;
                            break;
                        }

                        idx++;
                    }
                    if (bData != null)
                    {
                        param.BillingTargetList.Remove(bData);
                        CTS010_ScreenData = param;
                    }

                    foreach (CTS010_BillingTargetData bill in param.BillingTargetList)
                    {
                        #region Contract Fee

                        if (CommonUtil.IsNullOrEmpty(bill.ContractFee) == false)
                            totalContractFee += bill.ContractFee;
                        if (CommonUtil.IsNullOrEmpty(bill.ContractFeeUsd) == false)
                            totalContractFeeUS += bill.ContractFeeUsd;

                        #endregion
                        #region Install Fee Approval

                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_Approval) == false)
                            totalInstallationFee += bill.InstallFee_Approval;
                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_ApprovalUsd) == false)
                            totalInstallationFeeUS += bill.InstallFee_ApprovalUsd;

                        #endregion
                        #region Install Fee Complete Installation

                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_CompleteInstallation) == false)
                            totalInstallationFee += bill.InstallFee_CompleteInstallation;
                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_CompleteInstallationUsd) == false)
                            totalInstallationFeeUS += bill.InstallFee_CompleteInstallationUsd;

                        #endregion
                        #region Install Fee Start Service

                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_StartService) == false)
                            totalInstallationFee += bill.InstallFee_StartService;
                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_StartServiceUsd) == false)
                            totalInstallationFeeUS += bill.InstallFee_StartServiceUsd;

                        #endregion
                        #region Deposit Fee

                        if (CommonUtil.IsNullOrEmpty(bill.DepositFee) == false)
                            totalDepositFee += bill.DepositFee;
                        if (CommonUtil.IsNullOrEmpty(bill.DepositFeeUsd) == false)
                            totalDepositFeeUS += bill.DepositFeeUsd;

                        #endregion
                    }
                }

                CTS010_UpdateBillingResult result = new CTS010_UpdateBillingResult();
                result.TotalContractFee = CommonUtil.TextNumeric(totalContractFee);
                result.TotalContractFeeUsd = CommonUtil.TextNumeric(totalContractFeeUS);
                result.TotalInstallationFee = CommonUtil.TextNumeric(totalInstallationFee);
                result.TotalInstallationFeeUsd = CommonUtil.TextNumeric(totalInstallationFeeUS);
                result.TotalDepositFee = CommonUtil.TextNumeric(totalDepositFee);
                result.TotalDepositFeeUsd = CommonUtil.TextNumeric(totalDepositFeeUS);

                res.ResultData = result;
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
        public ActionResult CTS010_ClearTempBillingTargetDetailData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                    param.TempBillingTargetData = null;
                CTS010_ScreenData = param;
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
        public ActionResult CTS010_CopyBillingNameAddressData(CTS010_CopyBillingNameAddressCondition cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS010_BillingTargetData billingTargetData = null;

                doDraftRentalContractData draftData = null;
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param != null)
                {
                    if (param.doRegisterDraftRentalContractData != null)
                        draftData = param.doRegisterDraftRentalContractData;
                }
                if (draftData != null)
                {
                    string strIDNo = null; //Add by Jutarat A. on 17122013
                    if (cond.CopyMode == CTS010_CopyBillingNameAddressCondition.COPY_BILLING_NAME_ADDRESS_MODE.CONTRACT_TARGET)
                    {
                        if (param.doRegisterDraftRentalContractData.doContractCustomer != null)
                        {
                            //Add by Jutarat A. on 17122013
                            if (param.doRegisterDraftRentalContractData.doContractCustomer.DummyIDFlag != true)
                                strIDNo = param.doRegisterDraftRentalContractData.doContractCustomer.IDNo;
                            //End Add

                            billingTargetData = new CTS010_BillingTargetData();
                            billingTargetData.BillingClient = new tbm_BillingClient()
                            {
                                NameEN = param.doRegisterDraftRentalContractData.doContractCustomer.CustNameEN,
                                NameLC = param.doRegisterDraftRentalContractData.doContractCustomer.CustNameLC,
                                FullNameEN = param.doRegisterDraftRentalContractData.doContractCustomer.CustFullNameEN,
                                FullNameLC = param.doRegisterDraftRentalContractData.doContractCustomer.CustFullNameLC,
                                AddressEN = param.doRegisterDraftRentalContractData.doContractCustomer.AddressFullEN,
                                AddressLC = param.doRegisterDraftRentalContractData.doContractCustomer.AddressFullLC,
                                RegionCode = param.doRegisterDraftRentalContractData.doContractCustomer.RegionCode,
                                Nationality = param.doRegisterDraftRentalContractData.doContractCustomer.Nationality,
                                PhoneNo = param.doRegisterDraftRentalContractData.doContractCustomer.PhoneNo,
                                BusinessTypeCode = param.doRegisterDraftRentalContractData.doContractCustomer.BusinessTypeCode,
                                BusinessTypeName = param.doRegisterDraftRentalContractData.doContractCustomer.BusinessTypeName,
                                IDNo = strIDNo, //param.doRegisterDraftRentalContractData.doContractCustomer.IDNo, //Modify by Jutarat A. on 17122013
                                CustTypeCode = param.doRegisterDraftRentalContractData.doContractCustomer.CustTypeCode,
                                CompanyTypeCode = param.doRegisterDraftRentalContractData.doContractCustomer.CompanyTypeCode
                            };
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0073);
                            return Json(res);
                        }
                    }
                    else if (cond.CopyMode == CTS010_CopyBillingNameAddressCondition.COPY_BILLING_NAME_ADDRESS_MODE.BRANCH_CONTRACT_TARGET)
                    {
                        bool hasBranch = false;
                        if (CommonUtil.IsNullOrEmpty(cond.BranchNameEN) == false
                            && CommonUtil.IsNullOrEmpty(cond.BranchNameLC) == false
                            && CommonUtil.IsNullOrEmpty(cond.BranchAddressEN) == false
                            && CommonUtil.IsNullOrEmpty(cond.BranchAddressLC) == false)
                        {
                            hasBranch = true;
                        }

                        if (hasBranch && param.doRegisterDraftRentalContractData.doContractCustomer != null)
                        {
                            //Add by Jutarat A. on 17122013
                            if (param.doRegisterDraftRentalContractData.doContractCustomer.DummyIDFlag != true)
                                strIDNo = param.doRegisterDraftRentalContractData.doContractCustomer.IDNo;
                            //End Add

                            billingTargetData = new CTS010_BillingTargetData();
                            billingTargetData.BillingClient = new tbm_BillingClient()
                            {
                                NameEN = param.doRegisterDraftRentalContractData.doContractCustomer.CustNameEN,
                                NameLC = param.doRegisterDraftRentalContractData.doContractCustomer.CustNameLC,
                                FullNameEN = param.doRegisterDraftRentalContractData.doContractCustomer.CustFullNameEN,
                                FullNameLC = param.doRegisterDraftRentalContractData.doContractCustomer.CustFullNameLC,
                                //BranchNameEN = cond.BranchNameEN, //Comment by Jutarat A. on 17122013
                                //BranchNameLC = cond.BranchNameLC, //Comment by Jutarat A. on 17122013
                                AddressEN = cond.BranchAddressEN,
                                AddressLC = cond.BranchAddressLC,
                                RegionCode = param.doRegisterDraftRentalContractData.doContractCustomer.RegionCode,
                                Nationality = param.doRegisterDraftRentalContractData.doContractCustomer.Nationality,
                                BusinessTypeCode = param.doRegisterDraftRentalContractData.doContractCustomer.BusinessTypeCode,
                                BusinessTypeName = param.doRegisterDraftRentalContractData.doContractCustomer.BusinessTypeName,
                                IDNo = strIDNo, //param.doRegisterDraftRentalContractData.doContractCustomer.IDNo, //Modify by Jutarat A. on 17122013
                                CustTypeCode = param.doRegisterDraftRentalContractData.doContractCustomer.CustTypeCode,
                                CompanyTypeCode = param.doRegisterDraftRentalContractData.doContractCustomer.CompanyTypeCode
                            };
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0129);
                            return Json(res);
                        }
                    }
                    else if (cond.CopyMode == CTS010_CopyBillingNameAddressCondition.COPY_BILLING_NAME_ADDRESS_MODE.REAL_CUSTOMERE)
                    {
                        if (param.doRegisterDraftRentalContractData.doRealCustomer != null)
                        {
                            //Add by Jutarat A. on 17122013
                            if (param.doRegisterDraftRentalContractData.doRealCustomer.DummyIDFlag != true)
                                strIDNo = param.doRegisterDraftRentalContractData.doRealCustomer.IDNo;
                            //End Add

                            billingTargetData = new CTS010_BillingTargetData();
                            billingTargetData.BillingClient = new tbm_BillingClient()
                            {
                                NameEN = param.doRegisterDraftRentalContractData.doRealCustomer.CustNameEN,
                                NameLC = param.doRegisterDraftRentalContractData.doRealCustomer.CustNameLC,
                                FullNameEN = param.doRegisterDraftRentalContractData.doRealCustomer.CustFullNameEN,
                                FullNameLC = param.doRegisterDraftRentalContractData.doRealCustomer.CustFullNameLC,
                                AddressEN = param.doRegisterDraftRentalContractData.doRealCustomer.AddressFullEN,
                                AddressLC = param.doRegisterDraftRentalContractData.doRealCustomer.AddressFullLC,
                                RegionCode = param.doRegisterDraftRentalContractData.doRealCustomer.RegionCode,
                                Nationality = param.doRegisterDraftRentalContractData.doRealCustomer.Nationality,
                                PhoneNo = param.doRegisterDraftRentalContractData.doRealCustomer.PhoneNo,
                                BusinessTypeCode = param.doRegisterDraftRentalContractData.doRealCustomer.BusinessTypeCode,
                                BusinessTypeName = param.doRegisterDraftRentalContractData.doRealCustomer.BusinessTypeName,
                                IDNo = strIDNo, //param.doRegisterDraftRentalContractData.doRealCustomer.IDNo, //Modify by Jutarat A. on 17122013
                                CustTypeCode = param.doRegisterDraftRentalContractData.doRealCustomer.CustTypeCode,
                                CompanyTypeCode = param.doRegisterDraftRentalContractData.doRealCustomer.CompanyTypeCode
                            };
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0074);
                            return Json(res);
                        }
                    }
                    else if (cond.CopyMode == CTS010_CopyBillingNameAddressCondition.COPY_BILLING_NAME_ADDRESS_MODE.SITE)
                    {
                        if (param.doRegisterDraftRentalContractData.doRealCustomer != null
                            && param.doRegisterDraftRentalContractData.doSite != null)
                        {
                            //Add by Jutarat A. on 17122013
                            if (param.doRegisterDraftRentalContractData.doRealCustomer.DummyIDFlag != true)
                                strIDNo = param.doRegisterDraftRentalContractData.doRealCustomer.IDNo;
                            //End Add

                            billingTargetData = new CTS010_BillingTargetData();
                            billingTargetData.BillingClient = new tbm_BillingClient()
                            {
                                NameEN = param.doRegisterDraftRentalContractData.doRealCustomer.CustNameEN,
                                NameLC = param.doRegisterDraftRentalContractData.doRealCustomer.CustNameLC,
                                FullNameEN = param.doRegisterDraftRentalContractData.doRealCustomer.CustFullNameEN,
                                FullNameLC = param.doRegisterDraftRentalContractData.doRealCustomer.CustFullNameLC,
                                //BranchNameEN = param.doRegisterDraftRentalContractData.doSite.SiteNameEN, //Comment by Jutarat A. on 17122013
                                //BranchNameLC = param.doRegisterDraftRentalContractData.doSite.SiteNameLC, //Comment by Jutarat A. on 17122013
                                AddressEN = param.doRegisterDraftRentalContractData.doSite.AddressFullEN,
                                AddressLC = param.doRegisterDraftRentalContractData.doSite.AddressFullLC,
                                RegionCode = param.doRegisterDraftRentalContractData.doRealCustomer.RegionCode,
                                Nationality = param.doRegisterDraftRentalContractData.doRealCustomer.Nationality,
                                PhoneNo = param.doRegisterDraftRentalContractData.doSite.PhoneNo,
                                BusinessTypeCode = param.doRegisterDraftRentalContractData.doRealCustomer.BusinessTypeCode,
                                BusinessTypeName = param.doRegisterDraftRentalContractData.doRealCustomer.BusinessTypeName,
                                IDNo = strIDNo, //param.doRegisterDraftRentalContractData.doRealCustomer.IDNo, //Modify by Jutarat A. on 17122013
                                CustTypeCode = param.doRegisterDraftRentalContractData.doRealCustomer.CustTypeCode,
                                CompanyTypeCode = param.doRegisterDraftRentalContractData.doRealCustomer.CompanyTypeCode
                            };
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0130);
                            return Json(res);
                        }
                    }

                    #region Update Temp

                    param.TempBillingTargetData = billingTargetData;
                    CTS010_ScreenData = param;

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
        /// Set billing target data to session
        /// </summary>
        /// <param name="billingData"></param>
        /// <returns></returns>
        public ActionResult CTS010_UpdateBillingTargetDetail(CTS010_UpdateBillingTargetData billingData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();
                if (param.BillingTargetList == null)
                    param.BillingTargetList = new List<CTS010_BillingTargetData>();

                CTS010_BillingTargetData temp = null;
                if (param.TempBillingTargetData != null)
                    temp = param.TempBillingTargetData;
                else
                    temp = new CTS010_BillingTargetData();

                if (temp.BillingClient == null)
                {
                    int idx = 0;
                    foreach (CTS010_BillingTargetData bt in param.BillingTargetList)
                    {
                        //if (bt.IsBillingTarget(billingData.BillingClientCode, billingData.BillingOfficeCode))
                        if (idx == billingData.RowIndex)
                        {
                            temp.BillingClient = bt.BillingClient;
                            break;
                        }

                        idx++;
                    }
                }

                if (temp.BillingClient == null)
                    temp.BillingClient = new tbm_BillingClient();

                #region Validate require fields

                CTS010_UpdateBillingClient valOBj = CommonUtil.CloneObject<tbm_BillingClient, CTS010_UpdateBillingClient>(temp.BillingClient);

                ValidatorUtil validatorF = new ValidatorUtil(this);
                if (CommonUtil.IsNullOrEmpty(valOBj.BillingClientCode))
                {
                    if (CommonUtil.IsNullOrEmpty(valOBj.CustTypeCode)
                        && CommonUtil.IsNullOrEmpty(valOBj.NameEN)
                        && CommonUtil.IsNullOrEmpty(valOBj.NameLC)
                        && CommonUtil.IsNullOrEmpty(valOBj.AddressEN)
                        && CommonUtil.IsNullOrEmpty(valOBj.AddressLC))
                    {
                        validatorF.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS010",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "BillingClientCodeV",
                            "lblBillingClientCode",
                            null,
                            "1");
                    }
                }

                //Add by Jutarat A. on 25122013
                if (valOBj.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC
                    || valOBj.CustTypeCode == CustomerType.C_CUST_TYPE_ASSOCIATION
                    || valOBj.CustTypeCode == CustomerType.C_CUST_TYPE_PUBLIC_OFFICE)
                {
                    if (CommonUtil.IsNullOrEmpty(valOBj.IDNo))
                        validatorF.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                  "CTS010",
                                                  MessageUtil.MODULE_COMMON,
                                                  MessageUtil.MessageList.MSG0007,
                                                  "BillingIDNo",
                                                  "lblIDNo",
                                                  "BillingIDNo");

                    //if (CommonUtil.IsNullOrEmpty(valOBj.BranchNameEN))
                    //    validatorF.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                    //                              "CTS010",
                    //                              MessageUtil.MODULE_COMMON,
                    //                              MessageUtil.MessageList.MSG0007,
                    //                              "BillingBranchNameEN",
                    //                              "lblBranchNameEN",
                    //                              "BillingBranchNameEN");

                    //if (CommonUtil.IsNullOrEmpty(valOBj.BranchNameLC))
                    //    validatorF.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                    //                              "CTS010",
                    //                              MessageUtil.MODULE_COMMON,
                    //                              MessageUtil.MessageList.MSG0007,
                    //                              "BillingBranchNameLC",
                    //                              "lblBranchNameLC",
                    //                              "BillingBranchNameLC");
                }
                //End Add

                ValidatorUtil.BuildErrorMessage(res, validatorF, new object[] { valOBj });
                if (res.IsError)
                    return Json(res);

                #endregion
                #region Validate business

                ValidatorUtil validator = new ValidatorUtil();

                bool isDuplicate = false;
                string billingClientCode = billingData.BillingClientCode;
                if (billingData.UpdateMode == CTS010_UpdateBillingTargetData.UPDATE_MODE.NEW)
                    billingClientCode = valOBj.BillingClientCode;

                for(int idx = 0; idx < param.BillingTargetList.Count; idx++)
                {
                    if (billingData.RowIndex >= 0
                        && idx == billingData.RowIndex)
                        continue;

                    CTS010_BillingTargetData bt = param.BillingTargetList[idx];
                    if (CommonUtil.IsNullOrEmpty(billingClientCode) == false)
                    {
                        if (bt.IsBillingTarget(billingClientCode, billingData.BillingOfficeCode))
                        {
                            isDuplicate = true;
                            break;
                        }
                    }
                }

                if (isDuplicate)
                    validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3032, "BillingList");

                bool isError = true;
                if (billingData.ContractFee > 0 || billingData.ContractFeeUsd > 0
                    || billingData.InstallFee_Approval > 0 || billingData.InstallFee_ApprovalUsd > 0
                    || billingData.InstallFee_CompleteInstallation > 0 || billingData.InstallFee_CompleteInstallationUsd > 0
                    || billingData.InstallFee_StartService > 0 || billingData.InstallFee_StartServiceUsd > 0
                    || billingData.DepositFee > 0 || billingData.DepositFeeUsd > 0)
                {
                    isError = false;
                }
                if (isError)
                    validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3087, "Fee", "", "ALLFee");

                ValidatorUtil.BuildErrorMessage(res, validator,
                    new object[] 
                    { 
                        CommonUtil.CloneObject<CTS010_UpdateBillingTargetData, CTS010_UpdateBillingTargetData2>(billingData)
                    }, null, false);
                if (res.IsError)
                    return Json(res);

                //Add by Jutarat A. on 02012014
                if (valOBj.CustTypeCode == CustomerType.C_CUST_TYPE_JURISTIC
                    || valOBj.CustTypeCode == CustomerType.C_CUST_TYPE_ASSOCIATION
                    || valOBj.CustTypeCode == CustomerType.C_CUST_TYPE_PUBLIC_OFFICE)
                {
                    if (CommonUtil.IsNullOrEmpty(valOBj.IDNo) == false && valOBj.IDNo.Length != 15)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER,
                                        "CTS010",
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
                #region Add/Update Data

                CTS010_UpdateBillingResult result = new CTS010_UpdateBillingResult();
                CTS010_BillingTargetData bData = null;
                if (billingData.UpdateMode == CTS010_UpdateBillingTargetData.UPDATE_MODE.NEW)
                {
                    bData = CommonUtil.CloneObject<CTS010_BillingTargetData, CTS010_BillingTargetData>(temp);

                    ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    bData.Currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                    param.BillingTargetList.Add(bData);
                }
                else
                {
                    int idx = 0;
                    foreach (CTS010_BillingTargetData bt in param.BillingTargetList)
                    {
                        //if (bt.IsBillingTarget(billingData.BillingClientCode, billingData.BillingOfficeCode))
                        if (idx == billingData.RowIndex)
                        {
                            bData = bt;
                            break;
                        }

                        idx++;
                    }
                }
                if (bData != null)
                {
                    bData.BillingOfficeCode = billingData.BillingOfficeCode;
                    bData.DocLanguage = billingData.DocLanguage; //Add by Jutarat A. on 18122013

                    IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                    List<doFunctionBilling> clst = handler.GetFunctionBilling();
                    if (clst.Count > 0)
                    {
                        foreach (doFunctionBilling fb in clst)
                        {
                            if (fb.OfficeCode == bData.BillingOfficeCode)
                            {
                                bData.BillingOfficeName = fb.OfficeName;
                                break;
                            }
                        }
                    }

                    #region Contract Fee

                    bData.ContractFeeCurrencyType = billingData.ContractFeeCurrencyType;
                    bData.ContractFee = billingData.ContractFee;
                    bData.ContractFeeUsd = billingData.ContractFeeUsd;

                    #endregion
                    #region Install Fee Approval

                    bData.InstallFee_ApprovalCurrencyType = billingData.InstallFee_ApprovalCurrencyType;
                    bData.InstallFee_Approval = billingData.InstallFee_Approval;
                    bData.InstallFee_ApprovalUsd = billingData.InstallFee_ApprovalUsd;

                    #endregion
                    #region Install Fee Complete Installation

                    bData.InstallFee_CompleteInstallationCurrencyType = billingData.InstallFee_CompleteInstallationCurrencyType;
                    bData.InstallFee_CompleteInstallation = billingData.InstallFee_CompleteInstallation;
                    bData.InstallFee_CompleteInstallationUsd = billingData.InstallFee_CompleteInstallationUsd;

                    #endregion
                    #region Install Fee Start Service

                    bData.InstallFee_StartServiceCurrencyType = billingData.InstallFee_StartServiceCurrencyType;
                    bData.InstallFee_StartService = billingData.InstallFee_StartService;
                    bData.InstallFee_StartServiceUsd = billingData.InstallFee_StartServiceUsd;

                    #endregion
                    #region Deposit Fee

                    bData.DepositFeeCurrencyType = billingData.DepositFeeCurrencyType;
                    bData.DepositFee = billingData.DepositFee;
                    bData.DepositFeeUsd = billingData.DepositFeeUsd;

                    #endregion

                    bData.PaymentMethod_Approval = billingData.PaymentMethod_Approval;
                    bData.PaymentMethod_CompleteInstallation = billingData.PaymentMethod_CompleteInstallation;
                    bData.PaymentMethod_StartService = billingData.PaymentMethod_StartService;
                    bData.PaymentMethod_Deposit = billingData.PaymentMethod_Deposit;

                    bData.TotalFee = billingData.TotalFee;
                    bData.TotalFeeUsd = billingData.TotalFeeUsd;

                    //Update billing client
                    bData.BillingClientCode = temp.BillingClientCode;
                    bData.BillingTargetCode = temp.BillingTargetCode;
                    bData.BillingClient = temp.BillingClient;

                    param.TempBillingTargetData = null;
                    CTS010_ScreenData = param;


                    decimal? totalContractFee = 0;
                    decimal? totalContractFeeUS = 0;
                    decimal? totalInstallationFee = 0;
                    decimal? totalInstallationFeeUS = 0;
                    decimal? totalDepositFee = 0;
                    decimal? totalDepositFeeUS = 0;
                    foreach (CTS010_BillingTargetData bill in param.BillingTargetList)
                    {
                        #region Contract Fee

                        if (CommonUtil.IsNullOrEmpty(bill.ContractFee) == false)
                            totalContractFee += bill.ContractFee;
                        if (CommonUtil.IsNullOrEmpty(bill.ContractFeeUsd) == false)
                            totalContractFeeUS += bill.ContractFeeUsd;

                        #endregion
                        #region Install Fee Approval

                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_Approval) == false)
                            totalInstallationFee += bill.InstallFee_Approval;
                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_ApprovalUsd) == false)
                            totalInstallationFeeUS += bill.InstallFee_ApprovalUsd;

                        #endregion
                        #region Install Fee Complete Installation

                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_CompleteInstallation) == false)
                            totalInstallationFee += bill.InstallFee_CompleteInstallation;
                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_CompleteInstallationUsd) == false)
                            totalInstallationFeeUS += bill.InstallFee_CompleteInstallationUsd;

                        #endregion
                        #region Install Fee Start Serice

                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_StartService) == false)
                            totalInstallationFee += bill.InstallFee_StartService;
                        if (CommonUtil.IsNullOrEmpty(bill.InstallFee_StartServiceUsd) == false)
                            totalInstallationFeeUS += bill.InstallFee_StartServiceUsd;

                        #endregion
                        #region Deposite Fee

                        if (CommonUtil.IsNullOrEmpty(bill.DepositFee) == false)
                            totalDepositFee += bill.DepositFee;
                        if (CommonUtil.IsNullOrEmpty(bill.DepositFeeUsd) == false)
                            totalDepositFeeUS += bill.DepositFeeUsd;

                        #endregion
                    }

                    result.TotalContractFee = CommonUtil.TextNumeric(totalContractFee);
                    result.TotalContractFeeUsd = CommonUtil.TextNumeric(totalContractFeeUS);
                    result.TotalInstallationFee = CommonUtil.TextNumeric(totalInstallationFee);
                    result.TotalInstallationFeeUsd = CommonUtil.TextNumeric(totalInstallationFeeUS);
                    result.TotalDepositFee = CommonUtil.TextNumeric(totalDepositFee);
                    result.TotalDepositFeeUsd = CommonUtil.TextNumeric(totalDepositFeeUS);
                }

                #endregion

                res.ResultData = result;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }


        public ActionResult CTS010_CancelBillingTargetDetail()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                param.TempBillingTargetData = null;
                CTS010_ScreenData = param;

                res.ResultData = true;
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
        public ActionResult CTS010_RegisterRentalContractData(CTS010_RegisterRentalContractData contract, tbt_DraftRentalMaintenanceDetails maintenance)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();
                if (param.doRegisterDraftRentalContractData == null)
                    param.doRegisterDraftRentalContractData = new doDraftRentalContractData();

                #region Check is Suspending

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion
                #region Check Mandatory

                ValidatorUtil validator = new ValidatorUtil(this);

                //Comment by Jutarat A. on 12102012
                //if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE)
                //{
                //    if (CommonUtil.IsNullOrEmpty(contract.BICContractCode))
                //    {
                //        validator.AddErrorMessage(
                //            MessageUtil.MODULE_CONTRACT,
                //            "CTS010",
                //            MessageUtil.MODULE_COMMON,
                //            MessageUtil.MessageList.MSG0007,
                //            "BICContractCode",
                //            "lblBICContractCode",
                //            "BICContractCode",
                //            "11");
                //    }
                //}
                //End Comment

                bool isEmailEmpty = true;
                if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail != null)
                {
                    if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail.Count > 0)
                        isEmailEmpty = false;
                }
                if (isEmailEmpty)
                {
                    validator.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS010",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "draftRentalEmail",
                            "headerEmail",
                            "Email",
                            "20");
                }

                if (param.doRegisterDraftRentalContractData.doContractCustomer == null)
                {
                    validator.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS010",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "doContractCustomer",
                            "lblContractTarget",
                            null,
                            "21");
                }
                if (param.doRegisterDraftRentalContractData.doRealCustomer == null)
                {
                    validator.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS010",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "doRealCustomer",
                            "lblRealCustomerInfo",
                            null,
                            "27");
                }
                if (param.doRegisterDraftRentalContractData.doSite == null)
                {
                    validator.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS010",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "doSite",
                            "lblSiteInfo",
                            null,
                            "28");
                }

                if (param.BillingTargetList == null)
                    param.BillingTargetList = new List<CTS010_BillingTargetData>();
                if (param.BillingTargetList.Count == 0)
                {
                    validator.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS010",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "BillingTargetList",
                            "lblBillingTarget",
                            null,
                            "28");
                }

                object objVar = null;
                if (param.doDraftRentalContractData.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                    || param.doDraftRentalContractData.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    if ((contract.OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                        && CommonUtil.IsNullOrEmpty(contract.OrderContractFeeUsd))
                        || (contract.OrderContractFeeCurrencyType != SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            && CommonUtil.IsNullOrEmpty(contract.OrderContractFee)))
                    {
                        validator.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                "CTS010",
                                MessageUtil.MODULE_COMMON,
                                MessageUtil.MessageList.MSG0007,
                                "OrderContractFee",
                                "lblMSGContractFee",
                                "OrderContractFee",
                                "5");
                    }
                    if ((contract.OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                        && CommonUtil.IsNullOrEmpty(contract.OrderInstallFeeUsd))
                        || (contract.OrderInstallFeeCurrencyType != SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            && CommonUtil.IsNullOrEmpty(contract.OrderInstallFee)))
                    {
                        validator.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                "CTS010",
                                MessageUtil.MODULE_COMMON,
                                MessageUtil.MessageList.MSG0007,
                                "OrderInstallFee",
                                "lblMSGInstallationFee",
                                "OrderInstallFee",
                                "6");
                    }
                    if ((contract.OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                        && CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_ApproveContractUsd))
                        || (contract.OrderInstallFee_ApproveContractCurrencyType != SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            && CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_ApproveContract)))
                    {
                        validator.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                "CTS010",
                                MessageUtil.MODULE_COMMON,
                                MessageUtil.MessageList.MSG0007,
                                "OrderInstallFee_ApproveContract",
                                "lblMSGApproveContract",
                                "OrderInstallFee_ApproveContract",
                                "7");
                    }
                    if ((contract.OrderInstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                        && CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_CompleteInstallUsd))
                        || (contract.OrderInstallFee_CompleteInstallCurrencyType != SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            && CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_CompleteInstall)))
                    {
                        validator.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                "CTS010",
                                MessageUtil.MODULE_COMMON,
                                MessageUtil.MessageList.MSG0007,
                                "OrderInstallFee_CompleteInstall",
                                "lblMSGCompleteInstallation",
                                "OrderInstallFee_CompleteInstall",
                                "8");
                    }
                    if ((contract.OrderInstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                        && CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_StartServiceUsd))
                        || (contract.OrderInstallFee_StartServiceCurrencyType != SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            && CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_StartService)))
                    {
                        validator.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                "CTS010",
                                MessageUtil.MODULE_COMMON,
                                MessageUtil.MessageList.MSG0007,
                                "OrderInstallFee_StartService",
                                "lblMSGStartService",
                                "OrderInstallFee_StartService",
                                "9");
                    }
                    if ((contract.OrderDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                        && CommonUtil.IsNullOrEmpty(contract.OrderDepositFeeUsd))
                        || (contract.OrderDepositFeeCurrencyType != SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            && CommonUtil.IsNullOrEmpty(contract.OrderDepositFee)))
                    {
                        validator.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                "CTS010",
                                MessageUtil.MODULE_COMMON,
                                MessageUtil.MessageList.MSG0007,
                                "OrderDepositFee",
                                "lblMSGOrderDepositFee",
                                "OrderDepositFee",
                                "10");
                    }

                    objVar = CommonUtil.CloneObject<CTS010_RegisterRentalContractData, CTS010_RegisterRentalContractData_AL>(contract);

                    
                }
                else
                {
                    if ((contract.OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                        && CommonUtil.IsNullOrEmpty(contract.OrderContractFeeUsd))
                        || (contract.OrderContractFeeCurrencyType != SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            && CommonUtil.IsNullOrEmpty(contract.OrderContractFee)))
                    {
                        validator.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                "CTS010",
                                MessageUtil.MODULE_COMMON,
                                MessageUtil.MessageList.MSG0007,
                                "OrderContractFee",
                                "lblMSGContractFee",
                                "OrderContractFee",
                                "5");
                    }

                    objVar = CommonUtil.CloneObject<CTS010_RegisterRentalContractData, CTS010_RegisterRentalContractData_OTHER>(contract);
                }

                ValidatorUtil.BuildErrorMessage(res, validator, new object[] { objVar });
                if (res.IsError)
                    return Json(res);

                #endregion
                #region Mapping Data

                CTS010_MappingDraftContractData(param, contract, maintenance);

                #endregion
                #region Validate Business

                if (CTS010_ValidateBusiness(res,
                                            param.doRegisterDraftRentalContractData,
                                            param.BillingTargetList,
                                            contract.IsContractCodeForDepositFeeSlide,
                                            contract.IsBranchChecked,
                                            contract.IsBillingEditMode) == false)
                {
                    return Json(res);
                }

                #endregion
                #region Validate Customer

                CommonUtil cmm = new CommonUtil();
                ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                if (param.doRegisterDraftRentalContractData.doContractCustomer != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.doRegisterDraftRentalContractData.doContractCustomer.CustCode) == true)
                    {
                        if (custhandler.CheckDuplicateCustomer_IDNo(param.doRegisterDraftRentalContractData.doContractCustomer) == true)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1003);
                            return Json(res);
                        }
                    }
                }
                if (param.doRegisterDraftRentalContractData.doRealCustomer != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.doRegisterDraftRentalContractData.doRealCustomer.CustCode) == true)
                    {
                        if (custhandler.CheckDuplicateCustomer_IDNo(param.doRegisterDraftRentalContractData.doRealCustomer) == true)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1003);
                            return Json(res);
                        }
                    }
                }

                #endregion
                #region Validate Business for Warning

                #region Contract fee

                if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType ==
                        SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFee > 0
                        && (CommonUtil.IsNullOrEmpty(param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee)
                                || param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee == 0)
                        && CommonUtil.IsNullOrEmpty(param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.ApproveNo1) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3083);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                    if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFee > 0
                        && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee > 0
                        && CommonUtil.IsNullOrEmpty(param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.ApproveNo1) == false)
                    {
                        decimal? fee10 = (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFee * 0.1M);
                        decimal? fee1000 = (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFee * 10.0M);
                        if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee <= fee10)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3084);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        }
                        if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFee >= fee1000)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3084);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        }
                    }
                }
                else
                {
                    if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFeeUsd > 0
                        && (CommonUtil.IsNullOrEmpty(param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFeeUsd)
                                || param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFeeUsd == 0)
                        && CommonUtil.IsNullOrEmpty(param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.ApproveNo1) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3083);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    }
                    if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFeeUsd > 0
                        && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFeeUsd > 0
                        && CommonUtil.IsNullOrEmpty(param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.ApproveNo1) == false)
                    {
                        decimal? fee10 = (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFeeUsd * 0.1M);
                        decimal? fee1000 = (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.NormalContractFeeUsd * 10.0M);
                        if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFeeUsd <= fee10)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3084);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        }
                        if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFeeUsd >= fee1000)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3084);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        }
                    }
                }

                //if ()
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3340);
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                //}

                #endregion
                #region Order / Billing

                bool isSameOrderContractFee = true;
                bool isSameOrderInstallFee_Approval = true;
                bool isSameOrderInstallFee_CompleteInstall = true;
                bool isSameOrderInstallFee_StartService = true;
                bool isSameOrderDepositFee = true;
                if (param.BillingTargetList != null)
                {
                    foreach (CTS010_BillingTargetData billing in param.BillingTargetList)
                    {
                        if (billing.ContractFee > 0
                            || billing.ContractFeeUsd > 0)
                        {
                            if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderContractFeeCurrencyType != billing.ContractFeeCurrencyType
                                    && !CommonUtil.IsNullOrEmpty(billing.ContractFeeCurrencyType))
                                isSameOrderContractFee = false;
                        }
                        if (billing.InstallFee_Approval > 0
                            || billing.InstallFee_ApprovalUsd > 0)
                        {
                            if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType != param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType
                                    && !CommonUtil.IsNullOrEmpty(billing.InstallFee_ApprovalCurrencyType))
                                isSameOrderInstallFee_Approval = false;
                        }
                        if (billing.InstallFee_CompleteInstallation > 0
                            || billing.InstallFee_CompleteInstallationUsd > 0)
                        {
                            if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType != param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType
                                    && !CommonUtil.IsNullOrEmpty(billing.InstallFee_CompleteInstallationCurrencyType))
                                isSameOrderInstallFee_CompleteInstall = false;
                        }
                        if (billing.InstallFee_StartService > 0
                            || billing.InstallFee_StartServiceUsd > 0)
                        {
                            if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType != param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType
                                    && !CommonUtil.IsNullOrEmpty(billing.InstallFee_StartServiceCurrencyType))
                                isSameOrderInstallFee_StartService = false;
                        }
                        if (billing.DepositFee > 0
                            || billing.DepositFeeUsd > 0)
                        {
                            if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderDepositFeeCurrencyType != billing.DepositFeeCurrencyType
                                    && !CommonUtil.IsNullOrEmpty(billing.DepositFeeCurrencyType))
                                isSameOrderDepositFee = false;
                        }

                        //Add requirement
                        decimal? sumOrderInstallFee = 0;
                        if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            sumOrderInstallFee = sumOrderInstallFee + (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractUsd ?? 0);
                        else
                            sumOrderInstallFee = sumOrderInstallFee + (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContract ?? 0);

                        if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            sumOrderInstallFee = sumOrderInstallFee + (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallUsd ?? 0);
                        else
                            sumOrderInstallFee = sumOrderInstallFee + (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstall ?? 0);

                        if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            sumOrderInstallFee = sumOrderInstallFee + (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceUsd ?? 0);
                        else
                            sumOrderInstallFee = sumOrderInstallFee + (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartService ?? 0);

                        if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            if(((param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeUsd ?? 0) != sumOrderInstallFee
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType == param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType == param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType == param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType)
                                || ((param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeUsd ?? 0) != sumOrderInstallFee 
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType != param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType
                                && (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContract == 0 || param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractUsd == 0))

                                || ((param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeUsd ?? 0) != sumOrderInstallFee
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType != param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType
                                && (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstall == 0 || param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallUsd == 0))

                                || ((param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeUsd ?? 0) != sumOrderInstallFee
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType != param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType
                                && (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartService == 0 || param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceUsd == 0)))
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3088, null, new string[] { "OrderInstallFee" });
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                return Json(res);
                            }
                        }
                        else
                        {
                            if (((param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee ?? 0) != sumOrderInstallFee
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType == param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType == param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType == param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType)
                                || ((param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee ?? 0) != sumOrderInstallFee
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType != param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType
                                && (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContract == 0 || param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractUsd == 0))

                                || ((param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee ?? 0) != sumOrderInstallFee
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType != param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType
                                && (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstall == 0 || param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallUsd == 0))

                                || ((param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee ?? 0) != sumOrderInstallFee
                                && param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType != param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType
                                && (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartService == 0 || param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceUsd == 0)))
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3088, null, new string[] { "OrderInstallFee" });
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                return Json(res);
                            }
                        }
                    }
                }


                if (isSameOrderContractFee == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3340);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }

                if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType !=
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3341);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }
                if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType !=
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3314);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }
                //if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType !=
                //    param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3315);
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                //}

                if (isSameOrderInstallFee_Approval == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3316);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }
                if (isSameOrderInstallFee_CompleteInstall == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3317);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }
                if (isSameOrderInstallFee_StartService == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3318);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }

                if (isSameOrderDepositFee == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3319);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }

                #endregion
                #region Contract payment term

                if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.CalDailyFeeStatus != CalculationDailyFeeType.C_CALC_DAILY_FEE_TYPE_30_4)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3086);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }

                #endregion
                #region Line-up type

                if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalInstrument != null)
                {
                    foreach (tbt_DraftRentalInstrument inst in param.doRegisterDraftRentalContractData.doTbt_DraftRentalInstrument)
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

                CTS010_ScreenData = param;
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Event when clicking Comfirm (Step 1: check contract customer is duplicate?)
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS010_ConfirmRentalContractData_P1()
        {
            //CheckTimeLog("Start confirm rental contract data");

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();

                #region Check is Suspending

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (chandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion
                #region Validate Business

                if (CTS010_ValidateBusiness(res, param.doRegisterDraftRentalContractData, param.BillingTargetList, false) == false)
                    return Json(res);

                #endregion

                doDraftRentalContractData draft = param.doRegisterDraftRentalContractData;

                #region Validate quotation is registered?

                IDraftRentalContractHandler rhandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
                List<tbt_DraftRentalContract> rList = rhandler.GetTbt_DraftRentalContract(draft.doTbt_DraftRentalContrat.QuotationTargetCodeFull);
                if (rList.Count > 0)
                {
                    if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.NEW)
                    {

                        if (rList[0].DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_WAITFORAPPROVE)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3243);
                            return Json(res);
                        }
                        else if (rList[0].DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_REJECTED)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3244);
                            return Json(res);
                        }
                        else if (rList[0].DraftRentalContractStatus == ApprovalStatus.C_APPROVE_STATUS_RETURNED)
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
                    else if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.EDIT)
                    {
                        if (rList[0].DraftRentalContractStatus != ApprovalStatus.C_APPROVE_STATUS_RETURNED)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3068);
                            return Json(res);
                        }
                    }
                }

                #endregion

                CommonUtil cmm = new CommonUtil();
                ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                if (draft.doContractCustomer != null)
                {
                    if (CommonUtil.IsNullOrEmpty(draft.doContractCustomer.CustCode))
                    {
                        if (custhandler.CheckDuplicateCustomer_IDNo(draft.doContractCustomer) == true)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1003);
                            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                            return Json(res);
                        }
                    }
                    if (draft.doContractCustomer.IsNameLCChanged == true
                        && custhandler.CheckDuplicateCustomer_CustNameLC(draft.doContractCustomer) == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1004);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.ResultData = 1;
                        return Json(res);
                    }
                }

                return CTS010_ConfirmRentalContractData_P2();
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
        public ActionResult CTS010_ConfirmRentalContractData_P2()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();

                doDraftRentalContractData draft = param.doRegisterDraftRentalContractData;

                bool isSameCustomer = CTS010_IsSameCustomer(draft.doContractCustomer, draft.doRealCustomer);
                if (isSameCustomer == false)
                {
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
                }

                return CTS010_ConfirmRentalContractData_P3();
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
        public ActionResult CTS010_ConfirmRentalContractData_P3()
        {
            //CheckTimeLog("Finish confirm rental contract data");

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();

                doDraftRentalContractData draft = param.doRegisterDraftRentalContractData;

                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<dtQuotationNoData> lstQuotationData = handler.GetQuotationNo(draft.doTbt_DraftRentalContrat.QuotationTargetCode, draft.doTbt_DraftRentalContrat.Alphabet);

                //Get and Add QuotationNo for save
                if (lstQuotationData != null)
                    draft.QuotationNo = lstQuotationData[0].QuotationNo;

                CommonUtil cmm = new CommonUtil();
                ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                List<CTS010_BillingTargetData> tmpBillingTargetList = CommonUtil.ClonsObjectList<CTS010_BillingTargetData, CTS010_BillingTargetData>(param.BillingTargetList);

                #region Check Data is changed?

                CTS010_RetrieveQuotationCondition_Edit cond = new CTS010_RetrieveQuotationCondition_Edit();
                cond.QuotationTargetCode = draft.doTbt_DraftRentalContrat.QuotationTargetCodeShort;

                IDraftRentalContractHandler rhandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
                List<tbt_DraftRentalContract> contractLst = rhandler.GetTbt_DraftRentalContract(cond.QuotationTargetCodeLong);
                if (contractLst.Count > 0)
                {
                    if (CommonUtil.IsNullOrEmpty(param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.UpdateDate)
                        || contractLst[0].UpdateDate > param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.UpdateDate)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }

                #endregion

                using (TransactionScope scope = new TransactionScope())
                {
                    //CheckTimeLog("Start update customer data");

                    #region Update Customer

                    bool isSameCustomer = CTS010_IsSameCustomer(draft.doContractCustomer, draft.doRealCustomer);
                    if (draft.doContractCustomer != null)
                    {
                        doCustomerTarget custTarget = new doCustomerTarget();
                        custTarget.doCustomer = draft.doContractCustomer;
                        custTarget.dtCustomerGroup = CommonUtil.ClonsObjectList<dtCustomeGroupData, dtCustomerGroup>(draft.doContractCustomer.CustomerGroupData);

                        custTarget.doSite = null;
                        if (isSameCustomer)
                            custTarget.doSite = draft.doSite;

                        custTarget = custhandler.ManageCustomerTarget(custTarget);

                        if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE)
                        {
                            custhandler.ManageCustomerInformation(custTarget.doCustomer.CustCode);
                        }

                        draft.doTbt_DraftRentalContrat.ContractTargetCustCode = custTarget.doCustomer.CustCode;

                        if (isSameCustomer)
                        {
                            draft.doTbt_DraftRentalContrat.RealCustomerCustCode = custTarget.doCustomer.CustCode;
                            draft.doTbt_DraftRentalContrat.SiteCode = null;
                            if (custTarget.doSite != null)
                                draft.doTbt_DraftRentalContrat.SiteCode = custTarget.doSite.SiteCode;

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

                        if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE)
                        {
                            custhandler.ManageCustomerInformation(realCust.doCustomer.CustCode);
                        }

                        draft.doTbt_DraftRentalContrat.RealCustomerCustCode = realCust.doCustomer.CustCode;

                        draft.doTbt_DraftRentalContrat.SiteCode = null;
                        if (realCust.doSite != null)
                            draft.doTbt_DraftRentalContrat.SiteCode = realCust.doSite.SiteCode;
                    }

                    #endregion

                    //CheckTimeLog("Finish update customer data");

                    #region Billing Target

                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalBillingTarget = new List<tbt_DraftRentalBillingTarget>();
                    if (param.BillingTargetList != null)
                    {
                        IBillingMasterHandler bhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

                        int sequenceNo = 1;
                        foreach (CTS010_BillingTargetData billing in tmpBillingTargetList)
                        {
                            if (CommonUtil.IsNullOrEmpty(billing.BillingClientCode))
                            {
                                var newClientCode = bhandler.ManageBillingClient(billing.BillingClient);
                                billing.BillingClientCode = newClientCode;
                                billing.BillingTargetCode = null;
                            }
                            else
                            {
                                IBillingClientMasterHandler hand = ServiceContainer.GetService<IBillingClientMasterHandler>() as IBillingClientMasterHandler;
                                var billingclient = hand.GetTbm_BillingClient(billing.BillingClientCode).FirstOrDefault();

                                if (billingclient != null)
                                {
                                    billingclient.DeleteFlag = null;
                                    IMasterHandler mhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                                    List<tbm_Region> rgLst = mhandler.GetTbm_Region();
                                    billingclient.Nationality = rgLst.Where(rg => rg.RegionCode == billingclient.RegionCode).Select(rg => rg.Nationality).FirstOrDefault();
                                    List<tbm_BusinessType> bLst = mhandler.GetTbm_BusinessType();
                                    billingclient.BusinessTypeName = bLst.Where(b => b.BusinessTypeCode == billingclient.BusinessTypeCode).Select(b => b.BusinessTypeName).FirstOrDefault();
                                }

                                if (billingclient == null || CTS010_IsSameBillingClient(billing.BillingClient, billingclient) == false)
                                {
                                    var newClientCode = bhandler.ManageBillingClient(billing.BillingClient);
                                    billing.BillingClientCode = newClientCode;
                                    billing.BillingTargetCode = null;
                                }
                            }

                            for (int idx = 0; idx < 5; idx++)
                            {
                                tbt_DraftRentalBillingTarget bt = new tbt_DraftRentalBillingTarget();
                                bt.QuotationTargetCode = param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.QuotationTargetCode;
                                bt.SequenceNo = sequenceNo;
                                bt.BillingTargetCode = billing.BillingTargetCode;
                                bt.BillingClientCode = billing.BillingClientCode;
                                bt.BillingOfficeCode = billing.BillingOfficeCode;
                                bt.DocLanguage = billing.DocLanguage; //Add by Jutarat A. on 18122013
                                bt.DebtTracingOfficeCode = null;
                                bt.BillingCycle = param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.BillingCycle;
                                bt.CalDailyFeeStatus = param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.CalDailyFeeStatus;

                                if (idx == 0) //Contract
                                {
                                    bt.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE;
                                    if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                                    {
                                        bt.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON;
                                        if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails != null)
                                        {
                                            if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceFeeTypeCode == "1")
                                                bt.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE;
                                        }
                                    }

                                    bt.BillingTiming = null;
                                    bt.BillingAmtCurrencyType = billing.ContractFeeCurrencyType;
                                    bt.BillingAmt = billing.ContractFee;
                                    bt.BillingAmtUsd = billing.ContractFeeUsd;

                                    bt.PayMethod = draft.doTbt_DraftRentalContrat.PayMethod;
                                }
                                else if (idx == 1) //Deposit
                                {
                                    bt.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE;
                                    bt.BillingTiming = param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat.BillingTimingDepositFee;

                                    bt.BillingAmtCurrencyType = billing.DepositFeeCurrencyType;
                                    bt.BillingAmt = billing.DepositFee;
                                    bt.BillingAmtUsd = billing.DepositFeeUsd;

                                    bt.PayMethod = billing.PaymentMethod_Deposit;
                                }
                                else
                                {
                                    bt.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE;

                                    if (idx == 2 && 
                                        (billing.InstallFee_Approval > 0 || billing.InstallFee_ApprovalUsd > 0)) //Approve
                                    {
                                        bt.BillingTiming = BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT;

                                        bt.BillingAmtCurrencyType = billing.InstallFee_ApprovalCurrencyType;
                                        bt.BillingAmt = billing.InstallFee_Approval;
                                        bt.BillingAmtUsd = billing.InstallFee_ApprovalUsd;

                                        bt.PayMethod = billing.PaymentMethod_Approval;
                                    }
                                    else if (idx == 3 && 
                                        (billing.InstallFee_CompleteInstallation > 0 || billing.InstallFee_CompleteInstallationUsd > 0)) //Complete Installation
                                    {
                                        bt.BillingTiming = BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION;

                                        bt.BillingAmtCurrencyType = billing.InstallFee_CompleteInstallationCurrencyType;
                                        bt.BillingAmt = billing.InstallFee_CompleteInstallation;
                                        bt.BillingAmtUsd = billing.InstallFee_CompleteInstallationUsd;

                                        bt.PayMethod = billing.PaymentMethod_CompleteInstallation;
                                    }
                                    else if (idx == 4 && 
                                        (billing.InstallFee_StartService > 0 || billing.InstallFee_StartServiceUsd > 0)) //Start Service
                                    {
                                        bt.BillingTiming = BillingTiming.C_BILLING_TIMING_START_SERVICE;

                                        bt.BillingAmtCurrencyType = billing.InstallFee_StartServiceCurrencyType;
                                        bt.BillingAmt = billing.InstallFee_StartService;
                                        bt.BillingAmtUsd = billing.InstallFee_StartServiceUsd;

                                        bt.PayMethod = billing.PaymentMethod_StartService;
                                    }
                                }

                                if (bt.BillingAmt != null || bt.BillingAmtUsd != null)
                                {
                                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalBillingTarget.Add(bt);
                                    sequenceNo++;
                                }
                            }

                        }
                    }

                    #endregion
                    #region Update Draft Rental Data

                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    draft.doTbt_DraftRentalContrat.ApproveContractDate = null;

                    //IDraftRentalContractHandler rhandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
                    if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.NEW)
                    {
                        draft.doTbt_DraftRentalContrat.RegisterDate = dsTrans.dtOperationData.ProcessDateTime;
                        rhandler.CreateDraftRentalContractData(draft);
                    }
                    else
                    {
                        if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.EDIT)
                            rhandler.EditDraftRentalContractData(draft);
                        else if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE)
                        {
                            draft.doTbt_DraftRentalContrat.ApproveContractDate = dsTrans.dtOperationData.ProcessDateTime;
                            CTS010_ApproveContract(draft);
                        }
                    }

                    #endregion

                    //CheckTimeLog("Start update quotation data");

                    #region Update Quotation Data

                    doUpdateQuotationData qData = new doUpdateQuotationData()
                    {
                        QuotationTargetCode = draft.doTbt_DraftRentalContrat.QuotationTargetCode,
                        Alphabet = draft.doTbt_DraftRentalContrat.Alphabet,
                        LastUpdateDate = draft.LastUpdateDateQuotationData,
                        ContractCode = null,
                        ActionTypeCode = ActionType.C_ACTION_TYPE_DRAFT
                    };

                    if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE)
                    {
                        qData.ContractCode = draft.doTbt_DraftRentalContrat.ContractCode;
                        qData.ActionTypeCode = ActionType.C_ACTION_TYPE_APPROVE;
                    }

                    IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    qhandler.UpdateQuotationData(qData);


                    #endregion

                    //CheckTimeLog("Finish update quotation data");

                    #region Update Data

                    param.BillingTargetList = CTS010_SetBillingList(draft.doTbt_DraftRentalBillingTarget);
                    CTS010_ScreenData = param;

                    #endregion

                    bool isComplete = true;
                    if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.APPROVE)
                    {
                        #region Update to CTS030

                        CTS030_UpdateDataFromChildPage(param.CallerKey,
                                                        draft.doTbt_DraftRentalContrat.QuotationTargetCode,
                                                        draft.doTbt_DraftRentalContrat.ContractCode,
                                                        ApprovalStatus.C_APPROVE_STATUS_APPROVED);

                        #endregion
                        #region Send Email

                        if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail != null)
                        {
                            List<string> mailAddress = new List<string>();
                            foreach (tbt_DraftRentalEmail e in param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail)
                            {
                                mailAddress.Add(e.EmailAddress);
                            }
                            SendMail(CTS010_GenerateApproveMailTemplate(draft), mailAddress);
                        }

                        #endregion

                        if (isComplete)
                            res.ResultData = CallScreenURL(param, true);
                    }
                    else
                    {
                        CTS010_RegisterResult registerResult = new CTS010_RegisterResult()
                        {
                            ContractTargetCustCode = cmm.ConvertCustCode(draft.doTbt_DraftRentalContrat.ContractTargetCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                            RealCustomerCustCode = cmm.ConvertCustCode(draft.doTbt_DraftRentalContrat.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                            SiteCode = cmm.ConvertSiteCode(draft.doTbt_DraftRentalContrat.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT)
                        };

                        if (draft.doContractCustomer != null)
                        {
                            registerResult.ContractTargetIDNo = draft.doContractCustomer.IDNo;
                            registerResult.ContractTargetStatusCodeName = draft.doContractCustomer.CustStatusCodeName;
                        }
                        if (draft.doRealCustomer != null)
                        {
                            registerResult.RealCustomerIDNo = draft.doRealCustomer.IDNo;
                            registerResult.RealCustomerStatusCodeName = draft.doRealCustomer.CustStatusCodeName;
                        }

                        res.ResultData = new object[]{
                            MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3264),
                            registerResult
                        };
                    }

                    scope.Complete();
                    param.BillingTargetList = CommonUtil.ClonsObjectList<CTS010_BillingTargetData, CTS010_BillingTargetData>(tmpBillingTargetList);


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
        public ActionResult CTS010_RejectRentalContractData_P1()
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
        public ActionResult CTS010_RejectRentalContractData_P2()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();

                if (param.doDraftRentalContractData != null)
                {
                    #region Check Data is changed?

                    CTS010_RetrieveQuotationCondition_Edit cond = new CTS010_RetrieveQuotationCondition_Edit();
                    cond.QuotationTargetCode = param.doDraftRentalContractData.doTbt_DraftRentalContrat.QuotationTargetCodeShort;

                    IDraftRentalContractHandler rhandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
                    List<tbt_DraftRentalContract> contractLst = rhandler.GetTbt_DraftRentalContract(cond.QuotationTargetCodeLong);
                    if (contractLst.Count > 0)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.UpdateDate)
                        || contractLst[0].UpdateDate > param.doDraftRentalContractData.doTbt_DraftRentalContrat.UpdateDate)
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
                        param.doDraftRentalContractData.doTbt_DraftRentalContrat.DraftRentalContractStatus = ApprovalStatus.C_APPROVE_STATUS_REJECTED;
                        param.doDraftRentalContractData.doTbt_DraftRentalContrat.ApproveContractDate = dsTrans.dtOperationData.ProcessDateTime;

                        //IDraftRentalContractHandler rhandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
                        rhandler.UpdateTbt_DraftRentalContract(param.doDraftRentalContractData.doTbt_DraftRentalContrat);

                        #region Lock Quotation Data

                        IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                        qhandler.LockQuotation(
                            param.doDraftRentalContractData.doTbt_DraftRentalContrat.QuotationTargetCode,
                            param.doDraftRentalContractData.doTbt_DraftRentalContrat.Alphabet,
                            LockStyle.C_LOCK_STYLE_ALL);

                        #endregion

                        /*Comment by Jutarat A. on 25062013 (No cancel data)
                        #region Update Quotation Data

                        doUpdateQuotationData qData = new doUpdateQuotationData()
                        {
                            QuotationTargetCode = param.doDraftRentalContractData.doTbt_DraftRentalContrat.QuotationTargetCode,
                            Alphabet = param.doDraftRentalContractData.doTbt_DraftRentalContrat.Alphabet,
                            LastUpdateDate = param.doDraftRentalContractData.LastUpdateDateQuotationData,
                            ContractCode = null,
                            ActionTypeCode = ActionType.C_ACTION_TYPE_CANCEL
                        };
                        qhandler.UpdateQuotationData(qData);

                        #endregion*/

                        scope.Complete();
                    }

                    #region Update to CTS030

                    CTS030_UpdateDataFromChildPage(param.CallerKey,
                                                    param.doDraftRentalContractData.doTbt_DraftRentalContrat.QuotationTargetCode,
                                                    param.doDraftRentalContractData.doTbt_DraftRentalContrat.ContractCode,
                                                    ApprovalStatus.C_APPROVE_STATUS_REJECTED);

                    #endregion
                    #region Send Email

                    try
                    {
                        if (param.doDraftRentalContractData.doTbt_DraftRentalEmail != null)
                        {
                            List<string> mailAddress = new List<string>();
                            foreach (tbt_DraftRentalEmail e in param.doDraftRentalContractData.doTbt_DraftRentalEmail)
                            {
                                mailAddress.Add(e.EmailAddress);
                            }
                            SendMail(CTS010_GenerateRejectMailTemplate(param.doDraftRentalContractData), mailAddress);
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
        public ActionResult CTS010_ReturnRentalContractData_P1()
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
        public ActionResult CTS010_ReturnRentalContractData_P2()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
                if (param == null)
                    param = new CTS010_ScreenParameter();

                if (param.doDraftRentalContractData != null)
                {
                    #region Check Data is changed?

                    CTS010_RetrieveQuotationCondition_Edit cond = new CTS010_RetrieveQuotationCondition_Edit();
                    cond.QuotationTargetCode = param.doDraftRentalContractData.doTbt_DraftRentalContrat.QuotationTargetCodeShort;

                    IDraftRentalContractHandler rhandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
                    List<tbt_DraftRentalContract> contractLst = rhandler.GetTbt_DraftRentalContract(cond.QuotationTargetCodeLong);
                    if (contractLst.Count > 0)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.doDraftRentalContractData.doTbt_DraftRentalContrat.UpdateDate)
                        || contractLst[0].UpdateDate > param.doDraftRentalContractData.doTbt_DraftRentalContrat.UpdateDate)
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
                        param.doDraftRentalContractData.doTbt_DraftRentalContrat.DraftRentalContractStatus = ApprovalStatus.C_APPROVE_STATUS_RETURNED;
                        param.doDraftRentalContractData.doTbt_DraftRentalContrat.ApproveContractDate = dsTrans.dtOperationData.ProcessDateTime;

                        //IDraftRentalContractHandler rhandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
                        rhandler.UpdateTbt_DraftRentalContract(param.doDraftRentalContractData.doTbt_DraftRentalContrat);

                        scope.Complete();
                    }

                    #region Update to CTS030

                    CTS030_UpdateDataFromChildPage(param.CallerKey,
                                                    param.doDraftRentalContractData.doTbt_DraftRentalContrat.QuotationTargetCode,
                                                    param.doDraftRentalContractData.doTbt_DraftRentalContrat.ContractCode,
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
        public ActionResult CTS010_CloseRentalContract()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS010_ScreenParameter param = CTS010_ScreenData;
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
        private CTS010_ScreenParameter CTS010_ScreenData
        {
            get
            {
                return GetScreenObject<CTS010_ScreenParameter>();
            }
            set
            {
                UpdateScreenObject(value);
            }
        }
        /// <summary>
        /// Mapping object data
        /// </summary>
        /// <param name="param"></param>
        /// <param name="contract"></param>
        /// <param name="maintenance"></param>
        private void CTS010_MappingDraftContractData(CTS010_ScreenParameter param, CTS010_RegisterRentalContractData contract, tbt_DraftRentalMaintenanceDetails maintenance)
        {
            try
            {
                if (param.doRegisterDraftRentalContractData == null)
                    param.doRegisterDraftRentalContractData = new doDraftRentalContractData();

                #region Draft rental contract

                param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat =
                    CommonUtil.CloneObject<tbt_DraftRentalContract, tbt_DraftRentalContract>(param.doDraftRentalContractData.doTbt_DraftRentalContrat);
                tbt_DraftRentalContract draftContract = param.doRegisterDraftRentalContractData.doTbt_DraftRentalContrat;

                draftContract.QuotationTargetCode = contract.QuotationTargetCodeLong;
                draftContract.Alphabet = contract.Alphabet;

                if (param.doRegisterDraftRentalContractData.doContractCustomer != null)
                    draftContract.ContractTargetCustCode = param.doRegisterDraftRentalContractData.doContractCustomer.CustCode;

                draftContract.ContractTargetSignerTypeCode = contract.ContractTargetSignerTypeCode;
                draftContract.BranchNameEN = contract.BranchNameEN;
                draftContract.BranchNameLC = contract.BranchNameLC;
                draftContract.BranchAddressEN = contract.BranchAddressEN;
                draftContract.BranchAddressLC = contract.BranchAddressLC;

                if (param.doDraftRentalContractData.doTbt_DraftRentalContrat != null)
                {
                    draftContract.ContractTargetMemo = param.doDraftRentalContractData.doTbt_DraftRentalContrat.ContractTargetMemo;
                    draftContract.RealCustomerMemo = param.doDraftRentalContractData.doTbt_DraftRentalContrat.RealCustomerMemo;
                }

                if (param.doRegisterDraftRentalContractData.doRealCustomer != null)
                    draftContract.RealCustomerCustCode = param.doRegisterDraftRentalContractData.doRealCustomer.CustCode;

                draftContract.ContactPoint = contract.ContactPoint;

                if (param.doRegisterDraftRentalContractData.doSite != null)
                    draftContract.SiteCode = param.doRegisterDraftRentalContractData.doSite.SiteCode;

                if (param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.NEW
                    || param.ScreenMode == CTS010_ScreenParameter.SCREEN_MODE.EDIT)
                {
                    draftContract.DraftRentalContractStatus = ApprovalStatus.C_APPROVE_STATUS_WAITFORAPPROVE;
                    draftContract.ContractCode = null;
                }
                else
                {
                    draftContract.DraftRentalContractStatus = ApprovalStatus.C_APPROVE_STATUS_APPROVED;
                    draftContract.ContractCode = param.ContractCode;
                }

                draftContract.ProjectCode = contract.ProjectCode;
                draftContract.ContractOfficeCode = contract.ContractOfficeCode;
                draftContract.OperationOfficeCode = contract.OperationOfficeCode;
                draftContract.ExpectedInstallCompleteDate = contract.ExpectedInstallCompleteDate;
                draftContract.ExpectedStartServiceDate = contract.ExpectedStartServiceDate;
                               
                #region Contract Fee

                draftContract.OrderContractFeeCurrencyType = contract.OrderContractFeeCurrencyType;
                draftContract.OrderContractFee = contract.OrderContractFee;
                draftContract.OrderContractFeeUsd = contract.OrderContractFeeUsd;

                #endregion
                #region Order Install Fee

                draftContract.OrderInstallFeeCurrencyType = contract.OrderInstallFeeCurrencyType;
                draftContract.OrderInstallFee = contract.OrderInstallFee;
                draftContract.OrderInstallFeeUsd = contract.OrderInstallFeeUsd;

                #endregion
                #region Order Install Fee Approve Contract

                draftContract.OrderInstallFee_ApproveContractCurrencyType = contract.OrderInstallFee_ApproveContractCurrencyType;
                draftContract.OrderInstallFee_ApproveContract = contract.OrderInstallFee_ApproveContract;
                draftContract.OrderInstallFee_ApproveContractUsd = contract.OrderInstallFee_ApproveContractUsd;

                #endregion
                #region Order Install Fee Complete Install

                draftContract.OrderInstallFee_CompleteInstallCurrencyType = contract.OrderInstallFee_CompleteInstallCurrencyType;
                draftContract.OrderInstallFee_CompleteInstall = contract.OrderInstallFee_CompleteInstall;
                draftContract.OrderInstallFee_CompleteInstallUsd = contract.OrderInstallFee_CompleteInstallUsd;

                #endregion
                #region Order Install Fee Start Service

                draftContract.OrderInstallFee_StartServiceCurrencyType = contract.OrderInstallFee_StartServiceCurrencyType;
                draftContract.OrderInstallFee_StartService = contract.OrderInstallFee_StartService;
                draftContract.OrderInstallFee_StartServiceUsd = contract.OrderInstallFee_StartServiceUsd;

                #endregion
                #region Order Deposit Fee

                draftContract.OrderDepositFeeCurrencyType = contract.OrderDepositFeeCurrencyType;
                draftContract.OrderDepositFee = contract.OrderDepositFee;
                draftContract.OrderDepositFeeUsd = contract.OrderDepositFeeUsd;

                #endregion

                draftContract.BillingTimingDepositFee = contract.BillingTimingDepositFee;

                CommonUtil cmm = new CommonUtil();
                draftContract.CounterBalanceOriginContractCode = cmm.ConvertContractCode(contract.CounterBalanceOriginContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                draftContract.IrregulationContractDurationFlag = contract.IrregulationContractDurationFlag;
                draftContract.ContractDurationMonth = contract.ContractDurationMonth;
                draftContract.AutoRenewMonth = contract.AutoRenewMonth;
                draftContract.ContractEndDate = contract.ContractEndDate;
                
                draftContract.CalContractEndDate = contract.ContractEndDate;
                //if (CommonUtil.IsNullOrEmpty(draftContract.ContractEndDate)
                //    && CommonUtil.IsNullOrEmpty(draftContract.ContractDurationMonth) == false
                //    && CommonUtil.IsNullOrEmpty(draftContract.ExpectedStartServiceDate) == false)
                //{
                //    draftContract.CalContractEndDate = draftContract.ExpectedStartServiceDate.Value.AddMonths(draftContract.ContractDurationMonth.Value).AddDays(-1);
                //}

                draftContract.BillingCycle = contract.BillingCycle;
                draftContract.PayMethod = contract.PayMethod;
                draftContract.CreditTerm = contract.CreditTerm;
                draftContract.DivideContractFeeBillingFlag = contract.DivideContractFeeBillingFlag;
                draftContract.CalDailyFeeStatus = contract.CalDailyFeeStatus;
                draftContract.SalesmanEmpNo1 = contract.SalesmanEmpNo1;
                draftContract.SalesmanEmpNo2 = contract.SalesmanEmpNo2;
                draftContract.SalesSupporterEmpNo = contract.SalesSupporterEmpNo;
                draftContract.ApproveNo1 = contract.ApproveNo1;
                draftContract.ApproveNo2 = contract.ApproveNo2;
                draftContract.ApproveNo3 = contract.ApproveNo3;
                draftContract.ApproveNo4 = contract.ApproveNo4;
                draftContract.ApproveNo5 = contract.ApproveNo5;
                draftContract.BICContractCode = contract.BICContractCode;
                draftContract.Memo = null;
                draftContract.DivideContractFeeBillingFlag = contract.DivideContractFeeBillingFlag;
                draftContract.TotalFloorArea = 0;

                #endregion
                #region Draft rental BE detail

                if (param.doDraftRentalContractData.doTbt_DraftRentalBEDetails != null)
                {
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalBEDetails =
                        CommonUtil.CloneObject<tbt_DraftRentalBEDetails, tbt_DraftRentalBEDetails>(param.doDraftRentalContractData.doTbt_DraftRentalBEDetails);
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalBEDetails.QuotationTargetCode = contract.QuotationTargetCodeLong;
                }

                #endregion
                #region Draft rental Email

                if (param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail != null)
                {
                    int emailID = 1;
                    foreach (tbt_DraftRentalEmail email in param.doRegisterDraftRentalContractData.doTbt_DraftRentalEmail)
                    {
                        email.DraftRentalEmailID = emailID;
                        email.QuotationTargetCode = contract.QuotationTargetCodeLong;
                        email.Alphabet = contract.Alphabet;
                        emailID++;
                    }
                }

                #endregion
                #region Draft rental instrument

                if (param.doDraftRentalContractData.doTbt_DraftRentalInstrument != null)
                {
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalInstrument = new List<tbt_DraftRentalInstrument>();

                    foreach (tbt_DraftRentalInstrument inst in param.doDraftRentalContractData.doTbt_DraftRentalInstrument)
                    {
                        tbt_DraftRentalInstrument ninst = CommonUtil.CloneObject<tbt_DraftRentalInstrument, tbt_DraftRentalInstrument>(inst);
                        ninst.QuotationTargetCode = contract.QuotationTargetCodeLong;

                        param.doRegisterDraftRentalContractData.doTbt_DraftRentalInstrument.Add(ninst);
                    }
                }

                #endregion
                #region Draft rental maintenance details

                if (param.doDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails != null
                    && maintenance != null)
                {
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails =
                        CommonUtil.CloneObject<tbt_DraftRentalMaintenanceDetails, tbt_DraftRentalMaintenanceDetails>(param.doDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails);

                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.QuotationTargetCode = contract.QuotationTargetCodeLong;
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceContractStartMonth = maintenance.MaintenanceContractStartMonth;
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceContractStartYear = maintenance.MaintenanceContractStartYear;
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceFeeTypeCode = maintenance.MaintenanceFeeTypeCode;
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalMaintenanceDetails.MaintenanceMemo = maintenance.MaintenanceMemo;
                }

                #endregion
                #region Draft rental operation type

                param.doRegisterDraftRentalContractData.doTbt_DraftRentalOperationType = new List<tbt_DraftRentalOperationType>();
                if (contract.OperationType != null)
                {
                    foreach (string opt in contract.OperationType)
                    {
                        param.doRegisterDraftRentalContractData.doTbt_DraftRentalOperationType.Add(new tbt_DraftRentalOperationType()
                        {
                            QuotationTargetCode = contract.QuotationTargetCodeLong,
                            OperationTypeCode = opt
                        });
                    }
                }

                #endregion
                #region Draft rental sentry guard

                if (param.doDraftRentalContractData.doTbt_DraftRentalSentryGuard != null)
                {
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalSentryGuard =
                        CommonUtil.CloneObject<tbt_DraftRentalSentryGuard, tbt_DraftRentalSentryGuard>(param.doDraftRentalContractData.doTbt_DraftRentalSentryGuard);
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalSentryGuard.QuotationTargetCode = contract.QuotationTargetCodeLong;
                }
                if (param.doDraftRentalContractData.doTbt_DraftRentalSentryGuardDetails != null)
                {
                    param.doRegisterDraftRentalContractData.doTbt_DraftRentalSentryGuardDetails = new List<tbt_DraftRentalSentryGuardDetails>();
                    foreach (tbt_DraftRentalSentryGuardDetails sgd in param.doDraftRentalContractData.doTbt_DraftRentalSentryGuardDetails)
                    {
                        tbt_DraftRentalSentryGuardDetails nsgd = CommonUtil.CloneObject<tbt_DraftRentalSentryGuardDetails, tbt_DraftRentalSentryGuardDetails>(sgd);
                        nsgd.QuotationTargetCode = contract.QuotationTargetCodeLong;

                        param.doRegisterDraftRentalContractData.doTbt_DraftRentalSentryGuardDetails.Add(nsgd);
                    }
                }

                #endregion
                #region Relation type

                if (param.doDraftRentalContractData.doTbt_RelationType != null)
                {
                    List<tbt_RelationType> nrLst = new List<tbt_RelationType>();
                    foreach (tbt_RelationType rt in param.doDraftRentalContractData.doTbt_RelationType)
                    {
                        tbt_RelationType nr = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(rt);
                        nr.ContractCode = contract.QuotationTargetCodeLong;
                        nr.OCC = contract.Alphabet;

                        nrLst.Add(nr);
                    }

                    param.doRegisterDraftRentalContractData.doTbt_RelationType = nrLst;
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
        private bool CTS010_ValidateBusiness(ObjectResultData res,
                                                doDraftRentalContractData draft,
                                                List<CTS010_BillingTargetData> billingList,
                                                bool isContractCodeForDepositFeeSlide = false,
                                                bool isBranchChecked = false,
                                                bool isBillingEditMode = false)
        {
            try
            {
                if (draft == null)
                    return false;

                tbt_DraftRentalContract contract = draft.doTbt_DraftRentalContrat;

                DateTime currentDate = DateTime.Now;
                currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                //DateTime future = new DateTime(currentDate.Year + 3, currentDate.Month, currentDate.Day);
                DateTime future = currentDate.AddYears(3);

                decimal? totalContractFee = 0;
                decimal? totalContractFeeUS = 0;
                decimal? totalInstallFee_Approve = 0;
                decimal? totalInstallFee_ApproveUS = 0;
                decimal? totalInstallFee_CompleteInstallation = 0;
                decimal? totalInstallFee_CompleteInstallationUS = 0;
                decimal? totalInstallFee_StartService = 0;
                decimal? totalInstallFee_StartServiceUS = 0;
                decimal? totalDepositFee = 0;
                decimal? totalDepositFeeUS = 0;

                bool isSameContractFeeCurrency = true;
                bool isSameInstallFee_ApprovalCurrency = true;
                bool isSameInstallFee_CompleteInstallationCurrency = true;
                bool isSameInstallFee_StartServiceCurrency = true;
                bool isSameDepositFeeCurrency = true;

                if (billingList != null)
                {
                    foreach (CTS010_BillingTargetData billing in billingList)
                    {
                        #region Contract Fee

                        if (billing.ContractFee > 0
                            || billing.ContractFeeUsd > 0)
                        {
                            if (billing.ContractFeeCurrencyType != contract.NormalContractFeeCurrencyType)
                                isSameContractFeeCurrency = false;

                            if (billing.ContractFee != null)
                                totalContractFee += billing.ContractFee;
                            if (billing.ContractFeeUsd != null)
                                totalContractFeeUS += billing.ContractFeeUsd;
                        }

                        #endregion
                        #region Install Fee Approval

                        if (billing.InstallFee_Approval > 0
                            || billing.InstallFee_ApprovalUsd > 0)
                        {
                            if (billing.InstallFee_ApprovalCurrencyType != contract.NormalInstallFeeCurrencyType)
                                isSameInstallFee_ApprovalCurrency = false;

                            if (billing.InstallFee_Approval != null)
                                totalInstallFee_Approve += billing.InstallFee_Approval;
                            if (billing.InstallFee_ApprovalUsd != null)
                                totalInstallFee_ApproveUS += billing.InstallFee_ApprovalUsd;
                        }

                        #endregion
                        #region Install Fee Complete Installation

                        if (billing.InstallFee_CompleteInstallation > 0
                            || billing.InstallFee_CompleteInstallationUsd > 0)
                        {
                            if (billing.InstallFee_CompleteInstallationCurrencyType != contract.NormalInstallFeeCurrencyType)
                                isSameInstallFee_CompleteInstallationCurrency = false;

                            if (billing.InstallFee_CompleteInstallation != null)
                                totalInstallFee_CompleteInstallation += billing.InstallFee_CompleteInstallation;
                            if (billing.InstallFee_CompleteInstallationUsd != null)
                                totalInstallFee_CompleteInstallationUS += billing.InstallFee_CompleteInstallationUsd;
                        }

                        #endregion
                        #region Install Fee Start Service

                        if (billing.InstallFee_StartService > 0
                            || billing.InstallFee_StartServiceUsd > 0)
                        {
                            if (billing.InstallFee_StartServiceCurrencyType != contract.NormalInstallFeeCurrencyType)
                                isSameInstallFee_StartServiceCurrency = false;

                            if (billing.InstallFee_StartService != null)
                                totalInstallFee_StartService += billing.InstallFee_StartService;
                            if (billing.InstallFee_StartServiceUsd != null)
                                totalInstallFee_StartServiceUS += billing.InstallFee_StartServiceUsd;
                        }

                        #endregion
                        #region Deposit Fee

                        if (billing.DepositFee > 0
                            || billing.DepositFeeUsd > 0)
                        {
                            if (billing.DepositFeeCurrencyType != contract.NormalDepositFeeCurrencyType)
                                isSameDepositFeeCurrency = false;

                            if (billing.DepositFee != null)
                                totalDepositFee += billing.DepositFee;
                            if (billing.DepositFeeUsd != null)
                                totalDepositFeeUS += billing.DepositFeeUsd;
                        }

                        #endregion
                    }
                }

                #region Expect installation date

                if (contract.ExpectedInstallCompleteDate != null)
                {
                    //if (contract.ExpectedInstallCompleteDate.Value.CompareTo(currentDate) < 0)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3071,
                    //                        null, new string[] { "ExpectedInstallCompleteDate" });
                    //    return false;
                    //}
                    if (contract.ExpectedInstallCompleteDate.Value.CompareTo(future) > 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3072,
                                            null, new string[] { "ExpectedInstallCompleteDate" });
                        return false;
                    }
                }

                #endregion
                #region Expected operation date

                //if (contract.ExpectedStartServiceDate.Value.CompareTo(currentDate) < 0)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3069,
                //                        null, new string[] { "ExpectedStartServiceDate" });
                //    return false;
                //}
                if (contract.ExpectedStartServiceDate.Value.CompareTo(future) > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3070,
                                        null, new string[] { "ExpectedStartServiceDate" });
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
                #region Contract fee

                if (contract.OrderContractFee > CommonValue.C_MAX_MONTHLY_FEE_INPUT
                    || contract.OrderContractFeeUsd > CommonValue.C_MAX_MONTHLY_FEE_INPUT)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3287,
                                                           new string[] { CommonValue.C_MAX_MONTHLY_FEE_INPUT.ToString("N2") },
                                                           new string[] { "OrderContractFee" });
                    return false;
                }

                if (contract.NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    if ((contract.NormalContractFee > 0
                            && (CommonUtil.IsNullOrEmpty(contract.OrderContractFee)
                            || contract.OrderContractFee == 0))
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3003,
                                            null, new string[] { "ApproveNo1" });
                        return false;
                    }

                    if ((contract.NormalContractFee > 0
                            && contract.OrderContractFee > 0)
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        decimal? fee10 = (contract.NormalContractFee * 0.1M);
                        decimal? fee1000 = (contract.NormalContractFee * 10.0M);
                        if (contract.OrderContractFee <= fee10)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3004,
                                                null, new string[] { "ApproveNo1" });
                            return false;
                        }
                        if (contract.OrderContractFee >= fee1000)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3004,
                                                null, new string[] { "ApproveNo1" });
                            return false;
                        }
                    }

                    if (contract.NormalContractFee != contract.OrderContractFee
                        && contract.OrderContractFee > 0
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3005,
                                            null, new string[] { "ApproveNo1" });
                        return false;
                    }

                    if (isSameContractFeeCurrency == true
                        && contract.OrderContractFee != totalContractFee
                        && totalContractFeeUS == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3012,
                                            null, new string[] { "OrderContractFee" });
                        return false;
                    }
                }
                else
                {
                    if ((contract.NormalContractFeeUsd > 0
                                && (CommonUtil.IsNullOrEmpty(contract.OrderContractFeeUsd)
                                || contract.OrderContractFeeUsd == 0))
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3003,
                                            null, new string[] { "ApproveNo1" });
                        return false;
                    }

                    if ((contract.NormalContractFeeUsd > 0
                            && contract.OrderContractFeeUsd > 0)
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        decimal? fee10 = (contract.NormalContractFeeUsd * 0.1M);
                        decimal? fee1000 = (contract.NormalContractFeeUsd * 10.0M);
                        if (contract.OrderContractFeeUsd <= fee10)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3004,
                                                null, new string[] { "ApproveNo1" });
                            return false;
                        }
                        if (contract.OrderContractFeeUsd >= fee1000)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3004,
                                                null, new string[] { "ApproveNo1" });
                            return false;
                        }
                    }

                    if (contract.NormalContractFeeUsd != contract.OrderContractFeeUsd
                        && contract.OrderContractFeeUsd > 0
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3005,
                                            null, new string[] { "ApproveNo1" });
                        return false;
                    }

                    if (isSameContractFeeCurrency == true
                        && contract.OrderContractFeeUsd != totalContractFeeUS
                        && totalContractFee == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3012,
                                            null, new string[] { "OrderContractFee" });
                        return false;
                    }
                }

                #endregion
                #region Installation fee

                decimal ninst = 0;
                decimal oinst = 0;
                decimal oinst_a = 0;
                decimal oinst_c = 0;
                decimal oinst_s = 0;
                bool skpinst = false;

                #region Order Install Fee

                if (contract.OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (CommonUtil.IsNullOrEmpty(contract.NormalInstallFeeUsd) == false)
                        ninst = contract.NormalInstallFeeUsd.Value;
                    if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFeeUsd) == false)
                        oinst = contract.OrderInstallFeeUsd.Value;
                }
                else
                {
                    if (CommonUtil.IsNullOrEmpty(contract.NormalInstallFee) == false)
                        ninst = contract.NormalInstallFee.Value;
                    if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFee) == false)
                        oinst = contract.OrderInstallFee.Value;
                }

                #endregion
                #region Approve Contract

                if (contract.OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_ApproveContractUsd) == false)
                        oinst_a = contract.OrderInstallFee_ApproveContractUsd.Value;
                }
                else
                {
                    if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_ApproveContract) == false)
                        oinst_a = contract.OrderInstallFee_ApproveContract.Value;
                }
                if (oinst_a > 0 && contract.OrderInstallFee_ApproveContractCurrencyType != contract.OrderInstallFeeCurrencyType)
                    skpinst = true;

                #endregion
                #region Complete Install

                if (contract.OrderInstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_CompleteInstallUsd) == false)
                        oinst_c = contract.OrderInstallFee_CompleteInstallUsd.Value;
                }
                else
                {
                    if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_CompleteInstall) == false)
                        oinst_c = contract.OrderInstallFee_CompleteInstall.Value;
                }
                if (oinst_c > 0 && contract.OrderInstallFee_CompleteInstallCurrencyType != contract.OrderInstallFeeCurrencyType)
                    skpinst = true;

                #endregion
                #region Start Service

                if (contract.OrderInstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_StartServiceUsd) == false)
                        oinst_s = contract.OrderInstallFee_StartServiceUsd.Value;
                }
                else
                {
                    if (CommonUtil.IsNullOrEmpty(contract.OrderInstallFee_StartService) == false)
                        oinst_s = contract.OrderInstallFee_StartService.Value;
                }
                if (oinst_s > 0 && contract.OrderInstallFee_StartServiceCurrencyType != contract.OrderInstallFeeCurrencyType)
                    skpinst = true;

                #endregion
                
                if (ninst != oinst
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3007,
                                        null, new string[] { "ApproveNo1" });
                    return false;
                }

                //if (skpinst == false)
                //{
                //    decimal totalInstallFee = oinst_a + oinst_c + oinst_s;
                //    if (oinst != totalInstallFee)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3088,
                //                            null, new string[] { "OrderInstallFee" });
                //        return false;
                //    }                    
                //}

                if (isSameInstallFee_ApprovalCurrency
                    && ((contract.OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                            && totalInstallFee_Approve != oinst_a)
                        || (contract.OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            && totalInstallFee_ApproveUS != oinst_a)))
                    {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3089,
                                    null, new string[] { "OrderInstallFee_ApproveContract" });
                    return false;
                }
                if (isSameInstallFee_CompleteInstallationCurrency
                    && ((contract.OrderInstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                            && totalInstallFee_CompleteInstallation != oinst_c)
                        || (contract.OrderInstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            && totalInstallFee_CompleteInstallationUS != oinst_c)))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3027,
                                        null, new string[] { "OrderInstallFee_CompleteInstall" });
                    return false;
                }
                if (isSameInstallFee_StartServiceCurrency
                    && ((contract.OrderInstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                            && totalInstallFee_StartService != oinst_s)
                        || (contract.OrderInstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            && totalInstallFee_StartServiceUS != oinst_s)))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3028,
                                        null, new string[] { "OrderInstallFee_StartService" });
                    return false;
                }
                
                #endregion
                #region InstallFee_Approval

                for(int i=0; i< billingList.Count; i++)
                {
                    if(billingList[i].InstallFee_ApprovalCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if(billingList[i].InstallFee_ApprovalUsd > 0 
                            && (billingList[i].PaymentMethod_Approval == "2" || billingList[i].PaymentMethod_Approval == "3")
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3007,
                                            null, new string[] { "ApproveNo1" });
                            return false;
                        }
                    }
                    else
                    {
                        if (billingList[i].InstallFee_Approval > 0
                            && (billingList[i].PaymentMethod_Approval == "2" || billingList[i].PaymentMethod_Approval == "3")
                            && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3007,
                                            null, new string[] { "ApproveNo1" });
                            return false;
                        }
                    }
                }
                #endregion
                #region Deposit fee

                decimal ndf = 0;
                decimal odf = 0;
                if (contract.NormalDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (CommonUtil.IsNullOrEmpty(contract.NormalDepositFeeUsd))
                        contract.NormalDepositFeeUsd = 0;
                    if (CommonUtil.IsNullOrEmpty(contract.OrderDepositFeeUsd))
                        contract.OrderDepositFeeUsd = 0;

                    ndf = contract.NormalDepositFeeUsd.Value;
                    odf = contract.OrderDepositFeeUsd.Value;
                }
                else
                {
                    if (CommonUtil.IsNullOrEmpty(contract.NormalDepositFee))
                        contract.NormalDepositFee = 0;
                    if (CommonUtil.IsNullOrEmpty(contract.OrderDepositFee))
                        contract.OrderDepositFee = 0;

                    ndf = contract.NormalDepositFee.Value;
                    odf = contract.OrderDepositFee.Value;
                }

                if (ndf != odf
                    && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3008,
                                        null, new string[] { "ApproveNo1" });
                    return false;
                }

                if (isSameDepositFeeCurrency
                    && ((contract.OrderDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                            && contract.OrderDepositFee != totalDepositFee)
                        || (contract.OrderDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            && contract.OrderDepositFeeUsd != totalDepositFeeUS)))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3014,
                                        null, new string[] { "OrderDepositFee" });
                    return false;
                }

                if (contract.OrderDepositFee > 0
                        || contract.OrderDepositFeeUsd > 0)
                {
                    if (CommonUtil.IsNullOrEmpty(contract.BillingTimingDepositFee))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3090,
                                            null, new string[] { "BillingTimingDepositFee" });
                        return false;
                    }
                }
                else
                {
                    if (CommonUtil.IsNullOrEmpty(contract.BillingTimingDepositFee) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3091,
                                            null, new string[] { "BillingTimingDepositFee" });
                        return false;
                    }
                }
                
                #endregion
                #region Contract code for deposit slide

                if (isContractCodeForDepositFeeSlide
                    && CommonUtil.IsNullOrEmpty(contract.CounterBalanceOriginContractCode))
                {
                    res.AddErrorMessage(
                        MessageUtil.MODULE_CONTRACT,
                        MessageUtil.MessageList.MSG3092,
                        null,
                        new string[] { "CounterBalanceOriginContractCode" });
                    return false;
                }
                if (CommonUtil.IsNullOrEmpty(contract.CounterBalanceOriginContractCode) == false)
                {
                    CommonUtil cmm = new CommonUtil();
                    if (contract.CounterBalanceOriginContractCode.StartsWith("Q")
                        || contract.CounterBalanceOriginContractCode.StartsWith("q"))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3254,
                            new string[] { cmm.ConvertContractCode(contract.CounterBalanceOriginContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT) },
                            new string[] { "CounterBalanceOriginContractCode" });
                        return false;
                    }

                    IRentralContractHandler rhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                    List<tbt_RentalContractBasic> lst =
                        rhandler.GetTbt_RentalContractBasic(contract.CounterBalanceOriginContractCode, null);
                    if (lst.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0092,
                            new string[] { contract.CounterBalanceOriginContractCode },
                            new string[] { "CounterBalanceOriginContractCode" });
                        return false;
                    }
                }

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
                #region Line-up type

                if (draft.doTbt_DraftRentalInstrument != null)
                {
                    foreach (tbt_DraftRentalInstrument inst in draft.doTbt_DraftRentalInstrument)
                    {
                        if (inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3295);
                            return false;
                        }
                    }
                }

                #endregion
                #region Maintenance target contract

                if (draft.doTbt_DraftRentalContrat.ProductTypeCode != ProductType.C_PROD_TYPE_ONLINE
                    && draft.doTbt_RelationType != null)
                {
                    string contractLst = string.Empty;
                    foreach (tbt_RelationType rel in draft.doTbt_RelationType)
                    {
                        if (rel.ContractCode != null)
                        {
                            if (contractLst != string.Empty)
                                contractLst += ",";
                            contractLst += string.Format("\'{0}\'", rel.RelatedContractCode);
                        }
                    }

                    IContractHandler cohandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                    List<dsGetSiteContractList> list = cohandler.GetSiteContractList(contractLst, FlagType.C_FLAG_ON);
                    if (list != null && list.Count > 0)
                    {
                        bool isSameSite = true;
                        string siteCode = list[0].SiteCode;
                        foreach (dsGetSiteContractList site in list)
                        {
                            if (site.SiteCode != siteCode)
                            {
                                isSameSite = false;
                                break;
                            }
                        }
                        if (draft.doSite != null)
                        {
                            if (siteCode != draft.doSite.SiteCode)
                                isSameSite = false;
                        }
                        if (isSameSite == false)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3248);
                            return false;
                        }

                    }
                }

                #endregion
                #region Out of regulations about contract duration

                if (contract.IrregulationContractDurationFlag == "1")
                {
                    if (CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            MessageUtil.MessageList.MSG3079,
                            null,
                            new string[] { "ApproveNo1" });
                        return false;
                    }

                    if (CommonUtil.IsNullOrEmpty(contract.ContractDurationMonth) == false
                        && CommonUtil.IsNullOrEmpty(contract.AutoRenewMonth)
                        && CommonUtil.IsNullOrEmpty(contract.ContractEndDate))
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            MessageUtil.MessageList.MSG3034,
                            null,
                            new string[] { "AutoRenewMonth" });
                        return false;
                    }
                    if (CommonUtil.IsNullOrEmpty(contract.ContractDurationMonth) == false
                        && CommonUtil.IsNullOrEmpty(contract.ContractEndDate) == false)
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            MessageUtil.MessageList.MSG3033,
                            null,
                            new string[] { "AutoRenewMonth", "ContractEndDate" });
                        return false;
                    }
                    if (CommonUtil.IsNullOrEmpty(contract.ContractDurationMonth)
                        && CommonUtil.IsNullOrEmpty(contract.ContractEndDate))
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            MessageUtil.MessageList.MSG3035,
                            null,
                            new string[] { "ContractDurationMonth", "ContractEndDate" });
                        return false;
                    }
                    if (CommonUtil.IsNullOrEmpty(contract.ContractEndDate) == false)
                    {
                        if (contract.ContractEndDate.Value.CompareTo(currentDate) < 0)
                        {
                            res.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                MessageUtil.MessageList.MSG3080,
                                null,
                                new string[] { "ContractEndDate" });
                            return false;
                        }
                        if (contract.ContractEndDate.Value.CompareTo(contract.ExpectedStartServiceDate.Value) < 0)
                        {
                            res.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                MessageUtil.MessageList.MSG3129,
                                null,
                                new string[] { "ContractEndDate" });
                            return false;
                        }
                    }
                }
                if (CommonUtil.IsNullOrEmpty(contract.ContractDurationMonth) == false)
                {
                    if (contract.ContractDurationMonth < 36
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            MessageUtil.MessageList.MSG3081,
                            null,
                            new string[] { "ApproveNo1" });
                        return false;
                    }
                }
                if (CommonUtil.IsNullOrEmpty(contract.AutoRenewMonth) == false)
                {
                    if (contract.AutoRenewMonth < 12
                        && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            MessageUtil.MessageList.MSG3082,
                            null,
                            new string[] { "ApproveNo1" });
                        return false;
                    }
                }

                #endregion
                #region Billing cycle

                bool isBillingError = true;
                if (CommonUtil.IsNullOrEmpty(contract.BillingCycle) == false)
                {
                    if (contract.BillingCycle >= 3)
                        isBillingError = false;
                }
                if (isBillingError && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3251,
                            null, new string[] { "ApproveNo1" });
                    return false;
                }

                #endregion
                #region Contract payment term

                if (CommonUtil.IsNullOrEmpty(contract.PayMethod) == false)
                {
                    if (contract.PayMethod != PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                         && contract.PayMethod != PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                         && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3064);
                        return false;
                    }
                }
                if (CommonUtil.IsNullOrEmpty(contract.CreditTerm) == false)
                {
                    if (contract.CreditTerm > 0
                    && CommonUtil.IsNullOrEmpty(contract.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3255,
                            null, new string[] { "ApproveNo1" });
                        return false;
                    }
                }

                #endregion
                #region Salesman

                object template = CommonUtil.CloneObject<tbt_DraftRentalContract, CTS010_RegisterRentalContracrData_Employee>(contract);
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
                    contract.SalesSupporterEmpNo
                };
                List<string> ctrslLst = new List<string>();
                for (int sidx = 0; sidx < salesmanLst.Length; sidx++)
                {
                    if (CommonUtil.IsNullOrEmpty(salesmanLst[sidx]))
                        continue;

                    for (int ssidx = sidx + 1; ssidx < salesmanLst.Length; ssidx++)
                    {
                        if (salesmanLst[sidx] == salesmanLst[ssidx])
                        {
                            string ctrl = "SalesmanEmpNo" + (ssidx + 1).ToString();
                            if (ssidx == salesmanLst.Length - 1)
                                ctrl = "SalesSupporterEmpNo";
                            ctrslLst.Add(ctrl);
                        }
                    }
                    if (ctrslLst.Count > 0)
                    {
                        string ctrl = "SalesmanEmpNo" + (sidx + 1).ToString();
                        ctrslLst.Insert(0, ctrl);
                        break;
                    }
                }
                if (ctrslLst.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3247,
                            null, ctrslLst.ToArray());
                    return false;
                }

                #endregion

                ICustomerMasterHandler chandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                #region Check Existing Contract customer

                CTS010_RegisterContractCutomer cust1v = CommonUtil.CloneObject<doCustomerWithGroup, CTS010_RegisterContractCutomer>(draft.doContractCustomer);

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
                        CTS010_ScreenParameter param = CTS010_ScreenData;
                        param.doDraftRentalContractData.doContractCustomer.ValidateCustomerData = false;
                        CTS010_ScreenData = param;

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
                        CTS010_ScreenParameter param = CTS010_ScreenData;
                        param.doDraftRentalContractData.doRealCustomer.ValidateCustomerData = false;
                        CTS010_ScreenData = param;

                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3257);
                        return false;
                    }
                }

                #endregion
                #region Check Duplicate Tax ID

                if (draft.doContractCustomer != null
                    && draft.doRealCustomer != null)
                {
                    if (CTS010_IsSameCustomer(draft.doContractCustomer, draft.doRealCustomer) == false)
                    {
                        if (CommonUtil.IsNullOrEmpty(draft.doContractCustomer.IDNo) == false
                            && CommonUtil.IsNullOrEmpty(draft.doRealCustomer.IDNo) == false
                            && draft.doContractCustomer.IDNo == draft.doRealCustomer.IDNo)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3228);
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

                ISiteMasterHandler shandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                if (hasSiteCode)
                {
                    if (shandler.CheckExistSiteData(draft.doSite.SiteCode,
                                                        draft.doRealCustomer.CustCode) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2036, null, new string[] { "SiteCustCodeNo" });
                        return false;
                    }
                }
                else
                {
                    doSite s = CommonUtil.CloneObject<doSite, doSite>(draft.doSite);
                    shandler.ValidateSiteData(s);
                    if (s != null)
                    {
                        if (s.ValidateSiteData == false)
                        {
                            CTS010_ScreenParameter param = CTS010_ScreenData;
                            if (param.doDraftRentalContractData.doSite != null) //Add by Jutarat A. on 18102012
                                param.doDraftRentalContractData.doSite.ValidateSiteData = false;

                            CTS010_ScreenData = param;

                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3258);
                            return false;
                        }
                    }
                }

                #endregion
                #region Check billing is edited

                if (isBillingEditMode == true)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3253);
                    return false;
                }

                #endregion
                #region Billing target dividend

                int isContractFeeError1 = (contract.DivideContractFeeBillingFlag == true) ? 0 : -1;
                int isContractFeeError2 = (contract.DivideContractFeeBillingFlag == true) ? -1 : 0;
                if (billingList != null)
                {
                    foreach (CTS010_BillingTargetData billing in billingList)
                    {
                        if (billing.ContractFee > 0
                            || billing.ContractFeeUsd > 0)
                        {
                            if (contract.DivideContractFeeBillingFlag == true)
                            {
                                if (isContractFeeError1 >= 0)
                                    isContractFeeError1++;
                            }
                            else
                            {
                                if (isContractFeeError2 >= 0)
                                    isContractFeeError2++;
                            }
                        }
                    }
                }

                if (isContractFeeError1 >= 0 && isContractFeeError1 < 2)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3010);
                    return false;
                }
                if (isContractFeeError2 > 1)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3011);
                    return false;
                }

                #endregion

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Mapping Billing target data
        /// </summary>
        /// <param name="draftBillingList"></param>
        /// <returns></returns>
        private List<CTS010_BillingTargetData> CTS010_SetBillingList(List<tbt_DraftRentalBillingTarget> draftBillingList)
        {


            #region Comment
            try
            {
                List<CTS010_BillingTargetData> billingLst = new List<CTS010_BillingTargetData>();
                if (draftBillingList != null)
                {
                    IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                    List<doFunctionBilling> clst = handler.GetFunctionBilling();

                    IBillingMasterHandler bhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                    foreach (tbt_DraftRentalBillingTarget db in draftBillingList)
                    {
                        /* --- Check Is Exist --- */
                        CTS010_BillingTargetData billing = null;
                        foreach (CTS010_BillingTargetData b in billingLst)
                        {
                            if (b.IsBillingTarget(db.BillingClientCode, db.BillingOfficeCode))
                            {
                                billing = b;
                                break;
                            }
                        }
                        if (billing == null)
                        {
                            billing = new CTS010_BillingTargetData()
                            {
                                QuotationTargetCode = db.QuotationTargetCode,
                                BillingClientCode = db.BillingClientCode,
                                BillingOfficeCode = db.BillingOfficeCode,
                                DocLanguage = db.DocLanguage, //Add by Jutarat A. on 18122013
                                BillingTargetCode = db.BillingTargetCode,
                                BillingCycle = db.BillingCycle,
                                CalDailyFeeStatus = db.CalDailyFeeStatus
                            };
                            billingLst.Add(billing);

                            ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                            billing.Currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                            if (CommonUtil.IsNullOrEmpty(billing.BillingClientCode) == false)
                            {
                                List<dtBillingClientData> bc = bhandler.GetBillingClient(billing.BillingClientCode);
                                if (bc.Count > 0)
                                    billing.BillingClient = CommonUtil.CloneObject<dtBillingClientData, tbm_BillingClient>(bc[0]);
                            }
                            if (CommonUtil.IsNullOrEmpty(billing.BillingOfficeCode) == false)
                            {
                                if (clst.Count > 0)
                                {
                                    foreach (doFunctionBilling fb in clst)
                                    {
                                        if (fb.OfficeCode == billing.BillingOfficeCode)
                                        {
                                            billing.BillingOfficeName = fb.OfficeName;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (db.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
                            || db.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON
                            || db.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE)
                        {
                            billing.ContractFeeCurrencyType = db.BillingAmtCurrencyType;
                            billing.ContractFee = db.BillingAmt;
                            billing.ContractFeeUsd = db.BillingAmtUsd;
                        }
                        else if (db.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                        {
                            billing.DepositFeeCurrencyType = db.BillingAmtCurrencyType;
                            billing.DepositFee = db.BillingAmt;
                            billing.DepositFeeUsd = db.BillingAmtUsd;
                            
                            billing.PaymentMethod_Deposit = db.PayMethod;
                        }
                        else if (db.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE)
                        {
                            if (db.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                            {
                                billing.InstallFee_ApprovalCurrencyType = db.BillingAmtCurrencyType;
                                billing.InstallFee_Approval = db.BillingAmt;
                                billing.InstallFee_ApprovalUsd = db.BillingAmtUsd;

                                billing.PaymentMethod_Approval = db.PayMethod;
                            }
                            else if (db.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION)
                            {
                                billing.InstallFee_CompleteInstallationCurrencyType = db.BillingAmtCurrencyType;
                                billing.InstallFee_CompleteInstallation = db.BillingAmt;
                                billing.InstallFee_CompleteInstallationUsd = db.BillingAmtUsd;

                                billing.PaymentMethod_CompleteInstallation = db.PayMethod;
                            }
                            else if (db.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE)
                            {
                                billing.InstallFee_StartServiceCurrencyType = db.BillingAmtCurrencyType;
                                billing.InstallFee_StartService = db.BillingAmt;
                                billing.InstallFee_StartServiceUsd = db.BillingAmtUsd;

                                billing.PaymentMethod_StartService = db.PayMethod;
                            }
                        }
                    }
                }

                return billingLst;
            }
            catch (Exception)
            {
                throw;
            }
            #endregion
        }
        /// <summary>
        /// Method for approve contract
        /// </summary>
        /// <param name="draft"></param>
        private void CTS010_ApproveContract(doDraftRentalContractData draft)
        {
            try
            {
                //CheckTimeLog("Start update draft rental contract data");

                IContractHandler cthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                IRentralContractHandler rcthandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                IDraftRentalContractHandler rhandler = ServiceContainer.GetService<IDraftRentalContractHandler>() as IDraftRentalContractHandler;
                ICommonHandler cmhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                #region Generate Contract code

                string ContractCode = cthandler.GenerateContractCode(draft.doTbt_DraftRentalContrat.ProductTypeCode);

                #endregion
                #region Generate Security occurrence

                string OCC = rcthandler.GenerateContractOCC(ContractCode, false);

                #endregion
                #region Update Draft contract

                draft.doTbt_DraftRentalContrat.ContractCode = ContractCode;
                rhandler.EditDraftRentalContractDataForCTS010(draft);

                #endregion

                //CheckTimeLog("Finish update draft rental contract data");

                //CheckTimeLog("Start insert rental contract data");

                dsRentalContractData register = new dsRentalContractData();

                #region Rental contract basic

                tbt_RentalContractBasic basic = CommonUtil.CloneObject<tbt_DraftRentalContract, tbt_RentalContractBasic>(draft.doTbt_DraftRentalContrat);
                if (basic != null)
                {
                    basic.QuotationNo = draft.QuotationNo;
                    basic.ContractCode = ContractCode;
                    basic.LastOCC = OCC;
                    basic.LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_APPROVE;
                    basic.ContractStatus = ContractStatus.C_CONTRACT_STATUS_BEF_START;
                    basic.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;

                    List<tbs_ProductType> lst = cmhandler.GetTbs_ProductType(ServiceType.C_SERVICE_TYPE_RENTAL, draft.doTbt_DraftRentalContrat.ProductTypeCode);
                    if (lst.Count > 0)
                        basic.PrefixCode = lst[0].ContractPrefix;

                    basic.FirstInstallCompleteFlag = FlagType.C_FLAG_OFF;
                    basic.LastChangeImplementDate = draft.doTbt_DraftRentalContrat.ExpectedStartServiceDate;
                    basic.ContractConditionProcessDate = draft.doTbt_DraftRentalContrat.CreateDate;

                    #region Normal Deposit Fee

                    basic.NormalDepositFeeCurrencyType = draft.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType;
                    basic.NormalDepositFee = draft.doTbt_DraftRentalContrat.NormalDepositFee;
                    basic.NormalDepositFeeUsd = draft.doTbt_DraftRentalContrat.NormalDepositFeeUsd;

                    #endregion
                    #region Order Deposit Fee

                    basic.OrderDepositFeeCurrencyType = draft.doTbt_DraftRentalContrat.OrderDepositFeeCurrencyType;
                    basic.OrderDepositFee = draft.doTbt_DraftRentalContrat.OrderDepositFee;
                    basic.OrderDepositFeeUsd = draft.doTbt_DraftRentalContrat.OrderDepositFeeUsd;

                    #endregion
                    #region Exempted Deposit Fee

                    basic.ExemptedDepositFeeCurrencyType = draft.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType;

                    basic.ExemptedDepositFee = 0;                    
                    if (draft.doTbt_DraftRentalContrat.NormalDepositFee != null)
                        basic.ExemptedDepositFee += draft.doTbt_DraftRentalContrat.NormalDepositFee;
                    if (draft.doTbt_DraftRentalContrat.OrderDepositFee != null)
                        basic.ExemptedDepositFee -= draft.doTbt_DraftRentalContrat.OrderDepositFee;

                    basic.ExemptedDepositFeeUsd = 0;
                    if (draft.doTbt_DraftRentalContrat.NormalDepositFeeUsd != null)
                        basic.ExemptedDepositFeeUsd += draft.doTbt_DraftRentalContrat.NormalDepositFeeUsd;
                    if (draft.doTbt_DraftRentalContrat.OrderDepositFeeUsd != null)
                        basic.ExemptedDepositFeeUsd -= draft.doTbt_DraftRentalContrat.OrderDepositFeeUsd;

                    #endregion
                    #region Paid Deposit Fee

                    basic.PaidDepositFeeCurrencyType = draft.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType;
                    basic.PaidDepositFee = 0;
                    basic.PaidDepositFeeUsd = 0;

                    #endregion
                    #region Bill Deposit Fee

                    basic.BilledDepositFeeCurrencyType = draft.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType;
                    basic.BilledDepositFee = 0;
                    basic.BilledDepositFeeUsd = 0;

                    #endregion
                    #region Fine

                    basic.FineCurrencyType = draft.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType;
                    basic.Fine = 0;
                    basic.FineUsd = 0;

                    #endregion
                    #region  Revenue Order Received Fine

                    basic.RevenueOverReceivedFineCurrencyType = draft.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType;
                    basic.RevenueOverReceivedFine = 0;
                    basic.RevenueOverReceivedFineUsd = 0;

                    #endregion

                    basic.IrregurationDocUsageFlag = FlagType.C_FLAG_OFF;

                    basic.IrregulationDurationFlag = false;
                    if (draft.doTbt_DraftRentalContrat.IrregulationContractDurationFlag == "1")
                        basic.IrregulationDurationFlag = true;

                    basic.QuotationStaffEmpNo = draft.doTbt_DraftRentalContrat.QuotationStaffEmpNo;

                    #region Last Normal Contract Fee

                    basic.LastNormalContractFeeCurrencyType = draft.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType;
                    basic.LastNormalContractFee = draft.doTbt_DraftRentalContrat.NormalContractFee;
                    basic.LastNormalContractFeeUsd = draft.doTbt_DraftRentalContrat.NormalContractFeeUsd;

                    #endregion
                    #region Last Order Contract Fee

                    basic.LastOrderContractFeeCurrencyType = draft.doTbt_DraftRentalContrat.OrderContractFeeCurrencyType;
                    basic.LastOrderContractFee = draft.doTbt_DraftRentalContrat.OrderContractFee;
                    basic.LastOrderContractFeeUsd = draft.doTbt_DraftRentalContrat.OrderContractFeeUsd;

                    #endregion

                    IProductMasterHandler phandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    List<tbm_Product> pLst = phandler.GetTbm_Product(draft.doTbt_DraftRentalContrat.ProductCode, null);
                    if (pLst.Count > 0)
                    {
                        basic.DepreciationPeriodContract = pLst[0].DepreciationPeriodContract;
                        basic.DepreciationPeriodRevenue = pLst[0].DepreciationPeriodRevenue;
                    }
                }
                register.dtTbt_RentalContractBasic = new List<tbt_RentalContractBasic>() { basic };


                #endregion
                #region Rental security basic

                tbt_RentalSecurityBasic secBasic = CommonUtil.CloneObject<tbt_DraftRentalContract, tbt_RentalSecurityBasic>(draft.doTbt_DraftRentalContrat);
                if (secBasic != null)
                {
                    secBasic.ContractCode = ContractCode;
                    secBasic.OCC = OCC;
                    secBasic.ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_APPROVE;
                    secBasic.ImplementFlag = FlagType.C_FLAG_OFF;

                    #region Contract Fee On Stop

                    secBasic.ContractFeeOnStopCurrencyType = draft.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType;
                    secBasic.ContractFeeOnStop = null;
                    secBasic.ContractFeeOnStopUsd = null;

                    #endregion
                    #region NewBldMgmtCost

                    secBasic.NewBldMgmtCostCurrencyType = draft.doTbt_DraftRentalContrat.NewBldMgmtCostCurrencyType;
                    secBasic.NewBldMgmtCost = draft.doTbt_DraftRentalContrat.NewBldMgmtCost;
                    secBasic.NewBldMgmtCostUsd = draft.doTbt_DraftRentalContrat.NewBldMgmtCostUsd;

                    #endregion
                    #region Normal Contract Fee

                    secBasic.NormalContractFeeCurrencyType = draft.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType;
                    secBasic.NormalContractFee = draft.doTbt_DraftRentalContrat.NormalContractFee;
                    secBasic.NormalContractFeeUsd = draft.doTbt_DraftRentalContrat.NormalContractFeeUsd;

                    #endregion
                    #region Order Contract Fee

                    secBasic.OrderContractFeeCurrencyType = draft.doTbt_DraftRentalContrat.OrderContractFeeCurrencyType;
                    secBasic.OrderContractFee = draft.doTbt_DraftRentalContrat.OrderContractFee;
                    secBasic.OrderContractFeeUsd = draft.doTbt_DraftRentalContrat.OrderContractFeeUsd;

                    #endregion
                    #region Normal Additional Deposit Fee

                    secBasic.NormalAdditionalDepositFeeCurrencyType = draft.doTbt_DraftRentalContrat.NormalDepositFeeCurrencyType;
                    secBasic.NormalAdditionalDepositFee = draft.doTbt_DraftRentalContrat.NormalDepositFee;
                    secBasic.NormalAdditionalDepositFeeUsd = draft.doTbt_DraftRentalContrat.NormalDepositFeeUsd;

                    #endregion
                    #region Order Additional Deposit Fee

                    secBasic.OrderAdditionalDepositFeeCurrencyType = draft.doTbt_DraftRentalContrat.OrderDepositFeeCurrencyType;
                    secBasic.OrderAdditionalDepositFee = draft.doTbt_DraftRentalContrat.OrderDepositFee;
                    secBasic.OrderAdditionalDepositFeeUsd = draft.doTbt_DraftRentalContrat.OrderDepositFeeUsd;

                    #endregion
                    #region Insurance Coverage Amount

                    secBasic.InsuranceCoverageAmountCurrencyType = draft.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType;
                    secBasic.InsuranceCoverageAmount = null;
                    secBasic.InsuranceCoverageAmountUsd = null;

                    #endregion
                    #region Monthly Insurance Fee

                    secBasic.MonthlyInsuranceFeeCurrencyType = draft.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType;
                    secBasic.MonthlyInsuranceFee = null;
                    secBasic.MonthlyInsuranceFeeUsd = null;

                    #endregion
                    #region Order Install Fee

                    secBasic.OrderInstallFeeCurrencyType = draft.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType;
                    secBasic.OrderInstallFee = draft.doTbt_DraftRentalContrat.OrderInstallFee;
                    secBasic.OrderInstallFeeUsd = draft.doTbt_DraftRentalContrat.OrderInstallFeeUsd;

                    #endregion
                    #region Order Install Fee for Approve Contract

                    secBasic.OrderInstallFee_ApproveContractCurrencyType = draft.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType;
                    secBasic.OrderInstallFee_ApproveContract = draft.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContract;
                    secBasic.OrderInstallFee_ApproveContractUsd = draft.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractUsd;

                    #endregion
                    #region Order Install Fee for Complete Install

                    secBasic.OrderInstallFee_CompleteInstallCurrencyType = draft.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType;
                    secBasic.OrderInstallFee_CompleteInstall = draft.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstall;
                    secBasic.OrderInstallFee_CompleteInstallUsd = draft.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallUsd;

                    #endregion
                    #region Order Install Fee for Start Service

                    secBasic.OrderInstallFee_StartServiceCurrencyType = draft.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType;
                    secBasic.OrderInstallFee_StartService = draft.doTbt_DraftRentalContrat.OrderInstallFee_StartService;
                    secBasic.OrderInstallFee_StartServiceUsd = draft.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceUsd;

                    #endregion
                    #region Install Fee Paid by SECOM

                    secBasic.InstallFeePaidBySECOMCurrencyType = draft.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType;
                    secBasic.InstallFeePaidBySECOM = null;
                    secBasic.InstallFeePaidBySECOMUsd = null;

                    #endregion
                    #region Additional Fee 1

                    secBasic.AdditionalFee1CurrencyType = draft.doTbt_DraftRentalContrat.AdditionalFee1CurrencyType;
                    secBasic.AdditionalFee1 = draft.doTbt_DraftRentalContrat.AdditionalFee1;
                    secBasic.AdditionalFee1Usd = draft.doTbt_DraftRentalContrat.AdditionalFee1Usd;

                    #endregion
                    #region Additional Fee 2

                    secBasic.AdditionalFee2CurrencyType = draft.doTbt_DraftRentalContrat.AdditionalFee2CurrencyType;
                    secBasic.AdditionalFee2 = draft.doTbt_DraftRentalContrat.AdditionalFee2;
                    secBasic.AdditionalFee2Usd = draft.doTbt_DraftRentalContrat.AdditionalFee2Usd;

                    #endregion
                    #region Additional Fee 3

                    secBasic.AdditionalFee3CurrencyType = draft.doTbt_DraftRentalContrat.AdditionalFee3CurrencyType;
                    secBasic.AdditionalFee3 = draft.doTbt_DraftRentalContrat.AdditionalFee3;
                    secBasic.AdditionalFee3Usd = draft.doTbt_DraftRentalContrat.AdditionalFee3Usd;

                    #endregion
                    #region Maintenance Fee 1

                    secBasic.MaintenanceFee1CurrencyType = draft.doTbt_DraftRentalContrat.MaintenanceFee1CurrencyType;
                    secBasic.MaintenanceFee1 = draft.doTbt_DraftRentalContrat.MaintenanceFee1;
                    secBasic.MaintenanceFee1Usd = draft.doTbt_DraftRentalContrat.MaintenanceFee1Usd;

                    #endregion
                    #region Maintenance Fee 2

                    secBasic.MaintenanceFee2CurrencyType = draft.doTbt_DraftRentalContrat.MaintenanceFee2CurrencyType;
                    secBasic.MaintenanceFee2 = draft.doTbt_DraftRentalContrat.MaintenanceFee2;
                    secBasic.MaintenanceFee2Usd = draft.doTbt_DraftRentalContrat.MaintenanceFee2Usd;

                    #endregion

                    secBasic.DepositFeeBillingTiming = draft.doTbt_DraftRentalContrat.BillingTimingDepositFee;
                    secBasic.CounterNo = 0;
                    secBasic.ContractDocPrintFlag = FlagType.C_FLAG_OFF;
                    secBasic.InstallationCompleteFlag = null;

                    if (basic.ProductTypeCode == ProductType.C_PROD_TYPE_AL || basic.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                        secBasic.InstallationCompleteFlag = FlagType.C_FLAG_OFF;

                    secBasic.InstallationTypeCode = null;
                    secBasic.CalIndex = 0;
                    secBasic.InstallFeePaidBySECOM = null;
                    secBasic.InstallFeeRevenueBySECOM = null;
                    secBasic.SecurityMemo = draft.doTbt_DraftRentalContrat.Memo;

                    secBasic.QuotationAlphabet = draft.doTbt_DraftRentalContrat.Alphabet;
                    secBasic.ExpectedOperationDate = draft.doTbt_DraftRentalContrat.ExpectedStartServiceDate;
                    secBasic.ContractStartDate = null;
                    secBasic.ContractEndDate = draft.doTbt_DraftRentalContrat.ContractEndDate;
                    secBasic.ChangeImplementDate = draft.doTbt_DraftRentalContrat.ExpectedStartServiceDate;
                    secBasic.OrderContractFeePayMethod = draft.doTbt_DraftRentalContrat.PayMethod;
                    secBasic.ExpectedInstallationCompleteDate = draft.doTbt_DraftRentalContrat.ExpectedInstallCompleteDate;
                }
                register.dtTbt_RentalSecurityBasic = new List<tbt_RentalSecurityBasic>() { secBasic };

                #endregion
                #region Rental BE details

                tbt_RentalBEDetails beDetails = CommonUtil.CloneObject<tbt_DraftRentalBEDetails, tbt_RentalBEDetails>(draft.doTbt_DraftRentalBEDetails);
                if (beDetails != null)
                {
                    beDetails.ContractCode = ContractCode;
                    beDetails.OCC = OCC;
                }
                register.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>() { beDetails };

                #endregion
                #region Rental Instrument details

                register.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                if (draft.doTbt_DraftRentalInstrument != null)
                {
                    foreach (tbt_DraftRentalInstrument inst in draft.doTbt_DraftRentalInstrument)
                    {
                        tbt_RentalInstrumentDetails rinst = CommonUtil.CloneObject<tbt_DraftRentalInstrument, tbt_RentalInstrumentDetails>(inst);

                        rinst.ContractCode = ContractCode;
                        rinst.OCC = OCC;
                        rinst.AdditionalInstrumentQty = 0;
                        rinst.RemovalInstrumentQty = 0;

                        register.dtTbt_RentalInstrumentDetails.Add(rinst);
                    }
                }

                #endregion
                #region Rental Maintenance details

                tbt_RentalMaintenanceDetails maDetails =
                    CommonUtil.CloneObject<tbt_DraftRentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(draft.doTbt_DraftRentalMaintenanceDetails);
                if (maDetails != null)
                {
                    maDetails.ContractCode = ContractCode;
                    maDetails.OCC = OCC;
                }
                register.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>() { maDetails };

                #endregion
                #region Rental Operation type

                register.dtTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                if (draft.doTbt_DraftRentalOperationType != null)
                {
                    foreach (tbt_DraftRentalOperationType opt in draft.doTbt_DraftRentalOperationType)
                    {
                        register.dtTbt_RentalOperationType.Add(new tbt_RentalOperationType()
                        {
                            ContractCode = ContractCode,
                            OCC = OCC,
                            OperationTypeCode = opt.OperationTypeCode
                        });
                    }
                }

                #endregion
                #region Rental Sentry guard

                tbt_RentalSentryGuard sentryGuard = CommonUtil.CloneObject<tbt_DraftRentalSentryGuard, tbt_RentalSentryGuard>(draft.doTbt_DraftRentalSentryGuard);
                if (sentryGuard != null)
                {
                    sentryGuard.ContractCode = ContractCode;
                    sentryGuard.OCC = OCC;
                }
                register.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>() { sentryGuard };

                #endregion
                #region Rental Sentry guard details

                register.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                if (draft.doTbt_DraftRentalSentryGuardDetails != null)
                {
                    foreach (tbt_DraftRentalSentryGuardDetails sgd in draft.doTbt_DraftRentalSentryGuardDetails)
                    {
                        tbt_RentalSentryGuardDetails rsgd = CommonUtil.CloneObject<tbt_DraftRentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(sgd);

                        rsgd.ContractCode = ContractCode;
                        rsgd.OCC = OCC;

                        register.dtTbt_RentalSentryGuardDetails.Add(rsgd);
                    }
                }

                #endregion
                #region Relation type

                register.dtTbt_RelationType = new List<tbt_RelationType>();
                if (draft.doTbt_RelationType != null)
                {
                    foreach (tbt_RelationType rel in draft.doTbt_RelationType)
                    {
                        tbt_RelationType rrel = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(rel);

                        rrel.ContractCode = ContractCode;
                        rrel.OCC = OCC;

                        register.dtTbt_RelationType.Add(rrel);
                    }
                }

                #endregion

                #region Update Rental Contract

                rcthandler.InsertEntireContractForCTS010(register);

                #endregion

                //CheckTimeLog("Finish insert rental contract data");

                //CheckTimeLog("Start lock quotation data");

                #region Lock Quotation Data

                IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                qhandler.LockQuotation(draft.doTbt_DraftRentalContrat.QuotationTargetCode,
                                        draft.doTbt_DraftRentalContrat.Alphabet,
                                        LockStyle.C_LOCK_STYLE_ALL);

                #endregion

                //CheckTimeLog("Finish lock quotation data");
                //CheckTimeLog("Start update document data");

                #region Update Document data

                IContractDocumentHandler cdchandler = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;

                List<tbt_ContractDocument> docLst = cdchandler.GetContractDocHeaderByQuotationCode(
                    draft.doTbt_DraftRentalContrat.QuotationTargetCode,
                    draft.doTbt_DraftRentalContrat.Alphabet,
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
                    doc.QuotationTargetCode = draft.doTbt_DraftRentalContrat.QuotationTargetCode;
                    doc.Alphabet = draft.doTbt_DraftRentalContrat.Alphabet;
                    doc.ContractCode = ContractCode;
                    doc.OCC = ParticularOCC.C_PARTICULAR_OCC_CONTRACT_REP_BEF_START;
                    doc.ContractDocOCC = DocOCC;
                    doc.DocNo = CommonUtil.TextList(new string[]{   ContractCode, 
                                                                    ParticularOCC.C_PARTICULAR_OCC_CONTRACT_REP_BEF_START,
                                                                    DocOCC }, "-");
                    doc.DocumentCode = DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN;
                    doc.ContractTargetCustCode = draft.doTbt_DraftRentalContrat.ContractTargetCustCode;
                    doc.RealCustomerCustCode = draft.doTbt_DraftRentalContrat.RealCustomerCustCode;
                    doc.ContractOfficeCode = draft.doTbt_DraftRentalContrat.ContractOfficeCode;
                    doc.OperationOfficeCode = draft.doTbt_DraftRentalContrat.OperationOfficeCode;
                    doc.PhoneLineTypeCode = draft.doTbt_DraftRentalContrat.PhoneLineTypeCode1;
                    doc.PhoneLineOwnerTypeCode = draft.doTbt_DraftRentalContrat.PhoneLineOwnerTypeCode1;
                    doc.CreditTerm = draft.doTbt_DraftRentalContrat.CreditTerm;
                    doc.PaymentCycle = draft.doTbt_DraftRentalContrat.BillingCycle;
                    doc.FireSecurityFlag = draft.doTbt_DraftRentalContrat.FacilityMonitorFlag;
                    doc.CrimePreventFlag = draft.doTbt_DraftRentalContrat.CrimePreventFlag;
                    doc.EmergencyReportFlag = draft.doTbt_DraftRentalContrat.EmergencyReportFlag;
                    doc.FacilityMonitorFlag = draft.doTbt_DraftRentalContrat.FacilityMonitorFlag;
                    doc.ContractDurationMonth = draft.doTbt_DraftRentalContrat.ContractDurationMonth;

                    doc.ContractTargetNameEN = draft.doContractCustomer.CustFullNameEN;
                    doc.ContractTargetNameLC = draft.doContractCustomer.CustFullNameLC;
                    doc.ContractTargetAddressEN = draft.doContractCustomer.AddressFullEN;
                    doc.ContractTargetAddressLC = draft.doContractCustomer.AddressFullLC;
                    doc.RealCustomerNameEN = draft.doRealCustomer.CustFullNameEN;
                    doc.RealCustomerNameLC = draft.doRealCustomer.CustFullNameLC;
                    doc.SiteNameEN = draft.doSite.SiteNameEN;
                    doc.SiteNameLC = draft.doSite.SiteNameLC;
                    doc.SiteAddressEN = draft.doSite.AddressFullEN;
                    doc.SiteAddressLC = draft.doSite.AddressFullLC;
                    
                    

                    IOfficeMasterHandler offhandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                    List<tbm_Office> oLst = offhandler.GetTbm_Office(draft.doTbt_DraftRentalContrat.OperationOfficeCode);
                    if (oLst.Count > 0)
                    {
                        doc.OperationOfficeNameEN = oLst[0].OfficeNameEN;
                        doc.OperationOfficeNameLC = oLst[0].OfficeNameLC;
                    }

                    doc.NegotiationStaffEmpNo = draft.doTbt_DraftRentalContrat.SalesmanEmpNo1;
                    doc.DocStatus = ContractDocStatus.C_CONTRACT_DOC_STATUS_NOT_ISSUED;
                    doc.DocAuditResult = DocAuditResult.C_DOC_AUDIT_RESULT_NOT_RECEIVED;

                    IDocumentHandler dochandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                    List<tbm_DocumentTemplate> dLst = dochandler.GetDocumentTemplateByDocumentCode(DocumentCode.C_DOCUMENT_CODE_CONTRACT_EN); //("CTS010"); //Modify by Jutarat A. on 18122012
                    if (dLst.Count > 0)
                    {
                        doc.SubjectEN = dLst[0].DocumentNameEN;
                        doc.SubjectLC = dLst[0].DocumentNameLC;
                    }

                    doc.QuotationFee = draft.doTbt_DraftRentalContrat.NormalContractFee;

                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    doc.CreateOfficeCode = dsTrans.dtUserData.MainOfficeCode;

                    oLst = offhandler.GetTbm_Office(doc.CreateOfficeCode);
                    if (oLst.Count > 0)
                    {
                        doc.CreateOfficeNameEN = oLst[0].OfficeNameEN;
                        doc.CreateOfficeNameLC = oLst[0].OfficeNameLC;
                    }

                    doc.ProductCode = draft.doTbt_DraftRentalContrat.ProductCode;

                    IProductMasterHandler prodhandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    List<tbm_Product> prodLst = prodhandler.GetTbm_Product(
                        draft.doTbt_DraftRentalContrat.ProductCode,
                        draft.doTbt_DraftRentalContrat.ProductTypeCode);
                    if (prodLst.Count > 0)
                    {
                        doc.ProductNameEN = prodLst[0].ProductNameEN;
                        doc.ProductNameLC = prodLst[0].ProductNameLC;
                    }

                    List<doMiscTypeCode> pmiscLst = new List<doMiscTypeCode>();
                    if (CommonUtil.IsNullOrEmpty(draft.doTbt_DraftRentalContrat.PhoneLineTypeCode1) == false)
                    {
                        pmiscLst.Add(new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_PHONE_LINE_TYPE,
                            ValueCode = draft.doTbt_DraftRentalContrat.PhoneLineTypeCode1
                        });
                    }
                    if (CommonUtil.IsNullOrEmpty(draft.doTbt_DraftRentalContrat.PhoneLineOwnerTypeCode1) == false)
                    {
                        pmiscLst.Add(new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_PHONE_LINE_OWNER_TYPE,
                            ValueCode = draft.doTbt_DraftRentalContrat.PhoneLineOwnerTypeCode1
                        });
                    }

                    ICommonHandler cmmhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    pmiscLst = cmmhandler.GetMiscTypeCodeList(pmiscLst);
                    if (pmiscLst.Count > 0)
                    {
                        foreach (doMiscTypeCode misc in pmiscLst)
                        {
                            if (misc.FieldName == MiscType.C_PHONE_LINE_TYPE && misc.ValueCode == draft.doTbt_DraftRentalContrat.PhoneLineTypeCode1)
                            {
                                doc.PhoneLineTypeNameEN = misc.ValueDisplayEN;
                                doc.PhoneLineTypeNameLC = misc.ValueDisplayLC;
                            }
                            else if (misc.FieldName == MiscType.C_PHONE_LINE_OWNER_TYPE && misc.ValueCode == draft.doTbt_DraftRentalContrat.PhoneLineOwnerTypeCode1)
                            {
                                doc.PhoneLineOwnerTypeNameEN = misc.ValueDisplayEN;
                                doc.PhoneLineOwnerTypeNameLC = misc.ValueDisplayLC;
                            }
                        }
                    }

                    doc.ContractFee = draft.doTbt_DraftRentalContrat.OrderContractFee;
                    doc.DepositFee = draft.doTbt_DraftRentalContrat.OrderDepositFee;
                    doc.ContractFeePayMethod = draft.doTbt_DraftRentalContrat.PayMethod;
                    doc.BusinessTypeCode = draft.doContractCustomer.BusinessTypeCode;
                    doc.BusinessTypeNameEN = draft.doContractCustomer.BusinessTypeNameEN;
                    doc.BusinessTypeNameLC = draft.doContractCustomer.BusinessTypeNameLC;
                    doc.BuildingUsageCode = draft.doSite.BuildingUsageCode;
                    doc.BuildingUsageNameEN = draft.doSite.BuildingUsageNameEN;
                    doc.BuildingUsageNameLC = draft.doSite.BuildingUsageNameLC;
                    doc.OldContractCode = draft.doTbt_DraftRentalContrat.OldContractCode;

                    //Comment by Jutarat A. on 18122012 (Move to CreateContractDocData())
                    //List<tbt_ContractDocument> docResLst = cdchandler.InsertTbt_ContractDocument(new List<tbt_ContractDocument>() { doc });
                    
                    //Add by Narut T 2017-02-10
                    doc.QuotationFeeUsd = draft.doTbt_DraftRentalContrat.NormalContractFeeUsd;
                    doc.QuotationFeeCurrencyType = draft.doTbt_DraftRentalContrat.NormalContractFeeCurrencyType;
                    doc.ContractFeeUsd = draft.doTbt_DraftRentalContrat.OrderContractFeeUsd;
                    doc.ContractFeeCurrencyType = draft.doTbt_DraftRentalContrat.OrderContractFeeCurrencyType;
                    doc.DepositFeeUsd = draft.doTbt_DraftRentalContrat.OrderDepositFeeUsd;
                    doc.DepositFeeCurrencyType = draft.doTbt_DraftRentalContrat.OrderDepositFeeCurrencyType;
                    //End Add by Narut T.
                    #endregion
                    #region Create Document Report Do

                    tbt_DocContractReport docReport = null; //Add by Jutarat A. on 18122012
                    if (doc != null) //if (docResLst.Count > 0) //Modify by Jutarat A. on 18122012
                    {
                        List<doMiscTypeCode> miscLst = new List<doMiscTypeCode>();
                        if (CommonUtil.IsNullOrEmpty(draft.doTbt_DraftRentalContrat.DispatchTypeCode) == false)
                        {
                            miscLst.Add(new doMiscTypeCode() { FieldName = MiscType.C_DISPATCH_TYPE, ValueCode = draft.doTbt_DraftRentalContrat.DispatchTypeCode });
                            miscLst = cmhandler.GetMiscTypeCodeList(miscLst);
                        }

                        docReport = new tbt_DocContractReport(); //tbt_DocContractReport docReport = new tbt_DocContractReport(); //Modify by Jutarat A. on 18122012
                        //docReport.DocID = docResLst[0].DocID; //Comment by Jutarat A. on 18122012 (Move to CreateContractDocData())
                        docReport.PlanCode = draft.doTbt_DraftRentalContrat.PlanCode;

                        if (miscLst.Count > 0)
                            docReport.DispatchType = miscLst[0].ValueDisplayEN;

                        docReport.OperationDate = draft.doTbt_DraftRentalContrat.ExpectedStartServiceDate;
                        docReport.AutoRenewMonth = draft.doTbt_DraftRentalContrat.AutoRenewMonth;

                        #region Install Fee for Approve Contract

                        docReport.InstallFee_ApproveContractCurrencyType = draft.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractCurrencyType;
                        docReport.InstallFee_ApproveContract = draft.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContract;
                        docReport.InstallFee_ApproveContractUsd = draft.doTbt_DraftRentalContrat.OrderInstallFee_ApproveContractUsd;

                        #endregion
                        #region Install Fee for Complete Install

                        docReport.InstallFee_CompleteInstallCurrencyType = draft.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallCurrencyType;
                        docReport.InstallFee_CompleteInstall = draft.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstall;
                        docReport.InstallFee_CompleteInstallUsd = draft.doTbt_DraftRentalContrat.OrderInstallFee_CompleteInstallUsd;

                        #endregion
                        #region Install Fee for Start Service

                        docReport.InstallFee_StartServiceCurrencyType = draft.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceCurrencyType;
                        docReport.InstallFee_StartService = draft.doTbt_DraftRentalContrat.OrderInstallFee_StartService;
                        docReport.InstallFee_StartServiceUsd = draft.doTbt_DraftRentalContrat.OrderInstallFee_StartServiceUsd;

                        #endregion

                        if (draft.doTbt_DraftRentalBillingTarget != null)
                        {
                            foreach (tbt_DraftRentalBillingTarget billing in draft.doTbt_DraftRentalBillingTarget)
                            {
                                if (billing.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                                {
                                    docReport.DepositFeePayMethod = billing.PayMethod;
                                    docReport.DepositFeePhase = billing.BillingTiming;
                                }
                                else if (billing.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE)
                                {
                                    if (billing.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                                        docReport.InstallFeePayMethod_ApproveContract = billing.PayMethod;
                                    else if (billing.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION)
                                        docReport.InstallFeePayMethod_CompleteInstall = billing.PayMethod;
                                    else if (billing.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE)
                                        docReport.InstallFeePayMethod_StartService = billing.PayMethod;
                                }
                            }
                        }

                        docReport.NegotiationTotalInstallFee = draft.doTbt_DraftRentalContrat.OrderInstallFee;
                        docReport.DocumentLanguage = DocLanguage.C_DOC_LANGUAGE_EN;


                        docReport.NegotiationTotalInstallFeeUsd = draft.doTbt_DraftRentalContrat.OrderInstallFeeUsd;
                        docReport.NegotiationTotalInstallFeeCurrencyType = draft.doTbt_DraftRentalContrat.OrderInstallFeeCurrencyType;

                        //Comment by Jutarat A. on 18122012 (Move to CreateContractDocData())
                        //cdchandler.InsertTbt_DocContractReport(new List<tbt_DocContractReport>() { docReport });
                    }

                    #endregion

                    //Add by Jutarat A. on 18122012
                    //Create dtTbt_DocInstrumentDetails
                    List<tbt_DocInstrumentDetails> dtTbt_DocInstrumentDetails = null;
                    if (draft.doTbt_DraftRentalInstrument != null)
                    {
                        dtTbt_DocInstrumentDetails = new List<tbt_DocInstrumentDetails>();
                        foreach (tbt_DraftRentalInstrument detail in draft.doTbt_DraftRentalInstrument)
                        {
                            if (detail.InstrumentTypeCode == InstrumentType.C_INST_TYPE_GENERAL)
                            {
                                tbt_DocInstrumentDetails nd = new tbt_DocInstrumentDetails();
                                nd.DocID = doc.DocID;
                                nd.InstrumentCode = detail.InstrumentCode;
                                nd.InstrumentQty = detail.InstrumentQty;

                                dtTbt_DocInstrumentDetails.Add(nd);
                            }
                        }
                    }

                    //CreateContractDocData
                    dsContractDocData dsContractDoc = new dsContractDocData();
                    if (doc != null)
                        dsContractDoc.dtTbt_ContractDocument = new List<tbt_ContractDocument>() { doc };

                    if (docReport != null)
                        dsContractDoc.dtTbt_DocContractReport = new List<tbt_DocContractReport>() { docReport };

                    if (dtTbt_DocInstrumentDetails != null && dtTbt_DocInstrumentDetails.Count > 0)
                        dsContractDoc.dtTbt_DocInstrumentDetail = dtTbt_DocInstrumentDetails;

                    rcthandler.CreateContractDocData(dsContractDoc);
                    //End Add
                }

                #endregion

                //CheckTimeLog("Finish update document data");
                //CheckTimeLog("Start update contract customer history data");

                #region Update Contract Customer History Data

                tbt_ContractCustomerHistory custHist = new tbt_ContractCustomerHistory()
                {
                    ContractCode = ContractCode,
                    SequenceNo = 1,
                    ChangeDate = DateTime.Now,
                    OCC = OCC,
                    ChangeNameReasonType = null,
                    BranchNameEN = draft.doTbt_DraftRentalContrat.BranchNameEN,
                    BranchNameLC = draft.doTbt_DraftRentalContrat.BranchNameLC,
                    BranchAddressEN = draft.doTbt_DraftRentalContrat.BranchAddressEN,
                    BranchAddressLC = draft.doTbt_DraftRentalContrat.BranchAddressLC,
                    CSChangeFlag = false,
                    RCChangeFlag = false,
                    SiteChangeFlag = false
                };

                if (draft.doContractCustomer != null)
                {
                    custHist.CSCustCode = draft.doContractCustomer.CustCode;
                    custHist.ContractSignerTypeCode = draft.doTbt_DraftRentalContrat.ContractTargetSignerTypeCode;
                    custHist.CSAddressEN = draft.doContractCustomer.AddressEN;
                    custHist.CSAddressFullEN = draft.doContractCustomer.AddressFullEN;
                    custHist.CSAddressFullLC = draft.doContractCustomer.AddressFullLC;
                    custHist.CSAddressLC = draft.doContractCustomer.AddressLC;
                    custHist.CSAlleyEN = draft.doContractCustomer.AlleyEN;
                    custHist.CSAlleyLC = draft.doContractCustomer.AlleyLC;
                    custHist.CSBusinessTypeCode = draft.doContractCustomer.BusinessTypeCode;
                    custHist.CSCompanyTypeCode = draft.doContractCustomer.CompanyTypeCode;
                    custHist.CSContactPersonName = draft.doContractCustomer.ContactPersonName;
                    custHist.CSCustFullNameEN = draft.doContractCustomer.CustFullNameEN;
                    custHist.CSCustFullNameLC = draft.doContractCustomer.CustFullNameLC;
                    custHist.CSCustNameEN = draft.doContractCustomer.CustNameEN;
                    custHist.CSCustNameLC = draft.doContractCustomer.CustNameLC;
                    custHist.CSCustStatus = draft.doContractCustomer.CustStatus;
                    custHist.CSCustTypeCode = draft.doContractCustomer.CustTypeCode;
                    custHist.CSDistrictCode = draft.doContractCustomer.DistrictCode;
                    custHist.CSDummyIDFlag = draft.doContractCustomer.DummyIDFlag;
                    custHist.CSFaxNo = draft.doContractCustomer.FaxNo;
                    custHist.CSFinancialMarketTypeCode = draft.doContractCustomer.FinancialMarketTypeCode;
                    custHist.CSIDNo = draft.doContractCustomer.IDNo;
                    custHist.CSImportantFlag = draft.doContractCustomer.ImportantFlag;
                    custHist.CSMemo = draft.doContractCustomer.Memo;
                    custHist.CSPhoneNo = draft.doContractCustomer.PhoneNo;
                    custHist.CSProvinceCode = draft.doContractCustomer.ProvinceCode;
                    custHist.CSRegionCode = draft.doContractCustomer.RegionCode;
                    custHist.CSRepPersonName = draft.doContractCustomer.RepPersonName;
                    custHist.CSRoadEN = draft.doContractCustomer.RoadEN;
                    custHist.CSRoadLC = draft.doContractCustomer.RoadLC;
                    custHist.CSSECOMContactPerson = draft.doContractCustomer.SECOMContactPerson;
                    custHist.CSSubDistrictEN = draft.doContractCustomer.SubDistrictEN;
                    custHist.CSSubDistrictLC = draft.doContractCustomer.SubDistrictLC;
                    custHist.CSURL = draft.doContractCustomer.URL;
                    custHist.CSZipCode = draft.doContractCustomer.ZipCode;
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

                //CheckTimeLog("Finish update contract customer history data");

                #region Update AR

                tbt_AR ar = new tbt_AR()
                {
                    QuotationTargetCode = draft.doTbt_DraftRentalContrat.QuotationTargetCode,
                    ContractCode = draft.doTbt_DraftRentalContrat.ContractCode
                };

                IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
                arhandler.UpdateContractCode(ar);

                #endregion
                #region Keep billing data

                IBillingTempHandler bthandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

                // Get total billing temp.
                List<tbt_BillingTemp> btLst = bthandler.GetTbt_BillingTemp(basic.ContractCode, secBasic.OCC);
                if (draft.doTbt_DraftRentalBillingTarget != null)
                {
                    int count = btLst.Count;
                    foreach (tbt_DraftRentalBillingTarget bt in draft.doTbt_DraftRentalBillingTarget)
                    {
                        tbt_BillingTemp temp = new tbt_BillingTemp()
                        {
                            ContractCode = basic.ContractCode,
                            OCC = secBasic.OCC,
                            SequenceNo = count + 1,
                            BillingClientCode = bt.BillingClientCode,
                            BillingTargetCode = bt.BillingTargetCode,
                            BillingOfficeCode = bt.BillingOfficeCode,
                            DocLanguage = bt.DocLanguage, //Add by Jutarat A. on 18122013
                            BillingType = bt.BillingType,
                            CreditTerm = draft.doTbt_DraftRentalContrat.CreditTerm,
                            BillingTiming = bt.BillingTiming,

                            BillingAmtCurrencyType = bt.BillingAmtCurrencyType,
                            BillingAmt = bt.BillingAmt,
                            BillingAmtUsd = bt.BillingAmtUsd,

                            PayMethod = bt.PayMethod,
                            BillingCycle = bt.BillingCycle,
                            CalDailyFeeStatus = bt.CalDailyFeeStatus,
                            SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                        };

                        bthandler.InsertBillingTemp(temp);
                        count++;
                    }
                }

                #endregion
                #region Send billing data

                IBillingInterfaceHandler bihandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                bihandler.SendBilling_RentalApprove(draft.doTbt_DraftRentalContrat.ContractCode);

                #endregion

#if !ROUND1

                #region Generate Installation basic

                doGenInstallationBasic instBasic = new doGenInstallationBasic()
                {
                    ContractProjectCode = basic.ContractCode,
                    InstallationType = RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW,
                    OCC = basic.LastOCC,
                    ServiceTypeCode = basic.ServiceTypeCode,
                    OperationOfficeCode = basic.OperationOfficeCode,
                    SecurityTypeCode = secBasic.SecurityTypeCode,
                    NormalInstallFee = secBasic.NormalInstallFee != null? secBasic.NormalInstallFee.Value : 0,
                    NormalContractFee = secBasic.NormalContractFee != null ? secBasic.NormalContractFee.Value : 0,
                    ApproveNo1 = secBasic.ApproveNo1,
                    ApproveNo2 = secBasic.ApproveNo2
                };

                if (draft.doTbt_DraftRentalInstrument != null &&
                    (draft.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                        || draft.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
                {
                    instBasic.doInstrumentDetails = new List<tbt_InstallationInstrumentDetails>();
                    foreach (tbt_DraftRentalInstrument inst in draft.doTbt_DraftRentalInstrument)
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

                    IInstallationHandler ihandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    ihandler.GenerateInstallationBasic(instBasic);
                }
                                
                #endregion
                #region Book instrument

                if (draft.doTbt_DraftRentalInstrument != null && 
                    ( draft.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                        || draft.doTbt_DraftRentalContrat.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE) )
                {
                    if (draft.doTbt_DraftRentalInstrument.Count > 0)
                    {
                        doBooking booking = new doBooking()
                        {
                            ContractCode = basic.ContractCode,
                            ExpectedStartServiceDate = secBasic.ExpectedOperationDate != null ? secBasic.ExpectedOperationDate.Value : DateTime.Now
                        };

                        booking.InstrumentCode = new List<string>();
                        booking.InstrumentQty = new List<int>();
                        foreach (tbt_DraftRentalInstrument inst in draft.doTbt_DraftRentalInstrument)
                        {
                            booking.InstrumentCode.Add(inst.InstrumentCode);
                            booking.InstrumentQty.Add(inst.InstrumentQty != null ? inst.InstrumentQty.Value : 0);
                        }

                        IInventoryHandler ivhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
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
        private doEmailTemplate CTS010_GenerateRejectMailTemplate(doDraftRentalContractData draft)
        {
            try
            {
                doRejectContractEmailObject obj = new doRejectContractEmailObject();
                obj.QuotationTargetCode = draft.doTbt_DraftRentalContrat.QuotationTargetCodeFull;

                #region Customer / Site

                if (draft.doContractCustomer != null)
                {
                    obj.CustFullNameEN = draft.doContractCustomer.CustFullNameEN;
                    obj.CustFullNameLC = draft.doContractCustomer.CustFullNameLC;
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
                        draft.doTbt_DraftRentalContrat.ProductCode,
                        draft.doTbt_DraftRentalContrat.ProductTypeCode);

                if (pLst.Count > 0)
                {
                    obj.ProductNameEN = pLst[0].ProductNameEN;
                    obj.ProductNameLC = pLst[0].ProductNameLC;
                }

                #endregion
                #region Set Employee Name

                List<tbm_Employee> empLst = new List<tbm_Employee>();
                if (CommonUtil.IsNullOrEmpty(draft.doTbt_DraftRentalContrat.SalesmanEmpNo1) == false)
                    empLst.Add(new tbm_Employee() { EmpNo = draft.doTbt_DraftRentalContrat.SalesmanEmpNo1 });
                if (CommonUtil.IsNullOrEmpty(draft.doTbt_DraftRentalContrat.SalesmanEmpNo2) == false)
                    empLst.Add(new tbm_Employee() { EmpNo = draft.doTbt_DraftRentalContrat.SalesmanEmpNo2 });

                IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                empLst = emphandler.GetEmployeeList(empLst);
                foreach (tbm_Employee emp in empLst)
                {
                    if (emp.EmpNo == draft.doTbt_DraftRentalContrat.SalesmanEmpNo1)
                    {
                        obj.SalesmanEmpNameENNo1 = CommonUtil.TextFullName(emp.EmpFirstNameEN, emp.EmpLastNameEN);
                        obj.SalesmanEmpNameLCNo1 = CommonUtil.TextFullName(emp.EmpFirstNameLC, emp.EmpLastNameLC);
                    }
                    else if (emp.EmpNo == draft.doTbt_DraftRentalContrat.SalesmanEmpNo2)
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
                    if (office.OfficeCode == draft.doTbt_DraftRentalContrat.ContractOfficeCode)
                    {
                        obj.ContractOfficeNameEN = office.OfficeNameEN;
                        obj.ContractOfficeNameLC = office.OfficeNameLC;
                    }
                    if (office.OfficeCode == draft.doTbt_DraftRentalContrat.OperationOfficeCode)
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
        private doEmailTemplate CTS010_GenerateApproveMailTemplate(doDraftRentalContractData draft)
        {
            try
            {
                doApproveContractEmailObject obj = new doApproveContractEmailObject();
                obj.ContractCode = draft.doTbt_DraftRentalContrat.ContractCodeShort;

                #region Customer / Site

                if (draft.doContractCustomer != null)
                {
                    obj.CustFullNameEN = draft.doContractCustomer.CustFullNameEN;
                    obj.CustFullNameLC = draft.doContractCustomer.CustFullNameLC;
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
                        draft.doTbt_DraftRentalContrat.ProductCode,
                        draft.doTbt_DraftRentalContrat.ProductTypeCode);

                if (pLst.Count > 0)
                {
                    obj.ProductNameEN = pLst[0].ProductNameEN;
                    obj.ProductNameLC = pLst[0].ProductNameLC;
                }

                #endregion
                #region Set Employee Name

                List<tbm_Employee> empLst = new List<tbm_Employee>();
                if (CommonUtil.IsNullOrEmpty(draft.doTbt_DraftRentalContrat.SalesmanEmpNo1) == false)
                    empLst.Add(new tbm_Employee() { EmpNo = draft.doTbt_DraftRentalContrat.SalesmanEmpNo1 });
                if (CommonUtil.IsNullOrEmpty(draft.doTbt_DraftRentalContrat.SalesmanEmpNo2) == false)
                    empLst.Add(new tbm_Employee() { EmpNo = draft.doTbt_DraftRentalContrat.SalesmanEmpNo2 });

                IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                empLst = emphandler.GetEmployeeList(empLst);
                foreach (tbm_Employee emp in empLst)
                {
                    if (emp.EmpNo == draft.doTbt_DraftRentalContrat.SalesmanEmpNo1)
                    {
                        obj.SalesmanEmpNameENNo1 = CommonUtil.TextFullName(emp.EmpFirstNameEN, emp.EmpLastNameEN);
                        obj.SalesmanEmpNameLCNo1 = CommonUtil.TextFullName(emp.EmpFirstNameLC, emp.EmpLastNameLC);
                    }
                    else if (emp.EmpNo == draft.doTbt_DraftRentalContrat.SalesmanEmpNo2)
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
                    if (office.OfficeCode == draft.doTbt_DraftRentalContrat.ContractOfficeCode)
                    {
                        obj.ContractOfficeNameEN = office.OfficeNameEN;
                        obj.ContractOfficeNameLC = office.OfficeNameLC;
                    }
                    if (office.OfficeCode == draft.doTbt_DraftRentalContrat.OperationOfficeCode)
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
        private bool CTS010_IsSameCustomer(doCustomerWithGroup contract, doCustomerWithGroup real)
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
        private bool CTS010_IsSameBillingClient(tbm_BillingClient client, tbm_BillingClient new_client)
        {
            try
            {
                if (client != null && new_client != null)
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

                        object obj1 = prop.GetValue(client, null);
                        object obj2 = prop.GetValue(new_client, null);

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
