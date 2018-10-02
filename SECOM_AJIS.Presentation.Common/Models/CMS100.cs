using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Common.Models.MetaData;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Data object class of screen parameter CMS100
    /// </summary>
    public class CMS100_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string GroupCode { get; set; }
        [KeepSession]
        public doGroup GroupData { get; set; }
        [KeepSession]
        public List<dtGroupSummaryForShow> GroupSummaryList { get; set; }
        public List<dtCustomerListGrp> CustomerList { get; set; }
        public List<dtsiteListGrp> SiteList { get; set; }
        public List<dtContractListGrp> ContractList { get; set; }
    }

    //[MetadataType(typeof(CMS070_ContractByCode_MetaData))]
    //public class CMS070_ContractByCode : doSearchInfoCondition
    //{
    //}
}

namespace SECOM_AJIS.Presentation.Common.Models.MetaData
{
    //public class CMS070_CustomerByCode_MetaData
    //{
    //    [AtLeast1FieldNotNullOrEmpty]
    //    public string CustomerCode { get; set; }
    //    [AtLeast1FieldNotNullOrEmpty]
    //    public string GroupCode { get; set; }
    //}
}

