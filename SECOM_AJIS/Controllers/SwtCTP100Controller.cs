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
    public class SwtCTP100Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP100/

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

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");
            
            return result;
        }

        ///<summary>
        ///Purpose   : Generate AR request no. that related with customer code (How does the system performs if it over maximum of running no.)
        ///Parameters: - strARRelevantType = 0
        ///            - strARRelevantCode = C0000012025
        ///Expected  :  MSG3017: Cannot generate request no. because the number reach maximum, please contact administrator.
        ///</summary>
        public string Case1()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARRelevantType = "0";
            string strARRelevantCode = "C0000012025";

            string expected = "MSG3017";
            string actual;
            
            try
            {
                target.GenerateARRequestNo(strARRelevantType, strARRelevantCode);
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
        ///Purpose   : Generate AR request no. that related with customer code
        ///Parameters: - strARRelevantType = 0
        ///            - strARRelevantCode = C0000012025
        ///Expected  : Return to caller :
        ///            - strARRequestNo. = 000112A00001
        ///            - strAROffice = 0001
        ///</summary>
        public string Case2()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARRelevantType = "0";
            string strARRelevantCode = "C0000012025";

            string[] expected = new string[] { "000112A00001", "0001" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateARRequestNo(strARRelevantType, strARRelevantCode);
            }
            catch (ApplicationErrorException ex)
            {
                error = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                error = ex.StackTrace;
            }

            if (error == string.Empty)
            {
                //string strResult = CompareResult_String(expected, actual);
                //return string.Format(RESULT_FORMAT_LIST, 2, strResult);
                return string.Format(RESULT_FORMAT, 2, SetResult_String(expected), SetResult_String(actual), CompareResult_String(expected, actual));
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 2, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Generate AR request no. that related with site code (How does the system performs if it cannot get a record from contract data that related this site)
        ///Parameters: - strARRelevantType = 1
        ///            - strARRelevantCode = S0000012995-0001
        ///Expected  : MSG3192: Cannot generate the AR request no. Contract data does not exist.
        ///</summary>
        public string Case3()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARRelevantType = "1";
            string strARRelevantCode = "S0000012995-0001";

            string expected = "MSG3192";
            string actual;

            try
            {
                target.GenerateARRequestNo(strARRelevantType, strARRelevantCode);
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
        ///Purpose   : Generate AR request no. that related with site code (operation office of Alarm)
        ///Parameters: - strARRelevantType = 1
        ///            - strARRelevantCode = S0000012015-0001
        ///Expected  : Return to caller :
        ///            - strARRequestNo. = 102012A01022
        ///            - strAROffice = 1020
        ///</summary>
        public string Case4()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARRelevantType = "1";
            string strARRelevantCode = "S0000012015-0001";

            string[] expected = new string[] { "102012A01022", "1020" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateARRequestNo(strARRelevantType, strARRelevantCode);
            }
            catch (ApplicationErrorException ex)
            {
                error = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                error = ex.StackTrace;
            }

            if (error == string.Empty)
            {
                //string strResult = CompareResult_String(expected, actual);
                //return string.Format(RESULT_FORMAT_LIST, 4, strResult);
                return string.Format(RESULT_FORMAT, 4, SetResult_String(expected), SetResult_String(actual), CompareResult_String(expected, actual));
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 4, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Generate AR request no. that related with site code (operation office of SG)
        ///Parameters: - strARRelevantType = 1
        ///            - strARRelevantCode = S0000000032-0001
        ///Expected  : Return to caller :
        ///            - strARRequestNo. = 105012A01052
        ///            - strAROffice = 1050
        ///</summary>
        public string Case5()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARRelevantType = "1";
            string strARRelevantCode = "S0000000032-0001";

            string[] expected = new string[] { "105012A01052", "1050" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateARRequestNo(strARRelevantType, strARRelevantCode);
            }
            catch (ApplicationErrorException ex)
            {
                error = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                error = ex.StackTrace;
            }

            if (error == string.Empty)
            {
                //string strResult = CompareResult_String(expected, actual);
                //return string.Format(RESULT_FORMAT_LIST, 5, strResult);
                return string.Format(RESULT_FORMAT, 5, SetResult_String(expected), SetResult_String(actual), CompareResult_String(expected, actual));
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 5, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Generate AR request no. that related with site code (operation office of MA)
        ///Parameters: - strARRelevantType = 1
        ///            - strARRelevantCode = S0000000046-0001
        ///Expected  : Return to caller :
        ///            - strARRequestNo. = 206012A02062
        ///            - strAROffice = 2060
        ///</summary>
        public string Case6()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARRelevantType = "1";
            string strARRelevantCode = "S0000000046-0001";

            string[] expected = new string[] { "206012A02062", "2060" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateARRequestNo(strARRelevantType, strARRelevantCode);
            }
            catch (ApplicationErrorException ex)
            {
                error = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                error = ex.StackTrace;
            }

            if (error == string.Empty)
            {
                //string strResult = CompareResult_String(expected, actual);
                //return string.Format(RESULT_FORMAT_LIST, 6, strResult);
                return string.Format(RESULT_FORMAT, 6, SetResult_String(expected), SetResult_String(actual), CompareResult_String(expected, actual));
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 6, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Generate AR request no. that related with site code (operation office of Sale)
        ///Parameters: - strARRelevantType = 1
        ///            - strARRelevantCode = S0000000059-0001
        ///Expected  : Return to caller :
        ///            - strARRequestNo. = 102012A01023
        ///            - strAROffice = 1020
        ///</summary>
        public string Case7()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARRelevantType = "1";
            string strARRelevantCode = "S0000000059-0001";

            string[] expected = new string[] { "102012A01023", "1020" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateARRequestNo(strARRelevantType, strARRelevantCode);
            }
            catch (ApplicationErrorException ex)
            {
                error = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                error = ex.StackTrace;
            }

            if (error == string.Empty)
            {
                //string strResult = CompareResult_String(expected, actual);
                //return string.Format(RESULT_FORMAT_LIST, 7, strResult);
                return string.Format(RESULT_FORMAT, 7, SetResult_String(expected), SetResult_String(actual), CompareResult_String(expected, actual));
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 7, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Generate AR request no. that related with project code 
        ///Parameters: - strARRelevantType = 2
        ///            - strARRelevantCode = P0012015
        ///Expected  : Return to caller :
        ///            - strARRequestNo. = 000112A00002
        ///            - strAROffice = 0001
        ///</summary>
        public string Case8()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARRelevantType = "2";
            string strARRelevantCode = "P0012015";

            string[] expected = new string[] { "000112A00002", "0001" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateARRequestNo(strARRelevantType, strARRelevantCode);
            }
            catch (ApplicationErrorException ex)
            {
                error = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                error = ex.StackTrace;
            }

            if (error == string.Empty)
            {
                //string strResult = CompareResult_String(expected, actual);
                //return string.Format(RESULT_FORMAT_LIST, 8, strResult);
                return string.Format(RESULT_FORMAT, 8, SetResult_String(expected), SetResult_String(actual), CompareResult_String(expected, actual));
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 8, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Generate AR request no. that related with contract code
        ///Parameters: - strARRelevantType = 3
        ///            - strARRelevantCode = Q0000012015
        ///Expected  : Return to caller :
        ///            - strARRequestNo. = 201012A02012
        ///            - strAROffice = 2010
        ///</summary>
        public string Case9()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARRelevantType = "3";
            string strARRelevantCode = "Q0000012015";

            string[] expected = new string[] { "201012A02012", "2010" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateARRequestNo(strARRelevantType, strARRelevantCode);
            }
            catch (ApplicationErrorException ex)
            {
                error = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                error = ex.StackTrace;
            }

            if (error == string.Empty)
            {
                //string strResult = CompareResult_String(expected, actual);
                //return string.Format(RESULT_FORMAT_LIST, 9, strResult);
                return string.Format(RESULT_FORMAT, 9, SetResult_String(expected), SetResult_String(actual), CompareResult_String(expected, actual));
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 9, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Generate AR request no. that related with quotation target code
        ///Parameters: - strARRelevantType = 4
        ///            - strARRelevantCode = FSG0000012015
        ///Expected  : Return to caller :
        ///            - strARRequestNo. = 201012A02013
        ///            - strAROffice = 2010
        ///</summary>
        public string Case10()
        {
            IARHandler target = ServiceContainer.GetService<IARHandler>() as IARHandler;
            string strARRelevantType = "4";
            string strARRelevantCode = "FSG0000012015";

            string[] expected = new string[] { "201012A02013", "2010" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateARRequestNo(strARRelevantType, strARRelevantCode);
            }
            catch (ApplicationErrorException ex)
            {
                error = ex.ErrorResult.Message.Code;
            }
            catch (Exception ex)
            {
                error = ex.StackTrace;
            }

            if (error == string.Empty)
            {
                //string strResult = CompareResult_String(expected, actual);
                //return string.Format(RESULT_FORMAT_LIST, 10, strResult);
                return string.Format(RESULT_FORMAT, 10, SetResult_String(expected), SetResult_String(actual), CompareResult_String(expected, actual));
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 10, "Fail", error);
            }
        }

    }
}
