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
    public class SwtCTP021Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP021/

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
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract code]
        ///</summary>
        public string Case1()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            string strContractCode = null;
            string expected = "MSG0007";
            string actual;
            
            try
            {
                target.DeleteBillingTempByContractCode(strContractCode);
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
        ///Purpose   : Keep billing data (Delete)
        ///Parameters: ContractCode = N0012135
        ///Expected  : Delete tbt_BillingTemp : 3 records
        ///            In tbt_BillingTemp not found data of :
        ///            Contract code = N0012135
        ///            
        ///            Keep operation log to log table
        ///</summary>
        public string Case2()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            string strContractCode = "N0000012135";
            int expected = 3;
            int actual = 0;
            string error = string.Empty;

            try
            {
                List<tbt_BillingTemp> resultList = target.DeleteBillingTempByContractCode(strContractCode);
                if (resultList != null && resultList.Count > 0)
                    actual = resultList.Count;
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
                bool bResult = (actual == expected);
                return string.Format(RESULT_FORMAT_LIST, 2, bResult);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 2, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract code], [OCC]
        ///</summary>
        public string Case3()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            string strContractCode = null;
            string strOCC = null;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.DeleteBillingTempByContractCodeOCC(strContractCode, strOCC);
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
        ///Purpose   : Keep billing data (Delete)
        ///Parameters: - ContractCode = N0012225
        ///            - OCC = 0001
        ///Expected  : Delete tbt_BillingTemp : 3 records
        ///            In tbt_BillingTemp not found data of :
        ///            Contract code = N0012225
        ///            OCC = 0001 
        ///            
        ///            Keep operation log to log table
        ///</summary>
        public string Case4()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            string strContractCode = "N0000012225";
            string strOCC = "0001";
            int expected = 3;
            int actual = 0;
            string error = string.Empty;

            try
            {
                List<tbt_BillingTemp> resultList = target.DeleteBillingTempByContractCodeOCC(strContractCode, strOCC);
                if (resultList != null && resultList.Count > 0)
                    actual = resultList.Count;
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
                bool bResult = (actual == expected);
                return string.Format(RESULT_FORMAT_LIST, 4, bResult);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 4, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract code], [OCC]
        ///</summary>
        public string Case5()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            string strContractCode = null;
            string strOCC = null;
            int iSequenceNo = 0;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.DeleteBillingTempByKey(strContractCode, strOCC, iSequenceNo);
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
        ///Purpose   : Keep billing data (Delete)
        ///Parameters: - ContractCode = N0012125
        ///            - OCC = 0001
        ///            - SequenceNo = 1
        ///Expected  : Delete tbt_BillingTemp : 1 record
        ///            In tbt_BillingTemp not found data of :
        ///            Contract code = N0012125
        ///            OCC = 0001 
        ///            SqquenceNo = 1
        ///            
        ///            Keep operation log to log table
        ///</summary>
        public string Case6()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            string strContractCode = "N0000012125";
            string strOCC = "0001";
            int iSequenceNo = 1;
            int expected = 1;
            int actual = 0;
            string error = string.Empty;

            try
            {
                List<tbt_BillingTemp> resultList = target.DeleteBillingTempByKey(strContractCode, strOCC, iSequenceNo);
                if (resultList != null && resultList.Count > 0)
                    actual = resultList.Count;
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
                bool bResult = (actual == expected);
                return string.Format(RESULT_FORMAT_LIST, 6, bResult);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 6, "Fail", error);
            }
        }
    }
}
