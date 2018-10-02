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
    /// Screen parameter of BLS031
    /// </summary>
      
    public class BLS031_ScreenParameter : ScreenParameter
    {
        public AutoTransfer doAutoTransfer { get; set; }
    }

    /// <summary>
    /// Screen input validate of BLS031
    /// </summary>
    public class BLS031_ScreenInputValidate 
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                        Screen = "BLS031",
                        Parameter = "lblAutoTransferAccountName",
                        ControlName = "BLS031_AccountName")]
        public string AccountName { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                       Screen = "BLS031",
                       Parameter = "lblBank",
                       ControlName = "BLS031_BankCode")]
        public string BankCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                       Screen = "BLS031",
                       Parameter = "lblBranch",
                       ControlName = "BLS031_BankBranchCode")]
        public string BankBranchCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                 Screen = "BLS031",
                 Parameter = "lblAccountType",
                 ControlName = "BLS031_AccountType")]
        public string AccountType { get; set; }


        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                Screen = "BLS031",
                Parameter = "lblAccountNo",
                ControlName = "BLS031_Account1")]
        public string Account1 { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                Screen = "BLS031",
                Parameter = "lblAccountNo",
                ControlName = "BLS031_Account2")]
        public string Account2 { get; set; }


        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
                Screen = "BLS031",
                Parameter = "lblAccountNo",
                ControlName = "BLS031_Account3")]
        public string Account3 { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
             Screen = "BLS031",
             Parameter = "lblAccountNo",
             ControlName = "BLS031_Account4")]
        public string Account4 { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_BILLING,
           Screen = "BLS031",
           Parameter = "lblAutoTransferDate",
           ControlName = "BLS031_AutoTransferDate")]
        public string AutoTransferDate { get; set; }

    }

}