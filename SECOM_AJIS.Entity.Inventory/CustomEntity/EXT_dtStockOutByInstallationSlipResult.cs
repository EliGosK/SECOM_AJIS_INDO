using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory.MetaData;


namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(dtStockOutByInstallationSlipResult_Meta))]
    public partial class dtStockOutByInstallationSlipResult
    {
        public string SourceAreaName { get; set; }
        public string SourceArea
        {
            get
            {
                return string.Format("{0}: {1}", this.SourceAreaCode, this.SourceAreaName);
            }
        }
    }

}

namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class dtStockOutByInstallationSlipResult_Meta
    {
        [LanguageMapping]
        public string SourceAreaName { get; set; }
    }

}
