using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of employee
    /// </summary>
    public class doEmployee
    {
        private string strEmpNo; public string EmpNo { get { return this.strEmpNo; } set { this.strEmpNo = value; } }
        private string strEmpFirstNameEN; public string EmpFirstNameEN { get { return this.strEmpFirstNameEN; } set { this.strEmpFirstNameEN = value; } }
        private string strEmpLastNameEN; public string EmpLastNameEN { get { return this.strEmpLastNameEN; } set { this.strEmpLastNameEN = value; } }
        private string strEmpFirstNameLC; public string EmpFirstNameLC { get { return this.strEmpFirstNameLC; } set { this.strEmpFirstNameLC = value; } }
        private string strEmpLastNameLC; public string EmpLastNameLC { get { return this.strEmpLastNameLC; } set { this.strEmpLastNameLC = value; } }
        private DateTime datStartDate; public DateTime StartDate { get { return this.datStartDate; } set { this.datStartDate = value; } }
        private DateTime datEndDate; public DateTime EndDate { get { return this.datEndDate; } set { this.datEndDate = value; } }
        private string strClass; public string Class { get { return this.strClass; } set { this.strClass = value; } }
        private string strGender; public string Gender { get { return this.strGender; } set { this.strGender = value; } }
        private string strEmailAddress; public string EmailAddress { get { return this.strEmailAddress; } set { this.strEmailAddress = value; } }
        private string strPhoneNo; public string PhoneNo { get { return this.strPhoneNo; } set { this.strPhoneNo = value; } }
        private bool booDeleteFlag; public bool DeleteFlag { get { return this.booDeleteFlag; } set { this.booDeleteFlag = value; } }
        private bool booIncidentNotificationFlag; public bool IncidentNotificationFlag { get { return this.booIncidentNotificationFlag; } set { this.booIncidentNotificationFlag = value; } }
        private DateTime datCreateDate; public DateTime CreateDate { get { return this.datCreateDate; } set { this.datCreateDate = value; } }
        private string strCreateBy; public string CreateBy { get { return this.strCreateBy; } set { this.strCreateBy = value; } }
        private DateTime datUpdateDate; public DateTime UpdateDate { get { return this.datUpdateDate; } set { this.datUpdateDate = value; } }
        private string strUpdateBy; public string UpdateBy { get { return this.strUpdateBy; } set { this.strUpdateBy = value; } }

    }
}
