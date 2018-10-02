using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Controllers
{
    //
    // GET: /SwtCMP070/

    public class SwtCMP070Controller : SwtCommonController {
        public string index() {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2_1());
            lst.Add(Case2_2());
            lst.Add(Case3());
            lst.Add(Case4());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose:
        ///     Mandatory check
        ///     
        ///Parameters:
        ///     strRunningNo: NULL
        ///         
        ///Expected:
        ///     MSG0007: "These field was required: RunningNo."
        ///</summary>
        public string Case1() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            string strRunningNo = null;
            string expected = "MSG0007";
            string actual;

            try {
                actual = target.GenerateCheckDigit(strRunningNo);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
            } catch (Exception ex) {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Output check
        ///     
        ///Parameters:
        ///     strRunningNo: Try to input strRunningNo as several random numbers of length 6 and 9.
        ///         
        ///Expected:
        ///     iCheckDigit as 1 digit number.
        ///</summary>
        public string Case2_1() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            string strRunningNo = "793748";
            int expected = 1;
            int actual;

            try {
                actual = target.GenerateCheckDigit(strRunningNo).Length;
            } catch (ApplicationErrorException ex) {
                actual = -1;
            } catch (Exception ex) {
                actual = -1;
            }

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_int(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Output check
        ///     
        ///Parameters:
        ///     strRunningNo: Try to input strRunningNo as several random numbers of length 6 and 9.
        ///         
        ///Expected:
        ///     iCheckDigit as 1 digit number.
        ///</summary>
        public string Case2_2() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            string strRunningNo = "384790263";
            int expected = 1;
            int actual;

            try {
                actual = target.GenerateCheckDigit(strRunningNo).Length;
            } catch (ApplicationErrorException ex) {
                actual = -1;
            } catch (Exception ex) {
                actual = -1;
            }

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_int(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Consistency check
        ///     
        ///Parameters:
        ///     strRunningNo: Input strRunningNo of length 6. Use same input for both times.
        ///         
        ///Expected:
        ///     iCheckDigit as 1 digit number. The output must be the same for both times.
        ///</summary>
        public string Case3() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            string strRunningNo = "637482";
            string expected;
            string actual;

            try {
                actual = target.GenerateCheckDigit(strRunningNo);
                expected = target.GenerateCheckDigit(strRunningNo);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
                expected = "-2";
            } catch (Exception ex) {
                actual = ex.StackTrace;
                expected = "-2";
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose:
        ///     Consistency check
        ///     
        ///Parameters:
        ///     strRunningNo: Input strRunningNo of length 9. Use same input for both times.
        ///         
        ///Expected:
        ///     iCheckDigit as 1 digit number. The output must be the same for both times.
        ///</summary>
        public string Case4() {
            ICommonHandler target = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            string strRunningNo = "984756341";
            string expected;
            string actual;

            try {
                actual = target.GenerateCheckDigit(strRunningNo);
                expected = target.GenerateCheckDigit(strRunningNo);
            } catch (ApplicationErrorException ex) {
                actual = ex.ErrorResult.Message.Code;
                expected = "-2";
            } catch (Exception ex) {
                actual = ex.StackTrace;
                expected = "-2";
            }

            return string.Format(RESULT_FORMAT, 5, expected, actual, CompareResult_String(expected, actual));
        }
    }
}