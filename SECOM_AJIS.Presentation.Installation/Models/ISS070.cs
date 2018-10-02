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
    /// DO of validate data
    /// </summary>
    public class ISS070_ValidateData
    {

    }
    /// <summary>
    /// DO of input data from screen
    /// </summary>
    public class ISS070_ScreenInput
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS070",
                        Parameter = "Installation start date",
                        ControlName = "InstallStartDate")]
        public string InstallStartDate { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS070",
                        Parameter = "Installation finish date",
                        ControlName = "InstallFinishDate")]
        public string InstallFinishDate { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS070",
                        Parameter = "Installation Complete Date",
                        ControlName = "InstallCompleteDate")]
        public string InstallCompleteDate { get; set; }
        public Decimal NewNormalContractFee { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS070",
                        Parameter = "Normal Installation Fee",
                        ControlName = "NewNormalInstallFee")]
        public string NewNormalInstallFee { get; set; }
        public string NewInstallFeeBillingType { get; set; }
        public Decimal NewBillingInstallFee { get; set; }
        public string NewBillingOCC { get; set; }
        public Decimal NewSECOMPaymentFee { get; set; }
        public Decimal NewSECOMRevenueFee { get; set; }
        public string NewApproveNo1 { get; set; }
        public string NewApproveNo2 { get; set; }
        public string NewMemo { get; set; }
    }
    /// <summary>
    /// DO for send data to show in screen 
    /// </summary>
    public class ISS070_RegisterStartResumeTargetData  ///// FOR SENT VALUE TO SCREEN
    {
        public string ContractCodeShort { get; set; }
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
        public List<ISS070_DOEmailData> ListDOEmail { get; set; }
        public string LastSlipStatusName { get; set; }
        public bool blnValidateContractError { get; set; }
        public dtProjectForInstall dtProject { get; set; }
        public string InstallationMemo { get; set; }
        public List<tbt_InstallationPOManagement> doTbt_InstallationPOManagement { get; set; }
        public string ContractProjectCodeForShow { get; set; }
    } 
    /// <summary>
    /// Session parameter of screen ISS070
    /// </summary>
    public class ISS070_ScreenParameter : ScreenSearchParameter
    {        
        public ISS070_DOEmailData DOEmail { get; set; }
        public List<ISS070_DOEmailData> ListDOEmail { get; set; }
        public tbt_InstallationBasic do_TbtInstallationBasic { get; set; }
        public tbt_InstallationSlip do_TbtInstallationSlip { get; set; }
        public List<tbt_InstallationInstrumentDetails> do_TbtInstallationInstrumentDetail { get; set; }
        public List<tbt_InstallationSlipDetails> do_TbtInstallationSlipDetails { get; set; }
        public List<doInstrumentData> ListInstrumentData { get; set; }
        public dtRentalContractBasicForInstall dtRentalContractBasic { get; set; }
        public dtSaleBasic dtSale { get; set; }
        public dtProjectForInstall dtProject { get; set; }
        public string ServiceTypeCode { get; set; }
        [KeepSession]
        public string ContractCodeShort { get; set; }
        public string ContractCodeLong { get; set; }
        public string InstallType { get; set; }
        public string MaintenanceNo { get; set; }
        public bool m_blnbFirstTimeRegister { get; set; }
        public string[] arrayInstrumentName { get; set; }
        public string LastSlipStatusName { get; set; }
        public bool blnValidateContractError { get; set; }
        public bool checkHaveCond1 { get; set; }
        public bool checkHaveCond2 { get; set; }
        public bool checkHaveCond3 { get; set; }
        public bool checkHaveCond4 { get; set; }
        public bool checkHaveCond5 { get; set; }
        public List<ISS070_GridInstrumentData> GridInstrumentForValid { get; set; }
        public tbt_InstallationManagement dtInstallationManagement { get; set; }
        [KeepSession]
        public string strContractProjectCode { get; set; }
        public string InstallationMemo { get; set; }
        public List<tbt_InstallationPOManagement> doTbt_InstallationPOManagement { get; set; }
    }
    /// <summary>
    /// DO of email data 
    /// </summary>
    [MetadataType(typeof(ISS070_DOEmailData_Meta))]
    public partial class ISS070_DOEmailData : dtEmailAddress
    {
    }

    /// <summary>
    /// DO of tbt_InstallationSlipDetails
    /// </summary>
    public partial class ISS070_GridInstrumentData : tbt_InstallationSlipDetails
    {
        public int ContractInstalledAfterChange { get; set; }
    }
    /// <summary>
    /// DO condition for get instrument data
    /// </summary>
    public class ISS070_GetInstrumentDataCondition
    {
        public string InstrumentCode { get; set; }

        
        public string ProductCode { get; set; }

        public bool IsAskQuestion { get; set; }
    }



}

namespace SECOM_AJIS.Presentation.Installation.Models.MetaData
{
    public class ISS070_DOEmailData_Meta : ISS070_DOEmailData
    {
        [NotNullOrEmpty(ControlName = "EmailAddress", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Notify email")]
        public string EmailAddress { get; set; }
    }
}
