using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Common.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Common
{

    public partial class dtDocumentListForPrining
    {

        public string FilePathDocument
        {
            get 
            {
                return PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, FilePath); //ReportUtil.GetGeneratedReportPath(FilePath);
            }
        }
        
    }
}



