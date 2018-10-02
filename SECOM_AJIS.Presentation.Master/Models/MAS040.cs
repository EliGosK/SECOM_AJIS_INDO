using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Presentation.Master.Models.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Master.Models
{
    /// <summary>
    /// Parameter for screen MAS040.
    /// </summary>
    public class MAS040_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public doSite doSite { get; set; }

        public List<tbm_BuildingUsage> tbm_BuildingUsageList;
    }

    /// <summary>
    /// DO for validate combobox
    /// </summary>
    [MetadataType(typeof(MAS040_ValidateCombo_MetaData))]
    public class MAS040_ValidateCombo : doSite
    {
    }

    /// <summary>
    /// DO for stored information of site use when check require field.
    /// </summary>
    [MetadataType(typeof(MAS040_CheckRequiredField_MetaData))]
    public class MAS040_CheckRequiredField : doSite
    {
    }
}
namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    public class MAS040_ValidateCombo_MetaData
    {
        [CodeHasValue("BuildingUsageName",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS040",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 1,
            Parameter = "lblBuildingUsageCode",
            ControlName = "UsageCode")]
        public string BuildingUsageCode { get; set; }
        [CodeNotNullOtherNotNull("ProvinceCode",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS040",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 2,
            Parameter = "lblJangwatEN",
            ControlName = "ProvinceEN")]
        public string ProvinceNameEN { get; set; }
        [CodeNotNullOtherNotNull("DistrictCode",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS040",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 3,
            Parameter = "lblAmper_KedEN",
            ControlName = "DistrictEN")]
        public string DistrictNameEN { get; set; }
        //[CodeNotNullOtherNotNull("ProvinceCode",
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS040",
        //    MessageCode = MessageUtil.MessageList.MSG0066,
        //    Order = 4,
        //    Parameter = "lblJangwatEN",
        //    ControlName = "ProvinceLC")]
        //public string ProvinceNameLC { get; set; }
        //[CodeNotNullOtherNotNull("DistrictCode",
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS040",
        //    MessageCode = MessageUtil.MessageList.MSG0066,
        //    Order = 5,
        //    Parameter = "lblAmper_KedLC",
        //    ControlName = "DistrictLC")]
        //public string DistrictNameLC { get; set; }
    }
    public class MAS040_CheckRequiredField_MetaData
    {
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS040",
            Order = 1,
            Parameter = "lblSiteNameEN",
            ControlName = "Register #SiteNameEN")]
        public string SiteNameEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS040",
        //    Order = 2,
        //    Parameter = "lblSiteNameLC",
        //    ControlName = "Register #SiteNameLC")]
        //public string SiteNameLC { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS040",
            Order = 3,
            Parameter = "lblBuildingUsageCode",
            ControlName = "UsageCode")]
        public string BuildingUsageCode { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS040",
            Order = 4,
            Parameter = "lblAdressEN",
            ControlName = "AddressEN")]
        public string AddressEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS040",
        //    Order = 5,
        //    Parameter = "lblAdressLC",
        //    ControlName = "AddressLC")]
        //public string AddressLC { get; set; }
        [NotNullOrEmpty(
           Controller = MessageUtil.MODULE_MASTER,
           Screen = "MAS040",
           Order = 8,
           Parameter = "lblTambol_KwaengEN",
           ControlName = "SubDistrictEN")]
        public string SubDistrictEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS040",
        //    Order = 9,
        //    Parameter = "lblTambol_KwaengLC",
        //    ControlName = "SubDistrictLC")]
        //public string SubDistrictLC { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS040",
            Order = 10,
            Parameter = "lblJangwatEN",
            ControlName = "ProvinceEN")]
        public string ProvinceNameEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS040",
        //    Order = 11,
        //    Parameter = "lblJangwatLC",
        //    ControlName = "ProvinceLC")]
        //public string ProvinceNameLC { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS040",
            Order = 12,
            Parameter = "lblAmper_KedEN",
            ControlName = "DistrictEN")]
        public string DistrictNameEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS040",
        //    Order = 13,
        //    Parameter = "lblAmper_KedLC",
        //    ControlName = "DistrictLC")]
        //public string DistrictNameLC { get; set; }
    }
}