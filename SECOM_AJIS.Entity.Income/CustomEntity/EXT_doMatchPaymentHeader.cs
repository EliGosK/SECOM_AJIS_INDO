using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of a payment matching header information.
    /// </summary>
    public partial class doMatchPaymentHeader
    {
        public List<doMatchPaymentDetail> MatchPaymentDetail { get; set; }
    }
}
