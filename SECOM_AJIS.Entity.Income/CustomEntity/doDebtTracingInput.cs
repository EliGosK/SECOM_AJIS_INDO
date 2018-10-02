using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Income.MetaData;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data ojbect of debt tracing input
    /// </summary>
    [MetadataType(typeof(doDebtTracingInput_Meta))]
    public partial class doDebtTracingInput
    {
        public string BillingTargetCode { get; set; }
        public string BillingOfficeCode { get; set; }
        public string ServiceTypeCode { get; set; }
        public string Result { get; set; }
        public DateTime? NextCallDate { get; set; }
        public DateTime? EstimateDate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountUsd { get; set; }
        public string AmountCurrencyType { get; set; }
        public string PaymentMethod { get; set; }
        public string PostponeReason { get; set; }
        public string Remark { get; set; }
        public List<string> InvoiceNoList { get; set; }

        public bool? IsValidNextCallDate
        {
            get
            {
                if (this.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_BRANCH
                    || this.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_HQ
                    || this.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_LAWSUIT
                    || this.NextCallDate != null)
                    return true;
                else
                    return null;
            }
        }

        public bool? IsValidEstimateDate
        {
            get
            {
                if (this.Result != DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_FOR_PAYMENT || this.EstimateDate != null)
                    return true;
                else
                    return null;
            }
        }

        public bool? IsValidAmount
        {
            get
            {
                if (this.Result != DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_FOR_PAYMENT || this.Amount != null)
                    return true;
                else
                    return null;
            }
        }

        public bool? IsValidPaymentMethod
        {
            get
            {
                if (this.Result != DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_FOR_PAYMENT || this.PaymentMethod != null)
                    return true;
                else
                    return null;
            }
        }

        public bool? IsValidPostponeReason
        {
            get
            {
                if (this.Result != DebtTracingResult.C_DEBT_TRACE_RESULT_POSTPONE || this.PostponeReason != null)
                    return true;
                else
                    return null;
            }
        }

    }

    public class doDebtTracingInput_Meta : doDebtTracingInput
    {
        [NotNullOrEmpty]
        public string BillingTargetCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                        Screen = "ICS140",
                        Parameter = "lblResult",
                        ControlName = "cboInputResult",
                        Order = 1)]
        public string Result { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                        Screen = "ICS140",
                        Parameter = "lblNextCallDate",
                        ControlName = "txtNextCallDate",
                        Order = 2)]
        public bool? IsValidNextCallDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                        Screen = "ICS140",
                        Parameter = "lblEstimateDate",
                        ControlName = "txtInputEstimateDate",
                        Order = 3)]
        public bool? IsValidEstimateDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                        Screen = "ICS140",
                        Parameter = "lblAmount",
                        ControlName = "txtInputAmount",
                        Order = 4)]
        public bool? IsValidAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                        Screen = "ICS140",
                        Parameter = "lblPaymentMethod",
                        ControlName = "cboInputPaymentMethod",
                        Order = 5)]
        public bool? IsValidPaymentMethod { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                        Screen = "ICS140",
                        Parameter = "lblPostponeReason",
                        ControlName = "cboInputPostponeReason",
                        Order = 6)]
        public bool? IsValidPostponeReason { get; set; }

    }
}
