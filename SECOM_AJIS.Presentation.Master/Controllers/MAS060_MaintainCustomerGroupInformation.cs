//*********************************
// Create by: 
// Create date: /Jun/2010
// Update date: /Jun/2010
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
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Master.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;



namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        private const string MAS060_Screen = "MAS060";

        #region Authority

        /// <summary>
        /// - Check user permission for screen MAS010.<br />
        /// - Check system suspending.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS060_Authority(MAS060_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_GROUP_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                    || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_GROUP_INFO, FunctionID.C_FUNC_ID_ADD) == true
                    || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_GROUP_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                    || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_GROUP_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    )
                    )
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //Check system suspending
                res = checkSystemSuspending();
                if (res.IsError)
                    return Json(res);
                // Do in view
                //param.HasAddPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_GROUP_INFO, FunctionID.C_FUNC_ID_ADD);
                //param.HasEditPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_GROUP_INFO, FunctionID.C_FUNC_ID_EDIT);
                //param.HasDeletePermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_GROUP_INFO, FunctionID.C_FUNC_ID_DEL);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<MAS060_ScreenParameter>(MAS060_Screen, param, res);
        }
        #endregion

        #region View

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize(MAS060_Screen)]
        public ActionResult MAS060()
        {
            MAS060_ScreenParameter MAS060Param = new MAS060_ScreenParameter();
            ViewBag.HasAddPermission = "";
            ViewBag.HasEditPermission = "";
            ViewBag.HasDeletePermission = "";
            ViewBag.AddMode = FunctionID.C_FUNC_ID_ADD;
            ViewBag.EditMode = FunctionID.C_FUNC_ID_EDIT;
            ViewBag.PageRow = CommonValue.ROWS_PER_PAGE_FOR_VIEWPAGE;

            try
            {
                MAS060Param = GetScreenObject<MAS060_ScreenParameter>();
                ViewBag.HasAddPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_GROUP_INFO, FunctionID.C_FUNC_ID_ADD);
                ViewBag.HasEditPermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_GROUP_INFO, FunctionID.C_FUNC_ID_EDIT);
                ViewBag.HasDeletePermission = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_CUST_GROUP_INFO, FunctionID.C_FUNC_ID_DEL);

            }
            catch 
            {
            }

            return View(MAS060_Screen);
        }
        #endregion

        /// <summary>
        /// Get config for Group List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult InitialGrid_MAS060()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS060", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Load employee name by employee code to show in Person in Charge.
        /// </summary>
        /// <param name="GroupEmpNo"></param>
        /// <returns></returns>
        public ActionResult MAS060_LoadGroupEmpName(string GroupEmpNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //5.1 Get employee
                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_Employee> tbtEmp = masterHandler.GetActiveEmployee(GroupEmpNo);

                //5.2 If can't get employee data from database
                if (tbtEmp.Count <= 0)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0095, new string[] { GroupEmpNo }, new string[] { "GroupEmpNo" });
                    return Json(res);
                }

                //5.3 Show employee name on screen
                res.ResultData = getEmployeeDisplayName(GroupEmpNo);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        private String getEmployeeDisplayName(string GroupEmpNo)
        {
            String empDisplayName = String.Empty;
            try
            {
                IEmployeeMasterHandler employeeHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<dtEmpNo> dtEmpNo = employeeHandler.GetEmployeeNameByEmpNo(GroupEmpNo);
                if (dtEmpNo.Count > 0)
                    empDisplayName = dtEmpNo[0].EmployeeNameDisplay;
            }
            catch (Exception)
            {
                throw;
            }
            return empDisplayName;
        }

        /// <summary>
        /// Get group name match to cond for auto complete textbox.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult GetGroupName(string cond)
        {
            try
            {
                IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<doGroupNameDataList> lst = hand.GetGroupNameDataList(cond);
                List<string> strList = new List<string>();

                foreach (var l in lst)
                {
                    strList.Add(l.GroupName);
                }

                return Json(strList.ToArray());
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Search customer group and transform to xml format before return to screen.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult MAS060_Search(MAS060_Search cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { cond }); //AtLeast1FieldNotNullOrEmptyAttribute
                if (res.IsError)
                    return Json(res);

                IGroupMasterHandler hand = ServiceContainer.GetService<IGroupMasterHandler>() as IGroupMasterHandler;
                List<doGroup> lst = hand.GetGroup(cond);

                MAS060_ScreenParameter MAS060Param = GetScreenObject<MAS060_ScreenParameter>();
                MAS060Param.SearchResult = lst;
                UpdateScreenObject(MAS060Param);

                res.ResultData = CommonUtil.ConvertToXml<doGroup>(lst, "Master\\MAS060", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Search customer group and return list to screen.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult MAS060_SearchByGroupCode(MAS060_Search cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { cond }); //AtLeast1FieldNotNullOrEmptyAttribute
                if (res.IsError)
                    return Json(res);

                IGroupMasterHandler hand = ServiceContainer.GetService<IGroupMasterHandler>() as IGroupMasterHandler;
                List<doGroup> lst = hand.GetGroup(cond);

                MAS060_ScreenParameter MAS060Param = GetScreenObject<MAS060_ScreenParameter>();
                MAS060Param.SearchResult = lst;
                UpdateScreenObject(MAS060Param);

                res.ResultData = lst;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Logical delete group.<br />
        /// - Check is used group.<br />
        /// - Set delete flag.<br />
        /// - Update database.
        /// </summary>
        /// <param name="GroupCode"></param>
        /// <returns></returns>
        public ActionResult MAS060_Delete(String GroupCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //Check System Suspending
                res = checkSystemSuspending();
                if (res.IsError)
                    return Json(res);

                IGroupMasterHandler hand = ServiceContainer.GetService<IGroupMasterHandler>() as IGroupMasterHandler;
                bool bResult = hand.IsUsedGroupData(GroupCode);
                if (bResult)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1009, new String[] { GroupCode });
                    return Json(res);
                }
                else
                {
                    MAS060_ScreenParameter MAS060Param = GetScreenObject<MAS060_ScreenParameter>();

                    var doGroupList = from g in MAS060Param.SearchResult
                                       where g.GroupCode == GroupCode
                                       select g;

                    foreach (var group in doGroupList)
                    {
                        //Update DeleteFlag
                        group.DeleteFlag = true;
                        hand.UpdateGroup(group);

                        //Remove data from session
                        MAS060Param.SearchResult.Remove(group);
                        UpdateScreenObject(MAS060Param);
                        break;
                    }

                    //when finish with out error
                    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0047);
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Add/Edit customer group.<br />
        /// - Validate field.<br />
        /// - Check exist active employee.<br />
        /// - Check duplicate group.<br />
        /// - Update database.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult MAS060_AddEdit(MAS060_AddEdit cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
            try
            {
                //Validate Required Field
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                //Already validate when load emp name
                //if (!CommonUtil.IsNullOrEmpty(cond.GroupEmpNo) && CommonUtil.IsNullOrEmpty(cond.GroupEmpName))
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0024);
                //    return Json(res);
                //}

                if (CommonUtil.IsNullOrEmpty(cond.GroupEmpNo) == false)
                {
                    IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                    if (handler.CheckExistActiveEmployee(cond.GroupEmpNo) == false)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON,
                                            MessageUtil.MessageList.MSG0095,
                                            new string[] { cond.GroupEmpNo },
                                            new string[] { "GroupEmpNo" });
                        return Json(res);
                    }
                }

                IGroupMasterHandler hand = ServiceContainer.GetService<IGroupMasterHandler>() as IGroupMasterHandler;
                bool bResult = hand.CheckDuplicateGroupData(cond.GroupNameEN, cond.GroupCode);
                if (bResult)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1010);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                else
                {
                    //Check System Suspending
                    res = checkSystemSuspending();
                    if (res.IsError)
                        return Json(res);

                    MAS060_ScreenParameter MAS060Param = GetScreenObject<MAS060_ScreenParameter>();
                    doGroup resultObj = new doGroup();
                    if (cond.CurrentMode == FunctionID.C_FUNC_ID_ADD)
                    {
                        string strGroupCode = hand.GenerateGroupCode();
                        if (!CommonUtil.IsNullOrEmpty(strGroupCode))
                        {
                            //set insert value
                            doGroup group = new doGroup();
                            group.GroupCode = strGroupCode;
                            group.GroupNameEN = cond.GroupNameEN;
                            group.GroupNameLC = cond.GroupNameLC;
                            group.Memo = cond.Memo;
                            group.GroupOfficeCode = cond.GroupOfficeCode;
                            group.GroupEmpNo = cond.GroupEmpNo;
                            group.GroupEmpName = cond.GroupEmpName;
                            group.DeleteFlag = cond.DeleteFlag;

                            //Insert data in db
                            List<tbm_Group> inserted = hand.InsertGroup(group);

                            //Set new UpdateDate
                            group.UpdateDate = inserted[0].UpdateDate;

                            //Add new data in session
                            if (MAS060Param.SearchResult == null)
                                MAS060Param.SearchResult = new List<doGroup>();
                            MAS060Param.SearchResult.Add(group);
                            UpdateScreenObject(MAS060Param);
                            resultObj.GroupCode = strGroupCode;
                        }
                    }
                    else if (cond.CurrentMode == FunctionID.C_FUNC_ID_EDIT)
                    {
                        var doGroupList = from g in MAS060Param.SearchResult
                                          where g.GroupCode == cond.GroupCode
                                          select g;

                        foreach (var group in doGroupList)
                        {
                            //set update value
                            group.GroupCode = cond.GroupCode;
                            group.GroupNameEN = cond.GroupNameEN;
                            group.GroupNameLC = cond.GroupNameLC;
                            group.Memo = cond.Memo;
                            group.GroupOfficeCode = cond.GroupOfficeCode;
                            group.GroupEmpNo = cond.GroupEmpNo;
                            group.GroupEmpName = cond.GroupEmpName;
                            group.DeleteFlag = cond.DeleteFlag;

                            if (CommonUtil.IsNullOrEmpty(group.UpdateDate))
                            {
                                res.AddErrorMessage(
                                    MessageUtil.MODULE_COMMON,
                                    MessageUtil.MessageList.MSG0007, 
                                    new string[] { "UpdateDate" });
                                return Json(res);                                            
                            }

                            //Update data in db
                            List<tbm_Group> updated = hand.UpdateGroup(group);

                            //Set new UpdateDate
                            group.UpdateDate = updated[0].UpdateDate;

                            //Save edit data in session
                            UpdateScreenObject(MAS060Param);
                            resultObj.GroupCode = cond.GroupCode;
                            break;
                        }
                    }
                    
                    //when finish with out error
                    Object[] result = new Object[2];
                    result[0] = resultObj;
                    result[1] = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);

                    res.ResultData = result;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        private ObjectResultData checkSystemSuspending()
        {
            ObjectResultData res = new ObjectResultData();

            ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bSuspendingStatus = commonHandler.IsSystemSuspending();

            if (bSuspendingStatus)
            {
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
            }

            return res;
        }

        #region Old Code
        //[HttpPost]
        //public ActionResult GetGroupName(string cond)
        //{
        //    //MAEntities ef = new MAEntities();

        //    //if (CommonUtil.GetCurrentLanguage() == CommonUtil.GetDefaultLanguage_1
        //    //    || CommonUtil.GetCurrentLanguage() == CommonUtil.GetDefaultLanguage_2)
        //    //{
        //    //    List<GetGroupNameDataList> lst = ef.GetGroupNameDataList(cond).ToList();
        //    //    return new XmlResult(lst);
        //    //}
        //    //else
        //    //{
        //    //    List<GetGroupNameDataList> lst = ef.GetGroupNameDataList(cond).ToList();
        //    //    return new XmlResult(lst);
        //    //}
           
        //    try
        //    {
        //        IAutoCompleteHandler hand = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
        //        List<doGroupNameDataList> lst = hand.GetGroupNameDataList(cond).ToList();
        //        List<string> strList = new List<string>();

        //        foreach (var l in lst)
        //        {
        //            strList.Add(l.GroupName);
        //        }

        //        return Json(strList.ToArray());
        //    }
        //    catch (Exception ex)
        //    {
        //        ObjectResultData res = new ObjectResultData();
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }
        //}
        //public ActionResult MAS060_Xml()
        //{


        //    try
        //    {
        //        IGroupMasterHandler hand = ServiceContainer.GetService<IGroupMasterHandler>() as IGroupMasterHandler;
        //        List<doGroup> lst = hand.GetGroup(Request["txtGroupCodeSearch"], Request["txtGroupName"]).ToList();

        //        string xml = CommonUtil.ConvertToXml<doGroup>(lst);
        //        return Json(xml);
        //    }
        //    catch (Exception ex)
        //    {

        //        ObjectResultData res = new ObjectResultData();
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }
        //}
        //public int MAS060_IsUsedGroup()
        //{
        //     IGroupMasterHandler hand = ServiceContainer.GetService<IGroupMasterHandler>() as IGroupMasterHandler;
        //    List<int?> list= hand.IsUsedGroup(Request["txtGroupCode"]);
        //    int ret=0;
        //    if(list.Count!=0)
        //        ret=list[0].Value;
        //    return ret;
        //}
        //public ActionResult MAS060_Insert(tbm_Group group)
        //{
        //    try
        //    {
               
        //        IGroupMasterHandler hand = ServiceContainer.GetService<IGroupMasterHandler>() as IGroupMasterHandler;
        //        string user = "test";//CommonUtil.dsTransData.dtUserData.EmpNo;
        //        DateTime dt = DateTime.Now;
        //        List<tbm_Group> result = hand.InsertGroup(null, group.GroupNameEN
        //            , group.GroupNameLC
        //            , group.Memo
        //            , group.GroupOfficeCode
        //            , group.GroupEmpNo, 0, dt, user, dt, user);

        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        ObjectResultData res = new ObjectResultData();
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }        
                         
        //}
        //public ActionResult MAS060_Update(tbm_Group group)
        //{
        //    try
        //    {
        //        IGroupMasterHandler hand = ServiceContainer.GetService<IGroupMasterHandler>() as IGroupMasterHandler;
        //        string user = "test";
        //        DateTime dt = DateTime.Now;
        //        List<tbm_Group> result = hand.UpdateGroup(group.GroupCode
        //            , group.GroupNameEN
        //            , group.GroupNameLC
        //            , group.Memo
        //            , group.GroupOfficeCode
        //            , group.GroupEmpNo
        //            , group.DeleteFlag
        //            , dt, user);
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {


        //        ObjectResultData res = new ObjectResultData();
        //        res.AddErrorMessage(ex);
        //        return Json(res);
        //    }
        //}
        #endregion
    }
}

