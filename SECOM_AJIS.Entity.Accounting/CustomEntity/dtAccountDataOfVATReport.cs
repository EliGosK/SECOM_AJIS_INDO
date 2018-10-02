using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Accounting
{
    public partial class dtAccountDataOfVATReport
    {
        CommonUtil comUtil = new CommonUtil();
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }

        public List<dtAccountingConfig> FlagDisplay { get; set; }

        [CSVMapping("Tax invoice date/Credit note issue date", 1, CSVMappingAttribute.eDefaultValue.Dash)]
        public string DocDateCsv
        {
            get { return CommonUtil.TextDate(this.DocDate); }
        }

        [CSVMapping("Tax invoice no./Credit note no.", 2, CSVMappingAttribute.eDefaultValue.Dash)]
        public string DocNoCsv
        {
            get { return this.DocNo; }
        }

        [CSVMapping("VAT registrant name", 3, CSVMappingAttribute.eDefaultValue.Dash)]
        public string BillingClientNameCsv
        {
            get { return this.BillingCilentName; }
        }

        [CSVMapping("Tax ID", 4, CSVMappingAttribute.eDefaultValue.None)]
        public string TaxIDCsv
        {
            get { return this.TaxID; }
        }

        [CSVMapping("Head Office", 5, CSVMappingAttribute.eDefaultValue.None)]
        public string HeadOfficeCsv
        {
            get { return this.HeadOffice; }
        }

        [CSVMapping("Branch", 6, CSVMappingAttribute.eDefaultValue.None)]
        public string BranchCsv
        {
            get { return this.Branch; }
        }

        [CSVMapping("Currency", 7, CSVMappingAttribute.eDefaultValue.None)]
        public string CurrencyCsv
        {
            get
            {
                string txt = string.Empty;
                if (this.Currencies != null)
                {
                    DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.CurrencyType);
                    if (curr != null)
                    {
                        txt = curr.ValueDisplayEN;
                    }
                }
                return txt;
            }
        }


        [CSVMapping("Tax invoice amount (Ex.VAT)/ Credit note amount (Ex. VAT)", 8, CSVMappingAttribute.eDefaultValue.Zero)]
        public decimal? AmountExVATCsv
        {
            get { return this.AmountExVAT; }
        }

        [CSVMapping("VAT amount", 9, CSVMappingAttribute.eDefaultValue.Zero)]
        public decimal? VatAmountCsv
        {
            get { return this.VatAmount; }
        }

        [CSVMapping("CancelFlag", 10)]
        public string CancelFlagCsv
        {
            get
            {
                string txtValueCode;
                if (this.CancelFlag.GetValueOrDefault() == true)
                {
                    txtValueCode = AccountingConfig.C_ACC_CONFIG_NAME_FLAG_DISPLAY_YES;
                }
                else
                {
                    txtValueCode = AccountingConfig.C_ACC_CONFIG_NAME_FLAG_DISPLAY_NO;
                }

                string txt = string.Empty;
                if (this.FlagDisplay != null)
                {
                    DataEntity.Accounting.dtAccountingConfig disp = this.FlagDisplay.Find(x => x.ConfigName == txtValueCode);
                    if (disp != null)
                    {
                        txt = disp.ConfigValue;
                    }
                }
                return txt;
            }
        }

        [CSVMapping("Matching Date", 11, CSVMappingAttribute.eDefaultValue.Dash)]
        public string MatchDateCsv
        {
            get { return CommonUtil.TextDate(this.MatchDate); }
        }

        [CSVMapping("Payment Date", 12, CSVMappingAttribute.eDefaultValue.Dash)]
        public string PaymentDateCsv
        {
            get { return CommonUtil.TextDate(this.PaymentDate); }
        }

        [CSVMapping("Prefix", 13)]
        public string PrefixCsv
        {
            get { return this.Prefix; }
        }
    }
}
