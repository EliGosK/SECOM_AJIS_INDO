using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Common
{
    public class doBatchProcess
    {
        public string BatchCode { get; set; }
        public string BatchName { get; set; }
        public string BatchDescription { get; set; }
        public string BatchLastResult { get; set; }
        public string BatchStatus { get; set; }
        public int? Total { get; set; }
        public int? Complete { get; set; }
        public int? Failed { get; set; }
        public string BatchJobName { get; set; }
        public DateTime? BatchDate { get; set; }
        public string BatchUser { get; set; }
        public string EnableRun { get; set; }
        public string dbms_job_status { get; set; }
        
        // additional
        public bool isRunAll { get; set; }

    }
}
