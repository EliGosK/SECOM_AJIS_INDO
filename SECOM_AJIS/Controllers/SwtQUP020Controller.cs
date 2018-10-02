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
    public class SwtQUP020Controller : SwtCommonController
    {
        //
        // GET: /SwtQUP020/

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
        ///Purpose   : Mandatory check
        ///Parameters: strProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: strProductTypeCode
        ///</summary>
        public string Case1()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strProductTypeCode = null;
            string expected = "MSG0007";
            string actual;
            
            try
            {
                actual = target.GenerateQuotationTargetCode(strProductTypeCode);
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
        ///Purpose   : Get product type (How does the system perform if it cannot get a record from tbs_ProductType)
        ///Parameters: strProductTypeCode = 99
        ///Expected  : MSG2011: Cannot generate the quotation target code. Quotation prefix does not exist.
        ///</summary>
        public string Case2()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strProductTypeCode = "99";
            string expected = "MSG2011";
            string actual;
            try
            {
                actual = target.GenerateQuotationTargetCode(strProductTypeCode);
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
        ///Purpose   : Get product type (How does the system perform if the quotation prefix is null)
        ///Parameters: strProductTypeCode = 7
        ///Expected  : MSG2011: Cannot generate the quotation target code. Quotation prefix does not exist.
        ///</summary>
        public string Case3()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strProductTypeCode = "7";
            string expected = "MSG2011";
            string actual;
            try
            {
                actual = target.GenerateQuotationTargetCode(strProductTypeCode);
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
        ///Purpose   : Get product type (How does the system perform if the quotation prefix is blank)
        ///Parameters: strProductTypeCode = 8
        ///Expected  : MSG2011: Cannot generate the quotation target code. Quotation prefix does not exist.
        ///</summary>
        public string Case4()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strProductTypeCode = "8";
            string expected = "MSG2011";
            string actual;
            try
            {
                actual = target.GenerateQuotationTargetCode(strProductTypeCode);
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
        ///Purpose   : Compose quotation target code for sale quotation
        ///Pre-Condition :  1. CommonHandler.GetNextRunningNo   Return: 000000001
        ///                 2. CommonHandler.GenerateCheckDigit Return: 7
        ///Parameters: strProductTypeCode = 1
        ///Expected  : strQuotationTargetCode = FQ0000000017
        ///</summary>
        public string Case5()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strProductTypeCode = "1";
            string expected = "FQ0000000017";
            string actual;
            try
            {
                actual = target.GenerateQuotationTargetCode(strProductTypeCode);
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
        ///Purpose   : Compose quotation target code for maintenance quotation
        ///Pre-Condition :  1. CommonHandler.GetNextRunningNo   Return: 000000208
        ///                 2. CommonHandler.GenerateCheckDigit Return: 3
        ///Parameters: strProductTypeCode = 6
        ///Expected  : strQuotationTargetCode = FMA0000002083
        ///</summary>
        public string Case6()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strProductTypeCode = "6";
            string expected = "FMA0000002083";
            string actual;
            try
            {
                actual = target.GenerateQuotationTargetCode(strProductTypeCode);
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
