using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Function for retrieving debt tracing memo.
    /// </summary>
    public partial class doGetDebtTracingMemo
    {

        public string RegistrantString
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return this.RegistrantEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.RegistrantEN;
                }
                else
                {
                    return this.RegistrantLC;
                }
            }
        }

        public string TracingResultString
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return this.TracingResultEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.TracingResultJP;
                }
                else
                {
                    return this.TracingResultLC;
                }
            }
        }
        public string PaymentMethodTypeString
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return this.PaymentMethodTypeEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.PaymentMethodTypeJP;
                }
                else
                {
                    return this.PaymentMethodTypeLC;
                }
            }
        }
        public string DebtTracingLevelString
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return this.DebtTracingLevelEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.DebtTracingLevelJP;
                }
                else
                {
                    return this.DebtTracingLevelLC;
                }
            }
        }


        public string CreateDateString
        {
            get {
                return CommonUtil.TextDate(this.CreateDate);
            }
        }
        public string ExpectPaymentDateString
        {
            get
            {
                return CommonUtil.TextDate(this.ExpectPaymentDate);
            }
        }
        public string LastContactDateString
        {
            get
            {
                return CommonUtil.TextDate(this.LastContactDate);
            }
        }
    }
}