using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class tbt_ProjectStockoutBranchIntrumentDetails
    {
        public string ProjectCodeBranchNo { get { return this.ProjectCode + ":" + this.BranchNo; } }
    }
}
