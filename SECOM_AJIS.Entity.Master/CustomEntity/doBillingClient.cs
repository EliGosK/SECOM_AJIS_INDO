using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of billing client
    /// </summary>
    public class doBillingClient
    {
        public string BillingClientCode { get; set; }
        public string NameEN { get; set; }
        public string NameLC	{ get; set; }
        public string FullNameEN	{ get; set; }
        public string FullNameLC	{ get; set; }
        public string BranchNo { get; set; }
        public string BranchNameEN { get; set; }
        public string BranchNameLC	{ get; set; }
        
        [CustTypeMapping("CustTypeName")]
        public string CustTypeCode	{ get; set; }
        public string CustTypeName	{ get; set; }
        
        public string CompanyTypeCode	{ get; set; }
        public string CompanyTypeName	{ get; set; }
        public string BusinessTypeCode	{ get; set; }
        public string BusinessTypeName	{ get; set; }
        public string PhoneNo	{ get; set; }
        public string IDNo	{ get; set; }
        public string RegionCode	{ get; set; }
        public string Nationality	{ get; set; }
        public string AddressEN	{ get; set; }
        public string AddressLC	{ get; set; }
        public bool DeleteFlag	{ get; set; }
        public DateTime CreateDate	{ get; set; }
        public string CreateBy	{ get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy	{ get; set; }
        public bool ValidateBillingClient { get; set; } 
    }
}
