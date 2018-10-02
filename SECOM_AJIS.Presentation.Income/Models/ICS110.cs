using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
//using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Presentation.Income.Models.MetaData;
namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Parameter of ICS110 screen
    /// </summary>
    public class ICS110_ScreenParameter : ScreenParameter
    {
        public doPaymentForWHTRegister RegisterParam { get; set; }
        public string LoadWHTNo { get; set; }
    }

    [MetadataType(typeof(doPaymentForWHTRegister_Meta))]
    public class doPaymentForWHTRegister
    {
        public string WHTNo { get; set; }
        public decimal? Amount { get; set; }
        public string AmountCurrencyType { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? WHTMatchingDate { get; set; }
        public List<string> PaymentTransNoList { get; set; }
        public decimal? TotalMatchedAmount { get; set; }
        public string TotalMatchedAmountCurrencyType { get; set; }
    }
}

namespace SECOM_AJIS.Presentation.Income.Models.MetaData
{
    public class doPaymentForWHTRegister_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME, Screen = "ICS110", Parameter = "lblWHTAmount", ControlName = "txtWHTAmount")]
        public decimal? Amount { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME, Screen = "ICS110", Parameter = "lblDocumentDate", ControlName = "txtDocumentDate")]
        public DateTime? DocumentDate { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INCOME, Screen = "ICS110", Parameter = "lblMatchingDate", ControlName = "txtWHTMatchingDate")]
        public DateTime? WHTMatchingDate { get; set; }
    }

}
