using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Billing
{
    class doBatchGenerateAutoTransferBankFile
    {
    }

    public class doHeaderBatchGenerateAutoTransferBankFile
    {
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
        public string Col5 { get; set; }
        public string Col6 { get; set; }
        public string Col7 { get; set; }
    }

    public partial class doBodyBatchGenerateAutoTransferBankFile
    {
        #region Primitive Properties

        public Nullable<long> Row
        {
            get;
            set;
        }

        public string InvoiceNo
        {
            get;
            set;
        }

        public string FullNameEN
        {
            get;
            set;
        }

        public string FullNameLC
        {
            get;
            set;
        }

        public string AccountNo
        {
            get;
            set;
        }

        public Nullable<decimal> InvoiceAmount
        {
            get;
            set;
        }

        public decimal WHTAmount
        {
            get;
            set;
        }

        #endregion
    }

    public class doFooterBatchGenerateAutoTransferBankFile
    {
        public int Col1 { get; set; }
        public decimal? Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
        public string Col5 { get; set; }
        public string Col6 { get; set; }
        public string Col7 { get; set; }
    }
}
