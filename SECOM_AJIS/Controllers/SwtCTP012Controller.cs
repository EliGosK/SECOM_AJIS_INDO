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
    public class SwtCTP012Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP012/

        public string index()
        {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());
            lst.Add(Case3());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");
            
            return result;
        }

        ///<summary>
        ///Purpose   : Generate rental contract counter (last counter is maximum number)
        ///Parameters: strContractCode = N0000000017
        ///Expected  : MSG3024: Cannot generate the contract counter. The number reach maximum.
        ///</summary>
        public string Case1()
        {
            IRentralContractHandler target = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            string strContractCode = "N0000000017";
            string expected = "MSG3024";
            string actual;
            
            try
            {
                target.GenerateContractCounter(strContractCode);
                actual = string.Empty;
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
        ///Purpose   : Generate rental contract counter
        ///Parameters: strContractCode = N0000000017
        ///Expected  : Return counter that is 1.
        ///</summary>
        public string Case2()
        {
            IRentralContractHandler target = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            string strContractCode = "N0000000022";
            string expected = "1";
            string actual;

            try
            {
                int result = target.GenerateContractCounter(strContractCode);
                actual = result.ToString();
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
        ///Purpose   : Generate rental contract counter
        ///Parameters: strContractCode = N0000000038
        ///Expected  : Return counter that is 2.
        ///</summary>
        public string Case3()
        {
            IRentralContractHandler target = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            string strContractCode = "N0000000038";
            string expected = "2";
            string actual;

            try
            {
                int result = target.GenerateContractCounter(strContractCode);
                actual = result.ToString();
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
    }
}
