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
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.Presentation.Installation.Models.MetaData;
using SECOM_AJIS.Presentation.Installation.Models;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Installation.Models
{
    //public class ISS010_ScreenParameter : ScreenParameter
    //{
    //    public string ContractCodeProjectCode { get; set; }
    //    public string ContractCode { get; set; }
    //    public string ProjectCode { get; set; }
    //    public string ServiceTypeCode { get; set; }

    //}
    /// <summary>
    /// DO of main data screen ISS010
    /// </summary>
    public class ISS010_RegisterData
    {
        public string ContractCodeProjectCode { get; set; }
        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public string ServiceTypeCode { get; set; }

    }
    /// <summary>
    /// DO for validate data
    /// </summary>
    public class ISS010_ValidateData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS010",
                        Parameter = "lblProposedInstallStartDate",
                        ControlName = "ProposeInstallStartDate")]
        public string ProposeInstallStartDate { get; set; }

        public string ProposeInstallCompleteDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS010",
                        Parameter = "lblCustomerStaffBelonging",
                        ControlName = "CustomerStaffBelonging")]
        public string CustomerStaffBelonging { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS010",
                        Parameter = "lblCustomerStaffName",
                        ControlName = "CustomerStaffName")]
        public string CustomerStaffName { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS010",
                        Parameter = "lblCustomerStaffTelNo",
                        ControlName = "CustomerStaffPhoneNo")]
        public string CustomerStaffPhoneNo { get; set; }

    }
    /// <summary>
    /// DO for send data to show in screen
    /// </summary>
    public class ISS010_RegisterStartResumeTargetData
    {
        public dtRentalContractBasicForInstall dtRentalContractBasic { get; set; }
        public dtProjectForInstall dtProject { get; set; }
        public dtSaleBasic dtSale { get; set; }
        public string ServiceTypeCode { get; set; }

        public string InstallType { get; set; }
        public string MaintenanceNo { get; set; }
        public string ContractProjectCodeForShow;
        public bool blnCheckCP12 { get; set; }
        public tbt_InstallationBasic doTbt_InstallationBasic { get; set; }
    } 
    /// <summary>
    /// DO of session parameter of screen ISS010
    /// </summary>
    public class ISS010_ScreenParameter : ScreenSearchParameter
    {
        public ISS010_RegisterStartResumeTargetData ISS010_Session { get; set; }
        public ISS010_DOEmailData DOEmail { get; set; }
        public List<ISS010_DOEmailData> ListDOEmail { get; set; }
        public string ServiceTypeCode { get; set; }
        [KeepSession]
        public string ContractProjectCodeShort { get; set; }
        public string ContractProjectCodeLong { get; set; }
        public string InstallType { get; set; }
        public string MaintenanceNo { get; set; }
        [KeepSession]
        public string strContractProjectCode { get; set; }
        //=========== Teerapong S. 15/10/2012 ==============
        public tbt_InstallationBasic doIB { get; set; }
    }

    [MetadataType(typeof(ISS010_DOEmailData_Meta))]
    public partial class ISS010_DOEmailData : dtEmailAddress
    {
    }



}

namespace SECOM_AJIS.Presentation.Installation.Models.MetaData
{
    public class ISS010_DOEmailData_Meta : ISS010_DOEmailData
    {
        [NotNullOrEmpty(ControlName = "EmailAddress", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Notify email")]
        public string EmailAddress { get; set; }
    }
}