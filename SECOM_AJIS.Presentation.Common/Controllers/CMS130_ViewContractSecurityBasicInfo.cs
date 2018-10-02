//*********************************
// Create by: Narupon W.
// Create date: /Jun/2011
// Update date: /Jun/2011
//*********************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Xml;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;

using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Common.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS130
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS130_Authority(CMS130_ScreenParameter param) // IN parameter: string strContractCode, string strOCC
        {

            ObjectResultData res = new ObjectResultData();

            // Check permission
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_SECURITY_BASIC, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            // Check parameter is OK ?
            if (CommonUtil.IsNullOrEmpty(param.strContractCode) == false)
            {
                param.ContractCode = param.strContractCode;
                param.OCC = param.strOCC;
            }
            else
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                return Json(res);
            }

            // Check exist data
            try
            {
                CommonUtil c = new CommonUtil();
                string ContractCode = c.ConvertContractCode(param.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                // Rental
                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                List<dtTbt_RentalContractBasicForView> dtRentalContract = handler.GetTbt_RentalContractBasicForView(ContractCode);

                if (dtRentalContract.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<CMS130_ScreenParameter>("CMS130", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS130
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS130")]
        public ActionResult CMS130()
        {
            string strContractCode = "";
            string strOCC = "";
            try
            {
                CMS130_ScreenParameter param = GetScreenObject<CMS130_ScreenParameter>();
                strContractCode = param.ContractCode;
                strOCC = param.OCC;
            }
            catch
            {
            }

            /* ----- Set grobal variable for javascript side ---- */
            ViewBag.strContractCode = strContractCode;

            CommonUtil c = new CommonUtil();

            strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            List<dtTbt_RentalContractBasicForView> vw_dtRentalContract = new List<dtTbt_RentalContractBasicForView>();
            List<dtTbt_RentalSecurityBasicForView> vw_dtRentalSecurity = new List<dtTbt_RentalSecurityBasicForView>();

            ViewBag.Currency = CommonValue.CURRENCY_UNIT;
            string dateFormat = "dd-MMM-yyyy";
            string numberFormat = "N0";
            string floatNumberFormat = "N2";

            // default ViewBag
            ViewBag.chkFire_monitoring = false;
            ViewBag.chkCrime_prevention = false;
            ViewBag.chkEmergency_report = false;
            ViewBag.chkFacility_monitoring = false;

            ViewBag.chkOut_of_regulation_document_usage_flag = false;

            try
            {

                ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                List<string> lsFieldNames = new List<string>();
                lsFieldNames.Add(MiscType.C_RENTAL_CHANGE_TYPE);
                lsFieldNames.Add(MiscType.C_CHANGE_NAME_REASON_TYPE);
                lsFieldNames.Add(MiscType.C_REASON_TYPE);
                lsFieldNames.Add(MiscType.C_STOP_CANCEL_REASON_TYPE);
                lsFieldNames.Add(MiscType.C_MA_TARGET_PROD_TYPE);
                lsFieldNames.Add(MiscType.C_MA_TYPE);
                lsFieldNames.Add(MiscType.C_MA_FEE_TYPE);
                lsFieldNames.Add(MiscType.C_NUM_OF_DATE);
                lsFieldNames.Add(MiscType.C_OPERATION_TYPE);
                lsFieldNames.Add(MiscType.C_INSURANCE_TYPE);
                lsFieldNames.Add(MiscType.C_DOC_AUDIT_RESULT);
                lsFieldNames.Add(MiscType.C_RENTAL_INSTALL_TYPE);
                lsFieldNames.Add(MiscType.C_CHANGE_REASON_TYPE);

                // Get Misc type
                List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

                // Rental
                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                List<dtTbt_RentalContractBasicForView> dtRentalContract = handler.GetTbt_RentalContractBasicForView(strContractCode);
                List<dtTbt_RentalSecurityBasicForView> dtRentalSecurity = new List<dtTbt_RentalSecurityBasicForView>();
                List<dtTbt_RentalOperationTypeListForView> dtRentalOperation = new List<dtTbt_RentalOperationTypeListForView>();
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                if (dtRentalContract.Count > 0)
                {
                    // Get related data
                    dtRentalSecurity = handler.GetTbt_RentalSecurityBasicForView(strContractCode, (CommonUtil.IsNullOrEmpty(strOCC) == false ? strOCC : dtRentalContract[0].LastOCC));
                    dtRentalOperation = handler.GetTbt_RentalOperationTypeListForView(strContractCode, (dtRentalSecurity.Count > 0 ? dtRentalSecurity[0].OCC : strOCC));

                    for (int i = 0; i < dtRentalSecurity.Count(); i++)
                    {
                        dtRentalSecurity[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }
                    
                    //Check Currency
                    if(dtRentalSecurity[0].NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].NormalContractFee = dtRentalSecurity[0].NormalContractFeeUsd;
                    }
                    if (dtRentalSecurity[0].OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].OrderContractFee = dtRentalSecurity[0].OrderContractFeeUsd;
                    }
                    if (dtRentalSecurity[0].NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].NormalInstallFee = dtRentalSecurity[0].NormalInstallFeeUsd;
                    }
                    if (dtRentalSecurity[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].OrderInstallFee = dtRentalSecurity[0].OrderInstallFeeUsd;
                    }
                    if (dtRentalSecurity[0].NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].NewBldMgmtCost = dtRentalSecurity[0].NewBldMgmtCostUsd;
                    }
                    if (dtRentalSecurity[0].ContractFeeOnStopCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].ContractFeeOnStop = dtRentalSecurity[0].ContractFeeOnStopUsd;
                    }
                    if (dtRentalSecurity[0].InsuranceCoverageAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].InsuranceCoverageAmount = dtRentalSecurity[0].InsuranceCoverageAmountUsd;
                    }
                    if (dtRentalSecurity[0].MonthlyInsuranceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].MonthlyInsuranceFee = dtRentalSecurity[0].MonthlyInsuranceFeeUsd;
                    }
                    if (dtRentalSecurity[0].MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].MaintenanceFee1 = dtRentalSecurity[0].MaintenanceFee1Usd;
                    }
                    if (dtRentalSecurity[0].AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].AdditionalFee1 = dtRentalSecurity[0].AdditionalFee1Usd;
                    }
                    if (dtRentalSecurity[0].AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].AdditionalFee2 = dtRentalSecurity[0].AdditionalFee2Usd;
                    }
                    if (dtRentalSecurity[0].AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].AdditionalFee3 = dtRentalSecurity[0].AdditionalFee3Usd;
                    }
                    if (dtRentalSecurity[0].OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].OrderInstallFee_ApproveContract = dtRentalSecurity[0].OrderInstallFee_ApproveContractUsd;
                    }
                    if (dtRentalSecurity[0].OrderInstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].OrderInstallFee_CompleteInstall = dtRentalSecurity[0].OrderInstallFee_CompleteInstallUsd;
                    }
                    if (dtRentalSecurity[0].OrderInstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].OrderInstallFee_StartService = dtRentalSecurity[0].OrderInstallFee_StartServiceUsd;
                    }
                    if (dtRentalSecurity[0].InstallFeePaidBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].InstallFeePaidBySECOM = dtRentalSecurity[0].InstallFeePaidBySECOMUsd;
                    }
                    if (dtRentalSecurity[0].InstallFeeRevenueBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[0].InstallFeeRevenueBySECOM = dtRentalSecurity[0].InstallFeeRevenueBySECOMUsd;
                    }

                    /* ----- Set grobal variable for javascript side ---- */
                    ViewBag.strOCC = (dtRentalSecurity.Count > 0 ? dtRentalSecurity[0].OCC : strOCC);

                    // Select language
                    vw_dtRentalContract = CommonUtil.ConvertObjectbyLanguage<dtTbt_RentalContractBasicForView, dtTbt_RentalContractBasicForView>(dtRentalContract, "Quo_OfficeName", "Con_OfficeName", "Op_OfficeName");
                    vw_dtRentalSecurity = CommonUtil.ConvertObjectbyLanguage<dtTbt_RentalSecurityBasicForView, dtTbt_RentalSecurityBasicForView>(dtRentalSecurity,
                                            "ProductName",
                                            "DocumentName",
                                            "DocumentNoName",
                                            "SalesMan1_EmpFirstName",
                                            "SalesMan1_EmpFirstName",
                                            "SalesMan1_EmpLastName",
                                            "SalesMan2_EmpFirstName",
                                            "SalesMan2_EmpLastName",
                                            "SalesSupport_EmpFirstName",
                                            "SalesSupport_EmpLastName",
                                            "Alm_EmpFirstName",
                                            "Alm_EmpLastName",
                                            "NegStaff1_EmpFirstName",
                                            "NegStaff1_EmpLastName",
                                            "NegStaff2_EmpFirstName",
                                            "NegStaff2_EmpLastName",
                                            "CompStaff_EmpFirstName",
                                            "CompStaff_EmpLastName",
                                            "Planner_EmpFirstName",
                                            "Planner_EmpLastName",
                                            "PlanChecker_EmpFirstName",
                                            "PlanChecker_EmpLastName",
                                            "PlanApprover_EmpFirstName",
                                            "PlanApprover_EmpLastName"
                                           );

                    /**** Convert code to short format *****/

                    // vw_dtRentalContract
                    foreach (var item in vw_dtRentalContract)
                    {
                        // contractcode
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        item.OldContractCode = c.ConvertContractCode(item.OldContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        item.CounterBalanceOriginContractCode = c.ConvertContractCode(item.CounterBalanceOriginContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        // customercode
                        item.ContractTargetCustCode = c.ConvertCustCode(item.ContractTargetCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        item.RealCustomerCustCode = c.ConvertCustCode(item.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        // sitecode
                        item.SiteCode = c.ConvertSiteCode(item.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                    // vw_dtRentalSecurity
                    foreach (var item in vw_dtRentalSecurity)
                    {
                        // contractcode
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        // quotation target code
                        item.QuotationTargetCode = c.ConvertQuotationTargetCode(item.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }


                    if (vw_dtRentalContract.Count > 0)
                    {

                        /* ----- Set grobal variable for javascript side ---- */

                        ViewBag.ProductTypeCode = vw_dtRentalContract[0].ProductTypeCode;
                        ViewBag.ContractTargetCode = vw_dtRentalContract[0].ContractTargetCustCode;
                        ViewBag.RealCustomerCode = vw_dtRentalContract[0].RealCustomerCustCode;
                        ViewBag.SiteCode = vw_dtRentalContract[0].SiteCode;
                        ViewBag.ServiceTypeCode = vw_dtRentalContract[0].ServiceTypeCode;

                        ViewBag.SiteCodeList = dtRentalContract[0].SiteCode; // for parameter of generate link function ;
                        ViewBag.ContractCode = vw_dtRentalContract[0].ContractCode; // for parameter of generate link function ;

                        ViewBag.txtContractCode = vw_dtRentalContract[0].ContractCode;
                        ViewBag.txtUserCode = vw_dtRentalContract[0].UserCode;
                        ViewBag.lnkCustomerCodeC = vw_dtRentalContract[0].ContractTargetCustCode;
                        ViewBag.lnkCustomerCodeR = vw_dtRentalContract[0].RealCustomerCustCode;
                        ViewBag.lnkSiteCode = vw_dtRentalContract[0].SiteCode;

                        ViewBag.txtContractNameEng = vw_dtRentalContract[0].CustFullNameEN_Cust; // ContractTargetFullNameEN
                        ViewBag.txtContractAddrEng = vw_dtRentalContract[0].AddressFullEN_Cust;
                        ViewBag.txtSiteNameEng = vw_dtRentalContract[0].SiteNameEN_Site;
                        ViewBag.txtSiteAddrEng = vw_dtRentalContract[0].AddressFullEN_Site;

                        ViewBag.txtContractNameLocal = vw_dtRentalContract[0].CustFullNameLC_Cust; // ContractTargetFullNameLC
                        ViewBag.txtContractAddrLocal = vw_dtRentalContract[0].AddressFullLC_Cust;
                        ViewBag.txtSiteNameLocal = vw_dtRentalContract[0].SiteNameLC_Site;
                        ViewBag.txtSiteAddrLocal = vw_dtRentalContract[0].AddressFullLC_Site;

                        ViewBag.txtContactPoint = CommonUtil.IsNullOrEmpty(vw_dtRentalContract[0].ContactPoint) == true ? "-" : vw_dtRentalContract[0].ContactPoint;

                        //ViewBag.txtContactPoint = vw_dtRentalContract[0].ContactPoint;


                        //

                        ViewBag.chkOut_of_regulation_document_usage_flag = vw_dtRentalContract[0].IrregurationDocUsageFlag != null ?
                                                                           vw_dtRentalContract[0].IrregurationDocUsageFlag.Value : false;
                        ViewBag.txtAttachImportanceFlag = vw_dtRentalContract[0].SpecialCareFlag;
                    }
                }

                // RentalSecurityBasicForView (RSB)
                if (vw_dtRentalSecurity.Count > 0)
                {

                    /* ----- Set grobal variable for javascript side ---- */
                    ViewBag.QuotationTargetCode = vw_dtRentalSecurity[0].QuotationTargetCode;
                    ViewBag.QuotationAlphabet = vw_dtRentalSecurity[0].QuotationAlphabet;
                    ViewBag.InstallationSlipNo = vw_dtRentalSecurity[0].InstallationSlipNo;

                    // OCC
                    ViewBag.txtOccurrence = vw_dtRentalSecurity[0].OCC;

                    // Product information

                    ViewBag.txtContractFee = vw_dtRentalSecurity[0].TextTransferOrderContractFee;
                    //ViewBag.txtContractFee = vw_dtRentalSecurity[0].OrderContractFee != null ? vw_dtRentalSecurity[0].OrderContractFee.Value.ToString(floatNumberFormat) : "-";
                    ViewBag.txtChange_operation_date = (vw_dtRentalSecurity[0].ChangeImplementDate != null ? vw_dtRentalSecurity[0].ChangeImplementDate.Value.ToString(dateFormat) : "-");

                    ViewBag.txtStopFee = vw_dtRentalSecurity[0].TextTransferContractFeeOnStop;
                    //ViewBag.txtStopFee = vw_dtRentalSecurity[0].ContractFeeOnStop != null ? vw_dtRentalSecurity[0].ContractFeeOnStop.Value.ToString(floatNumberFormat) : "-";

                    ViewBag.txtSecurity_type_code = vw_dtRentalSecurity[0].SecurityTypeCode;
                    ViewBag.txtProduct = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].ProductCode, vw_dtRentalSecurity[0].ProductName);

                    string strChangeTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_RENTAL_CHANGE_TYPE, vw_dtRentalSecurity[0].ChangeType);
                    ViewBag.txtChange_type = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].ChangeType, strChangeTypeDisplayValue);

                    string strDisvplayValue = "";
                    if (vw_dtRentalSecurity[0].ChangeType == "24" || vw_dtRentalSecurity[0].ChangeType == "35")
                    {
                        strDisvplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_CHANGE_NAME_REASON_TYPE, vw_dtRentalSecurity[0].ChangeNameReasonType);
                        ViewBag.txtReason = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].ChangeNameReasonType, strDisvplayValue);
                    }
                    else if (vw_dtRentalSecurity[0].ChangeType == "21")
                    {
                        strDisvplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_CHANGE_REASON_TYPE, vw_dtRentalSecurity[0].ChangeReasonType);
                        ViewBag.txtReason = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].ChangeReasonType, strDisvplayValue);


                    }
                    else if (vw_dtRentalSecurity[0].ChangeType == "31" ||
                             vw_dtRentalSecurity[0].ChangeType == "32" ||
                             vw_dtRentalSecurity[0].ChangeType == "33" ||
                             vw_dtRentalSecurity[0].ChangeType == "34" ||
                             vw_dtRentalSecurity[0].ChangeType == "36" ||
                             vw_dtRentalSecurity[0].ChangeType == "38" ||
                             vw_dtRentalSecurity[0].ChangeType == "39"
                            )
                    {
                        strDisvplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_STOP_CANCEL_REASON_TYPE, vw_dtRentalSecurity[0].StopCancelReasonType);
                        ViewBag.txtReason = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].StopCancelReasonType, strDisvplayValue);
                    }
                    else
                    {
                        ViewBag.txtReason = "-";
                    }

                    ViewBag.txtNegotiation_staff_1 = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].NegotiationStaffEmpNo1, string.Format("{0} {1}",
                                                            vw_dtRentalSecurity[0].NegStaff1_EmpFirstName,
                                                            vw_dtRentalSecurity[0].NegStaff1_EmpLastName));
                    ViewBag.txtNegotiation_staff_2 = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].NegotiationStaffEmpNo2, string.Format("{0} {1}",
                                                            vw_dtRentalSecurity[0].NegStaff2_EmpFirstName,
                                                            vw_dtRentalSecurity[0].NegStaff2_EmpLastName));

                    ViewBag.txtApprove_no1 = vw_dtRentalSecurity[0].ApproveNo1;
                    ViewBag.txtApprove_no2 = vw_dtRentalSecurity[0].ApproveNo2;
                    ViewBag.txtChange_operation_register_date = (vw_dtRentalSecurity[0].CompleteChangeOperationDate != null ?
                                                                 vw_dtRentalSecurity[0].CompleteChangeOperationDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtChange_operation_registrant = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].CompleteChangeOperationEmpNo, string.Format("{0} {1}",
                                                                 vw_dtRentalSecurity[0].CompStaff_EmpFirstName,
                                                                 vw_dtRentalSecurity[0].CompStaff_EmpLastName));
                    ViewBag.txtSecurity_memo = vw_dtRentalSecurity[0].SecurityMemo;



                    // Alam provide service type
                    ViewBag.chkFire_monitoring = (vw_dtRentalSecurity[0].FireMonitorFlag != null ? vw_dtRentalSecurity[0].FireMonitorFlag.Value : false);
                    ViewBag.chkCrime_prevention = (vw_dtRentalSecurity[0].CrimePreventFlag != null ? vw_dtRentalSecurity[0].CrimePreventFlag.Value : false);
                    ViewBag.chkEmergency_report = (vw_dtRentalSecurity[0].EmergencyReportFlag != null ? vw_dtRentalSecurity[0].EmergencyReportFlag.Value : false);
                    ViewBag.chkFacility_monitoring = (vw_dtRentalSecurity[0].FacilityMonitorFlag != null ? vw_dtRentalSecurity[0].FacilityMonitorFlag.Value : false);


                    // section: insurance information

                    string strInsuranceTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_INSURANCE_TYPE, vw_dtRentalSecurity[0].InsuranceTypeCode);
                    ViewBag.txtInsuranceType = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].InsuranceTypeCode, strInsuranceTypeDisplayValue);
                    ViewBag.txtInsurance_coverage_amount = vw_dtRentalSecurity[0].TextTransferInsuranceCoverageAmount;
                    //ViewBag.txtInsurance_coverage_amount = (vw_dtRentalSecurity[0].InsuranceCoverageAmount != null ? vw_dtRentalSecurity[0].InsuranceCoverageAmount.Value.ToString(floatNumberFormat) : "-");
                    ViewBag.txtMonthly_insurance_fee = vw_dtRentalSecurity[0].TextTransferMonthlyInsuranceFee;
                    //ViewBag.txtMonthly_insurance_fee = (vw_dtRentalSecurity[0].MonthlyInsuranceFee != null ? vw_dtRentalSecurity[0].MonthlyInsuranceFee.Value.ToString(floatNumberFormat) : "-");

                    // section: contract document information

                    string strDocAuditResultDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_DOC_AUDIT_RESULT, vw_dtRentalSecurity[0].DocAuditResult);
                    ViewBag.txtDocument_audit_result = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].DocAuditResult, strDocAuditResultDisplayValue);
                    ViewBag.txtContract_document_type = vw_dtRentalSecurity[0].DocumentName;


                    // section: future date information
                    ViewBag.txtExpected_resume_service_date = vw_dtRentalSecurity[0].ExpectedResumeDate != null ? vw_dtRentalSecurity[0].ExpectedResumeDate.Value.ToString(dateFormat) : "-";
                    ViewBag.txtReturn_date_to_original_fee = vw_dtRentalSecurity[0].ReturnToOriginalFeeDate != null ? vw_dtRentalSecurity[0].ReturnToOriginalFeeDate.Value.ToString(dateFormat) : "-";


                    // section: quotaion information
                    ViewBag.lnkQuotationCode = (CommonUtil.IsNullOrEmpty(vw_dtRentalSecurity[0].QuotationTargetCode) == true || CommonUtil.IsNullOrEmpty(vw_dtRentalSecurity[0].QuotationAlphabet) == true) ? string.Empty : (string.Format("{0}-{1}", vw_dtRentalSecurity[0].QuotationTargetCode, vw_dtRentalSecurity[0].QuotationAlphabet));
                    ViewBag.txtPlan_code = vw_dtRentalSecurity[0].PlanCode;
                    ViewBag.txtQuatationNo = vw_dtRentalContract[0].QuotationNo;
                    ViewBag.txtPlan_approving_date = vw_dtRentalSecurity[0].PlanApproveDate != null ? vw_dtRentalSecurity[0].PlanApproveDate.Value.ToString(dateFormat) : "-";
                    ViewBag.txtPlan_approver = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].PlanApproverEmpNo, string.Format("{0} {1}",
                                                                vw_dtRentalSecurity[0].PlanApprover_EmpFirstName
                                                                , vw_dtRentalSecurity[0].PlanApprover_EmpLastName));

                    ViewBag.txtNormal_contract_fee = vw_dtRentalSecurity[0].TextTransferNormalContractFee;
                    //ViewBag.txtNormal_contract_fee = vw_dtRentalSecurity[0].NormalContractFee != null ? vw_dtRentalSecurity[0].NormalContractFee.Value.ToString(floatNumberFormat) : "-";


                    ViewBag.txtOutsourcing_fee = vw_dtRentalSecurity[0].TextTransferMaintenanceFee1;
                    //ViewBag.txtOutsourcing_fee = vw_dtRentalSecurity[0].MaintenanceFee1 != null ? vw_dtRentalSecurity[0].MaintenanceFee1.Value.ToString(floatNumberFormat) : "-";

                    ViewBag.txtAdditional_contract_fee1 = vw_dtRentalSecurity[0].TextTransferAdditionalFee1;
                    ViewBag.txtAdditional_contract_fee2 = vw_dtRentalSecurity[0].TextTransferAdditionalFee2;
                    ViewBag.txtAdditional_contract_fee3 = vw_dtRentalSecurity[0].TextTransferAdditionalFee3;

                    //ViewBag.txtAdditional_contract_fee1 = vw_dtRentalSecurity[0].AdditionalFee1 != null ? vw_dtRentalSecurity[0].AdditionalFee1.Value.ToString(floatNumberFormat) : "-";
                    //ViewBag.txtAdditional_contract_fee2 = vw_dtRentalSecurity[0].AdditionalFee2 != null ? vw_dtRentalSecurity[0].AdditionalFee2.Value.ToString(floatNumberFormat) : "-";
                    //ViewBag.txtAdditional_contract_fee3 = vw_dtRentalSecurity[0].AdditionalFee3 != null ? vw_dtRentalSecurity[0].AdditionalFee3.Value.ToString(floatNumberFormat) : "-";



                    // section: installation information

                    string strIntallationtypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_RENTAL_INSTALL_TYPE, vw_dtRentalSecurity[0].InstallationTypeCode);
                    ViewBag.txtInstallation_type = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].InstallationTypeCode, strIntallationtypeDisplayValue);

                    ViewBag.txtComplete_installation_date = vw_dtRentalSecurity[0].InstallationCompleteDate != null ? vw_dtRentalSecurity[0].InstallationCompleteDate.Value.ToString(dateFormat) : "-";

                    ViewBag.txtNormal_installation_fee = vw_dtRentalSecurity[0].TextTransferNormalInstallFee;
                    //ViewBag.txtNormal_installation_fee = vw_dtRentalSecurity[0].NormalInstallFee != null ? vw_dtRentalSecurity[0].NormalInstallFee.Value.ToString(floatNumberFormat) : "-";

                    ViewBag.txtOrder_installation_fee = vw_dtRentalSecurity[0].TextTransferOrderInstallFee;
                    //ViewBag.txtOrder_installation_fee = vw_dtRentalSecurity[0].OrderInstallFee != null ? vw_dtRentalSecurity[0].OrderInstallFee.Value.ToString(floatNumberFormat) : "-";

                    ViewBag.txtOrder_installation_fee_Approve_contract = vw_dtRentalSecurity[0].TextTransferOrderInstallFee_ApproveContract;
                    //ViewBag.txtOrder_installation_fee_Approve_contract = vw_dtRentalSecurity[0].OrderInstallFee_ApproveContract != null ? vw_dtRentalSecurity[0].OrderInstallFee_ApproveContract.Value.ToString(floatNumberFormat) : "-";

                    ViewBag.txtOrder_installation_fee_Complete_installation = vw_dtRentalSecurity[0].TextTransferOrderInstallFee_CompleteInstall;
                    //ViewBag.txtOrder_installation_fee_Complete_installation = vw_dtRentalSecurity[0].OrderInstallFee_CompleteInstall != null ? vw_dtRentalSecurity[0].OrderInstallFee_CompleteInstall.Value.ToString(floatNumberFormat) : "-";

                    ViewBag.txtOrder_installation_fee_Start_service = vw_dtRentalSecurity[0].TextTransferOrderInstallFee_StartService;
                    //ViewBag.txtOrder_installation_fee_Start_service = vw_dtRentalSecurity[0].OrderInstallFee_StartService != null ? vw_dtRentalSecurity[0].OrderInstallFee_StartService.Value.ToString(floatNumberFormat) : "-";

                    ViewBag.txtSECOM_payment = vw_dtRentalSecurity[0].TextTransferInstallFeePaidBySECOM;
                    //ViewBag.txtSECOM_payment = vw_dtRentalSecurity[0].InstallFeePaidBySECOM != null ? vw_dtRentalSecurity[0].InstallFeePaidBySECOM.Value.ToString(floatNumberFormat) : "-";
                    ViewBag.txtSECOM_revenue = vw_dtRentalSecurity[0].TextTransferInstallFeeRevenueBySECOM;
                    //ViewBag.txtSECOM_revenue = vw_dtRentalSecurity[0].InstallFeeRevenueBySECOM != null ? vw_dtRentalSecurity[0].InstallFeeRevenueBySECOM.Value.ToString(floatNumberFormat) : "-";
                    ViewBag.lnkInstallation_slip_no = vw_dtRentalSecurity[0].InstallationSlipNo;

                }

                //dtRentalOperation
                if (dtRentalOperation.Count > 0)
                {
                    //string strOperationtypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_OPERATION_TYPE, dtRentalOperation[0].OperationTypeCode );
                    //ViewBag.txtOperation_type = CommonUtil.TextCodeName(dtRentalOperation[0].OperationTypeCode, strOperationtypeDisplayValue);

                    List<string> lstOperationType = new List<string>();
                    foreach (var item in dtRentalOperation)
                    {
                        lstOperationType.Add(item.OperationTypeCode);
                    }

                    ViewBag.chkOperationTypeList = lstOperationType.ToArray();
                }


                return View();
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Initial grid of screen CMS130 (sub-contarctor grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS130_IntialGridSubcontractor()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS130"));
        }

        /// <summary>
        /// Get sub-contractor data list by contractcode , OCC
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS130_GetSubContractList(string strContractCode, string strOCC)
        {

            CommonUtil c = new CommonUtil();

            List<dtTbt_RentalInstSubContractorListForView> list = new List<dtTbt_RentalInstSubContractorListForView>();

            ObjectResultData res = new ObjectResultData();

            try
            {
                strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                list = handler.GetTbt_RentalInstSubContractorListForView(strContractCode, strOCC);


                //return Json(CommonUtil.ConvertToXml<dtTbt_RentalInstSubContractorListForView>(list, "Common\\CMS130"));

            }
            catch (Exception ex)
            {
                list = new List<dtTbt_RentalInstSubContractorListForView>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtTbt_RentalInstSubContractorListForView>(list, "Common\\CMS130");
            return Json(res);


        }

    }
}
