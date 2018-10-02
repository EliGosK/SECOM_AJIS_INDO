using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class View_dtContractData : dtContractData
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

        public string SiteCode_Short
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertSiteCode(base.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string CustCode_Short
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertCustCode(base.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string ContractCode_Short
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(base.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        // Create JSON object
        public string Object
        {
            get
            {
                //CommonUtil c = new CommonUtil();
                //base.SiteCode = c.ConvertSiteCode(base.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //base.CustCode = c.ConvertCustCode(base.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                //base.ContractCode = c.ConvertContractCode(base.ContractCode,CommonUtil.CONVERT_TYPE.TO_SHORT);

                srtObject = "{" +

                                "\"ContractCode\":\"" + ReplaceDoubleQuote(this.ContractCode_Short) + "\"," +
                                "\"OCC\":\"" + ReplaceDoubleQuote(base.OCC) + "\"," +
                                "\"ServiceTypeCode\":\"" + ReplaceDoubleQuote(base.ServiceTypeCode) + "\"," +
                                "\"CustCode\":\"" +  ReplaceDoubleQuote(this.CustCode_Short) + "\"," +
                                "\"CustNameEN\":\"" + ReplaceDoubleQuote(base.CustNameEN) + "\"," +
                                "\"CustNameLC\":\"" + ReplaceDoubleQuote(base.CustNameLC) + "\"," +
                                "\"CustFullNameEN\":\"" + ReplaceDoubleQuote(base.CustFullNameEN) + "\"," +
                                "\"CustFullNameLC\":\"" + ReplaceDoubleQuote(base.CustFullNameLC) + "\"," +
                                "\"AddressEN\":\"" + ReplaceDoubleQuote(base.AddressEN) + "\"," +
                                "\"AlleyEN\":\"" + ReplaceDoubleQuote(base.AlleyEN) + "\"," +
                                "\"RoadEN\":\"" + ReplaceDoubleQuote(base.RoadEN) + "\"," +
                                "\"SubDistrictEN\":\"" + ReplaceDoubleQuote(base.SubDistrictEN) + "\"," +
                                "\"AddressFullEN\":\"" + ReplaceDoubleQuote(base.AddressFullEN) + "\"," +
                                "\"AddressLC\":\"" + ReplaceDoubleQuote(base.AddressLC) + "\"," +
                                "\"AlleyLC\":\"" + ReplaceDoubleQuote(base.AlleyLC) + "\"," +
                                "\"RoadLC\":\"" + ReplaceDoubleQuote(base.RoadLC) + "\"," +
                                "\"SubDistrictLC\":\"" + ReplaceDoubleQuote(base.SubDistrictLC) + "\"," +
                                "\"AddressFullLC\":\"" + ReplaceDoubleQuote(base.AddressFullLC) + "\"," +
                                "\"DistrictCode\":\"" + ReplaceDoubleQuote(base.DistrictCode) + "\"," +
                                "\"DistrictNameEN\":\"" + ReplaceDoubleQuote(base.DistrictNameEN) + "\"," +
                                "\"DistrictNameLC\":\"" + ReplaceDoubleQuote(base.DistrictNameLC) + "\"," +
                                "\"ProvinceCode\":\"" + ReplaceDoubleQuote(base.ProvinceCode) + "\"," +
                                "\"ProvinceNameEN\":\"" + ReplaceDoubleQuote(base.ProvinceNameEN) + "\"," +
                                "\"ProvinceNameLC\":\"" + ReplaceDoubleQuote(base.ProvinceNameLC) + "\"," +
                                "\"ZipCode\":\"" + ReplaceDoubleQuote(base.ZipCode) + "\"," +
                                "\"SiteCode\":\"" + ReplaceDoubleQuote(this.SiteCode_Short)  + "\"," +
                                "\"SiteNameEN\":\"" + ReplaceDoubleQuote(base.SiteNameEN) + "\"," +
                                "\"SiteNameLC\":\"" + ReplaceDoubleQuote(base.SiteNameLC) + "\"," +
                                "\"OperationOfficeCode\":\"" + ReplaceDoubleQuote(base.OperationOfficeCode) + "\"," +
                                "\"OperationOfficeNameEN\":\"" + ReplaceDoubleQuote(base.OperationOfficeNameEN) + "\"," +
                                "\"OperationOfficeNameJP\":\"" + ReplaceDoubleQuote(base.OperationOfficeNameJP) + "\"," +
                                "\"OperationOfficeNameLC\":\"" + ReplaceDoubleQuote(base.OperationOfficeNameLC) + "\"," +
                                "\"ContractOfficeCode\":\"" + ReplaceDoubleQuote(base.ContractOfficeCode) + "\"," +
                                "\"ContractOfficeNameEN\":\"" + ReplaceDoubleQuote(base.ContractOfficeNameEN) + "\"," +
                                "\"ContractOfficeNameJP\":\"" + ReplaceDoubleQuote(base.ContractOfficeNameJP) + "\"," +
                                "\"ContractOfficeNameLC\":\"" + ReplaceDoubleQuote(base.ContractOfficeNameLC) + "\"," +
                                "\"OperationDate\":\"" + ReplaceDoubleQuote(base.OperationDate) + "\"," +
                                "\"Price\":\"" + ReplaceDoubleQuote(base.Price) + "\"" +

                             "}";

                return srtObject;
            }
        }
    }
}
