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
    public partial class RPTCancelContractMemoDetailDo
    {
        public string ContractCode_CounterBalanceShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(this.ContractCode_CounterBalance, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string CurrencyOfFeeAmount
        {
            get
            {
                return CommonUtil.TargetCurrency(FeeAmountCurrencyType);
            }
        }

        public decimal ValueOfFeeAmount
        {
            get
            {
                return CommonUtil.TargetFee(FeeAmountCurrencyType, FeeAmount, FeeAmountUsD);
            }
        }

        public string CurrencyOfTaxAmount
        {
            get
            {
                return CommonUtil.TargetCurrency(TaxAmountCurrencyType);
            }
        }

        public decimal ValueOfTaxAmount
        {
            get
            {
                return CommonUtil.TargetFee(TaxAmountCurrencyType, TaxAmount, TaxAmountUsD);
            }
        }
    }
}
