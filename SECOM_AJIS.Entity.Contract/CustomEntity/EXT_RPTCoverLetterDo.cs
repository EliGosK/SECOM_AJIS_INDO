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
    public partial class RPTCoverLetterDo
    {
        public string ContractCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string DocNoShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(DocNo, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string OldContractCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(OldContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string CurrencyOfNomalFee
        {
            get
            {
                return CommonUtil.TargetCurrency(QuotationFeeCurrencyType);
            }
        }

        public decimal ValueOfNomalFee
        {
            get
            {
                return CommonUtil.TargetFee(QuotationFeeCurrencyType, QuotationFee, QuotationFeeUsD);
            }
        }
        public string CurrencyOfServiceFee
        {
            get
            {
                return CommonUtil.TargetCurrency(ContractFeeCurrencyType);
            }
        }

        public decimal ValueOFServiceFee
        {
            get
            {
                return CommonUtil.TargetFee(ContractFeeCurrencyType, ContractFee, ContractFeeUsD);
            }
        }
        
        public string CurrencyOfDepositFee
        {
            get
            {
                return CommonUtil.TargetCurrency(DepositFeeCurrencyType);
            }
        }
        
        public decimal ValueOfDepositFee
        {
            get
            {
                return CommonUtil.TargetFee(DepositFeeCurrencyType, DepositFee, DepositFeeUsD);
            }
        }
    }
}
