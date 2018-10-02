using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Billing.MetaData;

namespace SECOM_AJIS.DataEntity.Billing
{

    public partial class dtRptPaymentForm
    {
        public string RPT_BillingClientName
        {
            get
            {
                string str = "-";

                if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                {
                    str = this.BillingClientNameEN;
                }
                else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                {
                    str = this.BillingClientNameLC;
                }

                return str;

            }
        }

        public string RPT_BillingClientCode
        {
            get
            {
                CommonUtil cm = new CommonUtil();
                //string str = string.IsNullOrEmpty(this.BillingClientCode) ? "-" : string.Format("{0}{1}", "0001", cm.ConvertBillingClientCode(this.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT));
                string str = string.IsNullOrEmpty(this.BillingClientCode) ? "-" : this.BillingClientCode; //Modify by Jutarat A. on 13122013

                // - Modified by Nontawat L. on 04-Jul-2014 : แทรก "1" ที่หลังตัวเลขตัวที่ 3 (ของเดิมเลขผิด)
                return string.Format("{0}{1}", " ", string.Join("  ", str.ToCharArray()));
                //return string.Format("{0}{1}", " ", string.Join("  ", str.Insert(3, "1").ToCharArray()));
            }
        }

        public string RPT_InvoiceNo
        {
            get
            {
                string str = "-";
                if (string.IsNullOrEmpty(this.InvoiceNo) == false)
                {
                    //Modify by Jutarat A. on 12122013
                    //str = this.InvoiceNo.ToUpper().Replace("A", "");
                    //str = str.Substring(2, str.Length - 2);
                    str = this.InvoiceNo.ToUpper();
                    //End Modify
                }

                return string.Format("{0}{1}", " ", string.Join("  ", str.ToCharArray()));

            }
        }

    }
}

