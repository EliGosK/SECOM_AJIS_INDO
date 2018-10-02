//*********************************
// Create by: Natthavat S.
// Create date: 26/APR/2012
// Update date: 26/APR/2012
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
using System.Transactions;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Installation;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check system suspending, user’s permission and user’s authority of screen
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS053_Authority(CTS053_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

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

                res = ValidateAuthority_CTS053(res, param);
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;

                if (res.IsError)
                {
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS053_ScreenParameter>("CTS053", param, res);
        }
        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS053")]
        public ActionResult CTS053()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();

            var confixLst = commonhandler.GetSystemConfig(ConfigName.C_EMAIL_SUFFIX);
            string mailsuffix = (confixLst.Count == 1) ? confixLst[0].ConfigValue : "";
            ViewBag.EmailSuffix = mailsuffix;

            param.mailList = new List<dtEmailAddress>();

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
        /// Load data of Contract when initial screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS053_RetrieveContractData()
        {
            ObjectResultData res = new ObjectResultData();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            ICustomerMasterHandler custhandler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;
            ISiteMasterHandler sitehandler = ServiceContainer.GetService<ISiteMasterHandler>() as ISiteMasterHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

            try
            {
                CommonUtil util = new CommonUtil();
                CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();
                CTS053_DisplayObject obj = new CTS053_DisplayObject();
                CTS053_ContractDisplayData contractObj = new CTS053_ContractDisplayData();
                CTS053_ChangeContractFeeDisplayData changeFeeObj = new CTS053_ChangeContractFeeDisplayData();

                var dsRentalContract = rentalhandler.GetEntireContract(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                var custData = custhandler.GetCustomer(dsRentalContract.dtTbt_RentalContractBasic[0].ContractTargetCustCode);
                var siteData = sitehandler.GetTbm_Site(dsRentalContract.dtTbt_RentalContractBasic[0].SiteCode);
                var officeData = officehandler.GetTbm_Office(dsRentalContract.dtTbt_RentalContractBasic[0].OperationOfficeCode);

                CommonUtil.MappingObjectLanguage<tbm_Office>(officeData);

                param.OCC = dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC;

                contractObj = new CTS053_ContractDisplayData()
                {
                    ContractCode = util.ConvertContractCode(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    UserCode = dsRentalContract.dtTbt_RentalContractBasic[0].UserCode,
                    CustCode = util.ConvertCustCode(dsRentalContract.dtTbt_RentalContractBasic[0].ContractTargetCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    EndUserCode = util.ConvertCustCode(dsRentalContract.dtTbt_RentalContractBasic[0].RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    SiteCode = util.ConvertSiteCode(dsRentalContract.dtTbt_RentalContractBasic[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    IsImportantCustomer = custData[0].ImportantFlag.GetValueOrDefault(),
                    CustNameEN = custData[0].CustFullNameEN,
                    CustNameLC = custData[0].CustFullNameLC,
                    CustAddressEN = custData[0].AddressFullEN,
                    CustAddressLC = custData[0].AddressFullLC,
                    SiteNameEN = siteData[0].SiteNameEN,
                    SiteNameLC = siteData[0].SiteNameLC,
                    SiteAddressEN = siteData[0].AddressFullEN,
                    SiteAddressLC = siteData[0].AddressFullLC,
                    OperationOffice = officeData[0].OfficeDisplay
                };

                string currentContractFeeCurrency;
                decimal? changeContractFee_Amount = 0;
                if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                {
                    currentContractFeeCurrency = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopCurrencyType;
                    if (dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        changeContractFee_Amount = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopUsd;
                    else
                        changeContractFee_Amount = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop;
                }
                else
                {
                    currentContractFeeCurrency = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType;
                    if (dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                        changeContractFee_Amount = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd;
                    else
                        changeContractFee_Amount = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee;
                }

                changeFeeObj = new CTS053_ChangeContractFeeDisplayData()
                {
                    ChangeDateContractFee = null,
                    CurrentContractFeeCurrencyType = currentContractFeeCurrency,
                    CurrentContractFee = CommonUtil.TextNumeric(changeContractFee_Amount),
                    ChangeContractFeeCurrencyType = currentContractFeeCurrency,
                    ChangeContractFee = null,
                    ChangeFeeFlag = false,
                    ReturnToOriginalFeeDate = null,
                    //ApproveNo1 = dsRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo1,
                    //ApproveNo2 = dsRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo2,
                    //ApproveNo3 = dsRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo3,
                    //ApproveNo4 = dsRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo4,
                    //ApproveNo5 = dsRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo5,
                    ApproveNo1 = null,
                    ApproveNo2 = null,
                    ApproveNo3 = null,
                    ApproveNo4 = null,
                    ApproveNo5 = null,
                    NegotiationStaffEmpName1 = null,
                    NegotiationStaffEmpName2 = null,
                    NegotiationStaffEmpNo1 = null,
                    NegotiationStaffEmpNo2 = null,
                    IsDisableDivideFlag = ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                    && (dsRentalContract.dtTbt_RentalMaintenanceDetails != null)
                    && (dsRentalContract.dtTbt_RentalMaintenanceDetails.Count > 0)
                    && (dsRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED))
                };

                obj.ChangeFeeData = changeFeeObj;
                obj.ContractData = contractObj;

                res.ResultData = obj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial grid NotifyEmail
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS053_InitialNotifyEmailGrid()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res.ResultData = CommonUtil.ConvertToXml<object>(null, "Contract\\CTS053Email", CommonUtil.GRID_EMPTY_TYPE.VIEW); ;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial grid BillingTemp
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS053_InitialBillingTempGrid()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res.ResultData = CommonUtil.ConvertToXml<object>(null, "Contract\\CTS053Billing", CommonUtil.GRID_EMPTY_TYPE.VIEW); ;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        
        /// <summary>
        /// Reload data to BillingTemp grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS053_RetrieveBillingTempGrid()
        {
            ObjectResultData res = new ObjectResultData();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            CommonUtil util = new CommonUtil();

            try
            {
                CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();
                List<CTS053_ChangePlanGrid> obj = new List<CTS053_ChangePlanGrid>();

                var billingTempData = rentalhandler.GetBillingTempForChangeFee(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                foreach (var billingTempItem in billingTempData)
                {
                    // is delete item ?
                    var deleteLst = from a in param.deleteItemList
                                    where
                                        (a.OriginalBillingOCC == billingTempItem.BillingOCC)
                                        && (a.OriginalBillingOfficeCode == billingTempItem.BillingOfficeCode)
                                        && (a.OriginalBillingClientCode == billingTempItem.BillingClientCode)
                                    select a;

                    if (deleteLst.Count() == 0)
                    {
                        // is update item ?

                        var updateLst = from a in param.updateItemList
                                        where
                                            (a.OriginalBillingOCC == billingTempItem.BillingOCC)
                                            && (a.OriginalBillingOfficeCode == billingTempItem.BillingOfficeCode)
                                            && (a.OriginalBillingClientCode == billingTempItem.BillingClientCode)
                                        select a;

                        if (updateLst.Count() == 0)
                        {
                            // use original item
                            obj.Add(CreateChangePlanObject_CTS053(billingTempItem));
                        }
                        else
                        {
                            obj.Add(CreateChangePlanObject_CTS053(updateLst.First()));
                        }
                    }
                }

                // concat with new item
                foreach (var newItem in param.newItemList)
                {
                    obj.Add(CreateChangePlanObject_CTS053(newItem));
                }

                res.ResultData = CommonUtil.ConvertToXml<CTS053_ChangePlanGrid>(obj, "Contract\\CTS053Billing", CommonUtil.GRID_EMPTY_TYPE.VIEW); ;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Reload data to Email grid
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS053_RetrieveEmailGrid()
        {
            ObjectResultData res = new ObjectResultData();
            CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();

            try
            {
                res.ResultData = CommonUtil.ConvertToXml<dtEmailAddress>(param.mailList, "Contract\\CTS053Email", CommonUtil.GRID_EMPTY_TYPE.VIEW); ;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Retrive email when click [Search email] button in ‘Notify target of return date to original fee’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS053_RetrieveEmailList()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();
                var mailList = from a in param.mailList select a;

                res.ResultData = mailList.ToList();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        
        /// <summary>
        /// Add Email to Email grid when click [Add] button in ‘Notify target of return date to original fee’ section
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="fromSearch"></param>
        /// <returns></returns>
        public ActionResult CTS053_AddEmail(string[] emailAddress, bool fromSearch = false)
        {
            ObjectResultData res = new ObjectResultData();
            CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            IUserHandler userhandler = ServiceContainer.GetService<IUserHandler>() as IUserHandler;

            try
            {
                var confixLst = commonhandler.GetSystemConfig(ConfigName.C_EMAIL_SUFFIX);
                string mailsuffix = (confixLst.Count == 1) ? confixLst[0].ConfigValue : "";

                if ((emailAddress == null) || (emailAddress.Length == 0))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "lblEmailAddress" }, new string[] { "EmailAddress" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                if (fromSearch)
                {
                    param.mailList.Clear();
                }

                foreach (var emailItem in emailAddress)
                {
                    string fullEmailItem = (!fromSearch) ? emailItem.ToLower() + mailsuffix : emailItem.ToLower();

                    // validate email
                    //var validEmail = emphandler.GetEmailAddress(null, fullEmailItem, null, null);
                    //if ((validEmail == null) || (validEmail.Count() == 0))
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { fullEmailItem }, new string[] { "EmailAddress" });
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    return Json(res);
                    //}
                    var tmpMail = userhandler.GetUserEmailAddressDataList(new doEmailSearchCondition()
                    {
                        EmailEddress = fullEmailItem
                    });

                    if (tmpMail.Count < 1)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { fullEmailItem }, new string[] { "EmailAddress" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }

                    // check duplicate
                    var duplicateMail = param.mailList.Where(x => x.EmailAddress.ToLower() == fullEmailItem);
                    if ((duplicateMail == null) || (duplicateMail.Count() != 0))
                    //if (param.mailList.Contains(fullEmailItem))
                    {
                        // it is duplicate
                        // do nothing for now
                    }
                    else
                    {
                        param.mailList.Add(tmpMail[0]);
                        //param.mailList.Add(fullEmailItem);
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
        /// Remove Email from Email grid when click [Remove] button in Notify Email grid
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public ActionResult CTS053_RemoveEmail(string emailAddress)
        {
            ObjectResultData res = new ObjectResultData();
            CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();

            try
            {
                // remove email
                var targMail = from a in param.mailList where a.EmailAddress.ToLower() == emailAddress.ToLower() select a;

                if ((targMail != null) && (targMail.Count() == 1))
                {
                    param.mailList.Remove(targMail.First());
                }

                //if (param.mailList.Contains(emailAddress.ToLower()))
                //{
                //    param.mailList.Remove(emailAddress.ToLower());
                //}

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Clear Email textbox when click [Clear] button in ‘Notify target of return date to original fee’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS053_ClearEmail()
        {
            ObjectResultData res = new ObjectResultData();
            CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();

            try
            {
                // clear email
                param.mailList.Clear();
                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Load detail of BillingTemp when click [Detail] button in ‘Billing target’ section
        /// </summary>
        /// <param name="editItem"></param>
        /// <returns></returns>
        public ActionResult CTS053_LoadBillingTempData(dtBillingTemp_SetItem editItem)
        {
            ObjectResultData res = new ObjectResultData();
            CTS053_DisplayBillingTargetDetail obj = new CTS053_DisplayBillingTargetDetail();
            CommonUtil util = new CommonUtil();
            CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            IBillingMasterHandler billingmasterhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            try
            {
                int billingSequenceNo = 0;

                if (!String.IsNullOrEmpty(editItem.UID) && !editItem.IsNew)
                {
                    // Load data from grid
                    dtBillingClientData clientObj = null;
                    var billingTempData = rentalhandler.GetBillingTempForChangeFee(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                    var billingTempTargetData = from a in billingTempData
                                                where
                                                    (a.BillingOfficeCode == editItem.BillingOfficeCode)
                                                    //(a.BillingTargetCode == util.ConvertBillingTargetCode(editItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG))
                                                    && (a.BillingClientCode == util.ConvertBillingClientCode(editItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG))
                                                    && (a.BillingOCC == editItem.BillingOCC)
                                                select a;

                    string objectOfficeCode = "";
                    string billingContractFeeCurrency = null;
                    decimal? billingContractFee = null;

                    if ((billingTempTargetData != null) && (billingTempTargetData.Count() > 0))
                    {
                        var currentBillingTemp = billingTempTargetData.First();
                        billingSequenceNo = currentBillingTemp.SequenceNo.GetValueOrDefault();

                        billingContractFeeCurrency = currentBillingTemp.BillingAmtCurrencyType;
                        if (currentBillingTemp.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            billingContractFee = currentBillingTemp.BillingAmtUsd;
                        else
                            billingContractFee = currentBillingTemp.BillingAmt;

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

                    if (clientObj != null)
                    {
                        obj = new CTS053_DisplayBillingTargetDetail()
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
                            BillingOCC = editItem.BillingOCC,
                            CustomerType = clientObj.CustTypeCode,
                            
                            BillingContractFeeCurrencyType = billingContractFeeCurrency,
                            BillingContractFee = CommonUtil.TextNumeric(billingContractFee).Replace(",", ""),

                            ObjectType = 0, //Default
                            UID = editItem.UID,
                            OriginalBillingClientCode = clientObj.BillingClientCode,
                            OriginalBillingOCC = editItem.BillingOCC,
                            OriginalBillingOfficeCode = editItem.BillingOfficeCode,
                            HasFee = true,
                            //SequenceNo = billingSequenceNo,
                        };
                    }

                    var checkUpdateItem = from a in param.updateItemList
                                          where
                                              (a.OriginalBillingClientCode == editItem.OriginalBillingClientCode)
                                              && (a.OriginalBillingOfficeCode == editItem.OriginalBillingOfficeCode)
                                              && (a.OriginalBillingOCC == editItem.OriginalBillingOCC)
                                          select a;
                    if ((checkUpdateItem != null) && (checkUpdateItem.Count() > 0))
                    {
                        var updateItem = checkUpdateItem.First();
                        var newobj = new CTS053_EditBillingTargetDetail()
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
                            BillingOCC = editItem.BillingOCC,
                            CustomerType = updateItem.CustomerType,
                            ObjectType = 0, //Default
                            UID = editItem.UID,
                            HasFee = true,

                            BillingContractFeeCurrencyType = updateItem.BillingContractFeeCurrencyType,
                            BillingContractFee = updateItem.BillingContractFee,

                            OldOfficeCode = updateItem.BillingOffice,
                            OriginalBillingClientCode = updateItem.OriginalBillingClientCode,
                            OriginalBillingOCC = updateItem.OriginalBillingOCC,
                            OriginalBillingOfficeCode = updateItem.OriginalBillingOfficeCode
                            //SequenceNo = billingSequenceNo,
                        };
                        res.ResultData = newobj;
                    }
                    else
                    {
                        res.ResultData = obj;
                    }
                } else
                {
                    // Load from new Item
                    var checkNewItem = from a in param.newItemList
                                       where
                                           (a.UID == editItem.UID)
                                       select a;

                    if ((checkNewItem != null) && (checkNewItem.Count() > 0))
                    {
                        var newItem = checkNewItem.First();
                        var newobj = new CTS053_EditBillingTargetDetail()
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
                            BillingOCC = editItem.BillingOCC,
                            CustomerType = newItem.CustomerType,
                            ObjectType = newItem.ObjectType,
                            UID = newItem.UID,

                            BillingContractFeeCurrencyType = newItem.BillingContractFeeCurrencyType,
                            BillingContractFee = newItem.BillingContractFee,

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
        /// Show ‘Billing target detail’ section when click [New] button in ‘Billing target’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS053_NewBillingTempData()
        {
            ObjectResultData res = new ObjectResultData();
            CTS053_DisplayBillingTargetDetail obj = new CTS053_DisplayBillingTargetDetail();
            CommonUtil util = new CommonUtil();

            try
            {
                CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();

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
        /// Remove Billing item from BillingTarget grid when click [Remove] button in ‘Billing target’ section
        /// </summary>
        /// <param name="delItem"></param>
        /// <returns></returns>
        public ActionResult CTS053_DeleteBillingItem(dtBillingTemp_SetItem delItem)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

            try
            {
                // Checking item can delete
                CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();

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
                    var billingTempData = rentalhandler.GetBillingTempForChangeFee(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                    var targDeleteBillingTemp = from a in billingTempData
                                                where
                                                    (a.BillingTargetCode == util.ConvertBillingTargetCode(delItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG))
                                                    && (a.BillingClientCode == util.ConvertBillingClientCode(delItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG))
                                                    && (a.BillingOCC == delItem.BillingOCC)
                                                select a;
                    if ((targDeleteBillingTemp != null) && (billingTempData.Count() > 0))
                    {
                        if (String.IsNullOrEmpty(billingTempData[0].BillingOCC))
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
        /// Retrieve BillingDetail when click [Retrieve] button in ‘Specify code’ section
        /// </summary>
        /// <param name="BillingCodeSelected"></param>
        /// <param name="BillingTargetCode"></param>
        /// <param name="BillingClientCode"></param>
        /// <param name="CanCauseError"></param>
        /// <returns></returns>
        public ActionResult CTS053_RetrieveBillingTargetDetailFromCode(string BillingTargetCode, bool CanCauseError = true)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            CTS053_DisplayBillingTargetDetail obj = new CTS053_DisplayBillingTargetDetail();

            try
            {
                IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                IBillingMasterHandler billingmasterhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                string targClientCode = "";
                string objectOfficeCode = null;
                if (String.IsNullOrEmpty(BillingTargetCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS053", MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "lblBillingTargetCodeDetail" }, new string[] { "BillingTargetCodeSearch" });
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
                    obj = new CTS053_DisplayBillingTargetDetail()
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
        public ActionResult CTS053_RetrieveBillingClientDetailFromCode(string BillingClientCode, bool CanCauseError = true)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            CTS053_DisplayBillingTargetDetail obj = new CTS053_DisplayBillingTargetDetail();

            try
            {
                IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                IBillingMasterHandler billingmasterhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                string targClientCode = "";
                string objectOfficeCode = null;
                if (string.IsNullOrEmpty(BillingClientCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, "CTS053", MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "lblBillingClientCodeDetail" }, new string[] { "BillingClientCodeSearch" });
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
                    obj = new CTS053_DisplayBillingTargetDetail()
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
        public ActionResult CTS053_RetrieveBillingDetailFromCopy(string AddressCopyInfo)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            CTS053_DisplayBillingTargetDetail obj = new CTS053_DisplayBillingTargetDetail();
            IMasterHandler masterhandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

            try
            {
                CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();
                var regionLst = masterhandler.GetTbm_Region();
                var businessTypeLst = masterhandler.GetTbm_BusinessType();
                var contractData = rentalhandler.GetTbt_RentalContractBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                var contractObj = contractData[0];

                if (AddressCopyInfo == "0") // Contract target
                {
                    var cusData = masterhandler.GetTbm_Customer(contractObj.ContractTargetCustCode);
                    var businessTypeData = from a in businessTypeLst where a.BusinessTypeCode == cusData[0].BusinessTypeCode select a;
                    var regionData = from a in regionLst where a.RegionCode == cusData[0].RegionCode select a;
                    obj = new CTS053_DisplayBillingTargetDetail()
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
                    obj = new CTS053_DisplayBillingTargetDetail()
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
                    obj = new CTS053_DisplayBillingTargetDetail()
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
                    obj = new CTS053_DisplayBillingTargetDetail()
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
                        BranchNameLC = siteData[0].SiteNameLC,
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
        /// Validate Business when click [Add/Update] button in ‘Billing target detail’ section
        /// </summary>
        /// <param name="targObj"></param>
        /// <returns></returns>
        public ActionResult CTS053_ValidateBusiness_ChangePlanDetail(dtBillingTemp_SetItem targObj)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

            try
            {
                CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();
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
                    fieldName.Add("lblBillingOfficeDetail");
                    controlName.Add("BillingOffice");
                }

                if ((!targObj.BillingContractFee.HasValue))
                {
                    fieldName.Add("lblBillingContractFeeGrid");
                    controlName.Add("BillingContractFee");
                }

                foreach (var rawFieldName in fieldName.Distinct())
                {
                    realFieldName.Add(CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS053", rawFieldName));
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
                    }

                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                if ((String.IsNullOrEmpty(targObj.BillingOCC)) && (targObj.BillingContractFee.GetValueOrDefault() == 0))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3230, null, new string[] { "BillingContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                // Validate Business
                var billingTempData = rentalhandler.GetBillingTempForChangeFee(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));

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
                    var fullUpdateListDuplicate = from a in fullUpdateList
                                                  where
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

                if ((String.IsNullOrEmpty(targObj.BillingOCC)) && (!targObj.AmountTotal.HasValue && !targObj.BillingContractFee.HasValue && !targObj.BillingDepositFee.HasValue))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3087, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                var targChangePlanObj = CreateChangePlanObject_CTS053(targObj);

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
        public ActionResult CTS053_ValidateBusiness_All(CTS053_RegisterChangePlan regisObj, bool HasChangePlanDetailTask)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res = ValidateBusiness_CTS053(regisObj, HasChangePlanDetailTask);
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
        public ActionResult CTS053_ConfirmRegister(CTS053_RegisterChangePlan regisObj, bool HasChangePlanDetailTask)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();
                res = ValidateBusiness_CTS053(regisObj, HasChangePlanDetailTask);
                if (res.HasResultData && ((bool)res.ResultData))
                {
                    res.ResultData = null;
                    int regisResult = RegisterChangeFee_CTS053(regisObj);

                    if (regisResult == 1)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON
                                    , MessageUtil.MessageList.MSG0046
                                    , null);
                    }
                    else if (regisResult == 2)
                    {
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT
                                    , MessageUtil.MessageList.MSG3043
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

        /// <summary>
        /// Reset data of screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS053_ResetData()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();
                param.deleteItemList = new List<dtBillingTemp_SetItem>();
                param.mailList = new List<dtEmailAddress>();
                param.newItemList = new List<dtBillingTemp_SetItem>();
                param.updateItemList = new List<dtBillingTemp_SetItem>();
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
        private ObjectResultData ValidateAuthority_CTS053(ObjectResultData res, CTS053_ScreenParameter param, bool isInitScreen = true)
        {
            try
            {
                CommonUtil util = new CommonUtil();
                ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                IInstallationHandler installhandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                IUserControlHandler usercontrolhandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                string strLastOCC = "";

                if (CheckIsSuspending(res))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP12_CHANGE_FEE, FunctionID.C_FUNC_ID_OPERATE))
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

                //Add by Jutarat A. on 22052013
                if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == rentalObj[0].ContractOfficeCode; }).Count == 0
                    && CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == rentalObj[0].OperationOfficeCode; }).Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }
                //End Add

                //Comment by Jutarat A. on 22052013
                //var existsOperateOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == rentalObj[0].OperationOfficeCode);

                //if (rentalObj[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                //        && (existsOperateOffice.Count() <= 0)
                //    )
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063, null, null);
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    return res;
                //}
                //End Comment

                strLastOCC = rentalhandler.GetLastUnimplementedOCC(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                if (string.IsNullOrEmpty(strLastOCC))
                {
                    strLastOCC = rentalhandler.GetLastImplementedOCC(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                }

                var dsRentalContract = rentalhandler.GetEntireContract(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), strLastOCC);

                if (dsRentalContract.dtTbt_RentalSecurityBasic[0].ImplementFlag.GetValueOrDefault() == FlagType.C_FLAG_OFF)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3039, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                var installStatus = installhandler.GetInstallationStatus(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                if (installStatus != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3039, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (((dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_ALTERNATIVE_START)
                    || (dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT)
                    || (dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL)
                    || (dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                    || (dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_TERMINATED))
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3042, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }
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
        /// <param name="billingTempItem"></param>
        /// <returns></returns>
        private CTS053_ChangePlanGrid CreateChangePlanObject_CTS053(dtBillingTempChangeFeeData billingTempItem)
        {
            CTS053_ChangePlanGrid res = new CTS053_ChangePlanGrid();
            CommonUtil util = new CommonUtil();
            IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            IBillingClientMasterHandler billingclienthandler = ServiceContainer.GetService<IBillingClientMasterHandler>() as IBillingClientMasterHandler;

            string billingTargetnamePattern = "(1) {0}<br />(2) {1}";
            string billingTargetNameEN = "-", billingTargetNameLC = "-";

            var billingTarget = billinghandler.GetBillingTarget(billingTempItem.BillingTargetCode);

            if (billingTarget.Count == 1)
            {
                var billingClient = billingclienthandler.GetTbm_BillingClient(billingTarget[0].BillingClientCode);
                if (billingClient.Count == 1)
                {
                    billingTargetNameEN = billingClient[0].FullNameEN;
                    billingTargetNameLC = billingClient[0].FullNameLC;
                }
            }

            var officeData = officehandler.GetTbm_Office(billingTempItem.BillingOfficeCode);
            CommonUtil.MappingObjectLanguage<tbm_Office>(officeData);

            ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

            string contractFee = CommonUtil.TextNumeric((billingTempItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US ? billingTempItem.BillingAmtUsd : billingTempItem.BillingAmt));
            if (currencies != null)
            {
                DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == billingTempItem.BillingAmtCurrencyType);
                if (curr == null)
                    curr = currencies[0];

                contractFee = string.Format("{0} {1}", curr.ValueDisplayEN, contractFee);
            }

            res = new CTS053_ChangePlanGrid()
            {
                BillingClientCode = util.ConvertBillingClientCode(billingTempItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                BillingOCC = billingTempItem.BillingOCC,
                BillingTargetCode = util.ConvertBillingTargetCode(billingTempItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                BillingTargetName = String.Format(billingTargetnamePattern, billingTargetNameEN, billingTargetNameLC),
                BillingOfficeCode = billingTempItem.BillingOfficeCode,
                BillingOfficeName = officeData[0].OfficeDisplay,
                CanDelete = (String.IsNullOrEmpty(billingTempItem.BillingOCC)),
                ContractFee = contractFee,
                HasUpdate = false,
                IsNew = false,
                UID = Guid.NewGuid().ToString(),
                OriginalBillingClientCode = billingTempItem.BillingClientCode,
                OriginalBillingOCC = billingTempItem.BillingOCC,
                OriginalBillingOfficeCode = billingTempItem.BillingOfficeCode
            };

            return res;
        }

        /// <summary>
        /// Create DO of ChangePlan
        /// </summary>
        /// <param name="billingTempItem"></param>
        /// <returns></returns>
        private CTS053_ChangePlanGrid CreateChangePlanObject_CTS053(dtBillingTemp_SetItem billingTempItem)
        {
            CTS053_ChangePlanGrid res = new CTS053_ChangePlanGrid();
            CommonUtil util = new CommonUtil();
            IBillingInterfaceHandler billinghandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

            string billingTargetnamePattern = "(1) {0}<br />(2) {1}";
            var officeData = officehandler.GetTbm_Office(billingTempItem.BillingOffice);
            CommonUtil.MappingObjectLanguage<tbm_Office>(officeData);

            ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

            string contractFee = CommonUtil.TextNumeric(billingTempItem.BillingContractFee);
            if (currencies != null)
            {
                DataEntity.Common.doMiscTypeCode curr = currencies.Find(x => x.ValueCode == billingTempItem.BillingContractFeeCurrencyType);
                if (curr == null)
                    curr = currencies[0];

                contractFee = string.Format("{0} {1}", curr.ValueDisplayEN, contractFee);
            }

            res = new CTS053_ChangePlanGrid()
            {
                BillingClientCode = billingTempItem.BillingClientCode,
                BillingOCC = billingTempItem.BillingOCC,
                BillingTargetCode = billingTempItem.BillingTargetCode,
                BillingTargetName = String.Format(billingTargetnamePattern, billingTempItem.FullNameEN, billingTempItem.FullNameLC),
                BillingOfficeCode = billingTempItem.BillingOffice,
                BillingOfficeName = officeData[0].OfficeDisplay,
                CanDelete = (String.IsNullOrEmpty(billingTempItem.BillingOCC)),
                ContractFee = contractFee,
                HasUpdate = (billingTempItem.ObjectType == 0),
                IsNew = (billingTempItem.ObjectType == 1),
                UID = ((billingTempItem.ObjectType == 1) && (String.IsNullOrEmpty(billingTempItem.UID))) ? Guid.NewGuid().ToString() : ((!String.IsNullOrEmpty(billingTempItem.UID)) ? billingTempItem.UID : null),
                OriginalBillingClientCode = (!String.IsNullOrEmpty(billingTempItem.OriginalBillingClientCode) ? billingTempItem.OriginalBillingClientCode : billingTempItem.BillingClientCode),
                OriginalBillingOCC = (!String.IsNullOrEmpty(billingTempItem.OriginalBillingOCC) ? billingTempItem.OriginalBillingOCC : billingTempItem.BillingOCC),
                OriginalBillingOfficeCode = (!String.IsNullOrEmpty(billingTempItem.OriginalBillingOfficeCode) ? billingTempItem.OriginalBillingOfficeCode : billingTempItem.BillingOffice),
            };

            return res;
        }

        /// <summary>
        /// Validate required field of DO for register ChangePlan
        /// </summary>
        /// <param name="regisObj"></param>
        /// <returns></returns>
        private ObjectResultData ValidateRequired_CTS053(CTS053_RegisterChangePlan regisObj)
        {
            ObjectResultData res = new ObjectResultData();
            CTS051_ScreenParameter2 param = GetScreenObject<CTS051_ScreenParameter2>();
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

            try
            {
                List<string> errorCtrl = new List<string>();
                List<string> errorLabel = new List<string>();

                if (!regisObj.ChangeDateContractFee.HasValue)
                {
                    errorCtrl.Add("ChangeDateContractFee");
                    errorLabel.Add("lblChangeImplementDate");
                }

                if (!regisObj.ChangeContractFee.HasValue)
                {
                    errorCtrl.Add("ChangeContractFee");
                    errorLabel.Add("lblContractFee");
                }

                if (!regisObj.ChangeFeeFlag && (!regisObj.ReturnToOriginalFeeDate.HasValue))
                {
                    errorCtrl.Add("ReturnToOriginalFeeDate");
                    errorLabel.Add("lblReturnToOriginalFeeDate");
                }

                if (String.IsNullOrEmpty(regisObj.NegotiationStaffEmpNo1))
                {
                    errorCtrl.Add("NegotiationStaffEmpNo1");
                    errorLabel.Add("lblNegotiationStaffEmpNo1");
                }

                if (String.IsNullOrEmpty(regisObj.ApproveNo1))
                {
                    errorCtrl.Add("ApproveNo1");
                    errorLabel.Add("lblApproveNo1");
                }

                if ((errorCtrl.Count > 0) || (errorLabel.Count > 0))
                {
                    //List<String> errorLabelText = new List<string>();
                    string errorLabelText = "";

                    foreach (string item in errorLabel.Distinct())
                    {
                        if (errorLabelText.Length > 0)
                        {
                            errorLabelText += ", ";
                        }

                        errorLabelText += CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS053", item);
                        //errorLabelText.Add(CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS053", item));
                    }

                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { errorLabelText }, errorCtrl.ToArray());
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
        private ObjectResultData ValidateBusiness_CTS053(CTS053_RegisterChangePlan regisObj, bool HasChangePlanDetailTask)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();
            CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            IQuotationHandler quotationhandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            IEmployeeMasterHandler emphandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            IBillingInterfaceHandler billinginterfacehandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            IContractHandler contracthandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            IBillingTempHandler billingtemphandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

            bool bWarningCurrency = false;

            try
            {
                res = ValidateAuthority_CTS053(res, param, false);

                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                res = ValidateRequired_CTS053(regisObj);

                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (regisObj.ChangeDateContractFee.GetValueOrDefault() > DateTime.Now)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0009, new string[] { CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS053", "lblChangeImplementDate") }, new string[] { "ChangeDateContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                var contractData = rentalhandler.GetEntireContract(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.OCC);
                if (regisObj.ChangeDateContractFee < contractData.dtTbt_RentalContractBasic[0].LastChangeImplementDate)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3045, null, new string[] { "ChangeDateContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (!regisObj.ChangeFeeFlag)
                {
                    if (regisObj.ReturnToOriginalFeeDate <= regisObj.ChangeDateContractFee)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3046, null, new string[] { "ReturnToOriginalFeeDate" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }

                    if (param.mailList.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0088, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }
                }
                else
                {
                    regisObj.ReturnToOriginalFeeDate = null;
                }
                                
                if (regisObj.ChangeContractFee.GetValueOrDefault() > CommonValue.C_MAX_MONTHLY_FEE_INPUT)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3287,
                        new string[] { CommonValue.C_MAX_MONTHLY_FEE_INPUT.ToString("N2") },
                        new string[] { "ChangeContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                //Comment by Jutarat A. on 09042013
                //if (!contractData.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate.HasValue
                //    && (regisObj.CurrentContractFee == regisObj.ChangeContractFee))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3047, null, new string[] { "ChangeContractFee" });
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    return res;
                //}
                //End Comment

                //if (contractData.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate.HasValue
                //    && (regisObj.CurrentContractFee != regisObj.ChangeContractFee)
                //    && (regisObj.CurrentContractFeeCurrencyType == regisObj.ChangeContractFeeCurrencyType))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3048, null, new string[] { "ChangeContractFee" });
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    return res;
                //}
                if (contractData.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate.HasValue
                    && (regisObj.CurrentContractFeeCurrencyType != regisObj.ChangeContractFeeCurrencyType))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3361, null, new string[] { "ChangeContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                    //return res;
                }

                if (contractData.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate.HasValue
                    && (regisObj.CurrentContractFeeCurrencyType == regisObj.ChangeContractFeeCurrencyType)
                    && (regisObj.CurrentContractFee != regisObj.ChangeContractFee))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3048, null, new string[] { "ChangeContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (regisObj.ChangeContractFeeCurrencyType != regisObj.CurrentContractFeeCurrencyType)
                {
                    bWarningCurrency = true;
                }

                var emp1 = emphandler.GetTbm_Employee(regisObj.NegotiationStaffEmpNo1);
                var emp2 = emphandler.GetTbm_Employee(regisObj.NegotiationStaffEmpNo2);

                if ((emp1 == null) || (emp1.Count == 0))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, new string[] { CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS053", "lblNegotiationStaffEmpNo1")}, new string[] { "NegotiationStaffEmpNo1" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if ((!String.IsNullOrEmpty(regisObj.NegotiationStaffEmpNo2)) && ((emp2 == null) || (emp2.Count == 0)))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, new string[] { CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS053", "lblNegotiationStaffEmpNo2")}, new string[] { "NegotiationStaffEmpNo2" });
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
                            } else if (String.IsNullOrEmpty(regisObj.ApproveNo2))
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
                            } else if (String.IsNullOrEmpty(regisObj.ApproveNo3))
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
                            } else if (String.IsNullOrEmpty(regisObj.ApproveNo4))
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
                            } else if (String.IsNullOrEmpty(regisObj.ApproveNo5))
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

                int cntContractFeeOverZero = 0;
                decimal sumContractFee = 0;
                bool isSameContractFeeCurrency = true;

                var cntNewRes = from a in param.newItemList where a.BillingContractFee.GetValueOrDefault() > 0 select a;
                var cntUpdateRes = from a in param.updateItemList where a.BillingContractFee.GetValueOrDefault() > 0 select a;

                var billingTmpFull = rentalhandler.GetBillingTempForChangeFee(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));

                foreach (var billingTempItem in billingTmpFull)
                {
                    var delItemFilter = from a in param.deleteItemList
                                        where
                                            (a.BillingOffice == billingTempItem.BillingOfficeCode)
                                            && (util.ConvertBillingClientCode(a.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) == billingTempItem.BillingClientCode)
                                            && (a.BillingOCC == billingTempItem.BillingOCC)
                                            //param.deleteItemList.Where(x => (x.BillingOffice == a.BillingOffice)
                                            //&& (x.BillingClientCode == a.BillingClientCode)
                                            //&& (x.BillingOCC == a.BillingOCC)).Count() > 0
                                        select a;

                    if ((delItemFilter != null) && (delItemFilter.Count() == 0))
                    {
                        var updateItemFilter = from a in param.updateItemList
                                               where
                                                   (a.BillingOffice == billingTempItem.BillingOfficeCode)
                                                    && (util.ConvertBillingClientCode(a.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) == billingTempItem.BillingClientCode)
                                                    && (a.BillingOCC == billingTempItem.BillingOCC)
                                               select a;

                        if ((updateItemFilter != null) && (updateItemFilter.Count() == 0))
                        {
                            if (billingTempItem.BillingAmt.GetValueOrDefault() > 0)
                            {
                                if (billingTempItem.BillingAmtCurrencyType != regisObj.ChangeContractFeeCurrencyType)
                                    isSameContractFeeCurrency = false;

                                cntContractFeeOverZero++;
                                sumContractFee += billingTempItem.BillingAmt.GetValueOrDefault();
                            }
                            //var billingClient = billinginterfacehandler.GetBillingBasicAsBillingTemp(billingTempItem.BillingTargetCode, billingTempItem.BillingOCC);
                            //var billingClient_ContractFee = from a in billingClient
                            //                                where
                            //                                    (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                            //                                select a;

                            //if ((billingClient_ContractFee != null) && (billingClient_ContractFee.Count() > 0) && (billingClient_ContractFee.First().BillingAmt.GetValueOrDefault() > 0))
                            //{
                            //    cntContractFeeOverZero++;
                            //    sumContractFee += billingClient_ContractFee.First().BillingAmt.GetValueOrDefault();
                            //}
                        }
                    }
                }

                if (param.newItemList.Exists(x => x.BillingContractFeeCurrencyType != regisObj.ChangeContractFeeCurrencyType))
                    isSameContractFeeCurrency = false;
                if (param.newItemList.Exists(x => x.BillingContractFeeCurrencyType != regisObj.ChangeContractFeeCurrencyType))
                    isSameContractFeeCurrency = false;

                cntContractFeeOverZero = cntContractFeeOverZero + cntNewRes.Count() + cntUpdateRes.Count();
                sumContractFee = sumContractFee + 
                    param.newItemList.Sum(x => x.BillingContractFee.GetValueOrDefault()) + 
                    param.updateItemList.Sum(x => x.BillingContractFee.GetValueOrDefault());

                if ((regisObj.ChangeContractFee.GetValueOrDefault() > 0)
                    && regisObj.DivideBillingContractFee
                    && (cntContractFeeOverZero <= 1))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3010, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if ((regisObj.ChangeContractFee.GetValueOrDefault() > 0)
                    && !regisObj.DivideBillingContractFee
                    && (cntContractFeeOverZero > 1))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3011, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if ((contractData.dtTbt_RentalMaintenanceDetails.Count > 0)
                    && (contractData.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
                    && (cntContractFeeOverZero > 1))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3150, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (sumContractFee != regisObj.ChangeContractFee.GetValueOrDefault()
                    && isSameContractFeeCurrency)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3012, null, new string[] { "ChangeContractFee" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                if (HasChangePlanDetailTask)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3253, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                res.ResultData = true;

                if (bWarningCurrency)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3343, null, new string[] { "CurrentContractFeeCurrencyType", "ChangeContractFeeCurrencyType" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Register ChangeFee process
        /// </summary>
        /// <param name="regisObj"></param>
        /// <returns></returns>
        private int RegisterChangeFee_CTS053(CTS053_RegisterChangePlan regisObj)
        {
            CommonUtil util = new CommonUtil();
            CTS053_ScreenParameter param = GetScreenObject<CTS053_ScreenParameter>();
            IRentralContractHandler rentalhandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            IUserHandler userhandler = ServiceContainer.GetService<IUserHandler>() as IUserHandler;

            var contractData = rentalhandler.GetEntireContract(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), param.OCC);

            // Map data
            contractData.dtTbt_RentalSecurityBasic[0].ChangeImplementDate = regisObj.ChangeDateContractFee;

            if (contractData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
            {
                contractData.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopCurrencyType = regisObj.ChangeContractFeeCurrencyType;
                if (contractData.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    contractData.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop = null;
                    contractData.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopUsd = regisObj.ChangeContractFee;
                }
                else
                {
                    contractData.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop = regisObj.ChangeContractFee;
                    contractData.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopUsd = null;
                }

                contractData.dtTbt_RentalContractBasic[0].LastOrderContractFeeCurrencyType = contractData.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopCurrencyType;
                contractData.dtTbt_RentalContractBasic[0].LastOrderContractFee = contractData.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop;
                contractData.dtTbt_RentalContractBasic[0].LastOrderContractFeeUsd = contractData.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopUsd;
            }
            else
            {
                contractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType = regisObj.ChangeContractFeeCurrencyType;
                if (contractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    contractData.dtTbt_RentalSecurityBasic[0].OrderContractFee = null;
                    contractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd = regisObj.ChangeContractFee;
                }
                else
                {
                    contractData.dtTbt_RentalSecurityBasic[0].OrderContractFee = regisObj.ChangeContractFee;
                    contractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd = null;
                }

                contractData.dtTbt_RentalContractBasic[0].LastOrderContractFeeCurrencyType = contractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType;
                contractData.dtTbt_RentalContractBasic[0].LastOrderContractFee = contractData.dtTbt_RentalSecurityBasic[0].OrderContractFee;
                contractData.dtTbt_RentalContractBasic[0].LastOrderContractFeeUsd = contractData.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd;
            }

            contractData.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate = regisObj.ReturnToOriginalFeeDate;
            contractData.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1 = regisObj.NegotiationStaffEmpNo1;
            contractData.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo2 = regisObj.NegotiationStaffEmpNo2;
            contractData.dtTbt_RentalSecurityBasic[0].ApproveNo1 = regisObj.ApproveNo1;
            contractData.dtTbt_RentalSecurityBasic[0].ApproveNo2 = regisObj.ApproveNo2;
            contractData.dtTbt_RentalSecurityBasic[0].ApproveNo3 = regisObj.ApproveNo3;
            contractData.dtTbt_RentalSecurityBasic[0].ApproveNo4 = regisObj.ApproveNo4;
            contractData.dtTbt_RentalSecurityBasic[0].ApproveNo5 = regisObj.ApproveNo5;
            contractData.dtTbt_RentalSecurityBasic[0].ExpectedResumeDate = null; //Add by Jutarat A. on 02122013

            // Create mail list
            //List<dtEmailAddress> mailList = new List<dtEmailAddress>();
            //foreach (var mailItem in param.mailList)
            //{
            //    var tmpMail = userhandler.GetUserEmailAddressDataList(new doEmailSearchCondition()
            //    {
            //        EmailEddress = mailItem.EmailAddress
            //    });

            //    if (tmpMail.Count == 1)
            //    {
            //        mailList.Add(tmpMail[0]);
            //    }
            //}

            int regisRes = rentalhandler.RegisterChangeContractFee(contractData, param.mailList, param.newItemList, param.updateItemList, param.deleteItemList, regisObj.ChangeDateContractFee, regisObj.ChangeContractFee);

            return regisRes;
        }
        
        #endregion
    }
}
