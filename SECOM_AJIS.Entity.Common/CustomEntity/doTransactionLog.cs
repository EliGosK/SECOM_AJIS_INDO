using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SECOM_AJIS.DataEntity.Common
{
    public class doTransactionLog
    {
        public enum eTransactionType
        {
            Insert
            ,Update
            , Delete
        }

        private eTransactionType? oTransactionType;
        public eTransactionType? TransactionType
        {
            get { return this.oTransactionType; }
            set { this.oTransactionType = value; }
        }

        private string strTableName;
        public string TableName
        {
            get { return this.strTableName; }
            set { this.strTableName = value; }
        }

        //private DataTable dtTableData;
        //public DataTable TableData
        //{
        //    get { return this.dtTableData; }
        //    set { this.dtTableData = value; }
        //}
        private string strTableData;
        public string TableData 
        {
            get { return this.strTableData; }
            set { this.strTableData = value; }
        }
    }
}
