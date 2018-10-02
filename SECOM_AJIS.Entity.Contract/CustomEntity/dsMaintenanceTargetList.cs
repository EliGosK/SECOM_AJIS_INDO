using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsMaintenanceTargetList
    {
        private List<doContractHeader> _doContractHeaderList;
        public List<doContractHeader> doContractHeaderList { get { return this._doContractHeaderList; } set { this._doContractHeaderList = value; } }
    }
}
