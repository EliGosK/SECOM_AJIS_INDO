using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Presentation.Quotation.Models
{
    /// <summary>
    /// DO for contract header data
    /// </summary>
    public class View_doContractHeader : doContractHeader
    {
        public bool ShowDetail { get; set; }
    }
}
