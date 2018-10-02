using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Common.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Common
{
    public partial class dtRptAccountDataOfCarryOverAndProfit
    {
        //First section attribute
        [CSVMapping("Billing code", 1)]
        public string BillingCodeCsv
        {
            get {
                CommonUtil util = new CommonUtil();
                return util.ConvertBillingTargetCode(this.BillingCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        //Add by Jutarat A. on 21012014
        [CSVMapping("Billing target code", 2)]
        public string BillingTargetCodeCsv
        {
            get {
                CommonUtil util = new CommonUtil();
                return util.ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        //End Add

        [CSVMapping("Billing target name", 3)]
        public string BillingTargetNameCsv
        {
            get { return this.BillingTargetName; }
        }
        
        [CSVMapping("Billing office", 4)]
        public string BillingOfficeNameCsv
        {
            get { return this.BillingOfficeName; }
        }
        
        [CSVMapping("Operation date at first", 5)]
        public string FirstOperationDateCsv
        {
            get { return CommonUtil.TextDate(this.FirstOperationDate); }
        }
        
        [CSVMapping("Monthly billing amount of contract fee", 6)]
        public Nullable<decimal> MonthlyBillingAmountCsv
        {
            get { return this.MonthlyBillingAmount; }
        }

        
        //Last month section attribute
        [CSVMapping("[Last month] Accumulated advance receive of contract fee", 7)]
        public Nullable<decimal> LastAccumulatedReceiveAmountCsv
        {
            get { return this.LastAccumulatedReceiveAmount; }
        }
        
        [CSVMapping("[Last month] Accumulated unpaid", 8)]
        public Nullable<decimal> LastAccumulatedUnpaidCsv
        {
            get { return this.LastAccumulatedUnpaid; }
        }


        //This month secion attribute
        [CSVMapping("[This month] Received amount in this month", 9)]
        public Nullable<decimal> ReceiveAmountCsv
        {
            get { return this.ReceiveAmount; }
        }
        
        [CSVMapping("[This month] Income rental fee this month", 10)]
        public Nullable<decimal> IncomeRentalFeeCsv
        {
            get { return this.IncomeRentalFee; }
        }
        
        [CSVMapping("[This month] Accumulated advance receive of contract fee", 11)]
        public Nullable<decimal> AccumulatedReceiveAmountCsv
        {
            get { return this.AccumulatedReceiveAmount; }
        }
        
        [CSVMapping("[This month] Accumlated unpaid", 12)]
        public Nullable<decimal> AccumulatedUnpaidCsv
        {
            get { return this.AccumulatedUnpaid; }
        }
        
        [CSVMapping("[This month] Income VAT this month", 13)]
        public Nullable<decimal> IncomeVatCsv
        {
            get { return this.IncomeVat; }
        }

        [CSVMapping("[This month] Unpaid period (month)", 14)]
        public Nullable<decimal> UnpaidPeriodCsv
        {
            get { return this.UnpaidPeriod.GetValueOrDefault(0); }
        }

        [CSVMapping("[This month] Income date this month", 15)]
        public string IncomeDateCsv
        {
            get { return CommonUtil.TextDate(this.IncomeDate); }
        }


        //contract secion attribute
        [CSVMapping("Contract status of the specied contract", 16)]
        public string ContractStatusCsv
        {
            get { return this.ContractStatus; }
        }


        //Billing history section attribute
        //History 1
        [CSVMapping("[Billing history 1] Monthly amount", 17)]
        public Nullable<decimal> MonthlyBillingAmountHistory1Csv
        {
            get { return this.MonthlyBillingAmountHistory1; }
        }

        [CSVMapping("[Billing history 1] Applicable period start", 18)]
        public string BillingStartDateHistory1Csv
        {
            get { return CommonUtil.TextDate(this.BillingStartDateHistory1); }
        }

        [CSVMapping("[Billing history 1] Applicable period end", 19)]
        public string BillingEndDateHistory1Csv
        {
            get { return CommonUtil.TextDate(this.BillingEndDateHistory1); }
        }

        //History 2
        [CSVMapping("[Billing history 2] Monthly amount", 20)]
        public Nullable<decimal> MonthlyBillingAmountHistory2Csv
        {
            get { return this.MonthlyBillingAmountHistory2; }
        }

        [CSVMapping("[Billing history 2] Applicable period start", 21)]
        public string BillingStartDateHistory2Csv
        {
            get { return CommonUtil.TextDate(this.BillingStartDateHistory2); }
        }

        [CSVMapping("[Billing history 2] Applicable period end", 22)]
        public string BillingEndDateHistory2Csv
        {
            get { return CommonUtil.TextDate(this.BillingEndDateHistory2); }
        }

        //History 3
        [CSVMapping("[Billing history 3] Monthly amount", 23)]
        public Nullable<decimal> MonthlyBillingAmountHistory3Csv
        {
            get { return this.MonthlyBillingAmountHistory3; }
        }

        [CSVMapping("[Billing history 3] Applicable period start", 24)]
        public string BillingStartDateHistory3Csv
        {
            get { return CommonUtil.TextDate(this.BillingStartDateHistory3); }
        }

        [CSVMapping("[Billing history 3] Applicable period end", 25)]
        public string BillingEndDateHistory3Csv
        {
            get { return CommonUtil.TextDate(this.BillingEndDateHistory3); }
        }

        //History 4
        [CSVMapping("[Billing history 4] Monthly amount", 26)]
        public Nullable<decimal> MonthlyBillingAmountHistory4Csv
        {
            get { return this.MonthlyBillingAmountHistory4; }
        }

        [CSVMapping("[Billing history 4] Applicable period start", 27)]
        public string BillingStartDateHistory4Csv
        {
            get { return CommonUtil.TextDate(this.BillingStartDateHistory4); }
        }

        [CSVMapping("[Billing history 4] Applicable period end", 28)]
        public string BillingEndDateHistory4Csv
        {
            get { return CommonUtil.TextDate(this.BillingEndDateHistory4); }
        }

        //History 5
        [CSVMapping("[Billing history 5] Monthly amount", 29)]
        public Nullable<decimal> MonthlyBillingAmountHistory5Csv
        {
            get { return this.MonthlyBillingAmountHistory5; }
        }

        [CSVMapping("[Billing history 5] Applicable period start", 30)]
        public string BillingStartDateHistory5Csv
        {
            get { return CommonUtil.TextDate(this.BillingStartDateHistory5); }
        }

        [CSVMapping("[Billing history 5] Applicable period end", 31)]
        public string BillingEndDateHistory5Csv
        {
            get { return CommonUtil.TextDate(this.BillingEndDateHistory5); }
        }

        //History 6
        [CSVMapping("[Billing history 6] Monthly amount", 32)]
        public Nullable<decimal> MonthlyBillingAmountHistory6Csv
        {
            get { return this.MonthlyBillingAmountHistory6; }
        }

        [CSVMapping("[Billing history 6] Applicable period start", 33)]
        public string BillingStartDateHistory6Csv
        {
            get { return CommonUtil.TextDate(this.BillingStartDateHistory6); }
        }

        [CSVMapping("[Billing history 6] Applicable period end", 34)]
        public string BillingEndDateHistory6Csv
        {
            get { return CommonUtil.TextDate(this.BillingEndDateHistory6); }
        }

        //History 7
        [CSVMapping("[Billing history 7] Monthly amount", 35)]
        public Nullable<decimal> MonthlyBillingAmountHistory7Csv
        {
            get { return this.MonthlyBillingAmountHistory7; }
        }

        [CSVMapping("[Billing history 7] Applicable period start", 36)]
        public string BillingStartDateHistory7Csv
        {
            get { return CommonUtil.TextDate(this.BillingStartDateHistory7); }
        }

        [CSVMapping("[Billing history 7] Applicable period end", 37)]
        public string BillingEndDateHistory7Csv
        {
            get { return CommonUtil.TextDate(this.BillingEndDateHistory7); }
        }

        //History 8
        [CSVMapping("[Billing history 8] Monthly amount", 38)]
        public Nullable<decimal> MonthlyBillingAmountHistory8Csv
        {
            get { return this.MonthlyBillingAmountHistory8; }
        }

        [CSVMapping("[Billing history 8] Applicable period start", 39)]
        public string BillingStartDateHistory8Csv
        {
            get { return CommonUtil.TextDate(this.BillingStartDateHistory8); }
        }

        [CSVMapping("[Billing history 8] Applicable period end", 40)]
        public string BillingEndDateHistory8Csv
        {
            get { return CommonUtil.TextDate(this.BillingEndDateHistory8); }
        }

        //History 9
        [CSVMapping("[Billing history 9] Monthly amount", 41)]
        public Nullable<decimal> MonthlyBillingAmountHistory9Csv
        {
            get { return this.MonthlyBillingAmountHistory9; }
        }

        [CSVMapping("[Billing history 9] Applicable period start", 42)]
        public string BillingStartDateHistory9Csv
        {
            get { return CommonUtil.TextDate(this.BillingStartDateHistory9); }
        }

        [CSVMapping("[Billing history 9] Applicable period end", 43)]
        public string BillingEndDateHistory9Csv
        {
            get { return CommonUtil.TextDate(this.BillingEndDateHistory9); }
        }

        //History 10
        [CSVMapping("[Billing history 10] Monthly amount", 44)]
        public Nullable<decimal> MonthlyBillingAmountHistory10Csv
        {
            get { return this.MonthlyBillingAmountHistory10; }
        }

        [CSVMapping("[Billing history 10] Applicable period start", 45)]
        public string BillingStartDateHistory10Csv
        {
            get { return CommonUtil.TextDate(this.BillingStartDateHistory10); }
        }

        [CSVMapping("[Billing history 10] Applicable period end", 46)]
        public string BillingEndDateHistory10Csv
        {
            get { return CommonUtil.TextDate(this.BillingEndDateHistory10); }
        }
    }
}




