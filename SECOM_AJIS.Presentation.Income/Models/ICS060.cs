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
    /// Parameter of ICS060 screen
    /// </summary>
    public class ICS060_ScreenParameter : ScreenParameter
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME,
                    Screen = "ICS060",
                    Parameter = "lblReceiptNo",
                    ControlName = "ReceiptNo")]
        public string ReceiptNo { set; get; }

        public string CancelMethod { set; get; }
    }

    /// <summary>
    /// DO of Receipt Information
    /// </summary>
    public class ICS060_ReceiptInformation
    {
        public doReceipt doReceipt { set; get; }
        public ComboBoxModel CancelMethodComboBoxModel { set; get; }
    }
}
