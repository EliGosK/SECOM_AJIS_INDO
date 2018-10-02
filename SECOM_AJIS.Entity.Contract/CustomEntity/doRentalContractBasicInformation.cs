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
    [MetadataType(typeof(doRentalContractBasicInformation_C_MetaData))]
    public partial class doRentalContractBasicInformation
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

        private string _ContractTargetCustCodeShort;
        public string ContractTargetCustCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertCustCode(ContractTargetCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _ContractTargetCustCodeShort = value; }
        }

        private string _RealCustomerCustCodeShort;
        public string RealCustomerCustCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertCustCode(RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _RealCustomerCustCodeShort = value; }
        }

        private string _SiteCodeShort;
        public string SiteCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertSiteCode(SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _SiteCodeShort = value; }
        }

        //public string OfficeName { get; set; }
        public string OperationOfficeName{ get; set; }
        public string OperationOfficeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.OperationOfficeCode, this.OperationOfficeName);
            }
        }

        public string ContractOfficeName { get; set; }
        public string ContractOfficeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.ContractOfficeCode, this.ContractOfficeName);
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
    public class doRentalContractBasicInformation_C_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        //[LanguageMapping]
        //public string OfficeName { get; set; }
        [LanguageMapping]
        public string OperationOfficeName { get; set; }

        [LanguageMapping]
        public string ContractOfficeName { get; set; }

        [RentalChangeTypeMapping("LastChangeTypeName")]
        public string LastChangeType { get; set; }
    }
}