using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Common
{
    public class View_dtMonthYear:SECOM_AJIS.DataEntity.Common.dtMonthYear
    {
        public string strDisplay
        {
            get
            {
                return String.Format("{0:MMMM yyyy}", this.MonthYear);
            }
        }
    }
}
