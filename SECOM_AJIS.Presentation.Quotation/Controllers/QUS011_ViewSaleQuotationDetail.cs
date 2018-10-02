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
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Quotation;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Presentation.Quotation.Models;

namespace SECOM_AJIS.Presentation.Quotation.Controllers
{
    public partial class QuotationController : BaseController
    {
        private const string QUS011_SCREEN_NAME = "QUS011";

        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult QUS011_Authority(QUS011_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CommonUtil cmm = new CommonUtil();
                param.Condition.QuotationTargetCode = cmm.ConvertQuotationTargetCode(
                    param.Condition.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IQuotationHandler handler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                param.doSaleQuotationData = handler.GetSaleQuotationData(param.Condition);

                if (param.doSaleQuotationData != null && param.doSaleQuotationData.dtTbt_QuotationBasic != null)
                {
                    param.doQuotationInstallationDetail = handler.GetTbt_QuotationInstallationDetail(
                        param.doSaleQuotationData.dtTbt_QuotationBasic.QuotationTargetCode, 
                        param.doSaleQuotationData.dtTbt_QuotationBasic.Alphabet
                    ).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<QUS011_ScreenParameter>("QUS011", param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize(QUS011_SCREEN_NAME)]
        public ActionResult QUS011()
        {
            ViewBag.HideQuotationTarget = true;

            try
            {
                QUS011_ScreenParameter param = GetScreenObject<QUS011_ScreenParameter>();
                if (param != null)
                    ViewBag.HideQuotationTarget = param.HideQuotationTarget;
            }
            catch
            {
            }

            return View();
        }
        /// <summary>
        /// Generate purchaser customer information section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS011_01()
        {
            ViewBag.HideBeanchContractInfo = true;

            try
            {
                doSaleQuotationData sqData = QUS011_SaleQuotationDataSession;
                if (sqData == null)
                    return Json("");

                ViewBag.QuotationTargetCode = sqData.dtTbt_QuotationBasic.QuotationTargetCodeFull; 

                if (sqData.doQuotationHeaderData.doQuotationTarget != null)
                {
                    ViewBag.ProductTypeCodeName = sqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCodeName;
                    ViewBag.ContractTargetMemo = sqData.doQuotationHeaderData.doQuotationTarget.ContractTargetMemo;
                    ViewBag.RealCustomerMemo = sqData.doQuotationHeaderData.doQuotationTarget.RealCustomerMemo;

                    if (sqData.doQuotationHeaderData.doQuotationTarget.BranchNameEN != null
                            || sqData.doQuotationHeaderData.doQuotationTarget.BranchAddressEN != null
                            || sqData.doQuotationHeaderData.doQuotationTarget.BranchNameLC != null
                            || sqData.doQuotationHeaderData.doQuotationTarget.BranchAddressLC != null)
                    {
                        ViewBag.HideBeanchContractInfo = false;
                        ViewBag.PurchaserBranchNameEN = sqData.doQuotationHeaderData.doQuotationTarget.BranchNameEN;
                        ViewBag.PurchaserBranchAddrEN = sqData.doQuotationHeaderData.doQuotationTarget.BranchAddressEN;
                        ViewBag.PurchaserBranchNameLC = sqData.doQuotationHeaderData.doQuotationTarget.BranchNameLC;
                        ViewBag.PurchaserBranchAddrLC = sqData.doQuotationHeaderData.doQuotationTarget.BranchAddressLC;
                    }
                }
                if (sqData.doQuotationHeaderData.doContractTarget != null)
                {
                    ViewBag.PurchaserCustCode = sqData.doQuotationHeaderData.doContractTarget.CustCodeShort;
                    ViewBag.PurchaserCustFullNameEN = sqData.doQuotationHeaderData.doContractTarget.CustFullNameEN;
                    ViewBag.PurchaserAddrFullEN = sqData.doQuotationHeaderData.doContractTarget.AddressFullEN;
                    ViewBag.PurchaserCustFullNameLC = sqData.doQuotationHeaderData.doContractTarget.CustFullNameLC;
                    ViewBag.PurchaserAddrFullLC = sqData.doQuotationHeaderData.doContractTarget.AddressFullLC;
                }
                if (sqData.doQuotationHeaderData.doRealCustomer != null)
                {
                    ViewBag.RealCustCode = sqData.doQuotationHeaderData.doRealCustomer.CustCodeShort;
                    ViewBag.RealCustFullNameEN = sqData.doQuotationHeaderData.doRealCustomer.CustFullNameEN;
                    ViewBag.RealAddressFullEN = sqData.doQuotationHeaderData.doRealCustomer.AddressFullEN;
                    ViewBag.RealCustFullNameLC = sqData.doQuotationHeaderData.doRealCustomer.CustFullNameLC;
                    ViewBag.RealAddrFullLC = sqData.doQuotationHeaderData.doRealCustomer.AddressFullLC;
                }
                if (sqData.doQuotationHeaderData.doQuotationSite != null)
                {
                    ViewBag.SiteCode = sqData.doQuotationHeaderData.doQuotationSite.SiteCodeShort;
                    ViewBag.SiteNameEN = sqData.doQuotationHeaderData.doQuotationSite.SiteNameEN;
                    ViewBag.SiteAddrEN = sqData.doQuotationHeaderData.doQuotationSite.AddressFullEN;
                    ViewBag.SiteNameLC = sqData.doQuotationHeaderData.doQuotationSite.SiteNameLC;
                    ViewBag.SiteAddrLC = sqData.doQuotationHeaderData.doQuotationSite.AddressFullLC;
                }
                if (sqData.doQuotationHeaderData.doQuotationTarget != null)
                {
                    ViewBag.QuotationOffice = sqData.doQuotationHeaderData.doQuotationTarget.QuotationOfficeCodeName;
                    ViewBag.OperationOffice = sqData.doQuotationHeaderData.doQuotationTarget.OperationOfficeCodeName;
                    ViewBag.AcquisitionType = sqData.doQuotationHeaderData.doQuotationTarget.AcquisitionTypeCodeName;
                    ViewBag.IntroducerCode = sqData.doQuotationHeaderData.doQuotationTarget.IntroducerCode;
                    ViewBag.MotivationType = sqData.doQuotationHeaderData.doQuotationTarget.MotivationTypeCodeName;
                    ViewBag.OldContractCode = sqData.doQuotationHeaderData.doQuotationTarget.OldContractCode;
                    //ViewBag.PurchaseReasonType = sqData.doQuotationHeaderData.doQuotationTarget.p
                    ViewBag.QuotationStaff = sqData.doQuotationHeaderData.doQuotationTarget.QuotationStaffCodeName;
                }
            }
            catch
            {
            }

            
            return View("QUS011/_QUS011_01");
        }
        /// <summary>
        /// Generate real customer information section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS011_02()
        {
            try
            {
                doSaleQuotationData sqData = QUS011_SaleQuotationDataSession;
                if (sqData == null)
                    return Json("");

                ViewBag.ProductCode = sqData.dtTbt_QuotationBasic.ProductCodeName;
                ViewBag.ContractTransferStatus = sqData.dtTbt_QuotationBasic.ContractTransferStatusCodeName;

                ViewBag.ProductPriceCurrencyType = sqData.dtTbt_QuotationBasic.ProductPriceCurrencyType;
                if (sqData.dtTbt_QuotationBasic.ProductPriceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    ViewBag.ProductPrice = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.ProductPriceUsd);
                }
                else
                {
                    ViewBag.ProductPrice = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.ProductPrice);
                }
                
                ViewBag.InstallationFeeCurrencyType = sqData.dtTbt_QuotationBasic.InstallationFeeCurrencyType;
                if (sqData.dtTbt_QuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    ViewBag.InstallationFee = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.InstallationFeeUsd);
                }
                else
                {
                    ViewBag.InstallationFee = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.InstallationFee);
                }

                ViewBag.PlanCode = sqData.dtTbt_QuotationBasic.PlanCode;
                ViewBag.QuotationNo = sqData.dtTbt_QuotationBasic.QuotationNo;

                if (sqData.dtTbt_QuotationBasic.SpecialInstallationFlag == true)
                    ViewBag.SpecialInstallation = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS011", "rdoSpecialInstall_Yes");
                else
                    ViewBag.SpecialInstallation = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS011", "rdoSpecialInstall_No");
                
                ViewBag.Planner = sqData.dtTbt_QuotationBasic.PlannerCodeName;
                ViewBag.PlanChecker = sqData.dtTbt_QuotationBasic.PlanCheckerCodeName;
                ViewBag.PlanCheckingDate = CommonUtil.TextDate(sqData.dtTbt_QuotationBasic.PlanCheckDate);
                ViewBag.PlanApprover = sqData.dtTbt_QuotationBasic.PlanApproverCodeName;
                ViewBag.PlanApprovingDate = CommonUtil.TextDate(sqData.dtTbt_QuotationBasic.PlanApproveDate);
                ViewBag.SiteBuildingArea = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.SiteBuildingArea);
                ViewBag.SecurityAreaSizeFrom = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.SecurityAreaFrom);
                ViewBag.SecurityAreaSizeTo = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.SecurityAreaTo);
                ViewBag.MainStructureType = sqData.dtTbt_QuotationBasic.MainStructureTypeCodeName;
                ViewBag.NewOldBuilding = sqData.dtTbt_QuotationBasic.BuildingTypeCodeName;

                if (sqData.dtTbt_QuotationBasic.NewBldMgmtFlag == true)
                    ViewBag.NewBldMgmtFlag = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS011", "rdoNewBuildingMgmtTypeFlagNeed");
                else
                    ViewBag.NewBldMgmtFlag = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS011", "rdoNewBuildingMgmtTypeFlagNoNeed");

                ViewBag.NewBldMgmtCostCurrencyType = sqData.dtTbt_QuotationBasic.NewBldMgmtCostCurrencyType;
                if (sqData.dtTbt_QuotationBasic.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.NewBldMgmtCostUsd);
                }
                else
                {
                    ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.NewBldMgmtCost);
                }
                ViewBag.Saleman1 = sqData.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo1;
                ViewBag.Saleman2 = sqData.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo2;
                ViewBag.Saleman3 = sqData.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo3;
                ViewBag.Saleman4 = sqData.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo4;
                ViewBag.Saleman5 = sqData.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo5;
                ViewBag.Saleman6 = sqData.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo6;
                ViewBag.Saleman7 = sqData.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo7;
                ViewBag.Saleman8 = sqData.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo8;
                ViewBag.Saleman9 = sqData.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo9;
                ViewBag.Saleman10 = sqData.dtTbt_QuotationBasic.SalesmanEmpCodeNameNo10;
                
                ViewBag.BidGuaranteeAmount1CurrencyType = sqData.dtTbt_QuotationBasic.BidGuaranteeAmount1CurrencyType;
                
                if (sqData.dtTbt_QuotationBasic.BidGuaranteeAmount1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    ViewBag.BidGuaranteeAmount1 = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.BidGuaranteeAmount1Usd);
                }
                else
                {
                    ViewBag.BidGuaranteeAmount1 = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.BidGuaranteeAmount1);
                }
                ViewBag.BidGuaranteeAmount2CurrencyType = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.BidGuaranteeAmount2CurrencyType);
                if (sqData.dtTbt_QuotationBasic.BidGuaranteeAmount2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    ViewBag.BidGuaranteeAmount2 = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.BidGuaranteeAmount2Usd);
                }
                else
                {
                    ViewBag.BidGuaranteeAmount2 = CommonUtil.TextNumeric(sqData.dtTbt_QuotationBasic.BidGuaranteeAmount2);
                }
                ViewBag.ApproveNo1 = sqData.dtTbt_QuotationBasic.ApproveNo1;
                ViewBag.ApproveNo2 = sqData.dtTbt_QuotationBasic.ApproveNo2;
                ViewBag.ApproveNo3 = sqData.dtTbt_QuotationBasic.ApproveNo3;
                ViewBag.ApproveNo4 = sqData.dtTbt_QuotationBasic.ApproveNo4;
                ViewBag.ApproveNo5 = sqData.dtTbt_QuotationBasic.ApproveNo5;

                QUS011_ScreenParameter param = GetScreenObject<QUS011_ScreenParameter>();
                if (param != null && param.doQuotationInstallationDetail != null)
                {
                    ViewBag.chkCeilingTypeTBar = param.doQuotationInstallationDetail.CeilingTypeTBar;
                    ViewBag.chkCeilingTypeSlabConcrete = param.doQuotationInstallationDetail.CeilingTypeSlabConcrete;
                    ViewBag.chkCeilingTypeMBar = param.doQuotationInstallationDetail.CeilingTypeMBar;
                    ViewBag.chkCeilingTypeSteel = param.doQuotationInstallationDetail.CeilingTypeSteel;
                    ViewBag.chkCeilingTypeNone = !(
                        (param.doQuotationInstallationDetail.CeilingTypeTBar ?? false)
                        || (param.doQuotationInstallationDetail.CeilingTypeSlabConcrete ?? false)
                        || (param.doQuotationInstallationDetail.CeilingTypeMBar ?? false)
                        || (param.doQuotationInstallationDetail.CeilingTypeSteel ?? false)
                    );
                    ViewBag.txtCeilingHeight = param.doQuotationInstallationDetail.CeilingHeight;
                    ViewBag.chkSpecialInsPVC = param.doQuotationInstallationDetail.SpecialInsPVC;
                    ViewBag.chkSpecialInsSLN = param.doQuotationInstallationDetail.SpecialInsSLN;
                    ViewBag.chkSpecialInsProtector = param.doQuotationInstallationDetail.SpecialInsProtector;
                    ViewBag.chkSpecialInsEMT = param.doQuotationInstallationDetail.SpecialInsEMT;
                    ViewBag.chkSpecialInsPE = param.doQuotationInstallationDetail.SpecialInsPE;
                    ViewBag.chkSpecialInsOther = param.doQuotationInstallationDetail.SpecialInsOther;
                    ViewBag.txtSpecialInsOther = param.doQuotationInstallationDetail.SpecialInsOtherText;
                }

            }
            catch
            {
            }

            return View("QUS011/_QUS011_02");
        }
        /// <summary>
        /// Generate site information section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS011_03()
        {
            return View("QUS011/_QUS011_03");
        }

        #endregion
        #region Actions

        /// <summary>
        /// Get instrument data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS011_GetInstrumentData()
        {
            ObjectResultData res = new ObjectResultData();

            List<doInstrumentDetail> lst = null;
            try
            {
                doSaleQuotationData sqData = QUS011_SaleQuotationDataSession;
                if (sqData != null)
                    lst = sqData.InstrumentDetailList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<doInstrumentDetail>(lst, "Quotation\\QUS011");
            return Json(res);
        }

        #endregion
        #region Methods

        /// <summary>
        /// Get/Set sale quotation data from session
        /// </summary>
        private doSaleQuotationData QUS011_SaleQuotationDataSession
        {
            get
            {
                doSaleQuotationData sqData = null;
                QUS011_ScreenParameter param = GetScreenObject<QUS011_ScreenParameter>();
                if (param != null)
                    sqData = param.doSaleQuotationData;
                return sqData;
            }
        }

        #endregion
    }
}
