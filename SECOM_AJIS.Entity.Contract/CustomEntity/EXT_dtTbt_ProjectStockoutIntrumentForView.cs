using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtTbt_ProjectStockoutIntrumentForView
    {
        public int RemainQty
        {
            get
            {
                return Convert.ToInt32(this.InstrumentQty) - Convert.ToInt32(this.SumAssignQty);
            }
        }
    }
}
