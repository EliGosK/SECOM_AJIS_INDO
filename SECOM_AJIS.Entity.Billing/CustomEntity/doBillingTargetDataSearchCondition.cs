using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SECOM_AJIS.DataEntity.Billing
{
    [Serializable]
    public class doBillingTargetDataSearchCondition
    {
        public string CMS470_BillingClientCode { get; set; }
        public string CMS470_BillingOffice { get; set; }
        public string CMS470_BillingClientName { get; set; }
        public string CMS470_chkJuristic { get; set; }
        public string CMS470_CompanyTypeCode { get; set; }
        public string CMS470_chkIndividual { get; set; }
        public string CMS470_chkAssociation { get; set; }
        public string CMS470_chkPublicOffice { get; set; }
        public string CMS470_chkOther { get; set; }
        public string CMS470_RegionCode { get; set; }
        public string CMS470_BusinessTypeCode { get; set; }
        public string CMS470_Address { get; set; }
        public string CMS470_TelephoneNo { get; set; }
        public string CMS470_CustomerTypeCode { get; set; }
        public int Counter { set; get; }

    }
}
