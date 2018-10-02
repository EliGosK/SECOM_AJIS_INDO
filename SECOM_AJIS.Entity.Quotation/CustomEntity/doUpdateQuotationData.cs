using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Quotation
{
   public partial class doUpdateQuotationData
    {
        private string strQuotationTargetCode; public string QuotationTargetCode { get { return this.strQuotationTargetCode; } set { this.strQuotationTargetCode = value; } }
        private string strAlphabet; public string Alphabet { get { return this.strAlphabet; } set { this.strAlphabet = value; } }
        private DateTime datLastUpdateDate; public DateTime LastUpdateDate { get { return this.datLastUpdateDate; } set { this.datLastUpdateDate = value; } }
        private string strContractCode; public string ContractCode { get { return this.strContractCode; } set { this.strContractCode = value; } }
        private string strActionTypeCode; public string ActionTypeCode { get { return this.strActionTypeCode; } set { this.strActionTypeCode = value; } }

        public string QuotationOfficeCode { get; set; }


    }
}
