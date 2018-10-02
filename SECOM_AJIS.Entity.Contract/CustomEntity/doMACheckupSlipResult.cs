using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doMACheckupSlipResult
    {
        public byte[] ResultData { get; set; }
        public ApplicationErrorException Error { get; set; }
        public string ErrorDetail { get; set; }
    }
}
