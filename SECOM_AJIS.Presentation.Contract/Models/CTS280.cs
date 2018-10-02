using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// DO for initial screen
    /// </summary>
    public class CTS280_ScreenParameter : ScreenParameter
    {
        CommonUtil c = new CommonUtil();

        [KeepSession]
        public String ContractCodeShow { get; set; }

        public String ContractCode
        {
            get { return c.ConvertContractCode(this.ContractCodeShow, CommonUtil.CONVERT_TYPE.TO_LONG); }
        }

        [KeepSession]
        public String ProductCode { get; set; }

        [KeepSession]
        public DateTime InstructionDate { get; set; }

        [KeepSession]
        public String Mode { get; set; }

        [KeepSession]
        public String CallerSessionKey { get; set; }

        [KeepSession]
        public dsCTS280Data data { get; set; }

        [KeepSession]
        public bool hasEditPermission { get; set; }

        public bool MaintenanceFeeFlag { get; set; }
        //public decimal? MaintenanceFeeInit { get; set; }
    }
    /// <summary>
    /// DO for CTS280 screen
    /// </summary>
    public class dsCTS280Data
    {
        private doMaintenanceCheckupInformation _doMaintCheckupInformation;
        private tbt_MaintenanceCheckup _dtTbtMaintenanceCheckup;
        private dsRentalContractData _dtEntireContract;
        public decimal? MaintenanceFeeInit { get; set; }
        //public decimal? MaintenanceFeeInitUsd { get; set; }  //Add by Narut T. 2017-02-10
        public doMaintenanceCheckupInformation doMaintCheckupInformation
        {
            get { return this._doMaintCheckupInformation; }
            set { this._doMaintCheckupInformation = value; }
        }
        public tbt_MaintenanceCheckup dtMaintenanceCheckup
        {
            get { return this._dtTbtMaintenanceCheckup; }
            set { this._dtTbtMaintenanceCheckup = value; }
        }
        public dsRentalContractData dtEntireContract
        {
            get { return this._dtEntireContract; }
            set { this._dtEntireContract = value; }
        }
    }
    //public class CTS280_InputParam
    //{
    //    public string ParamContractCode { get; set; }
    //    public string ParamProductCode { get; set; }
    //    public DateTime ParamInstructionDate { get; set; }
    //    public string ParamMode { get; set; }
    //    public string ParamCurrentMode { get; set; }
    //    public string ParamInstructionDateShow 
    //    {
    //        get { return (this.ParamInstructionDate == null) ? "" : this.ParamInstructionDate.ToString("dd-MMM-yyyy"); }
    //    }
    //}
    /// <summary>
    /// DO for render screen in view mode
    /// </summary>
    public class CTS280_RenderViewMode
    {
        public bool MaintenanceFeeFlag { get; set; }
        public bool EnableEditButton { get; set; }
    }
    /// <summary>
    /// DO for render screen in edit mode
    /// </summary>
    public class CTS280_RenderEditMode
    {
        public bool ShowOnlyExpectedMaintenanceDate { get; set; }
        public bool ShowMaintenanceFee { get; set; }
        public bool IsSetMaintenanceFee { get; set; }
        public string MetenanceFeeCurrencyType { get; set; }
        public decimal? MetenanceFee { get; set; }
    }
    /// <summary>
    /// DO for validate result data
    /// </summary>
    [MetadataType(typeof(CTS280_Result_MetaData))]
    public class CTS280_Result : tbt_MaintenanceCheckup
    {
        [LanguageMapping]
        public string MaintEmpName { get; set; }
        public string MaintEmpNameLC { get; set; }
        public string MaintEmpNameJP { get; set; }
        public string MaintEmpNameEN { get; set; }
        //public bool MaintenanceFeeFlag { get; set; }
        //public decimal? MaintenanceFeeInit { get; set; }
        public string ProcessType { get; set; }
        public bool InstrumentMalfunctionFlagData { get; set; }
        public bool NeedSalesmanFlagData { get; set; }
    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    /// <summary>
    /// Metadata for CTS280_Result DO
    /// </summary>
    public class CTS280_Result_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS280",
                        Parameter = "lblMaintenanceDate",
                        ControlName = "MaintenanceDate",
                        Order= 1)]
        public Nullable<System.DateTime> MaintenanceDate
        {
            get;
            set;
        }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS280",
                        Parameter = "lblCheckupUsageTime",
                        ControlName = "UsageTime",
                        Order = 6)]
        public Nullable<int> UsageTime
        {
            get;
            set;
        }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS280",
                        Parameter = "lblMaintenanceEmployee",
                        ControlName = "MaintEmpNo",
                        Order = 5)]
        public string MaintEmpNo
        {
            get;
            set;
        }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS280",
                        Parameter = "cboSubcontractorName",
                        ControlName = "SubcontractCode",
                        Order = 3)]
        public string SubcontractCode
        {
            get;
            set;
        }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS280",
                        Parameter = "lblPersonInChargeOfSubcontractor",
                        ControlName = "PICName",
                        Order = 4)]
        public string PICName
        {
            get;
            set;
        }
    }
}
