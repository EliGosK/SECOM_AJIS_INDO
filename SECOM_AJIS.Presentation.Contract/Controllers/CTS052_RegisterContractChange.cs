//*********************************
// Create by Songwut Chitipanich: 
// Create date: /AUG/2010
// Update date: /AUG/2010
//*********************************

using System;
using System.Web;
using System.Web.Mvc;

using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;

using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Contract.Models;

using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Controllers;

using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Common.Helpers;
using SECOM_AJIS.DataEntity.Installation;

namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Event

        /// <summary>
        /// Retrieve Quotation data when click [Retrieve] button in ‘Specify quotation’ section 
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ActionResult RetrieveClick_CTS052(CTS052_DOValidateBusinessData doValidateBusiness)
        {
            int countQuotationLinkage = 0;
            CommonUtil comU;
            dsQuotationData dsQuotation;
            List<dtInstrument> dtTbtInstrument;
            IQuotationHandler quotationHandler;
            IMasterHandler masterHandler;
            IInstrumentMasterHandler instrumentMasterHandler;
            IContractHandler contractHandler;
            ObjectResultData res = new ObjectResultData();
            CTS052_ScreenParameter session;

            try
            {
                comU = new CommonUtil();
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                quotationHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                instrumentMasterHandler = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;

                session = CTS052_GetImportData();

                //2.1 Validate require fields
                //2.1.1 
                if (doValidateBusiness.QuotationTargetCode == null || doValidateBusiness.QuotationTargetCode == "")
                {
                    string[] param = { CommonUtil.GetLabelFromResource("Contract", "CTS052", "lblQuotationCode") };
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "Quotation target code");
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, param, new string[] { "Alphabet" });
                    return Json(res);
                }

                //2.1.2
                if (doValidateBusiness.Alphabet == null || doValidateBusiness.Alphabet == "")
                {
                    string[] param = { CommonUtil.GetLabelFromResource("Contract", "CTS052", "lblAlplabet") };
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "Alphabet");
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, param, new string[] { "Alphabet" });
                    return Json(res);
                }

                //2.2 Load Quotation
                doGetQuotationDataCondition doQuotationCondition = new doGetQuotationDataCondition();
                doQuotationCondition.QuotationTargetCode = comU.ConvertQuotationTargetCode(doValidateBusiness.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                doQuotationCondition.Alphabet = doValidateBusiness.Alphabet;
                doQuotationCondition.ServiceTypeCode = SECOM_AJIS.Common.Util.ConstantValue.ServiceType.C_SERVICE_TYPE_RENTAL;
                doQuotationCondition.TargetCodeTypeCode = SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE;
                doQuotationCondition.ContractFlag = true;

                //2.3 Validate Quotation For error //This validation is the same other screeen                 
                dsQuotation = quotationHandler.GetQuotationData(doQuotationCondition);
                session.DSQuotation = dsQuotation;

                //2.3.0 Validate null dsQuotation
                if (dsQuotation == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, null);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0137, null, null);
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, null, new string[] { "Alphabet" });
                    return Json(res);
                }

                if (dsQuotation.dtTbt_QuotationMaintenanceLinkage != null)
                {
                    foreach (var item in dsQuotation.dtTbt_QuotationMaintenanceLinkage)
                    {
                        countQuotationLinkage += 1;
                    }

                    //2.3.1 The quotaion of maintenance contract must relate to only one contract
                    if (countQuotationLinkage > 1)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, null);
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, null, new string[] { "Alphabet" });
                        return Json(res);
                    }
                }

                //2.3.2 The last contract occ in quotation data must equal to the last contract occ in contract data
                if (dsQuotation.dtTbt_QuotationBasic.LastOccNo != session.DSRentalContract.dtTbt_RentalContractBasic[0].LastOCC)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, null);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3002, null, new string[] { "Alphabet" });
                    return Json(res);
                }

                //2.4 Validate quotation for warning
                foreach (var item in dsQuotation.dtTbt_QuotationInstrumentDetails)
                {
                    dtTbtInstrument = instrumentMasterHandler.GetInstrument(item.InstrumentCode, null, null, null);
                    if (dtTbtInstrument.Count != 0)
                    {
                        if (dtTbtInstrument[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE ||
                        dtTbtInstrument[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE)
                        {
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG;
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3038, null);
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3038, null, new string[] { "Alphabet" });
                            GetQuotationInformation_CTS052();
                            return Json(res);
                        }
                    }
                }

                GetQuotationInformation_CTS052();
                return Json(session.DOQuotation);
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
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ActionResult RegisterClick_CTS052(CTS052_DOValidateBusinessData doValidateBusiness)
        {
            ICommonHandler commonHandler;
            IRentralContractHandler rentralContractHandler;
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resBusiness = new ObjectResultData();

            dsRentalContractData dsRentalContract;
            CTS052_ScreenParameter session;


            try
            {
                session = CTS052_GetImportData();
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

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

                if (commonHandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3049, "");
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3049, null, null);
                    return Json(res);
                }

                resBusiness = ValidateBusiness_CTS052(doValidateBusiness);
                if (resBusiness != null)
                    return Json(resBusiness);

                dsRentalContract = session.DSRentalContract;
                rentralContractHandler.MapFromQuotation(session.DSQuotation, ref dsRentalContract, FlagType.C_FLAG_ON);

                //Update other information
                if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_BEF_START
                    && dsRentalContract.dtTbt_RentalContractBasic[0].StartType != StartType.C_START_TYPE_ALTER_START)
                {
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpNo1;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2 = session.DSQuotation.dtTbt_QuotationBasic.SalesmanEmpNo2;
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
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ActionResult ConfirmClick_CTS052(CTS052_DOValidateBusinessData doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resBusiness = new ObjectResultData();
            IRentralContractHandler rentralHandler;
            ICommonHandler commomHandler;
            CTS052_ScreenParameter session;

            try
            {
                session = CTS052_GetImportData();
                commomHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

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

                if (commomHandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, null, null);
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, "");
                    return Json(res);
                }

                resBusiness = ValidateBusiness_CTS052(doValidateBusiness);
                if (resBusiness != null)
                    return Json(resBusiness);

                session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate = doValidateBusiness.ChangeImplementDate;
                session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1 = doValidateBusiness.NegotiationStaffEmpNo1;
                session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1 = doValidateBusiness.NegotiationStaffEmpNo1;
                session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo2 = doValidateBusiness.NegotiationStaffEmpNo2;
                session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo1 = doValidateBusiness.ApproveNo1;
                session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo2 = doValidateBusiness.ApproveNo2;
                session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ExpectedResumeDate = null; //Add by Jutarat A. on 02122013

                rentralHandler.RegisterModifyInstrument(session.DSQuotation, session.DSRentalContract);
                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046, "");
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Clear data when click [Clear] button in ‘Specify quotation’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearClick()
        {
            ObjectResultData res = new ObjectResultData();
            CTS052_ScreenParameter session;

            try
            {
                session = CTS052_GetImportData();
                session.DOQuotation = null;
                session.DOValidateBusiness = null;
                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = null;
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
        /// Initial screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        [Initialize("CTS052")]
        public ActionResult CTS052(string contractCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CommonUtil util = new CommonUtil();
                contractCode = util.ConvertContractCode(contractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                CTS052_ScreenParameter session = GetScreenObject<CTS052_ScreenParameter>();
                ViewBag.ContractCode = session.ContractCode;
                ViewBag.ImportantFlag = false;
                //HasAuthority_CTS052(contractCode); //Comment by Jutarat A. on 22052013
                //if (ViewBag.Permission == true && ViewBag.IsSystemSuspending == false && ViewBag.HasAuthorityContract == true && ViewBag.HasAuthorityOperation == true) //Comment by Jutarat A. on 22052013
                InitialScreen_CTS052(session.ContractCode.Trim());

                return View("CTS052");
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Check system suspending, user’s permission and user’s authority of screen
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS052_Authority(CTS052_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                CommonUtil util = new CommonUtil();
                ISaleContractHandler salehandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IUserControlHandler userHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                IRentralContractHandler renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                IBillingMasterHandler billingMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                IInstallationHandler installhandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                //1.1 Check suspending
                if (commonHandler.IsSystemSuspending())
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0049, null, null);
                    return Json(res);
                }

                //1.2 Check user's permission
                //if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP12_CHANGE_FEE, FunctionID.C_FUNC_ID_OPERATE))
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP12_MODIFY_INSTRUMENT_QTY, FunctionID.C_FUNC_ID_OPERATE)) //Add by Jutarat A. on 17062013
                {
                    res.AddErrorMessage("Common", MessageUtil.MessageList.MSG0053, null, null);
                    return Json(res);
                }

                // Check parameter
                //if (String.IsNullOrEmpty(param.ContractCode) && !String.IsNullOrEmpty(CommonUtil.dsTransData.dtCommonSearch.ContractCode))
                //{
                //    param.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                //}
                if (String.IsNullOrEmpty(param.ContractCode) && param.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                        param.ContractCode = param.CommonSearch.ContractCode;
                }

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

                //Add by Jutarat A. on 22052013
                if (CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == contractObj[0].ContractOfficeCode; }).Count == 0
                    && CommonUtil.dsTransData.dtOfficeData.FindAll(delegate(OfficeDataDo s) { return s.OfficeCode == contractObj[0].OperationOfficeCode; }).Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                //End Add

                string strLastUnimplementOCC = renderHandler.GetLastUnimplementedOCC(contractObj[0].ContractCode);
                string strLastImplementedOCC = renderHandler.GetLastImplementedOCC(contractObj[0].ContractCode);

                if (String.IsNullOrEmpty(strLastImplementedOCC))
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3040, null, null);
                    return Json(res);
                }

                if (!String.IsNullOrEmpty(strLastUnimplementOCC))
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3285, null, null);
                    return Json(res);
                }

                var dsRentalContract = renderHandler.GetEntireContract(contractObj[0].ContractCode, strLastImplementedOCC);

                //Comment by Jutarat A. on 22052013
                //var existsOperateOffice = CommonUtil.dsTransData.dtOfficeData.Where(x => x.OfficeCode == contractObj[0].OperationOfficeCode);

                //if (contractObj[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                //        && (existsOperateOffice.Count() <= 0)
                //    )
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063, null, null);
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    return Json(res);
                //}
                //End Comment

                if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA
                        || dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG
                        || dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE
                    )
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3286, null, null);
                    return Json(res);
                }

                if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode != ProductType.C_PROD_TYPE_AL)
                    && (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode != ProductType.C_PROD_TYPE_RENTAL_SALE))
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3052, null, null);
                    return Json(res);
                }

                if (//(dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING) || //Comment by Jutarat A. on 18102013
                    (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL)
                    || (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_END)
                    || (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL))
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3283, null, null);
                    return Json(res);
                }

                if (dsRentalContract.dtTbt_RentalContractBasic[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_ALTERNATIVE_START)
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3272, null, null);
                    return Json(res);
                }

                var installStatus = installhandler.GetInstallationStatus(util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
#if !ROUND1
                if (installStatus != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                {
                    res.AddErrorMessage("Contract", MessageUtil.MessageList.MSG3284, null, null);
                    return Json(res);
                }
#endif
                InitialScreenSession_CTS052(param);
                param.ContractCode = util.ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                return InitialScreenEnvironment<CTS052_ScreenParameter>("CTS052", param, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        public ActionResult InitialScreen_CTS052(string contractCode)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonHandler;
            IUserControlHandler userHandler;
            IRentralContractHandler renderHandler;
            IBillingMasterHandler billingMasterHandler;

            CommonUtil comU = new CommonUtil();
            dsRentalContractData dsRentalContract;
            List<CTS052_DTBillingClientDetailData> listDTBillingClientDetail;
            List<string> listFieldName = new List<string>();

            string strLastUnimplementOCC;
            string strLastImplementedOCC;

            CTS052_ScreenParameter session;
            //contractCode = comU.ConvertContractCode(contractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            try
            {
                session = CTS052_GetImportData();
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                userHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                billingMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

                listDTBillingClientDetail = new List<CTS052_DTBillingClientDetailData>();

                //1.1 Validate entering conditions

                //1.1.1 Get last implemented OCC
                //-----------------------------------------------------------------------------
                //1.1.1.1 Get last implemented OCC
                strLastUnimplementOCC = renderHandler.GetLastUnimplementedOCC(contractCode);
                //1.1.1.2 Get last implemented OCC
                strLastImplementedOCC = renderHandler.GetLastImplementedOCC(contractCode);
                //-----------------------------------------------------------------------------

                //1.1.1 Initial contract data
                //-----------------------------------------------------------------------------
                //1.1.1.1 Get entire contract data
                ViewBag.OCCCode = strLastImplementedOCC;
                //1.1.1.2 Get entire contract data
                dsRentalContract = renderHandler.GetEntireContract(contractCode, strLastImplementedOCC);
                session.DSRentalContract = dsRentalContract;

                //1.1.2 Allow only alarm contract
                if (session.DSRentalContract != null)
                {

                    if ((session.DSRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode != ProductType.C_PROD_TYPE_AL)
                        && (session.DSRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode != ProductType.C_PROD_TYPE_RENTAL_SALE))
                    {
                        ViewBag.ISValidProductTypeCode = false;
                        return Json(res);
                    }
                    else
                        ViewBag.ISValidProductTypeCode = true;

                    //1.1.4 There must have implemented OCC
                    if (strLastImplementedOCC == null || strLastImplementedOCC == "")
                    {
                        ViewBag.ISValidLastImplementedOCC = false;
                        return Json(res);
                    }
                    else
                        ViewBag.ISValidLastImplementedOCC = true;

                    //1.1.3 There must no unimplement OCC
                    if (strLastUnimplementOCC != null && strLastUnimplementOCC != "")
                    {
                        ViewBag.ISValidLastUnimplementOCC = false;
                        return Json(res);
                    }
                    else
                        ViewBag.ISValidLastUnimplementOCC = true;

                    //1.2 Get data for ucRentalContractBasicInformation
                    session.DORentalContractBasicInformation = userHandler.GetRentalContactBasicInformationData(contractCode.Trim());

                    //1.3 Bind data to screen items
                    InitialScreenData_CTS052(contractCode, strLastImplementedOCC);

                    //3.1 For Event Search Quotation
                    ViewBag.ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL;
                    ViewBag.TargetCodeTypeCode = TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE;
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
        public void InitialScreenData_CTS052(string contractCode, string occ)
        {
            IUserControlHandler userHandler;
            IRentralContractHandler renderHandler;
            dtTbt_RentalSecurityBasicForView dtTbt_RentalSecurityBasicForView;

            try
            {
                userHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;
                renderHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                doRentalContractBasicInformation doRental = userHandler.GetRentalContactBasicInformationData(contractCode.Trim());
                dtTbt_RentalSecurityBasicForView = renderHandler.GetTbt_RentalSecurityBasicForView(contractCode.Trim(), occ.Trim())[0];
                if (dtTbt_RentalSecurityBasicForView != null)
                    ViewBag.ChangeImplementDate = CommonUtil.TextDate(dtTbt_RentalSecurityBasicForView.ChangeImplementDate);

                Bind_CTS052(doRental);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Initial session of screen
        /// </summary>
        /// <param name="importData"></param>
        public void InitialScreenSession_CTS052(CTS052_ScreenParameter importData)
        {
            try
            {
                importData.DOValidateBusiness = new CTS052_DOValidateBusinessData();
                importData.DSRentalContract = new dsRentalContractData();
                importData.ListRentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                importData.DORentalContractBasicInformation = new doRentalContractBasicInformation();
                importData.DOQuotation = new CTS052_DOQuotation();
                CTS052_SetImportData(importData);
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
        public void Bind_CTS052(doRentalContractBasicInformation doRental)
        {
            CommonUtil comU;
            CTS052_ScreenParameter session;

            try
            {
                comU = new CommonUtil();
                session = CTS052_GetImportData();

                if (session.DSRentalContract.dtTbt_RentalSecurityBasic != null)
                {
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic.Count() != 0)
                    {
                        //ViewBag.QuotationTargetCode = comU.ConvertQuotationTargetCode(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        ViewBag.QuotationTargetCode = doRental.ContractCodeShort;
                    }
                }

                ViewBag.ContractCodeLong = ViewBag.ContractCode;
                ViewBag.ContractCode = doRental.ContractCodeShort;
                ViewBag.UserCode = doRental.UserCode;
                ViewBag.CustomerCode = doRental.ContractTargetCustCodeShort;
                ViewBag.RealCustomerCode = comU.ConvertCustCode(doRental.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                ViewBag.SiteCode = doRental.SiteCodeShort;

                if (doRental.ContractTargetCustomerImportant == null)
                    ViewBag.ImportantFlag = false;
                else
                    ViewBag.ImportantFlag = (bool)doRental.ContractTargetCustomerImportant;

                ViewBag.CustFullNameEN = doRental.ContractTargetNameEN;
                ViewBag.CustFullNameLC = doRental.ContractTargetNameLC;
                ViewBag.AddressFullEN = doRental.ContractTargetAddressEN;
                ViewBag.AddressFullLC = doRental.ContractTargetAddressLC;
                ViewBag.SiteNameEN = doRental.SiteNameEN;
                ViewBag.SiteNameLC = doRental.SiteNameLC;
                ViewBag.SiteAddressEN = doRental.SiteAddressEN;
                ViewBag.SiteAddressLC = doRental.SiteAddressLC;
                ViewBag.InstallationStatus = CommonUtil.TextCodeName(doRental.InstallationStatusCode, doRental.InstallationStatusName);
                ViewBag.InstallationStatusCode = doRental.InstallationStatusCode;
                ViewBag.OperationOffice = doRental.OperationOfficeCode;
                ViewBag.OfficeName = CommonUtil.TextCodeName(doRental.OperationOfficeCode, doRental.OperationOfficeName);
                ViewBag.ServiceTypeCode = SECOM_AJIS.Common.Util.ConstantValue.ServiceType.C_SERVICE_TYPE_RENTAL;
                ViewBag.TargetCodeTypeCode = SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE;


                //if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee != null)
                //{
                //    ViewBag.ContractFeeCurrency = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrency;
                //    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrency == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                //        ViewBag.ContractFee = CommonUtil.TextNumeric(decimal.Parse(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeUS.ToString()));
                //    else
                //        ViewBag.ContractFee = CommonUtil.TextNumeric(decimal.Parse(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee.ToString()));
                //}
                ViewBag.ContractFeeCurrencyType = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Get Quotation information data
        /// </summary>
        /// <returns></returns>
        public CTS052_DOQuotation GetQuotationInformation_CTS052()
        {
            CTS052_ScreenParameter session;
            CTS052_DOQuotation doQuotation;
            try
            {
                session = CTS052_GetImportData();
                doQuotation = new CTS052_DOQuotation();

                doQuotation.ChangeImplementDate = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;
                doQuotation.ChangeImplementDateShow = CommonUtil.TextDate(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate);

                doQuotation.ContractFeeCurrencyType = session.DSQuotation.dtTbt_QuotationBasic.ContractFeeCurrencyType;
                if (doQuotation.ContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    doQuotation.ContractFee = decimal.Parse(session.DSQuotation.dtTbt_QuotationBasic.ContractFeeUsd.ToString()).ToString("#,##0.00");
                else
                    doQuotation.ContractFee = decimal.Parse(session.DSQuotation.dtTbt_QuotationBasic.ContractFee.ToString()).ToString("#,##0.00");

                doQuotation.OrderContractFeeCurrencyType = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType;
                if (doQuotation.OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    doQuotation.OrderContractFee = decimal.Parse(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd.ToString()).ToString("#,##0.00");
                else
                    doQuotation.OrderContractFee = decimal.Parse(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee.ToString()).ToString("#,##0.00");

                doQuotation.NegotiationStaffEmpNo1 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1;
                doQuotation.NegotiationStaffEmpName1 = GetEmployeeName_CTS052(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1);
                doQuotation.NegotiationStaffEmpNo2 = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo2;
                doQuotation.NegotiationStaffEmpName2 = GetEmployeeName_CTS052(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo2);
                doQuotation.ApproveNo1 = session.DSQuotation.dtTbt_QuotationBasic.ApproveNo1;
                doQuotation.ApproveNo2 = session.DSQuotation.dtTbt_QuotationBasic.ApproveNo2;
                doQuotation.Alphabet = session.DSQuotation.dtTbt_QuotationBasic.Alphabet;
                session.DOQuotation = doQuotation;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return doQuotation;
        }

        /// <summary>
        /// Get data of Quotation Information
        /// </summary>
        /// <returns></returns>
        public ActionResult GetQuotationInformationData_CTS052()
        {
            ObjectResultData res = new ObjectResultData();
            CTS052_ScreenParameter session;

            try
            {
                session = CTS052_GetImportData();
                GetQuotationInformation_CTS052();
                return Json(session.DOQuotation);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get Employee name by EmpNo
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public string GetEmployeeName_CTS052(string empNo)
        {
            if (string.IsNullOrEmpty(empNo))
            {
                return null;
            }

            ObjectResultData res = new ObjectResultData();
            IMasterHandler masterHandler;
            IEmployeeMasterHandler employeeHandler;
            List<dtEmpNo> listEmpNo = null;

            try
            {
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                listEmpNo = employeeHandler.GetEmployeeNameByEmpNo(empNo);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            if (listEmpNo != null && listEmpNo.Count > 0)
            {
                return listEmpNo[0].EmployeeNameDisplay.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Employee name when leave form EmployeeNo textbox
        /// </summary>
        /// <param name="doGetActiveEmployeeData"></param>
        /// <returns></returns>
        public ActionResult GetActiveEmployee_CTS052(CTS052_DOGetActiveEmployeeData doGetActiveEmployeeData)
        {
            ObjectResultData res = new ObjectResultData();
            IMasterHandler masterHandler;
            IEmployeeMasterHandler employeeHandler;
            List<tbm_Employee> listEmployee;
            List<dtEmpNo> listEmpNo;

            try
            {
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                listEmployee = masterHandler.GetActiveEmployee(doGetActiveEmployeeData.EmpNo);

                if (listEmployee.Count() == 0)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, "");
                    return Json(res);
                }

                listEmpNo = employeeHandler.GetEmployeeNameByEmpNo(doGetActiveEmployeeData.EmpNo);
                if (listEmpNo.Count() != 0)
                    doGetActiveEmployeeData.EmpName = listEmpNo[0].EmployeeNameDisplay;

                return Json(doGetActiveEmployeeData);
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
        public ActionResult ResetData_CTS052()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                CTS052_ScreenParameter session = GetScreenObject<CTS052_ScreenParameter>();
                InitialScreen_CTS052(session.ContractCode);
                session = GetScreenObject<CTS052_ScreenParameter>();

                CTS052_ScreenOutputObject outObj = new CTS052_ScreenOutputObject()
                {
                    AddressFullEN = session.DORentalContractBasicInformation.ContractTargetAddressEN,
                    AddressFullLC = session.DORentalContractBasicInformation.ContractTargetAddressLC,
                    Alphabet = "",
                    ContractCode = session.DORentalContractBasicInformation.ContractCode,
                    ContractCodeShort = session.DORentalContractBasicInformation.ContractCodeShort,
                    CustFullNameEN = session.DORentalContractBasicInformation.ContractTargetNameEN,
                    CustFullNameLC = session.DORentalContractBasicInformation.ContractTargetNameLC,
                    CustomerCode = session.DORentalContractBasicInformation.ContractTargetCustCodeShort,
                    RealCustomerCode = session.DORentalContractBasicInformation.RealCustomerCustCodeShort,
                    DisplayAll = "",
                    SiteAddress = session.DORentalContractBasicInformation.SiteAddressEN,
                    SiteAddressLC = session.DORentalContractBasicInformation.SiteAddressLC,
                    SiteCode = session.DORentalContractBasicInformation.SiteCodeShort,
                    SiteName = session.DORentalContractBasicInformation.SiteNameEN,
                    SiteNameLC = session.DORentalContractBasicInformation.SiteNameLC,
                    InstallationStatus = CommonUtil.TextCodeName(session.DORentalContractBasicInformation.InstallationStatusCode, session.DORentalContractBasicInformation.InstallationStatusName),
                    InstallationStatusCode = session.DORentalContractBasicInformation.InstallationStatusCode,
                    OfficeName = CommonUtil.TextCodeName(session.DORentalContractBasicInformation.OperationOfficeCode, session.DORentalContractBasicInformation.OperationOfficeName),
                    EndContractDate = CommonUtil.TextDate(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractEndDate),
                    ImportantFlag = session.DORentalContractBasicInformation.ContractTargetCustomerImportant.GetValueOrDefault(),
                    UserCode = session.DORentalContractBasicInformation.UserCode,
                    ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL,
                    TargetCodeType = TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE,
                    Sequence = "",
                    OCC = session.DSRentalContract.dtTbt_RentalContractBasic[0].LastOCC,
                    QuotationTargetCode = session.DORentalContractBasicInformation.ContractCodeShort,
                    ContractStatus = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractStatus,
                    ExpectOperationDate = CommonUtil.TextDate(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ExpectedOperationDate),
                    BillingClientCode = "",
                    BillingOffice = "",
                    EmpName = "",
                    EmpNo = "",
                    NegotiationStaffEmpNo1 = "",
                    PaymentMethod = ""
                };

                res.ResultData = outObj;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        #endregion

        #region Validate

        /// <summary>
        /// Check screen has authority
        /// </summary>
        /// <param name="contractCode"></param>
        public void HasAuthority_CTS052(string contractCode)
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

                if (commonHandler.IsSystemSuspending())
                    ViewBag.IsSystemSuspending = true;
                else
                    ViewBag.IsSystemSuspending = false;

                //1.2 Check user's permission
                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP12_CHANGE_FEE, FunctionID.C_FUNC_ID_OPERATE))
                    ViewBag.Permission = false;
                else
                    ViewBag.Permission = true;

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

        /// <summary>
        /// Validate retrieve Quotation data when click [Retrieve] button in ‘Specify quotation’ section
        /// </summary>
        /// <param name="doValidateRetrieveData"></param>
        /// <returns></returns>
        public ActionResult ValidateRetrieve_CTS052(CTS052_DOValidateRetrieveData doValidateRetrieveData)
        {
            ObjectResultData res = new ObjectResultData(); ;

            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.ResultData = false;
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
        /// Validate Business of screen
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ObjectResultData ValidateBusiness_CTS052(CTS052_DOValidateBusinessData doValidateBusiness)
        {
            CTS052_ScreenParameter session;
            IMasterHandler masterHandler;
            List<tbm_Employee> listEmployee;
            ObjectResultData res = new ObjectResultData(); ;

            try
            {
                session = CTS052_GetImportData();
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                if (doValidateBusiness.ChangeImplementDate.Value.Date > DateTime.Now.Date)
                {
                    string[] param = { CommonUtil.GetLabelFromResource("Contract", "CTS052", "lblRealInvestigationDate") };
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0009, "Real investigation date");
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0009, param, null);
                    return res;
                }

                if (doValidateBusiness.ChangeImplementDate.Value.Date < session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate.Value.Date)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3041, null);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3041, null, null);
                    return res;
                }

                if (doValidateBusiness.NegotiationStaffEmpNo1 != "" && doValidateBusiness.NegotiationStaffEmpNo1 != null)
                {
                    listEmployee = masterHandler.GetActiveEmployee(doValidateBusiness.NegotiationStaffEmpNo1);
                    if (listEmployee.Count() == 0)
                    {
                        string[] param = { CommonUtil.GetLabelFromResource("Contract", "CTS052", "lblNegotiationStaff1") };
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, param);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, param);
                        return res;
                    }
                }

                if (doValidateBusiness.NegotiationStaffEmpNo2 != "" && doValidateBusiness.NegotiationStaffEmpNo2 != null)
                {
                    listEmployee = masterHandler.GetActiveEmployee(doValidateBusiness.NegotiationStaffEmpNo2);
                    if (listEmployee.Count() == 0)
                    {
                        string[] param = { CommonUtil.GetLabelFromResource("Contract", "CTS052", "lblNegotiationStaff2") };
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, param);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, param);
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return null;
        }

        /// <summary>
        /// Validate data when retrieve BillingClient
        /// </summary>
        /// <param name="doRetrieveBillingClient"></param>
        /// <returns></returns>
        public ActionResult ValidateRetrieveBillingClient_CTS052(CTS052_DORetrieveBillingClientData doRetrieveBillingClient)
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
                        res.ResultData = false;
                        return Json(res);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Json(res);
        }

        #endregion

        #region Session

        /// <summary>
        /// Get import data from screen
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private CTS052_ScreenParameter CTS052_GetImportData(string key = null)
        {
            try
            {
                return GetScreenObject<CTS052_ScreenParameter>(key);
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
        private void CTS052_SetImportData(CTS052_ScreenParameter import, string key = null)
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
        public ActionResult CTS052_ClearSession()
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
