using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Common.CustomAttribute
{
    /// <summary>
    /// Abstract for misc type object
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public abstract class AMiscTypeMappingAttribute : Attribute
    {
        public string MiscTypeKey { get; protected set; }
        public string MiscTypeNameField { get; set; }
        public bool SetEachLanguage { get; set; }

        public AMiscTypeMappingAttribute(string MiscTypeNameField)
        {
            this.MiscTypeNameField = MiscTypeNameField;
            this.SetEachLanguage = false;
        }
    }
    /// <summary>
    /// Attribute for mapping main structure type data
    /// </summary>
    public class MainStructureTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public MainStructureTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_MAIN_STRUCTURE_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping contract transfer status data
    /// </summary>
    public class ContractTransferStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public ContractTransferStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_CONTRACT_TRANS_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping building type data
    /// </summary>
    public class BuildingTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public BuildingTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_BUILDING_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping dispatch type data
    /// </summary>
    public class DispatchTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public DispatchTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_DISPATCH_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping phone line type data
    /// </summary>
    public class PhoneLineTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public PhoneLineTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_PHONE_LINE_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping phone line owner type data
    /// </summary>
    public class PhoneLineOwnerTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public PhoneLineOwnerTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_PHONE_LINE_OWNER_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping insurance type data
    /// </summary>
    public class InsuranceTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public InsuranceTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_INSURANCE_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping sentry guard area type data
    /// </summary>
    public class SentryGuardAreaTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public SentryGuardAreaTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SG_AREA_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping maintenance target product type data
    /// </summary>
    public class MaintenanceTargetProductTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public MaintenanceTargetProductTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_MA_TARGET_PROD_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping maintenance type data
    /// </summary>
    public class MaintenanceTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public MaintenanceTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_MA_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping maintenance cycle data
    /// </summary>
    public class MaintenanceCycleMappingAttribute : AMiscTypeMappingAttribute
    {
        public MaintenanceCycleMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_MA_CYCLE;
        }
    }
    /// <summary>
    /// Attribute for mapping line-up type data
    /// </summary>
    public class LineUpTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public LineUpTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_LINE_UP_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping num of date data
    /// </summary>
    public class NumOfDateMappingAttribute : AMiscTypeMappingAttribute
    {
        public NumOfDateMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_NUM_OF_DATE;
        }
    }
    /// <summary>
    /// Attribute for mapping acquisition type data
    /// </summary>
    public class AcquisitionTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public AcquisitionTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_ACQUISITION_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping motivation type data
    /// </summary>
    public class MotivationTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public MotivationTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_MOTIVATION_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping sentry guard type data
    /// </summary>
    public class SentryGuardTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public SentryGuardTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SG_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping cust type data
    /// </summary>
    public class CustTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public CustTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_CUST_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping financecial market type data
    /// </summary>
    public class FinancialMarketTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public FinancialMarketTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_FINANCIAL_MARKET_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping address full data
    /// </summary>
    public class AddressFullMappingAttribute : AMiscTypeMappingAttribute
    {
        public AddressFullMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_ADDRESS_FULL;
        }
    }
    /// <summary>
    /// Attribute for mapping sale change type data
    /// </summary>
    public class SaleChangeTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public SaleChangeTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SALE_CHANGE_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping rental change type data
    /// </summary>
    public class RentalChangeTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public RentalChangeTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_RENTAL_CHANGE_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping process after counter balance type data
    /// </summary>
    public class ProcessAfterCounterBalanceTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public ProcessAfterCounterBalanceTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_PROC_AFT_COUNTER_BALANCE_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping billing type data
    /// </summary>
    public class BillingTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public BillingTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_BILLING_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping contract billing type data
    /// </summary>
    public class ContractBillingTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public ContractBillingTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_CONTRACT_BILLING_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping handling type data
    /// </summary>
    public class HandlingTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public HandlingTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_HANDLING_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping batch status data
    /// </summary>
    public class BatchStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public BatchStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_BATCH_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping cust status data
    /// </summary>
    public class CustStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public CustStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_CUST_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping approval status data
    /// </summary>
    public class ApprovalStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public ApprovalStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_APPROVE_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping doc audit result data
    /// </summary>
    public class DocAuditResultMappingAttribute : AMiscTypeMappingAttribute
    {
        public DocAuditResultMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_DOC_AUDIT_RESULT;
        }
    }
    /// <summary>
    /// Attribute for mapping project status data
    /// </summary>
    public class ProjectStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public ProjectStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_PROJECT_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping dead line time type data
    /// </summary>
    public class DeadLineTimeTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public DeadLineTimeTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_DEADLINE_TIME_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping lock status type data
    /// </summary>
    public class LockStatusTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public LockStatusTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_LOCK_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping doc language data
    /// </summary>
    public class DocLanguageMappingAttribute : AMiscTypeMappingAttribute
    {
        public DocLanguageMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_DOC_LANGUAGE;
        }
    }
    /// <summary>
    /// Attribute for mapping purchase order status data
    /// </summary>
    public class PurchaseOrderStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public PurchaseOrderStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_PURCHASE_ORDER_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping transport type data
    /// </summary>
    public class TransportTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public TransportTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_TRANSPORT_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping instrument area data
    /// </summary>
    public class InstrumentAreaMappingAttribute : AMiscTypeMappingAttribute
    {
        public InstrumentAreaMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_INV_AREA;
        }
    }
    /// <summary>
    /// Attribute for mapping issue inv time data
    /// </summary>
    public class IssueInvTimeMappingAttribute : AMiscTypeMappingAttribute
    {
        public IssueInvTimeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_ISSUE_INV_TIME;
        }
    }
    /// <summary>
    /// Attribute for mapping inv format type data
    /// </summary>
    public class InvFormatTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public InvFormatTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_INV_FORMAT;
        }
    }
    /// <summary>
    /// Attribute for mapping sig type data
    /// </summary>
    public class SigTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public SigTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SIG_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping show due date data
    /// </summary>
    public class ShowDueDateMappingAttribute : AMiscTypeMappingAttribute
    {
        public ShowDueDateMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SHOW_DUEDATE;
        }
    }
    /// <summary>
    /// Attribute for mapping issue receipt time data
    /// </summary>
    public class IssueRecieptTimeMappingAttribute : AMiscTypeMappingAttribute
    {
        public IssueRecieptTimeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_ISSUE_REC_TIME;
        }
    }
    /// <summary>
    /// Attribute for mapping show bank acc type data
    /// </summary>
    public class ShowBankAccTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public ShowBankAccTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SHOW_BANK_ACC;
        }
    }
    /// <summary>
    /// Attribute for mapping deduct type data
    /// </summary>
    public class DeductTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public DeductTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_DEDUCT_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping show issue date data
    /// </summary>
    public class ShowIssueDateMappingAttribute : AMiscTypeMappingAttribute
    {
        public ShowIssueDateMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SHOW_ISSUE_DATE;
        }
    }
    /// <summary>
    /// Attribute for mapping separate inv type data
    /// </summary>
    public class SeparateInvTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public SeparateInvTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SEP_INV;
        }
    }
    /// <summary>
    /// Attribute for mapping account type data
    /// </summary>
    public class AccountTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public AccountTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_ACCOUNT_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping auto transfer result data
    /// </summary>
    public class AutoTransferResultMappingAttribute : AMiscTypeMappingAttribute
    {
        public AutoTransferResultMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SHOW_AUTO_TRANSFER_RESULT;
        }
    }
    /// <summary>
    /// Attribute for mapping stop billing flag data
    /// </summary>
    public class StopBillingFlagMappingAttribute : AMiscTypeMappingAttribute
    {
        public StopBillingFlagMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_BILLING_FLAG;
        }
    }
    /// <summary>
    /// Attribute for mapping payment method data
    /// </summary>
    public class PaymentMethodMappingAttribute : AMiscTypeMappingAttribute
    {
        public PaymentMethodMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_PAYMENT_METHOD;
        }
    }
    /// <summary>
    /// Attribute for mapping credit card type data
    /// </summary>
    public class CreditCardTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public CreditCardTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_CREDIT_CARD_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping deposit status data
    /// </summary>
    public class DepositStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public DepositStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_DEPOSIT_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping payment status data
    /// </summary>
    public class PaymentStatuaMappingAttribute : AMiscTypeMappingAttribute
    {
        public PaymentStatuaMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_PAYMENT_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping slip status data
    /// </summary>
    public class SlipStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public SlipStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SLIP_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping management status data
    /// </summary>
    public class ManagementStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public ManagementStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_INSTALL_MANAGE_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping install fee billing type data
    /// </summary>
    public class InstallFeeBillingTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public InstallFeeBillingTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_INSTALL_FEE_BILLING_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping installation status data
    /// </summary>
    public class InstallationStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public InstallationStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_INSTALL_STATUS;
        }
    }

    /// <summary>
    /// Attribute for mapping SubContractor data
    /// </summary>
    public class SubContractorMappingAttribute : AMiscTypeMappingAttribute
    {
        public SubContractorMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SUB_CONTRATOR;
        }
    }
    /// <summary>
    /// Attribute for mapping installation by data
    /// </summary>
    public class InstallationByMappingAttribute : AMiscTypeMappingAttribute
    {
        public InstallationByMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_INSTALLATION_BY;
        }
    }
    /// <summary>
    /// Attribute for mapping install rental type data
    /// </summary>
    public class InstallRentalTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public InstallRentalTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_RENTAL_INSTALL_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping install sale type data
    /// </summary>
    public class InstallSaleTypeMappingAttribute : AMiscTypeMappingAttribute
    {
        public InstallSaleTypeMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SALE_INSTALL_TYPE;
        }
    }
    /// <summary>
    /// Attribute for mapping cause reason customer data
    /// </summary>
    public class CauseReasonCustomerMappingAttribute : AMiscTypeMappingAttribute
    {
        public CauseReasonCustomerMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_CUSTOMER_REASON;
        }
    }
    /// <summary>
    /// Attribute for mapping cause reason secom data
    /// </summary>
    public class CauseReasonSecomMappingAttribute : AMiscTypeMappingAttribute
    {
        public CauseReasonSecomMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_SECOM_REASON;
        }
    }
    /// <summary>
    /// Attribute for mapping instrument location data
    /// </summary>
    public class InstrumentLocationMappingAttribute : AMiscTypeMappingAttribute
    {
        public InstrumentLocationMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_INV_LOC;
        }
    }
    /// <summary>
    /// Attribute for mapping payment status data
    /// </summary>
    public class PaymentStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public PaymentStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_PAYMENT_STATUS;
        }
    }
    /// <summary>
    /// Attribute for mapping issue invoice data
    /// </summary>
    public class IssueInvoiceMappingAttribute : AMiscTypeMappingAttribute
    {
        public IssueInvoiceMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_ISSUE_INVOICE;
        }
    }
    /// <summary>
    /// Attribute for mapping payment data
    /// </summary>
    public class PaymentMappingAttribute : AMiscTypeMappingAttribute
    {
        public PaymentMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_ISSUE_PAYMENT;
        }
    }
    /// <summary>
    /// Attribute for mapping sub-installation flag data
    /// </summary>
    public class SubInstallationFlagMappingAttribute : AMiscTypeMappingAttribute
    {
        public SubInstallationFlagMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_FLAG_DISPLAY;
        }
    }
    /// <summary>
    /// Attribute for mapping sub-maintenance flag data
    /// </summary>
    public class SubMaintenanceFlagMappingAttribute : AMiscTypeMappingAttribute
    {
        public SubMaintenanceFlagMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_FLAG_DISPLAY;
        }
    }
    /// <summary>
    /// Attribute for mapping cal daily fee status data
    /// </summary>
    public class CalDailyFeeStatusMappingAttribute : AMiscTypeMappingAttribute
    {
        public CalDailyFeeStatusMappingAttribute(string MiscTypeNameField)
            : base(MiscTypeNameField)
        {
            this.MiscTypeKey = MiscType.C_CALC_DAILY_FEE_TYPE;
        }
    }

}
