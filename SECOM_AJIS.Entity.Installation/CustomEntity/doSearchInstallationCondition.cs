using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Installation
{
    [Serializable]
    public class doSearchInstallationCondition
    {

        public string ContractCode { get; set; }
        public string userCode { get; set; }
        public string planCode { get; set; }
        public string slipNo { get; set; }
        public string installationMaintenanceNo { get; set; }
        public string operationOffice { get; set; }
        public string salesmanEmpNo { get; set; }
        public Nullable<System.DateTime> slipIssueDateFrom { get; set; }
        public Nullable<System.DateTime> slipIssueDateTo { get; set; }
        public string contractTargetPurchaserName { get; set; }
        public string siteCode { get; set; }
        public string siteName { get; set; }
        public string siteAddress { get; set; }
        public string installationStatus { get; set; }
        public string slipStatus { get; set; }      
        public string managementStatus { set; get; }
        public Nullable<bool> slipNoNullFlag { set; get; }
        public Nullable<bool> ViewFlag { set; get; }
        public string InstallationBy { set; get; }
        public string subContractorName { get; set; }
        //check box ManagementStatus

        public string chkInstallationNotRegistered { get; set; }
        public string chkInstallationRequestedAndPoRegistered { get; set; }
        public string chkInstallationNotRequest { get; set; }
        public string chkInstallationUnderInstall { get; set; } //Add by Jutarat A. on 27032014
        public string chkInstallationCompleted { get; set; }
        public string chkInstallationRequestButPoNotRegistered { get; set; }
        public string chkInstallationCancelled { get; set; }

        //check box SlipStatus

        public string chkNotStockOut { get; set; }
        public string chkNoNeedToStockOut { get; set; }
        public string chkReturned { get; set; }
        public string chkPartialStockOut { get; set; }
        public string chkInstallationSlipCanceled { get; set; }
        public string chkNoNeedToReturn { get; set; }
        public string chkStockOut { get; set; }
        public string chkWaitForReturn { get; set; }
        public string chkReplaced { get; set; }

        //check box InstallationManagementStatus

        public string chkProcessing { get; set; }
        public string chkApproved { get; set; }
        public string chkCompleted { get; set; }
        public string chkRequestApprove { get; set; }
        public string chkRejected { get; set; }
        public string chkCanceled { get; set; }


        public Nullable<bool> NotRegisteredYetSlipFlag { get; set; }
        public Nullable<bool> NotRegisteredYetManagementFlag { get; set; }           

    }
}
