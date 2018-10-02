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
    public class dsARDetailIn
    {
        public tbt_AR tbt_AR { get; set; }
        public tbt_ARHistory tbt_ARHistory { get; set; }
        public List<tbt_ARHistoryDetail> tbt_ARHistoryDetail { get; set; }
        public List<tbt_ARRole> tbt_ARRoleAdd { get; set; }
        public List<tbt_ARRole> tbt_ARRoleEdit { get; set; }
        public List<int> tbt_ARRoleDelete { get; set; }
        public List<tbt_ARFeeAdjustment> tbt_ARFeeAdjustment { get; set; } //Add by Jutarat A. on 03042013
    }
}
