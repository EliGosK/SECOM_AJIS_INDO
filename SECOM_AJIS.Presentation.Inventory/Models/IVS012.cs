using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Presentation.Inventory.Models.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter for screen IVS012.
    /// </summary>
    public class IVS012_ScreenParameter : ScreenParameter
    {
        public List<doInventorySlipDetailList> lstInventorySlipDetail { get; set; }
        [KeepSession]
        public doOffice office { get; set; }
        public List<doIvs012Inventory> lstIVS012Inventory { get; set; }
        public string PreloadSlipNo { get; set; }
        public string SlipNo { get; set; }
        public List<tbt_InventorySlip> lstInventorySlip { get; set; } //Add by Jutarat A. on 30052013
        public string VoucherNo { get; set; }
        public DateTime? VoucherDate { get; set; }
    }

    /// <summary>
    /// Search condition for search slip use for validate.
    /// </summary>
    [MetadataType(typeof(IVS012_SearchCond_Meta))]
    public class IVS012_SearchCond
    {
        public string SlipNo { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class IVS012_SearchCond_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                Screen = "IVS012",
                Parameter = "lblStockInSlipNo",
                ControlName = "SlipNo", MessageCode = MessageUtil.MessageList.MSG4051, Module = MessageUtil.MODULE_INVENTORY)]
        public string SlipNo { get; set; }
    }
}