using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    public class View_dtBillingClientData : dtBillingClientDataForSearch
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

        public string BillingClientCode_Short
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertBillingClientCode(base.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        string srtObject = "";

        // Create JSON object
        public string Object
        {
            get
            {

                //CommonUtil c = new CommonUtil();
                //base.BillingClientCode = c.ConvertBillingClientCode(base.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                srtObject = "{" +
                                        "\"BillingClientCode\":\"" + ReplaceDoubleQuote(this.BillingClientCode_Short) + "\"," +
                                        "\"NameEN\":\"" + ReplaceDoubleQuote(base.NameEN) + "\"," +
                                        "\"NameLC\":\"" + ReplaceDoubleQuote(base.NameLC) + "\"," +
                                        "\"FullNameEN\":\"" + ReplaceDoubleQuote(base.FullNameEN) + "\"," +
                                        "\"FullNameLC\":\"" + ReplaceDoubleQuote(base.FullNameLC) + "\"," +
                                        "\"BranchNameEN\":\"" + ReplaceDoubleQuote(base.BranchNameEN) + "\"," +
                                        "\"BranchNameLC\":\"" + ReplaceDoubleQuote(base.BranchNameLC) + "\"," +
                                        "\"CustTypeCode\":\"" + ReplaceDoubleQuote(base.CustTypeCode) + "\"," +
                                        "\"CustTypeNameEN\":\"" + ReplaceDoubleQuote(base.CustTypeNameEN) + "\"," +
                                        "\"CustTypeNameJP\":\"" + ReplaceDoubleQuote(base.CustTypeNameJP) + "\"," +
                                        "\"CustTypeNameLC\":\"" + ReplaceDoubleQuote(base.CustTypeNameLC) + "\"," +
                                        "\"CompanyTypeCode\":\"" + ReplaceDoubleQuote(base.CompanyTypeCode) + "\"," +
                                        "\"CompanyTypeNameEN\":\"" + ReplaceDoubleQuote(base.CompanyTypeNameEN) + "\"," +
                                        "\"CompanyTypeNameLC\":\"" + ReplaceDoubleQuote(base.CompanyTypeNameLC)+ "\"," +
                                        "\"BusinessTypeCode\":\"" + ReplaceDoubleQuote(base.BusinessTypeCode) + "\"," +
                                        "\"BusinessTypeNameEN\":\"" + ReplaceDoubleQuote(base.BusinessTypeNameEN) + "\"," +
                                        "\"BusinessTypeNameJP\":\"" + ReplaceDoubleQuote(base.BusinessTypeNameJP) + "\"," +
                                        "\"BusinessTypeNameLC\":\"" + ReplaceDoubleQuote(base.BusinessTypeNameLC) + "\"," +
                                        "\"PhoneNo\":\"" + ReplaceDoubleQuote(base.PhoneNo) + "\"," +
                                        "\"IDNo\":\"" + ReplaceDoubleQuote(base.IDNo) + "\"," +
                                        "\"RegionCode\":\"" + ReplaceDoubleQuote(base.RegionCode) + "\"," +
                                        "\"NationalityEN\":\"" + ReplaceDoubleQuote(base.NationalityEN) + "\"," +
                                        "\"NationalityJP\":\"" + ReplaceDoubleQuote(base.NationalityJP) + "\"," +
                                        "\"NationalityLC\":\"" + ReplaceDoubleQuote(base.NationalityLC) + "\"," +
                                        "\"AddressEN\":\"" + ReplaceDoubleQuote(base.AddressEN) + "\"," +
                                        "\"AddressLC\":\"" + ReplaceDoubleQuote(base.AddressLC) + "\"" +

                            "}";

                return srtObject;

            }

        }


    }
}
