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
    public class SwtCTP015Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP015/

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
        ///Purpose   : Generate sale contract OCC (last OCC is minimum number)
        ///Parameters: strContractCode = Q0000000017
        ///Expected  : MSG3026: Cannot generate the sale contract occurrence. The number reach minimum.
        ///</summary>
        public string Case1()
        {
            ISaleContractHandler target = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            string strContractCode = "Q0000000017";
            string expected = "MSG3026";
            string actual;
            
            try
            {
                actual = target.GenerateContractOCC(strContractCode);
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
        ///Purpose   : Generate sale contract OCC
        ///Parameters: strContractCode = Q0000000022
        ///Expected  : Return OCC that is 9970.
        ///</summary>
        public string Case2()
        {
            ISaleContractHandler target = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            string strContractCode = "Q0000000022";
            string expected = "9970";
            string actual;

            try
            {
                actual = target.GenerateContractOCC(strContractCode);
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
        ///Purpose   : Generate sale contract OCC
        ///Parameters: strContractCode = Q0000000038
        ///Expected  : Return OCC that is 9990.
        ///</summary>
        public string Case3()
        {
            ISaleContractHandler target = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            string strContractCode = "Q0000000038";
            string expected = "9990";
            string actual;

            try
            {
                actual = target.GenerateContractOCC(strContractCode);
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
