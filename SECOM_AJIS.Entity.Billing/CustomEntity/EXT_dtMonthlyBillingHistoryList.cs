﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Billing
{
    /// <summary>
    /// do MonthlyBillingHistoryList
    /// </summary>
    public partial class doTbt_MonthlyBillingHistoryList
    {
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string TextTransferMonthlyBillingAmount
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.MonthlyBillingAmount);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.MonthlyBillingAmountCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
    }
}