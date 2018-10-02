﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtTbt_RentalSentryGuardForView
    {
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string TextTransferSecurityItemFee
        {
            get
            {
                // add by jirawat jannet on 2016-12-28
                decimal amt = SecurityItemFeeCurrencyType == CurrencyUtil.C_CURRENCY_US ? this.SecurityItemFeeUsd ?? 0 : SecurityItemFee ?? 0;
                //string txt = CommonUtil.TextNumeric(this.SecurityItemFee);  // comment by jirawat jannet on 2016-12-28
                string txt = CommonUtil.TextNumeric(amt);  // add by jirawat jannet on 2016-12-28

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.SecurityItemFeeCurrencyType);
                        // add by jirawat jannet on 2016-12-28
                        if (curr == null) curr = this.Currencies.Find(x => x.ValueCode == CurrencyUtil.C_CURRENCY_LOCAL);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferOtherItemFee
        {
            get
            {
                // add by jirawat jannet on 2016-12-28
                decimal amt = OtherItemFeeCurrencyType == CurrencyUtil.C_CURRENCY_US ? this.OtherItemFeeUsd ?? 0 : OtherItemFee ?? 0;
                //string txt = CommonUtil.TextNumeric(this.OtherItemFee);  // comment by jirawat jannet on 2016-12-28
                string txt = CommonUtil.TextNumeric(amt);  // add by jirawat jannet on 2016-12-28


                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.OtherItemFeeCurrencyType);
                        // add by jirawat jannet on 2016-12-28
                        if (curr == null) curr = this.Currencies.Find(x => x.ValueCode == CurrencyUtil.C_CURRENCY_LOCAL);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
    }
}
