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
    public class SwtCTP010Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP010/

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

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");
            
            return result;
        }

        ///<summary>
        ///Purpose   : Get product type (How does the system performs if it cannot get a record from tbs_ProductType)
        ///Parameters: strProductTypeCode = 99
        ///Expected  : MSG3022: Cannot generate the contract code. Contract prefix does not exist.
        ///</summary>
        public string Case1()
        {
            IContractHandler target = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            string strProductTypeCode = "99";
            string expected = "MSG3022";
            string actual;
            
            try
            {
                actual = target.GenerateContractCode(strProductTypeCode);
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
        ///Purpose   : Get product type (How does the system performs if the quotation prefix is null)
        ///Parameters: strProductTypeCode = 7
        ///Expected  : MSG3022: Cannot generate the contract code. Contract prefix does not exist.
        ///</summary>
        public string Case2()
        {
            IContractHandler target = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            string strProductTypeCode = "7";
            string expected = "MSG3022";
            string actual;
            try
            {
                actual = target.GenerateContractCode(strProductTypeCode);
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
        ///Purpose   : Get product type (How does the system performs if the quotation prefix is blank)
        ///Parameters: strProductTypeCode = 8
        ///Expected  : MSG3022: Cannot generate the contract code. Contract prefix does not exist.
        ///</summary>
        public string Case3()
        {
            IContractHandler target = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            string strProductTypeCode = "8";
            string expected = "MSG3022";
            string actual;
            try
            {
                actual = target.GenerateContractCode(strProductTypeCode);
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
        ///Purpose   : Generate contract code for sale
        ///Parameters: strProductTypeCode = 1
        ///Expected  : Return contract code that is Q000000001x.
        ///</summary>
        public string Case4()
        {
            IContractHandler target = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            string strProductTypeCode = "1";
            string expected = "Q000000001x";
            string actual;
            string com_actual = string.Empty;
            try
            {
                actual = target.GenerateContractCode(strProductTypeCode);
                com_actual = actual.Substring(0, actual.Length - 1) + "x";
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, com_actual));
        }

        ///<summary>
        ///Purpose   : Generate contract code for alarm
        ///Parameters: strProductTypeCode = 2
        ///Expected  : Return contract code that is N000000002x.
        ///</summary>
        public string Case5()
        {
            IContractHandler target = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            string strProductTypeCode = "2";
            string expected = "N000000002x";
            string actual;
            string com_actual = string.Empty;
            try
            {
                actual = target.GenerateContractCode(strProductTypeCode);
                com_actual = actual.Substring(0, actual.Length - 1) + "x";
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 5, expected, actual, CompareResult_String(expected, com_actual));
        }

        ///<summary>
        ///Purpose   : Generate contract code for sale online
        ///Parameters: strProductTypeCode = 3
        ///Expected  : Return contract code that is N000000003x.
        ///</summary>
        public string Case6()
        {
            IContractHandler target = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            string strProductTypeCode = "3";
            string expected = "N000000003x";
            string actual;
            string com_actual = string.Empty;
            try
            {
                actual = target.GenerateContractCode(strProductTypeCode);
                com_actual = actual.Substring(0, actual.Length - 1) + "x";
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 6, expected, actual, CompareResult_String(expected, com_actual));
        }

        ///<summary>
        ///Purpose   : Generate contract code for beat guard
        ///Parameters: strProductTypeCode = 4
        ///Expected  : Return contract code that is SG000000004x.
        ///</summary>
        public string Case7()
        {
            IContractHandler target = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            string strProductTypeCode = "4";
            string expected = "SG000000004x";
            string actual;
            string com_actual = string.Empty;
            try
            {
                actual = target.GenerateContractCode(strProductTypeCode);
                com_actual = actual.Substring(0, actual.Length - 1) + "x";
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 7, expected, actual, CompareResult_String(expected, com_actual));
        }

        ///<summary>
        ///Purpose   : Generate contract code for sentry guard
        ///Parameters: strProductTypeCode = 5
        ///Expected  : Return contract code that is SG000000005x.
        ///</summary>
        public string Case8()
        {
            IContractHandler target = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            string strProductTypeCode = "5";
            string expected = "SG000000005x";
            string actual;
            string com_actual = string.Empty;
            try
            {
                actual = target.GenerateContractCode(strProductTypeCode);
                com_actual = actual.Substring(0, actual.Length - 1) + "x";
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 8, expected, actual, CompareResult_String(expected, com_actual));
        }

        ///<summary>
        ///Purpose   : Generate contract code for maintenane
        ///Parameters: strProductTypeCode = 6
        ///Expected  : Return contract code that is MA000000006x.
        ///</summary>
        public string Case9()
        {
            IContractHandler target = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            string strProductTypeCode = "6";
            string expected = "MA000000006x";
            string actual;
            string com_actual = string.Empty;
            try
            {
                actual = target.GenerateContractCode(strProductTypeCode);
                com_actual = actual.Substring(0, actual.Length - 1) + "x";
            }
            catch (ApplicationErrorException ex)
            {
                actual = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                actual = ex.StackTrace;
            }

            return string.Format(RESULT_FORMAT, 9, expected, actual, CompareResult_String(expected, com_actual));
        }
    }
}
