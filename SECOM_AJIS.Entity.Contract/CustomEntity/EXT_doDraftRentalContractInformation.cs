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
    public partial class doDraftRentalContractInformation
    {
        CommonUtil comUtil = new CommonUtil();

        public string ContractCodeShort 
        {
            get
            {
                return comUtil.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string QuotationTargetCodeShort 
        {
            get
            {
                return comUtil.ConvertQuotationTargetCode(this.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string ContractTargetCustCodeShort
        {
            get
            {
                return comUtil.ConvertCustCode(this.ContractTargetCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string RealCustomerCustCodeShort
        {
            get
            {
                return comUtil.ConvertCustCode(this.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string SiteCodeShort
        {
            get
            {
                return comUtil.ConvertSiteCode(SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        [LanguageMapping]
        public string OperationOfficeName { get; set; }

        [LanguageMapping]
        public string ContractOfficeName { get; set; }
    }
}

