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
    public partial class dtAccountDataOfAgingReport
    {
        CommonUtil comUtil = new CommonUtil();

        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }


        //First section attribute
        [CSVMapping("Billing office", 1)]
        public string BillingOfficeCsv
        {
            get { return this.BillingOffice; }
        }

        [CSVMapping("Billing customer name", 2)]
        public string BillingTargetNameCsv
        {
            get { return this.BillingTargetName; }
        }

        [CSVMapping("Contract no.", 3)]
        public string ContractCodeCsv
        {
            get
            {
                return comUtil.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        [CSVMapping("Billing customer code", 4)]
        public string BillingTargetCodeCsv
        {
            get { return comUtil.ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT); }
        }

        [CSVMapping("Last change type", 5)]
        public string LastChangeTypeCsv
        {
            get { return this.LastChangeType; }
        }

        [CSVMapping("Last change date", 6)]
        public string LastChangeDateCsv
        {
            get { return CommonUtil.TextDate(this.LastChangeDate); }
        }

        [CSVMapping("Product name (En)", 7)]
        public string ProductNameCsv
        {
            get { return this.ProductName; }
        }

        [CSVMapping("Payment term", 8, CSVMappingAttribute.eDefaultValue.Zero)]
        public Nullable<int> BillingCycleCsv
        {
            get { return this.BillingCycle; }
        }

        [CSVMapping("Credit term", 9, CSVMappingAttribute.eDefaultValue.Zero)]
        public Nullable<int> CreditTermCsv
        {
            get { return this.CreditTerm; }
        }

        [CSVMapping("Payment method", 10)]
        public string PaymentMethodCsv
        {
            get { return this.PaymentMethod; }
        }

        [CSVMapping("Invoice no.", 11)]
        public string InvoiceNoCsv
        {
            get { return this.InvoiceNo; }
        }

        [CSVMapping("Invoice date", 12)]
        public string InvoiceDateCsv
        {
            get { return CommonUtil.TextDate(this.InvoiceDate); }
        }

        [CSVMapping("Payment due date", 13)]
        public string PaymentDueDateCsv
        {
            get { return CommonUtil.TextDate(this.PaymentDueDate); }
        }

        [CSVMapping("Billing period from", 14)]
        public string BillingStartDateCsv
        {
            get { return CommonUtil.TextDate(this.BillingStartDate); }
        }

        [CSVMapping("Billing period to", 15)]
        public string BillingEndDateCsv
        {
            get { return CommonUtil.TextDate(this.BillingEndDate); }
        }

        [CSVMapping("Currency", 16)]
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

        [CSVMapping("Invoice amount", 17, CSVMappingAttribute.eDefaultValue.Zero)]
        public Nullable<decimal> InvoiceAmountCsv
        {
            get { return this.InvoiceAmount; }
        }

        [CSVMapping("VAT", 18, CSVMappingAttribute.eDefaultValue.Zero)]
        public Nullable<decimal> VatAmountCsv
        {
            get { return this.VatAmount; }
        }

        [CSVMapping("Total", 19, CSVMappingAttribute.eDefaultValue.Zero)]
        public Nullable<decimal> TotalAmountCsv
        {
            get { return this.TotalAmount; }
        }

        [CSVMapping("Remark amount of credit note", 20)]
        public Nullable<decimal> CreditNoteAmountCsv
        {
            get { return this.CreditNoteAmount; }
        }

        [CSVMapping("Overdue date", 21, CSVMappingAttribute.eDefaultValue.Zero)]
        public Nullable<int> OverDueDateCsv
        {
            get { return this.OverDueDate; }
        }

        [CSVMapping("Overdue month", 22, CSVMappingAttribute.eDefaultValue.Zero)]
        public Nullable<decimal> OverDueMonthCsv
        {
            get { return this.OverDueMonth; }
        }

        [CSVMapping("Overdue 0-3", 23, CSVMappingAttribute.eDefaultValue.Zero)]
        public Nullable<decimal> OverDue0_3Csv
        {
            get { return this.OverDue0_3; }
        }

        [CSVMapping("Overdue 3-6", 24, CSVMappingAttribute.eDefaultValue.Zero)]
        public Nullable<decimal> OverDue3_6Csv
        {
            get { return this.OverDue3_6; }
        }

        [CSVMapping("Overdue 6-12", 25, CSVMappingAttribute.eDefaultValue.Zero)]
        public Nullable<decimal> OverDue6_12Csv
        {
            get { return this.OverDue6_12; }
        }

        [CSVMapping("Overdue over 12", 26, CSVMappingAttribute.eDefaultValue.Zero)]
        public Nullable<decimal> OverDueOver12Csv
        {
            get { return this.OverDueOver12; }
        }
    }
}
