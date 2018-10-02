using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
//using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Presentation.Income.Models.MetaData;
namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Parameter of ICS150 screen
    /// </summary>
    public class ICS150_ScreenParameter : ScreenParameter
    {
        public string PendingDownloadFilePath { get; set; }
        public string PendingDownloadFileName { get; set; }
        public doMatchRReport SearchReportParam { get; set; }
        public List<doMatchRReport> SearchReportParamList { set; get; }

    }

}

