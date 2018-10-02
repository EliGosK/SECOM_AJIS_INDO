using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace SECOM_AJIS.DataEntity.Accounting
{
    public partial class dtAccountingDocumentList
    {
        public string ReportMonthYear
        {
            get
            {
                string strMonth = "";
                if (this.ReportMonth.HasValue)
                {
                    var date = new DateTime(2000, this.ReportMonth.Value, 1);
                    strMonth = date.ToString("MMMM");
                }
                return string.Format("{0}/{1}", strMonth, this.ReportYear);
            }
        }

        public string ReportTargetPeriod
        {
            get
            {
                string strTargetPeriod = string.Empty;
                string strTargetPeriodFrom = string.Empty;
                string strTargetPeriodTo = string.Empty;
                CultureInfo c = new CultureInfo("en-US");

                strTargetPeriodTo = this.TargetPeriodTo.ToString("dd-MMM-yyyy", c);

                switch (this.DocumentTimingType)
                {
                    case "D2":
                        strTargetPeriodFrom = this.TargetPeriodFrom.ToString("dd-MMM-yyyy", c);
                        break;
                    default:
                        break;
                }
                if(strTargetPeriodFrom == string.Empty)
                {
                    strTargetPeriod = strTargetPeriodTo;
                }
                else
                {
                    strTargetPeriod = string.Format("{0} ~ {1}", strTargetPeriodFrom, strTargetPeriodTo);
                }
                return strTargetPeriod;
            }
        }
    }
}
