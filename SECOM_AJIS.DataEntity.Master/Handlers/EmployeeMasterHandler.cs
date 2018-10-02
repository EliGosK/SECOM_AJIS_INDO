using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.Reflection;
using System.Transactions;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
namespace SECOM_AJIS.DataEntity.Master
{
    public class EmployeeMasterHandler : BizMADataEntities, IEmployeeMasterHandler
    {
        #region Get Methods
        /// <summary>
        /// Getting chief of office data
        /// </summary>
        /// <param name="strOfficeCode"></param>
        /// <returns></returns>
        public List<dtOfficeChief> GetOfficeChiefList(string strOfficeCode)
        {
            return this.GetOfficeChiefList(strOfficeCode, FlagType.C_FLAG_ON);
        }

        /// <summary>
        /// Getting employee list data
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        public List<tbm_Employee> GetEmployeeList(List<tbm_Employee> emp)
        {
            try
            {
                string xml = CommonUtil.ConvertToXml_Store<tbm_Employee>(emp, "EmpNo");
                List<tbm_Employee> lst = this.GetEmployeeList(xml);
                if (lst == null)
                    lst = new List<tbm_Employee>();

                lst = CommonUtil.ConvertObjectbyLanguage<tbm_Employee, tbm_Employee>(lst, "EmpFirstName", "EmpLastName");
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public List<dtEmployee> GetBelongingEmpList(string strOfficeCode, string strDepartmentCode, bool? bChiefFlag, bool? bApproveFlag, bool? bCorrespondentFlag)
        //{
        //    return this.GetBelongingEmpList(strOfficeCode, strDepartmentCode, bChiefFlag, bApproveFlag, bCorrespondentFlag);
        //}

        //public List<dtDepartment> GetBelongingDepartmentList(string strOfficeCode, string strDepartmentCode)
        //{
        //    return this.GetBelongingDepartmentList(strOfficeCode, strDepartmentCode);
        //}

        /// <summary>
        /// Mapping employee list data
        /// </summary>
        /// <param name="empLst"></param>
        public void EmployeeListMapping(EmployeeMappingList empLst)
        {
            try
            {
                if (empLst == null)
                    return;

                List<tbm_Employee> lst = this.GetEmployeeList(empLst.GetEmployeeList());
                if (lst.Count > 0)
                    empLst.SetEmployeeValue(lst);
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Mapping active employee list data
        /// </summary>
        /// <param name="empLst"></param>
        public void ActiveEmployeeListMapping(EmployeeMappingList empLst)
        {
            try
            {
                if (empLst == null)
                    return;

                List<doActiveEmployeeList> lst = this.GetActiveEmployeeList(empLst.GetEmployeeList());
                if (lst.Count > 0)
                {
                    List<tbm_Employee> e = CommonUtil.ClonsObjectList<doActiveEmployeeList, tbm_Employee>(lst);
                    empLst.SetEmployeeValue(e);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Check exist employee
        /// </summary>
        /// <param name="empLst"></param>
        /// <param name="clearData"></param>
        public void CheckActiveEmployeeExist(EmployeeExistList empLst, bool clearData = false)
        {
            try
            {
                if (empLst == null)
                    return;

                List<doActiveEmployeeList> lst = this.GetActiveEmployeeList(empLst.GetEmployeeList());
                if (lst.Count > 0)
                {
                    List<tbm_Employee> e = CommonUtil.ClonsObjectList<doActiveEmployeeList, tbm_Employee>(lst);
                    empLst.CheckEmployeeExist(e, clearData);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Getting active employee data
        /// </summary>
        /// <param name="strEmpNo"></param>
        /// <returns></returns>
        public override List<tbm_Employee> GetActiveEmployee(string strEmpNo)
        {
            try
            {
                List<tbm_Employee> lst = base.GetActiveEmployee(strEmpNo);
                if (lst.Count > 0)
                    CommonUtil.MappingObjectLanguage<tbm_Employee>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Getting active employee list data
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        public List<doActiveEmployeeList> GetActiveEmployeeList(List<tbm_Employee> emp)
        {
            try
            {
                List<doActiveEmployeeList> lst = this.GetActiveEmployeeList(
                    SECOM_AJIS.Common.Util.CommonUtil.ConvertToXml_Store<tbm_Employee>(emp, "EmpNo"));
                if (lst == null)
                    lst = new List<doActiveEmployeeList>();
                lst = CommonUtil.ConvertObjectbyLanguage<doActiveEmployeeList, doActiveEmployeeList>(lst, "EmpFirstName", "EmpLastName");
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check exist active employee
        /// </summary>
        /// <param name="pchvEmpNo"></param>
        /// <returns></returns>
        public bool CheckExistActiveEmployee(string pchvEmpNo)
        {
            try
            {
                // Jirawat Jannet : 2016-08-16
                // Using hash password
                //string password = CommonUtil.HashPassword("");
                List<int?> chkResult = base.CheckExistActiveEmployee(pchvEmpNo);
                if (chkResult.Count > 0)
                {
                    if (CommonUtil.IsNullOrEmpty(chkResult[0]))
                        return false;
                    if (chkResult[0] == 1)
                        return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Getting employee name by employee no
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public List<dtEmpNo> GetEmployeeNameByEmpNo(string empNo)
        {
            List<dtEmpNo> result = base.GetEmployeeNameByEmpNo(empNo);
            CommonUtil.MappingObjectLanguage<dtEmpNo>(result);
            foreach (var i in result) {
                i.EmployeeNameDisplay = i.EmpFirstName + " " + i.EmpLastName;
            }

            return result;
        }

        /// <summary>
        /// Getting main belonging from employee no.
        /// </summary>
        /// <param name="strEmpNo"></param>
        /// <returns></returns>
        public List<doBelongingData> GetMainBelongingByEmpNo(string strEmpNo)
        {
            List<doBelongingData> BelongingData = new List<doBelongingData>();
            try
            {
                BelongingData = base.GetMainBelongingByEmpNo(strEmpNo);
                if (BelongingData.Count > 0)
                    CommonUtil.MappingObjectLanguage<doBelongingData>(BelongingData);
            }
            catch (Exception)
            {
                throw;
            }
            return BelongingData;

        }

        /// <summary>
        /// Getting main belonging employee list.
        /// </summary>
        /// <param name="strOfficeCode"></param>
        /// <param name="strDepartmentCode"></param>
        /// <param name="bChiefFlag"></param>
        /// <param name="bApproveFlag"></param>
        /// <param name="bCorrespondentFlag"></param>
        /// <returns></returns>
        public List<dtEmployeeBelonging> GetBelongingEmpList(string strOfficeCode, string strDepartmentCode, bool? bChiefFlag, bool? bApproveFlag, bool? bCorrespondentFlag)
        {
            List<dtEmployeeBelonging> BelongingData = null;
            try
            {
                BelongingData = base.GetBelongingEmpList(strOfficeCode, strDepartmentCode, bChiefFlag, bApproveFlag, bCorrespondentFlag, CommonUtil.dsTransData.dtUserData.EmpNo, FlagType.C_FLAG_ON);
                if (BelongingData != null && BelongingData.Count > 0)
                    CommonUtil.MappingObjectLanguage<dtEmployeeBelonging>(BelongingData);
            }
            catch (Exception)
            {
                throw;
            }
            return BelongingData;
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Update employee data
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public List<tbm_Employee> UpdateEmployee(tbm_Employee employee)
        {
            List<DateTime?> updateDateList = base.GetEmployeeUpdateDate(employee.EmpNo);
            //if (updateDateList == null || updateDateList.Count == 0 || updateDateList[0] == null
            //    || employee.UpdateDate == null || !employee.UpdateDate.HasValue)
            //{
            //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "UpdateDate" });
            //}

            if (CommonUtil.IsNullOrEmpty(employee.UpdateDate) && updateDateList[0].Value.CompareTo(employee.UpdateDate.Value) != 0) {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, new string[] { employee.EmpNo });
            }

            employee.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            employee.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            List<tbm_Employee> updateList = new List<tbm_Employee>();
            updateList.Add(employee);
            string xml = CommonUtil.ConvertToXml_Store(updateList);
            return base.UpdateEmployee(xml);
        }

        /// <summary>
        /// Delete employee data
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public List<tbm_Employee> DeleteEmployee(tbm_Employee employee) {
            List<DateTime?> updateDateList = base.GetEmployeeUpdateDate(employee.EmpNo);
            if (updateDateList == null || updateDateList.Count == 0 || updateDateList[0] == null
                || employee.UpdateDate == null || !employee.UpdateDate.HasValue)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "UpdateDate" });
            }

            if (updateDateList[0].Value.CompareTo(employee.UpdateDate.Value) != 0) {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, new string[] { employee.EmpNo });
            }

            employee.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            employee.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            return base.DeleteEmployee(employee.EmpNo, employee.DeleteFlag, employee.UpdateBy, employee.UpdateDate);
        }

        /// <summary>
        /// Check and update date  belonging data
        /// </summary>
        /// <param name="checkList"></param>
        public void checkBelongingUpdateDate(List<View_tbm_Belonging> checkList) {
            foreach (var belonging in checkList) {
                List<DateTime?> updateDateList = base.GetBelongingUpdateDate(belonging.BelongingID);
                if (updateDateList == null || updateDateList.Count == 0 || updateDateList[0] == null
                    || belonging.UpdateDate == null || !belonging.UpdateDate.HasValue)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "UpdateDate" });
                }

                if (updateDateList[0].Value.CompareTo(belonging.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, new string[] { belonging.EmpNo });
                }

                belonging.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                belonging.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            }
        }

        #endregion
    }
}
