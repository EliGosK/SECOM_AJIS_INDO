using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(doTbt_Project_MetaData))]
    public class doTbt_Project : tbt_Project
    {
        public string HeadSalesmanEmpFullName { get; set; }
        public string ProjectManagerEmpFullName { get; set; }
        public string ProjectSubManagerEmpFullName { get; set; }
        public string SecurityPlanningChiefEmpFullName { get; set; }
        public string InstallationChiefEmpFullName { get; set; }
        public string ProjectStatusName { get; set; }
        public string ProjectStatusCodeName
        {
            get
            {
                if (!CommonUtil.IsNullOrEmpty(this.ProjectStatus))
                    return this.ProjectStatus + ": " + this.ProjectStatusName;
                else
                    return "-";
            }
        }


    }
}
