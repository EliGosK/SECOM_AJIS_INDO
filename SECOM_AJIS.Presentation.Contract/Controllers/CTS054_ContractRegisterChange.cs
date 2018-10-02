//*********************************
// Create by: Songwut Chitipanich
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

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Event

        /// <summary>
        /// Update data to database when click [Confirm] button in ‘Action button’ section
        /// </summary>
        /// <param name="doCTS"></param>
        /// <returns></returns>
        public ActionResult ConfirmClick_CTS054(CTS054_DOValidateBusiness doCTS)
        {
            IUserControlHandler userHandler;
            IRentralContractHandler renderHandler;
            ObjectResultData res = new ObjectResultData();
            dtTbt_RentalContractBasicForView dtTbt_RentalContractBasicForView;
            dsRentalContractData dsRentalContract;

            CTS054_ScreenParameter session;

            try
            {
                session = CTS054_GetImportData();
                userHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                dtTbt_RentalContractBasicForView = renderHandler.GetTbt_RentalContractBasicForView(doCTS.ContractCode.Trim())[0];
                dsRentalContract = session.DSRentalContract;

                dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate = doCTS.ExpectedOperationDate;
                dsRentalContract.dtTbt_RentalContractBasic[0].LastChangeImplementDate = doCTS.ExpectedOperationDate;

                if (dtTbt_RentalContractBasicForView.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].ExpectedOperationDate = doCTS.ExpectedOperationDate;

                if (dtTbt_RentalContractBasicForView.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_AFTER_START)
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].ExpectedInstallationCompleteDate = doCTS.ExpectedOperationDate;

                renderHandler.RegisterExpectedOperationDate(dsRentalContract);
                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046, "");                      
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
        /// <param name="occ"></param>
        /// <returns></returns>
        public ActionResult ResetClick_CTS054(string occ)
        {
            ObjectResultData res = new ObjectResultData();
            string expectOperationDate = "";
            IUserControlHandler userHandler;
            IRentralContractHandler renderHandler;
            dtTbt_RentalContractBasicForView dtTbt_RentalContractBasicForView;
            dtTbt_RentalSecurityBasicForView dtTbt_RentalSecurityBasicForView;

            CTS054_ScreenParameter session;

            try
            {
                session = CTS054_GetImportData();

                userHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                doRentalContractBasicInformation doRental = userHandler.GetRentalContactBasicInformationData(session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractCode.Trim());
                dtTbt_RentalContractBasicForView = renderHandler.GetTbt_RentalContractBasicForView(session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractCode.Trim())[0];
                dtTbt_RentalSecurityBasicForView = renderHandler.GetTbt_RentalSecurityBasicForView(session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractCode.Trim(), occ.Trim())[0];
     
                if (dtTbt_RentalContractBasicForView.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    expectOperationDate = CommonUtil.TextDate(dtTbt_RentalSecurityBasicForView.ExpectedOperationDate);

                if (dtTbt_RentalContractBasicForView.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_AFTER_START)
                    expectOperationDate = CommonUtil.TextDate(dtTbt_RentalSecurityBasicForView.ExpectedInstallationCompleteDate);

                CTS054_ScreenOutputObject outObj = new CTS054_ScreenOutputObject()
                {
                    AddressFullEN = doRental.ContractTargetAddressEN,
                    AddressFullLC = doRental.ContractTargetAddressLC,
                    Alphabet = "",
                    ContractCode = doRental.ContractCode,
                    ContractCodeShort = doRental.ContractCodeShort,
                    CustFullNameEN = doRental.ContractTargetNameEN,
                    CustFullNameLC = doRental.ContractTargetNameLC,
                    CustomerCode = doRental.ContractTargetCustCodeShort,
                    RealCustomerCode = doRental.RealCustomerCustCodeShort,
                    DisplayAll = "",
                    SiteAddress = doRental.SiteAddressEN,
                    SiteAddressLC = doRental.SiteAddressLC,
                    SiteCode = doRental.SiteCodeShort,
                    SiteName = doRental.SiteNameEN,
                    SiteNameLC = doRental.SiteNameLC,
                    InstallationStatus = CommonUtil.TextCodeName(doRental.InstallationStatusCode, doRental.InstallationStatusName),
                    InstallationStatusCode = doRental.InstallationStatusCode,
                    OfficeName = CommonUtil.TextCodeName(doRental.OperationOfficeCode, doRental.OperationOfficeName),
                    EndContractDate = CommonUtil.TextDate(dtTbt_RentalSecurityBasicForView.ContractEndDate),
                    ImportantFlag = doRental.ContractTargetCustomerImportant.GetValueOrDefault(),
                    UserCode = doRental.UserCode,
                    ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL,
                    TargetCodeType = TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE,
                    Sequence = "",
                    OCC = dtTbt_RentalContractBasicForView.LastOCC,
                    QuotationTargetCode = doRental.ContractCodeShort,
                    ContractStatus = dtTbt_RentalContractBasicForView.ContractStatus,
                    ExpectOperationDate = expectOperationDate,
                    BillingClientCode = "",
                    BillingOffice = "",
                    EmpName = "",
                    EmpNo = "",
                    NegotiationStaffEmpNo1 = "",
                    PaymentMethod = ""
                };

                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = null;

                res.ResultData = outObj;
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
        /// Check system suspending, user’s permission and user’s authority of screen
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS054_Authority(CTS054_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CommonUtil util = new CommonUtil();
                ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IRentralContractHandler renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                //return InitialScreenEnvironment("CTS054", session);

                // Natthavat S., 2012/01/30
                // Check Authority Here

                //1.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                    return Json(res);
                }

                //1.2 Check user's permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP12_CHANGE_EXPECTED_OPR_DATE, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
                    return Json(res);
                }

                //if (String.IsNullOrEmpty(param.ContractCode) && !String.IsNullOrEmpty(CommonUtil.dsTransData.dtCommonSearch.ContractCode))
                //{
                //    param.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                //}
                if (String.IsNullOrEmpty(param.ContractCode) && param.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                        param.ContractCode = param.CommonSearch.ContractCode;
                }

                // Check parameter
                if ((param == null)
                    || (String.IsNullOrEmpty(param.ContractCode)))
                {
                    // Not valid
                    //res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { "Contract Code" }, null);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147, null, null);
                    return Json(res);
                }

                //Comment by Jutarat A. on 08082012
                //var saleExists = salehandler.IsContractExist(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                //if (saleExists.Count > 0 && saleExists[0].GetValueOrDefault())
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3278, null, null);
                //    return Json(res);
                //}

                // Check is contact exists
                var contractObj = renderHandler.GetTbt_RentalContractBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                if ((contractObj == null)
                    || (contractObj.Count == 0))
                {
                    // Not found
                    //res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { String.Format("Contract Code: {0}", param.ContractCode) }, null);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124, null, null);
                    return Json(res);
                }

                //1.3 Check user's authority to view data
                /*
                if (CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == contractObj[0].ContractOfficeCode).Count() == 0)
                {
                    //if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listRentalContractBasic[0].ContractOfficeCode; }).Count == 0)
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0063, null, null);
                    return Json(res);
                }

                if (CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == contractObj[0].OperationOfficeCode).Count() == 0)
                {
                    //if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listRentalContractBasic[0].OperationOfficeCode; }).Count == 0)
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0063, null, null);
                    return Json(res);
                }*/

                var existsContarctOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == contractObj[0].ContractOfficeCode);
                var existsOperateOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == contractObj[0].OperationOfficeCode);

                if ((contractObj[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
                        && (existsContarctOffice.Count() <= 0) && (existsOperateOffice.Count() <= 0))
                    || (contractObj[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                        && (existsOperateOffice.Count() <= 0))
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063, null, null);
                    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                string lastOCC = "";
                lastOCC = renderHandler.GetLastUnimplementedOCC(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                if (String.IsNullOrEmpty(lastOCC))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3053, null, null);
                    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                CTS054_ScreenParameter session = InitialScreenSession_CTS054();
                session.ScreenParameter.ContractCode = util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                //session.ScreenParameter.ContractCode = param.ContractCode;
                session.ContractCode = session.ScreenParameter.ContractCode;
                session.CommonSearch = param.CommonSearch;

                return InitialScreenEnvironment<CTS054_ScreenParameter>("CTS054", session, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //public ActionResult CTS054_Authority(string contractCode)
        //{
        //    ObjectResultData res = new ObjectResultData();

        //    try
        //    {
        //        CommonUtil util = new CommonUtil();
        //        CTS054_ScreenParameter session = InitialScreenSession_CTS054();
        //        session.ScreenParameter.ContractCode = util.ConvertContractCode(contractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
        //        return InitialScreenEnvironment("CTS054", session);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS054")]
        public ActionResult CTS054()
        {                      
            ObjectResultData res = new ObjectResultData();
            CommonUtil util = new CommonUtil();

            try
            {
                CTS054_ScreenParameter session = GetScreenObject<CTS054_ScreenParameter>();
                ViewBag.ContractCode = util.ConvertContractCode(session.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                ViewBag.ImportantFlag = false;
                HasAuthority_CTS054(session.ScreenParameter.ContractCode.Trim());
                if (ViewBag.Permission == true && ViewBag.IsSystemSuspending == false && ViewBag.HasAuthorityContract == true && ViewBag.HasAuthorityOperation == true)
                    InitialScreen_CTS054(session.ScreenParameter.ContractCode.Trim());

                return View("CTS054");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        public ActionResult InitialScreen_CTS054(string contractCode)
        {
            string occ;
            IRentralContractHandler renderHandler;
            ObjectResultData res = new ObjectResultData();

            CTS054_ScreenParameter session;

            try
            {
                session = CTS054_GetImportData();
                renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                occ = renderHandler.GetLastUnimplementedOCC(contractCode);
                if (occ != null && occ != "")
                {
                    ViewBag.OCC = occ;
                    session.DSRentalContract = renderHandler.GetEntireContract(contractCode, occ);
                    InitialScreenData_CTS054(contractCode, occ);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial data of screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        public void InitialScreenData_CTS054(string contractCode, string occ)
        {
            IUserControlHandler userHandler;
            IRentralContractHandler renderHandler;
            dtTbt_RentalContractBasicForView dtTbt_RentalContractBasicForView;
            dtTbt_RentalSecurityBasicForView dtTbt_RentalSecurityBasicForView;
            CTS054_ScreenParameter session;

            try
            {
                session = CTS054_GetImportData();
                userHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                
                doRentalContractBasicInformation doRental = userHandler.GetRentalContactBasicInformationData(contractCode.Trim());
                dtTbt_RentalContractBasicForView = renderHandler.GetTbt_RentalContractBasicForView(contractCode.Trim())[0];
                dtTbt_RentalSecurityBasicForView = renderHandler.GetTbt_RentalSecurityBasicForView(contractCode.Trim(), occ.Trim())[0];

                Bind_CTS054(doRental);

                if (dtTbt_RentalContractBasicForView.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    session.DOValidateBusiness.ExpectedOperationDate = dtTbt_RentalSecurityBasicForView.ExpectedOperationDate;

                if (dtTbt_RentalContractBasicForView.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_AFTER_START)
                    session.DOValidateBusiness.ExpectedOperationDate = dtTbt_RentalSecurityBasicForView.ExpectedInstallationCompleteDate;

                session.DOValidateBusiness.InstallationStatusCode = doRental.InstallationStatusCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Initial session of screen
        /// </summary>
        /// <returns></returns>
        public CTS054_ScreenParameter InitialScreenSession_CTS054()
        {
            try
            {
                CTS054_ScreenParameter importData = new CTS054_ScreenParameter()
                {
                    DORegisterData = new CTS054_DOValidateBusiness(),                    
                    DOValidateBusiness = new CTS054_DOValidateBusiness(),
                    ScreenParameter = new CTS054_Parameter()
                };

                CTS054_SetImportData(importData);
                return importData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Bind data to control in screen
        /// </summary>
        /// <param name="doRental"></param>
        public void Bind_CTS054(doRentalContractBasicInformation doRental)
        {
            CommonUtil comU;

            try
            {
                comU = new CommonUtil();
   
                ViewBag.RentalContractBasicInformation = doRental;
                ViewBag.ContractCodeLong = comU.ConvertContractCode(ViewBag.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                ViewBag.ContractCode = doRental.ContractCodeShort;
                ViewBag.UserCode = doRental.UserCode;
                ViewBag.CustomerCode = doRental.ContractTargetCustCodeShort;
                ViewBag.RealCustomerCode = comU.ConvertCustCode(doRental.RealCustomerCustCode,CommonUtil.CONVERT_TYPE.TO_SHORT);
                ViewBag.SiteCode = doRental.SiteCodeShort;

                if (doRental.ContractTargetCustomerImportant == null)
                    ViewBag.ImportantFlag = false;
                else
                    ViewBag.ImportantFlag = (bool)doRental.ContractTargetCustomerImportant;

                ViewBag.CustFullNameEN = doRental.ContractTargetNameEN;
                ViewBag.CustFullNameLC = doRental.ContractTargetNameLC;
                ViewBag.AddressFullEN = doRental.ContractTargetAddressEN;
                ViewBag.AddressFullLC = doRental.ContractTargetAddressLC;
                ViewBag.SiteName = doRental.SiteNameEN;
                ViewBag.SiteNameLC = doRental.SiteNameLC;
                ViewBag.SiteAddress = doRental.SiteAddressEN;
                ViewBag.SiteAddressLC = doRental.SiteAddressLC;                               
                ViewBag.InstallationStatus = doRental.InstallationStatusCode + ":" + doRental.InstallationStatusName;
                ViewBag.OperationOffice = doRental.OperationOfficeCode + ":" + doRental.OperationOfficeName;
                ViewBag.OfficeName = doRental.OperationOfficeCode + ":" + doRental.OperationOfficeName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get expected OperationDate for set initail data
        /// </summary>
        /// <returns></returns>
        public ActionResult GetExpectedOperationDate_CTS054()
        {
            ObjectResultData res = new ObjectResultData();
            CTS054_ScreenParameter session;

            try
            {
                session = CTS054_GetImportData();
                return Json(session.DOValidateBusiness);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validate Business when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <param name="doCTS054RegisterData"></param>
        /// <returns></returns>
        public ActionResult ValidateALL_CTS054(CTS054_DOValidateBusiness doCTS054RegisterData)
        {
            ObjectResultData res = new ObjectResultData();            

            try
            {                
                res = ValidateSystemSuspending_CTS054();
                if (res.ResultData != null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                
                //if (ModelState.IsValid == false)
                //{
                //    ValidatorUtil.BuildErrorMessage(res, this);
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.ResultData = true;

                //    if (res.IsError)
                //        return Json(res);
                //}
                if (!doCTS054RegisterData.ExpectedOperationDate.HasValue)
                {
                    res.ResultData = false;
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource("Common", "_ContractRegisterChange", "lblExpectOperationDate") }, new string[] { "dpExpectOperationDate" });
                    return Json(res);
                }

                res = ValidateBusiness_CTS054(doCTS054RegisterData);
                if (res.ResultData != null)
                {
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
        /// Validate for check system suspending
        /// </summary>
        /// <returns></returns>
        public ObjectResultData ValidateSystemSuspending_CTS054()
        {
            ICommonHandler commonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {                
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, "");
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
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
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ObjectResultData ValidateBusiness_CTS054(CTS054_DOValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                if (doValidateBusiness.ExpectedOperationDate <= DateTime.Now.Date)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3015, "");
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3015);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Check screen has authority
        /// </summary>
        /// <param name="contractCode"></param>
        public void HasAuthority_CTS054(string contractCode)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonHandler;
            IRentralContractHandler renderHandler;
            List<tbt_RentalContractBasic> listRentalContractBasic;

            try
            {
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                listRentalContractBasic = renderHandler.GetTbt_RentalContractBasic(contractCode, null);

                //1.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                    ViewBag.IsSystemSuspending = true;
                else
                    ViewBag.IsSystemSuspending = false;

                //1.2 Check user's permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP12_CHANGE_EXPECTED_OPR_DATE, FunctionID.C_FUNC_ID_OPERATE))
                    ViewBag.Permission = false;
                else
                    ViewBag.Permission = true;

                //1.3 Check user's authority to view data
                if (CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == listRentalContractBasic[0].ContractOfficeCode).Count() == 0)
                    //if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listRentalContractBasic[0].ContractOfficeCode; }).Count == 0)
                    ViewBag.HasAuthorityContract = false;
                else
                    ViewBag.HasAuthorityContract = true;

                if (CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == listRentalContractBasic[0].OperationOfficeCode).Count() == 0)
                    //if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listRentalContractBasic[0].OperationOfficeCode; }).Count == 0)
                    ViewBag.HasAuthorityOperation = false;
                else
                    ViewBag.HasAuthorityOperation = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        #endregion

        #region Session

        /// <summary>
        /// Get import data from screen
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private CTS054_ScreenParameter CTS054_GetImportData(string key = null)
        {
            try
            {
                return GetScreenObject<CTS054_ScreenParameter>(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Set import data to screen
        /// </summary>
        /// <param name="import"></param>
        /// <param name="key"></param>
        private void CTS054_SetImportData(CTS054_ScreenParameter import, string key = null)
        {
            try
            {
                UpdateScreenObject(import, key);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        ///// <summary>
        ///// Clear session of screen
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult CTS054_ClearSession()
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    try
        //    {
        //        UpdateScreenObject(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //    }

        //    return Json(res);
        //}

        #endregion
    }
}
