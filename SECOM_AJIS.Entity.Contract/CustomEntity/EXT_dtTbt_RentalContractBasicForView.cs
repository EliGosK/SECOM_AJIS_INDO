using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Contract
{
    /// <summary>
    /// Do Of table rental contract basic for view
    /// Merge All File at 14032017 By Pachara S.
    /// </summary>
    [MetadataType(typeof(dtTbt_RentalContractBasicForView_MetaData))]
    public partial class dtTbt_RentalContractBasicForView
    {
        private string _ContractCodeShort;

        public string ContractCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _ContractCodeShort = value; }
        }

        private string _ContractTargetCustCodeShort;

        public string ContractTargetCustCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertCustCode(ContractTargetCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _ContractTargetCustCodeShort = value; }
        }

        private string _RealCustomerCustCodeShort;

        public string RealCustomerCustCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertCustCode(this.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }

        }

        private string _SiteCodeShort;

        public string SiteCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertSiteCode(SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _SiteCodeShort = value; }
        }

        public string NormalDepositFeeNumeric
        {
            get
            {
                return CommonUtil.TextNumeric(NormalDepositFee);

            }
        }

        public string OrderDepositFeeNumeric
        {
            get
            {
                return CommonUtil.TextNumeric(OrderDepositFee);

            }
        }

        public string ExemptedDepositFeeNumeric
        {
            get
            {
                return CommonUtil.TextNumeric(ExemptedDepositFee);

            }
        }

        public string BillingDepositFeeNumeric
        {
            get
            {
                return CommonUtil.TextNumeric(BilledDepositFee);

            }
        }

        public string SiteNameEN
        {
            get
            {
                return this.SiteNameEN_Site;
            }
        }
        public string SiteNameLC
        {
            get
            {
                return this.SiteNameLC_Site;
            }
        }
        public string SiteName { get; set; }

        public string CustNameEN
        {
            get
            {
                return this.CustNameEN_Cust;
            }
        }
        public string CustNameLC
        {
            get
            {
                return this.CustNameLC_Cust;
            }
        }
        public string CustName { get; set; }
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }

        #region
        public string TextTransferNormalDepositFee
        {
            get
            {
                // begin add by jirawat jannet on 2016-12-29
                decimal? amt = 0;
                if (this.NormalDepositFeeCurrencyType == CurrencyType.C_CURRENCY_TYPE_USD) amt = this.NormalDepositFeeUsd;
                else amt = this.NormalDepositFee;
                string txt = CommonUtil.TextNumeric(amt);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.NormalDepositFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
        public string TextTransferOrderDepositFee
        {
            get
            {
                // begin add by jirawat jannet on 2016-12-29
                decimal? amt = 0;
                if (this.OrderDepositFeeCurrencyType == CurrencyType.C_CURRENCY_TYPE_USD) amt = this.OrderDepositFeeUsd;
                else amt = this.OrderDepositFee;
                string txt = CommonUtil.TextNumeric(amt);
                // end add

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.OrderDepositFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferExemptedDepositFee
        {
            get
            {
                // begin add by jirawat jannet on 2016-12-29
                decimal? amt = 0;
                if (this.ExemptedDepositFeeCurrencyType == CurrencyType.C_CURRENCY_TYPE_USD) amt = this.ExemptedDepositFeeUsd;
                else amt = this.ExemptedDepositFee;
                string txt = CommonUtil.TextNumeric(amt);
                // end add

                //string txt = CommonUtil.TextNumeric(this.ExemptedDepositFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.ExemptedDepositFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
        #endregion

        public string TextTransferLastOrderContractFee
        {
            get
            {
                // begin add by jirawat jannet on 2016-12-29
                decimal? amt = 0;
                if (LastOrderContractFeeCurrencyType == CurrencyType.C_CURRENCY_TYPE_USD) amt = this.LastOrderContractFeeUsd;
                else amt = LastOrderContractFee;

                string txt = CommonUtil.TextNumeric(amt);
                // end add

                //string txt = CommonUtil.TextNumeric(this.LastOrderContractFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.LastOrderContractFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferNormalDepositFee_CMS430
        {
            get
            {
                // begin add by jirawat jannet on 2016-12-29
                string txt = CommonUtil.TextNumeric((this.NormalDepositFee ?? 0));

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
        public string TextTransferOrderDepositFee_CMS430
        {
            get
            {
                // begin add by jirawat jannet on 2016-12-29
                string txt = CommonUtil.TextNumeric((this.OrderDepositFee ?? 0));
                // end add

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferExemptedDepositFee_CMS430
        {
            get
            {
                // begin add by jirawat jannet on 2016-12-29
                string txt = CommonUtil.TextNumeric((this.ExemptedDepositFee ?? 0));
                // end add

                //string txt = CommonUtil.TextNumeric(this.ExemptedDepositFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferNormalDepositFeeUsd_CMS430
        {
            get
            {
                // begin add by jirawat jannet on 2016-12-29
                string txt = CommonUtil.TextNumeric((this.NormalDepositFeeUsd ?? 0));

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
        public string TextTransferOrderDepositFeeUsd_CMS430
        {
            get
            {
                // begin add by jirawat jannet on 2016-12-29
                string txt = CommonUtil.TextNumeric((this.OrderDepositFeeUsd ?? 0));
                // end add

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferExemptedDepositFeeUsd_CMS430
        {
            get
            {
                // begin add by jirawat jannet on 2016-12-29
                string txt = CommonUtil.TextNumeric((this.ExemptedDepositFeeUsd ?? 0));
                // end add

                //string txt = CommonUtil.TextNumeric(this.ExemptedDepositFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    /// <summary>
    /// Do Of table rental contract basic for view meta data
    /// </summary>
    public class dtTbt_RentalContractBasicForView_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [LanguageMapping]
        public string Op_OfficeName { get; set; }

        [LanguageMapping]
        public string SiteName { get; set; }

        [LanguageMapping]
        public string CustName { get; set; }
    }
}
