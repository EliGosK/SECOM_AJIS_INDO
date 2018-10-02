using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object for credit note report.
    /// </summary>
    public partial class doRptCreditNote : ICloneable
    {
        #region Header
        public string RPT_CreditNoteDate
        {
            get
            {
                string resultdate = string.Empty;
                if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_CHRISTIAN)
                {
                    resultdate = this.CreditNoteDate;
                }
                else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_BUDDHIST)
                {
                    resultdate = this.CreditNoteDate_Buddhist;
                }
                //Do nothing for ShowIssueDate.C_SHOW_ISSUE_DATE_NOT status
                //else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_NOT)
                //{
                //    resultdate = string.Empty;
                //}

                if (resultdate.Equals("-"))
                    resultdate = string.Empty;

                return resultdate;
            }
        }
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
        #endregion

        #region Detail
        public string RPT_TaxInvoiceDate
        {
            get
            {
                string resultdate = string.Empty;
                if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_CHRISTIAN)
                {
                    resultdate = this.TaxInvoiceDate;
                }
                else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_BUDDHIST)
                {
                    resultdate = this.TaxInvoiceDate_Buddhist;
                }
                else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_NOT)
                {
                    //Confirm sa on 19/Jun/2012, Display same as christian format
                    resultdate = this.TaxInvoiceDate;
                }
                return resultdate;
            }
        }
        public string RPT_TaxInvoiceAmount
        {
            get
            {
                string str = (this.TaxInvoiceAmount - this.TaxInvoiceVATAmount).ToString("N2");
                return str;
            }
        }
        public string RPT_TaxInvoiceVATRate
        {
            get
            {
                //No field in tax invoice table, used same as RPT_VATRate
                return this.RPT_VATRate;
            }
        }
        public string RPT_TaxInvoiceVATAmount
        {
            get
            {
                string str = this.TaxInvoiceVATAmount.ToString("N2");
                return str;
            }
        }
        public string RPT_TaxInvoiceTotalAmount
        {
            get
            {
                string str = this.TaxInvoiceAmount.ToString("N2");
                return str;
            }
        }
        public string RPT_CreditNoteAmount
        {
            get
            {
                string str = (this.CreditAmountIncVAT - this.CreditVATAmount).ToString("N2");
                return str;
            }
        }
        #endregion

        #region Footer (Every page)
        public string RPT_CreditNotePaper { set; get; }

        public string RPT_CreditNoteSubTotalAmount
        {
            get
            {
                string str = (this.CreditAmountIncVAT - this.CreditVATAmount).ToString("N2");
                return str;
            }
        }
        public string RPT_VATRate
        {
            get
            {
                string str = string.Empty;
                decimal vat = this.VATRate * 100M;
                if (vat >= 0M)
                {
                    str = vat.ToString("N0");
                }

                return str;
            }
        }
        public string RPT_VatAmount
        {
            get
            {
                string str = this.CreditVATAmount.ToString("N2");
                return str;
            }
        }
        public string RPT_CreditNoteNetTotalAmount
        {
            get
            {
                string str = this.CreditAmountIncVAT.ToString("N2");
                return str;
            }
        }
        public string RPT_NetTotalWord
        {
            get
            {
                string str = string.Empty;

                if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                {
                    str = ReportUtil.CurrencyToEnglishWords(this.CreditAmountIncVAT.ToString());
                }
                else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                {
                    str = ReportUtil.CurrencyToThaiWords(this.CreditAmountIncVAT.ToString());
                }
                return str;
            }
        }

        public string RPT_SignatureImageFullPath
        {
            get
            {
                string fullPath = string.Empty;

                if (this.SignatureType == SigType.C_SIG_TYPE_HAVE && string.IsNullOrEmpty(this.ImageFileName) == false)
                {
                    fullPath = PathUtil.GetPathValue(PathUtil.PathName.ImageSignaturePath, this.ImageFileName);
                }

                return fullPath;
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

        #endregion


        public object Clone()
        {
            return this.MemberwiseClone();
        }

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

                //Add by Jutarat A. on 02012014
                string strPrefix = string.Empty;
                string strHeadOfficeEN = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_MASTER, "MAS020", "lblHeadOffice", CommonValue.DEFAULT_LANGUAGE_EN);
                string strHeadOfficeLC = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_MASTER, "MAS020", "lblHeadOffice", CommonValue.DEFAULT_LANGUAGE_LC);
                //End Add

                if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                {
                    //Modify by Jutarat A. on 02012014
                    //str = this.BranchNameEN;
                    if (String.IsNullOrEmpty(this.BranchNameEN) == false && this.BranchNameEN != "-" && this.BranchNameEN.ToUpper() != strHeadOfficeEN.ToUpper())
                        strPrefix = "Branch ";

                    str = strPrefix + this.BranchNameEN;
                    //End Modify
                }
                else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                {
                    //Modify by Jutarat A. on 02012014
                    //str = this.BranchNameLC;
                    if (String.IsNullOrEmpty(this.BranchNameLC) == false && this.BranchNameLC != "-" && this.BranchNameLC != strHeadOfficeLC)
                        strPrefix = "สาขา ";

                    str = strPrefix + this.BranchNameLC;
                    //End Modify
                }

                return str;
            }
        }
        //End Add
    }
}