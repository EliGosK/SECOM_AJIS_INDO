using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models.EmailTemplates
{
    /// <summary>
    /// DO for email content request approve
    /// </summary>
    public class doEmailContentRequestApprrove : ATemplateObject
    {
        public string ContractProjectCode { get; set; }
        public string MaintenanceNo  { get; set; }
        public string InstallationTypeNameLC { get; set; }
        public string InstallationTypeNameEN { get; set; }
        public string ChangeReasonNameLC { get; set; }
        public string ChangeReasonNameEN { get; set; }
        public string InstallationCauseReasonNameLC { get; set; }
        public string InstallationCauseReasonNameEN { get; set; }
        public string IEStaff1NameLC { get; set; }
        public string IEStaff1NameEN { get; set; }
        public string IEStaff2NameLC { get; set; }
        public string IEStaff2NameEN { get; set; }
        public string SenderLC  { get; set; }
        public string SenderEN  { get; set; }
        public string SubcontractorListLC { get; set; }
        public string SubcontractorListEN { get; set; }
        public string ContractTargetNameLC  { get; set; }
        public string ContractTargetNameEN  { get; set; }
        public string SiteNameLC  { get; set; }
        public string SiteNameEN  { get; set; }
        public string ProductNameLC { get; set; }
        public string ProductNameEN { get; set; }
        public string OperationOfficeNameLC { get; set; }
        public string OperationOfficeNameEN { get; set; }       
    }
}
