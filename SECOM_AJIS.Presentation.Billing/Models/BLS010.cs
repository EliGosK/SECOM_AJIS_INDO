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
    /// Screen parameter of BLS010
    /// </summary>
    public class BLS010_ScreenParameter : ScreenParameter
    {
        //[KeepSession]
        public string BillingClientCode { set; get; }

        //[KeepSession]
        public List<dtBillingClientData> doBillingClientList { get; set; }
        
        public tbt_BillingTarget doTbt_BillingTarget { get; set; }
        public dtBillingClientData doBillingClient { get; set; }

        public SECOM_AJIS.DataEntity.Master.tbm_BillingClient doTbmBillingClientParam { get; set; }        
    }

    /// <summary>
    /// Screen input validate of BLS010
    /// </summary>
    public class BLS010_ScreenInputValidate
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                        Screen = "BLS010",
                        Parameter = "lblBillingClientCode",
                        ControlName = "")]
        public string BillingClientCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                       Screen = "BLS010",
                       Parameter = "lblNameEnglish",
                       ControlName = "")]
        public string FullNameEN { get; set; }
    
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                 Screen = "BLS010",
                 Parameter = "lblBillingOffice",
                 ControlName = "BillingOfficeCode")]
        public string BillingOfficeCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                Screen = "BLS010",
                Parameter = "lblCustTypeCode",
                ControlName = "")]
        public string CustTypeCode { get; set; }
    }

}
