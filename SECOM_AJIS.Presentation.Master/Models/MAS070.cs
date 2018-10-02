using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Presentation.Master.Models.MetaData;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Master.Models
{
    /// <summary>
    /// Parameter for screen MAS070.
    /// </summary>
    public class MAS070_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public bool hasPermissionAdd { get; set; }
        [KeepSession]
        public bool hasPermissionEdit { get; set; }
        [KeepSession]
        public bool hasPermissionDelete { get; set; }
        public bool isReactivate { get; set; }
        public string reactivateEmpNo { get; set; }
        [KeepSession]
        public DateTime? updateDate { get; set; }
        public List<dtBelonging> belongingList { get; set; }
    }

    /// <summary>
    /// DO for stored search condition of search employee.
    /// </summary>
    public class MAS070_EmployeeSearchCondition {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string txtEmployeeCodeSearch { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string txtEmployeeNameSearch { get; set; }
    }

    /// <summary>
    /// DO for stored belonging information use in save belonging process.
    /// </summary>
    [MetadataType(typeof(MAS070_SaveBelonging_MetaData))]
    public class MAS070_SaveBelonging : tbm_Belonging {
        public List<View_tbm_Belonging> delBelList { get; set; }
    }

    /// <summary>
    /// DO of data for save employee
    /// </summary>
    [MetadataType(typeof(MAS070_SaveEmployee_MetaData))]
    public class MAS070_SaveEmployee : tbm_Employee
    {
        public string ModifyMode { get; set; }

        public bool ChangePasswordFlag { get; set; }
    }
}


namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    public class MAS070_SaveBelonging_MetaData {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS070",
                        Parameter = "lblOffice", 
                        ControlName = "OfficeCode")]
        public string OfficeCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS070",
                        Parameter = "lblDepartment", 
                        ControlName = "DepartmentCode")]
        public string DepartmentCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS070",
                        Parameter = "lblPosition", 
                        ControlName = "PositionCode")]
        public string PositionCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS070",
                        Parameter = "lblValidPeriodFrom",
                        ControlName = "ValidFrom")]
        public Nullable<System.DateTime> StartDate { get; set; }
    }

    public class MAS070_SaveEmployee_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS070",
                        Parameter = "lblEmployeeCode", 
                        ControlName = "EmpNo")]
        public string EmpNo { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS070",
                        Parameter = "lblFirstNameEng", 
                        ControlName = "EmpFirstNameEN")]
        public string EmpFirstNameEN { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS070",
                        Parameter = "lblLastNameEng", 
                        ControlName = "EmpLastNameEN")]
        public string EmpLastNameEN { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS070",
        //                Parameter = "lblFirstNameLC", 
        //                ControlName = "EmpFirstNameLC")]
        //public string EmpFirstNameLC { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS070",
        //                Parameter = "lblLastnameLC", 
        //                ControlName = "EmpLastNameLC")]
        //public string EmpLastNameLC { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS070",
                        Parameter = "lblStartDate",
                        ControlName = "StartDate")]
        public Nullable<System.DateTime> StartDate { get; set; }

        
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS070",
                        Parameter = "lblPassword",
                        ControlName = "Password")]
        public string Password { get; set; }
    }

}

