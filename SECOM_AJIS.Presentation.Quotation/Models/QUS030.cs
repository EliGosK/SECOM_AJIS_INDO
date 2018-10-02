using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Quotation.Models.MetaData;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master.MetaData;
using SECOM_AJIS.Presentation.Quotation.Models.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Quotation.Models
{
    /// <summary>
    /// DO for initial screen
    /// </summary>
    public class QUS030_ScreenParameter : ScreenParameter
    {
        public doInitQuotationData InitialData { get; set; }
        public doLinkageSaleContractData LinkageSaleContractData { get; set; }
        public List<doDefaultInstrument> DefaultInstrument { get; set; }
        public List<doDefaultFacility> DefaultFacility { get; set; }

        [KeepSession]
        public string ImportKey { get; set; }

        public doRegisterQuotationData RegisterData { get; set; }

        [KeepSession]
        public doQuotationKey QuotationKey { get; set; }
    }
    /// <summary>
    /// DO for initial quotation data
    /// </summary>
    public class QUS030_doInitQuotationData : doInitQuotationData
    {
        public bool IsProductTypeSale
        {
            get
            {
                if (doQuotationHeaderData != null)
                {
                    if (doQuotationHeaderData.doQuotationTarget != null)
                        return doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SALE;
                }
                return false;
            }
        }
        public bool IsProductTypeAL
        {
            get
            {
                if (doQuotationHeaderData != null)
                {
                    if (doQuotationHeaderData.doQuotationTarget != null)
                        return doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL;
                }
                return false;
            }
        }
        public bool IsProductTypeRentalSale
        {
            get
            {
                if (doQuotationHeaderData != null)
                {
                    if (doQuotationHeaderData.doQuotationTarget != null)
                        return doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_RENTAL_SALE;
                }
                return false;
            }
        }
        public bool IsProductSaleOnline
        {
            get
            {
                if (doQuotationHeaderData != null)
                {
                    if (doQuotationHeaderData.doQuotationTarget != null)
                        return doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_ONLINE;
                }
                return false;
            }
        }
        public bool IsProductBeatGuard
        {
            get
            {
                if (doQuotationHeaderData != null)
                {
                    if (doQuotationHeaderData.doQuotationTarget != null)
                        return doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_BE;
                }
                return false;
            }
        }
        public bool IsProductSentryGuard
        {
            get
            {
                if (doQuotationHeaderData != null)
                {
                    if (doQuotationHeaderData.doQuotationTarget != null)
                        return doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SG;
                }
                return false;
            }
        }
        public bool IsProductMaintenance
        {
            get
            {
                if (doQuotationHeaderData != null)
                {
                    if (doQuotationHeaderData.doQuotationTarget != null)
                        return doQuotationHeaderData.doQuotationTarget.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_MA;
                }
                return false;
            }
        }
        public bool IsShowInstrument01
        {
            get
            {
                if (doQuotationHeaderData != null)
                {
                    if (doQuotationHeaderData.doQuotationTarget != null)
                    {
                        if (doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE
                            || (doQuotationHeaderData.doQuotationTarget.TargetCodeTypeCode == SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType.C_TARGET_CODE_TYPE_CONTRACT_CODE
                                && FirstInstallCompleteFlag == false))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public bool IsEnableRegister { get; set; }
    }
    /// <summary>
    /// DO for quotation target data
    /// </summary>
    [MetadataType(typeof(QUS030_doQuotationTarget_Initial_MetaData))]
    public class QUS030_doQuotationTarget_Initial : doQuotationTarget
    {
    }
    /// <summary>
    /// DO for validating quotation data
    /// </summary>
    [MetadataType(typeof(QUS030_doQuotationDataCondition_MetaData))]
    public class QUS030_doGetQuotationDataCondition : doGetQuotationDataCondition
    {
    }
    /// <summary>
    /// DO for validating range of data
    /// </summary>
    public class QUS030_ValidateRangeData
    {
        [RangeNumberValue(0, 99999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSiteBuildingArea",
            ControlName = "SiteBuildingArea")]
        public Nullable<decimal> SiteBuildingArea { get; set; }
        [RangeNumberValue(0, 99999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSecurityAreaSizeFrom",
            ControlName = "SecurityAreaFrom")]
        public Nullable<decimal> SecurityAreaFrom { get; set; }
        [RangeNumberValue(0, 99999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSecurityAreaSizeTo",
            ControlName = "SecurityAreaTo")]
        public Nullable<decimal> SecurityAreaTo { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNewBuildingMgmtCost",
            ControlName = "NewBldMgmtCost")]
        public Nullable<decimal> NewBldMgmtCost { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblBidGuaranteeAmount1",
            ControlName = "BidGuaranteeAmount1")]
        public Nullable<decimal> BidGuaranteeAmount1 { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblBidGuaranteeAmount2",
            ControlName = "BidGuaranteeAmount2")]
        public Nullable<decimal> BidGuaranteeAmount2 { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNumberOfBuildings",
            ControlName = "NumOfBuilding")]
        public Nullable<int> NumOfBuilding { get; set; }
        [RangeNumberValue(0, 999,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNumberOfFloors",
            ControlName = "NumOfFloor")]
        public Nullable<int> NumOfFloor { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblInsuranceCoverageAmount",
            ControlName = "InsuranceCoverageAmount")]
        public Nullable<decimal> InsuranceCoverageAmount { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblMonthlyInsuranceFee",
            ControlName = "MonthlyInsuranceFee")]
        public Nullable<decimal> MonthlyInsuranceFee { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblOutsourcingFee",
            ControlName = "MaintenanceFee1")]
        public Nullable<decimal> MaintenanceFee1 { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblAdditionalContractFee1",
            ControlName = "AdditionalFee1")]
        public Nullable<decimal> AdditionalFee1 { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblAdditionalContractFee2",
            ControlName = "AdditionalFee2")]
        public Nullable<decimal> AdditionalFee2 { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblAdditionalContractFee3",
            ControlName = "AdditionalFee3")]
        public Nullable<decimal> AdditionalFee3 { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSecurityItemFee",
            ControlName = "SecurityItemFee")]
        public Nullable<decimal> SecurityItemFee { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblOtherItemFee",
            ControlName = "OtherItemFee")]
        public Nullable<decimal> OtherItemFee { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblWeekdayDayTime",
            ControlName = "NumOfDayTimeWd")]
        public Nullable<int> NumOfDayTimeWd { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblWeekdayNightTime",
            ControlName = "NumOfNightTimeWd")]
        public Nullable<int> NumOfNightTimeWd { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSaturdayDayTime",
            ControlName = "NumOfDayTimeSat")]
        public Nullable<int> NumOfDayTimeSat { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSaturdayNightTime",
            ControlName = "NumOfNightTimeSat")]
        public Nullable<int> NumOfNightTimeSat { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSundayDayTime",
            ControlName = "NumOfDayTimeSun")]
        public Nullable<int> NumOfDayTimeSun { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSundayNightTime",
            ControlName = "NumOfNightTimeSun")]
        public Nullable<int> NumOfNightTimeSun { get; set; }
        [RangeNumberValue(0, 999999,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNumerOfBeatGuardSteps",
            ControlName = "NumOfBeatStep")]
        public Nullable<int> NumOfBeatStep { get; set; }
        [RangeNumberValue(0, 999,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblFrequencyOfGateUsage",
            ControlName = "FreqOfGateUsage")]
        public Nullable<int> FreqOfGateUsage { get; set; }
        [RangeNumberValue(0, 999,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNumberOfClockKey",
            ControlName = "NumOfClockKey")]
        public Nullable<int> NumOfClockKey { get; set; }
        [TimeValue(
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNotifyTime",
            ControlName = "NotifyTime")]
        public Nullable<System.TimeSpan> NotifyTime { get; set; }
        [RangeNumberValue(1, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblProductPrice",
            ControlName = "ProductPrice")]
        public Nullable<decimal> ProductPrice { get; set; }
        [RangeNumberValue(1, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblContractFee",
            ControlName = "ContractFee")]
        public Nullable<decimal> ContractFee { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblInstallationFee",
            ControlName = "InstallationFee")]
        public Nullable<decimal> InstallationFee { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblDepositFee",
            ControlName = "DepositFee")]
        public Nullable<decimal> DepositFee { get; set; }
    }
    /// <summary>
    /// DO for validating range of data in case of alarm
    /// </summary>
    public class QUS030_ValidateRangeData_AL
    {
        [RangeNumberValue(0, 99999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSiteBuildingArea",
            ControlName = "SiteBuildingArea")]
        public Nullable<decimal> SiteBuildingArea { get; set; }
        [RangeNumberValue(0, 99999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSecurityAreaSizeFrom",
            ControlName = "SecurityAreaFrom")]
        public Nullable<decimal> SecurityAreaFrom { get; set; }
        [RangeNumberValue(0, 99999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSecurityAreaSizeTo",
            ControlName = "SecurityAreaTo")]
        public Nullable<decimal> SecurityAreaTo { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNewBuildingMgmtCost",
            ControlName = "NewBldMgmtCost")]
        public Nullable<decimal> NewBldMgmtCost { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblBidGuaranteeAmount1",
            ControlName = "BidGuaranteeAmount1")]
        public Nullable<decimal> BidGuaranteeAmount1 { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblBidGuaranteeAmount2",
            ControlName = "BidGuaranteeAmount2")]
        public Nullable<decimal> BidGuaranteeAmount2 { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNumberOfBuildings",
            ControlName = "NumOfBuilding")]
        public Nullable<int> NumOfBuilding { get; set; }
        [RangeNumberValue(0, 999,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNumberOfFloors",
            ControlName = "NumOfFloor")]
        public Nullable<int> NumOfFloor { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblInsuranceCoverageAmount",
            ControlName = "InsuranceCoverageAmount")]
        public Nullable<decimal> InsuranceCoverageAmount { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblMonthlyInsuranceFee",
            ControlName = "MonthlyInsuranceFee")]
        public Nullable<decimal> MonthlyInsuranceFee { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblOutsourcingFee",
            ControlName = "MaintenanceFee1")]
        public Nullable<decimal> MaintenanceFee1 { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblAdditionalContractFee1",
            ControlName = "AdditionalFee1")]
        public Nullable<decimal> AdditionalFee1 { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblAdditionalContractFee2",
            ControlName = "AdditionalFee2")]
        public Nullable<decimal> AdditionalFee2 { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblAdditionalContractFee3",
            ControlName = "AdditionalFee3")]
        public Nullable<decimal> AdditionalFee3 { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSecurityItemFee",
            ControlName = "SecurityItemFee")]
        public Nullable<decimal> SecurityItemFee { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblOtherItemFee",
            ControlName = "OtherItemFee")]
        public Nullable<decimal> OtherItemFee { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblWeekdayDayTime",
            ControlName = "NumOfDayTimeWd")]
        public Nullable<int> NumOfDayTimeWd { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblWeekdayNightTime",
            ControlName = "NumOfNightTimeWd")]
        public Nullable<int> NumOfNightTimeWd { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSaturdayDayTime",
            ControlName = "NumOfDayTimeSat")]
        public Nullable<int> NumOfDayTimeSat { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSaturdayNightTime",
            ControlName = "NumOfNightTimeSat")]
        public Nullable<int> NumOfNightTimeSat { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSundayDayTime",
            ControlName = "NumOfDayTimeSun")]
        public Nullable<int> NumOfDayTimeSun { get; set; }
        [RangeNumberValue(0, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSundayNightTime",
            ControlName = "NumOfNightTimeSun")]
        public Nullable<int> NumOfNightTimeSun { get; set; }
        [RangeNumberValue(0, 999999,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNumerOfBeatGuardSteps",
            ControlName = "NumOfBeatStep")]
        public Nullable<int> NumOfBeatStep { get; set; }
        [RangeNumberValue(0, 999,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblFrequencyOfGateUsage",
            ControlName = "FreqOfGateUsage")]
        public Nullable<int> FreqOfGateUsage { get; set; }
        [RangeNumberValue(0, 999,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNumberOfClockKey",
            ControlName = "NumOfClockKey")]
        public Nullable<int> NumOfClockKey { get; set; }
        [TimeValue(
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNotifyTime",
            ControlName = "NotifyTime")]
        public Nullable<System.TimeSpan> NotifyTime { get; set; }
        [RangeNumberValue(1, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblProductPrice",
            ControlName = "ProductPrice")]
        public Nullable<decimal> ProductPrice { get; set; }
        [RangeNumberValue(1, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblContractFee",
            ControlName = "ContractFee")]
        public Nullable<decimal> ContractFee { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblInstallationFee",
            ControlName = "InstallationFee")]
        public Nullable<decimal> InstallationFee { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblDepositFee",
            ControlName = "DepositFee")]
        public Nullable<decimal> DepositFee { get; set; }
    }
    /// <summary>
    /// DO for validating range of instrument data in case of sale, alarm (before 1st complete installation)
    /// </summary>
    public class QUS030_ValidateRangeData_Instrument1
    {
        [RangeNumberValue(0, 99999D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "headerQuantity",
            ControlName = "InstQuantity")]
        public Nullable<int> InstrumentQty { get; set; }
    }
    /// <summary>
    /// DO for validating range of instrument data in case of alarm (after 1st complete installation)
    /// </summary>
    public class QUS030_ValidateRangeData_Instrument2
    {
        [RangeNumberValue(0, 99999D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "headerBeforeQuantity")]
        public Nullable<int> InstrumentQty { get; set; }
        [RangeNumberValue(0, 99999D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "headerAdditionalQuantity")]
        public Nullable<int> AddQty { get; set; }
        [RangeNumberValue(0, 99999D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "headerRemovalQuantity")]
        public Nullable<int> RemoveQty { get; set; }
    }
    /// <summary>
    /// DO for validating range of facility data
    /// </summary>
    public class QUS030_ValidateRangeData_Facility
    {
        [RangeNumberValue(0, 99999D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "headerQuantity",
            ControlName = "FacQuantity")]
        public Nullable<int> InstrumentQty { get; set; }
    }
    /// <summary>
    /// DO for validating range of sentry guard data
    /// </summary>
    public class QUS030_ValidateRangeData_SentryGuard
    {
        [TimeValue(
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSecurityStartTime")]
        public Nullable<System.TimeSpan> SecurityStartTime { get; set; }
        [TimeValue(
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSecurityFinishTime")]
        public Nullable<System.TimeSpan> SecurityFinishTime { get; set; }
        [RangeNumberValue(0, 999999999999.99D,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblCostPerHour")]
        public Nullable<decimal> CostPerHour { get; set; }
        [RangeNumberValue(1, 99,
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblNumerOfSentryGuard")]
        public Nullable<int> NumOfSentryGuard { get; set; }
    }

    #region Import Data

    /// <summary>
    /// DO for validating quotation data
    /// </summary>
    [MetadataType(typeof(QUS030_ImportQuotationData_MetaData))]
    public class QUS030_ImportQuotationData : dsImportData
    {
    }
    /// <summary>
    /// DO for validating quotation basic data
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_ImportNull_MetaData))]
    public class QUS030_tbt_QuotationBasic_ImportNull : tbt_QuotationBasic
    {
    }
    /// <summary>
    /// DO for validating quotation basic data in case of alarm
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_AL_ImportNull_MetaData))]
    public class QUS030_tbt_QuotationBasic_AL_ImportNull : tbt_QuotationBasic
    {
    }
    /// <summary>
    /// DO for validating quotation basic data in case of rental sale
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_RENTAL_SALE_ImportNull_MetaData))]
    public class QUS030_tbt_QuotationBasic_RENTAL_SALE_ImportNull : tbt_QuotationBasic
    {
    }
    /// <summary>
    /// DO for validating quotation basic data in case of sale online
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_ONLINE_ImportNull_MetaData))]
    public class QUS030_tbt_QuotationBasic_ONLINE_ImportNull : tbt_QuotationBasic
    {
    }
    /// <summary>
    /// DO for validating quotation basic data in case of beat guard
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_BE_ImportNull_MetaData))]
    public class QUS030_tbt_QuotationBasic_BE_ImportNull : tbt_QuotationBasic
    {
    }
    /// <summary>
    /// DO for validating quotation basic data in case of sentry guard
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_SG_ImportNull_MetaData))]
    public class QUS030_tbt_QuotationBasic_SG_ImportNull : tbt_QuotationBasic
    {
    }
    /// <summary>
    /// DO for validating quotation basic data in case of maintenance
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_MA_ImportNull_MetaData))]
    public class QUS030_tbt_QuotationBasic_MA_ImportNull : tbt_QuotationBasic
    {
    }
    /// <summary>
    /// DO for validating beat guard detail data
    /// </summary>
    [MetadataType(typeof(QUS030_doBeatGuardDetail_ImportNull_MetaData))]
    public class QUS030_doBeatGuardDetail_ImportNull : doBeatGuardDetail
    {
    }

    #endregion
    #region Sale

    /// <summary>
    /// DO for initial quotation basic data in case of sale
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_MetaData))]
    public class QUS030_tbt_QuotationBasic : tbt_QuotationBasic
    {

        public bool CeilingTypeTBar { get; set; }
        public bool CeilingTypeSlabConcrete { get; set; }
        public bool CeilingTypeMBar { get; set; }
        public bool CeilingTypeSteel { get; set; }
        public bool CeilingTypeNone { get; set; }

        public bool? IsSelectedCeilingType
        {
            get
            {
                if (this.CeilingTypeTBar || this.CeilingTypeSlabConcrete 
                    || this.CeilingTypeMBar || this.CeilingTypeSteel || this.CeilingTypeNone)
                {
                    return true;
                }
                else
                {
                    return null;
                }
            }
        }
        public bool? IsSelectedCeilingTypeTBar { get { return this.IsSelectedCeilingType; } }
        public bool? IsSelectedCeilingTypeSlabConcrete { get { return this.IsSelectedCeilingType; } }
        public bool? IsSelectedCeilingTypeMBar { get { return this.IsSelectedCeilingType; } }
        public bool? IsSelectedCeilingTypeSteel { get { return this.IsSelectedCeilingType; } }
        public bool? IsSelectedCeilingTypeNone { get { return this.IsSelectedCeilingType; } }

        public decimal? CeilingHeight { get; set; }
        public bool SpecialInsPVC { get; set; }
        public bool SpecialInsSLN { get; set; }
        public bool SpecialInsProtector { get; set; }
        public bool SpecialInsEMT { get; set; }
        public bool SpecialInsPE { get; set; }
        public bool SpecialInsOther { get; set; }
        public string SpecialInsOtherText { get; set; }
        public bool? IsSelectedSpecialIns
        {
            get
            {
                if (this.SpecialInsPVC || this.SpecialInsSLN || this.SpecialInsProtector 
                    || this.SpecialInsEMT || this.SpecialInsPE || this.SpecialInsOther)
                {
                    return true;
                }
                else
                {
                    return null;
                }
            }
        }
        public bool? IsSelectedSpecialInsPVC { get { return this.IsSelectedSpecialIns; } }
        public bool? IsSelectedSpecialInsSLN { get { return this.IsSelectedSpecialIns; } }
        public bool? IsSelectedSpecialInsProtector { get { return this.IsSelectedSpecialIns; } }
        public bool? IsSelectedSpecialInsEMT { get { return this.IsSelectedSpecialIns; } }
        public bool? IsSelectedSpecialInsPE { get { return this.IsSelectedSpecialIns; } }
        public bool? IsSelectedSpecialInsOther { get { return this.IsSelectedSpecialIns; } }

    }
    /// <summary>
    /// DO for initial employee data in quotation basic in case of sale
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_Employee_MetaData))]
    public class QUS030_tbt_QuotationBasic_Employee : tbt_QuotationBasic
    {
    }

    #endregion
    #region AL

    /// <summary>
    /// DO for initial quotation basic data in case of alarm
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_AL_MetaData))]
    public class QUS030_tbt_QuotationBasic_AL : tbt_QuotationBasic
    {
        public List<string> OperationType { get; set; }
        public List<string> ServiceType { get; set; }
        
        public bool CeilingTypeTBar { get; set; }
        public bool CeilingTypeSlabConcrete { get; set; }
        public bool CeilingTypeMBar { get; set; }
        public bool CeilingTypeSteel { get; set; }
        public bool CeilingTypeNone { get; set; }

        public bool? IsSelectedCeilingType
        {
            get
            {
                if (this.CeilingTypeTBar || this.CeilingTypeSlabConcrete
                    || this.CeilingTypeMBar || this.CeilingTypeSteel || this.CeilingTypeNone)
                {
                    return true;
                }
                else
                {
                    return null;
                }
            }
        }
        public bool? IsSelectedCeilingTypeTBar { get { return this.IsSelectedCeilingType; } }
        public bool? IsSelectedCeilingTypeSlabConcrete { get { return this.IsSelectedCeilingType; } }
        public bool? IsSelectedCeilingTypeMBar { get { return this.IsSelectedCeilingType; } }
        public bool? IsSelectedCeilingTypeSteel { get { return this.IsSelectedCeilingType; } }
        public bool? IsSelectedCeilingTypeNone { get { return this.IsSelectedCeilingType; } }

        public decimal? CeilingHeight { get; set; }
        public bool SpecialInsPVC { get; set; }
        public bool SpecialInsSLN { get; set; }
        public bool SpecialInsProtector { get; set; }
        public bool SpecialInsEMT { get; set; }
        public bool SpecialInsPE { get; set; }
        public bool SpecialInsOther { get; set; }
        public string SpecialInsOtherText { get; set; }
        public bool? IsSelectedSpecialIns
        {
            get
            {
                if (this.SpecialInsPVC || this.SpecialInsSLN || this.SpecialInsProtector
                    || this.SpecialInsEMT || this.SpecialInsPE || this.SpecialInsOther)
                {
                    return true;
                }
                else
                {
                    return null;
                }
            }
        }
        public bool? IsSelectedSpecialInsPVC { get { return this.IsSelectedSpecialIns; } }
        public bool? IsSelectedSpecialInsSLN { get { return this.IsSelectedSpecialIns; } }
        public bool? IsSelectedSpecialInsProtector { get { return this.IsSelectedSpecialIns; } }
        public bool? IsSelectedSpecialInsEMT { get { return this.IsSelectedSpecialIns; } }
        public bool? IsSelectedSpecialInsPE { get { return this.IsSelectedSpecialIns; } }
        public bool? IsSelectedSpecialInsOther { get { return this.IsSelectedSpecialIns; } }

    }
    /// <summary>
    /// DO for initial employee data in quotation basic in case of alarm
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_AL_Employee_MetaData))]
    public class QUS030_tbt_QuotationBasic_AL_Employee : tbt_QuotationBasic
    {
    }

    #endregion
    #region Sale Online

    /// <summary>
    /// DO for initial quotation basic data in case of sale online
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_ONLINE_MetaData))]
    public class QUS030_tbt_QuotationBasic_ONLINE : tbt_QuotationBasic
    {
        public List<string> OperationType { get; set; }
        public List<string> ServiceType { get; set; }
    }
    /// <summary>
    /// DO for initial employee data in quotation basic in case of sale online
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_ONLINE_Employee_MetaData))]
    public class QUS030_tbt_QuotationBasic_ONLINE_Employee : tbt_QuotationBasic
    {
    }

    #endregion
    #region Beat Guard

    /// <summary>
    /// DO for initial quotation basic data in case of beat guard
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_BE_MetaData))]
    public class QUS030_tbt_QuotationBasic_BE : tbt_QuotationBasic
    {
    }
    /// <summary>
    /// DO for initial employee data in quotation basic in case of beat guard
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_BE_Employee_MetaData))]
    public class QUS030_tbt_QuotationBasic_BE_Employee : tbt_QuotationBasic
    {
    }

    #endregion
    #region Sentry Guard

    /// <summary>
    /// DO for initial quotation basic data in case of sentry guard
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_SG_MetaData))]
    public class QUS030_tbt_QuotationBasic_SG : tbt_QuotationBasic
    {
        public bool IsEditMode { get; set; }
    }
    /// <summary>
    /// DO for initial employee data in quotation basic in case of sentry guard
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_SG_Employee_MetaData))]
    public class QUS030_tbt_QuotationBasic_SG_Employee : tbt_QuotationBasic
    {
    }
    /// <summary>
    /// DO for validating sentry guard detail data
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationSentryGuardDetail_Import_MetaData))]
    public class QUS030_tbt_QuotationSentryGuardDetail_Import : tbt_QuotationSentryGuardDetails
    {
        public string SentryGuardTypeName { get; set; }
        public string NumOfDateValue { get; set; }
    }

    #endregion
    #region Maintenance

    /// <summary>
    /// DO for initial quotation basic data in case of maintenance
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_MA_MetaData))]
    public class QUS030_tbt_QuotationBasic_MA : tbt_QuotationBasic
    {
    }
    /// <summary>
    /// DO for initial employee data in quotation basic in case of maintenance
    /// </summary>
    [MetadataType(typeof(QUS030_tbt_QuotationBasic_MA_Employee_MetaData))]
    public class QUS030_tbt_QuotationBasic_MA_Employee : tbt_QuotationBasic
    {
    }

    #endregion

    /// <summary>
    /// DO for instrument data
    /// </summary>
    public class QUS030_tbt_QuotationInstrumentDetails : tbt_QuotationInstrumentDetails
    {
        public bool IsDefaultFlag { get; set; }
        public bool SaleFlag { get; set; }
        public bool RentalFlag { get; set; }
        public string LineUpTypeCode { get; set; }

    }
    /// <summary>
    /// DO for getting instrument data
    /// </summary>
    public class QUS030_GetInstrumentDataCondition
    {
        public string InstrumentCode { get; set; }

        [NotNullOrEmpty(Module = MessageUtil.MODULE_QUOTATION,
                        MessageCode = MessageUtil.MessageList.MSG2048,
                        ControlName = "ProductCode")]
        public string ProductCode { get; set; }

        public bool IsAskQuestion { get; set; }
    }
    /// <summary>
    /// DO for validating instrument data in case add new instrument
    /// </summary>
    public class QUS030_AddInstrumentData
    {
        [NotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0081,
                        ControlName = "InstrumentCode")]
        public string InstrumentCode { get; set; }
        public int? InstrumentQty { get; set; }
        public List<doInstrumentDetail> InstrumentList { get; set; }
        [NotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0082,
                        ControlName = "InstrumentCode")]
        public doInstrumentData doInstrumentData { get; set; }
    }
    /// <summary>
    /// DO for getting facility data
    /// </summary>
    public class QUS030_GetFacilityDataCondition
    {
        public string FacilityCode { get; set; }
        [NotNullOrEmpty(Module = MessageUtil.MODULE_QUOTATION,
                        MessageCode = MessageUtil.MessageList.MSG2048,
                        ControlName = "ProductCode")]
        public string ProductCode { get; set; }
        public bool IsAskQuestion { get; set; }
    }
    /// <summary>
    /// DO for validating facility data in case add new facility
    /// </summary>
    public class QUS030_AddFacilityData
    {
        [NotNullOrEmpty(Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0022,
                        ControlName = "FacilityCode")]
        public string FacilityCode { get; set; }
        public int? FacilityQty { get; set; }
        public string ProductTypeCode { get; set; }
        public List<doInstrumentDetail> FacilityList { get; set; }
        [NotNullOrEmpty(Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0023,
                        ControlName = "FacilityCode")]
        public doInstrumentData doFacilityDetail { get; set; }
    }
    /// <summary>
    /// DO for getting linkage sale contract data
    /// </summary>
    public class QUS030_GetLinkageSaleContractCondition
    {
        public string QuotationTargetCode { get; set; }

        [NotNullOrEmpty(Module = MessageUtil.MODULE_QUOTATION,
                        MessageCode = MessageUtil.MessageList.MSG2057,
                        ControlName = "SaleOnlineContractCode")]
        public string SaleOnlineContractCode { get; set; }
    }
    /// <summary>
    /// DO for validating maintenance data
    /// </summary>
    public class QUS030_AddMaintenanceDetailDataCondition
    {
        [NotNullOrEmpty(Module = MessageUtil.MODULE_QUOTATION,
                        MessageCode = MessageUtil.MessageList.MSG2043,
                        ControlName = "MaintenanceTargetContractCode")]
        public string MaintenanceTargetContractCode { get; set; }
        public List<View_doContractHeader> MaintenanceList { get; set; }
    }
    /// <summary>
    /// DO for validating sentry guard data
    /// </summary>
    [MetadataType(typeof(QUS030_doSentryGuardDetail_MetaData))]
    public class QUS030_AddSentryGuardDetail : doSentryGuardDetail
    {
        public int UpdateMode { get; set; }
    }
    /// <summary>
    /// DO for validating beat guard detail data
    /// </summary>
    [MetadataType(typeof(QUS030_doBeatGuardDetail_MetaData))]
    public class QUS030_doBeatGuardDetail : doBeatGuardDetail
    {

    }
}
namespace SECOM_AJIS.Presentation.Quotation.Models.CustomAttribute
{
    /// <summary>
    /// Attribute for checking target code type code is correct
    /// </summary>
    public class TargetCodeTypeQuotationCorrectAttribute : AValidatorAttribute
    {
        public TargetCodeTypeQuotationCorrectAttribute()
        {
            this.Module = MessageUtil.MODULE_QUOTATION;
            this.MessageCode = MessageUtil.MessageList.MSG2060;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value) == false)
            {
                doQuotationTarget qt = validationContext.ObjectInstance as doQuotationTarget;
                if (qt != null)
                {
                    if (value.ToString() == TargetCodeType.C_TARGET_CODE_TYPE_QTN_CODE
                        && qt.ContractTransferStatus == ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_APP)
                        return base.IsValid(value, validationContext);
                }
            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for checking cintract transfer status is correct
    /// </summary>
    public class ContractTransferStatusCorrectAttribute : AValidatorAttribute
    {
        public ContractTransferStatusCorrectAttribute()
        {
            this.Module = MessageUtil.MODULE_QUOTATION;
            this.MessageCode = MessageUtil.MessageList.MSG2061;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value) == false)
            {
                if (value.ToString() == ContractTransferStatus.C_CONTRACT_TRANS_STATUS_CONTRACT_CAN)
                    return base.IsValid(value, validationContext);
            }

            return null;
        }
    }
}
namespace SECOM_AJIS.Presentation.Quotation.Models.MetaData
{
    /// <summary>
    /// Metadata for QUS030_doQuotationTarget_Initial DO
    /// </summary>
    public class QUS030_doQuotationTarget_Initial_MetaData
    {
        [OfficeMapping("QuotationOfficeName")]
        public string QuotationOfficeCode { get; set; }
        [OfficeMapping("OperationOfficeName")]
        public string OperationOfficeCode { get; set; }
        [AcquisitionTypeMapping("AcquisitionTypeName")]
        public string AcquisitionTypeCode { get; set; }
        [MotivationTypeMapping("MotivationTypeName")]
        public string MotivationTypeCode { get; set; }
        [EmployeeMapping("QuotationStaffName")]
        public string QuotationStaffEmpNo { get; set; }
        [TargetCodeTypeQuotationCorrect]
        public string TargetCodeTypeCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_doQuotationDataCondition DO
    /// </summary>
    public class QUS030_doQuotationDataCondition_MetaData
    {
        [NotNullOrEmpty(
            Module = MessageUtil.MODULE_QUOTATION,
            MessageCode = MessageUtil.MessageList.MSG2058,
            ControlName = "QuotationTargetCodeShort")]
        public string QuotationTargetCode { get; set; }
    }

    #region Import Data

    /// <summary>
    /// Metadata for QUS030_ImportQuotationData DO
    /// </summary>
    public class QUS030_ImportQuotationData_MetaData
    {
        [NotNullOrEmpty(ControlName = "QuotationTargetCode")]
        public string QuotationTargetCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_ImportNull DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_ImportNull_MetaData
    {
        [CodeHasValue("ProductName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 1)]
        public string ProductCode { get; set; }
        [CodeHasValue("MainStructureTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMainStructureType",
                        ControlName = "MainStructureTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 2)]
        public string MainStructureTypeCode { get; set; }
        [CodeHasValue("BuildingTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNewOldBuilding",
                        ControlName = "BuildingTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 3)]
        public string BuildingTypeCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_AL_ImportNull DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_AL_ImportNull_MetaData
    {
        [CodeHasValue("ProductName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 1)]
        public string ProductCode { get; set; }
        [CodeHasValue("DispatchTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblDispatchType",
                        ControlName = "DispatchTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 2)]
        public string DispatchTypeCode { get; set; }
        [CodeHasValue("PhoneLineTypeName1",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNormalLineType",
                        ControlName = "PhoneLineTypeCode1",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 3)]
        public string PhoneLineTypeCode1 { get; set; }
        [CodeHasValue("PhoneLineOwnerTypeName1",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNormalLineOwnerType",
                        ControlName = "PhoneLineOwnerTypeCode1",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 4)]
        public string PhoneLineOwnerTypeCode1 { get; set; }
        [CodeHasValue("PhoneLineTypeName2",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblImageLineType",
                        ControlName = "PhoneLineTypeCode2",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 5)]
        public string PhoneLineTypeCode2 { get; set; }
        [CodeHasValue("PhoneLineOwnerTypeName2",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblImageLineOwnerType",
                        ControlName = "PhoneLineOwnerTypeCode2",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 6)]
        public string PhoneLineOwnerTypeCode2 { get; set; }
        [CodeHasValue("PhoneLineTypeName3",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblDisconnectLineType",
                        ControlName = "PhoneLineTypeCode3",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 7)]
        public string PhoneLineTypeCode3 { get; set; }
        [CodeHasValue("PhoneLineOwnerTypeName3",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblDisconnectLineOwnerType",
                        ControlName = "PhoneLineOwnerTypeCode3",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 8)]
        public string PhoneLineOwnerTypeCode3 { get; set; }
        [CodeHasValue("MainStructureTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMainStructureType",
                        ControlName = "MainStructureTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 9)]
        public string MainStructureTypeCode { get; set; }
        [CodeHasValue("BuildingTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNewOldBuilding",
                        ControlName = "BuildingTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 10)]
        public string BuildingTypeCode { get; set; }
        [CodeHasValue("MaintenanceCycleName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMaintenaceCycle",
                        ControlName = "MaintenanceCycle",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 11)]
        public Nullable<int> MaintenanceCycle { get; set; }
        [CodeHasValue("InsuranceTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblInsuranceType",
                        ControlName = "InsuranceTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 12)]
        public string InsuranceTypeCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_RENTAL_SALE_ImportNull DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_RENTAL_SALE_ImportNull_MetaData
    {
        [CodeHasValue("ProductName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 1)]
        public string ProductCode { get; set; }
        [CodeHasValue("PhoneLineTypeName1",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNormalLineType",
                        ControlName = "PhoneLineTypeCode1",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 3)]
        public string PhoneLineTypeCode1 { get; set; }
        [CodeHasValue("PhoneLineOwnerTypeName1",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNormalLineOwnerType",
                        ControlName = "PhoneLineOwnerTypeCode1",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 4)]
        public string PhoneLineOwnerTypeCode1 { get; set; }
        [CodeHasValue("PhoneLineTypeName2",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblImageLineType",
                        ControlName = "PhoneLineTypeCode2",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 5)]
        public string PhoneLineTypeCode2 { get; set; }
        [CodeHasValue("PhoneLineOwnerTypeName2",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblImageLineOwnerType",
                        ControlName = "PhoneLineOwnerTypeCode2",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 6)]
        public string PhoneLineOwnerTypeCode2 { get; set; }
        [CodeHasValue("PhoneLineTypeName3",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblDisconnectLineType",
                        ControlName = "PhoneLineTypeCode3",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 7)]
        public string PhoneLineTypeCode3 { get; set; }
        [CodeHasValue("PhoneLineOwnerTypeName3",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblDisconnectLineOwnerType",
                        ControlName = "PhoneLineOwnerTypeCode3",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 8)]
        public string PhoneLineOwnerTypeCode3 { get; set; }
        [CodeHasValue("MainStructureTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMainStructureType",
                        ControlName = "MainStructureTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 9)]
        public string MainStructureTypeCode { get; set; }
        [CodeHasValue("BuildingTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNewOldBuilding",
                        ControlName = "BuildingTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 10)]
        public string BuildingTypeCode { get; set; }
        [CodeHasValue("MaintenanceCycleName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMaintenaceCycle",
                        ControlName = "MaintenanceCycle",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 11)]
        public Nullable<int> MaintenanceCycle { get; set; }
        [CodeHasValue("InsuranceTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblInsuranceType",
                        ControlName = "InsuranceTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 12)]
        public string InsuranceTypeCode { get; set; }
    } 
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_ONLINE_ImportNull DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_ONLINE_ImportNull_MetaData
    {
        [CodeHasValue("ProductName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 1)]
        public string ProductCode { get; set; }
        [CodeHasValue("DispatchTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblDispatchType",
                        ControlName = "DispatchTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 2)]
        public string DispatchTypeCode { get; set; }
        [CodeHasValue("PhoneLineTypeName1",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNormalLineType",
                        ControlName = "PhoneLineTypeCode1",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 3)]
        public string PhoneLineTypeCode1 { get; set; }
        [CodeHasValue("PhoneLineOwnerTypeName1",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNormalLineOwnerType",
                        ControlName = "PhoneLineOwnerTypeCode1",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 4)]
        public string PhoneLineOwnerTypeCode1 { get; set; }
        [CodeHasValue("PhoneLineTypeName2",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblImageLineType",
                        ControlName = "PhoneLineTypeCode2",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 5)]
        public string PhoneLineTypeCode2 { get; set; }
        [CodeHasValue("PhoneLineOwnerTypeName2",
                       Controller = MessageUtil.MODULE_QUOTATION,
                       Screen = "QUS030",
                       Parameter = "lblImageLineOwnerType",
                       ControlName = "PhoneLineOwnerTypeCode2",
                       Module = MessageUtil.MODULE_COMMON,
                       MessageCode = MessageUtil.MessageList.MSG0066,
                       Order = 6)]
        public string PhoneLineOwnerTypeCode2 { get; set; }
        [CodeHasValue("PhoneLineTypeName3",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblDisconnectLineType",
                        ControlName = "PhoneLineTypeCode3",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 7)]
        public string PhoneLineTypeCode3 { get; set; }
        [CodeHasValue("PhoneLineOwnerTypeName3",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblDisconnectLineOwnerType",
                        ControlName = "PhoneLineOwnerTypeCode3",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 8)]
        public string PhoneLineOwnerTypeCode3 { get; set; }
        [CodeHasValue("MaintenanceCycleName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMaintenaceCycle",
                        ControlName = "MaintenanceCycle",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 9)]
        public Nullable<int> MaintenanceCycle { get; set; }
        [CodeHasValue("InsuranceTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblInsuranceType",
                        ControlName = "InsuranceTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 10)]
        public string InsuranceTypeCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_BE_ImportNull DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_BE_ImportNull_MetaData
    {
        [CodeHasValue("ProductName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066)]
        public string ProductCode { get; set; }

    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_SG_ImportNull DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_SG_ImportNull_MetaData
    {
        [CodeHasValue("ProductName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 1)]
        public string ProductCode { get; set; }
        [CodeHasValue("SentryGuardAreaTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSentryGuardAreaType",
                        ControlName = "SentryGuardAreaTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 2)]
        public string SentryGuardAreaTypeCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_MA_ImportNull DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_MA_ImportNull_MetaData
    {
        [CodeHasValue("ProductName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 1)]
        public string ProductCode { get; set; }
        [CodeHasValue("MaintenanceTargetProductTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMaintenanceTargetProduct",
                        ControlName = "MaintenanceTargetProductTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 2)]
        public string MaintenanceTargetProductTypeCode { get; set; }
        [CodeHasValue("MaintenanceTypeName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMaintenanceType",
                        ControlName = "MaintenanceTypeCode",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 3)]
        public string MaintenanceTypeCode { get; set; }
        [CodeHasValue("MaintenanceCycleName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMaintenaceCycle",
                        ControlName = "MaintenanceCycle",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066,
                        Order = 4)]
        public Nullable<int> MaintenanceCycle { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_doBeatGuardDetail_ImportNull DO
    /// </summary>
    public class QUS030_doBeatGuardDetail_ImportNull_MetaData
    {
        [CodeHasValue("NumOfDateName",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNumberOfDate",
                        ControlName = "NumOfDate",
                        Module = MessageUtil.MODULE_COMMON,
                        MessageCode = MessageUtil.MessageList.MSG0066)]
        public Nullable<decimal> NumOfDate { get; set; }
    }

    #endregion
    #region Sale

    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Order = 1)]
        public string ProductCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblPlanCode",
                        ControlName = "PlanCode",
                        Order = 2)]
        public string PlanCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblPlanner",
                        ControlName = "PlannerEmpNo",
                        Order = 3)]
        public string PlannerEmpNo { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblPlanChecker",
                        ControlName = "PlanCheckerEmpNo",
                        Order = 4)]
        public string PlanCheckerEmpNo { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblPlanApprover",
                        ControlName = "PlanApproverEmpNo",
                        Order = 5)]
        public string PlanApproverEmpNo { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNewOldBuilding",
                        ControlName = "BuildingTypeCode",
                        Order = 6)]
        public string BuildingTypeCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSaleman1",
                        ControlName = "SalesmanEmpNo1",
                        Order = 8)]
        public string SalesmanEmpNo1 { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductPrice",
                        ControlName = "ProductPrice",
                        Order = 10)]
        public string ProductPrice { get; set; }

        [RelateObject("SecurityAreaTo",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSecurityAreaSizeFrom",
                        ControlName = "SecurityAreaFrom",
                        RelateParameter = "lblSecurityAreaSizeTo",
                        RelateControlName = "SecurityAreaTo",
                        Order = 5)]
        public Nullable<decimal> SecurityAreaFrom { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblCeilingTypeTBar", ControlName = "chkCeilingTypeTBar", Order = 11)]
        public bool? IsSelectedCeilingTypeTBar { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblCeilingTypeSlabConcrete", ControlName = "chkCeilingTypeSlabConcrete", Order = 11)]
        public bool? IsSelectedCeilingTypeSlabConcrete { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblCeilingTypeMBar", ControlName = "chkCeilingTypeMBar", Order = 11)]
        public bool? IsSelectedCeilingTypeMBar { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblCeilingTypeSteel", ControlName = "chkCeilingTypeSteel", Order = 11)]
        public bool? IsSelectedCeilingTypeSteel { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblCeilingTypeNone", ControlName = "chkCeilingTypeNone", Order = 11)]
        public bool? IsSelectedCeilingTypeNone { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsPVC", ControlName = "chkSpecialInsPVC", Order = 11)]
        public bool? IsSelectedSpecialInsPVC { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsSLN", ControlName = "chkSpecialInsSLN", Order = 11)]
        public bool? IsSelectedSpecialInsSLN { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsProtector", ControlName = "chkSpecialInsProtector", Order = 11)]
        public bool? IsSelectedSpecialInsProtector { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsEMT", ControlName = "chkSpecialInsEMT", Order = 11)]
        public bool? IsSelectedSpecialInsEMT { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsPE", ControlName = "chkSpecialInsPE", Order = 11)]
        public bool? IsSelectedSpecialInsPE { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsOther", ControlName = "chkSpecialInsOther", Order = 11)]
        public bool? IsSelectedSpecialInsOther { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_Employee DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_Employee_MetaData
    {
        [EmployeeExist(Control = "PlannerEmpNo")]
        public string PlannerEmpNo { get; set; }
        [EmployeeExist(Control = "PlanCheckerEmpNo")]
        public string PlanCheckerEmpNo { get; set; }
        [EmployeeExist(Control = "PlanApproverEmpNo")]
        public string PlanApproverEmpNo { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo2")]
        public string SalesmanEmpNo2 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo3")]
        public string SalesmanEmpNo3 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo4")]
        public string SalesmanEmpNo4 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo5")]
        public string SalesmanEmpNo5 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo6")]
        public string SalesmanEmpNo6 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo7")]
        public string SalesmanEmpNo7 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo8")]
        public string SalesmanEmpNo8 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo9")]
        public string SalesmanEmpNo9 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo10")]
        public string SalesmanEmpNo10 { get; set; }
    }

    #endregion
    #region AL

    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_AL DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_AL_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Order = 1)]
        public string ProductCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblPlanCode",
                        ControlName = "PlanCode",
                        Order = 9)]
        public string PlanCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblPlanner",
                        ControlName = "PlannerEmpNo",
                        Order = 10)]
        public string PlannerEmpNo { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblPlanChecker",
                        ControlName = "PlanCheckerEmpNo",
                        Order = 11)]
        public string PlanCheckerEmpNo { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblPlanCheckingDate",
                        ControlName = "PlanCheckDate",
                        Order = 12)]
        public Nullable<System.DateTime> PlanCheckDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblPlanApprover",
                        ControlName = "PlanApproverEmpNo",
                        Order = 13)]
        public string PlanApproverEmpNo { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblPlanApprovingDate",
                        ControlName = "PlanApproveDate",
                        Order = 14)]
        public Nullable<System.DateTime> PlanApproveDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSiteBuildingArea",
                        ControlName = "SiteBuildingArea",
                        Order = 15)]
        public Nullable<decimal> SiteBuildingArea { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSecurityAreaSizeFrom",
                        ControlName = "SecurityAreaFrom",
                        Order = 16)]
        [RelateObject("SecurityAreaTo",
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSecurityAreaSizeFrom",
                        ControlName = "SecurityAreaFrom",
                        RelateParameter = "lblSecurityAreaSizeTo",
                        RelateControlName = "SecurityAreaTo",
                        Order = 17)]
        public Nullable<decimal> SecurityAreaFrom { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSecurityAreaSizeTo",
                        ControlName = "SecurityAreaTo",
                        Order = 18)]
        public Nullable<decimal> SecurityAreaTo { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMainStructureType",
                        ControlName = "MainStructureTypeCode",
                        Order = 19)]
        public string MainStructureTypeCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNewOldBuilding",
                        ControlName = "BuildingTypeCode",
                        Order = 20)]
        public string BuildingTypeCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMaintenaceCycle",
                        ControlName = "MaintenanceCycle",
                        Order = 22)]
        public Nullable<int> MaintenanceCycle { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNumberOfBuildings",
                        ControlName = "NumOfBuilding",
                        Order = 23)]
        public Nullable<int> NumOfBuilding { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNumberOfFloors",
                        ControlName = "NumOfFloor",
                        Order = 24)]
        public Nullable<int> NumOfFloor { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSaleman1",
                        ControlName = "SalesmanEmpNo1",
                        Order = 25)]
        public string SalesmanEmpNo1 { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblContractFee",
                        ControlName = "ContractFee",
                        Order = 30)]
        public Nullable<decimal> ContractFee { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblCeilingTypeTBar", ControlName = "chkCeilingTypeTBar", Order = 11)]
        public bool? IsSelectedCeilingTypeTBar { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblCeilingTypeSlabConcrete", ControlName = "chkCeilingTypeSlabConcrete", Order = 11)]
        public bool? IsSelectedCeilingTypeSlabConcrete { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblCeilingTypeMBar", ControlName = "chkCeilingTypeMBar", Order = 11)]
        public bool? IsSelectedCeilingTypeMBar { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblCeilingTypeSteel", ControlName = "chkCeilingTypeSteel", Order = 11)]
        public bool? IsSelectedCeilingTypeSteel { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblCeilingTypeNone", ControlName = "chkCeilingTypeNone", Order = 11)]
        public bool? IsSelectedCeilingTypeNone { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsPVC", ControlName = "chkSpecialInsPVC", Order = 11)]
        public bool? IsSelectedSpecialInsPVC { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsSLN", ControlName = "chkSpecialInsSLN", Order = 11)]
        public bool? IsSelectedSpecialInsSLN { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsProtector", ControlName = "chkSpecialInsProtector", Order = 11)]
        public bool? IsSelectedSpecialInsProtector { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsEMT", ControlName = "chkSpecialInsEMT", Order = 11)]
        public bool? IsSelectedSpecialInsEMT { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsPE", ControlName = "chkSpecialInsPE", Order = 11)]
        public bool? IsSelectedSpecialInsPE { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION, Screen = "QUS030", Parameter = "lblSpecialInsOther", ControlName = "chkSpecialInsOther", Order = 11)]
        public bool? IsSelectedSpecialInsOther { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_AL_Employee DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_AL_Employee_MetaData
    {
        [EmployeeExist(Control = "PlannerEmpNo")]
        public string PlannerEmpNo { get; set; }
        [EmployeeExist(Control = "PlanCheckerEmpNo")]
        public string PlanCheckerEmpNo { get; set; }
        [EmployeeExist(Control = "PlanApproverEmpNo")]
        public string PlanApproverEmpNo { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo2")]
        public string SalesmanEmpNo2 { get; set; }
        [EmployeeExist(Control = "SalesSupporterEmpNo")]
        public string SalesSupporterEmpNo { get; set; }
    }

    #endregion
    #region Sale Online

    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_ONLINE DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_ONLINE_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Order = 1)]
        public string ProductCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblDispatchType",
                        ControlName = "DispatchTypeCode",
                        Order = 2)]
        public string DispatchTypeCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblContractCode",
                        ControlName = "SaleOnlineContractCode",
                        Order = 9)]
        public string SaleOnlineContractCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblMaintenaceCycle",
                        ControlName = "MaintenanceCycle",
                        Order = 10)]
        public Nullable<int> MaintenanceCycle { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNumberOfBuildings",
                        ControlName = "NumOfBuilding",
                        Order = 11)]
        public Nullable<int> NumOfBuilding { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNumberOfFloors",
                        ControlName = "NumOfFloor",
                        Order = 12)]
        public Nullable<int> NumOfFloor { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSaleman1",
                        ControlName = "SalesmanEmpNo1",
                        Order = 13)]
        public string SalesmanEmpNo1 { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblContractFee",
                        ControlName = "ContractFee",
                        Order = 18)]
        public Nullable<decimal> ContractFee { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_ONLINE_Employee DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_ONLINE_Employee_MetaData
    {
        [EmployeeExist(Control = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo2")]
        public string SalesmanEmpNo2 { get; set; }
        [EmployeeExist(Control = "SalesSupporterEmpNo")]
        public string SalesSupporterEmpNo { get; set; }
    }

    #endregion
    #region Beat Guard

    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_BE DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_BE_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Order = 1)]
        public string ProductCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSaleman1",
                        ControlName = "SalesmanEmpNo1",
                        Order = 2)]
        public string SalesmanEmpNo1 { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblContractFee",
                        ControlName = "ContractFee",
                        Order = 15)]
        public Nullable<decimal> ContractFee { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_BE_Employee DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_BE_Employee_MetaData
    {
        [EmployeeExist(Control = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo2")]
        public string SalesmanEmpNo2 { get; set; }
        [EmployeeExist(Control = "SalesSupporterEmpNo")]
        public string SalesSupporterEmpNo { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_doBeatGuardDetail DO
    /// </summary>
    public class QUS030_doBeatGuardDetail_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblWeekdayDayTime",
                        ControlName = "NumOfDayTimeWd",
                        Order = 7)]
        public Nullable<int> NumOfDayTimeWd { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblWeekdayNightTime",
                        ControlName = "NumOfNightTimeWd",
                        Order = 8)]
        public Nullable<int> NumOfNightTimeWd { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSaturdayDayTime",
                        ControlName = "NumOfDayTimeSat",
                        Order = 9)]
        public Nullable<int> NumOfDayTimeSat { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSaturdayNightTime",
                        ControlName = "NumOfNightTimeSat",
                        Order = 10)]
        public Nullable<int> NumOfNightTimeSat { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSundayDayTime",
                        ControlName = "NumOfDayTimeSun",
                        Order = 11)]
        public Nullable<int> NumOfDayTimeSun { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSundayNightTime",
                        ControlName = "NumOfNightTimeSun",
                        Order = 12)]
        public Nullable<int> NumOfNightTimeSun { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNumerOfBeatGuardSteps",
                        ControlName = "NumOfBeatStep",
                        Order = 13)]
        public Nullable<int> NumOfBeatStep { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNumberOfDate",
                        ControlName = "NumOfDate",
                        Order = 14)]
        public Nullable<decimal> NumOfDate { get; set; }
    }

    #endregion
    #region Sentry Guard

    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_SG DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_SG_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblProductCode",
                        ControlName = "ProductCode",
                        Order = 1)]
        public string ProductCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSaleman1",
                        ControlName = "SalesmanEmpNo1",
                        Order = 2)]
        public string SalesmanEmpNo1 { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSecurityItemFee",
                        ControlName = "SecurityItemFee",
                        Order = 6)]
        public Nullable<decimal> SecurityItemFee { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblOtherItemFee",
                        ControlName = "OtherItemFee",
                        Order = 7)]
        public Nullable<decimal> OtherItemFee { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblContractFee",
                        ControlName = "ContractFee",
                        Order = 9)]
        public Nullable<decimal> ContractFee { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_SG_Employee DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_SG_Employee_MetaData
    {
        [EmployeeExist(Control = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo2")]
        public string SalesmanEmpNo2 { get; set; }
        [EmployeeExist(Control = "SalesSupporterEmpNo")]
        public string SalesSupporterEmpNo { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationSentryGuardDetail_Import DO
    /// </summary>
    public class QUS030_tbt_QuotationSentryGuardDetail_Import_MetaData
    {
        [SentryGuardTypeMapping("SentryGuardTypeName")]
        public string SentryGuardTypeCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_doSentryGuardDetail DO
    /// </summary>
    public class QUS030_doSentryGuardDetail_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSentryGuardType",
                        ControlName = "SentryGuardTypeCode")]
        public string SentryGuardTypeCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNumberOfDate",
                        ControlName = "NumOfDate")]
        public decimal? NumOfDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSecurityStartTime",
                        ControlName = "SecurityStartTime")]
        public TimeSpan? SecurityStartTime { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblSecurityFinishTime",
                        ControlName = "SecurityFinishTime")]
        public TimeSpan? SecurityFinishTime { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblCostPerHour",
                        ControlName = "CostPerHour")]
        public decimal? CostPerHour { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS030",
                        Parameter = "lblNumerOfSentryGuard",
                        ControlName = "NumOfSentryGuard")]
        public int? NumOfSentryGuard { get; set; }
    }

    #endregion
    #region Maintenance

    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_MA DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_MA_MetaData
    {
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblProductCode",
            Order = 1,
            ControlName = "ProductCode")]
        public string ProductCode { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblSaleman1",
            Order = 2,
            ControlName = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblMaintenanceTargetProduct",
            Order = 7,
            ControlName = "MaintenanceTargetProductTypeCode")]
        public string MaintenanceTargetProductTypeCode { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblMaintenaceCycle",
            Order = 10,
            ControlName = "MaintenanceCycle")]
        public Nullable<int> MaintenanceCycle { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS030",
            Parameter = "lblContractFee",
            Order = 11,
            ControlName = "ContractFee")]
        public Nullable<decimal> ContractFee { get; set; }
    }
    /// <summary>
    /// Metadata for QUS030_tbt_QuotationBasic_MA_Employee DO
    /// </summary>
    public class QUS030_tbt_QuotationBasic_MA_Employee_MetaData
    {
        [EmployeeExist(Control = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo2")]
        public string SalesmanEmpNo2 { get; set; }
        [EmployeeExist(Control = "SalesSupporterEmpNo")]
        public string SalesSupporterEmpNo { get; set; }
    }

    #endregion
}
