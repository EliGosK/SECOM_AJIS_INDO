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
    /// Parameter of BLS060 screen
    /// </summary>
    public class BLS060_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public List<tbt_BillingDetail> BillingDetail { set; get; }
        //[KeepSession]
        //public List<tbt_Invoice> Miscellaneous { set; get; }
        public BLS060_RegisterData RegisterData { set; get; }
        
    }

    // register com sent data to server
    /// <summary>
    /// DO of Register Header
    /// </summary>
    public class BLS060_HeaderRegisterData
    {
        public string rdoProcessType { set; get; }
        public string rdoProcessUnit { set; get; }
    }

    /// <summary>
    /// DO of Register Detail (Section1)
    /// </summary>
    public class BLS060_DetailRegisterDataSection1
    {
        public string txtContractCode { set; get; }
        public string txtBillingOCC { set; get; }
        public string txtRunningNo { set; get; }
        public DateTime? dtpIssueDate { set; get; }
        public bool chkRealTimeIssue { set; get; }

        public string  txtContractCodeID { set; get; }
        public string txtBillingOCCID { set; get; }
        public string txtRunningNoID { set; get; }
        public string dtpIssueDateID { set; get; }
    }

    /// <summary>
    /// DO of Register Detail (Section2)
    /// </summary>
    public class BLS060_DetailRegisterDataSection2
    {
        public string txtInvoiceNo { set; get; }
        public DateTime? dtpAutoTransferDate { set; get; }
        public bool chkRealTimeIssue { set; get; }

        public string txtInvoiceNoID { set; get; }
        public string dtpAutoTransferDateID { set; get; }
    }

    /// <summary>
    /// DO for Register Data
    /// </summary>
    public class BLS060_RegisterData
    {
        public BLS060_HeaderRegisterData Header { set; get; }
        public List<BLS060_DetailRegisterDataSection1> Detail1 { set; get; }
        public List<BLS060_DetailRegisterDataSection2> Detail2 { set; get; }
        public string strFilePath { set; get; }
    }



    //public class BLS060_ScreenInputValidate
    //{
    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //                    Screen = "BLS060",
    //                    Parameter = "lblBillingClientCode",
    //                    ControlName = "")]
    //    public string BillingClientCode { get; set; }

    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //                   Screen = "BLS060",
    //                   Parameter = "lblNameEnglish",
    //                   ControlName = "")]
    //    public string FullNameEN { get; set; }
    
    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //             Screen = "BLS060",
    //             Parameter = "lblBillingOffice",
    //             ControlName = "BillingOfficeCode")]
    //    public string BillingOfficeCode { get; set; }

    //    [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
    //            Screen = "BLS060",
    //            Parameter = "lblCustTypeCode",
    //            ControlName = "")]
    //    public string CustTypeCode { get; set; }
    //}

}
