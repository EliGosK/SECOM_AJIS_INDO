//*********************************
// Create by: Narupon W.
// Create date: /Jun/2010
// Update date: /Jun/2010
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

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS140
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS140_Authority(CMS140_ScreenParameter param) // IN parameter: string strContractCode, string strOCC
        {
            ObjectResultData res = new ObjectResultData();

            // Check permission
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_SECURITY_DETAIL, FunctionID.C_FUNC_ID_OPERATE))
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

            // Check data exist
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


            return InitialScreenEnvironment<CMS140_ScreenParameter>("CMS140", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS140
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS140")]
        public ActionResult CMS140()
        {
            string strContractCode = "";
            string strOCC = "";

            try
            {
                CMS140_ScreenParameter param = GetScreenObject<CMS140_ScreenParameter>();
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
            List<dtTbt_RentalMaintenanceDetailsForView> vw_dtRentalMaint = new List<dtTbt_RentalMaintenanceDetailsForView>();


            ViewBag.Currency = CommonValue.CURRENCY_UNIT;

            // default ViewBag
            ViewBag.chkFire_monitoring = false;
            ViewBag.chkCrime_prevention = false;
            ViewBag.chkEmergency_report = false;
            ViewBag.chkFacility_monitoring = false;


            try
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

                ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> MiscTypeCode = new List<doMiscTypeCode>();

                List<string> lsFieldNames = new List<string>();
                lsFieldNames.Add(MiscType.C_RENTAL_CHANGE_TYPE);
                lsFieldNames.Add(MiscType.C_CHANGE_NAME_REASON_TYPE);
                lsFieldNames.Add(MiscType.C_REASON_TYPE);
                lsFieldNames.Add(MiscType.C_STOP_CANCEL_REASON_TYPE);
                lsFieldNames.Add(MiscType.C_MA_TARGET_PROD_TYPE);
                lsFieldNames.Add(MiscType.C_MA_TYPE);
                lsFieldNames.Add(MiscType.C_MA_FEE_TYPE);
                lsFieldNames.Add(MiscType.C_SG_AREA_TYPE);
                lsFieldNames.Add(MiscType.C_SG_TYPE);
                lsFieldNames.Add(MiscType.C_NUM_OF_DATE);
                lsFieldNames.Add(MiscType.C_CHANGE_REASON_TYPE);

                // Get Misc type
                List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);

                // Rental
                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                List<dtTbt_RentalContractBasicForView> dtRentalContract = handler.GetTbt_RentalContractBasicForView(strContractCode);
                List<dtTbt_RentalSecurityBasicForView> dtRentalSecurity = new List<dtTbt_RentalSecurityBasicForView>();
                List<dtTbt_RentalMaintenanceDetailsForView> dtRentalMaint = new List<dtTbt_RentalMaintenanceDetailsForView>();
                List<dtTbt_RentalBEDetailsForView> dtRentalBED = new List<dtTbt_RentalBEDetailsForView>();
                List<dtTbt_RentalSentryGuardForView> RentalSG = new List<dtTbt_RentalSentryGuardForView>();

                if (dtRentalContract.Count > 0)
                {


                    // Get related data
                    dtRentalSecurity = handler.GetTbt_RentalSecurityBasicForView(strContractCode, (CommonUtil.IsNullOrEmpty(strOCC) == false ? strOCC : dtRentalContract[0].LastOCC));
                    dtRentalMaint = handler.GetTbt_RentalMaintenanceDetailsForView(strContractCode, (dtRentalSecurity.Count > 0 ? dtRentalSecurity[0].OCC : strOCC));
                    dtRentalBED = handler.GetTbt_RentalBEDetailsForView(strContractCode, (dtRentalSecurity.Count > 0 ? dtRentalSecurity[0].OCC : strOCC));
                    RentalSG = handler.GetTbt_RentalSentryGuardForView(strContractCode, (CommonUtil.IsNullOrEmpty(strOCC) == false ? strOCC : dtRentalContract[0].LastOCC));

                    //Add Currency
                    for (int i = 0; i < RentalSG.Count(); i++)
                    {
                        if(RentalSG[i].SecurityItemFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            RentalSG[i].SecurityItemFee = RentalSG[i].SecurityItemFeeUsd;
                        }
                        if (RentalSG[i].OtherItemFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            RentalSG[i].OtherItemFee = RentalSG[i].OtherItemFeeUsd;
                        }
                        RentalSG[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }


                    /* ----- Set grobal variable for javascript side ---- */

                    ViewBag.strOCC = (dtRentalSecurity.Count > 0 ? dtRentalSecurity[0].OCC : strOCC);
                    ViewBag.ProductTypeCode = dtRentalContract[0].ProductTypeCode;


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

                    foreach (var item in RentalSG)
                    {
                        string strDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_SG_AREA_TYPE, item.SentryGuardAreaTypeCode);
                        item.SentryGuardAreaType = CommonUtil.TextCodeName(item.SentryGuardAreaTypeCode, strDisplayValue);

                    }





                    if (vw_dtRentalContract.Count > 0)
                    {
                        /* -- global valiable for javascript */
                        ViewBag.ServiceTypeCode = vw_dtRentalContract[0].ServiceTypeCode;
                        ViewBag.ContractTargetCode = vw_dtRentalContract[0].ContractTargetCustCode;
                        ViewBag.RealCustomerCode = vw_dtRentalContract[0].RealCustomerCustCode;
                        ViewBag.SiteCode = vw_dtRentalContract[0].SiteCode;


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

                        DateTime datFirstSecurityStartDate;
                        if (vw_dtRentalContract[0].FirstSecurityStartDate != null)
                        {
                            datFirstSecurityStartDate = vw_dtRentalContract[0].FirstSecurityStartDate.Value.AddMonths(1);
                            ViewBag.txtMaintenanceContractStart_month_Online = datFirstSecurityStartDate.ToString("MMM-yyyy");
                        }
                        else
                        {
                            ViewBag.txtMaintenanceContractStart_month_Online = "";
                        }

                        ViewBag.txtAttachImportanceFlag = vw_dtRentalContract[0].SpecialCareFlag;
                    }
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
                    ViewBag.txtMaintenanceContractStart_month = strStartMonth + "-" + vw_dtRentalMaint[0].MaintenanceContractStartYear.ToString();

                    string strMaintenanceFeeTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_MA_FEE_TYPE, vw_dtRentalMaint[0].MaintenanceFeeTypeCode);
                    ViewBag.txtMaintenanceFeeType = CommonUtil.TextCodeName(vw_dtRentalMaint[0].MaintenanceFeeTypeCode, strMaintenanceFeeTypeDisplayValue);

                    if (CommonUtil.IsNullOrEmpty(vw_dtRentalMaint[0].MaintenanceMemo) == false)
                        ViewBag.txtMemo = vw_dtRentalMaint[0].MaintenanceMemo;

                    //ViewBag.txtMemo = vw_dtRentalMaint[0].MaintenanceMemo;

                }

                // RentalSecurityBasicForView (RSB)
                if (vw_dtRentalSecurity.Count > 0)
                {
                    // New requirement 27/Feb/2012
                    ViewBag.FacilityMemo = vw_dtRentalSecurity[0].FacilityMemo;

                    ViewBag.txtOccurrence = CommonUtil.IsNullOrEmpty(vw_dtRentalSecurity[0].OCC) == true ? "-" : vw_dtRentalSecurity[0].OCC;

                    // Product information
                    ViewBag.txtSecurity_type_code = vw_dtRentalSecurity[0].SecurityTypeCode;
                    ViewBag.txtProduct = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].ProductCode, vw_dtRentalSecurity[0].ProductName);

                    string strChangeTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_RENTAL_CHANGE_TYPE, vw_dtRentalSecurity[0].ChangeType);
                    ViewBag.txtChange_type = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].ChangeType, strChangeTypeDisplayValue);

                    string strDisvplayValue = "";
                    if (vw_dtRentalSecurity[0].ChangeType == "24")
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
                             vw_dtRentalSecurity[0].ChangeType == "36"
                            )
                    {
                        strDisvplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_STOP_CANCEL_REASON_TYPE, vw_dtRentalSecurity[0].StopCancelReasonType);
                        ViewBag.txtReason = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].StopCancelReasonType, strDisvplayValue);
                    }
                    else
                    {
                        ViewBag.txtReason = "-";
                    }


                    ViewBag.txtChange_operation_date = CommonUtil.TextDate(vw_dtRentalSecurity[0].ChangeImplementDate);

                    // Alam provide service type
                    ViewBag.chkFire_monitoring = (vw_dtRentalSecurity[0].FireMonitorFlag != null ? vw_dtRentalSecurity[0].FireMonitorFlag.Value : false);
                    ViewBag.chkCrime_prevention = (vw_dtRentalSecurity[0].CrimePreventFlag != null ? vw_dtRentalSecurity[0].CrimePreventFlag.Value : false);
                    ViewBag.chkEmergency_report = (vw_dtRentalSecurity[0].EmergencyReportFlag != null ? vw_dtRentalSecurity[0].EmergencyReportFlag.Value : false);
                    ViewBag.chkFacility_monitoring = (vw_dtRentalSecurity[0].FacilityMonitorFlag != null ? vw_dtRentalSecurity[0].FacilityMonitorFlag.Value : false);

                    //
                    ViewBag.txtMaintenanceCycleMonth = CommonUtil.TextNumeric(vw_dtRentalSecurity[0].MaintenanceCycle, 0);
                }

                // dtRentalBED
                if (dtRentalBED.Count > 0)
                {
                    ViewBag.txtWeekday_daytime = (dtRentalBED[0].NumOfDayTimeWd != null ? dtRentalBED[0].NumOfDayTimeWd.Value.ToString("N0") : "-");
                    ViewBag.txtWeekday_nighttime = (dtRentalBED[0].NumOfNightTimeWd != null ? dtRentalBED[0].NumOfNightTimeWd.Value.ToString("N0") : "-");
                    ViewBag.txtSaturday_daytime = (dtRentalBED[0].NumOfDayTimeSat != null ? dtRentalBED[0].NumOfDayTimeSat.Value.ToString("N0") : "-");
                    ViewBag.txtSaturday_nighttime = (dtRentalBED[0].NumOfNightTimeSat != null ? dtRentalBED[0].NumOfNightTimeSat.Value.ToString("N0") : "-");
                    ViewBag.txtSunday_daytime = (dtRentalBED[0].NumOfDayTimeSun != null ? dtRentalBED[0].NumOfDayTimeSun.Value.ToString("N0") : "-");
                    ViewBag.txtSunday_nighttime = (dtRentalBED[0].NumOfNightTimeSun != null ? dtRentalBED[0].NumOfNightTimeSun.Value.ToString("N0") : "-");
                    ViewBag.txtNumber_of_beat_guard_steps = (dtRentalBED[0].NumOfBeatStep != null ? dtRentalBED[0].NumOfBeatStep.Value.ToString("N0") : "-");
                    ViewBag.txtFrequency_of_gate_usage = (dtRentalBED[0].FreqOfGateUsage != null ? dtRentalBED[0].FreqOfGateUsage.Value.ToString("N0") : "-");
                    ViewBag.txtNumber_of_clock_key = (dtRentalBED[0].NumOfClockKey != null ? dtRentalBED[0].NumOfClockKey.Value.ToString("N0") : "-");

                    string strNumOfDate = (dtRentalBED[0].NumOfDate != null ? dtRentalBED[0].NumOfDate.Value.ToString() : "-");
                    string strNumOfDateDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_NUM_OF_DATE, strNumOfDate);
                    ViewBag.txtNumber_of_date = CommonUtil.TextCodeName(strNumOfDate, strNumOfDateDisplayValue);

                    ViewBag.txtNotify_time = (dtRentalBED[0].NotifyTime != null ?
                                                        string.Format("{0}:{1}", dtRentalBED[0].NotifyTime.Value.ToString("hh"), dtRentalBED[0].NotifyTime.Value.ToString("mm"))
                                                        : "-");
                }

                if (RentalSG.Count > 0)
                {
                    ViewBag.txtSecurity_item_fee = RentalSG[0].TextTransferSecurityItemFee;
                    ViewBag.txtOther_item_fee = RentalSG[0].TextTransferOtherItemFee;
                    //ViewBag.txtSecurity_item_fee = CommonUtil.TextNumeric(RentalSG[0].SecurityItemFee, 2) == string.Empty ? "-" : CommonUtil.TextNumeric(RentalSG[0].SecurityItemFee, 2);
                    //ViewBag.txtOther_item_fee = CommonUtil.TextNumeric(RentalSG[0].OtherItemFee, 2) == string.Empty ? "-" : CommonUtil.TextNumeric(RentalSG[0].OtherItemFee, 2);
                    ViewBag.txtSentry_guard_area_type = RentalSG[0].SentryGuardAreaType == string.Empty ? "-" : RentalSG[0].SentryGuardAreaType;

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
        /// Initial grid of screen CMS140 (maintenance contrct grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS140_IntialGridMaintenanceContract()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS140_MaintenanceContract"));
        }
        
        /// <summary>
        /// Initial grid of screen CMS140 (instrument detail grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS140_IntialGridInstrumentDetail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS140_InstrumentDetail"));
        }
        
        /// <summary>
        /// Initial grid of screen CMS140 (sentry guard detail grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS140_IntialSentryGuardDetail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS140_SentryGuardDetail"));
        }
        
        /// <summary>
        /// Initial grid of screen CMS140 (facility detail grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS140_IntialGridFacilityDeatail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS140_FacilityDeatail"));
        }
        
        /// <summary>
        /// Get maintenance contract target data list
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS140_GetMaintenanceContractTargetList(string strContractCode, string strOCC)
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

                //Add Currency to List
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

            }
            catch (Exception ex)
            {
                nlst = new List<View_dtRelatedContract>();
                res.AddErrorMessage(ex);

            }

            res.ResultData = CommonUtil.ConvertToXml<View_dtRelatedContract>(nlst, "Common\\CMS140_MaintenanceContract");
            return Json(res);


        }

        /// <summary>
        /// Get instrument detail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS140_GetInstrumentDetail(string strContractCode, string strOCC)
        {

            CommonUtil c = new CommonUtil();

            List<dtTbt_RentalInstrumentDetailsListForView> list = new List<dtTbt_RentalInstrumentDetailsListForView>();

            ObjectResultData res = new ObjectResultData();

            try
            {
                strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                list = handler.GetTbt_RentalInstrumentDetailsListForView(strContractCode, strOCC, null, InstrumentType.C_INST_TYPE_GENERAL);

                // Select language
                list = CommonUtil.ConvertObjectbyLanguage<dtTbt_RentalInstrumentDetailsListForView, dtTbt_RentalInstrumentDetailsListForView>(list, "InstrumentNameForCustomer");

                //return Json(CommonUtil.ConvertToXml<dtTbt_RentalInstrumentDetailsListForView>(list, "Common\\CMS140_InstrumentDetail"));

            }
            catch (Exception ex)
            {
                list = new List<dtTbt_RentalInstrumentDetailsListForView>();
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtTbt_RentalInstrumentDetailsListForView>(list, "Common\\CMS140_InstrumentDetail");
            return Json(res);


        }

        /// <summary>
        /// Get sentry guard detail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS140_GetSentryGuardDetail(string strContractCode, string strOCC)
        {

            CommonUtil c = new CommonUtil();

            List<dtTbt_RentalSentryGuardDetailsListForView> list = new List<dtTbt_RentalSentryGuardDetailsListForView>();

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

                list = handler.GetTbt_RentalSentryGuardDetailsListForView(strContractCode, strOCC, null);
                //Add Currency
                for (int i = 0; i < list.Count(); i++)
                {
                    if(list[i].TimeUnitPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        list[i].TimeUnitPrice = list[i].TimeUnitPriceUsd;
                    }

                    list[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<string> lsFieldNames = new List<string>();
                lsFieldNames.Add(MiscType.C_SG_TYPE);
                lsFieldNames.Add(MiscType.C_SG_AREA_TYPE);

                // Get Misc type
                List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);


                string strDisplayValue = "";
                foreach (var item in list)
                {
                    strDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList, MiscType.C_SG_TYPE, item.SentryGuardTypeCode);
                    item.SentryGuardType = strDisplayValue;//CommonUtil.TextCodeName(item.SentryGuardTypeCode, strDisplayValue);
                    item.strStartTime = (item.SecurityStartTime.HasValue ? string.Format("{0}:{1}", item.SecurityStartTime.Value.ToString("hh"), item.SecurityStartTime.Value.ToString("mm")) : "-");
                    item.strFinishTime = (item.SecurityFinishTime.HasValue ? string.Format("{0}:{1}", item.SecurityFinishTime.Value.ToString("hh"), item.SecurityFinishTime.Value.ToString("mm")) : "-");
                }




                // Order by first column (SentryGuardType)
                list = (from t in list orderby t.SentryGuardType select t).ToList<dtTbt_RentalSentryGuardDetailsListForView>();


            }
            catch (Exception ex)
            {
                list = new List<dtTbt_RentalSentryGuardDetailsListForView>();
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtTbt_RentalSentryGuardDetailsListForView>(list, "Common\\CMS140_SentryGuardDetail");
            return Json(res);

        }

        /// <summary>
        /// Get facility detail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS140_GetFacilityDetail(string strContractCode, string strOCC)
        {

            CommonUtil c = new CommonUtil();

            List<dtTbt_RentalInstrumentDetailsListForView> list = new List<dtTbt_RentalInstrumentDetailsListForView>();

            ObjectResultData res = new ObjectResultData();

            try
            {
                strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IRentralContractHandler handler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                list = handler.GetTbt_RentalInstrumentDetailsListForView(strContractCode, strOCC, null, InstrumentType.C_INST_TYPE_MONITOR);

                // Select language
                list = CommonUtil.ConvertObjectbyLanguage<dtTbt_RentalInstrumentDetailsListForView, dtTbt_RentalInstrumentDetailsListForView>(list, "InstrumentNameForCustomer");

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtTbt_RentalInstrumentDetailsListForView>(list, "Common\\CMS140_FacilityDeatail");
            return Json(res);


        }

    }
}
