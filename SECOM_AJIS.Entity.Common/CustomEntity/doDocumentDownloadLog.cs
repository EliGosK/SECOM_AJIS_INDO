using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using System.Reflection;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;



using SECOM_AJIS.DataEntity.Common.MetaData;

namespace SECOM_AJIS.DataEntity.Common
{

    [MetadataType(typeof(doDocumentDownloadLog_MetaData))]
    public class doDocumentDownloadLog : tbt_DocumentDownloadLog
    {
        public string DocumentOCC { set; get; }
    }
}

namespace SECOM_AJIS.DataEntity.Common.MetaData
{
    public class doDocumentDownloadLog_MetaData
    {
        [NotNullOrEmpty]
        public string DocumentNo { set; get; }
        [NotNullOrEmpty]
        public string DocumentCode { set; get; }
        [NotNullOrEmpty]
        public string DownloadBy { set; get; }
    }
}
