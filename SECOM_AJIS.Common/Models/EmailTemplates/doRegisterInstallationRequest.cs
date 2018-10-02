using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models.EmailTemplates
{
    /// <summary>
    /// DO for email content slip
    /// </summary>
    public class doRegisterInstallationRequest : ATemplateObject
    {

        public string ContractProjectCode { get; set; }
        public string UserCode { get; set; }
        public string SecurityTypeCode { get; set; }
        public string ProductNameLC { get; set; }
        public string ContractPurchaserName { get; set; }
        public string BranchNameLC { get; set; }
        public string SiteNameLC { get; set; }
        public string AddressFullLC { get; set; }
        public string OfficeNameLC { get; set; }
        public string MaintenanceNo { get; set; }
        public string PlanCode { get; set; }
        public string InstallationTypeName { get; set; }
        public string Salesman1 { get; set; }
        public string Salesman2 { get; set; }
        public string BuildingTypeName { get; set; }
        public string NewBuildingManagementType { get; set; }
        public string ProposeInstallStartDate { get; set; }
        public string ProposeInstallCompleteDate { get; set; }
        public string NewPhoneLineOpenDate { get; set; }
        public string NewConnectionPhoneNo { get; set; }
        public string NewPhoneLineOwnerTypeName { get; set; }
        public string RequestMemo { get; set; }
        
    }
}
