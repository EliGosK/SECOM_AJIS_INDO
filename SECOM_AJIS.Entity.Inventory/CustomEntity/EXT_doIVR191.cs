using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class doIVR191
    {
        public string TotalPriceInWord
        {
            get
            {
                CommonUtil cm = new CommonUtil();
                return cm.ToBahtText(this.TotalPriceIncludeVat);
            }
        }
        
        public string TotalPricePerRowShow { get; set; }
        public string UnitPriceShow { get; set; }
        public string OrderQtyShow { get; set; }
    }
}
