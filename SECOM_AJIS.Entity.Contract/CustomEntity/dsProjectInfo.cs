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
    public class dsProjectInfo
    {
        public List<tbt_Project> tbt_Project { get; set; }
        public dtProjectPurcheser dtProjectPurcheser { get; set; }
    }
}
