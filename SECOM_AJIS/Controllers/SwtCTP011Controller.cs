using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SECOM_AJIS.DataEntity.Quotation;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using System.Diagnostics;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Controllers
{
    public class SwtCTP011Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP011/

        public string index()
        {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());
            lst.Add(Case3());
            lst.Add(Case4());
            lst.Add(Case5());
            lst.Add(Case6());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");
            
            return result;
        }

        ///<summary>
        ///Purpose   : Generate rental contract OCC (last OCC is minimum number)
        ///Parameters: strContractCode = N0000000022
        ///            bImplementFlag = TRUE
        ///Expected  : MSG3023: Cannot generate the security occurrence. The number reach minimum.
        ///</summary>
        public string Case1()
        {
            IRentralContractHandler target = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            string strContractCode = "N0000000022";
            bool bImplementFlag = true;
            string expected = "MSG3023";
            string actual;
            
            try
            {
                actual = target.GenerateContractOCC(strContractCode, bImplementFlag);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate rental contract OCC
        ///Parameters: strContractCode = N0000000017
        ///            bImplementFlag = FALSE
        ///Expected  : Return OCC that is 0002.
        ///</summary>
        public string Case2()
        {
            IRentralContractHandler target = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            string strContractCode = "N0000000017";
            bool bImplementFlag = false;
            string expected = "0002";
            string actual;

            try
            {
                actual = target.GenerateContractOCC(strContractCode, bImplementFlag);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate rental contract OCC
        ///Parameters: strContractCode = N0000000017
        ///            bImplementFlag = TRUE
        ///Expected  : Return OCC that is 9960.
        ///</summary>
        public string Case3()
        {
            IRentralContractHandler target = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            string strContractCode = "N0000000017";
            bool bImplementFlag = true;
            string expected = "9960";
            string actual;

            try
            {
                actual = target.GenerateContractOCC(strContractCode, bImplementFlag);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate rental contract OCC
        ///Parameters: strContractCode = N0000000038
        ///            bImplementFlag = TRUE
        ///Expected  : Return OCC that is 9900.
        ///</summary>
        public string Case4()
        {
            IRentralContractHandler target = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            string strContractCode = "N0000000038";
            bool bImplementFlag = true;
            string expected = "9950";
            string actual;

            try
            {
                actual = target.GenerateContractOCC(strContractCode, bImplementFlag);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate rental contract OCC
        ///Parameters: strContractCode = N0000000041
        ///            bImplementFlag = FALSE
        ///Expected  : Return OCC that is 0001.
        ///</summary>
        public string Case5()
        {
            IRentralContractHandler target = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            string strContractCode = "N0000000041";
            bool bImplementFlag = false;
            string expected = "0001";
            string actual;

            try
            {
                actual = target.GenerateContractOCC(strContractCode, bImplementFlag);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 5, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate rental contract OCC
        ///Parameters: strContractCode = N0000000041
        ///            bImplementFlag = TRUE
        ///Expected  : Return OCC that is 9990.
        ///</summary>
        public string Case6()
        {
            IRentralContractHandler target = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            string strContractCode = "N0000000041";
            bool bImplementFlag = true;
            string expected = "9990";
            string actual;

            try
            {
                actual = target.GenerateContractOCC(strContractCode, bImplementFlag);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 6, expected, actual, CompareResult_String(expected, actual));
        }
    }
}
