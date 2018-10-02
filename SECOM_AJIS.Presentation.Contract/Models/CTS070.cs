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
    public class CTS070_RegisterStartResumeTargetData
    {
        public CTS070_InitialRegisterStartResumeTargetData InitialData { get; set; }
        public dsRentalContractData RegisterStartResumeData { get; set; }
    }

    /// <summary>
    /// Parameter for Initial data
    /// </summary>
    public class CTS070_InitialRegisterStartResumeTargetData
    {
        private string _contractCode;
        public string ContractCode
        {
            get { return String.IsNullOrEmpty(_contractCode) == false ? _contractCode.ToUpper() : string.Empty; }
            set { _contractCode = value; }
        }
        public string OCCCode { get; set; }
        public doRentalContractBasicInformation doRentalContractBasicData { get; set; }
        public tbt_RentalContractBasic RentalContractBasicData;
    }

    /// <summary>
    /// Parameter of CTS070 screen
    /// </summary>
    public class CTS070_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public CTS070_RegisterStartResumeTargetData CTS070_Session { get; set; }

        public string ContractCode { get; set; }
    }

    /// <summary>
    /// DO of StartResume Service 
    /// </summary>
    public class CTS070_doStartResumeService
    {
        public string StartType { get; set; }
        public Nullable<System.DateTime> StartResumeOperationDate { get; set; }
        public string ApproveNo { get; set; }
        public Nullable<System.DateTime> AdjustBillingTerm { get; set; }
        public string UserCode { get; set; }

        public string LineTypeNormal { get; set; }
        public string TelephoneOwnerNormal { get; set; }
        public string TelephoneNoNormal { get; set; }
        public string LineTypeImage { get; set; }
        public string TelephoneOwnerImage { get; set; }
        public string TelephoneNoImage { get; set; }
        public string LineTypeDisconnection { get; set; }
        public string TelephoneOwnerDisconnection { get; set; }
        public string TelephoneNoDisconnection { get; set; }
    }

    /// <summary>
    /// DO for check authority of RentalContractBasic 
    /// </summary>
    [MetadataType(typeof(CTS070_doRentalContractBasicAuthority_MetaData))]
    public class CTS070_doRentalContractBasicAuthority : tbt_RentalContractBasic
    {

    }

    /// <summary>
    /// DO for validate StartResume Service
    /// </summary>
    [MetadataType(typeof(CTS070_ValidateStartResumeService_MetaData))]
    public class CTS070_ValidateStartResumeService : CTS070_doStartResumeService
    {

    }

    /// <summary>
    /// DO for validate StartType NewStart
    /// </summary>
    [MetadataType(typeof(CTS070_ValidateStartTypeNewStart_MetaData))]
    public class CTS070_ValidateStartTypeNewStart : CTS070_doStartResumeService
    {

    }

    /// <summary>
    /// DO for validate StartType AlterStart
    /// </summary>
    [MetadataType(typeof(CTS070_ValidateStartTypeAlterStart_MetaData))]
    public class CTS070_ValidateStartTypeAlterStart : CTS070_doStartResumeService
    {

    }

    //Add by Jutarat A. on 15082012
    /// <summary>
    /// DO for validate StartType Resume
    /// </summary>
    [MetadataType(typeof(CTS070_ValidateStartTypeResume_MetaData))]
    public class CTS070_ValidateStartTypeResume : CTS070_doStartResumeService
    {

    }
    //End Add
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS070_doRentalContractBasicAuthority_MetaData
    {
        [OperationOfficeInRole(Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0063, ControlName = "txtSpecifyContractCode")]
        public string OperationOfficeCode { get; set; }

        [ContractOfficeInRole(Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0063, ControlName = "txtSpecifyContractCode")]
        public string ContractOfficeCode { get; set; }
    }

    public class CTS070_ValidateStartResumeService_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblStartType",
                ControlName = "ddlStartType")]
        public string StartType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblStartResumeOperationDate",
                ControlName = "dpStartResumeOperationDate")]
        public Nullable<System.DateTime> StartResumeOperationDate { get; set; }
    }

    public class CTS070_ValidateStartTypeNewStart_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblStartType",
                ControlName = "ddlStartType")]
        public string StartType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblStartResumeOperationDate",
                ControlName = "dpStartResumeOperationDate")]
        public Nullable<System.DateTime> StartResumeOperationDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblUserCode",
                ControlName = "txtUserCode")]
        public string UserCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblLineType",
                ControlName = "ddlLineTypeNormal")]
        public string LineTypeNormal { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblTelephoneOwner",
                ControlName = "ddlTelephoneOwnerNormal")]
        public string TelephoneOwnerNormal { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblTelephoneNo",
                ControlName = "txtTelephoneNoNormal")]
        public string TelephoneNoNormal { get; set; }
    }

    public class CTS070_ValidateStartTypeAlterStart_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblStartType",
                ControlName = "ddlStartType")]
        public string StartType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblStartResumeOperationDate",
                ControlName = "dpStartResumeOperationDate")]
        public Nullable<System.DateTime> StartResumeOperationDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblApprove",
                ControlName = "txtApproveNo")]
        public string ApproveNo { get; set; }
    }

    //Add by Jutarat A. on 15082012
    public class CTS070_ValidateStartTypeResume_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblStartType",
                ControlName = "ddlStartType")]
        public string StartType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblStartResumeOperationDate",
                ControlName = "dpStartResumeOperationDate")]
        public Nullable<System.DateTime> StartResumeOperationDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS070",
                Parameter = "lblUserCode",
                ControlName = "txtUserCode")]
        public string UserCode { get; set; }
    }
    //End Add
}
