using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.IO;
using SECOM_AJIS.DataEntity.Income;

namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Data object for ICR_010 report
    /// </summary>
    public class RPTICR010 : DataEntity.Income.doRptReceipt { }

    /// <summary>
    /// Data object for ICR_020 report
    /// </summary>
    public class RPTICR020 : DataEntity.Income.doRptCreditNote { }

    /// <summary>
    /// Data object for ICR_030 and ICR_040 report
    /// </summary>
    public class RPTICR030 : DataEntity.Income.doICR030 { }
}
