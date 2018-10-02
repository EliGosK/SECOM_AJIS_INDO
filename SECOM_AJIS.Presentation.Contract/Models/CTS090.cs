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
    public class CTS090_RegisterCancelTargetData
    {
        public CTS090_InitialRegisterCancelTargetData InitialData { get; set; }
        public tbt_SaleBasic RegisterCancelData { get; set; }
    }

    /// <summary>
    /// Parameter for Initial data
    /// </summary>
    public class CTS090_InitialRegisterCancelTargetData
    {
        public string ContractCode { get; set; }
        public string OCCCode { get; set; }
        public doSaleContractBasicInformation doSaleContractBasicData { get; set; }  
    }

    /// <summary>
    /// Parameter of CTS090 screen
    /// </summary>
    public class CTS090_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public CTS090_RegisterCancelTargetData CTS090_Session { get; set; }

        public string ContractCode { get; set; }
    }

    /// <summary>
    /// DO for check authority of SaleBasic
    /// </summary>
    [MetadataType(typeof(CTS090_doSaleBasicDataAuthority_MetaData))]
    public class CTS090_doSaleBasicDataAuthority : tbt_SaleBasic
    {

    }

    /// <summary>
    /// DO of CancelReason
    /// </summary>
    [MetadataType(typeof(CTS090_doCancelReason_MetaData))]
    public class CTS090_doCancelReason : tbt_SaleBasic
    {

    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS090_doSaleBasicDataAuthority_MetaData
    {
        [OperationOfficeInRole(Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0063)]
        public string OperationOfficeCode { get; set; }
    }

    public class CTS090_doCancelReason_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS090",
                Parameter = "lblCancelDate",
                ControlName = "dpCancelDate")]
        public Nullable<System.DateTime> CancelDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS090",
                Parameter = "lblCancelReason",
                ControlName = "ddlCancelReason")]
        public string CancelReasonType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS090",
                Parameter = "lblApproveNo",
                ControlName = "txtApproveNo")]
        public string ApproveNo1 { get; set; }
    }
}
