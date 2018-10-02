using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of beloging
    /// </summary>
    public class doBelonging
    {
        private int intBelongingID; public int BelongingID { get { return this.intBelongingID; } set { this.intBelongingID = value; } }
        private string strOfficeCode; public string OfficeCode { get { return this.strOfficeCode; } set { this.strOfficeCode = value; } }
        private string strDepartmentCode; public string DepartmentCode { get { return this.strDepartmentCode; } set { this.strDepartmentCode = value; } }
        private string strEmpNo; public string EmpNo { get { return this.strEmpNo; } set { this.strEmpNo = value; } }
        private string strPositionCode; public string PositionCode { get { return this.strPositionCode; } set { this.strPositionCode = value; } }
        private DateTime datStartDate; public DateTime StartDate { get { return this.datStartDate; } set { this.datStartDate = value; } }
        private DateTime datEndDate; public DateTime EndDate { get { return this.datEndDate; } set { this.datEndDate = value; } }
        private bool bitMainDepartmentFlag; public bool MainDepartmentFlag { get { return this.bitMainDepartmentFlag; } set { this.bitMainDepartmentFlag = value; } }
        private bool bitPersonInChargeFlag; public bool PersonInChargeFlag { get { return this.bitPersonInChargeFlag; } set { this.bitPersonInChargeFlag = value; } }
        private DateTime datCreateDate; public DateTime CreateDate { get { return this.datCreateDate; } set { this.datCreateDate = value; } }
        private string strCreateBy; public string CreateBy { get { return this.strCreateBy; } set { this.strCreateBy = value; } }
        private DateTime datUpdateDate; public DateTime UpdateDate { get { return this.datUpdateDate; } set { this.datUpdateDate = value; } }
        private string strUpdateBy; public string UpdateBy { get { return this.strUpdateBy; } set { this.strUpdateBy = value; } }
    }
}
