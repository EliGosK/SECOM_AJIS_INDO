using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsIncidentDetailIn
    {
        public dtIncident dtIncident { get; set; }
        public tbt_IncidentHistory tbt_IncidentHistory { get; set; }
        public List<tbt_IncidentHistoryDetail> tbt_IncidentHistoryDetail { get; set; }
        public List<tbt_IncidentRole> tbt_IncidentRoleAdd { get; set; }
        public List<tbt_IncidentRole> tbt_IncidentRoleEdit { get; set; }
        public List<int> tbt_IncidentRoleDelete { get; set; }
    }
}
