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
    [MetadataType(typeof(tbt_CancelContractMemoDetail_MetaData))]
    public partial class tbt_CancelContractMemoDetail
    {
        private string _ContractCodeShort;
        public string ContractCodeShort 
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _ContractCodeShort = value; }
        }

        public string BillingTypeName { get; set; }
        public string BillingTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.BillingType, this.BillingTypeName);
            }
        }

        public string HandlingTypeName { get; set; }
        public string HandlingTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.HandlingType, this.HandlingTypeName);
            }
        }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_CancelContractMemoDetail_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        //[BillingTypeMappingAttribute("BillingTypeName")]
        [ContractBillingTypeMappingAttribute("BillingTypeName")]
        public string BillingType { get; set; }

        [HandlingTypeMappingAttribute("HandlingTypeName")]
        public string HandlingType { get; set; }
    }
}
