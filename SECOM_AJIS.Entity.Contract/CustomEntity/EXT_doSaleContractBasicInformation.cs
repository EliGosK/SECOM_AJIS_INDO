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
    [MetadataType(typeof(doSaleContractBasicInformation_MetaData))]
    public partial class doSaleContractBasicInformation
    {
        CommonUtil comUtil = new CommonUtil();

        public string ContractCodeShort
        {
            get
            {
                return comUtil.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string PurchaserCustCodeShort
        {
            get
            {
                return comUtil.ConvertCustCode(PurchaserCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string RealCustomerCustCodeShort
        {
            get
            {
                return comUtil.ConvertCustCode(RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string SiteCodeShort
        {
            get
            {
                return comUtil.ConvertSiteCode(SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string InstallationStatusCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.InstallationStatusCode, this.InstallationStatusName);
            }
        }

        [LanguageMapping]
        public string ContractOfficeName { get; set; }
        public string ContractOfficeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.ContractOfficeCode, this.ContractOfficeName);
            }
        }

        [LanguageMapping]
        public string OperationOfficeName { get; set; }
        public string OperationOfficeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.OperationOfficeCode, this.OperationOfficeName);
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
    public class doSaleContractBasicInformation_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [SaleChangeTypeMapping("LastChangeTypeName")]
        public string LastChangeType { get; set; }

        [InstallationStatusMapping("InstallationStatusName")]
        public string InstallationStatusCode { get; set; }
    }
}
