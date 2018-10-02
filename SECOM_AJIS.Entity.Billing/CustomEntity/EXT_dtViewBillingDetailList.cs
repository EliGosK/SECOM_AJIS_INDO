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
    [MetadataType(typeof(dtViewBillingDetailList_MetaData))]
    public partial class dtViewBillingDetailList
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

        //public string CreditNoteTooltip
        //{
        //    get
        //    {
                
        //        return CreditNoteInvoiceForTooltip.Replace();
        //    }
        //}

        CommonUtil cm = new CommonUtil();
        public string BillingCode
        {
            get
            {
                //return String.Format("{0} / {1} /<br/> {2}", SaleContractCodeShort, SaleContractOCC, MAContractCodeShort);
                return String.Format("{0}-{1}", ContractCode, BillingOCC);
            }
        }

        public string ContractCode_Short
        {
            get
            {
                //return String.Format("{0} / {1} /<br/> {2}", SaleContractCodeShort, SaleContractOCC, MAContractCodeShort);
                //CommonUtil cm = new CommonUtil();
                return cm.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string BillingCode_Short
        {
            get
            {
                
                if (CommonUtil.IsNullOrEmpty(ContractCode_Short) && CommonUtil.IsNullOrEmpty(BillingOCC))
                {
                    return "-";
                }
                else
                {
                    //return String.Format("{0}-{1}", ContractCode_Short, BillingOCC);
                    return String.Format("{0}-{1}", CommonUtil.IsNullOrEmpty(this.ContractCode_Short) ? "-" : this.ContractCode_Short,
                   CommonUtil.IsNullOrEmpty(this.BillingOCC) ? "-" : this.BillingOCC);
                }              
            }
        }

        public string SecomBankAndBranchName
        {
            get
            {
                if (String.IsNullOrEmpty(this.SecomBankName) && String.IsNullOrEmpty(this.SecomBranchName))
                {
                    return "-";
                }
                else
                {
                    return String.Format("{0}/{1}", String.IsNullOrEmpty(this.SecomBankName) ? "-" : this.SecomBankName,
                    String.IsNullOrEmpty(this.SecomBranchName) ? "-" : this.SecomBranchName);
                }               
            }
        }

        public string SendingBankAndBranchName
        {
            get
            {
                if (String.IsNullOrEmpty(this.SendingBankName) && String.IsNullOrEmpty(this.SendingBranchName))
                {
                    return "-";
                }
                else
                {
                    return String.Format("{0}/{1}", String.IsNullOrEmpty(this.SendingBankName) ? "-" : this.SendingBankName,
                   String.IsNullOrEmpty(this.SendingBranchName) ? "-" : this.SendingBranchName);
                }               
            }
        }

        public string BillingAmountNumeric
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(BillingAmount)) ? "0.00" : CommonUtil.TextNumeric(BillingAmount);

            }
        }

        public string PaymentStatusName { get; set; }

        public int BillingDetailNo_Numeric
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(BillingDetailNo)) ? 0 : BillingDetailNo;
              
            }
        }

        public string InvoiceNo_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(InvoiceNo)) ? "-" : InvoiceNo;
            }
        }

        public string IssueInvDate_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(IssueInvDate)) ? "-" : CommonUtil.TextDate(IssueInvDate);
            }
        }

        public string BillingTypeCode_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(BillingTypeCode)) ? "-" : BillingTypeCode;
            }
        }

        public string PaymentStatus_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(PaymentStatus)) ? "-" : PaymentStatus;
            }
        }

        public string BillingStartDate_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(BillingStartDate)) ? "-" : CommonUtil.TextDate(BillingStartDate);
            }
        }

        public string BillingEndDate_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(BillingEndDate)) ? "-" : CommonUtil.TextDate(BillingEndDate);
            }
        }

        public string CreateDate_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(CreateDate)) ? "-" : CommonUtil.TextDate(CreateDate);
            }
        }

        public string PaymentDate_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(PaymentDate)) ? "-" : CommonUtil.TextDate(PaymentDate);
            }
        }

        public string MatchDate_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(MatchDate)) ? "-" : CommonUtil.TextDate(MatchDate);
            }
        }

        public string ReasonForFailure_Text
        {
            get
            {
                return (CommonUtil.IsNullOrEmpty(ReasonForFailure)) ? "-" : ReasonForFailure;
            }
        }

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
    public class dtViewBillingDetailList_MetaData
    {
        //[BillingTypeMappingAttribute("BillingTypeName")]        
        //public string BillingTypeCode { get; set; }

        [PaymentStatusMappingAttribute("PaymentStatusName")]        
        public string PaymentStatus { get; set; }

        [GridToolTip("BillingTypeName")]
        public string BillingTypeCode_Text { get; set; }

        [GridToolTip("PaymentStatusName")]
        public string PaymentStatus_Text { get; set; }

    }
}

