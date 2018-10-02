//*********************************
// Create by: Nattapong N.
// Create date: 8/Jul/2010
// Update date: 8/Jul/2010
//*********************************

using System.Linq;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Controllers
{
    public partial class CommonController : BaseController
    {
        /// <summary>
        /// Check user permission for screen CMS150 and check all parameter must have value.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CMS150_Authority(CMS150_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                //1. Check permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_VIEW_HISTORY_DIGEST, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //2. Check parameter is OK ?
                if (CommonUtil.IsNullOrEmpty(param.ContractCode) || CommonUtil.IsNullOrEmpty(param.ServiceTypeCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0040);
                    return Json(res);
                }

                CommonUtil ComUtil = new CommonUtil();
                string strLongContractCode = ComUtil.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                
               
                // ---- Wrong !! -- You must get these data when pass check Authority !!!
                //dsRentalBasicForHistDigestView dsRentBasicForHDView;
                //dsSaleBasicForHistDigestView dsSaleBasicForHDView;
                //if (param.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                //{
                //    dsRentBasicForHDView = GetRentalBasicForHistoryDigestView(strLongContractCode);
                //    param.dsRentBasicForHDView = dsRentBasicForHDView;
                //}
                //else
                //{
                //    dsSaleBasicForHDView = GetSaleBasicForHistoryDigestView(ComUtil.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.OCC);
                //    param.dsSaleBasicForHDView = dsSaleBasicForHDView;
                //}
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return InitialScreenEnvironment<CMS150_ScreenParameter>("CMS150", param, res);
        }

        /// <summary>
        /// Initial screen and get contract information.
        /// </summary>
        /// <returns></returns>
        [Initialize("CMS150")]
        public ActionResult CMS150()
        {
            CMS150_ScreenParameter param = GetScreenObject<CMS150_ScreenParameter>();
            ObjectResultData res = new ObjectResultData();
            if (param == null)
                param = new CMS150_ScreenParameter();
            
            //if (param.dsRentBasicForHDView == null)
            //    param.dsRentBasicForHDView = new dsRentalBasicForHistDigestView();
            //if (param.dsSaleBasicForHDView == null)
            //    param.dsSaleBasicForHDView = new dsSaleBasicForHistDigestView();


            dsRentalBasicForHistDigestView dsRentBasicForHDView;
            dsSaleBasicForHistDigestView dsSaleBasicForHDView;
            CommonUtil ComUtil = new CommonUtil();
            string strLongContractCode = ComUtil.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            // Get data
            if (param.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
            {
                dsRentBasicForHDView = GetRentalBasicForHistoryDigestView(strLongContractCode);
                param.dsRentBasicForHDView = dsRentBasicForHDView;
            }
            else
            {
                dsSaleBasicForHDView = GetSaleBasicForHistoryDigestView(strLongContractCode, param.OCC);
                param.dsSaleBasicForHDView = dsSaleBasicForHDView;
            }


            // Assign and use data
            if (param.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
            {
                dsRentBasicForHDView = param.dsRentBasicForHDView;
                //dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].ContractCode = ComUtil.ConvertContractCode(dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].ContractTargetCustCode = ComUtil.ConvertCustCode(dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].ContractTargetCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].RealCustomerCustCode = ComUtil.ConvertCustCode(dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].SiteCode = ComUtil.ConvertSiteCode(dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                dsRentBasicForHDView.View_dtRentalHistoryDigest = CommonUtil.ConvertObjectbyLanguage<dtRentalHistoryDigest, View_dtRentalHistoryDigest>(dsRentBasicForHDView.dtRentalHistoryDigest, "ChangeTypeName", "IncidentARTypeName", "DocAuditResultName", "DocumentName");

                if (dsRentBasicForHDView.dtTbt_RentalContractBasicForView != null && dsRentBasicForHDView.dtTbt_RentalContractBasicForView.Count > 0)
                {
                    ViewBag.ContractCode = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].ContractCodeShort;
                    ViewBag.UserCode = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].UserCode;
                    ViewBag.CustCodeCT = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].ContractTargetCustCodeShort;
                    ViewBag.CustCodeRC = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].RealCustomerCustCodeShort;
                    ViewBag.SiteCode = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].SiteCodeShort;
                    ViewBag.ContTGnamEn = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].CustFullNameEN_Cust;
                    ViewBag.ContTGaddrEn = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].AddressFullEN_Cust;

                    ViewBag.SiteNamEn = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].SiteNameEN_Site;
                    ViewBag.SiteAddrEn = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].AddressFullEN_Site;
                    ViewBag.ContTGnamLC = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].CustFullNameLC_Cust;
                    ViewBag.ContTGaddrL = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].AddressFullLC_Cust;
                    ViewBag.SiteNamLC = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].SiteNameLC_Site;
                    ViewBag.SiteAddrLC = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].AddressFullLC_Site;

                    ViewBag.ContPoint = CommonUtil.IsNullOrEmpty(dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].ContactPoint) == true ? "-" : dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].ContactPoint;

                    //ViewBag.ContPoint = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].ContactPoint;

                    ViewBag.ProductTypeCode = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].ProductTypeCode;

                    //for Rental
                    ViewBag.OCC = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].LastOCC;

                    ViewBag.txtRentalAttachImportanceFlag = dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].SpecialCareFlag;
                }

                ViewBag.EnableContractBasic = true;
            }
            else
            {
                dsSaleBasicForHDView = param.dsSaleBasicForHDView;
                //dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].ContractCode = ComUtil.ConvertContractCode(dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].PurchaserCustCode = ComUtil.ConvertCustCode(dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].PurchaserCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].RealCustomerCustCode = ComUtil.ConvertCustCode(dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].SiteCode = ComUtil.ConvertSiteCode(dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                dsSaleBasicForHDView.View_dtSaleHistoryDigestList = CommonUtil.ConvertObjectbyLanguage<dtSaleHistoryDigest, View_dtSaleHistoryDigestList>(dsSaleBasicForHDView.dtSaleHistoryDigestList, "ChangeTypeName", "IncidentARTypeName");

                if (dsSaleBasicForHDView.dtTbt_SaleBasicForView != null && dsSaleBasicForHDView.dtTbt_SaleBasicForView.Count > 0)
                {
                    ViewBag.ContractCode = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].ContractCode_Short;
                    ViewBag.CustCodePur = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].PurchaserCustCode_Short;
                    ViewBag.CustCodeRC = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].RealCustomerCustCode_Short;
                    ViewBag.SiteCode = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].SiteCode_Short;
                    ViewBag.PurNameEn = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].PurCust_CustFullNameEN;
                    ViewBag.PurAddrEn = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].AddressFullEN_PurCust;
                    ViewBag.SiteNamEn = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].site_SiteNameEN;

                    ViewBag.SiteAddrEn = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].AddressFullEN_site;
                    ViewBag.PurchaserNamLC = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].PurCust_CustFullNameLC;// CustFullNameLC_PurCust;
                    ViewBag.PurchaserAddrLC = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].AddressFullLC_PurCust;
                    ViewBag.SiteNamLC = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].site_SiteNameLC;
                    ViewBag.SiteAddrLC = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].AddressFullLC_site;
                    //ViewBag.ContPoint = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].ContactPoint;
                    ViewBag.ContPoint = CommonUtil.IsNullOrEmpty(dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].ContactPoint) == true ? "-" : dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].ContactPoint;
                    ViewBag.OCC = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].OCC;

                    ViewBag.txtSaleAttachImportanceFlag = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].SpecialCareFlag;

                    ViewBag.ProductTypeCode = dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].ProductTypeCode;
                }

                ViewBag.EnableSalesContractBasic = true;
            }

            //--------------- Viewbag share data----------
            ViewBag.C_SERVICE_TYPE_RENTAL = ServiceType.C_SERVICE_TYPE_RENTAL;
            ViewBag.strLongContractCode = ComUtil.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            ViewBag.CondOCC = param.OCC;
            ViewBag.ServiceType = param.ServiceTypeCode;
            ViewBag.C_CONTRACT_TYPE_CONTACT = ContractType.C_CONTRACT_TYPE_CONTACT;
            ViewBag.C_CONTRACT_TYPE_INCIDENT = ContractType.C_CONTRACT_TYPE_INCIDENT;
            ViewBag.C_CONTRACT_TYPE_AR = ContractType.C_CONTRACT_TYPE_AR;

            //ViewBag.EnableSecurityBasic = false;
            //ViewBag.EnableSecurityDetail = false;
            ViewBag.EnableHistoryDigest = false;


            ViewBag.EnableContractBillingTransfer = true;

            return View();


        }

        /// <summary>
        /// Get config for Rental table.
        /// </summary>
        /// <returns></returns>
        public ActionResult InitRentalGrid_CMS150()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS150"));
        }

        /// <summary>
        /// Get config for Sale table.
        /// </summary>
        /// <returns></returns>
        public ActionResult InitSaleGrid_CMS150()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Common\\CMS150_sale"));
        }

        /// <summary>
        /// Get Rental digest by type and contract code.
        /// </summary>
        /// <param name="sType"></param>
        /// <param name="strLongContractCode"></param>
        /// <returns></returns>
        public ActionResult getRentalDigestCMS150(string sType, string strLongContractCode)
        {
            if (strLongContractCode.Trim() == "")
                strLongContractCode = null;
            if (sType.Trim() == "")
                sType = null;


            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<View_dtRentalHistoryDigest> RentalHist = null;

            try
            {
                IRentralContractHandler handRen = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<dtTbt_RentalContractBasicForView> lstTbt_RentalContractBasicForView = handRen.GetTbt_RentalContractBasicForView(strLongContractCode);
                if (lstTbt_RentalContractBasicForView.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    // return Json(res);
                }
                else
                {
                    IViewContractHandler handView = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                    List<dtRentalHistoryDigest> dtRentalHistoryDigest = handView.GetRentalHistoryDigestList(lstTbt_RentalContractBasicForView[0].ContractCode, null, null);
                    RentalHist = CommonUtil.ConvertObjectbyLanguage<dtRentalHistoryDigest, View_dtRentalHistoryDigest>(dtRentalHistoryDigest, "ChangeTypeName", "IncidentARTypeName", "DocAuditResultName", "DocumentName");

                }
                res.ResultData = CommonUtil.ConvertToXml<View_dtRentalHistoryDigest>(RentalHist, "Common\\CMS150", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get Sale digest by type, contract code and OCC.
        /// </summary>
        /// <param name="sType"></param>
        /// <param name="strLongContractCode"></param>
        /// <param name="OCC"></param>
        /// <returns></returns>
        public ActionResult getSaleDigestCMS150(string sType, string strLongContractCode, string OCC)
        {
            if (OCC.Trim() == "")
                OCC = null;
            if (strLongContractCode.Trim() == "")
                strLongContractCode = null;
            if (sType.Trim() == "")
                sType = null;

            List<View_dtSaleHistoryDigestList> vSaleHist = null;
            ObjectResultData res = new ObjectResultData();
            try
            {
                ISaleContractHandler handSale = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();
                List<dtTbt_SaleBasicForView> dtTbt_SaleBasicForView = handSale.GetTbt_SaleBasicForView(strLongContractCode, OCC, FlagType.C_FLAG_ON);
                //Add Currency to List
                for (int i = 0; i < dtTbt_SaleBasicForView.Count(); i++)
                {
                    dtTbt_SaleBasicForView[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                if (dtTbt_SaleBasicForView.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    // return Json(res);
                }
                else
                {
                    IViewContractHandler handView = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                    List<dtSaleHistoryDigest> dtSaleHistoryDigest = handView.GetSaleHistoryDigestList(dtTbt_SaleBasicForView[0].ContractCode, null, null);
                    //Add Currency to List
                    for (int i = 0; i < dtSaleHistoryDigest.Count(); i++)
                    {
                        dtSaleHistoryDigest[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                    }
                    vSaleHist = CommonUtil.ConvertObjectbyLanguage<dtSaleHistoryDigest, View_dtSaleHistoryDigestList>(dtSaleHistoryDigest, "ChangeTypeName", "IncidentARTypeName");
                }

                string XmlSaleHist = CommonUtil.ConvertToXml<View_dtSaleHistoryDigestList>(vSaleHist, "Common\\CMS150_sale", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(XmlSaleHist);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }

        }

        /// <summary>
        /// Validate Contract information condition.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CMS150Valid(doContractInfoCondition Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                }
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get Sale Basic information from tbt_SaleBasic
        /// </summary>
        /// <param name="strLongContractCode"></param>
        /// <param name="OCC"></param>
        /// <returns></returns>
        public dsSaleBasicForHistDigestView GetSaleBasicForHistoryDigestView(string strLongContractCode, string OCC)
        {
            try
            {
                dsSaleBasicForHistDigestView dsSaleBasicForHDView = new dsSaleBasicForHistDigestView();
                ISaleContractHandler handSale = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtTbt_SaleBasicForView> dtTbt_SaleBasicForView = handSale.GetTbt_SaleBasicForView(strLongContractCode, OCC, FlagType.C_FLAG_ON);
                //Check Currency
                if (dtTbt_SaleBasicForView[0].NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtTbt_SaleBasicForView[0].NormalInstallFee = dtTbt_SaleBasicForView[0].NormalInstallFeeUsd;
                }
                if (dtTbt_SaleBasicForView[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtTbt_SaleBasicForView[0].OrderInstallFee = dtTbt_SaleBasicForView[0].OrderInstallFeeUsd;
                }
                if (dtTbt_SaleBasicForView[0].InstallFeePaidBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtTbt_SaleBasicForView[0].InstallFeePaidBySECOM = dtTbt_SaleBasicForView[0].InstallFeePaidBySECOMUsd;
                }
                if (dtTbt_SaleBasicForView[0].InstallFeeRevenueBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtTbt_SaleBasicForView[0].InstallFeeRevenueBySECOM = dtTbt_SaleBasicForView[0].InstallFeeRevenueBySECOMUsd;
                }
                if (dtTbt_SaleBasicForView[0].OrderProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtTbt_SaleBasicForView[0].OrderProductPrice = dtTbt_SaleBasicForView[0].OrderProductPriceUsd;
                }
                if (dtTbt_SaleBasicForView[0].NormalProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtTbt_SaleBasicForView[0].NormalProductPrice = dtTbt_SaleBasicForView[0].NormalProductPriceUsd;
                }
                if (dtTbt_SaleBasicForView[0].BidGuaranteeAmount1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtTbt_SaleBasicForView[0].BidGuaranteeAmount1 = dtTbt_SaleBasicForView[0].BidGuaranteeAmount1Usd;
                }
                if (dtTbt_SaleBasicForView[0].BidGuaranteeAmount2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtTbt_SaleBasicForView[0].BidGuaranteeAmount2 = dtTbt_SaleBasicForView[0].BidGuaranteeAmount2Usd;
                }
                if (dtTbt_SaleBasicForView[0].NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    dtTbt_SaleBasicForView[0].NewBldMgmtCost = dtTbt_SaleBasicForView[0].NewBldMgmtCostUsd;
                }

                //Add Currency to List
                for (int i = 0; i < dtTbt_SaleBasicForView.Count(); i++)
                {
                    dtTbt_SaleBasicForView[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                if (dtTbt_SaleBasicForView.Count <= 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                }
                dsSaleBasicForHDView.dtTbt_SaleBasicForView = dtTbt_SaleBasicForView;


                //3.3
                //List<doMiscTypeCode> dtGetMiscTypeCode = new List<doMiscTypeCode>();
                //for (int i = 0; i < 3; i++)
                //    dtGetMiscTypeCode.Add(new doMiscTypeCode());
                //dtGetMiscTypeCode[0].FieldName = MiscType.C_SALE_CHANGE_TYPE;
                //dtGetMiscTypeCode[0].ValueCode = "%";
                //dtGetMiscTypeCode[1].FieldName = MiscType.C_INCIDENT_TYPE;
                //dtGetMiscTypeCode[1].ValueCode = "%";
                //dtGetMiscTypeCode[2].FieldName = MiscType.C_AR_TYPE;
                //dtGetMiscTypeCode[2].ValueCode = "%";
                //ICommonHandler handCom = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                //List<doMiscTypeCode> MiscTypeResult = handCom.GetMiscTypeCodeList(dtGetMiscTypeCode);
                //dsSaleBasicForHDView.dtMiscellaneousType = MiscTypeResult;
                //3.4
                IViewContractHandler handView = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                List<dtSaleHistoryDigest> dtSaleHistoryDigestList = handView.GetSaleHistoryDigestList(dsSaleBasicForHDView.dtTbt_SaleBasicForView[0].ContractCode, null, null);
                
                //Add Currency to List
                for (int i = 0; i < dtSaleHistoryDigestList.Count(); i++)
                {
                    dtSaleHistoryDigestList[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                dsSaleBasicForHDView.dtSaleHistoryDigestList = dtSaleHistoryDigestList;

                return dsSaleBasicForHDView;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Get Rental Basic information from tbt_RentalBasic
        /// </summary>
        /// <param name="strLongContratCode"></param>
        /// <returns></returns>
        public dsRentalBasicForHistDigestView GetRentalBasicForHistoryDigestView(string strLongContratCode)
        {
            try
            {
                dsRentalBasicForHistDigestView dsRentBasicForHDView = new dsRentalBasicForHistDigestView();
                IRentralContractHandler handRen = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<dtTbt_RentalContractBasicForView> lstTbt_RentalContractBasicForView = handRen.GetTbt_RentalContractBasicForView(strLongContratCode);
                if (lstTbt_RentalContractBasicForView.Count <= 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                }
                dsRentBasicForHDView.dtTbt_RentalContractBasicForView = lstTbt_RentalContractBasicForView;

                //2.3
                ICommonHandler handCom = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //List<doMiscTypeCode> MiscTypeCode = new List<doMiscTypeCode>();
                //for (int i = 0; i < 3; i++)
                //    MiscTypeCode.Add(new doMiscTypeCode());

                //MiscTypeCode[0].FieldName = MiscType.C_RENTAL_CHANGE_TYPE;
                //MiscTypeCode[0].ValueCode = "%";
                //MiscTypeCode[1].FieldName = MiscType.C_INCIDENT_TYPE;
                //MiscTypeCode[1].ValueCode = "%";
                //MiscTypeCode[2].FieldName = MiscType.C_AR_TYPE;
                //MiscTypeCode[2].ValueCode = "%";

                //List<doMiscTypeCode> MiscTypeResult = handCom.GetMiscTypeCodeList(MiscTypeCode);
                //dsRentBasicForHDView.dtMiscellaneousType = MiscTypeResult;

                IViewContractHandler handView = ServiceContainer.GetService<IViewContractHandler>() as IViewContractHandler;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                List<dtRentalHistoryDigest> dtRentalHistoryDigest = handView.GetRentalHistoryDigestList(dsRentBasicForHDView.dtTbt_RentalContractBasicForView[0].ContractCode, null, null);

                for (int i = 0; i < dtRentalHistoryDigest.Count; i++)
                {
                    dtRentalHistoryDigest[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }

                dsRentBasicForHDView.dtRentalHistoryDigest = dtRentalHistoryDigest;
                return dsRentBasicForHDView;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Load ChangeType by selected serviceType.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public ActionResult ListMiscChangeType(string serviceType)
        {
            ICommonHandler handCom = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> MiscTypeCode = new List<doMiscTypeCode>();

            MiscTypeCode.Add(new doMiscTypeCode());

            if (serviceType == ServiceType.C_SERVICE_TYPE_RENTAL)
                MiscTypeCode[0].FieldName = MiscType.C_RENTAL_CHANGE_TYPE;
            else
                MiscTypeCode[0].FieldName = MiscType.C_SALE_CHANGE_TYPE;

            MiscTypeCode[0].ValueCode = "%";

            List<doMiscTypeCode> MiscTypeResult = handCom.GetMiscTypeCodeList(MiscTypeCode);
            ComboBoxModel CbxModel = new ComboBoxModel();

            CbxModel.SetList<doMiscTypeCode>(MiscTypeResult, "ValueCodeDisplay", "ValueCode", false);

            return Json(CbxModel);
        }

        /// <summary>
        /// Load Incident/AR Type by selected serviceType.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public ActionResult ListMiscIncidentARType(string serviceType)
        {
            ICommonHandler handCom = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> MiscTypeCode = new List<doMiscTypeCode>();
            for (int i = 0; i < 2; i++)
                MiscTypeCode.Add(new doMiscTypeCode());
            MiscTypeCode[0].FieldName = MiscType.C_INCIDENT_TYPE;
            MiscTypeCode[0].ValueCode = "%";
            MiscTypeCode[1].FieldName = MiscType.C_AR_TYPE;
            MiscTypeCode[1].ValueCode = "%";
            List<doMiscTypeCode> MiscTypeResult = handCom.GetMiscTypeCodeList(MiscTypeCode);

            ComboBoxModel CbxModel = new ComboBoxModel();
            CbxModel.SetList<doMiscTypeCode>(MiscTypeResult, "ValueCodeDisplay", "ValueCode", false);
            return Json(CbxModel);

        }
    }
}

