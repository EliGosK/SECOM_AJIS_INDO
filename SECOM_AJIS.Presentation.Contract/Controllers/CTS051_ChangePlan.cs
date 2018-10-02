//*********************************
// Create by: Natthavat S.
// Create date: 14/APR/2012
// Update date: 14/APR/2012
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
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Installation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;
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
        public ActionResult CTS051_Authority(CTS051_ScreenParameter2 param)
        {
            ObjectResultData res = new ObjectResultData();
            IARHandler arhandler = ServiceContainer.GetService<IARHandler>() as IARHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                //if (String.IsNullOrEmpty(param.ContractCode) && !String.IsNullOrEmpty(CommonUtil.dsTransData.dtCommonSearch.ContractCode))
                //{
                //    param.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                //}
                if (String.IsNullOrEmpty(param.ContractCode) && param.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                        param.ContractCode = param.CommonSearch.ContractCode;
                }

                res = ValidateAuthority_CTS051(res, param);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS051_ScreenParameter2>("CTS051", param, res);
        }
        #endregion

        #region Action
        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS051")]
        public ActionResult CTS051()
        {
            CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();
            param.newItemList = new List<dtBillingTemp_SetItem>();
            param.updateItemList = new List<dtBillingTemp_SetItem>();
            param.deleteItemList = new List<dtBillingTemp_SetItem>();

            bool isP1 = true;

            #if !ROUND1
            isP1 = false;
            #endif

            ViewBag.IsP1 = isP1;

            return View();
        }

        /// <summary>
        /// Initial grid ChangePlanBillingTarget
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS051_InitGridChangePlanBillingTarget()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();

                res.ResultData = CommonUtil.ConvertToXml<CTS051_ChangePlanGrid>(null, GetGridXMLSchema_CTS051(param.ContractStatus, param.ProductType, param.StartType), CommonUtil.GRID_EMPTY_TYPE.VIEW); ;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Load data to grid ChangePlanBillingTarget
        /// </summary>
        /// <param name="IsDisplayAll"></param>
        /// <returns></returns>
        public ActionResult CTS051_LoadGridChangePlanBillingTarget(bool IsDisplayAll)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            List<CTS051_ChangePlanGrid> obj = new List<CTS051_ChangePlanGrid>();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            try
            {
                CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();

                var billingTempData = rentalhandler.GetBillingTempForChangePlan(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.ContractOCC);
                var billingTempDataCodeOnly = from a in billingTempData select new { a.BillingOCC, a.BillingClientCode, a.BillingOfficeCode, IsBillingBasic = String.IsNullOrEmpty(a.CreateBy) };
                var billingTempGroup = billingTempDataCodeOnly.Distinct();

                foreach (var billingTempItem in billingTempGroup)
                {
                    //var billingClient = billinghandler.GetBillingBasicAsBillingTemp(billingTempItem.BillingTargetCode, billingTempItem.BillingOCC);
                    var billingClient = (from a in billingTempData
                                         where (a.BillingOfficeCode == billingTempItem.BillingOfficeCode)
                                             && (a.BillingOCC == billingTempItem.BillingOCC)
                                             && (a.BillingClientCode == billingTempItem.BillingClientCode)
                                         select a).ToList();
                    bool isShow = false;

                    // Filter Item
                    if (IsDisplayAll || (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                        ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
                    {
                        // Show
                        isShow = true;
                    }
                    else if (!IsDisplayAll && (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING))
                    {
                        var billingClientContractFee = billingTempData.Where(x => (billingTempItem.IsBillingBasic) && (x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE) && (x.BillingAmt.GetValueOrDefault() > 0));
                        if (billingClientContractFee.Count() > 0)
                        {
                            isShow = true;
                        }
                        else
                        {
                            if (param.InstallationCompleteFlag == FlagType.C_FLAG_OFF)
                            {
                                if (!String.IsNullOrEmpty(billingTempItem.BillingOCC))
                                {
                                    isShow = true;
                                }
                                else if (billingClient[0].OCC == param.ContractOCC)
                                {
                                    isShow = true;
                                }
                            }
                        }
                    }

                    // Filter delete item
                    var deleteItemFilter = param.deleteItemList.Where(x =>
                        (x.OriginalBillingClientCode == billingTempItem.BillingClientCode)
                            //&& (util.ConvertBillingTargetCode(x.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) == billingTempItem.BillingTargetCode)
                        && (x.OriginalBillingOfficeCode == billingTempItem.BillingOfficeCode)
                        && (x.OriginalBillingOCC == billingTempItem.BillingOCC));

                    if (isShow && ((deleteItemFilter.Count() > 0) && (String.IsNullOrEmpty(billingTempItem.BillingOCC))))
                    {
                        isShow = false;
                    }

                    if (isShow)
                    {
                        var itemInUpdateLst = from a in param.updateItemList
                                              where
                                                  //(util.ConvertBillingTargetCode(a.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) == billingTempItem.BillingTargetCode)
                                                  (a.OriginalBillingOfficeCode == billingTempItem.BillingOfficeCode)
                                                  //&& (util.ConvertBillingClientCode(a.OriginalBillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) == billingTempItem.BillingClientCode)
                                                  && (a.OriginalBillingClientCode == billingTempItem.BillingClientCode)
                                                  //&& (a.OriginalBillingOCC == billingClient[0].OCC)
                                                  && (a.OriginalBillingOCC == billingTempItem.BillingOCC)
                                              select a;

                        if ((itemInUpdateLst != null) && (itemInUpdateLst.Count() == 1))
                        {
                            // Use item in update list
                            obj.Add(CreateChangePlanObject_CTS051(itemInUpdateLst.First()));
                        }
                        else
                        {
                            // Use original item
                            obj.Add(CreateChangePlanObject_CTS051(billingClient));
                        }
                    }
                }

                List<CTS051_ChangePlanGrid> sortedList = new List<CTS051_ChangePlanGrid>();

                sortedList = obj.Where(x => !String.IsNullOrEmpty(x.BillingOCC)).OrderBy(x => x.BillingOCC).ToList();
                sortedList.AddRange(obj.Where(x => String.IsNullOrEmpty(x.BillingOCC)));

                // Concat with new item
                foreach (var newItem in param.newItemList)
                {
                    sortedList.Add(CreateChangePlanObject_CTS051(newItem));
                }

                res.ResultData = CommonUtil.ConvertToXml<CTS051_ChangePlanGrid>(sortedList, GetGridXMLSchema_CTS051(param.ContractStatus, param.ProductType, param.StartType), CommonUtil.GRID_EMPTY_TYPE.VIEW); ;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        
        /// <summary>
        /// Load data of Contract
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS051_LoadContractData()
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IInstallationHandler installhandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

            try
            {
                CTS051_DisplayContract obj = new CTS051_DisplayContract();
                CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();
                
                var contractData = rentalhandler.GetTbt_RentalContractBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                if ((contractData != null) && (contractData.Count == 1))
                {
                    List<string> miscTypeLst = new List<string>();
                    miscTypeLst.Add(MiscType.C_INSTALL_STATUS);

                    string installStatus = installhandler.GetInstallationStatus(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                    var miscData = commonhandler.GetMiscTypeCodeListByFieldName(miscTypeLst);
                    var securData = rentalhandler.GetTbt_RentalSecurityBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                    var custData = custhandler.GetCustomer(contractData[0].ContractTargetCustCode);
                    var siteData = sitehandler.GetSite(contractData[0].SiteCode, null);
                    var officeData = officehandler.GetTbm_Office(contractData[0].OperationOfficeCode);
                    var installData = from a in miscData where a.ValueCode == installStatus select a;

                    CommonUtil.MappingObjectLanguage<tbm_Office>(officeData);

                    if ((securData != null) && (securData.Count > 0) && (officeData != null) && (officeData.Count == 1) && (custData != null) && (custData.Count == 1) && (siteData != null) && (siteData.Count == 1))
                    {
                        //param.QuotationCode = securData[0].QuotationTargetCode;
                        param.QuotationCode = contractData[0].ContractCode;

                        //param.ContractOCC = contractData[0].LastOCC;
                        param.Alphabet = "";
                        param.ContractStatus = "";
                        param.ProductType = "";
                        param.InstallationCompleteFlag = false;
                        param.StartType = contractData[0].StartType;

                        obj = new CTS051_DisplayContract()
                        {
                            ContractCode = contractData[0].ContractCodeShort,
                            CustAddressEN = custData[0].AddressFullEN,
                            CustAddressLC = custData[0].AddressFullLC,
                            CustCode = custData[0].CustCodeShort,
                            CustNameEN = custData[0].CustFullNameEN,
                            CustNameLC = custData[0].CustFullNameLC,
                            EndUserCode = util.ConvertCustCode(contractData[0].RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                            IsImportantCustomer = custData[0].ImportantFlag.GetValueOrDefault(),
                            //QuotationCode = securData[0].QuotationTargetCodeShort,
                            QuotationCode = contractData[0].ContractCodeShort,
                            SiteAddressEN = siteData[0].AddressFullEN,
                            SiteAddressLC = siteData[0].AddressFullLC,
                            SiteCode = siteData[0].SiteCodeShort,
                            SiteNameEN = siteData[0].SiteNameEN,
                            SiteNameLC = siteData[0].SiteNameLC,
                            UserCode = contractData[0].UserCode,
                            OperationOffice = CommonUtil.TextCodeName(officeData[0].OfficeCode, officeData[0].OfficeName),
                            InstallationStatus = (installData.Count() > 0) ? installData.First().ValueCodeDisplay : String.Empty,
                            CanViewInstallStatus = (installStatus != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                        };
                    }
                    else
                    {
                        throw new Exception("Cannot load some data");
                    }

                }


                res.ResultData = obj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Load data of Quotation
        /// </summary>
        /// <param name="QuotationCode"></param>
        /// <param name="Alphabet"></param>
        /// <returns></returns>
        public ActionResult CTS051_LoadQuotationData(string QuotationCode, string Alphabet)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            IQuotationHandler quotehandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            IInstrumentMasterHandler instruhandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
            IEmployeeMasterHandler employeehandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                CTS051_DisplayChangePlan obj = new CTS051_DisplayChangePlan();
                CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();

                if (String.IsNullOrEmpty(QuotationCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS051", "lblQuotationCode") }, new string[] { "QuotationCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                if (String.IsNullOrEmpty(Alphabet))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS051", "lblAlphabet") }, new string[] { "Alphabet" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                var contractData = rentalhandler.GetTbt_RentalContractBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                var securData = rentalhandler.GetTbt_RentalSecurityBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                var quotationData = quotehandler.GetQuotationData(new doGetQuotationDataCondition()
                    {
                        Alphabet = Alphabet,
                        QuotationTargetCode = param.QuotationCode,
                        ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL,
                        TargetCodeTypeCode = TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE,
                        ContractFlag = true,
                    });

                if (quotationData != null)
                {
                    if (quotationData.dtTbt_QuotationBasic.LastOccNo != contractData[0].LastOCC)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }

                    if (quotationData.dtTbt_QuotationMaintenanceLinkage.Count > 0)
                    {
                        var linkageData = rentalhandler.GetTbt_RentalContractBasic(quotationData.dtTbt_QuotationMaintenanceLinkage[0].ContractCode, null);
                        if ((linkageData != null) && (linkageData.Count > 0))
                        {
                            if ((linkageData[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA) && (linkageData[0].ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL) && (quotationData.dtTbt_QuotationMaintenanceLinkage.Count > 1))
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, null, new string[] { "Alphabet" });
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                return Json(res);
                            }

                            if (linkageData[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                            {
                                var lastUpdateLinkage = rentalhandler.GetMaxUpdateDateOfMATargetContract(linkageData[0].ContractCode, linkageData[0].LastOCC);
                                if ((lastUpdateLinkage != null) && (lastUpdateLinkage.Count > 0))
                                {
                                    if (quotationData.dtTbt_QuotationBasic.CreateDate <= lastUpdateLinkage[0])
                                    {
                                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, null, new string[] { "Alphabet" });
                                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                        return Json(res);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var instruItem in quotationData.dtTbt_QuotationInstrumentDetails)
                    {
                        var instruData = instruhandler.GetTbm_Instrument(instruItem.InstrumentCode);
                        if ((instruData != null) && (instruData.Count > 0))
                        {
                            if ((instruData[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE) || (instruData[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE))
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3038, null, null);
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG;
                                //return Json(res);
                            }
                        }
                    }

                    var negotiateEmp1 = employeehandler.GetTbm_Employee(securData[0].NegotiationStaffEmpNo1);
                    var negotiateEmp2 = employeehandler.GetTbm_Employee(securData[0].NegotiationStaffEmpNo2);

                    CommonUtil.MappingObjectLanguage<tbm_Employee>(negotiateEmp1);
                    CommonUtil.MappingObjectLanguage<tbm_Employee>(negotiateEmp2);

                    param.Alphabet = quotationData.dtTbt_QuotationBasic.Alphabet;
                    param.ContractStatus = contractData[0].ContractStatus;
                    param.ProductType = contractData[0].ProductTypeCode;
                    param.InstallationCompleteFlag = securData[0].InstallationCompleteFlag.GetValueOrDefault();
                    param.deleteItemList = new List<dtBillingTemp_SetItem>();
                    param.newItemList = new List<dtBillingTemp_SetItem>();
                    param.updateItemList = new List<dtBillingTemp_SetItem>();
                    //param.IsContractApprove = (contractData[0].FirstInstallCompleteFlag.GetValueOrDefault() == FlagType.C_FLAG_OFF) && (securData[0].DepositFeeBillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT);
                    param.IsContractApprove = false;

                    bool isInitStartServiceFee = true, isEnableInstallationFeeAfterChange = true;
                    
                    List<string> disableFeeList = new List<string>();
                    disableFeeList.Add("ChangePlanNormalContractFee");
                    disableFeeList.Add("ChangePlanNormalContractFeeCurrencyType");

                    disableFeeList.Add("ChangePlanNormalInstallationFee");
                    disableFeeList.Add("ChangePlanNormalInstallationFeeCurrencyType");

                    disableFeeList.Add("ChangePlanApproveInstallationFee");
                    disableFeeList.Add("ChangePlanApproveInstallationFeeCurrencyType");

                    //disableFeeList.Add("ChangePlanNormalDepositFee");

                    if ((((contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_SALE)
                        || (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                        || (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                        || (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA))

                        && (securData[0].DepositFeeBillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT))
                        
                        ||

                        (((contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                        || (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
                        
                        &&

                        (contractData[0].FirstInstallCompleteFlag.GetValueOrDefault() == FlagType.C_FLAG_OFF)
                        
                        &&

                        (securData[0].DepositFeeBillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)))
                    {
                        param.IsContractApprove = true;
                    }

                    if ((contractData[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                        (contractData[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (contractData[0].StartType == StartType.C_START_TYPE_ALTER_START))
                    {
                        if ((contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL) || (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
                        {
                            
                        }
                        else if (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                        {
                            isEnableInstallationFeeAfterChange = false;
                            disableFeeList.Add("ChangePlanOrderInstallationFee");
                            disableFeeList.Add("ChangePlanOrderInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanCompleteInstallationFee");
                            disableFeeList.Add("ChangePlanCompleteInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanStartInstallationFee");
                            disableFeeList.Add("ChangePlanStartInstallationFeeCurrencyType");
                        }
                        else if (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                        {
                            isEnableInstallationFeeAfterChange = false;
                            disableFeeList.Add("ChangePlanOrderInstallationFee");
                            disableFeeList.Add("ChangePlanOrderInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanCompleteInstallationFee");
                            disableFeeList.Add("ChangePlanCompleteInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanStartInstallationFee");
                            disableFeeList.Add("ChangePlanStartInstallationFeeCurrencyType");
                            //disableFeeList.Add("ChangePlanOrderDepositFee");
                            //disableFeeList.Add("BillingTimingType");
                        }
                        else if ((contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                            || (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE))
                        {
                            disableFeeList.Add("ChangePlanOrderInstallationFee");
                            disableFeeList.Add("ChangePlanOrderInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanCompleteInstallationFee");
                            disableFeeList.Add("ChangePlanCompleteInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanStartInstallationFee");
                            disableFeeList.Add("ChangePlanStartInstallationFeeCurrencyType");
                            //disableFeeList.Add("ChangePlanOrderDepositFee");
                            //disableFeeList.Add("BillingTimingType");
                        }
                    }
                    else if (contractData[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                        || contractData[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
                    )
                    {
                        if ((contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL) || (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
                        {
                            disableFeeList.Add("ChangePlanStartInstallationFee");
                            disableFeeList.Add("ChangePlanStartInstallationFeeCurrencyType");
                            isInitStartServiceFee = false;
                        }
                        else if (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                        {
                            isEnableInstallationFeeAfterChange = false;
                            disableFeeList.Add("ChangePlanOrderInstallationFee");
                            disableFeeList.Add("ChangePlanOrderInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanCompleteInstallationFee");
                            disableFeeList.Add("ChangePlanCompleteInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanStartInstallationFee");
                            disableFeeList.Add("ChangePlanStartInstallationFeeCurrencyType");
                        }
                        else if (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                        {
                            isEnableInstallationFeeAfterChange = false;
                            disableFeeList.Add("ChangePlanOrderInstallationFee");
                            disableFeeList.Add("ChangePlanOrderInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanCompleteInstallationFee");
                            disableFeeList.Add("ChangePlanCompleteInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanStartInstallationFee");
                            disableFeeList.Add("ChangePlanStartInstallationFeeCurrencyType");
                            //disableFeeList.Add("ChangePlanOrderDepositFee");
                            //disableFeeList.Add("BillingTimingType");
                        }
                        else if ((contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                            || (contractData[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE))
                        {
                            isEnableInstallationFeeAfterChange = false;
                            disableFeeList.Add("ChangePlanOrderInstallationFee");
                            disableFeeList.Add("ChangePlanOrderInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanCompleteInstallationFee");
                            disableFeeList.Add("ChangePlanCompleteInstallationFeeCurrencyType");
                            disableFeeList.Add("ChangePlanStartInstallationFee");
                            disableFeeList.Add("ChangePlanStartInstallationFeeCurrencyType");
                            //disableFeeList.Add("ChangePlanOrderDepositFee");
                            //disableFeeList.Add("BillingTimingType");
                        }
                    }

                    if (param.IsContractApprove)
                    {
                        disableFeeList.Add("ChangePlanOrderDepositFee");
                    }

                    obj = new CTS051_DisplayChangePlan()
                    {
                        Alphabet = quotationData.dtTbt_QuotationBasic.Alphabet,
                        ApproveNo1 = quotationData.dtTbt_QuotationBasic.ApproveNo1,
                        ApproveNo2 = quotationData.dtTbt_QuotationBasic.ApproveNo2,
                        ApproveNo3 = quotationData.dtTbt_QuotationBasic.ApproveNo3,
                        ApproveNo4 = quotationData.dtTbt_QuotationBasic.ApproveNo4,
                        ApproveNo5 = quotationData.dtTbt_QuotationBasic.ApproveNo5,
                        AutoRenewMonth = quotationData.dtTbt_QuotationBasic.AutoRenewMonth.GetValueOrDefault().ToString(),
                        BillingTimingType = (!param.IsContractApprove) ? securData[0].DepositFeeBillingTiming : BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT,
                        NegotiationStaffEmpNo1 = securData[0].NegotiationStaffEmpNo1,
                        NegotiationStaffEmpNo2 = securData[0].NegotiationStaffEmpNo2,
                        NegotiationStaffEmpName1 = ((!String.IsNullOrEmpty(securData[0].NegotiationStaffEmpNo1)) && (negotiateEmp1.Count > 0)) ? negotiateEmp1[0].EmpFullName : "",
                        NegotiationStaffEmpName2 = ((!String.IsNullOrEmpty(securData[0].NegotiationStaffEmpNo2)) && (negotiateEmp2.Count > 0)) ? negotiateEmp2[0].EmpFullName : "",
                        EndContractDate = CommonUtil.TextDate(securData[0].ContractEndDate),
                        //ExpectedOperationDate = CommonUtil.TextDate(securData[0].ExpectedOperationDate),
                        ContractDurationFlag = false,
                        ContractDurationMonth = securData[0].ContractDurationMonth.GetValueOrDefault().ToString(),
                        QuotationCode = util.ConvertQuotationTargetCode(param.QuotationCode, CommonUtil.CONVERT_TYPE.TO_SHORT),

                        //ChangePlanNormalContractFee = CommonUtil.TextNumeric(quotationData.dtTbt_QuotationBasic.ContractFee),
                        //ChangePlanOrderContractFee = "",
                        //ChangePlanNormalInstallationFee = CommonUtil.TextNumeric(quotationData.dtTbt_QuotationBasic.InstallationFee.GetValueOrDefault()),
                        //ChangePlanOrderInstallationFee = (isEnableInstallationFeeAfterChange) ? "" : CommonUtil.TextNumeric(0.00),
                        ////ChangePlanApproveInstallationFee = CommonUtil.TextNumeric(securData[0].OrderInstallFee_ApproveContract),
                        //ChangePlanApproveInstallationFee = (contractData[0].FirstInstallCompleteFlag.GetValueOrDefault()) ? "" : CommonUtil.TextNumeric(securData[0].OrderInstallFee_ApproveContract),
                        //ChangePlanCompleteInstallationFee = (securData[0].InstallationCompleteFlag.GetValueOrDefault()) ? "" : CommonUtil.TextNumeric(securData[0].OrderInstallFee_CompleteInstall),
                        //ChangePlanStartInstallationFee = (securData[0].InstallationCompleteFlag.GetValueOrDefault()) ? "" : ((isInitStartServiceFee) ? CommonUtil.TextNumeric(securData[0].OrderInstallFee_StartService) : String.Empty),
                        DisableList = disableFeeList,
                        NeedFixTiming = param.IsContractApprove
                    };

                    #region Change Plan Normal Contract Fee

                    obj.ChangePlanNormalContractFeeCurrencyType = quotationData.dtTbt_QuotationBasic.ContractFeeCurrencyType;
                     
                    if (quotationData.dtTbt_QuotationBasic.ContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ChangePlanNormalContractFee = CommonUtil.TextNumeric(quotationData.dtTbt_QuotationBasic.ContractFeeUsd);
                    else
                        obj.ChangePlanNormalContractFee = CommonUtil.TextNumeric(quotationData.dtTbt_QuotationBasic.ContractFee);

                    #endregion
                    #region Change Plan Order Contract Fee

                    //obj.ChangePlanOrderContractFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    //obj.ChangePlanOrderContractFee = "";

                    obj.ChangePlanOrderContractFeeCurrencyType = contractData[0].LastOrderContractFeeCurrencyType;
                    if (contractData[0].LastOrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ChangePlanOrderContractFee = CommonUtil.TextNumeric(contractData[0].LastOrderContractFeeUsd);
                    else
                        obj.ChangePlanOrderContractFee = CommonUtil.TextNumeric(contractData[0].LastOrderContractFee);

                    #endregion
                    #region Change Plan Normal Installation Fee

                    obj.ChangePlanNormalInstallationFeeCurrencyType = quotationData.dtTbt_QuotationBasic.InstallationFeeCurrencyType;

                    if (quotationData.dtTbt_QuotationBasic.InstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ChangePlanNormalInstallationFee = CommonUtil.TextNumeric(quotationData.dtTbt_QuotationBasic.InstallationFeeUsd.GetValueOrDefault());
                    else
                        obj.ChangePlanNormalInstallationFee = CommonUtil.TextNumeric(quotationData.dtTbt_QuotationBasic.InstallationFee.GetValueOrDefault());

                    #endregion
                    #region Change Plan Order Installation Fee
                    obj.ChangePlanOrderInstallationFeeCurrencyType = securData[0].OrderInstallFeeCurrencyType;

                    if (securData[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ChangePlanOrderInstallationFee = CommonUtil.TextNumeric(securData[0].OrderInstallFeeUsd);
                    else
                        obj.ChangePlanOrderInstallationFee = CommonUtil.TextNumeric(securData[0].OrderInstallFee);

                    //obj.ChangePlanOrderInstallationFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    //obj.ChangePlanOrderInstallationFee = (isEnableInstallationFeeAfterChange) ? "" : CommonUtil.TextNumeric(0.00);

                    #endregion
                    #region Change Plan Approve Installation Fee

                    obj.ChangePlanApproveInstallationFeeCurrencyType = securData[0].OrderInstallFee_ApproveContractCurrencyType;

                    if (securData[0].OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ChangePlanApproveInstallationFee = (contractData[0].FirstInstallCompleteFlag.GetValueOrDefault()) ? "" : CommonUtil.TextNumeric(securData[0].OrderInstallFee_ApproveContractUsd);
                    else
                        obj.ChangePlanApproveInstallationFee = (contractData[0].FirstInstallCompleteFlag.GetValueOrDefault()) ? "" : CommonUtil.TextNumeric(securData[0].OrderInstallFee_ApproveContract);

                    #endregion
                    #region Change Plan Complete Installtion Fee

                    obj.ChangePlanCompleteInstallationFeeCurrencyType = securData[0].OrderInstallFee_CompleteInstallCurrencyType;

                    if (securData[0].OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ChangePlanCompleteInstallationFee = (securData[0].InstallationCompleteFlag.GetValueOrDefault()) ? "" : CommonUtil.TextNumeric(securData[0].OrderInstallFee_CompleteInstallUsd);
                    else
                        obj.ChangePlanCompleteInstallationFee = (securData[0].InstallationCompleteFlag.GetValueOrDefault()) ? "" : CommonUtil.TextNumeric(securData[0].OrderInstallFee_CompleteInstall);

                    #endregion
                    #region Change Plan Start Installtion Fee

                    obj.ChangePlanStartInstallationFeeCurrencyType = securData[0].OrderInstallFee_StartServiceCurrencyType;

                    if (securData[0].OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        obj.ChangePlanStartInstallationFee = (securData[0].InstallationCompleteFlag.GetValueOrDefault()) ? "" : ((isInitStartServiceFee) ? CommonUtil.TextNumeric(securData[0].OrderInstallFee_StartServiceUsd) : String.Empty);
                    else
                        obj.ChangePlanStartInstallationFee = (securData[0].InstallationCompleteFlag.GetValueOrDefault()) ? "" : ((isInitStartServiceFee) ? CommonUtil.TextNumeric(securData[0].OrderInstallFee_StartService) : String.Empty);
                    
                    #endregion
                    
                    // Modify 2012-08-30 Phoomsak L. remove condition and set ExpectedOperationDate = ExpectedOperationDate on screen always
                    //obj.ExpectedOperationDate = CommonUtil.TextDate(securData[0].ExpectedOperationDate);

                    //if ((contractData[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                    //        ((contractData[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (contractData[0].StartType == StartType.C_START_TYPE_ALTER_START)))
                    //{
                    //    obj.ExpectedOperationDate = CommonUtil.TextDate(securData[0].ExpectedOperationDate);
                    //}
                    //else if (contractData[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                    //    || contractData[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                    //{
                    //    obj.ExpectedOperationDate = CommonUtil.TextDate(securData[0].ExpectedInstallationCompleteDate);
                    //}

                    // retrieve billing temp data for test error
                    var billingTempData = rentalhandler.GetBillingTempForChangePlan(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.ContractOCC);

                    res.ResultData = obj;
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0137, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Load data of BillingTemp
        /// </summary>
        /// <param name="editItem"></param>
        /// <returns></returns>
        public ActionResult CTS051_LoadBillingTempData(dtBillingTemp_SetItem editItem)
        {
            ObjectResultData res = new ObjectResultData();
            CTS051_DisplayBillingTargetDetail obj = new CTS051_DisplayBillingTargetDetail();
            CommonUtil util = new CommonUtil();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            IBillingClientMasterHandler billingclienthandler = ServiceContainer.GetService<IBillingClientMasterHandler>() as IBillingClientMasterHandler;
            IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            IBillingMasterHandler billingmasterhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

            try
            {
                CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();

                List<string> hideObjList = new List<string>();
                List<string> disableObjList = new List<string>();
                bool IsDisableDeposit = false;

                if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                    ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
                {
                    if ((param.ProductType == ProductType.C_PROD_TYPE_AL) || (param.ProductType == ProductType.C_PROD_TYPE_RENTAL_SALE))
                    {
                        hideObjList.Add("trInstallFee");
                        disableObjList.Add("BillingApproveInstallationFee");
                        disableObjList.Add("BillingApproveInstallationFeeCurrencyType");
                        //disableObjList.Add("AmountTotal");
                        //disableObjList.Add("AmountTotalUsd");
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_ONLINE)
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_MA)
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                        //hideObjList.Add("trDepositFee");
                    }
                    else if ((param.ProductType == ProductType.C_PROD_TYPE_SG) || (param.ProductType == ProductType.C_PROD_TYPE_BE))
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                        //hideObjList.Add("trDepositFee");
                    }
                }
                else if (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                    || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                {
                    if ((param.ProductType == ProductType.C_PROD_TYPE_RENTAL_SALE) || (param.ProductType == ProductType.C_PROD_TYPE_AL))
                    {
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_ONLINE)
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_MA)
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                        //hideObjList.Add("trDepositFee");
                    }
                    else if ((param.ProductType == ProductType.C_PROD_TYPE_SG) || (param.ProductType == ProductType.C_PROD_TYPE_BE))
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                        //hideObjList.Add("trDepositFee");
                    }
                }

                int billingSequenceNo = 0;
                string finalOfficeCode = "";

                if (!String.IsNullOrEmpty(editItem.UID) && !editItem.IsNew)
                {
                    // Load data from grid
                    dtBillingClientData clientObj = null;
                    var billingTempData = rentalhandler.GetBillingTempForChangePlan(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.ContractOCC);
                    var billingTempTargetData = from a in billingTempData
                                                where
                                                    (a.BillingTargetCode == util.ConvertBillingTargetCode(editItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG))
                                                    && (a.BillingClientCode == util.ConvertBillingClientCode(editItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG))
                                                    && (a.BillingOCC == editItem.BillingOCC)
                                                select a;

                    string objectOfficeCode = "";
                    string contractFee = null
                            , contractFeeCurrency = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                            , installationFee = null
                            , installationFeeCurrency = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                            , installationPayMethod = null
                            , approvalInstallationFee = null
                            , approvalInstallationFeeCurrency = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                            , completeInstallationFee = null
                            , completeInstallationFeeCurrency = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                            , completeInstallationPayMethod = null
                            , startServiceInstallationFee = null
                            , startServiceInstallationFeeCurrency = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                            , startServiceInstallationPayMethod = null;

                    //bool isDepositFeeApprove = false;

                    if ((billingTempTargetData != null) && (billingTempTargetData.Count() > 0))
                    {
                        var currentBillingTemp = billingTempTargetData.First();
                        billingSequenceNo = currentBillingTemp.SequenceNo;
                        if (!String.IsNullOrEmpty(editItem.BillingTargetCode))
                        {
                            var billingTargetObj = billinghandler.GetBillingTarget(util.ConvertBillingTargetCode(editItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                            if ((billingTargetObj != null) && (billingTargetObj.Count > 0))
                            {
                                objectOfficeCode = billingTargetObj[0].BillingOfficeCode;
                                var billingClientObj = billingmasterhandler.GetBillingClient(billingTargetObj[0].BillingClientCode);
                                //var billingClientObj = billingclienthandler.GetTbm_BillingClient(billingTargetObj[0].BillingClientCode);
                                if ((billingClientObj != null) && (billingClientObj.Count == 1))
                                {
                                    clientObj = billingClientObj[0];
                                }
                            }
                            else if (!String.IsNullOrEmpty(editItem.BillingClientCode))
                            {
                                objectOfficeCode = currentBillingTemp.BillingOfficeCode;
                                var billingClientObj = billingmasterhandler.GetBillingClient(util.ConvertBillingClientCode(editItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                                //var billingClientObj = billingclienthandler.GetTbm_BillingClient(util.ConvertBillingClientCode(BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                                if ((billingClientObj != null) && (billingClientObj.Count > 0))
                                {
                                    clientObj = billingClientObj[0];
                                }
                            }
                        }
                        else if (!String.IsNullOrEmpty(editItem.BillingClientCode))
                        {
                            objectOfficeCode = currentBillingTemp.BillingOfficeCode;
                            var billingClientObj = billingmasterhandler.GetBillingClient(util.ConvertBillingClientCode(editItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                            //var billingClientObj = billingclienthandler.GetTbm_BillingClient(util.ConvertBillingClientCode(BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                            if ((billingClientObj != null) && (billingClientObj.Count > 0))
                            {
                                clientObj = billingClientObj[0];
                            }
                        }

                        if (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                            || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
                         )
                        {
                            var contractFeeBillingTemp = from a in billingTempTargetData where a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE select a;
                            if (contractFeeBillingTemp.Count() != 0)
                            {
                                dtBillingTempChangePlanData temp = contractFeeBillingTemp.First();

                                contractFeeCurrency = temp.BillingAmtCurrencyType;
                                if (temp.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                    contractFee = CommonUtil.TextNumeric(temp.BillingAmtUsd).Replace(",", "");
                                else
                                    contractFee = CommonUtil.TextNumeric(temp.BillingAmt).Replace(",", "");

                                if (String.IsNullOrEmpty(contractFee))
                                    contractFee = null;
                            }

                            var installationBillingTemp = from a in billingTempTargetData where (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE) select a;
                            if (installationBillingTemp.Count() != 0)
                            {
                                var tmpItem = installationBillingTemp.First();

                                installationFeeCurrency = tmpItem.BillingAmtCurrencyType;
                                if (tmpItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                    installationFee = CommonUtil.TextNumeric(tmpItem.BillingAmtUsd).Replace(",", "");
                                else
                                    installationFee = CommonUtil.TextNumeric(tmpItem.BillingAmt).Replace(",", "");

                                installationPayMethod = tmpItem.PayMethod;
                            }
                        }
                        else if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                            ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
                        {
                            var contractFeeBillingTemp = from a in billingTempTargetData where a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE select a;
                            if (contractFeeBillingTemp.Count() != 0)
                            {
                                dtBillingTempChangePlanData temp = contractFeeBillingTemp.First();

                                contractFeeCurrency = temp.BillingAmtCurrencyType;
                                if (temp.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                    contractFee = CommonUtil.TextNumeric(temp.BillingAmtUsd).Replace(",", "");
                                else
                                    contractFee = CommonUtil.TextNumeric(temp.BillingAmt).Replace(",", "");
                            }

                            var approvalInstallationBillingTemp = from a in billingTempTargetData where (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                                                                  && a.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT select a;
                            if (approvalInstallationBillingTemp.Count() != 0)
                            {
                                dtBillingTempChangePlanData temp = approvalInstallationBillingTemp.First();

                                approvalInstallationFeeCurrency = temp.BillingAmtCurrencyType;
                                if (temp.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                    approvalInstallationFee = CommonUtil.TextNumeric(temp.BillingAmtUsd).Replace(",", "");
                                else
                                    approvalInstallationFee = CommonUtil.TextNumeric(temp.BillingAmt).Replace(",", "");
                            }

                            var completeInstallationBillingTemp = from a in billingTempTargetData
                                                                  where (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                                                                      && a.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION
                                                                  select a;
                            if (completeInstallationBillingTemp.Count() != 0)
                            {
                                dtBillingTempChangePlanData temp = completeInstallationBillingTemp.First();

                                completeInstallationFeeCurrency = temp.BillingAmtCurrencyType;
                                if (temp.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                    completeInstallationFee = CommonUtil.TextNumeric(temp.BillingAmtUsd).Replace(",", "");
                                else
                                    completeInstallationFee = CommonUtil.TextNumeric(temp.BillingAmt).Replace(",", "");

                                completeInstallationPayMethod = temp.PayMethod;
                            }

                            var startServiceInstallationBillingTemp = from a in billingTempTargetData
                                                                  where (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                                                                      && a.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE
                                                                  select a;
                            if (startServiceInstallationBillingTemp.Count() != 0)
                            {
                                dtBillingTempChangePlanData temp = startServiceInstallationBillingTemp.First();

                                startServiceInstallationFeeCurrency = temp.BillingAmtCurrencyType;
                                if (temp.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                    startServiceInstallationFee = CommonUtil.TextNumeric(temp.BillingAmtUsd).Replace(",", "");
                                else
                                    startServiceInstallationFee = CommonUtil.TextNumeric(temp.BillingAmt).Replace(",", "");

                                startServiceInstallationPayMethod = temp.PayMethod;
                            }
                        }
                    }

                    if (clientObj != null)
                    {
                        //var tmpOfficeLst = officehandler.GetTbm_Office(objectOfficeCode);
                        //finalOfficeCode = (!String.IsNullOrEmpty(editItem.BillingOfficeCode)) ? editItem.BillingOfficeCode : objectOfficeCode;
                        obj = new CTS051_DisplayBillingTargetDetail()
                        {
                            BillingClientCode = editItem.BillingClientCode,
                            BillingTargetCode = editItem.BillingTargetCode,
                            FullNameLC = clientObj.FullNameLC,
                            FullNameEN = clientObj.FullNameEN,
                            AddressEN = clientObj.AddressEN,
                            AddressLC = clientObj.AddressLC,
                            IDNo = clientObj.IDNo,
                            BranchNameEN = clientObj.BranchNameEN,
                            BranchNameLC = clientObj.BranchNameLC,
                            PhoneNo = clientObj.PhoneNo,
                            BillingOffice = objectOfficeCode,
                            BusinessTypeCode = clientObj.BusinessTypeCode,
                            BusinessType = clientObj.BusinessTypeName,
                            Nationality = clientObj.Nationality,
                            RegionCode = clientObj.RegionCode,
                            IsBefore = ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) || ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START))),
                            DisableList = disableObjList,
                            HideList = hideObjList,
                            BillingOCC = editItem.BillingOCC,
                            CustomerType = clientObj.CustTypeCode,
                            ObjectType = 0, //Default
                            UID = editItem.UID,
                            OriginalBillingClientCode = clientObj.BillingClientCode,
                            OriginalBillingOCC = editItem.BillingOCC,
                            OriginalBillingOfficeCode = editItem.BillingOfficeCode,
                            BillingContractFee = contractFee,
                            BillingContractFeeCurrencyType = contractFeeCurrency,
                            BillingApproveInstallationFee = approvalInstallationFee,
                            BillingApproveInstallationFeeCurrencyType = approvalInstallationFeeCurrency,
                            BillingCompleteInstallationFee = completeInstallationFee,
                            BillingCompleteInstallationFeeCurrencyType = completeInstallationFeeCurrency,
                            BillingInstallationFee = installationFee,
                            BillingInstallationFeeCurrencyType = installationFeeCurrency,
                            BillingStartInstallationFee = startServiceInstallationFee,
                            BillingStartInstallationFeeCurrencyType = startServiceInstallationFeeCurrency,
                            PaymentCompleteInstallationFee = completeInstallationPayMethod,
                            PaymentInstallationFee = installationPayMethod,
                            PaymentStartInstallationFee = startServiceInstallationPayMethod,
                            HasFee = true
                            //SequenceNo = billingSequenceNo,
                        };

                        var checkUpdateItem = from a in param.updateItemList
                                              where
                                                  (a.OriginalBillingClientCode == util.ConvertBillingClientCode(editItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG))
                                                  && (a.OriginalBillingOfficeCode == editItem.BillingOfficeCode)
                                                  && (a.OriginalBillingOCC == editItem.BillingOCC)
                                              select a;
                        if ((checkUpdateItem != null) && (checkUpdateItem.Count() > 0))
                        {
                            var updateItem = checkUpdateItem.First();
                            //finalOfficeCode = (!String.IsNullOrEmpty(updateItem.OriginalBillingOfficeCode)) ? updateItem.OriginalBillingOfficeCode : updateItem.BillingOffice;
                            var newobj = new CTS051_EditBillingTargetDetail()
                            {
                                BillingClientCode = editItem.BillingClientCode,
                                BillingTargetCode = editItem.BillingTargetCode,
                                FullNameLC = updateItem.FullNameLC,
                                FullNameEN = updateItem.FullNameEN,
                                AddressEN = updateItem.AddressEN,
                                AddressLC = updateItem.AddressLC,
                                IDNo = updateItem.IDNo,
                                BranchNameEN = updateItem.BranchNameEN,
                                BranchNameLC = updateItem.BranchNameLC,
                                PhoneNo = updateItem.PhoneNo,
                                BillingOffice = updateItem.BillingOffice,
                                BusinessTypeCode = updateItem.BusinessTypeCode,
                                BusinessType = updateItem.BusinessType,
                                Nationality = updateItem.Nationality,
                                RegionCode = updateItem.RegionCode,
                                IsBefore = ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) || ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START))),
                                DisableList = disableObjList,
                                HideList = hideObjList,
                                BillingOCC = editItem.BillingOCC,
                                CustomerType = updateItem.CustomerType,
                                ObjectType = 0, //Default
                                UID = editItem.UID,
                                BillingApproveInstallationFee = updateItem.BillingApproveInstallationFee,
                                BillingApproveInstallationFeeCurrencyType = updateItem.BillingApproveInstallationFeeCurrencyType,
                                BillingCompleteInstallationFee = updateItem.BillingCompleteInstallationFee,
                                BillingCompleteInstallationFeeCurrencyType = updateItem.BillingCompleteInstallationFeeCurrencyType,
                                BillingContractFee = updateItem.BillingContractFee,
                                BillingContractFeeCurrencyType = updateItem.BillingContractFeeCurrencyType,
                                BillingInstallationFee = updateItem.BillingInstallationFee,
                                BillingInstallationFeeCurrencyType = updateItem.BillingInstallationFeeCurrencyType,
                                BillingStartInstallationFee = updateItem.BillingStartInstallationFee,
                                BillingStartInstallationFeeCurrencyType = updateItem.BillingStartInstallationFeeCurrencyType,
                                PaymentCompleteInstallationFee = updateItem.PaymentCompleteInstallationFee,
                                PaymentInstallationFee = updateItem.PaymentInstallationFee,
                                PaymentStartInstallationFee = updateItem.PaymentStartInstallationFee,
                                HasFee = true,
                                OldOfficeCode = updateItem.BillingOffice,
                                OriginalBillingClientCode = updateItem.OriginalBillingClientCode,
                                OriginalBillingOCC = updateItem.OriginalBillingOCC,
                                OriginalBillingOfficeCode = updateItem.OriginalBillingOfficeCode,
                                //SequenceNo = billingSequenceNo,
                            };
                            res.ResultData = newobj;
                        }
                        else
                        {
                            res.ResultData = obj;
                        }
                    }
                    else
                    {
                        throw new Exception("Billing client code not found.");
                    }
                }
                else
                {
                    // Load from new Item
                    var checkNewItem = from a in param.newItemList
                                       where
                                           (a.UID == editItem.UID)
                                       select a;

                    if ((checkNewItem != null) && (checkNewItem.Count() > 0))
                    {
                        var newItem = checkNewItem.First();
                        //finalOfficeCode = (!String.IsNullOrEmpty(newItem.OriginalBillingOfficeCode)) ? newItem.OriginalBillingOfficeCode : newItem.BillingOffice;
                        var newobj = new CTS051_EditBillingTargetDetail()
                        {
                            BillingClientCode = editItem.BillingClientCode,
                            BillingTargetCode = editItem.BillingTargetCode,
                            FullNameLC = newItem.FullNameLC,
                            FullNameEN = newItem.FullNameEN,
                            AddressEN = newItem.AddressEN,
                            AddressLC = newItem.AddressLC,
                            IDNo = newItem.IDNo,
                            BranchNameEN = newItem.BranchNameEN,
                            BranchNameLC = newItem.BranchNameLC,
                            PhoneNo = newItem.PhoneNo,
                            BillingOffice = newItem.BillingOffice,
                            BusinessTypeCode = newItem.BusinessTypeCode,
                            BusinessType = newItem.BusinessType,
                            RegionCode = newItem.RegionCode,
                            Nationality = newItem.Nationality,
                            IsBefore = ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) || ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START))),
                            DisableList = disableObjList,
                            HideList = hideObjList,
                            BillingOCC = editItem.BillingOCC,
                            CustomerType = newItem.CustomerType,
                            ObjectType = newItem.ObjectType,
                            UID = newItem.UID,
                            BillingApproveInstallationFee = newItem.BillingApproveInstallationFee,
                            BillingApproveInstallationFeeCurrencyType = newItem.BillingApproveInstallationFeeCurrencyType,
                            BillingCompleteInstallationFee = newItem.BillingCompleteInstallationFee,
                            BillingCompleteInstallationFeeCurrencyType = newItem.BillingCompleteInstallationFeeCurrencyType,
                            BillingContractFee = newItem.BillingContractFee,
                            BillingContractFeeCurrencyType = newItem.BillingContractFeeCurrencyType,
                            BillingInstallationFee = newItem.BillingInstallationFee,
                            BillingInstallationFeeCurrencyType = newItem.BillingInstallationFeeCurrencyType,
                            BillingStartInstallationFee = newItem.BillingStartInstallationFee,
                            BillingStartInstallationFeeCurrencyType = newItem.BillingStartInstallationFeeCurrencyType,
                            PaymentCompleteInstallationFee = newItem.PaymentCompleteInstallationFee,
                            PaymentInstallationFee = newItem.PaymentInstallationFee,
                            PaymentStartInstallationFee = newItem.PaymentStartInstallationFee,
                            HasFee = true,
                            OldOfficeCode = newItem.BillingOffice,
                            OriginalBillingClientCode = newItem.OriginalBillingClientCode,
                            OriginalBillingOCC = newItem.OriginalBillingOCC,
                            OriginalBillingOfficeCode = newItem.OriginalBillingOfficeCode
                            //SequenceNo = billingSequenceNo,
                        };
                        res.ResultData = newobj;
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
        /// Maintain items on screen when click [New] button in ‘Billing target’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS051_NewBillingTempData()
        {
            ObjectResultData res = new ObjectResultData();
            CTS051_DisplayBillingTargetDetail obj = new CTS051_DisplayBillingTargetDetail();
            CommonUtil util = new CommonUtil();

            try
            {
                CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();

                List<string> hideObjList = new List<string>();
                List<string> disableObjList = new List<string>();

                if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                    ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
                {
                    if ((param.ProductType == ProductType.C_PROD_TYPE_RENTAL_SALE) || (param.ProductType == ProductType.C_PROD_TYPE_AL))
                    {
                        hideObjList.Add("trInstallFee");
                        disableObjList.Add("BillingApproveInstallationFee");
                        disableObjList.Add("BillingApproveInstallationFeeCurrencyType");
                        //disableObjList.Add("AmountTotal");
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_ONLINE)
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_MA)
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                        hideObjList.Add("trDepositFee");
                    }
                    else if ((param.ProductType == ProductType.C_PROD_TYPE_SG) || (param.ProductType == ProductType.C_PROD_TYPE_BE))
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                        hideObjList.Add("trDepositFee");
                    }
                }
                else if (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                    || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
                 )
                {
                    if ((param.ProductType == ProductType.C_PROD_TYPE_RENTAL_SALE) || (param.ProductType == ProductType.C_PROD_TYPE_AL))
                    {
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_ONLINE)
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_MA)
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                        hideObjList.Add("trDepositFee");
                    }
                    else if ((param.ProductType == ProductType.C_PROD_TYPE_SG) || (param.ProductType == ProductType.C_PROD_TYPE_BE))
                    {
                        hideObjList.Add("trInstallFee");
                        hideObjList.Add("trApproval");
                        hideObjList.Add("trCompleteInstall");
                        hideObjList.Add("trStartService");
                        hideObjList.Add("trTotal");
                        hideObjList.Add("trDepositFee");
                    }
                }

                obj.PaymentInstallationFee = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                obj.PaymentCompleteInstallationFee = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER;
                obj.PaymentStartInstallationFee = MethodType.C_PAYMENT_METHOD_BANK_TRANSFER;

                obj.DisableList = disableObjList;
                obj.HideList = hideObjList;
                obj.IsBefore = ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) || ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)));
                obj.ObjectType = 1;// New

                res.ResultData = obj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve BillingDetail when click [Retrieve] button in ‘Specify code’ section
        /// </summary>
        /// <param name="BillingCodeSelected"></param>
        /// <param name="BillingTargetCode"></param>
        /// <param name="BillingClientCode"></param>
        /// <param name="CanCauseError"></param>
        /// <returns></returns>
        public ActionResult CTS051_RetrieveBillingTargetDetailFromCode(string BillingTargetCode, bool CanCauseError = true)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            CTS051_DisplayBillingTargetDetail obj = new CTS051_DisplayBillingTargetDetail();

            try
            {
                IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                IBillingMasterHandler billingmasterhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                string targClientCode = "";
                string objectOfficeCode = null;

                if (String.IsNullOrEmpty(BillingTargetCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS051", MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[]{"lblBillingTargetCode"}, new string[] { "BillingTargetCodeSearch" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                else
                {
                    var billingTargetItem = billinghandler.GetBillingTarget(util.ConvertBillingTargetCode(BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                    if ((billingTargetItem != null) && (billingTargetItem.Count == 1))
                    {
                        objectOfficeCode = billingTargetItem[0].BillingOfficeCode;
                        targClientCode = billingTargetItem[0].BillingClientCode;
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { BillingTargetCode }, new string[] { "BillingTargetCodeSearch" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        return Json(res);
                    }
                }

                var billingClientItem = billingmasterhandler.GetBillingClient(targClientCode);
                if ((billingClientItem != null) && (billingClientItem.Count == 1))
                {
                    var clientObj = billingClientItem[0];
                    obj = new CTS051_DisplayBillingTargetDetail()
                    {
                        BillingClientCode = util.ConvertBillingClientCode(targClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                        BillingTargetCode = BillingTargetCode,
                        FullNameLC = clientObj.FullNameLC,
                        FullNameEN = clientObj.FullNameEN,
                        AddressEN = clientObj.AddressEN,
                        AddressLC = clientObj.AddressLC,
                        IDNo = clientObj.IDNo,
                        BranchNameEN = clientObj.BranchNameEN,
                        BranchNameLC = clientObj.BranchNameLC,
                        PhoneNo = clientObj.PhoneNo,
                        BillingOffice = objectOfficeCode,
                        BusinessType = clientObj.BusinessTypeName,
                        Nationality = clientObj.Nationality,
                        CustomerType = clientObj.CustTypeCode,
                        RegionCode = clientObj.RegionCode,
                    };
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0138);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                res.ResultData = obj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Retrieve BillingDetail when click [Retrieve] button in ‘Specify code’ section
        /// </summary>
        /// <param name="BillingCodeSelected"></param>
        /// <param name="BillingTargetCode"></param>
        /// <param name="BillingClientCode"></param>
        /// <param name="CanCauseError"></param>
        /// <returns></returns>
        public ActionResult CTS051_RetrieveBillingClientDetailFromCode(string BillingClientCode, bool CanCauseError = true)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            CTS051_DisplayBillingTargetDetail obj = new CTS051_DisplayBillingTargetDetail();

            try
            {
                IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                IBillingMasterHandler billingmasterhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                string targClientCode = "";
                string objectOfficeCode = null;

                if (string.IsNullOrEmpty(BillingClientCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS051", MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "lblBillingClientCode" }, new string[] { "BillingClientCodeSearch" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                else
                {
                    targClientCode = util.ConvertBillingClientCode(BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                }

                var billingClientItem = billingmasterhandler.GetBillingClient(targClientCode);
                if ((billingClientItem != null) && (billingClientItem.Count == 1))
                {
                    var clientObj = billingClientItem[0];
                    obj = new CTS051_DisplayBillingTargetDetail()
                    {
                        BillingClientCode = util.ConvertBillingClientCode(targClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                        BillingTargetCode = null,
                        FullNameLC = clientObj.FullNameLC,
                        FullNameEN = clientObj.FullNameEN,
                        AddressEN = clientObj.AddressEN,
                        AddressLC = clientObj.AddressLC,
                        IDNo = clientObj.IDNo,
                        BranchNameEN = clientObj.BranchNameEN,
                        BranchNameLC = clientObj.BranchNameLC,
                        PhoneNo = clientObj.PhoneNo,
                        BillingOffice = objectOfficeCode,
                        BusinessType = clientObj.BusinessTypeName,
                        Nationality = clientObj.Nationality,
                        CustomerType = clientObj.CustTypeCode,
                        RegionCode = clientObj.RegionCode,
                    };
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { util.ConvertBillingClientCode(targClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT) }, new string[] { "BillingClientCodeSearch" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                res.ResultData = obj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrieve BillingDetail when click [Copy] button in ‘Copy name and address information’ section
        /// </summary>
        /// <param name="AddressCopyInfo"></param>
        /// <returns></returns>
        public ActionResult CTS051_RetrieveBillingDetailFromCopy(string AddressCopyInfo)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            CTS051_DisplayBillingTargetDetail obj = new CTS051_DisplayBillingTargetDetail();
            IMasterHandler masterhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

            try
            {
                CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();
                var regionLst = masterhandler.GetTbm_Region();
                var businessTypeLst = masterhandler.GetTbm_BusinessType();
                var contractData = rentalhandler.GetTbt_RentalContractBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                var contractObj = contractData[0];

                if (AddressCopyInfo == "0") // Contract target
                {
                    var cusData = masterhandler.GetTbm_Customer(contractObj.ContractTargetCustCode);
                    var businessTypeData = from a in businessTypeLst where a.BusinessTypeCode == cusData[0].BusinessTypeCode select a;
                    var regionData = from a in regionLst where a.RegionCode == cusData[0].RegionCode select a;
                    obj = new CTS051_DisplayBillingTargetDetail()
                    {
                        AddressEN = cusData[0].AddressFullEN,
                        AddressLC = cusData[0].AddressFullLC,
                        FullNameEN = cusData[0].CustFullNameEN,
                        FullNameLC = cusData[0].CustFullNameLC,
                        PhoneNo = cusData[0].PhoneNo,
                        IDNo = cusData[0].IDNo,
                        RegionCode = cusData[0].RegionCode,
                        Nationality = regionData.First().Nationality,
                        BusinessType = businessTypeData.First().BusinessTypeName,
                        CustomerType = cusData[0].CustTypeCode,
                        CompanyTypeCode = cusData[0].CompanyTypeCode
                    };
                }
                else if (AddressCopyInfo == "1") // Branch of contract target
                {
                    var cusData = masterhandler.GetTbm_Customer(contractObj.ContractTargetCustCode);
                    var businessTypeData = from a in businessTypeLst where a.BusinessTypeCode == cusData[0].BusinessTypeCode select a;
                    var regionData = from a in regionLst where a.RegionCode == cusData[0].RegionCode select a;
                    obj = new CTS051_DisplayBillingTargetDetail()
                    {
                        AddressEN = contractObj.BranchAddressEN,
                        AddressLC = contractObj.BranchAddressLC,
                        FullNameEN = cusData[0].CustFullNameEN,
                        FullNameLC = cusData[0].CustFullNameLC,
                        BranchNameEN = contractObj.BranchNameEN,
                        BranchNameLC = contractObj.BranchNameLC,
                        PhoneNo = cusData[0].PhoneNo,
                        IDNo = cusData[0].IDNo,
                        RegionCode = cusData[0].RegionCode,
                        Nationality = regionData.First().Nationality,
                        BusinessType = businessTypeData.First().BusinessTypeName,
                        CustomerType = cusData[0].CustTypeCode,
                        CompanyTypeCode = cusData[0].CompanyTypeCode
                    };
                }
                else if (AddressCopyInfo == "2") // Real customer (End user)
                {
                    var cusData = masterhandler.GetTbm_Customer(contractObj.RealCustomerCustCode);
                    var businessTypeData = from a in businessTypeLst where a.BusinessTypeCode == cusData[0].BusinessTypeCode select a;
                    var regionData = from a in regionLst where a.RegionCode == cusData[0].RegionCode select a;
                    obj = new CTS051_DisplayBillingTargetDetail()
                    {
                        AddressEN = cusData[0].AddressFullEN,
                        AddressLC = cusData[0].AddressFullLC,
                        FullNameEN = cusData[0].CustFullNameEN,
                        FullNameLC = cusData[0].CustFullNameLC,
                        PhoneNo = cusData[0].PhoneNo,
                        IDNo = cusData[0].IDNo,
                        Nationality = regionData.First().Nationality,
                        RegionCode = cusData[0].RegionCode,
                        BusinessType = businessTypeData.First().BusinessTypeName,
                        CustomerType = cusData[0].CustTypeCode,
                        CompanyTypeCode = cusData[0].CompanyTypeCode
                    };
                }
                else if (AddressCopyInfo == "3") // Site
                {
                    var cusData = masterhandler.GetTbm_Customer(contractObj.RealCustomerCustCode);
                    var siteData = masterhandler.GetTbm_Site(contractObj.SiteCode);
                    var businessTypeData = from a in businessTypeLst where a.BusinessTypeCode == cusData[0].BusinessTypeCode select a;
                    var regionData = from a in regionLst where a.RegionCode == cusData[0].RegionCode select a;
                    obj = new CTS051_DisplayBillingTargetDetail()
                    {
                        AddressEN = siteData[0].AddressFullEN,
                        AddressLC = siteData[0].AddressFullLC,
                        FullNameEN = cusData[0].CustFullNameEN,
                        FullNameLC = cusData[0].CustFullNameLC,
                        PhoneNo = cusData[0].PhoneNo,
                        IDNo = cusData[0].IDNo,
                        Nationality = regionData.First().Nationality,
                        RegionCode = cusData[0].RegionCode,
                        BusinessType = businessTypeData.First().BusinessTypeName,
                        BranchNameEN = siteData[0].SiteNameEN,
                        BranchNameLC= siteData[0].SiteNameLC,
                        CustomerType = cusData[0].CustTypeCode,
                        CompanyTypeCode = cusData[0].CompanyTypeCode
                    };
                }

                res.ResultData = obj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Delete Billing item when click [Remove] button in grid ChangePlanBillingTarget
        /// </summary>
        /// <param name="delItem"></param>
        /// <returns></returns>
        public ActionResult CTS051_DeleteBillingItem(dtBillingTemp_SetItem delItem)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

            try
            {
                // Checking item can delete
                CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();

                if (delItem.IsNew)
                {
                    var targDeleteNewBillingTemp = from a in param.newItemList
                                                   where (a.UID == delItem.UID)
                                                   select a;

                    if ((targDeleteNewBillingTemp != null) && (targDeleteNewBillingTemp.Count() > 0))
                    {
                        param.newItemList.Remove(targDeleteNewBillingTemp.First());
                        res.ResultData = true;
                    }
                }
                else
                {
                    var billingTempData = rentalhandler.GetBillingTempForChangePlan(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.ContractOCC);
                    var targDeleteBillingTemp = from a in billingTempData
                                                where
                                                    (a.BillingTargetCode == util.ConvertBillingTargetCode(delItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG))
                                                    && (a.BillingClientCode == util.ConvertBillingClientCode(delItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG))
                                                    && (a.BillingOCC == delItem.BillingOCC)
                                                select a;
                    if ((targDeleteBillingTemp != null) && (targDeleteBillingTemp.Count() > 0))
                    {
                        if (String.IsNullOrEmpty(targDeleteBillingTemp.First().BillingOCC))
                        {
                            // Can delete
                            param.deleteItemList.Add(delItem);
                            res.ResultData = true;
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
        /// Validate Business when click [Add/Update] button in ‘Billing target detail’ section
        /// </summary>
        /// <param name="targObj"></param>
        /// <returns></returns>
        public ActionResult CTS051_ValidateBusiness_ChangePlanDetail(dtBillingTemp_SetItem targObj)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

            try
            {
                CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();
                List<string> fieldName = new List<string>();
                List<string> realFieldName = new List<string>();
                List<string> controlName = new List<string>();
                List<MessageModel> errorList = new List<MessageModel>();

                // Validate Require field
                if (String.IsNullOrEmpty(targObj.CustomerType)
                    || String.IsNullOrEmpty(targObj.FullNameEN)
                    || String.IsNullOrEmpty(targObj.FullNameLC)
                    || String.IsNullOrEmpty(targObj.AddressEN)
                    || String.IsNullOrEmpty(targObj.AddressLC))
                {
                    //errorList.Add(MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0134, null));
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0134, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                if (String.IsNullOrEmpty(targObj.BillingOffice))
                {
                    fieldName.Add("lblBillingOffice");
                    controlName.Add("BillingOffice");
                }

                if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                    ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
                {
                    if ((param.ProductType == ProductType.C_PROD_TYPE_RENTAL_SALE) || (param.ProductType == ProductType.C_PROD_TYPE_AL))
                    {
                        //if (!targObj.BillingContractFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingContractFee");
                        //    controlName.Add("BillingContractFee");
                        //}

                        //if (!targObj.BillingCompleteInstallationFee.HasValue)
                        //{
                        //    fieldName.Add("lblCompleteInstallation");
                        //    controlName.Add("BillingCompleteInstallationFee");
                        //}

                        if (targObj.BillingCompleteInstallationFee.HasValue
                            && (targObj.BillingCompleteInstallationFee.GetValueOrDefault() > 0)
                            && String.IsNullOrEmpty(targObj.PaymentCompleteInstallationFee))
                        {
                            //fieldName.Add("lblCompleteInstallation");
                            errorList.Add(MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3029, null));
                            controlName.Add("PaymentCompleteInstallationFee");
                        }

                        //if (!targObj.BillingStartInstallationFee.HasValue)
                        //{
                        //    fieldName.Add("lblStartService");
                        //    controlName.Add("BillingStartInstallationFee");
                        //}

                        if (targObj.BillingStartInstallationFee.HasValue
                            && (targObj.BillingStartInstallationFee.GetValueOrDefault() > 0)
                            && String.IsNullOrEmpty(targObj.PaymentStartInstallationFee))
                        {
                            //fieldName.Add("lblStartService");
                            errorList.Add(MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3030, null));
                            controlName.Add("PaymentStartInstallationFee");
                        }

                        //if (!targObj.BillingDepositFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingDepositFee");
                        //    controlName.Add("BillingDepositFee");
                        //}

                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_ONLINE)
                    {
                        //if (!targObj.BillingContractFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingContractFee");
                        //    controlName.Add("BillingContractFee");
                        //}

                        //if (!targObj.BillingDepositFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingDepositFee");
                        //    controlName.Add("BillingDepositFee");
                        //}

                   }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_MA)
                    {
                        //if (!targObj.BillingContractFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingContractFee");
                        //    controlName.Add("BillingContractFee");
                        //}
                    }
                    else if ((param.ProductType == ProductType.C_PROD_TYPE_SG) || (param.ProductType == ProductType.C_PROD_TYPE_BE))
                    {
                        //if (!targObj.BillingContractFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingContractFee");
                        //    controlName.Add("BillingContractFee");
                        //}
                    }
                }
                else if (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                    || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
                  )
                {
                    if ((param.ProductType == ProductType.C_PROD_TYPE_RENTAL_SALE) || (param.ProductType == ProductType.C_PROD_TYPE_AL))
                    {
                        //if (!targObj.BillingContractFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingContractFee");
                        //    controlName.Add("BillingContractFee");
                        //}

                        //if (!targObj.BillingInstallationFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingInstallationFee");
                        //    controlName.Add("BillingInstallationFee");
                        //}

                        if (targObj.BillingInstallationFee.HasValue
                            && (targObj.BillingInstallationFee.GetValueOrDefault() > 0)
                            && String.IsNullOrEmpty(targObj.PaymentInstallationFee))
                        {
                            //fieldName.Add("lblBillingInstallationFee");
                            errorList.Add(MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3037, null));
                            controlName.Add("PaymentInstallationFee");
                        }

                        //if (!targObj.BillingDepositFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingDepositFee");
                        //    controlName.Add("BillingDepositFee");
                        //}
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_SALE)
                    {
                        //if (!targObj.BillingContractFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingContractFee");
                        //    controlName.Add("BillingContractFee");
                        //}

                        //if (!targObj.BillingDepositFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingDepositFee");
                        //    controlName.Add("BillingDepositFee");
                        //}

                        
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_MA)
                    {
                        //if (!targObj.BillingContractFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingContractFee");
                        //    controlName.Add("BillingContractFee");
                        //}
                        
                    }
                    else if ((param.ProductType == ProductType.C_PROD_TYPE_SG) || (param.ProductType == ProductType.C_PROD_TYPE_BE))
                    {
                        //if (!targObj.BillingContractFee.HasValue)
                        //{
                        //    fieldName.Add("lblBillingContractFee");
                        //    controlName.Add("BillingContractFee");
                        //}
                        
                    }
                }

                foreach (var rawFieldName in fieldName.Distinct())
                {
                    realFieldName.Add(CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS051", rawFieldName));
                }

                if ((realFieldName.Count > 0) || (controlName.Count > 0) || (errorList.Count > 0))
                {
                    if ((realFieldName.Count > 0) && (controlName.Count > 0))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, realFieldName.ToArray(), controlName.ToArray());
                    }

                    if (errorList.Count > 0)
                    {
                        if (res.MessageList == null)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, null, null);
                            res.MessageList.Clear();
                        }

                        res.MessageList.AddRange(errorList);

                        if (controlName.Count > 0)
                        {
                            res.Message.Controls = controlName.ToArray();
                        }
                    }

                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                // Validate Business
                var billingTempData = rentalhandler.GetBillingTempForChangePlan(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.ContractOCC);

                if (!String.IsNullOrEmpty(targObj.BillingClientCode))
                {
                    // Create full list

                    // Filter duplicate item which (if it new record)
                    var fullListDuplicate = from a in billingTempData
                                            where
                                                (a.BillingClientCode == util.ConvertBillingClientCode(targObj.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG))
                                                && ((a.BillingOfficeCode == targObj.BillingOffice)
                                                && (targObj.OldOfficeCode != targObj.BillingOffice))
                                                && (targObj.ObjectType == 1)
                                            select a;

                    var fullUpdateList = param.updateItemList.Union(param.newItemList).ToList();
                    var fullUpdateListDuplicate = from a in fullUpdateList where
                                                  (a.BillingClientCode == targObj.BillingClientCode)
                                                  && (a.BillingOffice == targObj.BillingOffice)
                                                  && (a.UID != targObj.UID)
                                                  select a;

                    if (((fullUpdateListDuplicate != null) && (fullUpdateListDuplicate.Count() > 0)) || ((fullListDuplicate != null) && (fullListDuplicate.Count() > 0)))
                    {
                        // Exists in old item
                        // Check is delete item
                        if (param.deleteItemList != null)
                        {
                            var deleteListDuplicate = from a in param.deleteItemList
                                                      where
                                                          (a.BillingClientCode == targObj.BillingClientCode)
                                                          && (a.BillingOfficeCode == targObj.BillingOffice)
                                                      select a;

                            if ((deleteListDuplicate != null) && (deleteListDuplicate.Count() == 0))
                            {
                                // Error: It's not delete item
                                // Item duplicated !!!!
                                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3032, null, null);
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                return Json(res);
                            }
                        }
                    }

                    if (param.newItemList != null)
                    {
                        // Check is new item
                        var newListDuplicate = from a in param.newItemList
                                               where (a.BillingClientCode == targObj.BillingClientCode)
                                               && (a.BillingOffice == targObj.BillingOffice)
                                               && (!String.IsNullOrEmpty(a.BillingClientCode))
                                               && (a.UID != targObj.UID)
                                               select a;

                        if ((newListDuplicate != null) && (newListDuplicate.Count() > 0))
                        {
                            // Error: It's duplicate with new item
                            // Item duplicated !!!!
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3032, null, null);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return Json(res);
                        }
                    }
                }

                if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                    ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
                {
                    if (String.IsNullOrEmpty(targObj.BillingOCC)
                        && !targObj.AmountTotal.HasValue 
                        && !targObj.AmountTotalUsd.HasValue
                        && !targObj.BillingContractFee.HasValue
                        && !targObj.BillingDepositFee.HasValue)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3087, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                    if (String.IsNullOrEmpty(targObj.BillingOCC)
                        && targObj.AmountTotal.GetValueOrDefault() == 0
                        && targObj.AmountTotalUsd.GetValueOrDefault() == 0
                        && targObj.BillingContractFee.GetValueOrDefault() == 0
                        && targObj.BillingDepositFee.GetValueOrDefault() == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3087, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }

                    //if ((String.IsNullOrEmpty(targObj.BillingOCC))
                    //        && (!targObj.AmountTotal.HasValue && !targObj.BillingContractFee.HasValue && !targObj.BillingDepositFee.HasValue))
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3087, null, null);
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    return Json(res);
                    //}
                    //if ((String.IsNullOrEmpty(targObj.BillingOCC))
                    //    && ((targObj.AmountTotal.GetValueOrDefault() == 0) && (targObj.BillingContractFee.GetValueOrDefault() == 0) && (targObj.BillingDepositFee.GetValueOrDefault() == 0)))
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3087, null, null);
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    return Json(res);
                    //}
                }
                else if (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                    || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
                 )
                {
                    if ((String.IsNullOrEmpty(targObj.BillingOCC))
                    && (!targObj.BillingInstallationFee.HasValue && !targObj.BillingContractFee.HasValue && !targObj.BillingDepositFee.HasValue))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3087, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }

                    if ((String.IsNullOrEmpty(targObj.BillingOCC))
                        && ((targObj.BillingInstallationFee.GetValueOrDefault() == 0) && (targObj.BillingContractFee.GetValueOrDefault() == 0) && (targObj.BillingDepositFee.GetValueOrDefault() == 0)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3087, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }

                

                var targChangePlanObj = CreateChangePlanObject_CTS051(targObj);

                if (!targChangePlanObj.IsNew && targChangePlanObj.HasUpdate)
                {
                    // It is old item
                    var existsItem = param.updateItemList.Where(x => x.UID == targChangePlanObj.UID);
                    if (existsItem.Count() > 0)
                    {
                        param.updateItemList.Remove(existsItem.First());
                    }
                    param.updateItemList.Add(targObj);
                }
                else if (targChangePlanObj.IsNew)
                {
                    // It is new item
                    targObj.UID = targChangePlanObj.UID;
                    var existsItem = param.newItemList.Where(x => x.UID == targChangePlanObj.UID);
                    if (existsItem.Count() > 0)
                    {
                        param.newItemList.Remove(existsItem.First());
                    }
                    param.newItemList.Add(targObj);
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
        /// Validate Business when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <param name="regisObj"></param>
        /// <param name="HasChangePlanDetailTask"></param>
        /// <returns></returns>
        public ActionResult CTS051_ValidateBusiness_All(CTS051_RegisterChangePlan regisObj, bool HasChangePlanDetailTask)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res = ValidateBusiness_CTS051(regisObj, HasChangePlanDetailTask);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Update data to database when click [Confirm] button in ‘Action button’ section
        /// </summary>
        /// <param name="regisObj"></param>
        /// <param name="HasChangePlanDetailTask"></param>
        /// <returns></returns>
        public ActionResult CTS051_ConfirmRegister(CTS051_RegisterChangePlan regisObj, bool HasChangePlanDetailTask)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();
                res = ValidateBusiness_CTS051(regisObj, HasChangePlanDetailTask);
                if (res.HasResultData && ((bool)res.ResultData))
                {
                    res = new ObjectResultData();
                    bool regisResult = RegisterChangePlan_CTS051(regisObj);
                    if (regisResult)
                    {
                        var rentalContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                        string contractcode = new CommonUtil().ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                        var contractinfo = rentalContractHandler.GetTbt_RentalContractBasic(contractcode, null).FirstOrDefault();
                        if (contractinfo != null && contractinfo.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            var viewinfo = rentalContractHandler.GetTbt_RentalContractBasicForView(contractinfo.ContractCode).FirstOrDefault();
                            CommonUtil.MappingObjectLanguage(viewinfo);
                            doChangePlanBeforeStartEmail templateObj = new doChangePlanBeforeStartEmail();
                            templateObj.ContractCode = viewinfo.ContractCode;
                            templateObj.CustCode = viewinfo.ContractTargetCustCode;
                            templateObj.CustName = viewinfo.CustName;
                            templateObj.SiteCode = viewinfo.SiteCode;
                            templateObj.SiteName = viewinfo.SiteName;
                            templateObj.OperationOfficeCode = viewinfo.OperationOfficeCode;
                            templateObj.OperationOfficeName = viewinfo.Op_OfficeName;
                            templateObj.CurrentDate = DateTime.Today;
                            if (CommonUtil.dsTransData != null && CommonUtil.dsTransData.dtUserData != null)
                            {
                                templateObj.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            }
                            var contracthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                            contracthandler.SendEmailChangePlanBeforeStart(templateObj);
                        }

                    }

                    if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) || 
                        ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT
                            , MessageUtil.MessageList.MSG3049
                            , null);
                    }
                    else if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING) && regisResult)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT
                            , MessageUtil.MessageList.MSG3050
                            , null);
                    }
                    else if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING) && !regisResult)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT
                            , MessageUtil.MessageList.MSG3051
                            , null);
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

        #region Method

        /// <summary>
        /// Validate authority of screen
        /// </summary>
        /// <param name="res"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private ObjectResultData ValidateAuthority_CTS051(ObjectResultData res, CTS051_ScreenParameter2 param, bool isInitScreen = true)
        {
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            CommonUtil util = new CommonUtil();

            try
            {
                if (CheckIsSuspending(res))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP12_CHANGE_PLAN, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (String.IsNullOrEmpty(param.ContractCode))
                {   
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                //Comment by Jutarat A. on 08082012
                //var saleExists = salehandler.IsContractExist(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                //if (saleExists.Count > 0 && saleExists[0].GetValueOrDefault())
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3278, null, null);
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    return res;
                //}

                var rentalObj = rentalhandler.GetTbt_RentalContractBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                if ((rentalObj == null) || (rentalObj.Count != 1))
                {
                    if (isInitScreen)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124, null, null);
                    }
                    //Add by Jutarat A. on 08082012
                    else
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { param.ContractCode }, null);
                    }
                    //End Add

                    return res;
                }

                var existsContarctOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == rentalObj[0].ContractOfficeCode);
                var existsOperateOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == rentalObj[0].OperationOfficeCode);

                if ((((rentalObj[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                    ((rentalObj[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (rentalObj[0].StartType == StartType.C_START_TYPE_ALTER_START)))
                        && (existsContarctOffice.Count() <= 0) && (existsOperateOffice.Count() <= 0))
                    || ( (rentalObj[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START || rentalObj[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                        && (existsOperateOffice.Count() <= 0))
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }
                
                var lastOCC = rentalhandler.GetLastUnimplementedOCC(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                if (String.IsNullOrEmpty(lastOCC))
                {
                    lastOCC = rentalhandler.GetLastImplementedOCC(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                }

                param.ContractOCC = lastOCC;

                var contractDat = rentalhandler.GetEntireContract(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), lastOCC);

                if ((contractDat.dtTbt_RentalContractBasic[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL)
                    || (contractDat.dtTbt_RentalContractBasic[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                    || (contractDat.dtTbt_RentalContractBasic[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT)
                    || (contractDat.dtTbt_RentalContractBasic[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_TERMINATED))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3001, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                #region Validate installation

                try
                {
                    IInstallationHandler insHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    insHandler.CheckCanRegisterCP12(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                }
                catch (Exception ex)
                {
                    res.AddErrorMessage(ex);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Create DO of ChangePlan
        /// </summary>
        /// <param name="billingClient"></param>
        /// <returns></returns>
        private CTS051_ChangePlanGrid CreateChangePlanObject_CTS051(List<dtBillingTempChangePlanData> billingClient)
        {
            CTS051_ChangePlanGrid result = new CTS051_ChangePlanGrid();
            CommonUtil util = new CommonUtil();
            CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            IBillingClientMasterHandler billingclienthandler = ServiceContainer.GetService<IBillingClientMasterHandler>() as IBillingClientMasterHandler;

            string billingTargetnamePattern = "(1) {0}<br />(2) {1}";
            string installationFeePattern = "(1) {0}<br />(2) {1}<br />(3) {2}";

            string targetName1 = "-";
            string targetName2 = "-";

            string installFee1 = "-";
            string installFee2 = "-";
            string installFee3 = "-";

            string contractFee = "-";
            string depositFee = "-";
            string installFee = "";

            ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

            foreach (tbt_BillingTemp billingClientItem in billingClient)
            {
                string txtAmount;
                if (billingClientItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    txtAmount = CommonUtil.TextNumeric(billingClientItem.BillingAmtUsd);
                else
                    txtAmount = CommonUtil.TextNumeric(billingClientItem.BillingAmt);

                if (currencies != null)
                {
                    DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == billingClientItem.BillingAmtCurrencyType);
                    if (curr == null)
                        curr = currencies[0];

                    txtAmount = string.Format("{0} {1}", curr.ValueDisplayEN, txtAmount);
                }

                if (billingClientItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                {
                    if (billingClientItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        contractFee = (billingClientItem.BillingAmtUsd.HasValue) ? txtAmount : "-";
                    }
                    else
                    {
                        contractFee = (billingClientItem.BillingAmt.HasValue) ? txtAmount : "-";
                    }
                }
                else if (billingClientItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                {
                    if (billingClientItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        depositFee = (billingClientItem.BillingAmtUsd.HasValue) ? txtAmount : "-";
                    }
                    else
                    {
                        depositFee = (billingClientItem.BillingAmt.HasValue) ? txtAmount : "-";
                    }
                }
                else
                {
                    if (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                        || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
                    )
                    {
                        if (String.IsNullOrEmpty(installFee))
                        {
                            if (billingClientItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || billingClientItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                            {
                                if (billingClientItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    installFee = (billingClientItem.BillingAmtUsd.HasValue) ? txtAmount : "-";
                                }
                                else
                                {
                                    installFee = (billingClientItem.BillingAmt.HasValue) ? txtAmount : "-";
                                }
                                    
                            }
                        }
                    }
                    else if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                        ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
                    {
                        if ((billingClientItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || billingClientItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                            && (billingClientItem.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT))
                        {
                            if (billingClientItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                installFee1 = (billingClientItem.BillingAmtUsd.HasValue) ? txtAmount : "-";
                            }
                            else
                            {
                                installFee1 = (billingClientItem.BillingAmt.HasValue) ? txtAmount : "-";
                            }
                          
                        }
                        else if ((billingClientItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || billingClientItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                          && (billingClientItem.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION))
                        {
                            
                            if (billingClientItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                installFee2 = (billingClientItem.BillingAmtUsd.HasValue) ? txtAmount : "-";
                            }
                            else
                            {
                                installFee2 = (billingClientItem.BillingAmt.HasValue) ? txtAmount : "-";
                            }
                        }
                        else if ((billingClientItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || billingClientItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                          && (billingClientItem.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE))
                        {
                            
                            if (billingClientItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                installFee3 = (billingClientItem.BillingAmtUsd.HasValue) ? txtAmount : "-";
                            }
                            else
                            {
                                installFee3 = (billingClientItem.BillingAmt.HasValue) ? txtAmount : "-";
                            }
                        }
                    }
                }
            }

            if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
            {
                installFee = String.Format(installationFeePattern, installFee1, installFee2, installFee3);
            }
            else
            {
                if (String.IsNullOrEmpty(installFee))
                {
                    installFee = "-";
                }
            }

            //var billingTargetTmp = billinghandler.GetBillingTarget(billingClient[0].BillingTargetCode);
            if (!String.IsNullOrEmpty(billingClient[0].BillingTargetCode))
            {
                var billingTargetObj = billinghandler.GetBillingTarget(billingClient[0].BillingTargetCode);

                if (billingTargetObj.Count == 1)
                {
                    var billingClientObj = billingclienthandler.GetTbm_BillingClient(billingTargetObj[0].BillingClientCode);

                    targetName1 = ((billingClientObj != null) && (billingClientObj.Count == 1)) ? billingClientObj[0].FullNameEN : "-";
                    targetName2 = ((billingClientObj != null) && (billingClientObj.Count == 1)) ? billingClientObj[0].FullNameLC : "-";
                }
                else if (!String.IsNullOrEmpty(billingClient[0].BillingClientCode))
                {
                    var billingClientObj = billingclienthandler.GetTbm_BillingClient(billingClient[0].BillingClientCode);

                    targetName1 = ((billingClientObj != null) && (billingClientObj.Count == 1)) ? billingClientObj[0].FullNameEN : "-";
                    targetName2 = ((billingClientObj != null) && (billingClientObj.Count == 1)) ? billingClientObj[0].FullNameLC : "-";
                }
            }
            else if (!String.IsNullOrEmpty(billingClient[0].BillingClientCode))
            {
                var billingClientObj = billingclienthandler.GetTbm_BillingClient(billingClient[0].BillingClientCode);

                targetName1 = ((billingClientObj != null) && (billingClientObj.Count == 1)) ? billingClientObj[0].FullNameEN : "-";
                targetName2 = ((billingClientObj != null) && (billingClientObj.Count == 1)) ? billingClientObj[0].FullNameLC : "-";
            }

            var officeItem = officehandler.GetTbm_Office(billingClient[0].BillingOfficeCode);
            CommonUtil.MappingObjectLanguage<tbm_Office>(officeItem);
            string officeName = ((officeItem != null) && (officeItem.Count == 1)) ? CommonUtil.TextCodeName(officeItem[0].OfficeCode, officeItem[0].OfficeName) : "";
            result = new CTS051_ChangePlanGrid()
            {
                BillingClientCode = util.ConvertBillingClientCode(billingClient[0].BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                BillingTargetCode = util.ConvertBillingTargetCode(billingClient[0].BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                BillingOCC = billingClient[0].BillingOCC,
                BillingOfficeCode = billingClient[0].BillingOfficeCode,
                BillingOfficeName = officeName,
                BillingTargetName = String.Format(billingTargetnamePattern, targetName1, targetName2),
                CanDelete = String.IsNullOrEmpty(billingClient[0].BillingOCC),
                IsNew = false,
                ContractFee = contractFee,
                DepositFee = depositFee,
                InstallFee = installFee,
                HasUpdate = false,
                UID = Guid.NewGuid().ToString(),
                OriginalBillingClientCode = billingClient[0].BillingClientCode,
                OriginalBillingOfficeCode = billingClient[0].BillingOfficeCode,
                OriginalBillingOCC = billingClient[0].BillingOCC
            };

            return result;
        }

        /// <summary>
        /// Create DO of ChangePlan
        /// </summary>
        /// <param name="changePlanItem"></param>
        /// <returns></returns>
        private CTS051_ChangePlanGrid CreateChangePlanObject_CTS051(dtBillingTemp_SetItem changePlanItem)
        {
            CTS051_ChangePlanGrid res = new CTS051_ChangePlanGrid();
            CommonUtil util = new CommonUtil();
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();

            ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

            string billingTargetnamePattern = "(1) {0}<br />(2) {1}";
            string installationFeePattern = "(1) {0}<br />(2) {1}<br />(3) {2}";

            string installFee1 = "-";
            string installFee2 = "-";
            string installFee3 = "-";

            string contractFee = "-";
            string depositFee = "-";
            string installFee = "-";

            if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
            {
                #region Contract Fee

                if (changePlanItem.BillingContractFee.HasValue)
                {
                    contractFee = CommonUtil.TextNumeric(changePlanItem.BillingContractFee);

                    DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == changePlanItem.BillingContractFeeCurrencyType);
                    if (curr == null)
                        curr = currencies[0];

                    contractFee = string.Format("{0} {1}", curr.ValueDisplayEN, contractFee);
                }
                else
                    contractFee = "-";

                #endregion
                #region Install Fee 1

                if (changePlanItem.BillingApproveInstallationFee.HasValue)
                {
                    installFee1 = CommonUtil.TextNumeric(changePlanItem.BillingApproveInstallationFee);

                    DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == changePlanItem.BillingApproveInstallationFeeCurrencyType);
                    if (curr == null)
                        curr = currencies[0];

                    installFee1 = string.Format("{0} {1}", curr.ValueDisplayEN, installFee1);
                }
                else
                    installFee1 = "-";

                #endregion
                #region Install Fee 2

                if (changePlanItem.BillingCompleteInstallationFee.HasValue)
                {
                    installFee2 = CommonUtil.TextNumeric(changePlanItem.BillingCompleteInstallationFee);

                    DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == changePlanItem.BillingCompleteInstallationFeeCurrencyType);
                    if (curr == null)
                        curr = currencies[0];

                    installFee2 = string.Format("{0} {1}", curr.ValueDisplayEN, installFee2);
                }
                else
                    installFee2 = "-";

                #endregion
                #region Install Fee 3

                if (changePlanItem.BillingStartInstallationFee.HasValue)
                {
                    installFee3 = CommonUtil.TextNumeric(changePlanItem.BillingStartInstallationFee);

                    DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == changePlanItem.BillingStartInstallationFeeCurrencyType);
                    if (curr == null)
                        curr = currencies[0];

                    installFee3 = string.Format("{0} {1}", curr.ValueDisplayEN, installFee3);
                }
                else
                    installFee3 = "-";

                #endregion
                #region Deposit Fee

                if (changePlanItem.BillingDepositFee.HasValue)
                {
                    depositFee = CommonUtil.TextNumeric(changePlanItem.BillingDepositFee);

                    DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == changePlanItem.BillingDepositFeeCurrencyType);
                    if (curr == null)
                        curr = currencies[0];

                    depositFee = string.Format("{0} {1}", curr.ValueDisplayEN, depositFee);
                }
                else
                    depositFee = "-";

                #endregion
                
                installFee = String.Format(installationFeePattern, installFee1, installFee2, installFee3);
            }
            else if (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
            )
            {
                #region Contract Fee

                if (changePlanItem.BillingContractFee.HasValue)
                {
                    contractFee = CommonUtil.TextNumeric(changePlanItem.BillingContractFee);

                    DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == changePlanItem.BillingContractFeeCurrencyType);
                    if (curr == null)
                        curr = currencies[0];

                    contractFee = string.Format("{0} {1}", curr.ValueDisplayEN, contractFee);
                }
                else
                    contractFee = "-";

                #endregion
                #region Install Fee

                if (changePlanItem.BillingInstallationFee.HasValue)
                {
                    installFee = CommonUtil.TextNumeric(changePlanItem.BillingInstallationFee);

                    DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == changePlanItem.BillingInstallationFeeCurrencyType);
                    if (curr == null)
                        curr = currencies[0];

                    installFee = string.Format("{0} {1}", curr.ValueDisplayEN, installFee);
                }
                else
                    installFee = "-";

                #endregion
                #region Deposit Fee

                if (changePlanItem.BillingDepositFee.HasValue)
                {
                    depositFee = CommonUtil.TextNumeric(changePlanItem.BillingDepositFee);

                    DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == changePlanItem.BillingDepositFeeCurrencyType);
                    if (curr == null)
                        curr = currencies[0];

                    depositFee = string.Format("{0} {1}", curr.ValueDisplayEN, depositFee);
                }
                else
                    depositFee = "-";

                #endregion
            }

            string targetName1 = changePlanItem.FullNameEN;
            string targetName2 = changePlanItem.FullNameLC;

            var officeItem = officehandler.GetTbm_Office(changePlanItem.BillingOffice);
            CommonUtil.MappingObjectLanguage<tbm_Office>(officeItem);
            string officeName = ((officeItem != null) && (officeItem.Count == 1)) ? CommonUtil.TextCodeName(officeItem[0].OfficeCode, officeItem[0].OfficeName) : "";

            res = new CTS051_ChangePlanGrid()
            {
                BillingClientCode = changePlanItem.BillingClientCode,
                BillingOCC = changePlanItem.BillingOCC,
                BillingOfficeCode = changePlanItem.BillingOffice,
                BillingTargetCode = changePlanItem.BillingTargetCode,
                CanDelete = String.IsNullOrEmpty(changePlanItem.BillingOCC),
                HasUpdate = (changePlanItem.ObjectType == 0),
                IsNew = (changePlanItem.ObjectType == 1),
                BillingOfficeName = officeName,
                BillingTargetName = String.Format(billingTargetnamePattern, targetName1, targetName2),
                InstallFee = installFee,
                ContractFee = contractFee,
                DepositFee = depositFee,
                UID = ((changePlanItem.ObjectType == 1) && (String.IsNullOrEmpty(changePlanItem.UID))) ? Guid.NewGuid().ToString() : ((!String.IsNullOrEmpty(changePlanItem.UID)) ? changePlanItem.UID : null),
                OriginalBillingClientCode = changePlanItem.OriginalBillingClientCode,
                OriginalBillingOCC = changePlanItem.OriginalBillingOCC,
                OriginalBillingOfficeCode = changePlanItem.OriginalBillingOfficeCode
            };

            return res;
        }

        /// <summary>
        /// Get XML schema of grid
        /// </summary>
        /// <param name="inContractStatus"></param>
        /// <param name="inProductType"></param>
        /// <returns></returns>
        private string GetGridXMLSchema_CTS051(string inContractStatus, string inProductType, string inStartType)
        {
            if ((inContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                ((inContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (inStartType == StartType.C_START_TYPE_ALTER_START)))
            {
                //if (inProductType == ProductType.C_PROD_TYPE_ONLINE)
                if ((inProductType == ProductType.C_PROD_TYPE_MA) || (inProductType == ProductType.C_PROD_TYPE_SG) || (inProductType == ProductType.C_PROD_TYPE_BE) || (inProductType == ProductType.C_PROD_TYPE_ONLINE))
                {
                    return "Contract\\CTS051BillingBeforeStart_ONLINE";
                }
                //else if ((inProductType == ProductType.C_PROD_TYPE_MA) || (inProductType == ProductType.C_PROD_TYPE_SG) || (inProductType == ProductType.C_PROD_TYPE_BE))
                //{
                //    return "Contract\\CTS051BillingBeforeStart_MA-SG";
                //}
                else
                {
                    return "Contract\\CTS051BillingBeforeStart";
                }
            }
            else if ((inContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) || (inContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING))
            {
                //if (inProductType == ProductType.C_PROD_TYPE_ONLINE)
                if ((inProductType == ProductType.C_PROD_TYPE_MA) || (inProductType == ProductType.C_PROD_TYPE_SG) || (inProductType == ProductType.C_PROD_TYPE_BE) || (inProductType == ProductType.C_PROD_TYPE_ONLINE))
                {
                    return "Contract\\CTS051BillingAfterStart_ONLINE";
                }
                //else if ((inProductType == ProductType.C_PROD_TYPE_MA) || (inProductType == ProductType.C_PROD_TYPE_SG) || (inProductType == ProductType.C_PROD_TYPE_BE))
                //{
                //    return "Contract\\CTS051BillingAfterStart_MA-SG";
                //}
                else
                {
                    return "Contract\\CTS051BillingAfterStart";
                }
            }
            else
            {
                throw new Exception("Invalid contract conditions to load grid.");
            }

            return null;
        }

        /// <summary>
        /// Validate required field of DO for register ChangePlan
        /// </summary>
        /// <param name="regisObj"></param>
        /// <returns></returns>
        private ObjectResultData ValidateRequired_CTS051(CTS051_RegisterChangePlan regisObj)
        {
            ObjectResultData res = new ObjectResultData();
            CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

            try
            {
                List<string> errorCtrl = new List<string>();
                List<string> errorLabel = new List<string>();

                if (!regisObj.ExpectedOperationDate.HasValue)
                {
                    errorCtrl.Add("ExpectedOperationDate");
                    errorLabel.Add("lblExpectedOperationDate");
                }

                if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                    ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
                {
                    if ((param.ProductType == ProductType.C_PROD_TYPE_AL) || (param.ProductType == ProductType.C_PROD_TYPE_RENTAL_SALE))
                    {
                        if (!regisObj.ChangePlanOrderContractFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanOrderContractFee");
                            errorLabel.Add("lblContractFeeAfterChange");
                        }

                        if (!regisObj.ChangePlanOrderInstallationFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanOrderInstallationFee");
                            errorLabel.Add("lblInstallationFeeAfterChange");
                        }

                        if (!regisObj.ChangePlanCompleteInstallationFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanCompleteInstallationFee");
                            errorLabel.Add("lblInstallationFeeAfterChange");
                        }

                        if (!regisObj.ChangePlanStartInstallationFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanStartInstallationFee");
                            errorLabel.Add("lblInstallationFeeAfterChange");
                        }
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_ONLINE)
                    {
                        if (!regisObj.ChangePlanOrderContractFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanOrderContractFee");
                            errorLabel.Add("lblContractFeeAfterChange");
                        }
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_MA)
                    {
                        if (!regisObj.ChangePlanOrderContractFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanOrderContractFee");
                            errorLabel.Add("lblContractFeeAfterChange");
                        }
                    }
                    else if ((param.ProductType == ProductType.C_PROD_TYPE_SG) || (param.ProductType == ProductType.C_PROD_TYPE_BE))
                    {
                        if (!regisObj.ChangePlanOrderContractFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanOrderContractFee");
                            errorLabel.Add("lblContractFeeAfterChange");
                        }
                    }
                }
                else if (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                    || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                {
                    if ((param.ProductType == ProductType.C_PROD_TYPE_AL) || (param.ProductType == ProductType.C_PROD_TYPE_RENTAL_SALE))
                    {
                        if (!regisObj.ChangePlanOrderContractFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanOrderContractFee");
                            errorLabel.Add("lblContractFeeAfterChange");
                        }

                        if (!regisObj.ChangePlanOrderInstallationFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanOrderInstallationFee");
                            errorLabel.Add("lblInstallationFeeAfterChange");
                        }

                        if (!regisObj.ChangePlanCompleteInstallationFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanCompleteInstallationFee");
                            errorLabel.Add("lblInstallationFeeAfterChange");
                        }
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_ONLINE)
                    {
                        if (!regisObj.ChangePlanOrderContractFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanOrderContractFee");
                            errorLabel.Add("lblContractFeeAfterChange");
                        }
                    }
                    else if (param.ProductType == ProductType.C_PROD_TYPE_MA)
                    {
                        if (!regisObj.ChangePlanOrderContractFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanOrderContractFee");
                            errorLabel.Add("lblContractFeeAfterChange");
                        }
                    }
                    else if ((param.ProductType == ProductType.C_PROD_TYPE_SG) || (param.ProductType == ProductType.C_PROD_TYPE_BE))
                    {
                        if (!regisObj.ChangePlanOrderContractFee.HasValue)
                        {
                            errorCtrl.Add("ChangePlanOrderContractFee");
                            errorLabel.Add("lblContractFeeAfterChange");
                        }
                    }
                }

                if (String.IsNullOrEmpty(regisObj.NegotiationStaffEmpNo1))
                {
                    errorCtrl.Add("NegotiationStaffEmpNo1");
                    errorLabel.Add("lblNegotiationStaff1");
                }
                //else
                //{
                //    var emp1 = emphandler.GetTbm_Employee(regisObj.NegotiationStaffEmpNo1);
                //    if ((emp1 == null) || (emp1.Count == 0))
                //    {
                //        errorCtrl.Add("NegotiationStaffEmpNo1");
                //        errorLabel.Add("lblNegotiationStaff1");
                //    }
                //}

                //if (!String.IsNullOrEmpty(regisObj.NegotiationStaffEmpNo2))
                //{
                //    var emp2 = emphandler.GetTbm_Employee(regisObj.NegotiationStaffEmpNo2);
                //    if ((emp2 == null) || (emp2.Count == 0))
                //    {
                //        errorCtrl.Add("NegotiationStaffEmpNo2");
                //        errorLabel.Add("lblNegotiationStaff2");
                //    }
                //}

                //if (regisObj.ContractDurationFlag)
                //{
                //    if (!regisObj.ContractDurationMonth.HasValue)
                //    {
                //        errorCtrl.Add("ContractDurationMonth");
                //        errorLabel.Add("lblContractDurationMonth");
                //    }

                //    if (!regisObj.AutoRenewMonth.HasValue)
                //    {
                //        errorCtrl.Add("AutoRenewMonth");
                //        errorLabel.Add("lblAutorenewMonth");
                //    }

                //    if (!regisObj.EndContractDate.HasValue)
                //    {
                //        errorCtrl.Add("EndContractDate");
                //        errorLabel.Add("lblContractEndDate");
                //    }
                //}

                
                if ((errorCtrl.Count > 0) || (errorLabel.Count > 0))
                {
                    string errorLabelText = "";
                    //List<String> errorLabelText = new List<string>();

                    foreach (string item in errorLabel.Distinct())
                    {
                        if (errorLabelText.Length > 0)
                        {
                            errorLabelText += ", ";
                        }

                        errorLabelText += CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS051", item);
                        //errorLabelText.Add(CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS051", item));
                    }

                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { errorLabelText }, errorCtrl.ToArray());
                }
                else
                {
                    ValidatorUtil validator = new ValidatorUtil();
                    bool isError = false;
                    //bool checkValidate = true;

                    //bool sameCurrencyType = false;

                    //foreach (var d in param.updateItemList)
                    //{
                    //    if(regisObj.ChangePlanNormalInstallationFeeCurrencyType == d.BillingApproveInstallationFeeCurrencyType)
                    //    {
                    //        if (d.BillingApproveInstallationFee != null)
                    //        {
                    //            if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != d.BillingApproveInstallationFeeCurrencyType)
                    //            {
                    //                sameCurrencyType = true;
                    //                break;
                    //            }
                    //        }
                    //        if (d.BillingCompleteInstallationFee != null)
                    //        {
                    //            if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != d.BillingCompleteInstallationFeeCurrencyType)
                    //            {
                    //                sameCurrencyType = true;
                    //                break;
                    //            }
                    //        }
                    //        if (d.BillingStartInstallationFee != null)
                    //        {
                    //            if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != d.BillingStartInstallationFeeCurrencyType)
                    //            {
                    //                sameCurrencyType = true;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {          
                    //        if (d.BillingApproveInstallationFee != null)
                    //        {
                    //            if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != d.BillingApproveInstallationFeeCurrencyType)
                    //            {
                    //                isError = true;
                    //                validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS051"
                    //                    , MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3314
                    //                    , "ChangePlanOrderInstallationFee", "lblChangePlanOrderInstallationFee", "ChangePlanOrderInstallationFee");
                    //                break;
                    //            }
                    //            else
                    //                checkValidate = false;
                    //        }
                    //        if (d.BillingCompleteInstallationFee != null)
                    //        {
                    //            if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != d.BillingCompleteInstallationFeeCurrencyType)
                    //            {
                    //                isError = true;
                    //                validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS051"
                    //                    , MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3314
                    //                    , "ChangePlanOrderInstallationFee", "lblChangePlanOrderInstallationFee", "ChangePlanOrderInstallationFee");
                    //                break;
                    //            }
                    //            else
                    //                checkValidate = false;
                    //        }
                    //        if (d.BillingStartInstallationFee != null)
                    //        {
                    //            if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != d.BillingStartInstallationFeeCurrencyType)
                    //            {
                    //                isError = true;
                    //                validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS051"
                    //                    , MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3314
                    //                    , "ChangePlanOrderInstallationFee", "lblChangePlanOrderInstallationFee", "ChangePlanOrderInstallationFee");
                    //                break;
                    //            }
                    //            else
                    //                checkValidate = false;
                    //        }

                    //        if(checkValidate)
                    //        {
                    //            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3314, null, new string[] { "ChangePlanOrderInstallationFeeCurrencyType" });
                    //            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG;
                    //        }
                            
                    //    }

                    //    if((regisObj.ChangePlanCompleteInstallationFeeCurrencyType != d.BillingCompleteInstallationFeeCurrencyType)
                    //        || regisObj.ChangePlanStartInstallationFeeCurrencyType != d.BillingStartInstallationFeeCurrencyType)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3314, null, new string[] { "ChangePlanCompleteInstallationFeeCurrencyType" , "ChangePlanStartInstallationFeeCurrencyType"
                    //        , "BillingCompleteInstallationFeeCurrencyType", "BillingStartInstallationFeeCurrencyType"});
                    //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG;
                    //    }
                    //}
                    //if (sameCurrencyType)
                    //{
                    //    //isError = true;
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3314, null, new string[] { "ChangePlanOrderInstallationFeeCurrencyType" });
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG;
                    //}

                    //if(!checkValidate)
                    //{
                    //    decimal? sumCheck = 0;
                    //    sumCheck = regisObj.ChangePlanApproveInstallationFee + regisObj.ChangePlanCompleteInstallationFee + regisObj.ChangePlanStartInstallationFee;

                    //    if(sumCheck != regisObj.ChangePlanOrderInstallationFee)
                    //    {
                    //        isError = true;
                    //        validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS051"
                    //           , MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3088
                    //           , "ChangePlanOrderInstallationFee", "lblChangePlanCompleteInstallationFee", "ChangePlanOrderInstallationFee");
                    //    }
                    //}

                    // add by jirawat jannet on 2016-11-4

                    //decimal sumBillingInstrumentFee = 0;
                    //sameCurrencyType = true;

                    //foreach (var d in param.updateItemList)
                    //{
                    //    if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != d.BillingInstallationFeeCurrencyType)
                    //    {
                    //        sameCurrencyType = false;
                    //        break;
                    //    }

                    //    sumBillingInstrumentFee += d.BillingInstallationFee ?? 0;
                    //}
                    //foreach (var d in param.newItemList)
                    //{
                    //    if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != d.BillingInstallationFeeCurrencyType)
                    //    {
                    //        sameCurrencyType = false;
                    //        break;
                    //    }

                    //    sumBillingInstrumentFee += d.BillingInstallationFee ?? 0;
                    //}

                    //if (sameCurrencyType)
                    //{
                    //    //if (sumBillingInstrumentFee.ConvertTo<decimal>(false, 0) != regisObj.ChangePlanOrderInstallationFee)
                    //    //{
                    //    //    isError = true;
                    //    //    validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS051"
                    //    //       , MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3342
                    //    //       , "ChangePlanOrderInstallationFee", "lblChangePlanOrderInstallationFee", "ChangePlanOrderInstallationFee");
                    //    //}
                    //}
                    // Begin add: by Jirawat Jannet on 2016-11-29

                    //if (param.updateItemList != null && param.updateItemList.Count > 0)
                    //{
                    //    string ChangePlanCompleteInstallationFeeCurrencyType = "ChangePlanCompleteInstallationFeeCurrencyType";
                    //    string ChangePlanStartInstallationFeeCurrencyType = "ChangePlanStartInstallationFeeCurrencyType";
                    //    bool isSameCompleteInstallationFeeCurrencyType = true;
                    //    bool isSameStartInstallationFeeCurrencyType = true;

                    //    foreach (var d in param.updateItemList)
                    //    {
                    //        if ((regisObj.ChangePlanCompleteInstallationFeeCurrencyType != d.BillingCompleteInstallationFeeCurrencyType)
                    //            && regisObj.ChangePlanApproveInstallationFee != 0)
                    //        {
                    //            isSameCompleteInstallationFeeCurrencyType = false;
                    //        }
                    //        if (regisObj.ChangePlanStartInstallationFeeCurrencyType != d.BillingStartInstallationFeeCurrencyType)
                    //            isSameStartInstallationFeeCurrencyType = false;
                    //    }

                    //    if (!isSameCompleteInstallationFeeCurrencyType)
                    //    {
                    //        isError = true;
                    //        validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS051"
                    //            , MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3317
                    //            , ChangePlanCompleteInstallationFeeCurrencyType);
                    //    }
                    //    if (!isSameStartInstallationFeeCurrencyType)
                    //    {
                    //        isError = true;
                    //        validator.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS051"
                    //            , MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3318
                    //            , ChangePlanStartInstallationFeeCurrencyType);
                    //    }
                    //}


                    if (isError)
                    {
                        ValidatorUtil.BuildErrorMessage(res, validator, null);
                    }


                    // End add

                    // end add
                }
              
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Validate Business of screen
        /// </summary>
        /// <param name="regisObj"></param>
        /// <param name="HasChangePlanDetailTask"></param>
        /// <returns></returns>
        private ObjectResultData ValidateBusiness_CTS051(CTS051_RegisterChangePlan regisObj, bool HasChangePlanDetailTask)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            IQuotationHandler quotationhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            IBillingInterfaceHandler billinginterfacehandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            IContractHandler contracthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;

            try
            {
                res = ValidateAuthority_CTS051(res, param, false);

                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                res = ValidateRequired_CTS051(regisObj);

                if (res.IsError)
                {
                    if (res.MessageType == MessageModel.MESSAGE_TYPE.WARNING_DIALOG)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG;
                    }
                    else
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    //return res;
                }

                var dsRentalContractData = rentalhandler.GetEntireContract(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.ContractOCC);
                var dsQuotation = quotationhandler.GetQuotationData(new doGetQuotationDataCondition()
                {
                    Alphabet = param.Alphabet,
                    QuotationTargetCode = param.QuotationCode
                });

                if (param.ProductType == ProductType.C_PROD_TYPE_MA)
                {
                    var maxUpdatedDate = rentalhandler.GetMaxUpdateDateOfMATargetContract(dsRentalContractData.dtTbt_RentalContractBasic[0].ContractCode, dsRentalContractData.dtTbt_RentalContractBasic[0].LastOCC);
                    if ((maxUpdatedDate != null) && (maxUpdatedDate.Count == 1))
                    {
                        if (dsQuotation.dtTbt_QuotationBasic.CreateDate <= maxUpdatedDate[0])
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, null, null);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    }
                }


                #region Validate installation

                try
                {
                    IInstallationHandler insHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    insHandler.CheckCanRegisterCP12(dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractCode);
                }
                catch (Exception ex)
                {
                    res.AddErrorMessage(ex);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                #endregion
                #region Validate expected operation date

                if ((dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                    || dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG
                    || dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE
                    || dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                    && dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_BEF_START)
                {
                    if (dsRentalContractData.dtTbt_RentalContractBasic[0].LastChangeImplementDate != null)
                    {
                        if (regisObj.ExpectedOperationDate < dsRentalContractData.dtTbt_RentalContractBasic[0].LastChangeImplementDate)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3298);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    }

                    DateTime nowDate = DateTime.Now;
                    nowDate = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day);
                    if (regisObj.ExpectedOperationDate > nowDate)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3299);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }
                }

                #endregion

                if (regisObj.ChangePlanOrderContractFee > CommonValue.C_MAX_MONTHLY_FEE_INPUT)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3287,
                        new string[] { CommonValue.C_MAX_MONTHLY_FEE_INPUT.ToString("N2") },
                        new string[] { "ChangePlanOrderContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (CommonUtil.IsNullOrEmpty(regisObj.ChangePlanNormalContractFeeCurrencyType))
                    regisObj.ChangePlanNormalContractFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                if (CommonUtil.IsNullOrEmpty(regisObj.ChangePlanApproveInstallationFeeCurrencyType))
                    regisObj.ChangePlanApproveInstallationFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                if (CommonUtil.IsNullOrEmpty(regisObj.ChangePlanCompleteInstallationFeeCurrencyType))
                    regisObj.ChangePlanCompleteInstallationFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                if (CommonUtil.IsNullOrEmpty(regisObj.ChangePlanStartInstallationFeeCurrencyType))
                    regisObj.ChangePlanStartInstallationFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;

                //Default Currency = 'Rp.'
                //Add By Pachara 23112016
                if (dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType == null)
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                if (regisObj.ChangePlanNormalInstallationFeeCurrencyType == null)
                    regisObj.ChangePlanNormalInstallationFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;



                if (regisObj.ChangePlanOrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    if ((regisObj.ChangePlanNormalContractFee > 0
                            && (CommonUtil.IsNullOrEmpty(regisObj.ChangePlanOrderContractFee)
                            || regisObj.ChangePlanOrderContractFee == 0))
                        && CommonUtil.IsNullOrEmpty(regisObj.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3003, null, new string[] { "ApproveNo1" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if ((regisObj.ChangePlanNormalContractFee > 0
                            && regisObj.ChangePlanOrderContractFee > 0)
                        && CommonUtil.IsNullOrEmpty(regisObj.ApproveNo1))
                    {
                        decimal? fee10 = (regisObj.ChangePlanNormalContractFee * 0.1M);
                        decimal? fee1000 = (regisObj.ChangePlanNormalContractFee * 10.0M);
                        if (regisObj.ChangePlanOrderContractFee <= fee10)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3004, null, new string[] { "ApproveNo1" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                        if (regisObj.ChangePlanOrderContractFee >= fee1000)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3004, null, new string[] { "ApproveNo1" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    }

                    if (regisObj.ChangePlanNormalContractFee != regisObj.ChangePlanOrderContractFee
                        && regisObj.ChangePlanOrderContractFee > 0
                        && CommonUtil.IsNullOrEmpty(regisObj.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3005, null, new string[] { "ApproveNo1" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    //if (isSameContractFeeCurrency == true
                    //    && contract.OrderContractFee != totalContractFee
                    //    && totalContractFeeUS == 0)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3012,
                    //                        null, new string[] { "OrderContractFee" });
                    //    return false;
                    //}
                }

                if (regisObj.ContractDurationFlag)
                {
                    if (regisObj.ContractDurationMonth.HasValue && !regisObj.AutoRenewMonth.HasValue)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3033, null, new string[] { "ContractDurationMonth" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if (regisObj.ContractDurationMonth.HasValue && regisObj.EndContractDate.HasValue)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3034, null, new string[] { "EndContractDate" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if (!regisObj.ContractDurationMonth.HasValue && !regisObj.EndContractDate.HasValue)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3035, null, new string[] { "ContractDurationMonth", "EndContractDate" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if (string.IsNullOrEmpty(regisObj.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3079, null, new string[] { "ApproveNo1" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if (regisObj.EndContractDate.HasValue && (regisObj.EndContractDate.Value <= DateTime.Now))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3080, null, new string[] { "EndContractDate" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if ((regisObj.ContractDurationMonth.HasValue) && (regisObj.ContractDurationMonth.Value < 1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3266, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS051", "lblContractDurationMonth") }, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if ((regisObj.ContractDurationMonth.GetValueOrDefault() < 36) && (String.IsNullOrEmpty(regisObj.ApproveNo1)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3081, null, new string[] { "ApproveNo1" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if ((regisObj.AutoRenewMonth.HasValue) && (regisObj.AutoRenewMonth.Value < 1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3266, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS051", "lblAutorenewMonth") }, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if ((regisObj.AutoRenewMonth.GetValueOrDefault() < 12) && (String.IsNullOrEmpty(regisObj.ApproveNo1)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3082, null, new string[] { "ApproveNo1" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }
                }

                if (regisObj.ChangePlanOrderContractFeeCurrencyType == dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType)
                {
                    if(dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if ((dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE)
                        && (dsRentalContractData.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate.HasValue)
                        && (regisObj.ChangePlanOrderContractFee.GetValueOrDefault() != dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3006, null, new string[] { "ChangePlanOrderContractFee" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    }
                    else
                    {
                        if ((dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE)
                        && (dsRentalContractData.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate.HasValue)
                        && (regisObj.ChangePlanOrderContractFee.GetValueOrDefault() != dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFee))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3006, null, new string[] { "ChangePlanOrderContractFee" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    } 
                }

                if ((dsRentalContractData.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE)
                        && (dsRentalContractData.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate.HasValue)
                        && (regisObj.ChangePlanOrderContractFeeCurrencyType != dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3360, null, new string[] { "ChangePlanOrderContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    //return res;
                }

                if (regisObj.ChangePlanNormalInstallationFeeCurrencyType == regisObj.ChangePlanOrderInstallationFeeCurrencyType)
                {
                    decimal ninst = 0;
                    decimal oinst = 0;
                    decimal oinst_a = 0;
                    decimal oinst_c = 0;
                    decimal oinst_s = 0;
                    bool skpinst = false;

                    if (CommonUtil.IsNullOrEmpty(regisObj.ChangePlanNormalInstallationFee) == false)
                        ninst = regisObj.ChangePlanNormalInstallationFee.Value;
                    if (CommonUtil.IsNullOrEmpty(regisObj.ChangePlanOrderInstallationFee) == false)
                        oinst = regisObj.ChangePlanOrderInstallationFee.Value;

                    if (CommonUtil.IsNullOrEmpty(regisObj.ChangePlanApproveInstallationFee) == false)
                        oinst_a = regisObj.ChangePlanApproveInstallationFee.Value;
                    if (oinst_a > 0 && regisObj.ChangePlanApproveInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType)
                        skpinst = true;

                    if (CommonUtil.IsNullOrEmpty(regisObj.ChangePlanCompleteInstallationFee) == false)
                        oinst_c = regisObj.ChangePlanCompleteInstallationFee.Value;
                    if (oinst_c > 0 && regisObj.ChangePlanCompleteInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType)
                        skpinst = true;

                    if (CommonUtil.IsNullOrEmpty(regisObj.ChangePlanStartInstallationFee) == false)
                        oinst_s = regisObj.ChangePlanStartInstallationFee.Value;
                    if (oinst_s > 0 && regisObj.ChangePlanStartInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType)
                        skpinst = true;

                    if (ninst != oinst
                        && CommonUtil.IsNullOrEmpty(regisObj.ApproveNo1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3007, null, new string[] { "ApproveNo1" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if (skpinst == false)
                    {
                        decimal? totalInstallFee = oinst_a + oinst_c + oinst_s;
                        if (oinst != totalInstallFee)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3088, null, new string[] { "ChangePlanOrderInstallationFee" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    }
                }

                var emp1 = emphandler.GetTbm_Employee(regisObj.NegotiationStaffEmpNo1);
                var emp2 = emphandler.GetTbm_Employee(regisObj.NegotiationStaffEmpNo2);

                if (!String.IsNullOrEmpty(regisObj.NegotiationStaffEmpNo1)
                    && ((emp1 == null) || (emp1.Count == 0)))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, new string[] { regisObj.NegotiationStaffEmpNo1 }, new string[] { "NegotiationStaffEmpNo1" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }
                if (!String.IsNullOrEmpty(regisObj.NegotiationStaffEmpNo2)
                    && ((emp2 == null) || (emp2.Count == 0)))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, new string[] { regisObj.NegotiationStaffEmpNo2 }, new string[] { "NegotiationStaffEmpNo2" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                var isSequence = true;
                var hasEmpty = false;

                for (int i = 1; i <= 5; i++)
                {
                    if (!isSequence)
                    {
                        break;
                    }

                    switch (i)
                    {
                        case 1:
                            {
                                if (String.IsNullOrEmpty(regisObj.ApproveNo1))
                                {
                                    hasEmpty = true;
                                }
                            }
                            break;

                        case 2:
                            {
                                if (!String.IsNullOrEmpty(regisObj.ApproveNo2) && hasEmpty)
                                {
                                    isSequence = false;
                                }
                                else if (String.IsNullOrEmpty(regisObj.ApproveNo2))
                                {
                                    hasEmpty = true;
                                }
                            }
                            break;

                        case 3:
                            {
                                if (!String.IsNullOrEmpty(regisObj.ApproveNo3) && hasEmpty)
                                {
                                    isSequence = false;
                                }
                                else if (String.IsNullOrEmpty(regisObj.ApproveNo3))
                                {
                                    hasEmpty = true;
                                }
                            }
                            break;

                        case 4:
                            {
                                if (!String.IsNullOrEmpty(regisObj.ApproveNo4) && hasEmpty)
                                {
                                    isSequence = false;
                                }
                                else if (String.IsNullOrEmpty(regisObj.ApproveNo4))
                                {
                                    hasEmpty = true;
                                }
                            }
                            break;

                        case 5:
                            {
                                if (!String.IsNullOrEmpty(regisObj.ApproveNo5) && hasEmpty)
                                {
                                    isSequence = false;
                                }
                                else if (String.IsNullOrEmpty(regisObj.ApproveNo5))
                                {
                                    hasEmpty = true;
                                }
                            }
                            break;

                    }
                }

                if (!isSequence)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3009, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (regisObj.ChangePlanNormalInstallationFeeCurrencyType == regisObj.ChangePlanOrderInstallationFeeCurrencyType)
                {
                    if ((regisObj.ChangePlanNormalInstallationFee.GetValueOrDefault() != regisObj.ChangePlanOrderInstallationFee.GetValueOrDefault())
                        && (String.IsNullOrEmpty(regisObj.ApproveNo1)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3007, null, new string[] { "ApproveNo1" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if (regisObj.ChangePlanOrderInstallationFeeCurrencyType == regisObj.ChangePlanApproveInstallationFeeCurrencyType
                        && regisObj.ChangePlanOrderInstallationFeeCurrencyType == regisObj.ChangePlanCompleteInstallationFeeCurrencyType
                        && regisObj.ChangePlanOrderInstallationFeeCurrencyType == regisObj.ChangePlanStartInstallationFeeCurrencyType)
                    {
                        if ((regisObj.ChangePlanOrderInstallationFee.GetValueOrDefault() != (
                                regisObj.ChangePlanApproveInstallationFee.GetValueOrDefault()
                                + regisObj.ChangePlanCompleteInstallationFee.GetValueOrDefault()
                                + regisObj.ChangePlanStartInstallationFee.GetValueOrDefault())))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3088, null, new string[] { "ChangePlanOrderInstallationFee" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    }
                }

                bool isSameContractFeeCurrency = true;
                bool isSameInstallCurrency = true;
                bool isSameInstallApproveCurrency = true;
                bool isSameInstallStartServiceCurrency = true;
                bool isSameInstallCompleteCurrency = true;
                
                //bool isSameInstallFeeAtApproveCurrency = true;

                int cntContractFeeOverZero = 0;
                decimal sumContractFee = 0, sumInstallationFee = 0, sumApproveContract = 0, sumCompleteInstallationFee = 0, sumStartServiceFee = 0, sumDepositFee = 0,
                        sumContractFeeUS = 0, sumInstallationFeeUS = 0, sumApproveContractUS = 0, sumCompleteInstallationFeeUS = 0, sumStartServiceFeeUS = 0, sumDepositFeeUS = 0;

                var cntNewRes = from a in param.newItemList where a.BillingContractFee.GetValueOrDefault() > 0 select a;
                var cntUpdateRes = from a in param.updateItemList where a.BillingContractFee.GetValueOrDefault() > 0 select a;

                //var billingTmpFull = billinginterfacehandler.GetBillingBasicAsBillingTemp(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.ContractOCC);
                var billingTmpFull = rentalhandler.GetBillingTempForChangePlan(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.ContractOCC);

                foreach (var billingTempItem in billingTmpFull)
                {
                    //var delItemFilter = from a in param.deleteItemList
                    //                    where
                    //                        param.deleteItemList.Where(x => (x.BillingOffice == a.BillingOffice)
                    //                        && (x.BillingClientCode == a.BillingClientCode)
                    //                        && (x.BillingOCC == a.BillingOCC)).Count() > 0
                    //                    select a;

                    var delItemFilter = from a in param.deleteItemList
                                        where (a.BillingOfficeCode == billingTempItem.BillingOfficeCode)
                                        && (util.ConvertBillingClientCode(a.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) == billingTempItem.BillingClientCode)
                                        && (a.BillingOCC == billingTempItem.BillingOCC)
                                        select a;

                    if ((delItemFilter != null) && (delItemFilter.Count() == 0))
                    {
                        //var updateItemFilter = from a in param.updateItemList
                        //                       where
                        //                           param.updateItemList.Where(x => (a.BillingTargetCode == a.BillingTargetCode)
                        //                           && (x.BillingClientCode == a.BillingClientCode)
                        //                           && (x.BillingOCC == a.BillingOCC)).Count() > 0
                        //                       select a;

                        var updateItemFilter = from a in param.updateItemList
                                               where (a.BillingOffice == billingTempItem.BillingOfficeCode)
                                               && (util.ConvertBillingClientCode(a.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) == billingTempItem.BillingClientCode)
                                               && (a.BillingOCC == billingTempItem.BillingOCC)
                                               select a;

                        if ((updateItemFilter != null) && (updateItemFilter.Count() == 0))
                        {
                            //if ((billingTempItem.BillingAmt.GetValueOrDefault() > 0)
                            //    && (billingTempItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE))
                            //{
                            //    cntContractFeeOverZero++;

                            //    if (regisObj.ChangePlanNormalContractFeeCurrencyType != billingTempItem.BillingAmtCurrencyType)
                            //        isSameContractFeeCurrency = false;

                            //    if (billingTempItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            //        sumContractFeeUS += billingTempItem.BillingAmt.GetValueOrDefault();
                            //    else
                            //        sumContractFee += billingTempItem.BillingAmt.GetValueOrDefault();
                            //}
                            if (billingTempItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                            {
                                //if (regisObj.ChangePlanNormalContractFeeCurrencyType != billingTempItem.BillingAmtCurrencyType)
                                //    isSameContractFeeCurrency = false;

                                if (billingTempItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    if (billingTempItem.BillingAmtUsd.GetValueOrDefault() > 0)
                                    {
                                        if (regisObj.ChangePlanOrderContractFeeCurrencyType != billingTempItem.BillingAmtCurrencyType)
                                            isSameContractFeeCurrency = false;

                                        cntContractFeeOverZero++;
                                        sumContractFeeUS += billingTempItem.BillingAmtUsd.GetValueOrDefault();
                                    }
                                }
                                else
                                {
                                    if (billingTempItem.BillingAmt.GetValueOrDefault() > 0)
                                    {
                                        if (regisObj.ChangePlanOrderContractFeeCurrencyType != billingTempItem.BillingAmtCurrencyType)
                                            isSameContractFeeCurrency = false;

                                        cntContractFeeOverZero++;
                                        sumContractFee += billingTempItem.BillingAmt.GetValueOrDefault();
                                    }
                                }

                            }


                            if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                                ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
                            {
                                if (billingTempItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || billingTempItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                                {
                                    if (billingTempItem.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION)
                                    {
                                        if (billingTempItem.BillingAmt > 0
                                            || billingTempItem.BillingAmtUsd > 0)
                                        {
                                            if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != billingTempItem.BillingAmtCurrencyType)
                                                isSameInstallCompleteCurrency = false;

                                            if (billingTempItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                                //sumCompleteInstallationFeeUS += billingTempItem.BillingAmt.GetValueOrDefault();
                                                sumCompleteInstallationFeeUS += billingTempItem.BillingAmtUsd.GetValueOrDefault();
                                            else
                                                sumCompleteInstallationFee += billingTempItem.BillingAmt.GetValueOrDefault();
                                        }
                                    }

                                    if (billingTempItem.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE)
                                    {
                                        if (billingTempItem.BillingAmt > 0
                                            || billingTempItem.BillingAmtUsd > 0)
                                        {
                                            if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != billingTempItem.BillingAmtCurrencyType)
                                                isSameInstallStartServiceCurrency = false;

                                            if (billingTempItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                                //sumStartServiceFeeUS += billingTempItem.BillingAmt.GetValueOrDefault();
                                                sumStartServiceFeeUS += billingTempItem.BillingAmtUsd.GetValueOrDefault();
                                            else
                                                sumStartServiceFee += billingTempItem.BillingAmt.GetValueOrDefault();
                                        }
                                    }


                                    if (billingTempItem.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                                    {
                                        //if (regisObj.ChangePlanOrderInstallationFee > 0)
                                        //{
                                        //    if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != billingTempItem.BillingAmtCurrencyType)
                                        //        isSameInstallApproveCurrency = false;
                                        //}

                                        if (billingTempItem.BillingAmt > 0
                                                || billingTempItem.BillingAmtUsd > 0)
                                        {
                                            if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != billingTempItem.BillingAmtCurrencyType)
                                                isSameInstallApproveCurrency = false;

                                            if (billingTempItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                                sumApproveContract += billingTempItem.BillingAmtUsd.GetValueOrDefault();
                                            else
                                                sumApproveContractUS += billingTempItem.BillingAmt.GetValueOrDefault();
                                        }
                                    }
                                }
                            }
                            else if (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                              || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
                          )
                            {
                                if (billingTempItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || billingTempItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                                {
                                    if (billingTempItem.BillingAmt > 0
                                        || billingTempItem.BillingAmtUsd > 0)
                                    {
                                        if (regisObj.ChangePlanOrderInstallationFeeCurrencyType != billingTempItem.BillingAmtCurrencyType)
                                            isSameInstallCurrency = false;

                                        if (billingTempItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            //sumInstallationFeeUS += billingTempItem.BillingAmt.GetValueOrDefault();
                                            sumInstallationFeeUS += billingTempItem.BillingAmtUsd.GetValueOrDefault();
                                        else
                                            sumInstallationFee += billingTempItem.BillingAmt.GetValueOrDefault();
                                    }
                                }
                            }

                            if (billingTempItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                            {
                                if (billingTempItem.BillingAmt > 0
                                    || billingTempItem.BillingAmtUsd > 0)
                                {
                                    if (billingTempItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        //sumDepositFeeUS += billingTempItem.BillingAmt.GetValueOrDefault();
                                        sumDepositFeeUS += billingTempItem.BillingAmtUsd.GetValueOrDefault();
                                    else
                                        sumDepositFee += billingTempItem.BillingAmt.GetValueOrDefault();
                                }
                            }

                            //var billingClient = billinginterfacehandler.GetBillingBasicAsBillingTemp(billingTempItem.BillingTargetCode, billingTempItem.BillingOCC);
                            //var billingClient_ContractFee = from a in billingClient
                            //                                where
                            //                                    (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                            //                                select a;

                            //var billingClient_CompleteInstallationFee = from a in billingClient
                            //                                            where (a.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION)
                            //                                            select a;

                            //var billingClient_StartServiceFee = from a in billingClient
                            //                                    where (a.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE)
                            //                                    select a;

                            //var billingClient_DepositFee = from a in billingClient
                            //                                    where (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                            //                                    select a;

                            //if ((billingClient_ContractFee != null) && (billingClient_ContractFee.Count() > 0) && (billingClient_ContractFee.First().BillingAmt.GetValueOrDefault() > 0))
                            //{
                            //    cntContractFeeOverZero++;
                            //    sumContractFee += billingClient_ContractFee.Sum(x => x.BillingAmt.GetValueOrDefault());
                            //}

                            //if ((billingClient_CompleteInstallationFee != null) && (billingClient_CompleteInstallationFee.Count() > 0))
                            //{
                            //    sumCompleteInstallationFee += billingClient_CompleteInstallationFee.Sum(x => x.BillingAmt.GetValueOrDefault());
                            //}

                            //if ((billingClient_StartServiceFee != null) && (billingClient_StartServiceFee.Count() > 0))
                            //{
                            //    sumStartServiceFee += billingClient_StartServiceFee.Sum(x => x.BillingAmt.GetValueOrDefault());
                            //}

                            //if ((billingClient_DepositFee != null) && (billingClient_DepositFee.Count() > 0))
                            //{
                            //    sumDepositFee += billingClient_DepositFee.Sum(x => x.BillingAmt.GetValueOrDefault());
                            //}
                        }
                    }
                }

                cntContractFeeOverZero = cntContractFeeOverZero + cntNewRes.Count() + cntUpdateRes.Count();

                #region Summary Contract Fee

                if (isSameContractFeeCurrency == true)
                {
                    //if ((param.newItemList.Exists(x => x.BillingContractFeeCurrencyType != regisObj.ChangePlanNormalContractFeeCurrencyType)
                    //    || param.updateItemList.Exists(x => x.BillingContractFeeCurrencyType != regisObj.ChangePlanNormalContractFeeCurrencyType))
                    //    && regisObj.ChangePlanApproveInstallationFee != 0)
                    //    isSameContractFeeCurrency = false;
                    if ((param.newItemList.Exists(x =>
                            x.BillingContractFeeCurrencyType != regisObj.ChangePlanOrderContractFeeCurrencyType
                            && x.BillingContractFee > 0)
                        || param.updateItemList.Exists(x =>
                            x.BillingContractFeeCurrencyType != regisObj.ChangePlanOrderContractFeeCurrencyType
                            && x.BillingContractFee > 0))
                        && regisObj.ChangePlanApproveInstallationFee != 0)
                        isSameContractFeeCurrency = false;
                }

                sumContractFee = sumContractFee +
                    param.newItemList.FindAll(x => x.BillingContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                        .Sum(x => x.BillingContractFee.GetValueOrDefault()) +
                    param.updateItemList.FindAll(x => x.BillingContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                        .Sum(x => x.BillingContractFee.GetValueOrDefault());
                sumContractFeeUS = sumContractFeeUS +
                    param.newItemList.FindAll(x => x.BillingContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        .Sum(x => x.BillingContractFee.GetValueOrDefault()) +
                    param.updateItemList.FindAll(x => x.BillingContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        .Sum(x => x.BillingContractFee.GetValueOrDefault());

                #endregion

                int useInstallType = 1; 
                if ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                                ((param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (param.StartType == StartType.C_START_TYPE_ALTER_START)))
                {
                    useInstallType = 1;

                    #region Summary Approve Installation Fee

                    if (isSameInstallApproveCurrency == true)
                    {
                        if ((param.newItemList.Exists(x =>
                                x.BillingApproveInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType
                                && x.BillingApproveInstallationFee > 0)
                            || param.updateItemList.Exists(x =>
                                x.BillingApproveInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType
                                && x.BillingApproveInstallationFee > 0))
                            && regisObj.ChangePlanApproveInstallationFee > 0)
                            isSameInstallApproveCurrency = false;
                    }

                    sumApproveContract = sumApproveContract +
                        param.newItemList.FindAll(x => x.BillingApproveInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                            .Sum(x => x.BillingApproveInstallationFee.GetValueOrDefault()) +
                        param.updateItemList.FindAll(x => x.BillingApproveInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                            .Sum(x => x.BillingApproveInstallationFee.GetValueOrDefault());
                    sumApproveContractUS = sumApproveContractUS +
                        param.newItemList.FindAll(x => x.BillingApproveInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            .Sum(x => x.BillingApproveInstallationFee.GetValueOrDefault()) +
                        param.updateItemList.FindAll(x => x.BillingApproveInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            .Sum(x => x.BillingApproveInstallationFee.GetValueOrDefault());
                    #endregion
                    #region Summary Complete Installation Fee

                    if (isSameInstallCompleteCurrency == true)
                    {
                        if ((param.newItemList.Exists(x =>
                                x.BillingCompleteInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType
                                && x.BillingCompleteInstallationFee > 0)
                            || param.updateItemList.Exists(x =>
                                x.BillingCompleteInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType
                                && x.BillingCompleteInstallationFee > 0))
                            && regisObj.ChangePlanCompleteInstallationFee > 0)
                            isSameInstallCompleteCurrency = false;
                    }

                    sumCompleteInstallationFee = sumCompleteInstallationFee +
                        param.newItemList.FindAll(x => x.BillingCompleteInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                            .Sum(x => x.BillingCompleteInstallationFee.GetValueOrDefault()) +
                        param.updateItemList.FindAll(x => x.BillingCompleteInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                            .Sum(x => x.BillingCompleteInstallationFee.GetValueOrDefault());
                    sumCompleteInstallationFeeUS = sumCompleteInstallationFeeUS +
                        param.newItemList.FindAll(x => x.BillingCompleteInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            .Sum(x => x.BillingCompleteInstallationFee.GetValueOrDefault()) +
                        param.updateItemList.FindAll(x => x.BillingCompleteInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            .Sum(x => x.BillingCompleteInstallationFee.GetValueOrDefault());

                    #endregion
                    #region Summary Start Service Fee

                    if (isSameInstallStartServiceCurrency == true)
                    {
                        if ((param.newItemList.Exists(x =>
                                x.BillingStartInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType
                                && x.BillingCompleteInstallationFee > 0)
                            || param.updateItemList.Exists(x =>
                                x.BillingStartInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType
                                && x.BillingCompleteInstallationFee > 0))
                            && regisObj.ChangePlanStartInstallationFee > 0)
                            isSameInstallStartServiceCurrency = false;
                    }

                    sumStartServiceFee = sumStartServiceFee +
                        param.newItemList.FindAll(x => x.BillingStartInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                            .Sum(x => x.BillingStartInstallationFee.GetValueOrDefault()) +
                        param.updateItemList.FindAll(x => x.BillingStartInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                            .Sum(x => x.BillingStartInstallationFee.GetValueOrDefault());
                    sumStartServiceFeeUS = sumStartServiceFeeUS +
                        param.newItemList.FindAll(x => x.BillingStartInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            .Sum(x => x.BillingStartInstallationFee.GetValueOrDefault()) +
                        param.updateItemList.FindAll(x => x.BillingStartInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            .Sum(x => x.BillingStartInstallationFee.GetValueOrDefault());

                    #endregion
                }
                else if (param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                               || param.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
                           )
                {
                    useInstallType = 2;

                    #region Summary Installation Fee

                    if (isSameInstallCurrency == true)
                    {
                        if ((param.newItemList.Exists(x =>
                                x.BillingInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType
                                && x.BillingInstallationFee > 0)
                            || param.updateItemList.Exists(x =>
                                x.BillingInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType
                                && x.BillingInstallationFee > 0))
                            && regisObj.ChangePlanCompleteInstallationFee > 0)
                            isSameInstallCurrency = false;
                    }

                    sumInstallationFee = sumInstallationFee +
                    param.newItemList.FindAll(x => x.BillingInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                        .Sum(x => x.BillingInstallationFee.GetValueOrDefault()) +
                    param.updateItemList.FindAll(x => x.BillingInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                        .Sum(x => x.BillingInstallationFee.GetValueOrDefault());
                    sumInstallationFeeUS = sumInstallationFeeUS +
                        param.newItemList.FindAll(x => x.BillingInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            .Sum(x => x.BillingInstallationFee.GetValueOrDefault()) +
                        param.updateItemList.FindAll(x => x.BillingInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            .Sum(x => x.BillingInstallationFee.GetValueOrDefault());

                    #endregion
                }
                                
                #region Summary Deposit Fee

                sumDepositFee = sumDepositFee + 
                    param.newItemList.FindAll(x => x.BillingDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                        .Sum(x => x.BillingDepositFee.GetValueOrDefault()) + 
                    param.updateItemList.FindAll(x => x.BillingDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                                        .Sum(x => x.BillingDepositFee.GetValueOrDefault());
                sumDepositFeeUS = sumDepositFeeUS +
                    param.newItemList.FindAll(x => x.BillingDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        .Sum(x => x.BillingDepositFee.GetValueOrDefault()) +
                    param.updateItemList.FindAll(x => x.BillingDepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        .Sum(x => x.BillingDepositFee.GetValueOrDefault());

                #endregion

                if (regisObj.DivideContractFeeBillingFlag
                    && (regisObj.ChangePlanOrderContractFee.GetValueOrDefault() > 0)
                    && (cntContractFeeOverZero <= 1))
                {
                    
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3010, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (!regisObj.DivideContractFeeBillingFlag
                    && (regisObj.ChangePlanOrderContractFee.GetValueOrDefault() > 0)
                    && (cntContractFeeOverZero > 1))
                {

                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3011, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if ((dsRentalContractData.dtTbt_RentalMaintenanceDetails.Count == 1) && (dsRentalContractData.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
                    && (cntContractFeeOverZero > 1))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3150, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                bool checkSum = true;
                if(((regisObj.ChangePlanOrderInstallationFeeCurrencyType != regisObj.ChangePlanApproveInstallationFeeCurrencyType) && regisObj.ChangePlanApproveInstallationFee > 0) ||
                   ((regisObj.ChangePlanOrderInstallationFeeCurrencyType != regisObj.ChangePlanCompleteInstallationFeeCurrencyType) && regisObj.ChangePlanCompleteInstallationFee > 0) ||
                   ((regisObj.ChangePlanOrderInstallationFeeCurrencyType != regisObj.ChangePlanStartInstallationFeeCurrencyType) && regisObj.ChangePlanStartInstallationFee > 0))
                {
                    checkSum = false;
                }

                if (regisObj.ChangePlanOrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (sumContractFee == 0 && sumContractFeeUS >= 0 && regisObj.ChangePlanOrderContractFee.GetValueOrDefault() != sumContractFeeUS)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3012, null, new string[] { "ChangePlanOrderContractFee" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if(checkSum)
                    {
                    if (useInstallType == 1)
                    {
                        if (sumCompleteInstallationFee == 0
                        && sumApproveContract == 0
                        && sumStartServiceFee == 0
                        && sumCompleteInstallationFeeUS >= 0
                        && sumApproveContractUS >= 0
                        && sumStartServiceFeeUS >= 0
                        && (regisObj.ChangePlanOrderInstallationFee.GetValueOrDefault() !=
                                (sumCompleteInstallationFeeUS + sumApproveContractUS + sumStartServiceFeeUS)))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3013, null, new string[] { "ChangePlanOrderInstallationFee" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    }
                    else
                    {
                        if (sumInstallationFee == 0
                            && sumInstallationFeeUS >= 0
                            && (regisObj.ChangePlanOrderInstallationFee.GetValueOrDefault() != sumInstallationFeeUS))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3013, null, new string[] { "ChangePlanOrderInstallationFee" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    }
                    }
                }
                else
                {
                    if (sumContractFee >= 0 && sumContractFeeUS == 0
                           && regisObj.ChangePlanOrderContractFee.GetValueOrDefault() != sumContractFee)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3012, null, new string[] { "ChangePlanOrderContractFee" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if(checkSum)
                    {
                    if (useInstallType == 1)
                    {
                        if (sumCompleteInstallationFee >= 0
                        && sumApproveContract >= 0
                        && sumStartServiceFee >= 0
                        && sumCompleteInstallationFeeUS == 0
                        && sumApproveContractUS == 0
                        && sumStartServiceFeeUS == 0
                        && (regisObj.ChangePlanOrderInstallationFee.GetValueOrDefault() !=
                                (sumCompleteInstallationFee + sumApproveContract + sumStartServiceFee)))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3013, null, new string[] { "ChangePlanOrderInstallationFee" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    }
                    else
                    {
                        if (sumInstallationFee >= 0
                            && sumInstallationFeeUS == 0
                            && (regisObj.ChangePlanOrderInstallationFee.GetValueOrDefault() != sumInstallationFee))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3013, null, new string[] { "ChangePlanOrderInstallationFee" });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    }
                }
                }

                if (isSameInstallCompleteCurrency == false
                    || isSameInstallCurrency == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3314, null, new string[] { "ChangePlanCompleteInstallationFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    //return res;
                }
                if (isSameInstallStartServiceCurrency == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3315, null, new string[] { "ChangePlanStartInstallationFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    //return res;
                }

                if (isSameInstallApproveCurrency == false
                    || isSameInstallCompleteCurrency == false
                    || isSameInstallStartServiceCurrency == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3346, null, new string[] { "ChangePlanOrderContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    //return res;
                }

                if (regisObj.ChangePlanNormalContractFeeCurrencyType != regisObj.ChangePlanOrderContractFeeCurrencyType)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3347, null, new string[] { "ChangePlanOrderContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }
                if (regisObj.ChangePlanNormalInstallationFeeCurrencyType != regisObj.ChangePlanOrderInstallationFeeCurrencyType)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3348, null, new string[] { "ChangePlanOrderContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }

                if (isSameContractFeeCurrency == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3340, null, new string[] { "ChangePlanOrderContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    //SECOM-SIMS.INDO-BGL.Thanawit.xls #2 : Show warning but also allow users to passthrough.
                    //return res;
                }
                if (isSameInstallApproveCurrency == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3341, null, new string[] { "ChangePlanCompleteInstallationFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    //return res;
                }
                //if (isSameInstallFeeAtApproveCurrency == false)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3316);
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                //    //return res;
                //}

                //if ((billingTmpFull.Exists(x => x.BillingAmtCurrencyType != regisObj.ChangePlanNormalContractFeeCurrencyType) == false
                //        && param.newItemList.Exists(x => x.BillingContractFeeCurrencyType != regisObj.ChangePlanNormalContractFeeCurrencyType) == false
                //        && param.updateItemList.Exists(x => x.BillingContractFeeCurrencyType != regisObj.ChangePlanNormalContractFeeCurrencyType) == false)
                //        && regisObj.ChangePlanApproveInstallationFee != 0)
                //{
                //    isSameContractFeeCurrency = true;
                //}
                if ((billingTmpFull.Exists(x => x.BillingAmtCurrencyType != regisObj.ChangePlanOrderContractFeeCurrencyType
                                                && x.BillingAmt > 0) == false
                        && param.newItemList.Exists(x => x.BillingContractFeeCurrencyType != regisObj.ChangePlanOrderContractFeeCurrencyType
                                                            && x.BillingContractFee > 0) == false
                        && param.updateItemList.Exists(x => x.BillingContractFeeCurrencyType != regisObj.ChangePlanOrderContractFeeCurrencyType
                                                                && x.BillingContractFee > 0) == false)
                        && regisObj.ChangePlanApproveInstallationFee != 0)
                {
                    isSameContractFeeCurrency = true;
                }

                //foreach (var updItem in param.updateItemList)
                //{
                //    if (updItem.BillingContractFee.GetValueOrDefault() != regisObj.ChangePlanOrderContractFee.GetValueOrDefault())
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3012, null, new string[] { "ChangePlanOrderContractFee" });
                //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //        return res;
                //    }
                //}

                if ((dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                    ((dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (dsRentalContractData.dtTbt_RentalContractBasic[0].StartType == StartType.C_START_TYPE_ALTER_START)))
                {
                    //if (regisObj.ChangePlanCompleteInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    //{
                    //    if (sumCompleteInstallationFee == 0 && sumCompleteInstallationFeeUS >= 0
                    //    && regisObj.ChangePlanCompleteInstallationFee.GetValueOrDefault() != sumCompleteInstallationFeeUS)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3027, null, new string[] { "ChangePlanCompleteInstallationFee" });
                    //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //        return res;
                    //    }
                    //}
                    //else
                    //{
                    //    if (sumCompleteInstallationFee >= 0 && sumCompleteInstallationFeeUS == 0
                    //    && regisObj.ChangePlanCompleteInstallationFee.GetValueOrDefault() != sumCompleteInstallationFee)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3027, null, new string[] { "ChangePlanCompleteInstallationFee" });
                    //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //        return res;
                    //    }
                    //}

                    //if (regisObj.ChangePlanStartInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    //{
                    //    if (sumCompleteInstallationFee == 0 && sumCompleteInstallationFeeUS >= 0
                    //        && regisObj.ChangePlanStartInstallationFee.GetValueOrDefault() != sumStartServiceFeeUS)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3028, null, new string[] { "ChangePlanStartInstallationFee" });
                    //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //        return res;
                    //    }
                    //}
                    //else
                    //{
                    //    if (sumCompleteInstallationFee >= 0 && sumCompleteInstallationFeeUS == 0
                    //        && regisObj.ChangePlanStartInstallationFee.GetValueOrDefault() != sumStartServiceFee)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3028, null, new string[] { "ChangePlanStartInstallationFee" });
                    //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //        return res;
                    //    }
                    //}
                }
                else if (dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                    || dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                {
                    //if (regisObj.ChangePlanCompleteInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    //{
                    //    if (sumCompleteInstallationFee == 0 && sumCompleteInstallationFeeUS >= 0
                    //        && regisObj.ChangePlanCompleteInstallationFee.GetValueOrDefault() != sumInstallationFeeUS)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3027, null, new string[] { "ChangePlanCompleteInstallationFee" });
                    //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //        return res;
                    //    }
                    //}
                    //else
                    //{
                    //    if (sumCompleteInstallationFee >= 0 && sumCompleteInstallationFeeUS == 0
                    //                                && regisObj.ChangePlanCompleteInstallationFee.GetValueOrDefault() != sumInstallationFee)
                    //    {
                    //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3027, null, new string[] { "ChangePlanCompleteInstallationFee" });
                    //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //        return res;
                    //    }
                    //}                 
                }

                // Validate 4.10 wait for P'Am
                // Get MA Contract Code List
                //var maContractCode = from a in dsRentalContractData.dtTbt_RelationType
                //                     where
                //                         (a.RelationType == RelationType.C_RELATION_TYPE_MA)
                //                     select a.RelatedContractCode;


                var maContractCode = from a in dsQuotation.dtTbt_QuotationMaintenanceLinkage
                                     select a.ContractCode;

                // Update Date: 27/04/2012 10:36
                // Update by: Warakorn M.
                // --------------------------------------------------------------------
                //var maCheckRes = contracthandler.CheckMaintenanceTargetContractList(maContractCode.ToList(), dsQuotation.dtTbt_QuotationBasic.QuotationTargetCode);
                //if (!maCheckRes.IsError && maCheckRes.HasResultData)
                //{
                //    List<doContractHeader> contractHeader = (List<doContractHeader>)maCheckRes.ResultData;
                //    var notMatchSiteCode = from a in contractHeader where a.SiteCode != dsRentalContractData.dtTbt_RentalContractBasic[0].SiteCode select a;

                //    if (notMatchSiteCode.Count() > 0)
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3093, null, null);
                //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //        return res;
                //    }
                //}
                //else if (maCheckRes.IsError)
                //{
                //    if (res.IsError)
                //    {
                //        res.MessageList.AddRange(maCheckRes.MessageList);
                //    }
                //    else
                //    {
                //        res = maCheckRes;
                //    }

                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    return res;
                //}
                try
                {
                    List<doContractHeader> contLst = contracthandler.CheckMaintenanceTargetContractList(maContractCode.ToList(), dsQuotation.dtTbt_QuotationBasic.QuotationTargetCode); ;
                    if (contLst.Count > 0)
                    {
                        var notMatchSiteCode = from a in contLst
                                               where a.SiteCode != dsRentalContractData.dtTbt_RentalContractBasic[0].SiteCode
                                               select a;

                        if (notMatchSiteCode.Count() > 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3093, null, null);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return res;
                        }
                    }
                }
                catch (ApplicationErrorException ex)
                {
                    if (res.IsError)
                        res.MessageList.AddRange(ex.ErrorResult.MessageList);
                    else
                        res.AddErrorMessage(ex);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }
                // --------------------------------------------------------------------

                if (HasChangePlanDetailTask)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3253, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                // validateBusinessForWarning
                //DateTime currentDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                //if (regisObj.ExpectedOperationDate < currentDay)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3015, null, null);
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                //    res.ResultData = true;
                //    //return res;
                //}

                if (regisObj.ChangePlanNormalContractFeeCurrencyType == regisObj.ChangePlanOrderContractFeeCurrencyType)
                {
                    if ((regisObj.ChangePlanNormalContractFee > 0)
                        && (regisObj.ChangePlanOrderContractFee == 0))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3270, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.ResultData = true;
                        //return res;
                    }

                    var percentOfNormalContractFee_1000 = regisObj.ChangePlanNormalContractFee * 1000 / 100;
                    //var percentOfNormalContractFee_10 = regisObj.ChangePlanNormalContractFee * 10 / 100;

                    if ((regisObj.ChangePlanNormalContractFee.GetValueOrDefault() > 0)
                        && (regisObj.ChangePlanOrderContractFee.GetValueOrDefault() > 0)
                        && ((regisObj.ChangePlanOrderContractFee >= percentOfNormalContractFee_1000)
                        /*|| (regisObj.ChangePlanOrderContractFee <= percentOfNormalContractFee_10)*/))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3271, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        res.ResultData = true;
                        //return res;
                    }
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Register ChangePlan process
        /// </summary>
        /// <param name="regisObj"></param>
        /// <returns></returns>
        private bool RegisterChangePlan_CTS051(CTS051_RegisterChangePlan regisObj)
        {
            bool res = false;
            CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();
            CommonUtil util = new CommonUtil();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            IQuotationHandler quotationhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

            // Retrieve all data
            List<tbt_BillingTemp> billingTemp = new List<tbt_BillingTemp>();
            List<tbm_BillingClient> billingClient = new List<tbm_BillingClient>();
            var dsRentalContractData = rentalhandler.GetEntireContract(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.ContractOCC);
            var dsQuotation = quotationhandler.GetQuotationData(new doGetQuotationDataCondition()
            {
                Alphabet = param.Alphabet,
                QuotationTargetCode = param.QuotationCode,
                ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL,
                TargetCodeTypeCode = TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE,
                ContractFlag = true,
            });

            rentalhandler.MapFromQuotation(dsQuotation, ref dsRentalContractData);

            // Mapping data from screen to object
            //dsRentalContractData.dtTbt_RentalSecurityBasic[0].ExpectedOperationDate = regisObj.ExpectedOperationDate;

            #region Normal Contract Fee

            dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalContractFeeCurrencyType = regisObj.ChangePlanNormalContractFeeCurrencyType;
            if (regisObj.ChangePlanNormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalContractFeeUsd = regisObj.ChangePlanNormalContractFee;
            else
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalContractFee = regisObj.ChangePlanNormalContractFee;

            #endregion
            #region Normal Installation Fee

            dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalInstallFeeCurrencyType = regisObj.ChangePlanNormalInstallationFeeCurrencyType;
            if (regisObj.ChangePlanNormalInstallationFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalInstallFeeUsd = regisObj.ChangePlanNormalInstallationFee;
            else
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalInstallFee = regisObj.ChangePlanNormalInstallationFee;

            #endregion

            if (dsRentalContractData.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == true
                || ( ( dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                        || dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE
                        || dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG
                        || dsRentalContractData.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                    && dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START))
            {
                #region Normal Additional Deposit Fee

                dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFeeCurrencyType = null;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee = null;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFeeUsd = null;

                #endregion
                #region Order Additional Deposit Fee

                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFeeCurrencyType = null;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee = null;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFeeUsd = null;

                #endregion
                
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming = null;
            }

            #region Order Contract Fee

            dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType = regisObj.ChangePlanOrderContractFeeCurrencyType;
            if (dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
            {
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd = regisObj.ChangePlanOrderContractFee;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFee = null;
            } 
            else
            {

                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFee = regisObj.ChangePlanOrderContractFee;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd = null;
            }

            #endregion
            #region Order Installation Fee

            dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFeeCurrencyType = regisObj.ChangePlanOrderInstallationFeeCurrencyType;
            if (dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
            {
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFeeUsd = regisObj.ChangePlanOrderInstallationFee;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee = null;
            }
            else
            {
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee = regisObj.ChangePlanOrderInstallationFee;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFeeUsd = null;
            }

            #endregion
            #region Order Install Fee > Complete Install

            dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallCurrencyType = regisObj.ChangePlanCompleteInstallationFeeCurrencyType;
            if (dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
            {
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallUsd = regisObj.ChangePlanCompleteInstallationFee;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = null;
            }  
            else
            {
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = regisObj.ChangePlanCompleteInstallationFee;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallUsd = null;
            }
                

            #endregion
            #region Order Install Fee > Start Service

            dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceCurrencyType = regisObj.ChangePlanStartInstallationFeeCurrencyType;
            if (dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
            {
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceUsd = regisObj.ChangePlanStartInstallationFee;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService = null;
            }  
            else
            {
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService = regisObj.ChangePlanStartInstallationFee;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceUsd = null;
            }
                

            #endregion
            #region Order Install Fee > Approve Contract

            dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractCurrencyType = regisObj.ChangePlanApproveInstallationFeeCurrencyType;
            if (dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
            {
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractUsd = regisObj.ChangePlanApproveInstallationFee;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract = null;
            }   
            else
            {
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract = regisObj.ChangePlanApproveInstallationFee;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractUsd = null;
            } 

            #endregion
            
            dsRentalContractData.dtTbt_RentalSecurityBasic[0].ApproveNo1 = regisObj.ApproveNo1;
            dsRentalContractData.dtTbt_RentalSecurityBasic[0].ApproveNo2 = regisObj.ApproveNo2;
            dsRentalContractData.dtTbt_RentalSecurityBasic[0].ApproveNo3 = regisObj.ApproveNo3;
            dsRentalContractData.dtTbt_RentalSecurityBasic[0].ApproveNo4 = regisObj.ApproveNo4;
            dsRentalContractData.dtTbt_RentalSecurityBasic[0].ApproveNo5 = regisObj.ApproveNo5;

            dsRentalContractData.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1 = regisObj.NegotiationStaffEmpNo1;
            dsRentalContractData.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo2 = regisObj.NegotiationStaffEmpNo2;

            //dsQuotation.dtTbt_QuotationBasic.ContractDurationMonth = regisObj.ContractDurationMonth;
            //dsQuotation.dtTbt_QuotationBasic.AutoRenewMonth = regisObj.AutoRenewMonth;
            //dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractEndDate = regisObj.EndContractDate;

            //Update other information
            if (dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_BEF_START
                && dsRentalContractData.dtTbt_RentalContractBasic[0].StartType != StartType.C_START_TYPE_ALTER_START)
            {
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo1;
                dsRentalContractData.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2 = dsQuotation.dtTbt_QuotationBasic.SalesmanEmpNo2;
            }

            // Prepare data
            bool hasChangeContractDuration = false;

            // Modify 2012-08-30 Phoomsak L. remove condition and set ExpectedOperationDate = ExpectedOperationDate on screen always
            dsRentalContractData.dtTbt_RentalSecurityBasic[0].ExpectedOperationDate = regisObj.ExpectedOperationDate;
            dsRentalContractData.dtTbt_RentalSecurityBasic[0].ExpectedResumeDate = null; //Add by Jutarat A. on 02122013

            //if ((dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
            //    ((dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (dsRentalContractData.dtTbt_RentalContractBasic[0].StartType == StartType.C_START_TYPE_ALTER_START)))
            //{
            //    dsRentalContractData.dtTbt_RentalSecurityBasic[0].ExpectedOperationDate = regisObj.ExpectedOperationDate;
            //}
            //else if (dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
            //    || dsRentalContractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
            //{
            //    dsRentalContractData.dtTbt_RentalSecurityBasic[0].ExpectedInstallationCompleteDate = regisObj.ExpectedOperationDate;
            //}

            if (regisObj.ContractDurationFlag)
            {
                var originCalContractEndDate = dsRentalContractData.dtTbt_RentalSecurityBasic[0].CalContractEndDate;

                if (regisObj.ContractDurationMonth.HasValue && regisObj.AutoRenewMonth.HasValue && !regisObj.EndContractDate.HasValue)
                {
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractDurationMonth = regisObj.ContractDurationMonth;
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].AutoRenewMonth = regisObj.AutoRenewMonth;
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractEndDate = null;

                    if (dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractStartDate != null)
                    {
                        dsRentalContractData.dtTbt_RentalSecurityBasic[0].CalContractEndDate = dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractStartDate.GetValueOrDefault().AddMonths(regisObj.ContractDurationMonth.GetValueOrDefault()).AddDays(-1);
                    }

                } else if (!regisObj.ContractDurationMonth.HasValue && regisObj.AutoRenewMonth.HasValue && regisObj.EndContractDate.HasValue)
                {
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractDurationMonth = null;
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].AutoRenewMonth = regisObj.AutoRenewMonth;
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractEndDate = regisObj.EndContractDate;
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].CalContractEndDate = regisObj.EndContractDate;
                } else if (!regisObj.ContractDurationMonth.HasValue && !regisObj.AutoRenewMonth.HasValue && regisObj.EndContractDate.HasValue)
                {
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractDurationMonth = null;
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].AutoRenewMonth = null;
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].ContractEndDate = regisObj.EndContractDate;
                    dsRentalContractData.dtTbt_RentalSecurityBasic[0].CalContractEndDate = regisObj.EndContractDate;
                }

                if (originCalContractEndDate != dsRentalContractData.dtTbt_RentalSecurityBasic[0].CalContractEndDate)
                {
                    hasChangeContractDuration = true;
                }
            }

            res = rentalhandler.RegisterChangePlan(dsRentalContractData, dsQuotation, param.newItemList, param.updateItemList, param.deleteItemList, hasChangeContractDuration, param.ContractOCC);

            return res;
        }
        #endregion

        #region Old
        private CTS051_ScreenParameter CTS051_GetImportData(string key = null)
        {
            try
            {
                return GetScreenObject<CTS051_ScreenParameter>(key);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
