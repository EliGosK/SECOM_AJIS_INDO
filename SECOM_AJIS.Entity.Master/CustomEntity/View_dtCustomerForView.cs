using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public class View_dtCustomerForView : dtCustomerForView
    {


        public string CustStatusName { set; get; }
        public string CustTypeName { set; get; }
        public string FinancialMaketTypeName { set; get; }
        public string BusinessTypeName { set; get; }

    }
}
