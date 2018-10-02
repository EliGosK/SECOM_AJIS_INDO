using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    [MetadataType(typeof(tbm_supplier_Meta))]
    public partial class tbm_Supplier
    {
        public string SupplierName { get; set; }
    }
}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    public class tbm_supplier_Meta
    {
        [LanguageMapping]
        public string SupplierName { get; set; }
    }
}
