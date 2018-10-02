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
    /// Screen paremeter of MAS110
    /// </summary>
    public class MAS110_ScreenParameter : ScreenParameter
    {
        public doSubcontractor currentSubcontractor { get; set; }
    }

    /// <summary>
    /// Inheritance do of sub contractor for MAS110
    /// </summary>
    [MetadataType(typeof(MAS110_SubcontractorData_MetaData))]
    public class MAS110_SubcontractorData : doSubcontractor
    { 
    
    }
}

namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    /// <summary>
    /// Validate sub contractor data of MAS110
    /// </summary>
    public class MAS110_SubcontractorData_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
             Screen = "MAS110",
             Parameter = "lblCoCompanyCode",
             ControlName = "COCompanyCode")]
        public string COCompanyCode { get; set; }


        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                     Screen = "MAS110",
                     Parameter = "lblTeamNo",
                     ControlName = "InstallationTeam")]
        public string InstallationTeam { get; set; }


        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                     Screen = "MAS110",
                     Parameter = "lblSubcontractorNameEN",
                     ControlName = "SubContractorNameEN")]
        public string SubContractorNameEN { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //             Screen = "MAS110",
        //             Parameter = "lblSubcontractorNameLC",
        //             ControlName = "SubContractorNameLC")]
        //public string SubContractorNameLC { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                     Screen = "MAS110",
                     Parameter = "lblRepresentativePersonName",
                     ControlName = "RepresentSubContractorName")]
        public string RepresentSubContractorName { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                     Screen = "MAS110",
                     Parameter = "lblAddressEnglish",
                     ControlName = "AddressEN")]
        public string AddressEN { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //     Screen = "MAS110",
        //     Parameter = "lblAddressLocal",
        //     ControlName = "AddressLC")]
        //public string AddressLC { get; set; }
    }
}

