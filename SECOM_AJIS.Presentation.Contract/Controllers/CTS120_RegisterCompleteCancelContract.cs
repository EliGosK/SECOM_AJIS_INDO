//*********************************
// Create by: Natthavat S.
// Create date: 04/Oct/2011
// Update date: 04/Oct/2011
//*********************************

//#define ROUND1

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
namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {
        #region Authority

        /// <summary>
        /// Check screen permission
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS120_Authority(CTS120_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();

            try
            {
                //if (String.IsNullOrEmpty(param.strContractCode) && (!String.IsNullOrEmpty(CommonUtil.dsTransData.dtCommonSearch.ContractCode)))
                //{
                //    param.strContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                //}
                if (String.IsNullOrEmpty(param.strContractCode) && param.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(param.CommonSearch.ContractCode) == false)
                        param.strContractCode = param.CommonSearch.ContractCode;
                }

                res = ValidateAuthority_CTS120(res);
                if (res.IsError)
                {
                    return Json(res);
                }

                //Add by Jutarat A. on 08082012
                if (String.IsNullOrEmpty(param.strContractCode) == false)
                {
                    IRentralContractHandler rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    string strContractCodeLong = comUtil.ConvertContractCode(param.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

                    List<tbt_RentalContractBasic> rentalContractBasicList = rentralHandler.GetTbt_RentalContractBasic(strContractCodeLong, null);
                    if ((rentalContractBasicList == null) || (rentalContractBasicList.Count < 1))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124, null, null);
                        return Json(res);
                    }
                }
                //End Add

                var validObj = CheckInitialParameter_CTS120(param.strContractCode);
                if (validObj.IsError)
                {
                    validObj.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    validObj.ResultData = null;
                    return Json(validObj);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS120_ScreenParameter>("CTS120", param, res);
        }

        //public ActionResult CTS120_Authority(string strContractCode)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    CommonUtil comUtil = new CommonUtil();
        //    CTS120_ScreenParameter sParam = null;

        //    try
        //    {
        //        // Check Screen Permission
        //        if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP99, FunctionID.C_FUNC_ID_OPERATE) == false)
        //        {
        //            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
        //            return Json(res);
        //        }

        //        // Check System Status
        //        if (CheckIsSuspending(res))
        //        {
        //            return Json(res);
        //        }

        //        sParam = new CTS120_ScreenParameter()
        //        {
        //            strContractCode = strContractCode
        //        };

        //        //SetScreenParameter_CTS120(sParam);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }


        //    return InitialScreenEnvironment("CTS120", sParam);
        //}
        
        #endregion

        #region Action

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS120")]
        public ActionResult CTS120() // InitialState
        {
            return View();
        }

        /// <summary>
        /// Checking initial parameter from another screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS120_CheckInitialParameter()
        {
            ObjectResultData res = new ObjectResultData();

            CTS120_ScreenParameter sParam = GetScreenObject<CTS120_ScreenParameter>();
            //CTS120_ScreenParameter sParam = GetScreenObject_CTS120();
            return Json(CheckInitialParameter_CTS120(sParam.strContractCode));
        }

        /// <summary>
        /// Retrieve contract data from contract code when click [Retrieve] button  on ‘Specify contract code’ section or has parameter from another screen
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS120_RetrieveRentalContractData(CTS120_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            IRentralContractHandler rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            CommonUtil util = new CommonUtil();

            // Check Mandatory
            if (String.IsNullOrEmpty(param.strContractCode))
            {
                // Error MSG0007
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS120", "lblEntryContractCode") }, new string[] { "EntryContractCode" });
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                return Json(res);
            }

            // Try Convert contract code to long
            string longContractCode = "";
            try
            {
                longContractCode = util.ConvertContractCode(param.strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

            // Check Exists Contract
            var dsRentalContract = rentralHandler.GetEntireContract(longContractCode, null);
            if ((dsRentalContract == null) || (dsRentalContract.dtTbt_RentalContractBasic == null) || (dsRentalContract.dtTbt_RentalContractBasic.Count == 0))
            {
                // Error MSG0011
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] {param.strContractCode}, null);
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                return Json(res);
            }

            // Check Data Authority
            if (!CheckDataAuthority_CTS120(res, param.strContractCode))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                return Json(res);
            }

            // RetrieveRentalContractData
            res = RetrieveRentalContractData_CTS120(param.strContractCode);
            
            return Json(res);
        }

        /// <summary>
        /// Proceed cancel contact when click [OK] button 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS120_CompleteCacelContract(CTS120_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                // Check Authority
                res = ValidateAuthority_CTS120(res);
                if (res.IsError)
                {
                    return Json(res);
                }

                // Check Mandatory
                if (String.IsNullOrEmpty(param.strContractCode))
                {
                    // Error MSG0007
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS120", "lblEntryContractCode") }, new string[] { "EntryContractCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                // Check System Status
                if (CheckIsSuspending(res))
                {
                    return Json(res);
                }

                // Check Validate Business
                if (!ValidateBusiness_CS120(res, param.strContractCode))
                {
                    return Json(res);
                }

                // Check exists contract code
                var tmpObj = RetrieveRentalContractData_CTS120(param.strContractCode);
                if (tmpObj.IsError)
                {
                    res = tmpObj;
                    return Json(res);
                }

                // Begin Update
                if (RegisterCompleteContract_CTS120(res, param.strContractCode))
                {
                    res.ResultData = true;
                }
                else
                {
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
        /// Clear contract data from parameter when click [Clear] button  on ‘Specify contract code’ section
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS120_ClearContract()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = String.Empty;
                //CommonUtil.dsTransData.dtCommonSearch.ProjectCode = String.Empty;

                CTS120_ScreenParameter param = GetScreenObject<CTS120_ScreenParameter>();
                param.CommonSearch = new ScreenParameter.CommonSearchDo();
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
        /// Checking and validate parameter when enter screen
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        private ObjectResultData CheckInitialParameter_CTS120(string contractCode)
        {
            ObjectResultData res = new ObjectResultData();

            if ((!String.IsNullOrEmpty(contractCode)))
            {
                var tmpObj = RetrieveRentalContractData_CTS120(contractCode);
                if (!tmpObj.IsError)
                {
                    res.ResultData = contractCode;
                }
                else
                {
                    //if (tmpObj.Message.Code == MessageUtil.MessageList.MSG0111.ToString())
                    //{
                    //    tmpObj = new ObjectResultData();
                    //    tmpObj.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124, null, null);
                    //}
                    res = tmpObj;
                }
            }

            return res;
        }

        /// <summary>
        /// Checking data authority when retrieve data or proceed cancel contact
        /// </summary>
        /// <param name="res"></param>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        private bool CheckDataAuthority_CTS120(ObjectResultData res, string strContractCode)
        {
            CommonUtil comUtil = new CommonUtil();
            dsTransDataModel dsTrans = null;
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;
            bool result = false;

            try
            {
                dsTrans = CommonUtil.dsTransData;
                rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                dsRentalContract = rentralHandler.GetEntireContract(comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);

                if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null)
                {
                    foreach (var dsTranObj in dsTrans.dtOfficeData)
                    {
                        var cntItem = (from a in dsRentalContract.dtTbt_RentalContractBasic
                                       where a.OperationOfficeCode == dsTranObj.OfficeCode
                                       select a);

                        if (cntItem.Count() > 0)
                        {
                            result = true;
                            break;
                        }
                    }
                }

                //var resDat = from a in dsTrans.dtOfficeData where a.OfficeCode == dsRentalContract.dtTbt_RentalContractBasic
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return result;
        }

        /// <summary>
        /// Validating business before register cancle contract
        /// </summary>
        /// <param name="res"></param>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        private bool ValidateBusiness_CS120(ObjectResultData res, string strContractCode)
        {
            CommonUtil comUtil = new CommonUtil();
            dsTransDataModel dsTrans = null;
            IRentralContractHandler rentralHandler;
            dsRentalContractData dsRentalContract = null;
            bool result = false;

            try
            {
                dsTrans = CommonUtil.dsTransData;
                rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                ICommonContractHandler commoncontracthandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                dsRentalContract = rentralHandler.GetEntireContract(comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);

                if ((dsRentalContract != null) && (dsRentalContract.dtTbt_RentalContractBasic != null) && (dsRentalContract.dtTbt_RentalContractBasic.Count > 0))
                {
                    if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3116, new string[] { strContractCode }, new string[] { "EntryContractCode" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return result;
                    }

                    //Add by Jutarat A. on 28092012
                    if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_CANCEL)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3306, null, new string[] { "EntryContractCode" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return result;
                    }
                    //End Add

                    if ( //(dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_CANCEL) && //Comment by Jutarat A. on 28092012
                        (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_END))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3115, null, new string[] { "EntryContractCode" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return result;
                    }

                    //if (dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationTypeCode != RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3117, null, new string[] { "EntryContractCode" });
                    //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    //    return result;
                    //}

                    #if !ROUND1
                    if (!commoncontracthandler.IsCompleteRemoveAll(comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3117, null, new string[] { "EntryContractCode" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return result;
                    }
                    #endif

                    result = true;
                }
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { strContractCode });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return result;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return result;
        }

        /// <summary>
        /// Validate data and retrieve rental contract data to display
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        private ObjectResultData RetrieveRentalContractData_CTS120(string strContractCode)
        {
            ObjectResultData res = new ObjectResultData();
            dsRentalContractData dsRentalContract = null;
            CTS120_View viewObj = null;
            CommonUtil comUtil = new CommonUtil();
            IRentralContractHandler rentralHandler;
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                
                rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler; ;
                List<string> miscNameList = new List<string>();
                miscNameList.Add(MiscType.C_STOP_CANCEL_REASON_TYPE);

                var miscList = commonhandler.GetMiscTypeCodeListByFieldName(miscNameList);

                // Get rental contract data
                dsRentalContract = rentralHandler.GetEntireContract(comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);

                // Check rental contract data
                if (dsRentalContract == null || dsRentalContract.dtTbt_RentalContractBasic == null || dsRentalContract.dtTbt_RentalContractBasic.Count < 1)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, new string[] { CommonUtil.GetLabelFromResource("Contract", "CTS120", "lblEntryContractCode") }, new string[] { "EntryContractCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                // Check data authority
                if (!CheckDataAuthority_CTS120(res, strContractCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return res;
                }

                // Validate business
                if (!ValidateBusiness_CS120(res, strContractCode))
                {
                    return res;
                }

                // Get doRentalContractBasicInformation
                var lstdoRentalContractBasicInformation = rentralHandler.GetRentalContractBasicInformation(comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG));
                var objOffice = from a in CommonUtil.dsTransData.dtOfficeData where a.OfficeCode == lstdoRentalContractBasicInformation[0].OperationOfficeCode select a;
                var objOfficeLst = objOffice.ToList();
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(objOfficeLst);

                string CancelReasonText = "";
                var tmpCancelReason = from a in miscList where a.ValueCode == dsRentalContract.dtTbt_RentalSecurityBasic[0].StopCancelReasonType select a;

                if (tmpCancelReason.Count() == 1)
                {
                    var tmpCancelReasonLst = tmpCancelReason.ToList();
                    CommonUtil.MappingObjectLanguage<doMiscTypeCode>(tmpCancelReasonLst);
                    CancelReasonText = tmpCancelReasonLst[0].ValueCodeDisplay;
                }

                viewObj = new CTS120_View()
                    {
                        ContractCode = lstdoRentalContractBasicInformation[0].ContractCodeShort,
                        UserCode = lstdoRentalContractBasicInformation[0].UserCode,
                        ContractTargetCustCodeShort = lstdoRentalContractBasicInformation[0].ContractTargetCustCodeShort,
                        RealCustomerCustCodeShort = lstdoRentalContractBasicInformation[0].RealCustomerCustCodeShort,
                        SiteCodeShort = lstdoRentalContractBasicInformation[0].SiteCodeShort,
                        chkContractTargetFlag = lstdoRentalContractBasicInformation[0].ContractTargetCustomerImportant.GetValueOrDefault(),
                        ContractTargetNameEN = lstdoRentalContractBasicInformation[0].ContractTargetNameEN,
                        ContractTargetAddressLC = lstdoRentalContractBasicInformation[0].ContractTargetAddressLC,
                        ContractTargetAddressEN = lstdoRentalContractBasicInformation[0].ContractTargetAddressEN,
                        ContractTargetNameLC = lstdoRentalContractBasicInformation[0].ContractTargetNameLC,
                        SiteNameEN = lstdoRentalContractBasicInformation[0].SiteNameEN,
                        SiteAddressEN = lstdoRentalContractBasicInformation[0].SiteAddressEN,
                        SiteNameLC = lstdoRentalContractBasicInformation[0].SiteNameLC,
                        SiteAddressLC = lstdoRentalContractBasicInformation[0].SiteAddressLC,
                        CancelDate = ((dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                        || (dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL)
                        || (dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT)) 
                        ? CommonUtil.TextDate(dsRentalContract.dtTbt_RentalSecurityBasic[0]) : String.Empty,
                        OperationOffice = (objOfficeLst.Count() > 0) ? objOfficeLst[0].OfficeCodeName : String.Empty,
                        //CancelReason = dsRentalContract.dtTbt_RentalSecurityBasic[0].StopCancelReasonType,
                        CancelReason = CancelReasonText,
                        //MonthlyContractFee = CommonUtil.TextNumeric(dsRentalContract.dtTbt_RentalContractBasic[0].LastOrderContractFee),
                        FirstOperationDate = CommonUtil.TextDate(dsRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate),
                        CanContinue = false
                    };

                viewObj.MonthlyContractFeeCurrencyType = dsRentalContract.dtTbt_RentalContractBasic[0].LastOrderContractFeeCurrencyType;
                if (viewObj.MonthlyContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    viewObj.MonthlyContractFee = CommonUtil.TextNumeric(dsRentalContract.dtTbt_RentalContractBasic[0].LastOrderContractFeeUsd);
                else
                    viewObj.MonthlyContractFee = CommonUtil.TextNumeric(dsRentalContract.dtTbt_RentalContractBasic[0].LastOrderContractFee);

                res.ResultData = viewObj;
                
                //CommonUtil.dsTransData.dtCommonSearch.ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCodeShort;

                CTS120_ScreenParameter param = GetScreenObject<CTS120_ScreenParameter>();
                if (param != null)
                {
                    param.CommonSearch = new ScreenParameter.CommonSearchDo()
                    {
                        ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCodeShort
                    };
                }
                if (viewObj != null)
                {
                    viewObj.CanContinue = true;
                    res.ResultData = viewObj;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Register complete cancle contract
        /// </summary>
        /// <param name="res"></param>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        private bool RegisterCompleteContract_CTS120(ObjectResultData res, string strContractCode)
        {
            bool result = false;
            CommonUtil comUtil = new CommonUtil();
            IRentralContractHandler rentralHandler;
            dsRentalContractData targContract;
            dsTransDataModel dsTrans = null;
            DateTime curDate;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    // Get Target Object
                    rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    dsTrans = CommonUtil.dsTransData;
                    string newOCC = rentralHandler.GenerateContractOCC(comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), true);
                    targContract = rentralHandler.GetEntireContract(comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                    curDate = DateTime.Now;

                    //using (TransactionScope scope = new TransactionScope())
                    //{
                    // Update data
                    // Update dtTbt_RentalContractBasic
                    targContract.dtTbt_RentalContractBasic[0].LastOCC = newOCC;
                    targContract.dtTbt_RentalContractBasic[0].LastChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_TERMINATED;
                    targContract.dtTbt_RentalContractBasic[0].ContractStatus = ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL;
                    targContract.dtTbt_RentalContractBasic[0].StartType = null;
                    targContract.dtTbt_RentalContractBasic[0].LastChangeImplementDate = curDate;
                    targContract.dtTbt_RentalContractBasic[0].ConfirmCancelAfterStartProcessDate = dsTrans.dtOperationData.ProcessDateTime;
                    //targContract.dtTbt_RentalContractBasic[0].UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                    //targContract.dtTbt_RentalContractBasic[0].UpdateBy = dsTrans.dtUserData.EmpNo;

                    // Update dtTbt_RentalSecurityBasic
                    tbt_RentalSecurityBasic newRentalSecurityBasic = new tbt_RentalSecurityBasic();
                    if ((targContract.dtTbt_RentalSecurityBasic != null) && (targContract.dtTbt_RentalSecurityBasic.Count > 0))
                    {
                        //Modify by Jutarat A. 03102012
                        //newRentalSecurityBasic = new tbt_RentalSecurityBasic()
                        //{
                        //    AdditionalApproveNo1 = targContract.dtTbt_RentalSecurityBasic[0].AdditionalApproveNo1,
                        //    AdditionalApproveNo2 = targContract.dtTbt_RentalSecurityBasic[0].AdditionalApproveNo2,
                        //    AdditionalApproveNo3 = targContract.dtTbt_RentalSecurityBasic[0].AdditionalApproveNo3,
                        //    AdditionalFee1 = targContract.dtTbt_RentalSecurityBasic[0].AdditionalFee1,
                        //    AdditionalFee2 = targContract.dtTbt_RentalSecurityBasic[0].AdditionalFee2,
                        //    AdditionalFee3 = targContract.dtTbt_RentalSecurityBasic[0].AdditionalFee3,
                        //    AlmightyProgramEmpNo = targContract.dtTbt_RentalSecurityBasic[0].AlmightyProgramEmpNo,
                        //    ApproveNo1 = targContract.dtTbt_RentalSecurityBasic[0].ApproveNo1,
                        //    ApproveNo2 = targContract.dtTbt_RentalSecurityBasic[0].ApproveNo2,
                        //    ApproveNo3 = targContract.dtTbt_RentalSecurityBasic[0].ApproveNo3,
                        //    ApproveNo4 = targContract.dtTbt_RentalSecurityBasic[0].ApproveNo4,
                        //    ApproveNo5 = targContract.dtTbt_RentalSecurityBasic[0].ApproveNo5,
                        //    AutoRenewMonth = targContract.dtTbt_RentalSecurityBasic[0].AutoRenewMonth,
                        //    BuildingTypeCode = targContract.dtTbt_RentalSecurityBasic[0].BuildingTypeCode,
                        //    CalContractEndDate = targContract.dtTbt_RentalSecurityBasic[0].CalContractEndDate,
                        //    CalIndex = targContract.dtTbt_RentalSecurityBasic[0].CalIndex,
                        //    //ChangeImplementDate = targContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate,
                        //    //ChangeNameReasonType = targContract.dtTbt_RentalSecurityBasic[0].ChangeNameReasonType,
                        //    //ChangeReasonType = targContract.dtTbt_RentalSecurityBasic[0].ChangeReasonType,
                        //    //ChangeType = targContract.dtTbt_RentalSecurityBasic[0].ChangeType,
                        //    CompleteChangeOperationDate = targContract.dtTbt_RentalSecurityBasic[0].CompleteChangeOperationDate,
                        //    CompleteChangeOperationEmpNo = targContract.dtTbt_RentalSecurityBasic[0].CompleteChangeOperationEmpNo,
                        //    ContractCode = targContract.dtTbt_RentalSecurityBasic[0].ContractCode,
                        //    //ContractDocPrintFlag = targContract.dtTbt_RentalSecurityBasic[0].ContractDocPrintFlag,
                        //    ContractDurationMonth = targContract.dtTbt_RentalSecurityBasic[0].ContractDurationMonth,
                        //    ContractEndDate = targContract.dtTbt_RentalSecurityBasic[0].ContractEndDate,
                        //    ContractFeeOnStop = targContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop,
                        //    ContractStartDate = targContract.dtTbt_RentalSecurityBasic[0].ContractStartDate,
                        //    CounterNo = 0,
                        //    CreateBy = targContract.dtTbt_RentalSecurityBasic[0].CreateBy,
                        //    CreateDate = targContract.dtTbt_RentalSecurityBasic[0].CreateDate,
                        //    CrimePreventFlag = targContract.dtTbt_RentalSecurityBasic[0].CrimePreventFlag,
                        //    DepositFeeBillingTiming = targContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming,
                        //    DispatchTypeCode = targContract.dtTbt_RentalSecurityBasic[0].DispatchTypeCode,
                        //    DivideContractFeeBillingFlag = targContract.dtTbt_RentalSecurityBasic[0].DivideContractFeeBillingFlag,
                        //    DocAuditResult = targContract.dtTbt_RentalSecurityBasic[0].DocAuditResult,
                        //    DocumentCode = targContract.dtTbt_RentalSecurityBasic[0].DocumentCode,
                        //    EmergencyReportFlag = targContract.dtTbt_RentalSecurityBasic[0].EmergencyReportFlag,
                        //    ExpectedInstallationCompleteDate = targContract.dtTbt_RentalSecurityBasic[0].ExpectedInstallationCompleteDate,
                        //    ExpectedOperationDate = targContract.dtTbt_RentalSecurityBasic[0].ExpectedOperationDate,
                        //    ExpectedResumeDate = targContract.dtTbt_RentalSecurityBasic[0].ExpectedResumeDate,
                        //    FacilityMemo = targContract.dtTbt_RentalSecurityBasic[0].FacilityMemo,
                        //    FacilityMonitorFlag = targContract.dtTbt_RentalSecurityBasic[0].FacilityMonitorFlag,
                        //    FacilityPassMonth = targContract.dtTbt_RentalSecurityBasic[0].FacilityPassMonth,
                        //    FacilityPassYear = targContract.dtTbt_RentalSecurityBasic[0].FacilityPassYear,
                        //    FireMonitorFlag = targContract.dtTbt_RentalSecurityBasic[0].FireMonitorFlag,
                        //    ImplementFlag = targContract.dtTbt_RentalSecurityBasic[0].ImplementFlag,
                        //    //InstallationCompleteDate = targContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteDate,
                        //    //InstallationCompleteEmpNo = targContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteEmpNo,
                        //    //InstallationCompleteFlag = targContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag,
                        //    //InstallationSlipNo = targContract.dtTbt_RentalSecurityBasic[0].InstallationSlipNo,
                        //    //InstallationTypeCode = targContract.dtTbt_RentalSecurityBasic[0].InstallationTypeCode,
                        //    //InstallFeePaidBySECOM = targContract.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOM,
                        //    //InstallFeeRevenueBySECOM = targContract.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOM,
                        //    InsuranceCoverageAmount = targContract.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmount,
                        //    InsuranceTypeCode = targContract.dtTbt_RentalSecurityBasic[0].InsuranceTypeCode,
                        //    MainStructureTypeCode = targContract.dtTbt_RentalSecurityBasic[0].MainStructureTypeCode,
                        //    MaintenanceCycle = targContract.dtTbt_RentalSecurityBasic[0].MaintenanceCycle,
                        //    MaintenanceFee1 = targContract.dtTbt_RentalSecurityBasic[0].MaintenanceFee1,
                        //    MaintenanceFee2 = targContract.dtTbt_RentalSecurityBasic[0].MaintenanceFee2,
                        //    MonthlyInsuranceFee = targContract.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFee,
                        //    NegotiationStaffEmpNo1 = targContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1,
                        //    NegotiationStaffEmpNo2 = targContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo2,
                        //    NewBldMgmtCost = targContract.dtTbt_RentalSecurityBasic[0].NewBldMgmtCost,
                        //    NewBldMgmtFlag = targContract.dtTbt_RentalSecurityBasic[0].NewBldMgmtFlag,
                        //    NormalAdditionalDepositFee = targContract.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee,
                        //    NormalContractFee = targContract.dtTbt_RentalSecurityBasic[0].NormalContractFee,
                        //    //NormalInstallFee = targContract.dtTbt_RentalSecurityBasic[0].NormalInstallFee,
                        //    NumOfBuilding = targContract.dtTbt_RentalSecurityBasic[0].NumOfBuilding,
                        //    NumOfFloor = targContract.dtTbt_RentalSecurityBasic[0].NumOfFloor,
                        //    //OCC = targContract.dtTbt_RentalSecurityBasic[0].OCC,
                        //    OrderAdditionalDepositFee = targContract.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee,
                        //    OrderContractFee = targContract.dtTbt_RentalSecurityBasic[0].OrderContractFee,
                        //    OrderContractFeePayMethod = targContract.dtTbt_RentalSecurityBasic[0].OrderContractFeePayMethod,
                        //    //OrderInstallFee = targContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee,
                        //    //OrderInstallFee_ApproveContract = targContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract,
                        //    //OrderInstallFee_CompleteInstall = targContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall,
                        //    //OrderInstallFee_StartService = targContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService,
                        //    PhoneLineOwnerTypeCode1 = targContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode1,
                        //    PhoneLineOwnerTypeCode2 = targContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode2,
                        //    PhoneLineOwnerTypeCode3 = targContract.dtTbt_RentalSecurityBasic[0].PhoneLineOwnerTypeCode3,
                        //    PhoneLineTypeCode1 = targContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode1,
                        //    PhoneLineTypeCode2 = targContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode2,
                        //    PhoneLineTypeCode3 = targContract.dtTbt_RentalSecurityBasic[0].PhoneLineTypeCode3,
                        //    PhoneNo1 = targContract.dtTbt_RentalSecurityBasic[0].PhoneNo1,
                        //    PhoneNo2 = targContract.dtTbt_RentalSecurityBasic[0].PhoneNo2,
                        //    PhoneNo3 = targContract.dtTbt_RentalSecurityBasic[0].PhoneNo3,
                        //    PlanApproveDate = targContract.dtTbt_RentalSecurityBasic[0].PlanApproveDate,
                        //    PlanApproverEmpNo = targContract.dtTbt_RentalSecurityBasic[0].PlanApproverEmpNo,
                        //    PlanApproverName = targContract.dtTbt_RentalSecurityBasic[0].PlanApproverName,
                        //    PlanCheckDate = targContract.dtTbt_RentalSecurityBasic[0].PlanCheckDate,
                        //    PlanCheckerEmpNo = targContract.dtTbt_RentalSecurityBasic[0].PlanCheckerEmpNo,
                        //    PlanCheckerName = targContract.dtTbt_RentalSecurityBasic[0].PlanCheckerName,
                        //    PlanCode = targContract.dtTbt_RentalSecurityBasic[0].PlanCode,
                        //    PlannerEmpNo = targContract.dtTbt_RentalSecurityBasic[0].PlannerEmpNo,
                        //    PlannerName = targContract.dtTbt_RentalSecurityBasic[0].PlannerName,
                        //    ProductCode = targContract.dtTbt_RentalSecurityBasic[0].ProductCode,
                        //    ProductTypeCode = targContract.dtTbt_RentalSecurityBasic[0].ProductTypeCode,
                        //    QuotationAlphabet = targContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet,
                        //    QuotationTargetCode = targContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode,
                        //    ReturnToOriginalFeeDate = targContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate,
                        //    SalesmanEmpNo1 = targContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1,
                        //    SalesmanEmpNo2 = targContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2,
                        //    SalesSupporterEmpNo = targContract.dtTbt_RentalSecurityBasic[0].SalesSupporterEmpNo,
                        //    SecurityAreaFrom = targContract.dtTbt_RentalSecurityBasic[0].SecurityAreaFrom,
                        //    SecurityAreaTo = targContract.dtTbt_RentalSecurityBasic[0].SecurityAreaTo,
                        //    SecurityMemo = targContract.dtTbt_RentalSecurityBasic[0].SecurityMemo,
                        //    SecurityTypeCode = targContract.dtTbt_RentalSecurityBasic[0].SecurityTypeCode,
                        //    SiteBuildingArea = targContract.dtTbt_RentalSecurityBasic[0].SiteBuildingArea,
                        //    SpecialInstallationFlag = targContract.dtTbt_RentalSecurityBasic[0].SpecialInstallationFlag,
                        //    //StopCancelReasonType = targContract.dtTbt_RentalSecurityBasic[0].StopCancelReasonType,
                        //    UninstallType = targContract.dtTbt_RentalSecurityBasic[0].UninstallType,
                        //};
                        newRentalSecurityBasic = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(targContract.dtTbt_RentalSecurityBasic[0]);
                        //End Modify

                        targContract.dtTbt_RentalSecurityBasic.Clear();

                        newRentalSecurityBasic.OCC = newOCC;
                        newRentalSecurityBasic.ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_TERMINATED;

                        //Add by Jutarat A. 03102012
                        newRentalSecurityBasic.QuotationTargetCode = null;
                        newRentalSecurityBasic.QuotationAlphabet = null;
                        newRentalSecurityBasic.SalesmanEmpNo1 = null;
                        newRentalSecurityBasic.SalesmanEmpNo2 = null;
                        newRentalSecurityBasic.SalesSupporterEmpNo = null;
                        //End Add

                        newRentalSecurityBasic.ChangeImplementDate = curDate;

                        //Add by Jutarat A. 03102012
                        newRentalSecurityBasic.NormalAdditionalDepositFee = null;
                        newRentalSecurityBasic.OrderAdditionalDepositFee = null;
                        newRentalSecurityBasic.DepositFeeBillingTiming = null;
                        newRentalSecurityBasic.PlanCode = null;
                        newRentalSecurityBasic.ApproveNo1 = null;
                        newRentalSecurityBasic.ApproveNo2 = null;
                        newRentalSecurityBasic.ApproveNo3 = null;
                        newRentalSecurityBasic.ApproveNo4 = null;
                        newRentalSecurityBasic.ApproveNo5 = null;
                        //End Add

                        newRentalSecurityBasic.CounterNo = 0;
                        newRentalSecurityBasic.ChangeReasonType = null;
                        newRentalSecurityBasic.ChangeNameReasonType = null;
                        newRentalSecurityBasic.StopCancelReasonType = null;
                        newRentalSecurityBasic.ContractDocPrintFlag = FlagType.C_FLAG_OFF;
                        newRentalSecurityBasic.InstallationCompleteFlag = null;
                        newRentalSecurityBasic.InstallationSlipNo = null;
                        newRentalSecurityBasic.InstallationCompleteDate = null;
                        newRentalSecurityBasic.InstallationCompleteEmpNo = null;
                        newRentalSecurityBasic.InstallationTypeCode = null;

                        //Add by Jutarat A. 03102012
                        newRentalSecurityBasic.NegotiationStaffEmpNo1 = null;
                        newRentalSecurityBasic.NegotiationStaffEmpNo2 = null;
                        //End Add

                        newRentalSecurityBasic.NormalInstallFee = null;
                        newRentalSecurityBasic.OrderInstallFee = null;
                        newRentalSecurityBasic.OrderInstallFee_ApproveContract = null;
                        newRentalSecurityBasic.OrderInstallFee_CompleteInstall = null;
                        newRentalSecurityBasic.OrderInstallFee_StartService = null;
                        newRentalSecurityBasic.InstallFeePaidBySECOM = null;
                        newRentalSecurityBasic.InstallFeeRevenueBySECOM = null;

                        //Add by Jutarat A. 03102012
                        newRentalSecurityBasic.DispatchTypeCode = null;
                        newRentalSecurityBasic.PlannerEmpNo = null;
                        newRentalSecurityBasic.PlanCheckerEmpNo = null;
                        newRentalSecurityBasic.PlanCheckDate = null;
                        newRentalSecurityBasic.PlanApproverEmpNo = null;
                        newRentalSecurityBasic.PlanApproveDate = null;
                        //End Add

                        newRentalSecurityBasic.CreateBy = dsTrans.dtUserData.EmpNo;
                        newRentalSecurityBasic.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        targContract.dtTbt_RentalSecurityBasic.Add(newRentalSecurityBasic);
                    }
                    else
                    {
                        //targContract.dtTbt_RentalSecurityBasic = new List<tbt_RentalSecurityBasic>();
                        //newRentalSecurityBasic.ContractCode = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }

                    // Update dtTbt_RentalBEDetails
                    //targContract.dtTbt_RentalBEDetails[0].OCC = newOCC;

                    tbt_RentalBEDetails newRentalBEDetail = new tbt_RentalBEDetails(); ;

                    if ((targContract.dtTbt_RentalBEDetails != null) && (targContract.dtTbt_RentalBEDetails.Count > 0))
                    {
                        newRentalBEDetail = new tbt_RentalBEDetails()
                        {
                            ContractCode = targContract.dtTbt_RentalBEDetails[0].ContractCode,
                            FreqOfGateUsage = targContract.dtTbt_RentalBEDetails[0].FreqOfGateUsage,
                            NotifyTime = targContract.dtTbt_RentalBEDetails[0].NotifyTime,
                            NumOfBeatStep = targContract.dtTbt_RentalBEDetails[0].NumOfBeatStep,
                            NumOfClockKey = targContract.dtTbt_RentalBEDetails[0].NumOfClockKey,
                            NumOfDayTimeSat = targContract.dtTbt_RentalBEDetails[0].NumOfDayTimeSat,
                            NumOfDate = targContract.dtTbt_RentalBEDetails[0].NumOfDate,
                            NumOfDayTimeSun = targContract.dtTbt_RentalBEDetails[0].NumOfDayTimeSun,
                            NumOfDayTimeWd = targContract.dtTbt_RentalBEDetails[0].NumOfDayTimeWd,
                            NumOfNightTimeSat = targContract.dtTbt_RentalBEDetails[0].NumOfNightTimeSat,
                            NumOfNightTimeSun = targContract.dtTbt_RentalBEDetails[0].NumOfNightTimeSun,
                            NumOfNightTimeWd = targContract.dtTbt_RentalBEDetails[0].NumOfNightTimeWd,
                        };

                        targContract.dtTbt_RentalBEDetails.Clear();

                        newRentalBEDetail.OCC = newOCC;
                        newRentalBEDetail.CreateBy = dsTrans.dtUserData.EmpNo;
                        newRentalBEDetail.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        //newRentalBEDetail.UpdateBy = dsTrans.dtUserData.EmpNo;
                        //newRentalBEDetail.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        targContract.dtTbt_RentalBEDetails.Add(newRentalBEDetail);
                    }
                    else
                    {
                        //targContract.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>();
                        //newRentalBEDetail.ContractCode = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }

                    // Update dtTbt_RentalInstrumentDetails
                    tbt_RentalInstrumentDetails newRentalInstrumentDetail = new tbt_RentalInstrumentDetails();

                    if ((targContract.dtTbt_RentalInstrumentDetails != null) && (targContract.dtTbt_RentalInstrumentDetails.Count > 0))
                    {
                        newRentalInstrumentDetail = new tbt_RentalInstrumentDetails()
                        {
                            AdditionalInstrumentQty = targContract.dtTbt_RentalInstrumentDetails[0].AdditionalInstrumentQty,
                            ContractCode = targContract.dtTbt_RentalInstrumentDetails[0].ContractCode,
                            InstrumentCode = targContract.dtTbt_RentalInstrumentDetails[0].InstrumentCode,
                            InstrumentQty = targContract.dtTbt_RentalInstrumentDetails[0].InstrumentQty,
                            InstrumentTypeCode = targContract.dtTbt_RentalInstrumentDetails[0].InstrumentTypeCode,
                            RemovalInstrumentQty = targContract.dtTbt_RentalInstrumentDetails[0].RemovalInstrumentQty,
                        };

                        targContract.dtTbt_RentalInstrumentDetails.Clear();

                        newRentalInstrumentDetail.OCC = newOCC;
                        newRentalInstrumentDetail.CreateBy = dsTrans.dtUserData.EmpNo;
                        newRentalInstrumentDetail.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        //newRentalInstrumentDetail.UpdateBy = dsTrans.dtUserData.EmpNo;
                        //newRentalInstrumentDetail.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        targContract.dtTbt_RentalInstrumentDetails.Add(newRentalInstrumentDetail);
                    }
                    else
                    {
                        //targContract.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                        //newRentalInstrumentDetail.ContractCode = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }

                    // Update dtTbt_RentalMaintenanceDetails
                    tbt_RentalMaintenanceDetails newRentalMaintenanceDetail = new tbt_RentalMaintenanceDetails();

                    if ((targContract.dtTbt_RentalMaintenanceDetails != null) && (targContract.dtTbt_RentalMaintenanceDetails.Count > 0))
                    {
                        newRentalMaintenanceDetail = new tbt_RentalMaintenanceDetails()
                        {
                            ContractCode = targContract.dtTbt_RentalMaintenanceDetails[0].ContractCode,
                            MaintenanceContractStartMonth = targContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartMonth,
                            MaintenanceContractStartYear = targContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceContractStartYear,
                            MaintenanceFeeTypeCode = targContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode,
                            MaintenanceMemo = targContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceMemo,
                            MaintenanceTargetProductTypeCode = targContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceTargetProductTypeCode,
                            MaintenanceTypeCode = targContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceTypeCode
                        };

                        targContract.dtTbt_RentalMaintenanceDetails.Clear();

                        newRentalMaintenanceDetail.OCC = newOCC;
                        newRentalMaintenanceDetail.CreateBy = dsTrans.dtUserData.EmpNo;
                        newRentalMaintenanceDetail.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        //newRentalMaintenanceDetail.UpdateBy = dsTrans.dtUserData.EmpNo;
                        //newRentalMaintenanceDetail.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        targContract.dtTbt_RentalMaintenanceDetails.Add(newRentalMaintenanceDetail);
                    }
                    else
                    {
                        //targContract.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                        //newRentalMaintenanceDetail.ContractCode = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }

                    // Update dtTbt_RentalOperationType
                    tbt_RentalOperationType newRentalOperationType = new tbt_RentalOperationType();

                    if ((targContract.dtTbt_RentalOperationType != null) && (targContract.dtTbt_RentalOperationType.Count > 0))
                    {
                        newRentalOperationType = new tbt_RentalOperationType()
                        {
                            ContractCode = targContract.dtTbt_RentalOperationType[0].ContractCode,
                            OperationTypeCode = targContract.dtTbt_RentalOperationType[0].OperationTypeCode
                        };

                        targContract.dtTbt_RentalOperationType.Clear();

                        newRentalOperationType.OCC = newOCC;
                        newRentalOperationType.CreateBy = dsTrans.dtUserData.EmpNo;
                        newRentalOperationType.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        //newRentalOperationType.UpdateBy = dsTrans.dtUserData.EmpNo;
                        //newRentalOperationType.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        targContract.dtTbt_RentalOperationType.Add(newRentalOperationType);
                    }
                    else
                    {
                        //targContract.dtTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                        //newRentalOperationType.ContractCode = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }

                    // Update dtTbt_RentalSentryGuard
                    tbt_RentalSentryGuard newRentalSentryGuard = new tbt_RentalSentryGuard();

                    if ((targContract.dtTbt_RentalSentryGuard != null) && (targContract.dtTbt_RentalSentryGuard.Count > 0))
                    {
                        newRentalSentryGuard = new tbt_RentalSentryGuard()
                        {
                            ContractCode = targContract.dtTbt_RentalSentryGuard[0].ContractCode,
                            OtherItemFee = targContract.dtTbt_RentalSentryGuard[0].OtherItemFee,
                            SecurityItemFee = targContract.dtTbt_RentalSentryGuard[0].SecurityItemFee,
                            SentryGuardFee = targContract.dtTbt_RentalSentryGuard[0].SentryGuardFee,
                            SentryGuardAreaTypeCode = targContract.dtTbt_RentalSentryGuard[0].SentryGuardAreaTypeCode,
                            TotalSentryGuardFee = targContract.dtTbt_RentalSentryGuard[0].TotalSentryGuardFee,
                        };
                        targContract.dtTbt_RentalSentryGuard.Clear();

                        newRentalSentryGuard.OCC = newOCC;
                        newRentalSentryGuard.CreateBy = dsTrans.dtUserData.EmpNo;
                        newRentalSentryGuard.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        //newRentalSentryGuard.UpdateBy = dsTrans.dtUserData.EmpNo;
                        //newRentalSentryGuard.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        targContract.dtTbt_RentalSentryGuard.Add(newRentalSentryGuard);
                    }
                    else
                    {
                        //targContract.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>();
                        //newRentalSentryGuard.ContractCode = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }

                    // Update dtTbt_RentalSentryGuardDetails
                    tbt_RentalSentryGuardDetails newRentalSentryGuardDetails = new tbt_RentalSentryGuardDetails();

                    if ((targContract.dtTbt_RentalSentryGuardDetails != null) && (targContract.dtTbt_RentalSentryGuardDetails.Count > 0))
                    {
                        newRentalSentryGuardDetails = new tbt_RentalSentryGuardDetails()
                        {
                            ContractCode = targContract.dtTbt_RentalSentryGuardDetails[0].ContractCode,
                            NumOfDate = targContract.dtTbt_RentalSentryGuardDetails[0].NumOfDate,
                            NumOfSentryGuard = targContract.dtTbt_RentalSentryGuardDetails[0].NumOfSentryGuard,
                            SecurityFinishTime = targContract.dtTbt_RentalSentryGuardDetails[0].SecurityFinishTime,
                            SentryGuardTypeCode = targContract.dtTbt_RentalSentryGuardDetails[0].SentryGuardTypeCode,
                            SecurityStartTime = targContract.dtTbt_RentalSentryGuardDetails[0].SecurityStartTime,
                            SequenceNo = targContract.dtTbt_RentalSentryGuardDetails[0].SequenceNo,
                            TimeUnitPrice = targContract.dtTbt_RentalSentryGuardDetails[0].TimeUnitPrice,
                            WorkHourPerMonth = targContract.dtTbt_RentalSentryGuardDetails[0].WorkHourPerMonth
                        };

                        targContract.dtTbt_RentalSentryGuardDetails.Clear();

                        newRentalSentryGuardDetails.OCC = newOCC;
                        //newRentalSentryGuardDetails.NumOfSentryGuard = XXXX;
                        newRentalSentryGuardDetails.CreateBy = dsTrans.dtUserData.EmpNo;
                        newRentalSentryGuardDetails.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        //newRentalSentryGuardDetails.UpdateBy = dsTrans.dtUserData.EmpNo;
                        //newRentalSentryGuardDetails.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        targContract.dtTbt_RentalSentryGuardDetails.Add(newRentalSentryGuardDetails);
                    }
                    else
                    {
                        //targContract.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                        //newRentalSentryGuardDetails.ContractCode = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }

                    // Update dtTbt_RelationType
                    tbt_RelationType newRelationType = new tbt_RelationType();

                    if ((targContract.dtTbt_RelationType != null) && (targContract.dtTbt_RelationType.Count > 0))
                    {
                        newRelationType = new tbt_RelationType()
                        {
                            ContractCode = targContract.dtTbt_RelationType[0].ContractCode,
                            ProductName = targContract.dtTbt_RelationType[0].ProductName,
                            ProductTypeCode = targContract.dtTbt_RelationType[0].ProductTypeCode,
                            RelatedContractCode = targContract.dtTbt_RelationType[0].RelatedContractCode,
                            RelatedOCC = targContract.dtTbt_RelationType[0].RelatedOCC,
                            RelationType = targContract.dtTbt_RelationType[0].RelationType,
                        };
                        targContract.dtTbt_RelationType.Clear();

                        newRelationType.OCC = newOCC;
                        newRelationType.CreateBy = dsTrans.dtUserData.EmpNo;
                        newRelationType.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        //newRelationType.UpdateBy = dsTrans.dtUserData.EmpNo;
                        //newRelationType.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        targContract.dtTbt_RelationType.Add(newRelationType);
                    }
                    else
                    {
                        //targContract.dtTbt_RelationType = new List<tbt_RelationType>();
                        //newRelationType.ContractCode = comUtil.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                    }

                    targContract.dtTbt_CancelContractMemoDetail.Clear();

                    // Insert new Rentral Contract
                    //rentralHandler.InsertEntireContract(targContract);
                    rentralHandler.InsertEntireContractForCTS010(targContract); //Modify by Jutarat A. on 19092013

                    //scope.Complete(); // Comment for test
                    result = true;
                    //}

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return result;
        }

        /// <summary>
        /// Validating user permission and check system suspending
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private ObjectResultData ValidateAuthority_CTS120(ObjectResultData res)
        {
            if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP99, FunctionID.C_FUNC_ID_OPERATE) == false)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                return res;
            }

            // Check System Status
            if (CheckIsSuspending(res))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return res;
            }

            return res;
        }

        //private void SetScreenParameter_CTS120(CTS120_ScreenParameter obj)
        //{
        //    Session.Remove("CTS120_PARAM");
        //    Session.Add("CTS120_PARAM", obj);
        //}

        //private CTS120_ScreenParameter GetScreenObject_CTS120()
        //{
        //    CTS120_ScreenParameter obj = null;

        //    if (Session["CTS120_PARAM"] != null)
        //    {
        //        obj = (CTS120_ScreenParameter)Session["CTS120_PARAM"];
        //    }

        //    return obj;
        //}

        #endregion
    }
}
