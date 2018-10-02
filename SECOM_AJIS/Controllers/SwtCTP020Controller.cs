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
    public class SwtCTP020Controller : SwtCommonController
    {
        //
        // GET: /SwtCTP020/

        public string index()
        {
            List<string> lst = new List<string>();
            lst.Add(Case1());
            lst.Add(Case2());

            string result = CommonUtil.TextList(lst.ToArray());
            result = result.Replace(", Case", "Case");
            
            return result;
        }

        ///<summary>
        ///Purpose   : Check Mandatory
        ///Parameters: None
        ///Expected  : MSG0007: These fields are required: [Contract code], [OCC], [BillingType], [BillingAmt], [PayMethod] and
        ///            At least one of [BillingOCC or BillingTargetCode or (BillingClientCode and BillingOfficeCode)].
        ///</summary>
        public string Case1()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            tbt_BillingTemp data = null;
            string expected = "MSG0007";
            string actual;
            
            try
            {
                target.InsertBillingTemp(data);
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
        ///Purpose   : Keep billing data (Add)
        ///Parameters: tbt_BillingTemp
        ///             - ContractCode = N0000012125
        ///             - OCC = 9970
        ///             - SequenceNo = NULL
        ///             - BillingOCC = 01
        ///             - BillingTargetRunningNo = 001
        ///             - BillingClientCode = C000012015
        ///             - BillingTargetCode = C000012015-001
        ///             - BillingOfficeCode = 1020
        ///             - BillingType = 01
        ///             - CreditTerm = 60
        ///             - BillingTiming = 1
        ///             - BillingAmt = 10000
        ///             - PayMethod = 1
        ///             - BillingCycle = 3
        ///             - CalDailyFeeStatus = 1
        ///             - SendFlag = 0
        ///             - CreateDate = Date today 
        ///             - CreateBy = 500629
        ///             - UpdateDate = Date today 
        ///             - UpdateBy = 500629
        ///Expected  : See expectation test case 2
        ///</summary>
        public string Case2()
        {
            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
            tbt_BillingTemp data = new tbt_BillingTemp();
            tbt_BillingTemp expected = new tbt_BillingTemp();
            tbt_BillingTemp actual = new tbt_BillingTemp();
            string error = string.Empty;

            DateTime dtProcessDateTime = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

            try
            {
                data = this.CreateParameterForCase2(dtProcessDateTime);
                List<tbt_BillingTemp> resultList = target.InsertBillingTemp(data);
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
                bool bResult = CompareObject<tbt_BillingTemp>(actual, expected);
                return string.Format(RESULT_FORMAT_LIST, 2, bResult);
            }
            else
            {
                return string.Format(RESULT_FORMAT_ERROR, 2, "Fail", error);
            }
        }


        private tbt_BillingTemp CreateParameterForCase2(DateTime dtProcessDateTime)
        {
            tbt_BillingTemp data = new tbt_BillingTemp();
            data.ContractCode = "N0000012125";
            data.OCC = "9970";
            //data.SequenceNo = null;
            data.BillingOCC = "01";
            data.BillingTargetRunningNo = "001";
            data.BillingClientCode = "C000012015";
            data.BillingTargetCode = "C000012015-001";
            data.BillingOfficeCode = "1020";
            data.BillingType = "01";
            data.CreditTerm = 60;
            data.BillingTiming = "1";
            data.BillingAmt = 10000;
            data.PayMethod = "1";
            data.BillingCycle = 3;
            data.CalDailyFeeStatus = "1";
            data.SendFlag = "0";
            data.CreateDate = dtProcessDateTime;
            data.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            data.UpdateDate = dtProcessDateTime;
            data.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

            return data;
        }

        private tbt_BillingTemp CreateExpectedForCase2(DateTime dtProcessDateTime)
        {
            tbt_BillingTemp data = new tbt_BillingTemp();
            data.ContractCode = "N0000012125";
            data.OCC = "9970";
            data.SequenceNo = 1;
            data.BillingOCC = "01";
            data.BillingTargetRunningNo = "001";
            data.BillingClientCode = "C000012015";
            data.BillingTargetCode = "C000012015-001";
            data.BillingOfficeCode = "1020";
            data.BillingType = "01";
            data.CreditTerm = 60;
            data.BillingTiming = "1";
            data.BillingAmt = 10000;
            data.PayMethod = "1";
            data.BillingCycle = 3;
            data.CalDailyFeeStatus = "1";
            data.SendFlag = "0";
            data.CreateDate = dtProcessDateTime;
            data.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            data.UpdateDate = dtProcessDateTime;
            data.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

            return data;
        }


    }
}
