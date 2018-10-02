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
    /// Parameter for screen MAS030.
    /// </summary>
    public class MAS030_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public doBillingClient InputData { get; set; }
        [KeepSession]
        public List<doCompanyType> CompanyTypeList { get; set; }
    }

    /// <summary>
    /// DO for stored information of company.
    /// </summary>
    public class MAS030_GetCompanyFullName
    {
        public string NameEN { get; set; }
        public string NameLC { get; set; }
        public string CompanyTypeCode { get; set; }
        public string FullNameEN { get; set; }
        public string FullNameLC { get; set; }
    }

    /// <summary>
    /// DO for stored information of billing client using in check require field.
    /// </summary>
    [MetadataType(typeof(MAS030_CheckReqField_MetaData))]
    public class MAS030_CheckReqField : doBillingClient
    {
        public string BranchType { get; set; }
    }

    /// <summary>
    /// DO for validate combobox
    /// </summary>
    [MetadataType(typeof(MAS030_ValidateCombo_MetaData))]
    public class MAS030_ValidateCombo : doCustomer
    {
    }
}
namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    public class MAS030_CheckReqField_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS030",
                        Parameter = "lblCustomerType", 
                        ControlName = "mas030_CustomerType")] 
        public string CustTypeCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS030",
                        Parameter = "lblNameEn",
                        ControlName = "mas030_CustNameEN")] 
        public string NameEN { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS030",
        //                Parameter = "lblNameLc",
        //                ControlName = "mas030_CustNameLC")] 
        //public string NameLC { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS030",
                        Parameter = "lblAddressEn",
                        ControlName = "mas030_AddressEN")] 
        public string AddressEN { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
        //                Screen = "MAS030",
        //                Parameter = "lblAddressLc",
        //                ControlName = "mas030_AddressLC")] 
        //public string AddressLC { get; set; }

        //Comment by Jutarat A. on 25122013
        /*//Add by Jutarat A. on 12122013
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS030",
                        Parameter = "lblBranchNameEn",
                        ControlName = "mas030_BranchNameEN")]
        public string BranchNameEN { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER,
                        Screen = "MAS030",
                        Parameter = "lblBranchNameLc",
                        ControlName = "mas030_BranchNameLC")]
        public string BranchNameLC { get; set; }
        //End Add*/
        //End Comment
    }
    public class MAS030_ValidateCombo_MetaData
    {
        [CodeHasValue("CustTypeName",
            Controller = MessageUtil.MODULE_MASTER,
            Screen = "MAS030",
            MessageCode = MessageUtil.MessageList.MSG0066,
            Order = 1,
            Parameter = "lblCustomerType",
            ControlName = "mas030_CustomerType")]
        public string CustTypeCode { get; set; }
    }
}