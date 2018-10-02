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
using SECOM_AJIS.Presentation.Inventory.Models.SECOM_AJIS.Presentation.Inventory.Models.MetaData;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// DO for initial screen.
    /// </summary>
    public class IVS288_ScreenParameter : ScreenParameter
    {
        public string PendingDownloadFilePath { get; set; }
        public string PendingDownloadFileName { get; set; }
        public doIVS288SearchCondition LastSearchParam { get; set; }
    }

    /// <summary>
    /// DO for validate Checking Detail search condition.
    /// </summary>
    [MetadataType(typeof(IVS288_ScreenParameter_MetaData))]
    public class IVS288_SearchCondition : doIVS288SearchCondition
    {

    }

    namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
    {
        public class IVS288_ScreenParameter_MetaData
        {
            [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                         Screen = "IVS288",
                         Parameter = "lblSourceArea",
                         ControlName = "cboSourceArea")]
            public string SourceAreaCode { get; set; }
            [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                         Screen = "IVS288",
                         Parameter = "lblDestinationArea",
                         ControlName = "cboDestinationArea")]
            public string DestinationAreaCode { get; set; }
        }
    }

}
