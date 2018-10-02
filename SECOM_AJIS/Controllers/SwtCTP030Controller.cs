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
    public class SwtCTP030Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP030/

        public string index()
        {
            List<string> lst = new List<string>();
            ////Rental Approve
            //lst.Add(Case1());
            //lst.Add(Case2());

            ////Rental Complete Install
            //lst.Add(Case3());
            //lst.Add(Case4());
            //lst.Add(Case5());
            //lst.Add(Case6());

            ////Start Service
            //lst.Add(Case7());
            //lst.Add(Case8());

            ////Stop Service
            //lst.Add(Case9());
            //lst.Add(Case10());

            ////Resume Service
            //lst.Add(Case11());
            //lst.Add(Case12());

            ////Rental Cancel
            //lst.Add(Case13());
            //lst.Add(Case14());
            //lst.Add(Case15());
            //lst.Add(Case16());

            ////Change Fee
            //lst.Add(Case17());
            //lst.Add(Case18());

            ////Change Name
            //lst.Add(Case19());
            //lst.Add(Case20());

            ////Rental Change Plan
            //lst.Add(Case21());
            //lst.Add(Case22());
            //lst.Add(Case23());
            //lst.Add(Case24());
            //lst.Add(Case25());

            ////Sale Approve
            //lst.Add(Case26());
            //lst.Add(Case27());
            //lst.Add(Case28());

            ////Sale Complete Install
            //lst.Add(Case29());
            //lst.Add(Case30());

            //MA Result Base
            lst.Add(Case31());
            lst.Add(Case32());
            lst.Add(Case33());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");

            return result;
        }

        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code]
        ///</summary>
        public string Case1()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.SendBilling_RentalApprove(null);
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
        ///Purpose   : Send rental approve billing data to billing module
        ///Parameters: - ContractCode = N0003000215
        ///Expected  : 1. Call BLP010 : Manage billing basic information of contract fee and other fee of approve phase 
        ///            2. Update billing temp data of contract fee and other fee of approve phase
        ///            3. Call BLP011 : Manage billing detail by contract of approve phase 
        ///            4. Update send flag in Billing temp of approve  phase
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation billing temp data in Exp : TestCase 2
        ///</summary>
        public string Case2()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string strContractCode = "N0003000215";

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_RentalApprove(strContractCode);
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
                return string.Format(RESULT_FORMAT_LIST, 2, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 2, "Fail", error);
            }
        }


        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code]
        ///</summary>
        public string Case3()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.SendBilling_RentalCompleteInstall(null,DateTime.Now);
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
        ///Purpose   : Send complete install billing data to billing module (Contract before start)
        ///Parameters: - ContractCode = N0003000215
        ///Expected  : 1. Call BLP010 : Manage billing basic information of deposit fee and other fee of complete install phase
        ///            2. Update billing temp data of deposit fee and other fee of complete install phase
        ///            3. Call BLP011 : Manage billing detail by contract of complete install phase
        ///            4. Update send flag in Billing temp of complete  phase
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation billing temp data in Exp : TestCase 4
        ///</summary>
        public string Case4()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string strContractCode = "N0003000215";

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_RentalCompleteInstall(strContractCode,DateTime.Now);
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
                return string.Format(RESULT_FORMAT_LIST, 4, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 4, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Send complete install billing data to billing module (Contract after start)
        ///Parameters: - ContractCode = MA0003000225
        ///Expected  : 1. Call BLP010 : Manage billing basic information of deposit fee and other fee of complete install phase
        ///            2. Update billing temp data of deposit fee and other fee of complete install phase
        ///            3. Call BLP011 : Manage billing detail by contract of complete install phase 
        ///            4. Update send flag in Billing temp of complete  phase
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation data and parameter in Exp : TestCase 5
        ///</summary>
        public string Case5()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string strContractCode = "MA0003000225";

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_RentalCompleteInstall(strContractCode,DateTime.Now);
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
                return string.Format(RESULT_FORMAT_LIST, 5, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 5, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Send complete install billing data to billing module  (Contract stop)
        ///Parameters: - ContractCode = N0003000235
        ///Expected  : 1. Call BLP010 : Manage billing basic information of deposit fee and other fee of complete install phase
        ///            2. Update billing temp data of deposit fee and other fee of complete install phase
        ///            3. Call BLP011 : Manage billing detail by contract of complete install phase 
        ///            4. Update send flag in Billing temp of complete  phase
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation data and parameter in Exp : TestCase 6
        ///</summary>
        public string Case6()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string strContractCode = "N0003000235";

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_RentalCompleteInstall(strContractCode,DateTime.Now);
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
                return string.Format(RESULT_FORMAT_LIST, 6, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 6, "Fail", error);
            }
        }


        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code], [StartDate]
        ///</summary>
        public string Case7()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.SendBilling_StartService(null, null, null,null);
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
        ///Purpose   : Send start service billing data to billing module
        ///Parameters: - ContractCode = N0003000215
        ///            - StartDate = Date today
        ///            - AdjustBillingTermEndDate = The last day of this month
        ///Expected  : 1. Call BLP010 : Manage billing basic information of contract fee and other fee of start service phase
        ///            2. Update billing temp data of contract fee and other fee of start servicel phase
        ///            3. Call BLP011 : Manage billing detail by contract of start service phase 
        ///            4. Update send flag in Billing temp of complete  phase
        ///            5. Call BLP012 : Manage billing basic for start of start service phase
        ///            6. Return blnProcessResult = TRUE to caller
        ///            7. See expectation data and parameter in Exp : TestCase 8
        ///</summary>
        public string Case8()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string strContractCode = "N0003000215";
            DateTime? dStartDate = DateTime.Now;
            DateTime? AdjustBillingTermEndDate = CommonUtil.LastDayOfMonthFromDateTime(dStartDate);

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_StartService(strContractCode, dStartDate, AdjustBillingTermEndDate, null);
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
                return string.Format(RESULT_FORMAT_LIST, 8, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 8, "Fail", error);
            }
        }


        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code], [StopDate]
        ///</summary>
        public string Case9()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.SendBilling_StopService(null, null, null);
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
        ///Purpose   : Send stop service billing data to billing module
        ///Parameters: - ContractCode = MA0003000225
        ///            - StartDate = Date today
        ///            - StopFee = 15000
        ///Expected  : 1. Call BLP015 : Manage billing basic for stop and set parameter for calling
        ///            ContractCode = MA3000225
        ///            StopDate = Date today
        ///            StopFee = 15000
        ///            
        ///            2. Return blnProcessResult = TRUE to caller
        ///</summary>
        public string Case10()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string strContractCode = "MA0003000225";
            DateTime? dStopDate = DateTime.Now;
            decimal? StopFee = 15000;

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_StopService(strContractCode, dStopDate, StopFee);
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
                return string.Format(RESULT_FORMAT_LIST, 10, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 10, "Fail", error);
            }
        }


        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code], [ResumeDate]
        ///</summary>
        public string Case11()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.SendBilling_ResumeService(null, null);
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
        ///Purpose   : Send resume service billing data to billing module
        ///Parameters: - ContractCode = N0003000235
        ///            - StartDate = Date today
        ///Expected  : 1. Call BLP013 : Manage billing basic for resume and set parameter for calling
        ///            strContractCode = N3000235
        ///            dtpResumeDate = Date today
        ///            
        ///            2. Return blnProcessResult = TRUE to caller
        ///</summary>
        public string Case12()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string strContractCode = "N0003000235";
            DateTime? ResumeDate = DateTime.Now;

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_ResumeService(strContractCode, ResumeDate);
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
                return string.Format(RESULT_FORMAT_LIST, 12, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 12, "Fail", error);
            }
        }


        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code], [CancelDate], [CompleteInstallFlag]
        ///</summary>
        public string Case13()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.SendBilling_RentalCancel(null, null, null, null, null);
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
        ///Purpose   : Send cancel billing data to billing module (First complete install and cancel befer start service)
        ///Parameters: - ContractCode = N0003000245
        ///            - CancelDate = Date today
        ///            
        ///            - doBillingTempBasicCancel
        ///            -    ContractCode = N0003000245
        ///            -    BillingOCC = 01
        ///            -    BillingClientCode = 0000000001
        ///            -    BillingOfficeCode = 1020 
        ///            -    BillingTargetCode = 0000000001-001
        ///            -    PaymentMethod = 2 : Credit card
        ///            -    ContractBillingType = 09 : Cancel contract fee
        ///            -    BillingAmount = 555.50
        ///
        ///            - doBillingTempDetailCancel
        ///            -    ContractCode = N0003000245
        ///            -    SequenceNo = NULL
        ///            -    BillingOCC = 01
        ///            -    ContractBillingType = 09 : Cancel contract fee
        ///            -    BillingDate = Date today
        ///            -    BillingAmount = 555.50
        ///            -    PaymentMethod = 2 : Credit card
        ///            -    BillingTiming = 1
        ///            - CompleteInstallFlag = 1
        ///Expected  : 1. Call BLP010 : Manage billing basic information
        ///            2. Call BLP011 : Manage billing detail by contract 
        ///            3. Call BLP016 : Manage billing basic information for cancel contract
        ///            4. Delete all record of this contract code in billing temp table
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation data and parameter in Exp : TestCase 14
        ///</summary>
        public string Case14()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string strContractCode = "N0003000245";
            DateTime? CancelDate = DateTime.Now;
            bool? CompleteInstallFlag = true;

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                doBillingTempBasic doBillingTempBasicData = new doBillingTempBasic();
                doBillingTempBasicData.ContractCode = "N0003000245";
                doBillingTempBasicData.BillingOCC = "01";
                doBillingTempBasicData.BillingClientCode = "0000000001";
                doBillingTempBasicData.BillingOfficeCode = "1020";
                doBillingTempBasicData.BillingTargetCode = "0000000001-001";
                doBillingTempBasicData.PaymentMethod = "2";
                doBillingTempBasicData.ContractBillingType = "09";
                doBillingTempBasicData.BillingAmount = 555.50M;

                doBillingTempDetail doBillingTempDetailData = new doBillingTempDetail();
                doBillingTempDetailData.ContractCode = "N0003000245";
                //doBillingTempDetailData.SequenceNo = null;
                doBillingTempDetailData.BillingOCC = "01";
                doBillingTempDetailData.ContractBillingType = "09";
                doBillingTempDetailData.BillingDate = DateTime.Now;
                doBillingTempDetailData.BillingAmount = 555.50M;
                doBillingTempDetailData.PaymentMethod = "2";
                doBillingTempDetailData.BillingTiming = "1";

                actual = target.SendBilling_RentalCancel(strContractCode, CancelDate, doBillingTempBasicData, doBillingTempDetailData, CompleteInstallFlag);
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
                return string.Format(RESULT_FORMAT_LIST, 14, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 14, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Send cancel billing data to billing module (Cancel befer start service)
        ///Parameters: - ContractCode = N0003000255
        ///            - CancelDate = Date today
        ///            
        ///            - doBillingTempBasicCancel
        ///            -    ContractCode = N0003000255
        ///            -    BillingOCC = 01
        ///            -    BillingClientCode = 0000000001
        ///            -    BillingOfficeCode = 1020 
        ///            -    BillingTargetCode = 0000000001-001
        ///            -    PaymentMethod = 2 : Credit card
        ///            -    ContractBillingType = 09 : Cancel contract fee
        ///            -    BillingAmount = 555.50
        ///
        ///            - doBillingTempDetailCancel
        ///            -    ContractCode = N0003000255
        ///            -    SequenceNo = NULL
        ///            -    BillingOCC = 01
        ///            -    ContractBillingType = 09 : Cancel contract fee
        ///            -    BillingDate = Date today
        ///            -    BillingAmount = 555.50
        ///            -    PaymentMethod = 2 : Credit card
        ///            -    BillingTiming = 1
        ///            - CompleteInstallFlag = 0
        ///Expected  : 1. Call BLP010 : Manage billing basic information
        ///            2. Call BLP011 : Manage billing detail by contract 
        ///            3. Call BLP016 : Manage billing basic information for cancel contract
        ///            4. Delete all record of this contract code in billing temp table
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation data and parameter in Exp : TestCase 15
        ///</summary>
        public string Case15()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string strContractCode = "N0003000255";
            DateTime? CancelDate = DateTime.Now;
            bool? CompleteInstallFlag = false;

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                doBillingTempBasic doBillingTempBasicData = new doBillingTempBasic();
                doBillingTempBasicData.ContractCode = "N0003000255";
                doBillingTempBasicData.BillingOCC = "01";
                doBillingTempBasicData.BillingClientCode = "0000000001";
                doBillingTempBasicData.BillingOfficeCode = "1020";
                doBillingTempBasicData.BillingTargetCode = "0000000001-001";
                doBillingTempBasicData.PaymentMethod = "2";
                doBillingTempBasicData.ContractBillingType = "09";
                doBillingTempBasicData.BillingAmount = 555.50M;

                doBillingTempDetail doBillingTempDetailData = new doBillingTempDetail();
                doBillingTempDetailData.ContractCode = "N0003000255";
                //doBillingTempDetailData.SequenceNo = null;
                doBillingTempDetailData.BillingOCC = "01";
                doBillingTempDetailData.ContractBillingType = "09";
                doBillingTempDetailData.BillingDate = DateTime.Now;
                doBillingTempDetailData.BillingAmount = 555.50M;
                doBillingTempDetailData.PaymentMethod = "2";
                doBillingTempDetailData.BillingTiming = "1";

                actual = target.SendBilling_RentalCancel(strContractCode, CancelDate, doBillingTempBasicData, doBillingTempDetailData, CompleteInstallFlag);
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
                return string.Format(RESULT_FORMAT_LIST, 15, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 15, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Send cancel billing data to billing module (Cancel befer start service)
        ///Parameters: - ContractCode = N0003000235
        ///            - CancelDate = Date today
        ///            
        ///            - doBillingTempBasicCancel
        ///            -    ContractCode = N0003000235
        ///            -    BillingOCC = 01
        ///            -    BillingClientCode = 0000000001
        ///            -    BillingOfficeCode = 0001 
        ///            -    BillingTargetCode = 0000000001-001
        ///            -    PaymentMethod = 1 : Other transfer
        ///            -    ContractBillingType = 09 : Cancel contract fee
        ///            -    BillingAmount = 15000.50
        ///
        ///            - doBillingTempDetailCancel
        ///            -    ContractCode = N0003000235
        ///            -    SequenceNo = NULL
        ///            -    BillingOCC = 01
        ///            -    ContractBillingType = 09 : Cancel contract fee
        ///            -    BillingDate = Date today
        ///            -    BillingAmount = 15000.50
        ///            -    PaymentMethod = 1 : Other transfer
        ///            -    BillingTiming = 1
        ///            - CompleteInstallFlag = 0
        ///Expected  : 1. Call BLP010 : Manage billing basic information
        ///            2. Call BLP011 : Manage billing detail by contract 
        ///            3. Call BLP016 : Manage billing basic information for cancel contract
        ///            4. Delete all record of this contract code in billing temp table
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation data and parameter in Exp : TestCase 16
        ///</summary>
        public string Case16()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string strContractCode = "N0003000235";
            DateTime? CancelDate = DateTime.Now;
            bool? CompleteInstallFlag = true;

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                doBillingTempBasic doBillingTempBasicData = new doBillingTempBasic();
                doBillingTempBasicData.ContractCode = "N0003000235";
                doBillingTempBasicData.BillingOCC = "01";
                doBillingTempBasicData.BillingClientCode = "0000000001";
                doBillingTempBasicData.BillingOfficeCode = "0001";
                doBillingTempBasicData.BillingTargetCode = "0000000001-001";
                doBillingTempBasicData.PaymentMethod = "1";
                doBillingTempBasicData.ContractBillingType = "09";
                doBillingTempBasicData.BillingAmount = 15000.50M;

                doBillingTempDetail doBillingTempDetailData = new doBillingTempDetail();
                doBillingTempDetailData.ContractCode = "N0003000235";
                //doBillingTempDetailData.SequenceNo = null;
                doBillingTempDetailData.BillingOCC = "01";
                doBillingTempDetailData.ContractBillingType = "09";
                doBillingTempDetailData.BillingDate = DateTime.Now;
                doBillingTempDetailData.BillingAmount = 15000.50M;
                doBillingTempDetailData.PaymentMethod = "1";
                doBillingTempDetailData.BillingTiming = "1";

                actual = target.SendBilling_RentalCancel(strContractCode, CancelDate, doBillingTempBasicData, doBillingTempDetailData, CompleteInstallFlag);
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
                return string.Format(RESULT_FORMAT_LIST, 16, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 16, "Fail", error);
            }
        }


        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code], 
        ///            [BillingOCC] or [BillingTargetCode] or [BillingOfficeCode and BillingClientCode], 
        ///            [ContractBillingType], [BillingAmount], [PaymentMethod],[BillingCycle]
        ///</summary>
        public string Case17()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                List<doBillingTempBasic> doBillingTempBasicList = new List<doBillingTempBasic>();
                doBillingTempBasic doBillingTempBasicData = new doBillingTempBasic();
                doBillingTempBasicList.Add(doBillingTempBasicData);

                target.SendBilling_ChangeFee(doBillingTempBasicList);
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

            return string.Format(RESULT_FORMAT, 17, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Send change fee billing data to billing module
        ///Parameters: - doBillingTempBasicList[1]
        ///            -   ContractCode = N0003000225
        ///            -   BillingOCC = 01
        ///            -   BillingClientCode = 0000000001
        ///            -   BillingOfficeCode = 1000 
        ///            -   BillingTargetCode = 0000000001-001
        ///            -   PaymentMethod = 0 : Bank transfer
        ///            -   ContractBillingType = 01 : Contract fee
        ///            -   BillingAmount = 10000.50
        ///            -   BillingCycle = 3
        ///            -   CalculationDailyFee = 1
        ///            -   ContractTiming = 3
        ///            -   ProductTypeCode = 2 : Alarm
        ///            -   ChangeFeeDate = Date today
        ///            -   CreditTerm = NULL
        ///            -   
        ///            - doBillingTempBasicList[2]
        ///            -   ContractCode = N0003000225
        ///            -   BillingOCC = NULL
        ///            -   BillingClientCode = NULL
        ///            -   BillingOfficeCode = 1000 
        ///            -   BillingTargetCode = 0000000002-001
        ///            -   PaymentMethod = 0 : Bank transfer
        ///            -   ContractBillingType = 01 : Contract fee
        ///            -   BillingAmount = 99999.99
        ///            -   BillingCycle = 3
        ///            -   CalculationDailyFee = 1
        ///            -   ContractTiming = 3
        ///            -   ProductTypeCode = 2 : Alarm
        ///            -   ChangeFeeDate = Date today
        ///            -   CreditTerm = NULL
        ///Expected  : 1. Call BLP010 : Manage billing basic information (This method will return doBillingTempBasicList)
        ///            see expectation in STC of BLP010 Manage billing basic information
        ///            
        ///            2. Return blnProcessResult = TRUE to caller
        ///</summary>
        public string Case18()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                List<doBillingTempBasic> doBillingTempBasicList = new List<doBillingTempBasic>();

                doBillingTempBasic doBillingTempBasicData = new doBillingTempBasic();
                doBillingTempBasicData.ContractCode = "N0003000225";
                doBillingTempBasicData.BillingOCC = "01";
                doBillingTempBasicData.BillingClientCode = "0000000001";
                doBillingTempBasicData.BillingOfficeCode = "1000";
                doBillingTempBasicData.BillingTargetCode = "0000000001-001";
                doBillingTempBasicData.PaymentMethod = "0";
                doBillingTempBasicData.ContractBillingType = "01";
                doBillingTempBasicData.BillingAmount = 10000.50M;
                doBillingTempBasicData.BillingCycle = 3;
                doBillingTempBasicData.CalculationDailyFee = "1";
                doBillingTempBasicData.ContractTiming = 3;
                doBillingTempBasicData.ProductTypeCode = "2";
                doBillingTempBasicData.ChangeFeeDate = DateTime.Now;
                //doBillingTempBasicData.CreditTerm = null;
                doBillingTempBasicList.Add(doBillingTempBasicData);

                doBillingTempBasicData = new doBillingTempBasic();
                doBillingTempBasicData.ContractCode = "N0003000225";
                doBillingTempBasicData.BillingOCC = null;
                doBillingTempBasicData.BillingClientCode = null;
                doBillingTempBasicData.BillingOfficeCode = "1000";
                doBillingTempBasicData.BillingTargetCode = "0000000002-001";
                doBillingTempBasicData.PaymentMethod = "0";
                doBillingTempBasicData.ContractBillingType = "01";
                doBillingTempBasicData.BillingAmount = 99999.99M;
                doBillingTempBasicData.BillingCycle = 3;
                doBillingTempBasicData.CalculationDailyFee = "1";
                doBillingTempBasicData.ContractTiming = 3;
                doBillingTempBasicData.ProductTypeCode = "2";
                doBillingTempBasicData.ChangeFeeDate = DateTime.Now;
                //doBillingTempBasicData.CreditTerm = null;
                doBillingTempBasicList.Add(doBillingTempBasicData);

                actual = target.SendBilling_ChangeFee(doBillingTempBasicList);
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
                return string.Format(RESULT_FORMAT_LIST, 18, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 18, "Fail", error);
            }
        }


        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code], [BillingOCC], 
        ///            [BillingTargetCode] or [BillingOfficeCode and BillingClientCode]
        ///</summary>
        public string Case19()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                List<doBillingTempBasic> doBillingTempBasicList = new List<doBillingTempBasic>();
                doBillingTempBasic doBillingTempBasicData = new doBillingTempBasic();
                doBillingTempBasicList.Add(doBillingTempBasicData);

                target.SendBilling_ChangeName(doBillingTempBasicList);
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

            return string.Format(RESULT_FORMAT, 19, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Send change name billing data to billing module
        ///Parameters: - doBillingTempBasicList[1]
        ///            -   ContractCode = N0003000225
        ///            -   BillingOCC = 01
        ///            -   BillingClientCode = 0000000001
        ///            -   BillingOfficeCode = 1000 
        ///            -   Set NULL to other data
        ///              
        ///            - doBillingTempBasicList[2]
        ///            -   ContractCode = N0003000225
        ///            -   BillingOCC = 02
        ///            -   BillingTargetCode = 0000000002-001
        ///            -   Set NULL to other data
        ///Expected  : 1. Call BLP017 : Manage billing basic information for change name and address (This method will return doBillingTempBasicList)
        ///            see expectation in STC of BLP010 Manage billing basic information
        ///            
        ///            2. Return blnProcessResult = TRUE to caller
        ///</summary>
        public string Case20()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                List<doBillingTempBasic> doBillingTempBasicList = new List<doBillingTempBasic>();

                doBillingTempBasic doBillingTempBasicData = new doBillingTempBasic();
                doBillingTempBasicData.ContractCode = "N0003000225";
                doBillingTempBasicData.BillingOCC = "01";
                doBillingTempBasicData.BillingClientCode = "0000000001";
                doBillingTempBasicData.BillingOfficeCode = "1000";
                doBillingTempBasicList.Add(doBillingTempBasicData);

                doBillingTempBasicData = new doBillingTempBasic();
                doBillingTempBasicData.ContractCode = "N0003000225";
                doBillingTempBasicData.BillingOCC = "02";
                doBillingTempBasicData.BillingTargetCode = "0000000002-001";
                doBillingTempBasicList.Add(doBillingTempBasicData);

                actual = target.SendBilling_ChangeName(doBillingTempBasicList);
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
                return string.Format(RESULT_FORMAT_LIST, 20, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 20, "Fail", error);
            }
        }


        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code], [OCC]
        ///</summary>
        public string Case21()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.SendBilling_RentalChangePlan(null, null);
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

            return string.Format(RESULT_FORMAT, 21, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Send change plan billing data to billing module (First complete installation)
        ///Parameters: - ContractCode = MA0003000285
        ///            - OCC = 9990
        ///Expected  : 1. Call BLP010 : Manage billing basic information of contract fee and other fee
        ///            2. Update billing temp data of contract fee and other fee of change plan
        ///            3. Call BLP011 : Manage billing detail by contract of after register phase
        ///            4. Update send flag in Billing temp of after register
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation billing temp data in Exp : TestCase 22
        ///</summary>
        public string Case22()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            string ContractCode = "MA0003000285";
            string OCC = "9990";

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_RentalChangePlan(ContractCode, OCC);
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
                return string.Format(RESULT_FORMAT_LIST, 22, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 22, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Send change plan billing data to billing module (Before start)
        ///Parameters: - ContractCode = N0003000265
        ///            - OCC = 0001
        ///Expected  : 1. Call BLP010 : Manage billing basic information of contract fee and other fee
        ///            2. Update billing temp data of contract fee and other fee of change plan
        ///            3. Call BLP011 : Manage billing detail by contract of after register phase
        ///            4. Update send flag in Billing temp of after register
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation billing temp data in Exp : TestCase 23
        ///</summary>
        public string Case23()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            string ContractCode = "N0003000265";
            string OCC = "0001";

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_RentalChangePlan(ContractCode, OCC);
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
                return string.Format(RESULT_FORMAT_LIST, 23, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 23, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Send change plan billing data to billing module (Stopping)
        ///Parameters: - ContractCode = N0003000275
        ///            OCC = 9980
        ///Expected  : 1. Call BLP010 : Manage billing basic information of contract fee and other fee
        ///            2. Update billing temp data of contract fee and other fee of change plan
        ///            3. Call BLP011 : Manage billing detail by contract of after register phase
        ///            4. Update send flag in Billing temp of after register
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation billing temp data in Exp : TestCase 24
        ///</summary>
        public string Case24()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            string ContractCode = "N0003000275";
            string OCC = "9980";

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_RentalChangePlan(ContractCode, OCC);
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
                return string.Format(RESULT_FORMAT_LIST, 24, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 24, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Send change plan billing data to billing module 
        ///Parameters: - ContractCode = N0003000255
        ///            - OCC = 9980
        ///Expected  : 1. Call BLP010 : Manage billing basic information of contract fee and other fee
        ///            2. Update billing temp data of contract fee and other fee of change plan
        ///            3. Call BLP011 : Manage billing detail by contract of after register phase
        ///            4. Update send flag in Billing temp of after register
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation billing temp data in Exp : TestCase 24
        ///</summary>
        public string Case25()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            string ContractCode = "N0003000255";
            string OCC = "9980";

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_RentalChangePlan(ContractCode, OCC);
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
                return string.Format(RESULT_FORMAT_LIST, 25, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 25, "Fail", error);
            }
        }


        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code], [SaleOCC]
        ///</summary>
        public string Case26()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.SendBilling_SaleApprove(null, null);
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

            return string.Format(RESULT_FORMAT, 26, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Send approve billing data to billing module (Keep in billing basic and billing detaill)
        ///Parameters: - ContractCode = Q0000030015
        ///            - OCC = 9990
        ///Expected  : 1. Call BLP010 : Manage billing basic information of contract fee and other fee
        ///            2. Update billing temp data of contract fee and other fee of change plan
        ///            3. Call BLP011 : Manage billing detail by contract of after register phase
        ///            4. Update send flag in Billing temp of after register
        ///            5. Return blnProcessResult = TRUE to caller
        ///            6. See expectation billing temp data in Exp : TestCase 27
        ///</summary>
        public string Case27()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            string ContractCode = "Q0000030015";
            string OCC = "9990";

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_SaleApprove(ContractCode, OCC);
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
                return string.Format(RESULT_FORMAT_LIST, 27, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 27, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Send approve billing data to billing module (Keep billing basic only)
        ///Parameters: - ContractCode = Q0000030025
        ///            - OCC = 9990
        ///Expected  : 1. Call BLP010 : Manage billing basic information of contract fee and other fee
        ///            2. Update billing temp data of contract fee and other fee of change plan
        ///            3. Return blnProcessResult = TRUE to caller
        ///            4. See expectation billing temp data in Exp : TestCase 28
        ///</summary>
        public string Case28()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            string ContractCode = "Q0000030025";
            string OCC = "9980";

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_SaleApprove(ContractCode, OCC);
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
                return string.Format(RESULT_FORMAT_LIST, 28, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 28, "Fail", error);
            }
        }


        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code], [BillingOCC],
        ///            [ContractBillingType], [BillingAmount]
        ///</summary>
        public string Case29()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.SendBilling_SaleCompleteInstall(null);
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

            return string.Format(RESULT_FORMAT, 29, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Send complete install billing data to billing module 
        ///Parameters: - BillingTempDetailCance[] 
        ///            -   ContractCode = Q0000030035
        ///            -   SequenceNo = NULL
        ///            -   BillingOCC = 01
        ///            -   ContractBillingType = 06 : Installation Fee
        ///            -   BillingDate = 1-Jan-2012
        ///            -   BillingAmount =12345.00
        ///            -   PaymentMethod = NULL
        ///            -   ProductTypeCode = NULL
        ///            -   ProductCode = NULL
        ///            -   BillingType = NULL
        ///Expected  : 1. Call BLP011 : Manage billing detail by contract of sale complete install and send BillingTempDetailCancel[]  
        ///            ContractCode = Q0000030035
        ///            SequenceNo = NULL
        ///            BillingOCC = 01
        ///            ContractBillingType = 06 : InstallationFee
        ///            BillingDate = Date now
        ///            BillingAmount =12345.00
        ///            PaymentMethod = NULL
        ///            ProductTypeCode = 1
        ///            ProductCode = 047
        ///            BillingType = NULL
        ///            2. Return blnProcessResult = TRUE to caller
        ///</summary>
        public string Case30()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                doBillingTempDetail doBillingTempDetailData = new doBillingTempDetail();
                doBillingTempDetailData.ContractCode = "Q0000030035";
                //doBillingTempDetailData.SequenceNo = null;
                doBillingTempDetailData.BillingOCC = "01";
                doBillingTempDetailData.ContractBillingType = "06";
                doBillingTempDetailData.BillingDate = new DateTime(2012, 1, 1);
                doBillingTempDetailData.BillingAmount = 12345.00M;
                doBillingTempDetailData.PaymentMethod = null;
                doBillingTempDetailData.ProductTypeCode = null;
                doBillingTempDetailData.ProductCode = null;

                actual = target.SendBilling_SaleCompleteInstall(doBillingTempDetailData);
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
                return string.Format(RESULT_FORMAT_LIST, 30, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 30, "Fail", error);
            }
        }


        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract Code], [BillingOCC], [ResultBaseFee]
        ///</summary>
        public string Case31()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
            string expected = "MSG0007";
            string actual;

            try
            {
                target.SendBilling_MAResultBase(null, null, null);
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

            return string.Format(RESULT_FORMAT, 31, expected, actual, CompareResult_String(expected, actual));
        }

        ///<summary>
        ///Purpose   : Validate result base fee  must not = 0
        ///Parameters: - ContractCode = MA0003000225
        ///            - BillingOCC =  01
        ///            - ResultBaseFee = 0
        ///Expected  : 1. Do not thing 
        ///            2. Return blnProcessResult = TRUE to caller
        ///</summary>
        public string Case32()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            string ContractCode = "MA0003000225";
            string BillingOCC = "01";
            decimal? ResultBaseFee = 0;

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_MAResultBase(ContractCode, BillingOCC, ResultBaseFee);
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
                return string.Format(RESULT_FORMAT_LIST, 32, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 32, "Fail", error);
            }
        }

        ///<summary>
        ///Purpose   : Send MA Result basel billing data to billing module 
        ///Parameters: - ContractCode = MA0003000225
        ///            - BillingOCC =  01
        ///            - ResultBaseFee = 5000.00
        ///Expected  : 1. Call BLP011 : Manage billing detail by contract of sale complete install and send BillingTempDetailCancel[]  
        ///              ContractCode = MA0003000225
        ///              SequenceNo = NULL
        ///              BillingOCC = 01
        ///              ContractBillingType = 04 
        ///              BillingDate = Date now
        ///              BillingAmount =5000.00
        ///              PaymentMethod = NULL
        ///              ProductTypeCode = 6
        ///              ProductCode = NULL
        ///              BillingType = NULL
        ///            2. Return blnProcessResult = TRUE to caller
        ///</summary>
        public string Case33()
        {
            IBillingInterfaceHandler target = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;

            string ContractCode = "MA0003000225";
            string BillingOCC = "01";
            decimal? ResultBaseFee = 5000.00M;

            bool expected = true;
            bool actual = false;
            string error = string.Empty;

            try
            {
                actual = target.SendBilling_MAResultBase(ContractCode, BillingOCC, ResultBaseFee);
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
                return string.Format(RESULT_FORMAT_LIST, 33, expected == actual);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 33, "Fail", error);
            }
        }

    }
}
