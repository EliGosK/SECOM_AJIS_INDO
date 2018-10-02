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
using SECOM_AJIS.DataEntity.Common;
using System.IO;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// DO for initial screen.
    /// </summary>
    public class IVS240_ScreenParameter : ScreenParameter
    {
        public Stream ResultStream { get; set; } //Add by Jutarat A. on 04122012
        public tbt_DocumentList ResultDocument { get; set; } //Add by Jutarat A. on 04122012
    }
}
