using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Common.MetaData;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter for CMS240
    /// </summary>
    public class CMS240_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_COMMON,
                        Screen = "CMS240",
                        Parameter = "lblmonthandyear",
                        ControlName = "MonthYear")]
        public DateTime? MonthYear { get; set; }
        [KeepSession]
        public dtTPL PurgeLogData { get; set; }
        [KeepSession]
        public List<CMS240_PurgeLogDataDetail> PurgeLogDetailData { get; set; }
    }


    /// <summary>
    /// Data object of Purge log data detail
    /// </summary>
    public class CMS240_PurgeLogDataDetail
    {
        public string TableName { get; set; }
        public string ErrorDescription { get; set; }
    }

    /// <summary>
    /// Data object of Purge log status
    /// </summary>
    public class CMS240_Status  : ScreenParameter
    {
        public bool SuspendFlag { get; set; }
        public bool isShowPurgeLogDataDetail { get; set; }
        public string PurgeStatusName { get; set; }
        public string PurgeStatus { get; set; }
        public string xml { get; set; }
        public bool IsPurgeSucceeded { get; set; }
        public DateTime? MonthYear { get; set; }
        public bool IsExistInTransLog { get; set; }
    }


}
