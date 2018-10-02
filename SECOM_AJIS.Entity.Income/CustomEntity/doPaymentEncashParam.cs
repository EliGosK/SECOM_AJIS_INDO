using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Income.MetaData;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data ojbect of payment encash
    /// </summary>
    public class doPaymentEncashParam
    {
        public string PaymentTransNo { get; set; }
        public int? EncashedFlag { get; set; }
        [CodeNotNullOtherNotNull("IsReturnCheque",
            Controller = MessageUtil.MODULE_INCOME,
            Screen = "ICS080",
            Parameter = "lblChequeReturnDate",
            ControlName = "txtChequeReturnDate")]
        public DateTime? ChequeReturnDate { get; set; }
        [CodeNotNullOtherNotNull("IsReturnCheque",
            Controller = MessageUtil.MODULE_INCOME,
            Screen = "ICS080",
            Parameter = "lblChequeReturnReason",
            ControlName = "cboChequeReturnReason")]
        public string ChequeReturnReason { get; set; }
        public string ChequeReturnRemark { get; set; }
        public string ChequeEncashRemark { get; set; }
        public byte? EncashedFlagByte
        {
            get
            {
                return (this.EncashedFlag.HasValue ? new byte?((byte)this.EncashedFlag.Value) : null);
            }
        }

        public bool? IsReturnCheque {
            get {
                return (this.EncashedFlag == 2 ? new bool?(true) : null);
            }
        }
    }
}
