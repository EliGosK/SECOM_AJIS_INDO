using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Installation.MetaData;

namespace SECOM_AJIS.DataEntity.Installation
{
    [MetadataType(typeof(tbt_InstallationSlip_MetaData))]
    public partial class tbt_InstallationSlip
    {
        public string InstallFeeBillingTypeName { get; set; }
        public string InstallationTypeRentalName { get; set; }
        public string InstallationTypeSaleName { get; set; }
        public string SaleInstallationType
        {
            get 
            {
                return this.InstallationType;
            }
        }
        public string CauseReasonSecom
        {
            get
            {
                return this.CauseReason;
            }
        }
        public string CauseReasonCustomerName { get; set; }
        public string CauseReasonSecomName { get; set; }
        public string SlipIssueOfficeName { get; set; }
    }

}

namespace SECOM_AJIS.DataEntity.Installation.MetaData
{
    public class tbt_InstallationSlip_MetaData
    {
        [InstallFeeBillingTypeMappingAttribute("InstallFeeBillingTypeName")]
        public string InstallFeeBillingType { get; set; }

        [InstallRentalTypeMappingAttribute("InstallationTypeRentalName")]        
        public string InstallationType { get; set; }

        [InstallSaleTypeMappingAttribute("InstallationTypeSaleName")]
        public string SaleInstallationType { get; set; }

        [CauseReasonCustomerMappingAttribute("CauseReasonCustomerName")]
        public string CauseReason { get; set; }

        [CauseReasonSecomMappingAttribute("CauseReasonSecomName")]
        public string CauseReasonSecom { get; set; }
    }
}
