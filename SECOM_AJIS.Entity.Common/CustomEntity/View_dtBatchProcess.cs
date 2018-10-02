using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Common
{
    public class View_dtBatchProcess : dtBatchProcess
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

        public string BatchStatusName { set; get; }
        public string BatchLastResultName { set; get; }
        public string BatchLastRunTimeEXT
        {
            get
            {
                if (BatchLastRunTime != null)
                {
                    return BatchLastRunTime.ToString("dd-MMM-yyyy HH:mm:ss");
                }
                else
                {
                    return "";
                }

            }

        }

        string srtObject = "";

        // Create JSON object
        public string Object
        {
            get
            {
                srtObject = "{" +
                                "\"BatchStatus\":\"" + ReplaceDoubleQuote(this.BatchStatus) + "\"," +
                                "\"dbms_job_status\":\"" + ReplaceDoubleQuote(this.dbms_job_status) + "\"," +
                                "\"EnableRun\":\"" + ReplaceDoubleQuote(this.EnableRun) + "\"," +
                                "\"BatchCode\":\"" + ReplaceDoubleQuote(this.BatchCode) + "\"," +
                                "\"BatchName\":\"" + ReplaceDoubleQuote(this.BatchName) + "\"," +

                                "\"Total\":\"" + ReplaceDoubleQuote(this.Total) + "\"," +
                                "\"Complete\":\"" + ReplaceDoubleQuote(this.Complete) + "\"," +
                                "\"Fail\":\"" + ReplaceDoubleQuote(this.Fail) + "\"," +

                                "\"BatchDescription\":\"" + ReplaceDoubleQuote(this.BatchDescription) + "\"," +
                                "\"BatchLastResult\":\"" + ReplaceDoubleQuote(this.BatchLastResult) + "\"," +
                                "\"BatchJobName\":\"" + ReplaceDoubleQuote(this.BatchJobName) + "\"" +  // don't forget remove commra(,) at last line

                            "}";

                return srtObject;
            }

        }
    }
}
