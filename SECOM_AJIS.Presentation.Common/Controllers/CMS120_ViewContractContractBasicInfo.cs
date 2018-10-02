//*********************************
// Create by: Narupon W.
// Create date: 01/Jun/2011
// Update date: 01/Jun/2011
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

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Common.Models;

using CSI.WindsorHelper;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS120
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS120_Authority(CMS120_ScreenParameter param) //IN parameter: string strContractCode
        {
            ObjectResultData res = new ObjectResultData();


            // Check permission
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_CONTRACT_BASIC, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            // Check parameter is OK ?
            if (CommonUtil.IsNullOrEmpty(param.strContractCode) == false)
            {
                param.ContractCode = param.strContractCode;
            }
            else
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                return Json(res);
            }

            // Check data exist
            try
            {
                CommonUtil c = new CommonUtil();
                string ContractCode = c.ConvertContractCode(param.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

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


            return InitialScreenEnvironment<CMS120_ScreenParameter>("CMS120", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS120
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS120")]
        public ActionResult CMS120()
        {
            string strContractCode = "";


            try
            {
                CMS120_ScreenParameter param = GetScreenObject<CMS120_ScreenParameter>();
                strContractCode = param.ContractCode;
            }
            catch
            {
            }

            ViewBag.ROWS_PER_PAGE_FOR_VIEWPAGE = CommonValue.ROWS_PER_PAGE_FOR_VIEWPAGE;

            // Set grobal variable for javascript side
            ViewBag.strContractCode = strContractCode;


            CommonUtil c = new CommonUtil();
            strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            List<dtTbt_RentalContractBasicForView> vw_dtRentalContract = new List<dtTbt_RentalContractBasicForView>();
            List<dtTbt_RentalSecurityBasicForView> vw_dtRentalSecurity = new List<dtTbt_RentalSecurityBasicForView>();
            List<dtTbt_RentalMaintenanceDetailsForView> vw_dtRentalMaint = new List<dtTbt_RentalMaintenanceDetailsForView>();
            List<dtRelatedContract> vw_dtSaleRelated = new List<dtRelatedContract>();

            List<dtMaintContractTargetInfoByRelated> dtMaintRelated = new List<dtMaintContractTargetInfoByRelated>();
            List<dtMaintContractTargetInfoByRelated> vw_dtMaintRelated = new List<dtMaintContractTargetInfoByRelated>();


            string dateFormat = "dd-MMM-yyyy";
            string numberFormat = "N0";
            string floatNumberFormat = "N2";
            ViewBag.Currency = CommonValue.CURRENCY_UNIT;

            // default ViewBage
            ViewBag.bIrregurationDocUsageFlag = false;
            ViewBag.chkFireMonitoring = false;
            ViewBag.chkCrimePrevention = false;
            ViewBag.chkEmergencyReport = false;
            ViewBag.chkFacilityMonitoring = false;

            ViewBag.hasStopConditionProcessDate = false;

            try
            {


                ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> MiscTypeCode = new List<doMiscTypeCode>();

                List<string> lsFieldNames = new List<string>();
                lsFieldNames.Add(MiscType.C_RENTAL_CHANGE_TYPE);
                lsFieldNames.Add(MiscType.C_ACQUISITION_TYPE);
                lsFieldNames.Add(MiscType.C_MOTIVATION_TYPE);
                lsFieldNames.Add(MiscType.C_DOC_AUDIT_RESULT);
                lsFieldNames.Add(MiscType.C_PHONE_LINE_TYPE);
                lsFieldNames.Add(MiscType.C_PHONE_LINE_OWNER_TYPE);
                lsFieldNames.Add(MiscType.C_MA_TARGET_PROD_TYPE);
                lsFieldNames.Add(MiscType.C_MA_TYPE);
                lsFieldNames.Add(MiscType.C_MA_FEE_TYPE);
                lsFieldNames.Add(MiscType.C_BUILDING_TYPE);
                lsFieldNames.Add(MiscType.C_MAIN_STRUCTURE_TYPE);
                lsFieldNames.Add(MiscType.C_BILLING_TYPE);
                lsFieldNames.Add(MiscType.C_HANDLING_TYPE);


                List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);


                // Rental
                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtTbt_RentalContractBasicForView> dtRentalContract = handler.GetTbt_RentalContractBasicForView(strContractCode);
                List<dtTbt_RentalSecurityBasicForView> dtRentalSecurity = new List<dtTbt_RentalSecurityBasicForView>();
                List<dtTbt_RentalMaintenanceDetailsForView> dtRentalMaint = new List<dtTbt_RentalMaintenanceDetailsForView>();
                List<dtRelatedContract> dtSaleRelated = new List<dtRelatedContract>();

                for (int i = 0; i < dtRentalContract.Count(); i++)
                {
                    dtRentalContract[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                if (dtRentalContract.Count > 0)
                {


                    // Get related data
                    dtRentalSecurity = handler.GetTbt_RentalSecurityBasicForView(strContractCode, dtRentalContract[0].LastOCC);
                    dtRentalMaint = handler.GetTbt_RentalMaintenanceDetailsForView(strContractCode, dtRentalContract[0].LastOCC);
                    dtSaleRelated = handler.GetRelatedContractList(RelationType.C_RELATION_TYPE_SALE, strContractCode, dtRentalContract[0].LastOCC);

                    for (int i = 0; i < dtRentalSecurity.Count(); i++)
                    {
                        dtRentalSecurity[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }

                    IViewContractHandler handlerVC = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                    //dtMaintRelated = handlerVC.GetMaintContractTargetInfoByRelated(strContractCode, MiscType.C_MA_TARGET_PROD_TYPE, MiscType.C_MA_TYPE, MiscType.C_MA_FEE_TYPE, RelationType.C_RELATION_TYPE_MA, dtRentalContract[0].LastOCC
                    //                    , ProductType.C_PROD_TYPE_SALE, ProductType.C_PROD_TYPE_AL, ProductType.C_PROD_TYPE_ONLINE, dtRentalContract[0].ProductTypeCode);
                    dtMaintRelated = handlerVC.GetMaintContractTargetInfoByRelated(strContractCode, dtRentalContract[0].LastOCC, dtRentalContract[0].ProductTypeCode);


                    // Select language

                    vw_dtMaintRelated = CommonUtil.ConvertObjectbyLanguage<dtMaintContractTargetInfoByRelated, dtMaintContractTargetInfoByRelated>(dtMaintRelated,
                                                                                                    "MaintenanceTargetProductTypeName",
                                                                                                    "MaintenanceTypeName",
                                                                                                    "MaintenanceFeeTypeName");

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
                    vw_dtSaleRelated = CommonUtil.ConvertObjectbyLanguage<dtRelatedContract, dtRelatedContract>(dtSaleRelated, "ProductName");
                    vw_dtRentalMaint = CommonUtil.ConvertObjectbyLanguage<dtTbt_RentalMaintenanceDetailsForView, dtTbt_RentalMaintenanceDetailsForView>(dtRentalMaint, "MaintenanceTargetProductType", "MaintenanceFeeType");


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

                    //vw_dtSaleRelated
                    foreach (var item in vw_dtSaleRelated)
                    {
                        // contractcode
                        item.RelatedContractCode = c.ConvertContractCode(item.RelatedContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                    // vw_dtRentalMaint
                    foreach (var item in vw_dtRentalMaint)
                    {
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                    // vw_dtRentalSecurity
                    foreach (var item in vw_dtRentalSecurity)
                    {
                        // contractcode
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        // quotation target code
                        item.QuotationTargetCode = c.ConvertQuotationTargetCode(item.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                    foreach (var item in vw_dtMaintRelated)
                    {
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }


                }


                /*************   CMS120 : 120 fields  ****************/

                // RentalContractBasicForView
                if (vw_dtRentalContract.Count > 0)
                {

                    // Set grobal variable for javascript side

                    ViewBag.strOCC = vw_dtRentalContract[0].LastOCC;
                    ViewBag.ProductTypeCode = vw_dtRentalContract[0].ProductTypeCode;
                    ViewBag.hasStopConditionProcessDate = (vw_dtRentalContract[0].StopConditionProcessDate != null ? true : false);
                    ViewBag.ServiceTypeCode = vw_dtRentalContract[0].ServiceTypeCode;
                    ViewBag.ContractTargetCode = vw_dtRentalContract[0].ContractTargetCustCode;
                    ViewBag.RealCustomerCode = vw_dtRentalContract[0].RealCustomerCustCode;
                    ViewBag.SiteCode = vw_dtRentalContract[0].SiteCode;
                    ViewBag.OldContractCode = vw_dtRentalContract[0].OldContractCode;

                    ViewBag.txtContractCode = vw_dtRentalContract[0].ContractCode;
                    ViewBag.txtUserCode = vw_dtRentalContract[0].UserCode;
                    ViewBag.lnkCustomerCodeC = vw_dtRentalContract[0].ContractTargetCustCode;
                    ViewBag.lnkCustomerCodeR = vw_dtRentalContract[0].RealCustomerCustCode;
                    ViewBag.lnkSiteCode = vw_dtRentalContract[0].SiteCode;

                    ViewBag.txtContractNameEng = vw_dtRentalContract[0].CustFullNameEN_Cust; // ContractTargetFullNameEN
                    ViewBag.txtBranchName_Eng = vw_dtRentalContract[0].BranchNameEN;
                    ViewBag.txtContractAddrEng = vw_dtRentalContract[0].AddressFullEN_Cust;
                    ViewBag.txtBranchAddress_Eng = vw_dtRentalContract[0].BranchAddressEN;
                    ViewBag.txtSiteNameEng = vw_dtRentalContract[0].SiteNameEN_Site;
                    ViewBag.txtSiteAddrEng = vw_dtRentalContract[0].AddressFullEN_Site;

                    ViewBag.txtContractNameLocal = vw_dtRentalContract[0].CustFullNameLC_Cust; // ContractTargetFullNameLC
                    ViewBag.txtBranchName_Local = vw_dtRentalContract[0].BranchNameLC;
                    ViewBag.txtContractAddrLocal = vw_dtRentalContract[0].AddressFullLC_Cust;
                    ViewBag.txtBranchAddress_Local = vw_dtRentalContract[0].BranchAddressLC;
                    ViewBag.txtSiteNameLocal = vw_dtRentalContract[0].SiteNameLC_Site;
                    ViewBag.txtSiteAddrLocal = vw_dtRentalContract[0].AddressFullLC_Site;

                    //ViewBag.txtContactPoint = vw_dtRentalContract[0].ContactPoint;
                    ViewBag.txtContactPoint = CommonUtil.IsNullOrEmpty(vw_dtRentalContract[0].ContactPoint) == true ? "-" : vw_dtRentalContract[0].ContactPoint;



                    // in _CSM120_2 : ContractRelated Information


                    ViewBag.txtContractFee = vw_dtRentalContract[0].TextTransferLastOrderContractFee;
                    ViewBag.txtStartDealDate = (vw_dtRentalContract[0].StartDealDate != null ? vw_dtRentalContract[0].StartDealDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtFirstOperationDate = (vw_dtRentalContract[0].FirstSecurityStartDate != null ? vw_dtRentalContract[0].FirstSecurityStartDate.Value.ToString(dateFormat) : "-");


                    string strLastChangeTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_RENTAL_CHANGE_TYPE,
                                                        vw_dtRentalContract[0].LastChangeType);
                    ViewBag.txtLastChangeType = CommonUtil.TextCodeName(vw_dtRentalContract[0].LastChangeType, strLastChangeTypeDisplayValue);
                    ViewBag.txtLastOperationDate = (vw_dtRentalContract[0].LastChangeImplementDate != null ? vw_dtRentalContract[0].LastChangeImplementDate.Value.ToString(dateFormat) : "-");

                    ViewBag.txtLastOccurrence = vw_dtRentalContract[0].LastOCC;
                    ViewBag.txtBICContractCode = vw_dtRentalContract[0].BICContractCode;
                    ViewBag.txtProjectCode = vw_dtRentalContract[0].ProjectCode;

                    ViewBag.txtApprove_contract_date = (vw_dtRentalContract[0].ApproveContractDate != null ? vw_dtRentalContract[0].ApproveContractDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtContractOffice = CommonUtil.TextCodeName(
                                                        vw_dtRentalContract[0].OfficeCode_Con,
                                                        vw_dtRentalContract[0].Con_OfficeName);

                    ViewBag.lnkOld_contract_code = vw_dtRentalContract[0].OldContractCode;


                    string strAcquisitionTypeDisplayVale = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_ACQUISITION_TYPE,
                                                        vw_dtRentalContract[0].AcquisitionTypeCode);
                    ViewBag.txtAcquisition_type = CommonUtil.TextCodeName(
                                                        vw_dtRentalContract[0].AcquisitionTypeCode,
                                                        strAcquisitionTypeDisplayVale);
                    ViewBag.txtIntroducer_code = vw_dtRentalContract[0].IntroducerCode;

                    string strMotivationTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_MOTIVATION_TYPE,
                                                        vw_dtRentalContract[0].MotivationTypeCode);
                    ViewBag.txtMotivation_type = CommonUtil.TextCodeName(
                                                        vw_dtRentalContract[0].MotivationTypeCode,
                                                        strMotivationTypeDisplayValue);

                    ViewBag.txtNormal_deposit_fee = vw_dtRentalContract[0].TextTransferNormalDepositFee;
                    //ViewBag.txtNormal_deposit_fee = (vw_dtRentalContract[0].NormalDepositFee != null ? vw_dtRentalContract[0].NormalDepositFee.Value.ToString(floatNumberFormat) : "-");
                    //ViewBag.txtBilling_deposit_fee = (vw_dtRentalContract[0].BilledDepositFee != null ? vw_dtRentalContract[0].BilledDepositFee.Value.ToString(floatNumberFormat) : "-");
                    // SA request to chagnge :31/Jan/2012

                    ViewBag.txtBilling_deposit_fee = vw_dtRentalContract[0].TextTransferOrderDepositFee;
                    //ViewBag.txtBilling_deposit_fee = (vw_dtRentalContract[0].OrderDepositFee != null ? vw_dtRentalContract[0].OrderDepositFee.Value.ToString(floatNumberFormat) : "-");

                    ViewBag.txtExempted_deposit_fee = vw_dtRentalContract[0].TextTransferExemptedDepositFee;
                    //ViewBag.txtExempted_deposit_fee = (vw_dtRentalContract[0].ExemptedDepositFee != null ? vw_dtRentalContract[0].ExemptedDepositFee.Value.ToString(floatNumberFormat) : "-");
                    ViewBag.txtOriginContractCode = vw_dtRentalContract[0].CounterBalanceOriginContractCode;
                    ViewBag.bIrregurationDocUsageFlag = (vw_dtRentalContract[0].IrregurationDocUsageFlag != null ? vw_dtRentalContract[0].IrregurationDocUsageFlag.Value : false);

                    string strPODocAuditResultDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_DOC_AUDIT_RESULT,
                                                        vw_dtRentalContract[0].PODocAuditResult);
                    ViewBag.txtDocument_audit_result_PO = CommonUtil.TextCodeName(
                                                        vw_dtRentalContract[0].PODocAuditResult,
                                                        strPODocAuditResultDisplayValue);
                    ViewBag.txtReceive_date_PO = (vw_dtRentalContract[0].PODocReceiveDate != null ? vw_dtRentalContract[0].PODocReceiveDate.Value.ToString(dateFormat) : "-");

                    string strContractDocAuditResultDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_DOC_AUDIT_RESULT,
                                                        vw_dtRentalContract[0].ContractDocAuditResult);
                    ViewBag.txtDocument_audit_result_Contract_report = CommonUtil.TextCodeName(
                                                                        vw_dtRentalContract[0].ContractDocAuditResult,
                                                                        strContractDocAuditResultDisplayValue);
                    ViewBag.txtReceive_date_Contract_report = (vw_dtRentalContract[0].ContractDocReceiveDate != null ? vw_dtRentalContract[0].ContractDocReceiveDate.Value.ToString(dateFormat) : "-");


                    string strStartMemoAuditResultDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                            MiscType.C_DOC_AUDIT_RESULT,
                                                            vw_dtRentalContract[0].StartMemoAuditResult);
                    ViewBag.txtDocument_audit_result_Memo = CommonUtil.TextCodeName(vw_dtRentalContract[0].StartMemoAuditResult, strStartMemoAuditResultDisplayValue);
                    ViewBag.txtReceive_date_Memo = (vw_dtRentalContract[0].StartMemoReceiveDate != null ? vw_dtRentalContract[0].StartMemoReceiveDate.Value.ToString(dateFormat) : "-");

                    ViewBag.txtMemmo_Oth = vw_dtRentalContract[0].Memo;


                    ViewBag.txtFN99 = (vw_dtRentalContract[0].ContractConditionProcessDate != null ? vw_dtRentalContract[0].ContractConditionProcessDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtCP01 = (vw_dtRentalContract[0].ApproveContractDate != null ? vw_dtRentalContract[0].ApproveContractDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtMK03 = (vw_dtRentalContract[0].NewInstallCompleteProcessDate != null ? vw_dtRentalContract[0].NewInstallCompleteProcessDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtCP05 = (vw_dtRentalContract[0].StartResumeProcessDate != null ? vw_dtRentalContract[0].StartResumeProcessDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtCP12_change_plan = (vw_dtRentalContract[0].ChangePlanProcessDate != null ? vw_dtRentalContract[0].ChangePlanProcessDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtCP15 = (vw_dtRentalContract[0].CancelBeforeStartProcessDate != null ? vw_dtRentalContract[0].CancelBeforeStartProcessDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtCP13 = (vw_dtRentalContract[0].StopProcessDate != null ? vw_dtRentalContract[0].StopProcessDate.Value.ToString(dateFormat) : "-");

                    ViewBag.txtCP14 = (vw_dtRentalContract[0].StopConditionProcessDate != null ? vw_dtRentalContract[0].StopConditionProcessDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtCP32 = (vw_dtRentalContract[0].AuditCollectionProcessDate != null ? vw_dtRentalContract[0].AuditCollectionProcessDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtCP16 = (vw_dtRentalContract[0].ChangeNameProcessDate != null ? vw_dtRentalContract[0].ChangeNameProcessDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtCP12_change_contract_fee_only = (vw_dtRentalContract[0].ChangeFeeProcessDate != null ? vw_dtRentalContract[0].ChangeFeeProcessDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtCS01 = (vw_dtRentalContract[0].IncidentProcessDate != null ? vw_dtRentalContract[0].IncidentProcessDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtCP33 = (vw_dtRentalContract[0].ReviseContractBasicProcessDate != null ? vw_dtRentalContract[0].ReviseContractBasicProcessDate.Value.ToString(dateFormat) : "-");
                    ViewBag.txtCP34 = (vw_dtRentalContract[0].ReviseSecurityBasicProcessDate != null ? vw_dtRentalContract[0].ReviseSecurityBasicProcessDate.Value.ToString(dateFormat) : "-");


                    DateTime datFirstSecurityStartDate;
                    if (vw_dtRentalContract[0].FirstSecurityStartDate != null)
                    {
                        //datFirstSecurityStartDate = vw_dtRentalContract[0].FirstSecurityStartDate.Value.AddMonths(1);
                        datFirstSecurityStartDate = vw_dtRentalContract[0].FirstSecurityStartDate.Value;
                        ViewBag.txtMaintenanceContractStart_month_Online = datFirstSecurityStartDate.ToString("MMM-yyyy");
                    }
                    else
                    {
                        ViewBag.txtMaintenanceContractStart_month_Online = "";
                    }

                    ViewBag.txtOperationOffice = CommonUtil.TextCodeName(vw_dtRentalContract[0].OperationOfficeCode, vw_dtRentalContract[0].Op_OfficeName);
                    ViewBag.txtAttachImportanceFlag = vw_dtRentalContract[0].SpecialCareFlag;


                    ViewBag.CancelContractDate = CommonUtil.TextDate(vw_dtRentalContract[0].LastChangeImplementDate);

                    IRentralContractHandler rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                    CommonUtil cmm = new CommonUtil();
                    DateTime? removalInstallationCompleteDate =
                                    rentralContractHandler.GetRemovalInstallCompleteDate(cmm.ConvertContractCode(vw_dtRentalContract[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                    ViewBag.RemovalInstallationCompleteDate = CommonUtil.TextDate(removalInstallationCompleteDate);
                }

                // RentalSecurityBasicForView (RSB)
                if (vw_dtRentalSecurity.Count > 0)
                {


                    ViewBag.txtSecurityTypeCode = vw_dtRentalSecurity[0].SecurityTypeCode;
                    ViewBag.txtProduct = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].ProductCode, vw_dtRentalSecurity[0].ProductName);

                    ViewBag.txtContractDuration_month = vw_dtRentalSecurity[0].ContractDurationMonth.HasValue == true ? vw_dtRentalSecurity[0].ContractDurationMonth.Value.ToString(numberFormat) : "-";

                    ViewBag.txtAutorenew_month = (vw_dtRentalSecurity[0].AutoRenewMonth != null ? vw_dtRentalSecurity[0].AutoRenewMonth.Value.ToString(numberFormat) : "-");
                    ViewBag.txtQuatationNo = vw_dtRentalContract[0].QuotationNo;
                    ViewBag.txtPaymentDate = (vw_dtRentalContract[0].PaymentDateIncentive != null ? vw_dtRentalContract[0].PaymentDateIncentive.Value.ToString(dateFormat) : string.Empty);

                    ViewBag.txtQuotationCode = vw_dtRentalSecurity[0].QuotationTargetCode + "-" + vw_dtRentalSecurity[0].QuotationAlphabet;
                    ViewBag.txtPlanCode = vw_dtRentalSecurity[0].PlanCode;

                    string strContractStartDate = (vw_dtRentalSecurity[0].ContractStartDate != null ? vw_dtRentalSecurity[0].ContractStartDate.Value.ToString(dateFormat) : string.Empty);
                    //string strContractEndDate = (vw_dtRentalSecurity[0].ContractEndDate != null ? vw_dtRentalSecurity[0].ContractEndDate.Value.ToString(dateFormat) : string.Empty);
                    string strContractEndDate = (vw_dtRentalSecurity[0].CalContractEndDate != null ? vw_dtRentalSecurity[0].CalContractEndDate.Value.ToString(dateFormat) : string.Empty);

                    string strContractDuration = "";

                    if (CommonUtil.IsNullOrEmpty(strContractStartDate) == false && CommonUtil.IsNullOrEmpty(strContractEndDate) == false)
                    {
                        strContractDuration = string.Format("{0} <span style='color:black;'>~</span> {1}", strContractStartDate, strContractEndDate);
                    }
                    else if (CommonUtil.IsNullOrEmpty(strContractStartDate) == false)
                    {
                        strContractDuration = string.Format("{0} <span style='color:black;'>~</span> {1}", strContractStartDate, "-");
                    }
                    else if (CommonUtil.IsNullOrEmpty(strContractEndDate) == false)
                    {
                        strContractDuration = string.Format("{0} <span style='color:black;'>~</span> {1}", "-", strContractEndDate);
                    }
                    else
                    {
                        strContractDuration = string.Empty;
                    }


                    ViewBag.strContractStartDate = strContractStartDate;
                    ViewBag.strContractEndDate = strContractEndDate;

                    ViewBag.txtContractDuration = strContractDuration;
                    ViewBag.ContractStartDate = strContractStartDate;
                    ViewBag.ContractEndDate = strContractEndDate;


                    ViewBag.txtSalesman_1 = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].SalesmanEmpNo1, string.Format("{0} {1}", vw_dtRentalSecurity[0].SalesMan1_EmpFirstName, vw_dtRentalSecurity[0].SalesMan1_EmpLastName));
                    ViewBag.txtSalesman_2 = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].SalesmanEmpNo2, string.Format("{0} {1}", vw_dtRentalSecurity[0].SalesMan2_EmpFirstName, vw_dtRentalSecurity[0].SalesMan2_EmpLastName));

                    ViewBag.txtSales_supporter = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].SalesSupporterEmpNo, string.Format("{0} {1}", vw_dtRentalSecurity[0].SalesSupport_EmpFirstName, vw_dtRentalSecurity[0].SalesSupport_EmpLastName));

                    ViewBag.chkFireMonitoring = (vw_dtRentalSecurity[0].FireMonitorFlag != null ? vw_dtRentalSecurity[0].FireMonitorFlag : false);
                    ViewBag.chkCrimePrevention = (vw_dtRentalSecurity[0].CrimePreventFlag != null ? vw_dtRentalSecurity[0].CrimePreventFlag : false);
                    ViewBag.chkEmergencyReport = (vw_dtRentalSecurity[0].EmergencyReportFlag != null ? vw_dtRentalSecurity[0].EmergencyReportFlag : false);
                    ViewBag.chkFacilityMonitoring = (vw_dtRentalSecurity[0].FacilityMonitorFlag != null ? vw_dtRentalSecurity[0].FacilityMonitorFlag : false);

                    string strPhoneTypeDisplayValue1 = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                                MiscType.C_PHONE_LINE_TYPE,
                                                                vw_dtRentalSecurity[0].PhoneLineTypeCode1);
                    string strPhoneLineOwnerTypeDisplayValue1 = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                                MiscType.C_PHONE_LINE_OWNER_TYPE,
                                                                vw_dtRentalSecurity[0].PhoneLineOwnerTypeCode1);
                    ViewBag.txtTelephoneLineType_Signal = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].PhoneLineTypeCode1, strPhoneTypeDisplayValue1);
                    ViewBag.txtTelephoneLineOwner_Signal = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].PhoneLineOwnerTypeCode1, strPhoneLineOwnerTypeDisplayValue1);
                    ViewBag.txtTelephoneNo_Signal = vw_dtRentalSecurity[0].PhoneNo1;


                    string strPhoneTypeDisplayValue2 = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                                MiscType.C_PHONE_LINE_TYPE,
                                                                vw_dtRentalSecurity[0].PhoneLineTypeCode2);
                    string strPhoneLineOwnerTypeDisplayValue2 = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                                MiscType.C_PHONE_LINE_OWNER_TYPE,
                                                                vw_dtRentalSecurity[0].PhoneLineOwnerTypeCode2);
                    ViewBag.txtTelephoneLineType_img = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].PhoneLineTypeCode2, strPhoneTypeDisplayValue2);
                    ViewBag.txtTelephoneLineOwner_img = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].PhoneLineOwnerTypeCode2, strPhoneLineOwnerTypeDisplayValue2);
                    ViewBag.txtTelephoneNo_img = vw_dtRentalSecurity[0].PhoneNo2;

                    string strPhoneTypeDisplayValue3 = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                               MiscType.C_PHONE_LINE_TYPE,
                                                               vw_dtRentalSecurity[0].PhoneLineTypeCode3);
                    string strPhoneLineOwnerTypeDisplayValue3 = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                                MiscType.C_PHONE_LINE_OWNER_TYPE,
                                                                vw_dtRentalSecurity[0].PhoneLineOwnerTypeCode3);
                    ViewBag.txtTelephoneLineType_discon = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].PhoneLineTypeCode3, strPhoneTypeDisplayValue3);
                    ViewBag.txtTelephoneLineOwner_discon = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].PhoneLineOwnerTypeCode3, strPhoneLineOwnerTypeDisplayValue3);
                    ViewBag.txtTelephoneNo_discon = vw_dtRentalSecurity[0].PhoneNo3;

                    ViewBag.txtMaintenanceCycleMonth = CommonUtil.TextNumeric(vw_dtRentalSecurity[0].MaintenanceCycle, 0);
                    ViewBag.txtMaintenanceCycleMonth_ma = CommonUtil.TextNumeric(vw_dtRentalSecurity[0].MaintenanceCycle, 0);

                    string strBiuldingTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_BUILDING_TYPE, vw_dtRentalSecurity[0].BuildingTypeCode);
                    ViewBag.txtNew_OldBuilding = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].BuildingTypeCode, strBiuldingTypeDisplayValue);

                    ViewBag.txtSiteBuildingArea = CommonUtil.TextNumeric(vw_dtRentalSecurity[0].SiteBuildingArea, 2);
                    ViewBag.txtNumberOfBuildings = CommonUtil.TextNumeric(vw_dtRentalSecurity[0].NumOfBuilding, 0);

                    string strSecurity_area_square_meter = (CommonUtil.IsNullOrEmpty(vw_dtRentalSecurity[0].SecurityAreaFrom) == true || CommonUtil.IsNullOrEmpty(vw_dtRentalSecurity[0].SecurityAreaTo) == true) ? string.Empty : string.Format("{0} <span style='color:black;'>~</span> {1}", CommonUtil.TextNumeric(vw_dtRentalSecurity[0].SecurityAreaFrom, 2), CommonUtil.TextNumeric(vw_dtRentalSecurity[0].SecurityAreaTo, 2));
                    ViewBag.SecurityAreaFrom = CommonUtil.TextNumeric(vw_dtRentalSecurity[0].SecurityAreaFrom, 2);
                    ViewBag.SecurityAreaTo = CommonUtil.TextNumeric(vw_dtRentalSecurity[0].SecurityAreaTo, 2);
                    ViewBag.txtSecurity_area_square_meter = strSecurity_area_square_meter;

                    ViewBag.txtNumber_of_floors = CommonUtil.TextNumeric(vw_dtRentalSecurity[0].NumOfFloor, 0);

                    string strStructureTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_MAIN_STRUCTURE_TYPE, vw_dtRentalSecurity[0].MainStructureTypeCode);
                    ViewBag.txtMain_structure_type = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].MainStructureTypeCode, strStructureTypeDisplayValue);


                    // additional
                    string lblNoNeed = CommonUtil.TextCodeName("0" , CommonUtil.GetLabelFromResource("Common", "CommonResources", "lblNoNeed") );
                    string lblNeed = CommonUtil.TextCodeName("1", CommonUtil.GetLabelFromResource("Common", "CommonResources", "lblNeed"));
                    ViewBag.txtNew_building_mgmt_type = vw_dtRentalSecurity[0].NewBldMgmtFlag != null ? (vw_dtRentalSecurity[0].NewBldMgmtFlag.Value == true ? lblNeed : lblNoNeed) : "-";
                    ViewBag.txtNew_building_mgmt_cost = vw_dtRentalSecurity[0].TextTransferNewBldMgmtCost;
                    //ViewBag.txtNew_building_mgmt_cost = CommonUtil.TextNumeric(vw_dtRentalSecurity[0].NewBldMgmtCost);

                }

                // SaleRelated
                if (vw_dtSaleRelated.Count > 0)
                {
                    //***** grobal variable for js **//
                    ViewBag.strRelatedOCC = vw_dtSaleRelated[0].RelatedOCC;

                    ViewBag.lnkLinkage_sale_contract_code = vw_dtSaleRelated[0].RelatedContractCode;
                    ViewBag.RelatedContractCode = vw_dtSaleRelated[0].RelatedContractCode;
                }

                // RentalMaintenance Detail for view
                if (vw_dtRentalMaint.Count > 0)
                {
                    ViewBag.txtMaintenance_contractCode = vw_dtRentalMaint[0].ContractCode;

                    string strMaintenanceTargetProductTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_MA_TARGET_PROD_TYPE, vw_dtRentalMaint[0].MaintenanceTargetProductTypeCode);
                    ViewBag.txtMaintenanceTargetProduct = CommonUtil.TextCodeName(vw_dtRentalMaint[0].MaintenanceTargetProductTypeCode, strMaintenanceTargetProductTypeDisplayValue);

                    string strMaintenanceTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_MA_TYPE, vw_dtRentalMaint[0].MaintenanceTypeCode);
                    ViewBag.txtMaintenanceType = CommonUtil.TextCodeName(vw_dtRentalMaint[0].MaintenanceTypeCode, strMaintenanceTypeDisplayValue);

                    int iStartMonth = (vw_dtRentalMaint[0].MaintenanceContractStartMonth != null ? vw_dtRentalMaint[0].MaintenanceContractStartMonth.Value : 1);
                    string strStartMonth = (new DateTime(2000, iStartMonth, 1)).ToString("MMM");
                    string strStartYear = vw_dtRentalMaint[0].MaintenanceContractStartYear.HasValue == true ? vw_dtRentalMaint[0].MaintenanceContractStartYear.Value.ToString() : "-";
                    ViewBag.txtMaintenanceContractStart_month = string.Format("{0}-{1}", strStartMonth, strStartYear);

                    string strMaintenanceFeeTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_MA_FEE_TYPE, vw_dtRentalMaint[0].MaintenanceFeeTypeCode);
                    ViewBag.txtMaintenanceFeeType = CommonUtil.TextCodeName(vw_dtRentalMaint[0].MaintenanceFeeTypeCode, strMaintenanceFeeTypeDisplayValue);

                    if (CommonUtil.IsNullOrEmpty(vw_dtRentalMaint[0].MaintenanceMemo) == false)
                        ViewBag.txtMemo = vw_dtRentalMaint[0].MaintenanceMemo;

                    //ViewBag.txtMemo = vw_dtRentalMaint[0].MaintenanceMemo;



                }

                // vw_dtMaintRelated (** new requirement)
                if (vw_dtMaintRelated.Count > 0)
                {
                    ViewBag.txtMaintenance_contractCodeh_ma = vw_dtMaintRelated[0].ContractCode;

                    string strMaintenanceTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_MA_TYPE, vw_dtMaintRelated[0].MaintenanceTypeCode);
                    ViewBag.txtMaintenanceType_ma = CommonUtil.TextCodeName(vw_dtMaintRelated[0].MaintenanceTypeCode, strMaintenanceTypeDisplayValue);
                    string strMaintenanceTargetProductTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_MA_TARGET_PROD_TYPE, vw_dtMaintRelated[0].MaintenanceTargetProductTypeCode);
                    ViewBag.txtMaintenanceTargetProduct_ma = CommonUtil.TextCodeName(vw_dtMaintRelated[0].MaintenanceTargetProductTypeCode, strMaintenanceTargetProductTypeDisplayValue);

                    ViewBag.txtMaintenanceCycleMonth_ma = vw_dtMaintRelated[0].MaintenanceCycle.HasValue == true ? CommonUtil.TextNumeric(vw_dtMaintRelated[0].MaintenanceCycle.Value, 0) : "-";


                    int iStartMonth = (vw_dtMaintRelated[0].MaintenanceContractStartMonth != null ? vw_dtMaintRelated[0].MaintenanceContractStartMonth.Value : 1);
                    string strStartMonth = (new DateTime(2000, iStartMonth, 1)).ToString("MMM");
                    string strStartYear = vw_dtMaintRelated[0].MaintenanceContractStartYear.HasValue == true ? vw_dtMaintRelated[0].MaintenanceContractStartYear.Value.ToString() : "-";
                    ViewBag.txtMaintenanceContractStart_month_Online = string.Format("{0}-{1}", strStartMonth, strStartYear);

                    string strMaintenanceFeeTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_MA_FEE_TYPE, vw_dtMaintRelated[0].MaintenanceFeeTypeCode);
                    ViewBag.txtMaintenanceFeeType_ma = CommonUtil.TextCodeName(vw_dtMaintRelated[0].MaintenanceFeeTypeCode, strMaintenanceFeeTypeDisplayValue);

                    if (CommonUtil.IsNullOrEmpty(vw_dtMaintRelated[0].MaintenanceMemo) == false)
                        ViewBag.txtMemo_ma = vw_dtMaintRelated[0].MaintenanceMemo;

                    //ViewBag.txtMemo_ma = vw_dtMaintRelated[0].MaintenanceMemo;
                }
                else
                {
                    ViewBag.txtMaintenance_contractCodeh_ma = "-";

                    MessageModel myMessage = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0123);
                    ViewBag.txtMaintenanceType_ma = myMessage.Message;  // "Periodical alarm maintenance"
                    ViewBag.txtMaintenanceTargetProduct_ma = "-";

                    ViewBag.txtMaintenanceFeeType_ma = "-";

                    ViewBag.txtMemo_ma = "-";


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
        /// Initial grid of screen CMS120 (maintenance contract grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS120_IntialGridMaintenanceContract()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS120_MaintenanceContract"));
        }

        /// <summary>
        /// Initial grid of screen CMS120 (cancel contract grid)
        /// </summary>
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS120_IntialGridCancelContract()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS120_CancelContract"));
        }

        /// <summary>
        /// Get maintenance contract target list
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS120_GetMaintenanceContractTargetList(string strContractCode, string strOCC)
        {

            CommonUtil c = new CommonUtil();
            List<View_dtRelatedContract> nlst = new List<View_dtRelatedContract>();

            ObjectResultData res = new ObjectResultData();

            try
            {
                strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtRelatedContract> list = handler.GetRelatedContractList(RelationType.C_RELATION_TYPE_MA, strContractCode, strOCC);

                for (int i = 0; i < list.Count(); i++)
                {
                    list[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                // Clone object to View object
                foreach (dtRelatedContract l in list)
                {
                    nlst.Add(CommonUtil.CloneObject<dtRelatedContract, View_dtRelatedContract>(l));
                }

                // Select language
                nlst = CommonUtil.ConvertObjectbyLanguage<View_dtRelatedContract, View_dtRelatedContract>(nlst, "ProductName");

                // convert to short format
                foreach (var item in nlst)
                {
                    // contractcode
                    item.RelatedContractCode = c.ConvertContractCode(item.RelatedContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }

                //return Json(CommonUtil.ConvertToXml<View_dtRelatedContract>(nlst, "Common\\CMS120_MaintenanceContract"));

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                nlst = new List<View_dtRelatedContract>();
                res.AddErrorMessage(ex);

            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtRelatedContract>(nlst, "Common\\CMS120_MaintenanceContract");
            return Json(res);
        }

        /// <summary>
        /// Get cancel contract memo detail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS120_GetCancelContractMemoDetailList(string strContractCode, string strOCC)
        {

            CommonUtil c = new CommonUtil();

            List<View_dtTbt_CancelContractMemoDetailForView> nlst = new List<View_dtTbt_CancelContractMemoDetailForView>();

            ObjectResultData res = new ObjectResultData();

            try
            {
                strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<dtTbt_CancelContractMemoDetailForView> list = handler.GetTbt_CancelContractMemoDetailForView(strContractCode, strOCC);

                // Select language
                list = CommonUtil.ConvertObjectbyLanguage<dtTbt_CancelContractMemoDetailForView, dtTbt_CancelContractMemoDetailForView>(list, "BillingTypeValueDisplay", "HandlingTypeValueDisplay");

                // View_dtTbt_CancelContractMemoDetailForView

                // Clone object to View object
                foreach (dtTbt_CancelContractMemoDetailForView l in list)
                {
                    nlst.Add(CommonUtil.CloneObject<dtTbt_CancelContractMemoDetailForView, View_dtTbt_CancelContractMemoDetailForView>(l));
                }

                //return Json(CommonUtil.ConvertToXml<View_dtTbt_CancelContractMemoDetailForView>(nlst, "Common\\CMS120_CancelContract"));

            }
            catch (Exception ex)
            {
                nlst = new List<View_dtTbt_CancelContractMemoDetailForView>();
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtTbt_CancelContractMemoDetailForView>(nlst, "Common\\CMS120_CancelContract");
            return Json(res);
        }

        /// <summary>
        /// get contract fee detail data list
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS120_GetContractFeeDetail(string strContractCode, string strOCC)
        {
            CommonUtil c = new CommonUtil();
            strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            List<CMS120_ContractFeeDetail> list = new List<CMS120_ContractFeeDetail>();

            ObjectResultData res = new ObjectResultData();

            try
            {
                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtTbt_RentalContractBasicForView> dtRentalContract = handler.GetTbt_RentalContractBasicForView(strContractCode);
                List<dtTbt_RentalSecurityBasicForView> dtRentalSecurity = handler.GetTbt_RentalSecurityBasicForView(strContractCode, strOCC);

                for (int i = 0; i < dtRentalContract.Count(); i++)
                {
                    dtRentalContract[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    if(dtRentalContract[i].NormalDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalContract[i].NormalDepositFee = dtRentalContract[i].NormalDepositFeeUsd;
                    }
                    if (dtRentalContract[i].OrderDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalContract[i].OrderDepositFee = dtRentalContract[i].OrderDepositFeeUsd;
                    }
                }
                for (int i = 0; i < dtRentalSecurity.Count(); i++)
                {
                    dtRentalSecurity[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    if(dtRentalSecurity[i].NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[i].NormalContractFee = dtRentalSecurity[i].NormalContractFeeUsd;
                    }
                    if (dtRentalSecurity[i].OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[i].OrderContractFee = dtRentalSecurity[i].OrderContractFeeUsd;
                    }
                    if (dtRentalSecurity[i].NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[i].NormalInstallFee = dtRentalSecurity[i].NormalInstallFeeUsd;
                    }
                    if (dtRentalSecurity[i].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalSecurity[i].OrderInstallFee = dtRentalSecurity[i].OrderInstallFeeUsd;
                    }
                }

                list.Add(new CMS120_ContractFeeDetail()
                {
                    Title = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS120", "lblContractFee"),
                    NormalAmt = (dtRentalSecurity.Count > 0 ? dtRentalSecurity[0].NormalContractFee ?? 0 : 0),
                    txtNormalAmt = dtRentalSecurity[0].TextTransferNormalContractFee,
                    Currency1 = dtRentalSecurity[0].NormalContractFeeCurrencyType,
                    OrderAmt = (dtRentalSecurity.Count > 0 ? dtRentalSecurity[0].OrderContractFee ?? 0 : 0),
                    txtOrderAmt = dtRentalSecurity[0].TextTransferOrderContractFee,
                    Currency2 = dtRentalSecurity[0].OrderContractFeeCurrencyType,
                    DiscountName = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS120", "headerDiscount"),
                });

                list.Add(new CMS120_ContractFeeDetail()
                {
                    Title = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS120", "lblInstallationFee"),
                    NormalAmt = (dtRentalSecurity.Count > 0 ? dtRentalSecurity[0].NormalInstallFee ?? 0 : 0),
                    txtNormalAmt = dtRentalSecurity[0].TextTransferNormalInstallFee,
                    Currency1 = dtRentalSecurity[0].NormalInstallFeeCurrencyType,
                    OrderAmt = (dtRentalSecurity.Count > 0 ? dtRentalSecurity[0].OrderInstallFee ?? 0 : 0),
                    txtOrderAmt = dtRentalSecurity[0].TextTransferOrderInstallFee,
                    Currency2 = dtRentalSecurity[0].OrderInstallFeeCurrencyType,
                    DiscountName = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS120", "headerDiscount"),
                });

                list.Add(new CMS120_ContractFeeDetail()
                {
                    Title = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS120", "lblDepositFee"),
                    NormalAmt = (dtRentalContract.Count > 0 ? dtRentalContract[0].NormalDepositFee ?? 0 : 0),
                    txtNormalAmt = dtRentalContract[0].TextTransferNormalDepositFee,
                    Currency1 = dtRentalContract[0].NormalDepositFeeCurrencyType,
                    OrderAmt = (dtRentalContract.Count > 0 ? dtRentalContract[0].OrderDepositFee ?? 0 : 0),
                    txtOrderAmt = dtRentalContract[0].TextTransferOrderDepositFee,
                    Currency2 = dtRentalContract[0].OrderDepositFeeCurrencyType,
                    DiscountName = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS120", "headerDiscount"),
                });

                foreach (var item in list)
                {
                    decimal normal = item.NormalAmt ?? 0;
                    decimal order = item.OrderAmt ?? 0;
                    decimal rate = 0;

                    if(item.Currency1 != item.Currency2)
                    {
                        item.DiscountName = (rate < 0 ? ReceivedRate.C_RECEIVED_RATE_OVER : (rate == 0 ? "-" : ReceivedRate.C_RECEIVED_RATE_DISCOUNT));
                        item.DiscountRate = "-";
                    }
                    else
                    {
                        if (normal > 0)
                        {
                            rate = ((normal - order) / normal) * 100;
                        }
                        item.DiscountName = (rate < 0 ? ReceivedRate.C_RECEIVED_RATE_OVER : (rate == 0 ? "-" : ReceivedRate.C_RECEIVED_RATE_DISCOUNT));
                        item.DiscountRate = string.Format("{0} %", Math.Abs(rate).ToString("N2"));
                    }
                }
            }
            catch (Exception ex)
            {
                list = new List<CMS120_ContractFeeDetail>();
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<CMS120_ContractFeeDetail>(list, "Common\\CMS120_ContractFeeDetail");
            return Json(res);
        }

    }
}
