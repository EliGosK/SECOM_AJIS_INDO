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
    /// Parameter for screen MAS010.
    /// </summary>
    public class MAS010_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public bool HasEditPermission { get; set; }
        [KeepSession]
        public bool HasDeletePermission { get; set; }
        [KeepSession]
        public List<doCompanyType> CompanyTypeList { get; set; }
        [KeepSession]
        public doCustomer CustomerData { get; set; }
        public List<doSite> SiteDataList { get; set; }
        [KeepSession]
        public List<doSite> UpdateList { get; set; }
        [KeepSession]
        public List<doSite> RemoveList { get; set; }

        public List<tbm_Group> CustomerGruopList { get; set; } // cbo
        public List<dtCustomeGroupData> CustomerGruopList_ForView { get; set; } // grid
        public string CustomerCode { get; set; }
        [KeepSession]
        public List<tbm_BuildingUsage> tbm_BuildingUsageList {get;set;}
    }

    /// <summary>
    /// DO for stored information of company.
    /// </summary>
    public class MAS010_GetCompanyFullName
    {
        public string NameEN { get; set; }
        public string NameLC { get; set; }
        public string CompanyTypeCode { get; set; }
        public string FullNameEN { get; set; }
        public string FullNameLC { get; set; }
    }

    /// <summary>
    /// DO for stored information of site.
    /// </summary>
    [MetadataType(typeof(MAS010_UpdateSite_MetaData))]
    public class MAS010_UpdateSite : doSite
    {
        public string ProvinceCodeEN { get; set; }
        public string ProvinceCodeLC { get; set; }
        public string DistrictCodeEN { get; set; }
        public string DistrictCodeLC { get; set; }
    }

    /// <summary>
    /// DO for stored information of customer.
    /// </summary>
    [MetadataType(typeof(MAS010_UpdateCustomer_MetaData))]
    public class MAS010_UpdateCustomer : doCustomer
    {
        public string ProvinceCodeEN { get; set; }
        public string ProvinceCodeLC { get; set; }
        public string DistrictCodeEN { get; set; }
        public string DistrictCodeLC { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    public class MAS010_UpdateSite_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblSiteNameEN",
                        ControlName = "site_SiteNameEN")]
        public string SiteNameEN { get; set; } //lblSiteNameEN
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblSiteNameLC",
        //                ControlName = "site_SiteNameLC")]
        //public string SiteNameLC { get; set; } //lblSiteNameLC
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblBuildingUsageCode",
                        ControlName = "site_BuildingUsageCode")]
        public string BuildingUsageCode { get; set; } //lblBuildingUsageCode
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblAdress",
                        ControlName = "site_AddressEN")]
        public string AddressEN { get; set; } //lblAdress
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblAdress",
        //                ControlName = "site_AddressLC")]
        //public string AddressLC { get; set; } //lblAdress
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblRoad",
        //                ControlName = "site_RoadEN")]
        //public string RoadEN { get; set; } //lblRoad
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblRoad",
        //                ControlName = "site_RoadLC")]
        //public string RoadLC { get; set; } //lblRoad
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblTambol_Kwaeng",
                        ControlName = "site_SubDistrictEN")]
        public string SubDistrictEN { get; set; } //lblTambol_Kwaeng
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblTambol_Kwaeng",
        //                ControlName = "site_SubDistrictLC")]
        //public string SubDistrictLC { get; set; } //lblTambol_Kwaeng
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblJangwat",
                        ControlName = "site_ProvinceCodeEN")]
        public string ProvinceCodeEN { get; set; } //lblJangwat
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblJangwat",
        //                ControlName = "site_ProvinceCodeLC")]
        //public string ProvinceCodeLC { get; set; } //lblJangwat
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblAmper_Ked",
                        ControlName = "site_DistrictCodeEN")]
        public string DistrictCodeEN { get; set; } //lblAmper_Ked
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblAmper_Ked",
        //                ControlName = "site_DistrictCodeLC")]
        //public string DistrictCodeLC { get; set; } //lblAmper_Ked
    }

    public class MAS010_UpdateCustomer_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblCustomerType",
                        ControlName = "cust_CustTypeCode")]
        public string CustTypeCode { get; set; } //lblCustomerType
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblName_English",
                        ControlName = "cust_CustNameEN")]
        public string CustNameEN { get; set; } //lblName_English
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblName_Local",
        //                ControlName = "cust_CustNameLC")]
        //public string CustNameLC { get; set; } //lblName_Local
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblNationality",
                        ControlName = "cust_RegionCode")]
        public string RegionCode { get; set; } //lblNationality
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblBusinessType",
                        ControlName = "cust_BusinessTypeCode")]
        public string BusinessTypeCode { get; set; } //lblBusinessType
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblAdress",
                        ControlName = "cust_AddressEN")]
        public string AddressEN { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblAdress",
        //                ControlName = "cust_AddressLC")]
        //public string AddressLC { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblRoad",
        //                ControlName = "cust_RoadEN")]
        //public string RoadEN { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblRoad",
        //                ControlName = "cust_RoadLC")]
        //public string RoadLC { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblTambol_Kwaeng",
                        ControlName = "cust_SubDistrictEN")]
        public string SubDistrictEN { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblTambol_Kwaeng",
        //                ControlName = "cust_SubDistrictLC")]
        //public string SubDistrictLC { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblJangwat",
                        ControlName = "cust_ProvinceCodeEN")]
        public string ProvinceCodeEN { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblJangwat",
        //                ControlName = "cust_ProvinceCodeLC")]
        //public string ProvinceCodeLC { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS010",
                        Parameter = "lblAmper_Ked",
                        ControlName = "cust_DistrictCodeEN")]
        public string DistrictCodeEN { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS010",
        //                Parameter = "lblAmper_Ked",
        //                ControlName = "cust_DistrictCodeLC")]
        //public string DistrictCodeLC { get; set; }
    }
}