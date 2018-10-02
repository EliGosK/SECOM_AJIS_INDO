using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of customer group
    /// </summary>
    public class dtCustomerGroup
    {
        private string strCustCode; public string CustCode { get { return this.strCustCode; } set { this.strCustCode = value; } }
        private string strGroupCode; public string GroupCode { get { return this.strGroupCode; } set { this.strGroupCode = value; } }
        private string strGroupNameEN; public string GroupNameEN { get { return this.strGroupNameEN; } set { this.strGroupNameEN = value; } }
        private string strGroupNameLC; public string GroupNameLC { get { return this.strGroupNameLC; } set { this.strGroupNameLC = value; } } 

    }
}
