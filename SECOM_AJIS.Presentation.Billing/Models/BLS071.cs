using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.Presentation.Common.Helpers;

namespace SECOM_AJIS.Presentation.Billing.Models
{
    /// <summary>
    /// Screen parameter of BLS071
    /// </summary>
    public class BLS071_ScreenParameter : ScreenParameter
    {
        public tbt_BillingTarget doBillingTarget { get ; set; }

        public doBillingDetail doBillingDetail { get; set; }

        public List<doBillingDetail> doSelectedBillingDetailList { get; set; }
        public List<doBillingDetail> dtOldBillingDetailList { get; set; }

        public List<BLS071_BillingDetail> doBillingDetailForCombineList { get; set; }

        public string strInvoiceNo { get; set; }
        public string currency { get; set; }
        public string currencyCode { get; set; }
       
    }

    /// <summary>
    /// Inheritance do of billing detail for BLS071
    /// </summary>
    public class BLS071_BillingDetail : doBillingDetail
    {
        CommonUtil cm = new CommonUtil();

        private string _ContractCodeShort;
        public string ContractCodeShort
        {
            get
            {

                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _ContractCodeShort = value; }
        }


        public string BillingCode
        {
            get
            {
                //return String.Format("{0} / {1} /<br/> {2}", SaleContractCodeShort, SaleContractOCC, MAContractCodeShort);
                return String.Format("{0}-{1}", ContractCode, BillingOCC);
            }
        }
        
        public string BillingType
        {
            get
            {
                //return String.Format("{0} / {1} /<br/> {2}", SaleContractCodeShort, SaleContractOCC, MAContractCodeShort);
                return CommonUtil.TextCodeName(BillingTypeCode,InvoiceDescription);
            }
        }

        public string SiteName
        {
            get
            {
                return String.Format("(1) {0}<br/>(2) {1}", SiteNameEN, SiteNameLC);
                
            }
        }

        public string BillingAmountNumeric
        {
            get
            {
                return BillingAmountCurrencyTypeName + " " + CommonUtil.TextNumeric(BillingAmount);
            }
        }  
    }

    /// <summary>
    /// Temp billing for filter
    /// </summary>
    public class BLS071_TempBillingForFilter
    {
        public string BillingCode { get; set; }
        public string BillingDetailNo { get; set; }
        public string key { get { return this.BillingCode + this.BillingDetailNo; } }
    }
}