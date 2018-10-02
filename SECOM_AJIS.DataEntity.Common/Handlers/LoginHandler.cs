using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;

using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;

using System.Transactions;

using SECOM_AJIS.Common;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using System.DirectoryServices.AccountManagement;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Configuration;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Master.Handlers;

namespace SECOM_AJIS.DataEntity.Common
{
    class LoginHandler : BizCMDataEntities, ILoginHandler
    {
        /// <summary>
        /// Keep user login log
        /// </summary>
        /// <param name="pchrEmpNo"></param>
        /// <param name="pchrLogType"></param>
        /// <returns></returns>
        public  bool KeepHistory(string pchrEmpNo, string pchrLogType)
        {
            List<int?> insertResult = base.KeepHistory_ef(pchrEmpNo, pchrLogType);

            if (insertResult[0] == 1)
                return true;
            else
                return false;

        }
        /// <summary>
        /// Validate user login with domail user permission
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public bool LoginDomain(doLogin Cond)
        {
            //string domain = ConfigurationManager.AppSettings["DOMAIN_NAME"];
            //if (string.IsNullOrEmpty(domain))
            //{
            //    domain = CommonValue.DOMAIN_NAME;
            //}
            //using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
            //{
            //    bool isValid = pc.ValidateCredentials(Cond.EmployeeNo, Cond.Password);
            //    // 2016.06 modify tanaka start
            //    isValid = true;
            //    // 2016.06 modify tanaka end
            //    return isValid;
            //}
            bool isValid = false;
            UserHandler uh = new UserHandler();
            isValid = uh.IsValidLogin(Cond.EmployeeNo, Cond.Password);
            return isValid;
        }

        public bool IsLockedEmployee(string empNo)
        {
            MADataEntities db = new MADataEntities();
            var lockedEmployee = db.tbm_Employee.Where(e => e.EmpNo == empNo && e.Status == PasswordHandler.STATUS_ACCOUNT_LOCK);
            if (lockedEmployee.Count() < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
