using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of customer group
    /// </summary>
    public partial class dtCustomeGroupData
    {
    }

    [MetadataType(typeof(dtCustomerGroupDataCondition_MetaData))]
    public class dtCustomerGroupDataCondition : dtCustomeGroupData
    {
    }
}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    /// <summary>
    /// Do Of customer group data condition
    /// </summary>
    public class dtCustomerGroupDataCondition_MetaData
    {
        [NotNullOrEmpty]
        public string CustCode { get; set; }
        [NotNullOrEmpty]
        public string GroupCode { get; set; }
    }
}