//*********************************
// Create by: Fikree S.
// Create date: 13/Jan/2012
// Update date: 13/Jan/2012
//*********************************

using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Master.Models;

namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        /// <summary>
        /// Check suspend, authority and resume of MAS110
        /// </summary>
        /// <param name="screenParam"></param>
        /// <returns></returns>
        public ActionResult MAS110_Authority(MAS110_ScreenParameter screenParam)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                return InitialScreenEnvironment<MAS110_ScreenParameter>("MAS110", screenParam, res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Initial screen MAS110
        /// </summary>
        /// <returns></returns>
        [Initialize("MAS110")]
        public ActionResult MAS110()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ViewBag.HasPermissionAdd = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_ADD);
                ViewBag.HasPermissionEdit = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_EDIT);
                ViewBag.HasPermissionDelete = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_DEL);

                //Get misc type
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> doFlagDisplay = commonHandler.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_FLAG_DISPLAY,
                        ValueCode = "%"
                    }
                });
                ViewBag.FlagDisplayYes = doFlagDisplay.Where(d => d.ValueCode == FlagDisplay.C_FLAG_DISPLAY_YES).First().ValueDisplay;
                ViewBag.FlagDisplayNo = doFlagDisplay.Where(d => d.ValueCode == FlagDisplay.C_FLAG_DISPLAY_NO).First().ValueDisplay;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
            return View();
        }

        /// <summary>
        /// Initial grid
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS110_InitGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS110"));
        }

        /// <summary>
        /// Search and get sub contractor data
        /// </summary>
        /// <param name="SubcontractorCodeSearch"></param>
        /// <param name="CoCompanyCodeSearch"></param>
        /// <param name="InstallationTeamSearch"></param>
        /// <param name="SubcontractorNameSearch"></param>
        /// <returns></returns>
        public ActionResult MAS110_Search(string SubcontractorCodeSearch, string CoCompanyCodeSearch, string InstallationTeamSearch, string SubcontractorNameSearch)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                SubcontractorCodeSearch = (SubcontractorCodeSearch == "" ? null : SubcontractorCodeSearch);
                CoCompanyCodeSearch = (CoCompanyCodeSearch == "" ? null : CoCompanyCodeSearch);
                InstallationTeamSearch = (InstallationTeamSearch == "" ? null : InstallationTeamSearch);
                SubcontractorNameSearch = (SubcontractorNameSearch == "" ? null : SubcontractorNameSearch);

                if (SubcontractorCodeSearch == null && CoCompanyCodeSearch == null && InstallationTeamSearch == null && SubcontractorNameSearch == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                ISubcontractorMasterHandler hand = ServiceContainer.GetService<ISubcontractorMasterHandler>() as ISubcontractorMasterHandler;
                List<doSubcontractor> list = hand.GetSubcontractor(SubcontractorCodeSearch, CoCompanyCodeSearch, InstallationTeamSearch, SubcontractorNameSearch);

                List<doSubcontractor> tmplist = (from t in list
                                         where t.DeleteFlag != true
                                              select t).ToList<doSubcontractor>();

                string xml = CommonUtil.ConvertToXml<doSubcontractor>(tmplist, "Master\\MAS110", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Search and get sub contractor detail 
        /// </summary>
        /// <param name="SubcontractorCode"></param>
        /// <returns></returns>
        public ActionResult MAS110_SearchDetail(string SubcontractorCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ISubcontractorMasterHandler hand = ServiceContainer.GetService<ISubcontractorMasterHandler>() as ISubcontractorMasterHandler;
                List<doSubcontractor> list = hand.GetSubcontractorDetail(SubcontractorCode);

                doSubcontractor data = null;
                if (list.Count > 0)
                {
                    data = list[0];

                    MAS110_ScreenParameter MAS110Param = GetScreenObject<MAS110_ScreenParameter>();
                    MAS110Param.currentSubcontractor = data;

                }

                res.ResultData = data;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }


        /// <summary>
        /// Insert sub contractor
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult MAS110_Insert(MAS110_SubcontractorData data)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                ISubcontractorMasterHandler hand = ServiceContainer.GetService<ISubcontractorMasterHandler>() as ISubcontractorMasterHandler;
                // Check required field.
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                

                data.DeleteFlag = false;
                data.CreateBy = data.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                data.CreateDate = data.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                //================= Change DDS SubContractorCode generate by companycode + teamcode =========
                List<doSubcontractor> list = new List<doSubcontractor>();
                data.SubContractorCode = data.COCompanyCode + data.InstallationTeam;
                //======= check duplicate
                List<doSubcontractor> doSubcon = hand.GetSubcontractor(data.SubContractorCode, null, null, null);
                if (doSubcon != null && doSubcon.Count > 0)
                {
                    if (doSubcon[0].DeleteFlag == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1056);
                    }
                    else
                    {
                        res.ResultData = "ConfirmUpdate";
                        return Json(res);
                    }
                }
                else
                {
                    if (CommonUtil.IsNullOrEmpty(data.SubInstallationFlag))
                    {
                        data.SubInstallationFlag = false;
                    }
                    if (CommonUtil.IsNullOrEmpty(data.SubMaintenanceFlag))
                    {
                        data.SubMaintenanceFlag = false;
                    }
                    list = hand.InsertSubcontractor(data);
                }
                //===========================================================================================
                
                

                doSubcontractor result = null;
                if (list.Count > 0)
                {
                    result = list[0];
                }

                res.ResultData = result;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Update sub contractor
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult MAS110_Update(MAS110_SubcontractorData data)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                // Check required field.
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                MAS110_ScreenParameter screenParam = GetScreenObject<MAS110_ScreenParameter>();
                data.DeleteFlag = (data.DeleteFlag == null ? false : true);
                data.CreateBy = screenParam.currentSubcontractor.CreateBy;
                data.CreateDate = screenParam.currentSubcontractor.CreateDate.Value;
                data.UpdateBy = screenParam.currentSubcontractor.UpdateBy;
                data.UpdateDate = screenParam.currentSubcontractor.UpdateDate.Value;
                
                ISubcontractorMasterHandler hand = ServiceContainer.GetService<ISubcontractorMasterHandler>() as ISubcontractorMasterHandler;
                if (CommonUtil.IsNullOrEmpty(data.SubInstallationFlag))
                {
                    data.SubInstallationFlag = false;
                }
                if (CommonUtil.IsNullOrEmpty(data.SubMaintenanceFlag))
                {
                    data.SubMaintenanceFlag = false;
                }
                List<doSubcontractor> list = hand.UpdateSubcontractor(data);

                doSubcontractor result = null;
                if (list.Count > 0)
                {
                    result = list[0];
                }

                res.ResultData = result;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Update sub contractor in case duplicate
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult MAS110_UpdateCaseDuplicate(MAS110_SubcontractorData data)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (commonHandler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_ADD) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                      || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                ISubcontractorMasterHandler hand = ServiceContainer.GetService<ISubcontractorMasterHandler>() as ISubcontractorMasterHandler;
                // Check required field.
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }
                data.SubContractorCode = data.COCompanyCode + data.InstallationTeam;
                data.DeleteFlag = (data.DeleteFlag == null ? false : true);
                List<doSubcontractor> curSubcontractor = hand.GetSubcontractor(data.SubContractorCode, null, null, null);
                if (curSubcontractor.Count > 0)
                {
                    data.UpdateBy = curSubcontractor[0].UpdateBy;
                    data.UpdateDate = curSubcontractor[0].UpdateDate;
                }


                if (CommonUtil.IsNullOrEmpty(data.SubInstallationFlag))
                {
                    data.SubInstallationFlag = false;
                }
                if (CommonUtil.IsNullOrEmpty(data.SubMaintenanceFlag))
                {
                    data.SubMaintenanceFlag = false;
                }
                List<doSubcontractor> list = hand.UpdateSubcontractor(data);

                doSubcontractor result = null;
                if (list.Count > 0)
                {
                    result = list[0];
                }

                res.ResultData = result;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

    }
}

