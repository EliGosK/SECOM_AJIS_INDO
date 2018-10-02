using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class doIncidentListByRole
    {
        public string incidentRole { get; set; }
        public string empNo { get; set; }
        public Nullable<System.DateTime> dueDate { get; set; }
        public string incidentStatus { get; set; }
    }
}
