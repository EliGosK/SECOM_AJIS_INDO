using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Common
{
    [MetadataType(typeof(doEmailProcess_MetaData))]
    public class doEmailProcess
    {
        public virtual string MailTo { get; set; }
        public virtual string MailFrom { get; set; }
        public virtual string MailFromAlias { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Message { get; set; }
        public virtual List<string> AttachFileList { get; set; }

        public virtual bool? IsBodyHtml { get; set; } //Add by Jutarat A. on 15072013
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class doEmailProcess_MetaData
    {
        [NotNullOrEmpty]
        public string MailTo { get; set; }
        [NotNullOrEmpty]
        public string Subject { get; set; }
        [NotNullOrEmpty]
        public string Message { get; set; }
    }
}
