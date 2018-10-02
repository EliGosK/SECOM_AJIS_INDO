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

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Event

        /// <summary>
        /// Validate required field and business when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ActionResult RegisterClick_CTS061(CTS061_DOValidateBusiness doValidateBusiness) //1.3 Validate required field
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resValidateBusiness = new ObjectResultData();

            ICommonHandler commonHandler;

            try
            {
                //1.1 Check suspending
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, "");
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                res = ValidateRequireField_CTS061();
                if (res.IsError)
                {
                    return Json(res);
                }

                //1.4 Validate business
                resValidateBusiness = ValidateBusiness_CTS061(doValidateBusiness);
                if (resValidateBusiness != null)
                    return Json(resValidateBusiness);

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
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ActionResult ConfirmClick_CTS061(CTS061_DOValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resValidateBusiness = new ObjectResultData();

            ICommonHandler commonHandler;
            ISaleContractHandler saleContractHandler;

            CTS061_ScreenParameter session;

            try
            {
                session = CTS061_GetImportData();
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                
                //3.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                {
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, "");
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);                    
                    return Json(res);
                }

                //3.2 Validate business
                resValidateBusiness = ValidateBusiness_CTS061(doValidateBusiness);
                if (resValidateBusiness != null)
                    return Json(resValidateBusiness);

                //3.3 Perform save operation
                session.DSSaleContract.dtTbt_SaleBasic[0].ExpectedInstallCompleteDate = doValidateBusiness.ExpectedInstallCompleteDate;                
                if (saleContractHandler.RegisterChangeExpectedInstallationCompleteDate(session.DSSaleContract))
                {
                    //3.4 Show success message
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046, "");                         
                    return Json(res);
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
        /// Reset data of screen
        /// </summary>
        /// <param name="lastOCC"></param>
        /// <returns></returns>
        public ActionResult ResetClick_CTS061(string lastOCC)
        {
            string expectedInstallCompleteDate = "";

            ObjectResultData res = new ObjectResultData();
            ObjectResultData resValidateBusiness = new ObjectResultData();
            CommonUtil util = new CommonUtil();

            CTS061_ScreenParameter session;
            ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IUserControlHandler usercontrolhandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;

            try
            {
                //Set screen to Initial state
                session = CTS061_GetImportData();
                expectedInstallCompleteDate = CommonUtil.TextDate(salehandler.GetTbt_SaleBasic(session.DSSaleContract.dtTbt_SaleBasic[0].ContractCode, lastOCC, true)[0].ExpectedInstallCompleteDate);
                var saleContractInfo = usercontrolhandler.GetSaleContractBasicInformationData(session.ContractCode, null);

                InitialScreen_CTS061(session.ContractCode);
                session = GetScreenObject<CTS061_ScreenParameter>();

                CTS061_ScreenOutputObject outObj = new CTS061_ScreenOutputObject()
                {
                    CanOperate = true,
                    ContractCode = session.ContractCode,
                    ContractCodeShort = util.ConvertContractCode(session.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    ExpectedInstallCompleteDate = expectedInstallCompleteDate,
                    InstallationStatusCode = saleContractInfo[0].InstallationStatusCode,
                    InstallationStatusCodeName = saleContractInfo[0].InstallationStatusCodeName,
                    LastOCC = salehandler.GetLastOCC(session.ContractCode),
                    ImportantFlag = saleContractInfo[0].PurchaserCustomerImportant.GetValueOrDefault(),
                    OperationOfficeName = CommonUtil.TextCodeName(saleContractInfo[0].OperationOfficeCode, saleContractInfo[0].OperationOfficeName),
                    PurchaserAddressEN = saleContractInfo[0].PurchaserAddressEN,
                    PurchaserAddressLC = saleContractInfo[0].PurchaserAddressLC,
                    PurchaserCustCode = util.ConvertCustCode(saleContractInfo[0].PurchaserCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    PurchaserNameEN = saleContractInfo[0].PurchaserNameEN,
                    PurchaserNameLC = saleContractInfo[0].PurchaserNameLC,
                    RealCustomerCustCode = util.ConvertCustCode(saleContractInfo[0].RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    SiteAddressEN = saleContractInfo[0].SiteAddressEN,
                    SiteAddressLC = saleContractInfo[0].SiteAddressLC,
                    SiteCode = util.ConvertSiteCode(saleContractInfo[0].SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT),
                    SiteNameEN = saleContractInfo[0].SiteNameEN,
                    SiteNameLC = saleContractInfo[0].SiteNameLC
                };

                res.ResultData = outObj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //BackClick perform in javascript

        #endregion

        #region Method

        /// <summary>
        /// Check system suspending, user’s permission and user’s authority of screen
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS061_Authority(CTS061_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CommonUtil util = new CommonUtil();
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                //1.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, new string[] { String.Format("Contract Code: {0}", param.ContractCode) }, null);
                    return Json(res);
                }

                //1.2 Check user's permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CQ12_CHANGE_COMPLETE_INSTALLATION_DATE, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
                    return Json(res);
                }

                // Check parameter
                if ((param == null)
                    || (String.IsNullOrEmpty(param.ContractCode)))
                {
                    //if (String.IsNullOrEmpty(CommonUtil.dsTransData.dtCommonSearch.ContractCode))
                    //{
                    //    // Not valid
                    //    //res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { "Contract Code" }, null);
                    //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147, null, null);
                    //    return Json(res);
                    //}
                    //else
                    //{
                    //    param.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                    //}

                    if (param.CommonSearch != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                            param.ContractCode = param.CommonSearch.ContractCode;
                    }
                    if (String.IsNullOrEmpty(param.ContractCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147, null, null);
                        return Json(res);
                    }
                    
                }

                // Check is contact exists
                var contractObj = saleHandler.GetTbt_SaleBasic(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null, true);
                if ((contractObj == null)
                    || (contractObj.Count == 0))
                {
                    // Not found
                    //res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { String.Format("Contract Code: {0}", param.ContractCode) }, null);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124, null, null);
                    return Json(res);
                }

                string lastOCC = saleHandler.GetLastOCC(contractObj[0].ContractCode);
                var dsSaleContract = saleHandler.GetEntireContract(contractObj[0].ContractCode, lastOCC);

                if ((dsSaleContract != null)
                    && (dsSaleContract.dtTbt_SaleBasic != null)
                    && (dsSaleContract.dtTbt_SaleBasic.Count != 0))
                {
                    /*
                    if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].ContractOfficeCode; }).Count == 0)
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0063, null, null);
                        return Json(res);
                    }

                    if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].OperationOfficeCode; }).Count == 0)
                    {
                        res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0063, null, null);
                        return Json(res);
                    }*/

                    var existsContarctOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].ContractOfficeCode);
                    var existsOperateOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].OperationOfficeCode);

                    if ((existsContarctOffice.Count() <= 0) && (existsOperateOffice.Count() <= 0))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }

                    if (dsSaleContract.dtTbt_SaleBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_ON)
                    {
                        res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3058, null, null);
                        return Json(res);
                    }
                }
                else
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0011, new string[] { String.Format("Contract Code: {0}", param.ContractCode) }, null);
                    return Json(res);
                }

                param.ScreenParameter = new CTS061_Parameter()
                {
                    contractCode = util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG)
                };
                param.ContractCode = param.ScreenParameter.contractCode;

                return InitialScreenEnvironment<CTS061_ScreenParameter>("CTS061", param, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        

        //public ActionResult CTS061_Authority(string contractCode)
        //{
        //    CommonUtil util = new CommonUtil();
        //    CTS061_ScreenParameter session = InitialScreenSession_CTS061();
        //    //session.ScreenParameter.contractCode = contractCode;
        //    session.ScreenParameter.contractCode = util.ConvertContractCode(contractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
        //    return InitialScreenEnvironment("CTS061", session);
        //}

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS061")]
        public ActionResult CTS061()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS061_ScreenParameter session = GetScreenObject<CTS061_ScreenParameter>();
                ViewBag.ContractCode = session.ScreenParameter.contractCode;
                InitialScreen_CTS061(ViewBag.ContractCode);
                return View("CTS061");
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
        public ActionResult InitialScreen_CTS061(string contractCode)
        {
            string lastOCC;
            dsSaleContractData dsSaleContract;            
            IUserControlHandler userControlHandler;
            ISaleContractHandler saleContractHandler;

            List<doSaleContractBasicInformation> listDOSaleContractBasicInformation;
            ObjectResultData res = new ObjectResultData();
            CTS061_ScreenParameter session;

            try
            {
                session = CTS061_GetImportData();
                userControlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                //1.1 Get contract data
                //1.1.1 Get last OCC
                lastOCC = saleContractHandler.GetLastOCC(contractCode);
                ViewBag.LastOCC = lastOCC;

                //1.1.2 Get entire contract data
                dsSaleContract = saleContractHandler.GetEntireContract(contractCode, lastOCC);
                session.DSSaleContract = dsSaleContract;
                HasAuthority_CTS061(contractCode);

                if (ViewBag.Permission == true && ViewBag.IsSystemSuspending == false && ViewBag.HasAuthorityContract == true && ViewBag.HasAuthorityOperation == true)
                {
                    //1.2 Validate entering conditions
                    if (dsSaleContract != null)
                    {
                        if (dsSaleContract.dtTbt_SaleBasic.Count() != 0)
                        {
                            //1.2.1 Installations of last OCC must not completed
                            if (dsSaleContract.dtTbt_SaleBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_ON)
                                ViewBag.ISValidOCCCompleted = false;
                            else
                                ViewBag.ISValidOCCCompleted = true;
                        }

                        //1.3 Get sale contract basic data
                        listDOSaleContractBasicInformation = userControlHandler.GetSaleContractBasicInformationData(contractCode, null);

                        //1.4 Bind ata to screen items
                        InitialScreenData_CTS061(listDOSaleContractBasicInformation, lastOCC);
                    }
                }

                try
                {
                    ViewBag.CanOperate = !ViewBag.IsSystemSuspending && ViewBag.Permission && ViewBag.HasAuthorityContract
                        && ViewBag.HasAuthorityOperation && (ViewBag.ISValidOCCCompleted || (ViewBag.ISValidOCCCompleted == null));
                }
                catch (Exception)
                {
                    ViewBag.CanOperate = false;
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
        /// <param name="listDOSaleContractBasicInformation"></param>
        /// <param name="lastOCC"></param>
        public void InitialScreenData_CTS061(List<doSaleContractBasicInformation> listDOSaleContractBasicInformation, string lastOCC)
        {
            CommonUtil comU;

            try
            {
                comU = new CommonUtil();
                ISaleContractHandler saleContractHandler;
                if (listDOSaleContractBasicInformation.Count() != 0)
                {
                    saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;

                    ViewBag.ContractCode = comU.ConvertContractCode(listDOSaleContractBasicInformation[0].ContractCode,CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.PurchaserCustCode = comU.ConvertCustCode(listDOSaleContractBasicInformation[0].PurchaserCustCode,CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.RealCustomerCustCode = comU.ConvertCustCode(listDOSaleContractBasicInformation[0].RealCustomerCustCode.ToString(), CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.SiteCode = comU.ConvertSiteCode(listDOSaleContractBasicInformation[0].SiteCode,CommonUtil.CONVERT_TYPE.TO_SHORT);
                    ViewBag.PurchaserNameEN = listDOSaleContractBasicInformation[0].PurchaserNameEN;
                    ViewBag.PurchaserAddressEN = listDOSaleContractBasicInformation[0].PurchaserAddressEN;

                    if (listDOSaleContractBasicInformation[0].PurchaserCustomerImportant == null)
                        ViewBag.ImportantFlag = false;
                    else
                        ViewBag.ImportantFlag = (bool)listDOSaleContractBasicInformation[0].PurchaserCustomerImportant;

                    ViewBag.SiteNameEN = listDOSaleContractBasicInformation[0].SiteNameEN;
                    ViewBag.SiteAddressEN = listDOSaleContractBasicInformation[0].SiteAddressEN;
                    ViewBag.PurchaserNameLC = listDOSaleContractBasicInformation[0].PurchaserNameLC;
                    ViewBag.PurchaserAddressLC = listDOSaleContractBasicInformation[0].PurchaserAddressLC;
                    ViewBag.SiteNameLC = listDOSaleContractBasicInformation[0].SiteNameLC;
                    ViewBag.SiteAddressLC = listDOSaleContractBasicInformation[0].SiteAddressLC;

                    if (listDOSaleContractBasicInformation[0].InstallationStatusCode != null && listDOSaleContractBasicInformation[0].InstallationStatusCode != "")                    
                        ViewBag.InstallationStatusCodeName = CommonUtil.TextCodeName(listDOSaleContractBasicInformation[0].InstallationStatusCode, listDOSaleContractBasicInformation[0].InstallationStatusName);
                    
                    ViewBag.InstallationStatusCode = listDOSaleContractBasicInformation[0].InstallationStatusCode;
                    ViewBag.OperationOfficeName = CommonUtil.TextCodeName(listDOSaleContractBasicInformation[0].OperationOfficeCode, listDOSaleContractBasicInformation[0].OperationOfficeName);
                    ViewBag.ExpectedInstallCompleteDate = CommonUtil.TextDate(saleContractHandler.GetTbt_SaleBasic(listDOSaleContractBasicInformation[0].ContractCode, lastOCC, null)[0].ExpectedInstallCompleteDate);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Initial session of screen
        /// </summary>
        /// <returns></returns>
        //public CTS061_ScreenParameter InitialScreenSession_CTS061()
        //{
        //    try
        //    {
        //        CTS061_ScreenParameter importData = new CTS061_ScreenParameter()
        //        {
        //            DSSaleContract = new dsSaleContractData(),
        //            DOValidateBusiness = new CTS061_DOValidateBusiness(),
        //            ScreenParameter = new CTS061_Parameter()
        //        };

        //        CTS061_SetImportData(importData);
        //        return importData;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        #endregion

        #region Validation

        /// <summary>
        /// Validate required field of DO validate business
        /// </summary>
        /// <returns></returns>
        public ObjectResultData ValidateRequireField_CTS061()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    res.ResultData = true;

                    if (res.IsError)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return res;
                    }
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
        public ObjectResultData ValidateBusiness_CTS061(CTS061_DOValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //DateTime future = new DateTime(DateTime.Now.Year+3, DateTime.Now.Month, DateTime.Now.Day);
                DateTime future = DateTime.Today.AddYears(3);
                if (doValidateBusiness.ExpectedInstallCompleteDate.Value.Date > future)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3206, "");
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3206);                    
                    return res;
                }  
            }
            catch (Exception ex)
            {                
                res.AddErrorMessage(ex);
            }

            return null;
        }

        /// <summary>
        /// Check screen has authority
        /// </summary>
        /// <param name="contractCode"></param>
        public void HasAuthority_CTS061(string contractCode)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonHandler;
            ISaleContractHandler saleContractHandler;
            List<tbt_SaleBasic> listSaleBasic;
            dsSaleContractData dsSaleContract;

            string lastOCC;

            try
            {
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                saleContractHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
               
                lastOCC = saleContractHandler.GetLastOCC(contractCode);               
                ViewBag.LastOCC = lastOCC;

                //1.1.2 Get entire contract data
                dsSaleContract = saleContractHandler.GetEntireContract(contractCode, lastOCC);
                         
                //1.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                    ViewBag.IsSystemSuspending = true;
                else
                    ViewBag.IsSystemSuspending = false;

                //1.2 Check user's permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CQ12_CHANGE_COMPLETE_INSTALLATION_DATE, FunctionID.C_FUNC_ID_OPERATE))
                    ViewBag.Permission = false;
                else
                    ViewBag.Permission = true;

                if (dsSaleContract != null)
                {
                    if (dsSaleContract.dtTbt_SaleBasic.Count != 0)
                    {
                        if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].ContractOfficeCode; }).Count == 0)
                            ViewBag.HasAuthorityContract = false;
                        else
                            ViewBag.HasAuthorityContract = true;

                        if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == dsSaleContract.dtTbt_SaleBasic[0].OperationOfficeCode; }).Count == 0)
                            ViewBag.HasAuthorityOperation = false;
                        else
                            ViewBag.HasAuthorityOperation = true;
                    }    
                }                
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
        private CTS061_ScreenParameter CTS061_GetImportData(string key = null)
        {
            try
            {
                return GetScreenObject<CTS061_ScreenParameter>(key);
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
        private void CTS061_SetImportData(CTS061_ScreenParameter import, string key = null)
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
        public ActionResult CTS061_ClearSession()
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
