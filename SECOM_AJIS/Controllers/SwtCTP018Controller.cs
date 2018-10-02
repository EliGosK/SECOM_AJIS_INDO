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
    public class SwtCTP018Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP018/

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
        ///Purpose   : Convert to short contract code
        ///Parameters: strContractCode = N0000000017
        ///Expected  : Return contract code that is N0000017.
        ///</summary>
        public string Case1()
        {
            CommonUtil target = new CommonUtil();
            string strContractCode = "N0000000017";
            string expected = "N0000017";
            string actual;
            
            try
            {
                actual = target.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
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
        ///Purpose   : Convert to short contract code
        ///Parameters: strContractCode = MA0000000017
        ///Expected  : Return contract code that is MA0000017.
        ///</summary>
        public string Case2()
        {
            CommonUtil target = new CommonUtil();
            string strContractCode = "MA0000000017";
            string expected = "MA0000017";
            string actual;

            try
            {
                actual = target.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
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
        ///Purpose   : Convert to short contract code
        ///Parameters: strContractCode = N0000000017-0001-01
        ///Expected  : Return contract code that is N0000017-0001-01.
        ///</summary>
        public string Case3()
        {
            CommonUtil target = new CommonUtil();
            string strContractCode = "N0000000017-0001-01";
            string expected = "N0000017-0001-01";
            string actual;

            try
            {
                actual = target.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
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
        ///Purpose   : Convert to long contract code
        ///Parameters: strContractCode = N0000017
        ///Expected  : Return contract code that is N0000000017.
        ///</summary>
        public string Case4()
        {
            CommonUtil target = new CommonUtil();
            string strContractCode = "N0000017";
            string expected = "N0000000017";
            string actual;

            try
            {
                actual = target.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
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
        ///Purpose   : Convert to long contract code
        ///Parameters: strContractCode = MA0000017
        ///Expected  : Return contract code that is MA0000000017.
        ///</summary>
        public string Case5()
        {
            CommonUtil target = new CommonUtil();
            string strContractCode = "MA0000017";
            string expected = "MA0000000017";
            string actual;

            try
            {
                actual = target.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
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
        ///Purpose   : Convert to long contract code
        ///Parameters: strContractCode = N0000017-0001-01
        ///Expected  : Return contract code that is N0000000017-0001-01.
        ///</summary>
        public string Case6()
        {
            CommonUtil target = new CommonUtil();
            string strContractCode = "N0000017-0001-01";
            string expected = "N0000000017-0001-01";
            string actual;

            try
            {
                actual = target.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
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
