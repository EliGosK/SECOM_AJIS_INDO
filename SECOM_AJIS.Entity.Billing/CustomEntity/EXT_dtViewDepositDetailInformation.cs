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
    /// Do Of view deposit detail information
    /// </summary>
    [MetadataType(typeof(dtViewDepositDetailInformation_MetaData))]
    public partial class dtViewDepositDetailInformation
    {

        [LanguageMapping]
        public string OfficeName { get; set; }

        public Nullable<decimal> BalanceOfDeposit { get; set; }

        CommonUtil cm = new CommonUtil();
        public string BillingTargetCode_short
        {
            get
            {
               
                return cm.ConvertBillingTargetCode(BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string BillingCode
        {
            get
            {
                return string.Format("{0}-{1}", ContractCode, BillingOCC);
                //return cm.ConvertBillingTargetCode(BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string BillingCode_Short
        {
            get
            {
                return cm.ConvertBillingCode(BillingCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string BillingTargetCodeForSlide
        {
            get
            {
                if (this.DepositStatus == SECOM_AJIS.Common.Util.ConstantValue.DepositStatus.C_DEPOSIT_STATUS_SLIDE)
                {
                    return BillingTargetCode_short;
                }
                else
                    return null;
            }
        }

        public string DepositStatusName { set; get; }

        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }

        public string TextTransferProcessAmount
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.ProcessAmount);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == (this.ProcessAmountCurrencyType ?? CurrencyUtil.C_CURRENCY_LOCAL));
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferReceivedFee
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.ReceivedFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == (this.ReceivedFeeCurrencyType ?? CurrencyUtil.C_CURRENCY_LOCAL));
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
    /// Do Of view deposit detail information meta data
    /// </summary>
    public class dtViewDepositDetailInformation_MetaData
    {
        [DepositStatusMappingAttribute("DepositStatusName")]
        public string DepositStatus { get ; set; }

     
    }
}