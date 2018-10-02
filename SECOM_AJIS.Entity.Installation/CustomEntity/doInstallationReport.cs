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
    public class doInstallationReport
    {
        public string SubcontractorCode { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_INSTALLATION,
            Screen = "ISS100",
            Parameter = "lblPaidDate",
            ControlName = "PaidDateFrom")]
        public DateTime? PaidDateFrom { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_INSTALLATION,
            Screen = "ISS100",
            Parameter = "lblPaidDate",
            ControlName = "PaidDateTo")]
        public DateTime? PaidDateTo { get; set; }
    }
}
