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
using SECOM_AJIS.DataEntity.Contract;
namespace SECOM_AJIS.Presentation.Billing.Models
{
    /// <summary>
    /// Parameter of BLS070 screen
    /// </summary>
    public class BLS070_ScreenParameter : ScreenSearchParameter
    {
        // send to browser
        // send sum of invoice back to screen
        [KeepSession]
        public string VatCurrencyCode { get; set; }
        [KeepSession]
        public string VatCurrency { get; set; }
        [KeepSession]
        public string WHTCurrencyCode { get; set; }
        [KeepSession]
        public string WHTCurrency { get; set; }
        [KeepSession]
        public decimal? decInvoiceTotal { set; get; }
        [KeepSession]
        public int intInvoiceCount { set; get; }

        // section 3
        public string txtVATAmount { set; get; }
        public string txtWHTAmount { set; get; }

        public doInvoice doGetInvoiceWithBillingClientName { set; get; }
        public List<doGetBillingDetailOfInvoice> doGetBillingDetailOfInvoiceList { set; get; }
        public List<tbt_BillingBasic> dotbt_BillingBasicList { set; get; }
        public List<doGetSaleDataForIssueInvoice> doGetSaleDataForIssueInvoice { set; get; } //Merge at 14032017 By Pachara S.
        public List<tbt_BillingDetail> doGetBillingDetailPartialFee { set; get; }

        // For case combine invoice
        public List<doGetBillingDetailOfInvoice> ConbineBillingDetail { set; get; }

        public bool bIssuePartialFee { set; get; }
        

        // send to server
        public BLS070_RegisterData RegisterData { set; get; }

        // pass param from other screen (CMS210)

        public string strCallerScreenID  { set; get; }
        public string ProcessType  { set; get; }
        public string ContractCode  { set; get; }
        public string ContractCode_short { set; get; }
        public string OCC { set; get; }
        public string defSelectProcessType  { set; get; }
        public string defstrContractCode   { set; get; }
        public string defstrBillingOCC { set; get; }
    }

    // register com sent data to server
    /// <summary>
    /// DO of Register Header
    /// </summary>
    public class BLS070_HeaderRegisterData
    {

        public string rdoProcessSelect { set; get; }

        public string txtSelSeparateFromInvoiceNo { set; get; }
        public string txtSelCombineToInvoiceNo { set; get; }
        public string txtSelContractCode { set; get; }
        public string txtSelSaleOCC { set; get; }

    }

    /// <summary>
    /// DO of Register Detail (1)
    /// </summary>
    public class BLS070_DetailRegisterData1
    {
        public string txtSepBillingTargetCode { set; get; }
        public string txtSepBliiingClientNameEN { set; get; }
        public string txtSepBliiingClientNameLC { set; get; }

        public string txtSepInvoiceNo { set; get; }
        public bool chkSepNotChangeInvoiceNo { set; get; }
        public string cboPaymentMethodsOfSeparateFrom { set; get; }
        public string cboIssueInvoiceAfterSeparate { set; get; }

    }

    /// <summary>
    /// DO of Register Detail (2)
    /// </summary>
    public class BLS070_DetailRegisterData2
    {
        public string txtComBillingTargetCode { set; get; }
        public string txtComBliiingClientNameEN { set; get; }
        public string txtComBliiingClientNameLC  { set; get; }

        public string txtComInvoiceNo { set; get; }
        public bool chkComNotChangeInvoiceNo { set; get; }
        public string cboPaymentMethodsOfCombineToInvoice { set; get; }
        public string cboIssueInvoiceAfterCombine { set; get; }
    }

    /// <summary>
    /// DO of Register Detail (3)
    /// </summary>
    public class BLS070_DetailRegisterData3
    {
        public string txtIssContractCode { set; get; }
        public string txtIssSaleOCC { set; get; }

        public string txtBillingAmount { set; get; }
        public string txtVATAmount { set; get; }
        public string txtWHTAmount { set; get; }

        public DateTime? dtpCustomerAcceptanceDate { set; get; }
    }

    public class BLS070_DetailRegisterData4
    {
        public string txtIssContractCode { set; get; }
        public string txtIssSaleOCC { set; get; }

        public string txtBillingAmount { set; get; }
        public string txtVATAmount { set; get; }
        public string txtWHTAmount { set; get; }

        public DateTime? dtpCustomerAcceptanceDate { set; get; }
    }

    /// <summary>
    /// DO of Register Detail (Section1)
    /// </summary>
    public class BLS070_DetailRegisterDataSection1
    {
        public bool chkSelectSeparateDetail { set; get; }

        public string BillingCode { set; get; }
        public string RunningNo { set; get; }
        public string BillingType { set; get; }
        public string BillingAmount { set; get; }
        public string SiteName  { set; get; }

        public string cboIssueInvoiceofSeparateDetail { set; get; }

        public string ContractCode { set; get; }
        public string BillingOCC { set; get; }
        public string BillingDetailNo { set; get; }

    }

    /// <summary>
    /// DO of Register Detail (Section2)
    /// </summary>
    public class BLS070_DetailRegisterDataSection2
    {
        public string BillingCode { set; get; }
        public string RunningNo { set; get; }
        public string BillingType { set; get; }
        public string BillingAmount { set; get; }
        public string SiteName { set; get; }

        public string ContractCode { set; get; }
        public string BillingOCC { set; get; }
        public string BillingDetailNo { set; get; }
    }

    /// <summary>
    /// DO for Register Data
    /// </summary>
    public class BLS070_RegisterData
    {
        public BLS070_HeaderRegisterData Header { set; get; }

        public BLS070_DetailRegisterData1 Details1 { set; get; }
        public BLS070_DetailRegisterData2 Details2 { set; get; }
        public BLS070_DetailRegisterData3 Details3 { set; get; }
        public BLS070_DetailRegisterData4 Details4 { set; get; }

        public List<BLS070_DetailRegisterDataSection1> Detail1 { set; get; }
        public List<BLS070_DetailRegisterDataSection2> Detail2 { set; get; }

        public string strFilePath { set; get; }
        public List<doBillingDetail> NewBillingDetail { set; get; }
        public bool bAlertConfirmDialog { set; get; } // for suppport case Issue sale (case 3)
    }



    //public class BLS070_ScreenInputValidate
    //{
    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //                    Screen = "BLS070",
    //                    Parameter = "lblBillingClientCode",
    //                    ControlName = "")]
    //    public string BillingClientCode { get; set; }

    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //                   Screen = "BLS070",
    //                   Parameter = "lblNameEnglish",
    //                   ControlName = "")]
    //    public string FullNameEN { get; set; }
    
    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //             Screen = "BLS070",
    //             Parameter = "lblBillingOffice",
    //             ControlName = "BillingOfficeCode")]
    //    public string BillingOfficeCode { get; set; }

    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //            Screen = "BLS070",
    //            Parameter = "lblCustTypeCode",
    //            ControlName = "")]
    //    public string CustTypeCode { get; set; }
    //}

}
