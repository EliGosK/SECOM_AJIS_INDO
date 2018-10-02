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
using System.Reflection;

namespace SECOM_AJIS.Controllers
{
    public class SwtQUP060Controller : SwtCommonController
    {
        //
        // GET: /SwtQUP060/

        public string index()
        {
            //Using in write log process
            CommonUtil.dsTransData = new dsTransDataModel();
            CommonUtil.dsTransData.dtUserData = new UserDataDo();
            CommonUtil.dsTransData.dtOperationData = new OperationDataDo();
            CommonUtil.dsTransData.dtTransHeader = new TransHeaderDo();
            CommonUtil.dsTransData.dtTransHeader.ScreenID = "QUP060";

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
            lst.Add(Case15());
            lst.Add(Case16());
            lst.Add(Case17());
            lst.Add(Case18());
            lst.Add(Case19());
            lst.Add(Case20());
            lst.Add(Case21());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose   : Mandatory check1 QuotationHandler.GetQuotationData (How does the system perform if contractFlag = true but ServiceTypeCode and TargetCodeTypeCode are not specified)
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = NULL
        ///             - Alphabet = NULL
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = TRUE
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: ServiceTypeCode, TargetCodeTypeCode.
        ///</summary>
        public string Case1()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = null;
            cond.Alphabet = null;
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = true;
            cond.ProductTypeCode = null;
            
            string expected = "MSG0007";
            string actual;

            try
            {
                target.GetQuotationData(cond);
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
        ///Purpose   : Mandatory check2 QuotationHandler.GetQuotationBasicData (How does the system perform if QuotationTargetCode and Alphabet are not specified.)
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = NULL
        ///             - Alphabet = NULL
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: QuotationTargetCode, Alphabet.
        ///</summary>
        public string Case2()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = null;
            cond.Alphabet = null;
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.GetQuotationBasicData(cond);
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
        ///Purpose   : Get quotation basic data (How does the system perform if it cannot get quotation basic data.)
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = FQ0000508
        ///             - Alphabet = AB
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : dsQuotationData = NULL
        ///</summary>
        public string Case3()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = "FQ0000508";
            cond.Alphabet = "AB";
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "null";
            string actual;

            try
            {
                dsQuotationData result = target.GetQuotationData(cond);
                actual = (result == null) ? "null" : "error";
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
        ///Purpose   : Get quotation basic data (How does the system perform if contractFlag = true and lock status of the returned information = 1)
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = N0000000362
        ///             - Alphabet = AA
        ///             - ServiceTypeCode = 2
        ///             - TargetCodeTypeCode = 2
        ///             - ContractFlag = TRUE
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG2007: The selected alphabet is locked, please select the new alphabet.
        ///</summary>
        public string Case4()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = "N0000000362";
            cond.Alphabet = "AA";
            cond.ServiceTypeCode = "2";
            cond.TargetCodeTypeCode = "2";
            cond.ContractFlag = true;
            cond.ProductTypeCode = null;

            string expected = "MSG2007";
            string actual;

            try
            {
                target.GetQuotationData(cond);
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
        ///Purpose   : Mandatory check3 QuotationHandler.GetTbt_QuotationTarget
        ///Pre-conditions: directly call QuotationHandler.GetTbt_QuotationTarget
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = NULL
        ///             - Alphabet = NULL
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: QuotationTargetCode.
        ///</summary>
        public string Case5()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = null;
            cond.Alphabet = null;
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.GetTbt_QuotationTarget(cond);
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
        ///Purpose   : Mandatory check4 QuotationHandler.GetTbt_QuotationCustomer
        ///Pre-conditions: directly call QuotationHandler.GetTbt_QuotationCustomer
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = NULL
        ///             - Alphabet = NULL
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: QuotationTargetCode.
        ///</summary>
        public string Case6()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = null;
            cond.Alphabet = null;
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.GetTbt_QuotationCustomer(cond);
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
        ///Purpose   : Mandatory check5 QuotationHandler.GetTbt_QuotationSite
        ///Pre-conditions: directly call QuotationHandler.GetTbt_QuotationSite
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = NULL
        ///             - Alphabet = NULL
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: QuotationTargetCode.
        ///</summary>
        public string Case7()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = null;
            cond.Alphabet = null;
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.GetTbt_QuotationSite(cond);
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

            return string.Format(RESULT_FORMAT, 7, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Mandatory check6 QuotationHandler.GetTbt_QuotationInstrumentDetails
        ///Pre-conditions: directly call QuotationHandler.GetTbt_QuotationInstrumentDetails
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = NULL
        ///             - Alphabet = NULL
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: QuotationTargetCode, Alphabet.
        ///</summary>
        public string Case8()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = null;
            cond.Alphabet = null;
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.GetTbt_QuotationInstrumentDetails(cond);
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

            return string.Format(RESULT_FORMAT, 8, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Mandatory check7 QuotationHandler.GetTbt_QuotationOperationType
        ///Pre-conditions: directly call QuotationHandler.GetTbt_QuotationOperationType
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = NULL
        ///             - Alphabet = NULL
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: QuotationTargetCode, Alphabet.
        ///</summary>
        public string Case9()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = null;
            cond.Alphabet = null;
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.GetTbt_QuotationOperationType(cond);
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

            return string.Format(RESULT_FORMAT, 9, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Mandatory check8 QuotationHandler.GetTbt_QuotationFacilityDetails
        ///Pre-conditions: directly call QuotationHandler.GetTbt_QuotationFacilityDetails
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = NULL
        ///             - Alphabet = NULL
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: QuotationTargetCode, Alphabet.
        ///</summary>
        public string Case10()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = null;
            cond.Alphabet = null;
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.GetTbt_QuotationFacilityDetails(cond);
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

            return string.Format(RESULT_FORMAT, 10, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Mandatory check9 QuotationHandler.GetTbt_QuotationBeatGuardDetails
        ///Pre-conditions: directly call QuotationHandler.GetTbt_QuotationBeatGuardDetails
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = NULL
        ///             - Alphabet = NULL
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: QuotationTargetCode, Alphabet.
        ///</summary>
        public string Case11()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = null;
            cond.Alphabet = null;
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.GetTbt_QuotationBeatGuardDetails(cond);
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

            return string.Format(RESULT_FORMAT, 11, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Mandatory check10 QuotationHandler.GetTbt_QuotationSentryGuardDetails
        ///Pre-conditions: directly call QuotationHandler.GetTbt_QuotationSentryGuardDetails
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = NULL
        ///             - Alphabet = NULL
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: QuotationTargetCode, Alphabet.
        ///</summary>
        public string Case12()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = null;
            cond.Alphabet = null;
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.GetTbt_QuotationSentryGuardDetails(cond);
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

            return string.Format(RESULT_FORMAT, 12, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Mandatory check11 QuotationHandler.GetTbt_QuotationMaintenanceLinkage
        ///Pre-conditions: directly call QuotationHandler.GetTbt_QuotationMaintenanceLinkage
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = NULL
        ///             - Alphabet = NULL
        ///             - ServiceTypeCode = NULL
        ///             - TargetCodeTypeCode = NULL
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : MSG0007: These field was required: QuotationTargetCode, Alphabet.
        ///</summary>
        public string Case13()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = null;
            cond.Alphabet = null;
            cond.ServiceTypeCode = null;
            cond.TargetCodeTypeCode = null;
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.GetTbt_QuotationMaintenanceLinkage(cond);
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

            return string.Format(RESULT_FORMAT, 13, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Get quotation target data (How does the system perform if it cannot get quotation target data.)
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = N0000000123
        ///             - Alphabet = BC
        ///             - ServiceTypeCode = 2
        ///             - TargetCodeTypeCode = 1
        ///             - ContractFlag = NULL
        ///             - ProductTypeCode = NULL
        ///Expected  : dsQuotationData = NULL
        ///</summary>
        public string Case14()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = "N0000000123";
            cond.Alphabet = "BC";
            cond.ServiceTypeCode = "2";
            cond.TargetCodeTypeCode = "1";
            cond.ContractFlag = null;
            cond.ProductTypeCode = null;

            string expected = "null";
            string actual;

            try
            {
                dsQuotationData result = target.GetQuotationData(cond);
                actual = (result == null) ? "null" : "error";
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

        ///<summary>
        ///Purpose   : Get a quotation which is not for contract (How does the system perform if  contractFlag = false and lock status of the returned information = 1)
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = N0000000362
        ///             - Alphabet = AA
        ///             - ServiceTypeCode = 2
        ///             - TargetCodeTypeCode = 2
        ///             - ContractFlag = FALSE
        ///             - ProductTypeCode = NULL
        ///Expected  : See expectation test case 15
        ///</summary>
        public string Case15()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = "N0000000362";
            cond.Alphabet = "AA";
            cond.ServiceTypeCode = "2";
            cond.TargetCodeTypeCode = "2";
            cond.ContractFlag = false;
            cond.ProductTypeCode = null;

            dsQuotationData expected = CreateExpectForCase15();
            dsQuotationData actual = null;
            string error = string.Empty;
            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case15";
                actual = target.GetQuotationData(cond);
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
                return CompareDsQuotationList(actual, expected, 15);
            else
                return string.Format(RESULT_FORMAT_ERROR, 15, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Get sale quotation for contract
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = Q0000000223
        ///             - Alphabet = AA
        ///             - ServiceTypeCode = 1
        ///             - TargetCodeTypeCode = 2
        ///             - ContractFlag = TRUE
        ///             - ProductTypeCode = NULL
        ///Expected  : See expectation test case 16
        ///</summary>
        public string Case16()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = "Q0000000223";
            cond.Alphabet = "AA";
            cond.ServiceTypeCode = "1";
            cond.TargetCodeTypeCode = "2";
            cond.ContractFlag = true;
            cond.ProductTypeCode = null;

            dsQuotationData expected = CreateExpectForCase16();
            dsQuotationData actual = null;
            string error = string.Empty;
            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case16";
                actual = target.GetQuotationData(cond);
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
                return CompareDsQuotationList(actual, expected, 16);
            else
                return string.Format(RESULT_FORMAT_ERROR, 16, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Get alarm quotation for contract
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = FN0000000358
        ///             - Alphabet = AB
        ///             - ServiceTypeCode = 2
        ///             - TargetCodeTypeCode = 1
        ///             - ContractFlag = TRUE
        ///             - ProductTypeCode = NULL
        ///Expected  : See expectation test case 17
        ///</summary>
        public string Case17()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = "FN0000000358";
            cond.Alphabet = "AB";
            cond.ServiceTypeCode = "2";
            cond.TargetCodeTypeCode = "1";
            cond.ContractFlag = true;
            cond.ProductTypeCode = null;

            dsQuotationData expected = CreateExpectForCase17();
            dsQuotationData actual = null;
            string error = string.Empty;
            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case17";
                actual = target.GetQuotationData(cond);
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
                return CompareDsQuotationList(actual, expected, 17);
            else
                return string.Format(RESULT_FORMAT_ERROR, 17, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Get sale online quotation for contract
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = N0000000375
        ///             - Alphabet = AA
        ///             - ServiceTypeCode = 2
        ///             - TargetCodeTypeCode = 2
        ///             - ContractFlag = TRUE
        ///             - ProductTypeCode = NULL
        ///Expected  : See expectation test case 18
        ///</summary>
        public string Case18()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = "N0000000375";
            cond.Alphabet = "AA";
            cond.ServiceTypeCode = "2";
            cond.TargetCodeTypeCode = "2";
            cond.ContractFlag = true;
            cond.ProductTypeCode = null;

            dsQuotationData expected = CreateExpectForCase18();
            dsQuotationData actual = null;
            string error = string.Empty;
            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case18";
                actual = target.GetQuotationData(cond);
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
                return CompareDsQuotationList(actual, expected, 18);
            else
                return string.Format(RESULT_FORMAT_ERROR, 18, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Get beat guard quotation for contract
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = FSG0000000269
        ///             - Alphabet = AA
        ///             - ServiceTypeCode = 2
        ///             - TargetCodeTypeCode = 1
        ///             - ContractFlag = TRUE
        ///             - ProductTypeCode = NULL
        ///Expected  : See expectation test case 19
        ///</summary>
        public string Case19()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = "FSG0000000269";
            cond.Alphabet = "AA";
            cond.ServiceTypeCode = "2";
            cond.TargetCodeTypeCode = "1";
            cond.ContractFlag = true;
            cond.ProductTypeCode = null;

            dsQuotationData expected = CreateExpectForCase19();
            dsQuotationData actual = null;
            string error = string.Empty;
            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case19";
                actual = target.GetQuotationData(cond);
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
                return CompareDsQuotationList(actual, expected, 19);
            else
                return string.Format(RESULT_FORMAT_ERROR, 19, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Get sentry guard quotation for contract
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = SG0000000285
        ///             - Alphabet = AC
        ///             - ServiceTypeCode = 2
        ///             - TargetCodeTypeCode = 2
        ///             - ContractFlag = TRUE
        ///             - ProductTypeCode = NULL
        ///Expected  : See expectation test case 20
        ///</summary>
        public string Case20()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = "SG0000000285";
            cond.Alphabet = "AC";
            cond.ServiceTypeCode = "2";
            cond.TargetCodeTypeCode = "2";
            cond.ContractFlag = true;
            cond.ProductTypeCode = null;

            dsQuotationData expected = CreateExpectForCase20();
            dsQuotationData actual = null;
            string error = string.Empty;
            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case20";
                actual = target.GetQuotationData(cond);
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
                return CompareDsQuotationList(actual, expected, 20);
            else
                return string.Format(RESULT_FORMAT_ERROR, 20, "Fail", error);
        }

        ///<summary>
        ///Purpose   : Get maintenance quotation for contract
        ///Parameters:  doGetQuotationDataCondition
        ///             - QuotationTargetCode = FMA0000000083
        ///             - Alphabet = ZZ
        ///             - ServiceTypeCode = 2
        ///             - TargetCodeTypeCode = 1
        ///             - ContractFlag = TRUE
        ///             - ProductTypeCode = NULL
        ///Expected  : See expectation test case 21
        ///</summary>
        public string Case21()
        {
            IQuotationHandler target = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            doGetQuotationDataCondition cond = new doGetQuotationDataCondition();
            cond.QuotationTargetCode = "FMA0000000083";
            cond.Alphabet = "ZZ";
            cond.ServiceTypeCode = "2";
            cond.TargetCodeTypeCode = "1";
            cond.ContractFlag = true;
            cond.ProductTypeCode = null;

            dsQuotationData expected = CreateExpectForCase21();
            dsQuotationData actual = null;
            string error = string.Empty;
            try
            {
                CommonUtil.dsTransData.dtOperationData.GUID = "Case21";
                actual = target.GetQuotationData(cond);
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
                return CompareDsQuotationList(actual, expected, 21);
            else
                return string.Format(RESULT_FORMAT_ERROR, 21, "Fail", error);
        }

        private string CompareDsQuotationList(dsQuotationData actual, dsQuotationData expect, int caseNo)
        {
            #region targeted field
            string fieldQTarget = "QuotationTargetCode	BranchNameEN	BranchNameLC	BranchAddressEN	BranchAddressLC	ProductTypeCode	PrefixCode	ServiceTypeCode	TargetCodeTypeCode	QuotationOfficeCode	OperationOfficeCode	AcquisitionTypeCode	IntroducerCode	MotivationTypeCode	OldContractCode	QuotationStaffEmpNo	LastAlphabet	ContractTransferStatus	ContractCode	TransferDate	TransferAlphabet	CreateDate	CreateBy	UpdateDate	UpdateBy";
            string fieldQCustomer = "QuotationTargetCode	CustPartTypeCode	CustCode	CustNameEN	CustNameLC	CustFullNameEN	CustFullNameLC	RepPersonName	ContactPersonName	SECOMContactPerson	CustTypeCode	CompanyTypeCode	FinancialMarketTypeCode	BusinessTypeCode	PhoneNo	IDNo	RegionCode	URL	AddressFullEN	AddressEN	AlleyEN	RoadEN	SubDistrictEN	AddressFullLC	AddressLC	AlleyLC	RoadLC	SubDistrictLC	DistrictCode	ProvinceCode	ZipCode	CreateDate	CreateBy	UpdateDate	UpdateBy";
            string fieldQSite = "QuotationTargetCode	SiteCode	SiteNo	SiteNameEN	SiteNameLC	SECOMContactPerson	PersonInCharge	PhoneNo	BuildingUsageCode	AddressFullEN	AddressEN	AlleyEN	RoadEN	SubDistrictEN	AddressFullLC	AddressLC	AlleyLC	RoadLC	SubDistrictLC	DistrictCode	ProvinceCode	ZipCode	CreateDate	CreateBy	UpdateDate	UpdateBy";
            string fieldQBasic = "QuotationTargetCode	Alphabet	OriginateProgramId	OriginateRefNo	ProductCode	SecurityTypeCode	DispatchTypeCode	ContractDurationMonth	AutoRenewMonth	LastValidDate	ContractTransferStatus	LockStatus	LastOccNo	CurrentSecurityTypeCode	PhoneLineTypeCode1	PhoneLineTypeCode2	PhoneLineTypeCode3	PhoneLineOwnerTypeCode1	PhoneLineOwnerTypeCode2	PhoneLineOwnerTypeCode3	FireMonitorFlag	CrimePreventFlag	EmergencyReportFlag	FacilityMonitorFlag	BeatGuardFlag	SentryGuardFlag	MaintenanceFlag	SaleOnlineContractCode	PlanCode	SpecialInstallationFlag	PlannerEmpNo	PlanCheckerEmpNo	PlanCheckDate	PlanApproverEmpNo	PlanApproveDate	SiteBuildingArea	SecurityAreaFrom	SecurityAreaTo	MainStructureTypeCode	BuildingTypeCode	NewBldMgmtFlag	NewBldMgmtCost	NumOfBuilding	NumOfFloor	FacilityPassYear	FacilityPassMonth	SalesmanEmpNo1	SalesmanEmpNo2	SalesmanEmpNo3	SalesmanEmpNo4	SalesmanEmpNo5	SalesmanEmpNo6	SalesmanEmpNo7	SalesmanEmpNo8	SalesmanEmpNo9	SalesmanEmpNo10	SalesSupporterEmpNo	InsuranceTypeCode	InsuranceCoverageAmount	MonthlyInsuranceFee	MaintenanceFee1	MaintenanceFee2	BidGuaranteeAmount1	BidGuaranteeAmount2	AdditionalFee1	AdditionalFee2	AdditionalFee3	AdditionalApproveNo1	AdditionalApproveNo2	AdditionalApproveNo3	ApproveNo1	ApproveNo2	ApproveNo3	ApproveNo4	ApproveNo5	ContractFee	ProductPrice	InstallationFee	DepositFee	FacilityMemo	MaintenanceMemo	SecurityItemFee	OtherItemFee	SentryGuardAreaTypeCode	SentryGuardFee	TotalSentryGuardFee	MaintenanceTargetProductTypeCode	MaintenanceTypeCode	MaintenanceCycle	CreateDate	CreateBy	UpdateDate	UpdateBy";
            string fieldQOperType = "QuotationTargetCode	Alphabet	OperationTypeCode	CreateDate	CreateBy	UpdateDate	UpdateBy";
            string fieldQInstDet = "QuotationTargetCode	Alphabet	InstrumentCode	InstrumentQty	AddQty	RemoveQty	CreateDate	CreateBy	UpdateDate	UpdateBy";
            string fieldQFacDet = "QuotationTargetCode	Alphabet	FacilityCode	FacilityQty	CreateDate	CreateBy	UpdateDate	UpdateBy";
            string fieldQBeatGrdDet = "QuotationTargetCode	Alphabet	NumOfDayTimeWd	NumOfNightTimeWd	NumOfDayTimeSat	NumOfNightTimeSat	NumOfDayTimeSun	NumOfNightTimeSun	NumOfBeatStep	FreqOfGateUsage	NumOfClockKey	NumOfDate	NotifyTime	CreateDate	CreateBy	UpdateDate	UpdateBy";
            string fieldQSenGrdDet = "QuotationTargetCode	Alphabet	RunningNo	SentryGuardTypeCode	NumOfDate	SecurityStartTime	SecurityFinishTime	WorkHourPerMonth	CostPerHour	NumOfSentryGuard	CreateDate	CreateBy	UpdateDate	UpdateBy";
            string fieldQMaintLinkage = "QuotationTargetCode	Alphabet	ContractCode	CreateDate	CreateBy	UpdateDate	UpdateBy";
            #endregion

            #region compare result 
            bool resultQTarget = CompareObject<tbt_QuotationTarget>(actual.dtTbt_QuotationTarget, expect.dtTbt_QuotationTarget, fieldQTarget);
            bool resultQCustomer = CompareObjectList<tbt_QuotationCustomer>(actual.dtTbt_QuotationCustomer, expect.dtTbt_QuotationCustomer, fieldQCustomer);
            bool resultQSite = CompareObject<tbt_QuotationSite>(actual.dtTbt_QuotationSite, expect.dtTbt_QuotationSite, fieldQSite);
            bool resultQBasic = CompareObject<tbt_QuotationBasic>(actual.dtTbt_QuotationBasic, expect.dtTbt_QuotationBasic, fieldQBasic);
            bool resultQOperType = CompareObjectList<tbt_QuotationOperationType>(actual.dtTbt_QuotationOperationType, expect.dtTbt_QuotationOperationType, fieldQOperType);
            bool resultQInstDet = CompareObjectList<tbt_QuotationInstrumentDetails>(actual.dtTbt_QuotationInstrumentDetails, expect.dtTbt_QuotationInstrumentDetails, fieldQInstDet);
            bool resultQFacDet = CompareObjectList<tbt_QuotationFacilityDetails>(actual.dtTbt_QuotationFacilityDetails, expect.dtTbt_QuotationFacilityDetails, fieldQFacDet);
            bool resultQBeatGrdDet = CompareObject<tbt_QuotationBeatGuardDetails>(actual.dtTbt_QuotationBeatGuardDetails, expect.dtTbt_QuotationBeatGuardDetails, fieldQBeatGrdDet);
            bool resultQSenGrdDet = CompareObjectList<tbt_QuotationSentryGuardDetails>(actual.dtTbt_QuotationSentryGuardDetails, expect.dtTbt_QuotationSentryGuardDetails, fieldQSenGrdDet);
            bool resultQMaintLinkage = CompareObjectList<tbt_QuotationMaintenanceLinkage>(actual.dtTbt_QuotationMaintenanceLinkage, expect.dtTbt_QuotationMaintenanceLinkage, fieldQMaintLinkage);
            #endregion

            #region return string
            bool bResult = resultQTarget && resultQCustomer && resultQSite && resultQBasic && resultQOperType && resultQOperType && resultQInstDet && resultQFacDet && resultQBeatGrdDet && resultQSenGrdDet && resultQMaintLinkage;
            string result = string.Format(RESULT_FORMAT_LIST, caseNo, bResult);
            result +=  string.Format(RESULT_FORMAT_LIST_DATA, caseNo, "dtTbt_QuotationTarget", resultQTarget);
            result += string.Format(RESULT_FORMAT_LIST_DATA, caseNo, "dtTbt_QuotationCustomer", resultQCustomer);
            result += string.Format(RESULT_FORMAT_LIST_DATA, caseNo, "dtTbt_QuotationSite", resultQSite);
            result += string.Format(RESULT_FORMAT_LIST_DATA, caseNo, "dtTbt_QuotationBasic", resultQBasic);
            result += string.Format(RESULT_FORMAT_LIST_DATA, caseNo, "dtTbt_QuotationOperationType", resultQOperType);
            result += string.Format(RESULT_FORMAT_LIST_DATA, caseNo, "dtTbt_QuotationInstrumentDetails", resultQInstDet);
            result += string.Format(RESULT_FORMAT_LIST_DATA, caseNo, "dtTbt_QuotationFacilityDetails", resultQFacDet);
            result += string.Format(RESULT_FORMAT_LIST_DATA, caseNo, "dtTbt_QuotationBeatGuardDetails", resultQBeatGrdDet);
            result += string.Format(RESULT_FORMAT_LIST_DATA, caseNo, "dtTbt_QuotationSentryGuardDetails", resultQSenGrdDet);
            result += string.Format(RESULT_FORMAT_LIST_DATA, caseNo, "dtTbt_QuotationMaintenanceLinkage", resultQMaintLinkage);
            #endregion

            return result;
        }

        private dsQuotationData CreateExpectForCase15()
        {
            dsQuotationData dsQuotationData = new dsQuotationData();
            
            //dtTbt_QuotationTarget[]------------------------------------------------------------------------------------
            tbt_QuotationTarget qTarget = new tbt_QuotationTarget();
            qTarget.QuotationTargetCode = "N0000000362";
            qTarget.BranchNameEN = null;
            qTarget.BranchNameLC = "บริษัท บี-ควิค จำกัด (สาขาพัฒนาการ)";
            qTarget.BranchAddressEN = null;
            qTarget.BranchAddressLC = "2465 ถ. พัฒนาการ, แขวงสวนหลวง, เขตสวนหลวง,จ. กรุงเทพมหานคร 10850";
            qTarget.ProductTypeCode = "2";
            qTarget.PrefixCode = "N";
            qTarget.ServiceTypeCode = "2";
            qTarget.TargetCodeTypeCode = "2";
            qTarget.QuotationOfficeCode = "2000";
            qTarget.OperationOfficeCode = "2040";
            qTarget.AcquisitionTypeCode = "3";
            qTarget.IntroducerCode = null;
            qTarget.MotivationTypeCode = "10";
            qTarget.OldContractCode = null;
            qTarget.QuotationStaffEmpNo = "480396";
            qTarget.LastAlphabet = "AA";
            qTarget.ContractTransferStatus = "3";
            qTarget.ContractCode = "N0000000362";
            qTarget.TransferDate = new DateTime(2011, 10, 24);
            qTarget.TransferAlphabet = "AA";
            qTarget.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qTarget.CreateBy = "440065";
            qTarget.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qTarget.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationTarget = qTarget;

            //dtTbt_QuotationCustomer[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationCustomer = new List<tbt_QuotationCustomer>();

            tbt_QuotationCustomer qCustomer1 = new tbt_QuotationCustomer();
            qCustomer1.QuotationTargetCode = "N0000000362";
            qCustomer1.CustPartTypeCode = "1";
            qCustomer1.CustCode = "C0000000014";
            qCustomer1.CustNameEN = null;
            qCustomer1.CustNameLC = null;
            qCustomer1.CustFullNameEN = null;
            qCustomer1.CustFullNameLC = null;
            qCustomer1.RepPersonName = null;
            qCustomer1.ContactPersonName = null;
            qCustomer1.SECOMContactPerson = null;
            qCustomer1.CustTypeCode = null;
            qCustomer1.CompanyTypeCode = null;
            qCustomer1.FinancialMarketTypeCode = null;
            qCustomer1.BusinessTypeCode = null;
            qCustomer1.PhoneNo = null;
            qCustomer1.IDNo = null;
            qCustomer1.RegionCode = null;
            qCustomer1.URL = null;
            qCustomer1.AddressFullEN = null;
            qCustomer1.AddressEN = null;
            qCustomer1.AlleyEN = null;
            qCustomer1.RoadEN = null;
            qCustomer1.SubDistrictEN = null;
            qCustomer1.AddressFullLC = null;
            qCustomer1.AddressLC = null;
            qCustomer1.AlleyLC = null;
            qCustomer1.RoadLC = null;
            qCustomer1.SubDistrictLC = null;
            qCustomer1.DistrictCode = null;
            qCustomer1.ProvinceCode = null;
            qCustomer1.ZipCode = null;
            qCustomer1.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qCustomer1.CreateBy = "440065";
            qCustomer1.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qCustomer1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationCustomer.Add(qCustomer1);

            tbt_QuotationCustomer qCustomer2 = new tbt_QuotationCustomer();
            qCustomer2.QuotationTargetCode = "N0000000362";
            qCustomer2.CustPartTypeCode = "2";
            qCustomer2.CustCode = "C0000000014";
            qCustomer2.CustNameEN = null;
            qCustomer2.CustNameLC = null;
            qCustomer2.CustFullNameEN = null;
            qCustomer2.CustFullNameLC = null;
            qCustomer2.RepPersonName = null;
            qCustomer2.ContactPersonName = null;
            qCustomer2.SECOMContactPerson = null;
            qCustomer2.CustTypeCode = null;
            qCustomer2.CompanyTypeCode = null;
            qCustomer2.FinancialMarketTypeCode = null;
            qCustomer2.BusinessTypeCode = null;
            qCustomer2.PhoneNo = null;
            qCustomer2.IDNo = null;
            qCustomer2.RegionCode = null;
            qCustomer2.URL = null;
            qCustomer2.AddressFullEN = null;
            qCustomer2.AddressEN = null;
            qCustomer2.AlleyEN = null;
            qCustomer2.RoadEN = null;
            qCustomer2.SubDistrictEN = null;
            qCustomer2.AddressFullLC = null;
            qCustomer2.AddressLC = null;
            qCustomer2.AlleyLC = null;
            qCustomer2.RoadLC = null;
            qCustomer2.SubDistrictLC = null;
            qCustomer2.DistrictCode = null;
            qCustomer2.ProvinceCode = null;
            qCustomer2.ZipCode = null;
            qCustomer2.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qCustomer2.CreateBy = "440065";
            qCustomer2.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qCustomer2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationCustomer.Add(qCustomer2);

            //dtTbt_QuotationSite[]-----------------------------------------------------------------------------------
            tbt_QuotationSite qSite = new tbt_QuotationSite();
            qSite.QuotationTargetCode = "N0000000362";
            qSite.SiteCode = "S0000000014-0002";
            qSite.SiteNo = "0002";
            qSite.SiteNameEN = null;
            qSite.SiteNameLC = null;
            qSite.SECOMContactPerson = null;
            qSite.PersonInCharge = null;
            qSite.PhoneNo = null;
            qSite.BuildingUsageCode = null;
            qSite.AddressFullEN = null;
            qSite.AddressEN = null;
            qSite.AlleyEN = null;
            qSite.RoadEN = null;
            qSite.SubDistrictEN = null;
            qSite.AddressFullLC = null;
            qSite.AddressLC = null;
            qSite.AlleyLC = null;
            qSite.RoadLC = null;
            qSite.SubDistrictLC = null;
            qSite.DistrictCode = null;
            qSite.ProvinceCode = null;
            qSite.ZipCode = null;
            qSite.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qSite.CreateBy = "440065";
            qSite.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qSite.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSite = qSite;

            //dtTbt_QuotationBasic[]-----------------------------------------------------------------------------------
            tbt_QuotationBasic qBasic = new tbt_QuotationBasic();
            qBasic.QuotationTargetCode = "N0000000362";
            qBasic.Alphabet = "AA";
            qBasic.OriginateProgramId = "QUS030";
            qBasic.OriginateRefNo = null;
            qBasic.ProductCode = "044";
            qBasic.SecurityTypeCode = null;
            qBasic.DispatchTypeCode = "2";
            qBasic.ContractDurationMonth = 6;
            qBasic.AutoRenewMonth = 12;
            qBasic.LastValidDate = new DateTime(2012,10,12);
            qBasic.ContractTransferStatus = "3";
            qBasic.LockStatus = "1";
            qBasic.LastOccNo = "0001";
            qBasic.CurrentSecurityTypeCode = null;
            qBasic.PhoneLineTypeCode1 = null;
            qBasic.PhoneLineTypeCode2 = "3";
            qBasic.PhoneLineTypeCode3 = null;
            qBasic.PhoneLineOwnerTypeCode1 = null;
            qBasic.PhoneLineOwnerTypeCode2 = "1";
            qBasic.PhoneLineOwnerTypeCode3 = null;
            qBasic.FireMonitorFlag = false;
            qBasic.CrimePreventFlag = false;
            qBasic.EmergencyReportFlag = true;
            qBasic.FacilityMonitorFlag = false;
            qBasic.BeatGuardFlag = null;
            qBasic.SentryGuardFlag = null;
            qBasic.MaintenanceFlag = null;
            qBasic.SaleOnlineContractCode = null;
            qBasic.PlanCode = "ST.1007082";
            qBasic.SpecialInstallationFlag = false;
            qBasic.PlannerEmpNo = "440065";
            qBasic.PlanCheckerEmpNo = "490441";
            qBasic.PlanCheckDate = null;
            qBasic.PlanApproverEmpNo = "490459";
            qBasic.PlanApproveDate = new DateTime(2011, 8, 2);
            qBasic.SiteBuildingArea = 44444.75M;
            qBasic.SecurityAreaFrom = null;
            qBasic.SecurityAreaTo = 70000.75M;
            qBasic.MainStructureTypeCode = null;
            qBasic.BuildingTypeCode = "1";
            qBasic.NewBldMgmtFlag = false;
            qBasic.NewBldMgmtCost = null;
            qBasic.NumOfBuilding = 50;
            qBasic.NumOfFloor = null;
            qBasic.FacilityPassYear = null;
            qBasic.FacilityPassMonth = null;
            qBasic.SalesmanEmpNo1 = "510729";
            qBasic.SalesmanEmpNo2 = null;
            qBasic.SalesmanEmpNo3 = null;
            qBasic.SalesmanEmpNo4 = null;
            qBasic.SalesmanEmpNo5 = null;
            qBasic.SalesmanEmpNo6 = null;
            qBasic.SalesmanEmpNo7 = null;
            qBasic.SalesmanEmpNo8 = null;
            qBasic.SalesmanEmpNo9 = null;
            qBasic.SalesmanEmpNo10 = null;
            qBasic.SalesSupporterEmpNo = "440065";
            qBasic.InsuranceTypeCode = "1";
            qBasic.InsuranceCoverageAmount = null;
            qBasic.MonthlyInsuranceFee = 5555555555.7500M;
            qBasic.MaintenanceFee1 = 1111111111.2500M;
            qBasic.MaintenanceFee2 = null;
            qBasic.BidGuaranteeAmount1 = null;
            qBasic.BidGuaranteeAmount2 = null;
            qBasic.AdditionalFee1 = null;
            qBasic.AdditionalFee2 = 1111122222.0000M;
            qBasic.AdditionalFee3 = null;
            qBasic.AdditionalApproveNo1 = null;
            qBasic.AdditionalApproveNo2 = "AR-800000000008";
            qBasic.AdditionalApproveNo3 = null;
            qBasic.ApproveNo1 = null;
            qBasic.ApproveNo2 = "AR-000000000007";
            qBasic.ApproveNo3 = "AR-000000000005";
            qBasic.ApproveNo4 = null;
            qBasic.ApproveNo5 = "AR-000000000002";
            qBasic.ContractFee = 4575000.7500M;
            qBasic.ProductPrice = null;
            qBasic.InstallationFee = 0.0000M;
            qBasic.DepositFee = 500000.0000M;
            qBasic.FacilityMemo = null;
            qBasic.MaintenanceMemo = null;
            qBasic.SecurityItemFee = null;
            qBasic.OtherItemFee = null;
            qBasic.SentryGuardAreaTypeCode = null;
            qBasic.SentryGuardFee = null;
            qBasic.TotalSentryGuardFee = null;
            qBasic.MaintenanceTargetProductTypeCode = null;
            qBasic.MaintenanceTypeCode = null;
            qBasic.MaintenanceCycle = 6;
            qBasic.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBasic.CreateBy = "440065";
            qBasic.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBasic.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationBasic = qBasic;

            //dtTbt_QuotationOperationType[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationOperationType = new List<tbt_QuotationOperationType>();

            tbt_QuotationOperationType qOperType1 = new tbt_QuotationOperationType();
            qOperType1.QuotationTargetCode = "N0000000362";
            qOperType1.Alphabet = "AA";
            qOperType1.OperationTypeCode = "0";
            qOperType1.CreateDate = new DateTime(2011, 8, 24);
            qOperType1.CreateBy = "440065";
            qOperType1.UpdateDate = new DateTime(2011, 8, 24);
            qOperType1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationOperationType.Add(qOperType1);

            tbt_QuotationOperationType qOperType2 = new tbt_QuotationOperationType();
            qOperType2.QuotationTargetCode = "N0000000362";
            qOperType2.Alphabet = "AA";
            qOperType2.OperationTypeCode = "4";
            qOperType2.CreateDate = new DateTime(2011, 8, 24);
            qOperType2.CreateBy = "440065";
            qOperType2.UpdateDate = new DateTime(2011, 8, 24);
            qOperType2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationOperationType.Add(qOperType2);

            //dtTbt_QuotationInstrumentDetails[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationInstrumentDetails = new List<tbt_QuotationInstrumentDetails>();

            tbt_QuotationInstrumentDetails qInstDet1 = new tbt_QuotationInstrumentDetails();
            qInstDet1.QuotationTargetCode = "N0000000362";
            qInstDet1.Alphabet = "AA";
            qInstDet1.InstrumentCode = "AC-A1030TH";
            qInstDet1.InstrumentQty = 9999;
            qInstDet1.AddQty = 0;
            qInstDet1.RemoveQty = 0;
            qInstDet1.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet1.CreateBy = "440065";
            qInstDet1.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet1);

            tbt_QuotationInstrumentDetails qInstDet2 = new tbt_QuotationInstrumentDetails();
            qInstDet2.QuotationTargetCode = "N0000000362";
            qInstDet2.Alphabet = "AA";
            qInstDet2.InstrumentCode = "BX2+BAT-7.0AH+AUX-24-7.0";
            qInstDet2.InstrumentQty = 885;
            qInstDet2.AddQty = 0;
            qInstDet2.RemoveQty = 0;
            qInstDet2.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet2.CreateBy = "440065";
            qInstDet2.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet2);

            tbt_QuotationInstrumentDetails qInstDet3 = new tbt_QuotationInstrumentDetails();
            qInstDet3.QuotationTargetCode = "N0000000362";
            qInstDet3.Alphabet = "AA";
            qInstDet3.InstrumentCode = "GEA-CE4-D36P-IP";
            qInstDet3.InstrumentQty = 20;
            qInstDet3.AddQty = 0;
            qInstDet3.RemoveQty = 0;
            qInstDet3.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet3.CreateBy = "440065";
            qInstDet3.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet3.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet3);

            tbt_QuotationInstrumentDetails qInstDet4 = new tbt_QuotationInstrumentDetails();
            qInstDet4.QuotationTargetCode = "N0000000362";
            qInstDet4.Alphabet = "AA";
            qInstDet4.InstrumentCode = "HSG-IP65BIR";
            qInstDet4.InstrumentQty = 40;
            qInstDet4.AddQty = 0;
            qInstDet4.RemoveQty = 0;
            qInstDet4.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet4.CreateBy = "440065";
            qInstDet4.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet4.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet4);

            tbt_QuotationInstrumentDetails qInstDet5 = new tbt_QuotationInstrumentDetails();
            qInstDet5.QuotationTargetCode = "N0000000362";
            qInstDet5.Alphabet = "AA";
            qInstDet5.InstrumentCode = "JKT-03120";
            qInstDet5.InstrumentQty = 56;
            qInstDet5.AddQty = 0;
            qInstDet5.RemoveQty = 0;
            qInstDet5.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet5.CreateBy = "440065";
            qInstDet5.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet5.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet5);

            //dtTbt_QuotationFacilityDetails[]--------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationFacilityDetails = new List<tbt_QuotationFacilityDetails>();

            tbt_QuotationFacilityDetails qFacDet1 = new tbt_QuotationFacilityDetails();
            qFacDet1.QuotationTargetCode = "N0000000362";
            qFacDet1.Alphabet = "AA";
            qFacDet1.FacilityCode = "1003";
            qFacDet1.FacilityQty = 4;
            qFacDet1.CreateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet1.CreateBy = "440065";
            qFacDet1.UpdateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet1);

            tbt_QuotationFacilityDetails qFacDet2 = new tbt_QuotationFacilityDetails();
            qFacDet2.QuotationTargetCode = "N0000000362";
            qFacDet2.Alphabet = "AA";
            qFacDet2.FacilityCode = "1004";
            qFacDet2.FacilityQty = 800;
            qFacDet2.CreateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet2.CreateBy = "440065";
            qFacDet2.UpdateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet2);

            tbt_QuotationFacilityDetails qFacDet3 = new tbt_QuotationFacilityDetails();
            qFacDet3.QuotationTargetCode = "N0000000362";
            qFacDet3.Alphabet = "AA";
            qFacDet3.FacilityCode = "1014";
            qFacDet3.FacilityQty = 150;
            qFacDet3.CreateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet3.CreateBy = "440065";
            qFacDet3.UpdateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet3.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet3);

            tbt_QuotationFacilityDetails qFacDet4 = new tbt_QuotationFacilityDetails();
            qFacDet4.QuotationTargetCode = "N0000000362";
            qFacDet4.Alphabet = "AA";
            qFacDet4.FacilityCode = "1015";
            qFacDet4.FacilityQty = 75;
            qFacDet4.CreateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet4.CreateBy = "440065";
            qFacDet4.UpdateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet4.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet4);

            tbt_QuotationFacilityDetails qFacDet5 = new tbt_QuotationFacilityDetails();
            qFacDet5.QuotationTargetCode = "N0000000362";
            qFacDet5.Alphabet = "AA";
            qFacDet5.FacilityCode = "1099";
            qFacDet5.FacilityQty = 1;
            qFacDet5.CreateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet5.CreateBy = "440065";
            qFacDet5.UpdateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet5.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet5);

            tbt_QuotationFacilityDetails qFacDet6 = new tbt_QuotationFacilityDetails();
            qFacDet6.QuotationTargetCode = "N0000000362";
            qFacDet6.Alphabet = "AA";
            qFacDet6.FacilityCode = "2011";
            qFacDet6.FacilityQty = 9999;
            qFacDet6.CreateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet6.CreateBy = "440065";
            qFacDet6.UpdateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet6.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet6);

            tbt_QuotationFacilityDetails qFacDet7 = new tbt_QuotationFacilityDetails();
            qFacDet7.QuotationTargetCode = "N0000000362";
            qFacDet7.Alphabet = "AA";
            qFacDet7.FacilityCode = "7749";
            qFacDet7.FacilityQty = 742;
            qFacDet7.CreateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet7.CreateBy = "440065";
            qFacDet7.UpdateDate = new DateTime(2011, 8, 31, 20, 7, 19, 030);
            qFacDet7.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet7);

            //dtTbt_QuotationBeatGuardDetails[]-------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationBeatGuardDetails = null;

            //dtTbt_QuotationSentryGuardDetails[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationSentryGuardDetails = new List<tbt_QuotationSentryGuardDetails>();

            //dtTbt_QuotationMaintenanceLinkage[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationMaintenanceLinkage = new List<tbt_QuotationMaintenanceLinkage>();

            return dsQuotationData;
        }

        private dsQuotationData CreateExpectForCase16()
        {
            dsQuotationData dsQuotationData = new dsQuotationData();

            //dtTbt_QuotationTarget[]------------------------------------------------------------------------------------
            tbt_QuotationTarget qTarget = new tbt_QuotationTarget();
            qTarget.QuotationTargetCode = "Q0000000223";
            qTarget.BranchNameEN = "SIZZLER (C.P TOWER)";
            qTarget.BranchNameLC = "ซิสเลอร์ ซีพี ทาวน์เวอร์";
            qTarget.BranchAddressEN = "313 FL., 2 C.P. TOWER,SILOM RD.,SILOM, BANGRAK, BANGKOK 10180";
            qTarget.BranchAddressLC = "313 ชั้น 2 ซีพี ทาวน์เวอร์,ถ.สีลม, แขวงสีลม, เขตบางรัก, จ.กรุงเทพมหานคร 10180";
            qTarget.ProductTypeCode = "1";
            qTarget.PrefixCode = "Q";
            qTarget.ServiceTypeCode = "1";
            qTarget.TargetCodeTypeCode = "2";
            qTarget.QuotationOfficeCode = "2000";
            qTarget.OperationOfficeCode = "2040";
            qTarget.AcquisitionTypeCode = "0";
            qTarget.IntroducerCode = "C0000032";
            qTarget.MotivationTypeCode = "06";
            qTarget.OldContractCode = null;
            qTarget.QuotationStaffEmpNo = "440065";
            qTarget.LastAlphabet = "AA";
            qTarget.ContractTransferStatus = "3";
            qTarget.ContractCode = null;
            qTarget.TransferDate = null;
            qTarget.TransferAlphabet = null;
            qTarget.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qTarget.CreateBy = "440065";
            qTarget.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qTarget.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationTarget = qTarget;

            //dtTbt_QuotationCustomer[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationCustomer = new List<tbt_QuotationCustomer>();

            tbt_QuotationCustomer qCustomer1 = new tbt_QuotationCustomer();
            qCustomer1.QuotationTargetCode = "Q0000000223";
            qCustomer1.CustPartTypeCode = "1";
            qCustomer1.CustCode = "C0000000059";
            qCustomer1.CustNameEN = null;
            qCustomer1.CustNameLC = null;
            qCustomer1.CustFullNameEN = null;
            qCustomer1.CustFullNameLC = null;
            qCustomer1.RepPersonName = null;
            qCustomer1.ContactPersonName = null;
            qCustomer1.SECOMContactPerson = null;
            qCustomer1.CustTypeCode = null;
            qCustomer1.CompanyTypeCode = null;
            qCustomer1.FinancialMarketTypeCode = null;
            qCustomer1.BusinessTypeCode = null;
            qCustomer1.PhoneNo = null;
            qCustomer1.IDNo = null;
            qCustomer1.RegionCode = null;
            qCustomer1.URL = null;
            qCustomer1.AddressFullEN = null;
            qCustomer1.AddressEN = null;
            qCustomer1.AlleyEN = null;
            qCustomer1.RoadEN = null;
            qCustomer1.SubDistrictEN = null;
            qCustomer1.AddressFullLC = null;
            qCustomer1.AddressLC = null;
            qCustomer1.AlleyLC = null;
            qCustomer1.RoadLC = null;
            qCustomer1.SubDistrictLC = null;
            qCustomer1.DistrictCode = null;
            qCustomer1.ProvinceCode = null;
            qCustomer1.ZipCode = null;
            qCustomer1.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qCustomer1.CreateBy = "440065";
            qCustomer1.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qCustomer1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationCustomer.Add(qCustomer1);

            tbt_QuotationCustomer qCustomer2 = new tbt_QuotationCustomer();
            qCustomer2.QuotationTargetCode = "Q0000000223";
            qCustomer2.CustPartTypeCode = "2";
            qCustomer2.CustCode = "C0000000027";
            qCustomer2.CustNameEN = null;
            qCustomer2.CustNameLC = null;
            qCustomer2.CustFullNameEN = null;
            qCustomer2.CustFullNameLC = null;
            qCustomer2.RepPersonName = null;
            qCustomer2.ContactPersonName = null;
            qCustomer2.SECOMContactPerson = null;
            qCustomer2.CustTypeCode = null;
            qCustomer2.CompanyTypeCode = null;
            qCustomer2.FinancialMarketTypeCode = null;
            qCustomer2.BusinessTypeCode = null;
            qCustomer2.PhoneNo = null;
            qCustomer2.IDNo = null;
            qCustomer2.RegionCode = null;
            qCustomer2.URL = null;
            qCustomer2.AddressFullEN = null;
            qCustomer2.AddressEN = null;
            qCustomer2.AlleyEN = null;
            qCustomer2.RoadEN = null;
            qCustomer2.SubDistrictEN = null;
            qCustomer2.AddressFullLC = null;
            qCustomer2.AddressLC = null;
            qCustomer2.AlleyLC = null;
            qCustomer2.RoadLC = null;
            qCustomer2.SubDistrictLC = null;
            qCustomer2.DistrictCode = null;
            qCustomer2.ProvinceCode = null;
            qCustomer2.ZipCode = null;
            qCustomer2.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qCustomer2.CreateBy = "440065";
            qCustomer2.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qCustomer2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationCustomer.Add(qCustomer2);

            //dtTbt_QuotationSite[]-----------------------------------------------------------------------------------
            tbt_QuotationSite qSite = new tbt_QuotationSite();
            qSite.QuotationTargetCode = "Q0000000223";
            qSite.SiteCode = "S0000000027-0002";
            qSite.SiteNo = "0001";
            qSite.SiteNameEN = null;
            qSite.SiteNameLC = null;
            qSite.SECOMContactPerson = null;
            qSite.PersonInCharge = null;
            qSite.PhoneNo = null;
            qSite.BuildingUsageCode = null;
            qSite.AddressFullEN = null;
            qSite.AddressEN = null;
            qSite.AlleyEN = null;
            qSite.RoadEN = null;
            qSite.SubDistrictEN = null;
            qSite.AddressFullLC = null;
            qSite.AddressLC = null;
            qSite.AlleyLC = null;
            qSite.RoadLC = null;
            qSite.SubDistrictLC = null;
            qSite.DistrictCode = null;
            qSite.ProvinceCode = null;
            qSite.ZipCode = null;
            qSite.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qSite.CreateBy = "440065";
            qSite.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qSite.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSite = qSite;

            //dtTbt_QuotationBasic[]-----------------------------------------------------------------------------------
            tbt_QuotationBasic qBasic = new tbt_QuotationBasic();
            qBasic.QuotationTargetCode = "Q0000000223";
            qBasic.Alphabet = "AA";
            qBasic.OriginateProgramId = "QUS030";
            qBasic.OriginateRefNo = null;
            qBasic.ProductCode = "010";
            qBasic.SecurityTypeCode = null;
            qBasic.DispatchTypeCode = null;
            qBasic.ContractDurationMonth = 6;
            qBasic.AutoRenewMonth = 12;
            qBasic.LastValidDate = new DateTime(2012, 10, 12);
            qBasic.ContractTransferStatus = "1";
            qBasic.LockStatus = "0";
            qBasic.LastOccNo = "9990";
            qBasic.CurrentSecurityTypeCode = null;
            qBasic.PhoneLineTypeCode1 = null;
            qBasic.PhoneLineTypeCode2 = null;
            qBasic.PhoneLineTypeCode3 = null;
            qBasic.PhoneLineOwnerTypeCode1 = null;
            qBasic.PhoneLineOwnerTypeCode2 = null;
            qBasic.PhoneLineOwnerTypeCode3 = null;
            qBasic.FireMonitorFlag = null;
            qBasic.CrimePreventFlag = null;
            qBasic.EmergencyReportFlag = null;
            qBasic.FacilityMonitorFlag = null;
            qBasic.BeatGuardFlag = null;
            qBasic.SentryGuardFlag = null;
            qBasic.MaintenanceFlag = null;
            qBasic.SaleOnlineContractCode = null;
            qBasic.PlanCode = "GWEST.0106055";
            qBasic.SpecialInstallationFlag = true;
            qBasic.PlannerEmpNo = "440065";
            qBasic.PlanCheckerEmpNo = "510729";
            qBasic.PlanCheckDate = new DateTime(2011, 9, 11);
            qBasic.PlanApproverEmpNo = "480394";
            qBasic.PlanApproveDate = new DateTime(2011, 9, 15);
            qBasic.SiteBuildingArea = 99999.99M;
            qBasic.SecurityAreaFrom = 54000.00M;
            qBasic.SecurityAreaTo = 98888.88M;
            qBasic.MainStructureTypeCode = "05";
            qBasic.BuildingTypeCode = "1";
            qBasic.NewBldMgmtFlag = true;
            qBasic.NewBldMgmtCost = 8888888.8800M;
            qBasic.NumOfBuilding = null;
            qBasic.NumOfFloor = null;
            qBasic.FacilityPassYear = null;
            qBasic.FacilityPassMonth = null;
            qBasic.SalesmanEmpNo1 = "480396";
            qBasic.SalesmanEmpNo2 = "500576";
            qBasic.SalesmanEmpNo3 = "540902";
            qBasic.SalesmanEmpNo4 = "490459";
            qBasic.SalesmanEmpNo5 = "470228";
            qBasic.SalesmanEmpNo6 = "490488";
            qBasic.SalesmanEmpNo7 = "480288";
            qBasic.SalesmanEmpNo8 = "500629";
            qBasic.SalesmanEmpNo9 = "500575";
            qBasic.SalesmanEmpNo10 = "490418";
            qBasic.SalesSupporterEmpNo = null;
            qBasic.InsuranceTypeCode = null;
            qBasic.InsuranceCoverageAmount = null;
            qBasic.MonthlyInsuranceFee = null;
            qBasic.MaintenanceFee1 = null;
            qBasic.MaintenanceFee2 = null;
            qBasic.BidGuaranteeAmount1 = 1000000000.0000M;
            qBasic.BidGuaranteeAmount2 = 2500000000.0000M;
            qBasic.AdditionalFee1 = null;
            qBasic.AdditionalFee2 = null;
            qBasic.AdditionalFee3 = null;
            qBasic.AdditionalApproveNo1 = null;
            qBasic.AdditionalApproveNo2 = null;
            qBasic.AdditionalApproveNo3 = null;
            qBasic.ApproveNo1 = "AR-000000000001";
            qBasic.ApproveNo2 = "AR-000000000002";
            qBasic.ApproveNo3 = "AR-000000000003";
            qBasic.ApproveNo4 = "AR-000000000004";
            qBasic.ApproveNo5 = "AR-000000000005";
            qBasic.ContractFee = null;
            qBasic.ProductPrice = 9999999999.9900M;
            qBasic.InstallationFee = 8888888888.8800M;
            qBasic.DepositFee = null;
            qBasic.FacilityMemo = null;
            qBasic.MaintenanceMemo = null;
            qBasic.SecurityItemFee = null;
            qBasic.OtherItemFee = null;
            qBasic.SentryGuardAreaTypeCode = null;
            qBasic.SentryGuardFee = null;
            qBasic.TotalSentryGuardFee = null;
            qBasic.MaintenanceTargetProductTypeCode = null;
            qBasic.MaintenanceTypeCode = null;
            qBasic.MaintenanceCycle = null;
            qBasic.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBasic.CreateBy = "440065";
            qBasic.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qBasic.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationBasic = qBasic;

            //dtTbt_QuotationOperationType[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationOperationType = new List<tbt_QuotationOperationType>();
            
            //dtTbt_QuotationInstrumentDetails[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationInstrumentDetails = new List<tbt_QuotationInstrumentDetails>();

            tbt_QuotationInstrumentDetails qInstDet1 = new tbt_QuotationInstrumentDetails();
            qInstDet1.QuotationTargetCode = "Q0000000223";
            qInstDet1.Alphabet = "AA";
            qInstDet1.InstrumentCode = "AC-A1030TH";
            qInstDet1.InstrumentQty = 9999;
            qInstDet1.AddQty = 0;
            qInstDet1.RemoveQty = 0;
            qInstDet1.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet1.CreateBy = "440065";
            qInstDet1.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet1);

            tbt_QuotationInstrumentDetails qInstDet2 = new tbt_QuotationInstrumentDetails();
            qInstDet2.QuotationTargetCode = "Q0000000223";
            qInstDet2.Alphabet = "AA";
            qInstDet2.InstrumentCode = "BE-L012TH";
            qInstDet2.InstrumentQty = 5;
            qInstDet2.AddQty = 0;
            qInstDet2.RemoveQty = 0;
            qInstDet2.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet2.CreateBy = "440065";
            qInstDet2.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet2);

            tbt_QuotationInstrumentDetails qInstDet3 = new tbt_QuotationInstrumentDetails();
            qInstDet3.QuotationTargetCode = "Q0000000223";
            qInstDet3.Alphabet = "AA";
            qInstDet3.InstrumentCode = "BX2+BAT-7.0AH+AUX-24-7.0";
            qInstDet3.InstrumentQty = 885;
            qInstDet3.AddQty = 0;
            qInstDet3.RemoveQty = 0;
            qInstDet3.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet3.CreateBy = "440065";
            qInstDet3.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet3.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet3);

            tbt_QuotationInstrumentDetails qInstDet4 = new tbt_QuotationInstrumentDetails();
            qInstDet4.QuotationTargetCode = "Q0000000223";
            qInstDet4.Alphabet = "AA";
            qInstDet4.InstrumentCode = "GEA-CE4-D36P-IP";
            qInstDet4.InstrumentQty = 20;
            qInstDet4.AddQty = 0;
            qInstDet4.RemoveQty = 0;
            qInstDet4.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet4.CreateBy = "440065";
            qInstDet4.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet4.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet4);

            tbt_QuotationInstrumentDetails qInstDet5 = new tbt_QuotationInstrumentDetails();
            qInstDet5.QuotationTargetCode = "Q0000000223";
            qInstDet5.Alphabet = "AA";
            qInstDet5.InstrumentCode = "HSG-IP65BIR";
            qInstDet5.InstrumentQty = 40;
            qInstDet5.AddQty = 0;
            qInstDet5.RemoveQty = 0;
            qInstDet5.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet5.CreateBy = "440065";
            qInstDet5.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet5.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet5);

            tbt_QuotationInstrumentDetails qInstDet6 = new tbt_QuotationInstrumentDetails();
            qInstDet6.QuotationTargetCode = "Q0000000223";
            qInstDet6.Alphabet = "AA";
            qInstDet6.InstrumentCode = "JKT-03120";
            qInstDet6.InstrumentQty = 56;
            qInstDet6.AddQty = 0;
            qInstDet6.RemoveQty = 0;
            qInstDet6.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet6.CreateBy = "440065";
            qInstDet6.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet6.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet6);

            tbt_QuotationInstrumentDetails qInstDet7 = new tbt_QuotationInstrumentDetails();
            qInstDet7.QuotationTargetCode = "Q0000000223";
            qInstDet7.Alphabet = "AA";
            qInstDet7.InstrumentCode = "PB-IN-100HF";
            qInstDet7.InstrumentQty = 486;
            qInstDet7.AddQty = 0;
            qInstDet7.RemoveQty = 0;
            qInstDet7.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet7.CreateBy = "440065";
            qInstDet7.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet7.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet7);

            tbt_QuotationInstrumentDetails qInstDet8 = new tbt_QuotationInstrumentDetails();
            qInstDet8.QuotationTargetCode = "Q0000000223";
            qInstDet8.Alphabet = "AA";
            qInstDet8.InstrumentCode = "PB-N0120";
            qInstDet8.InstrumentQty = 55;
            qInstDet8.AddQty = 0;
            qInstDet8.RemoveQty = 0;
            qInstDet8.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet8.CreateBy = "440065";
            qInstDet8.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet8.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet8);

            tbt_QuotationInstrumentDetails qInstDet9 = new tbt_QuotationInstrumentDetails();
            qInstDet9.QuotationTargetCode = "Q0000000223";
            qInstDet9.Alphabet = "AA";
            qInstDet9.InstrumentCode = "PI-S9000";
            qInstDet9.InstrumentQty = 9;
            qInstDet9.AddQty = 0;
            qInstDet9.RemoveQty = 0;
            qInstDet9.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet9.CreateBy = "440065";
            qInstDet9.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet9.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet9);
            
            tbt_QuotationInstrumentDetails qInstDet10 = new tbt_QuotationInstrumentDetails();
            qInstDet10.QuotationTargetCode = "Q0000000223";
            qInstDet10.Alphabet = "AA";
            qInstDet10.InstrumentCode = "RACK-C***";
            qInstDet10.InstrumentQty = 70;
            qInstDet10.AddQty = 0;
            qInstDet10.RemoveQty = 0;
            qInstDet10.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet10.CreateBy = "440065";
            qInstDet10.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet10.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet10);
            
            tbt_QuotationInstrumentDetails qInstDet11 = new tbt_QuotationInstrumentDetails();
            qInstDet11.QuotationTargetCode = "Q0000000223";
            qInstDet11.Alphabet = "AA";
            qInstDet11.InstrumentCode = "SMK-2000A";
            qInstDet11.InstrumentQty = 60;
            qInstDet11.AddQty = 0;
            qInstDet11.RemoveQty = 0;
            qInstDet11.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet11.CreateBy = "440065";
            qInstDet11.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet11.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet11);
            
            tbt_QuotationInstrumentDetails qInstDet12 = new tbt_QuotationInstrumentDetails();
            qInstDet12.QuotationTargetCode = "Q0000000223";
            qInstDet12.Alphabet = "AA";
            qInstDet12.InstrumentCode = "SP-K0030";
            qInstDet12.InstrumentQty = 248;
            qInstDet12.AddQty = 0;
            qInstDet12.RemoveQty = 0;
            qInstDet12.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet12.CreateBy = "440065";
            qInstDet12.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet12.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet12);
            
            tbt_QuotationInstrumentDetails qInstDet13 = new tbt_QuotationInstrumentDetails();
            qInstDet13.QuotationTargetCode = "Q0000000223";
            qInstDet13.Alphabet = "AA";
            qInstDet13.InstrumentCode = "SWC-160P";
            qInstDet13.InstrumentQty = 89;
            qInstDet13.AddQty = 0;
            qInstDet13.RemoveQty = 0;
            qInstDet13.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet13.CreateBy = "440065";
            qInstDet13.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet13.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet13);
            
            tbt_QuotationInstrumentDetails qInstDet14 = new tbt_QuotationInstrumentDetails();
            qInstDet14.QuotationTargetCode = "Q0000000223";
            qInstDet14.Alphabet = "AA";
            qInstDet14.InstrumentCode = "UL135S";
            qInstDet14.InstrumentQty = 44;
            qInstDet14.AddQty = 0;
            qInstDet14.RemoveQty = 0;
            qInstDet14.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet14.CreateBy = "440065";
            qInstDet14.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet14.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet14);
            
            tbt_QuotationInstrumentDetails qInstDet15 = new tbt_QuotationInstrumentDetails();
            qInstDet15.QuotationTargetCode = "Q0000000223";
            qInstDet15.Alphabet = "AA";
            qInstDet15.InstrumentCode = "WV-Q118";
            qInstDet15.InstrumentQty = 100;
            qInstDet15.AddQty = 0;
            qInstDet15.RemoveQty = 0;
            qInstDet15.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet15.CreateBy = "440065";
            qInstDet15.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet15.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet15);
            
            tbt_QuotationInstrumentDetails qInstDet16 = new tbt_QuotationInstrumentDetails();
            qInstDet16.QuotationTargetCode = "Q0000000223";
            qInstDet16.Alphabet = "AA";
            qInstDet16.InstrumentCode = "YV10X5B-SA2-PRO";
            qInstDet16.InstrumentQty = 238;
            qInstDet16.AddQty = 0;
            qInstDet16.RemoveQty = 0;
            qInstDet16.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet16.CreateBy = "440065";
            qInstDet16.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet16.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet16);

            tbt_QuotationInstrumentDetails qInstDet17 = new tbt_QuotationInstrumentDetails();
            qInstDet17.QuotationTargetCode = "Q0000000223";
            qInstDet17.Alphabet = "AA";
            qInstDet17.InstrumentCode = "ZR-DHD1621NP***";
            qInstDet17.InstrumentQty = 150;
            qInstDet17.AddQty = 0;
            qInstDet17.RemoveQty = 0;
            qInstDet17.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet17.CreateBy = "440065";
            qInstDet17.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet17.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet17);

            //dtTbt_QuotationFacilityDetails[]--------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationFacilityDetails = new List<tbt_QuotationFacilityDetails>();

            //dtTbt_QuotationBeatGuardDetails[]-------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationBeatGuardDetails = null;

            //dtTbt_QuotationSentryGuardDetails[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationSentryGuardDetails = new List<tbt_QuotationSentryGuardDetails>();

            //dtTbt_QuotationMaintenanceLinkage[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationMaintenanceLinkage = new List<tbt_QuotationMaintenanceLinkage>();

            return dsQuotationData;
        }

        private dsQuotationData CreateExpectForCase17()
        {
            dsQuotationData dsQuotationData = new dsQuotationData();

            //dtTbt_QuotationTarget[]------------------------------------------------------------------------------------
            tbt_QuotationTarget qTarget = new tbt_QuotationTarget();
            qTarget.QuotationTargetCode = "FN0000000358";
            qTarget.BranchNameEN = null;
            qTarget.BranchNameLC = null;
            qTarget.BranchAddressEN = null;
            qTarget.BranchAddressLC = null;
            qTarget.ProductTypeCode = "2";
            qTarget.PrefixCode = "FN";
            qTarget.ServiceTypeCode = "2";
            qTarget.TargetCodeTypeCode = "1";
            qTarget.QuotationOfficeCode = "7000";
            qTarget.OperationOfficeCode = "7010";
            qTarget.AcquisitionTypeCode = null;
            qTarget.IntroducerCode = null;
            qTarget.MotivationTypeCode = null;
            qTarget.OldContractCode = null;
            qTarget.QuotationStaffEmpNo = "470228";
            qTarget.LastAlphabet = "AB";
            qTarget.ContractTransferStatus = "1";
            qTarget.ContractCode = null;
            qTarget.TransferDate = null;
            qTarget.TransferAlphabet = null;
            qTarget.CreateDate = new DateTime(2011, 7, 9, 12, 9, 0); 
            qTarget.CreateBy = "440065";
            qTarget.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qTarget.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationTarget = qTarget;

            //dtTbt_QuotationCustomer[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationCustomer = new List<tbt_QuotationCustomer>();

            tbt_QuotationCustomer qCustomer = new tbt_QuotationCustomer();
            qCustomer.QuotationTargetCode = "FN0000000358";
            qCustomer.CustPartTypeCode = "1";
            qCustomer.CustCode = null;
            qCustomer.CustNameEN = "EASY BUY";
            qCustomer.CustNameLC = "อีซี่ บาย";
            qCustomer.CustFullNameEN = "EASY BUY PUBLIC COMPANY LIMITED";
            qCustomer.CustFullNameLC = "บริษัท อีซี่ บาย จำกัด (มหาชน)";
            qCustomer.RepPersonName = null;
            qCustomer.ContactPersonName = null;
            qCustomer.SECOMContactPerson = null;
            qCustomer.CustTypeCode = "0";
            qCustomer.CompanyTypeCode = "04";
            qCustomer.FinancialMarketTypeCode = null;
            qCustomer.BusinessTypeCode = "025";
            qCustomer.PhoneNo = null;
            qCustomer.IDNo = null;
            qCustomer.RegionCode = "TH";
            qCustomer.URL = null;
            qCustomer.AddressFullEN = "952 RAMALAND BLDG., 13TH FL., RAMA IV, SURIYAWONG, BANGRAK, BANGKOK 10500";
            qCustomer.AddressEN = "952 RAMALAND BLDG., 13TH FL.";
            qCustomer.AlleyEN = null;
            qCustomer.RoadEN = "RAMA IV";
            qCustomer.SubDistrictEN = "SURIYAWONG";
            qCustomer.AddressFullLC = "952 อาคารรามาแลนด์ ชั้น 13 ถ.พระราม 4 แขวงสุริยวงศ์ เขตบางรัก จ.กรุงเทพมหานคร 10500";
            qCustomer.AddressLC = "952 อาคารรามาแลนด์ ชั้น 13 ";
            qCustomer.AlleyLC = null;
            qCustomer.RoadLC = "พระราม 4";
            qCustomer.SubDistrictLC = "สุริยวงศ์";
            qCustomer.DistrictCode = "00010";
            qCustomer.ProvinceCode = "001";
            qCustomer.ZipCode = "10500";
            qCustomer.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qCustomer.CreateBy = "440065";
            qCustomer.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qCustomer.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationCustomer.Add(qCustomer);

            //dtTbt_QuotationSite[]-----------------------------------------------------------------------------------
            tbt_QuotationSite qSite = new tbt_QuotationSite();
            qSite.QuotationTargetCode = "FN0000000358";
            qSite.SiteCode = null;
            qSite.SiteNo = null;
            qSite.SiteNameEN = "EASY BUY PUBLIC COMPANY LIMITED (MBK CENTER)";
            qSite.SiteNameLC = "บริษัท อีซี่ บาย จำกัด (มหาชน) - สาขามาบุญครองเซ็นเตอร์";
            qSite.SECOMContactPerson = null;
            qSite.PersonInCharge = null;
            qSite.PhoneNo = null;
            qSite.BuildingUsageCode = "017";
            qSite.AddressFullEN = "444 MBK CENTER, 4TH FL., ROOM 1, PHAYATHAI RD., WANGMAI, PATUMWAN, BANGKOK 10330";
            qSite.AddressEN = "444 MBK CENTER, 4TH FL., ROOM 1";
            qSite.AlleyEN = null;
            qSite.RoadEN = "PHAYATHAI";
            qSite.SubDistrictEN = "WANGMAI";
            qSite.AddressFullLC = "444 อาคาร เอ็ม บี เค เซ็นเตอร์ ชั้น 4 ถ.พญาไท แขวงวังใหม่ เขตปทุมวัน จ.กรุงเทพมหานคร 10330 ";
            qSite.AddressLC = "444 อาคาร เอ็ม บี เค เซ็นเตอร์ ชั้น 4";
            qSite.AlleyLC = null;
            qSite.RoadLC = "พญาไท";
            qSite.SubDistrictLC = "วังใหม่";
            qSite.DistrictCode = "00031";
            qSite.ProvinceCode = "001";
            qSite.ZipCode = "10330";
            qSite.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qSite.CreateBy = "440065";
            qSite.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qSite.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSite = qSite;

            //dtTbt_QuotationBasic[]-----------------------------------------------------------------------------------
            tbt_QuotationBasic qBasic = new tbt_QuotationBasic();
            qBasic.QuotationTargetCode = "FN0000000358";
            qBasic.Alphabet = "AB";
            qBasic.OriginateProgramId = "QUS030";
            qBasic.OriginateRefNo = null;
            qBasic.ProductCode = "009";
            qBasic.SecurityTypeCode = null;
            qBasic.DispatchTypeCode = null;
            qBasic.ContractDurationMonth = 6;
            qBasic.AutoRenewMonth = 12;
            qBasic.LastValidDate = new DateTime(2012, 8, 12);
            qBasic.ContractTransferStatus = "1";
            qBasic.LockStatus = "0";
            qBasic.LastOccNo = null;
            qBasic.CurrentSecurityTypeCode = null;
            qBasic.PhoneLineTypeCode1 = null;
            qBasic.PhoneLineTypeCode2 = null;
            qBasic.PhoneLineTypeCode3 = null;
            qBasic.PhoneLineOwnerTypeCode1 = null;
            qBasic.PhoneLineOwnerTypeCode2 = null;
            qBasic.PhoneLineOwnerTypeCode3 = null;
            qBasic.FireMonitorFlag = false;
            qBasic.CrimePreventFlag = true;
            qBasic.EmergencyReportFlag = false;
            qBasic.FacilityMonitorFlag = false;
            qBasic.BeatGuardFlag = null;
            qBasic.SentryGuardFlag = null;
            qBasic.MaintenanceFlag = null;
            qBasic.SaleOnlineContractCode = null;
            qBasic.PlanCode = "ST.1007082";
            qBasic.SpecialInstallationFlag = false;
            qBasic.PlannerEmpNo = "440065";
            qBasic.PlanCheckerEmpNo = "490441";
            qBasic.PlanCheckDate = null;
            qBasic.PlanApproverEmpNo = "490459";
            qBasic.PlanApproveDate = null;
            qBasic.SiteBuildingArea = null;
            qBasic.SecurityAreaFrom = null;
            qBasic.SecurityAreaTo = null;
            qBasic.MainStructureTypeCode = null;
            qBasic.BuildingTypeCode = null;
            qBasic.NewBldMgmtFlag = false;
            qBasic.NewBldMgmtCost = null;
            qBasic.NumOfBuilding = null;
            qBasic.NumOfFloor = null;
            qBasic.FacilityPassYear = null;
            qBasic.FacilityPassMonth = null;
            qBasic.SalesmanEmpNo1 = "510729";
            qBasic.SalesmanEmpNo2 = null;
            qBasic.SalesmanEmpNo3 = null;
            qBasic.SalesmanEmpNo4 = null;
            qBasic.SalesmanEmpNo5 = null;
            qBasic.SalesmanEmpNo6 = null;
            qBasic.SalesmanEmpNo7 = null;
            qBasic.SalesmanEmpNo8 = null;
            qBasic.SalesmanEmpNo9 = null;
            qBasic.SalesmanEmpNo10 = null;
            qBasic.SalesSupporterEmpNo = null;
            qBasic.InsuranceTypeCode = "0";
            qBasic.InsuranceCoverageAmount = null;
            qBasic.MonthlyInsuranceFee = null;
            qBasic.MaintenanceFee1 = null;
            qBasic.MaintenanceFee2 = null;
            qBasic.BidGuaranteeAmount1 = null;
            qBasic.BidGuaranteeAmount2 = null;
            qBasic.AdditionalFee1 = null;
            qBasic.AdditionalFee2 = null;
            qBasic.AdditionalFee3 = null;
            qBasic.AdditionalApproveNo1 = null;
            qBasic.AdditionalApproveNo2 = null;
            qBasic.AdditionalApproveNo3 = null;
            qBasic.ApproveNo1 = null;
            qBasic.ApproveNo2 = null;
            qBasic.ApproveNo3 = null;
            qBasic.ApproveNo4 = null;
            qBasic.ApproveNo5 = null;
            qBasic.ContractFee = 4575000.7500M;
            qBasic.ProductPrice = null;
            qBasic.InstallationFee = 0.0000M;
            qBasic.DepositFee = 500000.0000M;
            qBasic.FacilityMemo = null;
            qBasic.MaintenanceMemo = null;
            qBasic.SecurityItemFee = null;
            qBasic.OtherItemFee = null;
            qBasic.SentryGuardAreaTypeCode = null;
            qBasic.SentryGuardFee = null;
            qBasic.TotalSentryGuardFee = null;
            qBasic.MaintenanceTargetProductTypeCode = null;
            qBasic.MaintenanceTypeCode = null;
            qBasic.MaintenanceCycle = 6;
            qBasic.CreateDate = new DateTime(2011, 7, 9, 12, 9, 0); 
            qBasic.CreateBy = "440065";
            qBasic.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0); 
            qBasic.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationBasic = qBasic;

            //dtTbt_QuotationOperationType[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationOperationType = new List<tbt_QuotationOperationType>();

            //dtTbt_QuotationInstrumentDetails[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationInstrumentDetails = new List<tbt_QuotationInstrumentDetails>();

            tbt_QuotationInstrumentDetails qInstDet1 = new tbt_QuotationInstrumentDetails();
            qInstDet1.QuotationTargetCode = "FN0000000358";
            qInstDet1.Alphabet = "AB";
            qInstDet1.InstrumentCode = "AC-A1030TH";
            qInstDet1.InstrumentQty = 9999;
            qInstDet1.AddQty = 0;
            qInstDet1.RemoveQty = 0;
            qInstDet1.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet1.CreateBy = "440065";
            qInstDet1.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet1);

            tbt_QuotationInstrumentDetails qInstDet2 = new tbt_QuotationInstrumentDetails();
            qInstDet2.QuotationTargetCode = "FN0000000358";
            qInstDet2.Alphabet = "AB";
            qInstDet2.InstrumentCode = "BX2+BAT-7.0AH+AUX-24-7.0";
            qInstDet2.InstrumentQty = 885;
            qInstDet2.AddQty = 0;
            qInstDet2.RemoveQty = 0;
            qInstDet2.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet2.CreateBy = "440065";
            qInstDet2.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet2);

            tbt_QuotationInstrumentDetails qInstDet3 = new tbt_QuotationInstrumentDetails();
            qInstDet3.QuotationTargetCode = "FN0000000358";
            qInstDet3.Alphabet = "AB";
            qInstDet3.InstrumentCode = "IQ-600NB";
            qInstDet3.InstrumentQty = 0;
            qInstDet3.AddQty = 0;
            qInstDet3.RemoveQty = 0;
            qInstDet3.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet3.CreateBy = "440065";
            qInstDet3.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet3.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet3);

            tbt_QuotationInstrumentDetails qInstDet4 = new tbt_QuotationInstrumentDetails();
            qInstDet4.QuotationTargetCode = "FN0000000358";
            qInstDet4.Alphabet = "AB";
            qInstDet4.InstrumentCode = "JKT-03120";
            qInstDet4.InstrumentQty = 56;
            qInstDet4.AddQty = 0;
            qInstDet4.RemoveQty = 0;
            qInstDet4.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet4.CreateBy = "440065";
            qInstDet4.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet4.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet4);

            tbt_QuotationInstrumentDetails qInstDet5 = new tbt_QuotationInstrumentDetails();
            qInstDet5.QuotationTargetCode = "FN0000000358";
            qInstDet5.Alphabet = "AB";
            qInstDet5.InstrumentCode = "RACK-C***";
            qInstDet5.InstrumentQty = 70;
            qInstDet5.AddQty = 0;
            qInstDet5.RemoveQty = 0;
            qInstDet5.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet5.CreateBy = "440065";
            qInstDet5.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet5.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet5);

            tbt_QuotationInstrumentDetails qInstDet6 = new tbt_QuotationInstrumentDetails();
            qInstDet6.QuotationTargetCode = "FN0000000358";
            qInstDet6.Alphabet = "AB";
            qInstDet6.InstrumentCode = "SMK-2000A";
            qInstDet6.InstrumentQty = 60;
            qInstDet6.AddQty = 0;
            qInstDet6.RemoveQty = 0;
            qInstDet6.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet6.CreateBy = "440065";
            qInstDet6.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet6.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet6);
            
            tbt_QuotationInstrumentDetails qInstDet7 = new tbt_QuotationInstrumentDetails();
            qInstDet7.QuotationTargetCode = "FN0000000358";
            qInstDet7.Alphabet = "AB";
            qInstDet7.InstrumentCode = "SP-K0030";
            qInstDet7.InstrumentQty = 248;
            qInstDet7.AddQty = 0;
            qInstDet7.RemoveQty = 0;
            qInstDet7.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet7.CreateBy = "440065";
            qInstDet7.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet7.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet7);
            
            tbt_QuotationInstrumentDetails qInstDet8 = new tbt_QuotationInstrumentDetails();
            qInstDet8.QuotationTargetCode = "FN0000000358";
            qInstDet8.Alphabet = "AB";
            qInstDet8.InstrumentCode = "UL135S";
            qInstDet8.InstrumentQty = 0;
            qInstDet8.AddQty = 0;
            qInstDet8.RemoveQty = 0;
            qInstDet8.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet8.CreateBy = "440065";
            qInstDet8.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet8.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet8);

            tbt_QuotationInstrumentDetails qInstDet9 = new tbt_QuotationInstrumentDetails();
            qInstDet9.QuotationTargetCode = "FN0000000358";
            qInstDet9.Alphabet = "AB";
            qInstDet9.InstrumentCode = "ZR-DHD1621NP***";
            qInstDet9.InstrumentQty = 0;
            qInstDet9.AddQty = 0;
            qInstDet9.RemoveQty = 0;
            qInstDet9.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet9.CreateBy = "440065";
            qInstDet9.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qInstDet9.UpdateBy = "440065";  
            dsQuotationData.dtTbt_QuotationInstrumentDetails.Add(qInstDet9);

            //dtTbt_QuotationFacilityDetails[]--------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationFacilityDetails = new List<tbt_QuotationFacilityDetails>();

            //dtTbt_QuotationBeatGuardDetails[]-------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationBeatGuardDetails = null;

            //dtTbt_QuotationSentryGuardDetails[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationSentryGuardDetails = new List<tbt_QuotationSentryGuardDetails>();

            //dtTbt_QuotationMaintenanceLinkage[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationMaintenanceLinkage = new List<tbt_QuotationMaintenanceLinkage>();

            return dsQuotationData;
        }

        private dsQuotationData CreateExpectForCase18()
        {
            dsQuotationData dsQuotationData = new dsQuotationData();

            //dtTbt_QuotationTarget[]------------------------------------------------------------------------------------
            tbt_QuotationTarget qTarget = new tbt_QuotationTarget();
            qTarget.QuotationTargetCode = "N0000000375";
            qTarget.BranchNameEN = "SIZZLER (C.P TOWER)";
            qTarget.BranchNameLC = "ซิสเลอร์ ซีพี ทาวน์เวอร์";
            qTarget.BranchAddressEN = "313 FL., 2 C.P. TOWER,SILOM RD.,SILOM, BANGRAK, BANGKOK 10180";
            qTarget.BranchAddressLC = "313 ชั้น 2 ซีพี ทาวน์เวอร์,ถ.สีลม, แขวงสีลม, เขตบางรัก, จ.กรุงเทพมหานคร 10180";
            qTarget.ProductTypeCode = "3";
            qTarget.PrefixCode = "N";
            qTarget.ServiceTypeCode = "2";
            qTarget.TargetCodeTypeCode = "2";
            qTarget.QuotationOfficeCode = "2000";
            qTarget.OperationOfficeCode = "2040";
            qTarget.AcquisitionTypeCode = "0";
            qTarget.IntroducerCode = "C0000032";
            qTarget.MotivationTypeCode = "06";
            qTarget.OldContractCode = "N0000000123";
            qTarget.QuotationStaffEmpNo = "440065";
            qTarget.LastAlphabet = "AA";
            qTarget.ContractTransferStatus = "3";
            qTarget.ContractCode = null;
            qTarget.TransferDate = null;
            qTarget.TransferAlphabet = null;
            qTarget.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qTarget.CreateBy = "440065";
            qTarget.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qTarget.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationTarget = qTarget;

            //dtTbt_QuotationCustomer[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationCustomer = new List<tbt_QuotationCustomer>();
            tbt_QuotationCustomer qCustomer1 = new tbt_QuotationCustomer();
            qCustomer1.QuotationTargetCode = "N0000000375";
            qCustomer1.CustPartTypeCode = "1";
            qCustomer1.CustCode = "C0000000059";
            qCustomer1.CustNameEN = null;
            qCustomer1.CustNameLC = null;
            qCustomer1.CustFullNameEN = null;
            qCustomer1.CustFullNameLC = null;
            qCustomer1.RepPersonName = null;
            qCustomer1.ContactPersonName = null;
            qCustomer1.SECOMContactPerson = null;
            qCustomer1.CustTypeCode = null;
            qCustomer1.CompanyTypeCode = null;
            qCustomer1.FinancialMarketTypeCode = null;
            qCustomer1.BusinessTypeCode = null;
            qCustomer1.PhoneNo = null;
            qCustomer1.IDNo = null;
            qCustomer1.RegionCode = null;
            qCustomer1.URL = null;
            qCustomer1.AddressFullEN = null;
            qCustomer1.AddressEN = null;
            qCustomer1.AlleyEN = null;
            qCustomer1.RoadEN = null;
            qCustomer1.SubDistrictEN = null;
            qCustomer1.AddressFullLC = null;
            qCustomer1.AddressLC = null;
            qCustomer1.AlleyLC = null;
            qCustomer1.RoadLC = null;
            qCustomer1.SubDistrictLC = null;
            qCustomer1.DistrictCode = null;
            qCustomer1.ProvinceCode = null;
            qCustomer1.ZipCode = null;
            qCustomer1.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer1.CreateBy = "440065";
            qCustomer1.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationCustomer.Add(qCustomer1);

            tbt_QuotationCustomer qCustomer2 = new tbt_QuotationCustomer();
            qCustomer2.QuotationTargetCode = "N0000000375";
            qCustomer2.CustPartTypeCode = "2";
            qCustomer2.CustCode = "C0000000027";
            qCustomer2.CustNameEN = null;
            qCustomer2.CustNameLC = null;
            qCustomer2.CustFullNameEN = null;
            qCustomer2.CustFullNameLC = null;
            qCustomer2.RepPersonName = null;
            qCustomer2.ContactPersonName = null;
            qCustomer2.SECOMContactPerson = null;
            qCustomer2.CustTypeCode = null;
            qCustomer2.CompanyTypeCode = null;
            qCustomer2.FinancialMarketTypeCode = null;
            qCustomer2.BusinessTypeCode = null;
            qCustomer2.PhoneNo = null;
            qCustomer2.IDNo = null;
            qCustomer2.RegionCode = null;
            qCustomer2.URL = null;
            qCustomer2.AddressFullEN = null;
            qCustomer2.AddressEN = null;
            qCustomer2.AlleyEN = null;
            qCustomer2.RoadEN = null;
            qCustomer2.SubDistrictEN = null;
            qCustomer2.AddressFullLC = null;
            qCustomer2.AddressLC = null;
            qCustomer2.AlleyLC = null;
            qCustomer2.RoadLC = null;
            qCustomer2.SubDistrictLC = null;
            qCustomer2.DistrictCode = null;
            qCustomer2.ProvinceCode = null;
            qCustomer2.ZipCode = null;
            qCustomer2.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer2.CreateBy = "440065";
            qCustomer2.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationCustomer.Add(qCustomer2);

            //dtTbt_QuotationSite[]-----------------------------------------------------------------------------------
            tbt_QuotationSite qSite = new tbt_QuotationSite();
            qSite.QuotationTargetCode = "N0000000375";
            qSite.SiteCode = "S0000000027-0002";
            qSite.SiteNo = "0002";
            qSite.SiteNameEN = null;
            qSite.SiteNameLC = null;
            qSite.SECOMContactPerson = null;
            qSite.PersonInCharge = null;
            qSite.PhoneNo = null;
            qSite.BuildingUsageCode = null;
            qSite.AddressFullEN = null;
            qSite.AddressEN = null;
            qSite.AlleyEN = null;
            qSite.RoadEN = null;
            qSite.SubDistrictEN = null;
            qSite.AddressFullLC = null;
            qSite.AddressLC = null;
            qSite.AlleyLC = null;
            qSite.RoadLC = null;
            qSite.SubDistrictLC = null;
            qSite.DistrictCode = null;
            qSite.ProvinceCode = null;
            qSite.ZipCode = null;
            qSite.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qSite.CreateBy = "440065";
            qSite.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qSite.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSite = qSite;

            //dtTbt_QuotationBasic[]-----------------------------------------------------------------------------------
            tbt_QuotationBasic qBasic = new tbt_QuotationBasic();
            qBasic.QuotationTargetCode = "N0000000375";
            qBasic.Alphabet = "AA";
            qBasic.OriginateProgramId = "QUS030";
            qBasic.OriginateRefNo = null;
            qBasic.ProductCode = "050";
            qBasic.SecurityTypeCode = "TH00100010009";
            qBasic.DispatchTypeCode = "1";
            qBasic.ContractDurationMonth = 6;
            qBasic.AutoRenewMonth = 12;
            qBasic.LastValidDate = new DateTime(2012, 10, 12);
            qBasic.ContractTransferStatus = "1";
            qBasic.LockStatus = "0";
            qBasic.LastOccNo = "9990";
            qBasic.CurrentSecurityTypeCode = null;
            qBasic.PhoneLineTypeCode1 = "2";
            qBasic.PhoneLineTypeCode2 = "3";
            qBasic.PhoneLineTypeCode3 = "0";
            qBasic.PhoneLineOwnerTypeCode1 = "2";
            qBasic.PhoneLineOwnerTypeCode2 = "1";
            qBasic.PhoneLineOwnerTypeCode3 = "0";
            qBasic.FireMonitorFlag = true;
            qBasic.CrimePreventFlag = false;
            qBasic.EmergencyReportFlag = false;
            qBasic.FacilityMonitorFlag = false;
            qBasic.BeatGuardFlag = null;
            qBasic.SentryGuardFlag = null;
            qBasic.MaintenanceFlag = null;
            qBasic.SaleOnlineContractCode = "Q0000000135";
            qBasic.PlanCode = null;
            qBasic.SpecialInstallationFlag = null;
            qBasic.PlannerEmpNo = null;
            qBasic.PlanCheckerEmpNo = null;
            qBasic.PlanCheckDate = null;
            qBasic.PlanApproverEmpNo = null;
            qBasic.PlanApproveDate = null;
            qBasic.SiteBuildingArea = null;
            qBasic.SecurityAreaFrom = null;
            qBasic.SecurityAreaTo = null;
            qBasic.MainStructureTypeCode = null;
            qBasic.BuildingTypeCode = null;
            qBasic.NewBldMgmtFlag = null;
            qBasic.NewBldMgmtCost = null;
            qBasic.NumOfBuilding = 50;
            qBasic.NumOfFloor = 888;
            qBasic.FacilityPassYear = null;
            qBasic.FacilityPassMonth = null;
            qBasic.SalesmanEmpNo1 = "510729";
            qBasic.SalesmanEmpNo2 = "540902";
            qBasic.SalesmanEmpNo3 = null;
            qBasic.SalesmanEmpNo4 = null;
            qBasic.SalesmanEmpNo5 = null;
            qBasic.SalesmanEmpNo6 = null;
            qBasic.SalesmanEmpNo7 = null;
            qBasic.SalesmanEmpNo8 = null;
            qBasic.SalesmanEmpNo9 = null;
            qBasic.SalesmanEmpNo10 = null;
            qBasic.SalesSupporterEmpNo = "440065";
            qBasic.InsuranceTypeCode = "1";
            qBasic.InsuranceCoverageAmount = 7777777777.5000M;
            qBasic.MonthlyInsuranceFee = 5555555555.7500M;
            qBasic.MaintenanceFee1 = 1111111111.2500M;
            qBasic.MaintenanceFee2 = null;
            qBasic.BidGuaranteeAmount1 = null;
            qBasic.BidGuaranteeAmount2 = null;
            qBasic.AdditionalFee1 = 2222222222.5000M;
            qBasic.AdditionalFee2 = 1111122222.0000M;
            qBasic.AdditionalFee3 = 4444455555.2500M;
            qBasic.AdditionalApproveNo1 = "AR-800000000005";
            qBasic.AdditionalApproveNo2 = "AR-800000000008";
            qBasic.AdditionalApproveNo3 = "AR-800000000002";
            qBasic.ApproveNo1 = "AR-000000000009";
            qBasic.ApproveNo2 = "AR-000000000007";
            qBasic.ApproveNo3 = "AR-000000000005";
            qBasic.ApproveNo4 = "AR-000000000008";
            qBasic.ApproveNo5 = "AR-000000000002";
            qBasic.ContractFee = 8888888888.7500M;
            qBasic.ProductPrice = null;
            qBasic.InstallationFee = null;
            qBasic.DepositFee = 3333333333.5000M;
            qBasic.FacilityMemo = "ด้วยปณิธานมุ่งมั่นที่จะเป็นผู้นำในการปฏิรูปแนวคิดและรูปแบบเกี่ยวกับการรักษาความปลอดภัย โดยมีจุดมุ่งหมายสูงสุดในการสร้างสังคมที่เต็มไปด้วย “ความอุ่นใจและความปลอดภัย” บริษัทไทยซีคอมพิทักษ์กิจจำกัด(ซีคอมประเทศไทย) ได้ให้บริการเกี่ยวกับการรักษาความปลอดภัยแบบครบวงจรแก่สังคมไทยมาเป็นเวลานานกว่า 20 ปี โดยนับตั้งแต่ก้าวแรกแห่งการเริ่มต้นเมื่อปี พ.ศ. 2530 โดยการร่วมทุนระหว่างเครือสหพัฒน์ฯและSecom Japan บริษัทรักษาความปลอดภัยอันดับ 1 ของญี่ปุ่น ซึ่งสั่งสมประสบการณ์มายาวนานกว่า 45 ปี จนถึงปัจจุบันเราภูมิใจ";
            qBasic.MaintenanceMemo = null;
            qBasic.SecurityItemFee = null;
            qBasic.OtherItemFee = null;
            qBasic.SentryGuardAreaTypeCode = null;
            qBasic.SentryGuardFee = null;
            qBasic.TotalSentryGuardFee = null;
            qBasic.MaintenanceTargetProductTypeCode = null;
            qBasic.MaintenanceTypeCode = null;
            qBasic.MaintenanceCycle = 12;
            qBasic.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBasic.CreateBy = "440065";
            qBasic.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBasic.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationBasic = qBasic;

            //dtTbt_QuotationOperationType[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationOperationType = new List<tbt_QuotationOperationType>();

            tbt_QuotationOperationType qOperType1 = new tbt_QuotationOperationType();
            qOperType1.QuotationTargetCode = "N0000000375";
            qOperType1.Alphabet = "AA";
            qOperType1.OperationTypeCode = "1";
            qOperType1.CreateDate = new DateTime(2011, 8, 24, 0, 0, 0);
            qOperType1.CreateBy = "440065";
            qOperType1.UpdateDate = new DateTime(2011, 8, 24, 0, 0, 0);
            qOperType1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationOperationType.Add(qOperType1);

            tbt_QuotationOperationType qOperType2 = new tbt_QuotationOperationType();
            qOperType2.QuotationTargetCode = "N0000000375";
            qOperType2.Alphabet = "AA";
            qOperType2.OperationTypeCode = "3";
            qOperType2.CreateDate = new DateTime(2011, 8, 24, 0, 0, 0);
            qOperType2.CreateBy = "440065";
            qOperType2.UpdateDate = new DateTime(2011, 8, 24, 0, 0, 0);
            qOperType2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationOperationType.Add(qOperType2);

            tbt_QuotationOperationType qOperType3 = new tbt_QuotationOperationType();
            qOperType3.QuotationTargetCode = "N0000000375";
            qOperType3.Alphabet = "AA";
            qOperType3.OperationTypeCode = "5";
            qOperType3.CreateDate = new DateTime(2011, 8, 24, 0, 0, 0);
            qOperType3.CreateBy = "440065";
            qOperType3.UpdateDate = new DateTime(2011, 8, 24, 0, 0, 0);
            qOperType3.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationOperationType.Add(qOperType3);
            
            //dtTbt_QuotationInstrumentDetails[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationInstrumentDetails = new List<tbt_QuotationInstrumentDetails>();

            //dtTbt_QuotationFacilityDetails[]--------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationFacilityDetails = new List<tbt_QuotationFacilityDetails>();

            tbt_QuotationFacilityDetails qFacDet1 = new tbt_QuotationFacilityDetails();
            qFacDet1.QuotationTargetCode = "N0000000375";
            qFacDet1.Alphabet = "AA";
            qFacDet1.FacilityCode = "1002";
            qFacDet1.FacilityQty =20;
            qFacDet1.CreateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet1.CreateBy = "440065";
            qFacDet1.UpdateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet1);

            tbt_QuotationFacilityDetails qFacDet2 = new tbt_QuotationFacilityDetails();
            qFacDet2.QuotationTargetCode = "N0000000375";
            qFacDet2.Alphabet = "AA";
            qFacDet2.FacilityCode = "1003";
            qFacDet2.FacilityQty =4;
            qFacDet2.CreateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet2.CreateBy = "440065";
            qFacDet2.UpdateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet2);

            tbt_QuotationFacilityDetails qFacDet3 = new tbt_QuotationFacilityDetails();
            qFacDet3.QuotationTargetCode = "N0000000375";
            qFacDet3.Alphabet = "AA";
            qFacDet3.FacilityCode = "1004";
            qFacDet3.FacilityQty =2;
            qFacDet3.CreateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet3.CreateBy = "440065";
            qFacDet3.UpdateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet3.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet3);

            tbt_QuotationFacilityDetails qFacDet4 = new tbt_QuotationFacilityDetails();
            qFacDet4.QuotationTargetCode = "N0000000375";
            qFacDet4.Alphabet = "AA";
            qFacDet4.FacilityCode = "1008";
            qFacDet4.FacilityQty =30;
            qFacDet4.CreateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet4.CreateBy = "440065";
            qFacDet4.UpdateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet4.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet4);

            tbt_QuotationFacilityDetails qFacDet5 = new tbt_QuotationFacilityDetails();
            qFacDet5.QuotationTargetCode = "N0000000375";
            qFacDet5.Alphabet = "AA";
            qFacDet5.FacilityCode = "1011";
            qFacDet5.FacilityQty =8;
            qFacDet5.CreateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet5.CreateBy = "440065";
            qFacDet5.UpdateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet5.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet5);

            tbt_QuotationFacilityDetails qFacDet6 = new tbt_QuotationFacilityDetails();
            qFacDet6.QuotationTargetCode = "N0000000375";
            qFacDet6.Alphabet = "AA";
            qFacDet6.FacilityCode = "1012";
            qFacDet6.FacilityQty =150;
            qFacDet6.CreateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet6.CreateBy = "440065";
            qFacDet6.UpdateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet6.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet6);

            tbt_QuotationFacilityDetails qFacDet7 = new tbt_QuotationFacilityDetails();
            qFacDet7.QuotationTargetCode = "N0000000375";
            qFacDet7.Alphabet = "AA";
            qFacDet7.FacilityCode = "1013";
            qFacDet7.FacilityQty =3;
            qFacDet7.CreateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet7.CreateBy = "440065";
            qFacDet7.UpdateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet7.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet7);

            tbt_QuotationFacilityDetails qFacDet8 = new tbt_QuotationFacilityDetails();
            qFacDet8.QuotationTargetCode = "N0000000375";
            qFacDet8.Alphabet = "AA";
            qFacDet8.FacilityCode = "1014";
            qFacDet8.FacilityQty =950;
            qFacDet8.CreateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet8.CreateBy = "440065";
            qFacDet8.UpdateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet8.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet8);

            tbt_QuotationFacilityDetails qFacDet9 = new tbt_QuotationFacilityDetails();
            qFacDet9.QuotationTargetCode = "N0000000375";
            qFacDet9.Alphabet = "AA";
            qFacDet9.FacilityCode = "1099";
            qFacDet9.FacilityQty =1;
            qFacDet9.CreateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet9.CreateBy = "440065";
            qFacDet9.UpdateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet9.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet9);

            tbt_QuotationFacilityDetails qFacDet10 = new tbt_QuotationFacilityDetails();
            qFacDet10.QuotationTargetCode = "N0000000375";
            qFacDet10.Alphabet = "AA";
            qFacDet10.FacilityCode = "2011";
            qFacDet10.FacilityQty =80;
            qFacDet10.CreateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet10.CreateBy = "440065";
            qFacDet10.UpdateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet10.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet10);

            tbt_QuotationFacilityDetails qFacDet11 = new tbt_QuotationFacilityDetails();
            qFacDet11.QuotationTargetCode = "N0000000375";
            qFacDet11.Alphabet = "AA";
            qFacDet11.FacilityCode = "7749";
            qFacDet11.FacilityQty =50;
            qFacDet11.CreateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet11.CreateBy = "440065";
            qFacDet11.UpdateDate = new DateTime(2011, 8, 31, 20, 0, 33, 180);
            qFacDet11.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationFacilityDetails.Add(qFacDet11);

            //dtTbt_QuotationBeatGuardDetails[]-------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationBeatGuardDetails = null;

            //dtTbt_QuotationSentryGuardDetails[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationSentryGuardDetails = new List<tbt_QuotationSentryGuardDetails>();

            //dtTbt_QuotationMaintenanceLinkage[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationMaintenanceLinkage = new List<tbt_QuotationMaintenanceLinkage>();

            return dsQuotationData;
        }

        private dsQuotationData CreateExpectForCase19()
        {
            dsQuotationData dsQuotationData = new dsQuotationData();

            //dtTbt_QuotationTarget[]------------------------------------------------------------------------------------
            tbt_QuotationTarget qTarget = new tbt_QuotationTarget();
            qTarget.QuotationTargetCode = "FSG0000000269";
            qTarget.BranchNameEN = null;
            qTarget.BranchNameLC = null;
            qTarget.BranchAddressEN = null;
            qTarget.BranchAddressLC = null;
            qTarget.ProductTypeCode = "4";
            qTarget.PrefixCode = "FSG";
            qTarget.ServiceTypeCode = "2";
            qTarget.TargetCodeTypeCode = "1";
            qTarget.QuotationOfficeCode = "7000";
            qTarget.OperationOfficeCode = "7010";
            qTarget.AcquisitionTypeCode = null;
            qTarget.IntroducerCode = null;
            qTarget.MotivationTypeCode = null;
            qTarget.OldContractCode = null;
            qTarget.QuotationStaffEmpNo = null;
            qTarget.LastAlphabet = "AA";
            qTarget.ContractTransferStatus = "1";
            qTarget.ContractCode = null;
            qTarget.TransferDate = null;
            qTarget.TransferAlphabet = null;
            qTarget.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qTarget.CreateBy = "440065";
            qTarget.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qTarget.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationTarget = qTarget;

            //dtTbt_QuotationCustomer[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationCustomer = new List<tbt_QuotationCustomer>();

            tbt_QuotationCustomer qCustomer = new tbt_QuotationCustomer();
            qCustomer.QuotationTargetCode = "FSG0000000269";
            qCustomer.CustPartTypeCode = "1";
            qCustomer.CustCode = null;
            qCustomer.CustNameEN = "EASY BUY";
            qCustomer.CustNameLC = "อีซี่ บาย";
            qCustomer.CustFullNameEN = "EASY BUY PUBLIC COMPANY LIMITED";
            qCustomer.CustFullNameLC = "บริษัท อีซี่ บาย จำกัด (มหาชน)";
            qCustomer.RepPersonName = null;
            qCustomer.ContactPersonName = null;
            qCustomer.SECOMContactPerson = null;
            qCustomer.CustTypeCode = "0";
            qCustomer.CompanyTypeCode = "04";
            qCustomer.FinancialMarketTypeCode = null;
            qCustomer.BusinessTypeCode = "025";
            qCustomer.PhoneNo = null;
            qCustomer.IDNo = null;
            qCustomer.RegionCode = "TH";
            qCustomer.URL = null;
            qCustomer.AddressFullEN = "952 RAMALAND BLDG., 13TH FL., RAMA IV, SURIYAWONG, BANGRAK, BANGKOK 10500";
            qCustomer.AddressEN = "952 RAMALAND BLDG., 13TH FL.";
            qCustomer.AlleyEN = null;
            qCustomer.RoadEN = "RAMA IV";
            qCustomer.SubDistrictEN = "SURIYAWONG";
            qCustomer.AddressFullLC = "952 อาคารรามาแลนด์ ชั้น 13 ถ.พระราม 4 แขวงสุริยวงศ์ เขตบางรัก จ.กรุงเทพมหานคร 10500";
            qCustomer.AddressLC = "952 อาคารรามาแลนด์ ชั้น 13 ";
            qCustomer.AlleyLC = null;
            qCustomer.RoadLC = "พระราม 4";
            qCustomer.SubDistrictLC = "สุริยวงศ์";
            qCustomer.DistrictCode = "00010";
            qCustomer.ProvinceCode = "001";
            qCustomer.ZipCode = "10500";
            qCustomer.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer.CreateBy = "440065";
            qCustomer.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationCustomer.Add(qCustomer);

            //dtTbt_QuotationSite[]-----------------------------------------------------------------------------------
            tbt_QuotationSite qSite = new tbt_QuotationSite();
            qSite.QuotationTargetCode = "FSG0000000269";
            qSite.SiteCode = null;
            qSite.SiteNo = null;
            qSite.SiteNameEN = "EASY BUY PUBLIC COMPANY LIMITED (MBK CENTER)";
            qSite.SiteNameLC = "บริษัท อีซี่ บาย จำกัด (มหาชน) - สาขามาบุญครองเซ็นเตอร์";
            qSite.SECOMContactPerson = null;
            qSite.PersonInCharge = null;
            qSite.PhoneNo = null;
            qSite.BuildingUsageCode = "017";
            qSite.AddressFullEN = "444 MBK CENTER, 4TH FL., ROOM 1, PHAYATHAI RD., WANGMAI, PATUMWAN, BANGKOK 10330";
            qSite.AddressEN = "444 MBK CENTER, 4TH FL., ROOM 1";
            qSite.AlleyEN = null;
            qSite.RoadEN = "PHAYATHAI";
            qSite.SubDistrictEN = "WANGMAI";
            qSite.AddressFullLC = "444 อาคาร เอ็ม บี เค เซ็นเตอร์ ชั้น 4 ถ.พญาไท แขวงวังใหม่ เขตปทุมวัน จ.กรุงเทพมหานคร 10330 ";
            qSite.AddressLC = "444 อาคาร เอ็ม บี เค เซ็นเตอร์ ชั้น 4";
            qSite.AlleyLC = null;
            qSite.RoadLC = "พญาไท";
            qSite.SubDistrictLC = "วังใหม่";
            qSite.DistrictCode = "00031";
            qSite.ProvinceCode = "001";
            qSite.ZipCode = "10330";
            qSite.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qSite.CreateBy = "440065";
            qSite.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qSite.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSite = qSite;

            //dtTbt_QuotationBasic[]-----------------------------------------------------------------------------------
            tbt_QuotationBasic qBasic = new tbt_QuotationBasic();
            qBasic.QuotationTargetCode = "FSG0000000269";
            qBasic.Alphabet = "AA";
            qBasic.OriginateProgramId = "QUS030";
            qBasic.OriginateRefNo = null;
            qBasic.ProductCode = "036";
            qBasic.SecurityTypeCode = null;
            qBasic.DispatchTypeCode = null;
            qBasic.ContractDurationMonth = 6;
            qBasic.AutoRenewMonth = 12;
            qBasic.LastValidDate = new DateTime(2012, 10, 12);
            qBasic.ContractTransferStatus = "1";
            qBasic.LockStatus = "0";
            qBasic.LastOccNo = null;
            qBasic.CurrentSecurityTypeCode = null;
            qBasic.PhoneLineTypeCode1 = null;
            qBasic.PhoneLineTypeCode2 = null;
            qBasic.PhoneLineTypeCode3 = null;
            qBasic.PhoneLineOwnerTypeCode1 = null;
            qBasic.PhoneLineOwnerTypeCode2 = null;
            qBasic.PhoneLineOwnerTypeCode3 = null;
            qBasic.FireMonitorFlag = null;
            qBasic.CrimePreventFlag = null;
            qBasic.EmergencyReportFlag = null;
            qBasic.FacilityMonitorFlag = null;
            qBasic.BeatGuardFlag = null;
            qBasic.SentryGuardFlag = null;
            qBasic.MaintenanceFlag = null;
            qBasic.SaleOnlineContractCode = null;
            qBasic.PlanCode = null;
            qBasic.SpecialInstallationFlag = null;
            qBasic.PlannerEmpNo = null;
            qBasic.PlanCheckerEmpNo = null;
            qBasic.PlanCheckDate = null;
            qBasic.PlanApproverEmpNo = null;
            qBasic.PlanApproveDate = null;
            qBasic.SiteBuildingArea = null;
            qBasic.SecurityAreaFrom = null;
            qBasic.SecurityAreaTo = null;
            qBasic.MainStructureTypeCode = null;
            qBasic.BuildingTypeCode = null;
            qBasic.NewBldMgmtFlag = null;
            qBasic.NewBldMgmtCost = null;
            qBasic.NumOfBuilding = null;
            qBasic.NumOfFloor = null;
            qBasic.FacilityPassYear = null;
            qBasic.FacilityPassMonth = null;
            qBasic.SalesmanEmpNo1 = "540902";
            qBasic.SalesmanEmpNo2 = null;
            qBasic.SalesmanEmpNo3 = null;
            qBasic.SalesmanEmpNo4 = null;
            qBasic.SalesmanEmpNo5 = null;
            qBasic.SalesmanEmpNo6 = null;
            qBasic.SalesmanEmpNo7 = null;
            qBasic.SalesmanEmpNo8 = null;
            qBasic.SalesmanEmpNo9 = null;
            qBasic.SalesmanEmpNo10 = null;
            qBasic.SalesSupporterEmpNo = null;
            qBasic.InsuranceTypeCode = null;
            qBasic.InsuranceCoverageAmount = null;
            qBasic.MonthlyInsuranceFee = null;
            qBasic.MaintenanceFee1 = null;
            qBasic.MaintenanceFee2 = null;
            qBasic.BidGuaranteeAmount1 = null;
            qBasic.BidGuaranteeAmount2 = null;
            qBasic.AdditionalFee1 = null;
            qBasic.AdditionalFee2 = null;
            qBasic.AdditionalFee3 = null;
            qBasic.AdditionalApproveNo1 = null;
            qBasic.AdditionalApproveNo2 = null;
            qBasic.AdditionalApproveNo3 = null;
            qBasic.ApproveNo1 = null;
            qBasic.ApproveNo2 = null;
            qBasic.ApproveNo3 = null;
            qBasic.ApproveNo4 = null;
            qBasic.ApproveNo5 = null;
            qBasic.ContractFee = 100250.0000M;
            qBasic.ProductPrice = null;
            qBasic.InstallationFee = null;
            qBasic.DepositFee = 25000.7500M;
            qBasic.FacilityMemo = null;
            qBasic.MaintenanceMemo = null;
            qBasic.SecurityItemFee = null;
            qBasic.OtherItemFee = null;
            qBasic.SentryGuardAreaTypeCode = null;
            qBasic.SentryGuardFee = null;
            qBasic.TotalSentryGuardFee = null;
            qBasic.MaintenanceTargetProductTypeCode = null;
            qBasic.MaintenanceTypeCode = null;
            qBasic.MaintenanceCycle = null;
            qBasic.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBasic.CreateBy = "440065";
            qBasic.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBasic.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationBasic = qBasic;

            //dtTbt_QuotationOperationType[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationOperationType = new List<tbt_QuotationOperationType>();

            //dtTbt_QuotationInstrumentDetails[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationInstrumentDetails = new List<tbt_QuotationInstrumentDetails>();

            //dtTbt_QuotationFacilityDetails[]--------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationFacilityDetails = new List<tbt_QuotationFacilityDetails>();

            //dtTbt_QuotationBeatGuardDetails[]-------------------------------------------------------------------------------------
            tbt_QuotationBeatGuardDetails qBeatGrdDet = new tbt_QuotationBeatGuardDetails();
            qBeatGrdDet.QuotationTargetCode = "FSG0000000269";
            qBeatGrdDet.Alphabet = "AA";
            qBeatGrdDet.NumOfDayTimeWd = 20;
            qBeatGrdDet.NumOfNightTimeWd = 30;
            qBeatGrdDet.NumOfDayTimeSat = 40;
            qBeatGrdDet.NumOfNightTimeSat = 45;
            qBeatGrdDet.NumOfDayTimeSun = 60;
            qBeatGrdDet.NumOfNightTimeSun = 75;
            qBeatGrdDet.NumOfBeatStep = 999999;
            qBeatGrdDet.FreqOfGateUsage = null;
            qBeatGrdDet.NumOfClockKey = null;
            qBeatGrdDet.NumOfDate = 30.4M;
            qBeatGrdDet.NotifyTime = null;
            qBeatGrdDet.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBeatGrdDet.CreateBy = "440065";
            qBeatGrdDet.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBeatGrdDet.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationBeatGuardDetails = qBeatGrdDet;
            
            //dtTbt_QuotationSentryGuardDetails[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationSentryGuardDetails = new List<tbt_QuotationSentryGuardDetails>();

            //dtTbt_QuotationMaintenanceLinkage[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationMaintenanceLinkage = new List<tbt_QuotationMaintenanceLinkage>();

            return dsQuotationData;
        }

        private dsQuotationData CreateExpectForCase20()
        {
            dsQuotationData dsQuotationData = new dsQuotationData();

            //dtTbt_QuotationTarget[]------------------------------------------------------------------------------------
            tbt_QuotationTarget qTarget = new tbt_QuotationTarget();
            qTarget.QuotationTargetCode = "SG0000000285";
            qTarget.BranchNameEN = "CENTRAL FOOD RETAIL COMPANY LIMITED  (CENTRAL WORLD)";
            qTarget.BranchNameLC = "บริษัท เซ็นทรัล ฟู้ด รีเทล จำกัด (สาขาเซ็นทรัลเวิลด์)";
            qTarget.BranchAddressEN = "FLOOR 7, CENTRAL WORLD PLAZA, RATCHADAMRI, PATUMWAN, BANGKOK 10330";
            qTarget.BranchAddressLC = "ชั้น 7 เซ็นทรัลเวิลด์พลาซ่า ถ.ราชดำริ แขวงปทุมวัน เขตปทุมวัน จ.กรุงเทพมหานคร 10330";
            qTarget.ProductTypeCode = "5";
            qTarget.PrefixCode = "SG";
            qTarget.ServiceTypeCode = "2";
            qTarget.TargetCodeTypeCode = "2";
            qTarget.QuotationOfficeCode = "2000";
            qTarget.OperationOfficeCode = "2060";
            qTarget.AcquisitionTypeCode = "2";
            qTarget.IntroducerCode = "FUJIKO";
            qTarget.MotivationTypeCode = "03";
            qTarget.OldContractCode = "SG0000000254";
            qTarget.QuotationStaffEmpNo = "470230";
            qTarget.LastAlphabet = "AD";
            qTarget.ContractTransferStatus = "3";
            qTarget.ContractCode = "SG0000000285";
            qTarget.TransferDate = new DateTime(2011, 10, 9);
            qTarget.TransferAlphabet = "AB";
            qTarget.CreateDate = new DateTime(2011, 9, 15, 12, 9, 0);
            qTarget.CreateBy = "440065";
            qTarget.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qTarget.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationTarget = qTarget;

            //dtTbt_QuotationCustomer[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationCustomer = new List<tbt_QuotationCustomer>();
            
            tbt_QuotationCustomer qCustomer1 = new tbt_QuotationCustomer();
            qCustomer1.QuotationTargetCode = "SG0000000285";
            qCustomer1.CustPartTypeCode = "1";
            qCustomer1.CustCode = "C0000000184";
            qCustomer1.CustNameEN = null;
            qCustomer1.CustNameLC = null;
            qCustomer1.CustFullNameEN = null;
            qCustomer1.CustFullNameLC = null;
            qCustomer1.RepPersonName = null;
            qCustomer1.ContactPersonName = null;
            qCustomer1.SECOMContactPerson = null;
            qCustomer1.CustTypeCode = null;
            qCustomer1.CompanyTypeCode = null;
            qCustomer1.FinancialMarketTypeCode = null;
            qCustomer1.BusinessTypeCode = null;
            qCustomer1.PhoneNo = null;
            qCustomer1.IDNo = null;
            qCustomer1.RegionCode = null;
            qCustomer1.URL = null;
            qCustomer1.AddressFullEN = null;
            qCustomer1.AddressEN = null;
            qCustomer1.AlleyEN = null;
            qCustomer1.RoadEN = null;
            qCustomer1.SubDistrictEN = null;
            qCustomer1.AddressFullLC = null;
            qCustomer1.AddressLC = null;
            qCustomer1.AlleyLC = null;
            qCustomer1.RoadLC = null;
            qCustomer1.SubDistrictLC = null;
            qCustomer1.DistrictCode = null;
            qCustomer1.ProvinceCode = null;
            qCustomer1.ZipCode = null;
            qCustomer1.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer1.CreateBy = "440065";
            qCustomer1.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationCustomer.Add(qCustomer1);

            tbt_QuotationCustomer qCustomer2 = new tbt_QuotationCustomer();
            qCustomer2.QuotationTargetCode = "SG0000000285";
            qCustomer2.CustPartTypeCode = "2";
            qCustomer2.CustCode = "C0000000108";
            qCustomer2.CustNameEN = null;
            qCustomer2.CustNameLC = null;
            qCustomer2.CustFullNameEN = null;
            qCustomer2.CustFullNameLC = null;
            qCustomer2.RepPersonName = null;
            qCustomer2.ContactPersonName = null;
            qCustomer2.SECOMContactPerson = null;
            qCustomer2.CustTypeCode = null;
            qCustomer2.CompanyTypeCode = null;
            qCustomer2.FinancialMarketTypeCode = null;
            qCustomer2.BusinessTypeCode = null;
            qCustomer2.PhoneNo = null;
            qCustomer2.IDNo = null;
            qCustomer2.RegionCode = null;
            qCustomer2.URL = null;
            qCustomer2.AddressFullEN = null;
            qCustomer2.AddressEN = null;
            qCustomer2.AlleyEN = null;
            qCustomer2.RoadEN = null;
            qCustomer2.SubDistrictEN = null;
            qCustomer2.AddressFullLC = null;
            qCustomer2.AddressLC = null;
            qCustomer2.AlleyLC = null;
            qCustomer2.RoadLC = null;
            qCustomer2.SubDistrictLC = null;
            qCustomer2.DistrictCode = null;
            qCustomer2.ProvinceCode = null;
            qCustomer2.ZipCode = null;
            qCustomer2.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer2.CreateBy = "440065";
            qCustomer2.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationCustomer.Add(qCustomer2);
            
            //dtTbt_QuotationSite[]-----------------------------------------------------------------------------------
            tbt_QuotationSite qSite = new tbt_QuotationSite();
            qSite.QuotationTargetCode = "SG0000000285";
            qSite.SiteCode = "S0000000108-0002";
            qSite.SiteNo = "0002";
            qSite.SiteNameEN = null;
            qSite.SiteNameLC = null;
            qSite.SECOMContactPerson = null;
            qSite.PersonInCharge = null;
            qSite.PhoneNo = null;
            qSite.BuildingUsageCode = null;
            qSite.AddressFullEN = null;
            qSite.AddressEN = null;
            qSite.AlleyEN = null;
            qSite.RoadEN = null;
            qSite.SubDistrictEN = null;
            qSite.AddressFullLC = null;
            qSite.AddressLC = null;
            qSite.AlleyLC = null;
            qSite.RoadLC = null;
            qSite.SubDistrictLC = null;
            qSite.DistrictCode = null;
            qSite.ProvinceCode = null;
            qSite.ZipCode = null;
            qSite.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qSite.CreateBy = "440065";
            qSite.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qSite.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSite = qSite;

            //dtTbt_QuotationBasic[]-----------------------------------------------------------------------------------
            tbt_QuotationBasic qBasic = new tbt_QuotationBasic();
            qBasic.QuotationTargetCode = "SG0000000285";
            qBasic.Alphabet = "AC";
            qBasic.OriginateProgramId = "QUS030";
            qBasic.OriginateRefNo = null;
            qBasic.ProductCode = "004";
            qBasic.SecurityTypeCode = null;
            qBasic.DispatchTypeCode = null;
            qBasic.ContractDurationMonth = 6;
            qBasic.AutoRenewMonth = 12;
            qBasic.LastValidDate = new DateTime(2012, 10, 12);
            qBasic.ContractTransferStatus = "1";
            qBasic.LockStatus = "0";
            qBasic.LastOccNo = "9980";
            qBasic.CurrentSecurityTypeCode = null;
            qBasic.PhoneLineTypeCode1 = null;
            qBasic.PhoneLineTypeCode2 = null;
            qBasic.PhoneLineTypeCode3 = null;
            qBasic.PhoneLineOwnerTypeCode1 = null;
            qBasic.PhoneLineOwnerTypeCode2 = null;
            qBasic.PhoneLineOwnerTypeCode3 = null;
            qBasic.FireMonitorFlag = null;
            qBasic.CrimePreventFlag = null;
            qBasic.EmergencyReportFlag = null;
            qBasic.FacilityMonitorFlag = null;
            qBasic.BeatGuardFlag = null;
            qBasic.SentryGuardFlag = null;
            qBasic.MaintenanceFlag = null;
            qBasic.SaleOnlineContractCode = null;
            qBasic.PlanCode = null;
            qBasic.SpecialInstallationFlag = null;
            qBasic.PlannerEmpNo = null;
            qBasic.PlanCheckerEmpNo = null;
            qBasic.PlanCheckDate = null;
            qBasic.PlanApproverEmpNo = null;
            qBasic.PlanApproveDate = null;
            qBasic.SiteBuildingArea = null;
            qBasic.SecurityAreaFrom = null;
            qBasic.SecurityAreaTo = null;
            qBasic.MainStructureTypeCode = null;
            qBasic.BuildingTypeCode = null;
            qBasic.NewBldMgmtFlag = null;
            qBasic.NewBldMgmtCost = null;
            qBasic.NumOfBuilding = null;
            qBasic.NumOfFloor = null;
            qBasic.FacilityPassYear = null;
            qBasic.FacilityPassMonth = null;
            qBasic.SalesmanEmpNo1 = null;
            qBasic.SalesmanEmpNo2 = null;
            qBasic.SalesmanEmpNo3 = null;
            qBasic.SalesmanEmpNo4 = null;
            qBasic.SalesmanEmpNo5 = null;
            qBasic.SalesmanEmpNo6 = null;
            qBasic.SalesmanEmpNo7 = null;
            qBasic.SalesmanEmpNo8 = null;
            qBasic.SalesmanEmpNo9 = null;
            qBasic.SalesmanEmpNo10 = null;
            qBasic.SalesSupporterEmpNo = null;
            qBasic.InsuranceTypeCode = null;
            qBasic.InsuranceCoverageAmount = null;
            qBasic.MonthlyInsuranceFee = null;
            qBasic.MaintenanceFee1 = null;
            qBasic.MaintenanceFee2 = null;
            qBasic.BidGuaranteeAmount1 = null;
            qBasic.BidGuaranteeAmount2 = null;
            qBasic.AdditionalFee1 = null;
            qBasic.AdditionalFee2 = null;
            qBasic.AdditionalFee3 = null;
            qBasic.AdditionalApproveNo1 = null;
            qBasic.AdditionalApproveNo2 = null;
            qBasic.AdditionalApproveNo3 = null;
            qBasic.ApproveNo1 = null;
            qBasic.ApproveNo2 = null;
            qBasic.ApproveNo3 = null;
            qBasic.ApproveNo4 = null;
            qBasic.ApproveNo5 = null;
            qBasic.ContractFee = null;
            qBasic.ProductPrice = null;
            qBasic.InstallationFee = null;
            qBasic.DepositFee = null;
            qBasic.FacilityMemo = null;
            qBasic.MaintenanceMemo = null;
            qBasic.SecurityItemFee = null;
            qBasic.OtherItemFee = null;
            qBasic.SentryGuardAreaTypeCode = null;
            qBasic.SentryGuardFee = null;
            qBasic.TotalSentryGuardFee = null;
            qBasic.MaintenanceTargetProductTypeCode = null;
            qBasic.MaintenanceTypeCode = null;
            qBasic.MaintenanceCycle = null;
            qBasic.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBasic.CreateBy = "440065";
            qBasic.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBasic.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationBasic = qBasic;

            //dtTbt_QuotationOperationType[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationOperationType = new List<tbt_QuotationOperationType>();

            //dtTbt_QuotationInstrumentDetails[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationInstrumentDetails = new List<tbt_QuotationInstrumentDetails>();

            //dtTbt_QuotationFacilityDetails[]--------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationFacilityDetails = new List<tbt_QuotationFacilityDetails>();

            //dtTbt_QuotationBeatGuardDetails[]-------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationBeatGuardDetails = null;

            //dtTbt_QuotationSentryGuardDetails[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationSentryGuardDetails = new List<tbt_QuotationSentryGuardDetails>();

            tbt_QuotationSentryGuardDetails qSenGrdDet1 = new tbt_QuotationSentryGuardDetails();
            qSenGrdDet1.QuotationTargetCode = "SG0000000285";
            qSenGrdDet1.Alphabet = "AC";
            qSenGrdDet1.RunningNo = 1;
            qSenGrdDet1.SentryGuardTypeCode = "1";
            qSenGrdDet1.NumOfDate = 30.4M;
            qSenGrdDet1.SecurityStartTime = new TimeSpan(0, 0, 0);
            qSenGrdDet1.SecurityFinishTime = new TimeSpan(6, 0, 0);
            qSenGrdDet1.WorkHourPerMonth = 182.4M;
            qSenGrdDet1.CostPerHour = 3000.0000M;
            qSenGrdDet1.NumOfSentryGuard = 99;
            qSenGrdDet1.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet1.CreateBy = "440065";
            qSenGrdDet1.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSentryGuardDetails.Add(qSenGrdDet1);

            tbt_QuotationSentryGuardDetails qSenGrdDet2 = new tbt_QuotationSentryGuardDetails();
            qSenGrdDet2.QuotationTargetCode = "SG0000000285";
            qSenGrdDet2.Alphabet = "AC";
            qSenGrdDet2.RunningNo = 2;
            qSenGrdDet2.SentryGuardTypeCode = "1";
            qSenGrdDet2.NumOfDate = 30.4M;
            qSenGrdDet2.SecurityStartTime = new TimeSpan(18, 0, 0);
            qSenGrdDet2.SecurityFinishTime = new TimeSpan(23, 59, 0);
            qSenGrdDet2.WorkHourPerMonth = 181.9M;
            qSenGrdDet2.CostPerHour = 3000.0000M;
            qSenGrdDet2.NumOfSentryGuard = 20;
            qSenGrdDet2.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet2.CreateBy = "440065";
            qSenGrdDet2.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSentryGuardDetails.Add(qSenGrdDet2);

            tbt_QuotationSentryGuardDetails qSenGrdDet3 = new tbt_QuotationSentryGuardDetails();
            qSenGrdDet3.QuotationTargetCode = "SG0000000285";
            qSenGrdDet3.Alphabet = "AC";
            qSenGrdDet3.RunningNo = 3;
            qSenGrdDet3.SentryGuardTypeCode = "1";
            qSenGrdDet3.NumOfDate = 30.4M;
            qSenGrdDet3.SecurityStartTime = new TimeSpan(6, 0, 0);
            qSenGrdDet3.SecurityFinishTime = new TimeSpan(12, 0, 0);
            qSenGrdDet3.WorkHourPerMonth = 182.4M;
            qSenGrdDet3.CostPerHour = 3000.0000M;
            qSenGrdDet3.NumOfSentryGuard = 85;
            qSenGrdDet3.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet3.CreateBy = "440065";
            qSenGrdDet3.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet3.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSentryGuardDetails.Add(qSenGrdDet3);

            tbt_QuotationSentryGuardDetails qSenGrdDet4 = new tbt_QuotationSentryGuardDetails();
            qSenGrdDet4.QuotationTargetCode = "SG0000000285";
            qSenGrdDet4.Alphabet = "AC";
            qSenGrdDet4.RunningNo = 4;
            qSenGrdDet4.SentryGuardTypeCode = "1";
            qSenGrdDet4.NumOfDate = 30.4M;
            qSenGrdDet4.SecurityStartTime = new TimeSpan(12, 0, 0);
            qSenGrdDet4.SecurityFinishTime = new TimeSpan(18, 0, 0);
            qSenGrdDet4.WorkHourPerMonth = 182.4M;
            qSenGrdDet4.CostPerHour = 3000.0000M;
            qSenGrdDet4.NumOfSentryGuard = 70;
            qSenGrdDet4.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet4.CreateBy = "440065";
            qSenGrdDet4.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet4.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSentryGuardDetails.Add(qSenGrdDet4);

            tbt_QuotationSentryGuardDetails qSenGrdDet5 = new tbt_QuotationSentryGuardDetails();
            qSenGrdDet5.QuotationTargetCode = "SG0000000285";
            qSenGrdDet5.Alphabet = "AC";
            qSenGrdDet5.RunningNo = 5;
            qSenGrdDet5.SentryGuardTypeCode = "1";
            qSenGrdDet5.NumOfDate = 9.9M;
            qSenGrdDet5.SecurityStartTime = new TimeSpan(0, 0, 0);
            qSenGrdDet5.SecurityFinishTime = new TimeSpan(23, 59, 0);
            qSenGrdDet5.WorkHourPerMonth = 237.4M;
            qSenGrdDet5.CostPerHour = 0.0000M;
            qSenGrdDet5.NumOfSentryGuard = 55;
            qSenGrdDet5.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet5.CreateBy = "440065";
            qSenGrdDet5.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet5.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSentryGuardDetails.Add(qSenGrdDet5);

            tbt_QuotationSentryGuardDetails qSenGrdDet6 = new tbt_QuotationSentryGuardDetails();
            qSenGrdDet6.QuotationTargetCode = "SG0000000285";
            qSenGrdDet6.Alphabet = "AC";
            qSenGrdDet6.RunningNo = 6;
            qSenGrdDet6.SentryGuardTypeCode = "1";
            qSenGrdDet6.NumOfDate = 20.5M;
            qSenGrdDet6.SecurityStartTime = new TimeSpan(9, 0, 0);
            qSenGrdDet6.SecurityFinishTime = new TimeSpan(15, 45, 0);
            qSenGrdDet6.WorkHourPerMonth = 138.4M;
            qSenGrdDet6.CostPerHour = 3000.0000M;
            qSenGrdDet6.NumOfSentryGuard = 30;
            qSenGrdDet6.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet6.CreateBy = "440065";
            qSenGrdDet6.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);;
            qSenGrdDet6.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSentryGuardDetails.Add(qSenGrdDet6);
            
            //dtTbt_QuotationMaintenanceLinkage[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationMaintenanceLinkage = new List<tbt_QuotationMaintenanceLinkage>();

            return dsQuotationData;
        }

        private dsQuotationData CreateExpectForCase21()
        {
            dsQuotationData dsQuotationData = new dsQuotationData();

            //dtTbt_QuotationTarget[]------------------------------------------------------------------------------------
            tbt_QuotationTarget qTarget = new tbt_QuotationTarget();
            qTarget.QuotationTargetCode = "FMA0000000083";
            qTarget.BranchNameEN = null;
            qTarget.BranchNameLC = null;
            qTarget.BranchAddressEN = null;
            qTarget.BranchAddressLC = null;
            qTarget.ProductTypeCode = "6";
            qTarget.PrefixCode = "FMA";
            qTarget.ServiceTypeCode = "2";
            qTarget.TargetCodeTypeCode = "1";
            qTarget.QuotationOfficeCode = "0001";
            qTarget.OperationOfficeCode = "1010";
            qTarget.AcquisitionTypeCode = null;
            qTarget.IntroducerCode = null;
            qTarget.MotivationTypeCode = null;
            qTarget.OldContractCode = null;
            qTarget.QuotationStaffEmpNo = "420022";
            qTarget.LastAlphabet = "ZZ";
            qTarget.ContractTransferStatus = "3";
            qTarget.ContractCode = "MA0000000085";
            qTarget.TransferDate = new DateTime(2011, 7, 17);
            qTarget.TransferAlphabet = "ZZ";
            qTarget.CreateDate = new DateTime(2011, 7, 11, 12, 9, 0);
            qTarget.CreateBy = "440065";
            qTarget.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qTarget.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationTarget = qTarget;

            //dtTbt_QuotationCustomer[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationCustomer = new List<tbt_QuotationCustomer>();

            tbt_QuotationCustomer qCustomer = new tbt_QuotationCustomer();
            qCustomer.QuotationTargetCode = "FMA0000000083";
            qCustomer.CustPartTypeCode = "1";
            qCustomer.CustCode = null;
            qCustomer.CustNameEN = "BANGOKOK UFJ";
            qCustomer.CustNameLC = "บางกอก ยูเอฟเจ";
            qCustomer.CustFullNameEN = "BANGKOK UFJ CO., LTD";
            qCustomer.CustFullNameLC = "บริษัท บางกอก ยูเอฟเจ จำกัด";
            qCustomer.RepPersonName = null;
            qCustomer.ContactPersonName = null;
            qCustomer.SECOMContactPerson = null;
            qCustomer.CustTypeCode = "0";
            qCustomer.CompanyTypeCode = "03";
            qCustomer.FinancialMarketTypeCode = null;
            qCustomer.BusinessTypeCode = "006";
            qCustomer.PhoneNo = null;
            qCustomer.IDNo = null;
            qCustomer.RegionCode = null;
            qCustomer.URL = null;
            qCustomer.AddressFullEN = "28th FLOOR, BANGKOK CITY TOWER,SOUTH SATHORN,THUNGMAHAMEK,SATHORN,BANGKOK 10190";
            qCustomer.AddressEN = "28th FLOOR, BANGKOK CITY TOWER";
            qCustomer.AlleyEN = null;
            qCustomer.RoadEN = "SOUTH SATHORN";
            qCustomer.SubDistrictEN = "THUNGMAHAMEK";
            qCustomer.AddressFullLC = "ชั้น 28 อาคาร บางกอกซิตี้ ถ.สาทรใต้ แขวงทุ่งมหาเมฆ เขตสาทร จ.กรุงเทพมหานคร 10190";
            qCustomer.AddressLC = "ชั้น 28 อาคาร บางกอกซิตี้";
            qCustomer.AlleyLC = null;
            qCustomer.RoadLC = "สาทรใต้";
            qCustomer.SubDistrictLC = "ทุ่งมหาเมฆ";
            qCustomer.DistrictCode = "00042";
            qCustomer.ProvinceCode = "001";
            qCustomer.ZipCode = "10190";
            qCustomer.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer.CreateBy = "440065";
            qCustomer.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qCustomer.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationCustomer.Add(qCustomer);

            //dtTbt_QuotationSite[]-----------------------------------------------------------------------------------
            tbt_QuotationSite qSite = new tbt_QuotationSite();
            qSite.QuotationTargetCode = "FMA0000000083";
            qSite.SiteCode = null;
            qSite.SiteNo = null;
            qSite.SiteNameEN = "BRINK'S (THAILAND) CO.,LTD ";
            qSite.SiteNameLC = "บริษัท บริงค์ส (ประเทศไทย) จำกัด";
            qSite.SECOMContactPerson = null;
            qSite.PersonInCharge = null;
            qSite.PhoneNo = null;
            qSite.BuildingUsageCode = "003";
            qSite.AddressFullEN = "919/586-591 JEWELRY TRADE CENTER BLDG., 47th FL., SILOM RD., SILOM, BANGRAK, BANGKOK 10500";
            qSite.AddressEN = "919/586-591 JEWELRY TRADE CENTER BLDG., 47th FL.";
            qSite.AlleyEN = null;
            qSite.RoadEN = "SILOM";
            qSite.SubDistrictEN = "SILOM";
            qSite.AddressFullLC = "919/586-591 ชั้น 47 อาคารจิวเวลรี่เทรดเซ็นเตอร์ ถ.สีลม แขวงสีลม เขตบางรัก จ.กรุงเทพมหานคร 10500";
            qSite.AddressLC = "919/586-591 ชั้น 47 อาคารจิวเวลรี่เทรดเซ็นเตอร์";
            qSite.AlleyLC = null;
            qSite.RoadLC = "สีลม";
            qSite.SubDistrictLC = "สีลม";
            qSite.DistrictCode = "00010";
            qSite.ProvinceCode = "001";
            qSite.ZipCode = "10500";
            qSite.CreateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qSite.CreateBy = "440065";
            qSite.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qSite.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationSite = qSite;

            //dtTbt_QuotationBasic[]-----------------------------------------------------------------------------------
            tbt_QuotationBasic qBasic = new tbt_QuotationBasic();
            qBasic.QuotationTargetCode = "FMA0000000083";
            qBasic.Alphabet = "ZZ";
            qBasic.OriginateProgramId = "QUS030";
            qBasic.OriginateRefNo = null;
            qBasic.ProductCode = "046";
            qBasic.SecurityTypeCode = null;
            qBasic.DispatchTypeCode = null;
            qBasic.ContractDurationMonth = 6;
            qBasic.AutoRenewMonth = 12;
            qBasic.LastValidDate = new DateTime(2012, 8, 12);
            qBasic.ContractTransferStatus = "3";
            qBasic.LockStatus = "0";
            qBasic.LastOccNo = null;
            qBasic.CurrentSecurityTypeCode = null;
            qBasic.PhoneLineTypeCode1 = null;
            qBasic.PhoneLineTypeCode2 = null;
            qBasic.PhoneLineTypeCode3 = null;
            qBasic.PhoneLineOwnerTypeCode1 = null;
            qBasic.PhoneLineOwnerTypeCode2 = null;
            qBasic.PhoneLineOwnerTypeCode3 = null;
            qBasic.FireMonitorFlag = null;
            qBasic.CrimePreventFlag = null;
            qBasic.EmergencyReportFlag = null;
            qBasic.FacilityMonitorFlag = null;
            qBasic.BeatGuardFlag = null;
            qBasic.SentryGuardFlag = null;
            qBasic.MaintenanceFlag = null;
            qBasic.SaleOnlineContractCode = null;
            qBasic.PlanCode = null;
            qBasic.SpecialInstallationFlag = null;
            qBasic.PlannerEmpNo = null;
            qBasic.PlanCheckerEmpNo = null;
            qBasic.PlanCheckDate = null;
            qBasic.PlanApproverEmpNo = null;
            qBasic.PlanApproveDate = null;
            qBasic.SiteBuildingArea = null;
            qBasic.SecurityAreaFrom = null;
            qBasic.SecurityAreaTo = null;
            qBasic.MainStructureTypeCode = null;
            qBasic.BuildingTypeCode = null;
            qBasic.NewBldMgmtFlag = null;
            qBasic.NewBldMgmtCost = null;
            qBasic.NumOfBuilding = null;
            qBasic.NumOfFloor = null;
            qBasic.FacilityPassYear = null;
            qBasic.FacilityPassMonth = null;
            qBasic.SalesmanEmpNo1 = "480396";
            qBasic.SalesmanEmpNo2 = null;
            qBasic.SalesmanEmpNo3 = null;
            qBasic.SalesmanEmpNo4 = null;
            qBasic.SalesmanEmpNo5 = null;
            qBasic.SalesmanEmpNo6 = null;
            qBasic.SalesmanEmpNo7 = null;
            qBasic.SalesmanEmpNo8 = null;
            qBasic.SalesmanEmpNo9 = null;
            qBasic.SalesmanEmpNo10 = null;
            qBasic.SalesSupporterEmpNo = null;
            qBasic.InsuranceTypeCode = null;
            qBasic.InsuranceCoverageAmount = null;
            qBasic.MonthlyInsuranceFee = null;
            qBasic.MaintenanceFee1 = null;
            qBasic.MaintenanceFee2 = null;
            qBasic.BidGuaranteeAmount1 = null;
            qBasic.BidGuaranteeAmount2 = null;
            qBasic.AdditionalFee1 = null;
            qBasic.AdditionalFee2 = null;
            qBasic.AdditionalFee3 = null;
            qBasic.AdditionalApproveNo1 = null;
            qBasic.AdditionalApproveNo2 = null;
            qBasic.AdditionalApproveNo3 = null;
            qBasic.ApproveNo1 = null;
            qBasic.ApproveNo2 = null;
            qBasic.ApproveNo3 = null;
            qBasic.ApproveNo4 = null;
            qBasic.ApproveNo5 = null;
            qBasic.ContractFee = null;
            qBasic.ProductPrice = null;
            qBasic.InstallationFee = null;
            qBasic.DepositFee = null;
            qBasic.FacilityMemo = null;
            qBasic.MaintenanceMemo = null;
            qBasic.SecurityItemFee = null;
            qBasic.OtherItemFee = null;
            qBasic.SentryGuardAreaTypeCode = null;
            qBasic.SentryGuardFee = null;
            qBasic.TotalSentryGuardFee = null;
            qBasic.MaintenanceTargetProductTypeCode = "1";
            qBasic.MaintenanceTypeCode = null;
            qBasic.MaintenanceCycle = null;
            qBasic.CreateDate = new DateTime(2011, 7, 11, 12, 9, 0);
            qBasic.CreateBy = "440065";
            qBasic.UpdateDate = new DateTime(2011, 7, 13, 12, 9, 0);
            qBasic.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationBasic = qBasic;

            //dtTbt_QuotationOperationType[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationOperationType = new List<tbt_QuotationOperationType>();

            //dtTbt_QuotationInstrumentDetails[]------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationInstrumentDetails = new List<tbt_QuotationInstrumentDetails>();

            //dtTbt_QuotationFacilityDetails[]--------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationFacilityDetails = new List<tbt_QuotationFacilityDetails>();

            //dtTbt_QuotationBeatGuardDetails[]-------------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationBeatGuardDetails = null;

            //dtTbt_QuotationSentryGuardDetails[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationSentryGuardDetails = new List<tbt_QuotationSentryGuardDetails>();

            //dtTbt_QuotationMaintenanceLinkage[]-----------------------------------------------------------------------------------
            dsQuotationData.dtTbt_QuotationMaintenanceLinkage = new List<tbt_QuotationMaintenanceLinkage>();

            tbt_QuotationMaintenanceLinkage qMaintLink1 = new tbt_QuotationMaintenanceLinkage();
            qMaintLink1.QuotationTargetCode = "FMA0000000083";
            qMaintLink1.Alphabet = "ZZ";
            qMaintLink1.ContractCode = "Q0000000521";
            qMaintLink1.CreateDate = new DateTime(2011, 8, 24, 0, 0, 0);
            qMaintLink1.CreateBy = "440065";
            qMaintLink1.UpdateDate = new DateTime(2011, 8, 24, 0, 0, 0);
            qMaintLink1.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationMaintenanceLinkage.Add(qMaintLink1);

            tbt_QuotationMaintenanceLinkage qMaintLink2 = new tbt_QuotationMaintenanceLinkage();
            qMaintLink2.QuotationTargetCode = "FMA0000000083";
            qMaintLink2.Alphabet = "ZZ";
            qMaintLink2.ContractCode = "Q0000000536";
            qMaintLink2.CreateDate = new DateTime(2011, 8, 24, 0, 0, 0);
            qMaintLink2.CreateBy = "440065";
            qMaintLink2.UpdateDate = new DateTime(2011, 8, 24, 0, 0, 0);
            qMaintLink2.UpdateBy = "440065";
            dsQuotationData.dtTbt_QuotationMaintenanceLinkage.Add(qMaintLink2);

            return dsQuotationData;
        }

    }
}
