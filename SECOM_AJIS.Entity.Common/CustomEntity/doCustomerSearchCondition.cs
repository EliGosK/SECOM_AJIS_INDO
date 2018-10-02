using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SECOM_AJIS.DataEntity.Common
{
    [Serializable]
    public class doCustomerSearchCondition
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string chkNewCustomer { get; set; }
        public string chkExistCustomer { get; set; }
        public string chkJuristic { get; set; }
        public string CompanyTypeCode { get; set; }
        public string chkIndividual { get; set; }
        public string chkAssociation { get; set; }
        public string chkPublicOffice { get; set; }
        public string chkOther { get; set; }
        public string IDNo { get; set; }
        public bool DummyIDFlag { get; set; }
        public string RegionCode { get; set; }
        public string BusinessTypeCode { get; set; }
        public string Address { get; set; }
        public string Alley { get; set; }
        public string Road { get; set; }
        public string SubDistrict { get; set; }
        public string ProvinceCode { get; set; }
        public string DistrictCode { get; set; }
        public string ZipCode { get; set; }
        public string TelephoneNo { get; set; }
        public string GroupName { get; set; }
        public string CustStatus { set; get; }
        public string CustomerTypeCode { set; get; }

        public int Counter { set; get; }

    }
}
