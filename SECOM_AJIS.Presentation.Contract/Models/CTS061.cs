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
    /// Parameter of CTS061 screen
    /// </summary>
    public class CTS061_Parameter
    {
        public string contractCode { get; set; }
    }

    /// <summary>
    /// DO of ScreenOutput
    /// </summary>
    public class CTS061_ScreenOutputObject
    {
        public string LastOCC { get; set; }
        public bool CanOperate { get; set; }
        public string ExpectedInstallCompleteDate { get; set; }
        public string InstallationStatusCode { get; set; }
        public string ContractCode { get; set; }
        public string ContractCodeShort { get; set; }
        public string PurchaserCustCode { get; set; }
        public string RealCustomerCustCode { get; set; }
        public string SiteCode { get; set; }
        public bool ImportantFlag { get; set; }
        public string PurchaserNameEN { get; set; }
        public string PurchaserAddressEN { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteAddressEN { get; set; }
        public string PurchaserNameLC { get; set; }
        public string PurchaserAddressLC { get; set; }
        public string SiteNameLC { get; set; }
        public string SiteAddressLC { get; set; }
        public string InstallationStatusCodeName { get; set; }
        public string OperationOfficeName { get; set; }
    }

    /// <summary>
    /// DO for validtae business
    /// </summary>
    [MetadataType(typeof(CTS061_DOValidateBusiness_MetaData))]
    public class CTS061_DOValidateBusiness
    {
        public DateTime? ExpectedInstallCompleteDate { get; set; }
    }

    /// <summary>
    /// Parameter of CTS061 screen
    /// </summary>
    public class CTS061_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public CTS061_Parameter ScreenParameter { get; set; }

        public dsSaleContractData DSSaleContract { get; set; }
        public CTS061_DOValidateBusiness DOValidateBusiness { get; set; }

        [KeepSession]
        public string ContractCode { get; set; }
    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS061_DOValidateBusiness_MetaData
    {
        [NotNullOrEmpty(ControlName = "ExpectedInstallCompleteDate", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblExpectedInstallationCompleteDate", Screen = "CTS061")]
        public DateTime? ExpectedInstallCompleteDate { get; set; }
    }
}