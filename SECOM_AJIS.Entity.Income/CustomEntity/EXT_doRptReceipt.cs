using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object for receipt report.
    /// </summary>
    public partial class doRptReceipt : ICloneable
    {
        #region Header
        public string RPT_AttentionTo { get; set; }
        public string RPT_InvoicePaper { set; get; }
        public int TotalPage { set; get; }

        public string RPT_ReceiptDate
        {
            get
            {
                string resultDate = string.Empty;
                if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_CHRISTIAN)
                {
                    resultDate = this.ReceiptDate;
                }
                else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_BUDDHIST)
                {
                    resultDate = this.ReceiptDate_Buddhist;
                }
                //Do nothing for ShowIssueDate.C_SHOW_ISSUE_DATE_NOT status
                //else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_NOT)
                //{
                //    receiptDate = string.Empty;
                //}
                return resultDate;
            }
        }
        public string RPT_BillingClientName
        {
            get
            {
                string str = "-",
                    billingClientName = "-",
                    branchName = "-";
                bool fullbranchName = false;

                if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                {
                    billingClientName = this.BillingClientNameEN;
                    fullbranchName = (this.BranchNo != "-" && this.BranchNo != "00000" && this.BranchNameEN != "-");
                }
                else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                {
                    billingClientName = this.BillingClientNameLC;
                    fullbranchName = (this.BranchNo != "-" && this.BranchNo != "00000" && this.BranchNameLC != "-");
                }

                branchName = this.RPT_BranchName;

                if (branchName == "-")
                {
                    str = billingClientName;
                }
                else if (fullbranchName)
                {
                    str = string.Format("{0}\n{1}", billingClientName, branchName);
                }
                else
                {
                    str = string.Format("{0} {1}", billingClientName, branchName);
                }

                return str;
            }
        }
        public string RPT_BillingClientAddress
        {
            get
            {
                string str = "-";

                if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                {
                    str = this.BillingClientAddressEN;
                }
                else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                {
                    str = this.BillingClientAddressLC;
                }

                return str;
            }
        }

        public string RPT_RealBillingClientName
        {
            get
            {
                string str = "-";

                if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                {
                    str = this.RealBillingClientNameEN;
                }
                else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                {
                    str = this.RealBillingClientNameLC;
                }

                return str;
            }
        }

        public string RPT_RealBillingClientAddress
        {
            get
            {
                string str = "-";

                if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                {
                    str = this.RealBillingClientAddressEN;
                }
                else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                {
                    str = this.RealBillingClientAddressLC;
                }

                return str;
            }
        }

        public string RPT_BillingOfficeName
        {
            get; set;
            // Comment by Jirawat Jannt on 2016-12-13
            //get
            //{
            //    if (BillingOfficeCode == OfficeCode.C_INV_OFFICE_INDO)
            //        return OfficeName.C_INV_OFFICE_INDO;
            //    else
            //        return OfficeName.C_INV_OFFICE_SNR;
            //}
        }

        #endregion

        #region Detail
        public string RPT_SortingType
        {
            get
            {
                string str = string.Empty;
                if (this.SeparateInvType == SECOM_AJIS.Common.Util.ConstantValue.SeparateInvType.C_SEP_INV_SORT_ASCE ||
                    this.SeparateInvType == SECOM_AJIS.Common.Util.ConstantValue.SeparateInvType.C_SEP_INV_SORT_DESC ||
                    this.SeparateInvType == SECOM_AJIS.Common.Util.ConstantValue.SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_ASCE ||
                    this.SeparateInvType == SECOM_AJIS.Common.Util.ConstantValue.SeparateInvType.C_SEP_INV_SAME_TYPE_SORT_DESC
                    )
                {
                    str = this.SortingType;
                }
                return str;
            }
        }
        public string RPT_InvoiceDescription
        {
            get
            {
                string str = string.Empty;

                if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                {
                    str = this.InvoiceDescriptionEN;
                }
                else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                {
                    str = this.InvoiceDescriptionLC;
                }

                return str;
            }
        }
        public string RPT_PeriodOfRental
        {
            get
            {
                if (this.FirstFeeFlag)
                {
                    if (this.BillingTypeCode == BillingType.C_BILLING_TYPE_SERVICE
                        || this.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_SERVICE
                        || this.BillingTypeCode == BillingType.C_BILLING_TYPE_MA
                        || this.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_MA
                        || this.BillingTypeCode == BillingType.C_BILLING_TYPE_SG
                        || this.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_SG
                        || this.BillingTypeCode == BillingType.C_BILLING_TYPE_MA_RESULT_BASE
                        || this.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_MA_RESULT_BASE)
                    {
                        if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                        {
                            return ReceiptDocument.C_RECEIPT_FIRST_FEE_EN;
                        }
                        else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                        {
                            return ReceiptDocument.C_RECEIPT_FIRST_FEE_TH;
                        }
                    }
                }
                return string.Empty;
            }
        }
        public string RPT_ContractCode
        {
            get
            {
                string str = string.Empty;

                CommonUtil cm = new CommonUtil();
                if (string.IsNullOrEmpty(this.ContractCode) == false && this.ContractCode.Length > 0)
                {
                    if (this.ContractCode.Substring(0, 1).ToUpper() != "P")
                    {
                        str = cm.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                    }
                    else
                    {
                        str = this.ContractCode;
                    }
                }

                string strReturn = string.Empty;

                if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                {
                    strReturn = string.Format("{0} {1}", InvoiceDocument.C_CONTRACT_NO_LABEL_TH, str);
                }
                else
                {
                    strReturn = string.Format("{0} {1}", InvoiceDocument.C_CONTRACT_NO_LABEL_EN, str);
                }

                return strReturn;
            }
        }
        public string RPT_SiteName
        {
            get
            {
                string str = string.Empty;

                if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                {
                    str = this.SiteNameEN;
                }
                else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                {
                    str = this.SiteNameLC;
                }
                return str;
            }
        }
        public string RPT_Period
        {
            get
            {
                string period = string.Empty;
                if (this.BillingTypeGroup == SECOM_AJIS.Common.Util.ConstantValue.BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES
                    || this.BillingTypeGroup == SECOM_AJIS.Common.Util.ConstantValue.BillingTypeGroup.C_BILLING_TYPE_GROUP_DIFF_AMOUNT) //Add by Jutarat A. on 11062013
                {
                    if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_CHRISTIAN)
                    {
                        period = string.Format("{0} - {1}", this.BillingStartDate, this.BillingEndDate);
                    }
                    else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_BUDDHIST)
                    {
                        period = string.Format("{0} - {1}", this.BillingStartDate_Buddhist, this.BillingEndDate_Buddhist);
                    }
                    else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_NOT)
                    {
                        //Confirm sa on 19/Jun/2012, Display same as christian format
                        period = string.Format("{0} - {1}", this.BillingStartDate, this.BillingEndDate);
                    }
                }
                return period;
            }
        }
        public string RPT_Amount
        {
            get
            {
                string str = this.RPT_AmountVal.ToString("N2");
                return string.Format("{0} {1}", RPT_AmountCurrencyTypeName, str);
            }
        }
        public string RPT_AmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.BillingAmountCurrencyType);
            }
        }

        public decimal RPT_AmountVal
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return BillingAmount;
                else if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return BillingAmountUsd;
                else return 0;
            }
        }
        #endregion

        #region Footer
        public string RPT_InvoiceDate
        {
            get
            {
                string resultdate = string.Empty;
                if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_CHRISTIAN)
                {
                    resultdate = this.IssueInvDate;
                }
                else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_BUDDHIST)
                {
                    resultdate = this.IssueInvDate_Buddhist;
                }
                else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_NOT)
                {
                    //Confirm sa on 19/Jun/2012, Display same as christian format
                    resultdate = this.IssueInvDate;
                }
                return resultdate;
            }
        }
        public string RPT_GrandTotal
        {
            get
            {

                if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return string.Format("{0} {1}", RPT_InvoiceAmountCurrencyTypeName, InvoiceAmount.ToString("N2"));
                else if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    return string.Format("{0} {1}", RPT_InvoiceAmountCurrencyTypeName, InvoiceAmountUsd.ToString("N2"));
                else
                    return RPT_InvoiceAmountCurrencyTypeName + " 0.00";
            }
        }
        public string RPT_VATRate
        {
            get
            {
                //Db 0.07, Convert before display
                //string str = (this.VATRate * 100).ToString("N0");
                //return str;
                string str = string.Empty;
                decimal vat = this.VATRate * 100M;
                if (vat >= 0M)
                {
                    str = vat.ToString("N0") + " %";
                }

                return str;
            }
        }
        public string RPT_VATAmount
        {
            get
            {
                //if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                //    return string.Format("{0} {1}", VatAmountCurrencyTypeName, VATAmount.ToString("N2"));
                //else if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                //    return string.Format("{0} {1}", VatAmountCurrencyTypeName, VatAmountUsd.ToString("N2"));
                //else
                //    return string.Format("{0} 0.00", VatAmountCurrencyTypeName);
                string str = string.Empty;

                if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    str = VatAmountCurrencyTypeName + " " + VATAmount.ToString("N2");
                else
                    str = VatAmountCurrencyTypeName + " " + VatAmountUsd.ToString("N2");
                return str;
            }
        }

        public string RPT_NetTatalWord
        {
            get
            {
                string str = string.Empty;

                if(this.InvoiceAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                {
                    if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                    {
                        str = ReportUtil.CurrencyToEnglishWords(Convert.ToDouble(this.InvoiceAmountUsd + this.VatAmountUsd));
                    }
                    else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                    {
                        //str = ReportUtil.CurrencyToThaiWords(Convert.ToDouble(this.InvoiceAmount + this.VATAmount)); // comment by jirawat jannet on 2016-11-16
                        str = ReportUtil.CurrencyToEnglishWords(Convert.ToDouble(this.InvoiceAmountUsd + this.VatAmountUsd)); // add  by jirawat jannet on 2016-11-16
                    }
                }
                else
                {
                    if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                    {
                        str = ReportUtil.CurrencyToEnglishWords(Convert.ToDouble(this.InvoiceAmount + this.VATAmount));
                    }
                    else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                    {
                        //str = ReportUtil.CurrencyToThaiWords(Convert.ToDouble(this.InvoiceAmount + this.VATAmount)); // comment by jirawat jannet on 2016-11-16
                        str = ReportUtil.CurrencyToEnglishWords(Convert.ToDouble(this.InvoiceAmount + this.VATAmount)); // add  by jirawat jannet on 2016-11-16
                    }
                }

                return RPT_InvoiceAmountCurrencyTypeFullName + " " + str;

            }
        }
        public string RPT_InvoiceAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.InvoiceAmountCurrencyType);
            }
        }
        public string RPT_InvoiceAmountCurrencyTypeFullName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyFullName(this.InvoiceAmountCurrencyType);
            }
        }
        public string VatAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.VatAmountCurrencyType);
            }
        }
        public string RPT_NetTatal
        {
            get
            {
                decimal netTotal = 0;
                string str = (this.InvoiceAmount + this.VATAmount).ToString("N2");
                if (this.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    netTotal += this.InvoiceAmount + this.VATAmount;
                else if (this.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    netTotal += this.InvoiceAmountUsd + this.VatAmountUsd;
                else netTotal = 0;

                return RPT_InvoiceAmountCurrencyTypeName + " " + str;
            }
        }
        //public string RPT_WHTRate
        //{
        //    get
        //    {
        //        string str = string.Empty;
        //        if (this.ShowInvWHTFlag)
        //        {
        //            str = this.WHTRate.ToString();
        //        }

        //        return str;
        //    }
        //}
        //public string RPT_WHTAmount
        //{
        //    get
        //    {
        //        string str = string.Empty;
        //        if (this.ShowInvWHTFlag)
        //        {
        //            str = this.WHTAmount.ToString("N2");
        //        }

        //        return str;
        //    }
        //}
        //public string RPT_Total_Reduce_WHT
        //{
        //    get
        //    {
        //        string str = string.Empty;
        //        if (this.ShowInvWHTFlag)
        //        {
        //            str = (this.InvoiceAmount + this.VatAmount - this.WHTAmount).ToString("N2");
        //        }

        //        return str;
        //    }
        //}

        public string RPT_PaymentMethod
        {
            get
            {
                string str = string.Empty;
                if (this.PaymentType == SECOM_AJIS.Common.Util.ConstantValue.PaymentType.C_PAYMENT_TYPE_CASH)
                {
                    str = "CASH";
                }
                else if (this.PaymentType == SECOM_AJIS.Common.Util.ConstantValue.PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER)
                {
                    str = "BANK_TRAN";
                }
                else if (this.PaymentType == SECOM_AJIS.Common.Util.ConstantValue.PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
                {
                    str = "AUTO_TRAN";
                }
                else if (this.PaymentType == SECOM_AJIS.Common.Util.ConstantValue.PaymentType.C_PAYMENT_TYPE_CHEQUE_NORMAL)
                {
                    str = "CHEQUE";
                }
                return str;
            }
        }

        // 20170302 nakajima modify start
        private string _RPT_SignatureImageFullPath = string.Empty;
        public string RPT_SignatureImageFullPath
        {
            //get; set;
            // Comment by Jirawat Jannet on 2016-12-13
            get
            {
                if (String.IsNullOrEmpty(ImageFileName) == false)
                    _RPT_SignatureImageFullPath = ImageFileName;

                return PathUtil.GetPathValue(PathUtil.PathName.ImageSignaturePath, _RPT_SignatureImageFullPath);  
            }
            set { }
        }
        // 20170302 nakajima modify end
        #endregion


        public object Clone()
        {
            return this.MemberwiseClone();
        }

        //Add by Jutarat A. on 11062013
        public string BillingTargetCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        //End Add

        //Add by Jutarat A. on 15102013
        public string RPT_BillingOffice
        {
            get { return CommonUtil.TextCodeName(this.BillingOfficeCode, this.BillingOffceNameLC); }
        }
        //End Add

        //Add by Jutarat A. on 13122013
        public string RPT_BranchName
        {
            get
            {
                string str = "-";
                string branchName = "-";
                string headOffice = "-";

                //Add by Jutarat A. on 02012014
                string strPrefix = string.Empty;
                string strHeadOfficeEN = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_MASTER, "MAS020", "lblHeadOffice", CommonValue.DEFAULT_LANGUAGE_EN);
                string strHeadOfficeLC = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_MASTER, "MAS020", "lblHeadOffice", CommonValue.DEFAULT_LANGUAGE_LC);
                //End Add

                if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                {
                    branchName = this.BranchNameEN;
                    headOffice = (strHeadOfficeEN ?? "").ToUpper();

                    if (this.BranchNo == "00000")
                    {
                        str = string.Format("({0})", headOffice);
                    }
                    else if (branchName != "-" && this.BranchNo != "-")
                    {
                        str = string.Format("{0} BRANCH (NO. {1})", branchName, this.BranchNo);
                    }
                    else if (branchName != "-")
                    {
                        str = string.Format("({0} BRANCH)", branchName);
                    }
                    else if (this.BranchNo != "-")
                    {
                        str = string.Format("(BRANCH NO. {0})", this.BranchNo);
                    }
                }
                else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                {
                    branchName = this.BranchNameLC;
                    headOffice = strHeadOfficeLC;

                    if (this.BranchNo == "00000")
                    {
                        str = string.Format("({0})", headOffice);
                    }
                    else if (branchName != "-" && this.BranchNo != "-")
                    {
                        str = string.Format("สาขา {0} (สาขาที่ {1})", branchName, this.BranchNo);
                    }
                    else if (branchName != "-")
                    {
                        str = string.Format("(สาขา {0})", branchName);
                    }
                    else if (this.BranchNo != "-")
                    {
                        str = string.Format("(สาขาที่ {0})", this.BranchNo);
                    }
                }


                return str;
            }
        }
        //End Add
    }
}