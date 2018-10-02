using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models.EmailTemplates
{
    /// <summary>
    /// DO for email with URL
    /// </summary>
    public partial class doEmailWithURL : ATemplateObject
    {
        public string ViewURL { get; set; }
        public string ViewURLLC { get; set; }

        //Add by Jutarat A. on 11072013
        public string ARRelatedCode { get; set; }
        public string ARRequestNo { get; set; }
        public string ARTypeEN { get; set; }
        public string ARTypeLC { get; set; }
        public string ARTitleEN { get; set; }
        public string ARTitleLC { get; set; }
        public string ARSubtitle { get; set; }
        public string ARPurpose { get; set; }
        public string AuditDetailHistory { get; set; }
        //End Add

        //Add by Jutarat A. on 16072013
        public string IncidentRelatedCode { get; set; }
        public string IncidentTitle { get; set; }
        public string IncidentNo { get; set; }
        public string ContactDetail { get; set; }
        public string ReceivedDetail { get; set; }
        //End Add
    }
}
