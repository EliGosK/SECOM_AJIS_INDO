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

namespace SECOM_AJIS.Controllers
{
    public class SwtQUP021Controller : SwtCommonController
    {
        //
        // GET: /SwtQUP021/

        public string index()
        {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());
            lst.Add(Case3());
            lst.Add(Case4());
            lst.Add(Case5());
            lst.Add(Case6());
            lst.Add(Case7());
            lst.Add(Case8());
            lst.Add(Case9());
            lst.Add(Case10());
            lst.Add(Case11());
            
            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");
            
            return result;
        }

        ///<summary>
        ///Purpose   : Mandatory check
        ///Parameters: strQuotationTargetCode = NULL
        ///Expected  : MSG0007: These field was required: strProductTypeCode
        ///</summary>
        public string Case1()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = null;
            string expected = "MSG0007";
            string actual;
            
            try
            {
                actual = target.GenerateQuotationAlphabet(strQuotationTargetCode);
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
        ///Purpose   : Get quotation target (How does the system perform if it cannot get a record from tbt_QuotationTarget)
        ///Parameters: strQuotationTargetCode = FN0000000144
        ///Expected  : MSG2003: Quotation target not found, FN0000144.
        ///</summary>
        public string Case2()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "FN0000000144";
            string expected = "MSG2003";
            string actual;
            try
            {
                actual = target.GenerateQuotationAlphabet(strQuotationTargetCode);
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
        ///Purpose   : Generate new alphabet (How does the system perform if number of digits of the last alphabet are less than rule which is defined - left space)
        ///Parameters: strQuotationTargetCode = FQ0000000438
        ///Expected  : MSG2008: Cannot generate the alphabet. Digit count <> 2.
        ///</summary>
        public string Case3()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strProductTypeCode = "FQ0000000438";
            string expected = "MSG2008";
            string actual;
            try
            {
                actual = target.GenerateQuotationAlphabet(strProductTypeCode);
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
        ///Purpose   : Generate new alphabet (How does the system perform if number of digits of the last alphabet are less than rule which is defined - right space )
        ///Parameters: strQuotationTargetCode = Q0000000471
        ///Expected  : MSG2008: Cannot generate the alphabet. Digit count <> 2.
        ///</summary>
        public string Case4()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "Q0000000471";
            string expected = "MSG2008";
            string actual;
            try
            {
                actual = target.GenerateQuotationAlphabet(strQuotationTargetCode);
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
        ///Purpose   : Generate new alphabet (How does the system perform if characters composed to the last alphabet do not conform to the rule which is defined)
        ///Parameters: strQuotationTargetCode = FSG0000000442
        ///Expected  : MSG2009: Cannot generate the alphabet. Invalid character is found.
        ///</summary>
        public string Case5()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "FSG0000000442";
            string expected = "MSG2009";
            string actual;
            try
            {
                actual = target.GenerateQuotationAlphabet(strQuotationTargetCode);
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
        ///Purpose   : Generate new alphabet (How does the system perform if  the last alphabet is the maximum boundary.)
        ///Parameters: strQuotationTargetCode = FMA0000000083
        ///Expected  : MSG2010: Cannot generate the alphabet. The top boundary of the alphabet is reached.
        ///</summary>
        public string Case6()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "FMA0000000083";
            string expected = "MSG2010";
            string actual;
            try
            {
                actual = target.GenerateQuotationAlphabet(strQuotationTargetCode);
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

        ///<summary>
        ///Purpose   : Generate new alphabet (How does the system perform if  the last alphabet is null.)
        ///Parameters: strQuotationTargetCode = FN0000000425
        ///Expected  : strAlphabet = AA
        ///</summary>
        public string Case7()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "FN0000000425";
            string expected = "AA";
            string actual;
            try
            {
                actual = target.GenerateQuotationAlphabet(strQuotationTargetCode);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 7, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate new alphabet (How does the system perform if  the last alphabet is AA.)
        ///Parameters: strQuotationTargetCode = MA0000000164
        ///Expected  : strAlphabet = AB
        ///</summary>
        public string Case8()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "MA0000000164";
            string expected = "AB";
            string actual;
            try
            {
                actual = target.GenerateQuotationAlphabet(strQuotationTargetCode);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 8, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate new alphabet (How does the system perform if  the last alphabet is BZ.)
        ///Parameters: strQuotationTargetCode = N0000000457
        ///Expected  : strAlphabet = CA
        ///</summary>
        public string Case9()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "N0000000457";
            string expected = "CA";
            string actual;
            try
            {
                actual = target.GenerateQuotationAlphabet(strQuotationTargetCode);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 9, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate new alphabet (How does the system perform if  the last alphabet is WE.)
        ///Parameters: strQuotationTargetCode = FMA0000000097
        ///Expected  : strAlphabet = WF
        ///</summary>
        public string Case10()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "FMA0000000097";
            string expected = "WF";
            string actual;
            try
            {
                actual = target.GenerateQuotationAlphabet(strQuotationTargetCode);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 10, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Generate new alphabet (How does the system perform if  the last alphabet is ZY.)
        ///Parameters: strQuotationTargetCode = SG0000000463
        ///Expected  : strAlphabet = ZZ
        ///</summary>
        public string Case11()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "SG0000000463";
            string expected = "ZZ";
            string actual;
            try
            {
                actual = target.GenerateQuotationAlphabet(strQuotationTargetCode);
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 11, expected, actual, CompareResult_String(expected, actual));
        }
    }
}
