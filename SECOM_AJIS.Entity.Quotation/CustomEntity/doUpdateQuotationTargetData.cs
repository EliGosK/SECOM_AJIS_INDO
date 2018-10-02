using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public partial class doUpdateQuotationTargetData
    {
        private string strQuotationTargetCode; public string QuotationTargetCode { get { return this.strQuotationTargetCode; } set { this.strQuotationTargetCode = value; } }
        private string strContractTransferStatus; public string ContractTransferStatus { get { return this.strContractTransferStatus; } set { this.strContractTransferStatus = value; } }
        private string strContractCode; public string ContractCode { get { return this.strContractCode; } set { this.strContractCode = value; } }
        private DateTime? datTransferDate; public DateTime? TransferDate { get { return this.datTransferDate; } set { this.datTransferDate = value; } }
        private string strTransferAlphabet; public string TransferAlphabet { get { return this.strTransferAlphabet; } set { this.strTransferAlphabet = value; } }
        private string strLastAlphabet; public string LastAlphabet { get { return this.strLastAlphabet; } set { this.strLastAlphabet = value; } }

        public string QuotationOfficeCode { get; set; }
        public string OperationOfficeCode { get; set; }
                        
    }
}
