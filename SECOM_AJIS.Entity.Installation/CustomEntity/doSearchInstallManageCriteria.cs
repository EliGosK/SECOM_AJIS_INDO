using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Installation
{
    public class doSearchInstallManageCriteria
    {

        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public string InstallationType { get; set; }
        public string IEStaffCode { get; set; }    
        public string SubcontractorCode { get; set; }
        public string SubcontractorGroupName { get; set; }
        public DateTime? ProposedInstallationCompleteDateFrom { get; set; }
        public DateTime? ProposedInstallationCompleteDateTo { get; set; }
        public DateTime? InstallationCompleteDateFrom { get; set; }
        public DateTime? InstallationCompleteDateTo { get; set; }
        public DateTime? InstallationStartDateFrom { get; set; }
        public DateTime? InstallationStartDateTo { get; set; }
        public DateTime? InstallationFinishDateFrom { get; set; }
        public DateTime? InstallationFinishDateTo { get; set; }
        public DateTime? ExpectedInstallationStartDateFrom { get; set; }
        public DateTime? ExpectedInstallationStartDateTo { get; set; }
        public DateTime? ExpectedInstallationFinishDateFrom { get; set; }
        public DateTime? ExpectedInstallationFinishDateTo { get; set; }
        public string SiteName { get; set; }
        public string SiteAddress { get; set; }
        public string OperationOfficeCode { get; set; }
        public string InstallationManagementStatus { get; set; }

        public DateTime? InstallationRequestDateFrom { get; set; } //Add by Jutarat A. on 22102013
        public DateTime? InstallationRequestDateTo { get; set; } //Add by Jutarat A. on 22102013
    }
}
