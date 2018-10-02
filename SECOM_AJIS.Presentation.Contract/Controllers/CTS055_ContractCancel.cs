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
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.Presentation.Contract.Models;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Event

        /// <summary>
        /// Update data to database when click [OK] button in ‘Action button’ section
        /// </summary>
        /// <param name="doCTS"></param>
        /// <returns></returns>
        public ActionResult OKClick_CTS055(CTS055_DOValidateBusiness doCTS)
        {
            IRentralContractHandler renderHandler;
            ObjectResultData res = new ObjectResultData();
            dsRentalContractData dsRental;

            CTS055_ScreenParameter session;

            try
            {
                session = CTS055_GetImportData();
                renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                dsRental = session.DSRentalContract;
                renderHandler.CancelUnoperatedContract(dsRental);
                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3057, "");
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
        public ActionResult CTS055_Authority(CTS055_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CommonUtil util = new CommonUtil();
                ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IRentralContractHandler renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                IInstallationHandler installhandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                //1.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                    return Json(res);
                }

                //1.2 Check user's permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP12_CANCEL_UNIMPLEMENTED_CONTRACT, FunctionID.C_FUNC_ID_OPERATE))
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
                if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == contractObj[0].ContractOfficeCode; }).Count == 0)
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0063, null, null);
                    return Json(res);
                }

                if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == contractObj[0].OperationOfficeCode; }).Count == 0)
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0063, null, null);
                    return Json(res);
                }
                 * */

                var existsContarctOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == contractObj[0].ContractOfficeCode);
                var existsOperateOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == contractObj[0].OperationOfficeCode);

                if ((contractObj[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
                        && (existsContarctOffice.Count() <= 0) && (existsOperateOffice.Count() <= 0))
                    || (contractObj[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                        && (existsOperateOffice.Count() <= 0))
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                var lastOCC = renderHandler.GetLastUnimplementedOCC(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                if (String.IsNullOrEmpty(lastOCC)
                    || contractObj[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA
                    || contractObj[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG
                    || contractObj[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE
                    )
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3056, null, null);
                    return Json(res);
                }

                
                if ((contractObj[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL)
                    || (contractObj[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                    || (contractObj[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT)
                    || (contractObj[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_TERMINATED))
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3001, null, null);
                    return Json(res);
                }

                var secureObj = renderHandler.GetTbt_RentalSecurityBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), lastOCC);

                // Unimplement Addition
                string installStatus = installhandler.GetInstallationStatus(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                if (((contractObj[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                    || (contractObj[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
                    && ((installStatus != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                    || (secureObj[0].InstallationCompleteFlag == FlagType.C_FLAG_ON)))
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3054, null, null);
                    return Json(res);
                }

                string occCode = renderHandler.GetLastUnimplementedOCC(contractObj[0].ContractCode);

                if (String.IsNullOrEmpty(occCode))
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3055, null, null);
                    return Json(res);
                }

                var dtRentalSecurityBasic = renderHandler.GetTbt_RentalSecurityBasicForView(contractObj[0].ContractCode, occCode.Trim())[0];

                if (StartType.C_START_TYPE_ALTER_START == contractObj[0].StartType
                    && OCCType.C_FIRST_UNIMPLEMENTED_SECURITY_OCC == dtRentalSecurityBasic.OCC)
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3288, null, null);
                    return Json(res);
                }

                if (contractObj[0].ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_BEF_START &&
                    dtRentalSecurityBasic.OCC == SECOM_AJIS.Common.Util.ConstantValue.OCCType.C_FIRST_UNIMPLEMENTED_SECURITY_OCC)
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3055, null, null);
                    return Json(res);
                }

                //================ CHeck permission to view contract data ===================
                List<tbt_RentalContractBasic> listRentalContractBasic;
                listRentalContractBasic = renderHandler.GetTbt_RentalContractBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);

                //1.3 Check user's authority to view data
                if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == contractObj[0].ContractOfficeCode; }).Count == 0)
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0063, null, null);
                    return Json(res);
                }               
                //===========================================================================

                CTS055_ScreenParameter session = InitialScreenSession_CTS055();
                session.ScreenParameter.ContractCode = util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                //return InitialScreenEnvironment("CTS054", session);
                return InitialScreenEnvironment<CTS055_ScreenParameter>("CTS055", session, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //public ActionResult CTS055_Authority(string contractCode)
        //{
        //    CommonUtil util = new CommonUtil();
        //    CTS055_ScreenParameter session = InitialScreenSession_CTS055();
        //    session.ScreenParameter.ContractCode = util.ConvertContractCode(contractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            
        //    return InitialScreenEnvironment("CTS055", session);
        //}

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        [Initialize("CTS055")]
        public ActionResult CTS055(string contractCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS055_ScreenParameter session = GetScreenObject<CTS055_ScreenParameter>();
                ViewBag.ContractCode = session.ScreenParameter.ContractCode;
                ViewBag.ImportantFlag = false;
                //HasAuthority_CTS055(contractCode);
                HasAuthority_CTS055(session.ScreenParameter.ContractCode);
                if (ViewBag.Permission == true && ViewBag.IsSystemSuspending == false && ViewBag.HasAuthorityContract == true && ViewBag.HasAuthorityOperation == true)
                    InitialScreen_CTS055(session.ScreenParameter.ContractCode.Trim());

                return View("CTS055");
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
        public ActionResult InitialScreen_CTS055(string contractCode)
        {
            string installationStatusCode = "00"; //I will default to 00 because the talble installation still not create.

            IRentralContractHandler renderHandler;
            
            ObjectResultData res = new ObjectResultData();
            dtTbt_RentalContractBasicForView dtRental;
            dtTbt_RentalSecurityBasicForView dtRentalSecurityBasic;

            CTS055_ScreenParameter session;

            try
            {
                session = CTS055_GetImportData();

                renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                string occCode = renderHandler.GetLastUnimplementedOCC(contractCode);

                if (occCode != null && occCode != "")
                    ViewBag.OCCCode = occCode;

                dtRental = renderHandler.GetTbt_RentalContractBasicForView(contractCode.Trim())[0];
                dtRentalSecurityBasic = renderHandler.GetTbt_RentalSecurityBasicForView(contractCode.Trim(), occCode.Trim())[0];
                ViewBag.DSRental = renderHandler.GetEntireContract(contractCode, occCode);

                if (dtRental.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_STOPPING)
                    ViewBag.STOPPING = true;

                //if (dtRental.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL
                //    && (/* AND InstallationStatus != C_INSTALLATION_STATUS_NO_INSTALLATION OR */
                //    dtRental.FirstInstallCompleteFlag == SECOM_AJIS.Common.Util.ConstantValue.FlagType.C_FLAG_ON))
                //    ViewBag.FLAGOFF = true;

                if (dtRental.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_BEF_START &&
                    dtRentalSecurityBasic.OCC == SECOM_AJIS.Common.Util.ConstantValue.OCCType.C_FIRST_UNIMPLEMENTED_SECURITY_OCC)
                    ViewBag.SECURITYOCC = true;

                session.DSRentalContract = renderHandler.GetEntireContract(contractCode, occCode);
                InitialScreenData_CTS055(contractCode, occCode);
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
        public void InitialScreenData_CTS055(string contractCode, string occ)
        {
            IUserControlHandler userHandler;
            IRentralContractHandler renderHandler;
            ObjectResultData res = new ObjectResultData();
            dtTbt_RentalContractBasicForView dtTbt_RentalContractBasicForView;
            dtTbt_RentalSecurityBasicForView dtTbt_RentalSecurityBasicForView;

            CTS055_ScreenParameter session;

            try
            {
                session = CTS055_GetImportData();
                userHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                doRentalContractBasicInformation doRental = userHandler.GetRentalContactBasicInformationData(contractCode.Trim());
                dtTbt_RentalContractBasicForView = renderHandler.GetTbt_RentalContractBasicForView(contractCode.Trim())[0];
                dtTbt_RentalSecurityBasicForView = renderHandler.GetTbt_RentalSecurityBasicForView(contractCode.Trim(), occ.Trim())[0];

                Bind_CTS055(doRental);

                if (dtTbt_RentalContractBasicForView.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    ViewBag.ExpectOperationDate = dtTbt_RentalSecurityBasicForView.ExpectedOperationDate;

                if (dtTbt_RentalContractBasicForView.ContractStatus == SECOM_AJIS.Common.Util.ConstantValue.ContractStatus.C_CONTRACT_STATUS_AFTER_START)
                    ViewBag.ExpectOperationDate = dtTbt_RentalSecurityBasicForView.ExpectedInstallationCompleteDate;

                session.DOValidateBusiness.InstallationStatusCode = doRental.InstallationStatusCode;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Initial session of screen
        /// </summary>
        /// <returns></returns>
        public CTS055_ScreenParameter InitialScreenSession_CTS055()
        {
            try
            {
                CTS055_ScreenParameter importData = new CTS055_ScreenParameter()
                {
                    DOValidateBusiness = new CTS055_DOValidateBusiness(),
                    ScreenParameter = new CTS055_Parameter()
                };

                CTS055_SetImportData(importData);
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
        public void Bind_CTS055(doRentalContractBasicInformation doRental)
        {
            CommonUtil comU = new CommonUtil();
            
            try
            {
                ViewBag.RentalContractBasicInformation = doRental;
                ViewBag.ContractCodeLong = ViewBag.ContractCode;
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
        /// Validate business when initail screen
        /// </summary>
        /// <returns></returns>
        public ActionResult GetValidateBusiness_CTS055()
        {
            ObjectResultData res = new ObjectResultData();
            CTS055_ScreenParameter session;

            try
            {
                session = CTS055_GetImportData();
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
        /// Validate business when click [OK] button in ‘Action button’ section
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ActionResult ValidateALL_CTS055(CTS055_DOValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res = ValidateSystemSuspending_CTS055();
                if (res.ResultData != null)
                    return Json(res);

                res = ValidateBusiness_CTS055(doValidateBusiness);
                if (res.ResultData != null)
                    return Json(res);

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
        public ObjectResultData ValidateSystemSuspending_CTS055()
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
        /// Validate business of screen
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ObjectResultData ValidateBusiness_CTS055(CTS055_DOValidateBusiness doValidateBusiness)
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
        public void HasAuthority_CTS055(string contractCode)
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
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP12_CANCEL_UNIMPLEMENTED_CONTRACT, FunctionID.C_FUNC_ID_OPERATE))
                    ViewBag.Permission = false;
                else
                    ViewBag.Permission = true;

                //1.3 Check user's authority to view data
                if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listRentalContractBasic[0].ContractOfficeCode; }).Count == 0)
                    ViewBag.HasAuthorityContract = false;
                else
                    ViewBag.HasAuthorityContract = true;

                if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == listRentalContractBasic[0].OperationOfficeCode; }).Count == 0)
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
        private CTS055_ScreenParameter CTS055_GetImportData(string key = null)
        {
            try
            {
                return GetScreenObject<CTS055_ScreenParameter>(key);
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
        private void CTS055_SetImportData(CTS055_ScreenParameter import, string key = null)
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

        /// <summary>
        /// Clear session of screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS055_ClearSession()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                UpdateScreenObject(null);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion
    }
}
