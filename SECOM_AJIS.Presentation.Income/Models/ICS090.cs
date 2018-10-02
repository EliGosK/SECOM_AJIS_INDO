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
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Data object for ICS_090 screen
    /// </summary>
    public class ICS090_ScreenParameter : ScreenParameter
    {
        public string InvoiceNo { set; get; }
        public string CorrectionReason { set; get; }
        public string ApproveNo { set; get; }
        public string PDFFilePath { set; get; }
    }

    /// <summary>
    /// Data object for invoice information of ICS_090 screen
    /// </summary>
    public class ICS090_InvoiceInfo
    {
        public doInvoice doInvoice { get; set; }
        public string PaymentStatus { get; set; }
        public string NextPaymentStatus { get; set; }
    }
}
