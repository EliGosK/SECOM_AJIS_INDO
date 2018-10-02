
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(dtTbt_ProjectSupportStaffDetailForView_metaData))]
    public partial class dtTbt_ProjectSupportStaffDetailForView
    {
        public string OfficeName { get; set; }
        public string EmpFullName { get; set; }
        public string Belonging { get { return this.OfficeName + " " + this.DepartmentName; } }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class dtTbt_ProjectSupportStaffDetailForView_metaData
    {
        [LanguageMapping]
        public string OfficeName { get; set; }
        [EmployeeMapping("EmpFullName")]
        public string EmpNo { get; set; }
    }
}