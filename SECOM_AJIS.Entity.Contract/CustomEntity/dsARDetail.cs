using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsARDetail
    {
        public dtAR dtAR { get; set; }
        public List<dtARRole> dtARRole { get; set; }
        public List<tbt_ARHistory> tbt_ARHistory { get; set; }
        public List<tbt_ARHistoryDetail> tbt_ARHistoryDetail { get; set; }
        public List<dtEmployeeOffice> dtEmployeeOffice { get; set; }
        public tbt_ARFeeAdjustment tbt_ARFeeAdjustment { get; set; }
    }
}
