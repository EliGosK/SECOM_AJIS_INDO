using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Common.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Installation.Models.MetaData;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.Presentation.Installation.Models;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Installation.Models
{

    /// <summary>
    /// DO of main data of screen ISS050 
    /// </summary>
    public class ISS050_RegisterData
    {
        public string ContractCodeProjectCode { get; set; }
        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public string ServiceTypeCode { get; set; }

    }
    /// <summary>
    /// DO of data from screen
    /// </summary>
    public class ISS050_ScreenInput
    {
        public string IEStaffEmpNo1 { get; set; }
        public string IEStaffEmpNo2 { get; set; }
        public string POMemo { get; set; }
    }

    /// <summary>
    /// DO for send data to show in screen
    /// </summary>
    public class ISS050_RegisterStartResumeTargetData
    {
        public dtRentalContractBasicForInstall dtRentalContractBasic { get; set; }
        public dtProjectForInstall dtProject { get; set; }
        public dtSaleBasic dtSale { get; set; }
        public string ServiceTypeCode { get; set; }

        public string InstallType { get; set; }
        public string MaintenanceNo { get; set; }
       
        public tbt_InstallationBasic dtInstallationBasic { get; set; }
        public tbt_InstallationManagement dtInstallationManagement { get; set; }

        public List<ISS050_DOEmailData> ListDOEmail { get; set; }
        public List<tbt_InstallationPOManagement> ListPOInfo { get; set; }

        public string[] arrayPOName { get; set; }
        public string ContractProjectCodeForShow { get; set; }
    }
    /// <summary>
    /// DO of session parameter of screen ISS050
    /// </summary>
    public class ISS050_ScreenParameter : ScreenSearchParameter
    {
        public ISS050_RegisterStartResumeTargetData ISS050_Session { get; set; }
        public ISS050_DOEmailData DOEmail { get; set; }
        public List<ISS050_DOEmailData> ListDOEmail { get; set; }
        
        public string ServiceTypeCode { get; set; }
        [KeepSession]
        public string ContractProjectCodeShort { get; set; }
        public string ContractProjectCodeLong { get; set; }
        public string InstallType { get; set; }
        [KeepSession]
        public string MaintenanceNo { get; set; }
       
        //public List<ISS050_RegisterInstallationPOInformation> ListPOInfo { get; set; } // FROM SCREEN
        public List<tbt_InstallationPOManagement> ListPOInfo { get; set; } // FROM DB
        public List<POCodeName> ListPOName { get; set; }
        public List<tbt_InstallationPOManagement> OldListPOInfo { get; set; } // FROM DB
        public tbt_InstallationBasic dtInstallationBasic { get; set; }
        public tbt_InstallationManagement dtInstallationManagement { get; set; }

        public string[] arrayPOName { get; set; }
        [KeepSession]
        public string strContractProjectCode { get; set; }
        
    }
    /// <summary>
    /// DO of subcontractor name
    /// </summary>
    public class POCodeName
    {
        public string SubcontractorName { get; set; }
    }

    [MetadataType(typeof(ISS050_DOEmailData_Meta))]
    public partial class ISS050_DOEmailData : dtEmailAddress
    {
    }



}

namespace SECOM_AJIS.Presentation.Installation.Models.MetaData
{
    public class ISS050_DOEmailData_Meta : ISS050_DOEmailData
    {
        [NotNullOrEmpty(ControlName = "EmailAddress", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Notify email")]
        public string EmailAddress { get; set; }
    }
}