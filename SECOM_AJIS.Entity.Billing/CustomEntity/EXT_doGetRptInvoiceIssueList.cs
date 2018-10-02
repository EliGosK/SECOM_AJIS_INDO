using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class doGetRptInvoiceIssueList
    {
        public string Kode_Jenis_Trans
        {
            get { return "01"; }
        }

        public string FP_FP_Pengganti
        {
            get { return "0";   }
        }
        public string Nomor_FP
        {
            get { return string.Empty; }
        }
        public string Keterangan
        {
            get { return string.Empty; }
        }
        public string Uang_Muka
        {
            get { return string.Empty; }
        }
        public string DPP_Uang_Muka
        {
            get { return string.Empty; }
        }
        public string PPN_Uang_Muka
        {
            get { return string.Empty; }
        }
        public string Uang_Muka_PPnBM
        {
            get { return string.Empty; }
        }
        public string Kode_Barang
        {
            get { return string.Empty;  }
        }
        public string Jumlah_Barang
        {
            get { return "1"; }
        }
        public string Diskon
        {
            get { return "0"; }
        }

    }
}
