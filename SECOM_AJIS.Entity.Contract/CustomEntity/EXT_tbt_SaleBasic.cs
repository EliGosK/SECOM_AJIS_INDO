using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_SaleBasic_MetaData))]
    public partial class tbt_SaleBasic
    {
        public string PlannerName { get; set; }
        public string PlanCheckerName { get; set; }
        public string PlanApproverName { get; set; }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_SaleBasic_MetaData
    {
        [EmployeeExist]
        [EmployeeMapping("PlannerName")]
        public string PlannerEmpNo { get; set; }
        [EmployeeExist]
        [EmployeeMapping("PlanCheckerName")]
        public string PlanCheckerEmpNo { get; set; }
        [EmployeeExist]
        [EmployeeMapping("PlanApproverName")]
        public string PlanApproverEmpNo { get; set; }
    }
}

