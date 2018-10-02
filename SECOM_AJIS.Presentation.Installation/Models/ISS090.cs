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
    /// DO of register code
    /// </summary>
    public class ISS090_RegisterData
    {
        public string ContractCodeProjectCode { get; set; }
        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public string ServiceTypeCode { get; set; }

    }
    /// <summary>
    /// DO of data input from screen
    /// </summary>
    public class ISS090_ScreenInput
    {
        public string IEStaffEmpNo1 { get; set; }
        public string IEStaffEmpNo2 { get; set; }
        public string POMemo { get; set; }
    }

    /// <summary>
    /// DO for send to show in screen
    /// </summary>
    public class ISS090_RegisterStartResumeTargetData
    {
        public dtRentalContractBasicForInstall dtRentalContractBasic { get; set; }
        public dtProjectForInstall dtProject { get; set; }
        public dtSaleBasic dtSale { get; set; }
        public string ServiceTypeCode { get; set; }

        public string InstallType { get; set; }
        public string MaintenanceNo { get; set; }
       
        public tbt_InstallationBasic dtInstallationBasic { get; set; }
        public tbt_InstallationManagement dtInstallationManagement { get; set; }

        public List<ISS090_DOEmailData> ListDOEmail { get; set; }
        public List<ISS090_DOEmailData> ListApproveEmail { get; set; }
        public List<tbt_InstallationPOManagement> ListPOInfo { get; set; }

        public string strMode { get; set; }

        public string[] arrayPOName { get; set; }
        public decimal TotalLastInstallFee { get; set; }
        public string strMemoHistory { get; set; }
        public List<ContractCodeList> doContractCodeListByProject { get; set; }

        public bool blnHavetbtInstallationBasic { get; set; }

        public Nullable<decimal> NormalInstallationFee { get; set; } //Add by Jutarat A. on 31072013
        public Nullable<decimal> BillingInstallationFee { get; set; } //Add by Jutarat A. on 31072013
    }

    //public class ISS090_RegisterInstallationPOInformation
    //{
    //    public string SubcontractorCode  { get; set; }
    //    public string SubcontractorName { get; set; }
    //    public string SubcontractorGroupCode { get; set; }
    //    public string SubcontractorGroupName { get; set; }
    //    public Nullable<decimal> NormalSubcontractorAmount { get; set; }
    //    public Nullable<decimal> ActualPOAmount { get; set; }
    //    public string PONo { get; set; }
    //    public DateTime ExpectedInstallationStartDate { get; set; }
    //    public DateTime ExpectedInstallationCompleteDate { get; set; }
       
    //} 
    /// <summary>
    /// DO of session parameter for screen ISS090
    /// </summary>
    public class ISS090_ScreenParameter : ScreenParameter
    {
        public ISS090_RegisterStartResumeTargetData ISS090_Session { get; set; }
        public ISS090_DOEmailData DOEmail { get; set; }
        public List<ISS090_DOEmailData> ListDOEmail { get; set; }
        public List<ISS090_DOEmailData> ListApproveEmail { get; set; }
        public string ServiceTypeCode { get; set; }
        [KeepSession]
        public string ContractProjectCodeShort { get; set; }
        public string ContractProjectCodeLong { get; set; }
        public string InstallType { get; set; }
        [KeepSession]
        public string MaintenanceNo { get; set; }
        [KeepSession]
        public string strContractProjectCode { get; set; }
       
        //public List<ISS090_RegisterInstallationPOInformation> ListPOInfo { get; set; } // FROM SCREEN
        public List<tbt_InstallationPOManagement> ListPOInfo { get; set; } // FROM DB
        public List<POCodeName> ListPOName { get; set; }
        public List<tbt_InstallationPOManagement> OldListPOInfo { get; set; } // FROM DB
        public tbt_InstallationBasic dtInstallationBasic { get; set; }
        public tbt_InstallationManagement dtInstallationManagement { get; set; }
        public dtRentalContractBasicForInstall dtRentalContractBasic { get; set; }
        public dtSaleBasic dtSaleContractBasic { get; set; }
        

        public string[] arrayPOName { get; set; }
        public decimal TotalLastInstallFee { get; set; }
        public string strMemoHistory { get; set; }
    }

  
    /// <summary>
    /// DO of email data
    /// </summary>
    [MetadataType(typeof(ISS090_DOEmailData_Meta))]
    public partial class ISS090_DOEmailData : dtEmailAddress
    {
    }



}

namespace SECOM_AJIS.Presentation.Installation.Models.MetaData
{
    public class ISS090_DOEmailData_Meta : ISS090_DOEmailData
    {
        [NotNullOrEmpty(ControlName = "EmailAddress", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Notify email")]
        public string EmailAddress { get; set; }
    }
}