using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Common.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Installation.Models.MetaData;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.Presentation.Installation.Models;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Installation.Models
{

    public class ISS100_ScreenParameter : ScreenParameter
    {

        public string PendingDownloadFilePath { get; set; }
        public string PendingDownloadFileName { get; set; }
        public doInstallationReport LastSearchParam { get; set; }
        public doInstallationReportMonthly SearchInstallationMonthlyParam { get; set; }
    }


}
