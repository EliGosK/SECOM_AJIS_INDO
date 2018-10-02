using System;
using System.Configuration;
using System.Reflection;

using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Common.Util
{
    /// <summary>
    /// Constant management
    /// </summary>
    public class ConstantUtil
    {
        #region Constants

        private const string CONSTANT_FILE = "Content\\config\\ConstantValues.config";
        private const string ROOT_NODE = "constant.classes/";

        #endregion
        #region Statice Methods

        /// <summary>
        /// Initial all constants object
        /// </summary>
        public static void InitialConstants()
        {
            try
            {
                ConstantUtil util = new ConstantUtil();


                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DepartmentMaster));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.TransferType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.StockInType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.LogisticFunction));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.RegionCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.NationCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PurchaseOrderType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstrumentArea));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InventoryConfig));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.RegisterAssetFlag));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARPermissionType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.RentalInstallationType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InteractionTypeCorrespondent));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InteractionTypeMax));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ProjectOwnerType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InteractionTypeAdministrator));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InteractionTypeChief));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CommonValue));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.FunctionID));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CustPartType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.TargetCodeType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ContractTransferStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ScreenID));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DistributeType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.LockStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ServiceType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.Quotation));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.LockStyle));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.MiscType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.FlagType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ProductType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.MaintenanceTargetProductType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.FunctionQuotation));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.FunctionSecurity));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InteractionTypeControlChief));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CustomerStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CustomerType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.NameCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ActionType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ConfigName));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.LogMessage));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.TransactionType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.TableName));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ContractStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstrumentType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DocumentType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DocumentCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.FunctionSale));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.FunctionBilling));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.FunctionLogistic));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.BatchStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ContractType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CustomerCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SiteCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ProjectCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.EventType));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.AttachDocumentCondition));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.AcquisitionType));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CustRoleType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.RentalChangeType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.RentalImplementType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ApprovalStatus));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.RelationType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARSearchDuedate));
                
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SaleChangeType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SaleProcessManageStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.LogType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARInteractionType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARApproveNo));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARNo));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARRelevant));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentNo));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentRelevant));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.BillingTemp));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.StartType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.BillingType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.BillingTiming));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.LineUpType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ExpansionType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.OCCType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.GenerateMAProcessType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ReceivedRate));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ProjectStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InsuranceType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SaleInstallationType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.MethodType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ContractEmailType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.RegisterMAProcessType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.MAFeeType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.RelatedContractType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.AddressFull));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentRole));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentSearchDuedate));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentSearchStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentSearchPeriod));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentPermissionType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentSummaryPeriod));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ProcAfterCounterBalanceType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.HandlingType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PaymentMethod));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CalculationDailyFeeType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.GroupCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARRole));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARSearchStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARSearchPeriod));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CompanyType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARSummaryPeriod));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ContractDocStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DocAuditResult));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ParticularOCC));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.EmailTemplateName));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DateType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ContractPrefix));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstallationMANo));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.AutoTransferBillingType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.BankTransferBillingType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstallationSlipNo));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstallationStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstallationManagementStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InventoryHeadOffice));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ModuleID));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.Department));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.EmailSenderName));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentInteractionType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstallFeeBillingType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.StockOutType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SlipStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.LastChangeType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CustomerReason));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstallationBy));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstallChangeReason));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DocLanguage));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ARTitle));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ContractBillingType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SlipID));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ShelfType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstrumentLocation));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.OfficeCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.OfficeName));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ShelfNo));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PurchaseOrderStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.AdjustmentType));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SaleProcessType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InventorySlipStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.BillingTypeGroup));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SendToBillingTiming));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IssueInv));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IssueInvTime));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IssueInvDate));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.LotNo));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InvFormatType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SigType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ShowDueDate));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IssueRecieptTime));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ShowBankAccType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ShowIssueDate));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SeparateInvType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PaymentMethodType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PaymentType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.BillingCycle));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CreditTerm));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PaymentStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InventoryAccountCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DeductType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CheckingStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.BillingServiceTypeCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ReportID));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.FunctionDebtTracing));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ValidationImportResult));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.AutoTransferResult));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.AutoTransferResultWord));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.StopBillingFlag));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.BillingInvFormatType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.AdvanceReceiptStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InventoryCheckingSlipStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.Issued));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.Paid));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SpecialCareful));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DepositStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CurrencyType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.RunningType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PaymentSystemMethod));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.UsedPaymentMethod));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PositionCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PickingListNo));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.AdjustType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstallationBeforeChangeReason));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstallationBeforeChangeRequester));                
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PaymentMatchingStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PaymentStatusSearch));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.BillingIncomeDocPrefix));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ProcessID));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PaymentMatchingDesc));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ChangeReasonType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InvoiceDocument));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.NotifyType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncidentReceivedMethod));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CompleteInstallFlag));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.OCC));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ReceiptDocument));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InvoiceType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CorrectionReason));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.ReportDocLanguage));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PermissionType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InvoiceProcessType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InstallationComplain));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.GroupProductType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.IncomeMiscWord));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.FlagDisplay));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CreditNoteType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CancelReceiptTarget));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.RentalCoverLetterDocCode));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.OfficeDummy));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.EventID));

                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.PrintingFlag)); //Add by Jutarat A. on 17092013
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.InventoryStartType));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DebtTracingSubStatus));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DebtTracingResult));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.EncashedFlag));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.SecomAccountID));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.DebitOutPutTax));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.CreditOutPutTax));
                util.LoadProperties(typeof(SECOM_AJIS.Common.Util.ConstantValue.AccountingConfig));
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Methods

        /// <summary>
        /// Mapping constants value in xml to constant object
        /// </summary>
        /// <param name="theClass"></param>
        private void LoadProperties(Type theClass)
        {
            string path = CommonUtil.WebPath + CONSTANT_FILE;

            ExeConfigurationFileMap configFile = new ExeConfigurationFileMap();
            string pathConstant = ConfigurationManager.AppSettings["ConstantFile"];
            if (System.IO.File.Exists(pathConstant))
                configFile.ExeConfigFilename = pathConstant;
            else
                configFile.ExeConfigFilename = path;
            //configFile.ExeConfigFilename = "d:\\ConstantValues.config";
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
            AppSettingsSection section = config.GetSection(ROOT_NODE + theClass.FullName) as AppSettingsSection;

            PropertyInfo[] props = theClass.GetProperties();
            foreach (PropertyInfo p in props)
            {
                KeyValueConfigurationElement e = section.Settings[p.Name];
                if (null != e)
                {
                    string v = e.Value;
                    string typeName = p.PropertyType.FullName;

                    if ("" == v)
                        p.SetValue(null, null, null);
                    else if (typeName.Contains(typeof(string).Name))
                    {
                        p.SetValue(null, v, null);
                    }
                    else if (typeName.Contains(typeof(Int16).Name))
                    {
                        p.SetValue(null, Int16.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Int32).Name))
                    {
                        p.SetValue(null, Int32.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Int64).Name))
                    {
                        p.SetValue(null, Int64.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Single).Name))
                    {
                        p.SetValue(null, Single.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Double).Name))
                    {
                        p.SetValue(null, Double.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Decimal).Name))
                    {
                        p.SetValue(null, Decimal.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(DateTime).Name))
                    {
                        try
                        {
                            p.SetValue(null, DateTime.ParseExact(v, "d/m/yyyy", null), null);
                            continue;
                        }
                        catch { }

                        try
                        {
                            p.SetValue(null, DateTime.ParseExact(v, "yyyy/m/d", null), null);
                            continue;
                        }
                        catch { }

                        p.SetValue(null, DateTime.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Byte).Name))
                    {
                        p.SetValue(null, Byte.Parse(v), null);
                    }
                    else if (typeName.Contains(typeof(Boolean).Name))
                    {
                        p.SetValue(null, Boolean.Parse(v), null);
                    }
                }
            }
        }

        #endregion
    }
}

