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
    /// Parameter for screen MAS010.
    /// </summary>
    public class MAS090_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public bool hasPermissionAdd { get; set; }
        [KeepSession]
        public bool hasPermissionEdit { get; set; }
        public DateTime updateDate { get; set; }
    }

    /// <summary>
    /// DO for stored information of instrument.
    /// </summary>
    [MetadataType(typeof(MAS090_SaveInstrument_MetaData))]
    public class MAS090_SaveInstrument : tbm_Instrument {
        public string AddUnitPriceText { get; set; }
        public string currentExpansionTypeCode { get; set; }
    }
}

namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    public class MAS090_SaveInstrument_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                Screen = "MAS090",
                Parameter = "lblInstrumentCode", 
                ControlName = "InstrumentCode")]
        public string InstrumentCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                Screen = "MAS090",
                Parameter = "lblApproveNo", 
                ControlName = "ApproveNo")]
        public string ApproveNo { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                Screen = "MAS090",
                Parameter = "lblInstrumentName", 
                ControlName = "InstrumentName")]
        public string InstrumentName { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                Screen = "MAS090",
                Parameter = "lblNameForSupplier",
                ControlName = "InstrumentNameForSupplier")]
        public string InstrumentNameForSupplier { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                Screen = "MAS090",
                Parameter = "lblInstrumentType",
                ControlName = "InstrumentTypeCode")]
        public string InstrumentTypeCode { get; set; }
        // [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //        Screen = "MAS090",
        //        Parameter = "lblNameForCustLocal",
        //        ControlName = "InstrumentNameForCustomerLC")]
        //public string InstrumentNameForCustomerLC { get; set; }
    }
}

