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
        #region Event

        //2. Event: Select [Process type] radio button in ‘Specify process type’ section (อยู่ใน javascript ไฟล์ CTS220_01.js)
        //4. Event: Select [Change type] drop-down list in ‘Product information’ section (อยู่ใน javascript ไฟล์ CTS220_03.js)
        //5. Event: Lost focus from [Salesman code] or [Negotiation staff] text box (อยู่ใน javascript ไฟล์ CTS220_03.js) 
        //6. Event: Click [Quotation code] link in ‘Quotation information’ section (เนื่องจาก CMS180 ยังไม่ Code นะครับก้อเลยยังไม่ได้ทำส่วนนี้ถ้าทำก้อ ทำเหมือนกับ 7. Event เอาไปอย่ใน CTS220_09.js)
        //7. Event: Click [Installation slip no.] link in ‘Installation information’ section (อยู่ใน javascript ไฟล์ CTS220_09.js) 
        //11. Event: Click [Reset] button in ‘Action button’ section (อยู่ใน javascript ไฟล์ CTS220_01.js) 
        //13. Event: Click [Back] button in ‘Action button’ section (อยู่ใน javascript ไฟล์ CTS220_01.js) 

        //3. Event: Click [Select process] button in ‘Specify process type’ section 
        /// <summary>
        /// Validate and retrive data when cick [Select process] button in ‘Specify process type’ section
        /// </summary>
        /// <param name="doSelectProcess"></param>
        /// <returns></returns>
        public ActionResult SelectProcessClick_CTS220(CTS220_DOSelectProcess doSelectProcess)
        {
            ObjectResultData res = new ObjectResultData();
            IRentralContractHandler rentralContract;
            //IInstallationInterfaceHandler installationInterface;

            dsRentalContractData dsRentalContract;

            string prevOCC = "";
            string nextOCC = "";
            string lastImplementOCC = "";
            string lastUnimplementedOCC = "";
            string installationStatusCode = "";

            CTS220_ScreenParameter session;
            CommonUtil comU;

            try
            {
                session = CTS220_GetImportData();
                comU = new CommonUtil();
                rentralContract = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                //installationInterface = ServiceContainer.GetService<IInstallationInterfaceHandler>() as IInstallationInterfaceHandler;

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

                doSelectProcess.ContractCode = comU.ConvertContractCode(doSelectProcess.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                //dsRentalContract = rentralContract.GetEntireContract(doSelectProcess.ContractCode, null);
                //session.DSRentalContract = dsRentalContract;

                //3. Event: Click [Select process] button in ‘Specify process type’ section 
                //3.1 If rdoProcessType = ‘Correct’ Then
                if (doSelectProcess.ProcessType == "Correct")
                {
                    dsRentalContract = rentralContract.GetEntireContract(doSelectProcess.ContractCode, doSelectProcess.OCC);
                    if (dsRentalContract == null)
                    {
                        string[] param = { doSelectProcess.OCC };
                        res.ResultData = false;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0106, param, null);
                        return Json(res);
                    }

                    if (dsRentalContract.dtTbt_RentalSecurityBasic[0].ImplementFlag == FlagType.C_FLAG_OFF)
                    {
                        res.ResultData = false;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3132);
                        return Json(res);
                    }

                    prevOCC = rentralContract.GetPreviousImplementedOCC(doSelectProcess.ContractCode, doSelectProcess.OCC);
                    nextOCC = rentralContract.GetNextImplementedOCC(doSelectProcess.ContractCode, doSelectProcess.OCC);

                    //Do not validate for showing MSG3133 
                    //if ((prevOCC == null || prevOCC == "") || (nextOCC == null || nextOCC == ""))
                    //{
                    //    res.ResultData = false;
                    //    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3133);
                    //    return Json(res);
                    //}

                    session.DSRentalContract = dsRentalContract;

                    if (prevOCC != null)
                        session.DSRentalContractPrevious = rentralContract.GetEntireContract(doSelectProcess.ContractCode, prevOCC);
                    if (nextOCC != null)
                        session.DSRentalContractNext = rentralContract.GetEntireContract(doSelectProcess.ContractCode, nextOCC);
                    session.DSRentalContractShow = session.DSRentalContract;
                }

                //3.2 If rdoProcessType = ‘Insert’ Then
                if (doSelectProcess.ProcessType == "Insert")
                {
                    dsRentalContract = rentralContract.GetEntireContract(doSelectProcess.ContractCode, doSelectProcess.OCC);

                    //if (dsRentalContract.dtTbt_RentalSecurityBasic != null)
                    //{
                    //    if (dsRentalContract.dtTbt_RentalSecurityBasic.Count() == 0)
                    //    {
                    //        res.ResultData = false;
                    //        string[] param = { doSelectProcess.ContractCode };
                    //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0106, param, null);
                    //        return Json(res);
                    //    }
                    //}
                    if (dsRentalContract != null && dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
                    {
                        string[] param = { doSelectProcess.OCC };
                        res.ResultData = false;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3238, param, null);
                        return Json(res);
                    }

                    prevOCC = rentralContract.GetPreviousImplementedOCC(doSelectProcess.ContractCode, doSelectProcess.OCC);
                    nextOCC = rentralContract.GetNextImplementedOCC(doSelectProcess.ContractCode, doSelectProcess.OCC);

                    if ((prevOCC == null || prevOCC == "") || (nextOCC == null || nextOCC == ""))
                    {
                        res.ResultData = false;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3133);
                        return Json(res);
                    }

                    session.DSRentalContract = dsRentalContract;
                    session.DSRentalContractPrevious = rentralContract.GetEntireContract(doSelectProcess.ContractCode, prevOCC);
                    session.DSRentalContractNext = rentralContract.GetEntireContract(doSelectProcess.ContractCode, nextOCC);
                    session.DSRentalContractShow = session.DSRentalContractPrevious;
                }

                //3.3 If rdoProcessType = ‘Delete’ Then
                if (doSelectProcess.ProcessType == "Delete")
                {
                    //3.3.1.1 Get last implemented OCC
                    lastImplementOCC = rentralContract.GetLastImplementedOCC(doSelectProcess.ContractCode);

                    //3.3.1.2 Get entire contract for the last OCC
                    dsRentalContract = rentralContract.GetEntireContract(doSelectProcess.ContractCode, lastImplementOCC);

                    //3.3.1.3 If dtEntireContract is empty Then
                    //if (dsRentalContract.dtTbt_RentalSecurityBasic != null)
                    //{
                    //    if (dsRentalContract.dtTbt_RentalSecurityBasic.Count() == 0)
                    //    {
                    //        res.ResultData = false;
                    //        string[] param = { doSelectProcess.ContractCode };
                    //        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0106, param, null);
                    //        return Json(res);
                    //    }
                    //}
                    if (dsRentalContract == null)
                    {
                        string[] param = { doSelectProcess.OCC };
                        res.ResultData = false;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0106, param, null);
                        return Json(res);
                    }

                    //3.3.1.4 If dtEntireContract.dtTbt_RentalSecurityBasic.ChangeType is the one of following items
                    if (dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_ALTERNATIVE_START ||
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_NEW_START)
                    {
                        res.ResultData = false;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3134);
                        return Json(res);
                    }

                    //3.3.1.5 Get last unimplemented OCC
                    lastUnimplementedOCC = rentralContract.GetLastUnimplementedOCC(doSelectProcess.ContractCode);
                    if (lastUnimplementedOCC != null && lastUnimplementedOCC != "")
                    {
                        res.ResultData = false;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3289);
                        return Json(res);
                    }

                    IInstallationHandler iHandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                    installationStatusCode = iHandler.GetInstallationStatus(doSelectProcess.ContractCode);
                    if (installationStatusCode != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                    {
                        res.ResultData = false;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3293);
                        return Json(res);
                    }           

                    res.ResultData = lastImplementOCC;

                    session.DSRentalContract = dsRentalContract;
                    session.DSRentalContractShow = session.DSRentalContract;
                }

                session.ProcessType = doSelectProcess.ProcessType;

                if (session.DOValidateBusiness == null)
                    session.DOValidateBusiness = new CTS220_ValidateBusiness();

                session.DOValidateBusiness.ProcessType = doSelectProcess.ProcessType;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //8. Event: Click [Remove] button in ‘Installation subcontractor’ subsection
        /// <summary>
        /// Remove data from Installation subcontractor grid when click [Remove] button in ‘Installation subcontractor’ subsection
        /// </summary>
        /// <param name="doSubContract"></param>
        /// <returns></returns>
        public ActionResult RemoveClick_CTS220(CTS220_DOSubContract doSubContract)
        {
            ObjectResultData res = new ObjectResultData();
            CTS220_ScreenParameter session;
            List<dtTbt_RentalInstSubContractorListForView> ListSubContractor;
            List<tbt_RentalInstSubcontractor> ListRentalInstSubcontractor;

            try
            {
                //8.1 Remove the current row from ‘Installation subcontractor’ subsection
                session = CTS220_GetImportData();
                ListSubContractor = session.ListSubContractor.FindAll(delegate(dtTbt_RentalInstSubContractorListForView s) { return s.SubcontractorCode == doSubContract.SubContractCode; });

                //ทำการเเยกการ Remove ออกเพราะว่า Case Correct กะ Insert จะทำการใช้ DSRentalContract คนละตัวกันในการ Save ครับ
                if (session.ProcessType == "Insert")
                {
                    ListRentalInstSubcontractor = session.DSRentalContractPrevious.dtTbt_RentalInstSubcontractor.FindAll(delegate(tbt_RentalInstSubcontractor s) { return s.SubcontractorCode == doSubContract.SubContractCode; });
                    foreach (var item in ListRentalInstSubcontractor)
                    {
                        session.DSRentalContractPrevious.dtTbt_RentalInstSubcontractor.Remove(item);
                    }
                }
                else
                {
                    ListRentalInstSubcontractor = session.DSRentalContract.dtTbt_RentalInstSubcontractor.FindAll(delegate(tbt_RentalInstSubcontractor s) { return s.SubcontractorCode == doSubContract.SubContractCode; });
                    foreach (var item in ListRentalInstSubcontractor)
                    {
                        session.DSRentalContract.dtTbt_RentalInstSubcontractor.Remove(item);
                    }
                }


                foreach (var item in ListSubContractor)
                {
                    session.ListSubContractor.Remove(item);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //9. Event: Click [Add] button in ‘Installation subcontractor’ subsection
        /// <summary>
        /// Add data to Installation subcontractor grid when click [Add] button in ‘Installation subcontractor’ subsection
        /// </summary>
        /// <param name="doSubContract"></param>
        /// <returns></returns>
        public ActionResult AddClick_CTS220(CTS220_DOSubContract doSubContract)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonContractHandler commonContractHandler;

            List<tbm_SubContractor> listSubContractor;
            dtTbt_RentalInstSubContractorListForView dttbtRentalInstSubContractor;
            tbt_RentalInstSubcontractor tbtRentalInstSubcontractor;
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
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

                if (session.ListSubContractor.FindAll(delegate(dtTbt_RentalInstSubContractorListForView s) { return s.SubcontractorCode == doSubContract.SubContractCode; }).Count() != 0)
                {
                    res.ResultData = false;
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3136, null, null);
                    return Json(res);
                }

                commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                listSubContractor = commonContractHandler.GetTbm_SubContractorData(doSubContract.SubContractCode);

                dttbtRentalInstSubContractor = new dtTbt_RentalInstSubContractorListForView();
                dttbtRentalInstSubContractor.SubcontractorCode = listSubContractor[0].SubContractorCode;
                dttbtRentalInstSubContractor.SubContractorNameEN_SubCont = listSubContractor[0].SubContractorNameEN;
                dttbtRentalInstSubContractor.SubContractorNameLC_SubCont = listSubContractor[0].SubContractorNameLC;
                dttbtRentalInstSubContractor.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                dttbtRentalInstSubContractor.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dttbtRentalInstSubContractor.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                dttbtRentalInstSubContractor.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                session.ListSubContractor.Add(dttbtRentalInstSubContractor);

                tbtRentalInstSubcontractor = new tbt_RentalInstSubcontractor();
                tbtRentalInstSubcontractor.ContractCode = session.DSRentalContractShow.dtTbt_RentalContractBasic[0].ContractCode;
                tbtRentalInstSubcontractor.OCC = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OCC;
                tbtRentalInstSubcontractor.SubcontractorCode = doSubContract.SubContractCode;
                tbtRentalInstSubcontractor.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                tbtRentalInstSubcontractor.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                tbtRentalInstSubcontractor.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                tbtRentalInstSubcontractor.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                if (doSubContract.ProcessType == "Insert")
                    session.DSRentalContractPrevious.dtTbt_RentalInstSubcontractor.Add(tbtRentalInstSubcontractor);
                else
                    session.DSRentalContract.dtTbt_RentalInstSubcontractor.Add(tbtRentalInstSubcontractor);
            }
            catch (Exception ex)
            {
                res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //10. Event: Click [Register] button in ‘Action button’ section
        /// <summary>
        /// Validate business of screen when click [Register] button in ‘Action button’ section
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ActionResult RegisterClick_CTS220(CTS220_ValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resValidate = new ObjectResultData();
            ICommonHandler commonHandler;
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                //10.1 Check suspending
                //10.1.1 Get suspending status
                //10.1.2 Not allow to continue operation if system is suspended
                if (commonHandler.IsSystemSuspending())
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, "");
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                //Check user’s permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_ADD) == false
                    && CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_EDIT) == false
                    && CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_DEL) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                // Validate require fields
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (doValidateBusiness.ProcessType == "Correct" || doValidateBusiness.ProcessType == "Insert")
                {
                    List<string> errorCtrl = new List<string>();
                    List<string> errorLabel = new List<string>();

                    if (!doValidateBusiness.ContractFee.HasValue)
                    {
                        errorCtrl.Add("OrderContractFee");
                        errorLabel.Add("lblContractFee");
                    }

                    //Validate required fields from insurance type
                    if (doValidateBusiness.InsuranceTypeCode != InsuranceType.C_INSURANCE_TYPE_NONE)
                    {
                        if(!doValidateBusiness.InsuranceCoverageAmount.HasValue){
                            errorCtrl.Add("InsuranceCoverageFee");
                            errorLabel.Add("lblInsuranceCoverageFee");
                        }

                        if(!doValidateBusiness.MonthlyInsuranceFee.HasValue){
                            errorCtrl.Add("MonthlyInsuranceFee");
                            errorLabel.Add("lblMonthlyInsuranceFee");
                        }
                    }

                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.ChangeImplementDate))
                    {
                        errorCtrl.Add("ChangeImplementDate");
                        errorLabel.Add("lblChangeOperationDate");
                    }

                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.ChangeType))
                    {
                        errorCtrl.Add("ChangeType");
                        errorLabel.Add("lblChangeType");
                    }
                    
                    //Validate required change reason type
                    if (doValidateBusiness.IsEnableReasonType && String.IsNullOrEmpty(doValidateBusiness.ChangeReasonType))
                    {
                        errorCtrl.Add("ChangeReasonType");
                        errorLabel.Add("lblReason");
                    }

                    if (CommonUtil.IsNullOrEmpty(doValidateBusiness.SalesmanEmpNo1))
                    {
                        errorCtrl.Add("SaleManEmpNo1");
                        errorLabel.Add("lblSalesMan1");
                    }
                    
                    ////Validate contract document name
                    //if (String.IsNullOrEmpty(doValidateBusiness.DocumentCode))
                    //{
                    //    errorCtrl.Add("DocumentName");
                    //    errorLabel.Add("lblDocumentName");
                    //}

                    if ((errorCtrl.Count > 0) || (errorLabel.Count > 0))
                    {
                        string errorLabelText = "";

                        foreach (string item in errorLabel.Distinct())
                        {
                            if (errorLabelText.Length > 0)
                            {
                                errorLabelText += ", ";
                            }

                            errorLabelText += CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS220", item);
                        }

                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { errorLabelText }, errorCtrl.ToArray());
                        return Json(res);
                    }

                    /*
                    CTS220_ValidateRequireField dataTemp = CommonUtil.CloneObject<CTS220_ValidateBusiness, CTS220_ValidateRequireField>(doValidateBusiness);

                    CTS220_ValidateChangeReasonType reasonTemp = null;
                    if (doValidateBusiness.IsEnableReasonType)
                    {
                        reasonTemp = new CTS220_ValidateChangeReasonType();
                        reasonTemp.ChangeReasonType = doValidateBusiness.ChangeReasonType;
                    }

                    ValidatorUtil.BuildErrorMessage(res, new object[] { dataTemp, reasonTemp }, null, false);
                    if (res.IsError)
                    {
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007);
                        return Json(res);
                    }
                     * */
                }

                //validateForCorrect() เเละ validateForInsert() เเยกกันที่ method validateRegisterConfirm_CTS220()
                resValidate = ValidateRegisterConfirm_CTS220(doValidateBusiness);
                if (resValidate.IsError)
                    return Json(resValidate);

                
                if (doValidateBusiness.ProcessType == "Correct")
                {
                    if (doValidateBusiness.ContractFee != session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee
                        || doValidateBusiness.ContractFeeOnStop != session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop) //Add by Jutarat A. on 14082012
                    {
                        //if (session.DSRentalContract.dtTbt_RentalContractBasic[0].LastOCC == session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OCC)
                        //{
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG;
                            res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3292);
                        //}
                    }
                }
                else if (doValidateBusiness.ProcessType == "Insert")
                {
                    //ValidateBusinessForWarning
                    if (doValidateBusiness.ContractFee != session.DSRentalContractPrevious.dtTbt_RentalSecurityBasic[0].OrderContractFee
                        || doValidateBusiness.ContractFeeOnStop != session.DSRentalContractPrevious.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop) //Add by Jutarat A. on 14082012
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG;
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3292);
                    }
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                //res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //12. Event: Click [Confirm] button in ‘Action button’ section
        /// <summary>
        /// Register maintenanced data to database when click [Confirm] button in ‘Action button’ section
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ActionResult ConfirmClick_CTS220(CTS220_ValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resValidate = new ObjectResultData();
            bool registerResult;

            ICommonHandler commonHandler;
            IRentralContractHandler rentralContractHandler;
            dsRentalContractData dsRentalContractPrevious = new dsRentalContractData();
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                if (commonHandler.IsSystemSuspending())
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, "");
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, null, null);
                    return Json(res);
                }

                //Check user’s permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_ADD) == false
                    && CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_EDIT) == false
                    && CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_DEL) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }


                resValidate = ValidateRegisterConfirm_CTS220(doValidateBusiness);
                if (resValidate.IsError)
                    return Json(resValidate);


                //เก็บค่าจากหน้า Screen ที่มีการเปลี่ยนเเปลงโดยใน Case ของ Correct จะใช้ DSRentalContract
                if (doValidateBusiness.ProcessType == "Correct")
                {
                    //tbt_RentalSecurityBasic
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee = doValidateBusiness.ContractFee;
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop = doValidateBusiness.ContractFeeOnStop; //Add by Jutarat A. on 14082012

                    #region Order Contract Fee

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType = doValidateBusiness.ContractFeeCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd = doValidateBusiness.ContractFee;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee = doValidateBusiness.ContractFee;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd = null;
                    }

                    #endregion
                    #region Contract Fee on Stop

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopCurrencyType = doValidateBusiness.ContractFeeOnStopCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopUsd = doValidateBusiness.ContractFeeOnStop;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop = doValidateBusiness.ContractFeeOnStop;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopUsd = null;
                    }

                    #endregion

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate = doValidateBusiness.ChangeImplementDate;
                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType = doValidateBusiness.ChangeType;

                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeReasonType = doValidateBusiness.ChangeReasonType;
                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeNameReasonType = null;
                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeReasonType = null;
                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].StopCancelReasonType = null;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME
                        || session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeNameReasonType = doValidateBusiness.ChangeReasonType;
                    }
                    else if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeReasonType = doValidateBusiness.ChangeReasonType;
                    }
                    else if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT
                        || session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL
                        || session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_STOP
                        || session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP
                        || session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START
                        || session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP
                        || session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_ALL)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].StopCancelReasonType = doValidateBusiness.ChangeReasonType;
                    }

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1 = doValidateBusiness.SalesmanEmpNo1;
                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2 = doValidateBusiness.SalesmanEmpNo2;
                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo1 = doValidateBusiness.ApproveNo1;
                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ApproveNo2 = doValidateBusiness.ApproveNo2;
                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].SecurityMemo = doValidateBusiness.SecurityMemo;

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1 = doValidateBusiness.NegotiationStaffEmpNo1;
                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo2 = doValidateBusiness.NegotiationStaffEmpNo2;

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InsuranceTypeCode = doValidateBusiness.InsuranceTypeCode;
                    
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmount = doValidateBusiness.InsuranceCoverageAmount;
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFee = doValidateBusiness.MonthlyInsuranceFee;

                    #region Insurance Coverage Amount

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmountCurrencyType = doValidateBusiness.InsuranceCoverageAmountCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmount = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmountUsd = doValidateBusiness.InsuranceCoverageAmount;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmount = doValidateBusiness.InsuranceCoverageAmount;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmountUsd = null;
                    }

                    #endregion
                    #region Monthly Insurance Fee

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFeeCurrencyType = doValidateBusiness.MonthlyInsuranceFeeCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFee = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFeeUsd = doValidateBusiness.MonthlyInsuranceFee;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFee = doValidateBusiness.MonthlyInsuranceFee;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFeeUsd = null;
                    }

                    #endregion

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].DocumentCode = doValidateBusiness.DocumentCode;

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ExpectedResumeDate = doValidateBusiness.ExpectedResumeDate;
                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate = doValidateBusiness.ReturnToOriginalFeeDate;

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].PlanCode = doValidateBusiness.PlanCode;
                    
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceFee1 = doValidateBusiness.MaintenanceFee1;
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee1 = doValidateBusiness.AdditionalFee1;
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee2 = doValidateBusiness.AdditionalFee2;
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee3 = doValidateBusiness.AdditionalFee3;


                    #region Maintenance Fee 1

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceFee1CurrencyType = doValidateBusiness.MaintenanceFee1CurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceFee1 = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceFee1Usd = doValidateBusiness.MaintenanceFee1;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceFee1 = doValidateBusiness.MaintenanceFee1;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].MaintenanceFee1Usd = null;
                    }

                    #endregion
                    #region Additional Fee 1

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee1CurrencyType = doValidateBusiness.AdditionalFee1CurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee1 = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee1Usd = doValidateBusiness.AdditionalFee1;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee1 = doValidateBusiness.AdditionalFee1;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee1Usd = null;
                    }

                    #endregion
                    #region Additional Fee 2

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee2CurrencyType = doValidateBusiness.AdditionalFee2CurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee2 = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee2Usd = doValidateBusiness.AdditionalFee2;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee2 = doValidateBusiness.AdditionalFee2;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee2Usd = null;
                    }

                    #endregion
                    #region Additional Fee 3

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee3CurrencyType = doValidateBusiness.AdditionalFee3CurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee3 = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee3Usd = doValidateBusiness.AdditionalFee3;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee3 = doValidateBusiness.AdditionalFee3;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].AdditionalFee3Usd = null;
                    }

                    #endregion

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallationTypeCode = doValidateBusiness.InstallationTypeCode;
                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteDate = doValidateBusiness.InstallationCompleteDate;

                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFee = doValidateBusiness.NormalInstallFee;
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee = doValidateBusiness.OrderInstallFee;
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract = doValidateBusiness.OrderInstallFee_ApproveContract;
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = doValidateBusiness.OrderInstallFee_CompleteInstall;
                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService = doValidateBusiness.OrderInstallFee_StartService;

                    #region Normal Install Fee

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFeeCurrencyType = doValidateBusiness.NormalInstallFeeCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFee = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFeeUsd = doValidateBusiness.NormalInstallFee;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFee = doValidateBusiness.NormalInstallFee;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFeeUsd = null;
                    }

                    #endregion
                    #region Order Install Fee

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeCurrencyType = doValidateBusiness.OrderInstallFeeCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeUsd = doValidateBusiness.OrderInstallFee;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee = doValidateBusiness.OrderInstallFee;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFeeUsd = null;
                    }

                    #endregion
                    #region Order Install Fee Approve Contract

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractCurrencyType = doValidateBusiness.OrderInstallFee_ApproveContractCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractUsd = doValidateBusiness.OrderInstallFee_ApproveContract;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract = doValidateBusiness.OrderInstallFee_ApproveContract;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractUsd = null;
                    }

                    #endregion
                    #region Order Install Fee Complete Install

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallCurrencyType = doValidateBusiness.OrderInstallFee_CompleteInstallCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallUsd = doValidateBusiness.OrderInstallFee_CompleteInstall;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = doValidateBusiness.OrderInstallFee_CompleteInstall;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallUsd = null;
                    }

                    #endregion
                    #region Order Install Fee Start Service

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceCurrencyType = doValidateBusiness.OrderInstallFee_StartServiceCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceUsd = doValidateBusiness.OrderInstallFee_StartService;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService = doValidateBusiness.OrderInstallFee_StartService;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceUsd = null;
                    }

                    #endregion

                    #region Install Fee Paid by SECOM

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOMCurrencyType = doValidateBusiness.InstallFeePaidBySECOMCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOM = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOMUsd = doValidateBusiness.InstallFeePaidBySECOM;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOM = doValidateBusiness.InstallFeePaidBySECOM;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOMUsd = null;
                    }

                    #endregion
                    #region Install Fee Revenue by SECOM

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOMCurrencyType = doValidateBusiness.InstallFeeRevenueBySECOMCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOM = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOMUsd = doValidateBusiness.InstallFeeRevenueBySECOM;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOM = doValidateBusiness.InstallFeeRevenueBySECOM;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOMUsd = null;
                    }

                    #endregion

                    //session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalContractFee = doValidateBusiness.NormalContractFee; //Add by Jutarat A. on 06022014

                    #region Normal Contract Fee

                    session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalContractFeeCurrencyType = doValidateBusiness.NormalContractFeeCurrencyType;
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalContractFee = null;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalContractFeeUsd = doValidateBusiness.NormalContractFee;
                    }
                    else
                    {
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalContractFee = doValidateBusiness.NormalContractFee;
                        session.DSRentalContract.dtTbt_RentalSecurityBasic[0].NormalContractFeeUsd = null;
                    }

                    #endregion


                    //tbt_RentalOperationType
                    List<tbt_RentalOperationType> doTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                    if (doValidateBusiness.OperationType != null)
                    {
                        foreach (string opt in doValidateBusiness.OperationType)
                        {
                            doTbt_RentalOperationType.Add(new tbt_RentalOperationType()
                            {
                                ContractCode = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode,
                                OCC = session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                OperationTypeCode = opt
                            });
                        }
                        session.DSRentalContract.dtTbt_RentalOperationType = doTbt_RentalOperationType;
                    }

                }

                //เก็บค่าจากหน้า Screen ที่มีการเปลี่ยนเเปลงโดยใน Case ของ Insert จะใช้ DSRentalContractPrevious
                if (doValidateBusiness.ProcessType == "Insert")
                {
                    string strOCC = doValidateBusiness.OCC;

                    //dtTbt_RentalContractBasic
                    dsRentalContractPrevious.dtTbt_RentalContractBasic = new List<tbt_RentalContractBasic>();

                    tbt_RentalContractBasic rentalContractBasic = CommonUtil.CloneObject<tbt_RentalContractBasic, tbt_RentalContractBasic>(session.DSRentalContractPrevious.dtTbt_RentalContractBasic[0]);
                    dsRentalContractPrevious.dtTbt_RentalContractBasic.Add(rentalContractBasic);


                    //tbt_RentalSecurityBasic
                    dsRentalContractPrevious.dtTbt_RentalSecurityBasic = new List<tbt_RentalSecurityBasic>();

                    tbt_RentalSecurityBasic rentalSecurityBasicData = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(session.DSRentalContractPrevious.dtTbt_RentalSecurityBasic[0]);

                    rentalSecurityBasicData.QuotationTargetCode = null;
                    rentalSecurityBasicData.InstallationSlipNo = null;
                    rentalSecurityBasicData.PlanApproveDate = null;
                    rentalSecurityBasicData.PlanApproverEmpNo = null;

                    rentalSecurityBasicData.OCC = strOCC;

                    //rentalSecurityBasicData.OrderContractFee = doValidateBusiness.ContractFee;
                    //rentalSecurityBasicData.ContractFeeOnStop = doValidateBusiness.ContractFeeOnStop; //Add by Jutarat A. on 14082012

                    #region Order Contract Fee

                    rentalSecurityBasicData.OrderContractFeeCurrencyType = doValidateBusiness.ContractFeeCurrencyType;
                    if (rentalSecurityBasicData.OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.OrderContractFee = null;
                        rentalSecurityBasicData.OrderContractFeeUsd = doValidateBusiness.ContractFee;
                    }
                    else
                    {
                        rentalSecurityBasicData.OrderContractFee = doValidateBusiness.ContractFee;
                        rentalSecurityBasicData.OrderContractFeeUsd = null;
                    }

                    #endregion
                    #region Contract Fee on Stop

                    rentalSecurityBasicData.ContractFeeOnStopCurrencyType = doValidateBusiness.ContractFeeOnStopCurrencyType;
                    if (rentalSecurityBasicData.ContractFeeOnStopCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.ContractFeeOnStop = null;
                        rentalSecurityBasicData.ContractFeeOnStopUsd = doValidateBusiness.ContractFeeOnStop;
                    }
                    else
                    {
                        rentalSecurityBasicData.ContractFeeOnStop = doValidateBusiness.ContractFeeOnStop;
                        rentalSecurityBasicData.ContractFeeOnStopUsd = null;
                    }

                    #endregion

                    rentalSecurityBasicData.ChangeImplementDate = doValidateBusiness.ChangeImplementDate;
                    rentalSecurityBasicData.ChangeType = doValidateBusiness.ChangeType;

                    //rentalSecurityBasicData.ChangeReasonType = doValidateBusiness.ChangeReasonType;
                    rentalSecurityBasicData.ChangeNameReasonType = null;
                    rentalSecurityBasicData.ChangeReasonType = null;
                    rentalSecurityBasicData.StopCancelReasonType = null;
                    if (rentalSecurityBasicData.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME
                       || rentalSecurityBasicData.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP)
                    {
                        rentalSecurityBasicData.ChangeNameReasonType = doValidateBusiness.ChangeReasonType;
                    }
                    else if (rentalSecurityBasicData.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE)
                    {
                        rentalSecurityBasicData.ChangeReasonType = doValidateBusiness.ChangeReasonType;
                    }
                    else if (rentalSecurityBasicData.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT
                        || rentalSecurityBasicData.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL
                        || rentalSecurityBasicData.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_STOP
                        || rentalSecurityBasicData.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP
                        || rentalSecurityBasicData.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START
                        || rentalSecurityBasicData.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP
                        || rentalSecurityBasicData.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_ALL)
                    {
                        rentalSecurityBasicData.StopCancelReasonType = doValidateBusiness.ChangeReasonType;
                    }

                    rentalSecurityBasicData.SalesmanEmpNo1 = doValidateBusiness.SalesmanEmpNo1;
                    rentalSecurityBasicData.SalesmanEmpNo2 = doValidateBusiness.SalesmanEmpNo2;
                    rentalSecurityBasicData.ApproveNo1 = doValidateBusiness.ApproveNo1;
                    rentalSecurityBasicData.ApproveNo2 = doValidateBusiness.ApproveNo2;
                    rentalSecurityBasicData.SecurityMemo = doValidateBusiness.SecurityMemo;

                    rentalSecurityBasicData.NegotiationStaffEmpNo1 = doValidateBusiness.NegotiationStaffEmpNo1;
                    rentalSecurityBasicData.NegotiationStaffEmpNo2 = doValidateBusiness.NegotiationStaffEmpNo2;

                    rentalSecurityBasicData.InsuranceTypeCode = doValidateBusiness.InsuranceTypeCode;
                    
                    //rentalSecurityBasicData.InsuranceCoverageAmount = doValidateBusiness.InsuranceCoverageAmount;
                    //rentalSecurityBasicData.MonthlyInsuranceFee = doValidateBusiness.MonthlyInsuranceFee;

                    #region Insurance Coverage Amount

                    rentalSecurityBasicData.InsuranceCoverageAmountCurrencyType = doValidateBusiness.InsuranceCoverageAmountCurrencyType;
                    if (rentalSecurityBasicData.InsuranceCoverageAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.InsuranceCoverageAmount = null;
                        rentalSecurityBasicData.InsuranceCoverageAmountUsd = doValidateBusiness.InsuranceCoverageAmount;
                    }
                    else
                    {
                        rentalSecurityBasicData.InsuranceCoverageAmount = doValidateBusiness.InsuranceCoverageAmount;
                        rentalSecurityBasicData.InsuranceCoverageAmountUsd = null;
                    }

                    #endregion
                    #region Monthly Insurance Fee

                    rentalSecurityBasicData.MonthlyInsuranceFeeCurrencyType = doValidateBusiness.MonthlyInsuranceFeeCurrencyType;
                    if (rentalSecurityBasicData.MonthlyInsuranceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.MonthlyInsuranceFee = null;
                        rentalSecurityBasicData.MonthlyInsuranceFeeUsd = doValidateBusiness.MonthlyInsuranceFee;
                    }
                    else
                    {
                        rentalSecurityBasicData.MonthlyInsuranceFee = doValidateBusiness.MonthlyInsuranceFee;
                        rentalSecurityBasicData.MonthlyInsuranceFeeUsd = null;
                    }

                    #endregion

                    rentalSecurityBasicData.DocumentCode = doValidateBusiness.DocumentCode;

                    rentalSecurityBasicData.ExpectedResumeDate = doValidateBusiness.ExpectedResumeDate;
                    rentalSecurityBasicData.ReturnToOriginalFeeDate = doValidateBusiness.ReturnToOriginalFeeDate;

                    rentalSecurityBasicData.PlanCode = doValidateBusiness.PlanCode;

                    //rentalSecurityBasicData.MaintenanceFee1 = doValidateBusiness.MaintenanceFee1;
                    //rentalSecurityBasicData.AdditionalFee1 = doValidateBusiness.AdditionalFee1;
                    //rentalSecurityBasicData.AdditionalFee2 = doValidateBusiness.AdditionalFee2;
                    //rentalSecurityBasicData.AdditionalFee3 = doValidateBusiness.AdditionalFee3;

                    #region Maintenance Fee 1

                    rentalSecurityBasicData.MaintenanceFee1CurrencyType = doValidateBusiness.MaintenanceFee1CurrencyType;
                    if (rentalSecurityBasicData.MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.MaintenanceFee1 = null;
                        rentalSecurityBasicData.MaintenanceFee1Usd = doValidateBusiness.MaintenanceFee1;
                    }
                    else
                    {
                        rentalSecurityBasicData.MaintenanceFee1 = doValidateBusiness.MaintenanceFee1;
                        rentalSecurityBasicData.MaintenanceFee1Usd = null;
                    }

                    #endregion
                    #region Additional Fee 1

                    rentalSecurityBasicData.AdditionalFee1CurrencyType = doValidateBusiness.AdditionalFee1CurrencyType;
                    if (rentalSecurityBasicData.AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.AdditionalFee1 = null;
                        rentalSecurityBasicData.AdditionalFee1Usd = doValidateBusiness.AdditionalFee1;
                    }
                    else
                    {
                        rentalSecurityBasicData.AdditionalFee1 = doValidateBusiness.AdditionalFee1;
                        rentalSecurityBasicData.AdditionalFee1Usd = null;
                    }

                    #endregion
                    #region Additional Fee 2

                    rentalSecurityBasicData.AdditionalFee2CurrencyType = doValidateBusiness.AdditionalFee2CurrencyType;
                    if (rentalSecurityBasicData.AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.AdditionalFee2 = null;
                        rentalSecurityBasicData.AdditionalFee2Usd = doValidateBusiness.AdditionalFee2;
                    }
                    else
                    {
                        rentalSecurityBasicData.AdditionalFee2 = doValidateBusiness.AdditionalFee2;
                        rentalSecurityBasicData.AdditionalFee2Usd = null;
                    }

                    #endregion
                    #region Additional Fee 3

                    rentalSecurityBasicData.AdditionalFee3CurrencyType = doValidateBusiness.AdditionalFee3CurrencyType;
                    if (rentalSecurityBasicData.AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.AdditionalFee3 = null;
                        rentalSecurityBasicData.AdditionalFee3Usd = doValidateBusiness.AdditionalFee3;
                    }
                    else
                    {
                        rentalSecurityBasicData.AdditionalFee3 = doValidateBusiness.AdditionalFee3;
                        rentalSecurityBasicData.AdditionalFee3Usd = null;
                    }

                    #endregion

                    rentalSecurityBasicData.InstallationTypeCode = doValidateBusiness.InstallationTypeCode;
                    rentalSecurityBasicData.InstallationCompleteDate = doValidateBusiness.InstallationCompleteDate;

                    //rentalSecurityBasicData.NormalInstallFee = doValidateBusiness.NormalInstallFee;
                    //rentalSecurityBasicData.OrderInstallFee = doValidateBusiness.OrderInstallFee;
                    //rentalSecurityBasicData.OrderInstallFee_ApproveContract = doValidateBusiness.OrderInstallFee_ApproveContract;
                    //rentalSecurityBasicData.OrderInstallFee_CompleteInstall = doValidateBusiness.OrderInstallFee_CompleteInstall;
                    //rentalSecurityBasicData.OrderInstallFee_StartService = doValidateBusiness.OrderInstallFee_StartService;

                    #region Normal Install Fee

                    rentalSecurityBasicData.NormalInstallFeeCurrencyType = doValidateBusiness.NormalInstallFeeCurrencyType;
                    if (rentalSecurityBasicData.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.NormalInstallFee = null;
                        rentalSecurityBasicData.NormalInstallFeeUsd = doValidateBusiness.NormalInstallFee;
                    }
                    else
                    {
                        rentalSecurityBasicData.NormalInstallFee = doValidateBusiness.NormalInstallFee;
                        rentalSecurityBasicData.NormalInstallFeeUsd = null;
                    }

                    #endregion
                    #region Order Install Fee

                    rentalSecurityBasicData.OrderInstallFeeCurrencyType = doValidateBusiness.OrderInstallFeeCurrencyType;
                    if (rentalSecurityBasicData.OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.OrderInstallFee = null;
                        rentalSecurityBasicData.OrderInstallFeeUsd = doValidateBusiness.OrderInstallFee;
                    }
                    else
                    {
                        rentalSecurityBasicData.OrderInstallFee = doValidateBusiness.OrderInstallFee;
                        rentalSecurityBasicData.OrderInstallFeeUsd = null;
                    }

                    #endregion
                    #region Order Install Fee Approve Contract

                    rentalSecurityBasicData.OrderInstallFee_ApproveContractCurrencyType = doValidateBusiness.OrderInstallFee_ApproveContractCurrencyType;
                    if (rentalSecurityBasicData.OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.OrderInstallFee_ApproveContract = null;
                        rentalSecurityBasicData.OrderInstallFee_ApproveContractUsd = doValidateBusiness.OrderInstallFee_ApproveContract;
                    }
                    else
                    {
                        rentalSecurityBasicData.OrderInstallFee_ApproveContract = doValidateBusiness.OrderInstallFee_ApproveContract;
                        rentalSecurityBasicData.OrderInstallFee_ApproveContractUsd = null;
                    }

                    #endregion
                    #region Order Install Fee Complete Install

                    rentalSecurityBasicData.OrderInstallFee_CompleteInstallCurrencyType = doValidateBusiness.OrderInstallFee_CompleteInstallCurrencyType;
                    if (rentalSecurityBasicData.OrderInstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.OrderInstallFee_CompleteInstall = null;
                        rentalSecurityBasicData.OrderInstallFee_CompleteInstallUsd = doValidateBusiness.OrderInstallFee_CompleteInstall;
                    }
                    else
                    {
                        rentalSecurityBasicData.OrderInstallFee_CompleteInstall = doValidateBusiness.OrderInstallFee_CompleteInstall;
                        rentalSecurityBasicData.OrderInstallFee_CompleteInstallUsd = null;
                    }

                    #endregion
                    #region Order Install Fee Start Service

                    rentalSecurityBasicData.OrderInstallFee_StartServiceCurrencyType = doValidateBusiness.OrderInstallFee_StartServiceCurrencyType;
                    if (rentalSecurityBasicData.OrderInstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.OrderInstallFee_StartService = null;
                        rentalSecurityBasicData.OrderInstallFee_StartServiceUsd = doValidateBusiness.OrderInstallFee_StartService;
                    }
                    else
                    {
                        rentalSecurityBasicData.OrderInstallFee_StartService = doValidateBusiness.OrderInstallFee_StartService;
                        rentalSecurityBasicData.OrderInstallFee_StartServiceUsd = null;
                    }

                    #endregion
                    #region Install Fee Paid by SECOM

                    rentalSecurityBasicData.InstallFeePaidBySECOMCurrencyType = doValidateBusiness.InstallFeePaidBySECOMCurrencyType;
                    if (rentalSecurityBasicData.InstallFeePaidBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.InstallFeePaidBySECOM = null;
                        rentalSecurityBasicData.InstallFeePaidBySECOMUsd = doValidateBusiness.InstallFeePaidBySECOM;
                    }
                    else
                    {
                        rentalSecurityBasicData.InstallFeePaidBySECOM = doValidateBusiness.InstallFeePaidBySECOM;
                        rentalSecurityBasicData.InstallFeePaidBySECOMUsd = null;
                    }

                    #endregion
                    #region Install Fee Revenue by SECOM

                    rentalSecurityBasicData.InstallFeeRevenueBySECOMCurrencyType = doValidateBusiness.InstallFeeRevenueBySECOMCurrencyType;
                    if (rentalSecurityBasicData.InstallFeeRevenueBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        rentalSecurityBasicData.InstallFeeRevenueBySECOM = null;
                        rentalSecurityBasicData.InstallFeeRevenueBySECOMUsd = doValidateBusiness.InstallFeeRevenueBySECOM;
                    }
                    else
                    {
                        rentalSecurityBasicData.InstallFeeRevenueBySECOM = doValidateBusiness.InstallFeeRevenueBySECOM;
                        rentalSecurityBasicData.InstallFeeRevenueBySECOMUsd = null;
                    }

                    #endregion

                    dsRentalContractPrevious.dtTbt_RentalSecurityBasic.Add(rentalSecurityBasicData);


                    //dtTbt_RentalBEDetails
                    if (session.DSRentalContractPrevious.dtTbt_RentalBEDetails != null && session.DSRentalContractPrevious.dtTbt_RentalBEDetails.Count > 0)
                    {
                        dsRentalContractPrevious.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>();
                        foreach (tbt_RentalBEDetails data in session.DSRentalContractPrevious.dtTbt_RentalBEDetails)
                        {
                            tbt_RentalBEDetails rentalBEDetailsData = CommonUtil.CloneObject<tbt_RentalBEDetails, tbt_RentalBEDetails>(data);
                            rentalBEDetailsData.OCC = strOCC;
                            dsRentalContractPrevious.dtTbt_RentalBEDetails.Add(rentalBEDetailsData);
                        }
                    }

                    //dtTbt_RentalInstrumentDetails
                    if (session.DSRentalContractPrevious.dtTbt_RentalInstrumentDetails != null && session.DSRentalContractPrevious.dtTbt_RentalInstrumentDetails.Count > 0)
                    {
                        dsRentalContractPrevious.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                        foreach (tbt_RentalInstrumentDetails data in session.DSRentalContractPrevious.dtTbt_RentalInstrumentDetails)
                        {
                            tbt_RentalInstrumentDetails rentalInstrumentDetailsData = CommonUtil.CloneObject<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(data);
                            rentalInstrumentDetailsData.OCC = strOCC;
                            dsRentalContractPrevious.dtTbt_RentalInstrumentDetails.Add(rentalInstrumentDetailsData);
                        }
                    }

                    //dtTbt_RentalMaintenanceDetails
                    if (session.DSRentalContractPrevious.dtTbt_RentalMaintenanceDetails != null && session.DSRentalContractPrevious.dtTbt_RentalMaintenanceDetails.Count > 0)
                    {
                        dsRentalContractPrevious.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                        foreach (tbt_RentalMaintenanceDetails data in session.DSRentalContractPrevious.dtTbt_RentalMaintenanceDetails)
                        {
                            tbt_RentalMaintenanceDetails rentalMaintenanceDetailsData = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(data);
                            rentalMaintenanceDetailsData.OCC = strOCC;
                            dsRentalContractPrevious.dtTbt_RentalMaintenanceDetails.Add(rentalMaintenanceDetailsData);
                        }
                    }

                    //tbt_RentalOperationType
                    List<tbt_RentalOperationType> doTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                    if (doValidateBusiness.OperationType != null)
                    {
                        foreach (string opt in doValidateBusiness.OperationType)
                        {
                            doTbt_RentalOperationType.Add(new tbt_RentalOperationType()
                            {
                                ContractCode = session.DSRentalContractPrevious.dtTbt_RentalSecurityBasic[0].ContractCode,
                                OCC = strOCC,
                                OperationTypeCode = opt
                            });
                        }
                        dsRentalContractPrevious.dtTbt_RentalOperationType = doTbt_RentalOperationType;
                    }

                    //dtTbt_RentalSentryGuard
                    if (session.DSRentalContractPrevious.dtTbt_RentalSentryGuard != null && session.DSRentalContractPrevious.dtTbt_RentalSentryGuard.Count > 0)
                    {
                        dsRentalContractPrevious.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>();
                        foreach (tbt_RentalSentryGuard data in session.DSRentalContractPrevious.dtTbt_RentalSentryGuard)
                        {
                            tbt_RentalSentryGuard rentalSentryGuardData = CommonUtil.CloneObject<tbt_RentalSentryGuard, tbt_RentalSentryGuard>(data);
                            rentalSentryGuardData.OCC = strOCC;
                            dsRentalContractPrevious.dtTbt_RentalSentryGuard.Add(rentalSentryGuardData);
                        }
                    }

                    //dtTbt_RentalSentryGuardDetails
                    if (session.DSRentalContractPrevious.dtTbt_RentalSentryGuardDetails != null && session.DSRentalContractPrevious.dtTbt_RentalSentryGuardDetails.Count > 0)
                    {
                        dsRentalContractPrevious.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                        foreach (tbt_RentalSentryGuardDetails data in session.DSRentalContractPrevious.dtTbt_RentalSentryGuardDetails)
                        {
                            tbt_RentalSentryGuardDetails rentalSentryGuardDetailsData = CommonUtil.CloneObject<tbt_RentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(data);
                            rentalSentryGuardDetailsData.OCC = strOCC;
                            dsRentalContractPrevious.dtTbt_RentalSentryGuardDetails.Add(rentalSentryGuardDetailsData);
                        }
                    }

                    //dtTbt_RelationType
                    if (session.DSRentalContractPrevious.dtTbt_RelationType != null && session.DSRentalContractPrevious.dtTbt_RelationType.Count > 0)
                    {
                        dsRentalContractPrevious.dtTbt_RelationType = new List<tbt_RelationType>();
                        foreach (tbt_RelationType data in session.DSRentalContractPrevious.dtTbt_RelationType)
                        {
                            tbt_RelationType relationTypeData = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(data);
                            relationTypeData.OCC = strOCC;
                            dsRentalContractPrevious.dtTbt_RelationType.Add(relationTypeData);
                        }
                    }

                    //dtTbt_RentalInstSubcontractor
                    //if (session.DSRentalContractPrevious.dtTbt_RentalInstSubcontractor != null && session.DSRentalContractPrevious.dtTbt_RentalInstSubcontractor.Count > 0)
                    //{
                    //    dsRentalContractPrevious.dtTbt_RentalInstSubcontractor = new List<tbt_RentalInstSubcontractor>();
                    //    foreach (tbt_RentalInstSubcontractor data in session.DSRentalContractPrevious.dtTbt_RentalInstSubcontractor)
                    //    {
                    //        tbt_RentalInstSubcontractor subcontractorData = CommonUtil.CloneObject<tbt_RentalInstSubcontractor, tbt_RentalInstSubcontractor>(data);
                    //        subcontractorData.OCC = strOCC;
                    //        dsRentalContractPrevious.dtTbt_RentalInstSubcontractor.Add(subcontractorData);
                    //    }
                    //}
                    dsRentalContractPrevious.dtTbt_RentalInstSubcontractor = new List<tbt_RentalInstSubcontractor>();
                }


                using (TransactionScope scope = new TransactionScope())
                {
                    if (doValidateBusiness.ProcessType == "Correct")
                    {
                        registerResult = rentralContractHandler.RegisterCP34Correct(session.DSRentalContract, session.DOValidateBusiness.IsSendNotifyEmail);
                        if (registerResult == true)
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
                        else
                            res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3043);
                    }

                    if (doValidateBusiness.ProcessType == "Insert")
                    {
                        registerResult = rentralContractHandler.RegisterCP34Insert(dsRentalContractPrevious);
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
                    }

                    if (doValidateBusiness.ProcessType == "Delete")
                    {
                        registerResult = rentralContractHandler.RegisterCP34Delete(session.DSRentalContract);
                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
                    }

                    scope.Complete();
                }

            }
            catch (Exception ex)
            {
                //res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //14. Event: Click [OK] button in ‘Action button’ section
        /// <summary>
        /// Register maintenanced data to database when click [OK] button in ‘Action button’ section
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ActionResult OKClick_CTS220(CTS220_ValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            ObjectResultData resValidate = new ObjectResultData();

            ICommonHandler commonHandler;
            IRentralContractHandler rentralContractHandler;

            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                if (commonHandler.IsSystemSuspending())
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049, "");
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
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

        //2. initialSelectProcessMode (อยู่ใน javascript ไฟล์ CTS220_01.js)
        //3. initialCorrectDataMode (อยู่ใน javascript ไฟล์ CTS220_01.js)
        //4. initialInsertDataMode (อยู่ใน javascript ไฟล์ CTS220_01.js)
        //5. InitialDeleteDataMode (อยู่ใน javascript ไฟล์ CTS220_01.js)

        /// <summary>
        /// Check system suspending and user’s permission
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public ActionResult CTS220_Authority(CTS220_ScreenParameter session)
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comUtil = new CommonUtil();

            try
            {
                //CTS220_ScreenParameter session = InitialScreenSession_CTS220();

                //CheckSystemStatus
                if (CheckIsSuspending(res) == true)
                    return Json(res);

                //Check user’s permission
                if (CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_ADD) == false
                    && CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_EDIT) == false
                    && CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_DEL) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //if (String.IsNullOrEmpty(session.ContractCode))
                //    session.ContractCode = CommonUtil.dsTransData.dtCommonSearch.ContractCode;
                if (String.IsNullOrEmpty(session.ContractCode) && session.CommonSearch != null)
                {
                    if (CommonUtil.IsNullOrEmpty(session.CommonSearch.ContractCode) == false)
                        session.ContractCode = session.CommonSearch.ContractCode;
                }

                //Check required field
                if (String.IsNullOrEmpty(session.ContractCode))
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                    //                    ScreenID.C_SCREEN_ID_CP34,
                    //                    MessageUtil.MODULE_COMMON,
                    //                    MessageUtil.MessageList.MSG0007,
                    //                    new string[] { "lblContractCode" },
                    //                    null);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0147);

                    return Json(res);
                }
                else
                {
                    CommonUtil cmmUtil = new CommonUtil();
                    IRentralContractHandler rHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                    bool isExist = false;
                    List<tbt_RentalContractBasic> lst = rHandler.GetTbt_RentalContractBasic(cmmUtil.ConvertContractCode(session.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);
                    if (lst.Count > 0)
                        isExist = true;

                    if (isExist == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0124);
                        return Json(res);
                    }
                }


                session.ScreenParameter = new CTS220_Parameter();
                session.ScreenParameter.contractCode = comUtil.ConvertContractCode(session.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS220_ScreenParameter>("CTS220", session, res);
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS220")]
        public ActionResult CTS220()
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comU;

            try
            {
                CTS220_ScreenParameter session = GetScreenObject<CTS220_ScreenParameter>();
                comU = new CommonUtil();
                ViewBag.ContractCode = comU.ConvertContractCode(session.ScreenParameter.contractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                session.ContractCode = session.ScreenParameter.contractCode;
                //HasAuthority_CTS220(session.ScreenParameter.contractCode.Trim());

                ViewBag.PermissionAdd = CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_ADD);
                ViewBag.PermissionEdit = CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_EDIT);
                ViewBag.PermissionDel = CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_DEL);

                CTS220_03();

                return View();
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
        /// <returns></returns>
        public ActionResult CTS220_02()
        {
            ObjectResultData res = new ObjectResultData();
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                InitialScreen_CTS220(session.ContractCode);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return View("CTS220/_CTS220_02");
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS220_03()
        {
            ObjectResultData res = new ObjectResultData();
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();

                ViewBag.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE = RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE;
                ViewBag.C_RENTAL_CHANGE_TYPE_CHANGE_NAME = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME;
                ViewBag.C_RENTAL_CHANGE_TYPE_END_CONTRACT = RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT;
                ViewBag.C_RENTAL_CHANGE_TYPE_CANCEL = RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL;
                ViewBag.C_RENTAL_CHANGE_TYPE_STOP = RentalChangeType.C_RENTAL_CHANGE_TYPE_STOP;
                ViewBag.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP;
                ViewBag.C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP;
                ViewBag.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START = RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START;
                ViewBag.C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP = RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP;
                ViewBag.C_RENTAL_CHANGE_TYPE_REMOVE_ALL = RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_ALL;


                if (session.DSRentalContractShow != null && session.DSRentalContractShow.dtTbt_RentalOperationType != null
                    && session.DSRentalContractShow.dtTbt_RentalOperationType.Count() > 0)
                {
                    List<string> lstOpt = new List<string>();
                    foreach (tbt_RentalOperationType opt in session.DSRentalContractShow.dtTbt_RentalOperationType)
                    {
                        lstOpt.Add(opt.OperationTypeCode);
                    }
                    ViewBag.OperationTypeList = lstOpt.ToArray();
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return View("CTS220/_CTS220_03");
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS220_04()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                BindDoChangeContractCondition_CTS220();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return View("CTS220/_CTS220_04");
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS220_05()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return View("CTS220/_CTS220_05");
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS220_06()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                BindDoContractDocument_CTS220();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return View("CTS220/_CTS220_06");
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS220_07()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return View("CTS220/_CTS220_07");
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS220_08()
        {
            ObjectResultData res = new ObjectResultData();
            CommonUtil comU;
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                comU = new CommonUtil();
                if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic.Count() != 0)
                {
                    if (session.ProcessType != "Insert")
                    {
                        ViewBag.QuotationTargetCode = comU.ConvertQuotationTargetCode(session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        ViewBag.QuotationAlphabet = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].QuotationAlphabet;
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return View("CTS220/_CTS220_08");
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS220_09()
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return View("CTS220/_CTS220_09");
        }

        /// <summary>
        /// Initial basic screen item
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        public ActionResult InitialScreen_CTS220(string contractCode)
        {
            ObjectResultData res = new ObjectResultData();
            IUserControlHandler userControlHandler;
            IRentralContractHandler rentralContractHandler;
            dsRentalContractData dsRentalContract;
            doRentalContractBasicInformation doContractBasicInformation;
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                userControlHandler = ServiceContainer.GetService<IUserControlHandler>() as IUserControlHandler;

                doContractBasicInformation = userControlHandler.GetRentalContactBasicInformationData(contractCode);
                BindDOContractBasicInformation_CTS220(doContractBasicInformation);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Initial session of screen
        /// </summary>
        /// <returns></returns>
        public CTS220_ScreenParameter InitialScreenSession_CTS220()
        {
            try
            {
                CTS220_ScreenParameter importData = new CTS220_ScreenParameter()
                {
                    ScreenParameter = new CTS220_Parameter(),
                    DOValidateBusiness = new CTS220_ValidateBusiness(),
                    DSRentalContract = new dsRentalContractData(),
                    DSRentalContractPrevious = new dsRentalContractData(),
                    DSRentalContractNext = new dsRentalContractData(),
                    DSRentalContractShow = new dsRentalContractData(),
                    ListRentalSecurityBasic = new List<tbt_RentalSecurityBasic>(),
                    ListSubContractor = new List<dtTbt_RentalInstSubContractorListForView>(),
                };

                CTS220_SetImportData(importData);
                return importData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check screen has authority
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        public ActionResult HasAuthority_CTS220(string contractCode)
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

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_EDIT))
                    ViewBag.PermissionEdit = false;
                else
                    ViewBag.PermissionEdit = true;

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_CP34, FunctionID.C_FUNC_ID_DEL))
                    ViewBag.PermissionDelete = false;
                else
                    ViewBag.PermissionDelete = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Bind ContractBasic to screen
        /// </summary>
        /// <param name="doRentalContractBasic"></param>
        public void BindDOContractBasicInformation_CTS220(doRentalContractBasicInformation doRentalContractBasic)
        {
            CommonUtil comU;
            IMasterHandler masterHandler;
            CTS220_ScreenParameter session;

            try
            {
                comU = new CommonUtil();
                session = CTS220_GetImportData();
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                ViewBag.ContractCode = comU.ConvertContractCode(doRentalContractBasic.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //ViewBag.OCC = doRentalContractBasic.l
                ViewBag.UserCode = doRentalContractBasic.UserCode;
                ViewBag.CustomerCode = doRentalContractBasic.ContractTargetCustCodeShort;
                ViewBag.RealCustomerCustCode = comU.ConvertCustCode(doRentalContractBasic.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                ViewBag.SiteCode = doRentalContractBasic.SiteCodeShort;
                ViewBag.ContractTargetCustomerImportant = doRentalContractBasic.ContractTargetCustomerImportant;
                ViewBag.CustFullNameEN = doRentalContractBasic.ContractTargetNameEN;
                ViewBag.CustFullNameLC = doRentalContractBasic.ContractTargetNameLC;
                ViewBag.AddressFullEN = doRentalContractBasic.ContractTargetAddressEN;
                ViewBag.AddressFullLC = doRentalContractBasic.ContractTargetAddressLC;
                ViewBag.SiteNameEN = doRentalContractBasic.SiteNameEN;
                ViewBag.SiteNameLC = doRentalContractBasic.SiteNameLC;
                ViewBag.SiteAddressEN = doRentalContractBasic.SiteAddressEN;
                ViewBag.SiteAddressLC = doRentalContractBasic.SiteAddressLC;

                //ViewBag.InstallationStatus = doRentalContractBasic.InstallationStatusCode + ":" + doRentalContractBasic.InstallationStatusName;
                ViewBag.InstallationStatus = string.Format("{0}:{1}", doRentalContractBasic.InstallationStatusCode, doRentalContractBasic.InstallationStatusName); //Modify by Jutarat A. on 16052013

                if (session != null && session.DSRentalContractShow != null && session.DSRentalContractShow.dtTbt_RentalSecurityBasic != null
                    && session.DSRentalContractShow.dtTbt_RentalSecurityBasic.Count > 0) //Add by Jutarat A. on 19022013
                {
                    //Modify by Jutarat A. on 16052013
                    //ViewBag.QuotationTargetCode = comU.ConvertQuotationTargetCode(session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT).Trim();
                    string strQuotationTargetCode = comU.ConvertQuotationTargetCode(session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    if (String.IsNullOrEmpty(strQuotationTargetCode) == false)
                        strQuotationTargetCode = strQuotationTargetCode.Trim();

                    ViewBag.QuotationTargetCode = strQuotationTargetCode;
                    //End Modify
                    
                    ViewBag.EndContractDate = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ContractEndDate;
                }

                ViewBag.RentalContractBasicInformation = doRentalContractBasic;
                ViewBag.ContractCodeLong = ViewBag.ContractCode;
                ViewBag.ContractCode = doRentalContractBasic.ContractCodeShort;
                ViewBag.UserCode = doRentalContractBasic.UserCode;
                ViewBag.CustomerCode = doRentalContractBasic.ContractTargetCustCodeShort;
                ViewBag.RealCustomerCode = doRentalContractBasic.RealCustomerCustCode;
                ViewBag.SiteCode = doRentalContractBasic.SiteCodeShort;

                if (session != null && session.ListRentalSecurityBasic != null && session.ListRentalSecurityBasic.Count > 0)
                    ViewBag.SecurityTypeCode = session.ListRentalSecurityBasic[0].SecurityTypeCode;

                ViewBag.ProductName = GetProductName_CTS220();
                ViewBag.OperationOffice = doRentalContractBasic.OperationOfficeName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Bind Product to screen
        /// </summary>
        /// <returns></returns>
        public ActionResult BindDOProductInformation_CTS220()
        {
            ObjectResultData res = new ObjectResultData();
            IMasterHandler masterHandler;
            IEmployeeMasterHandler employeeHandler;
            CTS220_DOProductInformation doProductInformation;
            CTS220_ScreenParameter session;

            try
            {
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                session = CTS220_GetImportData();
                doProductInformation = new CTS220_DOProductInformation();

                //if (session.DOValidateBusiness.ProcessType == "Correct")
                //{
                if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic.Count() != 0)
                {
                    #region Order Contract Fee

                    //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderContractFee != null)
                    //    doProductInformation.OrderContractFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderContractFee.Value.ToString("#,##0.00");

                    doProductInformation.OrderContractFeeCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType;
                    if (doProductInformation.OrderContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd != null)
                            doProductInformation.OrderContractFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd.Value.ToString("#,##0.00");
                    }
                    else
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderContractFee != null)
                            doProductInformation.OrderContractFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderContractFee.Value.ToString("#,##0.00");
                    }

                    #endregion
                    #region Contract Fee on Stop

                    ////Add by Jutarat A. on 14082012
                    //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop != null)
                    //    doProductInformation.ContractFeeOnStop = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop.Value.ToString("#,##0.00");
                    ////End Add

                    doProductInformation.ContractFeeOnStopCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopCurrencyType;
                    if (doProductInformation.ContractFeeOnStopCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopUsd != null)
                            doProductInformation.ContractFeeOnStop = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ContractFeeOnStopUsd.Value.ToString("#,##0.00");
                    }
                    else
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop != null)
                            doProductInformation.ContractFeeOnStop = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ContractFeeOnStop.Value.ToString("#,##0.00");
                    }

                    #endregion
                    

                    doProductInformation.ChangeImplementDate = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;
                    doProductInformation.SecurityTypeCode = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].SecurityTypeCode;
                    doProductInformation.ProductCode = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ProductCode;
                    doProductInformation.ChangeType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeType;
                    
                    //doProductInformation.ChangeReasonType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeReasonType;
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME
                        || session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP)
                    {
                        doProductInformation.ChangeReasonType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeNameReasonType;
                    }
                    else if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE)
                    {
                        doProductInformation.ChangeReasonType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeReasonType;
                    }
                    else if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT
                        || session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL
                        || session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_STOP
                        || session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP
                        || session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START
                        || session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP
                        || session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_ALL)
                    {
                        doProductInformation.ChangeReasonType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].StopCancelReasonType;
                    }

                    doProductInformation.SaleManEmpNo1 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1;
                    doProductInformation.SaleManEmpName1 = GetActiveEmployeeName_CTS220(session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1);
                    doProductInformation.SaleManEmpNo2 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2;
                    doProductInformation.SaleManEmpName2 = GetActiveEmployeeName_CTS220(session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2);
                    doProductInformation.ApproveNo1 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ApproveNo1;
                    doProductInformation.ApproveNo2 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ApproveNo2;
                    doProductInformation.CreateDate = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].CreateDate;
                    doProductInformation.CreateBy = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].CreateBy;
                    doProductInformation.CreateByName = GetActiveEmployeeName_CTS220(session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].CreateBy);
                    doProductInformation.SecurityMemo = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].SecurityMemo;
                }

                //if (session.DSRentalContractShow.dtTbt_RentalOperationType.Count() != 0)
                //{
                //    doProductInformation.OperationTypeCode = session.DSRentalContractShow.dtTbt_RentalOperationType[0].OperationTypeCode;
                //}
                //}

                return Json(doProductInformation);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Bind ChangeContract to screen
        /// </summary>
        public void BindDoChangeContractCondition_CTS220()
        {
            ObjectResultData res = new ObjectResultData();
            IEmployeeMasterHandler employeeHandler;

            List<dtEmpNo> listEmpNo;
            CTS220_ScreenParameter session;
            CTS220_DOEmployee doEmployee;

            try
            {
                session = CTS220_GetImportData();
                doEmployee = new CTS220_DOEmployee();

                employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                ViewBag.NegotiationStaffEmpNo1 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo1;
                ViewBag.NegotiationStaffEmpNo2 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NegotiationStaffEmpNo2;

                listEmpNo = employeeHandler.GetEmployeeNameByEmpNo(ViewBag.NegotiationStaffEmpNo1);
                if (listEmpNo.Count() != 0)
                {
                    if (ViewBag.NegotiationStaffEmpNo1 != null && ViewBag.NegotiationStaffEmpNo1 != "")
                        ViewBag.NegotiationStaffEmpName1 = GetActiveEmployeeName_CTS220(ViewBag.NegotiationStaffEmpNo1);
                }

                listEmpNo = employeeHandler.GetEmployeeNameByEmpNo(ViewBag.NegotiationStaffEmpNo2);
                if (listEmpNo.Count() != 0)
                {
                    if (ViewBag.NegotiationStaffEmpNo2 != null && ViewBag.NegotiationStaffEmpNo2 != "")
                        ViewBag.NegotiationStaffEmpName2 = GetActiveEmployeeName_CTS220(ViewBag.NegotiationStaffEmpNo2);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Bind Insurance to screen
        /// </summary>
        /// <returns></returns>
        public ActionResult BindDoInsurance_CTS220()
        {
            ObjectResultData res = new ObjectResultData();
            CTS220_DOInsuranceType doInsuranceType;
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();

                doInsuranceType = new CTS220_DOInsuranceType();
                doInsuranceType.InsuranceTypeCode = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InsuranceTypeCode;

                #region Insurance Coverage Amount

                //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmount != null)
                //    doInsuranceType.InsuranceCoverageAmount = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmount.Value.ToString("#,##0.00");
                //else
                //    doInsuranceType.InsuranceCoverageAmount = "";

                doInsuranceType.InsuranceCoverageAmountCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmountCurrencyType;
                if (doInsuranceType.InsuranceCoverageAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmountUsd != null)
                        doInsuranceType.InsuranceCoverageAmount = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmountUsd.Value.ToString("#,##0.00");
                }
                else
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmount != null)
                        doInsuranceType.InsuranceCoverageAmount = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InsuranceCoverageAmount.Value.ToString("#,##0.00");
                }

                #endregion
                #region Monthly Insurance Fee

                //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFee != null)
                //    doInsuranceType.MonthlyInsuranceFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFee.Value.ToString("#,##0.00");
                //else
                //    doInsuranceType.MonthlyInsuranceFee = "";

                doInsuranceType.MonthlyInsuranceFeeCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFeeCurrencyType;
                if (doInsuranceType.MonthlyInsuranceFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFeeUsd != null)
                        doInsuranceType.MonthlyInsuranceFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFeeUsd.Value.ToString("#,##0.00");
                }
                else
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFee != null)
                        doInsuranceType.MonthlyInsuranceFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MonthlyInsuranceFee.Value.ToString("#,##0.00");
                }

                #endregion


                

                return Json(doInsuranceType);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Bind ContractDocument to screen
        /// </summary>
        public void BindDoContractDocument_CTS220()
        {
            ObjectResultData res = new ObjectResultData();
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                //ViewBag.DocAuditResult = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].DocAuditResult;
                ViewBag.DocAuditResult = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].DocAuditResultCodeName;

                ViewBag.DocumentCode = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].DocumentCode;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
        }

        /// <summary>
        /// Bind FutureDate to screen
        /// </summary>
        /// <returns></returns>
        public ActionResult BindDoFutureDate_CTS220()
        {
            ObjectResultData res = new ObjectResultData();
            CTS220_DOFutureDate doFutureDate;
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                doFutureDate = new CTS220_DOFutureDate();
                doFutureDate.ExpectedResumeDate = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ExpectedResumeDate;
                doFutureDate.ReturnToOriginalFeeDate = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate;

                return Json(doFutureDate);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Bind Quotaion to screen
        /// </summary>
        /// <returns></returns>
        public ActionResult BindDOQuotaion_CTS220()
        {
            ObjectResultData res = new ObjectResultData();
            CTS220_DOQuotation doQuotation;
            CommonUtil comU;
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                comU = new CommonUtil();
                doQuotation = new CTS220_DOQuotation();

                if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic.Count() != 0)
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].QuotationTargetCode == null 
                        || session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].QuotationTargetCode == "")
                        doQuotation.QuotationTargetCode = "-";
                    else
                        doQuotation.QuotationTargetCode = comU.ConvertQuotationTargetCode(session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT) + "-" + session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].QuotationAlphabet;

                    doQuotation.QuotationAlphabet = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].QuotationAlphabet;
                    doQuotation.PlanCode = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].PlanCode;
                    doQuotation.PlanApproveDate = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].PlanApproveDate;
                    doQuotation.PlanApproverEmpNo = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].PlanApproverEmpNo;
                    //doQuotation.PlanApproverEmpName = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].PlanApproverName;
                    doQuotation.PlanApproverEmpName = GetActiveEmployeeName_CTS220(session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].PlanApproverEmpNo);


                    #region Normal Contract Fee

                    //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalContractFee != null)
                    //    doQuotation.NormalContractFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalContractFee.Value.ToString("#,##0.00");

                    doQuotation.NormalContractFeeCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalContractFeeCurrencyType;
                    if (doQuotation.NormalContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalContractFeeUsd != null)
                            doQuotation.NormalContractFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalContractFeeUsd.Value.ToString("#,##0.00");
                    }
                    else
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalContractFee != null)
                            doQuotation.NormalContractFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalContractFee.Value.ToString("#,##0.00");
                    }

                    #endregion
                    #region Maintenance Fee 1

                    //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MaintenanceFee1 != null)
                    //    doQuotation.MaintenanceFee1 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MaintenanceFee1.Value.ToString("#,##0.00");

                    doQuotation.MaintenanceFee1CurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MaintenanceFee1CurrencyType;
                    if (doQuotation.MaintenanceFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MaintenanceFee1Usd != null)
                            doQuotation.MaintenanceFee1 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MaintenanceFee1Usd.Value.ToString("#,##0.00");
                    }
                    else
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MaintenanceFee1 != null)
                            doQuotation.MaintenanceFee1 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].MaintenanceFee1.Value.ToString("#,##0.00");
                    }

                    #endregion
                    #region Additional Fee 1

                    //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee1 != null)
                    //    doQuotation.AdditionalFee1 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee1.Value.ToString("#,##0.00");

                    doQuotation.AdditionalFee1CurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee1CurrencyType;
                    if (doQuotation.AdditionalFee1CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee1Usd != null)
                            doQuotation.AdditionalFee1 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee1Usd.Value.ToString("#,##0.00");
                    }
                    else
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee1 != null)
                            doQuotation.AdditionalFee1 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee1.Value.ToString("#,##0.00");
                    }

                    #endregion
                    #region Additional Fee 2

                    //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee2 != null)
                    //    doQuotation.AdditionalFee2 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee2.Value.ToString("#,##0.00");

                    doQuotation.AdditionalFee2CurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee2CurrencyType;
                    if (doQuotation.AdditionalFee2CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee2Usd != null)
                            doQuotation.AdditionalFee2 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee2Usd.Value.ToString("#,##0.00");
                    }
                    else
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee2 != null)
                            doQuotation.AdditionalFee2 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee2.Value.ToString("#,##0.00");
                    }

                    #endregion
                    #region Additional Fee 3

                    //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee3 != null)
                    //    doQuotation.AdditionalFee3 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee3.Value.ToString("#,##0.00");

                    doQuotation.AdditionalFee3CurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee3CurrencyType;
                    if (doQuotation.AdditionalFee3CurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee3Usd != null)
                            doQuotation.AdditionalFee3 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee3Usd.Value.ToString("#,##0.00");
                    }
                    else
                    {
                        if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee3 != null)
                            doQuotation.AdditionalFee3 = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].AdditionalFee3.Value.ToString("#,##0.00");
                    }

                    #endregion

                    if (session.ProcessType == "Insert")
                    {
                        doQuotation.QuotationTargetCode = "-";
                        doQuotation.PlanCode = null;
                        doQuotation.PlanApproveDate = null;
                        doQuotation.PlanApproverEmpNo = null;
                        doQuotation.PlanApproverEmpName = null;
                    }
                }

                return Json(doQuotation);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Bind Installation to screen
        /// </summary>
        /// <returns></returns>
        public ActionResult BindDOInstallationInformation_CTS220()
        {
            ObjectResultData res = new ObjectResultData();
            CTS220_DOInstallation doInstallation;
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();

                doInstallation = new CTS220_DOInstallation();
                doInstallation.InstallationTypeCode = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallationTypeCode;
                doInstallation.InstallationCompleteDate = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallationCompleteDate;

                #region Normal Install Fee

                //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalInstallFee != null)
                //    doInstallation.NormalInstallFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalInstallFee.Value.ToString("#,##0.00");

                doInstallation.NormalInstallFeeCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalInstallFeeCurrencyType;
                if (doInstallation.NormalInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalInstallFeeUsd != null)
                        doInstallation.NormalInstallFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalInstallFeeUsd.Value.ToString("#,##0.00");
                }
                else
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalInstallFee != null)
                        doInstallation.NormalInstallFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].NormalInstallFee.Value.ToString("#,##0.00");
                }

                #endregion
                #region Order Install Fee

                //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee != null)
                //    doInstallation.OrderInstallFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee.Value.ToString("#,##0.00");

                doInstallation.OrderInstallFeeCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFeeCurrencyType;
                if (doInstallation.OrderInstallFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFeeUsd != null)
                        doInstallation.OrderInstallFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFeeUsd.Value.ToString("#,##0.00");
                }
                else
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee != null)
                        doInstallation.OrderInstallFee = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee.Value.ToString("#,##0.00");
                }

                #endregion
                #region Order Install Fee Approve Contract

                //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract != null)
                //    doInstallation.OrderInstallFee_ApproveContract = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract.Value.ToString("#,##0.00");

                doInstallation.OrderInstallFee_ApproveContractCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractCurrencyType;
                if (doInstallation.OrderInstallFee_ApproveContractCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractUsd != null)
                        doInstallation.OrderInstallFee_ApproveContract = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContractUsd.Value.ToString("#,##0.00");
                }
                else
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract != null)
                        doInstallation.OrderInstallFee_ApproveContract = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract.Value.ToString("#,##0.00");
                }

                #endregion
                #region Order Install Fee Complete Install

                //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall != null)
                //    doInstallation.OrderInstallFee_CompleteInstall = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall.Value.ToString("#,##0.00");

                doInstallation.OrderInstallFee_CompleteInstallCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallCurrencyType;
                if (doInstallation.OrderInstallFee_CompleteInstallCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallUsd != null)
                        doInstallation.OrderInstallFee_CompleteInstall = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstallUsd.Value.ToString("#,##0.00");
                }
                else
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall != null)
                        doInstallation.OrderInstallFee_CompleteInstall = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall.Value.ToString("#,##0.00");
                }

                #endregion
                #region Order Install Fee Start Service

                //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService != null)
                //    doInstallation.OrderInstallFee_StartService = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService.Value.ToString("#,##0.00");

                doInstallation.OrderInstallFee_StartServiceCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceCurrencyType;
                if (doInstallation.OrderInstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceUsd != null)
                        doInstallation.OrderInstallFee_StartService = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartServiceUsd.Value.ToString("#,##0.00");
                }
                else
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService != null)
                        doInstallation.OrderInstallFee_StartService = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService.Value.ToString("#,##0.00");
                }

                #endregion
                #region Install Fee Paid by SECOM

                //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOM != null)
                //    doInstallation.InstallFeePaidBySECOM = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOM.Value.ToString("#,##0.00");

                doInstallation.InstallFeePaidBySECOMCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOMCurrencyType;
                if (doInstallation.InstallFeePaidBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOMUsd != null)
                        doInstallation.InstallFeePaidBySECOM = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOMUsd.Value.ToString("#,##0.00");
                }
                else
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOM != null)
                        doInstallation.InstallFeePaidBySECOM = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeePaidBySECOM.Value.ToString("#,##0.00");
                }

                #endregion
                #region Install Fee Revenue by SECOM

                //if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOM != null)
                //    doInstallation.InstallFeeRevenueBySECOM = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOM.Value.ToString("#,##0.00");

                doInstallation.InstallFeeRevenueBySECOMCurrencyType = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOMCurrencyType;
                if (doInstallation.InstallFeeRevenueBySECOMCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOMUsd != null)
                        doInstallation.InstallFeeRevenueBySECOM = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOMUsd.Value.ToString("#,##0.00");
                }
                else
                {
                    if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOM != null)
                        doInstallation.InstallFeeRevenueBySECOM = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallFeeRevenueBySECOM.Value.ToString("#,##0.00");
                }

                #endregion

                if (session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallationSlipNo != null)
                    doInstallation.InstallationSlipNo = session.DSRentalContractShow.dtTbt_RentalSecurityBasic[0].InstallationSlipNo.ToString();


                if (session.ProcessType == "Insert")
                {
                    doInstallation.InstallationTypeCode = null;
                    doInstallation.InstallationCompleteDate = null;

                    doInstallation.NormalInstallFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    doInstallation.NormalInstallFee = null;
                    doInstallation.OrderInstallFeeCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    doInstallation.OrderInstallFee = null;
                    doInstallation.OrderInstallFee_ApproveContractCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    doInstallation.OrderInstallFee_ApproveContract = null;
                    doInstallation.OrderInstallFee_CompleteInstallCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    doInstallation.OrderInstallFee_CompleteInstall = null;
                    doInstallation.OrderInstallFee_StartServiceCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    doInstallation.OrderInstallFee_StartService = null;
                    doInstallation.InstallationSlipNo = "-";
                }

                return Json(doInstallation);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        //-------------------------------------------------------------

        /// <summary>
        /// Get name of Product
        /// </summary>
        /// <returns></returns>
        public string GetProductName_CTS220()
        {
            string productName = "";
            ObjectResultData res = new ObjectResultData();
            IMasterHandler masterHandler;
            List<tbm_Product> listProduct;
            List<CTS220_DOProductName> listDOProductName;
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                if (session != null && session.DSRentalContract != null && session.DSRentalContract.dtTbt_RentalSecurityBasic != null && session.DSRentalContract.dtTbt_RentalSecurityBasic.Count > 0) //Add by Jutarat A. on 21032013
                {
                    listProduct = masterHandler.GetTbm_Product(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ProductCode, session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ProductTypeCode);
                    listDOProductName = CommonUtil.ClonsObjectList<tbm_Product, CTS220_DOProductName>(listProduct);
                    CommonUtil.MappingObjectLanguage(listDOProductName);

                    //if (listDOProductName.Count() != 0)
                    if (listDOProductName != null && listDOProductName.Count() > 0) //Add by Jutarat A. on 21032013
                        productName = listDOProductName[0].ProductName;
                }

                return productName;
            }
            catch (Exception ex)
            {
            }

            return productName;
        }

        /// <summary>
        /// Get name of Employee
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public string GetActiveEmployeeName_CTS220(string empNo)
        {
            ObjectResultData res = new ObjectResultData();
            IMasterHandler masterHandler;
            IEmployeeMasterHandler employeeHandler;
            List<dtEmpNo> listEmpNo;

            try
            {
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                if (empNo != null)
                {
                    listEmpNo = employeeHandler.GetEmployeeNameByEmpNo(empNo);
                    if (listEmpNo.Count() != 0)
                        return listEmpNo[0].EmployeeNameDisplay;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return "";
        }

        /// <summary>
        /// Get data of ReasonMethod to ComboBox
        /// </summary>
        /// <param name="id"></param>
        /// <param name="changeType"></param>
        /// <returns></returns>
        public ActionResult GetComboBoxReasonMethod_CTS220(string id, string changeType)
        {
            ObjectResultData res = new ObjectResultData();
            List<doMiscTypeCode> lst;
            List<doMiscTypeCode> miscs;
            ComboBoxModel cboModel;

            try
            {
                lst = new List<doMiscTypeCode>();
                cboModel = new ComboBoxModel();

                miscs = new List<doMiscTypeCode>()
                {                    		 	                   
                    new doMiscTypeCode()
                    {                        
                        FieldName = MiscType.C_CHANGE_REASON_TYPE,
                        ValueCode = "%"
                    }
                };

                //4. Event: Select [Change type] drop-down list in ‘Product information’ section 
                //4.2 Load reason data when ddlReason is enabled
                //4.2.1	If ddlChangeType = C_RENTAL_CHANGE_TYPE_CHANGE_NAME Or C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP
                if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME ||
                    changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME_DURING_STOP)
                {
                    miscs = new List<doMiscTypeCode>()
                    {                    		 	                   
                        new doMiscTypeCode()
                        {                        
                            FieldName = MiscType.C_CHANGE_NAME_REASON_TYPE,
                            ValueCode = "%"
                        }
                    };
                }

                //4.2.2	If ddlChangeType = C_RENTAL_CHANGE_TYPE_PLAN_CHANGE
                if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE)
                {
                    miscs = new List<doMiscTypeCode>()
                    {                    		 	                   
                        new doMiscTypeCode()
                        {                        
                            FieldName = MiscType.C_CHANGE_REASON_TYPE,
                            ValueCode = "%"
                        }
                    };
                }

                //4.2.3	If ddlChangeType = C_RENTAL_CHANGE_TYPE_END_CONTRACT Or C_RENTAL_CHANGE_TYPE_CANCEL Or C_RENTAL_CHANGE_TYPE_STOP
                //Or C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP Or C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START Or C_RENTAL_CHANGE_TYPE_REMOVE_ALL
                if (changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_END_CONTRACT ||
                    changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL ||
                    changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_STOP ||
                    changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP ||
                    changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START ||
                    changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP ||
                    changeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_ALL)
                {
                    miscs = new List<doMiscTypeCode>()
                    {                    		 	                   
                        new doMiscTypeCode()
                        {                        
                            FieldName = MiscType.C_STOP_CANCEL_REASON_TYPE,
                            ValueCode = "%"
                        }
                    };
                }

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);
                cboModel.SetList<doMiscTypeCode>(lst, "ValueCodeDisplay", "ValueCode");
                res.ResultData = cboModel;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Get data of Employee when Lost focus from [Salesman code] or [Negotiation staff] text box 
        /// </summary>
        /// <param name="doEmployee"></param>
        /// <returns></returns>
        public ActionResult GetActiveEmployee_CTS220(CTS220_DOEmployee doEmployee)
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

                //5.1 Get employee
                listEmployee = masterHandler.GetActiveEmployee(doEmployee.EmpNo);

                //5.2 If can’t get employee data from database Then
                if (listEmployee.Count() == 0)
                {
                    string[] param = { doEmployee.EmpNo };
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0012, param);
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, null);
                    return Json(res);
                }

                //5.3 Show employee name on screen
                listEmpNo = employeeHandler.GetEmployeeNameByEmpNo(doEmployee.EmpNo);
                if (listEmpNo.Count() != 0)
                    doEmployee.EmpName = listEmpNo[0].EmployeeNameDisplay;

                return Json(doEmployee);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get data of Subcontractor
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public ActionResult GetSubcontractor_CTS220(string from)
        {
            ObjectResultData res = new ObjectResultData();

            IRentralContractHandler rentralContractHandler;
            List<dtTbt_RentalInstSubContractorListForView> listSubContractor;
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                if (from == "Load")
                {
                    rentralContractHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                    if (session.DOValidateBusiness.ProcessType == "Correct" || session.DOValidateBusiness.ProcessType == "Delete")
                        listSubContractor = rentralContractHandler.GetTbt_RentalInstSubContractorListForView(session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OCC);
                    //else
                    //    listSubContractor = rentralContractHandler.GetTbt_RentalInstSubContractorListForView(session.DSRentalContractPrevious.dtTbt_RentalSecurityBasic[0].ContractCode, session.DSRentalContractPrevious.dtTbt_RentalSecurityBasic[0].OCC);
                    else
                        listSubContractor = new List<dtTbt_RentalInstSubContractorListForView>();

                    session.ListSubContractor = listSubContractor;
                }
                else
                    res.ResultData = CommonUtil.ConvertToXml<dtTbt_RentalInstSubContractorListForView>(session.ListSubContractor, "Contract\\CTS220_Installation", CommonUtil.GRID_EMPTY_TYPE.INSERT);

                res.ResultData = CommonUtil.ConvertToXml<dtTbt_RentalInstSubContractorListForView>(session.ListSubContractor, "Contract\\CTS220_Installation", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate business of screen
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ObjectResultData ValidateRegisterConfirm_CTS220(CTS220_ValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                if (doValidateBusiness.ProcessType == "Correct")
                {
                    return ValidateForCorrect(doValidateBusiness);
                }

                if (doValidateBusiness.ProcessType == "Insert")
                {
                    return ValidateForInsert(doValidateBusiness);
                }

                if (doValidateBusiness.ProcessType == "Delete")
                {
                    return ValidateForDelete(doValidateBusiness);
                }
            }
            catch (Exception ex)
            {
                //res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Validate business of Correct process
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ObjectResultData ValidateForCorrect(CTS220_ValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            IMasterHandler masterHandler;
            List<tbm_Employee> listEmployee;

            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                if (doValidateBusiness.ChangeImplementDate > DateTime.Now)
                {
                    string[] param = { doValidateBusiness.ChangeImplementDate.Value.ToString("dd/MM/yyyy") };
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3126, param);
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3126, param, null);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP34,
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3126,
                                        new string[] { "lblChangeOperationDate" },
                                        new string[] { "ChangeImplementDate" });

                    return res;
                }

                if (session.DSRentalContractPrevious != null
                    && doValidateBusiness.ChangeImplementDate < session.DSRentalContractPrevious.dtTbt_RentalSecurityBasic[0].ChangeImplementDate)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3138);
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3138, null, null);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP34,
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3138,
                                        new string[] { "lblChangeOperationDate" },
                                        new string[] { "ChangeImplementDate" });
                    return res;
                }

                if (session.DSRentalContractNext != null
                    && doValidateBusiness.ChangeImplementDate > session.DSRentalContractNext.dtTbt_RentalSecurityBasic[0].ChangeImplementDate)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3139);
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3139, null, null);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP34,
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3139,
                                        new string[] { "lblChangeOperationDate" },
                                        new string[] { "ChangeImplementDate" });
                    return res;
                }




                if (doValidateBusiness.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE &&
                    doValidateBusiness.ReturnToOriginalFeeDate != null)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3197);
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3197, null, null);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP34,
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3197,
                                        new string[] { "lblReturnDateToOriginalFee" },
                                        new string[] { "ReturnToOriginalFeeDate" });
                    return res;
                }

                DateTime currentDate = DateTime.Now;
                currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);
                if (doValidateBusiness.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE &&
                    doValidateBusiness.ReturnToOriginalFeeDate != null && doValidateBusiness.ReturnToOriginalFeeDate < currentDate)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3141);
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3141, null, null);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP34,
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3141,
                                        new string[] { "lblReturnDateToOriginalFee" },
                                        new string[] { "ReturnToOriginalFeeDate" });
                    return res;
                }

                if (doValidateBusiness.ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_STOP &&
                    doValidateBusiness.ExpectedResumeDate != null)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3196, "");
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3196, null, null);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP34,
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3196,
                                        new string[] { "lblExpectedResumeServiceDate" },
                                        new string[] { "ExpectedResumeDate" });
                    return res;
                }

                if (doValidateBusiness.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_STOP &&
                    doValidateBusiness.ExpectedResumeDate != null && doValidateBusiness.ExpectedResumeDate < doValidateBusiness.ChangeImplementDate)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3140);
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3140, null, null);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP34,
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3140,
                                        new string[] { "lblExpectedResumeServiceDate" },
                                        new string[] { "ExpectedResumeDate" });
                    return res;
                }

                if (String.IsNullOrEmpty(doValidateBusiness.SalesmanEmpNo1) == false)
                {
                    listEmployee = masterHandler.GetActiveEmployee(doValidateBusiness.SalesmanEmpNo1);
                    if (listEmployee.Count() == 0)
                    {
                        string[] param = { doValidateBusiness.SalesmanEmpNo1 };
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.SalesmanEmpNo1);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, new string[] { "SaleManEmpNo1" });
                        return res;
                    }
                }

                if (String.IsNullOrEmpty(doValidateBusiness.SalesmanEmpNo2) == false)
                {
                    listEmployee = masterHandler.GetActiveEmployee(doValidateBusiness.SalesmanEmpNo2);
                    if (listEmployee.Count() == 0)
                    {
                        string[] param = { doValidateBusiness.SalesmanEmpNo2 };
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.SalesmanEmpNo2);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, new string[] { "SaleManEmpNo2" });
                        return res;
                    }
                }

                if (String.IsNullOrEmpty(doValidateBusiness.NegotiationStaffEmpNo1) == false)
                {
                    listEmployee = masterHandler.GetActiveEmployee(doValidateBusiness.NegotiationStaffEmpNo1);
                    if (listEmployee.Count() == 0)
                    {
                        string[] param = { doValidateBusiness.NegotiationStaffEmpNo1 };
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.NegotiationStaffEmpNo1);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, new string[] { "NegotiationStaffEmpNo1" });
                        return res;
                    }
                }

                if (String.IsNullOrEmpty(doValidateBusiness.NegotiationStaffEmpNo2) == false)
                {
                    listEmployee = masterHandler.GetActiveEmployee(doValidateBusiness.NegotiationStaffEmpNo2);
                    if (listEmployee.Count() == 0)
                    {
                        string[] param = { doValidateBusiness.NegotiationStaffEmpNo1 };
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.NegotiationStaffEmpNo2);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, new string[] { "NegotiationStaffEmpNo2" });
                        return res;
                    }
                }

                //Move to check at RegisterClick_CTS220
                //if (doValidateBusiness.ContractFee != session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OrderContractFee)
                //{
                //    if (session.DSRentalContract.dtTbt_RentalContractBasic[0].LastOCC == session.DSRentalContract.dtTbt_RentalSecurityBasic[0].OCC)
                //    {
                //        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3148);
                //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                //        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3148);
                //        return res;
                //    }
                //}

                if (doValidateBusiness.ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE)
                {
                    if (session.DSRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate != doValidateBusiness.ReturnToOriginalFeeDate)
                        session.DOValidateBusiness.IsSendNotifyEmail = true;
                    else
                        session.DOValidateBusiness.IsSendNotifyEmail = false;
                }

                #region Approve No.

                string[] approveLst = new[] 
                { 
                    doValidateBusiness.ApproveNo1,
                    doValidateBusiness.ApproveNo2
                };
                bool[] nullLst = new[] { false, false };
                int maxValue = -1;
                for (int idx = 0; idx < 2; idx++)
                {
                    if (CommonUtil.IsNullOrEmpty(approveLst[idx]))
                    {
                        nullLst[idx] = true;
                    }
                    else
                        maxValue = idx;
                }

                List<string> ctrlLst = new List<string>();
                if (maxValue > 0)
                {
                    for (int idx = maxValue; idx >= 0; idx--)
                    {
                        if (nullLst[idx])
                            ctrlLst.Insert(0, "ApproveNo" + (idx + 1).ToString());
                    }
                }
                if (ctrlLst.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3009,
                            null, ctrlLst.ToArray());
                    return res;
                }

                #endregion
                #region Negotiation staff.

                string[] staffLst = new[] 
                { 
                    doValidateBusiness.NegotiationStaffEmpNo1,
                    doValidateBusiness.NegotiationStaffEmpNo2
                };
                bool[] nnullLst = new[] { false, false };
                int nmaxValue = -1;
                for (int idx = 0; idx < 2; idx++)
                {
                    if (CommonUtil.IsNullOrEmpty(staffLst[idx]))
                    {
                        nnullLst[idx] = true;
                    }
                    else
                        nmaxValue = idx;
                }

                List<string> nctrlLst = new List<string>();
                if (nmaxValue > 0)
                {
                    for (int idx = nmaxValue; idx >= 0; idx--)
                    {
                        if (nnullLst[idx])
                            nctrlLst.Insert(0, "NegotiationStaffEmpNo" + (idx + 1).ToString());
                    }
                }
                if (nctrlLst.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3294,
                            null, nctrlLst.ToArray());
                    return res;
                }

                #endregion
            }
            catch (Exception ex)
            {
                //res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Validate business of Insert process
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ObjectResultData ValidateForInsert(CTS220_ValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            IMasterHandler masterHandler;
            List<tbm_Employee> listEmployee;

            CTS220_ScreenParameter session;

            try
            {
                session = CTS220_GetImportData();
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                if (doValidateBusiness.ChangeImplementDate < session.DSRentalContractPrevious.dtTbt_RentalSecurityBasic[0].ChangeImplementDate)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3138);
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3138, null, null);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP34,
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3138,
                                        new string[] { "lblChangeOperationDate" },
                                        new string[] { "ChangeImplementDate" });
                    return res;
                }

                if (doValidateBusiness.ChangeImplementDate > session.DSRentalContractNext.dtTbt_RentalSecurityBasic[0].ChangeImplementDate)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3139);
                    //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3139, null, null);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT,
                                        ScreenID.C_SCREEN_ID_CP34,
                                        MessageUtil.MODULE_CONTRACT,
                                        MessageUtil.MessageList.MSG3139,
                                        new string[] { "lblChangeOperationDate" },
                                        new string[] { "ChangeImplementDate" });
                    return res;
                }

                if (String.IsNullOrEmpty(doValidateBusiness.SalesmanEmpNo1) == false)
                {
                    listEmployee = masterHandler.GetActiveEmployee(doValidateBusiness.SalesmanEmpNo1);
                    if (listEmployee.Count() == 0)
                    {
                        string[] param = { doValidateBusiness.SalesmanEmpNo1 };
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.SalesmanEmpNo1);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, new string[] { "SaleManEmpNo1" });
                        return res;
                    }
                }

                if (String.IsNullOrEmpty(doValidateBusiness.SalesmanEmpNo2) == false)
                {
                    listEmployee = masterHandler.GetActiveEmployee(doValidateBusiness.SalesmanEmpNo2);
                    if (listEmployee.Count() == 0)
                    {
                        string[] param = { doValidateBusiness.SalesmanEmpNo2 };
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.SalesmanEmpNo2);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, new string[] { "SaleManEmpNo2" });
                        return res;
                    }
                }

                if (String.IsNullOrEmpty(doValidateBusiness.NegotiationStaffEmpNo1) == false)
                {
                    listEmployee = masterHandler.GetActiveEmployee(doValidateBusiness.NegotiationStaffEmpNo1);
                    if (listEmployee.Count() == 0)
                    {
                        string[] param = { doValidateBusiness.NegotiationStaffEmpNo1 };
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.NegotiationStaffEmpNo1);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, new string[] { "NegotiationStaffEmpNo1" });
                        return res;
                    }
                }

                if (String.IsNullOrEmpty(doValidateBusiness.NegotiationStaffEmpNo2) == false)
                {
                    listEmployee = masterHandler.GetActiveEmployee(doValidateBusiness.NegotiationStaffEmpNo2);
                    if (listEmployee.Count() == 0)
                    {
                        string[] param = { doValidateBusiness.NegotiationStaffEmpNo2 };
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, doValidateBusiness.NegotiationStaffEmpNo2);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, param, new string[] { "NegotiationStaffEmpNo2" });
                        return res;
                    }
                }

                #region Approve No.

                string[] approveLst = new[] 
                { 
                    doValidateBusiness.ApproveNo1,
                    doValidateBusiness.ApproveNo2
                };
                bool[] nullLst = new[] { false, false };
                int maxValue = -1;
                for (int idx = 0; idx < 2; idx++)
                {
                    if (CommonUtil.IsNullOrEmpty(approveLst[idx]))
                    {
                        nullLst[idx] = true;
                    }
                    else
                        maxValue = idx;
                }

                List<string> ctrlLst = new List<string>();
                if (maxValue > 0)
                {
                    for (int idx = maxValue; idx >= 0; idx--)
                    {
                        if (nullLst[idx])
                            ctrlLst.Insert(0, "ApproveNo" + (idx + 1).ToString());
                    }
                }
                if (ctrlLst.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3009,
                            null, ctrlLst.ToArray());
                    return res;
                }

                #endregion
                #region Negotiation staff.

                string[] staffLst = new[] 
                { 
                    doValidateBusiness.NegotiationStaffEmpNo1,
                    doValidateBusiness.NegotiationStaffEmpNo2
                };
                bool[] nnullLst = new[] { false, false };
                int nmaxValue = -1;
                for (int idx = 0; idx < 2; idx++)
                {
                    if (CommonUtil.IsNullOrEmpty(staffLst[idx]))
                    {
                        nnullLst[idx] = true;
                    }
                    else
                        nmaxValue = idx;
                }

                List<string> nctrlLst = new List<string>();
                if (nmaxValue > 0)
                {
                    for (int idx = nmaxValue; idx >= 0; idx--)
                    {
                        if (nnullLst[idx])
                            nctrlLst.Insert(0, "NegotiationStaffEmpNo" + (idx + 1).ToString());
                    }
                }
                if (nctrlLst.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3294,
                            null, nctrlLst.ToArray());
                    return res;
                }

                #endregion
            }
            catch (Exception ex)
            {
                //res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return res;
        }

        /// <summary>
        /// Validate business of Delete process
        /// </summary>
        /// <param name="doValidateBusiness"></param>
        /// <returns></returns>
        public ObjectResultData ValidateForDelete(CTS220_ValidateBusiness doValidateBusiness)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            IMasterHandler masterHandler;
            IRentralContractHandler rentralContract;
            dsRentalContractData dsRentalContract;

            CTS220_ScreenParameter session;

            string lastImplementOCC = "";
            string lastUnimplementedOCC = "";
            string installationStatusCode = "";

            try
            {
                session = CTS220_GetImportData();
                masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                rentralContract = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;

                string strContractCode = session.DSRentalContract.dtTbt_RentalContractBasic[0].ContractCode;

                lastImplementOCC = rentralContract.GetLastImplementedOCC(strContractCode);
                dsRentalContract = rentralContract.GetEntireContract(strContractCode, lastImplementOCC);

                if (dsRentalContract.dtTbt_RentalSecurityBasic == null)
                {
                    if (dsRentalContract.dtTbt_RentalSecurityBasic.Count() == 0)
                    {
                        //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG0106);
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0106);
                        return res;
                    }
                }

                if (dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_ALTERNATIVE_START ||
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_NEW_START)
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3134);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3134);
                    return res;
                }

                lastUnimplementedOCC = rentralContract.GetLastUnimplementedOCC(strContractCode);
                if (lastUnimplementedOCC != null && lastUnimplementedOCC != "")
                {
                    //res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3106);
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3106);
                    return res;
                }

                //installationStatusCode = installationInterface.GetInstallationStatus(strContractCode);
                //if (installationStatusCode != InstallationStatus.C_INSTALL_STATUS_NO_INSTALLATION)
                //{
                ////res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3135);
                //res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3135);
                //return Json(res);
                //}         

            }
            catch (Exception ex)
            {
                //res.ResultData = false;
                res.AddErrorMessage(ex);
            }

            return res;
        }

        #endregion

        #region Session

        /// <summary>
        /// Get import data from screen
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private CTS220_ScreenParameter CTS220_GetImportData(string key = null)
        {
            try
            {
                return GetScreenObject<CTS220_ScreenParameter>(key);
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
        private void CTS220_SetImportData(CTS220_ScreenParameter import, string key = null)
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

        //public ActionResult CTS220_ClearSession()
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
