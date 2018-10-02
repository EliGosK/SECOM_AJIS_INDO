using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Billing
{
    public class View_dtBillingTargetData : dtBillingTargetData
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
                //base.BillingClientCode = c.ConvertBillingClientCode(base.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                srtObject = "{" +
                                        "\"BillingTargetCode\":\"" + ReplaceDoubleQuote(base.BillingTargetCode) + "\"," +
                                        "\"BillingOfficeCode\":\"" + ReplaceDoubleQuote(base.BillingOfficeCode) + "\"," +
                                        "\"OfficeNameEN\":\"" + ReplaceDoubleQuote(base.OfficeNameEN) + "\"," +
                                        "\"BillingClientCode\":\"" + ReplaceDoubleQuote(base.BillingClientCode) + "\"," +
                                        "\"BillingTargetNo\":\"" + ReplaceDoubleQuote(base.BillingTargetNo) + "\"," +
                                        "\"NameEN\":\"" + ReplaceDoubleQuote(base.NameEN) + "\"," +
                                        "\"NameLC\":\"" + ReplaceDoubleQuote(base.NameLC) + "\"," +
                                        "\"FullNameEN\":\"" + ReplaceDoubleQuote(base.FullNameEN) + "\"," +
                                        "\"FullNameLC\":\"" + ReplaceDoubleQuote(base.FullNameLC) + "\"," +
                                        "\"AddressEN\":\"" + ReplaceDoubleQuote(base.AddressEN) + "\"," +
                                        "\"AddressLC\":\"" + ReplaceDoubleQuote(base.AddressLC) + "\"," +
                                        "\"NoOfBillingBasic\":\"" + ReplaceDoubleQuote(base.NoOfBillingBasic) + "\"," +
                                        "\"NoOfBillingBasic_Text\":\"" + ReplaceDoubleQuote(base.NoOfBillingBasic_Numeric) + "\"," +
                                        "\"CustTypeCode\":\"" + ReplaceDoubleQuote(base.CustTypeCode) + "\"," +
                                        "\"OfficeName\":\"" + ReplaceDoubleQuote(base.OfficeName) + "\"," +
                                        "\"OfficeName_Text\":\"" + ReplaceDoubleQuote(base.OfficeName_Text) + "\"," +
                                        "\"CustTypeName\":\"" + ReplaceDoubleQuote(base.CustTypeName) + "\"," +
                                        "\"OfficeNameLC\":\"" + ReplaceDoubleQuote(base.CustTypeName) + "\"," +
                                        "\"OfficeNameJP\":\"" + ReplaceDoubleQuote(base.CustTypeName) + "\"," +
                                        "\"BillingClientCode_Short\":\"" + ReplaceDoubleQuote(base.BillingClientCode_Short) + "\"," +
                                        "\"BillingTargetCode_Short\":\"" + ReplaceDoubleQuote(base.BillingTargetCode_Short) + "\"" +                                    
                            "}";

                return srtObject;

            }

        }


        
        
       
    }
}
