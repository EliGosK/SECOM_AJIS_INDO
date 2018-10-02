using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    /// <summary>
    /// Data object for register transfer data.
    /// </summary>
    public class doRegisterTransferInstrumentData
    {
        public string SlipId { get; set; }

        public tbt_InventorySlip InventorySlip { get; set; }
        public List<tbt_InventorySlipDetail> lstTbt_InventorySlipDetail { get; set; }
    }



}