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
    public class doSearchARListByRole
    {
        public string ARRole { get; set; }
        public string ARStatus { get; set; }
        public string ARSpecifyPeriod { get; set; }
        public DateTime? ARSpecifyPeriodFrom { get; set; }
        public DateTime? ARSpecifyPeriodTo { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{

}

