using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dsCancelContractQuotation
    {
        private List<tbt_CancelContractMemo> _dttbt_CancelContractMemo;
        public List<tbt_CancelContractMemo> dtTbt_CancelContractMemo { get { return this._dttbt_CancelContractMemo; } set { this._dttbt_CancelContractMemo = value; } }

        private List<tbt_CancelContractMemoDetail> _dttbt_CancelContractMemoDetail;
        public List<tbt_CancelContractMemoDetail> dtTbt_CancelContractMemoDetail { get { return this._dttbt_CancelContractMemoDetail; } set { this._dttbt_CancelContractMemoDetail = value; } }
    }
}
