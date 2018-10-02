using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Master;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Presentation.Master.Models.MetaData;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master.CustomAttribute;

namespace SECOM_AJIS.Presentation.Master.Models
{
    /// <summary>
    /// Parameter for screen MAS050.
    /// </summary>
    public class MAS050_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public doCustomerWithGroup doCustomer { get; set; }
    }

    /// <summary>
    /// DO for validate combobox
    /// </summary>
    [MetadataType(typeof(MAS050_ValidateCombo_MetaData))]
    public class MAS050_ValidateCombo : doCustomer
    {
    }

    /// <summary>
    /// DO for stored customer data in case check require field.
    /// </summary>
    [MetadataType(typeof(MAS050_CheckRequiredField_MetaData))]
    public class MAS050_CheckRequiredField : doCustomerWithGroup
    {
        public bool IsFullValidate { get; set; }
    }

    /// <summary>
    /// DO for check required field
    /// </summary>
    [MetadataType(typeof(MAS050_CheckRequiredFieldNotFull_MetaData))]
    public class MAS050_CheckRequiredFieldNotFull : MAS050_CheckRequiredField
    {
    }

    /// <summary>
    /// DO for check required field of customer
    /// </summary>
    [MetadataType(typeof(MAS050_CheckRequiredFieldCustNull_MetaData))]
    public class MAS050_CheckRequiredFieldCustNull : MAS050_CheckRequiredField
    {
    }

    /// <summary>
    /// DO for stored Customer Group data.
    /// </summary>
    public class MAS050_CustomerGroup
    {
        public string CustCode { get; set; }
        [NotNullOrEmpty(ControlName = "CustomerGroupCode")]
        public string GroupCode { get; set; }

        public List<dtCustomerGroup> CustomerGroupList { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    public class MAS050_ValidateCombo_MetaData
    {
        [CodeHasValue("CustTypeName",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 1,
            Parameter = "lblCustomerType",
            ControlName = "CustomerType")]
        public string CustTypeCode { get; set; }
        [CodeHasValue("CompanyTypeName",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 2,
            Parameter = "lblCompanyType",
            ControlName = "CompanyType")]
        public string CompanyTypeCode { get; set; }
        [CodeHasValue("FinancialMaketTypeName",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 3,
            Parameter = "lblFinancialMarketType",
            ControlName = "FinancialMarketType")]
        public string FinancialMarketTypeCode { get; set; }
        [CodeHasValue("Nationality",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 4,
            Parameter = "lblNationality",
            ControlName = "Nationality")]
        public string RegionCode { get; set; }
        [CodeHasValue("BusinessTypeName",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 5,
            Parameter = "lblBusinessType",
            ControlName = "BusinessType")]
        public string BusinessTypeCode { get; set; }
        [CodeNotNullOtherNotNull("ProvinceCode",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 6,
            Parameter = "lblJangwatEN",
            ControlName = "ProvinceEN")]
        public string ProvinceNameEN { get; set; }
        [CodeNotNullOtherNotNull("DistrictCode",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 7,
            Parameter = "lblAmper_KedEN",
            ControlName = "DistrictEN")]
        public string DistrictNameEN { get; set; }
        [CodeNotNullOtherNotNull("ProvinceCode",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 8,
            Parameter = "lblJangwatEN",
            ControlName = "ProvinceLC")]
        public string ProvinceNameLC { get; set; }
        [CodeNotNullOtherNotNull("DistrictCode",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 9,
            Parameter = "lblAmper_KedLC",
            ControlName = "DistrictLC")]
        public string DistrictNameLC { get; set; }
    }
    public class MAS050_CheckRequiredField_MetaData
    {
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 1,
            Parameter = "lblCustomerType",
            ControlName = "CustomerType")]
        public string CustTypeCode { get; set; }
        [IDNoNotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 2,
            Parameter = "lblIDno_TaxIDno",
            ControlName = "IDNo")]
        public string IDNo { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 3,
            Parameter = "lblName_English",
            ControlName = "CustNameEN")]
        public string CustNameEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS050",
        //    Order = 4,
        //    Parameter = "lblName_Local",
        //    ControlName = "CustNameLC")]
        //public string CustNameLC { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 5,
            Parameter = "lblNationality",
            ControlName = "Nationality")]
        public string RegionCode { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 6,
            Parameter = "lblBusinessType",
            ControlName = "BusinessType")]
        public string BusinessTypeCode { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 7,
            Parameter = "lblAdressEN",
            ControlName = "AddressEN")]
        public string AddressEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS050",
        //    Order = 8,
        //    Parameter = "lblAdressLC",
        //    ControlName = "AddressLC")]
        //public string AddressLC { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS050",
        //    Order = 9,
        //    Parameter = "lblRoadEN",
        //    ControlName = "RoadEN")]
        //public string RoadEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS050",
        //    Order = 10,
        //    Parameter = "lblRoadLC",
        //    ControlName = "RoadLC")]
        //public string RoadLC { get; set; }
        [NotNullOrEmpty(
           Controller = MessageUtil.MODULE_MASTER,
           Screen = "MAS050",
           Order = 11,
           Parameter = "lblTambol_KwaengEN",
           ControlName = "SubDistrictEN")]
        public string SubDistrictEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS050",
        //    Order = 12,
        //    Parameter = "lblTambol_KwaengLC",
        //    ControlName = "SubDistrictLC")]
        //public string SubDistrictLC { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 13,
            Parameter = "lblJangwatEN",
            ControlName = "ProvinceEN")]
        public string ProvinceNameEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS050",
        //    Order = 14,
        //    Parameter = "lblJangwatLC",
        //    ControlName = "ProvinceLC")]
        //public string ProvinceNameLC { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 15,
            Parameter = "lblAmper_KedEN",
            ControlName = "DistrictEN")]
        public string DistrictNameEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS050",
        //    Order = 16,
        //    Parameter = "lblAmper_KedLC",
        //    ControlName = "DistrictLC")]
        //public string DistrictNameLC { get; set; }
    }
    public class MAS050_CheckRequiredFieldNotFull_MetaData
    {
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 1,
            Parameter = "lblCustomerType",
            ControlName = "CustomerType")]
        public string CustTypeCode { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 2,
            Parameter = "lblName_English",
            ControlName = "CustNameEN")]
        public string CustNameEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS050",
        //    Order = 3,
        //    Parameter = "lblName_Local",
        //    ControlName = "CustNameLC")]
        //public string CustNameLC { get; set; }
    }
    public class MAS050_CheckRequiredFieldCustNull_MetaData
    {
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 1,
            Parameter = "lblCustomerType",
            ControlName = "CustomerType")]
        public string CustTypeCode { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 2,
            Parameter = "lblName_English",
            ControlName = "CustNameEN")]
        public string CustNameEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_MASTER,
        //    Screen = "MAS050",
        //    Order = 3,
        //    Parameter = "lblName_Local",
        //    ControlName = "CustNameLC")]
        //public string CustNameLC { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS050",
            Order = 4,
            Parameter = "lblNationality",
            ControlName = "Nationality")]
        public string RegionCode { get; set; }
    }
}