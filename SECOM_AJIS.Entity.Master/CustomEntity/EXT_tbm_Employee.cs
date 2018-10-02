using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    [MetadataType(typeof(tbm_Employee_MetaData))]
    public partial class tbm_Employee
    {
        public string EmpFirstName { get; set; }
        public string EmpLastName { get; set; }
        public string EmpFullName
        {
            get
            {
                return CommonUtil.TextList(new string[] { this.EmpFirstName ,this.EmpLastName } , " ");
            }
        }
    }

    #region Employee Mapping

    public class EmployeeMappingList
    {
        #region Inner class

        private class EmployeeMappingData
        {
            public string EmployeeNo { get; set; }
            private object EmployeeObject { get; set; }
            private PropertyInfo EmployeeNameProperty { get; set; }

            public EmployeeMappingData(string key, object obj, PropertyInfo field)
            {
                EmployeeNo = key;
                EmployeeObject = obj;
                EmployeeNameProperty = field;
            }

            public void SetEmployeeData(tbm_Employee emp)
            {
                if (emp == null
                    || EmployeeNameProperty == null
                    || EmployeeObject == null)
                    return;

                EmployeeNameProperty.SetValue(EmployeeObject, emp.EmpFullName, null);
            }
            
            
        }

        #endregion
        #region Variables

        private List<EmployeeMappingData> EmployeeList { get; set; }

        #endregion
        #region Initial Employee

        public void AddEmployee(params object[] employee)
        {
            try
            {
                if (employee != null)
                {
                    if (EmployeeList == null)
                        EmployeeList = new List<EmployeeMappingData>();

                    foreach (object emp in employee)
                    {
                        Dictionary<string, EmployeeMappingAttribute> empAttr = CommonUtil.CreateAttributeDictionary<EmployeeMappingAttribute>(emp);
                        foreach (KeyValuePair<string, EmployeeMappingAttribute> attr in empAttr)
                        {
                            PropertyInfo prop = emp.GetType().GetProperty(attr.Key);
                            if (prop != null)
                            {
                                object val = prop.GetValue(emp, null);
                                if (CommonUtil.IsNullOrEmpty(val) == true)
                                    continue;

                                PropertyInfo field = emp.GetType().GetProperty(attr.Value.EmployeeNameField);
                                if (field != null)
                                {
                                    EmployeeMappingData empData = new EmployeeMappingData(val.ToString(), emp, field);
                                    EmployeeList.Add(empData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Database Mapping

        public List<tbm_Employee> GetEmployeeList()
        {
            try
            {
                List<tbm_Employee> empLst = new List<tbm_Employee>();
                if (EmployeeList != null)
                {
                    foreach (EmployeeMappingData empData in EmployeeList)
                    {
                        tbm_Employee emp = null;
                        foreach (tbm_Employee ee in empLst)
                        {
                            if (ee.EmpNo == empData.EmployeeNo)
                                emp = ee;
                        }
                        if (emp == null)
                        {
                            emp = new tbm_Employee()
                            {
                                EmpNo = empData.EmployeeNo
                            };
                            empLst.Add(emp);
                        }
                    }
                }

                return empLst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SetEmployeeValue(List<tbm_Employee> empLst)
        {
            try
            {
                if (empLst != null
                    && EmployeeList != null)
                {
                    foreach (EmployeeMappingData empData in EmployeeList)
                    {
                        foreach (tbm_Employee emp in empLst)
                        {
                            if (empData.EmployeeNo == emp.EmpNo)
                            {
                                empData.SetEmployeeData(emp);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
    public class EmployeeExistList
    {
        #region Inner class

        private class EmployeeExistData
        {
            public string EmployeeNo { get; set; }
            public string Property { get; set; }
            public string Parameter { get; set; }
            public string Control { get; set; }

            private object EmployeeObject { get; set; }
            private PropertyInfo EmployeeNameProperty { get; set; }

            public EmployeeExistData(string key, string field, object obj, PropertyInfo prop, string parameter, string control = null)
            {
                EmployeeNo = key;
                Property = field;

                if (parameter == null)
                    Parameter = field;
                else
                    Parameter = parameter;

                Control = control;

                EmployeeObject = obj;
                EmployeeNameProperty = prop;
            }
            public void ClearEmployeeData()
            {
                if (EmployeeNameProperty == null
                    || EmployeeObject == null)
                    return;

                EmployeeNameProperty.SetValue(EmployeeObject, null, null);
            }
        }

        #endregion
        #region Variables

        private List<EmployeeExistData> EmployeeList { get; set; }

        #endregion
        #region Initial Employee

        public void AddEmployee(params object[] employee)
        {
            try
            {
                if (employee != null)
                {
                    if (EmployeeList == null)
                        EmployeeList = new List<EmployeeExistData>();

                    foreach (object emp in employee)
                    {
                        Dictionary<string, EmployeeExistAttribute> empAttr = CommonUtil.CreateAttributeDictionary<EmployeeExistAttribute>(emp);
                        foreach (KeyValuePair<string, EmployeeExistAttribute> attr in empAttr)
                        {
                            PropertyInfo prop = emp.GetType().GetProperty(attr.Key);
                            if (prop != null)
                            {
                                object val = prop.GetValue(emp, null);
                                if (CommonUtil.IsNullOrEmpty(val) == true)
                                    continue;

                                EmployeeExistData empData = new EmployeeExistData(val.ToString(), prop.Name, emp, prop, attr.Value.Parameter, attr.Value.Control);
                                EmployeeList.Add(empData);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Database Mapping

        public List<tbm_Employee> GetEmployeeList()
        {
            try
            {
                List<tbm_Employee> empLst = new List<tbm_Employee>();
                if (EmployeeList != null)
                {
                    foreach (EmployeeExistData empData in EmployeeList)
                    {
                        tbm_Employee emp = null;
                        foreach (tbm_Employee ee in empLst)
                        {
                            if (ee.EmpNo == empData.EmployeeNo)
                                emp = ee;
                        }
                        if (emp == null)
                        {
                            emp = new tbm_Employee()
                            {
                                EmpNo = empData.EmployeeNo
                            };
                            empLst.Add(emp);
                        }
                    }
                }

                return empLst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void CheckEmployeeExist(List<tbm_Employee> empLst, bool clearData)
        {
            try
            {
                if (empLst != null
                    && EmployeeList != null)
                {
                    List<EmployeeExistData> rmLst = new List<EmployeeExistData>();
                    foreach (EmployeeExistData empData in EmployeeList)
                    {
                        foreach (tbm_Employee emp in empLst)
                        {
                            /* --- Merge --- */
                            /* if (empData.EmployeeNo == emp.EmpNo) */
                            if (empData.EmployeeNo.ToUpper() == emp.EmpNo.ToUpper())
                            /* ------------- */
                            
                            {
                                rmLst.Add(empData);
                                break;
                            }
                        }
                    }
                    foreach (EmployeeExistData empData in rmLst)
                    {
                        EmployeeList.Remove(empData);
                    }

                    if (clearData)
                    {
                        foreach (EmployeeExistData empData in EmployeeList)
                        {
                            empData.ClearEmployeeData();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Return Result

        public string EmployeeNoExist
        {
            get
            {
                if (EmployeeList == null)
                    return string.Empty;

                List<string> lst = new List<string>();
                foreach (EmployeeExistData ex in EmployeeList)
                {
                    if (lst.IndexOf(ex.EmployeeNo) < 0)
                        lst.Add(ex.EmployeeNo);
                }
                return CommonUtil.TextList(lst.ToArray());
            }
        }
        public string[] ControlNoExist
        {
            get
            {
                if (EmployeeList == null)
                    return null;

                List<string> lst = new List<string>();
                foreach (EmployeeExistData ex in EmployeeList)
                {
                    if (ex.Control != null)
                    {
                        if (lst.IndexOf(ex.Control) < 0)
                            lst.Add(ex.Control);
                    }
                }
                return lst.ToArray();
            }
        }

        #endregion
    }
    
    #endregion
}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    public class tbm_Employee_MetaData
    {
        [LanguageMapping]
        public string EmpFirstName { get; set; }
        [LanguageMapping]
        public string EmpLastName { get; set; }
    }
}