using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(dtTbt_SaleBasicForView_MetaData))]
    public partial class dtTbt_SaleBasicForView
    {
        public string SiteCode_Short { get { return new CommonUtil().ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT); } }
        public string PurchaserCustCode_Short { get { return new CommonUtil().ConvertCustCode(this.PurchaserCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT); } }
        public string RealCustomerCustCode_Short { get { return new CommonUtil().ConvertCustCode(this.RealCustomerCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT); } }
        public string ContractCode_Short { get { return new CommonUtil().ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT); } }

        public string ChangeTypeName { get; set; }
        public string ChangeTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.ChangeType, this.ChangeTypeName);
            }
        }

        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string TextTransferNormalInstallFee
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.NormalInstallFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.NormalInstallFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferOrderInstallFee
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.OrderInstallFee);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.OrderInstallFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferInstallFeePaidBySECOM
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.InstallFeePaidBySECOM);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.InstallFeePaidBySECOMCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferInstallFeeRevenueBySECOM
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.InstallFeeRevenueBySECOM);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.InstallFeeRevenueBySECOMCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferOrderProductPrice
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.OrderProductPrice);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.OrderProductPriceCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferNormalProductPrice
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.NormalProductPrice);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.NormalProductPriceCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferBidGuaranteeAmount1
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.BidGuaranteeAmount1);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.BidGuaranteeAmount1CurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferBidGuaranteeAmount2
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.BidGuaranteeAmount2);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.BidGuaranteeAmount2CurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferNewBldMgmtCost
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.NewBldMgmtCost);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.NewBldMgmtCostCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferOrderSalePrice
        {
            get
            {
                decimal amt = 0;
                if (OrderSalePriceCurrencyType == CurrencyUtil.C_CURRENCY_US) amt = OrderSalePriceUsd ?? 0;
                else amt = OrderSalePrice ?? 0;

                string txt = CommonUtil.TextNumeric(amt);
                //string txt = CommonUtil.TextNumeric(this.OrderSalePrice);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.OrderSalePriceCurrencyType);
                        if(curr == null) curr = this.Currencies.Find(x => x.ValueCode == CurrencyUtil.C_CURRENCY_LOCAL);
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
    public class dtTbt_SaleBasicForView_MetaData
    {
        [SaleChangeTypeMapping("ChangeTypeName")]
        public string ChangeType { get; set; }
    }
}
