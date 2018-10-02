using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
//using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Presentation.Income.Models.MetaData;
namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Parameter of ICS032 screen
    /// </summary>
    public class ICS032_ScreenParameter : ScreenParameter
    {
        // send data back to client
        [KeepSession]
        public string strBillingOfficeCode { set; get; }
        [KeepSession]
        public string strBillingOfficeName { set; get; }
        [KeepSession]
        public string strInvoiceNo { set; get; }
        [KeepSession]
        public string strInvoiceOCC { set; get; }
        //[KeepSession]
        //public double? decTest { set; get; }

        [KeepSession]
        public List<doGetBillingTargetDebtSummaryByOffice> doBillingTargetDebtSummaryList { set; get; }
        [KeepSession]
        public List<doGetUnpaidInvoiceDebtSummaryByBillingTarget> doGetUnpaidInvoiceDebtSummaryByBillingTargetList { set; get; }
        [KeepSession]
        public List<doGetUnpaidDetailDebtSummary> doGetUnpaidDetailDebtSummaryByBillingCodeList { set; get; }
        public List<doGetBillingCodeDebtSummary> doGetBillingCodeDebtSummaryList { set; get; }

        public List<doGetDebtTracingMemo> doGetDebtTracingMemoList { set; get; }
        [KeepSession]
        public string strOpenFromListofUnpaidInvoiceByBillingTarget { set; get; }
        // send data from client to server
        public ICS032_RegisterData RegisterData { set; get; }

        public string strMode033 { set; get; }
        public List<doGetUnpaidDetailDebtSummary> doBillingTargetDebtSummary033 { set; get; }
        public string BillingTargetCode033 { set; get; }
        public string InvoiceNo033 { set; get; }
        public string InvoiceOCC033 { set; get; }
        public string BillingCode033 { set; get; }
        public string conYes { set; get; }
        public string conNo { set; get; }

    }
 
    // register com sent data to server
    /// <summary>
    /// DO of Register Header 
    /// </summary>
    public class ICS032_HeaderRegisterData
    {
        public string rdoProcessType { set; get; }

        public string cboTracingResault { set; get; }
        public DateTime? dtpLastContractDate { set; get; }
        public DateTime? dtpExpectedPaymentdate { set; get; }
        public string cboPaymentMethods { set; get; }
        public string txtaMemo { set; get; }
    }

    ///// <summary>
    ///// DO of Register Detail (Section1)
    ///// </summary>
    //public class ICS032_DetailRegisterDataSection1
    //{
 
    //}

    /// <summary>
    /// DO for Register Data
    /// </summary>
    public class ICS032_RegisterData
    {
        public ICS032_HeaderRegisterData Header { set; get; }
        //public List<ICS032_DetailRegisterDataSection1> Detail1 { set; get; }
    }
    

}
