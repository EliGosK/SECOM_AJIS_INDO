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
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Controllers
{
    public class SwtQUP030Controller : SwtCommonController
    {
        //
        // GET: /SwtQUP030/

        //public const string RESULT_FORMAT_LIST = "Case: {0}, Expected size: {1}, Actual size: {2}, Result: {3} <br>";

        public string index()
        {
            //Using in write log process
            CommonUtil.dsTransData = new dsTransDataModel();
            CommonUtil.dsTransData.dtUserData = new UserDataDo();
            CommonUtil.dsTransData.dtOperationData = new OperationDataDo();
            CommonUtil.dsTransData.dtTransHeader = new TransHeaderDo();
            
            //Login user = 510729
            //Process datetime = 2011/12/03  06:15:00 PM
            CommonUtil.dsTransData.dtUserData.EmpNo = "510729";
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 12, 3, 18, 15, 00);
            CommonUtil.dsTransData.dtTransHeader.ScreenID = "QUP030"; 
            
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
            lst.Add(Case12());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose   : Mandatory check (QuotationHandler.LockQuotation)
        ///Parameters:  strQuotationTargetCode = NULL
        ///             strAlphabet = NULL
        ///             strLockStyleCode = NULL
        ///Expected  : MSG0007: These field was required: strLockStyleCode.
        ///</summary>
        public string Case1()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = null;
            string strAlphabet = null;
            string strLockStyleCode = null;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.LockQuotation(strQuotationTargetCode, strAlphabet, strLockStyleCode);
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
        ///Purpose   : Mandatory check (QuotationHandler.CountQuotationBasic)
        ///Parameters:  strQuotationTargetCode = NULL
        ///             strAlphabet = NULL
        ///             strLockStyleCode = 1
        ///Expected  : MSG0007: These field was required: strQuotationTargetCode.
        ///</summary>
        public string Case2()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = null;
            string strAlphabet = null;
            string strLockStyleCode = "1";
            string expected = "MSG0007";
            string actual;

            try
            {
                target.LockQuotation(strQuotationTargetCode, strAlphabet, strLockStyleCode);
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

            return string.Format(RESULT_FORMAT, 2, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Get quotation basic (How does the system perform if it cannot get records from tbt_QuotationBasic)
        ///Parameters:  strQuotationTargetCode = FN0000000385
        ///             strAlphabet = NULL
        ///             strLockStyleCode = 1
        ///Expected  : MSG2012: Not exist quotation detail for specified quotation target code, FN0000385.
        ///</summary>
        public string Case3()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "FN0000000385";
            string strAlphabet = null;
            string strLockStyleCode = "1";
            string expected = "MSG2012";
            string actual;

            try
            {
                target.LockQuotation(strQuotationTargetCode, strAlphabet, strLockStyleCode);
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

            return string.Format(RESULT_FORMAT, 3, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Mandatory check (QuotationHandler.LockAll)
        ///Pre-Condition :  Create stub1 for QuotationHandler.LockQuotation to execute QuotationHandler.LockAll directly. 
        ///Parameters:  strQuotationTargetCode = NULL
        ///             strAlphabet = NULL
        ///             strLockStyleCode = NULL
        ///Expected  : MSG0007: These field was required: strQuotationTargetCode.
        ///</summary>
        public string Case4()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = null;
            //string strAlphabet = null;
            //string strLockStyleCode = null;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.LockAll(strQuotationTargetCode);
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

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Mandatory check (QuotationHandler.LockBackward)
        ///Pre-Condition :  Create stub2 for QuotationHandler.LockQuotation to execute QuotationHandler.LockBackward directly 
        ///Parameters:  strQuotationTargetCode = NULL
        ///             strAlphabet = NULL
        ///             strLockStyleCode = NULL
        ///Expected  : MSG0007: These field was required: strQuotationTargetCode, strAlphabet
        ///</summary>
        public string Case5()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = null;
            string strAlphabet = null;
            //string strLockStyleCode = null;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.LockBackward(strQuotationTargetCode, strAlphabet);
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

            return string.Format(RESULT_FORMAT, 5, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Mandatory check (QuotationHandler.LockIndividual)
        ///Pre-Condition :  Create stub3 for QuotationHandler.LockQuotation to execute QuotationHandler.LockIndividual directly
        ///Parameters:  strQuotationTargetCode = NULL
        ///             strAlphabet = NULL
        ///             strLockStyleCode = NULL
        ///Expected  : MSG0007: These field was required: strQuotationTargetCode, strAlphabet
        ///</summary>
        public string Case6()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = null;
            string strAlphabet = null;
            //string strLockStyleCode = null;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.LockIndividual(strQuotationTargetCode, strAlphabet);
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

            return string.Format(RESULT_FORMAT, 6, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Lock all (no quotation was locked)
        ///Pre-Condition :  Create stub4 to test expectation input for LogHandler.WriteTransactionLog
        ///Parameters:  strQuotationTargetCode =  FMA0000000486
        ///             strAlphabet = NULL
        ///             strLockStyleCode = 1
        ///Expected  : See expectation test case7
        ///</summary>
        public string Case7()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "FMA0000000486";
            string strAlphabet = null;
            string strLockStyleCode = "1";
            string expected = "True";
            string actual;
            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case7";
                bool result = target.LockQuotation(strQuotationTargetCode, strAlphabet, strLockStyleCode);
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

            return string.Format(RESULT_FORMAT, 7, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Backward lock (no quotation was locked)
        ///Pre-Condition :  Create stub5 to test expectation input for LogHandler.WriteTransactionLog
        ///Parameters:  strQuotationTargetCode =  N0000000492
        ///             strAlphabet = "CA"
        ///             strLockStyleCode = 2
        ///Expected  : See expectation test case8
        ///</summary>
        public string Case8()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "N0000000492";
            string strAlphabet = "CA";
            string strLockStyleCode = "2";
            string expected = "True";
            string actual;
            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case8";
                bool result = target.LockQuotation(strQuotationTargetCode, strAlphabet, strLockStyleCode);
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

            return string.Format(RESULT_FORMAT, 8, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Individual lock (no quotation was locked)
        ///Pre-Condition :  Create stub6 to test expectation input for LogHandler.WriteTransactionLog
        ///Parameters:  strQuotationTargetCode =  FQ0000000508
        ///             strAlphabet = "CA"
        ///             strLockStyleCode = 3
        ///Expected  : See expectation test case9
        ///</summary>
        public string Case9()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "FQ0000000508";
            string strAlphabet = "CA";
            string strLockStyleCode = "3";
            string expected = "True";
            string actual;
            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case9";
                bool result = target.LockQuotation(strQuotationTargetCode, strAlphabet, strLockStyleCode);
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

            return string.Format(RESULT_FORMAT, 9, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Lock all (there are some quotations was locked)
        ///Pre-Condition :  Create stub6 to test expectation input for LogHandler.WriteTransactionLog
        ///Parameters:  strQuotationTargetCode =  N0000000492
        ///             strAlphabet = NULL
        ///             strLockStyleCode = 1
        ///Expected  : See expectation test case10
        ///</summary>
        public string Case10()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "N0000000492";
            string strAlphabet = null;
            string strLockStyleCode = "1";
            string expected = "True";
            string actual;
            try
            {
                //Login user = 470228
                //Process datetime = 2012/01/15  09:30:00 AM
                CommonUtil.dsTransData.dtUserData.EmpNo = "470228";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2012, 1, 15, 9, 30, 00);

                CommonUtil.dsTransData.dtOperationData.GUID = "Case10";
                bool result = target.LockQuotation(strQuotationTargetCode, strAlphabet, strLockStyleCode);
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

            return string.Format(RESULT_FORMAT, 10, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Backward lock  (there are some quotations was locked)
        ///Pre-Condition :  Create stub6 to test expectation input for LogHandler.WriteTransactionLog
        ///Parameters:  strQuotationTargetCode =   FQ0000000508
        ///             strAlphabet = CB
        ///             strLockStyleCode = 2
        ///Expected  : See expectation test case11
        ///</summary>
        public string Case11()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "FQ0000000508";
            string strAlphabet = "CB";
            string strLockStyleCode = "2";
            string expected = "True";
            string actual;
            try
            {
                //Login user = 470228
                //Process datetime = 2012/01/15  09:30:00 AM
                CommonUtil.dsTransData.dtUserData.EmpNo = "470228";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2012, 1, 15, 9, 30, 00);

                CommonUtil.dsTransData.dtOperationData.GUID = "Case11";
                bool result = target.LockQuotation(strQuotationTargetCode, strAlphabet, strLockStyleCode);
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

            return string.Format(RESULT_FORMAT, 11, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Individual lock (all quotations was locked)
        ///Pre-Condition :  Create stub6 to test expectation input for LogHandler.WriteTransactionLog
        ///Parameters:  strQuotationTargetCode =   FMA0000000486
        ///             strAlphabet = CA
        ///             strLockStyleCode = 3
        ///Expected  :  No data in DB is updated. LogHandler.WriteTransactionLog is not called.
        ///</summary>
        public string Case12()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            string strQuotationTargetCode = "FMA0000000486";
            string strAlphabet = "CA";
            string strLockStyleCode = "3";
            string expected = "False";
            string actual;
            try
            {
                //Login user = 470228
                //Process datetime = 2012/01/15  09:30:00 AM
                CommonUtil.dsTransData.dtUserData.EmpNo = "470228";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2012, 1, 15, 9, 30, 00);

                CommonUtil.dsTransData.dtOperationData.GUID = "Case12";
                bool result = target.LockQuotation(strQuotationTargetCode, strAlphabet, strLockStyleCode);
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

            return string.Format(RESULT_FORMAT, 12, expected, actual, CompareResult_String(expected, actual));
        }

    }
}
