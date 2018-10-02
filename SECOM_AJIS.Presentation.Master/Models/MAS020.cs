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
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Master.Models
{
    /// <summary>
    /// Screen parameter of MAS020
    /// </summary>
    public class MAS020_ScreenParameter : ScreenParameter
    {
        public tbm_BillingClient currentBillingClient { get; set; }

        public List<doCompanyType> CompanyTypeList { get; set; }
    }

    /// <summary>
    /// Inheriatnce do of table billing client for MAS020
    /// </summary>
    [MetadataType(typeof(MAS020_BillingClientData_MetaData))]
    public class MAS020_BillingClientData : tbm_BillingClient
    {
        public string BranchType { get; set; }
    }

}

namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    /// <summary>
    /// Validate input parameter of billing client for MAS020
    /// </summary>
    public class MAS020_BillingClientData_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
             Screen = "MAS020",
             Parameter = "lblCustomerType",
             ControlName = "CustTypeCode")]
        public string CustTypeCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
             Screen = "MAS020",
             Parameter = "lblNameEN",
             ControlName = "NameEN")]
        public string NameEN { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //     Screen = "MAS020",
        //     Parameter = "lblNameLC",
        //     ControlName = "NameLC")]
        //public string NameLC { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
             Screen = "MAS020",
             Parameter = "lblAddressEN",
             ControlName = "AddressEN")]
        public string AddressEN { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //     Screen = "MAS020",
        //     Parameter = "lblAddressLC",
        //     ControlName = "AddressLC")]
        //public string AddressLC { get; set; }

        //Comment by Jutarat A. on 25122013
        /*//Add by Jutarat A. on 12122013
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
             Screen = "MAS020",
             Parameter = "lblBranchNameEN",
             ControlName = "BranchNameEN")]
        public string BranchNameEN { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
             Screen = "MAS020",
             Parameter = "lblBranchNameLC",
             ControlName = "BranchNameLC")]
        public string BranchNameLC { get; set; }
        //End Add*/
        //End Comment
    }
}


