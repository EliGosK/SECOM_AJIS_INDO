using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// DO for initial screen.
    /// </summary>
    public class IVS281_ScreenParameter : ScreenParameter
    {
        public string PendingDownloadFilePath { get; set; }
        public string PendingDownloadFileName { get; set; }
        public doIVS281SearchCondition LastSearchParam { get; set; }
    }

}
