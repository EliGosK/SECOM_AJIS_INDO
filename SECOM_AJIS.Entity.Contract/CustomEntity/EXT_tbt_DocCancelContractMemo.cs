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
    public partial class tbt_DocCancelContractMemo
    {
        public string AutoTransferBillingAmtForShow
        {
            get
            {
                return CommonUtil.TextNumeric(this.AutoTransferBillingAmt);
            }
        }

        public string BankTransferBillingAmtForShow
        {
            get
            {
                return CommonUtil.TextNumeric(this.BankTransferBillingAmt);
            }
        }
        public string BankTransferBillingAmtUsdForShow
        {
            get
            {
                return CommonUtil.TextNumeric(this.BankTransferBillingAmtUsd);
            }
        }
    }
}

