using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of permission
    /// </summary>
    [MetadataType(typeof(MAS080_Save_MetaData))]
    public class MAS080_Save : doPermission
    {
    }

    [MetadataType(typeof(MAS080_InsertIndividual_MetaData))]
    public class MAS080_InsertIndividual : doPermission
    {
    }
}

namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    /// <summary>
    /// Do Of validate for save MAS080
    /// </summary>
    public class MAS080_Save_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS080",
                        Parameter = "lblPermissionGroupName",
                        ControlName = "PermissionGroupName")]
        public string PermissionGroupName { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS080",
                        Parameter = "cboOffice",
                        ControlName = "OfficeCode")]
        public string OfficeCode { get; set; }
        //[NotNullOrEmpty(ControlName = "DepartmentCode")]
        //public string DepartmentCode { get; set; }
        //[NotNullOrEmpty(ControlName = "PositionCode")]
        //public string PositionCode { get; set; }
    }

    /// <summary>
    /// Do Of validate for insert individual MAS080
    /// </summary>
    public class MAS080_InsertIndividual_MetaData// : doPermission
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS080",
                        Parameter = "lblPermissionGroupName",
                        ControlName = "PermissionGroupName")]
        public string PermissionGroupName { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS080",
                        Parameter = "cboOffice",
                        ControlName = "OfficeCode")]
        public string OfficeCode { get; set; }
        //[NotNullOrEmpty(ControlName = "DepartmentCode")]
        //public string DepartmentCode { get; set; }
        //[NotNullOrEmpty(ControlName = "PositionCode")]
        //public string PositionCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS080",
                        Parameter = "lblPermissionIndividualName",
                        ControlName = "PermissionIndividualName")]
        public string PermissionIndividualName { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS080",
        //                Parameter = "lblEmployee",
        //                ControlName = "EmpNo")]
        //public string EmpNo { get; set; }
    }

}
