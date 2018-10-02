using System;
using System.Collections.Generic;
using System.IO;
using System.Transactions;
using System.Web;
using System.Linq;
//*********************************	
// Create by: Nattapong N.	
// Create date: 20/09/2011	
// Update date: 20/09/2011	
//*********************************	
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Contract.Model;
namespace SECOM_AJIS.Presentation.Contract.Controllers
{
    public partial class ContractController : BaseController
    {

        #region AuthorityAndInitial

        /// <summary>
        /// Authority screen CTS230
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult CTS230_Authority(CTS230_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler comhand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (comhand.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_PROJ_NEW, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<CTS230_ScreenParameter>("CTS230", param, res);
        }

        /// <summary>
        /// Initial screen CTS230
        /// </summary>
        /// <returns></returns>
        [Initialize("CTS230")]
        public ActionResult CTS230()
        {
            ViewBag.AttachKey = GetCurrentKey();
            ViewBag.Diff = ProjectOwnerType.C_PROJECT_OWNER_DIFF_CUST;
            ViewBag.Same = ProjectOwnerType.C_PROJECT_OWNER_SAME_CUST;
            return View();

        }

        /// <summary>
        /// Initial section upload in screen CTS230
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS230_Upload()
        {
            ViewBag.K = GetCurrentKey();
            return View();
        }
        #endregion


        #region Get Method
        /// <summary>
        /// Search instrument data by instrumentcode
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <returns></returns>
        public ActionResult CTS230_GetInstrumentDataForSearch(string InstrumentCode)
        {
            if (!CommonUtil.IsNullOrEmpty(InstrumentCode))
            {
                IInstrumentMasterHandler InstHand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                doInstrumentSearchCondition Cond = new doInstrumentSearchCondition();
                Cond.InstrumentCode = InstrumentCode;
                //====================== TRS update 17/07/2012 ============================
                //Cond.ExpansionType = new List<string>() { ExpansionType.C_EXPANSION_TYPE_PARENT };
                //Cond.InstrumentType = new List<string>() { InstrumentType.C_INST_TYPE_GENERAL };
                Cond.ExpansionType = null;
                Cond.InstrumentType = null;
                //=========================================================================
                List<doInstrumentData> dtNewInstrument = InstHand.GetInstrumentDataForSearch(Cond);

                if (dtNewInstrument.Count > 0)
                    return Json(dtNewInstrument[0]);
                else
                    return Json("");

            }
            return Json("");
        }

        /// <summary>
        /// Get active employee data
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <returns></returns>
        public ActionResult CTS230_getActiveEmployee(string EmpNo)
        {
            SupportStaff SuppStaff = new SupportStaff();
            SuppStaff = getStaff(EmpNo, false);
            return Json(SuppStaff.EmpFullName);

        }

        /// <summary>
        /// Get customer data from session
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS230_GetCustomer()
        {
            try
            {
                CTS230_ScreenParameter param = GetScreenObject<CTS230_ScreenParameter>();
                if (param != null)
                {
                    if (param.InitialData != null)
                        return Json(param.InitialData.doCustomer);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get staff data
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <returns></returns>
        public ActionResult CTS230_getSupportStaff(string EmpNo)
        {
            return Json(getStaff(EmpNo, true));
        }
        private SupportStaff getStaff(string EmpNo, bool IsNeedMainBelonging)
        {
            IEmployeeMasterHandler EmpHand = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            List<tbm_Employee> lstEmp = new List<tbm_Employee>();
            List<doBelongingData> belonging = new List<doBelongingData>();
            SupportStaff SupStff = new SupportStaff();
            SupStff.EmpFullName = "";
            lstEmp = EmpHand.GetActiveEmployee(EmpNo);
            if (lstEmp.Count > 0)
            {
                if (IsNeedMainBelonging)
                {
                    belonging = EmpHand.GetMainBelongingByEmpNo(EmpNo);
                    if (belonging.Count > 0)
                        SupStff.BelongingOfficeDepart = belonging[0].BelongingOfficeDepart;
                }
                SupStff.EmpFullName = lstEmp[0].EmpFullName;

                return (SupStff);
            }

            return (SupStff);
        }
        #endregion

        public ActionResult CTS230_SupportStaffCheck(string EmpNo)
        {
            IEmployeeMasterHandler EmpHand = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            List<tbm_Employee> lstEmp = new List<tbm_Employee>();

            lstEmp = EmpHand.GetActiveEmployee(EmpNo);
            if (lstEmp.Count > 0)
            {
                List<doBelongingData> belonging = EmpHand.GetMainBelongingByEmpNo(EmpNo);
            }
            else
            {
                return Json("");
            }

            return Json("");
        }

        /// <summary>
        /// Retrieve customer data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult CTS230_RetrieveCustomer(CTS230RetrieveCustomer cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                #region Validate Data
                if (CommonUtil.IsNullOrEmpty(cond.CustCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1039, null, new string[] { "CPCustCodeShort" });
                    return Json(res);
                }
                if (false == ModelState.IsValid)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }


                #endregion
                #region Get Data

                doCustomer custDo = null;
                CommonUtil cmm = new CommonUtil();
                ICustomerMasterHandler handler = ServiceContainer.GetService<ICustomerMasterHandler>() as ICustomerMasterHandler;

                string CustCodeLong = cmm.ConvertCustCode(cond.CustCode, CommonUtil.CONVERT_TYPE.TO_LONG);
                List<doCustomer> lst = handler.GetCustomerByLanguage(CustCodeLong);
                if (lst.Count > 0)
                    custDo = lst[0];

                if (custDo != null)
                {
                    res.ResultData = custDo;
                }
                else
                {
                    MessageUtil.MessageList msgCode = MessageUtil.MessageList.MSG0068;
                    if (cond.CustType == 2)
                        msgCode = MessageUtil.MessageList.MSG0078;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, msgCode, null, new string[] { "CustCode" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                }

                #endregion
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Set customer data to session
        /// </summary>
        /// <param name="cust"></param>
        /// <returns></returns>
        public ActionResult CTS230_SetDoCustomer(doCustomer cust)
        {
            try
            {
                CTS230_ScreenParameter param = GetScreenObject<CTS230_ScreenParameter>();
                if (param == null)
                    param = new CTS230_ScreenParameter();
                if (param.InitialData == null)
                    param.InitialData = new CTS230_InitialData();
                param.InitialData.doCustomer = cust;

                UpdateScreenObject(param);
                return Json(true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public ActionResult CTS230_RemoveNotCompleteAttachFile()
        //{
        //    //ObjectResultData res = new ObjectResultData();
        //    //try
        //    //{
        //    //    ICommonHandler handCom = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //    //    handCom.RemoveAttachFile(AttachmentModule.Project, null, Session.SessionID, false);
        //    //    res.ResultData = true;
        //    //    return Json(res);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    res.AddErrorMessage(ex);
        //    //    return Json(res);
        //    //}
        //    return View();
        //}

        //public ActionResult CTS230_AttachFile(HttpPostedFileBase uploadedFile, string DocName, string action, string delID)
        //{
        //    string TempRelatedID = Session.SessionID;
        //    MessageModel MsgModel;
        //    ObjectResultData res = new ObjectResultData();
        //    ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //    List<dtAttachFileNameID> lstAttachedName = handler.GetAttachFileName(TempRelatedID, null, false);

        //    if (action == "delete" && delID != "")
        //    {
        //        ICommonHandler comhand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        comhand.RemoveAttachFile(AttachmentModule.Project, Convert.ToInt32(delID), TempRelatedID, false);
        //        List<dtAttachFileNameID> list = comhand.GetAttachFileName(TempRelatedID, null, false);
        //        ViewBag.AttachFileList = list;
        //        return View("CTS230_Upload");
        //    }

        //    if (lstAttachedName.Count >= AttachDocumentCondition.C_ATTACH_DOCUMENT_MAXIMUM_NUMBER && action == "upload")
        //    {
        //        MsgModel = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0076);
        //        ViewBag.AttachFileList = lstAttachedName;
        //        ViewBag.Message = MsgModel.Message;
        //        ViewBag.MsgCode = "MSG0076";
        //        return View("CTS230_Upload");
        //    }
        //    else
        //    {
        //        bool IsFileNameExist = false;
        //        foreach (dtAttachFileNameID i in lstAttachedName)
        //        {
        //            if (i.FileName.ToUpper() == DocName.ToUpper())
        //            {
        //                lstAttachedName = handler.GetAttachFileName(TempRelatedID, null, false);
        //                IsFileNameExist = true;
        //                break;
        //            }
        //        }
        //        if (IsFileNameExist)
        //        {
        //            MsgModel = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115);
        //            lstAttachedName = handler.GetAttachFileName(TempRelatedID, null, false);
        //            ViewBag.AttachFileList = lstAttachedName;
        //            ViewBag.Message = MsgModel.Message;
        //            ViewBag.MsgCode = "MSG0115";
        //            return View("CTS230_Upload");
        //        }
        //    }
        //    if (uploadedFile != null)
        //    {
        //        int fileSize = uploadedFile.ContentLength;
        //        if (uploadedFile.ContentLength > 0)
        //        {
        //            byte[] data;
        //            using (BinaryReader reader = new BinaryReader(uploadedFile.InputStream))
        //            {
        //                data = reader.ReadBytes(uploadedFile.ContentLength);
        //            }
        //            string fileType = Path.GetExtension(uploadedFile.FileName);
        //            List<tbt_AttachFile> attach = handler.InsertAttachFile(TempRelatedID,
        //                                            DocName,
        //                                              fileType,
        //                                              fileSize,
        //                                              data,
        //                                              false,
        //                                              DateTime.Now,
        //                                              CommonUtil.dsTransData.dtUserData.EmpFullName,
        //                                              DateTime.Now,
        //                                              CommonUtil.dsTransData.dtUserData.EmpFullName);
        //        }
        //    }
        //    lstAttachedName = handler.GetAttachFileName(TempRelatedID, null, false);
        //    ViewBag.AttachFileList = lstAttachedName;
        //    return View("CTS230_Upload");
        //}

        #region CheckBeforeAdd

        /// <summary>
        /// Validate before add support staff
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS230_ChackBeforeAddSupportStaff(CTS230_SupportStaff Cond)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            if (ModelState.IsValid == false)
            {
                ValidatorUtil.BuildErrorMessage(res, this, null);
                if (res.IsError)
                    return Json(res);
            }
            SupportStaff staff = getStaff(Cond.StaffCode, true);
            if (staff != null && CommonUtil.IsNullOrEmpty(staff.EmpFullName.Trim()))
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { Cond.StaffCode }, new string[] { "SupportStaffCode" });
                return Json(res);
            }
            bool IsStaff = false;
            if (!CommonUtil.IsNullOrEmpty(Cond.lstStaffCode))
            {
                for (int i = 0; i < Cond.lstStaffCode.Length; i++)
                {
                    if ((Cond.lstStaffCode[i].ToUpper() == Cond.StaffCode.ToUpper()))
                    {
                        IsStaff = true;
                        break;
                    }
                }
            }
            if (IsStaff)
            {
                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3188, new string[] { Cond.StaffCode }, new string[] { "SupportStaffCode" });
                return Json(res);
            }
            return Json(true);

        }

        /// <summary>
        /// Validate data before add project related company/person information
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS230_CheckBeforeAddProjRelate(CTS230_ProjectRelateCompanyPerson Cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            if (ModelState.IsValid == false)
            {
                ValidatorUtil.BuildErrorMessage(res, this, null);
                if (res.IsError)
                    return Json(res);
            }

            bool IsCompNameExist = false;
            if (!CommonUtil.IsNullOrEmpty(Cond.lstCompanyName))
            {
                for (int i = 0; i < Cond.lstCompanyName.Length; i++)
                {
                    if ((Cond.lstCompanyName[i].ToUpper() == Cond.CompanyName.ToUpper()) && Cond.row_id != Cond.lstCompanyName_rowID[i])
                    {
                        IsCompNameExist = true;
                        break;
                    }
                }
            }
            if (IsCompNameExist)
            {
                res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3186, new string[] { Cond.CompanyName }, new string[] { "Company Name" });
                return Json(res);
            }
            return Json(true);
        }

        /// <summary>
        /// Validate data before add instrument 
        /// </summary>
        /// <param name="InstData"></param>
        /// <returns></returns>
        public ActionResult CTS230_CheckBeforeAddInstrument(CTS230_AddInstrumentData InstData)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            if (ModelState.IsValid == false)
            {
                ValidatorUtil.BuildErrorMessage(res, this, null);
                if (res.IsError)
                    return Json(res);
            }

            if ((!CommonUtil.IsNullOrEmpty(InstData.InstrumentCode)) && CommonUtil.IsNullOrEmpty(InstData.dtNewInstrument))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0082, new string[] { InstData.InstrumentCode }, new string[] { "InstrumentCode" });
                return Json(res);
            }
            if (CommonUtil.IsNullOrEmpty(InstData.InstrumentQty) || InstData.InstrumentQty <= 0)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0084, null, new string[] { "InstrumentQty" });
                return Json(res);
            }

            //======================= TRS Update 16/07/2012 ============================
            if (InstData.dtNewInstrument.InstrumentTypeCode != InstrumentType.C_INST_TYPE_GENERAL)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0014,  new string[] { InstData.InstrumentCode }, new string[] { "InstrumentCode" });
                return Json(res);
            }
            if (InstData.dtNewInstrument.ExpansionTypeCode != ExpansionType.C_EXPANSION_TYPE_PARENT)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0015,  new string[] { InstData.InstrumentCode }, new string[] { "InstrumentCode" });
                return Json(res);
            }
            if (InstData.dtNewInstrument.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0086,  new string[] { InstData.InstrumentCode }, new string[] { "InstrumentCode" });
                return Json(res);
            }
            //==========================================================================

            bool IsInstrExist = false;
            if (!CommonUtil.IsNullOrEmpty(InstData.lstInstrumentCode))
            {
                for (int i = 0; i < InstData.lstInstrumentCode.Length; i++)
                {
                    if (InstData.lstInstrumentCode[i].ToUpper() == InstData.InstrumentCode.ToUpper())
                    {
                        IsInstrExist = true;
                        break;
                    }
                }
            }

            if (IsInstrExist)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0083, new string[] { InstData.InstrumentCode }, new string[] { "InstrumentCode" });
                return Json(res);
            }

            return Json(true);

        }

        /// <summary>
        /// Validate data before add system product
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS230_CheckBeforeAddSystemProduct(CTS230_SystemProduct Cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;


            try
            {      //Valid Cond

                ValidatorUtil.BuildErrorMessage(res, this, null);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                bool IsSysProdExists = false;
                if (!CommonUtil.IsNullOrEmpty(Cond.lstSystemProductName))
                {
                    for (int i = 0; i < Cond.lstSystemProductName.Length; i++)
                    {
                        if (Cond.SystemProductName.ToUpper() == Cond.lstSystemProductName[i].ToUpper())
                        {
                            IsSysProdExists = true;
                            break;
                        }
                    }
                }

                if (IsSysProdExists)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3187, new string[] { Cond.SystemProductName });
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }


            return Json(true);
        }
        //public bool CTS230_CheckBeforeAddAttachFile(CTS230_AttachFile Cond)
        //{

        //    ObjectResultData res = new ObjectResultData();
        //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

        //    if (ModelState.IsValid == false)
        //    {
        //        ValidatorUtil.BuildErrorMessage(res, this, null);
        //        //  if (res.IsError)
        //        //return Json(res);
        //    }




        //    return true;
        //}
        #endregion

        /// <summary>
        /// Register new project data
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CTS230_RegisterNewProject(doRegisterProjectData Cond)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            ICommonHandler comhand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            if (comhand.IsSystemSuspending())
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                return Json(res);
            }

            if (!CheckUserPermission(ScreenID.C_SCREEN_ID_PROJ_NEW, FunctionID.C_FUNC_ID_OPERATE))
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                return Json(res);
            }

            CTS230_ProjectPurchaseCustomer Customer = CommonUtil.CloneObject<tbt_ProjectPurchaserCustomer, CTS230_ProjectPurchaseCustomer>(Cond.doTbt_ProjectPurchaserCustomer);
            tbt_Project_CTS230 Project = CommonUtil.CloneObject<tbt_Project_CTS230, tbt_Project_CTS230>(Cond.doTbt_Project);
            ValidatorUtil.BuildErrorMessage(res, new object[] { Project, Customer }, null, false);
            if (res.IsError)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                return Json(res);
            }
            CTS230_doValidateEmpNo EmpError = CTS230_ValidateEmployee(Cond.doTbt_Project);
            if (EmpError.isError)
            {

                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { EmpError.EmpNo }, new string[] { EmpError.controls });
                return Json(res);
            }

            #region Line-up type

            if (Cond.doTbt_ProjectExpectedInstrumentDetail != null)
            {
                foreach (tbt_ProjectExpectedInstrumentDetails inst in Cond.doTbt_ProjectExpectedInstrumentDetail)
                {
                    if (inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_STOP_SALE
                        || inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_ONE_TIME
                        || inst.LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_TEMPORARY)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3296);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG_LIST;
                        break;
                    }
                }
            }

            #endregion

            CTS230_ScreenParameter param = GetScreenObject<CTS230_ScreenParameter>();
            if (param != null)
                if (param.InitialData != null)
                {
                    Cond.doTbt_ProjectPurchaserCustomer = param.InitialData.doProjectPurchaserData;
                }
            param.InitialData.doRegisterData = new doRegisterProjectData();
            param.InitialData.doRegisterData = Cond;

            UpdateScreenObject(param);
            res.ResultData = true;
            return Json(res);
        }


        /// <summary>
        /// Clear session screen CTS230
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearScreenParam()
        {
            CTS230_ScreenParameter param = GetScreenObject<CTS230_ScreenParameter>();
            param.InitialData = null;
            return Json(true);
        }

        /// <summary>
        /// Valid before register new project data
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS230_ConfirmRegisterProject()
        {

            ObjectResultData res = new ObjectResultData();
            try
            {
                CTS230_ScreenParameter param = GetScreenObject<CTS230_ScreenParameter>();       //Check Suspend
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_SCREEN_ID_PROJ_NEW, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0063);
                    return Json(res);
                }


                string ProjectCode = RegisterData(param.InitialData.doRegisterData);
                res.ResultData = ProjectCode;
                return Json(res);

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Register new project data
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public string RegisterData(doRegisterProjectData Cond)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    IProjectHandler hand = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                    string ProjectCode = hand.GenerateProjectCode();

                    #region  prepare_do
                    Cond.doTbt_Project.ProjectCode = ProjectCode;
                    Cond.doTbt_Project.ProjectStatus = ProjectStatus.C_PROJECT_STATUS_PROCESSING;
                    if (Cond.doTbt_ProjectExpectedInstrumentDetail != null)
                        foreach (tbt_ProjectExpectedInstrumentDetails i in Cond.doTbt_ProjectExpectedInstrumentDetail)
                        {
                            i.ProjectCode = ProjectCode;
                        }
                    if (Cond.doTbt_ProjectOtherRalatedCompany != null)
                        foreach (tbt_ProjectOtherRalatedCompany i in Cond.doTbt_ProjectOtherRalatedCompany)
                        {
                            i.ProjectCode = ProjectCode;
                        }

                    //============================================================================================================================     
                    if (Cond.doTbt_ProjectPurchaserCustomer != null)
                    {

                        if (!CommonUtil.IsNullOrEmpty(Cond.doTbt_ProjectPurchaserCustomer.CustCode))
                        {
                            string CustCode = Cond.doTbt_ProjectPurchaserCustomer.CustCode;
                            Cond.doTbt_ProjectPurchaserCustomer = null;
                            Cond.doTbt_ProjectPurchaserCustomer = new tbt_ProjectPurchaserCustomer();
                            Cond.doTbt_ProjectPurchaserCustomer.CustCode = CustCode;
                        }
                        Cond.doTbt_ProjectPurchaserCustomer.ProjectCode = ProjectCode;
                    }

                    if (Cond.doTbt_ProjectSupportStaffDetails != null)
                        foreach (tbt_ProjectSupportStaffDetails i in Cond.doTbt_ProjectSupportStaffDetails)
                        {
                            i.ProjectCode = ProjectCode;
                        }
                    if (Cond.doTbt_ProjectSystemDetails != null)
                        foreach (tbt_ProjectSystemDetails i in Cond.doTbt_ProjectSystemDetails)
                        {
                            i.ProjectCode = ProjectCode;
                        }
                    #endregion
                   
                    if (Cond.doTbt_Project.OverallBudgetAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        Cond.doTbt_Project.OverallBudgetAmountUsd = Cond.doTbt_Project.OverallBudgetAmount;
                        Cond.doTbt_Project.OverallBudgetAmount = null;                        
                    }
                    else
                    {
                        Cond.doTbt_Project.OverallBudgetAmountUsd = null;
                    }

                    if (Cond.doTbt_Project.ReceivedBudgetAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    {
                        Cond.doTbt_Project.ReceivedBudgetAmountUsd = Cond.doTbt_Project.ReceivedBudgetAmount;
                        Cond.doTbt_Project.ReceivedBudgetAmount = null;
                    }
                    else
                    {
                        Cond.doTbt_Project.ReceivedBudgetAmountUsd = null;
                    }

                    hand.CreateProjectData(Cond);

                    ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    //comHand.UpdateFlagAttachFile(AttachmentModule.Project, Session.SessionID, ProjectCode);
                    //================= Update Attach ======================
                    string temp = Path.Combine(comHand.GetTemporaryAttachFilePath(), GetCurrentKey());
                    if (Directory.Exists(temp))
                    {
                        chandler.UpdateFlagAttachFile(AttachmentModule.Project, GetCurrentKey(), ProjectCode);
                    }
                    //======================================================
                    scope.Complete();
                    return ProjectCode;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        /// <summary>
        /// Validate employee data
        /// </summary>
        /// <param name="Project"></param>
        /// <returns></returns>
        public CTS230_doValidateEmpNo CTS230_ValidateEmployee(tbt_Project_CTS230 Project)
        {
            ObjectResultData res = new ObjectResultData();

            List<tbm_Employee> dtEmp = new List<tbm_Employee>();
            if (!CommonUtil.IsNullOrEmpty(Project.HeadSalesmanEmpNo))
            {
                tbm_Employee tmp = new tbm_Employee();
                tmp.EmpNo = Project.HeadSalesmanEmpNo;
                dtEmp.Add(tmp);
            }
            if (!CommonUtil.IsNullOrEmpty(Project.ProjectManagerEmpNo))
            {
                tbm_Employee tmp = new tbm_Employee();
                tmp.EmpNo = Project.ProjectManagerEmpNo;
                dtEmp.Add(tmp);
            }
            if (!CommonUtil.IsNullOrEmpty(Project.ProjectSubManagerEmpNo))
            {
                tbm_Employee tmp = new tbm_Employee();
                tmp.EmpNo = Project.ProjectSubManagerEmpNo;
                dtEmp.Add(tmp);
            }
            if (!CommonUtil.IsNullOrEmpty(Project.SecurityPlanningChiefEmpNo))
            {
                tbm_Employee tmp = new tbm_Employee();
                tmp.EmpNo = Project.SecurityPlanningChiefEmpNo;
                dtEmp.Add(tmp);
            }
            if (!CommonUtil.IsNullOrEmpty(Project.InstallationChiefEmpNo))
            {
                tbm_Employee tmp = new tbm_Employee();
                tmp.EmpNo = Project.InstallationChiefEmpNo;
                dtEmp.Add(tmp);
            }
            CTS230_doValidateEmpNo EmpError = new CTS230_doValidateEmpNo();
            IEmployeeMasterHandler EmpH = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            List<doActiveEmployeeList> dtTbm_Employee = EmpH.GetActiveEmployeeList(dtEmp);
            string EmpNo;
            string Controls;

            string Prefix = "sysin";
            if (dtTbm_Employee.Count != dtEmp.Count)
            {
                if (!CommonUtil.IsNullOrEmpty(Project.HeadSalesmanEmpNo) &&
                    !dtTbm_Employee.Any(a => a.EmpNo == Project.HeadSalesmanEmpNo))
                {
                    EmpNo = Project.HeadSalesmanEmpNo;
                    Controls = Prefix + "HeadSalesmanEmpNo";
                    EmpError.EmpNo = EmpNo;
                    EmpError.controls = Controls;
                    return EmpError;
                }
                if (!CommonUtil.IsNullOrEmpty(Project.ProjectManagerEmpNo) &&
                    !dtTbm_Employee.Any(a => a.EmpNo == Project.ProjectManagerEmpNo))
                {
                    EmpNo = Project.ProjectManagerEmpNo;
                    Controls = Prefix + "ProjectManagerEmpNo";
                    EmpError.EmpNo = EmpNo;
                    EmpError.controls = Controls;
                    return EmpError;
                }
                if (!CommonUtil.IsNullOrEmpty(Project.ProjectSubManagerEmpNo) &&
                    !dtTbm_Employee.Any(a => a.EmpNo == Project.ProjectSubManagerEmpNo))
                {
                    EmpNo = Project.ProjectSubManagerEmpNo;
                    Controls = Prefix + "ProjectSubManagerEmpNo";
                    EmpError.EmpNo = EmpNo;
                    EmpError.controls = Controls;
                    return EmpError;
                }
                if (!CommonUtil.IsNullOrEmpty(Project.SecurityPlanningChiefEmpNo) &&
                    !dtTbm_Employee.Any(a => a.EmpNo == Project.SecurityPlanningChiefEmpNo))
                {
                    EmpNo = Project.SecurityPlanningChiefEmpNo;
                    Controls = Prefix + "SecurityPlanningChiefEmpNo";
                    EmpError.EmpNo = EmpNo;
                    EmpError.controls = Controls;
                    return EmpError;
                }
                if (!CommonUtil.IsNullOrEmpty(Project.InstallationChiefEmpNo) &&
                    !dtTbm_Employee.Any(a => a.EmpNo == Project.InstallationChiefEmpNo))
                {
                    EmpNo = Project.InstallationChiefEmpNo;
                    Controls = Prefix + "InstallationChiefEmpNo";
                    EmpError.EmpNo = EmpNo;
                    EmpError.controls = Controls;
                    return EmpError;
                }
            }
            return new CTS230_doValidateEmpNo();
        }



        #region InitGrid

        /// <summary>
        /// Initial grid other project schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS230_Other()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS230_OtherProject", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Initial grid support staff schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS230_SupportStaff()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS230_SupportStaff", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Initial grid expect instrument schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS230_ExpectInstrumen()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS230_ExpectInstrumen", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Initial grid attach document schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS230_AttachedDoc()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS230_AttachedDoc", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Initial grid system product schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS230_SystemProduct()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "contract\\CTS230_SystemProduct", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }
        #endregion

        /// <summary>
        /// Attach file
        /// </summary>
        /// <param name="fileSelect"></param>
        /// <param name="DocumentName"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public ActionResult CTS230_AttachFile(HttpPostedFileBase fileSelect, string DocumentName,string k)
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            MessageModel outmsg = null;

            try
            {
                byte[] fileData;
                if (fileSelect == null)
                {
                    outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0050, null);
                }
                else
                {
                    using (BinaryReader reader = new BinaryReader(fileSelect.InputStream))
                    {
                        var fList = commonhandler.GetAttachFileForGridView(GetCurrentKey());

                        var filterDupItem = from a in fList where a.FileName.ToUpper().Equals(DocumentName.ToUpper() + Path.GetExtension(fileSelect.FileName).ToUpper()) select a;

                        if (filterDupItem.Count() > 0)
                        {
                            // Docname duplicate
                            outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0115, new string[] { DocumentName });
                        }                        
                        else if (DocumentName == null || DocumentName == "")
                        {
                            string nparam = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_CONTRACT, "CTS300", "lblDocumentName");
                            outmsg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { nparam });
                        }
                        else
                        {
                            fileData = reader.ReadBytes(fileSelect.ContentLength);

                            if (commonhandler.CanAttachFile(DocumentName, fileData.Length, Path.GetExtension(fileSelect.FileName), GetCurrentKey(), GetCurrentKey()))
                            {
                                DateTime currDate = DateTime.Now;
                                commonhandler.InsertAttachFile(GetCurrentKey()
                                , DocumentName
                                , Path.GetExtension(fileSelect.FileName)
                                , fileData.Length
                                , fileData
                                , false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                outmsg = new MessageModel();
                outmsg.Message = ((SECOM_AJIS.Common.Models.ApplicationErrorException)(ex)).ErrorResult.Message.Message;
                outmsg.Code = CommonValue.SYSTEM_MESSAGE_CODE;
            }

            if (outmsg != null)
            {
                ViewBag.Message = outmsg.Message;
                ViewBag.MsgCode = outmsg.Code;
            }
            ViewBag.K = k;

            return View("CTS230_Upload");
        }

        /// <summary>
        /// Initial grid attach schema
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS230_IntialGridAttachedDocList()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Installation\\ISS010_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Remove attach file
        /// </summary>
        /// <param name="AttachID"></param>
        /// <returns></returns>
        public ActionResult CTS230_RemoveAttach(string AttachID)
        {
            ObjectResultData res = new ObjectResultData();
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            try
            {
                int _attachID = int.Parse(AttachID);

                commonhandler.DeleteAttachFileByID(_attachID, GetCurrentKey());

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Load attach for show in grid attach
        /// </summary>
        /// <returns></returns>
        public ActionResult CTS230_LoadGridAttachedDocList()
        {
            ICommonHandler commonhandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ObjectResultData res = new ObjectResultData();

            try
            {
                List<dtAttachFileForGridView> lstAttachedName = commonhandler.GetAttachFileForGridView(GetCurrentKey());
                res.ResultData = CommonUtil.ConvertToXml<dtAttachFileForGridView>(lstAttachedName, "Installation\\ISS010_AttachedDocList", CommonUtil.GRID_EMPTY_TYPE.INSERT);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
    }
}



