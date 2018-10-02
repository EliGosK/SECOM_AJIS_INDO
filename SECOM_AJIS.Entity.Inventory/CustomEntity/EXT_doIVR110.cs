using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class doIVR110
    {
        public int DiffQty
        {
            get
            {
                return this.CheckingQty - this.StockQty;
            }
        }
    }
}
