using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract.MetaData;


namespace SECOM_AJIS.Presentation.Contract.Models
{

    /// <summary>
    /// DO of session parameter screen CTS240
    /// </summary>
    [MetadataType(typeof(CTS240_MetaData))]
    public class CTS240_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public string strProjectCode { get; set; }
        public List<dtTbt_ProjectStockoutIntrumentForView> lstWip { get; set; }
        public doRegisterProject240 doRegProject240 { get; set; }
    }
    /// <summary>
    /// DO of customer data
    /// </summary>
    [MetadataType(typeof(tbt_ProjectPurchaserCustomer_MetaData))]
    public class doRegisterCustomer : doCustomer
    {
        public ActionFlag ActionFlag { get; set; }
    }
    /// <summary>
    /// DO of tbt_projectsystemdetail for view
    /// </summary>
    [MetadataType(typeof(CTS240_SystemProduct_Meta))]
    public class CTS240_SystemProduct : dtTbt_ProjectSystemDetailForView
    {
        public string ProjectCode { get; set; }
        public string tmpProductCodeName { get; set; }
        public ActionFlag ActionFlag { get; set; }
    }
    /// <summary>
    /// DO of tbt_ProjectOtherRalatedCompany for view
    /// </summary>
    [MetadataType(typeof(CTS240_OtherRelate_MetaData))]
    public class CTS240_OtherRelate : tbt_ProjectOtherRalatedCompany
    {
        public ActionFlag ActionFlag { get; set; }
    }
    /// <summary>
    /// DO of Tbt_ProjectSupportStaffDetailForView for view
    /// </summary>
    [MetadataType(typeof(CTS240_Support_MetaData))]
    public class CTS240_SupportStaff : dtTbt_ProjectSupportStaffDetailForView
    {
        public ActionFlag ActionFlag { get; set; }
    }
    /// <summary>
    /// DO of Tbt_ProjectExpectedInstrumentDetail for view
    /// </summary>
    [MetadataType(typeof(CTS240_ExpectInst_MetaData))]
    public class CTS240_ExpectInstrument : dtTbt_ProjectExpectedInstrumentDetailsForView
    {
        public doInstrumentData dtNewInstrument { get; set; }
        public ActionFlag ActionFlag { get; set; }
    }
    /// <summary>
    /// DO of tbt_Project for view
    /// </summary>
    [MetadataType(typeof(CTS240_tbt_ProjectMetaData))]
    public class CTS240_ProjectData : tbt_Project { }

    /// <summary>
    /// DO for register project data
    /// </summary>
    public class doRegisterProject240
    {
        public CTS240_ProjectData doTbt_Project { get; set; }
        public List<CTS240_SystemProduct> SystemProduct { get; set; }
        public List<CTS240_OtherRelate> OtherRelate { get; set; }
        public List<CTS240_SupportStaff> SupportStaff { get; set; }
        public List<CTS240_ExpectInstrument> ExpectInstrument { get; set; }
        public List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> StockOut { get; set; }
        public doRegisterCustomer doRegCust { get; set; }

    }
    /// <summary>
    /// DO of Tbt_ProjectStockoutBranchIntrumentDetail for view
    /// </summary>
    public class View_dtTbt_ProjectStockoutBranchIntrumentDetailForView : dtTbt_ProjectStockoutBranchIntrumentDetailForView
    {
        public int? SumAssignQtyBefore { get { return base.SumAssignQty; } }
        public int? SumNotAssignQtyBefore { get { return base.SumNotAssign; } }
        public int? AssignBranchQtyBefore { get { return base.AssignBranchQty; } }

    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS240_SystemProduct_Meta
    {          //Valid attribute
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS240",
                        Parameter = "lblSystemProductName",
                        ControlName = "SysProductName")]
        public string ProductCode { get; set; }

    }
    public class CTS240_Support_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS240",
                Parameter = "headerSupportStaff",
         ControlName = "SupportStaffCode")]
        public string EmpNo { get; set; }
    }
    public class CTS240_MetaData
    {
        [NotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG3216, Module = MessageUtil.MODULE_CONTRACT, Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS240",
                Parameter = "lblProjectCode")]
        public string strProjectCode { get; set; }
    }

    public class CTS240_OtherRelate_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS230",
                Parameter = "lblCompanyName",
                ControlName = "ProjRelCompName")]
        public string CompanyName { get; set; }
    }
    public class CTS240_ExpectInst_MetaData
    {
        [NotNullOrEmpty(Module = MessageUtil.MODULE_COMMON, ControlName = "InstrumentCode", MessageCode = MessageUtil.MessageList.MSG0081, Screen = "CTS230", Parameter = "lblInstrumentCode")]
        public string InstrumentCode { get; set; }

    }
    public class CTS240_tbt_ProjectMetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                   Screen = "CTS240",
                   Parameter = "lblProjectName",
                   ControlName = "pjProjectName")]
        public string ProjectName { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                     Screen = "CTS240",
                     Parameter = "lblProjectRepresentativeAddress", ControlName = "pjProjectAddress")]
        public string ProjectAddress { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                 Screen = "CTS240",
                 Parameter = "lblHeadSalesman", ControlName = "stHeadSalesmanEmpNo")]
        public string HeadSalesmanEmpNo { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
              Screen = "CTS240",
              Parameter = "lblPjManager", ControlName = "stProjectManagerEmpNo")]
        public string ProjectManagerEmpNo { get; set; }
    }

}