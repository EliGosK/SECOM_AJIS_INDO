using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Quotation.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Reflection;
using System.Transactions;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.Presentation.Quotation.Controllers
{
    public partial class QuotationController : BaseController
    {
        private const string QUS030_SCREEN_NAME = "QUS030";

        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult QUS030_Authority(QUS030_ScreenParameter param)
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
                #region Check Import

                dsImportData importData = QUS050_GetImportData(param.ImportKey);
                if (importData == null)
                    param.ImportKey = null;

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<QUS030_ScreenParameter>(QUS030_SCREEN_NAME, param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize(QUS030_SCREEN_NAME)]
        public ActionResult QUS030()
        {
            ViewBag.IsImportFromQUS020 = null;

            try
            {
                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.QuotationKey != null)
                    {
                        ViewBag.QuotationTargetCode = param.QuotationKey.QuotationTargetCode;
                        ViewBag.IsImportFromQUS020 = param.ImportKey;
                    }
                }
            }
            catch
            {
            }

            return View();
        }
        /// <summary>
        /// Generate quotation detail information in case of sale section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_02()
        {
            try
            {
                ViewBag.EmployeeNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                ViewBag.SpecialInstallationFlagNo = true;
                ViewBag.SpecialInstallationFlagYes = false;
                ViewBag.NewBldMgmtFlagNeed = false;
                ViewBag.NewBldMgmtFlagNoNeed = true;

                ViewBag.DisabledProductCode = false;
                ViewBag.DisabledSpecialInstallationFlag = false;
                ViewBag.DisabledSiteBuildingArea = false;
                ViewBag.DisabledSecurityAreaFrom = false;
                ViewBag.DisabledSecurityAreaTo = false;
                ViewBag.DisabledMainStructureTypeCode = false;
                ViewBag.DisabledBuildingTypeCode = false;
                ViewBag.DisabledNewBldMgmtFlag = false;
                ViewBag.DisabledNewBldMgmtCost = false;
                ViewBag.DisabledNewBldMgmtCostCurrencyType = false;
                ViewBag.DisabledBidGuaranteeAmount1 = false;
                ViewBag.DisabledBidGuaranteeAmount1CurrencyType = false;
                ViewBag.DisabledBidGuaranteeAmount2 = false;
                ViewBag.DisabledBidGuaranteeAmount2CurrencyType = false;
                ViewBag.DisabledApproveNo1 = false;
                ViewBag.DisabledApproveNo2 = false;
                ViewBag.DisabledApproveNo3 = false;
                ViewBag.DisabledApproveNo4 = false;
                ViewBag.DisabledApproveNo5 = false;

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion
                ViewBag.SiteBuildingArea = "0.00";
                ViewBag.SecurityAreaFrom = "0.00";
                ViewBag.SecurityAreaTo =  "0.00";
                ViewBag.MainStructureTypeCode = "08";
                ViewBag.chkCeilingTypeNone = true;
                ViewBag.chkSpecialInsOther = true;
                if (doInitData != null)
                {
                    if (doInitData.doQuotationHeaderData != null)
                    {
                        if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                        {
                            ViewBag.ProductTypeCode = doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode;
                            if (doInitData.doQuotationHeaderData.doQuotationTarget != null && doInitData.doQuotationBasic != null)
                            {
                                if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode !=
                                    SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                                    ViewBag.ProductCodeCondition = doInitData.doQuotationBasic.ProductCode;
                            }
                        }

                    }


                    if (doInitData.doQuotationBasic != null)
                    {
                        ViewBag.ProductCode = doInitData.doQuotationBasic.ProductCode;
                        ViewBag.PlanCode = doInitData.doQuotationBasic.PlanCode;
                        ViewBag.QuotationNo = doInitData.doQuotationBasic.QuotationNo;
                        ViewBag.SpecialInstallationFlagYes = doInitData.doQuotationBasic.SpecialInstallationFlag != null ? doInitData.doQuotationBasic.SpecialInstallationFlag : false;
                        ViewBag.SpecialInstallationFlagNo = doInitData.doQuotationBasic.SpecialInstallationFlag != null ? !doInitData.doQuotationBasic.SpecialInstallationFlag : true;
                        ViewBag.PlannerEmpNo = doInitData.doQuotationBasic.PlannerEmpNo;
                        ViewBag.PlannerName = doInitData.doQuotationBasic.PlannerName;
                        ViewBag.PlanCheckerEmpNo = doInitData.doQuotationBasic.PlanCheckerEmpNo;
                        ViewBag.PlanCheckerName = doInitData.doQuotationBasic.PlanCheckerName;
                        ViewBag.PlanCheckDate = CommonUtil.TextDate(doInitData.doQuotationBasic.PlanCheckDate);
                        ViewBag.PlanApproverEmpNo = doInitData.doQuotationBasic.PlanApproverEmpNo;
                        ViewBag.PlanApproverName = doInitData.doQuotationBasic.PlanApproverName;
                        ViewBag.PlanApproveDate = CommonUtil.TextDate(doInitData.doQuotationBasic.PlanApproveDate);
                        ViewBag.SiteBuildingArea = CommonUtil.TextNumeric(doInitData.doQuotationBasic.SiteBuildingArea, 2, "0.00");
                        ViewBag.SecurityAreaFrom = CommonUtil.TextNumeric(doInitData.doQuotationBasic.SecurityAreaFrom, 2,"0.00");
                        ViewBag.SecurityAreaTo = CommonUtil.TextNumeric(doInitData.doQuotationBasic.SecurityAreaTo, 2, "0.00");
                        ViewBag.MainStructureTypeCode = doInitData.doQuotationBasic.MainStructureTypeCode;
                        ViewBag.BuildingTypeCode = doInitData.doQuotationBasic.BuildingTypeCode;
                        ViewBag.NewBldMgmtFlagNeed = doInitData.doQuotationBasic.NewBldMgmtFlag != null ? doInitData.doQuotationBasic.NewBldMgmtFlag : false;
                        ViewBag.NewBldMgmtFlagNoNeed = doInitData.doQuotationBasic.NewBldMgmtFlag != null ? !doInitData.doQuotationBasic.NewBldMgmtFlag : true;
                        //ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(doInitData.doQuotationBasic.NewBldMgmtCost);
                        //ViewBag.NewBldMgmtCostCurrencyType = doInitData.doQuotationBasic.NewBldMgmtCostCurrencyType;

                        ViewBag.NewBldMgmtCostCurrencyType = doInitData.doQuotationBasic.NewBldMgmtCostCurrencyType;
                        if (doInitData.doQuotationBasic.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(doInitData.doQuotationBasic.NewBldMgmtCostUsd);
                        }
                        else
                        {
                            ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(doInitData.doQuotationBasic.NewBldMgmtCost);
                        }

                        ViewBag.SalesmanEmpNo1 = doInitData.doQuotationBasic.SalesmanEmpNo1;
                        ViewBag.SalesmanEmpNo2 = doInitData.doQuotationBasic.SalesmanEmpNo2;
                        ViewBag.SalesmanEmpNo3 = doInitData.doQuotationBasic.SalesmanEmpNo3;
                        ViewBag.SalesmanEmpNo4 = doInitData.doQuotationBasic.SalesmanEmpNo4;
                        ViewBag.SalesmanEmpNo5 = doInitData.doQuotationBasic.SalesmanEmpNo5;
                        ViewBag.SalesmanEmpNo6 = doInitData.doQuotationBasic.SalesmanEmpNo6;
                        ViewBag.SalesmanEmpNo7 = doInitData.doQuotationBasic.SalesmanEmpNo7;
                        ViewBag.SalesmanEmpNo8 = doInitData.doQuotationBasic.SalesmanEmpNo8;
                        ViewBag.SalesmanEmpNo9 = doInitData.doQuotationBasic.SalesmanEmpNo9;
                        ViewBag.SalesmanEmpNo10 = doInitData.doQuotationBasic.SalesmanEmpNo10;

                        ViewBag.Saleman1Name = doInitData.doQuotationBasic.SalesmanEmpNameNo1;
                        ViewBag.Saleman2Name = doInitData.doQuotationBasic.SalesmanEmpNameNo2;
                        ViewBag.Saleman3Name = doInitData.doQuotationBasic.SalesmanEmpNameNo3;
                        ViewBag.Saleman4Name = doInitData.doQuotationBasic.SalesmanEmpNameNo4;
                        ViewBag.Saleman5Name = doInitData.doQuotationBasic.SalesmanEmpNameNo5;
                        ViewBag.Saleman6Name = doInitData.doQuotationBasic.SalesmanEmpNameNo6;
                        ViewBag.Saleman7Name = doInitData.doQuotationBasic.SalesmanEmpNameNo7;
                        ViewBag.Saleman8Name = doInitData.doQuotationBasic.SalesmanEmpNameNo8;
                        ViewBag.Saleman9Name = doInitData.doQuotationBasic.SalesmanEmpNameNo9;
                        ViewBag.Saleman10Name = doInitData.doQuotationBasic.SalesmanEmpNameNo10;

                        ViewBag.BidGuaranteeAmount1CurrencyType = doInitData.doQuotationBasic.BidGuaranteeAmount1CurrencyType;
                        if (doInitData.doQuotationBasic.BidGuaranteeAmount1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.BidGuaranteeAmount1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.BidGuaranteeAmount1Usd);
                        }
                        else
                        {
                            ViewBag.BidGuaranteeAmount1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.BidGuaranteeAmount1);
                        }

                        ViewBag.BidGuaranteeAmount2CurrencyType = doInitData.doQuotationBasic.BidGuaranteeAmount2CurrencyType;
                        if (doInitData.doQuotationBasic.BidGuaranteeAmount2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.BidGuaranteeAmount2 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.BidGuaranteeAmount2Usd);
                        }
                        else
                        {
                            ViewBag.BidGuaranteeAmount2 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.BidGuaranteeAmount2);
                        }


                        ViewBag.ApproveNo1 = doInitData.doQuotationBasic.ApproveNo1;
                        ViewBag.ApproveNo2 = doInitData.doQuotationBasic.ApproveNo2;
                        ViewBag.ApproveNo3 = doInitData.doQuotationBasic.ApproveNo3;
                        ViewBag.ApproveNo4 = doInitData.doQuotationBasic.ApproveNo4;
                        ViewBag.ApproveNo5 = doInitData.doQuotationBasic.ApproveNo5;

                        if (doInitData.doQuotationHeaderData != null)
                        {
                            if (doInitData.doQuotationHeaderData.doQuotationInstallationDetail != null)
                            {
                                ViewBag.chkCeilingTypeTBar = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeTBar;
                                ViewBag.chkCeilingTypeSlabConcrete = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeSlabConcrete;
                                ViewBag.chkCeilingTypeMBar = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeMBar;
                                ViewBag.chkCeilingTypeSteel = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeSteel;
                                ViewBag.chkCeilingTypeNone = !(
                                    (doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeTBar ?? false)
                                    || (doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeSlabConcrete ?? false)
                                    || (doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeMBar ?? false)
                                    || (doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeSteel ?? false)
                                );
                                
                                ViewBag.txtCeilingHeight = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingHeight;
                                ViewBag.chkSpecialInsPVC = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsPVC;
                                ViewBag.chkSpecialInsSLN = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsSLN;
                                ViewBag.chkSpecialInsProtector = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsProtector;
                                ViewBag.chkSpecialInsEMT = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsEMT;
                                ViewBag.chkSpecialInsPE = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsPE;
                                ViewBag.chkSpecialInsOther = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsOther;
                                ViewBag.txtSpecialInsOther = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsOtherText;
                            }
                        }

                        if (isImportData)
                        {
                            ViewBag.DisabledProductCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ProductCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ProductName);

                            ViewBag.DisabledSpecialInstallationFlag = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SpecialInstallationFlag);
                            ViewBag.DisabledSiteBuildingArea = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SiteBuildingArea);
                            ViewBag.DisabledSecurityAreaFrom = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SecurityAreaFrom);
                            ViewBag.DisabledSecurityAreaTo = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SecurityAreaTo);

                            ViewBag.DisabledMainStructureTypeCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MainStructureTypeCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MainStructureTypeName);

                            ViewBag.DisabledBuildingTypeCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.BuildingTypeCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.BuildingTypeName);

                            ViewBag.DisabledNewBldMgmtFlag = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.NewBldMgmtFlag);
                            ViewBag.DisabledNewBldMgmtCost = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.NewBldMgmtCost);
                            ViewBag.DisabledNewBldMgmtCostCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.NewBldMgmtCostCurrencyType);
                            ViewBag.DisabledBidGuaranteeAmount1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.BidGuaranteeAmount1);
                            ViewBag.DisabledBidGuaranteeAmount2 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.BidGuaranteeAmount2);
                            ViewBag.DisabledBidGuaranteeAmount1CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.BidGuaranteeAmount1CurrencyType);
                            ViewBag.DisabledBidGuaranteeAmount2CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.BidGuaranteeAmount2CurrencyType);
                            ViewBag.DisabledApproveNo1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo1);
                            ViewBag.DisabledApproveNo2 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo2);
                            ViewBag.DisabledApproveNo3 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo3);
                            ViewBag.DisabledApproveNo4 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo4);
                            ViewBag.DisabledApproveNo5 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo5);
                        }
                        else if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                        {
                            ViewBag.DisabledProductCode = true;
                        }
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_02");
        }
        /// <summary>
        /// Generate quotation detail information in case of alarm section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_03()
        {
            try
            {
                ViewBag.EmployeeNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                ViewBag.SpecialInstallationFlagNo = true;
                ViewBag.SpecialInstallationFlagYes = false;
                ViewBag.NewBldMgmtFlagNeed = false;
                ViewBag.NewBldMgmtFlagNoNeed = true;
                ViewBag.chkMaintenanceCycle = false;
                ViewBag.ShowDispathType = true;

                ViewBag.DisabledProductCode = false;
                ViewBag.DisabledSecurityTypeCode = false;
                ViewBag.DisabledDispatchTypeCode = false;
                ViewBag.DisabledOperationType = false;
                ViewBag.DisabledPhoneLineTypeCode1 = false;
                ViewBag.DisabledPhoneLineTypeCode2 = false;
                ViewBag.DisabledPhoneLineTypeCode3 = false;
                ViewBag.DisabledPhoneLineOwnerTypeCode1 = false;
                ViewBag.DisabledPhoneLineOwnerTypeCode2 = false;
                ViewBag.DisabledPhoneLineOwnerTypeCode3 = false;
                ViewBag.DisabledServiceType = false;
                ViewBag.DisabledSpecialInstallationFlag = false;
                ViewBag.DisabledSiteBuildingArea = false;
                ViewBag.DisabledSecurityAreaFrom = false;
                ViewBag.DisabledSecurityAreaTo = false;
                ViewBag.DisabledMainStructureTypeCode = false;
                ViewBag.DisabledBuildingTypeCode = false;
                ViewBag.DisabledNewBldMgmtFlag = false;
                ViewBag.DisabledNewBldMgmtCost = false;
                ViewBag.DisabledNewBldMgmtCostCurrencyType = false;
                ViewBag.DisabledNumOfBuilding = false;
                ViewBag.DisabledNumOfFloor = false;
                ViewBag.DisabledMaintenanceCycle = false;
                ViewBag.DisabledInsuranceTypeCode = false;
                ViewBag.DisabledInsuranceCoverageAmount = false;
                ViewBag.DisabledInsuranceCoverageAmountCurrencyType = false;
                ViewBag.DisabledMonthlyInsuranceFee = false;
                ViewBag.DisabledMonthlyInsuranceFeeCurrencyType = false;
                ViewBag.DisabledMaintenanceFee1 = false;
                ViewBag.DisabledMaintenanceFee1CurrencyType = false;
                ViewBag.DisabledAdditionalFee1 = false;
                ViewBag.DisabledAdditionalFee2 = false;
                ViewBag.DisabledAdditionalFee3 = false;
                ViewBag.DisabledAdditionalFee1CurrencyType = false;
                ViewBag.DisabledAdditionalFee2CurrencyType = false;
                ViewBag.DisabledAdditionalFee3CurrencyType = false;
                ViewBag.DisabledAdditionalApproveNo1 = false;
                ViewBag.DisabledAdditionalApproveNo2 = false;
                ViewBag.DisabledAdditionalApproveNo3 = false;
                ViewBag.DisabledApproveNo1 = false;
                ViewBag.DisabledApproveNo2 = false;
                ViewBag.DisabledApproveNo3 = false;
                ViewBag.DisabledApproveNo4 = false;
                ViewBag.DisabledApproveNo5 = false;

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

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion
                ViewBag.PhoneLineTypeCode1 = "9";
                ViewBag.PhoneLineOwnerTypeCode1= "2";
                ViewBag.SiteBuildingArea = "0.00";
                ViewBag.MainStructureTypeCode = "08";
                ViewBag.SecurityAreaFrom = "0.00";
                ViewBag.SecurityAreaTo = "0.00";
                ViewBag.chkCeilingTypeNone = true;
                ViewBag.chkSpecialInsOther = true;
                if (doInitData != null)
                {
                    if (doInitData.doQuotationHeaderData != null)
                    {
                        if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                        {
                            ViewBag.ProductTypeCode = doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode;
                            if (doInitData.doQuotationHeaderData.doQuotationTarget != null && doInitData.doQuotationBasic != null)
                            {
                                if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode !=
                                    SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                                    ViewBag.ProductCodeCondition = doInitData.doQuotationBasic.ProductCode;
                            }

                            if (doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                                ViewBag.ShowDispathType = false;
                        }
                    }

                    if (doInitData.doQuotationBasic != null)
                    {
                        ViewBag.ProductCode = doInitData.doQuotationBasic.ProductCode;
                        ViewBag.SecurityTypeCode = doInitData.doQuotationBasic.SecurityTypeCode;
                        ViewBag.DispatchTypeCode = doInitData.doQuotationBasic.DispatchTypeCode;
                        ViewBag.PhoneLineTypeCode1 = doInitData.doQuotationBasic.PhoneLineTypeCode1;
                        ViewBag.PhoneLineTypeCode2 = doInitData.doQuotationBasic.PhoneLineTypeCode2;
                        ViewBag.PhoneLineTypeCode3 = doInitData.doQuotationBasic.PhoneLineTypeCode3;
                        ViewBag.PhoneLineOwnerTypeCode1 = doInitData.doQuotationBasic.PhoneLineOwnerTypeCode1;
                        ViewBag.PhoneLineOwnerTypeCode2 = doInitData.doQuotationBasic.PhoneLineOwnerTypeCode2;
                        ViewBag.PhoneLineOwnerTypeCode3 = doInitData.doQuotationBasic.PhoneLineOwnerTypeCode3;

                        List<string> provLst = new List<string>();
                        if (doInitData.doQuotationBasic.FireMonitorFlag == true)
                            provLst.Add(CommonValue.QUOTATION_SERVICE_TYPE_FIRE_MONITORING);
                        if (doInitData.doQuotationBasic.CrimePreventFlag == true)
                            provLst.Add(CommonValue.QUOTATION_SERVICE_TYPE_CRIME_PREVENTION);
                        if (doInitData.doQuotationBasic.EmergencyReportFlag == true)
                            provLst.Add(CommonValue.QUOTATION_SERVICE_TYPE_EMERGENCY_REPORT);
                        if (doInitData.doQuotationBasic.FacilityMonitorFlag == true)
                            provLst.Add(CommonValue.QUOTATION_SERVICE_TYPE_FACILITY_MONITORING);
                        ViewBag.ProvideServiceType = provLst.ToArray();

                        ViewBag.PlanCode = doInitData.doQuotationBasic.PlanCode;
                        ViewBag.QuotationNo = doInitData.doQuotationBasic.QuotationNo;
                        ViewBag.SpecialInstallationFlagYes = doInitData.doQuotationBasic.SpecialInstallationFlag != null ? doInitData.doQuotationBasic.SpecialInstallationFlag : false;
                        ViewBag.SpecialInstallationFlagNo = doInitData.doQuotationBasic.SpecialInstallationFlag != null ? !doInitData.doQuotationBasic.SpecialInstallationFlag : true;
                        ViewBag.PlannerEmpNo = doInitData.doQuotationBasic.PlannerEmpNo;
                        ViewBag.PlannerName = doInitData.doQuotationBasic.PlannerName;
                        ViewBag.PlanCheckerEmpNo = doInitData.doQuotationBasic.PlanCheckerEmpNo;
                        ViewBag.PlanCheckerName = doInitData.doQuotationBasic.PlanCheckerName;
                        ViewBag.PlanCheckDate = CommonUtil.TextDate(doInitData.doQuotationBasic.PlanCheckDate);
                        ViewBag.PlanApproverEmpNo = doInitData.doQuotationBasic.PlanApproverEmpNo;
                        ViewBag.PlanApproverName = doInitData.doQuotationBasic.PlanApproverName;
                        ViewBag.PlanApproveDate = CommonUtil.TextDate(doInitData.doQuotationBasic.PlanApproveDate);
                        ViewBag.SiteBuildingArea = CommonUtil.TextNumeric(doInitData.doQuotationBasic.SiteBuildingArea, 2, "0.00");
                        ViewBag.SecurityAreaFrom = CommonUtil.TextNumeric(doInitData.doQuotationBasic.SecurityAreaFrom, 2, "0.00");
                        ViewBag.SecurityAreaTo = CommonUtil.TextNumeric(doInitData.doQuotationBasic.SecurityAreaTo, 2, "0.00");
                        ViewBag.MainStructureTypeCode = doInitData.doQuotationBasic.MainStructureTypeCode;
                        ViewBag.BuildingTypeCode = doInitData.doQuotationBasic.BuildingTypeCode;
                        ViewBag.NewBldMgmtFlagNeed = doInitData.doQuotationBasic.NewBldMgmtFlag != null ? doInitData.doQuotationBasic.NewBldMgmtFlag : false;
                        ViewBag.NewBldMgmtFlagNoNeed = doInitData.doQuotationBasic.NewBldMgmtFlag != null ? !doInitData.doQuotationBasic.NewBldMgmtFlag : true;

                        ViewBag.NewBldMgmtCostCurrencyType = doInitData.doQuotationBasic.NewBldMgmtCostCurrencyType;
                        if (doInitData.doQuotationBasic.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(doInitData.doQuotationBasic.NewBldMgmtCostUsd);
                        }
                        else
                        {
                            ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(doInitData.doQuotationBasic.NewBldMgmtCost);
                        }

                        ViewBag.NumOfBuilding = CommonUtil.TextNumeric(doInitData.doQuotationBasic.NumOfBuilding, 0);
                        ViewBag.NumOfFloor = CommonUtil.TextNumeric(doInitData.doQuotationBasic.NumOfFloor, 0);

                        ViewBag.MaintenanceCycle = ViewBag.DefaultMACycle;
                        if (doInitData.doQuotationBasic.MaintenanceCycle != null)
                        {
                            if (ViewBag.DefaultMACycle != doInitData.doQuotationBasic.MaintenanceCycle.ToString())
                            {
                                ViewBag.chkMaintenanceCycle = true;

                                /* --- *** --- */
                                /* ViewBag.MaintenanceCycle = doInitData.doQuotationBasic.MaintenanceCycle.ToString(); */

                                if (CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycle) == false
                                    && CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycleName))
                                {
                                    ViewBag.MaintenanceCycle = ViewBag.DefaultMACycle;
                                }
                                else
                                {
                                    ViewBag.MaintenanceCycle = doInitData.doQuotationBasic.MaintenanceCycle.ToString();
                                }
                            }
                        }

                        ViewBag.SalesmanEmpNo1 = doInitData.doQuotationBasic.SalesmanEmpNo1;
                        ViewBag.SalesmanEmpNo2 = doInitData.doQuotationBasic.SalesmanEmpNo2;
                        ViewBag.SalesSupporterEmpNo = doInitData.doQuotationBasic.SalesSupporterEmpNo;
                        ViewBag.Saleman1Name = doInitData.doQuotationBasic.SalesmanEmpNameNo1;
                        ViewBag.Saleman2Name = doInitData.doQuotationBasic.SalesmanEmpNameNo2;
                        ViewBag.SalesSupporterEmpName = doInitData.doQuotationBasic.SalesSupporterEmpName;
                        ViewBag.InsuranceTypeCode = doInitData.doQuotationBasic.InsuranceTypeCode;
                        ViewBag.InsuranceCoverageAmount = CommonUtil.TextNumeric(doInitData.doQuotationBasic.InsuranceCoverageAmount);

                        ViewBag.InsuranceCoverageAmountCurrencyType = doInitData.doQuotationBasic.InsuranceCoverageAmountCurrencyType;
                        if (doInitData.doQuotationBasic.InsuranceCoverageAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.InsuranceCoverageAmount = CommonUtil.TextNumeric(doInitData.doQuotationBasic.InsuranceCoverageAmountUsd);
                        }
                        else
                        {
                            ViewBag.InsuranceCoverageAmount = CommonUtil.TextNumeric(doInitData.doQuotationBasic.InsuranceCoverageAmount);
                        }

                        ViewBag.MonthlyInsuranceFeeCurrencyType = doInitData.doQuotationBasic.MonthlyInsuranceFeeCurrencyType;
                        if (doInitData.doQuotationBasic.MonthlyInsuranceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.MonthlyInsuranceFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.MonthlyInsuranceFeeUsd);
                        }
                        else
                        {
                            ViewBag.MonthlyInsuranceFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.MonthlyInsuranceFee);
                        }

                        ViewBag.MaintenanceFee1CurrencyType = doInitData.doQuotationBasic.MaintenanceFee1CurrencyType;
                        if (doInitData.doQuotationBasic.MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.MaintenanceFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.MaintenanceFee1Usd);
                        }
                        else
                        {
                            ViewBag.MaintenanceFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.MaintenanceFee1);
                        }

                        ViewBag.AdditionalFee1CurrencyType = doInitData.doQuotationBasic.AdditionalFee1CurrencyType;
                        if (doInitData.doQuotationBasic.AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.AdditionalFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee1Usd);
                        }
                        else
                        {
                            ViewBag.AdditionalFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee1);
                        }
                        ViewBag.AdditionalFee2CurrencyType = doInitData.doQuotationBasic.AdditionalFee2CurrencyType;
                        if (doInitData.doQuotationBasic.AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.AdditionalFee2 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee2Usd);
                        }
                        else
                        {
                            ViewBag.AdditionalFee2 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee2);
                        }
                        ViewBag.AdditionalFee3CurrencyType = doInitData.doQuotationBasic.AdditionalFee3CurrencyType;
                        if (doInitData.doQuotationBasic.AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.AdditionalFee3 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee3Usd);
                        }
                        else
                        {
                            ViewBag.AdditionalFee3 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee3);
                        }
                        
                        ViewBag.AdditionalApproveNo1 = doInitData.doQuotationBasic.AdditionalApproveNo1;
                        ViewBag.AdditionalApproveNo2 = doInitData.doQuotationBasic.AdditionalApproveNo2;
                        ViewBag.AdditionalApproveNo3 = doInitData.doQuotationBasic.AdditionalApproveNo3;
                        ViewBag.ApproveNo1 = doInitData.doQuotationBasic.ApproveNo1;
                        ViewBag.ApproveNo2 = doInitData.doQuotationBasic.ApproveNo2;
                        ViewBag.ApproveNo3 = doInitData.doQuotationBasic.ApproveNo3;
                        ViewBag.ApproveNo4 = doInitData.doQuotationBasic.ApproveNo4;
                        ViewBag.ApproveNo5 = doInitData.doQuotationBasic.ApproveNo5;

                        if (doInitData.doQuotationHeaderData != null)
                        {
                            if (doInitData.doQuotationHeaderData.doQuotationInstallationDetail != null)
                            {
                                ViewBag.chkCeilingTypeTBar = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeTBar;
                                ViewBag.chkCeilingTypeSlabConcrete = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeSlabConcrete;
                                ViewBag.chkCeilingTypeMBar = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeMBar;
                                ViewBag.chkCeilingTypeSteel = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeSteel;
                                ViewBag.chkCeilingTypeNone = !(
                                    (doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeTBar ?? false)
                                    || (doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeSlabConcrete ?? false)
                                    || (doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeMBar ?? false)
                                    || (doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingTypeSteel ?? false)
                                );
                                ViewBag.txtCeilingHeight = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.CeilingHeight;
                                ViewBag.chkSpecialInsPVC = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsPVC;
                                ViewBag.chkSpecialInsSLN = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsSLN;
                                ViewBag.chkSpecialInsProtector = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsProtector;
                                ViewBag.chkSpecialInsEMT = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsEMT;
                                ViewBag.chkSpecialInsPE = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsPE;
                                ViewBag.chkSpecialInsOther = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsOther;
                                ViewBag.txtSpecialInsOther = doInitData.doQuotationHeaderData.doQuotationInstallationDetail.SpecialInsOtherText;
                            }
                        }

                        if (isImportData)
                        {
                            ViewBag.DisabledProductCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ProductCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ProductName);

                            ViewBag.DisabledSecurityTypeCode = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SecurityTypeCode);

                            ViewBag.DisabledDispatchTypeCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.DispatchTypeCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.DispatchTypeName);

                            ViewBag.DisabledPhoneLineTypeCode1 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeCode1) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeName1);

                            ViewBag.DisabledPhoneLineTypeCode2 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeCode2) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeName2);

                            ViewBag.DisabledPhoneLineTypeCode3 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeCode3) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeName3);

                            ViewBag.DisabledPhoneLineOwnerTypeCode1 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeCode1) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeName1);

                            ViewBag.DisabledPhoneLineOwnerTypeCode2 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeCode2) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeName2);

                            ViewBag.DisabledPhoneLineOwnerTypeCode3 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeCode3) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeName3);

                            ViewBag.DisabledServiceType = (provLst.Count > 0);

                            ViewBag.DisabledSpecialInstallationFlag = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SpecialInstallationFlag);
                            ViewBag.DisabledSiteBuildingArea = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SiteBuildingArea);
                            ViewBag.DisabledSecurityAreaFrom = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SecurityAreaFrom);
                            ViewBag.DisabledSecurityAreaTo = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SecurityAreaTo);

                            ViewBag.DisabledMainStructureTypeCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MainStructureTypeCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MainStructureTypeName);

                            ViewBag.DisabledBuildingTypeCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.BuildingTypeCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.BuildingTypeName);

                            ViewBag.DisabledNewBldMgmtFlag = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.NewBldMgmtFlag);
                            ViewBag.DisabledNewBldMgmtCost = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.NewBldMgmtCost);
                            ViewBag.DisabledNewBldMgmtCostCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.NewBldMgmtCostCurrencyType);

                            ViewBag.DisabledNumOfBuilding = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.NumOfBuilding);
                            ViewBag.DisabledNumOfFloor = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.NumOfFloor);

                            ViewBag.DisabledMaintenanceCycle =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycle) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycleName);

                            ViewBag.DisabledInsuranceTypeCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InsuranceTypeCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InsuranceTypeName);

                            ViewBag.DisabledInsuranceCoverageAmount = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InsuranceCoverageAmount);
                            ViewBag.DisabledInsuranceCoverageAmountCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InsuranceCoverageAmountCurrencyType);
                            ViewBag.DisabledMonthlyInsuranceFee = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MonthlyInsuranceFee);
                            ViewBag.DisabledMonthlyInsuranceFeeCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MonthlyInsuranceFeeCurrencyType);
                            ViewBag.DisabledMaintenanceFee1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceFee1);
                            ViewBag.DisabledMaintenanceFee1CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceFee1CurrencyType);
                            ViewBag.DisabledAdditionalFee1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee1);
                            ViewBag.DisabledAdditionalFee2 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee2);
                            ViewBag.DisabledAdditionalFee3 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee3);
                            ViewBag.DisabledAdditionalFee1CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee1CurrencyType);
                            ViewBag.DisabledAdditionalFee2CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee2CurrencyType);
                            ViewBag.DisabledAdditionalFee3CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee3CurrencyType);
                            ViewBag.DisabledAdditionalApproveNo1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalApproveNo1);
                            ViewBag.DisabledAdditionalApproveNo2 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalApproveNo2);
                            ViewBag.DisabledAdditionalApproveNo3 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalApproveNo3);

                            ViewBag.DisabledApproveNo1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo1);
                            ViewBag.DisabledApproveNo2 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo2);
                            ViewBag.DisabledApproveNo3 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo3);
                            ViewBag.DisabledApproveNo4 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo4);
                            ViewBag.DisabledApproveNo5 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo5);
                        }
                        else if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                        {
                            if (doInitData.FirstInstallCompleteFlag == true
                                && doInitData.StartOperationFlag == false)
                                ViewBag.DisabledProductCode = true;
                        }
                    }
                    if (doInitData.OperationTypeList != null)
                    {
                        List<string> codeLst = new List<string>();
                        foreach (doQuotationOperationType o in doInitData.OperationTypeList)
                        {
                            codeLst.Add(o.OperationTypeCode);
                        }
                        ViewBag.OperationType = codeLst.ToArray();

                        if (isImportData)
                            ViewBag.DisabledOperationType = (codeLst.Count > 0);
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_03");
        }
        /// <summary>
        /// Generate quotation detail information in case of sale online section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_04()
        {
            try
            {
                ViewBag.EmployeeNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                ViewBag.chkMaintenanceCycle = false;

                ViewBag.DisabledProductCode = false;
                ViewBag.DisabledSecurityTypeCode = false;
                ViewBag.DisabledDispatchTypeCode = false;
                ViewBag.DisabledOperationType = false;
                ViewBag.DisabledPhoneLineTypeCode1 = false;
                ViewBag.DisabledPhoneLineTypeCode2 = false;
                ViewBag.DisabledPhoneLineTypeCode3 = false;
                ViewBag.DisabledPhoneLineOwnerTypeCode1 = false;
                ViewBag.DisabledPhoneLineOwnerTypeCode2 = false;
                ViewBag.DisabledPhoneLineOwnerTypeCode3 = false;
                ViewBag.DisabledServiceType = false;
                ViewBag.DisabledSaleOnlineContractCode = false;
                ViewBag.DisabledNumOfBuilding = false;
                ViewBag.DisabledNumOfFloor = false;
                ViewBag.DisabledMaintenanceCycle = false;
                ViewBag.DisabledInsuranceTypeCode = false;
                ViewBag.DisabledInsuranceCoverageAmount = false;
                ViewBag.DisabledInsuranceCoverageAmountCurrencyType = false;
                ViewBag.DisabledMonthlyInsuranceFee = false;
                ViewBag.DisabledMonthlyInsuranceFeeCurrencyType = false;
                ViewBag.DisabledMaintenanceFee1 = false;
                ViewBag.DisabledMaintenanceFee1CurrencyType = false;
                ViewBag.DisabledAdditionalFee1 = false;
                ViewBag.DisabledAdditionalFee2 = false;
                ViewBag.DisabledAdditionalFee3 = false;
                ViewBag.DisabledAdditionalFee1CurrencyType = false;
                ViewBag.DisabledAdditionalFee2CurrencyType = false;
                ViewBag.DisabledAdditionalFee3CurrencyType = false;
                ViewBag.DisabledAdditionalApproveNo1 = false;
                ViewBag.DisabledAdditionalApproveNo2 = false;
                ViewBag.DisabledAdditionalApproveNo3 = false;
                ViewBag.DisabledApproveNo1 = false;
                ViewBag.DisabledApproveNo2 = false;
                ViewBag.DisabledApproveNo3 = false;
                ViewBag.DisabledApproveNo4 = false;
                ViewBag.DisabledApproveNo5 = false;

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

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion
                ViewBag.PhoneLineTypeCode1 = "9";
                ViewBag.PhoneLineOwnerTypeCode1 = "2";

                if (doInitData != null)
                {
                    if (doInitData.doQuotationHeaderData != null)
                    {
                        if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                        {
                            ViewBag.ProductTypeCode = doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode;
                            if (doInitData.doQuotationHeaderData.doQuotationTarget != null && doInitData.doQuotationBasic != null)
                            {
                                if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode !=
                                    SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                                    ViewBag.ProductCodeCondition = doInitData.doQuotationBasic.ProductCode;
                            }
                        }
                    }

                    if (doInitData.doQuotationBasic != null)
                    {
                        ViewBag.ProductCode = doInitData.doQuotationBasic.ProductCode;
                        ViewBag.SecurityTypeCode = doInitData.doQuotationBasic.SecurityTypeCode;
                        ViewBag.DispatchTypeCode = doInitData.doQuotationBasic.DispatchTypeCode;
                        ViewBag.PhoneLineTypeCode1 = doInitData.doQuotationBasic.PhoneLineTypeCode1;
                        ViewBag.PhoneLineTypeCode2 = doInitData.doQuotationBasic.PhoneLineTypeCode2;
                        ViewBag.PhoneLineTypeCode3 = doInitData.doQuotationBasic.PhoneLineTypeCode3;
                        ViewBag.PhoneLineOwnerTypeCode1 = doInitData.doQuotationBasic.PhoneLineOwnerTypeCode1;
                        ViewBag.PhoneLineOwnerTypeCode2 = doInitData.doQuotationBasic.PhoneLineOwnerTypeCode2;
                        ViewBag.PhoneLineOwnerTypeCode3 = doInitData.doQuotationBasic.PhoneLineOwnerTypeCode3;

                        List<string> provLst = new List<string>();
                        if (doInitData.doQuotationBasic.FireMonitorFlag == true)
                            provLst.Add(CommonValue.QUOTATION_SERVICE_TYPE_FIRE_MONITORING);
                        if (doInitData.doQuotationBasic.CrimePreventFlag == true)
                            provLst.Add(CommonValue.QUOTATION_SERVICE_TYPE_CRIME_PREVENTION);
                        if (doInitData.doQuotationBasic.EmergencyReportFlag == true)
                            provLst.Add(CommonValue.QUOTATION_SERVICE_TYPE_EMERGENCY_REPORT);
                        if (doInitData.doQuotationBasic.FacilityMonitorFlag == true)
                            provLst.Add(CommonValue.QUOTATION_SERVICE_TYPE_FACILITY_MONITORING);
                        ViewBag.ProvideServiceType = provLst.ToArray();

                        /* --- Merge --- */
                        ViewBag.SaleOnlineContractCode = doInitData.doQuotationBasic.SaleOnlineContractCodeShort;
                        /* ------------- */

                        if (doInitData.doLinkageSaleContractData != null)
                        {
                            /* --- Merge --- */
                            /* ViewBag.SaleOnlineContractCode = doInitData.doQuotationBasic.SaleOnlineContractCodeShort; */
                            /* ------------- */
                            ViewBag.PlanCode = doInitData.doLinkageSaleContractData.PlanCode;
                            ViewBag.QuotationNo = doInitData.doQuotationBasic.QuotationNo;

                            ViewBag.SpecialInstallationFlag = doInitData.doLinkageSaleContractData.SpecialInstallationFlag != null ?
                                CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS030", "rdoSpecialInstall_Yes") :
                                CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS030", "rdoSpecialInstall_No");

                            ViewBag.PlannerEmpNo = doInitData.doLinkageSaleContractData.PlannerEmpNo;
                            ViewBag.PlannerName = doInitData.doLinkageSaleContractData.PlannerName;
                            ViewBag.PlanCheckerEmpNo = doInitData.doLinkageSaleContractData.PlanCheckerEmpNo;
                            ViewBag.PlanCheckerName = doInitData.doLinkageSaleContractData.PlanCheckerName;
                            ViewBag.PlanCheckDate = CommonUtil.TextDate(doInitData.doLinkageSaleContractData.PlanCheckDate);
                            ViewBag.PlanApproverEmpNo = doInitData.doLinkageSaleContractData.PlanApproverEmpNo;
                            ViewBag.PlanApproverName = doInitData.doLinkageSaleContractData.PlanApproverName;
                            ViewBag.PlanApproveDate = CommonUtil.TextDate(doInitData.doLinkageSaleContractData.PlanApproveDate);
                            ViewBag.SiteBuildingArea = CommonUtil.TextNumeric(doInitData.doLinkageSaleContractData.SiteBuildingArea, 2, "0.00");
                            ViewBag.SecurityAreaFrom = CommonUtil.TextNumeric(doInitData.doLinkageSaleContractData.SecurityAreaFrom, 2, "0.00");
                            ViewBag.SecurityAreaTo = CommonUtil.TextNumeric(doInitData.doLinkageSaleContractData.SecurityAreaTo);
                            ViewBag.MainStructureTypeCode = doInitData.doLinkageSaleContractData.MainStructureTypeCodeName;
                            ViewBag.BuildingTypeCode = doInitData.doLinkageSaleContractData.BuildingTypeCodeName;

                            if (doInitData.doLinkageSaleContractData.NewBldMgmtFlag == true)
                                ViewBag.NewBldMgmtFlag = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS030", "rdoNewBuildingMgmtTypeFlagNeed");
                            else
                                ViewBag.NewBldMgmtFlag = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS030", "rdoNewBuildingMgmtTypeFlagNoNeed");

                            ViewBag.NewBldMgmtCostCurrencyType = doInitData.doQuotationBasic.NewBldMgmtCostCurrencyType;
                            if (doInitData.doQuotationBasic.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(doInitData.doQuotationBasic.NewBldMgmtCostUsd);
                            }
                            else
                            {
                                ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(doInitData.doQuotationBasic.NewBldMgmtCost);
                            }

                        }

                        ViewBag.NumOfBuilding = CommonUtil.TextNumeric(doInitData.doQuotationBasic.NumOfBuilding, 0);
                        ViewBag.NumOfFloor = CommonUtil.TextNumeric(doInitData.doQuotationBasic.NumOfFloor, 0);

                        ViewBag.MaintenanceCycle = ViewBag.DefaultMACycle;
                        if (doInitData.doQuotationBasic.MaintenanceCycle != null)
                        {
                            if (ViewBag.DefaultMACycle != doInitData.doQuotationBasic.MaintenanceCycle.ToString())
                            {
                                ViewBag.chkMaintenanceCycle = true;

                                /* --- *** --- */
                                /* ViewBag.MaintenanceCycle = doInitData.doQuotationBasic.MaintenanceCycle.ToString(); */

                                if (CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycle) == false
                                    && CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycleName))
                                {
                                    ViewBag.MaintenanceCycle = ViewBag.DefaultMACycle;
                                }
                                else
                                {
                                    ViewBag.MaintenanceCycle = doInitData.doQuotationBasic.MaintenanceCycle.ToString();
                                }
                            }
                        }

                        ViewBag.SalesmanEmpNo1 = doInitData.doQuotationBasic.SalesmanEmpNo1;
                        ViewBag.SalesmanEmpNo2 = doInitData.doQuotationBasic.SalesmanEmpNo2;
                        ViewBag.SalesSupporterEmpNo = doInitData.doQuotationBasic.SalesSupporterEmpNo;
                        ViewBag.Saleman1Name = doInitData.doQuotationBasic.SalesmanEmpNameNo1;
                        ViewBag.Saleman2Name = doInitData.doQuotationBasic.SalesmanEmpNameNo2;
                        ViewBag.SalesSupporterEmpName = doInitData.doQuotationBasic.SalesSupporterEmpName;
                        ViewBag.InsuranceTypeCode = doInitData.doQuotationBasic.InsuranceTypeCode;
                        ViewBag.InsuranceCoverageAmount = CommonUtil.TextNumeric(doInitData.doQuotationBasic.InsuranceCoverageAmount);
                        ViewBag.InsuranceCoverageAmountCurrencyType = doInitData.doQuotationBasic.InsuranceCoverageAmountCurrencyType;
                        ViewBag.InsuranceCoverageAmountCurrencyType = doInitData.doQuotationBasic.InsuranceCoverageAmountCurrencyType;
                        if (doInitData.doQuotationBasic.InsuranceCoverageAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.InsuranceCoverageAmount = CommonUtil.TextNumeric(doInitData.doQuotationBasic.InsuranceCoverageAmountUsd);
                        }
                        else
                        {
                            ViewBag.InsuranceCoverageAmount = CommonUtil.TextNumeric(doInitData.doQuotationBasic.InsuranceCoverageAmount);
                        }

                        ViewBag.MonthlyInsuranceFeeCurrencyType = doInitData.doQuotationBasic.MonthlyInsuranceFeeCurrencyType;
                        if (doInitData.doQuotationBasic.MonthlyInsuranceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.MonthlyInsuranceFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.MonthlyInsuranceFeeUsd);
                        }
                        else
                        {
                            ViewBag.MonthlyInsuranceFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.MonthlyInsuranceFee);
                        }

                        ViewBag.MaintenanceFee1CurrencyType = doInitData.doQuotationBasic.MaintenanceFee1CurrencyType;
                        if (doInitData.doQuotationBasic.MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.MaintenanceFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.MaintenanceFee1Usd);
                        }
                        else
                        {
                            ViewBag.MaintenanceFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.MaintenanceFee1);
                        }

                        ViewBag.AdditionalFee1CurrencyType = doInitData.doQuotationBasic.AdditionalFee1CurrencyType;
                        if (doInitData.doQuotationBasic.AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.AdditionalFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee1Usd);
                        }
                        else
                        {
                            ViewBag.AdditionalFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee1);
                        }
                        ViewBag.AdditionalFee2CurrencyType = doInitData.doQuotationBasic.AdditionalFee2CurrencyType;
                        if (doInitData.doQuotationBasic.AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.AdditionalFee2 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee2Usd);
                        }
                        else
                        {
                            ViewBag.AdditionalFee2 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee2);
                        }
                        ViewBag.AdditionalFee3CurrencyType = doInitData.doQuotationBasic.AdditionalFee3CurrencyType;
                        if (doInitData.doQuotationBasic.AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.AdditionalFee3 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee3Usd);
                        }
                        else
                        {
                            ViewBag.AdditionalFee3 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee3);
                        }
                        ViewBag.AdditionalApproveNo1 = doInitData.doQuotationBasic.AdditionalApproveNo1;
                        ViewBag.AdditionalApproveNo2 = doInitData.doQuotationBasic.AdditionalApproveNo2;
                        ViewBag.AdditionalApproveNo3 = doInitData.doQuotationBasic.AdditionalApproveNo3;
                        ViewBag.ApproveNo1 = doInitData.doQuotationBasic.ApproveNo1;
                        ViewBag.ApproveNo2 = doInitData.doQuotationBasic.ApproveNo2;
                        ViewBag.ApproveNo3 = doInitData.doQuotationBasic.ApproveNo3;
                        ViewBag.ApproveNo4 = doInitData.doQuotationBasic.ApproveNo4;
                        ViewBag.ApproveNo5 = doInitData.doQuotationBasic.ApproveNo5;

                        if (isImportData)
                        {
                            ViewBag.DisabledProductCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ProductCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ProductName);

                            ViewBag.DisabledSecurityTypeCode = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SecurityTypeCode);

                            ViewBag.DisabledDispatchTypeCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.DispatchTypeCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.DispatchTypeName);

                            ViewBag.DisabledPhoneLineTypeCode1 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeCode1) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeName1);

                            ViewBag.DisabledPhoneLineTypeCode2 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeCode2) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeName2);

                            ViewBag.DisabledPhoneLineTypeCode3 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeCode3) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineTypeName3);

                            ViewBag.DisabledPhoneLineOwnerTypeCode1 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeCode1) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeName1);

                            ViewBag.DisabledPhoneLineOwnerTypeCode2 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeCode2) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeName2);

                            ViewBag.DisabledPhoneLineOwnerTypeCode3 =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeCode3) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.PhoneLineOwnerTypeName3);

                            ViewBag.DisabledServiceType = (provLst.Count > 0);

                            /* --- Merge --- */
                            /* if (doInitData.doLinkageSaleContractData != null)
                                ViewBag.DisabledSaleOnlineContractCode = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SaleOnlineContractCode); */
                            ViewBag.DisabledSaleOnlineContractCode = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SaleOnlineContractCode);
                            /* ------------- */


                            ViewBag.DisabledNumOfBuilding = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.NumOfBuilding);
                            ViewBag.DisabledNumOfFloor = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.NumOfFloor);

                            ViewBag.DisabledMaintenanceCycle =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycle) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycleName);

                            ViewBag.DisabledInsuranceTypeCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InsuranceTypeCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InsuranceTypeName);

                            ViewBag.DisabledInsuranceCoverageAmount = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InsuranceCoverageAmount);
                            ViewBag.DisabledInsuranceCoverageAmountCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InsuranceCoverageAmountCurrencyType);
                            ViewBag.DisabledMonthlyInsuranceFee = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MonthlyInsuranceFee);
                            ViewBag.DisabledMonthlyInsuranceFeeCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MonthlyInsuranceFeeCurrencyType);
                            ViewBag.DisabledMaintenanceFee1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceFee1);
                            ViewBag.DisabledMaintenanceFee1CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceFee1CurrencyType);
                            ViewBag.DisabledAdditionalFee1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee1);
                            ViewBag.DisabledAdditionalFee2 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee2);
                            ViewBag.DisabledAdditionalFee3 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee3);
                            ViewBag.DisabledAdditionalFee1CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee1CurrencyType);
                            ViewBag.DisabledAdditionalFee2CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee2CurrencyType);
                            ViewBag.DisabledAdditionalFee3CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee3CurrencyType);
                            ViewBag.DisabledAdditionalApproveNo1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalApproveNo1);
                            ViewBag.DisabledAdditionalApproveNo2 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalApproveNo2);
                            ViewBag.DisabledAdditionalApproveNo3 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalApproveNo3);

                            ViewBag.DisabledApproveNo1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo1);
                            ViewBag.DisabledApproveNo2 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo2);
                            ViewBag.DisabledApproveNo3 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo3);
                            ViewBag.DisabledApproveNo4 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo4);
                            ViewBag.DisabledApproveNo5 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo5);
                        }
                        else if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                        {
                            ViewBag.DisabledProductCode = true;
                            ViewBag.DisabledSaleOnlineContractCode = true;
                        }
                    }

                    if (doInitData.OperationTypeList != null)
                    {
                        List<string> codeLst = new List<string>();
                        foreach (doQuotationOperationType o in doInitData.OperationTypeList)
                        {
                            codeLst.Add(o.OperationTypeCode);
                        }
                        ViewBag.OperationType = codeLst.ToArray();

                        if (isImportData)
                            ViewBag.DisabledOperationType = (codeLst.Count > 0);
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_04");
        }
        /// <summary>
        /// Generate quotation detail information in case of beat guard, sentry gurard, or maintenance section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_05()
        {
            try
            {
                ViewBag.EmployeeNo = CommonUtil.dsTransData.dtUserData.EmpNo;

                ViewBag.DisabledProductCode = false;
                ViewBag.DisabledMaintenanceFee1 = false;
                ViewBag.DisabledAdditionalFee1 = false;
                ViewBag.DisabledAdditionalFee2 = false;
                ViewBag.DisabledAdditionalFee3 = false;
                ViewBag.DisabledAdditionalFee1CurrencyType = false;
                ViewBag.DisabledAdditionalFee2CurrencyType = false;
                ViewBag.DisabledAdditionalFee3CurrencyType = false;
                ViewBag.DisabledAdditionalApproveNo1 = false;
                ViewBag.DisabledAdditionalApproveNo2 = false;
                ViewBag.DisabledAdditionalApproveNo3 = false;
                ViewBag.DisabledApproveNo1 = false;
                ViewBag.DisabledApproveNo2 = false;
                ViewBag.DisabledApproveNo3 = false;
                ViewBag.DisabledApproveNo4 = false;
                ViewBag.DisabledApproveNo5 = false;

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion

                if (doInitData != null)
                {
                    if (doInitData.doQuotationHeaderData != null)
                    {
                        if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                        {
                            ViewBag.ProductTypeCode = doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode;
                            ViewBag.ProvideServiceName = doInitData.doQuotationHeaderData.doQuotationTarget.ProvideServiceName;
                            if (doInitData.doQuotationHeaderData.doQuotationTarget != null && doInitData.doQuotationBasic != null)
                            {
                                if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode !=
                                    SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                                    ViewBag.ProductCodeCondition = doInitData.doQuotationBasic.ProductCode;
                            }
                        }
                    }

                    ViewBag.ProductCode = doInitData.doQuotationBasic.ProductCode;
                    ViewBag.SalesmanEmpNo1 = doInitData.doQuotationBasic.SalesmanEmpNo1;
                    ViewBag.SalesmanEmpNo2 = doInitData.doQuotationBasic.SalesmanEmpNo2;
                    ViewBag.SalesSupporterEmpNo = doInitData.doQuotationBasic.SalesSupporterEmpNo;
                    ViewBag.Saleman1Name = doInitData.doQuotationBasic.SalesmanEmpNameNo1;
                    ViewBag.Saleman2Name = doInitData.doQuotationBasic.SalesmanEmpNameNo2;
                    ViewBag.SalesSupporterEmpName = doInitData.doQuotationBasic.SalesSupporterEmpName;

                    ViewBag.MaintenanceFee1CurrencyType = doInitData.doQuotationBasic.MaintenanceFee1CurrencyType;
                    if (doInitData.doQuotationBasic.MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        ViewBag.MaintenanceFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.MaintenanceFee1Usd);
                    }
                    else
                    {
                        ViewBag.MaintenanceFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.MaintenanceFee1);
                    }

                    ViewBag.AdditionalFee1CurrencyType = doInitData.doQuotationBasic.AdditionalFee1CurrencyType;
                    if (doInitData.doQuotationBasic.AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        ViewBag.AdditionalFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee1Usd);
                    }
                    else
                    {
                        ViewBag.AdditionalFee1 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee1);
                    }
                    ViewBag.AdditionalFee2CurrencyType = doInitData.doQuotationBasic.AdditionalFee2CurrencyType;
                    if (doInitData.doQuotationBasic.AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        ViewBag.AdditionalFee2 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee2Usd);
                    }
                    else
                    {
                        ViewBag.AdditionalFee2 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee2);
                    }
                    ViewBag.AdditionalFee3CurrencyType = doInitData.doQuotationBasic.AdditionalFee3CurrencyType;
                    if (doInitData.doQuotationBasic.AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        ViewBag.AdditionalFee3 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee3Usd);
                    }
                    else
                    {
                        ViewBag.AdditionalFee3 = CommonUtil.TextNumeric(doInitData.doQuotationBasic.AdditionalFee3);
                    }
                    ViewBag.AdditionalApproveNo1 = doInitData.doQuotationBasic.AdditionalApproveNo1;
                    ViewBag.AdditionalApproveNo2 = doInitData.doQuotationBasic.AdditionalApproveNo2;
                    ViewBag.AdditionalApproveNo3 = doInitData.doQuotationBasic.AdditionalApproveNo3;
                    ViewBag.ApproveNo1 = doInitData.doQuotationBasic.ApproveNo1;
                    ViewBag.ApproveNo2 = doInitData.doQuotationBasic.ApproveNo2;
                    ViewBag.ApproveNo3 = doInitData.doQuotationBasic.ApproveNo3;
                    ViewBag.ApproveNo4 = doInitData.doQuotationBasic.ApproveNo4;
                    ViewBag.ApproveNo5 = doInitData.doQuotationBasic.ApproveNo5;

                    if (isImportData)
                    {
                        ViewBag.DisabledProductCode =
                            !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ProductCode) &&
                            !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ProductName);

                        ViewBag.DisabledMaintenanceFee1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceFee1);
                        ViewBag.DisabledMaintenanceFee1CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceFee1CurrencyType);
                        ViewBag.DisabledAdditionalFee1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee1);
                        ViewBag.DisabledAdditionalFee2 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee2);
                        ViewBag.DisabledAdditionalFee3 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee3);
                        ViewBag.DisabledAdditionalFee1CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee1CurrencyType);
                        ViewBag.DisabledAdditionalFee2CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee2CurrencyType);
                        ViewBag.DisabledAdditionalFee3CurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalFee3CurrencyType);
                        ViewBag.DisabledAdditionalApproveNo1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalApproveNo1);
                        ViewBag.DisabledAdditionalApproveNo2 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalApproveNo2);
                        ViewBag.DisabledAdditionalApproveNo3 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.AdditionalApproveNo3);
                        ViewBag.DisabledApproveNo1 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo1);
                        ViewBag.DisabledApproveNo2 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo2);
                        ViewBag.DisabledApproveNo3 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo3);
                        ViewBag.DisabledApproveNo4 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo4);
                        ViewBag.DisabledApproveNo5 = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ApproveNo5);
                    }
                    else if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                    {
                        ViewBag.DisabledProductCode = true;
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_05");
        }
        /// <summary>
        /// Generate maintenance detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_06()
        {
            try
            {
                ViewBag.DisabledMaintenanceTargetProductTypeCode = false;
                ViewBag.DisabledMaintenanceTypeCode = false;
                ViewBag.DisabledMaintenanceCycle = false;
                ViewBag.DisabledMaintenanceMemo = false;
                ViewBag.DisabledMaintenanceTargetContract = true;
                ViewBag.DisabledMaintenanceTargetContractDetail = true;
                ViewBag.DisabledMaintenanceTargetContractRemove = true;

                /* --- Get Default Maintenance Cycle --- */
                /* ------------------------------------- */
                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doSystemConfig> lst = chandler.GetSystemConfig(ConfigName.C_CONFIG_DEFAULT_MA_CYCLE);
                if (lst.Count > 0)
                    ViewBag.MaintenanceCycle = lst[0].ConfigValue;
                /* ------------------------------------- */

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion

                if (doInitData != null)
                {
                    if (doInitData.doQuotationBasic != null)
                    {
                        ViewBag.MaintenanceTargetProductTypeCode = doInitData.doQuotationBasic.MaintenanceTargetProductTypeCode;
                        ViewBag.MaintenanceTypeCode = doInitData.doQuotationBasic.MaintenanceTypeCode;

                        /* --- *** --- */
                        /* ViewBag.MaintenanceCycle = doInitData.doQuotationBasic.MaintenanceCycle; */
                        if (CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycle) == false
                            && CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycleName))
                        {
                        }
                        else
                        {
                            ViewBag.MaintenanceCycle = doInitData.doQuotationBasic.MaintenanceCycle.ToString();
                        }

                        ViewBag.MaintenanceMemo = doInitData.doQuotationBasic.MaintenanceMemo;
                    }

                    if (isImportData)
                    {
                        ViewBag.DisabledMaintenanceTargetProductTypeCode =
                            !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceTargetProductTypeCode) &&
                            !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceTargetProductTypeName);

                        ViewBag.DisabledMaintenanceTypeCode =
                            !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceTypeCode) &&
                            !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceTypeName);

                        ViewBag.DisabledMaintenanceCycle =
                            !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycle) &&
                            !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceCycleName);

                        ViewBag.DisabledMaintenanceMemo = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.MaintenanceMemo);


                        bool isDisabled = true;
                        if (doInitData.doQuotationBasic.MaintenanceTargetProductTypeCode == MaintenanceTargetProductType.C_MA_TARGET_PROD_TYPE_SECOM)
                        {
                            if (doInitData.MaintenanceTargetList != null)
                            {
                                if (doInitData.MaintenanceTargetList.Count > 0)
                                    isDisabled = false;
                            }
                        }
                        ViewBag.DisabledMaintenanceTargetContractDetail = isDisabled;
                    }
                    else if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                    {
                        ViewBag.DisabledMaintenanceTargetProductTypeCode = true;

                        bool isDisabled = true;
                        if (doInitData.doQuotationBasic.MaintenanceTargetProductTypeCode == MaintenanceTargetProductType.C_MA_TARGET_PROD_TYPE_SECOM)
                            isDisabled = false;

                        ViewBag.DisabledMaintenanceTargetContract = isDisabled;
                        ViewBag.DisabledMaintenanceTargetContractDetail = isDisabled;
                        ViewBag.DisabledMaintenanceTargetContractRemove = isDisabled;
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_06");
        }
        /// <summary>
        /// Generate instrument detail in case of sale or  alarm (before 1st complete installation) section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_07()
        {
            try
            {
                ViewBag.IsDefaultInstrument = false;
                ViewBag.DisabledAllControls = false;
                ViewBag.DisabledRemoveButton = false;
                ViewBag.DisabledQuantityTextBox = false;

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion

                if (doInitData != null)
                {
                    if (isImportData)
                    {
                        ViewBag.DisabledAllControls = true;
                        ViewBag.DisabledRemoveButton = true;
                        ViewBag.DisabledQuantityTextBox = true;
                    }

                    if (doInitData.doQuotationHeaderData != null)
                    {
                        if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                        {
                            if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                            {
                                if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode ==
                                    SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE
                                    && (doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode ==
                                    SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL
                                    || doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode ==
                                    SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_RENTAL_SALE))
                                {
                                    if (isImportData == false)
                                        ViewBag.DisabledQuantityTextBox = false;
                                    else
                                        ViewBag.IsDefaultInstrument = true;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_07");
        }
        /// <summary>
        /// Generate instrument detail in case of alarm (after 1st complete installation) section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_08()
        {
            try
            {
                ViewBag.DisabledAllControls = false;
                ViewBag.DisabledRemoveButton = false;
                ViewBag.DisabledQuantityTextBox = false;

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion

                if (doInitData != null)
                {
                    if (isImportData)
                    {
                        ViewBag.DisabledAllControls = true;
                        ViewBag.DisabledRemoveButton = true;
                        ViewBag.DisabledQuantityTextBox = true;
                    }
                    else
                    {
                        ViewBag.DisabledRemoveButton = true;
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_08");
        }
        /// <summary>
        /// Generate instrument detail in case of sale online section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_09()
        {
            try
            {
                doInitQuotationData doInitData = null;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;

                #endregion

                if (doInitData != null)
                {
                    /* --- Merge --- */
                    /* if (doInitData.doLinkageSaleContractData != null)
                        ViewBag.SaleOnlineContractCode = doInitData.doQuotationBasic.SaleOnlineContractCodeShort; */
                    ViewBag.SaleOnlineContractCode = doInitData.doQuotationBasic.SaleOnlineContractCodeShort;
                    /* ------------- */
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_09");
        }
        /// <summary>
        /// Generate facility detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_10()
        {
            try
            {
                ViewBag.DisabledAllFacilityControls = false;
                ViewBag.DisabledFacilityRemoveButton = false;
                ViewBag.DisabledFacilityQuantityTextBox = false;
                ViewBag.DisabledFacilityMemo = false;

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion

                if (doInitData != null)
                {
                    ViewBag.FacilityMemo = doInitData.doQuotationBasic.FacilityMemo;

                    if (isImportData)
                    {
                        ViewBag.DisabledAllFacilityControls = true;
                        ViewBag.DisabledFacilityRemoveButton = true;
                        ViewBag.DisabledFacilityQuantityTextBox = true;

                        ViewBag.DisabledFacilityMemo = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.FacilityMemo);
                    }
                    else if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode != TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                    {
                        ViewBag.DisabledFacilityRemoveButton = true;
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_10");
        }
        /// <summary>
        /// Generate beat guard detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_11()
        {
            try
            {
                /* --- Merge --- */
                ViewBag.DisabledNumOfDayTimeWd = false;
                ViewBag.DisabledNumOfNightTimeWd = false;
                ViewBag.DisabledNumOfDayTimeSat = false;
                ViewBag.DisabledNumOfNightTimeSat = false;
                ViewBag.DisabledNumOfDayTimeSun = false;
                ViewBag.DisabledNumOfNightTimeSun = false;
                ViewBag.DisabledNumOfBeatStep = false;
                ViewBag.DisabledFreqOfGateUsage = false;
                ViewBag.DisabledNumOfClockKey = false;
                ViewBag.DisabledNumOfDate = false;
                ViewBag.DisabledNotifyTime = false;
                /* ------------- */

                doInitQuotationData doInitData = null;

                /* --- Merge --- */
                bool isImportData = false;
                /* ------------- */

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;

                /* --- Merge --- */
                /* if (param != null)
                    doInitData = param.InitialData; */
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }
                /* ------------- */

                #endregion
                ViewBag.NumOfDayTimeWd = "0";
                ViewBag.NumOfNightTimeWd = "0";
                ViewBag.NumOfDayTimeSat = "0";
                ViewBag.NumOfNightTimeSat = "0";
                ViewBag.NumOfDayTimeSun = "0";
                ViewBag.NumOfNightTimeSun = "0";
                ViewBag.NumOfBeatStep = "0";
                ViewBag.NumOfDate = "30.4";

                if (doInitData != null)
                {
                    if (doInitData.doBeatGuardDetail != null)
                    {
                        ViewBag.NumOfDayTimeWd = CommonUtil.TextNumeric(doInitData.doBeatGuardDetail.NumOfDayTimeWd, 0);
                        ViewBag.NumOfNightTimeWd = CommonUtil.TextNumeric(doInitData.doBeatGuardDetail.NumOfNightTimeWd, 0);
                        ViewBag.NumOfDayTimeSat = CommonUtil.TextNumeric(doInitData.doBeatGuardDetail.NumOfDayTimeSat, 0);
                        ViewBag.NumOfNightTimeSat = CommonUtil.TextNumeric(doInitData.doBeatGuardDetail.NumOfNightTimeSat, 0);
                        ViewBag.NumOfDayTimeSun = CommonUtil.TextNumeric(doInitData.doBeatGuardDetail.NumOfDayTimeSun, 0);
                        ViewBag.NumOfNightTimeSun = CommonUtil.TextNumeric(doInitData.doBeatGuardDetail.NumOfNightTimeSun, 0);
                        ViewBag.NumOfBeatStep = CommonUtil.TextNumeric(doInitData.doBeatGuardDetail.NumOfBeatStep, 0);
                        ViewBag.FreqOfGateUsage = CommonUtil.TextNumeric(doInitData.doBeatGuardDetail.FreqOfGateUsage, 0);
                        ViewBag.NumOfClockKey = CommonUtil.TextNumeric(doInitData.doBeatGuardDetail.NumOfClockKey, 0);

                        if (doInitData.doBeatGuardDetail.NumOfDate != null)
                            ViewBag.NumOfDate = doInitData.doBeatGuardDetail.NumOfDate.Value.ToString();

                        ViewBag.NotifyTime = CommonUtil.TextTime(doInitData.doBeatGuardDetail.NotifyTime);

                        /* --- Merge --- */
                        if (isImportData)
                        {
                            ViewBag.DisabledNumOfDayTimeWd = !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.NumOfDayTimeWd);
                            ViewBag.DisabledNumOfNightTimeWd = !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.NumOfNightTimeWd);
                            ViewBag.DisabledNumOfDayTimeSat = !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.NumOfDayTimeSat);
                            ViewBag.DisabledNumOfNightTimeSat = !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.NumOfNightTimeSat);
                            ViewBag.DisabledNumOfDayTimeSun = !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.NumOfDayTimeSun);
                            ViewBag.DisabledNumOfNightTimeSun = !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.NumOfNightTimeSun);
                            ViewBag.DisabledNumOfBeatStep = !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.NumOfBeatStep);
                            ViewBag.DisabledFreqOfGateUsage = !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.FreqOfGateUsage);
                            ViewBag.DisabledNumOfClockKey = !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.NumOfClockKey);
                            ViewBag.DisabledNumOfDate = !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.NumOfDate)
                                && !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.NumOfDateName);
                            ViewBag.DisabledNotifyTime = !CommonUtil.IsNullOrEmpty(doInitData.doBeatGuardDetail.NotifyTime);
                        }
                        /* ------------- */
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_11");
        }
        /// <summary>
        /// Generate sentry guard detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_12()
        {
            try
            {
                ViewBag.DisabledSecurityItemFee = false;
                ViewBag.DisabledOtherItemFee = false;
                ViewBag.DisabledSentryGuardAreaTypeCode = false;
                ViewBag.DisabledEditSentryGuardButton = false;
                ViewBag.DisabledRemoveSentryButton = false;
                ViewBag.DisabledSentryGuardDetail = false;

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion
                ViewBag.SentryGuardTypeCode = "1";
                ViewBag.NumOfDate = "30.4";

                ViewBag.SecurityStartTime = "00:00";
                ViewBag.SecurityFinishTime = "00:00";
                ViewBag.WorkHourPerMonth = "0.0";


                if (doInitData != null)
                {
                    if (doInitData.doQuotationBasic != null)
                    {
                        ViewBag.SecurityItemFeeCurrencyType = doInitData.doQuotationBasic.SecurityItemFeeCurrencyType;
                        if (doInitData.doQuotationBasic.SecurityItemFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.SecurityItemFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.SecurityItemFeeUsd);
                        }
                        else
                        {
                            ViewBag.SecurityItemFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.SecurityItemFee);
                        }

                        ViewBag.OtherItemFeeCurrencyType = doInitData.doQuotationBasic.OtherItemFeeCurrencyType;
                        if (doInitData.doQuotationBasic.OtherItemFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.OtherItemFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.OtherItemFeeUsd);
                        }
                        else
                        {
                            ViewBag.OtherItemFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.OtherItemFee);
                        }

                        ViewBag.SentryGuardAreaTypeCode = doInitData.doQuotationBasic.SentryGuardAreaTypeCode;

                        if (isImportData)
                        {
                            ViewBag.DisabledSecurityItemFee = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SecurityItemFee);
                            ViewBag.DisabledSecurityItemFeeCurrencyType = doInitData.doQuotationBasic.SecurityItemFeeCurrencyType;
                            ViewBag.DisabledOtherItemFee = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.OtherItemFee);
                            ViewBag.DisabledOtherItemFeeCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.OtherItemFeeCurrencyType);

                            ViewBag.DisabledSentryGuardAreaTypeCode =
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SentryGuardAreaTypeCode) &&
                                !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.SentryGuardAreaTypeName);

                            ViewBag.DisabledEditSentryGuardButton = true;
                            ViewBag.DisabledRemoveSentryButton = true;
                            ViewBag.DisabledSentryGuardDetail = true;
                        }
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_12");
        }
        /// <summary>
        /// Generate fee information in case of sale section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_13()
        {
            try
            {
                ViewBag.DisabledProductPrice = false;
                ViewBag.DisabledInstallationFee = false;
                ViewBag.DisabledProductPriceCurrencyType = false;
                ViewBag.DisabledInstallationFeeCurrencyType = false;

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion

                if (isImportData && doInitData != null)
                {
                    ViewBag.ProductPriceCurrencyType = doInitData.doQuotationBasic.ProductPriceCurrencyType;
                    if (doInitData.doQuotationBasic.ProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        ViewBag.ProductPrice = CommonUtil.TextNumeric(doInitData.doQuotationBasic.ProductPriceUsd);
                    }
                    else
                    {
                        ViewBag.ProductPrice = CommonUtil.TextNumeric(doInitData.doQuotationBasic.ProductPrice);
                    }

                    ViewBag.InstallationFeeCurrencyType = doInitData.doQuotationBasic.InstallationFeeCurrencyType;
                    if (doInitData.doQuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        ViewBag.InstallationFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.InstallationFeeUsd);
                    }
                    else
                    {
                        ViewBag.InstallationFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.InstallationFee);
                    }

                    ViewBag.DisabledProductPrice = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ProductPrice);
                    ViewBag.DisabledInstallationFee = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InstallationFee);
                    ViewBag.DisabledProductPriceCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ProductPriceCurrencyType);
                    ViewBag.DisabledInstallationFeeCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InstallationFeeCurrencyType);
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_13");
        }
        /// <summary>
        /// Generate fee information in case of alarm section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_14()
        {
            try
            {
                ViewBag.DisabledContractFee = false;
                ViewBag.DisabledInstallationFee = false;
                ViewBag.DisabledDepositFee = false;
                ViewBag.DisabledContractFeeCurrencyType = false;
                ViewBag.DisabledInstallationFeeCurrencyType = false;
                ViewBag.DisabledDepositFeeCurrencyType = false;

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion

                if (doInitData != null)
                {
                    if (isImportData)
                    {

                        ViewBag.ContractFeeCurrencyType = doInitData.doQuotationBasic.ContractFeeCurrencyType;
                        if (doInitData.doQuotationBasic.ContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.ContractFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.ContractFeeUsd);
                        }
                        else
                        {
                            ViewBag.ContractFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.ContractFee);
                        }

                        ViewBag.InstallationFeeCurrencyType = doInitData.doQuotationBasic.InstallationFeeCurrencyType;
                        if (doInitData.doQuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.InstallationFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.InstallationFeeUsd);
                        }
                        else
                        {
                            ViewBag.InstallationFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.InstallationFee);
                        }

                        ViewBag.DepositFeeCurrencyType = doInitData.doQuotationBasic.DepositFeeCurrencyType;
                        if (doInitData.doQuotationBasic.DepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.DepositFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.DepositFeeUsd);
                        }
                        else
                        {
                            ViewBag.DepositFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.DepositFee);
                        }

                        ViewBag.DisabledContractFee = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ContractFee);
                        ViewBag.DisabledInstallationFee = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InstallationFee);
                        ViewBag.DisabledDepositFee = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.DepositFee);
                        ViewBag.DisabledContractFeeCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ContractFeeCurrencyType);
                        ViewBag.DisabledInstallationFeeCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.InstallationFeeCurrencyType);
                        ViewBag.DisabledDepositFeeCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.DepositFeeCurrencyType);
                    }

                    if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                    {
                        ViewBag.DisabledDepositFee = true;
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_14");
        }
        /// <summary>
        /// Generate fee information in case of sale online, beat guard, sentry guard, or maintenance section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_15()
        {
            try
            {
                ViewBag.DisabledContractFee = false;
                ViewBag.DisabledDepositFee = false;
                ViewBag.DisabledContractFeeCurrencyType = false;
                ViewBag.DisabledDepositFeeCurrencyType = false;

                doInitQuotationData doInitData = null;
                bool isImportData = false;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doInitData = param.InitialData;
                    isImportData = (param.ImportKey != null);
                }

                #endregion

                if (doInitData != null)
                {
                    if (isImportData)
                    {
                        ViewBag.ContractFeeCurrencyType = doInitData.doQuotationBasic.ContractFeeCurrencyType;
                        if (doInitData.doQuotationBasic.ContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.ContractFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.ContractFeeUsd);
                        }
                        else
                        {
                            ViewBag.ContractFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.ContractFee);
                        }
                        ViewBag.DepositFeeCurrencyType = doInitData.doQuotationBasic.DepositFeeCurrencyType;
                        if (doInitData.doQuotationBasic.DepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.DepositFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.DepositFeeUsd);
                        }
                        else
                        {
                            ViewBag.DepositFee = CommonUtil.TextNumeric(doInitData.doQuotationBasic.DepositFee);
                        }

                        ViewBag.DisabledContractFee = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ContractFee);
                        ViewBag.DisabledDepositFee = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.DepositFee);
                        ViewBag.DisabledContractFeeCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.ContractFeeCurrencyType);
                        ViewBag.DisabledDepositFeeCurrencyType = !CommonUtil.IsNullOrEmpty(doInitData.doQuotationBasic.DepositFeeCurrencyType);
                    }

                    if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                    {
                        ViewBag.DisabledDepositFee = true;
                    }
                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_15");
        }
        /// <summary>
        /// Generate register quotation detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_16()
        {
            try
            {
                doQuotationKey doQuotationKey = null;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    doQuotationKey = param.QuotationKey;

                    param.QuotationKey = null;
                    param.ImportKey = null;
                }

                #endregion

                if (doQuotationKey != null)
                {
                    CommonUtil cmm = new CommonUtil();
                    ViewBag.QuotationTargetCode = cmm.ConvertQuotationTargetCode(doQuotationKey.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.Alphabet = doQuotationKey.Alphabet;


                }
            }
            catch
            {
            }

            return View("QUS030/_QUS030_16");
        }

        #endregion
        #region Actions

        /// <summary>
        /// Retrieve quotation target data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult QUS030_RetrieveQuotationTargetData(QUS030_doGetQuotationDataCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param == null)
                    param = new QUS030_ScreenParameter();

                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                CommonUtil cmm = new CommonUtil();
                cond.QuotationTargetCode = cmm.ConvertQuotationTargetCode(cond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                doInitQuotationData initData = InitQuotationData(res, cond);
                if (res.IsError)
                    return Json(res);

                #region Update Session

                param.InitialData = initData;
                QUS030_ScreenData = param;

                #endregion

                res.ResultData = CommonUtil.CloneObject<doInitQuotationData, QUS030_doInitQuotationData>(initData);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Initial quotation data from import file
        /// </summary>
        /// <param name="ImportKey"></param>
        /// <returns></returns>
        public ActionResult QUS030_ImportQuotationData(string ImportKey)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CommonUtil cmm = new CommonUtil();
                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param == null)
                    param = new QUS030_ScreenParameter();

                #region Get Import Section

                dsImportData importData = QUS050_GetImportData(ImportKey);
                if (importData == null)
                    importData = new dsImportData();
                if (param.ImportKey == ImportKey
                    && param.QuotationKey != null)
                {
                    importData.QuotationTargetCode = param.QuotationKey.QuotationTargetCode;
                }

                #endregion
                #region Validate Data

                QUS030_ImportQuotationData template = new QUS030_ImportQuotationData();
                if (importData.QuotationTargetCode == null)
                {
                    if (importData.dtTbt_QuotationTarget.Count > 0)
                        template.QuotationTargetCode = importData.dtTbt_QuotationTarget[0].QuotationTargetCode;
                }
                else
                    template.QuotationTargetCode = importData.QuotationTargetCode;

                template.QuotationTargetCode = cmm.ConvertQuotationTargetCode(template.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                ValidatorUtil.BuildErrorMessage(res, new object[] { template });
                if (res.IsError)
                    return Json(res);

                #endregion
                #region Create Init Quotation Data

                doGetQuotationDataCondition cond = new doGetQuotationDataCondition();

                if (importData.QuotationTargetCode == null)
                {
                    if (importData.dtTbt_QuotationTarget.Count > 0)
                        cond.QuotationTargetCode = importData.dtTbt_QuotationTarget[0].QuotationTargetCode;
                }
                else
                    cond.QuotationTargetCode = importData.QuotationTargetCode;

                cond.QuotationTargetCode = cmm.ConvertQuotationTargetCode(cond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                doInitQuotationData initData = InitQuotationData(res, cond);
                if (res.IsError)
                {
                    /* --- Merge --- */
                    /* res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK; */
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    /* ------------- */

                    return Json(res);
                }

                #endregion

                bool hasProductCode = false;
                if (importData.dtTbt_QuotationBasic != null)
                {
                    if (importData.dtTbt_QuotationBasic.Count > 0)
                    {
                        if (CommonUtil.IsNullOrEmpty(importData.dtTbt_QuotationBasic[0].ProductCode) == false)
                            hasProductCode = true;
                    }
                }
                if (hasProductCode)
                {
                    if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_SALE)
                        InitImportSaleQuotation(res, initData, importData);
                    else if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                        || initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                        InitImportALQuotation(res, initData, importData);
                    else if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                        InitImportSaleOnlineQuotation(res, initData, importData);
                    else if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_BE)
                        InitImportBEQuotation(res, initData, importData);
                    else if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                        InitImportSGQuotation(res, initData, importData);
                    else if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                        InitImportMAQuotation(res, initData, importData);
                }

                #region Mapping Data in Quotation Basic

                if (initData.doQuotationBasic != null)
                {
                    #region Set Product Name

                    IProductMasterHandler mhandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    IEmployeeMasterHandler empHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                    if (CommonUtil.IsNullOrEmpty(initData.doQuotationBasic.ProductCode) == false)
                    {
                        string pName = null;
                        if (CheckUserPermission(ScreenID.C_SCREEN_ID_QTN_DETAIL, FunctionID.C_FUNC_ID_PLANNER) == true)
                        {
                            List<View_tbm_Product> pLst = mhandler.GetTbm_ProductByLanguage(
                                            null,
                                            initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode);
                            if (pLst.Count > 0)
                            {
                                foreach (View_tbm_Product p in pLst)
                                {
                                    if (p.ProductCode == initData.doQuotationBasic.ProductCode)
                                    {
                                        pName = p.ProductName;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            List<View_tbm_Product> pLst = mhandler.GetActiveProductbyLanguage(
                                                initData.PriorProductCode,
                                                initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode);

                            foreach (View_tbm_Product p in pLst)
                            {
                                if (p.ProductCode == initData.doQuotationBasic.ProductCode)
                                {
                                    pName = p.ProductName;
                                    break;
                                }
                            }
                        }

                        if (pName != null)
                            initData.doQuotationBasic.ProductName = pName;
                    }

                    #endregion
                    #region Set Misc Name

                    MiscTypeMappingList miscLst = new MiscTypeMappingList();
                    miscLst.AddMiscType(initData.doQuotationBasic);
                    if (initData.doBeatGuardDetail != null)
                    {
                        miscLst.AddMiscType(initData.doBeatGuardDetail);
                    }

                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    chandler.MiscTypeMappingList(miscLst);

                    #endregion
                    #region Mapping Employee Name

                    EmployeeMappingList empLst = new EmployeeMappingList();
                    empLst.AddEmployee(initData.doQuotationBasic);

                    IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;


                    /* --- Merge --- */
                    /* ehandler.EmployeeListMapping(empLst); */
                    ehandler.ActiveEmployeeListMapping(empLst);
                    /* ------------- */

                    #endregion
                }

                #endregion

                bool isError = false;
                if (res.IsError)
                    isError = true;

                #region Check Field that has value but can not mapping

                List<object> objLst = new List<object>();
                if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_SALE)
                    objLst.Add(CommonUtil.CloneObject<tbt_QuotationBasic, QUS030_tbt_QuotationBasic_ImportNull>(initData.doQuotationBasic));
                else if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                    objLst.Add(CommonUtil.CloneObject<tbt_QuotationBasic, QUS030_tbt_QuotationBasic_AL_ImportNull>(initData.doQuotationBasic));
                else if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                    objLst.Add(CommonUtil.CloneObject<tbt_QuotationBasic, QUS030_tbt_QuotationBasic_RENTAL_SALE_ImportNull>(initData.doQuotationBasic));
                else if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                    objLst.Add(CommonUtil.CloneObject<tbt_QuotationBasic, QUS030_tbt_QuotationBasic_ONLINE_ImportNull>(initData.doQuotationBasic));
                else if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_BE)
                {
                    objLst.Add(CommonUtil.CloneObject<tbt_QuotationBasic, QUS030_tbt_QuotationBasic_BE_ImportNull>(initData.doQuotationBasic));
                    objLst.Add(CommonUtil.CloneObject<doBeatGuardDetail, QUS030_doBeatGuardDetail_ImportNull>(initData.doBeatGuardDetail));
                }
                else if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                    objLst.Add(CommonUtil.CloneObject<tbt_QuotationBasic, QUS030_tbt_QuotationBasic_SG_ImportNull>(initData.doQuotationBasic));
                else if (initData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                    objLst.Add(CommonUtil.CloneObject<tbt_QuotationBasic, QUS030_tbt_QuotationBasic_MA_ImportNull>(initData.doQuotationBasic));

                ValidatorUtil.BuildErrorMessage(res, objLst.ToArray(), null, false);

                #endregion
                #region Update Session

                param.InitialData = initData;
                param.ImportKey = hasProductCode ? ImportKey : null;

                QUS030_ScreenData = param;

                #endregion

                QUS030_doInitQuotationData result = CommonUtil.CloneObject<doInitQuotationData, QUS030_doInitQuotationData>(initData);
                result.IsEnableRegister = isError;

                res.ResultData = result;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        //public ActionResult QUS030_GetEmployeeName(string empNo)
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
        //            List<doActiveEmployeeList> lst = handler.GetActiveEmployeeList(empLst);
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
        /// Event when click comfirm
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_RegisterQuotationData()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                doRegisterQuotationData doRegisterQuotationData = null;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doRegisterQuotationData = param.RegisterData;

                #endregion
                #region Check is Suspending

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                #endregion

                using (TransactionScope scope = new TransactionScope())
                {
                    IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

                    #region Create Quotation Target, if not found

                    /* --- Get Quotation Target --- */
                    doGetQuotationDataCondition cond = new doGetQuotationDataCondition()
                    {
                        QuotationTargetCode = doRegisterQuotationData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode
                    };
                    List<tbt_QuotationTarget> qLst = qhandler.GetTbt_QuotationTarget(cond);
                    if (qLst.Count <= 0)
                    {
                        doRegisterQuotationTargetData rQTData = new doRegisterQuotationTargetData();
                        rQTData.doTbt_QuotationTarget = CommonUtil.CloneObject<doQuotationTarget, tbt_QuotationTarget>(doRegisterQuotationData.doQuotationHeaderData.doQuotationTarget);

                        if (doRegisterQuotationData.doQuotationHeaderData.doContractTarget != null)
                        {
                            rQTData.doTbt_QuotationCustomer1 = new tbt_QuotationCustomer()
                            {
                                QuotationTargetCode = doRegisterQuotationData.doQuotationHeaderData.doContractTarget.QuotationTargetCode,
                                CustPartTypeCode = doRegisterQuotationData.doQuotationHeaderData.doContractTarget.CustPartTypeCode,
                                CustCode = doRegisterQuotationData.doQuotationHeaderData.doContractTarget.CustCode
                            };
                        }

                        if (doRegisterQuotationData.doQuotationHeaderData.doRealCustomer != null)
                        {
                            rQTData.doTbt_QuotationCustomer2 = new tbt_QuotationCustomer()
                            {
                                QuotationTargetCode = doRegisterQuotationData.doQuotationHeaderData.doRealCustomer.QuotationTargetCode,
                                CustPartTypeCode = doRegisterQuotationData.doQuotationHeaderData.doRealCustomer.CustPartTypeCode,
                                CustCode = doRegisterQuotationData.doQuotationHeaderData.doRealCustomer.CustCode
                            };
                        }

                        if (doRegisterQuotationData.doQuotationHeaderData.doQuotationSite != null)
                        {
                            rQTData.doTbt_QuotationSite = new tbt_QuotationSite()
                            {
                                QuotationTargetCode = doRegisterQuotationData.doQuotationHeaderData.doQuotationSite.QuotationTargetCode,
                                SiteCode = doRegisterQuotationData.doQuotationHeaderData.doQuotationSite.SiteCode,
                                SiteNo = doRegisterQuotationData.doQuotationHeaderData.doQuotationSite.SiteNo
                            };
                        }

                        qhandler.CreateQuotationTargetData(rQTData);
                    }

                    #endregion
                    #region Generate Alphabet

                    doRegisterQuotationData.doTbt_QuotationBasic.Alphabet =
                        qhandler.GenerateQuotationAlphabet(doRegisterQuotationData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode);

                    #endregion
                    #region Insert Quotation Basic

                    qhandler.InsertQuotationBasic(doRegisterQuotationData.doTbt_QuotationBasic);

                    #endregion
                    #region Insert Operation Type

                    if (doRegisterQuotationData.OperationList != null)
                    {
                        foreach (tbt_QuotationOperationType oType in doRegisterQuotationData.OperationList)
                        {
                            oType.Alphabet = doRegisterQuotationData.doTbt_QuotationBasic.Alphabet;
                            qhandler.InsertQuotationOperationType(oType);
                        }
                    }

                    #endregion
                    #region Insert Instrument

                    if (doRegisterQuotationData.InstrumentList != null)
                    {
                        foreach (tbt_QuotationInstrumentDetails inst in doRegisterQuotationData.InstrumentList)
                        {
                            inst.Alphabet = doRegisterQuotationData.doTbt_QuotationBasic.Alphabet;
                            qhandler.InsertQuotationInstrumentDetails(inst);
                        }
                    }

                    #endregion
                    #region Insert Facility

                    if (doRegisterQuotationData.FacilityList != null)
                    {
                        foreach (tbt_QuotationFacilityDetails fac in doRegisterQuotationData.FacilityList)
                        {
                            fac.Alphabet = doRegisterQuotationData.doTbt_QuotationBasic.Alphabet;
                            qhandler.InsertQuotationFacilityDetails(fac);
                        }
                    }

                    #endregion
                    #region Insert Beat Guard

                    if (doRegisterQuotationData.doTbt_QuotationBeatGuardDetails != null)
                    {
                        doRegisterQuotationData.doTbt_QuotationBeatGuardDetails.Alphabet = doRegisterQuotationData.doTbt_QuotationBasic.Alphabet;
                        qhandler.InsertQuotationBeatGuardDetails(doRegisterQuotationData.doTbt_QuotationBeatGuardDetails);
                    }

                    #endregion
                    #region Sentry Guard

                    if (doRegisterQuotationData.SentryGuardList != null)
                    {
                        foreach (tbt_QuotationSentryGuardDetails sen in doRegisterQuotationData.SentryGuardList)
                        {
                            sen.Alphabet = doRegisterQuotationData.doTbt_QuotationBasic.Alphabet;
                            qhandler.InsertQuotationSentryGuardDetails(sen);
                        }
                    }

                    #endregion
                    #region Maintenance Linkage

                    if (doRegisterQuotationData.MaintenanceList != null)
                    {
                        foreach (tbt_QuotationMaintenanceLinkage man in doRegisterQuotationData.MaintenanceList)
                        {
                            man.Alphabet = doRegisterQuotationData.doTbt_QuotationBasic.Alphabet;
                            qhandler.InsertQuotationMaintenanceLinkage(man);
                        }
                    }

                    #endregion

                    #region Quotation Installation Detail

                    if (doRegisterQuotationData.doTbt_QuotationInstallationDetail != null)
                    {
                        doRegisterQuotationData.doTbt_QuotationInstallationDetail.QuotationTargetCode = doRegisterQuotationData.doTbt_QuotationBasic.QuotationTargetCode;
                        doRegisterQuotationData.doTbt_QuotationInstallationDetail.Alphabet = doRegisterQuotationData.doTbt_QuotationBasic.Alphabet;
                        var tmp = qhandler.GetTbt_QuotationInstallationDetail(
                            doRegisterQuotationData.doTbt_QuotationInstallationDetail.QuotationTargetCode,
                            doRegisterQuotationData.doTbt_QuotationInstallationDetail.Alphabet
                        );
                        if (tmp != null && tmp.Count > 0)
                        {
                            qhandler.UpdateQuotationInstallationDetail(new List<tbt_QuotationInstallationDetail>() { doRegisterQuotationData.doTbt_QuotationInstallationDetail });
                        }
                        else
                        {
                            qhandler.InsertQuotationInstallationDetail(new List<tbt_QuotationInstallationDetail>() { doRegisterQuotationData.doTbt_QuotationInstallationDetail });
                        }
                    }

                    #endregion

                    #region Update Last Alphabet in Quotation Target

                    doUpdateQuotationTargetData uT = new doUpdateQuotationTargetData()
                    {
                        QuotationTargetCode = doRegisterQuotationData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode,
                        LastAlphabet = doRegisterQuotationData.doTbt_QuotationBasic.Alphabet
                    };
                    if (qhandler.UpdateQuotationTarget(uT) > 0)
                    {
                        doQuotationKey doQuotationKey = new doQuotationKey()
                        {
                            QuotationTargetCode = doRegisterQuotationData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode,
                            Alphabet = doRegisterQuotationData.doTbt_QuotationBasic.Alphabet
                        };

                        #region Update Session

                        param.QuotationKey = doQuotationKey;
                        QUS030_ScreenData = param;

                        #endregion

                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
                        scope.Complete();
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Remove data from session
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_ClearSession()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.ImportKey != null)
                        UpdateScreenObject(null, param.ImportKey);
                }

                UpdateScreenObject(null);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Clear import data from session
        /// </summary>
        public void QUS030_ClearImportKey()
        {
            try
            {
                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.ImportKey != null)
                        UpdateScreenObject(null, param.ImportKey);

                    param.QuotationKey = null;
                    param.ImportKey = null;
                }
            }
            catch
            {
            }
        }


        #endregion
        #region Methods

        /// <summary>
        /// Get/Set data from session
        /// </summary>
        private QUS030_ScreenParameter QUS030_ScreenData
        {
            get
            {
                return GetScreenObject<QUS030_ScreenParameter>();
            }
            set
            {
                UpdateScreenObject(value);
            }
        }
        /// <summary>
        /// Validate product data
        /// </summary>
        /// <param name="res"></param>
        /// <param name="quotationBasic"></param>
        private void ValidateProduct(ObjectResultData res, tbt_QuotationBasic quotationBasic)
        {
            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;
                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;

                #endregion

                if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                {
                    if ((doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode != ProductType.C_PROD_TYPE_AL
                        && doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode != ProductType.C_PROD_TYPE_RENTAL_SALE)
                        || ((doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                                || doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                        && doInitData.FirstInstallCompleteFlag == true
                        && doInitData.StartOperationFlag == false))
                    {
                        if (doInitData.doQuotationBasic.ProductCode != doInitData.PriorProductCode)
                        {
                            /* --- Merge --- */
                            /* res.AddErrorMessage(
                                MessageUtil.MODULE_QUOTATION, 
                                MessageUtil.MessageList.MSG2073,
                                new string[] { doInitData.PriorProductCode });
                            return; */
                            string code = doInitData.PriorProductCode;
                            IProductMasterHandler mhandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                            List<View_tbm_Product> mpLst = mhandler.GetTbm_ProductByLanguage(doInitData.PriorProductCode, null);
                            if (mpLst.Count > 0)
                                code = CommonUtil.TextCodeName(code, mpLst[0].ProductName);

                            res.AddErrorMessage(
                                MessageUtil.MODULE_QUOTATION,
                                MessageUtil.MessageList.MSG2073,
                                new string[] { code });
                            return;
                            /* ------------- */

                        }
                    }
                }

                IProductMasterHandler handler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                List<tbm_Product> pLst = handler.GetTbm_Product(quotationBasic.ProductCode, null);
                if (pLst.Count <= 0)
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2080,

                        /* --- Merge --- */
                        /* new string[] { quotationBasic.ProductCode }, */
                        new string[] { quotationBasic.ProductCodeName },
                        /* ------------- */
                        new string[] { "ProductCode" });
                else
                {
                    bool isError = true;
                    if (pLst[0].SalesStartDate != null)
                    {
                        if (pLst[0].SalesStartDate.Value.CompareTo(DateTime.Now) <= 0)
                        {
                            if (pLst[0].SalesEndDate != null)
                            {
                                if (pLst[0].SalesEndDate.Value.CompareTo(DateTime.Now) > 0)
                                    isError = false;
                            }
                            else
                                isError = false;

                        }
                    }
                    if (isError)
                    {
                        /* --- Merge --- */
                        /* res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2081, new string[] { quotationBasic.ProductCode }); */
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2081, new string[] { quotationBasic.ProductCodeName });
                        /* ------------- */

                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.ResultData = "ProdError";
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Validate employee data
        /// </summary>
        /// <param name="res"></param>
        /// <param name="obj"></param>
        private void ValidateEmployeeData(ObjectResultData res, object obj)
        {
            if (obj == null)
                return;

            try
            {
                #region Create Object

                object template = null;
                if (obj is QUS030_tbt_QuotationBasic)
                    template = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic, QUS030_tbt_QuotationBasic_Employee>((QUS030_tbt_QuotationBasic)obj);
                else if (obj is QUS030_tbt_QuotationBasic_AL)
                    template = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic_AL, QUS030_tbt_QuotationBasic_AL_Employee>((QUS030_tbt_QuotationBasic_AL)obj);
                else if (obj is QUS030_tbt_QuotationBasic_ONLINE)
                    template = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic_ONLINE, QUS030_tbt_QuotationBasic_ONLINE_Employee>((QUS030_tbt_QuotationBasic_ONLINE)obj);
                else if (obj is QUS030_tbt_QuotationBasic_BE)
                    template = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic_BE, QUS030_tbt_QuotationBasic_BE_Employee>((QUS030_tbt_QuotationBasic_BE)obj);
                else if (obj is QUS030_tbt_QuotationBasic_SG)
                    template = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic_SG, QUS030_tbt_QuotationBasic_SG_Employee>((QUS030_tbt_QuotationBasic_SG)obj);
                else if (obj is QUS030_tbt_QuotationBasic_MA)
                    template = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic_MA, QUS030_tbt_QuotationBasic_MA_Employee>((QUS030_tbt_QuotationBasic_MA)obj);

                #endregion

                EmployeeExistList existLst = new EmployeeExistList();
                existLst.AddEmployee(template);

                IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                handler.CheckActiveEmployeeExist(existLst, true);

                string txt = existLst.EmployeeNoExist;
                if (txt != string.Empty)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                        "QUS030",
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0095,
                                        new string[] { txt },
                                        existLst.ControlNoExist);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Validate new building management cost data
        /// </summary>
        /// <param name="res"></param>
        /// <param name="quotationBasic"></param>
        private void ValidateNewBldMgmtCost(ObjectResultData res, tbt_QuotationBasic quotationBasic)
        {
            try
            {
                if (quotationBasic != null)
                {
                    bool isError = false;
                    if (CommonUtil.IsNullOrEmpty(quotationBasic.NewBldMgmtCost) == false)
                    {
                        isError = true;
                        if (quotationBasic.BuildingTypeCode == "1"
                            && quotationBasic.NewBldMgmtFlag == true)
                        {
                            isError = false;
                        }
                    }

                    if (isError)
                    {
                        quotationBasic.NewBldMgmtCost = null;
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2083);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Initial quotation detail data
        /// </summary>
        /// <param name="res"></param>
        /// <param name="cond"></param>
        /// <returns></returns>
        private doInitQuotationData InitQuotationData(ObjectResultData res, doGetQuotationDataCondition cond)
        {
            try
            {
                doInitQuotationData doInitData = null;

                List<object> oLst = CreateQuotationHeader(res, cond);
                if (oLst == null)
                    return null;

                if (oLst.Count > 0)
                {
                    doQuotationHeaderData qHeader = oLst[0] as doQuotationHeaderData;
                    if (qHeader != null)
                    {
                        if (qHeader.doQuotationTarget.TargetCodeTypeCode == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                        {
                            doInitData = new doInitQuotationData()
                            {
                                doQuotationHeaderData = qHeader,
                                doQuotationBasic = null,
                                FirstInstallCompleteFlag = false,
                                StartOperationFlag = false
                            };
                        }
                        else
                        {
                            if (qHeader.doQuotationTarget.ServiceTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ServiceType.C_SERVICE_TYPE_SALE)
                                doInitData = InitSaleQuotationData(res, qHeader, oLst);
                            else
                                doInitData = InitRentalQuotationData(res, qHeader, oLst);

                            if (res.IsError)
                                return null;
                        }
                    }
                }
                if (doInitData == null)
                {
                    CommonUtil cmm = new CommonUtil();
                    string qtCodeShort = cmm.ConvertQuotationTargetCode(
                        cond.QuotationTargetCode,
                        CommonUtil.CONVERT_TYPE.TO_SHORT);

                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2003, new string[] { qtCodeShort }, new string[] { "QuotationTargetCodeShort" });

                    /* --- Merge --- */
                    /* res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK; */
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    /* ------------- */
                }

                return doInitData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Create quotation header data
        /// </summary>
        /// <param name="res"></param>
        /// <param name="cond"></param>
        /// <returns></returns>
        private List<object> CreateQuotationHeader(ObjectResultData res, doGetQuotationDataCondition cond)
        {
            try
            {
                bool isSaleData = false;
                bool isRentalData = false;
                bool isCustomHeader = false;
                List<object> objLst = new List<object>();

                IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                doQuotationHeaderData qHeader = qhandler.GetQuotationHeaderData(cond);
                objLst.Add(qHeader);

                if (qHeader != null)
                {
                    if (qHeader.doQuotationTarget != null)
                    {
                        if (qHeader.doQuotationTarget.TargetCodeTypeCode != SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                        {
                            if (qHeader.doQuotationTarget.ServiceTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ServiceType.C_SERVICE_TYPE_SALE)
                                isSaleData = true;
                            else
                                isRentalData = true;
                        }
                    }
                }

                if (qHeader == null || isSaleData == true)
                {
                    #region Create Quotation Header from Sale Contract

                    ISaleContractHandler schandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                    doSaleContractData dsSaleContractData = schandler.GetSaleContractData(cond.QuotationTargetCode, null);

                    if (objLst.Count == 1)
                        objLst.Add(dsSaleContractData);
                    else
                        objLst[1] = dsSaleContractData;

                    /* --- Create Quotation Header from Sale Contract Data --- */
                    if (qHeader == null && dsSaleContractData != null)
                    {
                        if (dsSaleContractData.dtTbt_SaleBasic != null)
                        {
                            isCustomHeader = true;
                            qHeader = new doQuotationHeaderData();
                            objLst[0] = qHeader;

                            /* --- Clone data --- */
                            qHeader.doQuotationTarget = CommonUtil.CloneObject<tbt_SaleBasic, doQuotationTarget>(dsSaleContractData.dtTbt_SaleBasic);
                            qHeader.doQuotationTarget.QuotationTargetCode = dsSaleContractData.dtTbt_SaleBasic.ContractCode;

                            /* --- Mapping --- */
                            if (qHeader.doQuotationTarget != null)
                            {
                                qHeader.doQuotationTarget.TargetCodeTypeCode = SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE;

                                if (dsSaleContractData.dtTbt_SaleBasic.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_BEF_START)
                                    qHeader.doQuotationTarget.QuotationOfficeCode = dsSaleContractData.dtTbt_SaleBasic.ContractOfficeCode;
                                else
                                    qHeader.doQuotationTarget.QuotationOfficeCode = dsSaleContractData.dtTbt_SaleBasic.OperationOfficeCode;

                                qHeader.doQuotationTarget.OldContractCode = null;
                                qHeader.doQuotationTarget.LastAlphabet = null;

                                if (dsSaleContractData.dtTbt_SaleBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL)
                                    qHeader.doQuotationTarget.ContractTransferStatus = ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_CAN;
                                else
                                    qHeader.doQuotationTarget.ContractTransferStatus = ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_APP;

                                qHeader.doQuotationTarget.ContractCode = null;
                                qHeader.doQuotationTarget.TransferDate = null;
                                qHeader.doQuotationTarget.TransferAlphabet = null;
                            }

                            /* --- Create Contract Target --- */
                            qHeader.doContractTarget = new doQuotationCustomer()
                            {
                                CustPartTypeCode = SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET,
                                QuotationTargetCode = dsSaleContractData.dtTbt_SaleBasic.ContractCode,
                                CustCode = dsSaleContractData.dtTbt_SaleBasic.PurchaserCustCode
                            };

                            /* --- Real Customer --- */
                            qHeader.doRealCustomer = new doQuotationCustomer()
                            {
                                CustPartTypeCode = SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_REAL_CUST,
                                QuotationTargetCode = dsSaleContractData.dtTbt_SaleBasic.ContractCode,
                                CustCode = dsSaleContractData.dtTbt_SaleBasic.RealCustomerCustCode
                            };

                            /* --- Site --- */
                            if (dsSaleContractData.dtTbt_SaleBasic.SiteCode != null)
                            {
                                qHeader.doQuotationSite = new doQuotationSite()
                                {
                                    QuotationTargetCode = dsSaleContractData.dtTbt_SaleBasic.ContractCode,
                                    SiteCode = dsSaleContractData.dtTbt_SaleBasic.SiteCode,
                                    SiteNo = dsSaleContractData.dtTbt_SaleBasic.SiteCode.Substring(dsSaleContractData.dtTbt_SaleBasic.SiteCode.Length - 4)
                                };
                            }
                        }
                    }

                    #endregion
                }
                if (qHeader == null || isRentalData == true)
                {
                    #region Create Quotation Header from Rental Contract

                    IRentralContractHandler rhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    dsRentalContractData dsRentalContractData = rhandler.GetEntireContract(cond.QuotationTargetCode, null);

                    if (dsRentalContractData != null)
                    {
                        if (dsRentalContractData.dtTbt_RentalContractBasic.Count > 0)
                        {
                            if ((dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL
                                    || dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                                && dsRentalContractData.dtTbt_RentalContractBasic[0].StartType == StartType.C_START_TYPE_ALTER_START)
                            {
                                string lastUnimplementedOCC = rhandler.GetLastUnimplementedOCC(dsRentalContractData.dtTbt_RentalContractBasic[0].ContractCode);

                                dsRentalContractData =
                                    rhandler.GetEntireContract(dsRentalContractData.dtTbt_RentalContractBasic[0].ContractCode, lastUnimplementedOCC);
                            }
                        }
                    }

                    if (objLst.Count == 1)
                        objLst.Add(dsRentalContractData);
                    else
                        objLst[1] = dsRentalContractData;

                    /* --- Create Quotation Header from Rental Contract Data --- */
                    if (qHeader == null && dsRentalContractData != null)
                    {
                        if (dsRentalContractData.dtTbt_RentalContractBasic != null)
                        {
                            if (dsRentalContractData.dtTbt_RentalContractBasic.Count > 0)
                            {
                                isCustomHeader = true;
                                qHeader = new doQuotationHeaderData();
                                objLst[0] = qHeader;

                                /* --- Clone data --- */
                                /* ------------------ */
                                qHeader.doQuotationTarget = CommonUtil.CloneObject<tbt_RentalContractBasic, doQuotationTarget>(dsRentalContractData.dtTbt_RentalContractBasic[0]);
                                /* ------------------ */

                                #region Mapping Data

                                if (qHeader.doQuotationTarget != null)
                                {
                                    qHeader.doQuotationTarget.QuotationTargetCode = dsRentalContractData.dtTbt_RentalContractBasic[0].ContractCode;
                                    qHeader.doQuotationTarget.TargetCodeTypeCode = SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE;

                                    if (dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_BEF_START)
                                        qHeader.doQuotationTarget.QuotationOfficeCode = dsRentalContractData.dtTbt_RentalContractBasic[0].ContractOfficeCode;
                                    else
                                        qHeader.doQuotationTarget.QuotationOfficeCode = dsRentalContractData.dtTbt_RentalContractBasic[0].OperationOfficeCode;

                                    qHeader.doQuotationTarget.LastAlphabet = null;

                                    if (dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_END
                                        || dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_CANCEL
                                        || dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                                    {
                                        qHeader.doQuotationTarget.ContractTransferStatus = SECOM_AJIS.Common.Util.ConstantValue.ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_CAN;
                                    }
                                    else
                                        qHeader.doQuotationTarget.ContractTransferStatus = SECOM_AJIS.Common.Util.ConstantValue.ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_APP;

                                    qHeader.doQuotationTarget.ContractCode = null;
                                    qHeader.doQuotationTarget.TransferDate = null;
                                    qHeader.doQuotationTarget.TransferAlphabet = null;
                                }

                                #endregion
                                #region Create Contract Target

                                qHeader.doContractTarget = new doQuotationCustomer()
                                {
                                    CustPartTypeCode = SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_CONTRACT_TARGET,
                                    QuotationTargetCode = dsRentalContractData.dtTbt_RentalContractBasic[0].ContractCode,
                                    CustCode = dsRentalContractData.dtTbt_RentalContractBasic[0].ContractTargetCustCode
                                };

                                #endregion
                                #region Real Customer

                                qHeader.doRealCustomer = new doQuotationCustomer()
                                {
                                    CustPartTypeCode = SECOM_AJIS.Common.Util.ConstantValue.CustPartType.C_CUST_PART_TYPE_REAL_CUST,
                                    QuotationTargetCode = dsRentalContractData.dtTbt_RentalContractBasic[0].ContractCode,
                                    CustCode = dsRentalContractData.dtTbt_RentalContractBasic[0].RealCustomerCustCode
                                };

                                #endregion
                                #region Site

                                if (dsRentalContractData.dtTbt_RentalContractBasic[0].SiteCode != null)
                                {
                                    qHeader.doQuotationSite = new doQuotationSite()
                                    {
                                        QuotationTargetCode = dsRentalContractData.dtTbt_RentalContractBasic[0].ContractCode,
                                        SiteCode = dsRentalContractData.dtTbt_RentalContractBasic[0].SiteCode,
                                        SiteNo = dsRentalContractData.dtTbt_RentalContractBasic[0].SiteCode.Substring(dsRentalContractData.dtTbt_RentalContractBasic[0].SiteCode.Length - 4)
                                    };
                                }

                                #endregion
                            }
                        }
                    }

                    #endregion
                }

                if (qHeader != null)
                {
                    #region Authority check

                    bool hasAuthority = false;
                    List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
                    if (clst != null)
                    {
                        foreach (OfficeDataDo off in clst)
                        {
                            if (off.OfficeCode == qHeader.doQuotationTarget.QuotationOfficeCode
                                || off.OfficeCode == qHeader.doQuotationTarget.OperationOfficeCode)
                                hasAuthority = true;
                        }
                    }

                    if (hasAuthority == false)
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0063);
                        return null;
                    }
                    else
                    {
                        /* --- Merge --- */
                        /* QUS030_doQuotationTarget_Initial inv =
                            CommonUtil.CloneObject<doQuotationTarget, QUS030_doQuotationTarget_Initial>(qHeader.doQuotationTarget);
                        ValidatorUtil.BuildErrorMessage(res, new object[] { inv }); */
                        QUS030_doQuotationTarget_Initial inv =
                            CommonUtil.CloneObject<doQuotationTarget, QUS030_doQuotationTarget_Initial>(qHeader.doQuotationTarget);

                        ValidatorUtil validator = new ValidatorUtil(this);
                        if (inv.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE
                            && inv.ContractTransferStatus == ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_CAN)
                        {
                            validator.AddErrorMessage(
                                MessageUtil.MODULE_QUOTATION,
                                MessageUtil.MessageList.MSG2004,
                                "ContractTransferStatus");
                        }
                        else if (inv.TargetCodeTypeCode != TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE
                            && inv.ContractTransferStatus == ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_CAN)
                        {
                            validator.AddErrorMessage(
                                MessageUtil.MODULE_QUOTATION,
                                MessageUtil.MessageList.MSG2061,
                                "ContractTransferStatus");
                        }

                        ValidatorUtil.BuildErrorMessage(res, validator, new object[] { inv });
                        /* ------------- */

                        if (res.IsError)
                            return null;
                    }

                    #endregion

                    if (isCustomHeader == true)
                    {
                        if (qHeader.doQuotationTarget != null)
                        {
                            #region Mapping Product Type Data

                            ICommonHandler cmmhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                            List<tbs_ProductType> pLst = cmmhandler.GetTbs_ProductType(null, qHeader.doQuotationTarget.ProductTypeCode);
                            if (pLst.Count > 0)
                            {
                                qHeader.doQuotationTarget.ProductTypeName = pLst[0].ProductTypeName;
                                qHeader.doQuotationTarget.ProvideServiceName = pLst[0].ProvideServiceName;
                            }

                            #endregion
                            #region Mapping Qffice Data

                            OfficeMappingList officeMapping = new OfficeMappingList();
                            officeMapping.AddOffice(qHeader.doQuotationTarget);

                            IOfficeMasterHandler ohandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                            ohandler.OfficeListMapping(officeMapping);

                            #endregion
                            #region Mapping Misc Data

                            MiscTypeMappingList miscMapping = new MiscTypeMappingList();
                            miscMapping.AddMiscType(qHeader.doQuotationTarget);
                            cmmhandler.MiscTypeMappingList(miscMapping);

                            #endregion
                            #region Mapping Employee

                            IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                            EmployeeMappingList empLst = new EmployeeMappingList();
                            empLst.AddEmployee(qHeader.doQuotationTarget);
                            ehandler.EmployeeListMapping(empLst);

                            #endregion
                        }

                        #region Mapping Customer 1 & 2

                        if (qHeader.doContractTarget != null
                            || qHeader.doRealCustomer != null)
                        {
                            List<tbm_Customer> custLst = new List<tbm_Customer>();
                            if (qHeader.doContractTarget != null)
                                custLst.Add(new tbm_Customer() { CustCode = qHeader.doContractTarget.CustCode });
                            if (qHeader.doRealCustomer != null)
                                custLst.Add(new tbm_Customer() { CustCode = qHeader.doRealCustomer.CustCode });

                            bool isSignContractTarget = false;
                            bool isSignRealCustomer = false;
                            ICustomerMasterHandler chandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                            List<doCustomerList> customerLst = chandler.GetCustomerList(custLst);
                            foreach (doCustomerList customer in customerLst)
                            {
                                doQuotationCustomer cust = null;
                                if (isSignContractTarget == false && qHeader.doContractTarget != null)
                                {
                                    if (qHeader.doContractTarget.CustCode == customer.CustCode)
                                    {
                                        cust = qHeader.doContractTarget;
                                        isSignContractTarget = true;
                                    }
                                }
                                if (cust == null && isSignRealCustomer == false && qHeader.doRealCustomer != null)
                                {
                                    if (qHeader.doRealCustomer.CustCode == customer.CustCode)
                                    {
                                        cust = qHeader.doRealCustomer;
                                        isSignRealCustomer = true;
                                    }
                                }
                                if (cust != null)
                                {
                                    cust.CustFullNameEN = customer.CustFullNameEN;
                                    cust.CustFullNameLC = customer.CustFullNameLC;
                                    cust.AddressFullEN = customer.AddressFullEN;
                                    cust.AddressFullLC = customer.AddressFullLC;
                                }
                            }
                        }

                        #endregion
                        #region Mapping Site Data

                        if (qHeader.doQuotationSite != null)
                        {
                            ISiteMasterHandler shandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
                            List<doGetTbm_Site> lst = shandler.GetTbm_Site(qHeader.doQuotationSite.SiteCode);
                            if (lst.Count > 0)
                            {
                                qHeader.doQuotationSite.SiteNameEN = lst[0].SiteNameEN;
                                qHeader.doQuotationSite.SiteNameLC = lst[0].SiteNameLC;
                                qHeader.doQuotationSite.AddressFullEN = lst[0].AddressFullEN;
                                qHeader.doQuotationSite.AddressFullLC = lst[0].AddressFullLC;
                            }
                        }

                        #endregion
                    }
                }

                return objLst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Initial rental quotation detail data
        /// </summary>
        /// <param name="res"></param>
        /// <param name="qHeader"></param>
        /// <param name="objLst"></param>
        /// <returns></returns>
        private doInitQuotationData InitRentalQuotationData(ObjectResultData res, doQuotationHeaderData qHeader, List<object> objLst)
        {
            try
            {
                #region Create Init Rental Quotation Data

                dsRentalContractData dsRentalContractData = null;
                if (objLst.Count > 1)
                    dsRentalContractData = objLst[1] as dsRentalContractData;
                if (dsRentalContractData == null)
                {
                    CommonUtil cmm = new CommonUtil();
                    string qtCodeShort = cmm.ConvertQuotationTargetCode(
                            qHeader.doQuotationTarget.QuotationTargetCode,
                            CommonUtil.CONVERT_TYPE.TO_SHORT);

                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2003, new string[] { qtCodeShort });

                    /* --- Merge --- */
                    /* res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK; */
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    /* ------------- */

                    return null;
                }

                doInitRentalQuotationData irDo = null;
                if (dsRentalContractData.dtTbt_RentalContractBasic != null)
                {
                    if (dsRentalContractData.dtTbt_RentalContractBasic.Count > 0)
                    {
                        if (dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL
                            || dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_RENTAL_SALE)
                            irDo = InitALQuotationData(dsRentalContractData);
                        else if (dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_ONLINE)
                            irDo = InitSaleOnlineQuotationData(dsRentalContractData);
                        else if (dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_BE)
                            irDo = InitBEQuotationData(dsRentalContractData);
                        else if (dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_MA)
                            irDo = InitMAQuotationData(dsRentalContractData);
                        else if (dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SG)
                            irDo = InitSGQuotationData(dsRentalContractData);
                    }
                }

                #endregion

                if (irDo != null)
                {
                    doInitQuotationData doInitData = new doInitQuotationData()
                    {
                        doQuotationHeaderData = qHeader,
                        doQuotationBasic = irDo.doQuotationBasic,
                        OperationTypeList = irDo.OperationTypeList,
                        InstrumentDetailList = irDo.InstrumentDetailList,
                        FacilityDetailList = irDo.FacilityDetailList,
                        doLinkageSaleContractData = irDo.doLinkageSaleContractData,
                        doBeatGuardDetail = irDo.doBeatGuardDetail,
                        SentryGuardDetailList = irDo.SentryGuardDetailList,
                        MaintenanceTargetList = irDo.MaintenanceTargetList,
                        PriorInstrumentDetailList = new List<doInstrumentDetail>()
                    };

                    if (irDo.doQuotationBasic != null)
                    {
                        doInitData.PriorProductCode = irDo.doQuotationBasic.ProductCode;
                        doInitData.PriorSaleOnlineContractCode = irDo.doQuotationBasic.SaleOnlineContractCode;
                        doInitData.PriorMaintenanceTargetProductTypeCode = irDo.doQuotationBasic.MaintenanceTargetProductTypeCode;
                    }


                    if (dsRentalContractData.dtTbt_RentalContractBasic != null)
                    {
                        if (dsRentalContractData.dtTbt_RentalContractBasic.Count > 0)
                        {
                            doInitData.FirstInstallCompleteFlag = dsRentalContractData.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag;

                            if (dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_BEF_START)
                                doInitData.StartOperationFlag = false;
                            else
                                doInitData.StartOperationFlag = true;


                        }
                    }
                    if ((dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL
                            || dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_RENTAL_SALE)
                        && doInitData.FirstInstallCompleteFlag == true)
                    {
                        doInitData.PriorInstrumentDetailList = irDo.InstrumentDetailList;
                    }

                    /* --- *** --- */
                    #region Mapping Data in Quotation Basic

                    if (doInitData.doQuotationBasic != null)
                    {
                        #region Set Misc Name

                        MiscTypeMappingList miscLst = new MiscTypeMappingList();
                        miscLst.AddMiscType(doInitData.doQuotationBasic);
                        ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        chandler.MiscTypeMappingList(miscLst);

                        #endregion
                    }

                    #endregion

                    return doInitData;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Sale Actions

        /// <summary>
        /// Check quotation detail data in case of sale (step 1) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <param name="instLst"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckSaleQuotationData_P1(QUS030_tbt_QuotationBasic quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> instLst)
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
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                #endregion
                #region Validate Object

                ValidatorUtil validator = new ValidatorUtil(this);

                /* --- Special Validate --- */
                /* ------------------------ */
                bool isInstEmpty = true;
                IProductMasterHandler mhandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                List<View_tbm_Product> pLst = mhandler.GetTbm_ProductByLanguage(quotationBasic.ProductCode, "");
                if (instLst != null)
                {
                    if (instLst.Count > 0)
                        isInstEmpty = (instLst[0] == null);
                }
                if (pLst[0].OneShotFlag == false) {
                    if (isInstEmpty)
                        validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "InstrumentDetail",
                                                    "lblInstrumentDetail_01",
                                                    "InstrumentDetail",
                                                    "9");
                }
                /* ------------------------ */

                if (quotationBasic.NewBldMgmtFlag == true)
                {
                    if (CommonUtil.IsNullOrEmpty(quotationBasic.NewBldMgmtCost))
                    {
                        validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "NewBldMgmtCost",
                                                    "lblNewBuildingMgmtCost",
                                                    "NewBldMgmtCost",
                                                    "7");
                    }
                }

                ValidatorUtil.BuildErrorMessage(res, validator);
                if (res.IsError)
                    return Json(res);

                #endregion
                #region Validate Instrument

                string dup_inst = null;
                for (int i = 0; i < instLst.Count; i++)
                {
                    QUS030_tbt_QuotationInstrumentDetails inst = instLst[i];

                    List<string> eLst = new List<string>();
                    if (CommonUtil.IsNullOrEmpty(inst.InstrumentCode))
                        eLst.Add("headerInstrumentCode");
                    if (CommonUtil.IsNullOrEmpty(inst.InstrumentQty))
                        eLst.Add("headerQuantity");

                    if (eLst.Count > 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                            "QUS030",
                                            MessageUtil.MODULE_QUOTATION,
                                            MessageUtil.MessageList.MSG2079,
                                            new string[] { (i + 1).ToString(), CommonUtil.TextList(eLst.ToArray()) });
                        return Json(res);
                    }

                    if (dup_inst == null)
                    {
                        for (int j = 0; j < instLst.Count; j++)
                        {
                            if (i == j)
                                continue;

                            QUS030_tbt_QuotationInstrumentDetails cinst = instLst[j];

                            /* --- Merge --- */
                            /* if (inst.InstrumentCode == cinst.InstrumentCode)
                            {
                                dup_inst = inst.InstrumentCode;
                                break;
                            } */
                            if (CommonUtil.IsNullOrEmpty(cinst.InstrumentCode) == false)
                            {
                                if (inst.InstrumentCode.ToUpper().Trim() == cinst.InstrumentCode.ToUpper().Trim())
                                {
                                    dup_inst = inst.InstrumentCode;
                                    break;
                                }
                            }
                            /* ------------- */

                        }
                    }
                }
                if (dup_inst != null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                            MessageUtil.MessageList.MSG2086,
                                            new string[] { dup_inst });
                    return Json(res);
                }

                #endregion
                #region Validate Range

                QUS030_ValidateRangeData valRange = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic, QUS030_ValidateRangeData>(quotationBasic);

                Dictionary<string, RangeNumberValueAttribute> rangeAttr =
                    CommonUtil.CreateAttributeDictionary<RangeNumberValueAttribute>(valRange);
                foreach (KeyValuePair<string, RangeNumberValueAttribute> attr in rangeAttr)
                {
                    PropertyInfo prop = valRange.GetType().GetProperty(attr.Key);
                    if (prop != null)
                    {
                        object val = prop.GetValue(valRange, null);
                        if (CommonUtil.IsNullOrEmpty(val) == false)
                        {
                            if (attr.Value.IsValid(val) == false)
                            {
                                string format = "N2";
                                if (prop.PropertyType == typeof(int?))
                                    format = "N0";

                                string min = attr.Value.Min.ToString(format);
                                string max = attr.Value.Max.ToString(format);

                                res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    "QUS030",
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2087,
                                    new string[] { attr.Value.Parameter, min, max },
                                    new string[] { attr.Value.ControlName });
                                return Json(res);
                            }
                        }
                    }
                }

                int line = 1;
                foreach (QUS030_tbt_QuotationInstrumentDetails inst in instLst)
                {
                    QUS030_ValidateRangeData_Instrument1 valInstRange =
                        CommonUtil.CloneObject<QUS030_tbt_QuotationInstrumentDetails, QUS030_ValidateRangeData_Instrument1>(inst);

                    rangeAttr = CommonUtil.CreateAttributeDictionary<RangeNumberValueAttribute>(valInstRange);
                    foreach (KeyValuePair<string, RangeNumberValueAttribute> attr in rangeAttr)
                    {
                        PropertyInfo prop = valInstRange.GetType().GetProperty(attr.Key);
                        if (prop != null)
                        {
                            object val = prop.GetValue(valInstRange, null);
                            if (CommonUtil.IsNullOrEmpty(val) == false)
                            {
                                if (attr.Value.IsValid(val) == false)
                                {
                                    string format = "N2";
                                    if (prop.PropertyType == typeof(int?))
                                        format = "N0";

                                    string min = attr.Value.Min.ToString(format);
                                    string max = attr.Value.Max.ToString(format);

                                    res.AddErrorMessage(
                                        MessageUtil.MODULE_QUOTATION,
                                        "QUS030",
                                        MessageUtil.MODULE_QUOTATION,
                                        MessageUtil.MessageList.MSG2088,
                                        new string[] { inst.InstrumentCode, attr.Value.Parameter, min, max },
                                        new string[] { string.Format("InstrumentDetail;{0};{1}", attr.Value.ControlName, line) });
                                    return Json(res);
                                }
                            }
                        }
                    }

                    line++;
                }

                #endregion

                /* --- Merge --- */
                #region Validate Plan checking date/Plan approving date

                DateTime nowDate = DateTime.Now;
                nowDate = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day);
                if (CommonUtil.IsNullOrEmpty(quotationBasic.PlanCheckDate) == false)
                {
                    if (quotationBasic.PlanCheckDate > nowDate)
                    {
                        res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    "QUS030",
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2029,
                                    null,
                                    new string[] { "PlanCheckDate" });
                        return Json(res);
                    }
                }
                if (CommonUtil.IsNullOrEmpty(quotationBasic.PlanApproveDate) == false)
                {
                    if (quotationBasic.PlanApproveDate > nowDate)
                    {
                        res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    "QUS030",
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2031,
                                    null,
                                    new string[] { "PlanApproveDate" });
                        return Json(res);
                    }
                }

                #endregion
                #region Security Area From/To

                decimal dfrom = 0;
                decimal dto = 0;
                if (CommonUtil.IsNullOrEmpty(quotationBasic.SecurityAreaFrom) == false)
                    dfrom = quotationBasic.SecurityAreaFrom.Value;
                if (CommonUtil.IsNullOrEmpty(quotationBasic.SecurityAreaTo) == false)
                    dto = quotationBasic.SecurityAreaTo.Value;
                if (dfrom > dto)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                        MessageUtil.MessageList.MSG2063);
                    return Json(res);
                }

                #endregion
                /* ------------- */

                ValidateNewBldMgmtCost(res, quotationBasic);
                if (res.IsError)
                    return Json(res);

                ValidateEmployeeData(res, quotationBasic);
                if (res.IsError)
                    return Json(res);

                if (pLst[0].OneShotFlag == false)
                    ValidateInstrumentDetail01(res, quotationBasic, instLst);
                
                if (res.IsError)
                    return Json(res);

                ValidateProduct(res, quotationBasic);
                if (res.IsError)
                    return Json(res);
                else if (res.ResultData == null)
                    CheckSaleQuotationData(res, quotationBasic, instLst);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check quotation detail data in case of sale (step 2) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <param name="instLst"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckSaleQuotationData_P2(QUS030_tbt_QuotationBasic quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> instLst)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CheckSaleQuotationData(res, quotationBasic, instLst);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region Sale Methods

        /// <summary>
        /// Initial quotation detail data in case of sale
        /// </summary>
        /// <param name="res"></param>
        /// <param name="qHeader"></param>
        /// <param name="objLst"></param>
        /// <returns></returns>
        private doInitQuotationData InitSaleQuotationData(ObjectResultData res, doQuotationHeaderData qHeader, List<object> objLst)
        {
            try
            {

                QUS030_ScreenParameter param = QUS030_ScreenData;
                #region Create Quotation Basic

                doSaleContractData doSaleContractData = null;
                if (objLst.Count > 1)
                    doSaleContractData = objLst[1] as doSaleContractData;
                if (doSaleContractData == null)
                {
                    CommonUtil cmm = new CommonUtil();
                    string qtCodeShort = cmm.ConvertQuotationTargetCode(
                        qHeader.doQuotationTarget.QuotationTargetCode,
                        CommonUtil.CONVERT_TYPE.TO_SHORT);

                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2003, new string[] { qtCodeShort });

                    /* --- Merge --- */
                    /* res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK; */
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    /* ------------- */

                    return null;
                }

                tbt_QuotationBasic doQuotationBasic = new tbt_QuotationBasic();

                #region Mapping Employee Name

                IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                EmployeeMappingList empMLst = new EmployeeMappingList();
                empMLst.AddEmployee(doSaleContractData.dtTbt_SaleBasic);
                ehandler.ActiveEmployeeListMapping(empMLst);

                #endregion

                if (doSaleContractData.dtTbt_SaleBasic != null)
                {
                    doQuotationBasic = new tbt_QuotationBasic()
                    {
                        ProductCode = doSaleContractData.dtTbt_SaleBasic.ProductCode,
                        ContractDurationMonth = null,
                        AutoRenewMonth = null,
                        LastOccNo = doSaleContractData.dtTbt_SaleBasic.OCC,
                        PlanCode = doSaleContractData.dtTbt_SaleBasic.PlanCode,
                        //QuotationNo = param.InitialData.doQuotationBasic.QuotationNo,
                        SpecialInstallationFlag = doSaleContractData.dtTbt_SaleBasic.SpecialInstallationFlag,
                        PlannerEmpNo = doSaleContractData.dtTbt_SaleBasic.PlannerEmpNo,
                        PlannerName = doSaleContractData.dtTbt_SaleBasic.PlannerName,
                        PlanCheckerEmpNo = doSaleContractData.dtTbt_SaleBasic.PlanCheckerEmpNo,
                        PlanCheckerName = doSaleContractData.dtTbt_SaleBasic.PlanCheckerName,
                        PlanCheckDate = doSaleContractData.dtTbt_SaleBasic.PlanCheckDate,
                        PlanApproverEmpNo = doSaleContractData.dtTbt_SaleBasic.PlanApproverEmpNo,
                        PlanApproverName = doSaleContractData.dtTbt_SaleBasic.PlanApproverName,
                        PlanApproveDate = doSaleContractData.dtTbt_SaleBasic.PlanApproveDate,
                        SiteBuildingArea = doSaleContractData.dtTbt_SaleBasic.SiteBuildingArea,
                        SecurityAreaFrom = doSaleContractData.dtTbt_SaleBasic.SecurityAreaFrom,
                        SecurityAreaTo = doSaleContractData.dtTbt_SaleBasic.SecurityAreaTo,
                        MainStructureTypeCode = doSaleContractData.dtTbt_SaleBasic.MainStructureTypeCode,
                        BuildingTypeCode = doSaleContractData.dtTbt_SaleBasic.BuildingTypeCode,
                        NewBldMgmtFlag = doSaleContractData.dtTbt_SaleBasic.NewBldMgmtFlag,
                    };
                    
                    doQuotationBasic.NewBldMgmtCostCurrencyType = doSaleContractData.dtTbt_SaleBasic.NewBldMgmtCostCurrencyType;
                    if (doQuotationBasic.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        doQuotationBasic.NewBldMgmtCost = null;
                        doQuotationBasic.NewBldMgmtCostUsd = doSaleContractData.dtTbt_SaleBasic.NewBldMgmtCost;
                    }
                    else
                    {
                        doQuotationBasic.NewBldMgmtCost = doSaleContractData.dtTbt_SaleBasic.NewBldMgmtCost;
                        doQuotationBasic.NewBldMgmtCostUsd = null;
                    }
                }

                #endregion

                //Add by Jutarat A. on 31052013
                //Get instrument data
                ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                List<tbt_SaleInstrumentDetails> dtTbt_SaleInstrumentDetails = saleHandler.GetTbt_SaleInstrumentDetails(doSaleContractData.dtTbt_SaleBasic.ContractCode, doSaleContractData.dtTbt_SaleBasic.OCC);

                List<tbm_Instrument> mInstLst = new List<tbm_Instrument>();
                foreach (tbt_SaleInstrumentDetails inst in dtTbt_SaleInstrumentDetails)
                {
                    if (CommonUtil.IsNullOrEmpty(inst.InstrumentCode) == false)
                    {
                        mInstLst.Add(new tbm_Instrument()
                        {
                            InstrumentCode = inst.InstrumentCode
                        });
                    }
                }

                IInstrumentMasterHandler instrumentHandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<doInstrumentData> masterInstLst = instrumentHandler.GetInstrumentListForView(mInstLst);

                List<doInstrumentDetail> instDetailLst = new List<doInstrumentDetail>();
                foreach (tbt_SaleInstrumentDetails sInst in dtTbt_SaleInstrumentDetails)
                {
                    if (sInst.InstrumentTypeCode == InstrumentType.C_INST_TYPE_GENERAL)
                    {
                        doInstrumentData masterInst = null;
                        if (CommonUtil.IsNullOrEmpty(sInst.InstrumentCode) == false)
                        {
                            foreach (doInstrumentData mInst in masterInstLst)
                            {
                                if (mInst.InstrumentCode.ToUpper().Trim() == sInst.InstrumentCode.ToUpper().Trim())
                                {
                                    masterInst = mInst;
                                    break;
                                }
                            }
                        }

                        if (masterInst != null)
                        {
                            doInstrumentDetail nInst = new doInstrumentDetail();
                            nInst.MaintenanceFlag = masterInst.MaintenanceFlag;
                            nInst.InstrumentCode = masterInst.InstrumentCode;
                            nInst.InstrumentName = masterInst.InstrumentName;
                            nInst.LineUpTypeCode = masterInst.LineUpTypeCode;
                            nInst.LineUpTypeName = masterInst.LineUpTypeName;

                            //Modify by Jutarat A. on 06062013
                            //nInst.InstrumentQty = 0;
                            if (doSaleContractData.dtTbt_SaleBasic != null && doSaleContractData.dtTbt_SaleBasic.InstallationCompleteFlag == true)
                            {
                                nInst.InstrumentQty = 0;
                            }
                            else
                            {
                                nInst.InstrumentQty = sInst.InstrumentQty;
                            }
                            //End Modify

                            nInst.AddQty = 0;
                            nInst.RemoveQty = 0;
                            nInst.ControllerFlag = masterInst.ControllerFlag;
                            nInst.InstrumentFlag = masterInst.InstrumentFlag;
                            nInst.SaleFlag = masterInst.SaleFlag;
                            nInst.RentalFlag = masterInst.RentalFlag;

                            instDetailLst.Add(nInst);
                        }
                    }
                }
                //End Add

                doInitQuotationData doInitData = new doInitQuotationData()
                {
                    doQuotationHeaderData = qHeader,
                    doQuotationBasic = doQuotationBasic,
                    FirstInstallCompleteFlag = false,
                    PriorProductCode = doQuotationBasic.ProductCode,
                    PriorInstrumentDetailList = new List<doInstrumentDetail>(),
                    InstrumentDetailList = instDetailLst //Add by Jutarat A. on 31052013
                };

                if (doSaleContractData.dtTbt_SaleBasic != null)
                {
                    if (doSaleContractData.dtTbt_SaleBasic.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        doInitData.StartOperationFlag = false;
                    else
                        doInitData.StartOperationFlag = true;
                }

                return doInitData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Initial quotation detail data in case of sale from import file
        /// </summary>
        /// <param name="res"></param>
        /// <param name="initData"></param>
        /// <param name="importData"></param>
        private void InitImportSaleQuotation(ObjectResultData res, doInitQuotationData initData, dsImportData importData)
        {
            try
            {
                #region Create Quotation Basic

                if (importData.dtTbt_QuotationBasic != null)
                {
                    if (importData.dtTbt_QuotationBasic.Count > 0)
                    {
                        tbt_QuotationBasic nDo = importData.dtTbt_QuotationBasic[0];
                        if (initData.doQuotationBasic != null)
                        {
                            nDo.LastOccNo = initData.doQuotationBasic.LastOccNo;

                            if (initData.doQuotationBasic.ProductCode == nDo.ProductCode)
                            {
                                nDo.ContractDurationMonth = initData.doQuotationBasic.ContractDurationMonth;
                                nDo.AutoRenewMonth = initData.doQuotationBasic.AutoRenewMonth;
                            }
                        }
                        initData.doQuotationBasic = nDo;
                    }
                }

                #endregion
                #region Prepare Instrument Detail

                initData.InstrumentDetailList = new List<doInstrumentDetail>();
                if (importData.dtTbt_QuotationInstrumentDetails != null)
                {
                    if (importData.dtTbt_QuotationInstrumentDetails.Count > 0)
                    {
                        /* --- Merge --- */
                        /* List<doParentGeneralInstrument> condLst = new List<doParentGeneralInstrument>();
                        foreach (tbt_QuotationInstrumentDetails inst in importData.dtTbt_QuotationInstrumentDetails)
                        {
                            condLst.Add(new doParentGeneralInstrument()
                            {
                                InstrumentCode = inst.InstrumentCode
                            });
                        }
                        if (condLst.Count > 0)
                        {
                            IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                            List<doParentGeneralInstrument> lst = ihandler.GetParentGeneralInstrumentList(condLst);
                            if (lst.Count > 0)
                            {
                                foreach (doParentGeneralInstrument pInst in lst)
                                {
                                    tbt_QuotationInstrumentDetails inst = null;
                                    foreach (tbt_QuotationInstrumentDetails iinst in importData.dtTbt_QuotationInstrumentDetails)
                                    {
                                        if (pInst.InstrumentCode == iinst.InstrumentCode)
                                        {
                                            inst = iinst;
                                            break;
                                        }
                                    }
                                    if (inst != null)
                                    {
                                        initData.InstrumentDetailList.Add(new doInstrumentDetail()
                                            {
                                                InstrumentCode = inst.InstrumentCode,
                                                InstrumentQty = inst.InstrumentQty,
                                                AddQty = inst.AddQty,
                                                RemoveQty = inst.RemoveQty,
                                                MaintenanceFlag = pInst.MaintenanceFlag,
                                                InstrumentName = pInst.InstrumentName,
                                                LineUpTypeCode = pInst.LineUpTypeCode,
                                                LineUpTypeName = pInst.LineUpTypeName,
                                                ControllerFlag = pInst.ControllerFlag,
                                                InstrumentFlag = pInst.InstrumentFlag,
                                                SaleFlag = pInst.SaleFlag,
                                                RentalFlag = pInst.RentalFlag
                                            });
                                    }
                                }
                            }
                        } */
                        List<tbm_Instrument> condLst = new List<tbm_Instrument>();
                        foreach (tbt_QuotationInstrumentDetails inst in importData.dtTbt_QuotationInstrumentDetails)
                        {
                            if (CommonUtil.IsNullOrEmpty(inst.InstrumentCode) == false)
                            {
                                condLst.Add(new tbm_Instrument()
                                {
                                    InstrumentCode = inst.InstrumentCode
                                });
                            }
                        }
                        if (condLst.Count > 0)
                        {
                            IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                            List<doInstrumentData> lst = ihandler.GetInstrumentListForView(condLst);

                            foreach (tbt_QuotationInstrumentDetails iinst in importData.dtTbt_QuotationInstrumentDetails)
                            {
                                doInstrumentDetail instD = new doInstrumentDetail()
                                {
                                    InstrumentCode = iinst.InstrumentCode,
                                    AddQty = iinst.AddQty,
                                    RemoveQty = iinst.RemoveQty,
                                    InstrumentQty = iinst.InstrumentQty
                                };
                                initData.InstrumentDetailList.Add(instD);

                                if (lst.Count > 0 && CommonUtil.IsNullOrEmpty(iinst.InstrumentCode) == false)
                                {
                                    foreach (doInstrumentData pInst in lst)
                                    {
                                        if (pInst.InstrumentCode.ToUpper().Trim() == iinst.InstrumentCode.ToUpper().Trim())
                                        {
                                            instD.MaintenanceFlag = pInst.MaintenanceFlag;
                                            instD.InstrumentName = pInst.InstrumentName;
                                            instD.LineUpTypeCode = pInst.LineUpTypeCode;
                                            instD.LineUpTypeName = pInst.LineUpTypeName;
                                            instD.ControllerFlag = pInst.ControllerFlag;
                                            instD.InstrumentFlag = pInst.InstrumentFlag;
                                            instD.SaleFlag = pInst.SaleFlag;
                                            instD.RentalFlag = pInst.RentalFlag;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        /* ------------- */

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
        /// Check quotation detail data in case of sale is correct?
        /// </summary>
        /// <param name="res"></param>
        /// <param name="quotationBasic"></param>
        /// <param name="instLst"></param>
        private void CheckSaleQuotationData(ObjectResultData res, QUS030_tbt_QuotationBasic quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> instLst)
        {
            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;

                #endregion

                doRegisterQuotationData doRegister = new doRegisterQuotationData();
                doRegister.doQuotationHeaderData = doInitData.doQuotationHeaderData;

                doRegister.doTbt_QuotationBasic = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic, tbt_QuotationBasic>(quotationBasic);
                doRegister.doTbt_QuotationBasic.Alphabet = null;
                doRegister.doTbt_QuotationBasic.OriginateProgramId = null;
                doRegister.doTbt_QuotationBasic.OriginateRefNo = null;
                doRegister.doTbt_QuotationBasic.ContractTransferStatus =
                    SECOM_AJIS.Common.Util.ConstantValue.ContractTransferStatus.C_CONTRACT_TRANS_STATUS_QTN_REG;
                doRegister.doTbt_QuotationBasic.LockStatus = SECOM_AJIS.Common.Util.ConstantValue.LockStatus.C_LOCK_STATUS_UNLOCK;
                doRegister.doTbt_QuotationBasic.LastOccNo = null;

                //doRegister.doTbt_QuotationBasic.BidGuaranteeAmount1CurrencyType = quotationBasic.BidGuaranteeAmount1CurrencyType;
                if (doRegister.doTbt_QuotationBasic.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.NewBldMgmtCostUsd = doRegister.doTbt_QuotationBasic.NewBldMgmtCost;
                    doRegister.doTbt_QuotationBasic.NewBldMgmtCost = null;
                    //doRegister.doTbt_QuotationBasic.BidGuaranteeAmount1Usd = doInitData.doQuotationBasic.BidGuaranteeAmount1Usd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.BidGuaranteeAmount1 = doInitData.doQuotationBasic.BidGuaranteeAmount1;
                    doRegister.doTbt_QuotationBasic.NewBldMgmtCostUsd = null;
                }

                //doRegister.doTbt_QuotationBasic.BidGuaranteeAmount1CurrencyType = quotationBasic.BidGuaranteeAmount1CurrencyType;
                if (doRegister.doTbt_QuotationBasic.BidGuaranteeAmount1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.BidGuaranteeAmount1Usd = doRegister.doTbt_QuotationBasic.BidGuaranteeAmount1;
                    doRegister.doTbt_QuotationBasic.BidGuaranteeAmount1 = null;
                    //doRegister.doTbt_QuotationBasic.BidGuaranteeAmount1Usd = doInitData.doQuotationBasic.BidGuaranteeAmount1Usd;
                }
                else
                {
                   //doRegister.doTbt_QuotationBasic.BidGuaranteeAmount1 = doInitData.doQuotationBasic.BidGuaranteeAmount1;
                    doRegister.doTbt_QuotationBasic.BidGuaranteeAmount1Usd = null;
                }

                //doRegister.doTbt_QuotationBasic.BidGuaranteeAmount2CurrencyType = doInitData.doQuotationBasic.BidGuaranteeAmount2CurrencyType;
                if (doRegister.doTbt_QuotationBasic.BidGuaranteeAmount2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.BidGuaranteeAmount2Usd = doRegister.doTbt_QuotationBasic.BidGuaranteeAmount2;
                    doRegister.doTbt_QuotationBasic.BidGuaranteeAmount2 = null;
                    //doRegister.doTbt_QuotationBasic.BidGuaranteeAmount2Usd = doInitData.doQuotationBasic.BidGuaranteeAmount2Usd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.BidGuaranteeAmount2 = doInitData.doQuotationBasic.BidGuaranteeAmount2;
                    doRegister.doTbt_QuotationBasic.BidGuaranteeAmount2Usd = null;
                }

                //doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountCurrencyType = doInitData.doQuotationBasic.InsuranceCoverageAmountCurrencyType;
                if (doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountUsd = doRegister.doTbt_QuotationBasic.InsuranceCoverageAmount;
                    doRegister.doTbt_QuotationBasic.InsuranceCoverageAmount = null;
                    //doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountUsd = doInitData.doQuotationBasic.InsuranceCoverageAmountUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.InsuranceCoverageAmount = doInitData.doQuotationBasic.InsuranceCoverageAmount;
                    doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountUsd = null;
                }

                //doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeCurrencyType = doInitData.doQuotationBasic.MonthlyInsuranceFeeCurrencyType;
                if (doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeUsd = doRegister.doTbt_QuotationBasic.MonthlyInsuranceFee;
                    doRegister.doTbt_QuotationBasic.MonthlyInsuranceFee = null;
                    //doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeUsd = doInitData.doQuotationBasic.MonthlyInsuranceFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.MonthlyInsuranceFee = doInitData.doQuotationBasic.MonthlyInsuranceFee;
                    doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeUsd = null;
                }

                //doRegister.doTbt_QuotationBasic.MaintenanceFee1CurrencyType = doInitData.doQuotationBasic.MaintenanceFee1CurrencyType;
                if (doRegister.doTbt_QuotationBasic.MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1Usd = doRegister.doTbt_QuotationBasic.MaintenanceFee1;
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1 = null;
                    //doRegister.doTbt_QuotationBasic.MaintenanceFee1Usd = doInitData.doQuotationBasic.MaintenanceFee1Usd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.MaintenanceFee1 = doInitData.doQuotationBasic.MaintenanceFee1;
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1Usd = null;
                }

                //doRegister.doTbt_QuotationBasic.AdditionalFee1CurrencyType = doInitData.doQuotationBasic.AdditionalFee1CurrencyType;
                if (doRegister.doTbt_QuotationBasic.AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee1Usd = doRegister.doTbt_QuotationBasic.AdditionalFee1;
                    doRegister.doTbt_QuotationBasic.AdditionalFee1 = null;
                    //doRegister.doTbt_QuotationBasic.AdditionalFee1Usd = doInitData.doQuotationBasic.AdditionalFee1Usd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.AdditionalFee1 = doInitData.doQuotationBasic.AdditionalFee1;
                    doRegister.doTbt_QuotationBasic.AdditionalFee1Usd = null;
                }

                //doRegister.doTbt_QuotationBasic.AdditionalFee2CurrencyType = doInitData.doQuotationBasic.AdditionalFee2CurrencyType;
                if (doRegister.doTbt_QuotationBasic.AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee2Usd = doRegister.doTbt_QuotationBasic.AdditionalFee2;
                    doRegister.doTbt_QuotationBasic.AdditionalFee2 = null;
                    //doRegister.doTbt_QuotationBasic.AdditionalFee2Usd = doInitData.doQuotationBasic.AdditionalFee2Usd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.AdditionalFee2 = doInitData.doQuotationBasic.AdditionalFee2;
                    doRegister.doTbt_QuotationBasic.AdditionalFee2Usd = null;
                }

                //doRegister.doTbt_QuotationBasic.AdditionalFee3CurrencyType = doInitData.doQuotationBasic.AdditionalFee3CurrencyType;
                if (doRegister.doTbt_QuotationBasic.AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee3Usd = doRegister.doTbt_QuotationBasic.AdditionalFee3;
                    doRegister.doTbt_QuotationBasic.AdditionalFee3 = null;
                    //doRegister.doTbt_QuotationBasic.AdditionalFee3Usd = doInitData.doQuotationBasic.AdditionalFee3Usd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.AdditionalFee3 = doInitData.doQuotationBasic.AdditionalFee3;
                    doRegister.doTbt_QuotationBasic.AdditionalFee3Usd = null;
                }

                //doRegister.doTbt_QuotationBasic.SecurityItemFeeCurrencyType = doInitData.doQuotationBasic.SecurityItemFeeCurrencyType;
                if (doRegister.doTbt_QuotationBasic.SecurityItemFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.SecurityItemFeeUsd = doRegister.doTbt_QuotationBasic.SecurityItemFee;
                    doRegister.doTbt_QuotationBasic.SecurityItemFee = null;
                    //doRegister.doTbt_QuotationBasic.SecurityItemFeeUsd = doInitData.doQuotationBasic.SecurityItemFeeUsd;
                }
                else
                {
                   // doRegister.doTbt_QuotationBasic.SecurityItemFee = doInitData.doQuotationBasic.SecurityItemFee;
                    doRegister.doTbt_QuotationBasic.SecurityItemFeeUsd = null;
                }

               // doRegister.doTbt_QuotationBasic.OtherItemFeeCurrencyType = doInitData.doQuotationBasic.OtherItemFeeCurrencyType;
                if (doRegister.doTbt_QuotationBasic.OtherItemFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.OtherItemFeeUsd = doRegister.doTbt_QuotationBasic.OtherItemFee;
                    doRegister.doTbt_QuotationBasic.OtherItemFee = null;
                    //doRegister.doTbt_QuotationBasic.OtherItemFeeUsd = doInitData.doQuotationBasic.OtherItemFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.OtherItemFee = doInitData.doQuotationBasic.OtherItemFee;
                    doRegister.doTbt_QuotationBasic.OtherItemFeeUsd = null;
                }

                //doRegister.doTbt_QuotationBasic.ProductPriceCurrencyType = doInitData.doQuotationBasic.ProductPriceCurrencyType;
                if (doRegister.doTbt_QuotationBasic.ProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.ProductPriceUsd = doRegister.doTbt_QuotationBasic.ProductPrice;
                    doRegister.doTbt_QuotationBasic.ProductPrice = null;
                    //doRegister.doTbt_QuotationBasic.ProductPriceUsd = doInitData.doQuotationBasic.ProductPriceUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.ProductPrice = doInitData.doQuotationBasic.ProductPrice;
                    doRegister.doTbt_QuotationBasic.ProductPriceUsd = null;
                }

                //doRegister.doTbt_QuotationBasic.InstallationFeeCurrencyType = doInitData.doQuotationBasic.InstallationFeeCurrencyType;
                if (doRegister.doTbt_QuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.InstallationFeeUsd = doRegister.doTbt_QuotationBasic.InstallationFee;
                    doRegister.doTbt_QuotationBasic.InstallationFee = null;
                    //doRegister.doTbt_QuotationBasic.InstallationFeeUsd = doInitData.doQuotationBasic.InstallationFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.InstallationFee = doInitData.doQuotationBasic.InstallationFee;
                    doRegister.doTbt_QuotationBasic.InstallationFeeUsd = null;
                }

                //doRegister.doTbt_QuotationBasic.ContractFeeCurrencyType = doInitData.doQuotationBasic.ContractFeeCurrencyType;
                if (doRegister.doTbt_QuotationBasic.ContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.ContractFeeUsd = doRegister.doTbt_QuotationBasic.ContractFee;
                    doRegister.doTbt_QuotationBasic.ContractFee = null;
                    //doRegister.doTbt_QuotationBasic.ContractFeeUsd = doInitData.doQuotationBasic.ContractFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.ContractFee = doInitData.doQuotationBasic.ContractFee;
                    doRegister.doTbt_QuotationBasic.ContractFeeUsd = null;
                }

                //doRegister.doTbt_QuotationBasic.DepositFeeCurrencyType = doInitData.doQuotationBasic.DepositFeeCurrencyType;
                if (doRegister.doTbt_QuotationBasic.DepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.DepositFeeUsd = doRegister.doTbt_QuotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.DepositFee = null;
                    //doRegister.doTbt_QuotationBasic.DepositFeeUsd = doInitData.doQuotationBasic.DepositFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.DepositFee = doInitData.doQuotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.DepositFeeUsd = null;
                }

                bool isQuotation = false;
                if (doInitData.doQuotationHeaderData != null)
                {
                    if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                    {
                        doRegister.doTbt_QuotationBasic.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;

                        if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode
                            == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                            isQuotation = true;
                    }
                }
                if (doInitData.doQuotationBasic != null)
                {
                    if (isQuotation == false)
                        doRegister.doTbt_QuotationBasic.LastOccNo = doInitData.doQuotationBasic.LastOccNo;
                }

                if (doInitData.PriorProductCode == quotationBasic.ProductCode)
                {
                    if (doInitData.doQuotationBasic != null)
                    {
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = doInitData.doQuotationBasic.ContractDurationMonth;
                        doRegister.doTbt_QuotationBasic.AutoRenewMonth = doInitData.doQuotationBasic.AutoRenewMonth;
                    }
                }
                else
                {
                    IProductMasterHandler phandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    List<tbm_Product> pLst = phandler.GetTbm_Product(quotationBasic.ProductCode, null);
                    if (pLst.Count >= 0)
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = pLst[0].DepreciationPeriodContract;

                    doRegister.doTbt_QuotationBasic.AutoRenewMonth = SECOM_AJIS.Common.Util.ConstantValue.Quotation.C_DEFAULT_AUTO_RENEW_MONTH;
                }

                if (instLst != null)
                {
                    doRegister.InstrumentList = new List<tbt_QuotationInstrumentDetails>();
                    foreach (QUS030_tbt_QuotationInstrumentDetails inst in instLst)
                    {
                        if (inst.InstrumentQty > 0)
                        {
                            tbt_QuotationInstrumentDetails oInst = CommonUtil.CloneObject<QUS030_tbt_QuotationInstrumentDetails, tbt_QuotationInstrumentDetails>(inst);
                            oInst.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;
                            oInst.AddQty = 0;
                            oInst.RemoveQty = 0;
                            doRegister.InstrumentList.Add(oInst);
                        }
                    }
                }

                doRegister.doTbt_QuotationInstallationDetail = new tbt_QuotationInstallationDetail()
                {
                    CeilingTypeTBar = (!quotationBasic.CeilingTypeNone && quotationBasic.CeilingTypeTBar),
                    CeilingTypeSlabConcrete = (!quotationBasic.CeilingTypeNone && quotationBasic.CeilingTypeSlabConcrete),
                    CeilingTypeMBar = (!quotationBasic.CeilingTypeNone && quotationBasic.CeilingTypeMBar),
                    CeilingTypeSteel = (!quotationBasic.CeilingTypeNone && quotationBasic.CeilingTypeSteel),
                    CeilingHeight = quotationBasic.CeilingHeight,
                    SpecialInsPVC = quotationBasic.SpecialInsPVC,
                    SpecialInsSLN = quotationBasic.SpecialInsSLN,
                    SpecialInsProtector = quotationBasic.SpecialInsProtector,
                    SpecialInsEMT = quotationBasic.SpecialInsEMT,
                    SpecialInsPE = quotationBasic.SpecialInsPE,
                    SpecialInsOther = quotationBasic.SpecialInsOther,
                    SpecialInsOtherText = quotationBasic.SpecialInsOtherText
                };

                #region Update Session

                param.RegisterData = doRegister;
                QUS030_ScreenData = param;

                #endregion

                res.ResultData = doRegister;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region AL Actions

        /// <summary>
        /// Check quotation detail data in case of alarm (step 1) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <param name="instLst"></param>
        /// <param name="facilityLst"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckALQuotationData_P1(QUS030_tbt_QuotationBasic_AL quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> instLst, List<QUS030_tbt_QuotationInstrumentDetails> facilityLst)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;

                #endregion
                #region Check is Suspending

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                #endregion
                #region Validate Object

                ValidatorUtil validator = new ValidatorUtil(this);

                if (doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                {
                    if (CommonUtil.IsNullOrEmpty(quotationBasic.DispatchTypeCode))
                    {
                        validator.AddErrorMessage(
                            MessageUtil.MODULE_QUOTATION,
                            "QUS030",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "DispathType",
                            "lblDispatchType",
                            "DispatchTypeCode",
                            "2");
                    }
                }

                /* --- Merge --- */
                /* bool isInstEmpty = true;
                if (instLst != null)
                {
                    if (instLst.Count > 0)
                        isInstEmpty = (instLst[0] == null);
                } 
                if (isInstEmpty)
                    validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                "QUS030",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "InstrumentDetail",
                                                doInitData.FirstInstallCompleteFlag == false ? "lblInstrumentDetail_01" : "lblInstrumentDetail_02",
                                                "InstrumentDetail",
                                                "10"); */
                bool isAllEmpty = true;
                bool[][] phoneEmpty = new bool[][] { 
                    new bool[] { true, true }, 
                    new bool[] { true, true }, 
                    new bool[] { true, true } };
                for (int idx = 0; idx < 3; idx++)
                {
                    PropertyInfo propType = quotationBasic.GetType().GetProperty("PhoneLineTypeCode" + (idx + 1).ToString());
                    PropertyInfo propOwner = quotationBasic.GetType().GetProperty("PhoneLineOwnerTypeCode" + (idx + 1).ToString());
                    if (propType != null && propOwner != null)
                    {
                        object propTypeValue = propType.GetValue(quotationBasic, null);
                        object propOwnerValue = propOwner.GetValue(quotationBasic, null);

                        phoneEmpty[idx][0] = CommonUtil.IsNullOrEmpty(propTypeValue);
                        phoneEmpty[idx][1] = CommonUtil.IsNullOrEmpty(propOwnerValue);

                        if (phoneEmpty[idx][0] == false || phoneEmpty[idx][1] == false)
                            isAllEmpty = false;
                    }
                }
                if (isAllEmpty)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                "QUS030",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "PhoneLineInformation",
                                                "lblTelephoneLineInformationError",
                                                "PhoneLineTypeCode1,PhoneLineTypeCode2,PhoneLineTypeCode3,PhoneLineOwnerTypeCode1,PhoneLineOwnerTypeCode2,PhoneLineOwnerTypeCode3",
                                                "3");
                }
                else
                {
                    int order = 3;
                    for (int idx = 0; idx < 3; idx++)
                    {
                        if (phoneEmpty[idx][0] != phoneEmpty[idx][1])
                        {
                            if (phoneEmpty[idx][0] == true)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "PhoneLineTypeCode" + (idx + 1).ToString(),
                                                    "lblPhoneLineTypeCode" + (idx + 1).ToString(),
                                                    "PhoneLineTypeCode" + (idx + 1).ToString(),
                                                    (order).ToString());
                            }
                            else
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "PhoneLineOwnerTypeCode" + (idx + 1).ToString(),
                                                    "lblPhoneLineOwnerTypeCode" + (idx + 1).ToString(),
                                                    "PhoneLineOwnerTypeCode" + (idx + 1).ToString(),
                                                    (order + 1).ToString());
                            }
                        }

                        order += 2;
                    }
                }
                /* ------------- */

                if (quotationBasic.NewBldMgmtFlag == true)
                {
                    if (CommonUtil.IsNullOrEmpty(quotationBasic.NewBldMgmtCost))
                    {
                        /* --- Merge --- */
                        /* validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "NewBldMgmtCost",
                                                    "lblNewBuildingMgmtCost",
                                                    "NewBldMgmtCost",
                                                    "7");
                    } */
                        validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "NewBldMgmtCost",
                                                    "lblNewBuildingMgmtCost",
                                                    "NewBldMgmtCost",
                                                    "21");
                        /* ------------- */

                    }
                }

                /* --- Merge --- */
                for (int idx = 0; idx < 3; idx++)
                {
                    PropertyInfo propApproveNo = quotationBasic.GetType().GetProperty("AdditionalApproveNo" + (idx + 1).ToString());
                    PropertyInfo propApproveFee = quotationBasic.GetType().GetProperty("AdditionalFee" + (idx + 1).ToString());
                    if (propApproveNo != null && propApproveFee != null)
                    {
                        object propApproveNoValue = propApproveNo.GetValue(quotationBasic, null);
                        object propApproveFeeValue = propApproveFee.GetValue(quotationBasic, null);

                        if (CommonUtil.IsNullOrEmpty(propApproveNoValue)
                            || CommonUtil.IsNullOrEmpty(propApproveFeeValue))
                        {
                            string ctrlName = null;
                            string ctrlID = null;
                            if (CommonUtil.IsNullOrEmpty(propApproveNoValue) == false)
                            {
                                ctrlID = "lblAdditionalContractFee" + (idx + 1).ToString();
                                ctrlName = "AdditionalFee" + (idx + 1).ToString();
                            }
                            else if (CommonUtil.IsNullOrEmpty(propApproveFeeValue) == false)
                            {
                                ctrlID = "lblAdditionalApproveNo" + (idx + 1).ToString();
                                ctrlName = "AdditionalApproveNo" + (idx + 1).ToString();
                            }

                            if (ctrlName != null)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                "QUS030",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                ctrlName,
                                                ctrlID,
                                                ctrlName,
                                                (25 + idx).ToString());
                            }
                        }
                    }
                }

                bool isInstEmpty = true;
                if (instLst != null)
                {
                    if (instLst.Count > 0)
                        isInstEmpty = (instLst[0] == null);
                }
                if (isInstEmpty)
                    validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                "QUS030",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "InstrumentDetail",
                                                doInitData.FirstInstallCompleteFlag == false ? "lblInstrumentDetail_01" : "lblInstrumentDetail_02",
                                                "InstrumentDetail",
                                                "28");
                /* ------------------------ */
                /* ------------- */

                ValidatorUtil.BuildErrorMessage(res, validator);
                if (res.IsError)
                    return Json(res);

                #endregion
                #region Validate Instrument/Facility

                string dup_inst = null;
                string dup_facility = null;

                #region Instrument

                for (int i = 0; i < instLst.Count; i++)
                {
                    QUS030_tbt_QuotationInstrumentDetails inst = instLst[i];

                    List<string> eLst = new List<string>();
                    if (CommonUtil.IsNullOrEmpty(inst.InstrumentCode))
                        eLst.Add("headerInstrumentCode");

                    if (CommonUtil.IsNullOrEmpty(inst.InstrumentQty))
                        eLst.Add(doInitData.FirstInstallCompleteFlag == false ? "headerQuantity" : "headerBeforeQuantity");

                    if (doInitData.FirstInstallCompleteFlag == true)
                    {
                        if (CommonUtil.IsNullOrEmpty(inst.AddQty))
                            eLst.Add("headerAdditionalQuantity");
                        if (CommonUtil.IsNullOrEmpty(inst.RemoveQty))
                            eLst.Add("headerRemovalQuantity");
                    }

                    if (eLst.Count > 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                            "QUS030",
                                            MessageUtil.MODULE_QUOTATION,
                                            MessageUtil.MessageList.MSG2079,
                                            new string[] { (i + 1).ToString(), CommonUtil.TextList(eLst.ToArray()) });
                        return Json(res);
                    }

                    if (dup_inst == null)
                    {
                        for (int j = 0; j < instLst.Count; j++)
                        {
                            if (i == j)
                                continue;

                            QUS030_tbt_QuotationInstrumentDetails cinst = instLst[j];

                            /* --- Merge --- */
                            /* if (inst.InstrumentCode == cinst.InstrumentCode)
                            {
                                dup_inst = inst.InstrumentCode;
                                break;
                            } */
                            if (CommonUtil.IsNullOrEmpty(cinst.InstrumentCode) == false)
                            {
                                if (inst.InstrumentCode.ToUpper().Trim() == cinst.InstrumentCode.ToUpper().Trim())
                                {
                                    dup_inst = inst.InstrumentCode;
                                    break;
                                }
                            }
                            /* ------------- */
                        }
                    }
                }

                #endregion
                #region Facility

                for (int i = 0; i < facilityLst.Count; i++)
                {
                    QUS030_tbt_QuotationInstrumentDetails inst = facilityLst[i];

                    List<string> eLst = new List<string>();
                    if (CommonUtil.IsNullOrEmpty(inst.InstrumentCode))
                        eLst.Add("headerFacilityCode");
                    if (CommonUtil.IsNullOrEmpty(inst.InstrumentQty))
                        eLst.Add("headerQuantity");

                    if (eLst.Count > 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                            "QUS030",
                                            MessageUtil.MODULE_QUOTATION,
                                            MessageUtil.MessageList.MSG2075,
                                            new string[] { (i + 1).ToString(), CommonUtil.TextList(eLst.ToArray()) });
                        return Json(res);
                    }

                    if (dup_facility == null)
                    {
                        for (int j = 0; j < facilityLst.Count; j++)
                        {
                            if (i == j)
                                continue;

                            QUS030_tbt_QuotationInstrumentDetails cinst = facilityLst[j];

                            /* --- Merge --- */
                            /* if (inst.InstrumentCode == cinst.InstrumentCode)
                            {
                                dup_facility = inst.InstrumentCode;
                                break;
                            } */
                            if (CommonUtil.IsNullOrEmpty(cinst.InstrumentCode) == false)
                            {
                                if (inst.InstrumentCode.ToUpper().Trim() == cinst.InstrumentCode.ToUpper().Trim())
                                {
                                    dup_facility = inst.InstrumentCode;
                                    break;
                                }
                            }
                            /* ------------- */
                        }
                    }
                }

                #endregion

                if (dup_inst != null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                            MessageUtil.MessageList.MSG2086,
                                            new string[] { dup_inst });
                    return Json(res);
                }
                if (dup_facility != null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                            MessageUtil.MessageList.MSG2090,
                                            new string[] { dup_facility });
                    return Json(res);
                }

                #endregion
                #region Validate Range

                QUS030_ValidateRangeData_AL valRange = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic_AL, QUS030_ValidateRangeData_AL>(quotationBasic);

                Dictionary<string, RangeNumberValueAttribute> rangeAttr =
                    CommonUtil.CreateAttributeDictionary<RangeNumberValueAttribute>(valRange);
                foreach (KeyValuePair<string, RangeNumberValueAttribute> attr in rangeAttr)
                {
                    PropertyInfo prop = valRange.GetType().GetProperty(attr.Key);
                    if (prop != null)
                    {
                        object val = prop.GetValue(valRange, null);
                        if (CommonUtil.IsNullOrEmpty(val) == false)
                        {
                            if (attr.Value.IsValid(val) == false)
                            {
                                string format = "N2";
                                if (prop.PropertyType == typeof(int?))
                                    format = "N0";

                                string min = attr.Value.Min.ToString(format);
                                string max = attr.Value.Max.ToString(format);

                                res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    "QUS030",
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2087,
                                    new string[] { attr.Value.Parameter, min, max },
                                    new string[] { attr.Value.ControlName });
                                return Json(res);
                            }
                        }
                    }
                }

                #region Instrument

                int line = 1;
                foreach (QUS030_tbt_QuotationInstrumentDetails inst in instLst)
                {
                    object valInstRange = null;
                    if (doInitData.FirstInstallCompleteFlag == false)
                        valInstRange = CommonUtil.CloneObject<QUS030_tbt_QuotationInstrumentDetails, QUS030_ValidateRangeData_Instrument1>(inst);
                    else
                        valInstRange = CommonUtil.CloneObject<QUS030_tbt_QuotationInstrumentDetails, QUS030_ValidateRangeData_Instrument2>(inst);

                    rangeAttr = CommonUtil.CreateAttributeDictionary<RangeNumberValueAttribute>(valInstRange);
                    foreach (KeyValuePair<string, RangeNumberValueAttribute> attr in rangeAttr)
                    {
                        PropertyInfo prop = valInstRange.GetType().GetProperty(attr.Key);
                        if (prop != null)
                        {
                            object val = prop.GetValue(valInstRange, null);
                            if (CommonUtil.IsNullOrEmpty(val) == false)
                            {
                                if (attr.Value.IsValid(val) == false)
                                {
                                    string format = "N2";
                                    if (prop.PropertyType == typeof(int?))
                                        format = "N0";

                                    string min = attr.Value.Min.ToString(format);
                                    string max = attr.Value.Max.ToString(format);

                                    res.AddErrorMessage(
                                        MessageUtil.MODULE_QUOTATION,
                                        "QUS030",
                                        MessageUtil.MODULE_QUOTATION,
                                        MessageUtil.MessageList.MSG2088,
                                        new string[] { inst.InstrumentCode, attr.Value.Parameter, min, max },
                                        new string[] { string.Format("InstrumentDetail;{0};{1}", attr.Value.ControlName, line) });
                                    return Json(res);
                                }
                            }
                        }
                    }

                    line++;
                }

                #endregion
                #region Facility

                line = 1;
                foreach (QUS030_tbt_QuotationInstrumentDetails inst in facilityLst)
                {
                    QUS030_ValidateRangeData_Facility valInstRange =
                        CommonUtil.CloneObject<QUS030_tbt_QuotationInstrumentDetails, QUS030_ValidateRangeData_Facility>(inst);

                    rangeAttr = CommonUtil.CreateAttributeDictionary<RangeNumberValueAttribute>(valInstRange);
                    foreach (KeyValuePair<string, RangeNumberValueAttribute> attr in rangeAttr)
                    {
                        PropertyInfo prop = valInstRange.GetType().GetProperty(attr.Key);
                        if (prop != null)
                        {
                            object val = prop.GetValue(valInstRange, null);
                            if (CommonUtil.IsNullOrEmpty(val) == false)
                            {
                                if (attr.Value.IsValid(val) == false)
                                {
                                    string format = "N2";
                                    if (prop.PropertyType == typeof(int?))
                                        format = "N0";

                                    string min = attr.Value.Min.ToString(format);
                                    string max = attr.Value.Max.ToString(format);

                                    res.AddErrorMessage(
                                        MessageUtil.MODULE_QUOTATION,
                                        "QUS030",
                                        MessageUtil.MODULE_QUOTATION,
                                        MessageUtil.MessageList.MSG2091,
                                        new string[] { inst.InstrumentCode, attr.Value.Parameter, min, max },
                                        new string[] { string.Format("FacilityDetail;{0};{1}", attr.Value.ControlName, line) });
                                    return Json(res);
                                }
                            }
                        }
                    }

                    line++;
                }

                #endregion

                #endregion

                /* --- Merge --- */
                #region Validate Plan checking date/Plan approving date

                DateTime nowDate = DateTime.Now;
                nowDate = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day);
                if (CommonUtil.IsNullOrEmpty(quotationBasic.PlanCheckDate) == false)
                {
                    if (quotationBasic.PlanCheckDate > nowDate)
                    {
                        res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    "QUS030",
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2029,
                                    null,
                                    new string[] { "PlanCheckDate" });
                        return Json(res);
                    }
                }
                if (CommonUtil.IsNullOrEmpty(quotationBasic.PlanApproveDate) == false)
                {
                    if (quotationBasic.PlanApproveDate > nowDate)
                    {
                        res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    "QUS030",
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2031,
                                    null,
                                    new string[] { "PlanApproveDate" });
                        return Json(res);
                    }
                }

                #endregion
                #region Security Area From/To
                decimal dfrom = 0;
                decimal dto = 0;
                if (CommonUtil.IsNullOrEmpty(quotationBasic.SecurityAreaFrom) == false)
                    dfrom = quotationBasic.SecurityAreaFrom.Value;
                if (CommonUtil.IsNullOrEmpty(quotationBasic.SecurityAreaTo) == false)
                    dto = quotationBasic.SecurityAreaTo.Value;
                if (dfrom > dto)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                        MessageUtil.MessageList.MSG2063);
                    return Json(res);
                }

                #endregion
                /* ------------- */

                ValidateNewBldMgmtCost(res, quotationBasic);
                if (res.IsError)
                    return Json(res);

                ValidateEmployeeData(res, quotationBasic);
                if (res.IsError)
                    return Json(res);

                if (quotationBasic.InsuranceTypeCode == InsuranceType.C_INSURANCE_TYPE_NONE)
                {
                    bool isError = false;
                    if (CommonUtil.IsNullOrEmpty(quotationBasic.InsuranceCoverageAmount) == false)
                    {
                        if (quotationBasic.InsuranceCoverageAmount > 0)
                            isError = true;
                    }
                    if (CommonUtil.IsNullOrEmpty(quotationBasic.MonthlyInsuranceFee) == false)
                    {
                        if (quotationBasic.MonthlyInsuranceFee > 0)
                            isError = true;
                    }
                    if (isError)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2070);
                        return Json(res);
                    }
                }

                if (doInitData.FirstInstallCompleteFlag == false)
                    ValidateInstrumentDetail01(res, quotationBasic, instLst);
                else
                    ValidateInstrumentDetail02(res, quotationBasic, instLst);
                if (res.IsError)
                    return Json(res);

                ValidateFacilityDetail(res, quotationBasic, facilityLst);
                if (res.IsError)
                    return Json(res);

                ValidateProduct(res, quotationBasic);
                if (res.IsError)
                    return Json(res);
                else if (res.ResultData == null)
                    CheckALQuotationData(res, quotationBasic, instLst, facilityLst);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check quotation detail data in case of alarm (step 2) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <param name="instLst"></param>
        /// <param name="facilityLst"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckALQuotationData_P2(QUS030_tbt_QuotationBasic_AL quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> instLst, List<QUS030_tbt_QuotationInstrumentDetails> facilityLst)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CheckALQuotationData(res, quotationBasic, instLst, facilityLst);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region AL Methods

        /// <summary>
        /// Initial quotation detail data in case of alarm
        /// </summary>
        /// <param name="dsRentalContractData"></param>
        /// <returns></returns>
        private doInitRentalQuotationData InitALQuotationData(dsRentalContractData dsRentalContractData)
        {
            try
            {
                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (dsRentalContractData != null)
                {
                    doInitRentalQuotationData initDo = new doInitRentalQuotationData();

                    if (dsRentalContractData.dtTbt_RentalSecurityBasic != null)
                    {
                        if (dsRentalContractData.dtTbt_RentalSecurityBasic.Count > 0)
                        {
                            #region Mapping Employee Name

                            IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                            EmployeeMappingList empMLst = new EmployeeMappingList();
                            empMLst.AddEmployee(dsRentalContractData.dtTbt_RentalSecurityBasic[0]);
                            ehandler.ActiveEmployeeListMapping(empMLst);

                            #endregion
                            #region Create Quotation Basic

                            initDo.doQuotationBasic = new tbt_QuotationBasic()
                            {
                                ProductCode = dsRentalContractData.dtTbt_RentalSecurityBasic[0].ProductCode,
                                SecurityTypeCode = dsRentalContractData.dtTbt_RentalSecurityBasic[0].SecurityTypeCode,
                                DispatchTypeCode = dsRentalContractData.dtTbt_RentalSecurityBasic[0].DispatchTypeCode,
                                ContractDurationMonth = dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractDurationMonth,
                                AutoRenewMonth = dsRentalContractData.dtTbt_RentalSecurityBasic[0].AutoRenewMonth,

                                //LastOccNo = dsRentalContractData.dtTbt_RentalSecurityBasic[0].OCC,
                                LastOccNo = dsRentalContractData.dtTbt_RentalContractBasic[0].LastOCC,

                                CurrentSecurityTypeCode = dsRentalContractData.dtTbt_RentalSecurityBasic[0].SecurityTypeCode,
                                PhoneLineTypeCode1 = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1,
                                PhoneLineTypeCode2 = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode2,
                                PhoneLineTypeCode3 = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode3,
                                PhoneLineOwnerTypeCode1 = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1,
                                PhoneLineOwnerTypeCode2 = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode2,
                                PhoneLineOwnerTypeCode3 = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode3,
                                FireMonitorFlag = dsRentalContractData.dtTbt_RentalSecurityBasic[0].FireMonitorFlag,
                                CrimePreventFlag = dsRentalContractData.dtTbt_RentalSecurityBasic[0].CrimePreventFlag,
                                EmergencyReportFlag = dsRentalContractData.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag,
                                FacilityMonitorFlag = dsRentalContractData.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag,
                                PlanCode = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PlanCode,
                                //QuotationNo = param.InitialData.doQuotationBasic.QuotationNo,
                                SpecialInstallationFlag = dsRentalContractData.dtTbt_RentalSecurityBasic[0].SpecialInstallationFlag,
                                PlannerEmpNo = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PlannerEmpNo,
                                PlannerName = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PlannerName,
                                PlanCheckerEmpNo = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PlanCheckerEmpNo,
                                PlanCheckerName = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PlanCheckerName,
                                PlanCheckDate = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PlanCheckDate,
                                PlanApproverEmpNo = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PlanApproverEmpNo,
                                PlanApproverName = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PlanApproverName,
                                PlanApproveDate = dsRentalContractData.dtTbt_RentalSecurityBasic[0].PlanApproveDate,
                                SiteBuildingArea = dsRentalContractData.dtTbt_RentalSecurityBasic[0].SiteBuildingArea,
                                SecurityAreaFrom = dsRentalContractData.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom,
                                SecurityAreaTo = dsRentalContractData.dtTbt_RentalSecurityBasic[0].SecurityAreaTo,
                                MainStructureTypeCode = dsRentalContractData.dtTbt_RentalSecurityBasic[0].MainStructureTypeCode,
                                BuildingTypeCode = dsRentalContractData.dtTbt_RentalSecurityBasic[0].BuildingTypeCode,
                                NewBldMgmtFlag = dsRentalContractData.dtTbt_RentalSecurityBasic[0].NewBldMgmtFlag,
                                NewBldMgmtCost = dsRentalContractData.dtTbt_RentalSecurityBasic[0].NewBldMgmtCost,
                                NewBldMgmtCostCurrencyType = dsRentalContractData.dtTbt_RentalSecurityBasic[0].NewBldMgmtCostCurrencyType,
                                NewBldMgmtCostUsd = dsRentalContractData.dtTbt_RentalSecurityBasic[0].NewBldMgmtCostUsd,
                                NumOfBuilding = dsRentalContractData.dtTbt_RentalSecurityBasic[0].NumOfBuilding,
                                NumOfFloor = dsRentalContractData.dtTbt_RentalSecurityBasic[0].NumOfFloor,
                                FacilityPassYear = dsRentalContractData.dtTbt_RentalSecurityBasic[0].FacilityPassYear,
                                FacilityPassMonth = dsRentalContractData.dtTbt_RentalSecurityBasic[0].FacilityPassMonth,
                                FacilityMemo = dsRentalContractData.dtTbt_RentalSecurityBasic[0].FacilityMemo,
                                MaintenanceCycle = dsRentalContractData.dtTbt_RentalSecurityBasic[0].MaintenanceCycle,
                                /* Commented by anancha */
                                /* MaintenanceFee2 = dsRentalContractData.dtTbt_RentalSecurityBasic[0].MaintenanceFee2*/
                            };

                            #endregion

                            if (dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_BEF_START
                                && dsRentalContractData.dtTbt_RentalContractBasic[0].StartType != StartType.C_START_TYPE_ALTER_START)
                            {
                                initDo.doQuotationBasic.PlanCode = null;
                                initDo.doQuotationBasic.QuotationNo = null;
                                initDo.doQuotationBasic.SpecialInstallationFlag = null;
                                initDo.doQuotationBasic.PlannerEmpNo = null;
                                initDo.doQuotationBasic.PlannerName = null;
                                initDo.doQuotationBasic.PlanCheckerEmpNo = null;
                                initDo.doQuotationBasic.PlanCheckerName = null;
                                initDo.doQuotationBasic.PlanCheckDate = null;
                                initDo.doQuotationBasic.PlanApproverEmpNo = null;
                                initDo.doQuotationBasic.PlanApproverName = null;
                                initDo.doQuotationBasic.PlanApproveDate = null;
                            }
                        }
                        if (dsRentalContractData.dtTbt_RentalOperationType != null)
                        {
                            if (dsRentalContractData.dtTbt_RentalOperationType.Count > 0)
                            {
                                initDo.OperationTypeList = new List<doQuotationOperationType>();
                                foreach (tbt_RentalOperationType rODo in dsRentalContractData.dtTbt_RentalOperationType)
                                {
                                    initDo.OperationTypeList.Add(new doQuotationOperationType()
                                    {
                                        QuotationTargetCode = rODo.ContractCode,
                                        OperationTypeCode = rODo.OperationTypeCode
                                    });
                                }
                            }
                        }
                        if (dsRentalContractData.dtTbt_RentalInstrumentDetails != null)
                        {
                            if (dsRentalContractData.dtTbt_RentalInstrumentDetails.Count > 0)
                            {
                                #region Instrument Master

                                List<tbm_Instrument> iLst = new List<tbm_Instrument>();
                                foreach (tbt_RentalInstrumentDetails riDo in dsRentalContractData.dtTbt_RentalInstrumentDetails)
                                {
                                    iLst.Add(new tbm_Instrument()
                                    {
                                        InstrumentCode = riDo.InstrumentCode
                                    });
                                }
                                IInstrumentMasterHandler inshandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;

                                /* --- Merge --- */
                                /* List<tbm_Instrument> insLst = inshandler.GetIntrumentList(iLst); */
                                List<doInstrumentData> insLst = inshandler.GetInstrumentListForView(iLst);
                                /* ------------- */

                                #endregion
                                #region Create Instrument / Facility

                                foreach (tbt_RentalInstrumentDetails riDo in dsRentalContractData.dtTbt_RentalInstrumentDetails)
                                {
                                    /* --- Merge --- */
                                    /* tbm_Instrument ins = new tbm_Instrument();
                                    if (insLst != null)
                                    {
                                        foreach (tbm_Instrument i in insLst)
                                        {
                                            if (i.InstrumentCode == riDo.InstrumentCode)
                                            {
                                                ins = i;
                                                break;
                                            }
                                        }
                                    } */
                                    doInstrumentData ins = new doInstrumentData();
                                    if (insLst != null)
                                    {
                                        foreach (doInstrumentData i in insLst)
                                        {
                                            if (i.InstrumentCode.ToUpper().Trim() == riDo.InstrumentCode.ToUpper().Trim())
                                            {
                                                ins = i;
                                                break;
                                            }
                                        }
                                    }
                                    /* ------------- */

                                    if (riDo.InstrumentTypeCode == SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_GENERAL)
                                    {
                                        if (initDo.InstrumentDetailList == null)
                                            initDo.InstrumentDetailList = new List<doInstrumentDetail>();

                                        doInstrumentDetail nins = new doInstrumentDetail()
                                        {
                                            InstrumentCode = riDo.InstrumentCode,
                                            InstrumentName = ins.InstrumentName,
                                            MaintenanceFlag = ins.MaintenanceFlag,
                                            LineUpTypeCode = ins.LineUpTypeCode,

                                            /* --- Merge --- */
                                            LineUpTypeName = ins.LineUpTypeName,
                                            /* ------------- */

                                            AddQty = 0,
                                            RemoveQty = 0,
                                            ControllerFlag = ins.ControllerFlag,
                                            InstrumentFlag = ins.InstrumentFlag,
                                            SaleFlag = ins.SaleFlag,
                                            RentalFlag = ins.RentalFlag
                                        };

                                        if ((dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
                                             || dsRentalContractData.dtTbt_RentalContractBasic[0].StartType == StartType.C_START_TYPE_ALTER_START)
                                                && dsRentalContractData.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag == false)
                                        {
                                            nins.InstrumentQty = riDo.InstrumentQty;
                                        }
                                        else
                                        {
                                            nins.InstrumentQty = riDo.InstrumentQty;
                                            if (CommonUtil.IsNullOrEmpty(riDo.InstrumentQty) == false)
                                            {
                                                if (CommonUtil.IsNullOrEmpty(riDo.AdditionalInstrumentQty) == false)
                                                    nins.InstrumentQty += riDo.AdditionalInstrumentQty;
                                                if (CommonUtil.IsNullOrEmpty(riDo.RemovalInstrumentQty) == false)
                                                    nins.InstrumentQty -= riDo.RemovalInstrumentQty;
                                            }
                                        }

                                        initDo.InstrumentDetailList.Add(nins);
                                    }
                                    else if (riDo.InstrumentTypeCode == SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_MONITOR)
                                    {
                                        if (initDo.FacilityDetailList == null)
                                            initDo.FacilityDetailList = new List<doFacilityDetail>();

                                        initDo.FacilityDetailList.Add(new doFacilityDetail()
                                        {
                                            FacilityCode = riDo.InstrumentCode,
                                            FacilityName = ins.InstrumentName,
                                            FacilityQty = riDo.InstrumentQty
                                        });
                                    }
                                }

                                #endregion

                                /* --- Merge --- */
                                /* #region Get Misc Name

                                if (initDo.InstrumentDetailList != null)
                                {
                                    MiscTypeMappingList miscLst = new MiscTypeMappingList();
                                    miscLst.AddMiscType(initDo.InstrumentDetailList.ToArray());

                                    ICommonHandler cmmhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                                    cmmhandler.MiscTypeMappingList(miscLst);
                                }

                                #endregion */
                                /* ------------- */

                            }
                        }
                    }

                    return initDo;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Initial quotation detail data in case of alarm from import file
        /// </summary>
        /// <param name="res"></param>
        /// <param name="initData"></param>
        /// <param name="importData"></param>
        private void InitImportALQuotation(ObjectResultData res, doInitQuotationData initData, dsImportData importData)
        {
            try
            {
                #region Create Quotation Basic

                if (importData.dtTbt_QuotationBasic != null)
                {
                    if (importData.dtTbt_QuotationBasic.Count > 0)
                    {
                        tbt_QuotationBasic nDo = importData.dtTbt_QuotationBasic[0];
                        if (initData.doQuotationBasic != null)
                        {
                            nDo.CurrentSecurityTypeCode = initData.doQuotationBasic.SecurityTypeCode;
                            nDo.LastOccNo = initData.doQuotationBasic.LastOccNo;

                            if (initData.doQuotationBasic.ProductCode == nDo.ProductCode)
                            {
                                nDo.ContractDurationMonth = initData.doQuotationBasic.ContractDurationMonth;
                                nDo.AutoRenewMonth = initData.doQuotationBasic.AutoRenewMonth;
                            }
                        }

                        if (initData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                            nDo.DepositFee = null;

                        initData.doQuotationBasic = nDo;
                    }
                }

                #endregion
                #region Prepare Operation Type

                initData.OperationTypeList = new List<doQuotationOperationType>();
                if (importData.dtTbt_QuotationOperationType != null)
                {
                    if (importData.dtTbt_QuotationOperationType.Count > 0)
                        initData.OperationTypeList = CommonUtil.ClonsObjectList<tbt_QuotationOperationType, doQuotationOperationType>(importData.dtTbt_QuotationOperationType);
                }

                #endregion
                #region Prepare Instrument Detail

                bool isNullImportInst = true;
                if (importData.dtTbt_QuotationInstrumentDetails != null)
                {
                    if (importData.dtTbt_QuotationInstrumentDetails.Count > 0)
                        isNullImportInst = false;
                }
                if (isNullImportInst)
                    initData.InstrumentDetailList = new List<doInstrumentDetail>();
                else
                {
                    #region Get Master Instrument

                    /* --- Merge --- */
                    /* List<doParentGeneralInstrument> mInstLst = new List<doParentGeneralInstrument>();
                    foreach (tbt_QuotationInstrumentDetails inst in importData.dtTbt_QuotationInstrumentDetails)
                    {
                        mInstLst.Add(new doParentGeneralInstrument()
                        {
                            InstrumentCode = inst.InstrumentCode
                        });
                    } */
                    List<tbm_Instrument> mInstLst = new List<tbm_Instrument>();
                    foreach (tbt_QuotationInstrumentDetails inst in importData.dtTbt_QuotationInstrumentDetails)
                    {
                        if (CommonUtil.IsNullOrEmpty(inst.InstrumentCode) == false)
                        {
                            mInstLst.Add(new tbm_Instrument()
                            {
                                InstrumentCode = inst.InstrumentCode
                            });
                        }
                    }
                    /* ------------- */


                    IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;

                    /* --- Merge --- */
                    /* List<doParentGeneralInstrument> masterInstLst = ihandler.GetParentGeneralInstrumentList(mInstLst); */
                    List<doInstrumentData> masterInstLst = ihandler.GetInstrumentListForView(mInstLst);
                    /* ------------- */

                    #endregion

                    List<doInstrumentDetail> instDetailLst = new List<doInstrumentDetail>();
                    foreach (tbt_QuotationInstrumentDetails inst in importData.dtTbt_QuotationInstrumentDetails)
                    {
                        /* --- Merge --- */
                        /* doParentGeneralInstrument masterInst = null;
                        foreach (doParentGeneralInstrument mInst in masterInstLst)
                        {
                            if (mInst.InstrumentCode == inst.InstrumentCode)
                            {
                                masterInst = mInst;
                                break;
                            }
                        } */
                        doInstrumentData masterInst = null;
                        if (CommonUtil.IsNullOrEmpty(inst.InstrumentCode) == false)
                        {
                            foreach (doInstrumentData mInst in masterInstLst)
                            {
                                if (mInst.InstrumentCode.ToUpper().Trim() == inst.InstrumentCode.ToUpper().Trim())
                                {
                                    masterInst = mInst;
                                    break;
                                }
                            }
                        }
                        /* ------------- */


                        #region Add Instrument

                        doInstrumentDetail nInst = new doInstrumentDetail()
                        {
                            InstrumentCode = inst.InstrumentCode,
                            InstrumentQty = inst.InstrumentQty,
                            AddQty = inst.AddQty,
                            RemoveQty = inst.RemoveQty
                        };
                        if (masterInst != null)
                        {
                            nInst.MaintenanceFlag = masterInst.MaintenanceFlag;
                            nInst.InstrumentName = masterInst.InstrumentName;
                            nInst.LineUpTypeCode = masterInst.LineUpTypeCode;
                            nInst.LineUpTypeName = masterInst.LineUpTypeName;
                            nInst.ControllerFlag = masterInst.ControllerFlag;
                            nInst.InstrumentFlag = masterInst.InstrumentFlag;
                            nInst.SaleFlag = masterInst.SaleFlag;
                            nInst.RentalFlag = masterInst.RentalFlag;
                        }
                        instDetailLst.Add(nInst);

                        #endregion
                    }

                    initData.InstrumentDetailList = instDetailLst;
                }

                #endregion
                #region Prepare Facility Detail

                bool isNullImportFacility = true;
                if (importData.dtTbt_QuotationFacilityDetails != null)
                {
                    if (importData.dtTbt_QuotationFacilityDetails.Count > 0)
                        isNullImportFacility = false;
                }
                if (isNullImportFacility)
                    initData.FacilityDetailList = new List<doFacilityDetail>();
                else
                {
                    #region Get Master Facility

                    /* --- Merge --- */
                    /* List<doMonitoringInstrument> fLst = new List<doMonitoringInstrument>();
                    foreach (tbt_QuotationFacilityDetails fd in importData.dtTbt_QuotationFacilityDetails)
                    {
                        fLst.Add(new doMonitoringInstrument()
                        {
                            InstrumentCode = fd.FacilityCode
                        });
                    }

                    IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<doMonitoringInstrument> instLst = ihandler.GetMonitoringInstrumentList(fLst); */
                    List<tbm_Instrument> fLst = new List<tbm_Instrument>();
                    foreach (tbt_QuotationFacilityDetails fd in importData.dtTbt_QuotationFacilityDetails)
                    {
                        if (CommonUtil.IsNullOrEmpty(fd.FacilityCode) == false)
                        {
                            fLst.Add(new tbm_Instrument()
                            {
                                InstrumentCode = fd.FacilityCode
                            });
                        }
                    }

                    IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<tbm_Instrument> instLst = ihandler.GetIntrumentList(fLst);
                    /* ------------- */

                    #endregion

                    List<doFacilityDetail> facilityDetailLst = new List<doFacilityDetail>();
                    foreach (tbt_QuotationFacilityDetails facility in importData.dtTbt_QuotationFacilityDetails)
                    {
                        /* --- Merge --- */
                        /* doMonitoringInstrument masterInst = null;
                        foreach (doMonitoringInstrument mInst in instLst)
                        {
                            if (mInst.InstrumentCode == facility.FacilityCode)
                            {
                                masterInst = mInst;
                                break;
                            }
                        } */
                        tbm_Instrument masterInst = null;
                        if (CommonUtil.IsNullOrEmpty(facility.FacilityCode) == false)
                        {
                            foreach (tbm_Instrument mInst in instLst)
                            {
                                if (mInst.InstrumentCode.ToUpper().Trim() == facility.FacilityCode.ToUpper().Trim())
                                {
                                    masterInst = mInst;
                                    break;
                                }
                            }
                        }
                        /* ------------- */


                        #region Add Facility

                        doFacilityDetail fd = new doFacilityDetail()
                        {
                            FacilityCode = facility.FacilityCode,
                            FacilityQty = facility.FacilityQty
                        };
                        if (masterInst != null)
                        {
                            fd.FacilityName = masterInst.InstrumentName;
                        }
                        facilityDetailLst.Add(fd);

                        #endregion
                    }

                    initData.FacilityDetailList = facilityDetailLst;
                }

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check quotation detail data in case of alarm is correct?
        /// </summary>
        /// <param name="res"></param>
        /// <param name="quotationBasic"></param>
        /// <param name="instLst"></param>
        /// <param name="facilityLst"></param>
        private void CheckALQuotationData(ObjectResultData res, QUS030_tbt_QuotationBasic_AL quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> instLst, List<QUS030_tbt_QuotationInstrumentDetails> facilityLst)
        {
            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;

                #endregion

                doRegisterQuotationData doRegister = new doRegisterQuotationData();
                doRegister.doQuotationHeaderData = doInitData.doQuotationHeaderData;

                doRegister.doTbt_QuotationBasic = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic_AL, tbt_QuotationBasic>(quotationBasic);

                if (doInitData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    doRegister.doTbt_QuotationBasic.DispatchTypeCode = null;
                }

                doRegister.doTbt_QuotationBasic.Alphabet = null;
                doRegister.doTbt_QuotationBasic.OriginateProgramId = null;
                doRegister.doTbt_QuotationBasic.OriginateRefNo = null;
                doRegister.doTbt_QuotationBasic.ContractTransferStatus =
                    SECOM_AJIS.Common.Util.ConstantValue.ContractTransferStatus.C_CONTRACT_TRANS_STATUS_QTN_REG;
                doRegister.doTbt_QuotationBasic.LockStatus = SECOM_AJIS.Common.Util.ConstantValue.LockStatus.C_LOCK_STATUS_UNLOCK;

                doRegister.doTbt_QuotationBasic.LastOccNo = null;
                doRegister.doTbt_QuotationBasic.CurrentSecurityTypeCode = null;

                doRegister.doTbt_QuotationBasic.FireMonitorFlag = false;
                doRegister.doTbt_QuotationBasic.CrimePreventFlag = false;
                doRegister.doTbt_QuotationBasic.EmergencyReportFlag = false;
                doRegister.doTbt_QuotationBasic.FacilityMonitorFlag = false;
                doRegister.doTbt_QuotationBasic.BeatGuardFlag = false;
                doRegister.doTbt_QuotationBasic.SentryGuardFlag = false;
                doRegister.doTbt_QuotationBasic.MaintenanceFlag = false;
                doRegister.doTbt_QuotationBasic.SaleOnlineContractCode = null;

                //currency

                if (doRegister.doTbt_QuotationBasic.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.NewBldMgmtCostUsd = doRegister.doTbt_QuotationBasic.NewBldMgmtCost;
                    doRegister.doTbt_QuotationBasic.NewBldMgmtCost = null;
              }
                else
                {
                    doRegister.doTbt_QuotationBasic.NewBldMgmtCostUsd = null;
                }

                if (doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountUsd = doRegister.doTbt_QuotationBasic.InsuranceCoverageAmount;
                    doRegister.doTbt_QuotationBasic.InsuranceCoverageAmount = null;
                    //doRegister.doTbt_QuotationBasic.DepositFeeUsd = doInitData.doQuotationBasic.DepositFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.DepositFee = doInitData.doQuotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountUsd = null;
                }

                if (doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeUsd = doRegister.doTbt_QuotationBasic.MonthlyInsuranceFee;
                    doRegister.doTbt_QuotationBasic.MonthlyInsuranceFee = null;
                    //doRegister.doTbt_QuotationBasic.DepositFeeUsd = doInitData.doQuotationBasic.DepositFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.DepositFee = doInitData.doQuotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeUsd = null;
                }

                if (doRegister.doTbt_QuotationBasic.MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1Usd = doRegister.doTbt_QuotationBasic.MaintenanceFee1;
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1 = null;
                    //doRegister.doTbt_QuotationBasic.DepositFeeUsd = doInitData.doQuotationBasic.DepositFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.DepositFee = doInitData.doQuotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1Usd = null;
                }

                if (doRegister.doTbt_QuotationBasic.AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee1Usd = doRegister.doTbt_QuotationBasic.AdditionalFee1;
                    doRegister.doTbt_QuotationBasic.AdditionalFee1 = null;
                    //doRegister.doTbt_QuotationBasic.DepositFeeUsd = doInitData.doQuotationBasic.DepositFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.DepositFee = doInitData.doQuotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.AdditionalFee1Usd = null;
                }

                if (doRegister.doTbt_QuotationBasic.AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee2Usd = doRegister.doTbt_QuotationBasic.AdditionalFee2;
                    doRegister.doTbt_QuotationBasic.AdditionalFee2 = null;
                    //doRegister.doTbt_QuotationBasic.DepositFeeUsd = doInitData.doQuotationBasic.DepositFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.DepositFee = doInitData.doQuotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.AdditionalFee2Usd = null;
                }

                if (doRegister.doTbt_QuotationBasic.AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee3Usd = doRegister.doTbt_QuotationBasic.AdditionalFee3;
                    doRegister.doTbt_QuotationBasic.AdditionalFee3 = null;
                    //doRegister.doTbt_QuotationBasic.DepositFeeUsd = doInitData.doQuotationBasic.DepositFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.DepositFee = doInitData.doQuotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.AdditionalFee3Usd = null;
                }
                if (doRegister.doTbt_QuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.InstallationFeeUsd = doRegister.doTbt_QuotationBasic.InstallationFee;
                    doRegister.doTbt_QuotationBasic.InstallationFee = null;
                    //doRegister.doTbt_QuotationBasic.InstallationFeeUsd = doInitData.doQuotationBasic.InstallationFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.InstallationFee = doInitData.doQuotationBasic.InstallationFee;
                    doRegister.doTbt_QuotationBasic.InstallationFeeUsd = null;
                }
                if (doRegister.doTbt_QuotationBasic.DepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.DepositFeeUsd = doRegister.doTbt_QuotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.DepositFee = null;
                    //doRegister.doTbt_QuotationBasic.DepositFeeUsd = doInitData.doQuotationBasic.DepositFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.DepositFee = doInitData.doQuotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.DepositFeeUsd = null;
                }
                if (doRegister.doTbt_QuotationBasic.ContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.ContractFeeUsd = doRegister.doTbt_QuotationBasic.ContractFee;
                    doRegister.doTbt_QuotationBasic.ContractFee = null;
                    //doRegister.doTbt_QuotationBasic.DepositFeeUsd = doInitData.doQuotationBasic.DepositFeeUsd;
                }
                else
                {
                    //doRegister.doTbt_QuotationBasic.DepositFee = doInitData.doQuotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.ContractFeeUsd = null;
                }

                if (quotationBasic.ServiceType != null)
                {
                    foreach (string st in quotationBasic.ServiceType)
                    {
                        if (st == CommonValue.QUOTATION_SERVICE_TYPE_FIRE_MONITORING)
                            doRegister.doTbt_QuotationBasic.FireMonitorFlag = true;
                        if (st == CommonValue.QUOTATION_SERVICE_TYPE_CRIME_PREVENTION)
                            doRegister.doTbt_QuotationBasic.CrimePreventFlag = true;
                        if (st == CommonValue.QUOTATION_SERVICE_TYPE_EMERGENCY_REPORT)
                            doRegister.doTbt_QuotationBasic.EmergencyReportFlag = true;
                        if (st == CommonValue.QUOTATION_SERVICE_TYPE_FACILITY_MONITORING)
                            doRegister.doTbt_QuotationBasic.FacilityMonitorFlag = true;
                    }
                }

                bool isQuotation = false;
                if (doInitData.doQuotationHeaderData != null)
                {
                    if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                    {
                        doRegister.doTbt_QuotationBasic.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;

                        if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode
                            == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                            isQuotation = true;
                    }
                }
                if (doInitData.doQuotationBasic != null)
                {
                    if (isQuotation == false)
                    {
                        doRegister.doTbt_QuotationBasic.LastOccNo = doInitData.doQuotationBasic.LastOccNo;
                        doRegister.doTbt_QuotationBasic.CurrentSecurityTypeCode = doInitData.doQuotationBasic.CurrentSecurityTypeCode;
                    }

                    doRegister.doTbt_QuotationBasic.FacilityPassYear = doInitData.doQuotationBasic.FacilityPassYear;
                    doRegister.doTbt_QuotationBasic.FacilityPassMonth = doInitData.doQuotationBasic.FacilityPassMonth;
                    doRegister.doTbt_QuotationBasic.MaintenanceFee2 = doInitData.doQuotationBasic.MaintenanceFee2;
                }

                if (doInitData.PriorProductCode == quotationBasic.ProductCode)
                {
                    if (doInitData.doQuotationBasic != null)
                    {
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = doInitData.doQuotationBasic.ContractDurationMonth;
                        doRegister.doTbt_QuotationBasic.AutoRenewMonth = doInitData.doQuotationBasic.AutoRenewMonth;
                    }
                }
                else
                {
                    IProductMasterHandler phandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    List<tbm_Product> pLst = phandler.GetTbm_Product(quotationBasic.ProductCode, null);
                    if (pLst.Count >= 0)
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = pLst[0].DepreciationPeriodContract;

                    doRegister.doTbt_QuotationBasic.AutoRenewMonth = SECOM_AJIS.Common.Util.ConstantValue.Quotation.C_DEFAULT_AUTO_RENEW_MONTH;
                }

                if (quotationBasic.OperationType != null)
                {
                    doRegister.OperationList = new List<tbt_QuotationOperationType>();
                    foreach (string op in quotationBasic.OperationType)
                    {
                        tbt_QuotationOperationType opt = new tbt_QuotationOperationType()
                        {
                            QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode,
                            OperationTypeCode = op
                        };
                        doRegister.OperationList.Add(opt);
                    }
                }

                if (instLst != null)
                {
                    doRegister.InstrumentList = new List<tbt_QuotationInstrumentDetails>();
                    foreach (QUS030_tbt_QuotationInstrumentDetails inst in instLst)
                    {
                        if (doInitData.FirstInstallCompleteFlag == false)
                        {
                            if (inst.InstrumentQty > 0)
                            {
                                tbt_QuotationInstrumentDetails oInst = CommonUtil.CloneObject<QUS030_tbt_QuotationInstrumentDetails, tbt_QuotationInstrumentDetails>(inst);
                                oInst.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;
                                oInst.AddQty = 0;
                                oInst.RemoveQty = 0;
                                doRegister.InstrumentList.Add(oInst);
                            }
                        }
                        else
                        {
                            tbt_QuotationInstrumentDetails oInst = CommonUtil.CloneObject<QUS030_tbt_QuotationInstrumentDetails, tbt_QuotationInstrumentDetails>(inst);
                            oInst.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;
                            doRegister.InstrumentList.Add(oInst);
                        }
                    }
                }

                if (facilityLst != null)
                {
                    doRegister.FacilityList = new List<tbt_QuotationFacilityDetails>();
                    foreach (QUS030_tbt_QuotationInstrumentDetails inst in facilityLst)
                    {
                        if (inst.InstrumentQty > 0)
                        {
                            tbt_QuotationFacilityDetails facility = new tbt_QuotationFacilityDetails()
                            {
                                QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode,
                                FacilityCode = inst.InstrumentCode,
                                FacilityQty = inst.InstrumentQty
                            };
                            doRegister.FacilityList.Add(facility);
                        }
                    }
                }

                doRegister.doTbt_QuotationInstallationDetail = new tbt_QuotationInstallationDetail()
                {
                    CeilingTypeTBar = (!quotationBasic.CeilingTypeNone && quotationBasic.CeilingTypeTBar),
                    CeilingTypeSlabConcrete = (!quotationBasic.CeilingTypeNone && quotationBasic.CeilingTypeSlabConcrete),
                    CeilingTypeMBar = (!quotationBasic.CeilingTypeNone && quotationBasic.CeilingTypeMBar),
                    CeilingTypeSteel = (!quotationBasic.CeilingTypeNone && quotationBasic.CeilingTypeSteel),
                    CeilingHeight = quotationBasic.CeilingHeight,
                    SpecialInsPVC = quotationBasic.SpecialInsPVC,
                    SpecialInsSLN = quotationBasic.SpecialInsSLN,
                    SpecialInsProtector = quotationBasic.SpecialInsProtector,
                    SpecialInsEMT = quotationBasic.SpecialInsEMT,
                    SpecialInsPE = quotationBasic.SpecialInsPE,
                    SpecialInsOther = quotationBasic.SpecialInsOther,
                    SpecialInsOtherText = quotationBasic.SpecialInsOtherText
                };

                #region Update Session

                param.RegisterData = doRegister;
                QUS030_ScreenData = param;

                #endregion

                res.ResultData = doRegister;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Sale Online Actions

        /// <summary>
        /// Retrieve linkage sale contract data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult QUS030_RetrieveLinkageSaleContractData(QUS030_GetLinkageSaleContractCondition cond)
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
                cond.SaleOnlineContractCode = cmm.ConvertContractCode(cond.SaleOnlineContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                cond.QuotationTargetCode = cmm.ConvertQuotationTargetCode(cond.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                /* --- Merge --- */
                /* doLinkageSaleContractData linkageSaleContractData = GetLinkageSaleContractData(cond); */
                ISaleContractHandler chandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                doSaleContractData doSaleContractData = chandler.CheckLinkageSaleContract(cond.SaleOnlineContractCode, cond.QuotationTargetCode); //TODO: Bug report QU-124 Add param quotation target code

                IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                doLinkageSaleContractData linkageSaleContractData = qhandler.InitLinkageSaleContractData(doSaleContractData);
                /* ------------- */

                #region Update Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param == null)
                    param = new QUS030_ScreenParameter();
                param.LinkageSaleContractData = linkageSaleContractData;
                QUS030_ScreenData = param;

                #endregion

                res.ResultData = linkageSaleContractData;

            }
            catch (ApplicationErrorException ex)
            {
                ex.ErrorResult.Message.Controls = new string[] { "SaleOnlineContractCode" };
                res.AddErrorMessage(ex);

                if (ex.ErrorResult.Message.Code != MessageUtil.MessageList.MSG0093.ToString())
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check quotation detail data in case of sale online (step 1) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <param name="facilityLst"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckSaleOnlineQuotationData_P1(QUS030_tbt_QuotationBasic_ONLINE quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> facilityLst)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;

                #endregion
                #region Check is Suspending

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                #endregion
                #region Validate Object

                /* --- Merge --- */
                /* if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                } */
                ValidatorUtil validator = new ValidatorUtil(this);

                bool isAllEmpty = true;
                bool[][] phoneEmpty = new bool[][] { 
                    new bool[] { true, true }, 
                    new bool[] { true, true }, 
                    new bool[] { true, true } };
                for (int idx = 0; idx < 3; idx++)
                {
                    PropertyInfo propType = quotationBasic.GetType().GetProperty("PhoneLineTypeCode" + (idx + 1).ToString());
                    PropertyInfo propOwner = quotationBasic.GetType().GetProperty("PhoneLineOwnerTypeCode" + (idx + 1).ToString());
                    if (propType != null && propOwner != null)
                    {
                        object propTypeValue = propType.GetValue(quotationBasic, null);
                        object propOwnerValue = propOwner.GetValue(quotationBasic, null);

                        phoneEmpty[idx][0] = CommonUtil.IsNullOrEmpty(propTypeValue);
                        phoneEmpty[idx][1] = CommonUtil.IsNullOrEmpty(propOwnerValue);

                        if (phoneEmpty[idx][0] == false || phoneEmpty[idx][1] == false)
                            isAllEmpty = false;
                    }
                }
                if (isAllEmpty)
                {
                    validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                "QUS030",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "PhoneLineInformation",
                                                "lblTelephoneLineInformationError",
                                                "PhoneLineTypeCode1,PhoneLineTypeCode2,PhoneLineTypeCode3,PhoneLineOwnerTypeCode1,PhoneLineOwnerTypeCode2,PhoneLineOwnerTypeCode3",
                                                "3");
                }
                else
                {
                    int order = 3;
                    for (int idx = 0; idx < 3; idx++)
                    {
                        if (phoneEmpty[idx][0] != phoneEmpty[idx][1])
                        {
                            if (phoneEmpty[idx][0] == true)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "PhoneLineTypeCode" + (idx + 1).ToString(),
                                                    "lblPhoneLineTypeCode" + (idx + 1).ToString(),
                                                    "PhoneLineTypeCode" + (idx + 1).ToString(),
                                                    (order).ToString());
                            }
                            else
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "PhoneLineOwnerTypeCode" + (idx + 1).ToString(),
                                                    "lblPhoneLineOwnerTypeCode" + (idx + 1).ToString(),
                                                    "PhoneLineOwnerTypeCode" + (idx + 1).ToString(),
                                                    (order + 1).ToString());
                            }
                        }

                        order += 2;
                    }
                }

                for (int idx = 0; idx < 3; idx++)
                {
                    PropertyInfo propApproveNo = quotationBasic.GetType().GetProperty("AdditionalApproveNo" + (idx + 1).ToString());
                    PropertyInfo propApproveFee = quotationBasic.GetType().GetProperty("AdditionalFee" + (idx + 1).ToString());
                    if (propApproveNo != null && propApproveFee != null)
                    {
                        object propApproveNoValue = propApproveNo.GetValue(quotationBasic, null);
                        object propApproveFeeValue = propApproveFee.GetValue(quotationBasic, null);

                        if (CommonUtil.IsNullOrEmpty(propApproveNoValue)
                            || CommonUtil.IsNullOrEmpty(propApproveFeeValue))
                        {
                            string ctrlName = null;
                            string ctrlID = null;
                            if (CommonUtil.IsNullOrEmpty(propApproveNoValue) == false)
                            {
                                ctrlID = "lblAdditionalContractFee" + (idx + 1).ToString();
                                ctrlName = "AdditionalFee" + (idx + 1).ToString();
                            }
                            else if (CommonUtil.IsNullOrEmpty(propApproveFeeValue) == false)
                            {
                                ctrlID = "lblAdditionalApproveNo" + (idx + 1).ToString();
                                ctrlName = "AdditionalApproveNo" + (idx + 1).ToString();
                            }

                            if (ctrlName != null)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                "QUS030",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                ctrlName,
                                                ctrlID,
                                                ctrlName,
                                                (14 + idx).ToString());
                            }
                        }
                    }
                }

                ValidatorUtil.BuildErrorMessage(res, validator);
                if (res.IsError)
                    return Json(res);
                /* ------------- */

                #endregion
                #region Validate Facility

                string dup_facility = null;

                #region Facility

                for (int i = 0; i < facilityLst.Count; i++)
                {
                    QUS030_tbt_QuotationInstrumentDetails inst = facilityLst[i];

                    List<string> eLst = new List<string>();
                    if (CommonUtil.IsNullOrEmpty(inst.InstrumentCode))
                        eLst.Add("headerFacilityCode");
                    if (CommonUtil.IsNullOrEmpty(inst.InstrumentQty))
                        eLst.Add("headerQuantity");

                    if (eLst.Count > 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                            "QUS030",
                                            MessageUtil.MODULE_QUOTATION,
                                            MessageUtil.MessageList.MSG2075,
                                            new string[] { (i + 1).ToString(), CommonUtil.TextList(eLst.ToArray()) });
                        return Json(res);
                    }

                    if (dup_facility == null)
                    {
                        for (int j = 0; j < facilityLst.Count; j++)
                        {
                            if (i == j)
                                continue;

                            QUS030_tbt_QuotationInstrumentDetails cinst = facilityLst[j];

                            /* --- Merge --- */
                            /* if (inst.InstrumentCode == cinst.InstrumentCode)
                            {
                                dup_facility = inst.InstrumentCode;
                                break;
                            } */
                            if (CommonUtil.IsNullOrEmpty(cinst.InstrumentCode) == false)
                            {
                                if (inst.InstrumentCode.ToUpper().Trim() == cinst.InstrumentCode.ToUpper().Trim())
                                {
                                    dup_facility = inst.InstrumentCode;
                                    break;
                                }
                            }
                            /* ------------- */

                        }
                    }
                }

                #endregion

                if (dup_facility != null)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                            MessageUtil.MessageList.MSG2090,
                                            new string[] { dup_facility });
                    return Json(res);
                }

                #endregion
                #region Validate Range

                QUS030_ValidateRangeData valRange = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic_ONLINE, QUS030_ValidateRangeData>(quotationBasic);

                Dictionary<string, RangeNumberValueAttribute> rangeAttr =
                    CommonUtil.CreateAttributeDictionary<RangeNumberValueAttribute>(valRange);
                foreach (KeyValuePair<string, RangeNumberValueAttribute> attr in rangeAttr)
                {
                    PropertyInfo prop = valRange.GetType().GetProperty(attr.Key);
                    if (prop != null)
                    {
                        object val = prop.GetValue(valRange, null);
                        if (CommonUtil.IsNullOrEmpty(val) == false)
                        {
                            if (attr.Value.IsValid(val) == false)
                            {
                                string format = "N2";
                                if (prop.PropertyType == typeof(int?))
                                    format = "N0";

                                string min = attr.Value.Min.ToString(format);
                                string max = attr.Value.Max.ToString(format);

                                res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    "QUS030",
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2087,
                                    new string[] { attr.Value.Parameter, min, max },
                                    new string[] { attr.Value.ControlName });
                                return Json(res);
                            }
                        }
                    }
                }

                #region Facility

                int line = 1;
                foreach (QUS030_tbt_QuotationInstrumentDetails inst in facilityLst)
                {
                    QUS030_ValidateRangeData_Facility valInstRange =
                        CommonUtil.CloneObject<QUS030_tbt_QuotationInstrumentDetails, QUS030_ValidateRangeData_Facility>(inst);

                    rangeAttr = CommonUtil.CreateAttributeDictionary<RangeNumberValueAttribute>(valInstRange);
                    foreach (KeyValuePair<string, RangeNumberValueAttribute> attr in rangeAttr)
                    {
                        PropertyInfo prop = valInstRange.GetType().GetProperty(attr.Key);
                        if (prop != null)
                        {
                            object val = prop.GetValue(valInstRange, null);
                            if (CommonUtil.IsNullOrEmpty(val) == false)
                            {
                                if (attr.Value.IsValid(val) == false)
                                {
                                    string format = "N2";
                                    if (prop.PropertyType == typeof(int?))
                                        format = "N0";

                                    string min = attr.Value.Min.ToString(format);
                                    string max = attr.Value.Max.ToString(format);

                                    res.AddErrorMessage(
                                        MessageUtil.MODULE_QUOTATION,
                                        "QUS030",
                                        MessageUtil.MODULE_QUOTATION,
                                        MessageUtil.MessageList.MSG2091,
                                        new string[] { inst.InstrumentCode, attr.Value.Parameter, min, max },
                                        new string[] { string.Format("FacilityDetail;{0};{1}", attr.Value.ControlName, line) });
                                    return Json(res);
                                }
                            }
                        }
                    }

                    line++;
                }

                #endregion

                #endregion

                if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode ==
                    TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                {
                    CommonUtil cmm = new CommonUtil();
                    string pShort = cmm.ConvertContractCode(doInitData.PriorSaleOnlineContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    if (quotationBasic.SaleOnlineContractCode != pShort)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2077,
                            new string[] { pShort });
                        return Json(res);
                    }
                }
                else
                {
                    try
                    {
                        CommonUtil cmm = new CommonUtil();

                        ISaleContractHandler chandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                        doSaleContractData doSaleContractData =
                            chandler.CheckLinkageSaleContract(
                                cmm.ConvertContractCode(quotationBasic.SaleOnlineContractCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode);
                    }
                    catch (ApplicationErrorException erx)
                    {
                        res.AddErrorMessage(erx);
                        if (res.MessageList != null)
                        {
                            foreach (MessageModel m in res.MessageList)
                            {
                                m.Controls = new string[] { "SaleOnlineContractCode" };
                            }
                        }
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                    catch (Exception errx)
                    {
                        res.AddErrorMessage(errx);
                        return Json(res);
                    }
                }

                ValidateEmployeeData(res, quotationBasic);
                if (res.IsError)
                    return Json(res);

                if (quotationBasic.InsuranceTypeCode == InsuranceType.C_INSURANCE_TYPE_NONE)
                {
                    bool isError = false;
                    if (CommonUtil.IsNullOrEmpty(quotationBasic.InsuranceCoverageAmount) == false)
                    {
                        if (quotationBasic.InsuranceCoverageAmount > 0)
                            isError = true;
                    }
                    if (CommonUtil.IsNullOrEmpty(quotationBasic.MonthlyInsuranceFee) == false)
                    {
                        if (quotationBasic.MonthlyInsuranceFee > 0)
                            isError = true;
                    }
                    if (isError)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2070);
                        return Json(res);
                    }
                }

                ValidateFacilityDetail(res, quotationBasic, facilityLst);
                if (res.IsError)
                    return Json(res);

                ValidateProduct(res, quotationBasic);
                if (res.IsError)
                    return Json(res);
                else if (res.ResultData == null)
                    CheckSaleOnlineQuotationData(res, quotationBasic, facilityLst);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Checl quotation detail data in case of sale online (step 2) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <param name="facilityLst"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckSaleOnlineQuotationData_P2(QUS030_tbt_QuotationBasic_ONLINE quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> facilityLst)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CheckSaleOnlineQuotationData(res, quotationBasic, facilityLst);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region Sale Online Methods

        /// <summary>
        /// Initial quotation detail data in case of sale online
        /// </summary>
        /// <param name="doRentalContractData"></param>
        /// <returns></returns>
        private doInitRentalQuotationData InitSaleOnlineQuotationData(dsRentalContractData doRentalContractData)
        {
            try
            {
                doInitRentalQuotationData initDo = new doInitRentalQuotationData();
                if (doRentalContractData.dtTbt_RentalSecurityBasic != null)
                {
                    if (doRentalContractData.dtTbt_RentalSecurityBasic.Count > 0)
                    {
                        #region Create Quotation Basic

                        initDo.doQuotationBasic = new tbt_QuotationBasic()
                        {
                            ProductCode = doRentalContractData.dtTbt_RentalSecurityBasic[0].ProductCode,
                            SecurityTypeCode = doRentalContractData.dtTbt_RentalSecurityBasic[0].SecurityTypeCode,
                            DispatchTypeCode = doRentalContractData.dtTbt_RentalSecurityBasic[0].DispatchTypeCode,
                            ContractDurationMonth = doRentalContractData.dtTbt_RentalSecurityBasic[0].ContractDurationMonth,
                            AutoRenewMonth = doRentalContractData.dtTbt_RentalSecurityBasic[0].AutoRenewMonth,
                            LastOccNo = doRentalContractData.dtTbt_RentalSecurityBasic[0].OCC,
                            CurrentSecurityTypeCode = doRentalContractData.dtTbt_RentalSecurityBasic[0].SecurityTypeCode,
                            PhoneLineTypeCode1 = doRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1,
                            PhoneLineTypeCode2 = doRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode2,
                            PhoneLineTypeCode3 = doRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode3,
                            PhoneLineOwnerTypeCode1 = doRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1,
                            PhoneLineOwnerTypeCode2 = doRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode2,
                            PhoneLineOwnerTypeCode3 = doRentalContractData.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode3,
                            FireMonitorFlag = doRentalContractData.dtTbt_RentalSecurityBasic[0].FireMonitorFlag,
                            CrimePreventFlag = doRentalContractData.dtTbt_RentalSecurityBasic[0].CrimePreventFlag,
                            EmergencyReportFlag = doRentalContractData.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag,
                            FacilityMonitorFlag = doRentalContractData.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag,
                            NumOfBuilding = doRentalContractData.dtTbt_RentalSecurityBasic[0].NumOfBuilding,
                            NumOfFloor = doRentalContractData.dtTbt_RentalSecurityBasic[0].NumOfFloor,
                            FacilityPassYear = doRentalContractData.dtTbt_RentalSecurityBasic[0].FacilityPassYear,
                            FacilityPassMonth = doRentalContractData.dtTbt_RentalSecurityBasic[0].FacilityPassMonth,
                            FacilityMemo = doRentalContractData.dtTbt_RentalSecurityBasic[0].FacilityMemo,
                            MaintenanceCycle = doRentalContractData.dtTbt_RentalSecurityBasic[0].MaintenanceCycle
                        };

                        #endregion
                    }
                }
                if (initDo.doQuotationBasic != null
                    && doRentalContractData.dtTbt_RelationType != null)
                {
                    if (doRentalContractData.dtTbt_RelationType.Count > 0)
                        initDo.doQuotationBasic.SaleOnlineContractCode = doRentalContractData.dtTbt_RelationType[0].RelatedContractCode;
                }

                /* --- Merge --- */
                /* ISaleContractHandler schandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                doSaleContractData doSaleContractData = null; */
                /* ------------- */

                if (doRentalContractData.dtTbt_RelationType != null)
                {
                    if (doRentalContractData.dtTbt_RelationType.Count > 0)
                    {

                        /* --- Merge --- */
                        /* doSaleContractData = schandler.GetSaleContractData(
                                                    doRentalContractData.dtTbt_RelationType[0].RelatedContractCode,
                                                    null); */
                        IQuotationHandler qheader = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                        initDo.doLinkageSaleContractData = qheader.GetLinkageSaleContractData(doRentalContractData.dtTbt_RelationType[0].RelatedContractCode);
                        /* ------------- */

                    }
                }

                /* --- Merge --- */
                /* if (doSaleContractData != null)
                {
                    IQuotationHandler qheader = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    initDo.doLinkageSaleContractData = qheader.InitLinkageSaleContractData(doSaleContractData);
                } */
                /* ------------- */

                if (doRentalContractData.dtTbt_RentalOperationType != null)
                {
                    if (doRentalContractData.dtTbt_RentalOperationType.Count > 0)
                    {
                        initDo.OperationTypeList = new List<doQuotationOperationType>();
                        foreach (tbt_RentalOperationType rODo in doRentalContractData.dtTbt_RentalOperationType)
                        {
                            initDo.OperationTypeList.Add(new doQuotationOperationType()
                            {
                                QuotationTargetCode = rODo.ContractCode,
                                OperationTypeCode = rODo.OperationTypeCode
                            });
                        }
                    }
                }

                if (doRentalContractData.dtTbt_RentalInstrumentDetails != null)
                {
                    if (doRentalContractData.dtTbt_RentalInstrumentDetails.Count > 0)
                    {
                        #region Instrument Master

                        List<tbm_Instrument> iLst = new List<tbm_Instrument>();
                        foreach (tbt_RentalInstrumentDetails riDo in doRentalContractData.dtTbt_RentalInstrumentDetails)
                        {
                            if (riDo.InstrumentTypeCode == SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_MONITOR)
                            {
                                iLst.Add(new tbm_Instrument()
                                {
                                    InstrumentCode = riDo.InstrumentCode
                                });
                            }
                        }
                        IInstrumentMasterHandler inshandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                        List<tbm_Instrument> insLst = inshandler.GetIntrumentList(iLst);

                        #endregion
                        #region Create Facility

                        foreach (tbt_RentalInstrumentDetails riDo in doRentalContractData.dtTbt_RentalInstrumentDetails)
                        {
                            if (riDo.InstrumentTypeCode != SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_MONITOR)
                                continue;

                            tbm_Instrument ins = new tbm_Instrument();
                            if (insLst != null)
                            {
                                foreach (tbm_Instrument i in insLst)
                                {
                                    /* --- Merge --- */
                                    /* if (i.InstrumentCode == riDo.InstrumentCode)
                                    {
                                        ins = i;
                                        break;
                                    } */
                                    if (i.InstrumentCode.ToUpper().Trim() == riDo.InstrumentCode.ToUpper().Trim())
                                    {
                                        ins = i;
                                        break;
                                    }
                                    /* ------------- */

                                }
                            }

                            if (initDo.FacilityDetailList == null)
                                initDo.FacilityDetailList = new List<doFacilityDetail>();

                            initDo.FacilityDetailList.Add(new doFacilityDetail()
                            {
                                FacilityCode = riDo.InstrumentCode,
                                FacilityName = ins.InstrumentName,
                                FacilityQty = riDo.InstrumentQty
                            });
                        }

                        #endregion
                    }
                }

                return initDo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Initial quotation detail data in case of sale online from import file
        /// </summary>
        /// <param name="res"></param>
        /// <param name="initData"></param>
        /// <param name="importData"></param>
        private void InitImportSaleOnlineQuotation(ObjectResultData res, doInitQuotationData initData, dsImportData importData)
        {
            try
            {
                #region Create Quotation Basic

                if (importData.dtTbt_QuotationBasic != null)
                {
                    if (importData.dtTbt_QuotationBasic.Count > 0)
                    {
                        tbt_QuotationBasic nDo = importData.dtTbt_QuotationBasic[0];
                        if (initData.doQuotationBasic != null)
                        {
                            nDo.CurrentSecurityTypeCode = initData.doQuotationBasic.SecurityTypeCode;
                            nDo.LastOccNo = initData.doQuotationBasic.LastOccNo;

                            if (initData.doQuotationBasic.ProductCode == nDo.ProductCode)
                            {
                                nDo.ContractDurationMonth = initData.doQuotationBasic.ContractDurationMonth;
                                nDo.AutoRenewMonth = initData.doQuotationBasic.AutoRenewMonth;
                            }
                        }

                        if (initData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                            nDo.DepositFee = null;

                        initData.doQuotationBasic = nDo;
                    }
                }

                #endregion

                #region Prepare Operation Type

                initData.OperationTypeList = new List<doQuotationOperationType>();
                if (importData.dtTbt_QuotationOperationType != null)
                {
                    if (importData.dtTbt_QuotationOperationType.Count > 0)
                        initData.OperationTypeList = CommonUtil.ClonsObjectList<tbt_QuotationOperationType, doQuotationOperationType>(importData.dtTbt_QuotationOperationType);
                }

                #endregion
                #region Prepare Sale Online Information

                CommonUtil cmm = new CommonUtil();
                initData.doQuotationBasic.SaleOnlineContractCode =
                    cmm.ConvertContractCode(initData.doQuotationBasic.SaleOnlineContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);


                if ((initData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE
                    && CommonUtil.IsNullOrEmpty(initData.doQuotationBasic.SaleOnlineContractCode) == false)
                    || (initData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE
                    && initData.doQuotationBasic.SaleOnlineContractCode != initData.PriorSaleOnlineContractCode))
                {
                    QUS030_GetLinkageSaleContractCondition cond = new QUS030_GetLinkageSaleContractCondition()
                    {
                        SaleOnlineContractCode = initData.doQuotationBasic.SaleOnlineContractCode
                    };
                    initData.doLinkageSaleContractData = GetLinkageSaleContractData(cond);
                }

                #endregion
                #region Prepare Facility Detail

                bool isNullImportFacility = true;
                if (importData.dtTbt_QuotationFacilityDetails != null)
                {
                    if (importData.dtTbt_QuotationFacilityDetails.Count > 0)
                        isNullImportFacility = false;
                }
                if (isNullImportFacility)
                    initData.FacilityDetailList = new List<doFacilityDetail>();
                else
                {
                    #region Get Master Facility

                    /* --- Merge --- */
                    /* List<doMonitoringInstrument> fLst = new List<doMonitoringInstrument>();
                    foreach (tbt_QuotationFacilityDetails fd in importData.dtTbt_QuotationFacilityDetails)
                    {
                        fLst.Add(new doMonitoringInstrument()
                        {
                            InstrumentCode = fd.FacilityCode
                        });
                    }

                    IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<doMonitoringInstrument> instLst = ihandler.GetMonitoringInstrumentList(fLst); */
                    List<tbm_Instrument> fLst = new List<tbm_Instrument>();
                    foreach (tbt_QuotationFacilityDetails fd in importData.dtTbt_QuotationFacilityDetails)
                    {
                        if (CommonUtil.IsNullOrEmpty(fd.FacilityCode) == false)
                        {
                            fLst.Add(new tbm_Instrument()
                            {
                                InstrumentCode = fd.FacilityCode
                            });
                        }
                    }

                    IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<tbm_Instrument> instLst = ihandler.GetIntrumentList(fLst);
                    /* ------------- */

                    #endregion

                    List<doFacilityDetail> facilityDetailLst = new List<doFacilityDetail>();
                    foreach (tbt_QuotationFacilityDetails facility in importData.dtTbt_QuotationFacilityDetails)
                    {
                        /* --- Merge --- */
                        /* doMonitoringInstrument masterInst = null;
                        foreach (doMonitoringInstrument mInst in instLst)
                        {
                            if (mInst.InstrumentCode == facility.FacilityCode)
                            {
                                masterInst = mInst;
                                break;
                            }
                        } */
                        tbm_Instrument masterInst = null;
                        if (CommonUtil.IsNullOrEmpty(facility.FacilityCode) == false)
                        {
                            foreach (tbm_Instrument mInst in instLst)
                            {
                                if (mInst.InstrumentCode.ToUpper().Trim() == facility.FacilityCode.ToUpper().Trim())
                                {
                                    masterInst = mInst;
                                    break;
                                }
                            }
                        }
                        /* ------------- */


                        #region Add Instrument

                        doFacilityDetail fd = new doFacilityDetail()
                        {
                            FacilityCode = facility.FacilityCode,
                            FacilityQty = facility.FacilityQty
                        };
                        if (masterInst != null)
                        {
                            fd.FacilityName = masterInst.InstrumentName;
                        }
                        facilityDetailLst.Add(fd);

                        #endregion
                    }

                    initData.FacilityDetailList = facilityDetailLst;
                }

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Retrieve linkage sale contract data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        private doLinkageSaleContractData GetLinkageSaleContractData(QUS030_GetLinkageSaleContractCondition cond)
        {
            try
            {
                doLinkageSaleContractData doLinkage = null;

                /* --- Merge --- */
                /* ISaleContractHandler chandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                doSaleContractData doSaleContractData = chandler.CheckLinkageSaleContract(cond.SaleOnlineContractCode, cond.QuotationTargetCode); //TODO: Bug report QU-124 Add param quotation target code

                IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                doLinkage = qhandler.InitLinkageSaleContractData(doSaleContractData); */
                ISaleContractHandler chandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                doSaleContractData scData = chandler.GetSaleContractData(cond.SaleOnlineContractCode, null);
                if (scData != null)
                {
                    IQuotationHandler qhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    doLinkage = qhandler.InitLinkageSaleContractData(scData);
                }
                /* ------------- */

                return doLinkage;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check quotation detail data in case of sale online is correct?
        /// </summary>
        /// <param name="res"></param>
        /// <param name="quotationBasic"></param>
        /// <param name="facilityLst"></param>
        private void CheckSaleOnlineQuotationData(ObjectResultData res, QUS030_tbt_QuotationBasic_ONLINE quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> facilityLst)
        {
            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;

                #endregion

                doRegisterQuotationData doRegister = new doRegisterQuotationData();
                doRegister.doQuotationHeaderData = doInitData.doQuotationHeaderData;

                doRegister.doTbt_QuotationBasic = new tbt_QuotationBasic();
                doRegister.doTbt_QuotationBasic.ProductCode = quotationBasic.ProductCode;
                doRegister.doTbt_QuotationBasic.SecurityTypeCode = quotationBasic.SecurityTypeCode;
                doRegister.doTbt_QuotationBasic.DispatchTypeCode = quotationBasic.DispatchTypeCode;

                doRegister.doTbt_QuotationBasic.Alphabet = null;
                doRegister.doTbt_QuotationBasic.OriginateProgramId = null;
                doRegister.doTbt_QuotationBasic.OriginateRefNo = null;
                doRegister.doTbt_QuotationBasic.ContractTransferStatus =
                    SECOM_AJIS.Common.Util.ConstantValue.ContractTransferStatus.C_CONTRACT_TRANS_STATUS_QTN_REG;
                doRegister.doTbt_QuotationBasic.LockStatus = SECOM_AJIS.Common.Util.ConstantValue.LockStatus.C_LOCK_STATUS_UNLOCK;

                doRegister.doTbt_QuotationBasic.LastOccNo = null;
                doRegister.doTbt_QuotationBasic.CurrentSecurityTypeCode = null;
                doRegister.doTbt_QuotationBasic.SpecialInstallationFlag = null;
                doRegister.doTbt_QuotationBasic.NewBldMgmtFlag = null;

                CommonUtil cmm = new CommonUtil();
                doRegister.doTbt_QuotationBasic.SaleOnlineContractCode = cmm.ConvertContractCode(quotationBasic.SaleOnlineContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                doRegister.doTbt_QuotationBasic.QuotationNo = quotationBasic.QuotationNo;
                doRegister.doTbt_QuotationBasic.PhoneLineTypeCode1 = quotationBasic.PhoneLineTypeCode1;
                doRegister.doTbt_QuotationBasic.PhoneLineTypeCode2 = quotationBasic.PhoneLineTypeCode2;
                doRegister.doTbt_QuotationBasic.PhoneLineTypeCode3 = quotationBasic.PhoneLineTypeCode3;
                doRegister.doTbt_QuotationBasic.PhoneLineOwnerTypeCode1 = quotationBasic.PhoneLineOwnerTypeCode1;
                doRegister.doTbt_QuotationBasic.PhoneLineOwnerTypeCode2 = quotationBasic.PhoneLineOwnerTypeCode2;
                doRegister.doTbt_QuotationBasic.PhoneLineOwnerTypeCode3 = quotationBasic.PhoneLineOwnerTypeCode3;
                doRegister.doTbt_QuotationBasic.NumOfBuilding = quotationBasic.NumOfBuilding;
                doRegister.doTbt_QuotationBasic.NumOfFloor = quotationBasic.NumOfFloor;
                doRegister.doTbt_QuotationBasic.SalesmanEmpNo1 = quotationBasic.SalesmanEmpNo1;
                doRegister.doTbt_QuotationBasic.SalesmanEmpNo2 = quotationBasic.SalesmanEmpNo2;
                doRegister.doTbt_QuotationBasic.SalesSupporterEmpNo = quotationBasic.SalesSupporterEmpNo;
                doRegister.doTbt_QuotationBasic.InsuranceTypeCode = quotationBasic.InsuranceTypeCode;
                doRegister.doTbt_QuotationBasic.AdditionalApproveNo1 = quotationBasic.AdditionalApproveNo1;
                doRegister.doTbt_QuotationBasic.AdditionalApproveNo2 = quotationBasic.AdditionalApproveNo2;
                doRegister.doTbt_QuotationBasic.AdditionalApproveNo3 = quotationBasic.AdditionalApproveNo3;
                doRegister.doTbt_QuotationBasic.ApproveNo1 = quotationBasic.ApproveNo1;
                doRegister.doTbt_QuotationBasic.ApproveNo2 = quotationBasic.ApproveNo2;
                doRegister.doTbt_QuotationBasic.ApproveNo3 = quotationBasic.ApproveNo3;
                doRegister.doTbt_QuotationBasic.ApproveNo4 = quotationBasic.ApproveNo4;
                doRegister.doTbt_QuotationBasic.ApproveNo5 = quotationBasic.ApproveNo5;

                doRegister.doTbt_QuotationBasic.FacilityMemo = quotationBasic.FacilityMemo;
                doRegister.doTbt_QuotationBasic.MaintenanceCycle = quotationBasic.MaintenanceCycle;

                doRegister.doTbt_QuotationBasic.DepositFee = quotationBasic.DepositFee;
                doRegister.doTbt_QuotationBasic.DepositFeeCurrencyType = quotationBasic.DepositFeeCurrencyType;
                if (doRegister.doTbt_QuotationBasic.DepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.DepositFeeUsd = quotationBasic.DepositFee;
                    doRegister.doTbt_QuotationBasic.DepositFee = null;
                }
                else
                {
                    doRegister.doTbt_QuotationBasic.DepositFeeUsd = null;
                }


                //doRegister.doTbt_QuotationBasic.ContractFeeCurrencyType = quotationBasic.ContractFeeCurrencyType;
                //if (quotationBasic.ContractFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                //    doRegister.doTbt_QuotationBasic.ContractFeeUsd = quotationBasic.ContractFee;
                //else
                //    doRegister.doTbt_QuotationBasic.ContractFee = quotationBasic.ContractFee;

                doRegister.doTbt_QuotationBasic.ContractFee = quotationBasic.ContractFee;
                doRegister.doTbt_QuotationBasic.ContractFeeCurrencyType = quotationBasic.ContractFeeCurrencyType;
                if (doRegister.doTbt_QuotationBasic.ContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.ContractFeeUsd = quotationBasic.ContractFee;
                    doRegister.doTbt_QuotationBasic.ContractFee = null;
                }
                else
                {
                    doRegister.doTbt_QuotationBasic.ContractFeeUsd = null;
                }

                doRegister.doTbt_QuotationBasic.AdditionalFee1 = quotationBasic.AdditionalFee1;
                doRegister.doTbt_QuotationBasic.AdditionalFee2 = quotationBasic.AdditionalFee2;
                doRegister.doTbt_QuotationBasic.AdditionalFee3 = quotationBasic.AdditionalFee3;
                doRegister.doTbt_QuotationBasic.AdditionalFee1CurrencyType = quotationBasic.AdditionalFee1CurrencyType;
                doRegister.doTbt_QuotationBasic.AdditionalFee2CurrencyType = quotationBasic.AdditionalFee2CurrencyType;
                doRegister.doTbt_QuotationBasic.AdditionalFee3CurrencyType = quotationBasic.AdditionalFee3CurrencyType;
                if (doRegister.doTbt_QuotationBasic.AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee1Usd = quotationBasic.AdditionalFee1;
                    doRegister.doTbt_QuotationBasic.AdditionalFee1 = null;
                }
                else
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee1Usd = null;
                }
                if (doRegister.doTbt_QuotationBasic.AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee2Usd = quotationBasic.AdditionalFee2;
                    doRegister.doTbt_QuotationBasic.AdditionalFee2 = null;
                }
                else
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee2Usd = null;
                }
                if (doRegister.doTbt_QuotationBasic.AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee3Usd = quotationBasic.AdditionalFee3;
                    doRegister.doTbt_QuotationBasic.AdditionalFee3 = null;
                }
                else
                {
                    doRegister.doTbt_QuotationBasic.AdditionalFee3Usd = null;
                }

                doRegister.doTbt_QuotationBasic.InsuranceCoverageAmount = quotationBasic.InsuranceCoverageAmount;
                doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountCurrencyType = quotationBasic.InsuranceCoverageAmountCurrencyType;
                if (doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountUsd = quotationBasic.InsuranceCoverageAmount;
                    doRegister.doTbt_QuotationBasic.InsuranceCoverageAmount = null;
                }
                else
                {
                    doRegister.doTbt_QuotationBasic.InsuranceCoverageAmountUsd = null;
                }
                doRegister.doTbt_QuotationBasic.MonthlyInsuranceFee = quotationBasic.MonthlyInsuranceFee;
                doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeCurrencyType = quotationBasic.MonthlyInsuranceFeeCurrencyType;
                if (doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeUsd = quotationBasic.MonthlyInsuranceFee;
                    doRegister.doTbt_QuotationBasic.MonthlyInsuranceFee = null;
                }
                else
                {
                    doRegister.doTbt_QuotationBasic.MonthlyInsuranceFeeUsd = null;
                }
                doRegister.doTbt_QuotationBasic.MaintenanceFee1 = quotationBasic.MaintenanceFee1;
                doRegister.doTbt_QuotationBasic.MaintenanceFee1CurrencyType = quotationBasic.MaintenanceFee1CurrencyType;
                if (doRegister.doTbt_QuotationBasic.MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1Usd = quotationBasic.MaintenanceFee1;
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1 = null;
                }
                else
                {
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1Usd = null;
                }

                doRegister.doTbt_QuotationBasic.NewBldMgmtCost = quotationBasic.NewBldMgmtCost;
                doRegister.doTbt_QuotationBasic.NewBldMgmtCostCurrencyType = quotationBasic.NewBldMgmtCostCurrencyType;
                if (doRegister.doTbt_QuotationBasic.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doRegister.doTbt_QuotationBasic.NewBldMgmtCostUsd = quotationBasic.NewBldMgmtCost;
                    doRegister.doTbt_QuotationBasic.NewBldMgmtCost = null;
                }
                else
                {
                    doRegister.doTbt_QuotationBasic.NewBldMgmtCostUsd = null;
                }

                doRegister.doTbt_QuotationBasic.FireMonitorFlag = false;
                doRegister.doTbt_QuotationBasic.CrimePreventFlag = false;
                doRegister.doTbt_QuotationBasic.EmergencyReportFlag = false;
                doRegister.doTbt_QuotationBasic.FacilityMonitorFlag = false;

                doRegister.doTbt_QuotationBasic.BeatGuardFlag = false;
                doRegister.doTbt_QuotationBasic.SentryGuardFlag = false;
                doRegister.doTbt_QuotationBasic.MaintenanceFlag = false;

                if (quotationBasic.ServiceType != null)
                {
                    foreach (string st in quotationBasic.ServiceType)
                    {
                        if (st == CommonValue.QUOTATION_SERVICE_TYPE_FIRE_MONITORING)
                            doRegister.doTbt_QuotationBasic.FireMonitorFlag = true;
                        if (st == CommonValue.QUOTATION_SERVICE_TYPE_CRIME_PREVENTION)
                            doRegister.doTbt_QuotationBasic.CrimePreventFlag = true;
                        if (st == CommonValue.QUOTATION_SERVICE_TYPE_EMERGENCY_REPORT)
                            doRegister.doTbt_QuotationBasic.EmergencyReportFlag = true;
                        if (st == CommonValue.QUOTATION_SERVICE_TYPE_FACILITY_MONITORING)
                            doRegister.doTbt_QuotationBasic.FacilityMonitorFlag = true;
                    }
                }

                bool isQuotation = false;
                if (doInitData.doQuotationHeaderData != null)
                {
                    if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                    {
                        doRegister.doTbt_QuotationBasic.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;

                        if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode
                            == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                            isQuotation = true;
                    }
                }
                if (doInitData.doQuotationBasic != null)
                {
                    if (isQuotation == false)
                    {
                        doRegister.doTbt_QuotationBasic.LastOccNo = doInitData.doQuotationBasic.LastOccNo;
                        doRegister.doTbt_QuotationBasic.CurrentSecurityTypeCode = doInitData.doQuotationBasic.CurrentSecurityTypeCode;
                    }

                    doRegister.doTbt_QuotationBasic.FacilityPassYear = doInitData.doQuotationBasic.FacilityPassYear;
                    doRegister.doTbt_QuotationBasic.FacilityPassMonth = doInitData.doQuotationBasic.FacilityPassMonth;
                    doRegister.doTbt_QuotationBasic.CurrentSecurityTypeCode = doInitData.doQuotationBasic.CurrentSecurityTypeCode;
                    doRegister.doTbt_QuotationBasic.MaintenanceFee2 = doInitData.doQuotationBasic.MaintenanceFee2;
                }

                if (doInitData.PriorProductCode == quotationBasic.ProductCode)
                {
                    if (doInitData.doQuotationBasic != null)
                    {
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = doInitData.doQuotationBasic.ContractDurationMonth;
                        doRegister.doTbt_QuotationBasic.AutoRenewMonth = doInitData.doQuotationBasic.AutoRenewMonth;
                    }
                }
                else
                {
                    IProductMasterHandler phandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    List<tbm_Product> pLst = phandler.GetTbm_Product(quotationBasic.ProductCode, null);
                    if (pLst.Count >= 0)
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = pLst[0].DepreciationPeriodContract;

                    doRegister.doTbt_QuotationBasic.AutoRenewMonth = SECOM_AJIS.Common.Util.ConstantValue.Quotation.C_DEFAULT_AUTO_RENEW_MONTH;
                }

                if (quotationBasic.OperationType != null)
                {
                    doRegister.OperationList = new List<tbt_QuotationOperationType>();
                    foreach (string op in quotationBasic.OperationType)
                    {
                        tbt_QuotationOperationType opt = new tbt_QuotationOperationType()
                        {
                            QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode,
                            OperationTypeCode = op
                        };
                        doRegister.OperationList.Add(opt);
                    }
                }

                if (facilityLst != null)
                {
                    doRegister.FacilityList = new List<tbt_QuotationFacilityDetails>();
                    foreach (QUS030_tbt_QuotationInstrumentDetails inst in facilityLst)
                    {
                        if (inst.InstrumentQty > 0)
                        {
                            tbt_QuotationFacilityDetails facility = new tbt_QuotationFacilityDetails()
                            {
                                QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode,
                                FacilityCode = inst.InstrumentCode,
                                FacilityQty = inst.InstrumentQty
                            };
                            doRegister.FacilityList.Add(facility);
                        }
                    }
                }

                #region Update Session

                param.RegisterData = doRegister;
                QUS030_ScreenData = param;

                #endregion

                res.ResultData = doRegister;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Beat Guard Actions

        /// <summary>
        /// Check quotation detail data in case of beat guard (step 1) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <param name="doBeatGuardDetail"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckBeatGuardQuotationData_P1(QUS030_tbt_QuotationBasic_BE quotationBasic, QUS030_doBeatGuardDetail doBeatGuardDetail)
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
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                #endregion
                #region Validate Object

                /* --- Merge --- */
                /* if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                } */
                ValidatorUtil validator = new ValidatorUtil(this);

                for (int idx = 0; idx < 3; idx++)
                {
                    PropertyInfo propApproveNo = quotationBasic.GetType().GetProperty("AdditionalApproveNo" + (idx + 1).ToString());
                    PropertyInfo propApproveFee = quotationBasic.GetType().GetProperty("AdditionalFee" + (idx + 1).ToString());
                    if (propApproveNo != null && propApproveFee != null)
                    {
                        object propApproveNoValue = propApproveNo.GetValue(quotationBasic, null);
                        object propApproveFeeValue = propApproveFee.GetValue(quotationBasic, null);

                        if (CommonUtil.IsNullOrEmpty(propApproveNoValue)
                            || CommonUtil.IsNullOrEmpty(propApproveFeeValue))
                        {
                            string ctrlName = null;
                            string ctrlID = null;
                            if (CommonUtil.IsNullOrEmpty(propApproveNoValue) == false)
                            {
                                ctrlID = "lblAdditionalContractFee" + (idx + 1).ToString();
                                ctrlName = "AdditionalFee" + (idx + 1).ToString();
                            }
                            else if (CommonUtil.IsNullOrEmpty(propApproveFeeValue) == false)
                            {
                                ctrlID = "lblAdditionalApproveNo" + (idx + 1).ToString();
                                ctrlName = "AdditionalApproveNo" + (idx + 1).ToString();
                            }

                            if (ctrlName != null)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                "QUS030",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                ctrlName,
                                                ctrlID,
                                                ctrlName,
                                                (3 + idx).ToString());
                            }
                        }
                    }
                }

                ValidatorUtil.BuildErrorMessage(res, validator);
                if (res.IsError)
                    return Json(res);
                /* ------------- */

                #endregion
                #region Validate Range

                QUS030_ValidateRangeData valRange = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic_BE, QUS030_ValidateRangeData>(quotationBasic);
                valRange.NumOfDayTimeWd = doBeatGuardDetail.NumOfDayTimeWd;
                valRange.NumOfNightTimeWd = doBeatGuardDetail.NumOfNightTimeWd;
                valRange.NumOfDayTimeSat = doBeatGuardDetail.NumOfDayTimeSat;
                valRange.NumOfNightTimeSat = doBeatGuardDetail.NumOfNightTimeSat;
                valRange.NumOfDayTimeSun = doBeatGuardDetail.NumOfDayTimeSun;
                valRange.NumOfNightTimeSun = doBeatGuardDetail.NumOfNightTimeSun;
                valRange.NumOfBeatStep = doBeatGuardDetail.NumOfBeatStep;
                valRange.FreqOfGateUsage = doBeatGuardDetail.FreqOfGateUsage;
                valRange.NumOfClockKey = doBeatGuardDetail.NumOfClockKey;
                valRange.NotifyTime = doBeatGuardDetail.NotifyTime;

                Dictionary<string, RangeNumberValueAttribute> rangeAttr =
                    CommonUtil.CreateAttributeDictionary<RangeNumberValueAttribute>(valRange);
                foreach (KeyValuePair<string, RangeNumberValueAttribute> attr in rangeAttr)
                {
                    PropertyInfo prop = valRange.GetType().GetProperty(attr.Key);
                    if (prop != null)
                    {
                        object val = prop.GetValue(valRange, null);
                        if (CommonUtil.IsNullOrEmpty(val) == false)
                        {
                            if (attr.Value.IsValid(val) == false)
                            {
                                string format = "N2";
                                if (prop.PropertyType == typeof(int?))
                                    format = "N0";

                                string min = attr.Value.Min.ToString(format);
                                string max = attr.Value.Max.ToString(format);

                                res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    "QUS030",
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2087,
                                    new string[] { attr.Value.Parameter, min, max },
                                    new string[] { attr.Value.ControlName });
                                return Json(res);
                            }
                        }
                    }
                }
                Dictionary<string, TimeValueAttribute> timeAttr =
                    CommonUtil.CreateAttributeDictionary<TimeValueAttribute>(valRange);
                foreach (KeyValuePair<string, TimeValueAttribute> attr in timeAttr)
                {
                    PropertyInfo prop = valRange.GetType().GetProperty(attr.Key);
                    if (prop != null)
                    {
                        object val = prop.GetValue(valRange, null);
                        if (CommonUtil.IsNullOrEmpty(val) == false)
                        {
                            if (attr.Value.IsValid(val) == false)
                            {
                                res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    "QUS030",
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2087,
                                    new string[] { attr.Value.Parameter, "00:00", "23:59" },
                                    new string[] { attr.Value.ControlName });
                                return Json(res);
                            }
                        }
                    }
                }

                #endregion

                ValidateEmployeeData(res, quotationBasic);
                if (res.IsError)
                    return Json(res);

                ValidateProduct(res, quotationBasic);
                if (res.IsError)
                    return Json(res);
                else
                    CheckBeatGuardQuotationData(res, quotationBasic, doBeatGuardDetail);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check quotation detail data in case of beat guard (step 2) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <param name="doBeatGuardDetail"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckBeatGuardQuotationData_P2(QUS030_tbt_QuotationBasic_BE quotationBasic, QUS030_doBeatGuardDetail doBeatGuardDetail)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CheckBeatGuardQuotationData(res, quotationBasic, doBeatGuardDetail);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region Beat Guard Methods

        /// <summary>
        /// Initial quotation detail data in case of beat guard
        /// </summary>
        /// <param name="doRentalContractData"></param>
        /// <returns></returns>
        private doInitRentalQuotationData InitBEQuotationData(dsRentalContractData doRentalContractData)
        {
            try
            {
                doInitRentalQuotationData initDo = new doInitRentalQuotationData();
                if (doRentalContractData.dtTbt_RentalSecurityBasic != null)
                {
                    if (doRentalContractData.dtTbt_RentalSecurityBasic.Count > 0)
                    {
                        #region Create Quotation Basic

                        initDo.doQuotationBasic = new tbt_QuotationBasic()
                        {
                            ProductCode = doRentalContractData.dtTbt_RentalSecurityBasic[0].ProductCode,
                            ContractDurationMonth = doRentalContractData.dtTbt_RentalSecurityBasic[0].ContractDurationMonth,
                            AutoRenewMonth = doRentalContractData.dtTbt_RentalSecurityBasic[0].AutoRenewMonth,
                            LastOccNo = doRentalContractData.dtTbt_RentalSecurityBasic[0].OCC
                        };

                        #endregion
                    }
                }
                if (doRentalContractData.dtTbt_RentalBEDetails != null)
                {
                    if (doRentalContractData.dtTbt_RentalBEDetails.Count > 0)
                        initDo.doBeatGuardDetail = CommonUtil.CloneObject<tbt_RentalBEDetails, doBeatGuardDetail>(doRentalContractData.dtTbt_RentalBEDetails[0]);
                }

                return initDo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Initial quotation detail data in case of beat guard from import file
        /// </summary>
        /// <param name="res"></param>
        /// <param name="initData"></param>
        /// <param name="importData"></param>
        private void InitImportBEQuotation(ObjectResultData res, doInitQuotationData initData, dsImportData importData)
        {
            try
            {
                #region Create Quotation Basic

                if (importData.dtTbt_QuotationBasic != null)
                {
                    if (importData.dtTbt_QuotationBasic.Count > 0)
                    {
                        tbt_QuotationBasic nDo = importData.dtTbt_QuotationBasic[0];
                        if (initData.doQuotationBasic != null)
                        {
                            nDo.LastOccNo = initData.doQuotationBasic.LastOccNo;

                            if (initData.doQuotationBasic.ProductCode == nDo.ProductCode)
                            {
                                nDo.ContractDurationMonth = initData.doQuotationBasic.ContractDurationMonth;
                                nDo.AutoRenewMonth = initData.doQuotationBasic.AutoRenewMonth;
                            }
                        }

                        if (initData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                            nDo.DepositFee = null;

                        initData.doQuotationBasic = nDo;
                    }
                }

                #endregion
                #region Prepare Beat Guard Detail

                initData.doBeatGuardDetail = null;
                if (importData.dtTbt_QuotationBeatGuardDetails != null)
                {

                    /* --- Merge --- */
                    /* if (importData.dtTbt_QuotationBeatGuardDetails.Count > 0)
                        initData.doBeatGuardDetail = 
                            CommonUtil.CloneObject<tbt_QuotationBeatGuardDetails, doBeatGuardDetail>(importData.dtTbt_QuotationBeatGuardDetails[0]); */
                    if (importData.dtTbt_QuotationBeatGuardDetails.Count > 0)
                    {
                        initData.doBeatGuardDetail =
                            CommonUtil.CloneObject<tbt_QuotationBeatGuardDetails, doBeatGuardDetail>(importData.dtTbt_QuotationBeatGuardDetails[0]);
                    }
                    /* ------------- */

                }

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check quotation detail data in case of beat guard is correct?
        /// </summary>
        /// <param name="res"></param>
        /// <param name="quotationBasic"></param>
        /// <param name="doBeatGuardDetail"></param>
        private void CheckBeatGuardQuotationData(ObjectResultData res, QUS030_tbt_QuotationBasic_BE quotationBasic, QUS030_doBeatGuardDetail doBeatGuardDetail)
        {
            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;

                #endregion

                doRegisterQuotationData doRegister = new doRegisterQuotationData();
                doRegister.doQuotationHeaderData = doInitData.doQuotationHeaderData;

                doRegister.doTbt_QuotationBasic = new tbt_QuotationBasic();
                doRegister.doTbt_QuotationBasic.ProductCode = quotationBasic.ProductCode;
                doRegister.doTbt_QuotationBasic.ContractTransferStatus =
                        SECOM_AJIS.Common.Util.ConstantValue.ContractTransferStatus.C_CONTRACT_TRANS_STATUS_QTN_REG;
                doRegister.doTbt_QuotationBasic.LockStatus = SECOM_AJIS.Common.Util.ConstantValue.LockStatus.C_LOCK_STATUS_UNLOCK;

                doRegister.doTbt_QuotationBasic.FireMonitorFlag = false;
                doRegister.doTbt_QuotationBasic.CrimePreventFlag = false;
                doRegister.doTbt_QuotationBasic.EmergencyReportFlag = false;
                doRegister.doTbt_QuotationBasic.FacilityMonitorFlag = false;
                doRegister.doTbt_QuotationBasic.BeatGuardFlag = true;
                doRegister.doTbt_QuotationBasic.SentryGuardFlag = false;
                doRegister.doTbt_QuotationBasic.MaintenanceFlag = false;
                doRegister.doTbt_QuotationBasic.QuotationNo = quotationBasic.QuotationNo;
                doRegister.doTbt_QuotationBasic.SalesmanEmpNo1 = quotationBasic.SalesmanEmpNo1;
                doRegister.doTbt_QuotationBasic.SalesmanEmpNo2 = quotationBasic.SalesmanEmpNo2;
                doRegister.doTbt_QuotationBasic.SalesSupporterEmpNo = quotationBasic.SalesSupporterEmpNo;
                doRegister.doTbt_QuotationBasic.AdditionalApproveNo1 = quotationBasic.AdditionalApproveNo1;
                doRegister.doTbt_QuotationBasic.AdditionalApproveNo2 = quotationBasic.AdditionalApproveNo2;
                doRegister.doTbt_QuotationBasic.AdditionalApproveNo3 = quotationBasic.AdditionalApproveNo3;
                doRegister.doTbt_QuotationBasic.ApproveNo1 = quotationBasic.ApproveNo1;
                doRegister.doTbt_QuotationBasic.ApproveNo2 = quotationBasic.ApproveNo2;
                doRegister.doTbt_QuotationBasic.ApproveNo3 = quotationBasic.ApproveNo3;
                doRegister.doTbt_QuotationBasic.ApproveNo4 = quotationBasic.ApproveNo4;
                doRegister.doTbt_QuotationBasic.ApproveNo5 = quotationBasic.ApproveNo5;

                doRegister.doTbt_QuotationBasic.ContractFeeCurrencyType = quotationBasic.ContractFeeCurrencyType;
                if (quotationBasic.ContractFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.ContractFeeUsd = quotationBasic.ContractFee;
                else
                    doRegister.doTbt_QuotationBasic.ContractFee = quotationBasic.ContractFee;

                doRegister.doTbt_QuotationBasic.DepositFeeCurrencyType = quotationBasic.DepositFeeCurrencyType;
                if (quotationBasic.DepositFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.DepositFeeUsd = quotationBasic.DepositFee;
                else
                    doRegister.doTbt_QuotationBasic.DepositFee = quotationBasic.DepositFee;


                doRegister.doTbt_QuotationBasic.MaintenanceFee1CurrencyType = quotationBasic.MaintenanceFee1CurrencyType;
                if (quotationBasic.MaintenanceFee1CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1Usd = quotationBasic.MaintenanceFee1;
                else
                    //T.irie
                    //doRegister.doTbt_QuotationBasic.ContractFee = quotationBasic.MaintenanceFee1;
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1 = quotationBasic.MaintenanceFee1;

                doRegister.doTbt_QuotationBasic.AdditionalFee1CurrencyType = quotationBasic.AdditionalFee1CurrencyType;
                if (quotationBasic.AdditionalFee1CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.AdditionalFee1Usd = quotationBasic.AdditionalFee1;
                else
                    doRegister.doTbt_QuotationBasic.AdditionalFee1 = quotationBasic.AdditionalFee1;

                doRegister.doTbt_QuotationBasic.AdditionalFee2CurrencyType = quotationBasic.AdditionalFee2CurrencyType;
                if (quotationBasic.AdditionalFee2CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.AdditionalFee2Usd = quotationBasic.AdditionalFee2;
                else
                    doRegister.doTbt_QuotationBasic.AdditionalFee2 = quotationBasic.AdditionalFee2;

                doRegister.doTbt_QuotationBasic.AdditionalFee3CurrencyType = quotationBasic.AdditionalFee3CurrencyType;
                if (quotationBasic.AdditionalFee3CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.AdditionalFee3Usd = quotationBasic.AdditionalFee3;
                else
                    doRegister.doTbt_QuotationBasic.AdditionalFee3 = quotationBasic.AdditionalFee3;


                bool isQuotation = false;
                if (doInitData.doQuotationHeaderData != null)
                {
                    if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                    {
                        doRegister.doTbt_QuotationBasic.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;

                        if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode
                            == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                            isQuotation = true;
                    }
                }
                if (doInitData.doQuotationBasic != null)
                {
                    if (isQuotation == false)
                        doRegister.doTbt_QuotationBasic.LastOccNo = doInitData.doQuotationBasic.LastOccNo;

                    doRegister.doTbt_QuotationBasic.MaintenanceFee2 = doInitData.doQuotationBasic.MaintenanceFee2;
                }

                if (doInitData.PriorProductCode == quotationBasic.ProductCode)
                {
                    if (doInitData.doQuotationBasic != null)
                    {
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = doInitData.doQuotationBasic.ContractDurationMonth;
                        doRegister.doTbt_QuotationBasic.AutoRenewMonth = doInitData.doQuotationBasic.AutoRenewMonth;
                    }
                }
                else
                {
                    IProductMasterHandler phandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    List<tbm_Product> pLst = phandler.GetTbm_Product(quotationBasic.ProductCode, null);
                    if (pLst.Count >= 0)
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = pLst[0].DepreciationPeriodContract;

                    doRegister.doTbt_QuotationBasic.AutoRenewMonth = SECOM_AJIS.Common.Util.ConstantValue.Quotation.C_DEFAULT_AUTO_RENEW_MONTH;
                }

                if (doInitData.doQuotationHeaderData != null && doBeatGuardDetail != null)
                {
                    doRegister.doTbt_QuotationBeatGuardDetails = CommonUtil.CloneObject<QUS030_doBeatGuardDetail, tbt_QuotationBeatGuardDetails>(doBeatGuardDetail);

                    if (doBeatGuardDetail.NumOfBeatStep != null)
                        doRegister.doTbt_QuotationBeatGuardDetails.NumOfBeatStep = doBeatGuardDetail.NumOfBeatStep.Value;
                    if (doBeatGuardDetail.NumOfDate != null)
                        doRegister.doTbt_QuotationBeatGuardDetails.NumOfDate = doBeatGuardDetail.NumOfDate.Value;

                    if (doBeatGuardDetail.NumOfDayTimeWd != null)
                        doRegister.doTbt_QuotationBeatGuardDetails.NumOfDayTimeWd = doBeatGuardDetail.NumOfDayTimeWd.Value;
                    if (doBeatGuardDetail.NumOfNightTimeWd != null)
                        doRegister.doTbt_QuotationBeatGuardDetails.NumOfNightTimeWd = doBeatGuardDetail.NumOfNightTimeWd.Value;
                    if (doBeatGuardDetail.NumOfDayTimeSat != null)
                        doRegister.doTbt_QuotationBeatGuardDetails.NumOfDayTimeSat = doBeatGuardDetail.NumOfDayTimeSat.Value;
                    if (doBeatGuardDetail.NumOfNightTimeSat != null)
                        doRegister.doTbt_QuotationBeatGuardDetails.NumOfNightTimeSat = doBeatGuardDetail.NumOfNightTimeSat.Value;
                    if (doBeatGuardDetail.NumOfDayTimeSun != null)
                        doRegister.doTbt_QuotationBeatGuardDetails.NumOfDayTimeSun = doBeatGuardDetail.NumOfDayTimeSun.Value;
                    if (doBeatGuardDetail.NumOfNightTimeSun != null)
                        doRegister.doTbt_QuotationBeatGuardDetails.NumOfNightTimeSun = doBeatGuardDetail.NumOfNightTimeSun.Value;

                    if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                        doRegister.doTbt_QuotationBeatGuardDetails.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;
                    doRegister.doTbt_QuotationBeatGuardDetails.Alphabet = null;
                }

                #region Update Session

                param.RegisterData = doRegister;
                QUS030_ScreenData = param;

                #endregion

                res.ResultData = doRegister;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Sentry Guard Action

        /// <summary>
        /// Check quotation detail data in case of sentry guard (step 1) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckSentryGuardQuotationData_P1(QUS030_tbt_QuotationBasic_SG quotationBasic)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;
                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;
                if (doInitData == null)
                    doInitData = new doInitQuotationData();

                #endregion
                #region Check is Suspending

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                #endregion

                if (quotationBasic.IsEditMode == true)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2005);
                    return Json(res);
                }

                #region Validate Object

                ValidatorUtil validator = new ValidatorUtil(this);


                /* --- Merge --- */
                for (int idx = 0; idx < 3; idx++)
                {
                    PropertyInfo propApproveNo = quotationBasic.GetType().GetProperty("AdditionalApproveNo" + (idx + 1).ToString());
                    PropertyInfo propApproveFee = quotationBasic.GetType().GetProperty("AdditionalFee" + (idx + 1).ToString());
                    if (propApproveNo != null && propApproveFee != null)
                    {
                        object propApproveNoValue = propApproveNo.GetValue(quotationBasic, null);
                        object propApproveFeeValue = propApproveFee.GetValue(quotationBasic, null);

                        if (CommonUtil.IsNullOrEmpty(propApproveNoValue)
                            || CommonUtil.IsNullOrEmpty(propApproveFeeValue))
                        {
                            string ctrlName = null;
                            string ctrlID = null;
                            if (CommonUtil.IsNullOrEmpty(propApproveNoValue) == false)
                            {
                                ctrlID = "lblAdditionalContractFee" + (idx + 1).ToString();
                                ctrlName = "AdditionalFee" + (idx + 1).ToString();
                            }
                            else if (CommonUtil.IsNullOrEmpty(propApproveFeeValue) == false)
                            {
                                ctrlID = "lblAdditionalApproveNo" + (idx + 1).ToString();
                                ctrlName = "AdditionalApproveNo" + (idx + 1).ToString();
                            }

                            if (ctrlName != null)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                "QUS030",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                ctrlName,
                                                ctrlID,
                                                ctrlName,
                                                (3 + idx).ToString());
                            }
                        }
                    }
                }
                /* ------------- */

                bool isSGEmpty = true;
                if (doInitData.SentryGuardDetailList != null)
                {
                    if (doInitData.SentryGuardDetailList.Count > 0)
                        isSGEmpty = false;
                }

                /* --- Merge --- */
                /* if (isSGEmpty)
                    validator.AddErrorMessage(  MessageUtil.MODULE_QUOTATION,
                                                "QUS030",
                                                MessageUtil.MODULE_COMMON, 
                                                MessageUtil.MessageList.MSG0007,
                                                "SentryGuardDetail",
                                                "lblSGList", 
                                                "SentryGuardDetail",
                                                "5"); */
                if (isSGEmpty)
                    validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                "QUS030",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                "SentryGuardDetail",
                                                "lblSGList",
                                                "SentryGuardDetail",
                                                "8");
                /* ------------- */

                ValidatorUtil.BuildErrorMessage(res, validator);
                if (res.IsError)
                    return Json(res);

                #endregion
                #region Validate Sentry guard

                for (int i = 0; i < doInitData.SentryGuardDetailList.Count; i++)
                {
                    doSentryGuardDetail sg = doInitData.SentryGuardDetailList[i];

                    List<string> eLst = new List<string>();
                    if (CommonUtil.IsNullOrEmpty(sg.SentryGuardTypeCode))
                        eLst.Add("lblSentryGuardType");
                    if (CommonUtil.IsNullOrEmpty(sg.NumOfDate))
                        eLst.Add("lblNumberOfDate");
                    if (CommonUtil.IsNullOrEmpty(sg.SecurityStartTime))
                        eLst.Add("lblSecurityStartTime");
                    if (CommonUtil.IsNullOrEmpty(sg.SecurityFinishTime))
                        eLst.Add("lblSecurityFinishTime");
                    if (CommonUtil.IsNullOrEmpty(sg.CostPerHour))
                        eLst.Add("lblCostPerHour");
                    if (CommonUtil.IsNullOrEmpty(sg.NumOfSentryGuard))
                        eLst.Add("lblNumerOfSentryGuard");

                    if (eLst.Count > 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                            "QUS030",
                                            MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0007,
                                            new string[] { CommonUtil.TextList(eLst.ToArray()) });
                        return Json(res);
                    }
                }

                #endregion
                #region Validate Range

                QUS030_ValidateRangeData valRange = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic_SG, QUS030_ValidateRangeData>(quotationBasic);

                Dictionary<string, RangeNumberValueAttribute> rangeAttr =
                    CommonUtil.CreateAttributeDictionary<RangeNumberValueAttribute>(valRange);
                foreach (KeyValuePair<string, RangeNumberValueAttribute> attr in rangeAttr)
                {
                    PropertyInfo prop = valRange.GetType().GetProperty(attr.Key);
                    if (prop != null)
                    {
                        object val = prop.GetValue(valRange, null);
                        if (CommonUtil.IsNullOrEmpty(val) == false)
                        {
                            if (attr.Value.IsValid(val) == false)
                            {
                                string format = "N2";
                                if (prop.PropertyType == typeof(int?))
                                    format = "N0";

                                string min = attr.Value.Min.ToString(format);
                                string max = attr.Value.Max.ToString(format);

                                res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    "QUS030",
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2087,
                                    new string[] { attr.Value.Parameter, min, max },
                                    new string[] { attr.Value.ControlName });
                                return Json(res);
                            }
                        }
                    }
                }

                int line = 1;
                foreach (doSentryGuardDetail sg in doInitData.SentryGuardDetailList)
                {
                    QUS030_ValidateRangeData_SentryGuard valInstRange =
                        CommonUtil.CloneObject<doSentryGuardDetail, QUS030_ValidateRangeData_SentryGuard>(sg);

                    rangeAttr = CommonUtil.CreateAttributeDictionary<RangeNumberValueAttribute>(valInstRange);
                    foreach (KeyValuePair<string, RangeNumberValueAttribute> attr in rangeAttr)
                    {
                        PropertyInfo prop = valInstRange.GetType().GetProperty(attr.Key);
                        if (prop != null)
                        {
                            object val = prop.GetValue(valInstRange, null);
                            if (CommonUtil.IsNullOrEmpty(val) == false)
                            {
                                if (attr.Value.IsValid(val) == false)
                                {
                                    string format = "N2";
                                    if (prop.PropertyType == typeof(int?))
                                        format = "N0";

                                    string min = attr.Value.Min.ToString(format);
                                    string max = attr.Value.Max.ToString(format);

                                    res.AddErrorMessage(
                                        MessageUtil.MODULE_QUOTATION,
                                        "QUS030",
                                        MessageUtil.MODULE_QUOTATION,
                                        MessageUtil.MessageList.MSG2089,
                                        new string[] { line.ToString("N0"), attr.Value.Parameter, min, max },
                                        new string[] { attr.Value.ControlName });
                                    return Json(res);
                                }
                            }
                        }
                    }
                    Dictionary<string, TimeValueAttribute> timeAttr = CommonUtil.CreateAttributeDictionary<TimeValueAttribute>(valInstRange);
                    foreach (KeyValuePair<string, TimeValueAttribute> attr in timeAttr)
                    {
                        PropertyInfo prop = valInstRange.GetType().GetProperty(attr.Key);
                        if (prop != null)
                        {
                            object val = prop.GetValue(valInstRange, null);
                            if (CommonUtil.IsNullOrEmpty(val) == false)
                            {
                                if (attr.Value.IsValid(val) == false)
                                {
                                    res.AddErrorMessage(
                                        MessageUtil.MODULE_QUOTATION,
                                        "QUS030",
                                        MessageUtil.MODULE_QUOTATION,
                                        MessageUtil.MessageList.MSG2089,
                                        new string[] { line.ToString("N0"), attr.Value.Parameter, "00:00", "23:59" },
                                        new string[] { attr.Value.ControlName });
                                    return Json(res);
                                }
                            }
                        }
                    }

                    line++;
                }

                #endregion

                ValidateEmployeeData(res, quotationBasic);
                if (res.IsError)
                    return Json(res);

                #region Validate Sentry guard

                List<doMiscTypeCode> miscLst = new List<doMiscTypeCode>();
                miscLst.Add(new doMiscTypeCode()
                {
                    FieldName = MiscType.C_SG_TYPE,
                    ValueCode = "%"
                });
                miscLst.Add(new doMiscTypeCode()
                {
                    FieldName = MiscType.C_NUM_OF_DATE,
                    ValueCode = "%"
                });

                ICommonHandler cmmhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                miscLst = cmmhandler.GetMiscTypeCodeList(miscLst);

                for (int i = 0; i < doInitData.SentryGuardDetailList.Count; i++)
                {
                    bool isFoundSGType = false;
                    bool isFoundNumDate = false;
                    foreach (doMiscTypeCode misc in miscLst)
                    {
                        if (misc.FieldName == MiscType.C_SG_TYPE
                            && misc.ValueCode == doInitData.SentryGuardDetailList[i].SentryGuardTypeCode)
                        {
                            isFoundSGType = true;
                        }
                        else if (misc.FieldName == MiscType.C_NUM_OF_DATE
                            && doInitData.SentryGuardDetailList[i].NumOfDate != null)
                        {
                            if (misc.ValueCode == doInitData.SentryGuardDetailList[i].NumOfDate.Value.ToString())
                                isFoundNumDate = true;
                        }
                    }

                    if (isFoundSGType == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                            MessageUtil.MessageList.MSG2068,
                                            new string[] { (i + 1).ToString() });
                        return Json(res);
                    }
                    if (isFoundNumDate == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                            MessageUtil.MessageList.MSG2074,
                                            new string[] { (i + 1).ToString() });
                        return Json(res);
                    }
                }

                #endregion

                ValidateProduct(res, quotationBasic);
                if (res.IsError)
                    return Json(res);
                else
                    CheckSentryGuardQuotationData(res, quotationBasic, doInitData.SentryGuardDetailList);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check quotation detail data in case of sentry guard (step 2) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckSentryGuardQuotationData_P2(QUS030_tbt_QuotationBasic_SG quotationBasic)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;
                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;
                if (doInitData == null)
                    doInitData = new doInitQuotationData();

                #endregion

                CheckSentryGuardQuotationData(res, quotationBasic, doInitData.SentryGuardDetailList);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region Sentry Guard Methods

        /// <summary>
        /// Initial quotation detail data in case of sentry guard
        /// </summary>
        /// <param name="doRentalContractData"></param>
        /// <returns></returns>
        private doInitRentalQuotationData InitSGQuotationData(dsRentalContractData doRentalContractData)
        {
            try
            {
                doInitRentalQuotationData initDo = new doInitRentalQuotationData();
                if (doRentalContractData.dtTbt_RentalSecurityBasic != null)
                {
                    if (doRentalContractData.dtTbt_RentalSecurityBasic.Count > 0)
                    {
                        #region Create Quotation Basic

                        initDo.doQuotationBasic = new tbt_QuotationBasic()
                        {
                            ProductCode = doRentalContractData.dtTbt_RentalSecurityBasic[0].ProductCode,
                            ContractDurationMonth = doRentalContractData.dtTbt_RentalSecurityBasic[0].ContractDurationMonth,
                            AutoRenewMonth = doRentalContractData.dtTbt_RentalSecurityBasic[0].AutoRenewMonth,
                            LastOccNo = doRentalContractData.dtTbt_RentalSecurityBasic[0].OCC
                        };

                        #endregion
                    }
                }
                if (initDo.doQuotationBasic != null && doRentalContractData.dtTbt_RentalSentryGuard != null)
                {
                    if (doRentalContractData.dtTbt_RentalSentryGuard.Count > 0)
                    {
                        initDo.doQuotationBasic.SecurityItemFee = doRentalContractData.dtTbt_RentalSentryGuard[0].SecurityItemFee;
                        initDo.doQuotationBasic.SecurityItemFeeCurrencyType = doRentalContractData.dtTbt_RentalSentryGuard[0].SecurityItemFeeCurrencyType;
                        initDo.doQuotationBasic.SecurityItemFeeUsd = doRentalContractData.dtTbt_RentalSentryGuard[0].SecurityItemFeeUsd;
                        initDo.doQuotationBasic.OtherItemFeeCurrencyType = doRentalContractData.dtTbt_RentalSentryGuard[0].OtherItemFeeCurrencyType;
                        initDo.doQuotationBasic.OtherItemFeeUsd = doRentalContractData.dtTbt_RentalSentryGuard[0].OtherItemFeeUsd;
                        initDo.doQuotationBasic.OtherItemFee = doRentalContractData.dtTbt_RentalSentryGuard[0].OtherItemFee;
                        initDo.doQuotationBasic.SentryGuardAreaTypeCode = doRentalContractData.dtTbt_RentalSentryGuard[0].SentryGuardAreaTypeCode;
                    }
                }

                if (doRentalContractData.dtTbt_RentalSentryGuardDetails != null)
                {
                    foreach (tbt_RentalSentryGuardDetails rsd in doRentalContractData.dtTbt_RentalSentryGuardDetails)
                    {
                        if (initDo.SentryGuardDetailList == null)
                            initDo.SentryGuardDetailList = new List<doSentryGuardDetail>();

                        doSentryGuardDetail doSG = CommonUtil.CloneObject<tbt_RentalSentryGuardDetails, doSentryGuardDetail>(rsd);
                        if (doSG != null)
                        {
                            doSG.RunningNo = rsd.SequenceNo;
                            doSG.CostPerHour = rsd.TimeUnitPrice;
                            initDo.SentryGuardDetailList.Add(doSG);
                        }
                    }

                    #region Set Misc Name

                    if (initDo.SentryGuardDetailList != null)
                    {
                        MiscTypeMappingList miscLst = new MiscTypeMappingList();
                        miscLst.AddMiscType(initDo.SentryGuardDetailList.ToArray());

                        ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        chandler.MiscTypeMappingList(miscLst);
                    }

                    #endregion
                    #region Sort data

                    if (initDo.SentryGuardDetailList != null)
                    {
                        initDo.SentryGuardDetailList = (
                        from x in initDo.SentryGuardDetailList
                        orderby x.SentryGuardTypeCode, x.NumOfDate, x.SecurityStartTime
                        select x).ToList();
                    }

                    #endregion
                }

                return initDo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Initial quotation detail data in case of sentry guard from import file
        /// </summary>
        /// <param name="res"></param>
        /// <param name="initData"></param>
        /// <param name="importData"></param>
        private void InitImportSGQuotation(ObjectResultData res, doInitQuotationData initData, dsImportData importData)
        {
            try
            {
                #region Create Quotation Basic

                if (importData.dtTbt_QuotationBasic != null)
                {
                    if (importData.dtTbt_QuotationBasic.Count > 0)
                    {
                        tbt_QuotationBasic nDo = importData.dtTbt_QuotationBasic[0];
                        if (initData.doQuotationBasic != null)
                        {
                            nDo.LastOccNo = initData.doQuotationBasic.LastOccNo;

                            if (initData.doQuotationBasic.ProductCode == nDo.ProductCode)
                            {
                                nDo.ContractDurationMonth = initData.doQuotationBasic.ContractDurationMonth;
                                nDo.AutoRenewMonth = initData.doQuotationBasic.AutoRenewMonth;
                            }
                        }

                        if (initData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                            nDo.DepositFee = null;

                        initData.doQuotationBasic = nDo;


                        #region Mapping Employee Name

                        IEmployeeMasterHandler ehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                        EmployeeMappingList empLst = new EmployeeMappingList();
                        empLst.AddEmployee(initData.doQuotationBasic);
                        ehandler.ActiveEmployeeListMapping(empLst);

                        #endregion
                    }
                }

                #endregion
                #region Prepare Sentry Guard Detail

                initData.SentryGuardDetailList = new List<doSentryGuardDetail>();
                if (importData.dtTbt_QuotationSentryGuardDetails != null)
                {
                    bool isError = false;

                    List<QUS030_tbt_QuotationSentryGuardDetail_Import> impSGLst =
                        CommonUtil.ClonsObjectList<tbt_QuotationSentryGuardDetails, QUS030_tbt_QuotationSentryGuardDetail_Import>(importData.dtTbt_QuotationSentryGuardDetails);

                    #region Set Misc Name

                    MiscTypeMappingList miscLst = new MiscTypeMappingList();
                    miscLst.AddMiscType(impSGLst.ToArray());

                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    chandler.MiscTypeMappingList(miscLst);

                    #endregion

                    /* --- Merge --- */
                    #region Sort data

                    if (impSGLst != null)
                    {
                        impSGLst = (
                        from x in impSGLst
                        orderby x.SentryGuardTypeCode, x.NumOfDate, x.SecurityStartTime
                        select x).ToList();
                    }

                    #endregion
                    /* ------------- */

                    int runningNo = 1;
                    foreach (QUS030_tbt_QuotationSentryGuardDetail_Import sgd in impSGLst)
                    {
                        int dt1_hr = 0;
                        int dt1_min = 0;
                        DateTime dt1 = new DateTime();
                        int dt2_hr = 0;
                        int dt2_min = 0;
                        DateTime dt2 = new DateTime();

                        if (sgd.SecurityStartTime != null
                            && sgd.SecurityFinishTime != null)
                        {
                            dt1_hr = sgd.SecurityStartTime.Value.Hours;
                            dt1_min = sgd.SecurityStartTime.Value.Minutes;
                            dt2_hr = sgd.SecurityFinishTime.Value.Hours;
                            dt2_min = sgd.SecurityFinishTime.Value.Minutes;

                            if (dt1_hr > dt2_hr)
                                dt2 = dt2.AddDays(1);
                            else if (dt1_hr == dt2_hr && dt1_min > dt2_min)
                                dt2 = dt2.AddDays(1);
                        }

                        dt1 = dt1.AddHours(dt1_hr);
                        dt1 = dt1.AddMinutes(dt1_min);
                        dt2 = dt2.AddHours(dt2_hr);
                        dt2 = dt2.AddMinutes(dt2_min);

                        decimal diff = 0;
                        if (dt1 <= dt2)
                        {
                            diff = (decimal)(dt2 - dt1).TotalHours;
                            if (sgd.NumOfDate != null)
                            {
                                diff = Math.Round(diff * sgd.NumOfDate.Value, 2, MidpointRounding.AwayFromZero);
                            }
                        }

                        doSentryGuardDetail nSgd = new doSentryGuardDetail()
                        {
                            RunningNo = runningNo,
                            SentryGuardTypeCode = sgd.SentryGuardTypeCode,
                            NumOfDate = sgd.NumOfDate,
                            SecurityStartTime = sgd.SecurityStartTime,
                            SecurityFinishTime = sgd.SecurityFinishTime,
                            CostPerHour = sgd.CostPerHour,
                            NumOfSentryGuard = sgd.NumOfSentryGuard,
                            SentryGuardTypeName = sgd.SentryGuardTypeName,
                            WorkHourPerMonth = diff
                        };
                        initData.SentryGuardDetailList.Add(nSgd);

                        runningNo++;
                    }
                    if (isError)
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2078);
                }

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check quotation detail data in case of sentry guard is correct?
        /// </summary>
        /// <param name="res"></param>
        /// <param name="quotationBasic"></param>
        /// <param name="sgLst"></param>
        private void CheckSentryGuardQuotationData(ObjectResultData res, QUS030_tbt_QuotationBasic_SG quotationBasic, List<doSentryGuardDetail> sgLst)
        {
            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;

                #endregion

                doRegisterQuotationData doRegister = new doRegisterQuotationData();
                doRegister.doQuotationHeaderData = doInitData.doQuotationHeaderData;

                doRegister.doTbt_QuotationBasic = new tbt_QuotationBasic();
                doRegister.doTbt_QuotationBasic.ProductCode = quotationBasic.ProductCode;
                doRegister.doTbt_QuotationBasic.ContractTransferStatus =
                        SECOM_AJIS.Common.Util.ConstantValue.ContractTransferStatus.C_CONTRACT_TRANS_STATUS_QTN_REG;
                doRegister.doTbt_QuotationBasic.LockStatus = SECOM_AJIS.Common.Util.ConstantValue.LockStatus.C_LOCK_STATUS_UNLOCK;

                doRegister.doTbt_QuotationBasic.FireMonitorFlag = false;
                doRegister.doTbt_QuotationBasic.CrimePreventFlag = false;
                doRegister.doTbt_QuotationBasic.EmergencyReportFlag = false;
                doRegister.doTbt_QuotationBasic.FacilityMonitorFlag = false;
                doRegister.doTbt_QuotationBasic.BeatGuardFlag = false;
                doRegister.doTbt_QuotationBasic.SentryGuardFlag = true;
                doRegister.doTbt_QuotationBasic.MaintenanceFlag = false;

                doRegister.doTbt_QuotationBasic.QuotationNo = quotationBasic.QuotationNo;
                doRegister.doTbt_QuotationBasic.SalesmanEmpNo1 = quotationBasic.SalesmanEmpNo1;
                doRegister.doTbt_QuotationBasic.SalesmanEmpNo2 = quotationBasic.SalesmanEmpNo2;
                doRegister.doTbt_QuotationBasic.SalesSupporterEmpNo = quotationBasic.SalesSupporterEmpNo;

                doRegister.doTbt_QuotationBasic.MaintenanceFee1CurrencyType = quotationBasic.MaintenanceFee1CurrencyType;
                if (quotationBasic.MaintenanceFee1CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1Usd = quotationBasic.MaintenanceFee1;
                else
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1 = quotationBasic.MaintenanceFee1;

                doRegister.doTbt_QuotationBasic.AdditionalFee1CurrencyType = quotationBasic.AdditionalFee1CurrencyType;
                doRegister.doTbt_QuotationBasic.AdditionalFee2CurrencyType = quotationBasic.AdditionalFee2CurrencyType;
                doRegister.doTbt_QuotationBasic.AdditionalFee3CurrencyType = quotationBasic.AdditionalFee3CurrencyType;
                if (quotationBasic.AdditionalFee3CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.AdditionalFee3Usd = quotationBasic.AdditionalFee3;
                else
                    doRegister.doTbt_QuotationBasic.AdditionalFee3 = quotationBasic.AdditionalFee3;

                if (quotationBasic.AdditionalFee2CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.AdditionalFee2Usd = quotationBasic.AdditionalFee2;
                else
                    doRegister.doTbt_QuotationBasic.AdditionalFee2 = quotationBasic.AdditionalFee2;

                if (quotationBasic.AdditionalFee1CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.AdditionalFee1Usd = quotationBasic.AdditionalFee1;
                else
                    doRegister.doTbt_QuotationBasic.AdditionalFee1 = quotationBasic.AdditionalFee1;

                doRegister.doTbt_QuotationBasic.AdditionalApproveNo1 = quotationBasic.AdditionalApproveNo1;
                doRegister.doTbt_QuotationBasic.AdditionalApproveNo2 = quotationBasic.AdditionalApproveNo2;
                doRegister.doTbt_QuotationBasic.AdditionalApproveNo3 = quotationBasic.AdditionalApproveNo3;
                doRegister.doTbt_QuotationBasic.ApproveNo1 = quotationBasic.ApproveNo1;
                doRegister.doTbt_QuotationBasic.ApproveNo2 = quotationBasic.ApproveNo2;
                doRegister.doTbt_QuotationBasic.ApproveNo3 = quotationBasic.ApproveNo3;
                doRegister.doTbt_QuotationBasic.ApproveNo4 = quotationBasic.ApproveNo4;
                doRegister.doTbt_QuotationBasic.ApproveNo5 = quotationBasic.ApproveNo5;

                doRegister.doTbt_QuotationBasic.ContractFeeCurrencyType = quotationBasic.ContractFeeCurrencyType;
                if (quotationBasic.ContractFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.ContractFeeUsd = quotationBasic.ContractFee;
                else
                    doRegister.doTbt_QuotationBasic.ContractFee = quotationBasic.ContractFee;

                doRegister.doTbt_QuotationBasic.DepositFeeCurrencyType = quotationBasic.DepositFeeCurrencyType;
                if (quotationBasic.DepositFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.DepositFeeUsd = quotationBasic.DepositFee;
                else
                    doRegister.doTbt_QuotationBasic.DepositFee = quotationBasic.DepositFee;

                doRegister.doTbt_QuotationBasic.SecurityItemFeeCurrencyType = quotationBasic.SecurityItemFeeCurrencyType;
                if (quotationBasic.SecurityItemFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.SecurityItemFeeUsd = quotationBasic.SecurityItemFee;
                else
                    doRegister.doTbt_QuotationBasic.SecurityItemFee = quotationBasic.SecurityItemFee;

                doRegister.doTbt_QuotationBasic.OtherItemFeeCurrencyType = quotationBasic.OtherItemFeeCurrencyType;
                if (quotationBasic.OtherItemFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.OtherItemFeeUsd = quotationBasic.OtherItemFee;
                else
                    doRegister.doTbt_QuotationBasic.OtherItemFee = quotationBasic.OtherItemFee;

                doRegister.doTbt_QuotationBasic.SentryGuardAreaTypeCode = quotationBasic.SentryGuardAreaTypeCode;

                bool isQuotation = false;
                if (doInitData.doQuotationHeaderData != null)
                {
                    if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                    {
                        doRegister.doTbt_QuotationBasic.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;

                        if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode
                            == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                            isQuotation = true;
                    }
                }
                if (doInitData.doQuotationBasic != null)
                {
                    if (isQuotation == false)
                        doRegister.doTbt_QuotationBasic.LastOccNo = doInitData.doQuotationBasic.LastOccNo;

                    doRegister.doTbt_QuotationBasic.MaintenanceFee2 = doInitData.doQuotationBasic.MaintenanceFee2;
                }

                if (doInitData.PriorProductCode == quotationBasic.ProductCode)
                {
                    if (doInitData.doQuotationBasic != null)
                    {
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = doInitData.doQuotationBasic.ContractDurationMonth;
                        doRegister.doTbt_QuotationBasic.AutoRenewMonth = doInitData.doQuotationBasic.AutoRenewMonth;
                    }
                }
                else
                {
                    IProductMasterHandler phandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    List<tbm_Product> pLst = phandler.GetTbm_Product(quotationBasic.ProductCode, null);
                    if (pLst.Count >= 0)
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = pLst[0].DepreciationPeriodContract;

                    doRegister.doTbt_QuotationBasic.AutoRenewMonth = SECOM_AJIS.Common.Util.ConstantValue.Quotation.C_DEFAULT_AUTO_RENEW_MONTH;
                }

                if (doInitData.doQuotationHeaderData != null && sgLst != null)
                {
                    decimal SentryGuardFee = 0;
                    decimal SentryGuardFeeUsd = 0;
                    string SentryGuardFeeCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                    int lineNo = 1;
                    foreach (doSentryGuardDetail sg in sgLst)
                    {
                        if (doRegister.SentryGuardList == null)
                            doRegister.SentryGuardList = new List<tbt_QuotationSentryGuardDetails>();

                        tbt_QuotationSentryGuardDetails sgd = CommonUtil.CloneObject<doSentryGuardDetail, tbt_QuotationSentryGuardDetails>(sg);
                        if (sgd != null)
                        {
                            if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                                sgd.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;
                            sgd.RunningNo = lineNo;

                            doRegister.SentryGuardList.Add(sgd);
                            lineNo++;

                            if (sgd.WorkHourPerMonth != null
                                && sgd.CostPerHour != null
                                && sgd.NumOfSentryGuard != null)
                            {
                                SentryGuardFeeCurrencyType = sgd.CostPerHourCurrencyType;
                                if (sgd.CostPerHourCurrencyType == CurrencyUtil.C_CURRENCY_US)
                                {
                                    SentryGuardFeeUsd += sgd.WorkHourPerMonth.Value * sgd.CostPerHour.Value * sgd.NumOfSentryGuard.Value;
                                }
                                else
                                {
                                    SentryGuardFee += sgd.WorkHourPerMonth.Value * sgd.CostPerHour.Value * sgd.NumOfSentryGuard.Value;
                                }
                            }
                        }
                    }

                    if (doRegister.doTbt_QuotationBasic != null)
                    {
                        doRegister.doTbt_QuotationBasic.SentryGuardFee = SentryGuardFee;
                        doRegister.doTbt_QuotationBasic.SentryGuardFeeUsd = SentryGuardFeeUsd;
                        doRegister.doTbt_QuotationBasic.SentryGuardFeeCurrencyType = SentryGuardFeeCurrencyType;
                        doRegister.doTbt_QuotationBasic.TotalSentryGuardFee = 0;
                        doRegister.doTbt_QuotationBasic.TotalSentryGuardFeeUsd = 0;
                      
                        
                        if (doRegister.doTbt_QuotationBasic.SecurityItemFee != null) {
                            doRegister.doTbt_QuotationBasic.TotalSentryGuardFee += doRegister.doTbt_QuotationBasic.SecurityItemFee;
                        }
                        if (doRegister.doTbt_QuotationBasic.SecurityItemFeeUsd != null)
                        {
                            doRegister.doTbt_QuotationBasic.TotalSentryGuardFeeUsd += doRegister.doTbt_QuotationBasic.SecurityItemFeeUsd;
                        }
                        doRegister.doTbt_QuotationBasic.TotalSentryGuardFeeCurrencyType = doRegister.doTbt_QuotationBasic.OtherItemFeeCurrencyType;
                        if (doRegister.doTbt_QuotationBasic.OtherItemFee != null)
                        {
                            doRegister.doTbt_QuotationBasic.TotalSentryGuardFee += doRegister.doTbt_QuotationBasic.OtherItemFee;
                        }
                        if (doRegister.doTbt_QuotationBasic.OtherItemFeeUsd != null)
                        {
                            doRegister.doTbt_QuotationBasic.TotalSentryGuardFeeUsd += doRegister.doTbt_QuotationBasic.OtherItemFeeUsd;
                        }
                        doRegister.doTbt_QuotationBasic.TotalSentryGuardFee += SentryGuardFee;
                        doRegister.doTbt_QuotationBasic.TotalSentryGuardFeeUsd += SentryGuardFeeUsd;
                        if (
                            ((doRegister.doTbt_QuotationBasic.TotalSentryGuardFee !=null && doRegister.doTbt_QuotationBasic.TotalSentryGuardFee >0)
                            && (doRegister.doTbt_QuotationBasic.TotalSentryGuardFee > 0 && doRegister.doTbt_QuotationBasic.TotalSentryGuardFee != null) )
                                || (!(doRegister.doTbt_QuotationBasic.TotalSentryGuardFee != null && doRegister.doTbt_QuotationBasic.TotalSentryGuardFee > 0)
                                    && !(doRegister.doTbt_QuotationBasic.TotalSentryGuardFee > 0 && doRegister.doTbt_QuotationBasic.TotalSentryGuardFee != null)))
                        {
                            doRegister.doTbt_QuotationBasic.TotalSentryGuardFeeCurrencyType = null;
                        }
                        else
                        {
                            doRegister.doTbt_QuotationBasic.TotalSentryGuardFeeCurrencyType = doRegister.doTbt_QuotationBasic.TotalSentryGuardFeeCurrencyType;
                        }
                    }
                }

                #region Update Session

                param.RegisterData = doRegister;
                QUS030_ScreenData = param;

                #endregion

                res.ResultData = doRegister;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Maintenance Detail Action

        /// <summary>
        /// Check quotation detail data in case of maintenance (step 1) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <param name="maLst"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckMaintenanceDetailQuotationData_P1(QUS030_tbt_QuotationBasic_MA quotationBasic, List<doContractHeader> maLst)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;

                #endregion

                #region Check is Suspending

                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                #endregion
                #region Validate Object

                ValidatorUtil validator = new ValidatorUtil(this);

                /* --- Merge --- */
                for (int idx = 0; idx < 3; idx++)
                {
                    PropertyInfo propApproveNo = quotationBasic.GetType().GetProperty("AdditionalApproveNo" + (idx + 1).ToString());
                    PropertyInfo propApproveFee = quotationBasic.GetType().GetProperty("AdditionalFee" + (idx + 1).ToString());
                    if (propApproveNo != null && propApproveFee != null)
                    {
                        object propApproveNoValue = propApproveNo.GetValue(quotationBasic, null);
                        object propApproveFeeValue = propApproveFee.GetValue(quotationBasic, null);

                        if (CommonUtil.IsNullOrEmpty(propApproveNoValue)
                            || CommonUtil.IsNullOrEmpty(propApproveFeeValue))
                        {
                            string ctrlName = null;
                            string ctrlID = null;
                            if (CommonUtil.IsNullOrEmpty(propApproveNoValue) == false)
                            {
                                ctrlID = "lblAdditionalContractFee" + (idx + 1).ToString();
                                ctrlName = "AdditionalFee" + (idx + 1).ToString();
                            }
                            else if (CommonUtil.IsNullOrEmpty(propApproveFeeValue) == false)
                            {
                                ctrlID = "lblAdditionalApproveNo" + (idx + 1).ToString();
                                ctrlName = "AdditionalApproveNo" + (idx + 1).ToString();
                            }

                            if (ctrlName != null)
                            {
                                validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                "QUS030",
                                                MessageUtil.MODULE_COMMON,
                                                MessageUtil.MessageList.MSG0007,
                                                ctrlName,
                                                ctrlID,
                                                ctrlName,
                                                (3 + idx).ToString());
                            }
                        }
                    }
                }
                /* ------------- */

                if (quotationBasic.MaintenanceTargetProductTypeCode == MaintenanceTargetProductType.C_MA_TARGET_PROD_TYPE_SECOM)
                {
                    if (CommonUtil.IsNullOrEmpty(quotationBasic.MaintenanceTypeCode) == true)
                    {
                        /* --- Merge --- */
                        /* validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "MaintenanceTypeCode",
                                                    "lblMaintenanceType",
                                                    "MaintenanceTypeCode",
                                                    "5"); */
                        validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "MaintenanceTypeCode",
                                                    "lblMaintenanceType",
                                                    "MaintenanceTypeCode",
                                                    "8");
                        /* ------------- */

                    }

                    bool isMAEmpty = true;
                    if (maLst != null)
                    {
                        if (maLst.Count > 0)
                            isMAEmpty = (maLst[0] == null);
                    }
                    if (isMAEmpty)
                    {

                        /* --- Merge --- */
                        /* validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "MaintenanceDetail",
                                                    "lblMAList",
                                                    "MaintenanceDetail",
                                                    "7"); */
                        validator.AddErrorMessage(MessageUtil.MODULE_QUOTATION,
                                                    "QUS030",
                                                    MessageUtil.MODULE_COMMON,
                                                    MessageUtil.MessageList.MSG0007,
                                                    "MaintenanceDetail",
                                                    "lblMAList",
                                                    "MaintenanceDetail",
                                                    "9");
                        /* ------------- */

                    }
                }

                ValidatorUtil.BuildErrorMessage(res, validator);
                if (res.IsError)
                    return Json(res);

                #endregion
                #region Validate Maintenance

                if (maLst != null)
                {
                    for (int i = 0; i < maLst.Count; i++)
                    {
                        doContractHeader ma = maLst[i];
                        for (int j = 0; j < maLst.Count; j++)
                        {
                            if (i == j)
                                continue;


                            doContractHeader cma = maLst[j];
                            if (ma.ContractCode == cma.ContractCode)
                            {
                                res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2082,
                                    new string[] { ma.ContractCode });
                                return Json(res);
                            }
                        }
                    }
                }

                #endregion
                #region Validate Range

                QUS030_ValidateRangeData valRange = CommonUtil.CloneObject<QUS030_tbt_QuotationBasic_MA, QUS030_ValidateRangeData>(quotationBasic);

                Dictionary<string, RangeNumberValueAttribute> rangeAttr =
                    CommonUtil.CreateAttributeDictionary<RangeNumberValueAttribute>(valRange);
                foreach (KeyValuePair<string, RangeNumberValueAttribute> attr in rangeAttr)
                {
                    PropertyInfo prop = valRange.GetType().GetProperty(attr.Key);
                    if (prop != null)
                    {
                        object val = prop.GetValue(valRange, null);
                        if (CommonUtil.IsNullOrEmpty(val) == false)
                        {
                            if (attr.Value.IsValid(val) == false)
                            {
                                string format = "N2";
                                if (prop.PropertyType == typeof(int?))
                                    format = "N0";

                                string min = attr.Value.Min.ToString(format);
                                string max = attr.Value.Max.ToString(format);

                                res.AddErrorMessage(
                                    MessageUtil.MODULE_QUOTATION,
                                    "QUS030",
                                    MessageUtil.MODULE_QUOTATION,
                                    MessageUtil.MessageList.MSG2087,
                                    new string[] { attr.Value.Parameter, min, max },
                                    new string[] { attr.Value.ControlName });
                                return Json(res);
                            }
                        }
                    }
                }

                #endregion

                ValidateEmployeeData(res, quotationBasic);
                if (res.IsError)
                    return Json(res);

                #region Validate MA Product type

                if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                {
                    if (quotationBasic.MaintenanceTargetProductTypeCode != doInitData.PriorMaintenanceTargetProductTypeCode)
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2076);
                }

                #endregion
                #region Validate Maintenance Detail

                if (quotationBasic.MaintenanceTargetProductTypeCode == MaintenanceTargetProductType.C_MA_TARGET_PROD_TYPE_SECOM
                    && maLst != null)
                {
                    if (maLst.Count > 0)
                    {
                        CommonUtil cmm = new CommonUtil();
                        List<string> contractLst = new List<string>();
                        foreach (doContractHeader ml in maLst)
                        {
                            contractLst.Add(cmm.ConvertContractCode(ml.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                        }

                        try
                        {
                            IContractHandler chandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                            List<doContractHeader> contLst = chandler.CheckMaintenanceTargetContractList(contractLst, doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode); //TODO: Bug report QU-124 Add param quotation target code

                            bool isSiteError = false;
                            if (doInitData.doQuotationHeaderData.doQuotationSite == null)
                                isSiteError = true;
                            else
                            {
                                foreach (doContractHeader l in contLst)
                                {
                                    if (l.SiteCode != doInitData.doQuotationHeaderData.doQuotationSite.SiteCode)
                                    {
                                        isSiteError = true;
                                        break;
                                    }
                                }
                            }
                            if (isSiteError)
                                res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2042);
                        }
                        catch (ApplicationException ex)
                        {
                            res.AddErrorMessage(ex);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return Json(res);
                        }
                    }
                }

                #endregion

                ValidateProduct(res, quotationBasic);
                if (res.IsError)
                    return Json(res);
                else
                    CheckMaintenanceDetailQuotationData(res, quotationBasic, maLst);
            }
            catch (ApplicationErrorException erx)
            {
                res.AddErrorMessage(erx);
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check quotation detail data in case of maintenance (step 2) is correct?
        /// </summary>
        /// <param name="quotationBasic"></param>
        /// <param name="maLst"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckMaintenanceDetailQuotationData_P2(QUS030_tbt_QuotationBasic_MA quotationBasic, List<doContractHeader> maLst)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                CheckMaintenanceDetailQuotationData(res, quotationBasic, maLst);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
        #region Maintenance Detail Methods

        /// <summary>
        /// Initial quotation detail data in case of maintenance
        /// </summary>
        /// <param name="doRentalContractData"></param>
        /// <returns></returns>
        private doInitRentalQuotationData InitMAQuotationData(dsRentalContractData doRentalContractData)
        {
            try
            {
                doInitRentalQuotationData initDo = new doInitRentalQuotationData();

                if (doRentalContractData != null)
                {
                    #region Create Quotation Basic

                    initDo.doQuotationBasic = new tbt_QuotationBasic();
                    if (doRentalContractData.dtTbt_RentalSecurityBasic != null)
                    {
                        if (doRentalContractData.dtTbt_RentalSecurityBasic.Count > 0)
                        {
                            initDo.doQuotationBasic.ProductCode = doRentalContractData.dtTbt_RentalSecurityBasic[0].ProductCode;
                            initDo.doQuotationBasic.ContractDurationMonth = doRentalContractData.dtTbt_RentalSecurityBasic[0].ContractDurationMonth;
                            initDo.doQuotationBasic.AutoRenewMonth = doRentalContractData.dtTbt_RentalSecurityBasic[0].AutoRenewMonth;
                            initDo.doQuotationBasic.LastOccNo = doRentalContractData.dtTbt_RentalSecurityBasic[0].OCC;

                            initDo.doQuotationBasic.MaintenanceCycle = doRentalContractData.dtTbt_RentalSecurityBasic[0].MaintenanceCycle;
                        }
                    }
                    if (doRentalContractData.dtTbt_RentalMaintenanceDetails != null)
                    {
                        if (doRentalContractData.dtTbt_RentalMaintenanceDetails.Count > 0)
                        {
                            initDo.doQuotationBasic.MaintenanceTargetProductTypeCode = doRentalContractData.dtTbt_RentalMaintenanceDetails[0].MaintenanceTargetProductTypeCode;
                            initDo.doQuotationBasic.MaintenanceTypeCode = doRentalContractData.dtTbt_RentalMaintenanceDetails[0].MaintenanceTypeCode;
                            initDo.doQuotationBasic.MaintenanceMemo = doRentalContractData.dtTbt_RentalMaintenanceDetails[0].MaintenanceMemo;
                        }
                    }

                    #endregion
                    #region Create Maintenance Target List

                    List<tbt_SaleBasic> cond = new List<tbt_SaleBasic>();
                    if (doRentalContractData.dtTbt_RelationType != null)
                    {
                        foreach (tbt_RelationType r in doRentalContractData.dtTbt_RelationType)
                        {
                            cond.Add(new tbt_SaleBasic()
                            {
                                ContractCode = r.RelatedContractCode
                            });
                        }
                    }

                    IContractHandler chandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                    initDo.MaintenanceTargetList = chandler.GetContractHeaderDataByLanguage(cond);

                    #endregion
                }

                return initDo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Initial quotation detail data in case of maintenance from import file
        /// </summary>
        /// <param name="res"></param>
        /// <param name="initData"></param>
        /// <param name="importData"></param>
        private void InitImportMAQuotation(ObjectResultData res, doInitQuotationData initData, dsImportData importData)
        {
            try
            {
                #region Create Quotation Basic

                if (importData.dtTbt_QuotationBasic != null)
                {
                    if (importData.dtTbt_QuotationBasic.Count > 0)
                    {
                        tbt_QuotationBasic nDo = importData.dtTbt_QuotationBasic[0];
                        if (initData.doQuotationBasic != null)
                        {
                            nDo.LastOccNo = initData.doQuotationBasic.LastOccNo;

                            if (initData.doQuotationBasic.ProductCode == nDo.ProductCode)
                            {
                                nDo.ContractDurationMonth = initData.doQuotationBasic.ContractDurationMonth;
                                nDo.AutoRenewMonth = initData.doQuotationBasic.AutoRenewMonth;
                            }
                        }

                        if (initData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE)
                            nDo.DepositFee = null;

                        initData.doQuotationBasic = nDo;
                    }
                }

                #endregion
                #region Prepare Maintenance Detail

                /* --- Merge --- */
                /* initData.MaintenanceTargetList = new List<doContractHeader>();
                if (importData.dtTbt_QuotationMaintenanceLinkage != null)
                {
                    CommonUtil cmm = new CommonUtil();
                    List<string> contractLst = new List<string>();
                    foreach (tbt_QuotationMaintenanceLinkage ml in importData.dtTbt_QuotationMaintenanceLinkage)
                    {
                        contractLst.Add(cmm.ConvertContractCode(ml.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                    }

                    IContractHandler chandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                    ObjectResultData ress = chandler.CheckMaintenanceTargetContractList(contractLst, initData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode); //TODO: Bug report QU-124 Add param quotation target code
                    if (ress.IsError == false)
                        initData.MaintenanceTargetList = (List<doContractHeader>)ress.ResultData;
                } */
                if (initData.doQuotationBasic.MaintenanceTargetProductTypeCode == MaintenanceTargetProductType.C_MA_TARGET_PROD_TYPE_SECOM)
                {
                    initData.MaintenanceTargetList = new List<doContractHeader>();
                    if (importData.dtTbt_QuotationMaintenanceLinkage != null)
                    {
                        CommonUtil cmm = new CommonUtil();
                        //List<string> contractLst = new List<string>();
                        //foreach (tbt_QuotationMaintenanceLinkage ml in importData.dtTbt_QuotationMaintenanceLinkage)
                        //{
                        //    contractLst.Add(cmm.ConvertContractCode(ml.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                        //}

                        //IContractHandler chandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                        //ObjectResultData ress = chandler.CheckMaintenanceTargetContractList(contractLst, initData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode); //TODO: Bug report QU-124 Add param quotation target code
                        //if (ress.IsError == false)
                        //    initData.MaintenanceTargetList = (List<doContractHeader>)ress.ResultData;

                        List<tbt_SaleBasic> lst = new List<tbt_SaleBasic>();
                        foreach (tbt_QuotationMaintenanceLinkage ml in importData.dtTbt_QuotationMaintenanceLinkage)
                        {
                            lst.Add(new tbt_SaleBasic()
                            {
                                ContractCode = cmm.ConvertContractCode(ml.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                            });
                        }

                        IContractHandler chandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                        initData.MaintenanceTargetList = chandler.GetContractHeaderDataByLanguage(lst);
                    }
                }
                /* ------------- */

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Check quotation detail data in case of maintenance is correct?
        /// </summary>
        /// <param name="res"></param>
        /// <param name="quotationBasic"></param>
        /// <param name="maLst"></param>
        private void CheckMaintenanceDetailQuotationData(ObjectResultData res, QUS030_tbt_QuotationBasic_MA quotationBasic, List<doContractHeader> maLst)
        {
            try
            {
                #region Get Session

                doInitQuotationData doInitData = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    doInitData = param.InitialData;

                #endregion

                doRegisterQuotationData doRegister = new doRegisterQuotationData();
                doRegister.doQuotationHeaderData = doInitData.doQuotationHeaderData;

                doRegister.doTbt_QuotationBasic = new tbt_QuotationBasic();
                doRegister.doTbt_QuotationBasic.ProductCode = quotationBasic.ProductCode;
                doRegister.doTbt_QuotationBasic.ContractTransferStatus =
                        SECOM_AJIS.Common.Util.ConstantValue.ContractTransferStatus.C_CONTRACT_TRANS_STATUS_QTN_REG;
                doRegister.doTbt_QuotationBasic.LockStatus = SECOM_AJIS.Common.Util.ConstantValue.LockStatus.C_LOCK_STATUS_UNLOCK;

                doRegister.doTbt_QuotationBasic.FireMonitorFlag = false;
                doRegister.doTbt_QuotationBasic.CrimePreventFlag = false;
                doRegister.doTbt_QuotationBasic.EmergencyReportFlag = false;
                doRegister.doTbt_QuotationBasic.FacilityMonitorFlag = false;
                doRegister.doTbt_QuotationBasic.BeatGuardFlag = false;
                doRegister.doTbt_QuotationBasic.SentryGuardFlag = false;
                doRegister.doTbt_QuotationBasic.MaintenanceFlag = true;
                doRegister.doTbt_QuotationBasic.QuotationNo = quotationBasic.QuotationNo;
                doRegister.doTbt_QuotationBasic.SalesmanEmpNo1 = quotationBasic.SalesmanEmpNo1;
                doRegister.doTbt_QuotationBasic.SalesmanEmpNo2 = quotationBasic.SalesmanEmpNo2;
                doRegister.doTbt_QuotationBasic.SalesSupporterEmpNo = quotationBasic.SalesSupporterEmpNo;

                doRegister.doTbt_QuotationBasic.MaintenanceFee1CurrencyType = quotationBasic.MaintenanceFee1CurrencyType;
                if (quotationBasic.MaintenanceFee1CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1Usd = quotationBasic.MaintenanceFee1;
                else
                    doRegister.doTbt_QuotationBasic.MaintenanceFee1 = quotationBasic.MaintenanceFee1;

                doRegister.doTbt_QuotationBasic.AdditionalFee1CurrencyType = quotationBasic.AdditionalFee1CurrencyType;
                if (quotationBasic.AdditionalFee1CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.AdditionalFee1Usd = quotationBasic.AdditionalFee1;
                else
                    doRegister.doTbt_QuotationBasic.AdditionalFee1 = quotationBasic.AdditionalFee1;

                doRegister.doTbt_QuotationBasic.AdditionalFee2CurrencyType = quotationBasic.AdditionalFee2CurrencyType;
                if (quotationBasic.AdditionalFee2CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.AdditionalFee2Usd = quotationBasic.AdditionalFee2;
                else
                    doRegister.doTbt_QuotationBasic.AdditionalFee2 = quotationBasic.AdditionalFee2;

                doRegister.doTbt_QuotationBasic.AdditionalFee3CurrencyType = quotationBasic.AdditionalFee3CurrencyType;
                if (quotationBasic.AdditionalFee3CurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.AdditionalFee3Usd = quotationBasic.AdditionalFee3;
                else
                    doRegister.doTbt_QuotationBasic.AdditionalFee3 = quotationBasic.AdditionalFee3;



                doRegister.doTbt_QuotationBasic.AdditionalApproveNo1 = quotationBasic.AdditionalApproveNo1;
                doRegister.doTbt_QuotationBasic.AdditionalApproveNo2 = quotationBasic.AdditionalApproveNo2;
                doRegister.doTbt_QuotationBasic.AdditionalApproveNo3 = quotationBasic.AdditionalApproveNo3;
                doRegister.doTbt_QuotationBasic.ApproveNo1 = quotationBasic.ApproveNo1;
                doRegister.doTbt_QuotationBasic.ApproveNo2 = quotationBasic.ApproveNo2;
                doRegister.doTbt_QuotationBasic.ApproveNo3 = quotationBasic.ApproveNo3;
                doRegister.doTbt_QuotationBasic.ApproveNo4 = quotationBasic.ApproveNo4;
                doRegister.doTbt_QuotationBasic.ApproveNo5 = quotationBasic.ApproveNo5;
                
                doRegister.doTbt_QuotationBasic.MaintenanceMemo = quotationBasic.MaintenanceMemo;
                doRegister.doTbt_QuotationBasic.MaintenanceTargetProductTypeCode = quotationBasic.MaintenanceTargetProductTypeCode;
                doRegister.doTbt_QuotationBasic.MaintenanceTypeCode = quotationBasic.MaintenanceTypeCode;
                doRegister.doTbt_QuotationBasic.MaintenanceCycle = quotationBasic.MaintenanceCycle;

                doRegister.doTbt_QuotationBasic.DepositFeeCurrencyType = quotationBasic.DepositFeeCurrencyType;
                if (quotationBasic.DepositFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.DepositFeeUsd = quotationBasic.DepositFee;
                else
                    doRegister.doTbt_QuotationBasic.DepositFee = quotationBasic.DepositFee;

                doRegister.doTbt_QuotationBasic.ContractFeeCurrencyType = quotationBasic.ContractFeeCurrencyType;
                if (quotationBasic.ContractFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    doRegister.doTbt_QuotationBasic.ContractFeeUsd = quotationBasic.ContractFee;
                else
                    doRegister.doTbt_QuotationBasic.ContractFee = quotationBasic.ContractFee;

                bool isQuotation = false;
                if (doInitData.doQuotationHeaderData != null)
                {
                    if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                    {
                        doRegister.doTbt_QuotationBasic.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;

                        if (doInitData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode
                            == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE)
                            isQuotation = true;
                    }
                }
                if (doInitData.doQuotationBasic != null)
                {
                    if (isQuotation == false)
                        doRegister.doTbt_QuotationBasic.LastOccNo = doInitData.doQuotationBasic.LastOccNo;

                    doRegister.doTbt_QuotationBasic.MaintenanceFee2 = doInitData.doQuotationBasic.MaintenanceFee2;
                }

                if (doInitData.PriorProductCode == quotationBasic.ProductCode)
                {
                    if (doInitData.doQuotationBasic != null)
                    {
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = doInitData.doQuotationBasic.ContractDurationMonth;
                        doRegister.doTbt_QuotationBasic.AutoRenewMonth = doInitData.doQuotationBasic.AutoRenewMonth;
                    }
                }
                else
                {
                    IProductMasterHandler phandler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    List<tbm_Product> pLst = phandler.GetTbm_Product(quotationBasic.ProductCode, null);
                    if (pLst.Count >= 0)
                        doRegister.doTbt_QuotationBasic.ContractDurationMonth = pLst[0].DepreciationPeriodContract;

                    doRegister.doTbt_QuotationBasic.AutoRenewMonth = SECOM_AJIS.Common.Util.ConstantValue.Quotation.C_DEFAULT_AUTO_RENEW_MONTH;
                }

                if (doInitData.doQuotationHeaderData != null && maLst != null)
                {
                    CommonUtil cmm = new CommonUtil();
                    foreach (doContractHeader ma in maLst)
                    {
                        if (doRegister.MaintenanceList == null)
                            doRegister.MaintenanceList = new List<tbt_QuotationMaintenanceLinkage>();

                        tbt_QuotationMaintenanceLinkage mal = new tbt_QuotationMaintenanceLinkage();
                        doRegister.MaintenanceList.Add(mal);

                        if (doInitData.doQuotationHeaderData.doQuotationTarget != null)
                            mal.QuotationTargetCode = doInitData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode;
                        mal.ContractCode = cmm.ConvertContractCode(ma.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }
                }

                #region Update Session

                param.RegisterData = doRegister;
                QUS030_ScreenData = param;

                #endregion

                res.ResultData = doRegister;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Maintenance Detail Section

        /// <summary>
        /// Load maintenance detail data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_GetMaintenanceDetailData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doContractHeader> lst = new List<doContractHeader>();

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    lst = param.InitialData.MaintenanceTargetList;
                    if (lst != null)
                    {
                        lst = (from x in lst
                               orderby x.ContractCode
                               select x).ToList();
                    }
                }
                #endregion

                res.ResultData = CommonUtil.ConvertToXml<doContractHeader>(lst, "Quotation\\QUS030_MaintenanceDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check maintenance data that fill from screen is correct?
        /// </summary>
        /// <param name="doMaintenanceDetail"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckBeforeAddMaintenanceDetail(QUS030_AddMaintenanceDetailDataCondition doMaintenanceDetail)
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

                bool hasSaleContract = false;
                if (doMaintenanceDetail.MaintenanceList != null)
                {
                    foreach (View_doContractHeader cont in doMaintenanceDetail.MaintenanceList)
                    {
                        if (doMaintenanceDetail.MaintenanceTargetContractCode.ToUpper() == cont.ContractCode.ToUpper())
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2045,
                                            null,
                                            new string[] { "MaintenanceTargetContractCode" });
                            return Json(res);
                        }
                        if (cont.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                            || cont.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                            || cont.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0032,
                                            null,
                                            new string[] { "MaintenanceTargetContractCode" });
                            return Json(res);
                        }
                        if (cont.ProductTypeCode == ProductType.C_PROD_TYPE_SALE)
                            hasSaleContract = true;
                    }
                }

                CommonUtil cmm = new CommonUtil();
                string codeLong = cmm.ConvertContractCode(doMaintenanceDetail.MaintenanceTargetContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IContractHandler chandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                if (hasSaleContract)
                {
                    List<dtGetMaintenanceTargetContract> mLst = chandler.GetMaintenanceTargetContract(codeLong, true);
                    if (mLst.Count > 0)
                    {
                        if (mLst[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL
                            || mLst[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                            || mLst[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0132,
                                            null,
                                            new string[] { "MaintenanceTargetContractCode" });
                            return Json(res);
                        }
                    }
                }

                QUS030_ScreenParameter param = QUS030_ScreenData;
                res.ResultData = chandler.CheckMaintenanceTargetContract(codeLong, param.InitialData.doQuotationHeaderData.doQuotationTarget.QuotationTargetCode); //TODO: Bug report QU-124 Add param quotation target code

            }
            catch (ApplicationErrorException erx)
            {
                res.AddErrorMessage(erx);
                if (erx.ErrorResult != null)
                {
                    if (erx.ErrorResult.MessageList != null)
                    {
                        foreach (MessageModel m in erx.ErrorResult.MessageList)
                        {
                            if (m.Code != MessageUtil.MessageList.MSG0105.ToString())
                            {
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                break;
                            }
                        }
                    }
                }

                if (res.MessageList != null)
                {
                    foreach (MessageModel m in res.MessageList)
                    {
                        m.Controls = new string[] { "MaintenanceTargetContractCode" };
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Instrument 01 Section

        /// <summary>
        /// Load instrument detail data in case of sale or alarm (before 1st complete installation) to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_GetInstrumentDetail01Data()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doInstrumentDetail> lst = null;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.InitialData != null)
                    {
                        lst = param.InitialData.InstrumentDetailList;
                        if (lst != null)
                        {
                            if (param.InitialData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_SALE)
                            {
                                lst = (
                                        from x in lst
                                        orderby x.InstrumentFlag descending, x.InstrumentCode
                                        select x).ToList();
                            }
                            else
                            {
                                lst = (
                                        from x in lst
                                        orderby x.ControllerFlag descending, x.InstrumentCode
                                        select x).ToList();
                            }
                        }
                    }
                }

                #endregion

                if (lst == null)
                    lst = new List<doInstrumentDetail>();

                res.ResultData = CommonUtil.ConvertToXml<doInstrumentDetail>(lst, "Quotation\\QUS030_InstrumentDetail_01", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Load default instrument detail data in case of sale or alarm (before 1st complete installation) (step 1) to grid
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult QUS030_GetDefaultInstrumentDetail01Data_P1(QUS030_GetInstrumentDataCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                #region Get Session

                string ProductTypeCode = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param == null)
                    param = new QUS030_ScreenParameter();
                if (param.InitialData != null)
                    ProductTypeCode = param.InitialData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode;

                #endregion

                doDefaultInstrumentCondition diCond = new doDefaultInstrumentCondition()
                {
                    ProductCode = cond.ProductCode,
                    ProductTypeCode = ProductTypeCode
                };

                /* --- Merge --- */
                if (ProductTypeCode == ProductType.C_PROD_TYPE_SALE)
                    diCond.SaleFlag = true;
                else
                    diCond.RentalFlag = true;
                /* ------------- */

                IQuotationHandler handler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                List<doDefaultInstrument> lst = handler.GetDefaultInstrument(diCond);
                if (lst.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2046);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                }
                else
                {
                    if (cond.IsAskQuestion)
                    {
                        #region Update Session

                        param.DefaultInstrument = lst;
                        QUS030_ScreenData = param;

                        #endregion

                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2047);
                    }
                    else
                    {
                        res.ResultData = CommonUtil.ConvertToXml<doDefaultInstrument>(lst, "Quotation\\QUS030_InstrumentDetail_01", CommonUtil.GRID_EMPTY_TYPE.INSERT);
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
        /// Load default instrument detail data in case of sale or alarm (before 1st complete installation) (step 2) to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_GetDefaultInstrumentDetail01Data_P2()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doDefaultInstrument> lst = null;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    lst = param.DefaultInstrument;

                #endregion

                if (lst == null)
                    lst = new List<doDefaultInstrument>();
                res.ResultData = CommonUtil.ConvertToXml<doDefaultInstrument>(lst, "Quotation\\QUS030_InstrumentDetail_01", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Validate instrument detail data in case of sale or alarm (before 1st complete installation) is correct?
        /// </summary>
        /// <param name="res"></param>
        /// <param name="quotationBasic"></param>
        /// <param name="instLst"></param>
        private void ValidateInstrumentDetail01(ObjectResultData res, tbt_QuotationBasic quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> instLst)
        {
            try
            {
                if (instLst != null)
                {
                    #region Get Session

                    doQuotationTarget qt = null;
                    QUS030_ScreenParameter param = QUS030_ScreenData;
                    if (param != null)
                    {
                        if (param.InitialData != null)
                            qt = param.InitialData.doQuotationHeaderData.doQuotationTarget;
                    }

                    #endregion

                    //IProductMasterHandler handler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    //List<tbm_ProductInstrument> pLst = handler.GetTbm_ProductInstrument(quotationBasic.ProductCode);
                    //if (pLst != null)
                    //{
                    //    if (pLst.Count <= 0)
                    //        pLst = null;
                    //}

                    /* --- Merge --- */
                    /* bool needAR = false;
                    bool isAllQty0 = true;
                    string correctType_InstCode = null;
                    string lineType_InstCode = null;
                    //string mappingProdInst = null;
                    string notDef0_InstCode = null;
                    string notDef0_InstIdx = null;
                    for (int idx = 0; idx < instLst.Count; idx++)
                    {
                        QUS030_tbt_QuotationInstrumentDetails inst = instLst[idx];
                        if (CommonUtil.IsNullOrEmpty(inst.InstrumentQty)
                            || inst.InstrumentQty <= 0)
                        {
                            if (notDef0_InstCode == null
                                && inst.IsDefaultFlag == false)
                            {
                                notDef0_InstCode = inst.InstrumentCode;
                                notDef0_InstIdx = idx.ToString();
                            }
                        }
                        else
                            isAllQty0 = false;

                        if (qt.ProductTypeCode == ProductType.C_PROD_TYPE_SALE)
                        {
                            if (inst.SaleFlag == false)
                                correctType_InstCode = inst.InstrumentCode;
                        }
                        else if (qt.ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                        {
                            if (inst.RentalFlag == false)
                                correctType_InstCode = inst.InstrumentCode;
                        }

                        if (inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE
                            || inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                        {
                            if (lineType_InstCode == null)
                                lineType_InstCode = inst.InstrumentCode;
                        }
                        else
                        {
                            if (inst.LineUpTypeCode == SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_ONE_TIME
                            || inst.LineUpTypeCode == SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_TEMPORARY)
                            {
                                needAR = true;
                            }
                        }

                        //if (mappingProdInst == null && pLst != null)
                        //{
                        //    mappingProdInst = inst.InstrumentCode;
                        //    foreach (tbm_ProductInstrument p in pLst)
                        //    {
                        //        if (p.InstrumentCode.ToUpper() == inst.InstrumentCode.ToUpper())
                        //        {
                        //            mappingProdInst = null;
                        //            break;
                        //        }
                        //    }
                        //}
                    }

                    if (isAllQty0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2062,
                                            null,
                                            new string[] { string.Format("InstrumentDetail;InstQuantity;{0}", "ALL") });
                    }
                    else if (notDef0_InstCode != null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2065,
                                            new string[] { notDef0_InstCode },
                                            new string[] { string.Format("InstrumentDetail;InstQuantity;{0}", notDef0_InstIdx) });
                    }
                    else if (correctType_InstCode != null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0085,
                                            new string[] { correctType_InstCode });
                    }
                    else if (lineType_InstCode != null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0086,
                                            new string[] { lineType_InstCode });
                    }
                    else if (CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo1)
                        && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo2)
                        && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo3)
                        && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo4)
                        && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo5)
                        && needAR)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2064,
                                            null,
                                            new string[] { "ApproveNo1" });
                    }
                    //else if (qt.ProductTypeCode != ProductType.C_PROD_TYPE_SALE && mappingProdInst != null)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2069,
                    //                        new string[] { mappingProdInst, quotationBasic.ProductCode });
                    //} */

                    List<tbm_Instrument> iLst = new List<tbm_Instrument>();
                    foreach (QUS030_tbt_QuotationInstrumentDetails i in instLst)
                    {
                        iLst.Add(new tbm_Instrument() { InstrumentCode = i.InstrumentCode });
                    }

                    IInstrumentMasterHandler handler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<tbm_Instrument> mLst = handler.GetIntrumentList(iLst);

                    bool needAR = false;
                    bool isAllQty0 = true;
                    string notDef0_InstCode = null;
                    string notDef0_InstIdx = null;

                    string instErrorCode = null;
                    MessageUtil.MessageList instErrorMessage = MessageUtil.MessageList.MSG0082;

                    for (int idx = 0; idx < instLst.Count; idx++)
                    {
                        QUS030_tbt_QuotationInstrumentDetails inst = instLst[idx];
                        if (CommonUtil.IsNullOrEmpty(inst.InstrumentQty)
                            || inst.InstrumentQty <= 0)
                        {
                            if (notDef0_InstCode == null
                                && inst.IsDefaultFlag == false)
                            {
                                notDef0_InstCode = inst.InstrumentCode;
                                notDef0_InstIdx = idx.ToString();
                            }
                        }
                        else
                            isAllQty0 = false;

                        if (instErrorCode == null)
                        {
                            instErrorCode = inst.InstrumentCode;
                            if (mLst.Count > 0)
                            {
                                foreach (tbm_Instrument m in mLst)
                                {
                                    if (m.InstrumentCode.ToUpper().Trim() == inst.InstrumentCode.ToUpper().Trim())
                                    {
                                        if (m.InstrumentTypeCode != InstrumentType.C_INST_TYPE_GENERAL)
                                            instErrorMessage = MessageUtil.MessageList.MSG0014;
                                        else if (m.ExpansionTypeCode != ExpansionType.C_EXPANSION_TYPE_PARENT)
                                            instErrorMessage = MessageUtil.MessageList.MSG0015;
                                        else if (qt.ProductTypeCode == ProductType.C_PROD_TYPE_SALE && m.SaleFlag != true)
                                            instErrorMessage = MessageUtil.MessageList.MSG0016;
                                        else if (qt.ProductTypeCode != ProductType.C_PROD_TYPE_SALE && m.RentalFlag != true)
                                            instErrorMessage = MessageUtil.MessageList.MSG0085;
                                        else if (m.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                                        {
                                            instErrorMessage = MessageUtil.MessageList.MSG0086;
                                            instErrorCode = inst.InstrumentCode;
                                        }
                                        else if (m.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE)
                                        {
                                            if (CheckUserPermission(ScreenID.C_SCREEN_ID_QTN_DETAIL, FunctionID.C_FUNC_ID_PLANNER) == false)
                                            {
                                                instErrorMessage = MessageUtil.MessageList.MSG0045;
                                                instErrorCode = inst.InstrumentCode;
                                            }
                                            else
                                            {
                                                instErrorCode = null;
                                            }
                                        }
                                        else
                                        {
                                            instErrorCode = null;

                                            if (!CommonUtil.IsNullOrEmpty(inst.InstrumentQty) && inst.InstrumentQty > 0)
                                            {
                                                if (m.LineUpTypeCode == SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_ONE_TIME
                                                    || m.LineUpTypeCode == SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_TEMPORARY)
                                                {
                                                    needAR = true;
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (isAllQty0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2062,
                                            null,
                                            new string[] { string.Format("InstrumentDetail;InstQuantity;{0}", "ALL") });
                    }
                    else if (notDef0_InstCode != null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2065,
                                            new string[] { notDef0_InstCode },
                                            new string[] { string.Format("InstrumentDetail;InstQuantity;{0}", notDef0_InstIdx) });
                    }
                    else if (instErrorCode != null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, instErrorMessage,
                                            new string[] { instErrorCode });
                    }
                    else if (CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo1)
                        && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo2)
                        && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo3)
                        && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo4)
                        && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo5)
                        && needAR)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2064,
                                            null,
                                            new string[] { "ApproveNo1" });
                    }
                    /* ------------- */

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Instrument 02 Section

        /// <summary>
        /// Load instrument detail data in case of alarm (after 1st complete installation) to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_GetInstrumentDetail02Data()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                List<doInstrumentDetail> lst = null;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.InitialData != null)
                    {
                        lst = param.InitialData.InstrumentDetailList;
                        if (lst != null)
                        {
                            lst = (
                                        from x in lst
                                        orderby x.ControllerFlag descending, x.InstrumentCode
                                        select x).ToList();
                        }
                    }
                }

                #endregion

                if (lst == null)
                    lst = new List<doInstrumentDetail>();
                res.ResultData = CommonUtil.ConvertToXml<doInstrumentDetail>(lst, "Quotation\\QUS030_InstrumentDetail_02", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Validate instrument detail data in case of alarm (after 1st complete installation) is correct?
        /// </summary>
        /// <param name="res"></param>
        /// <param name="quotationBasic"></param>
        /// <param name="instLst"></param>
        private void ValidateInstrumentDetail02(ObjectResultData res, tbt_QuotationBasic quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> instLst)
        {
            try
            {
                if (instLst != null)
                {
                    #region Get Session

                    doQuotationTarget qt = null;
                    List<doInstrumentDetail> priorLst = null;
                    QUS030_ScreenParameter param = QUS030_ScreenData;
                    if (param != null)
                    {
                        if (param.InitialData != null)
                        {
                            priorLst = param.InitialData.PriorInstrumentDetailList;
                            qt = param.InitialData.doQuotationHeaderData.doQuotationTarget;
                        }
                    }

                    #endregion

                    int total_install = 0;
                    foreach (QUS030_tbt_QuotationInstrumentDetails inst in instLst)
                    {
                        if (inst.InstrumentQty.HasValue)
                            total_install += inst.InstrumentQty.Value;
                        if (inst.AddQty.HasValue)
                            total_install += inst.AddQty.Value;
                        if (inst.RemoveQty.HasValue)
                            total_install -= inst.RemoveQty.Value;
                    }
                    if (total_install <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2059);
                        return;
                    }

                    if (priorLst != null)
                    {
                        int error_idx = 0;
                        string instCode_Code = null;
                        string instCode_Qty = null;
                        string instCode_rQty = null;

                        int error_rIdx = 0;
                        foreach (doInstrumentDetail pinst in priorLst)
                        {
                            bool isSameCode = false;
                            bool isSameQty = false;
                            foreach (QUS030_tbt_QuotationInstrumentDetails inst in instLst)
                            {
                                /* --- Merge --- */
                                /* if (inst.InstrumentCode == pinst.InstrumentCode) */
                                if (inst.InstrumentCode.ToUpper().Trim() == pinst.InstrumentCode.ToUpper().Trim())
                                /* ------------- */
                                {
                                    isSameCode = true;
                                    inst.IsDefaultFlag = true;
                                    if (inst.InstrumentQty == pinst.InstrumentQty)
                                    {
                                        isSameQty = true;

                                        if (instCode_rQty == null && inst.RemoveQty > inst.InstrumentQty)
                                        {
                                            instCode_rQty = inst.InstrumentCode;
                                            error_rIdx = error_idx;
                                        }
                                    }
                                    break;
                                }
                            }

                            if (isSameCode == false)
                            {
                                instCode_Code = pinst.InstrumentCode;
                                break;
                            }

                            if (instCode_Qty == null && isSameQty == false)
                                instCode_Qty = pinst.InstrumentCode;
                            else
                                error_idx++;
                        }

                        if (instCode_Code != null)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2092,
                                        new string[] { instCode_Code },
                                        new string[] { string.Format("InstrumentDetail;ChangeQuantityAdditional;{0}", error_idx) });
                            return;
                        }
                        else if (instCode_Qty != null)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2027,
                                        new string[] { instCode_Qty },
                                        new string[] { string.Format("InstrumentDetail;ChangeQuantityAdditional;{0}", error_idx) });
                            return;
                        }
                        else if (instCode_rQty != null)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2067,
                                            new string[] { instCode_rQty },
                                            new string[] { string.Format("InstrumentDetail;ChangeQuantityAdditional;{0}", error_rIdx) });
                            return;
                        }
                    }

                    /* --- Merge --- */
                    /* //IProductMasterHandler handler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    //List<tbm_ProductInstrument> pLst = handler.GetTbm_ProductInstrument(quotationBasic.ProductCode);
                    //if (pLst != null)
                    //{
                    //    if (pLst.Count <= 0)
                    //        pLst = null;
                    //}

                    for (int idx = 0; idx < instLst.Count; idx++)
                    {
                        QUS030_tbt_QuotationInstrumentDetails inst = instLst[idx];
                        if (inst.IsDefaultFlag == true)
                            continue;

                        #region MSG0085

                        if (qt.ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                        {
                            if (inst.RentalFlag == false)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0085,
                                                new string[] { inst.InstrumentCode },
                                                new string[] { string.Format("InstrumentDetail;ChangeQuantityAdditional;{0}", idx) });
                                return;
                            }
                        }

                        #endregion
                        #region MSG0086

                        if (inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE
                            || inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0086,
                                            new string[] { inst.InstrumentCode },
                                            new string[] { string.Format("InstrumentDetail;ChangeQuantityAdditional;{0}", idx) });
                            return;
                        }

                        #endregion
                        #region MSG2032

                        if (inst.InstrumentQty != 0 || inst.RemoveQty != 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2032,
                                            new string[] { inst.InstrumentCode },
                                            new string[] { string.Format("InstrumentDetail;ChangeQuantityAdditional;{0}", idx) });
                            return;
                        }

                        #endregion
                        #region MSG2066

                        if (CommonUtil.IsNullOrEmpty(inst.AddQty)
                            || inst.AddQty <= 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2066,
                                            new string[] { inst.InstrumentCode },
                                            new string[] { string.Format("InstrumentDetail;ChangeQuantityAdditional;{0}", idx) });
                            return;
                        }

                        #endregion
                        #region MSG2064

                        if (inst.LineUpTypeCode == SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_ONE_TIME
                            || inst.LineUpTypeCode == SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_TEMPORARY)
                        {
                            if (CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo1)
                                && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo2)
                                && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo3)
                                && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo4)
                                && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo5))
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2064,
                                                    null,
                                                    new string[] { "ApproveNo1" });
                                return;
                            }
                        }

                        #endregion
                        //#region MSG2069

                        //if (pLst != null)
                        //{
                        //    bool isFound = false;
                        //    foreach (tbm_ProductInstrument p in pLst)
                        //    {
                        //        if (p.InstrumentCode.ToUpper() == inst.InstrumentCode.ToUpper())
                        //        {
                        //            isFound = true;
                        //            break;
                        //        }
                        //    }
                        //    if (isFound == false)
                        //    {
                        //        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2069,
                        //                new string[] { inst.InstrumentCode, quotationBasic.ProductCode });
                        //        return;
                        //    }
                        //}

                        //#endregion
                    } */
                    List<tbm_Instrument> iLst = new List<tbm_Instrument>();
                    foreach (QUS030_tbt_QuotationInstrumentDetails i in instLst)
                    {
                        iLst.Add(new tbm_Instrument() { InstrumentCode = i.InstrumentCode });
                    }

                    IInstrumentMasterHandler handler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<tbm_Instrument> mLst = handler.GetIntrumentList(iLst);

                    for (int idx = 0; idx < instLst.Count; idx++)
                    {
                        QUS030_tbt_QuotationInstrumentDetails inst = instLst[idx];
                        if (inst.IsDefaultFlag == true)
                            continue;

                        bool needAR = false;
                        string instErrorCode = inst.InstrumentCode;
                        MessageUtil.MessageList instErrorMessage = MessageUtil.MessageList.MSG0082;
                        if (mLst.Count > 0)
                        {
                            foreach (tbm_Instrument m in mLst)
                            {
                                if (m.InstrumentCode.ToUpper().Trim() == inst.InstrumentCode.ToUpper().Trim())
                                {
                                    if (m.InstrumentTypeCode != InstrumentType.C_INST_TYPE_GENERAL)
                                        instErrorMessage = MessageUtil.MessageList.MSG0014;
                                    else if (m.ExpansionTypeCode != ExpansionType.C_EXPANSION_TYPE_PARENT)
                                        instErrorMessage = MessageUtil.MessageList.MSG0015;
                                    else if (qt.ProductTypeCode == ProductType.C_PROD_TYPE_SALE && m.SaleFlag != true)
                                        instErrorMessage = MessageUtil.MessageList.MSG0016;
                                    else if (qt.ProductTypeCode != ProductType.C_PROD_TYPE_SALE && m.RentalFlag != true)
                                        instErrorMessage = MessageUtil.MessageList.MSG0085;
                                    else if (m.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                                    {
                                        instErrorMessage = MessageUtil.MessageList.MSG0086;
                                        instErrorCode = inst.InstrumentCode;
                                    }
                                    else if (m.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE)
                                    {
                                        if (CheckUserPermission(ScreenID.C_SCREEN_ID_QTN_DETAIL, FunctionID.C_FUNC_ID_PLANNER) == false)
                                        {
                                            instErrorMessage = MessageUtil.MessageList.MSG0045;
                                            instErrorCode = inst.InstrumentCode;
                                        }
                                        else
                                        {
                                            instErrorCode = null;
                                        }
                                    }
                                    else
                                    {
                                        instErrorCode = null;

                                        if (!CommonUtil.IsNullOrEmpty(inst.InstrumentQty) && inst.InstrumentQty > 0)
                                        {
                                            if (m.LineUpTypeCode == SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_ONE_TIME
                                                || m.LineUpTypeCode == SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_TEMPORARY)
                                            {
                                                needAR = true;
                                            }
                                        }
                                    }

                                    break;
                                }
                            }
                        }

                        if (instErrorCode != null)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, instErrorMessage,
                                                new string[] { instErrorCode },
                                                new string[] { string.Format("InstrumentDetail;ChangeQuantityAdditional;{0}", idx) });
                            return;
                        }

                        #region MSG2032

                        if (inst.InstrumentQty != 0 || inst.RemoveQty != 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2032,
                                            new string[] { inst.InstrumentCode },
                                            new string[] { string.Format("InstrumentDetail;ChangeQuantityAdditional;{0}", idx) });
                            return;
                        }

                        #endregion
                        #region MSG2066

                        if (CommonUtil.IsNullOrEmpty(inst.AddQty)
                            || inst.AddQty <= 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2066,
                                            new string[] { inst.InstrumentCode },
                                            new string[] { string.Format("InstrumentDetail;ChangeQuantityAdditional;{0}", idx) });
                            return;
                        }

                        #endregion
                        #region MSG2064

                        if (needAR)
                        {
                            if (CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo1)
                                && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo2)
                                && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo3)
                                && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo4)
                                && CommonUtil.IsNullOrEmpty(quotationBasic.ApproveNo5))
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2064,
                                                    null,
                                                    new string[] { "ApproveNo1" });
                                return;
                            }
                        }

                        #endregion
                    }
                    /* ------------- */


                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Instrument Section

        /// <summary>
        /// Get instrument data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult QUS030_GetInstrumentDetailInfo(QUS030_GetInstrumentDataCondition cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                #region Get Session

                string ProductTypeCode = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param == null)
                    param = new QUS030_ScreenParameter();
                if (param.InitialData != null)
                    ProductTypeCode = param.InitialData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode;

                #endregion

                doInstrumentSearchCondition dCond = new doInstrumentSearchCondition()
                {
                    InstrumentCode = CommonUtil.IsNullOrEmpty(cond.InstrumentCode) ? null : cond.InstrumentCode.Trim()
                };

                /* --- Merge --- */
                /* if (ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SALE)
                    dCond.SaleFlag = 1;
                if (ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL)
                    dCond.RentalFlag = 1;
                dCond.ExpansionType = new List<string>() { SECOM_AJIS.Common.Util.ConstantValue.ExpansionType.C_EXPANSION_TYPE_PARENT };
                dCond.InstrumentType = new List<string>() { SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_GENERAL }; */
                /* ------------- */


                IInstrumentMasterHandler handler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<doInstrumentData> lst = handler.GetInstrumentDataForSearch(dCond);
                if (lst != null)
                {
                    if (lst.Count > 0)
                        res.ResultData = lst[0];
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check instrument data that fill from screen is correct?
        /// </summary>
        /// <param name="doInstrument"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckBeforeAddInstrument(QUS030_AddInstrumentData doInstrument)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (ModelState.IsValid == false)
                {
                    MessageParameter mp = new MessageParameter();
                    mp.AddParameter(MessageUtil.MessageList.MSG0082, doInstrument.InstrumentCode);
                    ValidatorUtil.BuildErrorMessage(res, this, null, mp);
                    if (res.IsError)
                    {
                        if (res.MessageList != null)
                        {
                            foreach (MessageModel msg in res.MessageList)
                            {
                                if (msg.Code == MessageUtil.MessageList.MSG0082.ToString())
                                {
                                    /* --- Merge --- */
                                    /* res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK; */
                                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                    /* ------------- */

                                    break;
                                }
                            }
                        }
                        return Json(res);
                    }
                }

                #region Get Session

                string ProductTypeCode = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.InitialData != null)
                        ProductTypeCode = param.InitialData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode;
                }

                #endregion

                if (doInstrument.InstrumentList != null)
                {
                    foreach (doInstrumentDetail inst in doInstrument.InstrumentList)
                    {
                        /* --- Merge --- */
                        /* if (doInstrument.InstrumentCode.ToUpper() == inst.InstrumentCode.ToUpper()) */
                        if (doInstrument.InstrumentCode.ToUpper().Trim() == inst.InstrumentCode.ToUpper().Trim())
                        /* ------------- */
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0083,
                                new string[] { doInstrument.InstrumentCode }, new string[] { "InstrumentCode" });
                            return Json(res);
                        }
                    }
                }

                /* --- Merge --- */
                /* if ((ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SALE
                        && doInstrument.doInstrumentData.SaleFlag == false) ||
                        (ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL
                        && doInstrument.doInstrumentData.RentalFlag == false))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0085,
                                 new string[] { doInstrument.InstrumentCode }, new string[] { "InstrumentCode" });
                    return Json(res);
                } */
                bool isError = true;
                if (CommonUtil.IsNullOrEmpty(doInstrument.InstrumentQty) == false)
                {
                    if (doInstrument.InstrumentQty > 0)
                        isError = false;
                }
                if (isError)
                {
                    res.AddErrorMessage(
                                MessageUtil.MODULE_COMMON,
                                MessageUtil.MessageList.MSG0084,
                                null,
                                new string[] { "InstrumentQty" });
                    return Json(res);
                }

                if (doInstrument.doInstrumentData.InstrumentTypeCode != InstrumentType.C_INST_TYPE_GENERAL)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0014,
                                 new string[] { doInstrument.InstrumentCode }, new string[] { "InstrumentCode" });
                    return Json(res);
                }
                if (doInstrument.doInstrumentData.ExpansionTypeCode != ExpansionType.C_EXPANSION_TYPE_PARENT)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0015,
                                 new string[] { doInstrument.InstrumentCode }, new string[] { "InstrumentCode" });
                    return Json(res);
                }

                if (ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SALE)
                {
                    if (doInstrument.doInstrumentData.SaleFlag != true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0016,
                                 new string[] { doInstrument.InstrumentCode }, new string[] { "InstrumentCode" });
                        return Json(res);
                    }
                }
                else if (doInstrument.doInstrumentData.RentalFlag != true)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0085,
                                 new string[] { doInstrument.InstrumentCode }, new string[] { "InstrumentCode" });
                    return Json(res);
                }
                /* ------------- */

                if (doInstrument.doInstrumentData.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0086,
                               new string[] { doInstrument.InstrumentCode }, new string[] { "InstrumentCode" });
                    return Json(res);
                }
                else if (doInstrument.doInstrumentData.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE)
                {
                    if (CheckUserPermission(ScreenID.C_SCREEN_ID_QTN_DETAIL, FunctionID.C_FUNC_ID_PLANNER) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0045,
                               new string[] { doInstrument.InstrumentCode }, new string[] { "InstrumentCode" });
                        return Json(res);
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

        #endregion

        #region Instrument ONLINE Section

        /// <summary>
        /// Load instrument detail data in case of sale online to grid
        /// </summary>
        /// <param name="fromContract"></param>
        /// <returns></returns>
        public ActionResult QUS030_GetInstrumentDetailData_ONLINE(bool fromContract = false)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doInstrumentDetail> lst = null;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.InitialData != null && fromContract == false)
                    {
                        if (param.InitialData.doLinkageSaleContractData != null)
                        {
                            if (param.InitialData.doLinkageSaleContractData.SaleInstrumentDetailList != null)
                                lst = param.InitialData.doLinkageSaleContractData.SaleInstrumentDetailList;
                        }
                    }
                    else if (param.LinkageSaleContractData != null && fromContract == true)
                    {
                        if (param.LinkageSaleContractData.SaleInstrumentDetailList != null)
                            lst = param.LinkageSaleContractData.SaleInstrumentDetailList;
                    }
                }

                #endregion

                if (lst == null)
                    lst = new List<doInstrumentDetail>();
                else
                {
                    if (lst.Count > 0)
                    {
                        InstrumentMappingList instMappingLst = new InstrumentMappingList();
                        instMappingLst.AddInstrument(lst.ToArray());

                        IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                        ihandler.InstrumentListMapping(instMappingLst);

                        lst = (
                        from x in lst
                        orderby x.ControllerFlag descending, x.InstrumentCode
                        select x).ToList();
                    }
                }
                res.ResultData = CommonUtil.ConvertToXml<doInstrumentDetail>(lst, "Quotation\\QUS030_InstrumentDetail_ONLINE");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Facility Section

        /// <summary>
        /// Load facility detail data
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_GetFacilityDetailData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doInstrumentDetail> lst = new List<doInstrumentDetail>();

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.InitialData != null)
                    {
                        if (param.InitialData.FacilityDetailList != null)
                        {
                            foreach (doFacilityDetail f in param.InitialData.FacilityDetailList)
                            {
                                lst.Add(new doInstrumentDetail()
                                {
                                    InstrumentCode = f.FacilityCode,
                                    InstrumentName = f.FacilityName,
                                    InstrumentQty = f.FacilityQty
                                });
                            }

                            lst = (from x in lst
                                   orderby x.InstrumentCode
                                   select x).ToList();
                        }
                    }
                }

                #endregion

                if (lst == null)
                    lst = new List<doInstrumentDetail>();
                res.ResultData = CommonUtil.ConvertToXml<doInstrumentDetail>(lst, "Quotation\\QUS030_FacilityDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Load default facility detail data (step 1) to grid
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult QUS030_GetDefaultFacilityDetailData_P1(QUS030_GetFacilityDataCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    return Json(res);
                }

                IQuotationHandler handler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

                doDefaultFacilityCondition dfCond = CommonUtil.CloneObject<QUS030_GetFacilityDataCondition, doDefaultFacilityCondition>(cond);
                List<doDefaultFacility> lst = handler.GetDefaultFacility(dfCond);
                if (lst.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2049);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                }
                else
                {
                    if (cond.IsAskQuestion)
                    {
                        #region Update Session

                        QUS030_ScreenParameter param = QUS030_ScreenData;
                        if (param == null)
                            param = new QUS030_ScreenParameter();

                        param.DefaultFacility = lst;
                        QUS030_ScreenData = param;

                        #endregion

                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2050);
                    }
                    else
                    {
                        res.ResultData = CommonUtil.ConvertToXml<doDefaultFacility>(lst, "Quotation\\QUS030_FacilityDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT);
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
        /// Load default facility detail data (step 2) to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_GetDefaultFacilityDetailData_P2()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doDefaultFacility> lst = null;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                    lst = param.DefaultFacility;

                #endregion

                if (lst == null)
                    lst = new List<doDefaultFacility>();
                res.ResultData = CommonUtil.ConvertToXml<doDefaultFacility>(lst, "Quotation\\QUS030_FacilityDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get facility data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult QUS030_GetFacilityDetailInfo(QUS030_GetInstrumentDataCondition cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                doInstrumentSearchCondition dCond = new doInstrumentSearchCondition()
                {
                    InstrumentCode = CommonUtil.IsNullOrEmpty(cond.InstrumentCode) ? null : cond.InstrumentCode.Trim()
                };

                /* --- Merge --- */
                /* dCond.InstrumentType = new List<string>() { SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_MONITOR }; */
                /* ------------- */

                IInstrumentMasterHandler handler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<doInstrumentData> lst = handler.GetInstrumentDataForSearch(dCond);
                if (lst != null)
                {
                    if (lst.Count > 0)
                        res.ResultData = lst[0];
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check facility data that fill from screen is correct?
        /// </summary>
        /// <param name="doFacility"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckBeforeAddFacility(QUS030_AddFacilityData doFacility)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (ModelState.IsValid == false)
                {
                    MessageParameter param = new MessageParameter();

                    /* --- Merge --- */
                    /* param.AddParameter(MessageUtil.MessageList.MSG2052, doFacility.FacilityCode); */
                    param.AddParameter(MessageUtil.MessageList.MSG0023, doFacility.FacilityCode);
                    /* ------------- */

                    ValidatorUtil.BuildErrorMessage(res, this, null, param);
                    if (res.IsError)
                    {
                        if (res.MessageList != null)
                        {
                            foreach (MessageModel msg in res.MessageList)
                            {
                                /* --- Merge --- */
                                /* if (msg.Code == MessageUtil.MessageList.MSG2052.ToString())
                                {
                                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                                    break;
                                } */
                                if (msg.Code == MessageUtil.MessageList.MSG0023.ToString())
                                {
                                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                    break;
                                }
                                /* ------------- */

                            }
                        }
                        return Json(res);
                    }
                }

                if (doFacility.FacilityList != null)
                {
                    foreach (doInstrumentDetail inst in doFacility.FacilityList)
                    {
                        /* --- Merge --- */
                        /* if (doFacility.FacilityCode.ToUpper() == inst.InstrumentCode.ToUpper())
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2053,
                                new string[] { doFacility.FacilityCode }, new string[] { "FacilityCode" });
                            return Json(res);
                        } */
                        if (doFacility.FacilityCode.ToUpper().Trim() == inst.InstrumentCode.ToUpper().Trim())
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0024,
                                new string[] { doFacility.FacilityCode }, new string[] { "FacilityCode" });
                            return Json(res);
                        }
                        /* ------------- */

                    }
                }

                /* --- Merge --- */
                bool isError = true;
                if (CommonUtil.IsNullOrEmpty(doFacility.FacilityQty) == false)
                {
                    if (doFacility.FacilityQty > 0)
                        isError = false;
                }
                if (isError)
                {
                    res.AddErrorMessage(
                                MessageUtil.MODULE_COMMON,
                                MessageUtil.MessageList.MSG0025,
                                null,
                                new string[] { "FacilityQty" });
                    return Json(res);
                }

                if (doFacility.doFacilityDetail.InstrumentTypeCode != InstrumentType.C_INST_TYPE_MONITOR)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0026,
                                new string[] { doFacility.FacilityCode }, new string[] { "FacilityCode" });
                    return Json(res);
                }
                /* ------------- */


                //if (doFacility.doFacilityDetail.LineUpTypeCode == SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_STOP_SALE
                //        || doFacility.doFacilityDetail.LineUpTypeCode == SECOM_AJIS.Common.Util.ConstantValue.LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2055,
                //                new string[] { doFacility.FacilityCode }, new string[] { "FacilityCode" });
                //    return Json(res);
                //}

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Validate facility data is correct?
        /// </summary>
        /// <param name="res"></param>
        /// <param name="quotationBasic"></param>
        /// <param name="instLst"></param>
        private void ValidateFacilityDetail(ObjectResultData res, tbt_QuotationBasic quotationBasic, List<QUS030_tbt_QuotationInstrumentDetails> instLst)
        {
            try
            {
                if (instLst != null)
                {
                    //IProductMasterHandler handler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                    //List<tbm_ProductFacility> pLst = handler.GetTbm_ProductFacility(quotationBasic.ProductCode);
                    //if (pLst != null)
                    //{
                    //    if (pLst.Count <= 0)
                    //        pLst = null;
                    //}


                    /* --- Merge --- */
                    /* List<doMonitoringInstrument> niLst = new List<doMonitoringInstrument>();
                    foreach(QUS030_tbt_QuotationInstrumentDetails inst in instLst)
                    {
                        niLst.Add(new doMonitoringInstrument()
                        {
                            InstrumentCode = inst.InstrumentCode
                        });
                    }
                    IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<doMonitoringInstrument> iLst = ihandler.GetMonitoringInstrumentList(niLst);

                    string notDef0_InstCode = null;
                    string notDef0_InstIdx = null;
                    //string mappingProdInst = null;
                    string masterInst = null;
                    for (int idx = 0; idx < instLst.Count; idx++)
                    {
                        QUS030_tbt_QuotationInstrumentDetails inst = instLst[idx];
                        if (CommonUtil.IsNullOrEmpty(inst.InstrumentQty)
                            || inst.InstrumentQty <= 0)
                        {
                            if (notDef0_InstCode == null
                                && inst.IsDefaultFlag == false)
                            {
                                notDef0_InstCode = inst.InstrumentCode;
                                notDef0_InstIdx = idx.ToString();
                            }
                        }

                        #region Check facility master data.

                        if (masterInst == null && iLst != null)
                        {
                            masterInst = inst.InstrumentCode;
                            foreach (doMonitoringInstrument i in iLst)
                            {
                                if (i.InstrumentCode.ToUpper() == inst.InstrumentCode.ToUpper())
                                {
                                    masterInst = null;
                                    break;
                                }
                            }
                        }

                        #endregion
                        #region Check relation between product and instrument

                        //if (mappingProdInst == null && pLst != null)
                        //{
                        //    mappingProdInst = inst.InstrumentCode;
                        //    foreach (tbm_ProductFacility p in pLst)
                        //    {
                        //        if (p.FacilityCode.ToUpper() == inst.InstrumentCode.ToUpper())
                        //        {
                        //            mappingProdInst = null;
                        //            break;
                        //        }
                        //    }
                        //}

                        #endregion
                    }

                    if (notDef0_InstCode != null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2071,
                                 new string[] { notDef0_InstCode },
                                 new string[] { string.Format("FacilityDetail;FacQuantity;{0}", notDef0_InstIdx) });
                    }
                    else if (masterInst != null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2052,
                                 new string[] { masterInst });
                    }
                    //else if (mappingProdInst != null)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2072,
                    //             new string[] { mappingProdInst, quotationBasic.ProductCode });
                    //} */
                    List<tbm_Instrument> niLst = new List<tbm_Instrument>();
                    foreach (QUS030_tbt_QuotationInstrumentDetails inst in instLst)
                    {
                        niLst.Add(new tbm_Instrument()
                        {
                            InstrumentCode = inst.InstrumentCode
                        });
                    }
                    IInstrumentMasterHandler ihandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<tbm_Instrument> iLst = ihandler.GetIntrumentList(niLst);

                    string notDef0_InstCode = null;
                    string notDef0_InstIdx = null;
                    //string mappingProdInst = null;
                    string masterInst = null;
                    string instTypeError = null;
                    for (int idx = 0; idx < instLst.Count; idx++)
                    {
                        QUS030_tbt_QuotationInstrumentDetails inst = instLst[idx];
                        if (CommonUtil.IsNullOrEmpty(inst.InstrumentQty)
                            || inst.InstrumentQty <= 0)
                        {
                            if (notDef0_InstCode == null
                                && inst.IsDefaultFlag == false)
                            {
                                notDef0_InstCode = inst.InstrumentCode;
                                notDef0_InstIdx = idx.ToString();
                            }
                        }

                        #region Check facility master data.

                        if (masterInst == null && iLst != null)
                        {
                            masterInst = inst.InstrumentCode;
                            foreach (tbm_Instrument i in iLst)
                            {
                                if (i.InstrumentCode.ToUpper().Trim() == inst.InstrumentCode.ToUpper().Trim())
                                {
                                    masterInst = null;
                                    if (i.InstrumentTypeCode != InstrumentType.C_INST_TYPE_MONITOR)
                                        instTypeError = inst.InstrumentCode;

                                    break;
                                }
                            }
                        }

                        #endregion
                    }

                    if (notDef0_InstCode != null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2071,
                                 new string[] { notDef0_InstCode },
                                 new string[] { string.Format("FacilityDetail;FacQuantity;{0}", notDef0_InstIdx) });
                    }
                    else if (masterInst != null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0023,
                                 new string[] { masterInst });
                    }
                    else if (instTypeError != null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0026,
                                 new string[] { instTypeError });
                    }
                    /* ------------- */

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Sentry Guard Section

        /// <summary>
        /// Load sentry guard detail data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS030_GetSentryGuardDetailData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doSentryGuardDetail> lst = null;

                #region Get Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.InitialData != null)
                        lst = param.InitialData.SentryGuardDetailList;
                }

                #endregion

                if (lst == null)
                    lst = new List<doSentryGuardDetail>();

                res.ResultData = CommonUtil.ConvertToXml<doSentryGuardDetail>(lst, "Quotation\\QUS030_SentryGuardDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get selected sentry guard from session
        /// </summary>
        /// <param name="RunningNo"></param>
        /// <returns></returns>
        public ActionResult QUS030_GetSentryGuard(int RunningNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                doSentryGuardDetail sg = null;

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.InitialData != null)
                    {
                        if (param.InitialData.SentryGuardDetailList != null)
                        {
                            if (RunningNo > 0 && RunningNo <= param.InitialData.SentryGuardDetailList.Count)
                                sg = param.InitialData.SentryGuardDetailList[RunningNo - 1];
                        }
                    }
                }

                res.ResultData = sg;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        public ActionResult QUS030_GetSentryGuards()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.InitialData != null)
                    {
                        res.ResultData = param.InitialData.SentryGuardDetailList;
                        return Json(res);
                    }
                }

                res.ResultData = null;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Check sentry guard data that fill from screen is correct?
        /// </summary>
        /// <param name="doSentryGuard"></param>
        /// <returns></returns>
        public ActionResult QUS030_CheckBeforeAddSentryGuard(QUS030_AddSentryGuardDetail doSentryGuard)
        {
            ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();
            doSentryGuard.Currencies = new List<doMiscTypeCode>(tmpCurrencies);
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

                //if (doSentryGuard.SecurityStartTime >= doSentryGuard.SecurityFinishTime)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_QUOTATION, MessageUtil.MessageList.MSG2056,
                //        null,
                //        new string[] { "SecurityFinishTime" });
                //    return Json(res);
                //}

                #region Update Session

                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param == null)
                    param = new QUS030_ScreenParameter();
                if (param.InitialData == null)
                    param.InitialData = new doInitQuotationData();
                if (param.InitialData.SentryGuardDetailList == null)
                    param.InitialData.SentryGuardDetailList = new List<doSentryGuardDetail>();

                if (doSentryGuard.UpdateMode == 0)
                {
                    doSentryGuard.RunningNo = param.InitialData.SentryGuardDetailList.Count + 1;
                    param.InitialData.SentryGuardDetailList.Add(doSentryGuard);
                }
                else if (doSentryGuard.RunningNo > 0
                        && doSentryGuard.RunningNo <= param.InitialData.SentryGuardDetailList.Count)
                {
                    param.InitialData.SentryGuardDetailList[doSentryGuard.RunningNo - 1] = doSentryGuard;
                }

                QUS030_ScreenData = param;

                #endregion

                res.ResultData = doSentryGuard;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Remove selected sentry guard from session
        /// </summary>
        /// <param name="RunningNo"></param>
        /// <returns></returns>
        public ActionResult QUS030_RemoveSentryGuard(int RunningNo)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                QUS030_ScreenParameter param = QUS030_ScreenData;
                if (param != null)
                {
                    if (param.InitialData != null)
                    {
                        if (param.InitialData.SentryGuardDetailList != null)
                        {
                            if (RunningNo <= param.InitialData.SentryGuardDetailList.Count)
                            {
                                param.InitialData.SentryGuardDetailList.RemoveAt(RunningNo - 1);
                                for (int idx = 0; idx < param.InitialData.SentryGuardDetailList.Count; idx++)
                                {
                                    param.InitialData.SentryGuardDetailList[idx].RunningNo = idx + 1;
                                }
                            }

                            QUS030_ScreenData = param;
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

        #endregion
    }
}
