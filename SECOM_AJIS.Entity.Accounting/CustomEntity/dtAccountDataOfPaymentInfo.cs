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
    public partial class dtAccountDataOfPaymentInfo
    {
        CommonUtil comUtil = new CommonUtil();

        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }

        public List<dtAccountingConfig> FlagDisplay { get; set; }

        [CSVMapping("Payment trans No.", 1)]
        public string PaymentTransNoCsv
        {
            get
            {
                return this.PaymentTransNo;
            }
        }

        [CSVMapping("Payment type Code", 2)]
        public string PaymentTypeCodeCsv
        {
            get { return this.PaymentTypeCode; }
        }

        [CSVMapping("Payment type Name", 3)]
        public string PaymentTypeNameCsv
        {
            get { return this.PaymentTypeName; }
        }

        [CSVMapping("Payment date", 4)]
        public string PaymentDateCsv
        {
            get { return CommonUtil.TextDate(this.PaymentDate); }
        }

        [CSVMapping("Payment amount currency", 5)]
        public string PaymentAmountCurrecyCsv
        {
            get
            {
                string txt = string.Empty;
                if (this.Currencies != null)
                {
                    DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.PaymentAmountCurrencyType);
                    if (curr != null)
                    {
                        txt = curr.ValueDisplayEN;
                    }
                }
                return txt;
            }
        }

        [CSVMapping("Payment amount", 6, CSVMappingAttribute.eDefaultValue.Zero)]
        //public decimal PaymentAmountCsv
        public Nullable<decimal> PaymentAmountCsv
        {
            get { return this.PaymentAmount; }
        }


        [CSVMapping("SECOM Bank Acc. ID", 7, CSVMappingAttribute.eDefaultValue.Dash)]
        public int? SecomAccountIDCsv
        {
            get { return this.SecomAccountID; }
        }

        [CSVMapping("SECOM Bank Acc. No.", 8, CSVMappingAttribute.eDefaultValue.Dash)]
        public string SecomAccountNoCsv
        {
            get { return this.SecomAccountNo; }
        }


        [CSVMapping("SECOM Bank Acc. Name", 9, CSVMappingAttribute.eDefaultValue.Dash)]
        public string SecomAccountNameCsv
        {
            get { return this.SecomAccountName; }
        }

        [CSVMapping("SECOM Bank Bank code", 10, CSVMappingAttribute.eDefaultValue.Dash)]
        public string SecomBankCodeCsv
        {
            get { return this.SecomBankCode; }
        }

        [CSVMapping("SECOM Bank Bank name (En)", 11, CSVMappingAttribute.eDefaultValue.Dash)]
        public string SecomBankNameCsv
        {
            get { return this.SecomBankName; }
        }

        [CSVMapping("SECOM Bank Branch code", 12, CSVMappingAttribute.eDefaultValue.Dash)]
        public string SecomBankBranchCodeCsv
        {
            get { return this.SecomBankBranchCode; }
        }

        [CSVMapping("SECOM Bank Branch name (En)", 13, CSVMappingAttribute.eDefaultValue.Dash)]
        public string SecomBankBranchNameCsv
        {
            get { return this.SecomBankBranchName; }
        }

        [CSVMapping("Payer Name", 14, CSVMappingAttribute.eDefaultValue.Dash)]
        public string PayerCsv
        {
            get { return this.Payer; }
        }

        [CSVMapping("Payer Bank acc. no.", 15, CSVMappingAttribute.eDefaultValue.Dash)]
        public string PayerBankAccNoCsv
        {
            get { return this.PayerBankAccNo; }
        }

        [CSVMapping("Sending Bank Bank code", 16, CSVMappingAttribute.eDefaultValue.Dash)]
        public string SendingBankCodeCsv
        {
            get { return this.SendingBankCode; }
        }

        [CSVMapping("Sending Bank Bank name (En)", 17, CSVMappingAttribute.eDefaultValue.Dash)]
        public string SendingBankNameCsv
        {
            get { return this.SendingBankName; }
        }

        [CSVMapping("Sending Bank Branch code", 18, CSVMappingAttribute.eDefaultValue.Dash)]
        public string SendingBranchCodeCsv
        {
            get { return this.SendingBranchCode; }
        }

        [CSVMapping("Sending Bank Branch name (En)", 19, CSVMappingAttribute.eDefaultValue.Dash)]
        public string SendingBankBranchNameCsv
        {
            get { return this.SendingBankBranchName; }
        }



        [CSVMapping("Telephone no.", 20, CSVMappingAttribute.eDefaultValue.Dash)]
        public string TelNoCsv
        {
            get { return this.TelNo; }
        }

        [CSVMapping("Memo", 21, CSVMappingAttribute.eDefaultValue.Dash)]
        public string MemoCsv
        {
            get { return this.Memo; }
        }

        [CSVMapping("System Method", 22)]
        public string SystemMethodCsv
        {
            get { return this.SystemMethod; }
        }

        [CSVMapping("Delete Flag", 23)]
        public string DeleteFlagCsv
        {
            get
            {
                string txtValueCode;
                if (this.DeleteFlag.GetValueOrDefault() == true)
                {
                    txtValueCode = AccountingConfig.C_ACC_CONFIG_NAME_FLAG_DISPLAY_YES;
                }
                else {
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

        [CSVMapping("CreateBy ID", 24)]
        public string CreateIDCsv
        {
            get { return this.CreateID; }

        }

        [CSVMapping("CreateBy Name", 25)]
        public string CreateNameCsv
        {
            get { return this.CreateName; }
        }

        [CSVMapping("CreateDate", 26)]
        public string CreateDateCsv
        {
            get { return CommonUtil.TextDate(this.CreateDate); }
        }


        [CSVMapping("UpdateBy ID", 27)]
        public string UpdateIDCsv
        {
            get { return this.UpdateID; }
        }

        [CSVMapping("UpdateBy Name", 28)]
        public string UpdateNameCsv
        {
            get { return this.UpdateName; }
        }

        [CSVMapping("UpdateDate", 29)]
        public string UpdateDateCsv
        {
            get { return CommonUtil.TextDate(this.UpdateDate); }
        }
    }
}
