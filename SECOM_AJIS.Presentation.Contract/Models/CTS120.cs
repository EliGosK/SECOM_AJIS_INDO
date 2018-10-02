using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of screen
    /// </summary>
    public class CTS120_ScreenParameter : ScreenSearchParameter
    {
        [NotNullOrEmpty(ControlName = "EntryContractCode")]
        public String strContractCode { get; set; }
    }

    /// <summary>
    /// Object for display data to screen
    /// </summary>
    [Serializable]
    public class CTS120_View
    {
        public bool CanContinue { get; set; }
        public string ContractCode { get; set; }
        public string UserCode { get; set; }
        public string ContractTargetCustCodeShort { get; set; }
        public string RealCustomerCustCodeShort { get; set; }
        public string SiteCodeShort { get; set; }
        public bool chkContractTargetFlag { get; set; }
        public string ContractTargetNameEN { get; set; }
        public string ContractTargetAddressEN { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteAddressEN { get; set; }
        public string ContractTargetNameLC { get; set; }
        public string ContractTargetAddressLC { get; set; }
        public string SiteNameLC { get; set; }
        public string SiteAddressLC { get; set; }
        public string OperationOffice { get; set; }
        public string CancelDate { get; set; }
        public string CancelReason { get; set; }
        public string MonthlyContractFeeCurrencyType { get; set; }
        public string MonthlyContractFee { get; set; }
        public string FirstOperationDate { get; set; }
    }
}
