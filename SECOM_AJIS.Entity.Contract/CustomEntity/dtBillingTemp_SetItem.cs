using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtBillingTemp_SetItem
    {
        public decimal? BillingContractFee { get; set; }
        public string BillingContractFeeCurrencyType { get; set; }

        public decimal? BillingInstallationFee { get; set; }
        public string BillingInstallationFeeCurrencyType { get; set; }
        public string PaymentInstallationFee { get; set; }

        public decimal? BillingApproveInstallationFee { get; set; }
        public string BillingApproveInstallationFeeCurrencyType { get; set; }

        public decimal? BillingCompleteInstallationFee { get; set; }
        public string BillingCompleteInstallationFeeCurrencyType { get; set; }
        public string PaymentCompleteInstallationFee { get; set; }

        public decimal? BillingStartInstallationFee { get; set; }
        public string BillingStartInstallationFeeCurrencyType { get; set; }
        public string PaymentStartInstallationFee { get; set; }

        public decimal? AmountTotal { get; set; }
        public decimal? AmountTotalUsd { get; set; }

        public decimal? BillingDepositFee { get; set; }
        public string BillingDepositFeeCurrencyType { get; set; }
        public string PaymentDepositFee { get; set; }

        public string UID { get; set; }
        public string OldOfficeCode { get; set; }

        public string BillingTargetCode { get; set; }
        public string BillingClientCode { get; set; }

        public string FullNameEN { get; set; }
        public string BranchNameEN { get; set; }
        public string AddressEN { get; set; }

        public string FullNameLC { get; set; }
        public string BranchNameLC { get; set; }
        public string AddressLC { get; set; }

        public string Nationality { get; set; }
        public string PhoneNo { get; set; }
        public string BusinessTypeCode { get; set; }
        public string BusinessType { get; set; }
        public string IDNo { get; set; }
        public string BillingOffice { get; set; }
        public string BillingOCC { get; set; }
        public string CustomerType { get; set; }
        public string RegionCode { get; set; }
        public string CompanyTypeCode { get; set; }
        public int SequenceNo { get; set; }

        public int ObjectType { get; set; }

        public string BillingOfficeCode { get; set; }
        public bool IsNew { get; set; }
        public bool HasUpdate { get; set; }

        // Addition for keep old grouping id
        public string OriginalBillingOCC { get; set; }
        public string OriginalBillingClientCode { get; set; }
        public string OriginalBillingOfficeCode { get; set; }
    }
}
