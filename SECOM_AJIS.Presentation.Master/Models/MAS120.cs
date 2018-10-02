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
    /// Screen parameter of MAS120
    /// </summary>
    public class MAS120_ScreenParameter : ScreenParameter
    {
        public DateTime? updateDate { get; set; }
    }

    /// <summary>
    /// Inheritance do of table shelf for MAS120
    /// </summary>
    [MetadataType(typeof(MAS120_ShelfData_MetaData))]
    public class MAS120_ShelfData : tbm_Shelf
    {

    }
}

namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    /// <summary>
    /// Validate input parameter of MAS120
    /// </summary>
    public class MAS120_ShelfData_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
             Screen = "MAS120",
             Parameter = "lblShelfNo",
             ControlName = "ShelfNo")]
        public string ShelfNo { get; set; }


        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                     Screen = "MAS120",
                     Parameter = "lblShelfName",
                     ControlName = "ShelfName")]
        public string ShelfName { get; set; }


        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                     Screen = "MAS120",
                     Parameter = "lblShelfType",
                     ControlName = "ShelfTypeCode")]
        public string ShelfTypeCode { get; set; }
    }
}

