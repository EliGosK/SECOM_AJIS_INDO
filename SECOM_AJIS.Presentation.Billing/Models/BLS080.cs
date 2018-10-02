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
    /// Screen parameter for BLS080
    /// </summary>
    public class BLS080_ScreenParameter : ScreenParameter
    {

    }

    /// <summary>
    /// Search condition class of screen BLS080
    /// </summary>
    public class BLS080_SearchCondition
    {
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
        //            Screen = "BLS080",
        //            Parameter = "lblBankBranchName",
        //            ControlName = "SecomAccountID")]
        public int? SecomAccountID { set; get; }
        public DateTime? AutoTranferDateFrom { set; get; }
        public DateTime? AutoTranferDateTo { set; get; }
        public DateTime? GeneateDateFrom { set; get; }
        public DateTime? GeneateDateTo { set; get; }
    }

}