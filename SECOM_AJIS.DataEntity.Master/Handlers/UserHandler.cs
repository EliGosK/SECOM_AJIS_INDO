using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master.Handlers;

namespace SECOM_AJIS.DataEntity.Master
{
    public class UserHandler : BizMADataEntities, IUserHandler
    {
        
        public  List<dtEmailAddress> GetUserEmailAddressDataList(doEmailSearchCondition cond)
        {
            //doEmailCondition
            try
            {

                return base.GetUserEmailAddressDataList( cond.EmployeeName , cond.EmailEddress, cond.Office, cond.Department);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            
        }

        public bool IsValidLogin(string employeeNo, string password)
        {
            try
            {
                List<tbm_Employee> user = base.GetEmployeeByEmpNo(employeeNo);
                if (user.Count < 1)
                {
                    return false;
                }

                tbm_Employee loginUser = user.First();
                PasswordHandler handle = new PasswordHandler();
                if (loginUser.Status == PasswordHandler.STATUS_ACCOUNT_LOCK)
                {
                    return false;
                }

                if (loginUser.Password == handle.GeneratePasswordHash(password))
                {
                    handle.PasswordWrongCountReset(loginUser.EmpNo);
                    return true;
                }
                else
                {
                    handle.PasswordWrongCountUp(loginUser.EmpNo);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
