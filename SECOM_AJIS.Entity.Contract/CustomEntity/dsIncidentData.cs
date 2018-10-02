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
    public class dsIncidentData
    {
        public tbt_Incident tbt_Incident { get; set; }
        public List<tbt_IncidentRole> tbt_IncidentRole { get; set; }
    }
}
