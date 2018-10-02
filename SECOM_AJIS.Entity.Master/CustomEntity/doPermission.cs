using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of permission
    /// </summary>
    public class doPermission
    {
	    public bool? TypeOffice {get; set;}
        public bool? TypeIndividual {get; set;}
        public string PermissionGroupCode { get; set; }
        public string PermissionGroupName {get; set;}
        public string PermissionIndividualCode {get; set;}
        public string PermissionIndividualName {get; set;}
        public string OfficeCode {get; set;}
        public string DepartmentCode {get; set;}
        public string PositionCode {get; set;}
        public string EmpNo { get; set; }
        public string DelEmpNo { get; set; }
        public DataTable dtEmpNo {get; set;}
        public string ObjectFunction {get; set;}
        public DataTable dtObjectFunction { get; set; }
        public DateTime? CreateDate;
        public string CreateBy {get; set;}
        public DateTime? UpdateDate {get; set;}
        public string UpdateBy { get; set; }
    }
}
