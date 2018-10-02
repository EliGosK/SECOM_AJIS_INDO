using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsContractDocData
    {
        private List<tbt_ContractDocument> _dttbt_ContractDocument;
        public List<tbt_ContractDocument> dtTbt_ContractDocument { get { return this._dttbt_ContractDocument; } set { this._dttbt_ContractDocument = value; } }

        private List<tbt_DocCancelContractMemo> _dttbt_DocCancelContractMemo;
        public List<tbt_DocCancelContractMemo> dtTbt_DocCancelContractMemo { get { return this._dttbt_DocCancelContractMemo; } set { this._dttbt_DocCancelContractMemo = value; } }

        private List<tbt_DocCancelContractMemoDetail> _dttbt_DocCancelContractMemoDetail;
        public List<tbt_DocCancelContractMemoDetail> dtTbt_DocCancelContractMemoDetail { get { return this._dttbt_DocCancelContractMemoDetail; } set { this._dttbt_DocCancelContractMemoDetail = value; } }

        private List<tbt_DocChangeMemo> _dttbt_DocChangeMemo;
        public List<tbt_DocChangeMemo> dtTbt_DocChangeMemo { get { return this._dttbt_DocChangeMemo; } set { this._dttbt_DocChangeMemo = value; } }

        private List<tbt_DocChangeNotice> _dttbt_DocChangeNotice;
        public List<tbt_DocChangeNotice> dtTbt_DocChangeNotice { get { return this._dttbt_DocChangeNotice; } set { this._dttbt_DocChangeNotice = value; } }

        private List<tbt_DocChangeFeeMemo> _dttbt_DocChangeFeeMemo;
        public List<tbt_DocChangeFeeMemo> dtTbt_DocChangeFeeMemo { get { return this._dttbt_DocChangeFeeMemo; } set { this._dttbt_DocChangeFeeMemo = value; } }

        private List<tbt_DocContractReport> _dttbt_DocContractReport;
        public List<tbt_DocContractReport> dtTbt_DocContractReport { get { return this._dttbt_DocContractReport; } set { this._dttbt_DocContractReport = value; } }

        private List<tbt_DocConfirmCurrentInstrumentMemo> _dttbt_DocConfirmCurrentInstrumentMemo;
        public List<tbt_DocConfirmCurrentInstrumentMemo> dtTbt_DocConfirmCurrentInstrumentMemo { get { return this._dttbt_DocConfirmCurrentInstrumentMemo; } set { this._dttbt_DocConfirmCurrentInstrumentMemo = value; } }

        private List<tbt_DocInstrumentDetails> _dttbt_DocInstrumentDetail;
        public List<tbt_DocInstrumentDetails> dtTbt_DocInstrumentDetail { get { return this._dttbt_DocInstrumentDetail; } set { this._dttbt_DocInstrumentDetail = value; } }

        //Add by Jutarat A. on 22042013
        private List<tbt_DocStartMemo> _dttbt_DocStartMemo;
        public List<tbt_DocStartMemo> dtTbt_DocStartMemo { get { return this._dttbt_DocStartMemo; } set { this._dttbt_DocStartMemo = value; } }
        //End Add

    }
}
