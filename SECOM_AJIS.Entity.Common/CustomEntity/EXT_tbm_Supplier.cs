using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Common
{
    [MetadataType(typeof(tbm_Supplier_MetaData))]
    public partial class tbm_Supplier
    {
        public string SupplierName { get; set; }
    }

    public class tbm_Supplier_MetaData
    {
        [LanguageMapping]
        public string SupplierName { get; set; }
    }
}
