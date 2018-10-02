//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.DataEntity.Contract;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Installation;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check suspending, user’s permission and user’s authority of screen
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS140_Authority(CTS140_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
           
            try
            {
                #region Check is Suspending

                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                #endregion
                #region Check Authority

                //Check user’s permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP33, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                #endregion
                #region Check Business

                //if (String.IsNullOrEmpty(param.ContractCode))
                //    param.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                if (String.IsNullOrEmpty(param.ContractCode) && param.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                        param.ContractCode = param.CommonSearch.ContractCode;
                }

                //Check required field
                if (String.IsNullOrEmpty(param.ContractCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147);
                    return Json(res);
                }
                //Get rental contract data
                CheckDataAuthority_CTS110(res, param.ContractCodeLong, true);
                if (res.IsError)
                    return Json(res);

                #endregion
                #region Retrieve data

                CTS140_InitialData(param);

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS140_ScreenParameter>("CTS140", param, res);
        }

        #endregion
        #region Views

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS140")]
        public ActionResult CTS140()
        {
            try
            {
                ViewBag.C_PROD_TYPE_AL = ProductType.C_PROD_TYPE_AL;
                ViewBag.C_PROD_TYPE_BE = ProductType.C_PROD_TYPE_BE;
                ViewBag.C_PROD_TYPE_MA = ProductType.C_PROD_TYPE_MA;
                ViewBag.C_PROD_TYPE_ONLINE = ProductType.C_PROD_TYPE_ONLINE;
                ViewBag.C_PROD_TYPE_SALE = ProductType.C_PROD_TYPE_SALE;
                ViewBag.C_PROD_TYPE_SG = ProductType.C_PROD_TYPE_SG;
                ViewBag.C_PROD_TYPE_RENTAL_SALE = ProductType.C_PROD_TYPE_RENTAL_SALE;
                ViewBag.C_CONTRACT_STATUS_AFTER_START = ContractStatus.C_CONTRACT_STATUS_AFTER_START;
                ViewBag.C_CONTRACT_STATUS_BEF_START = ContractStatus.C_CONTRACT_STATUS_BEF_START;
                ViewBag.C_CONTRACT_STATUS_CANCEL = ContractStatus.C_CONTRACT_STATUS_CANCEL;
                ViewBag.C_CONTRACT_STATUS_END = ContractStatus.C_CONTRACT_STATUS_END;
                ViewBag.C_CONTRACT_STATUS_FIXED_CANCEL = ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL;
                ViewBag.C_CONTRACT_STATUS_STOPPING = ContractStatus.C_CONTRACT_STATUS_STOPPING;
                ViewBag.C_ACQUISITION_TYPE_CUST = AcquisitionType.C_ACQUISITION_TYPE_CUST;
                ViewBag.C_ACQUISITION_TYPE_INSIDE_COMPANY = AcquisitionType.C_ACQUISITION_TYPE_INSIDE_COMPANY;
                ViewBag.C_ACQUISITION_TYPE_SUBCONTRACTOR = AcquisitionType.C_ACQUISITION_TYPE_SUBCONTRACTOR;
                ViewBag.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL;
                ViewBag.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT;
                ViewBag.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE;
                ViewBag.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND;
                ViewBag.RequiredDate = false;

                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();

                #region Contract basic information section

                IUserControlHandler userControlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                doRentalContractBasicInformation doRentalContractBasic = userControlHandler.GetRentalContactBasicInformationData(param.ContractCodeLong);
                if (doRentalContractBasic != null)
                {
                    ViewBag.ContractCode = doRentalContractBasic.ContractCodeShort;
                    ViewBag.UserCode = doRentalContractBasic.UserCode;
                    ViewBag.CustomerCode = doRentalContractBasic.ContractTargetCustCodeShort;
                    ViewBag.RealCustomerCustCode = doRentalContractBasic.RealCustomerCustCodeShort;
                    ViewBag.SiteCode = doRentalContractBasic.SiteCodeShort;

                    if (doRentalContractBasic.ContractTargetCustomerImportant == null)
                        ViewBag.ImportantFlag = false;
                    else
                        ViewBag.ImportantFlag = (bool)doRentalContractBasic.ContractTargetCustomerImportant;

                    ViewBag.CustFullNameEN = doRentalContractBasic.ContractTargetNameEN;
                    ViewBag.AddressFullEN = doRentalContractBasic.ContractTargetAddressEN;
                    ViewBag.SiteNameEN = doRentalContractBasic.SiteNameEN;
                    ViewBag.SiteAddressEN = doRentalContractBasic.SiteAddressEN;
                    ViewBag.CustFullNameLC = doRentalContractBasic.ContractTargetNameLC;
                    ViewBag.AddressFullLC = doRentalContractBasic.ContractTargetAddressLC;
                    ViewBag.SiteNameLC = doRentalContractBasic.SiteNameLC;
                    ViewBag.SiteAddressLC = doRentalContractBasic.SiteAddressLC;
                    ViewBag.InstallationStatus = CommonUtil.TextCodeName(doRentalContractBasic.InstallationStatusCode, doRentalContractBasic.InstallationStatusName);
                    ViewBag.OperationOffice = CommonUtil.TextCodeName(doRentalContractBasic.OperationOfficeCode, doRentalContractBasic.OperationOfficeName);
                    ViewBag.OperationOfficeCode = doRentalContractBasic.OperationOfficeCode; //Add by Jutarat A. on 15082012

                    ViewBag.QuotationNo = doRentalContractBasic.QuotationNo;
                }

                ViewBag.ProductTypeCode = null;
                ViewBag.FirstInstallCompleteFlag = null;
                if (param.DSRentalContract != null)
                {
                    if (param.DSRentalContract.dtTbt_RentalContractBasic != null
                        && param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0)
                    {
                        ViewBag.ProductTypeCode = param.DSRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode;
                        ViewBag.ContractStatus = param.DSRentalContract.dtTbt_RentalContractBasic[0].ContractStatus;
                        ViewBag.FirstInstallCompleteFlag = param.DSRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag;
                        ViewBag.Memo = param.DSRentalContract.dtTbt_RentalContractBasic[0].Memo;
                    }

                    if (param.DSRentalContract.dtTbt_RentalSecurityBasic != null
                        && param.DSRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
                    {
                        #region Product name

                        IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                        List<tbm_Product> listProduct = masterHandler.GetTbm_Product(
                            param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ProductCode,
                            param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ProductTypeCode);
                        List<CTS140_DOProductName> listDOProductName = CommonUtil.ClonsObjectList<tbm_Product, CTS140_DOProductName>(listProduct);
                        CommonUtil.MappingObjectLanguage(listDOProductName);

                        if (listDOProductName.Count() > 0)
                        {
                            ViewBag.ProductName = CommonUtil.TextCodeName(
                                param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ProductCode,
                                listDOProductName[0].ProductName);
                        }

                        #endregion
                    }
                }

                if (param.ListRentalSecurityBasic != null
                    && param.ListRentalSecurityBasic.Count > 0)
                {
                    ViewBag.SecurityTypeCode = param.ListRentalSecurityBasic[0].SecurityTypeCode;
                }

                #endregion
                #region Contract relate information section

                if (param.DOContractRelateInformation != null)
                {
                    #region Last Order Contract Fee

                    ViewBag.LastOrderContractFeeCurrencyType = param.DOContractRelateInformation.LastOrderContractFeeCurrencyType;
                    ViewBag.LastOrderContractFee = CommonUtil.TextNumeric(param.DOContractRelateInformation.LastOrderContractFee);

                    #endregion

                    ViewBag.StartDealDate = CommonUtil.TextDate(param.DOContractRelateInformation.StartDealDate);
                    ViewBag.FirstSecurityStartDate = CommonUtil.TextDate(param.DOContractRelateInformation.FirstSecurityStartDate);
                    ViewBag.LastChangeType = param.DOContractRelateInformation.LastChangeTypeCodeName;
                    ViewBag.ContractStartDate = CommonUtil.TextDate(param.DOContractRelateInformation.ContractStartDate);
                    ViewBag.ContractEndDate = CommonUtil.TextDate(param.DOContractRelateInformation.ContractEndDate);
                    
                    if (param.DOContractRelateInformation.ContractDurationMonth != null)
                        ViewBag.ContractDurationMonth = param.DOContractRelateInformation.ContractDurationMonth.ToString();

                    ViewBag.LastChangeImplementDate = CommonUtil.TextDate(param.DOContractRelateInformation.LastChangeImplementDate);
                    ViewBag.LastOCC = param.DOContractRelateInformation.LastOCC;
                    ViewBag.ProjectCode = param.DOContractRelateInformation.ProjectCode;

                    if (param.DSRentalContract.dtTbt_RentalSecurityBasic[0].AutoRenewMonth != null)
                        ViewBag.AutoRenewMonth = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].AutoRenewMonth.ToString();

                    if (CommonUtil.IsNullOrEmpty(param.DSRentalContract.dtTbt_RentalContractBasic[0].StartType) == false)
                        ViewBag.RequiredDate = true;
                }

                #endregion
                #region Contract agreement information section

                if (param.DOContractAgreementInformation != null)
                {
                    ViewBag.ApproveContractDate = CommonUtil.TextDate(param.DOContractAgreementInformation.ApproveContractDate);
                    ViewBag.ContractOfficeCode = param.DOContractAgreementInformation.ContractOfficeCode;
                    ViewBag.QuotationTargetCode = param.DOContractAgreementInformation.QuotationTargetCodeShort;
                    
                    ViewBag.QuotationAlphabet = param.DOContractAgreementInformation.QuotationAlphabet;
                    ViewBag.PlanCode = param.DOContractAgreementInformation.PlanCode;
                    ViewBag.SalesmanEmpNo1 = param.DOContractAgreementInformation.SalesmanEmpNo1;
                    ViewBag.SalesmanEmpName1 = param.DOContractAgreementInformation.SalesmanEmpName1;
                    ViewBag.SalesmanEmpNo2 = param.DOContractAgreementInformation.SalesmanEmpNo2;
                    ViewBag.SalesmanEmpName2 = param.DOContractAgreementInformation.SalesmanEmpName2;
                    ViewBag.SalesSupporterEmpNo = param.DOContractAgreementInformation.SalesSupporterEmpNo;
                    ViewBag.SalesSupporterEmpName = param.DOContractAgreementInformation.SalesSupporterEmpName;
                    ViewBag.RelatedContractCode = param.DOContractAgreementInformation.RelatedContractCodeShort;
                    ViewBag.OldContractCode = param.DOContractAgreementInformation.OldContractCodeShort;
                    ViewBag.AcquisitionTypeCode = param.DOContractAgreementInformation.AcquisitionTypeCode;
                    ViewBag.IntroducerCode = param.DOContractAgreementInformation.IntroducerCode;
                    ViewBag.MotivationTypeCode = param.DOContractAgreementInformation.MotivationTypeCode;

                    ViewBag.PaymentDateIncentive = CommonUtil.TextDate(param.DOContractAgreementInformation.PaymentDateIncentive);
                }

                #endregion
                #region Deposite information section

                if (param.DODepositInformation != null)
                {
                    #region Normal Deposit Fee

                    ViewBag.NormalDepositFeeCurrencyType = param.DODepositInformation.NormalDepositFeeCurrencyType;
                    ViewBag.NormalDepositFee = CommonUtil.TextNumeric(param.DODepositInformation.NormalDepositFee);

                    #endregion
                    #region Order Depoisit Fee

                    ViewBag.OrderDepositFeeCurrencyType = param.DODepositInformation.OrderDepositFeeCurrencyType;
                    ViewBag.OrderDepositFee = CommonUtil.TextNumeric(param.DODepositInformation.OrderDepositFee);

                    #endregion
                    #region Exempted Deposit Fee

                    ViewBag.ExemptedDepositFeeCurrencyType = param.DODepositInformation.ExemptedDepositFeeCurrencyType;
                    ViewBag.ExemptedDepositFee = CommonUtil.TextNumeric(param.DODepositInformation.ExemptedDepositFee);

                    #endregion

                    ViewBag.CounterBalanceOriginContractCode = param.DODepositInformation.CounterBalanceOriginContractCodeShort;
                }

                #endregion
                #region Contract document information section

                ViewBag.IrregurationDocUsageFlag = false;
                if (param.DOContractDocumentInformation != null)
                {
                    if (param.DOContractDocumentInformation.IrregurationDocUsageFlag != null)
                        ViewBag.IrregurationDocUsageFlag = param.DOContractDocumentInformation.IrregurationDocUsageFlag;

                    ViewBag.PODocAuditResult = param.DOContractDocumentInformation.PODocAuditResultCodeName;
                    ViewBag.PODocReceiveDate = param.DOContractDocumentInformation.PODocReceiveDate;
                    ViewBag.ContractDocAuditResult = param.DOContractDocumentInformation.ContractDocAuditResultCodeName;
                    ViewBag.ContractDocReceiveDate = param.DOContractDocumentInformation.ContractDocReceiveDate;
                    ViewBag.StartMemoAuditResult = param.DOContractDocumentInformation.StartMemoAuditResultCodeName;
                    ViewBag.StartMemoReceiveDate = param.DOContractDocumentInformation.StartMemoReceiveDate;
                }

                #endregion
                #region Provide service information section

                ViewBag.FireMonitorFlag = false;
                ViewBag.CrimePreventFlag = false;
                ViewBag.EmergencyReportFlag = false;
                ViewBag.FacilityMonitorFlag = false;
                if (param.DOProvideServiceInformation != null)
                {
                    ViewBag.FireMonitorFlag = param.DOProvideServiceInformation.FireMonitorFlag;
                    ViewBag.CrimePreventFlag = param.DOProvideServiceInformation.CrimePreventFlag;
                    ViewBag.EmergencyReportFlag = param.DOProvideServiceInformation.EmergencyReportFlag;
                    ViewBag.FacilityMonitorFlag = param.DOProvideServiceInformation.FacilityMonitorFlag;
                    ViewBag.PhoneLineTypeCode1 = param.DOProvideServiceInformation.PhoneLineTypeCode1;
                    ViewBag.PhoneLineOwnerCode1 = param.DOProvideServiceInformation.PhoneLineOwnerCode1;
                    ViewBag.PhoneLineTypeCode2 = param.DOProvideServiceInformation.PhoneLineTypeCode2;
                    ViewBag.PhoneLineOwnerCode2 = param.DOProvideServiceInformation.PhoneLineOwnerCode2;
                    ViewBag.PhoneLineTypeCode3 = param.DOProvideServiceInformation.PhoneLineTypeCode3;
                    ViewBag.PhoneLineOwnerCode3 = param.DOProvideServiceInformation.PhoneLineOwnerCode3;

                    ViewBag.PhoneNo1 = param.DOProvideServiceInformation.PhoneNo1;
                    ViewBag.PhoneNo2 = param.DOProvideServiceInformation.PhoneNo2;
                    ViewBag.PhoneNo3 = param.DOProvideServiceInformation.PhoneNo3;
                }

                #endregion
                #region Maintenance information section

                if (param.DOMaintenanceInformation != null)
                {
                    ViewBag.MaintenanceTypeCode = param.DOMaintenanceInformation.MaintenanceTypeCode;
                    ViewBag.MaintenanceCycle = param.DOMaintenanceInformation.MaintenanceCycle;
                    ViewBag.MaintenanceContractStartMonth = param.DOMaintenanceInformation.MaintenanceContractStartMonth;
                    ViewBag.MaintenanceContractStartYear = param.DOMaintenanceInformation.MaintenanceContractStartYear;
                    ViewBag.MaintenanceFeeTypeCode = param.DOMaintenanceInformation.MaintenanceFeeTypeCode;
                }

                #endregion
                #region Site information section

                if (param.DOSiteInformation != null)
                {
                    ViewBag.BuildingTypeCode = param.DOSiteInformation.BuildingTypeCode;
                    ViewBag.SiteBuildingArea = param.DOSiteInformation.SiteBuildingArea;

                    if (param.DOSiteInformation.NumOfBuilding != null)
                        ViewBag.NumOfBuilding = param.DOSiteInformation.NumOfBuilding.ToString();

                    ViewBag.SecurityAreaFrom = param.DOSiteInformation.SecurityAreaFrom;
                    ViewBag.SecurityAreaTo = param.DOSiteInformation.SecurityAreaTo;

                    if (param.DOSiteInformation.NumOfFloor != null)
                        ViewBag.NumOfFloor = param.DOSiteInformation.NumOfFloor.ToString();

                    ViewBag.MainStructureTypeCode = param.DOSiteInformation.MainStructureTypeCode;
                }

                #endregion
                #region Cancel contract condition section

                ViewBag.Refund = false;
                ViewBag.ReceiveRevenue = false;
                ViewBag.Bill = false;
                ViewBag.Exempt = false;

                if (param.DOCancelContractCondition != null)
                {
                    ViewBag.CancelContractDate = param.DOCancelContractCondition.CancelContractDate;
                    ViewBag.RemovalInstallationCompleteDate = param.DOCancelContractCondition.RemovalInstallationCompleteDate;


                    if (param.DOCancelContractCondition.ProcessAfterCounterBalanceType == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND)
                        ViewBag.Refund = true;
                    else
                        ViewBag.Refund = false;

                    if (param.DOCancelContractCondition.ProcessAfterCounterBalanceType == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE)
                        ViewBag.ReceiveRevenue = true;
                    else
                        ViewBag.ReceiveRevenue = false;

                    if (param.DOCancelContractCondition.ProcessAfterCounterBalanceType == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL)
                        ViewBag.Bill = true;
                    else
                        ViewBag.Bill = false;

                    if (param.DOCancelContractCondition.ProcessAfterCounterBalanceType == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT)
                        ViewBag.Exempt = true;
                    else
                        ViewBag.Exempt = false;



                    if (param.DOCancelContractCondition.ProcessAfterCounterBalanceTypeUsd == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND)
                        ViewBag.RefundUsd = true;
                    else
                        ViewBag.RefundUsd = false;

                    if (param.DOCancelContractCondition.ProcessAfterCounterBalanceTypeUsd == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE)
                        ViewBag.ReceiveRevenueUsd = true;
                    else
                        ViewBag.ReceiveRevenueUsd = false;

                    if (param.DOCancelContractCondition.ProcessAfterCounterBalanceTypeUsd == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL)
                        ViewBag.BillUsd = true;
                    else
                        ViewBag.BillUsd = false;

                    if (param.DOCancelContractCondition.ProcessAfterCounterBalanceTypeUsd == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT)
                        ViewBag.ExemptUsd = true;
                    else
                        ViewBag.ExemptUsd = false;


                    ViewBag.C_BILLING_TYPE_CONTRACT_FEE = param.DOCancelContractCondition.C_BILLING_TYPE_CONTRACT_FEE;
                    ViewBag.C_BILLING_TYPE_MAINTENANCE_FEE = param.DOCancelContractCondition.C_BILLING_TYPE_MAINTENANCE_FEE;
                    ViewBag.C_BILLING_TYPE_DEPOSIT_FEE = param.DOCancelContractCondition.C_BILLING_TYPE_DEPOSIT_FEE;
                    ViewBag.C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE = param.DOCancelContractCondition.C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;
                    ViewBag.C_BILLING_TYPE_CANCEL_CONTRACT_FEE = param.DOCancelContractCondition.C_BILLING_TYPE_CANCEL_CONTRACT_FEE;
                    ViewBag.C_BILLING_TYPE_CHANGE_INSTALLATION_FEE = param.DOCancelContractCondition.C_BILLING_TYPE_CHANGE_INSTALLATION_FEE;
                    ViewBag.C_BILLING_TYPE_CARD_FEE = param.DOCancelContractCondition.C_BILLING_TYPE_CARD_FEE;
                    ViewBag.C_BILLING_TYPE_OTHER_FEE = param.DOCancelContractCondition.C_BILLING_TYPE_OTHER_FEE;
                    ViewBag.C_HANDLING_TYPE_BILL_UNPAID_FEE = param.DOCancelContractCondition.C_HANDLING_TYPE_BILL_UNPAID_FEE;
                    ViewBag.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE = param.DOCancelContractCondition.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE;
                    ViewBag.C_HANDLING_TYPE_RECEIVE_AS_REVENUE = param.DOCancelContractCondition.C_HANDLING_TYPE_RECEIVE_AS_REVENUE;
                    ViewBag.C_HANDLING_TYPE_REFUND = param.DOCancelContractCondition.C_HANDLING_TYPE_REFUND;
                    ViewBag.C_HANDLING_TYPE_SLIDE = param.DOCancelContractCondition.C_HANDLING_TYPE_SLIDE;
                }

                #endregion

            }
            catch
            {
            }

            return View();
        }

        #endregion
        #region Actions

        /// <summary>
        /// Validate entering section when initail screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS140_ValidateEnteringCondition()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();

                //1.9 Validate entering conditions
                if (param.DSRentalContract != null)
                {
                    if (param.DSRentalContract.dtTbt_RentalContractBasic != null)
                    {
                        if (param.DSRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3119, new string[] { param.ContractCode }, null);
                            res.ResultData = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                res.ResultData = false;
            }

            return Json(res);
        }
        /// <summary>
        /// Get Constant for check hide section when initail screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS140_GetConstantHideShowDIV()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();

                res.ResultData = new CTS140_DOConstantHideDIV()
                {
                    ContractStatus = param.DSRentalContract.dtTbt_RentalContractBasic[0].ContractStatus,
                    ProductTypeCode = param.DSRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode,
                    C_PROD_TYPE_AL = ProductType.C_PROD_TYPE_AL,
                    C_PROD_TYPE_BE = ProductType.C_PROD_TYPE_BE,
                    C_PROD_TYPE_MA = ProductType.C_PROD_TYPE_MA,
                    C_PROD_TYPE_ONLINE = ProductType.C_PROD_TYPE_ONLINE,
                    C_PROD_TYPE_SALE = ProductType.C_PROD_TYPE_SALE,
                    C_PROD_TYPE_SG = ProductType.C_PROD_TYPE_SG,
                    C_PROD_TYPE_RENTAL_SALE = ProductType.C_PROD_TYPE_RENTAL_SALE,
                    C_CONTRACT_STATUS_AFTER_START = ContractStatus.C_CONTRACT_STATUS_AFTER_START,
                    C_CONTRACT_STATUS_BEF_START = ContractStatus.C_CONTRACT_STATUS_BEF_START,
                    C_CONTRACT_STATUS_CANCEL = ContractStatus.C_CONTRACT_STATUS_CANCEL,
                    C_CONTRACT_STATUS_END = ContractStatus.C_CONTRACT_STATUS_END,
                    C_CONTRACT_STATUS_FIXED_CANCEL = ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL,
                    C_CONTRACT_STATUS_STOPPING = ContractStatus.C_CONTRACT_STATUS_STOPPING
                };
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get data of Maintenance to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS140_GetMaintenanceGrid()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();
                res.ResultData = CommonUtil.ConvertToXml<CTS140_DOMaintenanceGrid>(param.ListDOMaintenanceGridEdit, "Contract\\CTS140Maintenance", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get ScreenCode when click [Detail] button on Maintenance grid
        /// </summary>
        /// <param name="doGetScreeenCode"></param>
        /// <returns></returns>
        public ActionResult CTS140_GetScreenCode(CTS140_DOGetScreeenCode doGetScreeenCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IRentralContractHandler rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<tbt_RentalContractBasic> listRentalContractBasic = rentralContractHandler.GetTbt_RentalContractBasic(doGetScreeenCode.ContractCodeLong, null);

                if (listRentalContractBasic.Count() > 0)
                    res.ResultData = "CMS140";
                else
                    res.ResultData = "CMS160";

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Add data to Maintenance grid when click [Add] button in ‘Maintenance information’ section
        /// </summary>
        /// <param name="doMaintenanceGrid"></param>
        /// <returns></returns>
        public ActionResult CTS140_Add(CTS140_DOMaintenanceGrid doMaintenanceGrid)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }

                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();

                // 3.1 Check whether contract code can be used as maintenance target contract or not
                CommonUtil comUtil = new CommonUtil();
                string strContractCodeLong = comUtil.ConvertContractCode(doMaintenanceGrid.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                IContractHandler contractHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                //TODO: (Jutarat) Bug report QU-124 Add param quotation target code
                doContractHeader doContractHeader = contractHandler.CheckMaintenanceTargetContract(
                        strContractCodeLong,
                        param.DSRentalContract.dtTbt_RentalContractBasic[0].ContractCode);
                if (doContractHeader != null)
                {
                    #region Validate business

                    bool isSameSite = false;
                    if (param.DSRentalContract.dtTbt_RentalContractBasic != null)
                    {
                        if (param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0)
                        {
                            if (doContractHeader.SiteCode == param.DSRentalContract.dtTbt_RentalContractBasic[0].SiteCode)
                                isSameSite = true;
                        }
                    }
                    if (isSameSite == false)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3261);
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3261, null, new string[] { "MaintenanceTargetContractCode" });
                        return Json(res);
                    }

                    string duplicateCode = null;
                    bool isSameProductType = false;
                    bool isSaleExist = false;

                    CommonUtil cmm = new CommonUtil();
                    string contractCodeShort = cmm.ConvertContractCode(doContractHeader.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    foreach (CTS140_DOMaintenanceGrid ma in param.ListDOMaintenanceGridEdit)
                    {
                        // 3.2.1 Check the duplicate contract code in the list
                        if (duplicateCode == null
                            && ma.ContractCode == contractCodeShort)
                        {
                            duplicateCode = contractCodeShort;
                            break;
                        }
                        // 3.2.2 Can add only 1 N code
                        else if (
                            (doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                            || doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                                || doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                            && (ma.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                            || ma.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                                    || ma.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
                        {
                            isSameProductType = true;
                        }
                        // 3.2.3 Cannot add rental contract and sale contract together
                        else if (

                            ((doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                                || doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                                || doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                                && ma.ProductTypeCode == ProductType.C_PROD_TYPE_SALE)

                            || ((ma.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                            || ma.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                                    || ma.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                            && doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_SALE))
                        {
                            isSaleExist = true;
                        }
                    }
                    if (duplicateCode != null)
                    {
                        string[] obj = { duplicateCode };
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3194, duplicateCode);
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3194, obj, new string[] { "MaintenanceTargetContractCode" });
                        return Json(res);
                    }
                    if (isSameProductType)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0032);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0032, null, new string[] { "MaintenanceTargetContractCode" });
                        return Json(res);
                    }
                    if (isSaleExist)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0132);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0132, null, new string[] { "MaintenanceTargetContractCode" });
                        return Json(res);
                    }

                    #endregion


                    doMaintenanceGrid.ContractCode =
                        comUtil.ConvertContractCode(doContractHeader.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    doMaintenanceGrid.ProductCode = doContractHeader.ProductCode;
                    doMaintenanceGrid.ProductName = doContractHeader.ProductName;
                    doMaintenanceGrid.ProductTypeCode = doContractHeader.ProductTypeCode;

                    param.ListDOMaintenanceGridEdit.Add(doMaintenanceGrid);
                    res.ResultData = param.ListDOMaintenanceGridEdit;
                }
            }
            catch (Exception ex)
            {
                res.ResultData = false;
                res.AddErrorMessage(ex);
                res.Message.Controls = new string[] { "MaintenanceTargetContractCode" };
            }

            return Json(res);
        }
        /// <summary>
        /// Remove data from Maintenance grid when click [Remove] button in 'Maintenance target contract list' section
        /// </summary>
        /// <param name="doMaintenanceGrid"></param>
        /// <returns></returns>
        public ActionResult CTS140_Remove(CTS140_DOMaintenanceGrid doMaintenanceGrid)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();

                if (param.ListDOMaintenanceGridEdit != null)
                {
                    List<CTS140_DOMaintenanceGrid> listRelationType = param.ListDOMaintenanceGridEdit.FindAll(delegate(CTS140_DOMaintenanceGrid s)
                    {
                        return s.ContractCode == doMaintenanceGrid.ContractCode;
                    });
                    foreach (var item in listRelationType)
                    {
                        param.ListDOMaintenanceGridEdit.Remove(item);
                    }
                }
                if (param.DSRentalContract != null)
                {
                    if (param.DSRentalContract.dtTbt_RelationType != null)
                    {
                        List<tbt_RelationType> listTbtRelationType = param.DSRentalContract.dtTbt_RelationType.FindAll(delegate(tbt_RelationType s)
                        {
                            return s.ContractCode == doMaintenanceGrid.ContractCode;
                        });
                        foreach (var item in listTbtRelationType)
                        {
                            param.DSRentalContract.dtTbt_RelationType.Remove(item);
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
        /// Get data of CancelContract to grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS140_GetCancelGrid()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblNormalFee");
                string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblContractCodeForSlideFee");

                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();
                if (param.ListDOCancelContractGrid != null)
                {
                    DataEntity.Common.doMiscTypeCode curr = null;
                    ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    List<doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                    CommonUtil cmm = new CommonUtil();
                    foreach (var item in param.ListDOCancelContractGrid)
                    {
                        if (currencies != null)
                        {
                            #region Fee Amount

                            curr = currencies.Find(x => x.ValueCode == item.FeeAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];
                            
                            if (item.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                if (item.FeeAmountUsd != null)
                                    item.FeeAmountString = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(item.FeeAmountUsd));
                            }
                            else
                            {
                                if (item.FeeAmount != null)
                                    item.FeeAmountString = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(item.FeeAmount));
                            }

                            #endregion
                            #region Tax Amount

                            curr = currencies.Find(x => x.ValueCode == item.TaxAmountCurrencyType);
                            if (curr == null)
                                curr = currencies[0];

                            if (item.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                if (item.TaxAmountUsd != null)
                                    item.TaxAmountString = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(item.TaxAmountUsd));
                            }
                            else
                            {
                                if (item.TaxAmount != null)
                                    item.TaxAmountString = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(item.TaxAmount));
                            }

                            #endregion
                        }

                        //item.FeeAmountString = CommonUtil.TextNumeric(item.FeeAmount);
                        //item.TaxAmountString = CommonUtil.TextNumeric(item.TaxAmount);

                        if (item.StartPeriodDate != null && item.EndPeriodDate != null)
                        {
                            item.PeriodString = CommonUtil.TextDate(item.StartPeriodDate) + " To " + CommonUtil.TextDate(item.EndPeriodDate);
                        }
                        else
                        {
                            if (item.StartPeriodDate != null)
                                item.PeriodString = CommonUtil.TextDate(item.StartPeriodDate);

                            if (item.EndPeriodDate != null)
                                item.PeriodString = CommonUtil.TextDate(item.EndPeriodDate);
                        }


                        //#region Normal Fee Amount

                        //string txtNormalFee = null;

                        //curr = currencies.Find(x => x.ValueCode == item.NormalFeeAmountCurrencyType);
                        //if (curr == null)
                        //    curr = currencies[0];

                        //if (item.NormalFeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        //{
                        //    if (item.NormalFeeAmountUsd != null)
                        //        txtNormalFee = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(item.NormalFeeAmountUsd));
                        //}
                        //else
                        //{
                        //    if (item.NormalFeeAmount != null)
                        //        txtNormalFee = string.Format("{0} {1}", curr.ValueDisplayEN, CommonUtil.TextNumeric(item.NormalFeeAmount));
                        //}

                        //#endregion

                        //string remark = string.Empty; 
                        //List<string> rLst = new List<string>()
                        //{
                        //    item.Remark,
                        //    cmm.ConvertContractCode(item.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_SHORT),
                        //    txtNormalFee
                        //};
                        //foreach (string r in rLst)
                        //{
                        //    if (CommonUtil.IsNullOrEmpty(r) == false)
                        //    {
                        //        if (remark != string.Empty)
                        //            remark += "<br/>";
                        //        remark += r;
                        //    }
                        //}
                        //item.RemarkString = remark;

                        #region Remark

                        item.RemarkString = string.Empty;
                        if (string.IsNullOrEmpty(item.Remark) == false)
                            item.RemarkString = item.Remark;
                        if (item.ContractCode_CounterBalance != null)
                        {
                            if (string.IsNullOrEmpty(item.RemarkString) == false)
                                item.RemarkString += "<br/>";

                            item.RemarkString += string.Format("{0}: {1}", lblContractCodeForSlideFee, cmm.ConvertContractCode(item.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_SHORT));
                        }
                        if (item.NormalFeeAmount != null
                            || item.NormalFeeAmountUsd != null)
                        {
                            if (string.IsNullOrEmpty(item.RemarkString) == false)
                                item.RemarkString += "<br/>";

                            string txtcurr = string.Empty;
                            curr = currencies.Find(x => x.ValueCode == item.NormalFeeAmountCurrencyType);
                            if (curr != null)
                                txtcurr = curr.ValueDisplayEN;

                            if (item.NormalFeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                if (item.NormalFeeAmountUsd != null)
                                    item.RemarkString += string.Format("{0}: {1} {2}", lblNormalFee, txtcurr, CommonUtil.TextNumeric(item.NormalFeeAmountUsd.Value));
                            }
                            else
                            {
                                if (item.NormalFeeAmount != null)
                                    item.RemarkString += string.Format("{0}: {1} {2}", lblNormalFee, txtcurr, CommonUtil.TextNumeric(item.NormalFeeAmount.Value));
                            }
                        }

                        #endregion

                    }

                }

                res.ResultData = CommonUtil.ConvertToXml<CTS140_DOCancelContractGrid>(param.ListDOCancelContractGrid, "Contract\\CTS140CancelContract", CommonUtil.GRID_EMPTY_TYPE.VIEW);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Initial data of BillingType to ComboBox
        /// </summary>
        /// <param name="doCancelContractCondition"></param>
        /// <returns></returns>
        public ActionResult CTS140_InitialBillingType(CTS140_DOCancelContractCondition doCancelContractCondition)
        {
            ComboBoxModel cboModel = new ComboBoxModel();
            ObjectResultData res = new ObjectResultData();
            try
            {
                List<doMiscTypeCode>  listDoMiscTypeCode = new List<doMiscTypeCode>();
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                doMiscTypeCode doMiscType = null;
                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
                {
                    doMiscType = new doMiscTypeCode();
                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
                    listDoMiscTypeCode.Add(doMiscType);

                    //Comment by Jutarat A. on 07082012
                    //doMiscType = new doMiscTypeCode();
                    //doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE;
                    //doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
                    //listDoMiscTypeCode.Add(doMiscType);
                }

                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
                {
                    doMiscType = new doMiscTypeCode();
                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
                    listDoMiscTypeCode.Add(doMiscType);
                }

                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                {
                    doMiscType = new doMiscTypeCode();
                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
                    listDoMiscTypeCode.Add(doMiscType);

                    doMiscType = new doMiscTypeCode();
                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE;
                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
                    listDoMiscTypeCode.Add(doMiscType);
                }

                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE)
                {
                    doMiscType = new doMiscTypeCode();
                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
                    listDoMiscTypeCode.Add(doMiscType);

                    doMiscType = new doMiscTypeCode();
                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE;
                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
                    listDoMiscTypeCode.Add(doMiscType);
                }

                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE ||
                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE ||
                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE ||
                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                {
                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
                    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                            {
                                new doMiscTypeCode()
                                {
                                    FieldName = MiscType.C_HANDLING_TYPE,
                                    ValueCode = "%"
                                }
                            };

                    ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    listDoMiscTypeCode = hand.GetMiscTypeCodeList(miscs);
                }

                List<doMiscTypeCode> listBillingType = commonHandler.GetMiscTypeCodeList(listDoMiscTypeCode);
                cboModel.SetList<doMiscTypeCode>(listBillingType, "ValueDisplay", "ValueCode");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            return Json(cboModel);
        }
        /// <summary>
        /// Add data to CancelContract grid when click [Add] button in ‘Cancel contract condition’ section
        /// </summary>
        /// <param name="doCancelContractCondition"></param>
        /// <returns></returns>
        public ActionResult CTS140_AddCancel(CTS140_DOCancelContractCondition doCancelContractCondition) //3.1.1 If not pass required field validation (อยู่ใน Model นะครับ)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<CTS140_DOCancelContractGrid> listDOCancelContractGrid;
            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                ICommonContractHandler comContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;

                CTS140_DOCancelContractGrid doCancelContractMemoDetail = new CTS140_DOCancelContractGrid();


                DataEntity.Common.doMiscTypeCode curr = null;
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                #region Fee Amount

                doCancelContractMemoDetail.FeeAmountCurrencyType = doCancelContractCondition.FeeAmountCurrencyType;

                if (doCancelContractCondition.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doCancelContractMemoDetail.FeeAmount = null;

                    if (doCancelContractCondition.FeeAmount != null && doCancelContractCondition.FeeAmount != "none")
                        doCancelContractMemoDetail.FeeAmountUsd = decimal.Parse(doCancelContractCondition.FeeAmount);
                }
                else
                {
                    if (doCancelContractCondition.FeeAmount != null && doCancelContractCondition.FeeAmount != "none")
                        doCancelContractMemoDetail.FeeAmount = decimal.Parse(doCancelContractCondition.FeeAmount);

                    doCancelContractMemoDetail.FeeAmountUsd = null;
                }
                if (currencies != null)
                {
                    curr = currencies.Find(x => x.ValueCode == doCancelContractCondition.FeeAmountCurrencyType);
                    if (curr == null)
                        curr = currencies[0];

                    if (doCancelContractCondition.FeeAmount != null && doCancelContractCondition.FeeAmount != "none")
                        doCancelContractMemoDetail.FeeAmountString = string.Format("{0} {1}", curr.ValueDisplayEN, doCancelContractCondition.FeeAmount);
                }

                #endregion
                #region Tax Amount

                doCancelContractMemoDetail.TaxAmountCurrencyType = doCancelContractCondition.TaxAmountCurrencyType;

                if (doCancelContractCondition.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doCancelContractMemoDetail.TaxAmount = null;

                    if (doCancelContractCondition.TaxAmount != null && doCancelContractCondition.TaxAmount != "none")
                        doCancelContractMemoDetail.TaxAmountUsd = decimal.Parse(doCancelContractCondition.TaxAmount);
                }
                else
                {
                    if (doCancelContractCondition.TaxAmount != null && doCancelContractCondition.TaxAmount != "none")
                        doCancelContractMemoDetail.TaxAmount = decimal.Parse(doCancelContractCondition.TaxAmount);

                    doCancelContractMemoDetail.TaxAmountUsd = null;
                }
                if (currencies != null)
                {
                    curr = currencies.Find(x => x.ValueCode == doCancelContractCondition.TaxAmountCurrencyType);
                    if (curr == null)
                        curr = currencies[0];

                    if (doCancelContractCondition.TaxAmount != null && doCancelContractCondition.TaxAmount != "none")
                        doCancelContractMemoDetail.TaxAmountString = string.Format("{0} {1}", curr.ValueDisplayEN, doCancelContractCondition.TaxAmount);
                }

                #endregion
                
                if (doCancelContractCondition.BillingType != null && doCancelContractCondition.BillingType != "none")
                    doCancelContractMemoDetail.BillingType = doCancelContractCondition.BillingType;

                if (doCancelContractCondition.HandlingType != null && doCancelContractCondition.HandlingType != "none")
                    doCancelContractMemoDetail.HandlingType = doCancelContractCondition.HandlingType;

                if (doCancelContractCondition.StartPeriodDate != null)
                    doCancelContractMemoDetail.StartPeriodDate = doCancelContractCondition.StartPeriodDate;

                if (doCancelContractCondition.EndPeriodDate != null)
                    doCancelContractMemoDetail.EndPeriodDate = doCancelContractCondition.EndPeriodDate;

                if (doCancelContractCondition.Remark != null && doCancelContractCondition.Remark != "none")
                    doCancelContractMemoDetail.Remark = doCancelContractCondition.Remark;
                
                
                ////string lblNormalFee = "Normal fee";
                ////string lblContractCodeForSlideFee = "Contract code for slide fee";
                //string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS140", "lblNormalFee");
                //string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS140", "lblContractCodeForSlideFee");

                //doCancelContractMemoDetail.Remark = String.Format("{0}{1}{2}",
                //        string.IsNullOrEmpty(doCancelContractCondition.Remark) ? string.Empty : string.Format("{0}<br/>", doCancelContractCondition.Remark)
                //        , doCancelContractCondition.ContractCode_CounterBalance == null ? string.Empty : string.Format("{0}: {1}<br/>", lblContractCodeForSlideFee, doCancelContractCondition.ContractCode_CounterBalance)
                //        , doCancelContractCondition.NormalFeeAmount == null ? string.Empty : string.Format("{0}: {1}", lblNormalFee, doCancelContractCondition.NormalFeeAmount));

                if (doCancelContractCondition.ContractCode_CounterBalance != null && doCancelContractCondition.ContractCode_CounterBalance != "none")
                {
                    CommonUtil cmm = new CommonUtil();
                    doCancelContractMemoDetail.ContractCode_CounterBalance = cmm.ConvertContractCode(doCancelContractCondition.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_LONG);
                }

                #region Normal Fee Amount

                doCancelContractMemoDetail.NormalFeeAmountCurrencyType = doCancelContractCondition.NormalFeeAmountCurrencyType;

                if (doCancelContractCondition.NormalFeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    doCancelContractMemoDetail.NormalFeeAmount = null;

                    if (doCancelContractCondition.NormalFeeAmount != null && doCancelContractCondition.NormalFeeAmount != "none")
                        doCancelContractMemoDetail.NormalFeeAmountUsd = decimal.Parse(doCancelContractCondition.NormalFeeAmount);
                }
                else
                {
                    if (doCancelContractCondition.NormalFeeAmount != null && doCancelContractCondition.NormalFeeAmount != "none")
                        doCancelContractMemoDetail.NormalFeeAmount = decimal.Parse(doCancelContractCondition.NormalFeeAmount);

                    doCancelContractMemoDetail.NormalFeeAmountUsd = null;
                }

                #endregion

                doCancelContractMemoDetail.StatusGrid = "ADD";
                doCancelContractMemoDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doCancelContractMemoDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                string strSequence = "0";
                if (param.ListDOCancelContractGrid.Count == 0)
                    strSequence = "0";
                else
                    strSequence = (int.Parse(param.ListDOCancelContractGrid.Max(t => t.Sequence)[0].ToString()) + 1).ToString();

                doCancelContractMemoDetail.Sequence = strSequence;

                MiscTypeMappingList miscList = new MiscTypeMappingList();
                miscList.AddMiscType(doCancelContractMemoDetail);
                commonHandler.MiscTypeMappingList(miscList);

                param.ListDOCancelContractGrid.Add(doCancelContractMemoDetail);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Remove data from CancelContract grid when click [Remove] button in 'Cancel contract condition' section
        /// </summary>
        /// <param name="strSequence"></param>
        /// <returns></returns>
        public ActionResult CTS140_RemoveCancel(string strSequence)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();
                if (param.ListDOCancelContractGrid != null)
                {
                    List<CTS140_DOCancelContractGrid> listDOCancelContractGrid = param.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.Sequence == strSequence; });
                    foreach (var item in listDOCancelContractGrid)
                    {
                        param.ListDOCancelContractGrid.Remove(item);
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
        /// Get cancel contract memo
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS140_GetCancelContractMemo()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();
                res.ResultData = param.DOCancelContractCondition;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get total amount of Fee
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS140_GetCancelGridTotal()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS140_DOCancelContractTotal doCancelContractTotal = new CTS140_DOCancelContractTotal();
                
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();
                if (param.ListDOCancelContractGrid != null)
                {
                    //doCancelContractTotal.TotalSlideAmt = CommonUtil.TextNumeric(param.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) 
                    //    { 
                    //        return s.HandlingType == HandlingType.C_HANDLING_TYPE_SLIDE; 
                    //    }).Sum(s => (s.FeeAmount !=null? s.FeeAmount : 0) + (s.TaxAmount != null? s.TaxAmount : 0)));

                    //doCancelContractTotal.TotalRefundAmt = CommonUtil.TextNumeric(param.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) 
                    //    { 
                    //        return s.HandlingType == HandlingType.C_HANDLING_TYPE_REFUND;
                    //    }).Sum(s => (s.FeeAmount != null ? s.FeeAmount : 0) + (s.TaxAmount != null ? s.TaxAmount : 0)));

                    //doCancelContractTotal.TotalBillingAmt = CommonUtil.TextNumeric(param.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) 
                    //    {
                    //        return s.HandlingType == HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
                    //    }).Sum(s => (s.FeeAmount != null ? s.FeeAmount : 0) + (s.TaxAmount != null ? s.TaxAmount : 0)));

                    //doCancelContractTotal.TotalAmtAfterCounterBalanceType = 
                    //    CommonUtil.TextNumeric((decimal.Parse(doCancelContractTotal.TotalRefundAmt) - decimal.Parse(doCancelContractTotal.TotalBillingAmt)));

                    //if (doCancelContractTotal.TotalSlideAmt == "")
                    //    doCancelContractTotal.TotalSlideAmt = "0";

                    //if (doCancelContractTotal.TotalRefundAmt == "")
                    //    doCancelContractTotal.TotalRefundAmt = "0";

                    //if (doCancelContractTotal.TotalBillingAmt == "")
                    //    doCancelContractTotal.TotalBillingAmt = "0";

                    //if (doCancelContractTotal.TotalSlideAmt == "")
                    //    doCancelContractTotal.TotalAmtAfterCounterBalanceType = "0";

                    //if (decimal.Parse(doCancelContractTotal.TotalRefundAmt) > decimal.Parse(doCancelContractTotal.TotalBillingAmt))
                    //    doCancelContractTotal.IsShowReturnReceive = true;
                    //else
                    //    doCancelContractTotal.IsShowReturnReceive = false;

                    decimal decSlideAmount = 0;
                    decimal decRefundAmount = 0;
                    decimal decBillingAmount = 0;
                    decimal decCounterBalAmount = 0;

                    decimal decSlideAmountUsd = 0;
                    decimal decRefundAmountUsd = 0;
                    decimal decBillingAmountUsd = 0;
                    decimal decCounterBalAmountUsd = 0;

                    foreach (CTS140_DOCancelContractGrid memoDetail in param.ListDOCancelContractGrid)
                    {
                        decimal decFeeAmount = 0;
                        decimal decTaxAmount = 0;
                        decimal decTotalAmount = 0;
                        decimal decFeeAmountUsd = 0;
                        decimal decTaxAmountUsd = 0;
                        decimal decTotalAmountUsd = 0;

                        if (memoDetail.FeeAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            decFeeAmountUsd = memoDetail.FeeAmountUsd == null ? 0 : memoDetail.FeeAmountUsd.Value;
                        else
                            decFeeAmount = memoDetail.FeeAmount == null ? 0 : memoDetail.FeeAmount.Value;

                        if (memoDetail.TaxAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            decTaxAmountUsd = memoDetail.TaxAmountUsd == null ? 0 : memoDetail.TaxAmountUsd.Value;
                        else
                            decTaxAmount = memoDetail.TaxAmount == null ? 0 : memoDetail.TaxAmount.Value;

                        decTotalAmount = (decFeeAmount + decTaxAmount);
                        decTotalAmountUsd = (decFeeAmountUsd + decTaxAmountUsd);

                        if (memoDetail.HandlingType == HandlingType.C_HANDLING_TYPE_SLIDE)
                        {
                            decSlideAmount += decTotalAmount;
                            decSlideAmountUsd += decTotalAmountUsd;
                        }
                        else if (memoDetail.HandlingType == HandlingType.C_HANDLING_TYPE_REFUND)
                        {
                            decRefundAmount += decTotalAmount;
                            decRefundAmountUsd += decTotalAmountUsd;
                        }
                        else if (memoDetail.HandlingType == HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE)
                        {
                            decBillingAmount += decTotalAmount;
                            decBillingAmountUsd += decTotalAmountUsd;
                        }
                    }

                    decCounterBalAmount = decRefundAmount - decBillingAmount;
                    decCounterBalAmountUsd = decRefundAmountUsd - decBillingAmountUsd;
                    
                    doCancelContractTotal.TotalSlideAmt = CommonUtil.TextNumeric(decSlideAmount);
                    doCancelContractTotal.TotalRefundAmt = CommonUtil.TextNumeric(decRefundAmount);
                    doCancelContractTotal.TotalBillingAmt = CommonUtil.TextNumeric(decBillingAmount);
                    doCancelContractTotal.TotalAmtAfterCounterBalanceType = CommonUtil.TextNumeric(decCounterBalAmount);

                    doCancelContractTotal.TotalSlideAmtUsd = CommonUtil.TextNumeric(decSlideAmountUsd);
                    doCancelContractTotal.TotalRefundAmtUsd = CommonUtil.TextNumeric(decRefundAmountUsd);
                    doCancelContractTotal.TotalBillingAmtUsd = CommonUtil.TextNumeric(decBillingAmountUsd);
                    doCancelContractTotal.TotalAmtAfterCounterBalanceTypeUsd = CommonUtil.TextNumeric(decCounterBalAmountUsd);

                    if (decimal.Parse(doCancelContractTotal.TotalRefundAmt) > decimal.Parse(doCancelContractTotal.TotalBillingAmt))
                        doCancelContractTotal.IsShowReturnReceive = true;
                    else
                        doCancelContractTotal.IsShowReturnReceive = false;
                }
                res.ResultData = doCancelContractTotal;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Validate business for CancelContract when click [Add] button on ‘Cancel contract condition’ section
        /// </summary>
        /// <param name="doADDCancelContractCondition"></param>
        /// <returns></returns>
        public ActionResult CTS140_ValidateAddCancelRequireField(CTS140_DOCancelContractCondition doCancelContractCondition)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS140_DOADDCancelContractCondition doADDCancelContractCondition = CommonUtil.CloneObject<CTS140_DOCancelContractCondition, CTS140_DOADDCancelContractCondition>(doCancelContractCondition);
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.ResultData = false;
                        return Json(res);
                    }
                }

                object vDo = doADDCancelContractCondition;
                if (doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
                    || doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                    vDo = CommonUtil.CloneObject<CTS140_DOADDCancelContractCondition, CTS140_DOCancelContractCondition_FeeC1>(doADDCancelContractCondition);
                else if (doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE
                    || doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE
                    || doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
                    vDo = CommonUtil.CloneObject<CTS140_DOADDCancelContractCondition, CTS140_DOCancelContractCondition_FeeC2>(doADDCancelContractCondition);
                else if (doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
                    vDo = CommonUtil.CloneObject<CTS140_DOADDCancelContractCondition, CTS140_DOCancelContractCondition_FeeC3>(doADDCancelContractCondition);
                else if (doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
                    vDo = CommonUtil.CloneObject<CTS140_DOADDCancelContractCondition, CTS140_DOCancelContractCondition_FeeC4>(doADDCancelContractCondition);
                else if (doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                    vDo = CommonUtil.CloneObject<CTS140_DOADDCancelContractCondition, CTS140_DOCancelContractCondition_FeeC5>(doADDCancelContractCondition);
                
                ValidatorUtil.BuildErrorMessage(res, new object[] { vDo });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }


                //ValidateBusiness
                CTS140_ValidateBusinessCancelContract(res, doCancelContractCondition);
                if (res.IsError)
                    return Json(res);

                //ValidateBusinessForWarning
                CTS140_ValidateBusinessCancelContractForWarning(res, doCancelContractCondition);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Validate business when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ActionResult CTS140_Register(CTS140_DOValidateBusiness doValidateBusiness)
        {
            bool isGenerateMAScheduleAgain = false;
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resValidateBusiness = new ObjectResultData();
            ObjectResultData resValidateForWarning = new ObjectResultData();

            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                CommonUtil comUtil = new CommonUtil();

                #region Validate

                //7.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, null, null);
                    return Json(res);
                }

                //Check user’s permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP33, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                CTS140_ValidateProcessAfterCounterBalanceType validProcAftCntBal =
                    CommonUtil.CloneObject<CTS140_DOValidateBusiness, CTS140_ValidateProcessAfterCounterBalanceType>(doValidateBusiness);
                if (validProcAftCntBal.TotalReturnAmt > validProcAftCntBal.TotalBillingAmt)
                {
                    if (validProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND
                        && validProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE)
                    {
                        validProcAftCntBal.ProcessAfterCounterBalanceType = null;
                    }
                }
                else if (validProcAftCntBal.TotalReturnAmt < validProcAftCntBal.TotalBillingAmt)
                {
                    if (validProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL
                        && validProcAftCntBal.ProcessAfterCounterBalanceType != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT)
                    {
                        validProcAftCntBal.ProcessAfterCounterBalanceType = null;
                    }
                }
                else
                {
                    validProcAftCntBal.ProcessAfterCounterBalanceType = "0";
                }




                if (validProcAftCntBal.TotalReturnAmtUsd > validProcAftCntBal.TotalBillingAmtUsd)
                {
                    if (validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND
                        && validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE)
                    {
                        validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd = null;
                    }
                }
                else if (validProcAftCntBal.TotalReturnAmtUsd < validProcAftCntBal.TotalBillingAmtUsd)
                {
                    if (validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL
                        && validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd != ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT)
                    {
                        validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd = null;
                    }
                }
                else
                {
                    validProcAftCntBal.ProcessAfterCounterBalanceTypeUsd = "0";
                }

                //Add by Jutarat A. on 16082012
                CTS140_ValidateLinkageSaleContractCode validLinkage = null;
                if (param.DSRentalContract.dtTbt_RentalContractBasic != null && param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0
                    && param.DSRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                {
                    validLinkage = CommonUtil.CloneObject<CTS140_DOValidateBusiness, CTS140_ValidateLinkageSaleContractCode>(doValidateBusiness);
                }
                //End Add

                if (CommonUtil.IsNullOrEmpty(param.DSRentalContract.dtTbt_RentalContractBasic[0].StartType) == false)
                {
                    //Add by Jutarat A. on 18102013
                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.FirstSecurityStartDate))
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS140",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            new string[] { "lblFirstOperationDate" },
                            new string[] { "FirstSecurityStartDate" });
                    }
                    //End Add

                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.StartDealDate))
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS140",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            new string[] { "lblStartDealDate" },
                            new string[] { "StartDealDate" });
                    }
                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.ContractStartDate))
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS140",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            new string[] { "lblContractDuration" },
                            new string[] { "ContractStartDate" });
                    }
                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.ContractEndDate))
                    {
                        res.AddErrorMessage(
                            MessageUtil.MODULE_CONTRACT,
                            "CTS140",
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            new string[] { "lblContractDuration" },
                            new string[] { "ContractEndDate" });
                    }
                }

                ValidatorUtil.BuildErrorMessage(res, this, new object[] { validProcAftCntBal, validLinkage }, null, false);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                #endregion

                //7.5 Validate business
                resValidateBusiness = CTS140_ValidateBusiness(doValidateBusiness);
                if (resValidateBusiness != null && resValidateBusiness.IsError)
                    return Json(resValidateBusiness);

                //7.2 Map data from screen into dtEntireContract
                //7.2.1 Check the maintenance information
                isGenerateMAScheduleAgain = false;
                if (doValidateBusiness.ContractDurationMonth != param.DOContractRelateInformation.ContractDurationMonth)
                    isGenerateMAScheduleAgain = true;
                else
                {
                    string mac = doValidateBusiness.MaintenanceCycle != "none" ? doValidateBusiness.MaintenanceCycle : null;
                    int? imac = null;
                    int ii = 0;
                    if (int.TryParse(mac, out ii))
                        imac = ii;

                    if (imac != param.DOMaintenanceInformation.MaintenanceCycle)
                        isGenerateMAScheduleAgain = true;
                    else if (param.ListDOMaintenanceGrid != null)
                    {
                        if (param.ListDOMaintenanceGrid.Count() != param.ListDOMaintenanceGridEdit.Count())
                            isGenerateMAScheduleAgain = true;
                        else
                        {
                            foreach (var item in param.ListDOMaintenanceGrid)
                            {
                                if (param.ListDOMaintenanceGridEdit.FindAll(delegate(CTS140_DOMaintenanceGrid s) { return s.ContractCode == item.ContractCode && s.ProductName == item.ProductName; }).Count() == 0)
                                {
                                    isGenerateMAScheduleAgain = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                //7.2.2 Map for removal installation fee
                if (param.ListDOCancelContractGrid != null)
                {
                    foreach (CTS140_DOCancelContractGrid grid in param.ListDOCancelContractGrid)
                    {
                        if (grid.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
                        {
                            if (param.DSRentalContract.dtTbt_RentalSecurityBasic != null)
                            {
                                if (param.DSRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
                                {
                                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFee = grid.NormalFeeAmount;
                                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee = grid.FeeAmount;
                                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = grid.FeeAmount;
                                }
                            }
                        }
                    }
                }

                //Add by Jutarat A. on 21102013
                if (param.DSRentalContract.dtTbt_RentalContractBasic != null && param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0
                        && param.DSRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate != doValidateBusiness.FirstSecurityStartDate)
                {
                    if (param.DSRentalContract.dtTbt_RentalContractBasic[0].LastOCC == OCCType.C_FIRST_IMPLEMENTED_SECURITY_OCC)
                    {
                        param.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeImplementDate = doValidateBusiness.FirstSecurityStartDate;
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate = doValidateBusiness.FirstSecurityStartDate;
                    }
                }
                //End Add

                //Add by Jutarat A. on 16082012
                //7.2.3 Map data from ‘Linkage sale contract code’ into dtTbt_RelationType
                if (param.DSRentalContract.dtTbt_RentalContractBasic != null && param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0
                    && param.DSRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                {
                    if (param.DSRentalContract.dtTbt_RelationType != null)
                    {
                        List<tbt_RelationType> relationTypeList = CommonUtil.ClonsObjectList<tbt_RelationType, tbt_RelationType>(param.DSRentalContract.dtTbt_RelationType);

                        string strRelatedContractCodeLong = comUtil.ConvertContractCode(doValidateBusiness.RelatedContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                        foreach (var item in relationTypeList)
                        {
                            if (item.RelationType == RelationType.C_RELATION_TYPE_SALE)
                            {
                                item.ContractCode = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
                                item.OCC = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
                                item.RelatedContractCode = strRelatedContractCodeLong;
                            }
                        }

                        param.DSRentalContract.dtTbt_RelationType = relationTypeList;
                    }
                }
                //End Add


                //7.3 Map data from 'Maintenance target contract list' into dtTbt_RelationType
                List<tbt_RelationType> ListRelationTypeEdit = CommonUtil.ClonsObjectList<tbt_RelationType, tbt_RelationType>(param.ListRelationType);
                foreach (var item in ListRelationTypeEdit)
                {
                    item.ContractCode = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
                    item.OCC = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
                    item.RelatedContractCode = doValidateBusiness.ContractCode;
                    item.RelationType = RelationType.C_RELATION_TYPE_MA;
                }
                param.ListRelationType = ListRelationTypeEdit;

                //Move up
                ////7.5 Validate business
                //resValidateBusiness = CTS140_ValidateBusiness(doValidateBusiness);
                //if (resValidateBusiness != null && resValidateBusiness.IsError)
                //    return Json(resValidateBusiness);

                param.DSRentalContract.dtTbt_RentalContractBasic[0].UserCode = doValidateBusiness.UserCode; //Add by Jutarat A. on 18102013
                param.DSRentalContract.dtTbt_RentalContractBasic[0].OperationOfficeCode = doValidateBusiness.OperationOfficeCode; //Add by Jutarat A. on 15082012
                
                //ContractRelateInformation
                if (doValidateBusiness.IsShowContractRelatedInformation == true)
                {
                    param.DSRentalContract.dtTbt_RentalContractBasic[0].StartDealDate = doValidateBusiness.StartDealDate;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractStartDate = doValidateBusiness.ContractStartDate;

                    if (param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractEndDate.HasValue
                        && param.DSRentalContract.dtTbt_RentalSecurityBasic[0].CalContractEndDate != doValidateBusiness.ContractEndDate)
                    {
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractEndDate = doValidateBusiness.ContractEndDate;
                    }
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].CalContractEndDate = doValidateBusiness.ContractEndDate;

                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractDurationMonth = doValidateBusiness.ContractDurationMonth;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].AutoRenewMonth = doValidateBusiness.AutoRenewMonth;
                    param.DSRentalContract.dtTbt_RentalContractBasic[0].ProjectCode = doValidateBusiness.ProjectCode;
                }

                //ContractAgreementInformation           
                if (doValidateBusiness.IsShowContractAgreementInformation)
                {
                    param.DSRentalContract.dtTbt_RentalContractBasic[0].ContractOfficeCode = doValidateBusiness.ContractOfficeCode;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PlanCode = doValidateBusiness.PlanCode;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1 = doValidateBusiness.SalesmanEmpNo1;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2 = doValidateBusiness.SalesmanEmpNo2;
                    param.DSRentalContract.dtTbt_RentalContractBasic[0].OldContractCode = comUtil.ConvertContractCode(doValidateBusiness.OldContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    param.DSRentalContract.dtTbt_RentalContractBasic[0].AcquisitionTypeCode = doValidateBusiness.AcquisitionTypeCode;
                    param.DSRentalContract.dtTbt_RentalContractBasic[0].MotivationTypeCode = doValidateBusiness.MotivationTypeCode;
                    param.DSRentalContract.dtTbt_RentalContractBasic[0].IntroducerCode = doValidateBusiness.IntroducerCode;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesSupporterEmpNo = doValidateBusiness.SalesSupporterEmpNo;
                    
                    param.DSRentalContract.dtTbt_RentalContractBasic[0].PaymentDateIncentive = doValidateBusiness.PaymentDateIncentive;

                    param.DSRentalContract.dtTbt_RentalContractBasic[0].QuotationNo = doValidateBusiness.QuotationNo;
                }

                //Deposit Information

                if (doValidateBusiness.IsShowDepositInformation == true)
                {
                    #region Normal Deposit Fee

                    param.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFeeCurrencyType = doValidateBusiness.NormalDepositFeeCurrencyType;

                    if (doValidateBusiness.NormalDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        param.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFee = null;

                        if (doValidateBusiness.NormalDepositFee != null && doValidateBusiness.NormalDepositFee != "")
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFeeUsd = decimal.Parse(doValidateBusiness.NormalDepositFee);
                        else
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFeeUsd = null; 
                    }
                    else
                    {
                        if (doValidateBusiness.NormalDepositFee != null && doValidateBusiness.NormalDepositFee != "")
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFee = decimal.Parse(doValidateBusiness.NormalDepositFee);
                        else
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFee = null; 

                        param.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFeeUsd = null;
                    }

                    #endregion
                    #region Order Deposit Fee

                    param.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFeeCurrencyType = doValidateBusiness.OrderDepositFeeCurrencyType;

                    if (doValidateBusiness.OrderDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        param.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFee = null;

                        if (doValidateBusiness.OrderDepositFee != null && doValidateBusiness.OrderDepositFee != "")
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFeeUsd = decimal.Parse(doValidateBusiness.OrderDepositFee);
                        else
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFeeUsd = null;
                    }
                    else
                    {
                        if (doValidateBusiness.OrderDepositFee != null && doValidateBusiness.OrderDepositFee != "")
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFee = decimal.Parse(doValidateBusiness.OrderDepositFee);
                        else
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFee = null; //Add by Jutarat A. on 06022014

                        param.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFeeUsd = null;
                    }

                    #endregion
                    #region Exempted Deposit Fee

                    param.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFeeCurrencyType = doValidateBusiness.ExemptedDepositFeeCurrencyType;

                    if (doValidateBusiness.ExemptedDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        param.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFee = null;

                        if (doValidateBusiness.ExemptedDepositFee != null && doValidateBusiness.ExemptedDepositFee != "")
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFeeUsd = decimal.Parse(doValidateBusiness.ExemptedDepositFee);
                        else
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFeeUsd = null;
                    }
                    else
                    {
                        if (doValidateBusiness.ExemptedDepositFee != null && doValidateBusiness.ExemptedDepositFee != "")
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFee = decimal.Parse(doValidateBusiness.ExemptedDepositFee);
                        else
                            param.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFee = null; //Add by Jutarat A. on 06022014

                        param.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFeeUsd = null;
                    }

                    #endregion

                    param.DSRentalContract.dtTbt_RentalContractBasic[0].CounterBalanceOriginContractCode = comUtil.ConvertContractCode(doValidateBusiness.CounterBalanceOriginContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                }

                //Contract document information
                if (doValidateBusiness.IsShowContractDocumentInformation == true)
                    param.DSRentalContract.dtTbt_RentalContractBasic[0].IrregurationDocUsageFlag = doValidateBusiness.IrregurationDocUsageFlag;

                //Provide service information

                if (doValidateBusiness.IsShowProvideServiceInformation == true)
                {
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].FireMonitorFlag = doValidateBusiness.FireMonitorFlag;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].CrimePreventFlag = doValidateBusiness.CrimePreventFlag;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag = doValidateBusiness.EmergencyReportFlag;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag = doValidateBusiness.FacilityMonitorFlag;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1 = doValidateBusiness.PhoneLineTypeCode1;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1 = doValidateBusiness.PhoneLineOwnerCode1;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo1 = doValidateBusiness.PhoneNo1;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode2 = doValidateBusiness.PhoneLineTypeCode2;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode2 = doValidateBusiness.PhoneLineOwnerCode2;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo2 = doValidateBusiness.PhoneNo2;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode3 = doValidateBusiness.PhoneLineTypeCode3;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode3 = doValidateBusiness.PhoneLineOwnerCode3;
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo3 = doValidateBusiness.PhoneNo3;
                }

                //Maintenanece Information

                if (doValidateBusiness.IsShowMaintenanceInformation == true)
                {
                    if (param.DSRentalContract.dtTbt_RentalMaintenanceDetails != null && param.DSRentalContract.dtTbt_RentalMaintenanceDetails.Count() > 0)
                    {
                        param.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceTypeCode = doValidateBusiness.MaintenanceTypeCode;

                        if (doValidateBusiness.MaintenanceCycle != null && doValidateBusiness.MaintenanceCycle != "")
                            param.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceCycle = int.Parse(doValidateBusiness.MaintenanceCycle);
                        else
                            param.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceCycle = null; //Add by Jutarat A. on 06022014

                        if (doValidateBusiness.MaintenanceContractStartMonth != null && doValidateBusiness.MaintenanceContractStartMonth != "")
                            param.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartMonth = int.Parse(doValidateBusiness.MaintenanceContractStartMonth);
                        else
                            param.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartMonth = null; //Add by Jutarat A. on 06022014

                        if (doValidateBusiness.MaintenanceContractStartYear != null && doValidateBusiness.MaintenanceContractStartYear != "")
                            param.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartYear = int.Parse(doValidateBusiness.MaintenanceContractStartYear);
                        else
                            param.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartYear = null; //Add by Jutarat A. on 06022014

                        param.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode = doValidateBusiness.MaintenanceFeeTypeCode;
                    }
                }

                //Site Information

                if (doValidateBusiness.IsShowSiteInformation == true)
                {
                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].BuildingTypeCode = doValidateBusiness.BuildingTypeCode;

                    if (doValidateBusiness.SiteBuildingArea != null && doValidateBusiness.SiteBuildingArea != "")
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SiteBuildingArea = decimal.Parse(doValidateBusiness.SiteBuildingArea);
                    else
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SiteBuildingArea = null; //Add by Jutarat A. on 06022014

                    if (doValidateBusiness.NumOfBuilding != null && doValidateBusiness.NumOfBuilding != "")
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfBuilding = int.Parse(doValidateBusiness.NumOfBuilding);
                    else
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfBuilding = null; //Add by Jutarat A. on 06022014

                    if (doValidateBusiness.SecurityAreaFrom != null && doValidateBusiness.SecurityAreaFrom != "")
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom = decimal.Parse(doValidateBusiness.SecurityAreaFrom);
                    else
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom = null; //Add by Jutarat A. on 06022014

                    if (doValidateBusiness.SecurityAreaTo != null && doValidateBusiness.SecurityAreaTo != "")
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaTo = decimal.Parse(doValidateBusiness.SecurityAreaTo);
                    else
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaTo = null; //Add by Jutarat A. on 06022014

                    if (doValidateBusiness.NumOfFloor != null && doValidateBusiness.NumOfFloor != "")
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfFloor = int.Parse(doValidateBusiness.NumOfFloor);
                    else
                        param.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfFloor = null; //Add by Jutarat A. on 06022014

                    param.DSRentalContract.dtTbt_RentalSecurityBasic[0].MainStructureTypeCode = doValidateBusiness.MainStructureTypeCode;

                }

                //Cancel Contract Condition
                if (param.DSRentalContract.dtTbt_CancelContractMemo != null && param.DSRentalContract.dtTbt_CancelContractMemo.Count > 0)
                {
                    param.DSRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType =
                        doValidateBusiness.TotalAmtAfterCounterBalance != 0 ? doValidateBusiness.ProcessAfterCounterBalanceType : null;
                    param.DSRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceTypeUsd =
                        doValidateBusiness.TotalAmtAfterCounterBalanceUsd != 0 ? doValidateBusiness.ProcessAfterCounterBalanceTypeUsd : null;

                    param.DSRentalContract.dtTbt_CancelContractMemo[0].OtherRemarks = doValidateBusiness.OtherRemarks;

                    param.DSRentalContract.dtTbt_CancelContractMemo[0].TotalSlideAmt = doValidateBusiness.TotalSlideAmt;
                    param.DSRentalContract.dtTbt_CancelContractMemo[0].TotalReturnAmt = doValidateBusiness.TotalReturnAmt;
                    param.DSRentalContract.dtTbt_CancelContractMemo[0].TotalBillingAmt = doValidateBusiness.TotalBillingAmt;
                    param.DSRentalContract.dtTbt_CancelContractMemo[0].TotalAmtAfterCounterBalance = doValidateBusiness.TotalAmtAfterCounterBalance;

                    param.DSRentalContract.dtTbt_CancelContractMemo[0].TotalSlideAmtUsd = doValidateBusiness.TotalSlideAmtUsd;
                    param.DSRentalContract.dtTbt_CancelContractMemo[0].TotalReturnAmtUsd = doValidateBusiness.TotalReturnAmtUsd;
                    param.DSRentalContract.dtTbt_CancelContractMemo[0].TotalBillingAmtUsd = doValidateBusiness.TotalBillingAmtUsd;
                    param.DSRentalContract.dtTbt_CancelContractMemo[0].TotalAmtAfterCounterBalanceUsd = doValidateBusiness.TotalAmtAfterCounterBalanceUsd;
                }

                //Other Infromation
                param.DSRentalContract.dtTbt_RentalContractBasic[0].Memo = doValidateBusiness.Memo;

                doValidateBusiness.IsGenerateMAScheduleAgain = isGenerateMAScheduleAgain;
                param.DOValidateBusiness = doValidateBusiness;

                //7.6 Validate business for warning
                resValidateForWarning = CTS140_ValidateForWarning(doValidateBusiness);
                if (resValidateForWarning != null)
                {
                    if (resValidateForWarning.ResultData == null)
                        resValidateForWarning.ResultData = true;
                    return Json(resValidateForWarning);
                }

                //อันนีี้ไม่ได้อย่ใน เอกสารนะครับสงสัยถามพี่ AMP ได้
                if (doValidateBusiness.IsShowCancelContractCondition == true)
                {
                    if (param.ListDOCancelContractGrid.Count() == 0)
                    {
                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3113, null);

                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3113, null, null);
                        res.ResultData = true;
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
        /// <summary>
        /// Validate business when click [Confirm] button in ‘Action button’ section
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ActionResult CTS140_Confirm(CTS140_DOValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resValidateBusiness = new ObjectResultData();
            ObjectResultData resValidateForWarning = new ObjectResultData();

            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IRentralContractHandler rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                //9.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                {
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, null);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, null, null);
                    return Json(res);
                }

                //Check user’s permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP33, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                //9.2 Validate business
                resValidateBusiness = CTS140_ValidateBusiness(doValidateBusiness);
                if (resValidateBusiness != null && resValidateBusiness.IsError)
                    return Json(resValidateBusiness);

                //resValidateForWarning = ValidateForWarning_CTS140(doValidateBusiness);
                //if (resValidateForWarning != null)
                //    return Json(resValidateForWarning);

                //9.3 Checking for update billing temp
                if (doValidateBusiness.IsContractCancelShow == true)
                {
                    IInstallationHandler iHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    List<doGetRemovalData> lst = iHandler.GetRemovalData(param.ContractCodeLong);

                    bool hasOnlyNew = false;
                    bool isRemoval = false;
                    List<tbt_CancelContractMemoDetail> dtMemoDetialList = 
                        param.DSRentalContract.dtTbt_CancelContractMemoDetail.FindAll(delegate(tbt_CancelContractMemoDetail s) 
                        { 
                            return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; 
                        });
                    if (dtMemoDetialList != null)
                    {
                        if (dtMemoDetialList.Count > 0)
                            isRemoval = true;
                    }
                    if (isRemoval == false)
                    {
                        if (param.ListDOCancelContractGrid.Count > 0)
                            hasOnlyNew = true;

                        List<CTS140_DOCancelContractGrid> dtMemoDetialTempList =
                            param.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s)
                            {
                                return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;
                            });
                        if (dtMemoDetialTempList != null)
                        {
                            if (dtMemoDetialTempList.Count > 0)
                                isRemoval = true;
                        }
                    }

                    if (isRemoval == true)
                    {
                        if (lst.Count > 0)
                        {
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3125, null);
                            return Json(res);
                        }
                        else
                        {
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3124, null);
                            return Json(res);
                        }
                    }
                    else if (hasOnlyNew == true)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3292, null);
                        return Json(res);
                    }
                }
            }
            catch (Exception ex)
            {
                res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Update data to database when click [Confirm] button in ‘Action button’ section
        /// </summary>
        /// <param name="isUpdateRemovalFeeToBillingTemp"></param>
        /// <returns></returns>
        public ActionResult CTS140_Save(bool isUpdateRemovalFeeToBillingTemp)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();
                
                List<string> listContractCode = new List<string>();
                foreach (var item in param.ListDOMaintenanceGridEdit)
                {
                    //listContractCode.Add(item.ContractCode);
                    listContractCode.Add(item.ContractCodeLong);
                }

                IRentralContractHandler rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                //rentralContractHandler.RegisterCP33(session.DSRentalContract, session.ListRelationType, listContractCode, isUpdateRemovalFeeToBillingTemp, session.DOValidateBusiness.IsGenerateMAScheduleAgain);
                //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046, null);
                //return Json(res);

                using (TransactionScope scope = new TransactionScope())
                {
                    string strContractCode = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
                    string strOCC = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].OCC;

                    List<tbt_CancelContractMemoDetail> cancelContractMemoDetailList = new List<tbt_CancelContractMemoDetail>();

                    int intSequenceNo = 1;
                    foreach (CTS140_DOCancelContractGrid memoDetailTemp in param.ListDOCancelContractGrid)
                    {
                        tbt_CancelContractMemoDetail memoDetail = new tbt_CancelContractMemoDetail();
                        memoDetail.ContractCode = strContractCode;
                        memoDetail.OCC = strOCC;
                        memoDetail.SequenceNo = intSequenceNo;
                        memoDetail.BillingType = memoDetailTemp.BillingType;
                        memoDetail.HandlingType = memoDetailTemp.HandlingType;
                        memoDetail.StartPeriodDate = memoDetailTemp.StartPeriodDate;
                        memoDetail.EndPeriodDate = memoDetailTemp.EndPeriodDate;

                        #region Fee Amount

                        memoDetail.FeeAmountCurrencyType = memoDetailTemp.FeeAmountCurrencyType;
                        memoDetail.FeeAmount = memoDetailTemp.FeeAmount;
                        memoDetail.FeeAmountUsd = memoDetailTemp.FeeAmountUsd;

                        #endregion
                        #region Tax Amount

                        memoDetail.TaxAmountCurrencyType = memoDetailTemp.TaxAmountCurrencyType;
                        memoDetail.TaxAmount = memoDetailTemp.TaxAmount;
                        memoDetail.TaxAmountUsd = memoDetailTemp.TaxAmountUsd;

                        #endregion
                        #region Normal Fee Amount

                        memoDetail.NormalFeeAmountCurrencyType = memoDetailTemp.NormalFeeAmountCurrencyType;
                        memoDetail.NormalFeeAmount = memoDetailTemp.NormalFeeAmount;
                        memoDetail.NormalFeeAmountUsd = memoDetailTemp.NormalFeeAmountUsd;

                        #endregion

                        memoDetail.ContractCode_CounterBalance = memoDetailTemp.ContractCode_CounterBalance;
                        memoDetail.Remark = memoDetailTemp.Remark;
                        memoDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        memoDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        memoDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        memoDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        cancelContractMemoDetailList.Add(memoDetail);

                        intSequenceNo++;
                    }
                    param.DSRentalContract.dtTbt_CancelContractMemoDetail = cancelContractMemoDetailList;

                    //Add by Jutarat A. 17082012
                    dsRentalContractData dsRentalContract = new dsRentalContractData();

                    List<tbt_RentalContractBasic> rentalContractBasic = CommonUtil.ClonsObjectList<tbt_RentalContractBasic, tbt_RentalContractBasic>(param.DSRentalContract.dtTbt_RentalContractBasic);
                    dsRentalContract.dtTbt_RentalContractBasic = rentalContractBasic;

                    List<tbt_RentalSecurityBasic> rentalSecurityBasic = CommonUtil.ClonsObjectList<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(param.DSRentalContract.dtTbt_RentalSecurityBasic);
                    dsRentalContract.dtTbt_RentalSecurityBasic = rentalSecurityBasic;

                    List<tbt_RentalBEDetails> rentalBEDetails = CommonUtil.ClonsObjectList<tbt_RentalBEDetails, tbt_RentalBEDetails>(param.DSRentalContract.dtTbt_RentalBEDetails);
                    dsRentalContract.dtTbt_RentalBEDetails = rentalBEDetails;

                    List<tbt_RentalInstrumentDetails> rentalInstrumentDetails = CommonUtil.ClonsObjectList<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(param.DSRentalContract.dtTbt_RentalInstrumentDetails);
                    dsRentalContract.dtTbt_RentalInstrumentDetails = rentalInstrumentDetails;

                    List<tbt_RentalInstSubcontractor> rentalInstSubcontractor = CommonUtil.ClonsObjectList<tbt_RentalInstSubcontractor, tbt_RentalInstSubcontractor>(param.DSRentalContract.dtTbt_RentalInstSubcontractor);
                    dsRentalContract.dtTbt_RentalInstSubcontractor = rentalInstSubcontractor;

                    List<tbt_RentalMaintenanceDetails> rentalMaintenanceDetails = CommonUtil.ClonsObjectList<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(param.DSRentalContract.dtTbt_RentalMaintenanceDetails);
                    dsRentalContract.dtTbt_RentalMaintenanceDetails = rentalMaintenanceDetails;

                    List<tbt_RentalOperationType> rentalOperationType = CommonUtil.ClonsObjectList<tbt_RentalOperationType, tbt_RentalOperationType>(param.DSRentalContract.dtTbt_RentalOperationType);
                    dsRentalContract.dtTbt_RentalOperationType = rentalOperationType;

                    List<tbt_RentalSentryGuard> rentalSentryGuard = CommonUtil.ClonsObjectList<tbt_RentalSentryGuard, tbt_RentalSentryGuard>(param.DSRentalContract.dtTbt_RentalSentryGuard);
                    dsRentalContract.dtTbt_RentalSentryGuard = rentalSentryGuard;

                    List<tbt_RentalSentryGuardDetails> rentalSentryGuardDetails = CommonUtil.ClonsObjectList<tbt_RentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(param.DSRentalContract.dtTbt_RentalSentryGuardDetails);
                    dsRentalContract.dtTbt_RentalSentryGuardDetails = rentalSentryGuardDetails;

                    List<tbt_CancelContractMemo> cancelContractMemo = CommonUtil.ClonsObjectList<tbt_CancelContractMemo, tbt_CancelContractMemo>(param.DSRentalContract.dtTbt_CancelContractMemo);
                    dsRentalContract.dtTbt_CancelContractMemo = cancelContractMemo;

                    List<tbt_CancelContractMemoDetail> cancelContractMemoDetail = CommonUtil.ClonsObjectList<tbt_CancelContractMemoDetail, tbt_CancelContractMemoDetail>(param.DSRentalContract.dtTbt_CancelContractMemoDetail);
                    dsRentalContract.dtTbt_CancelContractMemoDetail = cancelContractMemoDetail;

                    List<tbt_RelationType> relationType = CommonUtil.ClonsObjectList<tbt_RelationType, tbt_RelationType>(param.DSRentalContract.dtTbt_RelationType);
                    dsRentalContract.dtTbt_RelationType = relationType;
                    //End Add

                    //Add by Jutarat A. on 18102013
                    bool isUpdateBilling = false; 

                    //9.4	Checking for updating first operation date to billing module
                    if (dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0
                        && dsRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate != param.DOValidateBusiness.FirstSecurityStartDate)
                    {
                        isUpdateBilling = FlagType.C_FLAG_ON;
                        dsRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate = param.DOValidateBusiness.FirstSecurityStartDate;
                    }
                    //End Add

                    //rentralContractHandler.RegisterCP33(param.DSRentalContract, param.ListRelationType, listContractCode, isUpdateRemovalFeeToBillingTemp, param.DOValidateBusiness.IsGenerateMAScheduleAgain);
                    rentralContractHandler.RegisterCP33(dsRentalContract, param.ListRelationType, listContractCode, isUpdateRemovalFeeToBillingTemp, param.DOValidateBusiness.IsGenerateMAScheduleAgain, isUpdateBilling); //Modify (Add isUpdateBilling) by Jutarat A. on 18102013
                                        
                    ICommonContractHandler commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                    commonContractHandler.UpdateOperationOffice(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, dsRentalContract.dtTbt_RentalContractBasic[0].OperationOfficeCode);

                    scope.Complete();

                    //CommonUtil.dsTransData.dtCommonSearch.ContractCode = string.Empty;
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046, null);
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
        /// Initial object data
        /// </summary>
        /// <param name="param"></param>
        public void CTS140_InitialData(CTS140_ScreenParameter param)
        {
            try
            {
                #region Initial contract

                IRentralContractHandler rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                param.DSRentalContract = rentralContractHandler.GetEntireContract(param.ContractCodeLong, null);
                
                #endregion

                if (param.DSRentalContract != null)
                {
                    #region Initial linkage sale contract

                    ICommonContractHandler commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                    param.ListRelationType = commonContractHandler.GetContractLinkageRelation(
                        param.ContractCodeLong,
                        param.DSRentalContract.dtTbt_RentalContractBasic[0].LastOCC,
                        RelationType.C_RELATION_TYPE_SALE);
                        //RelationType.C_RELATION_TYPE_MA);

                    #endregion
                    #region Initial rental security basic

                    param.ListRentalSecurityBasic = rentralContractHandler.GetTbt_RentalSecurityBasic(
                        param.ContractCodeLong, param.DSRentalContract.dtTbt_RentalContractBasic[0].LastOCC);

                    #endregion
                    #region Initial relate information

                    param.DOContractRelateInformation = new CTS140_DOContractRelateInformation();
                    if (param.DSRentalContract.dtTbt_RentalContractBasic != null)
                    {
                        if (param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0)
                        {
                            #region Last Order Contract Fee

                            param.DOContractRelateInformation.LastOrderContractFeeCurrencyType = param.DSRentalContract.dtTbt_RentalContractBasic[0].LastOrderContractFeeCurrencyType;
                            if (param.DOContractRelateInformation.LastOrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                param.DOContractRelateInformation.LastOrderContractFee = param.DSRentalContract.dtTbt_RentalContractBasic[0].LastOrderContractFeeUsd;
                            else
                                param.DOContractRelateInformation.LastOrderContractFee = param.DSRentalContract.dtTbt_RentalContractBasic[0].LastOrderContractFee;

                            #endregion

                            param.DOContractRelateInformation.StartDealDate = param.DSRentalContract.dtTbt_RentalContractBasic[0].StartDealDate;
                            param.DOContractRelateInformation.FirstSecurityStartDate = param.DSRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate;
                            param.DOContractRelateInformation.ContractStartDate = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractStartDate;
                            param.DOContractRelateInformation.ContractEndDate = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].CalContractEndDate;
                            param.DOContractRelateInformation.LastChangeType = param.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeType;
                            param.DOContractRelateInformation.LastChangeTypeCodeName = param.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeTypeCodeName;
                            param.DOContractRelateInformation.ContractDurationMonth = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractDurationMonth;
                            param.DOContractRelateInformation.LastChangeImplementDate = param.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeImplementDate;
                            param.DOContractRelateInformation.AutoRenewMonth = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].AutoRenewMonth;
                            param.DOContractRelateInformation.LastOCC = param.DSRentalContract.dtTbt_RentalContractBasic[0].LastOCC;
                            param.DOContractRelateInformation.ProjectCode = param.DSRentalContract.dtTbt_RentalContractBasic[0].ProjectCode;
                        }
                    }

                    #endregion
                    #region Initial contract agreement information

                    param.DOContractAgreementInformation = new CTS140_DOContractAgreementInformation();
                    if (param.DSRentalContract.dtTbt_RentalContractBasic != null)
                    {
                        if (param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0)
                        {
                            param.DOContractAgreementInformation.ApproveContractDate = param.DSRentalContract.dtTbt_RentalContractBasic[0].ApproveContractDate;
                            param.DOContractAgreementInformation.ContractOfficeCode = param.DSRentalContract.dtTbt_RentalContractBasic[0].ContractOfficeCode;
                            param.DOContractAgreementInformation.OldContractCode = param.DSRentalContract.dtTbt_RentalContractBasic[0].OldContractCode;
                            param.DOContractAgreementInformation.OldContractCodeShort = param.DSRentalContract.dtTbt_RentalContractBasic[0].OldContractCodeShort;
                            param.DOContractAgreementInformation.AcquisitionTypeCode = param.DSRentalContract.dtTbt_RentalContractBasic[0].AcquisitionTypeCode;
                            param.DOContractAgreementInformation.IntroducerCode = param.DSRentalContract.dtTbt_RentalContractBasic[0].IntroducerCode;
                            param.DOContractAgreementInformation.MotivationTypeCode = param.DSRentalContract.dtTbt_RentalContractBasic[0].MotivationTypeCode;

                            param.DOContractAgreementInformation.PaymentDateIncentive = param.DSRentalContract.dtTbt_RentalContractBasic[0].PaymentDateIncentive;
                        }
                    }
                    if (param.DSRentalContract.dtTbt_RentalSecurityBasic != null)
                    {
                        if (param.DSRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
                        {
                            param.DOContractAgreementInformation.QuotationTargetCode = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode;
                            param.DOContractAgreementInformation.QuotationTargetCodeShort = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCodeShort;
                            param.DOContractAgreementInformation.QuotationAlphabet = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet;
                            param.DOContractAgreementInformation.PlanCode = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PlanCode;

                            param.DOContractAgreementInformation.SalesmanEmpNo1 = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1;
                            param.DOContractAgreementInformation.SalesmanEmpNo2 = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2;
                            param.DOContractAgreementInformation.SalesSupporterEmpNo = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesSupporterEmpNo;

                            List<tbm_Employee> empLst = new List<tbm_Employee>();
                            if (param.DOContractAgreementInformation.SalesmanEmpNo1 != null)
                            {
                                empLst.Add(new tbm_Employee()
                                {
                                    EmpNo = param.DOContractAgreementInformation.SalesmanEmpNo1
                                });
                            }
                            if (param.DOContractAgreementInformation.SalesmanEmpNo2 != null)
                            {
                                empLst.Add(new tbm_Employee()
                                {
                                    EmpNo = param.DOContractAgreementInformation.SalesmanEmpNo2
                                });
                            }
                            if (param.DOContractAgreementInformation.SalesSupporterEmpNo != null)
                            {
                                empLst.Add(new tbm_Employee()
                                {
                                    EmpNo = param.DOContractAgreementInformation.SalesSupporterEmpNo
                                });
                            }

                            IEmployeeMasterHandler employeeMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                            List<doActiveEmployeeList> lst = employeeMasterHandler.GetActiveEmployeeList(empLst);
                            if (lst.Count > 0)
                            {
                                foreach (doActiveEmployeeList emp in lst)
                                {
                                    if (param.DOContractAgreementInformation.SalesmanEmpNo1 == emp.EmpNo)
                                        param.DOContractAgreementInformation.SalesmanEmpName1 = emp.EmpFullName;
                                    //else //Comment by Jutarat A. on 06022014
                                    if (param.DOContractAgreementInformation.SalesmanEmpNo2 == emp.EmpNo)
                                        param.DOContractAgreementInformation.SalesmanEmpName2 = emp.EmpFullName;
                                    //else //Comment by Jutarat A. on 06022014
                                    if (param.DOContractAgreementInformation.SalesSupporterEmpNo == emp.EmpNo)
                                        param.DOContractAgreementInformation.SalesSupporterEmpName = emp.EmpFullName;
                                }
                            }

                        }
                    }
                    if (param.DSRentalContract.dtTbt_RelationType != null)
                    {
                        if (param.DSRentalContract.dtTbt_RelationType.Count() != 0)
                        {
                            foreach (tbt_RelationType r in param.DSRentalContract.dtTbt_RelationType)
                            {
                                if (r.RelationType == RelationType.C_RELATION_TYPE_SALE)
                                {
                                    //param.DOContractAgreementInformation.RelatedContractCode = param.DSRentalContract.dtTbt_RelationType[0].RelatedContractCode;
                                    //param.DOContractAgreementInformation.RelatedContractCodeShort = param.DSRentalContract.dtTbt_RelationType[0].RelatedContractCodeShort;
                                    param.DOContractAgreementInformation.RelatedContractCode = r.RelatedContractCode;
                                    param.DOContractAgreementInformation.RelatedContractCodeShort = r.RelatedContractCodeShort;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion
                    #region Initial deposit information

                    param.DODepositInformation = new CTS140_DODepositInformation();
                    if (param.DSRentalContract.dtTbt_RentalContractBasic != null)
                    {
                        if (param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0)
                        {
                            #region Normal Deposit Fee

                            param.DODepositInformation.NormalDepositFeeCurrencyType = param.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFeeCurrencyType;
                            if (param.DODepositInformation.NormalDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                param.DODepositInformation.NormalDepositFee = param.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFeeUsd;
                            else
                                param.DODepositInformation.NormalDepositFee = param.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFee;

                            #endregion
                            #region Order Deposit Fee

                            param.DODepositInformation.OrderDepositFeeCurrencyType = param.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFeeCurrencyType;
                            if (param.DODepositInformation.OrderDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                param.DODepositInformation.OrderDepositFee = param.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFeeUsd;
                            else
                                param.DODepositInformation.OrderDepositFee = param.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFee;

                            #endregion
                            #region Exempted Deposit Fee

                            param.DODepositInformation.ExemptedDepositFeeCurrencyType = param.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFeeCurrencyType;
                            if (param.DODepositInformation.ExemptedDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                param.DODepositInformation.ExemptedDepositFee = param.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFeeUsd;
                            else
                                param.DODepositInformation.ExemptedDepositFee = param.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFee;

                            #endregion

                            param.DODepositInformation.CounterBalanceOriginContractCode = param.DSRentalContract.dtTbt_RentalContractBasic[0].CounterBalanceOriginContractCode;
                            param.DODepositInformation.CounterBalanceOriginContractCodeShort = param.DSRentalContract.dtTbt_RentalContractBasic[0].CounterBalanceOriginContractCodeShort;
                        }
                    }
                    #endregion
                    #region Initial contract document information

                    param.DOContractDocumentInformation = new CTS140_DOContractDocumentInformation();
                    if (param.DSRentalContract.dtTbt_RentalContractBasic != null)
                    {
                        if (param.DSRentalContract.dtTbt_RentalContractBasic.Count() != 0)
                        {
                            param.DOContractDocumentInformation.PODocAuditResult = param.DSRentalContract.dtTbt_RentalContractBasic[0].PODocAuditResult;
                            param.DOContractDocumentInformation.PODocAuditResultCodeName = param.DSRentalContract.dtTbt_RentalContractBasic[0].PODocAuditResultCodeName;
                            param.DOContractDocumentInformation.PODocReceiveDate = param.DSRentalContract.dtTbt_RentalContractBasic[0].PODocReceiveDate;
                            param.DOContractDocumentInformation.ContractDocAuditResult = param.DSRentalContract.dtTbt_RentalContractBasic[0].ContractDocAuditResult;
                            param.DOContractDocumentInformation.ContractDocAuditResultCodeName = param.DSRentalContract.dtTbt_RentalContractBasic[0].ContractDocAuditResultCodeName;
                            param.DOContractDocumentInformation.ContractDocReceiveDate = param.DSRentalContract.dtTbt_RentalContractBasic[0].ContractDocReceiveDate;
                            param.DOContractDocumentInformation.StartMemoAuditResult = param.DSRentalContract.dtTbt_RentalContractBasic[0].StartMemoAuditResult;
                            param.DOContractDocumentInformation.StartMemoAuditResultCodeName = param.DSRentalContract.dtTbt_RentalContractBasic[0].StartMemoAuditResultCodeName;
                            param.DOContractDocumentInformation.StartMemoReceiveDate = param.DSRentalContract.dtTbt_RentalContractBasic[0].StartMemoReceiveDate;

                            if (param.DSRentalContract.dtTbt_RentalContractBasic[0].IrregurationDocUsageFlag != null)
                                param.DOContractDocumentInformation.IrregurationDocUsageFlag = param.DSRentalContract.dtTbt_RentalContractBasic[0].IrregurationDocUsageFlag;
                            else
                                param.DOContractDocumentInformation.IrregurationDocUsageFlag = false;
                        }
                    }

                    #endregion
                    #region Initial provide service information

                    param.DOProvideServiceInformation = new CTS140_DOProvideServiceInformation();
                    if (param.DSRentalContract.dtTbt_RentalSecurityBasic != null)
                    {
                        if (param.DSRentalContract.dtTbt_RentalSecurityBasic.Count != 0)
                        {
                            param.DOProvideServiceInformation.FireMonitorFlag = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].FireMonitorFlag;
                            param.DOProvideServiceInformation.CrimePreventFlag = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].CrimePreventFlag;
                            param.DOProvideServiceInformation.EmergencyReportFlag = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag;
                            param.DOProvideServiceInformation.FacilityMonitorFlag = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag;
                            param.DOProvideServiceInformation.PhoneLineTypeCode1 = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1;
                            param.DOProvideServiceInformation.PhoneLineOwnerCode1 = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1;
                            param.DOProvideServiceInformation.PhoneNo1 = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo1;
                            param.DOProvideServiceInformation.PhoneLineTypeCode2 = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode2;
                            param.DOProvideServiceInformation.PhoneLineOwnerCode2 = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode2;
                            param.DOProvideServiceInformation.PhoneNo2 = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo2;
                            param.DOProvideServiceInformation.PhoneLineTypeCode3 = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode3;
                            param.DOProvideServiceInformation.PhoneLineOwnerCode3 = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode3;
                            param.DOProvideServiceInformation.PhoneNo3 = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo3; 
                        }
                    }

                    #endregion
                    #region Initial maintenance information

                    param.DOMaintenanceInformation = new CTS140_DOMaintenanceInformation();
                    if (param.DSRentalContract.dtTbt_RentalMaintenanceDetails != null)
                    {
                        if (param.DSRentalContract.dtTbt_RentalMaintenanceDetails.Count > 0)
                        {
                            param.DOMaintenanceInformation.MaintenanceTypeCode = param.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceTypeCode;
                            param.DOMaintenanceInformation.MaintenanceCycle = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceCycle;
                            param.DOMaintenanceInformation.MaintenanceContractStartMonth = param.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartMonth;
                            param.DOMaintenanceInformation.MaintenanceContractStartYear = param.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartYear;
                            param.DOMaintenanceInformation.MaintenanceFeeTypeCode = param.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode;
                        }
                    }

                    param.ListDOMaintenanceGrid = new List<CTS140_DOMaintenanceGrid>();
                    param.ListDOMaintenanceGridEdit = new List<CTS140_DOMaintenanceGrid>();
                    if (param.DSRentalContract.dtTbt_RelationType != null)
                    {
                        CTS140_DOMaintenanceGrid doMaintenanceGrid;

                        List<tbt_SaleBasic> lst = new List<tbt_SaleBasic>();
                        foreach (tbt_RelationType rm in param.DSRentalContract.dtTbt_RelationType)
                        {
                            if (CommonUtil.IsNullOrEmpty(rm.RelatedContractCode) == false)
                            {
                                lst.Add(new tbt_SaleBasic()
                                {
                                    ContractCode = rm.RelatedContractCode
                                });
                            }
                        }

                        CommonUtil cmm = new CommonUtil();
                        IContractHandler cthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                        List<doContractHeader> ctLst = cthandler.GetContractHeaderDataByLanguage(lst);
                        if (ctLst.Count > 0)
                        {
                            foreach (tbt_RelationType rm in param.DSRentalContract.dtTbt_RelationType)
                            {
                                foreach (doContractHeader ct in ctLst)
                                {
                                    if (ct.ContractCode == rm.RelatedContractCode)
                                    {
                                        doMaintenanceGrid = new CTS140_DOMaintenanceGrid();
                                        doMaintenanceGrid.ContractCode = cmm.ConvertContractCode(ct.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                                        doMaintenanceGrid.ProductCode = ct.ProductTypeCode;
                                        doMaintenanceGrid.ProductTypeCode = ct.ProductTypeCode;
                                        doMaintenanceGrid.ProductName = ct.ProductName;

                                        param.ListDOMaintenanceGrid.Add(doMaintenanceGrid);
                                        param.ListDOMaintenanceGridEdit.Add(doMaintenanceGrid);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                    #region Initial site information

                    param.DOSiteInformation = new CTS140_DOSiteInformation();
                    if (param.DSRentalContract.dtTbt_RentalSecurityBasic != null)
                    {
                        param.DOSiteInformation.BuildingTypeCode = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].BuildingTypeCode;
                        if (param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SiteBuildingArea != null)
                            param.DOSiteInformation.SiteBuildingArea = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SiteBuildingArea.Value.ToString();
                        else
                            param.DOSiteInformation.SiteBuildingArea = "";

                        param.DOSiteInformation.NumOfBuilding = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfBuilding;
                        if (param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom != null)
                            param.DOSiteInformation.SecurityAreaFrom = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom.ToString();
                        else
                            param.DOSiteInformation.SecurityAreaFrom = "";

                        if (param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaTo != null)
                            param.DOSiteInformation.SecurityAreaTo = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaTo.ToString();
                        else
                            param.DOSiteInformation.SecurityAreaTo = "";

                        param.DOSiteInformation.NumOfFloor = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfFloor;
                        param.DOSiteInformation.MainStructureTypeCode = param.DSRentalContract.dtTbt_RentalSecurityBasic[0].MainStructureTypeCode;
                    }

                    #endregion
                    #region Initial cancel contract condition

                    param.DOCancelContractCondition = new CTS140_DOCancelContractCondition();
                    param.ListDOCancelContractGrid = new List<CTS140_DOCancelContractGrid>();

                    param.DOCancelContractCondition.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL;
                    param.DOCancelContractCondition.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT;
                    param.DOCancelContractCondition.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE;
                    param.DOCancelContractCondition.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND;
                    param.DOCancelContractCondition.C_BILLING_TYPE_CONTRACT_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE;
                    param.DOCancelContractCondition.C_BILLING_TYPE_MAINTENANCE_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE;
                    param.DOCancelContractCondition.C_BILLING_TYPE_DEPOSIT_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE;
                    param.DOCancelContractCondition.C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;
                    param.DOCancelContractCondition.C_BILLING_TYPE_CANCEL_CONTRACT_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE;
                    param.DOCancelContractCondition.C_BILLING_TYPE_CHANGE_INSTALLATION_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE;
                    param.DOCancelContractCondition.C_BILLING_TYPE_CARD_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE;
                    param.DOCancelContractCondition.C_BILLING_TYPE_OTHER_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE;
                    param.DOCancelContractCondition.C_HANDLING_TYPE_BILL_UNPAID_FEE = HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
                    param.DOCancelContractCondition.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE = HandlingType.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE;
                    param.DOCancelContractCondition.C_HANDLING_TYPE_RECEIVE_AS_REVENUE = HandlingType.C_HANDLING_TYPE_RECEIVE_AS_REVENUE;
                    param.DOCancelContractCondition.C_HANDLING_TYPE_REFUND = HandlingType.C_HANDLING_TYPE_REFUND;
                    param.DOCancelContractCondition.C_HANDLING_TYPE_SLIDE = HandlingType.C_HANDLING_TYPE_SLIDE;

                    if (param.DSRentalContract != null)
                    {
                        if (param.DSRentalContract.dtTbt_RentalContractBasic != null)
                        {
                            if (param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0)
                            {
                                param.DOCancelContractCondition.CancelContractDate =
                                    CommonUtil.TextDate(param.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeImplementDate);

                                DateTime? removalInstallationCompleteDate =
                                    rentralContractHandler.GetRemovalInstallCompleteDate(param.DSRentalContract.dtTbt_RentalContractBasic[0].ContractCode);
                                param.DOCancelContractCondition.RemovalInstallationCompleteDate = CommonUtil.TextDate(removalInstallationCompleteDate);
                            }
                        }
                        if (param.DSRentalContract.dtTbt_CancelContractMemo != null)
                        {
                            if (param.DSRentalContract.dtTbt_CancelContractMemo.Count() > 0)
                            {
                                param.DOCancelContractCondition.ProcessAfterCounterBalanceType = 
                                    param.DSRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType;
                                param.DOCancelContractCondition.ProcessAfterCounterBalanceTypeUsd =
                                    param.DSRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceTypeUsd;

                                param.DOCancelContractCondition.OtherRemarks = param.DSRentalContract.dtTbt_CancelContractMemo[0].OtherRemarks;
                            }
                        }
                        if (param.DSRentalContract.dtTbt_CancelContractMemoDetail != null)
                        {
                            int intSequence = -1;
                            foreach (var item in param.DSRentalContract.dtTbt_CancelContractMemoDetail)
                            {
                                CTS140_DOCancelContractGrid doCancelContractGrid 
                                    = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS140_DOCancelContractGrid>(item);
                                doCancelContractGrid.BillingTypeName = item.BillingTypeName;
                                doCancelContractGrid.BillingType = item.BillingType;
                                doCancelContractGrid.HandlingTypeName = item.HandlingTypeName;
                                doCancelContractGrid.HandlingType = item.HandlingType;
                                doCancelContractGrid.NormalFeeAmount = item.NormalFeeAmount;
                                doCancelContractGrid.Sequence = (intSequence + 1).ToString();

                                param.ListDOCancelContractGrid.Add(doCancelContractGrid);
                                intSequence++;
                            }
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// ValidateBusiness Cancel Contract
        /// </summary>
        /// <param name="doCancelContractCondition"></param>
        /// <returns></returns>
        private void CTS140_ValidateBusinessCancelContract(ObjectResultData res, CTS140_DOCancelContractCondition doCancelContractCondition)
        {
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            List<CTS140_DOCancelContractGrid> listDOCancelContractGrid;
            
            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                ICommonContractHandler comContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;

                //3.2.1 When dtpPeriodFrom and dtpPeriodTo is a required field
                if (doCancelContractCondition.StartPeriodDate > doCancelContractCondition.EndPeriodDate)
                {
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0101, null);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0101, null, new string[] { "StartPeriodDateContract" });
                    return;
                }

                //3.2.2	When ddlFeeType is one of the following items
                //-	C_BILLING_TYPE_DEPOSIT_FEE
                //-	C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE
                //-	C_BILLING_TYPE_CANCEL_CONTRACT_FEE
                //-	C_BILLING_TYPE_CHANGE_INSTALLATION_FEE
                //-	C_BILLING_TYPE_CARD_FEE
                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE ||
                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE ||
                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE ||
                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE ||
                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE)
                {
                    listDOCancelContractGrid = param.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == doCancelContractCondition.BillingType; });
                    if (listDOCancelContractGrid.Count() != 0)
                    {
                        //3.2.2.1 The adding fee type must not existing in ‘Cancel contract condition list’ Show Show message when not pass validation
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null);
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null, new string[] { "FeeType" });
                        return;
                    }
                }

                //3.2.3 When ddlFeeType = C_BILLING_TYPE_CONTRACT_FEE
                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                {
                    //3.2.3.2 Maintenanace fee must not registered in ‘Cancel contract condition list’
                    if (param.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE; }).Count() != 0)
                    {
                        string[] message = new string[] { doCancelContractCondition.C_ContractFeeValidate, doCancelContractCondition.C_MaintenanceFeeValidate };
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message);
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message, new string[] { "FeeType" });
                        return;
                    }
                }
                //3.2.4
                else if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                {
                    if (param.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE; }).Count() != 0)
                    {
                        string[] message = new string[] { doCancelContractCondition.C_MaintenanceFeeValidate, doCancelContractCondition.C_ContractFeeValidate };
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message);
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message, new string[] { "FeeType" });
                        return;
                    }
                }
                else if (doCancelContractCondition.BillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
                {
                    //ไม่ได้อยู่ใน Spec นะครับเป็นการเช็คไม่ให้ Add ซ้ำครับ
                    if (param.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == doCancelContractCondition.BillingType && s.HandlingType == doCancelContractCondition.HandlingType; }).Count() != 0)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null);
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null, new string[] { "FeeType", "HandlingType" });
                        return;
                    }
                }

                //3.2.5
                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
                {
                    //if (session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; }).Count() == 0)
                    if (param.DSRentalContract.dtTbt_CancelContractMemoDetail.FindAll(delegate(tbt_CancelContractMemoDetail s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; }).Count() == 0)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3122, null);
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3122, null, new string[] { "FeeType" });
                        return;
                    }
                }

                if (doCancelContractCondition.ContractCode_CounterBalance != null
                    && doCancelContractCondition.ContractCode_CounterBalance != "none")
                {
                    if (String.IsNullOrEmpty(doCancelContractCondition.ContractCode_CounterBalance) == false)
                    {
                        CommonUtil cmm = new CommonUtil();
                        string strContractCode_CounterBalanceLong = cmm.ConvertContractCode(doCancelContractCondition.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_LONG);
                        if (comContractHandler.IsContractExistInRentalOrSale(strContractCode_CounterBalanceLong) == false)
                        {
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, doCancelContractCondition.ContractCode_CounterBalance);
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, new string[] { doCancelContractCondition.ContractCode_CounterBalance }, new string[] { "ContractCode_CounterBalance" });
                            return;
                        }

                        //Bug report CT-148
                        if (param.ContractCodeLong == strContractCode_CounterBalanceLong.ToUpper())
                        {
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281);
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281, null, new string[] { "ContractCode_CounterBalance" });
                            return;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }
        /// <summary>
        /// ValidateBusiness Cancel Contract (Warning)
        /// </summary>
        /// <param name="doCancelContractCondition"></param>
        /// <returns></returns>
        private void CTS140_ValidateBusinessCancelContractForWarning(ObjectResultData res, CTS140_DOCancelContractCondition doCancelContractCondition)
        {
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;

            try
            {
                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
                    || doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
                {
                    if (CTS140_ValidatePeriodverLap(doCancelContractCondition.StartPeriodDate, doCancelContractCondition.EndPeriodDate, doCancelContractCondition.BillingType))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3282);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }
        /// <summary>
        /// Check PeriodDate is Overlap
        /// </summary>
        /// <param name="startPeriodDate"></param>
        /// <param name="endPeriodDate"></param>
        /// <param name="strBillingType"></param>
        /// <returns></returns>
        private bool CTS140_ValidatePeriodverLap(DateTime? startPeriodDate, DateTime? endPeriodDate, string strBillingType)
        {
            bool bResult = false;

            CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();
            foreach (CTS140_DOCancelContractGrid memoDetail in param.ListDOCancelContractGrid)
            {
                if ((strBillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE && memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                    || (strBillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE && memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE))
                {
                    if (CheckOverlapPeriodDate_CTS110(memoDetail.StartPeriodDate, memoDetail.EndPeriodDate, startPeriodDate, endPeriodDate))
                    {
                        bResult = true;
                        break;
                    }
                }
            }

            return bResult;
        }
        /// <summary>
        /// Validate business of screen
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ObjectResultData CTS140_ValidateBusiness(CTS140_DOValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                IEmployeeMasterHandler employeeMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                IProjectHandler projectHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                IRentralContractHandler rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ICustomerMasterHandler customerMasterHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
                ICommonContractHandler subContractorMasterHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                ICommonContractHandler comContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                CommonUtil comUtil = new CommonUtil();

                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();

                //3. VaidateBusiness                
                if (doValidateBusiness.IsShowContractRelatedInformation == true)
                {
                    //3.1 If dtpStartDealDate > TODAY Then
                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.StartDealDate) == false)
                    {
                        if (doValidateBusiness.StartDealDate.Value.Date > DateTime.Now.Date)
                        {
                            //string[] param = { "Start deal date" };
                            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3126, "Start deal date");

                            //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3126, param, null);
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                ScreenID.C_SCREEN_ID_CP33,
                                                MessageUtil.MODULE_CONTRACT,
                                                MessageUtil.MessageList.MSG3126,
                                                new string[] { "lblStartDealDate" },
                                                new string[] { "StartDealDate" });
                            return res;
                        }
                    }

                    //Add by Jutarat A. on 18102013
                    //3.2	Get change implement date of ‘9980’ OCC
                    List<tbt_RentalSecurityBasic> dtRentalSecurityBasicForValidation = rentralContractHandler.GetTbt_RentalSecurityBasic(param.ContractCodeLong, "9980");

                    //3.3	If dtRentalSecurityBasicForValidation is not null 
                    //AND dtpFirstOperationDate > dtRentalSecurityBasicForValidation.ChangeImplementDate Then
                    if (dtRentalSecurityBasicForValidation != null && dtRentalSecurityBasicForValidation.Count > 0)
                    {
                        if (doValidateBusiness.FirstSecurityStartDate > dtRentalSecurityBasicForValidation[0].ChangeImplementDate)
                        {
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3310, CommonUtil.TextDate(dtRentalSecurityBasicForValidation[0].ChangeImplementDate));

                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                ScreenID.C_SCREEN_ID_CP33,
                                                MessageUtil.MODULE_CONTRACT,
                                                MessageUtil.MessageList.MSG3310,
                                                new string[] { CommonUtil.TextDate(dtRentalSecurityBasicForValidation[0].ChangeImplementDate) },
                                                new string[] { "FirstSecurityStartDate" });
                            return res;
                        }
                    }
                    //End Add

                    //Add by Jutarat A. on 21102013
                    //3.4	If dtTbt_RentalContractBasic.ProductTypeCode in (C_PROD_TYPE_AL, C_PROD_TYPE_RENTAL_SALE) AND Month of dtpFirstOperationDate is changed Then
                    if (param.DSRentalContract.dtTbt_RentalContractBasic != null && param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0
                        && (param.DSRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL
                            || param.DSRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                        && (param.DSRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate.HasValue && doValidateBusiness.FirstSecurityStartDate.HasValue
                            && (param.DSRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate.Value.Month != doValidateBusiness.FirstSecurityStartDate.Value.Month
                                || param.DSRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate.Value.Year != doValidateBusiness.FirstSecurityStartDate.Value.Year)))
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3311);

                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                            ScreenID.C_SCREEN_ID_CP33,
                                            MessageUtil.MODULE_CONTRACT,
                                            MessageUtil.MessageList.MSG3311,
                                            null,
                                            new string[] { "FirstSecurityStartDate" });
                        return res;
                    }
                    //End Add

                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.ContractStartDate) == false)
                    {
                        //3.2 If dtpContractDurationStartDate > TODAY Then
                        if (doValidateBusiness.ContractStartDate.Value.Date > DateTime.Now.Date)
                        {
                            //string[] param = { "Contract duration start date" };
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3126, "Contract duration start date");

                            //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3126, param, null);
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                                ScreenID.C_SCREEN_ID_CP33,
                                                MessageUtil.MODULE_CONTRACT,
                                                MessageUtil.MessageList.MSG3126,
                                                new string[] { "lblContractDuration" },
                                                new string[] { "ContractStartDate" });
                            return res;
                        }
                    }
                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.ContractEndDate) == false)
                    {
                        //3.3 If dtpContractDurationEndDate <= dtpContractDurationStartDate Then
                        if (doValidateBusiness.ContractEndDate.Value.Date <= doValidateBusiness.ContractStartDate.Value.Date)
                        {
                            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3127, null);
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3127, null, new string[] { "ContractEndDate" });
                            return res;
                        }
                    }

                    //3.7 If txtProjectCode is not existing in tbt_Project.ProjectCode Then
                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.ProjectCode) == false)
                    {
                        if (projectHandler.GetTbt_Project(doValidateBusiness.ProjectCode).Count() == 0)
                        {
                            string[] paramx = { doValidateBusiness.ProjectCode };
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0091, doValidateBusiness.ProjectCode);
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0091, paramx, new string[] { "ProjectCode" });
                            return res;
                        }
                    }

                    //Add by Jutarat A. on 16082012
                    //3.8	If dtTbt_RentalContractBasic.ProductTypeCode = C_PROD_TYPE_ONLINE  
                    //      And txtLinkageSaleContractCode is not existing in tbt_SaleBasic.ContractCode Then
                    if (param.DSRentalContract.dtTbt_RentalContractBasic != null && param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0
                        && param.DSRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                    {
                        string strRelatedContractCodeLong = comUtil.ConvertContractCode(doValidateBusiness.RelatedContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                        List<tbt_SaleBasic> listSaleBasic = saleHandler.GetTbt_SaleBasic(strRelatedContractCodeLong, null, null);
                        if (listSaleBasic == null || listSaleBasic.Count < 1)
                        {
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, doValidateBusiness.RelatedContractCode);
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, new string[] { doValidateBusiness.RelatedContractCode }, new string[] { "RelatedContractCode" });
                            return res;
                        }
                    }
                    //End Add
                }

                if (doValidateBusiness.IsShowContractAgreementInformation == true)
                {
                    //3.4 If txtSalesman1Code is not existing in the tbm_Employee.EmpNo Then
                    if (doValidateBusiness.SalesmanEmpNo1 != "none")
                    {
                        if (doValidateBusiness.SalesmanEmpNo1 != null)
                        {
                            if (employeeMasterHandler.GetActiveEmployee(doValidateBusiness.SalesmanEmpNo1).Count() == 0)
                            {
                                string[] paramx = { doValidateBusiness.SalesmanEmpNo1 };
                                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.SalesmanEmpNo1);
                                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, paramx, new string[] { "SalesmanEmpNo1" });
                                return res;
                            }
                        }
                    }

                    //3.5 If txtSalesman2Code is not existing in the tbm_Employee.EmpNo Then

                    if (doValidateBusiness.SalesmanEmpNo2 != null)
                    {
                        if (employeeMasterHandler.GetActiveEmployee(doValidateBusiness.SalesmanEmpNo2).Count() == 0)
                        {
                            string[] paramx = { doValidateBusiness.SalesmanEmpNo2 };
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.SalesmanEmpNo2);
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, paramx, new string[] { "SalesmanEmpNo2" });
                            return res;
                        }
                    }

                    //3.6 If txtSalesSupporterCode is not existing in the tbm_Employee.EmpNo Then

                    if (doValidateBusiness.SalesSupporterEmpNo != null)
                    {
                        if (employeeMasterHandler.GetActiveEmployee(doValidateBusiness.SalesSupporterEmpNo).Count() == 0)
                        {
                            string[] paramx = { doValidateBusiness.SalesSupporterEmpNo };
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.SalesSupporterEmpNo);
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, paramx, new string[] { "SalesSupporterEmpNo" });
                            return res;
                        }
                    }

                    //3.8 If txtOldContractCode is not existing in tbt_RentalContractBasic.ContractCode Then
                    string strOldContractCodeLong = comUtil.ConvertContractCode(doValidateBusiness.OldContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    if (rentralContractHandler.GetTbt_RentalContractBasic(strOldContractCodeLong, null).Count() == 0)
                    {
                        string[] paramx = { doValidateBusiness.OldContractCode };
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, doValidateBusiness.OldContractCode);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, paramx, new string[] { "OldContractCode" });
                        return res;
                    }

                    //3.10 Validate introducer code
                    //3.10.1 If ddlAcquisitionType = C_ACQUISITION_TYPE_CUST Then
                    //if (doValidateBusiness.AcquisitionTypeCode == AcquisitionType.C_ACQUISITION_TYPE_CUST)
                    //{
                    //    CommonUtil cmm = new CommonUtil();
                    //    ListDOCustomer = customerMasterHandler.GetCustomer(cmm.ConvertCustCode(doValidateBusiness.IntroducerCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                    //    if (ListDOCustomer != null)
                    //    {
                    //        if (ListDOCustomer.Count() == 0)
                    //        {
                    //            string[] param = { doValidateBusiness.CounterBalanceOriginContractCode };
                    //            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3128, doValidateBusiness.CounterBalanceOriginContractCode);

                    //            //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3128, param);
                    //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                    //                        ScreenID.C_SCREEN_ID_CP33,
                    //                        MessageUtil.MODULE_CONTRACT,
                    //                        MessageUtil.MessageList.MSG3128,
                    //                        new string[] { "lblCustomerCodeMsg" },
                    //                        new string[] { "IntroducerCode" });
                    //            return res;
                    //        }
                    //    }
                    //}

                    ////3.10.2 If ddlAcquisitionType = C_ACQUISITION_TYPE_INSIDE_COMPANY Then
                    //if (doValidateBusiness.AcquisitionTypeCode == AcquisitionType.C_ACQUISITION_TYPE_INSIDE_COMPANY)
                    //{
                    //    ListDTEmployee = employeeMasterHandler.GetEmployee(doValidateBusiness.IntroducerCode, null, null);
                    //    if (ListDTEmployee != null)
                    //    {
                    //        if (ListDTEmployee.Count() == 0)
                    //        {
                    //            string[] param = { doValidateBusiness.IntroducerCode };
                    //            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3128, doValidateBusiness.IntroducerCode);

                    //            //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3128, param);
                    //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                    //                        ScreenID.C_SCREEN_ID_CP33,
                    //                        MessageUtil.MODULE_CONTRACT,
                    //                        MessageUtil.MessageList.MSG3128,
                    //                        new string[] { "lblEmployeeCodeMsg" },
                    //                        new string[] { "IntroducerCode" });
                    //            return res;
                    //        }
                    //    }
                    //}

                    ////3.10.3 If ddlAcquisitionType = C_ACQUISITION_TYPE_SUBCONTRACTOR Then
                    //if (doValidateBusiness.AcquisitionTypeCode == AcquisitionType.C_ACQUISITION_TYPE_SUBCONTRACTOR)
                    //{
                    //    ListDOSubcontractorDetails = subContractorMasterHandler.GetTbm_SubContractorData(doValidateBusiness.IntroducerCode);
                    //    if (ListDOSubcontractorDetails != null)
                    //    {
                    //        if (ListDOSubcontractorDetails.Count() == 0)
                    //        {
                    //            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3128, doValidateBusiness.IntroducerCode);
                    //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                    //            ScreenID.C_SCREEN_ID_CP33,
                    //            MessageUtil.MODULE_CONTRACT,
                    //            MessageUtil.MessageList.MSG3128,
                    //            new string[] { "lblSubcontractorCodeMsg" },
                    //            new string[] { "IntroducerCode" });
                    //            return res;
                    //        }
                    //    }
                    //}

                    // 3.11 Validate Project code
                    //CTS140_ScreenParameter session = CTS140_GetImportData();
                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.ProjectCode) == false
                        && doValidateBusiness.ProjectCode != param.DSRentalContract.dtTbt_RentalContractBasic[0].ProjectCode)
                    {
#if !ROUND1

                        IInstallationHandler iHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                        string status = iHandler.GetInstallationStatus(doValidateBusiness.ProjectCode);
                        if (status != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                        {
                            res.ResultData = MessageUtil.GetMessage(
                                MessageUtil.MODULE_CONTRACT,
                                MessageUtil.MessageList.MSG3274,
                                doValidateBusiness.ProjectCode);
                            res.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                MessageUtil.MessageList.MSG3274,
                                new string[] { doValidateBusiness.ProjectCode },
                                new string[] { "ProjectCode" });
                            return res;
                        }

#endif

                        bool isError = true;
                        List<string> lst = projectHandler.GetProjectStatus(doValidateBusiness.ProjectCode);
                        if (lst.Count > 0)
                        {
                            if (lst[0] == ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE)
                                isError = false;
                        }
                        if (isError)
                        {
                            res.ResultData = MessageUtil.GetMessage(
                                MessageUtil.MODULE_CONTRACT,
                                MessageUtil.MessageList.MSG3260,
                                doValidateBusiness.ProjectCode);
                            res.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                ScreenID.C_SCREEN_ID_CP33,
                                MessageUtil.MODULE_CONTRACT,
                                MessageUtil.MessageList.MSG3260,
                                new string[] { doValidateBusiness.ProjectCode },
                                new string[] { "ProjectCode" });
                            return res;
                        }
                    }

                    if (doValidateBusiness.PaymentDateIncentive != null)
                    {
                        DateTime currentDate = DateTime.Now;
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);

                        if (doValidateBusiness.PaymentDateIncentive.Value.CompareTo(currentDate) > 0)
                        {
                            res.AddErrorMessage(
                                MessageUtil.MODULE_CONTRACT,
                                ScreenID.C_SCREEN_ID_CP33,
                                MessageUtil.MODULE_CONTRACT,
                                MessageUtil.MessageList.MSG3350,
                                null,
                                new string[] { "PaymentDateIncentive" });
                            return res;
                        }
                    }
                }

                //3.12 Validate maintenance target contract code
                if (doValidateBusiness.IsShowMaintenanceInformation == true)
                {
                    //CTS140_ScreenParameter session = CTS140_GetImportData();
                    if (param.ListDOMaintenanceGridEdit != null)
                    {
                        List<string> contractLst = new List<string>();
                        foreach (CTS140_DOMaintenanceGrid grid in param.ListDOMaintenanceGridEdit)
                        {
                            contractLst.Add(grid.ContractCodeLong);
                        }

                        IContractHandler contractHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;

                        try
                        {
                            bool isSiteError = true;
                            if (param.DSRentalContract.dtTbt_RentalContractBasic != null)
                            {
                                if (param.DSRentalContract.dtTbt_RentalContractBasic.Count > 0)
                                {
                                    List<doContractHeader> contLst = contractHandler.CheckMaintenanceTargetContractList(
                                        contractLst,
                                        param.DSRentalContract.dtTbt_RentalContractBasic[0].ContractCode);

                                    isSiteError = false;
                                    List<doContractHeader> lst = contLst;
                                    foreach (doContractHeader l in lst)
                                    {
                                        if (l.SiteCode != param.DSRentalContract.dtTbt_RentalContractBasic[0].SiteCode)
                                        {
                                            isSiteError = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (isSiteError)
                            {
                                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3093);
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3093);
                                return res;
                            }
                        }
                        catch (ApplicationErrorException ex)
                        {
                            if (res.MessageList == null)
                                res.AddErrorMessage(ex);
                            else
                                res.MessageList.AddRange(ex.ErrorResult.MessageList);

                            return res;
                        }
                    }
                }

                if (doValidateBusiness.IsShowDepositInformation == true)
                {
                    //3.9 If txtOriginContractCodeForSlide is not existing in tbt_RentalContractBasic.ContractCode and tbt_SaleBasic.ContractCode Then
                    string strCounterBalanceOriginContractCodeLong = comUtil.ConvertContractCode(doValidateBusiness.CounterBalanceOriginContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    //if (rentralContractHandler.GetTbt_RentalContractBasic(strCounterBalanceOriginContractCodeLong, null).Count() == 0)
                    if (comContractHandler.IsContractExistInRentalOrSale(strCounterBalanceOriginContractCodeLong) == false)
                    {
                        string[] paramx = { doValidateBusiness.CounterBalanceOriginContractCode };
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, doValidateBusiness.CounterBalanceOriginContractCode);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, paramx, new string[] { "CounterBalanceOriginContractCode" });
                        return res;
                    }

                    //Bug report CT-148
                    if (param.ContractCodeLong == strCounterBalanceOriginContractCodeLong)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281);
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281, null, new string[] { "CounterBalanceOriginContractCode" });
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            //return null;
            return res;
        }
        /// <summary>
        /// Validate data and show warning message
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ObjectResultData CTS140_ValidateForWarning(CTS140_DOValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS140_ScreenParameter param = GetScreenObject<CTS140_ScreenParameter>();

                if (doValidateBusiness.MaintenanceFeeTypeCode == "none")
                    doValidateBusiness.MaintenanceFeeTypeCode = null;
                if (param.DOMaintenanceInformation.MaintenanceFeeTypeCode != doValidateBusiness.MaintenanceFeeTypeCode)
                {
                    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3151, null);

                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3151, null, null);
                    return res;
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return null;
        }

        #endregion





//        #region Method

//        /// <summary>
//        /// Initial data of BillingType to ComboBox
//        /// </summary>
//        /// <param name="doCancelContractCondition"></param>
//        /// <returns></returns>
//        public ActionResult InitialBillingType_CTS140(CTS140_DOCancelContractCondition doCancelContractCondition)
//        {
//            ObjectResultData res = new ObjectResultData();
//            ICommonHandler commonHandler;
//            List<doMiscTypeCode> listBillingType;
//            List<doMiscTypeCode> listDoMiscTypeCode;
//            ComboBoxModel cboModel;
//            doMiscTypeCode doMiscType;

//            CTS051_ScreenParameter session;
//            try
//            {
//                session = CTS051_GetImportData();
//                cboModel = new ComboBoxModel();
//                listDoMiscTypeCode = new List<doMiscTypeCode>();
//                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

//                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
//                {
//                    doMiscType = new doMiscTypeCode();
//                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
//                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
//                    listDoMiscTypeCode.Add(doMiscType);

//                    doMiscType = new doMiscTypeCode();
//                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE;
//                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
//                    listDoMiscTypeCode.Add(doMiscType);
//                }

//                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
//                {
//                    doMiscType = new doMiscTypeCode();
//                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
//                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
//                    listDoMiscTypeCode.Add(doMiscType);
//                }

//                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
//                {
//                    doMiscType = new doMiscTypeCode();
//                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
//                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
//                    listDoMiscTypeCode.Add(doMiscType);

//                    doMiscType = new doMiscTypeCode();
//                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE;
//                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
//                    listDoMiscTypeCode.Add(doMiscType);
//                }

//                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE)
//                {
//                    doMiscType = new doMiscTypeCode();
//                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
//                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
//                    listDoMiscTypeCode.Add(doMiscType);

//                    doMiscType = new doMiscTypeCode();
//                    doMiscType.ValueCode = SECOM_AJIS.Common.Util.ConstantValue.HandlingType.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE;
//                    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
//                    listDoMiscTypeCode.Add(doMiscType);
//                }

//                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE ||
//                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE ||
//                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE ||
//                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
//                {
//                    List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
//                    List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
//                    {
//                        new doMiscTypeCode()
//                        {
//                            FieldName = MiscType.C_HANDLING_TYPE,
//                            ValueCode = "%"
//                        }
//                    };

//                    ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
//                    listDoMiscTypeCode = hand.GetMiscTypeCodeList(miscs);
//                }

//                listBillingType = commonHandler.GetMiscTypeCodeList(listDoMiscTypeCode);
//                cboModel.SetList<doMiscTypeCode>(listBillingType, "ValueDisplay", "ValueCode");
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//                return Json(res);
//            }

//            return Json(cboModel);
//        }

//        /// <summary>
//        /// Initial screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult InitialScreen_CTS140(string contractCode)
//        {
//            IRentralContractHandler rentralContractHandler;
//            ICommonContractHandler commonContractHandler;

//            IUserControlHandler userControlHandler;
//            ObjectResultData res = new ObjectResultData();
//            dsRentalContractData dsRentalContract;
//            doRentalContractBasicInformation doRental;

//            List<string> listFieldName = new List<string>();
//            List<doMiscTypeCode> listMistTypeCode;
//            List<doMiscTypeCode> listMistTypeCodeNew;
//            List<tbt_RelationType> ListRelationType;

//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                userControlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
//                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
//                commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;

//                //1.Event Initial screen
//                //1.1 Initial contract data
//                //1.1.1 Get entire contract 
//                dsRentalContract = rentralContractHandler.GetEntireContract(contractCode.Trim(), null);

//                //1.1.2 Get linkage sale contract
//                if (dsRentalContract != null)
//                {
//                    if (dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
//                    {
//                        ListRelationType = commonContractHandler.GetContractLinkageRelation(contractCode.Trim(), dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC, RelationType.C_RELATION_TYPE_MA);
//                        doRental = userControlHandler.GetRentalContactBasicInformationData(contractCode.Trim());

//                        session.DSRentalContract = dsRentalContract;
//                        session.ListRentalSecurityBasic = rentralContractHandler.GetTbt_RentalSecurityBasic(contractCode.Trim(), dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC);
//                        session.ListRelationType = ListRelationType;
//                        BindDOContractBasicInformation_CTS140(doRental);

//                        BindDOContractRelateInformation_CTS140();
//                        BindDOContractAgreementInformation_CTS140();
//                        BindDODepositInformation_CTS140();
//                        BindDOContractDocumentInformation_CTS140();
//                        BindDOProvideServiceInformation_CTS140();
//                        BindDOMaintenanceInformation_CTS140();
//                        BindDOSiteInformation_CTS140();
//                        BindCancelContractCondition_CTS140();

//                        //BindOtherInformation_CTS140();
//                        ViewBag.Memo = dsRentalContract.dtTbt_RentalContractBasic[0].Memo;

//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Initial session of screen
//        /// </summary>
//        /// <returns></returns>
//        public CTS140_ScreenParameter InitialScreenSession_CTS140()
//        {
//            try
//            {
//                CTS140_ScreenParameter importData = new CTS140_ScreenParameter()
//                {
//                    DSRentalContract = new dsRentalContractData(),

//                    ListRentalSecurityBasic = new List<tbt_RentalSecurityBasic>(),
//                    ListDOMaintenanceGrid = new List<CTS140_DOMaintenanceGrid>(),
//                    ListDOMaintenanceGridEdit = new List<CTS140_DOMaintenanceGrid>(),
//                    ListDOCancelContractGrid = new List<CTS140_DOCancelContractGrid>(),
//                    ListDOCancelContractGridRemove = new List<string>(),

//                    DOValidateBusiness = new CTS140_DOValidateBusiness(),
//                    DOContractRelateInformation = new CTS140_DOContractRelateInformation(),
//                    DOContractAgreementInformation = new CTS140_DOContractAgreementInformation(),
//                    DODepositInformation = new CTS140_DODepositInformation(),
//                    DOContractDocumentInformation = new CTS140_DOContractDocumentInformation(),
//                    DOProvideServiceInformation = new CTS140_DOProvideServiceInformation(),
//                    DOMaintenanceInformation = new CTS140_DOMaintenanceInformation(),
//                    DOSiteInformation = new CTS140_DOSiteInformation(),
//                    DOCancelContractCondition = new CTS140_DOCancelContractCondition(),
//                };

//                CTS140_SetImportData(importData);
//                return importData;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        /// <summary>
//        /// Bind ContractBasic to screen
//        /// </summary>
//        /// <param name="doRentalContractBasic"></param>
//        public void BindDOContractBasicInformation_CTS140(doRentalContractBasicInformation doRentalContractBasic)
//        {
//            CommonUtil comU;
//            IMasterHandler masterHandler;
//            //List<tbm_Product> ListProduct;
//            CTS140_ScreenParameter session;

//            try
//            {
//                comU = new CommonUtil();
//                session = CTS140_GetImportData();
//                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

//                ViewBag.ContractCode = doRentalContractBasic.ContractCodeShort;
//                ViewBag.ProductTypeCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode;
//                ViewBag.ContractStatus = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractStatus;
//                ViewBag.UserCode = doRentalContractBasic.UserCode;
//                ViewBag.CustomerCode = doRentalContractBasic.ContractTargetCustCodeShort;
//                ViewBag.RealCustomerCustCode = doRentalContractBasic.RealCustomerCustCodeShort;
//                ViewBag.SiteCode = doRentalContractBasic.SiteCodeShort;

//                if (doRentalContractBasic.ContractTargetCustomerImportant == null)
//                    ViewBag.ImportantFlag = false;
//                else
//                    ViewBag.ImportantFlag = (bool)doRentalContractBasic.ContractTargetCustomerImportant;

//                ViewBag.CustFullNameEN = doRentalContractBasic.ContractTargetNameEN;
//                ViewBag.CustFullNameLC = doRentalContractBasic.ContractTargetNameLC;
//                ViewBag.AddressFullEN = doRentalContractBasic.ContractTargetAddressEN;
//                ViewBag.AddressFullLC = doRentalContractBasic.ContractTargetAddressLC;
//                ViewBag.SiteNameEN = doRentalContractBasic.SiteNameEN;
//                ViewBag.SiteNameLC = doRentalContractBasic.SiteNameLC;
//                ViewBag.SiteAddressEN = doRentalContractBasic.SiteAddressEN;
//                ViewBag.SiteAddressLC = doRentalContractBasic.SiteAddressLC;
//                ViewBag.InstallationStatus = CommonUtil.TextCodeName(doRentalContractBasic.InstallationStatusCode, doRentalContractBasic.InstallationStatusName);
//                ViewBag.QuotationTargetCode = comU.ConvertQuotationTargetCode(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT).Trim();
//                ViewBag.EndContractDate = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].CalContractEndDate;
//                ViewBag.RentalContractBasicInformation = doRentalContractBasic;
//                ViewBag.ContractCodeLong = doRentalContractBasic.ContractCode;
//                //ViewBag.ContractCode = doRentalContractBasic.ContractCodeShort;
//                ViewBag.UserCode = doRentalContractBasic.UserCode;

//                if (session.ListRentalSecurityBasic.Count != 0)
//                    ViewBag.SecurityTypeCode = session.ListRentalSecurityBasic[0].SecurityTypeCode;

//                ViewBag.ProductName = GetProductName_CTS140();
//                ViewBag.OperationOffice = CommonUtil.TextCodeName(doRentalContractBasic.OperationOfficeCode.ToString(), doRentalContractBasic.OperationOfficeName.ToString());
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        /// <summary>
//        /// Bind ContractRelate to screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult BindDOContractRelateInformation_CTS140()
//        {
//            CommonUtil comU;
//            ObjectResultData res = new ObjectResultData();
//            CTS140_DOContractRelateInformation doContractRelateInformation;
//            CTS140_ScreenParameter session;

//            try
//            {
//                comU = new CommonUtil();
//                session = CTS140_GetImportData();

//                doContractRelateInformation = new CTS140_DOContractRelateInformation();
//                if (session.DSRentalContract.dtTbt_RentalContractBasic != null)
//                {
//                    if (session.DSRentalContract.dtTbt_RentalContractBasic.Count() != 0)
//                    {
//                        doContractRelateInformation.LastOrderContractFee = session.DSRentalContract.dtTbt_RentalContractBasic[0].LastOrderContractFee;
//                        doContractRelateInformation.StartDealDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].StartDealDate;
//                        doContractRelateInformation.FirstSecurityStartDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate;
//                        doContractRelateInformation.ContractStartDate = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractStartDate;
//                        doContractRelateInformation.ContractEndDate = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].CalContractEndDate;
//                        doContractRelateInformation.LastChangeType = session.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeType;
//                        doContractRelateInformation.LastChangeTypeCodeName = session.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeTypeCodeName;
//                        doContractRelateInformation.ContractDurationMonth = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractDurationMonth;
//                        doContractRelateInformation.LastChangeImplementDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeImplementDate;
//                        doContractRelateInformation.AutoRenewMonth = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AutoRenewMonth;
//                        doContractRelateInformation.LastOCC = session.DSRentalContract.dtTbt_RentalContractBasic[0].LastOCC;
//                        doContractRelateInformation.ProjectCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].ProjectCode;

//                        if (session.DSRentalContract.dtTbt_RentalContractBasic[0].LastOrderContractFee != null)
//                            ViewBag.LastOrderContractFee = CommonUtil.TextNumeric(session.DSRentalContract.dtTbt_RentalContractBasic[0].LastOrderContractFee);

//                        if (session.DSRentalContract.dtTbt_RentalContractBasic[0].StartDealDate != null)
//                            ViewBag.StartDealDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].StartDealDate.Value.ToString("dd/MM/yyyy");

//                        if (session.DSRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate != null)
//                            ViewBag.FirstSecurityStartDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate.Value.ToString("dd/MM/yyyy"); ;

//                        if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractStartDate != null)
//                            ViewBag.ContractStartDate = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractStartDate.Value.ToString("dd/MM/yyyy"); ;

//                        if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].CalContractEndDate != null)
//                            ViewBag.ContractEndDate = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].CalContractEndDate.Value.ToString("dd/MM/yyyy"); ;

//                        ViewBag.LastChangeType = session.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeTypeCodeName;



//                        ViewBag.ContractDurationMonth = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractDurationMonth.ToString();

//                        if (session.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeImplementDate != null)
//                            ViewBag.LastChangeImplementDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeImplementDate.Value.ToString("dd/MM/yyyy"); ;

//                        if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AutoRenewMonth != null)
//                            ViewBag.AutoRenewMonth = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AutoRenewMonth.ToString();

//                        if (session.DSRentalContract.dtTbt_RentalContractBasic[0].LastOCC != null)
//                            ViewBag.LastOCC = session.DSRentalContract.dtTbt_RentalContractBasic[0].LastOCC.ToString();

//                        if (session.DSRentalContract.dtTbt_RentalContractBasic[0].ProjectCode != null)
//                            ViewBag.ProjectCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].ProjectCode.ToString();
//                    }
//                }

//                session.DOContractRelateInformation = doContractRelateInformation;
//                return Json(doContractRelateInformation);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Bind ContractAgreement to screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult BindDOContractAgreementInformation_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();

//            CommonUtil comU;
//            CTS140_ScreenParameter session;
//            IEmployeeMasterHandler employeeMasterHandler;
//            CTS140_DOContractAgreementInformation doContractAgreementInformation;

//            try
//            {
//                comU = new CommonUtil();
//                session = CTS140_GetImportData();
//                employeeMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

//                doContractAgreementInformation = new CTS140_DOContractAgreementInformation();
//                if (session.DSRentalContract.dtTbt_RentalContractBasic != null)
//                {
//                    if (session.DSRentalContract.dtTbt_RentalContractBasic.Count() != 0)
//                    {
//                        doContractAgreementInformation.ApproveContractDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].ApproveContractDate;
//                        doContractAgreementInformation.ContractOfficeCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractOfficeCode;
//                        doContractAgreementInformation.QuotationTargetCode = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode;
//                        doContractAgreementInformation.QuotationTargetCodeShort = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCodeShort;
//                        doContractAgreementInformation.QuotationAlphabet = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet;
//                        doContractAgreementInformation.PlanCode = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PlanCode;
                        
//                        doContractAgreementInformation.SalesmanEmpNo1 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1;
//                        doContractAgreementInformation.SalesmanEmpNo2 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2;
//                        doContractAgreementInformation.SalesSupporterEmpNo = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesSupporterEmpNo;

//                        List<tbm_Employee> empLst = new List<tbm_Employee>();

//                        if (doContractAgreementInformation.SalesmanEmpNo1 != null)
//                        {
//                            empLst.Add(new tbm_Employee()
//                            {
//                                EmpNo = doContractAgreementInformation.SalesmanEmpNo1
//                            });
//                        }
//                        if (doContractAgreementInformation.SalesmanEmpNo2 != null)
//                        {
//                            empLst.Add(new tbm_Employee()
//                            {
//                                EmpNo = doContractAgreementInformation.SalesmanEmpNo2
//                            });
//                        }
//                        if (doContractAgreementInformation.SalesSupporterEmpNo != null)
//                        {
//                            empLst.Add(new tbm_Employee()
//                            {
//                                EmpNo = doContractAgreementInformation.SalesSupporterEmpNo
//                            });
//                        }

//                        List<doActiveEmployeeList> lst = employeeMasterHandler.GetActiveEmployeeList(empLst);
//                        if (lst.Count > 0)
//                        {
//                            foreach (doActiveEmployeeList emp in lst)
//                            {
//                                if (doContractAgreementInformation.SalesmanEmpNo1 == emp.EmpNo)
//                                    doContractAgreementInformation.SalesmanEmpName1 = emp.EmpFullName;
//                                else if (doContractAgreementInformation.SalesmanEmpNo2 == emp.EmpNo)
//                                    doContractAgreementInformation.SalesmanEmpName2 = emp.EmpFullName;
//                                else if (doContractAgreementInformation.SalesSupporterEmpNo == emp.EmpNo)
//                                    doContractAgreementInformation.SalesSupporterEmpName = emp.EmpFullName;
//                            }
//                        }
                        
//                        if (session.DSRentalContract.dtTbt_RelationType != null)
//                        {
//                            if (session.DSRentalContract.dtTbt_RelationType.Count() != 0)
//                            {
//                                foreach (tbt_RelationType r in session.DSRentalContract.dtTbt_RelationType)
//                                {
//                                    if (r.RelationType == RelationType.C_RELATION_TYPE_SALE)
//                                    {
//                                        doContractAgreementInformation.RelatedContractCode = session.DSRentalContract.dtTbt_RelationType[0].RelatedContractCode;
//                                        doContractAgreementInformation.RelatedContractCodeShort = session.DSRentalContract.dtTbt_RelationType[0].RelatedContractCodeShort;
//                                        break;
//                                    }
//                                }
//                            }
//                        }

//                        doContractAgreementInformation.OldContractCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].OldContractCode;
//                        doContractAgreementInformation.OldContractCodeShort = session.DSRentalContract.dtTbt_RentalContractBasic[0].OldContractCodeShort;
//                        doContractAgreementInformation.AcquisitionTypeCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].AcquisitionTypeCode;
//                        doContractAgreementInformation.IntroducerCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].IntroducerCode;
//                        doContractAgreementInformation.MotivationTypeCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].MotivationTypeCode;
                        
//                        ViewBag.ApproveContractDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].ApproveContractDate;
//                        ViewBag.ContractOfficeCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractOfficeCode;
//                        ViewBag.QuotationTargetCode = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCodeShort;
//                        ViewBag.QuotationAlphabet = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet;
//                        ViewBag.PlanCode = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PlanCode;

//                        ViewBag.SalesmanEmpNo1 = doContractAgreementInformation.SalesmanEmpNo1;
//                        ViewBag.SalesmanEmpName1 = doContractAgreementInformation.SalesmanEmpName1;

//                        ViewBag.SalesmanEmpNo2 = doContractAgreementInformation.SalesmanEmpNo2;
//                        ViewBag.SalesmanEmpName2 = doContractAgreementInformation.SalesmanEmpName2;

//                        ViewBag.SalesSupporterEmpNo = doContractAgreementInformation.SalesSupporterEmpNo;
//                        ViewBag.SalesSupporterEmpName = doContractAgreementInformation.SalesSupporterEmpName;

//                        ViewBag.RelatedContractCode = doContractAgreementInformation.RelatedContractCodeShort;
                        
//                        ViewBag.OldContractCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].OldContractCodeShort;
//                        ViewBag.AcquisitionTypeCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].AcquisitionTypeCode;
//                        ViewBag.IntroducerCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].IntroducerCode;
//                        ViewBag.MotivationTypeCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].MotivationTypeCode;
                        
//                    }
//                }

//                ViewBag.C_ACQUISITION_TYPE_CUST = AcquisitionType.C_ACQUISITION_TYPE_CUST;
//                ViewBag.C_ACQUISITION_TYPE_INSIDE_COMPANY = AcquisitionType.C_ACQUISITION_TYPE_INSIDE_COMPANY;
//                ViewBag.C_ACQUISITION_TYPE_SUBCONTRACTOR = AcquisitionType.C_ACQUISITION_TYPE_SUBCONTRACTOR;

//                session.DOContractAgreementInformation = doContractAgreementInformation;
//                return Json(doContractAgreementInformation);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Bind Deposit to screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult BindDODepositInformation_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_DODepositInformation doDepositInformation;
//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                doDepositInformation = new CTS140_DODepositInformation();

//                if (session.DSRentalContract.dtTbt_RentalContractBasic != null)
//                {
//                    if (session.DSRentalContract.dtTbt_RentalContractBasic.Count() != 0)
//                    {
//                        doDepositInformation.NormalDepositFee = session.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFee;
//                        doDepositInformation.OrderDepositFee = session.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFee;
//                        doDepositInformation.ExemptedDepositFee = session.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFee;
//                        doDepositInformation.CounterBalanceOriginContractCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].CounterBalanceOriginContractCode;
//                        doDepositInformation.CounterBalanceOriginContractCodeShort = session.DSRentalContract.dtTbt_RentalContractBasic[0].CounterBalanceOriginContractCodeShort;

//                        ViewBag.NormalDepositFee = session.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFee.Value.ToString("#,##0.00");
//                        ViewBag.OrderDepositFee = session.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFee.Value.ToString("#,##0.00");
//                        ViewBag.ExemptedDepositFee = session.DSRentalContract.dtTbt_RentalContractBasic[0].ExemptedDepositFee.Value.ToString("#,##0.00");
//                        ViewBag.CounterBalanceOriginContractCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].CounterBalanceOriginContractCodeShort;
//                    }
//                }

//                session.DODepositInformation = doDepositInformation;
//                return Json(doDepositInformation);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Bind ContractDocument to screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult BindDOContractDocumentInformation_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_DOContractDocumentInformation doContractDocumentInformation;
//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                doContractDocumentInformation = new CTS140_DOContractDocumentInformation();
//                ViewBag.IrregurationDocUsageFlag = false;

//                if (session.DSRentalContract.dtTbt_RentalContractBasic != null)
//                {
//                    if (session.DSRentalContract.dtTbt_RentalContractBasic.Count() != 0)
//                    {
//                        if (session.DSRentalContract.dtTbt_RentalContractBasic[0].IrregurationDocUsageFlag != null)
//                            ViewBag.IrregurationDocUsageFlag = session.DSRentalContract.dtTbt_RentalContractBasic[0].IrregurationDocUsageFlag;
//                        else
//                            ViewBag.IrregurationDocUsageFlag = false;

//                        doContractDocumentInformation.PODocAuditResult = session.DSRentalContract.dtTbt_RentalContractBasic[0].PODocAuditResult;
//                        doContractDocumentInformation.PODocAuditResultCodeName = session.DSRentalContract.dtTbt_RentalContractBasic[0].PODocAuditResultCodeName;
//                        doContractDocumentInformation.PODocReceiveDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].PODocReceiveDate;

//                        doContractDocumentInformation.ContractDocAuditResult = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractDocAuditResult;
//                        doContractDocumentInformation.ContractDocAuditResultCodeName = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractDocAuditResultCodeName;
//                        doContractDocumentInformation.ContractDocReceiveDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractDocReceiveDate;

//                        doContractDocumentInformation.StartMemoAuditResult = session.DSRentalContract.dtTbt_RentalContractBasic[0].StartMemoAuditResult;
//                        doContractDocumentInformation.StartMemoAuditResultCodeName = session.DSRentalContract.dtTbt_RentalContractBasic[0].StartMemoAuditResultCodeName;
//                        doContractDocumentInformation.StartMemoReceiveDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].StartMemoReceiveDate;

//                        doContractDocumentInformation.IrregurationDocUsageFlag = ViewBag.IrregurationDocUsageFlag;

//                        ViewBag.PODocAuditResult = session.DSRentalContract.dtTbt_RentalContractBasic[0].PODocAuditResultCodeName;
//                        ViewBag.PODocReceiveDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].PODocReceiveDate;
//                        ViewBag.ContractDocAuditResult = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractDocAuditResultCodeName;
//                        ViewBag.ContractDocReceiveDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractDocReceiveDate;
//                        ViewBag.StartMemoAuditResult = session.DSRentalContract.dtTbt_RentalContractBasic[0].StartMemoAuditResultCodeName;
//                        ViewBag.StartMemoReceiveDate = session.DSRentalContract.dtTbt_RentalContractBasic[0].StartMemoReceiveDate;
//                    }
//                }

//                session.DOContractDocumentInformation = doContractDocumentInformation;
//                return Json(doContractDocumentInformation);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Bind ProvideService to screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult BindDOProvideServiceInformation_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_DOProvideServiceInformation doProvideServiceInformation;
//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                doProvideServiceInformation = new CTS140_DOProvideServiceInformation();
//                ViewBag.FireMonitorFlag = false;
//                ViewBag.CrimePreventFlag = false;
//                ViewBag.EmergencyReportFlag = false;
//                ViewBag.FacilityMonitorFlag = false;

//                if (session.DSRentalContract.dtTbt_RentalSecurityBasic != null)
//                {
//                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic.Count != 0)
//                    {
//                        doProvideServiceInformation.FireMonitorFlag = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].FireMonitorFlag;
//                        doProvideServiceInformation.CrimePreventFlag = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].CrimePreventFlag;
//                        doProvideServiceInformation.EmergencyReportFlag = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag;
//                        doProvideServiceInformation.FacilityMonitorFlag = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag;
//                        doProvideServiceInformation.PhoneLineTypeCode1 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1;
//                        doProvideServiceInformation.PhoneLineOwnerCode1 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1;
//                        doProvideServiceInformation.PhoneNo1 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo1; 
//                        doProvideServiceInformation.PhoneLineTypeCode2 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode2;
//                        doProvideServiceInformation.PhoneLineOwnerCode2 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode2;
//                        doProvideServiceInformation.PhoneNo2 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo2; 
//                        doProvideServiceInformation.PhoneLineTypeCode3 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode3;
//                        doProvideServiceInformation.PhoneLineOwnerCode3 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode3;
//                        doProvideServiceInformation.PhoneNo3 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo3; 

//                        ViewBag.FireMonitorFlag = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].FireMonitorFlag;
//                        ViewBag.CrimePreventFlag = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].CrimePreventFlag;
//                        ViewBag.EmergencyReportFlag = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag;
//                        ViewBag.FacilityMonitorFlag = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag;
//                        ViewBag.PhoneLineTypeCode1 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1;
//                        ViewBag.PhoneLineOwnerCode1 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1;
//                        ViewBag.PhoneLineTypeCode2 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode2;
//                        ViewBag.PhoneLineOwnerCode2 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode2;
//                        ViewBag.PhoneLineTypeCode3 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode3;
//                        ViewBag.PhoneLineOwnerCode3 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode3;

//                        ViewBag.PhoneNo1 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo1;
//                        ViewBag.PhoneNo2 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo2;
//                        ViewBag.PhoneNo3 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo3;
//                    }
//                }

//                session.DOProvideServiceInformation = doProvideServiceInformation;
//                return Json(doProvideServiceInformation);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Bind Maintenance to screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult BindDOMaintenanceInformation_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_DOMaintenanceInformation doMaintenanceInformation;
//            List<tbm_Product> listProduct;
            
//            List<CTS140_DOProductName> listDOProductName = null;

//            IMasterHandler masterHandler;
//            IRentralContractHandler rentralContractHandler;
//            CommonUtil comU;

//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                session.ListDOMaintenanceGrid = new List<CTS140_DOMaintenanceGrid>();
//                session.ListDOMaintenanceGridEdit = new List<CTS140_DOMaintenanceGrid>();
//                comU = new CommonUtil();

//                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
//                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

//                doMaintenanceInformation = new CTS140_DOMaintenanceInformation();
//                if (session.DSRentalContract.dtTbt_RentalMaintenanceDetails != null)
//                {
//                    if (session.DSRentalContract.dtTbt_RentalMaintenanceDetails.Count != 0)
//                    {
//                        doMaintenanceInformation.MaintenanceTypeCode = session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceTypeCode;
//                        doMaintenanceInformation.MaintenanceCycle = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceCycle;
//                        doMaintenanceInformation.MaintenanceContractStartMonth = session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartMonth;
//                        doMaintenanceInformation.MaintenanceContractStartYear = session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartYear;
//                        doMaintenanceInformation.MaintenanceFeeTypeCode = session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode;

//                        ViewBag.MaintenanceTypeCode = session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceTypeCode;
//                        ViewBag.MaintenanceCycle = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceCycle;
//                        ViewBag.MaintenanceContractStartMonth = session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartMonth;
//                        ViewBag.MaintenanceContractStartYear = session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartYear;
//                        ViewBag.MaintenanceFeeTypeCode = session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode;
//                    }
//                }

//                session.DOMaintenanceInformation = doMaintenanceInformation;
//                if (session.DSRentalContract.dtTbt_RelationType != null)
//                {
//                    CTS140_DOMaintenanceGrid doMaintenanceGrid;


//                    List<tbt_SaleBasic> lst = new List<tbt_SaleBasic>();
//                    foreach (tbt_RelationType rm in session.DSRentalContract.dtTbt_RelationType)
//                    {
//                        if (CommonUtil.IsNullOrEmpty(rm.RelatedContractCode) == false)
//                        {
//                            lst.Add(new tbt_SaleBasic()
//                            {
//                                ContractCode = rm.RelatedContractCode
//                            });
//                        }
//                    }

//                    IContractHandler cthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
//                    List<doContractHeader> ctLst = cthandler.GetContractHeaderDataByLanguage(lst);
//                    if (ctLst.Count > 0)
//                    {
//                        foreach (tbt_RelationType rm in session.DSRentalContract.dtTbt_RelationType)
//                        {
//                            foreach (doContractHeader ct in ctLst)
//                            {
//                                if (ct.ContractCode == rm.RelatedContractCode)
//                                {
//                                    doMaintenanceGrid = new CTS140_DOMaintenanceGrid();
//                                    doMaintenanceGrid.ContractCode = comU.ConvertContractCode(ct.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
//                                    doMaintenanceGrid.ProductCode = ct.ProductTypeCode;
//                                    doMaintenanceGrid.ProductTypeCode = ct.ProductTypeCode;
//                                    doMaintenanceGrid.ProductName = ct.ProductName;

//                                    session.ListDOMaintenanceGrid.Add(doMaintenanceGrid);
//                                    session.ListDOMaintenanceGridEdit.Add(doMaintenanceGrid);

//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }

//                return Json(doMaintenanceInformation);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Bind Site to screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult BindDOSiteInformation_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_DOSiteInformation doSiteInformation;
//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                doSiteInformation = new CTS140_DOSiteInformation();

//                if (session.DSRentalContract.dtTbt_RentalSecurityBasic != null)
//                {
//                    doSiteInformation.BuildingTypeCode = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].BuildingTypeCode;
//                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SiteBuildingArea != null)
//                        doSiteInformation.SiteBuildingArea = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SiteBuildingArea.Value.ToString();
//                    else
//                        doSiteInformation.SiteBuildingArea = "";

//                    doSiteInformation.NumOfBuilding = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfBuilding;
//                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom != null)
//                        doSiteInformation.SecurityAreaFrom = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom.ToString();
//                    else
//                        doSiteInformation.SecurityAreaFrom = "";

//                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaTo != null)
//                        doSiteInformation.SecurityAreaTo = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaTo.ToString();
//                    else
//                        doSiteInformation.SecurityAreaTo = "";

//                    doSiteInformation.NumOfFloor = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfFloor;
//                    doSiteInformation.MainStructureTypeCode = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MainStructureTypeCode;
//                    session.DOSiteInformation = doSiteInformation;

//                    ViewBag.BuildingTypeCode = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].BuildingTypeCode;

//                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SiteBuildingArea != null)
//                        //ViewBag.SiteBuildingArea = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SiteBuildingArea.ToString();
//                        ViewBag.SiteBuildingArea = CommonUtil.TextNumeric(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SiteBuildingArea);

//                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfBuilding != null)
//                        ViewBag.NumOfBuilding = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfBuilding.ToString();

//                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom != null)
//                        //ViewBag.SecurityAreaFrom = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom.ToString();
//                        ViewBag.SecurityAreaFrom = CommonUtil.TextNumeric(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom);

//                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaTo != null)
//                        //ViewBag.SecurityAreaTo = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaTo.ToString();
//                        ViewBag.SecurityAreaTo = CommonUtil.TextNumeric(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaTo);

//                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfFloor != null)
//                        ViewBag.NumOfFloor = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfFloor.ToString();

//                    ViewBag.MainStructureTypeCode = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MainStructureTypeCode;
//                    session.DOSiteInformation = doSiteInformation;
//                }

//                return Json(doSiteInformation);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Bind CancelContract to screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult BindCancelContractCondition_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_DOCancelContractCondition doCancelContractCondition;
//            List<CTS140_DOCancelContractGrid> doCancelContractGridList;
//            CTS140_DOCancelContractGrid doCancelContractGrid;
//            CTS140_ScreenParameter session;

//            try
//            {
//                //Set default
//                ViewBag.Refund = false;
//                ViewBag.ReceiveRevenue = false;
//                ViewBag.Bill = false;
//                ViewBag.Exempt = false;

//                session = CTS140_GetImportData();
//                doCancelContractCondition = new CTS140_DOCancelContractCondition();
//                doCancelContractGridList = new List<CTS140_DOCancelContractGrid>();

//                if (session.DSRentalContract.dtTbt_RentalContractBasic != null)
//                {
//                    ViewBag.ContractCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractCodeShort;

//                    doCancelContractCondition.CancelContractDate = CommonUtil.TextDate(session.DSRentalContract.dtTbt_RentalContractBasic[0].LastChangeImplementDate);

//                    IRentralContractHandler rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
//                    DateTime? removalInstallationCompleteDate =
//                        rentralContractHandler.GetRemovalInstallCompleteDate(session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractCode);
//                    doCancelContractCondition.RemovalInstallationCompleteDate = CommonUtil.TextDate(removalInstallationCompleteDate);
//                }

//                //if (session.DSRentalContract.dtTbt_CancelContractMemoDetail != null)
//                //{
//                //    if (session.DSRentalContract.dtTbt_CancelContractMemoDetail.Count() != 0)
//                //    {
//                if (session.DSRentalContract.dtTbt_CancelContractMemo != null && session.DSRentalContract.dtTbt_CancelContractMemo.Count() != 0)
//                {
//                    doCancelContractCondition.ProcessAfterCounterBalanceType = session.DSRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType;
//                    if (session.DSRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND)
//                        ViewBag.Refund = true;
//                    else
//                        ViewBag.Refund = false;

//                    if (session.DSRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE)
//                        ViewBag.ReceiveRevenue = true;
//                    else
//                        ViewBag.ReceiveRevenue = false;

//                    if (session.DSRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL)
//                        ViewBag.Bill = true;
//                    else
//                        ViewBag.Bill = false;

//                    if (session.DSRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType == ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT)
//                        ViewBag.Exempt = true;
//                    else
//                        ViewBag.Exempt = false;

//                    doCancelContractCondition.OtherRemarks = session.DSRentalContract.dtTbt_CancelContractMemo[0].OtherRemarks;
//                }
//                else
//                {
//                    ViewBag.Refund = false;
//                    ViewBag.ReceiveRevenue = false;
//                    ViewBag.Bill = false;
//                    ViewBag.Exempt = false;
//                }
//                //    }
//                //    else
//                //    {
//                //        ViewBag.Refund = false;
//                //        ViewBag.ReceiveRevenue = false;
//                //        ViewBag.Bill = false;
//                //        ViewBag.Exempt = false;
//                //    }
//                //}

//                doCancelContractCondition.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL;
//                doCancelContractCondition.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT;
//                doCancelContractCondition.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE;
//                doCancelContractCondition.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND = ProcAfterCounterBalanceType.C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND;

//                doCancelContractCondition.C_BILLING_TYPE_CONTRACT_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE;
//                doCancelContractCondition.C_BILLING_TYPE_MAINTENANCE_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE;
//                doCancelContractCondition.C_BILLING_TYPE_DEPOSIT_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE;
//                doCancelContractCondition.C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;
//                doCancelContractCondition.C_BILLING_TYPE_CANCEL_CONTRACT_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE;
//                doCancelContractCondition.C_BILLING_TYPE_CHANGE_INSTALLATION_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE;
//                doCancelContractCondition.C_BILLING_TYPE_CARD_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE;
//                doCancelContractCondition.C_BILLING_TYPE_OTHER_FEE = ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE;

//                doCancelContractCondition.C_HANDLING_TYPE_BILL_UNPAID_FEE = HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE;
//                doCancelContractCondition.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE = HandlingType.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE;
//                doCancelContractCondition.C_HANDLING_TYPE_RECEIVE_AS_REVENUE = HandlingType.C_HANDLING_TYPE_RECEIVE_AS_REVENUE;
//                doCancelContractCondition.C_HANDLING_TYPE_REFUND = HandlingType.C_HANDLING_TYPE_REFUND;
//                doCancelContractCondition.C_HANDLING_TYPE_SLIDE = HandlingType.C_HANDLING_TYPE_SLIDE;

//                session.DOCancelContractCondition = doCancelContractCondition;
//                if (session.DSRentalContract.dtTbt_CancelContractMemoDetail != null)
//                {
//                    if (session.ListDOCancelContractGrid == null)
//                        session.ListDOCancelContractGrid = new List<CTS140_DOCancelContractGrid>();

//                    int intSequence = -1;
//                    foreach (var item in session.DSRentalContract.dtTbt_CancelContractMemoDetail)
//                    {
//                        doCancelContractGrid = new CTS140_DOCancelContractGrid();
//                        doCancelContractGrid = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, CTS140_DOCancelContractGrid>(item) as CTS140_DOCancelContractGrid;
//                        doCancelContractGrid.BillingTypeName = item.BillingTypeName;
//                        doCancelContractGrid.BillingType = item.BillingType;
//                        doCancelContractGrid.HandlingTypeName = item.HandlingTypeName;
//                        doCancelContractGrid.HandlingType = item.HandlingType;
//                        doCancelContractGrid.NormalFeeAmount = item.NormalFeeAmount;
//                        doCancelContractGrid.Sequence = (intSequence + 1).ToString();
//                        doCancelContractGridList.Add(doCancelContractGrid);

//                        intSequence++;
//                    }

//                    session.ListDOCancelContractGrid = doCancelContractGridList;
//                }

//                return Json(doCancelContractCondition);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Bind Other to screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult BindOtherInformation_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_ScreenParameter session;
//            CTS140_DOOtherInformation doOtherInformation;

//            try
//            {
//                session = CTS140_GetImportData();
//                doOtherInformation = new CTS140_DOOtherInformation();

//                if (session.DSRentalContract.dtTbt_RentalContractBasic.Count() != 0)
//                    doOtherInformation.Memo = session.DSRentalContract.dtTbt_RentalContractBasic[0].Memo;

//                return Json(doOtherInformation);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        //---------------------------------------------------------

//        /// <summary>
//        /// Get data of Maintenance to grid
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult GetMaintenanceGrid_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                res.ResultData = CommonUtil.ConvertToXml<CTS140_DOMaintenanceGrid>(session.ListDOMaintenanceGridEdit, "Contract\\CTS140Maintenance", CommonUtil.GRID_EMPTY_TYPE.VIEW);
//                return Json(res);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Get data of CancelContract to grid
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult GetCancelGrid_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_ScreenParameter session;
//            //List<CTS140_DOCancelContractGrid> listDOCancelContractGridTemp;
//            CommonUtil comUtil = new CommonUtil();

//            try
//            {
//                string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblNormalFee");
//                string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblContractCodeForSlideFee");

//                session = CTS140_GetImportData();
//                //listDOCancelContractGridTemp = CommonUtil.ClonsObjectList<tbt_CancelContractMemoDetail, CTS140_DOCancelContractGrid>(session.DSRentalContract.dtTbt_CancelContractMemoDetail);
//                //for (int i = 0; i <= listDOCancelContractGridTemp.Count() - 1; i++)
//                //{
//                //    listDOCancelContractGridTemp[i].FeeAmountString = session.ListDOCancelContractGrid[i].FeeAmountString;
//                //    listDOCancelContractGridTemp[i].TaxAmountString = session.ListDOCancelContractGrid[i].TaxAmountString;
//                //    listDOCancelContractGridTemp[i].PeriodString = session.ListDOCancelContractGrid[i].PeriodString;
//                //    listDOCancelContractGridTemp[i].StatusGrid = session.ListDOCancelContractGrid[i].StatusGrid;
//                //}
//                //session.ListDOCancelContractGrid = listDOCancelContractGridTemp;

//                if (session.ListDOCancelContractGrid != null)
//                {
//                    foreach (var item in session.ListDOCancelContractGrid)
//                    {
//                        //if (item.FeeAmount != null)
//                        //    item.FeeAmountString = item.FeeAmount.Value.ToString("#,##0.00");

//                        //if (item.TaxAmount != null)
//                        //    item.TaxAmountString = item.TaxAmount.Value.ToString("#,##0.00");

//                        //if (item.StartPeriodDate != null)
//                        //    item.PeriodString = item.StartPeriodDate.Value.ToString("dd-MMMM-yyyy");

//                        //if (item.EndPeriodDate != null)
//                        //    item.PeriodString = item.EndPeriodDate.Value.ToString("dd-MMMM-yyyy");

//                        //if (item.StartPeriodDate != null && item.EndPeriodDate != null)
//                        //    item.PeriodString = item.StartPeriodDate.Value.ToString("dd-MMMM-yyyy") + " To " + item.EndPeriodDate.Value.ToString("dd-MMMM-yyyy");

//                        item.FeeAmountString = CommonUtil.TextNumeric(item.FeeAmount);
//                        item.TaxAmountString = CommonUtil.TextNumeric(item.TaxAmount);

//                        if (item.StartPeriodDate != null && item.EndPeriodDate != null)
//                        {
//                            item.PeriodString = CommonUtil.TextDate(item.StartPeriodDate) + " To " + CommonUtil.TextDate(item.EndPeriodDate);
//                        }
//                        else
//                        {
//                            if (item.StartPeriodDate != null)
//                                item.PeriodString = CommonUtil.TextDate(item.StartPeriodDate);

//                            if (item.EndPeriodDate != null)
//                                item.PeriodString = CommonUtil.TextDate(item.EndPeriodDate);
//                        }

//                        if (item.Remark != null)
//                        {
//                            string strContractCodeCounterBal = comUtil.ConvertContractCode(item.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_SHORT);

//                            item.RemarkString = String.Format("{0} {1} {2}", item.Remark
//                                , item.ContractCode_CounterBalance == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, strContractCodeCounterBal)
//                                , item.NormalFeeAmount == null ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, CommonUtil.TextNumeric(item.NormalFeeAmount)));
//                        }
//                    }

//                }

//                res.ResultData = CommonUtil.ConvertToXml<CTS140_DOCancelContractGrid>(session.ListDOCancelContractGrid, "Contract\\CTS140CancelContract", CommonUtil.GRID_EMPTY_TYPE.VIEW);
//                return Json(res);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        //public ActionResult CTS140_RefreshCancelGrid()
//        //{
//        //    ObjectResultData res = new ObjectResultData();
//        //    CTS140_ScreenParameter session;
//        //    CommonUtil comUtil = new CommonUtil();

//        //    try
//        //    {
//        //        string lblNormalFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblNormalFee");
//        //        string lblContractCodeForSlideFee = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, ScreenID.C_SCREEN_ID_CANCEL_RENTAL_CONTRACT, "lblContractCodeForSlideFee");

//        //        session = CTS140_GetImportData();
//        //        foreach (var item in session.ListDOCancelContractGrid)
//        //        {
//        //            item.FeeAmountString = CommonUtil.TextNumeric(item.FeeAmount);
//        //            item.TaxAmountString = CommonUtil.TextNumeric(item.TaxAmount);

//        //            if (item.StartPeriodDate != null && item.EndPeriodDate != null)
//        //            {
//        //                item.PeriodString = CommonUtil.TextDate(item.StartPeriodDate) + " To " + CommonUtil.TextDate(item.EndPeriodDate);
//        //            }
//        //            else
//        //            {
//        //                if (item.StartPeriodDate != null)
//        //                    item.PeriodString = CommonUtil.TextDate(item.StartPeriodDate);

//        //                if (item.EndPeriodDate != null)
//        //                    item.PeriodString = CommonUtil.TextDate(item.EndPeriodDate);
//        //            }

//        //            if (item.Remark != null)
//        //            {
//        //                string strContractCodeCounterBal = comUtil.ConvertContractCode(item.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_SHORT);

//        //                item.Remark = String.Format("{0} {1} {2}", item.Remark
//        //                    , item.ContractCode_CounterBalance == null ? string.Empty : string.Format("<br/>{0}: {1}", lblContractCodeForSlideFee, strContractCodeCounterBal)
//        //                    , String.IsNullOrEmpty(item.NormalFeeAmountString) ? string.Empty : string.Format("<br/>{0}: {1}", lblNormalFee, item.NormalFeeAmountString));
//        //            }
//        //        }

//        //        res.ResultData = CommonUtil.ConvertToXml<CTS140_DOCancelContractGrid>(session.ListDOCancelContractGrid, "Contract\\CTS140CancelContract", CommonUtil.GRID_EMPTY_TYPE.VIEW);
//        //        return Json(res);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        res.AddErrorMessage(ex);
//        //    }

//        //    return Json(res);
//        //}

//        /// <summary>
//        /// Get total amount of Fee
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult GetCancelGridTotal_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_DOCancelContractTotal doCancelContractTotal;
//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                doCancelContractTotal = new CTS140_DOCancelContractTotal();

//                //if (session.DSRentalContract.dtTbt_CancelContractMemoDetail != null)
//                if (session.ListDOCancelContractGrid != null)
//                {
//                    //doCancelContractTotal.TotalSlideAmt = session.DSRentalContract.dtTbt_CancelContractMemoDetail.FindAll(delegate(tbt_CancelContractMemoDetail s) { return s.HandlingType == HandlingType.C_HANDLING_TYPE_SLIDE; }).Sum(s => s.FeeAmount + s.TaxAmount).Value.ToString("#,##0.00");
//                    //doCancelContractTotal.TotalRefundAmt = session.DSRentalContract.dtTbt_CancelContractMemoDetail.FindAll(delegate(tbt_CancelContractMemoDetail s) { return s.HandlingType == HandlingType.C_HANDLING_TYPE_REFUND; }).Sum(s => s.FeeAmount + s.TaxAmount).Value.ToString("#,##0.00");
//                    //doCancelContractTotal.TotalBillingAmt = session.DSRentalContract.dtTbt_CancelContractMemoDetail.FindAll(delegate(tbt_CancelContractMemoDetail s) { return s.HandlingType == HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE; }).Sum(s => s.FeeAmount + s.TaxAmount).Value.ToString("#,##0.00");
//                    //doCancelContractTotal.TotalAmtAfterCounterBalanceType = (decimal.Parse(doCancelContractTotal.TotalRefundAmt) - decimal.Parse(doCancelContractTotal.TotalBillingAmt)).ToString("#,##0.00");

//                    doCancelContractTotal.TotalSlideAmt = CommonUtil.TextNumeric(session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.HandlingType == HandlingType.C_HANDLING_TYPE_SLIDE; }).Sum(s => s.FeeAmount + s.TaxAmount));
//                    doCancelContractTotal.TotalRefundAmt = CommonUtil.TextNumeric(session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.HandlingType == HandlingType.C_HANDLING_TYPE_REFUND; }).Sum(s => s.FeeAmount + s.TaxAmount));
//                    doCancelContractTotal.TotalBillingAmt = CommonUtil.TextNumeric(session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.HandlingType == HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE; }).Sum(s => s.FeeAmount + s.TaxAmount));
//                    doCancelContractTotal.TotalAmtAfterCounterBalanceType = CommonUtil.TextNumeric((decimal.Parse(doCancelContractTotal.TotalRefundAmt) - decimal.Parse(doCancelContractTotal.TotalBillingAmt)));

//                    if (doCancelContractTotal.TotalSlideAmt == "")
//                        doCancelContractTotal.TotalSlideAmt = "0";

//                    if (doCancelContractTotal.TotalRefundAmt == "")
//                        doCancelContractTotal.TotalRefundAmt = "0";

//                    if (doCancelContractTotal.TotalBillingAmt == "")
//                        doCancelContractTotal.TotalBillingAmt = "0";

//                    if (doCancelContractTotal.TotalSlideAmt == "")
//                        doCancelContractTotal.TotalAmtAfterCounterBalanceType = "0";

//                    if (decimal.Parse(doCancelContractTotal.TotalRefundAmt) > decimal.Parse(doCancelContractTotal.TotalBillingAmt))
//                        doCancelContractTotal.IsShowReturnReceive = true;
//                    else
//                        doCancelContractTotal.IsShowReturnReceive = false;

//                }

//                return Json(doCancelContractTotal);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Get Name of Product
//        /// </summary>
//        /// <returns></returns>
//        public string GetProductName_CTS140()
//        {
//            string productName = "";
//            ObjectResultData res = new ObjectResultData();
//            IMasterHandler masterHandler;
//            List<tbm_Product> listProduct;
//            List<CTS140_DOProductName> listDOProductName;
//            CTS140_ScreenParameter session;

//            try
//            {
//                //2.6 Show product name 
//                session = CTS140_GetImportData();
//                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
//                listProduct = masterHandler.GetTbm_Product(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ProductCode, session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ProductTypeCode);
//                listDOProductName = CommonUtil.ClonsObjectList<tbm_Product, CTS140_DOProductName>(listProduct);
//                CommonUtil.MappingObjectLanguage(listDOProductName);

//                if (listDOProductName.Count() != 0)
//                    productName = CommonUtil.TextCodeName( 
//                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ProductCode,
//                        listDOProductName[0].ProductName.ToString());

//                return productName;
//            }
//            catch (Exception ex)
//            {
//            }

//            return productName;
//        }

//        /// <summary>
//        /// Get ScreenCode when click [Detail] button on Maintenance grid
//        /// </summary>
//        /// <param name="doGetScreeenCode"></param>
//        /// <returns></returns>
//        public ActionResult GetScreenCode(CTS140_DOGetScreeenCode doGetScreeenCode)
//        {
//            ObjectResultData res = new ObjectResultData();
//            IRentralContractHandler rentralContractHandler;
//            List<tbt_RentalContractBasic> listRentalContractBasic;

//            try
//            {
//                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
//                listRentalContractBasic = rentralContractHandler.GetTbt_RentalContractBasic(doGetScreeenCode.ContractCodeLong, null);

//                if (listRentalContractBasic.Count() > 0)
//                    res.ResultData = "CMS140";
//                else
//                    res.ResultData = "CMS160";

//                return Json(res);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Get Employee name to TextBox
//        /// </summary>
//        /// <param name="doEmployee"></param>
//        /// <returns></returns>
//        public ActionResult GetActiveEmployee_CTS140(CTS140_DOEmployee doEmployee)
//        {
//            ObjectResultData res = new ObjectResultData();
//            IMasterHandler masterHandler;
//            IEmployeeMasterHandler employeeHandler;
//            List<tbm_Employee> listEmployee;
//            List<dtEmpNo> listEmpNo;

//            try
//            {
//                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
//                employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

//                listEmployee = masterHandler.GetActiveEmployee(doEmployee.EmpNo);

//                if (listEmployee.Count() == 0)
//                {
//                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, "");
//                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { doEmployee.EmpNo }, null);
//                    return Json(res);
//                }

//                listEmpNo = employeeHandler.GetEmployeeNameByEmpNo(doEmployee.EmpNo);
//                if (listEmpNo.Count() != 0)
//                    doEmployee.EmpName = listEmpNo[0].EmployeeNameDisplay;

//                return Json(doEmployee);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//                return Json(res);
//            }
//        }

//        /// <summary>
//        /// Validate entering section when initail screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult ValidateEnteringCondition_CTS140()
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_ScreenParameter session;
//            CommonUtil comUtil = new CommonUtil();

//            try
//            {
//                session = CTS140_GetImportData();

//                //1.9 Validate entering conditions
//                if (session.DSRentalContract.dtTbt_RentalContractBasic != null)
//                {
//                    if (session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
//                    {
//                        string strContractCode = comUtil.ConvertContractCode(session.contractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

//                        res.ResultData = false;
//                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3119, null);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3119, new string[] { strContractCode }, null);
//                        return Json(res);
//                    }
//                }

//                return Json(res);
//            }
//            catch (Exception ex)
//            {
//                res.ResultData = false;
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Validate business of screen
//        /// </summary>
//        /// <param name="doValidateBusiness"></param>
//        /// <returns></returns>
//        public ObjectResultData ValidateBusiness_CTS140(CTS140_DOValidateBusiness doValidateBusiness)
//        {
//            ObjectResultData res = new ObjectResultData();
//            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

//            IEmployeeMasterHandler employeeMasterHandler;
//            IProjectHandler projectHandler;
//            IRentralContractHandler rentralContractHandler;
//            ICustomerMasterHandler customerMasterHandler;
//            ICommonContractHandler subContractorMasterHandler;
//            ICommonContractHandler comContractHandler;

//            List<doCustomer> ListDOCustomer;
//            List<dtEmployee> ListDTEmployee;
//            List<tbm_SubContractor> ListDOSubcontractorDetails;
//            CommonUtil comUtil = new CommonUtil();

//            try
//            {
//                employeeMasterHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
//                projectHandler = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
//                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
//                customerMasterHandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
//                subContractorMasterHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
//                comContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;

//                CTS140_ScreenParameter session = CTS140_GetImportData();

//                //3. VaidateBusiness                
//                if (doValidateBusiness.IsShowContractRelatedInformation == true)
//                {
//                    //3.1 If dtpStartDealDate > TODAY Then
//                    if (doValidateBusiness.StartDealDate.Value.Date > DateTime.Now.Date)
//                    {
//                        //string[] param = { "Start deal date" };
//                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3126, "Start deal date");

//                        //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3126, param, null);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
//                                            ScreenID.C_SCREEN_ID_CP33,
//                                            MessageUtil.MODULE_CONTRACT,
//                                            MessageUtil.MessageList.MSG3126,
//                                            new string[] { "lblStartDealDate" },
//                                            new string[] { "StartDealDate" });
//                        return res;
//                    }

//                    //3.2 If dtpContractDurationStartDate > TODAY Then
//                    if (doValidateBusiness.ContractStartDate.Value.Date > DateTime.Now.Date)
//                    {
//                        //string[] param = { "Contract duration start date" };
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3126, "Contract duration start date");

//                        //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3126, param, null);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
//                                            ScreenID.C_SCREEN_ID_CP33,
//                                            MessageUtil.MODULE_CONTRACT,
//                                            MessageUtil.MessageList.MSG3126,
//                                            new string[] { "lblContractDuration" },
//                                            new string[] { "ContractStartDate" });
//                        return res;
//                    }

//                    //3.3 If dtpContractDurationEndDate <= dtpContractDurationStartDate Then
//                    if (doValidateBusiness.ContractEndDate.Value.Date <= doValidateBusiness.ContractStartDate.Value.Date)
//                    {
//                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3127, null);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3127, null, new string[] { "ContractEndDate" });
//                        return res;
//                    }

//                    //3.7 If txtProjectCode is not existing in tbt_Project.ProjectCode Then
//                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.ProjectCode) == false)
//                    {
//                        if (projectHandler.GetTbt_Project(doValidateBusiness.ProjectCode).Count() == 0)
//                        {
//                            string[] param = { doValidateBusiness.ProjectCode };
//                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0091, doValidateBusiness.ProjectCode);
//                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0091, param, new string[] { "ProjectCode" });
//                            return res;
//                        }
//                    }
//                }

//                if (doValidateBusiness.IsShowContractAgreementInformation == true)
//                {
//                    //3.4 If txtSalesman1Code is not existing in the tbm_Employee.EmpNo Then
//                    if (doValidateBusiness.SalesmanEmpNo1 != "none")
//                    {
//                        if (doValidateBusiness.SalesmanEmpNo1 != null)
//                        {
//                            if (employeeMasterHandler.GetActiveEmployee(doValidateBusiness.SalesmanEmpNo1).Count() == 0)
//                            {
//                                string[] param = { doValidateBusiness.SalesmanEmpNo1 };
//                                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.SalesmanEmpNo1);
//                                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, new string[] { "SalesmanEmpNo1" });
//                                return res;
//                            }
//                        }
//                    }

//                    //3.5 If txtSalesman2Code is not existing in the tbm_Employee.EmpNo Then

//                    if (doValidateBusiness.SalesmanEmpNo2 != null)
//                    {
//                        if (employeeMasterHandler.GetActiveEmployee(doValidateBusiness.SalesmanEmpNo2).Count() == 0)
//                        {
//                            string[] param = { doValidateBusiness.SalesmanEmpNo2 };
//                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.SalesmanEmpNo2);
//                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, new string[] { "SalesmanEmpNo2" });
//                            return res;
//                        }
//                    }

//                    //3.6 If txtSalesSupporterCode is not existing in the tbm_Employee.EmpNo Then

//                    if (doValidateBusiness.SalesSupporterEmpNo != null)
//                    {
//                        if (employeeMasterHandler.GetActiveEmployee(doValidateBusiness.SalesSupporterEmpNo).Count() == 0)
//                        {
//                            string[] param = { doValidateBusiness.SalesSupporterEmpNo };
//                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.SalesSupporterEmpNo);
//                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, new string[] { "SalesSupporterEmpNo" });
//                            return res;
//                        }
//                    }

//                    //3.8 If txtOldContractCode is not existing in tbt_RentalContractBasic.ContractCode Then
//                    string strOldContractCodeLong = comUtil.ConvertContractCode(doValidateBusiness.OldContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
//                    if (rentralContractHandler.GetTbt_RentalContractBasic(strOldContractCodeLong, null).Count() == 0)
//                    {
//                        string[] param = { doValidateBusiness.OldContractCode };
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, doValidateBusiness.OldContractCode);
//                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, param, new string[] { "OldContractCode" });
//                        return res;
//                    }

//                    //3.10 Validate introducer code
//                    //3.10.1 If ddlAcquisitionType = C_ACQUISITION_TYPE_CUST Then
//                    //if (doValidateBusiness.AcquisitionTypeCode == AcquisitionType.C_ACQUISITION_TYPE_CUST)
//                    //{
//                    //    CommonUtil cmm = new CommonUtil();
//                    //    ListDOCustomer = customerMasterHandler.GetCustomer(cmm.ConvertCustCode(doValidateBusiness.IntroducerCode, CommonUtil.CONVERT_TYPE.TO_LONG));
//                    //    if (ListDOCustomer != null)
//                    //    {
//                    //        if (ListDOCustomer.Count() == 0)
//                    //        {
//                    //            string[] param = { doValidateBusiness.CounterBalanceOriginContractCode };
//                    //            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
//                    //            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3128, doValidateBusiness.CounterBalanceOriginContractCode);

//                    //            //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3128, param);
//                    //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
//                    //                        ScreenID.C_SCREEN_ID_CP33,
//                    //                        MessageUtil.MODULE_CONTRACT,
//                    //                        MessageUtil.MessageList.MSG3128,
//                    //                        new string[] { "lblCustomerCodeMsg" },
//                    //                        new string[] { "IntroducerCode" });
//                    //            return res;
//                    //        }
//                    //    }
//                    //}

//                    ////3.10.2 If ddlAcquisitionType = C_ACQUISITION_TYPE_INSIDE_COMPANY Then
//                    //if (doValidateBusiness.AcquisitionTypeCode == AcquisitionType.C_ACQUISITION_TYPE_INSIDE_COMPANY)
//                    //{
//                    //    ListDTEmployee = employeeMasterHandler.GetEmployee(doValidateBusiness.IntroducerCode, null, null);
//                    //    if (ListDTEmployee != null)
//                    //    {
//                    //        if (ListDTEmployee.Count() == 0)
//                    //        {
//                    //            string[] param = { doValidateBusiness.IntroducerCode };
//                    //            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
//                    //            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3128, doValidateBusiness.IntroducerCode);

//                    //            //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3128, param);
//                    //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
//                    //                        ScreenID.C_SCREEN_ID_CP33,
//                    //                        MessageUtil.MODULE_CONTRACT,
//                    //                        MessageUtil.MessageList.MSG3128,
//                    //                        new string[] { "lblEmployeeCodeMsg" },
//                    //                        new string[] { "IntroducerCode" });
//                    //            return res;
//                    //        }
//                    //    }
//                    //}

//                    ////3.10.3 If ddlAcquisitionType = C_ACQUISITION_TYPE_SUBCONTRACTOR Then
//                    //if (doValidateBusiness.AcquisitionTypeCode == AcquisitionType.C_ACQUISITION_TYPE_SUBCONTRACTOR)
//                    //{
//                    //    ListDOSubcontractorDetails = subContractorMasterHandler.GetTbm_SubContractorData(doValidateBusiness.IntroducerCode);
//                    //    if (ListDOSubcontractorDetails != null)
//                    //    {
//                    //        if (ListDOSubcontractorDetails.Count() == 0)
//                    //        {
//                    //            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3128, doValidateBusiness.IntroducerCode);
//                    //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
//                    //            ScreenID.C_SCREEN_ID_CP33,
//                    //            MessageUtil.MODULE_CONTRACT,
//                    //            MessageUtil.MessageList.MSG3128,
//                    //            new string[] { "lblSubcontractorCodeMsg" },
//                    //            new string[] { "IntroducerCode" });
//                    //            return res;
//                    //        }
//                    //    }
//                    //}

//                    // 3.11 Validate Project code
//                    //CTS140_ScreenParameter session = CTS140_GetImportData();
//                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.ProjectCode) == false
//                        && doValidateBusiness.ProjectCode != session.DSRentalContract.dtTbt_RentalContractBasic[0].ProjectCode)
//                    {
//#if !ROUND1

//                        IInstallationHandler iHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
//                        string status = iHandler.GetInstallationStatus(doValidateBusiness.ProjectCode);
//                        if (status != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
//                        {
//                            res.ResultData = MessageUtil.GetMessage(
//                                MessageUtil.MODULE_CONTRACT,
//                                MessageUtil.MessageList.MSG3274,
//                                doValidateBusiness.ProjectCode);
//                            res.AddErrorMessage(
//                                MessageUtil.MODULE_CONTRACT,
//                                MessageUtil.MessageList.MSG3274,
//                                new string[] { doValidateBusiness.ProjectCode },
//                                new string[] { "ProjectCode" });
//                            return res;
//                        }

//#endif

//                        bool isError = true;
//                        List<string> lst = projectHandler.GetProjectStatus(doValidateBusiness.ProjectCode);
//                        if (lst.Count > 0)
//                        {
//                            if (lst[0] == ProjectStatus.C_PROJECT_STATUS_LASTCOMPLETE)
//                                isError = false;
//                        }
//                        if (isError)
//                        {
//                            res.ResultData = MessageUtil.GetMessage(
//                                MessageUtil.MODULE_CONTRACT, 
//                                MessageUtil.MessageList.MSG3260,
//                                doValidateBusiness.ProjectCode);
//                            res.AddErrorMessage(
//                                MessageUtil.MODULE_CONTRACT,
//                                ScreenID.C_SCREEN_ID_CP33,
//                                MessageUtil.MODULE_CONTRACT,
//                                MessageUtil.MessageList.MSG3260,
//                                new string[] { doValidateBusiness.ProjectCode },
//                                new string[] { "ProjectCode" });
//                                return res;
//                        }
//                    }
//                }

//                //3.12 Validate maintenance target contract code
//                if (doValidateBusiness.IsShowMaintenanceInformation == true)
//                {
//                    //CTS140_ScreenParameter session = CTS140_GetImportData();
//                    if (session.ListDOMaintenanceGridEdit != null)
//                    {
//                        List<string> contractLst = new List<string>();
//                        foreach (CTS140_DOMaintenanceGrid grid in session.ListDOMaintenanceGridEdit)
//                        {
//                            contractLst.Add(grid.ContractCodeLong);
//                        }

//                        IContractHandler contractHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;

//                        try
//                        {
//                            bool isSiteError = true;
//                            if (session.DSRentalContract.dtTbt_RentalContractBasic != null)
//                            {
//                                if (session.DSRentalContract.dtTbt_RentalContractBasic.Count > 0)
//                                {
//                                    List<doContractHeader> contLst = contractHandler.CheckMaintenanceTargetContractList(
//                                        contractLst,
//                                        session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractCode);

//                                    isSiteError = false;
//                                    List<doContractHeader> lst = contLst;
//                                    foreach (doContractHeader l in lst)
//                                    {
//                                        if (l.SiteCode != session.DSRentalContract.dtTbt_RentalContractBasic[0].SiteCode)
//                                        {
//                                            isSiteError = true;
//                                            break;
//                                        }
//                                    }
//                                }
//                            }
//                            if (isSiteError)
//                            {
//                                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3093);
//                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3093);
//                                return res;
//                            }
//                        }
//                        catch (ApplicationErrorException ex)
//                        {
//                            if (res.MessageList == null)
//                                res.AddErrorMessage(ex);
//                            else
//                                res.MessageList.AddRange(ex.ErrorResult.MessageList);
                            
//                            return res;
//                        }
//                    }
//                }

//                if (doValidateBusiness.IsShowDepositInformation == true)
//                {
//                    //3.9 If txtOriginContractCodeForSlide is not existing in tbt_RentalContractBasic.ContractCode and tbt_SaleBasic.ContractCode Then
//                    string strCounterBalanceOriginContractCodeLong = comUtil.ConvertContractCode(doValidateBusiness.CounterBalanceOriginContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
//                    //if (rentralContractHandler.GetTbt_RentalContractBasic(strCounterBalanceOriginContractCodeLong, null).Count() == 0)
//                    if (comContractHandler.IsContractExistInRentalOrSale(strCounterBalanceOriginContractCodeLong) == false)
//                    {
//                        string[] param = { doValidateBusiness.CounterBalanceOriginContractCode };
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, doValidateBusiness.CounterBalanceOriginContractCode);
//                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, param, new string[] { "CounterBalanceOriginContractCode" });
//                        return res;
//                    }

//                    //Bug report CT-148
//                    if (session.contractCode == strCounterBalanceOriginContractCodeLong)
//                    {
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281, null, new string[] { "CounterBalanceOriginContractCode" });
//                        return res;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            //return null;
//            return res;
//        }

//        /// <summary>
//        /// Validate data and show warning message
//        /// </summary>
//        /// <param name="doValidateBusiness"></param>
//        /// <returns></returns>
//        public ObjectResultData ValidateForWarning_CTS140(CTS140_DOValidateBusiness doValidateBusiness)
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                if (doValidateBusiness.MaintenanceFeeTypeCode == "none")
//                    doValidateBusiness.MaintenanceFeeTypeCode = null;
//                if (session.DOMaintenanceInformation.MaintenanceFeeTypeCode != doValidateBusiness.MaintenanceFeeTypeCode)
//                {
//                    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
//                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3151, null);

//                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
//                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3151, null, null);
//                    return res;
//                }

//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return null;
//        }

//        /// <summary>
//        /// Check PeriodDate is Overlap
//        /// </summary>
//        /// <param name="BS"></param>
//        /// <param name="BE"></param>
//        /// <returns></returns>
//        public bool ValidatePeriodverLap_CTS140(DateTime? BS, DateTime? BE)
//        {
//            ObjectResultData res = new ObjectResultData();
//            CTS140_ScreenParameter session;

//            session = CTS140_GetImportData();
//            DateTime? TS = session.ListDOCancelContractGrid.Max(a => a.StartPeriodDate);
//            DateTime? TE = session.ListDOCancelContractGrid.Min(a => a.EndPeriodDate);

//            return
//            (
//            (TS >= BS && TS < BE)
//            ||
//            (TE <= BE && TE > BS)
//            ||
//            (TS <= BS && TE >= BE)
//            );
//        }

//        /// <summary>
//        /// Check PeriodDate is Overlap
//        /// </summary>
//        /// <param name="startPeriodDate"></param>
//        /// <param name="endPeriodDate"></param>
//        /// <param name="strBillingType"></param>
//        /// <returns></returns>
//        public bool ValidatePeriodverLap_CTS140(DateTime? startPeriodDate, DateTime? endPeriodDate, string strBillingType)
//        {
//            bool bResult = false;

//            CTS140_ScreenParameter session;
//            session = CTS140_GetImportData();

//            foreach (CTS140_DOCancelContractGrid memoDetail in session.ListDOCancelContractGrid)
//            {
//                if ((strBillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE && memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
//                    || (strBillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE && memoDetail.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE))
//                {
//                    if (CheckOverlapPeriodDate_CTS110(memoDetail.StartPeriodDate, memoDetail.EndPeriodDate, startPeriodDate, endPeriodDate))
//                    {
//                        bResult = true;
//                        break;
//                    }
//                }
//            }

//            return bResult;
//        }

//        /// <summary>
//        /// Validate business for CancelContract when click [Add] button on ‘Cancel contract condition’ section
//        /// </summary>
//        /// <param name="doADDCancelContractCondition"></param>
//        /// <returns></returns>
//        //public ActionResult ValidateAddCancelRequireField_CTS140(CTS140_DOADDCancelContractCondition doADDCancelContractCondition)
//        public ActionResult ValidateAddCancelRequireField_CTS140(CTS140_DOCancelContractCondition doCancelContractCondition)
//        {
//            ObjectResultData res = new ObjectResultData();

//            try
//            {
//                CTS140_DOADDCancelContractCondition doADDCancelContractCondition = CommonUtil.CloneObject<CTS140_DOCancelContractCondition, CTS140_DOADDCancelContractCondition>(doCancelContractCondition);

//                if (ModelState.IsValid == false)
//                {
//                    ValidatorUtil.BuildErrorMessage(res, this);
//                    if (res.IsError)
//                    {
//                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
//                        res.ResultData = false;
//                        return Json(res);
//                    }
//                }

//                object vDo = null;
//                if (doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
//                    || doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
//                    vDo = CommonUtil.CloneObject<CTS140_DOADDCancelContractCondition, CTS140_DOCancelContractCondition_FeeC1>(doADDCancelContractCondition);
//                else if (doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE
//                    || doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE
//                    || doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
//                    vDo = CommonUtil.CloneObject<CTS140_DOADDCancelContractCondition, CTS140_DOCancelContractCondition_FeeC2>(doADDCancelContractCondition);
//                else if (doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
//                {
//                    if (doADDCancelContractCondition.HandlingType == HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE
//                        || doADDCancelContractCondition.HandlingType == HandlingType.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE)
//                        vDo = CommonUtil.CloneObject<CTS140_DOADDCancelContractCondition, CTS140_DOCancelContractCondition_FeeC3>(doADDCancelContractCondition);
//                }
//                else if (doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
//                {
//                    if (doADDCancelContractCondition.HandlingType == HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE)
//                        vDo = CommonUtil.CloneObject<CTS140_DOADDCancelContractCondition, CTS140_DOCancelContractCondition_FeeC4>(doADDCancelContractCondition);
//                }
//                else if (doADDCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
//                {
//                    if (doADDCancelContractCondition.HandlingType == HandlingType.C_HANDLING_TYPE_BILL_UNPAID_FEE
//                        || doADDCancelContractCondition.HandlingType == HandlingType.C_HANDLING_TYPE_EXEMPT_UNPAID_FEE)
//                        vDo = CommonUtil.CloneObject<CTS140_DOADDCancelContractCondition, CTS140_DOCancelContractCondition_FeeC5>(doADDCancelContractCondition);
//                }
//                ValidatorUtil.BuildErrorMessage(res, new object[] { vDo });
//                if (res.IsError)
//                {
//                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
//                    return Json(res);
//                }


//                //ValidateBusiness
//                ValidateBusinessCancelContract_CTS140(res, doCancelContractCondition);
//                if (res.IsError)
//                    return Json(res);

//                //ValidateBusinessForWarning
//                ValidateBusinessCancelContractForWarning_CTS140(res, doCancelContractCondition);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Check screen has authority
//        /// </summary>
//        /// <param name="contractCode"></param>
//        public void HasAuthority_CTS140(string contractCode)
//        {
//            ObjectResultData res = new ObjectResultData();
//            ICommonHandler commonHandler;
//            IRentralContractHandler renderHandler;
//            List<tbt_RentalContractBasic> listRentalContractBasic;

//            try
//            {
//                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
//                renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
//                listRentalContractBasic = renderHandler.GetTbt_RentalContractBasic(contractCode, null);

//                if (commonHandler.IsSystemSuspending())
//                    ViewBag.IsSystemSuspending = true;
//                else
//                    ViewBag.IsSystemSuspending = false;

//                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP33, FunctionID.C_FUNC_ID_OPERATE))
//                    ViewBag.Permission = false;
//                else
//                    ViewBag.Permission = true;

//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }
//        }

//        /// <summary>
//        /// Get Constant for check hide section when initail screen
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult GetConstantHideShowDIV()
//        {
//            CTS140_DOConstantHideDIV res = new CTS140_DOConstantHideDIV();
//            CTS140_ScreenParameter session = new CTS140_ScreenParameter();

//            session = CTS140_GetImportData();
//            res.ContractStatus = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractStatus;
//            res.ProductTypeCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode;
//            res.C_PROD_TYPE_AL = ProductType.C_PROD_TYPE_AL;
//            res.C_PROD_TYPE_BE = ProductType.C_PROD_TYPE_BE;
//            res.C_PROD_TYPE_MA = ProductType.C_PROD_TYPE_MA;
//            res.C_PROD_TYPE_ONLINE = ProductType.C_PROD_TYPE_ONLINE;
//            res.C_PROD_TYPE_SALE = ProductType.C_PROD_TYPE_SALE;
//            res.C_PROD_TYPE_SG = ProductType.C_PROD_TYPE_SG;
//            res.C_PROD_TYPE_RENTAL_SALE = ProductType.C_PROD_TYPE_RENTAL_SALE;
//            res.C_CONTRACT_STATUS_AFTER_START = ContractStatus.C_CONTRACT_STATUS_AFTER_START;
//            res.C_CONTRACT_STATUS_BEF_START = ContractStatus.C_CONTRACT_STATUS_BEF_START;
//            res.C_CONTRACT_STATUS_CANCEL = ContractStatus.C_CONTRACT_STATUS_CANCEL;
//            res.C_CONTRACT_STATUS_END = ContractStatus.C_CONTRACT_STATUS_END;
//            res.C_CONTRACT_STATUS_FIXED_CANCEL = ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL;
//            res.C_CONTRACT_STATUS_STOPPING = ContractStatus.C_CONTRACT_STATUS_STOPPING;

//            return Json(res);
//        }

//        #endregion

//        #region Event

//        /// <summary>
//        /// Validate business when click [Register] button in ‘Action button’ section
//        /// </summary>
//        /// <param name="doValidateBusiness"></param>
//        /// <returns></returns>
//        public ActionResult Register_CTS140(CTS140_DOValidateBusiness doValidateBusiness)
//        {
//            bool isGenerateMAScheduleAgain = false;
//            ObjectResultData res = new ObjectResultData();
//            ObjectResultData resValidateBusiness = new ObjectResultData();
//            ObjectResultData resValidateForWarning = new ObjectResultData();

//            ICommonHandler commonHandler;
//            CTS140_ScreenParameter session;
//            CommonUtil comUtil = new CommonUtil();

//            try
//            {
//                session = CTS140_GetImportData();
//                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

//                //7.1 Check suspending
//                if (commonHandler.IsSystemSuspending())
//                {
//                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, null, null);
//                    return Json(res);
//                }

//                //Check user’s permission
//                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP33, FunctionID.C_FUNC_ID_OPERATE) == false)
//                {
//                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
//                    return Json(res);
//                }


//                if (ModelState.IsValid == false)
//                {
//                    ValidatorUtil.BuildErrorMessage(res, this);
//                    if (res.IsError)
//                    {
//                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
//                        return Json(res);
//                    }
//                }

//                //7.2 Map data from screen into dtEntireContract
//                //7.2.1 Check the maintenance information
//                isGenerateMAScheduleAgain = false;
//                if (doValidateBusiness.ContractDurationMonth != session.DOContractRelateInformation.ContractDurationMonth)
//                    isGenerateMAScheduleAgain = true;
//                else
//                {
//                    string mac = doValidateBusiness.MaintenanceCycle != "none" ? doValidateBusiness.MaintenanceCycle : null;
//                    int? imac = null;
//                    int ii = 0;
//                    if (int.TryParse(mac, out ii))
//                        imac = ii;

//                    if (imac != session.DOMaintenanceInformation.MaintenanceCycle)
//                        isGenerateMAScheduleAgain = true;
//                    else if (session.ListDOMaintenanceGrid != null)
//                    {
//                        if (session.ListDOMaintenanceGrid.Count() != session.ListDOMaintenanceGridEdit.Count())
//                            isGenerateMAScheduleAgain = true;
//                        else
//                        {
//                            foreach (var item in session.ListDOMaintenanceGrid)
//                            {
//                                if (session.ListDOMaintenanceGridEdit.FindAll(delegate(CTS140_DOMaintenanceGrid s) { return s.ContractCode == item.ContractCode && s.ProductName == item.ProductName; }).Count() == 0)
//                                {
//                                    isGenerateMAScheduleAgain = true;
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }

//                //7.2.2 Map for removal installation fee
//                if (session.ListDOCancelContractGrid != null)
//                {
//                    foreach (CTS140_DOCancelContractGrid grid in session.ListDOCancelContractGrid)
//                    {
//                        if (grid.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
//                        {
//                            if (session.DSRentalContract.dtTbt_RentalSecurityBasic != null)
//                            {
//                                if (session.DSRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
//                                {
//                                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFee = grid.NormalFeeAmount;
//                                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee = grid.FeeAmount;
//                                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = grid.FeeAmount;
//                                }
//                            }
//                        }
//                    }
//                }

//                //7.3 Map data from 'Maintenance target contract list' into dtTbt_RelationType
//                List<tbt_RelationType> ListRelationTypeEdit = CommonUtil.ClonsObjectList<tbt_RelationType, tbt_RelationType>(session.ListRelationType);
//                foreach (var item in ListRelationTypeEdit)
//                {
//                    item.ContractCode = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
//                    item.OCC = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
//                    item.RelatedContractCode = doValidateBusiness.ContractCode;
//                    item.RelationType = RelationType.C_RELATION_TYPE_MA;
//                }
//                session.ListRelationType = ListRelationTypeEdit;

//                //7.5 Validate business
//                resValidateBusiness = ValidateBusiness_CTS140(doValidateBusiness);
//                if (resValidateBusiness != null && resValidateBusiness.IsError)
//                    return Json(resValidateBusiness);

//                //ContractRelateInformation
//                if (doValidateBusiness.IsShowContractRelatedInformation == true)
//                {
//                    session.DSRentalContract.dtTbt_RentalContractBasic[0].StartDealDate = doValidateBusiness.StartDealDate;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractStartDate = doValidateBusiness.ContractStartDate;
                    
//                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractEndDate.HasValue
//                        && session.DSRentalContract.dtTbt_RentalSecurityBasic[0].CalContractEndDate != doValidateBusiness.ContractEndDate)
//                    {
//                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractEndDate = doValidateBusiness.ContractEndDate;
//                    }
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].CalContractEndDate = doValidateBusiness.ContractEndDate;

//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractDurationMonth = doValidateBusiness.ContractDurationMonth;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AutoRenewMonth = doValidateBusiness.AutoRenewMonth;
//                    session.DSRentalContract.dtTbt_RentalContractBasic[0].ProjectCode = doValidateBusiness.ProjectCode;
//                }

//                //ContractAgreementInformation           
//                if (doValidateBusiness.IsShowContractAgreementInformation)
//                {
//                    session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractOfficeCode = doValidateBusiness.ContractOfficeCode;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PlanCode = doValidateBusiness.PlanCode;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1 = doValidateBusiness.SalesmanEmpNo1;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2 = doValidateBusiness.SalesmanEmpNo2;
//                    session.DSRentalContract.dtTbt_RentalContractBasic[0].OldContractCode = comUtil.ConvertContractCode(doValidateBusiness.OldContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
//                    session.DSRentalContract.dtTbt_RentalContractBasic[0].AcquisitionTypeCode = doValidateBusiness.AcquisitionTypeCode;
//                    session.DSRentalContract.dtTbt_RentalContractBasic[0].MotivationTypeCode = doValidateBusiness.MotivationTypeCode;
//                    session.DSRentalContract.dtTbt_RentalContractBasic[0].IntroducerCode = doValidateBusiness.IntroducerCode;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesSupporterEmpNo = doValidateBusiness.SalesSupporterEmpNo;
//                }

//                //Deposit Information

//                if (doValidateBusiness.IsShowDepositInformation == true)
//                {
//                    if (doValidateBusiness.NormalDepositFee != null && doValidateBusiness.NormalDepositFee != "")
//                        session.DSRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFee = decimal.Parse(doValidateBusiness.NormalDepositFee);

//                    if (doValidateBusiness.OrderDepositFee != null && doValidateBusiness.OrderDepositFee != "")
//                        session.DSRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFee = decimal.Parse(doValidateBusiness.OrderDepositFee);

//                    session.DSRentalContract.dtTbt_RentalContractBasic[0].CounterBalanceOriginContractCode = comUtil.ConvertContractCode(doValidateBusiness.CounterBalanceOriginContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
//                }

//                //Contract document information
//                if (doValidateBusiness.IsShowContractDocumentInformation == true)
//                    session.DSRentalContract.dtTbt_RentalContractBasic[0].IrregurationDocUsageFlag = doValidateBusiness.IrregurationDocUsageFlag;

//                //Provide service information

//                if (doValidateBusiness.IsShowProvideServiceInformation == true)
//                {
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].FireMonitorFlag = doValidateBusiness.FireMonitorFlag;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].CrimePreventFlag = doValidateBusiness.CrimePreventFlag;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag = doValidateBusiness.EmergencyReportFlag;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag = doValidateBusiness.FacilityMonitorFlag;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1 = doValidateBusiness.PhoneLineTypeCode1;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1 = doValidateBusiness.PhoneLineOwnerCode1;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo1 = doValidateBusiness.PhoneNo1;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode2 = doValidateBusiness.PhoneLineTypeCode2;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode2 = doValidateBusiness.PhoneLineOwnerCode2;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo2 = doValidateBusiness.PhoneNo2;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode3 = doValidateBusiness.PhoneLineTypeCode3;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode3 = doValidateBusiness.PhoneLineOwnerCode3;
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PhoneNo3 = doValidateBusiness.PhoneNo3;
//                }

//                //Maintenanece Information

//                if (doValidateBusiness.IsShowMaintenanceInformation == true)
//                {
//                    if (session.DSRentalContract.dtTbt_RentalMaintenanceDetails != null && session.DSRentalContract.dtTbt_RentalMaintenanceDetails.Count() > 0)
//                    {
//                        session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceTypeCode = doValidateBusiness.MaintenanceTypeCode;

//                        if (doValidateBusiness.MaintenanceCycle != null && doValidateBusiness.MaintenanceCycle != "")
//                            session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceCycle = int.Parse(doValidateBusiness.MaintenanceCycle);

//                        if (doValidateBusiness.MaintenanceContractStartMonth != null && doValidateBusiness.MaintenanceContractStartMonth != "")
//                            session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartMonth = int.Parse(doValidateBusiness.MaintenanceContractStartMonth);

//                        if (doValidateBusiness.MaintenanceContractStartYear != null && doValidateBusiness.MaintenanceContractStartYear != "")
//                            session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartYear = int.Parse(doValidateBusiness.MaintenanceContractStartYear);

//                        session.DSRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode = doValidateBusiness.MaintenanceFeeTypeCode;
//                    }
//                }

//                //Site Information

//                if (doValidateBusiness.IsShowSiteInformation == true)
//                {
//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].BuildingTypeCode = doValidateBusiness.BuildingTypeCode;

//                    if (doValidateBusiness.SiteBuildingArea != null && doValidateBusiness.SiteBuildingArea != "")
//                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SiteBuildingArea = decimal.Parse(doValidateBusiness.SiteBuildingArea);

//                    if (doValidateBusiness.NumOfBuilding != null && doValidateBusiness.NumOfBuilding != "")
//                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfBuilding = int.Parse(doValidateBusiness.NumOfBuilding);

//                    if (doValidateBusiness.SecurityAreaFrom != null && doValidateBusiness.SecurityAreaFrom != "")
//                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom = decimal.Parse(doValidateBusiness.SecurityAreaFrom);

//                    if (doValidateBusiness.SecurityAreaTo != null && doValidateBusiness.SecurityAreaTo != "")
//                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityAreaTo = decimal.Parse(doValidateBusiness.SecurityAreaTo);

//                    if (doValidateBusiness.NumOfFloor != null && doValidateBusiness.NumOfFloor != "")
//                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NumOfFloor = int.Parse(doValidateBusiness.NumOfFloor);

//                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MainStructureTypeCode = doValidateBusiness.MainStructureTypeCode;

//                }

//                //Cancel Contract Condition
//                if (session.DSRentalContract.dtTbt_CancelContractMemo != null && session.DSRentalContract.dtTbt_CancelContractMemo.Count > 0)
//                {
//                    session.DSRentalContract.dtTbt_CancelContractMemo[0].ProcessAfterCounterBalanceType = doValidateBusiness.ProcessAfterCounterBalanceType;
//                    session.DSRentalContract.dtTbt_CancelContractMemo[0].OtherRemarks = doValidateBusiness.OtherRemarks;
//                    session.DSRentalContract.dtTbt_CancelContractMemo[0].TotalSlideAmt = doValidateBusiness.TotalSlideAmt;
//                    session.DSRentalContract.dtTbt_CancelContractMemo[0].TotalReturnAmt = doValidateBusiness.TotalReturnAmt;
//                    session.DSRentalContract.dtTbt_CancelContractMemo[0].TotalBillingAmt = doValidateBusiness.TotalBillingAmt;
//                    session.DSRentalContract.dtTbt_CancelContractMemo[0].TotalAmtAfterCounterBalance = doValidateBusiness.TotalAmtAfterCounterBalance;
//                }

//                //Other Infromation
//                session.DSRentalContract.dtTbt_RentalContractBasic[0].Memo = doValidateBusiness.Memo;

//                doValidateBusiness.IsGenerateMAScheduleAgain = isGenerateMAScheduleAgain;
//                session.DOValidateBusiness = doValidateBusiness;

//                CTS140_SetImportData(session);

//                //7.6 Validate business for warning
//                resValidateForWarning = ValidateForWarning_CTS140(doValidateBusiness);
//                if (resValidateForWarning != null)
//                {
//                    if (resValidateForWarning.ResultData == null)
//                        resValidateForWarning.ResultData = true;
//                    return Json(resValidateForWarning);
//                }

//                //อันนีี้ไม่ได้อย่ใน เอกสารนะครับสงสัยถามพี่ AMP ได้
//                if (doValidateBusiness.IsShowCancelContractCondition == true)
//                {
//                    if (session.ListDOCancelContractGrid.Count() == 0)
//                    {
//                        //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
//                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3113, null);

//                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3113, null, null);
//                        res.ResultData = true;
//                        return Json(res);
//                    }
//                }

//                res.ResultData = true;
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Validate business when click [Confirm] button in ‘Action button’ section
//        /// </summary>
//        /// <param name="doValidateBusiness"></param>
//        /// <returns></returns>
//        public ActionResult Confirm_CTS140(CTS140_DOValidateBusiness doValidateBusiness)
//        {
//            ObjectResultData res = new ObjectResultData();
//            ObjectResultData resValidateBusiness = new ObjectResultData();
//            ObjectResultData resValidateForWarning = new ObjectResultData();

//            ICommonHandler commonHandler;
//            IRentralContractHandler rentralContractHandler;
//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
//                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

//                //9.1 Check suspending
//                if (commonHandler.IsSystemSuspending())
//                {
//                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, null);
//                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, null, null);
//                    return Json(res);
//                }

//                //Check user’s permission
//                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP33, FunctionID.C_FUNC_ID_OPERATE) == false)
//                {
//                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
//                    return Json(res);
//                }


//                //9.2 Validate business
//                resValidateBusiness = ValidateBusiness_CTS140(doValidateBusiness);
//                if (resValidateBusiness != null && resValidateBusiness.IsError)
//                    return Json(resValidateBusiness);

//                //resValidateForWarning = ValidateForWarning_CTS140(doValidateBusiness);
//                //if (resValidateForWarning != null)
//                //    return Json(resValidateForWarning);

//                //9.3 Checking for update billing temp
//                if (doValidateBusiness.IsContractCancelShow == true)
//                {
//                    //foreach (var item in session.ListDOCancelContractGrid)
//                    //{
//                    //    if (item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE && item.StatusGrid == "ADD")
//                    //    {
//                    //        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3124, null);
//                    //        return Json(res);
//                    //    }

//                    //    if (item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE && session.ListDOCancelContractGridRemove.Count() != 0)
//                    //    {
//                    //        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3124, null);
//                    //        return Json(res);
//                    //    }
//                    //}

//                    List<tbt_CancelContractMemoDetail> dtMemoDetialList = session.DSRentalContract.dtTbt_CancelContractMemoDetail.FindAll(delegate(tbt_CancelContractMemoDetail s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; });
//                    List<CTS140_DOCancelContractGrid> dtMemoDetialTempList = session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; });

//                    if (dtMemoDetialList != null && dtMemoDetialTempList != null)
//                    {
//                        if ((dtMemoDetialList.Count == 0 && dtMemoDetialTempList.Count > 0) || (dtMemoDetialList.Count > 0 && dtMemoDetialTempList.Count == 0))
//                        {
//                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3124, null);
//                            return Json(res);
//                        }
//                    }

//                }
//                else
//                {
//                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3125, null);
//                    return Json(res);
//                }

//            }
//            catch (Exception ex)
//            {
//                res.ResultData = false;
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Update data to database when click [Confirm] button in ‘Action button’ section
//        /// </summary>
//        /// <param name="isUpdateRemovalFeeToBillingTemp"></param>
//        /// <returns></returns>
//        public ActionResult Save_CTS140(bool isUpdateRemovalFeeToBillingTemp)
//        {
//            ObjectResultData res = new ObjectResultData();
//            IRentralContractHandler rentralContractHandler;
//            List<string> listContractCode;
//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                listContractCode = new List<string>();

//                foreach (var item in session.ListDOMaintenanceGridEdit)
//                {
//                    //listContractCode.Add(item.ContractCode);
//                    listContractCode.Add(item.ContractCodeLong);
//                }

//                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

//                //rentralContractHandler.RegisterCP33(session.DSRentalContract, session.ListRelationType, listContractCode, isUpdateRemovalFeeToBillingTemp, session.DOValidateBusiness.IsGenerateMAScheduleAgain);
//                //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046, null);
//                //return Json(res);

//                using (TransactionScope scope = new TransactionScope())
//                {
//                    string strContractCode = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
//                    string strOCC = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OCC;

//                    List<tbt_CancelContractMemoDetail> cancelContractMemoDetailList = new List<tbt_CancelContractMemoDetail>();

//                    int intSequenceNo = 1;
//                    foreach (CTS140_DOCancelContractGrid memoDetailTemp in session.ListDOCancelContractGrid)
//                    {
//                        tbt_CancelContractMemoDetail memoDetail = new tbt_CancelContractMemoDetail();
//                        memoDetail.ContractCode = strContractCode;
//                        memoDetail.OCC = strOCC;
//                        memoDetail.SequenceNo = intSequenceNo;
//                        memoDetail.BillingType = memoDetailTemp.BillingType;
//                        memoDetail.HandlingType = memoDetailTemp.HandlingType;
//                        memoDetail.StartPeriodDate = memoDetailTemp.StartPeriodDate;
//                        memoDetail.EndPeriodDate = memoDetailTemp.EndPeriodDate;
//                        memoDetail.FeeAmount = memoDetailTemp.FeeAmount;
//                        memoDetail.TaxAmount = memoDetailTemp.TaxAmount;
//                        memoDetail.NormalFeeAmount = memoDetailTemp.NormalFeeAmount;
//                        memoDetail.ContractCode_CounterBalance = memoDetailTemp.ContractCode_CounterBalance;
//                        memoDetail.Remark = memoDetailTemp.Remark;
//                        memoDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
//                        memoDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
//                        memoDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
//                        memoDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
//                        cancelContractMemoDetailList.Add(memoDetail);

//                        intSequenceNo++;
//                    }

//                    session.DSRentalContract.dtTbt_CancelContractMemoDetail = CommonUtil.ClonsObjectList<tbt_CancelContractMemoDetail, tbt_CancelContractMemoDetail>(cancelContractMemoDetailList);
//                    rentralContractHandler.RegisterCP33(session.DSRentalContract, session.ListRelationType, listContractCode, isUpdateRemovalFeeToBillingTemp, session.DOValidateBusiness.IsGenerateMAScheduleAgain);

//                    scope.Complete();

//                    //CommonUtil.dsTransData.dtCommonSearch.ContractCode = string.Empty;
//                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046, null);
//                }
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Add data to Maintenance grid when click [Add] button in ‘Maintenance information’ section
//        /// </summary>
//        /// <param name="doMaintenanceGrid"></param>
//        /// <returns></returns>
//        public ActionResult Add_CTS140(CTS140_DOMaintenanceGrid doMaintenanceGrid)
//        {
//            ObjectResultData res = new ObjectResultData();

//            try
//            {
//                if (ModelState.IsValid == false)
//                {
//                    ValidatorUtil.BuildErrorMessage(res, this);
//                    if (res.IsError)
//                    {
//                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
//                        return Json(res);
//                    }
//                }

//                CTS140_ScreenParameter session = CTS140_GetImportData();

//                // 3.1 Check whether contract code can be used as maintenance target contract or not
//                CommonUtil comUtil = new CommonUtil();
//                string strContractCodeLong = comUtil.ConvertContractCode(doMaintenanceGrid.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

//                IContractHandler contractHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
//                //TODO: (Jutarat) Bug report QU-124 Add param quotation target code
//                doContractHeader doContractHeader = contractHandler.CheckMaintenanceTargetContract(strContractCodeLong,
//                    session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractCode);
//                if (doContractHeader != null)
//                {
//                    #region Validate business

//                    bool isSameSite = false;
//                    if (session.DSRentalContract.dtTbt_RentalContractBasic != null)
//                    {
//                        if (session.DSRentalContract.dtTbt_RentalContractBasic.Count > 0)
//                        {
//                            if (doContractHeader.SiteCode == session.DSRentalContract.dtTbt_RentalContractBasic[0].SiteCode)
//                                isSameSite = true;
//                        }
//                    }
//                    if (isSameSite == false)
//                    {
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3261);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3261, null, new string[]{"MaintenanceTargetContractCode"});
//                        return Json(res);
//                    }

//                    string duplicateCode = null;
//                    bool isSameProductType = false;
//                    bool isSaleExist = false;

//                    CommonUtil cmm = new CommonUtil();
//                    string contractCodeShort = cmm.ConvertContractCode(doContractHeader.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
//                    foreach (CTS140_DOMaintenanceGrid ma in session.ListDOMaintenanceGridEdit)
//                    {
//                        // 3.2.1 Check the duplicate contract code in the list
//                        if (duplicateCode == null
//                            && ma.ContractCode == contractCodeShort)
//                        {
//                            duplicateCode = contractCodeShort;
//                            break;
//                        }
//                        // 3.2.2 Can add only 1 N code
//                        else if (
//                            (doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_AL
//                            || doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
//                                || doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
//                            && ( ma.ProductTypeCode == ProductType.C_PROD_TYPE_AL
//                            || ma.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
//                                    || ma.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
//                        {
//                            isSameProductType = true;
//                        }
//                        // 3.2.3 Cannot add rental contract and sale contract together
//                        else if (
                            
//                            ((doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_AL
//                                || doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
//                                || doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
//                                && ma.ProductTypeCode == ProductType.C_PROD_TYPE_SALE)

//                            || ((ma.ProductTypeCode == ProductType.C_PROD_TYPE_AL
//                            || ma.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
//                                    || ma.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
//                            && doContractHeader.ProductTypeCode == ProductType.C_PROD_TYPE_SALE))
//                        {
//                            isSaleExist = true;
//                        }
//                    }
//                    if (duplicateCode != null)
//                    {
//                        string[] param = { duplicateCode };
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3194, duplicateCode);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3194, param, new string[] { "MaintenanceTargetContractCode" });
//                        return Json(res);
//                    }
//                    if (isSameProductType)
//                    {
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0032);
//                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0032, null, new string[] { "MaintenanceTargetContractCode" });
//                        return Json(res);
//                    }
//                    if (isSaleExist)
//                    {
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0132);
//                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0132, null, new string[] { "MaintenanceTargetContractCode" });
//                        return Json(res);
//                    }

//                    #endregion


//                    doMaintenanceGrid.ContractCode = 
//                        comUtil.ConvertContractCode(doContractHeader.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
//                    doMaintenanceGrid.ProductCode = doContractHeader.ProductCode;
//                    doMaintenanceGrid.ProductName = doContractHeader.ProductName;
//                    doMaintenanceGrid.ProductTypeCode = doContractHeader.ProductTypeCode;

//                    session.ListDOMaintenanceGridEdit.Add(doMaintenanceGrid);
//                    CTS140_SetImportData(session);

//                    res.ResultData = session.ListDOMaintenanceGridEdit;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.ResultData = false;
//                res.AddErrorMessage(ex);
//                res.Message.Controls = new string[] { "MaintenanceTargetContractCode" };
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Add data to CancelContract grid when click [Add] button in ‘Cancel contract condition’ section
//        /// </summary>
//        /// <param name="doCancelContractCondition"></param>
//        /// <returns></returns>
//        public ActionResult AddCancel_CTS140(CTS140_DOCancelContractCondition doCancelContractCondition) //3.1.1 If not pass required field validation (อยู่ใน Model นะครับ)
//        {
//            ObjectResultData res = new ObjectResultData();
//            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

//            List<CTS140_DOCancelContractGrid> listDOCancelContractGrid;
//            //List<doMiscTypeCode> listDoMiscTypeCode;
//            //doMiscTypeCode doMiscType;

//            ICommonHandler commonHandler;
//            CTS140_ScreenParameter session;
//            CommonUtil comUtil = new CommonUtil();
//            ICommonContractHandler comContractHandler;
                
//            try
//            {
//                session = CTS140_GetImportData();
//                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
//                comContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;

//                #region Move to ValidateAddCancelRequireField_CTS140()
//                ////3.2.1 When dtpPeriodFrom and dtpPeriodTo is a required field
//                //if (doCancelContractCondition.StartPeriodDate > doCancelContractCondition.EndPeriodDate)
//                //{
//                //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0101, null);
//                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0101, null, new string[] { "StartPeriodDateContract" });
//                //    return Json(res);
//                //}

//                ////3.2.2	When ddlFeeType is one of the following items
//                ////-	C_BILLING_TYPE_DEPOSIT_FEE
//                ////-	C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE
//                ////-	C_BILLING_TYPE_CANCEL_CONTRACT_FEE
//                ////-	C_BILLING_TYPE_CHANGE_INSTALLATION_FEE
//                ////-	C_BILLING_TYPE_CARD_FEE
//                //if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE ||
//                //    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE ||
//                //    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE ||
//                //    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE ||
//                //    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE)
//                //{
//                //    listDOCancelContractGrid = session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == doCancelContractCondition.BillingType; });
//                //    if (listDOCancelContractGrid.Count() != 0)
//                //    {
//                //        //3.2.2.1 The adding fee type must not existing in ‘Cancel contract condition list’ Show Show message when not pass validation
//                //        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null);
//                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null, new string[] { "FeeType" });
//                //        return Json(res);
//                //    }
//                //}

//                ////3.2.3 When ddlFeeType = C_BILLING_TYPE_CONTRACT_FEE
//                //if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
//                //{
//                //    //Move to ValidateBusinessCancelContractForWarning_CTS140()
//                //    ////3.2.3.1 The period (from - to) must not overlap with the registered contact fee in ‘Cancel contract condition list’
//                //    ////if (ValidatePeriodverLap_CTS140(doCancelContractCondition.StartPeriodDate, doCancelContractCondition.EndPeriodDate) == true)
//                //    //if (ValidatePeriodverLap_CTS140(doCancelContractCondition.StartPeriodDate, doCancelContractCondition.EndPeriodDate, doCancelContractCondition.BillingType))
//                //    //{
//                //    //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null);
//                //    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null, new string[] { "StartPeriodDateContract", "EndPeriodDateContract" });
//                //    //    return Json(res);
//                //    //}

//                //    //3.2.3.2 Maintenanace fee must not registered in ‘Cancel contract condition list’
//                //    if (session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE; }).Count() != 0)
//                //    {
//                //        string[] message = new string[] { doCancelContractCondition.C_ContractFeeValidate, doCancelContractCondition.C_MaintenanceFeeValidate };
//                //        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message);
//                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message, new string[] { "FeeType" });
//                //        return Json(res);
//                //    }
//                //}
//                ////3.2.4
//                //else if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
//                //{
//                //    //Move to ValidateBusinessCancelContractForWarning_CTS140()
//                //    ////if (ValidatePeriodverLap_CTS140(doCancelContractCondition.StartPeriodDate, doCancelContractCondition.EndPeriodDate) == true)
//                //    //if (ValidatePeriodverLap_CTS140(doCancelContractCondition.StartPeriodDate, doCancelContractCondition.EndPeriodDate, doCancelContractCondition.BillingType))
//                //    //{
//                //    //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null);
//                //    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null, new string[] { "StartPeriodDateContract", "EndPeriodDateContract" });
//                //    //    return Json(res);
//                //    //}

//                //    if (session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE; }).Count() != 0)
//                //    {
//                //        string[] message = new string[] { doCancelContractCondition.C_MaintenanceFeeValidate, doCancelContractCondition.C_ContractFeeValidate };
//                //        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message);
//                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message, new string[] { "FeeType" });
//                //        return Json(res);
//                //    }
//                //}
//                //else
//                //{
//                //    //ไม่ได้อยู่ใน Spec นะครับเป็นการเช็คไม่ให้ Add ซ้ำครับ
//                //    if (session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == doCancelContractCondition.BillingType && s.HandlingType == doCancelContractCondition.HandlingType; }).Count() != 0)
//                //    {
//                //        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null);
//                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null, new string[] { "FeeType", "HandlingType" });
//                //        return Json(res);
//                //    }
//                //}

//                ////3.2.5
//                //if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
//                //{
//                //    //if (session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; }).Count() == 0)
//                //    if (session.DSRentalContract.dtTbt_CancelContractMemoDetail.FindAll(delegate(tbt_CancelContractMemoDetail s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; }).Count() == 0)
//                //    {
//                //        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3122, null);
//                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3122, null, new string[] { "FeeType" });
//                //        return Json(res);
//                //    }
//                //}

//                //if (doCancelContractCondition.ContractCode_CounterBalance != null
//                //    && doCancelContractCondition.ContractCode_CounterBalance != "none")
//                //{
//                //    string strContractCode_CounterBalanceLong = comUtil.ConvertContractCode(doCancelContractCondition.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_LONG);
//                //    if (comContractHandler.IsContractExistInRentalOrSale(strContractCode_CounterBalanceLong) == false)
//                //    {
//                //        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, doCancelContractCondition.ContractCode_CounterBalance);
//                //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, new string[] { doCancelContractCondition.ContractCode_CounterBalance }, new string[] { "ContractCode_CounterBalance" });
//                //        return Json(res);
//                //    }

//                //    //Bug report CT-148
//                //    if (session.contractCode == strContractCode_CounterBalanceLong)
//                //    {
//                //        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281);
//                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281, null, new string[] { "ContractCode_CounterBalance" });
//                //        return Json(res);
//                //    }
//                //}

//                ////ValidateBusinessForWarning
//                //ValidateBusinessCancelContractForWarning_CTS140(res, doCancelContractCondition);
//                #endregion

//                CTS140_DOCancelContractGrid doCancelContractMemoDetail = new CTS140_DOCancelContractGrid();

//                if (doCancelContractCondition.FeeAmount != null && doCancelContractCondition.FeeAmount != "none")
//                    doCancelContractMemoDetail.FeeAmount = decimal.Parse(doCancelContractCondition.FeeAmount);

//                if (doCancelContractCondition.FeeAmount != null && doCancelContractCondition.FeeAmount != "none")
//                    doCancelContractMemoDetail.FeeAmountString = doCancelContractCondition.FeeAmount;

//                if (doCancelContractCondition.TaxAmount != null && doCancelContractCondition.TaxAmount != "none")
//                    doCancelContractMemoDetail.TaxAmount = decimal.Parse(doCancelContractCondition.TaxAmount);

//                if (doCancelContractCondition.TaxAmount != null && doCancelContractCondition.TaxAmount != "none")
//                    doCancelContractMemoDetail.TaxAmountString = doCancelContractCondition.TaxAmount;

//                if (doCancelContractCondition.BillingType != null && doCancelContractCondition.BillingType != "none")
//                    doCancelContractMemoDetail.BillingType = doCancelContractCondition.BillingType;

//                if (doCancelContractCondition.HandlingType != null && doCancelContractCondition.HandlingType != "none")
//                    doCancelContractMemoDetail.HandlingType = doCancelContractCondition.HandlingType;

//                //listDoMiscTypeCode = new List<doMiscTypeCode>();
//                //doMiscType = new doMiscTypeCode();
//                //doMiscType.ValueCode = doCancelContractCondition.BillingType;
//                //doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_CONTRACT_BILLING_TYPE;
//                //listDoMiscTypeCode.Add(doMiscType);
//                //commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
//                //listDoMiscTypeCode = commonHandler.GetMiscTypeCodeList(listDoMiscTypeCode);
//                //if (listDoMiscTypeCode.Count() != 0)
//                //    doCancelContractMemoDetail.BillingTypeName = listDoMiscTypeCode[0].ValueDisplay;

//                //doCancelContractMemoDetail.HandlingType = doCancelContractCondition.HandlingType;
//                //listDoMiscTypeCode = new List<doMiscTypeCode>();
//                //doMiscType = new doMiscTypeCode();
//                //doMiscType.ValueCode = doCancelContractCondition.HandlingType;
//                //doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
//                //listDoMiscTypeCode.Add(doMiscType);
//                //commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
//                //listDoMiscTypeCode = commonHandler.GetMiscTypeCodeList(listDoMiscTypeCode);
//                //if (listDoMiscTypeCode.Count() != 0)
//                //    doCancelContractMemoDetail.HandlingTypeName = listDoMiscTypeCode[0].ValueDisplay;

//                if (doCancelContractCondition.StartPeriodDate != null)
//                    doCancelContractMemoDetail.StartPeriodDate = doCancelContractCondition.StartPeriodDate;

//                if (doCancelContractCondition.EndPeriodDate != null)
//                    doCancelContractMemoDetail.EndPeriodDate = doCancelContractCondition.EndPeriodDate;

//                //if (doCancelContractCondition.Remark == "none")
//                //    doCancelContractMemoDetail.Remark = "";
//                if (doCancelContractCondition.Remark != null && doCancelContractCondition.Remark != "none")
//                    doCancelContractMemoDetail.Remark = doCancelContractCondition.Remark;

//                if (doCancelContractCondition.ContractCode_CounterBalance != null && doCancelContractCondition.ContractCode_CounterBalance != "none")
//                    doCancelContractMemoDetail.ContractCode_CounterBalance = comUtil.ConvertContractCode(doCancelContractCondition.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_LONG);

//                if (doCancelContractCondition.NormalFeeAmount != null && doCancelContractCondition.NormalFeeAmount != "none")
//                    doCancelContractMemoDetail.NormalFeeAmount = decimal.Parse(doCancelContractCondition.NormalFeeAmount);

//                doCancelContractMemoDetail.StatusGrid = "ADD";
//                doCancelContractMemoDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
//                doCancelContractMemoDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

//                string strSequence = "0";
//                if (session.ListDOCancelContractGrid.Count == 0)
//                    strSequence = "0";
//                else
//                    strSequence = (int.Parse(session.ListDOCancelContractGrid.Max(t => t.Sequence)[0].ToString()) + 1).ToString();

//                doCancelContractMemoDetail.Sequence = strSequence;

//                MiscTypeMappingList miscList = new MiscTypeMappingList();
//                miscList.AddMiscType(doCancelContractMemoDetail);
//                commonHandler.MiscTypeMappingList(miscList);

//                session.ListDOCancelContractGrid.Add(doCancelContractMemoDetail);
//                //session.DSRentalContract.dtTbt_CancelContractMemoDetail.Add(doCancelContractMemoDetail);

//                CTS140_SetImportData(session);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// ValidateBusiness Cancel Contract
//        /// </summary>
//        /// <param name="doCancelContractCondition"></param>
//        /// <returns></returns>
//        private void ValidateBusinessCancelContract_CTS140(ObjectResultData res, CTS140_DOCancelContractCondition doCancelContractCondition)
//        {
//            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

//            List<CTS140_DOCancelContractGrid> listDOCancelContractGrid;
//            //List<doMiscTypeCode> listDoMiscTypeCode;
//            //doMiscTypeCode doMiscType;

//            ICommonHandler commonHandler;
//            CTS140_ScreenParameter session;
//            CommonUtil comUtil = new CommonUtil();
//            ICommonContractHandler comContractHandler;

//            try
//            {
//                session = CTS140_GetImportData();
//                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
//                comContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;

//                //3.2.1 When dtpPeriodFrom and dtpPeriodTo is a required field
//                if (doCancelContractCondition.StartPeriodDate > doCancelContractCondition.EndPeriodDate)
//                {
//                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0101, null);
//                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0101, null, new string[] { "StartPeriodDateContract" });
//                    return;
//                }

//                //3.2.2	When ddlFeeType is one of the following items
//                //-	C_BILLING_TYPE_DEPOSIT_FEE
//                //-	C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE
//                //-	C_BILLING_TYPE_CANCEL_CONTRACT_FEE
//                //-	C_BILLING_TYPE_CHANGE_INSTALLATION_FEE
//                //-	C_BILLING_TYPE_CARD_FEE
//                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE ||
//                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE ||
//                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE ||
//                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE ||
//                    doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CARD_FEE)
//                {
//                    listDOCancelContractGrid = session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == doCancelContractCondition.BillingType; });
//                    if (listDOCancelContractGrid.Count() != 0)
//                    {
//                        //3.2.2.1 The adding fee type must not existing in ‘Cancel contract condition list’ Show Show message when not pass validation
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null, new string[] { "FeeType" });
//                        return;
//                    }
//                }

//                //3.2.3 When ddlFeeType = C_BILLING_TYPE_CONTRACT_FEE
//                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
//                {
//                    //Move to ValidateBusinessCancelContractForWarning_CTS140()
//                    ////3.2.3.1 The period (from - to) must not overlap with the registered contact fee in ‘Cancel contract condition list’
//                    ////if (ValidatePeriodverLap_CTS140(doCancelContractCondition.StartPeriodDate, doCancelContractCondition.EndPeriodDate) == true)
//                    //if (ValidatePeriodverLap_CTS140(doCancelContractCondition.StartPeriodDate, doCancelContractCondition.EndPeriodDate, doCancelContractCondition.BillingType))
//                    //{
//                    //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null);
//                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null, new string[] { "StartPeriodDateContract", "EndPeriodDateContract" });
//                    //    return;
//                    //}

//                    //3.2.3.2 Maintenanace fee must not registered in ‘Cancel contract condition list’
//                    if (session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE; }).Count() != 0)
//                    {
//                        string[] message = new string[] { doCancelContractCondition.C_ContractFeeValidate, doCancelContractCondition.C_MaintenanceFeeValidate };
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message, new string[] { "FeeType" });
//                        return;
//                    }
//                }
//                //3.2.4
//                else if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
//                {
//                    //Move to ValidateBusinessCancelContractForWarning_CTS140()
//                    ////if (ValidatePeriodverLap_CTS140(doCancelContractCondition.StartPeriodDate, doCancelContractCondition.EndPeriodDate) == true)
//                    //if (ValidatePeriodverLap_CTS140(doCancelContractCondition.StartPeriodDate, doCancelContractCondition.EndPeriodDate, doCancelContractCondition.BillingType))
//                    //{
//                    //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null);
//                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3121, null, new string[] { "StartPeriodDateContract", "EndPeriodDateContract" });
//                    //    return;
//                    //}

//                    if (session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE; }).Count() != 0)
//                    {
//                        string[] message = new string[] { doCancelContractCondition.C_MaintenanceFeeValidate, doCancelContractCondition.C_ContractFeeValidate };
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3123, message, new string[] { "FeeType" });
//                        return;
//                    }
//                }
//                else if (doCancelContractCondition.BillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_OTHER_FEE)
//                {
//                    //ไม่ได้อยู่ใน Spec นะครับเป็นการเช็คไม่ให้ Add ซ้ำครับ
//                    if (session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == doCancelContractCondition.BillingType && s.HandlingType == doCancelContractCondition.HandlingType; }).Count() != 0)
//                    {
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3112, null, new string[] { "FeeType", "HandlingType" });
//                        return;
//                    }
//                }

//                //3.2.5
//                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE)
//                {
//                    //if (session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; }).Count() == 0)
//                    if (session.DSRentalContract.dtTbt_CancelContractMemoDetail.FindAll(delegate(tbt_CancelContractMemoDetail s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; }).Count() == 0)
//                    {
//                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3122, null);
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3122, null, new string[] { "FeeType" });
//                        return;
//                    }
//                }

//                if (doCancelContractCondition.ContractCode_CounterBalance != null
//                    && doCancelContractCondition.ContractCode_CounterBalance != "none")
//                {
//                    if (String.IsNullOrEmpty(doCancelContractCondition.ContractCode_CounterBalance) == false)
//                    {
//                        string strContractCode_CounterBalanceLong = comUtil.ConvertContractCode(doCancelContractCondition.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_LONG);
//                        if (comContractHandler.IsContractExistInRentalOrSale(strContractCode_CounterBalanceLong) == false)
//                        {
//                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, doCancelContractCondition.ContractCode_CounterBalance);
//                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, new string[] { doCancelContractCondition.ContractCode_CounterBalance }, new string[] { "ContractCode_CounterBalance" });
//                            return;
//                        }

//                        //Bug report CT-148
//                        if (session.contractCode == strContractCode_CounterBalanceLong.ToUpper())
//                        {
//                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281);
//                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3281, null, new string[] { "ContractCode_CounterBalance" });
//                            return;
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }
//        }

//        /// <summary>
//        /// ValidateBusiness Cancel Contract (Warning)
//        /// </summary>
//        /// <param name="doCancelContractCondition"></param>
//        /// <returns></returns>
//        private void ValidateBusinessCancelContractForWarning_CTS140(ObjectResultData res, CTS140_DOCancelContractCondition doCancelContractCondition)
//        {
//            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;

//            try
//            {
//                if (doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
//                    || doCancelContractCondition.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE)
//                {
//                    if (ValidatePeriodverLap_CTS140(doCancelContractCondition.StartPeriodDate, doCancelContractCondition.EndPeriodDate, doCancelContractCondition.BillingType))
//                    {
//                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3282);
//                        return;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }
//        }

//        /// <summary>
//        /// Remove data from Maintenance grid when click [Remove] button in 'Maintenance target contract list' section
//        /// </summary>
//        /// <param name="doMaintenanceGrid"></param>
//        /// <returns></returns>
//        public ActionResult Remove_CTS140(CTS140_DOMaintenanceGrid doMaintenanceGrid)
//        {
//            ObjectResultData res = new ObjectResultData();
//            List<CTS140_DOMaintenanceGrid> listRelationType;
//            List<tbt_RelationType> listTbtRelationType;
//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();
//                listRelationType = session.ListDOMaintenanceGridEdit.FindAll(delegate(CTS140_DOMaintenanceGrid s) { return s.ContractCode == doMaintenanceGrid.ContractCode; });
//                foreach (var item in listRelationType)
//                {
//                    session.ListDOMaintenanceGridEdit.Remove(item);
//                }

//                listTbtRelationType = session.DSRentalContract.dtTbt_RelationType.FindAll(delegate(tbt_RelationType s) { return s.ContractCode == doMaintenanceGrid.ContractCode; });
//                foreach (var item in listTbtRelationType)
//                {
//                    session.DSRentalContract.dtTbt_RelationType.Remove(item);
//                }

//                CTS140_SetImportData(session);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        /// <summary>
//        /// Remove data from CancelContract grid when click [Remove] button in 'Cancel contract condition' section
//        /// </summary>
//        /// <param name="strSequence"></param>
//        /// <returns></returns>
//        public ActionResult RemoveCancel_CTS140(string strSequence) //(string billingType, string handlingType)
//        {
//            ObjectResultData res = new ObjectResultData();
//            List<CTS140_DOCancelContractGrid> listDOCancelContractGrid;
//            //List<tbt_CancelContractMemoDetail> listCancelContractMemoDetail;

//            CTS140_ScreenParameter session;

//            try
//            {
//                session = CTS140_GetImportData();

//                //listDOCancelContractGrid = session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.BillingType == billingType && s.HandlingType == handlingType; });
//                //listCancelContractMemoDetail = session.DSRentalContract.dtTbt_CancelContractMemoDetail.FindAll(delegate(tbt_CancelContractMemoDetail s) { return s.BillingType == billingType && s.HandlingType == handlingType; });
//                listDOCancelContractGrid = session.ListDOCancelContractGrid.FindAll(delegate(CTS140_DOCancelContractGrid s) { return s.Sequence == strSequence; });

//                foreach (var item in listDOCancelContractGrid)
//                {
//                    //session.ListDOCancelContractGridRemove.Add(item.ContractCode);
//                    session.ListDOCancelContractGrid.Remove(item);
//                }

//                //foreach (var item in listCancelContractMemoDetail)
//                //{
//                //    session.DSRentalContract.dtTbt_CancelContractMemoDetail.Remove(item);
//                //}

//                CTS140_SetImportData(session);
//            }
//            catch (Exception ex)
//            {
//                res.AddErrorMessage(ex);
//            }

//            return Json(res);
//        }

//        #endregion

//        #region Session

//        /// <summary>
//        /// Get import data from screen
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        private CTS140_ScreenParameter CTS140_GetImportData(string key = null)
//        {
//            try
//            {
//                return GetScreenObject<CTS140_ScreenParameter>(key);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        /// <summary>
//        /// Set import data to screen
//        /// </summary>
//        /// <param name="import"></param>
//        /// <param name="key"></param>
//        private void CTS140_SetImportData(CTS140_ScreenParameter import, string key = null)
//        {
//            try
//            {
//                UpdateScreenObject(import, key);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }

//        //public ActionResult CTS140_ClearSession()
//        //{
//        //    ObjectResultData res = new ObjectResultData();
//        //    try
//        //    {
//        //        UpdateScreenObject(null);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        res.AddErrorMessage(ex);
//        //    }

//        //    return Json(res);
//        //}

//        #endregion
    }
}
