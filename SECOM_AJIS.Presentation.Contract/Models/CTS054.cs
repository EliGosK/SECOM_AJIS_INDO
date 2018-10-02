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
    /// Parameter of CTS054 screen
    /// </summary>
    public class CTS054_Parameter 
    {
        public string ContractCode { get; set; }        
    }

    /// <summary>
    /// DO for validate business
    /// </summary>
    [MetadataType(typeof(CTS054_DOValidateBusiness_MetaData))]
    public class CTS054_DOValidateBusiness
    {
        public DateTime? ExpectedOperationDate { get; set; }
        public String ContractCode { get; set; }
        public String OCC { get; set; }
        public String InstallationStatusCode { get; set; }
    }

    /// <summary>
    /// DO of ScreenOutput
    /// </summary>
    public class CTS054_ScreenOutputObject
    {
        public string ContractCodeShort { get; set; }
        public string ContractCode { get; set; }
        public string OCC { get; set; }
        public string ContractStatus { get; set; }
        public string Alphabet { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string DisplayAll { get; set; }
        public string BillingClientCode { get; set; }
        public string BillingOffice { get; set; }
        public string PaymentMethod { get; set; }
        public string Sequence { get; set; }
        public string ServiceTypeCode { get; set; }
        public string TargetCodeType { get; set; }
        public string EndContractDate { get; set; }
        public string InstallationStatusCode { get; set; }
        public string UserCode { get; set; }
        public string CustomerCode { get; set; }
        public string RealCustomerCode { get; set; }
        public string SiteCode { get; set; }
        public string CustFullNameEN { get; set; }
        public string AddressFullEN { get; set; }
        public string SiteName { get; set; }
        public string SiteAddress { get; set; }
        public string CustFullNameLC { get; set; }
        public string AddressFullLC { get; set; }
        public string SiteNameLC { get; set; }
        public string SiteAddressLC { get; set; }
        public string InstallationStatus { get; set; }
        public string OfficeName { get; set; }
        public string QuotationTargetCode { get; set; }
        public string ExpectOperationDate { get; set; }
        public string NegotiationStaffEmpNo1 { get; set; }
        public bool ImportantFlag { get; set; }

    }

    /// <summary>
    /// Parameter of CTS054 screen
    /// </summary>
    public class CTS054_ScreenParameter : ScreenSearchParameter
    {
        public dsRentalContractData DSRentalContract { get; set; }

        [KeepSession]
        public CTS054_Parameter ScreenParameter { get; set; }

        public CTS054_DOValidateBusiness DORegisterData { get; set; }
        public CTS054_DOValidateBusiness DOValidateBusiness { get; set; }

        [KeepSession]
        public string ContractCode { get; set; }
    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS054_DOValidateBusiness_MetaData : CTS054_DOValidateBusiness
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty()]
        public string OCC { get; set; }

        //[NotNullOrEmpty(ControlName = "dpExpectOperationDate", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Expect Operation Date")]
        public string ExpectedOperationDate { get; set; }
    }
}