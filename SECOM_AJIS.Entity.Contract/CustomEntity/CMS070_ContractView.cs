using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class CMS070_ContractView
    {
        public string ContractCode { get; set; }
        public string ContractNameDisplay { get; set; }
        public string ChangeTypeNameDisplay { get; set; }
        public string ProductNameDisplay { get; set; }
        public string OfficeDisplay { get; set; }
        public string ContactDetail { get; set; }
        public string SiteDetail { get; set; }
        public string ServiceTypeCode { get; set; }
        public string SiteCode { get; set; }
    }
    public class CMS070_CustomerView
    {
        public string CustCode { get; set; }
        public string CustNameDisplay { get; set; }
        public string CustAddressDisplay { get; set; }
        //public string NumofSite_Rental { get; set; }
        //public string NumofSite_Sale { get; set; }
        public int NumofSite_Rental { get; set; }
        public int NumofSite_Sale { get; set; }
        public string Detail { get; set; }

    }
}
