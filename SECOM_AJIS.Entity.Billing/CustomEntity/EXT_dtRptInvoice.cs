﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Billing.MetaData;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;

namespace SECOM_AJIS.DataEntity.Billing
{
     
    public partial class dtRptInvoice : ICloneable
    {
        //public string RPT_ReportName_TH
        //{
        //    get
        //    {
        //        return (this.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER || this.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD) ?
        //            InvoiceDocument.C_INVOICE_HEADER_TH_AUTO_TRANS :
        //            InvoiceDocument.C_INVOICE_HEADER_TH_NORMAL;
        //    }
        //}

        public string RPT_ReportName_EN
        {
            get
            {
                return (this.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER || this.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD) ?
                    InvoiceDocument.C_INVOICE_HEADER_EN_AUTO_TRANS :
                    InvoiceDocument.C_INVOICE_HEADER_EN_NORMAL;
            }
        }

        public string RPT_InvoiceDate
        {
            get
            {
                string strInvoiceDate = string.Empty;
                if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_CHRISTIAN)
                {
                    strInvoiceDate = this.IssueInvDate;
                }
                else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_BUDDHIST)
                {
                    strInvoiceDate = this.IssueInvDate_Buddhist;
                }

                return strInvoiceDate;
            }
        }

        public string RPT_PaymentCondition
        {
            get
            {

                string strPaymentCondition = "-";
                if (this.ShowDueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowDueDate.C_SHOW_DUEDATE_7)
                {
                    strPaymentCondition = this.DueDate7DescEN;
                }
                else if (this.ShowDueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowDueDate.C_SHOW_DUEDATE_30)
                {
                    strPaymentCondition = this.DueDate30DescEN;
                }
                else if (this.ShowDueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowDueDate.C_SHOW_DUEDATE_14)
                {
                    strPaymentCondition = this.DueDate14DescEN;
                }
                else if (this.ShowDueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowDueDate.C_SHOW_DUEDATE_NONE)
                {
                    strPaymentCondition = this.DueDateNoneDescEN;
                }
                //if (this.ShowDueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowDueDate.C_SHOW_DUEDATE_7)
                //{
                //    if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                //    {
                //        strPaymentCondition = this.DueDate7DescEN;
                //    }
                //    else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                //    {
                //        strPaymentCondition = this.DueDate7DescLC;
                //    }
                //}
                //else if (this.ShowDueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowDueDate.C_SHOW_DUEDATE_30)
                //{
                //    if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                //    {
                //        strPaymentCondition = this.DueDate30DescEN;
                //    }
                //    else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                //    {
                //        strPaymentCondition = this.DueDate30DescLC;
                //    }
                //}

                return strPaymentCondition;

            }
        }

        public string RPT_AccountNo
        {
            get
            {
                string str = "-";

                if (this.PaymentMethod == SECOM_AJIS.Common.Util.ConstantValue.PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                {
                    if (this.ShowAccType == ShowBankAccType.C_SHOW_BANK_ACC_SHOW)
                    {
                        if (!string.IsNullOrEmpty(this.AccountNo))
                        {
                            this.AccountNo = this.AccountNo.Replace("-", "");

                            // XXX-X-XXXXX-X  => (10 digits)
                            if (this.AccountNo.Length == 10)
                            {
                                //str = string.Format("{0}-{1}-{2}-{3}", this.AccountNo.Substring(0, 3), this.AccountNo.Substring(3, 1), this.AccountNo.Substring(4, 5), this.AccountNo.Substring(9, 1));

                                string strAccount = this.AccountNo;


                                string substr = strAccount.Substring((strAccount.Length - 1) - 3, 3);
                                strAccount = substr.PadLeft(strAccount.Length, 'X');

                                str = string.Format("{0}-{1}-{2}-{3}", strAccount.Substring(0, 3), strAccount.Substring(3, 1), strAccount.Substring(4, 5), strAccount.Substring(9, 1));
                            }
                            else
                            {
                                str = this.AccountNo;
                                if (!string.IsNullOrEmpty(str)) // add by Jirawat Jannet on 2016 -12-26
                                {
                                    string substr = "";
                                    if (str.Length >= 4)
                                        substr = str.Substring((str.Length - 1) - 3, 3);
                                    str = substr.PadLeft(str.Length, 'X');
                                }
                            }
                        }
                    }
                }


                return str;
            }
        }

        public string RPT_Bank_Branch
        {
            get
            {
                string str = "-";

                if (this.PaymentMethod == SECOM_AJIS.Common.Util.ConstantValue.PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER)
                {
                    if (this.ShowAccType == ShowBankAccType.C_SHOW_BANK_ACC_SHOW || this.ShowAccType == ShowBankAccType.C_SHOW_BANK_ACC_NOT_SHOW_ACC)
                    {
                        str = string.Format("{0}/{1}", this.BankNameEN, this.BankBranchNameEN);
                        //if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                        //{
                        //    str = string.Format("{0}/{1}", this.BankNameEN, this.BankBranchNameEN);
                        //}
                        //else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                        //{
                        //    str = string.Format("{0}/{1}", this.BankNameLC, this.BankBranchNameLC);
                        //}
                    }

                }

                return str;
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

                billingClientName = this.BillingClientNameEN;
                fullbranchName = (this.BranchNo != "-" && this.BranchNo != "00000" && this.BranchNameEN != "-");

                //if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                //{
                //    billingClientName = this.BillingClientNameEN;
                //    fullbranchName = (this.BranchNo != "-" && this.BranchNo != "00000" && this.BranchNameEN != "-");
                //}
                //else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                //{
                //    billingClientName = this.BillingClientNameLC;
                //    fullbranchName = (this.BranchNo != "-" && this.BranchNo != "00000" && this.BranchNameLC != "-");
                //}

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
                str = this.BillingClientAddressEN;
                //if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                //{
                //    str = this.BillingClientAddressEN;
                //}
                //else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                //{
                //    str = this.BillingClientAddressLC;
                //}

                return str;
            }
        }

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
                str = this.InvoiceDescriptionEN;
                //if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                //{
                //    str = this.InvoiceDescriptionEN;
                //}
                //else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                //{
                //    str = this.InvoiceDescriptionLC;
                //}

                return str;
            }
        }

        public string RPT_PreroidOfRental
        {
            get
            {
                string str = string.Empty;

                if (this.FirstFeeFlag == true &&
                    (
                        this.BillingTypeCode == SECOM_AJIS.Common.Util.ConstantValue.BillingType.C_BILLING_TYPE_SERVICE ||
                        this.BillingTypeCode == SECOM_AJIS.Common.Util.ConstantValue.BillingType.C_BILLING_TYPE_DIFF_SERVICE ||
                        this.BillingTypeCode == SECOM_AJIS.Common.Util.ConstantValue.BillingType.C_BILLING_TYPE_SG ||
                        this.BillingTypeCode == SECOM_AJIS.Common.Util.ConstantValue.BillingType.C_BILLING_TYPE_DIFF_SG
                    )
                    )
                {
                    str = InvoiceDocument.C_INVOICE_FIRST_FEE_EN;
                    //if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                    //{
                    //    str = InvoiceDocument.C_INVOICE_FIRST_FEE_EN;
                    //}
                    //else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                    //{
                    //    str = InvoiceDocument.C_INVOICE_FIRST_FEE_TH;
                    //}

                }

                return str;
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
                strReturn = string.Format("{0} {1}", InvoiceDocument.C_CONTRACT_NO_LABEL_EN, str);
                //if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                //{
                //    strReturn = string.Format("{0} {1}", InvoiceDocument.C_CONTRACT_NO_LABEL_TH, str);
                //}
                //else
                //{
                //    strReturn = string.Format("{0} {1}", InvoiceDocument.C_CONTRACT_NO_LABEL_EN, str);
                //}

                return strReturn;
            }
        }

        public string RPT_SiteName
        {
            get
            {
                string str = string.Empty;
                str = this.SiteNameEN;
                //if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                //{
                //    str = this.SiteNameEN;
                //}
                //else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                //{
                //    str = this.SiteNameLC; //this.SiteNameEN; //Modify by Jutarat A. on 15102013
                //}
                return str;
            }
        }

        public string RPT_Period
        {
            get
            {
                string str = string.Empty;


                if (this.BillingTypeGroup == SECOM_AJIS.Common.Util.ConstantValue.BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES
                    || this.BillingTypeGroup == SECOM_AJIS.Common.Util.ConstantValue.BillingTypeGroup.C_BILLING_TYPE_GROUP_DIFF_AMOUNT) //Add by Jutarat A. on 11062013
                {
                    if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_CHRISTIAN)
                    {
                        str = string.Format("{0} - {1}", this.BillingStartDate, this.BillingEndDate);
                    }
                    else if (this.ShowIssueDate == SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate.C_SHOW_ISSUE_DATE_BUDDHIST)
                    {
                        str = string.Format("{0} - {1}", this.BillingStartDate_Buddhist, this.BillingEndDate_Buddhist);
                    }
                    else
                    {
                        str = string.Format("{0} - {1}", this.BillingStartDate, this.BillingEndDate);
                    }
                }

                return str;
            }
        }

        // edit by Jirawat Jannet change to virtual variable
        // เพื่อที่จะเรียกข้อมูลชื่อของ currency จาก common แต่ไม่สามารถเรียกจากตรงนี้ได้ จึงต้องไปทำที่ ReportModel
        public string RPT_Amount
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return string.Format("{0} {1}"
                        , RPT_BillingAmountCurrencyName
                        , BillingAmount.ToString("N2"));
                else if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    return string.Format("{0} {1}"
                       , RPT_BillingAmountCurrencyName
                       , BillingAmountUsd.ToString("N2"));
                else return "";
            }
        }

        public string RPT_SumAmount { get; set; }

        // Add by Jirawat Jannet
        public string RPT_BillingAmountCurrencyName
        {
            get
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(BillingAmountCurrencyType);
            }
        }
        // Add by Jirawta jannet
        public string RPT_VatAmountCurrencyName
        {
            get
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(VatAmountCurrencyType);
            }
        }
        // add by jirawat jannet
        public decimal RPT_BillingAmountVal
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return BillingAmount;
                else if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return BillingAmountUsd;
                else return 0;
            }
        }

        // add by jirawat jannet
        public decimal RPT_GrandTotalValue
        {
            get
            {
                decimal val = 0;
                if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    val = InvoiceAmount + VatAmount;
                else 
                    val = InvoiceAmountUsd + VatAmountUsd;

                return val;
            }
        }
        // edit by jirawat jannet
        public string RPT_GrandTotal
        {
            get
            {
                return RPT_VatAmountCurrencyName + " " + RPT_GrandTotalValue.ToString("N2");
            }
        }
        // add by jirawat jannet
        public string RPT_GrandTotalWord
        {
            get
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return string.Format("{0} {1}", ReportUtil.CurrencyToEnglishWords((double)RPT_GrandTotalValue), hand.getCurrencyFullName(VatAmountCurrencyType));
            }
        }

        public string RPT_VATRate
        {
            get
            {
                string str = string.Empty;
                decimal vat = this.VatRate * 100M;
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
                string str = string.Empty;

                if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    str = RPT_VatAmountCurrencyName + " " + this.VatAmount.ToString("N2");
                else
                    str = RPT_VatAmountCurrencyName + " " + this.VatAmountUsd.ToString("N2");
                return str;
            }
        }

        public decimal RPT_VatAmountVal
        {
            get
            {
                if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return VatAmount;
                else if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    return VatAmountUsd;
                else
                    return 0;
            }
        }


        public string RPT_NetTatalWord
        {
            get
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return string.Format("{0} ～ {1}"
                                            , hand.getCurrencyFullName(InvoiceAmountCurrencyType)
                                            , ReportUtil.CurrencyToEnglishWords((double)RPT_NetTotalVal));
            }
        }
        // add by jirawat jannet
        public decimal RPT_NetTotalVal
        {
            get
            {
                return RPT_GrandTotalValue - RPT_WHTAmountVal;
            }
        }
        public string RPT_NetTatal
        {
            get
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(InvoiceAmountCurrencyType) + " " + RPT_NetTotalVal.ToString("N2");
            }
        }

        public string RPT_WHTRate
        {
            get
            {
                string str = string.Empty;
                if (this.ShowInvWHTFlag)
                {
                    str = (this.WHTRate * 100M).ToString("N0") + " %";
                }

                return str;
            }
        }
        // add by jirawat jannet
        public decimal RPT_WHTAmountVal
        {
            get
            {
                if (WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return this.WHTAmount;
                else 
                    return this.WHTAmountUsd;
            }
        }
        public string RPT_WHTAmount
        {
            get
            {
                string str = string.Empty;
                if (this.ShowInvWHTFlag)
                {
                    str = this.RPT_WHTAmountVal.ToString("N2");
                }

                return str;
            }
        }


        public string RPT_Total_Reduce_WHT
        {
            get
            {
                string str = string.Empty;
                if (this.ShowInvWHTFlag)
                {
                    str = (this.InvoiceAmount + this.VatAmount - this.WHTAmount).ToString("N2");
                }

                return str;
            }
        }

        public string RPT_PaymentMethod
        {
            get
            {
                string str = string.Empty;
                if (this.PaymentMethod == SECOM_AJIS.Common.Util.ConstantValue.PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER)
                {
                    str = "T";
                }
                else if (this.PaymentMethod == SECOM_AJIS.Common.Util.ConstantValue.PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER ||
                        this.PaymentMethod == SECOM_AJIS.Common.Util.ConstantValue.PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER
                    )
                {
                    str = "A";
                }
                else if (this.PayByChequeFlag)
                {
                    str = "M";
                }
                else if (this.PaymentMethod == SECOM_AJIS.Common.Util.ConstantValue.PaymentMethod.C_PAYMENT_METHOD_MESSENGER_TRANSFER)
                {
                    str = "B";
                }

                //return str;
                return string.Format("{0}{1}", str, this.CreditTerm); //Modify by Jutarat A. on 16102013
            }
        }

        public string RPT_InvoicePaper { set; get; }

        public int TotalPage { set; get; }

        public string RPT_RealBillingClientName
        {
            get
            {
                string str = "-";
                str = this.RealBillingClientNameLC;
                //if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                //{
                //    str = this.RealBillingClientNameEN;
                //}
                //else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                //{
                //    str = this.RealBillingClientNameLC;
                //}

                return str;
            }
        }

        public string RPT_RealBillingClientAddress
        {
            get
            {
                string str = "-";
                str = this.RealBillingClientAddressEN;
                //if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                //{
                //    str = this.RealBillingClientAddressEN;
                //}
                //else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                //{
                //    str = this.RealBillingClientAddressLC;
                //}

                return str;
            }
        }

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
                //if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_ENG)
                //{
                //    branchName = this.BranchNameEN;
                //    headOffice = (strHeadOfficeEN ?? "").ToUpper();

                //    if (this.BranchNo == "00000")
                //    {
                //        str = string.Format("({0})", headOffice);
                //    }
                //    else if (branchName != "-" && this.BranchNo != "-")
                //    {
                //        str = string.Format("{0} BRANCH (NO. {1})", branchName, this.BranchNo);
                //    }
                //    else if (branchName != "-")
                //    {
                //        str = string.Format("({0} BRANCH)", branchName);
                //    }
                //    else if (this.BranchNo != "-")
                //    {
                //        str = string.Format("(BRANCH NO. {0})", this.BranchNo);
                //    }
                //}
                //else if (this.DocLanguage == SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage.C_DOC_LANG_LOCAL)
                //{
                //    branchName = this.BranchNameLC;
                //    headOffice = strHeadOfficeLC;

                //    if (this.BranchNo == "00000")
                //    {
                //        str = string.Format("({0})", headOffice);
                //    }
                //    else if (branchName != "-" && this.BranchNo != "-")
                //    {
                //        str = string.Format("สาขา {0} (สาขาที่ {1})", branchName, this.BranchNo);
                //    }
                //    else if (branchName != "-")
                //    {
                //        str = string.Format("(สาขา {0})", branchName);
                //    }
                //    else if (this.BranchNo != "-")
                //    {
                //        str = string.Format("(สาขาที่ {0})", this.BranchNo);
                //    }
                //}

                return str;
            }
        }
        //End Add

        public string RPT_CurrentAccountNumber
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return "Rp. Account Number";
                else if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    return "US$ Account Number";
                else
                    return string.Empty;
            }
        }

        public string RPT_AccountNo1
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return AccountNoIndo1Rp;
                else if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    return AccountNoIndo1Usd;
                else
                    return string.Empty;
            }
        }
        public string RPT_AccountNo2
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return AccountNoIndo2Rp;
                else if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    return AccountNoIndo2Usd;
                else
                    return string.Empty;
            }
        }
        public string RPT_AccountNo3
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return AccountNoIndo3Rp;
                else if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    return AccountNoIndo3Usd;
                else
                    return string.Empty;
            }
        }
        public string RPT_AccountNo4
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return AccountNoIndo4Rp;
                else if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    return AccountNoIndo4Usd;
                else
                    return string.Empty;
            }
        }
        public string RPT_AccountBayanNo1
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return AccountNoBayan1Rp;
                else
                    return string.Empty;
            }
        }
        public string RPT_AccountBayanNo2
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return AccountNoBayan2Rp;
                else
                    return string.Empty;
            }
        }
        public string RPT_AccountBayanNo3
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return AccountNoBayan3Rp;
                else
                    return string.Empty;
            }
        }
        public string RPT_AccountBayanNo4
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return AccountNoBayan4Rp;
                else
                    return string.Empty;
            }
        }
        public string RPT_AccountBayanNo5
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return AccountNoBayan5Rp;
                else
                    return string.Empty;
            }
        }
        public string RPT_Name
        {
            get { return SignatureName; }
        }
        public string RPT_Executive
        {
            get { return SignatureExecutive; }
        }
        public string RPT_BillingCustomerCode
        {
            get { return BillingClientCode; }
        }
        public string BillingOffice
        {   
            get { return BillingOfficeCode + ": " + BillingOffceNameLC; }
        }


        public bool IsAutoTransfer
        {
            get
            {
                return (this.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER || this.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD);
            }
        }

        // add by Jirawat Jannet on 2016-02-06
        // 2017.03.08 modify matsuda start
        //public string RPT_SignatureImageFullPath { get; set; }
        public string RPT_SignatureImageFullPath
        {
            get
            {
                return PathUtil.GetPathValue(PathUtil.PathName.ImageSignaturePath, SignatureImageFullPath);
            }
            set { }
        }
        // 2017.03.08 modify matsuda end

        public string RPT_CompanyName { get; set; }

        public int RPT_VisibleRecord { get; set; }
    }
}

