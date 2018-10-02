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
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Quotation.Models;

namespace SECOM_AJIS.Presentation.Quotation.Controllers
{
    public partial class QuotationController : BaseController
    {
        private enum MODE_TYPE
        {
            AL,
            ONLINE,
            OTHER
        }
        private const string QUS012_SCREEN_NAME = "QUS012";

        #region Authority

        /// <summary>
        /// Check screen authority and permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult QUS012_Authority(QUS012_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CommonUtil cmm = new CommonUtil();
                param.Condition.QuotationTargetCode =
                    cmm.ConvertQuotationTargetCode(param.Condition.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IQuotationHandler handler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                param.doRentalQuotationData = handler.GetRentalQuotationData(param.Condition);

                if (param.doRentalQuotationData != null && param.doRentalQuotationData.dtTbt_QuotationBasic != null)
                {
                    param.doQuotationInstallationDetail = handler.GetTbt_QuotationInstallationDetail(
                        param.doRentalQuotationData.dtTbt_QuotationBasic.QuotationTargetCode,
                        param.doRentalQuotationData.dtTbt_QuotationBasic.Alphabet
                    ).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<QUS012_ScreenParameter>(QUS012_SCREEN_NAME, param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Generate screen
        /// </summary>
        /// <returns></returns>
        [Initialize(QUS012_SCREEN_NAME)]
        public ActionResult QUS012()
        {
            ViewBag.HideQuotationTarget = true;
            ViewBag.HideQuotationDetailInfo_AL = true;
            ViewBag.HideQuotationDetailInfo_ONLINE = true;
            ViewBag.HideQuotationDetailInfo_OTHER = true;
            ViewBag.HideMaintenanceDetail = true;
            ViewBag.HideInstrumentDetail_ALB = true;
            ViewBag.HideInstrumentDetail_ALA = true;
            ViewBag.HideInstrumentDetail_ONLINE = true;
            ViewBag.HideFacilityDetail = true;
            ViewBag.HideBeatGuardDetail = true;
            ViewBag.HideSentryGuardDetail = true;

            try
            {
                QUS012_ScreenParameter param = GetScreenObject<QUS012_ScreenParameter>();
                if (param != null)
                    ViewBag.HideQuotationTarget = param.HideQuotationTarget;

                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData == null)
                    return Json("");

                if (rqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL
                    || rqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_RENTAL_SALE)
                {
                    if (rqData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE
                        || (rqData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE
                            && rqData.FirstInstallCompleteFlag == false))
                    {
                        ViewBag.HideQuotationDetailInfo_AL = false;
                        ViewBag.HideInstrumentDetail_ALB = false;
                        ViewBag.HideFacilityDetail = false;
                    }
                    else if (rqData.doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE
                        && rqData.FirstInstallCompleteFlag == true)
                    {
                        ViewBag.HideQuotationDetailInfo_AL = false;
                        ViewBag.HideInstrumentDetail_ALA = false;
                        ViewBag.HideFacilityDetail = false;
                    }
                }
                else if (rqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SALE
                        || rqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_ONLINE)
                {
                    ViewBag.HideQuotationDetailInfo_ONLINE = false;
                    ViewBag.HideInstrumentDetail_ONLINE = false;
                    ViewBag.HideFacilityDetail = false;
                }
                else if (rqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_BE)
                {
                    ViewBag.HideQuotationDetailInfo_OTHER = false;
                    ViewBag.HideBeatGuardDetail = false;
                }
                else if (rqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SG)
                {
                    ViewBag.HideQuotationDetailInfo_OTHER = false;
                    ViewBag.HideSentryGuardDetail = false;
                }
                else if (rqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_MA)
                {
                    ViewBag.HideQuotationDetailInfo_OTHER = false;
                    ViewBag.HideMaintenanceDetail = false;
                }
            }
            catch
            {
            }

            return View();
        }
        /// <summary>
        /// Generate the quotation target section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_01()
        {
            ViewBag.HideBeanchContractInfo = true;

            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData == null)
                    return Json("");

                ViewBag.QuotationTargetCode = rqData.dtTbt_QuotationBasic.QuotationTargetCodeFull;
                ViewBag.ProductType = rqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCodeName;

                if (rqData.doQuotationHeaderData.doContractTarget != null)
                {
                    ViewBag.ContractCustCode = rqData.doQuotationHeaderData.doContractTarget.CustCodeShort;
                    ViewBag.ContractCustFullNameEN = rqData.doQuotationHeaderData.doContractTarget.CustFullNameEN;
                    ViewBag.ContractAddrFullEN = rqData.doQuotationHeaderData.doContractTarget.AddressFullEN;
                    ViewBag.ContractCustFullNameLC = rqData.doQuotationHeaderData.doContractTarget.CustFullNameLC;
                    ViewBag.ContractAddrFullLC = rqData.doQuotationHeaderData.doContractTarget.AddressFullLC;
                }
                if (rqData.doQuotationHeaderData.doQuotationTarget != null)
                {
                    ViewBag.ContractTargetMemo = rqData.doQuotationHeaderData.doQuotationTarget.ContractTargetMemo;
                    ViewBag.RealCustomerMemo = rqData.doQuotationHeaderData.doQuotationTarget.RealCustomerMemo;

                    if (rqData.doQuotationHeaderData.doQuotationTarget.BranchNameEN != null
                        || rqData.doQuotationHeaderData.doQuotationTarget.BranchAddressEN != null
                        || rqData.doQuotationHeaderData.doQuotationTarget.BranchNameLC != null
                        || rqData.doQuotationHeaderData.doQuotationTarget.BranchAddressLC != null)
                    {
                        ViewBag.HideBeanchContractInfo = false;
                        ViewBag.ContractBranchNameEN = rqData.doQuotationHeaderData.doQuotationTarget.BranchNameEN;
                        ViewBag.ContractBranchAddrEN = rqData.doQuotationHeaderData.doQuotationTarget.BranchAddressEN;
                        ViewBag.ContractBranchNameLC = rqData.doQuotationHeaderData.doQuotationTarget.BranchNameLC;
                        ViewBag.ContractBranchAddrLC = rqData.doQuotationHeaderData.doQuotationTarget.BranchAddressLC;
                    }
                }
                if (rqData.doQuotationHeaderData.doRealCustomer != null)
                {
                    ViewBag.RealCustCode = rqData.doQuotationHeaderData.doRealCustomer.CustCodeShort;
                    ViewBag.RealCustFullNameEN = rqData.doQuotationHeaderData.doRealCustomer.CustFullNameEN;
                    ViewBag.RealAddressFullEN = rqData.doQuotationHeaderData.doRealCustomer.AddressFullEN;
                    ViewBag.RealCustFullNameLC = rqData.doQuotationHeaderData.doRealCustomer.CustFullNameLC;
                    ViewBag.RealAddrFullLC = rqData.doQuotationHeaderData.doRealCustomer.AddressFullLC;
                }
                if (rqData.doQuotationHeaderData.doQuotationSite != null)
                {
                    ViewBag.SiteCode = rqData.doQuotationHeaderData.doQuotationSite.SiteCodeShort;
                    ViewBag.SiteNameEN = rqData.doQuotationHeaderData.doQuotationSite.SiteNameEN;
                    ViewBag.SiteAddrEN = rqData.doQuotationHeaderData.doQuotationSite.AddressFullEN;
                    ViewBag.SiteNameLC = rqData.doQuotationHeaderData.doQuotationSite.SiteNameLC;
                    ViewBag.SiteAddrLC = rqData.doQuotationHeaderData.doQuotationSite.AddressFullLC;
                }
                if (rqData.doQuotationHeaderData.doQuotationTarget != null)
                {
                    ViewBag.QuotationOffice = rqData.doQuotationHeaderData.doQuotationTarget.QuotationOfficeCodeName;
                    ViewBag.OperationOffice = rqData.doQuotationHeaderData.doQuotationTarget.OperationOfficeCodeName;
                    ViewBag.AcquisitionType = rqData.doQuotationHeaderData.doQuotationTarget.AcquisitionTypeCodeName;
                    ViewBag.IntroducerCode = rqData.doQuotationHeaderData.doQuotationTarget.IntroducerCode;
                    ViewBag.MotivationType = rqData.doQuotationHeaderData.doQuotationTarget.MotivationTypeCodeName;
                    ViewBag.OldContractCode = rqData.doQuotationHeaderData.doQuotationTarget.OldContractCodeShort;
                    ViewBag.QuotationStaff = rqData.doQuotationHeaderData.doQuotationTarget.QuotationStaffCodeName;
                }
            }
            catch
            {
            }

            return View("QUS012/_QUS012_01");
        }
        /// <summary>
        /// Generate the quotation detail for alarm section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_02()
        {
            if (PrepareDetailData(MODE_TYPE.AL) == false)
                return Json("");

            doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
            if (rqData == null)
                return Json("");

            ViewBag.HideDispathType = false;
            if (rqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_RENTAL_SALE)
                ViewBag.HideDispathType = true;

            QUS012_ScreenParameter param = GetScreenObject<QUS012_ScreenParameter>();
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

            return View("QUS012/_QUS012_02");
        }
        /// <summary>
        /// Generate the quotation detail for sale online section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_03()
        {
            if (PrepareDetailData(MODE_TYPE.ONLINE) == false)
                return Json("");

            return View("QUS012/_QUS012_03");
        }
        /// <summary>
        /// Generate the quotation detail for beat guard, sentry guard, maintenance section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_04()
        {
            if (PrepareDetailData(MODE_TYPE.OTHER) == false)
                return Json("");

            return View("QUS012/_QUS012_04");
        }
        /// <summary>
        /// Generate the maintenance detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_05()
        {
            ViewBag.HideMaintenanceTargetCode = true;
            ViewBag.C_PROD_TYPE_SALE = ProductType.C_PROD_TYPE_SALE;

            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData == null)
                    return Json("");

                if (rqData.doQuotationHeaderData.doQuotationTarget.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                {
                    if (rqData.dtTbt_QuotationBasic.MaintenanceTargetProductTypeCode == MaintenanceTargetProductType.C_MA_TARGET_PROD_TYPE_SECOM)
                        ViewBag.HideMaintenanceTargetCode = false;
                }

                ViewBag.MaintenaceTargetProduct = rqData.dtTbt_QuotationBasic.MaintenanceTargetProductTypeCodeName;
                ViewBag.MaintenaceType = rqData.dtTbt_QuotationBasic.MaintenanceTypeCodeName;
                ViewBag.MaintenaceCycle = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.MaintenanceCycle, 0);

                //if (CommonUtil.IsNullOrEmpty(rqData.dtTbt_QuotationBasic.MaintenanceMemo) == false)
                //    ViewBag.MaintenanceMemo = rqData.dtTbt_QuotationBasic.MaintenanceMemo.Replace("\n","<br/>");
                ViewBag.MaintenanceMemo = rqData.dtTbt_QuotationBasic.MaintenanceMemo;


            }
            catch
            {
            }

            return View("QUS012/_QUS012_05");
        }
        /// <summary>
        /// Generate the instrument detail in case of before 1st complete installation section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_06()
        {
            return View("QUS012/_QUS012_06");
        }
        /// <summary>
        /// Generate the instrument detail in case of after 1st complete installation section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_07()
        {
            return View("QUS012/_QUS012_07");
        }
        /// <summary>
        /// Generate the instrument detail in case of sale online section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_08()
        {
            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData == null)
                    return Json("");

                ViewBag.LinkageSaleContractCode = rqData.dtTbt_QuotationBasic.SaleOnlineContractCodeShort;
            }
            catch
            {
            }

            return View("QUS012/_QUS012_08");
        }
        /// <summary>
        /// Generate the facility detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_09()
        {
            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData == null)
                    return Json("");

                //if (CommonUtil.IsNullOrEmpty(rqData.dtTbt_QuotationBasic.FacilityMemo) == false)
                //        ViewBag.FacilityMemo = rqData.dtTbt_QuotationBasic.FacilityMemo.Replace("\n", "<br/>");

                ViewBag.FacilityMemo = rqData.dtTbt_QuotationBasic.FacilityMemo;


            }
            catch
            {
            }

            return View("QUS012/_QUS012_09");
        }
        /// <summary>
        /// Generate the beat guard detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_10()
        {
            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData == null)
                    return Json("");

                if (rqData.doBeatGuardDetail != null)
                {
                    ViewBag.WeekdayDayTime = CommonUtil.TextNumeric(rqData.doBeatGuardDetail.NumOfDayTimeWd, 0);
                    ViewBag.WeekdayNightTime = CommonUtil.TextNumeric(rqData.doBeatGuardDetail.NumOfNightTimeWd, 0);
                    ViewBag.SaturdayDayTime = CommonUtil.TextNumeric(rqData.doBeatGuardDetail.NumOfDayTimeSat, 0);
                    ViewBag.SaturdayNightTime = CommonUtil.TextNumeric(rqData.doBeatGuardDetail.NumOfNightTimeSat, 0);
                    ViewBag.SundayDayTime = CommonUtil.TextNumeric(rqData.doBeatGuardDetail.NumOfDayTimeSun, 0);
                    ViewBag.SundayNightTime = CommonUtil.TextNumeric(rqData.doBeatGuardDetail.NumOfNightTimeSun, 0);
                    ViewBag.NoBeatGuardSteps = CommonUtil.TextNumeric(rqData.doBeatGuardDetail.NumOfBeatStep, 0);
                    ViewBag.FrequencyGateUsage = CommonUtil.TextNumeric(rqData.doBeatGuardDetail.FreqOfGateUsage, 0);
                    ViewBag.NoClockKey = CommonUtil.TextNumeric(rqData.doBeatGuardDetail.NumOfClockKey, 0);

                    if (rqData.doBeatGuardDetail.NumOfDate != null)
                        ViewBag.NoDate = rqData.doBeatGuardDetail.NumOfDateCodeName;

                    ViewBag.NotifyTime = CommonUtil.TextTime(rqData.doBeatGuardDetail.NotifyTime);
                }
            }
            catch
            {
            }

            return View("QUS012/_QUS012_10");
        }
        /// <summary>
        /// Generate the sentry guard detail section
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_11()
        {
            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData == null)
                    return Json("");

                if (rqData.dtTbt_QuotationBasic != null)
                {
                   
                    ViewBag.SentryGuardAreaType = rqData.dtTbt_QuotationBasic.SentryGuardAreaTypeCodeName;
                    ViewBag.SecurityItemFeeCurrencyType = string.IsNullOrEmpty(rqData.dtTbt_QuotationBasic.SecurityItemFeeCurrencyType) ? ""
                                                            : rqData.dtTbt_QuotationBasic.SecurityItemFeeCurrencyType;
                    ViewBag.OtherItemFeeCurrencyType = string.IsNullOrEmpty(rqData.dtTbt_QuotationBasic.OtherItemFeeCurrencyType)?""
                                                            : rqData.dtTbt_QuotationBasic.OtherItemFeeCurrencyType;

                    if (rqData.dtTbt_QuotationBasic.OtherItemFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        ViewBag.OtherItemFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.OtherItemFeeUsd);
                    }
                    else
                    {
                        ViewBag.OtherItemFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.OtherItemFee);
                    }

                    if (rqData.dtTbt_QuotationBasic.SecurityItemFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        ViewBag.SecurityItemFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.SecurityItemFeeUsd);
                    }
                    else
                    {
                        ViewBag.SecurityItemFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.SecurityItemFee);
                    }
                }
            }
            catch
            {
            }

            return View("QUS012/_QUS012_11");
        }

        #endregion
        #region Actions

        /// <summary>
        /// Load maintenance detail data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_GetMaintenanceDetailData()
        {
            ObjectResultData res = new ObjectResultData();

            List<View_doContractHeader> lst = null;
            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData != null)
                {
                    if (rqData.MaintenanceTargetList != null)
                    {
                        lst = new List<View_doContractHeader>();
                        foreach (doContractHeader d in rqData.MaintenanceTargetList)
                        {
                            View_doContractHeader nd = CommonUtil.CloneObject<doContractHeader, View_doContractHeader>(d);
                            lst.Add(nd);

                            if (rqData.dtTbt_QuotationBasic != null)
                            {
                                if (rqData.dtTbt_QuotationBasic.CreateDate >= nd.CreateDate)
                                {
                                    nd.ShowDetail = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<View_doContractHeader>(lst, "Quotation\\QUS012_Maintenance_Detail");
            return Json(res);
        }
        /// <summary>
        /// Load instrument data in case of before 1st complete installation to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_GetBeforeInstrumentDetailData()
        {
            ObjectResultData res = new ObjectResultData();

            List<doInstrumentDetail> lst = null;
            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData != null)
                    lst = rqData.InstrumentDetailList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<doInstrumentDetail>(lst, "Quotation\\QUS012_InstrumentDetail_Before");
            return Json(res);
        }
        /// <summary>
        /// Load instrument data in case of after 1st complete installation to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_GetAfterInstrumentDetailData()
        {
            ObjectResultData res = new ObjectResultData();

            List<doInstrumentDetail> lst = null;
            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData != null)
                    lst = rqData.InstrumentDetailList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<doInstrumentDetail>(lst, "Quotation\\QUS012_InstrumentDetail_After");
            return Json(res);
        }
        /// <summary>
        /// Load instrument data in case of sale online to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_GetOnlineInstrumentDetailData()
        {
            ObjectResultData res = new ObjectResultData();

            List<doInstrumentDetail> lst = null;
            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData != null)
                {
                    if (rqData.doLinkageSaleContractData != null)
                        lst = rqData.doLinkageSaleContractData.SaleInstrumentDetailList;
                }
                if (lst != null)
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
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<doInstrumentDetail>(lst, "Quotation\\QUS012_InstrumentDetail_Online");
            return Json(res);
        }
        /// <summary>
        /// Load facility data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_GetFacilityDetailData()
        {
            ObjectResultData res = new ObjectResultData();

            List<doFacilityDetail> lst = null;
            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData != null)
                    lst = rqData.FacilityDetailList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<doFacilityDetail>(lst, "Quotation\\QUS012_FacilityDetail");
            return Json(res);
        }
        /// <summary>
        /// Load sentry guard data to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult QUS012_GetSentryGuardDetailData()
        {
            ObjectResultData res = new ObjectResultData();

            List<doSentryGuardDetail> lst = null;
            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData != null)
                    lst = rqData.SentryGuardDetailList;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<doSentryGuardDetail>(lst, "Quotation\\QUS012_SentryGuardDetail");
            return Json(res);
        }

        #endregion
        #region Methods

        /// <summary>
        /// Get/Set rental quotation data from session
        /// </summary>
        private doRentalQuotationData QUS012_RentalQuotationDataSession
        {
            get
            {
                doRentalQuotationData rqData = null;
                QUS012_ScreenParameter param = GetScreenObject<QUS012_ScreenParameter>();
                if (param != null)
                    rqData = param.doRentalQuotationData;
                return rqData;
            }
        }
        /// <summary>
        /// Generate provide service type data
        /// </summary>
        /// <param name="qb"></param>
        /// <returns></returns>
        private string[] SetProvideServiceType(tbt_QuotationBasic qb)
        {
            try
            {
                List<string> flag = new List<string>();
                if (qb != null)
                {
                    if (qb.FireMonitorFlag != null)
                    {
                        if (qb.FireMonitorFlag.Value)
                            flag.Add("1");
                    }
                    if (qb.CrimePreventFlag != null)
                    {
                        if (qb.CrimePreventFlag.Value)
                            flag.Add("2");
                    }
                    if (qb.EmergencyReportFlag != null)
                    {
                        if (qb.EmergencyReportFlag.Value)
                            flag.Add("3");
                    }
                    if (qb.FacilityMonitorFlag != null)
                    {
                        if (qb.FacilityMonitorFlag.Value)
                            flag.Add("4");
                    }
                }

                if (flag.Count == 0)
                    return null;

                return flag.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Prepare detail data for mapping to screen
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private bool PrepareDetailData(MODE_TYPE mode)
        {
            try
            {
                doRentalQuotationData rqData = QUS012_RentalQuotationDataSession;
                if (rqData == null)
                    return false;

                ViewBag.ProductCode = CommonUtil.TextCodeName(rqData.dtTbt_QuotationBasic.ProductCode,
                                                                rqData.dtTbt_QuotationBasic.ProductName);
                ViewBag.ContractTransferStatus = CommonUtil.TextCodeName(
                                                                rqData.dtTbt_QuotationBasic.ContractTransferStatus,
                                                                rqData.dtTbt_QuotationBasic.ContractTransferStatusName);
                ViewBag.ContractFeeCurrencyType = rqData.dtTbt_QuotationBasic.ContractFeeCurrencyType;
                if (rqData.dtTbt_QuotationBasic.ContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    ViewBag.ContractFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.ContractFeeUsd);
                }
                else
                {
                    ViewBag.ContractFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.ContractFee);
                }
                
                ViewBag.DepositFeeCurrencyType = rqData.dtTbt_QuotationBasic.DepositFeeCurrencyType;
                if (rqData.dtTbt_QuotationBasic.DepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    ViewBag.DepositFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.DepositFeeUsd);
                }
                else
                {
                    ViewBag.DepositFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.DepositFee);
                }
                ViewBag.Saleman1 = CommonUtil.TextCodeName(rqData.dtTbt_QuotationBasic.SalesmanEmpNo1,
                                                            rqData.dtTbt_QuotationBasic.SalesmanEmpNameNo1);
                ViewBag.Saleman2 = CommonUtil.TextCodeName(rqData.dtTbt_QuotationBasic.SalesmanEmpNo2,
                                                            rqData.dtTbt_QuotationBasic.SalesmanEmpNameNo2);
                ViewBag.SalesSupporter = CommonUtil.TextCodeName(rqData.dtTbt_QuotationBasic.SalesSupporterEmpNo,
                                                            rqData.dtTbt_QuotationBasic.SalesSupporterEmpName);
                
                ViewBag.OutsourcingFeeCurrencyType = rqData.dtTbt_QuotationBasic.MaintenanceFee1CurrencyType;
                if (rqData.dtTbt_QuotationBasic.MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    ViewBag.OutsourcingFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.MaintenanceFee1Usd);
                }
                else
                {
                    ViewBag.OutsourcingFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.MaintenanceFee1);
                }
                
                ViewBag.AdditionalContractFee1CurrencyType = rqData.dtTbt_QuotationBasic.AdditionalFee1CurrencyType;
                if (rqData.dtTbt_QuotationBasic.AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    ViewBag.AdditionalContractFee1 = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.AdditionalFee1Usd);
                }
                else
                {
                    ViewBag.AdditionalContractFee1 = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.AdditionalFee1);
                }
                ViewBag.AdditionalApproveNo1 = rqData.dtTbt_QuotationBasic.AdditionalApproveNo1;
                ViewBag.AdditionalContractFee2CurrencyType = rqData.dtTbt_QuotationBasic.AdditionalFee2CurrencyType;
                if (rqData.dtTbt_QuotationBasic.AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    ViewBag.AdditionalContractFee2 = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.AdditionalFee2Usd);
                }
                else
                {
                    ViewBag.AdditionalContractFee2 = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.AdditionalFee2);
                }
                ViewBag.AdditionalApproveNo2 = rqData.dtTbt_QuotationBasic.AdditionalApproveNo2;
                ViewBag.AdditionalContractFee3CurrencyType = rqData.dtTbt_QuotationBasic.AdditionalFee3CurrencyType;
                if (rqData.dtTbt_QuotationBasic.AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    ViewBag.AdditionalContractFee3 = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.AdditionalFee3Usd);
                }
                else
                {
                    ViewBag.AdditionalContractFee3 = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.AdditionalFee3);
                }
                ViewBag.AdditionalApproveNo3 = rqData.dtTbt_QuotationBasic.AdditionalApproveNo3;
                ViewBag.ApproveNo1 = rqData.dtTbt_QuotationBasic.ApproveNo1;
                ViewBag.ApproveNo2 = rqData.dtTbt_QuotationBasic.ApproveNo2;
                ViewBag.ApproveNo3 = rqData.dtTbt_QuotationBasic.ApproveNo3;
                ViewBag.ApproveNo4 = rqData.dtTbt_QuotationBasic.ApproveNo4;
                ViewBag.ApproveNo5 = rqData.dtTbt_QuotationBasic.ApproveNo5;

                if (mode == MODE_TYPE.AL || mode == MODE_TYPE.ONLINE)
                {
                    ViewBag.SecurityTypeCode = rqData.dtTbt_QuotationBasic.SecurityTypeCode;
                    ViewBag.DispatchType = rqData.dtTbt_QuotationBasic.DispatchTypeCodeName;

                    if (rqData.OperationTypeList != null)
                    {
                        List<string> lstOpt = new List<string>();
                        foreach (doQuotationOperationType opt in rqData.OperationTypeList)
                        {
                            lstOpt.Add(opt.OperationTypeCode);
                        }
                        ViewBag.OperationTypeList = lstOpt.ToArray();
                    }

                    ViewBag.PhoneLineType = rqData.dtTbt_QuotationBasic.PhoneLineTypeCodeName1;
                    ViewBag.ImageMonitoringType = rqData.dtTbt_QuotationBasic.PhoneLineTypeCodeName2;
                    ViewBag.DisconnectionMonitoringType = rqData.dtTbt_QuotationBasic.PhoneLineTypeCodeName3;

                    ViewBag.PhoneLineOwner = rqData.dtTbt_QuotationBasic.PhoneLineOwnerTypeCodeName1;
                    ViewBag.ImageMonitoringOwner = rqData.dtTbt_QuotationBasic.PhoneLineOwnerTypeCodeName2;
                    ViewBag.DisconnectionMonitoringOwner = rqData.dtTbt_QuotationBasic.PhoneLineOwnerTypeCodeName3;

                    ViewBag.ProvideServiceType = SetProvideServiceType(rqData.dtTbt_QuotationBasic);
                    ViewBag.PlanCode = rqData.dtTbt_QuotationBasic.PlanCode;
                    ViewBag.QuotationNo = rqData.dtTbt_QuotationBasic.QuotationNo;

                    if (rqData.dtTbt_QuotationBasic.SpecialInstallationFlag == true)
                        ViewBag.SpecialInstallationFlag = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS012", "rdoSpecialInstall_Yes");
                    else
                        ViewBag.SpecialInstallationFlag = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS012", "rdoSpecialInstall_No");

                    ViewBag.Planner = rqData.dtTbt_QuotationBasic.PlannerCodeName;
                    ViewBag.PlanChecker = rqData.dtTbt_QuotationBasic.PlanCheckerCodeName;
                    ViewBag.PlanCheckingDate = CommonUtil.TextDate(rqData.dtTbt_QuotationBasic.PlanCheckDate);
                    ViewBag.PlanApprover = rqData.dtTbt_QuotationBasic.PlanApproverCodeName;
                    ViewBag.PlanApprovingDate = CommonUtil.TextDate(rqData.dtTbt_QuotationBasic.PlanApproveDate);
                    ViewBag.SiteBuildingArea = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.SiteBuildingArea);
                    ViewBag.SecurityAreaSizeFrom = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.SecurityAreaFrom);
                    ViewBag.SecurityAreaSizeTo = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.SecurityAreaTo);
                    ViewBag.MainStructureType = rqData.dtTbt_QuotationBasic.MainStructureTypeCodeName;
                    ViewBag.NewOldBuilding = rqData.dtTbt_QuotationBasic.BuildingTypeCodeName;

                    if (rqData.dtTbt_QuotationBasic.NewBldMgmtFlag == true)
                        ViewBag.NewBldMgmtFlag = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS012", "rdoNewBuildingMgmtTypeFlagNeed");
                    else
                        ViewBag.NewBldMgmtFlag = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS012", "rdoNewBuildingMgmtTypeFlagNoNeed");

                   
                    ViewBag.NewBldMgmtCostCurrencyType = rqData.dtTbt_QuotationBasic.NewBldMgmtCostCurrencyType;
                    if (rqData.dtTbt_QuotationBasic.NewBldMgmtCostCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.NewBldMgmtCostUsd);
                    }
                    else
                    {
                        ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.NewBldMgmtCost);
                    }
                    ViewBag.NoBuilding = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.NumOfBuilding, 0);
                    ViewBag.NoFloor = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.NumOfFloor, 0);
                    ViewBag.MaintenanceCycle = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.MaintenanceCycle, 0);
                    ViewBag.InsuranceType = rqData.dtTbt_QuotationBasic.InsuranceTypeCodeName;
                    
                    ViewBag.InsuranceCoverageAmountCurrencyType = rqData.dtTbt_QuotationBasic.InsuranceCoverageAmountCurrencyType;
                    if (rqData.dtTbt_QuotationBasic.InsuranceCoverageAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        ViewBag.InsuranceCoverageAmount = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.InsuranceCoverageAmountUsd);
                    }
                    else
                    {
                        ViewBag.InsuranceCoverageAmount = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.InsuranceCoverageAmount);
                    }
                    
                    ViewBag.MonthlyInsuranceFeeCurrencyType = rqData.dtTbt_QuotationBasic.MonthlyInsuranceFeeCurrencyType;
                    if (rqData.dtTbt_QuotationBasic.MonthlyInsuranceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        ViewBag.MonthlyInsuranceFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.MonthlyInsuranceFeeUsd);
                    }
                    else
                    {
                        ViewBag.MonthlyInsuranceFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.MonthlyInsuranceFee);
                    }

                    if (mode == MODE_TYPE.AL)
                    {
                        
                        ViewBag.InstallationFeeCurrencyType = rqData.dtTbt_QuotationBasic.InstallationFeeCurrencyType;
                        if (rqData.dtTbt_QuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        {
                            ViewBag.InstallationFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.InstallationFeeUsd);
                        }
                        else
                        {
                            ViewBag.InstallationFee = CommonUtil.TextNumeric(rqData.dtTbt_QuotationBasic.InstallationFee);
                        }
                    }
                    else if (mode == MODE_TYPE.ONLINE)
                    {
                        ViewBag.ContractCode = rqData.dtTbt_QuotationBasic.SaleOnlineContractCodeShort;

                        if (rqData.doLinkageSaleContractData != null)
                        {
                            ViewBag.PlanCode = rqData.doLinkageSaleContractData.PlanCode;
                            ViewBag.Planner = rqData.doLinkageSaleContractData.PlannerCodeName;
                            ViewBag.PlanChecker = rqData.doLinkageSaleContractData.PlanCheckerCodeName;
                            ViewBag.PlanCheckingDate = CommonUtil.TextDate(rqData.doLinkageSaleContractData.PlanCheckDate);
                            ViewBag.PlanApprover = rqData.doLinkageSaleContractData.PlanApproverCodeName;
                            ViewBag.PlanApprovingDate = CommonUtil.TextDate(rqData.doLinkageSaleContractData.PlanApproveDate);
                            ViewBag.SiteBuildingArea = CommonUtil.TextNumeric(rqData.doLinkageSaleContractData.SiteBuildingArea);
                            ViewBag.SecurityAreaSizeFrom = CommonUtil.TextNumeric(rqData.doLinkageSaleContractData.SecurityAreaFrom);
                            ViewBag.SecurityAreaSizeTo = CommonUtil.TextNumeric(rqData.doLinkageSaleContractData.SecurityAreaTo);
                            ViewBag.MainStructureType = rqData.doLinkageSaleContractData.MainStructureTypeCodeName;
                            ViewBag.NewOldBuilding = rqData.doLinkageSaleContractData.BuildingTypeCodeName;

                            if (rqData.doLinkageSaleContractData.NewBldMgmtFlag == true)
                                ViewBag.NewBldMgmtFlag = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS012", "rdoNewBuildingMgmtTypeFlagNeed");
                            else
                                ViewBag.NewBldMgmtFlag = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_QUOTATION, "QUS012", "rdoNewBuildingMgmtTypeFlagNoNeed");

                            ViewBag.NewBldMgmtCost = CommonUtil.TextNumeric(rqData.doLinkageSaleContractData.NewBldMgmtCost);
                        }
                    }
                }
                else if (mode == MODE_TYPE.OTHER)
                {
                    ViewBag.ProvidedServiceType = rqData.doQuotationHeaderData.doQuotationTarget.ProvideServiceName;
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        #endregion
    }
}
