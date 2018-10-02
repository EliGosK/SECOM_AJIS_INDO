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
    /// Screen parameter of BLS032
    /// </summary>
      
    public class BLS032_ScreenParameter : ScreenParameter
    {
        public doCreditCard doCredit { get; set; }
    }

    /// <summary>
    /// Screen input validate of BLS032
    /// </summary>
    public class BLS032_ScreenInputValidate 
    {

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                       Screen = "BLS032",
                       Parameter = "lblCreditCardType",
                       ControlName = "BLS032_CreditCardType")]
        public string CreditCardType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                      Screen = "BLS032",
                      Parameter = "lblCardHolderName",
                      ControlName = "BLS032_CardName")]
        public string CardName { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                      Screen = "BLS032",
                      Parameter = "lblCreditCardCompany",
                      ControlName = "BLS032_CreditCardCompanyCode")]
        public string CreditCardCompanyCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                      Screen = "BLS032",
                      Parameter = "lblCreditCardNo",
                      ControlName = "BLS032_CreditCardNo1")]
        public string CreditCardNo1 { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                    Screen = "BLS032",
                    Parameter = "lblCreditCardNo",
                    ControlName = "BLS032_CreditCardNo2")]
        public string CreditCardNo2 { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                    Screen = "BLS032",
                    Parameter = "lblCreditCardNo",
                    ControlName = "BLS032_CreditCardNo3")]
        public string CreditCardNo3 { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                    Screen = "BLS032",
                    Parameter = "lblCreditCardNo",
                    ControlName = "BLS032_CreditCardNo4")]
        public string CreditCardNo4 { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                    Screen = "BLS032",
                    Parameter = "lblExpireDateMonth",
                    ControlName = "BLS032_ExpMonth")]
        public string ExpMonth { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                    Screen = "BLS032",
                    Parameter = "lblExpireDateYear",
                    ControlName = "BLS032_ExpYear")]
        public string ExpYear { get; set; }
    }
}