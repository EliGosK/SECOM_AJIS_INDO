using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtGroupSummary
    {
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string TextTransferSumContractAmount_CT_LOCAL
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.SumContractAmount_CT1);

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

        public string TextTransferSumContractAmount_CT_US
        {
            get
            {
                string txt = CommonUtil.TextNumeric(SumContractAmount_CT2);

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

        public string TextTransferSumContractAmount_R_LOCAL
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.SumContractAmount_R1);

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

        public string TextTransferSumContractAmount_R_US
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.SumContractAmount_R2);

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

        public string TextSumContractAmount_CT_LOCAL
        {
            get
            {
                ICommonHandler commonHandler = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                decimal amt = SumContractAmount_CT1 ?? 0;
                decimal amt2 = SumContractAmount_CT2 ?? 0;
                double errorCode = 0;

                decimal localAmt = commonHandler.ConvertCurrencyPrice(amt, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_LOCAL, DateTime.Now, ref errorCode);
                decimal usdAmt = commonHandler.ConvertCurrencyPrice(amt2, CurrencyUtil.C_CURRENCY_US, CurrencyUtil.C_CURRENCY_LOCAL, DateTime.Now, ref errorCode);
                amt = localAmt + usdAmt;

                string txt = CommonUtil.TextNumeric(amt);

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

        public string TextSumContractAmount_CT_US
        {
            get
            {
                ICommonHandler commonHandler = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                decimal amt = SumContractAmount_CT1 ?? 0;
                decimal amt2 = SumContractAmount_CT2 ?? 0;
                double errorCode = 0;

                decimal localAmt = commonHandler.ConvertCurrencyPrice(amt, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US, DateTime.Now, ref errorCode);
                decimal usdAmt = commonHandler.ConvertCurrencyPrice(amt2, CurrencyUtil.C_CURRENCY_US, CurrencyUtil.C_CURRENCY_US, DateTime.Now, ref errorCode);
                amt = localAmt + usdAmt;

                string txt = CommonUtil.TextNumeric(amt);

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

        public string TextSumContractAmount_R_LOCAL
        {
            get
            {
                ICommonHandler commonHandler = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                decimal amt = SumContractAmount_R1 ?? 0;
                decimal amt2 = SumContractAmount_R2 ?? 0;
                double errorCode = 0;

                decimal localAmt = commonHandler.ConvertCurrencyPrice(amt, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_LOCAL, DateTime.Now, ref errorCode);
                decimal usdAmt = commonHandler.ConvertCurrencyPrice(amt2, CurrencyUtil.C_CURRENCY_US, CurrencyUtil.C_CURRENCY_LOCAL, DateTime.Now, ref errorCode);
                amt = localAmt + usdAmt;

                string txt = CommonUtil.TextNumeric(amt);

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

        public string TextSumContractAmount_R_US
        {
            get
            {
                ICommonHandler commonHandler = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                decimal amt = SumContractAmount_R1 ?? 0;
                decimal amt2 = SumContractAmount_R2 ?? 0;
                double errorCode = 0;

                decimal localAmt = commonHandler.ConvertCurrencyPrice(amt, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US, DateTime.Now, ref errorCode);
                decimal usdAmt = commonHandler.ConvertCurrencyPrice(amt2, CurrencyUtil.C_CURRENCY_US, CurrencyUtil.C_CURRENCY_US, DateTime.Now, ref errorCode);
                amt = localAmt + usdAmt;

                string txt = CommonUtil.TextNumeric(amt);

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

        public string FinalTextContractAmount_CT
        {
            get
            {
                return CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "colAmountOfContract")
                    + "<br>" + TextSumContractAmount_CT_LOCAL + "<br>" + TextSumContractAmount_CT_US;
            }
        }

        public string FinalTextContractAmount_R
        {
            get
            {
                return CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, ScreenID.C_SCREEN_ID_VIEW_CUSTOMER_GROUP, "colAmountOfContract")
                    + "<br>" + TextSumContractAmount_R_LOCAL + "<br>" + TextSumContractAmount_R_US;
            }
        }
    }
}
