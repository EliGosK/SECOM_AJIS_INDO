using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Presentation.Common
{
    /// <summary>
    /// DO for view group list
    /// </summary>
    [Serializable]
    public class View_dtGroupList : dtGroupList
    {
        public string GroupNameDisplay { get; set; }
        public string OfficeInCharge { get; set; }
        public string PersonInCharge { get; set; }
    }
}
