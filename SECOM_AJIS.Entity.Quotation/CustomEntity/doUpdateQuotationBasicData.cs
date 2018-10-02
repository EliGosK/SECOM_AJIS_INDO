using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Quotation
{
   public class doUpdateQuotationBasicData
    {
        private string strQuotationTargetCode; public string QuotationTargetCode { get { return this.strQuotationTargetCode; } set { this.strQuotationTargetCode = value; } }
        private string strAlphabet; public string Alphabet { get { return this.strAlphabet; } set { this.strAlphabet = value; } }
        private string strContractTransferStatus; public string ContractTransferStatus { get { return this.strContractTransferStatus; } set { this.strContractTransferStatus = value; } } 

    }
}
