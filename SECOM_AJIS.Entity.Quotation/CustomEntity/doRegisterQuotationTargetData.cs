using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class doRegisterQuotationTargetData
    {
        private tbt_QuotationTarget _Tbt_QuotationTarget; public tbt_QuotationTarget doTbt_QuotationTarget { get { return this._Tbt_QuotationTarget; } set { this._Tbt_QuotationTarget = value; } }
        private tbt_QuotationCustomer _Tbt_QuotationCustomer1; public tbt_QuotationCustomer doTbt_QuotationCustomer1 { get { return this._Tbt_QuotationCustomer1; } set { this._Tbt_QuotationCustomer1 = value; } }
        private tbt_QuotationCustomer _Tbt_QuotationCustomer2; public tbt_QuotationCustomer doTbt_QuotationCustomer2 { get { return this._Tbt_QuotationCustomer2; } set { this._Tbt_QuotationCustomer2 = value; } }
        private tbt_QuotationSite _Tbt_QuotationSite; public tbt_QuotationSite doTbt_QuotationSite { get { return this._Tbt_QuotationSite; } set { this._Tbt_QuotationSite = value; } }
        private bool booBranchContractFlag; public bool BranchContractFlag { get { return this.booBranchContractFlag; } set { this.booBranchContractFlag = value; } } 

    }
}
