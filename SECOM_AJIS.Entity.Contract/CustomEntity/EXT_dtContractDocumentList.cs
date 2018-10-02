using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtContractDocumentList
    {
        public string ContractCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string QuotationTargetCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertQuotationTargetCode(QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string NegotiationStaffEmpNameJP
        {
            get
            {
                return NegotiationStaffEmpNameEN;
            }
        }

        [LanguageMapping]
        public string DocumentName { get; set; }

        [LanguageMapping]
        public string DocStatusName { get; set; }

        [LanguageMapping]
        public string DocAuditResultName { get; set; }

        [LanguageMapping]
        public string ContractOfficeName { get; set; }

        [LanguageMapping]
        public string OperationOfficeName { get; set; }

        [LanguageMapping]
        public string NegotiationStaffEmpName { get; set; }
    }
}
