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
    [MetadataType(typeof(tbt_InstallationBasic_MetaData))]
    public partial class tbt_InstallationBasic
    {
        public string InstallationTypeName
        {
            get;
            set;
        }
        public string InstallationByName
        {
            get;
            set;
        }
        public string InstallationTypeRentalName { get; set; }
        public string InstallationTypeSaleName { get; set; }
        public string SaleInstallationType
        {
            get
            {
                return this.InstallationType;
            }
        }
        
    }
}

namespace SECOM_AJIS.DataEntity.Installation.MetaData
{
    public class tbt_InstallationBasic_MetaData
    {
        [InstallationByMappingAttribute("InstallationByName")]
        public string InstallationBy { get; set; }

        [InstallRentalTypeMappingAttribute("InstallationTypeRentalName")]
        public string InstallationType { get; set; }

        [InstallSaleTypeMappingAttribute("InstallationTypeSaleName")]
        public string SaleInstallationType { get; set; }
    }
}
