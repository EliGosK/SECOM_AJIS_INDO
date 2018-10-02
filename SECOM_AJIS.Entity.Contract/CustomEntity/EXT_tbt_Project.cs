
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract.Model
{
    [MetadataType(typeof(CTS230_tbt_ProjectMetaData))]
    public class tbt_Project_CTS230 : tbt_Project
    {

    }    
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{

    public class CTS230_tbt_ProjectMetaData
    {
        [NotNullOrEmpty(ControlName = "ProjectName", Controller = MessageUtil.MODULE_CONTRACT, Screen = "CTS230", Parameter = "lblProjectName")]
        public string ProjectName { get; set; }
        [NotNullOrEmpty(ControlName = "ProjectRepresentAddr", Controller = MessageUtil.MODULE_CONTRACT, Screen = "CTS230", Parameter = "lblProjectRepresentativeAddress")]
        public string ProjectAddress { get; set; }
        [NotNullOrEmpty(ControlName = "sysinHeadSalesmanEmpNo", Controller = MessageUtil.MODULE_CONTRACT, Screen = "CTS230", Parameter = "lblHeadSalesman")]
        public string HeadSalesmanEmpNo { get; set; }
        [NotNullOrEmpty(ControlName = "sysinProjectManagerEmpNo", Controller = MessageUtil.MODULE_CONTRACT, Screen = "CTS230", Parameter = "lblProjectManager")]
        public string ProjectManagerEmpNo { get; set; }
    }
    public class doTbt_Project_MetaData
    {
        [EmployeeMapping("HeadSalesmanEmpFullName")]
        public string HeadSalesmanEmpNo { get; set; }
        [EmployeeMapping("ProjectManagerEmpFullName")]
        public string ProjectManagerEmpNo { get; set; }
        [EmployeeMapping("ProjectSubManagerEmpFullName")]
        public string ProjectSubManagerEmpNo { get; set; }
        [EmployeeMapping("SecurityPlanningChiefEmpFullName")]
        public string SecurityPlanningChiefEmpNo { get; set; }
        [EmployeeMapping("InstallationChiefEmpFullName")]
        public string InstallationChiefEmpNo { get; set; }
        [ProjectStatusMapping("ProjectStatusName")]
        public string ProjectStatus { get; set; }

    }

}
