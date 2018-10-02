using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    public class View_dtCustomerData : dtCustomerData
    {
        private string ReplaceDoubleQuote(object txtVal)
        {
            if (txtVal != null)
            {
                if (txtVal.GetType() == typeof(string))
                {
                    txtVal = (txtVal == null) ? txtVal : ((string)txtVal).Replace("\"", "\\\"");
                }
                else if (txtVal.GetType() == typeof(bool))
                {
                    txtVal = txtVal.ToString();
                }
                else
                {
                    txtVal = txtVal.ToString();
                }
            }

            return (string)txtVal;
            
        }

        string srtObject = "";

        // Create JSON object
        public string Object
        {
            get
            {
                //CommonUtil c = new CommonUtil();
                //base.CustCode = c.ConvertCustCode(base.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                srtObject = "{" +

                                "\"CustCode\":\"" + ReplaceDoubleQuote(this.CustCode) + "\"," +
                                "\"CustStatus\":\"" + ReplaceDoubleQuote(this.CustStatus) + "\"," +
                                "\"CustStatusNameEN\":\"" + ReplaceDoubleQuote(this.CustStatusNameEN) + "\"," +
                                "\"CustStatusNameJP\":\"" + ReplaceDoubleQuote(this.CustStatusNameJP) + "\"," +
                                "\"CustStatusNameLC\":\"" + ReplaceDoubleQuote(this.CustStatusNameLC) + "\"," +
                                "\"ImportantFlag\":\"" + ReplaceDoubleQuote(this.ImportantFlag) + "\"," +
                                "\"CustNameEN\":\"" + ReplaceDoubleQuote(this.CustNameEN) + "\"," +
                                "\"CustNameLC\":\"" + ReplaceDoubleQuote(this.CustNameLC) + "\"," +
                                "\"CustFullNameEN\":\"" + ReplaceDoubleQuote(this.CustFullNameEN) + "\"," +
                                "\"CustFullNameLC\":\"" + ReplaceDoubleQuote(this.CustFullNameLC) + "\"," +
                                "\"RepPersonName\":\"" + ReplaceDoubleQuote(this.RepPersonName) + "\"," +
                                "\"ContactPersonName\":\"" + ReplaceDoubleQuote(this.ContactPersonName) + "\"," +
                                "\"SECOMContactPerson\":\"" + ReplaceDoubleQuote(this.SECOMContactPerson) + "\"," +
                                "\"CustTypeCode\":\"" + ReplaceDoubleQuote(this.CustTypeCode) + "\"," +
                                "\"CustTypeNameEN\":\"" + ReplaceDoubleQuote(this.CustTypeNameEN) + "\"," +
                                "\"CustTypeNameJP\":\"" + ReplaceDoubleQuote(this.CustTypeNameJP) + "\"," +
                                "\"CustTypeNameLC\":\"" + ReplaceDoubleQuote(this.CustTypeNameLC) + "\"," +
                                "\"CompanyTypeCode\":\"" + ReplaceDoubleQuote(this.CompanyTypeCode) + "\"," +
                                "\"CompanyTypeNameEN\":\"" + ReplaceDoubleQuote(this.CompanyTypeNameEN) + "\"," +
                                "\"CompanyTypeNameLC\":\"" + ReplaceDoubleQuote(this.CompanyTypeNameLC) + "\"," +
                                "\"FinancialMaketTypeCode\":\"" + ReplaceDoubleQuote(this.FinancialMaketTypeCode) + "\"," +
                                "\"FinancialMaketTypeNameEN\":\"" + ReplaceDoubleQuote(this.FinancialMaketTypeNameEN) + "\"," +
                                "\"FinancialMaketTypeNameJP\":\"" + ReplaceDoubleQuote(this.FinancialMaketTypeNameJP) + "\"," +
                                "\"FinancialMaketTypeNameLC\":\"" + ReplaceDoubleQuote(this.FinancialMaketTypeNameLC) + "\"," +
                                "\"BusinessTypeCode\":\"" + ReplaceDoubleQuote(this.BusinessTypeCode) + "\"," +
                                "\"BusinessTypeNameEN\":\"" + ReplaceDoubleQuote(this.BusinessTypeNameEN) + "\"," +
                                "\"BusinessTypeNameJP\":\"" + ReplaceDoubleQuote(this.BusinessTypeNameJP) + "\"," +
                                "\"BusinessTypeNameLC\":\"" + ReplaceDoubleQuote(this.BusinessTypeNameLC) + "\"," +
                                "\"PhoneNo\":\"" + ReplaceDoubleQuote(this.PhoneNo) + "\"," +
                                "\"FaxNo\":\"" + ReplaceDoubleQuote(this.FaxNo) + "\"," +
                                "\"IDNo\":\"" + ReplaceDoubleQuote(this.IDNo) + "\"," +
                                "\"DummyIDFlag\":\"" + ReplaceDoubleQuote(this.DummyIDFlag) + "\"," +
                                "\"RegionCode\":\"" + ReplaceDoubleQuote(this.RegionCode) + "\"," +
                                "\"NationalityEN\":\"" + ReplaceDoubleQuote(this.NationalityEN) + "\"," +
                                "\"NationalityJP\":\"" + ReplaceDoubleQuote(this.NationalityJP) + "\"," +
                                "\"NationalityLC\":\"" + ReplaceDoubleQuote(this.NationalityLC) + "\"," +
                                "\"URL\":\"" + ReplaceDoubleQuote(this.URL) + "\"," +
                                "\"Memo\":\"" + ReplaceDoubleQuote(this.Memo) + "\"," +
                                "\"AddressEN\":\"" + ReplaceDoubleQuote(this.AddressEN) + "\"," +
                                "\"AlleyEN\":\"" + ReplaceDoubleQuote(this.AlleyEN) + "\"," +
                                "\"RoadEN\":\"" + ReplaceDoubleQuote(this.RoadEN) + "\"," +
                                "\"SubDistrictEN\":\"" + ReplaceDoubleQuote(this.SubDistrictEN) + "\"," +
                                "\"AddressFullEN\":\"" + ReplaceDoubleQuote(this.AddressFullEN) + "\"," +
                                "\"AddressLC\":\"" + ReplaceDoubleQuote(this.AddressLC) + "\"," +
                                "\"AlleyLC\":\"" + ReplaceDoubleQuote(this.AlleyLC) + "\"," +
                                "\"RoadLC\":\"" + ReplaceDoubleQuote(this.RoadLC) + "\"," +
                                "\"SubDistrictLC\":\"" + ReplaceDoubleQuote(this.SubDistrictLC) + "\"," +
                                "\"AddressFullLC\":\"" + ReplaceDoubleQuote(this.AddressFullLC) + "\"," +
                                "\"DistrictCode\":\"" + ReplaceDoubleQuote(this.DistrictCode) + "\"," +
                                "\"DistrictNameEN\":\"" + ReplaceDoubleQuote(this.DistrictNameEN) + "\"," +
                                "\"DistrictNameLC\":\"" + ReplaceDoubleQuote(this.DistrictNameLC) + "\"," +
                                "\"ProvinceCode\":\"" + ReplaceDoubleQuote(this.ProvinceCode) + "\"," +
                                "\"ProvinceNameEN\":\"" + ReplaceDoubleQuote(this.ProvinceNameEN) + "\"," +
                                "\"ProvinceNameLC\":\"" + ReplaceDoubleQuote(this.ProvinceNameLC) + "\"," +
                                "\"ZipCode\":\"" + ReplaceDoubleQuote(this.ZipCode) + "\"," +
                                "\"DeleteFlag\":\"" + ReplaceDoubleQuote(this.DeleteFlag) + "\"" +


                    "}";


                return srtObject;
            }
        }

    }
    public class View_dtCustomerData2 
    {
        public string CustCode
        {
            get;
            set;
        }
        public string CustName_Extra
        {
            get;
            set;
        }

        public string Address_Extra
        {
            get;
            set;
        }
        private string ReplaceDoubleQuote(object txtVal)
        {
            if (txtVal != null)
            {
                if (txtVal.GetType() == typeof(string))
                {
                    txtVal = (txtVal == null) ? txtVal : ((string)txtVal).Replace("\"", "\\\"");
                }
                else if (txtVal.GetType() == typeof(bool))
                {
                    txtVal = txtVal.ToString();
                }
                else
                {
                    txtVal = txtVal.ToString();
                }
            }

            return (string)txtVal;

        }

        string srtObject = "";

        // Create JSON object
        public string Object
        {
            get
            {
                //CommonUtil c = new CommonUtil();
                //base.CustCode = c.ConvertCustCode(base.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                srtObject = "{" +

                                "\"CustCode\":\"" + ReplaceDoubleQuote(this.CustCode) + "\"" +
                                


                    "}";


                return srtObject;
            }
        }

    }
}
