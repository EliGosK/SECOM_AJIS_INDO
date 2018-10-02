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
    /// Screen parameter of BLS020
    /// </summary>
    public class BLS020_ScreenParameter : ScreenParameter
    {

        [KeepSession]
        public string BillingTargetCode { set; get; }

      

        public dtTbt_BillingTargetForView doBillingTarget { get; set; }
        public tbt_BillingTarget doTbt_BillingTarget { get; set; }
    }

    /// <summary>
    /// Screen input validate of BLS020
    /// </summary>
    public class BLS020_ScreenInputValidate : ScreenParameter
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                        Screen = "BLS020",
                        Parameter = "lblBillingClientCode",
                        ControlName = "BillingClientCode")]
        public string BillingClientCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                       Screen = "BLS020",
                       Parameter = "lblBillingTargetNo",
                       ControlName = "BillingTargetNo")]
        public string BillingTargetNo { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                 Screen = "BLS020",
                 Parameter = "lblBillingOffice",
                 ControlName = "BillingOfficeCode")]
        public string BillingOfficeCode { get; set; }


    }
}