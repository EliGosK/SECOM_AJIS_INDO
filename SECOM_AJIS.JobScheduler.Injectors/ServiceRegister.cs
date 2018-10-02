using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Common;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace SECOM_AJIS.JobScheduler.Injectors
{
    public static class ServiceRegister
    {
        private static void RegisterServiceSingleton<Tinterface, Tclass>(this WindsorContainer TheContainer, params Type[] intercepters)
            where Tinterface : class
            where Tclass : Tinterface
        {
            TheContainer.Register(Component.For<Tinterface>().ImplementedBy<Tclass>().Interceptors(intercepters).LifeStyle.Singleton);
        }

        private static void RegisterServiceSingleton<Tinterface>(this WindsorContainer TheContainer, Type implementedby, params Type[] intercepters)
            where Tinterface : class
        {
            TheContainer.Register(Component.For<Tinterface>().ImplementedBy(implementedby).Interceptors(intercepters).LifeStyle.Singleton);
        }

        private static void RegisterServiceTransient<Tinterface, Tclass>(this WindsorContainer TheContainer, params Type[] intercepters)
            where Tinterface : class
            where Tclass : Tinterface
        {
            TheContainer.Register(Component.For<Tinterface>().ImplementedBy<Tclass>().Interceptors(intercepters).LifeStyle.Transient);
        }

        private static void RegisterServiceTransient<Tinterface>(this WindsorContainer TheContainer, Type implementedby, params Type[] intercepters)
            where Tinterface : class
        {
            TheContainer.Register(Component.For<Tinterface>().ImplementedBy(implementedby).Interceptors(intercepters).LifeStyle.Transient);
        }

        public static void Initial()
        {
            ServiceContainer.Init();

            var asmCommon = Assembly.GetAssembly(typeof(SECOM_AJIS.DataEntity.Common.BizCMDataEntities));
            var asmBL = Assembly.GetAssembly(typeof(SECOM_AJIS.DataEntity.Billing.BizBLDataEntities));
            var asmCT = Assembly.GetAssembly(typeof(SECOM_AJIS.DataEntity.Contract.BizCTDataEntities));
            var asmIC = Assembly.GetAssembly(typeof(SECOM_AJIS.DataEntity.Income.BizICDataEntities));
            var asmIS = Assembly.GetAssembly(typeof(SECOM_AJIS.DataEntity.Installation.BizISDataEntities));
            var asmIV = Assembly.GetAssembly(typeof(SECOM_AJIS.DataEntity.Inventory.BizIVDataEntities));
            var asmMAS = Assembly.GetAssembly(typeof(SECOM_AJIS.DataEntity.Master.BizMADataEntities));

            //ServiceContainer.Container.RegisterServiceTransient<IBatchProcessHandler, BatchProcessHandler>();
            //ServiceContainer.Container.RegisterServiceTransient<ILogHandler>(asmCommon.GetType("SECOM_AJIS.DataEntity.Common.LogHandler"));
            
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Common.IBatchProcessHandler, SECOM_AJIS.DataEntity.Common.BatchProcessHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Common.ICommonHandler, SECOM_AJIS.DataEntity.Common.CommonHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Common.ILogHandler, SECOM_AJIS.DataEntity.Common.LogHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Common.IDocumentHandler, SECOM_AJIS.DataEntity.Common.DocumentHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Common.ITransDataHandler>(asmCommon.GetType("SECOM_AJIS.DataEntity.Common.TransDataHandler"));
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Common.ILoginHandler>(asmCommon.GetType("SECOM_AJIS.DataEntity.Common.LoginHandler"));
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IAutoCompleteHandler, SECOM_AJIS.DataEntity.Master.AutoCompleteHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IBillingMasterHandler, SECOM_AJIS.DataEntity.Master.BillingMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IBillingClientMasterHandler, SECOM_AJIS.DataEntity.Master.BillingClientMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.ICustomerMasterHandler, SECOM_AJIS.DataEntity.Master.CustomerMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IEmployeeMasterHandler, SECOM_AJIS.DataEntity.Master.EmployeeMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IGroupMasterHandler, SECOM_AJIS.DataEntity.Master.GroupMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IInstrumentMasterHandler, SECOM_AJIS.DataEntity.Master.InstrumentMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IMasterHandler, SECOM_AJIS.DataEntity.Master.MasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IOfficeMasterHandler, SECOM_AJIS.DataEntity.Master.OfficeMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IPermissionMasterHandler, SECOM_AJIS.DataEntity.Master.PermissionMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.ISiteMasterHandler, SECOM_AJIS.DataEntity.Master.SiteMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.ISupplierMasterHandler, SECOM_AJIS.DataEntity.Master.SupplierMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IUserHandler, SECOM_AJIS.DataEntity.Master.UserHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IProductMasterHandler, SECOM_AJIS.DataEntity.Master.ProductMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.ISubcontractorMasterHandler, SECOM_AJIS.DataEntity.Master.SubcontractorMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.ISafetyStockMasterHandler, SECOM_AJIS.DataEntity.Master.SafetyStockMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Master.IShelfMasterHandler, SECOM_AJIS.DataEntity.Master.ShelfMasterHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IApprovalPermissionHandler, SECOM_AJIS.DataEntity.Contract.ApprovalPermissionHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IARHandler, SECOM_AJIS.DataEntity.Contract.ARHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IBillingInterfaceHandler, SECOM_AJIS.DataEntity.Contract.BillingInterfaceHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IBillingTempHandler, SECOM_AJIS.DataEntity.Contract.BillingTempHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.ICommonContractHandler, SECOM_AJIS.DataEntity.Contract.CommonContractHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IContractDocumentHandler, SECOM_AJIS.DataEntity.Contract.ContractDocumentHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IContractHandler, SECOM_AJIS.DataEntity.Contract.ContractHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IDraftContractHandler, SECOM_AJIS.DataEntity.Contract.DraftContractHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IDraftRentalContractHandler, SECOM_AJIS.DataEntity.Contract.DraftRentalContractHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IDraftSaleContractHandler, SECOM_AJIS.DataEntity.Contract.DraftSaleContractHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IIncidentHandler, SECOM_AJIS.DataEntity.Contract.IncidentHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IMaintenanceHandler, SECOM_AJIS.DataEntity.Contract.MaintenanceHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IProjectHandler, SECOM_AJIS.DataEntity.Contract.ProjectHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IRentralContractHandler, SECOM_AJIS.DataEntity.Contract.RentralContractHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IReportHandler, SECOM_AJIS.DataEntity.Contract.ReportHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.ISaleContractHandler, SECOM_AJIS.DataEntity.Contract.SaleContractHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IUserControlHandler, SECOM_AJIS.DataEntity.Contract.UserControlHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Contract.IViewContractHandler, SECOM_AJIS.DataEntity.Contract.ViewContractHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Quotation.IQuotationHandler, SECOM_AJIS.DataEntity.Quotation.QuotationHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Installation.IInstallationHandler>(asmIS.GetType("SECOM_AJIS.DataEntity.Installation.InstallationHandler"));
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Installation.IReportHandler, SECOM_AJIS.DataEntity.Installation.ReportHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Installation.IInstallationDocumentHandler>(asmIS.GetType("SECOM_AJIS.DataEntity.Installation.InstallationDocumentHandler"));
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Inventory.IInventoryHandler>(asmIV.GetType("SECOM_AJIS.DataEntity.Inventory.InventoryHandler"));
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Inventory.IInventoryDocumentHandler>(asmIV.GetType("SECOM_AJIS.DataEntity.Inventory.InventoryDocumentHandler"));
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Billing.IBillingHandler, SECOM_AJIS.DataEntity.Billing.BillingHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Billing.IViewBillingHandler, SECOM_AJIS.DataEntity.Billing.ViewBillingHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Billing.IBillingDocumentHandler, SECOM_AJIS.DataEntity.Billing.BillingDocumentHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Income.IIncomeHandler, SECOM_AJIS.DataEntity.Income.IncomeHandler>();
            ServiceContainer.Container.RegisterServiceTransient<SECOM_AJIS.DataEntity.Income.IIncomeDocumentHandler, SECOM_AJIS.DataEntity.Income.IncomeDocumentHandler>();


        }

    }
}
