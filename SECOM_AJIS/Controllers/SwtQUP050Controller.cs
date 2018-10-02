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
    public class SwtQUP050Controller : SwtCommonController
    {
        //
        // GET: /SwtQUP050/

        public string index()
        {
            //Using in write log process
            CommonUtil.dsTransData = new dsTransDataModel();
            CommonUtil.dsTransData.dtUserData = new UserDataDo();
            CommonUtil.dsTransData.dtOperationData = new OperationDataDo();
            CommonUtil.dsTransData.dtTransHeader = new TransHeaderDo();
            CommonUtil.dsTransData.dtTransHeader.ScreenID = "QUP050";

            //Login user = 490488
            //Process datetime = 2011-11-14 09:20:00.000
            CommonUtil.dsTransData.dtUserData.EmpNo = "490488";
            CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 11, 14, 09, 20, 00);
            
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
            lst.Add(Case13());
            lst.Add(Case14());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose   : Mandatory check1 (How does the system perform if action type is not specified.)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = NULL
        ///                - Alphabet = NULL
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = NULL
        ///Expected  : MSG0007: These field was required: ActionTypeCode.
        ///</summary>
        public string Case1()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = null;
            doUpdate.Alphabet = null;
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = null;
            string expected = "MSG0007";
            string actual;

            try
            {
                int result = target.UpdateQuotationData(doUpdate);
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

            return string.Format(RESULT_FORMAT, 1, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Get quotation basic data (How does the system perform if it cannot get quotation basic data when action type = draft)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = FMA0000000482
        ///                - Alphabet = AA
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = 1
        ///Expected  : MSG2006: Quotation code not found, FMA0000482-AA.
        ///</summary>
        public string Case2()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "FMA0000000482";
            doUpdate.Alphabet = "AA";
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = "1";
            string expected = "MSG2006";
            string actual;

            try
            {
                int result = target.UpdateQuotationData(doUpdate);
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
        ///Purpose   : Get quotation basic data (How does the system perform if it cannot get quotation basic data when action type = approve)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = FQ0000000508
        ///                - Alphabet = AB
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = 2
        ///Expected  : MSG2006: Quotation code not found, FQ0000508-AB.
        ///</summary>
        public string Case3()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "FQ0000000508";
            doUpdate.Alphabet = "AB";
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = "2";
            string expected = "MSG2006";
            string actual;

            try
            {
                int result = target.UpdateQuotationData(doUpdate);
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

        ///<summary>
        ///Purpose   : Get quotation basic data (How does the system perform if it cannot get quotation basic data when action type = change)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = N0000000492
        ///                - Alphabet = BX
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = 3
        ///Expected  : MSG2006: Quotation code not found, N0000492-BX.
        ///</summary>
        public string Case4()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "N0000000492";
            doUpdate.Alphabet = "BX";
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = "3";
            string expected = "MSG2006";
            string actual;

            try
            {
                int result = target.UpdateQuotationData(doUpdate);
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

            return string.Format(RESULT_FORMAT, 4, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Get quotation basic data (How does the system perform if it cannot get quotation target data when action type = change)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = SG0000000514
        ///                - Alphabet = AB
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = 3
        ///Expected  : MSG2003: Quotation target not found, SG0000514.
        ///</summary>
        public string Case5()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "SG0000000514";
            doUpdate.Alphabet = "AB";
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = "3";
            string expected = "MSG2003";
            string actual;

            try
            {
                int result = target.UpdateQuotationData(doUpdate);
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

            return string.Format(RESULT_FORMAT, 5, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Get quotation basic data (How does the system perform if it cannot get quotation basic and target data when action type = cancel)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = MA0000000319
        ///                - Alphabet = AA
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = 4
        ///Expected  : MSG2003: Quotation target not found, MA0000319.
        ///</summary>
        public string Case6()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "MA0000000319";
            doUpdate.Alphabet = "AA";
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = "4";
            string expected = "MSG2003";
            string actual;

            try
            {
                int result = target.UpdateQuotationData(doUpdate);
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

            return string.Format(RESULT_FORMAT, 6, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Mandatory check2  (How does the system perform if action type = draft but last update date is not specified)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = FN0000000527
        ///                - Alphabet = AA
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = 1
        ///Expected  : MSG0007: These field was required: LastUpdateDate.
        ///</summary>
        public string Case7()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "FN0000000527";
            doUpdate.Alphabet = "AA";
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = "1";
            string expected = "MSG0007";
            string actual;

            try
            {
                int result = target.UpdateQuotationData(doUpdate);
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
        ///Purpose   : Update quotation target (How does the system perform if action type = draft and last updated date on screen is not equal to last update date in db)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = FN0000000527
        ///                - Alphabet = AA
        ///                - LastUpdateDate = 2011-07-15 13:00:00.000
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = 1
        ///Expected  : MSG0019: Record already updated by another user.
        ///</summary>
        public string Case8()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "FN0000000527";
            doUpdate.Alphabet = "AA";
            doUpdate.LastUpdateDate = new DateTime(2011, 7, 15, 13, 0, 0);
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = "1";
            string expected = "MSG0019";
            string actual;

            try
            {
                int result = target.UpdateQuotationData(doUpdate);
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
        ///Purpose   : Mandatory check3  (How does the system perform if action type = approve but contract code is not specified)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = FN0000000527
        ///                - Alphabet = AA
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = 2
        ///Expected  : MSG0007: These field was required: ContractCode.
        ///</summary>
        public string Case9()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "FN0000000527";
            doUpdate.Alphabet = "AA";
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = "2";
            string expected = "MSG0007";
            string actual;

            try
            {
                int result = target.UpdateQuotationData(doUpdate);
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
        ///Purpose   : Mandatory check4  (How does the system perform if action type = change but contract code is not specified)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = Q0000000536
        ///                - Alphabet = AA
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = 3
        ///Expected  : MSG0007: These field was required: ContractCode.
        ///</summary>
        public string Case10()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "Q0000000536";
            doUpdate.Alphabet = "AA";
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = "3";
            string expected = "MSG0007";
            string actual;

            try
            {
                int result = target.UpdateQuotationData(doUpdate);
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
        ///Purpose   : Update quotation data when action type = 1 (draft)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = FN0000000527
        ///                - Alphabet = AA
        ///                - LastUpdateDate = 2011-07-14 09:00:00.000
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = 1
        ///Expected  : See expectation test case11
        ///</summary>
        public string Case11()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "FN0000000527";
            doUpdate.Alphabet = "AA";
            doUpdate.LastUpdateDate = new DateTime(2011, 7, 14, 9, 0, 0);
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = "1";
            string expected = "1";
            string actual;
            try
            {
                //Login user = 490488
                //Process datetime = 2011-11-14 09:20:00.000
                CommonUtil.dsTransData.dtUserData.EmpNo = "490488";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 11, 14, 09, 20, 00);
                
                CommonUtil.dsTransData.dtOperationData.GUID = "Case11";
                int result = target.UpdateQuotationData(doUpdate);
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
        ///Purpose   : Update quotation data when action type = 2 (approve)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = FN0000000527
        ///                - Alphabet = AA
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = N0000000152
        ///                - ActionTypeCode = 2
        ///Expected  : See expectation test case12
        ///</summary>
        public string Case12()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "FN0000000527";
            doUpdate.Alphabet = "AA";
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = "N0000000152";
            doUpdate.ActionTypeCode = "2";
            string expected = "1";
            string actual;
            try
            {
                //Login user = 510729
                //Process datetime = 2011-11-19  10:30:00.000
                CommonUtil.dsTransData.dtUserData.EmpNo = "510729";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 11, 19, 10, 30, 00);

                CommonUtil.dsTransData.dtOperationData.GUID = "Case12";
                int result = target.UpdateQuotationData(doUpdate);
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

        ///<summary>
        ///Purpose   : Update quotation data when action type = 3 (change)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = Q0000000536
        ///                - Alphabet = AB
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = Q0000000536
        ///                - ActionTypeCode = 3
        ///Expected  : See expectation test case13
        ///</summary>
        public string Case13()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "Q0000000536";
            doUpdate.Alphabet = "AB";
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = "Q0000000536";
            doUpdate.ActionTypeCode = "3";
            string expected = "1";
            string actual;
            try
            {
                //Login user = 490488
                //Process datetime = 2011-11-14 09:20:00.000
                CommonUtil.dsTransData.dtUserData.EmpNo = "490488";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 11, 14, 09, 20, 00);

                CommonUtil.dsTransData.dtOperationData.GUID = "Case13";
                int result = target.UpdateQuotationData(doUpdate);
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

            return string.Format(RESULT_FORMAT, 13, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Update quotation data when action type = 4 (cancel)
        ///Parameters:  doUpdateQuotationData
        ///                - QuotationTargetCode = Q0000000536
        ///                - Alphabet = NULL
        ///                - LastUpdateDate = NULL
        ///                - ContractCode = NULL
        ///                - ActionTypeCode = 4
        ///Expected  : See expectation test case14
        ///</summary>
        public string Case14()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doUpdateQuotationData doUpdate = new doUpdateQuotationData();
            doUpdate.QuotationTargetCode = "Q0000000536";
            doUpdate.Alphabet = null;
            doUpdate.LastUpdateDate = DateTime.MinValue;
            doUpdate.ContractCode = null;
            doUpdate.ActionTypeCode = "4";
            string expected = "1";
            string actual;
            try
            {
                //Login user = 510729
                //Process datetime = 2011-11-19  10:30:00.000
                CommonUtil.dsTransData.dtUserData.EmpNo = "510729";
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime = new DateTime(2011, 11, 19, 10, 30, 00);

                CommonUtil.dsTransData.dtOperationData.GUID = "Case14";
                int result = target.UpdateQuotationData(doUpdate);
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

            return string.Format(RESULT_FORMAT, 14, expected, actual, CompareResult_String(expected, actual));
        }

    }
}
