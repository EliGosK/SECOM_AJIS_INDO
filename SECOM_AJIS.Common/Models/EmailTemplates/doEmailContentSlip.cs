using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models.EmailTemplates
{
    /// <summary>
    /// DO for email content slip
    /// </summary>
    public class doEmailContentSlip : ATemplateObject
    {
               
        public string ContractCode { get; set; }
        public string ContractTargetNameLC	 { get; set; }
        public string SiteNameLC { get; set; }
        public string ProductNameLC { get; set; }
        public string InstallationMaintenanceNo { get; set; }
        public string InstallationTypeLC { get; set; }
        public string ChangeReasonTypeLC { get; set; }
        public string InstallationCauseReasonLC { get; set; }
        public string Salesman1NameLC { get; set; }
        public string Salesman2NameLC { get; set; }
        public string OperationOfficeNameLC { get; set; }
        public string SenderLC { get; set; }
        public string ContractTargetNameEN	 { get; set; }
        public string SiteNameEN { get; set; }
        public string ProductNameEN { get; set; }
        public string InstallationTypeEN { get; set; }
        public string ChangeReasonTypeEN { get; set; }
        public string InstallationCauseReasonEN { get; set; }
        public string Salesman1NameEN { get; set; }
        public string Salesman2NameEN { get; set; }
        public string OperationOfficeNameEN { get; set; }
        public string SenderEN { get; set; }


    }
}
