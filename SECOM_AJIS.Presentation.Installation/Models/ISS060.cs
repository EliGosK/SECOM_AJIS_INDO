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
    public class ISS060_ValidateData
    {       

    }
    /// <summary>
    /// DO of data from screen
    /// </summary>
    public class ISS060_ScreenInput
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS060",
                        Parameter = "Installation start date",
                        ControlName = "InstallStartDate")]
        public string InstallStartDate { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS060",
                        Parameter = "Installation finish date",
                        ControlName = "InstallFinishDate")]
        public string InstallFinishDate { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS060",
                        Parameter = "Installation Complete Date",
                        ControlName = "InstallCompleteDate")]
        public string InstallCompleteDate { get; set; }
        public Decimal? NewNormalContractFee { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "ISS060",
                        Parameter = "Normal Installation Fee",
                        ControlName = "NewNormalInstallFee")]
        public string NewNormalInstallFee { get; set; }

        public string UnremoveApproveNo { get; set; }
        public string NewInstallFeeBillingType { get; set; }
        public Decimal? NewBillingInstallFee { get; set; }
        public string NewBillingOCC { get; set; }
        public Decimal? NewSECOMPaymentFee { get; set; }
        public Decimal? NewSECOMRevenueFee { get; set; }
        public string NewApproveNo1 { get; set; }
        public string NewApproveNo2 { get; set; }
        public string NewMemo { get; set; }
        public bool chkHaveUnremove { get; set; }
        public bool chkHaveAdditional { get; set; }
        public string AdditionalStockOutOfficeCode { get; set; }

        public Decimal? NormalContractFee { get; set; }
        public Decimal? NormalContractFeeUsd { get; set; }
        public string NormalContractFeeCurrencyType { get; set; }
        public Decimal? NormalInstallFee { get; set; }
        public Decimal? NormalInstallFeeUsd { get; set; }
        public string NormalInstallFeeCurrencyType { get; set; }
    }
    /// <summary>
    /// DO for send data to show in screen
    /// </summary>
    public class ISS060_RegisterStartResumeTargetData  ///// FOR SENT VALUE TO SCREEN
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
        public List<ISS060_DOEmailData> ListDOEmail { get; set; }
        public string LastSlipStatusName { get; set; }
        public bool blnValidateContractError { get; set; }
        public string ContractCodeShort { get; set; }
        public string Memo { get; set; }
        public string ContractProjectCodeForShow { get; set; }
    } 
    /// <summary>
    /// DO of session parameter of screen ISS060
    /// </summary>
    public class ISS060_ScreenParameter : ScreenSearchParameter
    {        
        public ISS060_DOEmailData DOEmail { get; set; }
        public List<ISS060_DOEmailData> ListDOEmail { get; set; }
        public tbt_InstallationBasic do_TbtInstallationBasic { get; set; }
        public tbt_InstallationSlip do_TbtInstallationSlip { get; set; }
        public List<tbt_InstallationInstrumentDetails> do_TbtInstallationInstrumentDetail { get; set; }
        public List<tbt_InstallationSlipDetails> do_TbtInstallationSlipDetails { get; set; }
        public List<doInstrumentData> ListInstrumentData { get; set; }
        [KeepSession]
        public dtRentalContractBasicForInstall dtRentalContractBasic { get; set; }
        [KeepSession]
        public dtSaleBasic dtSale { get; set; }
        [KeepSession]
        public string ServiceTypeCode { get; set; }
        [KeepSession]
        public string ContractCodeShort { get; set; }
        [KeepSession]
        public string ContractCodeLong { get; set; }
        [KeepSession]
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
        public List<ISS060_GridInstrumentData> GridInstrumentForValid { get; set; }
        [KeepSession]
        public string strContractCode { get; set; }
        public string Memo { get; set; }
        [KeepSession]
        public string ContractProjectCodeForShow { get; set; }
        public DateTime? TbtInstallationBasicUpdateDate { get; set; }
        public bool IsUseContractData { get; set; } //Add by Jutarat A. on 11042013
        public List<tbt_RentalSecurityBasic> do_TbtRentalSecurityBasicLastUnimp { get; set; } //Add by Jutarat A. on 11042013
    }

    [MetadataType(typeof(ISS060_DOEmailData_Meta))]
    public partial class ISS060_DOEmailData : dtEmailAddress
    {
    }

    /// <summary>
    /// DO of tbt_installationslipdetails
    /// </summary>
    public partial class ISS060_GridInstrumentData : tbt_InstallationSlipDetails
    {
        public int AddRemovedQTYTemp { get; set; }
        public int ContractInstalledAfterChange { get; set; }
    }
    /// <summary>
    /// DO for get instrument data condition
    /// </summary>
    public class ISS060_GetInstrumentDataCondition
    {
        public string InstrumentCode { get; set; }

        
        public string ProductCode { get; set; }

        public bool IsAskQuestion { get; set; }
    }



}

namespace SECOM_AJIS.Presentation.Installation.Models.MetaData
{
    public class ISS060_DOEmailData_Meta : ISS060_DOEmailData
    {
        [NotNullOrEmpty(ControlName = "EmailAddress", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Notify email")]
        public string EmailAddress { get; set; }
    }
}
