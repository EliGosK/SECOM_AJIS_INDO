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
using SECOM_AJIS.DataEntity.Installation;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check permission for access screen CMS190
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS190_Authority(CMS190_ScreenParameter param) // IN parameter: string strContractCode, string strServiceTypeCode
        {
            ObjectResultData res = new ObjectResultData();


            // Check permission
            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_CONTRACT_DIGEST, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return Json(res);
            }

            if (String.IsNullOrEmpty(param.strContractCode))
            {
                //param.strContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                if (param.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                        param.strContractCode = param.CommonSearch.ContractCode;
                }
            }

            // Check parameter
            if (CommonUtil.IsNullOrEmpty(param.strContractCode) == true)
            {
                //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147);
                return Json(res);
            }

            // Check exist data
            try
            {
                CommonUtil c = new CommonUtil();
                string ContractCode = c.ConvertContractCode(param.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);


                // Rental
                IRentralContractHandler handlerR = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                // Sale
                ISaleContractHandler handlerS = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                //List<dtTbt_RentalContractBasicForView> dtRentalContract = new List<dtTbt_RentalContractBasicForView>();
                //List<dtTbt_SaleBasicForView> dtSaleContract = new List<dtTbt_SaleBasicForView>();

                List<tbt_RentalContractBasic> dtRentalContract = new List<tbt_RentalContractBasic>();
                List<tbt_SaleBasic> dtSaleContract = new List<tbt_SaleBasic>();

                // get data for check exist
                if (param.strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    dtRentalContract = handlerR.GetTbt_RentalContractBasic(ContractCode, null);
                }
                else if (param.strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    dtSaleContract = handlerS.GetTbt_SaleBasic(ContractCode, null, true);
                }
                else
                {
                    dtRentalContract = handlerR.GetTbt_RentalContractBasic(ContractCode, null);


                    if (dtRentalContract.Count == 0)
                    {
                        dtSaleContract = handlerS.GetTbt_SaleBasic(ContractCode, null, true);

                        param.strServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE;
                    }
                    else
                    {
                        param.strServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                    }
                }


                CommonUtil comUtil = new CommonUtil();
                
                if (dtRentalContract.Count > 0)
                {
                    param.strContractCode = comUtil.ConvertContractCode(dtRentalContract[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }
                else if (dtSaleContract.Count > 0)
                {
                    param.strContractCode = comUtil.ConvertContractCode(dtSaleContract[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }
                // parameter
                param.ContractCode = param.strContractCode;
                param.ServiceTypeCode = param.strServiceTypeCode;


                if (dtRentalContract.Count == 0 && dtSaleContract.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }

                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = param.strContractCode;
                //CommonUtil.dsTransData.dtCommonSearch.ProjectCode = null;
                param.CommonSearch = new ScreenParameter.CommonSearchDo()
                {
                    ContractCode = param.strContractCode
                };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<CMS190_ScreenParameter>("CMS190", param, res);

        }

        /// <summary>
        /// Method for return view of screen CMS190
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS190")]
        public ActionResult CMS190()
        {

            string strContractCode = "";
            string strServiceTypeCode = "";

            try
            {
                CMS190_ScreenParameter param = GetScreenObject<CMS190_ScreenParameter>();
                strContractCode = param.ContractCode;
                strServiceTypeCode = param.ServiceTypeCode;
            }
            catch
            {
            }

            /* ----- Set grobal variable for javascript side ---- */
            ViewBag._ServiceTypeCode = strServiceTypeCode;
            ViewBag._ContractCode = strContractCode;

            CommonUtil c = new CommonUtil();


            List<dtTbt_RentalContractBasicForView> vw_dtRentalContract = new List<dtTbt_RentalContractBasicForView>();
            List<dtTbt_SaleBasicForView> vw_dtSaleContract = new List<dtTbt_SaleBasicForView>();
            List<dtTbt_RentalSecurityBasicForView> vw_dtRentalSecurity = new List<dtTbt_RentalSecurityBasicForView>();
            //List<dtContractTargetInfoByRelated> vw_dtMaintenanceContractInfo = new List<dtContractTargetInfoByRelated>();

            ViewBag.Currency = CommonValue.CURRENCY_UNIT;
            string dateFormat = "dd-MMM-yyyy";
            string numberFormat = "N0";
            string floatNumberFormat = "N2";

            // default ViewBag
            ViewBag.chkFire_monitoring = false;
            ViewBag.chkCrime_prevention = false;
            ViewBag.chkEmergency_report = false;
            ViewBag.chkFacility_monitoring = false;
            ViewBag.bOutOfRegulationDocFlag = false;



            try
            {
                strContractCode = c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                ICommonHandler handlerComm = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                /******  get misc display value ******/
                List<string> lsFieldNames = new List<string>();
                lsFieldNames.Add(MiscType.C_RENTAL_CHANGE_TYPE);
                lsFieldNames.Add(MiscType.C_SALE_CHANGE_TYPE);
                lsFieldNames.Add(MiscType.C_SALE_TYPE);
                lsFieldNames.Add(MiscType.C_SALE_PROC_MANAGE_STATUS);

                List<doMiscTypeCode> MiscTypeList = handlerComm.GetMiscTypeCodeListByFieldName(lsFieldNames);
                /************************************/



                // Rental
                IRentralContractHandler handlerR = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                // Sale
                ISaleContractHandler handlerS = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                // Online
                IViewContractHandler handlerVC = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;

                IInstallationHandler iHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                string strInstallationStatusCode = "-";
                string strInstallationStatusName = "-";

                List<dtTbt_RentalContractBasicForView> dtRentalContract = new List<dtTbt_RentalContractBasicForView>();
                List<dtTbt_SaleBasicForView> dtSaleContract = new List<dtTbt_SaleBasicForView>();
                List<dtTbt_RentalSecurityBasicForView> dtRentalSecurity = new List<dtTbt_RentalSecurityBasicForView>();
                List<dtContractTargetInfoByRelated> dtOnlineContractInfo = new List<dtContractTargetInfoByRelated>();
                List<dtContractTargetInfoByRelated> dtMaintenanceContractInfo = new List<dtContractTargetInfoByRelated>();
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    dtRentalContract = handlerR.GetTbt_RentalContractBasicForView(strContractCode);
                    //Check Currency *For view
                    if(dtRentalContract[0].NormalDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalContract[0].NormalDepositFee = dtRentalContract[0].NormalDepositFeeUsd;
                    }
                    if (dtRentalContract[0].OrderDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalContract[0].OrderDepositFee = dtRentalContract[0].OrderDepositFeeUsd;
                    }
                    if (dtRentalContract[0].LastOrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalContract[0].LastOrderContractFee = dtRentalContract[0].LastOrderContractFeeUsd;
                    }
                    if (dtRentalContract[0].ExemptedDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        dtRentalContract[0].ExemptedDepositFee = dtRentalContract[0].ExemptedDepositFeeUsd;
                    }
                    //Add currency to List
                    for (int i = 0; i < dtRentalContract.Count(); i++)
                    {
                        dtRentalContract[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }
                }
                else if (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    dtSaleContract = handlerS.GetTbt_SaleBasicForView(strContractCode, null, true);
                    if (dtSaleContract[0].NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        dtSaleContract[0].NormalInstallFee = dtSaleContract[0].NormalInstallFeeUsd;
                    if (dtSaleContract[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        dtSaleContract[0].OrderInstallFee = dtSaleContract[0].OrderInstallFeeUsd;
                    if (dtSaleContract[0].InstallFeePaidBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        dtSaleContract[0].InstallFeePaidBySECOM = dtSaleContract[0].InstallFeePaidBySECOMUsd;
                    if (dtSaleContract[0].InstallFeeRevenueBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        dtSaleContract[0].InstallFeeRevenueBySECOM = dtSaleContract[0].InstallFeeRevenueBySECOMUsd;
                    if (dtSaleContract[0].OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        dtSaleContract[0].OrderProductPrice = dtSaleContract[0].OrderProductPriceUsd;
                    if (dtSaleContract[0].NormalProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        dtSaleContract[0].NormalProductPrice = dtSaleContract[0].NormalProductPriceUsd;
                    if (dtSaleContract[0].BidGuaranteeAmount1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        dtSaleContract[0].BidGuaranteeAmount1 = dtSaleContract[0].BidGuaranteeAmount1Usd;
                    if (dtSaleContract[0].BidGuaranteeAmount2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        dtSaleContract[0].BidGuaranteeAmount2 = dtSaleContract[0].BidGuaranteeAmount2Usd;
                    if (dtSaleContract[0].NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        dtSaleContract[0].NewBldMgmtCost = dtSaleContract[0].NewBldMgmtCostUsd;
                    //Add Currency to List
                    for (int i = 0; i < dtSaleContract.Count(); i++)
                    {
                        dtSaleContract[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }
                }


                if (dtRentalContract.Count > 0)
                {
                    // Get related data
                    dtRentalSecurity = handlerR.GetTbt_RentalSecurityBasicForView(strContractCode, dtRentalContract[0].LastOCC);

                    for (int i = 0; i < dtRentalSecurity.Count(); i++)
                    {
                        dtRentalSecurity[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }
                    //Must get InstallationInterfaceHandler.GetInstallationStatus (Next phase)
                    strInstallationStatusCode = iHandler.GetInstallationStatus(strContractCode);
                    //======== GET INSTALLATION STATUS NAME =============================
                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                    {
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_INSTALL_STATUS,
                            ValueCode = strInstallationStatusCode
                        }
                    };
                    lst = chandler.GetMiscTypeCodeList(miscs);
                    if (lst != null && lst.Count > 0)
                    {
                        strInstallationStatusName = lst[0].ValueDisplay;
                    }
                    //================================================================

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
                    foreach (var item in vw_dtRentalContract)
                    {
                        // contract code
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        item.OldContractCode = c.ConvertContractCode(item.OldContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        // customer code
                        item.ContractTargetCustCode = c.ConvertCustCode(item.ContractTargetCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        item.RealCustomerCustCode = c.ConvertCustCode(item.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);


                        //site code
                        item.SiteCode = c.ConvertSiteCode(item.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                    }

                    // dtOnlineContractInfo
                    foreach (var item in dtOnlineContractInfo)
                    {
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }


                }

                // Sale
                if (dtSaleContract.Count > 0)
                {
                    // Get related data
                    //dtOnlineContractInfo = handlerVC.GetContractTargetInfoByRelated(strContractCode, RelationType.C_RELATION_TYPE_SALE, dtSaleContract[0].OCC, ProductType.C_PROD_TYPE_SALE, ProductType.C_PROD_TYPE_AL, ProductType.C_PROD_TYPE_ONLINE, dtSaleContract[0].ProductTypeCode,
                    //                                                                 RelationType.C_RELATION_TYPE_MA, RelationType.C_RELATION_TYPE_SALE);
                    //dtMaintenanceContractInfo = handlerVC.GetContractTargetInfoByRelated(strContractCode, RelationType.C_RELATION_TYPE_MA, dtSaleContract[0].OCC, ProductType.C_PROD_TYPE_SALE, ProductType.C_PROD_TYPE_AL, ProductType.C_PROD_TYPE_ONLINE, dtSaleContract[0].ProductTypeCode,
                    //                                                                     RelationType.C_RELATION_TYPE_MA, RelationType.C_RELATION_TYPE_SALE);
                    dtOnlineContractInfo = handlerVC.GetContractTargetInfoByRelated(strContractCode, RelationType.C_RELATION_TYPE_SALE, dtSaleContract[0].OCC, dtSaleContract[0].ProductTypeCode);
                    dtMaintenanceContractInfo = handlerVC.GetContractTargetInfoByRelated(strContractCode, RelationType.C_RELATION_TYPE_MA, dtSaleContract[0].OCC, dtSaleContract[0].ProductTypeCode);


                    //Must get InstallationInterfaceHandler.GetInstallationStatus (Next phase)
                    strInstallationStatusCode = iHandler.GetInstallationStatus(strContractCode);
                    //======== GET INSTALLATION STATUS NAME =============================
                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                    {
                        new doMiscTypeCode()
                        {
                            FieldName = MiscType.C_INSTALL_STATUS,
                            ValueCode = strInstallationStatusCode
                        }
                    };
                    lst = chandler.GetMiscTypeCodeList(miscs);
                    if (lst != null && lst.Count > 0)
                    {
                        strInstallationStatusName = lst[0].ValueDisplay;
                    }
                    //================================================================

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




                    /**** Convert code to short format *****/

                    // vw_dtSaleContract
                    foreach (var item in vw_dtSaleContract)
                    {
                        // contract code
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        // customer code

                        item.CustCode_PurCust = c.ConvertCustCode(item.CustCode_PurCust, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        item.CustCode_RealCust = c.ConvertCustCode(item.CustCode_RealCust, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        item.RealCustomerCustCode = c.ConvertCustCode(item.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                        // site code
                        item.SiteCode = c.ConvertSiteCode(item.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }


                    // dtMaintenanceContractInfo
                    foreach (var item in dtMaintenanceContractInfo)
                    {
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }

                    // Online contract
                    foreach (var item in dtOnlineContractInfo)
                    {
                        item.ContractCode = c.ConvertContractCode(item.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }
                }


                // Rental
                if (vw_dtRentalContract.Count > 0)
                {
                    /*  --- global variable for javascript side --*/

                    ViewBag._ContractCode = vw_dtRentalContract[0].ContractCode;
                    ViewBag._OCC = vw_dtRentalContract[0].LastOCC;
                    ViewBag._ContractTargetCode = vw_dtRentalContract[0].ContractTargetCustCode;
                    ViewBag._RealCustomerCode = vw_dtRentalContract[0].RealCustomerCustCode;
                    ViewBag._SiteCode = vw_dtRentalContract[0].SiteCode;
                    ViewBag._ServiceTypeCode = vw_dtRentalContract[0].ServiceTypeCode;
                    ViewBag._OldContractCode = vw_dtRentalContract[0].OldContractCode;

                    ViewBag.ProductTypeCode = vw_dtRentalContract[0].ProductTypeCode;
                    ViewBag.SiteCodeList = vw_dtRentalContract[0].SiteCode;
                    ViewBag.ContractCode = vw_dtRentalContract[0].ContractCode;

                    ViewBag.txtContractCode = vw_dtRentalContract[0].ContractCode;
                    ViewBag.txtOccurrence = vw_dtRentalContract[0].LastOCC;
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

                    //Add by Jutarat A. on 26032014
                    ViewBag.txtCustomerContact = vw_dtRentalContract[0].ContactPersonName_Cust;
                    ViewBag.txtTelephoneNoCust = vw_dtRentalContract[0].PhoneNo_Cust;
                    ViewBag.txtFaxNo = vw_dtRentalContract[0].FaxNo_Cust;

                    ViewBag.txtPersonInCharge = vw_dtRentalContract[0].PersonInCharge_Site;
                    ViewBag.txtTelephoneNoSite = vw_dtRentalContract[0].PhoneNo_Site;
                    //End Add

                    //ViewBag.txtContactPoint = vw_dtRentalContract[0].ContactPoint;
                    ViewBag.txtContactPoint = CommonUtil.IsNullOrEmpty(vw_dtRentalContract[0].ContactPoint) == true ? "-" : vw_dtRentalContract[0].ContactPoint;



                    // section: contract detail info

                    //ViewBag.txtReceived_deposit_fee = vw_dtRentalContract[0].BilledDepositFee != null ? vw_dtRentalContract[0].BilledDepositFee.Value.ToString(floatNumberFormat) : "-";
                    // SA request to chagnge :31/Jan/2012  ref. UCCM1-8
                    //ViewBag.txtReceived_deposit_fee = vw_dtRentalContract[0].PaidDepositFee.HasValue ? vw_dtRentalContract[0].PaidDepositFee.Value.ToString(floatNumberFormat) : "-";
                    // SA request to chagnge#2 :15/Mar/2012 ref. UCCM1-75
                    ViewBag.txtReceived_deposit_fee = vw_dtRentalContract[0].TextTransferOrderDepositFee;
                    //ViewBag.txtReceived_deposit_fee = vw_dtRentalContract[0].OrderDepositFee.HasValue ? vw_dtRentalContract[0].OrderDepositFee.Value.ToString(floatNumberFormat) : "-";


                    ViewBag.txtOperation_office = CommonUtil.TextCodeName(vw_dtRentalContract[0].OperationOfficeCode, vw_dtRentalContract[0].Op_OfficeName);
                    ViewBag.txtFirst_operation_date = vw_dtRentalContract[0].FirstSecurityStartDate != null ? vw_dtRentalContract[0].FirstSecurityStartDate.Value.ToString(dateFormat) : "-";
                    ViewBag.txtContract_fee = vw_dtRentalContract[0].TextTransferLastOrderContractFee;
                    //ViewBag.txtContract_fee = vw_dtRentalContract[0].LastOrderContractFee != null ? vw_dtRentalContract[0].LastOrderContractFee.Value.ToString(floatNumberFormat) : "-";
                    ViewBag.txtContract_office = CommonUtil.TextCodeName(vw_dtRentalContract[0].OfficeCode_Con, vw_dtRentalContract[0].Con_OfficeName);
                    ViewBag.txtLast_operation_date = vw_dtRentalContract[0].LastChangeImplementDate != null ? vw_dtRentalContract[0].LastChangeImplementDate.Value.ToString(dateFormat) : "-";
                    //ViewBag.txtProcessing_installation = "-";  Get value from InstallInterfaceHandler.GetInstallationSatatus (ContractCode)

                    if (strInstallationStatusCode != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                    {
                        ViewBag.txtProcessing_installation = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS190", "lblYes");
                    }
                    else
                    {
                        ViewBag.txtProcessing_installation = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS190", "lblNo");
                    }
                    //ViewBag.txtProcessing_installation = strInstallationStatusCode+" : "+strInstallationStatusName;

                    string strLastChangeTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                                                            MiscType.C_RENTAL_CHANGE_TYPE,
                                                                                            vw_dtRentalContract[0].LastChangeType);
                    ViewBag.txtLast_change_type = CommonUtil.TextCodeName(vw_dtRentalContract[0].LastChangeType, strLastChangeTypeDisplayValue);
                    //ViewBag.txtProcessing_installation_status = "-"; Get value from InstallInterfaceHandler.GetInstallationSatatus (ContractCode)
                    ViewBag.txtProcessing_installation_status = strInstallationStatusCode + " : " + strInstallationStatusName;

                    ViewBag.lnkOld_contract_codeC = vw_dtRentalContract[0].OldContractCode;


                    //** // Change bOutOfRegulationDocFlag  --> to RentalContract[0].IrregurationDocUsageFlag (old: vw_dtRentalSecurity[0].DocAuditResult )
                    ViewBag.bOutOfRegulationDocFlag = vw_dtRentalContract[0].IrregurationDocUsageFlag.HasValue == true ? vw_dtRentalContract[0].IrregurationDocUsageFlag.Value : false;

                    ViewBag.txtRentalAttachImportanceFlag = vw_dtRentalContract[0].SpecialCareFlag;
                }

                if (vw_dtRentalSecurity.Count > 0)
                {
                    // section: contract detail info
                    ViewBag.txtProduct = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].ProductCode, vw_dtRentalSecurity[0].ProductName);
                    ViewBag.txtSecurity_type_code = vw_dtRentalSecurity[0].SecurityTypeCode;

                    // ContractStartDate - ContractEndDate
                    string strContractStartDate = vw_dtRentalSecurity[0].ContractStartDate != null ? vw_dtRentalSecurity[0].ContractStartDate.Value.ToString(dateFormat) : string.Empty;
                    //string strContractEndDate = vw_dtRentalSecurity[0].ContractEndDate != null ? vw_dtRentalSecurity[0].ContractEndDate.Value.ToString(dateFormat) : string.Empty;
                    string strContractEndDate = vw_dtRentalSecurity[0].CalContractEndDate != null ? vw_dtRentalSecurity[0].CalContractEndDate.Value.ToString(dateFormat) : string.Empty;

                    ViewBag.strContractStartDate = strContractStartDate;
                    ViewBag.strContractEndDate = strContractEndDate;
                    if (CommonUtil.IsNullOrEmpty(strContractStartDate) == false && CommonUtil.IsNullOrEmpty(strContractEndDate) == false)
                    {
                        ViewBag.txtContract_duration = string.Format("{0} <span style='color:black;'> ~ </span> {1}", strContractStartDate, strContractEndDate);
                    }
                    else if (CommonUtil.IsNullOrEmpty(strContractStartDate) == false)
                    {
                        ViewBag.txtContract_duration = string.Format("{0} <span style='color:black;'> ~ </span> {1}", strContractStartDate,  "-");
                    }
                    else if (CommonUtil.IsNullOrEmpty(strContractEndDate) == false)
                    {
                        ViewBag.txtContract_duration = string.Format("{0} <span style='color:black;'> ~ </span> {1}", "-", strContractEndDate);
                    }
                    else
                    {
                        ViewBag.txtContract_duration = string.Empty;
                    }



                    ViewBag.txtSalesman1 = CommonUtil.TextCodeName(vw_dtRentalSecurity[0].SalesmanEmpNo1, string.Format("{0} {1}", vw_dtRentalSecurity[0].SalesMan1_EmpFirstName, vw_dtRentalSecurity[0].SalesMan1_EmpLastName)); ;
                    ViewBag.txtContract_duration_month = vw_dtRentalSecurity[0].ContractDurationMonth != null ? vw_dtRentalSecurity[0].ContractDurationMonth.Value.ToString(numberFormat) : "-";
                    ViewBag.txtAuto_renew_months = vw_dtRentalSecurity[0].AutoRenewMonth != null ? vw_dtRentalSecurity[0].AutoRenewMonth.Value.ToString(numberFormat) : "-";

                    // Change bOutOfRegulationDocFlag  --> to RentalContract[0].IrregurationDocUsageFlag
                    //ViewBag.bOutOfRegulationDocFlag = vw_dtRentalSecurity[0].DocAuditResult == "1" ? true : false  ;

                    ViewBag.chkFire_monitoring = vw_dtRentalSecurity[0].FireMonitorFlag.HasValue == true ? vw_dtRentalSecurity[0].FireMonitorFlag.Value : false;
                    ViewBag.chkCrime_prevention = vw_dtRentalSecurity[0].CrimePreventFlag.HasValue == true ? vw_dtRentalSecurity[0].CrimePreventFlag.Value : false; ;
                    ViewBag.chkEmergency_report = vw_dtRentalSecurity[0].EmergencyReportFlag.HasValue == true ? vw_dtRentalSecurity[0].EmergencyReportFlag.Value : false; ;
                    ViewBag.chkFacility_monitoring = vw_dtRentalSecurity[0].FacilityMonitorFlag.HasValue == true ? vw_dtRentalSecurity[0].FacilityMonitorFlag.Value : false; ;

                }

                // Sale
                if (vw_dtSaleContract.Count > 0)
                {
                    /*  --- global variable for javascript side --*/
                    ViewBag._ContractCode = vw_dtSaleContract[0].ContractCode;
                    ViewBag._OCC = vw_dtSaleContract[0].OCC;
                    ViewBag._PurchaserCustCode = vw_dtSaleContract[0].CustCode_PurCust;
                    ViewBag._RealCustomerCode = vw_dtSaleContract[0].RealCustomerCustCode;
                    ViewBag._SiteCode = vw_dtSaleContract[0].SiteCode;
                    ViewBag._ServiceTypeCode = vw_dtSaleContract[0].ServiceTypeCode;
                    ViewBag.SiteCodeList = vw_dtSaleContract[0].SiteCode;
                    ViewBag.ContractCode = vw_dtSaleContract[0].ContractCode;
                    ViewBag.ProductTypeCode = vw_dtSaleContract[0].ProductTypeCode;

                    ViewBag.txtContractCode = vw_dtSaleContract[0].ContractCode;
                    ViewBag.txtOccurrence = vw_dtSaleContract[0].OCC;

                    ViewBag.lnkCustomerCodeC_Purchaser = vw_dtSaleContract[0].CustCode_PurCust;
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

                    //Add by Jutarat A. on 26032014
                    ViewBag.txtCustomerContact = vw_dtSaleContract[0].ContactPersonName_PurCust;
                    ViewBag.txtTelephoneNoCust = vw_dtSaleContract[0].PhoneNo_PurCust;
                    ViewBag.txtFaxNo = vw_dtSaleContract[0].FaxNo_PurCust;

                    ViewBag.txtPersonInCharge = vw_dtSaleContract[0].PersonInCharge_site;
                    ViewBag.txtTelephoneNoSite = vw_dtSaleContract[0].PhoneNo_site;
                    //End Add

                    //ViewBag.txtContactPoint = vw_dtSaleContract[0].ContactPoint;
                    ViewBag.txtContactPoint = CommonUtil.IsNullOrEmpty(vw_dtSaleContract[0].ContactPoint) == true ? "-" : vw_dtSaleContract[0].ContactPoint;


                    // section : sale contract detail info
                    ViewBag.txtProduct = CommonUtil.TextCodeName(vw_dtSaleContract[0].ProductCode, vw_dtSaleContract[0].ProductName);

                    string strSaleTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                                                    MiscType.C_SALE_TYPE,
                                                                                    vw_dtSaleContract[0].SalesType);
                    ViewBag.txtSale_type = CommonUtil.TextCodeName(vw_dtSaleContract[0].SalesType, strSaleTypeDisplayValue);
                    ViewBag.txtProduct_Price = vw_dtSaleContract[0].TextTransferOrderProductPrice;
                    ViewBag.txtInstallation_Fee = vw_dtSaleContract[0].TextTransferOrderInstallFee;
                    //ViewBag.txtBilling_amount = vw_dtSaleContract[0].TextTransferOrderSalePrice;
                    //ViewBag.txtBilling_amount = vw_dtSaleContract[0].OrderSalePrice != null ? vw_dtSaleContract[0].OrderSalePrice.Value.ToString(floatNumberFormat) : "-";

                    string SaleProdessManagementStatusDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                                                    MiscType.C_SALE_PROC_MANAGE_STATUS,
                                                                                    vw_dtSaleContract[0].SaleProcessManageStatus);
                    ViewBag.txtProcess_management_status = CommonUtil.TextCodeName(vw_dtSaleContract[0].SaleProcessManageStatus, SaleProdessManagementStatusDisplayValue);
                    
                    ViewBag.txtLast_operation_date = vw_dtSaleContract[0].ChangeImplementDate != null ? vw_dtSaleContract[0].ChangeImplementDate.Value.ToString(dateFormat) : "-";
                   
                    //ViewBag.txtProcessing_installation = "-";Get value from InstallInterfaceHandler.GetInstallationSatatus (ContractCode)
                    if (strInstallationStatusCode != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                    {
                        ViewBag.txtProcessing_installation = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS190", "lblYes");
                    }
                    else
                    {
                        ViewBag.txtProcessing_installation = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CMS190", "lblNo");
                    }
                    //ViewBag.txtProcessing_installation = strInstallationStatusCode + " : " + strInstallationStatusName;

                    string strChangeTypeDisplayValue = handlerComm.GetMiscDisplayValue(MiscTypeList,
                                                                                    MiscType.C_SALE_CHANGE_TYPE,
                                                                                    vw_dtSaleContract[0].ChangeType);
                    ViewBag.txtLast_change_type = CommonUtil.TextCodeName(vw_dtSaleContract[0].ChangeType, strChangeTypeDisplayValue);
                    //ViewBag.txtProcessing_installation_status = "-"; Get value from InstallInterfaceHandler.GetInstallationSatatus (ContractCode)
                    ViewBag.txtProcessing_installation_status = strInstallationStatusCode + " : " + strInstallationStatusName;

                    ViewBag.txtApprove_date = vw_dtSaleContract[0].FirstContractDate != null ? vw_dtSaleContract[0].FirstContractDate.Value.ToString(dateFormat) : "-";
                    ViewBag.txtSalesman1 = CommonUtil.TextCodeName(vw_dtSaleContract[0].SalesmanEmpNo1, string.Format("{0} {1}", vw_dtSaleContract[0].SalesMan1_EmpFirstName, vw_dtSaleContract[0].SalesMan1_EmpLastName));

                    ViewBag.txtInstrument_stock_out_date = vw_dtSaleContract[0].InstrumentStockOutDate != null ? vw_dtSaleContract[0].InstrumentStockOutDate.Value.ToString(dateFormat) : "-";
                    ViewBag.txtContract_office = CommonUtil.TextCodeName(vw_dtSaleContract[0].ContractOfficeCode, vw_dtSaleContract[0].Con_OfficeName);

                    ViewBag.txtComplete_installation_date = vw_dtSaleContract[0].InstallCompleteDate != null ? vw_dtSaleContract[0].InstallCompleteDate.Value.ToString(dateFormat) : "-";
                    ViewBag.txtSales_office = CommonUtil.TextCodeName(vw_dtSaleContract[0].SalesOfficeCode, vw_dtSaleContract[0].Sale_OfficeName);
                    ViewBag.txtCustomer_acceptance_date = vw_dtSaleContract[0].CustAcceptanceDate != null ? vw_dtSaleContract[0].CustAcceptanceDate.Value.ToString(dateFormat) : "-";
                    ViewBag.txtOperation_office = CommonUtil.TextCodeName(vw_dtSaleContract[0].OperationOfficeCode, vw_dtSaleContract[0].Op_OfficeName);

                    ViewBag.txtSaleAttachImportanceFlag = vw_dtSaleContract[0].SpecialCareFlag;
                }

                if (dtOnlineContractInfo.Count > 0)
                {
                    ViewBag.lnkOnline_contract_code = dtOnlineContractInfo[0].ContractCode;
                    ViewBag._OnlineContractCode = dtOnlineContractInfo[0].ContractCode;
                }

                if (dtMaintenanceContractInfo.Count > 0)
                {
                    ViewBag.lnkMaintenance_contract_code = dtMaintenanceContractInfo[0].ContractCode;
                    ViewBag._MaintenanceContractCode = dtMaintenanceContractInfo[0].ContractCode;

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


    }
}
