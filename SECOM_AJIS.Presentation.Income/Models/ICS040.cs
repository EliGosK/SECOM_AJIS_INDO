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
using SECOM_AJIS.DataEntity.Income;

namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Data object for ICS_040 screen
    /// </summary>
    public class ICS040_ScreenParameter : ScreenParameter
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                     Screen = "ICS040",
                     Parameter = "lblInvoiceNo",
                     ControlName = "InvoiceNo")]
        public string InvoiceNo { set; get; }
        public string ApproveNo { set; get; }

        public List<doInvoice> InvoiceList { set; get; }
    }
}
