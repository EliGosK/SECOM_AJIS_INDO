using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    public class View_dtOffice : dtOffice
    {
        public string OfficeName { get; set; }
        public string ValueCodeDisplay
        {
            get
            {
                return SECOM_AJIS.Common.Util.CommonUtil.TextCodeName(this.OfficeCode, this.OfficeName);
            }
        }
    }
}
