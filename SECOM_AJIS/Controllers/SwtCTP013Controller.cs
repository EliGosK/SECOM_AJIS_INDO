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
    public class SwtCTP013Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP013/

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
        ///Purpose   : Generate contract doc OCC (last doc OCC is maximum number)
        ///Parameters: strCode = N0000000017
        ///            strOCC = 0001
        ///Expected  : MSG3025: Cannot generate the contract document occurrence. The number reach maximum.
        ///</summary>
        public string Case1()
        {
            IContractDocumentHandler target = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            string strCode = "N0000000017";
            string strOCC = "0001";
            string expected = "MSG3025";
            string actual;
            
            try
            {
                actual = target.GenerateDocOCC(strCode, strOCC);
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
        ///Purpose   : Generate contract doc OCC
        ///Parameters: strCode = FN0000000017
        ///            strOCC = AA
        ///Expected  : Return contract doc occ that is 02.
        ///</summary>
        public string Case2()
        {
            IContractDocumentHandler target = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            string strCode = "FN0000000017";
            string strOCC = "AA";
            string expected = "02";
            string actual;
            
            try
            {
                actual = target.GenerateDocOCC(strCode, strOCC);
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
        ///Purpose   : Generate contract doc OCC
        ///Parameters: strCode = N0000000022
        ///            strOCC = 9990
        ///Expected  : Return contract doc occ that is 04.
        ///</summary>
        public string Case3()
        {
            IContractDocumentHandler target = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            string strCode = "N0000000022";
            string strOCC = "9990";
            string expected = "04";
            string actual;

            try
            {
                actual = target.GenerateDocOCC(strCode, strOCC);
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
        ///Purpose   : Generate contract doc OCC
        ///Parameters: strCode = N0000000038
        ///            strOCC = 9950
        ///Expected  : Return contract doc occ that is 06.
        ///</summary>
        public string Case4()
        {
            IContractDocumentHandler target = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            string strCode = "N0000000038";
            string strOCC = "9950";
            string expected = "06";
            string actual;

            try
            {
                actual = target.GenerateDocOCC(strCode, strOCC);
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
        ///Purpose   : Generate contract doc OCC
        ///Parameters: strCode = FN0000000022
        ///            strOCC = AC
        ///Expected  : Return contract doc occ that is 06.
        ///</summary>
        public string Case5()
        {
            IContractDocumentHandler target = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            string strCode = "FN0000000022";
            string strOCC = "AC";
            string expected = "06";
            string actual;

            try
            {
                actual = target.GenerateDocOCC(strCode, strOCC);
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
        ///Purpose   : Generate contract doc OCC
        ///Parameters: strCode = N0000000041
        ///            strOCC = 0001
        ///Expected  : Return contract doc occ that is 01.
        ///</summary>
        public string Case6()
        {
            IContractDocumentHandler target = ServiceContainer.GetService<IContractDocumentHandler>() as IContractDocumentHandler;
            string strCode = "N0000000041";
            string strOCC = "0001";
            string expected = "01";
            string actual;

            try
            {
                actual = target.GenerateDocOCC(strCode, strOCC);
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
