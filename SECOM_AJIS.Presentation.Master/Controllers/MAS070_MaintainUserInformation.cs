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
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using SECOM_AJIS.Presentation.Master.Models;

using System.Transactions;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master.Handlers;

namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {
        private const string MAS070_Screen = "MAS070";
        private const int C_EMAIL_LENGTH = 50;
        private string emailSuffix = null;

        /// <summary>
        /// - Check user permission for screen MAS070.<br />
        /// - Check system suspending.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS070_Authority(MAS070_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                    || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_ADD) == true
                    || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                    || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_DEL) == true
                    ))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //Check system suspending
                res = checkSystemSuspending();
                if (res.IsError)
                {
                    return Json(res);
                }
                // Do in View
                //param.hasPermissionAdd = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_ADD);
                //param.hasPermissionEdit = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_EDIT);
                //param.hasPermissionDelete = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_DEL);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<MAS070_ScreenParameter>(MAS070_Screen, param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize(MAS070_Screen)]
        public ActionResult MAS070()
        {
            MAS070_ScreenParameter MAS070Param = new MAS070_ScreenParameter();
            ViewBag.HasPermissionAdd = "";
            ViewBag.HasPermissionEdit = "";
            ViewBag.HasPermissionDelete = "";
            try
            {
                MAS070Param = GetScreenObject<MAS070_ScreenParameter>();
                ViewBag.HasPermissionAdd = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_ADD);
                ViewBag.HasPermissionEdit = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_EDIT);
                ViewBag.HasPermissionDelete = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_USER_INFO, FunctionID.C_FUNC_ID_DEL);
            }
            catch
            {
            }


            ICommonHandler cHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            emailSuffix = cHand.GetSystemStatusValue(ConfigName.C_EMAIL_SUFFIX);
            ViewBag.EmailSuffix = emailSuffix;
            ViewBag.EmailLength = C_EMAIL_LENGTH - emailSuffix.Length;

            IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            List<string> positionCode = handler.GetPositionCodeAtMaxPositionLevel();
            if (positionCode != null && positionCode.Count != 0)
            {
                ViewBag.DefaultPositionCode = positionCode[0].Trim();
            }

            return View();
        }

        /// <summary>
        /// Get config for employee list table.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS070_InitGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS070_employee"));
        }

        /// <summary>
        /// Get config for belonging list table.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS070_InitBelGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS070_belonging"));
        }

        /// <summary>
        /// Validate search condition for search employee.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult MAS070_ValidateEmployeeSearch(MAS070_EmployeeSearchCondition condition)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //3.1	Validate require field
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { condition });

                if (!res.IsError)
                {
                    res.ResultData = "P";
                }

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Search employee data.
        /// </summary>
        /// <param name="txtEmployeeCodeSearch"></param>
        /// <param name="txtEmployeeNameSearch"></param>
        /// <returns></returns>
        public ActionResult MAS070_EmployeeSearch(string txtEmployeeCodeSearch, string txtEmployeeNameSearch)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                txtEmployeeCodeSearch = txtEmployeeCodeSearch == "" ? null : txtEmployeeCodeSearch;
                txtEmployeeNameSearch = txtEmployeeNameSearch == "" ? null : txtEmployeeNameSearch;
                IEmployeeMasterHandler hand = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<dtEmployee> list = hand.GetEmployee(txtEmployeeCodeSearch, txtEmployeeNameSearch, null);
                string xml = CommonUtil.ConvertToXml<dtEmployee>(list, "Master\\MAS070_employee", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Search selected employee detail.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS070_EmployeeSearchDetail()
        {
            ObjectResultData res = new ObjectResultData();
            string empNo;
            try
            {
                empNo = (Request["EmpNo"] == "" ? null : Request["EmpNo"]);
                IEmployeeMasterHandler hand = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<dtEmployeeDetail> list = hand.GetEmployeeDetail(empNo);
                if (list != null && list.Count != 0)
                {
                    dtEmployeeDetail emp = list[0];
                    try
                    {
                        if (emp.EmailAddress != null)
                        {
                            emp.EmailAddress = emp.EmailAddress.Substring(0, emp.EmailAddress.IndexOf('@'));
                        }

                        MAS070_ScreenParameter MAS070Param = GetScreenObject<MAS070_ScreenParameter>();
                        if (emp.UpdateDate.HasValue)
                        {
                            MAS070Param.updateDate = emp.UpdateDate.Value;
                        }
                    }
                    catch { }
                    res.ResultData = emp;
                }
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get belonging of selected employee.
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public ActionResult MAS070_SearchBelonging(string empNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                empNo = empNo == "" ? null : empNo;
                IEmployeeMasterHandler hand = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<dtBelonging> list = hand.GetBelonging(null, null, null, empNo);
                CommonUtil.MappingObjectLanguage<dtBelonging>(list);

                MAS070_ScreenParameter MAS070Param = GetScreenObject<MAS070_ScreenParameter>();
                MAS070Param.belongingList = list;

                string xml = CommonUtil.ConvertToXml<dtBelonging>(list, "Master\\MAS070_belonging", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Validate belonging before save.<br />
        /// - Check require field.<br />
        /// - Check exist main department person in charge.
        /// </summary>
        /// <param name="belonging"></param>
        /// <returns></returns>
        public ActionResult MAS070_ValidateBelonging(MAS070_SaveBelonging belonging)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //12.1	Validate require field
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (false == ModelState.IsValid)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                IEmployeeMasterHandler hand = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                ////12.2	Check exist Primary belonging
                //if (belonging.MainDepartmentFlag != null && belonging.MainDepartmentFlag == true) {
                //    List<bool?> listCheck = hand.CheckExistMainDepartmentFlag(belonging.EmpNo, belonging.BelongingID);
                //    if (listCheck.Count != 0 && listCheck[0].Value) {
                //        res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1014);
                //    }
                //}

                if (belonging.DepPersonInchargeFlag != null && belonging.DepPersonInchargeFlag == true)
                {
                    List<bool?> listCheck = hand.CheckExistMainDepartmentPersonInCharge(belonging.OfficeCode, belonging.DepartmentCode, belonging.BelongingID);
                    if (listCheck.Count != 0 && listCheck[0].Value)
                    {
                        bool isError = true;
                        if (belonging.delBelList != null)
                        {
                            foreach (View_tbm_Belonging b in belonging.delBelList)
                            {
                                if (b.OfficeCode == belonging.OfficeCode
                                    && b.DepartmentCode == belonging.DepartmentCode
                                    && b.DepPersonInchargeFlag == true)
                                {
                                    isError = false;
                                    break;
                                }
                            }
                        }
                        if (isError == true)
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1038);
                    }
                }

                //List<bool?> listCheckBel = hand.CheckExistBelonging(belonging.OfficeCode, belonging.DepartmentCode, belonging.EmpNo, belonging.BelongingID);
                //if (listCheckBel.Count != 0 && listCheckBel[0].Value) {
                //    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1011);
                //}

                if (!res.IsError)
                {
                    res.ResultData = "P";
                }

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Get employee name match to cond for auto complete textbox.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult MAS070_GetEmployeeName(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            List<string> listEmpName = new List<string>();

            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                List<dtEmployeeName> lst = handler.GetEmployeeName(cond);

                foreach (var item in lst)
                {
                    listEmpName.Add(item.EmpName);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = listEmpName.ToArray();
            return Json(res);
        }

        /// <summary>
        /// Save employee.<br />
        /// - Check system suspending.<br />
        /// - Validate data.<br />
        /// - Save employee data.<br />
        /// - Save belonging data.<br />
        /// </summary>
        /// <param name="employeeInfo"></param>
        /// <returns></returns>
        public ActionResult MAS070_SaveEmployee(dsEmployeeBelonging employeeInfo)
        {
            ObjectResultData res = new ObjectResultData();

            List<tbm_Employee> resultList = null;
            try
            {
                res = checkSystemSuspending();
                if (res.IsError)
                {
                    return Json(res);
                }

                //12.1	Validate require field
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (false == ModelState.IsValid)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                }

                bool foundPrimary = false;
                foreach (var item in employeeInfo.belongingList)
                {
                    if (item.MainDepartmentFlag == true)
                    {
                        foundPrimary = true;
                    }

                    if (item.ModifyMode.Equals("NONE"))
                    {
                        continue;
                    }
                }

                if (!foundPrimary)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1042);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                IEmployeeMasterHandler hand = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<dtEmployeeDetail> emp = hand.GetEmployeeDetail(employeeInfo.employee.EmpNo);
                // Akat K. : check exist employee number
                if (employeeInfo.employee.ModifyMode.Equals("ADD"))
                {
                   
                    if (emp != null && emp.Count != 0)
                    {
                        if (emp[0].DeleteFlag.Value)
                        {
                            MAS070_ScreenParameter MAS070Param = GetScreenObject<MAS070_ScreenParameter>();
                            MAS070Param.updateDate = emp[0].UpdateDate.Value;
                            MAS070Param.isReactivate = true;
                            MAS070Param.reactivateEmpNo = employeeInfo.employee.EmpNo;
                            res.ResultData = "CONF";
                            return Json(res);
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1013);
                            return Json(res);
                        }
                    }
                }
                else
                {
                    if (emp != null && emp.Count != 0)
                    {
                        employeeInfo.employee.PasswordWrongCount = emp[0].PasswordWrongCount;
                        employeeInfo.employee.PasswordLastUpdateDate = emp[0].PasswordLastUpdateDate;
                        employeeInfo.employee.Status = emp[0].Status;
                    }

                    MAS070_ScreenParameter MAS070Param = GetScreenObject<MAS070_ScreenParameter>();
                    if (MAS070Param.isReactivate)
                    {
                        if (employeeInfo.employee.EmpNo.Equals(MAS070Param.reactivateEmpNo))
                        {
                            hand.DeleteAllBelonging(MAS070Param.reactivateEmpNo);
                        }
                        MAS070Param.isReactivate = false;
                        MAS070Param.reactivateEmpNo = null;
                    }
                }

                if (employeeInfo.belongingList != null)
                {
                    foreach (View_tbm_Belonging bl in employeeInfo.belongingList)
                    {
                        if (bl.StartDate < employeeInfo.employee.StartDate
                            || bl.EndDate < employeeInfo.employee.StartDate)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1050);
                            return Json(res);
                        }
                        if (employeeInfo.employee.EndDate != null)
                        {
                            if (bl.StartDate > employeeInfo.employee.EndDate
                            || bl.EndDate > employeeInfo.employee.EndDate)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1051);
                                return Json(res);
                            }
                        }
                    }
                }

                resultList = this.SaveEmployee(employeeInfo, hand);

                if (resultList != null && resultList.Count != 0)
                {
                    res.ResultData = "P";
                }

            }
            catch (Exception ex)
            {
                res.ResultData = "NP";
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Logical delete employee.<br />
        /// - Check system suspending.<br />
        /// - Set delete flag and update database.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public ActionResult MAS070_DeleteEmployee(tbm_Employee employee)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res = checkSystemSuspending();
                if (res.IsError)
                {
                    return Json(res);
                }

                MAS070_ScreenParameter MAS070Param = GetScreenObject<MAS070_ScreenParameter>();
                employee.UpdateDate = MAS070Param.updateDate;

                IEmployeeMasterHandler hand = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<tbm_Employee> list = hand.DeleteEmployee(employee);

                if (list == null || list.Count == 0)
                {
                    return Json(res);
                }

                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_EMPLOYEE,
                    TableData = CommonUtil.ConvertToXml(list)
                };
                ILogHandler loghand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                loghand.WriteTransactionLog(logData);

                string xml = CommonUtil.ConvertToXml<tbm_Employee>(list);
                res.ResultData = xml;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.ResultData = "NP";
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        private List<tbm_Employee> SaveEmployee(dsEmployeeBelonging employeeInfo, IEmployeeMasterHandler hand)
        {
            List<tbm_Employee> result = null;
            doTransactionLog.eTransactionType? TransactionType = null;
            MAS070_ScreenParameter MAS070Param = GetScreenObject<MAS070_ScreenParameter>();
            PasswordHandler handler = new PasswordHandler();

            using (TransactionScope scope = new TransactionScope())
            {
                if (employeeInfo.employee.ModifyMode.Equals("ADD"))
                {
                    employeeInfo.employee.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    employeeInfo.employee.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    employeeInfo.employee.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    employeeInfo.employee.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    employeeInfo.employee.Password = handler.GeneratePasswordHash(employeeInfo.employee.Password);
                    List<tbm_Employee> insertList = new List<tbm_Employee>();
                    insertList.Add(employeeInfo.employee);
                    string xml = CommonUtil.ConvertToXml_Store(insertList);
                    result = hand.InsertEmployee(xml);
                    TransactionType = doTransactionLog.eTransactionType.Insert;
                }
                else
                {
                    if (employeeInfo.employee.ChangePasswordFlag)
                    {
                        employeeInfo.employee.Password = handler.GeneratePasswordHash(employeeInfo.employee.Password);
                        employeeInfo.employee.PasswordLastUpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        employeeInfo.employee.PasswordWrongCount = 0;
                        employeeInfo.employee.Status = null; 
                    }
                    result = hand.UpdateEmployee(employeeInfo.employee);
                    TransactionType = doTransactionLog.eTransactionType.Update;
                }

                if (result.Count == 0)
                {
                    return null;
                }
                else
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = TransactionType,
                        TableName = TableName.C_TBL_NAME_EMPLOYEE,
                        TableData = CommonUtil.ConvertToXml(result)
                    };
                    ILogHandler loghand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    loghand.WriteTransactionLog(logData);
                }

                List<tbm_Belonging> insertBelList = new List<tbm_Belonging>();
                List<View_tbm_Belonging> checkUpdateBelList = new List<View_tbm_Belonging>();
                List<View_tbm_Belonging> updateBelList = new List<View_tbm_Belonging>();

                foreach (var item in employeeInfo.belongingList)
                {
                    if (item.ModifyMode.Equals("NONE"))
                    {
                        continue;
                    }

                    if (item.ModifyMode.Equals("ADD"))
                    {
                        item.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        item.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        item.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        item.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        insertBelList.Add(item);
                    }
                    else if (item.ModifyMode.Equals("EDIT"))
                    {
                        if (MAS070Param.belongingList != null)
                        {
                            var updateDate = from g in MAS070Param.belongingList
                                             where g.BelongingID == item.BelongingID
                                             select g.UpdateDate;

                            foreach (var date in updateDate)
                            {
                                item.UpdateDate = date;
                            }
                        }

                        if (item.UpdateDate != null)
                        {
                            checkUpdateBelList.Add(item);
                            updateBelList.Add(item);
                        }
                        else
                        {
                            item.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            item.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            item.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            item.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            insertBelList.Add(item);
                        }
                    }
                }


                if (employeeInfo.delBelList != null && employeeInfo.delBelList.Count != 0)
                {
                    foreach (var item in employeeInfo.delBelList)
                    {
                        var updateDate = from g in MAS070Param.belongingList
                                         where g.BelongingID == item.BelongingID
                                         select g.UpdateDate;

                        foreach (var date in updateDate)
                        {
                            item.UpdateDate = date;
                        }
                    }

                    checkUpdateBelList.AddRange(employeeInfo.delBelList);
                }

                if (checkUpdateBelList.Count != 0)
                {
                    hand.checkBelongingUpdateDate(checkUpdateBelList);
                }

                if (insertBelList.Count != 0)
                {
                    string xml = CommonUtil.ConvertToXml_Store(insertBelList);
                    List<tbm_Belonging> insertedList = hand.InsertBelonging(xml);
                    if (insertedList.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        doTransactionLog logData = new doTransactionLog()
                        {
                            TransactionType = doTransactionLog.eTransactionType.Insert,
                            TableName = TableName.C_TBL_NAME_BELONGING,
                            TableData = CommonUtil.ConvertToXml(insertedList)
                        };
                        ILogHandler loghand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        loghand.WriteTransactionLog(logData);
                    }
                }

                foreach (var item in updateBelList)
                {
                    List<tbm_Belonging> updateList = new List<tbm_Belonging>();
                    updateList.Add(item);
                    string xml = CommonUtil.ConvertToXml_Store(updateList);
                    List<tbm_Belonging> updatedList = hand.UpdateBelonging(xml, item.BelongingID);
                    if (updatedList.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        doTransactionLog logData = new doTransactionLog()
                        {
                            TransactionType = doTransactionLog.eTransactionType.Update,
                            TableName = TableName.C_TBL_NAME_BELONGING,
                            TableData = CommonUtil.ConvertToXml(updatedList)
                        };
                        ILogHandler loghand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        loghand.WriteTransactionLog(logData);
                    }
                }

                if (employeeInfo.delBelList != null && employeeInfo.delBelList.Count != 0)
                {
                    foreach (var item in employeeInfo.delBelList)
                    {
                        List<tbm_Belonging> deletedList = hand.DeleteBelonging(item.BelongingID);
                        if (deletedList.Count == 0)
                        {
                            return null;
                        }
                        else
                        {
                            doTransactionLog logData = new doTransactionLog()
                            {
                                TransactionType = doTransactionLog.eTransactionType.Delete,
                                TableName = TableName.C_TBL_NAME_BELONGING,
                                TableData = CommonUtil.ConvertToXml(deletedList)
                            };
                            ILogHandler loghand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                            loghand.WriteTransactionLog(logData);
                        }
                    }
                }

                scope.Complete();
            }

            return result;
        }

    }
}

