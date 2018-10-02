using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(dtTbt_ProjectPurchaserCustomerForView_MetaData))]
    public partial class dtTbt_ProjectPurchaserCustomerForView
    {
        public string CustTypeName { get; set; }
        public string CompanyTypeName { get; set; }
        public string FinancialMarketTypeName { get; set; }
        public string BusinessTypeName { get; set; }
        public string Nationality { get; set; }
        public string DistrictName { get; set; }
        public string ProvinceName { get; set; }
        public string CustCodeShort { get { return new CommonUtil().ConvertCustCode(this.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT); } }
        public string CustTypeCodeName
        {
            get
            {
                return this.CustTypeCode
                    + ": " + this.CustTypeName;
            }
        }
        public string CustStatusCodeName { get; set; }

       

    }
}
namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class dtTbt_ProjectPurchaserCustomerForView_MetaData
    {
        [CustTypeMapping("CustTypeName")]
        public string CustTypeCode { get; set; }
        [LanguageMapping]
        public string CompanyTypeName { get; set; }
        [FinancialMarketTypeMapping("FinancialMarketTypeName")]
        public string FinancialMarketTypeCode { get; set; }
        [LanguageMapping]
        public string BusinessTypeName { get; set; }
        [LanguageMapping]
        public string Nationality { get; set; }
        [LanguageMapping]
        public string DistrictName { get; set; }
        [LanguageMapping]
        public string ProvinceName { get; set; }
    }

}
