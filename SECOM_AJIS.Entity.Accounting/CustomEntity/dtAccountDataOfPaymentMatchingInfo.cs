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
    public partial class dtAccountDataOfPaymentMatchingInfo
    {
        CommonUtil comUtil = new CommonUtil();
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }

        [CSVMapping("MatchID", 1)]
        public string MatchIDCsv
        {
            get
            {
                return this.MatchID;
            }
        }

        [CSVMapping("Payment trans No.", 2)]
        public string PaymentTransNoCsv
        {
            get { return this.PaymentTransNo; }
        }

        [CSVMapping("InvoiceNo", 3)]
        public string InvoiceNoCsv
        {
            get { return this.InvoiceNo; }
        }

        [CSVMapping("InvoiceOCC", 4)]
        public int InvoiceOCCCsv
        {
            get { return this.InvoiceOCC; }
        }

        //Add by Jutarat A. on 10072013
        [CSVMapping("TaxInvoiceNo", 5)]
        public string TaxInvoiceNoCsv
        {
            get { return this.TaxInvoiceNo; }
        }

        [CSVMapping("Tax Invoice Date", 6)]
        public string TaxInvoiceDateCsv
        {
            get { return CommonUtil.TextDate(this.TaxInvoiceDate); }
        }
        //End Add

        [CSVMapping("VAT registrant name (EN)", 7)]
        public string FullNameENCsv
        {
            get { return this.FullNameEN; }
        }

        [CSVMapping("VAT registrant ID no./Tax ID no.", 8)]
        public string IDNoCsv
        {
            get { return this.IDNo; }
        }

        [CSVMapping("Invoice Amount Currency Type", 9)]
        public string InvoiceAmountCurrencyCsv
        {
            get
            {

                string txt = string.Empty;
                if (this.Currencies != null)
                {
                    DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.InvoiceAmountCurrencyType);
                    if (curr != null)
                    {
                        txt = curr.ValueDisplayEN;
                    }
                }
                return txt;
            }
        }


        [CSVMapping("Invoice Amount", 10)]
        public decimal? InvoiceAmountCsv
        {
            get { return this.InvoiceAmount; }
        }

        [CSVMapping("VAT Amount", 11)]
        public decimal? VatAmountCsv
        {
            get { return this.VatAmount; }
        }

        [CSVMapping("Billing Type", 12)]
        public string BillingTypeCsv
        {
            get { return CommonUtil.TextCodeName(this.BillingTypeCode, this.BillingTypeNameEN); }
        }


        [CSVMapping("Service fee", 13)]
        public decimal? ServiceFeeCsv
        {
            get { return this.ServiceFee; }
        }

        [CSVMapping("VAT for service fee", 14)]
        public decimal? VatForServiceFeeCsv
        {
            get { return this.VatForServiceFee; }
        }


        [CSVMapping("WHT for service fee", 15)]
        public decimal? WhtForServiceFeeCsv
        {
            get { return this.WhtForServiceFee; }
        }


        [CSVMapping("WHT Amount", 16, CSVMappingAttribute.eDefaultValue.Zero)]
        public decimal? WHTAmountCsv
        {
            get { return this.WHTAmount; }
        }

        [CSVMapping("Bank Fee", 17, CSVMappingAttribute.eDefaultValue.Zero)]
        public decimal? BankFeeAmountCsv
        {
            get { return this.BankFeeAmount; }
        }

        [CSVMapping("Cash receive", 18, CSVMappingAttribute.eDefaultValue.Zero)]
        public decimal? CashReceiveCsv
        {
            get { return this.CashReceive; }
        }

        [CSVMapping("Other expense", 19, CSVMappingAttribute.eDefaultValue.Zero)]
        public decimal? OtherExpenseAmountCsv
        {
            get { return this.OtherExpenseAmount; }
        }

        [CSVMapping("Other income", 20, CSVMappingAttribute.eDefaultValue.Zero)]
        public decimal? OtherIncomeAmountCsv
        {
            get { return this.OtherIncomeAmount; }
        }

        [CSVMapping("Bank name", 21, CSVMappingAttribute.eDefaultValue.Dash)]
        public string BankNameCsv
        {
            get { return this.BankName; }
        }


        [CSVMapping("Branch code", 22, CSVMappingAttribute.eDefaultValue.Dash)]
        public string BankBranchCodeCsv
        {
            get { return this.BankBranchCode; }
        }


        [CSVMapping("Bank acc. name", 23, CSVMappingAttribute.eDefaultValue.Dash)]
        public string AccountNameCsv
        {
            get { return this.AccountName; }
        }

        [CSVMapping("Approve no.1", 24, CSVMappingAttribute.eDefaultValue.Dash)]
        public string ApproveNo1Csv
        {
            get { return this.ApproveNo1; }
        }

        [CSVMapping("Approve no.2", 25, CSVMappingAttribute.eDefaultValue.Dash)]
        public string ApproveNo2Csv
        {
            get { return this.ApproveNo2; }
        }

        [CSVMapping("Match Status", 26, CSVMappingAttribute.eDefaultValue.Dash)]
        public string MatchStatusCsv
        {
            get
            {
                return this.MatchStatus;
            }
        }


        [CSVMapping("CreateBy ID", 27)]
        public string CreateIDCsv
        {
            get { return this.CreateID; }
        }

        [CSVMapping("CreateBy Name", 28)]
        public string CreateNameCsv
        {
            get { return this.CreateName; }
        }

        [CSVMapping("CreateDate", 29)]
        public string CreateDateCsv
        {
            get { return CommonUtil.TextDate(this.CreateDate); }
        }


        [CSVMapping("UpdateBy ID", 30)]
        public string UpdateIDCsv
        {
            get { return this.UpdateID; }
        }

        [CSVMapping("UpdateBy Name", 31)]
        public string UpdateNameCsv
        {
            get { return this.UpdateName; }
        }

        [CSVMapping("UpdateDate", 32)]
        public string UpdateDateCsv
        {
            get { return CommonUtil.TextDate(this.UpdateDate); }
        }

    }
}
