using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory.MetaData;

namespace SECOM_AJIS.DataEntity.Inventory
{

    public partial class tbt_InventoryCheckingSlipDetail
    {
        public int Page { set; get; }
        public string key { get { return string.Format("{0}-{1}-{2}",this.InstrumentCode ,this.AreaCode ,this.ShelfNo); } }
        public string txtCheckingQtyID { set; get; }
        public int? DefaultCheckingQty { get; set; }
    }

}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
   

}