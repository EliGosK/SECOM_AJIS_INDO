using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.Globalization;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class doIVR192
    {
        public string RPT_Date
        {
            get
            {
                return (this.CreateDate.Date.Equals(new DateTime(1900, 1, 1)) ? "" : this.CreateDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            }
        }

        public string RPT_TotalAmountText
        {
            get
            {
                return ReportUtil.CurrencyToEnglishWords(this.TotalAmount, this.Currency);
            }
        }
    }
}
