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
    public class SwtCTP090Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP090/

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
        ///Purpose   : Generate incident no. that related with customer code (How does the system performs if it over maximum of running no.)
        ///Parameters: - strIncidentRelevantType = 0
        ///            - strIncidentRelevantCode = C0000012025
        ///            - strIncidentOfficeCode = 0001
        ///Expected  : MSG3016: Cannot generate incident no. because the number reach maximum, please contact administrator.
        ///</summary>
        public string Case1()
        {
            IIncidentHandler target = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            string strIncidentRelevantType = "0";
            string strIncidentRelevantCode = "C0000012025";
            string strIncidentOfficeCode = "0001";
            string expected = "MSG3016";
            string actual;
            
            try
            {
                target.GenerateIncidentNo(strIncidentRelevantType, strIncidentRelevantCode, strIncidentOfficeCode);
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
        ///Purpose   : Generate incident no. that related with customer code
        ///Parameters: - strIncidentRelevantType = 0
        ///            - strIncidentRelevantCode = C0000012025
        ///            - strIncidentOfficeCode = 1000
        ///Expected  : Return to caller :
        ///            - strIncidentNo = 100012S00001
        ///            - strIncidentOffice = 1000
        ///</summary>
        public string Case2()
        {
            IIncidentHandler target = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            string strIncidentRelevantType = "0";
            string strIncidentRelevantCode = "C0000012025";
            string strIncidentOfficeCode = "1000";
            
            string[] expected = new string[] { "100012S00001", "1000" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateIncidentNo(strIncidentRelevantType, strIncidentRelevantCode, strIncidentOfficeCode);
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
        ///Purpose   : Generate incident no. that related with site code (How does the system performs if it cannot get a record from contract data that related this site)
        ///Parameters: - strIncidentRelevantType = 1
        ///            - strIncidentRelevantCode = S0000012995-0001
        ///            - strIncidentOfficeCode = NULL
        ///Expected  : MSG3190: Cannot generate the incident no. Contract data does not exist.
        ///</summary>
        public string Case3()
        {
            IIncidentHandler target = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            string strIncidentRelevantType = "1";
            string strIncidentRelevantCode = "S0000012995-0001";
            string strIncidentOfficeCode = null;
            string expected = "MSG3190";
            string actual;

            try
            {
                target.GenerateIncidentNo(strIncidentRelevantType, strIncidentRelevantCode, strIncidentOfficeCode);
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
        ///Purpose   : Generate incident no. that related with site code (operation office of alarm)
        ///Parameters: - strIncidentRelevantType = 1
        ///            - strIncidentRelevantCode = S0000012015-0001
        ///            - strIncidentOfficeCode = NULL
        ///Expected  : Return to caller :
        ///            - strIncidentNo = 102012S01022
        ///            - strIncidentOffice = 1020
        ///</summary>
        public string Case4()
        {
            IIncidentHandler target = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            string strIncidentRelevantType = "1";
            string strIncidentRelevantCode = "S0000012015-0001";
            string strIncidentOfficeCode = null;

            string[] expected = new string[] { "102012S01022", "1020" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateIncidentNo(strIncidentRelevantType, strIncidentRelevantCode, strIncidentOfficeCode);
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
        ///Purpose   : Generate incident no. that related with site code (operation office of SG)
        ///Parameters: - strIncidentRelevantType = 1
        ///            - strIncidentRelevantCode = S0000000032-0001
        ///            - strIncidentOfficeCode = NULL
        ///Expected  : Return to caller :
        ///            - strIncidentNo = 105012S01052
        ///            - strIncidentOffice = 1050
        ///</summary>
        public string Case5()
        {
            IIncidentHandler target = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            string strIncidentRelevantType = "1";
            string strIncidentRelevantCode = "S0000000032-0001";
            string strIncidentOfficeCode = null;

            string[] expected = new string[] { "105012S01052", "1050" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateIncidentNo(strIncidentRelevantType, strIncidentRelevantCode, strIncidentOfficeCode);
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
        ///Purpose   : Generate incident no. that related with site code (operation office of MA)
        ///Parameters: - strIncidentRelevantType = 1
        ///            - strIncidentRelevantCode = S0000000046-0001
        ///            - strIncidentOfficeCode = NULL
        ///Expected  : Return to caller :
        ///            - strIncidentNo = 206012S02062
        ///            - strIncidentOffice = 2060
        ///</summary>
        public string Case6()
        {
            IIncidentHandler target = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            string strIncidentRelevantType = "1";
            string strIncidentRelevantCode = "S0000000046-0001";
            string strIncidentOfficeCode = null;

            string[] expected = new string[] { "206012S02062", "2060" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateIncidentNo(strIncidentRelevantType, strIncidentRelevantCode, strIncidentOfficeCode);
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
        ///Purpose   : Generate incident no. that related with site code (operation office of Sale)
        ///Parameters: - strIncidentRelevantType = 1
        ///            - strIncidentRelevantCode = S0000000059-0001
        ///            - strIncidentOfficeCode = NULL
        ///Expected  : Return to caller :
        ///            - strIncidentNo = 102012S01023
        ///            - strIncidentOffice = 1020
        ///</summary>
        public string Case7()
        {
            IIncidentHandler target = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            string strIncidentRelevantType = "1";
            string strIncidentRelevantCode = "S0000000059-0001";
            string strIncidentOfficeCode = null;

            string[] expected = new string[] { "102012S01023", "1020" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateIncidentNo(strIncidentRelevantType, strIncidentRelevantCode, strIncidentOfficeCode);
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
        ///Purpose   : Generate incident no. that related with project code 
        ///Parameters: - strIncidentRelevantType = 2
        ///            - strIncidentRelevantCode = P0012015
        ///            - strIncidentOfficeCode = 1000
        ///Expected  : Return to caller :
        ///            - strIncidentNo = 100012S00002
        ///            - strIncidentOffice = 1000
        ///</summary>
        public string Case8()
        {
            IIncidentHandler target = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            string strIncidentRelevantType = "2";
            string strIncidentRelevantCode = "P0012015";
            string strIncidentOfficeCode = "1000";

            string[] expected = new string[] { "100012S00002", "1000" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateIncidentNo(strIncidentRelevantType, strIncidentRelevantCode, strIncidentOfficeCode);
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
        ///Purpose   : Generate incident no. that related with contract code
        ///Parameters: - strIncidentRelevantType = 3
        ///            - strIncidentRelevantCode = MA0000012165
        ///            - strIncidentOfficeCode = NULL
        ///Expected  : Return to caller :
        ///            - strIncidentNo = 202012S02022
        ///            - strIncidentOffice = 2020
        ///</summary>
        public string Case9()
        {
            IIncidentHandler target = ServiceContainer.GetService<IIncidentHandler>() as IIncidentHandler;
            string strIncidentRelevantType = "3";
            string strIncidentRelevantCode = "MA0000012165";
            string strIncidentOfficeCode = null;

            string[] expected = new string[] { "202012S02022", "2020" };
            string[] actual = null;
            string error = string.Empty;

            try
            {
                actual = target.GenerateIncidentNo(strIncidentRelevantType, strIncidentRelevantCode, strIncidentOfficeCode);
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
    }
}
