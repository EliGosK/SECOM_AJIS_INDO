using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SECOM_AJIS.DataEntity.Common
{
    [Serializable]
    public class doBillingClientSearchCondition
    {
        public string BillingClientCode { get; set; }
        public string BillingClientName { get; set; }
        public string chkJuristic { get; set; }
        public string CompanyTypeCode { get; set; }
        public string chkIndividual { get; set; }
        public string chkAssociation { get; set; }
        public string chkPublicOffice { get; set; }
        public string chkOther { get; set; }
        public string RegionCode { get; set; }
        public string BusinessTypeCode { get; set; }
        public string Address { get; set; }
        public string TelephoneNo { get; set; }
        public string CustomerTypeCode { get; set; }

        public int Counter { set; get; }

    }
}
