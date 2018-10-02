using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsRentalContractData
    {
        //private List<tbt_RentalOperationType> _dtTbt_RentalOperationType;
        //private List<tbt_RentalSecurityBasic> _dtTbt_RentalSecurityBasic;
        //private List<tbt_RentalContractBasic> _dtTbt_RentalContractBasic;
        //private List<tbt_RentalInstrumentDetails> _dtTbt_RentalInstrumentDetails;

        //public List<tbt_RentalOperationType> dtTbt_RentalOperationType
        //{
        //    get { return this._dtTbt_RentalOperationType; }
        //    set { this._dtTbt_RentalOperationType = value; }
        //}

        //dsRentalContract
        //.dtTbt_RentalContractBasic
        //.dtTbt_RentalSecurityBasic        //.dtTbt_RentalBEDetails        //.dtTbt_RentalInstrumentDetails        //.dtTbt_RentalInstSubcontractor
        //.dtTbt_RentalMaintenanceDetails        //.dtTbt_RentalOperationType        //.dtTbt_RentalSentryGuard        //.dtTbt_RentalSentryGuardDetails        //.dtTbt_CancelContractMemo
        //.dtTbt_CancelContractMemoDetail

        private List<tbt_RentalContractBasic> _dttbt_RentalContractBasic;
        public List<tbt_RentalContractBasic> dtTbt_RentalContractBasic { get { return this._dttbt_RentalContractBasic; } set { this._dttbt_RentalContractBasic = value; } }

        private List<tbt_RentalSecurityBasic> _dttbt_RentalSecurityBasic;
        public List<tbt_RentalSecurityBasic> dtTbt_RentalSecurityBasic { get { return this._dttbt_RentalSecurityBasic; } set { this._dttbt_RentalSecurityBasic = value; } }

        private List<tbt_RentalBEDetails> _dttbt_RentalBEDetails;
        public List<tbt_RentalBEDetails> dtTbt_RentalBEDetails { get { return this._dttbt_RentalBEDetails; } set { this._dttbt_RentalBEDetails = value; } }
                
        private List<tbt_RentalInstrumentDetails> _dttbt_RentalInstrumentDetails; 
        public List<tbt_RentalInstrumentDetails> dtTbt_RentalInstrumentDetails { get { return this._dttbt_RentalInstrumentDetails; } set { this._dttbt_RentalInstrumentDetails = value; } }

        private List<tbt_RentalInstSubcontractor> _dttbt_RentalInstSubcontractor;
        public List<tbt_RentalInstSubcontractor> dtTbt_RentalInstSubcontractor { get { return this._dttbt_RentalInstSubcontractor; } set { this._dttbt_RentalInstSubcontractor = value; } }

        private List<tbt_RentalMaintenanceDetails> _dttbt_RentalMaintenanceDetails;
        public List<tbt_RentalMaintenanceDetails> dtTbt_RentalMaintenanceDetails { get { return this._dttbt_RentalMaintenanceDetails; } set { this._dttbt_RentalMaintenanceDetails = value; } }

        private List<tbt_RentalOperationType> _dttbt_RentalOperationType;
        public List<tbt_RentalOperationType> dtTbt_RentalOperationType { get { return this._dttbt_RentalOperationType; } set { this._dttbt_RentalOperationType = value; } }

        private List<tbt_RentalSentryGuard> _dttbt_RentalSentryGuard;
        public List<tbt_RentalSentryGuard> dtTbt_RentalSentryGuard { get { return this._dttbt_RentalSentryGuard; } set { this._dttbt_RentalSentryGuard = value; } }

        private List<tbt_RentalSentryGuardDetails> _dttbt_RentalSentryGuardDetails;
        public List<tbt_RentalSentryGuardDetails> dtTbt_RentalSentryGuardDetails { get { return this._dttbt_RentalSentryGuardDetails; } set { this._dttbt_RentalSentryGuardDetails = value; } }

        private List<tbt_CancelContractMemo> _dttbt_CancelContractMemo;
        public List<tbt_CancelContractMemo> dtTbt_CancelContractMemo { get { return this._dttbt_CancelContractMemo; } set { this._dttbt_CancelContractMemo = value; } }

        private List<tbt_CancelContractMemoDetail> _dttbt_CancelContractMemoDetail;
        public List<tbt_CancelContractMemoDetail> dtTbt_CancelContractMemoDetail { get { return this._dttbt_CancelContractMemoDetail; } set { this._dttbt_CancelContractMemoDetail = value; } }

        public List<tbt_RelationType> dtTbt_RelationType { get; set; }

    }
}
