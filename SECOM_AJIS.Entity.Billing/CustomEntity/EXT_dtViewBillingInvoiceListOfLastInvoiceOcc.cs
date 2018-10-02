using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Billing.MetaData;

namespace SECOM_AJIS.DataEntity.Billing
{
    /// <summary>
    /// Do Of view billing invoice list of last invoice occ
    /// </summary>
    [MetadataType(typeof(dtViewBillingInvoiceListOfLastInvoiceOcc_MetaData))]
    public partial class dtViewBillingInvoiceListOfLastInvoiceOcc
    {

        //public string CreditNoteInvoiceForTooltip { get; set; }
        CommonUtil cm = new CommonUtil();
        public string Name
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}<br/>(3) {2}",
                String.IsNullOrEmpty(this.NameEN) ? "-" : this.NameEN,
                String.IsNullOrEmpty(this.NameLC) ? "-" : this.NameLC,
                String.IsNullOrEmpty(this.IDNo) ? "-" : this.IDNo);
            }
        }      
      
        public string InvoiceNoAndTaxNo
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}<br/>(3) {2}",
                String.IsNullOrEmpty(this.InvoiceNo) ? "-" : this.InvoiceNo,
                String.IsNullOrEmpty(this.TaxInvoiceNo) ? "-" : this.TaxInvoiceNo,
                String.IsNullOrEmpty(this.ReceiptNo) ? "-" : this.ReceiptNo);
            }
        }

        public string TaxAmount
        {
            get
            {
                              
                //return string.Format("(1) {0}<br/>(2) {1}",CommonUtil.TextNumeric(VatAmount),CommonUtil.TextNumeric(WHTAmount));              
                return string.Format("(1) {0}<br/>(2) {1}",
                   (VatAmount == null) ? "-" : CommonUtil.TextNumeric(VatAmount),
                   (WHTAmount == null) ? "-" : CommonUtil.TextNumeric(WHTAmount));              
            }
        }

        public string InvoicePaymentStatusName { get; set; }

        public string NoOfBillingDetail_Text
        {
            get
            {
                
                return CommonUtil.TextNumeric(NoOfBillingDetail);
            }
        }

        public string InvoicePaymentStatusMapping
        {
            get
            {
                return CommonUtil.TextCodeName(InvoicePaymentStatus,InvoicePaymentStatusName);
            }
        }
    }
}

namespace SECOM_AJIS.DataEntity.Billing.MetaData
{
    /// <summary>
    /// Do Of view billing invoice list of last invoice occ meta data
    /// </summary>
    public class dtViewBillingInvoiceListOfLastInvoiceOcc_MetaData
    {
        [PaymentStatuaMappingAttribute("InvoicePaymentStatusName")]
        public string InvoicePaymentStatus { get; set; }


    }
}


