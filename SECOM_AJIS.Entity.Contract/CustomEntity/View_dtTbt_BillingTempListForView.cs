using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class View_dtTbt_BillingTempListForView : dtTbt_BillingTempListForView
    {
        public string BillingClientName
        {
            get { return string.Format("(1) {0}<br/>(2) {1}" , this.NameEN , this.NameLC); }
        }
        public string BillingOfficeEN
        {
            //get { return string.Format("{0}: {1}", this.BillingOfficeCode, this.BillingOfficeNameEN); }
            get { return this.BillingOfficeNameEN; }
        }

        public string BillingOfficeJP
        {
            //get { return string.Format("{0}: {1}", this.BillingOfficeCode, this.BillingOfficeNameJP); }
            get { return this.BillingOfficeNameJP; }
        }

        public string BillingOfficeLC
        {
            //get { return  string.Format("{0}: {1}", this.BillingOfficeCode, this.BillingOfficeNameLC); }
            get { return this.BillingOfficeNameLC; }
        }

        public string BillingOffice { get; set; }

        public string BillingClientCode_EX
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertBillingClientCode(this.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string BillingTiming_EX { get; set; }
        public string BillingType_EX { get; set; }
        public string Paymethod_EX { get; set; }
    }
}
