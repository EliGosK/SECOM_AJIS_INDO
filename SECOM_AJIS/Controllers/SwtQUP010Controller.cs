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
    public class SwtQUP010Controller : SwtCommonController
    {
        //
        // GET: /SwtQUP010/

        public string index()
        {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            //lst.Add(Case2());
            //lst.Add(Case3());
            //lst.Add(Case4());
            //lst.Add(Case5());
            //lst.Add(Case6());
            //lst.Add(Case7());
            //lst.Add(Case8());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");
            
            return result;
        }

        ///<summary>
        ///Purpose   : Purge data from db
        ///Procedure : 1. Directly call QuotationHandler.DeleteQuotation from test class.
        ///            2. Check return result of the process and remaining data in DB.
        ///Expected  : 1. dtBatchProcessResult (7 cases following locked tables)
        ///                .Result  = 1
        ///                .BatchStatus = NULL
        ///                .Total  = 13
        ///                .Complete = 13
        ///                .Failed  = 0
        ///                .ErrorMessage = NULL
        ///            2. Expected data in db
        ////                - See expectation test case1
        ///</summary>
        public string Case1()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dtBatchProcessResult expected = CreateExpectForNormalCase();
            dtBatchProcessResult actual = null;
            string error = string.Empty;
            try
            {
                List<dtBatchProcessResult> lst = target.DeleteQuotation();
                actual = lst[0];
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
                return CompareDtBatchProcessResult(actual, expected, 1);
            else
                return string.Format(RESULT_FORMAT_ERROR, 1, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Purge data from db
        ///Procedure : 1. Directly call QuotationHandler.DeleteQuotation from test class.
        ///            2. Check return result of the process and remaining data in DB.
        ///            Change locked table - tbt_QuotationSentryGuardDetails
        ///Expected  : 1. dtBatchProcessResult (7 cases following locked tables)
        ///                .Result  = 0
        ///                .BatchStatus = NULL
        ///                .Total  = 13
        ///                .Complete = 0
        ///                .Failed  = 13
        ///                .ErrorMessage = Delete quotation process is failed. Cannot delete tbt_QuotationSentryGuardDetails. All deleted data is rollbacked.
        ///            2. Expected data in db
        ////                - Not change from before running the process
        ///</summary>
        public string Case2()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dtBatchProcessResult expected = CreateExpectForErrorCase("tbt_QuotationSentryGuardDetails");
            dtBatchProcessResult actual = null;
            string error = string.Empty;
            try
            {
                List<dtBatchProcessResult> lst = target.DeleteQuotation();
                actual = lst[0];
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
                return CompareDtBatchProcessResult(actual, expected, 2);
            else
                return string.Format(RESULT_FORMAT_ERROR, 2, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Purge data from db
        ///Procedure : 1. Directly call QuotationHandler.DeleteQuotation from test class.
        ///            2. Check return result of the process and remaining data in DB.
        ///            Change locked table - tbt_QuotationBeatGuardDetails
        ///Expected  : 1. dtBatchProcessResult (7 cases following locked tables)
        ///                .Result  = 0
        ///                .BatchStatus = NULL
        ///                .Total  = 13
        ///                .Complete = 0
        ///                .Failed  = 13
        ///                .ErrorMessage = Delete quotation process is failed. Cannot delete tbt_QuotationBeatGuardDetails. All deleted data is rollbacked.
        ///            2. Expected data in db
        ////                - Not change from before running the process
        ///</summary>
        public string Case3()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dtBatchProcessResult expected = CreateExpectForErrorCase("tbt_QuotationBeatGuardDetails");
            dtBatchProcessResult actual = null;
            string error = string.Empty;
            try
            {
                List<dtBatchProcessResult> lst = target.DeleteQuotation();
                actual = lst[0];
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
                return CompareDtBatchProcessResult(actual, expected, 3);
            else
                return string.Format(RESULT_FORMAT_ERROR, 3, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Purge data from db
        ///Procedure : 1. Directly call QuotationHandler.DeleteQuotation from test class.
        ///            2. Check return result of the process and remaining data in DB.
        ///            3. Change locked table - tbt_QuotationFacilityDetails
        ///Expected  : 1. dtBatchProcessResult (7 cases following locked tables)
        ///                .Result  = 0
        ///                .BatchStatus = NULL
        ///                .Total  = 13
        ///                .Complete = 0
        ///                .Failed  = 13
        ///                .ErrorMessage = Delete quotation process is failed. Cannot delete tbt_QuotationFacilityDetails. All deleted data is rollbacked.
        ///            2. Expected data in db
        ////                - Not change from before running the process
        ///</summary>
        public string Case4()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dtBatchProcessResult expected = CreateExpectForErrorCase("tbt_QuotationFacilityDetails");
            dtBatchProcessResult actual = null;
            string error = string.Empty;
            try
            {
                List<dtBatchProcessResult> lst = target.DeleteQuotation();
                actual = lst[0];
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
                return CompareDtBatchProcessResult(actual, expected, 4);
            else
                return string.Format(RESULT_FORMAT_ERROR, 4, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Purge data from db
        ///Procedure : 1. Directly call QuotationHandler.DeleteQuotation from test class.
        ///            2. Check return result of the process and remaining data in DB.
        ///            3. Change locked table - tbt_QuotationInstrumentDetails
        ///Expected  : 1. dtBatchProcessResult (7 cases following locked tables)
        ///                .Result  = 0
        ///                .BatchStatus = NULL
        ///                .Total  = 13
        ///                .Complete = 0
        ///                .Failed  = 13
        ///                .ErrorMessage = Delete quotation process is failed. Cannot delete tbt_QuotationInstrumentDetails. All deleted data is rollbacked.
        ///            2. Expected data in db
        ////                - Not change from before running the process
        ///</summary>
        public string Case5()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dtBatchProcessResult expected = CreateExpectForErrorCase("tbt_QuotationInstrumentDetails");
            dtBatchProcessResult actual = null;
            string error = string.Empty;
            try
            {
                List<dtBatchProcessResult> lst = target.DeleteQuotation();
                actual = lst[0];
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
                return CompareDtBatchProcessResult(actual, expected, 5);
            else
                return string.Format(RESULT_FORMAT_ERROR, 5, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Purge data from db
        ///Procedure : 1. Directly call QuotationHandler.DeleteQuotation from test class.
        ///            2. Check return result of the process and remaining data in DB.
        ///            3. Change locked table - tbt_QuotationMaintenanceLinkage
        ///Expected  : 1. dtBatchProcessResult (7 cases following locked tables)
        ///                .Result  = 0
        ///                .BatchStatus = NULL
        ///                .Total  = 13
        ///                .Complete = 0
        ///                .Failed  = 13
        ///                .ErrorMessage = Delete quotation process is failed. Cannot delete tbt_QuotationMaintenanceLinkage. All deleted data is rollbacked.
        ///            2. Expected data in db
        ////                - Not change from before running the process
        ///</summary>
        public string Case6()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dtBatchProcessResult expected = CreateExpectForErrorCase("tbt_QuotationMaintenanceLinkage");
            dtBatchProcessResult actual = null;
            string error = string.Empty;
            try
            {
                List<dtBatchProcessResult> lst = target.DeleteQuotation();
                actual = lst[0];
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
                return CompareDtBatchProcessResult(actual, expected, 6);
            else
                return string.Format(RESULT_FORMAT_ERROR, 6, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Purge data from db
        ///Procedure : 1. Directly call QuotationHandler.DeleteQuotation from test class.
        ///            2. Check return result of the process and remaining data in DB.
        ///            3. Change locked table - tbt_QuotationOperationType
        ///Expected  : 1. dtBatchProcessResult (7 cases following locked tables)
        ///                .Result  = 0
        ///                .BatchStatus = NULL
        ///                .Total  = 13
        ///                .Complete = 0
        ///                .Failed  = 13
        ///                .ErrorMessage = Delete quotation process is failed. Cannot delete tbt_QuotationOperationType. All deleted data is rollbacked.
        ///            2. Expected data in db
        ////                - Not change from before running the process
        ///</summary>
        public string Case7()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dtBatchProcessResult expected = CreateExpectForErrorCase("tbt_QuotationOperationType");
            dtBatchProcessResult actual = null;
            string error = string.Empty;
            try
            {
                List<dtBatchProcessResult> lst = target.DeleteQuotation();
                actual = lst[0];
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
                return CompareDtBatchProcessResult(actual, expected, 7);
            else
                return string.Format(RESULT_FORMAT_ERROR, 7, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Purge data from db
        ///Procedure : 1. Directly call QuotationHandler.DeleteQuotation from test class.
        ///            2. Check return result of the process and remaining data in DB.
        ///            3. Change locked table - tbt_QuotationBasic
        ///Expected  : 1. dtBatchProcessResult (7 cases following locked tables)
        ///                .Result  = 0
        ///                .BatchStatus = NULL
        ///                .Total  = 13
        ///                .Complete = 0
        ///                .Failed  = 13
        ///                .ErrorMessage = Delete quotation process is failed. Cannot delete tbt_QuotationBasic. All deleted data is rollbacked.
        ///            2. Expected data in db
        ////                - Not change from before running the process
        ///</summary>
        public string Case8()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            dtBatchProcessResult expected = CreateExpectForErrorCase("tbt_QuotationBasic");
            dtBatchProcessResult actual = null;
            string error = string.Empty;
            try
            {
                List<dtBatchProcessResult> lst = target.DeleteQuotation();
                actual = lst[0];
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
                return CompareDtBatchProcessResult(actual, expected, 8);
            else
                return string.Format(RESULT_FORMAT_ERROR, 8, "Fail", error);
        }

        private string CompareDtBatchProcessResult(dtBatchProcessResult actual, dtBatchProcessResult expect, int caseNo)
        {
            bool bResult = CompareObject<dtBatchProcessResult>(actual, expect);

            string result = string.Format(RESULT_FORMAT_LIST, caseNo, bResult);
            //result += string.Format(RESULT_FORMAT_LIST_DATA, caseNo, "ErrorMessage", actual.ErrorMessage);
            
            return result;
        }

        private dtBatchProcessResult CreateExpectForNormalCase()
        {
            dtBatchProcessResult dtBatch = new dtBatchProcessResult();
            dtBatch.Result = true;
            dtBatch.BatchStatus = null;
            dtBatch.Total = 14;
            dtBatch.Complete = 14;
            dtBatch.Failed = 0;
            dtBatch.ErrorMessage = null;

            return dtBatch;
        }

        private dtBatchProcessResult CreateExpectForErrorCase(string strTableName)
        {
            dtBatchProcessResult dtBatch = new dtBatchProcessResult();
            dtBatch.Result = false;
            dtBatch.BatchStatus = null;
            dtBatch.Total = 14;
            dtBatch.Complete = 0;
            dtBatch.Failed = 14;
            dtBatch.ErrorMessage = String.Format("Delete quotation process is failed. Cannot delete {0}. All deleted data is rollbacked.", strTableName);

            return dtBatch;
        }
    }
}
