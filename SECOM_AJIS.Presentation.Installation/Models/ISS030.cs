using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Common.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.Presentation.Installation.Models.MetaData;
using SECOM_AJIS.Presentation.Installation.Models;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Installation.Models
{
    /// <summary>
    /// DO for validate data
    /// </summary>
    public class ISS030_ValidateData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS030",
                        Parameter = "lblInstallationType",
                        ControlName = "InstallationType")]
        public string InstallationType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS030",
                        Parameter = "lblInstallationCauseReason",
                        ControlName = "CauseReason")]
        public string CauseReason { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS030",
                        Parameter = "lblInstallationBy",
                        ControlName = "InstallationBy")]
        public string InstallationBy { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS030",
                        Parameter = "lblChangeReason",
                        ControlName = "ChangeReasonCode")]
        public string ChangeReasonCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                       Screen = "ISS030",
                       Parameter = "lblInstallSlipIssueDate",
                       ControlName = "SlipIssueDate")]
        public string SlipIssueDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                       Screen = "ISS030",
                       Parameter = "lblInstallSlipOutputTarget",
                       ControlName = "SlipIssueOfficeCode")]
        public string SlipIssueOfficeCode { get; set; }


        public string NormalInstallFee { get; set; }

    }
    /// <summary>
    /// DO for send data to show in screen
    /// </summary>
    public class ISS030_RegisterStartResumeTargetData  ///// FOR SENT VALUE TO SCREEN
    {
        public dtRentalContractBasicForInstall dtRentalContractBasic { get; set; }        
        public dtSaleBasic dtSale { get; set; }
        public string ServiceTypeCode { get; set; }
        public string InstallType { get; set; }
        public string MaintenanceNo { get; set; }
        public string SlipNo { get; set; }
        public tbt_InstallationBasic do_TbtInstallationBasic { get; set; }
        public tbt_InstallationSlip do_TbtInstallationSlip { get; set; }
        public List<tbt_InstallationInstrumentDetails> do_TbtInstallationInstrumentDetail { get; set; }
        public bool m_blnbFirstTimeRegister { get; set; }
        public List<tbt_InstallationSlipDetails> do_TbtInstallationSlipDetails { get; set; }
        public List<doInstrumentData> ListInstrumentData { get; set; }
        public string[] arrayInstrumentName { get; set; }
        public List<ISS030_DOEmailData> ListDOEmail { get; set; }
        public string LastSlipStatusName { get; set; }
        public string InstallationOfficeOutputTarget { get; set; }

        public List<tbt_InstallationSlipDetails> do_TbtInstallationSlipDetailsForFacility { get; set; }
        public List<dtTbt_RentalInstrumentDetailsListForView> dtRentalInstrumentDetailsForFacility { get; set; }
        public string ContractCodeShort { get; set; }
        public List<doRentalInstrumentdataList> doRentalInstrumentdataList { get; set; }
        public List<doSaleInstrumentdataList> doSaleInstrumentdataList { get; set; }
        public bool blnUseContractData { get; set; }
        public string ContractProjectCodeForShow { get; set; }
        public doRentalFeeResult doRentalFeeResult { get; set; }
        public bool blnCheckCP12 { get; set; }

        public List<doRentalInstrumentdataList> doRentalInstrumentdataExchangeList { get; set; } //Add by Jutarat A. on 17062013
        public List<doSaleInstrumentdataList> doSaleInstrumentdataExchangeList { get; set; } //Add by Jutarat A. on 17062013

        public string RentalContactBasicLastChangeType { get; set; }
    } 
    /// <summary>
    /// DO of session parameter of screen ISS050
    /// </summary>
    public class ISS030_ScreenParameter : ScreenSearchParameter
    {        
        public ISS030_DOEmailData DOEmail { get; set; }
        public List<ISS030_DOEmailData> ListDOEmail { get; set; }
        public tbt_InstallationBasic do_TbtInstallationBasic { get; set; }
        public tbt_InstallationSlip do_TbtInstallationSlip { get; set; }
        public List<tbt_InstallationInstrumentDetails> do_TbtInstallationInstrumentDetail { get; set; }
        public List<tbt_InstallationSlipDetails> do_TbtInstallationSlipDetails { get; set; }

        public List<tbt_InstallationSlipDetails> do_TbtInstallationSlipDetailsForFacility { get; set; }
        public List<dtTbt_RentalInstrumentDetailsListForView> dtRentalInstrumentDetailsForFacility { get; set; }
        public List<doInstrumentData> ListInstrumentData { get; set; }
        public dtRentalContractBasicForInstall dtRentalContractBasic { get; set; }
        public dtSaleBasic dtSale { get; set; }
        public string ServiceTypeCode { get; set; }
        [KeepSession]
        public string ContractCodeShort { get; set; }
        public string ContractCodeLong { get; set; }
        public string InstallType { get; set; }
        public string MaintenanceNo { get; set; }
        public bool m_blnbFirstTimeRegister { get; set; }
        public string[] arrayInstrumentName { get; set; }
        public string LastSlipStatusName { get; set; }
        [KeepSession]
        public string strContractCode { get; set; }
        public string InstallationOfficeOutputTarget { get; set; }
        public List<doRentalInstrumentdataList> doRentalInstrumentdataList { get; set; }
        public List<doSaleInstrumentdataList> doSaleInstrumentdataList { get; set; }
        public bool blnUseContractData { get; set; }

        public List<doRentalInstrumentdataList> doRentalInstrumentdataExchangeList { get; set; } //Add by Jutarat A. on 17062013
        public List<doSaleInstrumentdataList> doSaleInstrumentdataExchangeList { get; set; } //Add by Jutarat A. on 17062013
    }

    [MetadataType(typeof(ISS030_DOEmailData_Meta))]
    public partial class ISS030_DOEmailData : dtEmailAddress
    {
    }
    /// <summary>
    /// DO for get instrument data condition
    /// </summary>
    public class ISS030_GetInstrumentDataCondition
    {
        public string InstrumentCode { get; set; }

        
        public string ProductCode { get; set; }

        public bool IsAskQuestion { get; set; }
    }



}

namespace SECOM_AJIS.Presentation.Installation.Models.MetaData
{
    public class ISS030_DOEmailData_Meta : ISS030_DOEmailData
    {
        [NotNullOrEmpty(ControlName = "EmailAddress", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Notify email")]
        public string EmailAddress { get; set; }
    }
}
