using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class RPTSlipReport
    {
        public List<RPTMACheckupSlipDo> listRPTMACheckupSlipDo { get; set; }
        public List<RPTInstrumentCheckupDo> listRPTInstrumentCheckupDo { get; set; }
    }
}
