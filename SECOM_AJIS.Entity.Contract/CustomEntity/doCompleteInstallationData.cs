using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doCompleteInstallationData
    {
        public virtual string ContractCode { get; set; }
        public virtual string OCC { get; set; }
        public virtual string ServiceTypeCode { get; set; }
        public virtual string InstallationType { get; set; }
        public virtual decimal? NormalInstallationFee { get; set; }
        public virtual decimal? BillingInstallationFee { get; set; }
        public virtual string BillingOCC { get; set; }
        public virtual string InstallationSlipNo { get; set; }
        public virtual DateTime InstallationCompleteDate { get; set; }
        public virtual string InstallationMemo { get; set; }
        public virtual string IEInchargeEmpNo { get; set; }
        public virtual DateTime InstallationCompleteProcessDate { get; set; }
        public virtual bool CompleteInstallationProcessFlag { get; set; }
        public virtual decimal? SECOMPaymentFee { get; set; }
        public virtual decimal? SECOMRevenueFee { get; set; }

        private List<doInstrumentDetails> _doInstrumentDetails;
        public List<doInstrumentDetails> doInstrumentDetailsList { get { return this._doInstrumentDetails; } set { this._doInstrumentDetails = value; } }

        private List<doSubcontractorDetails> _doSubcontractorDetails;
        public List<doSubcontractorDetails> doSubcontractorDetailsList { get { return this._doSubcontractorDetails; } set { this._doSubcontractorDetails = value; } }
    }

    [MetadataType(typeof(doComplete_ContractCompleteInstallation_MetaData))]
    public class doCompleteContractCompleteInstallation : doCompleteInstallationData
    {

    }

    [MetadataType(typeof(doComplete_RentalCompleteInstallation_MetaData))]
    public class doCompleteRentalCompleteInstallation : doCompleteInstallationData
    {

    }

    [MetadataType(typeof(doComplete_SaleCompleteInstallation_MetaData))]
    public class doCompleteSaleCompleteInstallation : doCompleteInstallationData
    {

    }

    [MetadataType(typeof(doComplete_SaleCancelInstallation_MetaData))]
    public class doCompleteSaleCancelInstallation : doCompleteInstallationData
    {

    }

}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class doComplete_ContractCompleteInstallation_MetaData
    {
        [NotNullOrEmpty]
        public string ServiceTypeCode { get; set; }

        [NotNullOrEmpty]
        public bool CompleteInstallationProcessFlag { get; set; }
    }

    public class doComplete_RentalCompleteInstallation_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public string InstallationType { get; set; }

        [NotNullOrEmpty]
        public string InstallationSlipNo { get; set; }
    }

    public class doComplete_SaleCompleteInstallation_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public string InstallationType { get; set; }

        [NotNullOrEmpty]
        public string InstallationSlipNo { get; set; }

        [NotNullOrEmpty]
        public bool CompleteInstallationProcessFlag { get; set; }
    }

    public class doComplete_SaleCancelInstallation_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public string OCC { get; set; }
    }
}

