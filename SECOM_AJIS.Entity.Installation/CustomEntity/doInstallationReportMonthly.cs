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
    public class doInstallationReportMonthly
    {
        public string ReportType { get; set; }

        [CodeNotNullOtherNotNull("ReceiveDateTo",
            Controller = MessageUtil.MODULE_INSTALLATION,
            Screen = "ISS100",
            Parameter = "lblReceiveDate",
            ControlName = "ReceiveDateFrom")]
        public DateTime? ReceiveDateFrom { get; set; }

        [CodeNotNullOtherNotNull("ReceiveDateFrom",
            Controller = MessageUtil.MODULE_INSTALLATION,
            Screen = "ISS100",
            Parameter = "lblReceiveDate",
            ControlName = "ReceiveDateTo")]
        public DateTime? ReceiveDateTo { get; set; }

        [CodeNotNullOtherNotNull("CompleteDateTo",
            Controller = MessageUtil.MODULE_INSTALLATION,
            Screen = "ISS100",
            Parameter = "lblCompleteDate",
            ControlName = "CompleteDateFrom")]
        public DateTime? CompleteDateFrom { get; set; }

        [CodeNotNullOtherNotNull("CompleteDateFrom",
            Controller = MessageUtil.MODULE_INSTALLATION,
            Screen = "ISS100",
            Parameter = "lblCompleteDate",
            ControlName = "CompleteDateTo")]
        public DateTime? CompleteDateTo { get; set; }

        [CodeNotNullOtherNotNull("ExpectedStartDateTo",
            Controller = MessageUtil.MODULE_INSTALLATION,
            Screen = "ISS100",
            Parameter = "lblExpectedStartDate",
            ControlName = "ExpectedStartDateFrom")]
        public DateTime? ExpectedStartDateFrom { get; set; }

        [CodeNotNullOtherNotNull("ExpectedStartDateFrom",
            Controller = MessageUtil.MODULE_INSTALLATION,
            Screen = "ISS100",
            Parameter = "lblExpectedStartDate",
            ControlName = "ExpectedStartDateTo")]
        public DateTime? ExpectedStartDateTo { get; set; }

        [CodeNotNullOtherNotNull("ExpectedCompleteDateTo",
            Controller = MessageUtil.MODULE_INSTALLATION,
            Screen = "ISS100",
            Parameter = "lblExpectedCompleteDate",
            ControlName = "ExpectedCompleteDateFrom")]
        public DateTime? ExpectedCompleteDateFrom { get; set; }

        [CodeNotNullOtherNotNull("ExpectedCompleteDateFrom",
            Controller = MessageUtil.MODULE_INSTALLATION,
            Screen = "ISS100",
            Parameter = "lblExpectedCompleteDate",
            ControlName = "ExpectedCompleteDateTo")]
        public DateTime? ExpectedCompleteDateTo { get; set; }

        public string ContractCode { get; set; }
        public string SiteName { get; set; }
        public string SubContractorCode { get; set; }
        public string ProductName { get; set; }
        public string InstallationStatus { get; set; }
        public string BuildingType { get; set; }

    }
}