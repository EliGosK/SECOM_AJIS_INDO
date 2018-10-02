using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsSaleContractData
    {
        private List<tbt_SaleBasic> _dttbt_SaleBasic;
        public List<tbt_SaleBasic> dtTbt_SaleBasic { get { return this._dttbt_SaleBasic; } set { this._dttbt_SaleBasic = value; } }

        private List<tbt_SaleInstrumentDetails> _dttbt_SaleInstrumentDetails;
        public List<tbt_SaleInstrumentDetails> dtTbt_SaleInstrumentDetails { get { return this._dttbt_SaleInstrumentDetails; } set { this._dttbt_SaleInstrumentDetails = value; } }

    }
}
