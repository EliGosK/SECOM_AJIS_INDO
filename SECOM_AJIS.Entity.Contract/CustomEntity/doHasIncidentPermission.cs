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
    public class doHasIncidentPermission
    {
        public bool AssignChiefFlag { get; set; }
        public bool AssignCorrespondentFlag { get; set; }
        public bool AssignAssistantFlag { get; set; }
        public bool ViewConfidentialIncidentFlag { get; set; }
        public bool EditIncidentFlag { get; set; }
        public bool EditConfidentailIncidentFlag { get; set; }
        public string[] IncidentInteractionTypeList { get; set; }
    }
}
