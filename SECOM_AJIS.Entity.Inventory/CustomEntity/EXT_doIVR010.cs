using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class doIVR010 {
        public string TransferQtyShow {
            get {
                return TransferQty.ToString("#,##0");
            }
        }
    }
}