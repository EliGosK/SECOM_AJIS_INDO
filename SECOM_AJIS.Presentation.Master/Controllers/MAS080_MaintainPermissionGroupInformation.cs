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
using System.Xml;
using System.IO;
using SECOM_AJIS.Presentation.Master.Models;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.Presentation.Master.Controllers
{
    public partial class MasterController : BaseController
    {

        private const string MAS080_Screen = "MAS080";

        /// <summary>
        /// - Check user permission for screen MAS080.<br />
        /// - Check system suspending.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult MAS080_Authority(MAS080_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (!(CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_PERMISSION_GROUP_INFO, FunctionID.C_FUNC_ID_VIEW) == true
                       || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_PERMISSION_GROUP_INFO, FunctionID.C_FUNC_ID_ADD) == true
                       || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_PERMISSION_GROUP_INFO, FunctionID.C_FUNC_ID_EDIT) == true
                       || CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_PERMISSION_GROUP_INFO, FunctionID.C_FUNC_ID_DEL) == true
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
                //param.hasPermissionAdd = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_PERMISSION_GROUP_INFO, FunctionID.C_FUNC_ID_ADD);
                //param.hasPermissionEdit = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_PERMISSION_GROUP_INFO, FunctionID.C_FUNC_ID_EDIT);
                //param.hasPermissionDelete = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_PERMISSION_GROUP_INFO, FunctionID.C_FUNC_ID_DEL);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<MAS080_ScreenParameter>("MAS080", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize(MAS080_Screen)]
        public ActionResult MAS080()
        {
            MAS080_ScreenParameter MAS080Param = new MAS080_ScreenParameter();
            ViewBag.HasPermissionAdd = "";
            ViewBag.HasPermissionEdit = "";
            ViewBag.HasPermissionDelete = "";
            ViewBag.FunctionView = FunctionID.C_FUNC_ID_VIEW;
            ViewBag.FunctionOperate = FunctionID.C_FUNC_ID_OPERATE;
            ViewBag.FunctionPlanner = FunctionID.C_FUNC_ID_PLANNER;

            try
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> lstMiscTypeCode = new List<doMiscTypeCode>();
                doMiscTypeCode MiscTypeCode = new doMiscTypeCode();
                MiscTypeCode.FieldName = MiscType.C_PERMISSION_TYPE;
                MiscTypeCode.ValueCode = "%";
                lstMiscTypeCode.Add(MiscTypeCode);
                List<doMiscTypeCode> MiscLock = hand.GetMiscTypeCodeList(lstMiscTypeCode);
                foreach (doMiscTypeCode i in MiscLock)
                {
                    if (i.ValueCode == PermissionType.C_PERMISSION_TYPE_OFFICE)
                    {
                        ViewBag.PermissionTypeOffice = i.ValueDisplay;
                    }
                    else
                    {
                        ViewBag.PermissionTypeIndividual = i.ValueDisplay;
                    }
                }

                MAS080Param = GetScreenObject<MAS080_ScreenParameter>();
                ViewBag.HasPermissionAdd = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_PERMISSION_GROUP_INFO, FunctionID.C_FUNC_ID_ADD);
                ViewBag.HasPermissionEdit = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_PERMISSION_GROUP_INFO, FunctionID.C_FUNC_ID_EDIT);
                ViewBag.HasPermissionDelete = CheckUserPermission(ScreenID.C_SCREEN_ID_MAINTAIN_PERMISSION_GROUP_INFO, FunctionID.C_FUNC_ID_DEL);
                ViewBag.FunctionView = FunctionID.C_FUNC_ID_VIEW;
            }
            catch
            {
            }

            return View();
        }

        /// <summary>
        /// Get config for Permission table.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS080_InitPermissionGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS080_permission", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get config for Employee table.
        /// </summary>
        /// <returns></returns>
        public ActionResult MAS080_InitEmployeeGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Master\\MAS080_employee", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Get Function, Screen, Module to fill in Screen Function Tree.
        /// </summary>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        public ActionResult GetFuntionXml(int? moduleID)
        {
            string xml = string.Empty;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<tree id='0'></tree>");

                IPermissionMasterHandler hand = ServiceContainer.GetService<IPermissionMasterHandler>() as IPermissionMasterHandler;
                List<dtObjectFunction> list = hand.GetObjectFunction(moduleID);

                IList<dtObjectFunction> modules = list.GroupBy(p => p.ModuleID)
                                                                .Select(g => g.First())
                                                                .ToList();

                foreach (dtObjectFunction module in modules)
                {
                    XmlNode node = doc.CreateNode(XmlNodeType.Element, "item", "");
                    XmlAttribute xat = doc.CreateAttribute("text");
                    xat.Value = module.ModuleName;
                    node.Attributes.Append(xat);

                    var objects = (from l in list
                                   where l.ModuleID == module.ModuleID
                                   select new
                                   {
                                       ObjectID = l.ObjectID,
                                       ObjectNameEn = l.ObjectNameEn,
                                       ModuleID = l.ModuleID
                                   }).Distinct();

                    foreach (var obj in objects)
                    {
                        XmlNode nobj = doc.CreateNode(XmlNodeType.Element, "item", "");
                        XmlAttribute xat2 = doc.CreateAttribute("text");
                        xat2.Value = obj.ObjectID + ":" + obj.ObjectNameEn;
                        nobj.Attributes.Append(xat2);

                        var functions = from l in list
                                        where l.ObjectID == obj.ObjectID && l.ModuleID == obj.ModuleID
                                        select l;

                        foreach (dtObjectFunction function in functions)
                        {
                            XmlNode nfunc = doc.CreateNode(XmlNodeType.Element, "item", "");
                            XmlAttribute xat3 = doc.CreateAttribute("text");
                            xat3.Value = function.FunctionName;
                            nfunc.Attributes.Append(xat3);

                            XmlAttribute xa3 = doc.CreateAttribute("id");
                            //xa3.Value = "f" + function.FunctionID + "o" + obj.ObjectID + "m" + module.ModuleID.ToString();
                            xa3.Value = function.FunctionID + "-" + obj.ObjectID;
                            nfunc.Attributes.Append(xa3);
                            nobj.AppendChild(nfunc);
                        }
                        XmlAttribute xa2 = doc.CreateAttribute("id");
                        xa2.Value = "o" + obj.ObjectID + "m" + module.ModuleID.ToString();
                        nobj.Attributes.Append(xa2);
                        node.AppendChild(nobj);
                    }
                    XmlAttribute xa1 = doc.CreateAttribute("id");
                    xa1.Value = "m" + module.ModuleID.ToString();
                    node.Attributes.Append(xa1);
                    doc.ChildNodes[0].AppendChild(node);
                    //doc.AppendChild(node);
                }

                StringWriter sw = new StringWriter();
                XmlTextWriter tx = new XmlTextWriter(sw);
                doc.WriteTo(tx);

                xml = sw.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            ContentResult res = new ContentResult();
            res.Content = xml;
            res.ContentType = "text/xml";
            return res;
        }

        /// <summary>
        /// Get permission group's name match to cond for auto complete textbox.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult MAS080_GetPermissionGroupName(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            List<string> listName = new List<string>();

            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                listName = handler.GetPermissionGroupName(cond);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = listName.ToArray();
            return Json(res);
        }

        /// <summary>
        /// Get employee code match to cond for auto complete textbox.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult MAS080_EmployeeCode(string cond)
        {
            ObjectResultData res = new ObjectResultData();
            List<string> listCode = new List<string>();

            try
            {
                IAutoCompleteHandler handler = ServiceContainer.GetService<IAutoCompleteHandler>() as IAutoCompleteHandler;
                listCode = handler.GetEmployeeCode(cond);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            res.ResultData = listCode.ToArray();
            return Json(res);
        }

        /// <summary>
        /// Search permission match to condition and transform to xml format before return to screen.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MAS080_SearchPermission(doPermission condition)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                condition.ObjectFunction = CommonUtil.IsNullOrEmpty(condition.ObjectFunction) ? null : "," + condition.ObjectFunction + ",";
                IPermissionMasterHandler hand = ServiceContainer.GetService<IPermissionMasterHandler>() as IPermissionMasterHandler;
                List<dtPermissionHeader> list = hand.GetPermission(condition);
                CommonUtil.MappingObjectLanguage<dtPermissionHeader>(list);

                MAS080_ScreenParameter MAS080Param = GetScreenObject<MAS080_ScreenParameter>();
                MAS080Param.PermissionList = list;

                string xml = CommonUtil.ConvertToXml<dtPermissionHeader>(list, "Master\\MAS080_permission", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                res.ResultData = xml;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get allow function in user selected permission.
        /// </summary>
        /// <param name="permissionGroupCode"></param>
        /// <param name="permissionIndividualCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MAS080_GetFunction(string permissionGroupCode, string permissionIndividualCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                permissionGroupCode = (permissionGroupCode == "" ? null : permissionGroupCode);
                permissionIndividualCode = (permissionIndividualCode == "" ? null : permissionIndividualCode);

                IPermissionMasterHandler hand = ServiceContainer.GetService<IPermissionMasterHandler>() as IPermissionMasterHandler;
                List<dtFunction> list = hand.GetFunction(permissionGroupCode, permissionIndividualCode);
                res.ResultData = list;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get employee list for permission individual.
        /// </summary>
        /// <param name="permissionGroupCode"></param>
        /// <param name="permissionIndividualCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MAS080_GetEmpNo(string permissionGroupCode, string permissionIndividualCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                permissionGroupCode = (permissionGroupCode == "" ? null : permissionGroupCode);
                permissionIndividualCode = (permissionIndividualCode == "" ? null : permissionIndividualCode);

                IPermissionMasterHandler hand = ServiceContainer.GetService<IPermissionMasterHandler>() as IPermissionMasterHandler;
                List<dtEmpNo> list = hand.GetEmpNo(permissionGroupCode, permissionIndividualCode);

                foreach (var i in list)
                {
                    if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        i.EmployeeNameDisplay = i.EmpFirstNameEN + " " + i.EmpLastNameEN;
                    }
                    else
                    {
                        i.EmployeeNameDisplay = i.EmpFirstNameLC + " " + i.EmpLastNameLC;
                    }
                }

                string xml = CommonUtil.ConvertToXml<dtEmpNo>(list, "Master\\MAS080_employee", CommonUtil.GRID_EMPTY_TYPE.INSERT);
                res.ResultData = xml;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Check is employee already set to current permission individual.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="officeCode"></param>
        /// <param name="departmentCode"></param>
        /// <param name="positionCode"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MAS080_CheckExistEmpNo(List<MAS080_dtEmpNo> current, string officeCode, string departmentCode, string positionCode, string empNo)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                if (current != null)
                {
                    foreach (MAS080_dtEmpNo emp in current)
                    {
                        if (emp == null)
                            continue;

                        if (emp.EmpNo == empNo
                            && emp.ModifyMode != "DEL")
                        {
                            res.AddErrorMessage(
                                MessageUtil.MODULE_MASTER,
                                MessageUtil.MessageList.MSG1025);
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            return Json(res);
                        }
                    }
                }

                departmentCode = (departmentCode == "" ? null : departmentCode);
                positionCode = (positionCode == "" ? null : positionCode);

                IPermissionMasterHandler hand = ServiceContainer.GetService<IPermissionMasterHandler>() as IPermissionMasterHandler;
                List<bool?> list = hand.CheckExistEmpNo(officeCode, departmentCode, positionCode, empNo);

                if (list == null || list.Count == 0 || !list[0].HasValue || !list[0].Value)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1015);
                }

                if (!res.IsError)
                {
                    res.ResultData = "P";
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Validate data of permission type office.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public ActionResult MAS080_ValidateTypeOffice(MAS080_Save permission)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (false == ModelState.IsValid)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                res.ResultData = "P";
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Validate data of permission type individual.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public ActionResult MAS080_ValidateTypeInidividual(MAS080_InsertIndividual permission)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (false == ModelState.IsValid)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                res.ResultData = "P";
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Insert new permission type office.<br />
        /// - Check exist permission.<br />
        /// - Insert new permission group to database.<br />
        /// - Insert new permission detail (allow function) to database.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public ActionResult MAS080_InsertPermissionOffice(MAS080_Save permission)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res = checkSystemSuspending();
                if (res.IsError)
                {
                    return Json(res);
                }

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (false == ModelState.IsValid)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                permission.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                permission.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                permission.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                permission.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                IPermissionMasterHandler hand = ServiceContainer.GetService<IPermissionMasterHandler>() as IPermissionMasterHandler;
                List<bool?> list = hand.CheckExistPermission(permission.OfficeCode, permission.DepartmentCode, permission.PositionCode);

                //3.1	Validate require field
                if (list == null || list.Count == 0 || !list[0].HasValue || list[0].Value)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_MASTER, MessageUtil.MessageList.MSG1018);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                if (permission.ObjectFunction != null && permission.ObjectFunction != "")
                {
                    string objFunc = "";
                    string[] checkedList = permission.ObjectFunction.Split(',');
                    foreach (var i in checkedList)
                    {
                        if (i.IndexOf('-') > 0)
                        {
                            objFunc += "," + i;
                        }
                    }
                    permission.ObjectFunction = objFunc.Substring(1);
                }

                List<tbm_PermissionGroup> result = hand.AddPermissionTypeOffice(permission);

                if (result != null)
                {
                    res.ResultData = "P";
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Insert new permission type individual.<br />
        /// - Insert new permission group to database.<br />
        /// - Insert new permission detail (allow function) to database.<br />
        /// - Insert new permission individual to database.<br />
        /// - Insert new permission individual detail (employee) to database.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public ActionResult MAS080_InsertPermissionIndividual(MAS080_InsertIndividual permission)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res = checkSystemSuspending();
                if (res.IsError)
                {
                    return Json(res);
                }

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (false == ModelState.IsValid)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                permission.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                permission.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                permission.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                permission.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                permission.EmpNo = permission.EmpNo.Substring(1);
                string empno = "";
                string[] empList = permission.EmpNo.Split(',');
                foreach (var i in empList)
                {
                    empno += "," + i.Split(':')[0];
                }
                permission.EmpNo = empno.Substring(1);

                if (permission.ObjectFunction != null && permission.ObjectFunction != "")
                {
                    string objFunc = "";
                    string[] checkedList = permission.ObjectFunction.Split(',');
                    foreach (var i in checkedList)
                    {
                        if (i.IndexOf('-') > 0)
                        {
                            objFunc += "," + i;
                        }
                    }
                    permission.ObjectFunction = objFunc.Substring(1);
                }

                IPermissionMasterHandler hand = ServiceContainer.GetService<IPermissionMasterHandler>() as IPermissionMasterHandler;
                List<tbm_PermissionIndividual> result = hand.AddPermissionTypeIndividual(permission);

                if (result != null)
                {
                    res.ResultData = "P";
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Edit new permission type office.<br />
        /// - Update permission group to database.<br />
        /// - Delete all permission detail.<br />
        /// - Insert new permission detail.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public ActionResult MAS080_EditPermissionTypeOffice(MAS080_Save permission)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res = checkSystemSuspending();
                if (res.IsError)
                {
                    return Json(res);
                }

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (false == ModelState.IsValid)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                permission.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                permission.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //permission.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                //permission.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                if (permission.ObjectFunction != null && permission.ObjectFunction != "")
                {
                    string objFunc = "";
                    string[] checkedList = permission.ObjectFunction.Split(',');
                    foreach (var i in checkedList)
                    {
                        if (i.IndexOf('-') > 0)
                        {
                            objFunc += "," + i;
                        }
                    }
                    permission.ObjectFunction = objFunc.Substring(1);
                }

                MAS080_ScreenParameter MAS080Param = GetScreenObject<MAS080_ScreenParameter>();
                var updateDate = from g in MAS080Param.PermissionList
                                 where g.crPermissionGroupCode == permission.PermissionGroupCode
                                 select g.UpdateDate;

                foreach (var date in updateDate)
                {
                    permission.UpdateDate = date;
                }

                IPermissionMasterHandler hand = ServiceContainer.GetService<IPermissionMasterHandler>() as IPermissionMasterHandler;
                List<tbm_PermissionGroup> result = hand.EditPermissionTypeOffice(permission);

                if (result != null)
                {
                    res.ResultData = "P";
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Edit permission type individual.<br />
        /// - Delete all permission detail.<br />
        /// - Insert new permission detail.
        /// - Edit permission individual to database.<br />
        /// - Delete all permission individual detail.<br />
        /// - Insert new permission individual detail.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public ActionResult MAS080_EditPermissionTypeIndividual(MAS080_InsertIndividual permission)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res = checkSystemSuspending();
                if (res.IsError)
                {
                    return Json(res);
                }

                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                if (false == ModelState.IsValid)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                permission.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                permission.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //permission.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                //permission.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                if (permission.ObjectFunction != null && permission.ObjectFunction != "")
                {
                    string objFunc = "";
                    string[] checkedList = permission.ObjectFunction.Split(',');
                    foreach (var i in checkedList)
                    {
                        if (i.IndexOf('-') > 0)
                        {
                            objFunc += "," + i;
                        }
                    }
                    permission.ObjectFunction = objFunc.Substring(1);
                }

                string addempno = "";
                string remempno = ",";
                if (permission.EmpNo != null && !permission.EmpNo.Equals(""))
                {
                    permission.EmpNo = permission.EmpNo.Substring(1);
                    string[] empList = permission.EmpNo.Split(',');
                    foreach (var i in empList)
                    {
                        string[] empno = i.Split(':');
                        if (empno[1].Equals("DEL"))
                        {
                            remempno += empno[0] + ",";
                        }
                        else if (empno[1].Equals("ADD"))
                        {
                            addempno += "," + empno[0];
                        }
                    }
                }

                if (!addempno.Equals(""))
                {
                    permission.EmpNo = addempno.Substring(1);
                }
                else
                {
                    permission.EmpNo = null;
                }
                if (!remempno.Equals(","))
                {
                    permission.DelEmpNo = remempno;
                }
                else
                {
                    permission.DelEmpNo = null;
                }

                MAS080_ScreenParameter MAS080Param = GetScreenObject<MAS080_ScreenParameter>();
                var updateDate = from g in MAS080Param.PermissionList
                                 where g.crPermissionGroupCode == permission.PermissionGroupCode
                                    && g.crPermissionIndividualCode == permission.PermissionIndividualCode
                                 select g.UpdateDate;

                foreach (var date in updateDate)
                {
                    permission.UpdateDate = date;
                }

                IPermissionMasterHandler hand = ServiceContainer.GetService<IPermissionMasterHandler>() as IPermissionMasterHandler;
                List<tbm_PermissionIndividual> result = hand.EditPermissionTypeIndividual(permission);

                if (result != null)
                {
                    res.ResultData = "P";
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Physical delete permission type office.<br />
        /// - Delete all permission detail.<br />
        /// - Delete permission group.
        /// </summary>
        /// <param name="permissionGroupCode"></param>
        /// <returns></returns>
        public ActionResult MAS080_DeletePermissionTypeOffice(string permissionGroupCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res = checkSystemSuspending();
                if (res.IsError)
                {
                    return Json(res);
                }

                DateTime UpdateDatetime = DateTime.Now;
                MAS080_ScreenParameter MAS080Param = GetScreenObject<MAS080_ScreenParameter>();
                var updateDate = from g in MAS080Param.PermissionList
                                 where g.crPermissionGroupCode == permissionGroupCode
                                 select g.UpdateDate;

                foreach (var date in updateDate)
                {
                    UpdateDatetime = date.Value;
                }

                IPermissionMasterHandler hand = ServiceContainer.GetService<IPermissionMasterHandler>() as IPermissionMasterHandler;
                List<tbm_PermissionGroup> result = hand.DeletePermissionTypeOffice(permissionGroupCode, UpdateDatetime);

                if (result != null)
                {
                    res.ResultData = "P";
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Physical delete permission type individual.<br />
        /// - Delete all permission detail.<br />
        /// - Delete all permission individual detail.<br />
        /// - Delete permission individual.
        /// </summary>
        /// <param name="permissionGroupCode"></param>
        /// <param name="permissionIndividualCode"></param>
        /// <returns></returns>
        public ActionResult MAS080_DeletePermissionTypeInidividual(string permissionGroupCode, string permissionIndividualCode)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                res = checkSystemSuspending();
                if (res.IsError)
                {
                    return Json(res);
                }

                DateTime UpdateDatetime = DateTime.Now;
                MAS080_ScreenParameter MAS080Param = GetScreenObject<MAS080_ScreenParameter>();
                var updateDate = from g in MAS080Param.PermissionList
                                 where g.crPermissionGroupCode == permissionGroupCode
                                    && g.crPermissionIndividualCode == permissionIndividualCode
                                 select g.UpdateDate;

                foreach (var date in updateDate)
                {
                    UpdateDatetime = date.Value;
                }

                IPermissionMasterHandler hand = ServiceContainer.GetService<IPermissionMasterHandler>() as IPermissionMasterHandler;
                List<tbm_PermissionDetail> result = hand.DeletePermissionTypeIndividual(
                    permissionGroupCode,
                    permissionIndividualCode,
                    UpdateDatetime);

                if (result != null)
                {
                    res.ResultData = "P";
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }

        /// <summary>
        /// Get employee name of user inputed employee number.
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MAS080_GetEmpNameByEmpNo(string empNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IEmployeeMasterHandler hand = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                List<dtEmpNo> result = hand.GetEmployeeNameByEmpNo(empNo);

                if (result != null && result.Count > 0)
                {
                    res.ResultData = result[0].EmployeeNameDisplay;
                }
                else
                {
                    res.ResultData = "";
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }
            return Json(res);
        }
    }
}

