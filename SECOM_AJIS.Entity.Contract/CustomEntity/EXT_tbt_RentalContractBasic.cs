using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(tbt_RentalContractBasic_MetaData))]
    public partial class tbt_RentalContractBasic
    {
        CommonUtil comUtil = new CommonUtil();

        public string PODocAuditResultName { get; set; }
        public string PODocAuditResultCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.PODocAuditResult, this.PODocAuditResultName);
            }
        }

        public string ContractDocAuditResultName { get; set; }
        public string ContractDocAuditResultCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.ContractDocAuditResult, this.ContractDocAuditResultName);
            }
        }

        public string StartMemoAuditResultName { get; set; }
        public string StartMemoAuditResultCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.StartMemoAuditResult, this.StartMemoAuditResultName);
            }
        }

        public string ContractCodeShort
        {
            get
            {
                return comUtil.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string OldContractCodeShort
        {
            get
            {
                return comUtil.ConvertContractCode(OldContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string CounterBalanceOriginContractCodeShort
        {
            get
            {
                return comUtil.ConvertContractCode(CounterBalanceOriginContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string LastChangeTypeName { get; set; }
        public string LastChangeTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.LastChangeType, this.LastChangeTypeName);
            }
        }
    }

}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class tbt_RentalContractBasic_MetaData
    {
        [DocAuditResultMapping("PODocAuditResultName")]
        public string PODocAuditResult { get; set; }

        [DocAuditResultMapping("ContractDocAuditResultName")]
        public string ContractDocAuditResult { get; set; }

        [DocAuditResultMapping("StartMemoAuditResultName")]
        public string StartMemoAuditResult { get; set; }

        [RentalChangeTypeMapping("LastChangeTypeName")]
        public string LastChangeType {get;set;}
    }
}

