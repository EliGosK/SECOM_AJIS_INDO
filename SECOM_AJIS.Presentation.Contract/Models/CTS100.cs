using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter for Register data
    /// </summary>
    public class CTS100_RegisterStopTargetData
    {
        public CTS100_InitialRegisterStopTargetData InitialData { get; set; }
        public dsRentalContractData RegisterStopData { get; set; }
    }

    /// <summary>
    /// Parameter for Initial data
    /// </summary>
    public class CTS100_InitialRegisterStopTargetData
    {
        private string _contractCode;
        public string ContractCode
        {
            get { return String.IsNullOrEmpty(_contractCode) == false ? _contractCode.ToUpper() : string.Empty; }
            set { _contractCode = value; }
        }
        public string OCCCode { get; set; }
        public doRentalContractBasicInformation doRentalContractBasicData { get; set; }
        public int HasStopFee { get; set; }
        public tbt_RentalContractBasic RentalContractBasicData;
    }

    /// <summary>
    /// Parameter of CTS100 screen
    /// </summary>
    public class CTS100_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public CTS100_RegisterStopTargetData CTS100_Session { get; set; }

        public string ContractCode { get; set; }
    }

    /// <summary>
    /// DO for check authority of RentalContractBasic
    /// </summary>
    [MetadataType(typeof(CTS100_doRentalContractBasicAuthority_MetaData))]
    public class CTS100_doRentalContractBasicAuthority : tbt_RentalContractBasic
    {

    }

    /// <summary>
    /// DO of StopReason
    /// </summary>
    [MetadataType(typeof(CTS100_doStopReason_MetaData))]
    public class CTS100_doStopReason : tbt_RentalSecurityBasic
    {

    }

}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS100_doRentalContractBasicAuthority_MetaData
    {
        [OperationOfficeInRole(Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0063)]
        public string OperationOfficeCode { get; set; }
    }

    public class CTS100_doStopReason_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS100",
                Parameter = "lblStopDate",
                ControlName = "dpStopDate",
                Order = 1)]
        public Nullable<System.DateTime> ChangeImplementDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS100",
                Parameter = "lblStopReason",
                ControlName = "ddlStopReason",
                Order = 2)]
        public string StopCancelReasonType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS100",
                Parameter = "lblStopFee",
                ControlName = "txtStopFee",
                Order = 3)]
        public Nullable<decimal> ContractFeeOnStop { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS100",
                Parameter = "lblOperationDate",
                ControlName = "dpOperationDate",
                Order = 4)]
        public Nullable<System.DateTime> ExpectedResumeDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS100",
                Parameter = "lblApprove1",
                ControlName = "txtApproveNo1",
                Order = 5)]
        public string ApproveNo1 { get; set; }

        public string ApproveNo2 { get; set; }
    }
}
