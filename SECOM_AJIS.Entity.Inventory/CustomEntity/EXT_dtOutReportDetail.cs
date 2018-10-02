using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(dtOutReportDetail_Meta))]
    public partial class dtOutReportDetail
    {
        public string SiteName { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class dtOutReportDetail_Meta
    {
        [LanguageMapping]
        public string SiteName { get; set; }
    }
}
