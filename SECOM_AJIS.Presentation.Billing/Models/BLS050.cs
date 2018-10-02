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
namespace SECOM_AJIS.Presentation.Billing.Models
{
    /// <summary>
    /// Parameter of BLS050 screen
    /// </summary>
    public class BLS050_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string ContractCode { get; set; }

        [KeepSession]
        public string BillingOCC { get; set; }

        public doTbt_BillingBasic doBillingBasic { get; set; }

        public dtTbt_BillingTargetForView doBillingTarget { get; set; }
        public List<doGetBillingDetailForCancel> doBillingDetailForCancel { get; set; }
        public tbt_BillingBasic doTbtBillingBasic { get; set; }

        public BLS050_RegisterData RegisterData { set; get; }

        public List<tbt_AutoTransferBankAccount> dotbt_AutoTransferBankAccount { set; get; }
        public List<tbt_CreditCard> doTbt_CreditCard { set; get; }
        public string C_ISSUE_INV_NORMAL { set; get; }
        // turn BLS050

        public doBLS050GetBillingBasic _doBLS050GetBillingBasic { get; set; }
        public List<doBLS050GetBillingDetailForCancelList> _doBLS050GetBillingDetailForCancelList { get; set; }
        public doBLS050GetTbt_BillingTargetForView _doBLS050GetTbt_BillingTargetForView { get; set; }

        public string LastOCC { get; set; }
    }

    // register com sent data to server
    /// <summary>
    /// DO of Register Header 
    /// </summary>
    public class BLS050_HeaderRegisterData
    {
        public string strContractCode { set; get; }
        public string strBillingOCC { set; get; }
        public string rdoProcessTypeSpe { set; get; }
    }

    /// <summary>
    /// DO of Register Detail (Section1)
    /// </summary>
    public class BLS050_DetailRegisterDataSection1
    {
        public string strchkDel_id { set; get; }
        public bool bolDel { set; get; }
        public string strInvoiceno { set; get; }
        public string strInvoiceOCC { set; get; }
        public string strRunningno { set; get; }
        public string strBillingtype { set; get; }
        public string strPaymentstatus { set; get; }
        public string strBillingperiod { set; get; }
        public string strBillingamount { set; get; }

        public DateTime strDateStart { set; get; }
        public DateTime strDateEnd { set; get; }
        public string strBillingtypeCode { set; get; }

        public string strInvoicenoID { set; get; }
        public string strInvoiceOCCID { set; get; }
        public string strRunningnoID { set; get; }
        public string strBillingtypeID { set; get; }
        public string strPaymentstatusID { set; get; }
        public string strBillingperiodID { set; get; }
        public string strBillingamountID { set; get; }

        public Nullable<bool> FirstFeeFlag { get; set; } //Add by Jutarat A. on 29072013
    }

    /// <summary>
    /// DO of Register Detail (Section2)
    /// </summary>
    public class BLS050_DetailRegisterDataSection2
    {
        public string strBillingtype { set; get; }
        public DateTime? dtpFrom { set; get; }
        public DateTime? dtpTo { set; get; }
        public string intBillingamount { set; get; }
        public string initBillingamountCurrency { get; set; }
        public string strIssueinvoice { set; get; }
        public string strPaymentmethod { set; get; }
        public string strBillingdetailinvoiceformat { set; get; }
        public DateTime? dtpExpectedissueautotransferdate { set; get; }
        public bool? FirstFeeFlag { set; get; }
        public string ContractOCC { set; get; }

        public string strBillingtypeID { set; get; }
        public string dtpFromID { set; get; }
        public string dtpToID { set; get; }
        public string intBillingamountID { set; get; }
        public string initBillingamountCurrencyID { get; set; }
        public string strIssueinvoiceID { set; get; }
        public string strPaymentmethodID { set; get; }
        public string strBillingdetailinvoiceformatID { set; get; }
        public string dtpExpectedissueautotransferdateID { set; get; }
        public string chkFirstFeeFlagId { set; get; }
        public string cboContractOCCId { set; get; }
        // for report
        public string strInvoiceno { set; get; }
        public string strInvoiceOCC { set; get; }
    }

    /// <summary>
    /// DO of Register Detail (Section3)
    /// </summary>
    public class BLS050_DetailRegisterDataSection3
    {
        public string rdoProcessTypeAdj { set; get; }
        public string cboAdjustmentType { set; get; }
        public string intBillingAmountAdj { set; get; }
        public string intBillingAmountAdjCurrency { get; set; }
        public DateTime? dptAdjustBillingPeriodDateFrom { set; get; }
        public DateTime? dptAdjustBillingPeriodDateTo { set; get; }
    }

    /// <summary>
    /// DO for Register Data
    /// </summary>
    public class BLS050_RegisterData
    {
        public BLS050_HeaderRegisterData Header { set; get; }
        public List<BLS050_DetailRegisterDataSection1> Detail1 { set; get; }
        public List<BLS050_DetailRegisterDataSection2> Detail2 { set; get; }
        public BLS050_DetailRegisterDataSection3 Detail3 { set; get; }
        public string strFilePath { set; get; }
    }

    public class BLS050_WarningMessage
    {
        public string Code { get; set; }
        public List<string> Params { get; set; }
    }

    /// <summary>
    /// DO of Register Result
    /// </summary>
    public class BLS050_RegisterResult
    {
        public string ResultFlag { get; set; }
        public List<BLS050_WarningMessage> ConfirmMessageID { get; set; }
    }



    //public class BLS050_ScreenInputValidate
    //{
    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //                    Screen = "BLS050",
    //                    Parameter = "lblBillingClientCode",
    //                    ControlName = "")]
    //    public string BillingClientCode { get; set; }

    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //                   Screen = "BLS050",
    //                   Parameter = "lblNameEnglish",
    //                   ControlName = "")]
    //    public string FullNameEN { get; set; }

    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //             Screen = "BLS050",
    //             Parameter = "lblBillingOffice",
    //             ControlName = "BillingOfficeCode")]
    //    public string BillingOfficeCode { get; set; }

    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //            Screen = "BLS050",
    //            Parameter = "lblCustTypeCode",
    //            ControlName = "")]
    //    public string CustTypeCode { get; set; }
    //}

}
