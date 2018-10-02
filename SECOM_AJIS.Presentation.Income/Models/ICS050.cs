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

namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Data object for ICS_020 screen
    /// </summary>
    public class ICS050_ScreenParameter : ScreenParameter
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                     Screen = "ICS050",
                     Parameter = "lblInvoiceNo",
                     ControlName = "InvoiceNo")]
        public string InvoiceNo { set; get; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                     Screen = "ICS050",
                     Parameter = "lblIssueInvoiceDate",
                     ControlName = "IssueInvoiceDate")]
        public DateTime? IssueInvoiceDate { set; get; }

        public string PDFFilePath { get; set; }
    }
}
