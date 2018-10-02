using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsIncidentDetail
    {
        public dtIncident dtIncident { get; set; }
        public List<dtIncidentRole> dtIncidentRole { get; set; }
        public List<dtIncidentHistory> dtIncidentHistory { get; set; }
        public List<dtEmployeeOffice> dtEmployeeOffice { get; set; }
        public List<tbt_IncidentHistoryDetail> Tbt_IncidentHistoryDetail { get; set; }
        public List<tbt_AttachFile> Tbt_AttachFile { get; set; }
    }
}
