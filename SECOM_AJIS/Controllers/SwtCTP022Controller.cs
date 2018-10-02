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
    public class SwtCTP022Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP022/

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
        ///Expected  : MSG0007: These fields are required: [Contract code], [OCC], [BillingType], [BillingAmt], [PayMethod] and
        ///            At least one of [BillingOCC or BillingTargetCode or (BillingClientCode and BillingOfficeCode)]
        ///</summary>
        public string Case1()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            tbt_BillingTemp data = null;
            string expected = "MSG0007";
            string actual;
            
            try
            {
                target.UpdateBillingTempByKey(data);
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
        ///Purpose   : Keep billing data (Edit)
        ///Parameters: tbt_BillingTemp
        ///            - ContractCode = N0000012225
        ///            - OCC = 0001
        ///            - SequenceNo = 3
        ///            - BillingOCC = 05
        ///            - BillingTargetRunningNo = 005
        ///            - BillingClientCode = C000012015
        ///            - BillingTargetCode = C000012015-005
        ///            - BillingOfficeCode = 1000
        ///            - BillingType = 03
        ///            - CreditTerm = 90
        ///            - BillingTiming = 6
        ///            - BillingAmt = 99999999.9999
        ///            - PayMethod = 4
        ///            - BillingCycle = 6
        ///            - CalDailyFeeStatus = 0
        ///            - SendFlag = 1
        ///            - CreateDate = Date today 
        ///            - CreateBy = 500576
        ///            - UpdateDate = Date today 
        ///            - UpdateBy = 500576
        ///Expected  : Update tbt_BillingTemp :
        ///            Contract code = N0012135
        ///            OCC = 0001
        ///            SequenceNo = 3
        ///            
        ///            Keep operation log to log table
        ///            
        ///            See expectation data test case 2 
        ///</summary>
        public string Case2()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            tbt_BillingTemp data = new tbt_BillingTemp();
            tbt_BillingTemp expected = new tbt_BillingTemp();
            tbt_BillingTemp actual = new tbt_BillingTemp();
            string error = string.Empty;

            DateTime dtProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            string strFieldName = "ContractCode OCC SequenceNo BillingOCC BillingTargetRunningNo BillingClientCode BillingTargetCode BillingOfficeCode BillingType CreditTerm BillingTiming BillingAmt PayMethod BillingCycle CalDailyFeeStatus SendFlag UpdateDate UpdateBy";

            try
            {
                data = this.CreateParameterForCase2(dtProcessDateTime);
                List<tbt_BillingTemp> resultList = target.UpdateBillingTempByKey(data);
                if (resultList != null && resultList.Count > 0)
                    actual = resultList[0];

                expected = this.CreateExpectedForCase2(dtProcessDateTime);
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
                bool bResult = CompareObject<tbt_BillingTemp>(actual, expected, strFieldName);
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
        ///Expected  : MSG0007: These fields are required: [Contract code], [OldBillingClientCode], [OldBillingOfficeCode], [OldBillingTargetCode]
        ///</summary>
        public string Case3()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

            string strContractCode = null;
            string strOldBillingClientCode = null;
            string strOldBillingOfficeCode = null;
            string strOldBillingTargetCode = null;
            string strNewBillingClientCode = null;
            string strNewBillingOfficeCode = null;
            string strNewBillingTargetCode = null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.UpdateBillingTempByBillingTarget(strContractCode, strOldBillingClientCode, strOldBillingOfficeCode, strOldBillingTargetCode
                                                        , strNewBillingClientCode, strNewBillingOfficeCode, strNewBillingTargetCode);
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
        ///Purpose   : Keep billing data (Edit)
        ///Parameters: - ContractCode = N0000012125
        ///            - OldBillingClientCode = C000012015
        ///            - OldBillingOfficeCode = 1020
        ///            - OldBillingTargetCode = C000012015-001
        ///            - NewBillingClientCode = C000012025
        ///            - NewBillingOfficeCode = 1000
        ///            - NewBillingTargetCode = C000012025-001
        ///Expected  : Update tbt_BillingTemp : 2 records
        ///            ContractCode = N0012125
        ///            OldBillingClientCode = C000012015
        ///            OldBillingOfficeCode = 1020
        ///            OldBillingTargetCode = C000012015-001
        ///            SendFlag = 0
        ///            
        ///            Keep operation log to log table
        ///            
        ///            See expectation data test case 4
        ///</summary>
        public string Case4()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

            string strContractCode = "N0000012125";
            string strOldBillingClientCode = "C000012015";
            string strOldBillingOfficeCode = "1020";
            string strOldBillingTargetCode = "C000012015-001";
            string strNewBillingClientCode = "C000012025";
            string strNewBillingOfficeCode = "1000";
            string strNewBillingTargetCode = "C000012025-001";

            List<tbt_BillingTemp> expected = null;
            List<tbt_BillingTemp> actual = null;
            string error = string.Empty;

            DateTime dtProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            string strFieldName = "ContractCode OCC SequenceNo BillingOCC BillingTargetRunningNo BillingClientCode BillingTargetCode BillingOfficeCode BillingType CreditTerm BillingTiming BillingAmt PayMethod BillingCycle CalDailyFeeStatus SendFlag UpdateDate UpdateBy";

            try
            {
                actual = target.UpdateBillingTempByBillingTarget(strContractCode, strOldBillingClientCode, strOldBillingOfficeCode, strOldBillingTargetCode
                                                                , strNewBillingClientCode, strNewBillingOfficeCode, strNewBillingTargetCode);

                expected = this.CreateExpectedForCase4(dtProcessDateTime);
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
                bool bResult = CompareObjectList<tbt_BillingTemp>(actual, expected, strFieldName);
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
        ///Expected  : MSG0007: These fields are required: [Contract code], [Billing client code], [Billing office code], [Billing OCC], [Billing target code]
        ///</summary>
        public string Case5()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

            string strContractCode = null;
            string strBillingClientCode = null;
            string strBillingOfficeCode = null;
            string strBillingOCC = null;
            string strBillingTargetCode =  null;

            string expected = "MSG0007";
            string actual;

            try
            {
                target.UpdateBillingTempByBillingClientAndOffice(strContractCode, strBillingClientCode, strBillingOfficeCode, strBillingOCC, strBillingTargetCode);
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
        ///Purpose   : Keep billing data (Edit)
        ///Parameters: - Contract Code = N0000012125
        ///            - Billing Client Code = C000000032
        ///            - Billing Office Code = 1000
        ///            - Billing OCC = 04
        ///            - Billing Target Code =  C000000032-001
        ///Expected  : Update tbt_BillingTemp : 1 records
        ///            Contract Code = N0012125
        ///            Billing Client Code = C000000032
        ///            Billing Office Code = 1000
        ///            
        ///            Keep operation log to log table
        ///            
        ///            See expectation data test case 6
        ///</summary>
        public string Case6()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

            string strContractCode = "N0000012125";
            string strBillingClientCode = "C000000032";
            string strBillingOfficeCode = "1000";
            string strBillingOCC = "04";
            string strBillingTargetCode = "C000000032-001";

            List<tbt_BillingTemp> expected = null;
            List<tbt_BillingTemp> actual = null;
            string error = string.Empty;

            DateTime dtProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            string strFieldName = "ContractCode OCC SequenceNo BillingOCC BillingTargetRunningNo BillingClientCode BillingTargetCode BillingOfficeCode BillingType CreditTerm BillingTiming BillingAmt PayMethod BillingCycle CalDailyFeeStatus SendFlag UpdateDate UpdateBy";

            try
            {
                actual = target.UpdateBillingTempByBillingClientAndOffice(strContractCode, strBillingClientCode, strBillingOfficeCode, strBillingOCC, strBillingTargetCode);

                expected = this.CreateExpectedForCase6(dtProcessDateTime);
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
                bool bResult = CompareObjectList<tbt_BillingTemp>(actual, expected, strFieldName);
                return string.Format(RESULT_FORMAT_LIST, 6, bResult);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 6, "Fail", error);
            }
        }


        private tbt_BillingTemp CreateParameterForCase2(DateTime dtProcessDateTime)
        {
            tbt_BillingTemp data = new tbt_BillingTemp();
            data.ContractCode = "N0000012225";
            data.OCC = "0001";
            data.SequenceNo = 3;
            data.BillingOCC = "05";
            data.BillingTargetRunningNo = "005";
            data.BillingClientCode = "C000012015";
            data.BillingTargetCode = "C000012015-005";
            data.BillingOfficeCode = "1000";
            data.BillingType = "03";
            data.CreditTerm = 90;
            data.BillingTiming = "6";
            data.BillingAmt = 99999999.9999M;
            data.PayMethod = "4";
            data.BillingCycle = 6;
            data.CalDailyFeeStatus = "0";
            data.SendFlag = "1";
            data.UpdateDate = dtProcessDateTime;
            data.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

            return data;
        }

        private tbt_BillingTemp CreateExpectedForCase2(DateTime dtProcessDateTime)
        {
            tbt_BillingTemp data = new tbt_BillingTemp();
            data.ContractCode = "N0000012225";
            data.OCC = "0001";
            data.SequenceNo = 3;
            data.BillingOCC = "05";
            data.BillingTargetRunningNo = "005";
            data.BillingClientCode = "C000012015";
            data.BillingTargetCode = "C000012015-005";
            data.BillingOfficeCode = "1000";
            data.BillingType = "03";
            data.CreditTerm = 90;
            data.BillingTiming = "6";
            data.BillingAmt = 99999999.9999M;
            data.PayMethod = "4";
            data.BillingCycle = 6;
            data.CalDailyFeeStatus = "0";
            data.SendFlag = "1";
            data.UpdateDate = dtProcessDateTime;
            data.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

            return data;
        }

        private List<tbt_BillingTemp> CreateExpectedForCase4(DateTime dtProcessDateTime)
        {
            List<tbt_BillingTemp> dataList = new List<tbt_BillingTemp>();

            tbt_BillingTemp data = new tbt_BillingTemp();
            data.ContractCode = "N0000012125";
            data.OCC = "0002";
            data.SequenceNo = 1;
            data.BillingOCC = "01";
            data.BillingTargetRunningNo = "001";
            data.BillingClientCode = "C000012025";
            data.BillingTargetCode = "C000012025-001";
            data.BillingOfficeCode = "1000";
            data.BillingType = "01";
            data.CreditTerm = 60;
            data.BillingTiming = "1";
            data.BillingAmt = 10000.0000M;
            data.PayMethod = "1";
            data.BillingCycle = 3;
            data.CalDailyFeeStatus = "1";
            data.SendFlag = "0";
            data.UpdateDate = dtProcessDateTime;
            data.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            dataList.Add(data);

            data = new tbt_BillingTemp();
            data.ContractCode = "N0000012125";
            data.OCC = "0003";
            data.SequenceNo = 1;
            data.BillingOCC = "01";
            data.BillingTargetRunningNo = "001";
            data.BillingClientCode = "C000012025";
            data.BillingTargetCode = "C000012025-001";
            data.BillingOfficeCode = "1000";
            data.BillingType = "01";
            data.CreditTerm = 60;
            data.BillingTiming = "1";
            data.BillingAmt = 10000.0000M;
            data.PayMethod = "1";
            data.BillingCycle = 3;
            data.CalDailyFeeStatus = "1";
            data.SendFlag = "0";
            data.UpdateDate = dtProcessDateTime;
            data.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            dataList.Add(data);

            return dataList;
        }

        private List<tbt_BillingTemp> CreateExpectedForCase6(DateTime dtProcessDateTime)
        {
            List<tbt_BillingTemp> dataList = new List<tbt_BillingTemp>();

            tbt_BillingTemp data = new tbt_BillingTemp();
            data.ContractCode = "N0000012125";
            data.OCC = "0002";
            data.SequenceNo = 4;
            data.BillingOCC = "04";
            data.BillingTargetRunningNo = "004";
            data.BillingClientCode = "C000000032";
            data.BillingTargetCode = "C000000032-001";
            data.BillingOfficeCode = "1000";
            data.BillingType = "02";
            data.CreditTerm = 60;
            data.BillingTiming = "3";
            data.BillingAmt = 99999.9999M;
            data.PayMethod = "3";
            data.BillingCycle = 3;
            data.CalDailyFeeStatus = "2";
            data.SendFlag = "0";
            data.UpdateDate = dtProcessDateTime;
            data.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            dataList.Add(data);

            return dataList;
        }
    }
}
