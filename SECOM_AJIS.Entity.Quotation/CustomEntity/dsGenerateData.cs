using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [Serializable]
    public partial class dsGenerateData
    {
        private dtHeader _dtHeader;
        private List<dtInstrumentDetails> _dtInstrumentDetails;

        public dtHeader dtHeader
        {
            get { return this._dtHeader; }
            set { this._dtHeader = value; }
        }
        public List<dtInstrumentDetails> dtInstrumentDetails
        {
            get { return this._dtInstrumentDetails; }
            set { this._dtInstrumentDetails = value; }
        }

        public List<dtFacilityDetails> dtFacilityDetails { get; set; }

    }
    [Serializable]
    public class dtHeader
    {
        private string strInstallationSlipNo; public string InstallationSlipNo { get { return this.strInstallationSlipNo; } set { this.strInstallationSlipNo = value; } }
        private string strInstallationEngineerEmpNo; public string InstallationEngineerEmpNo { get { return this.strInstallationEngineerEmpNo; } set { this.strInstallationEngineerEmpNo = value; } }
        private string strApproveNo1; public string ApproveNo1 { get { return this.strApproveNo1; } set { this.strApproveNo1 = value; } }
        private string strApproveNo2; public string ApproveNo2 { get { return this.strApproveNo2; } set { this.strApproveNo2 = value; } }

        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public decimal? InstallationFee { get; set; }
        public decimal? InstallationFeeUsd { get; set; }
        public string InstallationFeeCurrencyType { get; set; }
        //[NotNullOrEmpty]
        public decimal? NormalContractFee { get; set; }
        public decimal? NormalContractFeeUsd { get; set; }
        public string NormalContractFeeCurrencyType { get; set; }
    }
    [Serializable]
    public class dtInstrumentDetails
    {
        private string strInstrumentCode; public string InstrumentCode { get { return this.strInstrumentCode; } set { this.strInstrumentCode = value; } }
        //private int intInstallQty; public int InstallQty { get { return this.intInstallQty; } set { this.intInstallQty = value; } }
        //private int intAcmAddQty; public int AcmAddQty { get { return this.intAcmAddQty; } set { this.intAcmAddQty = value; } }
        //private int intAcmRemoveQty; public int AcmRemoveQty { get { return this.intAcmRemoveQty; } set { this.intAcmRemoveQty = value; } } 
        public int? InstallQty { get; set; }
        public int? AcmAddQty { get; set; }
        public int? AcmRemoveQty { get; set; }
    }
    [Serializable]
    public class dtFacilityDetails
    {
        public string FacilityCode { get; set; }
        public int? FacilityQty { get; set; }
    }
}
