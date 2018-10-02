using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.Reflection;
using System.Transactions;
using SECOM_AJIS.Common.Models;
using System.Security.Cryptography;

namespace SECOM_AJIS.DataEntity.Master.Handlers
{
    public class PasswordHandler
    {
        public const int MAX_PASSWORD_WRONG_COUNT = 5;
        public const string STATUS_ACCOUNT_LOCK = "L";

        MADataEntities db = new MADataEntities();

        public ObjectResultData UpdatePassword(string oldPassword, string newPassword)
        {
            ObjectResultData res = new ObjectResultData();
            string employeeNo = CommonUtil.dsTransData.dtUserData.EmpNo;
            string oldPasswordHash = GeneratePasswordHash(oldPassword);
            var targetEmployee = db.tbm_Employee.Where(e => e.EmpNo == employeeNo && e.Password == oldPasswordHash);
            if (targetEmployee.Count() < 1)
            {
                res.ResultData = 0;
                return res;
            }
            
            tbm_Employee employee = targetEmployee.First();
            employee.Password = GeneratePasswordHash(newPassword);
            employee.PasswordLastUpdateDate = DateTime.Now;
            res.ResultData = db.SaveChanges();
            
            return res;
        }

        public ObjectResultData PasswordWrongCountUp(string employeeNo)
        {
            ObjectResultData res = new ObjectResultData();
            var employees = db.tbm_Employee.Where(e => e.EmpNo == employeeNo);
            if (employees.Count() < 1)
            {
                return res;
            }
            tbm_Employee employee = employees.First();
            if (CommonUtil.IsNullOrEmpty(employee.PasswordWrongCount))
            {
                employee.PasswordWrongCount = 0;
            }

            employee.PasswordWrongCount = employee.PasswordWrongCount + 1;

            // mistake password 5 times -> account lock
            if (employee.PasswordWrongCount == MAX_PASSWORD_WRONG_COUNT)
            {
                employee.Status = STATUS_ACCOUNT_LOCK;
            }
            
            res.ResultData = db.SaveChanges();
            return res;
        }
        
        public ObjectResultData PasswordWrongCountReset(string employeeNo)
        {
            ObjectResultData res = new ObjectResultData();
            var employees = db.tbm_Employee.Where(e => e.EmpNo == employeeNo);
            if (employees.Count() < 1)
            {
                return res;
            }
            tbm_Employee employee = employees.First();
            employee.PasswordWrongCount = 0;
            res.ResultData = db.SaveChanges();
            return res;
        }

        public string GeneratePasswordHash(string basePassword)
        {
            return System.Convert.ToBase64String(SHA256Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(basePassword)));
        }
    }
}
