using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Presentation.Master.Models.MetaData;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Master.Models
{
    /// <summary>
    /// Screen paramater of MAS130
    /// </summary>
    public class MAS130_ScreenParameter : ScreenParameter
    {
        public tbm_SafetyStock currentSafeStock { get; set; }
    }

    /// <summary>
    /// Inheritance do of table safety stock for MAS130
    /// </summary>
    [MetadataType(typeof(MAS130_SafetyStockData_MetaData))]
    public class MAS130_SafetyStockData : tbm_SafetyStock
    { 
    
    }
}

namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    /// <summary>
    /// Validate input parameter safety stock of MAS130
    /// </summary>
    public class MAS130_SafetyStockData_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
             Screen = "MAS130",
             Parameter = "lblInstrument",
             ControlName = "InstrumentCode")]
        public string InstrumentCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
             Screen = "MAS130",
             Parameter = "lblInventoryFixedQuantity",
             ControlName = "InventoryFixedQuantity")]
        public string InventoryFixedQuantity { get; set; }
    }
}

