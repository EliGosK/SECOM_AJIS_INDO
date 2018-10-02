//*********************************
// Create by: Jutarat A.
// Create date: 29/Aug/2011
// Update date: 29/Aug/2011
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
using SECOM_AJIS.DataEntity.Quotation;
using System.Transactions;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Installation;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check system suspending, user’s permission and user’s authority of screen
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public ActionResult CTS090_Authority(CTS090_ScreenParameter sParam)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();
            ISaleContractHandler saleHandler;
            List<tbt_SaleBasic> tbtSaleBasicList;
            tbt_SaleBasic tbt_SaleBasicData = null;
            string strContractCodeLong = string.Empty;
            string strOccCode = string.Empty;
            CTS090_doSaleBasicDataAuthority doSaleBasicDataAuthority;

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                /*--- HasAuthority ---*/
                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CANCEL_SALE_CONTRACT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //if (String.IsNullOrEmpty(sParam.ContractCode))
                //    sParam.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                if (String.IsNullOrEmpty(sParam.ContractCode) && sParam.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(sParam.CommonSearch.ContractCode) == false)
                        sParam.ContractCode = sParam.CommonSearch.ContractCode;
                }

                //Check required field
                if (String.IsNullOrEmpty(sParam.ContractCode))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "ContractCode" });
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                    //                    ScreenID.C_SCREEN_ID_CANCEL_SALE_CONTRACT,
                    //                    MessageUtil.MODULE_COMMON,
                    //                    MessageUtil.MessageList.MSG0007,
                    //                    new string[] { "lblContractCode" },
                    //                    null);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147);

                    return Json(res);
                }
                strContractCodeLong = comUtil.ConvertContractCode(sParam.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                //RetrieveSaleContractData
                saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                strOccCode = saleHandler.GetLastOCC(strContractCodeLong);
                tbtSaleBasicList = saleHandler.GetTbt_SaleBasic(strContractCodeLong, strOccCode, FlagType.C_FLAG_ON);

                //Check existing of sale contract data
                if (tbtSaleBasicList == null || tbtSaleBasicList.Count < 1)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0093, new string[] { sParam.ContractCode });
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124);
                    return Json(res);
                }

                //CheckDataAuthority
                tbt_SaleBasicData = tbtSaleBasicList[0];
                doSaleBasicDataAuthority = CommonUtil.CloneObject<tbt_SaleBasic, CTS090_doSaleBasicDataAuthority>(tbt_SaleBasicData);
                if (CommonUtil.IsNullOrEmpty(doSaleBasicDataAuthority.OperationOfficeCode) == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, new object[] { doSaleBasicDataAuthority }, null, false);
                    if (res.IsError)
                        return Json(res);
                }
                /*-------------------------*/


                //ValidateDataBusiness
                ValidateDataBusiness_CTS090(res, tbt_SaleBasicData);
                if (res.IsError)
                    return Json(res);

                //sParam = new CTS090_ScreenParameter();
                sParam.CTS090_Session = new CTS090_RegisterCancelTargetData();
                sParam.CTS090_Session.InitialData = new CTS090_InitialRegisterCancelTargetData();
                sParam.CTS090_Session.InitialData.ContractCode = strContractCodeLong;
                sParam.CTS090_Session.InitialData.OCCCode = strOccCode;
                sParam.CTS090_Session.RegisterCancelData = tbt_SaleBasicData;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            //**** Change Session ****//
            //return InitialScreenEnvironment("CTS090", new object[] { strContractCodeLong, strOccCode, tbt_SaleBasicData});
            return InitialScreenEnvironment<CTS090_RegisterCancelTargetData>("CTS090", sParam, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS090")]
        public ActionResult CTS090()
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            CommonUtil comUtil = new CommonUtil();
            IUserControlHandler userCtrlHandler;
            List<doSaleContractBasicInformation> doSaleBasicList;
            doSaleContractBasicInformation doSaleBasicData = null;
            string strContractCodeLong = string.Empty;
            string strOccCode = string.Empty;

            try
            {
                //**** Change Session ****//
                //ScreenParameter sParam = GetScreenObject();
                //if (sParam.Parameter.Length > 0)
                //{
                //    strContractCodeLong = sParam.Parameter[(int)CTS090_InitialRegisterCancelTargetData.eParam.CONTRACT_CODE].ToString();
                //    strOccCode = sParam.Parameter[(int)CTS090_InitialRegisterCancelTargetData.eParam.OCC_CODE].ToString();
                //    tbt_SaleBasicData = (tbt_SaleBasic)sParam.Parameter[(int)CTS090_InitialRegisterCancelTargetData.eParam.SALE_BASIC_DATA];
                //}
                CTS090_ScreenParameter sParam = GetScreenObject<CTS090_ScreenParameter>();
                strContractCodeLong = sParam.CTS090_Session.InitialData.ContractCode;
                strOccCode = sParam.CTS090_Session.InitialData.OCCCode;

                userCtrlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                doSaleBasicList = userCtrlHandler.GetSaleContractBasicInformationData(strContractCodeLong, strOccCode);
                if (doSaleBasicList.Count > 0)
                {
                    Bind_CTS090(doSaleBasicList[0]);
                    doSaleBasicData = doSaleBasicList[0];
                }

                ViewBag.CurrentDate = DateTime.Now.Date;

                //Set data to CTS090_Session
                sParam.CTS090_Session.InitialData.doSaleContractBasicData = doSaleBasicData;
                UpdateScreenObject(sParam);

                return View();
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
        /// <param name="doCancalReason"></param>
        /// <returns></returns>
        public ActionResult CTS090_RegisterCancelData(CTS090_doCancelReason doCancalReason)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CANCEL_SALE_CONTRACT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                //CheckMandatory
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (ModelState.IsValid == false)
                    ValidatorUtil.BuildErrorMessage(res, this);

                if (res.IsError)
                    return Json(res);

                //ValidateScreenBusiness
                if (doCancalReason.CancelDate > DateTime.Now.Date)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3154, null, new string[] { "dpCancelDate" });
                    return Json(res);
                }

                //ValidateDataBusiness
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                ValidateDataBusiness_CTS090(res);
                if (res.IsError)
                    return Json(res);

                res.ResultData = true;
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
        /// <param name="doCancalReason"></param>
        /// <returns></returns>
        public ActionResult CTS090_ConfirmRegisterCancelData(CTS090_doCancelReason doCancalReason)
        {
            ObjectResultData res = new ObjectResultData();
            CTS090_RegisterCancelTargetData registerCancelData;
            List<tbt_SaleBasic> tbt_SaleBasicList;
            ISaleContractHandler saleHandler;
            IBillingTempHandler billingHandler;
            List<tbt_BillingTemp> tbt_BillingTempList;
            IQuotationHandler guotHandler;
            doUpdateQuotationData doUpdateQuotation;

            try
            {
                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check screen permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CANCEL_SALE_CONTRACT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                //ValidateScreenBusiness
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (doCancalReason.CancelDate > DateTime.Now.Date)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3154, null, new string[] { "dpCancelDate" });
                    return Json(res);
                }

                //ValidateDataBusiness
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                ValidateDataBusiness_CTS090(res);
                if (res.IsError)
                    return Json(res);


                CTS090_ScreenParameter sParam = GetScreenObject<CTS090_ScreenParameter>();
                using (TransactionScope scope = new TransactionScope())
                {
                    /*--- RegisterCancelContract ---*/
                    saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                    registerCancelData = sParam.CTS090_Session;
                    if (registerCancelData.RegisterCancelData != null)
                    {
                        string strContractCode = registerCancelData.RegisterCancelData.ContractCode;
                        string strOCCCode = registerCancelData.RegisterCancelData.OCC;
                        string strQuotationTargetCode = registerCancelData.RegisterCancelData.QuotationTargetCode;
                        string strAlphabet = registerCancelData.RegisterCancelData.Alphabet;

                        //MapSaleContractData
                        bool isUpdateQuotation = false; 
                        if (registerCancelData.RegisterCancelData.ChangeType == SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE)
                        {
                            registerCancelData.RegisterCancelData.ContractStatus = ContractStatus.C_CONTRACT_STATUS_CANCEL;
                            isUpdateQuotation = true;
                        }
                        registerCancelData.RegisterCancelData.OCC = registerCancelData.InitialData.OCCCode;
                        registerCancelData.RegisterCancelData.ChangeType = SaleChangeType.C_SALE_CHANGE_TYPE_CANCEL;
                        registerCancelData.RegisterCancelData.SaleProcessManageStatus = SaleProcessManageStatus.C_SALE_PROCESS_STATUS_CANCEL;
                        registerCancelData.RegisterCancelData.ApproveNo1 = doCancalReason.ApproveNo1;
                        registerCancelData.RegisterCancelData.CancelReasonType = doCancalReason.CancelReasonType;
                        registerCancelData.RegisterCancelData.CancelDate = doCancalReason.CancelDate;
                        registerCancelData.RegisterCancelData.CancelProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        registerCancelData.RegisterCancelData.ChangeImplementDate = doCancalReason.CancelDate;

                        //Save cancel contract data
                        tbt_SaleBasicList = saleHandler.UpdateTbt_SaleBasic(registerCancelData.RegisterCancelData);

                        //Delete billing temp
                        billingHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                        tbt_BillingTempList = billingHandler.DeleteBillingTempByContractCodeOCC(strContractCode, strOCCCode);

                        //Lock quotation
                        guotHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                        bool isLockQuotComplete = guotHandler.LockQuotation(strQuotationTargetCode, strAlphabet, LockStyle.C_LOCK_STYLE_ALL);

                        if (isUpdateQuotation)
                        {
                            //Update quotation data
                            doUpdateQuotation = new doUpdateQuotationData();
                            doUpdateQuotation.QuotationTargetCode = strQuotationTargetCode;
                            doUpdateQuotation.Alphabet = strAlphabet;
                            doUpdateQuotation.LastUpdateDate = DateTime.MinValue; //null;
                            doUpdateQuotation.ContractCode = strContractCode;
                            doUpdateQuotation.ActionTypeCode = ActionType.C_ACTION_TYPE_CANCEL;
                            int iUpdateQuotRowCount = guotHandler.UpdateQuotationData(doUpdateQuotation);
                        }

                        //Delete installation basic
                        IInstallationHandler installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                        bool blnProcessResult = installHandler.DeleteInstallationBasicData(strContractCode);

                        //Cancel book 
                        IInventoryHandler inventHandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                        doBooking booking = new doBooking();
                        booking.ContractCode = registerCancelData.RegisterCancelData.ContractCode;
                        bool isCancelBookComplete = inventHandler.CancelBooking(booking);                  
                    }
                    /*--------------------------*/

                    scope.Complete();
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
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

        //Change to Screen param
        //public CTS090_RegisterCancelTargetData CTS090_Session
        //{
        //    get
        //    {
        //        return CommonUtil.GetSession<CTS090_RegisterCancelTargetData>(ScreenID.C_SCREEN_ID_REGISTER_CANCEL_SALE_CONTRACT);
        //    }
        //    set
        //    {
        //        CommonUtil.SetSession(ScreenID.C_SCREEN_ID_REGISTER_CANCEL_SALE_CONTRACT, value);
        //    }
        //}

        /// <summary>
        /// Check system is suspending
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool CheckIsSuspending(ObjectResultData res)
        {
            ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            if (handler.IsSystemSuspending())
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Bind data to control on screen
        /// </summary>
        /// <param name="doSaleBasic"></param>
        private void Bind_CTS090(doSaleContractBasicInformation doSaleBasic)
        {
            ViewBag.SaleContractBasicInformation = doSaleBasic;
            ViewBag.ContractCodeLong = doSaleBasic.ContractCode;
            ViewBag.ContractCodeShort = doSaleBasic.ContractCodeShort;
            ViewBag.PurchaserCustCodeShort = doSaleBasic.PurchaserCustCodeShort;
            ViewBag.RealCustomerCustCodeShort = doSaleBasic.RealCustomerCustCodeShort;
            ViewBag.SiteCodeShort = doSaleBasic.SiteCodeShort;
            ViewBag.PurchaserCustomerImportant = doSaleBasic.PurchaserCustomerImportant;
            ViewBag.PurchaserNameEN = doSaleBasic.PurchaserNameEN;
            ViewBag.PurchaserAddressEN = doSaleBasic.PurchaserAddressEN;
            ViewBag.SiteNameEN = doSaleBasic.SiteNameEN;
            ViewBag.SiteAddressEN = doSaleBasic.SiteAddressEN;
            ViewBag.PurchaserNameLC = doSaleBasic.PurchaserNameLC;
            ViewBag.PurchaserAddressLC = doSaleBasic.PurchaserAddressLC;
            ViewBag.SiteNameLC = doSaleBasic.SiteNameLC;
            ViewBag.SiteAddressLC = doSaleBasic.SiteAddressLC;
            ViewBag.InstallationStatusCodeName = doSaleBasic.InstallationStatusCodeName;
            ViewBag.OperationOfficeName = doSaleBasic.OperationOfficeCodeName;
            ViewBag.ContractOfficeName = doSaleBasic.ContractOfficeCodeName;
            ViewBag.LastChangeType = doSaleBasic.LastChangeTypeCodeName;
        }

        /// <summary>
        /// Validate business of screen
        /// </summary>
        /// <param name="res"></param>
        /// <param name="tbt_SaleBasicData"></param>
        public void ValidateDataBusiness_CTS090(ObjectResultData res, tbt_SaleBasic tbt_SaleBasicData = null)
        {
            CTS090_RegisterCancelTargetData registerCancelData;
            IInstallationHandler installHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

            try
            {
                if (tbt_SaleBasicData != null)
                {
                    registerCancelData = new CTS090_RegisterCancelTargetData();
                    registerCancelData.RegisterCancelData = tbt_SaleBasicData;
                }
                else
                {
                    CTS090_ScreenParameter sParam = GetScreenObject<CTS090_ScreenParameter>();
                    registerCancelData = sParam.CTS090_Session;
                }

                if (registerCancelData != null)
                {
                    //Check change type
                    if ((registerCancelData.RegisterCancelData != null && registerCancelData.RegisterCancelData.ChangeType != null)
                        && ((registerCancelData.RegisterCancelData.ChangeType != SaleChangeType.C_SALE_CHANGE_TYPE_NEW_SALE)
                            && (registerCancelData.RegisterCancelData.ChangeType != SaleChangeType.C_SALE_CHANGE_TYPE_ADD_SALE)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3168, null, null);
                        return;
                    }

                    //Check complete installation status
                    if ((registerCancelData.RegisterCancelData != null && registerCancelData.RegisterCancelData.InstallationCompleteFlag != null)
                        && (registerCancelData.RegisterCancelData.InstallationCompleteFlag == FlagType.C_FLAG_ON))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3058, null, null);
                        return;
                    }

                    //Check cancel status
                    if ((registerCancelData.RegisterCancelData != null && registerCancelData.RegisterCancelData.SaleProcessManageStatus != null)
                        && (registerCancelData.RegisterCancelData.SaleProcessManageStatus == SaleProcessManageStatus.C_SALE_PROCESS_STATUS_CANCEL))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3105, null, null);
                        return;
                    }

                    //Add by Jutarat A. on 06062013
                    //Check installation status
                    //Get installation basic
                    List<tbt_InstallationBasic> doTbt_InstallationBasicValidate = installHandler.GetTbt_InstallationBasicData(registerCancelData.RegisterCancelData.ContractCode);
                    if (doTbt_InstallationBasicValidate != null && doTbt_InstallationBasicValidate.Count > 0
                        && doTbt_InstallationBasicValidate[0].InstallationStatus != InstallationStatus.C_INSTALL_STATUS_INSTALL_NOT_REGISTERED)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3309, null, null);
                        return;
                    }
                    //End Add
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }
        #endregion

    }
}
