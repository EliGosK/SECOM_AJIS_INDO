using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_DocCancelContractMemoDetail_MetaData))]
    public partial class tbt_DocCancelContractMemoDetail
    {
        public string BillingTypeNameForShow { get; set; }
        public string BillingTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.BillingType, this.BillingTypeNameForShow);
            }
        }

        public string HandlingTypeNameForShow { get; set; }
        public string HandlingTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.HandlingType, this.HandlingTypeNameForShow);
            }
        }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_DocCancelContractMemoDetail_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [ContractBillingTypeMappingAttribute("BillingTypeNameForShow")]
        public string BillingType { get; set; }

        [HandlingTypeMappingAttribute("HandlingTypeNameForShow")]
        public string HandlingType { get; set; }
    }
}
