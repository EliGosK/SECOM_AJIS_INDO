using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class tbt_MaintenanceCheckup
    {
        public string MaintEmpName { get; set; }
        public string KeyIndex
        {
            get
            {
                return string.Format("{0}:{1}:{2}",
                    this.ContractCode,
                    this.ProductCode,
                    this.InstructionDate.ToString("yyyyMMdd"));
            }
        }
    }
}
