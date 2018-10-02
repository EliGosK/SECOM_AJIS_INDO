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
    /// Data object for ICS_010 screen
    /// </summary>
    public class ICS010_ScreenParameter : ScreenParameter
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                     Screen = "ICS010",
                     Parameter = "lblReceiptNo",
                     ControlName = "RefAdvanceReceiptNo")]
        public string ReceiptNo { set; get; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                     Screen = "ICS010",
                     Parameter = "lblPaymentType",
                     ControlName = "PaymentType")]
        public string PaymentType { set; get; }
        public List<tbt_Payment> PaymentList { set; get; }
        public tbt_Payment Payment { get; set; }
    }
}
