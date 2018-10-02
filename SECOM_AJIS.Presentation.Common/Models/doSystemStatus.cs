using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Presentation.Common
{
    /// <summary>
    /// DO for store status of System
    /// </summary>
    [Serializable]
    public class doSystemStatus
    {
        public bool CompleteFlag { get; set; }
        public string SystemStatus { get; set; }
        public string NextResumeServiceDateTime { get; set; }
        public string NextSuspendServiceDateTime { get; set; }
        public string SystemStatusDisplayName { get; set; }
        
        // additional
        public string UpdateType { get; set; }
    }
}
