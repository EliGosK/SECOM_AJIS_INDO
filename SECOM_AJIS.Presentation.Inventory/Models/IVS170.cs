using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Inventory;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Presentation.Inventory.Models.MetaData;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter for screen IVS170.
    /// </summary>
    public class IVS170_ScreenParameter : ScreenParameter
    {
        public string PendingDownloadFilePath { get; set; }
        public string PendingDownloadFileName { get; set; }
    }

    /// <summary>
    /// DO for validate stock checking search criteria.
    /// </summary>
    [MetadataType(typeof(IVS170_doGetStockCheckingList_MetaData))]
    public class IVS170_doGetStockCheckingList : doGetStockCheckingList
    { 
    }

}

namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class IVS170_doGetStockCheckingList_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                     Screen = "IVS170",
                     Parameter = "lblCheckingYearMonth",
                     ControlName = "CheckingYearMonth")]
        public string CheckingYearMonth { get; set; }
    }

}
