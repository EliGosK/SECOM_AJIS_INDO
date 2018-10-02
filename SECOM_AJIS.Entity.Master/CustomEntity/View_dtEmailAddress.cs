using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;


namespace SECOM_AJIS.DataEntity.Master
{
    public class View_dtEmailAddress : dtEmailAddress
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
              
                
                srtObject = "{"+

                                "\"EmpNo\":\"" + ReplaceDoubleQuote(this.EmpNo) + "\"," +
                                "\"EmpNameEN\":\"" + ReplaceDoubleQuote(this.EmpNameEN) + "\"," +
                                "\"EmpNameLC\":\"" + ReplaceDoubleQuote(this.EmpNameLC) + "\"," +
                                "\"EmailAddress\":\"" + ReplaceDoubleQuote(this.EmailAddress) + "\"," +
                                "\"OfficeCode\":\"" + ReplaceDoubleQuote(this.OfficeCode) + "\"," +
                                "\"DepartmentCode\":\"" + ReplaceDoubleQuote(this.DepartmentCode) + "\"" +

                            "}"
                            ;



                return srtObject;

            }
        }

     
    }
}
