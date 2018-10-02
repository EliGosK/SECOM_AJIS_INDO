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

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using System.Resources;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS160
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS160_Authority(CMS160_ScreenParameter param) // IN Parameter: string strContractCode, string strOCC 
        {

            ObjectResultData res = new ObjectResultData();

            // Check permission
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_SALE_CONTRACT, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            // Check parameter is OK?
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


                // Sale
                ISaleContractHandler handlerS = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                List<dtTbt_SaleBasicForView> dtSaleContract = new List<dtTbt_SaleBasicForView>();
                // get data
                if (CommonUtil.IsNullOrEmpty(param.strOCC)) // if strOCC is null or emptry LastOCCFlag = true
                {
                    dtSaleContract = handlerS.GetTbt_SaleBasicForView(ContractCode, null, true);
                }
                else
                {
                    dtSaleContract = handlerS.GetTbt_SaleBasicForView(ContractCode, param.strOCC, null);
                }

                if (dtSaleContract.Count == 0)
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



            return InitialScreenEnvironment<CMS160_ScreenParameter>("CMS160", param, res);
        }

        /// <summary>
        /// Method for return view of screen CMS160
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS160")]
        public ActionResult CMS160()
        {
            string strContractCode = "";
            string strOCC = "";

            try
            {
                CMS160_ScreenParameter param = GetScreenObject<CMS160_ScreenParameter>();
                strContractCode = param.ContractCode;
                strOCC = param.OCC;
            }
            catch
            {
            }

            CommonUtil c = new CommonUtil();
            strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            List<dtTbt_SaleBasicForView> vw_dtSaleContract = new List<dtTbt_SaleBasicForView>();
            List<dtMaintContractTargetInfoByRelated> vw_dtMaintenanceContractInfo = new List<dtMaintContractTargetInfoByRelated>();

            ViewBag.Currency = CommonValue.CURRENCY_UNIT;

            ViewBag.isShowMaintenanceContract = false;

            try
            {
                ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                /******  get misc display value ******/
                List<string> lsFieldNames = new List<string>();
                lsFieldNames.Add(MiscType.C_RENTAL_CHANGE_TYPE);
                lsFieldNames.Add(MiscType.C_SALE_CHANGE_TYPE);
                lsFieldNames.Add(MiscType.C_SALE_TYPE);
                lsFieldNames.Add(MiscType.C_SALE_PROC_MANAGE_STATUS);
                lsFieldNames.Add(MiscType.C_DOC_AUDIT_RESULT);
                lsFieldNames.Add(MiscType.C_DISTRIBUTED_TYPE);
                lsFieldNames.Add(MiscType.C_MOTIVATION_TYPE);
                lsFieldNames.Add(MiscType.C_MA_TARGET_PROD_TYPE);
                lsFieldNames.Add(MiscType.C_MA_TYPE);
                lsFieldNames.Add(MiscType.C_MA_FEE_TYPE);
                lsFieldNames.Add(MiscType.C_BUILDING_TYPE);
                lsFieldNames.Add(MiscType.C_SALE_INSTALL_TYPE);
                lsFieldNames.Add(MiscType.C_ACQUISITION_TYPE);
                lsFieldNames.Add(MiscType.C_NEW_BLD_MGMT_FLAG);

                List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);
                /************************************/

                /********* get employee code name list *********/
                List<doEmpCodeName> EmployeeList = handlerComm.GetEmployeeCodeNameList();

                // when get : handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, "500575");
                // return emp code name (by language)
                /**********************************************/


                // Sale
                ISaleContractHandler handlerS = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                // Online
                IViewContractHandler handlerVC = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                // declare
                List<dtTbt_SaleBasicForView> dtSaleContract = new List<dtTbt_SaleBasicForView>();
                List<dtMaintContractTargetInfoByRelated> dtMaintenanceContractInfo = new List<dtMaintContractTargetInfoByRelated>();
                List<dtContractTargetInfoByRelated> dtOnlineContractInfo = new List<dtContractTargetInfoByRelated>();

                // get data
                if (CommonUtil.IsNullOrEmpty(strOCC)) // if strOCC is null or emptry LastOCCFlag = true
                {
                    dtSaleContract = handlerS.GetTbt_SaleBasicForView(strContractCode, null, true);
                }
                else
                {
                    dtSaleContract = handlerS.GetTbt_SaleBasicForView(strContractCode, strOCC, null);
                }

                //Check Currency
                if (dtSaleContract[0].NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].NormalInstallFee = dtSaleContract[0].NormalInstallFeeUsd;
                }
                if (dtSaleContract[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].OrderInstallFee = dtSaleContract[0].OrderInstallFeeUsd;
                }
                if (dtSaleContract[0].InstallFeePaidBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].InstallFeePaidBySECOM = dtSaleContract[0].InstallFeePaidBySECOMUsd;
                }
                if (dtSaleContract[0].InstallFeeRevenueBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].InstallFeeRevenueBySECOM = dtSaleContract[0].InstallFeeRevenueBySECOMUsd;
                }
                if (dtSaleContract[0].OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].OrderProductPrice = dtSaleContract[0].OrderProductPriceUsd;
                }
                if (dtSaleContract[0].NormalProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].NormalProductPrice = dtSaleContract[0].NormalProductPriceUsd;
                }
                if (dtSaleContract[0].BidGuaranteeAmount1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].BidGuaranteeAmount1 = dtSaleContract[0].BidGuaranteeAmount1Usd;
                }
                if (dtSaleContract[0].BidGuaranteeAmount2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].BidGuaranteeAmount2 = dtSaleContract[0].BidGuaranteeAmount2Usd;
                }
                if (dtSaleContract[0].NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].NewBldMgmtCost = dtSaleContract[0].NewBldMgmtCostUsd;
                }

                //Add Currency to List
                for (int i = 0; i < dtSaleContract.Count(); i++)
                {
                    dtSaleContract[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                if (dtSaleContract.Count > 0)
                {
                    // get related data
                    //dtOnlineContractInfo = handlerVC.GetContractTargetInfoByRelated(strContractCode, RelationType.C_RELATION_TYPE_SALE, dtSaleContract[0].OCC, ProductType.C_PROD_TYPE_SALE, ProductType.C_PROD_TYPE_AL, ProductType.C_PROD_TYPE_ONLINE, dtSaleContract[0].ProductTypeCode,
                    //                                                                 RelationType.C_RELATION_TYPE_MA, RelationType.C_RELATION_TYPE_SALE);
                    //dtMaintenanceContractInfo = handlerVC.GetMaintContractTargetInfoByRelated(strContractCode, ProductType.C_PROD_TYPE_MA, MiscType.C_MA_TYPE, MiscType.C_MA_FEE_TYPE, RelationType.C_RELATION_TYPE_MA, dtSaleContract[0].OCC
                    //                            , ProductType.C_PROD_TYPE_SALE, ProductType.C_PROD_TYPE_AL, ProductType.C_PROD_TYPE_ONLINE, dtSaleContract[0].ProductTypeCode);
                    dtOnlineContractInfo = handlerVC.GetContractTargetInfoByRelated(strContractCode, RelationType.C_RELATION_TYPE_SALE, dtSaleContract[0].OCC, dtSaleContract[0].ProductTypeCode);
                    dtMaintenanceContractInfo = handlerVC.GetMaintContractTargetInfoByRelated(strContractCode, dtSaleContract[0].OCC, dtSaleContract[0].ProductTypeCode);


                    // select language
                    vw_dtSaleContract = CommonUtil.ConvertObjectbyLanguage<dtTbt_SaleBasicForView, dtTbt_SaleBasicForView>(dtSaleContract,
                                                                                                    "PurCust_CustName",
                                                                                                    "PurCust_CustFullName",
                                                                                                    "RealCust_CustName",
                                                                                                    "RealCust_CustFullName",
                                                                                                    "site_SiteName",
                                                                                                    "QuoEmp_EmpFirstName",
                                                                                                    "QuoEmp_EmpLastName",
                                                                                                    "DocTemp_DocumentName",
                                                                                                    "DocTemp_DocumentNoName",
                                                                                                    "PlanEmp_EmpFirstName",
                                                                                                    "PlanEmp_EmpLastName",
                                                                                                    "planAppEmp_EmpFirstName",
                                                                                                    "planAppEmp_EmpLastName",
                                                                                                    "PlanChkrEmp_EmpFirstName",
                                                                                                    "PlanChkrEmp_EmpLastName",
                                                                                                    "ProductName",
                                                                                                    "SalesMan1_EmpFirstName",
                                                                                                    "SalesMan1_EmpLastName",
                                                                                                    "Quo_OfficeName", "Con_OfficeName", "Sale_OfficeName", "Op_OfficeName");


                    vw_dtMaintenanceContractInfo = CommonUtil.ConvertObjectbyLanguage<dtMaintContractTargetInfoByRelated, dtMaintContractTargetInfoByRelated>(dtMaintenanceContractInfo,
                                                                                                    "MaintenanceTargetProductTypeName",
                                                                                                    "MaintenanceTypeName",
                                                                                                    "MaintenanceFeeTypeName");


                    /**** Convert code to short format *****/

                    // vw_dtSaleContract
                    foreach (var item in vw_dtSaleContract)
                    {
                        // contract code
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        item.DistributedOriginCode = c.ConvertContractCode(item.DistributedOriginCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        // customer code
                        item.CustCode_PurCust = c.ConvertCustCode(item.CustCode_PurCust, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        item.CustCode_RealCust = c.ConvertCustCode(item.CustCode_RealCust, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        item.RealCustomerCustCode = c.ConvertCustCode(item.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        // site code
                        item.SiteCode = c.ConvertSiteCode(item.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        // quatation target code
                        item.QuotationTargetCode = c.ConvertQuotationTargetCode(item.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }


                    // vw_dtMaintenanceContractInfo
                    foreach (var item in vw_dtMaintenanceContractInfo)
                    {
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                    // Online contract
                    foreach (var item in dtOnlineContractInfo)
                    {
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                } // end if dtSaleContract.Count > 0


                // vw_dtSaleContract
                if (vw_dtSaleContract.Count > 0)
                {
                    string dateFormat = "dd-MMM-yyyy";

                    // -- global variable --
                    ViewBag.ChangeType = vw_dtSaleContract[0].ChangeType;
                    //ViewBag.isShowMaintenanceContract = vw_dtSaleContract[0].MaintenanceContractFlag.HasValue == true ? vw_dtSaleContract[0].MaintenanceContractFlag.Value : false;
                    ViewBag.SiteCodeList = vw_dtSaleContract[0].SiteCode;
                    ViewBag.ContractCode = vw_dtSaleContract[0].ContractCode;

                    ViewBag._ContractCode = vw_dtSaleContract[0].ContractCode;
                    ViewBag._OCC = vw_dtSaleContract[0].OCC;
                    ViewBag._PurchaserCustCode = vw_dtSaleContract[0].CustCode_PurCust;
                    ViewBag._RealCustomerCode = vw_dtSaleContract[0].RealCustomerCustCode;
                    ViewBag._SiteCode = vw_dtSaleContract[0].SiteCode;

                    ViewBag._QuotationTargetCode = vw_dtSaleContract[0].QuotationTargetCode;
                    ViewBag._Alphabet = vw_dtSaleContract[0].Alphabet;
                    ViewBag._DistributedOriginCode = vw_dtSaleContract[0].DistributedOriginCode;
                    ViewBag._InstallationSlipNo = vw_dtSaleContract[0].InstallationSlipNo;
                    ViewBag._ServiceTypeCode = vw_dtSaleContract[0].ServiceTypeCode;


                    // Section : sale contracts basic
                    ViewBag.txtContractCode = vw_dtSaleContract[0].ContractCode;
                    ViewBag.txtOccurrence = vw_dtSaleContract[0].OCC;
                    ViewBag.lnkCustomerCodeC = vw_dtSaleContract[0].CustCode_PurCust;
                    ViewBag.lnkCustomerCodeR = vw_dtSaleContract[0].RealCustomerCustCode;
                    ViewBag.lnkSiteCode = vw_dtSaleContract[0].SiteCode;

                    ViewBag.txtContractNameEng = vw_dtSaleContract[0].PurCust_CustFullNameEN;
                    ViewBag.txtContractAddrEng = vw_dtSaleContract[0].AddressFullEN_PurCust;
                    ViewBag.txtSiteNameEng = vw_dtSaleContract[0].site_SiteNameEN;
                    ViewBag.txtSiteAddrEng = vw_dtSaleContract[0].AddressFullEN_site;

                    ViewBag.txtContractNameLocal = vw_dtSaleContract[0].PurCust_CustFullNameLC;
                    ViewBag.txtContractAddrLocal = vw_dtSaleContract[0].AddressFullLC_PurCust;
                    ViewBag.txtSiteNameLocal = vw_dtSaleContract[0].site_SiteNameLC;
                    ViewBag.txtSiteAddrLocal = vw_dtSaleContract[0].AddressFullLC_site;

                    //ViewBag.txtContactPoint = vw_dtSaleContract[0].ContactPoint;
                    ViewBag.txtContactPoint = CommonUtil.IsNullOrEmpty(vw_dtSaleContract[0].ContactPoint) == true ? "-" : vw_dtSaleContract[0].ContactPoint;


                    // Section : sale contract detail information
                    ViewBag.txtProduct = CommonUtil.TextCodeName(vw_dtSaleContract[0].ProductCode, vw_dtSaleContract[0].ProductName);
                    ViewBag.txtApprove_contract_date = CommonUtil.TextDate(vw_dtSaleContract[0].FirstContractDate);
                    ViewBag.txtContract_office = CommonUtil.TextCodeName(vw_dtSaleContract[0].ContractOfficeCode, vw_dtSaleContract[0].Con_OfficeName);

                    string strSaleTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_SALE_TYPE,
                                                        vw_dtSaleContract[0].SalesType);
                    ViewBag.txtSale_type = CommonUtil.TextCodeName(vw_dtSaleContract[0].SalesType, strSaleTypeDisplayValue);
                    ViewBag.txtOperation_office = CommonUtil.TextCodeName(vw_dtSaleContract[0].OperationOfficeCode, vw_dtSaleContract[0].Op_OfficeName);
                    ViewBag.txtQuotationNo = vw_dtSaleContract[0].QuotationNo;
                    ViewBag.txtPlan_code = vw_dtSaleContract[0].PlanCode;
                    ViewBag.txtSales_office = CommonUtil.TextCodeName(vw_dtSaleContract[0].SalesOfficeCode, vw_dtSaleContract[0].Sale_OfficeName);
                    ViewBag.lnkQuotation_code = (CommonUtil.IsNullOrEmpty(vw_dtSaleContract[0].QuotationTargetCode) == true || CommonUtil.IsNullOrEmpty(vw_dtSaleContract[0].Alphabet) == true) ? string.Empty : string.Format("{0}-{1}", vw_dtSaleContract[0].QuotationTargetCode, vw_dtSaleContract[0].Alphabet);
                    ViewBag.txtProject_code = vw_dtSaleContract[0].ProjectCode;

                    string strDocAuditResult = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_DOC_AUDIT_RESULT,
                                                        vw_dtSaleContract[0].DocAuditResult);
                    ViewBag.txtDocument_audit_result = CommonUtil.TextCodeName(vw_dtSaleContract[0].DocAuditResult, strDocAuditResult);

                    string strDistributedInstallType = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_DISTRIBUTED_TYPE,
                                                        vw_dtSaleContract[0].DistributedInstallTypeCode);
                    ViewBag.txtDistributed_type = CommonUtil.TextCodeName(vw_dtSaleContract[0].DistributedInstallTypeCode, strDistributedInstallType);
                    ViewBag.txtDocument_receive_date = CommonUtil.TextDate(vw_dtSaleContract[0].DocReceiveDate);
                    ViewBag.lnkDistributed_origin_code = vw_dtSaleContract[0].DistributedOriginCode;

                    string strChangeTypeName = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_SALE_CHANGE_TYPE,
                                                        vw_dtSaleContract[0].ChangeType);
                    ViewBag.txtChangeType = CommonUtil.TextCodeName(vw_dtSaleContract[0].ChangeType, strChangeTypeName);

                    // affer grid sale contract deatail info

                    ViewBag.txtBid_guarantee_amount1 = vw_dtSaleContract[0].TextTransferBidGuaranteeAmount1;
                    //ViewBag.txtBid_guarantee_amount1 = CommonUtil.TextNumeric(vw_dtSaleContract[0].BidGuaranteeAmount1, 2);
                    ViewBag.txtBid_guarantee_return_date1 = CommonUtil.TextDate(vw_dtSaleContract[0].BidGuaranteeReturnDate1);
                    ViewBag.txtBid_guarantee_amount2 = vw_dtSaleContract[0].TextTransferBidGuaranteeAmount2;
                    //ViewBag.txtBid_guarantee_amount2 = CommonUtil.TextNumeric(vw_dtSaleContract[0].BidGuaranteeAmount2, 2);
                    ViewBag.txtBid_guarantee_return_date2 = CommonUtil.TextDate(vw_dtSaleContract[0].BidGuaranteeReturnDate2);
                    ViewBag.txtApprove_no1_Sale = vw_dtSaleContract[0].ApproveNo1;
                    ViewBag.txtlblApprove_no2_Sale = vw_dtSaleContract[0].ApproveNo2;

                    string strMotivationType = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_MOTIVATION_TYPE,
                                                        vw_dtSaleContract[0].MotivationTypeCode);
                    ViewBag.txtPurchase_reason_type = CommonUtil.TextCodeName(vw_dtSaleContract[0].MotivationTypeCode, strMotivationType);

                    string strAcqisitionType = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_ACQUISITION_TYPE,
                                                        vw_dtSaleContract[0].AcquisitionTypeCode);
                    ViewBag.txtlblAcquisition_type = CommonUtil.TextCodeName(vw_dtSaleContract[0].AcquisitionTypeCode, strAcqisitionType);

                    ViewBag.txtIntroducer_code = vw_dtSaleContract[0].IntroducerCode;

                    // --- salesman ---

                    ViewBag.txtSalesman_1 = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].SalesmanEmpNo1);
                    ViewBag.txtSalesman_2 = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].SalesmanEmpNo2);
                    ViewBag.txtSalesman_3 = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].SalesmanEmpNo3);
                    ViewBag.txtSalesman_4 = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].SalesmanEmpNo4);
                    ViewBag.txtSalesman_5 = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].SalesmanEmpNo5);
                    ViewBag.txtSalesman_6 = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].SalesmanEmpNo6);
                    ViewBag.txtSalesman_7 = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].SalesmanEmpNo7);
                    ViewBag.txtSalesman_8 = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].SalesmanEmpNo8);
                    ViewBag.txtSalesman_9 = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].SalesmanEmpNo9);
                    ViewBag.txtSalesman_10 = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].SalesmanEmpNo10);


                    // Section : installation information

                    string strSaleProcessManageStatus = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_SALE_PROC_MANAGE_STATUS,
                                                        vw_dtSaleContract[0].SaleProcessManageStatus);
                    ViewBag.txtProcess_management_status = CommonUtil.TextCodeName(vw_dtSaleContract[0].SaleProcessManageStatus, strSaleProcessManageStatus);
                    ViewBag.lnkInstallation_slip_no_Install = vw_dtSaleContract[0].InstallationSlipNo;
                    ViewBag.txtExpect_complete_installation_date = CommonUtil.TextDate(vw_dtSaleContract[0].ExpectedInstallCompleteDate);
                    ViewBag.txtExpected_customer_acceptance_date = CommonUtil.TextDate(vw_dtSaleContract[0].ExpectedCustAcceptanceDate);

                    string strBuildingType = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_BUILDING_TYPE,
                                                        vw_dtSaleContract[0].BuildingTypeCode);
                    ViewBag.txtNew_old_building = CommonUtil.TextCodeName(vw_dtSaleContract[0].BuildingTypeCode, strBuildingType);
                    ViewBag.txtInstrument_stock_out_date = CommonUtil.TextDate(vw_dtSaleContract[0].InstrumentStockOutDate);
                    ViewBag.txtSubcontract_complete_installation_date = CommonUtil.TextDate(vw_dtSaleContract[0].SubcontractInstallCompleteDate);
                    ViewBag.txtLastOperationDate = CommonUtil.TextDate(vw_dtSaleContract[0].ChangeImplementDate); //Add by Jutarat A. on 20120706

                    string strNewBldMgmtFlag = (vw_dtSaleContract[0].NewBldMgmtFlag.HasValue == true ? (vw_dtSaleContract[0].NewBldMgmtFlag.Value == true ? "1" : "0") : "");
                    string strBuildingMgmtType = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                       MiscType.C_NEW_BLD_MGMT_FLAG,
                                                        strNewBldMgmtFlag);
                    ViewBag.txtNew_building_mgmt_type = CommonUtil.TextCodeName(strNewBldMgmtFlag, strBuildingMgmtType);
                    ViewBag.txtNew_building_mgmt_cost = vw_dtSaleContract[0].TextTransferNewBldMgmtCost;
                    //ViewBag.txtNew_building_mgmt_cost = CommonUtil.TextNumeric(vw_dtSaleContract[0].NewBldMgmtCost);
                    ViewBag.txtComplete_installation_date_Install = CommonUtil.TextDate(vw_dtSaleContract[0].InstallCompleteDate);
                    ViewBag.txtLastChangeType = vw_dtSaleContract[0].ChangeTypeCodeName; //Add by Jutarat A. on 20120706

                    ViewBag.txtCustomer_acceptance_date = CommonUtil.TextDate(vw_dtSaleContract[0].CustAcceptanceDate);
                    ViewBag.txtDelivery_document_receive_date = CommonUtil.TextDate(vw_dtSaleContract[0].DeliveryDocReceiveDate);


                    ViewBag.txtIE_in_charge = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].IEInchargeEmpNo); ;

                    // Section : Maintenance Info
                    ViewBag.txtStart_maintenance_date = CommonUtil.TextDate(vw_dtSaleContract[0].StartMaintenanceDate);
                    ViewBag.txtEnd_maintenance_date = CommonUtil.TextDate(vw_dtSaleContract[0].EndMaintenanceDate);

                    // Section : change installation after start operation

                    string strInstallationType = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_SALE_INSTALL_TYPE,
                                                        vw_dtSaleContract[0].InstallationTypeCode);
                    ViewBag.txtInstallation_type = CommonUtil.TextCodeName(vw_dtSaleContract[0].InstallationTypeCode, strInstallationType);
                    ViewBag.lnkInstallation_slip_no_Change = vw_dtSaleContract[0].InstallationSlipNo; ;
                    ViewBag.txtComplete_installation_date_Change = CommonUtil.TextDate(vw_dtSaleContract[0].InstallCompleteDate);
                    ViewBag.txtNormal_installation_fee = vw_dtSaleContract[0].TextTransferNormalInstallFee;
                    //ViewBag.txtNormal_installation_fee = CommonUtil.TextNumeric(vw_dtSaleContract[0].NormalInstallFee);
                    ViewBag.txtOrder_installation_fee = vw_dtSaleContract[0].TextTransferOrderInstallFee;
                    //ViewBag.txtOrder_installation_fee = CommonUtil.TextNumeric(vw_dtSaleContract[0].OrderInstallFee);
                    ViewBag.txtSECOM_payment = vw_dtSaleContract[0].TextTransferInstallFeePaidBySECOM;
                    //ViewBag.txtSECOM_payment = CommonUtil.TextNumeric(vw_dtSaleContract[0].InstallFeePaidBySECOM);
                    ViewBag.txtSECOM_revenue = vw_dtSaleContract[0].TextTransferInstallFeeRevenueBySECOM;
                    //ViewBag.txtSECOM_revenue = CommonUtil.TextNumeric(vw_dtSaleContract[0].InstallFeeRevenueBySECOM);
                    ViewBag.txtApprove_no1_Change = vw_dtSaleContract[0].ApproveNo1;
                    ViewBag.txtNegotiation_staff = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].NegotiationStaffEmpNo1);
                    ViewBag.txtApprove_no2_Change = vw_dtSaleContract[0].ApproveNo2;
                    ViewBag.txtComplete_registrant = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].InstallationCompleteEmpNo);



                    // Section : instrument detail

                    ViewBag.txtWarrantee_period_from = CommonUtil.TextDate(vw_dtSaleContract[0].WarranteeFrom);
                    ViewBag.txtWarrantee_period_to = CommonUtil.TextDate(vw_dtSaleContract[0].WarranteeTo);

                    // -- grid --

                    ViewBag.txtFQ_99 = CommonUtil.TextDate(vw_dtSaleContract[0].ContractConditionProcessDate);
                    ViewBag.txtFQ_registrant = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].ContractConditionProcessEmpNo);
                    ViewBag.txtCP_01 = CommonUtil.TextDate(vw_dtSaleContract[0].FirstContractDate);
                    ViewBag.txtMK_20 = CommonUtil.TextDate(vw_dtSaleContract[0].NewAddInstallCompleteProcessDate);
                    ViewBag.txtMK_registrant = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].NewAddInstallCompleteEmpNo);
                    ViewBag.txtCQ_31 = CommonUtil.TextDate(vw_dtSaleContract[0].DataCorrectionProcessDate);
                    ViewBag.txtCQ_31_registrant = handlerComm.GetEmplyeeDisplayCodeName(EmployeeList, vw_dtSaleContract[0].DataCorrectionProcessEmpNo); // old: .ContractConditionProcessEmpNo

                    ViewBag.txtAttachImportanceFlag = vw_dtSaleContract[0].SpecialCareFlag;
                    ViewBag.txtBICContractCode = vw_dtSaleContract[0].BICContractCode;
                    ViewBag.txtPaymentDate_Incen = (vw_dtSaleContract[0].PaymentDateIncentive != null ? vw_dtSaleContract[0].PaymentDateIncentive.Value.ToString(dateFormat) : string.Empty);

                    //Add by Jutarat A. on 17082012
                    string strConnectionContractCodeShort = c.ConvertContractCode(vw_dtSaleContract[0].ConnectTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.lnkConnectionContractCode = strConnectionContractCodeShort;
                    ViewBag._ConnectionContractCode = strConnectionContractCodeShort;
                    //End Add
                }

                // vw_dtMaintenanceContractInfo
                if (vw_dtMaintenanceContractInfo.Count > 0)
                {
                    // Addition DDS
                    ViewBag.isShowMaintenanceContract = true;

                    ViewBag.txtMaintenance_contractCode = vw_dtMaintenanceContractInfo[0].ContractCode;

                    string strMaintenanceTargetProductType = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_MA_TARGET_PROD_TYPE,
                                                        vw_dtMaintenanceContractInfo[0].MaintenanceTargetProductTypeCode);
                    ViewBag.txtMaintenanceTargetProduct = CommonUtil.TextCodeName(vw_dtMaintenanceContractInfo[0].MaintenanceTargetProductTypeCode, strMaintenanceTargetProductType);

                    string strMaintenanceType = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_MA_TYPE,
                                                        vw_dtMaintenanceContractInfo[0].MaintenanceTypeCode);
                    ViewBag.txtMaintenanceType = CommonUtil.TextCodeName(vw_dtMaintenanceContractInfo[0].MaintenanceTypeCode, strMaintenanceType);
                    ViewBag.txtMaintenanceCycleMonth = CommonUtil.TextNumeric(vw_dtMaintenanceContractInfo[0].MaintenanceCycle, 2);


                    int month = vw_dtMaintenanceContractInfo[0].MaintenanceContractStartMonth != null ? vw_dtMaintenanceContractInfo[0].MaintenanceContractStartMonth.Value : 1;
                    int year = vw_dtMaintenanceContractInfo[0].MaintenanceContractStartYear != null ? vw_dtMaintenanceContractInfo[0].MaintenanceContractStartYear.Value : 2000;

                    DateTime maDatetime = new DateTime(year, month, 1);

                    ViewBag.txtMaintenanceContractStart_month = maDatetime.ToString("MMM-yyyy");

                    string strMaintenanceFeetype = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                        MiscType.C_MA_FEE_TYPE,
                                                        vw_dtMaintenanceContractInfo[0].MaintenanceFeeTypeCode);
                    ViewBag.txtMaintenanceFeeType = strMaintenanceFeetype;
                    if (CommonUtil.IsNullOrEmpty(vw_dtMaintenanceContractInfo[0].MaintenanceMemo) == false)
                        ViewBag.txtMemo = vw_dtMaintenanceContractInfo[0].MaintenanceMemo;
                    //ViewBag.txtMemo = vw_dtMaintenanceContractInfo[0].MaintenanceMemo;

                    ViewBag.txtContract_duration_month = CommonUtil.TextNumeric(vw_dtMaintenanceContractInfo[0].ContractDurationMonth, 0);
                    ViewBag.txtEnd_contract_date = CommonUtil.TextDate(vw_dtMaintenanceContractInfo[0].ContractEndDate);
                    ViewBag.txtAuto_renew_month = CommonUtil.TextNumeric(vw_dtMaintenanceContractInfo[0].AutoRenewMonth, 0);
                }


                // dtOnlineContractInfo
                if (dtOnlineContractInfo.Count > 0)
                {
                    ViewBag.lnkOnline_contract_code = dtOnlineContractInfo[0].ContractCode;
                    ViewBag._OnlineContractCode = dtOnlineContractInfo[0].ContractCode;
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
        /// Initial grid of screen CMS160 (sub-contractor grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS160_IntialGridSubcontractor()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS160_SubContrctor"));
        }

        /// <summary>
        /// Initial grid of screen CMS160 (instrument detail grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS160_IntialGridInstrumentDetail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS160_InstrumentDetail"));
        }

        /// <summary>
        /// Initial grid of screen CMS160 (sub-contractor detail grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult CMS160_IntialGridSaleContractDetail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS160_SaleContractDetail"));
        }

        /// <summary>
        /// Get sub-contractor data list
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS160_GetSubContractList(string strContractCode, string strOCC)
        {

            CommonUtil c = new CommonUtil();

            List<dtTbt_SaleInstSubcontractorListForView> list = new List<dtTbt_SaleInstSubcontractorListForView>();

            ObjectResultData res = new ObjectResultData();

            try
            {
                strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                ISaleContractHandler handler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                list = handler.GetTbt_SaleInstSubcontractorListForView(strContractCode, strOCC, null);


                //return Json(CommonUtil.ConvertToXml<dtTbt_SaleInstSubcontractorListForView>(list, "Common\\CMS160_SubContrctor"));

            }
            catch (Exception ex)
            {
                list = new List<dtTbt_SaleInstSubcontractorListForView>();
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtTbt_SaleInstSubcontractorListForView>(list, "Common\\CMS160_SubContrctor");
            return Json(res);

        }

        /// <summary>
        /// Get instrument detail data list
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS160_GetInstrumentDetail(string strContractCode, string strOCC)
        {

            CommonUtil c = new CommonUtil();
            strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            List<dtSaleInstruDetailListForView> list = new List<dtSaleInstruDetailListForView>();

            ObjectResultData res = new ObjectResultData();

            try
            {
                IViewContractHandler handler = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                list = handler.GetSaleInstruDetailListForView(strContractCode, strOCC, InstrumentType.C_INST_TYPE_GENERAL);


                //return Json(CommonUtil.ConvertToXml<dtSaleInstruDetailListForView>(list, "Common\\CMS160_InstrumentDetail"));

            }
            catch (Exception ex)
            {
                list = new List<dtSaleInstruDetailListForView>();
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtSaleInstruDetailListForView>(list, "Common\\CMS160_InstrumentDetail");
            return Json(res);

        }

        /// <summary>
        /// get sale contract detail data list
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public ActionResult CMS160_GetSaleContractDetail(string strContractCode, string strOCC)
        {
            CommonUtil c = new CommonUtil();
            strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            List<doCMS160_SaleContractDetail> list = new List<doCMS160_SaleContractDetail>();

            ObjectResultData res = new ObjectResultData();

            try
            {
                // Sale
                ISaleContractHandler handlerS = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtTbt_SaleBasicForView> dtSaleContract = new List<dtTbt_SaleBasicForView>();

                // get data
                if (CommonUtil.IsNullOrEmpty(strOCC)) // if strOCC is null or emptry LastOCCFlag = true
                {
                    dtSaleContract = handlerS.GetTbt_SaleBasicForView(strContractCode, null, true);
                }
                else
                {
                    dtSaleContract = handlerS.GetTbt_SaleBasicForView(strContractCode, strOCC, null);
                }

                //Check Currency
                if (dtSaleContract[0].NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].NormalInstallFee = dtSaleContract[0].NormalInstallFeeUsd;
                }
                if (dtSaleContract[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].OrderInstallFee = dtSaleContract[0].OrderInstallFeeUsd;
                }
                if (dtSaleContract[0].InstallFeePaidBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].InstallFeePaidBySECOM = dtSaleContract[0].InstallFeePaidBySECOMUsd;
                }
                if (dtSaleContract[0].InstallFeeRevenueBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].InstallFeeRevenueBySECOM = dtSaleContract[0].InstallFeeRevenueBySECOMUsd;
                }
                if (dtSaleContract[0].OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].OrderProductPrice = dtSaleContract[0].OrderProductPriceUsd;
                }
                if (dtSaleContract[0].NormalProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].NormalProductPrice = dtSaleContract[0].NormalProductPriceUsd;
                }
                if (dtSaleContract[0].BidGuaranteeAmount1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].BidGuaranteeAmount1 = dtSaleContract[0].BidGuaranteeAmount1Usd;
                }
                if (dtSaleContract[0].BidGuaranteeAmount2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].BidGuaranteeAmount2 = dtSaleContract[0].BidGuaranteeAmount2Usd;
                }
                if (dtSaleContract[0].NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtSaleContract[0].NewBldMgmtCost = dtSaleContract[0].NewBldMgmtCostUsd;
                }

                //Add Currency to List
                for (int i = 0; i < dtSaleContract.Count(); i++)
                {
                    dtSaleContract[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                doCMS160_SaleContractDetail row1 = new doCMS160_SaleContractDetail();
                doCMS160_SaleContractDetail row2 = new doCMS160_SaleContractDetail();
                //doCMS160_SaleContractDetail row3 = new doCMS160_SaleContractDetail();

                if (dtSaleContract.Count > 0)
                {

                    // assign value for 3 rows
                    decimal normal = 0;
                    decimal order = 0;
                    decimal rate = 0;



                    // rate = (normal-order) / normal * 100

                    // Row1  // Product price
                    normal = dtSaleContract[0].NormalProductPrice != null ? dtSaleContract[0].NormalProductPrice.Value : 0;
                    order = dtSaleContract[0].OrderProductPrice != null ? dtSaleContract[0].OrderProductPrice.Value : 0;

                    row1.Title = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS160", "lblGrid_ProductPrice") + "<br/>(1)"; //"Product price";
                    row1.txtBillinAmt = dtSaleContract[0].TextTransferOrderProductPrice;
                    //row1.BillinAmt = dtSaleContract[0].OrderProductPrice;
                    row1.txtNormalAmt = dtSaleContract[0].TextTransferNormalProductPrice;
                    //row1.NormalAmt = dtSaleContract[0].NormalProductPrice;
            
                    if (normal > 0)
                    {
                        if (dtSaleContract[0].OrderProductPriceCurrencyType != dtSaleContract[0].NormalProductPriceCurrencyType)
                        {
                            rate = 0;
                            row1.DiscountName = (rate < 0 ? ReceivedRate.C_RECEIVED_RATE_OVER : (rate == 0 ? "-" : ReceivedRate.C_RECEIVED_RATE_DISCOUNT));
                            row1.DiscountRate = "-";
                        }
                        else
                        {
                            rate = ((normal - order) / normal) * 100;
                            row1.DiscountName = (rate < 0 ? ReceivedRate.C_RECEIVED_RATE_OVER : (rate == 0 ? "-" : ReceivedRate.C_RECEIVED_RATE_DISCOUNT));
                            row1.DiscountRate = string.Format("{0} %", Math.Abs(rate).ToString("N2"));
                        }               
                    }
                    row1.DiscountName = (rate < 0 ? ReceivedRate.C_RECEIVED_RATE_OVER : (rate == 0 ? "-" : ReceivedRate.C_RECEIVED_RATE_DISCOUNT));
                    row1.DiscountRate = string.Format("{0} %", Math.Abs(rate).ToString("N2"));


                    // Row2  // Installation fee
                    normal = dtSaleContract[0].NormalInstallFee != null ? dtSaleContract[0].NormalInstallFee.Value : 0;
                    order = dtSaleContract[0].OrderInstallFee != null ? dtSaleContract[0].OrderInstallFee.Value : 0;

                    row2.Title = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS160", "lblGrid_InstallationFee") + "<br/>(2)"; //"Installation fee";
                    row2.txtBillinAmt = dtSaleContract[0].TextTransferOrderInstallFee;
                    //row2.BillinAmt = dtSaleContract[0].OrderInstallFee;
                    row2.txtNormalAmt = dtSaleContract[0].TextTransferNormalInstallFee;
                    //row2.NormalAmt = dtSaleContract[0].NormalInstallFee;

                    if (normal > 0)
                    {
                        if (dtSaleContract[0].OrderInstallFeeCurrencyType != dtSaleContract[0].NormalInstallFeeCurrencyType)
                        {
                            rate = 0;
                            row2.DiscountName = (rate < 0 ? ReceivedRate.C_RECEIVED_RATE_OVER : (rate == 0 ? "-" : ReceivedRate.C_RECEIVED_RATE_DISCOUNT));
                            row2.DiscountRate = "-";
                        }
                        else
                        {
                            rate = ((normal - order) / normal) * 100;
                            row2.DiscountName = (rate < 0 ? ReceivedRate.C_RECEIVED_RATE_OVER : (rate == 0 ? "-" : ReceivedRate.C_RECEIVED_RATE_DISCOUNT));
                            row2.DiscountRate = string.Format("{0} %", Math.Abs(rate).ToString("N2"));
                        }
                    }
                    row2.DiscountName = (rate < 0 ? ReceivedRate.C_RECEIVED_RATE_OVER : (rate == 0 ? "-" : ReceivedRate.C_RECEIVED_RATE_DISCOUNT));
                    row2.DiscountRate = string.Format("{0} %", Math.Abs(rate).ToString("N2"));


                    // Row3  // Sale price
                    //normal = dtSaleContract[0].NormalSalePrice != null ? dtSaleContract[0].NormalSalePrice.Value : 0;
                    //order = dtSaleContract[0].OrderSalePrice != null ? dtSaleContract[0].OrderSalePrice.Value : 0;

                    //if (normal > 0)
                    //    rate = ((normal - order) / normal) * 100;

                    //row3.Title = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS160", "lblGrid_SalePrice") + "<br/>(1)+(2)";  //"Sale price";
                    //row3.BillinAmt = dtSaleContract[0].OrderSalePrice;
                    //row3.NormalAmt = dtSaleContract[0].NormalSalePrice;
                    //row3.DiscountName = (rate < 0 ? ReceivedRate.C_RECEIVED_RATE_OVER : (rate == 0 ? "-" : ReceivedRate.C_RECEIVED_RATE_DISCOUNT));
                    //row3.DiscountRate = string.Format("{0} %", Math.Abs(rate).ToString("N2"));

                }

                list.Add(row1);
                list.Add(row2);
                //list.Add(row3);


            }
            catch (Exception ex)
            {
                list = new List<doCMS160_SaleContractDetail>();
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<doCMS160_SaleContractDetail>(list, "Common\\CMS160_SaleContractDetail");
            return Json(res);
        }

    }
}