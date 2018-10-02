using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_DraftRentalMaintenanceDetails_MetaData))]
    public partial class tbt_DraftRentalMaintenanceDetails
    {
        public string MaintenanceTargetProductTypeName { get; set; }
        public string MaintenanceTargetProductTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.MaintenanceTargetProductTypeCode, this.MaintenanceTargetProductTypeName);
            }
        }
        public string MaintenanceTypeName { get; set; }
        public string MaintenanceTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.MaintenanceTypeCode, this.MaintenanceTypeName);
            }
        }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_DraftRentalMaintenanceDetails_MetaData
    {
        [MaintenanceTargetProductTypeMapping("MaintenanceTargetProductTypeName")]
        public string MaintenanceTargetProductTypeCode { get; set; }
        [MaintenanceTypeMapping("MaintenanceTypeName")]
        public string MaintenanceTypeCode { get; set; }
    }
}