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
    [MetadataType(typeof(tbt_CancelContractMemo_MetaData))]
    public partial class tbt_CancelContractMemo
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

        public string ProcessAfterCounterBalanceTypeName { get; set; }
        public string ProcessAfterCounterBalanceTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.ProcessAfterCounterBalanceType, this.ProcessAfterCounterBalanceTypeName);
            }
        }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_CancelContractMemo_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [ProcessAfterCounterBalanceTypeMappingAttribute("ProcessAfterCounterBalanceTypeName")]
        public string ProcessAfterCounterBalanceType { get; set; }
    }
}
