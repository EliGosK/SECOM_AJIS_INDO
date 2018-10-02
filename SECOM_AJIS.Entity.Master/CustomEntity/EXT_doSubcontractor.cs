using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of sub contractor
    /// </summary>
    [MetadataType(typeof(doSubcontractor_MetaData))]
    public partial class doSubcontractor
    {
        public string SubContractorName
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}",
                String.IsNullOrEmpty(this.SubContractorNameEN) ? "-" : this.SubContractorNameEN,
                String.IsNullOrEmpty(this.SubContractorNameLC) ? "-" : this.SubContractorNameLC);
            }
        }

        //public string SubInstallationFlagDisplay
        //{
        //    get { return this.SubInstallationFlag == true ? "Yes" : "No"; }
        //}

        //public string SubMaintenanceFlagDisplay
        //{
        //    get { return this.SubMaintenanceFlag == true ? "Yes" : "No"; }
        //}

        //Modify by Siripoj 12-06-2012

        public string SubInstallationFlagMapping
        {
            get { return (this.SubInstallationFlag == true) ? "1" : "0"; }
        }

        public string SubMaintenanceFlagMapping
        {
            get { return (this.SubMaintenanceFlag == true) ? "1" : "0"; }
        }

        public string SubInstallationFlagDisplay { get; set; }

        public string SubMaintenanceFlagDisplay { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    /// <summary>
    /// Do Of subcontractor meta data
    /// </summary>
    public class doSubcontractor_MetaData
    {
        [SubInstallationFlagMappingAttribute("SubInstallationFlagDisplay")]
        public string SubInstallationFlagMapping { get; set; }

        [SubMaintenanceFlagMappingAttribute("SubMaintenanceFlagDisplay")]
        public string SubMaintenanceFlagMapping { get; set; }

    }
}