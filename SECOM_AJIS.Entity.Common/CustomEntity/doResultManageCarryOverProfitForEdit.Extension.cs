using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Common
{
    public partial class doResultManageCarryOverProfitForEdit
    {
        CommonUtil com = new CommonUtil();

        public string BillingCodeShow
        {
            get
            {
                return string.Format("{0}-{1}", com.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT), BillingOCC);
            }
        }

        public string ReceiveAmountShow
        {
            get { return ReceiveAmount.HasValue ? ReceiveAmount.Value.ToString("#,##0.00") : string.Empty; }
        }

        public string IncomeRentalFeeShow
        {
            get { return IncomeRentalFee.HasValue ? IncomeRentalFee.Value.ToString("#,##0.00") : string.Empty; }
        }

        public string AccumulatedReceiveAmountShow
        {
            get { return AccumulatedReceiveAmount.HasValue ? AccumulatedReceiveAmount.Value.ToString("#,##0.00") : string.Empty; }
        }

        public string AccumulatedUnpaidShow
        {
            get { return AccumulatedUnpaid.HasValue ? AccumulatedUnpaid.Value.ToString("#,##0.00") : string.Empty; }
        }

        public string IncomeVatShow
        {
            get { return IncomeVat.HasValue ? IncomeVat.Value.ToString("#,##0.00") : string.Empty; }
        }

        public string UnpaidPeriodShow
        {
            get { return UnpaidPeriod.ToString("#,##0.00"); }
        }

        public string IncomeDateStr { get { return IncomeDate.HasValue ? IncomeDate.Value.ToString("dd-MMM-yyyy") : string.Empty; } }
    }
}
