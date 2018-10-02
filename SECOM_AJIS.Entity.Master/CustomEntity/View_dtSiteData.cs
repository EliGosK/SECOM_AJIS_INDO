using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    public class View_dtSiteData : dtSiteData
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
       

        // Create JSON object
        public string Object
        {
           
 
            get {


                srtObject = "{"+
                                    "\"SiteCode\":\""+ ReplaceDoubleQuote(this.SiteCode_Short) +"\","+
                                    "\"CustCode\":\"" + ReplaceDoubleQuote(this.CustCode_Short) + "\"," +
                                    "\"CustStatus\":\""+ReplaceDoubleQuote(base.CustStatus) +"\","+
                                    "\"CustStatusNameEN\":\""+ReplaceDoubleQuote(base.CustStatusNameEN) +"\","+
                                    "\"CustStatusNameLC\":\""+ReplaceDoubleQuote(base.CustStatusNameLC) +"\","+
                                    "\"CustStatusNameJP\":\""+ReplaceDoubleQuote(base.CustStatusNameJP) +"\","+
                                    "\"CustNameEN\":\""+ReplaceDoubleQuote(base.CustNameEN) +"\","+
                                    "\"CustNameLC\":\""+ReplaceDoubleQuote(base.CustNameLC) +"\","+
                                    "\"CustContactPersonName\":\""+ReplaceDoubleQuote(base.CustContactPersonName) +"\","+
                                    "\"CustSECOMContactPerson\":\""+ReplaceDoubleQuote(base.CustSECOMContactPerson) +"\","+
                                    "\"SiteNo\":\""+ReplaceDoubleQuote(base.SiteNo) +"\","+
                                    "\"SiteNameEN\":\""+ReplaceDoubleQuote(base.SiteNameEN) +"\","+
                                    "\"SiteNameLC\":\""+ReplaceDoubleQuote(base.SiteNameLC) +"\","+
                                    "\"SECOMContactPerson\":\""+ReplaceDoubleQuote(base.SECOMContactPerson) +"\","+
                                    "\"PersonInCharge\":\""+ReplaceDoubleQuote(base.PersonInCharge) +"\","+
                                    "\"PhoneNo\":\""+ReplaceDoubleQuote(base.PhoneNo) +"\","+
                                    "\"BuildingUsageCode\":\""+ReplaceDoubleQuote(base.BuildingUsageCode) +"\","+
                                    "\"BuildingUsageNameEN\":\""+ReplaceDoubleQuote(base.BuildingUsageNameEN) +"\","+
                                    "\"BuildingUsageNameJP\":\""+ReplaceDoubleQuote(base.BuildingUsageNameJP) +"\","+
                                    "\"BuildingUsageNameLC\":\""+ReplaceDoubleQuote(base.BuildingUsageNameLC) +"\","+
                                    "\"AddressEN\":\""+ReplaceDoubleQuote(base.AddressEN) +"\","+
                                    "\"AlleyEN\":\""+ReplaceDoubleQuote(base.AlleyEN) +"\","+
                                    "\"RoadEN\":\""+ReplaceDoubleQuote(base.RoadEN) +"\","+
                                    "\"SubDistrictEN\":\""+ReplaceDoubleQuote(base.SubDistrictEN) +"\","+
                                    "\"AddressFullEN\":\""+ReplaceDoubleQuote(base.AddressFullEN) +"\","+
                                    "\"AddressLC\":\""+ReplaceDoubleQuote(base.AddressLC) +"\","+
                                    "\"AlleyLC\":\""+ReplaceDoubleQuote(base.AlleyLC) +"\","+
                                    "\"RoadLC\":\""+ReplaceDoubleQuote(base.RoadLC) +"\","+
                                    "\"SubDistrictLC\":\""+ReplaceDoubleQuote(base.SubDistrictLC) +"\","+
                                    "\"AddressFullLC\":\""+ReplaceDoubleQuote(base.AddressFullLC) +"\","+
                                    "\"DistrictCode\":\""+ReplaceDoubleQuote(base.DistrictCode) +"\","+
                                    "\"ProvinceCode\":\""+ReplaceDoubleQuote(base.ProvinceCode) +"\","+
                                    "\"ZipCode\":\""+ReplaceDoubleQuote(base.ZipCode)+"\""
                            +"}";

                return srtObject;
            
            
            }
        }

    }
}
