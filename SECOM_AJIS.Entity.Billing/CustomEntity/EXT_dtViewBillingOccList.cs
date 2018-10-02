using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Billing.MetaData;

namespace SECOM_AJIS.DataEntity.Billing
{
    /// <summary>
    /// Do Of view billing occ list
    /// </summary>
    [MetadataType(typeof(dtViewBillingOccList_MetaData))]
    public partial class dtViewBillingOccList
    {
        [LanguageMapping]
        public string OfficeName { get; set; }

        public string BillingTargetCode_short
        {
            get
            {
                CommonUtil cm = new CommonUtil();
                return cm.ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string Name
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}",
                String.IsNullOrEmpty(this.NameEN) ? "-" : this.NameEN,
                String.IsNullOrEmpty(this.NameLC) ? "-" : this.NameLC);
            }
        }

        public string IssueInvoice_WithCheckConts
        {
            get
            {
                if (Issue_Invoice == Convert.ToInt32(Issued.C_ISSUED))
                {
                    return "Issued";
                }
                else return "Not Issue";                               
            }
        }

        public string Payment_WithCheckConts
        {
            get
            {
                if (Payment == Convert.ToInt32(Paid.C_PAID))
                {
                    return "Paid";
                }
                else return "Not Paid";
            }
        }

        public string ContractCode_short
        {
            get
            {
                CommonUtil cm = new CommonUtil();
                return cm.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string IssueInvoiceName { get; set; }

        public string PaymentName { get; set; }

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
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == (this.MonthlyBillingAmountCurrencyType ?? CurrencyUtil.C_CURRENCY_LOCAL));
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferBalanceDeposit
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.BalanceDeposit);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == (this.BalanceDepositCurrencyType ?? CurrencyUtil.C_CURRENCY_LOCAL));
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
    }
}

namespace SECOM_AJIS.DataEntity.Billing.MetaData
{
    /// <summary>
    /// Do Of view billing occ list meta data
    /// </summary>
    public class dtViewBillingOccList_MetaData
    {
        [IssueInvoiceMappingAttribute("IssueInvoiceName")]
        public string Issue_Invoice { get; set; }

        [PaymentMappingAttribute("PaymentName")]
        public string Payment { get; set; }
    }
}