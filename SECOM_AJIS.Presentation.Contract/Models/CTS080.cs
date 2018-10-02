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
    public class CTS080_RegisterQuotationTargetData
    {
        public CTS080_InitialRegisterQuotationTargetData InitialData { get; set; }
        public dsRentalContractData RegisterRentalContractData { get; set; }
    }

    /// <summary>
    /// Parameter for Initial data
    /// </summary>
    public class CTS080_InitialRegisterQuotationTargetData
    {
        public string ContractCode { get; set; }
        public string OCCCode { get; set; }
        public doRentalContractBasicInformation doRentalContractBasicData { get; set; }
        public bool HasStopFee { get; set; }
        public tbt_RentalContractBasic RentalContractBasicData { get; set; }
        public dsCancelContractQuotation CancelContractQuotationData { get; set; }
        public List<CTS110_CancelContractMemoDetailTemp> CancelContractMemoDetailTempData { get; set; }

        public decimal? TotalSlideAmount { get; set; }
        public decimal? TotalSlideAmountUsd { get; set; }
        public decimal? TotalRefundAmount { get; set; }
        public decimal? TotalRefundAmountUsd { get; set; }
        public decimal? TotalBillingAmount { get; set; }
        public decimal? TotalBillingAmountUsd { get; set; }
        public decimal? TotalCounterBalAmount { get; set; }
        public decimal? TotalCounterBalAmountUsd { get; set; }
    }

    /// <summary>
    /// Parameter of CTS080 screen
    /// </summary>
    public class CTS080_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public CTS080_RegisterQuotationTargetData CTS080_Session { get; set; }

        public string ContractCode { get; set; }
    }

    /// <summary>
    /// DO for check authority of RentalContractBasic 
    /// </summary>
    [MetadataType(typeof(CTS080_doRentalContractBasicAuthority_MetaData))]
    public class CTS080_doRentalContractBasicAuthority : tbt_RentalContractBasic
    {

    }

    /// <summary>
    /// Parameter for Register Quotation data
    /// </summary>
    public class CTS080_RegisterQuotationData
    {
        public DateTime? StartServiceDate { get; set; }
        public DateTime? CancelContractDate { get; set; }
        public string OtherRemarks { get; set; }
        public bool SECOMSignatureFlag { get; set; }
        public string EmpName { get; set; }
        public string EmpPosition { get; set; }
    }

}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS080_doRentalContractBasicAuthority_MetaData
    {
        [OperationOfficeInRole(Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0063)]
        public string OperationOfficeCode { get; set; }
    }
}
