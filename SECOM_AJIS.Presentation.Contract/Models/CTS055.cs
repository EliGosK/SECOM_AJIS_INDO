using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of CTS055 screen
    /// </summary>
    public class CTS055_Parameter
    {
        public string ContractCode { get; set; }
    }

    /// <summary>
    /// DO for validtae business
    /// </summary>
    [MetadataType(typeof(CTS055_DORegisterData_MetaData))]
    public class CTS055_DOValidateBusiness
    {
        public DateTime? ExpectedOperationDate { get; set; }
        public String ContractCode { get; set; }
        public String OCC { get; set; }
        public String InstallationStatusCode { get; set; }
    }

    /// <summary>
    /// Parameter of CTS055 screen
    /// </summary>
    public class CTS055_ScreenParameter : ScreenSearchParameter
    {
        public dsRentalContractData DSRentalContract { get; set; }

        [KeepSession]
        public CTS055_Parameter ScreenParameter { get; set; }

        public CTS055_DOValidateBusiness DOValidateBusiness { get; set; }

        [KeepSession]
        public string ContractCode { get; set; }
    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS055_DORegisterData_MetaData : CTS055_DOValidateBusiness
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty()]
        public string OCC { get; set; }

        [NotNullOrEmpty(ControlName = "dpExpectOperationDate", Screen = "_ContractCancel", Controller = MessageUtil.MODULE_COMMON, Parameter = "lblExpectOperationDate")]
        public string ExpectedOperationDate { get; set; }
    }
}