using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract.Meta;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_ProjectStockOutMemo_Meta))]
    public partial class tbt_ProjectStockOutMemo
    {
        public string CreateByName { get; set; }
        public string UpdateByName { get; set; }
      
    }
}
namespace SECOM_AJIS.DataEntity.Contract.Meta
{
    public partial class tbt_ProjectStockOutMemo_Meta
    {
        [EmployeeMapping("CreateByName")]
        public string CreateBy { get; set; }
        [EmployeeMapping("UpdateByName")]
        public string UpdateBy { get; set; }

    }


}