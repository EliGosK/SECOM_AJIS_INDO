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
    [MetadataType(typeof(dtViewBillingDetailListOfLastInvoiceOCC_MetaData))]
    public partial class dtViewBillingDetailListOfLastInvoiceOCC
    {
        [LanguageMapping]
        public string SecomBankName { get; set; }

        [LanguageMapping]
        public string SecomBranchName { get; set; }

        [LanguageMapping]
        public string SendingBankName { get; set; }

        [LanguageMapping]
        public string SendingBranchName { get; set; }

        [LanguageMapping]
        public string BillingTypeName { get; set; }
        
        CommonUtil cm = new CommonUtil();
        public string BillingCode
        {
            get
            {
                //return String.Format("{0} / {1} /<br/> {2}", SaleContractCodeShort, SaleContractOCC, MAContractCodeShort);
                return String.Format("{0}-{1}", ContractCode, BillingOCC);
            }
        }
        public string BillingCode_Short
        {
            get
            {
                //return String.Format("{0} / {1} /<br/> {2}", SaleContractCodeShort, SaleContractOCC, MAContractCodeShort);
                return String.Format("{0}-{1}", ContractCode_Short, BillingOCC);
            }
        }
        public string ContractCode_Short
        {
            get
            {
                //return String.Format("{0} / {1} /<br/> {2}", SaleContractCodeShort, SaleContractOCC, MAContractCodeShort);
                return cm.ConvertContractCode(ContractCode,CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
             
        public string SecomBankAndBranchName
        {
            get
            {

                return String.Format("{0}/{1}", SecomBankName, SecomBranchName);
            }
        }
        public string SendingBankAndBranchName
        {
            get
            {

                return String.Format("{0}/{1}", SendingBankName, SendingBranchName);
            }
        }
        public string BillingAmountNumeric
        {
            get
            {
                return CommonUtil.TextNumeric(BillingAmount);

            }
        }

        public string PaymentStatusName { get; set; }
        
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string TextTransferBillingAmount
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.BillingAmount);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.BillingAmountCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
    }
}

namespace SECOM_AJIS.DataEntity.Billing.MetaData
{
    public class dtViewBillingDetailListOfLastInvoiceOCC_MetaData
    {
        //[BillingTypeMappingAttribute("BillingTypeName")]
        [GridToolTip("BillingTypeName")]
        public string BillingTypeCode { get; set; }

        [PaymentStatusMappingAttribute("PaymentStatusName")]
        [GridToolTip("PaymentStatusName")]
        public string PaymentStatus { get; set; }

    }
}

