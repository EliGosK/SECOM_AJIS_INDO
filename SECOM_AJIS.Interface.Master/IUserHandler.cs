using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IUserHandler
    {
        List<dtEmailAddress> GetUserEmailAddressDataList(doEmailSearchCondition cond);
        List<tbm_Employee> GetEmployeeByEmpNo(string employeeNo);
    }
}
