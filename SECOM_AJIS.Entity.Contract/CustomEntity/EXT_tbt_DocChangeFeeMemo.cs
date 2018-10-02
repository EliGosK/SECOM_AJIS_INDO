using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class tbt_DocChangeFeeMemo
    {
        public string OldContractFeeForShow
        {
            get
            {
                if (this.OldContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    return CommonUtil.TextNumeric(this.OldContractFeeUsd);
                return CommonUtil.TextNumeric(this.OldContractFee);
            }
        }

        public string NewContractFeeForShow
        {
            get
            {
                if (this.NewContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                    return CommonUtil.TextNumeric(this.NewContractFeeUsd);
                return CommonUtil.TextNumeric(this.NewContractFee);
            }
        }

    }
}
